namespace ScopeX.Core.PowerAnalysis
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using NPOI.POIFS.Crypt.Dsig;
    using ScopeX.ComModel;
    using ScopeX.Core.Decode;
    using ScopeX.Core.Tools;
    using ScopeX.Measure;

    internal class PwrInrushCurrentModel : INotifyPropertyChanged
    {
        public class InrushCurrentItem
        {
            private Double _Current;

            public Double Current
            {
                get => _Current;

                set
                {
                    _Current = value;
                    if (!Double.IsNaN(value))
                    {
                        StaBuffer.Insert(value);
                    }
                }
            }

            public readonly StatisticBuffer StaBuffer;

            public InrushCurrentItem(Int32 size = 1000)
            {
                StaBuffer = new(size);
            }
        };

        public PwrInrushCurrentModel(PowerAnalysisModel pam, MeasureModel mm)
        {
            Analysis = pam;
            _Meas = mm;

            _InrushCurrent = new ConcurrentDictionary<String, InrushCurrentItem>();
            foreach (var item in _Items)
            {
                _InrushCurrent.TryAdd(item, new InrushCurrentItem());
            }
        }

        private readonly List<String> _Items = new List<String>() { "Max", "Min", "Pk2Pk" };

        public PowerAnalysisModel Analysis
        {
            get;
        }

        private readonly MeasureModel _Meas;

        private readonly ConcurrentDictionary<String, InrushCurrentItem> _InrushCurrent;

        public InrushCurrentItem this[String key] => _InrushCurrent[key];

        private TestMode _TestMode = TestMode.Single;

        public TestMode TestMode
        {
            get => _TestMode;
            set
            {
                if (_TestMode != value)
                {
                    _TestMode = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _RunFlag = false;
        /// <summary>
        /// 单次分析允许标志
        /// </summary>
        public Boolean RunFlag
        {
            get => _RunFlag;
            set
            {
                if (_RunFlag != value)
                {
                    _RunFlag = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _RunCompleted = true;

        /// <summary>
        /// 运行完成标志
        /// </summary>
        public Boolean RunCompleted
        {
            get => _RunCompleted;
            set
            {
                if (_RunCompleted != value)
                {
                    _RunCompleted = value;
                    OnPropertyChanged();
                }
            }
        }

        public Double MaxCurrent => Constants.MAX_YDIVS_NUM / 2.0 * TryGetChannelMaxConditioningScale(Analysis.CurrentSrc1) * ProtocolModel.TryGetChannelGain(Analysis.CurrentSrc1);
        public Double MinCurrent => -MaxCurrent;

        private Double _PeakCurrent = 1;

        public Double PeakCurrent
        {
            get
            {
                return _PeakCurrent;
            }
            set
            {
                if (_PeakCurrent != value)
                {
                    _PeakCurrent = value;
                    OnPropertyChanged();
                }
            }
        }

        public String Unit
        {
            get
            {
                if(DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Analysis.CurrentSrc1,out var cp)&&cp!=null)
                {
                    return cp.Unit;
                }
                else
                {
                    return QuantityUnit.Ampere.ToUnitString();
                }
            }
        }

        internal static Double TryGetChannelMaxConditioningScale(ChannelId channelId)
        {
            if (!DsoModel.Default.TryGetChannel(channelId, out ChannelModel? channelModel) || channelModel is not AnalogModel analogModel)
            {
                return Double.NaN;
            }
            return analogModel.Conditioning.MaxScale / 1E3;
        }

        /// <summary>
        /// 单次运行流程
        /// </summary>
        private void SingleTestInrush()
        {
            //打开电流源通道，进入触发Stop状态
            if (!DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Analysis.CurrentSrc1, out var chnl) || !(chnl is AnalogPrsnt cur))
            {
                return;
            }
            if (!cur.Active)
            {
                cur.Active = true;
            }

            DsoPrsnt.DefaultDsoPrsnt.Stop();
            //提示"关闭电源,点击下一步"（可点击退出）
            if (StrongTip.Default.Show(MsgTipId.PowerAnalysisInrushCurrent, MsgTipId.PowerAnalysisCloseSourceAndClickNext, MessageType.Information))
            {
                //设置通道属性
                cur.Coupling = AnaChnlCoupling.DC1M;

                cur.Bandwidth = cur.BandWidthNames.Last().Index;

                cur.ScaleBymV = Math.Abs(_PeakCurrent) * 1E3 * 4 / Constants.VIS_YDIVS_NUM;

                cur.PosIndexBymDiv = 0;

                //触发切换为单次触发,切换触发类型为边沿触发
                TriggerModel.Mode = TriggerMode.OneShot;
                var trigprsnt = TriggerPrsnt.GetOrMakeTrigger(DsoPrsnt.DefaultDsoPrsnt, TriggerType.Edge);
                if (trigprsnt != null && trigprsnt is TrigEdgePrsnt edgeprsnt)
                {
                    if (edgeprsnt!.Source != Analysis.CurrentSrc1)
                    {
                        edgeprsnt!.Source = Analysis.CurrentSrc1;
                    }

                    edgeprsnt.Slope = EdgeSlope.Both;
                    edgeprsnt.CompPositionBymV = _PeakCurrent * 1E3 * 0.5;//触发电平为期望值的一半
                }
                //设置高分辨率
                if (DsoPrsnt.DefaultDsoPrsnt.Timebase.Mode != AnaChnlAcqMode.HighRes)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Timebase.Mode = AnaChnlAcqMode.HighRes;
                }

                //设置时基
                if (DsoModel.Default.Timebase.ScaleIndex != DsoModel.Default.Timebase.ScanMinIndex - 1)
                {
                    DsoModel.Default.Timebase.ScaleIndex = DsoModel.Default.Timebase.ScanMinIndex - 1;
                }
                Thread.Sleep(500);
                //提示"打开电源，系统触发后，请点击下一步"（可点击退出）
                if (StrongTip.Default.Show(MsgTipId.PowerAnalysisInrushCurrent, MsgTipId.PowerAnalysisOpenSourceAndClickNext, MessageType.Information))
                {
                    if (TriggerModel.State == SysState.Stop)//只处理单次触发
                    {
                        Double? max = _Meas.Calc.ForceGetResultOrCalc(nameof(MeasParameter.Max), Analysis.CurrentSrc1);
                        Double? min = _Meas.Calc.ForceGetResultOrCalc(nameof(MeasParameter.Min), Analysis.CurrentSrc1);
                        FillData(max, min);
                    }
                    else if (TriggerModel.State == SysState.Ready || TriggerModel.State == SysState.Auto)
                    {
                        //提示"未检测到可用信号，浪涌电流单次测试结束"
                        WeakTip.Default.Write(nameof(PowerAnalysisOpt.InrushCurrent), MsgTipId.PowerAnalysisNoSignal, duration: 2);
                    }

                }
            }
            else
            {
                return;
            }
        }


        private void RealTimeTestInrush()
        {

        }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="max"></param>
        /// <param name="min"></param>
        private void FillData(Double? max, Double? min)
        {
            if (max != null && Double.IsFinite(max.Value) && min != null && Double.IsFinite(min.Value))
            {
                var pk2pk = max.Value - min.Value;
                _InrushCurrent[_Items[0]].Current = max.Value * 1E-3;
                _InrushCurrent[_Items[1]].Current = min.Value * 1E-3;
                _InrushCurrent[_Items[2]].Current = pk2pk * 1E-3;
            }
            else
            {
                _InrushCurrent[_Items[0]].Current = Double.NaN;
                _InrushCurrent[_Items[1]].Current = Double.NaN;
                _InrushCurrent[_Items[2]].Current = Double.NaN;
            }
        }

        //Run方法，执行计算的操作
        public void Run()
        {
            if (RunFlag && RunCompleted)
            {
                _ = SingleRun();
            }
        }
        public async Task SingleRun()
        {
            try
            {
                await Task.Run(() => InrushCurrentAnalysis());
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }

        private void InrushCurrentAnalysis()
        {
            RunCompleted = false;
            PowerAnalysisTools.PwrFlagOther(RunFlag);
            RunAnalysis();
            RunFlag = false;
            PowerAnalysisTools.PwrFlagOther(RunFlag);
            RunCompleted = true;
        }

        private void RunAnalysis()
        {
            if (_TestMode == TestMode.Single)
            {
                SingleTestInrush();
            }
        }

        public void Reset()
        {
            foreach (var p in _InrushCurrent)
            {
                p.Value.Current= Double.NaN;
                p.Value.StaBuffer.Clear();
            }
        }

        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
            }
        }


        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

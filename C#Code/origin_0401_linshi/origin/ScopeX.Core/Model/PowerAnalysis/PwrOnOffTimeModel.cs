using NPOI.HSSF.Record.CF;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;
using ScopeX.ComModel;
using ScopeX.Core.Decode;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.Measure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core.PowerAnalysis
{
    internal class PwrOnOffTimeModel : INotifyPropertyChanged
    {
        public class OnOffTimeItem
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

            public OnOffTimeItem(Int32 size = 1000)
            {
                StaBuffer = new(size);
            }
        };

        public PwrOnOffTimeModel(PowerAnalysisModel pam, MeasureModel mm)
        {
            Analysis = pam;
            _Meas = mm;

            _OnOffTimes = new ConcurrentDictionary<String, OnOffTimeItem>();

            for (Int32 i = 0, l = _Items.Count; i < l; i++)
            {
                _OnOffTimes.TryAdd(_Items[i], new OnOffTimeItem());
            }
            Count = _OnOffTimes.Count;
        }


        private readonly List<String> _Items = new List<String>() { "TurnOnTime", "TurnOffTime" };

        public PowerAnalysisModel Analysis
        {
            get;
        }

        private readonly MeasureModel _Meas;

        public readonly String Titles = "Value,Average,Maximum,Minimum";

        private readonly ConcurrentDictionary<String, OnOffTimeItem> _OnOffTimes;

        public OnOffTimeItem this[String key] => _OnOffTimes[key];

        public readonly Int32 Count;

        /// <summary>
        /// 电源器件转换类型
        /// </summary>
        private TurnOnOffType _Type = TurnOnOffType.AC2DC;
        public TurnOnOffType Type
        {
            get
            {
                return _Type;
            }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 测试类型（开启或者关闭时间）
        /// </summary>
        private TurnOnOffTestType _TestType = TurnOnOffTestType.TurnOn;
        public TurnOnOffTestType TestType
        {
            get
            {
                return _TestType;
            }
            set
            {
                if (_TestType != value)
                {
                    _TestType = value;
                    OnPropertyChanged();
                }
            }
        }

        //默认C1
        public ChannelId InVoltageSrc
        {
            get => Analysis.VoltageSrc1;
            set
            {
                if (Analysis.VoltageSrc1 != value)
                {
                    Analysis.VoltageSrc1 = value;
                    OnPropertyChanged();
                }
            }
        }

        //默认C2
        public ChannelId OutVoltageSrc
        {
            get => Analysis.CurrentSrc1;
            set
            {
                if (Analysis.CurrentSrc1 != value)
                {
                    Analysis.CurrentSrc1 = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxAcquisitionTime = 1E3;//1000s

        public readonly Double MinAcquisitionTime = 0.2;//最少200ms

        private Double _AcquisitionTime = 1;

        public Double AcquisitionTime
        {
            get => _AcquisitionTime;
            set
            {
                if (_AcquisitionTime != value)
                {
                    _AcquisitionTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public Double MaxInPeakVoltage => Constants.MAX_YDIVS_NUM / 2.0 * PwrInrushCurrentModel.TryGetChannelMaxConditioningScale(InVoltageSrc) * ProtocolModel.TryGetChannelGain(InVoltageSrc);

        public Double MinInPeakVoltage => -MaxInPeakVoltage;

        private Double _InPeakVoltage = 50;

        public Double InPeakVoltage
        {
            get => _InPeakVoltage;
            set
            {
                if (_InPeakVoltage != value)
                {
                    _InPeakVoltage = value;
                    OnPropertyChanged();
                }
            }
        }

        public Double MaxOutPeakVoltage => Constants.MAX_YDIVS_NUM / 2.0 * PwrInrushCurrentModel.TryGetChannelMaxConditioningScale(OutVoltageSrc) * ProtocolModel.TryGetChannelGain(OutVoltageSrc);

        public Double MinOutPeakVoltage => -MaxOutPeakVoltage;

        private Double _OutPeakVoltage = 5;

        public Double OutPeakVoltage
        {
            get => _OutPeakVoltage;
            set
            {
                if (_OutPeakVoltage != value)
                {
                    _OutPeakVoltage = value;
                    OnPropertyChanged();
                }
            }
        }

        private Stopwatch _CheckSW = new Stopwatch();

        private void TurnOnOffAnalysis()
        {
            //打开输入电压源和输出电压源通道，进入触发Stop状态
            if (!DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(InVoltageSrc, out var inchnl) || !(inchnl is AnalogPrsnt invol))
            {
                return;
            }

            if (!DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(OutVoltageSrc, out var outchnl) || !(outchnl is AnalogPrsnt outvol))
            {
                return;
            }

            if (invol == null || outvol == null)
            {
                return;
            }

            if (!invol.Active)
            {
                invol.Active = true;
            }
            if (!outvol.Active)
            {
                outvol.Active = true;
            }
            DsoPrsnt.DefaultDsoPrsnt.Stop();
            //提示"关闭电源,点击下一步"（可点击退出）
            Double rst = Double.NaN;
            if (_TestType == TurnOnOffTestType.TurnOn)
            {
                rst = TurnOnAnalysis(invol, outvol);

                _OnOffTimes[_Items[0]].Current = rst;
            }
            else if (_TestType == TurnOnOffTestType.TurnOff)
            {
                rst = TurnOffAnalysis(invol, outvol);
                _OnOffTimes[_Items[1]].Current = rst;
            }
        }

        private Double TurnOnAnalysis(AnalogPrsnt invol, AnalogPrsnt outvol)
        {
            var turn_on_time = Double.NaN;
            if (StrongTip.Default.Show(MsgTipId.PowerAnalysisTurnOnTime, MsgTipId.PowerAnalysisCloseSourceAndClickNext, MessageType.Information))
            {
                CommonSetting(invol, outvol);
                //触发切换为单次触发,切换触发类型为边沿触发
                var trigprsnt = TriggerPrsnt.GetOrMakeTrigger(DsoPrsnt.DefaultDsoPrsnt, TriggerType.Edge);
                if (trigprsnt != null && trigprsnt is TrigEdgePrsnt edgeprsnt)
                {
                    if (edgeprsnt!.Source != OutVoltageSrc)
                    {
                        edgeprsnt!.Source = OutVoltageSrc;
                    }
                    //开启输出正电压一定是上升沿，负电压一定是下降沿
                    if (_OutPeakVoltage >= 0)
                    {
                        edgeprsnt.Slope = EdgeSlope.Rise;
                    }
                    else
                    {
                        edgeprsnt.Slope = EdgeSlope.Fall;
                    }

                    edgeprsnt.CompPositionBymV = _OutPeakVoltage * 1E3 * 0.5;//触发电平为期望值的一半
                }
                TriggerModel.Mode = TriggerMode.OneShot;

                Thread.Sleep(100);
                if (_AcquisitionTime > 1)//采集时间大于1秒需要做判定
                {
                    WeakTip.Default.Write(nameof(PowerAnalysisOpt.TurnOnOff), MsgTipId.PowerAnalysisWaitForTriggerReady, false, "", Int32.MaxValue);
                    WeakTip.Default.Enabled = false;
                    while (TriggerModel.State == SysState.Armed || TriggerModel.State == SysState.Scan)
                    {
                        Thread.Sleep(10);
                    }
                    WeakTip.Default.Enabled = true;
                }
                WeakTip.Default.Close();
                //提示"打开电源，系统触发后，请点击下一步"（可点击退出）
                if (StrongTip.Default.Show(MsgTipId.PowerAnalysisTurnOnTime, MsgTipId.PowerAnalysisOpenSourceAndClickNext, MessageType.Information))
                {
                    var checktime = _AcquisitionTime * 0.75 * 1E3;
                    _CheckSW.Restart();
                    while (TriggerModel.State != SysState.Stop)
                    {
                        if (_AcquisitionTime > 1)
                        {
                            WeakTip.Default.Write(nameof(PowerAnalysisOpt.TurnOnOff), MsgTipId.PowerAnalysisWaitForTriggerCompleted, false, "", Int32.MaxValue);
                            WeakTip.Default.Enabled = false;
                        }
                        Thread.Sleep(100);
                        _CheckSW.Stop();
                        if (_CheckSW.ElapsedMilliseconds > checktime)
                        {
                            break;
                        }
                        else
                        {
                            _CheckSW.Start();
                        }
                    }
                    WeakTip.Default.Close();
                    WeakTip.Default.Enabled = true;
                    _CheckSW.Stop();
                    Thread.Sleep(1000);
                    if (TriggerModel.State == SysState.Stop)//只处理单次触发
                    {
                        var in_vol_pkg = DsoModel.Default.GetWfmPack(InVoltageSrc);
                        var out_vol_pkg = DsoModel.Default.GetWfmPack(OutVoltageSrc);
                        if (in_vol_pkg?.Buffer == null || out_vol_pkg?.Buffer == null)
                        {
                            EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs("TurnOnAnalysis Get Channel Data Is Null！！！", EventBus.LogLevel.Error));
                            return turn_on_time;
                        }
                        Double[] in_vol_data = in_vol_pkg.Buffer.Cast<Double>().Select(x => x * 1E-3).ToArray();
                        Double[] out_vol_data = out_vol_pkg.Buffer.Cast<Double>().Select(x => x * 1E-3).ToArray();

                        if (_Type == TurnOnOffType.AC2DC)
                        {
                            turn_on_time = TurnOn_AC2DC(in_vol_data, out_vol_data);
                            WeakTip.Default.Write(nameof(PowerAnalysisOpt.TurnOnOff), MsgTipId.AC2DCTurnOnCompleted, false, "", 1);
                        }
                        else if (_Type == TurnOnOffType.DC2DC)
                        {
                            turn_on_time = TurnOn_DC2DC();
                            WeakTip.Default.Write(nameof(PowerAnalysisOpt.TurnOnOff), MsgTipId.DC2DCTurnOnCompleted, false, "", 1);
                        }
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
                return turn_on_time;
            }

            return turn_on_time;
        }


        #region 开启时间
        private Double TurnOn_AC2DC(Double[] involdata, Double[] outvoldata)
        {
            Double? start_index = null, end_index = null;
            Double turn_on_time = Double.NaN;

            Int32 in_start_index = 0;

            //输出：找到上升沿位置取end

            if (_OutPeakVoltage > 0)
            {
                var out_rise = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.RiseEdges), OutVoltageSrc);
                if (out_rise != null && out_rise.Count > 0)
                {
                    end_index = out_rise.FirstOrDefault().End;
                }
            }
            else
            {
                var out_fall = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.FallEdges), OutVoltageSrc);
                if (out_fall != null && out_fall.Count > 0)
                {
                    end_index = out_fall.FirstOrDefault().End;
                }
            }

            #region 输入开启位置

            var threshold = 0.1;
            start_index = FindACTurnOnLocation(involdata, threshold);
            if (start_index == 0)
            {
                start_index = null;
            }
            #endregion



            //计算结果
            if (start_index != null && end_index != null)
            {
                Debug.WriteLine($"{start_index} {end_index}");
                var chnl = DsoModel.Default.GetChannel(InVoltageSrc);
                var pkg = chnl.Pack;

                if (pkg != null)
                {
                    turn_on_time = (end_index.Value - start_index.Value) * pkg.Properties.SampInterval;
                }
            }

            return turn_on_time;
        }


        private Double FindACTurnOnLocation(Double[] involdata, Double threshold)
        {
            Double turn_on_location = 0;
            Double max = Double.MinValue;
            for (Int32 i = 0, l = involdata.Length; i < l; i++)
            {
                if (max < Math.Abs(involdata[i]))
                {
                    max = Math.Abs(involdata[i]);
                }
            }

            var indexes = FindACOffIndexes(involdata, threshold, max);

            // 计算差分
            for (Int32 i = indexes.EndIndex; i < involdata.Length - 1; i++)
            {
                if (turn_on_location == 0)
                {
                    var temp = Math.Abs(involdata[i] / max);
                    if (temp > threshold)
                    {
                        turn_on_location = i;//找到AC关闭的区域
                        i--;
                        if (i < indexes.EndIndex)
                        {
                            break;
                        }

                    }
                }
                else//继续往前找到第一个大于等于最大值10%的位置
                {
                    if (Math.Abs(involdata[i]) >= max * 0.1)
                    {
                        turn_on_location = i;
                        break;
                    }
                }
            }

            return turn_on_location;
        }
        private Double TurnOn_DC2DC()
        {
            Double? start_index = null, end_index = null;
            Double turn_on_time = Double.NaN;
            #region 输入开启位置
            var in_rise = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.RiseEdges), InVoltageSrc);
            var in_fall = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.FallEdges), InVoltageSrc);
            if (InPeakVoltage > 0)
            {
                if (in_rise != null && in_rise.Count > 0)
                {
                    start_index = in_rise.FirstOrDefault().Start;
                }
            }
            else
            {
                if (in_fall != null && in_fall.Count > 0)
                {
                    start_index = in_fall.FirstOrDefault().Start;
                }
            }
            #endregion

            #region 输出开启位置
            var out_rise = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.RiseEdges), OutVoltageSrc);
            var out_fall = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.FallEdges), OutVoltageSrc);
            if (OutPeakVoltage >= 0)
            {
                if (out_rise != null && out_rise.Count > 0)
                {
                    end_index = out_rise.FirstOrDefault().End;
                }
            }
            else
            {
                if (out_fall != null && out_fall.Count > 0)
                {
                    end_index = out_fall.FirstOrDefault().End;
                }
            }
            #endregion

            //计算结果
            if (start_index != null && end_index != null)
            {
                var chnl = DsoModel.Default.GetChannel(InVoltageSrc);
                var pkg = chnl.Pack;

                if (pkg != null)
                {
                    turn_on_time = (end_index.Value - start_index.Value) * pkg.Properties.SampInterval;
                }
            }

            return turn_on_time;
        }

        #endregion



        private Double TurnOffAnalysis(AnalogPrsnt invol, AnalogPrsnt outvol)
        {
            var turn_off_time = Double.NaN;
            if (StrongTip.Default.Show(MsgTipId.PowerAnalysisTurnOffTime, MsgTipId.PowerAnalysisOpenSourceAndClickNext, MessageType.Information))
            {
                CommonSetting(invol, outvol);
                //触发切换为单次触发,切换触发类型为边沿触发
                var trigprsnt = TriggerPrsnt.GetOrMakeTrigger(DsoPrsnt.DefaultDsoPrsnt, TriggerType.Edge);
                if (trigprsnt != null && trigprsnt is TrigEdgePrsnt edgeprsnt)
                {
                    if (edgeprsnt!.Source != OutVoltageSrc)
                    {
                        edgeprsnt!.Source = OutVoltageSrc;
                    }

                    if (_OutPeakVoltage >= 0)
                    {
                        edgeprsnt.Slope = EdgeSlope.Fall;//开启输出一定是上升沿
                    }
                    else
                    {
                        edgeprsnt.Slope = EdgeSlope.Rise;
                    }
                    edgeprsnt.CompPositionBymV = _OutPeakVoltage * 1E3 * 0.5;//触发电平为期望值的一半
                }
                TriggerModel.Mode = TriggerMode.OneShot;

                Thread.Sleep(100);
                if (_AcquisitionTime > 1)//采集时间大于1秒需要做判定
                {
                    WeakTip.Default.Write(nameof(PowerAnalysisOpt.TurnOnOff), MsgTipId.PowerAnalysisWaitForTriggerReady, false, "", Int32.MaxValue);
                    WeakTip.Default.Enabled = false;
                    while (TriggerModel.State == SysState.Armed || TriggerModel.State == SysState.Scan)
                    {
                        Thread.Sleep(10);
                    }
                    WeakTip.Default.Enabled = true;
                }
                WeakTip.Default.Close();
                //提示"关闭电源，系统触发后，请点击下一步"（可点击退出）
                if (StrongTip.Default.Show(MsgTipId.PowerAnalysisTurnOffTime, MsgTipId.PowerAnalysisCloseSourceAndClickNext, MessageType.Information))
                {
                    var checktime = _AcquisitionTime * 0.75 * 1E3;
                    _CheckSW.Restart();
                    while (TriggerModel.State != SysState.Stop)
                    {
                        if (_AcquisitionTime > 1)
                        {
                            WeakTip.Default.Write(nameof(PowerAnalysisOpt.TurnOnOff), MsgTipId.PowerAnalysisWaitForTriggerCompleted, false, "", Int32.MaxValue);
                            WeakTip.Default.Enabled = false;
                        }
                        Thread.Sleep(100);
                        _CheckSW.Stop();
                        if (_CheckSW.ElapsedMilliseconds > checktime)
                        {
                            break;
                        }
                        else
                        {
                            _CheckSW.Start();
                        }
                    }
                    WeakTip.Default.Enabled = true;
                    WeakTip.Default.Close();
                    _CheckSW.Stop();
                    Thread.Sleep(1000);
                    if (TriggerModel.State == SysState.Stop)//只处理单次触发
                    {
                        var in_vol_pkg = DsoModel.Default.GetWfmPack(InVoltageSrc);
                        var out_vol_pkg = DsoModel.Default.GetWfmPack(OutVoltageSrc);
                        if (in_vol_pkg?.Buffer == null || out_vol_pkg?.Buffer == null)
                        {
                            EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs("TurnOffAnalysis Get Channel Data Is Null！！！", EventBus.LogLevel.Error));
                            return turn_off_time;
                        }
                        Double[] in_vol_data = in_vol_pkg.Buffer.Cast<Double>().Select(x => x * 1E-3).ToArray();
                        Double[] out_vol_data = out_vol_pkg.Buffer.Cast<Double>().Select(x => x * 1E-3).ToArray();

                        if (_Type == TurnOnOffType.AC2DC)
                        {
                            turn_off_time = TurnOff_AC2DC(in_vol_data, out_vol_data);
                            WeakTip.Default.Write(nameof(PowerAnalysisOpt.TurnOnOff), MsgTipId.AC2DCTurnOffCompleted, false, "", 1);
                        }
                        else if (_Type == TurnOnOffType.DC2DC)
                        {
                            turn_off_time = TurnOff_DC2DC();
                            WeakTip.Default.Write(nameof(PowerAnalysisOpt.TurnOnOff), MsgTipId.DC2DCTurnOffCompleted, false, "", 1);
                        }
                    }
                    else if (TriggerModel.State == SysState.Armed || TriggerModel.State == SysState.Ready || TriggerModel.State == SysState.Auto)
                    {
                        //提示"未检测到可用信号，单次测试结束"
                        WeakTip.Default.Write(nameof(PowerAnalysisOpt.InrushCurrent), MsgTipId.PowerAnalysisNoSignal, duration: 2);
                    }

                }
            }
            else
            {
                return turn_off_time;
            }

            return turn_off_time;



        }
        #region 关闭时间
        private Double TurnOff_AC2DC(Double[] involdata, Double[] outvoldata)
        {
            Double? start_index = null, end_index = null;
            Double turn_off_time = Double.NaN;

            //输出：找到上升沿位置取end
            if (OutPeakVoltage > 0)
            {
                var out_fall = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.FallEdges), OutVoltageSrc);
                if (out_fall != null && out_fall.Count > 0)
                {
                    end_index = out_fall.FirstOrDefault().End;
                }
            }
            else
            {
                var out_raise = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.RiseEdges), OutVoltageSrc);
                if (out_raise != null && out_raise.Count > 0)
                {
                    end_index = out_raise.FirstOrDefault().End;
                }
            }

            #region 输入开启位置
            var threshold = 0.1;
            start_index = FindACTurnOffLocation(involdata, threshold);
            if (start_index == 0)
            {
                start_index = null;
            }
            #endregion



            //计算结果
            if (start_index != null && end_index != null)
            {
                var chnl = DsoModel.Default.GetChannel(InVoltageSrc);
                var pkg = chnl.Pack;

                if (pkg != null)
                {
                    turn_off_time = (end_index.Value - start_index.Value) * pkg.Properties.SampInterval;
                }
            }

            return turn_off_time;
        }

        private Double FindACTurnOffLocation(Double[] involdata, Double threshold)
        {
            Double turn_off_location = 0;
            Double max = Double.MinValue;
            for (Int32 i = 0, l = involdata.Length; i < l; i++)
            {
                if (max < Math.Abs(involdata[i]))
                {
                    max = Math.Abs(involdata[i]);
                }
            }

            var indexes = FindACOffIndexes(involdata, threshold, max);

            // 计算差分
            for (Int32 i = indexes.StarIndex; i >= 0; i--)
            {
                if (turn_off_location == 0)
                {
                    var temp = Math.Abs(involdata[i] / max);
                    if (temp > threshold)
                    {
                        turn_off_location = i;//找到AC关闭的区域
                        i++;
                        if (i > indexes.StarIndex)
                        {
                            break;
                        }
                    }
                }
                else//继续往前找到第一个大于等于最大值10%的位置
                {
                    if (Math.Abs(involdata[i]) >= max * 0.1)
                    {
                        turn_off_location = i;
                        break;
                    }
                }
            }


            return turn_off_location;
        }

        private Double TurnOff_DC2DC()
        {
            Double? start_index = null, end_index = null;
            Double turn_off_time = Double.NaN;
            #region 输入开启位置
            var in_fall = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.FallEdges), InVoltageSrc);
            var in_rise = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.Rise), InVoltageSrc);
            if (InPeakVoltage > 0)
            {
                if (in_fall != null && in_fall.Count > 0)
                {
                    start_index = in_fall.FirstOrDefault().End;
                }
            }
            else if (InPeakVoltage < 0)
            {
                if (in_rise != null && in_rise.Count > 0)
                {
                    start_index = in_rise.FirstOrDefault().End;
                }
            }
            #endregion

            #region 输出开启位置
            var out_fall = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.FallEdges), OutVoltageSrc);
            var out_rise = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.RiseEdges), OutVoltageSrc);
            if (OutPeakVoltage > 0)
            {
                if (out_fall != null && out_fall.Count > 0)
                {
                    end_index = out_fall.FirstOrDefault().End;
                }
            }
            else if (OutPeakVoltage < 0)
            {
                if (out_rise != null && out_rise.Count > 0)
                {
                    end_index = out_rise.FirstOrDefault().End;
                }
            }
            #endregion

            //计算结果
            if (start_index != null && end_index != null)
            {
                var chnl = DsoModel.Default.GetChannel(InVoltageSrc);
                var pkg = chnl.Pack;

                if (pkg != null)
                {
                    turn_off_time = (end_index.Value - start_index.Value) * pkg.Properties.SampInterval;
                }
            }

            return turn_off_time;
        }

        #endregion
        /// <summary>
        /// 寻找稳态区间
        /// </summary>
        /// <param name="involdata"></param>
        /// <param name="threshold"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private (Int32 StarIndex, Int32 EndIndex) FindACOffIndexes(Double[] involdata, Double threshold, Double max)
        {
            Byte[] reshaped = new Byte[involdata.Length];
            for (Int32 i = 0, l = involdata.Length; i < l; i++)
            {
                if (max < Math.Abs(involdata[i]))
                {
                    max = Math.Abs(involdata[i]);
                }
            }

            for (Int32 i = 0, l = involdata.Length; i < l; i++)
            {
                var value = Math.Abs(involdata[i] / max);
                if (value > threshold)
                {
                    reshaped[i] = 1;
                }
                else
                {
                    reshaped[i] = 0;
                }
            }
            //从前往后找到，连续10个0的起始位置
            Int32 count = 0;
            Int32 start_index = 0;
            for (Int32 i = 0; i < reshaped.Length - 1; i++)
            {
                if (reshaped[i] == 0)
                {
                    count++;
                }
                else
                {
                    count = 0;
                }
                if (count == 100)
                {
                    start_index = i;
                    break;
                }
            }

            //从后往前找，找到连续10个0的起始位置
            count = 0;
            Int32 end_index = reshaped.Length - 1;
            for (Int32 i = reshaped.Length - 1; i >= 0; i--)
            {
                if (reshaped[i] == 0)
                {
                    count++;
                }
                else
                {
                    count = 0;
                }
                if (count == 100)
                {
                    end_index = i - 10;
                    break;
                }
            }

            return (start_index, end_index);
        }

        private void CommonSetting(AnalogPrsnt invol, AnalogPrsnt outvol)
        {
            //设置通道属性
            invol.Coupling = AnaChnlCoupling.DC1M;

            invol.Bandwidth = invol.BandWidthNames.Last().Index;

            var invol_scale = Math.Abs(_InPeakVoltage) * 1E3 * 4 / Constants.VIS_YDIVS_NUM;
            if (invol_scale > invol.MaxScale)
            {
                invol_scale = invol.MaxScale;
            }
            if (invol_scale < invol.MinScale)
            {
                invol_scale = invol.MinScale;
            }

            invol.ScaleBymV = invol_scale;

            invol.PosIndexBymDiv = 0;

            //设置通道属性
            outvol.Coupling = AnaChnlCoupling.DC1M;

            outvol.Bandwidth = outvol.BandWidthNames.Last().Index;

            var outvol_scale = Math.Abs(_OutPeakVoltage) * 1E3 * 4 / Constants.VIS_YDIVS_NUM;

            if (outvol_scale > outvol.MaxScale)
            {
                outvol_scale = outvol.MaxScale;
            }
            if (outvol_scale < outvol.MinScale)
            {
                outvol_scale = outvol.MinScale;
            }

            outvol.ScaleBymV = outvol_scale;

            outvol.PosIndexBymDiv = 0;

            //设置高分辨率
            if (DsoPrsnt.DefaultDsoPrsnt.Timebase.Mode != AnaChnlAcqMode.HighRes)
            {
                DsoPrsnt.DefaultDsoPrsnt.Timebase.Mode = AnaChnlAcqMode.HighRes;
            }

            //设置时基
            var timebase = _AcquisitionTime * 1E6 / Constants.VIS_XDIVS_NUM;
            var timescaleindex = DsoModel.Default.Timebase.GetTimebaseIndexByValue(timebase);
            if (timescaleindex != DsoModel.Default.Timebase.ScaleIndex)
            {
                DsoModel.Default.Timebase.ScaleIndex = timescaleindex;
            }
            DsoPrsnt.DefaultDsoPrsnt.Timebase.ResetPosIndex();
        }

        public readonly String Unit = QuantityUnit.Voltage.ToUnitString();

        private Boolean _Statistics = true;
        public Boolean Statistics
        {
            get => _Statistics;
            set
            {
                if (_Statistics != value)
                {
                    _Statistics = value;
                    OnPropertyChanged();
                }
            }
        }
        private Boolean _RunCompleted = true;
        public Boolean RunCompleted
        {
            get { return _RunCompleted; }
            set
            {
                if (_RunCompleted != value)
                {
                    _RunCompleted = value;
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

        public void Run()
        {
            if (RunFlag && RunCompleted)
            {
                _ = SingleRun();
            }
        }

        public async Task SingleRun()
        {
            await Task.Run(() => RunAnalysis());
        }

        private void RunAnalysis()
        {
            RunCompleted = false;
            PowerAnalysisTools.PwrFlagOther(RunFlag);
            try
            {
                TurnOnOffAnalysis();
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
            finally
            {
                RunFlag = false;
                PowerAnalysisTools.PwrFlagOther(RunFlag);
                RunCompleted = true;
            }
        }

        public void Reset()
        {
            foreach (var p in _OnOffTimes)
            {
                p.Value.Current = Double.NaN;
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

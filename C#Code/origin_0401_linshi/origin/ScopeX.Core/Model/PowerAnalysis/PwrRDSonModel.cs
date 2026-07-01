using NPOI.SS.Formula;
using ScopeX.ComModel;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core.Tools;
using ScopeX.Measure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    internal class PwrRDSonModel : INotifyPropertyChanged
    {
        public class RDSonItems
        {
            private Double? _Current;

            public Double? Current
            {
                get => _Current;

                set
                {
                    _Current = value;
                    if (value != null && !Double.IsNaN(value.Value))
                    {
                        StaBuffer.Insert(value.Value);
                    }
                }
            }

            public readonly StatisticBuffer StaBuffer;

            public RDSonItems(Int32 size = 1000)
            {
                StaBuffer = new(size);
            }
        };

        public PwrRDSonModel(PowerAnalysisModel pam, MeasureModel mm)
        {
            Analysis = pam;
            _Meas = mm;

            _RDSons = new ConcurrentDictionary<String, RDSonItems>();
            foreach (var item in _Items)
            {
                _RDSons.TryAdd(item, new RDSonItems());
            }
            Count = _RDSons.Count;

            _Formula = $"{MathType.Custom}:Execute.Abs({Analysis.VoltageSrc1})/Execute.Abs({Analysis.CurrentSrc1})";
        }

        public void UpdateSource()
        {
            Formula = $"{MathType.Custom}:Execute.Abs({Analysis.VoltageSrc1})/Execute.Abs({Analysis.CurrentSrc1})";
        }

        private readonly List<String> _Items = new List<String>() { "Value" };

        public PowerAnalysisModel Analysis
        {
            get;
        }

        private readonly MeasureModel _Meas;

        private readonly ConcurrentDictionary<String, RDSonItems> _RDSons;
        public readonly String Titles = "Value,Average,Maxumum,Minimum";
        public RDSonItems this[String key] => _RDSons[key];

        public readonly Int32 Count;

        private (Prefix Prefix, String Name) _Unit = (Prefix.Empty, QuantityUnit.Ohm.ToUnitString());
        public (Prefix Prefix, String Name) Unit
        {
            get => _Unit;
            set
            {
                if (_Unit != value)
                {
                    _Unit = value;
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

        //所有导通区间(导通区间特征：电压稳态，电流上升)
        private List<(Int32 StartIndex, Int32 EndIndex)> _Conductions = new List<(Int32 StartIndex, Int32 EndIndex)>();

        //每个导通区间的rdson
        private List<List<Double>> _Rdsons = new List<List<Double>>();

        //每个稳态区间里面的最小rdson
        private List<Double> _MinRdsons = new List<Double>();

        private void CalcPwrRDSon()
        {
            _Conductions.Clear();
            _Rdsons.Clear();
            _MinRdsons.Clear();
            if (DsoModel.Default.TryGetChannel(Analysis.VoltageSrc1, out var volmodel) && volmodel is AnalogModel vol
               && DsoModel.Default.TryGetChannel(Analysis.VoltageSrc1, out var curmodel) && curmodel is AnalogModel cur)
            {
                //检查通道是否打开,因为是实时分析，所以主动不打开通道
                if (vol.Active && cur.Active)
                {
                    //1.找到电压源多个<下降沿，上升沿>位置
                    var volpkg = DsoModel.Default.GetWfmPack(Analysis.VoltageSrc1);
                    var curpkg = DsoModel.Default.GetWfmPack(Analysis.CurrentSrc1);
                    if (volpkg?.Buffer != null && curpkg?.Buffer != null)
                    {
                        Double[] voldata = volpkg.Buffer.Cast<Double>().Select(x => x * 1E-3).ToArray();
                        Double[] curdata = curpkg.Buffer.Cast<Double>().Select(x => x * 1E-3).ToArray();
                        if (voldata.Length > 0 && curdata.Length > 0)
                        {
                            try
                            {
                                _Conductions = CalcConductionIndexes(Analysis.VoltageSrc1, Analysis.CurrentSrc1, voldata);

                                //只需要认定下降沿和上升沿中间区域为稳态区间就可以进行计算
                                if (_Conductions != null && _Conductions.Count > 0)
                                {
                                    //2.遍历<下降沿，上升沿>键值对

                                    foreach (var indexes in _Conductions)
                                    {
                                        List<Double> rdsons = new List<Double>();
                                        var start = indexes.StartIndex;
                                        var end = indexes.EndIndex;
                                        for (Int32 i = start; i < end; i++)
                                        {
                                            var vol_value = Math.Abs(voldata[i]);
                                            var cur_value = Math.Abs(curdata[i]);
                                            if (cur_value > 0 && vol_value > 0)
                                            {
                                                rdsons.Add(vol_value / cur_value);
                                            }
                                        }
                                        _Rdsons.Add(rdsons);
                                    }

                                    //3.找到每个区域中最小值的点 
                                    foreach (var rdsons in _Rdsons)
                                    {
                                        if (rdsons.Count > 0)
                                        {
                                            _MinRdsons.Add(rdsons.Min());
                                        }
                                    }

                                    if (_MinRdsons.Count > 0)
                                    {
                                        _RDSons[_Items[0]].Current = _MinRdsons.Average();
                                    }
                                    else
                                    {
                                        _RDSons[_Items[0]].Current = 0;
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                String msg = $"{e.Message} \r\n {e.StackTrace}";
                                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(msg, EventBus.LogLevel.Debug));
                            }
                        }
                    }
                }
                else
                {
                    _RDSons[_Items[0]].Current = Double.NaN;
                }
            }
        }

        private List<(Int32 StartIndex, Int32 EndIndex)> CalcConductionIndexes(ChannelId vol, ChannelId cur, Double[] volData)
        {
            List<(Int32 StartIndex, Int32 EndIndex)> values = new List<(Int32 StartIndex, Int32 EndIndex)>();
            //导通区间由下降沿-稳态-上升沿组成负脉宽
            var negs = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.NegPulses), vol, 50, 10, 75);

            if (negs != null)
            {
                //提取下降沿结束位置和上升沿开始位置
                foreach (var neg in negs)
                {
                    //var std = CalcStd(volData, neg.Start, neg.End);

                    //if (std <= 0.1)
                    {
                        values.Add(((Int32)neg.Start, (Int32)neg.End));
                    }
                }

            }

            return values;
        }

        /// <summary>
        /// 使用标准差判定下降沿结束和上升沿开始区间内的电压是否处于稳态
        /// </summary>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private static Double CalcStd(Double[] data, Double start, Double end)
        {
            if (data.Length == 0)
            {
                return Double.MaxValue;
            }

            Int32 data_start = (Int32)start;
            Int32 data_end = (Int32)end + 1;
            Double mean = 0;//计算均值
            Int32 length = 0;
            for (Int32 i = data_start; i <= data_end; i++)
            {
                mean += data[i];
                length++;
            }
            mean = mean / length;
            Double var = 0;
            for (Int32 i = data_start; i <= data_end; i++)
            {
                var += Math.Pow(data[i] - mean, 2);
            }
            var = var / length;
            var = Math.Sqrt(var);
            return var;
        }

        private Boolean _Statistics;
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


        //Run方法，执行计算的操作
        public void Run()
        {
            if (_RunCompleted)
            {
                _ = SingleRun();
            }
        }

        public async Task SingleRun()
        {
            try
            {
                await Task.Run(() => RDsonAnalysis());
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }


        private void RDsonAnalysis()
        {
            _RunCompleted = false;
            CalcPwrRDSon();
            _RunCompleted = true;
            Thread.Sleep(10);
        }

        public void Reset()
        {
            foreach (var p in _RDSons)
            {
                p.Value.Current = Double.NaN;
                p.Value.StaBuffer.Clear();
            }
        }

        public ChannelId BoundMathId
        {
            get;
            set;
        } = ChannelId.M1;

        private String _Formula;
        public String Formula
        {
            get
            {
                return _Formula;
            }
            private set
            {
                if (_Formula != value)
                {
                    _Formula = value;
                    if (BoundMathId != null && DsoModel.Default != null && DsoModel.Default.TryGetChannel(BoundMathId, out var m1) && m1 is MathModel math && m1.Active)
                    {
                        math.Formula = _Formula;
                    }
                }
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

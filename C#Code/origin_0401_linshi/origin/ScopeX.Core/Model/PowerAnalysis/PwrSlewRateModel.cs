using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.Measure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ScopeX.Core.PowerAnalysis
{
    internal class PwrSlewRateModel : INotifyPropertyChanged
    {
        public ChannelId BoundMathId1
        {
            get;
            set;
        } = ChannelId.M1;

        private String _Formula1;

        public String Formula1
        {
            get
            {
                return _Formula1;
            }
            private set
            {
                if (_Formula1 != value)
                {
                    _Formula1 = value;
                    if (BoundMathId1 != null && DsoModel.Default != null && DsoModel.Default.TryGetChannel(BoundMathId1, out var m1) && m1 is MathModel math && m1.Active)
                    {
                        math.Formula = _Formula1;
                    }
                }
            }
        }

        public ChannelId BoundMathId2
        {
            get;
            set;
        } = ChannelId.M2;

        private String _Formula2;

        public String Formula2
        {
            get
            {
                return _Formula2;
            }
            set
            {
                if (_Formula2 != value)
                {
                    _Formula2 = value;
                    if (BoundMathId2 != null && DsoModel.Default != null && DsoModel.Default.TryGetChannel(BoundMathId2, out var m2) && m2 is MathModel math && m2.Active)
                    {
                        math.Formula = _Formula2;
                    }
                }
            }
        }

        public class SlewRateItem
        {
            private Double _Current;

            public Double Current
            {
                get => _Current;

                set
                {
                    _Current = value;
                    if (Double.IsFinite(value))
                    {
                        StaBuffer.Insert(value);
                    }
                }
            }

            public readonly StatisticBuffer StaBuffer;

            public SlewRateItem(Int32 size = 1000)
            {
                StaBuffer = new(size);
            }
        };

        public PwrSlewRateModel(PowerAnalysisModel pam, MeasureModel mm)
        {
            Analysis = pam;
            _Meas = mm;

            _SlewRates = new ConcurrentDictionary<String, SlewRateItem>();

            foreach (var item in Items)
            {
                _SlewRates.TryAdd(item, new SlewRateItem());
            }

            Count = _SlewRates.Count;
            _Formula1 = $"{MathType.Custom}:Execute.Der({Analysis.VoltageSrc1})";
            _Formula2 = $"{MathType.Custom}:Execute.Der({Analysis.CurrentSrc1})";
        }

        public void UpdateSource()
        {
            Formula1 = $"{MathType.Custom}:Execute.Der({Analysis.VoltageSrc1})";
            Formula2 = $"{MathType.Custom}:Execute.Der({Analysis.CurrentSrc1})";
        }


        internal readonly List<String> Items = new List<String>()
        {
            "VoltageRaiseRateMax",//0
            "VoltageRaiseRateMin",//1
            "VoltageRaiseRatePk2Pk",//2
            "VoltageFallRateMax",//3
            "VoltageFallRateMin",//4
            "VoltageFallRatePk2Pk",//5
            "CurrentRaiseRateMax",//6
            "CurrentRaiseRateMin",//7
            "CurrentRaiseRatePk2Pk",//8
            "CurrentFallRateMax",//9
            "CurrentFallRateMin",//10
            "CurrentFallRatePk2Pk"//11
        };

        public PowerAnalysisModel Analysis
        {
            get;
        }

        private readonly MeasureModel _Meas;

        private readonly ConcurrentDictionary<String, SlewRateItem> _SlewRates;

        public SlewRateItem this[String key] => _SlewRates[key];

        public readonly Int32 Count;

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

        public String VoltageRateUnit
        {
            get
            {
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Analysis.VoltageSrc1, out var cp) && cp != null)
                {
                    return $"{cp.Unit}/s";
                }
                else
                {
                    return QuantityUnit.VoltageRate.ToUnitString();
                }
            }
        }
        public String CurrentRateUnit
        {
            get
            {
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Analysis.CurrentSrc1, out var cp) && cp != null)
                {
                    return $"{cp.Unit}/s";
                }
                else
                {
                    return QuantityUnit.CurrentRate.ToUnitString();
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

        //Run方法，执行计算的操作
        public void Run()
        {
            if (RunCompleted)
            {
                _ = SingleRun();
            }
        }

        public async Task SingleRun()
        {
            try
            {
                await Task.Run(() => SlewRateAnalysis());
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }

        public void SlewRateAnalysis()
        {
            RunCompleted = false;
            RunAnalysis();
            RunCompleted = true;
        }


        private List<(Double Start, Double End, Int32 Type)> _VoltageEdges = new List<(Double Start, Double End, Int32 Type)>();

        private List<(Double Rate, Int32 Type)> _VoltageRate = new List<(Double Rate, Int32 Type)>();

        private List<(Double Start, Double End, Int32 Type)> _CurrentEdges = new List<(Double Start, Double End, Int32 Type)>();

        private List<(Double Rate, Int32 Type)> _CurrentRate = new List<(Double Rate, Int32 Type)>();


        private void RunAnalysis()
        {
            _VoltageEdges.Clear();
            _VoltageRate.Clear();
            _CurrentEdges.Clear();
            _CurrentRate.Clear();

            try
            {
                //获取数据
                DsoModel.Default.TryGetChannel(Analysis.VoltageSrc1, out var invol);
                DsoModel.Default.TryGetChannel(Analysis.CurrentSrc1, out var incur);
                if (incur == null || !incur.Active || invol == null || !invol.Active)
                {
                    return;
                }

                (Double[] data, Double fs)? voldata = null;
                (Double[] data, Double fs)? curdata = null;

                //电压转换速率
                //找到所有电压的上升沿和下降沿
                _VoltageEdges = GetChannelAllEdges(Analysis.VoltageSrc1, ref voldata);

                //电流转换速率
                //找到所有电流的上升沿和下降沿
                _CurrentEdges = GetChannelAllEdges(Analysis.CurrentSrc1, ref curdata);

                //分别计算上升沿和下降沿的转换速度
                if (voldata != null && _VoltageEdges.Count != 0)
                {
                    CalcRate(_VoltageEdges, voldata.Value, ref _VoltageRate);
                }

                //分别计算上升沿和下降沿的转换速度
                if (curdata != null && _CurrentEdges.Count != 0)
                {
                    CalcRate(_CurrentEdges, curdata.Value, ref _CurrentRate);
                }

                ////装填数据
                var vol = GetRateRange(_VoltageRate);

                _SlewRates[Items[0]].Current = Quantity.ConvertByPrefix(vol.Raise_Max, Prefix.Empty);
                _SlewRates[Items[1]].Current = Quantity.ConvertByPrefix(vol.Raise_Min, Prefix.Empty);
                _SlewRates[Items[2]].Current = Quantity.ConvertByPrefix(vol.Raise_Max - vol.Raise_Min, Prefix.Empty);
                _SlewRates[Items[3]].Current = Quantity.ConvertByPrefix(vol.Fall_Max, Prefix.Empty);
                _SlewRates[Items[4]].Current = Quantity.ConvertByPrefix(vol.Fall_Min, Prefix.Empty);
                _SlewRates[Items[5]].Current = Quantity.ConvertByPrefix(vol.Fall_Max - vol.Fall_Min, Prefix.Empty);

                var cur = GetRateRange(_CurrentRate);
                _SlewRates[Items[6]].Current = Quantity.ConvertByPrefix(cur.Raise_Max, Prefix.Empty);
                _SlewRates[Items[7]].Current = Quantity.ConvertByPrefix(cur.Raise_Min, Prefix.Empty);
                _SlewRates[Items[8]].Current = Quantity.ConvertByPrefix(cur.Raise_Max - cur.Raise_Min, Prefix.Empty);
                _SlewRates[Items[9]].Current = Quantity.ConvertByPrefix(cur.Fall_Max, Prefix.Empty);
                _SlewRates[Items[10]].Current = Quantity.ConvertByPrefix(cur.Fall_Min, Prefix.Empty);
                _SlewRates[Items[11]].Current = Quantity.ConvertByPrefix(cur.Fall_Max - cur.Fall_Min, Prefix.Empty);
            }
            catch (Exception e)
            {
                String msg = $"{e.Message} \r\n {e.StackTrace}";
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(msg, EventBus.LogLevel.Debug));
            }
        }


        private List<(Double Start, Double End, Int32 Type)> GetChannelAllEdges(ChannelId chnl, ref (Double[] data, Double fa)? calcData)
        {
            List<(Double Start, Double End, Int32 Type)> temp = new List<(Double Start, Double End, Int32 Type)>();
            var vol_rises = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.RiseEdges), chnl, ref calcData);
            var cur_falls = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.FallEdges), chnl, ref calcData);

            if (vol_rises != null && vol_rises.Count > 0)
            {
                foreach (var item in vol_rises)
                {
                    temp.Add((item.Start, item.End, 1));
                }
            }

            if (cur_falls != null && cur_falls.Count > 0)
            {
                foreach (var item in cur_falls)
                {
                    temp.Add((item.Start, item.End, -1));
                }
            }

            return temp;
        }

        public void CalcRate(List<(Double Start, Double End, Int32 Type)> edges, (Double[] Data, Double Fs) data, ref List<(Double Rate, Int32 Type)> rates)
        {
            Double rate = Double.NaN;
            Double delta_y = Double.NaN;
            Double delta_x = Double.NaN;
            Int32 x_start = -1;
            Int32 x_end = -1;
            for (Int32 i = 0, l = edges.Count; i < l; i++)
            {
                x_start = (Int32)Math.Round(edges[i].Start, 0);
                x_end = (Int32)Math.Round(edges[i].End, 0);
                delta_y = (data.Data[x_end] - data.Data[x_start]) * 1E-3;
                delta_x = (x_end - x_start) / data.Fs;
                rate = delta_y / delta_x;
                rates.Add((rate, edges[i].Type));
            }
        }


        public (Double Raise_Max, Double Raise_Min, Double Fall_Max, Double Fall_Min) GetRateRange(List<(Double Rate, Int32 Type)> values)
        {
            if (values.Count != 0)
            {
                Double raise_max = Double.MinValue;
                Double raise_min = Double.MaxValue;
                Double fall_max = Double.MinValue;
                Double fall_min = Double.MaxValue;
                foreach (var item in values)
                {
                    if (item.Type == 1)
                    {
                        if (item.Rate > raise_max)
                        {
                            raise_max = item.Rate;
                        }
                        if (item.Rate < raise_min)
                        {
                            raise_min = item.Rate;
                        }
                    }
                    else
                    {
                        if (item.Rate > fall_max)
                        {
                            fall_max = item.Rate;
                        }
                        if (item.Rate < fall_min)
                        {
                            fall_min = item.Rate;
                        }
                    }
                }

                if (raise_max == Double.MinValue)
                {
                    raise_max = Double.NaN;
                }
                if (raise_min == Double.MaxValue)
                {
                    raise_min = Double.NaN;
                }
                if (fall_max == Double.MinValue)
                {
                    fall_max = Double.NaN;
                }
                if (fall_min == Double.MaxValue)
                {
                    fall_min = Double.NaN;
                }

                return (raise_max, raise_min, fall_max, fall_min);
            }
            else
            {
                return (Double.NaN, Double.NaN, Double.NaN, Double.NaN);
            }
        }

        public void Reset()
        {
            foreach (var p in _SlewRates)
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

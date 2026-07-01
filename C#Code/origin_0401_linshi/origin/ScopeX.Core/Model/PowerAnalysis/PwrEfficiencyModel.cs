namespace ScopeX.Core.PowerAnalysis
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;
    using ScopeX.Measure;

    internal class PwrEfficiencyModel : INotifyPropertyChanged
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
                    if (BoundMathId1 != null && DsoModel.Default.TryGetChannel(BoundMathId1, out var m1) && m1 is MathModel math && m1.Active)
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
                    if (BoundMathId2 != null && DsoModel.Default.TryGetChannel(BoundMathId2, out var m2) && m2 is MathModel math && m2.Active)
                    {
                        math.Formula = _Formula2;
                    }
                }
            }
        }


        public class EfficiencyItem
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

            public EfficiencyItem(Int32 size = 1000)
            {
                StaBuffer = new(size);
            }
        };

        public PwrEfficiencyModel(PowerAnalysisModel pam, MeasureModel mm)
        {
            Analysis = pam;
            _Meas = mm;

            _Efficiencies = new ConcurrentDictionary<String, EfficiencyItem>();

            foreach (var item in Items)
            {
                _Efficiencies.TryAdd(item, new EfficiencyItem());
            }

            Count = _Efficiencies.Count;

            _Formula1 = $"{MathType.Custom}:{InVoltageSrc}*{InCurrentSrc}";
            _Formula2 = $"{MathType.Custom}:{OutVoltageSrc}*{OutCurrentSrc}";
        }

        public void UpdateSource()
        {
            Formula1 = $"{MathType.Custom}:{InVoltageSrc}*{InCurrentSrc}";
            Formula2 = $"{MathType.Custom}:{OutVoltageSrc}*{OutCurrentSrc}";
        }

        internal readonly List<String> Items = new List<String>()
        { "InTruePower", "OutTruePower","Efficiency" };

        public PowerAnalysisModel Analysis
        {
            get;
        }

        private readonly MeasureModel _Meas;

        private readonly ConcurrentDictionary<String, EfficiencyItem> _Efficiencies;

        public EfficiencyItem this[String key] => _Efficiencies[key];

        public readonly Int32 Count;

        public ChannelId InCurrentSrc
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

        public ChannelId OutCurrentSrc
        {
            get => Analysis.CurrentSrc2;
            set
            {
                if (Analysis.CurrentSrc2 != value)
                {
                    Analysis.CurrentSrc2 = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public ChannelId OutVoltageSrc
        {
            get => Analysis.VoltageSrc2;
            set
            {
                if (Analysis.VoltageSrc2 != value)
                {
                    Analysis.VoltageSrc2 = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _CalcCompleted = true;
        public Boolean CalcCompleted
        {
            get { return _CalcCompleted; }
            set
            {
                if (_CalcCompleted != value)
                {
                    _CalcCompleted = value;
                }
            }
        }

        private CurrentType _InputType = CurrentType.DC;
        public CurrentType InputType
        {
            get => _InputType;
            set
            {
                if (value != _InputType)
                {
                    _InputType = value;
                    OnPropertyChanged();
                }
            }
        }

        private CurrentType _OutputType = CurrentType.DC;
        public CurrentType OutputType
        {
            get => _OutputType;
            set
            {
                if (value != _OutputType)
                {
                    _OutputType = value;
                    OnPropertyChanged();
                }
            }
        }


        //Run方法，执行计算的操作
        public void Run()
        {
            if (CalcCompleted)
            {
                _ = SingleRun();
            }
        }

        public async Task SingleRun()
        {
            await Task.Run(() => EfficiencyAnalysis());
        }

        public void EfficiencyAnalysis()
        {
            CalcCompleted = false;
            try
            {
                RunAnalysis();
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
            finally
            {
                CalcCompleted = true;
            }
        }

        private void RunAnalysis()
        {
            //获取数据
            //输入
            DsoModel.Default.TryGetChannel(InCurrentSrc, out var incur);
            DsoModel.Default.TryGetChannel(InVoltageSrc, out var invol);
            var incurpkg = DsoModel.Default.GetWfmPack(InCurrentSrc);
            var involpkg = DsoModel.Default.GetWfmPack(InVoltageSrc);
            if ((incur == null || !incur.Active) || incurpkg == null || (invol == null || !invol.Active) || involpkg == null)
            {
                return;
            }

            Double[] incurdata = incurpkg.Buffer.Cast<Double>().Select(x => x * 1E-3).ToArray();
            Double[] involdata = involpkg.Buffer.Cast<Double>().Select(x => x * 1E-3).ToArray();

            //输出
            DsoModel.Default.TryGetChannel(OutCurrentSrc, out var outcur);
            DsoModel.Default.TryGetChannel(OutVoltageSrc, out var outvol);
            var outcurpkg = DsoModel.Default.GetWfmPack(OutCurrentSrc);
            var outvolpkg = DsoModel.Default.GetWfmPack(OutVoltageSrc);
            if ((outcur == null || !outcur.Active) || outcurpkg == null || (outvol == null || !outvol.Active) || outvolpkg == null)
            {
                return;
            }
            Double[] outcurdata = outcurpkg.Buffer.Cast<Double>().Select(x => x * 1E-3).ToArray();
            Double[] outvoldata = outvolpkg.Buffer.Cast<Double>().Select(x => x * 1E-3).ToArray();

            //找出周期

            var indexes = GetCalcIndexes(incurdata, involdata);

            //计算
            Double intruepower = GetTruePower(incurdata, involdata, indexes.StartIndex, indexes.EndIndex);
            if (Double.IsFinite(intruepower))
            {
                intruepower = Quantity.ConvertByPrefix(intruepower, Prefix.Empty);
            }
            _Efficiencies[Items[0]].Current = intruepower;
            Double outtruepower = GetTruePower(outcurdata, outvoldata, indexes.StartIndex, indexes.EndIndex);
            if (Double.IsFinite(outtruepower))
            {
                outtruepower = Quantity.ConvertByPrefix(outtruepower, Prefix.Empty);
            }
            _Efficiencies[Items[1]].Current = outtruepower;

            Double efficiency = Double.NaN;
            if (Double.IsFinite(outtruepower) && Double.IsFinite(intruepower))
            {
                efficiency = 100 * outtruepower / intruepower;
                efficiency = Quantity.ConvertByPrefix(efficiency, Prefix.Empty);
            }

            _Efficiencies[Items[2]].Current = efficiency;
        }

        public (Int32 StartIndex, Int32 EndIndex) GetCalcIndexes(Double[] incur, Double[] invol)
        {
            Int32 start_index = 0;
            Int32 end_index = invol.Count();
            var cycles = _Meas.Calc.ForceGetResultOrCalcForPowerByScreen(nameof(MeasParameter.Period), InVoltageSrc);
            if (cycles != null && cycles.Count > 0)
            {
                start_index = (Int32)Math.Round(cycles.First().Start);
                end_index = (Int32)Math.Round(cycles.Last().End);
            }
            return (start_index, end_index);
        }

        public Double GetTruePower(Double[] cur, Double[] vol, Int32 startIndex, Int32 endIndex)
        {
            Double sum = 0;
            Double length = 0;
            if (startIndex < 0 || startIndex >= cur.Length - 1 || startIndex >= vol.Length - 1)
            {
                startIndex = 0;
            }
            if (endIndex > cur.Length - 1 || endIndex >= vol.Length - 1 || endIndex <= 0)
            {
                endIndex = Math.Min(cur.Length - 1, vol.Length - 1);
            }

            for (Int32 i = startIndex, l = cur.Length; i <= endIndex && i < l; i++)
            {
                sum += cur[i] * vol[i];
                length++;
            }

            return sum / length;
        }

        public void Reset()
        {
            foreach (var p in _Efficiencies)
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

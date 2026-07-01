namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using ScopeX.ComModel;
    using static ScopeX.Core.SearchInfo;

    internal class LocationAssistedModel : INotifyPropertyChanged
    {
        public LocationAssistedModel(MeasureModel mm)
        {
            Meas = mm;
        }

        protected readonly MeasureModel Meas;

        public readonly String Name = "SoftAssisted";

        private Boolean _Enabled = false;
        public Boolean Enabled
        {
            get => _Enabled;
            set
            {
                if (_Enabled != value)
                {
                    _Enabled = value;
                    OnPropertyChanged();
                }
            }
        }

        private ChannelId _Source = ChannelId.C1;
        public ChannelId Source
        {
            get => _Source;
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    OnPropertyChanged();
                }
            }
        }

        private String _MeasName = "Max";
        public String MeasName
        {
            get => _MeasName;
            set
            {
                if (_MeasName != value)
                {
                    _MeasName = value;
                    OnPropertyChanged();
                }
            }
        }

        private CompareCondition _Condition = CompareCondition.GreaterThan;
        public CompareCondition Condition
        {
            get => _Condition;
            set
            {
                if (_Condition != value)
                {
                    _Condition = value;
                    OnPropertyChanged();
                }
            }
        }

        private Double _LowerThreshold = 0;
        public Double LowerThreshold
        {
            get => _LowerThreshold;
            set
            {
                value = ValidateThrold(value);

                if (value != _LowerThreshold)
                {
                    _LowerThreshold = value;
                    OnPropertyChanged();
                }
            }
        }

        public Double MaxThreshold
        {
            get; init;
        } = Double.MaxValue;

        public Double MinThreshold
        {
            get; init;
        } = Double.MinValue;

        private Double ValidateThrold(Double value)
        {
            //value = Math.Round(value, 7, MidpointRounding.AwayFromZero);
            if (value > MaxThreshold)
            {
                value = MaxThreshold;
            }
            else if (value < MinThreshold)
            {
                value = MinThreshold;
            }

            return value;
        }


        private Double _UpperThreshold = 0;
        public Double UpperThreshold
        {
            get => _UpperThreshold;
            set
            {
                value = ValidateThrold(value);

                if (value != _UpperThreshold)
                {
                    _UpperThreshold = value;
                    OnPropertyChanged();
                }
            }
        }

        public Double Position
        {
            get;
            private set;
        } = -1;

        public Double Locate()
        {
            Double pos = -1;
        
            var datasearch = SelectResult(MeasName, Source, Condition);
            if (datasearch != null && datasearch.Contents.Count > 0)
            {
                pos = datasearch.Contents[0].Location;
            }

            return Position = pos;
        }

        public SearchInfo SelectResult(String measureName, ChannelId source, CompareCondition condition)
        {
            var (x, y) = Meas.Calc.GetTrack(new MeasureItemModel(measureName, source), MeasureGate.Screen);

            if ((y != null && x != null) && y.Count <= x.Count)
            {

                List<SearchArgs> result = new();

                switch (condition)
                {
                    case CompareCondition.GreaterThan:
                        {
                            for (Int32 i = 0; i < y.Count; i++)
                            {
                                if (y[i] > _LowerThreshold)
                                {
                                    result.Add(new SearchArgs(SearchType.Pulse, x[i], measureName));
                                }
                            }
                            return new SearchInfo(result);
                        }
                    case CompareCondition.LessThan:
                        {
                            for (Int32 i = 0; i < y.Count; i++)
                            {
                                if (y[i] < _LowerThreshold)
                                {
                                    result.Add(new SearchArgs(SearchType.Pulse, x[i], measureName));
                                }
                            }
                            return new SearchInfo(result);
                        }
                    case CompareCondition.InRange:
                        {
                            var max = Math.Max(_LowerThreshold, _UpperThreshold);
                            var min = Math.Max(_LowerThreshold, _UpperThreshold);

                            for (Int32 i = 0; i < y.Count; i++)
                            {
                                if (y[i] < max || y[i] > min)
                                {
                                    result.Add(new SearchArgs(SearchType.Pulse, x[i], measureName));
                                }
                            }
                            return new SearchInfo(result);
                        }
                    case CompareCondition.OutRange:
                        {
                            var max = Math.Max(_LowerThreshold, _UpperThreshold);
                            var min = Math.Max(_LowerThreshold, _UpperThreshold);

                            for (Int32 i = 0; i < y.Count; i++)
                            {
                                if (y[i] > max || y[i] < min)
                                {
                                    result.Add(new SearchArgs(SearchType.Pulse, x[i], measureName));
                                }
                            }
                            return new SearchInfo(result);
                        }
                    case CompareCondition.NotCare:
                        {
                            for (Int32 i = 0; i < y.Count; i++)
                            {
                                result.Add(new SearchArgs(SearchType.Pulse, x[i], measureName));
                            }
                            return new SearchInfo(result);
                        }
                    default:
                        break;
                }
            }
            return new SearchInfo(new List<SearchInfo.SearchArgs>());
        }


        protected PropertyChangedEventHandler? _PropertyChanged;

        public virtual event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
                TriggerShareParameter.Default.PropertyChanged += value;
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
                TriggerShareParameter.Default.PropertyChanged -= value;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

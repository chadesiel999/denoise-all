using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    internal class MeasureModel : INotifyPropertyChanged
    {
        private Boolean _Active = false;
        public Boolean Active
        {
            get => _Active;
            set
            {
                if (_Active != value)
                {
                    _Active = value;
                    KeyLed.Default.SetLed(LedEnum.LedMeasure, value);
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _IsStatActive = false;
        public Boolean IsStatActive
        {
            get => _IsStatActive;
            set
            {
                if (_IsStatActive != value)
                {
                    _IsStatActive = value;
                    OnPropertyChanged();
                }
            }
        }


        private MeasureGate _Strobe = MeasureGate.Screen;
        public MeasureGate Strobe
        {
            get => _Strobe;
            set
            {
                if (_Strobe != value)
                {
                    _Strobe = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _StopMeasure = false;
        public Boolean StopMeasure
        {
            get => _StopMeasure;
            set
            {
                if (_StopMeasure != value)
                {
                    _StopMeasure = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _Indicator = 0;
        public Int32 Indicator
        {
            get => _Indicator;
            set
            {
                if (_Indicator != value)
                {
                    _Indicator = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 在获取值时请调用 CalcSnapshotAllResult函数
        /// </summary>
        internal List<String> SnapShotDataTable = new();

        internal Boolean CalcSnapshotAllResult() => Calc.CalcSnapshotAllResult();

        private Boolean _SnapshotActive = false;
        public Boolean SnapshotActive
        {
            get => _SnapshotActive;
            set
            {
                if (_SnapshotActive != value)
                {
                    _SnapshotActive = value;
                    OnPropertyChanged();
                }
            }
        }

        private ChannelId _SnapshotSrc = ChannelId.C1;
        public ChannelId SnapshotSource
        {
            get => _SnapshotSrc;
            set
            {
                if (_SnapshotSrc != value)
                {
                    _SnapshotSrc = value;
                    OnPropertyChanged();
                }
            }
        }
        public String[] SnapShotTableNames = { "Max", "Min", "Pk2Pk", "Top", "Base", "Mid", "Amplitude", "Average", "RMS", "Stddev", "POverShoot", "NOverShoot", "Area", "Period", "Freq", "Rise", "Fall", "PWidth", "NWidth", "Duty", "NDuty", "T@max", "T@min", "CycMax", "CycMin", "CycRMS", "CycAverage", "CycPeak", "CycArea", "CycMid", "WfmLength", "Cycles", "PPreShoot", "NPreShoot" };

        public Dictionary<String, String> SnapShotResult = new Dictionary<String, String>();


        public readonly ImmutableDictionary<String, Func<Double?, Double?, Double?>> ExtCalcName = new Dictionary<String, Func<Double?, Double?, Double?>>
        {
            { $"+", (s1, s2) => s1 + s2 },
            { $"-", (s1, s2) => s1 - s2 },
            { $"*", (s1, s2) => s1 * s2 },
            { $"/", (s1, s2) => s1 / s2 },
            { $"Abs", (s1, s2) => Math.Abs(s1 ?? Double.NaN) },
        }.ToImmutableDictionary();

        //??? There are a little problem about concurrent access.
        public MeasureItemModel[] SelectedItems
        {
            get;
        }

        internal MeasureProc Calc
        {
            get;
        }

        public MeasureModel()
        {
            Calc = new(this);

            SelectedItems = new MeasureItemModel[]
            {
                new MeasureItemModel(ChannelId.P1, "Pk2Pk", ItemPropertyChanged),
                new MeasureItemModel(ChannelId.P2, "Average", ItemPropertyChanged),
                new MeasureItemModel(ChannelId.P3, "RMS", ItemPropertyChanged),
                new MeasureItemModel(ChannelId.P4, "Period", ItemPropertyChanged),
                new MeasureItemModel(ChannelId.P5, "Freq", ItemPropertyChanged),
                new MeasureItemModel(ChannelId.P6, "PWidth", ItemPropertyChanged),
                new MeasureItemModel(ChannelId.P7, "Duty", ItemPropertyChanged),
                new MeasureItemModel(ChannelId.P8, "Rise", ItemPropertyChanged),
                new MeasureItemModel(ChannelId.P9, "Duty", ItemPropertyChanged),
                new MeasureItemModel(ChannelId.P10, "Rise", ItemPropertyChanged),
            };
            foreach (var item in SnapShotTableNames)
            {
                SnapShotResult.Add(item, String.Empty);
            }
        }

        #region INotifyPropertyChanged
        private PropertyChangedEventHandler? _PropertyChanged;
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

        protected void ItemPropertyChanged(Object? sender, PropertyChangedEventArgs e)
        {
            _PropertyChanged?.Invoke(sender, e);
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

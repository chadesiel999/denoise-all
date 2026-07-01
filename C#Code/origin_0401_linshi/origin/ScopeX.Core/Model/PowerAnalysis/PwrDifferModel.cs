// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/12</date>

namespace ScopeX.Core.PowerAnalysis
{
    using System;
    using System.Collections.Concurrent;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using ScopeX.ComModel;
    using ScopeX.MathExt;
    using ScopeX.Measure;

    internal class PwrDifferModel : INotifyPropertyChanged
    {
        public class DifferItems
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

            public DifferItems(Int32 size = 1000)
            {
                StaBuffer = new(size);
            }
        };

        public PwrDifferModel(PowerAnalysisModel pam, MeasureModel mm)
        {
            Analysis = pam;
            _Meas = mm;

            _Differs = new ConcurrentDictionary<String, DifferItems>()
            {
                ["Max"] = new DifferItems(),
                ["Min"] = new DifferItems()

            };
            Count = _Differs.Count;
        }

        public PowerAnalysisModel Analysis
        {
            get;
        }

        private readonly MeasureModel _Meas;

        private readonly ConcurrentDictionary<String, DifferItems> _Differs;
        
        public DifferItems this[String key] => _Differs[key];

        public readonly Int32 Count;

        public String Unit
        {
            get;
            private set;
        } = "";

        public Boolean ValidExp
        {
            get;
            private set;
        } = false;

        private ChannelId GetDifferSrc()
        {
            return (Source == VIType.V) ? Analysis.VoltageSrc1 : Analysis.CurrentSrc1;
        }

        private void CalcMaxMinSlewRate()
        {
            var max = Double.NaN;
            var min = Double.NaN;
            MathVecBuffer.Default.TryGetVector(BoundMathId.ToString(), out var vec);
            if (vec?.Elements is not null)
            {
                max = vec.Elements.ToEnumerable().Max();
                min = vec.Elements.ToEnumerable().Min();
                Unit = vec.YUnit;
            }
            _Differs["Max"].Current = max * 1E-3;
            _Differs["Min"].Current = min * 1E-3;
        }

        private VIType _Source = VIType.V;
        public VIType Source
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

        public void Run()
        {
            var pwrch = (MathModel)DsoModel.Default.GetChannel(BoundMathId);
            ValidExp = pwrch.Formula == Formula && pwrch.Active;
            if (ValidExp)
            {
                CalcMaxMinSlewRate();
            }
        }

        public void Reset()
        {
            foreach (var p in _Differs)
            {
                p.Value.StaBuffer.Clear();
            }
        }

        //private ChannelId _MathId = ChannelId.M1;

        //public void TryShowDifferWfm(ChannelId id)
        //{
        //    _MathId = id;

        //    var pwrch = (MathModel)DsoModel.Default.GetChannel(_MathId);
        //    var pwrexp = GetExpression(GetDifferSource());
        //    if (pwrch.Formula != pwrexp)
        //    {
        //        pwrch.Formula = pwrexp;
        //        pwrch.Args = new MathCustomArg(_MathId, pwrexp, Analysis);

        //        pwrch.InitFlag = true;
        //    }

        //    pwrch.Label = Source == VIType.V ? "dV/dt" : "dI/dt";
        //    pwrch.Active = true;
        //}

        public ChannelId BoundMathId
        {
            get;
            set;
        } = ChannelId.M1;

        public String Formula => $"{MathType.Custom}:Execute.Avg(Execute.Der({GetDifferSrc()}), 5)";

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

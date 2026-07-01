using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScopeX.ComModel;

namespace ScopeX.Core.PowerAnalysis
{
    internal class PwrTransientModel : INotifyPropertyChanged
    {
        public class TransientItems
        {
            public Double Current
            {
                get;
                set;
            }
        }

        public PwrTransientModel(PowerAnalysisModel pam, MeasureModel mm)
        {
            Analysis = pam;
            _Meas = mm;

            _Transient = new ConcurrentDictionary<String, TransientItems>()
            {
                ["Transient"] = new TransientItems(),
            };
            Count = _Transient.Count;
        }

        public PowerAnalysisModel Analysis
        {
            get;
        }

        private readonly ConcurrentDictionary<String, TransientItems> _Transient;

        private readonly MeasureModel _Meas;

        public TransientItems this[String key] => _Transient[key];

        public readonly Int32 Count;
               
        public void Run()
        {
            CalcTransient();
        }

        private void CalcTransient()
        {
            //Double StartTime = _Meas.Calc.GetResultOrCalc("StartTime", _Analysis.VoltageSrc) ?? Double.NaN;
            //Double EndTime = _Meas.Calc.GetResultOrCalc("EndTime", _Analysis.VoltageSrc) ?? Double.NaN;
            // Double InrushCurrent = _Meas.Calc.GetResultOrCalc("InrushCurrent", _Analysis.CurrentSrc) ?? Double.NaN;
            // Double imax = _Meas.Calc.GetResultOrCalc("Max", _Analysis.CurrentSrc) ?? Double.NaN;
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
        
        private Double _StableValue = 0;
        public Double StableValue //稳定输出电压
        {
            get => _StableValue;
            set
            {
                if (_StableValue != value)
                {
                    _StableValue = value;
                    OnPropertyChanged();
                }
            }
        }

        private Double _Overshoot = 10;
        public Double Overshoot //过冲百分比
        {
            get => _Overshoot;
            set
            {
                if (_Overshoot != value)
                {
                    _Overshoot = value;
                    OnPropertyChanged();
                }
            }
        }

        public ChannelId BoundMathId
        {
            get;
            set;
        } = ChannelId.M1;

        public String Formula => "";


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
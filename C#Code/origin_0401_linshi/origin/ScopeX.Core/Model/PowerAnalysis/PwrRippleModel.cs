namespace ScopeX.Core.PowerAnalysis
{
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;
    using ScopeX.Measure;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    internal class PwrRippleModel : INotifyPropertyChanged
    {
        public class RippleItem
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

            public RippleItem(Int32 size = 1000)
            {
                StaBuffer = new(size);
            }
        };

        public readonly static List<String> RippleHearders = new List<String>() { "Max","Min","Pk2Pk","Average", "Stddev" };

        public PwrRippleModel(PowerAnalysisModel pam, MeasureModel mm)
        {
            Analysis = pam;
            _Meas = mm;

            _Ripple = new ConcurrentDictionary<String, RippleItem>();
           
            foreach (var item in RippleHearders)
            {
                _Ripple.TryAdd(item, new RippleItem());
            }

            Count = _Ripple.Count;
        }

        public PowerAnalysisModel Analysis
        {
            get;
        }

        private readonly MeasureModel _Meas;

        private readonly ConcurrentDictionary<String, RippleItem> _Ripple;

        public RippleItem this[String key] => _Ripple[key];

        public readonly Int32 Count;
        public String Ttitles = "Value,Average,Maximum,Minimum";

        private ChannelId GetRefSource()
        {
            return (Source == VIType.V) ? Analysis.VoltageSrc1 : Analysis.CurrentSrc1;
        }

        private void RippleAnalysis()
        {
            _CalcCompleted = false;
            foreach (var item in RippleHearders)
            {
                Double data = _Meas.Calc.ForceGetResultOrCalc(item, GetRefSource()) ?? Double.NaN;
                if(Double.IsFinite(data))
                {
                    _Ripple[item].Current = Quantity.ConvertByPrefix(data, MeasureProc.GetPfxUnitString(item, GetRefSource()).Prefix);
                }
                else
                {
                    _Ripple[item].Current = data;
                }
            }
            _CalcCompleted = true;
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
                    Reset();
                    foreach (var item in RippleHearders)
                    {
                        _Ripple[item].Current = Double.NaN;
                    }
                    OnPropertyChanged();
                }
            }
        }

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

        private Boolean _CalcCompleted = true;
        public Boolean CalcCompleted
        {
            get { return _CalcCompleted; }
        }

        public void Run()
        {
            if (CalcCompleted)
            {
                _ = SingleRun();
            }
        }

        public async Task SingleRun()
        {
            try
            {
                await Task.Run(() => RippleAnalysis());
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }

        public void Reset()
        {
            foreach (var p in _Ripple)
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

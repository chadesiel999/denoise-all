using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics;
using NPOI.POIFS.Crypt.Dsig;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.Measure;

namespace ScopeX.Core.PowerAnalysis
{
    public class ModulationItem
    {
        private Double _Current;

        public Double Current
        {
            get => _Current;

            set
            {
                lock (this)
                {
                    _Current = value;
                    if (!Double.IsNaN(value))
                    {
                        StaBuffer.Insert(value);
                    }
                }
            }
        }

        private Boolean _TrendActive = false;
        public Boolean TrendActive
        {
            get => _TrendActive;

            set
            {
                if (_TrendActive != value)
                {
                    _TrendActive = value;
                    // 消息提醒
                }
            }
        }

        private Int64? _TrendWindowId = null;
        public Int64? TrendWindowId
        {
            get => _TrendWindowId;
            set
            {
                if (_TrendWindowId != value)
                {
                    _TrendWindowId = value;
                }
            }
        }

        private Boolean _HistgramActive = false;
        public Boolean HistgramActive
        {
            get => _HistgramActive;

            set
            {
                if (_HistgramActive != value)
                {
                    _HistgramActive = value;
                    // 消息提醒
                }
            }
        }

        private Int64? _HistgramWindowId = null;
        public Int64? HistgramWindowId
        {
            get => _HistgramWindowId;
            set
            {
                if (_HistgramWindowId != value)
                {
                    _HistgramWindowId = value;
                }
            }
        }

        private Boolean _ClearFlag = false;
        public Boolean ClearFlag
        {
            get => _ClearFlag;
            set
            {
                if (_ClearFlag != value)
                {
                    _ClearFlag = value;
                }
            }
        }

        public List<Double> HistgramBins = new List<Double>();

        public List<(Double Value, Int32 Index)> Histgram = new List<(Double Value, Int32 Index)>();

        public List<Int32> HistgramCounts = new List<Int32>();


        private List<(Double Value, Int32 Index)> _CurrentArray=new List<(Double Value, Int32 Index)>();

        public List<(Double Value, Int32 Index)> CurrentArray
        {
            get => _CurrentArray;

            set
            {
                lock (this)
                {
                    _CurrentArray = value;
                    if (value != null && value.Count > 0)
                    {
                        Histgram.AddRange(value);
                    }
                }
            }
        }

        public readonly StatisticBuffer StaBuffer;

        public ModulationItem(Int32 size = 1000)
        {
            StaBuffer = new(size);
        }

        public void Clear()
        {
            lock (this)
            {
                StaBuffer.Clear();
                CurrentArray.Clear();
                Histgram.Clear();
            }
        }
    };
    internal class PwrModulationModel : INotifyPropertyChanged
    {
        #region 注释的代码
        //internal class ModulationItem
        //{
        //    private Double _Current;

        //    public Double Current
        //    {
        //        get => _Current;

        //        set
        //        {
        //            lock (this)
        //            {
        //                _Current = value;
        //                if (!Double.IsNaN(value))
        //                {
        //                    StaBuffer.Insert(value);
        //                }
        //            }
        //        }
        //    }

        //    private Boolean _TrendActive = false;
        //    public Boolean TrendActive
        //    {
        //        get => _TrendActive;

        //        set
        //        {
        //            if (_TrendActive != value)
        //            {
        //                _TrendActive = value;
        //                // 消息提醒
        //            }
        //        }
        //    }

        //    private Int64? _TrendWindowId = null;
        //    public Int64? TrendWindowId
        //    {
        //        get => _TrendWindowId;
        //        set
        //        {
        //            if(_TrendWindowId != value)
        //            {
        //                _TrendWindowId = value;
        //            }
        //        }
        //    }

        //    private Boolean _HistgramActive = false;
        //    public Boolean HistgramActive
        //    {
        //        get => _HistgramActive;

        //        set
        //        {
        //            if (_HistgramActive != value)
        //            {
        //                _HistgramActive = value;
        //                // 消息提醒
        //            }
        //        }
        //    }

        //    private Int64? _HistgramWindowId = null;
        //    public Int64? HistgramWindowId
        //    {
        //        get => _HistgramWindowId;
        //        set
        //        {
        //            if (_HistgramWindowId != value)
        //            {
        //                _HistgramWindowId = value;
        //            }
        //        }
        //    }

        //    public List<Double> Histgram = new List<Double>();


        //    private List<(Double Value, Int32 Index)> _CurrentArray;

        //    public List<(Double Value, Int32 Index)> CurrentArray
        //    {
        //        get => _CurrentArray;

        //        set
        //        {
        //            lock (this)
        //            {
        //                _CurrentArray = value;
        //                if (value != null && value.Count > 0)
        //                {
        //                    Histgram.AddRange(value.Select(x => x.Value));
        //                }
        //            }
        //        }
        //    }

        //    public readonly StatisticBuffer StaBuffer;

        //    public ModulationItem(Int32 size = 1000)
        //    {
        //        StaBuffer = new(size);
        //    }

        //    public void Clear()
        //    {
        //        lock (this)
        //        {
        //            StaBuffer.Clear();
        //            CurrentArray.Clear();
        //            Histgram.Clear();
        //        }
        //    }
        //};
        #endregion


        public PwrModulationModel(PowerAnalysisModel pam, MeasureModel mm)
        {
            Analysis = pam;
            _Meas = mm;

            _Modulation = new ConcurrentDictionary<String, ModulationItem>();
            foreach (var item in _Items)
            {
                _Modulation.TryAdd(item, new ModulationItem());
            }
            _ModulationItems = new ConcurrentDictionary<ModulationType, ModulationItem>
            {
                [ModulationType.Period] = new ModulationItem(),
                [ModulationType.Frequency] = new ModulationItem(),
                [ModulationType.PDuty] = new ModulationItem(),
                [ModulationType.NDuty] = new ModulationItem(),
                [ModulationType.PWidth] = new ModulationItem(),
                [ModulationType.NWidth] = new ModulationItem(),
                [ModulationType.RiseTime] = new ModulationItem(),
                [ModulationType.FallTime] = new ModulationItem(),
            };
            _ItemType = new ConcurrentDictionary<ChannelId, ModulationType>();
        }

        public PowerAnalysisModel Analysis
        {
            get;
        }

        private readonly MeasureModel _Meas;

        private readonly ConcurrentDictionary<String, ModulationItem> _Modulation;

        public ModulationItem this[String key] => _Modulation[key];

        private List<String> _Items = new List<String>()
            {nameof(ModulationType.Period),
            nameof(ModulationType.Frequency),
            nameof(ModulationType.PDuty),
            nameof(ModulationType.NDuty),
            nameof(ModulationType.PWidth),
            nameof(ModulationType.NWidth),
            nameof(ModulationType.RiseTime),
            nameof(ModulationType.FallTime),};

        public List<String> Items => _Items;

        private ConcurrentDictionary<ModulationType, ModulationItem> _ModulationItems;

        public ConcurrentDictionary<ModulationType, ModulationItem> ModulationItems
        {
            get => _ModulationItems;
            set
            {
                if (_ModulationItems != value)
                {
                    _ModulationItems = value;
                }
            }
        }
        private ConcurrentDictionary<ChannelId, ModulationType> _ItemType;
        public ConcurrentDictionary<ChannelId, ModulationType> ItemType
        {
            get => _ItemType;
            set
            {
                if (_ItemType != value)
                {
                    _ItemType = value;
                }
            }
        }
        private Int64? _WindowId;
        public Int64? WindowId
        {
            get => _WindowId;
            set
            {
                if (_WindowId != value)
                {
                    _WindowId = value;
                    OnPropertyChanged();
                }
            }
        }
        private ChannelId _Source = ChannelId.None;
        public ChannelId Source
        {
            get
            {
                if (_SourceType == VIType.V)
                    _Source = Analysis.VoltageSrc1;
                else
                    _Source = Analysis.CurrentSrc1;
                return _Source;
            }
        }
        private Boolean _SourceChanged = false;
        private VIType _SourceType = VIType.V;
        public VIType SourceType
        {
            get => _SourceType;
            set
            {
                if (_SourceType != value)
                {
                    _SourceType = value;
                    _SourceChanged = true;
                    OnPropertyChanged();
                }
            }
        }
        private ModulationType _HistogramSource = ModulationType.Period;
        public ModulationType HistogramSource
        {
            get => _HistogramSource;
            set
            {
                if (_HistogramSource != value)
                {
                    _HistogramSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private ModulationType _TrendSource = ModulationType.Period;
        public ModulationType TrendSource
        {
            get => _TrendSource;
            set
            {
                if (_TrendSource != value)
                {
                    _TrendSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private ChannelId _MathId = ChannelId.None;
        public ChannelId MathId
        {
            get => _MathId;
            set
            {
                if (_MathId != value)
                {
                    _MathId = value;
                }
            }
        }

        public readonly String Titles = "Value,Average,Maxumum,Minimum";

        private Boolean _CompletionFlag = true;
        public Boolean CompletionFlag
        {
            get => _CompletionFlag;
            set
            {
                if (_CompletionFlag != value)
                {
                    _CompletionFlag = value;
                }
            }
        }

        public void Run()
        {
            if (TriggerModel.State == SysState.Stop)
                return;
            if (_CompletionFlag)
            {
                _ = SingleRun();
            }

        }

        public async Task SingleRun()
        {
            try
            {
                await Task.Run(() => ModulationAnalysis());
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }

        private void ModulationAnalysis()
        {
            _CompletionFlag = false;
            if (_SourceType == VIType.V)
                _Source = Analysis.VoltageSrc1;
            else
                _Source = Analysis.CurrentSrc1;
            if(_SourceChanged)
            {
                Reset();
                _SourceChanged = false;
            }
            CalcModulations();
            _CompletionFlag = true;
            Thread.Sleep(100);
        }

        private void CalcModulations()
        {
            (Double[] Data, Double Fs)? data = null;
           var meas = _Meas.Calc.ForceGetResultOrCalcForModulation(_Source, ref data);
            if (meas.Periods != null && meas.Periods.Count > 0 && data != null && data.Value.Data.Length > 0 && Double.IsFinite(data.Value.Fs))
            {
               
                List<(Double Value, Int32 Index)>? values = null;

                for (Int32 i = 0, l = _Items.Count; i < l; i++)
                {
                    var item = _Items[i];
                    switch (item)
                    {
                        case nameof(ModulationType.Period):
                        case nameof(ModulationType.Frequency):
                            {
                                values = new List<(Double value, Int32 Index)>();
                                for (Int32 ii = 0, pl = meas.Periods.Count(); ii < pl; ii++)
                                {
                                    var value = (meas.Periods[ii].End - meas.Periods[ii].Start) / data.Value.Fs; /// data.Value.Fs;
                                    if (item.Equals(nameof(ModulationType.Frequency)))
                                    {
                                        value = 1 / value;
                                    }
                                    values.Add((value, (Int32)Math.Round(meas.Periods[ii].End)));
                                }
                                if (item.Equals(nameof(ModulationType.Period)))
                                {
                                    Double period = values.Average(v => v.Value);
                                    _Modulation[nameof(ModulationType.Period)].Current = period != null && !Double.IsNaN(period)
                                        ? Quantity.ConvertByPrefix(period!, MeasureProc.GetPfxUnitString(nameof(ModulationType.Period), _Source).Prefix)
                                        : Double.NaN;
                                    _ModulationItems[ModulationType.Period].CurrentArray = values;
                                }
                                else
                                {
                                    Double frq = values.Average(v => v.Value);
                                    _Modulation[nameof(ModulationType.Frequency)].Current = frq != null && !Double.IsNaN(frq)
                                        ? Quantity.ConvertByPrefix(frq!, MeasureProc.GetPfxUnitString(nameof(ModulationType.Frequency), _Source).Prefix)
                                        : Double.NaN;
                                    _ModulationItems[ModulationType.Frequency].CurrentArray = values;
                                }
                            }
                            break;
                        case nameof(ModulationType.PDuty):
                        case nameof(ModulationType.PWidth):
                            {
                                if (meas.PosPulses != null && meas.PosPulses.Count > 0)//正常情况下正脉冲和周期数一样
                                {
                                    values = new List<(Double value, Int32 Index)>();
                                    for (Int32 ii = 0, pl = meas.PosPulses.Count(); ii < pl; ii++)
                                    {
                                        var value = (meas.PosPulses[ii].End - meas.PosPulses[ii].Start) / data.Value.Fs;// / data.Value.Fs;
                                        if (item.Equals(nameof(ModulationType.PDuty)))//占空比 
                                        {
                                            if (meas.Periods.Count >= ii + 1)
                                            {
                                                var per = (meas.Periods[ii].End - meas.Periods[ii].Start) / data.Value.Fs;// / data.Value.Fs;
                                                value = value / per * 100;
                                            }
                                            else
                                            {
                                                value = Double.NaN;
                                            }
                                        }
                                        if (value.IsFinite())
                                        {
                                            values.Add((value, (Int32)Math.Round(meas.PosPulses[ii].End)));
                                        }
                                    }
                                    if (item.Equals(nameof(ModulationType.PDuty)))
                                    {
                                        Double pduty = values.Average(v => v.Value);
                                        if (pduty > 100)
                                        {
                                            pduty = _Meas.Calc.ForceGetResultOrCalc("Duty", _Source)!.Value;
                                        }
                                        _Modulation[nameof(ModulationType.PDuty)].Current = pduty != null && !Double.IsNaN(pduty)
                                            ? Quantity.ConvertByPrefix(pduty!, MeasureProc.GetPfxUnitString(nameof(ModulationType.PDuty), _Source).Prefix)
                                            : Double.NaN;
                                        _ModulationItems[ModulationType.PDuty].CurrentArray = values;
                                    }
                                    else
                                    {
                                        Double pwidth = values.Average(v => v.Value);
                                        _Modulation[nameof(ModulationType.PWidth)].Current = pwidth != null && !Double.IsNaN(pwidth)
                                            ? Quantity.ConvertByPrefix(pwidth!, MeasureProc.GetPfxUnitString(nameof(ModulationType.PWidth), _Source).Prefix)
                                            : Double.NaN;
                                        _ModulationItems[ModulationType.PWidth].CurrentArray = values;
                                    }
                                }
                                else
                                {
                                    _Modulation[nameof(ModulationType.PDuty)].Current = Double.NaN;
                                    _Modulation[nameof(ModulationType.PWidth)].Current = Double.NaN;
                                    _ModulationItems[ModulationType.PDuty].CurrentArray.Clear();
                                    _ModulationItems[ModulationType.PWidth].CurrentArray.Clear();
                                }
                            }
                            break;
                        case nameof(ModulationType.NDuty):
                        case nameof(ModulationType.NWidth):
                            {
                                if (meas.NegPulses != null && meas.NegPulses.Count > 0)//正常情况下正脉冲和周期数一样
                                {
                                    values = new List<(Double value, Int32 Index)>();
                                    for (Int32 ii = 0, pl = meas.NegPulses.Count(); ii < pl; ii++)
                                    {
                                        var value = (meas.NegPulses[ii].End - meas.NegPulses[ii].Start) / data.Value.Fs;// / data.Value.Fs;
                                        if (item.Equals(nameof(ModulationType.NDuty)))//占空比 
                                        {
                                            if (meas.Periods.Count >= ii + 1)
                                            {
                                                var per = (meas.Periods[ii].End - meas.Periods[ii].Start) / data.Value.Fs;// / data.Value.Fs;
                                                value = value / per * 100;
                                            }
                                            else
                                            {
                                                value = Double.NaN;
                                            }
                                        }
                                        if (value.IsFinite())
                                        {
                                            values.Add((value, (Int32)Math.Round(meas.NegPulses[ii].End)));
                                        }
                                    }
                                    if (item.Equals(nameof(ModulationType.NDuty)))
                                    {
                                        Double nduty = values.Average(v => v.Value);
                                        if (nduty > 100)
                                        {
                                            nduty = _Meas.Calc.ForceGetResultOrCalc("NDuty", _Source)!.Value;
                                        }
                                        _Modulation[nameof(ModulationType.NDuty)].Current = nduty != null && !Double.IsNaN(nduty)
                                            ? Quantity.ConvertByPrefix(nduty, MeasureProc.GetPfxUnitString(nameof(ModulationType.NDuty), _Source).Prefix)
                                            : Double.NaN;
                                        _ModulationItems[ModulationType.NDuty].CurrentArray = values;
                                    }
                                    else
                                    {
                                        Double nwidth = values.Average(v => v.Value);
                                        _Modulation[nameof(ModulationType.NWidth)].Current = nwidth != null && !Double.IsNaN(nwidth)
                                            ? Quantity.ConvertByPrefix(nwidth!, MeasureProc.GetPfxUnitString(nameof(ModulationType.NWidth), _Source).Prefix)
                                            : Double.NaN;
                                        _ModulationItems[ModulationType.NWidth].CurrentArray = values;
                                    }
                                }
                                else
                                {
                                    _Modulation[nameof(ModulationType.NDuty)].Current = Double.NaN;
                                    _Modulation[nameof(ModulationType.NWidth)].Current = Double.NaN;
                                    _ModulationItems[ModulationType.NDuty].CurrentArray.Clear();
                                    _ModulationItems[ModulationType.NWidth].CurrentArray.Clear();
                                }
                            }
                            break;
                        case nameof(ModulationType.RiseTime):
                            {
                                if (meas.Rises != null && meas.Rises.Count > 0)//与周期数可能不一样
                                {
                                    values = new List<(Double value, Int32 Index)>();
                                    for (Int32 ii = 0, pl = meas.Rises.Count(); ii < pl; ii++)
                                    {
                                        var value = (meas.Rises[ii].End - meas.Rises[ii].Start) / data.Value.Fs;// / data.Value.Fs;
                                        values.Add((value, (Int32)Math.Round(meas.Rises[ii].End)));
                                    }
                                    Double rise = values.Average(v => v.Value);
                                    _Modulation[nameof(ModulationType.RiseTime)].Current = rise != null && !Double.IsNaN(rise)
                                        ? Quantity.ConvertByPrefix(rise!, MeasureProc.GetPfxUnitString(nameof(ModulationType.RiseTime), _Source).Prefix)
                                        : Double.NaN;
                                    _ModulationItems[ModulationType.RiseTime].CurrentArray = values;
                                }
                                else
                                {
                                    _Modulation[nameof(ModulationType.RiseTime)].Current = Double.NaN;
                                    _ModulationItems[ModulationType.RiseTime].CurrentArray.Clear();
                                }
                            }
                            break;
                        case nameof(ModulationType.FallTime):
                            {
                                if (meas.Falls != null && meas.Falls.Count > 0)//与周期数可能不一样
                                {
                                    values = new List<(Double value, Int32 Index)>();
                                    for (Int32 ii = 0, pl = meas.Falls.Count(); ii < pl; ii++)
                                    {
                                        var value = (meas.Falls[ii].End - meas.Falls[ii].Start) / data.Value.Fs;// / data.Value.Fs;
                                        values.Add((value, (Int32)Math.Round(meas.Falls[ii].End)));
                                    }
                                    Double fall = values.Average(v => v.Value);
                                    _Modulation[nameof(ModulationType.FallTime)].Current = fall != null && !Double.IsNaN(fall)
                                        ? Quantity.ConvertByPrefix(fall, MeasureProc.GetPfxUnitString(nameof(ModulationType.FallTime), _Source).Prefix)
                                        : Double.NaN;
                                    _ModulationItems[ModulationType.FallTime].CurrentArray = values;
                                }
                                else
                                {
                                    _Modulation[nameof(ModulationType.FallTime)].Current = Double.NaN;
                                    _ModulationItems[ModulationType.FallTime].CurrentArray.Clear();
                                }
                            }
                            break;
                    }

                    //if (values != null && values.Count > 0)
                    //{
                    //    _Modulation[item].Current = values.Select(x => x.Value).Average();
                    //    _Modulation[item].CurrentArray = values;
                    //}
                }
            }
            else
            {
                _Modulation[nameof(ModulationType.Period)].Current = Double.NaN;
                _Modulation[nameof(ModulationType.Frequency)].Current = Double.NaN;
                _ModulationItems[ModulationType.Period].CurrentArray.Clear();
                _ModulationItems[ModulationType.Frequency].CurrentArray.Clear();
                _Modulation[nameof(ModulationType.PDuty)].Current = Double.NaN;
                _Modulation[nameof(ModulationType.PWidth)].Current = Double.NaN;
                _ModulationItems[ModulationType.PDuty].CurrentArray.Clear();
                _ModulationItems[ModulationType.PWidth].CurrentArray.Clear();
                _Modulation[nameof(ModulationType.NDuty)].Current = Double.NaN;
                _Modulation[nameof(ModulationType.NWidth)].Current = Double.NaN;
                _ModulationItems[ModulationType.NDuty].CurrentArray.Clear();
                _ModulationItems[ModulationType.NWidth].CurrentArray.Clear();
                _Modulation[nameof(ModulationType.RiseTime)].Current = Double.NaN;
                _ModulationItems[ModulationType.RiseTime].CurrentArray.Clear();
                _Modulation[nameof(ModulationType.FallTime)].Current = Double.NaN;
                _ModulationItems[ModulationType.FallTime].CurrentArray.Clear();
            }
        }

        public Double? GetLocationByIndex(Double value)
        {
            var chnl = DsoModel.Default.GetChannel(_Source);
            var pkg = chnl.Pack;
            if (pkg is null)
            {
                return null;
            }

            var ratio = (chnl.Sampling.Scale * 1E-6 / chnl.Sampling.PosIdxPerDiv) / pkg.Properties.SampInterval;
            var start = chnl.Sampling.PosIndex - (pkg.Properties.TmbPosition.Index - pkg.Properties.VuStartIndex) * pkg.Properties.TmbScale.Value / chnl.Sampling.Scale;
            return (value - pkg.Offset) / ratio + start;
        }
        public void Reset()
        {
            lock (_Modulation)
            {
                foreach (var item in _Items)
                {
                    _Modulation[item].Clear();
                }
                foreach (var item in _ModulationItems)
                {
                    ModulationItems[item.Key].CurrentArray.Clear();
                    _ModulationItems[item.Key].ClearFlag = true;
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

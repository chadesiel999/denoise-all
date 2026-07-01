using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Jitter;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    public class JitterPrsnt : MulticastPrsnt<IJitterView>, IJitterPrsnt, IBadge
    {
        public JitterPrsnt(IDsoPrsnt idp, IJitterView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.JitterModel,
                ModelCreateOptions.Standalone => new JitterModel(ChannelId.JITTER),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };
            Model.PropertyChanged += OnPropertyChanged;
            DrawColor = ColorLookup.Default[Id.ToString()];
            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }


            var jittermaths = ChannelIdExt.GetJitterMaths().ToList();
            _GraphPrsntTable.Add(JitterGraphType.Histogram, new JitterGraphPrsnt(Model.HistogramGraph) { DestMathChannel = jittermaths[(Int32)JitterGraphType.Histogram] });
            _GraphPrsntTable.Add(JitterGraphType.Trend, new JitterGraphPrsnt(Model.TrendGraph) { DestMathChannel = jittermaths[(Int32)JitterGraphType.Trend] });
            _GraphPrsntTable.Add(JitterGraphType.Spectrum, new JitterGraphPrsnt(Model.SpectrumGraph) { DestMathChannel = jittermaths[(Int32)JitterGraphType.Spectrum] });
            _GraphPrsntTable.Add(JitterGraphType.QFactor, new JitterGraphPrsnt(Model.QFactorGraph) { DestMathChannel = jittermaths[(Int32)JitterGraphType.QFactor] });
            _GraphPrsntTable.Add(JitterGraphType.Bathtub, new JitterGraphPrsnt(Model.BathtubGraph) { DestMathChannel = jittermaths[(Int32)JitterGraphType.Bathtub] });
            _GraphPrsntTable.Add(JitterGraphType.Eye, new JitterGraphPrsnt(Model.EyeGraph) { DestMathChannel = jittermaths[(Int32)JitterGraphType.Eye] });
        }

        private protected override JitterModel Model { get; }

        public Boolean Active
        {
            get => Model.Active;
            set
            {
                if (!Constants.ENABLE_SDA && value)
                {
                    WeakTip.Default.Write("Jitter", MsgTipId.FunctionDisabled);
                    Model.Active = false;
                    return;
                }
                if (value && Model.Active != value)
                {
                    if (FunctionLimit.JitterFunctionLimit(((DsoPrsnt)Dso).MutexFunctionFlag) == false)
                    {
                        return;
                    }
                    DsoPrsnt.DefaultDsoPrsnt.Timebase.LimitScan(MsgTipId.JitterIsNotSupportedInScan);
                    AcqLimit();
                    //if (!DsoPrsnt.DefaultDsoPrsnt.CheckLAMutex(true))
                    //    return;
                }
                if (!value)
                {
                    ClearGraph();
                }
                Model.Active = value;
                if (value)
                {
                    JitterParamEnable = true;
                }
            }
        }


        public Boolean NeedRecordData
        {
            get => Model.NeedRecordData;
            set
            {
                if (Model.NeedRecordData != value)
                {
                    Model.NeedRecordData = value;
                }
            }
        }
        private void AcqLimit()
        {
            if (DsoPrsnt.DefaultDsoPrsnt.Timebase.Mode == AnaChnlAcqMode.Average)
            {
                DsoPrsnt.DefaultDsoPrsnt.Timebase.Mode = AnaChnlAcqMode.Normal;
            }
            //var lengths = DsoPrsnt.DefaultDsoPrsnt.Timebase.AnaChnlLengthSource.Select(x => x.Value).ToList();
            //if (lengths[DsoPrsnt.DefaultDsoPrsnt.Timebase.StorageDepthOpt] < 10_000_000 || DsoPrsnt.DefaultDsoPrsnt.Timebase.StorageDepthOpt == 0)
            //{
            //    DsoPrsnt.DefaultDsoPrsnt.Timebase.StorageDepthOpt = DsoPrsnt.DefaultDsoPrsnt.Timebase.AnaChnlLengthSource.ToList().FindIndex(x => x.Key != "Auto" && x.Value >= 10_000_000);
            //}
        }

        public Boolean EnableMeas { get => Model.EnableMeas; set => Model.EnableMeas = value; }

        public Int32 HistCount => Model.HistItemCount;

        public JitterItem HistTjBER => new(Model.GetHistItem("TjBER").Current, QuantityUnit.Second);

        public JitterItem HistTj => new(Model.GetHistItem("Tj").Current, QuantityUnit.Second);

        public JitterItem HistRj => new(Model.GetHistItem("Rj").Current, QuantityUnit.Second);

        public JitterItem HistDj => new(Model.GetHistItem("Dj").Current, QuantityUnit.Second);

        public IEnumerable<JitterItem> HistItems
        {
            get
            {
                yield return HistTjBER;
                yield return HistTj;
                yield return HistRj;
                yield return HistDj;
            }
        }

        public Int32 PatCount => Model.PatItemCount;

        public JitterItem TIEMean => new(Model.GetPatItem(nameof(TIEMean)).Current, QuantityUnit.Second);

        public JitterItem TIEPeak => new(Model.GetPatItem(nameof(TIEPeak)).Current, QuantityUnit.Second);
        public JitterItem TIEMax => new(Model.GetPatItem(nameof(TIEPeak)).Max, QuantityUnit.Second);
        public JitterItem TIEMin => new(Model.GetPatItem(nameof(TIEPeak)).Min, QuantityUnit.Second);
        public JitterItem TIECount => new(Model.GetPatItem(nameof(TIEPeak)).Count, QuantityUnit.Count);

        public JitterItem TIERms => new(Model.GetPatItem(nameof(TIERms)).Current, QuantityUnit.Second);

        public JitterItem PjMean => new(Model.GetPatItem(nameof(PjMean)).Current, QuantityUnit.Second);

        public JitterItem PjPeak => new(Model.GetPatItem(nameof(PjPeak)).Current, QuantityUnit.Second);

        public JitterItem PjRms => new(Model.GetPatItem(nameof(PjRms)).Current, QuantityUnit.Second);

        public JitterItem CCjMean => new(Model.GetPatItem(nameof(CCjMean)).Current, QuantityUnit.Second);

        public JitterItem CCjPeak => new(Model.GetPatItem(nameof(CCjPeak)).Current, QuantityUnit.Second);

        public JitterItem CCjRms => new(Model.GetPatItem(nameof(CCjRms)).Current, QuantityUnit.Second);

        public IEnumerable<JitterItem> PatItems
        {
            get
            {
                yield return TIEMean;
                yield return TIEPeak;
                yield return TIERms;
                yield return HistDj;
            }
        }

        public Int32 SpecCount => Model.SpecItemCount;

        public JitterItem SpecTj => new(Model.GetSpecItem("Tj").Current, QuantityUnit.Second);

        public JitterItem SpecRj => new(Model.GetSpecItem("Rj").Current, QuantityUnit.Second);

        public JitterItem SpecDDj => new(Model.GetSpecItem("DDj").Current, QuantityUnit.Second);

        public JitterItem SpecDCD => new(Model.GetSpecItem("DCD").Current, QuantityUnit.Second);

        public JitterItem SpecPj => new(Model.GetSpecItem("Pj").Current, QuantityUnit.Second);

        public JitterItem SpecISI => new(Model.GetSpecItem("ISI").Current, QuantityUnit.Second);

        public void TryShowEyeWfm(MathPrsnt mp)
        {
            //var pwrexp = $"{MathType.Custom}:{Model.Analysis.VoltageSrc}*{Model.Analysis.CurrentSrc}";
            //if (mp.Formula != pwrexp)
            //{
            //    mp.GetOrMakeArg(MathType.Custom);
            //    mp.Formula = pwrexp;
            //}

            //mp.Args.Occupier = Model.Analysis;
            //mp.Label = "Eye";
            //mp.Active = true;
        }

        public Boolean FastEye { get => Model.FastEye; set => Model.FastEye = value; }


        /// <summary>
        /// Gets or sets the BitRate.
        /// </summary>
        public Double BitRate { get => Model.BitRate; set => Model.BitRate = value; }

        /// <summary>
        /// Gets or sets the CutoffDivisor.
        /// </summary>
        public Double CutoffDivisor { get => Model.CutoffDivisor; set => Model.CutoffDivisor = value; }

        /// <summary>
        /// Gets or sets the CutoffFreq.
        /// </summary>
        public Double CutoffFreq1 { get => Model.CutoffFreq1; set => Model.CutoffFreq1 = value; }

        /// <summary>
        /// Gets or sets the CutoffFreq.
        /// </summary>
        public Double CutoffFreq2 { get => Model.CutoffFreq2; set => Model.CutoffFreq2 = value; }

        /// <summary>
        /// Gets or sets the DamplingFactor.
        /// </summary>
        public Double DamplingFactor { get => Model.DamplingFactor; set => Model.DamplingFactor = value; }

        /// <summary>
        /// Gets or sets the EnableBitRateSearch.
        /// </summary>
        public Boolean EnableBitRateSearch { get => Model.EnableBitRateSearch; set => Model.EnableBitRateSearch = value; }

        public Boolean EnableRefClk { get => Model.EnableRefClk; set => Model.EnableRefClk = value; }

        public Boolean EnablePll { get => Model.EnablePll; set => Model.EnablePll = value; }

        /// <summary>
        /// Gets or sets the Hysteresis.
        /// </summary>
        public Double Hysteresis { get => Model.Hysteresis; set => Model.Hysteresis = value; }

        /// <summary>
        /// Gets or sets the IsMatlabMode.
        /// </summary>
        public Boolean IsMatlabMode { get => Model.IsMatlabMode; set => Model.IsMatlabMode = value; }

        /// <summary>
        /// Gets the MaxBitRate.
        /// </summary>
        public Double MaxBitRate => Model.MaxBitRate;

        /// <summary>
        /// Gets the MaxCutDivisor.
        /// </summary>
        public Double MaxCutDivisor => Model.MaxCutDivisor;

        /// <summary>
        /// Gets the MaxCutoffFreq.
        /// </summary>
        public Double MaxCutoffFreq => Model.MaxCutoffFreq;

        /// <summary>
        /// Gets the MaxDamplingFactor.
        /// </summary>
        public Double MaxDamplingFactor => Model.MaxDamplingFactor;

        /// <summary>
        /// Gets the MaxHysteresis.
        /// </summary>
        public Double MaxHysteresis => Model.MaxHysteresis;

        /// <summary>
        /// Gets the MaxNaturalFreq.
        /// </summary>
        public Double MaxNaturalFreq => Model.MaxNaturalFreq;

        /// <summary>
        /// Gets the MaxRefClkDeskew.
        /// </summary>
        public Double MaxRefClkDeskew => Model.MaxRefClkDeskew;

        /// <summary>
        /// Gets the MaxThreshold.
        /// </summary>
        public Double MaxThreshold => Model.MaxThreshold;
        public Double MaxTopThreshold => Model.MaxTopThreshold;
        public Double MaxBaseThreshold => Model.MaxBaseThreshold;

        /// <summary>
        /// Gets the MinBitRate.
        /// </summary>
        public Double MinBitRate => Model.MinBitRate;

        /// <summary>
        /// Gets the MinCutDivisor.
        /// </summary>
        public Double MinCutDivisor => Model.MinCutDivisor;

        /// <summary>
        /// Gets the MinCutoffFreq.
        /// </summary>
        public Double MinCutoffFreq => Model.MinCutoffFreq;

        /// <summary>
        /// Gets the MinDamplingFactor.
        /// </summary>
        public Double MinDamplingFactor => Model.MinDamplingFactor;

        /// <summary>
        /// Gets the MinHysteresis.
        /// </summary>
        public Double MinHysteresis => Model.MinHysteresis;

        /// <summary>
        /// Gets the MinNaturalFreq.
        /// </summary>
        public Double MinNaturalFreq => Model.MinNaturalFreq;

        /// <summary>
        /// Gets the MinRefClkDeskew.
        /// </summary>
        public Double MinRefClkDeskew => Model.MinRefClkDeskew;

        /// <summary>
        /// Gets the MinThreshold.
        /// </summary>
        public Double MinThreshold => Model.MinThreshold;
        public Double MinTopThreshold => Model.MinTopThreshold;
        public Double MinBaseThreshold => Model.MinBaseThreshold;

        /// <summary>
        /// Gets or sets the NaturalFreq.
        /// </summary>
        public Double NaturalFreq { get => Model.NaturalFreq; set => Model.NaturalFreq = value; }

        /// <summary>
        /// Gets or sets the ClockType.
        /// </summary>
        public ClockTypeOpt ClockType { get => Model.ClockType; set => Model.ClockType = value; }

        /// <summary>
        /// Gets or sets the PllType.
        /// </summary>
        public PllTypeOpt PllType { get => Model.PllType; set => Model.PllType = value; }

        /// <summary>
        /// Gets or sets the RefClkDeskew.
        /// </summary>
        public Double RefClkDeskew { get => Model.RefClkDeskew; set => Model.RefClkDeskew = value; }

        /// <summary>
        /// Gets or sets the RefClkSlope.
        /// </summary>
        public EdgeSlope RefClkSlope { get => Model.RefClkSlope; set => Model.RefClkSlope = value; }

        /// <summary>
        /// Gets or sets the RefClkSource.
        /// </summary>
        public ChannelId RefClkSource { get => Model.RefClkSource; set => Model.RefClkSource = value; }

        /// <summary>
        /// Gets or sets the RefClkTholdType.
        /// </summary>
        public TholdTypeOpt RefClkTholdType { get => Model.RefClkTholdType; set => Model.RefClkTholdType = value; }

        /// <summary>
        /// Gets or sets the RefClkThreshold.
        /// </summary>
        public Double RefClkThreshold { get => Model.RefClkThreshold; set => Model.RefClkThreshold = value; }

        /// <summary>
        /// Gets or sets the Slope.
        /// </summary>
        public EdgeSlope Slope { get => Model.Slope; set => Model.Slope = value; }

        /// <summary>
        /// Gets or sets the Source.
        /// </summary>
        public ChannelId Source { get => Model.Source; set => Model.Source = value; }

        /// <summary>
        /// Gets or sets the TholdType.
        /// </summary>
        public TholdTypeOpt TholdType { get => Model.TholdType; set => Model.TholdType = value; }

        /// <summary>
        /// Gets or sets the Threshold.
        /// </summary>
        public Double Threshold { get => Model.Threshold; set => Model.Threshold = value; }
        public Double TopThreshold { get => Model.TopThreshold; set => Model.TopThreshold = value; }
        public Double BaseThreshold { get => Model.BaseThreshold; set => Model.BaseThreshold = value; }

        public Double ThresholdFreq
        {
            get => Model.ThresholdFreq;
            set => Model.ThresholdFreq = value;
        }

        public JitterSignalType SignalType { get => Model.SignalType; set => Model.SignalType = value; }

        public Int32 PatternLength
        {
            get => Model.PatternLength;
            set => Model.PatternLength = value;
        }

        public Double BinWidth { get => Model.BinWidth; set => Model.BinWidth = value; }
        public MaxBinNum CurrentBinNum { get => Model.CurrentBinNum; set => Model.CurrentBinNum = value; }

        public Double EyeSaturation { get => Model.EyeSaturation; set => Model.EyeSaturation = value; }

        public StatisticalConstructionMode ConstructionMode { get => Model.ConstructionMode; set => Model.ConstructionMode = value; }

        public Boolean EyeEnable
        {
            get
            {
                return Model.EyeEnable;
            }
            set
            {
                if (Model.EyeEnable != value)
                {
                    SetGraphEnable(JitterGraphType.Eye, value);
                    Model.EyeEnable = value;
                    EyeParamEnable = value;
                }
            }
        }

        public Boolean EyeParamEnable
        {
            get
            {
                if (!EyeEnable)
                {
                    Model.EyeParamEnable = false;
                }

                return Model.EyeParamEnable;
            }
            set
            {
                Model.EyeParamEnable = value;
            }
        }

        public Boolean JitterParamEnable
        {
            get => Model.JitterParamEnable;
            set
            {
                if (value && Active != true)
                {
                    return;
                }
                Model.JitterParamEnable = value;
            }
        }



        public Dictionary<String, String> EyeParamTable
        {
            get { return Model.EyeParamTable; }
        }

        public Dictionary<String, String> JitterParamTable
        {
            get { return Model.JitterParamTable; }
        }


        public JitterGraphType CurGraphType { get; set; } = JitterGraphType.Histogram;

        private Dictionary<JitterGraphType, AdvancedMathPrsnt> _GraphPrsntTable = new();
        private static readonly Object _Locker = new Object();
        public AdvancedMathPrsnt? GetCurGraphPrsnt(JitterGraphType type)
        {
            if (!_GraphPrsntTable.TryGetValue(type, out AdvancedMathPrsnt? graphprsnt))
            {
                return null;
            }
            return graphprsnt;
        }

        public void CheckMathPrsnt(ChannelId mathid)
        {
            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(mathid, out var mathprsnt) && mathprsnt is MathPrsnt math && math != null)
            {
                if (math.Args.Occupier == null)
                {
                    math.GetOrMakeArg(MathType.Custom);
                }
            }
        }

        public Boolean SetGraphEnable(JitterGraphType graphType, Boolean state)
        {
            lock (_Locker)
            {
                if (_GraphPrsntTable.TryGetValue(graphType, out AdvancedMathPrsnt? graphprsnt))
                {
                    CheckMathPrsnt(graphprsnt.DestMathChannel);
                    graphprsnt.Enabled = state;
                    return true;
                }
                return false;
            }
        }
        public Boolean IsEnableGraphByType(JitterGraphType type)
        {
            lock (_Locker)
            {
                if (_GraphPrsntTable.TryGetValue(type, out AdvancedMathPrsnt? graphprsnt) && (graphprsnt == null || !graphprsnt.Enabled))
                {
                    return false;
                }
                return true;
            }
        }

        public Boolean IsAllEnableGraph(JitterGraphType? exclusion = null)
        {
            lock (_Locker)
            {
                foreach (var type in _GraphPrsntTable.Keys)
                {
                    if ((exclusion != null && type == exclusion.Value) || type == JitterGraphType.Eye)
                        continue;

                    if (_GraphPrsntTable.TryGetValue(type, out AdvancedMathPrsnt? graphprsnt) && !graphprsnt.Enabled)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public Boolean IsAnyEnableGraph()
        {
            lock (_Locker)
            {
                if (_GraphPrsntTable.Values.Any(x => x != null && x.Enabled))
                {
                    return true;
                }
                return false;
            }
        }

        public (Double StepValue, Double Offset)? GetEyeVerticalParams(Double step, Double stepLength) => Model.GetEyeVerticalParams(step,stepLength);

        public void ClearGraph()
        {
            lock (_Locker)
            {
                EyeEnable = false;
                JitterParamEnable = false;
                foreach (var item in Enum.GetValues<JitterGraphType>())
                {
                    var advancedmath = GetCurGraphPrsnt(item);
                    if (advancedmath != null)
                    {
                        advancedmath.Enabled = false;
                    }
                }

            }

        }

        public Boolean EnableChannelSim
        {
            get => Model.EnableChannelSim;
            set => Model.EnableChannelSim = value;
        }

        public Boolean EnableRxFFE
        {
            get => Model.EnableRxFFE;
            set => Model.EnableRxFFE = value;
        }

        public String S2PPath
        {
            get => Model.S2PPath;
            set => Model.S2PPath = value;
        }

        public String TapPath
        {
            get => Model.TapPath;
            set => Model.TapPath = value;
        }

        public ChannelType Type => throw new NotImplementedException();

        public ChannelId Id => Model.Id;

        public String Name => Model.Name;

        private Color _DrawColor;

        public Color DrawColor
        {
            get => _DrawColor;
            set
            {
                if (_DrawColor != value)
                {
                    _DrawColor = value;
                    OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(DrawColor)));
                }
            }
        }
    }
}

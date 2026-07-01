using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.Measure;
using System.Collections.Concurrent;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;
using ScopeX.Core.Model.Jitter.Eye;
using ScopeX.Core.Model.Jitter.Common;
using NPOI.SS.Formula.Functions;
using NPOI.POIFS.Crypt.Dsig;

namespace ScopeX.Core
{
    internal class JitterModel : INotifyPropertyChanged
    {

        /// <summary>
        /// 最大获取的深存储数据量
        /// </summary>
        public readonly Int32 MaxDataLength = PlatformManager.Default.Platform.JitterMaxDataLength;

        public JitterModel(ChannelId id)
        {
            Id = id;
            Name = id.ToString();
            Spectrum = new();
            Statistical = new();
            _Eye = new EyeMethod();

            _Parameter = new();
            _Jitter = new();
            _CurrentAnalyzer = Statistical.MJSQ;

            HistogramGraph = new(JitterGraphType.Histogram.GetDescription(), DrawMethod.Bar);
            TrendGraph = new(JitterGraphType.Trend.GetDescription());
            SpectrumGraph = new(JitterGraphType.Spectrum.GetDescription());
            QFactorGraph = new(JitterGraphType.QFactor.GetDescription());
            BathtubGraph = new(JitterGraphType.Bathtub.GetDescription());
            EyeGraph = new(JitterGraphType.Eye.GetDescription(), DrawMethod.DPX);

            _HistItems = new ConcurrentDictionary<String, JitterItems>()
            {
                ["TjBER"] = new JitterItems(),
                ["Tj"] = new JitterItems(),
                ["Rj"] = new JitterItems(),
                ["Dj"] = new JitterItems(),
            };
            HistItemCount = _HistItems.Count;

            _PatItems = new ConcurrentDictionary<String, JitterItems>()
            {
                ["TIEMean"] = new JitterItems(),
                ["TIEPeak"] = new JitterItems(),
                ["TIERms"] = new JitterItems(),
                ["PjMean"] = new JitterItems(),
                ["PjPeak"] = new JitterItems(),
                ["PjRms"] = new JitterItems(),
                ["CCjMean"] = new JitterItems(),
                ["CCjPeak"] = new JitterItems(),
                ["CCjRms"] = new JitterItems(),
            };
            PatItemCount = _PatItems.Count;

            _SpecItems = new ConcurrentDictionary<String, JitterItems>()
            {
                ["Tj"] = new JitterItems(),
                ["Rj"] = new JitterItems(),
                ["DDj"] = new JitterItems(),
                ["DCD"] = new JitterItems(),
                ["Pj"] = new JitterItems(),
                ["ISI"] = new JitterItems(),
            };
            SpecItemCount = _SpecItems.Count;

            _DecompositionType = JitterDecompositionType.MJSQ;

            SignalIntegrityMatlab.AsyncInit();

            CutoffFreq1 = BitRate / CutoffDivisor;
        }

        public ChannelId Id
        {
            get;
        }

        public String Name
        {
            get;
        }
        //public static readonly JitterModel Default = new JitterModel();
        private static readonly Object _Locker = new();

        public readonly JitterGraphModel HistogramGraph;

        public readonly JitterGraphModel TrendGraph;

        public readonly JitterGraphModel SpectrumGraph;

        public readonly JitterGraphModel QFactorGraph;

        public readonly JitterGraphModel BathtubGraph;

        public readonly JitterGraphModel EyeGraph;



        public Boolean FastEye
        {
            get
            {
                return _Eye.FastEye;
            }
            set
            {
                if (value != _Eye.FastEye)
                {
                    _Eye.FastEye = value;
                    _Eye?.ClearEyeUiBuffer();
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsNeedResetOHistoryValue = false;

        private Double _BitRate = Double.NaN; //Constants.MAX_BIT_RATE;

        /// <summary>
        /// Gets or sets the BitRate.
        /// </summary>
        public Double BitRate
        {
            get => _BitRate;
            set
            {
                value = ValidateBitRate(value);
                if (_BitRate != value)
                {
                    _BitRate = value;

                    _CutoffFreq1 = ValidateCutoffFreq1(_BitRate / CutoffDivisor, MaxCutoffFreq);
                    _NaturalFreq = ValidateNaturalFreq(_NaturalFreq);
                    OnPropertyChanged();
                    OnPropertyChanged("CutoffFreq1");
                    EnableBitRateSearch = false;
                }
            }
        }

        public readonly Double MaxBitRate = Constants.MAX_BIT_RATE;

        public readonly Double MinBitRate = Constants.MIN_BIT_RATE;

        private Double ValidateBitRate(Double value)
        {
            value = Math.Round(value, 7, MidpointRounding.AwayFromZero);
            if (value > MaxBitRate)
            {
                value = MaxBitRate;
            }
            else if (value < MinBitRate)
            {
                value = MinBitRate;
            }

            return value;
        }

        private Double _CutoffDivisor = Constants.STD_PLL_CUTOFF_DIVISOR;

        /// <summary>
        /// Gets or sets the CutDivisor.
        /// </summary>
        public Double CutoffDivisor
        {
            get => _CutoffDivisor;
            set
            {
                value = ValidateCutoffDivisor(value);
                if (_CutoffDivisor != value)
                {
                    _CutoffDivisor = value;

                    var freq = BitRate / value;
                    //if (RespType == FilterResponseType.BandPass)
                    //{
                    //    freq = ValidateCutoffFreq1(freq, CutoffFreq2 - GapCutoffFreq);
                    //}
                    //else
                    //{
                    freq = ValidateCutoffFreq1(freq, MaxCutoffFreq);
                    //}
                    _CutoffFreq1 = freq;
                    OnPropertyChanged();
                    OnPropertyChanged("CutoffFreq1");
                }
            }
        }

        public readonly Double MaxCutDivisor = Constants.MAX_PLL_CUTOFF_DIVISOR;

        public readonly Double MinCutDivisor = Constants.MIN_PLL_CUTOFF_DIVISOR;

        private Double ValidateCutoffDivisor(Double value)
        {
            value = Math.Truncate(value);

            if (value > MaxCutDivisor)
            {
                value = MaxCutDivisor;
            }
            else if (value < MinCutDivisor)
            {
                value = MinCutDivisor;
            }

            return value;
        }

        private Double _DamplingFactor = Constants.STD_PLL_DAMPLING_FACTOR;
        /// <summary>
        /// Gets or sets the DamplingFactor.
        /// </summary>
        public Double DamplingFactor
        {
            get => _DamplingFactor;
            set
            {
                value = ValidateDamplingFactor(value);
                if (_DamplingFactor != value)
                {
                    _DamplingFactor = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxDamplingFactor = Constants.MAX_PLL_DAMPLING_FACTOR;

        public readonly Double MinDamplingFactor = Constants.MIN_PLL_DAMPLING_FACTOR;

        private Double ValidateDamplingFactor(Double value)
        {
            value = Math.Round(value, 3, MidpointRounding.AwayFromZero);
            if (value > MaxDamplingFactor)
            {
                value = MaxDamplingFactor;
            }
            else if (value < MinDamplingFactor)
            {
                value = MinDamplingFactor;
            }

            return value;
        }

        private Boolean _EnableBitRateSearch = true;
        /// <summary>
        /// Gets or sets the EnableBitRateSearch.
        /// </summary>
        public Boolean EnableBitRateSearch
        {
            get => _EnableBitRateSearch;
            set
            {
                if (_EnableBitRateSearch != value)
                {
                    _EnableBitRateSearch = value;
                    OnPropertyChanged();
                }
            }
        }
        private Double _Hysteresis = Constants.DEF_HYSTERESIS_DIV;
        /// <summary>
        /// Gets or sets the Hysteresis.
        /// Based on mdiv
        /// </summary>
        public Double Hysteresis
        {
            get => _Hysteresis;
            set
            {
                value = ValidateHysteresis(value);
                if (_Hysteresis != value)
                {
                    _Hysteresis = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxHysteresis = Constants.MAX_HYSTERESIS_DIV;

        public readonly Double MinHysteresis = Constants.MIN_HYSTERESIS_DIV;

        private Double ValidateHysteresis(Double value)
        {
            value = Math.Round(value, 3, MidpointRounding.AwayFromZero);
            if (value > MaxHysteresis)
            {
                value = MaxHysteresis;
            }
            else if (value < MinHysteresis)
            {
                value = MinHysteresis;
            }

            return value;
        }

        /// <summary>
        /// Gets or sets the IsMatlabMode.
        /// </summary>
        public Boolean IsMatlabMode { get; set; } = true;

        private Double _NaturalFreq;
        /// <summary>
        /// Gets or sets the NaturalFreq.
        /// </summary>
        public Double NaturalFreq
        {
            get => _NaturalFreq;
            set
            {
                value = ValidateNaturalFreq(value);
                if (_NaturalFreq != value)
                {
                    _NaturalFreq = value;
                    IsNeedResetOHistoryValue = true;
                    OnPropertyChanged();
                }
            }
        }

        public Double MaxNaturalFreq => (BitRate / Constants.MIN_FREQ_FAC) / GetNaturalFreqCoeff();

        public Double MinNaturalFreq => (BitRate / Constants.MAX_FREQ_FAC) / GetNaturalFreqCoeff();

        private Double GetNaturalFreqCoeff()
        {
            var d = 2 * DamplingFactor * DamplingFactor + 1;
            return Math.Sqrt(d + Math.Sqrt(d * d + 1));
        }

        private Double ValidateNaturalFreq(Double value)
        {
            value = Math.Round(value, 7, MidpointRounding.AwayFromZero);

            if (value > MaxNaturalFreq)
            {
                value = MaxNaturalFreq;
            }
            else if (value < MinNaturalFreq)
            {
                value = MinNaturalFreq;
            }

            return Math.Round(value, 7, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Gets or sets the NominalRate.
        /// Based on bit/s, editable when SignalType is custom
        /// </summary>
        public Double NominalRate { get; set; } = 1E6;

        private ClockTypeOpt _ClockType = ClockTypeOpt.Constant;
        /// <summary>
        /// Gets or sets the ClockType.
        /// </summary>
        public ClockTypeOpt ClockType
        {
            get => _ClockType;
            set
            {
                if (_ClockType != value)
                {
                    _ClockType = value;
                    IsNeedResetOHistoryValue = true;
                    OnPropertyChanged();
                }
            }
        }

        private PllTypeOpt _PllType = PllTypeOpt.Golden;
        /// <summary>
        /// Gets or sets the PllType.
        /// </summary>
        public PllTypeOpt PllType
        {
            get => _PllType;
            set
            {
                if (_PllType != value)
                {
                    _PllType = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _EnablePll = true;

        public Boolean EnablePll
        {
            get => _EnablePll;
            set
            {
                if (_EnablePll != value)
                {
                    _EnablePll = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _EnableRefClk = false;

        public Boolean EnableRefClk
        {
            get => _EnableRefClk;
            set
            {
                if (_EnableRefClk != value)
                {
                    _EnableRefClk = value;
                    OnPropertyChanged();
                }
            }
        }

        private Double _RefClkDeskew = Constants.MIN_REF_CLK_DESKEW_PS;
        /// <summary>
        /// Gets or sets the RefClkDeskew.
        /// Based on picosecond.
        /// </summary>
        public Double RefClkDeskew
        {
            get => _RefClkDeskew;
            set
            {
                value = ValidateRefClkDeskew(value);
                if (_RefClkDeskew != value)
                {
                    _RefClkDeskew = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxRefClkDeskew = Constants.MAX_REF_CLK_DESKEW_PS;

        public readonly Double MinRefClkDeskew = Constants.MIN_REF_CLK_DESKEW_PS;

        private Double ValidateRefClkDeskew(Double value)
        {
            value = Math.Round(value, 7, MidpointRounding.AwayFromZero);

            if (value > MaxRefClkDeskew)
            {
                value = MaxRefClkDeskew;
            }
            else if (value < MinRefClkDeskew)
            {
                value = MinRefClkDeskew;
            }

            return Math.Round(value, 7, MidpointRounding.AwayFromZero);
        }

        private EdgeSlope _RefClkSlope = EdgeSlope.Rise;
        /// <summary>
        /// Gets or sets the RefClkSlope.
        /// </summary>
        public EdgeSlope RefClkSlope
        {
            get => _RefClkSlope;
            set
            {
                if (_RefClkSlope != value)
                {
                    _RefClkSlope = value;
                    OnPropertyChanged();
                }
            }
        }

        private ChannelId _RefClkSource = ChannelId.C1;
        /// <summary>
        /// Gets or sets the RefClkSource.
        /// </summary>
        public ChannelId RefClkSource
        {
            get => _RefClkSource;
            set
            {
                if (_RefClkSource != value)
                {
                    _RefClkSource = value;
                    OnPropertyChanged();
                }
            }
        }


        private TholdTypeOpt _RefClkTholdType = TholdTypeOpt.Percent;
        /// <summary>
        /// Gets or sets the RefClkTholdType.
        /// </summary>
        public TholdTypeOpt RefClkTholdType
        {
            get => _RefClkTholdType;
            set
            {
                if (_RefClkTholdType != value)
                {
                    _RefClkTholdType = value;
                    OnPropertyChanged();
                }
            }
        }

        private Double _RefClkThreshold = Constants.DEF_THRESHOLD_PERCENT;
        /// <summary>
        /// Gets or sets the RefClkThreshold.
        /// Based on percent, similar to signal threshold.
        /// </summary>
        public Double RefClkThreshold
        {
            get => _RefClkThreshold;
            set
            {
                value = ValidateThreshold(value);
                if (_RefClkThreshold != value)
                {
                    _RefClkThreshold = value;
                    OnPropertyChanged();
                }
            }
        }


        private JitterSignalType _SignalType = JitterSignalType.Custom;
        /// <summary>
        /// Gets or sets the SignalType.
        /// </summary>
        public JitterSignalType SignalType
        {
            get => _SignalType;
            set
            {
                if (_SignalType != value)
                {
                    _SignalType = value;
                    IsNeedResetOHistoryValue = true;
                    OnPropertyChanged();
                }
            }
        }

        private EdgeSlope _Slope = EdgeSlope.Rise;
        /// <summary>
        /// Gets or sets the Slope.
        /// </summary>
        public EdgeSlope Slope
        {
            get => _Slope;
            set
            {
                if (_Slope != value)
                {
                    _Slope = value;
                    IsNeedResetOHistoryValue = true;
                    OnPropertyChanged();
                }
            }
        }

        private ChannelId _Source = ChannelId.C1;
        /// <summary>
        /// Gets or sets the Source.
        /// </summary>
        public ChannelId Source
        {
            get => _Source;
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    _Eye.Source = value;
                    IsNeedResetOHistoryValue = true;
                    OnPropertyChanged();
                }
            }
        }

        private TholdTypeOpt _TholdType = TholdTypeOpt.Percent;
        /// <summary>
        /// Gets or sets the TholdType.
        /// </summary>
        public TholdTypeOpt TholdType
        {
            get => _TholdType;
            set
            {
                if (_TholdType != value)
                {
                    _TholdType = value;
                    OnPropertyChanged();
                }
            }
        }

        private Double _Threshold = Constants.DEF_THRESHOLD_PERCENT;
        /// <summary>
        /// Gets or sets the Threshold.
        /// </summary>
        public Double Threshold
        {
            get => _Threshold;
            set
            {
                value = ValidateThreshold(value);
                if (_Threshold != value)
                {
                    _Threshold = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxThreshold = Constants.MAX_THRESHOLD_PERCENT;

        public readonly Double MinThreshold = Constants.MIN_THRESHOLD_PERCENT;



        private Double _TopThreshold = Constants.DEF_TOP_THRESHOLD_PERCENT;
        /// <summary>
        /// Gets or sets the Threshold.
        /// </summary>
        public Double TopThreshold
        {
            get => _TopThreshold;
            set
            {
                value = ValidateTopThreshold(value);
                if (_TopThreshold != value)
                {
                    _TopThreshold = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxTopThreshold = Constants.MAX_TOP_THRESHOLD_PERCENT;

        public readonly Double MinTopThreshold = Constants.MIN_TOP_THRESHOLD_PERCENT;


        private Double _BaseThreshold = Constants.DEF_BASE_THRESHOLD_PERCENT;
        /// <summary>
        /// Gets or sets the Threshold.
        /// </summary>
        public Double BaseThreshold
        {
            get => _BaseThreshold;
            set
            {
                value = ValidateBaseThreshold(value);
                if (_BaseThreshold != value)
                {
                    _BaseThreshold = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxBaseThreshold = Constants.MAX_BASE_THRESHOLD_PERCENT;

        public readonly Double MinBaseThreshold = Constants.MIN_BASE_THRESHOLD_PERCENT;


        private Double _ThresholdFreq = 50;
        public Double ThresholdFreq
        {
            get => _ThresholdFreq;
            set
            {
                if (_ThresholdFreq != value)
                {
                    _ThresholdFreq = value > 100 ? 100 : value;
                    _ThresholdFreq = value < 20 ? 20 : value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean EyeEnable
        {
            get => _Eye.EyeEnable;
            set
            {
                if (_Eye.EyeEnable != value)
                {
                    _Eye.EyeEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean EyeParamEnable
        {
            get => _Eye.EyeParamEnable;
            set
            {
                if (_Eye.EyeParamEnable != value)
                {
                    _Eye.EyeParamEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _JitterParamEnable = false;
        public Boolean JitterParamEnable
        {
            get => _JitterParamEnable;
            set
            {
                if (_JitterParamEnable != value)
                {
                    _JitterParamEnable = value;
                    OnPropertyChanged();
                }
            }
        }


        private Double ValidateThreshold(Double value)
        {
            value = Math.Truncate(value);

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
        private Double ValidateTopThreshold(Double value)
        {
            value = Math.Truncate(value);

            if (value > MaxTopThreshold)
            {
                value = MaxTopThreshold;
            }
            else if (value < MinTopThreshold)
            {
                value = MinTopThreshold;
            }

            return value;
        }
        private Double ValidateBaseThreshold(Double value)
        {
            value = Math.Truncate(value);

            if (value > MaxBaseThreshold)
            {
                value = MaxBaseThreshold;
            }
            else if (value < MinBaseThreshold)
            {
                value = MinBaseThreshold;
            }

            return value;
        }

        private Int32 _MaxLength = Int32.MaxValue;
        private Int32 _MinLength = 0;
        private Int32 _PatternLength = 127;
        public Int32 PatternLength
        {
            get => _PatternLength;
            set
            {
                if (_PatternLength != value)
                {
                    if (value > _MaxLength)
                        value = _MaxLength;
                    if (value < _MinLength)
                        value = _MinLength;

                    _PatternLength = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 相对于TIE的比值
        /// </summary>
        private Double _BinWidth = 0.01;
        public Double BinWidth
        {
            get => _BinWidth;
            set
            {
                if (_BinWidth != value)
                {
                    _BinWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        private MaxBinNum _CurrentBinNum = MaxBinNum.Num250;
        public MaxBinNum CurrentBinNum
        {
            get => _CurrentBinNum;
            set
            {
                if (_CurrentBinNum != value)
                {
                    _CurrentBinNum = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _InterpolationMultiple = 1;
        public Int32 InterpolationMultiple
        {
            get => _InterpolationMultiple;
            set
            {
                if (_InterpolationMultiple != value)
                {
                    _InterpolationMultiple = value;
                    OnPropertyChanged();
                }
            }
        }

        private Double _EyeSaturation = 50;
        public Double EyeSaturation
        {
            get => _EyeSaturation;
            set
            {
                if (_EyeSaturation != value)
                {
                    _EyeSaturation = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _Active = false;

        public Boolean Active
        {
            get { return _Active; }
            set
            {
                if (value && !OptionsManager.Default.GetOptionAvailable(OptionType.Jitter))
                {
                    WeakTip.Default.Write("Jitter", MsgTipId.PurchaseOptions, duration: 4);
                    value = false;
                }

                if (value != _Active)
                {
                    _Active = value;
                    IsNeedResetOHistoryValue = true;
                    Acquisition.Default.UpdateReadInfoList();
                    OnPropertyChanged();
                }
            }
        }
        private Boolean _EnableMeas = false;
        public Boolean EnableMeas
        {
            get { return _EnableMeas; }
            set
            {
                if (value != _EnableMeas)
                {
                    _EnableMeas = value;
                    OnPropertyChanged();
                }
            }
        }

        private readonly ConcurrentDictionary<String, JitterItems> _HistItems;

        private readonly ConcurrentDictionary<String, JitterItems> _PatItems;

        private readonly ConcurrentDictionary<String, JitterItems> _SpecItems;

        public JitterItems GetHistItem(String key) => _HistItems[key];

        public JitterItems GetPatItem(String key) => _PatItems[key];

        public JitterItems GetSpecItem(String key) => _SpecItems[key];

        public readonly Int32 HistItemCount;

        public readonly Int32 PatItemCount;

        public readonly Int32 SpecItemCount;

        private Double _CutoffFreq1;
        public Double CutoffFreq1
        {
            get => _CutoffFreq1;
            set
            {
                if (RespType == FilterResponseType.BandPass)
                {
                    value = ValidateCutoffFreq1(value, CutoffFreq2 - GapCutoffFreq);
                }
                else
                {
                    value = ValidateCutoffFreq1(value, MaxCutoffFreq);
                }

                if (_CutoffFreq1 != value)
                {
                    _CutoffFreq1 = value;

                    if (!Double.IsNaN(value))
                    {
                        _CutoffDivisor = ValidateCutoffDivisor(BitRate / CutoffFreq1);
                        OnPropertyChanged();
                        OnPropertyChanged("CutoffDivisor");
                    }

                }
            }
        }

        public Double MaxCutoffFreq => Math.Round(BitRate / MinCutDivisor, 7, MidpointRounding.AwayFromZero);
        //public Double MaxCutoffFreq => Math.Round(BitRate / Constants.MIN_FREQ_FAC, 7, MidpointRounding.AwayFromZero);
        public Double MinCutoffFreq => Math.Round(BitRate / MaxCutDivisor, 7, MidpointRounding.AwayFromZero);

        //public Double MinCutoffFreq => Math.Round(BitRate / Constants.MAX_FREQ_FAC, 7, MidpointRounding.AwayFromZero);

        public Double GapCutoffFreq => Math.Round(BitRate / Constants.MAX_FREQ_FAC * 10, 7, MidpointRounding.AwayFromZero);

        private Double ValidateCutoffFreq1(Double value, Double max)
        {
            value = Math.Round(value, 7, MidpointRounding.AwayFromZero);
            if (value > max)
            {
                value = max;
            }
            else if (value < MinCutoffFreq)
            {
                value = MinCutoffFreq;
            }

            return value;
        }

        private Double _CutoffFreq2;
        public Double CutoffFreq2
        {
            get => _CutoffFreq2;
            set
            {
                if (RespType == FilterResponseType.BandPass)
                {
                    value = ValidateCutoffFreq2(value, CutoffFreq1 + GapCutoffFreq);
                }
                else
                {
                    value = ValidateCutoffFreq1(value, MinCutoffFreq);
                }

                if (_CutoffFreq2 != value)
                {
                    _CutoffFreq2 = value;
                    OnPropertyChanged();
                }
            }
        }

        private Double ValidateCutoffFreq2(Double value, Double min)
        {
            value = Math.Round(value, 7, MidpointRounding.AwayFromZero);
            if (value > MaxCutoffFreq)
            {
                value = MaxCutoffFreq;
            }
            else if (value < min)
            {
                value = min;
            }

            return value;
        }

        public JitterDualDiracModel DualDirac { get; set; } = JitterDualDiracModel.SpectralRjDirect;


        private Boolean _EnableFilter = false;
        public Boolean EnableFilter
        {
            get => _EnableFilter;
            set
            {
                if (_EnableFilter != value)
                {
                    _EnableFilter = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _EnableChannelSim = false;
        public Boolean EnableChannelSim
        {
            get => _EnableChannelSim;
            set
            {
                if (_EnableChannelSim != value)
                {
                    _EnableChannelSim = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _EnableRxFFE = false;
        public Boolean EnableRxFFE
        {
            get => _EnableRxFFE;
            set
            {
                if (_EnableRxFFE != value)
                {
                    _EnableRxFFE = value;
                    OnPropertyChanged();
                }
            }
        }

        private String _S2PPath = "";
        public String S2PPath
        {
            get => _S2PPath;
            set
            {
                if (_S2PPath != value)
                {
                    _S2PPath = value;
                    OnPropertyChanged();
                }
            }
        }

        private String _TapPath = "";
        public String TapPath
        {
            get => _TapPath;
            set
            {
                if (_TapPath != value)
                {
                    _TapPath = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _Log10BER = Constants.DEF_LOG10_BER;
        public Int32 Log10BER
        {
            get => _Log10BER;
            set
            {
                value = ValidateLog10BER(value);
                if (_Log10BER != value)
                {
                    _Log10BER = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Int32 MaxLog10BER = Constants.MAX_LOG10_BER;

        public readonly Int32 MinLog10BER = Constants.MIN_LOG10_BER;

        private Int32 ValidateLog10BER(Int32 value)
        {
            if (value > MaxLog10BER)
            {
                value = MaxLog10BER;
            }
            else if (value < MinLog10BER)
            {
                value = MinLog10BER;
            }

            return value;
        }

        private FilterResponseType _RespType = FilterResponseType.LowPass;
        public FilterResponseType RespType
        {
            get => _RespType;
            set
            {
                if (_RespType != value)
                {
                    _RespType = value;
                    System.Threading.Volatile.Write(ref _CutoffFreq2, _CutoffFreq2);
                    OnPropertyChanged();
                }
            }
        }

        private StatisticalConstructionMode _ConstructionMode = StatisticalConstructionMode.Accumulation;
        public StatisticalConstructionMode ConstructionMode
        {
            get => _ConstructionMode;
            set
            {
                if (_ConstructionMode != value)
                {
                    _ConstructionMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public Dictionary<String, String> EyeParamTable => _Eye.EyeParamTable;

        public Dictionary<String, String> JitterParamTable = new Dictionary<String, String>() {
            {"TJ",MeasureHelper.MeasureEmpty },
            {"TJ@BER-12",MeasureHelper.MeasureEmpty },
            {"DJ",MeasureHelper.MeasureEmpty },
            {"PJ",MeasureHelper.MeasureEmpty },
            {"RJ",MeasureHelper.MeasureEmpty },
            {"DDJ",MeasureHelper.MeasureEmpty },
            {"DCD",MeasureHelper.MeasureEmpty },
            {"ISI",MeasureHelper.MeasureEmpty },
            {"CC",MeasureHelper.MeasureEmpty },
        };

        public (Double StepValue, Double Offset)? GetEyeVerticalParams(Double step, Double stepLength) => _Eye.GetEyeVerticalParams(step, stepLength);


        private Double[] ReadDataFromFile(String fileName)
        {
            if (!File.Exists(fileName))
            {
                return new Double[0];
            }
            StreamReader sr = new StreamReader(fileName);
            List<Double> data = new List<Double>();

            while (!sr.EndOfStream)
            {
                data.Add(Double.Parse(sr.ReadLine()!));
            }
            sr.Close();
            return data.ToArray();
        }

        private Double[] _JitterData = new Double[0];

        public void ReadData()
        {
            if (_JitterData.Length == 0)
            {
                //_JitterData = new Double[1_000_000_000];
                //_JitterData = ReadDataFromFile("D:/U/OscilloscopeDocument/JitterData/1gdata.txt");
                _JitterData = ReadDataFromFile("D:/U/OscilloscopeDocument/JitterData/4gdata.txt");
                //_JitterData = ReadDataFromFile("D:/U/OscilloscopeDocument/JitterData/interp4_1gdata.txt");
                //_JitterData = ReadDataFromFile("D:/U/OscilloscopeDocument/JitterData/interp16_4gdata.txt");
                //_JitterData = ReadDataFromFile("D:/U/OscilloscopeDocument/JitterData/interp10_1gdata.txt");
                _JitterData = _JitterData.Select(o => (o + 0.5) * 256).ToArray().Take(100000).ToArray();
                _TempData = _JitterData;
            }
        }
        private Double[] _TempData = new Double[0];
        private (Double SampleInterval, Double VerPosIndex, Double VerBias, Double VerScale, Double HorScale) _TempParams;
        public void SetData(UInt16[] Datas, (Double SampleInterval, Double VerPosIndex, Double VerBias, Double VerScale, Double HorScale) @params)
        {
            if (Datas != null)
            {
                ////固定眼图显示范围
                //var buffer = Datas.Select(o => (Double)o).ToArray();
                //var bit = (Int32)Math.Log2(Algorithm.MinSquareGratterThan(Constants.MAX_ADC_RES));
                //buffer = buffer.Select(o => o + 4096).ToArray();
                //Algorithm.ResolutionAdaptation(buffer, out var adapteddata, bit, 8);
                //Data.Data = adapteddata;
                //Data.Fs = 1 / sampleInterval;
                //不固定沿途的垂直位置以及显示范围
                var buffer = Datas.Select(o => (Double)o).ToArray();
                lock (_TempData.SyncRoot)
                {
                    _TempData = buffer;
                    _TempParams = @params;
                }
            }
        }

        public void GetData()
        {
            var pack = DsoModel.Default.GetWfmPack(Source);
            if (pack != null)
            {
                //var buffer = pack.Buffer.ToJagged()[0];
                //var bit = (Int32)Math.Log2(Algorithm.MinSquareGratterThan(Constants.MAX_ADC_RES));
                //buffer = buffer.Select(o => o + 4096).ToArray();
                ////ResolutionAdaptation(buffer,out var adaptedData, bit, 8);
                //Algorithm.ResolutionAdaptation(buffer, out var adaptedData, 13, 8);
                //Data.Data = adaptedData;
                var buffer = pack.Buffer.ToJagged()[0];
                //var bit = (Int32)Math.Log2(Algorithm.MinSquareGratterThan(Constants.MAX_ADC_RES));
                //buffer = buffer.Select(o => o + 4096).ToArray();
                ////ResolutionAdaptation(buffer,out var adaptedData, bit, 8);
                //Algorithm.ResolutionAdaptation(buffer, out var adaptedData, 13, 8);
                _TempData = buffer;
                _Parameter.Fs = 1 / pack.Properties.SampInterval;
                //GC.Collect();
            }
        }

        private void PrepareHistParm(Double[] data, JitterParameter @params, JitterPrepare prepare)
        {
            if (_BinWidth == 0 || _BinWidth >= 1)
            {
                _BinWidth = 0.01;
            }

            var max = data[0];

            var min = data[0];

            for (Int32 i = 0, l = data.Length; i < l; i++)
            {
                if (data[i] > max)
                {
                    max = data[i];
                }
                if (data[i] < min)
                {
                    min = data[i];
                }
            }
            var bin = (Int32)(max - min);

            var datahist = new Measure.Histogram(bin, data);
            @params.Hysteresis = _Hysteresis;
            @params.BinWidth = 1.0f / bin;
            @params.Threshold = _Threshold;
            @params.TopThreshold = _TopThreshold;
            @params.BaseThreshold = _BaseThreshold;
            @params.InterpolationMultiple = _InterpolationMultiple;
            @params.Fs = _InterpolationMultiple * @params.Fs;
            @params.ThresholdFreq = _ThresholdFreq;
            @params.ClockType = _ClockType;
            @params.CutoffDivisor = _CutoffDivisor;
            @params.NaturalFreq = _NaturalFreq;
            @params.DamplingFactor = _DamplingFactor;
            @params.CutoffFreq = _CutoffFreq1;
            @params.Hysteresis = _Hysteresis;
            @params.PatternLength = _PatternLength;
            @params.UIBinNum = _CurrentBinNum;

            if (Math.Abs(datahist.Top - max) / Math.Abs(max) > 0.05)
            {
                prepare.HighLevel = max;
            }
            else
            {
                prepare.HighLevel = datahist.Top;
            }
            if (Math.Abs(datahist.Base - min) / Math.Abs(min) > 0.05)
            {
                prepare.LowLevel = min;
            }
            else
            {
                prepare.LowLevel = datahist.Base;
            }
        }


        private (Double HighLevel, Double LowLevel) FindLevelBySincInterp(Double[] data)
        {
            //1.找前面10个周期的数据，如果周期数少于10则全部插值
            var hist = new Measure.Histogram((Int32)(data.Max() - data.Min()), data);
            var calc = new Calculator(data);
            var @top = calc.Take(MeasParameter.Top).First();
            var @base = calc.Take(MeasParameter.Base).First();
            var periods = calc.Take(MeasParameter.PeriodSequence);

            if (periods.Count - 1 <= 10)
            {
                return (@top, @base);
            }
            else
            {
                var length = ((Int32)periods[10] + 1) - (Int32)periods[0];
                if (length < 2000)//如果10个周期大于2000个点则不进行插值
                {
                    var temp = new Double[length];
                    Array.Copy(data, (Int32)periods[0], temp, 0, temp.Length);
                    //做正弦插值
                    temp = Algorithm.SinxItpl(temp, 0, temp.Length, 5);
                    temp = temp.Where(x => Double.IsFinite(x)).ToArray();
                    //重新计算顶值和底值
                    calc = new Calculator(temp);
                    @top = calc.Take(MeasParameter.Top).First();
                    @base = calc.Take(MeasParameter.Base).First();
                }

                return (@top, @base);
            }
        }

        public Boolean NeedRecordData = false;

        private String _RecordDataPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "JitterDataRecord");

        private String _RecordDataFirstName = DateTime.Now.ToString("yyyyMMddHHmm");

        private Int32 _RecordDataIndex = 1;


        private Boolean _IsDisposed = true;

        public EyeSignalType EyeSignalType = EyeSignalType.NRZ;

        public void Run()
        {
            lock (_Locker)
            {
                try
                {
                    if (IsNeedResetOHistoryValue)
                    {
                        ClearAllUiBuffer();

                        IsNeedResetOHistoryValue = false;
                    }
                    if (!(Active || EyeEnable))
                    {
                        this.Dispose();
                        return;
                    }

                    if (!(DsoModel.Default.TryGetChannel(Source, out var chnl) && chnl.Active == true))
                    {
                        ClearAllUiBuffer();
                        if (chnl != null)
                        {
                            chnl.Active = true;
                            DsoPrsnt.FocusId = chnl.Id;
                        }
                        return;
                    }

                    //GetData();

                    //ReadData();
                    Double[] data;

                    lock (_TempData.SyncRoot)
                    {
                        if (_TempData.Length > 0)
                        {
                            data = _TempData;
                            _Parameter.DataParams = _TempParams;
                            _Parameter.Fs = 1 / _TempParams.SampleInterval;
                            _TempData = new Double[0];
                        }
                        else
                        {
                            return;
                        }
                    }


                    if (data == null)
                    {
                        return;
                    }
                    _Parameter.Source = Source;

                    if (data.Length < 100)
                    {
                        JitterCommon.LimitPrintJitterError(MsgTipId.DataLengthIsInsufficient);
                        //WeakTip.Default.Write("Print", MsgTipId.DataLengthIsInsufficient);
                        if (data.Length > 0)
                        {
                            _Parameter.TIEData = new Double[0];
                            _Parameter.TIEDataAccumulate = new Double[0];
                            _Parameter.TIEDataAfterInterp = new Double[0];
                            _Parameter.TIEDataWithoutDDJAfterInterp = new Double[0];
                        }
                        return;
                    }
                    else if (data.Length > 2 && (data.Max() - data.Min()) < 10)
                    {
                        ClearAllUiBuffer();
                        JitterCommon.LimitPrintJitterError(MsgTipId.CurrentChannelAmpTooLow);
                        return;
                    }

                    if (TriggerPrsnt.State == SysState.Stop)
                        return;

                    #region 连续保存记录测试数据排查问题使用
                    RecoderData(data);
                    #endregion


                    _IsDisposed = false;
                    JitterPrepare jitter_prepare = new();
                    #region SignalIntegrity
                    if (EnableChannelSim)
                    {
                        String path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Matlab\\0_10G_1025pts.s2p");
                        //if (SignalIntegrity.ChannelSim(Data.Data, S2PPath, out Double[] outputChnlSim))
                        if (SignalIntegrity.ChannelSim(data, path, out Double[] outputChnlSim))
                        {
                            data = outputChnlSim;
                        }
                    }

                    if (EnableRxFFE)
                    {
                        //var clockEdges = ClockRecovery.ExtractEdgesByCubicSpline(Data.Data, Data.HighLevel, Data.LowLevel, Data.TopThreshold / 100, Data.BaseThreshold / 100, Data.Threshold / 100);
                        var clockedges = ClockRecovery.NRZExtractEdgesByCubicSpline(data, jitter_prepare.HighLevel, jitter_prepare.LowLevel, _Parameter.Threshold + _Parameter.Hysteresis / 100, _Parameter.Threshold - _Parameter.Hysteresis / 100, _Parameter.Threshold / 100);
                        ClockRecovery.GetNRZSignalType(clockedges, out Double averageUILength);

                        String path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Matlab\\tap_0_10G_1025pts.txt");


                        //if (SignalIntegrity.RxFFE(Data.Data, TapPath, (Int32)Math.Round(averageUILength), out Double[] outputRxFFE))
                        if (SignalIntegrity.RxFFE(data, path, (Int32)Math.Round(averageUILength), out Double[] outputRxFFE))
                        {
                            data = outputRxFFE;
                        }

                    }

                    #endregion

                    if (EyeSignalType == EyeSignalType.NRZ)
                    {
                        NrzEyePrepare nrz_eye_prepare = new();

                        var rst = NRZPrepare(data, _Parameter, jitter_prepare, nrz_eye_prepare);

                        if (rst)
                        {
                            AnalysisPreCheck(jitter_prepare);

                            _ = JitterAnalysisTask(_Parameter, jitter_prepare);

                            _ = _Eye.EyeAnalysisTask(nrz_eye_prepare);
                        }
                        else
                        {
                            ClearAllUiBuffer();
                        }
                    }
                    else if (EyeSignalType == EyeSignalType.PAM4)
                    {
                        Pam4EyePrepare pam4_eye_prepare = new()
                        {
                            SampleData = data,
                            DataParams = _Parameter.DataParams,
                            Fs = _Parameter.Fs,
                            UILength = 400,
                        };

                        _ = _Eye.EyeAnalysisTask(pam4_eye_prepare);

                    }
                }
                catch (Exception e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Jitter Warning! (id = {Thread.CurrentThread.ManagedThreadId})!\n{e.Message}\n{e.StackTrace}", EventBus.LogLevel.Debug));
                }
                finally
                {
                }
            }
        }

        private void RecoderData(Double[] data)
        {
            if (NeedRecordData)
            {
                if (!Directory.Exists(_RecordDataPath))
                {
                    Directory.CreateDirectory(_RecordDataPath);  // 创建文件夹
                }

                SaveJitterDataByCSV(data, Source, Path.Combine(_RecordDataPath, $"{_RecordDataFirstName}_Data_{_RecordDataIndex}.csv"));
                _RecordDataIndex++;
                //using (StreamWriter sw = new StreamWriter(Path.Combine(_RecordDataPath, $"{_RecordDataFirstName}_Data_{_RecordDataIndex}.txt")))
                //{
                //    foreach (Double data in Data.Data)
                //    {
                //        sw.WriteLine(data.ToString());
                //    }

                //_RecordDataIndex++;
                //}
            }


            void SaveJitterDataByCSV(Double[] data, ChannelId id, String path)
            {
                if (data == null || data.Length == 0 || String.IsNullOrWhiteSpace(path) || !DsoModel.Default.TryGetChannel(id, out var chnl))
                    return;
                if (!(chnl is ChannelModel ctx) || ctx == null || ctx.Pack == null)
                    return;
                var pos0ByAdc = (chnl.Conditioning.PosIndex / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2);
                var temp = new Double[1, data.Length];//存储量化值后的数据进行csv文件进行保存
                                                      //pkg.Buffer[i, j] = (pkg.Buffer[i, j] - ctx.Pos0ByAdc) / Constants.SAMPS_PER_YDIV * ctx.Properties.ChnlScale.Value + ctx.Properties.ChnlBias;
                for (Int64 i = 0, l = data.Length; i < l; i++)
                {
                    temp[0, i] = (data[i] - pos0ByAdc) / Constants.SAMPS_PER_YDIV * ctx.Pack.Properties.ChnlScale.Value + ctx.Pack.Properties.ChnlBias;
                }

                WfmPack wfm = new(temp, 0, temp.GetLength(1), (WfmProperties)ctx.Pack.Properties);

                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    FilePrsnt.SaveWaveByCSV(fs, wfm, (x, y) => x.ToString("E") + "," + String.Join(",", y.Select(o => o.ToString("G"))));
                }
            }

        }

        private Boolean NRZPrepare(Double[] data, JitterParameter @params, JitterPrepare prepare, NrzEyePrepare nrzEyePrepare)
        {
            PrepareHistParm(data, @params, prepare);

            if (!ClockRecovery.GetNRZClock(data, @params, prepare))
            {
                return false;
            }

            var ties = TIE.CalculateClockTIE(prepare.ClockEdges, prepare.RecoveredEdges); ;
            if (ties.Length < 2)
            {
                JitterCommon.LimitPrintJitterError(MsgTipId.SDADataCyclesTooFew);
                return false;
            }

            prepare.Fs = @params.Fs;
            prepare.TIEData = ties;

            nrzEyePrepare.SampleData = data;
            nrzEyePrepare.DataParams = @params.DataParams;
            nrzEyePrepare.Fs = prepare.Fs;
            nrzEyePrepare.HighLevel = prepare.HighLevel;
            nrzEyePrepare.LowLevel = prepare.LowLevel;
            nrzEyePrepare.Threshold = this.Threshold;
            nrzEyePrepare.UICount = prepare.UICount;
            nrzEyePrepare.AverageUILength = prepare.AverageUILength;
            nrzEyePrepare.ClockEdges = prepare.ClockEdges;
            nrzEyePrepare.RecoveredEdges = prepare.RecoveredEdges;
            nrzEyePrepare.TIEData = prepare.TIEData;

            return true;
        }

        private void AnalysisPreCheck(JitterPrepare prepare)
        {
            Byte clear_flage = 0;//0不清除,1清除抖动结果，2清除眼图结果，3全清除
            if (_OldAverageUILength != prepare.AverageUILength)
            {
                if (_OldAverageUILength != Double.NaN && Math.Abs(_OldAverageUILength - prepare.AverageUILength) / _OldAverageUILength > 0.08)//0.08是经验值，最好不要大于0.1
                {
                    if (!_JitterAnalysisCompleted)
                    {
                        _CancelUpdateJitterData = true;
                    }
                    if (!_Eye.EyeAnalysisCompleted)
                    {
                        _Eye.CancelUpdateEyeData = true;
                    }
                    clear_flage = 3;
                }
                _OldAverageUILength = prepare.AverageUILength;
            }

            if (prepare.Fs != _OldFs)
            {
                _OldFs = prepare.Fs;
                clear_flage = 3;
            }

            switch (clear_flage)
            {
                case 1:
                    ClearJitterUiBuffer();
                    break;
                case 2:
                    _Eye.ClearEyeUiBuffer();
                    break;
                case 3:
                    ClearAllUiBuffer();
                    break;
            }
        }


        private Boolean _JitterAnalysisCompleted = true;

        private Boolean _CancelUpdateJitterData = false;

        private async Task JitterAnalysisTask(JitterParameter @params, JitterPrepare prepare)
        {
            try
            {
                await Task.Run(() => JitterAnalysis(@params, prepare));
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }

        private void JitterAnalysis(JitterParameter @params, JitterPrepare prepare)
        {
            if (!_JitterAnalysisCompleted)
            {
                return;
            }

            try
            {
                _JitterAnalysisCompleted = false;
                var jitterresult = new JitterResult();
                try
                {
                    TIE.GetCyleToCycle(prepare, jitterresult);
                    var tiesuccess = TIE.GetTIE(@params, prepare, jitterresult);
                    if (!tiesuccess)
                    {
                        ClearJitterUiBuffer();
                        return;
                    }
                    var trendvector = TIE.GetTrend(@params, prepare);
                    JitterBuff.Default.Provide(Constants.DATA_JITTER_TREND, trendvector);
                }
                catch (Exception e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Jitter Warning! (id = {Thread.CurrentThread.ManagedThreadId})!\n{e.Message}\n{e.StackTrace}", EventBus.LogLevel.Debug));
                }

                try
                {
                    var histvector = TIE.GetTIEHist(@params);
                    JitterBuff.Default.Provide(Constants.DATA_JITTER_HISTOGRAM, histvector);
                }
                catch (Exception e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Jitter Warning! (id = {Thread.CurrentThread.ManagedThreadId})!\n{e.Message}\n{e.StackTrace}", EventBus.LogLevel.Debug));
                }

                try
                {
                    @params.ThresholdFreq = _ThresholdFreq;
                    Spectrum.SpectrumSeparation(@params, jitterresult);
                    var spectrumvector = Spectrum.GetSpectrum(@params, prepare, jitterresult);
                    JitterBuff.Default.Provide(Constants.DATA_JITTER_SPECTRUM, spectrumvector);
                }
                catch (Exception e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Jitter Warning! (id = {Thread.CurrentThread.ManagedThreadId})!\n{e.Message}\n{e.StackTrace}", EventBus.LogLevel.Debug));
                }

                try
                {
                    if (Statistical.MJSQ.Run(@params, prepare, jitterresult))
                    {
                        var bathvector = Statistical.MJSQ.GetButhWave(@params);
                        JitterBuff.Default.Provide(Constants.DATA_JITTER_BATHTUB, bathvector);
                        var qvector = Statistical.MJSQ.GetQWave(@params, prepare);
                        JitterBuff.Default.Provide(Constants.DATA_JITTER_QFACTOR, bathvector);
                    }
                }
                catch (Exception e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Jitter Warning! (id = {Thread.CurrentThread.ManagedThreadId})!\n{e.Message}\n{e.StackTrace}", EventBus.LogLevel.Debug));
                }

                if (!_CancelUpdateJitterData)
                {
                    JitterMeasure(jitterresult, prepare);
                }
                else
                {
                    TIE.Dispose();
                }
            }
            catch (Exception e)
            {

                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Jitter Analysis Warning! (id = {Thread.CurrentThread.ManagedThreadId})!\n{e.Message}\n{e.StackTrace}", EventBus.LogLevel.Debug));
            }
            finally
            {
                _JitterAnalysisCompleted = true;
                _CancelUpdateJitterData = false;
            }

        }







        public Measure.Histogram Hist
        {
            get { return Statistical.Hist; }
        }

        private JitterParameter _Parameter { get; set; }
        private JitterResult _Jitter { get; set; }

        private Double _OldAverageUILength { get; set; } = Double.NaN;
        private Double _OldFs { get; set; } = Double.NaN;
        private Double _OldSource { get; set; } = Double.NaN;


        public IJitterAnalyzer _CurrentAnalyzer { get; set; }

        private JitterDecompositionType _DecompositionType;
        public JitterDecompositionType DecompositionType
        {
            get { return _DecompositionType; }
            set
            {
                if (value != _DecompositionType)
                {
                    _DecompositionType = value;
                    switch (value)
                    {
                        case JitterDecompositionType.Spectrum:
                            _CurrentAnalyzer = Spectrum;
                            break;
                        case JitterDecompositionType.MJSQ:
                            _CurrentAnalyzer = Statistical.MJSQ;
                            break;
                        case JitterDecompositionType.NQScale:
                            _CurrentAnalyzer = Statistical.NQScale;
                            break;
                        default:

                            break;
                    }
                }
            }
        }

        public SpectrumAnalyzer Spectrum { get; set; }

        public StatisticalAnalyzer Statistical { get; set; }


        private EyeMethod _Eye { get; set; }



        private void JitterMeasure(JitterResult result, JitterPrepare prepare)
        {
            GetJitterParameter(result);
            _HistItems["TjBER"].Current = result.TJ_BER12;
            _HistItems["Tj"].Current = result.TJ;
            _HistItems["Rj"].Current = result.RJ;
            _HistItems["Dj"].Current = result.DJ;

            _PatItems["TIEPeak"].Current = result.TIE;
            _PatItems["PjPeak"].Current = result.PJ;
            _PatItems["CCjPeak"].Current = result.CC;//cycle-cycle抖动

            _SpecItems["DCD"].Current = result.DCD;
            _SpecItems["DDj"].Current = result.DDJ;
            _SpecItems["ISI"].Current = result.ISI;//码间干扰

            if (_EnableBitRateSearch)
            {
                BitRate = _Parameter.Fs / prepare.AverageUILength;
            }
        }

        private void ClearJitterUiBuffer()
        {
            //清除浴盆曲线数据
            JitterBuff.Default.Provide(Constants.DATA_JITTER_BATHTUB, new Vector(new Double[0],
               QuantityUnitExt.ToUnitString(QuantityUnit.Second),
               QuantityUnitExt.ToUnitString(QuantityUnit.BER),
               1, Constants.DEF_XPOS_IDX));
            //清除趋势图数据
            JitterBuff.Default.Provide(Constants.DATA_JITTER_TREND, new Vector(new Double[0],
               QuantityUnitExt.ToUnitString(QuantityUnit.Second),
               QuantityUnitExt.ToUnitString(QuantityUnit.Second),
               1, Constants.DEF_XPOS_IDX));
            //清除直方图数据
            JitterBuff.Default.Provide(Constants.DATA_JITTER_HISTOGRAM, new Vector(new Double[0],
                QuantityUnitExt.ToUnitString(QuantityUnit.Second),
                QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
                1, Constants.DEF_XPOS_IDX));
            //清除频谱图数据
            JitterBuff.Default.Provide(Constants.DATA_JITTER_SPECTRUM, new Vector(new Double[0],
                QuantityUnitExt.ToUnitString(QuantityUnit.Hertz),
                QuantityUnitExt.ToUnitString(QuantityUnit.Second),
                1, Constants.DEF_XPOS_IDX));
            //清除频谱图数据
            JitterBuff.Default.Provide(Constants.DATA_JITTER_QFACTOR, new Vector(new Double[0],
                QuantityUnitExt.ToUnitString(QuantityUnit.BER),
                QuantityUnitExt.ToUnitString(QuantityUnit.Second),
                1, Constants.DEF_XPOS_IDX));
            ClearJitterMeasure();
        }

        private void ClearJitterMeasure()
        {
            TIE.Dispose();
            _HistItems["TjBER"].Current = Double.NaN;
            _HistItems["Tj"].Current = Double.NaN;
            _HistItems["Rj"].Current = Double.NaN;
            _HistItems["Dj"].Current = Double.NaN;
            _PatItems["TIEPeak"].Current = Double.NaN;
            _PatItems["PjPeak"].Current = Double.NaN;
            _PatItems["CCjPeak"].Current = Double.NaN;
            _SpecItems["DCD"].Current = Double.NaN;
            _SpecItems["DDj"].Current = Double.NaN;
            _SpecItems["ISI"].Current = Double.NaN;
        }

        public Boolean GetJitterParameter(JitterResult result)
        {
            var analyzer = _CurrentAnalyzer.GetResult();
            result.TIE = result.TIE / _Parameter.Fs;
            result.DJ = analyzer.DJ / _Parameter.Fs;
            result.PJ = Spectrum.PJ / _Parameter.Fs;
            result.RJ = analyzer.RJ / _Parameter.Fs;
            result.TJ = analyzer.TJ / _Parameter.Fs;
            result.TJ_BER12 = analyzer.TJ_BER12 / _Parameter.Fs;
            result.ISI = result.ISI / _Parameter.Fs;
            result.DDJ = result.DDJ / _Parameter.Fs;
            result.DCD = result.DCD / _Parameter.Fs;
            result.CC = result.CC / _Parameter.Fs;

            return true;
        }

        private void ClearAllUiBuffer()
        {
            ClearJitterUiBuffer();
            if (EyeEnable)
            {
                _Eye.ClearEyeUiBuffer();
            }
        }

        public void Dispose()
        {
            if (!_IsDisposed)
            {
                _IsDisposed = true;
                _Parameter.Dispose();
                _TempData = new Double[0];
                _Eye?.Dispose();
                this._CurrentAnalyzer?.Dispose();
                Hist?.Dispose();
                Spectrum?.Dispose();
                Statistical?.Dispose();
                ClearAllUiBuffer();
                GC.Collect();
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

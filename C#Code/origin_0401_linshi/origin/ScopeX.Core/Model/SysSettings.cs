using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ScopeX.ComModel;
using ScopeX.Core.Decode;
using ScopeX.Core.Hardware;
using ScopeX.Core.PowerAnalysis;
using ScopeX.MathExt;
using static ScopeX.Core.AnalogModel;

namespace ScopeX.Core.Tools
{
    public class SysSettings
    {
        private const String _VER = "U2Core221118";
        [BinaryConvert.Seializable(false)]
        private readonly String _Version = _VER;

        private record AnalogChnlSettings(Boolean Active, Int32 ScaleIndex, Double PosIndex, Prefix Prefix, String Unit)
        {
            public Int32 Bandwidth
            {
                get;
                init;
            }

            public Boolean ProbeUnitIsCustomized
            {
                get;
                init;
            }
            public Boolean IsInverted
            {
                get;
                init;
            }

            public Double Scale
            {
                get;
                init;
            }

            public Double Position
            {
                get;
                init;
            }

            public AnaChnlCoupling Coupling
            {
                get;
                init;
            }

            public AnaChnlProbe ProbeIndex
            {
                get;
                init;
            }

            public AnaChnlIpnutSource InputSource
            {
                get;
                init;
            }

            public Int64 InterChannelOffset
            {
                get;
                init;
            }
        }

        private record MathChnlSettings(Boolean Active, String Formula)
        {
            public (Int32 Index, Double Value) ChnlScale
            {
                get;
                init;
            }

            public (Double Index, Double Value) ChnlPosition
            {
                get;
                init;
            }

            public (Prefix Prefix, String Unit) ChnlUnit
            {
                get;
                init;
            }

            public (Int32 Index, Double Value) SamplingScale
            {
                get;
                init;
            }

            public (Double Index, Double Value) SamplingPosition
            {
                get;
                init;
            }

            public Boolean IsAutoUnit
            {
                get;
                init;
            }

            public (Int32 Index, Double Value) ChnlInitScale
            {
                get;
                init;
            }

            public (Int32 Index, Double Value) SamplingInitScale
            {
                get;
                init;
            }
        }

        private record DigiChnlBitSettings(Boolean Active, Int32 ScaleIndex, Double PosIndex);

        private record DigiChnlCtrlGrpSettings(DigiTholdFamily Family, Int32 UserThroldIndex, Int32 UserHystIndex);

        private record DigiChnlSettings(Int32 FocusBitId, DigiChnlBitSettings[] Bits, DigiChnlCtrlGrpSettings[] CtrlGroups);

        private record AwgSettings(Boolean Active, WfmGenMode Mode, ArbWfmType WfmType, Int64 Frequency, Int32 Amplitude, Int32 Offset, Int32 Duty, Int32 Phase, Int32 Noise)
        {
            public WfmGenImpedance Impedance
            {
                get;
                init;
            }

            public WfmModMethod ModMethod
            {
                get;
                init;
            }

            public ArbWfmType ModulatedWfm
            {
                get;
                init;
            }

            public Int64 ModFreq
            {
                get;
                init;
            }

            public Int32 AmpDepth
            {
                get;
                init;
            }

            public Int64 FreqBias
            {
                get;
                init;
            }

            /// <summary>
            /// 扫频开始频率
            /// </summary>
            /// <Remark>更改人：彭博 创建日期：2024/1/16 9:57:00  原因：保存扫频的起始频率 </Remark>
            public Int64 SweepStartFreq
            {
                get;
                init;
            }

            /// <summary>
            /// 扫频结束频率
            /// </summary>
            /// <Remark>更改人：彭博 创建日期：2024/1/16 9:57:00  原因：保存扫频的起始频率 </Remark>
            public Int64 SweepEndFreq
            {
                get;
                init;
            }

            /// <summary>
            /// 扫频时间
            /// </summary>
            /// <Remark>更改人：彭博 创建日期：2024/1/16 9:57:00  原因：保存扫频的起始频率 </Remark>
            public Int64 SweepDuration
            {
                get;
                init;
            }

            /// <summary>
            /// 连续波模式波类型
            /// </summary>
            public ArbWfmType ContinuousArbWfmType
            {
                get;
                init;
            }

            /// <summary>
            /// 调制波模式波类型
            /// </summary>
            public ArbWfmType ModulationWfmType
            {
                get;
                init;
            }

            /// <summary>
            /// 扫频模式波类型
            /// </summary>
            public ArbWfmType SweepWfmType
            {
                get;
                init;
            }

            public Boolean? IsShow
            {
                get;
                init;
            }
        }

        private record MeasItemSettings(Boolean Active, String Name, ChannelId Source, ChannelId Source2nd, Boolean StatActive, MeasureType MeasureType, MeasureOperator Operation)
        {
            public MeasureTopBaseRefUnit RefUnit
            {
                get;
                init;
            }

            public MeasureTopBaseRef RefStd
            {
                get;
                init;
            }

            public Int32 MidThrold
            {
                get;
                init;
            }

            public Int32 HighThrold
            {
                get;
                init;
            }

            public Int32 LowThrold
            {
                get;
                init;
            }
        };

        private record RadioFrequencyChnlSettings(Boolean Active)
        {
            public Boolean NormalLine
            {
                get;
                init;
            }

            public Boolean MaxHoldLine
            {
                get;
                init;
            }
            public Boolean MinHoldLine
            {
                get;
                init;
            }
            public Boolean AverageLine
            {
                get;
                init;
            }
            public Int32 AverageTimes
            {
                get;
                init;
            }

            public RFWindowType Window
            {
                get;
                init;
            }

            public RFDataType FrequencyDataType
            {
                get;
                init;
            }

            public Int32 WaveDataLength
            {
                get;
                init;
            }

            public Double TranslateADCSamplerate
            {
                get;
                init;
            }

            public Int32 FFTLength
            {
                get;
                init;
            }

            public Int64 StartFrequency
            {
                get;
                init;
            }

            public Int64 CenterFrequency
            {
                get;
                init;
            }

            public Int64 EndFrequency
            {
                get;
                init;
            }

            public Int64 Span
            {
                get;
                init;
            }

            public Int64 RBW
            {
                get;
                init;
            }

            public Int64 FreqScale
            {
                get;
                init;
            }

            public Int64 FigureCenterFrequency
            {
                get;
                init;
            }

            public Double RefLevelValue
            {
                get;
                init;
            }

            public AmplitudeUnitType UnitType
            {
                get;
                init;
            }

            public LogarithmUnit PUnit
            {
                get;
                init;
            }

            public Double AmpScale
            {
                get;
                init;
            }
            public Double FigureCenterAmplitude
            {
                get;
                init;
            }
            public String Unit
            {
                get;
                init;
            }
            public Prefix Prefix
            {
                get;
                init;
            }
            public Double PosIndex
            {
                get;
                init;
            }
        }

        #region 电源分析
        private record PwrQualitySettings(VIType refFreq);
        private record PwrHarmonicSettings(Int32 harmonicNum, VIType source, HarmonicRefFreqSrc refFreqSrc, HarmonicDisplayOpt harmonicOpt);
        private record PwrRippleSettings(VIType source);
        private record PwrSwitchingLossSettings(Double rdsOn);
        private record PwrSOASettings(Boolean stopOnFail, Double maxLinX, Double minLinX, Double maxLinY, Double minLinY);
        private record PwrLoopAnalysisSettings(AWGId awgSource, ImpedanceType impedance, Int64 startFreq, Int64 endFreq, Int32 scanNum, AmplitudeMode amplitudeMode, Boolean checkTriggerStatus);


        private record PowerAnalysisSettings(PowerAnalysisOpt mode, ChannelId voltageSrc, ChannelId currentSrc)
        {
            public PwrQualitySettings PwrQuality { get; set; }
            public PwrHarmonicSettings PwrHarmonic { get; set; }
            public PwrRippleSettings PwrRipple { get; set; }
            public PwrSwitchingLossSettings PwrSwitchingLoss { get; set; }
            public PwrSOASettings PwrSOA { get; set; }
            public PwrLoopAnalysisSettings PwrLoopAnalysis { get; set; }
        }

        [BinaryConvert.Seializable(false)]
        private PowerAnalysisSettings[]? _PowerAnalysis;
        #endregion 电源分析
        /// <summary>
        /// BUS解码界面配置
        /// </summary>
        private record BusChnlSettings(Boolean Active)
        {
            public Double ScaleBymV { get; init; }

            public Double PosIndexBymDiv { get; init; }
            public Int32 ScaleIndex { get; init; }

            public Int32 ScaleTick { get; init; }
            public SerialProtocolType ProtocolType { get; init; }

            public DecodeDisplayMode Format { get; init; }

            public ChannelId Id { get; init; }
        }

        [BinaryConvert.Seializable(false)]
        private ProductType _Product;

        [BinaryConvert.Seializable(false)]
        private AnalogChnlSettings[]? _Analog;

        [BinaryConvert.Seializable(false)]
        private MathChnlSettings[]? _Math;

        [BinaryConvert.Seializable(false)]
        private DigiChnlSettings? _Digital;

        [BinaryConvert.Seializable(false)]
        private BusChnlSettings[]? _Bus;

        [BinaryConvert.Seializable(false)]
        private String[]? _BusProtoclStr;

        [BinaryConvert.Seializable(false)]
        private SerialProtocolType _TriggerProtocolType = SerialProtocolType.Close;

        [BinaryConvert.Seializable(false)]
        private String _CurrentSeriealTriggerInfo;

        [BinaryConvert.Seializable(false)]
        private String _CurrentSeriealTriggerDecodeInfo;

        [BinaryConvert.Seializable(false)]
        private RadioFrequencyChnlSettings[]? _RadioFrequency;

        [BinaryConvert.Seializable(false)]
        private Double _EnhancedBits;
        [BinaryConvert.Seializable(false)]
        private Boolean _EnhancedBitsActive;
        [BinaryConvert.Seializable(false)]
        private AnaChnlAcqMode _AcqMode;
        [BinaryConvert.Seializable(false)]
        private AnaChnlStorageMode _AcqLength;
        [BinaryConvert.Seializable(false)]
        private Int32 _StorageDepthOpt;
        [BinaryConvert.Seializable(false)]
        private Int32 _AverageCnt;
        [BinaryConvert.Seializable(false)]
        private Int32 _EnvelopeCnt;
        [BinaryConvert.Seializable(false)]
        private EvlpOpt _EnvelopOpt;
        [BinaryConvert.Seializable(false)]
        private AnaChnlClkSrc _ClockSrc;
        [BinaryConvert.Seializable(false)]
        private AnaChnlItplType _InterplType;
        [BinaryConvert.Seializable(false)]
        private SegmentWorkMode _WorkMode;
        [BinaryConvert.Seializable(false)]
        private Int32 _CurFrameId;
        [BinaryConvert.Seializable(false)]
        private Int32 _ReferFrameIds;
        [BinaryConvert.Seializable(false)]
        private Int32 _SequentStartFrame;
        [BinaryConvert.Seializable(false)]
        private PlotRenderType _RenderType;

        [BinaryConvert.Seializable(false)]
        private Int32 _TmbScaleIndex;
        [BinaryConvert.Seializable(false)]
        private Double _TmbScale;
        [BinaryConvert.Seializable(false)]
        private Double _TmbPosIndex;
        [BinaryConvert.Seializable(false)]
        private Double _TmbPosition;

        [BinaryConvert.Seializable(false)]
        private TriggerType _TrgType;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgHoldoff;

        [BinaryConvert.Seializable(false)]
        private String _WfmPath;
        [BinaryConvert.Seializable(false)]
        private String _PicPath;

        #region 触发配置字段部分
        [BinaryConvert.Seializable(false)]
        private ChannelId _TrgEdgeSource;
        [BinaryConvert.Seializable(false)]
        private TriggerImpedance _TrgEdgeImp;
        [BinaryConvert.Seializable(false)]
        private TriggerCoupling _TrgEdgeCouple;
        [BinaryConvert.Seializable(false)]
        private Int32 _TrgEdgeSensitivity;
        [BinaryConvert.Seializable(false)]
        private EdgeSlope _TrgEdgeSlope;
        [BinaryConvert.Seializable(false)]
        private (Double Index, Double Value)[]? _TrgEdgePosition;

        [BinaryConvert.Seializable(false)]
        private ChannelId _TrgPulseSource;
        [BinaryConvert.Seializable(false)]
        private PulsePolarity _TrgPulsePolarity;
        [BinaryConvert.Seializable(false)]
        private PulseCondition _TrgPulseCondition;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgPulseWidth;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgPulseUpperWidth;
        [BinaryConvert.Seializable(false)]
        private (Double Index, Double Value)[]? _TrgPulsePosition;

        [BinaryConvert.Seializable(false)]
        private ChannelId _TrgVideoSource;
        [BinaryConvert.Seializable(false)]
        private VideoSync _TrgVideoSync;
        [BinaryConvert.Seializable(false)]
        private Int16 _TrgVideoLine;
        [BinaryConvert.Seializable(false)]
        private VideoPolarity _TrgVideoPolarity;
        [BinaryConvert.Seializable(false)]
        private VideoStandard _TrgVideoStandard;
        [BinaryConvert.Seializable(false)]
        private (Double Index, Double Value)[]? _TrgVideoPosition;

        [BinaryConvert.Seializable(false)]
        private ChannelId _TrgSlopeSource;
        [BinaryConvert.Seializable(false)]
        private EdgeSlope _TrgSlopeSlope;
        [BinaryConvert.Seializable(false)]
        private PulseCondition _TrgSlopeCondition;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgSlopeWidthbyps;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgSlopeUpperWidthByps;
        [BinaryConvert.Seializable(false)]
        private (Double Lower, Double Upper) _TrgSlopePosIndex;

        [BinaryConvert.Seializable(false)]
        private ChannelId _TrgRuntSource;
        [BinaryConvert.Seializable(false)]
        private PulsePolarity _TrgRuntPolarity;
        [BinaryConvert.Seializable(false)]
        private PulseCondition _TrgRuntCondition;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgRuntWidthbyps;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgRuntUpperWidthByps;
        [BinaryConvert.Seializable(false)]
        private (Double Lower, Double Upper) _TrgRuntPosIndex;

        [BinaryConvert.Seializable(false)]
        private ChannelId _TrgDelaySource1;
        [BinaryConvert.Seializable(false)]
        private ChannelId _TrgDelaySource2 = ChannelId.C2;
        [BinaryConvert.Seializable(false)]
        private EdgeSlope _TrgDelayEdge1;
        [BinaryConvert.Seializable(false)]
        private EdgeSlope _TrgDelayEdge2;
        [BinaryConvert.Seializable(false)]
        private PulseCondition _TrgDelayCondition;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgDelayWidthbyps;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgDelayUpperWidthByps;
        [BinaryConvert.Seializable(false)]
        private (Double Lower, Double Upper) _TrgDelayPosIndex;
        [BinaryConvert.Seializable(false)]
        private Double _TrgDelayDataCompPosIndex;

        [BinaryConvert.Seializable(false)]
        private ChannelId _TrgTimeoutSource;
        [BinaryConvert.Seializable(false)]
        private LevelPolarity _TrgTimeoutPolarity;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgTimeoutDurationByps;

        [BinaryConvert.Seializable(false)]
        private PulseCondition _TrgDurationCondition;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgDurationWidthByps;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgDurationUpperWidthByps;
        [BinaryConvert.Seializable(false)]
        private ChannelId[]? _TrgDurationConditionsChannelID;
        [BinaryConvert.Seializable(false)]
        private SustainTimeLevelCondition[]? _TrgDurationConditionsConditions;
        [BinaryConvert.Seializable(false)]
        private Double[]? _TrgDurationPositions;

        [BinaryConvert.Seializable(false)]
        private ChannelId _TrgSetupHoldClockSource;
        [BinaryConvert.Seializable(false)]
        private EdgeSlope _TrgSetupHoldClockPolarity;
        [BinaryConvert.Seializable(false)]
        private ChannelId _TrgSetupHoldDataSource = ChannelId.C2;
        [BinaryConvert.Seializable(false)]
        private EdgeSlope _TrgSetupHoldDataPolarity;
        [BinaryConvert.Seializable(false)]
        private Double _TrgSetupHoldClkCompPosIndex;
        [BinaryConvert.Seializable(false)]
        private Double _TrgSetupHoldUpperDataPosIndex;
        [BinaryConvert.Seializable(false)]
        private SetupHoldViolation _TrgSetupHoldViolation;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgSetupHoldTsuByps;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgSetupHoldThdByps;


        [BinaryConvert.Seializable(false)]
        private ChannelId _TrgNedgeDataSource;
        [BinaryConvert.Seializable(false)]
        private EdgeSlope _TrgNedgeDataPolarity;
        [BinaryConvert.Seializable(false)]
        private Int64 _TrgNedgeDurationByps;
        [BinaryConvert.Seializable(false)]
        private Int32 _TrgNedgeEdgeNumber;

        [BinaryConvert.Seializable(false)]
        private PatLevelCondition[]? _TrgPatternCondition;
        [BinaryConvert.Seializable(false)]
        private Double[]? _TrgPatternPositions;
        #endregion

        [BinaryConvert.Seializable(false)]
        private WfmDrawMode _WfmDrawMode;
        [BinaryConvert.Seializable(false)]
        private MultiWfmsLayout _WfmLayout;
        [BinaryConvert.Seializable(false)]
        private WfmPersist _Persist;
        [BinaryConvert.Seializable(false)]
        private Int32 _WfmIntensity;
        [BinaryConvert.Seializable(false)]
        private GridType _GridStyle;
        [BinaryConvert.Seializable(false)]
        private Int32 _GridIntensity;
        [BinaryConvert.Seializable(false)]
        private Boolean _AxisTickVisible;
        [BinaryConvert.Seializable(false)]
        private Boolean _XAxisTickBottom;
        [BinaryConvert.Seializable(false)]
        private Boolean _YAxisTickRight;

        [BinaryConvert.Seializable(false)]
        private MeasureGate _Strobe;
        [BinaryConvert.Seializable(false)]
        private Int32 _Indicator;
        [BinaryConvert.Seializable(false)]
        private ChannelId _SnapshotSrc;
        [BinaryConvert.Seializable(false)]
        private MeasItemSettings[]? _MeasItems;
        [BinaryConvert.Seializable(false)]
        private Boolean _MeasVisiable;
        [BinaryConvert.Seializable(false)]
        private Boolean _SnapshotActive;
        [BinaryConvert.Seializable(false)]
        private Boolean _IsStatActive;

        [BinaryConvert.Seializable(false)]
        private Boolean _CursorActive;
        [BinaryConvert.Seializable(false)]
        private CursorType _CursorType;
        [BinaryConvert.Seializable(false)]
        private CursorPositionMode _CursorPosMode;
        [BinaryConvert.Seializable(false)]
        private ChannelId _HCursorSource;
        [BinaryConvert.Seializable(false)]
        private CursorPosFormat _HCursorFormat;
        [BinaryConvert.Seializable(false)]
        private Double[]? _HCursorPosition;
        [BinaryConvert.Seializable(false)]
        private ChannelId _VCursorSource;
        [BinaryConvert.Seializable(false)]
        private CursorPosFormat _VCursorFormat;
        [BinaryConvert.Seializable(false)]
        private Double[]? _VCursorPosition;
        [BinaryConvert.Seializable(false)]
        private Boolean _TraceWave;
        [BinaryConvert.Seializable(false)]
        private Boolean _IsSyncMove;


        [BinaryConvert.Seializable(false)]
        private AwgSettings[]? _Awg;

        [BinaryConvert.Seializable(false)]
        private Boolean _VoltmeterActive;
        [BinaryConvert.Seializable(false)]
        private Boolean _VoltmeterStatActive;
        [BinaryConvert.Seializable(false)]
        private ChannelId _VoltmeterSource;
        [BinaryConvert.Seializable(false)]
        private VoltmeterMode _VoltmeterMode;

        [BinaryConvert.Seializable(false)]
        private Boolean _CymometerActive;
        [BinaryConvert.Seializable(false)]
        private Boolean _CymometerStatActive;
        [BinaryConvert.Seializable(false)]
        private ChannelId? _CymometerSource;
        [BinaryConvert.Seializable(false)]
        private Boolean _CymometerShowPeriod;

        [BinaryConvert.Seializable(false)]
        private ScopeX.MathExt.FilterResponseType _FilterResp;
        [BinaryConvert.Seializable(false)]
        private ScopeX.MathExt.FilterType _FilterType;
        [BinaryConvert.Seializable(false)]
        private ScopeX.MathExt.FIRType _FIRMethod;
        [BinaryConvert.Seializable(false)]
        private ScopeX.MathExt.IIRType _IIRMethod;
        [BinaryConvert.Seializable(false)]
        private ScopeX.MathExt.FilterOrderMode _FilterOrderMode;
        [BinaryConvert.Seializable(false)]
        private ScopeX.MathExt.WindowType _FilterWindow;
        [BinaryConvert.Seializable(false)]
        private Int32 _FilterOrder;
        [BinaryConvert.Seializable(false)]
        private Double _FilterSamplingFreq;
        [BinaryConvert.Seializable(false)]
        private Int32 _FilterLowPassFreq;
        [BinaryConvert.Seializable(false)]
        private Int32 _FilterLowStopFreq;
        [BinaryConvert.Seializable(false)]
        private Int32 _FilterHighPassFreq;
        [BinaryConvert.Seializable(false)]
        private Int32 _FilterHighStopFreq;
        [BinaryConvert.Seializable(false)]
        private Int32 _FilterPassMag;
        [BinaryConvert.Seializable(false)]
        private Int32 _FilterStopMag;
        [BinaryConvert.Seializable(false)]
        private UInt32 _FilterDensityFactor;
        [BinaryConvert.Seializable(false)]
        private Boolean _JitterActive;
        [BinaryConvert.Seializable(false)]
        private Boolean _EyeEnable;
        [BinaryConvert.Seializable(false)]
        private Double _BinWidth;
        [BinaryConvert.Seializable(false)]
        private Int32 _PatternLength;
        [BinaryConvert.Seializable(false)]
        private JitterSignalType _SignalType;
        [BinaryConvert.Seializable(false)]
        private Double _ThresholdFreq;
        [BinaryConvert.Seializable(false)]
        private Double _Threshold;
        [BinaryConvert.Seializable(false)]
        private ChannelId _Source;
        [BinaryConvert.Seializable(false)]
        private ClockTypeOpt _ClockType;
        [BinaryConvert.Seializable(false)]
        private PllTypeOpt _PllType;
        [BinaryConvert.Seializable(false)]
        private Boolean _JitterParamEnable;
        [BinaryConvert.Seializable(false)]
        private Boolean _EyeParamEnable;
        [BinaryConvert.Seializable(false)]
        private Double _CutoffFreq1;
        [BinaryConvert.Seializable(false)]
        private Double _CutoffDivisor;
        [BinaryConvert.Seializable(false)]
        private Double _DamplingFactor;
        [BinaryConvert.Seializable(false)]
        private Double _NaturalFreq;
        [BinaryConvert.Seializable(false)]
        private MaxBinNum _CurrentBinNum;
        [BinaryConvert.Seializable(false)]
        private Double _BitRate;
        [BinaryConvert.Seializable(false)]
        private Double _Hysteresis;
        [BinaryConvert.Seializable(false)]
        private List<(JitterGraphType, Boolean)> _JitterGraphTypeList;

        [BinaryConvert.Seializable(false)]
        private List<String> _NavBarGroupRecordKeys = new List<String>();
        [BinaryConvert.Seializable(false)]
        private List<Int32> _NavBarGroupRecordValues = new List<Int32>();

        [BinaryConvert.Seializable(false)]
        private Boolean _PassFailActive;
        [BinaryConvert.Seializable(false)]
        private ChannelId _PassFailSource;
        [BinaryConvert.Seializable(false)]
        private PFTestMode _PassFailMode;
        [BinaryConvert.Seializable(false)]
        private Int32 _PassFailVertTolerance;
        [BinaryConvert.Seializable(false)]
        private Int32 _PassFailHorzTolerance;
        [BinaryConvert.Seializable(false)]
        private ChannelId _PassFailMaskSource;
        [BinaryConvert.Seializable(false)]
        private Int32 _PassFailViolations;
        [BinaryConvert.Seializable(false)]
        private Int32 _PassFailTestWfms;
        [BinaryConvert.Seializable(false)]
        private Int32 _PassFailTestDurationByms;
        [BinaryConvert.Seializable(false)]
        private Boolean _PassFailStore;
        [BinaryConvert.Seializable(false)]
        private Boolean _PassFailBeep;
        [BinaryConvert.Seializable(false)]
        private Boolean _PassFailPulse;
        [BinaryConvert.Seializable(false)]
        private Boolean _PassFailHardCopy;
        [BinaryConvert.Seializable(false)]
        private PFStdMaskType _PassFailStdMaskType;
        [BinaryConvert.Seializable(false)]
        private Boolean _PassFailMaskLocked;
        [BinaryConvert.Seializable(false)]
        private Int32 _PassFailStdMaskIndex;
        #region REF
        private record RefSettings(Boolean active, String fullFileName, Double scaleBymV, Double posIndexBymDiv, Double timeScale, Double timePosIndexBymDiv, String label, Boolean ylevel_SelectStatus);
        [BinaryConvert.Seializable(false)]
        private RefSettings[]? _REF;
        #endregion REF

        #region Lissajous
        private record LissajousSettings(Int32 id, ChannelId xSource, ChannelId ySource);
        [BinaryConvert.Seializable(false)]
        private LissajousSettings[]? _Lissajous;
        #endregion Lissajous
        public void OnSerializing()
        {
            _Product = PlatformManager.Default.Platform.ProductType;

            Int32 i;
            _Analog = new AnalogChnlSettings[ChannelIdExt.AnaChnlNum];
            for (i = 0; i < _Analog.Length; i++)
            {
                var ach = (AnalogModel)DsoModel.Default.GetChannel(ChannelId.C1 + i);
                _Analog[i] = new(ach.Active, (Int32)ach.Conditioning.ScaleIndex, ach.Conditioning.PosIndex, ach.Conditioning.Prefix, ach.Conditioning.Unit)
                {
                    Bandwidth = ach.Conditioning.Bandwidth,
                    ProbeUnitIsCustomized = ach.Conditioning.ProbeUnitIsCustomized,
                    IsInverted = ach.Conditioning.IsInverted,
                    Coupling = ach.Conditioning.Coupling,
                    ProbeIndex = ach.Conditioning.ProbeIndex,
                    Scale = ach.Conditioning.Scale,
                    Position = ach.Conditioning.Position,
                    InputSource = (AnaChnlIpnutSource)(ach.Conditioning.FlagInfo == null ? new AnaChnlIpnutSource() : ach.Conditioning.FlagInfo),
                    InterChannelOffset = ach.Deskew,
                };
            }

            _Math = new MathChnlSettings[ChannelIdExt.MathChnlNum];
            for (i = 0; i < _Math.Length; i++)
            {
                var mch = (MathModel)DsoModel.Default.GetChannel(ChannelId.M1 + i);
                _Math[i] = new(mch.Active, mch.Formula)
                {
                    ChnlScale = (mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                    ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                    ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),
                    ChnlInitScale = mch.Conditioning.InitialScale,

                    SamplingScale = (mch.Sampling.ScaleIndex, mch.Sampling.Scale),
                    SamplingPosition = (mch.Sampling.PosIndex, mch.Sampling.Position),
                    SamplingInitScale = mch.Sampling.InitialScale,

                    IsAutoUnit = mch.Conditioning.IsAutoUnit,
                };
            }

            _RadioFrequency = new RadioFrequencyChnlSettings[ChannelIdExt.RFChnlNum];
            for (i = 0; i < _RadioFrequency.Length; i++)
            {
                var rfch = (RadioFrequencyModel)DsoModel.Default.GetChannel(ChannelId.RF1 + i);
                _RadioFrequency[i] = new(rfch.Active)
                {
                    NormalLine = rfch.NormalLine,
                    MaxHoldLine = rfch.MaxHoldLine,
                    MinHoldLine = rfch.MinHoldLine,
                    AverageLine = rfch.AverageLine,
                    AverageTimes = rfch.AverageTimes,
                    Window = rfch.Window,
                    FFTLength = rfch.Sampling.FFTLength,
                    StartFrequency = rfch.Sampling.StartFrequency,
                    CenterFrequency = rfch.Sampling.CenterFrequency,
                    EndFrequency = rfch.Sampling.EndFrequency,
                    Span = rfch.Sampling.Span,
                    FreqScale = rfch.Sampling.FrequencyScale,
                    FigureCenterFrequency = rfch.Sampling.FigureCenterFrequency,
                    RefLevelValue = rfch.Conditioning.RefLevelValue,
                    UnitType = rfch.Conditioning.UnitType,
                    PUnit = rfch.Conditioning.PUnit,
                    AmpScale = rfch.Conditioning.AmpScale,
                    FigureCenterAmplitude = rfch.Conditioning.FigureCenterAmplitude,
                    Unit = rfch.Conditioning.Unit,
                    Prefix = rfch.Conditioning.Prefix,
                    PosIndex = rfch.Conditioning.PosIndex,
                };
            }

            _Bus = new BusChnlSettings[ChannelIdExt.BusChnlNum];
            _BusProtoclStr = new String[ChannelIdExt.BusChnlNum];
            var activedchs = DsoModel.Default.Channels.Where(c => c.Id.IsDecode()).ToArray();
            for (i = 0; i < _Bus.Length; i++)
            {
                ChannelModel am = activedchs[i];
                if (am is not DecodeModel dm)
                    continue;

                _Bus[i] = new BusChnlSettings(am.Active)
                {
                    Format = dm.Format,
                    PosIndexBymDiv = dm.Conditioning.PosIndex,
                    ProtocolType = dm.ProtocolType,
                    ScaleBymV = dm.Conditioning.Scale,
                    ScaleIndex = dm.Conditioning.ScaleIndex,
                    ScaleTick = dm.Conditioning.ScaleTick,
                    Id = dm.Id
                };

                var protocolprsnt = ProtocolPrsnt.GetCurrentChannelDecodePrsnt(am.Id, DsoPrsnt.DefaultDsoPrsnt);
                if (protocolprsnt != null)
                {
                    _BusProtoclStr[i] = GetObjPropJson(protocolprsnt);
                }
            }

            _TriggerProtocolType = TriggerSerialShareParameter.Default.ProtocolType;
            var trigggermode = TriggerSerialShareParameter.Default.GetTriggerSerial(_TriggerProtocolType);
            if (trigggermode != null)
            {
                _CurrentSeriealTriggerInfo = GetObjPropJson(trigggermode);
            }

            var triggerDecodeMode = DecodeTools.GetChannelDecodeModel(TriggerSerialShareParameter.Default.Source, TriggerSerialShareParameter.Default.ProtocolType);
            if (triggerDecodeMode != null)
            {
                _CurrentSeriealTriggerDecodeInfo = GetObjPropJson(triggerDecodeMode);
            }

            var dch = (DigitalModel)DsoModel.Default.GetChannel(ChannelId.D0);
            var bits = new DigiChnlBitSettings[dch.Conditioning.Bits.Count];
            for (i = 0; i < bits.Length; i++)
            {
                bits[i] = new(dch.Conditioning.Bits[i].ActiveBit,
                    dch.Conditioning.Bits[i].ScaleIndex,
                    dch.Conditioning.Bits[i].PosIndex);
            }

            var ctrlgrps = new DigiChnlCtrlGrpSettings[dch.Conditioning.Groups.Count];
            for (i = 0; i < ctrlgrps.Length; i++)
            {
                ctrlgrps[i] = new(dch.Conditioning.Groups[i].Family,
                    dch.Conditioning.Groups[i].UserThroldIndex,
                    dch.Conditioning.Groups[i].UserHystIndex);
            }
            _Digital = new DigiChnlSettings(dch.FocusBitId, bits, ctrlgrps);


            _AcqMode = DsoModel.Default.Timebase.Mode;
            _AcqLength = DsoModel.Default.Timebase.StorageMode;
            _StorageDepthOpt = DsoModel.Default.Timebase.StorageDepthOpt;
            _AverageCnt = DsoModel.Default.Timebase.AverageCnt;
            _EnvelopeCnt = DsoModel.Default.Timebase.EnvelopeCnt;
            _EnvelopOpt = DsoModel.Default.Timebase.EnvelopOpt;
            _ClockSrc = DsoModel.Default.Timebase.ClockSrc;
            _InterplType = DsoModel.Default.Timebase.InterplType;
            _WorkMode = DsoModel.Default.Timebase.WorkMode;
            _CurFrameId = DsoModel.Default.Timebase.CurFrameId;
            _ReferFrameIds = DsoModel.Default.Timebase.ReferFrameIds;
            _SequentStartFrame = DsoModel.Default.Timebase.SequentStartFrame;
            _RenderType = DsoModel.Default.Timebase.RenderType;
            _EnhancedBitsActive = DsoModel.Default.Timebase.EnhancedBitsActive;
            _EnhancedBits = DsoModel.Default.Timebase.EnhancedBits;

            _TmbScaleIndex = (Int32)DsoModel.Default.Timebase.ScaleIndex;
            _TmbPosIndex = DsoModel.Default.Timebase.PosIndex;
            _TmbScale = DsoModel.Default.Timebase.Scale;
            _TmbPosition = DsoModel.Default.Timebase.Position;

            _TrgType = TriggerModel.Type;
            _TrgHoldoff = TriggerModel.HoldoffByps;

            _WfmPath = DsoModel.Default.File.WfmPath;
            _PicPath = DsoModel.Default.File.PicPath;

            #region 触发类型的配置保存

            var edge = (TriggerEdgeModel)DsoModel.Default.GetTriggerModel(TriggerType.Edge);
            _TrgEdgeSource = edge.Source;
            _TrgEdgeImp = edge.Impedance;
            _TrgEdgeCouple = edge.Coupling;
            _TrgEdgeSlope = edge.Slope;
            _TrgEdgeSensitivity = edge.SensitivityBymdiv;
            _TrgEdgePosition = edge.Positions.ToArray();

            var pw = (TriggerWidthModel)DsoModel.Default.GetTriggerModel(TriggerType.PulseWidth);
            _TrgPulseSource = pw.Source;
            _TrgPulsePolarity = pw.Polarity;
            _TrgPulseCondition = pw.Condition;
            _TrgPulseWidth = pw.WidthByps;
            _TrgPulseUpperWidth = pw.UpperWidthByps;
            _TrgPulsePosition = pw.Positions.ToArray();

            var video = (TriggerVideoModel)DsoModel.Default.GetTriggerModel(TriggerType.Video);
            _TrgVideoSource = video.Source;
            _TrgVideoSync = video.Sync;
            _TrgVideoLine = video.Line;
            _TrgVideoPolarity = video.Polarity;
            _TrgVideoStandard = video.Standard;
            _TrgVideoPosition = video.Positions.ToArray();

            var slope = (TriggerTransModel)DsoModel.Default.GetTriggerModel(TriggerType.Transition);
            _TrgSlopeSource = slope.Source;
            _TrgSlopeSlope = slope.Slope;
            _TrgSlopeCondition = slope.Condition;
            _TrgSlopeWidthbyps = slope.WidthByps;
            _TrgSlopeUpperWidthByps = slope.UpperWidthByps;
            _TrgSlopePosIndex = slope.PosIndex;

            var runt = (TriggerRuntModel)DsoModel.Default.GetTriggerModel(TriggerType.Runt);
            _TrgRuntSource = runt.Source;
            _TrgRuntPolarity = runt.Polarity;
            _TrgRuntCondition = runt.Condition;
            _TrgRuntWidthbyps = runt.WidthByps;
            _TrgRuntUpperWidthByps = runt.UpperWidthByps;
            _TrgRuntPosIndex = runt.PosIndex;

            var delay = (TriggerDelayModel)DsoModel.Default.GetTriggerModel(TriggerType.Delay);
            _TrgDelaySource1 = delay.SourceOne;
            _TrgDelaySource2 = delay.SourceTwo;
            _TrgDelayEdge1 = delay.SourceOneSlope;
            _TrgDelayEdge2 = delay.SourceTwoSlope;
            _TrgDelayCondition = delay.Condition;
            _TrgDelayWidthbyps = delay.WidthByps;
            _TrgDelayUpperWidthByps = delay.UpperWidthByps;
            _TrgDelayPosIndex = delay.PosIndex;
            _TrgDelayDataCompPosIndex = delay.DataCompPosIndex;

            var timeout = (TriggerTimeOutModel)DsoModel.Default.GetTriggerModel(TriggerType.TimeOut);
            _TrgTimeoutSource = timeout.Source;
            _TrgTimeoutPolarity = timeout.Polarity;
            _TrgTimeoutDurationByps = timeout.DurationByps;

            var duration = (TriggerSustainTimeModel)DsoModel.Default.GetTriggerModel(TriggerType.SustainTime);
            _TrgDurationCondition = duration.Condition;
            _TrgDurationWidthByps = duration.WidthByps;
            _TrgDurationUpperWidthByps = duration.UpperWidthByps;
            var durationconditiondict = duration.Bits?.GetConditions();
            if (durationconditiondict != null && durationconditiondict.Any())
            {
                List<ChannelId> ids = new List<ChannelId>();
                List<SustainTimeLevelCondition> conditions = new List<SustainTimeLevelCondition>();
                List<Double> positions = new List<Double>();
                foreach (var condition in durationconditiondict)
                {
                    if (condition.Key > ChannelId.C4)
                        continue;
                    ids.Add(condition.Key);
                    conditions.Add(condition.Value);
                    positions.Add(duration.Bits!.GetPosIndex(condition.Key));
                }
                _TrgDurationConditionsChannelID = ids.ToArray();
                _TrgDurationConditionsConditions = conditions.ToArray();
                _TrgDurationPositions = positions.ToArray();
            }

            var setupHold = (TriggerSetupHoldModel)DsoModel.Default.GetTriggerModel(TriggerType.SetupHold);
            _TrgSetupHoldClockSource = setupHold.ClkSource;
            _TrgSetupHoldClockPolarity = setupHold.ClkPolarity;
            _TrgSetupHoldDataSource = setupHold.DataSource;
            _TrgSetupHoldDataPolarity = setupHold.DataPosPolarity;
            _TrgSetupHoldClkCompPosIndex = setupHold.ClkCompPosIndex;
            _TrgSetupHoldUpperDataPosIndex = setupHold.UpperDataPosIndex;
            _TrgSetupHoldViolation = setupHold.Violation;
            _TrgSetupHoldTsuByps = setupHold.TsuByps;
            _TrgSetupHoldThdByps = setupHold.ThdByps;


            var nedge = (TriggerNEdgeModel)DsoModel.Default.GetTriggerModel(TriggerType.NEdge);
            _TrgNedgeDataSource = nedge.Source;
            _TrgNedgeDataPolarity = nedge.Polarity;
            _TrgNedgeDurationByps = nedge.DurationByps;
            _TrgNedgeEdgeNumber = nedge.EdgeNumber;


            var pattern = (TriggerPatternModel)DsoModel.Default.GetTriggerModel(TriggerType.Pattern);
            if (pattern.Bits != null)
            {
                var c1condition = pattern.Bits.GetCondition(ChannelId.C1);
                var c1position = pattern.Bits.GetPosIndex(ChannelId.C1);

                var c2condition = pattern.Bits.GetCondition(ChannelId.C2);
                var c2position = pattern.Bits.GetPosIndex(ChannelId.C2);

                var c3condition = pattern.Bits.GetCondition(ChannelId.C3);
                var c3position = pattern.Bits.GetPosIndex(ChannelId.C3);

                var c4condition = pattern.Bits.GetCondition(ChannelId.C4);
                var c4position = pattern.Bits.GetPosIndex(ChannelId.C4);

                _TrgPatternCondition = new PatLevelCondition[] { c1condition, c2condition, c3condition, c4condition };
                _TrgPatternPositions = new Double[] { c1position, c2position, c3position, c4position };
            }

            #endregion


            var disp = DsoModel.Default.Display;
            _WfmDrawMode = disp.DrawMode;
            _WfmLayout = disp.WfmLayout;
            _Persist = disp.Persist;
            _WfmIntensity = disp.WfmIntensity;
            _GridIntensity = disp.GridIntensity;
            _GridStyle = disp.GridStyle;
            _AxisTickVisible = disp.AxisTickVisible;
            _XAxisTickBottom = disp.XAxisTickBottom;
            _YAxisTickRight = disp.YAxisTickRight;

            var meas = DsoModel.Default.Meas;
            _MeasVisiable = meas.Active;
            _Strobe = meas.Strobe;
            _Indicator = meas.Indicator;
            _SnapshotSrc = meas.SnapshotSource;
            _SnapshotActive = meas.SnapshotActive;
            _IsStatActive = meas.IsStatActive;

            _MeasItems = new MeasItemSettings[meas.SelectedItems.Length];
            for (i = 0; i < _MeasItems.Length; i++)
            {
                var selected = meas.SelectedItems[i];
                _MeasItems[i] = new(selected.Active, selected.Name, selected.Source, selected.Source2nd, selected.IsStatActive, selected.MeasureType, selected.Operation)
                {
                    RefUnit = selected.RefLevel.RefUnit,
                    RefStd = selected.RefLevel.RefStandard,
                    MidThrold = selected.RefLevel.MidThrold,
                    HighThrold = selected.RefLevel.HighThrold,
                    LowThrold = selected.RefLevel.LowThrold,
                };
            }

            var cursors = DsoModel.Default.Cursors;
            _CursorActive = cursors.Active;
            _CursorType = cursors.Type;
            _CursorPosMode = cursors.PositionMode;
            _HCursorSource = cursors.HCursor.Source;
            _HCursorFormat = cursors.HCursor.PosFormat;
            _HCursorPosition = cursors.HCursor.PosIndexes.Select(o => o.Position).ToArray();
            _VCursorSource = cursors.VCursor.Source;
            _VCursorFormat = cursors.VCursor.PosFormat;
            _VCursorPosition = cursors.VCursor.PosIndexes.Select(o => o.Position).ToArray();
            _TraceWave = cursors.TraceWave;
            _IsSyncMove = cursors.IsSyncMove;

            _Awg = new AwgSettings[ChannelIdExt.AwgNum];
            for (i = 0; i < _Awg.Length; i++)
            {
                var awg = DsoModel.Default.GetWfmGenerator(ChannelId.AWG1 + i);
                //2024/4/12 09:24 第一次启动AWG，幅度超过2.5V将会过压保护
                Int32 amp = awg.Amplitude > 4800 ? 4800 : awg.Amplitude;
                _Awg[i] = new(awg.Active, awg.Mode, awg.WfmType, awg.Frequency, amp, awg.Offset, awg.Duty, awg.Phase, awg.Noise)
                {
                    ModMethod = awg.ModMethod,
                    ModulatedWfm = awg.ModulatedWfm,
                    ModFreq = awg.ModFreq,
                    AmpDepth = awg.AmpDepth,
                    FreqBias = awg.FreqBias,
                    //<Remark>更改人：彭博 创建日期：2024/1/16 9:57:00  原因：保存扫频的起始频率 </Remark>
                    SweepStartFreq = awg.SweepStartFreq,
                    SweepEndFreq = awg.SweepEndFreq,
                    SweepDuration = awg.SweepDuration,
                    ContinuousArbWfmType = awg.ContinuousArbWfmType,
                    ModulationWfmType = awg.ModulationWfmType,
                    SweepWfmType = awg.SweepWfmType,
                    IsShow = awg.IsShow,
                };
            }
            Int32 num = DsoModel.Default.Channels.Where(c => c.Id.IsReference()).ToList().Count();
            if (num > 0)
            {
                _REF = new RefSettings[num];
                Int32 index = 0;
                foreach (var item in DsoModel.Default.ReferenceChnls)
                {
                    if (index >= num)
                    {
                        break;
                    }
                    _REF[index] = new RefSettings(item.Active, item.FullFileName, item.ScaleBymV, item.Conditioning.PosIndex, item.Sampling.Scale, item.Sampling.PosIndex, item.Label, item.Ylevel_SelectStatus);
                    index++;
                }
            }

            num = LissajousPrsnt.GetLissajousCount();
            if (num > 0)
            {
                _Lissajous = new LissajousSettings[num];
                Int32 index = 0;
                foreach (var item in LissajousPrsnt.LissajousPrsnts)
                {
                    if (index >= num)
                    {
                        break;
                    }
                    if (item is LissajousPrsnt prsnt)
                    {
                        _Lissajous[index] = new LissajousSettings(prsnt.ID, prsnt.SourceX, prsnt.SourceY);
                        index++;
                    }
                }

            }


            var vm = DsoModel.Default.Voltmeter;
            _VoltmeterActive = vm.Active;
            _VoltmeterStatActive = vm.IsStatActive;
            _VoltmeterSource = vm.Source;
            _VoltmeterMode = vm.Mode;

            var cm = DsoModel.Default.Cymometer;
            _CymometerActive = cm.Active;
            _CymometerStatActive = cm.IsStatActive;
            _CymometerSource = cm.Source;
            _CymometerShowPeriod = cm.ShowPeriod;

            var filter = DsoModel.Default.Filter;
            _FilterResp = filter.RespType;
            _FilterType = filter.FilterType;
            _FIRMethod = filter.FIRMethod;
            _IIRMethod = filter.IIRMethod;
            _FilterOrderMode = filter.OrderMode;
            _FilterWindow = filter.Window;
            _FilterOrder = filter.Order;
            _FilterSamplingFreq = filter.SamplingFreq;
            _FilterLowPassFreq = filter.LowPassFreq;
            _FilterLowStopFreq = filter.LowStopFreq;
            _FilterHighPassFreq = filter.HighPassFreq;
            _FilterHighStopFreq = filter.HighStopFreq;
            _FilterPassMag = filter.PassMag;
            _FilterStopMag = filter.StopMag;
            _FilterDensityFactor = filter.DensityFactor;
            var jitter = DsoModel.Default.JitterModel;
            _JitterActive = jitter.Active;
            _EyeEnable = jitter.EyeEnable;
            _PatternLength = jitter.PatternLength;
            _SignalType = jitter.SignalType;
            _ThresholdFreq = jitter.ThresholdFreq;
            _Threshold = jitter.Threshold;
            _Source = jitter.Source;
            _ClockType = jitter.ClockType;
            _PllType = jitter.PllType;
            _JitterParamEnable = jitter.JitterParamEnable;
            _EyeParamEnable = jitter.EyeParamEnable;
            _CutoffFreq1 = jitter.CutoffFreq1;
            _CutoffDivisor = jitter.CutoffDivisor;
            _DamplingFactor = jitter.DamplingFactor;
            _NaturalFreq = jitter.NaturalFreq;
            _CurrentBinNum = jitter.CurrentBinNum;
            _BitRate = jitter.BitRate;
            _Hysteresis = jitter.Hysteresis;

            var jittergraphtypearray = Enum.GetValues<JitterGraphType>();
            _JitterGraphTypeList = new List<(JitterGraphType, Boolean)>();
            foreach (var item in jittergraphtypearray)
            {
                if (item == JitterGraphType.QFactor)
                {
                    continue;
                }
                Boolean result = DsoPrsnt.DefaultDsoPrsnt?.Jitter?.IsEnableGraphByType(item) ?? false;
                _JitterGraphTypeList.Add((item, result));
            }


            _NavBarGroupRecordKeys = DsoModel.NavBarGroupRecords.Keys.ToList();
            _NavBarGroupRecordValues = DsoModel.NavBarGroupRecords.Values.ToList();

            _PassFailActive = DsoModel.Default.PassFail.Active;
            _PassFailSource = DsoModel.Default.PassFail.Source;
            _PassFailMode = DsoModel.Default.PassFail.Mode;
            _PassFailVertTolerance = DsoModel.Default.PassFail.LimitTest.VertTolerance;
            _PassFailHorzTolerance = DsoModel.Default.PassFail.LimitTest.HorzTolerance;
            _PassFailMaskSource = DsoModel.Default.PassFail.MaskSource;
            _PassFailViolations = DsoModel.Default.PassFail.Violations;
            _PassFailTestWfms = DsoModel.Default.PassFail.TestWfms;
            _PassFailTestDurationByms = DsoModel.Default.PassFail.TestDurationByms;
            _PassFailStore = DsoModel.Default.PassFail.Store;
            _PassFailBeep = DsoModel.Default.PassFail.Beep;
            _PassFailPulse = DsoModel.Default.PassFail.Pulse;
            _PassFailHardCopy = DsoModel.Default.PassFail.HardCopy;
            _PassFailStdMaskType = DsoModel.Default.PassFail.StdMaskTest.StdMaskType;
            _PassFailMaskLocked = DsoModel.Default.PassFail.MaskLocked;
            _PassFailStdMaskIndex = DsoModel.Default.PassFail.StdMaskTest.MaskIndex;


            var pwranalysislist = DsoPrsnt.DefaultDsoPrsnt?.PwrAnalysisDictionary?.ToList();
            var poweranalysesnum = pwranalysislist?.Count ?? -1;
            if (poweranalysesnum > 0)
            {
                _PowerAnalysis = new PowerAnalysisSettings[poweranalysesnum];
                Int32 index = 0;
                foreach (var item in pwranalysislist)
                {
                    _PowerAnalysis[index] = new PowerAnalysisSettings(item.Value.Mode, item.Value.VoltageSrc1, item.Value.CurrentSrc1)
                    {
                        PwrQuality = new(item.Value.QualityPrsnt.Value.RefFreq),
                        PwrHarmonic = new PwrHarmonicSettings(item.Value.HarmonicPrsnt.Value.HarmonicNum, item.Value.HarmonicPrsnt.Value.Source, item.Value.HarmonicPrsnt.Value.RefFreqSrc, item.Value.HarmonicPrsnt.Value.HarmonicOpt),
                        PwrRipple = new PwrRippleSettings(item.Value.RipplePrsnt.Value.Source),
                        PwrSwitchingLoss = new PwrSwitchingLossSettings(item.Value.SwitchingLossPrsnt.Value.RdsOn),
                        PwrSOA = new PwrSOASettings(item.Value.SOAPrsnt.Value.StopOnFail, item.Value.SOAPrsnt.Value.MaxLinX, item.Value.SOAPrsnt.Value.MinLinX, item.Value.SOAPrsnt.Value.MaxLinY, item.Value.SOAPrsnt.Value.MinLinY),
                        PwrLoopAnalysis = new PwrLoopAnalysisSettings(item.Value.LoopAnalysisPrsnt.Value.AWGSource, item.Value.LoopAnalysisPrsnt.Value.Impedance, item.Value.LoopAnalysisPrsnt.Value.StartFreq,
                        item.Value.LoopAnalysisPrsnt.Value.EndFreq, item.Value.LoopAnalysisPrsnt.Value.ScanNum, item.Value.LoopAnalysisPrsnt.Value.AmplitudeMode, item.Value.LoopAnalysisPrsnt.Value.CheckTriggerStatus)
                    };
                    index++;
                }
            }

        }

        private PropertyInfo[] GetReadWriteProperties(Type type)
        {
            // 递归获取类型及其所有基类的属性
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            // 递归获取基类的属性
            Type baseType = type.BaseType;
            if (baseType != null)
            {
                PropertyInfo[] baseProperties = GetReadWriteProperties(baseType);
                properties = properties.Concat(baseProperties).ToArray();
            }

            // 过滤出可读可写的属性
            properties = properties.Where(p => p.CanRead && p.CanWrite).ToArray();

            return properties;
        }

        /// <summary>
        /// 获取对象的可读可写属性，转为Json字符串。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private String GetObjPropJson(Object obj)
        {
            if (obj == null)
                return null;

            var triggertype = obj.GetType();
            if (triggertype.IsValueType)
                return null;

            var allprops = GetReadWriteProperties(triggertype);
            Dictionary<String, Object> result = new Dictionary<String, Object>();
            foreach (var property in allprops)
            {
                if (!property.CanRead || !property.CanWrite)
                    continue;

                var v = property.GetValue(obj, null);
                if (v == null)
                    continue;

                result.Add(property.Name, v);
            }

            return JsonSerializer.Serialize(result);
        }

        /// <summary>
        /// 将Json数据赋值给对象属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="jsonobj"></param>
        private void SetObjProps(Object obj, String jsonstr, Func<JsonObject, Boolean> condition = null)
        {
            if (obj == null || String.IsNullOrEmpty(jsonstr))
                return;

            JsonObject? jsonobj = JsonSerializer.Deserialize(jsonstr, typeof(JsonObject)) as JsonObject;
            if (jsonobj == null)
                return;

            if (condition != null)
            {
                if (!condition.Invoke(jsonobj))
                    return;
            }

            var ptype = obj.GetType();
            var allprops = GetReadWriteProperties(ptype);
            foreach (var prop in allprops)
            {
                if (!prop.CanWrite || !prop.CanRead)
                    continue;

                try
                {
                    var propval = jsonobj[prop.Name]?.ToString();

                    if (propval == null)
                        continue;

                    if (prop.PropertyType.IsEnum)
                    {
                        if (Enum.TryParse(prop.PropertyType, propval, out Object? resval))
                        {
                            prop.SetValue(obj, resval);
                        }
                    }
                    else if (prop.PropertyType.IsValueType)
                    {
                        var val = Convert.ChangeType(propval, prop.PropertyType);
                        prop.SetValue(obj, val);
                    }
                    else
                    {
                        prop.SetValue(obj, propval);
                    }
                }
                catch (Exception ex)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
                }
            }
        }

        public void OnDeserialized()
        {
            if (_Product != PlatformManager.Default.Platform.ProductType || _Version != _VER)
            {
                return;
            }

            foreach (var id in ChannelIdExt.GetAnalogs())
            {
                var ach = (AnalogModel)DsoModel.Default.GetChannel(id);

                Int32 i = id - ChannelId.C1;

                ach.Active = _Analog![i].Active;
                ach.Conditioning.ScaleIndex = (AnaChnlScaleIndex)_Analog[i].ScaleIndex;
                ach.Conditioning.PosIndex = _Analog[i].PosIndex;
                //ach.Conditioning.Prefix = _Analog[i].Prefix;
                ach.Conditioning.Unit = _Analog[i].Unit;

                ach.Conditioning.Bandwidth = _Analog[i].Bandwidth;
                ach.Conditioning.ProbeUnitIsCustomized = _Analog[i].ProbeUnitIsCustomized;
                ach.Conditioning.IsInverted = _Analog[i].IsInverted;
                ach.Conditioning.Coupling = _Analog[i].Coupling;
                ach.Conditioning.ProbeIndex = _Analog[i].ProbeIndex;
                ach.Conditioning.FlagInfo = _Analog[i].InputSource;

                ach.Deskew = _Analog[i].InterChannelOffset;
            }

            foreach (var id in ChannelIdExt.GetMaths())
            {
                var mch = (MathModel)DsoModel.Default.GetChannel(id);

                Int32 i = id - ChannelId.M1;

                mch.Active = Constants.ENABLE_Math && _Math![i].Active;
                mch.Formula = _Math[i].Formula;
                mch.Conditioning.IsAutoUnit = _Math[i].IsAutoUnit;
                mch.Conditioning.ScaleIndex = _Math[i].ChnlScale.Index;
                mch.Conditioning.PosIndex = _Math[i].ChnlPosition.Index;
                mch.Conditioning.Unit = _Math[i].ChnlUnit.Unit;
                mch.Conditioning.Prefix = _Math[i].ChnlUnit.Prefix;
                mch.Conditioning.InitialScale = _Math[i].ChnlInitScale;

                mch.Sampling.ScaleIndex = _Math[i].SamplingScale.Index;
                mch.Sampling.PosIndex = _Math[i].SamplingPosition.Index;
                mch.Sampling.InitialScale = _Math[i].SamplingInitScale;
            }

            TriggerSerialShareParameter.Default.ProtocolType = _TriggerProtocolType;

            if (_Bus != null && _Bus.Any())
            {
                var activedchs = DsoModel.Default.Channels.Where(c => c.Id.IsDecode()).ToArray();
                for (var i = 0; i < activedchs.Length; i++)
                {
                    var item = activedchs[i];
                    if (item is not DecodeModel dm)
                        continue;

                    var config = _Bus.FirstOrDefault(c => c.Id == item.Id);
                    if (config == null)
                        continue;

                    dm.Active = config.Active;
                    dm.Format = config.Format;
                    dm.Conditioning.ScaleTick = config.ScaleTick;
                    dm.Conditioning.ScaleIndex = config.ScaleIndex;
                    dm.Conditioning.Scale = config.ScaleBymV;
                    dm.ProtocolType = config.ProtocolType;
                    dm.Conditioning.PosIndex = config.PosIndexBymDiv;
                    /*if (config.Active)
                        TriggerSerialShareParameter.Default.ProtocolType = config.ProtocolType;*/

                    if (_BusProtoclStr != null && _BusProtoclStr.Length >= _Bus.Length)
                    {
                        var jsonstr = _BusProtoclStr[i];
                        var ppst = ProtocolPrsnt.GetCurrentChannelDecodePrsnt(item.Id, DsoPrsnt.DefaultDsoPrsnt);
                        if (!String.IsNullOrEmpty(jsonstr))
                        {
                            SetObjProps(ppst, jsonstr);
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(_CurrentSeriealTriggerInfo))
            {
                var trigggermode = TriggerSerialShareParameter.Default.GetTriggerSerial(TriggerSerialShareParameter.Default.ProtocolType);
                if (trigggermode != null)
                {
                    SetObjProps(trigggermode, _CurrentSeriealTriggerInfo);
                }
            }

            if (!String.IsNullOrEmpty(_CurrentSeriealTriggerDecodeInfo))
            {
                var triggerDecodeMode = DecodeTools.GetChannelDecodeModel(TriggerSerialShareParameter.Default.Source, TriggerSerialShareParameter.Default.ProtocolType);
                if (triggerDecodeMode != null)
                {
                    // 当协议类型不一致时，不要赋值，以免导致类型被意外修改
                    SetObjProps(triggerDecodeMode, _CurrentSeriealTriggerDecodeInfo, c =>
                    {
                        var cdition = c["ProtocolType"]?.ToString();
                        if (Enum.TryParse(typeof(SerialProtocolType), cdition, out Object? resval))
                        {
                            if (resval is SerialProtocolType spt && spt != triggerDecodeMode.ProtocolType)
                                return false;
                        }
                        return true;
                    });
                }
            }

            var dch = (DigitalModel)DsoModel.Default.GetChannel(ChannelId.D0);
            dch.FocusBitId = _Digital!.FocusBitId;

            for (var i = 0; i < _Digital.Bits.Length; i++)
            {
                dch.Conditioning.Bits[i].ActiveBit = Constants.ENABLE_LA && _Digital.Bits[i].Active;
                dch.Conditioning.Bits[i].ScaleIndex = _Digital.Bits[i].ScaleIndex;
                dch.Conditioning.Bits[i].PosIndex = _Digital.Bits[i].PosIndex;
            }
            for (var i = 0; i < _Digital.CtrlGroups.Length; i++)
            {
                dch.Conditioning.Groups[i].Family = _Digital.CtrlGroups[i].Family;
                dch.Conditioning.Groups[i].UserThroldIndex = _Digital.CtrlGroups[i].UserThroldIndex;
                dch.Conditioning.Groups[i].UserHystIndex = _Digital.CtrlGroups[i].UserHystIndex;
            }

            if (_RadioFrequency != null)
            {
                foreach (var id in ChannelIdExt.GetRadioFrequencies())
                {
                    var rfch = (RadioFrequencyModel)DsoModel.Default.GetChannel(id);

                    Int32 i = id - ChannelId.RF1;

                    rfch.Active = _RadioFrequency![i].Active;
                    rfch.NormalLine = _RadioFrequency[i].NormalLine;
                    rfch.MaxHoldLine = _RadioFrequency[i].MaxHoldLine;
                    rfch.MinHoldLine = _RadioFrequency[i].MinHoldLine;
                    rfch.AverageLine = _RadioFrequency[i].AverageLine;
                    rfch.AverageTimes = _RadioFrequency[i].AverageTimes;
                    rfch.Window = _RadioFrequency[i].Window;
                    rfch.Sampling.FFTLength = _RadioFrequency[i].FFTLength;
                    //rfch.Sampling.StartFrequency = _RadioFrequency[i].StartFrequency;
                    rfch.Sampling.CenterFrequency = _RadioFrequency[i].CenterFrequency;
                    //rfch.Sampling.EndFrequency = _RadioFrequency[i].EndFrequency;
                    rfch.Sampling.Span = _RadioFrequency[i].Span;
                    //rfch.Sampling.RBW = _RadioFrequency[i].RBW;
                    rfch.Sampling.FigureCenterFrequency = _RadioFrequency[i].FigureCenterFrequency;
                    rfch.Sampling.FrequencyScale = _RadioFrequency[i].FreqScale;
                    rfch.TimeVSFrequency.Sampling.FFTLength = _RadioFrequency[i].FFTLength;
                    //rfch.TimeVSFrequency.Sampling.StartFrequency = _RadioFrequency[i].StartFrequency;
                    rfch.TimeVSFrequency.Sampling.CenterFrequency = _RadioFrequency[i].CenterFrequency;
                    //rfch.TimeVSFrequency.Sampling.EndFrequency = _RadioFrequency[i].EndFrequency;
                    rfch.TimeVSFrequency.Sampling.Span = _RadioFrequency[i].Span;
                    //rfch.TimeVSFrequency.Sampling.RBW = _RadioFrequency[i].RBW;
                    rfch.TimeVSFrequency.Sampling.FigureCenterFrequency = _RadioFrequency[i].FigureCenterFrequency;
                    rfch.TimeVSFrequency.Sampling.FrequencyScale = _RadioFrequency[i].FreqScale;
                    rfch.Conditioning.RefLevelValue = _RadioFrequency[i].RefLevelValue;
                    rfch.Conditioning.UnitType = _RadioFrequency[i].UnitType;
                    rfch.Conditioning.PUnit = _RadioFrequency[i].PUnit;
                    rfch.Conditioning.AmpScale = _RadioFrequency[i].AmpScale;
                    rfch.Conditioning.FigureCenterAmplitude = _RadioFrequency[i].FigureCenterAmplitude;
                    rfch.Conditioning.Unit = _RadioFrequency[i].Unit;
                    rfch.Conditioning.Prefix = _RadioFrequency[i].Prefix;
                    rfch.Conditioning.PosIndex = _RadioFrequency[i].PosIndex;
                }
            }

            DsoModel.Default.Timebase.Mode = _AcqMode;
            DsoModel.Default.Timebase.StorageMode = _AcqLength;
            DsoModel.Default.Timebase.StorageDepthOpt = _StorageDepthOpt;

            DsoModel.Default.Timebase.AverageCnt = _AverageCnt;
            DsoModel.Default.Timebase.EnvelopeCnt = _EnvelopeCnt;
            DsoModel.Default.Timebase.EnvelopOpt = _EnvelopOpt;
            DsoModel.Default.Timebase.ClockSrc = _ClockSrc;

            DsoModel.Default.Timebase.InterplType = _InterplType;
            DsoModel.Default.Timebase.WorkMode = _WorkMode;
            DsoModel.Default.Timebase.CurFrameId = _CurFrameId;
            DsoModel.Default.Timebase.ReferFrameIds = _ReferFrameIds;
            DsoModel.Default.Timebase.SequentStartFrame = _SequentStartFrame;
            DsoModel.Default.Timebase.RenderType = _RenderType;

            DsoModel.Default.Timebase.ScaleIndex = (AnaChnlTimebaseIndex)_TmbScaleIndex;
            DsoModel.Default.Timebase.PosIndex = _TmbPosIndex;
            DsoModel.Default.Timebase.EnhancedBitsActive = _EnhancedBitsActive;
            DsoModel.Default.Timebase.EnhancedBits = _EnhancedBits;

            TriggerModel.Type = _TrgType;
            TriggerModel.HoldoffByps = _TrgHoldoff;

            if (Directory.Exists(_WfmPath))
            {
                DsoModel.Default.File.WfmPath = _WfmPath;
            }
            if (Directory.Exists(_PicPath))
            {
                DsoModel.Default.File.PicPath = _PicPath;
            }

            #region 触发配置加载
            var edge = (TriggerEdgeModel)DsoModel.Default.GetTriggerModel(TriggerType.Edge);
            edge.Source = _TrgEdgeSource;
            edge.Impedance = _TrgEdgeImp;
            edge.Coupling = _TrgEdgeCouple;
            edge.SensitivityBymdiv = _TrgEdgeSensitivity;
            edge.Slope = _TrgEdgeSlope;
            if (_TrgEdgePosition is not null)
            {
                edge.SetPosIndex(ChannelId.C1, _TrgEdgePosition[0].Index);
                edge.SetPosIndex(ChannelId.C2, _TrgEdgePosition[1].Index);
                edge.SetPosIndex(ChannelId.C3, _TrgEdgePosition[2].Index);
                edge.SetPosIndex(ChannelId.C4, _TrgEdgePosition[3].Index);
                edge.SetPosIndex(ChannelId.Ext, _TrgEdgePosition[4].Index);
            }

            var pulse = (TriggerWidthModel)DsoModel.Default.GetTriggerModel(TriggerType.PulseWidth);
            pulse.Source = _TrgPulseSource;
            pulse.Polarity = _TrgPulsePolarity;
            pulse.Condition = _TrgPulseCondition;
            pulse.WidthByps = _TrgPulseWidth;
            pulse.UpperWidthByps = _TrgPulseUpperWidth;
            if (_TrgPulsePosition is not null)
            {
                pulse.SetPosIndex(ChannelId.C1, _TrgPulsePosition[0].Index);
                pulse.SetPosIndex(ChannelId.C2, _TrgPulsePosition[1].Index);
                pulse.SetPosIndex(ChannelId.C3, _TrgPulsePosition[2].Index);
                pulse.SetPosIndex(ChannelId.C4, _TrgPulsePosition[3].Index);
                pulse.SetPosIndex(ChannelId.Ext, _TrgPulsePosition[4].Index);
            }

            var video = (TriggerVideoModel)DsoModel.Default.GetTriggerModel(TriggerType.Video);
            video.Source = _TrgVideoSource;
            video.Sync = _TrgVideoSync;
            video.Line = _TrgVideoLine;
            video.Polarity = _TrgVideoPolarity;
            video.Standard = _TrgVideoStandard;
            if (_TrgVideoPosition is not null)
            {
                video.SetPosIndex(ChannelId.C1, _TrgVideoPosition[0].Index);
                video.SetPosIndex(ChannelId.C2, _TrgVideoPosition[1].Index);
                video.SetPosIndex(ChannelId.C3, _TrgVideoPosition[2].Index);
                video.SetPosIndex(ChannelId.C4, _TrgVideoPosition[3].Index);
                video.SetPosIndex(ChannelId.Ext, _TrgVideoPosition[4].Index);
            }

            var slope = (TriggerTransModel)DsoModel.Default.GetTriggerModel(TriggerType.Transition);
            slope.Source = _TrgSlopeSource;
            slope.Slope = _TrgSlopeSlope;
            slope.Condition = _TrgSlopeCondition;
            slope.UpperWidthByps = _TrgSlopeUpperWidthByps;
            slope.WidthByps = _TrgSlopeWidthbyps;
            slope.PosUpperIndex = _TrgSlopePosIndex.Upper;
            slope.PosLowerIndex = _TrgSlopePosIndex.Lower;

            var runt = (TriggerRuntModel)DsoModel.Default.GetTriggerModel(TriggerType.Runt);
            runt.Source = _TrgRuntSource;
            runt.Polarity = _TrgRuntPolarity;
            runt.Condition = _TrgRuntCondition;
            runt.UpperWidthByps = _TrgRuntUpperWidthByps;
            runt.WidthByps = _TrgRuntWidthbyps;
            runt.PosUpperIndex = _TrgRuntPosIndex.Upper;
            runt.PosLowerIndex = _TrgRuntPosIndex.Lower;

            var delay = (TriggerDelayModel)DsoModel.Default.GetTriggerModel(TriggerType.Delay);
            delay.SourceOne = _TrgDelaySource1;
            if (_TrgDelaySource2 == _TrgDelaySource1)
                _TrgDelaySource2 = _TrgDelaySource1 + 1;
            delay.SourceTwo = _TrgDelaySource2;
            delay.SourceOneSlope = _TrgDelayEdge1;
            delay.SourceTwoSlope = _TrgDelayEdge2;
            delay.Condition = _TrgDelayCondition;
            delay.UpperWidthByps = _TrgDelayUpperWidthByps;
            delay.WidthByps = _TrgDelayWidthbyps;
            delay.PosUpperIndex = _TrgDelayPosIndex.Upper;
            delay.PosLowerIndex = _TrgDelayPosIndex.Lower;
            delay.DataCompPosIndex = _TrgDelayDataCompPosIndex;

            var timeout = (TriggerTimeOutModel)DsoModel.Default.GetTriggerModel(TriggerType.TimeOut);
            timeout.Source = _TrgTimeoutSource;
            timeout.Polarity = _TrgTimeoutPolarity;
            timeout.DurationByps = _TrgTimeoutDurationByps;

            var duration = (TriggerSustainTimeModel)DsoModel.Default.GetTriggerModel(TriggerType.SustainTime);
            duration.Condition = _TrgDurationCondition;
            duration.UpperWidthByps = _TrgDurationUpperWidthByps;
            duration.WidthByps = _TrgDurationWidthByps;
            if (_TrgDurationConditionsChannelID != null && _TrgDurationConditionsChannelID.Any() && _TrgDurationConditionsConditions != null && _TrgDurationConditionsConditions.Length == _TrgDurationConditionsChannelID.Length)
            {
                Boolean cansetposition = _TrgDurationPositions != null && _TrgDurationPositions.Length == _TrgDurationConditionsChannelID.Length;
                for (Int32 i = 0; i < _TrgDurationConditionsChannelID.Length; i++)
                {
                    duration.Bits.SetCondition(_TrgDurationConditionsChannelID[i], _TrgDurationConditionsConditions[i]);
                    if (cansetposition)
                    {
                        duration.Bits.SetPosIndex(_TrgDurationConditionsChannelID[i], _TrgDurationPositions![i]);
                    }
                }
            }

            var setupHold = (TriggerSetupHoldModel)DsoModel.Default.GetTriggerModel(TriggerType.SetupHold);
            setupHold.ClkSource = _TrgSetupHoldClockSource;
            setupHold.ClkPolarity = _TrgSetupHoldClockPolarity;
            if (_TrgSetupHoldDataSource == _TrgSetupHoldClockSource)
                _TrgSetupHoldDataSource = _TrgSetupHoldClockSource + 1;
            setupHold.DataSource = _TrgSetupHoldDataSource;
            setupHold.DataPosPolarity = _TrgSetupHoldDataPolarity;
            setupHold.ClkCompPosIndex = _TrgSetupHoldClkCompPosIndex;
            setupHold.UpperDataPosIndex = _TrgSetupHoldUpperDataPosIndex;
            setupHold.Violation = _TrgSetupHoldViolation;
            setupHold.TsuByps = _TrgSetupHoldTsuByps;
            setupHold.ThdByps = _TrgSetupHoldThdByps;

            var nedge = (TriggerNEdgeModel)DsoModel.Default.GetTriggerModel(TriggerType.NEdge);
            nedge.Source = _TrgNedgeDataSource;
            nedge.Polarity = _TrgNedgeDataPolarity;
            nedge.DurationByps = _TrgNedgeDurationByps;
            nedge.EdgeNumber = _TrgNedgeEdgeNumber;

            var pattern = (TriggerPatternModel)DsoModel.Default.GetTriggerModel(TriggerType.Pattern);
            if (pattern.Bits != null)
            {
                if (_TrgPatternCondition != null && _TrgPatternCondition.Length >= 4)
                {
                    pattern.Bits.SetCondition(ChannelId.C1, _TrgPatternCondition[0]);
                    pattern.Bits.SetCondition(ChannelId.C2, _TrgPatternCondition[1]);
                    pattern.Bits.SetCondition(ChannelId.C3, _TrgPatternCondition[2]);
                    pattern.Bits.SetCondition(ChannelId.C4, _TrgPatternCondition[3]);
                }

                if (_TrgPatternPositions != null && _TrgPatternPositions.Length >= 4)
                {
                    pattern.Bits.SetPosIndex(ChannelId.C1, _TrgPatternPositions[0]);
                    pattern.Bits.SetPosIndex(ChannelId.C2, _TrgPatternPositions[1]);
                    pattern.Bits.SetPosIndex(ChannelId.C3, _TrgPatternPositions[2]);
                    pattern.Bits.SetPosIndex(ChannelId.C4, _TrgPatternPositions[3]);
                }
            }

            #endregion

            var disp = DsoModel.Default.Display;
            disp.DrawMode = _WfmDrawMode;
            disp.WfmLayout = _WfmLayout;
            disp.Persist = _Persist;
            disp.WfmIntensity = _WfmIntensity;
            disp.GridIntensity = _GridIntensity;
            disp.GridStyle = _GridStyle;
            disp.AxisTickVisible = _AxisTickVisible;
            disp.XAxisTickBottom = _XAxisTickBottom;
            disp.YAxisTickRight = _YAxisTickRight;

            var meas = DsoModel.Default.Meas;
            meas.Active = Constants.ENABLE_Measure && _MeasVisiable;
            meas.Strobe = _Strobe;
            meas.Indicator = _Indicator;
            meas.SnapshotSource = _SnapshotSrc;
            //meas.SnapshotActive = _SnapshotActive;
            meas.IsStatActive = _IsStatActive;

            for (Int32 i = 0; i < _MeasItems?.Length; i++)
            {
                var selected = meas.SelectedItems[i];
                selected.Active = Constants.ENABLE_Measure && _MeasItems[i].Active;
                selected.MeasureType = _MeasItems[i].MeasureType;
                selected.Operation = _MeasItems[i].Operation;
                selected.Name = _MeasItems[i].Name;
                if (_MeasItems[i].Source.IsReference())
                {
                    var measitems = _MeasItems.Where(x => x.Name.Equals(_MeasItems[i].Name)).Select(x => (Int32)x.Source).ToList();
                    for (Int32 ii = 0; ii < ChannelIdExt.AnaChnlNum; ii++)
                    {
                        if (!measitems.Contains((Int32)(ChannelId.C1 + ii)))
                        {
                            selected.Source = (ChannelId)(ChannelId.C1 + ii);
                            break;
                        }
                        if (ii < ChannelIdExt.AnaChnlNum)
                        {
                            selected.Active = false;
                        }
                    }
                }
                else
                {
                    selected.Source = _MeasItems[i].Source;
                }
                selected.Source2nd = _MeasItems[i].Source2nd;
                selected.IsStatActive = _MeasItems[i].StatActive;
                selected.RefLevel.RefUnit = _MeasItems[i].RefUnit;
                selected.RefLevel.RefStandard = _MeasItems[i].RefStd;
                selected.RefLevel.MidThrold = _MeasItems[i].MidThrold;
                selected.RefLevel.HighThrold = _MeasItems[i].HighThrold;
                selected.RefLevel.LowThrold = _MeasItems[i].LowThrold;
            }

            var cursors = DsoModel.Default.Cursors;
            cursors.Active = _CursorActive;
            cursors.Type = _CursorType;
            cursors.PositionMode = _CursorPosMode;
            cursors.HCursor.Source = _HCursorSource;
            cursors.HCursor.PosFormat = _HCursorFormat;
            for (Int32 i = 0; i < _HCursorPosition?.Length; i++)
            {
                cursors.HCursor[i] = _HCursorPosition[i];
            }
            cursors.VCursor.Source = _VCursorSource;
            cursors.VCursor.PosFormat = _VCursorFormat;
            for (Int32 i = 0; i < _VCursorPosition?.Length; i++)
            {
                cursors.VCursor[i] = _VCursorPosition[i];
            }
            cursors.TraceWave = _TraceWave;
            cursors.IsSyncMove = _IsSyncMove;

            foreach (var id in ChannelIdExt.GetAWGs())
            {
                var awg = DsoModel.Default.GetWfmGenerator(id);

                Int32 i = id - ChannelId.AWG1;
                if (i >= _Awg!.Length)
                {
                    break;
                }

                //awg.Active = _Awg[i].Active; //AWG开机保持关闭
                awg.Active = _Awg[i].Active;
                awg.IsShow = _Awg[i].IsShow ?? false;
                awg.Mode = _Awg[i].Mode;
                awg.WfmType = _Awg[i].WfmType;
                awg.Frequency = _Awg[i].Frequency;
                //2024/4/12 09:24 第一次启动AWG，幅度超过2.5V将会过压保护
                awg.Amplitude = _Awg[i].Amplitude > 4800 ? 4800 : _Awg[i].Amplitude;
                awg.Offset = _Awg[i].Offset;
                awg.Duty = _Awg[i].Duty;
                awg.Phase = _Awg[i].Phase;
                awg.Noise = _Awg[i].Noise;
                awg.ModMethod = _Awg[i].ModMethod;
                awg.ModulatedWfm = _Awg[i].ModulatedWfm;
                awg.ModFreq = _Awg[i].ModFreq;
                awg.AmpDepth = _Awg[i].AmpDepth;
                awg.FreqBias = _Awg[i].FreqBias;
                //<Remark>更改人：彭博 创建日期：2024/1/16 9:57:00  原因：保存扫频的起始频率 </Remark>
                awg.SweepStartFreq = _Awg[i].SweepStartFreq;
                awg.SweepEndFreq = _Awg[i].SweepEndFreq;
                awg.SweepDuration = _Awg[i].SweepDuration;
                awg.ContinuousArbWfmType = _Awg[i].ContinuousArbWfmType;
                awg.ModulationWfmType = _Awg[i].ModulationWfmType;
                awg.SweepWfmType = _Awg[i].SweepWfmType;
            }

            var vm = DsoModel.Default.Voltmeter;
            vm.Active = _VoltmeterActive;
            vm.IsStatActive = _VoltmeterStatActive;
            vm.Source = _VoltmeterSource;
            vm.Mode = _VoltmeterMode;

            var cm = DsoModel.Default.Cymometer;
            cm.IsStatActive = _CymometerStatActive;
            cm.Active = _CymometerActive;
            cm.Source = _CymometerSource;
            cm.ShowPeriod = _CymometerShowPeriod;

            var filter = DsoModel.Default.Filter;
            filter.RespType = _FilterResp;
            filter.FilterType = _FilterType;
            filter.FIRMethod = _FIRMethod;
            filter.IIRMethod = _IIRMethod;
            filter.OrderMode = _FilterOrderMode;
            filter.Window = _FilterWindow;
            filter.Order = _FilterOrder;
            filter.SamplingFreq = _FilterSamplingFreq;
            filter.LowPassFreq = _FilterLowPassFreq;
            filter.LowStopFreq = _FilterLowStopFreq;
            filter.HighPassFreq = _FilterHighPassFreq;
            filter.HighStopFreq = _FilterHighStopFreq;
            filter.PassMag = _FilterPassMag;
            filter.StopMag = _FilterStopMag;
            filter.DensityFactor = _FilterDensityFactor;

            if (_NavBarGroupRecordKeys != null && _NavBarGroupRecordValues != null && _NavBarGroupRecordKeys.Count == _NavBarGroupRecordValues.Count)
            {
                DsoModel.NavBarGroupRecords.Clear();
                for (Int32 i = 0; i < _NavBarGroupRecordKeys.Count; i++)
                {
                    DsoModel.NavBarGroupRecords.AddOrUpdate(_NavBarGroupRecordKeys[i], _NavBarGroupRecordValues[i], (k, v) => _NavBarGroupRecordValues[i]);
                }
            }
            HdCmdFactory.Push(HdCmd.Run);
        }

        public void LoadFunction()
        {
            LoadPassFail();
            LoadPowerAnalysis();
            LoadJitter();
            //LoadRef();
            LoadLissajou();
            LoadSnapshot();
        }

        private void LoadSnapshot()
        {
            var meas = DsoModel.Default.Meas;
            meas.SnapshotActive = _SnapshotActive;
        }

        private void LoadLissajou()
        {
            if (_Lissajous == null)
            {
                return;
            }
            foreach (var item in _Lissajous)
            {
                if (item == null)
                {
                    continue;
                }
                if (LissajousPrsnt.GetorMakeLissajousPrsnt(item.id, out var xyprsnt))
                {
                    if (xyprsnt == null)
                    {
                        continue;
                    }
                    xyprsnt.SourceX = item.xSource;
                    xyprsnt.SourceY = item.ySource;
                    xyprsnt.Active = true;
                }
            }
        }

        private void LoadRef()
        {
            if (_REF == null)
            {
                return;
            }
            Int32 index = 0;
            foreach (var item in _REF)
            {
                if (item == null)
                {
                    continue;
                }
                ReferencePrsnt? rprsnt = null;
                if (ReferencePrsnt.TryAddRefPrsnt(ChannelId.R1 + index, item.fullFileName, ref rprsnt))
                {
                    rprsnt!.Active = item.active;
                    rprsnt!.ScaleBymV = item.scaleBymV;
                    rprsnt!.PosIndexBymDiv = item.posIndexBymDiv;
                    rprsnt!.Sampling.Scale = item.timeScale;
                    rprsnt!.Sampling.PosIndexBymDiv = item.timePosIndexBymDiv;
                    rprsnt!.Label = item.label;
                    rprsnt!.Ylevel_SelectStatus = item.ylevel_SelectStatus;
                    index++;
                }
            }
        }

        private void LoadJitter()
        {
            var jitter = DsoModel.Default.JitterModel;
            jitter.Active = _JitterActive;
            jitter.EyeEnable = _EyeEnable;
            jitter.PatternLength = _PatternLength;
            jitter.SignalType = _SignalType;
            jitter.ThresholdFreq = _ThresholdFreq;
            jitter.Threshold = _Threshold;
            jitter.Source = _Source;
            jitter.ClockType = _ClockType;
            jitter.PllType = _PllType;
            jitter.JitterParamEnable = _JitterParamEnable;
            jitter.EyeParamEnable = _EyeParamEnable;
            jitter.CutoffFreq1 = _CutoffFreq1;
            jitter.CutoffDivisor = _CutoffDivisor;
            jitter.DamplingFactor = _DamplingFactor;
            jitter.NaturalFreq = _NaturalFreq;
            jitter.CurrentBinNum = _CurrentBinNum;
            jitter.BitRate = _BitRate;
            jitter.Hysteresis = _Hysteresis;
            if (_JitterGraphTypeList != null)
            {
                foreach (var item in _JitterGraphTypeList)
                {
                    DsoPrsnt.DefaultDsoPrsnt?.Jitter?.SetGraphEnable(item.Item1, item.Item2);
                }
            }
        }

        private void LoadPowerAnalysis()
        {
            if (_PowerAnalysis == null)
            {
                return;
            }
            foreach (var item in _PowerAnalysis)
            {
                if (item == null)
                {
                    continue;
                }
                if (PowerAnalysisPrsnt.TryAddPowerAnalysis(item.mode, out var pwprsnt, item.voltageSrc, item.currentSrc))
                {
                    if (pwprsnt == null)
                    {
                        continue;
                    }
                    switch (item.mode)
                    {
                        case PowerAnalysisOpt.PowerQuality:
                            {
                                pwprsnt.QualityPrsnt.Value.RefFreq = item.PwrQuality.refFreq;
                            }
                            break;
                        case PowerAnalysisOpt.Harmonic:
                            {
                                pwprsnt.HarmonicPrsnt.Value.HarmonicNum = item.PwrHarmonic.harmonicNum;
                                pwprsnt.HarmonicPrsnt.Value.Source = item.PwrHarmonic.source;
                                pwprsnt.HarmonicPrsnt.Value.RefFreqSrc = item.PwrHarmonic.refFreqSrc;
                                pwprsnt.HarmonicPrsnt.Value.HarmonicOpt = item.PwrHarmonic.harmonicOpt;
                            }
                            break;
                        case PowerAnalysisOpt.Ripple:
                            {
                                pwprsnt.RipplePrsnt.Value.Source = item.PwrRipple.source;
                            }
                            break;
                        case PowerAnalysisOpt.SwitchingLoss:
                            {
                                pwprsnt.SwitchingLossPrsnt.Value.RdsOn = item.PwrSwitchingLoss.rdsOn;
                            }
                            break;
                        case PowerAnalysisOpt.SafeOperationArea:
                            {
                                pwprsnt.SOAPrsnt.Value.StopOnFail = item.PwrSOA.stopOnFail;
                                pwprsnt.SOAPrsnt.Value.MaxLinX = item.PwrSOA.maxLinX;
                                pwprsnt.SOAPrsnt.Value.MinLinX = item.PwrSOA.minLinX;
                                pwprsnt.SOAPrsnt.Value.MaxLinY = item.PwrSOA.maxLinY;
                                pwprsnt.SOAPrsnt.Value.MinLinY = item.PwrSOA.minLinY;
                            }
                            break;
                        case PowerAnalysisOpt.LoopAnalysis:
                            {
                                pwprsnt.LoopAnalysisPrsnt.Value.AWGSource = item.PwrLoopAnalysis.awgSource;
                                pwprsnt.LoopAnalysisPrsnt.Value.Impedance = item.PwrLoopAnalysis.impedance;
                                pwprsnt.LoopAnalysisPrsnt.Value.StartFreq = item.PwrLoopAnalysis.startFreq;
                                pwprsnt.LoopAnalysisPrsnt.Value.EndFreq = item.PwrLoopAnalysis.endFreq;
                                pwprsnt.LoopAnalysisPrsnt.Value.ScanNum = item.PwrLoopAnalysis.scanNum;
                                pwprsnt.LoopAnalysisPrsnt.Value.AmplitudeMode = item.PwrLoopAnalysis.amplitudeMode;
                                pwprsnt.LoopAnalysisPrsnt.Value.CheckTriggerStatus = item.PwrLoopAnalysis.checkTriggerStatus;
                            }
                            break;
                        case PowerAnalysisOpt.Modulation:
                            break;
                        case PowerAnalysisOpt.InrushCurrent:
                            break;
                        case PowerAnalysisOpt.PowerEfficency:
                            break;
                        case PowerAnalysisOpt.Differ:
                            break;
                        case PowerAnalysisOpt.Transient:
                            break;
                        case PowerAnalysisOpt.RDSon:
                            break;
                        case PowerAnalysisOpt.TurnOnOff:
                            break;
                        default:
                            break;
                    }
                }

            }
        }

        private void LoadPassFail()
        {
            DsoModel.Default.PassFail.Active = _PassFailActive;
            DsoModel.Default.PassFail.Source = _PassFailSource;
            DsoModel.Default.PassFail.Mode = _PassFailMode;
            DsoModel.Default.PassFail.LimitTest.VertTolerance = _PassFailVertTolerance;
            DsoModel.Default.PassFail.LimitTest.HorzTolerance = _PassFailHorzTolerance;
            DsoModel.Default.PassFail.MaskSource = _PassFailMaskSource;
            DsoModel.Default.PassFail.Violations = _PassFailViolations;
            DsoModel.Default.PassFail.TestWfms = _PassFailTestWfms;
            DsoModel.Default.PassFail.TestDurationByms = _PassFailTestDurationByms;
            DsoModel.Default.PassFail.Store = _PassFailStore;
            DsoModel.Default.PassFail.Beep = _PassFailBeep;
            DsoModel.Default.PassFail.Pulse = _PassFailPulse;
            DsoModel.Default.PassFail.HardCopy = _PassFailHardCopy;
            DsoModel.Default.PassFail.StdMaskTest.StdMaskType = _PassFailStdMaskType;
            DsoModel.Default.PassFail.MaskLocked = _PassFailMaskLocked;
            DsoModel.Default.PassFail.StdMaskTest.MaskIndex = _PassFailStdMaskIndex;
            if (DsoModel.Default.PassFail.Active)
            {
                if (DsoModel.Default.PassFail.Mode == PFTestMode.LimitMode)
                {
                    DsoPrsnt.DefaultDsoPrsnt?.PassFail?.MakeMask();
                }
                else
                {
                    DsoPrsnt.DefaultDsoPrsnt?.PassFail?.ReadStdMask();
                }
            }
        }
    }
}

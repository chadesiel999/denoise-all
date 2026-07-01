using System;
using System.Collections.Generic;
using System.Drawing;


namespace ScopeX.ComModel
{
	public partial record HdMessage
	{
		public UInt64 Command
		{
			get;
			init;
		}

		public Int32 LowPower
		{
			get;
			init;
		}

		public Int32 TempMode
		{
			get;
			init;
		}

        public Int32 ComboBits
        {
            get;
            init;
        }
        public Boolean bAcquireStopped
        {
            get;
            init;
        }
        public ChannelId FocusId
        {
            get;
            init;
        }

        /// <summary>
        /// 方波端子开关
        /// </summary>
        public Boolean bSquareWaveSwitch
        {
            get;
            init;
        }

        public DisplayOptions? Display
        {
            get;
            init;
        }
        public AnalogOptions[]? Analog
        {
            get;
            init;
        }

        public TimebaseOptions? Timebase
        {
            get;
            init;
        }

        public DecoderOptions[]? Decoder
        {
            get;
            init;
        }

        public TriggerOptions? Trigger
        {
            get;
            init;
        }

        public DigitalOptions[]? Digital
        {
            get;
            init;
        }

        public ArbWfmGenOptions[]? ArbWfmGen
        {
            get;
            init;
        }

        public CymometerOptions? Cymometer
        {
            get;
            init;
        }

        public SearchOptions? Search
        {
            get;
            init;
        }

        public PrecisionOptions? Precision
        {
            get;
            init;
        }

        public Dictionary<ChannelId, AiOptions>? AiTable
        {
            get;
            init;
        }

        public SystemOptions? System { get; init; }

        public MultiDomainRecord? MultiDomain { get; init; }


        public record DisplayOptions()
        {

            public WfmDrawMode DrawMode
            {
                get;
                init;
            }

            public WfmPersist Persist
            {
                get;
                init;
            }

            public Boolean IsFast
            {
                get;
                init;
            }
            public Int32 AnalogZIndex
            {
                get;
                init;
            }
        }

        public record AnalogOptions(Boolean Active, Int32 ScaleIndex, Double PositionIndex)
        {
            public Double Scale
            {
                get;
                init;
            }

            public Double ScaleBymV
            {
                get;
                init;
            }

            public Double Position
            {
                get;
                init;
            }

            public Double Bias
            {
                get;
                init;
            }

            /// <summary>
            /// add by lihuijun
            /// 
            /// 探头偏置校正，默认为0
            /// 检测到有源探头后，根据探头信息从本地校准数据读取
            /// </summary>
            public Double ProbeOffsetCaliBias
            {
                get;
                init;
            }

            public Int32 Bandwidth
            {
                get;
                init;
            }

            public Boolean IsInverted
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

            public Double ProbeGain
            {
                get;
                init;
            }


            public Double ProbeUnitRatio
            {
                get;
                init;
            }

            /// <summary>
            /// add by lihuijun
            /// 
            /// 探头增益校正系数，默认为1
            /// 检测到有源探头后，根据探头信息从本地校准数据读取
            /// </summary>
            public Double ProbeGainCaliRatio
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

            /// <summary>
            /// 通道第一级延时点数
            /// </summary>
            public int FirstStageDelay { get; init; }

            /// <summary>
            /// 通道第二级延时点数
            /// </summary>
            public int SecondStageDelay { get; init; }
        }

        public record TimebaseOptions(Double TmbPositionIndex, Double TmbPosition, Int32 TmbScaleIndex, Double TmbScale)
        {
            public AnaChnlStorageMode StorageMode
            {
                get;
                init;
            }
            public AnaChnlAcqMode AcqMode
            {
                get;
                init;
            }

            public AnaChnlStorageMode AcqLength
            {
                get;
                init;
            }

            public Boolean IsScan
            {
                get;
                init;
            }

            public Int64 StorageWaveDotsCnt
            {
                get;
                init;
            }

            public Int64 NeedWaveDotsCnt
            {
                get;
                init;
            }

            public AdcInterleaveMode InterleaveMode
            {
                get;
                init;
            }
            #region 分段存储

            public Boolean CallBack
            {
                get;
                init;
            }

            public Int32 FrameCount
            {
                get;
                init;
            }

            /// <summary>
            /// 选定帧
            /// </summary>
            public Int32 CurFrameId
            {
                get;
                init;
            }

            /// <summary>
            /// 参考帧
            /// </summary>
            public UInt32 ReferFrameId
            {
                get;
                init;
            }

            public UInt32 SegmentActive
            {
                get;
                init;
            }

            public SegmentWorkMode SegmentWorkMode
            {
                get;
                init;
            }

            #endregion
            public AnaChnlClkSrc ClockSrc
            {
                get;
                init;
            }

            public UInt32 BlankTime
            {
                get;
                init;
            }
            public AnaChnlItplType InterpolateType
            {
                get;
                init;
            } = AnaChnlItplType.Sinx;

            #region ZOOM
            public Double ZoomCenterX
            {
                get;
                init;
            }
            public Double ZoomCenterY
            {
                get;
                init;
            }
            public Double ZoomScaleX
            {
                get;
                init;
            }
            public Double ZoomScaleY
            {
                get;
                init;
            }
            #endregion

            #region ERes
            public Boolean EnhancedBitsActive
            {
                get;
                init;
            }

            public Double EnhancedBits
            {
                get;
                init;
            }
            #endregion
			
			public Boolean InterplotEnable
			{
				get;
				init;
			}

            #region 数据导出
            public ChannelId LongSorageWfmSource { get; init; }
			public Int64 LongStorageWfmCnt { get; init; }
            public String LongStorageFullFileName { get; init; }
            public Int32 LongStorageSaveEventCnt { get; init; }
            #endregion
        }

        public record DigitalOptions(Boolean Active, Int32 UserThroldIndex, Double UserThroldBymV, Double HystIndex, Double HystBymV);

        public record TriggerOptions(TriggerType TrigType, TriggerMode Mode)
        {
            public DelayOpt HoldoffType
            {
                get;
                init;
            }

            public Int64 HoldoffByps
            {
                get;
                init;
            }
            public Int32 HoldoffByCnt
            {
                get;
                init;
            }

            public Boolean EnableExtAtten
            {
                get;
                init;
            }

            public SysState TriggerStatus
            {
                get;
                init;
            }
            /// <summary>
            /// 区域触发配置
            /// </summary>
            public List<TriggerAreasOptions>? AreasTrigger { get; init; }

            public TrigEdgeOptions? Edge
            {
                get;
                init;
            }

            public TrigPulseOptions? Pulse
            {
                get;
                init;
            }

            public TrigPulseOptions? Glitch
            {
                get;
                init;
            }

            public TrigPulseOptions? Interval
            {
                get;
                init;
            }

            public TrigRuntOptions? Runt
            {
                get;
                init;
            }

            public TrigTransOptions? Transition
            {
                get;
                init;
            }

            public TrigWindowOptions? Window
            {
                get;
                init;
            }

            public TrigDelayOptions? Delay
            {
                get;
                init;
            }

            public TrigTimeOutOptions? TimeOut
            {
                get;
                init;
            }

            public TrigSustainTimeOptions? SustainTime
            {
                get;
                init;
            }

            public TrigNEdgeOptions? NEdge
            {
                get;
                init;
            }

            public TrigVideoOptions? Video
            {
                get;
                init;
            }

            public TrigPatOptions? Pattern
            {
                get;
                init;
            }

            public TrigStateOptions? State
            {
                get;
                init;
            }

            public TrigSetupHoldOptions? SetupHold
            {
                get;
                init;
            }

            public TrigDecoderOptions? TrigDecoder
            {
                get;
                init;
            }

            public TrigMultiQualifiedOptions? TrigMultiQualified
            {
                get;
                init;
            }
			public TrigMultiChnlOrOpions? TrigMultiChnlOr
			{
				get;
				init;
			}

        }

        /// <summary>
        /// 区域触发配置项
        /// </summary>
        /// <param name="Source1"></param>
        /// <param name="Source2"></param>
        public record TriggerAreasOptions(ChannelId Source1, VisualTriggerShape TriggerShape = VisualTriggerShape.Rectangle)
        {
            /// <summary>
            /// 是否重置
            /// </summary>
            public bool Reset { get; init; }

            /// <summary>
            /// 是否启用
            /// </summary>
            public bool Enabled { get; init; }

            /// <summary>
            /// Y轴最小值
            /// </summary>
            public float MinY { get; init; }

            /// <summary>
            /// Y轴最大值
            /// </summary>
            public float MaxY { get; init; }

            public float MinX { get; set; }

            public float MaxX { get; set; }
            /// <summary>
            /// 触发状态，区域内/区域外
            /// </summary>
            public VisualTriggerState TriggerState { get; init; }
        }

        public record TrigEdgeOptions(ChannelId Source, EdgeSlope Slope, Double PosIndex) : ISearchTypeOptions, ITriggerTypeOptions
        {
            public Double Position
            {
                get;
                init;
            }

            public TriggerCoupling Coupling
            {
                get;
                init;
            }

            public TriggerImpedance Impedance
            {
                get;
                init;
            }
            public Int32 SensitivityBymdiv
            {
                get;
                init;
            }

        }

        public record TrigPulseOptions(ChannelId Source, PulsePolarity Polarity, PulseCondition Condition, Int64 WidthByps, Int64 UpperWidthByps, Double PosIndex) : ISearchTypeOptions, ITriggerTypeOptions
        {
            public Double Position
            {
                get;
                init;
            }

        };

        public record TrigDelayOptions(ChannelId SourceOne, EdgeSlope SourceOneSlope, ChannelId SourceTwo, EdgeSlope SourceTwoSlope, PulseCondition Condition, Int64 LowerByps, Int64 UpperByps, Double UpperPosIndex, Double LowerPosIndex) : ISearchTypeOptions
        {
            public Double UpperPosition
            {
                get;
                init;
            }

            public Double LowerPosition
            {
                get;
                init;
            }

        }

        public record TrigTimeOutOptions(ChannelId Source, LevelPolarity Polarity, Int64 DurationByps, Double PosIndex) : ISearchTypeOptions, ITriggerTypeOptions
        {
            public Double Position
            {
                get;
                init;
            }

        };

        public record TrigSustainTimeOptions(PulseCondition Condition, Int64 WidthByps, Int64 UpperWidthByps, Int32 BitLength) : ISearchTypeOptions, ITriggerTypeOptions
        {
            public (SustainTimeLevelCondition Condition, Double Index, Double Value)[]? Positions
            {
                get;
                init;
            }

        };

        public record TrigNEdgeOptions(ChannelId Source, EdgeSlope Polarity, Int64 DurationByps, Int32 EdgeNumber, Double PosIndex) : ISearchTypeOptions, ITriggerTypeOptions
        {
            public Double Position
            {
                get;
                init;
            }

        };

        public record TrigRuntOptions(ChannelId Source, PulsePolarity Polarity, PulseCondition Condition, Int64 WidthByps, Int64 UpperWidthByps, Double UpperPosIndex, Double LowerPosIndex) : ISearchTypeOptions, ITriggerTypeOptions
        {
            public Double UpperPosition
            {
                get;
                init;
            }

            public Double LowerPosition
            {
                get;
                init;
            }

        }

        public record TrigTransOptions(ChannelId Source, EdgeSlope Slope, PulseCondition Condition, Int64 WidthByps, Int64 UpperWidthByps, Double UpperPosIndex, Double LowerPosIndex) : ISearchTypeOptions, ITriggerTypeOptions
        {
            public Double UpperPosition
            {
                get;
                init;
            }

            public Double LowerPosition
            {
                get;
                init;
            }
        }

        public record TrigWindowOptions(ChannelId Source, WindowRange Range, WindowTimeCondition Condition, Int64 WidthByps, Double UpperPosIndex, Double LowerPosIndex) : ISearchTypeOptions, ITriggerTypeOptions
        {
            public Double UpperPosition
            {
                get;
                init;
            }

            public Double LowerPosition
            {
                get;
                init;
            }
        }

        public record TrigVideoOptions(ChannelId Source, VideoStandard Standard, VideoPolarity Polarity, VideoSync Sync, Int16 Field, Int16 Line, Double PosIndex)
        {
            public Double Position
            {
                get;
                init;
            }

        }

        public record TrigPatOptions(PatOperator Operator, PatTimeCondition TimeCondition, Int64 DurationByps, Int64 UpperDurationByps, Int32 BitLength) : ISearchTypeOptions
        {
            public (PatLevelCondition Condition, Double Index, Double Value)[]? Positions
            {
                get;
                init;
            }

        }

        public record TrigStateOptions(PatOperator Operator, PatTimeCondition TimeCondition, Int64 DurationByps, Int32 BitLength)
        {
            public (PatLevelCondition Condition, Double Index, Double Value)[]? Positions
            {
                get;
                init;
            }

            public ChannelId ClkSource
            {
                get;
                init;
            }

            public PulsePolarity ClkPolarity
            {
                get;
                init;
            }

            public Boolean Conformed
            {
                get;
                init;
            }

        }

        public record TrigSetupHoldOptions(ChannelId ClkSource, EdgeSlope ClkPolarity, ChannelId DataSource, EdgeSlope DataPosPolarity, SetupHoldViolation Violation, Int64 TsuByps, Int64 ThdByps) : ISearchTypeOptions
        {
            public (Double Index, Double Value) ClkPosition
            {
                get;
                init;
            }

            public (Double Index, Double Value) DataUpperPosition
            {
                get;
                init;
            }

            public (Double Index, Double Value) DataLowerPosition
            {
                get;
                init;
            }

        }

		public record TrigMultiQualifiedOptions()
		{	
			public Int64 TimeRst
			{
				get;
                init;
			}
			public Boolean TimeRstEnable
			{
				get;
                init;
			}
            public Boolean StateRstEnable
            {
                get;
                init;
            }
            public Boolean StateRiseRstEnable
            {
                get;
                init;
            }
            public UInt32 ForceRstCnt
            {
                get;
                init;
            }
            public (String Name, ITriggerTypeOptions? TriggerOption, DelayOpt DelayType, Int32 Counts, Int64 DurationByps)?[] EventOptions
            {
                get;
                init;
            } = new (String Name, ITriggerTypeOptions? TriggerOption, DelayOpt DelayType, Int32 Counts, Int64 DurationByps)?[4];

        }
		public record TrigMultiChnlOrOpions() : ISearchTypeOptions
		{
			public Double ThresHold
			{
				get;
				init;
			}
			public (EdgeSlope Condtion, bool Enable)[]? Condition_Enable
			{
				get;
				init;
			}
		}

        public record ArbWfmGenOptions(Boolean Active, ArbWfmType WfmType, WfmGenImpedance Impedance, Int64 Frequency, Int32 Amplitude, Int32 Offset,
            Int32 Duty, Int32 Phase, TriggerSource WfmGenTrigerSource, WfmRampType rampType = WfmRampType.Rise)
        {

            public ChannelId GenChannelId
            {
                get;
                set;
            }

            public bool EnablePointByPoint
            {
                get;
                init;
            }


            public Int32 Noise
            {
                get;
                init;
            }

            public WfmGenMode Mode
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

            public Int32 PhaseBias
            {
                get;
                init;
            }

            public Int64 FreqBias
            {
                get;
                init;
            }

            public Int32 CustomImpedance
            {
                get;
                init;
            }

            public Int64 SweepFreqTime
            {
                get;
                init;
            }

            public Boolean SweepFreqActive
            {
                get;
                init;
            }

            public SweepType SweepFreqType
            {
                get;
                init;
            }

            public Int64 SweepFreqStartFreq
            {
                get;
                init;
            }

            public Int64 SweepFreqEndFreq
            {
                get;
                init;
            }

            public String FilePath
            {
                get;
                init;
            } = "";

            public String ModFilePath
            {
                get;
                init;
            } = "";

            public WfmModMethod ModMethod
            {
                get;
                init;
            }

            public ArbWfmType ModWfmType
            {
                get;
                init;
            }
            public List<int>? ArbWfmData
            {
                get;
                init;
            }
            public bool AuxIn
            {
                get;
                init;
            }
            public AwgTrigPolarity AuxInPolarity
            {
                get;
                init;
            }
            public bool AuxOut
            {
                get;
                init;
            }

            public AwgTrigPolarity AuxOutPolarity
            {
                get;
                init;
            }

            public Int64 PulseRiseTime
            {
                get;
                init;
            }
            public Int64 PulseFallTime
            {
                get;
                init;
            }
            public List<int>? ModArbWfmData
            {
                get;
                set;
            }

            public WfmRampType RampType
            {
                get;
                init;
            }

            /// <summary>
            /// 是否推送波形
            /// </summary>
            /// <Remark>作者：彭博 创建日期：2023/12/1 11:28:00 创建原因：添加只选择波形时才发生波形的功能 </Remark>
            public bool IsSendWaveType
            {
                get;
                init;
            } = false;

        }

        public record CymometerOptions(ChannelId? Source);


        public record RadioFrequencyOptions(Boolean Active)
        {
            public ChannelId Source
            {
                get;
                init;
            }
            public Double ReferenceLevel
            {
                get;
                init;
            }
            public Int64 CenterFrequency
            {
                get;
                init;
            }

            public Int64 Span
            {
                get;
                init;
            }
            public Double RBW
            {
                get;
                init;
            }
            public Int32 FFTLength
            {
                get;
                init;
            }

            public RFWindowType Window
            {
                get;
                init;
            }

            public Int64 STFTLength
            {
                get;
                init;
            }

            public Int64 STFTStep
            {
                get;
                init;
            }

            public Boolean TVFON
            {
                get;
                init;
            }

            public Boolean AVTON
            {
                get;
                init;
            }

            public Boolean PVFON
            {
                get;
                init;
            }

            public Boolean PVTON
            {
                get;
                init;
            }

            public bool RoughSpecON { get; init; }
        }

        public RadioFrequencyOptions[]? RadioFrequency
        {
            get;
            init;
        }

        public record SearchOptions(Boolean Active)
        {
            public Dictionary<Int64, (SearchType Type, ISearchTypeOptions Option)>? Searchs
            {
                get; init;
            }
        }
    }
    public record SystemOptions(Boolean AutoCfgFansSpeed, Dictionary<String, Int32> FansSpeed);
    public record PrecisionOptions
    {
        public Boolean AutoCfgBitWidthEnable { get; init; }

        public Int32 AnaChnlBitWidth { get; init; }
    }

    public record AiOptions()
    {
        public RecfgDbiRecord? RecfgDbi { get; init; }

        public TemplateTriggerRecord? TemplateTrigger { get; init; }

        public CaptureExceptionRecord? CaptureException { get; init; }

        public AIUnionRecord? AIUnion { get; init; }
    }

    public record RecfgDbiRecord(Boolean Enable)
    {
        public AutoFilterMode AutoFilterMode { get; init; }

        public UInt32 CriticalFreq { get; init; }

        public UInt32 SubbandEnable { get; init; }

        public SubbandCtrlMethod SubbandCtrlMethod { get; init; }

        public Boolean IterFilterEnable { get; init; }

        public Dictionary<Int32, UInt32> BaseNoise { get; init; } = new();

        public Dictionary<Int32, UInt64> LocalFreqByHz { get; init; } = new();

        public Dictionary<Int32, UInt64> BandFreqLimitByHz { get; init; } = new();

        public Dictionary<Int32, AntImageFreq> AntImageFreqByHz { get; init; } = new();
    }

    public record struct AntImageFreq(UInt64 LeftFreqByHz, UInt64 RightFreqByHz);

    public record TemplateTriggerRecord(Boolean Enable, TemplateSourceEnum SourceType, UInt32 Offset, Int32 UserDefinePosStart)
    {
        public Int32 SendTemplateCnt { get; init; }

        public UInt32 FrameIdForTrig { get; init; }

        public UInt32 FrameTrigDataLen { get; init; }
    }

    public record CaptureExceptionRecord(Boolean Enable)
    {
        public TemplateTriggerSourceEnum SourceType { get; init; }

        public Int32 FrameLength { get; init; }

        public Int32 SendTemplateCnt { get; init; }

        public Int32 Export2FileCnt { get; init; }

        public List<UInt32>? ImfData { get; init; }

        public List<UInt32>? ResData { get; init; }
    }

    public record AIUnionRecord()
    {
        public Boolean CaptureExceptionUnion { get; init; }
        public Boolean RecfgDbiUnion { get; init; }
        public Boolean AINoiseReductionEnable { get; init; }
        public Boolean AverageEnable { get; init; }

        public NoiseRedutionMethod CurNoiseRedutionMethod { get; init; }
    }


    public record MultiDomainRecord()
    {
        public Boolean Active { get; init; }

        public Boolean SynchronizationEnable { get; init; }

        public Boolean ParameterTuningEnable { get; init; }

        public UInt32 RoughSpecCnt { get; init; }

        public Boolean AutoSetEnable { get; init; }

        public ChannelId Source { get; init; }

        public RFWindowType WindowType { get; init; }

        public Int64 FFTLength { get; init; }

        public Int64 STFTLength { get; init; }

        public Int64 STFTStep { get; init; }

        public Int64 CenterFreqByHz { get; init; }

        public Int64 SpanByHz { get; init; }

        public Double SpanForTimeFreq { get; init; }

        public Double TimeScaleForTimeFreq { get; init; }

        public Boolean AVTON { get; init; }

        public Boolean FVTON { get; init; }

        public Boolean PVTON { get; init; }

        public Boolean SpecON { get; init; }

        public Double ZoomStart { get; init; }

        public Double ZoomLength { get; init; }

        public Double TimeStep { get; init; }
    }
    
}

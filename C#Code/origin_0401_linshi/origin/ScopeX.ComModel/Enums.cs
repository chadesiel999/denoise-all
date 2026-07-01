using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ScopeX.ComModel
{
    [Flags]
    public enum HdCmd : Int64
    {
        None = 0,

        ChnlPosition = 0x01,       //通道基线
        ChnlGain = 0x02,       //通道的增益及带宽
        ChnlInverted = 0x04,       //波形反相
        ChnlBias = 0x08,       //通道前级偏置
        ChnlScaleIndex = 0x10,
        ChnlActive = 0x20,
        ChnlBandwidth = 0x40,
        /// <summary>
        /// 通道延迟
        /// </summary>
        ChnlDelay = 0x80,

        //Channel = ChnlPosition | ChnlScaleIndex | ChnlInverted | ChnlBias | ChnlActive | ChnlBandwidth,

        TrigMode = 0x01_00,        //触发模式
        TrigTypeAndParameters = 0x02_00,          //触发类型选择
        TrigSource = 0x04_00,        //触发源选择
        TrigCoupling = 0x08_00,        //触发耦合
        TrigHoldoff = 0x10_00,        //触发抑制
        TrigPosition = 0x20_00,        //触发电压
        TrigSensitivity = 0x40_00,    //触发灵敏度
        /// <summary>
        /// 区域触发 0x8000
        /// </summary>
        TrigAreas = 1 << 15,
        //Trigger = TrigMode | TrigTypeAndParameters | TrigSource | TrigCoupling | TrigHoldoff | TrigPosition,

        TmbScaleIndex = 0x01_00_00,
        TmbPosition = 0x02_00_00,
        TmbStorageLen = 0x04_00_00,         //Storage Length
        TmbLongStorage = 0x08_00_00,
        /// <summary>
        /// 插值模式
        /// </summary>
        TmbInterpolateMode = 0x10_00_00,
        //Timebase = TmbScaleIndex | TmbPosition | TmbStorageLen,

        /// <summary>
        /// 交织模式变化  0x20_00_00
        /// </summary>
        InterleaveMode = 1 << 21, // 0x20_00_00

        DpxVectorized = 0x01_00_00_00,      //是否矢量显示
        DpxColorStep = 0x02_00_00_00,      //颜色累加步进
        DpxEnabled = 0x08_00_00_00,

        KeyLed = 0x00_01_00_00_00_00,

        DecodeParameters = 0x00_02_00_00_00_00,     //协议参数
        DecodeProtocal = 0x00_04_00_00_00_00,     //解码通道协议类型

        Digital = 0x00_08_00_00_00_00,

        SyncDelay = 0x00_10_00_00_00_00,
        DigTrigger = 0x00_20_00_00_00_00,
        SmpClock = 0x00_40_00_00_00_00,
        CaliDataChanged = 0x00_80_00_00_00_00,

        AWGConfig = 0x00_00_01_00_00_00_00_00,
        AWGTrigger1 = 0x00_00_10_00_00_00_00_00,
        AWGTrigger2 = 0x00_00_20_00_00_00_00_00,
        AWGTrigger3 = 0x00_00_40_00_00_00_00_00,
        AWGTrigger4 = 0x00_00_80_00_00_00_00_00,

        AWGData = 0x00_00_02_00_00_00_00_00,
        AWGCalibATT,
        AWGCalibDCDAC,
        AWGCalibAMP,
        AWGCalibFreq,
        AWGCalibActive,
        AWGCalibWorkMode,
        AWGCalibWfmType,
        AWGCalibWriteFile,
        CymometerSrc = 0x00_00_04_00_00_00_00_00,

        RadioFrequency = 0x00_00_08_00_00_00_00_00,

        Search = 0x01_00_00_00_00_00_00,        //搜索

        Run = 0x01_00_00_00_00_00_00_00,
        AdjustAnaChnlGain = 0x02_00_00_00_00_00_00_00,

        Combo = 0x10_00_00_00_00_00_00_00,
		
		//OuterPannelLEDCtrl = 0x40_00_00_00_00_00_00_00,

        ArtificialIntelligence = 0x08_00_00_00_00_00_00_00,

      

        //OuterPannelLEDCtrl = 0x04_00_00_00_00_00_00_00,
        SystemCtrl = 0x04_00_00_00_00_00_00_00,

        //DecodeDisabled = 0x08_00_00_00_00_00_00_00, // 禁用协议解码功能

        EnhancedBits = 0x20_00_00_00_00_00_00_00,//ERes

        //LASwitch = 0x40_00_00_00_00_00_00_00,//LA开关切换

        //SystemCtrl = 0x80_00_00_00_00_00_00_00,

        /// <summary>
        /// 方波端子开关
        /// </summary>
        SquareWaveSwitch = 0x04_00_00_00,//方波端子开关切换
        //All = Channel | Trigger | Timebase | KeyLed | DpxEnabled | DpxVectorized | RadioFrequency,
        ExceptionCapture = 1L << 63,//异常捕获
    }

    #region NetworkAdapter

    public enum NetworkAdapterType
    {
        EthernetWirelessUsing,//正在使用的网络
        All,//所有
        Ethernet,
        Wireless,
    }

    public enum GetIPMethod
    {
        Auto,
        Manual
    }

    #endregion NetworkAdapter

    public enum ChannelType
    {
        [Description("模拟")]
        Analog,
        [Description("逻辑")]
        Logic,
        [Description("逻辑")]
        LogicGroup,
        [Description("解码")]
        Decode,
        [Description("射频")]
        RadioFrequency,
        AmpVSTime,
        PhaseVSTime,
        PhaseVSFrequency,
        PhaseGroupDelay,
        TimeVSFrequency,
        FrequencyVSTime,
        [Description("数学")]
        Math,
        [Description("文件")]
        File,
        [Description("参数")]
        Parameter,
        [Description("信号源")]
        SigGen,
        [Description("电压表")]
        Voltmeter,
        [Description("频率计")]
        Cymometer,
		[Description("EMD")]
        EmdProcess,
        [Description("ReconfigDBI")]
        ReconfigDBI,
        [Description("TemplateTrigger")]
        TemplateTrigger,

    }

    public enum MeasureType
    {
        Single,//独立测量项
        Composite//复合测量运算项
    }

    public enum MeasureOperator
    {
        [Description("+")]
        Add,
        [Description("-")]
        Subtract,
        [Description("×")]
        Multiply,
        [Description("÷")]
        Division,
    }

    #region Analog Channel
    public enum AnaChnlCoupling
    {
        [Alias("DC1MΩ")]
        DC1M,
        [Alias("AC1MΩ")]
        AC1M,
        [Alias("DC50Ω")]
        DC50,
        [Alias("Ground")]
        Gnd
    }

    public enum AnaChnlProbe
    {
        [Description("1X")]
        [Alias("X1")]
        x1,
        [Description("5X")]
        [Alias("X5")]
        x5,
        [Description("10X")]
        [Alias("X10")]
        x10,
        [Description("100X")]
        [Alias("X100")]
        x100,
        [Description("自定义")]
        [Alias("Custom")]
        custom
    }
    public enum AnaChnlProbeBymVA
    {
        [Description("100mV/A")]
        x10,
        [Description("200mV/A")]
        x5,
        [Description("500mV/A")]
        x2,
        [Description("自定义")]
        custom
    }
    public enum AnaChnlImpedance
    {
        High1M,
        Low50
    }
    public enum ProbeKeyType
    {
        [Description("前灯")]
        Headlight = 0,

        [Description("运行/停止")]
        RunOrStop,

        [Description("清除显示")]
        Clear,

        [Description("强制触发")]
        ForceTrig,

        [Description("无操作")]
        NoOps
    }
    public enum ProbeUnitType
    {
        V,
        A,
        W,
        U
    }
    [Obsolete("Donot use this enum, it will be discarded.", true)]
    public enum AnaChnlBandwidth
    {
        Full,
        Bw500MHz,
        Bw20MHz,
    }

    public enum AnaChnlIpnutSource
    {
        BNC,
        SMA,
    }

    public enum AnaChnlType
    {
        ANA_2D5G,
        ANA_3G,
        ANA_5G,
        ANA_8G,
    }
    #endregion

    #region Trigger Channel
    public enum SysState
    {
        Stop = 0,
        Armed,
        Ready,
        Triged,
        Auto,
        Scan,
        Reset
    }

    public enum TriggerType
    {
        Edge,           //边沿触发
        PulseWidth,     //脉宽触发		    
        Video,          //视频触发
        Transition,     //斜率触发
        Runt,           //欠幅触发
        BeyondVol,      //超幅触发
        Delay,          //延迟触发
        TimeOut,        //超时触发，跌落触发
        SustainTime,    //持续时间触发
        SetupHold,      //建立保持触发
        NEdge,           //N边沿触发
        Pattern,        //码型触发
        Serial,         //串行触发
        State,          //状态触发
        Glitch,         //毛刺
        Window,         //窗口
        Interval,       //间隔
        MultiQulified,  //级联触发
		MultiChnlOr,    //多边沿或
    }

    public enum TriggerMode
    {
        OneShot,
        Normal,
        Auto,
    };

    //public enum TriggerSource
    //{
    //    //C1-C4 is corresponding to ChannelId.C1-C4;
    //    C1, C2, C3, C4,
    //    //D1-D32 is corresponding to ChannelId.D1-D32;
    //    D1 = 40, D2, D3, D4, D5, D6, D7, D8,
    //    D9, D10, D11, D12, D13, D14, D15, D16,
    //    //D17, D18, D19, D20, D21, D22, D23, D24,
    //    //D25, D26, D27, D28, D29, D30, D31, D32,
    //    Ext = 80, Ext5, AC,
    //}

    public enum TriggerCoupling
    {
        DC,
        AC,
        LFR,
        HFR,
        NR
    }

    public enum TriggerImpedance
    {
        High1M,
        Low50
    }

    public enum EdgeSlope
    {
        Rise,
        Fall,
        Both,
    }

    public enum PulsePolarity
    {
        Positive,
        Negative,
    }

    public enum PulseCondition
    {
        GreaterThan,
        LessThan,
        Equal,
        NotEqual,
    }
    public enum AwgTrigPolarity
    {
        Rise,
        Fall
    }
    public enum PatTimeCondition
    {
        //Begin, //LessThan 0
        //End,   //GreaterThan inf
        //Continue, //GreaterThan 
        //Inside,   
        //Outside,
        //NotEqual
        GreaterThan,
        LessThan,
        Inside,
        Outside
    }

    public enum CompareCondition
    {
        /// <summary>
        /// Defines the GreaterThan.
        /// </summary>
        [Alias(">")]
        GreaterThan,

        /// <summary>
        /// Defines the LessThan.
        /// </summary>
        [Alias("<")]
        LessThan,

        /// <summary>
        /// Defines the InRange.
        /// </summary>
        [Alias("[ ... ]")]
        InRange,

        /// <summary>
        /// Defines the OutRange.
        /// </summary>
        [Alias("] ... [")]
        OutRange,

        /// <summary>
        /// Defines the NotCare.
        /// </summary>
        [Alias("Any")]
        NotCare
    }

    //视频触发
    public enum VideoStandard
    {
        PAL,
        NTSC,
        EDTV480I60,
        EDTV576I50,
        HDTV480P60,
        HDTV576P50,
        HDTV720P60,
        HDTV1080P60,
        HDTV1080I60,
    }

    public enum VideoPolarity
    {
        Positive,
        Negative,
    }

    public enum VideoSync
    {
        Odd,
        Even,
        All,
        Specified,
    }

    public enum VideoMux
    {
        OneByOne,
        TwoByOne,
        FourByOne,
        EightByOne
    }

    public enum VideoFrameRate
    {
        FM25,
        FM30,
        FM50,
        FM60
    }

    public enum LevelPolarity
    {
        Positive,
        Negative,
        Any
    }

    public enum GlitchCondition
    {
        LessThan,
        Equal,
    }

    public enum PatOperator
    {
        And,
        Nand,
        Or,
        Nor
    }

    public enum PatLevelCondition
    {
        GreaterThan,
        LessThan,
        Any,
        Rise,
        Fall
    }

    public enum DelayOpt
    {
        Time,
        Event,
    }

    public enum WindowRange
    {
        Inside,
        Outside,
    }

    public enum WindowTimeCondition
    {
        OnEnter,
        GreaterThan,
        LessThan
    }

    public enum SustainTimeLevelCondition
    {
        GreaterThan,
        LessThan,
        Any
    }

    public enum SetupHoldViolation
    {
        Setup,
        Hold,
        SetupHold,
    }

    public enum VisualTriggerState
    {
        Overlap,
        NonOverlap,
    }
    public enum VisualTriggerShape
    {
        Rectangle,
        Triangle,
        Polygon
    }

    /// <summary>
    /// 视觉触发的区域关系枚举
    /// </summary>
    public enum VisualTriggerRelation
    {
        /// <summary>
        /// 与
        /// </summary>
        And,

        /// <summary>
        /// 或
        /// </summary>
        Or,
    }
    #endregion

    #region Digital Channel
    public enum DigiHeightOpt
    {
        Small,
        Medium,
        Large,
    }

    public enum DigiTholdFamily
    {
        TTL,
        [Description("5.0V CMOS")]
        CMOS5000,
        [Description("3.3V CMOS")]
        CMOS3300,
        [Description("2.5V CMOS")]
        CMOS2500,
        [Description("1.8V CMOS")]
        CMOS1800,
        ECL,
        PECL,
        LVDS,
        USER
    }
    #endregion

    #region Timebase

    public enum IgnoreScaleLimit
    {
        None = 0,
        Min = 1,
        Max = 2,
        Both = 3
    }

    public enum AnaChnlLengthOpt
    {
        [Alias("Auto")]
        Auto,
        [Alias("25kpts")]
        Of25KDots,
        [Alias("250kpts")]
        Of250KDots,
        [Alias("2.5Mpts")]
        Of2_5MDots,
        [Alias("25Mpts")]
        Of25MDots,
        [Alias("250Mpts")]
        Of250MDots,
        [Alias("Full")]
        Full
    }

    public enum AnaChnlStorageMode
    {
        Normal = 0,
        Long = 1,
        Fast = 2
    }

    public enum AnaChnlAcqMode
    {
        Normal,
        Peak,
        Average,
        HighRes,
        Envelope,
        //Sequence,
    }

    public enum AnaChnlClkSrc
    {
        Inner,
        Outter,
    }

    public enum AnaChnlItplType
    {
        Line,
        Sinx,
    }
    #endregion

    #region Segmentation
    public enum SegmentWorkMode
    {

        Single,
        Sequent,
        Select,
    }
    #endregion

    #region Cursor

    public enum CursorType
    {
        Horizontal,
        Vertical,
        HorizontalVertical,
        XY,
    };

    public enum CursorPosFormat
    {
        Axis,
        InvAxis,
        Degree,
        Percent,
        Custom,
    };

    public enum CursorMoveMode
    {
        [Description("细调")]
        Fine = 0,
        [Description("粗调")]
        Fast
    }

    public enum CursorPositionMode
    {
        [Description("绝对值")]
        Absolute = 0,
        [Description("相对值")]
        Relative
    }

    #endregion Cursor

    #region Measure
    public enum MeasureTopBaseRef
    {
        BaseTop,
        MinMax,
        ZeroMin,
        ZeroMax
    }

    public enum MeasureTopBaseRefUnit
    {
        Percent,
        Absolute,
    }

    public enum MeasureGate
    {
        Screen,
        Cursor
    }

    public enum MeasureExtCalc
    {
        [Alias("+")]
        Add,
        [Alias("-")]
        Sub,
        [Alias("*")]
        Mul,
        [Alias("/")]
        Div,
        [Alias("Abs")]
        Abs,
    }
    #endregion

    #region Waveform Generator
    public enum TriggerSource
    {
        [Description("NeiBu")]
        Inside,
        [Description("ShouDong")]
        Manual,
        [Description("WaiBu")]
        Outside
    }

    public enum SweepType
    {
        [Description("XianXing")]
        Linear,
        [Description("DuiShu")]
        Logarithmic
    }

    public enum ArbWfmType
    {
        [Description("正弦波")]
        Sinusoid,
        [Description("方波")]
        Square,
        [Description("脉冲波")]
        Pulse,
        [Description("斜波")]
        Ramp,
        //<Remark>更改人：彭博 创建日期：2023/11/24 16:44:00  原因：技术手册改动，将锯齿波和三角波合为斜波 </Remark>
        //[Description("三角波")]
        //Triangular,
        //[Description("锯齿波")]
        //Ramp,
        [Description("噪声")]
        Noise,
        [Description("直流")]
        DC,
        [Description("Sinc")]
        Sinc,
        [Description("指数上升")]
        ExpRise,
        [Description("指数下降")]
        ExpFall,
        [Description("洛伦兹")]
        Lorentz,
        [Description("半正矢")]
        Haversine,
        [Description("高斯")]
        Gaussian,
        [Description("心电图")]
        ECG,
        [Description("任意波")]
        Arbitrary,
    }

    public enum WfmGenMode
    {
        [Description("连续波")]
        Continuous,
        [Description("调制")]
        Modulation,
        [Description("扫频")]
        Sweep,
    }

    public enum WfmModMethod
    {
        FM,
        AM,
        PM,
        FSK
    }

    public enum WfmGenImpedance
    {
        HighZ,
        Low50,
        Custom
    }

    public enum WfmRampType
    {
        Rise,
        Fall,
    }
    public enum WfmGenCalStatus
    {
        OFF,
        Cal_DC,
        Cal_AC,
        Cal_DC_Verify,
        Cal_AC_Verify
    }
    #endregion

    #region Voltmeter
    public enum VoltmeterMode
    {
        DC,
        ACrms,
        DCACrms,
    }
    #endregion

    #region Pass Fail Test
    public enum PFTestMode
    {
        LimitMode,
        StdMaskMode
    };

    public enum PFTestStopOpt
    {
        TestWfms,
        TestTime
    };

    public enum PFTestState
    {
        Pass,
        Fail
    };

    public enum PFStdMaskType
    {
        ANSI_T1_102,
        ITU_T,
        USB,
    };

    #endregion

    #region Frequency Channel
    public enum RFDataType
    {
        Raw,//对应频谱的通过频域通道传输的时域数据
        IQ,//先存储I（前一半），再存储Q（后一半） 
        Frequency,//频率数据
        AmpVSTime,//幅度域数据
        PhaseVSTime,//相位域数据
    }

    public enum AmplitudeUnitType
    {
        // Vrms,
        Linear,
        Logarithm
    }

    public enum LogarithmUnit
    {
        //Vrms,
        dBm,
        dBmV,
        dBmA,
        dBμW,
        dBμV,
        dBμA
    }

    public enum RFWaveType
    {
        Normal,
        Average,
        MaxHold,
        MinHold,
    }

    public enum MDVirticalType
    {
        Phase,
        Amplitude,
        Time,
        Frequency,
    }

    public enum PickMode
    {
        Sample,
        Average,
        Maxmum,
        Minmum,
    }

    public enum RFWindowType
    {
        Rectangle,
        Hann,
        Hamming,
        Blackman,
        Flattop,
        Kaiser,
        Gaussian
    }

    public enum MarkerReadMode
    {
        Absolute = 0,//绝对值
        Delta = 1//增量
    }
    public enum SortOption
    {
        Frequency = 0,
        Amplitude = 1
    }

    public enum PhaseUnitType
    {
        Degree,
        Radian,
        GroupDelay
    }
    public enum OBWAnalysisType
    {
        Percentage,
        dBDown,
    }

    public enum FrequencyMeasureType
    {
        ACPR,
        OB,
        CP,
        THD,
    }
    #endregion

    #region Draw
    public enum RenderingMode
    {
        Default,
        CPU,
        GPU,
    }
    public enum DrawMethod
    {
        Plot,
        Bar,
        Stair,
        XYDots,
        DPX,
        XYLines,
    }


    public enum Clipping
    {
        None,
        Pos,
        Neg,
        Both
    }

    public enum PlotRenderType
    {
        Angle,
        Stack,
        Superimpose,
        Spliced,
        None,
    }

    public enum AxisType
    {
        Linear = 0,
        Log = 1
    }

    #endregion Draw

    #region Search
    public enum SearchType
    {
        Edge,
        Pulse,
        Timeout,
        Runt,
        Window,
        Transition,
        Pattern,//Pattern,
        SetupHold,
        Auto,
    }

    #endregion Search

    #region VSA&SDA
    public enum MaxBinNum
    {
        [Description("25")]
        Num25,
        [Description("50")]
        Num50,
        [Description("100")]
        Num100,
        [Description("250")]
        Num250,
        [Description("500")]
        Num500,
        [Description("2000")]
        Num2000,
        [Description("MaxNum")]
        Max,
    }

    public enum MultiWfmsLayout
    {
        Overlay,
        Adjacent,
        Waterfall,
        Perspective,
        Mosaic,
    }

    public enum TholdTypeOpt
    {
        Percent,
        Absolute,
    }
    public enum ClockTypeOpt
    {
        Constant,
        PLL,
        External,
    }

    public enum PllTypeOpt
    {
        [Alias("FC Golden")]
        Golden,
        [Alias("First Order")]
        FirstOrder,
        [Alias("Second Order")]
        SecondOrder,
        [Alias("PCI-Express Gen1")]
        PCIExpressGen1,
        [Alias("PCI-Express G2 A 3dBpk 16MHz fc")]
        PCIExpressGen2A,
        [Alias("PCI-Express G2 B 3dBpk 8MHz fc")]
        PCIExpressGen2B,
        [Alias("PCI-Express G2 C 1dBpk 5MHz fc")]
        PCIExpressGen2C,
        [Alias("DVI")]
        DVI,
        [Alias("USB3.0 SS")]
        USB,
        [Alias("FB-DIMM")]
        FBDIMM
    }

    public enum JitterSignalType
    {
        [Alias("数据信号")]
        Custom,
        [Alias("时钟信号")]
        Clock,
    }



    public enum JitterDualDiracModel
    {
        SpectralRjDirect,
        SepctralRjDjCDFFit,
        NQScale
    }

    public enum VsaSignalType
    {
        GeneralDigtal,
        Bluetooth,
        IEEE802_11ad,
        LTE,
        WLAN,
        NR_5G,
        OFDM
    }

    public enum SymbolSynUseScne
    {
        /// <summary>
        /// 恒模数字调制格式的定时同步，例如QPSK,BPSK,8PSK
        /// </summary>
        OneModuleSymbolSyn,

        /// <summary>
        /// 多模调制格式定时同步，例如MQAM
        /// </summary>
        MultiModuleSymbolSyn,

    };
    public enum VsaPllUseScne
    {
        /// <summary>
        /// MPSK等定时同步
        /// </summary>
        MPSKsymbolsyn = 0,

        /// <summary>
        /// MQAM等定时同步
        /// </summary>
        MQAMsymbolsyn = 1,

        /// <summary>
        /// 用于载波同步
        /// </summary>
        carriersyn = 2
    };



    public enum VsaGraphType
    {
        /// <summary>
        /// I路时域图
        /// </summary>
        ITime,

        /// <summary>
        /// Q路时域图
        /// </summary>
        QTime,

        /// <summary>
        /// 星座图
        /// </summary>
        Constellation,

        /// <summary>
        /// I路眼图
        /// </summary>
        IEye,

        /// <summary>
        /// Q路眼图
        /// </summary>
        QEye,

        /// <summary>
        /// 矢量图
        /// </summary>
        Vector,

        /// <summary>
        /// EVM
        /// </summary>
        EVM,

        /// <summary>
        /// 相位误差图
        /// </summary>
        PhaseDeviation,

        /// <summary>
        /// 幅度误差图
        /// </summary>
        AmplitudeDeviation,
    }

    public enum VsaFormatOpt
    {
        QPSK,
        PSK8,
        D8PSK,
        D16PSK,
        PI2DBPSK,
        DQPSK,
        PI4DQPSK,
        BPSK,
        OQPSK,
        QAM16,
        QAM32,
        QAM64,
        QAM128,
        QAM256,
        QAM1024,
        MSK,
        FSK2,
        FSK4,
        FSK8,
        FSK16,
        CPM,
        SOQPSK,
        SBPSK,
        C4FM,
        APSK16,
        APSK32,
    }

    public enum VsaTemplateOpt
    {
        RF,
        IQ,
        Custom
    }

    public enum VsaTimingEstOpt
    {
        Square,
        Stddev,
        EVM,
        CrossPoints
    }

    public enum VsaMeasureFilterTypeOpt
    {
        NoFilter,
        RootRaisedCosine,
        RaisedCosine,
        Gaussian,
        UserCustomization,
        Rectangular,
        IS95TXMEA,
        IS95TXEQMEA,
    }

    public enum VsaRefFilterTypeOpt
    {
        NoFilter,
        RaisedCosine,
        Gaussian,
        UserCustomization,
        Rectangular,
        IS95REF,
    }

    public enum VsaPhaseEstOpt
    {
        Viterbi,
        FeedForward,
        ImprovedViterbi
    }

    public enum VsaItplOpt
    {
        Linear,
        Sinc
    }

    public enum VsaEqualizeMode
    {
        Tranning,
        Hold,
    }

    public enum VsaTimeSyncType
    {
        Gardner,
        OM,
    }

    public enum VsaCarrySyncType
    {
        DD,
        PFD_DD,
    }

    public enum JitterGraphType
    {
        [Description(Constants.JITTER_TREND_FORMULA)]
        Trend = 0,
        [Description(Constants.JITTER_SPECTRUM_FORMULA)]
        Spectrum = 1,
        [Description(Constants.JITTER_HISTOGRAM_FORMULA)]
        Histogram = 2,      
        [Description(Constants.JITTER_BATHTUB_FORMULA)]
        Bathtub = 3,
        [Description(Constants.JITTER_QFACTOR_FORMULA)]
        QFactor = 4,
        [Description(Constants.JITTER_EYE_FORMULA)]
        Eye = 5,
    }

    public enum JitterInterpolationType
    {
        Linear,
        CubicSpline,
        Sinc
    }


    #endregion

    #region Probe
    public enum ProbeBtnType
    {
        [Description("前灯")]
        Headlight = 0,
        [Description("InfiniiMode")]
        InfiniiMode,
        [Description("运行/停止")]
        RunOrStop,
        [Description("单")]
        Single,
        [Description("清除显示")]
        Clear,
        [Description("自动定标")]
        AutoScale,
        [Description("强制触发")]
        ForceTrig,
        [Description("自定义快捷键")]
        KeyboardShortcuts,
        [Description("无操作")]
        NoOps
    }
    public enum AttenuationType
    {
        [Description("分贝")]
        Decibel = 0,
        [Description("比率")]
        Rate,
    }
    #endregion
    #region Display
    public enum WfmDrawMode
    {
        Vector = 0,
        Dot = 1
    }

    public enum WfmPersist
    {
        Close,
        Auto,
        Infinity
    }
    #endregion

    #region Update

    public enum UpdateProcessState
    {
        None,
        Updating,
        HaveError,
        Done
    }

    #endregion

    #region OuterPannelLED
    public enum OuterPannelLEDType
    {
        LAN,
        ACQ,
        RUN,
        POWER,
        CH1_50,
        CH1_1M,
        CH2_50,
        CH2_1M,
        CH3_50,
        CH3_1M,
        CH4_50,
        CH4_1M,
    }
    #endregion
    #region ErrorType
    public enum ErrorType
    {
        /// <summary>
        /// 无错误（用于清除错误码）
        /// </summary>
        [Description("Z0000")]
        None,
        /// <summary>
        /// 未知错误
        /// </summary>
        [Description("Z0001")]
        Nnknown,
        /// <summary>
        /// 键盘板未找到(S0001)
        /// </summary>
        [Description("S0001")]
        S_Keyboard_NotFound_0001,
        /// <summary>
        /// SCPIManager.Start调用异常(S0001)
        /// </summary>
        [Description("S0002")]
        S_SCPIStart_Error_0002,
        /// <summary>
        /// 启动时，Driver对象创建失败为NULL
        /// </summary>
        [Description("S0003")]
        S_Driver_Is_Null_0003,

        /// <summary>
        /// 启动时，PCIE未找到
        /// </summary>
        [Description("S0004")]
        S_PCIE_NotFound_0004,
        /// <summary>
        /// 启动时，PCIE打开失败
        /// </summary>
        [Description("S0005")]
        S_PCIE_Open_Error_0005,

        /// <summary>
        /// 启动时Hd.bPowerOff为False
        /// </summary>
        [Description("S0006")]
        S_PowerOff_0006,

        /// <summary>
        /// ADC扫窗回读失败，可能是寄存器回读异常！
        /// </summary>
        [Description("S0007")]
        S_ADC_0007,
        /// <summary>
        /// ADC扫窗回读失败，采集板掉电
        /// </summary>
        [Description("S00071")]
        S_ACQ_PowerOff_00071,
        /// <summary>
        /// ADC扫窗TAP值验证失败，最大最小值差异过大！
        /// </summary>
        [Description("S00072")]
        S_ADC_Tap_Max_Min_00072,
        /// <summary>
        /// ADC扫窗TAP值验证失败，TAP值全为0！
        /// </summary>
        [Description("S00073")]
        S_ADC_Tap_Zero_00073,

        /// <summary>
        /// VersionInfo.xml加载异常
        /// </summary>
        [Description("S0008")]
        S_VersionInfo_Load_Error_0008,
        /// <summary>
        /// 版本检查捕获异常
        /// </summary>
        [Description("S0009")]
        S_Version_Try_Error_0009,

        /// <summary>
        /// VersionInfo.xml内容加载异常
        /// </summary>
        [Description("S0010")]
        S_VersionInfo_Info_Error_0010,

        /// <summary>
        /// 版本过低
        /// </summary>
        [Description("S0011")]
        S_VersionInfo_Lower_0011,

        /// <summary>
        /// 主7044配置异常
        /// </summary>
        [Description("S0012")]
        S_Config_Master7044_0012,

        /// <summary>
        /// 子7044配置异常
        /// </summary>
        [Description("S0013")]
        S_Config_Sub7044_0013,

        /// <summary>
        /// 2595配置异常
        /// </summary>
        [Description("S0014")]
        S_Config_2595_0014,

        /// <summary>
        /// 配置5200和204B异常
        /// </summary>
        [Description("S0015")]
        S_Config_5200And204B_0015,

        /// <summary>
        /// 按键板串口打开异常
        /// </summary>
        [Description("K0001")]
        K_Open_Error_0001,

        /// <summary>
        /// 按键板串口读取超时
        /// </summary>
        [Description("K0002")]
        K_Read_Timeout_0002,

        /// <summary>
        /// 按键板串口通信异常
        /// </summary>
        [Description("K0003")]
        K_Exception_0003,

        /// <summary>
        /// 按键板初始化失败
        /// </summary>
        [Description("K0004")]
        K_Initialization_Failure_0004,

        /// <summary>
        /// 按键板数据解析异常
        /// </summary>
        [Description("K0005")]
        K_Analytic_Exception_0005,

        /// <summary>
        /// 固件升级成功
        /// </summary>
        [Description("F0001")]
        F_FireWare_Update_Success_0001,

        /// <summary>
        /// 固件升级失败
        /// </summary>
        [Description("F0002")]
        F_FireWare_Update_Failed_0002,

        /// <summary>
        /// 固件升级中（用于状态灯闪烁）
        /// </summary>
        [Description("F0003")]
        F_FireWare_Updating_0003,

        /// <summary>
        /// 固件升级中（用于状态灯闪烁）
        /// </summary>
        [Description("F0004")]
        F_FireWare_Updating_0004,
    }
    #endregion

    #region Option  此枚举如有修改 请及时告知负责人，原则上不允许修改

    public enum OptionType
    {
        [OptionDescription("MD2G", "将示波器最大存储深度扩展至1Gpts/CH（全通道）2Gpts/CH（半通道）")]
        [Display("MD2G")]
        [ProductTypes("JiHe_MSO8000X")]
        MD2G,

        [OptionDescription("BW-10T20", "1GHz升级2GHz带宽，MSO7104X升级到MSO7204X", "1GHz升级2GHz带宽，UPO7104L升级到UPO7204L")]
        [Display("BW-10T20")]
        [ProductTypes("JiHe_MSO7000X", "JiHe_UPO7000L")]
        BW10T20,

        [OptionDescription("AWG", "双通道60 MHz任意波发生器", "60MHz任意波发生器")]
        [Display("AWG")]
        [ProductTypes("JiHe_MSO7000X", "JiHe_UPO7000L", "JiHe_MSO8000X")]
        AWG,

        [OptionDescription("LA", "16通道数字探头（UT-M15）及分析")]
        [Display("LA")]
        [ProductTypes("JiHe_MSO7000X", "JiHe_MSO8000X")]
        LA,

        [OptionDescription("JITTER", "高级抖动和眼图分析")]
        [Display("JITTER")]
        [ProductTypes("JiHe_MSO7000X", "JiHe_UPO7000L", "JiHe_MSO8000X")]
        Jitter,

        [OptionDescription("PWR", "电源分析")]
        [Display("PWR")]
        [ProductTypes("JiHe_MSO7000X", "JiHe_UPO7000L", "JiHe_MSO8000X")]
        Pwr,

        [OptionDescription("CANFD", "汽车串行触发和解码（CAN-FD）")]
        [Display("CAN-FD")]
        [ProductTypes("JiHe_MSO7000X", "JiHe_UPO7000L", "JiHe_MSO8000X")]
        Decode_CanFD,

        [OptionDescription("FLEX", "汽车串行触发和解码（FlexRay）")]
        [Display("FLEX")]
        [ProductTypes("JiHe_MSO7000X", "JiHe_UPO7000L", "JiHe_MSO8000X")]
        Decode_FlexRay,

        [OptionDescription("SENT", "汽车传感器串行触发和解码（SENT）")]
        [Display("SENT")]
        [ProductTypes("JiHe_MSO7000X", "JiHe_UPO7000L", "JiHe_MSO8000X")]
        Decode_SENT,

        [OptionDescription("AUDIO", "音频串行触发和解码（I2S、LJ、RJ、TDM）")]
        [Display("AUDIO")]
        [ProductTypes("JiHe_MSO7000X", "JiHe_UPO7000L", "JiHe_MSO8000X")]
        Decode_AudioBus,

        [OptionDescription("AERO", "航空航天串行触发和解码（MIL-STD-1553，ARINC 429）")]
        [Display("AERO")]
        [ProductTypes("JiHe_MSO7000X", "JiHe_UPO7000L", "JiHe_MSO8000X")]
        Decode_AERO,

        [OptionDescription("SMBUS", "嵌入式串行总线触发和分析选件（SMBus）")]
        [Display("SMBUS")]
        [ProductTypes("JiHe_MSO8000X")]
        Decaode_SMBUS,

        [OptionDescription("SPMI", "电源管理串行总线触发和分析选件（SPMI）")]
        [Display("SPMI")]
        [ProductTypes("JiHe_MSO8000X")]
        Decaode_SPMI,

        [OptionDescription("I3C", "MIPI-I3C总线触发和分析选件（I3C）")]
        [Display("I3C")]
        [ProductTypes("JiHe_MSO8000X")]
        Decaode_I3C,

        [OptionDescription("PSI5", "汽车串行总线分析选件（PSI5）")]
        [Display("PSI5")]
        [ProductTypes("JiHe_MSO8000X")]
        Decaode_PSI5,

        [OptionDescription("USB2", "USB总线触发和分析选件（USB2.0）")]
        [Display("USB2")]
        [ProductTypes("JiHe_MSO8000X")]
        Decaode_USB2,

        [OptionDescription("PCIe2", "PCIe总线触发和分析选件（PCIe1.0，2.0）")]
        [Display("PCIe2")]
        [ProductTypes("JiHe_MSO8000X")]
        Decaode_PCIe2,

        [OptionDescription("NET", "以太网总线分析选件（10BASE-T、100BASE-TX）")]
        [Display("NET")]
        [ProductTypes("JiHe_MSO8000X")]
        Decaode_NET,

        [OptionDescription("NRZ", "NRZ信号分析选件（NRZ）")]
        [Display("NRZ")]
        [ProductTypes("JiHe_MSO8000X")]
        Decaode_NRZ,

        [OptionDescription("MANCH", "曼彻斯特信号分析选件（Manchester）")]
        [Display("MANCH")]
        [ProductTypes("JiHe_MSO8000X")]
        Decaode_MANCH,

        [OptionDescription("8B10B", "8b/10b信号分析选件（8B/10B）")]
        [Display("8B10B")]
        [ProductTypes("JiHe_MSO8000X")]
        Decaode_8B10B,

        [OptionDescription("FILTER", "高级滤波设计器选件 ")]
        [Display("FILTER")]
        [ProductTypes("JiHe_MSO8000X")]
        FILTER,

        [OptionDescription("MAT", "Matlab嵌入式编程选件，允许用户创建Matlab代码以自定义数学函数")]
        [Display("MAT")]
        [ProductTypes("JiHe_MSO8000X")]
        MAT,

        [OptionDescription("BND", "升级套装（JITTER，PWR，CAN-FD，FLEX，SENT，AUDIO，AERO）")]
        [Display("BND")]
        [ProductTypes("JiHe_MSO7000X", "JiHe_UPO7000L", "JiHe_MSO8000X")]
        BND,

        //[OptionDescription("RM", "机架安装套件")]
        //RM,
    }

    #endregion Option  此枚举如有修改 请及时告知负责人，原则上不允许修改

    #region Lissajous

    public enum LissajousCursorType
    {
        [Description("光标")]
        Cursor,
        [Description("波形")]
        Wave
    }

    public enum LissajousDataType
    {
        [Description("矩形")]
        Rectangle,
        [Description("极性")]
        Polar
    }

    #endregion
	
	
	
	   #region MultiDomain
    public enum MultiDomainFigureEnum
    {
        [Alias("幅度对时间")]
        AmpVsTime,

        [Alias("相位对时间")]
        PhaseVsTime,

        [Alias("相位对频率")]
        PhaseVsFreq,

        [Alias("幅度对频率")]
        AmpleVsFreq,

        [Alias("频率对时间")]
        FreqVsTime,

        /// <summary>
        /// 时间对频率，不断累积的幅频曲线压缩成的DPX效果
        /// </summary>
        [Alias("瀑布图")]
        Waterfalls,

        /// <summary>
        /// 
        /// </summary>
        [Alias("时频图")]
        Spectrogram,
    }
    #endregion

    #region AI
    public enum AiModeEnum
    {
        [Alias("关闭")]
        Manual,

        [Alias("自动")]
        Auto,

        [Alias("询问")]
        Query,

        [Alias("学习")]
        Train,
    }

    public enum TemplateTriggerSourceEnum
    {
        [Alias("原始数据")]
        Origin,

        [Alias("算法分解")]
        Inner,

        [Alias("文件")]
        File,
    }

    public enum TemplateSourceEnum
    {
        /// <summary>
        /// FPGA算法捕获的异常信号
        /// </summary>
        Outside,

        /// <summary>
        /// 根据输入信号，使用算法训练出来的信号
        /// </summary>
        Inner,

        /// <summary>
        /// 用户自己在屏幕上选择的信号
        /// </summary>
        UserDefine,
    }

    public enum ExceptionViewMode
    {
        [Alias("关闭")]
        None,

        [Alias("单帧")]
        Single,

        [Alias("所有帧")]
        All,
    }

    public enum AutoFilterMode
    { 
        Closed,
        Open,
        CalcNoise,
    }

    public enum NoiseRedutionMethod
    {
        [Alias("关闭")]
        Close,

        [Alias("神经网络")]
        NeuralNetwork,

        [Alias("加权平均")]
        Average,

        [Alias("频域滤波")]
        FreqDomainFilter,

        [Alias("时域滤波")]
        TimeDomainFilter,

        [Alias("自适应滤波")]
        AdaptiveFilter,
    }

    public enum SubbandCtrlMethod
    {
        [Alias("用户设置")]
        UserManual,

        [Alias("子带自适应")]
        SubbandAdaptive,

        [Alias("TS自适应")]
        BitWidthAdaptive,
    }
    #endregion
	
	

    //    /// <summary>
    /// 支持的语言
    /// </summary>
    public enum Language
    {
        简体中文 = 0,
        English,
        German,
        French,
        Spanish,
        Italian
        //Japanese,
    }

    public enum Edge
    {
        None,
        Falling,
        Rise
    };

    public class ProductTypesAttribute : Attribute
    {
        public ProductTypesAttribute(params String[] productTypes)
        {
            ProductTypes = productTypes;
        }
        public String[] ProductTypes { get; }
    }

    public class OptionDescriptionAttribute : Attribute
    {
        public OptionDescriptionAttribute(String ModelName, String Description, Object? Custom = null)
        {
            this.ModelName = ModelName;
            this.Description = Description;
            this.Custom = Custom;
        }
        public String ModelName { get; set; }
        public String Description { get; set; }
        public Object? Custom { get; set; }
    }

    public static class EnumEx
    {
        public static T Clamp<T>(this T value) where T : struct, Enum
        {
            var enumvalus = Enum.GetValues<T>();
            return enumvalus.Contains(value) ? value : enumvalus.First();
        }
        public static String GetAlias(this Enum value)
        {
            FieldInfo? fi = value.GetType().GetField(value.ToString());
            var attrs = fi?.GetCustomAttributes(typeof(AliasAttribute), false) as AliasAttribute[];
            return attrs?.Length > 0 ? attrs[0].Alias : value.ToString();
        }

        public static String[] GetProductTypes(this Enum value)
        {
            FieldInfo? fi = value.GetType().GetField(value.ToString());
            var attr = fi?.GetCustomAttribute(typeof(ProductTypesAttribute), false) as ProductTypesAttribute;
            return attr != null ? attr.ProductTypes : value.ToString().Split(',');
        }

        public static String GetDisplay(this Enum value)
        {
            FieldInfo? fi = value.GetType().GetField(value.ToString());
            var attr = fi?.GetCustomAttribute(typeof(DisplayAttribute), false) as DisplayAttribute;
            return attr != null ? attr.Diplay : value.ToString();
        }

        public static String GetDescription(this Enum value)
        {
            FieldInfo? fi = value.GetType().GetField(value.ToString());
            var attrs = fi?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attrs?.Length > 0 ? attrs[0].Description : value.ToString();
        }

        public static String GetClassDescription(this Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            return attrs?.Length > 0 ? attrs[0].Description : type.ToString();
        }

        public static (String ModelName, String Description) GetOptionDescription(this Enum value)
        {
            FieldInfo? fi = value.GetType().GetField(value.ToString());
            var attrs = fi?.GetCustomAttributes(typeof(OptionDescriptionAttribute), false) as OptionDescriptionAttribute[];
            return attrs?.Length > 0 ? (attrs[0].ModelName, attrs[0].Description) : ((string ModelName, string Description))(value.ToString(), value.ToString());
        }

        public static (String ModelName, String Description, Object? Custom) GetOptionAllDescription(this Enum value)
        {
            FieldInfo? fi = value.GetType().GetField(value.ToString());
            var attrs = fi?.GetCustomAttributes(typeof(OptionDescriptionAttribute), false) as OptionDescriptionAttribute[];
            return attrs?.Length > 0
                ? (attrs[0].ModelName, attrs[0].Description, attrs[0].Custom)
                : ((string ModelName, string Description, object? Custom))(value.ToString(), value.ToString(), value.ToString());
        }
    }
}

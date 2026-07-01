// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/14</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;
    using ScopeX.MathExt;
    using static ScopeX.Core.DataSrcMath;

    /// <summary>
    /// Defines the MathType.
    /// </summary>
    public enum MathType
    {
        /// <summary>
        /// Defines the Binary.
        /// </summary>
        Binary,

        /// <summary>
        /// Defines the FFT.
        /// </summary>
        FFT,

        /// <summary>
        /// Defines the Zoom.
        /// </summary>
        Zoom,

        /// <summary>
        /// Defines the Filter.
        /// </summary>
        Filter,

        /// <summary>
        /// Defines the ERes.
        /// </summary>
        ERes,

        /// <summary>
        /// Defines the Histgram.
        /// </summary>
        Histgram,

        /// <summary>
        /// Defines the Track.
        /// </summary>
        Track,

        /// <summary>
        /// Defines the Trend.
        /// </summary>
        Trend,

        /// <summary>
        /// Defines the Custom.
        /// </summary>
        Custom,

        /// <summary>
        /// 三方引擎库.
        /// </summary>
        UserProgram,
    }

    public enum RunStateType
    {
        Single,
        Repeat,
        Stop,
    }

    /// <summary>
    /// Defines the MathBinaryType.
    /// </summary>
    public enum MathBinaryType
    {
        [Alias("+")]
        Add,

        [Alias("-")]
        Subtract,

        [Alias("×")]
        Multiply,

        [Alias("÷")]
        Divide,
    }

    /// <summary>
    /// Defines the FFTNumber.
    /// </summary>
    public enum FFTNumber
    {
        /// <summary>
        /// Defines the Num1K.
        /// </summary>
        [Description("1KPts")]
        Num1K = 1024,

        /// <summary>
        /// Defines the Num2K.
        /// </summary>
        [Description("2KPts")]
        Num2K = 2048,

        /// <summary>
        /// Defines the Num4K.
        /// </summary>
        [Description("4KPts")]
        Num4K = 4096,

        /// <summary>
        /// Defines the Num8K.
        /// </summary>
        [Description("8KPts")]
        Num8K = 8192,

        /// <summary>
        /// Defines the Num8K.
        /// </summary>
        [Description("16KPts")]
        Num16K = 16384,

        [Description("32KPts")]
        Num32K = 32768,

        [Description("64KPts")]
        Num64K = 65536,

        [Description("128KPts")]
        Num128K = 131072,

        [Description("256KPts")]
        Num256K = 262144,

        [Description("512KPts")]
        Num512K = 524288,

        //[Description("1MPts")]
        //Num1M = 1048576,

        //[Description("2MPts")]
        //Num2M = 2097152,

        //[Description("4MPts")]
        //Num4M = 4194304,

        //[Description("8MPts")]
        //Num8M = 8388608,
    }
    public enum UserProgramType
    {
        Matlab,
        JavaScript,
        VbScript,
        CPlusPlus,
        Excel,
        Close
    }

    /// <summary>
    /// Defines the <see cref="MathModel" />.
    /// </summary>
    internal class MathModel : ChannelModel
    {
        internal Func<MathType, MathArgPrsnt>? GetOrMakeArg;
        /// <summary>
        /// Defines the _Formula.
        /// </summary>
        private String _Formula = $"{nameof(MathType.Binary)}:C1+C2";

        //Enusure Math channel's arguments are within a reasonable range to observe waveform
        /// <summary>
        /// Defines the _InitFlag.
        /// </summary>
        private Boolean _InitFlag = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="MathModel"/> class.
        /// </summary>
        /// <param name="id">The id<see cref="ChannelId"/>.</param>
        /// <param name="color">The color<see cref="Color"/>.</param>
        /// <param name="active">The active<see cref="Boolean"/>.</param>
        public MathModel(ChannelId id, Color color, Boolean active)
            : base(ChannelType.Math, id, color)
        {
            Active = active;

            Conditioning = new ConditioningModel(/*this*/);
            Sampling = new SamplingModelEx(MathType);

            var dsm = new DataSrcMath();
            PrepareSamples = dsm.Prepare;
            ReadSamples = dsm.Read;
            ProcessSamples = dsm.Process;

            ProcessNormalSamples = dsm.ProcessNormal;
            ProcessMaxHoldSamples = dsm.ProcessMaxHold;
            ProcessMinHoldSamples = dsm.ProcessMinHold;
            ProcessAverageSamples = dsm.ProcessAverage;
            ProcessInit = dsm.Init;


            MakeVuSamples = WfmVuDatabase.Rescale;
            MakeVuSamplesMathFFT = WfmVuDatabase.RescaleMathFFT;

            OldID = id;
            _FrequencyAdapter = new(this, OnPropertyChanged);
        }
        private MathArgPrsnt? _Args = null;

        private FrequencyAdapter _FrequencyAdapter;
        public FrequencyAdapter FrequencyAdapter
        {
            get { return _FrequencyAdapter; }
        }
        private static readonly Object _Lock = new Object();

        private List<ChannelId> _PreMathChannels = new List<ChannelId>();
        public List<ChannelId> PreMathChannels
        {
            get
            {
                lock (this)
                {
                    return _PreMathChannels;
                }
            }
            set
            {
                lock (_Lock)
                {
                    _PreMathChannels = value;
                }
            }
        }

        public Boolean IsAutoUnit
        {
            get { return Conditioning.IsAutoUnit || AutoScale; }
            set
            {
                if (Conditioning.IsAutoUnit != value)
                    Conditioning.IsAutoUnit = value;
                OnPropertyChanged("Formula");

            }
        }

        private String _CustomUnit = "V";
        public String CustomUnit
        {
            get => _CustomUnit;
            set
            {
                if (_CustomUnit != value)
                {
                    _CustomUnit = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _AutoScale = true;
        /// <summary>
        /// Gets or sets the Args.
        /// </summary>
        public MathArgPrsnt? Args
        {
            get { return _Args; }
            set
            {
                if (value != Args && value != null)
                {
                    if (_Args != null)
                    {
                        if (value is not MathFftArg && _Args is MathFftArg fft)
                        {
                            if (fft.Marker != null)
                            {
                                fft.Marker.AtuoMarkerActive = false;
                            }
                            Sampling.IgnoreScaleMaxMin = IgnoreScaleLimit.Both;
                            Sampling.IgnorePositionMaxMin = IgnoreScaleLimit.Both;
                        }
                        else
                        {
                            Sampling.IgnoreScaleMaxMin = IgnoreScaleLimit.None;
                            Sampling.IgnorePositionMaxMin = IgnoreScaleLimit.None;
                        }
                        MathType = value.Type;
                        _Args = value;
                    }
                    else
                    {
                        _Args = value;
                        MathType = value.Type;
                    }
                    OnPropertyChanged();
                }
            }
        }

        #region 递归获取任意情况下的Math通道的最终独立信源列表
        /// <summary>
        /// 获取数据运算的数据信源源
        /// </summary>
        /// <returns></returns>
        public List<ChannelId> GetSources()
        {
            List<ChannelId> sources = new List<ChannelId>() { Id};
            List<ChannelId> temp = new List<ChannelId>();
            if (Args != null && Active)
            {
                switch (MathType)
                {
                    case MathType.Binary:
                        if (Args is MathBinaryArg binary && binary != null)
                        {
                            temp = new List<ChannelId>() { binary.Source1st, binary.Source2nd };

                        }
                        break;
                    case MathType.FFT:
                        if (Args is MathFftArg fft && fft != null)
                        {
                            temp.Add(fft.Source);
                        }
                        break;
                    case MathType.Filter:
                        if (Args is MathFilterArg filter && filter != null)
                        {
                            temp.Add(filter.Source);
                        }
                        break;
                    case MathType.Custom:
                        if (Args is MathCustomArg custom && custom != null)
                        {
                            temp = custom.GetSourcesByToken();
                        }
                        break;
                    case MathType.UserProgram:
                        if (Args is MathUserProgramArg user && user != null)
                        {
                            temp.Add(user.Source);
                        }
                        break;
                    case MathType.Histgram:
                        if (Args is MathHistArg hist && hist != null)
                        {
                            temp.Add(hist.Source);
                        }
                        break;
                    case MathType.Trend:
                        if (Args is MathTrendArg trend && trend != null)
                        {
                            temp.Add(trend.Source);
                        }
                        break;
                    case MathType.Track:
                        if (Args is MathTrackArg track && track != null)
                        {
                            temp.Add(track.Source);
                        }
                        break;
                }
            }

            if (temp != null && temp.Count > 0)
            {
                foreach (var id in temp)
                {
                    var childrenids = GetSourcesByChannelId(id);
                    if (childrenids != null && childrenids.Count > 0)
                    {
                        sources.AddRange(childrenids);
                    }
                    sources.Add(id);
                }

                sources = sources.Distinct().ToList();
            }

            return sources;

        }

        private List<ChannelId> GetSourcesByChannelId(ChannelId id)
        {
            List<ChannelId> sources = new List<ChannelId>();

            if (id.IsMath() && DsoModel.Default.TryGetChannel(id, out var chnl) && chnl is MathModel mm && mm != null)
            {
                var s1st = mm.GetSources();
                if (s1st != null && s1st.Count > 0)
                {
                    foreach (var s in s1st)
                    {
                        AddSourceToLsit(s, ref sources);
                    }
                }
            }
            else if (id.IsMeasure())//如果信源是参数测量
            {
                var measitem = DsoModel.Default.Meas.SelectedItems[id - ChannelId.P1];

                if (measitem != null)
                {
                    //参数测量又分为独立运算
                    if (measitem.MeasureType == MeasureType.Single)
                    {
                        AddSourceToLsit(measitem.Source, ref sources);

                        if (measitem.Dualsrc)
                        {
                            AddSourceToLsit(measitem.Source2nd, ref sources);
                        }
                    }
                    else if (measitem.MeasureType == MeasureType.Composite)//和复合运算
                    {
                        var temp1 = GetSourcesByChannelId(measitem.Source);
                        if (temp1 != null && temp1.Count > 0)
                        {
                            foreach (var temp in temp1)
                            {
                                AddSourceToLsit(temp, ref sources);
                            }
                        }
                        var temp2 = GetSourcesByChannelId(measitem.Source2nd);
                        if (temp2 != null && temp2.Count > 0)
                        {
                            foreach (var temp in temp2)
                            {
                                AddSourceToLsit(temp, ref sources);
                            }
                        }
                    }
                }

            }

            return sources;


            void AddSourceToLsit(ChannelId id, ref List<ChannelId> ids)
            {
                var temp = GetSourcesByChannelId(id);
                if (temp != null && temp.Count > 0)
                {
                    ids.AddRange(temp);
                }
                ids.Add(id);
            }

        }

        #endregion


        private readonly Dictionary<AnaChnlTimebaseIndex, Single> ScaleTableByus = new()
        {
            [AnaChnlTimebaseIndex.Lv2p] = 2e-6F,
            [AnaChnlTimebaseIndex.Lv5p] = 5e-6F,
            [AnaChnlTimebaseIndex.Lv10p] = 10e-6F,
            [AnaChnlTimebaseIndex.Lv20p] = 20e-6F,
            [AnaChnlTimebaseIndex.Lv50p] = 50e-6F,
            [AnaChnlTimebaseIndex.Lv100p] = 100e-6F,
            [AnaChnlTimebaseIndex.Lv200p] = 200e-6F,
            [AnaChnlTimebaseIndex.Lv500p] = 500e-6F,
            [AnaChnlTimebaseIndex.Lv1n] = 1e-3F,
            [AnaChnlTimebaseIndex.Lv2n] = 2e-3F,
            [AnaChnlTimebaseIndex.Lv5n] = 5e-3F,
            [AnaChnlTimebaseIndex.Lv10n] = 10e-3F,
            [AnaChnlTimebaseIndex.Lv20n] = 20e-3F,
            [AnaChnlTimebaseIndex.Lv50n] = 50e-3F,
            [AnaChnlTimebaseIndex.Lv100n] = 100e-3F,
            [AnaChnlTimebaseIndex.Lv200n] = 200e-3F,
            [AnaChnlTimebaseIndex.Lv500n] = 500e-3F,
            [AnaChnlTimebaseIndex.Lv1u] = 1e0F,
            [AnaChnlTimebaseIndex.Lv2u] = 2e0F,
            [AnaChnlTimebaseIndex.Lv5u] = 5e0F,
            [AnaChnlTimebaseIndex.Lv10u] = 10e0F,
            [AnaChnlTimebaseIndex.Lv20u] = 20e0F,
            [AnaChnlTimebaseIndex.Lv50u] = 50e0F,
            [AnaChnlTimebaseIndex.Lv100u] = 100e0F,
            [AnaChnlTimebaseIndex.Lv200u] = 200e0F,
            [AnaChnlTimebaseIndex.Lv500u] = 500e0F,
            [AnaChnlTimebaseIndex.Lv1m] = 1e3F,
            [AnaChnlTimebaseIndex.Lv2m] = 2e3F,
            [AnaChnlTimebaseIndex.Lv5m] = 5e3F,
            [AnaChnlTimebaseIndex.Lv10m] = 10e3F,
            [AnaChnlTimebaseIndex.Lv20m] = 20e3F,
            [AnaChnlTimebaseIndex.Lv50m] = 50e3F,
            [AnaChnlTimebaseIndex.Lv100m] = 100e3F,
            [AnaChnlTimebaseIndex.Lv200m] = 200e3F,
            [AnaChnlTimebaseIndex.Lv500m] = 500e3F,
            [AnaChnlTimebaseIndex.Lv1] = 1e6F,
            [AnaChnlTimebaseIndex.Lv2] = 2e6F,
            [AnaChnlTimebaseIndex.Lv5] = 5e6F,
            [AnaChnlTimebaseIndex.Lv10] = 10e6F,
            [AnaChnlTimebaseIndex.Lv20] = 20e6F,
            [AnaChnlTimebaseIndex.Lv50] = 50e6F,
            [AnaChnlTimebaseIndex.Lv100] = 100e6F,
            [AnaChnlTimebaseIndex.Lv200] = 200e6F,
            [AnaChnlTimebaseIndex.Lv500] = 500e6F,
            [AnaChnlTimebaseIndex.Lv1k] = 1e9F,
            [AnaChnlTimebaseIndex.Lv2k] = 2e9F,
            [AnaChnlTimebaseIndex.Lv5k] = 5e9F,

            [AnaChnlTimebaseIndex.Lv10k] = 10e9F,
            [AnaChnlTimebaseIndex.Lv20k] = 20e9F,
            [AnaChnlTimebaseIndex.Lv50k] = 50e9F,
            [AnaChnlTimebaseIndex.Lv100k] = 100e9F,
            [AnaChnlTimebaseIndex.Lv200k] = 200e9F,
            [AnaChnlTimebaseIndex.Lv500k] = 500e9F,
            [AnaChnlTimebaseIndex.Lv1M] = 1e12F,
            [AnaChnlTimebaseIndex.Lv2M] = 2e12F,
            [AnaChnlTimebaseIndex.Lv5M] = 5e12F,
        };

        //public (Int32 Index,Int32 Tick) TryGetAbsoluteScaleIndex(Double scale, Prefix prefix)
        //{
        //    var abscale = scale * Math.Pow(1000.0, prefix-Prefix.Micro);

        //    var key=ScaleTableByus.Aggregate((x, y) => Math.Abs(x.Value - abscale) < Math.Abs(y.Value - abscale) ? x : y).Key;

        //    var tick = abscale - ScaleTableByus[key];

        //    return ((Int32)key, (Int32)tick);
        //}

        public (Single ScaleValue, Int32 MinScaleIndex, Int32 MaxScaleIndex) GetHistSampleScale(Double width)
        {
            Double pervalue = width / 8 * 1E6;

            List<Single> scales = ScaleTableByus.Values.ToList();
            for (Int32 index = 0; index < ScaleTableByus.Count - 1; index++)
            {
                if (scales[index] < pervalue && scales[index + 1] >= pervalue)
                {
                    return (scales[index + 1], -index, scales.Count - index - 1);
                }
            }
            return (scales[1], -1, ScaleTableByus.Count - 1);
        }

        public Boolean AutoScale
        {
            get => _AutoScale;
            set
            {
                if (_AutoScale != value)
                {
                    _AutoScale = value;
                    OnPropertyChanged();
                }
            }
        }

        public Object? Occupier
        {
            get;
            internal set;
        }

        public override Boolean Active
        {
            get => base.Active;
            set
            {
                base.Active = value;
                if (Occupier != null)
                {
                    if (Occupier is AdvancedMathModel advanced)
                    {
                        advanced.Enabled = value;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Conditioning.
        /// </summary>
        public override ConditioningModel Conditioning { get; }

        private Dictionary<MathType, (Int32 ConditionScalIndex, Int32 SamplingScaleIndex)> ArgeScale = new()
        {
            //{MathType.Binary,(0,0) },
            //{MathType.FFT,(0,0) },
            //{MathType.Zoom,(0,0) },
            //{MathType.Filter,(0,0) },
            //{MathType.ERes,(0,0) },
            //{MathType.Histgram,(0,0) },
            //{MathType.Track,(0,0) },
            //{MathType.Trend,(0,0) },
            //{MathType.Custom,(0,0) },
            //{MathType.UserProgram,(0,0) },
        };

        /// <summary>
        /// Gets or sets the Formula.
        /// </summary>
        public String Formula
        {
            get => _Formula;
            set
            {
                if (_Formula != value)
                {
                    _Formula = value;
                    OnPropertyChanged();
                    //InitFlag = 1;
                }
            }
        }

        public void FreshMeasureSetParameter()
        {
            OnPropertyChanged("FFTMeasure");
        }

        public String OldKey = "";

        public ChannelId OldID;

        public MathType _MathType = MathType.Binary;
        public MathType MathType
        {
            get { return _MathType; }
            set
            {
                if (_MathType != value)
                {
                    if (ArgeScale.ContainsKey(_MathType))
                    {
                        ArgeScale[_MathType] = (Conditioning.ScaleIndex, Sampling.ScaleIndex);
                    }
                    _MathType = value;
                    Sampling.MathType = value;
                    if (value != MathType.Histgram && value != MathType.Trend && value != MathType.Track)
                    {
                        AutoScale = false;
                    }
                    else
                    {
                        AutoScale = true;
                    }
                    //Conditioning.ScaleIndex = ArgeScale[_MathType].ConditionScalIndex;
                    //Sampling.ScaleIndex = ArgeScale[_MathType].SamplingScaleIndex;
                }
            }
        }

        public (Int32 VScaleIndex, Int32 HScaleIndex) ReadMathScale(MathType type, Vector? res)
        {
            if (ArgeScale.ContainsKey(type) && !AutoScale)
            {
                return (ArgeScale[type].ConditionScalIndex, ArgeScale[type].SamplingScaleIndex);
            }
            else
            {
                VScaleFit(this, res);
                ArgeScale.TryAdd(type, (Conditioning.ScaleIndex, Sampling.ScaleIndex));
                return (Conditioning.ScaleIndex, Sampling.ScaleIndex);
            }

        }

        public Boolean ClearFlag { get; set; } = false;

        /// <summary>
        /// Gets or sets the InitFlag.
        /// </summary>
        public Boolean InitFlag
        {
            get
            {
                return _InitFlag;
                //Int32 x = Interlocked.Decrement(ref _InitFlag);
                //if (_InitFlag <= 0)
                //{
                //    _InitFlag = 0;
                //}
                //return x >= 0;
            }
            set
            {
                _InitFlag = value;
                //Interlocked.Add(ref _InitFlag,2);
            }
        }

        public Boolean ResetFFT
        {
            get;
            set;
        }

        public Boolean IsSwitchWindow
        {
            get;
            set;
        } = false;

        private Boolean _TmbInitFlag = false;
        public Boolean TmbInitFlag
        {
            get
            {
                return _TmbInitFlag;
            }
            set
            {
                _TmbInitFlag = value;
            }
        }

        #region 谱线

        public delegate void Init();
        public Init? ProcessInit
        {
            get;
            set;
        }
        public ProcessHandler<Double[,]>? ProcessNormalSamples
        {
            get;
            set;
        }
        public ProcessHandler<Double[,]>? ProcessAverageSamples
        {
            get;
            set;
        }
        public ProcessHandler<Double[,]>? ProcessMaxHoldSamples
        {
            get;
            set;
        }
        public ProcessHandler<Double[,]>? ProcessMinHoldSamples
        {
            get;
            set;
        }
        public Func<MathModel, Int32, RFWaveType, WfmVuBaseParam?, WfmVuBlock?>? MakeVuSamplesMathFFT
        {
            get;
            set;
        }
        private Boolean _ResetLine = false;
        public Boolean ResetLine
        {
            get
            {
                return _ResetLine;
            }
            set
            {
                _ResetLine = value;
            }
        }
        #region 正常
        private Boolean _NormalLine = true;
        public Boolean NormalLine
        {
            get { return _NormalLine; }
            set
            {
                if (_NormalLine != value)
                {
                    _NormalLine = value;
                    _InitFlag = true;
                    _ResetLine = true;
                }
            }
        }

        private PickMode _NormalLinePickMode = PickMode.Sample;
        public PickMode NormalLinePickMode
        {
            get { return _NormalLinePickMode; }
            set
            {
                if (_NormalLinePickMode != value)
                {
                    _NormalLinePickMode = value;
                    _ResetLine = true;
                }
            }
        }

        public Color NormalLineColor = Color.Yellow;

        public WfmPack? PackNormal
        {
            get;
            protected set;
        }

        public WfmVuDatabase VuDatabaseNormal
        {
            get;
        } = new();

        #endregion

        #region 最大值保持

        private Boolean _MaxHoldLine = false;
        public Boolean MaxHoldLine
        {
            get { return _MaxHoldLine; }
            set
            {
                if (_MaxHoldLine != value)
                {
                    _MaxHoldLine = value;
                    _InitFlag = true;
                    _ResetLine = true;
                }
            }
        }

        private PickMode _MaxHoldLinePickMode = PickMode.Sample;
        public PickMode MaxHoldLinePickMode
        {
            get { return _MaxHoldLinePickMode; }
            set
            {
                if (_MaxHoldLinePickMode != value)
                {
                    _MaxHoldLinePickMode = value;
                    _ResetLine = true;
                    //OnPropertyChanged();
                }
            }
        }

        public Color MaxHoldLineColor = Color.Red;

        public WfmPack? PackMaxHold
        {
            get;
            protected set;
        }

        public WfmVuDatabase VuDatabaseMaxHold
        {
            get;
        } = new();

        #endregion

        #region 最小值保持


        private Boolean _MinHoldLine = false;
        public Boolean MinHoldLine
        {
            get { return _MinHoldLine; }
            set
            {
                if (_MinHoldLine != value)
                {
                    _MinHoldLine = value;
                    _InitFlag = true;
                    _ResetLine = true;
                }
            }
        }

        private PickMode _MinHoldLinePickMode = PickMode.Sample;
        public PickMode MinHoldLinePickMode
        {
            get { return _MinHoldLinePickMode; }
            set
            {
                if (_MinHoldLinePickMode != value)
                {
                    _MinHoldLinePickMode = value;
                    _ResetLine = true;
                    //OnPropertyChanged();
                }
            }
        }

        public Color MinHoldLineColor = Color.Blue;

        public WfmPack? PackMinHold
        {
            get;
            protected set;
        }

        public WfmVuDatabase VuDatabaseMinHold
        {
            get;
        } = new();

        #endregion

        #region 平均

        private Int32 _AverageTimes = Constants.RF_AVERAGE_TIMES_MAX;
        public Int32 AverageTimes
        {
            get { return _AverageTimes; }
            set
            {
                value = value < Constants.RF_AVERAGE_TIMES_MIN ? Constants.RF_AVERAGE_TIMES_MIN : value;
                value = value > Constants.RF_AVERAGE_TIMES_MAX ? Constants.RF_AVERAGE_TIMES_MAX : value;

                if (_AverageTimes != value)
                {
                    _AverageTimes = value;
                    _InitFlag = true;
                    _ResetLine = true;
                    //OnPropertyChanged();
                }
            }
        }

        private Boolean _AverageLine = false;
        public Boolean AverageLine
        {
            get { return _AverageLine; }
            set
            {
                if (_AverageLine != value)
                {
                    _AverageLine = value;
                    _InitFlag = true;
                    _ResetLine = true;
                }
            }
        }

        private PickMode _AverageLinePickMode = PickMode.Sample;
        public PickMode AverageLinePickMode
        {
            get { return _AverageLinePickMode; }
            set
            {
                if (_AverageLinePickMode != value)
                {
                    _AverageLinePickMode = value;
                    _ResetLine = true;
                    //OnPropertyChanged();
                }
            }
        }

        public Color AverageLineColor = Color.LightGreen;

        public WfmPack? PackAverage
        {
            get;
            protected set;
        }

        public WfmVuDatabase VuDatabaseAverage
        {
            get;
        } = new();

        #endregion
        #endregion


        /// <summary>
        /// Gets the Sampling.
        /// </summary>
        public override SamplingModelEx Sampling { get; }

        private static void VScaleFit(MathModel mch, Vector? res)
        {
            if (res == null)
            {
                return;
            }
            //=res.XUnit;
            var unx = res.XUnit;
            var uny = res.YUnit;

            //设置Y轴值范围
            var data = res.Elements;
            if (data != null && data.Length > 0)
            {
                Double max = data[0, 0];
                Double min = data[0, 0];
                for (Int32 i = 0; i < data.GetLength(0); i++)
                {
                    for (Int32 j = 0; j < data.GetLength(1); j++)
                    {
                        if (data[i, j] > max)
                            max = data[i, j];
                        if (data[i, j] < min)
                            min = data[i, j];
                    }
                }

                if (mch.MathType == MathType.Histgram)
                {
                    min = 0;
                    if (max == 0)
                    {
                        max = 10;
                    }
                }
                

                Double range = (max == min) ? max : max - min;
                Double average = (max == min) ? max / 2 : (max + min) / 2;

                var prefix = uny == "V" ? Prefix.Milli : mch.Conditioning.Prefix;
                range = Quantity.ConvertByPrefix(range, Prefix.Empty, prefix);
                average = Quantity.ConvertByPrefix(average, Prefix.Empty, prefix);
                //从小到大找到合适的刻度
                for (Int32 i = mch.Conditioning.ScaleMinIndex; i < mch.Conditioning.ScaleMaxIndex; i++)
                {
                    Double scale = mch.Conditioning.GetScaleValue(i, 0);
                    if (range <= (scale * Constants.VIS_YDIVS_NUM * 4 / 5) ||
                        i == mch.Conditioning.ScaleMaxIndex)
                    {
                        mch.Conditioning.ScaleIndex = i;
                        break;
                    }
                }

                //设置到合适的中间位置
                //mch.Conditioning.PosIndex = -1 * average / mch.Conditioning.Scale * mch.Conditioning.PosIdxPerDiv;
            }

        }



        /// <summary>
        /// Defines the <see cref="ConditioningModel" />.
        /// </summary>
        internal sealed class ConditioningModel : VertAxisModel
        {
            /// <summary>
            /// Defines the _IsAutoUnit.
            /// </summary>
            private Boolean _IsAutoUnit = true;

            //private readonly MathModel _OuterModel;
            /// <summary>
            /// Initializes a new instance of the <see cref="ConditioningModel"/> class.
            /// </summary>
            public ConditioningModel(/*MathModel outerModel*/) : base("Conditioning")
            {
            }

            /// <summary>
            /// Gets or sets the IsAutoUnit.
            /// </summary>
            public Boolean IsAutoUnit
            {
                get => _IsAutoUnit;
                set
                {
                    if (value != _IsAutoUnit)
                    {
                        _IsAutoUnit = value;
                        OnPropertyChanged();
                    }
                }
            }

            public override String Unit
            {
                get
                {
                    return base.Unit;
                }
                set
                {
                    base.Unit = value;
                }

            }

            /// <summary>
            /// The GetPosition.
            /// </summary>
            /// <param name="posIndex">The posIndex<see cref="Double"/>.</param>
            /// <returns>The <see cref="Double"/>.</returns>
            public Double GetPosition(Double posIndex)
            {
                return GetPosValue(posIndex, 0);
            }

            //public override Double PosIndex
            //{
            //    get => base.PosIndex;
            //    set
            //    {
            //        base.PosIndex = value;
            //        //_OuterModel?.UpdateFormula();
            //        OnPropertyChanged(nameof(_OuterModel.Formula));
            //    }
            //}

            public override Int32 ScaleIndex
            {
                get => base.ScaleIndex;
                //set
                //{
                //    base.ScaleIndex = value;
                //    //_OuterModel?.UpdateFormula();
                //    OnPropertyChanged(nameof(_OuterModel.Formula));
                //}
            }

            /// <summary>
            /// The GetScale.
            /// </summary>
            /// <param name="scaleIndex">The scaleIndex<see cref="Int32"/>.</param>
            /// <returns>The <see cref="Double"/>.</returns>
            public Double GetScale(Int32 scaleIndex)
            {
                return GetScaleValue(scaleIndex, 0);
            }
        }

        /// <summary>
        /// Defines the <see cref="SamplingModelEx" />.
        /// </summary>
        internal sealed class SamplingModelEx : SamplingModel
        {
            //private readonly MathModel _OuterModel;
            /// <summary>
            /// Initializes a new instance of the <see cref="SamplingModelEx"/> class.
            /// </summary>
            public SamplingModelEx(MathType type) : base()
            {
                MathType = type;
            }

            public MathType MathType
            {
                get;
                set;
            }

            //public override Double PosIndex
            //{
            //    get => base.PosIndex;
            //    set
            //    {
            //        base.PosIndex = value;

            //        OnPropertyChanged(nameof(_OuterModel.Formula));
            //    }
            //}
            /// <summary>
            /// Gets or sets the ScaleIndex.
            /// </summary>
            public override Int32 ScaleIndex
            {
                get => base.ScaleIndex;
                set
                {
                    if (base.ScaleIndex != value)
                    {
                        base.ScaleIndex = value;

                        if (MathType != MathType.FFT)
                        {
                            ReCalcMaxMinPosIdx(InitialScale.Value);
                        }
                        else
                        {
                            PosMinIndex = -PosDefIndex * 1E10;
                            PosMaxIndex = PosDefIndex * 10;
                        }

                        var tmp = PosDefIndex - TempPosition * PosIdxPerDiv / Scale;
                        if (!Double.IsFinite(tmp))
                            tmp = PosDefIndex;
                        base.PosIndex = tmp;
                    }
                    //OnPropertyChanged(nameof(_OuterModel.Formula));
                }
            }

            /// <summary>
            /// The GetPosition.
            /// </summary>
            /// <param name="posIndex">The posIndex<see cref="Double"/>.</param>
            /// <returns>The <see cref="Double"/>.</returns>
            public Double GetPosition(Double posIndex)
            {
                return GetPosValue(posIndex, 0);
            }

            /// <summary>
            /// The GetScale.
            /// </summary>
            /// <param name="scaleIndex">The scaleIndex<see cref="Int32"/>.</param>
            /// <returns>The <see cref="Double"/>.</returns>
            public Double GetScale(Int32 scaleIndex)
            {
                return GetScaleValue(scaleIndex, 0);
            }
        }

        public override Boolean Take(Boolean init, CancellationToken ct, CancellationToken? softResetToken = null)
        {
            var args = PrepareSamples?.Invoke(init, Id, ct, DataRole.View, softResetToken);
            var pkg = ReadSamples?.Invoke(args);
            if (pkg is not null)
            {
                lock (this)
                {

                }
                Pack = ProcessSamples?.Invoke(pkg.Value, args);

                MathContext ctx = (MathContext)args!;
                if (!(ctx is null) && ctx.Math.Args is MathFftArg)
                {
                    if (_ResetLine)
                    {
                        ProcessInit?.Invoke();
                        _ResetLine = false;

                    }

                    PackNormal = ProcessNormalSamples?.Invoke(((Double[,], Object))pkg, args);
                    PackAverage = ProcessAverageSamples?.Invoke(((Double[,], Object))pkg, args);
                    PackMaxHold = ProcessMaxHoldSamples?.Invoke(((Double[,], Object))pkg, args);
                    PackMinHold = ProcessMinHoldSamples?.Invoke(((Double[,], Object))pkg, args);
                }
                else
                {
                    if (_ResetLine)
                    {
                        ProcessInit?.Invoke();
                        _ResetLine = false;
                    }
                }

                return true;
            }
            return false;
        }
        public override void ClearBuffer()
        {
            Pack = null;
            PackNormal = null;
            PackAverage = null;
            PackMaxHold = null;
            PackMinHold = null;
        }
    }
}

// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/2</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using ScopeX.ComModel;
    using ScopeX.Core.Decode;
    using ScopeX.Core.Tools;
    using System.Linq;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.Core.Model;

    /// <summary>
    /// Defines the <see cref="DsoModel" />.
    /// </summary>
    internal class DsoModel
    {
        /// <summary>
        /// Defines the Default.
        /// </summary>
        public static readonly DsoModel Default = new();

        /// <summary>
        /// Defines the _Channels.
        /// </summary>
        private readonly ConcurrentDictionary<ChannelId, ChannelModel> _Channels = new();
        private readonly ConcurrentDictionary<ChannelId, PowerAnalysisModel> _PowerAnalysisModels = new();

        private readonly ConcurrentDictionary<ChannelId, RadioFrequencyModel> _RadioFrequencies = new();

        private readonly ConcurrentDictionary<ChannelId, AmpVSTimeModel> _AmpVSTimes = new();

        private readonly ConcurrentDictionary<ChannelId, PhaseVSTimeModel> _PhaseVSTimes = new();

        private readonly ConcurrentDictionary<ChannelId, PhaseVSFrequencyModel> _PhaseVSFrequencies = new();

        private readonly ConcurrentDictionary<ChannelId, TimeVSFrequencyModel> _TimeVSFrequencies = new();

        private readonly ConcurrentDictionary<ChannelId, FrequencyVSTimeModel> _FrequencyVSTimes = new();

        public static ConcurrentDictionary<String, Int32> NavBarGroupRecords = new ConcurrentDictionary<String, Int32>();

        public ConcurrentDictionary<ChannelId, UInt16[,]> ChannelAdcDatas = new ConcurrentDictionary<ChannelId, UInt16[,]>()
        {
            [ChannelId.C1] = new UInt16[1, 0],
            [ChannelId.C2] = new UInt16[1, 0],
            [ChannelId.C3] = new UInt16[1, 0],
            [ChannelId.C4] = new UInt16[1, 0]
        };

        /// <summary>
        /// Defines the _Trigger.
        /// </summary>
        private readonly Dictionary<TriggerType, TriggerModel> _Trigger = new();

        /// <summary>
        /// Defines the _WfmGenerator.
        /// </summary>
        private readonly Dictionary<ChannelId, ArbWfmGenModel> _WfmGenerator = new();

        /// <summary>
        /// Defines the _DataSrcOpt.
        /// </summary>
        private static DataSourceOpt _DataSrcOpt = DataSourceOpt.Simulator;

        public LocationAssistedModel LocAssisted { get; }

        public TriggerAssistedModel TriggerAssisted { get; }

        public VisualTriggerModel VisualTrigger { get; }
        public DateTime AnalogChPositionUpdateTime { get; set; }

        public DateTime DigitalChPositionUpdateTime { get; set; }

        /// <summary>
        /// DsoInfo：获取产品信息或选件信息时使用
        /// </summary>
        internal DsoInfos DsoInfo { get; }

        /// <summary>
        /// DsoInfo副本：修改产品信息或选件信息时使用
        /// </summary>
        internal DsoInfos DsoInfoBackup { get; set; }

        //private SystemMode _SysMode = SystemMode.Normal;
        //internal SystemMode SysMode
        //{
        //    get => _SysMode;
        //    set
        //    {
        //        if (_SysMode == value)
        //            return;
        //        _SysMode = value;
        //    }
        //}

        /// <summary>
        /// Prevents a default instance of the <see cref="DsoModel"/> class from being created.
        /// </summary>
        private DsoModel()
        {
            DsoInfo = new();
            DsoInfoBackup = new();
            Display = new();
            Cursors = new();
            Markers = new();
            Meas = new();
            PassFail = new();
            PwrAnalysis = new(Meas);
            //WfmInspector = new(Meas);

            JitterModel = new(ChannelId.JITTER);
            AreaHistModel=new();
            VectorAnalysisModel = new();
            Print = new();
            SystemCheck = new();
            File = new();
            Setting = new();
            LIN = new();
            //XY = new();
            Filter = new();
            LocAssisted = new(Meas);
            VisualTrigger = new();
            TriggerAssisted = new(LocAssisted, VisualTrigger);
            Search = new(Meas);
            ArtificialIntelligence = new();
            MultiDomain = new();
            IntelligentChartManager = new();
            TempCtrl = new();
            ExceptionCapture = new(ChannelId.ExceptionCapture);
            //MultiDomain = new();
            var config = AppConfig.GetIntance();

            if (!Enum.TryParse(config.TimebaseMaxIndex/*AppConfigureHelper.AppSettings["TimebaseMaxIndex"]?.ToString()*/, out AnaChnlTimebaseIndex maxtbi))
            {
                maxtbi = AnaChnlTimebaseIndex.Lv50;
            }

            if (!Enum.TryParse(config.TimebaseMinIndex/*AppConfigureHelper.AppSettings["TimebaseMinIndex"]?.ToString()*/, out AnaChnlTimebaseIndex mintbi))
            {
                mintbi = AnaChnlTimebaseIndex.Lv500p;
            }

            if (!Enum.TryParse(config.TimebaseMinScanIndex/*AppConfigureHelper.AppSettings["TimebaseMinScanIndex"]?.ToString()*/, out AnaChnlTimebaseIndex minscan))
            {
                minscan = AnaChnlTimebaseIndex.Lv100m;
            }

            if (!Enum.TryParse(config.TimebaseMaxItplIndex/*AppConfigureHelper.AppSettings["TimebaseMaxItplIndex"]?.ToString()*/, out AnaChnlTimebaseIndex maxitpl))
            {
                maxitpl = AnaChnlTimebaseIndex.Lv100n;
            }

            Timebase = new(maxtbi, mintbi, minscan, maxitpl);
            //if (Boolean.TryParse(AppConfigureHelper.AppSettings["AdaptiveLength"]?.ToString(), out var al))
            //{
            //    Timebase.AdaptiveLength = al;
            //}
            Timebase.AdaptiveLength = config.AdaptiveLength;

            _Trigger.Add(TriggerType.Edge, new TriggerEdgeModel());
            _Trigger.Add(TriggerType.PulseWidth, new TriggerWidthModel());
            _Trigger.Add(TriggerType.Glitch, new TriggerGlitchModel());
            _Trigger.Add(TriggerType.Interval, new TriggerIntervalModel());
            _Trigger.Add(TriggerType.Pattern, new TriggerPatternModel());
            _Trigger.Add(TriggerType.State, new TriggerStateModel());
            _Trigger.Add(TriggerType.Runt, new TriggerRuntModel());
            _Trigger.Add(TriggerType.Transition, new TriggerTransModel());
            _Trigger.Add(TriggerType.Video, new TriggerVideoModel());
            _Trigger.Add(TriggerType.Window, new TriggerWindowModel());
            _Trigger.Add(TriggerType.Delay, new TriggerDelayModel());
            _Trigger.Add(TriggerType.TimeOut, new TriggerTimeOutModel());
            _Trigger.Add(TriggerType.SustainTime, new TriggerSustainTimeModel());
            _Trigger.Add(TriggerType.SetupHold, new TriggerSetupHoldModel());
            _Trigger.Add(TriggerType.NEdge, new TriggerNEdgeModel());
            _Trigger.Add(TriggerType.MultiQulified, new TriggerMultiQualifiedModel());
            _Trigger.Add(TriggerType.Serial, new CloseTrigSerialModel());

            #region MD
            Frequency = new FrequencyModel[ChannelIdExt.RFChnlNum];
            for (Int32 i = 0; i < Frequency.Length; i++)
            {
                Frequency[i] = new FrequencyModel();
            }
            foreach (var id in ChannelIdExt.GetAmpVSTimes())
            {
                _ = _AmpVSTimes.TryAdd(id, new AmpVSTimeModel(id, ColorLookup.Default[id.ToString()], false, Timebase));
            }
            foreach (var id in ChannelIdExt.GetPhaseVSTimes())
            {
                _ = _PhaseVSTimes.TryAdd(id, new PhaseVSTimeModel(id, ColorLookup.Default[id.ToString()], false, Timebase));
            }
            foreach (var id in ChannelIdExt.GetPhaseVSFrequencies())
            {
                _ = _PhaseVSFrequencies.TryAdd(id, new PhaseVSFrequencyModel(id, ColorLookup.Default[id.ToString()], false, Frequency[id - ChannelIdExt.MinPVFChId]));
            }
            foreach (var id in ChannelIdExt.GetTimeVSFrequencies())
            {
                _ = _TimeVSFrequencies.TryAdd(id, new TimeVSFrequencyModel(id, ColorLookup.Default[id.ToString()], false, Frequency[id - ChannelIdExt.MinTVFChId]));
            }
            foreach (var id in ChannelIdExt.GetFrequencyVSTimes())
            {
                _ = _FrequencyVSTimes.TryAdd(id, new FrequencyVSTimeModel(id, ColorLookup.Default[id.ToString()], false, Timebase));
            }

            foreach (var id in ChannelIdExt.GetRadioFrequencies())
            {
                AmpVSTimeModel ampVSTime = _AmpVSTimes[id - ChannelIdExt.MinRFChId + ChannelIdExt.MinAVTChId];
                PhaseVSTimeModel phaseVSTime = _PhaseVSTimes[id - ChannelIdExt.MinRFChId + ChannelIdExt.MinPVTChId];
                PhaseVSFrequencyModel phaseVSFrequency = _PhaseVSFrequencies[id - ChannelIdExt.MinRFChId + ChannelIdExt.MinPVFChId];
                TimeVSFrequencyModel timeVSFrequency = _TimeVSFrequencies[id - ChannelIdExt.MinRFChId + ChannelIdExt.MinTVFChId];
                FrequencyVSTimeModel frequencyVSTime = _FrequencyVSTimes[id - ChannelIdExt.MinRFChId + ChannelIdExt.MinFVTChId];
                RadioFrequencyModel rfm = new RadioFrequencyModel(id, ColorLookup.Default[id.ToString()], false, Frequency[id - ChannelIdExt.MinRFChId], ampVSTime, phaseVSTime, phaseVSFrequency, timeVSFrequency, frequencyVSTime);

                //!!!Initialize the RF channel's source
                rfm.Source = ChannelId.C1 + (id - ChannelId.RF1);
                ampVSTime.Source = rfm.Source;
                phaseVSTime.Source = rfm.Source;
                phaseVSFrequency.Source = rfm.Source;
                timeVSFrequency.Source = rfm.Source;
                frequencyVSTime.Source = rfm.Source;

                _ = _RadioFrequencies.TryAdd(id, rfm);
            }

            #endregion

            foreach (var id in ChannelIdExt.GetAnalogs())
            {
                var ach = new AnalogModel(id, ColorLookup.Default[id.ToString()], true, Timebase);

                _ = _Channels.TryAdd(id, ach);

                Double posindex = ach.Conditioning.PosIndex;
                String scalemaxindex = "";
                String scaleminindex = "";

                if (id == ChannelId.C1)
                {
                    posindex = config.C1PosIndex;
                    scalemaxindex = config.C1ScaleMaxIndex;
                    scaleminindex = config.C1ScaleMinIndex;
                }
                else if (id == ChannelId.C2)
                {
                    posindex = config.C2PosIndex;
                    scalemaxindex = config.C2ScaleMaxIndex;
                    scaleminindex = config.C2ScaleMinIndex;
                }
                else if (id == ChannelId.C3)
                {
                    posindex = config.C3PosIndex;
                    scalemaxindex = config.C3ScaleMaxIndex;
                    scaleminindex = config.C3ScaleMinIndex;
                }
                else if (id == ChannelId.C4)
                {
                    posindex = config.C4PosIndex;
                    scalemaxindex = config.C4ScaleMaxIndex;
                    scaleminindex = config.C4ScaleMinIndex;
                }
                else
                {
                }
                
                ach.Conditioning.PosIndex = posindex;
                if (!String.IsNullOrEmpty(scalemaxindex) && Enum.TryParse(scalemaxindex, out AnaChnlScaleIndex maxscale))
                {
                    ach.Conditioning.ScaleMaxIndex = maxscale;
                    ach.Conditioning.ScaleHighZMaxIndex = maxscale;
                    ach.Conditioning.Scale50OhmMaxIndex = AnaChnlScaleIndex.Lv1;
                }

                if (!String.IsNullOrEmpty(scaleminindex) && Enum.TryParse(scaleminindex, out AnaChnlScaleIndex minscale))
                {
                    ach.Conditioning.ScaleMinIndex = minscale;
                }


                //if (Double.TryParse(AppConfigureHelper.AppSettings[id.ToString() + "PosIndex"]?.ToString(), out var pos))
                //{
                //    ach.Conditioning.PosIndex = pos;
                //}

                //if (Enum.TryParse(AppConfigureHelper.AppSettings[id.ToString() + "ScaleMaxIndex"]?.ToString(), out AnaChnlScaleIndex maxscale))
                //{
                //    ach.Conditioning.ScaleMaxIndex = maxscale;
                //    ach.Conditioning.ScaleHighZMaxIndex = maxscale;
                //    ach.Conditioning.Scale50OhmMaxIndex = AnaChnlScaleIndex.Lv1;
                //}

                //if (Enum.TryParse(AppConfigureHelper.AppSettings[id.ToString() + "ScaleMinIndex"]?.ToString(), out AnaChnlScaleIndex minscale))
                //{
                //    ach.Conditioning.ScaleMinIndex = minscale;
                //}

                //!!!Let user bind RF channel to anylog channel
                //ach.RadioFrequency = _RadioFrequencies[ChannelId.RF1 + (id - ChannelId.C1)];
            }

            //!!!Add external RF channel
            //_ = _Channels.TryAdd(ChannelId.RF, _RadioFrequencies[ChannelId.RF]);

            foreach (var id in ChannelIdExt.GetMaths())
            {
                _ = _Channels.TryAdd(id, new MathModel(id, ChannelPrsnt.GetDrawColor(id), false));
            }
            foreach (var id in ChannelIdExt.GetPowerAnalysisMaths())
            {
                _ = _Channels.TryAdd(id, new MathModel(id, ChannelPrsnt.GetDrawColor(id), false));
            }
            foreach (var id in ChannelIdExt.GetJitterMaths())
            {
                _ = _Channels.TryAdd(id, new MathModel(id, ChannelPrsnt.GetDrawColor(id), false));
            }
            foreach (var id in ChannelIdExt.GetDecodes())
            {
                _ = _Channels.TryAdd(id, new DecodeModel(id, ChannelPrsnt.GetDrawColor(id), false, Timebase));
            }
            foreach (var id in ChannelIdExt.GetCEMaths())
            {
                _ = _Channels.TryAdd(id, new MathModel(id, ChannelPrsnt.GetDrawColor(id), false));
            }
            foreach (var id in ChannelIdExt.GetIRMaths())
            {
                _ = _Channels.TryAdd(id, new MathModel(id, ChannelPrsnt.GetDrawColor(id), false));
            }
            foreach (var id in ChannelIdExt.GetMDMaths())
            {
                _ = _Channels.TryAdd(id, new MathModel(id, ColorLookup.Default[id.ToString()], false));
            }
            _ = _Channels.TryAdd(ChannelId.D0, new DigitalModel(ChannelId.D0, ChannelPrsnt.GetDrawColor(ChannelId.D0), Timebase));

            _WfmGenerator.Add(ChannelId.AWG1, new(ChannelId.AWG1) { WfmType = ArbWfmType.Sinusoid });
            _WfmGenerator.Add(ChannelId.AWG2, new(ChannelId.AWG2) { WfmType = ArbWfmType.Pulse });
#if DEBUG
            _WfmGenerator.Add(ChannelId.AWG3, new(ChannelId.AWG3) { WfmType = ArbWfmType.Ramp });
            _WfmGenerator.Add(ChannelId.AWG4, new(ChannelId.AWG4) { WfmType = ArbWfmType.DC });
#endif

            Voltmeter = new(ChannelId.DVM);
            Cymometer = new(ChannelId.CYM);
        }

        public void RefreshAWGStaus()
        {
            KeyLed.Default.SetLed(LedEnum.LedAWG, _WfmGenerator.Values.Any(x => x.Active));
        }
        /// <summary>
        /// Gets or sets the DataSrcOpt.
        /// </summary>
        public static DataSourceOpt DataSrcOpt
        {
            get => _DataSrcOpt;
            set
            {
                if (_DataSrcOpt != value)
                {
                    _DataSrcOpt = value;
                    Dispatcher.OnDataSourceChanged();
                }
            }
        }

        /// <summary>
        /// Gets the AnalogChnls.
        /// </summary>
        public IEnumerable<AnalogModel> AnalogChnls
        {
            get
            {
                foreach (var id in ChannelIdExt.GetAnalogs())
                {
                    yield return (AnalogModel)_Channels[id];
                }
            }
        }
        public Boolean AnalogChnlsIsNull
        {
            get => _Channels == null;
        }

        /// <summary>
        /// Gets the Channels.
        /// </summary>
        public IEnumerable<ChannelModel> Channels
        {
            get
            {
                foreach (var (_, cm) in _Channels)
                {
                    yield return cm;
                }
            }
        }

        /// <summary>
        /// Gets the Cursors.
        /// </summary>
        public CursorModel Cursors { get; }

        /// <summary>
        /// Gets the Cymometer.
        /// </summary>
        public CymometerModel Cymometer { get; }
        public Int32 _SoftResetCount = 0;
        public Int32 SoftResetCount
        {
            get
            {
                return _SoftResetCount;
            }
            set
            {
                _SoftResetCount = value;
                if (_SoftResetCount > 100000)
                {
                    _SoftResetCount = 0;
                }
            }
        }
        public Int64? MainWindowId
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the DecodeChnls.
        /// </summary>
        public IEnumerable<ChannelModel> DecodeChnls
        {
            get
            {
                foreach (var id in ChannelIdExt.GetDecodes())
                {
                    yield return _Channels[id];
                }
            }
        }

        /// <summary>
        /// Gets the DigitalChnls.
        /// </summary>
        public IEnumerable<ChannelModel> DigitalChnls
        {
            get
            {
                yield return _Channels[ChannelId.D0];
            }
        }

        /// <summary>
        /// Gets the Display.
        /// </summary>
        public DisplayModel Display { get; }

        /// <summary>
        /// Gets the File.
        /// </summary>
        public FileModel File { get; }

        /// <summary>
        /// Gets the Filter.
        /// </summary>
        public FilterModel Filter { get; }

        /// <summary>
        /// Gets the MathChnls.
        /// </summary>
        public IEnumerable<MathModel> MathChnls
        {
            get
            {
                foreach (var id in ChannelIdExt.GetMaths())
                {
                    yield return (MathModel)_Channels[id];
                }
                foreach (var id in ChannelIdExt.GetPowerAnalysisMaths())
                {
                    yield return (MathModel)_Channels[id];
                }
                foreach (var id in ChannelIdExt.GetJitterMaths())
                {
                    yield return (MathModel)_Channels[id];
                }
                foreach (var id in ChannelIdExt.GetCEMaths())
                {
                    yield return (MathModel)_Channels[id];
                }
                foreach (var id in ChannelIdExt.GetIRMaths())
                {
                    yield return (MathModel)_Channels[id];
                }
                foreach (var id in ChannelIdExt.GetMDMaths())
                {
                    yield return (MathModel)_Channels[id];
                }
            }
        }

        /// <summary>
        /// Gets the Meas.
        /// </summary>
        public MeasureModel Meas { get; }

        /// <summary>
        /// Gets the PassFail.
        /// </summary>
        public PassFailModel PassFail { get; }

        /// <summary>
        /// Gets the Setting.
        /// </summary>
        public SettingModel Setting { get; }

        /// <summary>
        /// Gets the LIN.
        /// </summary>
        public LANModel LIN { get; }

        /// <summary>
        /// Gets the Print.
        /// </summary>
        public PrintModel Print { get; }

        /// <summary>
        /// Gets the SystemCheck.
        /// </summary>
        public SystemCheckModel SystemCheck { get; }

        public ExceptionCaptureModel ExceptionCapture { get; }

        public TempCtrlModel TempCtrl { get; }
        /// <summary>
        /// Gets the PwrAnalysis.
        /// </summary>
        public PowerAnalysis.PowerAnalysisModel PwrAnalysis { get; }

        /// <summary>
        /// Gets the ReferenceChnls.
        /// </summary>
        public IEnumerable<ReferenceModel> ReferenceChnls
        {
            get
            {
                foreach (var id in ChannelIdExt.GetReferences())
                {
                    if (_Channels.ContainsKey(id))
                    {
                        yield return (ReferenceModel)_Channels[id];
                    }
                }
            }
        }

        public MarkerModel Markers
        {
            get;
        }


        public IEnumerable<RadioFrequencyModel> RadioFrequencyChnls
        {
            get
            {
                foreach (var id in ChannelIdExt.GetRadioFrequencies())
                {
                    if (_RadioFrequencies.ContainsKey(id))
                        yield return _RadioFrequencies[id];
                }
            }
        }

        public IEnumerable<AmpVSTimeModel> RFAmpVSTimeChnls
        {
            get
            {
                foreach (var id in ChannelIdExt.GetAmpVSTimes())
                {
                    if (_AmpVSTimes.ContainsKey(id))
                        yield return _AmpVSTimes[id];
                }
            }
        }

        public IEnumerable<PhaseVSTimeModel> RFPhaseVSTimeChnls
        {
            get
            {
                foreach (var id in ChannelIdExt.GetPhaseVSTimes())
                {
                    if (_PhaseVSTimes.ContainsKey(id))
                        yield return _PhaseVSTimes[id];
                }
            }
        }

        public IEnumerable<PhaseVSFrequencyModel> RFPhaseVSFrequencyChnls
        {
            get
            {
                foreach (var id in ChannelIdExt.GetPhaseVSFrequencies())
                {
                    if (_PhaseVSFrequencies.ContainsKey(id))
                        yield return _PhaseVSFrequencies[id];
                }
            }
        }

        public IEnumerable<TimeVSFrequencyModel> RFTimeVSFrequencyChnls
        {
            get
            {
                foreach (var id in ChannelIdExt.GetTimeVSFrequencies())
                {
                    if (_TimeVSFrequencies.ContainsKey(id))
                        yield return _TimeVSFrequencies[id];
                }
            }
        }

        public IEnumerable<FrequencyVSTimeModel> RFFrequencyVSTimeChnls
        {
            get
            {
                foreach (var id in ChannelIdExt.GetFrequencyVSTimes())
                {
                    if (_FrequencyVSTimes.ContainsKey(id))
                        yield return _FrequencyVSTimes[id];
                }
            }
        }


        /// <summary>
        /// Gets the Jitter.
        /// </summary>
        public JitterModel JitterModel { get; }

        /// <summary>
        /// Gets the Timebase.
        /// </summary>
        public TimebaseModel Timebase { get; }

        public FrequencyModel[] Frequency { get; }

        /// <summary>
        /// Gets the VectorAnalysisModel.
        /// </summary>
        public VectorAnalysisModel VectorAnalysisModel { get; }

        /// <summary>
        /// Gets the Voltmeter.
        /// </summary>
        public VoltmeterModel Voltmeter { get; }

        /// <summary>
        /// Gets the WfmInspector.
        /// </summary>
        //public Search.SearchModel WfmInspector { get; }

        public AreaHistogramModel AreaHistModel { get; }
        public SearchModel Search { get; }
        public ArtificialIntelligenceModel ArtificialIntelligence { get; }

        public MultiDomainModel MultiDomain { get; }

        public IntelligentChartManager IntelligentChartManager { get; }
		//public TempCtrlModel TempCtrl { get; }

        //public MultiDomainModel MultiDomain { get; }

        /// <summary>
        /// The AddChannel.
        /// </summary>
        /// <param name="id">The id<see cref="ChannelId"/>.</param>
        /// <param name="cm">The cm<see cref="ChannelModel"/>.</param>
        public void AddChannel(ChannelId id, ChannelModel cm)
        {
            if (!_Channels.TryAdd(id, cm))
            {
                if (_Channels[id] != cm)
                {
                    _Channels[id] = cm;
                }
            }
        }

        /// <summary>
        /// The GetChannel.
        /// </summary>
        /// <param name="id">The id<see cref="ChannelId"/>.</param>
        /// <returns>The <see cref="ChannelModel"/>.</returns>
        public ChannelModel GetChannel(ChannelId id)
        {
            if (_Channels.TryGetValue(id, out var channel))
            {
                return channel;
            }
            if (_RadioFrequencies.TryGetValue(id, out var rfchannel))
                return rfchannel;
            if (_AmpVSTimes.TryGetValue(id, out var avtchannel))
                return avtchannel;
            if (_PhaseVSTimes.TryGetValue(id, out var pvtchannel))
                return pvtchannel;
            if (_PhaseVSFrequencies.TryGetValue(id, out var pvfchannel))
                return pvfchannel;
            if (_TimeVSFrequencies.TryGetValue(id, out var tvfchannel))
                return tvfchannel;
            if (_FrequencyVSTimes.TryGetValue(id, out var fvtchannel))
                return fvtchannel;

            throw new NullReferenceException($"{id} channel does not exist!");
        }

        /// <summary>
        /// The GetTrigger.
        /// </summary>
        /// <returns>The <see cref="TriggerModel"/>.</returns>
        public TriggerModel GetTrigger()
        {
            return GetTriggerModel(TriggerModel.Type);
        }

        /// <summary>
        /// The GetTrigger.
        /// </summary>
        /// <param name="tt">The tt<see cref="TriggerType"/>.</param>
        /// <returns>The <see cref="TriggerModel"/>.</returns>
        public TriggerModel GetTriggerModel(TriggerType tt)
        {
            if (tt == TriggerType.Serial)
            {
                return TriggerSerialShareParameter.Default.GetTriggerSerial();
            }

            if (_Trigger.TryGetValue(tt, out var trigger))
            {
                return trigger;
            }

            throw new NotImplementedException($"{tt} trigger is not supported!");
        }

        /// <summary>
        /// The GetWfmGenerator.
        /// </summary>
        /// <param name="id">The id<see cref="ChannelId"/>.</param>
        /// <returns>The <see cref="ArbWfmGenModel"/>.</returns>
        public ArbWfmGenModel GetWfmGenerator(ChannelId id)
        {
            if (_WfmGenerator.TryGetValue(id, out var generator))
            {
                return generator;
            }

            throw new NotImplementedException($"{id} is not supported!");
        }

        /// <summary>
        /// The GetWfmPack.
        /// </summary>
        /// <param name="id">The id<see cref="ChannelId"/>.</param>
        /// <returns>The <see cref="WfmPack?"/>.</returns>
        public WfmPack? GetWfmPack(ChannelId id)
        {
            if (_Channels.TryGetValue(id, out var channel))
            {
                return channel.Pack;
            }

            return null;
        }

        /// <summary>
        /// The RemoveChannel.
        /// </summary>
        /// <param name="id">The id<see cref="ChannelId"/>.</param>
        /// <returns>The <see cref="Boolean"/>.</returns>
        public Boolean RemoveChannel(ChannelId id)
        {
            return _Channels.Remove(id, out var _);
        }

        /// <summary>
        /// The TryGetChannel.
        /// </summary>
        /// <param name="id">The id<see cref="ChannelId"/>.</param>
        /// <param name="chnl">The chnl<see cref="ChannelModel?"/>.</param>
        /// <returns>The <see cref="Boolean"/>.</returns>
        public Boolean TryGetChannel(ChannelId id, [NotNullWhen(true)] out ChannelModel? chnl)
        {
            return _Channels.TryGetValue(id, out chnl);
        }

        public IEnumerable<PowerAnalysisModel> PowerAnalysisModels
        {
            get
            {
                foreach (var id in ChannelIdExt.GetPowers())
                {
                    if (_PowerAnalysisModels.ContainsKey(id))
                        yield return _PowerAnalysisModels[id];
                }
            }
        }
        public void AddPowerChannel(ChannelId id, PowerAnalysisModel cm)
        {
            if (!_PowerAnalysisModels.TryAdd(id, cm))
            {
                if (_PowerAnalysisModels[id] != cm)
                {
                    _PowerAnalysisModels[id] = cm;
                }
            }
        }
        public PowerAnalysisModel GetPowerChannel(ChannelId id)
        {
            if (_PowerAnalysisModels.TryGetValue(id, out var channel))
            {
                return channel;
            }
            return null;
        }
    }
}

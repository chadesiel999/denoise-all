using EventBus;
using ScopeX.ComModel;
using ScopeX.Core.Decode;
using ScopeX.Core.Hardware;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;


namespace ScopeX.Core
{
    public enum ModelCreateOptions
    {
        Dependant,
        Standalone,
        InitializedByChild
    }

    public enum DataSourceOpt
    {
        PCIe,
        Simulator,
        NetWork,
        USB,
        RS232,
        FILE
    }

    public enum Forms
    {
        [Description("Wu")]
        None = 0,
        [Description("GuangBiao")]
        Cursor = 0x26,
        [Description("SheZhi")]
        Setting = 0x28,
        [Description("CunChu")]
        Save = 0x25,
        [Description("DouDongFenXi")]
        Jitter = KeyCode.VK_SDA,
        [Description("FFT")]
        FFT = KeyCode.VK_FFT,
        [Description("P_F_CeShi")]
        PassFail = KeyCode.VK_PASSFAIL,
        [Description("DianYuanFenXi")]
        PowerAnalysis = KeyCode.VK_PWRANALYSIS,
        [Description("UltraAcq")]
        FastReq = KeyCode.FASTACQ,
        [Description("SouSuo")]
        Search = KeyCode.VK_WAVESEARCH,
        [Description("JiePing")]
        ScreenShot = KeyCode.VK_SCREENSHOT,
        [Description("AboutClear")]
        Clear = KeyCode.VK_CLEAR,
        [Description("AboutPrinter")]
        Printer = KeyCode.VK_SETPRINTER,
    }
    public class DsoPrsnt : IDsoPrsnt
    {
        public static DsoPrsnt DefaultDsoPrsnt = null;

        public static Boolean IsDsoClosing = false;

        private ProductType productType;
        private Default? _Default = null;
        public static Boolean KeyBoardLockEnable { get; set; } = false;
        public static List<Int32> KeyBoardForbidLockKeys = new List<Int32>();
        public static Boolean AWGProtectEnable { get; set; } = false;

        public static ConcurrentDictionary<String, Int32> NavBarGroupRecords => DsoModel.NavBarGroupRecords;
        public static Func<Boolean> GetKeyBoardLocked = new Func<Boolean>(() => { return KeyBoardLockEnable; });
        public String SoftWareVersion = String.Empty;
        public String HardWareVersion = String.Empty;

        private Boolean _IsCheckLAMutex = false;
        public Action<Int32>? ErrorMsgBox;
        public Action? VuMinimize;
        public Action? VuClose;
        public Action? VuShutDowm;
        public Action? VuRestart;
        public Action? VuLogout;
        private Demo _Demo;

        private Int32 _CloseMutexFunctionFlag = 0; // 0=false, 1=true

        // 获取标志并自动重置为false
        public Boolean MutexFunctionFlag => Interlocked.Exchange(ref _CloseMutexFunctionFlag, 0) == 1;
        // 设置标志为true
        public void SetMutexFunctionFlag()
        {
            Interlocked.Exchange(ref _CloseMutexFunctionFlag, 1);
        }


        public static DataSourceOpt DataSrcOpt
        {
            get => DsoModel.DataSrcOpt;
            set => DsoModel.DataSrcOpt = value;
        }

        private Language _SysLanguage = Language.简体中文;
        /// <summary>
        /// 系统语言
        /// </summary>
        public Language SysLanguage
        {
            get => _SysLanguage;
            set
            {
                if (_SysLanguage != value)
                {
                    _SysLanguage = value;
                    View?.UpdateView(this, nameof(SysLanguage));

                    //保存语言信息
                    AppConfig.GetIntance().LANGUAGEID = (Int32)_SysLanguage;
                    AppConfig.GetIntance().SaveConfig();
                }
            }
        }
        private ManufacturerAdatper _ManufacturerAdatper = ManufacturerAdatper.Default;
        /// <summary>
        /// Scpi指令厂家适配器
        /// </summary>
        public ManufacturerAdatper ManufacturerAdatper
        {
            get => _ManufacturerAdatper;
            set
            {
                if (_ManufacturerAdatper != value)
                {
                    _ManufacturerAdatper = value;
                    View?.UpdateView(this, nameof(ManufacturerAdatper));
                }
            }
        }

        public IReadOnlyList<ScpiAdapter.ScpiAdapter>? ScpiAdapters;

        /// <summary>
        /// 数学自定义公式函数字典
        /// </summary>
        public IReadOnlyDictionary<String, MathFormulaInfo>? MathFormulaCollections = null;

        public ConcurrentDictionary<ChannelId, UInt16[,]> ChannelAdcDatas => DsoModel.Default.ChannelAdcDatas;

        public static ChannelId FocusId
        {
            get => ChannelModel.FocusId;
            set
            {
                DefaultDsoPrsnt.Markers.SwtichFocusItemMethod(value);
                ChannelZIndex.Default.Prepend(value);
                if (ChannelModel.FocusId != value)
                {
                    ChannelModel.FocusId = value;
                    Dispatcher.SoftReset();
                    //Hardware.HdCmdFactory.Push(HdCmd.ChnlActive);
                    if (ChannelModel.FocusId.IsAnalog())
                    {
                        KeyLed.Default.SetFocusChannel(ChannelModel.FocusId);
                    }
                    //<Remark>更改人：彭博 创建日期：2024/3/4 15:53:00 原因：当选中当前通道时，参数测量的信源随之切换</Remark>
                    if (FocusId.IsAnalog() && DsoModel.Default.TryGetChannel(FocusId, out var cm) && cm.Active)
                    {
                        DefaultDsoPrsnt.Measure.SnapshotSource = FocusId;
                    }
                    if (FocusId.IsAnalog() || (FocusId.IsReference() || FocusId.IsBaseMath()) && DsoModel.Default.TryGetChannel(FocusId, out cm) && cm.Active)
                    {
                        if (DefaultDsoPrsnt.Cursor.Active)
                        {
                            DefaultDsoPrsnt.Cursor.SyncSource = DsoPrsnt.FocusId;
                        }
                    }
                }
            }
        }
        public Forms UserSettingForm { get; set; }

        /// <summary>
        /// 设置光标测量的信号源
        /// </summary>
        /// <Remark>更改人：彭博 创建日期：2024/2/17 15:50:00 原因：光标测量信号源自动切换</Remark>
        public void SetCursorSyncSource()
        {
            if (Cursor.Active)
            {
                if (FocusId.IsAnalog() || FocusId.IsBaseMath() || FocusId.IsReference())
                {
                    Cursor.SyncSource = FocusId;
                }
                else
                {
                    //当前的信源为模拟通道、数学或参考时，无需切换信源
                    //if (DsoPrsnt.DefaultDsoPrsnt.Cursor.SyncSource.IsAnalog() || DsoPrsnt.DefaultDsoPrsnt.Cursor.SyncSource.IsBaseMath() || DsoPrsnt.DefaultDsoPrsnt.Cursor.SyncSource.IsReference())
                    //    return;
                    //当有Math和Reference时，优先切换到Math或Reference
                    var zlist = ChannelZIndexList.ToList();
                    foreach (var z in zlist)
                    {
                        DsoModel.Default.TryGetChannel(z, out ChannelModel cm);
                        if (cm != null && cm.Active && (cm.Id.IsBaseMath() || cm.Id.IsReference() || cm.Id.IsAnalog()))
                        {
                            Cursor.SyncSource = cm.Id;
                            break;
                        }
                    }
                    if ((Cursor.SyncSource.IsBaseMath() || Cursor.SyncSource.IsReference()) && DsoModel.Default.TryGetChannel(Cursor.SyncSource, out ChannelModel channelModel) && !channelModel.Active)
                    {
                        Cursor.SyncSource = ChannelId.C1;
                    }
                }
            }
        }
        public Int32 SoftResetCount
        {
            get
            {
                return DsoModel.Default.SoftResetCount;
            }
        }
        public static List<ChannelId> ChannelZIndexList
        {
            get
            {
                return ChannelZIndex.Default.ChannelZIndexList;
            }
        }
        public Int64? MainWindowId
        {
            get
            {
                return DsoModel.Default.MainWindowId;
            }
            set
            {
                DsoModel.Default.MainWindowId = value;
            }
        }
        public void UpdateAnalogZIndex()
        {
            Int32 index = 0;
            Int32 analogzindex = 0;


            var analogzlist = ChannelZIndexList.Where(a => { return a.IsAnalog(); }).ToList();

            for (Int32 i = analogzlist.Count - 1; i >= 0; i--)
            {
                if (TryGetChannel(analogzlist[i], out IChnlPrsnt? channel) && channel is AnalogPrsnt analogprsnt)
                {
                    analogprsnt.ZIndex = index;
                    analogzindex = (analogzindex << 4) | (analogzlist[i] - ChannelId.C1);
                    index++;
                }
            }

            Display.AnalogZIndex = analogzindex;
        }
        public void MoveFocusId(ChannelId firstId = ChannelId.C1, ChannelId lastId = ChannelId.D15)
        {
            var zlist = ChannelZIndexList.Where(a => a >= firstId && a <= lastId).ToList();
            foreach (var item in zlist)
            {
                if (item == FocusId)
                {
                    continue;
                }
                if (_ChnlPrsntMap.ContainsKey(item) && _ChnlPrsntMap[item].Active)
                {
                    FocusId = item;
                    SetCursorSyncSource();
                    return;
                }
            }
            FocusId = _ChnlPrsntMap.Any(p => p.Value.Active) ? FocusId : ChannelZIndexList.First(p => p.IsAnalog());
            ChannelZIndex.Default.Prepend(FocusId);
        }

        private static readonly ConcurrentDictionary<ChannelId, IChnlPrsnt> _ChnlPrsntMap = new();

        //除了不被系统支持的特定通道以外，其他通道始终都存在，但通道的具体模型类型是可变的，动态的
        public Boolean TryGetChannel(ChannelId id, [NotNullWhen(true)] out IChnlPrsnt? cprsnt)
        {
            return _ChnlPrsntMap.TryGetValue(id, out cprsnt);
        }

        //Creates a shallow copy of a range of elements in the source _ChnlPrsntMap
        public List<IChnlPrsnt> TryGetRange(IEnumerable<ChannelId> identities)
        {
            var cprsnts = new List<IChnlPrsnt>(64);
            foreach (var id in identities)
            {
                cprsnts.Add(_ChnlPrsntMap[id]);
            }
            return cprsnts.OrderBy(x => x.Id).ToList();
        }

        public List<IChnlPrsnt> TryGetRange(Predicate<IChnlPrsnt> match)
        {
            return _ChnlPrsntMap.Where(kvp => match(kvp.Value)).Select(kvp => kvp.Value).OrderBy(x => x.Id).ToList();
        }

        public List<IChnlPrsnt> TryGetRange(IEnumerable<ChannelId> identities, Predicate<IChnlPrsnt> match)
        {
            var cprsnts = new List<IChnlPrsnt>(64);
            foreach (var id in identities)
            {
                if (match(_ChnlPrsntMap[id]))
                {
                    cprsnts.Add(_ChnlPrsntMap[id]);
                }
            }
            return cprsnts.OrderBy(x => x.Id).ToList();
        }

        public static Color GetDrawColor(ChannelId id)
        {
            return ColorLookup.Default[id.ToString()];
        }

        public void AddChannel(ChannelId id, IChnlPrsnt cprsnt)
        {
            if (!_ChnlPrsntMap.TryAdd(id, cprsnt))
            {
                if (_ChnlPrsntMap[id] != cprsnt)
                {
                    _ChnlPrsntMap[id] = cprsnt;
                }
            }
            else
            {
            }
        }

        public Boolean RemoveChannel(ChannelId id)
        {
            return _ChnlPrsntMap.Remove(id, out var _);
        }

        public IEnumerable<IChnlPrsnt> GetAllChnls()
        {
            //foreach (var (_, prsnt) in _ChnlPrsntMap)
            //    yield return prsnt;
            return _ChnlPrsntMap.Select(kvp => kvp.Value);
        }

        public List<ChannelId> FindIdentities(Predicate<IChnlPrsnt> match)
        {
            var activechnls = new List<ChannelId>(64);
            foreach (var (k, v) in _ChnlPrsntMap)
            {
                if (match(v))
                {
                    activechnls.Add(k);
                }
            }
            return activechnls;
        }

        public Boolean ContainsKey(ChannelId id)
        {
            return _ChnlPrsntMap.ContainsKey(id);
        }

        public Boolean TryGetBadge(ChannelId id, [NotNullWhen(true)] out IBadge? badge)
        {
            if (id.IsDigital())
            {
                id = ChannelId.D0;
            }

            if (TryGetChannel(id, out var cp))
            {
                badge = cp;
                return true;
            }
            else if (id.IsAWG())
            {
                badge = GetWfmGenerator(id);
                return true;
            }
            else if (id == ChannelId.DVM)
            {
                badge = Voltmeter;
                return true;
            }
            else if (id == ChannelId.CYM)
            {
                badge = Cymometer;
            }
            badge = null;
            return false;
        }

        public TimebasePrsnt Timebase
        {
            get;
        }

        ITimebasePrsnt IDsoPrsnt.Timebase => Timebase;

        public TriggerPrsnt CurrentTrigger => TriggerPrsnt.GetOrMakeTrigger(this, TriggerPrsnt.Type);

        ITriggerPrsnt IDsoPrsnt.CurrentTrigger => CurrentTrigger;

        public TriggerPrsnt SetTrigger(TriggerType tt, ITriggerView? itv = null)
        {
            return TriggerPrsnt.GetOrMakeTrigger(this, tt, itv);
        }

        public IDsoView? View
        {
            get;
            set;
        }

        public void Run()
        {
            Dispatcher.Run();
            UpdateVuTask.Run();
            Resume();
        }

        public void Cancel()
        {
            Dispatcher.Cancel();
            UpdateVuTask.Cancel();
            KeyLed.Default.SetClose(false);
        }

        public void Stop()
        {
            Dispatcher.Stop();
            //Hardware.HdCmdFactory.Push(HdCmd.OuterPannelLEDCtrl);
        }

        public void Resume()
        {
            Dispatcher.Resume();
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            _Demo.RemoveDemo();
        }
        public void RemoveDemo()
        {
            _Demo.RemoveDemo();
        }
        public Boolean Open(String deviceInfo, DataSourceOpt ds = DataSourceOpt.Simulator)
        {
            return Dispatcher.Open(deviceInfo, productType, ds, Logs, ErrorMsgBox);
        }

        public void Close()
        {
            Dispatcher.Close();
        }
        public void CloseAllLed()
        {
            ExportHdFuncs.CloseAllLed();
        }

        public void HardwarePowerOff()
        {
            Dispatcher.HardwarePowerOff();
        }

        private Boolean _SquareWaveSwitch = true;
        /// <summary>
        /// 方波开关
        /// </summary>
        public Boolean SquareWaveSwitch
        {
            get { return _SquareWaveSwitch; }
            set
            {
                _SquareWaveSwitch = value;
                PushHdCmd(HdCmd.SquareWaveSwitch);
            }
        }

        public Boolean IsScan => Dispatcher.IsScan;

        public Boolean IsSoftReset => Dispatcher.IsModelNewerThanDraw();

        public void SoftReset()
        {
            Dispatcher.SoftReset();
        }

        public void UpdateDraw() => Dispatcher.UpdateDrawStamp();

        public readonly DisplayPrsnt Display;

        public readonly MeasPrsnt Measure;

        public readonly CursorPrsnt Cursor;

        public readonly MarkerPrsnt Markers;

        public readonly PassFailPrsnt PassFail;

        public Dictionary<ChannelId, PowerAnalysis.PowerAnalysisPrsnt> PwrAnalysisDictionary = new();

        public readonly SearchPrsnt Search;

        public readonly JitterPrsnt Jitter;

        public readonly VectorAnalysisPrsnt VectorAnalysis;

        public readonly PrintPrsnt Print;

        public readonly FilePrsnt File;

        public readonly FilterPrsnt Filter;

        public readonly AutoSet AutoSet;

        public readonly AutoCalibration AutoCalibration;

        public readonly ProbeCalibration ProbeCalibration;

        public readonly SettingPrsnt Setting;

        public readonly TempCtrlPrsnt TempCtrl;

        public readonly SystemCheckPrsnt SystemCheck;

        private readonly Dictionary<ChannelId, ArbWfmGenPrsnt> _WfmGenPrsntMap = new();

        public OptionsManager OptionsManager;

        public SysRunTimeMangager SysRunTimeMangager;

        public readonly SaveDataSoure SaveDataSoure;


        public ArbWfmGenPrsnt GetWfmGenerator(ChannelId id)
        {
            return _WfmGenPrsntMap[id];
        }

        public IEnumerable<ArbWfmGenPrsnt> ArbWfmGens => _WfmGenPrsntMap.Values;

        public ProductType ProductType { get => productType; }

        public readonly VoltmeterPrsnt Voltmeter;

        public readonly ExceptionCapturePrsnt ExceptionCapture;

        public readonly CymometerPrsnt Cymometer;

        public readonly WidgetPrsnt Widgets;

        public readonly LocationAssistedPrsnt LocAssisted;

        public readonly VisualTriggerPrsnt VisualTrigger;

        public readonly TriggerAssistedPrsnt TriggerAssist;

        public readonly LANPrsnt LAN;

        public readonly AreaHistogramPrsnt AreaHistogram;
		
		public readonly ArtificialIntelligencePrsnt ArtificialIntelligence;

        public readonly MultiDomainPrsnt MultiDomain;

        public DsoPrsnt(IDsoView? form = null, ProductType product = ProductType.Base)
        {
            productType = product;
            //由于需要获取硬件配置，故先初始化硬件定义
            Hd.Load(productType);

            CheckOnStarting();

            SysLanguage = (Language)AppConfig.GetIntance().LANGUAGEID!.Value;

            OptionsManager = OptionsManager.Default;

            SysRunTimeMangager = SysRunTimeMangager.Default;

            AdcInterleaveProcessor.Default.Oscilloscope = this;

            PlatformManager.Default.Platform.LoadOriginSetting();
            LoadLastSetting();
            AdcInterleaveProcessor.Default.Process();

            Timebase = new TimebasePrsnt(this, form?.TimebaseVu);

            Cursor = new CursorPrsnt(this, null);

            Markers = new MarkerPrsnt(this, null);

            Display = new DisplayPrsnt(this, null);

            Measure = new MeasPrsnt(this);

            _Demo = new Demo(this);

            if (Constants.ENABLE_PassFail)
                PassFail = new PassFailPrsnt(this, null);


            if (Constants.ENABLE_Search)
                Search = new SearchPrsnt(this, null);

            //if (Constants.ENABLE_SDA)
            Jitter = new JitterPrsnt(this, null);

            VectorAnalysis = new VectorAnalysisPrsnt(this, null);

            Print = new PrintPrsnt(this, null);

            File = new FilePrsnt(this, null);

            Filter = new FilterPrsnt(this, null);

            AutoSet = new AutoSet(this);

            AutoCalibration = new AutoCalibration(this);

            ProbeCalibration = new ProbeCalibration(this);

            Setting = new SettingPrsnt(this, null);

            SystemCheck = new SystemCheckPrsnt(this, null);

            Widgets = new WidgetPrsnt();

            LocAssisted = new LocationAssistedPrsnt(this);

            VisualTrigger = new VisualTriggerPrsnt(this);

            TriggerAssist = new TriggerAssistedPrsnt(this);

            if (TriggerPrsnt.Type != TriggerType.Serial)
                TriggerPrsnt.GetOrMakeTrigger(this, TriggerPrsnt.Type, form?.TriggerVu);

            SaveDataSoure = new SaveDataSoure(this);

            LAN = new LANPrsnt(this);

            AreaHistogram = new(this);

            TempCtrl = new TempCtrlPrsnt(this);
			
			ArtificialIntelligence = new ArtificialIntelligencePrsnt(this, form?.ArtificialIntelligenceView);

            MultiDomain = new MultiDomainPrsnt(this);
            #region MD

            foreach (var id in ChannelIdExt.GetAmpVSTimes())
                _ChnlPrsntMap.TryAdd(id, new AmpVSTimePrsnt(id, this));

            foreach (var id in ChannelIdExt.GetPhaseVSTimes())
                _ChnlPrsntMap.TryAdd(id, new PhaseVSTimePrsnt(id, this));

            foreach (var id in ChannelIdExt.GetPhaseVSFrequencies())
                _ChnlPrsntMap.TryAdd(id, new PhaseVSFrequencyPrsnt(id, this));

            foreach (var id in ChannelIdExt.GetTimeVSFrequencies())
                _ChnlPrsntMap.TryAdd(id, new TimeVSFrequencyPrsnt(id, this));

            foreach (var id in ChannelIdExt.GetFrequencyVSTimes())
                _ChnlPrsntMap.TryAdd(id, new FrequencyVSTimePrsnt(id, this));

            foreach (var id in ChannelIdExt.GetRadioFrequencies())
                _ChnlPrsntMap.TryAdd(id, new RadioFrequencyPrsnt(id, this,
                    (AmpVSTimePrsnt)(_ChnlPrsntMap[id - (Int32)ChannelIdExt.MinRFChId + (Int32)ChannelIdExt.MinAVTChId]),
                    (PhaseVSTimePrsnt)(_ChnlPrsntMap[id - (Int32)ChannelIdExt.MinRFChId + (Int32)ChannelIdExt.MinPVTChId]),
                    (PhaseVSFrequencyPrsnt)(_ChnlPrsntMap[id - (Int32)ChannelIdExt.MinRFChId + (Int32)ChannelIdExt.MinPVFChId]),
                    (TimeVSFrequencyPrsnt)(_ChnlPrsntMap[id - (Int32)ChannelIdExt.MinRFChId + (Int32)ChannelIdExt.MinTVFChId]),
                    (FrequencyVSTimePrsnt)(_ChnlPrsntMap[id - (Int32)ChannelIdExt.MinRFChId + (Int32)ChannelIdExt.MinFVTChId])));

            #endregion

            foreach (var id in ChannelIdExt.GetAnalogs())
            {
                var aprsnt = new AnalogPrsnt(id, this, null, Timebase);
                _ChnlPrsntMap.TryAdd(id, aprsnt);

                //!!!Let user bind RF channel to anylog channel
                //aprsnt.RadioFrequency = (RadioFrequencyPrsnt)(_ChnlPrsntMap[id + (Int32)ChannelIdExt.MinRFChId]);
            }

            if (Constants.ENABLE_Math)
            {
                foreach (var id in ChannelIdExt.GetMaths())
                {
                    var p = new MathPrsnt(id, this);
                    _ChnlPrsntMap.TryAdd(id, p);
                }
            }

            if (Constants.ENABLE_PowerAs)
            {
                foreach (var id in ChannelIdExt.GetPowerAnalysisMaths())
                {
                    _ChnlPrsntMap.TryAdd(id, new MathPrsnt(id, this));
                }
            }

            if (Constants.ENABLE_SDA)
            {
                foreach (var id in ChannelIdExt.GetJitterMaths())
                {
                    _ChnlPrsntMap.TryAdd(id, new MathPrsnt(id, this));
                }
            }

            if (Constants.ENABLE_BUS)
            {
                foreach (var id in ChannelIdExt.GetDecodes())
                {
                    _ChnlPrsntMap.TryAdd(id, new DecodePrsnt(id, this, Timebase));
                }

                if (TriggerPrsnt.Type == TriggerType.Serial)
                    TriggerPrsnt.GetOrMakeTrigger(this, TriggerPrsnt.Type, form?.TriggerVu);
            }

            if (Constants.ENABLE_LA)
                _ChnlPrsntMap.TryAdd(ChannelId.D0, new DigitalPrsnt(ChannelId.D0, this, Timebase));

            if (Constants.ENABLE_AWG)
            {
                _WfmGenPrsntMap.TryAdd(ChannelId.AWG1, new ArbWfmGenPrsnt(ChannelId.AWG1, this));
                _WfmGenPrsntMap.TryAdd(ChannelId.AWG2, new ArbWfmGenPrsnt(ChannelId.AWG2, this));
#if DEBUG
                _WfmGenPrsntMap.TryAdd(ChannelId.AWG3, new ArbWfmGenPrsnt(ChannelId.AWG3, this));
                _WfmGenPrsntMap.TryAdd(ChannelId.AWG4, new ArbWfmGenPrsnt(ChannelId.AWG4, this));
#endif
            }

            Voltmeter = new(this);

            ExceptionCapture = new ExceptionCapturePrsnt(this, null);
            foreach (var id in ChannelIdExt.GetCEMaths())
            {
                _ChnlPrsntMap.TryAdd(id, new MathPrsnt(id, this));
            }
            foreach (var id in ChannelIdExt.GetIRMaths())
            {
                _ChnlPrsntMap.TryAdd(id, new MathPrsnt(id, this));
            }
            foreach (var id in ChannelIdExt.GetMDMaths())
            {
                var p = new MathPrsnt(id, this);
                _ChnlPrsntMap.TryAdd(id, p);
            }

            Cymometer = new(this);

            if (form is not null)
            {
                View = form;
                View.Presenter = this;
            }
            UpdateAnalogZIndex();
            _Default = new Default(this);

            DefaultDsoPrsnt = this;

        }
        public void AddDemo(DemoType demoType)
        {
            _Demo.AddDemo(demoType);
        }
        public Boolean IsDemoMode()
        {
            return _Demo.Type != DemoType.None;
        }
        private static void LoadLastSetting()
        {
            //!!!Notice: Load Last Setting after the Model is initialized before the Presenter is initialized.
            try
            {
                FilePrsnt.LoadSetting(Environment.CurrentDirectory + "\\LastSettings.set");
            }
            catch (TypeInitializationException)
            {
                throw;
            }
            catch
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs("Cannot load last settings", EventBus.LogLevel.Error));
            }
        }

        private static Boolean CheckOnStarting()
        {
            try
            {
                if (!Directory.Exists(Constants.SET_DEF_PATH))
                {
                    Directory.CreateDirectory(Constants.SET_DEF_PATH);
                }

                if (!Directory.Exists(Constants.WFM_DEF_PATH))
                {
                    Directory.CreateDirectory(Constants.WFM_DEF_PATH);
                }

                if (!Directory.Exists(Constants.PIC_DEF_PATH))
                {
                    Directory.CreateDirectory(Constants.PIC_DEF_PATH);
                }

                if (!Directory.Exists(Constants.PRNT_DEF_PATH))
                {
                    Directory.CreateDirectory(Constants.PRNT_DEF_PATH);
                }

                if (!Directory.Exists(Constants.PASSFAIL_DEF_PATH))
                {
                    Directory.CreateDirectory(Constants.PASSFAIL_DEF_PATH);
                }

                if (!Directory.Exists(Constants.AWG_DEF_PATH))
                {
                    Directory.CreateDirectory(Constants.AWG_DEF_PATH);
                }

                if (!Directory.Exists(Constants.USERCODE_DEF_PATH))
                {
                    Directory.CreateDirectory(Constants.USERCODE_DEF_PATH);
                }

                if (!System.IO.File.Exists(Constants.SET_DEF_PATH + "\\" + Constants.FACTORY_SET_NAME + ".set"))
                {
                    FilePrsnt.SaveSetting(Constants.SET_DEF_PATH, Constants.FACTORY_SET_NAME, false, true);
                }
                //Need check the version of factory setting
            }
            catch
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"Error ocurrs when the function '{nameof(CheckOnStarting)}' is running", EventBus.LogLevel.Error));

                return false;
            }
            return true;
        }

        public void PushHdCmd(HdCmd cmd)
        {
            Hardware.HdCmdFactory.Push(cmd);
        }
        public void DoClear()
        {
            Dispatcher.DoClear();
        }

        private void Logs(String msg, String level)
        {
            LogLevel logLevel = LogLevel.Debug;
            switch (level)
            {
                case "Debug":
                    logLevel = LogLevel.Debug;
                    break;
                case "Info":
                    logLevel = LogLevel.Info;
                    break;
                case "Warn":
                    logLevel = LogLevel.Warn;
                    break;
                case "Error":
                    logLevel = LogLevel.Error;
                    break;
                default:
                    break;
            }

            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(msg, logLevel));
        }
        private Boolean _DefaultFlag = false;

        public void Default(Boolean isAsk = true)
        {
            if (!_DefaultFlag)
            {
                _DefaultFlag = true;
                try
                {
                    VuClear();
                    _Default?.SetDefault();
                    _DefaultFlag = false;
                }
                catch (Exception ex)
                {
                    _DefaultFlag = false;
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Warn));
                }
            }
        }

        public void VKClear()
        {
            try
            {
                Measure.ClearFlag = true;
                Measure.ClearHisFlag = true;
                Measure.ClearStrongFlag = true;
                Voltmeter.ResetStatistics();
                Cymometer.ResetStatistics();
                ClearPwr();
                var activerchs = TryGetRange(c => c.Active && c.Id.IsReference());
                foreach (var item in activerchs)
                {
                    item.Active = false;
                }
                SoftReset();
                DoClear();
                WeakTip.Default.Write("Clear Command", MsgTipId.ClearingSuccess, false, "", 2);
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Warn));
            }
        }

        private static void ClearPwr()
        {
            var powermodel = DsoModel.Default.PowerAnalysisModels.Where(x => x.Active);
            foreach (var item in powermodel)
            {
                item.Reset(item.Mode);
            }
        }
        public void PwrModulationClear()
        {
            var powermodel = DsoModel.Default.PowerAnalysisModels.Where(x => x.Active);
            foreach (var item in powermodel)
            {
                if (item.Mode == PowerAnalysis.PowerAnalysisOpt.Modulation)
                    item.Reset(item.Mode);
            }
        }

        public void VuClear()
        {
            View?.UpdateView(this, "VuClear");
        }

        /// <summary>
        /// 操作LA功能时，检查与其他功能的互斥
        /// </summary>
        /// <returns>检查的结果</returns>
        public Boolean DoLAMutex()
        {
            //暂停状态下禁止操作LA
            if (TriggerModel.State == SysState.Stop)
            {
                WeakTip.Default.Write("", MsgTipId.StopBanDigital, emergent: false, "", 5);
                return false;
            }

            if (FunctionLimit.DigitalFunctionLimit(MutexFunctionFlag) == false)
            {
                return false;
            }

            //LA操作状态下模拟通道3和4，必须处于关闭
            foreach (var ch in DsoModel.Default.AnalogChnls)
            {
                if (ch.Active && (ch.Id == ChannelId.C3 || ch.Id == ChannelId.C4))
                {
                    ch.Active = false;
                    WeakTip.Default.Write("", MsgTipId.DigitalOccupyAnalog, emergent: false, "", 5);
                }
            }

            return true;
        }

        /// <summary>
        /// 操作其他功能时，检查是否与LA功能互斥
        /// </summary>
        /// <param name="priorityThanLA">功能优先级是否大于LA</param>
        /// <returns>检查的结果</returns>
        public Boolean CheckLAMutex(Boolean priorityThanLA)
        {
            if (_IsCheckLAMutex)
            {
                return false;
            }
            _IsCheckLAMutex = true;

            if ((DsoModel.Default?.DigitalChnls.Any(d => d.Active) ?? false))
            {
                if (priorityThanLA)
                {
                    if (!StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DigitalMutex, MessageType.Asking))
                    {
                        _IsCheckLAMutex = false;
                        return false;
                    }
                }
                else
                {
                    WeakTip.Default.Write("", MsgTipId.DigitalOccupyAnalog, emergent: false, "", 5);
                    _IsCheckLAMutex = false;
                    return false;
                }
            }
            DsoModel.Default?.DigitalChnls.ToList().ForEach(d => d.Active = false);
            _IsCheckLAMutex = false;
            return true;
        }

        /// <summary>
        /// RST指令重置SystemCheck属性
        /// </summary>
        /// <param name="priorityThanLA">功能优先级是否大于LA</param>
        /// <returns>检查的结果</returns>

        public void SystemCheckDefault()
        {
            SystemCheck.SystemCheckRst();
        }
        public void SetProductModel(String productModel)
        {
            PlatformManager.Default.Platform.SetProductModel(productModel);
        }

        public String GetProductModel()
        {
            return PlatformManager.Default.Platform.GetProductModel();
        }

        public IDictionary<SerialProtocolType, OptionType> OptionProtocols => PlatformManager.Default.Platform.OptionProtocols;
    }
}

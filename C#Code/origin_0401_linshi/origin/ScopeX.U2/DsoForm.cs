using EventBus;
using Microsoft.Win32;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Structs;
using ScopeX.Controls.Language;
using ScopeX.Core;
using ScopeX.Core.Decode;
using ScopeX.Core.Model;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core.Presenter.RadioFrequency;
using ScopeX.Core.Tools;
using ScopeX.U2.Search;
using ScopeX.U2.Tools;
using ScopeX.UserControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ScopeX.U2
{
    public enum PopOrientation
    {
        Above,
        Under,
        Left,
        Right,
    }


    public partial class DsoForm : Form, IDsoView
    {
        public EventHandler<ChannelId> RemoveZoomRectangleAreaEventArgs;
        public EventHandler<(ChannelId, ChannelId)> AddZoomRectangleAreaEventArgs;
        /// <summary>
        /// badge控件使用的多语言资源名称，用于多语言切换使用。
        /// </summary>
        private Dictionary<string, ChannelType> _badgeTextLangKey = new Dictionary<string, ChannelType>();
        private Boolean _MathKeycodeCtrl = false;
        private Boolean _DecoedKeycodeCtrl = false;
        private Boolean _AWGKeycodeCtrl = false;
        private Boolean _LOGICKeycodeCtrl = false;
        private Boolean _RefKeycodeCtrl = false;
        private String _RefInitialDirectory = null;
        private FormsManager _FormsManager;
        private Boolean _IsShowForm = false;
        internal MultiWindowManager MultiWindowManager
        {
            get;
            private set;
        }

        internal IPanelManager PanelManager
        {
            get;
        }
        public string SoftWareVersion = string.Empty;

        public DsoForm()
        {
            this.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DsoFormTextInfo");
            HelpProcessManager.StartProcess();
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            SoftWareVersion = Application.ProductVersion;
            InitializeComponent();

            _ = new Logger();

            RunTimeStatistics.Default.InitLog(d =>
            {
                if (string.IsNullOrEmpty(d))
                    return;

                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs(d, LogLevel.Debug));
            });

            _FormsManager = new FormsManager(this);

            PanelManager = new PanelManageForm(Height * 2 / 3);//最大高度为主界面高度的2/3;
            try
            {
                Program.MoveToScreen(this, 1);
            }
            catch (Exception)
            {
                Program.MoveToScreen(this, -1);
            }

            EventBus.EventBroker.Instance.GetEvent<ProbeKeyType>().Subscrip((sender, args) =>
            {
                if (args.Data == ProbeKeyType.Clear)
                {
                    ProcessHotKeyCode(KeyCode.VK_CLEAR, 1);
                }
            });

            EventBus.EventBroker.Instance.GetEvent<HardwareWarningEventMessageArgs>().Subscrip((sender, args) =>
            {
                if (args.Data != null)
                {
                    //界面逻辑差异化处理
                    PlatformUIManager.Default.Platform.HardwareWarningEventHandler(args.Data);

                }
            });

            EventBus.EventBroker.Instance.GetEvent<Search.SearchInfoVisibleArgs>().Subscrip((sender, args) =>
            {
                if (sender is Search.SearchInfo info && args.Data != null && args.Data is SearchInfoVisibleArgs arg)
                {
                    if (arg.visible)
                    {
                        if (info != null)
                        {
                            PanelManager.Add(info);
                        }
                    }
                    else
                    {
                        if (info != null && PanelManager.Contains(info))
                            PanelManager.Remove(info);
                    }
                }
            });


            AnalogPrsnt.InitAnalogHighVoltageWarningEvent();
        }

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e)
        {
            this.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("DsoFormTextInfo");
            LanguageFactory.ChangeLanguage();
            var subcontrols = DsoInfoStrip.GetBadgeControls();
            if (subcontrols != null)
            {
                foreach (var item in subcontrols)
                {
                    if (item is Control c && _badgeTextLangKey.ContainsKey(c.Name))
                    {
                        c.Text = _badgeTextLangKey[c.Name].GetDescription_Lang();
                        c.Refresh();
                    }
                }
            }
            //重载数学库信息
            LoadMathCustomFormula();
        }

        public DsoPrsnt Presenter
        {
            get;
            set;
        }

        IDsoPrsnt IDsoView.Presenter
        {
            get => Presenter;
            set
            {
                Presenter = (DsoPrsnt)value;
                Presenter.SoftWareVersion = SoftWareVersion;
            }
        }

        public ITimebaseView TimebaseVu => DsoTopStrip;
        public IArtificialIntelligenceView ArtificialIntelligenceView => DsoTopStrip;

        public ITriggerView TriggerVu => DsoTopStrip;

        public IBadgeView GetBadge(ChannelId id) => DsoInfoStrip.GetBadge(id);

        public void UpdateView(Object sender, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(Update, new Object[] { sender, propertyName });
            }
            else
            {
                Update(sender, propertyName);
            }

            //!!!!!Response messsage when view object does not exist
            void Update(Object sender, String propertyName)
            {
                switch (sender)
                {
                    case MultiDomainPrsnt md:
                        if (propertyName == "ThreeDimensionalEnable")
                        {
                            if (md.ThreeDimensionalEnable)
                            {
                                (Program.Oscilloscope.View as DsoForm).TryAddThreeDimensionalUI(md);
                            }
                            else
                            {
                                (Program.Oscilloscope.View as DsoForm).TryRemoveThreeDimensionalUI(md);
                            }
                        }
                        break;
                    case MathPrsnt mp:
                        if (propertyName == nameof(mp.Active) && mp.Active)
                        {
                            TryAddMathUI(mp);
                            DsoPrsnt.FocusId = mp.Id;
                        }
                        break;
                    case ReferencePrsnt rp:
                        if (propertyName == nameof(rp.Active) && rp.Active)
                        {
                            TryAddRefUI(rp);
                            DsoPrsnt.FocusId = rp.Id;
                        }
                        break;
                    case RadioFrequencyPrsnt rfp:
                        if (propertyName == nameof(rfp.Active) && rfp.Active)
                        {
                            TryAddRFUI(rfp);
                            DsoPrsnt.FocusId = rfp.Id;
                        }
                        break;
                    case AmpVSTimePrsnt avtp:
                        if (propertyName == nameof(avtp.Active) && avtp.Active)
                        {
                            TryAddAVTUI(avtp);
                            DsoPrsnt.FocusId = avtp.Id;
                        }
                        break;
                    case PhaseVSTimePrsnt pvtp:
                        if (propertyName == nameof(pvtp.Active) && pvtp.Active)
                        {
                            TryAddPVTUI(pvtp);
                            DsoPrsnt.FocusId = pvtp.Id;
                        }
                        break;
                    case FrequencyVSTimePrsnt fvtp:
                        if (propertyName == nameof(fvtp.Active) && fvtp.Active)
                        {
                            TryAddFVTUI(fvtp);
                            DsoPrsnt.FocusId = fvtp.Id;
                        }
                        break;
                    case PhaseVSFrequencyPrsnt pvfp:
                        if (propertyName == nameof(pvfp.Active) && pvfp.Active)
                        {
                            TryAddPVFUI(pvfp);
                            DsoPrsnt.FocusId = pvfp.Id;
                        }
                        break;
                    case DecodePrsnt bp:
                        if (propertyName == nameof(bp.Active) && bp.Active)
                        {
                            if (DecodeFunctionLimit())
                            {
                                TryAddDecodeUI(bp);
                                DsoPrsnt.FocusId = bp.Id;
                            }
                            else
                            {
                                bp.Active = false;
                            }
                        }
                        break;
                    case DigitalPrsnt dp:
                        if (propertyName == nameof(dp.Active) && dp.Active)
                        {
                            TryAddDigiUI(dp);
                            DsoPrsnt.FocusId = dp.Id;
                        }
                        break;

                    case PassFailPrsnt pfp:
                        if (propertyName == nameof(pfp.Active))
                        {
                            ;
                        }
                        break;
                    case ArbWfmGenPrsnt arb:
                        if (propertyName == nameof(arb.Active))
                        {
                            TryAddAwgInfo(arb.Id);
                        }
                        break;
                    case LissajousPrsnt lp:
                        if (propertyName == nameof(lp.Active))
                        {
                            if (lp.Active)
                            {
                                TryAddLissajousUI(lp, lp.SourceX, lp.SourceY);
                            }
                            else
                            {
                                TryRemoveLissajousUI(lp);
                            }
                        }
                        break;
                    case SearchItemPrsnt sp:
                        var properties = propertyName.Split(":");
                        if (properties.Length > 1)
                        {
                            if (properties[1] == nameof(sp.Active))
                            {
                                if (sp.Active)
                                    SearchApp.Default.AddSearchInfo(sp);
                            }
                        }

                        break;
                    case MarkerItemPrsnt mip:
                        if (mip.AtuoMarkerActive && propertyName.Equals(mip.MarkerResultsTableEnable))
                        {
                            MarkerApp.Default.ShowDataTableForm(mip);
                        }
                        break;

                    case PowerAnalysisPrsnt pap:
                        if (propertyName == nameof(pap.Active))
                        {
                            if (pap.Active)
                            {
                                TryAddPowerInfo(pap, pap.Mode);
                            }
                        }
                        break;
                    case LocationAssistedPrsnt lap:
                        if (propertyName.Equals(nameof(TriggerPrsnt.State)))
                        {
                            SearchApp.Default.SystemStateChanged();
                        }
                        break;
                    case JitterPrsnt jp:
                        if (propertyName.Equals(nameof(jp.Active)))
                        {
                            if (jp.Active)
                            {
                                TryAddJitterInfo(jp);
                            }
                        }
                        break;
                    case VectorAnalysisPrsnt vap:
                        if (Constants.ENABLE_VSA
                            && ReferenceEquals(vap, Presenter.VectorAnalysis)
                            && propertyName == nameof(VectorAnalysisPrsnt.Enabled))
                        {
                            VectorAnalysisApp.Default?.SyncVsaErrParamInfoFormWithPresenterEnabled();
                        }
                        break;
                    case DsoPrsnt dso:
                        if (propertyName == nameof(Presenter.SysLanguage))
                        {
                            ScopeX.Controls.Language.LanguageManger.Instance.Language = LanguageFactory.GetLanguage(Program.Oscilloscope.SysLanguage);
                            HelpProcessManager.SendCommand((Int32)Program.Oscilloscope.SysLanguage);
                            System.Threading.Thread.Sleep(1000);//等待1000ms 重新加载Xml
                            HelpDocumentManager.Default.LoadDocumentInfo();
                        }
                        else if (propertyName == nameof(Presenter.ManufacturerAdatper))
                        {
                            var adpater = Presenter.ScpiAdapters.FirstOrDefault(adp => adp.Manufacturer == Presenter.ManufacturerAdatper);
                            Scpi.ScpiManager.SetAdapter(adpater);
                        }
                        break;
                }
                if (MultiWindowManager != null && propertyName == "FocusId")
                {
                    Presenter.UpdateAnalogZIndex();

                    var fig = MultiWindowManager.GetFigure(DsoPrsnt.FocusId);
                    if (fig == null)
                    {
                        return;
                    }
                    if (fig is BaseDisplayForm form)
                    {
                        form.Activate();
                    }
                }
                else if (MultiWindowManager != null && propertyName == "Default")
                {
                    //TouchController.EnableTouch(true);
                    Presenter.Display.TouchLock = false;
                    foreach (var item in Presenter.ArbWfmGens)
                    {
                        RemoveWaveformUI(item);
                    }
                    MultiWindowManager.RemoveAllWindows();
                    if (MultiWindowManager.MainFigure is WaveformGPUFigure waveform)
                    {
                        //waveform.CloseZoom();
                        waveform.SetSpecAreaBarVisible(false);
                    }
                    var webstatus = (WebServerStatus)Scpi.ScpiManager.IsWebModelRunning().ToInt();
                    if (webstatus == WebServerStatus.Close)
                    {
                        webstatus = (WebServerStatus)Scpi.ScpiManager.StartWebModel().ToInt();
                    }

                }
                else if (propertyName == "VuClear")
                {
                    CloseSettingForm();
                }
                else if (propertyName == nameof(TimebasePrsnt.StorageMode))
                {
                    // 快采功能关闭时，禁用所有的区域触发使能
                    if (Program.Oscilloscope.Timebase.StorageMode != AnaChnlStorageMode.Fast)
                    {
                        foreach (var item in Program.Oscilloscope.VisualTrigger.SelectedItems)
                        {
                            item.Enabled = false;
                        }
                    }
                }
                else if (propertyName == nameof(TrigSerialPrsnt.Source))
                {
                    DsoTopStrip.ReLoadSource();
                }
                else
                {
                }
            }
        }

        public Boolean TryAddExceptionCaptureInfo(ExceptionCapturePrsnt prsnt)
        {
            ExceptionCaptureInfo jtinfo = new()
            {
                Name = "ExceptionCaptureInfo",
                Text = "异常捕获",
                Dock = DockStyle.None,
                Presenter = Program.Oscilloscope.ExceptionCapture
            };
            prsnt.Active = true;
            prsnt.TryAddView(jtinfo);
            DsoInfoStrip.AddBadge(jtinfo);
            if (DsoInfoStrip.IsHandleCreated)
                jtinfo.OnBodyClicked();
            return true;
        }

        public void ManualUpdateTriggerState(SysState state = SysState.Stop) => DsoTopStrip.ManualUpdateTriggerState(state);

        private Boolean DecodeFunctionLimit()
        {
            if (Program.Oscilloscope.PwrAnalysisDictionary.Where(pa => pa.Value.Active).ToList().Count() > 0)
            {
                if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DecodeIsNotSupportedInPowerAnalysis, MessageType.Asking))
                {
                    foreach (var item in Program.Oscilloscope.PwrAnalysisDictionary)
                    {
                        item.Value.Active = false;
                    }
                }
                else
                {
                    return false;
                }
            }
            if (Program.Oscilloscope.Jitter.Active)
            {
                if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DecodeIsNotSupportedInJitter, MessageType.Asking))
                {
                    Program.Oscilloscope.Jitter.Active = false;
                }
                else
                {
                    return false;
                }
            }
            if (Program.Oscilloscope.PassFail.Active)
            {
                if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DecodeIsNotSupportedInPassFail, MessageType.Asking))
                {
                    Program.Oscilloscope.PassFail.Active = false;
                }
                else
                {
                    return false;
                }
            }
            if (Program.Oscilloscope.Timebase.SegmentActive)
            {
                if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.DecodeIsNotSupportedInSegment, MessageType.Asking))
                {
                    Program.Oscilloscope.Timebase.SegmentActive = false;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        private const Int32 WM_DPICHANGED = 0x02E0;
        protected override void DefWndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case NativeMethods.WM_USERKEYDOWN:
                    switch ((Int32)msg.WParam)
                    {
                        //keyboard message
                        case 12:
                            ProcessKeycode((Int32)msg.LParam);
                            break;
                    }
                    break;
                case NativeMethods.WM_DEVICECHANGE:
                    string portName = null;
                    int offset = Marshal.SizeOf<NativeMethods.DEV_BROADCAST_HDR>();
                    switch ((int)msg.WParam)
                    {
                        case NativeMethods.DBT_DEVICEARRIVAL:
                            var lpdb = (NativeMethods.DEV_BROADCAST_HDR)Marshal.PtrToStructure(msg.LParam, typeof(NativeMethods.DEV_BROADCAST_HDR));
                            if (lpdb.Dbch_DeviceType != NativeMethods.DBCH_DEVICETYPE.DBT_DEVTYP_PORT)
                                break;
                            portName = Marshal.PtrToStringAuto(msg.LParam + offset, 6).Trim('\0');
                            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs($"串口：{portName}插入", LogLevel.Debug));
                            EventBroker.Instance.GetEvent<SerialPortRecord>().Publish(this, new SerialPortRecord(SerialPortChangeType.Insert, portName));
                            break;
                        case NativeMethods.DBT_DEVICEREMOVECOMPLETE:
                            var lpdb_remove = (NativeMethods.DEV_BROADCAST_HDR)Marshal.PtrToStructure(msg.LParam, typeof(NativeMethods.DEV_BROADCAST_HDR));
                            if (lpdb_remove.Dbch_DeviceType != NativeMethods.DBCH_DEVICETYPE.DBT_DEVTYP_PORT)
                                break;
                            portName = Marshal.PtrToStringAuto(msg.LParam + offset, 6).Trim('\0');
                            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs($"串口：{portName}拔出", LogLevel.Debug));
                            EventBroker.Instance.GetEvent<SerialPortRecord>().Publish(this, new SerialPortRecord(SerialPortChangeType.Remove, portName));
                            break;
                    }
                    break;
                case WM_DPICHANGED:
                    SuitResolution();
                    break;
                default:
                    base.DefWndProc(ref msg);
                    break;
            }
        }
        public async Task<AsyncTaskResult> StartScpiManagerAsync(String mark)
        {
            return await Task.Run(() =>
            {
                DateTime dt = DateTime.Now;
                const Int32 maxRetryCount = 5;
                const Int32 retryDelayMs = 300;
                Boolean result = false;
                String errorMsg = String.Empty;

                // Some VISA/USBTMC resources may appear with a short delay.
                Thread.Sleep(200);
                for (Int32 retry = 1; retry <= maxRetryCount; retry++)
                {
                    try
                    {
                        result = Scpi.ScpiManager.Start(Presenter, Constants.ENABLE_LXI, Constants.ENABLE_WEBCORE, Constants.ENABLE_USB);
                        if (result)
                            break;
                    }
                    catch (Exception ex)
                    {
                        errorMsg = ex.Message;
                        EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs($"SCPI Start异常(第{retry}/{maxRetryCount}次): {ex.Message}", LogLevel.Warn));
                    }

                    if (retry < maxRetryCount)
                    {
                        Thread.Sleep(retryDelayMs);
                    }
                }

                if (result == false)
                {
                    ComModel.ErrorCode.ErrorType = ErrorType.S_SCPIStart_Error_0002;
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs($"SCPI Start失败: 重试{maxRetryCount}次后仍未成功", LogLevel.Warn));
                }
                return new AsyncTaskResult() { Success = result, Mark = mark, ErrorMsg = errorMsg, UsedMilliseconds = DateTime.Now.Subtract(dt).TotalMilliseconds };
            }
            );
        }

        public async Task<AsyncTaskResult> InitAsync(Form form, String mark)
        {
            return await Task.Run(() =>
            {
                DateTime dt = DateTime.Now;
                form.Invoke(new Action(() =>
                {
                    //启动关闭其他右键菜单
                    CloseAllContextMenusWithESC();
                    InitOnLoad();
                    //启动关闭其他右键菜单
                    CloseAllContextMenusWithESC();
                }));
                return new AsyncTaskResult() { Success = true, Mark = mark, ErrorMsg = "", UsedMilliseconds = DateTime.Now.Subtract(dt).TotalMilliseconds };
            }
            );
        }

        private void InitOnLoad()
        {
            SystemEvents.SessionEnding += (_, _) =>
            {
                Close();
            };
            AppDomain.CurrentDomain.ProcessExit += (_, _) =>
            {
                Close();
            };

            DockPanelInit();

            InitHotKnobFunc();
            CursorApp.Default = new(Presenter.Cursor);//提前初始化
            Presenter.Cursor?.TryAddView(DsoTopStrip);
            MultiWindowManager = new MultiWindowManager(Presenter, WindowDockPanel);
            Keyboard.Default.ProcessKey = (keycode) =>
            {
                if (DsoPrsnt.KeyBoardLockEnable && !DsoPrsnt.KeyBoardForbidLockKeys.Contains(keycode.Value.Code))
                    return;

                if (InvokeRequired)
                {
                    BeginInvoke(new Action<Int32, Int32>(ProcessKeycode), new Object[] { keycode.Value.Code, keycode.Value.Step });
                }
                else
                {
                    ProcessKeycode(keycode.Value.Code, keycode.Value.Step);
                }
            };

            WeakTip.Default.Init((e) =>
                EventBroker.Instance.GetEvent<WeakTipEventArgs>().Publish(this, e));

            StrongTip.Default.Init((e) =>
                EventBroker.Instance.GetEvent<StrongTipEventArgs, DialogResult>().Publish(this, e) == DialogResult.Yes);

            SegmentApp.Default = new(Presenter.Timebase);

            MeasureApp.Default = new(Presenter.Measure);

            PrintApp.Default = new(Presenter.Print);

            FileApp.Default = new(Presenter.File);

            RemovableStorageManager.Default = new(Presenter.File);

            DecodeApp.Default = new(Presenter.TryGetRange(c => c.Id.IsDecode()));

            ManufacturerAdatperApp.Default = new();

            _RefInitialDirectory = Presenter.File.WfmPath;

            PassFailApp.Default = new(Presenter.PassFail);

            PowerAnalysisApp.Default = new();

            SearchApp.Default = new(Presenter.Search);
            Presenter.Search?.TryAddView(DsoTopStrip);

            MarkerApp.Default = new(Presenter.Markers);

            JitterApp.Default = new(Presenter.Jitter);

           VectorAnalysisApp.Default = new(Presenter.VectorAnalysis);

            ArtificialIntelligenceApp.Default = new(Presenter.ArtificialIntelligence);

            SegmentApp.Default.Init();
            CursorApp.Default.Init();
            MeasureApp.Default.Init();
            ArtificialIntelligenceApp.Default.Init();
            Presenter.AutoCalibration.Use2SCPI = AutoCalibration;
            InitBadge();

            //!!!zqc 11.05
            var pm = PanelManager as PanelManageForm;
            pm.Show(
                new Point(Width - pm.Width - 600,    //离有边界偏移150 
                DsoTopStrip.Height + 2));

            //设置控件颜色
            TlpMain.BackColor = AppStyleConfig.DefaultAreaBackColor;

            InitWidgets();

            HelpDocumentManager.Default = new();

            //!!!When DsoResultStrip's UpdateView did not invoked, here force DsoResultStrip refreshed.
            DsoResultStrip.MeasPresenter = Presenter.Measure;
            Presenter.Measure.TryAddView(DsoResultStrip);
            DsoResultStrip.CymometerPresenter = Presenter.Cymometer;
            Presenter.Cymometer.TryAddView(DsoResultStrip);
            DsoResultStrip.VoltmeterPresenter = Presenter.Voltmeter;
            Presenter.Voltmeter.TryAddView(DsoResultStrip);
            DsoResultStrip.Refresh();
            //!!!Let App button gets focus when starting.
            DsoTopStrip.Select();

            SuitResolution();
            Presenter.SoftWareVersion = Application.ProductVersion;
            if (!PlatformUIManager.Default.Platform.Attribute.SupportGetOrSetBrightness)
            {
                Presenter.Display.SetBrightness = null;
                Presenter.Display.GetBrightness = null;
            }
            else
            {
                Presenter.Display.SetBrightness = ScreenUtility.Default.SetBrigtness;
                Presenter.Display.GetBrightness = ScreenUtility.Default.GetCurrentBrigtness;
                Presenter.Display.SetTouchable = ScreenUtility.Default.TrySetTouchable;
                Presenter.Display.GetTouchable = ScreenUtility.Default.TryGetTouchable;
            }
            BringToFront();
            Presenter.Display.SetContrast = ScreenUtility.Default.SetContrast;
            Presenter.Display.GetContrast = ScreenUtility.Default.GetCurrentContrast;
            Presenter.Display.SetLocalTime = SysTimeUtility.Default.SetLocalTime;
            Presenter.Display.GetLocalTime = SysTimeUtility.Default.GetLocalTime;

            Presenter.VuMinimize = SetFormMinimize;
            Presenter.VuClose = SetFormClose;
            Presenter.VuShutDowm = SetFormShutDowm;
            Presenter.VuRestart = SetFormRestart;
            Presenter.VuLogout = SetFormLogout;
            HelpProcessManager.SendCommand((Int32)Program.Oscilloscope.SysLanguage);
            System.Threading.Thread.Sleep(1000);//等待1000ms 重新加载Xml
            HelpDocumentManager.Default.LoadDocumentInfo();

            LoadMathCustomFormula();
        }

        #region Operation app and computer

        public void SetFormMinimize()
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => ProcessKeycode(KeyCode.VK_MINIMIZE));
            }
            else
                ProcessKeycode(KeyCode.VK_MINIMIZE);
        }

        public void SetFormClose()
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => Close());
            }
            else
                Close();
        }

        public void SetFormShutDowm()
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => ProcessKeycode(-KeyCode.VK_SHUTDOWN));
            }
            else
                ProcessKeycode(-KeyCode.VK_SHUTDOWN);
        }

        public void SetFormRestart()
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => ProcessKeycode(-KeyCode.VK_RESTART));
            }
            else
                ProcessKeycode(-KeyCode.VK_RESTART);
        }

        public void SetFormLogout()
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => ProcessKeycode(-KeyCode.VK_LOGOUT));
            }
            else
                ProcessKeycode(-KeyCode.VK_LOGOUT);
        }

        #endregion Operation app and computer

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LanguageFactory.CacheFormLanguageControls(this);
            Presenter.HardWareVersion = VersionManager.HardWareVersion;

        }

        /// <summary>
        /// 加载数学自定义公式
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private Boolean LoadMathCustomFormula(String path = "")
        {
            var mathfunctions = new Dictionary<String, MathFormulaInfo>();

            XmlReader reader;
            //Load CustomFormula.xml
            try
            {
                if (System.IO.File.Exists(path))
                {
                    reader = new XmlTextReader(path);
                }
                else
                {

                    Type type = typeof(CustomFormulaForm);
                    string langstr = "";
                    if (LanguageFactory.Current == Language.English)
                        langstr = "_en";
                    if (LanguageFactory.Current == Language.German)
                        langstr = "_ge";
                    if (LanguageFactory.Current == Language.French)
                        langstr = "_fr";
                    if (LanguageFactory.Current == Language.Italian)
                        langstr = "_it";
                    if (LanguageFactory.Current == Language.Spanish)
                        langstr = "_sp";

                    string xmlpath = $"{type.Namespace}.Resources.Language.CustomFormula.CustomFormula{langstr}.xml";
                    Stream sm = type.Assembly.GetManifestResourceStream(xmlpath);
                    reader = XmlReader.Create(sm);
                }

                reader.ReadToFollowing("SourceItems");
                XmlReader srcreader = reader.ReadSubtree();
                while (srcreader.Read())
                {
                    if (srcreader.NodeType == XmlNodeType.Element && srcreader.Name == "Item")
                    {
                        var name = srcreader.GetAttribute("name");
                        if (Enum.TryParse<ChannelId>(name, out var id))
                        {
                            switch (name[0])
                            {
                                case 'C':
                                    if (!ChannelIdExt.IsAnalog(id))
                                    {
                                        continue;
                                    }

                                    break;
                                case 'M':
                                    if (!ChannelIdExt.IsMath(id))
                                    {
                                        continue;
                                    }

                                    break;
                                case 'R':
                                    if (!ChannelIdExt.IsReference(id))
                                    {
                                        continue;
                                    }

                                    break;
                                case 'D':
                                    if (PlatformUIManager.Default.Platform.Attribute.SupportDigital)
                                    {
                                        if (!ChannelIdExt.IsDigital(id))
                                        {
                                            continue;
                                        }
                                        break;
                                    }
                                    // UPO7000L没有数字通道
                                    continue;
                                default:
                                    continue;
                            }

                            AddOneFunction(srcreader, MathDefineFormulaType.Source);
                        }
                    }
                }

                reader.ReadToFollowing("NumbericItems");
                XmlReader numreader = reader.ReadSubtree();
                while (numreader.Read())
                {
                    if (numreader.NodeType == XmlNodeType.Element && numreader.Name == "Item")
                    {
                        AddOneFunction(numreader, MathDefineFormulaType.Numberic);
                    }
                }

                reader.ReadToFollowing("FuncItems");
                XmlReader funcreader = reader.ReadSubtree();
                while (funcreader.Read())
                {
                    if (funcreader.NodeType == XmlNodeType.Element && funcreader.Name == "Item")
                    {
                        AddOneFunction(funcreader, MathDefineFormulaType.Func);
                    }
                }
            }
            catch
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs($"The load of CustomFormula.xml fails or its format is wrong!", EventBus.LogLevel.Error));
#if DEBUG
                throw;
#else
                return false;
#endif
            }
            Presenter.MathFormulaCollections = mathfunctions as IReadOnlyDictionary<String, MathFormulaInfo>;
            return true;

            void AddOneFunction(XmlReader reader, MathDefineFormulaType type)
            {
                var imgkey = reader.GetAttribute("image");
                if (String.IsNullOrEmpty(imgkey))
                {
                    imgkey = reader.GetAttribute("name");
                }

                mathfunctions.Add(reader.GetAttribute("id"),
                    new MathFormulaInfo(
                        type,
                        reader.GetAttribute("name"),
                        reader.GetAttribute("symbol"),
                        reader.GetAttribute("expression"),
                        imgkey,
                        reader.GetAttribute("info")));
            }
        }
        public void OnMeasurementClick(Func<FloatForm> creator) => MakeOperateForm("Measure", DsoResultStrip.PointToScreen(new Point(0, 0)), PopOrientation.Above, creator);
        private void DockPanelInit()
        {
            VS2015LightTheme.ColorPalette.MainWindowActive.Background = AppStyleConfig.MainWindowActiveBackground;// Color.Black;
            VS2015LightTheme.ColorPalette.ToolWindowCaptionActive.Background = AppStyleConfig.ToolWindowCaptionActiveBackground;//Color.FromArgb(0x3e, 0x3e, 0x3e);
            VS2015LightTheme.ColorPalette.TabSelectedInactive.Background = AppStyleConfig.TabSelectedInactiveBackground;//Color.FromArgb(38, 38, 46);
            VS2015LightTheme.ColorPalette.TabSelectedActive.Background = AppStyleConfig.TabSelectedActiveBackground;//Color.FromArgb(0, 209, 255);
            WindowDockPanel.Theme = VS2015LightTheme;
            WindowDockPanel.Theme.Skin.DockPaneStripSkin.TextFont = new Font("MiSans", 12.5F, FontStyle.Regular, GraphicsUnit.Point);
            WindowDockPanel.Theme.ColorPalette.TabSelectedActive.Text = AppStyleConfig.TabSelectedActiveForeColor;//Color.Black;
            WindowDockPanel.Theme.ColorPalette.TabSelectedInactive.Text = AppStyleConfig.TabSelectedInactiveForeColor;//Color.White;
        }

        private static void InitWidgets()
        {
            WidgetPrsnt.SetMousePosHandler = (Int32 x, Int32 y) => Cursor.Position = new(x, y); //NativeMethods.SetCursorPos(x, y);

            WidgetPrsnt.MouseEventHandler = (UInt32 flag, Int32 d) => _ = NativeMethods.mouse_event(flag, 0, 0, 0, 0);

            WidgetPrsnt.KeyboradEventHandler = (Byte key, Int32 flags) => NativeMethods.keybd_event(key, 0, flags, 0);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            DsoPrsnt.IsDsoClosing = true;
            LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
            FilePrsnt.SaveSetting(Environment.CurrentDirectory, "LastSettings", false, true);
            DsoClosing();
        }

        public void DsoClosing()
        {
#if DEBUG && SaveLanguage
            LanguageFactory.Save2Disk("templanguage.txt");
#endif
            Presenter.Display.TouchLock = false;
            Presenter.Cancel();
            MultiWindowManager?.Dispose();
            RemovableStorageManager.Default?.Dispose();
            Scpi.ScpiManager.Close();
            Presenter.Close();
        }

        private void SuitResolution()
        {
            Screen cs = Screen.FromHandle(Handle);

            Size = cs.Bounds.Size;
            Location = cs.Bounds.Location;
        }

        #region dso keyboard
        private readonly Dictionary<String, IHotKnob> _HotKnobFuncTable = new();

        private void InitHotKnobFunc()
        {
            _HotKnobFuncTable.Add(nameof(DisplayHotKnob), new DisplayHotKnob(Presenter.Display));
            _HotKnobFuncTable.Add(nameof(CursorHotKnob), new CursorHotKnob(Presenter.Cursor));
            _HotKnobFuncTable.Add(nameof(TriggerHotKnob), new TriggerHotKnob(Presenter.CurrentTrigger));

            _HotKnob = _HotKnobFuncTable[nameof(CursorHotKnob)];
        }

        private IHotKnob _HotKnob;

        private void ProcessHotKeyCode(Int32 keyCode, Int32 keyStep = 1)
        {
            //判断系统自检模式是否已开
            if (ProcessSystemCheckMode(keyCode))
            {
                return;
            }
            switch (keyCode)
            {
                case KeyCode.KNOB_YPOS_LEFT:
                    {
                        if (DsoPrsnt.FocusId.IsMath() && Program.Oscilloscope.TryGetChannel(DsoPrsnt.FocusId, out var prst) && prst is MathPrsnt math && math.AutoScale)
                            return;
                        if (Presenter.TryGetChannel(DsoPrsnt.FocusId, out var cp))
                        {
                            if (cp.Id.IsDigital())
                            {
                                var dcp = (DigitalPrsnt)cp;
                                var bitheight = dcp.BitHeight;
                                keyStep = (int)(bitheight * Math.Ceiling(keyStep / bitheight));
                            }
                            cp.PosIndexBymDiv -= keyStep;
                        }
                    }
                    break;

                case KeyCode.KNOB_YPOS_RIGHT:
                    {
                        if (DsoPrsnt.FocusId.IsMath() && Program.Oscilloscope.TryGetChannel(DsoPrsnt.FocusId, out var prst) && prst is MathPrsnt math && math.AutoScale)
                            return;
                        if (Presenter.TryGetChannel(DsoPrsnt.FocusId, out var cp))
                        {
                            if (cp.Id.IsDigital())
                            {
                                var dcp = (DigitalPrsnt)cp;
                                var bitheight = dcp.BitHeight;
                                keyStep = (int)(bitheight * Math.Ceiling(keyStep / bitheight));
                            }
                            cp.PosIndexBymDiv += keyStep;
                        }
                    }
                    break;

                case KeyCode.KNOB_YPOS_SELECT:
                    {
                        if (DsoPrsnt.FocusId.IsMath() && Program.Oscilloscope.TryGetChannel(DsoPrsnt.FocusId, out var prst) && prst is MathPrsnt math && math.AutoScale)
                            return;
                        if (Presenter.TryGetChannel(DsoPrsnt.FocusId, out var cp))
                        {
                            cp.ResetPosIndex();
                        }
                    }
                    break;


                case KeyCode.KNOB_CH1YPOS_LEFT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C1, out var cp))
                        {
                            cp.PosIndexBymDiv -= keyStep;
                        }
                    }
                    break;

                case KeyCode.KNOB_CH1YPOS_RIGHT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C1, out var cp))
                        {
                            cp.PosIndexBymDiv += keyStep;
                        }
                    }
                    break;

                case KeyCode.KNOB_CH1YPOS_SELECT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C1, out var cp))
                        {
                            cp.ResetPosIndex();
                        }
                    }
                    break;
                case KeyCode.KNOB_CH2YPOS_LEFT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C2, out var cp))
                        {
                            cp.PosIndexBymDiv -= keyStep;
                        }
                    }
                    break;

                case KeyCode.KNOB_CH2YPOS_RIGHT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C2, out var cp))
                        {
                            cp.PosIndexBymDiv += keyStep;
                        }
                    }
                    break;

                case KeyCode.KNOB_CH2YPOS_SELECT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C2, out var cp))
                        {
                            cp.ResetPosIndex();
                        }
                    }
                    break;

                case KeyCode.KNOB_CH3YPOS_LEFT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C3, out var cp))
                        {
                            cp.PosIndexBymDiv -= keyStep;
                        }
                    }
                    break;

                case KeyCode.KNOB_CH3YPOS_RIGHT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C3, out var cp))
                        {
                            cp.PosIndexBymDiv += keyStep;
                        }
                    }
                    break;

                case KeyCode.KNOB_CH3YPOS_SELECT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C3, out var cp))
                        {
                            cp.ResetPosIndex();
                        }
                    }
                    break;

                case KeyCode.KNOB_CH4YPOS_LEFT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C4, out var cp))
                        {
                            cp.PosIndexBymDiv -= keyStep;
                        }
                    }
                    break;

                case KeyCode.KNOB_CH4YPOS_RIGHT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C4, out var cp))
                        {
                            cp.PosIndexBymDiv += keyStep;
                        }
                    }
                    break;

                case KeyCode.KNOB_CH4YPOS_SELECT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C4, out var cp))
                        {
                            cp.ResetPosIndex();
                        }
                    }
                    break;
                case KeyCode.KNOB_YLEVEL_LEFT:
                    {
                        if (DsoPrsnt.FocusId.IsMath() && Program.Oscilloscope.TryGetChannel(DsoPrsnt.FocusId, out var prst) && prst is MathPrsnt math && math.AutoScale)
                            return;
                        if (Presenter.TryGetChannel(DsoPrsnt.FocusId, out var cp))
                        {
                            if ((cp is AnalogPrsnt) && (cp as AnalogPrsnt).Ylevel_SelectStatus)
                            {
                                (cp as AnalogPrsnt).SetScaleValueBymV(keyStep);
                            }
                            else if ((cp is ReferencePrsnt) && (cp as ReferencePrsnt).Ylevel_SelectStatus)
                            {
                                (cp as ReferencePrsnt).SetScaleValueBymV(keyStep);
                            }
                            else
                            {
                                cp.ScaleIndex += keyStep;
                            }
                        }
                    }
                    break;
                case KeyCode.KNOB_YLEVEL_RIGHT:
                    {
                        if (DsoPrsnt.FocusId.IsMath() && Program.Oscilloscope.TryGetChannel(DsoPrsnt.FocusId, out var prst) && prst is MathPrsnt math && math.AutoScale)
                            return;
                        if (Presenter.TryGetChannel(DsoPrsnt.FocusId, out var cp))
                        {
                            if ((cp is AnalogPrsnt) && (cp as AnalogPrsnt).Ylevel_SelectStatus)
                            {
                                (cp as AnalogPrsnt).SetScaleValueBymV(-keyStep);
                            }
                            else if ((cp is ReferencePrsnt) && (cp as ReferencePrsnt).Ylevel_SelectStatus)
                            {
                                (cp as ReferencePrsnt).SetScaleValueBymV(-keyStep);
                            }
                            else
                            {
                                cp.ScaleIndex -= keyStep;
                            }
                        }
                    }
                    break;
                //垂直挡位选中，或取消
                case KeyCode.KNOB_YLEVEL_SELECT:
                    {
                        if (DsoPrsnt.FocusId.IsMath() && Program.Oscilloscope.TryGetChannel(DsoPrsnt.FocusId, out var prst) && prst is MathPrsnt math && math.AutoScale)
                            return;
                        ChannelId FocusId = DsoPrsnt.FocusId;
                        if (Presenter.TryGetChannel(FocusId, out var cp))
                        {
                            if (cp is AnalogPrsnt)
                            {
                                (cp as AnalogPrsnt).Ylevel_SelectStatus = !(cp as AnalogPrsnt).Ylevel_SelectStatus;
                            }
                            if (cp is ReferencePrsnt)
                            {
                                (cp as ReferencePrsnt).Ylevel_SelectStatus = !(cp as ReferencePrsnt).Ylevel_SelectStatus;
                            }
                        }
                    }
                    break;

                case KeyCode.KNOB_CH1YLEVEL_LEFT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C1, out var cp))
                        {
                            cp.ScaleIndex += keyStep;
                        }
                    }
                    break;
                case KeyCode.KNOB_CH1YLEVEL_RIGHT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C1, out var cp))
                        {
                            cp.ScaleIndex -= keyStep;
                        }
                    }
                    break;
                case KeyCode.KNOB_CH2YLEVEL_LEFT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C2, out var cp))
                        {
                            cp.ScaleIndex += keyStep;
                        }
                    }
                    break;
                case KeyCode.KNOB_CH2YLEVEL_RIGHT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C2, out var cp))
                        {
                            cp.ScaleIndex -= keyStep;
                        }
                    }
                    break;
                case KeyCode.KNOB_CH3YLEVEL_LEFT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C3, out var cp))
                        {
                            cp.ScaleIndex += keyStep;
                        }
                    }
                    break;
                case KeyCode.KNOB_CH3YLEVEL_RIGHT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C3, out var cp))
                        {
                            cp.ScaleIndex -= keyStep;
                        }
                    }
                    break;
                case KeyCode.KNOB_CH4YLEVEL_LEFT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C4, out var cp))
                        {
                            cp.ScaleIndex += keyStep;
                        }
                    }
                    break;
                case KeyCode.KNOB_CH4YLEVEL_RIGHT:
                    {
                        if (Presenter.TryGetChannel(ChannelId.C4, out var cp))
                        {
                            cp.ScaleIndex -= keyStep;
                        }
                    }
                    break;

                case KeyCode.KNOB_XPOS_LEFT:
                    {
                        Presenter.TryGetChannel(DsoPrsnt.FocusId, out var chl);
                        if (chl is ReferencePrsnt refprsnt)
                        {
                            refprsnt.Sampling.PosIndexBymDiv -= keyStep;
                        }
                        else
                        {
                            Presenter.Timebase.PosIndexBymDiv -= keyStep;
                        }
                    }
                    break;
                case KeyCode.KNOB_XPOS_RIGHT:
                    {
                        Presenter.TryGetChannel(DsoPrsnt.FocusId, out var chl);
                        if (chl is ReferencePrsnt refprsnt)
                        {
                            refprsnt.Sampling.PosIndexBymDiv += keyStep;
                        }
                        else
                        {
                            Presenter.Timebase.PosIndexBymDiv += keyStep;
                        }
                    }
                    break;
                case KeyCode.KNOB_XPOS_SELECT:
                    {
                        Presenter.Timebase.ResetPosIndex();
                    }
                    break;
                case KeyCode.KNOB_XLEVEL_LEFT:
                    {
                        Presenter.TryGetChannel(DsoPrsnt.FocusId, out var chl);
                        if (chl is ReferencePrsnt refprsnt)
                        {
                            refprsnt.Sampling.ScaleIndex += keyStep;
                        }
                        else
                        {
                            Presenter.Timebase.ScaleIndex += keyStep;
                        }
                    }
                    break;
                case KeyCode.KNOB_XLEVEL_RIGHT:
                    {
                        Presenter.TryGetChannel(DsoPrsnt.FocusId, out var chl);
                        if (chl is ReferencePrsnt refprsnt)
                        {
                            refprsnt.Sampling.ScaleIndex -= keyStep;
                        }
                        else
                        {
                            Presenter.Timebase.ScaleIndex -= keyStep;
                        }
                    }
                    break;
                case KeyCode.KNOB_TRIG_YPOS_LEFT:
                    if (Presenter.CurrentTrigger is TrigSustainTimePrsnt trgsl)
                    {
                        var pos = trgsl.GetPosIndex(DsoPrsnt.FocusId);
                        pos -= keyStep;
                        trgsl.SetPosIndex(DsoPrsnt.FocusId, pos);
                    }
                    else if (Presenter.CurrentTrigger is TrigPatPrsnt trgpl)
                    {
                        var pos = trgpl.GetPosIndex(DsoPrsnt.FocusId);
                        pos -= keyStep;
                        trgpl.SetPosIndex(DsoPrsnt.FocusId, pos);
                    }
                    else if (Presenter.CurrentTrigger is TrigSetupHoldPrsnt trgshl)
                    {
                        if (trgshl.ClkSource == DsoPrsnt.FocusId)
                        {
                            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(trgshl.ClkSource, out IChnlPrsnt t) && t.Active)
                                trgshl.ClkRelPosIndex -= keyStep;
                        }
                        if (trgshl.DataSource == DsoPrsnt.FocusId)
                        {
                            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(trgshl.DataSource, out IChnlPrsnt t) && t.Active)
                                trgshl.UpperDataPosIndex -= keyStep;
                        }
                    }
                    else if (Presenter.CurrentTrigger is TrigDelayPrsnt trgdl)
                    {
                        if (trgdl.SourceOne == DsoPrsnt.FocusId)
                        {
                            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(trgdl.SourceOne, out IChnlPrsnt t) && t.Active)
                                trgdl.RelPosUpperIndex -= keyStep;
                        }
                        if (trgdl.SourceTwo == DsoPrsnt.FocusId)
                        {
                            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(trgdl.SourceTwo, out IChnlPrsnt t) && t.Active)
                                trgdl.DataRelPosIndex -= keyStep;
                        }
                    }
                    else
                    {
                        Presenter.CurrentTrigger.PosIndex -= keyStep;
                    }
                    break;
                case KeyCode.KNOB_TRIG_YPOS_RIGHT:
                    if (Presenter.CurrentTrigger is TrigSustainTimePrsnt trgsr)
                    {
                        var pos = trgsr.GetPosIndex(DsoPrsnt.FocusId);
                        pos += keyStep;
                        trgsr.SetPosIndex(DsoPrsnt.FocusId, pos);
                    }
                    else if (Presenter.CurrentTrigger is TrigPatPrsnt trgpr)
                    {
                        var pos = trgpr.GetPosIndex(DsoPrsnt.FocusId);
                        pos += keyStep;
                        trgpr.SetPosIndex(DsoPrsnt.FocusId, pos);
                    }
                    else if (Presenter.CurrentTrigger is TrigSetupHoldPrsnt trgshr)
                    {
                        if (trgshr.ClkSource == DsoPrsnt.FocusId)
                        {
                            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(trgshr.ClkSource, out IChnlPrsnt t) && t.Active)
                                trgshr.ClkRelPosIndex += keyStep;
                        }
                        if (trgshr.DataSource == DsoPrsnt.FocusId)
                        {
                            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(trgshr.DataSource, out IChnlPrsnt t) && t.Active)
                                trgshr.UpperDataPosIndex += keyStep;
                        }
                    }
                    else if (Presenter.CurrentTrigger is TrigDelayPrsnt trgdr)
                    {
                        if (trgdr.SourceOne == DsoPrsnt.FocusId)
                        {
                            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(trgdr.SourceOne, out IChnlPrsnt t) && t.Active)
                                trgdr.RelPosUpperIndex += keyStep;
                        }
                        if (trgdr.SourceTwo == DsoPrsnt.FocusId)
                        {
                            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(trgdr.SourceTwo, out IChnlPrsnt t) && t.Active)
                                trgdr.DataRelPosIndex += keyStep;
                        }
                    }
                    else
                    {
                        Presenter.CurrentTrigger.PosIndex += keyStep;
                    }
                    break;
                case KeyCode.KNOB_TRIG_YPOS_SELECT:
                    switch (Presenter.CurrentTrigger)
                    {
                        case TrigEdgePrsnt trigEdgePrsnt:
                            switch (trigEdgePrsnt.Coupling)
                            {
                                case TriggerCoupling.AC:
                                case TriggerCoupling.LFR:
                                    trigEdgePrsnt.SetPosIndexCenterZero();
                                    break;
                                case TriggerCoupling.HFR:
                                default:
                                    trigEdgePrsnt.SetPosIndexCenter();
                                    break;
                            }
                            break;
                        case TrigSingleSrcPrsnt tsrp:
                            tsrp.SetPosIndexCenter();
                            break;
                        case TrigMultiLevelPrsnt tmlp:
                            tmlp.PosSwitch ^= 1;
                            if (Presenter.CurrentTrigger is TrigSustainTimePrsnt trgst)
                            {
                                trgst.SetCompPosCenter(DsoPrsnt.FocusId);
                            }
                            else if (Presenter.CurrentTrigger is TrigDelayPrsnt trgd)
                            {
                                if (trgd.SourceOne == DsoPrsnt.FocusId)
                                {

                                    trgd.UpperCompPosition = trgd.SetPosIndexCenter(trgd.SourceOne);
                                }
                                else if (trgd.SourceTwo == DsoPrsnt.FocusId)
                                {
                                    trgd.DataCompPosition = trgd.SetPosIndexCenter(trgd.SourceTwo);
                                }
                            }
                            break;
                        case TrigSetupHoldPrsnt trgsh:
                            if (trgsh.ClkSource == DsoPrsnt.FocusId)
                            {
                                trgsh.ClkCompPosition = trgsh.SetCompPosCenter(trgsh.ClkSource);
                            }
                            else if (trgsh.DataSource == DsoPrsnt.FocusId)
                            {
                                trgsh.UpperDataPosition = trgsh.SetCompPosCenter(trgsh.DataSource);
                            }
                            break;
                        case TrigPatPrsnt trgpt:
                            trgpt.SetCompPosCenter(DsoPrsnt.FocusId);
                            break;
                    }
                    break;
                case KeyCode.TRIG_FORCE:

                    PlatformUIManager.Default.Platform.KeyEnumTrigForceHandler();
                    break;

                case KeyCode.RUNSTOP:
                    if (TriggerPrsnt.State != SysState.Stop)
                    {
                        Presenter.Stop();
                    }
                    else
                    {
                        Presenter.Resume();
                    }

                    break;
                case KeyCode.SINGLE:
                    TriggerPrsnt.Mode = TriggerMode.OneShot;
                    break;
                case KeyCode.NORMAL:
                    TriggerPrsnt.Mode = TriggerMode.Normal;
                    break;
                case KeyCode.AUTO:
                    TriggerPrsnt.Mode = TriggerMode.Auto;
                    break;
                case KeyCode.AUTOSET:
                    Activate();
                    AutoSet();
                    break;
                case KeyCode.AUTOCALIBRATION:
                    AutoCalibration();
                    break;
                case KeyCode.SAVEDATASOURCE:
                    SaveDataSoure();
                    break;
                case KeyCode.CLEAR:
                    Presenter.SoftReset();
                    break;
                case KeyCode.PRINT:
                    {
                        if (!String.IsNullOrEmpty(PrintApp.PrinterName))
                        {
                            PrintApp.Print();
                        }
                    }
                    break;
                case KeyCode.DEFAULT:
                    {
                        Activate();
                        if (!StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.RestoreFactoryDefault, MessageType.Warning))
                        {
                            break;
                        }
                        Program.Oscilloscope.Default();
                    }
                    break;
                case KeyCode.FASTACQ:
                    {
                        WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
                        //Fastacq();
                    }
                    break;
                case KeyCode.LAYEROFF:
                    MultiWindowManager.RemoveAllWindows();
                    break;
                case KeyCode.VK_SNAPSHOT:
                    if (WindowState != FormWindowState.Minimized)
                    {
                        Presenter.Measure.SnapshotActive = !Presenter.Measure.SnapshotActive;
                    }
                    break;
                case KeyCode.VK_SCREENSHOT:
                    {
                        ScreenShot();
                    }
                    break;
                case KeyCode.VK_CLEAR:
                    {
                        Presenter.VKClear();
                    }
                    break;
                case KeyCode.VK_ZOOM:
                    AddMathWfm(mp =>
                    {
                        var src = ChannelId.C1;
                        if (DsoPrsnt.FocusId.IsAnalog())
                        {
                            src = DsoPrsnt.FocusId;
                        }

                        var mza = (MathZoomArg)mp.GetOrMakeArg(MathType.Zoom);
                        mza.Source = src;
                    });
                    break;

                case KeyCode.VK_FFT:

                    if (!Constants.ENABLE_Math)
                    {
                        WeakTip.Default.Write("FFT", MsgTipId.FunctionDisabled);
                        return;
                    }
                    if (!FFTFunctionLimit())
                    {
                        return;
                    }
                    AddMathWfm(mp =>
                    {
                        var src = ChannelId.C1;
                        if (DsoPrsnt.FocusId.IsAnalog())
                        {
                            src = DsoPrsnt.FocusId;
                        }

                        var mfa = (MathFftArg)mp.GetOrMakeArg(MathType.FFT);
                        mfa.Source = src;
                    });
                    break;
                case KeyCode.TOUCH:
                    try
                    {

                        Presenter.Display.TouchLock = !Presenter.Display.TouchLock;
                    }
                    catch (Exception ex)
                    {
                        WeakTip.Default.Write("ChkTouch", MsgTipId.AdministrtorAuthorityRequired);
                        EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Error));
                    }
                    break;
                case KeyCode.AI_SET:
                    DsoTopStrip.OnAiSetClicked(() =>
                    {
                        var aif = new AiParamsSetForm()
                        {
                            Presenter = Presenter.ArtificialIntelligence,
                            Anchor = AnchorStyles.Top,
                        };

                        aif.Presenter.TryAddView(aif);

                        return aif;
                    });
                    break;
                case KeyCode.MULTI_DOMAIN:
                    MakeOperateForm("MultiDomain", PointToScreen(new(Width / 2, Height / 6)), PopOrientation.Under, () =>
                    {
                        var aif = new MultiDomainForm()
                        {
                            Presenter = Presenter.MultiDomain,
                            Anchor = AnchorStyles.Top,
                        };

                        aif.Presenter.TryAddView(aif);

                        return aif;
                    });
                    break;
                case KeyCode.VK_TEMPCTRL:
                    MakeOperateForm("TempCtrl", PointToScreen(new(Width / 2, Height / 6)), PopOrientation.Under, () =>
                    {
                        var af = new TempControlForm()
                        {
                            Anchor = AnchorStyles.Top,
                            TempCtrlPrsnt = Presenter.TempCtrl,
                            StartPosition = FormStartPosition.CenterScreen,
                        };
                        af.TempCtrlPrsnt.TryAddView(af);
                        return af;
                    });
                    break;
                case KeyCode.VK_AI:
                    MakeOperateForm("AICtrl", PointToScreen(new(Width, DsoTopStrip.Height)), PopOrientation.Under, () =>
                    {
                        var af = new AiParamsSetForm()
                        {
                            Anchor = AnchorStyles.Top,
                            Presenter = Presenter.ArtificialIntelligence,
                            StartPosition = FormStartPosition.CenterScreen,

                        };
                        af.Presenter.TryAddView(af);
                        return af;
                    });
                    break;
                default:
                    if (HotKnobManager.Default.LostFocus)
                        _HotKnob.Turn(keyCode, keyStep);
                    else
                        ControlsHotKnob.Default.Turn(keyCode, keyStep);
                    break;
            }
        }

        private void CloseSettingForm()
        {
            EventBroker.Instance.GetEvent<FormCloseEventArgs>().Publish(this, new FormCloseEventArgs() { Type = FormType.ShowDialogForm });
            EventBroker.Instance.GetEvent<FormCloseEventArgs>().Publish(this, new FormCloseEventArgs() { Type = FormType.SettingForm });
            IntPtr intPtr = NativeMethods.FindWindow(null, "打开");
            if (intPtr != IntPtr.Zero)
            {
                NativeMethods.PostMessage(intPtr, 0x0010, 0, 0);
            }
        }

        private void Fastacq()
        {
            try
            {
                Presenter.Timebase.StorageMode = Presenter.Timebase.StorageMode == AnaChnlStorageMode.Fast ? AnaChnlStorageMode.Long : AnaChnlStorageMode.Fast;
                DsoTopStrip.OnFastAcqClicked();
                (MultiWindowManager.GetMainWindow() as WaveformGPUFigure).UpdateView(DsoPrsnt.DefaultDsoPrsnt.Timebase, nameof(Presenter.Timebase.StorageMode));//通知主窗口设置显隐余辉设置
                if (Presenter.Timebase.StorageMode == AnaChnlStorageMode.Fast)
                {
                    foreach (var item in ChannelIdExt.GetAnalogs())
                    {
                        if (Presenter.TryGetChannel(item, out var cprsnt) && cprsnt is AnalogPrsnt analog && analog.Coupling == AnaChnlCoupling.Gnd)
                        {
                            analog.Coupling = AnaChnlCoupling.DC1M;
                        }
                    }

                    if (Presenter.Timebase.Mode == AnaChnlAcqMode.Average || Presenter.Timebase.Mode == AnaChnlAcqMode.Envelope)
                    {
                        Presenter.Timebase.Mode = AnaChnlAcqMode.Normal;
                    }
                }
                Presenter.SoftReset();//关闭快采时也应该重新刷新波形
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Warn));
            }
        }

        private void ScreenShot()
        {
            try
            {
                if (!Directory.Exists(FileApp.Default.Presenter.PicPath))
                {
                    WeakTip.Default.Write("PicPath", MsgTipId.FilePathNotExist, false, "", 2);
                    return;
                }

                FileApp.Default.Presenter.FileName = FileApp.Default.Presenter.MakeDefaultFileName(FileApp.Default.Presenter.PicPath, FilePrsnt.GetPicFileExtName(FileApp.Default.Presenter.PicFormat));

                if (FilePrsnt.SaveImage(FileApp.Default.Presenter.PicPath, FileApp.Default.Presenter.FileName, FileApp.Default.Presenter.PicFormat, FileApp.Default.Presenter.PicRegion, FileApp.Default.Presenter.IfAppendDatetime, FileApp.Default.Presenter.PicColor, needCloseWeakTip: true))
                {
                    WeakTip.Default.Write("File", MsgTipId.SavingSuccess, false, FileApp.Default.Presenter.PicPath);
                }
                else
                {
                    WeakTip.Default.Write("File", MsgTipId.SavingFailed);
                }
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Warn));
            }
        }

        private void ProcessKeycode(Int32 keyCode, Int32 keyStride = 1)
        {
            if (ProcessSystemCheckMode(keyCode))
            {
                return;
            }
            if (DsoPrsnt.KeyBoardLockEnable && !DsoPrsnt.KeyBoardForbidLockKeys.Contains(keyCode))
            {
                return;
            }
            switch (keyCode)
            {
                case KeyCode.CH1:
                    SetFocusChnl(ChannelId.C1);
                    break;
                case KeyCode.CH2:
                    SetFocusChnl(ChannelId.C2);
                    break;
                case KeyCode.CH3:
                    SetFocusChnl(ChannelId.C3);
                    break;
                case KeyCode.CH4:
                    SetFocusChnl(ChannelId.C4);
                    break;
                case KeyCode.CH5:
                    SetFocusChnl(ChannelId.C5);
                    break;
                case KeyCode.CH6:
                    SetFocusChnl(ChannelId.C6);
                    break;
                case KeyCode.CH7:
                    SetFocusChnl(ChannelId.C7);
                    break;
                case KeyCode.CH8:
                    SetFocusChnl(ChannelId.C8);
                    break;

                case KeyCode.MATH:
                    if (!Constants.ENABLE_Math)
                    {
                        WeakTip.Default.Write("Math", MsgTipId.FunctionDisabled);
                        return;
                    }
                    if (!_MathKeycodeCtrl)
                    {
                        try
                        {
                            Activate();
                            _MathKeycodeCtrl = true;
                            var activemchs = Presenter.TryGetRange(c => c.Active && c.Id.IsMath() && !c.Id.IsAdvancedMath());
                            if (activemchs.Any())
                            {
                                var index = activemchs.FindIndex(x => x.Id == DsoPrsnt.FocusId);

                                if (activemchs.Count == 1)
                                {
                                    #region 当前只有一个Math，且未激活状态就切换到Math，如果激活状态就取消Math的激活
                                    if (index == -1)
                                    {
                                        DsoPrsnt.FocusId = activemchs.First().Id;
                                        DsoInfoStrip.ShowForm(DsoPrsnt.FocusId);
                                    }
                                    else
                                    {
                                        activemchs.First().Active = false;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 如果多个Math，且都处于未激活状态就激活最近的Math，如果当前是Math就循环切换到下一个
                                    if (index == -1)
                                    {
                                        Presenter.MoveFocusId(ChannelIdExt.MinMChId, ChannelIdExt.MaxMChId);
                                        DsoInfoStrip.ShowForm(DsoPrsnt.FocusId);//影响性能，暂时不弹出信息窗
                                    }
                                    else
                                    {
                                        DsoPrsnt.FocusId = index == activemchs.Count - 1 ? activemchs.First().Id : activemchs[index + 1].Id;
                                    }
                                    #endregion
                                }
                                // <Remark>更改人：彭博 创建日期：2023/12/13 19:25:00  原因：当信源通道关闭时，自动切换光标测量信源通道 </Remark>
                                AutoChanageCursors();
                            }
                            else
                            {
                                TryAddMathWaveform();
                            }
                        }
                        catch (Exception ex)
                        {
                            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex.Message, LogLevel.Warn));
                        }
                        finally
                        {
                            _MathKeycodeCtrl = false;
                        }
                    }
                    break;
                case KeyCode.R1:
                    TryAddRefWaveform(ChannelId.R1);
                    break;
                case KeyCode.R2:
                    TryAddRefWaveform(ChannelId.R2);
                    break;
                case KeyCode.R3:
                    TryAddRefWaveform(ChannelId.R3);
                    break;
                case KeyCode.R4:
                    TryAddRefWaveform(ChannelId.R4);
                    break;
                case KeyCode.REF:
                    if (!Constants.ENABLE_Ref)
                    {
                        WeakTip.Default.Write("Ref", MsgTipId.FunctionDisabled);
                        return;
                    }
                    if (!_RefKeycodeCtrl)
                    {
                        try
                        {
                            var activedigitalchs = Presenter.TryGetRange(c => c.Active && c.Id.IsDigital());
                            if (activedigitalchs.Count > 0)
                            {
                                CloseCfgForm(activedigitalchs.First());
                            }

                            var activerchs = Presenter.TryGetRange(c => c.Active && c.Id.IsReference());
                            if (activerchs.Any())
                            {
                                var index = activerchs.FindIndex(x => x.Id == DsoPrsnt.FocusId);

                                if (activerchs.Count == 1)
                                {
                                    #region 当前只有一个REF，且未激活状态就切换到REF，如果激活状态就取消REF的激活
                                    if (index == -1)
                                    {
                                        DsoPrsnt.FocusId = activerchs.First().Id;
                                        DsoInfoStrip.ShowForm(DsoPrsnt.FocusId);
                                    }
                                    else
                                    {
                                        activerchs.First().Active = false;
                                        RemoveWaveformUI(activerchs.First());
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 如果多个REF，且都处于未激活状态就激活最近的REF，如果当前是REF就循环切换到下一个
                                    if (index == -1)
                                    {
                                        Presenter.MoveFocusId(ChannelIdExt.MinRChId, ChannelIdExt.MaxRChId);
                                        DsoInfoStrip.ShowForm(DsoPrsnt.FocusId);//影响性能，暂时不弹出信息窗
                                    }
                                    else
                                    {
                                        DsoPrsnt.FocusId = index == activerchs.Count - 1 ? activerchs.First().Id : activerchs[index + 1].Id;
                                    }
                                    #endregion
                                }
                                // <Remark>更改人：彭博 创建日期：2023/12/13 19:25:00  原因：当信源通道关闭时，自动切换光标测量信源通道 </Remark>
                                AutoChanageCursors();
                            }
                            else
                            {
                                TryAddRefWaveform();
                            }

                        }
                        catch (Exception ex)
                        {
                            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex.Message, LogLevel.Warn));
                        }
                        finally
                        {
                            _RefKeycodeCtrl = false;
                        }
                    }
                    break;

                case KeyCode.DECODE:
                    if (!Constants.ENABLE_BUS)
                    {
                        WeakTip.Default.Write("DECODE", MsgTipId.FunctionDisabled);
                        return;
                    }
                    if (!_DecoedKeycodeCtrl)
                    {
                        try
                        {
                            _DecoedKeycodeCtrl = true;
                            var activedchs = Presenter.TryGetRange(c => c.Active && c.Id.IsDecode());
                            if (activedchs.Any())
                            {
                                var index = activedchs.FindIndex(x => x.Id == DsoPrsnt.FocusId);

                                if (activedchs.Count == 1)
                                {
                                    #region 当前只有一个REF，且未激活状态就切换到REF，如果激活状态就取消REF的激活
                                    if (index == -1)
                                    {
                                        DsoPrsnt.FocusId = activedchs.First().Id;
                                    }
                                    else
                                    {
                                        activedchs.First().Active = false;
                                        RemoveWaveformUI(activedchs.First());
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region 如果多个Decode，且都处于未激活状态就激活最近的Decode，如果当前是REF就循环切换到下一个
                                    if (index == -1)
                                    {
                                        Presenter.MoveFocusId(ChannelIdExt.MinBChId, ChannelIdExt.MaxBChId);
                                        DsoInfoStrip.ShowForm(DsoPrsnt.FocusId);//影响性能，暂时不弹出信息窗
                                    }
                                    else
                                    {
                                        DsoPrsnt.FocusId = index == activedchs.Count - 1 ? activedchs.First().Id : activedchs[index + 1].Id;
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                TryAddDecodeWaveform();
                            }
                        }
                        catch (Exception ex)
                        {
                            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex.Message, LogLevel.Warn));
                        }
                        finally
                        {
                            _DecoedKeycodeCtrl = false;
                        }
                    }
                    break;

                case KeyCode.LOGIC:
                    if (!Constants.ENABLE_LA)
                    {
                        WeakTip.Default.Write("Digital", MsgTipId.FunctionDisabled);
                        return;
                    }
                    if (_LOGICKeycodeCtrl == false)
                    {
                        _LOGICKeycodeCtrl = true;

                        if (OptionManager.Default.Checked(OptionType.LA) == false)
                        {
                            _LOGICKeycodeCtrl = false;
                            break;
                        }
                        try
                        {

                            var activelchs = Presenter.TryGetRange(c => c.Active && c.Id.IsDigital());
                            if (activelchs.Count > 1)
                            {
                                Presenter.MoveFocusId(ChannelIdExt.MinDChId, ChannelIdExt.MaxDChId);
                            }
                            else if (activelchs.Count == 1)
                            {
                                var rch = activelchs.First();

                                if (DsoPrsnt.FocusId == rch.Id)
                                {
                                    rch.Active = false;
                                }
                                else
                                {
                                    DsoPrsnt.FocusId = rch.Id;
                                }
                            }
                            else
                            {
                                TryAddDigiWaveform();
                            }
                            _LOGICKeycodeCtrl = false;
                        }
                        catch (Exception)
                        {
                            _LOGICKeycodeCtrl = false;
                        }
                    }
                    break;

                case KeyCode.VK_AWGALL:
                    if (WindowState == FormWindowState.Minimized)
                    {
                        break;
                    }
                    if (!Constants.ENABLE_AWG)
                    {
                        WeakTip.Default.Write("AWG", MsgTipId.FunctionDisabled);
                        return;
                    }
                    if (_AWGKeycodeCtrl == false)
                    {
                        _AWGKeycodeCtrl = true;

                        if (OptionManager.Default.Checked(OptionType.AWG) == false)
                        {
                            _AWGKeycodeCtrl = false;
                            break;
                        }
                        try
                        {
                            ArbWfmGenPrsnt awgAll1 = Presenter.GetWfmGenerator(ChannelId.AWG1);
                            ArbWfmGenPrsnt awgAll2 = Presenter.GetWfmGenerator(ChannelId.AWG2);
                            if (awgAll1.Active == awgAll2.Active)
                            {
                                awgAll1.Active = !awgAll1.Active;
                                awgAll2.Active = !awgAll2.Active;
                                if (awgAll1.Active)
                                {
                                    TryAddAwgInfo(ChannelId.AWG1);
                                    TryAddAwgInfo(ChannelId.AWG2);
                                }
                                else
                                {
                                    RemoveWaveformUI(awgAll1);
                                    RemoveWaveformUI(awgAll2);
                                }
                            }
                            else if (awgAll1.Active)
                            {
                                awgAll1.Active = false;
                                awgAll1.IsShow = awgAll2.IsShow = false;
                                RemoveWaveformUI(awgAll1);
                            }
                            else
                            {
                                awgAll2.Active = false;
                                awgAll1.IsShow = awgAll2.IsShow = false;
                                RemoveWaveformUI(awgAll2);
                            }
                            _AWGKeycodeCtrl = false;
                        }
                        catch (Exception)
                        {
                            _AWGKeycodeCtrl = false;
                        }
                    }
                    break;

                case KeyCode.VK_AWG1:

                    if (!Constants.ENABLE_AWG)
                    {
                        WeakTip.Default.Write("AWG", MsgTipId.FunctionDisabled);
                        return;
                    }

                    if (_AWGKeycodeCtrl == false)
                    {
                        _AWGKeycodeCtrl = true;

                        if (OptionManager.Default.Checked(OptionType.AWG) == false)
                        {
                            _AWGKeycodeCtrl = false;
                            break;
                        }
                        try
                        {
                            ArbWfmGenPrsnt awg1 = Presenter.GetWfmGenerator(ChannelId.AWG1);
                            awg1.Active = !awg1.Active;
                            if (awg1.Active)
                            {
                                TryAddAwgInfo(ChannelId.AWG1);
                            }
                            else
                            {
                                RemoveWaveformUI(awg1);
                            }
                            _AWGKeycodeCtrl = false;
                        }
                        catch (Exception)
                        {
                            _AWGKeycodeCtrl = false;
                        }
                    }
                    break;
                case KeyCode.VK_AWG2:
                    if (!Constants.ENABLE_AWG)
                    {
                        WeakTip.Default.Write("AWG", MsgTipId.FunctionDisabled);
                        return;
                    }
                    if (_AWGKeycodeCtrl == false)
                    {
                        _AWGKeycodeCtrl = true;

                        if (OptionManager.Default.Checked(OptionType.AWG) == false)
                        {
                            _AWGKeycodeCtrl = false;
                            break;
                        }
                        try
                        {
                            ArbWfmGenPrsnt awg2 = Presenter.GetWfmGenerator(ChannelId.AWG2);
                            awg2.Active = !awg2.Active;
                            if (awg2.Active)
                            {
                                TryAddAwgInfo(ChannelId.AWG2);
                            }
                            else
                            {
                                RemoveWaveformUI(awg2);
                            }
                            _AWGKeycodeCtrl = false;
                        }
                        catch (Exception)
                        {
                            _AWGKeycodeCtrl = false;
                        }
                    }
                    break;


                case KeyCode.VK_VOLTMETER:
                    Presenter.Voltmeter.Active = !Presenter.Voltmeter.Active;
                    Activate();
                    break;

                case KeyCode.VK_CYMOMETER:
                    Presenter.Cymometer.Active = !Presenter.Cymometer.Active;
                    Activate();
                    break;

                case KeyCode.TIMEBASE:
                    DsoTopStrip.OnTimebaseClicked(() =>
                    {
                        var tmbf = new TimebaseForm()
                        {
                            Presenter = Presenter.Timebase,
                            Anchor = AnchorStyles.Top,
                        };

                        tmbf.Presenter.TryAddView(tmbf);

                        return tmbf;
                    });
                    break;

                case KeyCode.ACQUIRE:
                    DsoTopStrip.OnAcquisitionClicked(() =>
                    {
                        var af = new AcquireForm()
                        {
                            Presenter = Presenter.Timebase,
                            Anchor = AnchorStyles.Top,
                        };

                        af.Presenter.TryAddView(af);

                        return af;
                    });
                    break;
                case -KeyCode.ACQUIRE:
                    DsoTopStrip.OnAcquisitionClicked(() =>
                    {
                        var af = new AcquireForm()
                        {
                            Presenter = Presenter.Timebase,
                            Anchor = AnchorStyles.Top,
                            GroupIndex = 1,
                        };

                        af.Presenter.TryAddView(af);

                        return af;
                    });
                    break;

                case KeyCode.TRIGGER:
                    if (WindowState != FormWindowState.Minimized)
                    {
                        if (DsoTopStrip.IsTriggerFormShow)
                        {
                            Activate();
                            DsoTopStrip.IsTriggerFormShow = false;
                            break;
                        }
                        DsoTopStrip.OnTriggerClicked(() =>
                        {
                            var tf = new TriggerForm
                            {
                                Anchor = AnchorStyles.Top,
                                Presenter = Presenter.CurrentTrigger
                            };
                            tf.Presenter.TryAddView(tf);
                            tf.Presenter.TryAddView(DsoTopStrip);
                            tf.TriggerAssistedPresenter = Presenter.TriggerAssist;
                            tf.TriggerAssistedPresenter.TryAddView(tf);
                            tf.LocationAssistedPresenter = Presenter.LocAssisted;
                            tf.LocationAssistedPresenter.TryAddView(tf);
                            tf.VisualTriggerPresenter = Presenter.VisualTrigger;
                            tf.VisualTriggerPresenter.TryAddView(tf);

                            tf.FormClosing += (_, _) =>
                            {
                                tf.Presenter?.TryRemoveView(tf);
                                tf.TriggerAssistedPresenter?.TryRemoveView(tf);
                                tf.LocationAssistedPresenter?.TryRemoveView(tf);
                                tf.VisualTriggerPresenter?.TryRemoveView(tf);
                            };

                            return tf;
                        });
                        DsoTopStrip.IsTriggerFormShow = true;
                    }
                    break;
                case KeyCode.CURSOR:
                    if (Presenter.Cursor.Active == false)
                    {
                        //<Remark>更改人：彭博 创建日期：2023/12/13 18:47:00  原因：当参考波形打开时，光标测量未选择参考波形 </Remark>
                        if (DsoPrsnt.FocusId.IsAnalog() || DsoPrsnt.FocusId.IsBaseMath() || DsoPrsnt.FocusId.IsReference())
                        {
                            Presenter.Cursor.SyncSource = DsoPrsnt.FocusId;
                        }
                        else
                        {
                            var analogpresenter = Presenter.TryGetRange(c => (c.Id.IsAnalog() || c.Id.IsBaseMath()) && c.Active)?.FirstOrDefault();
                            if (analogpresenter != null)
                            {
                                Presenter.Cursor.SyncSource = analogpresenter.Id;
                            }
                            else
                            {
                                //提示
                            }
                        }
                    }
                    Activate();
                    Presenter.Cursor.Active = !Presenter.Cursor.Active;
                    _HotKnob = _HotKnobFuncTable[nameof(CursorHotKnob)];
                    break;
                case -KeyCode.CURSOR:
                    Activate();

                    DsoTopStrip.OnCursorClicked(() =>
                    {
                        var cf = new CursorForm(Presenter.Cursor)
                        {
                            Anchor = AnchorStyles.Top,
                        };
                        return cf;
                    });
                    _HotKnob = _HotKnobFuncTable[nameof(CursorHotKnob)];
                    break;

                case KeyCode.MEASURE:
                    MeasureApp.Default.Presenter.Active = !MeasureApp.Default.Presenter.Active;
                    break;
                case -KeyCode.MEASURE:
                    OnMeasurementClick(() =>
                    {
                        var mf = new MeasureMenuForm()
                        {
                            Anchor = AnchorStyles.Top,
                            Presenter = Presenter.Measure,
                            CymometerPresenter = Presenter.Cymometer,
                            VoltmeterPresenter = Presenter.Voltmeter,
                        };
                        mf.Presenter.TryAddView(mf);
                        mf.CymometerPresenter.TryAddView(mf);
                        mf.VoltmeterPresenter.TryAddView(mf);

                        return mf;
                    });
                    break;
                case KeyCode.STORAGE:
                    DsoTopStrip.OnFileClicked(() => FileApp.Default.MakeForm());
                    break;

                case KeyCode.SETTING:
                    if (WindowState != FormWindowState.Minimized)
                    {
                        if (DsoTopStrip.IsSettingFormShow)
                        {
                            Activate();
                            DsoTopStrip.IsSettingFormShow = false;
                            break;
                        }
                        DsoTopStrip.OnSettingClicked(() =>
                        {
                            var sf = new SettingForm()
                            {
                                Presenter = Presenter.Setting,
                                Anchor = AnchorStyles.Top,
                            };

                            return sf;
                        });
                        DsoTopStrip.IsSettingFormShow = true;
                    }
                    break;
                case -KeyCode.SETTING://自动设置入口
                    if (WindowState != FormWindowState.Minimized)
                    {
                        if (DsoTopStrip.IsSettingFormShow)
                        {
                            Activate();
                            DsoTopStrip.IsSettingFormShow = false;
                            break;
                        }
                        DsoTopStrip.OnSettingClicked(() =>
                        {
                            var sf = new SettingForm()
                            {
                                Presenter = Presenter.Setting,
                                Anchor = AnchorStyles.Top,
                                GroupIndex = 1
                            };
                            return sf;
                        });
                        DsoTopStrip.IsSettingFormShow = true;
                    }
                    break;
                case KeyCode.HELP:
                    break;

                case KeyCode.VK_APPS:
                    if (WindowState != FormWindowState.Minimized)
                    {
                        if (DsoTopStrip.IsAppsFormShow)
                        {
                            Activate();
                            DsoTopStrip.IsAppsFormShow = false;
                            break;
                        }
                        DsoTopStrip.OnAppsClicked(() =>
                        {
                            return StartExtentForm.Defalut;
                        });
                        DsoTopStrip.IsAppsFormShow = true;
                    }
                    break;

                case KeyCode.VK_LISSAJOUS:
                    if (!Constants.ENABLE_Lissajous)
                    {
                        WeakTip.Default.Write("Lissajous", MsgTipId.FunctionDisabled);
                        return;
                    }
                    MakeOperateForm("Lissajous", PointToScreen(new(Width, DsoTopStrip.Height)), PopOrientation.Under, () =>
                         new LissajousForm() { Anchor = AnchorStyles.Top | AnchorStyles.Right });
                    break;
                case KeyCode.VK_PASSFAIL:
                    if (!Constants.ENABLE_PassFail)
                    {
                        WeakTip.Default.Write("PassFail", MsgTipId.FunctionDisabled);
                        return;
                    }
                    MakeOperateForm("PassFail", PointToScreen(new(Width, DsoTopStrip.Height)), PopOrientation.Under, () =>
                        PassFailApp.Default.MakeForm());
                    break;
                case KeyCode.VK_SDA:
                    //WeakTip.Default.Write("SDA", MsgTipId.FunctionDisabled);
                    //WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
                    MakeOperateForm("SDA", PointToScreen(new(Width, DsoTopStrip.Height)), PopOrientation.Under, () =>
                        JitterApp.Default.MakeForm());
                    return;
                    //if (!JitterApp.Default.Presenter.Active)
                    //{
                    //    JitterApp.Default.Presenter.Active = true;
                    //}
                    //else
                    //{
                    //    JitterApp.Default.ShowForm();
                    //}

                    break;
                case KeyCode.VK_PWRANALYSIS:
                    if (!Constants.ENABLE_PowerAs)
                    {
                        WeakTip.Default.Write("PwrAnalysis", MsgTipId.FunctionDisabled);
                        return;
                    }
                    MakeOperateForm("PwrAnalysis", PointToScreen(new(Width, DsoTopStrip.Height)), PopOrientation.Under, () =>
                        PowerAnalysisApp.Default.MakeForm());
                    break;
                case KeyCode.VK_VSA:
                    if (Constants.ENABLE_VSA)
                    {
                        MakeOperateForm("VSA", PointToScreen(new(Width, DsoTopStrip.Height)), PopOrientation.Under, () =>
                            VectorAnalysisApp.Default.MakeForm());
                    }
                    else
                    {
                        WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
                    }
                    break;
                case KeyCode.VK_DDRA:
                    break;
                case KeyCode.VK_COMPLIANCETEST:
                    break;
                case KeyCode.VK_WAVESEARCH:
                    if (!Constants.ENABLE_Search)
                    {
                        WeakTip.Default.Write("VK_WAVESEARCH", MsgTipId.FunctionDisabled);
                        return;
                    }
                    if (Presenter.Search.TryMake(SearchType.Edge, out var pt))
                    {
                        pt.Active = true;
                        if (SearchApp.Default.ItemForm == null || SearchApp.Default.ItemForm.IsDisposed)
                        {
                            MakeOperateForm(SearchApp.Default.FoucsItem.Name, PointToScreen(new(Width, DsoTopStrip.Height)), PopOrientation.Under, () =>
                          SearchApp.Default.MakeItemForm(SearchApp.Default.FoucsItem));
                        }
                    }
                    break;
                case KeyCode.VK_WAVESEARCH_ITEMSEETING:
                    if (SearchApp.Default.FoucsItem != null)
                    {
                        var panelleft = (PanelManager as Form).Left;
                        var panelright = (PanelManager as Form).Right;
                        var x = panelleft > SearchItemForm.Width ? (panelleft - Left - SearchItemForm.Width / 2) : (panelright - Left + SearchItemForm.Width / 2);
                        MakeOperateForm(SearchApp.Default.FoucsItem.Name, PointToScreen(new(x, DsoTopStrip.Height)), PopOrientation.Under, () =>
                           SearchApp.Default.MakeItemForm(SearchApp.Default.FoucsItem));
                    }
                    break;
                case KeyCode.VK_WAVESEARCH_ITEMCLOSE_CURRENT:
                    Presenter.Search.RemoveSearch(SearchApp.Default.FoucsItem.ID);
                    break;
                case KeyCode.VK_WAVESEARCH_ITEMCLOSEAll:
                    Presenter.Search.RemoveAll();
                    break;
                case KeyCode.VK_SETPRINTER:
                    DsoTopStrip.OnPrinterClicked(() => PrintApp.Default.MakeForm());
                    break;
                case KeyCode.VK_3D:
                    break;

                case KeyCode.VK_MINIMIZE:
                    WindowState = FormWindowState.Minimized;
                    break;
                case KeyCode.VK_CLOSE:
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.AppClose, MessageType.Asking))
                    {
                        //Saving Setting 
                        FilePrsnt.SaveSetting(Environment.CurrentDirectory, "LastSettings", false, true);

                        Close();
                    }
                    break;
                case KeyCode.VK_SHUTDOWN:
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.ComputerShutDown, MessageType.Asking))
                    {
                        FilePrsnt.SaveSetting(Environment.CurrentDirectory, "LastSettings", false, true);

                        if (!Debugger.IsAttached)
                        {
                            Shutdown.PowerOff(Presenter);
                        }
                        else
                        {
                            this.Close();
                        }
                    }
                    break;
                case -KeyCode.VK_SHUTDOWN:

                    FilePrsnt.SaveSetting(Environment.CurrentDirectory, "LastSettings", false, true);

                    if (!Debugger.IsAttached)
                    {
                        Shutdown.PowerOff(Presenter);
                    }
                    else
                    {
                        this.Close();
                    }
                    break;
                case KeyCode.VK_RESTART:
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.ComputerRestart, MessageType.Asking))
                    {
                        //Saving Setting 
                        FilePrsnt.SaveSetting(Environment.CurrentDirectory, "LastSettings", false, true);

                        if (!Debugger.IsAttached)
                        {
                            Shutdown.Reboot(Presenter);
                        }
                        else
                        {
                            this.Close();
                        }
                    }
                    break;
                case -KeyCode.VK_RESTART:

                    FilePrsnt.SaveSetting(Environment.CurrentDirectory, "LastSettings", false, true);

                    if (!Debugger.IsAttached)
                    {
                        Shutdown.Reboot(Presenter);
                    }
                    else
                    {
                        this.Close();
                    }
                    break;
                case KeyCode.VK_LOGOUT:
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.UserLogOut, MessageType.Asking))
                    {
                        //Saving Setting 
                        FilePrsnt.SaveSetting(Environment.CurrentDirectory, "LastSettings", false, true);

                        if (!Debugger.IsAttached)
                        {
                            Shutdown.LogOff();
                        }
                        else
                        {
                            this.Close();
                        }
                    }
                    break;
                case -KeyCode.VK_LOGOUT:

                    FilePrsnt.SaveSetting(Environment.CurrentDirectory, "LastSettings", false, true);

                    if (!Debugger.IsAttached)
                    {
                        Shutdown.LogOff();
                    }
                    else
                    {
                        this.Close();
                    }
                    break;
                case KeyCode.VK_ABOUT:

                    MakeOperateForm("About", PointToScreen(new(Width / 2, Height / 6)), PopOrientation.Under, () =>
                    {
                        var af = new AboutForm()
                        {
                            Anchor = AnchorStyles.Top,
                            StartPosition = FormStartPosition.CenterScreen,
                        };

                        return af;
                    });
                    break;
                case KeyCode.VK_EYEPATTERN:

                    MakeOperateForm("About", PointToScreen(new(Width / 2, Height / 6)), PopOrientation.Under, () =>
                    {
                        var af = new DemoSetForm()
                        {
                            Anchor = AnchorStyles.Top,
                            StartPosition = FormStartPosition.CenterScreen,
                        };

                        return af;
                    });
                    break;
                case KeyCode.VK_SCOPE_CHECK_MASK:
                    MakeOperateForm("SystemCheck", PointToScreen(new(Width, Height)), PopOrientation.Under, () =>
                    {
                        switch (Presenter.SystemCheck.ScopeCheckType)
                        {
                            case CheckType.ScreenCheck:
                                MaskForm mask = new MaskForm()
                                {
                                    Presenter = Presenter.SystemCheck,
                                    Anchor = AnchorStyles.Top,
                                    Width = Width,//1920
                                    Height = Height,//1080
                                };
                                mask.InitMask();
                                return mask;
                            case CheckType.TouchCheck:
                                TouchTestForm touchTest = new TouchTestForm()
                                {
                                    Presenter = Presenter.SystemCheck,
                                    Anchor = AnchorStyles.Top,
                                };
                                touchTest.InitTouch();
                                return touchTest;
                            case CheckType.KeyboardCheck:
                                KeyboardDetectionForm keyboard = new KeyboardDetectionForm()
                                {
                                    Presenter = Presenter.SystemCheck,
                                    Anchor = AnchorStyles.Top,
                                };
                                Presenter.SystemCheck.CheckEnable = true;
                                return keyboard;
                            case CheckType.LEDCheck: break;
                            default: break;

                        }
                        return new MaskForm();
                    });
                    break;
                //{
                //    MaskForm mask = new MaskForm()
                //    {
                //        Presenter = Presenter.SystemCheck,
                //        Anchor = AnchorStyles.Top,
                //    };
                //    mask.InitMask();
                //    this.Controls.Add(mask);
                //    mask.Show();
                //}
                default:
                    ProcessHotKeyCode(keyCode, keyStride);
                    break;
            }
        }

        /// <summary>
        /// 自动切换光标测量信源通道 
        /// </summary>
        /// <Remark>更改人：彭博 创建日期：2023/12/13 19:25:00  原因：当信源通道关闭时，自动切换光标测量信源通道 </Remark>
        private void AutoChanageCursors()
        {
            if (Presenter.Cursor.Active)
            {
                if (DsoPrsnt.FocusId.IsAnalog() || DsoPrsnt.FocusId.IsBaseMath() || DsoPrsnt.FocusId.IsReference())
                {
                    Presenter.Cursor.SyncSource = DsoPrsnt.FocusId;
                }
                else
                {
                    var analogpresenter = Presenter.TryGetRange(c => c.Id.IsAnalog() || (c.Id.IsBaseMath()) && c.Active)?.FirstOrDefault();
                    if (analogpresenter != null)
                    {
                        Presenter.Cursor.SyncSource = analogpresenter.Id;
                    }
                    else
                    {
                        //提示
                    }
                }
            }
        }

        public Boolean FFTFunctionLimit()
        {
            if (!Constants.ENABLE_SDA)
            {

            }
            else
            {
                if (Presenter.Jitter?.Active ?? true)
                {

                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.FFTIsNotSupportedInJitter, MessageType.Asking))
                    {
                        Presenter.Jitter.Active = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
          

            return true;
        }

        private void AddMathWfm(Action<MathPrsnt> initialize)
        {
            if (TryAddMathWaveform(initialize) is null)
            {
                WeakTip.Default.Write("Math", MsgTipId.NoMoreChannels);
            }
        }

        #endregion

        #region computer keyboard
        //ljw 24.1.31 快捷键 全局穿透
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {

            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void DsoForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (DsoPrsnt.KeyBoardLockEnable && !DsoPrsnt.KeyBoardForbidLockKeys.Contains((Int32)e.KeyCode))
            {
                return;
            }
            if (ProcessSystemCheckMode())
            {
                return;
            }
            switch (e.KeyCode)
            {
                //Channel
                case Keys.D1:
                    if (e.Control)
                    {
                        if (e.Shift)
                        {
                            //todo  Zn
                        }
                        else
                        {
                            ProcessKeycode(KeyCode.CH1);
                        }

                    }

                    break;
                case Keys.D2:
                    if (e.Control)
                    {
                        if (e.Shift)
                        {
                            //todo  Zn
                        }
                        else
                        {
                            ProcessKeycode(KeyCode.CH2);
                        }
                    }

                    break;
                case Keys.D3:
                    if (e.Control)
                    {
                        if (e.Shift)
                        {
                            //todo  Zn
                        }
                        else
                        {
                            ProcessKeycode(KeyCode.CH3);
                        }
                    }

                    break;
                case Keys.D4:
                    if (e.Control)
                    {
                        if (e.Shift)
                        {
                            //todo  Zn
                        }
                        else
                        {
                            ProcessKeycode(KeyCode.CH4);
                        }
                    }

                    break;
                case Keys.D5:
                    if (e.Control)
                    {
                        if (e.Shift)
                        {
                            ProcessKeycode(KeyCode.R1);
                        }
                        else
                        {
                            ProcessKeycode(KeyCode.MATH);
                        }
                    }
                    break;
                case Keys.D6:
                    if (e.Control)
                    {
                        if (e.Shift)
                        {
                            ProcessKeycode(KeyCode.R2);
                        }
                        else
                        {
                            ProcessKeycode(KeyCode.REF);
                        }
                    }

                    break;
                case Keys.D7:
                    if (e.Control)
                    {
                        if (e.Shift)
                        {
                            ProcessKeycode(KeyCode.R3);
                        }
                        else
                        {
                            ProcessKeycode(KeyCode.LOGIC);
                        }
                    }
                    break;
                case Keys.D8:
                    if (e.Control)
                    {
                        if (e.Shift)
                        {
                            ProcessKeycode(KeyCode.R4);
                        }
                        else
                        {
                            ProcessKeycode(KeyCode.DECODE);
                        }
                    }

                    break;
                case Keys.D9:
                    if (e.Control)
                    {
                        ProcessKeycode(KeyCode.VK_AWG1);
                    }
                    break;
                case Keys.D0:
                    if (e.Control)
                    {
                        ProcessKeycode(KeyCode.VK_AWG2);
                    }

                    break;
                case Keys.T:
                    if (e.Control)
                    {
                        ProcessKeycode(KeyCode.TIMEBASE);
                    }
                    break;

                case Keys.G:
                    if (e.Control)
                    {
                        ProcessKeycode(KeyCode.TRIGGER);
                    }
                    break;

                //张老师说暂时注释 ljw 24.1.31
                //多功能A
                //case Keys.Oemplus:
                //	ProcessKeycode(KeyCode.KNOB_UPMULTI_RIGHT);
                //	break;
                //case Keys.OemMinus:
                //	ProcessKeycode(KeyCode.KNOB_UPMULTI_LEFT);
                //	break;
                //case Keys.Oem5:
                //	ProcessKeycode(KeyCode.KNOB_UPMULTI_SELECT);
                //	break;
                //时基档
                case Keys.S:
                    if (e.Control)
                    {
                        FilePrsnt.SaveSetting(Constants.SET_DEF_PATH, "ScopeX", true);
                    }
                    else
                    {
                        ProcessKeycode(KeyCode.KNOB_XLEVEL_LEFT);
                    }
                    break;
                case Keys.W:
                    ProcessKeycode(KeyCode.KNOB_XLEVEL_RIGHT);
                    break;
                case Keys.A:
                    if (e.Control)
                    {
                        ProcessKeycode(KeyCode.ACQUIRE);
                    }
                    else
                    {
                        ProcessKeycode(KeyCode.KNOB_XPOS_RIGHT);
                    }
                    break;
                case Keys.D:
                    ProcessKeycode(KeyCode.KNOB_XPOS_LEFT);
                    break;

                //当前物理通道
                case Keys.J:
                    {
                        var keys = new Int32[]
                        {
                            KeyCode.KNOB_CH1YLEVEL_LEFT,
                            KeyCode.KNOB_CH2YLEVEL_LEFT,
                            KeyCode.KNOB_CH3YLEVEL_LEFT,
                            KeyCode.KNOB_CH4YLEVEL_LEFT
                        };
                        var id = (Int32)DsoPrsnt.FocusId;
                        if (id < keys.Length)
                        {
                            ProcessKeycode(keys[id]);
                        }

                        break;
                    }
                case Keys.L:
                    {
                        var keys = new Int32[]
                        {
                            KeyCode.KNOB_CH1YLEVEL_RIGHT,
                            KeyCode.KNOB_CH2YLEVEL_RIGHT,
                            KeyCode.KNOB_CH3YLEVEL_RIGHT,
                            KeyCode.KNOB_CH4YLEVEL_RIGHT
                        };
                        var id = (Int32)DsoPrsnt.FocusId;
                        if (id < keys.Length)
                        {
                            ProcessKeycode(keys[id]);
                        }
                        break;
                    }
                case Keys.I:
                    {
                        var keys = new Int32[]
                        {
                            KeyCode.KNOB_CH1YPOS_LEFT,
                            KeyCode.KNOB_CH2YPOS_LEFT,
                            KeyCode.KNOB_CH3YPOS_LEFT,
                            KeyCode.KNOB_CH4YPOS_LEFT
                        };
                        var id = (Int32)DsoPrsnt.FocusId;
                        if (id < keys.Length)
                        {
                            ProcessKeycode(keys[id]);
                        }
                        break;
                    }
                case Keys.K:
                    {
                        var keys = new Int32[]
                        {
                            KeyCode.KNOB_CH1YPOS_RIGHT,
                            KeyCode.KNOB_CH2YPOS_RIGHT,
                            KeyCode.KNOB_CH3YPOS_RIGHT,
                            KeyCode.KNOB_CH4YPOS_RIGHT
                        };
                        var id = (Int32)DsoPrsnt.FocusId;
                        if (id < keys.Length)
                        {
                            ProcessKeycode(keys[id]);
                        }
                        break;
                    }

                case Keys.Z:
                    if (e.Control)
                    {
                        ProcessKeycode(KeyCode.DEFAULT);
                    }
                    else
                    {
                        ProcessKeycode(KeyCode.KNOB_TRIG_YPOS_LEFT);
                    }
                    break;
                case Keys.X:
                    if (e.Control)
                    {
                        WidgetPrsnt.Combo ^= 0x01;
                    }
                    else
                    {
                        ProcessKeycode(KeyCode.KNOB_TRIG_YPOS_RIGHT);
                    }
                    break;


                case Keys.F1:
                    if (e.Control)
                    {
                        var shortcutform = new ShortcutForm();
                        shortcutform.ShowDialog();
                    }
                    else//帮助手册 首页
                    {
                        HelpProcessManager.SendCommand();
                    }
                    break;

                case Keys.F2:
                    //张老师说注释 24.1.31
                    //WidgetPrsnt.Combo ^= 0x02;
                    //DsoPrsnt.DataSrcOpt = (WidgetPrsnt.Combo & 0x02) == 0x02 ? DataSourceOpt.Simulator : DataSourceOpt.PCIe;
                    break;
                case Keys.F3:
                    //切回真实波形 ljw
                    //DsoPrsnt.DataSrcOpt = DataSourceOpt.PCIe;
                    ProcessKeycode(KeyCode.AUTOSET);
                    //if (Presenter.TryGetChannel(ChannelId.C2, out var mp))
                    //{
                    //    mp.Active = !mp.Active;
                    //}
                    break;
                //case Keys.F4:
                //    if ((int)DsoPrsnt.FocusId < ChannelIdExt.AnaChnlNum)
                //    {
                //        WidgetPrsnt.HardwareMiscFunc("UserAutoCali", $"Gain_{DsoPrsnt.FocusId}");
                //    }
                //    break;
                //case Keys.F5:
                //    WidgetPrsnt.HardwareMiscFunc("UserAutoCali", "Baseline");
                //    break;
                //case Keys.F6:
                //    WidgetPrsnt.HardwareMiscFunc("UserAutoCali", "Offset3Div");
                //    break;
                //case Keys.F7:
                //    WidgetPrsnt.HardwareMiscFunc("UserAutoCali", "DC_Bias");
                //    break;
                case Keys.F8:
                    ProcessKeycode(KeyCode.CURSOR);
                    break;
                case Keys.F9:
                    ProcessKeycode(KeyCode.MEASURE);
                    Focus();
                    break;
                case Keys.F10:
                    ProcessKeycode(KeyCode.VK_SNAPSHOT);
                    break;
                case Keys.F:
                    if (e.Control)
                    {
                        ProcessKeycode(KeyCode.VK_CYMOMETER);
                    }
                    Focus();
                    break;
                case Keys.U:
                    if (e.Control)
                    {
                        ProcessKeycode(KeyCode.VK_VOLTMETER);
                    }
                    Focus();
                    break;
                //顿号 ljw
                case Keys.Oem3:
                    if (e.Control)
                    {
                        Program.MoveToScreen(this, (Program.GetScreenIdx(this) + 1) % Program.GetScreenCount());
                        SuitResolution();
                        Focus();
                    }
                    break;
                case Keys.P:
                    if (e.Control)
                    {
                        if (e.Shift)
                        {
                            ProcessKeycode(KeyCode.VK_SCREENSHOT);
                        }
                        else
                        {
                            ProcessKeycode(KeyCode.PRINT);
                        }
                    }
                    break;
                case Keys.O:
                    if (e.Control)
                    {
                        ProcessKeycode(KeyCode.STORAGE);
                    }
                    break;
                case Keys.M:
                    if (e.Control)
                    {
                        ProcessKeycode(KeyCode.VK_MINIMIZE);
                    }
                    break;
                case Keys.Q:
                    if (e.Control)
                    {
                        ProcessKeycode(KeyCode.VK_SHUTDOWN);
                    }
                    break;
            }
        }
        #endregion

        #region MultiFigures
        public void AssignNewWindowId(ChannelPrsnt cp)
        {
            var mainfig = MultiWindowManager.MainFigure;
            if (mainfig.IsExitWaveByChannelId(cp.Id) == true)
            {
                cp.WindowId = ChannelPrsnt.GetNewWindowId();
            }
        }
        public void SetFigMarkBtnVisible(ChannelPrsnt cp, Boolean visible)
        {
            var fig = MultiWindowManager.GetWindow(cp.WindowId);
            if (fig == null)
            {
                return;
            }

            if (!fig.IsMainForm && fig.ButtonSource != null)
            {
                var btnlist = fig.ButtonSource.Where((o) => { return o.BtnType == WeifenLuo.WinFormsUI.Docking.ButtonType.Cursor; });
                if (btnlist != null && btnlist.Count() > 0)
                {
                    btnlist.First().IsVisible = visible;

                    WindowDockPanel.RefreshBtn(fig);
                }
            }
        }
        public void UpdateFigTitle(ChannelPrsnt cp, String title)
        {
            var fig = MultiWindowManager.GetWindow(cp.WindowId);
            if (fig == null)
            {
                return;
            }

            if (!fig.IsMainForm)
            {
                fig.Title = title;
            }
        }

        public DataTableFigure CreateDataTableFig(Control control, Boolean isShowExport = false, (Bitmap FourBtnBmp, Action Action)? fourBtn = null)
        {
            DataTableFigure table = new(isShowExport, fourBtn)
            {
                Margin = new Padding(0),
                IsFill = true,
                ControlVertical = VerticalStyle.Top,
                ControlHorizontal = HorizontalStyle.Center
            };

            table.AddControl(control);

            if (control is Form form && !string.IsNullOrEmpty(form.Text))
            {
                form.FormClosed += (s, e) =>
                {
                    LanguageManger.Instance.LanguageChanged -= LangChange;
                };

                void LangChange(object? s, ILanguage l)
                {
                    if (table != null && form != null && !string.IsNullOrEmpty(form.Text))
                        table.Title = form.Text;
                }
                LanguageManger.Instance.LanguageChanged -= LangChange;
                LanguageManger.Instance.LanguageChanged += LangChange;
            }

            MultiWindowManager.AddWindow(table);
            return table;
        }
        public void ChangeWaveformFig(IChnlPrsnt cp)
        {

            MultiWindowManager.RemoveWaveform(cp);
            if (cp.Active)
            {
                MultiWindowManager.AddWaveform(cp);
            }
        }
        #endregion

        #region Operate Form
        public Form MakeOperateForm(String name, Control src, PopOrientation orientation, Func<FloatForm> creator)
        {
            var pt = orientation switch
            {
                PopOrientation.Above => src.PointToScreen(new Point(src.Width / 2, 0)),
                PopOrientation.Under => src.PointToScreen(new Point(src.Width / 2, src.Height)),
                PopOrientation.Left => src.PointToScreen(new Point(0, src.Height / 2)),
                PopOrientation.Right => src.PointToScreen(new Point(src.Width, src.Height / 2)),
                _ => src.PointToScreen(new Point(src.Width / 2, src.Height / 2)),
            };
            //pt = PointToClient(pt);
            return MakeOperateForm(name, pt, orientation, creator);
        }
        public Form MakeOperateForm(String name, Point scrpos, PopOrientation orientation, Func<FloatForm> creator, FormType formType = FormType.SettingForm)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                return null;
            }
            int edgedistance = 5; //窗体距离边界的距离
            var form = creator();
            if (form is null)
            {
                return null;
            }

            form.Tag = name;

            //Point pt = position;
            Point pt = PointToClient(scrpos);

            switch (orientation)
            {
                case PopOrientation.Above:
                    pt -= new Size(form.Width / 2, form.Height + edgedistance);
                    break;
                case PopOrientation.Under:
                    pt -= new Size(form.Width / 2, -edgedistance);
                    break;
                case PopOrientation.Left:
                    break;
                case PopOrientation.Right:
                    break;
            }

            //窗口位置要是超出左右上下边界时，调整窗口的位置
            if (pt.X < edgedistance)
            {
                pt.X = edgedistance;
            }
            else if (pt.X + form.Width > Width)
            {
                pt.X = Width - form.Width;
            }

            if (pt.Y < edgedistance)
            {
                pt.Y = edgedistance;
            }
            else if (pt.Y + form.Height > Height)
            {
                pt.Y = Height - form.Height;
            }

            form.Location = pt;

            EventBroker.Instance.GetEvent<FormEventArgs>().Publish(form, new() { Current = form, Type = formType });
            return form;
        }

        public void CloseCfgForm(IBadge badge)
        {
            EventBroker.Instance.GetEvent<FormCloseEventArgs>().Publish(this, new() { Name = badge.Name, Type = FormType.SettingForm });
        }

        private void AutoSet()
        {
            if (PlatformUIManager.Default.Platform.Attribute.AutosetWaitingMode)
            {
                AutoSetWaitingMode();
            }
            else
            {
                AutoSetNormal();
            }
        }



        private void AutoSetNormal()
        {
            try
            {
                if (Presenter.CurrentTrigger is TrigEdgePrsnt tep)
                {
                    // 边沿触发时，按下AutoSet键时，回到DC模式。
                    tep.Coupling = TriggerCoupling.DC;
                }

                Presenter.AutoSet.Run(new System.Threading.CancellationToken());
                if (!Presenter.AutoSet.AutosetBOK)
                {
                    return;
                }
                MakeAutoSetSettingForm();
            }
            catch (Exception ex)
            {
                DsoPrsnt.KeyBoardLockEnable = false;
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Warn));
            }
        }

        private async void AutoSetWaitingMode()
        {
            var weakipenabled = WeakTip.Default.Enabled;
            var wf = new WaittingForm();
            try
            {
                wf.Opacity = 0.95;
                wf.TopMost = true;
                wf.DisableMouseInput();
                //wf.Refresh();
                WeakTip.Default.Enabled = false;
                if (Presenter.CurrentTrigger is TrigEdgePrsnt tep)
                {
                    // 边沿触发时，按下AutoSet键时，回到DC模式。
                    tep.Coupling = TriggerCoupling.DC;
                }

                wf.Show();
                wf.Active = true;
                var bOK = await Task.Run(() =>
                {
                    Presenter.AutoSet.Run(new System.Threading.CancellationToken());
                    return Presenter.AutoSet.AutosetBOK;
                });

                if (!wf.IsDisposed)
                {
                    wf.Close();
                }
                switch (Presenter.AutoSet.AtSetResult)
                {
                    case AutoSetResult.NoSignal:
                        WeakTip.Default.Write("AutoSet", MsgTipId.NoSignal, emergent: false, "", 5);
                        break;
                    case AutoSetResult.ReadDataTimeout:
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs("Autosetting fail,reading channel data timeout!", EventBus.LogLevel.Debug));
                        break;
                    case AutoSetResult.ReadCymometeTimeout:
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs("Autosetting fail,reading cymomete timeout!", EventBus.LogLevel.Debug));
                        break;
                    case AutoSetResult.Finish:
                        break;
                    default:
                        break;
                }
                Presenter.AutoSet.AtSetResult = AutoSetResult.Finish;
                if (!bOK)
                {
                    WeakTip.Default.Enabled = weakipenabled;
                    return;
                }

                MakeAutoSetSettingForm();
                WeakTip.Default.Enabled = weakipenabled;
            }
            catch (Exception ex)
            {
                if (!(wf?.IsDisposed ?? false))
                {
                    wf.Close();
                }
                Presenter.AutoSet.AtSetResult = AutoSetResult.Finish;
                WeakTip.Default.Enabled = weakipenabled;
                DsoPrsnt.KeyBoardLockEnable = weakipenabled;
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Warn));
            }
        }

        private void AutoCalibration()
        {
            Task.Run(() =>
            {
                Invoke(() =>
                {
                    var wf = new WaittingForm();
                    try
                    {
                        wf.Opacity = 0.95;
                        wf.TopMost = true;
                        wf.DisableMouseInput();
                     

                        wf.Show();
                        wf.Active = true;

                        AutoCalibrationForm caliform = new();
                        caliform.Owner = this;
                        caliform.ShowDialog();


                        if (!wf.IsDisposed)
                        {
                            wf.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!(wf?.IsDisposed ?? false))
                        {
                            wf.Close();
                        }
                        DsoPrsnt.KeyBoardLockEnable = false;
                        EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Warn));
                    }
                });
            });
        }

        /// <summary>
        /// 探头用户校准
        /// </summary>
        /// <param name="chl"></param>
        public void ProbeCalibrationWithUser(ChannelId chl)
        {
            ProbeUserCaliForm af = new(chl) { StartPosition = FormStartPosition.CenterScreen };
            af.TopMost = true;
            af.ShowDialogByEvent();
        }

        /// <summary>
        /// 探头出厂校准工具未完成前，出厂校准实现暂时依附于示波器软件
        /// </summary>
        /// <param name="chl"></param>
        public void ProbeCalibrationWithFact(ChannelId chl)
        {
            Task.Run(() =>
            {
                Invoke(() =>
                {
                    try
                    {
                        ProbeCalibrationForm caliform = new();
                        caliform.ProbeAtChlId = chl;// Presenter.Id;
                        caliform.Owner = (Program.Oscilloscope.View as DsoForm);
                        caliform.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        DsoPrsnt.KeyBoardLockEnable = false;
                        EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Warn));
                    }
                });
            });
        }

        private void SaveDataSoure()
        {

            Task.Run(() =>
            {
                Invoke(() =>
                {
                    try
                    {
                        SaveDataForm caliform = new();
                        caliform.Owner = this;
                        caliform.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        DsoPrsnt.KeyBoardLockEnable = false;
                        EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Warn));
                    }
                });
            });
        }

        private void MakeAutoSetSettingForm()
        {
            if (AutosetSettingForm.Default.IsShow)
            {
                AutosetSettingForm.Default.ResetShowTime();
                return;
            }
            AutosetSettingForm.Default.Location = new Point((this.Width - AutosetSettingForm.Default.Width) / 2, DsoTopStrip.Height + AutosetSettingForm.Default.Height);
            EventBroker.Instance.GetEvent<FormEventArgs>().Publish(this, new FormEventArgs() { Current = AutosetSettingForm.Default, Type = FormType.SettingForm });
        }

        /// <summary>
        /// 关闭提示弹窗
        /// </summary>
        /// <Remark>作者：彭博 创建日期：2024/2/26 14:14:00 创建原因：主动关闭提示窗口，保证只有一个提示窗口 </Remark>
        internal void CloseFormsManagerMessageForm()
        {
            _FormsManager.CloseMessageFormSubscrip();
        }

        #endregion

        #region Badge Info     
        public void InitBadge()
        {
            IWaveformFigure mainfig = MultiWindowManager.MainFigure;

            foreach (var ap in Presenter.TryGetRange(c => c.Id.IsAnalog()))
            {
                if (GetBadge(ap.Id) is null)
                {
                    var ainfo = new AnalogInfo()
                    {
                        Name = $"{ap.Id}Info",
                        Text = ap.Id.ToString(),
                    };
                    ap.TryAddView(ainfo);
                    //!!!Bind DsoTopStrip to analog presenter for trigger's unit change
                    ap.TryAddView(DsoTopStrip);

                    ainfo.Presenter = ap as AnalogPrsnt;

                    DsoInfoStrip.AddBadge(ainfo);
                }
                ap.WindowId = mainfig.WindowId;
                if (ap.Active)
                {
                    DsoPrsnt.FocusId = ap.Id;
                }
            }

            foreach (var mp in Presenter.TryGetRange(c => c.Id.IsMath()))
            {
                if (mp.Active)
                {
                    if (GetBadge(mp.Id) is null)
                    {
                        TryAddMathUI((MathPrsnt)mp);
                    }
                }
                else
                {
                    if (GetBadge(mp.Id) is not null)
                    {
                        RemoveWaveformUI(mp);
                    }
                }

            }

            foreach (var rp in Presenter.TryGetRange(c => c.Id.IsReference()))
            {
                if (rp.Active)
                {
                    if (GetBadge(rp.Id) is null)
                    {
                        TryAddRefUI((ReferencePrsnt)rp);
                    }
                }
                else
                {
                    if (GetBadge(rp.Id) is not null)
                    {
                        RemoveWaveformUI(rp);
                    }
                }

            }

            if (Constants.ENABLE_LA)
            {
                foreach (var dp in Presenter.TryGetRange(c => c.Id.IsDigital()))
                {
                    if (dp.Active)
                    {
                        if (GetBadge(dp.Id) is null)
                        {
                            TryAddDigiUI((DigitalPrsnt)dp);
                        }
                    }
                    else
                    {
                        if (GetBadge(dp.Id) is not null)
                        {
                            RemoveWaveformUI(dp);
                        }
                    }

                }
            }


            if (Constants.ENABLE_RF)
            {
                foreach (var rfp in Presenter.TryGetRange(c => c.Id.IsRadioFrequency()))
                {
                    if (rfp.Active)
                    {
                        if (GetBadge(rfp.Id) is null)
                        {
                            TryAddRFUI((RadioFrequencyPrsnt)rfp);
                        }
                    }
                    else
                    {
                        if (GetBadge(rfp.Id) is not null)
                        {
                            RemoveWaveformUI(rfp);
                        }
                    }
                }

                foreach (var rfp in Presenter.TryGetRange(c => c.Id.IsAmpVSTime()))
                {
                    if (rfp.Active)
                    {
                        if (GetBadge(rfp.Id) is null)
                        {
                            TryAddAVTUI((AmpVSTimePrsnt)rfp);
                        }
                    }
                    else
                    {
                        if (GetBadge(rfp.Id) is not null)
                        {
                            RemoveWaveformUI(rfp);
                        }
                    }
                }
                foreach (var rfp in Presenter.TryGetRange(c => c.Id.IsPhaseVSTime()))
                {
                    if (rfp.Active)
                    {
                        if (GetBadge(rfp.Id) is null)
                        {
                            TryAddPVTUI((PhaseVSTimePrsnt)rfp);
                        }
                    }
                    else
                    {
                        if (GetBadge(rfp.Id) is not null)
                        {
                            RemoveWaveformUI(rfp);
                        }
                    }
                }
                foreach (var rfp in Presenter.TryGetRange(c => c.Id.IsPhaseVSFrequency()))
                {
                    if (rfp.Active)
                    {
                        if (GetBadge(rfp.Id) is null)
                        {
                            TryAddPVFUI((PhaseVSFrequencyPrsnt)rfp);
                        }
                    }
                    else
                    {
                        if (GetBadge(rfp.Id) is not null)
                        {
                            RemoveWaveformUI(rfp);
                        }
                    }
                }
                foreach (var rfp in Presenter.TryGetRange(c => c.Id.IsFrequencyVSTime()))
                {
                    if (rfp.Active)
                    {
                        if (GetBadge(rfp.Id) is null)
                        {
                            TryAddFVTUI((FrequencyVSTimePrsnt)rfp);
                        }
                    }
                    else
                    {
                        if (GetBadge(rfp.Id) is not null)
                        {
                            RemoveWaveformUI(rfp);
                        }
                    }
                }
            }

            if (Constants.ENABLE_AWG)
            {
                foreach (ChannelId id in ChannelIdExt.GetAWGs())
                {
                    ArbWfmGenPrsnt awgp = Presenter.GetWfmGenerator(id);
                    if (awgp.Active)
                    {
                        if (GetBadge(awgp.Id) is null)
                        {
                            MakeAwgInfo(awgp);
                        }
                    }
                    else
                    {
                        if (GetBadge(awgp.Id) is not null)
                        {
                            RemoveWaveformUI(awgp);
                        }
                    }
                }
            }

            if (Constants.ENABLE_BUS)
            {
                foreach (var mp in Presenter.TryGetRange(c => c.Id.IsDecode()))
                {
                    if (mp.Active)
                    {
                        if (GetBadge(mp.Id) is null)
                        {
                            TryAddDecodeUI(mp);
                        }
                    }
                    else
                    {
                        if (GetBadge(mp.Id) is not null)
                        {
                            RemoveWaveformUI(mp);
                        }
                    }

                }
            }

            if (Constants.ENABLE_SDA)
            {
                if (Presenter.Jitter != null)
                {
                    if (Presenter.Jitter.Active)
                    {
                        if (GetBadge(Presenter.Jitter.Id) is null)
                        {
                            TryAddJitterInfo(Presenter.Jitter);
                        }
                    }
                }
            }

            _IsShowForm = true;
        }

        public void UpdateAllAnalogInfo(ChannelId id = ChannelId.C1)
        {
            AnalogInfo current = null;
            var ach = DsoPrsnt.DefaultDsoPrsnt.TryGetRange(x => x.Id.IsAnalog()).ToList();
            foreach (var ch in ach)
            {
                var badge = GetBadge(ch.Id);
                if (badge is AnalogInfo ainfo)
                {
                    ainfo.UpdateView(null, null);
                    if (id == ch.Id)
                    {
                        current = ainfo;
                    }
                }
            }

            current.UpdateView(null, null);
        }
        /// <summary>
        /// 临时解决单通道打开基线变粗的问题
        /// </summary>
        private ChannelId testid = ChannelId.M1;
        public void InitChannelActive()
        {
            var chnlprsntlist = Presenter.TryGetRange(c => c.Id.IsAnalog() && c.Active);
            if (chnlprsntlist != null && chnlprsntlist.Count == 1)
            {

                foreach (var item in Presenter.TryGetRange(c => c.Id.IsAnalog() && c.Active == false))
                {
                    item.Active = true;
                    testid = item.Id;
                    break;
                }

                if (testid.IsAnalog())
                {
                    Task.Run(() =>
                    {
                        Int64 num = 0;
                        while (num < 100)
                        {
                            Thread.Sleep(1);
                            num++;
                        }
                        if (Program.Oscilloscope.TryGetChannel(testid, out IChnlPrsnt channel))
                        {
                            channel.Active = false;
                        }
                    });
                }
            }

            #region 临时解决首次未开快采时，在Scan挡位下开快采，然后暂停，没有波形的问题（FPGA没时间解决，软件暂时处理）
            AnaChnlStorageMode temp = Presenter.Timebase.StorageMode;
            AnaChnlTimebaseIndex scaleindextemp = Presenter.Timebase.ScaleIndex;
            if (Presenter.Timebase.StorageMode != AnaChnlStorageMode.Fast)
            {
                Presenter.Timebase.StorageMode = AnaChnlStorageMode.Fast;
                Presenter.Timebase.ScaleIndex = AnaChnlTimebaseIndex.Lv20m;
                Task.Run(() =>
                {
                    Int64 num = 0;
                    while (num < 200)
                    {
                        Thread.Sleep(1);
                        num++;
                    }
                    Presenter.Timebase.StorageMode = temp;
                    Presenter.Timebase.ScaleIndex = scaleindextemp;
                });
            }
            #endregion
        }

        public void LoadLastSettingFunction()
        {
            try
            {
                String path = Environment.CurrentDirectory + "\\LastSettings.set";
                if (!System.IO.File.Exists(path))
                {
                    WeakTip.Default.Write(nameof(LoadLastSettingFunction), MsgTipId.ReadingFailed);
                    return;
                }
                using MemoryStream memorystream = new(System.IO.File.ReadAllBytes(path));
                var setting = BinaryConvert.Deserialize<SysSettings>(memorystream);
                if (setting != null)
                {
                    setting.LoadFunction();
                }
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex.ToString(), EventBus.LogLevel.Error));
            }

            if (Presenter.PassFail?.Active ?? false)
            {
                this.Invoke(new Action(() =>
                {
                    PassFailApp.Default.ShowInfoForm();
                }));
            }
            //启动关闭其他右键菜单
            CloseAllContextMenusWithESC();
        }

        //Open new math channel
        public MathPrsnt TryAddMathWaveform(Action<MathPrsnt> initialize = null)
        {
            //!!!Notice: ensure the MathPrsnt exist
            if (Presenter.TryGetRange(c => c.Id.IsMath() && c.Id <= ChannelIdExt.MaxMChId).FirstOrDefault(p => !p.Active) is not MathPrsnt mprsnt)
            {
                WeakTip.Default.Write("Math", MsgTipId.NoMoreChannels);
                return null;
            }

            if (initialize == null)
            {
                if (mprsnt.Args is MathHistArg || mprsnt.Args is MathTrackArg || mprsnt.Args is MathTrendArg)
                {
                    mprsnt.GetOrMakeArg(MathType.Binary);
                }
            }
            else
            {
                initialize.Invoke(mprsnt);
            }
            if (mprsnt.Args.Type == MathType.FFT)
            {
                var fftmath = Presenter.TryGetRange(c => c.Id.IsMath() && c.Id <= ChannelIdExt.MaxMChId && c.Active).Select(p => (MathPrsnt)p).Where(m => m.Args.Type == MathType.FFT).ToList();
                if (fftmath != null && fftmath.Count >= 2)
                {
                    mprsnt.GetOrMakeArg(MathType.Binary);
                }
            }
            mprsnt.Active = true; //主要耗时在界面新建窗口
            UpdateFFT();
            return mprsnt;
        }

        private Boolean TryAddMathUI(MathPrsnt mprsnt)
        {
            MathInfo minfo = new(mprsnt)
            {
                Name = mprsnt.Id.ToString() + "Info",
                Text = mprsnt.Id.ToString(),
                Dock = DockStyle.None,
            };

            DsoInfoStrip.AddBadge(minfo);

            if (mprsnt.IsOnlyIndependenForm())
            {
                mprsnt.WindowId = ChannelPrsnt.GetNewWindowId();
            }
            else
            {
                mprsnt.WindowId = MultiWindowManager.MainFigure?.WindowId ?? ChannelPrsnt.GetNewWindowId();
            }
            if (TriggerPrsnt.Mode == TriggerMode.Auto)
            {
                Presenter.Timebase.LimitScan(MsgTipId.MathIsNotSupportedInScan);
            }

            // <Remark>更改人：彭博 创建日期：2024/2/26 14:24:00  原因：当初始控件被创建时，显示本窗体 </Remark>
            if (_IsShowForm && !(mprsnt.Args is MathCustomArg && mprsnt.Args.Occupier != null && mprsnt.Args.IsJitterTypeOccupier()))
            {
                minfo.ShowForm();
            }
            return true;
        }

        //private FileBrowserForm _RdForm = null;
        public Boolean TryAddRefWaveform()
        {
            if (_RefKeycodeCtrl == true)
                return false;
            _RefKeycodeCtrl = true;

            var activerchs = Presenter.TryGetRange(c => c.Active && c.Id.IsReference());
            if (activerchs.Count >= ChannelIdExt.RefChnlNum)
            {
                _RefKeycodeCtrl = false;
                return false;
            }
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Binary(*.bin)|*.bin| CSV(*.csv) |*.csv";
            dialog.InitialDirectory = _RefInitialDirectory;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                foreach (ChannelId id in ChannelIdExt.GetReferences())
                {
                    Presenter.TryGetChannel(id, out IChnlPrsnt cp);
                    if (cp is null || !cp.Active)
                    {
                        FileInfo fileInfo = new FileInfo(dialog.FileName);
                        _RefInitialDirectory = string.IsNullOrEmpty(fileInfo.DirectoryName) ? _RefInitialDirectory : fileInfo.DirectoryName;
                        ReferencePrsnt rprsnt = null;
                        if (fileInfo.Extension == "." + WfmFormat.Binary.GetAlias())
                        {
                            if (ReferencePrsnt.TryRead(id, Presenter, dialog.FileName, ref rprsnt))
                            {
                                Presenter.AddChannel(id, rprsnt);
                                rprsnt.Active = true;
                                _RefKeycodeCtrl = false;
                                return true;
                            }
                        }
                        if (fileInfo.Extension == "." + WfmFormat.CSV.GetAlias())
                        {
                            if (ReferencePrsnt.TryReadSVG(id, Presenter, dialog.FileName, ref rprsnt))
                            {
                                Presenter.AddChannel(id, rprsnt);
                                rprsnt.Active = true;
                                _RefKeycodeCtrl = false;
                                return true;
                            }
                        }
                        WeakTip.Default.Write("REF", MsgTipId.RefFileError1);
                    }
                }
                _RefKeycodeCtrl = false;
                return false;
            }
            _RefKeycodeCtrl = false;
            return true;
        }

        public Boolean TryAddRefWaveform(ChannelId channelId)
        {
            if (_RefKeycodeCtrl == true)
                return false;
            _RefKeycodeCtrl = true;

            var activerchs = Presenter.TryGetRange(c => c.Active && c.Id == channelId);
            if (activerchs.Count == 1)
            {
                var rch = activerchs.First();
                if (DsoPrsnt.FocusId == rch.Id)
                {
                    rch.Active = false;
                    RemoveWaveformUI(rch);
                }
                else
                {
                    DsoPrsnt.FocusId = rch.Id;
                }
                _RefKeycodeCtrl = false;
                return true;
            }
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Binary(*.bin)|*.bin| CSV(*.csv) |*.csv";
            dialog.InitialDirectory = _RefInitialDirectory;

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {

                Presenter.TryGetChannel(channelId, out IChnlPrsnt cp);
                if (cp is null || !cp.Active)
                {
                    FileInfo fileInfo = new FileInfo(dialog.FileName);
                    _RefInitialDirectory = string.IsNullOrEmpty(fileInfo.DirectoryName) ? _RefInitialDirectory : fileInfo.DirectoryName;
                    ReferencePrsnt rprsnt = null;
                    if (fileInfo.Extension == "." + WfmFormat.Binary.GetAlias())
                    {
                        if (ReferencePrsnt.TryRead(channelId, Presenter, dialog.FileName, ref rprsnt))
                        {
                            Presenter.AddChannel(channelId, rprsnt);
                            rprsnt.Active = true;
                            _RefKeycodeCtrl = false;
                            return true;
                        }
                    }
                    if (fileInfo.Extension == "." + WfmFormat.CSV.GetAlias())
                    {
                        if (ReferencePrsnt.TryReadSVG(channelId, Presenter, dialog.FileName, ref rprsnt))
                        {
                            Presenter.AddChannel(channelId, rprsnt);
                            rprsnt.Active = true;
                            _RefKeycodeCtrl = false;
                            return true;
                        }
                    }
                }

                _RefKeycodeCtrl = false;
                return false;
            }
            _RefKeycodeCtrl = false;
            return true;
        }

        private Boolean TryAddRefUI(ReferencePrsnt rprsnt)
        {
            RefInfo rinfo = new(rprsnt)
            {
                Name = rprsnt.Id.ToString() + "Info",
                Text = rprsnt.Id.ToString(),
                Dock = DockStyle.None,
            };

            DsoInfoStrip.AddBadge(rinfo);

            rprsnt.WindowId = MultiWindowManager.MainFigure?.WindowId ?? ChannelPrsnt.GetNewWindowId();

            // <Remark>更改人：彭博 创建日期：2024/2/26 14:24:00  原因：当初始控件被创建时，显示本窗体 </Remark>
            if (_IsShowForm)
            {
                rinfo.ShowForm();
            }
            return true;
        }

        public Boolean TryAddDigiWaveform()
        {
            if (OptionManager.Default.Checked(OptionType.LA) == false)
                return false;
            if (Presenter.TryGetRange(c => c.Id.IsDigital()).FirstOrDefault(p => !p.Active) is not DigitalPrsnt dprsnt)
            {
                return false;
            }

            for (Int32 i = 0; i < 4; i++)
            {
                dprsnt.SetActiveAt(i, true);
            }

            return true;
        }

        private Boolean TryAddDigiUI(DigitalPrsnt dprsnt)
        {
            DigitalInfo dinfo = new(dprsnt)
            {
                Name = "DigiGroupInfo",
                Text = dprsnt.Type.GetDescription_Lang(),
                Dock = DockStyle.None,
                ContentAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Consolas", 14F),
            };

            if (!_badgeTextLangKey.ContainsKey(dinfo.Name))
                _badgeTextLangKey.Add(dinfo.Name, dprsnt.Type);


            DsoInfoStrip.AddBadge(dinfo);

            dprsnt.WindowId = MultiWindowManager.MainFigure?.WindowId ?? ChannelPrsnt.GetNewWindowId();

            // <Remark>更改人：彭博 创建日期：2024/2/26 14:24:00  原因：当初始控件被创建时，显示本窗体 </Remark>
            if (_IsShowForm)
            {
                dinfo.ShowForm();
            }
            return true;
        }

        public Boolean TryAddRFWaveform()
        {
            if (Presenter.TryGetRange(c => c.Id.IsRadioFrequency()).FirstOrDefault(p => !p.Active) is not RadioFrequencyPrsnt rfprsnt)
            {
                return false;
            }
            rfprsnt.Active = true;
            return true;
        }
        public Boolean TryAddAVTWaveform(AmpVSTimePrsnt avtprsnt)
        {
            avtprsnt.Active = true;
            return true;
        }
        public Boolean TryAddPVTWaveform(PhaseVSTimePrsnt pvtprsnt)
        {
            pvtprsnt.Active = true;
            return true;
        }
        public Boolean TryAddPVFWaveform(PhaseVSFrequencyPrsnt pvfprsnt)
        {
            pvfprsnt.Active = true;
            return true;
        }
        public Boolean TryAddFVTWaveform(FrequencyVSTimePrsnt fvtprsnt)
        {
            fvtprsnt.Active = true;
            return true;
        }

        private Boolean TryAddRFUI(RadioFrequencyPrsnt rfprsnt)
        {
            RFInfo rfinfo = new(rfprsnt)
            {
                Name = "RFInfo",
                Text = rfprsnt.Type.GetDescription_Lang(),
                Dock = DockStyle.None,
            };

            if (!_badgeTextLangKey.ContainsKey(rfprsnt.Name))
                _badgeTextLangKey.Add(rfprsnt.Name, rfprsnt.Type);

            DsoInfoStrip.AddBadge(rfinfo);

            rfprsnt.WindowId = ChannelPrsnt.GetNewWindowId();

            return true;
        }
        private Boolean TryAddAVTUI(AmpVSTimePrsnt avtprsnt)
        {
            AVTInfo avtinfo = new(avtprsnt)
            {
                Name = "AVTInfo",
                Text = avtprsnt.Type.GetDescription_Lang(),
                Dock = DockStyle.None,
            };

            if (!_badgeTextLangKey.ContainsKey(avtprsnt.Name))
                _badgeTextLangKey.Add(avtprsnt.Name, avtprsnt.Type);

            DsoInfoStrip.AddBadge(avtinfo);

            avtprsnt.WindowId = ChannelPrsnt.GetNewWindowId();

            return true;
        }
        private Boolean TryAddPVFUI(PhaseVSFrequencyPrsnt pvfprsnt)
        {
            PVFInfo pvfinfo = new(pvfprsnt)
            {
                Name = "PVFInfo",
                Text = pvfprsnt.Type.GetDescription_Lang(),
                Dock = DockStyle.None,
            };

            if (!_badgeTextLangKey.ContainsKey(pvfprsnt.Name))
                _badgeTextLangKey.Add(pvfprsnt.Name, pvfprsnt.Type);

            DsoInfoStrip.AddBadge(pvfinfo);

            pvfprsnt.WindowId = ChannelPrsnt.GetNewWindowId();

            return true;
        }
        private Boolean TryAddPVTUI(PhaseVSTimePrsnt pvtprsnt)
        {
            PVTInfo pvtinfo = new(pvtprsnt)
            {
                Name = "PVTInfo",
                Text = pvtprsnt.Type.GetDescription_Lang(),
                Dock = DockStyle.None,
            };

            if (!_badgeTextLangKey.ContainsKey(pvtprsnt.Name))
                _badgeTextLangKey.Add(pvtprsnt.Name, pvtprsnt.Type);

            DsoInfoStrip.AddBadge(pvtinfo);

            pvtprsnt.WindowId = ChannelPrsnt.GetNewWindowId();

            return true;
        }
        private Boolean TryAddFVTUI(FrequencyVSTimePrsnt fvtprsnt)
        {
            FVTInfo fvtinfo = new(fvtprsnt)
            {
                Name = "FVTInfo",
                Text = fvtprsnt.Type.GetDescription_Lang(),
                Dock = DockStyle.None,
            };

            if (!_badgeTextLangKey.ContainsKey(fvtprsnt.Name))
                _badgeTextLangKey.Add(fvtprsnt.Name, fvtprsnt.Type);

            DsoInfoStrip.AddBadge(fvtinfo);

            fvtprsnt.WindowId = ChannelPrsnt.GetNewWindowId();

            return true;
        }

        public Boolean TryAddDecodeWaveform()
        {
            IChnlPrsnt bprsnt = Presenter.TryGetRange(c => c.Id.IsDecode()).FirstOrDefault(p => !p.Active);
            if (bprsnt is null)
            {
                return false;
            }

            bprsnt.Active = true;

            return true;
        }

        private Boolean TryAddDecodeUI(IChnlPrsnt dprsnt)
        {
            DecodeInfo binfo = new(dprsnt)
            {
                Name = dprsnt.Id.ToString() + "Info",
                Text = dprsnt.Id.ToString(),
                Dock = DockStyle.None,
            };

            DsoInfoStrip.AddBadge(binfo);
            dprsnt.WindowId = MultiWindowManager.MainFigure?.WindowId ?? ChannelPrsnt.GetNewWindowId();
            Presenter.Timebase.LimitScan(MsgTipId.DecodeIsNotSupportedInScan);
            Presenter.Timebase.LimitScan(MsgTipId.DecodeIsSupportedMinTimebase, true);

            // <Remark>更改人：彭博 创建日期：2024/2/26 14:24:00  原因：当初始控件被创建时，显示本窗体 </Remark>
            if (_IsShowForm)
            {
                binfo.ShowForm();
            }
            return true;
        }

        public void RemoveWaveformUI(IBadge badge)
        {
            if (badge is ChannelPrsnt cp)
            {
                MultiWindowManager.RemoveWaveform(cp);
                cp.WindowId = null;
            }

            CloseCfgForm(badge);

            Invoke(() =>
            {
                DsoInfoStrip.RemoveBadge(badge.Id);
                if (badge.Id.IsMath())
                {
                    UpdateFFT();
                }
            });

            RemoveZoomRectangleAreaEventArgs?.Invoke(this, badge.Id);
        }

        public void UpdateFFT()
        {
            DsoTopStrip.UpdateFFT();
        }

        public Boolean TryAddAwgInfo(ChannelId id)
        {
            if (id.IsAWG())
            {
                if (OptionManager.Default.Checked(OptionType.AWG) == false)
                    return false;

                ArbWfmGenPrsnt awgprsnt = Presenter.GetWfmGenerator(id);
                if (DsoInfoStrip.GetBadge(awgprsnt.Id) is null)
                {
                    MakeAwgInfo(awgprsnt);
                    return true;
                }
            }
            return false;
        }

        private void MakeAwgInfo(ArbWfmGenPrsnt awgprsnt)
        {
            AWG.AWGInfo awginfo = new(awgprsnt)
            {
                Name = awgprsnt.Id.ToString() + "Info",
                NameWidth = 90,
                Text = awgprsnt.Name.Replace("AW", ""),
                Dock = DockStyle.None,
                Row1stLangName = "AWGInfo_Frequency",
                Row2stLangName = "AWGInfo_Amplitude",
                Row3stLangName = "AWGInfo_Offset",
                Row1stName = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AWGInfo_Frequency"), // Properties.Resources.AWGInfo_Frequency,
                Row2ndName = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AWGInfo_Amplitude"), // Properties.Resources.AWGInfo_Amplitude,
                Row3rdName = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AWGInfo_Offset"), // Properties.Resources.AWGInfo_Offset,
            };

            Invoke(() => DsoInfoStrip.AddBadge(awginfo));

            // <Remark>更改人：彭博 创建日期：2024/2/26 14:24:00  原因：当初始控件被创建时，显示本窗体 </Remark>
            if (_IsShowForm)
            {
                Invoke(() => awginfo.ShowForm());
            }
        }

        public Boolean TryAddPowerInfo(PowerAnalysisPrsnt powerAnalysisPrsnt, Func<PowerAnalysisPrsnt, IPwrAnalysisView> creator)
        {
            var pqinfo = creator(powerAnalysisPrsnt);
            powerAnalysisPrsnt.TryAddView(pqinfo);
            DsoInfoStrip.AddBadge(pqinfo);
            return true;
        }

        private void TryAddPowerInfo(PowerAnalysisPrsnt prsnt, PowerAnalysisOpt pao)
        {
            switch (pao)
            {
                case PowerAnalysisOpt.PowerQuality:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(prsnt, (p) =>
                    {
                        PowerQualityInfo pqinfo = new()
                        {
                            Name = "PowerQualityInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrVoltageSource",
                            Row2LangKey = "PwrCurrentSource",
                            Row3LangKey = "GongLvTu",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                            Row3rdName = LanguageHelper.GetPowerAnalysisString("GongLvTu"),
                        };
                        return pqinfo;
                    }))
                    {
                        prsnt.QualityPrsnt.Value.Statistics = true;
                        PowerAnalysisApp.Default.ShowDataTableForm(prsnt);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.Harmonic:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(prsnt, (p) =>
                    {
                        PwrHarmonicInfo pqinfo = new()
                        {
                            Name = "PwrHarmonicInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrVoltageSource",
                            Row2LangKey = "PwrCurrentSource",
                            Row3LangKey = "PwrHarmonicNum",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                            Row3rdName = LanguageHelper.GetPowerAnalysisString("PwrHarmonicNum"),
                        };
                        return pqinfo;
                    }))
                    {
                        (Program.Oscilloscope.View as DsoForm).TryAddPwrHarmonicUI(prsnt);
                        PowerAnalysisApp.Default.ShowDataTableForm(prsnt);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.Ripple:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(prsnt, (p) =>
                    {
                        PwrRippleInfo pqinfo = new()
                        {
                            Name = "PwrRippleInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrVoltageSource",
                            Row2LangKey = "PwrCurrentSource",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                        };
                        return pqinfo;
                    }))
                    {
                        PowerAnalysisApp.Default.ShowDataTableForm(prsnt);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.Modulation:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(prsnt, (p) =>
                    {
                        PwrModulationInfo pqinfo = new()
                        {
                            Name = "PwrModulationInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrVoltageSource",
                            Row2LangKey = "PwrCurrentSource",
                            //Row3LangKey = "GongLvTu",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                            //Row3rdName = LanguageHelper.GetPowerAnalysisString("GongLvTu"),
                        };
                        return pqinfo;
                    }))
                    {
                        prsnt.QualityPrsnt.Value.Statistics = true;
                        PowerAnalysisApp.Default.ShowDataTableForm(prsnt);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.SwitchingLoss:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(prsnt, (p) =>
                    {
                        PwrSwitchingLossInfo pqinfo = new()
                        {
                            Name = "PwrSwitchingLossInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrVoltageSource",
                            Row2LangKey = "PwrCurrentSource",
                            Row3LangKey = "GongLvTu",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                            Row3rdName = LanguageHelper.GetPowerAnalysisString("GongLvTu"),
                        };
                        return pqinfo;
                    }))
                    {
                        prsnt.SwitchingLossPrsnt.Value.Statistics = true;
                        PowerAnalysisApp.Default.ShowDataTableForm(prsnt);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.SafeOperationArea:
                    {
                        if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(prsnt, (p) =>
                        {
                            PwrSOAInfo pqinfo = new()
                            {
                                Name = "PwrSOAInfo",
                                NameWidth = 150,
                                Text = p.Id.ToString(),
                                Dock = DockStyle.None,
                                Presenter = p,
                                Row1LangKey = "PwrVoltageSource",
                                Row2LangKey = "PwrCurrentSource",
                                Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                                Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                            };
                            return pqinfo;
                        }))
                        {
                            (Program.Oscilloscope.View as DsoForm).TryAddSOAUI(prsnt);
                            return;
                        }
                        WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    }
                    break;
                case PowerAnalysisOpt.LoopAnalysis:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(prsnt, (p) =>
                    {
                        PwrLoopAnalysisInfo pqinfo = new()
                        {
                            Name = "PwrLoopAnalysisInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "LoopAnalysisInputSource",
                            Row2LangKey = "LoopAnalysisOutputSource",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("LoopAnalysisInputSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("LoopAnalysisOutputSource"),
                            //Row3rdName = LanguageHelper.GetPowerAnalysisString("PwrHarmonicNum"),
                        };
                        return pqinfo;
                    }))
                    {
                        (Program.Oscilloscope.View as DsoForm).TryAddLoopAnalysisUI(prsnt);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.InrushCurrent:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(prsnt, (p) =>
                    {
                        PwrInrushCurrentInfo pqinfo = new()
                        {
                            Name = "PwrInrushCurrentInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrCurrentSource",
                            Row2LangKey = "PwrCurrentSource",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                        };
                        return pqinfo;
                    }))
                    {
                        PowerAnalysisApp.Default.ShowDataTableForm(prsnt);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.PowerEfficency:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(prsnt, (p) =>
                    {
                        PwrEfficiencyInfo pqinfo = new()
                        {
                            Name = "PowerEfficiencyInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "ShuRuYuan",
                            Row2LangKey = "ShuChuYuan",
                            Row3LangKey = "GongLvTu",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("ShuRuYuan"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("ShuChuYuan"),
                            Row3rdName = LanguageHelper.GetPowerAnalysisString("GongLvTu"),
                        };
                        return pqinfo;
                    }))
                    {
                        PowerAnalysisApp.Default.ShowDataTableForm(prsnt);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.RDSon:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(prsnt, (p) =>
                    {
                        PwrRDSonInfo pqinfo = new()
                        {
                            Name = "PwrRDSonInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrVoltageSource",
                            Row2LangKey = "PwrCurrentSource",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                        };
                        return pqinfo;
                    }))
                    {
                        PowerAnalysisApp.Default.ShowDataTableForm(prsnt);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.TurnOnOff:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(prsnt, (p) =>
                    {
                        PwrOnOffTimeInfo pqinfo = new()
                        {
                            Name = "PwrOnOffTimeInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrInVoltageSource",
                            Row2LangKey = "PwrOutVoltageSource",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrInVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrOutVoltageSource"),
                        };
                        return pqinfo;
                    }))
                    {
                        prsnt.OnOffTimePrsnt.Value.Statistics = true;
                        PowerAnalysisApp.Default.ShowDataTableForm(prsnt);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.Differ:
                    break;
                case PowerAnalysisOpt.Transient:
                    break;
                case PowerAnalysisOpt.PSRR:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(prsnt, (p) =>
                    {
                        PwrPSRRInfo pqinfo = new()
                        {
                            Name = "PwrPSRRInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "ShuRuYuan",
                            Row2LangKey = "ShuChuYuan",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("ShuRuYuan"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("ShuChuYuan"),
                        };
                        return pqinfo;
                    }))
                    {
                        (Program.Oscilloscope.View as DsoForm).TryAddPSRRUI(prsnt);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
                case PowerAnalysisOpt.SlewRate:
                    if ((Program.Oscilloscope.View as DsoForm).TryAddPowerInfo(prsnt, (p) =>
                    {
                        PwrSlewRateInfo pqinfo = new()
                        {
                            Name = "PowerSlewRateInfo",
                            NameWidth = 150,
                            Text = p.Id.ToString(),
                            Dock = DockStyle.None,
                            Presenter = p,
                            Row1LangKey = "PwrVoltageSource",
                            Row2LangKey = "PwrCurrentSource",
                            Row1stName = LanguageHelper.GetPowerAnalysisString("PwrVoltageSource"),
                            Row2ndName = LanguageHelper.GetPowerAnalysisString("PwrCurrentSource"),
                        };
                        return pqinfo;
                    }))
                    {
                        PowerAnalysisApp.Default.ShowDataTableForm(prsnt);
                        return;
                    }
                    WeakTip.Default.Write("Power", MsgTipId.NoMoreChannels);
                    break;
            };
        }

        public Boolean TryAddJitterInfo(JitterPrsnt prsnt)
        {
            JitterInfo jtinfo = new()
            {
                Name = "JitterInfo",
                Text = prsnt.Id.ToString(),
                Dock = DockStyle.None,
                Presenter = DsoPrsnt.DefaultDsoPrsnt.Jitter
            };
            prsnt.TryAddView(jtinfo);
            DsoInfoStrip.AddBadge(jtinfo);
            Presenter.Timebase.LimitScan(MsgTipId.JitterIsSupportedMinTimebase, true);
            if (DsoInfoStrip.IsHandleCreated)
                jtinfo.OnBodyClicked();
            JitterApp.Default.JitterInfo = jtinfo;
            return true;
        }
        public Boolean TryAddLissajousUI(LissajousPrsnt xyprsnt, ChannelId x, ChannelId y)
        {
            if (MultiWindowManager.GetWindow(xyprsnt.WindowId) != null)
                return false;
            IWaveformFigure fig = Constants.RENDERINGMODE switch
            {
                RenderingMode.GPU => new WaveformGPUFigure()
                {
                    Text = $"XY({x},{y})",
                    Margin = new Padding(0),
                },
                _ => new WaveformFigure()
                {
                    Text = "XY",
                    Margin = new Padding(0),
                    VisibleCursorBox = true,
                },
            };
            if (fig is WaveformGPUFigure gpuform)
            {
                gpuform.Text += $"-{xyprsnt.ID}";
                MultiWindowManager.SetFigMarkBtnVisible((WaveformGPUFigure)fig, true);
                gpuform.DispPresenter = new DisplayPrsnt(Presenter, gpuform, ModelCreateOptions.Standalone);
                gpuform.LissaPrsnt = xyprsnt;
                fig.AddWave(PlotDrawType.Display, gpuform.DispPresenter);
                fig.AddWave(PlotDrawType.XY, gpuform.LissaPrsnt);
            }
            else if (fig is WaveformFigure form)
            {
                form.DispPresenter = new DisplayPrsnt(Presenter, form, ModelCreateOptions.Standalone);
                form.LissaPrsnt = xyprsnt;
                form.AddWave(PlotDrawType.Display, form.DispPresenter);
                form.AddWave(PlotDrawType.XY, form.LissaPrsnt);
            }
            xyprsnt.TryAddView((IXYView)fig);
            MultiWindowManager.AddWindow((BaseDisplayForm)fig);
            xyprsnt.WindowId = fig.WindowId;
            if (fig is BaseDisplayForm bdf)
                bdf.Focus();
            return true;
        }
        public Boolean TryAddSearchInfo(SearchItemPrsnt sip)
        {
            SearchApp.Default.AddSearchInfo(sip);
            return true;
        }

        public Boolean TryAddLissajousUI(ChannelId x, ChannelId y)
        {
            IWaveformFigure fig = Constants.RENDERINGMODE switch
            {
                RenderingMode.GPU => new WaveformGPUFigure()
                {
                    Text = $"XY({x},{y})",
                    Margin = new Padding(0),
                },
                _ => new WaveformFigure()
                {
                    Text = "XY",
                    Margin = new Padding(0),
                    VisibleCursorBox = true,
                },
            };
            LissajousPrsnt prsnt = null;
            if (fig is WaveformGPUFigure gpuform)
            {
                if (!LissajousPrsnt.TryMake(Presenter, gpuform, x, y, out prsnt))
                {
                    return false;
                }
                gpuform.Text += $"-{prsnt.ID}";
                MultiWindowManager.SetFigMarkBtnVisible((WaveformGPUFigure)fig, true);
                gpuform.DispPresenter = new DisplayPrsnt(Presenter, gpuform, ModelCreateOptions.Standalone);
                gpuform.LissaPrsnt = prsnt;
                fig.AddWave(PlotDrawType.Display, gpuform.DispPresenter);
                fig.AddWave(PlotDrawType.XY, gpuform.LissaPrsnt);
            }
            else if (fig is WaveformFigure form)
            {
                if (!LissajousPrsnt.TryMake(Presenter, form, x, y, out prsnt))
                {
                    return false;
                }

                form.DispPresenter = new DisplayPrsnt(Presenter, form, ModelCreateOptions.Standalone);
                form.LissaPrsnt = prsnt;
                form.AddWave(PlotDrawType.Display, form.DispPresenter);
                form.AddWave(PlotDrawType.XY, form.LissaPrsnt);
            }
            prsnt.TryAddView((IXYView)fig);
            MultiWindowManager.AddWindow((BaseDisplayForm)fig);
            prsnt.WindowId = fig.WindowId;
            prsnt.Active = true;
            if (fig is BaseDisplayForm bdf)
                bdf.Focus();
            return true;
        }

        public Boolean TryRemoveLissajousUI(LissajousPrsnt prsnt)
        {
            var form = MultiWindowManager.GetWindow(prsnt.WindowId);
            if (form != null)
            {
                MultiWindowManager.RemoveWindow(form);
            }
            return true;
        }

        public Boolean TryAddSOAUI(PowerAnalysisPrsnt powerAnalysisPrsnt)
        {
            if (MultiWindowManager.GetWindow(powerAnalysisPrsnt.SOAPrsnt.Value.WindowId) == null)
            {
                IWaveformFigure fig = Constants.RENDERINGMODE switch
                {
                    RenderingMode.GPU => new WaveformGPUFigure()
                    {
                        Text = powerAnalysisPrsnt.Id.ToString() + "- " + powerAnalysisPrsnt.Mode.GetDescription_Lang(),
                        Margin = new Padding(0),
                    },
                    _ => new WaveformFigure()
                    {
                        Text = powerAnalysisPrsnt.Id.ToString() + "- " + powerAnalysisPrsnt.Mode.GetDescription_Lang(),
                        Margin = new Padding(0),
                        VisibleCursorBox = true,
                    },
                };
                if (fig is WaveformGPUFigure gpuform)
                {
                    MultiWindowManager.SetFigMarkBtnVisible((WaveformGPUFigure)fig, false);
                    gpuform.DispPresenter = new DisplayPrsnt(Presenter, gpuform, ModelCreateOptions.Standalone);
                    fig.AddWave(PlotDrawType.Display, gpuform.DispPresenter);
                    fig.AddWave(PlotDrawType.PwrSOA, powerAnalysisPrsnt);
                    powerAnalysisPrsnt.Active = true;
                }
                else if (fig is WaveformFigure form)
                {
                    form.DispPresenter = new DisplayPrsnt(Presenter, form, ModelCreateOptions.Standalone);
                    fig.AddWave(PlotDrawType.Display, form.DispPresenter);
                    fig.AddWave(PlotDrawType.PwrSOA, powerAnalysisPrsnt);
                    powerAnalysisPrsnt.Active = true;
                }
                MultiWindowManager.AddWindow((BaseDisplayForm)fig);

                powerAnalysisPrsnt.SOAPrsnt.Value.WindowId = fig.WindowId;

                if (fig is BaseDisplayForm bdf)
                    bdf.Focus();
            }
            return true;
        }
        public Boolean TryRemoveSOAUI(PwrSOAPrsnt pwrSOAPrsnt)
        {
            var form = MultiWindowManager.GetWindow(pwrSOAPrsnt.WindowId);
            if (form != null)
            {
                MultiWindowManager.RemoveWindow(form);
            }
            return true;
        }

        public Boolean TryAddPwrModulationUI(PowerAnalysisPrsnt poweranalysisprsnt, MathType mathtype, ModulationType modulationtype)
        {
            if (mathtype == MathType.Trend)
            {
                if (MultiWindowManager.GetWindow(poweranalysisprsnt.ModulationPrsnt.Value.Items[modulationtype].TrendWindowId) == null)
                {
                    String modestr = poweranalysisprsnt.Mode.GetDescription();
                    //String currenttext = $"{poweranalysisprsnt.Id}-{LanguageManger.Instance.GetIDMessage("QuShiTu")}({LanguageManger.Instance.GetIDMessage(modestr)})";
                    String currenttext = $"{LanguageManger.Instance.GetIDMessage("QuShiTu")}({LanguageManger.Instance.GetIDMessage(modulationtype.GetDescription())})";
                    IWaveformFigure fig = Constants.RENDERINGMODE switch
                    {
                        RenderingMode.GPU => new WaveformGPUFigure()
                        {
                            Text = currenttext,
                            ExtTitle = "{0}({1})",
                            TitleLanugageIDs = new List<String>() { "QuShiTu", modulationtype.GetDescription() },
                            Margin = new Padding(0),
                        },
                        _ => new WaveformFigure()
                        {
                            Text = currenttext,
                            Margin = new Padding(0),
                            ExtTitle = "{0}({1})",
                            TitleLanugageIDs = new List<String>() { "QuShiTu", modulationtype.GetDescription() },
                            VisibleCursorBox = true,
                        },
                    };
                    if (fig is WaveformGPUFigure gpuform)
                    {
                        MultiWindowManager.SetFigMarkBtnVisible((WaveformGPUFigure)fig, false);
                        gpuform.DispPresenter = new DisplayPrsnt(Presenter, gpuform, ModelCreateOptions.Standalone);
                        fig.AddWave(PlotDrawType.Display, gpuform.DispPresenter);
                        fig.AddWave(PlotDrawType.PwrModulation, poweranalysisprsnt);
                        poweranalysisprsnt.Active = true;
                    }
                    else if (fig is WaveformFigure form)
                    {
                        form.DispPresenter = new DisplayPrsnt(Presenter, form, ModelCreateOptions.Standalone);
                        fig.AddWave(PlotDrawType.Display, form.DispPresenter);
                        fig.AddWave(PlotDrawType.PwrModulation, poweranalysisprsnt);
                        poweranalysisprsnt.Active = true;
                    }
                    MultiWindowManager.AddWindow((BaseDisplayForm)fig);

                    poweranalysisprsnt.ModulationPrsnt.Value.Items[modulationtype].TrendWindowId = fig.WindowId;

                    if (fig is BaseDisplayForm bdf)
                        bdf.Focus();
                }
                return true;
            }
            else if (mathtype == MathType.Histgram)
            {
                if (MultiWindowManager.GetWindow(poweranalysisprsnt.ModulationPrsnt.Value.Items[modulationtype].HistgramWindowId) == null)
                {
                    String modestr = poweranalysisprsnt.Mode.GetDescription();
                    //String currenttext = $"{poweranalysisprsnt.Id}-{LanguageManger.Instance.GetIDMessage("ZhiFangTu")}({LanguageManger.Instance.GetIDMessage(modestr)})";
                    String currenttext = $"{LanguageManger.Instance.GetIDMessage("ZhiFangTu")}({LanguageManger.Instance.GetIDMessage(modulationtype.GetDescription())})";
                    IWaveformFigure fig = Constants.RENDERINGMODE switch
                    {
                        RenderingMode.GPU => new WaveformGPUFigure()
                        {
                            Text = currenttext,
                            ExtTitle = "{0}({1})",
                            TitleLanugageIDs = new List<String>() { "ZhiFangTu", modulationtype.GetDescription() },
                            Margin = new Padding(0),
                        },
                        _ => new WaveformFigure()
                        {
                            Text = currenttext,
                            Margin = new Padding(0),
                            ExtTitle = "{0}({1})",
                            TitleLanugageIDs = new List<String>() { "ZhiFangTu", modulationtype.GetDescription() },
                            VisibleCursorBox = true,
                        },
                    };
                    if (fig is WaveformGPUFigure gpuform)
                    {
                        MultiWindowManager.SetFigMarkBtnVisible((WaveformGPUFigure)fig, false);
                        gpuform.DispPresenter = new DisplayPrsnt(Presenter, gpuform, ModelCreateOptions.Standalone);
                        fig.AddWave(PlotDrawType.Display, gpuform.DispPresenter);
                        fig.AddWave(PlotDrawType.PwrModulation, poweranalysisprsnt);
                        poweranalysisprsnt.Active = true;
                    }
                    else if (fig is WaveformFigure form)
                    {
                        form.DispPresenter = new DisplayPrsnt(Presenter, form, ModelCreateOptions.Standalone);
                        fig.AddWave(PlotDrawType.Display, form.DispPresenter);
                        fig.AddWave(PlotDrawType.PwrModulation, poweranalysisprsnt);
                        poweranalysisprsnt.Active = true;
                    }
                    MultiWindowManager.AddWindow((BaseDisplayForm)fig);

                    poweranalysisprsnt.ModulationPrsnt.Value.Items[modulationtype].HistgramWindowId = fig.WindowId;

                    if (fig is BaseDisplayForm bdf)
                        bdf.Focus();
                }
                return true;
            }
            return false;

        }

        public Boolean TryRemovePwrModulationUI(PwrModulationPrsnt pwrmodulationprsnt)
        {
            foreach (var item in pwrmodulationprsnt.Items)
            {
                var trendform = MultiWindowManager.GetWindow(item.Value.TrendWindowId);
                if (trendform != null)
                {
                    MultiWindowManager.RemoveWindow(trendform);
                }
                var histform = MultiWindowManager.GetWindow(item.Value.HistgramWindowId);
                if (histform != null)
                {
                    MultiWindowManager.RemoveWindow(histform);
                }
            }
            var form = MultiWindowManager.GetWindow(pwrmodulationprsnt.WindowId);
            if (form != null)
            {
                MultiWindowManager.RemoveWindow(form);
            }
            return true;
        }

        public Boolean TryAddLoopAnalysisUI(PowerAnalysisPrsnt powerAnalysisPrsnt)
        {
            if (MultiWindowManager.GetWindow(powerAnalysisPrsnt.LoopAnalysisPrsnt.Value.WindowId) == null)
            {
                String modestr = powerAnalysisPrsnt.Mode.GetDescription();
                String currenttext = $"{powerAnalysisPrsnt.Id}-{LanguageManger.Instance.GetIDMessage("BoTeTu")}";
                IWaveformFigure fig = Constants.RENDERINGMODE switch
                {
                    RenderingMode.GPU => new WaveformGPUFigure()
                    {
                        Text = currenttext,
                        ExtTitle = $"{powerAnalysisPrsnt.Id}-{0}({1})",
                        TitleLanugageIDs = new List<String>() { "BoTeTu" },
                        Margin = new Padding(0),
                    },
                    _ => new WaveformFigure()
                    {
                        Text = currenttext,
                        Margin = new Padding(0),
                        ExtTitle = $"{powerAnalysisPrsnt.Id}-{0}",
                        TitleLanugageIDs = new List<String>() { "BoTeTu" },
                        VisibleCursorBox = true,
                    },
                };
                if (fig is WaveformGPUFigure gpuform)
                {
                    MultiWindowManager.SetFigMarkBtnVisible((WaveformGPUFigure)fig, false);
                    gpuform.DispPresenter = new DisplayPrsnt(Presenter, gpuform, ModelCreateOptions.Standalone);
                    fig.AddWave(PlotDrawType.Display, gpuform.DispPresenter);
                    fig.AddWave(PlotDrawType.PwrLoopAnalysis, powerAnalysisPrsnt);
                    powerAnalysisPrsnt.Active = true;
                }
                else if (fig is WaveformFigure form)
                {
                    form.DispPresenter = new DisplayPrsnt(Presenter, form, ModelCreateOptions.Standalone);
                    fig.AddWave(PlotDrawType.Display, form.DispPresenter);
                    fig.AddWave(PlotDrawType.PwrLoopAnalysis, powerAnalysisPrsnt);
                    powerAnalysisPrsnt.Active = true;
                }
                MultiWindowManager.AddWindow((BaseDisplayForm)fig);

                powerAnalysisPrsnt.LoopAnalysisPrsnt.Value.WindowId = fig.WindowId;

                if (fig is BaseDisplayForm bdf)
                    bdf.Focus();
            }
            return true;
        }
        public Boolean TryRemoveLoopAnalysisUI(PwrLoopAnalysisPrsnt pwrLoopAnalysisPrsnt)
        {
            var form = MultiWindowManager.GetWindow(pwrLoopAnalysisPrsnt.WindowId);
            if (form != null)
            {
                MultiWindowManager.RemoveWindow(form);
            }
            return true;
        }
        public Boolean TryAddPwrHarmonicUI(PowerAnalysisPrsnt powerAnalysisPrsnt)
        {
            if (MultiWindowManager.GetWindow(powerAnalysisPrsnt.HarmonicPrsnt.Value.WindowId) == null)
            {
                //IWaveformFigure fig = Constants.RENDERINGMODE switch
                //{
                //    RenderingMode.GPU => new WaveformGPUFigure()
                //    {
                //        Text = powerAnalysisPrsnt.Id.ToString() + "- " + powerAnalysisPrsnt.Mode.GetDescription(),
                //        Margin = new Padding(0),
                //    },
                //    _ => new WaveformFigure()
                //    {
                //        Text = powerAnalysisPrsnt.Id.ToString() + "- " + powerAnalysisPrsnt.Mode.GetDescription(),
                //        Margin = new Padding(0),
                //        VisibleCursorBox = true,
                //    },
                //};
                IWaveformFigure fig = new WaveformFigure()
                {
                    Text = $"{powerAnalysisPrsnt.Id}-{LanguageManger.Instance.GetIDMessage(powerAnalysisPrsnt.Mode.GetDescription())}",
                    ExtTitle = $"{powerAnalysisPrsnt.Id}-{0}({1})",
                    TitleLanugageIDs = new List<String>() { "ZhiFangTu", powerAnalysisPrsnt.Mode.GetDescription(), },
                    Margin = new Padding(0),
                    VisibleCursorBox = true,
                };

                if (fig is WaveformGPUFigure gpuform)
                {
                    gpuform.DispPresenter = new DisplayPrsnt(Presenter, gpuform, ModelCreateOptions.Standalone);
                    fig.AddWave(PlotDrawType.Display, gpuform.DispPresenter);
                    fig.AddWave(PlotDrawType.PwrHarmonic, powerAnalysisPrsnt);
                    powerAnalysisPrsnt.Active = true;
                    MultiWindowManager.SetFigMarkBtnVisible((WaveformGPUFigure)fig, false);
                }
                else if (fig is WaveformFigure form)
                {
                    form.DispPresenter = new DisplayPrsnt(Presenter, form, ModelCreateOptions.Standalone);
                    form.DispPresenter.WfmIntensity = 100;
                    form.DispPresenter.GridIntensity = 80;
                    fig.AddWave(PlotDrawType.Display, form.DispPresenter);
                    fig.AddWave(PlotDrawType.PwrHarmonic, powerAnalysisPrsnt);
                    powerAnalysisPrsnt.Active = true;
                    if (form.ButtonSource != null)
                    {
                        var btnlist = form.ButtonSource.Where((o) => { return o.BtnType == WeifenLuo.WinFormsUI.Docking.ButtonType.Cursor; });
                        if (btnlist != null && btnlist.Count() > 0)
                        {
                            btnlist.First().IsVisible = false;
                        }
                    }
                }
                MultiWindowManager.AddWindow((BaseDisplayForm)fig);
                powerAnalysisPrsnt.HarmonicPrsnt.Value.WindowId = fig.WindowId;
                if (fig is BaseDisplayForm bdf)
                    bdf.Focus();
            }
            return true;
        }
        public Boolean TryRemovePwrHarmonicUI(PwrHarmonicPrsnt PwrHarmonicPrsnt)
        {
            var form = MultiWindowManager.GetWindow(PwrHarmonicPrsnt.WindowId);
            if (form != null)
            {
                MultiWindowManager.RemoveWindow(form);
            }
            return true;
        }

        public Boolean TryAddPSRRUI(PowerAnalysisPrsnt powerAnalysisPrsnt)
        {
            if (MultiWindowManager.GetWindow(powerAnalysisPrsnt.PSRRPrsnt.Value.WindowId) == null)
            {
                String modestr = powerAnalysisPrsnt.Mode.GetDescription();
                String currenttext = $"{powerAnalysisPrsnt.Id}-{LanguageManger.Instance.GetIDMessage(modestr)}";

                IWaveformFigure fig = Constants.RENDERINGMODE switch
                {
                    RenderingMode.GPU => new WaveformGPUFigure()
                    {
                        Text = currenttext,
                        ExtTitle = $"{powerAnalysisPrsnt.Id}-{0}({1})",
                        TitleLanugageIDs = new List<String>() { modestr },
                        Margin = new Padding(0),
                    },
                    _ => new WaveformFigure()
                    {
                        Text = currenttext,
                        Margin = new Padding(0),
                        ExtTitle = $"{powerAnalysisPrsnt.Id}-{0}({1})",
                        TitleLanugageIDs = new List<String>() { modestr },
                        VisibleCursorBox = true,
                    },
                };
                if (fig is WaveformGPUFigure gpuform)
                {
                    MultiWindowManager.SetFigMarkBtnVisible((WaveformGPUFigure)fig, false);
                    gpuform.DispPresenter = new DisplayPrsnt(Presenter, gpuform, ModelCreateOptions.Standalone);
                    fig.AddWave(PlotDrawType.Display, gpuform.DispPresenter);
                    fig.AddWave(PlotDrawType.PwrPSRR, powerAnalysisPrsnt);
                    powerAnalysisPrsnt.Active = true;
                }
                else if (fig is WaveformFigure form)
                {
                    form.DispPresenter = new DisplayPrsnt(Presenter, form, ModelCreateOptions.Standalone);
                    fig.AddWave(PlotDrawType.Display, form.DispPresenter);
                    fig.AddWave(PlotDrawType.PwrPSRR, powerAnalysisPrsnt);
                    powerAnalysisPrsnt.Active = true;
                }
                MultiWindowManager.AddWindow((BaseDisplayForm)fig);

                powerAnalysisPrsnt.PSRRPrsnt.Value.WindowId = fig.WindowId;

                if (fig is BaseDisplayForm bdf)
                    bdf.Focus();
            }
            return true;
        }
        public Boolean TryRemovePSRRUI(PwrPSRRPrsnt pwrPSRRPrsnt)
        {
            var form = MultiWindowManager.GetWindow(pwrPSRRPrsnt.WindowId);
            if (form != null)
            {
                MultiWindowManager.RemoveWindow(form);
            }
            return true;
        }

        public Boolean TryAddDecodeView(DecodePrsnt prsnt)
        {
            if (prsnt != null && DsoTopStrip != null)
            {
                prsnt.TryAddView(DsoTopStrip);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean TryRemoveDecodeView(DecodePrsnt prsnt)
        {
            if (prsnt != null && DsoTopStrip != null)
            {
                prsnt.TryRemoveView(DsoTopStrip);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean TryAddProtocolView(IProtocolPrsnt prsnt)
        {
            if (prsnt != null && DsoTopStrip != null)
            {
                prsnt.TryAddView(DsoTopStrip);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean TryRemoveProtocolView(IProtocolPrsnt prsnt)
        {
            if (prsnt != null && DsoTopStrip != null)
            {
                prsnt.TryRemoveView(DsoTopStrip);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean TryAdd3dUI(RadioFrequencyPrsnt rfPrsnt)
        {
            //MakeFigure(() =>
            //{
            //    ThreeDimensionalFigure form = null;
            //    (Program.Oscilloscope.View as DsoForm).Invoke(new Action(() =>
            //    {
            //        form = new ThreeDimensionalFigure()
            //        {
            //            Text = "3D",
            //            Margin = new Padding(0),
            //        };
            //        form.RadioFrequency = rfPrsnt;
            //    }));
            //    return form;
            //});

            //return true;

            //var fig3d = new ThreeDimensionalFigure
            //{
            //    Text = "3D",
            //    Margin = new Padding(0),
            //    Presenter = rfPrsnt
            //};

            //fig3d.Presenter = rfPrsnt;
            //fig3d.Presenter.TryAddView(fig3d);

            //fig3d.TopLevel = true;
            //fig3d.ShowDialog();

            return true;
        }

        public Boolean TryAddThreeDimensionalUI(MultiDomainPrsnt MultiDomainPrsnt)
        {
            if (MultiWindowManager.GetWindow(MultiDomainPrsnt.ThreeDimensionalWindowsId) == null)
            {
                ThreeDimensionalFigure form = null;
                (Program.Oscilloscope.View as DsoForm).Invoke(new Action(() =>
                {
                    form = new ThreeDimensionalFigure()
                    {
                        Text = "3D",
                        Margin = new Padding(0),
                    };
                    form.MultiDomainPresenter = MultiDomainPrsnt;
                }));
                MultiWindowManager.AddWindow(form);
                MultiDomainPrsnt.ThreeDimensionalWindowsId = form.WindowId;
            }
            return true;
        }

        public Boolean TryRemoveThreeDimensionalUI(MultiDomainPrsnt MultiDomainPrsnt)
        {
            var form = MultiWindowManager.GetWindow(MultiDomainPrsnt.ThreeDimensionalWindowsId);
            if (form != null)
            {
                MultiWindowManager.RemoveWindow(form);
            }
            return true;
        }

        private void DsoInfoStrip_Load(Object sender, EventArgs e)
        {
            DsoInfoStrip.BtnDigital.Visible = Constants.ENABLE_LA;
            DsoInfoStrip.BtnRadio.Visible = Constants.ENABLE_RF;

            DsoInfoStrip.BtnAwg1.Visible = Constants.ENABLE_AWG1;
            DsoInfoStrip.BtnAwg2.Visible = Constants.ENABLE_AWG2;
        }

        private void DsoInfoStrip_ControlAdded(Object sender, ControlEventArgs e)
        {
        }

        private void DsoInfoStrip_ControlRemoved(Object sender, ControlEventArgs e)
        {
        }

        private void UpdateInfoStrip()
        {
            var mid = Presenter.TryGetRange(c => c.Id.IsMath() && !c.Active).MinBy(c => c.Id)?.Id ?? ChannelId.M1;
            DsoInfoStrip.BtnMath.BackColor = ChannelPrsnt.GetDrawColor(mid);
            var rid = ChannelId.R1;
            foreach (var id in ChannelIdExt.GetReferences())
            {
                Presenter.TryGetChannel(id, out IChnlPrsnt cp);
                if (cp is null || !cp.Active)
                {
                    rid = id;
                    break;
                }
            }
            DsoInfoStrip.BtnReference.BackColor = ChannelPrsnt.GetDrawColor(rid);
            var did = Presenter.TryGetRange(c => c.Id.IsDigital() && !c.Active).MinBy(c => c.Id)?.Id ?? ChannelId.D0;
            DsoInfoStrip.BtnDigital.BackColor = ChannelPrsnt.GetDrawColor(did);
            var bid = Presenter.TryGetRange(c => c.Id.IsDecode() && !c.Active).MinBy(c => c.Id)?.Id ?? ChannelId.B1;
            DsoInfoStrip.BtnBus.BackColor = ChannelPrsnt.GetDrawColor(bid);
            var rfid = Presenter.TryGetRange(c => c.Id.IsRadioFrequency() && !c.Active).MinBy(c => c.Id)?.Id ?? ChannelId.RF1;
            DsoInfoStrip.BtnRadio.BackColor = ChannelPrsnt.GetDrawColor(rfid);
        }

        #endregion

        public void DefaultInit()
        {
            foreach (var dp in Presenter.TryGetRange(c => !c.Id.IsAnalog()))
            {
                if (GetBadge(dp.Id) is not null)
                {
                    dp.Active = false;
                    RemoveWaveformUI(dp);
                }
            }
            foreach (var item in Presenter.PwrAnalysisDictionary)
            {
                if (GetBadge(item.Value.Id) is not null)
                {
                    item.Value.Active = false;
                    //RemoveWaveformUI(dp);
                }
            }
        }
        private void SetFocusChnl(ChannelId id)
        {
            if (Presenter.TryGetChannel(id, out var prsnt))
            {
                if (DsoPrsnt.FocusId == id)
                {
                    prsnt.Active ^= true;
                }
                else
                {
                    prsnt.Active = true;
                    if (prsnt.Active)
                    {
                        DsoPrsnt.FocusId = id;
                    }
                }
            }
        }

        #region DragDrop
        //!!!Sample code for DragDrop
        private void ItemTarget_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void WindowDockPanel_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ScopeX.UserControls.ScopeXListPage)))
            {
                var data = e.Data.GetData(typeof(ScopeX.UserControls.ScopeXListPage)) as ScopeX.UserControls.ScopeXListPage;
                if (data.Name.Contains("LpMeasure"))
                {
                    Int32 idx = Int32.Parse(data.Header[1..]) - 1;
                    Presenter.Measure[idx].Active = false;
                }
                else if (data.Name.Contains("LpCymometer"))
                {
                    Presenter.Cymometer.Active = false;
                }
                else if (data.Name.Contains("LpVoltmeter"))
                {
                    Presenter.Voltmeter.Active = false;
                }
            }
            else if (e.Data.GetDataPresent(nameof(IBadgeView)))
            {
                var data = e.Data.GetData(nameof(IBadgeView)) as IBadgeView;

                if (!data.Presenter.Id.IsAWG())
                {
                    if (Presenter.TryGetChannel(data.Presenter.Id, out var cp))
                    {
                        cp.Active = false;
                        RemoveWaveformUI(cp);
                    }
                }
                else
                {
                    RemoveWaveformUI(Presenter.GetWfmGenerator(data.Presenter.Id));
                }
            }
        }

        private void DsoInfoStrip_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(ScopeX.UserControls.ScopeXListPage)))
            {
                var data = e.Data.GetData(typeof(ScopeX.UserControls.ScopeXListPage)) as ScopeX.UserControls.ScopeXListPage;
                if (data.Name.Contains("LpMeasure"))
                {
                    Int32 idx = Int32.Parse(data.Header[1..]) - 1;
                    Presenter.Measure[idx].Active = false;
                }
                else if (data.Name.Contains("LpCymometer"))
                {
                    Presenter.Cymometer.Active = false;
                }
                else if (data.Name.Contains("LpVoltmeter"))
                {
                    Presenter.Voltmeter.Active = false;
                }
            }
        }
        #endregion

        #region 软关机建关机参数保存
        private const Int32 WM_ENDSESSION = 0x0016;
        private const Int32 WM_QUERYENDSESSION = 0x0011;
        private const Int32 VM = 0x0011;
        private const Int32 WM_DISPLAYCHANGE = 0x007E;  // 显示器改变
        protected override void WndProc(ref Message SystemMessage)
        {//处理系统消息问询
            switch (SystemMessage.Msg)
            {
                case VM:
                    Presenter.CloseAllLed();
                    FilePrsnt.SaveSetting(Environment.CurrentDirectory, "LastSettings", false, true);
                    SystemMessage.Result = (IntPtr)0;
                    break;
                case WM_QUERYENDSESSION or WM_ENDSESSION:
                    Presenter.CloseAllLed();
                    break;
                case WM_DISPLAYCHANGE:
                    // 屏幕分辨率或显示器改变后的处理逻辑
                    // 你可以在这里调整窗口布局或重新计算控件大小
                    SuitResolution();
                    break;
                default:
                    base.WndProc(ref SystemMessage);
                    break;
            }
        }
        #endregion

        #region 弱提示窗体关闭
        public void CloseWeakTipForm()
        {
            if (this.IsHandleCreated && this.OwnedForms != null)
            {
                var tip = this.OwnedForms.FirstOrDefault(x => x is WeakTipForm);
                if (tip != null && tip.Visible)
                {
                    tip.Visible = false;
                }
            }
        }

        #endregion

        #region 屏幕自检相关内容
        public Boolean ProcessSystemCheckMode(Int32 msglpara = -1)
        {
            var sys_check_pst = Presenter?.SystemCheck;
            Int32 msg_lparam = msglpara;
            //如果系统自检模式已开
            if (sys_check_pst != null && sys_check_pst.CheckEnable)
            {
                switch (sys_check_pst.ScopeCheckType)
                {
                    case CheckType.ScreenCheck:
                        {
                            if (msg_lparam == KeyCode.RUNSTOP)
                            {
                                sys_check_pst.ExitCount++;
                                return true;
                            }
                            sys_check_pst.ExitCount = 0;
                            if (sys_check_pst.ScreenColorDisplay < ScreenMaskColor.White)
                            {
                                sys_check_pst.ScreenColorDisplay++;
                            }
                            else
                            {
                                sys_check_pst.ScreenColorDisplay = ScreenMaskColor.Red;
                            }
                        }
                        break;
                    case CheckType.TouchCheck:
                        {
                            if (msg_lparam == KeyCode.RUNSTOP)
                            {
                                sys_check_pst.ExitCount++;
                                return true;
                            }
                            sys_check_pst.ExitCount = 0;
                            //if (sys_check_pst.TextColorDisplay < TouchTestTextColor.Black)
                            //{
                            //    sys_check_pst.TextColorDisplay++;
                            //}
                            //else
                            //{
                            //    sys_check_pst.TextColorDisplay = TouchTestTextColor.Red;
                            //}
                        }
                        break;
                    case CheckType.KeyboardCheck:
                        sys_check_pst.KeyCheckCode = msg_lparam;
                        if (msg_lparam == KeyCode.RUNSTOP)
                        {
                            sys_check_pst.ExitCount++;
                            return true;
                        }
                        sys_check_pst.ExitCount = 0;
                        break;
                    case CheckType.LEDCheck:
                        break;
                    default:
                        return false;
                }
                return true;
            }
            return false;
        }

        #endregion

        #region 启动关闭其他右键菜单
        // 导入键盘事件API
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        // 定义ESC键常量
        private const int VK_ESCAPE = 0x1B;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x1;
        private const uint KEYEVENTF_KEYUP = 0x2;
        /// <summary>
        /// 通过模拟ESC键关闭所有层级的右键菜单
        /// </summary>
        private void CloseAllContextMenusWithESC()
        {
            // 连续发送多次ESC，确保关闭多级菜单
            for (int i = 0; i < 3; i++)
            {
                PressAndReleaseESC();
                Application.DoEvents(); // 处理消息队列
                System.Threading.Thread.Sleep(50); // 短暂延迟确保系统响应
            }
            EnumWindows(new EnumWindowsProc((hWnd, lParam) =>
            {
                StringBuilder className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);

                if (className.ToString() == "#32768")
                {
                    // 发送WM_CLOSE消息尝试关闭菜单
                    SendMessage(hWnd, 0x0010, IntPtr.Zero, IntPtr.Zero); // WM_CLOSE
                }

                return true;
            }), IntPtr.Zero);
        }

        /// <summary>
        /// 模拟按下并释放ESC键
        /// </summary>
        private void PressAndReleaseESC()
        {
            // 按下ESC
            keybd_event((byte)VK_ESCAPE, 0, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
            // 释放ESC
            keybd_event((byte)VK_ESCAPE, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, IntPtr.Zero);
        }
        #endregion 启动关闭其他右键菜单
    }
}

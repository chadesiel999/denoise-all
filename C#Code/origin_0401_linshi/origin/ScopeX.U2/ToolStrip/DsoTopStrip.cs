using EventBus;
using PdfSharpCore.Drawing;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Helper;
using ScopeX.Controls.Common.Structs;
using ScopeX.Controls.Language;
using ScopeX.Core;
using ScopeX.Core.Decode;
using ScopeX.Core.Tools;
using ScopeX.U2.Search;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using Svg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class DsoTopStrip : UserControl, ITimebaseView, ITriggerView, IChnlView, ISearchView, IProtocolView, ICursorView, IArtificialIntelligenceView
    {
        public ChannelId Id { get; set; }

        //Si信息对应的最大长度集合(1920分辨率下)
        //private (int SiTimeBase, int SiAcquisition, int SiTrigger) _SiWidthMax = (0, 0, 0);

        //Si信息对应的最小长度集合(其他分辨率下)
        //private (int SiTimeBase, int SiAcquisition, int SiTrigger) _SiWidthMin = (224, 345, 284);

        //private (int Width, int IconWidth, int IconPos) _AutosetOrigin = (0, 0, 0);
        private ToolTip _Tip = new ToolTip();
        private readonly ContextMenuStrip _aiSetMenu = new();
        private readonly ToolStripMenuItem _aiSetContinuousMenuItem = new("连续AiSet：已关闭（点击开启）");
        private readonly ToolStripMenuItem _aiSetIdentifyOnlyMenuItem = new("识别信号");
        private readonly ToolStripMenuItem _aiSetAutoScaleOnlyMenuItem = new("调整幅度档");
        private readonly ToolStripMenuItem _aiSetTimebaseOnlyMenuItem = new("调整时基");
        private readonly ToolStripMenuItem _aiSetAcqResourceAdaptiveMenuItem = new("采集资源自适应");
        private readonly ToolStripMenuItem _aiSetRestoreMenuItem = new("恢复AiSet前状态");
        private readonly ToolStripMenuItem _aiSetGenerateReportMenuItem = new("生成测试报告");
        private static readonly TimeSpan _AiSetLeftClickInterval = TimeSpan.FromMilliseconds(800);
        private DateTime _LastAiSetLeftClickTime = DateTime.MinValue;
        private Size _AcqInfoNormalSize;
        private Boolean _StringColorChanged = false;
        public Boolean IsTriggerFormShow { get; set; } = false;
        public Boolean IsAppsFormShow { get; set; } = false;
        public Boolean IsSettingFormShow { get; set; } = false;
        public DsoTopStrip()
        {
            InitializeComponent();

            String imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\StartMenu", "logo.png");
            String filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ScopXConfig", "logo.png");
            //try
            //{
            Bitmap bitmap;
            if (!System.IO.File.Exists(filePath))
            {
                bitmap = new Bitmap(imagePath);
            }
            else
            {
                bitmap = new Bitmap(filePath);
            }
            BtnLog.Icon = bitmap;
            if (BtnLog.Icon.Width % BtnLog.Icon.Height == 0)
            {
                BtnLog.IconSize = new Size(50, 35);
            }
            //    int targetWidth = BtnLog.IconSize.Width;
            //    int targetHeight = BtnLog.IconSize.Height;

            //    float scale = Math.Min((float)targetWidth / bitmap.Width, (float)targetHeight / bitmap.Height);

            //    int newWidth = (int)(bitmap.Width * scale);
            //    int newHeight = (int)(bitmap.Height * scale);

            //    Bitmap scaledBitmap = new Bitmap(newWidth, newHeight);

            //    using (Graphics g = Graphics.FromImage(scaledBitmap))
            //    {
            //        g.Clear(Color.Transparent);
            //        g.DrawImage(bitmap, 0, 0, newWidth, newHeight);
            //    }
            //    BtnLog.Icon = scaledBitmap;
            //}
            //catch
            //{
            //    //Console.WriteLine($"An error occurred: {ex.Message}");
            //}
            //string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\StartMenu", "logo.png");
            //if (!System.IO.File.Exists(imagePath))
            //{
            //    BtnLog.Icon = Properties.Resources.logo;
            //}
            //else
            //{
            //    Bitmap bitmap = new Bitmap(imagePath);
            //    BtnLog.Icon = bitmap;
            //}
            Init();
        }
        private void BtnLog_Paint(object sender, PaintEventArgs e)
        {
            ScopeXIconButton btn = (ScopeXIconButton)sender;

            int targetWidth = btn.IconSize.Width;
            int targetHeight = btn.IconSize.Height;

            float wscale = (float)targetWidth / btn.Icon.Width;
            float hscale = (float)targetHeight / btn.Icon.Height;
            int newWidth = (int)(btn.Icon.Width * wscale);
            int newHeight = (int)(btn.Icon.Height * hscale);
            Bitmap scaledBitmap = new Bitmap(newWidth, newHeight);

            //btn.Icon = scaledBitmap;
            ISvgRenderer Renderer = SvgRenderer.FromGraphics(e.Graphics);
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            e.Graphics.SetSmoothMode(true);
            Renderer.DrawImage(scaledBitmap, new RectangleF(btn.IconOffset, 0, btn.IconSize.Width, btn.IconSize.Height), new RectangleF(new Point(btn.ClientRectangle.X, btn.ClientRectangle.Y), new Size(btn.Icon.Width, btn.Icon.Height)), GraphicsUnit.Pixel);

            e.Graphics.SetSmoothMode(false);

        }
        protected new Boolean DesignMode
        {
            get
            {
                Boolean rtnflag = false;
#if DEBUG
                rtnflag = DesignTimeHelper.InDesignMode(this);
#endif
                return rtnflag;
            }
        }

        public ArtificialIntelligencePrsnt artificialIntelligence
        {
            get;
            set;
        }

        IArtificialIntelligencePrsnt IView<IArtificialIntelligencePrsnt>.Presenter
        {
            get => artificialIntelligence;
            set => artificialIntelligence = (ArtificialIntelligencePrsnt)value;
        }


        public TimebasePrsnt TmbPresenter
        {
            get;
            set;
        }

        ITimebasePrsnt IView<ITimebasePrsnt>.Presenter
        {
            get => TmbPresenter;
            set => TmbPresenter = (TimebasePrsnt)value;
        }

        public TriggerPrsnt TrgPresenter
        {
            get;
            set;
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => TrgPresenter;
            set => TrgPresenter = (TriggerPrsnt)value;
        }

        public AnalogPrsnt ChnlPresenter
        {
            get;
            set;
        }

        IBadge IView<IBadge>.Presenter
        {
            get => ChnlPresenter;
            set => ChnlPresenter = (AnalogPrsnt)value;
        }
        ISearchPrsnt IView<ISearchPrsnt>.Presenter
        {
            get;
            set;
        }
        public IProtocolPrsnt Presenter { get; set; }
        ICursorPrsnt IView<ICursorPrsnt>.Presenter { get; set; }

        void IView.UpdateView(Object presenter, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { presenter, propertyName });
            }
            else
            {
                Update(presenter, propertyName);
            }
        }

        public void ManualUpdateTriggerState(SysState state = SysState.Stop)
        {
            DrawTriggerState(state);
        }

        protected void Update(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
            if (TriggerPrsnt.Type != TriggerType.Serial)
                TrgPresenter = TriggerPrsnt.GetOrMakeTrigger(TrgPresenter.Dso, TriggerPrsnt.Type);

            if (presenter is TriggerPrsnt tp)
            {
                FocuTriggerPos(tp, propertyName);
                var triggercolor = AppStyleConfig.DefaultTitleForeColor;
                switch (TriggerPrsnt.Mode)
                {
                    case TriggerMode.OneShot:
                        if (TriggerPrsnt.State == SysState.Stop)
                        {
                            triggercolor = AppStyleConfig.DefaultRunSotpSingleStopBackColor;
                        }
                        else
                        {
                            triggercolor = AppStyleConfig.DefaultRunSotpRunBackColor;
                        }
                        break;
                    case TriggerMode.Normal:
                    case TriggerMode.Auto:
                    default:
                        break;
                }

                CBTriggerMode.ForeColor = triggercolor; //TriggerPrsnt.Mode == TriggerMode.OneShot ? AppStyleConfig.DefaultRunSotpRunBackColor : AppStyleConfig.DefaultTitleForeColor;
                CBTriggerMode.MouseinForeColor = CBTriggerMode.ForeColor;
                CBTriggerMode.PressedForeColor = CBTriggerMode.ForeColor;
                if (propertyName == nameof(TriggerPrsnt.Type) || propertyName == "Source") SearchApp.Default.TriggerTypeChanged();
                //if (propertyName == nameof(TriggerPrsnt.Mode))
                //    CBTriggerMode.SelectedIndex = (Int32)TriggerPrsnt.Mode;
            }
            ////search
            //if (presenter is SearchPrsnt sp)
            //{
            //    switch (propertyName)
            //    {
            //        case nameof(sp.SearchCount):
            //            if (sp.SearchCount > 0)
            //            {
            //                BtnSearch.IsIndicatorShow = true;
            //            }
            //            else if (sp.SearchCount == 0)
            //            {
            //                BtnSearch.IsIndicatorShow = false;
            //            }


            //            break;
            //    }
            //}
            //decode
            if (presenter is DecodePrsnt dp && TriggerPrsnt.Type == TriggerType.Serial)
            {
                var source = TriggerPrsnt.GetTriggerSource();

                switch (propertyName)
                {
                    case nameof(dp.ProtocolType):
                        if (dp.Active && source != null && dp.Id == source.Value)
                        {
                            TriggerPrsnt.UpdateTrigSerialType(dp.ProtocolType);
                        }
                        break;
                    default:
                        if (source != null && (source.Value.IsDecode() || source == ChannelId.None))
                        {
                            if (source.Value == dp.Id)
                            {
                                if (!dp.Active)//如果解码通道被关闭则需要自动切换到另一个解码通道，如果没有解码通道存在则选择Close
                                {
                                    var decodes = ChannelIdExt.GetDecodes();
                                    foreach (var id in decodes)
                                    {
                                        if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id, out var p) && p.Active)
                                        {
                                            TriggerPrsnt.SetTrigSerialSource(id);
                                            break;
                                        }
                                    }
                                    source = TriggerPrsnt.GetTriggerSource();
                                    if (source.Value == dp.Id)
                                    {
                                        TriggerPrsnt.SetTrigSerialSource(null);
                                    }

                                    dp.TryRemoveView(this);
                                }
                            }
                            else
                            {
                                if (dp.Active && source.Value == ChannelId.None)
                                {
                                    TriggerPrsnt.SetTrigSerialSource(dp.Id);
                                }
                            }
                        }
                        break;
                }
            }
            //cursor
            if (presenter is CursorPrsnt cp)
            {
                switch (propertyName)
                {
                    case nameof(cp.Active):
                        BtnCursor.IsIndicatorShow = cp.Active;
                        break;
                }
            }
            if (!DesignMode)
            {
                switch (propertyName)
                {
                    case nameof(TmbPresenter.SegmentActive):
                    case nameof(TmbPresenter.FrameCount):
                    case nameof(TimebasePrsnt.IsScan):
                    case "SamplingScale" or "SamplingPosition":
                        UpdateTimebase(propertyName);
                        if (propertyName == nameof(TmbPresenter.SegmentActive))
                        {
                            if (TmbPresenter.SegmentActive)
                            {
                                SegmentApp.Default.Presenter.TryAddView(SegmentApp.Default.InfoStrip);
                                SegmentApp.Default.InfoStrip.Visible = TmbPresenter.SegmentActive;
                            }
                        }
                        if (propertyName == nameof(TimebasePrsnt.IsScan))
                        {
                            ((Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetMainWindow() as WaveformGPUFigure).UpdateView(DsoPrsnt.DefaultDsoPrsnt.Timebase, nameof(TimebasePrsnt.IsScan));//通知主窗口设置显隐余辉设置
                        }
                        return;
                    case nameof(TmbPresenter.Mode):
                    case nameof(TmbPresenter.LengthOpt):
                    case nameof(TmbPresenter.AverageCnt):
                    case nameof(TmbPresenter.StorageMode):
                    case nameof(TmbPresenter.Ext10MHzLocked):
                    case nameof(TmbPresenter.ClockSrc):
                    case nameof(TmbPresenter.StorageDepthOpt):
                    case nameof(TimebasePrsnt.AnaChnlLengthSource):
                    case nameof(TmbPresenter.AnalogSamplingRate):
                        SiAcquisition.DataSource = GetAcqDataSource();
                        SetAcquisitionTip();
                        OnFastAcqClicked();
                        return;
                    case nameof(TriggerPrsnt.State):
                        UpdateTimebase(propertyName);
                        DrawTriggerState(TriggerPrsnt.State);
                        return;
                    case nameof(TmbPresenter.IsZoom):
                        {
                            var mainwindow = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetMainWindow();
                            if (mainwindow != null && mainwindow is WaveformGPUFigure fig)
                            {
                                fig.OpenOrCloseZoom(TmbPresenter.IsZoom);
                            }
                            return;
                        }
                    case "IsInverted":
                        if (presenter is AnalogPrsnt anaprsnt)
                        {
                            //触发相关更改
                            var tri = Program.Oscilloscope.CurrentTrigger;
                            if (tri is TrigEdgePrsnt edgeprsnt)
                            {
                                if (edgeprsnt.Source == anaprsnt.Id)
                                {
                                    // 触发电平取反，边沿触发的边沿属性要反向
                                    edgeprsnt.PosIndex = -edgeprsnt.PosIndex;
                                    if (edgeprsnt.Slope == EdgeSlope.Rise)
                                        edgeprsnt.Slope = EdgeSlope.Fall;
                                    else if (edgeprsnt.Slope == EdgeSlope.Fall)
                                        edgeprsnt.Slope = EdgeSlope.Rise;

                                    ReLoadSource();
                                }
                            }
                        }
                        return;
                }
                ReLoadSource();
            }
        }

        private void UpdateTimebase(string propertyName)
        {
            SiTimebase.BeginUpdate();
            SiTimebase.DataSource = new List<Object>() { TimebaseToString() + " ", (TmbPresenter.IsScan && TriggerPrsnt.State != SysState.Stop) ? "" : " " + TriggerDelayToString() };
            if (propertyName == "SamplingScale")
            {
                SiTimebase.Prompt(new DefaultPromptProperty(new Font(SiTimebase.Font, FontStyle.Bold), AppStyleConfig.DefaultCheckedBackColor), "0");
                _StringColorChanged = true;
            }
            else if (propertyName == "SamplingPosition")
            {
                if (!_StringColorChanged)
                    SiTimebase.Prompt(new DefaultPromptProperty(new Font(SiTimebase.Font, FontStyle.Bold), AppStyleConfig.DefaultCheckedBackColor), "1");
                _StringColorChanged = false;
            }
            SiTimebase.EndUpdate();
            SetTimebaseTip();
        }


        //触发位置变化聚焦效果；
        private void FocuTriggerPos(TriggerPrsnt presenter, String propertyName)
        {
            if (presenter is TrigSingleSrcPrsnt singlePrsnt)
            {
                if (propertyName == "CompPosIndex")
                {
                    //if(FocusRender.IsInKeyBoardState(FocusRender.KeyBoardState.TriggerCompPos))
                    {
                        SiTrigger.Prompt(new DefaultPromptProperty(new Font(SiTrigger.Font, FontStyle.Bold), AppStyleConfig.DefaultCheckedBackColor), "-1");
                    }
                }
            }
            else if (presenter is TrigMultiLevelPrsnt multiprsnt)
            {
                if (propertyName == nameof(TrigMultiLevelPrsnt.PosIndex))
                {
                    //if (FocusRender.IsInKeyBoardState(FocusRender.KeyBoardState.TriggerCompPos))
                    {
                        SiTrigger.Prompt(new DefaultPromptProperty(new Font(SiTrigger.Font, FontStyle.Bold), AppStyleConfig.DefaultCheckedBackColor), "-1");
                    }

                }
            }
        }

        //void ITriggerView.Update(String propertyName)
        //{
        //    throw new NotImplementedException();
        //}

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                BtnAutoset.Height = TlpBody.Height;
                SiAcquisition.Height = TlpBody.Height;
                SiTrigger.Height = TlpBody.Height;

                SiTimebase.DataSource = new List<Object>()
                {
                    TimebaseToString(),
                    " " + TriggerDelayToString()
                };
                SiAcquisition.DataSource = GetAcqDataSource();
                SiTrigger.DataSource = TriggerInfoList();

                DrawTriggerState(TriggerPrsnt.State);
                DrawRunStopState(TriggerPrsnt.State);
                //  BRunStop.SelectedIndex = (Int32)TriggerPrsnt.Mode;




                SetTimebaseTip();
                SetAcquisitionTip();
                SetTriggerTip();
            }
        }

        private void DrawRunStopState(SysState state)
        {
            switch (state)
            {
                case SysState.Stop:
                    BtnTriggerState.Text = @"Stop";
                    break;
                default:
                    BtnTriggerState.Text = @"Run";
                    break;
            }
            BtnTriggerState.ForeColor = (state != SysState.Stop) ? AppStyleConfig.DefaultRunSotpRunBackColor : AppStyleConfig.DefaultRunSotpStopBackColor;
            BtnTriggerState.MouseinForeColor = BtnTriggerState.ForeColor;
            BtnTriggerState.PressedForeColor = BtnTriggerState.ForeColor;
            BtnTriggerState.PressedBackColor = BtnTriggerState.BackColor;
            BtnTriggerState.Invalidate();
        }

        private void DrawTriggerState(SysState state)
        {

            switch (state)
            {
                case SysState.Reset:
                case SysState.Armed:
                    BtnTriggerState.Text = @"Armed";
                    break;
                case SysState.Ready:
                    BtnTriggerState.Text = @"Ready";
                    break;
                case SysState.Triged:
                    BtnTriggerState.Text = @"Triged";
                    break;
                case SysState.Auto:
                    BtnTriggerState.Text = @"Auto";
                    break;
                case SysState.Scan:
                    BtnTriggerState.Text = @"Roll";
                    break;
                case SysState.Stop:
                    BtnTriggerState.Text = @"Stop";
                    break;
            }

            //BtnTriggerState.BackColor = (state != SysState.Stop) ? AppStyleConfig.DefaultRunSotpRunBackColor : AppStyleConfig.DefaultRunSotpStopBackColor;
            BtnTriggerState.ForeColor = (state != SysState.Stop) ? AppStyleConfig.DefaultRunSotpRunBackColor : AppStyleConfig.DefaultRunSotpStopBackColor;
            BtnTriggerState.MouseinForeColor = BtnTriggerState.ForeColor;
            BtnTriggerState.PressedForeColor = BtnTriggerState.ForeColor;
            //BtnTriggerState.PressedBackColor = BtnTriggerState.BackColor;
            BtnTriggerState.Invalidate();
        }

        private void DrawSyncState()
        {

            //BtnSearch.Text = @"Armed";


            ////BtnSearch.BackColor = (state != SysState.Stop) ? AppStyleConfig.DefaultRunSotpRunBackColor : AppStyleConfig.DefaultRunSotpStopBackColor;
            //BtnSearch.ForeColor = ;
            //BtnSearch.MouseinForeColor = BtnSearch.ForeColor;
            //BtnSearch.PressedForeColor = BtnSearch.ForeColor;
            ////BtnSearch.PressedBackColor = BtnSearch.BackColor;
            //BtnSearch.Invalidate();
        }

        private String TimebaseToString()
        {
            return new Quantity(TmbPresenter.ScaleByus, TmbPresenter.Prefix, TmbPresenter.Unit)
.ToString() + "/div";
        }

        private String TriggerDelayToString()
        {
            return new Quantity(TmbPresenter.PositionByus, TmbPresenter.Prefix, TmbPresenter.Unit)
.ToString(6, true);
        }

        private static Image GetAcquisitionModeIcon(AnaChnlAcqMode mode)
        {
            Image img = mode switch
            {
                AnaChnlAcqMode.Normal => GetTopInfoImg("AcqModeNormal.png"),
                //AnaChnlAcqMode.Sequence => GetTopInfoImg("AcqModeSequence.png"),
                AnaChnlAcqMode.Peak => GetTopInfoImg("AcqModePeak.png"),
                AnaChnlAcqMode.HighRes => GetTopInfoImg("AcqModeHighRes.png"),
                AnaChnlAcqMode.Average => GetTopInfoImg("AcqModeAverage.png"),
                AnaChnlAcqMode.Envelope => GetTopInfoImg("AcqModeEnvelope.png"),
                _ => throw new ArgumentException(null, nameof(mode)),
            };
            return img;
        }

        private static Image GetTopInfoImg(String imgName)
        {
            Type tp = typeof(DsoTopStrip);
            Stream stream = tp.Assembly.GetManifestResourceStream(tp.Namespace + ".Resources.TopInfoBar." + imgName);
            return new Bitmap(stream);
        }

        public void UpdateFFT()
        {
            var fftmath = Program.Oscilloscope.TryGetRange(c => c.Id.IsMath() && c.Id <= ChannelIdExt.MaxMChId && c.Active).Select(p => (MathPrsnt)p).Where(m => m.Args.Type == MathType.FFT).ToList();
            if (fftmath != null && fftmath.Count > 0)
            {
                BtnFft.IsIndicatorShow = true;
            }
            else
            {
                BtnFft.IsIndicatorShow = false;
            }
        }


        private List<Object> TriggerInfoList()
        {
            return TriggerPrsnt.Type switch
            {
                TriggerType.Edge => GetEdgeInfoList(TrgPresenter as TrigEdgePrsnt),
                TriggerType.PulseWidth => GetPulseWidthInfoList(TrgPresenter as TrigWidthPrsnt),
                TriggerType.Video => GetVideoInfoList(TrgPresenter as TrigVideoPrsnt),
                TriggerType.Pattern => GetPatternInfoList(TrgPresenter as TrigPatPrsnt),
                TriggerType.State => GetStateInfoList(TrgPresenter as TrigStatePrsnt),
                TriggerType.Delay => GetDelayInfoList(TrgPresenter as TrigDelayPrsnt),
                TriggerType.TimeOut => GetTimeOutInfoList(TrgPresenter as TrigTimeOutPrsnt),
                TriggerType.SustainTime => GetSustainTimeInfoList(TrgPresenter as TrigSustainTimePrsnt),
                TriggerType.SetupHold => GetSetupHoldInfoList(TrgPresenter as TrigSetupHoldPrsnt),
                TriggerType.NEdge => GetNEdgeInfoList(TrgPresenter as TrigNEdgePrsnt),
                TriggerType.Runt => GetRuntInfoList(TrgPresenter as TrigRuntPrsnt),
                TriggerType.Transition => GetTransitionInfoList(TrgPresenter as TrigTransPrsnt),
                TriggerType.Glitch => GetGlitchInfoList(TrgPresenter as TrigGlitchPrsnt),
                TriggerType.Window => GetWindowInfoList(TrgPresenter as TrigWindowPrsnt),
                TriggerType.MultiQulified => GetMultiQulifiedInfoList(),
                TriggerType.Serial => GetSerialInfoList(),
                TriggerType.Interval => GetIntervalInfoList(TrgPresenter as TrigIntervalPrsnt),
                _ => GetDefaultInfoList(),
            };
        }

        #region 获取不同触发的信息表

        private List<Object> GetEdgeInfoList(TrigEdgePrsnt edge)
        {
            Image slope = edge.Slope switch
            {
                EdgeSlope.Rise => GetTopInfoImg("PosSlope.png"),
                EdgeSlope.Fall => GetTopInfoImg("NegSlope.png"),
                _ => GetTopInfoImg("BothSlope.png"),
            };

            if (slope == null)
            {
                slope = edge.Slope switch
                {
                    EdgeSlope.Rise => GetTopInfoImg("PosSlope.png"),
                    EdgeSlope.Fall => GetTopInfoImg("NegSlope.png"),
                    _ => GetTopInfoImg("BothSlope.png"),
                };
            }

            SiTrigger.CancelAllStringColorByIndex();
            SetSourceIdStringColor(edge.Source!.Value, 1);
            string str = edge.Source.ToString();
            if (edge.Source == ChannelId.Ext5)
                str = "Ext/5";
            var couplingstr = "";
            switch (edge.Coupling)
            {
                case TriggerCoupling.AC:
                    couplingstr = "AC";
                    break;
                case TriggerCoupling.LFR:
                    couplingstr = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("LFReject");
                    break;
                case TriggerCoupling.HFR:
                    couplingstr = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("HFReject");
                    break;
                case TriggerCoupling.NR:
                    couplingstr = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("NoiseReject");
                    break;
                default:
                case TriggerCoupling.DC:
                    break;
            }
            if (edge.Source > ChannelId.C4)
                couplingstr = "";

            return new()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Edge") + " " + couplingstr,
                 edge.Source !.Value.IsDigital()?"D" + (edge.Source -ChannelId.D0) + " ":str + " ",
                slope,
                " " + new Quantity(edge.VuCompPosition, edge.PosPrefix, edge.PosUnit).ToString("##0.###;-##0.###;0", true),
            };
        }

        private void SetSourceIdStringColor(ChannelId channelId, Int32 index)
        {

            ChannelId id = channelId.IsDigital() ? ChannelId.D0 : channelId;
            if (Program.Oscilloscope != null && Program.Oscilloscope.TryGetChannel(id, out IChnlPrsnt channel))
            {
                if (id.IsDigital())
                {
                    var dch = (DigitalPrsnt)channel;
                    SiTrigger.SetStringColorByIndex(index, dch.GetColorAt(channelId - id));
                }
                else
                {
                    SiTrigger.SetStringColorByIndex(index, channel.DrawColor);
                }
            }
            else
            {
                SiTrigger.CancelAllStringColorByIndex();
            }
        }

        private List<Object> GetPulseWidthInfoList(TrigWidthPrsnt pulse)
        {
            var cond = pulse.Condition switch
            {
                PulseCondition.GreaterThan => ">",
                PulseCondition.LessThan => "<",
                PulseCondition.Equal => "[...]",
                PulseCondition.NotEqual => "]...[",
                _ => "",
            };
            String condwidth = string.Empty;
            if (pulse.Condition == PulseCondition.GreaterThan)
            {
                condwidth = new Quantity(pulse.WidthByps, Prefix.Pico, "s").ToString("##0.###", true);
            }
            else if (pulse.Condition == PulseCondition.LessThan)
            {
                condwidth = new Quantity(pulse.UpperWidthByps, Prefix.Pico, "s").ToString("##0.###", true);
            }
            if (pulse.Condition == PulseCondition.Equal || pulse.Condition == PulseCondition.NotEqual)
            {
                condwidth = new Quantity(pulse.UpperWidthByps, Prefix.Pico, "s").ToString("##0.###", true) + "\r\n"
                    + new Quantity(pulse.WidthByps, Prefix.Pico, "s").ToString("##0.###", true);
            }
            SiTrigger.CancelAllStringColorByIndex();
            SetSourceIdStringColor(pulse.Source!.Value, 1);
            return new List<Object>()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_PulseWidth"),
                pulse.Source !.Value.ToString() + " ",
                pulse.Polarity == PulsePolarity.Positive ? GetTopInfoImg("PosPulse.png") : GetTopInfoImg("NegPulse.png"),
                " " + cond,
                condwidth,
            };
        }
        private List<Object> GetIntervalInfoList(TrigIntervalPrsnt pulse)
        {
            var cond = pulse.Condition switch
            {
                PulseCondition.GreaterThan => ">",
                PulseCondition.LessThan => "<",
                PulseCondition.Equal => "[...]",
                PulseCondition.NotEqual => "]...[",
                _ => "",
            };
            String condwidth = new Quantity(pulse.WidthByps, Prefix.Pico, "s").ToString("##0.###", true);
            if (pulse.Condition == PulseCondition.Equal || pulse.Condition == PulseCondition.NotEqual)
            {
                condwidth = new Quantity(pulse.UpperWidthByps, Prefix.Pico, "s").ToString("##0.###", true) + "\r\n" + condwidth;
            }
            SiTrigger.CancelAllStringColorByIndex();
            SetSourceIdStringColor(pulse.Source!.Value, 1);

            return new List<Object>()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Interval"),
                pulse.Source !.Value.ToString() + " ",
                pulse.Polarity == PulsePolarity.Positive ? GetTopInfoImg("PosPulse.png") : GetTopInfoImg("NegPulse.png"),
                " " + cond,
                condwidth,
            };
        }
        private List<Object> GetVideoInfoList(TrigVideoPrsnt video)
        {
            var cond = video.Sync switch
            {
                VideoSync.All => LanguageManger.Instance.GetIDMessage("Enum_VideoSync_All"),//"All Lines",
                VideoSync.Specified => $"{LanguageManger.Instance.GetIDMessage("VideoTrigger_Line")} {video.Line}", // , Field {video.Field}
                _ => "",
            };
            SiTrigger.CancelAllStringColorByIndex();
            SetSourceIdStringColor(video.Source!.Value, 1);

            return new List<Object>()
            {
               ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Video"),
                video.Source !.Value.ToString(),
                video.Standard.ToString(),
                cond,
            };
        }

        private List<Object> GetPatternInfoList(TrigPatPrsnt pattern)
        {
            SiTrigger.CancelAllStringColorByIndex();
            return new()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Pattern"),
                "",
                //pattern.Operator.ToString().ToUpper(),
                //pattern.TimeCondition.ToString(),
                //new Quantity(pattern.DurationByps, Prefix.Pico, "s").ToString("##0.###", true),
            };
        }

        private List<Object> GetStateInfoList(TrigStatePrsnt state)
        {
            SiTrigger.CancelAllStringColorByIndex();
            SetSourceIdStringColor(state.ClkSource, 4);
            return new()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_State"),
                state.Operator.ToString().ToUpper(),
                "Clk:" + " ",
                state.ClkPolarity == PulsePolarity.Positive ? GetTopInfoImg("PosPulse.png") : GetTopInfoImg("NegPulse.png"),
                " " + state.ClkSource.ToString(),
            };
        }

        private List<Object> GetDelayInfoList(TrigDelayPrsnt dl)
        {
            SiTrigger.CancelAllStringColorByIndex();

            SetSourceIdStringColor(dl.SourceOne, 1);
            SetSourceIdStringColor(dl.SourceTwo, 3);

            return new()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Delay"),
                dl.SourceOne.ToString() + ":",
                dl.SourceOneSlope == EdgeSlope.Rise ? GetTopInfoImg("PosPulse.png") : GetTopInfoImg("NegPulse.png"),
                " " + dl.SourceTwo.ToString() + ":",
                dl.SourceTwoSlope == EdgeSlope.Rise ? GetTopInfoImg("PosPulse.png") : GetTopInfoImg("NegPulse.png"),
            };
        }

        private List<Object> GetTimeOutInfoList(TrigTimeOutPrsnt timeout)
        {
            /*var cond = timeout.Polarity switch
            {
                LevelPolarity.Positive => "H",
                LevelPolarity.Negative => "L",
                LevelPolarity.Any => "X",
                _ => "",
            };*/
            Image slope = timeout.Polarity switch
            {
                LevelPolarity.Positive => GetTopInfoImg("PosSlope.png"),
                LevelPolarity.Negative => GetTopInfoImg("NegSlope.png"),
                _ => GetTopInfoImg("BothSlope.png"),
            };

            SiTrigger.CancelAllStringColorByIndex();
            SetSourceIdStringColor(timeout.Source!.Value, 2);

            return new List<Object>()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_TimeOut"),
                slope,
                timeout.Source !.Value.ToString(),
                //cond,
                new Quantity(timeout.DurationByps, Prefix.Pico, "s").ToString("##0.###", true),
            };
        }

        private List<Object> GetSustainTimeInfoList(TrigSustainTimePrsnt sustaintime)
        {
            var cond = sustaintime.Condition switch
            {
                PulseCondition.GreaterThan => ">",
                PulseCondition.LessThan => "<",
                PulseCondition.Equal => "[...]",
                PulseCondition.NotEqual => "]...[",
                _ => "",
            };
            String condwidth = string.Empty;
            if (sustaintime.Condition == PulseCondition.GreaterThan)
            {
                condwidth = new Quantity(sustaintime.WidthByps, Prefix.Pico, "s").ToString("##0.###", true);
            }
            else if (sustaintime.Condition == PulseCondition.LessThan)
            {
                condwidth = new Quantity(sustaintime.UpperWidthByps, Prefix.Pico, "s").ToString("##0.###", true);
            }
            if (sustaintime.Condition == PulseCondition.Equal || sustaintime.Condition == PulseCondition.NotEqual)
            {
                condwidth = new Quantity(sustaintime.UpperWidthByps, Prefix.Pico, "s").ToString("##0.###", true) + "\r\n"
                    + new Quantity(sustaintime.WidthByps, Prefix.Pico, "s").ToString("##0.###", true);
            }
            SiTrigger.CancelAllStringColorByIndex();
            SetSourceIdStringColor(sustaintime.Source, 1);

            return new List<Object>()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_SustainTime"),
                " " + cond,
                condwidth,
            };
        }

        private List<Object> GetSetupHoldInfoList(TrigSetupHoldPrsnt sh)
        {
            SiTrigger.CancelAllStringColorByIndex();

            SetSourceIdStringColor(sh.ClkSource, 3);
            SetSourceIdStringColor(sh.DataSource, 5);

            return new()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_SetupHold"),
                "Clk: " + " ",
                sh.ClkPolarity == EdgeSlope.Rise ? GetTopInfoImg("PosSlope.png") : GetTopInfoImg("NegSlope.png"),
                " " + sh.ClkSource.ToString(),
                $"Data: ",
                sh.DataSource.ToString(),
            };
        }

        private List<Object> GetNEdgeInfoList(TrigNEdgePrsnt edge)
        {
            Image slope = edge.Polarity switch
            {
                EdgeSlope.Rise => GetTopInfoImg("PosSlope.png"),
                EdgeSlope.Fall => GetTopInfoImg("NegSlope.png"),
                _ => GetTopInfoImg("BothSlope.png"),
            };

            SiTrigger.CancelAllStringColorByIndex();
            SetSourceIdStringColor(edge.Source!.Value, 1);
            return new()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_NEdge"),
                 edge.Source !.Value.IsDigital()?"D" + (edge.Source -ChannelId.D0) + " ":edge.Source !.Value.ToString() + " ",
                slope,
                " " + new Quantity(edge.CompPosition, edge.PosPrefix, edge.PosUnit).ToString("##0.###", true),
            };
        }

        private List<Object> GetRuntInfoList(TrigRuntPrsnt runt)
        {
            var cond = runt.WidthCompCondition switch
            {
                PulseCondition.GreaterThan => ">",
                PulseCondition.LessThan => "<",
                PulseCondition.Equal => "[...]",
                PulseCondition.NotEqual => "]...[",
                _ => "",
            };
            String condwidth = string.Empty;
            if (runt.WidthCompCondition == PulseCondition.GreaterThan)
            {
                condwidth = new Quantity(runt.WidthByps, Prefix.Pico, "s").ToString("##0.###", true);
            }
            else if (runt.WidthCompCondition == PulseCondition.LessThan)
            {
                condwidth = new Quantity(runt.UpperWidthByps, Prefix.Pico, "s").ToString("##0.###", true);
            }
            if (runt.WidthCompCondition == PulseCondition.Equal || runt.WidthCompCondition == PulseCondition.NotEqual)
            {
                condwidth = new Quantity(runt.UpperWidthByps, Prefix.Pico, "s").ToString("##0.###", true) + "\r\n"
                    + new Quantity(runt.WidthByps, Prefix.Pico, "s").ToString("##0.###", true);
            }
            SiTrigger.CancelAllStringColorByIndex();
            SetSourceIdStringColor(runt.Source, 1);

            return new List<Object>()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Runt"),
                runt.Source.ToString() + " ",
                runt.Polarity == PulsePolarity.Positive ? GetTopInfoImg("PosRunt.png") : GetTopInfoImg("NegRunt.png") ,
                " " + cond,
                condwidth,
            };
        }

        private List<Object> GetTransitionInfoList(TrigTransPrsnt trans)
        {
            var cond = trans.WidthCompCondition switch
            {
                PulseCondition.GreaterThan => ">",
                PulseCondition.LessThan => "<",
                PulseCondition.Equal => "[...]",
                PulseCondition.NotEqual => "]...[",
                _ => "",
            };

            string CompPosToString(Double position)
            {
                if (trans.PosUnit == "V")
                {
                    return new Quantity(position, trans.PosPrefix, trans.PosUnit).ToString("##0.####", true, 7);
                }
                else
                {
                    return new Quantity(position, trans.PosPrefix, trans.PosUnit).ToString("##0.###", true, 7);
                }
            }

            // 从字符串中提取单位字符串
            string GetUnit(string str)
            {
                Match match = Regex.Match(str, @"[^\d.]+"); // 使用正则表达式匹配非数字和非小数点字符
                if (match.Success)
                    return match.Value; // 获取匹配的单位字符

                return "";
            }

            // 从字符串中提取数字部分
            double GetDouble(string str)
            {
                string numberPattern = @"(\d+(\.\d+)?)"; // 数字的正则表达式模式
                Match match = Regex.Match(str, numberPattern); // 使用正则匹配数字部分

                if (!match.Success)
                    return double.NaN;

                string numberString = match.Groups[1].Value; // 提取匹配到的数字部分
                if (!double.TryParse(numberString, out double number))
                    return double.NaN;

                return number;
            }

            String WidthToString(Int64 width)
            {
                return new Quantity(width, Prefix.Pico, "s").ToString("##0.###########", true, 14);
            }

            String condwidth = string.Empty;
            string slewRate = " "; // 压摆率

            double dt_compposition = trans.UpperCompPosition - trans.LowerCompPosition; // 高低电平差值

            string dt_pos_str = CompPosToString(dt_compposition);
            double srpos = GetDouble(dt_pos_str); //电平差值
            string verticalUnit = GetUnit(dt_pos_str); // 电平差值单位

            string lowerslewstr = WidthToString(trans.WidthByps);
            string upperslewstr = WidthToString(trans.UpperWidthByps);

            double lowerslewval = GetDouble(lowerslewstr);
            string lowerslewunit = GetUnit(lowerslewstr);

            double upperslewval = GetDouble(upperslewstr);
            string upperslewunit = GetUnit(upperslewstr);


            var lowerslewrate = srpos / (lowerslewval == 0 ? 1 : lowerslewval);  // 压摆率的下限值
            var upperslewrate = srpos / (upperslewval == 0 ? 1 : upperslewval);  // 压摆率的上限值


            if (trans.WidthCompCondition == PulseCondition.GreaterThan)
            {
                condwidth = new Quantity(trans.WidthByps, Prefix.Pico, "s").ToString("##0.###", true);
                slewRate = Math.Round(lowerslewrate, 2) + verticalUnit + "/" + lowerslewunit;
            }
            else if (trans.WidthCompCondition == PulseCondition.LessThan)
            {
                condwidth = new Quantity(trans.UpperWidthByps, Prefix.Pico, "s").ToString("##0.###", true);
                slewRate = Math.Round(upperslewrate, 2) + verticalUnit + "/" + upperslewunit;
            }

            if (trans.WidthCompCondition == PulseCondition.Equal || trans.WidthCompCondition == PulseCondition.NotEqual)
            {
                condwidth = new Quantity(trans.UpperWidthByps, Prefix.Pico, "s").ToString("##0.###", true) + "\r\n"
                    + new Quantity(trans.WidthByps, Prefix.Pico, "s").ToString("##0.###", true);
                slewRate = Math.Round(lowerslewrate, 2) + verticalUnit + "/" + lowerslewunit;

                slewRate += "\r\n" + Math.Round(upperslewrate, 2) + verticalUnit + "/" + upperslewunit;
            }
            SiTrigger.CancelAllStringColorByIndex();
            SetSourceIdStringColor(trans.Source, 1);

            return new List<Object>()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Transition"),
                trans.Source.ToString() + " ",
                trans.Slope == EdgeSlope.Rise ? GetTopInfoImg("PosSlope.png"):GetTopInfoImg("NegSlope.png"),
                " " + cond,
                condwidth,
                slewRate
                //$"U: {new Quantity(trans.UpperCompPosition, trans.PosPrefix, trans.PosUnit).ToString("##0.###", true)}" + "\r\n" +
                //$"L: {new Quantity(trans.LowerCompPosition, trans.PosPrefix, trans.PosUnit).ToString("##0.###", true)}",
            };
        }

        private List<Object> GetGlitchInfoList(TrigGlitchPrsnt glitch)
        {
            SiTrigger.CancelAllStringColorByIndex();
            SetSourceIdStringColor(glitch.Source!.Value, 1);

            return new()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Glitch"),
                glitch.Source.ToString() + " ",
                glitch.Polarity == PulsePolarity.Positive ? GetTopInfoImg("PosPulse.png") : GetTopInfoImg("NegPulse.png"),
                " " + $"< {new Quantity(glitch.WidthByps, Prefix.Pico, "s").ToString("##0.###", true)}",
            };
        }

        private List<Object> GetWindowInfoList(TrigWindowPrsnt win)
        {
            var cond = win.TimeCondition switch
            {
                WindowTimeCondition.OnEnter => nameof(WindowTimeCondition.OnEnter),
                WindowTimeCondition.GreaterThan => $"> {new Quantity(win.WidthByps, Prefix.Pico, "s").ToString("##0.###", true)}",
                WindowTimeCondition.LessThan => $"< {new Quantity(win.WidthByps, Prefix.Pico, "s").ToString("##0.###", true)}",
                _ => "",
            };
            SiTrigger.CancelAllStringColorByIndex();
            SetSourceIdStringColor(win.Source, 1);

            return new List<Object>()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Window"),
                win.Source.ToString(),
                cond,
                Environment.NewLine,
                $"U: {new Quantity(win.UpperCompPosition, win.PosPrefix, win.PosUnit).ToString(5, true)}" + "\r\n" +
                $"L: {new Quantity(win.LowerCompPosition, win.PosPrefix, win.PosUnit).ToString("##0.###", true)}",
            };
        }

        private List<Object> GetMultiQulifiedInfoList()
        {
            SiTrigger.CancelAllStringColorByIndex();
            return new()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_MultiQulified"),
                ""
            };
        }

        private List<Object> GetSerialInfoList()
        {
            SiTrigger.CancelAllStringColorByIndex();

            return new()
            {
                ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Enum_TriggerType_Serial"),
                Core.Decode.ProtocolPrsnt.CurrentType.GetDisplay().ToString(),
            };
        }

        private List<Object> GetDefaultInfoList()
        {
            SiTrigger.CancelAllStringColorByIndex();

            return new() { " " };
        }

        #endregion 获取不同触发的信息表

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        private void Init()
        {
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged1;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged1;
            //_SiWidthMax = (SiTimebase.Width, SiAcquisition.Width, SiTrigger.Width);
            //_AutosetOrigin = (BtnAutoset.Width, BtnAutoset.IconSize.Width, BtnAutoset.IconOffset);
            _AcqInfoNormalSize = SiAcquisition.Size;

            //BtnCursor.IsIndicatorShow = DsoPrsnt.DefaultDsoPrsnt.Cursor.Active;
            //颜色风格变更

            //BtnAutoset.ForeColor = AppStyleConfig.DefaultTitleForeColor;
            //BtnAutoset.MouseinForeColor = AppStyleConfig.DefaultTitleForeColor;
            //BtnAutoset.PressedForeColor = AppStyleConfig.DefaultTitleForeColor;
            //BtnAutoset.MouseInBorderThickness = 2;
            //BtnAutoset.MouseinBorderColor = AppStyleConfig.DefaultContextDarkBackColor;
            //BtnAutoset.MouseinBackColor = AppStyleConfig.DefaultTitleBackColor;
            //BtnAutoset.BorderColor = Color.FromArgb(33, 33, 40);//AppStyleConfig.DefaultContextDarkBackColor;
            //BtnAutoset.BackColor = Color.FromArgb(72, 77, 85);
            //BtnAutoset.MouseinBackColor = Color.FromArgb(72, 77, 85);

            //BtnTriggerState.ForeColor = AppStyleConfig.DefaultTitleForeColor;
            //BtnTriggerState.MouseinForeColor = AppStyleConfig.DefaultTitleForeColor;
            //BtnTriggerState.PressedForeColor = AppStyleConfig.DefaultTitleForeColor;
            //BtnTriggerState.MouseInBorderThickness = 2;
            //BtnTriggerState.MouseinBorderColor = AppStyleConfig.DefaultContextDarkBackColor;
            //BtnTriggerState.MouseinBackColor = AppStyleConfig.DefaultTitleBackColor;
            //BtnTriggerState.BackColor = Color.FromArgb(72, 77, 85);
            //BtnTriggerState.BorderColor = Color.FromArgb(33, 33, 40);
            //BtnTriggerState.MouseinBackColor = Color.FromArgb(72, 77, 85);

            //CBTriggerMode.MouseinBackColor = Color.FromArgb(72, 77, 85);
            //CBTriggerMode.BackColor = Color.FromArgb(72, 77, 85);//AppStyleConfig.DefaultContextBackColor;
            //CBTriggerMode.BorderColor = Color.FromArgb(33, 33, 40);//AppStyleConfig.DefaultContextDarkBackColor;
            //CBTriggerMode.ForeColor = AppStyleConfig.DefaultTitleForeColor;
            //CBTriggerMode.MouseinForeColor = AppStyleConfig.DefaultTitleForeColor;
            //CBTriggerMode.PressedForeColor = AppStyleConfig.DefaultTitleForeColor;
            //CBTriggerMode.MouseinBackColor = AppStyleConfig.DefaultTitleBackColor;
            //CBTriggerMode.MouseInBorderThickness = 2;
            //CBTriggerMode.MouseinBorderColor = AppStyleConfig.DefaultContextDarkBackColor;
            InitAiSetMenu();
        }

        private void InitAiSetMenu()
        {
            _aiSetMenu.Items.Clear();
            _aiSetMenu.Items.Add(_aiSetContinuousMenuItem);
            _aiSetMenu.Items.Add(new ToolStripSeparator());
            _aiSetMenu.Items.Add(_aiSetIdentifyOnlyMenuItem);
            _aiSetMenu.Items.Add(_aiSetAutoScaleOnlyMenuItem);
            _aiSetMenu.Items.Add(_aiSetTimebaseOnlyMenuItem);
            _aiSetMenu.Items.Add(_aiSetAcqResourceAdaptiveMenuItem);
            _aiSetMenu.Items.Add(new ToolStripSeparator());
            _aiSetMenu.Items.Add(_aiSetRestoreMenuItem);
            _aiSetMenu.Items.Add(new ToolStripSeparator());
            _aiSetMenu.Items.Add(_aiSetGenerateReportMenuItem);

            _aiSetMenu.ShowImageMargin = false;
            _aiSetMenu.ShowCheckMargin = false;
            _aiSetMenu.RenderMode = ToolStripRenderMode.Professional;
            _aiSetMenu.Renderer = new ToolStripProfessionalRenderer(new AiSetMenuColorTable());
            _aiSetMenu.BackColor = AppStyleConfig.DefaultContextDarkBackColor;
            _aiSetMenu.ForeColor = AppStyleConfig.DefaultTitleForeColor;
            _aiSetMenu.Font = BtnSearch.Font;
            _aiSetMenu.Padding = new Padding(2);

            foreach (ToolStripItem item in _aiSetMenu.Items)
            {
                if (item is ToolStripMenuItem menuItem)
                {
                    menuItem.BackColor = AppStyleConfig.DefaultContextDarkBackColor;
                    menuItem.ForeColor = AppStyleConfig.DefaultTitleForeColor;
                    menuItem.AutoSize = false;
                    menuItem.Height = 28;
                }
            }

            _aiSetIdentifyOnlyMenuItem.Click -= AiSetIdentifyOnlyMenuItem_Click;
            _aiSetAutoScaleOnlyMenuItem.Click -= AiSetAutoScaleOnlyMenuItem_Click;
            _aiSetTimebaseOnlyMenuItem.Click -= AiSetTimebaseOnlyMenuItem_Click;
            _aiSetAcqResourceAdaptiveMenuItem.Click -= AiSetAcqResourceAdaptiveMenuItem_Click;
            _aiSetRestoreMenuItem.Click -= AiSetRestoreMenuItem_Click;
            _aiSetContinuousMenuItem.Click -= AiSetContinuousMenuItem_Click;
            _aiSetGenerateReportMenuItem.Click -= AiSetGenerateReportMenuItem_Click;
            _aiSetMenu.Opening -= AiSetMenu_Opening;

            _aiSetContinuousMenuItem.Click += AiSetContinuousMenuItem_Click;
            _aiSetIdentifyOnlyMenuItem.Click += AiSetIdentifyOnlyMenuItem_Click;
            _aiSetAutoScaleOnlyMenuItem.Click += AiSetAutoScaleOnlyMenuItem_Click;
            _aiSetTimebaseOnlyMenuItem.Click += AiSetTimebaseOnlyMenuItem_Click;
            _aiSetAcqResourceAdaptiveMenuItem.Click += AiSetAcqResourceAdaptiveMenuItem_Click;
            _aiSetRestoreMenuItem.Click += AiSetRestoreMenuItem_Click;
            _aiSetGenerateReportMenuItem.Click += AiSetGenerateReportMenuItem_Click;
            _aiSetMenu.Opening += AiSetMenu_Opening;
            UpdateContinuousAiSetMenuState();
        }

        private void AiSetMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateContinuousAiSetMenuState();
            _aiSetRestoreMenuItem.Enabled = artificialIntelligence != null && artificialIntelligence.CanRestoreAiSet();
            _aiSetGenerateReportMenuItem.Enabled = artificialIntelligence != null;
        }

        private void AiSetContinuousMenuItem_Click(object sender, EventArgs e)
        {
            if (artificialIntelligence == null)
                return;
            artificialIntelligence.ContinuousAiSetEnabled = !artificialIntelligence.ContinuousAiSetEnabled;
            UpdateContinuousAiSetMenuState();
        }

        private void UpdateContinuousAiSetMenuState()
        {
            Boolean enabled = artificialIntelligence != null && artificialIntelligence.ContinuousAiSetEnabled;
            _aiSetContinuousMenuItem.Text = enabled
                ? "连续AiSet：已开启"
                : "连续AiSet：已关闭";
            _aiSetContinuousMenuItem.AutoSize = false;
            _aiSetContinuousMenuItem.Height = 28;
            _aiSetContinuousMenuItem.BackColor = enabled ? AppStyleConfig.DefaultCheckedBackColor : AppStyleConfig.DefaultTitleBackColor;
            _aiSetContinuousMenuItem.ForeColor = enabled ? AppStyleConfig.DefaultCheckedForeColor : AppStyleConfig.DefaultTitleForeColor;
            _aiSetContinuousMenuItem.Font = new Font(BtnSearch.Font, FontStyle.Regular);
        }

        private void AiSetIdentifyOnlyMenuItem_Click(object sender, EventArgs e)
        {
            artificialIntelligence?.ActionAiSetIdentifyOnly();
        }

        private void AiSetAutoScaleOnlyMenuItem_Click(object sender, EventArgs e)
        {
            artificialIntelligence?.ActionAiSetAutoScaleOnly();
        }

        private void AiSetTimebaseOnlyMenuItem_Click(object sender, EventArgs e)
        {
            artificialIntelligence?.ActionAiSetTimebaseOnly();
        }

        private void AiSetAcqResourceAdaptiveMenuItem_Click(object sender, EventArgs e)
        {
            artificialIntelligence?.ActionAiSetAcqResourceAdaptive();
        }

        private void AiSetRestoreMenuItem_Click(object sender, EventArgs e)
        {
            artificialIntelligence?.ActionAiSetRestoreLast();
        }

        private async void AiSetGenerateReportMenuItem_Click(object sender, EventArgs e)
        {
            if (artificialIntelligence == null)
                return;

            _aiSetGenerateReportMenuItem.Enabled = false;
            using Form waitingDialog = CreateAiSetReportWaitingDialog();
            Rectangle wa = Screen.FromControl(this).WorkingArea;
            waitingDialog.Location = new Point(
                wa.Left + (wa.Width - waitingDialog.Width) / 2,
                wa.Top + (wa.Height - waitingDialog.Height) / 2);
            waitingDialog.Show(FindForm());
            waitingDialog.Refresh();

            String reportJson = String.Empty;
            try
            {
                reportJson = await Task.Run(() => artificialIntelligence.GetAiSetScpiStatusJson());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"生成测试报告失败：{ex.Message}", "AiSet测试报告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                waitingDialog.Close();
                _aiSetGenerateReportMenuItem.Enabled = true;
            }

            String reportText = BuildAiSetReportDisplayText(reportJson);
            ShowAiSetReportDialog(reportText, reportJson);
        }

        private static Form CreateAiSetReportWaitingDialog()
        {
            Form dialog = new()
            {
                Text = "正在生成测试报告",
                StartPosition = FormStartPosition.Manual,
                Size = new Size(420, 140),
                MinimumSize = new Size(420, 140),
                MaximumSize = new Size(420, 140),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ControlBox = false,
                ShowInTaskbar = false,
                TopMost = true
            };

            Label label = new()
            {
                Dock = DockStyle.Top,
                Height = 48,
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "正在生成测试报告，请稍候..."
            };

            System.Windows.Forms.ProgressBar progress = new()
            {
                Dock = DockStyle.Top,
                Height = 18,
                Style = System.Windows.Forms.ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 25
            };

            Panel panel = new()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 14, 16, 12)
            };
            panel.Controls.Add(progress);
            panel.Controls.Add(label);

            dialog.Controls.Add(panel);
            return dialog;
        }

        private static String BuildAiSetReportDisplayText(String reportJson)
        {
            if (String.IsNullOrWhiteSpace(reportJson))
                return "报告内容为空。";

            try
            {
                using JsonDocument doc = JsonDocument.Parse(reportJson);
                JsonElement root = doc.RootElement;

                Boolean status = root.TryGetProperty("status", out var statusNode) && statusNode.ValueKind == JsonValueKind.True;
                String generatedAt = root.TryGetProperty("generatedAt", out var generatedAtNode) ? (generatedAtNode.GetString() ?? String.Empty) : String.Empty;
                UInt32 requestId = root.TryGetProperty("requestId", out var requestIdNode) && requestIdNode.TryGetUInt32(out var rid) ? rid : 0;
                UInt32 executedId = root.TryGetProperty("executedRequestId", out var executedIdNode) && executedIdNode.TryGetUInt32(out var eid) ? eid : 0;
                String report = root.TryGetProperty("report", out var reportNode) ? (reportNode.GetString() ?? String.Empty) : String.Empty;

                StringBuilder sb = new();
                sb.AppendLine("========================================");
                sb.AppendLine("              AiSet 测试报告");
                sb.AppendLine("========================================");
                sb.AppendLine($"生成时间：{(String.IsNullOrWhiteSpace(generatedAt) ? "未返回" : generatedAt)}");
                sb.AppendLine($"请求ID：{requestId}");
                sb.AppendLine($"已执行ID：{executedId}");
                sb.AppendLine($"状态：{(status ? "成功" : "未完成")}");
                sb.AppendLine();

                if (!status)
                {
                    sb.AppendLine("AiSet尚未完成，请先执行完整AiSet后再生成报告。");
                    sb.AppendLine();
                    sb.AppendLine("原始返回：");
                    sb.AppendLine(reportJson);
                    return sb.ToString();
                }

                sb.AppendLine(FormatAiSetReportMarkdown(report));
                return sb.ToString();
            }
            catch
            {
                return $"报告返回非JSON格式：\r\n{reportJson}";
            }
        }

        private static String FormatAiSetReportMarkdown(String report)
        {
            if (String.IsNullOrWhiteSpace(report))
                return "报告为空。";

            String[] lines = report.Replace("\r\n", "\n").Split('\n');
            StringBuilder sb = new();
            foreach (String rawLine in lines)
            {
                String line = rawLine.TrimEnd();
                if (String.IsNullOrWhiteSpace(line))
                {
                    sb.AppendLine();
                    continue;
                }

                if (line == "---" || line == "***")
                    continue;

                line = Regex.Replace(line, @"\*\*(.*?)\*\*", "$1");
                line = line.Replace("`", String.Empty);

                if (line.StartsWith("### "))
                {
                    sb.AppendLine();
                    sb.AppendLine($"【{line[4..].Trim()}】");
                    continue;
                }
                if (line.StartsWith("## "))
                {
                    sb.AppendLine();
                    sb.AppendLine($"【{line[3..].Trim()}】");
                    continue;
                }
                if (line.StartsWith("# "))
                {
                    sb.AppendLine();
                    sb.AppendLine($"【{line[2..].Trim()}】");
                    continue;
                }

                line = Regex.Replace(line, @"^\s*[-*]\s+", "• ");
                sb.AppendLine(line);
            }

            String prettyText = Regex.Replace(sb.ToString(), @"(\r?\n){3,}", Environment.NewLine + Environment.NewLine);
            return prettyText.Trim();
        }

        private void ShowAiSetReportDialog(String reportText, String reportJson)
        {
            using Form dialog = new();
            dialog.Text = "AiSet测试报告";
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.Size = new Size(920, 660);
            dialog.MinimumSize = new Size(760, 520);
            dialog.FormBorderStyle = FormBorderStyle.Sizable;

            RichTextBox reportBox = new()
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                WordWrap = true,
                Text = reportText,
                Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point),
                BackColor = AppStyleConfig.DefaultContextDarkBackColor,
                ForeColor = AppStyleConfig.DefaultTitleForeColor,
                BorderStyle = BorderStyle.FixedSingle
            };

            Panel bottomPanel = new()
            {
                Dock = DockStyle.Bottom,
                Height = 52
            };

            Button saveButton = new()
            {
                Text = "保存到本地",
                Width = 120,
                Height = 30,
                Left = 12,
                Top = 10
            };
            saveButton.Click += (_, _) =>
            {
                using SaveFileDialog sfd = new()
                {
                    Title = "保存AiSet测试报告",
                    Filter = "文本文件 (*.txt)|*.txt|JSON文件 (*.json)|*.json|所有文件 (*.*)|*.*",
                    FileName = $"AiSetReport_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                };
                if (sfd.ShowDialog(dialog) != DialogResult.OK)
                    return;

                try
                {
                    String content = sfd.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? reportJson : reportText;
                    System.IO.File.WriteAllText(sfd.FileName, content, new System.Text.UTF8Encoding(false));
                    MessageBox.Show("测试报告已保存。", "AiSet测试报告", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"保存失败：{ex.Message}", "AiSet测试报告", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            Button closeButton = new()
            {
                Text = "关闭",
                Width = 90,
                Height = 30,
                Left = saveButton.Right + 12,
                Top = 10
            };
            closeButton.Click += (_, _) => dialog.Close();

            bottomPanel.Controls.Add(saveButton);
            bottomPanel.Controls.Add(closeButton);
            dialog.Controls.Add(reportBox);
            dialog.Controls.Add(bottomPanel);
            dialog.ShowDialog(FindForm());
        }

        private void Instance_LanguageChanged1(object sender, ILanguage e)
        {
            SiAcquisition.DataSource = GetAcqDataSource();
            SiTrigger.DataSource = TriggerInfoList();
        }

        private void Stylize()
        {
            TlpBody.BackColor = Color.FromArgb(33, 33, 40);//AppStyleConfig.DefaultContextBackColor;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);

            //IconButton颜色变更
            static void ChangeColor(ScopeXIconButton uiBtn)
            {
                uiBtn.BackColor = Color.FromArgb(72, 77, 85);//AppStyleConfig.DefaultContextBackColor;
                uiBtn.MouseinBackColor = AppStyleConfig.DefaultTitleBackColor;
                uiBtn.PressedBackColor = uiBtn.MouseinBackColor;
                uiBtn.SVGForeColor = AppStyleConfig.DefaultTitleForeColor;
                uiBtn.MouseinSvgForeColor = uiBtn.SVGForeColor;
                uiBtn.PressedSvgForeColor = uiBtn.SVGForeColor;
                uiBtn.Size = new Size(uiBtn.Width, 38);
            }
            ChangeColor(BtnApps);
            ChangeColor(BtnCursor);
            ChangeColor(BtnSetting);
            ChangeColor(BtnFile);
            ChangeColor(BtnSearch);
            ChangeColor(BtnPrinter);

            ChangeColor(BtnFft);
            ChangeColor(BtnScreenShot);
            ChangeColor(BtnFastReq);
            ChangeColor(BtnClear);

            //显示内容背景色变更

            SiTimebase.StickyContentBackColor = Color.FromArgb(50, 55, 65);//AppStyleConfig.DefaultContextDarkBackColor;
            SiAcquisition.StickyContentBackColor = Color.FromArgb(50, 55, 65);
            SiTrigger.StickyContentBackColor = Color.FromArgb(50, 55, 65);
            //BtnTriggerState.BackColor = Color.FromArgb(72, 77, 85);//AppStyleConfig.DefaultContextBackColor;

            //BtnTriggerState.BorderColor = Color.FromArgb(33, 33, 40); //AppStyleConfig.DefaultContextDarkBackColor;
            SiTimebase.StickyBackColor = Color.FromArgb(72, 77, 85);//AppStyleConfig.DefaultContextBackColor;
            SiAcquisition.StickyBackColor = Color.FromArgb(72, 77, 85);
            SiTrigger.StickyBackColor = Color.FromArgb(72, 77, 85);

            SiTimebase.Font = new Font("MiSans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            SiAcquisition.Font = new Font("MiSans", 12F, FontStyle.Regular, GraphicsUnit.Point);
            SiTrigger.Font = new Font("MiSans", 12F, FontStyle.Regular, GraphicsUnit.Point);
        }

        #region event handle

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            UpdateView();
            SetToolTip(_Tip);
            this.SizeChanged += OnSizeChanged;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        private void Instance_LanguageChanged(object sender, ILanguage e)
        {
            SetToolTip(_Tip);
        }

        /// <summary>
        /// 适应分辨率1920，1680，1280，1024
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnSizeChanged(Object sender, EventArgs args)
        {
            //设置Si信息为最小长度
            //void SetWidth()
            //{
            //    SiTimebase.Width = _SiWidthMin.SiTimeBase;
            //    SiAcquisition.Width = _SiWidthMin.SiAcquisition;
            //    SiTrigger.Width = _SiWidthMin.SiTrigger;
            //}

            //设置相关按钮(BtnTriggerState，BtnSingle，BtnAutoset)的显示
            //void SetBtnState(Boolean zoomInFlag)
            //{
            //    Single zoominratio = 0.7F;

            //    BtnSingle.Visible = !zoomInFlag;
            //    BtnAutoset.Width = zoomInFlag ? (Int32)(_AutosetOrigin.Width * zoominratio) : _AutosetOrigin.Width;
            //    BtnAutoset.IconSize = zoomInFlag ? new Size((Int32)(_AutosetOrigin.IconWidth * zoominratio), BtnAutoset.IconSize.Height)
            //        : new Size(_AutosetOrigin.IconWidth, BtnAutoset.IconSize.Height);
            //    BtnAutoset.IconOffset = zoomInFlag ? (Int32)(_AutosetOrigin.IconPos * zoominratio) : _AutosetOrigin.IconPos;
            //    BtnTriggerState.Width = BtnAutoset.Width;//BtnTriggerState的Width与BtnAutoset一致
            //}

            //根据不同分辨率设置工具栏Item的状态
            //switch (Size.Width)
            //{
            //    case 1920:
            //        SiTimebase.Width = _SiWidthMax.SiTimeBase;
            //        SiAcquisition.Width = _SiWidthMax.SiAcquisition;
            //        SiTrigger.Width = _SiWidthMax.SiTrigger;
            //        SetBtnState(false);
            //        break;
            //    case 1680:
            //    case 1280:
            //        SetWidth();
            //        SetBtnState(false);
            //        break;
            //    case 1024:
            //        SetWidth();
            //        SetBtnState(true);
            //        break;
            //    default:
            //        //StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.ResolutionMismatch, MessageType.Warning);
            //        break;
            //}

            //根据当前空余的长度，来决定功能图标的显示与否
            //Int32 showwidthmax = BtnApps.Bounds.Left - SpliterBtn.Right;
            //Int32 showwidthcurrent = 0;

            ////*从倒数第二个元素开始,显示优先级是从高到低
            //for (int i = TlpBody.ColumnCount - 2; ; i--)
            //{
            //    Control currentitem = TlpBody.GetControlFromPosition(i, 0);
            //    if (currentitem != null && TlpBody.ColumnStyles[i].SizeType == SizeType.AutoSize)
            //    {
            //        //1024,1280分辨率要特殊处理一下，不显示任何功能图标
            //        if (this.Size.Width == 1024 || this.Size.Width == 1280)
            //        {
            //            currentitem.Visible = false;
            //        }
            //        else
            //        {
            //            if (showwidthcurrent + currentitem.Width < showwidthmax)
            //            {
            //                currentitem.Visible = true;
            //                showwidthcurrent += currentitem.Width;
            //            }
            //            else
            //            {
            //                currentitem.Visible = false;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}

            ////刷新工具栏
            //UpdateView();
            var minwidth = BtnTriggerState.Width + CBTriggerMode.Width + BtnAutoset.Width + SiTimebase.Width + _AcqInfoNormalSize.Width + SiTrigger.Width + BtnApps.Width;
            if (Width <= minwidth)
            {
                SiAcquisition.Size = SiAcquisition.MinimumSize;
                SiAcquisition.Visible = false;
                CBTriggerMode.Visible = false;
                BtnAutoset.Visible = false;
            }
            else
            {
                SiAcquisition.Size = _AcqInfoNormalSize;
                SiAcquisition.Visible = true;
                CBTriggerMode.Visible = true;
                BtnAutoset.Visible = true;
            }
            UpdateView();
        }

        private void SiTimebase_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.TIMEBASE);
        }

        public void OnTimebaseClicked(Func<FloatForm> creator)
        {
            Form fform = (ParentForm as DsoForm)?.MakeOperateForm("Timebase", SiTimebase, PopOrientation.Under, creator);
            SiTimebase.IsStickyBackColorChanged = true;
            fform.FormClosed += (s, e) => { SiTimebase.IsStickyBackColorChanged = false; };
        }

        private void SiTrigger_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.TRIGGER);
        }

        public void OnTriggerClicked(Func<FloatForm> creator)
        {
            Form fform = (ParentForm as DsoForm)?.MakeOperateForm("Trigger", SiTrigger, PopOrientation.Under, creator);
            SiTrigger.IsStickyBackColorChanged = true;
            fform.FormClosed += (s, e) =>
            {
                SiTrigger.IsStickyBackColorChanged = false;
                IsTriggerFormShow = false;
            };
        }

        private void SiAcquisition_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.ACQUIRE);
        }

        public void OnAcquisitionClicked(Func<FloatForm> creator)
        {
            Form fform = (ParentForm as DsoForm)?.MakeOperateForm("Acquisition", SiAcquisition, PopOrientation.Under, creator);
            SiAcquisition.IsStickyBackColorChanged = true;
            fform.FormClosed += (s, e) => { SiAcquisition.IsStickyBackColorChanged = false; };
        }

        //private void CBTriggerMode_Click(Object sender, EventArgs e)
        //{
        //    if(CBTriggerMode.SelectedIndex == 0)
        //    {
        //        _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.SINGLE);
        //    }
        //}

        //private void CBTriggerMode_SelectIndexChanged(Object sender, EventArgs e)
        //{
        //    if (CBTriggerMode.SelectedIndex == 0)
        //        _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.SINGLE);
        //    else if(CBTriggerMode.SelectedIndex == 1)
        //        _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.NORMAL);
        //    else
        //        _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.AUTO);
        //}
        private void CBTriggerMode_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.SINGLE);
        }
        private void BtnTriggerState_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.RUNSTOP);
        }

        private Timer _EnableButtonTimer = new Timer() { Interval = 3000, };

        private void EnableButtonTimer_Tick(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(TickEvent));
            }
            else
            {
                TickEvent();
            }

            void TickEvent()
            {
                // 启用按钮并停止定时器
                BtnAutoset.Enabled = true;
                _EnableButtonTimer.Stop();
            }
        }

        private void BtnAutoset_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }
            BtnAutoset.Enabled = false;
            _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.AUTOSET);
            _EnableButtonTimer.Tick -= EnableButtonTimer_Tick;
            _EnableButtonTimer.Tick += EnableButtonTimer_Tick;
            _EnableButtonTimer.Start();
        }

        private void BtnApps_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.VK_APPS);
        }

        public void OnAppsClicked(Func<FloatForm> creator)
        {
            Form form = creator();
            EventBroker.Instance.GetEvent<FormEventArgs>().Publish(form, new() { Current = form, Type = FormType.SettingForm });
            form.FormClosing += (s, e) => { IsAppsFormShow = false; };
        }

        private void BtnSetting_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.SETTING);
        }

        public void OnSettingClicked(Func<FloatForm> creator)
        {
            Form fform = (ParentForm as DsoForm)?.MakeOperateForm("Setting", BtnSetting, PopOrientation.Under, creator);
            BtnSetting.IsIndicatorShow = true;
            fform.FormClosing += (s, e) =>
            {
                BtnSetting.IsIndicatorShow = false;
                IsSettingFormShow = false;
            };
        }

        private void BtnClear_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;
            _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.VK_CLEAR);
        }

        private void BtnZoom_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                return;
            _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.VK_ZOOM);
        }

        private void BtnScreenShot_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.VK_SCREENSHOT);
        }

        private void BtnFile_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.STORAGE);
        }

        public void OnFileClicked(Func<FloatForm> creator)
        {
            Form fform = (ParentForm as DsoForm)?.MakeOperateForm("Storage", BtnFile, PopOrientation.Under, creator);
            BtnFile.IsIndicatorShow = true;
            fform.FormClosed += (s, e) => { BtnFile.IsIndicatorShow = false; };
        }

        private void BtnSearch_MouseClick(object sender, MouseEventArgs e)
        {
            if (MessageBox.Show("请确保各个通道输入端口没有信号输入！", "提示", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;
            //if (e.Button == MouseButtons.Right)
            //{
            //    return;
            //}

            //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_WAVESEARCH);
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.AUTOCALIBRATION);
        }

        public void OnSearchClicked(Func<FloatForm> creator)
        {
            //Form fform = (ParentForm as DsoForm)?.MakeOperateForm("Search", BtnSearch, PopOrientation.Under, creator);
            //BtnSearch.IsIndicatorShow = true;
            //fform.FormClosed += (s, e) => { BtnSearch.IsIndicatorShow = false; };
        }

        private void BtnPrinter_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_SETPRINTER);
        }

        public void OnPrinterClicked(Func<FloatForm> creator)
        {
            Form fform = (ParentForm as DsoForm)?.MakeOperateForm("Print", BtnPrinter, PopOrientation.Under, creator);
            BtnPrinter.IsIndicatorShow = true;
            fform.FormClosed += (s, e) => { BtnPrinter.IsIndicatorShow = false; };
        }

        private void BtnFastReq_MouseClick(Object sender, MouseEventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            ////if (e.Button == MouseButtons.Right)
            //{
            //    return;
            //}

            //_ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.FASTACQ);
            // WeakTip.Default.Write("FastAcq", MsgTipId.FunctionUnused, false, "", 5);
        }
        public void OnFastAcqClicked()
        {
            ////BtnFastReq.IsIndicatorShow = !BtnFastReq.IsIndicatorShow;
            //if (TmbPresenter.StorageMode == AnaChnlStorageMode.Fast)
            //    BtnFastReq.IsIndicatorShow = true;
            //else
            //    BtnFastReq.IsIndicatorShow = false;
            //BtnCursor.IsIndicatorShow = DsoPrsnt.DefaultDsoPrsnt.Cursor.Active;
        }
        private void BtnFft_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }
            var fftmath = Program.Oscilloscope.TryGetRange(c => c.Id.IsMath() && c.Id <= ChannelIdExt.MaxMChId && c.Active).Select(p => (MathPrsnt)p).Where(m => m.Args.Type == MathType.FFT).ToList();
            if (fftmath != null && fftmath.Count >= 2)
            {
                WeakTip.Default.Write("Math", MsgTipId.NoMoreChannels);
                return;
            }
            _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, KeyCode.VK_FFT);
        }

        private void BtnCursor_MouseClick(Object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }

            _ = NativeMethods.PostMessage(ParentForm.Handle, 0x0400, 12, -KeyCode.CURSOR);

        }

        private void BtnAiSet_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _aiSetMenu.Show(BtnSearch, new Point(BtnSearch.Width - 1, BtnSearch.Height - 1));
                return;
            }

            if (DateTime.Now - _LastAiSetLeftClickTime < _AiSetLeftClickInterval)
            {
                return;
            }
            _LastAiSetLeftClickTime = DateTime.Now;

            artificialIntelligence?.ActionAiSet();
        }

        public void OnAiSetClicked(Func<FloatForm> creator)
        {
            (ParentForm as DsoForm)?.MakeOperateForm("AiSet", BtnSearch, PopOrientation.Under, creator);
        }

        public void OnCursorClicked(Func<FloatForm> creator)
        {
            Form fform = (ParentForm as DsoForm)?.MakeOperateForm("Cursor", BtnCursor, PopOrientation.Under, creator);
            BtnCursor.IsIndicatorShow = true;
            if (!DsoPrsnt.DefaultDsoPrsnt.Cursor.Active)
            {
                BtnCursor.IsIndicatorShow = false;
            }
        }

        #endregion event handle

        public void SetToolTip(ToolTip toolTip)
        {
            //toolTip.SetToolTip(BtnTriggerState, Properties.ToolTips.AboutTriggerState);
            //toolTip.SetToolTip(CBTriggerMode, Properties.ToolTips.SetSingleTriggerMode);
            //toolTip.SetToolTip(BtnAutoset, Properties.ToolTips.SetAutoset);

            //应用按钮
            //toolTip.SetToolTip(BtnCursor, Properties.ToolTips.AboutCursor);
            toolTip.SetToolTip(BtnCursor, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GuangBiaoCeLiang"));
            //toolTip.SetToolTip(BtnFft, Properties.ToolTips.AboutFft);
            toolTip.SetToolTip(BtnFft, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FastFourierTransform"));
            //toolTip.SetToolTip(BtnFastReq, Properties.ToolTips.AboutFastReq);
            toolTip.SetToolTip(BtnFastReq, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("KuaiCai"));
            //toolTip.SetToolTip(BtnSearch, Properties.ToolTips.AboutSearch);
            //toolTip.SetToolTip(BtnSearch, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BoXingSouSuo"));
            toolTip.SetToolTip(BtnSearch, "AiSet（左键一键执行，右键更多动作）");
            //toolTip.SetToolTip(BtnPrinter, Properties.ToolTips.AboutPrinter);
            toolTip.SetToolTip(BtnPrinter, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AboutPrinter"));
            //toolTip.SetToolTip(BtnFile, Properties.ToolTips.AboutSave);
            toolTip.SetToolTip(BtnFile, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("CunChu"));
            //toolTip.SetToolTip(BtnScreenShot, Properties.ToolTips.AboutScreenShot);
            toolTip.SetToolTip(BtnScreenShot, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("JiePing"));
            //toolTip.SetToolTip(BtnClear, Properties.ToolTips.AboutClear);
            toolTip.SetToolTip(BtnClear, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AboutClear"));
            //toolTip.SetToolTip(BtnSetting, Properties.ToolTips.AboutSetting);
            toolTip.SetToolTip(BtnSetting, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AboutSetting"));
            //toolTip.SetToolTip(BtnApps, Properties.ToolTips.AboutAllApps);
            toolTip.SetToolTip(BtnApps, ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AboutAllApps"));

            //SiTimebase.SetToolTip(toolTip, Properties.ToolTips.AboutTimebase);
            //SiAcquisition.SetToolTip(toolTip, Properties.ToolTips.AboutAcquire);
            //SiTrigger.SetToolTip(toolTip, Properties.ToolTips.AboutTrigger);
            //_ToolTip = toolTip;

        }

        private ToolTip _ToolTip;

        private void SetTimebaseTip()
        {
            //SiTimebase.SetToolTip(_ToolTip, $"{Properties.ToolTips.AboutTimebase}\n{SiTimebase.DataSource[0]}{SiTimebase.DataSource[1]}");
            SiTimebase.SetToolTip(_ToolTip, $"{ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AboutTimebase")}\n{SiTimebase.DataSource[0]}{SiTimebase.DataSource[1]}");
        }

        private void SetAcquisitionTip()
        {
            //SiAcquisition.SetToolTip(_ToolTip, $"{Properties.ToolTips.AboutAcquire}\n{SiAcquisition.DataSource[1]}");
            SiAcquisition.SetToolTip(_ToolTip, $"{ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AboutAcquire")}\n{SiAcquisition.DataSource[1]}");
        }

        private void SetTriggerTip()
        {
            //SiTrigger.SetToolTip(_ToolTip, $"{Properties.ToolTips.AboutTrigger}\n{SiTrigger.DataSource[0]} {SiTrigger.DataSource[1]}");
            SiTrigger.SetToolTip(_ToolTip, $"{ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("AboutTrigger")}\n{SiTrigger.DataSource[0]} {SiTrigger.DataSource[1]}");
        }

        private Point GetPreferredLocation(Control c)
        {
            if (c.Right > SiTrigger.Right)
            {
                return new(c.Left + c.Width / 2, c.Height);
            }
            else
            {
                return new(Width, Height);
            }
        }

        private List<object> GetAcqDataSource()
        {
            String acqmode = TmbPresenter.Mode switch
            {
                AnaChnlAcqMode.Normal => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("ZhengChang"),
                AnaChnlAcqMode.Average => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PingJun"),
                AnaChnlAcqMode.Envelope => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("BaoLuo"),
                AnaChnlAcqMode.Peak => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("FengZhiJianCe"),
                AnaChnlAcqMode.HighRes => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("GaoFenBianLv"),
                _ => "",
            };
            if (TmbPresenter.Mode == AnaChnlAcqMode.Average)
            {
                acqmode += " " + TmbPresenter.AverageCnt.ToString() + ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Ci");
            }

            String longpoints = TmbPresenter.AnaChnlLengthSource[TmbPresenter.StorageDepthOpt].Key;
            String acqstorage = TmbPresenter.StorageMode switch
            {
                AnaChnlStorageMode.Normal => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PuTongCunChu"),
                AnaChnlStorageMode.Fast => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("KuaiCai"),
                //AnaChnlStorageMode.Long => "长存储",
                AnaChnlStorageMode.Long => ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("PuTongCunChu"),
                _ => longpoints, //AnaChnlStorageMode.Long
            };
            if (longpoints == TmbPresenter.AnaChnlLengthSource[0].Key)
            {
                longpoints = SIHelper.ValueChangeToSI(TmbPresenter.AnaChnlLengthSource[TmbPresenter.StorageDepthOpt].Value, 2, "Pts");
            }
            String ext10mhzlockedstring = "";
            if (TmbPresenter.ClockSrc == AnaChnlClkSrc.Outter && TmbPresenter.Ext10MHzLocked)
            {
                // ext10mhzlockedstring = "10MHz";
            }

            String AnalogSamplingRate = SIHelper.ValueChangeToSI(TmbPresenter.AnalogSamplingRate, 2, "Sa/s");

            String highresbit = "";
            if (TmbPresenter.Mode == AnaChnlAcqMode.HighRes)
            {
                Double rate = TmbPresenter.InterleaveMode switch
                {
                    AdcInterleaveMode.Mode4To1 => 10000000000,
                    AdcInterleaveMode.Mode2To1 => 5000000000,
                    AdcInterleaveMode.Mode1To1 => 2500000000,
                    _ => 2.5,
                };
                var scale = Constants.ADC_BITS + ((Int32)Math.Floor(Math.Log2((rate / TmbPresenter.AnalogSamplingRate)) / 2));
                scale = Math.Min(Constants.ADC_BITS + 4, Math.Max(Constants.ADC_BITS, scale));
                highresbit = scale.ToString() + ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("Wei");
            }

            return new List<Object>()
            {
                GetAcquisitionModeIcon(TmbPresenter.Mode),
                acqmode + ",",
                (String.IsNullOrEmpty(highresbit)?"":highresbit +","),
                longpoints +",",
                AnalogSamplingRate +(String.IsNullOrEmpty(ext10mhzlockedstring)?"":","),
                ext10mhzlockedstring ,
            };
        }
        private void BtnTriggerState_Paint(object sender, PaintEventArgs e)
        {



            if (BtnTriggerState.ForeColor == AppStyleConfig.DefaultRunSotpStopBackColor)
            {

                ScopeXIconButton btn = (ScopeXIconButton)sender;
                Color color1 = AppStyleConfig.DefaultGradualBackColorOne;
                Color color2 = AppStyleConfig.DefaultGradualBackColorTwo;
                LinearGradientBrush brush = new LinearGradientBrush(btn.ClientRectangle, color1, color2, LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(brush, btn.ClientRectangle);
                TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, btn.ClientRectangle, AppStyleConfig.DefaultRunSotpStopBackColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);

            }
            else
            {
                ScopeXIconButton btn = (ScopeXIconButton)sender;
                Color color1 = AppStyleConfig.DefaultGradualBackColorOne;
                Color color2 = AppStyleConfig.DefaultGradualBackColorTwo;
                LinearGradientBrush brush = new LinearGradientBrush(btn.ClientRectangle, color1, color2, LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(brush, btn.ClientRectangle);
                TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, btn.ClientRectangle, AppStyleConfig.DefaultRunSotpRunBackColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                //BtnTriggerState.ForeColor = AppStyleConfig.DefaultRunSotpRunBackColor;
            }
            if (BtnTriggerState.Tag == "1")
            {
                ControlPaint.DrawBorder(
                   e.Graphics,
                   BtnTriggerState.ClientRectangle,
                   Color.FromArgb(0, 191, 255),      // 左侧线颜色
                   2,               // 左侧线宽度
                   ButtonBorderStyle.Solid,
                   Color.FromArgb(0, 191, 255),      // 右侧线颜色
                   2,               // 右侧线宽度
                   ButtonBorderStyle.Solid,
                   Color.FromArgb(0, 191, 255),      // 上侧线颜色
                   2,               // 上侧线宽度
                   ButtonBorderStyle.Solid,
                   Color.FromArgb(0, 191, 255),      // 下侧线颜色
                   2,               // 下侧线宽度
                   ButtonBorderStyle.Solid);


            }




        }
        private void Trigger_MouseDown(object sender, MouseEventArgs e)
        {
            ScopeXIconButton btn = (ScopeXIconButton)sender;
            btn.Padding = new Padding(btn.Padding.Left + 1,
                                           btn.Padding.Top + 1,
                                           btn.Padding.Right + 1,
                                           btn.Padding.Bottom + 1);


            //btn.BorderThickness = 2;
            //btn.BorderColor = Color.FromArgb(0, 191, 255);
            //btn.BackColor = Color.Gray;
            btn.Tag = "1";
            Invalidate();


        }
        private void Trigger_MouseUp(object sender, MouseEventArgs e)
        {
            ScopeXIconButton btn = (ScopeXIconButton)sender;
            btn.Padding = new Padding(btn.Padding.Left - 1,
                                          btn.Padding.Top - 1,
                                          btn.Padding.Right - 1,
                                          btn.Padding.Bottom - 1);
            btn.Tag = "0";
            Invalidate();
            //btn.BackColor = Color.FromArgb(72, 77, 85);
        }

        private void CBTriggerMode_Paint(object sender, PaintEventArgs e)
        {


            ScopeXIconButton btn = (ScopeXIconButton)sender;
            Color color1 = AppStyleConfig.DefaultGradualBackColorOne;
            Color color2 = AppStyleConfig.DefaultGradualBackColorTwo;
            LinearGradientBrush brush = new LinearGradientBrush(btn.ClientRectangle, color1, color2, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, btn.ClientRectangle);
            if (CBTriggerMode.ForeColor == AppStyleConfig.DefaultTitleForeColor)
            {
                TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, btn.ClientRectangle, AppStyleConfig.DefaultTitleForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            }
            else
            {
                TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, btn.ClientRectangle, AppStyleConfig.DefaultRunSotpRunBackColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            }
            if (CBTriggerMode.Tag == "1")
            {
                ControlPaint.DrawBorder(
                   e.Graphics,
                   CBTriggerMode.ClientRectangle,
                   Color.FromArgb(0, 191, 255),      // 左侧线颜色
                   2,               // 左侧线宽度
                   ButtonBorderStyle.Solid,
                   Color.FromArgb(0, 191, 255),      // 右侧线颜色
                   2,               // 右侧线宽度
                   ButtonBorderStyle.Solid,
                   Color.FromArgb(0, 191, 255),      // 上侧线颜色
                   2,               // 上侧线宽度
                   ButtonBorderStyle.Solid,
                   Color.FromArgb(0, 191, 255),      // 下侧线颜色
                   2,               // 下侧线宽度
                   ButtonBorderStyle.Solid);
            }

        }

        private void BtnAutoset_Paint(object sender, PaintEventArgs e)
        {
            ScopeXIconButton btn = (ScopeXIconButton)sender;

            Color color1 = AppStyleConfig.DefaultGradualBackColorOne;
            Color color2 = AppStyleConfig.DefaultGradualBackColorTwo;
            LinearGradientBrush brush = new LinearGradientBrush(btn.ClientRectangle, color1, color2, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, btn.ClientRectangle);
            TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, btn.ClientRectangle, AppStyleConfig.DefaultTitleForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            if (btn.Tag == "1")
            {
                // btn.BackColor = Color.DeepSkyBlue;
                ControlPaint.DrawBorder(
                  e.Graphics,
                  CBTriggerMode.ClientRectangle,
                  Color.FromArgb(0, 191, 255),      // 左侧线颜色
                  2,               // 左侧线宽度
                  ButtonBorderStyle.Solid,
                  Color.FromArgb(0, 191, 255),      // 右侧线颜色
                  2,               // 右侧线宽度
                  ButtonBorderStyle.Solid,
                  Color.FromArgb(0, 191, 255),      // 上侧线颜色
                  2,               // 上侧线宽度
                  ButtonBorderStyle.Solid,
                  Color.FromArgb(0, 191, 255),      // 下侧线颜色
                  2,               // 下侧线宽度
                  ButtonBorderStyle.Solid);

            }

        }
        private void BtnSearch_Paint(object sender, PaintEventArgs e)
        {
            ScopeXIconButton btn = (ScopeXIconButton)sender;

            Color color1 = AppStyleConfig.DefaultGradualBackColorOne;
            Color color2 = AppStyleConfig.DefaultGradualBackColorTwo;
            LinearGradientBrush brush = new LinearGradientBrush(btn.ClientRectangle, color1, color2, LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, btn.ClientRectangle);
            TextRenderer.DrawText(e.Graphics, btn.Text, btn.Font, btn.ClientRectangle, AppStyleConfig.DefaultTitleForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
            if (btn.Tag == "1")
            {
                //btn.BackColor = Color.FromArgb(72, 77, 85);

                ControlPaint.DrawBorder(
                   e.Graphics,
                   btn.ClientRectangle,
                   Color.FromArgb(0, 191, 255),      // 左侧线颜色
                   2,               // 左侧线宽度
                   ButtonBorderStyle.Solid,
                   Color.FromArgb(0, 191, 255),      // 右侧线颜色
                   2,               // 右侧线宽度
                   ButtonBorderStyle.Solid,
                   Color.FromArgb(0, 191, 255),      // 上侧线颜色
                   2,               // 上侧线宽度
                   ButtonBorderStyle.Solid,
                   Color.FromArgb(0, 191, 255),      // 下侧线颜色
                   2,               // 下侧线宽度
                   ButtonBorderStyle.Solid);

            }

        }
        private void IconBtn_Paint(object sender, PaintEventArgs e)
        {
            ScopeXIconButton btn = (ScopeXIconButton)sender;
            if (btn.IsIndicatorShow == true) ;
            else
            {
                Color color1 = AppStyleConfig.DefaultGradualBackColorOne;
                Color color2 = AppStyleConfig.DefaultGradualBackColorTwo;
                ISvgRenderer Renderer = SvgRenderer.FromGraphics(e.Graphics);
                int newWidth = btn.IconSize.Width;
                int newHeight = btn.IconSize.Height;
                int thumbnailWidth = 30;
                int thumbnailHeight = 30;
                Single scale = Math.Min((Single)thumbnailWidth / newWidth, (Single)thumbnailHeight / newHeight);
                int scaledWidth = (int)(newWidth * scale);
                int scaledHeight = (int)(newHeight * scale);
                Image img = btn.Icon;
                img = btn.Icon.GetThumbnailImage(scaledWidth, scaledHeight, null, IntPtr.Zero);
                using (var brush = new LinearGradientBrush(btn.ClientRectangle, color1, color2, LinearGradientMode.Vertical))
                {
                    e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                    e.Graphics.FillRectangle(brush, btn.ClientRectangle);
                    Size IndicatorSize = new Size(12, 2);
                    int IndicatorBottom = 2;
                    int icontop = btn.IsIndicatorShow ? (Height - IndicatorSize.Height - IndicatorBottom - btn.IconSize.Height) / 2
                                          : (btn.Height - btn.IconSize.Height) / 2;
                    e.Graphics.SetSmoothMode(true);
                    Renderer.DrawImage(btn.Icon, new RectangleF(btn.IconOffset, icontop, btn.IconSize.Width, btn.IconSize.Height), new RectangleF(new Point(btn.ClientRectangle.X, btn.ClientRectangle.Y), new Size(btn.Icon.Width, btn.Icon.Height)), GraphicsUnit.Pixel);

                    e.Graphics.SetSmoothMode(false);
                }

            }
            if (btn.Tag == "1")
            {
                //btn.BackColor = Color.FromArgb(72, 77, 85);

                ControlPaint.DrawBorder(
                   e.Graphics,
                   btn.ClientRectangle,
                   Color.FromArgb(0, 191, 255),      // 左侧线颜色
                   2,               // 左侧线宽度
                   ButtonBorderStyle.Solid,
                   Color.FromArgb(0, 191, 255),      // 右侧线颜色
                   2,               // 右侧线宽度
                   ButtonBorderStyle.Solid,
                   Color.FromArgb(0, 191, 255),      // 上侧线颜色
                   2,               // 上侧线宽度
                   ButtonBorderStyle.Solid,
                   Color.FromArgb(0, 191, 255),      // 下侧线颜色
                   2,               // 下侧线宽度
                   ButtonBorderStyle.Solid);
            }






        }

        private void Si_MouseDown(object sender, MouseEventArgs e)
        {
            StickyInfo info = (StickyInfo)sender;
            info.Padding = new Padding(info.Padding.Left + 1,
                                           info.Padding.Top + 1,
                                           info.Padding.Right + 1,
                                           info.Padding.Bottom + 1);
            //info.BackColor = Color.FromArgb(0,191,255);
            info.Tag = "1";
            Invalidate();

        }
        private void Si_MouseUp(object sender, MouseEventArgs e)
        {
            StickyInfo info = (StickyInfo)sender;
            info.Padding = new Padding(info.Padding.Left - 1,
                                           info.Padding.Top - 1,
                                           info.Padding.Right - 1,
                                           info.Padding.Bottom - 1);
            //info.BackColor = Color.FromArgb(41, 42, 45);
            info.Tag = "0";
            Invalidate();
        }
        private void Si_PaddingChanged(object sender, EventArgs e)
        {
            StickyInfo info = (StickyInfo)sender;
            Graphics graphics = this.CreateGraphics();
            if (info.Tag == "1")
            {
                ControlPaint.DrawBorder(
                  graphics,
                  info.ClientRectangle,
                  Color.FromArgb(0, 191, 255),      // 左侧线颜色
                  1,               // 左侧线宽度
                  ButtonBorderStyle.Solid,
                  Color.FromArgb(0, 191, 255),      // 右侧线颜色
                  1,               // 右侧线宽度
                  ButtonBorderStyle.Solid,
                  Color.FromArgb(0, 191, 255),      // 上侧线颜色
                  1,               // 上侧线宽度
                  ButtonBorderStyle.Solid,
                  Color.FromArgb(0, 191, 255),      // 下侧线颜色
                  1,               // 下侧线宽度
                  ButtonBorderStyle.Solid);
            }

        }

        public void ReLoadSource()
        {
            SiTrigger.DataSource = TriggerInfoList();
            SetTriggerTip();
        }

        public void Reload()
        {
            SiTrigger.DataSource = TriggerInfoList();
            SetTriggerTip();
        }

        public void UpdateThresholdUnit()
        {
        }

        private void BtnPrinter_Click(object sender, EventArgs e)
        {

        }

        private sealed class AiSetMenuColorTable : ProfessionalColorTable
        {
            public override Color ToolStripDropDownBackground => AppStyleConfig.DefaultContextDarkBackColor;
            public override Color ImageMarginGradientBegin => AppStyleConfig.DefaultContextDarkBackColor;
            public override Color ImageMarginGradientMiddle => AppStyleConfig.DefaultContextDarkBackColor;
            public override Color ImageMarginGradientEnd => AppStyleConfig.DefaultContextDarkBackColor;
            public override Color MenuBorder => AppStyleConfig.DefaultBorderColor;
            public override Color MenuItemBorder => AppStyleConfig.DefaultFocusBorderColor;
            public override Color MenuItemSelected => AppStyleConfig.DefaultTitleBackColor;
            public override Color MenuItemSelectedGradientBegin => AppStyleConfig.DefaultTitleBackColor;
            public override Color MenuItemSelectedGradientEnd => AppStyleConfig.DefaultTitleBackColor;
            public override Color SeparatorDark => AppStyleConfig.DefaultBorderColor;
            public override Color SeparatorLight => AppStyleConfig.DefaultBorderColor;
        }
    }
}

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.UserControls;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;
using ScopeX.UserControls.Style;
using ScopeX.Core;
using ScopeX.Controls.Common.Helper;
using ScopeX.U2.LanguageSupoort;
using ScopeX.Core.Tools;

namespace ScopeX.U2
{
    public partial class StartForm : FloatForm
    {
        //private readonly string _Title;         //记录用户输入的标题
        ////private Timer _TitleTimer;              //刷新Title内容的计时器
        //MouseDragMsgFilter _MouseDragMsgFilter; //处理鼠标事件的消息处理器
        //private Point _MousePreviousPoint;      //鼠标按下时的点坐标
        ////标签内容区的配对集合
        //private List<(ScopeXIconButton label, UserControl Area, int originY)> _LabelAreaPairs
        //    = new List<(ScopeXIconButton label, UserControl Area, int originY)>();

        

        public StartForm()
        {
            InitializeComponent();
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

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Stylize();
            InitOnLoad();
            // LanguageFactory.CacheFormLanguageControls(this);
        }

        private void Stylize()
        {
            IsShowHelp = false;
            //DefaultStyleManager.Instance.RegisterControlRecursion(this);
            //HeadBackColor = Color.FromArgb(62, 62, 62);
        }

        private void InitOnLoad()
        {
            Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm", DateTimeFormatInfo.InvariantInfo);

            foreach (var c in TlpBody.Controls.OfType<ScopeXIconButton>())
            {
                c.MouseinBackColor = ControlPaint.Light(c.BackColor, 0.1F);
                c.PressedBackColor = ControlPaint.Light(c.BackColor, 0.2F);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _ = NativeMethods.PostMessage(Owner.Handle, NativeMethods.WM_KEYDOWN, (Int32)e.KeyCode, 0);
        }



        ///// <summary>
        ///// 调用ContentPanel上的控件的事件处理方法
        ///// </summary>
        ///// <param name="eventProc">事件处理方法的名称</param>
        ///// <param name="EventLocation">事件发生的地点，原点为屏幕坐标系</param>
        //private void InvokeContentPanelEvent(String eventProc, Point EventLocation)
        //{
        //    Control ctl = PnlSlideContent;
        //    while(true)
        //    {
        //        Control temp = ctl.GetChildAtPoint(PnlSlideContent.PointToClient(EventLocation));
        //        if(temp == null)
        //        {
        //            //调用事件处理函数
        //            var onclickInfo = ctl.GetType().GetMethod(eventProc, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        //            onclickInfo.Invoke(ctl, new object[] { new EventArgs()});
        //            break;
        //        }
        //        else
        //        {
        //            ctl = temp;
        //        }
        //    }

        //}

        ///// <summary>
        ///// 选中按钮标签
        ///// </summary>
        ///// <param name="choosedBtn"></param>
        //private void ChooseBtnLabel(ScopeXIconButton choosedBtn)
        //{
        //    //调整区域滑动位置
        //    if (choosedBtn == _LabelAreaPairs.First().label)
        //    {
        //        PnlSlideContent.VerticalScroll.Value = PnlSlideContent.VerticalScroll.Minimum;
        //    }
        //    else if (choosedBtn == _LabelAreaPairs.Last().label)
        //    {
        //        if (PnlSlideContent.VerticalScroll.Maximum > PnlSlideContent.Height)
        //            PnlSlideContent.VerticalScroll.Value = PnlSlideContent.VerticalScroll.Maximum - PnlSlideContent.Height;
        //    }
        //    else
        //    {
        //        var areaPair = _LabelAreaPairs.First(pair => pair.label == choosedBtn);
        //        if(PnlSlideContent.VerticalScroll.Maximum > PnlSlideContent.Height)
        //            PnlSlideContent.VerticalScroll.Value = Math.Min(PnlSlideContent.VerticalScroll.Maximum - PnlSlideContent.Height, areaPair.originY);
        //    }
        //    PnlSlideContent.PerformLayout();

        //    SetChoosedBtnLabelColor(choosedBtn);
        //    this.Invalidate();
        //}

        //private void SetChoosedBtnLabelColor(ScopeXIconButton choosedBtn)
        //{
        //    //调整标签颜色
        //    BtnNormal.BackColor = this.ContentBackColor;
        //    BtnMeasureAnalyze.BackColor = this.ContentBackColor;
        //    BtnFunc.BackColor = this.ContentBackColor;

        //    choosedBtn.BackColor = AppStyleConfig.DefaultCheckedBackColor;
        //    choosedBtn.Focus();
        //}

        ///// <summary>
        ///// 滚动到响应的按钮标签
        ///// </summary>
        ///// <param name="isDown">是否往下</param>
        //private void ScrollToBtnLabel(bool isDown)
        //{
        //    if (PnlSlideContent.VerticalScroll.Value == PnlSlideContent.VerticalScroll.Minimum)
        //    {
        //        SetChoosedBtnLabelColor(_LabelAreaPairs.First().label);
        //    }
        //    //判断滚动到底，不能直接用Maximum,控件高度有影响；
        //    else if (PnlSlideContent.VerticalScroll.Value >= PnlSlideContent.VerticalScroll.Maximum - PnlSlideContent.Height)
        //    {
        //        SetChoosedBtnLabelColor(_LabelAreaPairs.Last().label);
        //    }
        //    else
        //    {
        //        //切换到相应的标签
        //        for (int i = 0; i < _LabelAreaPairs.Count; i++)
        //        {
        //            int index = isDown ? i : (_LabelAreaPairs.Count - 1 - i);
        //            var pair = _LabelAreaPairs[index];

        //            int areaTop = pair.originY;
        //            int areaBottom = areaTop + pair.Area.Height;
        //            if (PnlSlideContent.VerticalScroll.Value >= areaTop &&
        //                PnlSlideContent.VerticalScroll.Value <= areaBottom)
        //            {
        //                SetChoosedBtnLabelColor(pair.label);
        //                break;
        //            }
        //        }
        //    }
        //}

        //private void Stylize()
        //{
        //    DefaultStyleManager.Instance.RegisterControlRecursion(this);
        //    //风格属性调整
        //    this.HeadBackColor = AppStyleConfig.DefaultTitleBackColor;
        //    this.IconSideDistance = 3;

        //    BtnNormal.MouseinBackColor = AppStyleConfig.DefaultCheckedBackColor;
        //    BtnNormal.BackColor = this.ContentBackColor;

        //    BtnMeasureAnalyze.MouseinBackColor = AppStyleConfig.DefaultCheckedBackColor;
        //    BtnMeasureAnalyze.BackColor = this.ContentBackColor;

        //    BtnFunc.MouseinBackColor = AppStyleConfig.DefaultCheckedBackColor;
        //    BtnFunc.BackColor = this.ContentBackColor;

        //    BtnLogo.BackColor = Color.SteelBlue; 
        //    BtnLogo.MouseinBackColor = BtnLogo.BackColor;
        //    BtnLogo.PressedBackColor = BtnLogo.BackColor;

        //    SystemPage.SetElementStyle();
        //    FuncPage.SetElementStyle();
        //    MeasureAnalyzePage.SetElementStyle();
        //    NormalPage.SetElementStyle();
        //    LblTemperature.BackColor = AppStyleConfig.DefaultTitleBackColor;
        //}

        //protected override void OnLoad(EventArgs e)
        //{
        //    //窗体风格注册
        //    Stylize();

        //    base.OnLoad(e);

        //    //开启标题栏内容更新
        //    Title = _Title + " " + DateTime.Now.ToString("yyyy/MM/dd HH:mm", DateTimeFormatInfo.InvariantInfo);

        //    //注册消息拦截器
        //    _MouseDragMsgFilter = new MouseDragMsgFilter(PnlSlideContent,
        //                                                PnlSlideContent_MouseUp,
        //                                                PnlSlideContent_MouseDown,
        //                                                PnlSlideContent_MouseMove,
        //                                                PnlSlideContent_MouseWheel);
        //    Application.AddMessageFilter(_MouseDragMsgFilter);

        //    //!!!zqc 11.05
        //    //调整滑动内容区的位置
        //    //ChooseBtnLabel(BtnNormal);
        //}

        //protected override void OnActivated(EventArgs e)
        //{
        //    base.OnActivated(e);

        //    ChooseBtnLabel(BtnNormal);
        //}

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    base.OnClosing(e);
        //    //e.Cancel = true;
        //    //this.Visible = false;
        //    //Trace.WriteLine($"=====Start Form (id = {Handle}) =====");
        //    //TmUpdate.Enabled = false;
        //}

        //protected override void OnClosed(EventArgs e)
        //{
        //    //注销消息拦截器
        //    Application.RemoveMessageFilter(_MouseDragMsgFilter);

        //    base.OnClosed(e);
        //}

        //#region 事件处理相关方法

        //private void BtnNormal_Click(object sender, EventArgs e)
        //{
        //    ChooseBtnLabel(sender as ScopeXIconButton);
        //}

        //private void BtnMeasureAnalyze_Click(object sender, EventArgs e)
        //{
        //    ChooseBtnLabel(sender as ScopeXIconButton);
        //}

        //private void BtnFunc_Click(object sender, EventArgs e)
        //{
        //    ChooseBtnLabel(sender as ScopeXIconButton);
        //}

        //private void PnlSlideContent_MouseDown(object sender, MouseEventArgs args)
        //{
        //    _MousePreviousPoint = args.Location;
        //}

        //private void PnlSlideContent_MouseMove(object sender, MouseEventArgs args)
        //{
        //    Int32 scrollmin = PnlSlideContent.VerticalScroll.Minimum;
        //    Int32 scrollmax = PnlSlideContent.VerticalScroll.Maximum - PnlSlideContent.Height;
        //    Int32 deltay = args.Y - _MousePreviousPoint.Y;

        //    //无移动或不需要滑动。直接退出
        //    if(deltay == 0 || scrollmax < 0)
        //    {
        //        return;
        //    }

        //    //赋值新的滚动值
        //    Int32 newvalue = PnlSlideContent.VerticalScroll.Value - deltay;
        //    if (newvalue > scrollmax)
        //    {
        //        PnlSlideContent.VerticalScroll.Value = scrollmax;
        //    }
        //    else if(newvalue < scrollmin)
        //    {
        //        PnlSlideContent.VerticalScroll.Value = scrollmin;
        //    }
        //    else
        //    {
        //        PnlSlideContent.VerticalScroll.Value = newvalue;
        //    }
        //    PnlSlideContent.PerformLayout();

        //    ScrollToBtnLabel(deltay < 0);

        //    _MousePreviousPoint = args.Location;
        //}

        //private void PnlSlideContent_MouseUp(object sender, MouseEventArgs args)
        //{

        //}

        //private void PnlSlideContent_MouseWheel(object sender, MouseEventArgs args)
        //{
        //    ScrollToBtnLabel(args.Delta < 0);
        //}

        //#endregion 事件处理相关方法

        private void TmUpdate_Tick(Object sender, EventArgs e)
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
                Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm", DateTimeFormatInfo.InvariantInfo);
                //LblTemperature.Text = WidgetPrsnt.GetAnalogChnlTemperature().ToString("##0.##") + "℃";
            }
        }

        private void BtnAwg1_Click(Object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_AWG1);
            //BtnAwg1.MouseinBackColor = ControlPaint.Light(BtnAwg1.BackColor, 0.1F);
        }

        private void BtnAwg2_Click(Object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_AWG2);
        }

        private void BtnCymometer_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_CYMOMETER);
        }

        private void BtnDvm_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_VOLTMETER);
        }

        private void BtnAutoSet_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.AUTOSET);
        }

        private void BtnMeasure_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, -KeyCode.MEASURE);
        }

        private void BtnSnapShot_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_SNAPSHOT);
        }

        private void BtnCursor_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, -KeyCode.CURSOR);
        }

        private void BtnSave_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.STORAGE);
        }

        private void BtnPrinter_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_SETPRINTER);
        }

        private void BtnLissajous_Click(Object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_LISSAJOUS);
        }

        private void BtnFailTest_Click(Object sender, EventArgs e)
        {
            //WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_PASSFAIL);
        }

        private void BtnPwrAnalysis_Click(Object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_PWRANALYSIS);
        }

        private void BtnJitterAnalysis_Click(Object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_SDA);
        }

        private void BtnVsaAnalysis_Click(Object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_VSA);
        }

        private void BtnZoom_Click(Object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_ZOOM);
        }

        private void BtnSearch_Click(Object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_WAVESEARCH);
        }

        private void BtnScreenShot_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_SCREENSHOT);
        }

        private void BtnLayerOff_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.LAYEROFF);
        }

        private void BtnClear_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_CLEAR);
        }

        private void BtnSetting_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.SETTING);
        }

        private void BtnRecover_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.DEFAULT);
        }

        private void BtnAbout_Click(Object sender, EventArgs e)
        {
            //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_ABOUT);
            AboutForm af = new() { StartPosition = FormStartPosition.CenterScreen };
            CanClose = false;
            af.ShowDialogByEvent();
            CanClose = true;
        }

        private void BtnMinimize_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_MINIMIZE);
        }

        private void BtnClose_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_CLOSE);
        }

        private void BtnShutDown_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_SHUTDOWN);
        }

        private void BtnRestart_Click(Object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_RESTART);
        }

        private void BtnDebug_Click(Object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            ////_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_DEBUG);
            //DebugForm df = new() { StartPosition = FormStartPosition.CenterScreen };
            //CanClose = false;
            //df.ShowDialogByPosition();
            //CanClose = true;
        }

    }
}

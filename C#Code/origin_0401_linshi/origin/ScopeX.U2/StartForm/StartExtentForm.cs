using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Core.Tools;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using ScopeX.ComModel;
using ScopeX.U2.Tools;
using ScopeX.U2.LanguageSupoort;
using System.Reflection.Emit;
using ScopeX.Controls.Language;
using ScopeX.Controls.Common.Helper;
using ScopeX.Controls.Common.Structs;
using EventBus;

namespace ScopeX.U2
{
    public partial class StartExtentForm : FloatForm
    {
        public static StartExtentForm Defalut { get; } = new StartExtentForm();
        private int _Width;
        private int _Height;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Stylize();

            LanguageFactory.CacheFormLanguageControls(this);
            LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
        }

        private void Instance_LanguageChanged(object sender, ILanguage e)
        {
            //LblDemo.Text = LanguageManger.Instance.GetIDMessage("YanShi");
            //LblUserSetting.Text = LanguageManger.Instance.GetIDMessage("ZiDingYiChuangKou");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            this.Visible = false;
            base.OnFormClosing(e);
            Size = new Size(_Width, _Height);

        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
            base.OnClosing(e);
            //this.Dispose();
            LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
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

        private StartExtentForm()
        {
            InitializeComponent();
            Stylize();
            _Width = Width;
            _Height = Height;
            DsoForm mainform = (DsoForm)Program.Oscilloscope.View;
            Location = new Point(mainform.Width - Width, 0);
            BtnUserSetting.Visible = LblUserSetting.Visible = PlatformUIManager.Default.Platform.Attribute.SupportUtilityKey;
        }

        private void Stylize()
        {
            IsShowHelp = false;
            //for (int i = 1; i <= 6; i += 2)
            //{
            //    TlpBody.RowStyles[i] = new RowStyle(SizeType.Absolute, AppStyleConfig.DefaultLabelHeight);
            //}
            for (int i = 0; i < TlpBody.ColumnCount; i++)
            {
                TlpBody.ColumnStyles[i] = new ColumnStyle(SizeType.Absolute, 90);
            }

            foreach (var ctl in TlpBody.Controls)
            {
                if (ctl is ScopeXLabel label)
                {
                    label.Font = AppStyleConfig.DefaultLabelFont;
                    label.Height = AppStyleConfig.DefaultLabelHeight;
                }
                if (ctl is ScopeXIconButton btn)
                {
                    btn.MouseinBackColor = TlpBody.BackColor.GetBrightnessColor(0.1);
                }
            }

            SystemPage.SetElementStyle();
        }

        #region EventHandle

        private void BtnFft_Click(object sender, EventArgs e)
        {
            var fftmath = Program.Oscilloscope.TryGetRange(c => c.Id.IsMath() && c.Id <= ChannelIdExt.MaxMChId && c.Active).Select(p => (Core.MathPrsnt)p).Where(m => m.Args.Type == Core.MathType.FFT).ToList();
            if (fftmath != null && fftmath.Count >= 2)
            {
                WeakTip.Default.Write("Math", MsgTipId.NoMoreChannels);
                return;
            }
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_FFT);

        }

        private void BtnEyePattern_Click(object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            // _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_EYEPATTERN);

        }

        private void BtnCursor_Click(object sender, EventArgs e)
        {
            //CursorApp.Default.IsShowFuncForm = true;
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, -KeyCode.CURSOR);
        }

        private void BtnThreeD_Click(object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_3D);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.STORAGE);

        }

        private void BtnFailTest_Click(object sender, EventArgs e)
        {
            //WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_PASSFAIL);
        }

        private void BtnPrinter_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_SETPRINTER);

        }

        private void BtnAbout_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_ABOUT);
        }

        private void BtnDebug_Click(object sender, EventArgs e)
        {
            //  _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_DEBUG);
        }

        private void BtnScreenShot_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_SCREENSHOT);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_CLEAR);
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_RESET);
        }

        private void BtnRecover_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.DEFAULT/*KeyCode.VK_RECOVER*/);
        }

        private void BtnSetting_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.SETTING);
        }

        private void BtnPwrAnalysis_Click(object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            //if (OptionManager.Default.Checked(OptionType.Pwr) == false)
            //{
            //    return;
            //}
            //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_PWRANALYSIS);
        }

        private void BtnFastReq_Click(object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.FASTACQ);
        }

        private void BtnJitterAnalysis_Click(object sender, EventArgs e)
        {
            //WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            ////if (OptionManager.Default.Checked(OptionType.Jitter) == false)
            //{
            //    return;
            //}
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_SDA);
            //if ((Program.Oscilloscope.View as DsoForm).TryAddJitterInfo())
            //{
            //    return;
            //}
            //WeakTip.Default.Write("JitterAnalysis", MsgTipId.NoMoreChannels);
        }

        private void BtnLissajous_Click(object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            // _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_LISSAJOUS);
        }

        private void BtnLayerOff_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.LAYEROFF);
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            // _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_WAVESEARCH);
        }
        private void BtnTempCheck_Click(object sender, EventArgs e)
        {
            //var tempChcek = new TemperatureTestForm(Program.Oscilloscope);
            //tempChcek.Show();
            //this.Close();
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_TEMPCTRL);

        }

        private void BtnMultiDomain_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.MULTI_DOMAIN);

        }

        private void BtnAiDomain_Click(object sender, EventArgs e)
        {
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.AI_SET);

        }

        private void BtnVSA_Click(object sender, EventArgs e)
        {
            //  _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_DEBUG);
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_VSA);
        }

        private void BtnException_Click(object sender, EventArgs e)
        {
            if (Program.Oscilloscope.ExceptionCapture.Active)
            {
                return;
            }
            if (Program.Oscilloscope.View is DsoForm dso)
            {
                dso.TryAddExceptionCaptureInfo(Program.Oscilloscope.ExceptionCapture);
            }
        }

        private void BtnBegin_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void BtnUserSetting_Click(object sender, EventArgs e)
        {

            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            ////CustomizeForm custom = new CustomizeForm();

            //EventBroker.Instance.GetEvent<FormEventArgs>().Publish(custom, new() { Current = custom, Type = FormType.SettingForm });
        }
        private void BtnDemo_Click(object sender, EventArgs e)
        {
            WeakTip.Default.Write("Config", MsgTipId.ThisItemDisabled);
            //_ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_EYEPATTERN);

        }

      

        #endregion EventHandle
    }
}

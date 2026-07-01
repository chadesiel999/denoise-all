// Copyright (c) ScopeX. All Rights Reserved
// <author>Zhang XuLin</author>
// <date>2022/4/20</date>

namespace ScopeX.U2
{
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Core;
    using ScopeX.Core.Tools;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Management;
    using System.Windows.Forms;
    using static ScopeX.UserControls.SelectComboBox;

    /// <summary>
    /// Defines the <see cref="ScreenDetectionPage" />.
    /// </summary>
    public partial class ScreenDetectionPage : UserControl, IStylize//ISystemCheckView
    {
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        private Boolean _ArgToCtrl = false;

        public ScreenDetectionPage()
        {
            InitializeComponent();
            LblScreen.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.ScreenDetectionPage.LblScreen");
            BtnScreenDetection.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.ScreenDetectionPage.BtnScreenDetection");
            LblTouch.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.ScreenDetectionPage.LblTouch");
            BtnTouchDetection.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.ScreenDetectionPage.BtnTouchDetection");
            LblKeyboard.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.ScreenDetectionPage.LblKeyboard");
            BtnKeyboardDetection.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.ScreenDetectionPage.BtnKeyboardDetection");
            LblLed.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.ScreenDetectionPage.LblLed");
            BtnLEDDetection.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.ScreenDetectionPage.BtnLEDDetection");

        }

        /// <summary>
        /// Gets the DesignMode.
        /// </summary>
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

        public SystemCheckPrsnt Presenter
        {
            get;
            set;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //UpdateView();
        }
        public override void Refresh()
        {
            base.Refresh();
        }

        private void BtnScreenDetection_Click(object sender, EventArgs e)
        {
            Presenter.ScopeCheckType = CheckType.ScreenCheck;
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_SCOPE_CHECK_MASK);
            //InitChildPage(CheckType.ScreenCheck);
        }

        private void BtnTouchDetection_Click(object sender, EventArgs e)
        {
            Presenter.ScopeCheckType = CheckType.TouchCheck;
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_SCOPE_CHECK_MASK);
            //InitChildPage(CheckType.TouchCheck);
        }

        private void BtnKeyboardDetection_Click(object sender, EventArgs e)
        {
            Presenter.ScopeCheckType = CheckType.KeyboardCheck;
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.VK_SCOPE_CHECK_MASK);
            //InitChildPage(CheckType.KeyboardCheck);
        }

        private void BtnLEDDetection_Click(object sender, EventArgs e)
        {
            Presenter.ScopeCheckType = CheckType.LEDCheck;
            //InitChildPage(CheckType.LEDCheck);
        }

    }
}

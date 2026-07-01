using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using ScopeX.UserControls;
using ScopeX.Controls.Common.Default;

namespace ScopeX.U2
{
    public partial class AutoSettingPage :UserControl, IStylize
    {
        private Boolean _ArgToCtrl = false;

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.FontSize;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

        public DsoPrsnt Presenter { get; init; }

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
        public AutoSettingPage(DsoPrsnt dso)
        {
            Presenter = dso;
            InitializeComponent();
            InitControlsText();
            ChkAmpCalibration.Font = ChkChannelSetting.Font = ChkHorizontalSetting.Font = ChkTriggerSetting.Font = ChkAcquisitionSetting.Font = ChkVerticalSetting.Font = ChkCouplingHold.Font = ChkOverlapView.Font = AppStyleConfig.DefaultLabelFont;
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            _ArgToCtrl = true;
            ChkAcquisitionSetting.Checked = Presenter.AutoSet.AcquisitionSetting;
            ChkCouplingHold.Checked = Presenter.AutoSet.CouplingHold;
            ChkVerticalSetting.Checked = Presenter.AutoSet.VerticalSetting;
            ChkTriggerSetting.Checked = Presenter.AutoSet.TriggerSetting;
            ChkOverlapView.Checked = Presenter.AutoSet.OverlapView;
            ChkHorizontalSetting.Checked = Presenter.AutoSet.HorizontalSetting;
            ChkChannelSetting.Checked = Presenter.AutoSet.ChannelSetting;
            _ArgToCtrl = false;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged -= Instance_LanguageChanged;
            base.OnHandleDestroyed(e);
        }

        private void Instance_LanguageChanged(object sender, Controls.Language.ILanguage e) => InitControlsText();
        private void InitControlsText()
        {
            LblVerticalSetting.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AutoSettingPage.ChkVerticalSetting"); // "垂直设置";
            LblAutoSettingItems.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AutoSettingPage.LblAutoSettingItems"); // "自动设置项";
            LblHorizontalSetting.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AutoSettingPage.ChkHorizontalSetting"); // "水平设置";
            LblAcquisitionSetting.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AutoSettingPage.ChkAcquisitionSetting"); // "采集设置";
            LblTriggerSetting.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AutoSettingPage.ChkTriggerSetting"); // "触发设置";
            BtnAutoCorrection.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AutoSettingPage.BtnAutoCorrection"); // "自动校正";
            LblAutoCorrection.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AutoSettingPage.LblAutoCorrection"); // "自动校正";
            LblOverlapView.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AutoSettingPage.ChkOverlapView"); // "堆叠显示";
            LblCouplingHold.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AutoSettingPage.ChkCouplingHold"); // "耦合保持";
            LblChannelSetting.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AutoSettingPage.ChkChannelSetting"); // "通道设置";
            LblAmpCalibration.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("SettingForm.NbgSetting.TlpNavBarContainer.AutoSettingPage.ChkAmpCalibration"); // "幅度自校正";

        }

        private void BtnAutoCorrection_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }
            var res = StrongTip.Default.Show(MsgTipId.Warning, MsgTipId.AutoCalibration, MessageType.Warning);
            if (!res)
            {
                return;
            }
            ParentForm.Close();
            _ = NativeMethods.PostMessage((Program.Oscilloscope.View as DsoForm).Handle, 0x0400, 12, KeyCode.AUTOCALIBRATION);
        }

        private void ChkVerticalSetting_CheckedChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AutoSet.VerticalSetting = ChkVerticalSetting.Checked;
            }
        }

        private void ChkHorizontalSetting_CheckedChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AutoSet.HorizontalSetting = ChkHorizontalSetting.Checked;
            }
        }

        private void ChkAcquisitionSetting_CheckedChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AutoSet.AcquisitionSetting = ChkAcquisitionSetting.Checked;
            }
        }

        private void ChkTriggerSetting_CheckedChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AutoSet.TriggerSetting = ChkTriggerSetting.Checked;
            }
        }

        private void ChkOverlapView_CheckedChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AutoSet.OverlapView = ChkOverlapView.Checked;
            }
        }

        private void ChkCouplingHold_CheckedChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AutoSet.CouplingHold = ChkCouplingHold.Checked;
            }
        }

        private void ChkChannlSetting_CheckedChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AutoSet.ChannelSetting = ChkChannelSetting.Checked;
            }
        }
        private void ChkAmpCalibration_CheckedChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {

            }
        }
    }
}

using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Language;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class ComponentSettingPage : UserControl, IStylize
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
        public ComponentSettingPage(DsoPrsnt dso)
        {
            Presenter = dso;
            InitializeComponent();
            InitLanguages();
            ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += (_, _) =>
            {
                InitLanguages();
            };
        }

        private void InitLanguages()
        {
            LblComponent.Text = LanguageManger.Instance.GetIDMessage("KeXuanZuJian");
            StAwg.Texts = new string[] { LanguageManger.Instance.GetIDMessage("YiQiYong"), LanguageManger.Instance.GetIDMessage("YiJinYong") };
            StBus.Texts = new string[] { LanguageManger.Instance.GetIDMessage("YiQiYong"), LanguageManger.Instance.GetIDMessage("YiJinYong") };
            StMath.Texts = new string[] { LanguageManger.Instance.GetIDMessage("YiQiYong"), LanguageManger.Instance.GetIDMessage("YiJinYong") };
            StSearch.Texts = new string[] { LanguageManger.Instance.GetIDMessage("YiQiYong"), LanguageManger.Instance.GetIDMessage("YiJinYong") };
            StPassFail.Texts = new string[] { LanguageManger.Instance.GetIDMessage("YiQiYong"), LanguageManger.Instance.GetIDMessage("YiJinYong") };
            StPower.Texts = new string[] { LanguageManger.Instance.GetIDMessage("YiQiYong"), LanguageManger.Instance.GetIDMessage("YiJinYong") };
            StMeasure.Texts = new string[] { LanguageManger.Instance.GetIDMessage("YiQiYong"), LanguageManger.Instance.GetIDMessage("YiJinYong") };
            StSegement.Texts = new string[] { LanguageManger.Instance.GetIDMessage("YiQiYong"), LanguageManger.Instance.GetIDMessage("YiJinYong") };
            StJitter.Texts = new string[] { LanguageManger.Instance.GetIDMessage("YiQiYong"), LanguageManger.Instance.GetIDMessage("YiJinYong") };
            StLissajous.Texts = new string[] { LanguageManger.Instance.GetIDMessage("YiQiYong"), LanguageManger.Instance.GetIDMessage("YiJinYong") };
            StRef.Texts = new string[] { LanguageManger.Instance.GetIDMessage("YiQiYong"), LanguageManger.Instance.GetIDMessage("YiJinYong") };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
            UpdateControlsStatus();
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
            UpdateControlsStatus();
        }

        protected void UpdateView()
        {
            _ArgToCtrl = true;

            StAwg.Checked = Constants.ENABLE_AWG;
            //StFFT.Checked = Constants.ENABLE_FFT;
            StSearch.Checked = Constants.ENABLE_Search;
            StJitter.Checked = Constants.ENABLE_SDA;
            StSegement.Checked = Constants.ENABLE_Segement;
            StMeasure.Checked = Constants.ENABLE_Measure;
            StPassFail.Checked = Constants.ENABLE_PassFail;
            StPower.Checked = Constants.ENABLE_PowerAs;
            StBus.Checked = Constants.ENABLE_BUS;
            StLissajous.Checked = Constants.ENABLE_Lissajous;
            StMath.Checked = Constants.ENABLE_Math;
            StRef.Checked = Constants.ENABLE_Ref;
            _ArgToCtrl = false;
        }

        protected void UpdateControlsStatus()
        {
            StAwg.Enabled = Program.Oscilloscope.OptionsManager.GetOptionAvailable(OptionType.AWG);
            StJitter.Enabled = Program.Oscilloscope.OptionsManager.GetOptionAvailable(OptionType.Jitter);
            StPower.Enabled = Program.Oscilloscope.OptionsManager.GetOptionAvailable(OptionType.Pwr);
        }

        private void StSegement_CheckedChanged(Object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                AppConfig.GetIntance().ENABLE_Segement = StSegement.Checked;
                SaveConfig();
            }
        }

        private void StMeasure_CheckedChanged(Object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                AppConfig.GetIntance().ENABLE_Measure = StMeasure.Checked;
                SaveConfig();
            }
        }

        private void StPassFail_CheckedChanged(Object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                AppConfig.GetIntance().ENABLE_PassFail = StPassFail.Checked;
                SaveConfig();
            }
        }

        private void StSearch_CheckedChanged(Object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                AppConfig.GetIntance().ENABLE_Search = StSearch.Checked;
                SaveConfig();
            }
        }

        private void StPower_CheckedChanged(Object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                AppConfig.GetIntance().ENABLE_PowerAs = StPower.Checked;
                SaveConfig();
            }
        }

        private void StBus_CheckedChanged(Object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                AppConfig.GetIntance().ENABLE_BUS = StBus.Checked;
                SaveConfig();
            }
        }

        private void StLissajous_CheckedChanged(Object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                AppConfig.GetIntance().ENABLE_Lissajous = StLissajous.Checked;
                SaveConfig();
            }
        }

        private void StJitter_CheckedChanged(Object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                AppConfig.GetIntance().ENABLE_SDA = StJitter.Checked;
                SaveConfig();
            }
        }

        private void StAwg_CheckedChanged(Object sender, System.EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                AppConfig.GetIntance().ENABLE_AWG = StAwg.Checked;
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            AppConfig.GetIntance().SaveConfig();
            WeakTip.Default.Write("Component", MsgTipId.RestartingProgram);
        }

        private void StRef_CheckedChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                AppConfig.GetIntance().ENABLE_Ref = StRef.Checked;
                SaveConfig();
            }
        }

        private void StMath_CheckedChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                AppConfig.GetIntance().ENABLE_Math = StMath.Checked;
                SaveConfig();
            }
        }
    }
}

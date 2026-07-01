using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Language;
using ScopeX.Core;
using ScopeX.UserControls.Style;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class AuxOutputPage : UserControl, ISettingView, IStylize
    {
        private Boolean _ArgToCtrl;
        public AuxOutputPage()
        {
            //ScopeX.Controls.Language.LanguageManger.Instance.LanguageChanged += Instance_LanguageChanged;
            InitializeComponent();
            ProductAdaptation();
        }

        private void ProductAdaptation()
        {
            if (!PlatformUIManager.Default.Platform.Attribute.SupportAuxIn)
            {
                LblAuxInput.Visible = false;
                LblAuxInPolarity.Visible = false;
                RboAuxInput.Visible = false;
                RboAuxInPolarity.Visible = false;


                int offsety = 137;
                LblAuxOutput.Location = new System.Drawing.Point(LblAuxOutput.Location.X, LblAuxOutput.Location.Y - offsety);
                RboAuxOutput.Location = new System.Drawing.Point(RboAuxOutput.Location.X, RboAuxOutput.Location.Y - offsety);

                LblAuxOutPolarity.Location = new System.Drawing.Point(LblAuxOutPolarity.Location.X, LblAuxOutPolarity.Location.Y - offsety);
                RboAuxOutPolarity.Location = new System.Drawing.Point(RboAuxOutPolarity.Location.X, RboAuxOutPolarity.Location.Y - offsety);
            }
        }

        private void Instance_LanguageChanged(object sender, ILanguage e)
        {
            if (this.Visible)
            {
                if (Presenter.AuxInputSignal == AuxInputType.Trigger)
                {
                    LblAuxInTip.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MsgTipId.AuxInTriggerTip");//"提示：请正确接入外触发信号，否则示波器无法触发！";
                }
                else if (Presenter.AuxInputSignal == AuxInputType.Sync_AWG)
                {
                    LblAuxInTip.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MsgTipId.AuxInSyncAWGTip");//"提示：请正确接入外触发信号，否则AWG无信号输出！";
                }
            }
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

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

        public SettingPrsnt Presenter
        {
            get;
            set;
        }

        ISettingPrsnt IView<ISettingPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (SettingPrsnt)value;
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }
        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                RboAuxInput.ChoosedButtonIndex = (Int32)Presenter.AuxInputSignal;
                RboAuxInPolarity.ChoosedButtonIndex = (Int32)Presenter.AuxInPolarity;
                RboAuxOutput.ChoosedButtonIndex = (Int32)Presenter.AuxOutputSignal;
                if (Presenter.AuxOutputSignal == AuxOutputType.Other)
                    RboAuxOutput.ChoosedButtonIndex = 3;
                RboAuxOutPolarity.ChoosedButtonIndex = (Int32)Presenter.AuxOutPolarity;
                if (Presenter.AuxInputSignal == AuxInputType.Trigger)
                {
                    LblAuxInTip.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MsgTipId.AuxInTriggerTip");//"提示：请正确接入外触发信号，否则示波器无法触发！";
                    LblAuxInTip.Visible = true;
                }
                else if (Presenter.AuxInputSignal == AuxInputType.Sync_AWG)
                {
                    LblAuxInTip.Text = ScopeX.Controls.Language.LanguageManger.Instance.GetIDMessage("MsgTipId.AuxInSyncAWGTip");//"提示：请正确接入外触发信号，否则AWG无信号输出！";
                    LblAuxInTip.Visible = true;
                }
                else
                    LblAuxInTip.Visible = false;
                _ArgToCtrl = false;
            }

        }

        public void UpdateView(Object presenter, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { presenter, propertyName });
            }
            else
                Update(presenter, propertyName);

        }
        protected void Update(Object presenter, String propertyName)
        {
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.AuxInputSignal):
                    RboAuxInput.ChoosedButtonIndex = (Int32)Presenter.AuxInputSignal;
                    UpdateAuxInTips();
                    break;
                case nameof(Presenter.AuxInPolarity):
                    RboAuxInPolarity.ChoosedButtonIndex = (Int32)Presenter.AuxInPolarity;
                    break;
                case nameof(Presenter.AuxOutputSignal):
                    RboAuxOutput.ChoosedButtonIndex = Presenter.AuxOutputSignal == AuxOutputType.Other ? 3 : (Int32)Presenter.AuxOutputSignal;
                    UpdateAuxOutTips();
                    break;
                case nameof(Presenter.AuxOutPolarity):
                    RboAuxOutPolarity.ChoosedButtonIndex = (Int32)Presenter.AuxOutPolarity;
                    break;
                default:
                    break;
            }
            _ArgToCtrl = false;
        }

        private void RboAuxInput_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AuxInputSignal = (AuxInputType)RboAuxInput.ChoosedButtonIndex;
            }
        }

        private void UpdateAuxInTips()
        {
            if (Presenter.AuxInputSignal == AuxInputType.Trigger)
            {
                CloseWfmGenTriger();
                LblAuxInTip.Text = LanguageManger.Instance.GetIDMessage("MsgTipId.AuxInTriggerTip");
                LblAuxInTip.Visible = true;
            }
            else if (Presenter.AuxInputSignal == AuxInputType.Sync_AWG)
            {
                // < Remark > 更改人：彭博 创建日期：2024 / 1 / 22 15:50:00  原因：辅助输出选择关联触发输出 </ Remark >
                if (DsoPrsnt.DefaultDsoPrsnt.GetWfmGenerator(ChannelId.AWG1).WfmGenTriger != TriggerSource.Outside && DsoPrsnt.DefaultDsoPrsnt.GetWfmGenerator(ChannelId.AWG2).WfmGenTriger != TriggerSource.Outside)
                {
                    DsoPrsnt.DefaultDsoPrsnt.GetWfmGenerator(ChannelId.AWG1).WfmGenTriger = TriggerSource.Outside;
                }
                LblAuxInTip.Text = LanguageManger.Instance.GetIDMessage("MsgTipId.AuxInSyncAWGTip");
                LblAuxInTip.Visible = true;
            }
            else
            {
                CloseWfmGenTriger();
                LblAuxInTip.Visible = false;
            }
        }

        /// <summary>
        /// 关闭触发输出
        /// </summary>
        /// < Remark > 更改人：彭博 创建日期：2024 / 1 / 22 15:50:00  原因：辅助输出选择关联触发输出 </ Remark >
        public void CloseWfmGenTriger()
        {
            if (DsoPrsnt.DefaultDsoPrsnt.GetWfmGenerator(ChannelId.AWG1).WfmGenTriger == TriggerSource.Outside)
            {
                DsoPrsnt.DefaultDsoPrsnt.GetWfmGenerator(ChannelId.AWG1).WfmGenTriger = TriggerSource.Inside;
            }
            if (DsoPrsnt.DefaultDsoPrsnt.GetWfmGenerator(ChannelId.AWG2).WfmGenTriger == TriggerSource.Outside)
            {
                DsoPrsnt.DefaultDsoPrsnt.GetWfmGenerator(ChannelId.AWG2).WfmGenTriger = TriggerSource.Inside;
            }
        }

        private void RboAuxInPolarity_IndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AuxInPolarity = (EdgeSlope)RboAuxInPolarity.ChoosedButtonIndex;
            }
        }
        private void RboAuxOutput_IndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AuxOutputSignal = RboAuxOutput.ChoosedButtonIndex == 3 ? AuxOutputType.Other : (AuxOutputType)RboAuxOutput.ChoosedButtonIndex;
            }
        }

        private void UpdateAuxOutTips()
        {
            // < Remark > 更改人：彭博 创建日期：2024 / 1 / 22 15:50:00  原因：辅助输出选择关联触发输出 </ Remark >
            if (Presenter.AuxOutputSignal == AuxOutputType.Sync_AWG)
            {
                if (!DsoPrsnt.DefaultDsoPrsnt.GetWfmGenerator(ChannelId.AWG1).TirgerOutEnabel && !DsoPrsnt.DefaultDsoPrsnt.GetWfmGenerator(ChannelId.AWG2).TirgerOutEnabel)
                {
                    DsoPrsnt.DefaultDsoPrsnt.GetWfmGenerator(ChannelId.AWG1).TirgerOutEnabel = true;
                }
            }
            else
            {
                DsoPrsnt.DefaultDsoPrsnt.GetWfmGenerator(ChannelId.AWG1).TirgerOutEnabel = false;
                DsoPrsnt.DefaultDsoPrsnt.GetWfmGenerator(ChannelId.AWG2).TirgerOutEnabel = false;
            }
        }

        private void RboAuxOutPolarity_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.AuxOutPolarity = (EdgeSlope)RboAuxOutPolarity.ChoosedButtonIndex;
            }
        }
    }
}

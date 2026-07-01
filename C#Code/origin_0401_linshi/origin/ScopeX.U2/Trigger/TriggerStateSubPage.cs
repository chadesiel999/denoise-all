// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/18</date>

namespace ScopeX.U2
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Controls.Common.Helper;
    using ScopeX.Core;
    using ScopeX.UserControls;
    using ScopeX.UserControls.Style;

    public partial class TriggerStateSubPage : UserControl, ITriggerView, IStylize
    {

        private Boolean _ArgToCtrl;

        private StateSettingForm _StateForm;

        public TriggerStateSubPage()
        {
            InitializeComponent();
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

        public TrigStatePrsnt Presenter
        {
            get => (TrigStatePrsnt)(ParentForm as ITriggerView).Presenter;
            set => (ParentForm as ITriggerView).Presenter = value;
        }

        ITriggerPrsnt IView<ITriggerPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (TrigStatePrsnt)value;
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

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(Presenter.ClkSource):
                    CbxClkSource.SelectedIndex = CbxClkSource.FindStringExact(Presenter.ClkSource.ToString());
                    break;
                case nameof(Presenter.ClkPolarity):
                    RdoClkPolarity.ChoosedButtonIndex = (Int32)Presenter.ClkPolarity;
                    break;
                case nameof(Presenter.Conformed):
                    ChkCondition.Checked = Presenter.Conformed;
                    break;
            }

            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                LoadClkSourceList();
                RdoClkPolarity.ChoosedButtonIndex = (Int32)Presenter.ClkPolarity;
                ChkCondition.Checked = Presenter.Conformed;
                _ArgToCtrl = false;
            }
        }

        private void LoadClkSourceList()
        {
            CbxClkSource.Items.Clear();
            CbxClkSource.Items.AddRange(ChannelIdExt.GetAnalogs().Select(o => o.ToString()).ToArray());
            CbxClkSource.Items.Add(ChannelId.Ext);
            CbxClkSource.SelectedIndex = CbxClkSource.FindStringExact(Presenter.ClkSource.ToString());
        }

        private void BtnDefine_Click(object sender, EventArgs e)
        {
            _StateForm = new StateSettingForm
            {
                StartPosition = FormStartPosition.CenterScreen,
                TrigPresenter = Presenter,
                ActiveBorderColor = AppStyleConfig.DefaultFormActiveBorderColor,
                ActiveBorderVisiable = true,
            };
            _StateForm.TrigPresenter.TryAddView(_StateForm);

            if (Program.Oscilloscope.TryGetChannel(ChannelId.D0, out var dch))
            {
                _StateForm.DigiPresenter = (DigitalPrsnt)dch;
                _StateForm.DigiPresenter.TryAddView(_StateForm);
            }

            (ParentForm as TriggerForm).CanClose = false;
            _StateForm.ShowDialogByPosition();
            (ParentForm as TriggerForm).CanClose = true;
        }

        private void CbxClkSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ClkSource = Enum.Parse<ChannelId>(CbxClkSource.SelectedItem.ToString());
            }
        }

        private void ChkMeet_CheckedChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Conformed = (sender as ScopeXSwitchButton).Checked;
            }
        }

        private void RdoClkPolarity_ButtonSelect(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.ClkPolarity = (PulsePolarity)RdoClkPolarity.ChoosedButtonIndex;
            }
        }
    }
}

namespace ScopeX.U2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Core;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.UserControls.Style;
    using ScopeX.Controls.Common.Default;
    using static ScopeX.UserControls.SelectComboBox;

    public partial class PwrEfficiencyPage : UserControl, IPwrOptionView, IStylize
    {
        private Boolean _ArgToCtrl;

        public PwrEfficiencyPage()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
            InitSourceList(Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()));
        }

        private void InitSourceList(IEnumerable<ChannelId> sources)
        {
            CbxInVoltageSrc.DataSource = sources.Select(x => new ComboBoxItem(x.GetDescription(), x, null)).ToList();

            CbxInCurrentSrc.DataSource = sources.Select(x => new ComboBoxItem(x.GetDescription(), x, null)).ToList();

            CbxOutVoltageSrc.DataSource = sources.Select(x => new ComboBoxItem(x.GetDescription(), x, null)).ToList();

            CbxOutCurrentSrc.DataSource = sources.Select(x => new ComboBoxItem(x.GetDescription(), x, null)).ToList();

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

        public PowerAnalysisOpt Mode => PowerAnalysisOpt.PowerEfficency;

        public PowerAnalysisPrsnt PowerPresenter
        {
            get;
            set;
        }


        public PwrEfficiencyPrsnt Presenter { get; set; }

        IPwrOptionPrsnt IView<IPwrOptionPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (PwrEfficiencyPrsnt)value;
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
            }
            else
            {
                Update(prsnt, propertyName);
            }
        }

        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(PowerPresenter.Active):
                    if (ChkActive.Checked != PowerPresenter.Active)
                    {
                        ChkActive.Checked = PowerPresenter.Active;
                    }
                    break;
                case nameof(Presenter.InVoltageSrc):
                    if ((ChannelId)CbxInVoltageSrc.SelectValue != Presenter.InVoltageSrc)
                    {
                        CbxInVoltageSrc.SelectValue = Presenter.InVoltageSrc;
                    }
                    break;
                case nameof(Presenter.InCurrentSrc):
                    if ((ChannelId)CbxInCurrentSrc.SelectValue != Presenter.InCurrentSrc)
                    {
                        CbxInCurrentSrc.SelectValue = Presenter.InCurrentSrc;
                    }
                    break;
                case nameof(Presenter.OutVoltageSrc):
                    if ((ChannelId)CbxOutVoltageSrc.SelectValue != Presenter.OutVoltageSrc)
                    {
                        CbxOutVoltageSrc.SelectValue = Presenter.OutVoltageSrc;
                    }
                    break;
                case nameof(Presenter.OutCurrentSrc):
                    if ((ChannelId)CbxOutCurrentSrc.SelectValue != Presenter.OutCurrentSrc)
                    {
                        CbxOutCurrentSrc.SelectValue = Presenter.OutCurrentSrc;
                    }
                    break;
                case nameof(Presenter.InputType):
                    if ((CurrentType)RdoInputType.ChoosedButtonIndex != Presenter.InputType)
                    {
                        RdoInputType.ChoosedButtonIndex = (Int32)Presenter.InputType;
                    }
                    break;
                case nameof(Presenter.OutputType):
                    if ((CurrentType)RdoOutputType.ChoosedButtonIndex != Presenter.OutputType)
                    {
                        RdoOutputType.ChoosedButtonIndex = (Int32)Presenter.OutputType;
                    }
                    break;
            }
            _ArgToCtrl = false;
        }

        private void CbxInVoltageSrc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if ((ChannelId)CbxOutCurrentSrc.SelectValue != Presenter.OutCurrentSrc)
            {
                Presenter.OutCurrentSrc = (ChannelId)CbxOutCurrentSrc.SelectValue;
            }
        }

        private void CbxOutVoltageSrc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if ((ChannelId)CbxOutVoltageSrc.SelectValue != Presenter.OutVoltageSrc)
            {
                Presenter.OutVoltageSrc = (ChannelId)CbxOutVoltageSrc.SelectValue;
            }
        }

        private void CbxInCurrentSrc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if ((ChannelId)CbxInCurrentSrc.SelectValue != Presenter.InCurrentSrc)
            {
                Presenter.InCurrentSrc = (ChannelId)CbxInCurrentSrc.SelectValue;
            }
        }

        private void CbxOutCurrentSrc_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if ((ChannelId)CbxOutCurrentSrc.SelectValue != Presenter.OutCurrentSrc)
            {
                Presenter.OutCurrentSrc = (ChannelId)CbxOutCurrentSrc.SelectValue;
            }
        }

        //!!!Notice: Dispose Presenter
        protected override void Dispose(Boolean disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            Presenter.TryRemoveView(this);

            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
            UpdateView();
            Presenter.VuTryAddPwr1 = TryAddPwr1;
            Presenter.VuTryAddPwr2 = TryAddPwr2;
            this.Refresh();
        }
        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkActive.Checked = PowerPresenter.Active;
                CbxInVoltageSrc.SelectValue = Presenter.InVoltageSrc;
                CbxInCurrentSrc.SelectValue = Presenter.InCurrentSrc;
                CbxOutVoltageSrc.SelectValue = Presenter.OutVoltageSrc;
                CbxOutCurrentSrc.SelectValue = Presenter.OutCurrentSrc;
                RdoInputType.ChoosedButtonIndex = (Int32)Presenter.InputType;
                RdoOutputType.ChoosedButtonIndex = (Int32)Presenter.OutputType;
                RdoInputType.Visible = RdoOutputType.Visible = LblInputType.Visible = LblOutputType.Visible = false;
                _ArgToCtrl = false;
            }
        }
        private void TryAddPwr1()
        {
            PowerPresenter.BoundMathPrsnt1.Active = true;
            DsoPrsnt.FocusId = PowerPresenter.BoundMathPrsnt1.Id;
            var fig = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetFigure(PowerPresenter.BoundMathPrsnt1.Id);
            if (fig is BaseDisplayForm form)
            {
                form.Activate();
            }
        }
        private void TryAddPwr2()
        {
            PowerPresenter.BoundMathPrsnt2.Active = true;
            DsoPrsnt.FocusId = PowerPresenter.BoundMathPrsnt2.Id;
            var fig = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetFigure(PowerPresenter.BoundMathPrsnt2.Id);
            if (fig is BaseDisplayForm form)
            {
                form.Activate();
            }
        }
        private void BtnShowPower1_Click(object sender, EventArgs e)
        {
            PowerPresenter.BoundMathPrsnt1.Active = true;
            DsoPrsnt.FocusId = PowerPresenter.BoundMathPrsnt1.Id;
            var fig = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetFigure(PowerPresenter.BoundMathPrsnt1.Id);
            if (fig is BaseDisplayForm form)
            {
                form.Activate();
            }

            //Presenter.TryShowEfficiencyWfmPower1();
        }

        private void BtnShowPower2_Click(object sender, EventArgs e)
        {
            PowerPresenter.BoundMathPrsnt2.Active = true;
            DsoPrsnt.FocusId = PowerPresenter.BoundMathPrsnt2.Id;
            var fig = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetFigure(PowerPresenter.BoundMathPrsnt2.Id);
            if (fig is BaseDisplayForm form)
            {
                form.Activate();
            }
        }


        private void BtnGuide_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.TryShowEfficiencyGuideForm();
        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                PowerPresenter.Active = ChkActive.Checked;
            }
        }

        private void BtnResultTable_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.Default.ShowDataTableForm(PowerPresenter);
        }

        private void RdoInputType_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.InputType = (CurrentType)RdoInputType.ChoosedButtonIndex;
            }
        }

        private void RdoOutputType_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.OutputType = (CurrentType)RdoOutputType.ChoosedButtonIndex;
            }
        }
    }
}

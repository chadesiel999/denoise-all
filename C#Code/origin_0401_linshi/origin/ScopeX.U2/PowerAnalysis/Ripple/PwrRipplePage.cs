namespace ScopeX.U2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;
    using ScopeX.ComModel;
    using ScopeX.Controls.Common.Default;
    using ScopeX.Core;
    using ScopeX.Core.PowerAnalysis;
    using ScopeX.UserControls.Style;
    using static ScopeX.UserControls.SelectComboBox;

    public partial class PwrRipplePage : UserControl, IPwrOptionView, IStylize
    {
        private Boolean _ArgToCtrl;

        public PwrRipplePage()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer| ControlStyles.AllPaintingInWmPaint| ControlStyles.SupportsTransparentBackColor, true);
            InitComboxList();
        }
        private void InitComboxList()
        {
            var dss = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).
                Select(x => new ComboBoxItem(x.GetDescription(), x)).ToList();
            CbxVoltageSrc.DataSource = dss;
            //selectTouch1.SelectValue = PowerPresenter.VoltageSrc;
            //selectTouch1.Text = PowerPresenter.VoltageSrc.ToString();

            CbxVoltageSrc.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    PowerPresenter.VoltageSrc1 = (ChannelId)CbxVoltageSrc.SelectValue;
                }
            };

            CbxCurrentSrc.DataSource = dss;
            //selectTouch2.SelectValue = PowerPresenter.CurrentSrc;
            //selectTouch2.Text = PowerPresenter.CurrentSrc.ToString();

            CbxCurrentSrc.SelectedIndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    PowerPresenter.CurrentSrc1 = (ChannelId)CbxCurrentSrc.SelectValue;
                }
            };
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
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        public PowerAnalysisOpt Mode => PowerAnalysisOpt.Ripple;

        public PwrRipplePrsnt Presenter { get; set; }

        IPwrOptionPrsnt IView<IPwrOptionPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (PwrRipplePrsnt)value;
        }
        public PowerAnalysisPrsnt PowerPresenter
        {
            get;
            set;
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

        public void Update(Object prsnt, String propertyName)
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
                    ChkActive.Checked = PowerPresenter.Active;
                    break;
                case nameof(PowerPresenter.VoltageSrc1):
                    CbxVoltageSrc.SelectValue = PowerPresenter.VoltageSrc1;
                    break;
                case nameof(PowerPresenter.CurrentSrc1):
                    CbxCurrentSrc.SelectValue = PowerPresenter.CurrentSrc1;
                    break;
                case nameof(Presenter.Source):
                    RdoRippleSrc.ChoosedButtonIndex = (Int32)Presenter.Source;
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                ChkActive.Checked = PowerPresenter.Active;
                CbxVoltageSrc.SelectValue = PowerPresenter.VoltageSrc1;
                CbxCurrentSrc.SelectValue = PowerPresenter.CurrentSrc1;
                RdoRippleSrc.ChoosedButtonIndex = (Int32)Presenter.Source;
                _ArgToCtrl = false;
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
        }
        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }
        private void BtnResetStatistic_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Reset();
            }
        }

        private void RdoRippleSrc_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Source = (VIType)RdoRippleSrc.ChoosedButtonIndex;
            }
        }

        private void BtnResultTable_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.Default.ShowDataTableForm(PowerPresenter);
        }

        private void BtnGuide_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.TryShowRippleGuideForm();
        }

        private void ChkActive_CheckedChangedEvent(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                PowerPresenter.Active = ChkActive.Checked;
            }
        }
    }
}

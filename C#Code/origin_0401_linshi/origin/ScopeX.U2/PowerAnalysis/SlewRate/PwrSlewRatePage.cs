using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;
using ScopeX.UserControls.Style;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static ScopeX.UserControls.SelectComboBox;

namespace ScopeX.U2
{
    public partial class PwrSlewRatePage : UserControl, IPwrOptionView, IStylize
    {
        private Boolean _ArgToCtrl;
        public PwrSlewRatePage()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.SupportsTransparentBackColor, true);
            InitComboxList();
        }

        private void InitComboxList()
        {
            var dss = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).
                Select(x => new ComboBoxItem(x.GetDescription(), x)).ToList();
            CbxVoltageSrc.DataSource = dss;

            CbxVoltageSrc.SelectedIndexChanged -= CbxVoltageSrc_SelectedIndexChanged;
            CbxVoltageSrc.SelectedIndexChanged += CbxVoltageSrc_SelectedIndexChanged;

            CbxCurrentSrc.DataSource = dss;

            CbxCurrentSrc.SelectedIndexChanged -= CbxCurrentSrc_SelectedIndexChanged;
            CbxCurrentSrc.SelectedIndexChanged += CbxCurrentSrc_SelectedIndexChanged;
        }

        private void CbxVoltageSrc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                PowerPresenter.VoltageSrc1 = (ChannelId)CbxVoltageSrc.SelectValue;
            }
        }

        private void CbxCurrentSrc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                PowerPresenter.CurrentSrc1 = (ChannelId)CbxCurrentSrc.SelectValue;
            }
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;
        public PowerAnalysisPrsnt PowerPresenter
        {
            get;
            set;
        }
        public PwrSlewRatePrsnt Presenter
        {
            get;
            set;
        }
        IPwrOptionPrsnt IView<IPwrOptionPrsnt>.Presenter
        {
            get => (IPwrOptionPrsnt)Presenter;
            set
            {
                Presenter = (PwrSlewRatePrsnt)value;
            }
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

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public PowerAnalysisOpt Mode => PowerAnalysisOpt.TurnOnOff;


        protected void Update(Object prsnt, String propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
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
                default:
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
                _ArgToCtrl = false;
            }
        }

        private String FreqToString(Double rds) => (rds).ToString("0.###Ω");
        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
            Presenter.VuTryAddDvdtUI = TryAddVoltageRate;
            Presenter.VuTryAddDidtUI = TryAddCurrentRate;
            UpdateView();
            this.Refresh();
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        private void BtnShowVoltageRate_Click(object sender, EventArgs e)
        {
            PowerPresenter.BoundMathPrsnt1.Active = true;
            DsoPrsnt.FocusId = PowerPresenter.BoundMathPrsnt1.Id;
            var fig = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetFigure(PowerPresenter.BoundMathPrsnt1.Id);
            if (fig is BaseDisplayForm form)
            {
                form.Activate();
            }

        }
        private void TryAddVoltageRate()
        {
            PowerPresenter.BoundMathPrsnt1.Active = true;
            DsoPrsnt.FocusId = PowerPresenter.BoundMathPrsnt1.Id;
            var fig = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetFigure(PowerPresenter.BoundMathPrsnt1.Id);
            if (fig is BaseDisplayForm form)
            {
                form.Activate();
            }
        }
        private void BtnShowCurrentRate_Click(object sender, EventArgs e)
        {
            PowerPresenter.BoundMathPrsnt2.Active = true;
            DsoPrsnt.FocusId = PowerPresenter.BoundMathPrsnt2.Id;
            var fig = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetFigure(PowerPresenter.BoundMathPrsnt2.Id);
            if (fig is BaseDisplayForm form)
            {
                form.Activate();
            }

        }
        private void TryAddCurrentRate()
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
            PowerAnalysisApp.TryShowSlewRateGuideForm();
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
    }
}

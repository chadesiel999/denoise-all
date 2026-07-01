using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using static ScopeX.UserControls.SelectComboBox;
using ScopeX.Controls.Common.Helper;

namespace ScopeX.U2
{
    public partial class PwrSwitchingLossPage : UserControl, IPwrOptionView, IStylize
    {
        private Boolean _ArgToCtrl;
        public PwrSwitchingLossPage()
        {
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer| ControlStyles.AllPaintingInWmPaint| ControlStyles.SupportsTransparentBackColor, true);
            InitComboxList();
        }

        private void InitComboxList()
        {
            var dss = Enum.GetValues<ChannelId>().Where(x => x.IsAnalog()).
                Select(x => new ComboBoxItem(x.GetDescription(), x)).ToList();
            CbxVoltageSrc.DataSource =dss;
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
        public PowerAnalysisPrsnt PowerPresenter
        {
            get;
            set;
        }
        public PwrSwitchingLossPrsnt Presenter
        {
            get;
            set;
        }
        IPwrOptionPrsnt IView<IPwrOptionPrsnt>.Presenter
        {
            get => (IPwrOptionPrsnt)Presenter;
            set
            {
                Presenter = (PwrSwitchingLossPrsnt)value;
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

        public PowerAnalysisOpt Mode => PowerAnalysisOpt.Harmonic;


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
                case nameof(Presenter.RdsOn):
                    NebRdsOn.UpdateValueString();
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

                ControlsHotKnob.Default.InitHotKnob(NebRdsOn);
                NebRdsOn.EditValueOnceClicked += (_, _) => 
                {
                    ControlsHotKnob.Default.SetHotKnob(Presenter, NebRdsOn, nameof(Presenter.RdsOn));
                };
                NebRdsOn.StringFormatFunc = (value) => FreqToString(Presenter.RdsOn);
                NebRdsOn.AddClicked = (a, b) => Presenter.RdsOn += 0.001;
                NebRdsOn.SubClicked = (a, b) => Presenter.RdsOn -= 0.001;
                NebRdsOn.EditValueChicked = (a, b) =>
                {
                    var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, NebRdsOn);
                    var onokclickeventaction = new Action<Double>((data) =>
                        Presenter.RdsOn = Math.Round(data,3, MidpointRounding.AwayFromZero));

                    nkf.SetKeyBoardValue(NebRdsOn.Text, "", 3, onokclickeventaction,
                        Presenter.RdsOn,
                        Presenter.MaxRdsOn,
                        Presenter.MinRdsOn);

                    DialogResult dialogresult = nkf.ShowDialogByPosition();
                };

                NebRdsOn.UpdateValueString();
                _ArgToCtrl = false;
            }
        }

        private String FreqToString(Double rds) => (rds).ToString("0.###Ω");
        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
            UpdateView();
            this.Refresh();
        }

        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
        }

        private void BtnGuide_Click(object sender, EventArgs e)
        {
            PowerAnalysisApp.TryShowSwitchingLossGuideForm();
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
        private void BtnPowerPic_Click(object sender, EventArgs e)
        {
            PowerPresenter.BoundMathPrsnt1.Active = true;
            DsoPrsnt.FocusId = PowerPresenter.BoundMathPrsnt1.Id;
            var fig = (Program.Oscilloscope.View as DsoForm).MultiWindowManager.GetFigure(PowerPresenter.BoundMathPrsnt1.Id);
            if (fig is BaseDisplayForm form)
            {
                form.Activate();
            }
        }
    }
}

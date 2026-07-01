using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core.Jitter;


namespace ScopeX.U2
{
    public partial class PllPage : UserControl, IJitterView, IStylize
    {
        public PllPage()
        {
            InitializeComponent();
            InitHotKnobValue();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            Presenter?.TryRemoveView(this);
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = true;

        public JitterPrsnt Presenter
        {
            get;//=> (JitterPrsnt)(ParentForm as IJitterView).Presenter;
            set;// => (ParentForm as IJitterView).Presenter = value;
        }
        IJitterPrsnt IView<IJitterPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (JitterPrsnt)value;
        }
        private Boolean _ArgToCtrl;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        private void RdoPllTypeChange()
        {
            if (RdoPllType.ChoosedButtonIndex == 0)
            {
                LblCutOffFreq.Visible = true;
                BtnCutOffFreq.Visible = true;
                LblCutoffDivisor.Visible = true;
                BtnCutoffDivisor.Visible = true;

                LblDamping.Visible = false;
                BtnDamping.Visible = false;
                LblNaturalFreq.Visible = false;
                BtnNaturalFreq.Visible = false;
            }
            else
            {
                LblCutOffFreq.Visible = false;
                BtnCutOffFreq.Visible = false;
                LblCutoffDivisor.Visible = false;
                BtnCutoffDivisor.Visible = false;

                LblDamping.Visible = true;
                BtnDamping.Visible = true;
                LblNaturalFreq.Visible = true;
                BtnNaturalFreq.Visible = true;
            }
        }
        private void InitHotKnobValue()
        {
            ControlsHotKnob.Default.InitHotKnob(BtnDamping);
            BtnDamping.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnDamping, nameof(Presenter.DamplingFactor));

            };
            ControlsHotKnob.Default.InitHotKnob(BtnNaturalFreq);
            BtnNaturalFreq.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnNaturalFreq, nameof(Presenter.NaturalFreq));

            };
            ControlsHotKnob.Default.InitHotKnob(BtnCutoffDivisor);
            BtnCutoffDivisor.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnCutoffDivisor, nameof(Presenter.CutoffDivisor));

            };
            ControlsHotKnob.Default.InitHotKnob(BtnCutOffFreq);
            BtnCutOffFreq.Click += (_, _) =>
            {
                ControlsHotKnob.Default.SetHotKnob(Presenter, BtnCutOffFreq, nameof(Presenter.CutoffFreq1));

            };
        }
        private void RdoPllType_IndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.PllType = RdoPllType.ChoosedButtonIndex == 0 ? PllTypeOpt.Golden : PllTypeOpt.SecondOrder;
            }
        }
        public override void Refresh()
        {
            base.Refresh();
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
                case nameof(Presenter.PllType):
                    RdoPllType.ChoosedButtonIndex = Presenter.PllType == PllTypeOpt.Golden ? 0 : 1;
                    RdoPllTypeChange();
                    break;
                case nameof(Presenter.DamplingFactor):
                    BtnDamping.Text = GetDamplingFactor();
                    break;
                case nameof(Presenter.NaturalFreq):
                    BtnNaturalFreq.Text = GetNaturalFreq();
                    break;
                case nameof(Presenter.CutoffFreq1):
                    BtnCutOffFreq.Text = GetCutoffFreq();
                    break;
                case nameof(Presenter.CutoffDivisor):
                    BtnCutoffDivisor.Text = GetCutoffDivisor();
                    break;
            }
            _ArgToCtrl = false;
        }

        private String GetDamplingFactor() => Presenter.DamplingFactor.ToString();// new Quantity(Presenter.DamplingFactor, Prefix.Empty, "").ToString("#0.000", true);
        private String GetNaturalFreq() => new Quantity(Presenter.NaturalFreq, Prefix.Empty, QuantityUnit.Hertz).ToString("#0.###", true);

        private String GetCutoffFreq() => new Quantity(Presenter.CutoffFreq1, Prefix.Empty, QuantityUnit.Hertz).ToString("#0.###", true);

        private String GetCutoffDivisor() => Presenter.CutoffDivisor.ToString();// new Quantity(Presenter.CutoffDivisor, Prefix.Empty, "").ToString("#0.0", true);

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                BtnCutOffFreq.Text = Presenter.CutoffFreq1.ToString();
                BtnCutoffDivisor.Text = Presenter.CutoffDivisor.ToString();
                BtnDamping.Text = GetDamplingFactor();
                BtnNaturalFreq.Text = GetNaturalFreq();
                BtnCutOffFreq.Text = GetCutoffFreq();
                BtnCutoffDivisor.Text = GetCutoffDivisor();
                RdoPllType.ChoosedButtonIndex = Presenter.PllType == PllTypeOpt.Golden ? 0 : 1;
                RdoPllTypeChange();
                _ArgToCtrl = false;
            }
        }

        private void BtnDamping_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnDamping);
            var oncomfirm = new Action<Double>((data) => Presenter.DamplingFactor = data);

            nkf.SetKeyBoardValue(LblDamping.Text, "", 3, oncomfirm,
                Presenter.DamplingFactor, Presenter.MaxDamplingFactor, Presenter.MinDamplingFactor);

            nkf.ShowDialogByPosition();
        }

        private void BtnNaturalFreq_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnNaturalFreq);
            var oncomfirm = new Action<Double>((data) => Presenter.NaturalFreq = data);

            nkf.SetKeyBoardValue(LblNaturalFreq.Text, "", 3, oncomfirm,
                Presenter.NaturalFreq, Presenter.MaxNaturalFreq, Presenter.MinNaturalFreq);

            nkf.ShowDialogByPosition();
        }

        private void BtnCutOffFreq_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnCutOffFreq);
            var oncomfirm = new Action<Double>((data) => Presenter.CutoffFreq1 = data);

            nkf.SetKeyBoardValue(LblCutOffFreq.Text, QuantityUnit.Hertz.ToUnitString(), 10, oncomfirm,
                Presenter.CutoffFreq1, Presenter.MaxCutoffFreq, Presenter.MinCutoffFreq);
            nkf.ShowDialogByPosition();
        }

        private void BtnCutoffDivisor_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this, BtnCutoffDivisor);

            var oncomfirm = new Action<Double>((data) => Presenter.CutoffDivisor = data);

            nkf.SetKeyBoardValue(LblCutoffDivisor.Text, "", 7, oncomfirm,
                Presenter.CutoffDivisor, Presenter.MaxCutDivisor, Presenter.MinCutDivisor);

            nkf.ShowDialogByPosition();
        }
    }
}

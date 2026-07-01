using ScottPlot;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Core;
using ScopeX.Controls.Common.Helper;

namespace ScopeX.U2
{
    public partial class GeneralDigitalFreqBandWidthPage : UserControl, IStylize, IVsaGenerateDigtalView
    {
        private Boolean _ArgToCtrl;
        public GeneralDigitalFreqBandWidthPage()
        {
            InitializeComponent();
        }

        public GenerateDigtalPrsnt Presenter
        {
            get;
            set;
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        IVsaVsaGenerateDigtalPrsnt IView<IVsaVsaGenerateDigtalPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (GenerateDigtalPrsnt)value;
        }

        public void UpdateView(object presenter, string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                UpdateView();
                return;
            }

            _ArgToCtrl = true;

            switch (propertyName)
            {
                case nameof(Presenter.SymbolRate):
                    BtnCarryFreq.Text = CarryFreqToString();
                    break;
                case nameof(Presenter.BandWidth):
                    BtnCarryFreq.Text = BandwidthToString();
                    break;
                case nameof(Presenter.CarryFreqError):
                    BtnCarryFreqErr.Text = CarryFreqErrToString();
                    break; 

            }

            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                BtnCarryFreq.Text = CarryFreqToString();
                BtnBandwidth.Text = BandwidthToString();
                BtnCarryFreqErr.Text = CarryFreqErrToString();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        private String CarryFreqToString() => new Quantity(Presenter.CarryFreq, Prefix.Empty, "Hz").ToString("##0.######", true);

        private void BtnCarryFreq_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => Presenter.CarryFreq = data);

            nkf.SetKeyBoardValue(LblCarryFreq.Text, "Hz", 3, oncomfirm,
                Presenter.CarryFreq, Presenter.CarryFreqMax, Presenter.CarryFreqMin);

            nkf.ShowDialogByPosition();
        }

        private String BandwidthToString() => new Quantity(Presenter.BandWidth, Prefix.Empty, "Hz").ToString("##0.######", true);
        private void BtnBandwidth_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => Presenter.BandWidth = data);

            nkf.SetKeyBoardValue(LblBandwidth.Text, "Hz", 3, oncomfirm,
                Presenter.BandWidth, Presenter.BandWidthMax, Presenter.BandWidthMin);

            nkf.ShowDialogByPosition();
        }

        private String CarryFreqErrToString() => new Quantity(Presenter.CarryFreqError, Prefix.Empty, "Hz").ToString("##0.######", true);
        private void BtnCarryFreqErr_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => Presenter.CarryFreqError = data);

            nkf.SetKeyBoardValue(LblBandwidth.Text, "Hz", 3, oncomfirm,
                Presenter.CarryFreqError, Presenter.CarryFreqErrorMax, Presenter.CarryFreqErrorMin);

            nkf.ShowDialogByPosition();
        }

        private void BtnFreqDetec_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.FreqDetect = ChkFreqDetec.Checked;
            }
        }
    }
}

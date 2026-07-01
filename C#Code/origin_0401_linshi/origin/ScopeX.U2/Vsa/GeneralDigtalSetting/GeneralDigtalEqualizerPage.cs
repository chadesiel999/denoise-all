using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.UserControls.Style;
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
//using static ScopeX.Core.Vsa.GenerateDigtalModel;

namespace ScopeX.U2
{
    public partial class GeneralDigtalEqualizerPage : UserControl, IStylize, IVsaGenerateDigtalView
    {
        private Boolean _ArgToCtrl;
        public GeneralDigtalEqualizerPage()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        public GenerateDigtalPrsnt Presenter
        {
            get;
            set;
        }
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
                    BtnConvergenceCoefficient.Text = ConvergenceCoefficientToString();
                    break;
                case nameof(Presenter.FilterCofficientCnt):
                    BtnFilterCoefficient.Text = FilterCoefficientToString();
                    break;
                case nameof(Presenter.OverSample):
                    CbxOversampleRate.SelectedIndex = (Int32)Presenter.OverSample;
                    break;
                case nameof(Presenter.EqualizeMode):
                    CbxMode.SelectedIndex = (Int32)Presenter.EqualizeMode;
                    break;
                case nameof(Presenter.SymbolLength):
                    BtnSymbolLength.Text = SymbolLengthToString();
                    break;

            }

            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;

                BtnConvergenceCoefficient.Text = ConvergenceCoefficientToString();
                BtnFilterCoefficient.Text = FilterCoefficientToString();
                BtnSymbolLength.Text = SymbolLengthToString();

                _ArgToCtrl = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        private void ChkEnabled_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.EqualizerEnabled = ChkEnabled.Checked;
            }
        }

        private void CbxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.EqualizeMode = (VsaEqualizeMode)CbxMode.SelectedIndex;
            }
        }

        private String ConvergenceCoefficientToString() => new Quantity(Presenter.ConvergenceCoefficient, Prefix.Empty, "").ToString("##0.######", true);
        private void BtnConvergenceCoefficient_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => Presenter.ConvergenceCoefficient = data);

            nkf.SetKeyBoardValue(LblConvergenceCoefficient.Text, "", 3, oncomfirm,
                Presenter.ConvergenceCoefficient, Presenter.ConvergenceCoefficientMax, Presenter.ConvergenceCoefficientMin);

            nkf.ShowDialogByPosition();
        }

        private String FilterCoefficientToString() => new Quantity(Presenter.FilterCofficientCnt, Prefix.Empty, "").ToString("######", true);
        private void BtnFilterCoefficient_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => Presenter.FilterCofficientCnt = (Int32)data);

            nkf.SetKeyBoardValue(LblConvergenceCoefficient.Text, "", 3, oncomfirm,
                Presenter.FilterCofficientCnt, Presenter.FilterCofficientCntMax, Presenter.FilterCofficientCntMin);

            nkf.ShowDialogByPosition();
        }

        private void CbxOversampleRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.OverSample = (EqualizeOverSampling)CbxOversampleRate.SelectedIndex;
            }
        }

        private void ChkReset_Click(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.EqualizerReset = ChkReset.Checked;
            }
        }


        private String SymbolLengthToString() => new Quantity(Presenter.SymbolLength, Prefix.Empty, "").ToString("######", true);
        private void BtnSymbolLength_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => Presenter.SymbolLength = (Int32)data);

            nkf.SetKeyBoardValue(LblConvergenceCoefficient.Text, "", 3, oncomfirm,
                Presenter.SymbolLength, Presenter.SymbolLengthMax, Presenter.SymbolLengthMin);

            nkf.ShowDialogByPosition();
        }
    }
}

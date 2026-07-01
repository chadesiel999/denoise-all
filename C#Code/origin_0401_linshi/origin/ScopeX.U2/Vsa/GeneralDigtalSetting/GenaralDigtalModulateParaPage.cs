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
using ScopeX.Controls.Common.Default;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class GenaralDigtalModulateParaPage : UserControl, IStylize, IVsaGenerateDigtalView
    {
        private Boolean _ArgToCtrl;
        public GenaralDigtalModulateParaPage()
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

        private void CbxFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
                Presenter.FormatOpt = (VsaFormatOpt)CbxFormat.SelectedIndex;
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
                case nameof(Presenter.FormatOpt):
                    CbxFormat.SelectedIndex = (Int32)Presenter.FormatOpt;
                    break;
                case nameof(Presenter.SymbolRate):
                    BtnSymbolRate.Text = SymbolRateToString();
                    break;
                case nameof(Presenter.FilterPara):
                    BtnFilterPara.Text = FilterParaToString();
                    break;
                case nameof(Presenter.MeasureFilterType):
                    CbxMeasureFilter.SelectedIndex = (Int32)Presenter.MeasureFilterType;
                    break;
                case nameof(Presenter.RefFilterType):
                    CbxRefFilter.SelectedIndex = (Int32)Presenter.RefFilterType;
                    break;
                case nameof(Presenter.RollOffFactor):
                    BtnRollOffFactor.Text = RollOffFactorToString();
                    break;


            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                CbxFormat.SelectedIndex = (Int32)Presenter.FormatOpt;
                BtnSymbolRate.Text = SymbolRateToString();
                BtnFilterPara.Text = FilterParaToString();
                BtnRollOffFactor.Text = RollOffFactorToString();
                CbxMeasureFilter.SelectedIndex = (Int32)Presenter.MeasureFilterType;
                CbxRefFilter.SelectedIndex = (Int32)Presenter.RefFilterType;
                _ArgToCtrl = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        private String SymbolRateToString() => new Quantity(Presenter.SymbolRate, Prefix.Empty, "Baud").ToString("##0.######", true);

        private void BtnSymbolRate_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => Presenter.SymbolRate = data);

            nkf.SetKeyBoardValue(LblSymbolRate.Text, "Baud", 3, oncomfirm,
                Presenter.SymbolRate, Presenter.SymbolRateMax, Presenter.SymbolRateMin);

            nkf.ShowDialogByPosition();
        }

        private String FilterParaToString() => new Quantity(Presenter.FilterPara, Prefix.Empty, "").ToString("##0.######", true);

        private void BtnFilterPara_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => Presenter.FilterPara = data);

            nkf.SetKeyBoardValue(LblFilterPara.Text, "", 3, oncomfirm,
                Presenter.FilterPara, Presenter.FilterParaMax, Presenter.FilterParaMin);

            nkf.ShowDialogByPosition();
        }

        private void CbxMeasureFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
                Presenter.MeasureFilterType = (VsaMeasureFilterTypeOpt)CbxMeasureFilter.SelectedIndex;
        }

        private void CbxRefFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.RefFilterType = (VsaRefFilterTypeOpt)CbxRefFilter.SelectedIndex;
            }

        }
        private String RollOffFactorToString() => new Quantity(Presenter.RollOffFactor, Prefix.Empty, "").ToString("##0.######", true);
        private void BtnRollOffFactor_Click(object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => Presenter.RollOffFactor = data);

            nkf.SetKeyBoardValue(LblRollOffFactor.Text, "", 3, oncomfirm,
                Presenter.RollOffFactor, Presenter.RollOffFactorMax, Presenter.RollOffFactorMin);

            nkf.ShowDialogByPosition();
        }
    }
}

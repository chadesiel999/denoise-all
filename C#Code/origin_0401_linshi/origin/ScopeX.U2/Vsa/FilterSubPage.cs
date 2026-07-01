using System;
using System.Windows.Forms;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;

namespace ScopeX.U2
{
    public partial class FilterSubPage : UserControl, IVsaView
    {
        private Boolean _ArgToCtrl = false;

        public FilterSubPage(VectorAnalysisPrsnt vap, FilterNodePrsnt fnp)
        {
            InitializeComponent();
            Presenter = vap;
            FilterPrsnt = fnp;
        }

        public VectorAnalysisPrsnt Presenter
        {
            get;
            set;
        }

        public FilterNodePrsnt FilterPrsnt
        {
            get;
        }

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

        IVsaPrsnt IView<IVsaPrsnt>.Presenter { get => Presenter; set => Presenter = (VectorAnalysisPrsnt)value; }

        private void ChangeOptionState()
        {
            LblRolloffFactor.Visible = BtnRolloffFactor.Visible = FilterPrsnt.MeasureFilterType == ComModel.VsaMeasureFilterTypeOpt.RaisedCosine || FilterPrsnt.MeasureFilterType == ComModel.VsaMeasureFilterTypeOpt.RootRaisedCosine;

            LblOrder.Visible = BtnOrder.Visible = FilterPrsnt.MeasureFilterType == ComModel.VsaMeasureFilterTypeOpt.RaisedCosine || FilterPrsnt.MeasureFilterType == ComModel.VsaMeasureFilterTypeOpt.Gaussian;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ChangeOptionState();
            UpdateView();
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        public void UpdateView(Object prsnt, String propertyName)
        {
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(FilterPrsnt.MeasureFilterType):
                    CbxFilterType.SelectedIndex = (Int32)FilterPrsnt.MeasureFilterType;
                    ChangeOptionState();
                    break;
                case nameof(FilterPrsnt.CenterFreq):
                    BtnCenterFreq.Text = CenterFreqToString();
                    break;
                case nameof(FilterPrsnt.Bandwidth):
                    BtnSpan.Text = SpanToString();
                    break;
                case nameof(FilterPrsnt.Order):
                    BtnOrder.Text = OrderToString();
                    break;
                case nameof(FilterPrsnt.RolloffFactor):
                    BtnRolloffFactor.Text = RolloffToString();
                    break;
                default:
                    UpdateView();
                    break;
            }
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                CbxFilterType.SelectedIndex = (Int32)FilterPrsnt.MeasureFilterType;
                BtnCenterFreq.Text = CenterFreqToString();
                BtnSpan.Text = SpanToString();
                BtnOrder.Text = OrderToString();
                BtnRolloffFactor.Text = RolloffToString();
                _ArgToCtrl = false;
            }
        }

        private String CenterFreqToString() => new Quantity(FilterPrsnt.CenterFreq, Prefix.Empty, QuantityUnit.Hertz).ToString("##0.######", true);

        private String SpanToString() => new Quantity(FilterPrsnt.Bandwidth, Prefix.Empty, QuantityUnit.Hertz).ToString("##0.######", true);

        private String OrderToString() => new Quantity(FilterPrsnt.Order, Prefix.Empty, QuantityUnit.Constant).ToString("##0", false);

        private String RolloffToString() => new Quantity(FilterPrsnt.RolloffFactor, Prefix.Empty, QuantityUnit.Constant).ToString("##0.###", false);

        private void CbxFilterType_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                FilterPrsnt.MeasureFilterType = (ComModel.VsaMeasureFilterTypeOpt)CbxFilterType.SelectedIndex;
            }
        }

        private void BtnCenterFreq_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => FilterPrsnt.CenterFreq = data);

            nkf.SetKeyBoardValue(LblCenterFreq.Text, QuantityUnit.Hertz.ToUnitString(), 3, oncomfirm,
                FilterPrsnt.CenterFreq, FilterPrsnt.MaxCenterFreq, FilterPrsnt.MinCenterFreq);

            nkf.ShowDialogByPosition();
        }

        private void BtnSpan_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => FilterPrsnt.Bandwidth = data);

            nkf.SetKeyBoardValue(LblSpan.Text, QuantityUnit.Hertz.ToUnitString(), 3, oncomfirm,
                FilterPrsnt.Bandwidth, FilterPrsnt.MaxBandwidth, FilterPrsnt.MinBandwidth);

            nkf.ShowDialogByPosition();
        }

        private void BtnOrder_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => FilterPrsnt.Order = (Int32)data);

            nkf.SetKeyBoardValue(LblOrder.Text, "", 0, oncomfirm,
                FilterPrsnt.Order, FilterPrsnt.MaxOrder, FilterPrsnt.MinOrder);

            nkf.ShowDialogByPosition();
        }

        private void BtnRolloffFactor_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => FilterPrsnt.RolloffFactor = data);

            nkf.SetKeyBoardValue(LblRolloffFactor.Text, "", 0, oncomfirm,
                FilterPrsnt.RolloffFactor, FilterPrsnt.MaxRolloffFactor, FilterPrsnt.MinRolloffFactor);

            nkf.ShowDialogByPosition();
        }
    }
}

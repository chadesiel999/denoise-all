using System;
using System.Windows.Forms;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;

namespace ScopeX.U2
{
    public partial class MixerSubPage : UserControl, IVsaView
    {
        public MixerSubPage(VectorAnalysisPrsnt vap, MixerNodePrsnt mnp)
        {
            InitializeComponent();
            Presenter = vap;
            MixerPrsnt = mnp;
        }

        public VectorAnalysisPrsnt Presenter
        {
            get;
            set;
        }

        public MixerNodePrsnt MixerPrsnt
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

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateView();
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        public void UpdateView(Object prsnt, String propertyName) => BtnCarrierFreq.Text = CarrierFreqToString();

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                BtnCarrierFreq.Text = CarrierFreqToString();
            }
        }

        private String CarrierFreqToString() => new Quantity(MixerPrsnt.SuggestedFreq, Prefix.Empty, QuantityUnit.Hertz).ToString("##0.######", true);

        private void BtnCarrierFreq_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => MixerPrsnt.SuggestedFreq = data);

            nkf.SetKeyBoardValue(LblCarrierFreq.Text, QuantityUnit.Hertz.ToUnitString(), 3, oncomfirm,
                MixerPrsnt.SuggestedFreq, MixerPrsnt.MaxSuggestedFreq, MixerPrsnt.MinSuggestedFreq);

            nkf.ShowDialogByPosition();
        }
    }
}

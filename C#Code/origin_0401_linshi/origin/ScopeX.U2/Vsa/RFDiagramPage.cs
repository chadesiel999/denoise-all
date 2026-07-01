using System;
using System.Windows.Forms;
using ScopeX.Core;

namespace ScopeX.U2
{
    public partial class RFDiagramPage : UserControl, IVsaView
    {
        private Control _OptionSubPage;

        public RFDiagramPage(VectorAnalysisPrsnt prsnt)
        {
            InitializeComponent();
            Presenter = prsnt;
        }

        public VectorAnalysisPrsnt Presenter
        {
            get;
            set;
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

        public void UpdateView(Object prsnt, String propertyName)
        {
            (_OptionSubPage as IVsaView)?.UpdateView(prsnt, propertyName);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadOptionPage(0);
        }

        private Control GetOptionPage(Int32 index) => index switch
        {
            0 => new MixerSubPage(Presenter, (MixerNodePrsnt)Presenter.GetNode(0)),
            1 => new FilterSubPage(Presenter, (FilterNodePrsnt)Presenter.GetNode(1)),
            3 => new EqualizerSubPage(Presenter, (EqualizerNodePrsnt)Presenter.GetNode(3)),
            4 => new PhaseEstSubPage(Presenter, (PhaseEstNodePrsnt)Presenter.GetNode(4)),
            _ => null,
        };

        private void LoadOptionPage(Int32 index)
        {
            if (TlpRFCfg.Controls.Contains(_OptionSubPage))
            {
                TlpRFCfg.Controls.Remove(_OptionSubPage);
                _OptionSubPage.Dispose();
            }

            _OptionSubPage = GetOptionPage(index);
            if (_OptionSubPage is not null)
            {
                _OptionSubPage.Dock = DockStyle.Fill;
                TlpRFCfg.Controls.Add(_OptionSubPage, 0, 1);
            }
        }

        private void BtnMixer_Click(Object sender, EventArgs e) => LoadOptionPage(0);

        private void BtnFilter_Click(Object sender, EventArgs e) => LoadOptionPage(1);

        private void BtnCarrierEst_Click(Object sender, EventArgs e) => LoadOptionPage(2);

        private void BtnEqualizer_Click(Object sender, EventArgs e) => LoadOptionPage(3);

        private void BtnPhaseEst_Click(Object sender, EventArgs e) => LoadOptionPage(4);


    }
}

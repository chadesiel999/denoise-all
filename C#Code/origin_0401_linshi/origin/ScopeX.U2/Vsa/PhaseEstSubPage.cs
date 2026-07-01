using System;
using System.Windows.Forms;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;

namespace ScopeX.U2
{
    public partial class PhaseEstSubPage : UserControl, IVsaView
    {
        private Boolean _ArgToCtrl = false;

        public PhaseEstSubPage(VectorAnalysisPrsnt vap, PhaseEstNodePrsnt penp)
        {
            InitializeComponent();
            Presenter = vap;
            PhaseEstPrsnt = penp;
        }

        public VectorAnalysisPrsnt Presenter
        {
            get;
            set;
        }

        public PhaseEstNodePrsnt PhaseEstPrsnt
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

        public void UpdateView(Object prsnt, String propertyName)
        {
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case nameof(PhaseEstPrsnt.PhaseEst):
                    CbxPhaseEstType.SelectedIndex = (Int32)PhaseEstPrsnt.PhaseEst;
                    break;
                case nameof(PhaseEstPrsnt.SymLength):
                    BtnSymLength.Text = SymLengthToString();
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
                CbxPhaseEstType.SelectedIndex = (Int32)PhaseEstPrsnt.PhaseEst;
                BtnSymLength.Text = SymLengthToString();
                _ArgToCtrl = false;
            }
        }

        private String SymLengthToString() => PhaseEstPrsnt.SymLength.ToString("D1");

        private void CbxPhaseEstType_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                PhaseEstPrsnt.PhaseEst = (ComModel.VsaPhaseEstOpt)CbxPhaseEstType.SelectedIndex;
            }
        }

        private void BtnSymLength_Click(Object sender, EventArgs e)
        {
            var nkf = new NumberKeybordForm().UniformInitKeyBoard(this);
            var oncomfirm = new Action<Double>((data) => PhaseEstPrsnt.SymLength = (Int32)data);

            nkf.SetKeyBoardValue(LblSymLength.Text, "", 0, oncomfirm,
                PhaseEstPrsnt.SymLength, PhaseEstPrsnt.MaxSymLength, PhaseEstPrsnt.MinSymLength);

            nkf.ShowDialogByPosition();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.Core;

namespace ScopeX.U2
{
    public partial class CustomDiagramPage : UserControl, IVsaView
    {
        private Boolean _ArgToCtrl = false;

        private Control _OptionSubPage;

        public CustomDiagramPage(VectorAnalysisPrsnt prsnt)
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
            _ArgToCtrl = true;
            switch (propertyName)
            {
                case "NodeType0":
                    CbxNode1.SelectedIndex = (Int32)Presenter.GetNode(0).NodeType;
                    ChangeOptionPage(0);
                    break;
                case "NodeType1":
                    CbxNode2.SelectedIndex = (Int32)Presenter.GetNode(1).NodeType;
                    ChangeOptionPage(1);
                    break;
                case "NodeType2":
                    CbxNode2.SelectedIndex = (Int32)Presenter.GetNode(2).NodeType;
                    ChangeOptionPage(2);
                    break;
                case "NodeType3":
                    CbxNode4.SelectedIndex = (Int32)Presenter.GetNode(3).NodeType;
                    ChangeOptionPage(3);
                    break;
                case "NodeType4":
                    CbxNode5.SelectedIndex = (Int32)Presenter.GetNode(4).NodeType;
                    ChangeOptionPage(4);
                    break;
                case "NodeType5":
                    CbxNode6.SelectedIndex = (Int32)Presenter.GetNode(5).NodeType;
                    ChangeOptionPage(5);
                    break;
                case "NodeType6":
                    CbxNode7.SelectedIndex = (Int32)Presenter.GetNode(6).NodeType;
                    ChangeOptionPage(6);
                    break;
                case "NodeType7":
                    CbxNode8.SelectedIndex = (Int32)Presenter.GetNode(7).NodeType;
                    ChangeOptionPage(7);
                    break;
                default:
                    (_OptionSubPage as IVsaView)?.UpdateView(prsnt, propertyName);
                    break;
            }

            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                CbxNode1.SelectedIndex = (Int32)Presenter.GetNode(0).NodeType;
                CbxNode2.SelectedIndex = (Int32)Presenter.GetNode(1).NodeType;
                CbxNode3.SelectedIndex = (Int32)Presenter.GetNode(2).NodeType;
                CbxNode4.SelectedIndex = (Int32)Presenter.GetNode(3).NodeType;
                CbxNode5.SelectedIndex = (Int32)Presenter.GetNode(4).NodeType;
                CbxNode6.SelectedIndex = (Int32)Presenter.GetNode(5).NodeType;
                CbxNode7.SelectedIndex = (Int32)Presenter.GetNode(6).NodeType;
                CbxNode8.SelectedIndex = (Int32)Presenter.GetNode(7).NodeType;

                _OptionSubPage?.Refresh();
                _ArgToCtrl = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ChangeOptionPage(0);

            UpdateView();
        }

        private Control GetOptionPage(Int32 index) => Presenter.GetNode(index).NodeType switch
        {
            VsaNodeTypeOpt.Mixer => new MixerSubPage(Presenter, (MixerNodePrsnt)Presenter.GetNode(index)),
            VsaNodeTypeOpt.Filter => new FilterSubPage(Presenter, (FilterNodePrsnt)Presenter.GetNode(index)),
            VsaNodeTypeOpt.Equalizer => new EqualizerSubPage(Presenter, (EqualizerNodePrsnt)Presenter.GetNode(index)),
            VsaNodeTypeOpt.DCBlock => new DCBlockSubPage(Presenter, (DCBlockNodePrsnt)Presenter.GetNode(index)),
            VsaNodeTypeOpt.PhaseEst => new PhaseEstSubPage(Presenter, (PhaseEstNodePrsnt)Presenter.GetNode(index)),
            VsaNodeTypeOpt.Custom => throw new NotImplementedException(),
            _ => null,
        };

        private void ChangeOptionPage(Int32 index)
        {
            if (TlpCustomCfg.Controls.Contains(_OptionSubPage))
            {
                TlpCustomCfg.Controls.Remove(_OptionSubPage);
                _OptionSubPage.Dispose();
            }

            _OptionSubPage = GetOptionPage(index);
            if (_OptionSubPage is not null)
            {
                _OptionSubPage.Dock = DockStyle.Fill;
                TlpCustomCfg.Controls.Add(_OptionSubPage, 0, 1);
            }
        }

        private void CbxNode1_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SetCustomNode(0, (VsaNodeTypeOpt)CbxNode1.SelectedIndex);
                ChangeOptionPage(0);
            }
        }

        private void CbxNode2_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SetCustomNode(1, (VsaNodeTypeOpt)CbxNode2.SelectedIndex);
                ChangeOptionPage(1);
            }
        }

        private void CbxNode3_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SetCustomNode(2, (VsaNodeTypeOpt)CbxNode3.SelectedIndex);
                ChangeOptionPage(2);
            }
        }

        private void CbxNode4_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SetCustomNode(3, (VsaNodeTypeOpt)CbxNode4.SelectedIndex);
                ChangeOptionPage(3);
            }
        }

        private void CbxNode5_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SetCustomNode(4, (VsaNodeTypeOpt)CbxNode5.SelectedIndex);
                ChangeOptionPage(4);
            }
        }

        private void CbxNode6_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SetCustomNode(5, (VsaNodeTypeOpt)CbxNode6.SelectedIndex);
                ChangeOptionPage(5);
            }
        }

        private void CbxNode7_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SetCustomNode(6, (VsaNodeTypeOpt)CbxNode7.SelectedIndex);
                ChangeOptionPage(6);
            }
        }

        private void CbxNode8_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.SetCustomNode(7, (VsaNodeTypeOpt)CbxNode8.SelectedIndex);
                ChangeOptionPage(7);
            }
        }

        private void BtnNode1_Click(Object sender, EventArgs e)
        {
            ChangeOptionPage(0);
        }

        private void BtnNode2_Click(Object sender, EventArgs e)
        {
            ChangeOptionPage(1);
        }

        private void BtnNode3_Click(Object sender, EventArgs e)
        {
            ChangeOptionPage(2);
        }

        private void BtnNode4_Click(Object sender, EventArgs e)
        {
            ChangeOptionPage(3);
        }

        private void BtnNode5_Click(Object sender, EventArgs e)
        {
            ChangeOptionPage(4);
        }

        private void BtnNode6_Click(Object sender, EventArgs e)
        {
            ChangeOptionPage(5);
        }

        private void BtnNode7_Click(Object sender, EventArgs e)
        {
            ChangeOptionPage(6);
        }

        private void BtnNode8_Click(Object sender, EventArgs e)
        {
            ChangeOptionPage(7);
        }
    }
}

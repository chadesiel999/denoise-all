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
    public partial class IQDiagramPage : UserControl, IVsaView
    {
        private Control _OptionSubPage;

        public IQDiagramPage(VectorAnalysisPrsnt prsnt)
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
            0 => new DCBlockSubPage(Presenter, (DCBlockNodePrsnt)Presenter.GetNode(0)),
            1 => new FilterSubPage(Presenter, (FilterNodePrsnt)Presenter.GetNode(1)),
            2 => new EqualizerSubPage(Presenter, (EqualizerNodePrsnt)Presenter.GetNode(2)),
            _ => null,
        };

        private void LoadOptionPage(Int32 index)
        {
            if (TlpIQCfg.Controls.Contains(_OptionSubPage))
            {
                TlpIQCfg.Controls.Remove(_OptionSubPage);
                _OptionSubPage.Dispose();
            }

            _OptionSubPage = GetOptionPage(index);
            if (_OptionSubPage is not null)
            {
                _OptionSubPage.Dock = DockStyle.Fill;
                TlpIQCfg.Controls.Add(_OptionSubPage, 0, 1);
            }
        }

        private void BtnDCBlock_Click(Object sender, EventArgs e)
        {
            LoadOptionPage(0);
        }

        private void BtnFilter_Click(Object sender, EventArgs e)
        {
            LoadOptionPage(1);
        }

        private void BtnEqualizer_Click(Object sender, EventArgs e)
        {
            LoadOptionPage(2);
        }
    }
}

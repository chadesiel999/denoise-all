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
    public partial class DCBlockSubPage : UserControl, IVsaView
    {
        private Boolean _ArgToCtrl = false;

        public DCBlockSubPage(VectorAnalysisPrsnt vap, DCBlockNodePrsnt dbnp)
        {
            InitializeComponent();
            Presenter = vap;
            DCBlockPrsnt = dbnp;
        }

        public VectorAnalysisPrsnt Presenter
        {
            get;
            set;
        }

        public DCBlockNodePrsnt DCBlockPrsnt
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
            RdoDCBlockMode.ChoosedButtonIndex = DCBlockPrsnt.IsAnalogDCBlock ? 0 : 1;
            _ArgToCtrl = false;
        }

        protected void UpdateView()
        {
            if (!DesignMode)
            {
                _ArgToCtrl = true;
                RdoDCBlockMode.ChoosedButtonIndex = DCBlockPrsnt.IsAnalogDCBlock ? 0 : 1;
                _ArgToCtrl = false;
            }
        }

        private void RdoDCBlockMode_IndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                DCBlockPrsnt.IsAnalogDCBlock = RdoDCBlockMode.ChoosedButtonIndex == 0;
            }
        }
    }
}

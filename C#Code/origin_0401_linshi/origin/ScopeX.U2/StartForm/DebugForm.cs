using System;
using System.Windows.Forms;
using ScopeX.Core;
using ScopeX.U2.LanguageSupoort;
using ScopeX.UserControls;

namespace ScopeX.U2
{
    public partial class DebugForm : FloatForm
    {
        public DebugForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            CbxDataSource.SelectedIndex = (Int32)DsoPrsnt.DataSrcOpt;
            // LanguageFactory.CacheFormLanguageControls(this);
        }

        private void CbxDataSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            DsoPrsnt.DataSrcOpt = (DataSourceOpt)CbxDataSource.SelectedIndex;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Escape)
            {
                Close();
                return;
            }
            base.OnKeyPress(e);
        }
    }
}

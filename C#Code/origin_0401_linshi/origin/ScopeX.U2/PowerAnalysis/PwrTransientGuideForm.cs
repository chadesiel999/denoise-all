using System;
using System.Windows.Forms;
using ScopeX.UserControls;

namespace ScopeX.U2
{
    public partial class PwrTransientGuideForm : FloatForm
    {
        public PwrTransientGuideForm()
        {
            InitializeComponent();
            IsShowPin = false;
            this.FixedToolIconInfos[2].IsShow = false;
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

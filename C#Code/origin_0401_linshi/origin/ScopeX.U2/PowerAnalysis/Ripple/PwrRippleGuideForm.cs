using System.Windows.Forms;
using System;
using ScopeX.UserControls;
using ScopeX.UserControls.Style;
using ScopeX.U2.BaseControl;

namespace ScopeX.U2
{
    public partial class PwrRippleGuideForm :FlashBorderForm
    {
        public PwrRippleGuideForm()
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
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            base.OnLoad(e);
        }
        private void Stylize()
        {
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(LblGuide, StyleFlag.FontSize);
        }
    }
}

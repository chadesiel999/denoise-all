using System.Windows.Forms;
using System;
using ScopeX.UserControls;
using ScopeX.U2.BaseControl;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class PwrLoopAnalysisGuideForm :FlashBorderForm
    {
        public PwrLoopAnalysisGuideForm()
        {
            InitializeComponent();
            IsShowPin = false;
            this.FixedToolIconInfos[2].IsShow = false;
            DefaultStyleManager.Instance.RegisterControlRecursion(this, StyleFlag.FontSize);
            DefaultStyleManager.Instance.RegisterControlRecursion(LblGuide, StyleFlag.FontSize);
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
    }
}

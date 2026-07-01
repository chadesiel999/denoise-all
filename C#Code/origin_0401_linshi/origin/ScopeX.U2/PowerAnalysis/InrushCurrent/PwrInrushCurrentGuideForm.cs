using System;
using System.Windows.Forms;
using ScopeX.U2.BaseControl;
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class PwrInrushCurrentGuideForm : FlashBorderForm
    {
        public PwrInrushCurrentGuideForm()
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

        private void PwrQualityGuideForm_Load(object sender, EventArgs e)
        {
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this);
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

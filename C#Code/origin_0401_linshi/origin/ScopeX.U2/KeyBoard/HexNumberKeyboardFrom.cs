using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.UserControls;

namespace ScopeX.U2
{
    public partial class HexNumberKeyboardFrom :FloatForm
    {
        public HexNumberKeyboardFrom()
        {
            InitializeComponent();
            FixedToolIconInfos[2].IsShow = false;
        }
        public ScopeX.UserControls.HexNumberKeyboard NumberKeyboard => Keyboard;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
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
        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            //HeadHeight = this.HeadHeight - 10;
            //ToolIconSize = new Size(ToolIconSize.Width - 10 / 3, ToolIconSize.Width - 10 / 3);
            //BorderThickness = 2;
            //BorderBackColor = this.HeadBackColor;
            //!!!ZQC 11.14
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(Keyboard, ScopeX.UserControls.Style.StyleFlag.FontSize);
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, ScopeX.UserControls.Style.StyleFlag.FontSize);
            NumberKeyboard.Value = NumberKeyboard.Value;

            base.OnLoad(e);
        }
        private void Stylize()
        {
            IsShowHelp = false;
            ScopeXLabel lbl = (ScopeXLabel)Keyboard.GetType().GetField("LblValue", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(Keyboard);
            Single fontsize = lbl.Font.Size;
            var right = lbl.TextAlign;
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(Keyboard, ScopeX.UserControls.Style.StyleFlag.FontSize);
            lbl.Font = new Font(lbl.Font.FontFamily, fontsize);
            lbl.TextAlign = right;
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (Font != null)
            {
                Font = null;
            }
            base.OnFormClosed(e);
        }
    }
}

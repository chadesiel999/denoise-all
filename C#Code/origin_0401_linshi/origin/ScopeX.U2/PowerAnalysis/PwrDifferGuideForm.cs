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
using ScopeX.UserControls.Style;

namespace ScopeX.U2
{
    public partial class PwrDifferGuideForm : FloatForm
    {
        public PwrDifferGuideForm()
        {
            InitializeComponent();
            IsShowPin = false;
            this.FixedToolIconInfos[2].IsShow = false;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DefaultStyleManager.Instance.RegisterControl(this, StyleFlag.FontSize);
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

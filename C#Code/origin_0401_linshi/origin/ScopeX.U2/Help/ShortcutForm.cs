using System;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class ShortcutForm : Form
    {
        public ShortcutForm()
        {
            InitializeComponent();
        }

        private void LvContent_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ShortcutForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keys.Escape == e.KeyCode)
                Close();
        }
    }
}

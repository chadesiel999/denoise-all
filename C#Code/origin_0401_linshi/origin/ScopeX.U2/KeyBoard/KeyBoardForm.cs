using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class KeyBoardForm : Form,IDisposable
    {
        public KeyBoardForm(String title)
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();

            this.Text = title;
        }

        private Boolean _CapsLocked = true;
        private Boolean _Shifted = false;

        public string Content => this.tbxContent.Text;

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnBackspace_Click(object sender, EventArgs e)
        {
            if (this.tbxContent.Text.Length - 1 >= 0)
            {
                this.tbxContent.Text = this.tbxContent.Text.Substring(0, this.tbxContent.Text.Length - 1);
            }
        }

        private void btnKey_Click(object sender, EventArgs e)
        {
            this.tbxContent.Text += (sender as Button).Text;
        }

        private void btnCap_Click(object sender, EventArgs e)
        {
            this._CapsLocked = !this._CapsLocked;
            if (this._CapsLocked)
            {
                this.btnCaps.ForeColor = Color.DarkOrange;
            }
            else
            {
                this.btnCaps.ForeColor = Color.White;
            }
            ChangeKeyLetter();
        }

        private void btnShift_Click(object sender, EventArgs e)
        {
            this._Shifted = !this._Shifted;
            if (this._Shifted)
            {
                this.btnShift.ForeColor = Color.DarkOrange;
            }
            else
            {
                this.btnShift.ForeColor = Color.White;
            }

            ChangeKeyCharTable();
        }

        private void ChangeKeyLetter()
        {
            for (Char letter = 'A'; letter <= 'Z'; letter++)
            {
                Button btn = Controls[@"btn" + letter] as Button;
                if (_CapsLocked)
                {
                    btn.Text = btn.Text.ToUpper();
                }
                else
                {
                    btn.Text = btn.Text.ToLower();
                }
            }
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (Font != null)
            {
                Font = null;
            }
            base.OnFormClosed(e);
            this.Dispose();
        }
        private void ChangeKeyCharTable()
        {
            if (_Shifted)
            {
                this.btnTilde.Text = @"`";
                this.btn1.Text = @"!";
                this.btn2.Text = @"@";
                this.btn3.Text = @"#";
                this.btn4.Text = @"$";
                this.btn5.Text = @"%";
                this.btn6.Text = @"^";
                this.btn7.Text = "&&";
                this.btn8.Text = @"*";
                this.btn9.Text = @"(";
                this.btn0.Text = @")";
                this.btnMinus.Text = @"_";
                this.btnEqua.Text = @"+";
                this.btnLBracket.Text = @"{";
                this.btnRBracket.Text = @"}";
                this.btnBackSlash.Text = @"|";
                this.btnSemicolon.Text = @":";
                this.btnApostrophe.Text = "\"";
                this.btnComma.Text = @"<";
                this.btnPoint.Text = @">";
                this.btnSlash.Text = @"?";
            }
            else
            {
                this.btnTilde.Text = @"~";
                this.btn1.Text = @"1";
                this.btn2.Text = @"2";
                this.btn3.Text = @"3";
                this.btn4.Text = @"4";
                this.btn5.Text = @"5";
                this.btn6.Text = @"6";
                this.btn7.Text = @"7";
                this.btn8.Text = @"8";
                this.btn9.Text = @"9";
                this.btn0.Text = @"0";
                this.btnMinus.Text = @"-";
                this.btnEqua.Text = @"=";
                this.btnLBracket.Text = @"[";
                this.btnRBracket.Text = @"]";
                this.btnBackSlash.Text = @"\";
                this.btnSemicolon.Text = @";";
                this.btnApostrophe.Text = @"'";
                this.btnComma.Text = @",";
                this.btnPoint.Text = @".";
                this.btnSlash.Text = @"/";

            }

        }


    }


}


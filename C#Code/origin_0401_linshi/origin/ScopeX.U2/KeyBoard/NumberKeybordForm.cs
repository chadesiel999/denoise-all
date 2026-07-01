using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.U2.BaseControl;
using ScopeX.UserControls;
using SharpGen.Runtime.Win32;
using Veldrid.Common;
using static ScopeX.U2.NumberKeybordForm;

namespace ScopeX.U2
{
    public enum HookType :int
    {
        WH_MOUSE_LL = 14,
        WH_KEYBOARD_LL = 13,
    }
    public struct POINT
    {
        public int x;
        public int y;
    }

    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }


    public partial class NumberKeybordForm :FlashBorderForm,IDisposable
    {


        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public NumberKeybordForm()
        {
            InitializeComponent();
            FixedToolIconInfos[2].IsShow = false;
        }

        public NumberKeyboard NumberKeyboard => Keyboard;


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            this.Size = new Size(0, 0);
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
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Keyboard.Focus();
        }
        protected override void OnLoad(EventArgs e)
        {
            Stylize();
            //HeadHeight = this.HeadHeight - 10;
            //ToolIconSize = new Size(ToolIconSize.Width - 10 / 3, ToolIconSize.Width - 10 / 3);
            //BorderThickness = 2;
            //BorderBackColor = this.HeadBackColor;
            //!!!ZQC 11.14
            NumberKeyboard.DefaultValue = NumberKeyboard.DefaultValue;
            base.OnLoad(e);
            Keyboard.AutoLayout();
        }

        private void Stylize()
        {
            IsShowHelp = false;
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(Keyboard, ScopeX.UserControls.Style.StyleFlag.FontSize);
            ScopeX.UserControls.Style.DefaultStyleManager.Instance.RegisterControlRecursion(this, ScopeX.UserControls.Style.StyleFlag.FontSize);
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




    }

    
}

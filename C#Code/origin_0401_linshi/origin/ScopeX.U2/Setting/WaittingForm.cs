using System;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class WaittingForm : Form, IDisposable
    {
        public WaittingForm()
        {
            InitializeComponent();
            // 启用双缓冲
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();

            if (Screen.AllScreens.Length > 0)
            {
                var width = Screen.PrimaryScreen.Bounds.Width;
                var height = Screen.PrimaryScreen.Bounds.Height;
                if (width < Size.Width)
                {
                    Size = new System.Drawing.Size(width, height);
                    LoadCircle.Location = new System.Drawing.Point((Size.Width - LoadCircle.Size.Width) / 2, (Size.Height - LoadCircle.Size.Height) / 2);
                }
            }
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        public Boolean Active
        {
            get => LoadCircle.Active;
            set => LoadCircle.Active = value;
        }
        public override void Refresh()
        {
            LoadCircle.Refresh();
            // base.Refresh();
        }
        public void DisableMouseInput()
        {
            this.Enabled = false;  // 禁用整个窗口
        }

        public void EnableMouseInput()
        {
            this.Enabled = true;  // 启用整个窗口
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            LoadCircle.Dispose();
            base.OnFormClosing(e);
        }
    }
}

using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.ComponentModel;
using ScopeX.Controls.LanguageDefinition;
using ScopeX.Controls.Common.APIs;

namespace ScopeX.U2
{
    /// <summary>
    /// pop弹出窗口基类
    /// 本控件位于窗体中所有控件的上面，但位于其他Form的的下面
    /// 与原生的<see cref="ToolStripDropDown"/>控件不同的是此控件位于窗体中，而不是在窗体外面
    /// 同时本控件支持背景透明和全局透明，支持完全的鼠标穿透，及控件中的文字图标均可以不响应鼠标事件
    /// 同时由于本控件不支持任何其他控件，因此默认不支持多语言切换功能
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ToolboxItem(false)]
    abstract class ScopeXDropDown<T> : Control, ScopeX.Controls.LanguageDefinition.ILanguageControl where T : Control
    {
        private int backopacity = 100;
        /// <summary>
        /// 控件背景透明度
        /// </summary>
        public int Backopacity
        {
            get { return backopacity; }
            set
            {
                if (value != backopacity)
                {
                    backopacity = value;
                    Draw();
                }
            }
        }
        private int opacity = 100;
        /// <summary>
        /// 控件整体透明度
        /// </summary>
        public int Opacity
        {
            get { return opacity; }
            set
            {
                if (opacity != value)
                {
                    opacity = value;
                    Draw();
                }
            }
        }
        protected T _owner;
        Bitmap _Bitmap;
        /// <summary>
        /// 由于未知问题导致控件上方有一个30像素高度的标题栏区域，因此现只能通过平移的方式去除掉上方区域
        /// 由于本控件在完全透明的情况下时是能鼠标穿透的，因此上方区域并不影响鼠标操作
        /// </summary>
        protected readonly int HeightOffset = 30;
        public new int Height { get => base.Height - HeightOffset; set => base.Height = value + HeightOffset; }
        private bool isHitTestVisible = true;
        public bool IsHitTestVisible
        {
            get => isHitTestVisible;
            set
            {
                isHitTestVisible = value;
                var exstyle = laststyle;
                if (!value)
                {
                    exstyle |= 0x20;
                }
                APIsUser32.SetWindowLong(Handle, GWL_EXSTYLE, (uint)exstyle);
            }
        }
        private bool autoClose = false;

        public bool AutoClose
        {
            get { return autoClose; }
            set { autoClose = value; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_owner">拥有者</param>
        public ScopeXDropDown(T _owner)
        {
            if (_owner == null) return;
            this._owner = _owner;
            base.Height = _owner.Height;
            base.Width = _owner.Width;
            Left = _owner.Left;
            Top = _owner.Bottom + 10;
            DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            Cursor = Cursors.Hand;
            Task.Run(async () =>
            {
                while (true)
                {
                    var form = _owner.FindForm();
                    if (form != null)
                    {
                        this.Invoke(new Action(() =>
                        {
                            SetParent(form.Handle);
                        }));
                        form.SizeChanged += (_, _) =>
                          {
                              SetParent(form.Handle);
                              if (Visible)
                              {
                                  Refresh();
                              }
                          };

                        form.LocationChanged += (_, _) =>
                          {
                              APIsUser32.SetParent(Handle, form.Handle);
                          };
                        _owner.LocationChanged += (_, _) =>
                          {
                              APIsUser32.SetParent(Handle, form.Handle);
                          };
                        return;
                    }
                    await Task.Delay(100);
                }
            });
        }
        ~ScopeXDropDown()
        {
            if(Font != null)
            {
                Font = null;
            }
        }
        public void SetParent() => SetParent(_owner.FindForm().Handle);
        void SetParent(IntPtr parent)
        {
            APIsUser32.SetParent(Handle, parent);
            APIsUser32.SetWindowPos(Handle, (IntPtr)(-1), 0, 0, 0, 0, 0x01 | 0x02);
        }
        protected void DisposeBitMap() => _Bitmap?.Dispose();
        const int AC_SRC_OVER = 0;
        const int AC_SRC_ALPHA = 1;
        const int GWL_EXSTYLE = -20;
        int laststyle = 0;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x112 || m.Msg == 0xF012) return;
            base.WndProc(ref m);
        }
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = new CreateParams();
                cp.ClassStyle = 0x8;
                cp.ExStyle = 0x00080008;
                cp.Style = 0x16010000;
                cp.Width = base.Height;
                cp.Height = base.Width;
                cp.Caption = this.GetType().Name;
                laststyle = cp.ExStyle;
                if (!IsHitTestVisible) cp.ExStyle |= 0x20;
                return cp;
            }
        }
        public override void Refresh()
        {
            Draw();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (AutoClose) Hide();
        }
        public new void Show()
        {
            Visible = true;
        }
        public new void Hide()
        {
            Visible = false;
        }
        /// <summary>
        /// 显示或者隐藏控件
        /// </summary>
        public new bool Visible
        {
            get => base.Visible;
            set
            {
                if (base.Visible != value)
                {
                    base.Visible = value;
                    if (value)
                    {
                        APIsUser32.ShowWindow(Handle, APIsEnums.ShowWindowStyles.SHOW);
                    }
                    else APIsUser32.ShowWindow(Handle, APIsEnums.ShowWindowStyles.HIDE);
                }
            }
        }
        private protected LanguagePattern languagePattern = LanguagePattern.Ignore;
        public virtual LanguagePattern LanguagePattern { get => languagePattern; set => languagePattern = LanguagePattern.Ignore; }

        Bitmap CreatBitMap()
        {
            DisposeBitMap();
            if (base.Width == 0 || base.Height == 0) return null;
            return new Bitmap(base.Width, base.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }
        protected int X;
        protected int Y;
        /// <summary>
        /// 在指定位置显示控件
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="y">y</param>
        public void Show(int x, int y)
        {
            this.X = x;
            this.Y = y;
            Show();
        }

        bool firstDrawFlag = true;
        void Draw()
        {
            _Bitmap = CreatBitMap();
            if (_Bitmap == null) return;
            Graphics graphics = Graphics.FromImage(_Bitmap);
            DrawContext(graphics);
            graphics.Dispose();

            #region 添加补丁:王昌杰20211124
            //Bug:  该补丁是为了处理ScopeXDropDown设置了Parent过后,控件显示消失的问题;
            //原因: 在构造函数中调用了SetParent进行设置了Parent.但是,需要在首次显示之前再次调用
            //      SetParent进行设置该控件才能显示正常;
            if (firstDrawFlag)
            {
                var form = _owner.FindForm();
                if (form != null)
                {
                    APIsUser32.SetParent(Handle, form.Handle);
                    firstDrawFlag = false;
                }
            }
            #endregion 添加补丁:王昌杰20211124

            IntPtr screenDc = APIsUser32.GetDC(Handle);
            IntPtr memDc = APIsGdi.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;
            try
            {

                hBitmap = _Bitmap.GetHbitmap(Color.FromArgb((int)(backopacity / 100f * 255), Color.Empty));  // grab a GDI handle from this GDI+ bitmap
                oldBitmap = APIsGdi.SelectObject(memDc, hBitmap);
                APIsStructs.SIZE size = new APIsStructs.SIZE()
                {
                    cx = _Bitmap.Width,
                    cy = _Bitmap.Height,
                };
                APIsStructs.POINTAPI pointSource = new APIsStructs.POINTAPI(0, 0);
                APIsStructs.POINTAPI topPos = new APIsStructs.POINTAPI(X, Y - HeightOffset);
                APIsStructs.BLENDFUNCTION blend = new APIsStructs.BLENDFUNCTION();
                blend.BlendOp = AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = (byte)(Opacity / 100f * 255);
                blend.AlphaFormat = AC_SRC_ALPHA;

                APIsUser32.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, APIsEnums.UpdateLayeredWindowFlags.ALPHA);
            }
            finally
            {
                APIsUser32.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    APIsGdi.SelectObject(memDc, oldBitmap);
                    APIsGdi.DeleteObject(hBitmap);
                }
                APIsGdi.DeleteDC(memDc);
            }
            _Bitmap.Dispose();
        }
        /// <summary>
        /// 绘制控件内容，在绘制时，注意控件上方<see cref="HeightOffset"/>高度的区域时不可绘制的区域
        /// 因此在绘制时需要在垂直方向上平移<see cref="HeightOffset"/>像素
        /// </summary>
        /// <param name="graphics">GDI+</param>
        public virtual void DrawContext(Graphics graphics)
        {
            graphics.Clear(Color.Red);
            TextRenderer.DrawText(graphics, Text, Font, new Point(), ForeColor);
        }
    }
}

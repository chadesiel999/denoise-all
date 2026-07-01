using ScopeX.Controls.Common.APIs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public class MeasureToolButton : Control
    {
        private const Int32 AC_SRC_OVER = 0;
        private const Int32 AC_SRC_ALPHA = 1;
        protected const Int32 HeightOffset = 30;
        private const Int32 AreaMouseExpand = 10;//鼠标扩大范围
        private Control _Parent;
        private Int32 _LastStyle;
        private Bitmap _Bitmap;
        private Int32 _BackOpacity = 80;
        private Int32 _Opacity = 80;

        public new Size Size { get; set; } = new Size(50, 50);
        public new Point Location { get; set; }
        private Bitmap MeasureToolBitmap => Properties.Resources.MeasureTool;

        public Size VisionRegionSize { get; internal set; } = Properties.Resources.MeasureTool.Size;

        public new (Int32 Left, Int32 Top, Int32 Right, Int32 Bottom) Margin { get; set; } = (0, 0, 0, 0);


        /// <summary>
        /// 显示或者隐藏控件
        /// </summary>
        public new Boolean Visible
        {
            get => base.Visible;
            set
            {
                base.Visible = value;
                if (value)
                {
                    APIsUser32.ShowWindow(Handle, APIsEnums.ShowWindowStyles.SHOW);
                }
                else APIsUser32.ShowWindow(Handle, APIsEnums.ShowWindowStyles.HIDE);
            }
        }

        public MeasureToolButton(Control parent)
        {
            _Parent = parent;

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            DoubleBuffered = true;
            Cursor = Cursors.Hand;
            BackColor = Color.Black;
            SetParent(parent.Handle);

            _Parent.LocationChanged += (_, _) =>
            {
                SetParent(_Parent.Handle);
            };
            //_Parent.SizeChanged += (_, _) =>
            //{
            //    SetParent(_Parent.Handle);
            //};
        }

        public override void Refresh()
        {
            Draw();
        }

        public new void Show() => Visible = true;
        public new void Hide() => Visible = false;

        private void Draw()
        {
            _Bitmap = CreatBitMap();
            if (_Bitmap == null) return;
            Graphics graphics = Graphics.FromImage(_Bitmap);
            //画图

            graphics.FillRectangle(new SolidBrush(Color.FromArgb(5, 0, 0, 0)), 0, HeightOffset, MeasureToolBitmap.Width + AreaMouseExpand, MeasureToolBitmap.Height + AreaMouseExpand);
            graphics.DrawImage(Properties.Resources.MeasureTool, new Point(0, 0 + HeightOffset + 4));

            graphics.Dispose();
            //SetParent(_Parent.Handle);

            IntPtr screenDc = APIsUser32.GetDC(Handle);
            IntPtr memDc = APIsGdi.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;
            try
            {

                hBitmap = _Bitmap.GetHbitmap(Color.FromArgb((int)(_BackOpacity / 100f * 255), BackColor));  // grab a GDI handle from this GDI+ bitmap
                oldBitmap = APIsGdi.SelectObject(memDc, hBitmap);
                APIsStructs.SIZE size = new APIsStructs.SIZE()
                {
                    cx = _Bitmap.Width,
                    cy = _Bitmap.Height,
                };
                APIsStructs.POINTAPI pointSource = new APIsStructs.POINTAPI(0, 0);
                APIsStructs.POINTAPI topPos = new APIsStructs.POINTAPI(Location.X, Location.Y - HeightOffset);
                APIsStructs.BLENDFUNCTION blend = new APIsStructs.BLENDFUNCTION();
                blend.BlendOp = AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = (byte)(_Opacity / 100f * 255);
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

        private Bitmap CreatBitMap()
        {
            _Bitmap?.Dispose();
            if (base.Width == 0 || base.Height == 0) return null;
            //return new Bitmap(base.Width, base.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            return new Bitmap(Size.Width, Size.Height + HeightOffset, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        public void SetParent(IntPtr parent)
        {
            APIsUser32.SetParent(Handle, parent);
            APIsUser32.SetWindowPos(Handle, (IntPtr)(-1), 0, 0, 0, 0, 0x01 | 0x02);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x112 || m.Msg == 0xF012) return;
            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                //Int32 CS_DBLCLKS = 0x0008;

                //Int32 WS_EX_LAYERED = 0x00080000;
                //Int32 WS_EX_TOPMOST = 0x00000008;

                //Int32 WS_VISIBLE = 0x10000000;
                //Int32 WS_CLIPCHILDREN = 0x02000000;
                //Int32 WS_CLIPSIBLINGS = 0x04000000;
                //Int32 WS_MAXIMIZEBOX = 0x00010000;

                var cp = new CreateParams();
                cp.ClassStyle = 0x8;
                cp.ExStyle = 0x00080008;
                cp.Style = 0x16C00000;
                cp.Width = Size.Width;
                cp.Height = Size.Height + HeightOffset;
                //cp.ExStyle |= 0x20;
                _LastStyle = cp.ExStyle;
                return cp;
            }
        }

    }
}

using FontStashSharp;
using System.Runtime.InteropServices;
using ScopeX.Controls.Common.APIs;
using ScopeX.Touch;
using Veldrid.Common;
using Veldrid.Common.Tools;

namespace Veldrid.Windows.Winform
{
    public class VeldridControl : Control, ITouchAble
    {
        IVeldridContent control;
        public VeldridControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.StandardClick, true);
            SetStyle(ControlStyles.StandardDoubleClick, true);
            SetStyle(ControlStyles.UserPaint, true);
            control = new Veldrid.Common.VeldridContent();
            control.CursorVisible = true;
            control.WindowSize = this.Size;
            control.BorderVisible = false;
            control.Visible = false;
            SetParent(control.Hwnd, Handle);
            SetWindowPos(control.Hwnd, IntPtr.Zero, 0, 0, Width, Height, SetWindowPosFlags.HideWindow);
            SetWindowLong(control.Hwnd, GWL_STYLE, 0x40000000);
            SetWindowLong(control.Hwnd, GWL_EXSTYLE, 0x08000000);
            _wndProc = OnVeldridWndProc;
            IntPtr intptr = Marshal.GetFunctionPointerForDelegate(_wndProc);
            _originalWndProc = SetWindowLongPtr(control.Hwnd, GWL_WNDPROC, intptr);
            control.X = 0;
            control.Y = 0;

        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            control.Visible = Visible;
        }
        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);
        //    control.Visible = true;
        //}
        const int GWL_EXSTYLE = -20;
        const int GWL_STYLE = -16;
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [Flags()]
        private enum SetWindowPosFlags : uint
        {
            /// <summary>If the calling thread and the thread that owns the window are attached to different input queues,
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from
            /// blocking its execution while other threads process the request.</summary>
            /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
            AsynchronousWindowPosition = 0x4000,
            /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
            /// <remarks>SWP_DEFERERASE</remarks>
            DeferErase = 0x2000,
            /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
            /// <remarks>SWP_DRAWFRAME</remarks>
            DrawFrame = 0x0020,
            /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE
            /// is sent only when the window's size is being changed.</summary>
            /// <remarks>SWP_FRAMECHANGED</remarks>
            FrameChanged = 0x0020,
            /// <summary>Hides the window.</summary>
            /// <remarks>SWP_HIDEWINDOW</remarks>
            HideWindow = 0x0080,
            /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
            /// parameter).</summary>
            /// <remarks>SWP_NOACTIVATE</remarks>
            DoNotActivate = 0x0010,
            /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid
            /// contents of the client area are saved and copied back into the client area after the window is sized or
            /// repositioned.</summary>
            /// <remarks>SWP_NOCOPYBITS</remarks>
            DoNotCopyBits = 0x0100,
            /// <summary>Retains the current position (ignores X and Y parameters).</summary>
            /// <remarks>SWP_NOMOVE</remarks>
            IgnoreMove = 0x0002,
            /// <summary>Does not change the owner window's position in the Z order.</summary>
            /// <remarks>SWP_NOOWNERZORDER</remarks>
            DoNotChangeOwnerZOrder = 0x0200,
            /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent
            /// window uncovered as a result of the window being moved. When this flag is set, the application must
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
            /// <remarks>SWP_NOREDRAW</remarks>
            DoNotRedraw = 0x0008,
            /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
            /// <remarks>SWP_NOREPOSITION</remarks>
            DoNotReposition = 0x0200,
            /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
            /// <remarks>SWP_NOSENDCHANGING</remarks>
            DoNotSendChangingEvent = 0x0400,
            /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
            /// <remarks>SWP_NOSIZE</remarks>
            IgnoreResize = 0x0001,
            /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
            /// <remarks>SWP_NOZORDER</remarks>
            IgnoreZOrder = 0x0004,
            /// <summary>Displays the window.</summary>
            /// <remarks>SWP_SHOWWINDOW</remarks>
            ShowWindow = 0x0040,
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "GetWindowLong")]
        static extern int GetWindowLong([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, int nIndex);

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "SetWindowLong")]
        static extern int SetWindowLong([System.Runtime.InteropServices.InAttribute()] System.IntPtr hWnd, int nIndex, int dwNewLong);
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    cp.ClassStyle |= 0x01 | 0x02 | 0x20;
                }
                return cp;
            }
        }

        private const int GWL_WNDPROC = -4;
        public IVeldridContent Content => control;
        public List<IAxis> Axes => control.Axes;

        public List<ISeries> Series => control.Series;

        public List<ICursor> Cursors => control.Cursors;
        public List<IRender> Sundries => control.Sundries;
        public Double FPS => control.FPS;
        public override Color BackColor { get => control.BackColor; set => control.BackColor = value; }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetParent(control.Hwnd, Handle);
            control.WindowSize = Size;
            control.Visible = true;
            SetWindowPos(control.Hwnd, IntPtr.Zero, 0, 0, Width, Height, SetWindowPosFlags.ShowWindow);
        }

        protected override void Dispose(bool disposing)
        {
            this.ClearEventHandle();
            _TouchHelper?.ClearEventHandle();
            control?.ClearEventHandle();
            control?.Dispose();
            base.Dispose(disposing);
        }
        public void DoRender()
        {
            control.DoRender();
        }
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr VeldridWndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        private VeldridWndProc _wndProc;
        private IntPtr _originalWndProc;


        private IntPtr OnVeldridWndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            Message message = Message.Create(hWnd, msg, wParam, lParam);
            switch (message.Msg)
            {
                //解码触摸消息
                case APIsUser32.WM_TOUCH:
                    _TouchHelper?.DecodeTouch(ref message);
                    break;
                //如果控件处于Touching,那么屏蔽控件的鼠标相关事件
                case APIsUser32.WM_LBUTTONDOWN:
                case APIsUser32.WM_LBUTTONUP:
                case APIsUser32.WM_RBUTTONDOWN:
                case APIsUser32.WM_RBUTTONUP:
                case APIsUser32.WM_MOUSEMOVE:
                case APIsUser32.WM_MOUSELEAVE:
                    if (_TouchHelper.TouchingFlag)
                    {
                    }
                    break;
            }
            return CallWindowProc(_originalWndProc, hWnd, msg, wParam, lParam);
        }
        private static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
            {
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            }
            else
            {
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
            }
        }



        #region ITouchAble
        //定义ITouchAble的接口相关的事件
        public event Action<int, int> TouchDown;
        public event Action<int, int, int, int> TouchMove;
        public event Action<int, int> TouchUp;
        public event Action<float> TouchRotation;
        public event Action<float, ScaleDirection> TouchScale;

        private TouchHelper _TouchHelper;   //触摸功能的帮助器

        /// <summary>
        /// 在创建控件的时候进行触摸的相关初始化
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!DesignMode)
            {
                //初始化该控件的触摸帮助器
                _TouchHelper = new TouchHelper(this);

                _TouchHelper.TouchDown += (x, y) => TouchDown?.Invoke(x, y);
                _TouchHelper.TouchMove += (x, y, transX, transY) => TouchMove?.Invoke(x, y, transX, transY);
                _TouchHelper.TouchUp += (x, y) => TouchUp?.Invoke(x, y);
                _TouchHelper.TouchRotation += (x) => TouchRotation?.Invoke(x);
                _TouchHelper.TouchScale += (x, y) => TouchScale?.Invoke(x, y);

                // 目前基础的MouseDown、OnMouseUp、OnMouseMove都采用系统的事件，不使用触摸中的事件，防止触摸和系统事件同时触发导致意外情况
                // 系统事件注册在：Veldrid.Sdl2.Sdl2Window中
                /*//触发
                TouchDown += (x, y) =>
                {
                    //control.OnMouseDown(new MouseEvent(MouseButton.Left, true, x, y, 1));
                    //OnMouseDown(new MouseEventArgs(MouseButtons.Left, 0, x, y, 0));
                };
                TouchUp += (x, y) =>
                {
                    //control.OnMouseUp(new MouseEvent(MouseButton.Left, false, x, y, 1));
                    //OnMouseUp(new MouseEventArgs(MouseButtons.Left, 0, x, y, 0));
                };
                TouchMove += (x, y, tx, ty) =>
                {
                    //control.OnMouseMove(new Sdl2.MouseMoveEventArgs(new Sdl2.MouseState(x, y, true, false, false, false, false, false, false, false, false, false, false, false, false), new System.Numerics.Vector2(x, y)));
                    //OnMouseMove(new MouseEventArgs(MouseButtons.Left, 0, x, y, 0));
                };*/
            }
        }



        ///// <summary>
        ///// 在消息处理循环中进行相关消息的处理；
        ///// </summary>
        //protected override void WndProc(ref Message msg)
        //{
        //    switch (msg.Msg)
        //    {
        //        //解码触摸消息
        //        case APIsUser32.WM_TOUCH:
        //            _TouchHelper?.DecodeTouch(ref msg);
        //            return;
        //        //如果控件处于Touching,那么屏蔽控件的鼠标相关事件
        //        case APIsUser32.WM_LBUTTONDOWN:
        //        case APIsUser32.WM_LBUTTONUP:
        //        case APIsUser32.WM_RBUTTONDOWN:
        //        case APIsUser32.WM_RBUTTONUP:
        //        case APIsUser32.WM_MOUSEMOVE:
        //        case APIsUser32.WM_MOUSELEAVE:
        //            if (_TouchHelper.TouchingFlag)
        //            {
        //                return;
        //            }
        //            break;
        //    }
        //    base.WndProc(ref msg);
        //}

        #endregion ITouchAble
    }
}
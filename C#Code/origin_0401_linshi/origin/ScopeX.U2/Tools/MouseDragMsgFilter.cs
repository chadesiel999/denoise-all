using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public class MouseDragMsgFilter : IMessageFilter
    {
        public const Int32 WM_LBUTTONUP = 0x0202;
        public const Int32 WM_LBUTTONDOWN = 0x0201;
        public const Int32 WM_MOUSEMOVE = 0x0200;
        public const Int32 WM_MOUSEWHEEL = 0x020A;

        public const Int32 MK_LBUTTON = 0x0001;

        private readonly Control _ParentControl;

        private readonly MouseEventHandler _MouseUp;
        private readonly MouseEventHandler _MouseDown;
        private readonly MouseEventHandler _MouseMove;
        private readonly MouseEventHandler _MouseWheel;

        /// <summary>
        /// MouseMsgFilter 
        /// </summary>
        /// <param name="parentControl">所在的控件</param>
        /// <param name="mouseLBUp">鼠标左键释放事件</param>
        /// <param name="mouseLBDown">鼠标左键按下事件</param>
        /// <param name="mouseLBDownAndMove">鼠标左键按下时移动事件</param>
        /// <param name="mouseWheel">鼠标滚轮事件</param>
        public MouseDragMsgFilter(Control parentControl, MouseEventHandler mouseLBUp, MouseEventHandler mouseLBDown,
            MouseEventHandler mouseLBDownAndMove, MouseEventHandler mouseWheel)
        {
            _ParentControl = parentControl;
            _MouseUp = mouseLBUp;
            _MouseDown = mouseLBDown;
            _MouseMove = mouseLBDownAndMove;
            _MouseWheel = mouseWheel;
        }

        private Boolean _IsDragging = false;
        private int _MoveCount = 0;
                        
        public Boolean PreFilterMessage(ref Message m)
        {
            if (_ParentControl == null || _ParentControl.IsDisposed)
            {
                Application.RemoveMessageFilter(this);
                return false;
            }

            Point clientpos;
            MouseEventArgs mea;
            Boolean inrange;
            switch (m.Msg)
            {
                case WM_LBUTTONDOWN:
                    clientpos = _ParentControl.PointToClient(Control.MousePosition);                    
                    inrange = _ParentControl.ClientRectangle.Contains(clientpos);
                    if (inrange)
                    {
                        _IsDragging = true;
                        mea = new MouseEventArgs(MouseButtons.Left, 1, clientpos.X, clientpos.Y, 0);
                        _MouseDown?.Invoke(_ParentControl, mea);
                        _MoveCount = 0;
                    }
                    break;

                case WM_MOUSEMOVE:
                    if (_IsDragging)
                    {
                        if ((Int32)m.WParam == MK_LBUTTON)
                        {
                            clientpos = _ParentControl.PointToClient(Control.MousePosition);
                            mea = new MouseEventArgs(MouseButtons.Left, 1, clientpos.X, clientpos.Y, 0);
                            _MouseMove?.Invoke(_ParentControl, mea);
                            _MoveCount++;
                        }
                        else
                            _IsDragging = false;
                    }
                    break;

                case WM_LBUTTONUP:                    
                    if (_IsDragging)
                    {
                        _IsDragging = false; 
                        clientpos = _ParentControl.PointToClient(Control.MousePosition);
                        mea = new MouseEventArgs(MouseButtons.Left, 1, clientpos.X, clientpos.Y, 0);
                        _MouseUp?.Invoke(_ParentControl, mea);

                        //拖拽状态直接退出，不往下传递MouseUp消息，避免Click消息的产生
                        //if (_MoveCount > 10)
                        //{
                        //    return true;
                        //}
                    }
                    break;

                case WM_MOUSEWHEEL:
                    var screenpos = Control.MousePosition;
                    clientpos = _ParentControl.PointToClient(screenpos);
                    inrange = _ParentControl.ClientRectangle.Contains(clientpos);
                    if (inrange)
                    {
                        mea = new MouseEventArgs(MouseButtons.None, 1, screenpos.X, screenpos.Y, (Int32)(Int64)m.WParam >> 16);
                        _MouseWheel?.Invoke(_ParentControl, mea);
                    }
                    break;
            }

            return false;
        }

    }
}

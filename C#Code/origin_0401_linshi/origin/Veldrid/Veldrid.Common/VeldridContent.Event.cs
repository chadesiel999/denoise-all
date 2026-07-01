using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Veldrid.Common.Tools;
using Veldrid.Sdl2;

namespace Veldrid.Common
{
    public partial class VeldridContent
    {
        private event EventHandler PlottableDragged;
        private event EventHandler PlottableDropped;
        private event EventHandler RenderEventHandler;
        private event EventHandler Resized;
        private event EventHandler Closing;
        private event EventHandler Closed;
        private event EventHandler FocusLost;
        private event EventHandler FocusGained;
        private event EventHandler Shown;
        private event EventHandler Hidden;
        private event EventHandler MouseEntered;
        private event EventHandler MouseLeave;
        private event EventHandler Exposed;
        private event EventHandler<Point> Moved;
        private event EventHandler<MouseWheelEventArgs> MouseWheel;
        private event EventHandler<MouseMoveEventArgs> MouseMove;
        private event EventHandler<MouseEvent> MouseDown;
        private event EventHandler<MouseEvent> RightMouseDown;
        private event EventHandler<MouseEvent> MouseUp;
        private event EventHandler<KeyEvent> KeyDown;
        private event EventHandler<KeyEvent> KeyUp;
        private event EventHandler<DragDropEvent> DragDrop;
        private IDropRender dropRender = null;// 有多线程安全问题。
        private void InitEvent()
        {
            graphicsManger.Window.Closed += Window_Closed;
            graphicsManger.Window.Closing += Window_Closing;
            graphicsManger.Window.MouseDown += Window_MouseDown;
            graphicsManger.Window.RightMouseDown += Window_RightMouseDown;
            graphicsManger.Window.DoubleClick += Window_DoubleClick;
            graphicsManger.Window.Shown += Window_Shown;
            graphicsManger.Window.KeyDown += Window_KeyDown;
            graphicsManger.Window.KeyUp += Window_KeyUp;
            graphicsManger.Window.MouseEntered += Window_MouseEntered;
            graphicsManger.Window.MouseMove += Window_MouseMove;
            graphicsManger.Window.MouseUp += Window_MouseUp;
            graphicsManger.Window.MouseLeave += Window_MouseLeave;
            graphicsManger.Window.MouseWheel += Window_MouseWheel;
            graphicsManger.Window.Moved += Window_Moved;
            graphicsManger.Window.Exposed += Window_Exposed;
            graphicsManger.Window.FocusGained += Window_FocusGained;
            graphicsManger.Window.FocusLost += Window_FocusLost;
            graphicsManger.Window.DragDrop += Window_DragDrop;
            graphicsManger.Window.Resized += Window_Resized;
            graphicsManger.Window.Hidden += Window_Hidden;
        }


        private void Window_Hidden()
        {
            Hidden?.Invoke(this, EventArgs.Empty);
        }

        private void Window_Resized()
        {
            sizechanged = true;
            Resized?.Invoke(this, EventArgs.Empty);
        }

        private void Window_DragDrop(DragDropEvent obj)
        {
            DragDrop?.Invoke(this, obj);
        }

        private void Window_FocusLost()
        {
            FocusLost?.Invoke(this, EventArgs.Empty);
        }

        private void Window_FocusGained()
        {
            FocusGained?.Invoke(this, EventArgs.Empty);
        }

        private void Window_Exposed()
        {
            Exposed?.Invoke(this, EventArgs.Empty);
        }

        private void Window_Moved(Point obj)
        {
            Moved?.Invoke(this, obj);
        }

        private void Window_MouseWheel(MouseWheelEventArgs obj)
        {
            MouseWheel?.Invoke(this, obj);
        }

        private void Window_MouseLeave()
        {
            bool result = false;
            if (dropRender != null)
            {
                dropRender.OnMouseLeave(ref result);
                dropRender.Selected = false;
                dropRender = null;
            }
            else
            {
                var items = Series.Cast<IRender>().Concat(Cursors).Concat(Sundries).Where(x => !x.Skip && x is IDropRender).Cast<IDropRender>().OrderByDescending(x => x.ZIndex).ToList();
                foreach (var item in items)
                {
                    item.OnMouseLeave(ref result);
                }
                foreach (var item in items)
                {
                    item.Selected = false;
                }
            }
            MouseLeave?.Invoke(this, EventArgs.Empty);
        }

        private void Window_MouseUp(MouseEvent obj)
        {
            bool result = false;

            if (dropRender != null)
            {
                dropRender.OnMouseUp(dropRender.LocalPointToVirtualPoint(obj.Position), ref result);
                if (result)
                {
                    PlottableDropped?.Invoke(dropRender, EventArgs.Empty);
                }
                dropRender.Selected = false;
                dropRender = null;
            }
            else
            {
                var items = Series.Cast<IRender>().Concat(Cursors).Concat(Sundries).Where(x => !x.Skip && x is IDropRender).Cast<IDropRender>().OrderByDescending(x => x.ZIndex).ToList();
                foreach (var item in items)
                {
                    item.OnMouseUp(item.LocalPointToVirtualPoint(obj.Position), ref result);
                    if (result)
                    {
                        PlottableDropped?.Invoke(item, EventArgs.Empty);
                    }
                }
                foreach (var item in items)
                {
                    item.Selected = false;
                }
            }
            MouseUp?.Invoke(this, obj);
        }

        private void Window_MouseMove(MouseMoveEventArgs obj)
        {
            if (dropRender != null)
            {
                dropRender.OnDragged(dropRender.LocalPointToVirtualPoint(obj.MousePosition));
                lastpoint = obj.MousePosition;
            }
            else
            {
                var items = Series.Cast<IRender>().Concat(Cursors).Concat(Sundries).Where(x => !x.Skip && x is IDropRender).Cast<IDropRender>().OrderByDescending(x => x.ZIndex).ToList();
                bool result = false;
                foreach (var item in items)
                {
                    item.OnMouseMove(item.LocalPointToVirtualPoint(obj.MousePosition), ref result);
                    if (result)
                    {
                        break;
                    }
                }
                if (!result)
                {
                    graphicsManger.Window.Cursor = SDL_SystemCursor.Arrow;
                }
            }
            MouseMove?.Invoke(this, obj);
        }

        private void Window_MouseEntered()
        {
            MouseEntered?.Invoke(this, EventArgs.Empty);
        }

        private void Window_KeyUp(KeyEvent obj)
        {
            KeyUp?.Invoke(this, obj);
        }

        private void Window_KeyDown(KeyEvent obj)
        {
            KeyDown?.Invoke(this, obj);
        }

        private void Window_Shown()
        {
            Shown?.Invoke(this, EventArgs.Empty);
        }

        private void Window_DoubleClick(MouseEvent obj)
        {
            if ((DateTime.Now - lastmousedowntime).TotalMilliseconds < 500)
            {
                // FPSVisibily = !FPSVisibily;
            }
            lastmousedowntime = DateTime.Now;
            var items = Series.Cast<IRender>().Concat(Cursors).Concat(Sundries).Where(x => !x.Skip && x is IDropRender).Cast<IDropRender>().OrderByDescending(x => x.ZIndex).ToList();
            bool result = false;
            foreach (var item in items)
            {
                var point = item.LocalPointToVirtualPoint(obj.Position);
                item.OnDoubleClick(point, ref result);
                if (result)
                {
                    item.Selected = true;
                    lastpoint = obj.Position;
                    //PlottableDragged?.Invoke(item, EventArgs.Empty);
                    //dropRender = item;
                    break;
                }
            }
        }

        private void Window_RightMouseDown(MouseEvent obj)
        {
            if ((DateTime.Now - lastmousedowntime).TotalMilliseconds < 500)
            {
                // FPSVisibily = !FPSVisibily;
            }
            lastmousedowntime = DateTime.Now;
            RightMouseDown?.Invoke(this, obj);
            var items = Series.Cast<IRender>().Concat(Cursors).Concat(Sundries).Where(x => !x.Skip && x is IDropRender).Cast<IDropRender>().OrderByDescending(x => x.ZIndex).ToList();
            bool result = false;
            foreach (var item in items)
            {
                var point = item.LocalPointToVirtualPoint(obj.Position);
                item.OnRightMouseDown(point, ref result);
                if (result)
                {
                    item.Selected = true;
                    lastpoint = obj.Position;
                    //PlottableDragged?.Invoke(item, EventArgs.Empty);
                    //dropRender = item;
                    break;
                }
            }
        }

        private void Window_MouseDown(MouseEvent obj)
        {
            if ((DateTime.Now - lastmousedowntime).TotalMilliseconds < 500)
            {
                // FPSVisibily = !FPSVisibily;
            }
            lastmousedowntime = DateTime.Now;
            MouseDown?.Invoke(this, obj);
            var items = Series.Cast<IRender>().Concat(Cursors).Concat(Sundries).Where(x => !x.Skip && x is IDropRender).Cast<IDropRender>().OrderByDescending(x => x.ZIndex).ToList();
            bool result = false;
            foreach (var item in items)
            {
                var point = item.LocalPointToVirtualPoint(obj.Position);
                item.OnMouseDown(point, ref result);
                if (result)
                {
                    item.Selected = true;
                    lastpoint = obj.Position;
                    PlottableDragged?.Invoke(item, EventArgs.Empty);
                    dropRender = item;
                    break;
                }
            }
        }

        private void Window_Closing()
        {
            Closing?.Invoke(this, EventArgs.Empty);
        }

        private void Window_Closed()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        event EventHandler IVeldridContent.Resized
        {
            add
            {
                this.Resized += value;
            }

            remove
            {
                this.Resized -= value;
            }
        }

        event EventHandler IVeldridContent.Closing
        {
            add
            {
                Closing += value;
            }

            remove
            {
                Closing -= value;
            }
        }

        event EventHandler IVeldridContent.Closed
        {
            add
            {
                Closed += value;
            }

            remove
            {
                Closed -= value;
            }
        }

        event EventHandler IVeldridContent.FocusLost
        {
            add
            {
                FocusLost += value;
            }

            remove
            {
                FocusLost -= value;
            }
        }

        event EventHandler IVeldridContent.FocusGained
        {
            add
            {
                FocusGained += value;
            }

            remove
            {
                FocusGained -= value;
            }
        }

        event EventHandler IVeldridContent.Shown
        {
            add
            {
                Shown += value;
            }

            remove
            {
                Shown -= value;
            }
        }

        event EventHandler IVeldridContent.Hidden
        {
            add
            {
                Hidden += value;
            }

            remove
            {
                Hidden -= value;
            }
        }

        event EventHandler IVeldridContent.MouseEntered
        {
            add
            {
                MouseEntered += value;
            }

            remove
            {
                MouseEntered -= value;
            }
        }

        event EventHandler IVeldridContent.MouseLeave
        {
            add
            {
                MouseLeave += value;
            }

            remove
            {
                MouseLeave -= value;
            }
        }

        event EventHandler IVeldridContent.Exposed
        {
            add
            {
                Exposed += value;
            }

            remove
            {
                Exposed -= value;
            }
        }

        event EventHandler<Point> IVeldridContent.Moved
        {
            add
            {
                Moved += value;
            }

            remove
            {
                Moved -= value;
            }
        }

        event EventHandler<MouseWheelEventArgs> IVeldridContent.MouseWheel
        {
            add
            {
                MouseWheel += value;
            }

            remove
            {
                MouseWheel -= value;
            }
        }

        event EventHandler<MouseMoveEventArgs> IVeldridContent.MouseMove
        {
            add
            {
                MouseMove += value;
            }

            remove
            {
                MouseMove -= value;
            }
        }

        event EventHandler<MouseEvent> IVeldridContent.MouseDown
        {
            add
            {
                MouseDown += value;
            }

            remove
            {
                MouseDown -= value;
            }
        }

        event EventHandler<MouseEvent> IVeldridContent.RightMouseDown
        {
            add
            {
                RightMouseDown += value;
            }

            remove
            {
                RightMouseDown -= value;
            }
        }

        event EventHandler<MouseEvent> IVeldridContent.MouseUp
        {
            add
            {
                MouseUp += value;
            }

            remove
            {
                MouseUp -= value;
            }
        }

        event EventHandler<KeyEvent> IVeldridContent.KeyDown
        {
            add
            {
                KeyDown += value;
            }

            remove
            {
                KeyDown -= value;
            }
        }

        event EventHandler<KeyEvent> IVeldridContent.KeyUp
        {
            add
            {
                KeyUp += value;
            }

            remove
            {
                KeyUp -= value;
            }
        }

        event EventHandler<DragDropEvent> IVeldridContent.DragDrop
        {
            add => DragDrop += value;
            remove => DragDrop -= value;
        }
        event EventHandler IVeldridContent.PlottableDragged
        {
            add => PlottableDragged += value;
            remove => PlottableDragged -= value;
        }
        event EventHandler IVeldridContent.PlottableDropped
        {
            add => PlottableDropped += value;
            remove => PlottableDropped -= value;
        }
        event EventHandler IVeldridContent.RenderEventHandler
        {
            add => RenderEventHandler += value;
            remove => RenderEventHandler -= value;
        }
        Vector2 lastpoint;

        private DateTime lastmousedowntime;

        public void OnMouseDown(MouseEvent mouseEvent)
        {
            if (MouseDown != null)
            {
                MouseDown?.Invoke(this, mouseEvent);
            }
            else
            {
                Window_MouseDown(mouseEvent);
            }
        }

        public void OnRightMouseDown(MouseEvent mouseEvent)
        {
            if (RightMouseDown != null)
            {
                RightMouseDown?.Invoke(this, mouseEvent);
            }
            else
            {
                Window_RightMouseDown(mouseEvent);
            }
        }
        public void OnMouseMove(MouseMoveEventArgs mouseMove)
        {
            if (MouseMove != null)
            {
                MouseMove?.Invoke(this, mouseMove);
            }
            else
            {
                Window_MouseMove(mouseMove);
            }
        }
        public void OnMouseUp(MouseEvent mouseEvent)
        {
            if (MouseUp != null)
            {
                MouseUp?.Invoke(this, mouseEvent);
            }
            else
            {
                Window_MouseUp(mouseEvent);
            }
        }
        public void OnMouseWheel(MouseWheelEventArgs mouseWheel)
        {
            if (MouseWheel != null)
            {
                MouseWheel?.Invoke(this, mouseWheel);
            }
            else
            {
                Window_MouseWheel(mouseWheel);
            }
        }

    }
}

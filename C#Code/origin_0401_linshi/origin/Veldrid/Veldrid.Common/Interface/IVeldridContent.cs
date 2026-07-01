using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Sdl2;

namespace Veldrid.Common
{
    public interface IVeldridContent:IDisposable
    {
        public SDL_SystemCursor Cursor
        {
            get => Window.Cursor;
            set => Window.Cursor = value;
        }
        public double FPS { get; }
        internal GraphicsManger GraphicsManger { get; }
        public List<IAxis> Axes { get; }
        public List<ISeries> Series { get; }
        public List<ICursor> Cursors { get; }
        public List<IRender> Sundries { get; }
        public void DoRender();
        internal Sdl2.Sdl2Window Window => GraphicsManger.Window;
        public Color BackColor { get; set; }
        public IntPtr Hwnd => Window.Handle;
        public Boolean Visible { get=>Window.Visible; set=>Window.Visible =value; }
        public Size WindowSize { get; set; }
        public Boolean IsInitialized { get; }
        public void Init();
        public Boolean IsExists => GraphicsManger==null?false: GraphicsManger.IsExists;
        public String Title { get=>Window.Title; set=>Window.Title =value; }
        public int X { get => Window.X; set => Window.X = value; }
        public int Y { get => Window.Y;set=>Window.Y = value; }
        public Boolean BorderVisible { get => Window.BorderVisible; set => Window.BorderVisible = value;}
        public Boolean CursorVisible { get => Window.CursorVisible;set=>Window.CursorVisible= value;}
        public Rectangle Bounds=>Window.Bounds;
        public Vector2 ScaleFactor=>Window.ScaleFactor;
        public Vector2 MouseDelta => Window.MouseDelta;
        public Boolean Focused=> Window.Focused;
        public void SetMousePosition(int x, int y)=>Window.SetMousePosition(x, y);
        public Boolean Resizable
        {
            get=> Window.Resizable;
            set => Window.Resizable = value;
        }
        public float Opacity
        {
            get=> Window.Opacity;
            set => Window.Opacity = value;
        }
        public void SetLineRange(int minx ,int maxx,int miny,int maxy)
        {
            GraphicsManger.SetLineRange(minx, maxx, miny, maxy);
            Axes.ForEach(x => x.LineRange = GraphicsManger.DefaultLineRange);
            Series.ForEach(x => x.LineRange = GraphicsManger.DefaultLineRange);
            Cursors.ForEach(x => x.LineRange = GraphicsManger.DefaultLineRange);
            Sundries.ForEach(x => x.LineRange = GraphicsManger.DefaultLineRange);
        }
        public LineRange Range => GraphicsManger.DefaultLineRange;
        #region event

        public event EventHandler PlottableDragged;
        public event EventHandler PlottableDropped;
        public event EventHandler RenderEventHandler;


        public event EventHandler Resized;
        public event EventHandler Closing;
        public event EventHandler Closed;
        public event EventHandler FocusLost;
        public event EventHandler FocusGained;
        public event EventHandler Shown;
        public event EventHandler Hidden;
        public event EventHandler MouseEntered;
        public event EventHandler MouseLeave;
        public event EventHandler Exposed;
        public event EventHandler<Point> Moved;
        public event EventHandler<MouseWheelEventArgs> MouseWheel;
        public event EventHandler<MouseMoveEventArgs> MouseMove;
        public event EventHandler<MouseEvent> MouseDown;
        public event EventHandler<MouseEvent> RightMouseDown;
        public event EventHandler<MouseEvent> MouseUp;
        public event EventHandler<KeyEvent> KeyDown;
        public event EventHandler<KeyEvent> KeyUp;
        public event EventHandler<DragDropEvent> DragDrop;

        public void OnMouseDown(MouseEvent mouseEvent);
        public void OnRightMouseDown(MouseEvent mouseEvent);
        public void OnMouseMove(MouseMoveEventArgs mouseMove);
        public void OnMouseUp(MouseEvent mouseEvent);
        public void OnMouseWheel(MouseWheelEventArgs mouseWheel);
        #endregion
    }
}

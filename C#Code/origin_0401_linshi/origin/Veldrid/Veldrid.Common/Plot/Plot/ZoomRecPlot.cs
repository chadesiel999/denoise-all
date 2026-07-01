using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid.Common.VeldridRender;

namespace Veldrid.Common.Plot
{
    public class ZoomRecPlot : BaseDropRender
    {
        private PointF _LastPointF = new PointF();
        private ZoomRecRender dataRender;
        private Boundary _SelectedBoundary = Boundary.Body;
        private const float _BorderRangeByUnit = 20;
        public  event EventHandler RectChanged;
        public ZoomRecPlot(IVeldridContent control) : base(control)
        {
            dataRender = new ZoomRecRender(control);
            dataRender.CreateResources();
            this.Dragged += (o, p) => OnDragged(o, p);
        }
        private protected override BaseVeldridRender Renderer => dataRender;

        public Color BackColor { get => dataRender.BackColor; set => dataRender.BackColor = value; }
        public Color BorderColor { get => dataRender.BorderColor; set => dataRender.BorderColor = value; }
        public RectangleF Rect { get => dataRender.Rect; set => dataRender.Rect = value; }

        private void OnDragged(Object sender, PointF point)
        {
            RectangleF temprect = Rect;
            switch (_SelectedBoundary)
            {
                case Boundary.Top:
                    temprect.Height = Math.Min(LineRange.MaxY - temprect.Y, point.Y - temprect.Y);
                    SetCursor(Sdl2.SDL_SystemCursor.SizeNS);
                    break;
                case Boundary.Right:
                    temprect.Width = Math.Min(LineRange.MaxX - temprect.X, point.X - temprect.X);
                    SetCursor(Sdl2.SDL_SystemCursor.SizeWE);
                    break;
                case Boundary.Bottom:
                    Single topy = temprect.Y + temprect.Height;

                    temprect.Height -= point.Y - temprect.Y;
                    if (temprect.Height < 8)//8是垂直位置的千分之一。
                    {
                        temprect.Height = 8;
                        temprect.Y = topy - 8;
                    }
                    else
                    {
                        temprect.Y = point.Y;
                    }
                    SetCursor(Sdl2.SDL_SystemCursor.SizeNS);
                    break;
                case Boundary.Left:
                    SetCursor(Sdl2.SDL_SystemCursor.SizeWE);
                    Single rightx= temprect.X + temprect.Width;
                    temprect.Width -= point.X - temprect.X;
                    if (temprect.Width < 10)//10是水平位置的千分之一
                    {
                        temprect.Width = 10;
                        temprect.X = rightx - 10;
                    }
                    else
                    {
                        temprect.X = point.X;
                    }
                    break;
                case Boundary.Body:
                    SetCursor(Sdl2.SDL_SystemCursor.Hand);
                    float minx = temprect.X - LineRange.MinX;
                    float maxx = LineRange.MaxX - temprect.X - temprect.Width;
                    float miny = temprect.Y - LineRange.MinY;
                    float maxy = LineRange.MaxY - temprect.Y - temprect.Height;
                    float offsetx = point.X - _LastPointF.X;
                    float offsety = point.Y - _LastPointF.Y;
                    if (offsetx >= 0)
                    {
                        temprect.X += Math.Min(maxx, offsetx);
                    }
                    else
                    {
                        temprect.X -= Math.Min(minx, Math.Abs(offsetx));
                    }

                    if (offsety >= 0)
                    {
                        temprect.Y += Math.Min(maxy, offsety);
                    }
                    else
                    {
                        temprect.Y -= Math.Min(miny, Math.Abs(offsety));
                    }
                    break;
                default:
                    SetCursor(Sdl2.SDL_SystemCursor.Arrow);
                    break;
            }
            if (temprect.Width <= 0)
            {
                //temprect.X = temprect.X + temprect.Width;
                //temprect.Width = -temprect.Width;
                temprect.Width = 10;
            }
            if (temprect.Height <= 0)
            {
                //temprect.Y = temprect.Y + temprect.Height;
                //temprect.Height = -temprect.Height;

                temprect.Height = 8;
            }
            Rect = temprect;
            _LastPointF = point;
            if(RectChanged!=null)
            {
                RectChanged.Invoke(this, EventArgs.Empty);
            }
        }

        internal override void OnMouseMove(PointF point, ref Boolean handle)
        {
            base.OnMouseMove(point, ref handle);
            Boundary boundary = CheckPoisition(point);
            handle = boundary != Boundary.Out;
            if (handle)
            {
                switch (boundary)
                {
                    case Boundary.Left:
                    case Boundary.Right:
                        SetCursor(Sdl2.SDL_SystemCursor.SizeWE);
                        break;
                    case Boundary.Top:
                    case Boundary.Bottom:
                        SetCursor(Sdl2.SDL_SystemCursor.SizeNS);
                        break;
                    case Boundary.Body:
                        SetCursor(Sdl2.SDL_SystemCursor.Hand);
                        break;
                    default:
                        SetCursor(Sdl2.SDL_SystemCursor.Arrow);
                        break;
                }
            }
        }

        internal override void OnMouseDown(PointF point, ref Boolean handle)
        {
            base.OnMouseDown(point, ref handle);
            handle = true;
            _SelectedBoundary = CheckPoisition(point);

            if (_SelectedBoundary == Boundary.Out)
            {
                handle = false;
            }
            else
            {
                _LastPointF = point;
            }
        }

        internal override void OnMouseUp(PointF point, ref Boolean handle)
        {
            base.OnMouseUp(point, ref handle);
        }

        internal override void OnMouseLeave(ref Boolean handle)
        {
            base.OnMouseLeave(ref handle);
        }

        private Boundary CheckPoisition(PointF point)
        {
            var boundary = Boundary.Body;
            var xoffset = _BorderRangeByUnit * LineRange.XLenght / dataRender.Rectangle.Width;
            var yoffset = _BorderRangeByUnit * LineRange.YLenght / dataRender.Rectangle.Height;
            var temprect = new RectangleF(Rect.X - xoffset, Rect.Y, xoffset * 2, Rect.Height);
            if (temprect.Contains(point))
            {
                boundary = Boundary.Left;
            }
            else
            {
                temprect = new RectangleF(Rect.X, Rect.Y + Rect.Height - yoffset, Rect.Width, yoffset * 2);
                if (temprect.Contains(point))
                {
                    boundary = Boundary.Top;
                }
                else
                {
                    temprect = new RectangleF(Rect.X + Rect.Width - xoffset, Rect.Y, xoffset * 2, Rect.Height);
                    if (temprect.Contains(point))
                    {
                        boundary = Boundary.Right;
                    }
                    else
                    {
                        temprect = new RectangleF(Rect.X, Rect.Y - yoffset, Rect.Width, yoffset * 2);
                        if (temprect.Contains(point))
                        {
                            boundary = Boundary.Bottom;
                        }
                        else
                        {
                            temprect = new RectangleF(Rect.X + xoffset, Rect.Y + yoffset, Rect.Width - xoffset, Rect.Height - yoffset);
                            if (temprect.Contains(point))
                            {
                                boundary = Boundary.Body;
                            }
                            else
                            {
                                boundary = Boundary.Out;
                            }
                        }
                    }
                }
            }
            return boundary;
        }
    }
}

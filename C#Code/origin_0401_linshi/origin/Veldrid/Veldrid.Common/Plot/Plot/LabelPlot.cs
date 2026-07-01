using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.Plot.Plot
{
    public class LabelPlot : BaseDropRender
    {
        private VeldridText _VeldridText;
        private PointF _LastPointF = new PointF();
        public event EventHandler<PointF> LocalChanged;
        private Boundary _SelectedBoundary = Boundary.Body;

        public LabelPlot(IVeldridContent control) : base(control)
        {
            _VeldridText = new VeldridText(control, false);
            _VeldridText.CreateResources();
        }
        private protected override BaseVeldridRender Renderer => _VeldridText;
        public String FontName { get => _VeldridText.FontName; set => _VeldridText.FontName = value; }

        public String FontStyle { get => _VeldridText.FontStyle; set => _VeldridText.FontStyle = value; }

        public float FontSize { get => _VeldridText.FontSize; set => _VeldridText.FontSize = value; }
        public String Label { get => _VeldridText.Text; set => _VeldridText.Text = value; }
        public bool LabelVisibility { get => _VeldridText.Visibily; set => _VeldridText.Visibily = value; }
        public Color Color { get => _VeldridText.Color; set => _VeldridText.Color = value; }
        public PointF TextLocal { get => _VeldridText.Local; set => _VeldridText.Local = value; }
        public void OnDragged(Object sender, PointF point)
        {
            var size = _VeldridText.VirtualSize;
            RectangleF temprect = new RectangleF(_VeldridText.Local, new SizeF(size.X, size.Y));
            PointF temp = _VeldridText.Local;
            switch (_SelectedBoundary)
            {
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
                        temp.X += Math.Min(maxx, offsetx);
                    }
                    else
                    {
                        temp.X -= Math.Min(minx, Math.Abs(offsetx));
                    }

                    if (offsety >= 0)
                    {
                        temp.Y += Math.Min(maxy, offsety);
                    }
                    else
                    {
                        temp.Y -= Math.Min(miny, Math.Abs(offsety));
                    }
                    break;
                default:
                    SetCursor(Sdl2.SDL_SystemCursor.Arrow);
                    break;
            }
            _LastPointF = point;
            _VeldridText.Local = temp;
            if (LocalChanged != null)
            {
                LocalChanged.Invoke(this, new PointF());
            }
        }

        private RectangleF GetLabelRect()
        {
            var size = _VeldridText.VirtualSize;
            return new RectangleF(_VeldridText.Local.X - size.X/2, _VeldridText.Local.Y - size.Y*3/2,size.X *2,size.Y*4);
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
                    case Boundary.Body:
                        SetCursor(Sdl2.SDL_SystemCursor.Hand);
                        break;
                    default:
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
        public Boolean IsInRegion(PointF point)
        {
            point = this.LocalPointToVirtualPoint(new System.Numerics.Vector2(point.X, point.Y));
            RectangleF temprect = GetLabelRect();
            return temprect.Contains(point);
        }


        private Boundary CheckPoisition(PointF point)
        {
            var boundary = Boundary.Body;
            RectangleF temprect = GetLabelRect();

            if (temprect.Contains(point))
            {
                boundary = Boundary.Body;
            }
            else
            {
                boundary = Boundary.Out;
            }

            return boundary;
        }
    }
}

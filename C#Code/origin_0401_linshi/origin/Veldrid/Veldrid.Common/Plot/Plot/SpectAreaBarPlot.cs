using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Veldrid.Common.VeldridRender;

namespace Veldrid.Common.Plot
{
    public class SpectAreaBarPlot : BasePlot
    {
        //pixel
        private const Int32 _AreaTouchExpand = 25;
        private Object locker = new Object();
        private SpectAreaBarRender _SpectAreaBarRender;
        private PointF _LastPointF = new PointF();
        private const Int32 _XBarCount = 50;
        private const Int32 _YBarCount = 50;
        private Int32[] _XBarHeight = new Int32[_XBarCount];
        private Int32[] _YBarHeight = new Int32[_YBarCount];
        private enum Boundary { Top, Bottom, Left, Right, In, Out };
        private Boundary _SelectedBoundary = Boundary.Out;

        public event Action? DoubleClick;

        public event Action? Clicked;

        public SpectAreaBarPlot(IVeldridContent control, Int32 maxChannelDataLenght) : base(control)
        {
            MaxChannelDataLenght = maxChannelDataLenght;
            _SpectAreaBarRender = new SpectAreaBarRender(control, _XBarCount, _YBarCount);

            _SpectAreaBarRender.CreateResources();
        }
        public Int32 MaxChannelDataLenght { get; }

        private protected override BaseVeldridRender Renderer => _SpectAreaBarRender;

        public Color BarPlotColor
        {
            get => _SpectAreaBarRender.BarColor;
            set
            {
                _SpectAreaBarRender.BarColor = value;
                _SpectAreaBarRender.RectColor = value;
            }
        }

        public void UpdaeBarData() => CalcBarData();

        private void CalcBarData()
        {
            _XBarHeight = new Int32[_XBarCount];
            _YBarHeight = new Int32[_YBarCount];
            CalcBarValue(ref _XBarHeight, ref _YBarHeight);
            List<Vector2> vector2s = new List<Vector2>();
            for (Int32 index = 0; index < _XBarCount; index++)
            {
                vector2s.Add(new Vector2(rect.X + rect.Width / _XBarCount * index, _XBarHeight[index] + LineRange.MinY));
                vector2s.Add(new Vector2(rect.X + rect.Width / _XBarCount * (index + 1), _XBarHeight[index] + LineRange.MinY));
                vector2s.Add(new Vector2(rect.X + rect.Width / _XBarCount * index, LineRange.MinY));
                vector2s.Add(new Vector2(rect.X + rect.Width / _XBarCount * (index + 1), LineRange.MinY));
            }
            for (Int32 index = 0; index < _YBarCount; index++)
            {
                vector2s.Add(new Vector2(LineRange.MinX, rect.Y + rect.Height - (index * (rect.Height / _YBarCount))));
                vector2s.Add(new Vector2(LineRange.MinX + _YBarHeight[^(index + 1)], rect.Y + rect.Height - (index * (rect.Height / _YBarCount))));
                vector2s.Add(new Vector2(LineRange.MinX, rect.Y + rect.Height - ((index + 1) * (rect.Height / _YBarCount))));
                vector2s.Add(new Vector2(LineRange.MinX + _YBarHeight[^(index + 1)], rect.Y + rect.Height - ((index + 1) * (rect.Height / _YBarCount))));
            }
            _SpectAreaBarRender.SetBarData(vector2s.ToArray());
        }

        protected override void Dispose(bool disposing)
        {
            datas = new double[0, 0];
            base.Dispose(disposing);
        }
        public Double[,] Datas
        {
            get => datas;
            set
            {
                lock (locker)
                {
                    if (datas != value)
                    {
                        datas = value;
                        //CalcBarData();
                    }
                }
            }
        }

        private unsafe void CalcBarValue(ref Int32[] xresult, ref Int32[] yresult)
        {
            if (datas == null || datas.Length == 0) return;
            Int32 startx = (Int32)Math.Floor((rect.X - OffsetX) * ZoomRatio) - 1;
            Int32 endx = (Int32)Math.Ceiling((Rect.X + Rect.Width - OffsetX) * ZoomRatio) + 1;
            float width = 0;

            if (startx < 0 && endx < datas.Length)
            {
                startx = 0;
                width = OffsetX == 0 ? Rect.Width / (endx - startx) : (Rect.X + Rect.Width - OffsetX) / (endx - startx);
            }
            else if (startx > 0 && endx > datas.Length)
            {
                endx = datas.Length;
                width = (datas.Length / ZoomRatio + OffsetX - Rect.X) / (endx - startx);

            }
            else if (startx < 0 && endx > datas.Length)
            {
                startx = 0;
                endx = (Int32)(datas.Length / ZoomRatio + OffsetX);

                width = (endx - OffsetX) / (endx - startx);
            }
            else
            {
                width = Rect.Width / (endx - startx);
            }

            if (endx - startx <= 0)
            {
                return;
            }
            Int32 frmaecount = (Int32)Math.Ceiling(datas.Length / (Single)MaxChannelDataLenght);
            for (Int32 frame = 0; frame < frmaecount; frame++)
            {

                fixed (double* ptr = &(datas[frame, startx]))
                {
                    Int32 lastindex = -1;
                    for (Int32 i = 0; i < endx - startx; i++)
                    {
                        Int32 yindex = (Int32)((ptr[i] - rect.Y) / (rect.Height / _YBarCount));
                        int xindex = ZoomRatio == 1 || startx > 0
                            ? (Int32)(i / (rect.Width / _XBarCount * ZoomRatio))
                            : (Int32)((i + (OffsetX - rect.X) * ZoomRatio) / (rect.Width / _XBarCount * ZoomRatio));
                        if (xindex >= 0 && xindex <= _XBarCount - 1 && (yindex >= 0 && yindex <= _YBarCount - 1))
                        {
                            if (lastindex != xindex)
                            {
                                xresult[xindex] += 200;
                            }
                            lastindex = xindex;
                            yresult[yindex]++;
                        }
                    }
                }
            }
        }

        public override float Brightness
        {
            get => _SpectAreaBarRender.Brightness;
            set
            {
                _SpectAreaBarRender.Brightness = value;
            }
        }

        private RectangleF rect;

        public RectangleF Rect
        {
            get
            {
                return rect;
            }
            set
            {
                if (rect != value)
                {
                    rect = value;
                    Vector2[] vectors = new Vector2[5];
                    vectors[0] = new Vector2(rect.X, rect.Y);
                    vectors[1] = new Vector2(rect.X + rect.Width, rect.Y);
                    vectors[2] = new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
                    vectors[3] = new Vector2(rect.X, rect.Y + rect.Height);
                    vectors[4] = new Vector2(rect.X, rect.Y);
                    _SpectAreaBarRender.SetRectData(vectors);
                    CalcBarData();
                }
            }
        }
        public void ClearCache()
        {
            _SpectAreaBarRender.ClearCache();
        }

        private double[,] datas = new double[0, 0];
        private float zoomRatio;
        private float offsetX;

        public float OffsetX
        {
            get => offsetX;
            set
            {
                if (offsetX != value)
                {
                    offsetX = value;
                    //CalcBarData();
                }
            }
        }
        public float ZoomRatio
        {
            get => zoomRatio;
            set
            {
                if (zoomRatio != value)
                {
                    zoomRatio = value;
                    //CalcBarData();
                }
            }
        }
        public Boolean UseCache { get => _SpectAreaBarRender.UseCache; set => _SpectAreaBarRender.UseCache = value; }

        /// <summary>
        /// 是否在区域内或者边缘
        /// </summary>
        /// <returns>true:边缘或者内部</returns>
        public Boolean IsInPlot(PointF point)
        {
            //var x = (10000 * point.X / _SpectAreaBarRender.Rectangle.Width);
            //var y = 4000 - (8000 * point.Y / _SpectAreaBarRender.Rectangle.Height);
            _SelectedBoundary = CheckPoisition(point);
            return _SelectedBoundary != Boundary.Out;
        }

        private Boundary CheckPoisition(PointF point)
        {
            Boundary boundary = Boundary.Out;
            var xoffset = _AreaTouchExpand * LineRange.XLenght / _SpectAreaBarRender.Rectangle.Width;
            var yoffset = _AreaTouchExpand * LineRange.YLenght / _SpectAreaBarRender.Rectangle.Height;

            RectangleF temprect = new RectangleF(rect.X - xoffset, rect.Y, xoffset * 2, rect.Height);
            if (temprect.Contains(point))
            {
                boundary = Boundary.Left;
            }
            else
            {
                temprect = new RectangleF(rect.X, rect.Y + rect.Height - yoffset, rect.Width, yoffset * 2);
                if (temprect.Contains(point))
                {
                    boundary = Boundary.Top;
                }
                else
                {
                    temprect = new RectangleF(rect.X + rect.Width - xoffset, rect.Y, xoffset * 2, rect.Height);
                    if (temprect.Contains(point))
                    {
                        boundary = Boundary.Right;
                    }
                    else
                    {
                        temprect = new RectangleF(rect.X, rect.Y - yoffset, rect.Width, yoffset * 2);
                        if (temprect.Contains(point))
                        {
                            boundary = Boundary.Bottom;
                        }
                        else
                        {
                            temprect = new RectangleF(rect.X + xoffset, rect.Y + yoffset, rect.Width - xoffset, rect.Height - yoffset);
                            boundary = temprect.Contains(point) ? Boundary.In : Boundary.Out;
                        }
                    }
                }

            }
            return boundary;
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
                Clicked?.Invoke();
            }
        }

        internal override void OnDoubleClick(PointF point, ref bool handle)
        {
            base.OnDoubleClick(point, ref handle);
            /// 双击和右击是一个效果
            OnRightMouseDown(point, ref handle);
        }

        internal override void OnRightMouseDown(PointF point, ref bool handle)
        {
            base.OnRightMouseDown(point, ref handle);
            //Visibily = !Visibily;
            DoubleClick?.Invoke();
        }

        internal override void OnMouseUp(PointF point, ref Boolean handle)
        {
            base.OnMouseUp(point, ref handle);
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
                    case Boundary.In:
                        SetCursor(Sdl2.SDL_SystemCursor.Hand);
                        break;
                }
            }
        }
        protected override void ActiveDragged(object sender, PointF point)
        {
            RectangleF temprect = rect;
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
                    temprect.Height -= point.Y - temprect.Y;
                    temprect.Y = point.Y;
                    SetCursor(Sdl2.SDL_SystemCursor.SizeNS);
                    break;
                case Boundary.Left:
                    SetCursor(Sdl2.SDL_SystemCursor.SizeWE);
                    temprect.Width -= point.X - temprect.X;
                    temprect.X = point.X;
                    break;
                case Boundary.In:
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
                case Boundary.Out:
                    SetCursor(Sdl2.SDL_SystemCursor.Arrow);
                    break;
                default:
                    break;
            }
            if (temprect.Width < 0)
            {
                temprect.X = temprect.X + temprect.Width;
                temprect.Width = -temprect.Width;
            }
            if (temprect.Height < 0)
            {
                temprect.Y = temprect.Y + temprect.Height;
                temprect.Height = -temprect.Height;
            }
            Rect = temprect;
            _LastPointF = point;
            base.ActiveDragged(sender, point);
        }

    }
}

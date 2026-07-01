using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Veldrid.Common.VeldridRender;

namespace Veldrid.Common.Tools
{
    public static class AxesTransfer
    {
        public static PointF LocalPointToVirtualPoint<T>(this T render, Vector2 point) where T :IRender
        {
            return LocalPointToVirtualPoint(render.Margin, render.LineRange, render.WindowSize, point);
        }
        internal static PointF LocalPointToVirtualPoint(this BaseVeldridRender render, Vector2 point)
        {
            return LocalPointToVirtualPoint(render.Margin, render.Range, render.WindowSize, point);
        }
        private static PointF LocalPointToVirtualPoint(Veldrid.Common.Padding margin,LineRange range,Vector2 windowsize, Vector2 point)
        {
            float left = point.X - margin.Left;
            float top = point.Y - margin.Top;
            left = range.XLenght / (windowsize.X - margin.Left - margin.Right) * left + range.MinX;
            top = range.YLenght / (windowsize.Y - margin.Top - margin.Bottom) * top;
            top = range.MaxY - top;
            return new PointF(left, top);
        }
        public static Vector2 LocalSizeToVirtualSize<T>(this T render, Vector2 size) where T :IRender
        {
            SizeF sizeF = render.LocalSizeToVirtualSize(size.X, size.Y);
            return Unsafe.As<SizeF, Vector2>(ref sizeF);
        }
        public static Vector2 VirtualSizeToLocalSize<T>(this T render, float width, float height) where T :IRender
        {
            return VirtualSizeToLocalSize(render.Margin, render.LineRange, render.WindowSize, width, height);
        }
        internal static Vector2 LocalSizeToVirtualSize(this BaseVeldridRender render, Vector2 size)
        {
            SizeF sizeF = render.LocalSizeToVirtualSize(size.X, size.Y);
            return Unsafe.As<SizeF, Vector2>(ref sizeF);
        }
        internal static Vector2 VirtualSizeToLocalSize(this BaseVeldridRender render, float width, float height)
        {
            return VirtualSizeToLocalSize(render.Margin, render.Range, render.WindowSize, width, height);
        }
        internal static Vector2 VirtualSizeToLocalSize(Padding margin,LineRange range,Vector2 windowsize, float width, float height)
        {
            Vector2 rect = new Vector2(windowsize.X - margin.Left - margin.Right, windowsize.Y - margin.Top - margin.Bottom);
            float w = width / range.XLenght * (rect.X / windowsize.X) * 2;
            float h = height / range.YLenght * (rect.Y / windowsize.Y) * 2;
            return new Vector2(w, h);
        }
        public static Vector2 VirtualPointToLocalPoint<T>(this T render, float x, float y) where T:IRender
        {
            return VirtualPointToLocalPoint(render.Camera.OrthographicMatrix, render.Camera.GetLineMatrix(render.Margin, render.LineRange),x,y);
        }
        internal static Vector2 VirtualPointToLocalPoint(this BaseVeldridRender render, float x, float y)
        {
            return VirtualPointToLocalPoint(render.Camera.OrthographicMatrix, render.Camera.GetLineMatrix(render.Margin, render.Range), x, y);
        }
        internal static Vector2 VirtualPointToLocalPoint(Matrix4x4 orth,Matrix4x4 view, float x, float y)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.M11 = x;
            matrix.M21 = y;
            matrix.M41 = 1;
            matrix = view*orth*matrix;

            return new Vector2(matrix.M11-1, matrix.M21*2+1);
        }
        public static SizeF LocalSizeToVirtualSize<T>(this T render, float width, float height) where T:IRender
        {
            return LocalSizeToVirtualSize(render.Rectangle, render.LineRange, width, height);
        }
        internal static SizeF LocalSizeToVirtualSize(this BaseVeldridRender render, float width, float height)
        {
            return LocalSizeToVirtualSize(render.Rectangle, render.Range, width, height);
        }
        private static SizeF LocalSizeToVirtualSize(RectangleF rect,LineRange range, float width, float height)
        {
            return new SizeF(width / rect.Width * (range.MaxX - range.MinX), height / rect.Height * (range.MaxY -range.MinY));
        }
    }
}

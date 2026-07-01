using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Numerics;
using System.Text;

namespace Veldrid.Common.Tools
{
    internal static class ShapeFillHelper
    {
        /// <summary>
        /// 使用点来填充矩形
        /// </summary>
        /// <param name="rectangle">矩形位置和大小</param>
        /// <param name="intervalx">点之间的水平间隔</param>
        /// <param name="intervaly">点之间的垂直间隔</param>
        /// <returns>填充点集合</returns>
        internal static IEnumerable<Vector2> FillPoint(RectangleF rectangle, float intervalx, float intervaly)
        {
            List<Vector2> result = new List<Vector2>();
            var minx = rectangle.Location.X;
            var maxx = rectangle.Location.X + rectangle.Width;
            var miny = rectangle.Location.Y - rectangle.Height;
            var maxy = rectangle.Location.Y;
            for (var x = minx + intervalx; x < maxx; x += intervalx)
            {
                for (var y = maxy - intervaly; y > miny; y -= intervaly)
                {
                    result.Add(new Vector2(x, y));
                }
            }
            return result;
        }

        /// <summary>
        /// 使用线条来填充矩形
        /// </summary>
        /// <param name="rectangle">待填充的矩形</param>
        /// <param name="margion">填充项之间的margion</param>
        /// <param name="line_start">填充线起点</param>
        /// <param name="line_end">填充线终点</param>
        /// <param name="padding">填充项之间的padding</param>
        /// <returns>填充点集合</returns>
        internal static IEnumerable<Vector2> FillLine(RectangleF rectangle, float margion, PointF line_start, PointF line_end, float padding = 0f)
        {
            List<Vector2> result = new List<Vector2>();
            if (margion * 2 >= Math.Abs(rectangle.Width) || margion * 2 >= Math.Abs(rectangle.Height))
                return result;

            // 计算真实的可用区域的w、h
            var real_w = rectangle.Width - margion * 2;
            var real_h = rectangle.Height - margion * 2;

            // 可用区域矩形的四个坐标点，后续约束使用
            var real_leftup = new PointF(rectangle.X + margion, rectangle.Y - margion);
            var real_rightup = new PointF(rectangle.X + rectangle.Width - margion, rectangle.Y - margion);
            var real_rightdown = new PointF(rectangle.X + rectangle.Width - margion, rectangle.Y - rectangle.Height + margion);
            var real_leftdown = new PointF(rectangle.X + margion, rectangle.Y - rectangle.Height + margion);

            // 单个填充线条形成的虚拟矩形的宽高
            var rw = Math.Abs(line_end.X - line_start.X);
            var rh = Math.Abs(line_end.Y - line_start.Y);

            // 可用区域至少得能放置一个线条
            if (rw > real_w || rh > real_h)
                return result;

            // 计算水平方向需要多少个填充矩形(start - end组成的矩形)
            var numberofhorizontol = (real_w + padding) / (rw + padding);
            numberofhorizontol = (float)Math.Ceiling(numberofhorizontol);

            // 计算垂直方向上需要多少个填充矩形
            var numberofVertical = (real_h + padding) / (rh + padding);
            numberofVertical = (float)Math.Ceiling(numberofVertical);

            //约束任何的坐标点不能超过可用区域
            void Constraint(ref Vector2 v)
            {
                if (v.X < real_leftup.X)
                {
                    v.X = real_leftup.X;
                }
                else if (v.X > real_rightup.X)
                {
                    v.X = real_rightup.X;
                }

                if (v.Y > real_leftup.Y)
                {
                    v.Y = real_leftup.Y;
                }
                else if (v.Y < real_rightdown.Y)
                {
                    v.Y = real_rightdown.Y;
                }
            }

            // 行列扫描创建
            Vector2 startp, endp;
            for (int i = 0; i < numberofVertical; i++)
            {
                for (int j = 0; j < numberofhorizontol; j++)
                {
                    startp = new Vector2();
                    endp = new Vector2();

                    startp.X = real_leftup.X + j * (rw + padding) + line_start.X;
                    startp.Y = real_leftup.Y + i * (rh + padding) + line_start.Y;

                    endp.X = real_leftup.X + j * (rw + padding) + line_end.X;
                    endp.Y = real_leftup.Y + i * (rh + padding) + line_end.Y;

                    // 约束任何的坐标点不能超过可用区域
                    Constraint(ref startp);
                    Constraint(ref endp);

                    result.Add(startp);
                    result.Add(endp);
                }
            }

            return result;
        }
    }
}

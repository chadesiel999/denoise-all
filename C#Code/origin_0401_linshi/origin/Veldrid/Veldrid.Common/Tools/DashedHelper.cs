using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using Veldrid.Common.Plot;

namespace Veldrid.Common.Tools
{
    internal static class DashedHelper
    {
        public static Vector2[] CalcDashedPoints(IRender render,CursorLineStyle lineStyle,float local,Vector2 imagepositionvirsize,Position imageposition = Position.Left)
        {
            return CalcDashedPoints(render,lineStyle,local,imagepositionvirsize,new DashedStyle(),imageposition);
        }
        public static Vector2[] CalcDashedPoints(IRender render,CursorLineStyle lineStyle,float local,Vector2 imagepositionvirsize,DashedStyle dashedStyle,Position imageposition = Position.Left)
        {
            var position = 0f;
            if (imagepositionvirsize.X == 0 || imagepositionvirsize.Y == 0 || Single.IsNaN(imagepositionvirsize.X) || Single.IsNaN(imagepositionvirsize.Y) || Single.IsInfinity(imagepositionvirsize.X) || Single.IsInfinity(imagepositionvirsize.Y))
            {
                return new Vector2[0];
            }

            var start = new Vector2(0, 0);
            var end = new Vector2(0, 0);
            float range = 0;

            switch (imageposition)
            {
                case Position.Left:
                    position = local;
                    start.X = render.LineRange.MinX + imagepositionvirsize.X + render.Margin.Left;
                    start.Y = position;
                    end.X = render.LineRange.MaxX;
                    end.Y = position;
                    range = end.X - start.X;
                    break;
                case Position.Bottom:
                    position = local;
                    start.Y = render.LineRange.MinY + imagepositionvirsize.Y + render.Margin.Bottom;
                    start.X = position;
                    end.Y = render.LineRange.MaxY;
                    end.X = position;
                    range = end.Y - start.Y;
                    break;
                case Position.Right:
                    position = local;
                    start.X = render.LineRange.MinX;
                    start.Y = position;
                    end.X = render.LineRange.MaxX - imagepositionvirsize.X - render.Margin.Right;
                    end.Y = position;
                    range = end.X - start.X;
                    break;
                case Position.Top:
                    position = local;
                    start.Y = render.LineRange.MinY;
                    start.X = position;
                    end.Y = render.LineRange.MaxY - imagepositionvirsize.Y - render.Margin.Top;
                    end.X = position;
                    range = end.Y - start.Y;
                    break;
            }
            if (range <= 0)
            {
                return new Vector2[0];
            }
            switch (lineStyle)
            {
                case CursorLineStyle.Line:
                    return new Vector2[] { start, end };
                case CursorLineStyle.None:
                default:
                    return new Vector2[0];
                case CursorLineStyle.Dashed:
                    Vector2[] temp = Enumerable.Range(0, (int)Math.Ceiling(range / (dashedStyle.LineLength + dashedStyle.DashedLength))).SelectMany(x =>
                    {
                        Vector2[] temps = new Vector2[2];
                        temps[0] = start;
                        switch (imageposition)
                        {
                            case Position.Left:
                            case Position.Right:
                                temps[1].X = start.X + dashedStyle.LineLength;
                                start.X = temps[1].X + dashedStyle.DashedLength;
                                temps[1].Y = start.Y;
                                break;
                            case Position.Top:
                            case Position.Bottom:
                                temps[1].Y = start.Y + dashedStyle.LineLength;
                                start.Y = temps[1].Y + dashedStyle.DashedLength;
                                temps[1].X = temps[0].X;
                                break;
                        }
                        return temps;

                    }).ToArray();
                    return temp;
            }
        }
    }
}

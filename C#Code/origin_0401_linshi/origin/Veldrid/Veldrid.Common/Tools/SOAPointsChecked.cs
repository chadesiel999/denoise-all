using System;
using System.Collections.Generic;
using System.Text;
using Veldrid.Common.Poly2Tri.Polygons;
using Veldrid.Common.Poly2Tri;
using System.Linq;
using System.Drawing;

namespace Veldrid.Common.Tools
{
    public static class SOAPointsChecked
    {
        public static Boolean Checked(List<(double x, double y)> pointList, List<PointF> trianglespointlist)
        {
            if(pointList == null || pointList.Count<3)
            {
                return false;
            }
            try
            {
                Double xmax = pointList[0].x;
                Double xmin = pointList[0].x;
                Double ymax = pointList[0].y;
                Double ymin = pointList[0].y;


                List<PolygonPoint> polygonPoints = new List<PolygonPoint>();
                foreach (var itempoint in pointList)
                {
                    polygonPoints.Add(new PolygonPoint(itempoint.x, itempoint.y));
                    if(xmax < itempoint.x)
                    {
                        xmax = itempoint.x;
                    }
                    if (xmin > itempoint.x)
                    {
                        xmin = itempoint.x;
                    }

                    if(ymax < itempoint.y)
                    {
                        ymax = itempoint.y;
                    }
                    if (ymin > itempoint.y)
                    {
                        ymin = itempoint.y;
                    }
                }
                Polygon polygon = new Polygon(polygonPoints);
                
                P2T.Triangulate(polygon);
                trianglespointlist = trianglespointlist == null ? new List<PointF>() : trianglespointlist;
                foreach (var item in polygon.Triangles)
                {
                    if (item.Points[0].X > xmax || item.Points[0].X < xmin)
                    {
                        return false;
                    }
                    if (item.Points[1].X > xmax || item.Points[1].X < xmin)
                    {
                        return false;
                    }
                    if (item.Points[2].X > xmax || item.Points[2].X < xmin)
                    {
                        return false;
                    }

                    if (item.Points[0].Y > ymax || item.Points[0].Y < ymin)
                    {
                        return false;
                    }
                    if (item.Points[1].Y > ymax || item.Points[1].Y < ymin)
                    {
                        return false;
                    }
                    if (item.Points[2].Y > ymax || item.Points[2].Y < ymin)
                    {
                        return false;
                    }

                    trianglespointlist.Add(new PointF((float)item.Points[0].X, (float)item.Points[0].Y));
                    trianglespointlist.Add(new PointF((float)item.Points[2].X, (float)item.Points[2].Y));
                    trianglespointlist.Add(new PointF((float)item.Points[1].X, (float)item.Points[1].Y));
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

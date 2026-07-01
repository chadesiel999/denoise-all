using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Common.Poly2Tri;
using Veldrid.Common.Poly2Tri.Polygons;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.Plot
{
    public class PolygonPlot : BasePlot
    {
        private MutiText veldridText;
        private DataRender dataRender;
        private protected override BaseVeldridRender Renderer => dataRender;
        public MutiCursorTextConfig TextConfig { get; } = new MutiCursorTextConfig();
        public PolygonPlot(IVeldridContent control) : base(control)
        {
            dataRender = new DataRender(control, 10000);
            dataRender.DataRenderConfigs = new DataRenderConfig[]
            {
                new DataRenderConfig()
                {
                    PointConfigs = new PointConfig[]
                    {
                        new PointConfig()
                        {
                            PointCounts = new PointVisibily[]{0 },
                        },
                    },
                    Primitive = PrimitiveTopology.TriangleList,
                },
            };
            dataRender.CreateResources();

            veldridText = new MutiText(control, true,true); 
            veldridText.TextInfos = new TextInfo[1]
            {
                new TextInfo(),
            };
            veldridText.CreateResources();
            (this as IDropRender).Children.Add(veldridText);
            TextConfig.PropertyChanged += (sender, args) =>
            {
                SetTextInfo(args);
            };
        }
        private void SetTextInfo(string propertyName)
        {
            Int32 index = 0;
            switch (propertyName)
            {
                case nameof(MutiCursorTextConfig.Visibily):
                    veldridText.TextInfos[index].Visibily = TextConfig.Visibily;
                    break;
                case nameof(MutiCursorTextConfig.Text):
                    veldridText.TextInfos[index].Text = TextConfig.Text;
                    break;
                case nameof(MutiCursorTextConfig.Vertical):
                    veldridText.TextInfos[index].VerticalAlignment = TextConfig.Vertical;
                    break;
                case nameof(MutiCursorTextConfig.Horizontal):
                    veldridText.TextInfos[index].HorizontalAlignment = TextConfig.Horizontal;
                    break;
                case nameof(MutiCursorTextConfig.Color):
                    veldridText.TextInfos[index].Color = TextConfig.Color;
                    break;
                case nameof(MutiCursorTextConfig.BackColor):
                    veldridText.TextInfos[index].BackColor = TextConfig.BackColor;
                    break;
                case nameof(MutiCursorTextConfig.FixSize):
                    veldridText.TextInfos[index].FixBackColorSize = TextConfig.FixSize;
                    break;
                case nameof(MutiCursorTextConfig.Local):
                    veldridText.TextInfos[index].Local = TextConfig.Local;
                    break;
            }
        }
        public System.Drawing.Color Color
        {
            get => dataRender.DataRenderConfigs[0].PointConfigs[0].Color;
            set => dataRender.DataRenderConfigs[0].PointConfigs[0].Color = value;
        }
        public override LineRange LineRange
        {
            get => base.LineRange;
            set
            {
                base.LineRange = value;
            }
        }

        public override Padding Margin
        {
            get => base.Margin;
            set
            {
                base.Margin = value;
            }
        }

        public override float Brightness
        {
            get => dataRender.DataRenderConfigs[0].PointConfigs[0].Brightness;
            set => dataRender.DataRenderConfigs[0].PointConfigs[0].Brightness = value;
        }
        public float FontSize { get => veldridText.FontSize; set => veldridText.FontSize = value; }
        private List<List<(double x, double y)>> _Points;
        public List<List<(double x, double y)>> Points
        {
            get => _Points;
            set
            {
                _Points = value;

                if (value == null)
                {
                    dataRender.WriteData(0, new Vector2[0]);
                    dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts = new PointVisibily[] { 0 };
                    dataRender.DataRenderConfigs[0].FixedDataLenght = (uint)0;
                    dataRender.DataRenderConfigs[0].DataLenght = (uint)0;
                    return;
                }
                List<Vector2> vector2 = new List<Vector2>();
                foreach (var item in _Points)
                {
                    if (item.Count < 3)
                        continue;
                    List<PolygonPoint> polygonPoints = new List<PolygonPoint>();
                    foreach (var itempoint in item)
                    {
                        polygonPoints.Add(new PolygonPoint(itempoint.x, itempoint.y));
                    }
                    Polygon polygon = new Polygon(polygonPoints);

                    P2T.Triangulate(polygon);
                    foreach (var p in polygon.Triangles)
                    {
                        vector2.Add(new Vector2((float)p.Points[0].X, (float)p.Points[0].Y));
                        vector2.Add(new Vector2((float)p.Points[2].X, (float)p.Points[2].Y));
                        vector2.Add(new Vector2((float)p.Points[1].X, (float)p.Points[1].Y));
                    }

                    polygon.ClearSteinerPoints();
                    polygon.ClearTriangles();
                }
                dataRender.WriteData(0, vector2.ToArray());
                dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts = new PointVisibily[] { vector2.Count };
                dataRender.DataRenderConfigs[0].FixedDataLenght = (uint)vector2.Count;
                dataRender.DataRenderConfigs[0].DataLenght = (uint)vector2.Count;
            }
        }
        public override void Draw()
        {
            if (!Visibily) return;
            base.Draw();
        }

        public override float HorizontalOffset { get => dataRender.HorizontalOffset; set => dataRender.HorizontalOffset = value; }

        public SizeF? GetVirtualSizeByMutiText(Int32 strindex)
        {
            if (this.veldridText != null && !veldridText.IsDisposed
                && TextConfig != null && TextConfig.Infos != null && TextConfig.Infos.Length> strindex)
            {
                var vector2 = this.veldridText.GetVirtualSize(TextConfig.Infos[strindex]);
                return new SizeF(vector2.X, vector2.Y);
            }
            else
            {
                return null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Points?.Clear();
            base.Dispose(disposing);
        }
    }
}

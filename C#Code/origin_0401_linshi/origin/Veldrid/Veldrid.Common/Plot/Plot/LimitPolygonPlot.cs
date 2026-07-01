using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.LineRender;

namespace Veldrid.Common.Plot
{
    public class LimitPolygonPlot : BasePlot
    {
        private DataRender dataRender;
        private protected override BaseVeldridRender Renderer => dataRender;
        public LimitPolygonPlot(IVeldridContent control) : base(control)
        {
            dataRender = new DataRender(control, 4000000);
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

        public System.Drawing.Color Color
        {
            get => dataRender.DataRenderConfigs[0].PointConfigs[0].Color;
            set => dataRender.DataRenderConfigs[0].PointConfigs[0].Color = value;
        }
        public override float Brightness
        {
            get => dataRender.DataRenderConfigs[0].PointConfigs[0].Brightness;
            set => dataRender.DataRenderConfigs[0].PointConfigs[0].Brightness = value;
        }
        public void SetSegment(IReadOnlyList<double> topSegment, IReadOnlyList<double> btmSegment)
        {
            if (topSegment == null || btmSegment == null)
            {
                dataRender.WriteData(0, new Vector2[0]);
                dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts = new PointVisibily[] { 0 };
                dataRender.DataRenderConfigs[0].FixedDataLenght = (uint)0;
                dataRender.DataRenderConfigs[0].DataLenght = (uint)0;
                return;
            }
            if (topSegment.Count < 2 || btmSegment.Count < 2)
            {
                dataRender.WriteData(0, new Vector2[0]);
                dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts = new PointVisibily[] { 0 };
                dataRender.DataRenderConfigs[0].FixedDataLenght = (uint)0;
                dataRender.DataRenderConfigs[0].DataLenght = (uint)0;
                return;
            }
            var columns = Enumerable.Range(0, topSegment.Count());
            List<(float x, float y)> ps = columns.Select(x => ((float)(HorizontalOffset + x / SampleRate), (float)topSegment[x])).ToList();

            List<Vector2> temp = new List<Vector2>();

            if (ps.Any(point => point.x == Renderer.Range.MaxX) == false)
            {
                ps.Add((ps[ps.Count - 1].x, (Renderer.Range.MinY + Renderer.Range.MaxY) / 2));
                ps.Add((Renderer.Range.MaxX, (Renderer.Range.MinY + Renderer.Range.MaxY) / 2));
            }
            if (ps.Any(x => x.x == Renderer.Range.MinX) == false)
            {
                ps.Add((Renderer.Range.MinX, (Renderer.Range.MinY + Renderer.Range.MaxY) / 2));
                ps.Add((ps[0].x, (Renderer.Range.MinY + Renderer.Range.MaxY) / 2));
            }
            ps.Sort((a, b) =>
            {
                if (a.x > b.x || (a.x == b.x && a.y > b.y))
                {
                    return 1;
                }
                return -1;
            });
            for (int i = 0; i < ps.Count - 1; i++)
            {
                temp.Add(new Vector2(ps[i].x, Renderer.Range.MaxY));
                temp.Add(new Vector2(ps[i + 1].x, Renderer.Range.MaxY));
                temp.Add(new Vector2(ps[i].x, ps[i].y));
                temp.Add(new Vector2(ps[i].x, ps[i].y));
                temp.Add(new Vector2(ps[i + 1].x, Renderer.Range.MaxY));
                temp.Add(new Vector2(ps[i + 1].x, ps[i + 1].y));
            }

            var btmcolumns = Enumerable.Range(0, btmSegment.Count());
            List<(float x, float y)> btmps = btmcolumns.Select(x => ((float)(HorizontalOffset + x / SampleRate), (float)btmSegment[x])).ToList();
            if (btmps.Any(x => x.x == Renderer.Range.MaxX) == false)
            {
                btmps.Add((btmps[btmps.Count - 1].x, (Renderer.Range.MinY + Renderer.Range.MaxY) / 2));
                btmps.Add((Renderer.Range.MaxX, (Renderer.Range.MinY + Renderer.Range.MaxY) / 2));
            }
            if (btmps.Any(x => x.x == Renderer.Range.MinX) == false)
            {
                btmps.Add((Renderer.Range.MinX, (Renderer.Range.MinY + Renderer.Range.MaxY) / 2));
                btmps.Add((btmps[0].x, (Renderer.Range.MinY + Renderer.Range.MaxY) / 2));
            }

            btmps.Sort((a, b) =>
            {
                if (a.x > b.x || (a.x == b.x && a.y > b.y))
                {
                    return 1;
                }
                return -1;
            });
            for (int i = 0; i < btmps.Count - 1; i++)
            {
                temp.Add(new Vector2(btmps[i].x, Renderer.Range.MinY));
                temp.Add(new Vector2(btmps[i].x, btmps[i].y));
                temp.Add(new Vector2(btmps[i + 1].x, btmps[i + 1].y));
                temp.Add(new Vector2(btmps[i].x, Renderer.Range.MinY));
                temp.Add(new Vector2(btmps[i + 1].x, btmps[i + 1].y));
                temp.Add(new Vector2(btmps[i + 1].x, Renderer.Range.MinY));
            }
            dataRender.WriteData(0, temp.ToArray());
            dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts = new PointVisibily[] { temp.Count };
            dataRender.DataRenderConfigs[0].FixedDataLenght = (uint)temp.Count;
            dataRender.DataRenderConfigs[0].DataLenght = (uint)temp.Count;
        }

        public override void Draw()
        {
            if (!Visibily) return;
            base.Draw();
        }

        public override float HorizontalOffset { get; set; }// { get => dataRender.HorizontalOffset; set => dataRender.HorizontalOffset = value; }

        public double SampleRate { get; set; }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

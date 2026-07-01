using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender;
using System.Drawing;
using System.Numerics;

namespace Veldrid.Common.Plot
{
    public class FailPlot : BasePlot
    {
        private DataRender dataRender;
        private protected override BaseVeldridRender Renderer => dataRender;
        public FailPlot(IVeldridContent control) : base(control)
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
                            Color = Color.Red,
                        },
                    },
                    Primitive = PrimitiveTopology.PointList,
                },
                new DataRenderConfig()
                {
                      PointConfigs = new PointConfig[]
                    {
                        new PointConfig()
                        {
                            PointCounts = new PointVisibily[]{0 },
                            Color = Color.Red,
                        },
                    },
                    Primitive = PrimitiveTopology.LineList,
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
        private List<(double x, double y)> _Hits;
        public List<(double x, double y)> Hits
        {
            get => _Hits;
            set
            {
                _Hits = value;
                if (value != null)
                {
                    var hitlist = Hits.Select(x => { return new Vector2((float)(x.x), (float)x.y); });

                    Int32 count = hitlist.Count();
                    dataRender.WriteData(0, hitlist.ToArray());
                    dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts = new PointVisibily[] { count };
                    dataRender.DataRenderConfigs[0].FixedDataLenght = (uint)count;
                    dataRender.DataRenderConfigs[0].DataLenght = (uint)count;
                }
                else
                {
                    dataRender.WriteData(0, new Vector2[0]);
                    dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts = new PointVisibily[] { 0 };
                    dataRender.DataRenderConfigs[0].FixedDataLenght = (uint)0;
                    dataRender.DataRenderConfigs[0].DataLenght = (uint)0;
                }
            }
        }
        private List<(double x, double y)> _LineHits;
        public List<(double x, double y)> LineHits
        {
            get => _LineHits;
            set
            {
                _LineHits = value;
                if (value != null)
                {
                    var hitlist = _LineHits.Select(x => { return new Vector2((float)(x.x), (float)x.y); });

                    Int32 count = hitlist.Count();
                    dataRender.WriteData(dataRender.DataRenderConfigs[0].DataLenght, hitlist.ToArray());
                    dataRender.DataRenderConfigs[1].PointConfigs[0].PointCounts = new PointVisibily[] { count };
                    dataRender.DataRenderConfigs[1].FixedDataLenght = (uint)count;
                    dataRender.DataRenderConfigs[1].DataLenght = (uint)count;
                }
                else
                {
                    dataRender.WriteData(dataRender.DataRenderConfigs[0].DataLenght, new Vector2[0]);
                    dataRender.DataRenderConfigs[1].PointConfigs[0].PointCounts = new PointVisibily[] { 0 };
                    dataRender.DataRenderConfigs[1].FixedDataLenght = (uint)0;
                    dataRender.DataRenderConfigs[1].DataLenght = (uint)0;
                }
            }
        }
        private LineStyle lineStyle = LineStyle.Line;

        public override LineStyle LineStyle
        {
            get => lineStyle;
            set
            {
                if (lineStyle != value)
                {
                    lineStyle = value;
                    if(value == LineStyle.Line)
                    {
                        dataRender.DataRenderConfigs[1].Primitive = PrimitiveTopology.LineList;
                    }
                    else
                    {
                        dataRender.DataRenderConfigs[1].Primitive = PrimitiveTopology.PointList;
                    }
                }
            }
        }
        public override void Draw()
        {
            if (!Visibily) return;
            base.Draw();
        }

        public override float HorizontalOffset { get => dataRender.HorizontalOffset; set => dataRender.HorizontalOffset = value; }

        protected override void Dispose(bool disposing)
        {
            Hits?.Clear();
            LineHits?.Clear();
            base.Dispose(disposing);
        }
    }
}

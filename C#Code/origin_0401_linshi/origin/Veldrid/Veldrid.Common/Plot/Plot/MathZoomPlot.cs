using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Common;
using Veldrid.Common.Plot;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.LineRender;

namespace Veldrid.Common.Plot
{
    public class MathZoomPlot : BasePlot
    {
        private DataRender dataRender;
        private protected override BaseVeldridRender Renderer => dataRender;

        public MathZoomPlot(IVeldridContent control) : base(control)
        {
            dataRender = new DataRender(control, 20);
            dataRender.DataRenderConfigs = new DataRenderConfig[]
            {
                new DataRenderConfig()
                {
                    PointConfigs = new PointConfig[]
                    {
                        new PointConfig()
                        {
                            PointCounts = new PointVisibily[]{0 },
                            Brightness =4,
                        },
                    },
                    Primitive = PrimitiveTopology.TriangleStrip,
                },
                new DataRenderConfig()
                {
                      PointConfigs = new PointConfig[]
                    {
                        new PointConfig()
                        {
                            PointCounts = new PointVisibily[]{0 },
                            Color = Color.Red,
                            Brightness = 100,
                        },
                    },
                    Primitive = PrimitiveTopology.LineList,
                },
            };
            dataRender.CreateResources();
        }
        private Color _BackColor;
        public Color BackColor
        {
            get
            {
                return _BackColor;
            }
            set
            {
                _BackColor = value;
                dataRender.DataRenderConfigs[0].PointConfigs[0].Color = value;// Color.FromArgb(10, value);
                dataRender.DataRenderConfigs[1].PointConfigs[0].Color = value;// Color.FromArgb(255, value);
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

        public void SetAreaBoundaryOnly(float xMin, float xMax)
        {
            Vector2[] vector2 = new Vector2[4];

            vector2[0] = new Vector2(xMin, Renderer.Range.MaxY - 100);
            vector2[1] = new Vector2(xMax, Renderer.Range.MaxY - 100);
            vector2[2] = new Vector2(xMin, Renderer.Range.MinY + 50);
            vector2[3] = new Vector2(xMax, Renderer.Range.MinY + 50);

            Vector2[] linevector2 = new Vector2[12];

            linevector2[0] = new Vector2(xMin, Renderer.Range.MaxY - 50);
            linevector2[1] = new Vector2(xMin + 50, Renderer.Range.MaxY - 50);
            linevector2[2] = new Vector2(xMin, Renderer.Range.MaxY - 50);
            linevector2[3] = new Vector2(xMin, Renderer.Range.MinY + 50);
            linevector2[4] = new Vector2(xMin, Renderer.Range.MinY + 50);
            linevector2[5] = new Vector2(xMin + 50, Renderer.Range.MinY + 50);
            linevector2[6] = new Vector2(xMax - 50, Renderer.Range.MaxY - 50);
            linevector2[7] = new Vector2(xMax, Renderer.Range.MaxY - 50);
            linevector2[8] = new Vector2(xMax, Renderer.Range.MaxY - 50);
            linevector2[9] = new Vector2(xMax, Renderer.Range.MinY + 50);
            linevector2[10] = new Vector2(xMax - 50, Renderer.Range.MinY + 50);
            linevector2[11] = new Vector2(xMax, Renderer.Range.MinY + 50);

            dataRender.WriteData(0, vector2);
            dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts = new PointVisibily[] { 4 };
            dataRender.DataRenderConfigs[0].FixedDataLenght = 4;
            dataRender.DataRenderConfigs[0].DataLenght = 4;

            dataRender.WriteData(dataRender.DataRenderConfigs[0].DataLenght, linevector2);
            dataRender.DataRenderConfigs[1].PointConfigs[0].PointCounts = new PointVisibily[] { 12 };
            dataRender.DataRenderConfigs[1].FixedDataLenght = 12;
            dataRender.DataRenderConfigs[1].DataLenght = 12;
        }
        public override void Draw()
        {
            if (!Visibily) return;
            base.Draw();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}

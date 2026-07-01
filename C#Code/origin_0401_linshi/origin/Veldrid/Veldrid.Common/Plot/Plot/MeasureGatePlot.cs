using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Veldrid.Common;
using Veldrid.Common.Plot;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.Plot
{
    public class MeasureGatePlot : BasePlot
    {
        private DataRender dataRender;
        private protected override BaseVeldridRender Renderer => dataRender;
        //12组阴影
        public MeasureGatePlot(IVeldridContent control) : base(control)
        {
            dataRender = new DataRender(control, 400);
            dataRender.DataRenderConfigs = new DataRenderConfig[]
            {
                new DataRenderConfig()
                {
                    PointConfigs = new PointConfig[]
                    {
                        new PointConfig()
                        {
                            PointCounts = new PointVisibily[]{0 },
                            Brightness =14,
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
                            Brightness = 14,
                        },
                    },
                    Primitive = PrimitiveTopology.PointList,
                },
            };
            dataRender.CreateResources();

            veldridText = new MutiText(control, true);
            veldridText.TextInfos = new TextInfo[11] {
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo(),
                new TextInfo()
            };
            Text = new MutiCursorTextConfig[11] {
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig(),
                new MutiCursorTextConfig()
            };

            veldridText.CreateResources();
            (this as IDropRender).Children.Add(veldridText);
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

            vector2[0] = new Vector2(xMin, Renderer.Range.MaxY);
            vector2[1] = new Vector2(xMax, Renderer.Range.MaxY);
            vector2[2] = new Vector2(xMin, Renderer.Range.MinY);
            vector2[3] = new Vector2(xMax, Renderer.Range.MinY);

            dataRender.WriteData(0, vector2);
            dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts = new PointVisibily[] { 4 };
            dataRender.DataRenderConfigs[0].FixedDataLenght = 4;
            dataRender.DataRenderConfigs[0].DataLenght = 4;
        }

        public void SetAllAreaBoundary(List<(float xLeft, float xRight)> values)
        {
            if (values.Count < 1)
            {
                dataRender.WriteData(0, new Vector2[0]);
                dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts = new PointVisibily[] { 0 };
                dataRender.DataRenderConfigs[0].FixedDataLenght = (uint)0;
                dataRender.DataRenderConfigs[0].DataLenght = (uint)0;
                return;
            }

            Vector2[] vector2 = new Vector2[4 * values.Count];
            
            for (int i = 0; i < values.Count; i++)
            {
                vector2[4 * i + 0] = new Vector2(values[i].xLeft, Renderer.Range.MaxY);
                vector2[4 * i + 1] = new Vector2(values[i].xRight, Renderer.Range.MaxY);
                vector2[4 * i + 2] = new Vector2(values[i].xLeft, Renderer.Range.MinY);
                vector2[4 * i + 3] = new Vector2(values[i].xRight, Renderer.Range.MinY);

                dataRender.WriteData(0, vector2);
                dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts = new PointVisibily[] { vector2.Length };
                dataRender.DataRenderConfigs[0].FixedDataLenght = (uint)vector2.Length;
                dataRender.DataRenderConfigs[0].DataLenght = (uint)vector2.Length;
            }
            
        }
        public override void Draw()
        {
            if (!Visibily) return;
            base.Draw();
        }

        protected override void Dispose(bool disposing)
        {
            for (int i = 0; i < Text.Length; i++)
            {
                Text[i].Dispose();
            }
            base.Dispose(disposing);
        }

        private MutiText veldridText;
        public MutiCursorTextConfig[] Text { get; } = new MutiCursorTextConfig[11];
        public (float position, String infos)[] MeasResultInfo
        {
            set
            {
                _Position = value.Select(o => o.position).ToArray();
                for (int i = 0; i < value.Length; i++)
                {
                    veldridText.TextInfos[i].Text = value[i].infos;
                }
                for (int i = 0; i < veldridText.TextInfos.Length; i++)
                {
                    if (i < value.Length)
                    {
                        veldridText.TextInfos[i].Text = value[i].infos;
                    }
                    else
                    {
                        veldridText.TextInfos[i].Text = "";
                    }

                }
                CalcTextPosition(value);
            }
        }

        public Color TextBackColor
        {
            set
            {
                for (int i = 0; i < veldridText.TextInfos.Length; i++)
                {
                    veldridText.TextInfos[i].BackColor = value;
                }
            }
        }
        private float[] _Position = new float[11];
        public float[] Position
        {
            set
            {
                _Position = value;
            }
        }
        private float _Offset = 500;
        private void CalcTextPosition((float position, String infos)[] values)
        {
            var point = new PointF();
            for (int i = 0; i < Text.Length; i++)
            {
                if (i < values.Length)
                {
                    var mtextsize = veldridText.GetVirtualSize(veldridText.TextInfos[i].Text);
                    var xOffset = _Position[i] /*- mtextsize.X */;
                    point.X = xOffset;
                    point.Y = LineRange.MaxY - _Offset;
                    if (Renderer.Range.MaxX - xOffset < mtextsize.X && _Position[i] < Renderer.Range.MaxX)
                    {
                        point.X = Renderer.Range.MaxX - mtextsize.X;
                    }

                    veldridText.TextInfos[i].Local = point;
                }

            }

        }
    }
}

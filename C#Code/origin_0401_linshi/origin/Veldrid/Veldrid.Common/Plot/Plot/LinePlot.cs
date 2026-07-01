using System.Drawing;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.LineRender;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;
using System;

namespace Veldrid.Common.Plot
{
    public class LinePlot : BasePlot
    {
        private DataRender _DataRender;
        public LinePlot(IVeldridContent control) : base(control)
        {
            _DataRender = new DataRender(control);
            _DataRender.DataRenderConfigs = new DataRenderConfig[]
          {
                new DataRenderConfig()
                {
                    PointConfigs = new PointConfig[]
                    {
                        new PointConfig()
                        {
                            PointCounts = new PointVisibily[]{2 },
                            Brightness=50
                        },
                    },
                    Primitive = PrimitiveTopology.LineList,
                },
          };
            _DataRender.CreateResources();
        }

        private protected override BaseVeldridRender Renderer => _DataRender;

        public Color Color
        {
            get => _DataRender.DataRenderConfigs[0].PointConfigs[0].Color;
            set => _DataRender.DataRenderConfigs[0].PointConfigs.ToList().ForEach(config => config.Color = value);
        }

        public override float Brightness
        {
            get => _DataRender.DataRenderConfigs[0].PointConfigs[0].Brightness;
            set => _DataRender.DataRenderConfigs[0].PointConfigs.ToList().ForEach(config => config.Brightness = value);
        }

        private Position _LinePosition = Position.Left;
        public Position LinePosition
        {
            get => _LinePosition;
            set
            {
                if (_LinePosition != value)
                {
                    switch (value)
                    {
                        case Position.Top:
                        case Position.Bottom:
                            _LinePosition = Position.Top;
                            break;
                        case Position.Left:
                        case Position.Right:
                            _LinePosition = Position.Left;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private float[] _Points;
        public float[] Points
        {
            get => _Points;
            set
            {
                if (_Points != value)
                {
                    _Points = value;
                    UpdateRenderPoints();
                }
            }
        }

        private void UpdateRenderPoints()
        {
            var vertexrects = new List<Vector2>();
            _Points.ToList().ForEach(point =>
            {
                if (LinePosition == Position.Left)
                {
                    vertexrects.Add(new Vector2(LineRange.MinX, point));
                    vertexrects.Add(new Vector2(LineRange.MaxX, point));
                }
                else if (LinePosition == Position.Top)
                {
                    vertexrects.Add(new Vector2(point, LineRange.MaxY));
                    vertexrects.Add(new Vector2(point, LineRange.MinY));
                }
            }
            );

            _DataRender.WriteData(0, vertexrects.ToArray());
            if (_DataRender.DataRenderConfigs[0].PointConfigs.Count() != Points.Count())
            {

                _DataRender.DataRenderConfigs[0].PointConfigs = new PointConfig[_Points.Count()];
                for (var i = 0; i < _DataRender.DataRenderConfigs[0].PointConfigs.Length; i++)
                {
                    _DataRender.DataRenderConfigs[0].PointConfigs[i] = new PointConfig()
                    {
                        PointCounts = new PointVisibily[] { 0 },
                        Brightness = 50,
                        Color = Color.White,
                    };
                    _DataRender.DataRenderConfigs[0].PointConfigs[i].PointCounts = new PointVisibily[] { 2 };
                }
            }

            _DataRender.DataRenderConfigs[0].FixedDataLenght = (uint)(vertexrects.ToArray().Length);
            _DataRender.DataRenderConfigs[0].DataLenght = (uint)(vertexrects.ToArray().Length);

        }

    }
}

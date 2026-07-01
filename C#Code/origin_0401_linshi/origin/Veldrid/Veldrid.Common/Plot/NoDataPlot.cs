using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using Veldrid.Common.Tools;
using Veldrid.Common.VeldridRender;

namespace Veldrid.Common.Plot
{
    internal class NoDataPlot :BaseRender
    {
        private VeldridRender.TextRender.VeldridText veldridText;
        private VeldridRender.LineRender.DataRender dataRender;
        private Vector2[] _Points = new Vector2[4];
        public NoDataPlot(IVeldridContent control) : base(control)
        {
            veldridText = new VeldridRender.TextRender.VeldridText(control, true);
            veldridText.CreateResources();
            veldridText.Local = new PointF((LineRange.MaxX + LineRange.MinX) / 2, 0);
            veldridText.FontStyle = "Semibold";
            veldridText.FontSize = 16;
            veldridText.VerticalAlignment = VeldridRender.TextRender.VerticalAlignment.Center;
            veldridText.HorizontalAlignment = VeldridRender.TextRender.HorizontalAlignment.Center;
            veldridText.Text = "No Data";

            dataRender = new VeldridRender.LineRender.DataRender(control, 4);
            dataRender.DataRenderConfigs = new VeldridRender.LineRender.DataRenderConfig[]
            {
                new VeldridRender.LineRender.DataRenderConfig()
                {
                    PointConfigs = new VeldridRender.LineRender.PointConfig[]
                    {
                        new VeldridRender.LineRender.PointConfig()
                        {
                            Brightness = 100,
                            PointCounts = new VeldridRender.LineRender.PointVisibily[1]{4 },
                        }
                    },
                    DataLenght =4,
                    FixedDataLenght =4,
                    Primitive = PrimitiveTopology.LineList,
                }
            };
            dataRender.Visibily = true;
            dataRender.CreateResources();
            (this as IRender).Children.Add(dataRender);
            CalcPoints();

        }
        private protected override BaseVeldridRender Renderer => veldridText;
        public String Text
        {
            get => veldridText.Text;
            set
            {
                if (veldridText.Text != value && !String.IsNullOrEmpty(value))
                {
                    veldridText.Text = value;
                    CalcPoints();
                }
            }
        }
        public String FontStyle { get => veldridText.FontStyle; set => veldridText.FontStyle = value; }
        public String FontName { get => veldridText.FontName; set => veldridText.FontName = value; }
        public Single FontSize { get => veldridText.FontSize; set => veldridText.FontSize = value; }

        public Color Color
        {
            get => veldridText.Color;
            set
            {
                veldridText.Color = value;
                dataRender.DataRenderConfigs[0].PointConfigs[0].Color = value;
            }
        }
        private Single _Local = 0;

        public float ZoomY = 1.0f;

        public float Local
        {
            get => _Local;
            set
            {
                if (_Local != value)
                {
                    float temp = value;
                    if (value > LineRange.MaxY && ZoomY == 1.0)
                    {
                        temp = LineRange.MaxY;
                    }
                    if (value < LineRange.MinY && ZoomY == 1.0)
                    {
                        temp = LineRange.MinY;
                    }
                    veldridText.Local = new PointF((LineRange.MaxX + LineRange.MinX) / 2, temp);
                    for (Int32 index = 0; index < _Points.Length; index++)
                    {
                        _Points[index].Y = temp;
                    }
                    dataRender.WriteData(0, _Points);
                    _Local = temp;
                }
            }
        }
        private void CalcPoints()
        {
            var nodatasize = this.LocalSizeToVirtualSize(veldridText.MeasureSize(veldridText.Text));
            _Points[0] = new Vector2(LineRange.MinX, Local);
            _Points[1] = new Vector2(LineRange.MinX + (LineRange.XLenght - nodatasize.X - 100) / 2, Local);
            _Points[2] = new Vector2(_Points[1].X + 100 + nodatasize.X, Local);
            _Points[3] = new Vector2(LineRange.MaxX, Local);
            dataRender.WriteData(0, _Points);
        }
    }
}

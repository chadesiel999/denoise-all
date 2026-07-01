using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Common;
using Veldrid.Common.Plot;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Windows.Plot
{
    public class LinePlotXY : BasePlot
    {
        private DataRender dataRender;
        private protected override BaseVeldridRender Renderer => dataRender;
        public CursorPlot YCursorPlot { get; set; }
        public CursorPlot XCursorPlot { get; set; }
        [AllowNull]
        public CursorPlot CurrentCursorPlot { get; set; }
        private VeldridText veldridText;
        public LinePlotXY(IVeldridContent control,int maxDataCount=10000) : base(control)
        {
            dataRender = new DataRender(control, maxDataCount);
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
                    Primitive = PrimitiveTopology.LineStrip,
                },
            };
            dataRender.CreateResources();

            YCursorPlot = new CursorPlot(control);
            YCursorPlot.MouseDownRegion = TouchRegion.Cursor;
            YCursorPlot.MouseUpRegion = TouchRegion.CursorAndLine;
            YCursorPlot.HorizontalAlignment = HorizontalAlignment.Left;
            YCursorPlot.VerticalAlignment = VerticalAlignment.Center;
            YCursorPlot.Position = Position.Left;
            cursors.Add(YCursorPlot);
            XCursorPlot = new CursorPlot(control);
            XCursorPlot.MouseDownRegion = TouchRegion.Cursor;
            XCursorPlot.MouseUpRegion = TouchRegion.CursorAndLine;
            XCursorPlot.LineRange = Renderer.Range;
            XCursorPlot.HorizontalAlignment = HorizontalAlignment.Center;
            XCursorPlot.VerticalAlignment = VerticalAlignment.Bottom;
            XCursorPlot.Position = Position.Bottom;
            cursors.Add(XCursorPlot);

            veldridText = new VeldridText(control);
            veldridText.Text = "";
            veldridText.CreateResources();
            (this as IRender).Children.Add(veldridText);
        }
        public System.Drawing.Color Color
        {
            get => dataRender.DataRenderConfigs[0].PointConfigs[0].Color;
            set
            {
                dataRender.DataRenderConfigs[0].PointConfigs[0].Color = value;
                veldridText.Color = value;
            }
        }
        public override float Brightness
        {
            get => dataRender.DataRenderConfigs[0].PointConfigs[0].Brightness;
            set
            {
                dataRender.DataRenderConfigs[0].PointConfigs[0].Brightness = value;
            }
        }
        public override string Label { get => veldridText.Text; set => veldridText.Text = value; }
        public override String FontName { get => veldridText.FontName; set => veldridText.FontName = value; }
        public override String FontStyle { get => veldridText.FontStyle; set => veldridText.FontStyle = value; }
        public override float FontSize { get => veldridText.FontSize; set => veldridText.FontSize = value; }
        public override bool LabelVisibility { get => veldridText.Visibily; set => veldridText.Visibily = value; }

        public void UpdatePoints(Double[,] xPoints, Double[,] yPoints)
        {
            Int32 ynum = yPoints.GetLength(1);
            Int32 xnum = xPoints.GetLength(1);

            Int32 num = ynum > xnum ? xnum : ynum;
            Vector2[] linePoints = new Vector2[num];
            for (int i = 0; i < num; i++)
            {
                linePoints[i] = new Vector2((float)xPoints[0, i] + 5000, (float)yPoints[0, i]);
            }
            dataRender.WriteData(0, linePoints);
            dataRender.DataRenderConfigs[0].PointConfigs[0].PointCounts = new PointVisibily[] { linePoints.Length };
            dataRender.DataRenderConfigs[0].FixedDataLenght = (uint)linePoints.Length;
            dataRender.DataRenderConfigs[0].DataLenght = (uint)linePoints.Length;
        }
        public override void Draw()
        {
            if (!Visibily) return;
            base.Draw();
        }
        private LineStyle lineStyle = LineStyle.Line;

        public override LineStyle LineStyle
        {
            get { return lineStyle; }
            set
            {
                lineStyle = value;
                dataRender.DataRenderConfigs[0].Primitive = value == LineStyle.Line ? PrimitiveTopology.LineStrip : PrimitiveTopology.PointList;
            }
        }
        public override float VerticalOffset
        {
            get => YCursorPlot.Local;
            set
            {
                if (value != YCursorPlot.Local)
                {
                    if (value >= LineRange.MinY && value <= LineRange.MaxY)
                    {
                        YCursorPlot.Local = value;
                    }
                    else if (value < LineRange.MinY)
                    {
                        YCursorPlot.Local = LineRange.MinY + YCursorPlot.GetVirtualSize().Y / 2;
                    }
                    else
                    {
                        YCursorPlot.Local = LineRange.MaxY - YCursorPlot.GetVirtualSize().Y / 2;
                    }
                    CalcLabelPosition();
                    OnVerticalOffsetChanged(value);
                }
            }
        }
        public float XVerticalOffset
        {
            get => XCursorPlot.Local;
            set
            {
                if (value != XCursorPlot.Local)
                {
                    if (value >= LineRange.MinX && value <= LineRange.MaxX)
                    {
                        XCursorPlot.Local = value;
                    }
                    else if (value < LineRange.MinX)
                    {
                        XCursorPlot.Local = LineRange.MinX + XCursorPlot.GetVirtualSize().X/2;
                    }
                    else
                    {
                        XCursorPlot.Local = LineRange.MaxX - XCursorPlot.GetVirtualSize().X/2;
                    }
                }
            }
        }
        private void CalcLabelPosition()
        {
            var size = YCursorPlot.GetVirtualSize();
            veldridText.Local = new PointF(size.X, YCursorPlot.Local - size.Y / 4);
        }
        public override float HorizontalOffset { get => dataRender.HorizontalOffset; set => dataRender.HorizontalOffset = value; }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Veldrid.Common.Plot.Plot;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.VeldridRender.LineRender;
using Veldrid.Common.VeldridRender.TextRender;

namespace Veldrid.Common.Plot
{
    public class MathPlot : BasePlot
    {
        private MathPlotRender analogRender;
        private MathPlotRender analogRenderNml;
        private MathPlotRender analogRenderMax;
        private MathPlotRender analogRenderMin;
        private MathPlotRender analogRenderAvg;
        //private VeldridText veldridText;
        private CursorPlot cursorPlot;
        private DashedStyle dashedStyle;
        private NoDataPlot noDataPlot;

        public MathPlot(IVeldridContent control) : base(control)
        {
            dashedStyle = new DashedStyle();
            analogRender = new MathPlotRender(control);
            analogRender.CreateResources();
            analogRenderNml = new MathPlotRender(control);
            analogRenderNml.CreateResources();
            analogRenderMax = new MathPlotRender(control);
            analogRenderMax.CreateResources();
            analogRenderMin = new MathPlotRender(control);
            analogRenderMin.CreateResources();
            analogRenderAvg = new MathPlotRender(control);
            analogRenderAvg.CreateResources();
            //veldridText = new VeldridText(control);
            //veldridText.CreateResources();
            //(this as IRender).Children.Add(veldridText);
            LabelPlot = new LabelPlot(control);

            cursorPlot = new CursorPlot(control);
            cursorPlot.HorizontalAlignment = HorizontalAlignment.Left;
            cursorPlot.VerticalAlignment = VerticalAlignment.Center;
            cursors.Add(cursorPlot);

            noDataPlot = new NoDataPlot(control);
            noDataPlot.Visibily = false;

            Color = analogRender.Color;

            analogRenderNml.Color = Color.Yellow;
            analogRenderMax.Color = Color.Red;
            analogRenderMin.Color = Color.Blue;
            analogRenderAvg.Color = Color.LightGreen;
        }
        public override float Brightness
        {
            get => analogRender.Brightness;
            set
            {
                analogRender.Brightness = value;
                analogRenderNml.Brightness = value;
                analogRenderMax.Brightness = value;
                analogRenderMin.Brightness = value;
                analogRenderAvg.Brightness = value;
            }
        }
        public override String FontName { get => LabelPlot.FontName; set => LabelPlot.FontName = value; }
        public override String FontStyle { get => LabelPlot.FontStyle; set => LabelPlot.FontStyle = value; }
        public override float FontSize { get => LabelPlot.FontSize; set => LabelPlot.FontSize = value; }
        public Color Color
        {
            get => analogRender.Color;
            set
            {
                analogRender.Color = value;
                LabelPlot.Color = value;
                cursorPlot.Color = value;
                noDataPlot.Color = value;
            }
        }
        public Color[] Colors
        {
            get => new Color[4] {analogRender.Color, analogRenderMax.Color, analogRenderMin.Color, analogRenderAvg.Color } ;
            set
            {
                analogRender.Color = value[0];
                analogRenderNml.Color = value[0];
                analogRenderMax.Color = value[1];
                analogRenderMin.Color = value[2];
                analogRenderAvg.Color = value[3];
            }
        }
        public override float VerticalOffset
        {
            get => cursorPlot.Local;
            set
            {
                if (cursorPlot.Local != value)
                {
                    cursorPlot.Local = value;
                    OnVerticalOffsetChanged(value);
                }
                noDataPlot.Local = value;
               // veldridText.Local = new PointF(LineRange.MinX + 168, VerticalOffset - 10);
            }
        }
        public float ValueOffset 
        { 
            get => analogRender.VerticalOffset;
            set { 
                analogRender.VerticalOffset = value; 
                analogRenderNml.VerticalOffset = value;
                analogRenderMax.VerticalOffset = value; 
                analogRenderMin.VerticalOffset = value;
                analogRenderAvg.VerticalOffset = value;
            }
        }
        public CursorLineStyle CursorLineStyle { get => cursorPlot.LineStyle; set => cursorPlot.LineStyle = value; }
        public CursorImageCollection CursorImages => cursorPlot.CursorImages;
        public int CursorIndex { get => cursorPlot.CursorIndex; set => cursorPlot.CursorIndex = value; }
        public override String Label
        {
            get => LabelPlot.Label;
            set => LabelPlot.Label = value;
        }
        public override bool LabelVisibility { get => LabelPlot.Visibily; set => LabelPlot.Visibily = value; }
        public override float HorizontalOffset
        {
            get => analogRender.HorizontalOffset;
            set
            {
                if (analogRender.HorizontalOffset != value)
                {
                    analogRender.HorizontalOffset = value;
                    analogRenderNml.HorizontalOffset = value;
                    analogRenderMax.HorizontalOffset = value;
                    analogRenderMin.HorizontalOffset = value;
                    analogRenderAvg.HorizontalOffset = value;
                    OnHorizontalOffsetChanged(value);
                }
            }
        }
        private protected override BaseVeldridRender Renderer => analogRender;
        public float SampleRate 
        {
            get => analogRender.SampleRate;
            set { 
                analogRender.SampleRate = value; 
                analogRenderNml.SampleRate = value;
                analogRenderMax.SampleRate = value;
                analogRenderMin.SampleRate = value;
                analogRenderAvg.SampleRate = value;
            }
        }
        public override LineStyle LineStyle
        {
            get => base.LineStyle;
            set
            {
                base.LineStyle = value;
                analogRender.PrimitiveTopology = value == LineStyle.Line ? PrimitiveTopology.LineStrip : PrimitiveTopology.PointList;
                analogRenderNml.PrimitiveTopology = value == LineStyle.Line ? PrimitiveTopology.LineStrip : PrimitiveTopology.PointList;
                analogRenderMax.PrimitiveTopology = value == LineStyle.Line ? PrimitiveTopology.LineStrip : PrimitiveTopology.PointList;
                analogRenderMin.PrimitiveTopology = value == LineStyle.Line ? PrimitiveTopology.LineStrip : PrimitiveTopology.PointList;
                analogRenderAvg.PrimitiveTopology = value == LineStyle.Line ? PrimitiveTopology.LineStrip : PrimitiveTopology.PointList;
            }
        }

        public void SetDatas(float[] datas)
        {
            noDataPlot.Visibily = (datas == null || datas.Length == 0);
            if (datas == null || datas.Length == 0) return;
            analogRender?.SetDatas(datas);
            analogRenderNml?.SetDatas(new float[0]);
            analogRenderMax?.SetDatas(new float[0]);
            analogRenderMin?.SetDatas(new float[0]);
            analogRenderAvg?.SetDatas(new float[0]);
        }

        public void SetAllLinesDatas(float[] datasNml, float[] datasMax, float[] datasMin, float[] datasAvg)
        {
            noDataPlot.Visibily = (datasNml == null || datasNml.Length == 0);
            if (datasNml == null || datasNml.Length == 0) return;

            if (datasNml == null || datasNml.Length == 0) return;
            if (datasMax == null || datasMax.Length == 0) return;
            if (datasMin == null || datasMin.Length == 0) return;
            if (datasAvg == null || datasAvg.Length == 0) return;
            analogRender?.SetDatas(new float[0]);
            analogRenderNml?.SetDatas(datasNml);
            analogRenderMax?.SetDatas(datasMax);
            analogRenderMin?.SetDatas(datasMin);
            analogRenderAvg?.SetDatas(datasAvg);
        }

        public void SetVisiblities(Boolean boolNormal, Boolean boolMax, Boolean boolMin, Boolean boolAvg)
        {
            analogRender.Visibily= true;
            analogRenderNml.Visibily= boolNormal;
            analogRenderMax.Visibily = boolMax;
            analogRenderMin.Visibily = boolMin;
            analogRenderAvg.Visibily = boolAvg;
        }

        public override void Draw()
        {
            if (!Visibily) return;
            if(noDataPlot.Visibily)
            {
                noDataPlot?.Draw();
            }
            analogRenderMax.Draw();
            analogRenderMin.Draw();
            analogRenderAvg.Draw();
            analogRenderNml.Draw();
            base.Draw();
        }
        protected override void Dispose(Boolean disposing)
        {
            noDataPlot?.Dispose();
            base.Dispose(disposing);
        }
    }
}

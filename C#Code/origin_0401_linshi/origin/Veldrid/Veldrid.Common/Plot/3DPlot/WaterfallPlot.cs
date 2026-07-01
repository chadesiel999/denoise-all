using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Text;
using Veldrid.Common.VeldridRender;
using Veldrid.Common.Tools;

namespace Veldrid.Common.Plot
{
    public class WaterfallPlot : BasePlot
    {

        private Color minColor;
        private Color middleColor;
        private Color lastColor;
        Veldrid.Common.VeldridRender.LineRender.WaterfallRender render;
        public WaterfallPlot(IVeldridContent control, int chDataLenght = 10000, int maxframeCount = 30) : base(control)
        {
            render = new VeldridRender.LineRender.WaterfallRender(control,chDataLenght,maxframeCount);
            render.CreateResources();
            render.PrimitiveTopology = PrimitiveTopology.LineStrip;
        }

        private protected override BaseVeldridRender Renderer => render;

        public int MaxFrameCount => render.MaxFrameCount;
        public void SetData(float[] data)
        {
            if (data == null || data.Length == 0) return;
            render.SetData(data);
        }
        public int FrameLenght { get => render.FrameLenght; }
        public override float Brightness { get => render.Brightness; set => render.Brightness = value; }
        public int TotalFrameCount => render.TotalFrameCount;
        public Color MinColor
        {
            get => minColor;
            set
            {
                if (minColor != value)
                {
                    minColor = value;
                    render.MinColor = value.ColorConverToRGBA();
                }
            }
        }
        public Color MiddleColor
        {
            get => middleColor;
            set
            {
                if (middleColor != value)
                {
                    middleColor = value;
                    render.MiddleColor = value.ColorConverToRGBA();
                }
            }
        }
        public Color LastColor
        {
            get => lastColor;
            set
            {
                if (lastColor != value)
                {
                    lastColor = value;
                    render.LastColor = value.ColorConverToRGBA();
                }
            }
        }
        public float MinValue { get => render.MinValue; set => render.MinValue = value; }
        public float MiddleValue { get => render.MiddleValue; set => render.MiddleValue = value; }
        public float LastValue { get => render.LastValue; set => render.LastValue = value; }
    }
}

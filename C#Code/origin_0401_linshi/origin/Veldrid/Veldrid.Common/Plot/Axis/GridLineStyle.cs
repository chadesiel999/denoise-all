using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;

namespace Veldrid.Common.Plot
{

    public class GridLineStyle : BaseProperty
    {
        private Color color = Color.White;
        private LineStyle lineStyle = LineStyle.Line;
        private int tick = 10;
        private float step = 1000;

        public Color LabelColor { get; set; } = Color.White;
        public Color Color { get => color; set => Set(ref color, value); }

        public LineStyle LineStyle { get => lineStyle; set => Set(ref lineStyle, value); }


        public int Tick { get => tick; set => Set(ref tick, value); }
        private Color tickColor = Color.White;

        public Color TickColor
        {
            get { return tickColor; }
            set { Set(ref tickColor, value); }
        }

        private TickStyle tickStyle = TickStyle.InSide;

        public TickStyle TickStyle
        {
            get { return tickStyle; }
            set { Set(ref tickStyle, value); }
        }

        public float Step { get => step; set => Set(ref step, value); }
        private float pointOffset = 10;

        public float PointOffset
        {
            get { return pointOffset; }
            set { Set(ref pointOffset, value); }
        }
        private float fontSize = 12;

        public float FontSize
        {
            get { return fontSize; }
            set {Set(ref fontSize, value); }
        }
        private Byte[] fontFile = new Byte[0];

        public Byte[] FontFile
        {
            get { return fontFile; }
            set {Set(ref fontFile,value); }
        }

        [AllowNull]
        public Func<float, String> LabelFormatter { get; set; }
        private Boolean labelVisibily = true;

        public Boolean LabelVisibily
        {
            get { return labelVisibily; }
            set { Set(ref labelVisibily, value); }
        }


    }
}

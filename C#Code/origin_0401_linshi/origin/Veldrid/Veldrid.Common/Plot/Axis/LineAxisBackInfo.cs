using System;
using System.Drawing;

namespace Veldrid.Common.Plot
{
    public class LineAxisBackInfo : BaseProperty
    {
        private bool pointVisibily = true;
        private float pointBrightness = 100;
        private bool crossVisibily = true;
        private float crossBrighness = 100;
        private Color crossColor = Color.White;
        private Color pointColor =Color.White;  

        public Boolean PointVisibily { get => pointVisibily; set => Set(ref pointVisibily, value); }
        public float PointBrightness { get => pointBrightness; set => Set(ref pointBrightness, Math.Clamp(value, 0, 100)); }
        public Boolean CrossVisibily { get => crossVisibily; set => Set(ref crossVisibily, value); }
        public float CrossBrighness { get => crossBrighness; set => Set(ref crossBrighness, Math.Clamp(value, 0, 100)); }
        public Color CrossColor { get => crossColor; set =>Set(ref crossColor,value); }
        public Color PointColor { get => pointColor; set =>Set(ref pointColor,value); }
    }
}

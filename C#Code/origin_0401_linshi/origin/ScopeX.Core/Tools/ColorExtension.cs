using System;
using System.Drawing;

namespace ScopeX.Core.Tools
{
    internal static class ColorExtension
    {
        private static readonly Byte[] _BrightnessTable = new Byte[256];

        public static Color GetGradient(this Color color, Int32 degree)
        {
            Double level = degree / 100.0;
            return Color.FromArgb(
                _BrightnessTable[(Int32)(color.A * level)],
                _BrightnessTable[(Int32)(color.R * level)],
                _BrightnessTable[(Int32)(color.G * level)],
                _BrightnessTable[(Int32)(color.B * level)]);
        }

        private static void CreateBrightnessTable(Int32 blackThreshold, Int32 whiteThreshold, Double gamma)
        {
            System.Diagnostics.Trace.Assert(
                blackThreshold >= 0 && blackThreshold < whiteThreshold && whiteThreshold <= 255,
                "Black and white threshold settings are wrong!");
            System.Diagnostics.Trace.Assert(
                gamma >= 0 && gamma <= 10,
                "Gamma settings is wrong!");

            //小于黑场阈值都设成0
            for (Int32 i = 0; i < blackThreshold; i++)
            {
                _BrightnessTable[i] = 0;
            }

            //中间部分做gamma校正
            Double ig = (gamma == 0.0) ? 0.0 : 1 / gamma;
            Double threshold = whiteThreshold - blackThreshold;
            for (Int32 i = blackThreshold; i < whiteThreshold; i++)
            {
                _BrightnessTable[i] = (Byte)Math.Max(Math.Min(Math.Pow((i - blackThreshold) / threshold, ig) * 255, 255.0), 0.0);
            }

            //大于白场阈值都设为255
            for (Int32 i = whiteThreshold; i < 256; i++)
            {
                _BrightnessTable[i] = 255;
            }
        }

        static ColorExtension()
        {
            CreateBrightnessTable(10, 240, 5);
        }
    }
}

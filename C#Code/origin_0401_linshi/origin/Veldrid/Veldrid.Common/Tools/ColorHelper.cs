using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid.Common.Tools
{
    internal static class ColorHelper
    {
        public static System.Numerics.Vector4 ColorConverToVect4(this Color color)
        {
            return new System.Numerics.Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }
        public static RgbaFloat ColorConverToRGBA(this Color color)
        {
            return new RgbaFloat(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        /// <summary>
        /// 获取互补色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static RgbaFloat ColorConvertToRGBA_Complementary(this Color color)
        {
            return new RgbaFloat(1 - (color.R / 255f), 1 - (color.G / 255f), 1 - (color.B / 255f), color.A / 255f);
        }

        /// <summary>
        /// 获取互补色
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color Get_Complementary(this Color color)
        {
            return Color.FromArgb(color.A, byte.MaxValue - color.R, byte.MaxValue - color.G, byte.MaxValue - color.B);
        }
    }
}

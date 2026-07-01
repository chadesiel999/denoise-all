using System;
using System.Drawing;

namespace ScopeX.Core
{
    public class Spectrogram
    {
        #region SpectrogramDatatoRGB

        /// <summary>
        /// 1670万色 赋色（value由   小 -> 大， 映射到颜色值由  紫 -> 蓝 -> 浅蓝 -> 绿 -> 黄 -> 红）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color GetSpectrumColor8Bit(Double value, Double lowest = -300, Double highest = 30)
        {
            highest = 30;
            lowest = -200;
            if (lowest >= highest)
            {
                Double temp = highest;
                highest = lowest;
                lowest = temp;
            }
            Color c = Color.Black;
            Double c0 = 0.0F, c1 = 256.0F, c2 = 256.0F * 2.0F, c3 = 256.0F * 3.0F, c4 = 256.0F * 4.0F, c5 = 256.0F * 5.0F;
            Double range = highest - lowest;
            Double step = c5 * (value - lowest) / range;

            if (step > c4 && step <= c5)
            {
                Double a = step - c4; c = Color.FromArgb(255, (Int32)(255.0F - 255.0F * a / c1), 0); //255，255，0开始      255，0，0结束     黄 -> 红 
            }
            else if (step > c3 && step <= c4)
            {
                Double a = step - c3; c = Color.FromArgb((Int32)(255.0F * a / c1), 255, 0); //0，255，0开始      255，255，0结束   绿 -> 黄 
            }
            else if (step > c2 && step <= c3)
            {
                Double a = step - c2; c = Color.FromArgb(0, 255, (Int32)(255.0F - 255.0F * a / c1)); //0，255，255开始      0，255，0结束    浅蓝 -> 绿
            }
            else if (step > c1 && step <= c2)
            {
                Double a = step - c1; c = Color.FromArgb(0, (Int32)(255.0F * a / c1), 255); //0，0，255开始      0，255，255结束    蓝 -> 浅蓝
            }
            else if (step >= c0 && step <= c1)
            {
                Double a = step; c = Color.FromArgb((Int32)(255.0F - 255.0F * a / c1), 0, 255); //255，0，255开始      0，0，255结束    紫 -> 蓝
            }
            else if (step > c5)
            {
                c = Color.Red;
            }
            else
            {
                c = Color.Black;
            }
            return c;
        }

        /// <summary>
        /// 10.7亿色 赋色（value由   小 -> 大， 映射到颜色值由  紫 -> 蓝 -> 浅蓝 -> 绿 -> 黄 -> 红）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Color GetSpectrumColor10Bit(Double value, Double range)
        {
            Color c = Color.Black;
            Double c0 = 0.0F, c1 = 1024.0F, c2 = 1024.0F * 2.0F, c3 = 1024.0F * 3.0F, c4 = 1024.0F * 4.0F, c5 = 1024.0F * 5.0F;
            Double step = c5 * (value) / range;
            if (step > c4 && step <= c5)
            {
                Double a = step - c4; c = Color.FromArgb(1024, (Int32)(1024.0F - 1024.0F * a / c1), 0); //1024，1024，0开始      1024，0，0结束     黄 -> 红 
            }
            else if (step > c3 && step <= c4)
            {
                Double a = step - c3; c = Color.FromArgb((Int32)(1024.0F * a / c1), 1024, 0); //0，1024，0开始      1024，1024，0结束   绿 -> 黄 
            }
            else if (step > c2 && step <= c3)
            {
                Double a = step - c2; c = Color.FromArgb(0, 1024, (Int32)(1024.0F - 1024.0F * a / c1)); //0，1024，1024开始      0，1024，0结束    浅蓝 -> 绿
            }
            else if (step > c1 && step <= c2)
            {
                Double a = step - c1; c = Color.FromArgb(0, (Int32)(1024.0F * a / c1), 1024); //0，0，1024开始      0，1024，1024结束    蓝 -> 浅蓝
            }
            else if (step >= c0 && step <= c1)
            {
                Double a = step; c = Color.FromArgb((Int32)(1024.0F - 1024.0F * a / c1), 0, 1024); //1024，0，1024开始      0，0，1024结束    紫 -> 蓝
            }
            else
            {
                c = Color.Black;
            }
            return c;
        }
        #endregion
    }
}

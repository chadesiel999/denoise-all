using System;
using System.Collections.Generic;
using System.Text;

namespace Veldrid.Common.Tools
{
    internal static class SiHelper
    {
        private static string SICollection = "yzafpnμmDkMGTPEZY";
        public static string ValueChangeToSI(double value, out string siUnit, out string numberstring, int decimals = 1, string unit = "", bool invalid = false)
        {
            string formatstring = "#0";
            if (!invalid)
            {
                if (decimals > 0)
                {
                    formatstring += ".";
                    for (int i = 0; i < decimals; i++) formatstring += "#";
                }
            }
            else
            {
                formatstring = "N" + decimals;
            }
            siUnit = unit;
            if (double.IsNaN(value))
            {
                numberstring = "NaN";
                return "NaN";
            }
            if (value == 0)
            {
                numberstring = value.ToString(formatstring);
                return numberstring + unit;
            }
            if (Math.Abs(value) < 1E-24)
            {
                numberstring = Math.Round((decimal)0f, decimals) + "";
                return Math.Round((decimal)0f, decimals) + unit;
            }
            string SI = SICollection;
            double d = Math.Log(Math.Abs(value), 1000);
            decimal number = Math.Round((decimal)(value / Math.Pow(1000, Math.Floor(d))), decimals);
            d += 8;
            if (d > 16)
            {
                numberstring = "NaN";
                siUnit = "";
                return "";
            }
            string s = SI.Substring((int)d, 1);
            if (s == "D") s = "";
            siUnit = s + unit;
            if(unit=="UI"||unit=="BER")//浴盆曲线固定转换
            {
                if (s == "m")
                {
                    number /= 1000;
                    siUnit = unit;
                }
            }           
            numberstring = number.ToString(formatstring);
            if (unit == "BER")
            {
                numberstring = "1E" + (number >0?"+":"")+ numberstring;
            }
            return numberstring + siUnit;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    public static class MeasureHelper
    {
        public const String MeasureEmpty = "------";
        public const String MeasureInfinity = "+Inf";
        public const String MeasureError = "******";

        private static readonly Regex regex = new Regex(@"^(?<number>\d+(\.\d+)?)(?<unit>[a-zA-Z%]+)?$");
        public static String ToFormat(this String value,String format)
        {
            String @new= value;
           if(regex.IsMatch(@new) && !string.IsNullOrEmpty(@new)&& !string.IsNullOrEmpty(format))
            {
                var match = regex.Match(@new);
                var numberPart = match.Groups["number"].Value;
                var unitPart = match.Groups["unit"].Value;
                //<Remark>创建人：彭博 创建日期：2024/2/22 20:00:00  原因：numberPart可能为空 </Remark>
                if (numberPart != null && string.IsNullOrEmpty(numberPart))
                {
                    numberPart = "0";
                }
                switch (format)
                {
                    case "D"://整数
                    case "0.000"://保留三位小数
                    default:
                        @new = $"{Double.Parse(numberPart!).ToString(format)}{unitPart}";
                        break;
                    case "f3"://保留三位小数的百分比
                        unitPart = unitPart == "%" ? unitPart : "%";
                        @new = $"{(Double.Parse(numberPart!)).ToString(format)}{unitPart}";
                        break;
                }
            }

            return @new;
        }
    }
}

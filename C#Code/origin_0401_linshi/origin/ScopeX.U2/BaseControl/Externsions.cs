using System;

namespace ScopeX.U2
{
    /// <summary>
    /// Class Ext.
    /// </summary>
    public static partial class Externsions
    {
        #region 数值转换

        /// <summary>
        /// 转换为整型
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>System.Int32.</returns>
        public static Int32 ToInt(this Object data)
        {
            if (data == null)
                return 0;
            if (data is Boolean)
            {
                return (Boolean)data ? 1 : 0;
            }
            Int32 result;
            var success = Int32.TryParse(data.ToString(), out result);
            if (success)
                return result;
            try
            {
                return Convert.ToInt32(ToDouble(data, 0));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 转换为可空整型
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>System.Nullable&lt;System.Int32&gt;.</returns>
        public static Int32? ToIntOrNull(this Object data)
        {
            if (data == null)
                return null;
            Int32 result;
            var isValid = Int32.TryParse(data.ToString(), out result);
            if (isValid)
                return result;
            return null;
        }

        /// <summary>
        /// 转换为双精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>System.Double.</returns>
        public static Double ToDouble(this Object data)
        {
            if (data == null)
                return 0;
            Double result;
            return Double.TryParse(data.ToString(), out result) ? result : 0;
        }

        /// <summary>
        /// 转换为双精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digits">小数位数</param>
        /// <returns>System.Double.</returns>
        public static Double ToDouble(this Object data, Int32 digits)
        {
            return Math.Round(ToDouble(data), digits, System.MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 转换为可空双精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>System.Nullable&lt;System.Double&gt;.</returns>
        public static Double? ToDoubleOrNull(this Object data)
        {
            if (data == null)
                return null;
            Double result;
            var isValid = Double.TryParse(data.ToString(), out result);
            if (isValid)
                return result;
            return null;
        }

        /// <summary>
        /// 转换为高精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>System.Decimal.</returns>
        public static Decimal ToDecimal(this Object data)
        {
            if (data == null)
                return 0;
            Decimal result;
            return Decimal.TryParse(data.ToString(), out result) ? result : 0;
        }

        /// <summary>
        /// 转换为高精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digits">小数位数</param>
        /// <returns>System.Decimal.</returns>
        public static Decimal ToDecimal(this Object data, Int32 digits)
        {
            return Math.Round(ToDecimal(data), digits, System.MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 转换为可空高精度浮点数
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>System.Nullable&lt;System.Decimal&gt;.</returns>
        public static Decimal? ToDecimalOrNull(this Object data)
        {
            if (data == null)
                return null;
            Decimal result;
            var isValid = Decimal.TryParse(data.ToString(), out result);
            if (isValid)
                return result;
            return null;
        }

        /// <summary>
        /// 转换为可空高精度浮点数,并按指定的小数位4舍5入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="digits">小数位数</param>
        /// <returns>System.Nullable&lt;System.Decimal&gt;.</returns>
        public static Decimal? ToDecimalOrNull(this Object data, Int32 digits)
        {
            var result = ToDecimalOrNull(data);
            if (result == null)
                return null;
            return Math.Round(result.Value, digits, System.MidpointRounding.AwayFromZero);
        }

        #endregion 数值转换

        #region 日期转换

        /// <summary>
        /// 转换为日期
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToDate(this object data)
        {
            try
            {
                if (data == null)
                    return DateTime.MinValue;
                if (System.Text.RegularExpressions.Regex.IsMatch(data.ToStringExt(), @"^\d{8}$"))
                {
                    var strValue = data.ToStringExt();
                    return new DateTime(strValue.Substring(0, 4).ToInt(), strValue.Substring(4, 2).ToInt(), strValue.Substring(6, 2).ToInt());
                }
                DateTime result;
                return DateTime.TryParse(data.ToString(), out result) ? result : DateTime.MinValue;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// 转换为可空日期
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>System.Nullable&lt;DateTime&gt;.</returns>
        public static DateTime? ToDateOrNull(this object data)
        {
            try
            {
                if (data == null)
                    return null;
                if (System.Text.RegularExpressions.Regex.IsMatch(data.ToStringExt(), @"^\d{8}$"))
                {
                    var strValue = data.ToStringExt();
                    return new DateTime(strValue.Substring(0, 4).ToInt(), strValue.Substring(4, 2).ToInt(), strValue.Substring(6, 2).ToInt());
                }
                DateTime result;
                var isValid = DateTime.TryParse(data.ToString(), out result);
                if (isValid)
                    return result;
                return null;
            }
            catch
            {
                return null;
            }
        }

        #endregion 日期转换

        #region 字符串转换

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>System.String.</returns>
        private static String ToStringExt(this object data)
        {
            return data == null ? string.Empty : data.ToString();
        }

        #endregion 字符串转换

        #region 是否数字

        /// <summary>
        /// 是否数字
        /// </summary>
        /// <param name="value">value</param>
        /// <returns>返回值</returns>
        public static Boolean IsNum(this String value)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d+(\.\d*)?$");
        }

        #endregion 是否数字
    }
}

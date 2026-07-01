using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ScopeX.ComModel;

namespace ScopeX.Core.Tools
{
    public enum Prefix
    {
        Femto,
        Pico,
        Nano,
        Micro,
        Milli,
        Empty,
        Kilo,
        Mega,
        Giga,
        Tera,
        Peta,
        Exa,
    }

    public enum QuantityUnit
    {
        Voltage,
        VoltagePeakPeak,
        Vrms,
        dBm,
        //<Remark>创建人：彭博 创建日期：2023/11/29 11:43:00  原因：测试需求，新增采样率参数 </Remark>
        SampleRate,     //采样率 
        Ampere,
        Ohm,
        Watt,           //功率
        VA,             //功率
        Var,            //功率  
        Joule,
        Count,      //次数
        Constant,
        Hertz,
        BitPerSecond,
        Second,     //时间
        Minute,     //时间    
        Hour,       //时间  
        Angle,      //角度
        Radian,     //弧度
        Percent,
        PartPerThousand,
        Decibel,
        Celsius,    //温度    
        Fahrenheit, //温度
        Division,   //格
        Bit,
        Byte,
        Coulomb,    //电荷
        Farad,      //电容
        Henry,
        Meter,      //长度
        BER,
        HITS,
        SER,
        VoltageRate,
        CurrentRate,
        Unknow,
        Variant,    //用户指定
    }

    public static class PrefixExt
    {

        public static readonly ImmutableDictionary<Prefix, String> _PfxTable = new Dictionary<Prefix, String>
        {
            { Prefix.Femto, "f" },
            { Prefix.Pico, "p" },
            { Prefix.Nano, "n" },
            { Prefix.Micro, "μ" },
            { Prefix.Milli, "m" },
            { Prefix.Empty, "" },
            { Prefix.Kilo, "k" },
            { Prefix.Mega, "M" },
            { Prefix.Giga, "G" },
            { Prefix.Tera, "T" },
            { Prefix.Peta, "P" },
            { Prefix.Exa, "E" },
        }.ToImmutableDictionary();

        public static String ToPfxString(this Prefix pfx)
        {
            if (_PfxTable.TryGetValue(pfx, out var name))
            {
                return name;
            }

            return "";
        }

        public static Prefix Inverse(Prefix pfx) => Prefix.Empty - (pfx - Prefix.Empty);
    }

    public static class QuantityUnitExt
    {
        public static readonly ImmutableDictionary<QuantityUnit, String> UnitTable = new Dictionary<QuantityUnit, String>()
        {
            { QuantityUnit.Voltage, "V" },
            { QuantityUnit.VoltagePeakPeak, "Vpp" },
            { QuantityUnit.Vrms, "Vrms" },
            { QuantityUnit.dBm, "dBm" },
            { QuantityUnit.SampleRate, "Sa/s" }, //<Remark>创建人：彭博 创建日期：2023/11/29 11:43:00  原因：测试需求，新增采样率参数 </Remark>
            { QuantityUnit.Ampere, "A" },
            { QuantityUnit.Ohm, "Ω" },
            { QuantityUnit.Watt, "W" },
            { QuantityUnit.VA, "VA" },
            { QuantityUnit.Var, "Var" },
            { QuantityUnit.Joule, "J" },
            { QuantityUnit.Count, "#" },
            { QuantityUnit.Constant, " " },
            { QuantityUnit.Hertz, "Hz" },
            { QuantityUnit.BitPerSecond, "bps" },
            { QuantityUnit.Second, "s" },
            { QuantityUnit.Minute, "min" },
            { QuantityUnit.Hour, "h" },
            { QuantityUnit.Angle, "°" },
            { QuantityUnit.Radian,"rad" },
            { QuantityUnit.Percent, "%" },
            { QuantityUnit.PartPerThousand, "‰"},
            { QuantityUnit.Decibel, "dB"},
            { QuantityUnit.Celsius, "°C" },
            { QuantityUnit.Fahrenheit, "°F" },
            { QuantityUnit.Division, "div" },
            { QuantityUnit.Bit, "bit"},
            { QuantityUnit.Byte, "B" },
            { QuantityUnit.Coulomb, "C"},
            { QuantityUnit.Farad, "F"},
            { QuantityUnit.Henry, "H"},
            { QuantityUnit.Meter, "m"},
            { QuantityUnit.BER, "BER"},
            { QuantityUnit.HITS, "hits"},
            { QuantityUnit.SER, "SER"},
            { QuantityUnit.VoltageRate, "V/s"},
            { QuantityUnit.CurrentRate, "A/s"},
            { QuantityUnit.Unknow, "?" },
            { QuantityUnit.Variant, "" },
        }.ToImmutableDictionary();

        public static String ToUnitString(this QuantityUnit unit)
        {
            if (UnitTable.TryGetValue(unit, out var name))
            {
                return name;
            }

            return "?";
        }

        public static QuantityUnit Inverse(QuantityUnit qu) => qu switch
        {
            QuantityUnit.Hertz => QuantityUnit.Second,
            QuantityUnit.Second => QuantityUnit.Hertz,
            _ => throw new ArgumentException("Not Implement!"),
        };

        public static QuantityUnit Parse(String unitString)
        {
            foreach (var kvp in UnitTable)
            {
                if (kvp.Value == unitString)
                {
                    return kvp.Key;
                }
            }
            return QuantityUnit.Variant;
        }
    }

    public struct Quantity
    {
        //private static readonly ImmutableArray<String> _SupScptTable = new()
        //{
        //    "⁰", "¹", "²", "³", "⁶", "⁴", "⁵", "⁶", "⁷" ,"⁸", "⁹"
        //};

        private Double Value
        {
            get;
            set;
        }

        private Prefix Prefix
        {
            get;
            set;
        }

        private QuantityUnit TypeUnit
        {
            get;
            set;
        }

        private QuantityUnit Unit
        {
            get;
            set;
        }

        private String UnitString
        {
            get => _UnitString;
            set => _UnitString = value;
        }

        private String TypeUnitString
        {
            get => _TypeUnitString;
            set => _TypeUnitString = value;
        }

        private String _UnitString;

        private String _TypeUnitString;

        private const String _SICollection = "yzafpnμmDkMGTPEZY";

        private const String _ErrorInfo = MeasureHelper.MeasureError;

        private const String _PosInfInfo = MeasureHelper.MeasureInfinity;

        private const String _NegInfInfo = MeasureHelper.MeasureEmpty;

        public String ToString(String format, Int16 maxCharNum, Prefix pfx, Boolean appendUnit = false)
        {
            if (Double.IsNaN(Value))
            {
                return _ErrorInfo;
            }
            else if (Double.IsPositiveInfinity(Value))
            {
                return _PosInfInfo;
            }
            else if (Double.IsNegativeInfinity(Value))
            {
                return _NegInfInfo;
            }

            if (!Enum.IsDefined(Prefix))
            {
                return _ErrorInfo;
            }

            //let Prefix==pfx
            if (Prefix != pfx)
            {
                try
                {
                    // The following line raises an exception because it is checked.
                    Value /= Math.Pow(1000, pfx - Prefix);
                    Prefix = pfx;
                }
                catch (NotFiniteNumberException)
                {
                    return _ErrorInfo;
                }
            }

            String tmp = String.Format("{0:" + format + "}", Value);

            if (Value < 0)
            {
                maxCharNum++;
            }

            if (maxCharNum > tmp.Length)
            {
                maxCharNum = (Int16)tmp.Length;
            }

            tmp = tmp.Substring(0, maxCharNum).TrimEnd(new Char[] { '.' }) + " ";

            tmp += _TypeUnitString + Prefix.ToPfxString();

            if (appendUnit)
            {
                tmp += UnitString;
            }

            return tmp;
        }

        //Normalize to 1<=Value<1000
        private Int32 Normalize()
        {
            var oldpfx = Prefix;
            if (Math.Abs(Value) >= Double.Epsilon)
            {
                while (Math.Abs(Value) >= 1000.0)
                {
                    Value /= 1000.0;
                    Prefix++;
                }
                while (Math.Abs(Value) < 1.0 - Double.Epsilon)
                {
                    //<Remark>创建人：彭博 创建日期：2024/2/22 20:00:00  原因：最小单位a选中显示异常问题 </Remark>
                    if (Prefix > 0)
                    {
                        Value *= 1000.0;
                        Prefix--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return Prefix - oldpfx;
        }
        /// <summary>
        /// 按SI单位制为数字添加后缀
        /// </summary>
        /// <param name="value">需要转换的数字</param>
        /// <param name="decimals">小数位数</param>
        /// <param name="unit">附加单位</param>
        /// <returns></returns>
        public String ValueChangeToSI(Int32 decimals = 1, Boolean fillzero = false)
        {
            Double hex = 1000;
            String unit = UnitString;
            Double value = Value * Math.Pow(hex, Prefix - Prefix.Empty);
            if (Double.IsNaN(value))
                return _NegInfInfo;
            if (Double.IsPositiveInfinity(value))
                return _PosInfInfo;
            if (Double.IsNegativeInfinity(value))
                return _NegInfInfo;
            String formatstring = String.Empty;
            if (fillzero)
            {
                formatstring = "#0";
                if (decimals > 0)
                {
                    formatstring += ".";
                    for (Int32 i = 0; i < decimals; i++)
                        formatstring += "0";
                }
            }
            else
            {
                formatstring = "{0:0." + String.Join("", Enumerable.Repeat("#", decimals)) + "}{1}{2}";
            }
            if (value == 0)
            {
                if (fillzero)
                    return 0.ToString(formatstring) + unit;
                return String.Format(formatstring, 0, "", unit);
            }
            if (Math.Abs(value) < 1E-24 || Math.Abs(value) > 1E24)
                return Math.Round((Decimal)0f, decimals) + unit;
            String SI = _SICollection;
            Decimal d = (Decimal)Math.Log(Math.Abs(value), hex);
            Decimal number = Math.Round((Decimal)(value / Math.Pow(hex, (Double)Math.Floor(d))), decimals);
            d += 8;
            String s = SI.Substring((Int32)d, 1);
            if (s == "D")
                s = "";
            if (fillzero)
            {
                return number.ToString(formatstring) + s + unit;
            }
            else
            {
                return String.Format(formatstring, number, s, unit);
            }
        }

        public String ToString(String format, Boolean appendUnit, Int16 maxCharNum)
        {
            if (Double.IsNaN(Value))
            {
                return _NegInfInfo;
            }
            else if (Double.IsPositiveInfinity(Value))
            {
                return _PosInfInfo;
            }
            else if (Double.IsNegativeInfinity(Value))
            {
                return _NegInfInfo;
            }

            Normalize();

            if (!Enum.IsDefined(Prefix))
            {
                return _ErrorInfo;
            }

            String tmp = String.Format("{0:" + format + "}", Value);

            //#region 特殊处理过程精度问题
            //Value = Double.Parse(tmp);
            //while (Value >= 1000)
            //{
            //    Value /= 1000.0;
            //    Prefix++;
            //}
            //tmp = String.Format("{0:" + format + "}", Value);
            //#endregion


            if (Value < 0)
            {
                maxCharNum++;
            }

            if (maxCharNum > tmp.Length)
            {
                maxCharNum = (Int16)tmp.Length;
            }

            tmp = tmp[..maxCharNum].TrimEnd(new Char[] { '.' }) + " ";

            if (Math.Abs(Value) > Double.Epsilon)
            {
                if (UnitString == "UI")
                {
                    if (Prefix == Prefix.Milli)
                    {
                        tmp = (Value / 1000).ToString();
                    }
                }
                else if (UnitString == "BER")
                {
                    if (Prefix == Prefix.Milli)
                    {
                        tmp = (Value / 1000).ToString();
                    }
                    tmp = "1E+" + tmp;
                }
                else
                {
                    tmp += TypeUnitString + Prefix.ToPfxString();
                }
            }

            if (appendUnit)
            {
                tmp += UnitString;
            }



            return tmp;
        }

        public List<String> ToString(String format, Boolean appendUnit, Int16 maxCharNum, Boolean separate)
        {
            if (!separate)
            {
                return new List<String>() { ToString(format, appendUnit, maxCharNum) };
            }
            if (Double.IsNaN(Value))
            {
                return new List<String>() { _ErrorInfo, UnitString };
            }
            else if (Double.IsPositiveInfinity(Value))
            {
                return new List<String>() { _PosInfInfo, UnitString };
            }
            else if (Double.IsNegativeInfinity(Value))
            {
                return new List<String>() { _NegInfInfo, UnitString };
            }

            Normalize();

            if (!Enum.IsDefined(Prefix))
            {
                return new List<String>() { _ErrorInfo, UnitString };
            }

            String tmp = String.Format("{0:" + format + "}", Value);

            if (Value < 0)
            {
                maxCharNum++;
            }

            if (maxCharNum > tmp.Length)
            {
                maxCharNum = (Int16)tmp.Length;
            }

            tmp = tmp[..maxCharNum].TrimEnd(new Char[] { '.' });

            String tmpunit = "";
            if (Math.Abs(Value) > Double.Epsilon)
            {
                tmpunit += Prefix.ToPfxString();
            }

            if (appendUnit)
            {
                tmpunit += UnitString;
            }

            return new List<String>() { tmp, tmpunit };
        }
        public String ToString(Int32 integerLen, Int32 decimalLen, Boolean appendUnit = false)
        {
            if (Double.IsNaN(Value) || Double.IsInfinity(Value))
            {
                return _ErrorInfo;
            }

            Normalize();

            if (!Enum.IsDefined(Prefix))
            {
                return _ErrorInfo;
            }

            //var nfi = CultureInfo.InvariantCulture.NumberFormat;
            //nfi.NumberDecimalDigits = (Prefix - Prefix.Empty) * 3 - exp;
            //String tmp = Value.ToString("F", nfi); 

            var d = (Prefix - Prefix.Empty) * 3;
            var il = integerLen - d;
            var dl = d - decimalLen;
            if (dl < 0)
            {
                dl = 0;
            }
            String fmt;
            if (il <= 0)
            {
                fmt = "###." + new String('0', dl);
            }
            else
            {
                fmt = new String('0', il) + "." + new String('0', dl);
            }

            String tmp = Value.ToString(fmt);

            tmp += Prefix.ToPfxString();
            if (appendUnit)
            {
                tmp += UnitString;
            }

            return tmp;
        }

        public String ToString(String format, Boolean appendUnit) => ToString(format, appendUnit, Int16.MaxValue - 1);

        public String ToString(Int16 maxDigLen, Boolean appendUnit) => ToString("#.00000;;0", appendUnit, maxDigLen);

        public override String ToString() => ToString(4, true);

        public Quantity(Double value = 0.0)
        {
            Value = value;

            Prefix = Prefix.Empty;
            Unit = QuantityUnit.Constant;
            TypeUnit = QuantityUnit.Variant;
            _UnitString = QuantityUnitExt.ToUnitString(Unit);
            _TypeUnitString = QuantityUnitExt.ToUnitString(TypeUnit);
        }

        public Quantity(Int64 value, Prefix pfx, QuantityUnit unit)
        {
            Value = Convert.ToDouble(value);

            Prefix = pfx;
            Unit = unit;
            TypeUnit = QuantityUnit.Variant;
            _UnitString = QuantityUnitExt.ToUnitString(Unit);
            _TypeUnitString = QuantityUnitExt.ToUnitString(TypeUnit);
        }

        public Quantity(Double value, Prefix pfx, QuantityUnit typeunit, QuantityUnit unit)
        {
            Value = Convert.ToDouble(value);

            Prefix = pfx;
            Unit = unit;
            TypeUnit = typeunit;
            _UnitString = QuantityUnitExt.ToUnitString(Unit);
            _TypeUnitString = QuantityUnitExt.ToUnitString(TypeUnit);
        }

        public Quantity(Double value, Prefix pfx, QuantityUnit unit)
        {
            Value = value;

            Prefix = pfx;
            Unit = unit;
            TypeUnit = QuantityUnit.Variant;
            _UnitString = QuantityUnitExt.ToUnitString(Unit);
            _TypeUnitString = QuantityUnitExt.ToUnitString(TypeUnit);
        }

        public Quantity(Double? value, Prefix pfx, QuantityUnit unit)
        {
            if (value.HasValue)
            {
                Value = value.Value;
            }
            else
            {
                Value = Double.NaN;
            }

            Prefix = pfx;
            Unit = unit;
            TypeUnit = QuantityUnit.Variant;
            _UnitString = QuantityUnitExt.ToUnitString(Unit);
            _TypeUnitString = QuantityUnitExt.ToUnitString(TypeUnit);
        }

        public Quantity(Int64 value, Prefix pfx, String unitString)
        {
            Value = Convert.ToDouble(value);

            Prefix = pfx;
            Unit = QuantityUnitExt.Parse(unitString);
            TypeUnit = QuantityUnit.Variant;
            _UnitString = unitString;
            _TypeUnitString = QuantityUnitExt.ToUnitString(TypeUnit);
        }

        public Quantity(Double value, Prefix pfx, String unitString)
        {
            Value = value;

            Prefix = pfx;

            Unit = QuantityUnitExt.Parse(unitString);
            TypeUnit = QuantityUnit.Variant;
            _UnitString = unitString;
            _TypeUnitString = QuantityUnitExt.ToUnitString(TypeUnit);
        }

        public Quantity(Double? value, Prefix pfx, String unitString)
        {
            if (value.HasValue)
            {
                Value = value.Value;
            }
            else
            {
                Value = Double.NaN;
            }

            Prefix = pfx;

            Unit = QuantityUnitExt.Parse(unitString);
            TypeUnit = QuantityUnit.Variant;
            _UnitString = unitString;
            _TypeUnitString = QuantityUnitExt.ToUnitString(TypeUnit);
        }


        public static Double ConvertByPrefix(Double? value, Prefix fromPfx, Prefix toPfx = Prefix.Empty)
        {
            if (value != null && Double.IsFinite(value.Value))
            {
                return (Double)((Decimal)value * (Decimal)Math.Pow(1000, fromPfx - toPfx));
            }
            else
            {
                return Double.NaN;
            }
        }
        public static Double ConverByQuint(Double value)
        {
           
            //var result= Math.Round(value, 16);
            var str = "F18";
            var resulttostr = value.ToString(str);
            var returnresult = Convert.ToDouble(resulttostr);
            return returnresult;
        }
        public static String ConverByQuintWithUnit(Double value, QuantityUnit unit, Int32 decimalNumber)
        {
            Int32 i = 0;
            if (value != 0)
            {
                while (Math.Floor(Math.Abs(value)) != 0)
                {
                    value /= 1000;
                    i++;
                }
                while (Math.Floor(Math.Abs(value)) == 0)
                {
                    value *= 1000;
                    i--;
                }
            }
            while (i + 6 <= 0) //处理单位a
            {
                value /= 1000;
                i++;
            }
            var index = (Prefix)((Int32)Prefix.Empty + i);
            
            String formatString = "F" + decimalNumber;

            if (index >= 0)
            {
                var unittostring = PrefixExt._PfxTable[index] + QuantityUnitExt.UnitTable[unit];
                var result = value.ToString(formatString) + unittostring;
                return result;
            }

            return value.ToString();


        }

        public static Prefix GetValuePrefix(Double value, Prefix pfx)
        {
            value = ConvertByPrefix(value, pfx);
            Double absValue = Math.Abs(value);
            if (absValue == 0)
                return Prefix.Empty; // Handle zero case if necessary

            Double logValue = Math.Log10(absValue);
            if (logValue < -15)
                return Prefix.Femto;
            else if (logValue < -12)
                return Prefix.Pico;
            else if (logValue < -9)
                return Prefix.Nano;
            else if (logValue < -6)
                return Prefix.Micro;
            else if (logValue < -3)
                return Prefix.Milli;
            else if (logValue < 3)
                return Prefix.Empty;
            else if (logValue < 6)
                return Prefix.Kilo;
            else if (logValue < 9)
                return Prefix.Mega;
            else if (logValue < 12)
                return Prefix.Giga;
            else if (logValue < 15)
                return Prefix.Tera;
            else if (logValue < 18)
                return Prefix.Peta;
            else
                return Prefix.Exa;
        }

    }
}

using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        #region P层对象
        public static DsoPrsnt Presenter { set; get; }

        #endregion  P层对象

        /// <summary>
        /// 返回值自动科学计数法 开关
        /// </summary>
        #region 公有基础方法

        #region 字符串编码

        private static readonly string UnitCollection = "yzafpnumDkMGTPEZY";

        public static string encodingBytes(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        public static List<string> ParamListToStrList(List<byte[]> inputBytesList)
        {
            if (inputBytesList == null)
            {
                return null;
            }
            List<string> returnValue = new List<string>();
            foreach (byte[] bytes in inputBytesList)
                returnValue.Add(encodingBytes(bytes));
            return returnValue;
        }
        public static byte[] decodeStr(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static decimal StringToDecimal(String number, String unit)
        {
            return SIChangeToValue(number, unit);
        }

        internal static decimal SIChangeToValue(string number, string unit, double hex = 1000)
        {
            try
            {
                if (hex <= 0)
                {
                    hex = 1000;
                }
                if (string.IsNullOrEmpty(number))
                {
                    return 0;
                }
                char inputunit;
                char inputseconduint;
                if (string.IsNullOrEmpty(unit))//输入时不带单位 默认系统单位
                {
                    return (decimal)Double.Parse(number);
                }

                if (unit.Length >= 2)//mV us Hz mHz
                {
                    if (unit[0] == 'H')//如果第一位是H 则特殊处理
                    {
                        inputunit = UnitCollection[8];
                        inputseconduint = unit[0];
                    }
                    else
                    {
                        inputunit = unit[0];//取第一位
                        inputseconduint = unit[1];//取第二位
                    }
                }
                else//如果只有一位单位 则默认是基本单位
                {
                    inputunit = UnitCollection[8];
                    inputseconduint = unit[0];//取 V s H
                }

                //inputunit = split[1].Length >= 2 ? split[1][0] : UnitCollection[8];
                //inputseconduint = split[1].Length >= 2 ? split[1][1] : split[1][0];
                //inputseconduint = inputunit == 'H' ? inputunit : inputseconduint;
                //inputunit = inputunit == 'H' ? UnitCollection[8] : inputunit;

                var unitindex = UnitCollection.IndexOf(inputunit);
                var defaultunit = GetDefaultUnit(inputseconduint);
                var index = unitindex - defaultunit;

                var multiplier = Math.Pow(hex, index);
                if (double.TryParse(number, out var value))
                {
                    decimal valueByDecimal = (decimal)value;
                    return valueByDecimal * (decimal)multiplier;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private static int GetDefaultUnit(char inputSecondUnit)
        {
            switch (inputSecondUnit)
            {
                case 's':
                    return UnitCollection.IndexOf("u");//时间默认单位us
                case 'H':
                case 'V':
                    return UnitCollection.IndexOf("m");//赫兹默认单位mHz 幅度默认单位mV
                default:
                    return UnitCollection.IndexOf("D");//默认为标准单位
            }
        }

        #endregion 字符串编码
        #region 命令缩写
        private static string shortCMD(string str)
        {
            int index = findLowerChar(str);
            int numIndex = findNumChar(str);
            if (index < 0)
            {
                return str;
            }
            else
            {
                return numIndex < index ? str.Substring(0, index) : str.Substring(0, index) + str.Substring(numIndex);
            }
        }
        private static int findNumChar(string str)
        {
            var lowerChar = str.FirstOrDefault(cr => cr >= '0' && cr <= '9');
            return str.IndexOf(lowerChar);
        }
        private static int findLowerChar(string str)
        {
            var lowerChar = str.FirstOrDefault(cr => cr >= 'a');
            return str.IndexOf(lowerChar);
        }
        #endregion 命令缩写
        #region 将一个对象转换为指定类型
        /// <summary>
        /// 可空枚举 对应Source2 , ljw 21.10
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsNullableEnum(Type type)
        {
            Type utype = Nullable.GetUnderlyingType(type);
            return (utype != null) && utype.IsEnum;
        }
        /// <summary>
        /// 将一个对象转换为指定类型
        /// </summary>
        /// <param name="obj">待转换的对象</param>
        /// <param name="type">目标类型</param>
        /// <returns>转换后的对象</returns>
        private static object? ConvertObject(string obj, Type type, List<string> paramList)
        {
            if (type == null || obj == null) return null;
            if (type.IsEnum)
            {
                var enumValues = Enum.GetValues(type);
                if (paramList == null)
                    return null;
                int foundIndex = paramList.FindIndex(s => s.ToUpper() == obj.ToUpper() || shortCMD(s) == obj);
                if (foundIndex == -1)
                {
                    return Enum.Parse(type, enumValues.GetValue(0)!.ToString()!);//返回第一个
                }

                //设置枚举值，按顺序来设置，避免Int值的不连续性
                // 22.2.22 跳过C5-C8
                if (type == typeof(ChannelId) && foundIndex > (int)ChannelId.C4)
                {
                    var tmpStr = obj.Replace("CHANnel", "C").Replace("CHAN", "C");

                    if (Enum.TryParse(type, tmpStr, out object? chnlId))
                    {
                        var test = (ChannelId)chnlId!;
                        return (ChannelId)chnlId;
                    }
                }

                //设置枚举值，按顺序来设置，避免Int值的不连续性
                int? enumindex = null;
                for (int i = 0; i < enumValues.Length; i++)
                {
                    if (enumValues.GetValue(i)!.ToString()!.ToUpper() == paramList[foundIndex].ToUpper())
                    {
                        enumindex = i;
                        break;
                    }
                }
                //int? foundEnumIndex = enumValues.Cast<int>().FirstOrDefault(m => m == foundIndex);
                if (enumindex == null)//not find in enum collections but must get a enum value 
                {
                    if (foundIndex < enumValues.Length)
                    {
                        return Enum.Parse(type, enumValues.GetValue(foundIndex)!.ToString()!);
                    }
                    else
                        enumindex = 0;
                }

                if (enumindex != null && enumindex >= 0)
                {
                    return Enum.Parse(type, enumValues.GetValue((int)enumindex)!.ToString()!);
                }
            }
            else if (IsNullableEnum(type))
            {
                var utype = Nullable.GetUnderlyingType(type);
                if (paramList == null)
                    return null;

                int foundIndex = paramList.FindIndex(s => s.ToUpper() == obj.ToUpper() || shortCMD(s) == obj);
                if (foundIndex == -1)
                    return null;

                //设置枚举值，按顺序来设置，避免Int值的不连续性
                if (utype == null)
                    return null;

                int? enumindex = null;
                var enumValues = Enum.GetValues(utype);
                for (int i = 0; i < enumValues.Length; i++)
                {
                    if (enumValues.GetValue(i)!.ToString()!.ToUpper() == paramList[foundIndex].ToUpper())
                    {
                        enumindex = i;
                        break;
                    }
                }

                if (enumindex == null)
                {
                    if (foundIndex < enumValues.Length)
                    {
                        return Enum.Parse(type, enumValues.GetValue(foundIndex)!.ToString()!);
                    }
                    else
                        enumindex = 0;
                }

                //int? foundEnumIndex = enumValues.Cast<int>().FirstOrDefault(m => m == foundIndex);
                if (enumindex != null && enumindex >= 0)
                {
                    return Enum.Parse(utype, enumValues.GetValue((int)enumindex)!.ToString()!);
                }
            }
            else if (type.IsValueType)
            {
                if (type != typeof(Boolean))
                {
                    if (type == typeof(Int32))
                    {
                        if (decimal.TryParse(obj, out decimal val))
                        {
                            var value = (Int32)val;
                            return value;
                        }
                        //if (Int32.TryParse(obj, out Int32 v))
                        //    return v;
                    }
                    else if (type == typeof(double))
                    {
                        if (obj.Contains('e') || (obj.Contains('E')))
                        {
                            if (double.TryParse(obj, NumberStyles.AllowExponent, null, out double v))
                                return v;
                        }
                        else if (double.TryParse(obj, out double v))
                            return v;
                    }
                    else if (type == typeof(UInt32))
                    {
                        if (UInt32.TryParse(obj, out UInt32 v))
                            return v;
                    }
                    else if (type == typeof(Int16))
                    {
                        if (Int16.TryParse(obj, out Int16 v))
                            return v;
                    }
                    else if (type == typeof(UInt16))
                    {
                        if (UInt16.TryParse(obj, out UInt16 v))
                            return v;
                    }
                    else if (type == typeof(long))
                    {
                        if (long.TryParse(obj, out long v))
                            return v;
                    }
                    else if (type == typeof(UInt64))
                    {
                        if (UInt64.TryParse(obj, out UInt64 v))
                            return v;
                    }
                    else if (type == typeof(float))
                    {
                        if (float.TryParse(obj, out float v))
                            return v;
                    }
                    else if (type == typeof(char))
                    {
                        if (int.TryParse(obj, out var res))
                        {
                            var v = Convert.ToChar(res);
                            return v;
                        }
                    }
                    else if (type == typeof(byte))
                    {
                        if (int.TryParse(obj, out var re))
                        {
                            var v = Convert.ToByte(re);
                            return v;
                        }
                    }
                    return null;
                }
                //Boolean
                if (paramList == null)
                    return false;
                if (paramList.Count != 2)
                    return false;
                if ((paramList[0].ToUpper() == obj.ToUpper()) || obj == shortCMD(paramList[0]))
                    return false;
                if ((paramList[1].ToUpper() == obj.ToUpper()) || obj == shortCMD(paramList[1]))
                    return true;
                if (obj == "1")
                    return true;
                return obj == "0" ? false : null;
            }
            else if (type == typeof(String))
                return obj;
            return null;
        }
        #endregion 将一个对象转换为指定类型

        public static bool TryGetPropertyInfoByUsingDeclareTablePrsntObject(SCPICommandProcessFuncParam analyResult, out PropertyInfo propertyInfo)
        {
            propertyInfo = null;
            if ((analyResult.Tag is null) || (analyResult.Tag is not ScpiTagObj))
                return false;
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;

            if (scpiTagObj.PrsntObj is null || scpiTagObj.PropertyName is null || scpiTagObj.PropertyName.Trim() == "")
                return false;
            propertyInfo = scpiTagObj.PrsntObj.GetType().GetProperty(scpiTagObj.PropertyName);
            if (propertyInfo == null)
                return false;
            if (propertyInfo.GetType() == typeof(Enum))
            {
                if (scpiTagObj.ParamList == null)
                    return false;
            }
            return true;

        }
        public static bool TryGetPropertyInfo(object obj, string propertyName, out PropertyInfo propertyInfo)
        {
            propertyInfo = null;
            if (obj != null)
                propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo != null;

        }

        public static bool TrySetPropertyValue(object nodeClassPrsntObject, PropertyInfo propertyInfo, string valueString, List<string> paramList = null, long multiply = 1)
        {
            if (propertyInfo != null)
            {
                // 定义匹配数字和小数点，并以特定单位结尾的正则表达式
                Regex regex = new Regex(@"^(?<number>[+-]?\d+(\.\d+)?([eE][+-]?\d+)?)\s*(?<unit>[a-zA-Z%]+)$");
                // 进行匹配
                Match match = regex.Match(valueString);
                if (match.Success)
                {
                    string number = match.Groups["number"].Value;
                    string unit = match.Groups["unit"].Value;
                    if (unit == "%" || unit == "°")//特殊单位
                    {
                        var valueBydecimal = (Decimal)(Double.Parse(number));//long类型的整数与double相乘时，可能会出现精度损失；
                        valueString = (valueBydecimal * multiply).ToString();
                    }
                    else
                        valueString = StringToDecimal(number, unit).ToString();
                }
                else if (multiply != 0 && Double.TryParse(valueString, out Double value))
                {
                    var valueBydecimal = (Decimal)value;//long类型的整数与double相乘时，可能会出现精度损失；
                    valueString = (valueBydecimal * multiply).ToString();
                }
                object setValue = ConvertObject(valueString, propertyInfo.PropertyType, paramList);
                if (setValue != null)
                {
                    propertyInfo.SetValue(nodeClassPrsntObject, setValue);
                }
                else
                    return false;
            }
            return propertyInfo != null;
        }
        public static bool TryGetPropertyValue(object nodeClassPrsntObject, PropertyInfo propertyInfo, out bool usingScientific, out string outputString, List<string> paramList = null, long intOrDoubleMultiplier = 1)
        {
            usingScientific = true;
            outputString = null;
            if (nodeClassPrsntObject == null)
                return false;
            if (propertyInfo == null)
                return false;
            object value = propertyInfo.GetValue(nodeClassPrsntObject);
            if (value == null)
            {
                return false;
            }
            if (value is Enum && paramList != null)
            {
                usingScientific = false;
                //需要解决ParamList定义的连续顺序与枚举Int值不连续的问题
                if (value is ChannelId channelId)
                {
                    //22.2.22
                    if (channelId <= ChannelId.C4)
                    {
                        outputString = paramList[channelId - ChannelId.C1];
                    }
                    else
                    {
                        var index = paramList.FindLastIndex(s => s.ToUpper() == channelId.ToString().ToUpper());
                        if (index >= 0)
                        {
                            outputString = paramList[index];
                        }
                    }
                }
                else
                {
                    if (paramList != null && paramList.Count > 0)
                    {
                        var findindex = paramList.FindIndex(param => param.ToUpper() == value.ToString()!.ToUpper());
                        if (findindex != -1)
                        {
                            outputString = paramList[findindex];
                        }
                        else
                        {
                            var enumIndex = (Int32)value;
                            var okParamListIndex = 0;
                            bool bFound = false;
                            foreach (int enumValue in Enum.GetValues(value.GetType()))
                            {
                                if (enumValue == enumIndex)
                                {
                                    bFound = true;
                                    break;
                                }
                                okParamListIndex++;
                            }

                            if (bFound && okParamListIndex < paramList.Count)
                                outputString = paramList[okParamListIndex];
                        }
                    }

                }
            }
            else if ((value is Boolean) && paramList != null && paramList.Count == 2)
            {
                usingScientific = false;
                outputString = paramList[0] == "OFF" && paramList[1] == "ON" ? ((bool)value) == false ? "0" : "1" : paramList[((bool)value) == false ? 0 : 1];
            }
            else if (intOrDoubleMultiplier != 0 && double.TryParse(value.ToString(), out double getValue))
            {
                outputString = (getValue / intOrDoubleMultiplier).ToString();
                if (value is Int32 || value is String || value is Int16 || value is UInt16 || value is UInt32)//Double Int64
                {
                    usingScientific = false;
                }
            }
            else if (value is string)
            {
                outputString = value.ToString();
                usingScientific = false;
            }
            else if (value is char)
            {
                int asciiValue = Convert.ToInt32(value); // 将 char 转换为 int
                //string hexValue = $"0x{asciiValue:X2}";
                outputString = $"{asciiValue}";
                usingScientific = false;
            }
            return (outputString != null);

        }
        public static bool TryGetPropertyValue(object classPrsntObject, string propertyName, out bool usingScientific, out string outputString, List<string> paramList = null, long intOrDoubleMultiplier = 1)
        {
            object searchPrsnt = classPrsntObject == null ? Presenter : classPrsntObject;
            PropertyInfo propertyInfo = searchPrsnt.GetType().GetProperty(propertyName);
            return TryGetPropertyValue(searchPrsnt, propertyInfo, out usingScientific, out outputString, paramList, intOrDoubleMultiplier);
        }

        #endregion

        #region 通用可直接使用的Query和SettingStubFunc函数

        internal static bool scpiQuy_Timbase(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (TryGetPropertyInfoByUsingDeclareTablePrsntObject(analyResult, out PropertyInfo propertyInfo))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyValue(scpiTagObj.PrsntObj, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;
                    sendMessage.UsingScientificNotation = usingScientific;
                    sendMessage.SendData = decodeStr(outputString);
                    return true;
                }
            }
            return false;
        }
        //        internal static Boolean IsTimeOrFreq(String propertyInfoName)
        //        {
        //            var uppername = propertyInfoName.ToUpper();
        //            var isfreq = uppername.Contains("FREQ");
        //            var istime = false;
        //            foreach (var unit in TimeUnit)
        //            {
        //                if ((uppername.Contains("POS") || uppername.Contains("BIAS") || uppername.Contains("BITS") || uppername.Contains("THRES") || uppername.Contains("PHASE") || uppername.Contains("SET"))
        //&& !uppername.Contains("BYUS") && !uppername.Contains("BYMHZ"))
        //                {
        //                    istime = false;
        //                    break;
        //                }
        //                if (propertyInfoName.Contains(unit))
        //                {
        //                    istime = true;
        //                    break;
        //                }
        //                else
        //                    istime = false;
        //            }
        //            return istime || isfreq;
        //        }

        public static bool scpiQuy_CommonByUsingDeclareTable(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (TryGetPropertyInfoByUsingDeclareTablePrsntObject(analyResult, out PropertyInfo propertyInfo))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyValue(scpiTagObj.PrsntObj, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;// !IsTimeOrFreq(propertyInfo.Name);
                    sendMessage.UsingScientificNotation = usingScientific;
                    sendMessage.SendData = decodeStr(outputString);
                    return true;
                }
            }
            return false;
        }
        public static bool scpiSet_CommonByUsingDeclareTable(SCPICommandProcessFuncParam analyResult)
        {
            if (!scpiSet_ParamCheck(analyResult, out string param))
            {
                return false;
            }
            if (TryGetPropertyInfoByUsingDeclareTablePrsntObject(analyResult, out PropertyInfo propertyInfo))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (scpiTagObj.PropertyName == nameof(Presenter.Timebase.SegmentActive) && (param.ToString().ToUpper() == "ON" || param.ToString() == "1"))
                {
                    Presenter?.SetMutexFunctionFlag();
                }
                if (TrySetPropertyValue(scpiTagObj.PrsntObj, propertyInfo, param, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                    return true;
            }
            return false;
        }
        public static bool scpiQuy_CommonByUsingDeclareTableDivide10_6(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (TryGetPropertyInfoByUsingDeclareTablePrsntObject(analyResult, out PropertyInfo propertyInfo))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyValue(scpiTagObj.PrsntObj, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, 1000_000))
                {
                    sendMessage.SendData = decodeStr(outputString);
                    sendMessage.UsingScientificNotation = usingScientific;
                    return true;
                }
            }
            return false;
        }
        #region 调试信息
        private static void PrintDebug(List<Byte> inputMessage, List<byte> outputMessage)
        {
            //todo
        }
        private static List<byte> ConvertInputData(SCPICommandProcessFuncParam analyResult, int MaxLength = 100)
        {
            StringBuilder outputStrBuilder = new StringBuilder();
            outputStrBuilder.Append(">> " + analyResult.CmdPath + " ");
            bool bFirst = true;
            if (analyResult.Params != null)
            {
                foreach (byte[] param in analyResult.Params)
                {
                    if (param != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (byte b in param)
                            sb.Append((char)b);
                        if (!bFirst)
                        {
                            outputStrBuilder.Append("," + sb.ToString());
                        }
                        else
                        {
                            outputStrBuilder.Append(sb.ToString());
                            bFirst = false;
                        }
                    }
                }
            }
            string outputStr = outputStrBuilder.ToString();
            if (outputStr.Length > MaxLength)
                outputStr = outputStr.Substring(0, MaxLength);

            return new List<byte>(decodeStr(outputStr));
        }
        private static List<byte> ConvertOutputData(string resultStr, int MaxLength = 100)
        {
            List<byte> result = new List<byte>();
            if (resultStr == "")
                return result;
            if (resultStr.Length > MaxLength)
                resultStr = resultStr.Substring(0, MaxLength);
            result.AddRange(decodeStr(resultStr));
            return result;
        }
        private static List<byte> ConvertOutputData(byte[] resultBytes, bool bIsBinFormat = false, int MaxLength = 100)
        {
            List<byte> result = new List<byte>();
            if (resultBytes == null)
                return result;
            if (bIsBinFormat)
                result.AddRange(decodeStr("#9" + resultBytes.Length.ToString().PadLeft(9, '0')));
            result.AddRange(resultBytes);
            if (resultBytes.Length > MaxLength)
                result.RemoveRange(MaxLength, resultBytes.Length - MaxLength);
            return result;
        }
        private static byte[] ConvertBinDataFromScpiData(byte[] origin)
        {
            if (origin[0] != 0x23)
            {
                return origin;
            }
            int lengthMarkLen = origin[1] - '0';

            lengthMarkLen = lengthMarkLen < 0 ? 0 : lengthMarkLen;

            StringBuilder lenStr = new StringBuilder("");
            lenStr.Append(Encoding.ASCII.GetChars(origin, 2, lengthMarkLen));

            int dataLength = 0;
            byte[] resultData = new byte[dataLength];
            if (int.TryParse(lenStr.ToString(), out dataLength))
            {
                resultData = new byte[dataLength];
                Array.Copy(origin, lengthMarkLen + 2, resultData, 0, dataLength);
            };
            return resultData;
        }
        #endregion 调试信息

        #endregion 基础方法

        #region 参数检查
        /// <summary>
        /// 参数检查
        /// </summary>
        /// <param name="analyResult">传入对象</param>
        /// <returns></returns>
        private static bool scpiSet_ParamCheck(SCPICommandProcessFuncParam analyResult)
        {
            return scpiSet_ParamCheck(analyResult, out string paraStr);
        }
        /// <summary>
        /// 参数检查
        /// </summary>
        /// <param name="analyResult">传入对象</param>
        /// <param name="paraStr">返回参数</param>
        /// <returns></returns>
        private static bool scpiSet_ParamCheck(SCPICommandProcessFuncParam analyResult, out string paraStr)
        {
            paraStr = "";
            if (analyResult == null || analyResult.Params == null || analyResult.Params.Count == 0
                || string.IsNullOrWhiteSpace(encodingBytes(analyResult.Params[0])))
            {
                //todo msg - 参数为空
                return false;
            }
            paraStr = encodingBytes(analyResult.Params[0]).Trim();
            return true;
        }
        #endregion 参数检查

        #region 数字转科学计数法
        /// <summary>
        /// 科学计数法
        /// </summary>
        /// <param name="num">值</param>
        /// <param name="digit">保留小数位数</param>
        /// <returns></returns>
        private static string scientific(double num, uint digit = 4)
        {
            string digitStr = $"E{digit}";
            return num.ToString(digitStr);
        }
        #endregion   数字转科学计数法

        #region 二进制数据转化为十进制

        private static Int32? StringToInt32(String numString)
        {
            if (IsBinaryString(numString))
            {
                return Convert.ToInt32(numString, 2);
            }
            else
                return null;
        }

        private static Boolean IsBinaryString(String str)
        {
            // 正则表达式，匹配只包含0和1的字符串
            string pattern = @"^[01]+$";
            return Regex.IsMatch(str, pattern);
        }

        #endregion

        #region 十进制数据转化为二进制

        private static String ConverToBin(String num)
        {
            var number = Convert.ToInt32(num);
            return Convert.ToString(number, 2);
        }
        #endregion
    }
}

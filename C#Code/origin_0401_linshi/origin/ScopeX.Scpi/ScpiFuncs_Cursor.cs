using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.SCPIManager;
using ScopeX.Core;
using System.Reflection;
using System.ComponentModel;
using ScopeX.Core.Decode;
using ScopeX.Core.Tools;
using ScopeX.ComModel;
using System.Text.RegularExpressions;

namespace ScopeX.Scpi
{
    partial class StubFunc
    {
        //================= 光标 =================================================================================================

        /// <summary>
        /// 查询 光标类型
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static Boolean scpiQuy_CursorType(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            Boolean returnResult = false;
            CursorPrsnt prsnt = Presenter.Cursor;

            if (!prsnt.Active)
            {
                sendMessage.SendData = decodeStr("CLOSe");
                returnResult = true;
            }
            else
            {
                switch (prsnt.Type)
                {
                    case ComModel.CursorType.Horizontal:
                        sendMessage.SendData = decodeStr("AMPlitude");
                        returnResult = true;
                        break;
                    case ComModel.CursorType.Vertical:
                        sendMessage.SendData = decodeStr("TIME");
                        returnResult = true;
                        break;
                    case ComModel.CursorType.HorizontalVertical:
                        sendMessage.SendData = decodeStr("SCReen");
                        returnResult = true;
                        break;
                    case ComModel.CursorType.XY:
                        sendMessage.SendData = decodeStr("XY");
                        returnResult = true;
                        break;
                    default:
                        break;
                }

            }
            return returnResult;
        }

        /// <summary>
        /// 设置 光标类型
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static Boolean scpiSet_CursorType(SCPICommandProcessFuncParam analyResult)
        {
            var prsnt = Presenter.Cursor;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (analyResult.Params == null || analyResult.Params.Count() <= 0)
            {
                return false;
            }

            if (analyResult.Tag is not ScpiTagObj)
            {
                return false;
            }

            var scpiTag = (ScpiTagObj)analyResult.Tag;
            String cmd = encodingBytes(analyResult.Params[0]).Trim();
            var paramlist = scpiTag.ParamList;
            if (paramlist == null || paramlist.Count() <= 0)
            {
                return false;
            }
            var foundindex = paramlist.FindIndex(param => shortCMD(param) == cmd || param.ToUpper() == cmd.ToUpper());
            if (foundindex == -1)
            {
                return false;
            }
            if (foundindex == paramlist.Count() - 1)//close
            {
                prsnt.Active = false;
                return true;
            }
            prsnt.Active = true;
            prsnt.Type = (CursorType)foundindex;
            return true;
        }

        /// <summary>
        /// 光标通用查询
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static Boolean scpiQuy_CursorCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            Boolean returnResult = false;
            //CursorPrsnt prsnt = Presenter.Cursor;
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (scpiTagObj.PrsntObj == null)
            {
                return false;
            }
            if (TryGetPropertyInfo(scpiTagObj.PrsntObj, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                if (TryGetPropertyValue(scpiTagObj.PrsntObj, propertyInfo, out Boolean usingScientific, out String outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.SendData = decodeStr(outputString);
                    sendMessage.UsingScientificNotation = usingScientific;
                    returnResult = true;
                }
            }

            return returnResult;
        }

        /// <summary>
        /// 光标通用设置
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static Boolean scpiSet_CursorCommon(SCPICommandProcessFuncParam analyResult)
        {
            Boolean returnResult = false;
            //CursorPrsnt prsnt = Presenter.Cursor;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }

            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (scpiTagObj.PrsntObj == null)
            {
                return false;
            }
            if (TryGetPropertyInfo(scpiTagObj.PrsntObj, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                List<String> param = ParamListToStrList(analyResult.Params);
                if (param.Count > 0)
                {
                    if (TrySetPropertyValue(scpiTagObj.PrsntObj, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        returnResult = true;
                }
            }

            return returnResult;
        }

        /// <summary>
        /// 查询 光标位置
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static Boolean scpiQuy_CursorCUR(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            Boolean returnResult = false;
            CursorPrsnt prsnt = Presenter.Cursor;
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (!int.TryParse(scpiTagObj.Tag.ToString(), out int index))
            {
                return false;
            }

            if (prsnt.Current != null && prsnt.Current.Count > index)
            {
                Double? pos = 0.0;
                Double value = 0.0;
                if (scpiTagObj.PropertyName == nameof(CursorPrsnt.VCursor))
                {
                    pos = prsnt.VCursor[index];
                    var relpos = prsnt.VCursor.GetRelPosByAxis((Double)pos);
                    value = Quantity.ConvertByPrefix(relpos.Value, relpos.Pfx, Prefix.Empty);
                }
                if (scpiTagObj.PropertyName == nameof(CursorPrsnt.HCursor))
                {
                    pos = prsnt.HCursor[index];
                    var relpos = prsnt.HCursor.GetRelPosByAxis((Double)pos);
                    value = Quantity.ConvertByPrefix(relpos.Value, relpos.Pfx, Prefix.Empty);
                }

                sendMessage.UsingScientificNotation = true;
                sendMessage.UseShortScientificNotation = true;
                sendMessage.SendData = decodeStr(value.ToString());
                returnResult = true;
            }

            //if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            //{
            //	if (TryGetPropertyArrayValue(prsnt, propertyInfo, out String outputString, index))
            //	{
            //		sendMessage.SendData = decodeStr(outputString);
            //		returnResult = true;
            //	}
            //}
            return returnResult;
        }

        /// <summary>
        /// 设置 光标位置
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static Boolean scpiSet_CursorCUR(SCPICommandProcessFuncParam analyResult)
        {
            Boolean returnResult = false;
            //CursorPrsnt prsnt = Presenter.Cursor;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return returnResult;
            }

            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (!Int32.TryParse(scpiTagObj.Tag.ToString(), out var index))
            {
                return returnResult;
            }
            List<String> param = ParamListToStrList(analyResult.Params);
            var cursorbarprsnt = scpiTagObj.PropertyName == nameof(CursorPrsnt.VCursor) ? Presenter.Cursor.VCursor : Presenter.Cursor.HCursor;
            Decimal setvalue = 0;
            if (param.Count > 0)
            {
                // 定义匹配数字和小数点，并以特定单位结尾的正则表达式
                Regex regex = new Regex(@"^(?<number>[+-]?\d+(\.\d+)?([eE][+-]?\d+)?)\s*(?<unit>[a-zA-Z%]+)$");
                // 进行匹配
                Match match = regex.Match(param[0]);
                if (match.Success)
                {
                    String number = match.Groups["number"].Value;
                    String unit = match.Groups["unit"].Value;
                    if (unit == "%" || unit == "°")//特殊单位
                    {
                        var valuebydecimal = (Decimal)(Double.Parse(number));//long类型的整数与Double相乘时，可能会出现精度损失；
                        setvalue = (valuebydecimal * scpiTagObj.IntOrDoubleMultiplier);
                    }
                    else
                    {
                        setvalue = StringToDecimal(number, unit);
                    }
                }
                else if (scpiTagObj.IntOrDoubleMultiplier != 0 && Double.TryParse(param[0], out Double value))
                {
                    var valueBydecimal = (Decimal)value;//long类型的整数与Double相乘时，可能会出现精度损失；
                    setvalue = (valueBydecimal * scpiTagObj.IntOrDoubleMultiplier);
                }

                var newvalue = 0.0;
                if (scpiTagObj.PropertyName == nameof(CursorPrsnt.HCursor))
                {
                    if (Presenter.TryGetChannel(cursorbarprsnt.Source, out var cprsnt))
                    {
                        newvalue = Quantity.ConvertByPrefix((Double)setvalue, Prefix.Milli, cprsnt.Prefix);
                    }
                }
                else
                    newvalue = Quantity.ConvertByPrefix((Double)setvalue, Prefix.Micro, Presenter.Timebase.Prefix);
                cursorbarprsnt[index] = cursorbarprsnt.GetPosIndex(newvalue);
                returnResult = true;
                //if (param.Count > 0 && Double.TryParse(param[0], out Double value))
                //{
                //    cursorbarprsnt[index] = Quantity.ConvertByPrefix(value, Prefix.Empty, Prefix.Milli);
                //    returnResult = true;
                //}
            }
            //if (param.Count > 0 && Double.TryParse(param[0], out Double value))
            //{
            //    cursorbarprsnt[index] = Quantity.ConvertByPrefix(value, Prefix.Empty, Prefix.Milli);
            //    returnResult = true;
            //}

            return returnResult;
        }
        public static Boolean scpiQuy_CursorMeasureValue(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            Boolean returnResult = false;

            CursorPrsnt prsnt = Presenter.Cursor;
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (!int.TryParse(scpiTagObj.Tag.ToString(), out int index))
            {
                return false;
            }
            var cursorbarprsnt = scpiTagObj.PropertyName == nameof(CursorPrsnt.VCursor) ? Presenter.Cursor.VCursor : Presenter.Cursor.HCursor;

            Double value = 0.0D;
            switch (scpiTagObj.PropertyName)
            {
                case nameof(CursorPrsnt.VCursor):
                    value = VCursorToString(index);
                    sendMessage.SendData = decodeStr($"{value}");
                    returnResult = true;
                    break;
                case nameof(CursorPrsnt.HCursor):
                    value = HCursorToString(index);
                    sendMessage.SendData = decodeStr($"{value}");
                    returnResult = true;
                    break;
                default:
                    break;
            }
            return returnResult;
        }
        private static Double VCursorToString(Int32 index)
        {
            var (ax, axp, axu) = Presenter.Cursor.VCursor.GetPosInfo(Presenter.Cursor.VCursor[index]!.Value);
            var value = Quantity.ConvertByPrefix(ax, axp, Prefix.Empty);
            return value;
        }
        private static Double HCursorToString(Int32 index)
        {
            var (ax, axp, axu) = Presenter.Cursor.HCursor.GetPosInfo(Presenter.Cursor.HCursor[index]!.Value);
            var value = Quantity.ConvertByPrefix(ax, axp, Prefix.Empty);
            return value;
        }
    }
}

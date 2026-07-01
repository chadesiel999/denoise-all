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
        //================= LA =================================================================================================

        public static bool scpiQuy_LACommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetLAChannelPrsnt(analyResult, out DigitalPrsnt prsnt))
            {
                if (analyResult.ChannelIndex >= prsnt.BitLength)
                {
                    return false;
                }

                var channelindex = Math.Clamp(analyResult.ChannelIndex, 0, prsnt.BitLength - 1);

                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                switch ((string)scpiTagObj.Tag)
                {
                    case "DISP":
                        var act = prsnt.GetActiveAt(channelindex);
                        sendMessage.SendData = decodeStr(act ? "1" : "0");
                        sendMessage.UsingScientificNotation = false;
                        returnResult = true;
                        break;
                    case "OFFS":
                        var offset = prsnt.GetPosIndexAt(channelindex);
                        sendMessage.SendData = decodeStr($"{offset / scpiTagObj.IntOrDoubleMultiplier}");
                        returnResult = true;
                        break;
                    case "LAB":
                        var value = prsnt.GetLabelAt(channelindex);
                        sendMessage.SendData = decodeStr($"{value}");
                        sendMessage.UsingScientificNotation = false;
                        returnResult = true;
                        break;
                    case "THRE":
                        var family = prsnt.GetFamilyAt(channelindex);
                        if ((Int32)family > scpiTagObj.ParamList.Count - 1)
                        {
                            return false;
                        }
                        sendMessage.SendData = decodeStr($"{scpiTagObj.ParamList[(Int32)family]}");
                        returnResult = true;
                        break;
                    case "LEV":
                        var level = prsnt.GetUserThroldAt(channelindex);
                        sendMessage.UsingScientificNotation = true;
                        sendMessage.UseShortScientificNotation = true;
                        sendMessage.SendData = decodeStr($"{level / (scpiTagObj.IntOrDoubleMultiplier == 0 ? 1000 : scpiTagObj.IntOrDoubleMultiplier)}");

                        returnResult = true;
                        break;
                    case "HEIG":
                        var bithigh = prsnt.BitHeightOpt;
                        if ((Int32)bithigh > scpiTagObj.ParamList.Count - 1)
                        {
                            return false;
                        }
                        sendMessage.SendData = decodeStr($"{scpiTagObj.ParamList[(Int32)bithigh]}");
                        returnResult = true;
                        break;
                    case "SER":
                        var series = prsnt.GetFamilyAtGrp(channelindex);
                        if ((Int32)series > scpiTagObj.ParamList.Count - 1)
                        {
                            return false;
                        }
                        sendMessage.SendData = decodeStr($"{scpiTagObj.ParamList[(Int32)series]}");
                        returnResult = true;
                        break;
                    case "GROUPTHR":
                        var groupthr = prsnt.GetUserThroldAtGrp(channelindex);
                        sendMessage.UsingScientificNotation = true;
                        sendMessage.UseShortScientificNotation = true;
                        sendMessage.SendData = decodeStr($"{groupthr / (scpiTagObj.IntOrDoubleMultiplier == 0 ? 1000 : scpiTagObj.IntOrDoubleMultiplier)}");
                        returnResult = true;
                        break;

                    default:
                        break;

                }
                //更具需要添加
            }
            return returnResult;
        }

        public static bool scpiSet_LACommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetLAChannelPrsnt(analyResult, out DigitalPrsnt prsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                List<string> param = ParamListToStrList(analyResult.Params);
                if (param.Count <= 0)
                {
                    return false;
                }
                if (analyResult.ChannelIndex >= prsnt.BitLength)
                {
                    return false;
                }
                var channelindex = Math.Clamp(analyResult.ChannelIndex, 0, prsnt.BitLength - 1);
                var valueString = param[0];
                try
                {
                    switch ((string)scpiTagObj.Tag)
                    {
                        case "DISP":
                            if (scpiTagObj.ParamList.Count != 2)
                            {
                                return false;
                            }

                            if (valueString == scpiTagObj.ParamList[1] || valueString == "1")
                            {
                                Presenter.SetMutexFunctionFlag();
                                prsnt.SetActiveAt(channelindex, true);
                            }
                            else if (valueString == scpiTagObj.ParamList[0] || valueString == "0")
                                prsnt.SetActiveAt(channelindex, false);
                            returnResult = true;
                            break;
                        case "OFFS":
                            double.TryParse(valueString, out double levValue);
                            prsnt.SetPosIndexAt(channelindex, levValue * scpiTagObj.IntOrDoubleMultiplier);
                            returnResult = true;
                            break;
                        case "LAB":
                            prsnt.SetLabelAt(channelindex, valueString);
                            returnResult = true;
                            break;
                        case "THRE":
                            if (scpiTagObj.ParamList.Count <= 0)
                            {
                                return false;
                            }
                            var findindex = scpiTagObj.ParamList.FindIndex(s => s.ToUpper() == valueString.ToUpper() || shortCMD(s) == valueString);
                            if (findindex == -1)
                            {
                                return false;
                            }

                            prsnt.SetFamilyAt(channelindex, (DigiTholdFamily)findindex);
                            returnResult = true;
                            break;
                        case "LEV":
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
                                    var valueBydecimal = (decimal)(double.Parse(number));//long类型的整数与double相乘时，可能会出现精度损失；
                                    valueString = (valueBydecimal * scpiTagObj.IntOrDoubleMultiplier).ToString();
                                }
                                else
                                    valueString = StringToDecimal(number, unit).ToString();
                                double.TryParse(valueString, out double setvalue);
                                prsnt.SetUserThroldAt(channelindex, setvalue);
                                returnResult = true;
                            }
                            else if (scpiTagObj.IntOrDoubleMultiplier != 0 && double.TryParse(valueString, out double value))
                            {
                                var valueBydecimal = (decimal)value;//long类型的整数与double相乘时，可能会出现精度损失；
                                var setvalue = valueBydecimal * scpiTagObj.IntOrDoubleMultiplier;
                                prsnt.SetUserThroldAt(channelindex, (double)setvalue);
                                returnResult = true;
                            }
                            else
                            {
                                return false;
                            }

                            break;
                        case "AUTL":
                            if (valueString == scpiTagObj.ParamList[1] || valueString == "1")
                            {
                                prsnt.AutoLocate();
                            }
                            break;
                        case "ALLD":
                            if (valueString == scpiTagObj.ParamList[1] || valueString == "1")
                            {
                                prsnt.OpenAllDigital();
                            }
                            else if (valueString == scpiTagObj.ParamList[0] || valueString == "0")
                            {
                                prsnt.CloseAllDigital();
                            }
                            returnResult = true;
                            break;
                        case "HEIG":
                            findindex = scpiTagObj.ParamList.FindIndex(s => s.ToUpper() == valueString.ToUpper() || shortCMD(s) == valueString);
                            if (findindex == -1)
                            {
                                return false;
                            }
                            prsnt.BitHeightOpt = (DigiHeightOpt)findindex;
                            returnResult = true;
                            break;
                        case "SER":
                            if (scpiTagObj.ParamList.Count <= 0)
                            {
                                return false;
                            }
                            findindex = scpiTagObj.ParamList.FindIndex(s => s.ToUpper() == valueString.ToUpper() || shortCMD(s) == valueString);
                            if (findindex == -1)
                            {
                                return false;
                            }
                            prsnt.SetFamilyAtGrp(channelindex, (DigiTholdFamily)findindex);
                            returnResult = true;
                            break;
                        case "GROUPTHR":
                            if (Double.TryParse(valueString, out var thrvalue))
                            {
                                prsnt.SetUserThroldAtGrp(channelindex, thrvalue * scpiTagObj.IntOrDoubleMultiplier);
                                returnResult = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
                catch
                { }
            }
            return returnResult;
        }

        public static bool TryGetLAChannelPrsnt(SCPICommandProcessFuncParam analyResult, out DigitalPrsnt prsnt)
        {
            prsnt = null;

            if (Presenter.TryGetRange(c => c.Id.IsDigital()).FirstOrDefault() is not DigitalPrsnt dprsnt)
            {
                return false;
            }
            prsnt = dprsnt;
            return true;
        }

    }
}

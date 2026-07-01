using ScopeX.ComModel;
using ScopeX.Core.Decode;
using ScopeX.Core.Tools.DataExport;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {

        public static bool scpiQuy_BusDataCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                var titles = prsnt.DecodeChPrsnt.EventInfoTitles;
                if (titles == null || !titles.Any())
                    return false;
                var temp = prsnt.DecodeChPrsnt.ProtocolEvents.Where(x => x != null).ToList();
                if (temp == null)
                    return false;
                StringBuilder stringBuilder = new StringBuilder();
                var outputstring = prsnt.ProtocolType.GetDisplay() + "," + Environment.NewLine;
                var result = DataExportHelper.GetDecodeData(temp, titles.Count, (0, temp.Count));

                outputstring += String.Join(",", titles) + Environment.NewLine;
                stringBuilder.Append(outputstring);
                stringBuilder.Append(result);

                sendMessage.SendData = decodeStr(stringBuilder.ToString());
                sendMessage.IsDataBlock = true;
                returnResult = true;
            }
            return returnResult;
        }
        public static bool scpiQuy_BusCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                {
                    if (TryGetPropertyValue(prsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                    {
                        sendMessage.SendData = decodeStr(outputString);
                        sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                        sendMessage.UsingScientificNotation = usingScientific;
                        returnResult = true;
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiQuy_RS232Common(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is RS232DecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(dp, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                            sendMessage.SendData = decodeStr(outputString);
                            sendMessage.UsingScientificNotation = usingScientific;
                            returnResult = true;
                        }
                    }
                }
            }

            return returnResult;
        }

        public static bool scpiQuy_I2CCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is I2CDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(dp, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                            sendMessage.SendData = decodeStr(outputString);
                            sendMessage.UsingScientificNotation = usingScientific;
                            returnResult = true;
                        }
                    }
                }
            }

            return returnResult;
        }

        public static bool scpiQuy_SPICommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is SPIDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(dp, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                            sendMessage.SendData = decodeStr(outputString);
                            sendMessage.UsingScientificNotation = usingScientific;
                            returnResult = true;
                        }
                    }
                }
            }

            return returnResult;
        }

        public static bool scpiQuy_CanCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is CANDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(dp, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;// !IsTimeOrFreq(propertyInfo.Name);
                            sendMessage.SendData = decodeStr(outputString);
                            sendMessage.UsingScientificNotation = usingScientific;
                            returnResult = true;
                        }
                    }
                }
            }

            return returnResult;
        }

        public static bool scpiQuy_CanFdCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is CANFDDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(dp, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                            sendMessage.SendData = decodeStr(outputString);
                            sendMessage.UsingScientificNotation = usingScientific;
                            returnResult = true;
                        }
                    }
                }
            }

            return returnResult;
        }

        public static bool scpiQuy_LINCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is LINDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(dp, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                            sendMessage.SendData = decodeStr(outputString);
                            sendMessage.UsingScientificNotation = usingScientific;
                            returnResult = true;
                        }
                    }
                }
            }

            return returnResult;
        }

        public static bool scpiQuy_FRCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is FlexRayDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(dp, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                            sendMessage.SendData = decodeStr(outputString);
                            sendMessage.UsingScientificNotation = usingScientific;
                            returnResult = true;
                        }
                    }
                }
            }

            return returnResult;
        }

        public static bool scpiQuy_ABCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is AudioBusDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(dp, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                            sendMessage.SendData = decodeStr(outputString);
                            sendMessage.UsingScientificNotation = usingScientific;
                            returnResult = true;
                        }
                    }
                }
            }

            return returnResult;
        }

        public static bool scpiQuy_MSCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is MILDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(dp, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                            sendMessage.SendData = decodeStr(outputString);
                            sendMessage.UsingScientificNotation = usingScientific;
                            returnResult = true;
                        }
                    }
                }
            }

            return returnResult;
        }

        public static bool scpiQuy_A429Common(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is ARINC429DecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    var propertyName = scpiTagObj.PropertyName;
                    if (scpiTagObj.Tag != null)
                    {
                        string tagStr = scpiTagObj.Tag.ToString();
                        switch (tagStr)
                        {
                            case "Source":
                                if (analyResult.ChannelIndexs.Count > 1 && analyResult.ChannelIndexs[1] == 2)
                                {
                                    propertyName = "Source2";
                                }
                                else
                                {
                                    propertyName = "Source1";
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    if (TryGetPropertyInfo(dp, propertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(dp, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                            sendMessage.SendData = decodeStr(outputString);
                            sendMessage.UsingScientificNotation = usingScientific;
                            returnResult = true;
                        }
                    }
                }
            }

            return returnResult;
        }

        public static bool scpiQuy_UsbCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is USBDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    var propertyName = scpiTagObj.PropertyName;
                    if (scpiTagObj.Tag != null)
                    {
                        string tagStr = scpiTagObj.Tag.ToString();
                        switch (tagStr)
                        {
                            case "Source":
                                if (analyResult.ChannelIndexs.Count > 1 && analyResult.ChannelIndexs[1] == 2)
                                {
                                    propertyName = "Source2";
                                }
                                else
                                {
                                    propertyName = "Source1";
                                }
                                break;
                            case "Threshold":
                                if (analyResult.ChannelIndexs.Count > 1 && analyResult.ChannelIndexs[1] == 2)
                                {
                                    propertyName = "Source2Threshold";
                                }
                                else
                                {
                                    propertyName = "Source1Threshold";
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    if (TryGetPropertyInfo(dp, propertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(dp, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                            sendMessage.SendData = decodeStr(outputString);
                            sendMessage.UsingScientificNotation = usingScientific;
                            returnResult = true;
                        }
                    }
                }
            }

            return returnResult;
        }

        public static bool scpiQuy_SentCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is SENTDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(dp, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                            sendMessage.SendData = decodeStr(outputString);
                            sendMessage.UsingScientificNotation = usingScientific;
                            returnResult = true;
                        }
                    }
                }
            }

            return returnResult;
        }

        public static bool scpiQuy_SpmiCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is SPMIDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(dp, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                            sendMessage.SendData = decodeStr(outputString);
                            sendMessage.UsingScientificNotation = usingScientific;
                            returnResult = true;
                        }
                    }
                }
            }

            return returnResult;
        }

        public static bool scpiSet_BusCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                {
                    List<string> param = ParamListToStrList(analyResult.Params);
                    if (param.Count > 0)
                    {
                        if (scpiTagObj.PropertyName == nameof(prsnt.Active) && (param[0].ToString().ToUpper() == "ON" || param[0].ToString() == "1"))
                        {
                            Presenter?.SetMutexFunctionFlag();
                        }
                        if (TrySetPropertyValue(prsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                            returnResult = true;
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_RS232Common(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is RS232DecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            if (TrySetPropertyValue(dp, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_I2CCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is I2CDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            if (TrySetPropertyValue(dp, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_SPICommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is SPIDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            if (TrySetPropertyValue(dp, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_CanCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is CANDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            if (TrySetPropertyValue(dp, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_CanFdCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is CANFDDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            if (TrySetPropertyValue(dp, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_LINCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is LINDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            if (TrySetPropertyValue(dp, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_FRCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is FlexRayDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            if (TrySetPropertyValue(dp, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_ABCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is AudioBusDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            if (TrySetPropertyValue(dp, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_MSCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is MILDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            if (TrySetPropertyValue(dp, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_A429Common(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is ARINC429DecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    var propertyName = scpiTagObj.PropertyName;
                    if (scpiTagObj.Tag != null)
                    {
                        string tagStr = scpiTagObj.Tag.ToString();
                        switch (tagStr)
                        {
                            case "Source":
                                if (analyResult.ChannelIndexs.Count > 1 && analyResult.ChannelIndexs[1] == 2)
                                {
                                    propertyName = "Source2";
                                }
                                else
                                {
                                    propertyName = "Source1";
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    if (TryGetPropertyInfo(dp, propertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            if (TrySetPropertyValue(dp, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_UsbCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is USBDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    var propertyName = scpiTagObj.PropertyName;
                    if (scpiTagObj.Tag != null)
                    {
                        string tagStr = scpiTagObj.Tag.ToString();
                        switch (tagStr)
                        {
                            case "Source":
                                if (analyResult.ChannelIndexs.Count > 1 && analyResult.ChannelIndexs[1] == 2)
                                {
                                    propertyName = "Source2";
                                }
                                else
                                {
                                    propertyName = "Source1";
                                }
                                break;
                            case "Threshold":
                                if (analyResult.ChannelIndexs.Count > 1 && analyResult.ChannelIndexs[1] == 2)
                                {
                                    propertyName = "Source2Threshold";
                                }
                                else
                                {
                                    propertyName = "Source1Threshold";
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    if (TryGetPropertyInfo(dp, propertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            if (TrySetPropertyValue(dp, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_SentCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is SENTDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            switch (param[0])
                            {
                                case "Fast":
                                    param[0] = "FastChannel";
                                    break;
                                case "Slow":
                                    param[0] = "SlowChannel";
                                    break;
                                case "Nib":
                                    param[0] = "Nibbles";
                                    break;
                                
                            }
                            if (TrySetPropertyValue(dp, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_SpmiCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetBusPrsnt(analyResult, out var prsnt))
            {
                if (prsnt.DecodeChPrsnt is SPMIDecodePrsnt dp)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    if (TryGetPropertyInfo(dp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            if (TrySetPropertyValue(dp, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool TryGetBusPrsnt(SCPICommandProcessFuncParam analyResult, out DecodePrsnt prsnt)
        {
            prsnt = null;
            if (!CheckBusChannel(analyResult, out var id))
            {
                return false;
            }
            var cp = Presenter.TryGetRange(c => c.Id.IsDecode()).FirstOrDefault(cp => cp.Id == id);
            if (cp == null)
            {
                return false;
            }
            if (!(cp is DecodePrsnt))
            {
                return false;
            }
            prsnt = (DecodePrsnt)cp;
            return true;
        }
        private static bool CheckBusChannel(SCPICommandProcessFuncParam analyResult, out ChannelId chnlId)
        {
            chnlId = ChannelId.B1;
            if (analyResult == null)
                return false;
            if (analyResult.ChannelIndex <= 0)
            {
                analyResult.ChannelIndex = 1;
            }
            if (analyResult.ChannelIndex + chnlId - 1 < 0 || analyResult.ChannelIndex + chnlId - 1 > ChannelIdExt.MaxBChId)
            {
                return false;
            }
            chnlId = analyResult.ChannelIndex + chnlId - 1;
            return true;
        }
    }
}

using ScopeX.ComModel;
using ScopeX.Core.Decode;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        public static bool scpiQuy_TriggerBusCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;
            var tsp = Presenter.CurrentTrigger;
            if (tsp is not TrigSerialPrsnt)
            {
                return false;
            }
            var propertyname = scpiTagObj.PropertyName;
            PropertyInfo? proInfo = prsntType?.GetProperty(propertyname);
            if (proInfo == null)
            {
                return false;
            }
            if (TryGetPropertyValue(tsp, proInfo, out bool usingScientific, out var outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
            {
                if (scpiTagObj.IsData)
                {
                    outputString = ConverToBin(outputString);
                }
                sendMessage.UsingScientificNotation = scpiTagObj.IsTimeOrFreq;
                sendMessage.SendData = decodeStr(outputString);
                sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(proInfo.Name);
                return true;
            }
            return false;
        }

        public static bool scpiSet_TriggerBusCommon(SCPICommandProcessFuncParam analyResult)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;

            if (analyResult.Params==null||analyResult.Params.Count<=0)
            {
                return false;
            }
            if (Presenter.CurrentTrigger is TrigSerialPrsnt tsp)
            {
                var param = ParamListToStrList(analyResult.Params);
                if (scpiTagObj.IsData)
                {
                    var inputparam = StringToInt32(param[0]);
                    if (inputparam == null)
                    {
                        return false;
                    }
                    param[0] = inputparam!.ToString();
                }
                var propertyname = scpiTagObj.PropertyName;
                PropertyInfo? proInfo = prsntType?.GetProperty(propertyname);
                if (proInfo == null)
                {
                    return false;
                }
                if (TrySetPropertyValue(tsp, proInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                    return true;
            }
            return false;
        }

        public static bool scpiQuy_TriggerDecodeCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;
            if (Presenter.CurrentTrigger is TrigSerialPrsnt tsp)
            {
                var dp = ProtocolPrsnt.GetTrigSerialDecodePrsnt(Presenter, tsp.SerialType, null);
                if (prsntType != dp.GetType())
                {
                    return false;
                }

                var propertyname = scpiTagObj.PropertyName;
                PropertyInfo? proInfo = prsntType?.GetProperty(propertyname);
                if (proInfo == null)
                {
                    return false;
                }
                if (TryGetPropertyValue(dp, proInfo, out bool usingScientific, out var outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.UsingScientificNotation = usingScientific;
                    sendMessage.SendData = decodeStr(outputString);
                    sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(proInfo.Name);
                    return true;
                }
            }
            return false;
        }

        public static bool scpiSet_TriggerDecodeCommon(SCPICommandProcessFuncParam analyResult)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;
            if (Presenter.CurrentTrigger is TrigSerialPrsnt tsp)
            {
                var dp = ProtocolPrsnt.GetTrigSerialDecodePrsnt(Presenter, tsp.SerialType, null);
                if (prsntType != dp.GetType())
                {
                    return false;
                }

                List<string> param = ParamListToStrList(analyResult.Params);
                var propertyname = scpiTagObj.PropertyName;
                PropertyInfo? proInfo = prsntType?.GetProperty(propertyname);
                if (proInfo == null)
                {
                    return false;
                }
                if (TrySetPropertyValue(dp, proInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                    return true;
            }
            return false;
        }

        public static bool scpiQuy_TriggerBusType(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;
            if (Presenter.CurrentTrigger is TrigSerialPrsnt tsp)
            {
                var propertyname = scpiTagObj.PropertyName;
                PropertyInfo? proInfo = prsntType?.GetProperty(propertyname);
                if (proInfo == null)
                {
                    return false;
                }
                TryGetPropertyValue(tsp, proInfo, out bool usingScientific, out var outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier);
                sendMessage.UsingScientificNotation = usingScientific;
                sendMessage.SendData = decodeStr(outputString);
                sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(proInfo.Name);
                return true;
            }
            return false;
        }

        public static bool scpiSet_TriggerBusType(SCPICommandProcessFuncParam analyResult)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;
            if (Presenter.CurrentTrigger is TrigSerialPrsnt tsp)
            {
                List<string> param = ParamListToStrList(analyResult.Params);
                var index = scpiTagObj.ParamList.FindIndex(x => x.ToUpper() == param[0].ToUpper());
                if (index == -1)
                {
                    return false;
                }
                tsp.Source = index + ChannelId.B1;
                return true;
            }
            return false;
        }
    }
}
//================= 共2个方法 =

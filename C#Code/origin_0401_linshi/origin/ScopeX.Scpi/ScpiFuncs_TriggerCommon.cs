using ScopeX.Core;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ScopeX.Scpi
{
    partial class StubFunc
    {
        public static bool scpiQuy_TriggerCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;

            TriggerPrsnt triggerPrsnt = Presenter.CurrentTrigger;
            string propertyname = string.Empty;
            if (scpiTagObj.Tag != null && analyResult.ChannelIndexs.Count > 0)
            {
                switch (scpiTagObj.Tag)
                {
                    case "Source":
                        propertyname = analyResult.ChannelIndexs[0] == 1 ? "SourceOne" : "SourceTwo";
                        break;
                    case "Slope":
                        propertyname = analyResult.ChannelIndexs[0] == 1 ? "SourceOneSlope" : "SourceTwoSlope";
                        break;
                    case "CompPosition":
                        propertyname = analyResult.ChannelIndexs[0] == 1 ? "UpperCompPosition" : "DataCompPosition";
                        break;
                    default:
                        propertyname = scpiTagObj.PropertyName;
                        break;
                }
            }
            else
                propertyname = scpiTagObj.PropertyName;
            string outputString;
            if (prsntType == typeof(TriggerPrsnt))
            {
                PropertyInfo proInfo = prsntType.GetProperty(propertyname);
                if (proInfo == null)
                {
                    return false;
                }
                TryGetPropertyValue(triggerPrsnt, proInfo, out bool usingScientific, out outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier);
                sendMessage.UsingScientificNotation=usingScientific;
                sendMessage.SendData = decodeStr(outputString);
                sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(proInfo.Name);
                return true;
            }
            else if ((Type)scpiTagObj.PrsntObj == triggerPrsnt.GetType() && TryGetPropertyValue(triggerPrsnt, propertyname, out bool usingScientific, out outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
            {
                sendMessage.UsingScientificNotation = usingScientific;
                sendMessage.SendData = decodeStr(outputString);
                sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyname);

                return true;
            }

            return false;
        }
        public static bool scpiSet_TriggerCommon(SCPICommandProcessFuncParam analyResult)
        {
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            List<string> param = ParamListToStrList(analyResult.Params);

            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;

            TriggerPrsnt triggerPrsnt = Presenter.CurrentTrigger;

            string propertyname = string.Empty;
            if (scpiTagObj.Tag != null && analyResult.ChannelIndexs.Count > 0)
            {
                switch (scpiTagObj.Tag)
                {
                    case "Source":
                        propertyname = analyResult.ChannelIndexs[0] == 1 ? "SourceOne" : "SourceTwo";
                        break;
                    case "Slope":
                        propertyname = analyResult.ChannelIndexs[0] == 1 ? "SourceOneSlope" : "SourceTwoSlope";
                        break;
                    case "CompPosition":
                        propertyname = analyResult.ChannelIndexs[0] == 1 ? "UpperCompPosition" : "DataCompPosition";
                        break;
                    default:
                        propertyname = scpiTagObj.PropertyName;
                        break;
                }
            }
            else
                propertyname = scpiTagObj.PropertyName;
            PropertyInfo propertyInfo;

            bool isTriggerPrsnt = prsntType == typeof(TriggerPrsnt);
            if (isTriggerPrsnt)
            {
                propertyInfo = ((Type)scpiTagObj.PrsntObj).GetProperty(propertyname);
                if (propertyInfo == null)
                {
                    return false;
                }

                if (TrySetPropertyValue(triggerPrsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                    return true;
            }

            else if ((Type)scpiTagObj.PrsntObj == triggerPrsnt.GetType() && TryGetPropertyInfo(triggerPrsnt, propertyname, out propertyInfo))
            {
                if (TrySetPropertyValue(triggerPrsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                    return true;
            }
            return false;
        }
        public static bool scpiSet_ResetCommon(SCPICommandProcessFuncParam analyResult)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;
            var triggerPrsnt = Presenter.CurrentTrigger;
            var prsnt = triggerPrsnt as TrigEdgePrsnt;
            prsnt.ScpiResetPos();
            return true;
        }
        public static bool scpiSet_LineResetCommon(SCPICommandProcessFuncParam analyResult)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;
            var triggerPrsnt = Presenter.CurrentTrigger;
            if (triggerPrsnt == null)
                return false;
            var prsnt = triggerPrsnt as TrigVideoPrsnt;
            prsnt.ScpiResetLine();
            return true;
        }

    }
}

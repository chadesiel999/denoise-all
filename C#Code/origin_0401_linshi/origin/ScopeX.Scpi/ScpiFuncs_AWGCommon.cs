using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.SCPIManager;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ScopeX.Scpi
{
    partial class StubFunc
    {
        //================= AWG =================================================================================================
        public static bool TryGetAWGChannelPrsnt(ChannelId chnlId, out ArbWfmGenPrsnt prsnt)
        {
            prsnt = null;
            var channel = Presenter.GetWfmGenerator(chnlId);
            if (channel == null)
            {
                return false;
            }
            if (!(channel is ArbWfmGenPrsnt))
            {
                return false;
            }
            prsnt = channel;
            return true;
        }
        public static bool TryGetAWGChannelPrsnt(SCPIManager.SCPICommandProcessFuncParam analyResult, out ArbWfmGenPrsnt prsnt)
        {
            prsnt = null;

            if (!checkChannel(analyResult, out ChannelId chnlId))
                return false;
            if ((int)chnlId + ChannelId.AWG1 == ChannelId.AWG2 && Constants.ENABLE_AWG2 == false)
            {
                return false;
            }
            var channel = Presenter.GetWfmGenerator((int)chnlId + ChannelId.AWG1);
            if (channel == null)
            {
                return false;
            }
            if (!(channel is ArbWfmGenPrsnt))
            {
                return false;
            }
            prsnt = channel;
            return true;
        }
        public static bool scpiQuy_AWGChannelCommon(SCPIManager.SCPICommandProcessFuncParam analyResult, ref SCPIManager.SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetAWGChannelPrsnt(analyResult, out ArbWfmGenPrsnt prsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                {
                    if (TryGetPropertyValue(prsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                    {
                        sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;// !IsTimeOrFreq(propertyInfo.Name);
                        sendMessage.SendData = decodeStr(outputString);
                        sendMessage.UsingScientificNotation = usingScientific;
                        returnResult = true;
                    }
                }
            }
            return returnResult;
        }
        public static bool scpiSet_AWGChannelCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetAWGChannelPrsnt(analyResult, out ArbWfmGenPrsnt prsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                {
                    List<string> param = ParamListToStrList(analyResult.Params);
                    if (param.Count > 0)
                    {
                        if (TrySetPropertyValue(prsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                            returnResult = true;
                    }
                }
            }
            return returnResult;
        }
        public static bool scpiSet_AWGChannelMODulateWave(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetAWGChannelPrsnt(analyResult, out ArbWfmGenPrsnt prsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                {
                    List<string> param = ParamListToStrList(analyResult.Params);
                    if (param.Count <= 0)
                    {
                        return false;
                    }
                    var foundindex = scpiTagObj.ParamList.FindIndex(p => p.ToUpper() == param[0].ToUpper() || shortCMD(p) == param[0]);
                    if (foundindex < 0)
                    {
                        return false;
                    }
                    prsnt.ModulatedWfm = prsnt.ModulatedSignalList[foundindex];
                }
            }
            return returnResult;
        }
        public static bool scpiSet_AWGTriggerCommon(SCPICommandProcessFuncParam analyResult)
        {
            if (analyResult == null)
            {
                return false;
            }
            if (TryGetAWGChannelPrsnt(analyResult, out ArbWfmGenPrsnt prsnt))
            {
                prsnt.WfmGenDoTriger();
                return true;
            }
            return false;
        }
    }
}

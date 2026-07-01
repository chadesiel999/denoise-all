using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        public static bool scpiQuy_AreaHisCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;
            var prsnt = Presenter.AreaHistogram;
            if (prsnt == null)
                return false;

            if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out var propertyInfo))
            {
                if (TryGetPropertyValue(prsnt, propertyInfo, out var usingScientific, out var outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.UsingScientificNotation = usingScientific;
                    sendMessage.SendData = decodeStr(outputString);
                    return true;
                }
            }

            return false;
        }

        public static Boolean scpiSet_AreaHisCommon(SCPICommandProcessFuncParam analyResult)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;
            var prsnt = Presenter.AreaHistogram;
            List<String> @params = ParamListToStrList(analyResult.Params);
            if (@params.Count <= 0)
            {
                return false;
            }
            if (scpiTagObj.Tag!=null)
            {
                if (scpiTagObj.Tag.ToString() == "Position")
                {
                    var rectangle = GetRectangle(@params, prsnt, scpiTagObj.IntOrDoubleMultiplier);
                    prsnt.RectanglePoints = rectangle;
                    return true;
                }
            }
            
            if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out var propertyInfo))
            {
                if (TrySetPropertyValue(prsnt, propertyInfo, @params[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

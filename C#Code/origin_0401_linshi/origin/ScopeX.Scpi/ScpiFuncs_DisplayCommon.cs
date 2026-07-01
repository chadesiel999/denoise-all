using ScopeX.SCPIManager;
using System;
using System.Reflection;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        public static bool scpiQuy_DisplayCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            return scpiQuy_CommonByUsingDeclareTable(analyResult, ref sendMessage);
        }
        public static bool scpiSet_DisplayCommon(SCPICommandProcessFuncParam analyResult)
        {
            return scpiSet_CommonByUsingDeclareTable(analyResult);
        }
        public static bool scpiSet_DisplayMark(SCPICommandProcessFuncParam analyResult)
        {
            if (!scpiSet_ParamCheck(analyResult, out string param))
            {
                return false;
            }
            if (TryGetPropertyInfoByUsingDeclareTablePrsntObject(analyResult, out PropertyInfo propertyInfo))
            {
                if (propertyInfo == null)
                {
                    return false;
                }
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                var setValue = !(param.ToUpper() == scpiTagObj.ParamList[0].ToUpper() || param.ToUpper() == shortCMD(scpiTagObj.ParamList[0].ToUpper()));
                propertyInfo.SetValue(scpiTagObj.PrsntObj, setValue);
                return true;
            }
            return false;
        }
    }
}

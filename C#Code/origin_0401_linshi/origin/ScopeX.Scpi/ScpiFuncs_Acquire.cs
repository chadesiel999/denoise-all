using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.SCPIManager;
using ScopeX.Core;
using System.Reflection;
using System.ComponentModel;
using ScopeX.Controls.Common.Helper;

namespace ScopeX.Scpi
{
    partial class StubFunc
    {
        //================= 采集 =================================================================================================
        //================= 采集 =================================================================================================
        /// <summary>
        /// 查询触发模式状态
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_ACQuireModeState(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult == null || analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj)
            {
                return false;
            }
            string outputString = scpiTagObj.ParamList[(int)TriggerPrsnt.State];

            sendMessage.SendData = decodeStr(outputString);
            return true;
        }
        public static bool scpiSet_ACQuireLongStorage(SCPICommandProcessFuncParam analyResult)
        {
            if (!scpiSet_ParamCheck(analyResult, out string param))
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (scpiTagObj.ParamList.Count == 0)
            {
                return false;
            }
            var index = scpiTagObj.ParamList.FindIndex(p => p == param);
            if (index == -1)
            {
                return false;
            }
            if (Presenter.Timebase.AnaChnlLengthSource[0].Key.ToUpper() == param.ToUpper())
            {
                Presenter.Timebase.StorageDepthOpt = 0;
                return true;
            }
            for (index = 0; index < Presenter.Timebase.AnaChnlLengthSource.Count; index++)
            {
                if (Presenter.Timebase.AnaChnlLengthSource[index].Key.Contains(param))
                {
                    Presenter.Timebase.StorageDepthOpt = index;
                    return true;
                }
            }
            return false;
        }
        public static bool scpiQuy_ACQuireLongStorage(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (TryGetPropertyInfoByUsingDeclareTablePrsntObject(analyResult, out PropertyInfo propertyInfo))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyValue(scpiTagObj.PrsntObj, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {

                    var index = int.Parse(outputString);
                    if (index == 0)//Auto
                        outputString = SIHelper.ValueChangeToSI(Presenter.Timebase.AnaChnlLengthSource[Presenter.Timebase.StorageDepthOpt].Value, 2, "");
                    else
                        outputString = Presenter.Timebase.AnaChnlLengthSource[index].Value.ToString();
                    sendMessage.UsingScientificNotation = true;
                    sendMessage.UseShortScientificNotation= true;
                    sendMessage.SendData = decodeStr(outputString);
                    return true;
                }
            }

            return false;
        }
    }
}

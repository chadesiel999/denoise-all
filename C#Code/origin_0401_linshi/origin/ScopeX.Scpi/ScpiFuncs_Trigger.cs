using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScopeX.Scpi
{
    partial class StubFunc
    {
        //================= 触发 =================================================================================================
        private static List<ChannelId> TriggerBaseSources = new List<ChannelId>() { ChannelId.C1, ChannelId.C2, ChannelId.C3, ChannelId.C4 };
        private static List<ChannelId> TriggerDigitSources = new List<ChannelId>()
            { ChannelId.D0, ChannelId.D1, ChannelId.D2, ChannelId.D3 ,
            ChannelId.D4, ChannelId.D5, ChannelId.D6, ChannelId.D7 ,
            ChannelId.D8, ChannelId.D9, ChannelId.D10, ChannelId.D11 ,
            ChannelId.D12, ChannelId.D13, ChannelId.D14, ChannelId.D15 ,};
        private static List<ChannelId> TriggerEdgeSources = new List<ChannelId>() { ChannelId.Ext, ChannelId.Ext5, ChannelId.AC, ChannelId.AuxIn };
        /// <summary>
        /// 设置或查询触发类型
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_TriggerType(SCPICommandProcessFuncParam analyResult)
        {
            if (analyResult.Tag is not ScpiTagObj tagObj || !scpiSet_ParamCheck(analyResult, out string param))
            {
                return false;
            }
            var paramslist = tagObj.ParamList;
            if (paramslist == null)
            {
                return false;
            }
            //命令等于全写或缩写
            var foundindex = paramslist.FindIndex(cmd => shortCMD(cmd) == param || cmd.ToUpper() == param.ToUpper());
            if (foundindex == -1)
            {
                return false;
            }
            #region  设置值 枚举处理

            var triggertypes = Enum.GetValues<TriggerType>().ToList();
            int? enumindex = null;
            for (int index = 0; index < triggertypes.Count; index++)
            {
                if (triggertypes[index].ToString().ToUpper() == paramslist[foundindex].ToUpper())
                {
                    enumindex = index;
                    break;
                }
            }

            if (enumindex == null)
            {
                enumindex = Enum.GetValues<TriggerType>().Cast<int>().FirstOrDefault(m => m == foundindex);
            }

            if (enumindex == null || enumindex < 0)
            {
                return false;
            }
            //var setValue = triggertypes.FirstOrDefault(type => type.ToString().ToUpper() == setItem.ToUpper());

            var viewLists = Presenter.CurrentTrigger.GetViewList().ToList()[0];

            TriggerPrsnt.GetOrMakeTrigger(Presenter.CurrentTrigger.Dso, triggertypes[(int)enumindex]);

            return true;

            #endregion  设置值 枚举处理
        }


        //public static bool scpiSet_TriggerSource_Edge(SCPICommandProcessFuncParam analyResult)
        //{
        //	if (!scpiSet_ParamCheck(analyResult))
        //	{
        //		return false;
        //	}
        //	List<string> param = ParamListToStrList(analyResult.Params);
        //	if (param.Count == 0)
        //	{
        //		return false;
        //	}
        //	if (!Enum.TryParse(param[0], out ChannelId source))
        //	{
        //		return false;
        //	}
        //	if (!TriggerBaseSources.Contains(source) &&
        //		!TriggerDigitSources.Contains(source) &&
        //		!TriggerEdgeSources.Contains(source))
        //	{
        //		return false;
        //	}

        //	#region  设置值 枚举处理
        //	if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
        //		return false;

        //	TriggerPrsnt triggerPrsnt = Presenter.CurrentTrigger;

        //	PropertyInfo propertyInfo;

        //	bool isTriggerPrsnt = prsntType == typeof(TriggerPrsnt);
        //	if (isTriggerPrsnt)
        //	{
        //		propertyInfo = ((Type)scpiTagObj.PrsntObj).GetProperty(scpiTagObj.PropertyName);
        //		if (propertyInfo == null)
        //		{
        //			return false;
        //		}

        //		if (TrySetPropertyValue(triggerPrsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
        //			return true;
        //	}
        //	return true;
        //	#endregion  设置值 枚举处理
        //}

        /// <summary>
        /// 查询触发模式状态
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_TriggerModeState(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult == null || analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj)
            {
                return false;
            }
            string outputString = scpiTagObj.ParamList[(int)TriggerPrsnt.State];

            sendMessage.SendData = decodeStr(outputString);
            return true;
        }
    }
}
//================= 共2个方法 =

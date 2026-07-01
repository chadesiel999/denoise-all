using ScopeX.SCPIManager;
using System.Collections.Generic;
using System.Reflection;
namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        //================= 通过失败测试 =============================================================================================
        /// <summary>
        /// 设置或查询通过失败测试数据源
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_PassFailSource(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            var prsnt = Presenter.PassFail;
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;

            if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                if (TryGetPropertyValue(prsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.UsingScientificNotation = usingScientific;
                    sendMessage.SendData = decodeStr(outputString);
                    returnResult = true;
                }
            }
            return returnResult;
        }
        /// <summary>
        /// 设置或查询通过失败测试数据源
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_PassFailSource(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            var prsnt = Presenter.PassFail;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }

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

            return returnResult;
        }
        /// <summary>
        /// 查询通过失败测试的结果表
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_PassFailDataTableCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            var prsnt = Presenter.PassFail;
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var outputstring = prsnt.GetTableResultString();
            sendMessage.SendData = decodeStr(outputstring);
            sendMessage.IsDataBlock = true;
            return true;
        }
        /// <summary>
        /// 设置或查询通过失败测试
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_PassFailCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            var prsnt = Presenter.PassFail;
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                if (TryGetPropertyValue(prsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                    sendMessage.UsingScientificNotation = usingScientific;
                    sendMessage.SendData = decodeStr(outputString);
                    returnResult = true;
                }
            }

            return returnResult;
        }
        /// <summary>
        /// 设置或查询通过失败测试
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_PassFailCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            var prsnt = Presenter.PassFail;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }

            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                List<string> param = ParamListToStrList(analyResult.Params);
                if (param.Count > 0)
                {
                    if (scpiTagObj.PropertyName == nameof(prsnt.Active) && (param[0].ToString().ToUpper() == "ON" || param[0].ToString() == "1"))
                    {
                        Presenter.SetMutexFunctionFlag();
                    }
                    if (TrySetPropertyValue(prsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        returnResult = true;
                }
            }

            return returnResult;
        }

        public static bool scpiSet_CreateOrReadPassFail(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            var prsnt = Presenter.PassFail;
            if (!prsnt.Active)
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (scpiTagObj.Tag == null)
            {
                return false;
            }
            switch (scpiTagObj.Tag.ToString())
            {
                case "Create":
                    if (Presenter.TryGetChannel(prsnt.MaskSource, out var channel))
                    {
                        if (!channel.Active)
                        {
                            return false;
                        }
                        prsnt.MakeMask();
                        return true;
                    }
                    break;
                case "Read":
                    prsnt.ReadStdMask();
                    return true;
                default:
                    break;
            }

            return returnResult;
        }

    }
}
//================= 共4个方法 =

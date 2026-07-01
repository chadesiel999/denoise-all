using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        public static bool scpiQuy_ReferenceCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            TryGetRefPrsnt(analyResult, out var prsnt);
            if (prsnt is null)
            {
                return false;
            }

            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (scpiTagObj.Tag != null && scpiTagObj.Tag.ToString() == "Hori")
            {
                if (TryGetPropertyInfo(prsnt.Sampling, scpiTagObj.PropertyName, out PropertyInfo horpropertyInfo))
                {
                    if (TryGetPropertyValue(prsnt.Sampling, horpropertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                    {
                        sendMessage.UsingScientificNotation = usingScientific;
                        sendMessage.SendData = decodeStr(outputString);
                        sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;
                        return true;
                    }
                }
                return returnResult;
            }
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
        public static bool scpiSet_ReferenceCommon(SCPICommandProcessFuncParam analyResult)
        {
            if (!scpiSet_RefParamCheck(analyResult, out var paramlist))
            {
                return false;
            }
            TryGetRefPrsnt(analyResult, out var prsnt);

            if (prsnt is null)
            {
                return false;
            }
            List<string> param = ParamListToStrList(analyResult.Params);
            if (param.Count > 0)
            {
                var scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (scpiTagObj.Tag!=null && scpiTagObj.Tag.ToString() == "Hori")
                {
                    if (TryGetPropertyInfo(prsnt.Sampling, scpiTagObj.PropertyName, out PropertyInfo horpropertyInfo))
                    {
                        if (TrySetPropertyValue(prsnt.Sampling, horpropertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                            return true;
                    }
                    return false;
                }
                if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                {
                    if (TrySetPropertyValue(prsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        return true;
                }
            }
            return false;
        }

        public static bool scpiSet_Reference(SCPICommandProcessFuncParam analyResult)
        {
            var bOK = checkRefChannel(analyResult, out var channelId);
            if (!bOK)
            {
                return false;
            }

            bOK = scpiSet_RefParamCheck(analyResult, out var paramList);
            if (!bOK || paramList == null || paramList.Length == 0)
            {
                return false;
            }
            var isactive = paramList[0] == "1" || paramList[0].ToUpper() == "ON";

            TryGetRefPrsnt(analyResult, out var cp);

            ReferencePrsnt? refPrsnt = null;
            if (isactive && paramList.Length < 2)
            {
                return false;
            }

            if (!isactive && cp is not null)
            {
                cp.Active = isactive;
                return true;
            }

            var filename = paramList[1];
            if (!File.Exists(filename))
            {
                return false;
            }
            if (cp is null)
            {
                return ReadFile(channelId, filename, ref refPrsnt, true);
            }
            else
            {
                ReferencePrsnt? rprsnt = cp;
                rprsnt.Active = false;
                Thread.Sleep(500);//等待Core U2 逻辑执行完 ？？？
                return ReadFile(channelId, filename, ref refPrsnt, false);
            }
        }

        private static Boolean ReadFile(ChannelId channelId, string fileName, ref ReferencePrsnt? prsnt, bool isAddToChannel = false)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            if (fileInfo.Extension == "." + WfmFormat.Binary.GetAlias())
            {
                if (ReferencePrsnt.TryRead(channelId, Presenter, fileName, ref prsnt))
                {
                    Presenter.AddChannel(channelId, prsnt!);
                    prsnt!.Active = true;
                    return true;
                }
            }
            if (fileInfo.Extension == "." + WfmFormat.CSV.GetAlias())
            {
                if (ReferencePrsnt.TryReadSVG(channelId, Presenter, fileName, ref prsnt))
                {
                    Presenter.AddChannel(channelId, prsnt!);
                    prsnt!.Active = true;
                    return true;
                }
            }
            return false;
        }

        public static bool TryGetRefPrsnt(SCPICommandProcessFuncParam analyResult, out ReferencePrsnt? refPrsnt)
        {
            refPrsnt = null;

            if (!checkRefChannel(analyResult, out ChannelId chnlId))
                return false;
            Presenter.TryGetChannel(chnlId, out var cp);
            if (cp is not null && cp is ReferencePrsnt prsnt)
            {
                refPrsnt = prsnt;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 通道号处理
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="chnlId"></param>
        /// <returns></returns>
        private static bool checkRefChannel(SCPICommandProcessFuncParam analyResult, out ChannelId chnlId)
        {
            chnlId = ChannelId.R1;
            if (analyResult == null)
                return false;
            if (analyResult.ChannelIndex <= 0)
            {
                analyResult.ChannelIndex = 1;
            }
            if (analyResult.ChannelIndex + chnlId - 1 < 0 || analyResult.ChannelIndex + chnlId - 1 > ChannelIdExt.MaxRChId)
            {
                return false;
            }
            chnlId = analyResult.ChannelIndex + chnlId - 1;
            return true;
        }

        /// <summary>
        /// 参数检查
        /// </summary>
        /// <param name="analyResult">传入对象</param>
        /// <param name="paraStr">返回参数</param>
        /// <returns></returns>
        private static bool scpiSet_RefParamCheck(SCPICommandProcessFuncParam analyResult, out string[]? paramslist)
        {
            if (analyResult == null || analyResult.Params == null || analyResult.Params.Count == 0 || string.IsNullOrWhiteSpace(encodingBytes(analyResult.Params[0])))
            {
                //todo msg - 参数为空
                paramslist = null;
                return false;
            }
            paramslist = new string[analyResult.Params.Count];
            for (int i = 0; i < analyResult.Params.Count; i++)
            {
                paramslist[i] = encodingBytes(analyResult.Params[i]).Trim();
            }
            return true;
        }
    }
}

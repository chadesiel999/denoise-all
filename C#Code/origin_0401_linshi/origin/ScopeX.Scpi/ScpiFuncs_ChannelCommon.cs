using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ScopeX.Scpi
{
    partial class StubFunc
    {
        #region 模拟通道

        private static List<IChnlPrsnt> allChnls
        {
            get => Presenter.GetAllChnls().ToList();
        }

        private static List<string> TimeUnit = new List<string>()
        {
            "fs",
            "ps",
            "ns",
            "us",
            "ms",
            "s",
            "ks",
            "Hz"
        };

        /// <summary>
        /// 通道号处理
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="chnlId"></param>
        /// <returns></returns>
        private static bool checkChannel(SCPICommandProcessFuncParam analyResult, out ChannelId chnlId)
        {
            chnlId = ChannelId.C1;
            if (analyResult == null)
                return false;

            var chnlsList = Enum.GetNames(typeof(ChannelId));
            if (analyResult.FirstChannelIndex < 0 || analyResult.FirstChannelIndex > chnlsList.Count())
            {
                return false;
            }
            return Enum.TryParse(chnlsList[analyResult.FirstChannelIndex - 1], out chnlId);
        }

        public static bool TryGetAnalogChannelPrsnt(SCPICommandProcessFuncParam analyResult, out AnalogPrsnt analogPrsnt)
        {
            analogPrsnt = null;

            if (!checkChannel(analyResult, out ChannelId chnlId))
                return false;
            var channel = allChnls.FirstOrDefault(chnl => chnl.Id == chnlId);
            if (channel == null)
            {
                return false;
            }
            if (!(channel is AnalogPrsnt))
            {
                return false;
            }
            analogPrsnt = (AnalogPrsnt)channel;
            return true;

        }

        public static bool scpiQuy_AnalogChannelCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetAnalogChannelPrsnt(analyResult, out AnalogPrsnt analogPrsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyInfo(analogPrsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                {
                    if (TryGetPropertyValue(analogPrsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
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
        public static bool scpiQuy_ProbeInfos(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetAnalogChannelPrsnt(analyResult, out AnalogPrsnt analogPrsnt))
            {
                if (String.IsNullOrEmpty(analogPrsnt.SerailNumber))
                {
                    sendMessage.SendData = decodeStr("null");
                    return true;
                }

                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                var infos = analogPrsnt.SerailNumber.Replace("\r", "").Split(',');
                if (infos.Length < 3)
                {
                    sendMessage.SendData = decodeStr("null");
                    return true;
                }
                switch (scpiTagObj.Tag.ToString())
                {
                    case "Model":
                        var model = infos[1];
                        sendMessage.UsingScientificNotation = false;
                        sendMessage.SendData = decodeStr(model);
                        returnResult = true;
                        break;
                    case "SN":
                    case "SerailNumber":
                        var sn = infos[2];
                        sendMessage.UsingScientificNotation = false;
                        sendMessage.SendData = decodeStr(sn);
                        returnResult = true;
                        break;
                    default:
                        break;
                }
            }
            return returnResult;
        }
        public static bool scpiSet_AnalogChannelCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetAnalogChannelPrsnt(analyResult, out AnalogPrsnt analogPrsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyInfo(analogPrsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                {
                    List<string> param = ParamListToStrList(analyResult.Params);
                    if (param.Count > 0)
                    {
                        if (TrySetPropertyValue(analogPrsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                            returnResult = true;
                    }
                }
            }
            return returnResult;
        }
        public static bool scpiSet_AnalogChannelSelect(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (analyResult != null && analyResult.ChannelIndex <= 3)
            {
                DsoPrsnt.FocusId = (ChannelId)(analyResult.ChannelIndex - 1);
                returnResult = true;
            }
            return returnResult;
        }
        public static bool scpiQuy_AnalogChannelSelect(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;

            if (TryGetPropertyInfo(DsoPrsnt.DefaultDsoPrsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                String outputString = string.Empty;
                if (DsoPrsnt.FocusId == (ChannelId)analyResult.ChannelIndex - 1)
                {
                    outputString = "1";
                }
                else
                {
                    outputString = "0";
                }
                sendMessage.SendData = decodeStr(outputString);
                sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                sendMessage.UsingScientificNotation = false;
                returnResult = true;

            }
            return returnResult;
        }

        public static bool scpiSet_ProbeInfos(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetAnalogChannelPrsnt(analyResult, out AnalogPrsnt analogPrsnt) || analogPrsnt == null)
            {

                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (scpiTagObj.Tag.ToString() == "SN")
                {
                    if(!analogPrsnt.ProbeConnected || analogPrsnt.GetProbeType(analogPrsnt.Id) == ProbeType.Null)
                    {
                        return false;
                    }
                    List<String> @params = ParamListToStrList(analyResult.Params);
                    var infos = analogPrsnt.SerailNumber.Replace("\r", "").Split(',');
                    if(infos.Length <3)
                    {
                        return false;
                    }
                    infos[2] = @params[0];
                    analogPrsnt.WriteProbeFactInfo(String.Join(',', infos));
                }
                else
                {
                    List<String> @params = ParamListToStrList(analyResult.Params);
                    if (@params.Count > 0)
                    {
                        analogPrsnt.SettingProbeInfo(@params[0]);
                        returnResult = true;
                    }
                }
            }

            return returnResult;
        }

        #endregion 模拟通道
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.SCPIManager;

namespace ScopeX.Scpi
{
    partial class StubFunc
    {
        public static bool TryGetMathChannelPrsnt(SCPICommandProcessFuncParam analyResult, out MathPrsnt mathPrsnt)
        {
            mathPrsnt = null;

            if (!checkChannel(analyResult, out ChannelId chnlId))
                return false;
            chnlId = (ChannelId)((Int32)chnlId + ChannelId.M1 - ChannelId.C1);
            var channel = allChnls.FirstOrDefault(chnl => chnl.Id == chnlId);
            if (channel == null)
            {
                return false;
            }
            if (!(channel is MathPrsnt))
            {
                return false;
            }
            mathPrsnt = (MathPrsnt)channel;
            return true;

        }
        public static bool scpiQuy_MathSource(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetMathChannelPrsnt(analyResult, out MathPrsnt prsnt))
            {
                MathType mathType = prsnt.Args.Type;
                MathArgPrsnt mathArg = prsnt.GetOrMakeArg(mathType);
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                bool isSource1 = true;
                if (analyResult.ChannelIndexs.Count > 1 && analyResult.ChannelIndexs[1] == 2)
                {
                    isSource1 = false;
                }
                if (!getMathPropertyName(isSource1, mathType, mathArg, out string propertyName))
                {
                    return false;
                }
                if (TryGetPropertyInfo(mathArg, propertyName, out PropertyInfo propertyInfo))
                {
                    if (TryGetPropertyValue(mathArg, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                    {
                        sendMessage.SendData = decodeStr(outputString);
                        sendMessage.UsingScientificNotation = usingScientific;
                        returnResult = true;
                    }
                }
            }
            return returnResult;
        }
        public static bool scpiSet_MathSource(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }

            if (TryGetMathChannelPrsnt(analyResult, out MathPrsnt prsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                MathType mathType = prsnt.Args.Type;
                MathArgPrsnt mathArg = prsnt.GetOrMakeArg(mathType);
                bool isSource1 = true;
                if (analyResult.ChannelIndexs.Count > 1 && analyResult.ChannelIndexs[1] == 2)
                {
                    isSource1 = false;
                }
                if (!getMathPropertyName(isSource1, mathType, mathArg, out string propertyName))
                {
                    return false;
                }
                if (TryGetPropertyInfo(mathArg, propertyName, out PropertyInfo propertyInfo))
                {
                    List<string> param = ParamListToStrList(analyResult.Params);
                    if (param.Count > 0)
                    {
                        if (TrySetPropertyValue(mathArg, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                            returnResult = true;
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiQuy_MathArg(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetMathChannelPrsnt(analyResult, out MathPrsnt prsnt))
            {
                MathType mathType = prsnt.Args.Type;
                MathArgPrsnt mathArg = prsnt.GetOrMakeArg(mathType);
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                var proertyName = scpiTagObj.PropertyName;
                if (scpiTagObj != null && scpiTagObj.Tag != null && (string)scpiTagObj.Tag == "StopFreq")
                {
                    if (analyResult.ChannelIndexs.Count == 1)
                    {
                        if (mathArg is MathFilterArg filterArg)
                        {
                            proertyName = nameof(filterArg.Freq1BymHz);
                        }
                    }
                    else if (analyResult.ChannelIndexs.Count == 2)
                    {
                        if (mathArg is MathFilterArg filterArg)
                        {
                            proertyName = analyResult.ChannelIndexs[1] == 1 ? nameof(filterArg.Freq1BymHz) : nameof(filterArg.Freq2BymHz);
                        }
                    }
                }
                //string prsntType = scpiTagObj.PrsntObj.ToString().Trim();

                //switch (mathType)
                //{
                //    case MathType.Binary:
                //        if (!prsntType.EndsWith("MathBinaryType"))
                //        {
                //            return false;
                //        }
                //        break;
                //    case MathType.FFT:
                //        if (!prsntType.EndsWith("MathFftType"))
                //        {
                //            return false;
                //        }
                //        break;
                //    default:
                //        return false;
                //}
                if (TryGetPropertyInfo(mathArg, proertyName, out PropertyInfo propertyInfo))
                {
                    if (TryGetPropertyValue(mathArg, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                    {
                        sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                        sendMessage.SendData = decodeStr(outputString);
                        sendMessage.UsingScientificNotation = usingScientific;
                        returnResult = true;
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_MathArg(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetMathChannelPrsnt(analyResult, out MathPrsnt prsnt))
            {
                MathType mathType = prsnt.Args.Type;
                MathArgPrsnt mathArg = prsnt.GetOrMakeArg(mathType);
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                var proertyName = scpiTagObj.PropertyName;
                if (scpiTagObj != null && scpiTagObj.Tag != null && (string)scpiTagObj.Tag == "StopFreq")
                {
                    if (analyResult.ChannelIndexs.Count == 1)
                    {
                        if (mathArg is MathFilterArg filterArg)
                        {
                            proertyName = nameof(filterArg.Freq1BymHz);
                        }
                    }
                    else if (analyResult.ChannelIndexs.Count == 2)
                    {
                        if (mathArg is MathFilterArg filterArg)
                        {
                            proertyName = analyResult.ChannelIndexs[1] == 1 ? nameof(filterArg.Freq1BymHz) : nameof(filterArg.Freq2BymHz);
                        }
                    }
                }
                //string prsntType = scpiTagObj.PrsntObj.ToString().Trim();

                //switch (mathType)
                //{
                //    case MathType.Binary:
                //        if (!prsntType.EndsWith("MathBinaryType"))
                //        {
                //            return false;
                //        }
                //        break;
                //    case MathType.FFT:
                //        if (!prsntType.EndsWith("MathFftType"))
                //        {
                //            return false;
                //        }
                //        break;
                //    default:
                //        return false;
                //}

                if (TryGetPropertyInfo(mathArg, proertyName, out PropertyInfo propertyInfo))
                {
                    List<string> param = ParamListToStrList(analyResult.Params);
                    if (param.Count > 0)
                    {
                        if (proertyName == nameof(MathFftArg.Number))//特殊处理
                        {
                            if (TrySetFFTNumberValue(mathArg, propertyInfo, param[0], scpiTagObj.ParamList))
                            {
                                returnResult = true;
                            }
                        }
                        else
                        {
                            if (TrySetPropertyValue(mathArg, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiQuy_MathFFT(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;

            if (analyResult.Tag is ScpiTagObj scpiTagObj)
            {
                if (TryGetMathChannelPrsnt(analyResult, out MathPrsnt prsnt))
                {
                    if (TryGetPropertyInfo(prsnt.FrequencyAdapter, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(prsnt.FrequencyAdapter, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.SendData = decodeStr(outputString);
                            sendMessage.UsingScientificNotation = true;
                            sendMessage.UseShortScientificNotation = false;
                            returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }
        public static bool scpiSet_MathFFT(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (analyResult.Tag is ScpiTagObj scpiTagObj)
            {
                if (TryGetMathChannelPrsnt(analyResult, out MathPrsnt prsnt))
                {
                    if (TryGetPropertyInfo(prsnt.FrequencyAdapter, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            if (TrySetPropertyValue(prsnt.FrequencyAdapter, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                returnResult = true;
                        }
                    }
                }
            }
            return returnResult;
        }

        private static bool TrySetFFTNumberValue(object nodeClassPrsntObject, PropertyInfo propertyInfo, string valueString, List<string> paramList = null)
        {
            int foundIndex = paramList.FindIndex(s => s == valueString);
            if (foundIndex < 0)
                return false;
            var enums = Enum.GetValues<FFTNumber>().ToList();
            if (foundIndex > enums.Count)
            {
                return false;
            }
            var setvalue = enums[foundIndex];
            propertyInfo.SetValue(nodeClassPrsntObject, setvalue);
            return true;
        }

        public static bool scpiQuy_MathCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetMathChannelPrsnt(analyResult, out MathPrsnt prsnt))
            {
                //MathType mathType = prsnt.Args.Type;
                //MathArgPrsnt MathArgPrsnt = prsnt.GetOrMakeArg(mathType);
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (scpiTagObj.Tag != null && scpiTagObj.Tag.ToString() == "Hori")
                {
                    TryGetPropertyInfo(prsnt.Sampling, scpiTagObj.PropertyName, out PropertyInfo horipropertyInfo);
                    if (TryGetPropertyValue(prsnt.Sampling, horipropertyInfo, out bool _usingScientific, out string _outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                    {
                        sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                        sendMessage.UsingScientificNotation = _usingScientific;
                        sendMessage.SendData = decodeStr(_outputString);
                        returnResult = true;
                    }
                    return returnResult;
                }
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
            }
            return returnResult;
        }

        public static bool scpiSet_MathCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetMathChannelPrsnt(analyResult, out MathPrsnt prsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                {
                    List<string> param = ParamListToStrList(analyResult.Params);
                    if (param.Count > 0)
                    {
                        if (scpiTagObj.PropertyName == nameof(prsnt.Active) && (param[0].ToString().ToUpper() == "ON" || param[0].ToString() == "1"))
                        {
                            Presenter?.SetMutexFunctionFlag();
                        }
                        if (scpiTagObj.Tag != null && scpiTagObj.Tag.ToString() == "Hori")
                        {
                            if (TryGetPropertyInfo(prsnt.Sampling, scpiTagObj.PropertyName, out PropertyInfo horpropertyInfo))
                            {
                                if (TrySetPropertyValue(prsnt.Sampling, horpropertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                    return true;
                            }
                            return false;
                        }
                        if (TrySetPropertyValue(prsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                            returnResult = true;
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiSet_MathCMD(SCPICommandProcessFuncParam analyResult)
        {
            if (!(analyResult.Tag is ScpiTagObj tagObj) || string.IsNullOrWhiteSpace($"{tagObj.Tag}"))
            {
                return false;
            }
            string tag = $"{tagObj.Tag}".Trim();

            List<ChannelId> mathsList = ChannelIdExt.GetMaths().ToList();
            IChnlPrsnt chnlPrsnt;
            ChannelId mathId;
            bool getChannel;

            switch (tag)
            {
                case "ADD":
                    foreach (var math in mathsList)
                    {
                        Presenter.TryGetChannel(math, out chnlPrsnt);
                        if (chnlPrsnt != null)
                        {
                            if (chnlPrsnt.Active)
                            {
                                continue;
                            }
                            chnlPrsnt.Active = true;
                            return true;
                        }
                    }
                    return true;
                case "DEL":
                    if (analyResult.FirstChannelIndex <= 0)
                    {
                        return false;
                    }
                    mathId = ChannelId.M1 + analyResult.FirstChannelIndex - 1;
                    getChannel = Presenter.TryGetChannel(mathId, out chnlPrsnt);
                    if (!getChannel || chnlPrsnt is not MathPrsnt)
                    {
                        break;
                    }
                    chnlPrsnt.Active = false;
                    return true;
                //case "RES":
                //    mathId = ChannelId.M1 + analyResult.FirstChannelIndex - 1;
                //    getChannel = Presenter.TryGetChannel(mathId, out chnlPrsnt);
                //    if (!getChannel || chnlPrsnt is not MathPrsnt)
                //    {
                //        break;
                //    }
                //    chnlPrsnt = new MathPrsnt(mathId);
                //    chnlPrsnt.Active = true;
                //    return true;
                default:
                    break;
            }

            return false;
        }

        public static bool scpiQuy_MathCMD(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            string quyResult = "";
            if (analyResult.Tag is not ScpiTagObj tagObj || string.IsNullOrWhiteSpace($"{tagObj.Tag}"))
            {
                return false;
            }
            List<ChannelId> mathsList = ChannelIdExt.GetMaths().ToList();
            if ($"{tagObj.Tag}" != "LIST")
            {
                return false;
            }
            foreach (var math in mathsList)
            {
                bool getPrsnt = Presenter.TryGetChannel(math, out IChnlPrsnt chnlPrsnt);
                if (getPrsnt && chnlPrsnt.Active == true)
                {
                    quyResult += $"{math},";
                }
            }
            quyResult = quyResult.TrimEnd(',');
            sendMessage.SendData = decodeStr(quyResult);
            return true;
        }
        #region 私有方法
        private static bool getMathPropertyName(bool isSource1, MathType mathType, MathArgPrsnt mathArg, out string propertyName)
        {
            propertyName = null;

            if (!(isSource1 || mathType == MathType.Binary))
            {
                return false;
            }
            switch (mathType)
            {
                case MathType.Binary:
                    propertyName = isSource1 ? "Source1st" : "Source2nd";
                    break;
                case MathType.Custom:
                    return false;
                case MathType.FFT:
                case MathType.Zoom:
                case MathType.Filter:
                case MathType.ERes:
                case MathType.Histgram:
                case MathType.Track:
                case MathType.UserProgram:
                    propertyName = "Source";
                    break;
            }

            return true;
        }
        #endregion 私有方法

    }
}

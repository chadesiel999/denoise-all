using ScopeX.SCPIManager;
using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using ScopeX.Core;
using System.Threading;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        public static bool scpiQuy_JitterCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            var prsnt = Presenter.Jitter;
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

        public static bool scpiQuy_JitterDataCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            var prsnt = Presenter.Jitter;
            if (prsnt.Active == false)
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            StringBuilder resultBuilder = new StringBuilder();
            double data = 0D;
            switch (scpiTagObj.Tag)
            {
                case "Jitter":
                    data = prsnt.TIEPeak.Value == null ? double.MaxValue : (Double)prsnt.TIEPeak.Value;
                    resultBuilder.Append(data.ToString("E5", CultureInfo.InvariantCulture) + ",");

                    data = prsnt.HistTjBER.Value == null ? double.MaxValue : (Double)prsnt.HistTjBER.Value;
                    resultBuilder.Append(data.ToString("E5", CultureInfo.InvariantCulture) + ",");

                    data = prsnt.HistRj.Value == null ? double.MaxValue : (Double)prsnt.HistRj.Value;
                    resultBuilder.Append(data.ToString("E5", CultureInfo.InvariantCulture) + ",");

                    data = prsnt.HistDj.Value == null ? double.MaxValue : (Double)prsnt.HistDj.Value;
                    resultBuilder.Append(data.ToString("E5", CultureInfo.InvariantCulture) + ",");

                    data = prsnt.PjPeak.Value == null ? double.MaxValue : (Double)prsnt.PjPeak.Value;
                    resultBuilder.Append(data.ToString("E5", CultureInfo.InvariantCulture) + ",");

                    data = prsnt.SpecDDj.Value == null ? double.MaxValue : (Double)prsnt.SpecDDj.Value;
                    resultBuilder.Append(data.ToString("E5", CultureInfo.InvariantCulture) + ",");

                    data = prsnt.SpecDCD.Value == null ? double.MaxValue : (Double)prsnt.SpecDCD.Value;
                    resultBuilder.Append(data.ToString("E5", CultureInfo.InvariantCulture) + ",");

                    data = prsnt.SpecISI.Value == null ? double.MaxValue : (Double)prsnt.SpecISI.Value;
                    resultBuilder.Append(data.ToString("E5", CultureInfo.InvariantCulture));

                    sendMessage.IsDataBlock = true;
                    sendMessage.SendData = Encoding.UTF8.GetBytes(resultBuilder.ToString());
                    return true;
                case "Eye":
                    string pattern = @"(\d+(\.\d+)?)"; // 匹配整数或小数
                    foreach (var item in prsnt.EyeParamTable)
                    {
                        Match match = Regex.Match(item.Value, pattern);
                        if (match.Success)
                        {
                            data = double.Parse(match.Groups[1].Value);
                        }
                        else
                        {
                            data = double.MaxValue;
                        }
                        resultBuilder.Append(data.ToString("E5", CultureInfo.InvariantCulture) + ",");
                    }
                    sendMessage.IsDataBlock = true;
                    sendMessage.SendData = Encoding.UTF8.GetBytes(resultBuilder.ToString());
                    return true;
                default:
                    break;
            }
            return returnResult;
        }

        public static bool scpiQuy_JitterEyeMask(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            var prsnt = Presenter.Jitter;
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

        public static bool scpiSet_JitterCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            var prsnt = Presenter.Jitter;
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
                        Presenter?.SetMutexFunctionFlag();
                    }
                    if (scpiTagObj.PropertyName == nameof(JitterPrsnt.CurrentBinNum))//特殊处理
                    {
                        TrySetHistBinNumValue(prsnt, propertyInfo, param[0], scpiTagObj.ParamList);
                    }
                    else
                    {
                        if (TrySetPropertyValue(prsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                            returnResult = true;
                    }
                }
            }

            return returnResult;
        }

        private static bool TrySetHistBinNumValue(object nodeClassPrsntObject, PropertyInfo propertyInfo, string valueString, List<string> paramList = null)
        {
            int foundIndex = paramList.FindIndex(s => s == valueString);
            if (foundIndex < 0)
                return false;
            var enums = Enum.GetValues<MaxBinNum>().ToList();
            if (foundIndex > enums.Count)
            {
                return false;
            }
            var setvalue = enums[foundIndex];
            propertyInfo.SetValue(nodeClassPrsntObject, setvalue);
            return true;
        }
        public static bool scpiSet_JitterGraphEnabe(SCPICommandProcessFuncParam analyResult)
        {
            var prsnt = Presenter.Jitter;
            if (!prsnt.Active)
            {
                return false;
            }
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }

            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var userparams = ParamListToStrList(analyResult.Params);
            var paramList = scpiTagObj.ParamList;
            if (userparams.Count < 2)
            {
                return false;
            }

            if (userparams[0] == "ALL")
            {
                return JitterALLGraphEnabe(prsnt, userparams[1]);
            }

            var foundIndex = paramList.FindIndex(s => s.ToUpper() == userparams[0].ToUpper() || shortCMD(s) == userparams[0]);
            if (foundIndex == -1)
                return false;

            var enumValues = Enum.GetValues<JitterGraphType>().ToList();
            if (foundIndex > enumValues.Count)
            {
                return false;
            }


            Int32? enumindex = null;
            for (int index = 0; index < enumValues.Count; index++)
            {
                if (enumValues[index]!.ToString()!.ToUpper() == paramList[foundIndex].ToUpper())
                {
                    enumindex = index;
                    break;
                }
            }

            if (enumindex == null)
                return false;

            prsnt.CurGraphType = enumValues[(Int32)enumindex];
            var enable = false;
            enable = userparams[1].ToUpper() == "ON" || userparams[1] == "1";

            if (!GetMathItems(out var mathId))
            {
                return false;
            }
            if (enable)
            {
                prsnt.GetCurGraphPrsnt(prsnt.CurGraphType).DestMathChannel = (ChannelId)mathId;
                if (Presenter.TryGetChannel(prsnt.GetCurGraphPrsnt(prsnt.CurGraphType).DestMathChannel, out var mathprsnt))
                {
                    if (mathprsnt is MathPrsnt)
                    {
                        (mathprsnt as MathPrsnt)!.GetOrMakeArg(MathType.Custom);
                    }
                }
            }
            else
            {
                prsnt.CurGraphType = (JitterGraphType)enumindex;
            }
            prsnt.GetCurGraphPrsnt(prsnt.CurGraphType).Enabled = enable;
            return true;
        }

        public static bool scpiSet_JitterEyeMask(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            var prsnt = Presenter.Jitter;
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
                    if (scpiTagObj.PropertyName == nameof(JitterPrsnt.CurrentBinNum))//特殊处理
                    {
                        TrySetHistBinNumValue(prsnt, propertyInfo, param[0], scpiTagObj.ParamList);
                    }
                    else
                    {
                        if (TrySetPropertyValue(prsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                            returnResult = true;
                    }
                }
            }

            return returnResult;
        }

        private static Boolean GetMathItems(out ChannelId? channelId)
        {
            channelId = null;
            foreach (var channel in ChannelIdExt.GetJitterMaths())
            {
                if (Presenter.TryGetChannel(channel, out var mathprsnt))
                {
                    if (mathprsnt is MathPrsnt && (mathprsnt as MathPrsnt).Args.Occupier == null)
                    {
                        (mathprsnt as MathPrsnt)!.GetOrMakeArg(MathType.Custom);
                        channelId = channel;
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool JitterALLGraphEnabe(JitterPrsnt prsnt, String active)
        {
            var state = active.ToUpper() == "ON" || active == "1";
            var types = Enum.GetValues(typeof(JitterGraphType));

            foreach (JitterGraphType type in types)
            {
                if (type == JitterGraphType.QFactor)
                    continue;

                prsnt.CurGraphType = type;
                if (prsnt.GetCurGraphPrsnt(type).Enabled != state)
                {
                    if (state)
                    {
                        if (GetMathItems(out var id))
                        {
                            Active(prsnt, (ChannelId)id, state);
                        }
                    }
                    else
                    {
                        prsnt.GetCurGraphPrsnt(prsnt.CurGraphType).Enabled = false;
                        if (type == JitterGraphType.Eye)
                        {
                            prsnt.EyeParamEnable = false;
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            return true;
        }

        private static void Active(JitterPrsnt prsnt, ChannelId id, Boolean state)
        {
            prsnt.GetCurGraphPrsnt(prsnt.CurGraphType).DestMathChannel = id;
            if (Presenter.TryGetChannel(prsnt.GetCurGraphPrsnt(prsnt.CurGraphType).DestMathChannel, out var mathprsnt))
            {
                if (mathprsnt is MathPrsnt)
                {
                    (mathprsnt as MathPrsnt)!.GetOrMakeArg(MathType.Custom);
                }
            }
            prsnt.GetCurGraphPrsnt(prsnt.CurGraphType).Enabled = state;

        }
    }
}
//================= 共4个方法 =

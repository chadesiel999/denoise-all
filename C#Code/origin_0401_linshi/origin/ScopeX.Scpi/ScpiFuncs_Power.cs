using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.PowerAnalysis;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        //================= 电源分析 =============================================================================================
        /// <summary>
        /// 设置或查询电源分析
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_PowerAnalysisCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            var returnResult = false;
            var prsntmap = Presenter.PwrAnalysisDictionary;
            if (prsntmap.Count == 0)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                returnResult = false;
            }
            else
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                var id = $"POWER{analyResult.ChannelIndex}";
                var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id);
                if (prsnt.Value != null)
                {
                    if (TryGetPropertyInfo(prsnt.Value, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(prsnt.Value, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;
                            sendMessage.UsingScientificNotation = usingScientific;
                            sendMessage.SendData = decodeStr(outputString);
                            returnResult = true;
                        }
                    }
                }
                else
                {
                    sendMessage.UsingScientificNotation = false;
                    sendMessage.SendData = decodeStr("0");
                    returnResult = false;
                }
            }
            return returnResult;
        }

        /// <summary>
        /// 设置或查询电源分析的结果表
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_PowerAnalysisTableDataCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            var returnResult = false;
            var prsntmap = Presenter.PwrAnalysisDictionary;

            if (prsntmap.Count == 0)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                returnResult = false;
            }
            else
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                var paramlist = scpiTagObj.ParamList;
                var id = $"POWER{analyResult.ChannelIndex}";
                var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id);

                if (prsnt.Value != null)
                {
                    var outputstring = paramlist[(Int32)prsnt.Value.Mode] + "," + Environment.NewLine;

                    switch (prsnt.Value.Mode)
                    {
                        case PowerAnalysisOpt.PowerQuality:
                            outputstring += prsnt.Value.QualityPrsnt.Value.Titles + "," + Environment.NewLine;
                            outputstring += prsnt.Value.QualityPrsnt.Value.QualityTableData();
                            break;
                        case PowerAnalysisOpt.Harmonic:
                            outputstring += prsnt.Value.HarmonicPrsnt.Value.GetHarmonicTableData();
                            break;
                        case PowerAnalysisOpt.Ripple:
                            outputstring += prsnt.Value.RipplePrsnt.Value.GetRippleTableData();
                            break;
                        case PowerAnalysisOpt.LoopAnalysis:
                            outputstring += prsnt.Value.LoopAnalysisPrsnt.Value.Titles + "," + Environment.NewLine;
                            outputstring += prsnt.Value.LoopAnalysisPrsnt.Value.GetLoopTableData();
                            break;
                        case PowerAnalysisOpt.SafeOperationArea:
                            return false;
                        case PowerAnalysisOpt.SwitchingLoss:
                            outputstring += prsnt.Value.SwitchingLossPrsnt.Value.Titles + "," + Environment.NewLine;
                            outputstring += prsnt.Value.SwitchingLossPrsnt.Value.GetSwLossTableData();
                            break;
                        case PowerAnalysisOpt.Modulation:
                            outputstring += prsnt.Value.ModulationPrsnt.Value.ModulationTableData();
                            break;
                        case PowerAnalysisOpt.InrushCurrent:
                            outputstring += prsnt.Value.InrushCurrentPrsnt.Value.InrushTableData();
                            break;
                        case PowerAnalysisOpt.PowerEfficency:
                            outputstring += prsnt.Value.EfficiencyPrsnt.Value.EfficiencyTableData();
                            break;
                        case PowerAnalysisOpt.RDSon:
                            outputstring += prsnt.Value.RDSonPrsnt.Value.RdsonTableData();
                            break;
                        case PowerAnalysisOpt.TurnOnOff:
                            outputstring += prsnt.Value.OnOffTimePrsnt.Value.OnOffTimeTableData();
                            break;
                        case PowerAnalysisOpt.PSRR:
                            outputstring += prsnt.Value.PSRRPrsnt.Value.GetPSRRTableData();
                            break;
                        case PowerAnalysisOpt.SlewRate:
                            outputstring += prsnt.Value.SlewRatePrsnt.Value.SlewRateTableData();
                            break;
                        default:
                            break;
                    }
                    sendMessage.SendData = decodeStr(outputstring);
                    sendMessage.IsDataBlock = true;
                    return true;
                }
                else
                {
                    sendMessage.UsingScientificNotation = false;
                    sendMessage.SendData = decodeStr("0");
                    returnResult = false;
                }
            }
            return returnResult;
        }

        public static bool scpiQuy_PowerQualityAnalysis(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.PowerQuality);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.QualityPrsnt.Value;

            if (prsnt == null)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                return false;
            }

            if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                if (TryGetPropertyValue(prsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;
                    sendMessage.UsingScientificNotation = usingScientific;
                    sendMessage.SendData = decodeStr(outputString);
                    returnResult = true;
                }
            }
            return returnResult;
        }

        public static bool scpiSet_PowerQualityAnalysis(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.PowerQuality);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.QualityPrsnt.Value;
            if (prsnt == null)
            {
                return false;
            }
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

        public static bool scpiQuy_PowerHarmonic(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.Harmonic);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.HarmonicPrsnt.Value;
            if (prsnt == null)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                return false;
            }
            if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                if (TryGetPropertyValue(prsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;
                    sendMessage.UsingScientificNotation = usingScientific;
                    sendMessage.SendData = decodeStr(outputString);
                    returnResult = true;
                }
            }
            return returnResult;
        }
        public static bool scpiSet_PowerHarmonic(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.Harmonic);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.HarmonicPrsnt.Value;
            if (prsnt == null)
            {
                return false;
            }
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
        public static bool scpiQuy_PowerRipple(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.Ripple);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.RipplePrsnt.Value;
            if (prsnt == null)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                return false;
            }
            if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                if (TryGetPropertyValue(prsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;
                    sendMessage.UsingScientificNotation = usingScientific;
                    sendMessage.SendData = decodeStr(outputString);
                    returnResult = true;
                }
            }
            return returnResult;
        }
        public static bool scpiSet_PowerRipple(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.Ripple);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.RipplePrsnt.Value;
            if (prsnt == null)
            {
                return false;
            }
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
        public static bool scpiQuy_PowerSafeOperationArea(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.SafeOperationArea);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.SOAPrsnt.Value;
            if (prsnt == null)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                return false;
            }
            if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                if (TryGetPropertyValue(prsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;
                    sendMessage.UsingScientificNotation = usingScientific;
                    sendMessage.SendData = decodeStr(outputString);
                    returnResult = true;
                }
            }
            return returnResult;
        }
        public static bool scpiSet_PowerSafeOperationArea(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.SafeOperationArea);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.SOAPrsnt.Value;
            if (prsnt == null)
            {
                return false;
            }
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
        public static bool scpiQuy_PowerLoopAnalysis(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.LoopAnalysis);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.LoopAnalysisPrsnt.Value;
            if (prsnt == null)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                return false;
            }
            if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                if (TryGetPropertyValue(prsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;
                    sendMessage.UsingScientificNotation = usingScientific;
                    sendMessage.SendData = decodeStr(outputString);
                    returnResult = true;
                }
            }
            return returnResult;
        }
        public static bool scpiSet_PowerLoopAnalysis(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.LoopAnalysis);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.LoopAnalysisPrsnt.Value;
            if (prsnt == null)
            {
                return false;
            }
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
        public static bool scpiQuy_ModulationAnalysis(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.Modulation);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.ModulationPrsnt.Value;
            if (prsnt == null)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                return false;
            }
            if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                if (TryGetPropertyValue(prsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;
                    sendMessage.UsingScientificNotation = usingScientific;
                    sendMessage.SendData = decodeStr(outputString);
                    returnResult = true;
                }
            }
            return returnResult;
        }

        public static bool scpiSet_ModulationAnalysis(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.Modulation);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.ModulationPrsnt.Value;
            if (prsnt == null)
            {
                return false;
            }
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

        public static bool scpiSet_AddUiModulationAnalysis(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.Modulation);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.ModulationPrsnt.Value;
            if (prsnt == null)
            {
                return false;
            }
            switch (scpiTagObj.Tag)
            {
                case "Hist":
                    prsnt.VuAddHistogram?.Invoke();
                    returnResult = true;
                    break;
                case "Trend":
                    prsnt.VuAddTrend?.Invoke();
                    returnResult= true;
                    break;
            }
            return returnResult;
        }

        public static bool scpiQuy_PwrAnalysis(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            Object? prsnt = null;
            List<KeyValuePair<ChannelId, PowerAnalysisPrsnt>>? prsntmap = null;
            var type= scpiTagObj.PrsntObj.GetType();
            
            switch (scpiTagObj.PrsntObj.ToString())
            {
                case "ScopeX.Core.PowerAnalysis.PwrInrushCurrentPrsnt":
                    prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.InrushCurrent);
                    prsnt = prsntmap?.FirstOrDefault(p => $"{p.Key}" == id).Value?.InrushCurrentPrsnt.Value;
                    break;
                case "ScopeX.Core.PowerAnalysis.PwrEfficiencyPrsnt":
                    prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.PowerEfficency);
                    prsnt = prsntmap?.FirstOrDefault(p => $"{p.Key}" == id).Value?.EfficiencyPrsnt.Value;
                    break;
                case "ScopeX.Core.PowerAnalysis.PwrOnOffTimePrsnt":
                    prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.TurnOnOff);
                    prsnt = prsntmap?.FirstOrDefault(p => $"{p.Key}" == id).Value?.OnOffTimePrsnt.Value;
                    break;
                case "ScopeX.Core.PowerAnalysis.PwrPSRRPrsnt":
                    prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.PSRR);
                    prsnt = prsntmap?.FirstOrDefault(p => $"{p.Key}" == id).Value?.PSRRPrsnt.Value;
                    break;
            }

            if (prsntmap == null || prsntmap.Count == 0)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                return false;
            }
            
            if (prsnt == null)
            {
                sendMessage.UsingScientificNotation = false;
                sendMessage.SendData = decodeStr("0");
                return false;
            }
            if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                if (TryGetPropertyValue(prsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;
                    sendMessage.UsingScientificNotation = usingScientific;
                    sendMessage.SendData = decodeStr(outputString);
                    returnResult = true;
                }
            }
            return returnResult;
        }

        public static bool scpiSet_PwrAnalysis(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            Object? prsnt = null;
            var type = scpiTagObj.PrsntObj.GetType();
            List<KeyValuePair<ChannelId, PowerAnalysisPrsnt>>? prsntmap = null;
            switch (scpiTagObj.PrsntObj.ToString())
            {
                case "ScopeX.Core.PowerAnalysis.PwrInrushCurrentPrsnt":
                    prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.InrushCurrent);
                    prsnt = prsntmap?.FirstOrDefault(p => $"{p.Key}" == id).Value?.InrushCurrentPrsnt.Value;
                    break;
                case "ScopeX.Core.PowerAnalysis.PwrEfficiencyPrsnt":
                    prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.PowerEfficency);
                    prsnt = prsntmap?.FirstOrDefault(p => $"{p.Key}" == id).Value?.EfficiencyPrsnt.Value;
                    break;
                case "ScopeX.Core.PowerAnalysis.PwrOnOffTimePrsnt":
                    prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.TurnOnOff);
                    prsnt = prsntmap?.FirstOrDefault(p => $"{p.Key}" == id).Value?.OnOffTimePrsnt.Value;
                    break;
                case "ScopeX.Core.PowerAnalysis.PwrPSRRPrsnt":
                    prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.PSRR);
                    prsnt = prsntmap?.FirstOrDefault(p => $"{p.Key}" == id).Value?.PSRRPrsnt.Value;
                    break;
            }

            if (prsntmap == null || prsntmap.Count == 0)
            {
                return false;
            }
            
            if (prsnt == null)
            {
                return false;
            }
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

        public static bool scpiSet_InrushRunAnalysis(SCPICommandProcessFuncParam analyResult)
        {
            
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.InrushCurrent);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.InrushCurrentPrsnt.Value;
            if (prsnt == null)
            {
                return false;
            }
            prsnt.VuSingleRun?.Invoke();
            return true;
        }

        public static bool scpiSet_EfficiencyDiagramAnalysis(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.PowerEfficency);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.EfficiencyPrsnt.Value;
            if (prsnt == null)
            {
                return false;
            }
            switch (scpiTagObj.Tag)
            {
                case "Input":
                    prsnt.VuTryAddPwr1?.Invoke();
                    returnResult = true;
                    break;
                case "Output":
                    prsnt.VuTryAddPwr2?.Invoke();
                    returnResult = true;
                    break;
            }
            return returnResult;
        }

        public static bool scpiSet_RDSWaveAnalysis(SCPICommandProcessFuncParam analyResult)
        {
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.RDSon);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                return false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == id).Value?.RDSonPrsnt.Value;
            if (prsnt == null)
            {
                return false;
            }
            prsnt.VuTryAddRDSWave?.Invoke();
            return true;
        }

        public static bool scpiSet_PsrrTryAddUIAnalysis(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.PSRR);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                returnResult= false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap?.FirstOrDefault(p => $"{p.Key}" == id).Value?.PSRRPrsnt.Value;
            if (prsnt == null)
            {
                returnResult = false;
            }
            switch (scpiTagObj.Tag)
            {
                case "Run":
                    prsnt?.VuPsrrRun?.Invoke();
                    returnResult = true;
                    break;
                case "Bode":
                    prsnt?.VuTryAddPsrrBode?.Invoke();
                    returnResult = true;
                    break;
            }
            return returnResult;
        }
        public static bool scpiSet_SlewRateTryAddUIAnalysis(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            var prsntmap = GetPowerAnalysisPrsntByMode(PowerAnalysisOpt.SlewRate);

            if (prsntmap == null || prsntmap.Count == 0)
            {
                returnResult = false;
            }
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var id = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap?.FirstOrDefault(p => $"{p.Key}" == id).Value?.SlewRatePrsnt.Value;
            if (prsnt == null)
            {
                returnResult = false;
            }
            switch (scpiTagObj.Tag)
            {
                case "DVDT":
                    prsnt?.VuTryAddDvdtUI?.Invoke();
                    returnResult = true;
                    break;
                case "DIDT":
                    prsnt?.VuTryAddDidtUI?.Invoke();
                    returnResult = true;
                    break;
            }
            return returnResult;
        }
        /// <summary>
        /// 设置或查询通过失败测试数据源
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_PowerAnalysisCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }

            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var prsntmap = Presenter.PwrAnalysisDictionary;

            var cid = $"POWER{analyResult.ChannelIndex}";
            var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == cid);
            if (prsnt.Value == null)
            {
                var pp = new PowerAnalysisPrsnt(Presenter, null, (ChannelId)Enum.Parse(typeof(ChannelId), cid));
                Presenter.PwrAnalysisDictionary.Add(pp.Id, pp);
                pp.BoundMeasPrsnt = Presenter.Measure;
                foreach (var item in ChannelIdExt.GetPowerAnalysisMaths())
                {
                    if (Presenter.TryGetChannel(item, out var mpp))
                    {
                        if (mpp.Active == false &&
                            Presenter.PwrAnalysisDictionary.Where((a) => { return a.Value.BoundMathPrsnt1?.Id == item; }).Count() < 1)
                        {
                            pp.BoundMathPrsnt1 = (MathPrsnt)mpp;
                            break;
                        }
                    }
                }
                if (pp.BoundMathPrsnt1 == null)
                {
                    Presenter.PwrAnalysisDictionary.Remove(pp.Id);
                    pp.Dispose();
                    pp = null;
                    return false;
                }
            }
            prsntmap = Presenter.PwrAnalysisDictionary;
            prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == cid);
            if (TryGetPropertyInfo(prsnt.Value, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                List<string> param = ParamListToStrList(analyResult.Params);
                if (param.Count > 0)
                {
                    if (TrySetPropertyValue(prsnt.Value, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        returnResult = true;
                }
            }
            return returnResult;
        }

        public static bool scipQuy_AllPowerAnalysis(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            var prsntmap = Presenter.PwrAnalysisDictionary;
            if (prsntmap == null)
                return false;
            var activeprsnt = prsntmap.Where(pa => pa.Value.Active).ToList();
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var paramlist = scpiTagObj.ParamList;
            string outputString = string.Empty;
            if (activeprsnt.Count > 0)
            {
                foreach (var prsnt in activeprsnt)
                {
                    if ((Int32)prsnt.Value.Mode >= paramlist.Count)
                    {
                        return false;
                    }
                    outputString += $"{prsnt.Value.Id},{paramlist[(Int32)prsnt.Value.Mode]},";
                }
                sendMessage.SendData = decodeStr(outputString);
                return true;
            }
            return false;
        }

        internal static bool scpiSet_DeletePowerAnalysis(SCPICommandProcessFuncParam analyResult)
        {
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            var prsntmap = Presenter.PwrAnalysisDictionary;
            var userparams = ParamListToStrList(analyResult.Params);
            if (userparams != null && userparams.Count > 0)
            {
                var cid = $"POWER{userparams[0]}";
                var prsnt = prsntmap.FirstOrDefault(p => $"{p.Key}" == cid);
                if (prsnt.Value != null)
                {
                    prsnt.Value.Active = false;
                    return true;
                }
            }
            return false;
        }

        internal static bool scpiSet_AddPowerAnalysis(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }

            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var userparams = ParamListToStrList(analyResult.Params);
            var paramlist = scpiTagObj.ParamList;
            if (userparams == null || userparams.Count < 3)
                return false;

            var foundIndex = paramlist.FindIndex(s => s.ToUpper() == userparams[0].ToUpper() || shortCMD(s) == userparams[0]);
            if (foundIndex == -1)
                return false;

            if (!Enum.TryParse(typeof(ChannelId), userparams[1], out var vsource))
                return false;
            var voltagesrc = (ChannelId)vsource;

            if (!Enum.TryParse(typeof(ChannelId), userparams[2], out var csource))
                return false;
            var currentsrc = (ChannelId)csource;

            var typeenums = Enum.GetValues<PowerAnalysisOpt>().ToList();
            if (typeenums == null || foundIndex > typeenums.Count)
                return false;

            var powertype = typeenums[foundIndex];
            Presenter?.SetMutexFunctionFlag();
            return PowerAnalysisPrsnt.TryAddPowerAnalysis(powertype, out var prsnt, voltagesrc, currentsrc);

        }

        private static List<KeyValuePair<ChannelId, PowerAnalysisPrsnt>>? GetPowerAnalysisPrsntByMode(PowerAnalysisOpt power)
        {
            var prsntmap = Presenter.PwrAnalysisDictionary;
            if (prsntmap.Count == 0)
            {
                return null;
            }
            switch (power)
            {
                default:
                case PowerAnalysisOpt.PowerQuality:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.PowerQuality).ToList();
                case PowerAnalysisOpt.Harmonic:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.Harmonic).ToList();
                case PowerAnalysisOpt.Ripple:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.Ripple).ToList();
                case PowerAnalysisOpt.SwitchingLoss:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.SwitchingLoss).ToList();
                case PowerAnalysisOpt.SafeOperationArea:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.SafeOperationArea).ToList();
                case PowerAnalysisOpt.LoopAnalysis:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.LoopAnalysis).ToList();
                case PowerAnalysisOpt.Modulation:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.Modulation).ToList();
                case PowerAnalysisOpt.InrushCurrent:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.InrushCurrent).ToList();
                case PowerAnalysisOpt.PowerEfficency:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.PowerEfficency).ToList();
                case PowerAnalysisOpt.Differ:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.Differ).ToList();
                case PowerAnalysisOpt.Transient:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.Transient).ToList();
                case PowerAnalysisOpt.RDSon:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.RDSon).ToList(); ;
                case PowerAnalysisOpt.TurnOnOff:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.TurnOnOff).ToList();
                case PowerAnalysisOpt.PSRR:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.PSRR).ToList();
                case PowerAnalysisOpt.SlewRate:
                    return prsntmap.Where(p => p.Value.Mode == PowerAnalysisOpt.SlewRate).ToList();
            }
        }

    }
}
//================= 共4个方法 =

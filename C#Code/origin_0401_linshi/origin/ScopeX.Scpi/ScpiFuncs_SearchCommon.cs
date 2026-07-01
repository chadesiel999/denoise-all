using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        #region 搜索

        //public static bool scpiQuy_SearchCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        //{
        //    bool returnResult = false;
        //    if (TryGetSearchPrsnt(analyResult, out var prsnt))
        //    {
        //        if (prsnt == null)
        //            return false;
        //        ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
        //        if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
        //        {
        //            if (TryGetPropertyValue(prsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
        //            {
        //                sendMessage.SendData = decodeStr(outputString);
        //                sendMessage.UsingScientificNotation = usingScientific;
        //                returnResult = true;
        //            }
        //        }
        //    }
        //    return returnResult;
        //}

        public static bool scpiQuy_SearchCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetSearchPrsnt(analyResult, out var prsnt))
            {
                if (prsnt == null)
                    return false;
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;

                if (scpiTagObj.Tag == null)
                {
                    if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        if (TryGetPropertyValue(prsnt, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        {
                            sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                            sendMessage.UsingScientificNotation = usingScientific;
                            sendMessage.SendData = decodeStr(outputString);
                            return returnResult = true;
                        }
                    }
                }

                if (scpiTagObj.Tag.ToString() == "Edge")
                {
                    if (prsnt.SearchTypePrsnt is SearchEdgePrsnt ep)
                    {
                        if (TryGetPropertyInfo(ep, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                        {
                            if (TryGetPropertyValue(ep, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                            {
                                sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                                sendMessage.UsingScientificNotation = usingScientific;
                                sendMessage.SendData = decodeStr(outputString);
                                returnResult = true;
                            }
                        }
                    }
                }
                else if (scpiTagObj.Tag.ToString() == "Pulse")
                {
                    if (prsnt.SearchTypePrsnt is SearchPulsePrsnt pp)
                    {
                        if (TryGetPropertyInfo(pp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                        {
                            if (TryGetPropertyValue(pp, propertyInfo, out bool usingScientific, out string outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                            {
                                sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                                sendMessage.UsingScientificNotation = usingScientific;
                                sendMessage.SendData = decodeStr(outputString);
                                returnResult = true;
                            }
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool scpiQuy_SearchData(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (TryGetSearchPrsnt(analyResult, out var prsnt))
            {
                if (prsnt == null)
                    return false;
                var eventdata = prsnt.GetSearchResultsByTmb();
                if (eventdata == null)
                    return false;
                var resultBuilder = new StringBuilder();
                resultBuilder.Append($"index,type,position,delta,description,{Environment.NewLine}");
                eventdata.ForEach(res =>
                {
                    resultBuilder.Append($"{res.Index},{res.Type},{res.PositionBys},{res.DeltaBys},{res.Description},{Environment.NewLine}");
                });

                sendMessage.SendData = decodeStr(resultBuilder.ToString());
                sendMessage.IsDataBlock = true;
                return true;
            }
            return false;
        }

        public static bool scpiSet_SearchCommon(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetSearchPrsnt(analyResult, out var prsnt))
            {
                if (prsnt == null)
                    return false;
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;

                if (scpiTagObj.Tag == null)
                {
                    if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                    {
                        List<string> param = ParamListToStrList(analyResult.Params);
                        if (param.Count > 0)
                        {
                            if (TrySetPropertyValue(prsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                return returnResult = true;
                            else
                                return false;
                        }
                    }
                }

                if (scpiTagObj.Tag.ToString() == "Edge")
                {
                    if (prsnt.SearchTypePrsnt is SearchEdgePrsnt ep)
                    {
                        if (TryGetPropertyInfo(ep, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                        {
                            List<string> param = ParamListToStrList(analyResult.Params);
                            if (param.Count > 0)
                            {
                                if (TrySetPropertyValue(ep, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                    return returnResult = true;
                            }
                        }
                    }
                }
                else if (scpiTagObj.Tag.ToString() == "Pulse")
                {
                    if (prsnt.SearchTypePrsnt is SearchPulsePrsnt pp)
                    {
                        if (TryGetPropertyInfo(pp, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                        {
                            List<string> param = ParamListToStrList(analyResult.Params);
                            if (param.Count > 0)
                            {
                                if (TrySetPropertyValue(pp, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                    return returnResult = true;
                            }
                        }
                    }
                }
            }
            return returnResult;
        }

        public static bool TryGetSearchPrsnt(SCPICommandProcessFuncParam analyResult, out SearchItemPrsnt? searchItemPrsnt)
        {
            searchItemPrsnt = null;
            var id = analyResult.ChannelIndex;
            if (id < 0 || id > Constants.MAX_SEARCH_CNT)
            {
                return false;
            }
            return Presenter.Search.GetorMakeSearchItemPrsnt(id, out searchItemPrsnt);
        }

        #endregion 搜索
    }
}

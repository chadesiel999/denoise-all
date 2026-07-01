using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.MathExt;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ScopeX.Scpi
{
    partial class StubFunc
    {
        //================= 数学 =================================================================================================
        /// <summary>
        /// 设置或查询数学通道自定义运算
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_MathDefine(SCPICommandProcessFuncParam analyResult)
        {
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetMathChannelPrsnt(analyResult, out MathPrsnt prsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                MathType mathType = prsnt.Args.Type;
                if (mathType != MathType.Custom)
                {
                    return false;
                }
                var matharg = (MathCustomArg)prsnt.GetOrMakeArg(MathType.Custom);
                if (TryGetPropertyInfo(matharg, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                {
                    List<string> param = ParamListToStrList(analyResult.Params);
                    if (param.Count <= 0 || !ConvertCmd2Formula(param[0], out string formula))
                    {
                        return false;
                    }

                    var customArg = (MathCustomArg)prsnt.GetOrMakeArg(MathType.Custom);
                    customArg.Formula = formula;
                    //customArg.SetDescription(param[0].Trim());
                    customArg.Expression = param[0].Trim();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 设置或查询数学通道数学运算类型
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_MathType(SCPICommandProcessFuncParam analyResult)
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
                        var setvalue = ConvertObject(encodingBytes(analyResult.Params[0]), typeof(MathType), scpiTagObj.ParamList);
                        if (setvalue != null)
                        {
                            prsnt.GetOrMakeArg((MathType)setvalue);
                        }
                    }
                }
            }
            return returnResult;
        }

        ///// <summary>
        ///// 这个正则表达式使用了命名捕获组（(?<Open>...) 和 (?<-Open>...)）来处理嵌套的括号。
        ///// ([^()]*|(?<Open>\()[^()]*?(?<-Open>\)))* 是关键部分，它允许括号内嵌套其他括号。
        ///// 这种方法可以处理较复杂的括号嵌套，但如果嵌套层次非常深或结构更复杂，可能需要更复杂的解析策略。
        ///// </summary>
        //private static string functionPattern = @"[a-zA-Z0-9]+\(([^()]*|(?<Open>\()[^()]*?(?<-Open>\)))*\)";
        private static bool ConvertCmd2Formula(string param, out string formula)
        {
            //Regex regex = new Regex(functionPattern);
            //var funcs = regex.Matches(param).ToList();
            //if (funcs.Count > 0)
            //{
            //    param = AnalyzeFormula(param);
            //    //param = regex.Replace(param, m =>
            //    //{
            //    //    return "Execute." + System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(m.Value);
            //    //});
            //}
            ////else
            ////{
            ////    param = "Execute." + System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(param);
            ////}
            formula = AnalyzeFormula(param);
            var res = DynamicExecute.Evaluate("Custom", formula, out String errmsg);
            return res;
        }

        static List<(String InitValue, String ReplaceValue, String Expression)> SpecialFormula = new()
        {
            ("arcsin","arcs","Execute.Asin"),
            ("arccos","arcc","Execute.Acos"),
            ("arctan","arct","Execute.Atan"),
            ("sinh","sh","Execute.Sinh"  ),
            ("cosh","ch","Execute.Cosh"  ),
            ("neq","notsame","Execute.ElementWiseNotEqu"),
            ("exp10","express","Execute.Exp10"  ),
        };

        private static string AnalyzeFormula(string formula)
        {
            var newformula = string.Empty;
            List<KeyValuePair<string, MathFormulaInfo>> mathches = new();
            if (Presenter.MathFormulaCollections == null || Presenter.MathFormulaCollections.Count <= 0)
            {
                return string.Empty;
            }

            foreach (var item in SpecialFormula)
            {
                if (formula.Contains(item.InitValue))
                {
                    formula = formula.Replace(item.InitValue, item.ReplaceValue);
                }
            }

            foreach (var func in Presenter.MathFormulaCollections)
            {
                if (func.Value.Type != MathDefineFormulaType.Func)
                {
                    continue;
                }
                string Symbol = func.Value.Symbol;

                if (formula.Contains(Symbol))
                {
                    if (!mathches.Contains(func))
                    {
                        mathches.Add(func);
                    }
                }
            }
            foreach (var func in mathches)
            {
                formula = formula.Replace(func.Value.Symbol, func.Value.Expression);
            }
            foreach (var specitem in SpecialFormula)
            {
                if (formula.Contains(specitem.ReplaceValue))
                {
                    formula = formula.Replace(specitem.ReplaceValue, specitem.Expression);
                }
            }

            newformula = formula;
            return newformula;
        }

        /// <summary>
        /// 设置或查询数学通道数学运算类型
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_MathType(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetMathChannelPrsnt(analyResult, out MathPrsnt prsnt))
            {
                MathType mathType = prsnt.Args.Type;
                int index = mathType - MathType.Binary;
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (scpiTagObj.ParamList != null && scpiTagObj.ParamList.Count > index)
                {
                    sendMessage.SendData = decodeStr(scpiTagObj.ParamList[index]);
                    returnResult = true;
                }
            }
            return returnResult;

        }
        /// <summary>
        /// 设置或查询数学通道自定义运算
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_MathDefine(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetMathChannelPrsnt(analyResult, out MathPrsnt prsnt))
            {
                var customArg = (MathCustomArg)prsnt.GetOrMakeArg(MathType.Custom);
                sendMessage.SendData = decodeStr(customArg.Expression);
                returnResult = true;
            }
            return returnResult;
        }
    }
}
//================= 共4个方法 =

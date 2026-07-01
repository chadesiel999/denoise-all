using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.SCPIManager;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScopeX.Scpi
{
    partial class StubFunc
    {
        /// <summary>
        /// 设置或查询指示器
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_MeasureIndicator(SCPICommandProcessFuncParam analyResult)
        {
            if (Presenter == null || analyResult.Tag is not ScpiTagObj tagObj || !scpiSet_ParamCheck(analyResult, out string paraStr))
            {
                return false;
            }
            var measure = Presenter.Measure;

            if (shortCMD(paraStr) == "OFF")
            {
                Presenter.Measure.Indicator = 0;
            }
            else if ((!int.TryParse(paraStr.Replace("P", ""), out int index)) || !(index < 1 || index > measure.Length))
            {
                Presenter.Measure.Indicator = index;
            }
            else
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 设置或查询指示器
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_MeasureIndicator(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (Presenter == null || analyResult.Tag is not ScpiTagObj tagObj)
            {
                return false;
            }
            var indicator = Presenter.Measure.Indicator;
            string returnStr;
            if (indicator <= 0)
            {
                returnStr = "OFF";
            }
            else
            {
                returnStr = $"P{indicator}";
            }
            sendMessage.SendData = decodeStr(returnStr);
            return true;
        }

        /// <summary>
        /// 设置快速采集类型
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_MeasureUltraAcq(SCPICommandProcessFuncParam analyResult)
        {
            if (Presenter == null || analyResult.Tag is not ScpiTagObj tagObj
                || !scpiSet_ParamCheck(analyResult, out string paraStr))
            {
                return false;
            }
            paraStr = shortCMD(paraStr);
            var measure = Presenter.Measure;
            switch (paraStr)
            {
                case "VERT":
                    for (Int32 i = 0; i < measure.Length; i++)
                    {
                        measure[i].Name = _StdVertItems[i];
                        measure[i].Source = measure.SnapshotSource;
                        measure[i].Source2nd = measure.SnapshotSource;
                        measure[i].Active = true;
                        measure.ResetStat(i);
                    }
                    break;
                case "HOR":
                    for (Int32 i = 0; i < measure.Length; i++)
                    {
                        measure[i].Name = _StdHorzItems[i];
                        measure[i].Source = measure.SnapshotSource;
                        measure[i].Source2nd = measure.SnapshotSource;
                        measure[i].Active = true;
                        measure.ResetStat(i);
                    }
                    break;
                default:
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 设置所有测量项是否激活
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_MeasureAllActive(SCPICommandProcessFuncParam analyResult)
        {
            if (Presenter == null || analyResult.Tag is not ScpiTagObj tagObj
                || !scpiSet_ParamCheck(analyResult, out string paraStr))
            {
                return false;
            }
            paraStr = shortCMD(paraStr);
            var measure = Presenter.Measure;
            switch (paraStr)
            {
                case "ON":
                    measure.SetAllActive(true);
                    break;
                case "OFF":
                    measure.SetAllActive(false);
                    break;
                default:
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 参数测量统计复位
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_MeasureStatisticsReset(SCPICommandProcessFuncParam analyResult)
        {
            Presenter.Measure.ResetAllStats();
            return true;
        }

        /// <summary>
        /// 设置或查询测量项类型
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_MeasureItemType(SCPICommandProcessFuncParam analyResult)
        {
            if (Presenter == null || analyResult.Tag is not ScpiTagObj tagObj || !CheckMeasItemNumber(analyResult, out var itemprsnt) || !scpiSet_ParamCheck(analyResult, out string parastr))
            {
                return false;
            }
            if (itemprsnt == null)
            {
                return false;
            }
            MeasPrsnt measure = Presenter.Measure;
            if (!measure.ScpiNameTable!.TryGetValue(shortCMD(parastr), out var setvalue) || String.IsNullOrWhiteSpace(setvalue))
            {
                // 定义正则表达式
                var pattern = @"P([1-9]|10)[\+\-\*/]P([1-9]|10)";
                // 创建 Regex 对象
                Regex regex = new Regex(pattern);
                if (regex.IsMatch(parastr))
                {
                    var separator = String.Empty;
                    if (parastr.Contains("+"))
                    {
                        separator = "+";
                        itemprsnt.Operation = MeasureOperator.Add;
                    }
                    if (parastr.Contains("-"))
                    {
                        separator = "-";
                        itemprsnt.Operation = MeasureOperator.Subtract;
                    }
                    if (parastr.Contains("*"))
                    {
                        separator = "*";
                        itemprsnt.Operation = MeasureOperator.Multiply;
                    }
                    if (parastr.Contains("/"))
                    {
                        separator = "/";
                        itemprsnt.Operation = MeasureOperator.Division;
                    }
                    var sources = parastr.Split(separator);
                    if (Enum.TryParse<ChannelId>(sources[0], out var s1) && Enum.TryParse<ChannelId>(sources[1], out var s2))
                    {
                        if (itemprsnt.Id != s1 && itemprsnt.Id != s2)
                        {
                            var items1 = measure.SelectedItems.Where(i => i.Id == s1).ToList();
                            var items2 = measure.SelectedItems.Where(i => i.Id == s2).ToList();
                            if (items1 != null && items2 != null && items1.Count != 0 && items2.Count != 0 && items1[0].Active && items1[0].MeasureType != MeasureType.Composite && items2[0].Active && items2[0].MeasureType != MeasureType.Composite)
                            {
                                itemprsnt.Active = false;
                                itemprsnt.MeasureType = MeasureType.Composite;
                                itemprsnt.Source = s1;
                                itemprsnt.Source2nd = s2;
                                itemprsnt.Active = true;
                                measure.ResetStat(analyResult.FirstChannelIndex - 1);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            else
                itemprsnt.Name = setvalue;
            measure.ResetStat(analyResult.FirstChannelIndex - 1);
            return true;
        }
        /// <summary>
        /// 设置或查询测量项类型
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_MeasureItemType(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (Presenter == null || !CheckMeasItemNumber(analyResult, out var itemprsnt) || analyResult.Tag is not ScpiTagObj tagObj)
            {
                return false;
            }
            if (itemprsnt == null)
            {
                return false;
            }
            MeasPrsnt measure = Presenter.Measure;
            string itemname = itemprsnt.Name;
            var keyValue = measure.ScpiNameTable!.FirstOrDefault(data => data.Value == itemname);
            var returnvalue = string.IsNullOrWhiteSpace(keyValue.Key) ? itemname.Replace(" ", "").Replace("÷", "/").Replace("×", "*") : keyValue.Key;
            sendMessage.SendData = decodeStr(returnvalue);
            return true;
        }

    }
}
//================= 共6个方法 =

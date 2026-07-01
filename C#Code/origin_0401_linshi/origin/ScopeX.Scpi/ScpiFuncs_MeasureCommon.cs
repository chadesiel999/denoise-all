using ScopeX.ComModel;
using ScopeX.Controls.Common.Helper;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        #region 公有方法
        /// <summary>
        /// 查询测量值
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static Boolean scpiQuy_MeasureAllItemData(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            var returnreslut = false;
            var measprsnt = Presenter.Measure;
            var scpiTagObj = (ScpiTagObj)analyResult.Tag;
            List<string> param = ParamListToStrList(analyResult.Params);

            if (param != null)
            {
                var index = scpiTagObj.ParamList.FindIndex(x => x.Equals(param[0].ToUpper()));
                if (index == -1)
                {
                    return false;
                }
                else
                {
                    measprsnt.SnapshotSource = (ChannelId)(index);
                }
            }

            if (!measprsnt.CalcSnapshotAllResult())
            {
                return false;
            }
            var data = measprsnt.SnapShotDataTable;
            var datainfo = String.Join(",", data);
            sendMessage.SendData = decodeStr(datainfo);
            sendMessage.IsDataBlock = true;
            returnreslut = true;

            return returnreslut;
        }


        /// <summary>
        /// 查询测量值
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static Boolean scpiQuy_MeasureItemData(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            //判断
            if (analyResult.Tag is not ScpiTagObj tagObj || tagObj.Tag == null
                || !CheckMeasItemNumber(analyResult, out MeasItemPrsnt itemPrsnt))
            {
                return false;
            }
            Object? value = null;

            MeasPrsnt measPrsnt = Presenter.Measure;
            Boolean toCovert = false;

            switch (tagObj.Tag.ToString())
            {
                case "Value":
                    value = measPrsnt.GetResult(analyResult.FirstChannelIndex - 1) ?? Double.NaN;
                    toCovert = true;
                    break;
                case "Max":
                    value = measPrsnt.GetStatMax(analyResult.FirstChannelIndex - 1) ?? Double.NaN;
                    toCovert = true;
                    break;
                case "Min":
                    value = measPrsnt.GetStatMin(analyResult.FirstChannelIndex - 1) ?? Double.NaN;
                    toCovert = true;
                    break;
                case "Dev":
                    value = measPrsnt.GetStatStddev(analyResult.FirstChannelIndex - 1) ?? Double.NaN;
                    toCovert = true;
                    break;
                case "Pop":
                    value = measPrsnt.GetStatCount(analyResult.FirstChannelIndex - 1);
                    break;
                case "Avg":
                    value = measPrsnt.GetStatAverage(analyResult.FirstChannelIndex - 1) ?? Double.NaN;
                    toCovert = true;
                    break;
                default:
                    break;
            }
            if (value != null && Double.TryParse(value.ToString(), out Double valDlb))
            {
                if (toCovert)
                {
                    (Prefix pfx, String unit) = measPrsnt.GetPfxUnitString(analyResult.FirstChannelIndex - 1);
                    value = SIHelper.SIUnitConversion(valDlb, (Int32)pfx, (Int32)Prefix.Empty);
                    foreach (var timeunit in TimeUnit)
                    {
                        if (unit.Contains(timeunit))
                        {
                            sendMessage.UseShortScientificNotation = false;
                            break;
                        }
                        sendMessage.UseShortScientificNotation = true;
                    }
                }
                sendMessage.SendData = decodeStr(value.ToString());
                return true;
            }
            return false;
        }
        /// <summary>
        /// 查询测量项
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static Boolean scpiQuy_MeasureItemCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            //判断
            if (analyResult.Tag is not ScpiTagObj scpiTagObj || !CheckMeasItemNumber(analyResult, out MeasItemPrsnt itemPrsnt))
            {
                return false;
            }
            PropertyInfo propertyInfo;
            if (String.IsNullOrWhiteSpace(scpiTagObj.PropertyName) && scpiTagObj.Tag != null)
            {
                String tagStr = scpiTagObj.Tag.ToString();
                switch (tagStr)
                {
                    case "Source":
                        tagStr = analyResult.ChannelIndexs.Count > 1 && analyResult.ChannelIndexs[1] == 2 ? "Source2nd" : "Source";
                        break;
                    default:
                        break;
                }
                if (!TryGetPropertyInfo(itemPrsnt, tagStr, out propertyInfo))
                {
                    return false;
                }
            }
            else if (!TryGetPropertyInfo(itemPrsnt, scpiTagObj.PropertyName, out propertyInfo))
            {
                return false;
            }

            if (TryGetPropertyValue(itemPrsnt, propertyInfo, out var usingScientific, out var outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
            {
                sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(propertyInfo.Name);
                sendMessage.UsingScientificNotation = usingScientific;
                sendMessage.SendData = decodeStr(outputString);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 设置测量项
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static Boolean scpiSet_MeasureItemCommon(SCPICommandProcessFuncParam analyResult)
        {
            //判断
            if (!CheckMeasItemNumber(analyResult, out MeasItemPrsnt itemPrsnt)
                || !scpiSet_ParamCheck(analyResult, out var param)
                || analyResult.Tag is not ScpiTagObj tagObj)
            {
                return false;
            }
            PropertyInfo propertyInfo;
            if (string.IsNullOrWhiteSpace(tagObj.PropertyName) && tagObj.Tag != null)
            {
                string tagStr = tagObj.Tag.ToString();
                switch (tagStr)
                {
                    case "Source":
                        tagStr = analyResult.ChannelIndexs.Count > 1 && analyResult.ChannelIndexs[1] == 2 ? "Source2nd" : "Source";
                        break;
                    default:
                        break;
                }
                if (!TryGetPropertyInfo(itemPrsnt, tagStr, out propertyInfo))
                {
                    return false;
                }
            }
            else if (!TryGetPropertyInfo(itemPrsnt, tagObj.PropertyName, out propertyInfo))
            {
                return false;
            }
            return TrySetPropertyValue(itemPrsnt, propertyInfo, param, tagObj.ParamList, tagObj.IntOrDoubleMultiplier);
        }
        #endregion 公有方法

        #region 基础方法属性

        #region 测量
        private static String[] _StdVertItems = new[]
        {
            "Average",
            "Max",
            "Min",
            "Amplitude",
            "Pk2Pk",
            "Top",
            "Base",
            "Mid",
            "RMS",
            "Stddev"
        };

        private static String[] _StdHorzItems = new[]
        {
            "Freq",
            "Period",
            "PWidth",
            "NWidth",
            "Rise",
            "Fall",
            "Duty",
            "NDuty",
            "POverShoot",
            "Cycles",
        };
        /// <summary>
        /// 检查测量项编号
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        private static Boolean CheckMeasItemNumber(SCPICommandProcessFuncParam analyResult, out MeasItemPrsnt? itemPrsnt)
        {
            itemPrsnt = null;
            if (Presenter == null || Presenter.Measure == null)
            {
                return false;
            }
            var measurePrsnt = Presenter.Measure;
            var itemNo = analyResult.FirstChannelIndex - 1;
            if (itemNo < 0 || itemNo >= measurePrsnt.Length)
            {
                return false;
            }
            itemPrsnt = measurePrsnt[itemNo];
            return true;
        }


        #endregion 测量
        #endregion  基础方法属性

    }
}

using ScopeX.Core;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        public static Boolean scpiQuy_TriggerAreaCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;
            var items = Presenter.VisualTrigger.SelectedItems;

            var num = analyResult.ChannelIndex;
            var itemprsnt = num switch
            {
                1 => items.FirstOrDefault(x => x.Name == "A"),
                2 => items.FirstOrDefault(x => x.Name == "B"),
                _ => null
            };
            if (itemprsnt == null)
                return false;

            if (TryGetPropertyInfo(itemprsnt, scpiTagObj.PropertyName, out var propertyInfo))
            {
                if (TryGetPropertyValue(itemprsnt, propertyInfo, out var usingScientific, out var outputString, scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    sendMessage.UsingScientificNotation = usingScientific;
                    sendMessage.SendData = decodeStr(outputString);
                    sendMessage.UseShortScientificNotation = !scpiTagObj.IsTimeOrFreq;//!IsTimeOrFreq(proInfo.Name);
                    return true;
                }
            }

            return false;
        }

        public static Boolean scpiSet_TriggerAreaCommon(SCPICommandProcessFuncParam analyResult)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;

            var items = Presenter.VisualTrigger.SelectedItems;
            var num = analyResult.ChannelIndex;
            var itemprsnt = num switch
            {
                1 => items.FirstOrDefault(x => x.Name == "A"),
                2 => items.FirstOrDefault(x => x.Name == "B"),
                _ => null
            };
            if (itemprsnt == null)
                return false;
            List<String> paramslist = ParamListToStrList(analyResult.Params);
            if (paramslist.Count <= 0)
            {
                return false;
            }
            if (scpiTagObj.Tag != null && scpiTagObj.Tag.ToString() == "Position")
            {
                var par = GetRectangle(paramslist, itemprsnt, scpiTagObj.IntOrDoubleMultiplier);
                itemprsnt.RectanglePoints = par;
                return true;
            }
            if (TryGetPropertyInfo(itemprsnt, scpiTagObj.PropertyName, out var propertyInfo))
            {
                if (TrySetPropertyValue(itemprsnt, propertyInfo, paramslist[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                {
                    return true;
                }
            }
            return false;
        }

        private static (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) GetRectangle(List<String> list, Object prsnt, Int64 multiply)
        {
            List<Double> result = new List<Double>();
            (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) rectangle = (PointF.Empty, PointF.Empty, PointF.Empty, PointF.Empty);

            for (var index = 0; index < list.Count; index++)
            {
                // 定义匹配数字和小数点，并以特定单位结尾的正则表达式
                Regex regex = new Regex(@"^(?<number>[+-]?\d+(\.\d+)?([eE][+-]?\d+)?)\s*(?<unit>[a-zA-Z%]+)$");
                // 进行匹配
                Match match = regex.Match(list[index]);
                if (match.Success)
                {
                    var number = match.Groups["number"].Value;
                    var unit = match.Groups["unit"].Value;
                    var valueString = StringToDecimal(number, unit).ToString();
                    if (Double.TryParse(valueString, out var setvalue))
                    {
                        result.Add(Convert.ToDouble(setvalue));
                    }
                }
                else if (multiply != 0 && Double.TryParse(list[index], out Double value))
                {
                    multiply = index % 2 != 0 ? 1000L : multiply;
                    var valueBydecimal = (Decimal)value;//long类型的整数与double相乘时，可能会出现精度损失；
                    result.Add((Double)(valueBydecimal * multiply));
                }
            }

            //将值转换为us和mv
            var pointleftx = result[0];//,Prefix.Micro);
            var pointlefty = result[1];//Prefix.Milli);
            var pointrightx = result[2];// Prefix.Micro);
            var pointrighty = result[3];//Prefix.Milli);

            //转化值为虚拟坐标
            if (prsnt is VisualTriggerItemPrsnt vip)
            {
                var leftup = new PointF(vip.VitrualValue2PointValueByvs((float)pointleftx), vip.VirualValue2PointValueBymV((float)pointlefty, vip.VerticalPosIndexBymDiv, vip.VerticalScale));
                var rightup = new PointF(vip.VitrualValue2PointValueByvs((float)pointrightx), vip.VirualValue2PointValueBymV((float)pointlefty, vip.VerticalPosIndexBymDiv, vip.VerticalScale));
                var rightdown = new PointF(vip.VitrualValue2PointValueByvs((float)pointrightx), vip.VirualValue2PointValueBymV((float)pointrighty, vip.VerticalPosIndexBymDiv, vip.VerticalScale));
                var leftdown = new PointF(vip.VitrualValue2PointValueByvs((float)pointleftx), vip.VirualValue2PointValueBymV((float)pointrighty, vip.VerticalPosIndexBymDiv, vip.VerticalScale));
                rectangle = (leftup, rightup, rightdown, leftdown);
            }

            if (prsnt is AreaHistogramPrsnt ap)
            {
                var leftup = new PointF(ap.VitrualValue2PointValueByvs((float)pointleftx), ap.VirualValue2PointValueBymV((float)pointlefty, ap.VerticalPosIndexBymDiv, ap.VerticalScale));
                var rightup = new PointF(ap.VitrualValue2PointValueByvs((float)pointrightx), ap.VirualValue2PointValueBymV((float)pointlefty, ap.VerticalPosIndexBymDiv, ap.VerticalScale));
                var rightdown = new PointF(ap.VitrualValue2PointValueByvs((float)pointrightx), ap.VirualValue2PointValueBymV((float)pointrighty, ap.VerticalPosIndexBymDiv, ap.VerticalScale));
                var leftdown = new PointF(ap.VitrualValue2PointValueByvs((float)pointleftx), ap.VirualValue2PointValueBymV((float)pointrighty, ap.VerticalPosIndexBymDiv, ap.VerticalScale));
                rectangle = (leftup, rightup, rightdown, leftdown);
            }

            return rectangle;
        }
    }
}

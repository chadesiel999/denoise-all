using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.SCPIManager;
using ScopeX.Core;
using System.Reflection;
using System.ComponentModel;
using ScopeX.ComModel;
using System.Text.RegularExpressions;

namespace ScopeX.Scpi
{
    partial class StubFunc
    {
        //================= 触发-码型 ===========================================================================================
        /// <summary>
        /// 设置或查询码型触发逻辑资格的所有通道触发条件
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_TriggerLogicCondition(SCPICommandProcessFuncParam analyResult)
        {
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            List<string> param = ParamListToStrList(analyResult.Params);

            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || string.IsNullOrWhiteSpace($"{scpiTagObj.Tag}"))
                return false;

            TriggerPrsnt triggerPrsnt = Presenter.CurrentTrigger;

            var valueString = param[0];
            var paramList = scpiTagObj.ParamList;
            List<ChannelId> triggerSource = new List<ChannelId>();
            var patLevel = ConvertObject(valueString, typeof(PatLevelCondition), paramList);
            switch ($"{scpiTagObj.Tag}")
            {
                case "ALL":
                    for (int i = 0; i < 4; i++)
                    {
                        triggerSource.Add(ChannelId.C1 + i);
                    }
                    for (int i = 0; i < 16; i++)
                    {
                        triggerSource.Add(ChannelId.D0 + i);
                    }
                    break;
                case "C":
                    triggerSource.Add(ChannelId.C1 + analyResult.FirstChannelIndex - 1);
                    break;
                case "D":
                    triggerSource.Add(ChannelId.D0 + analyResult.FirstChannelIndex - 1);
                    break;
                default:
                case "EXT":
                    triggerSource.Add(ChannelId.Ext);
                    break;
            }
            if (triggerPrsnt is TrigPatPrsnt trigPatPrsnt)
            {
                foreach (var tSource in triggerSource)
                {
                    trigPatPrsnt.SetPosCompCondition(tSource, (PatLevelCondition)patLevel);
                }
            }
            else if (triggerPrsnt is TrigStatePrsnt trigStatePrsnt)
            {
                foreach (var tSource in triggerSource)
                {
                    trigStatePrsnt.SetPosCompCondition(tSource, (PatLevelCondition)patLevel);
                }
            }
            else if (triggerPrsnt is TrigSustainTimePrsnt trigSustain)
            {
                foreach (var tSource in triggerSource)
                {
                    trigSustain.SetPosCompCondition(tSource, (SustainTimeLevelCondition)patLevel);
                }
            }
            else
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 设置或查询码型触发逻辑资格的单个模拟通道触发条件
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_TriggerLogicCondition(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;

            TriggerPrsnt triggerPrsnt = Presenter.CurrentTrigger;
            ChannelId triggerSource;
            switch ($"{scpiTagObj.Tag}")
            {
                case "C":
                    triggerSource = ChannelId.C1 + analyResult.FirstChannelIndex - 1;
                    break;
                case "D":
                    triggerSource = ChannelId.D0 + analyResult.FirstChannelIndex - 1;
                    break;
                case "Ext":
                    triggerSource = ChannelId.Ext;
                    break;
                default:
                    triggerSource = ChannelId.Ext;
                    break;
            }

            if (triggerPrsnt is TrigPatPrsnt trigPatPrsnt)
            {
                var condition = trigPatPrsnt.GetPosCompCondition(triggerSource);

                var outputstring = scpiTagObj.ParamList.Count > 0 ? scpiTagObj.ParamList.Count > (int)condition ? scpiTagObj.ParamList[(int)condition] : scpiTagObj.ParamList.FirstOrDefault() : $"{condition}";

                sendMessage.SendData = decodeStr($"{outputstring}");

                return true;
            }
            else if (triggerPrsnt is TrigSustainTimePrsnt trigSustain)
            {
                var condition = trigSustain.GetPosCompCondition(triggerSource);
                var outputstring = scpiTagObj.ParamList.Count > 0 ? scpiTagObj.ParamList.Count > (int)condition ? scpiTagObj.ParamList[(int)condition] : scpiTagObj.ParamList.FirstOrDefault() : $"{condition}";

                sendMessage.SendData = decodeStr($"{outputstring}");

                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置或查询码型触发逻辑资格的单个模拟通道门限
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_TriggerLogicGATe(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || scpiTagObj.PrsntObj is not Type prsntType)
                return false;

            TriggerPrsnt triggerPrsnt = Presenter.CurrentTrigger;
            ChannelId triggerSource;
            switch ($"{scpiTagObj.Tag}")
            {
                case "C":
                    triggerSource = ChannelId.C1 + analyResult.FirstChannelIndex - 1;
                    break;
                case "D":
                    triggerSource = ChannelId.D0 + analyResult.FirstChannelIndex - 1;
                    break;
                case "Ext":
                    triggerSource = ChannelId.Ext;
                    break;
                default:
                    triggerSource = ChannelId.Ext;
                    break;
            }

            if (triggerPrsnt is TrigPatPrsnt trigPatPrsnt)
            {
                var condition = trigPatPrsnt.GetCompPosition(triggerSource);

                //var outputstring = scpiTagObj.ParamList.Count > 0 ? scpiTagObj.ParamList.Count > (int)condition ? scpiTagObj.ParamList[(int)condition] : scpiTagObj.ParamList.FirstOrDefault() : $"{condition}";

                var gate = scpiTagObj.IntOrDoubleMultiplier == 0 ? condition : condition / scpiTagObj.IntOrDoubleMultiplier;

                sendMessage.SendData = decodeStr($"{gate}");

                return true;
            }
            else if (triggerPrsnt is TrigSustainTimePrsnt trigSustain)
            {
                var condition = trigSustain.GetCompPosition(triggerSource);

                //var outputstring = scpiTagObj.ParamList.Count > 0 ? scpiTagObj.ParamList.Count > (int)condition ? scpiTagObj.ParamList[(int)condition] : scpiTagObj.ParamList.FirstOrDefault() : $"{condition}";

                var gate = scpiTagObj.IntOrDoubleMultiplier == 0 ? condition : condition / scpiTagObj.IntOrDoubleMultiplier;

                sendMessage.SendData = decodeStr($"{gate}");

                return true;
            }

            return false;
        }
        /// <summary>
        /// 设置或查询码型触发逻辑资格的单个模拟通道门限
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_TriggerLogicGATe(SCPICommandProcessFuncParam analyResult)
        {
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (analyResult.Tag == null || analyResult.Tag is not ScpiTagObj scpiTagObj || string.IsNullOrWhiteSpace($"{scpiTagObj.Tag}"))
            {
                return false;
            }
            TriggerPrsnt triggerPrsnt = Presenter.CurrentTrigger;

            ChannelId triggerSource;
            switch ($"{scpiTagObj.Tag}")
            {
                case "C":
                    triggerSource = ChannelId.C1 + analyResult.FirstChannelIndex - 1;
                    break;
                case "D":
                    triggerSource = ChannelId.D0 + analyResult.FirstChannelIndex - 1;
                    break;
                case "Ext":
                    triggerSource = ChannelId.Ext;
                    break;
                default:
                    triggerSource = ChannelId.Ext;
                    break;
            }
            List<string> param = ParamListToStrList(analyResult.Params);
            if (param.Count <= 0)
            {
                return false;
            }

            // 定义匹配数字和小数点，并以特定单位结尾的正则表达式
            Regex regex = new Regex(@"^(?<number>[+-]?\d+(\.\d+)?([eE][+-]?\d+)?)\s*(?<unit>[a-zA-Z%]+)$");
            var valueString = param[0];
            var multiply = scpiTagObj.IntOrDoubleMultiplier;
            var setvalue = 0.0m;
            // 进行匹配
            Match match = regex.Match(valueString);
            if (match.Success)
            {
                string number = match.Groups["number"].Value;
                string unit = match.Groups["unit"].Value;

                setvalue = StringToDecimal(number, unit);
            }
            else if (multiply != 0 && double.TryParse(valueString, out double value))
            {
                var valueBydecimal = (decimal)value;//long类型的整数与double相乘时，可能会出现精度损失；
                setvalue = valueBydecimal * multiply;
            }

            if (triggerPrsnt is TrigPatPrsnt trigPatPrsnt)
            {
                trigPatPrsnt.SetCompPosition(triggerSource, (double)setvalue);
                return true;
            }
            else if (triggerPrsnt is TrigSustainTimePrsnt trigSustain)
            {
                trigSustain.SetCompPosition(triggerSource, (double)setvalue);
                return true;
            }
            return false;
        }
    }
}
//================= 共4个方法 =

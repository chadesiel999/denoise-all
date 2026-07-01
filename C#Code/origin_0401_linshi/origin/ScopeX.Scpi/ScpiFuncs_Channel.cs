using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.SCPIManager;
using ScopeX.Core;
using System.Reflection;
using ScopeX.ComModel;
using System.ComponentModel;
using System.Xml.Schema;

namespace ScopeX.Scpi
{
    partial class StubFunc
    {
        //================= 通道 =================================================================================================
        /// <summary>
        /// 设置或查询  带宽限制
        /// </summary>
        /// <param name="analyResult"></param>
        /// <param name="sendMessage"></param>
        /// <returns></returns>
        public static bool scpiQuy_AnalogChannelBWLimit(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetAnalogChannelPrsnt(analyResult, out AnalogPrsnt prsnt))
            {
                var index = prsnt.Bandwidth;
                var bandlist = new List<(Int32 Index, String Name)>();
                //bandlist = (prsnt.Coupling == AnaChnlCoupling.DC50 ? ComModel.Constants.LZBANDWIDTHNAMES : Constants.HZBANDWIDTHNAMES).ToList();
                bandlist = prsnt.BandWidthNames.ToList();
                if (prsnt.Coupling != AnaChnlCoupling.DC50 && index == 0)
                {
                    index = 1;
                }

                var band = prsnt.BandWidthNames[index].Name;
                sendMessage.SendData = decodeStr(band);
                //Debug.WriteLine("Test39:{band}");
                returnResult = true;
            }
            return returnResult;
        }


        /// <summary>
        /// 设置或查询 带宽限制
        /// </summary>
        /// <param name="analyResult"></param>
        /// <returns></returns>
        public static bool scpiSet_AnalogChannelBWLimit(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetAnalogChannelPrsnt(analyResult, out AnalogPrsnt prsnt))
            {
                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                {
                    List<string> param = ParamListToStrList(analyResult.Params);
                    if (param.Count > 0)
                    {
                        var item = prsnt.BandWidthNames.FirstOrDefault(x => x.Name.ToUpper() == param[0].ToUpper());
                        if (item != (0, null))
                        {
                            prsnt.Bandwidth = item.Index;
                            returnResult = true;
                        }
                        if (param[0].Trim().ToUpper() == "FULL")
                        {
                            prsnt.Bandwidth = 0;
                            returnResult = true;
                        }

                    }
                }
            }
            return returnResult;
        }
        #region 通道输入端口，使用了object类型
        private static int AnalogChannel_FindIndex(List<string> list, string value)
        {
            int index = 0;
            foreach (string s in list)
            {
                if (s == value)
                    break;
                index++;
            }
            return index;
        }
        public static bool scpiQuy_AnalogChannelInput(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetAnalogChannelPrsnt(analyResult, out AnalogPrsnt analogPrsnt))
            {

                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                sendMessage.SendData = decodeStr(scpiTagObj.ParamList[(int)analogPrsnt.FlagInfo]);
                returnResult = true;
            }
            return returnResult;
        }

        public static bool scpiSet_AnalogChannelInput(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetAnalogChannelPrsnt(analyResult, out AnalogPrsnt analogPrsnt))
            {
                List<string> param = ParamListToStrList(analyResult.Params);
                if (param.Count > 0)
                {
                    ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                    analogPrsnt.FlagInfo = AnalogChannel_FindIndex(scpiTagObj.ParamList, param[0]);
                    returnResult = true;
                }
            }
            return returnResult;
        }
        #endregion

        #region 带宽限制，使用了任意配置方式，故需要重写
        public static bool scpiQuy_AnalogChannelBandwidth(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            bool returnResult = false;
            if (TryGetAnalogChannelPrsnt(analyResult, out AnalogPrsnt analogPrsnt))
            {
                var items = analogPrsnt.BandWidthNames.Select(o => new KeyValuePair<Int32, String>(o.Index, o.Name)).ToList();
                //var items = (analogPrsnt.Coupling == AnaChnlCoupling.DC50 ? ComModel.Constants.LZBANDWIDTHNAMES : Constants.HZBANDWIDTHNAMES).Select(o => new KeyValuePair<Int32, String>(o.Index, o.Name)).ToList();

                foreach (var item in items)
                {
                    if (item.Key == analogPrsnt.Bandwidth)
                    {
                        sendMessage.SendData = decodeStr(item.Value);
                        returnResult = true;
                        break;
                    }
                }
            }
            return returnResult;
        }
        public static bool scpiSet_AnalogChannelBandwidth(SCPICommandProcessFuncParam analyResult)
        {
            bool returnResult = false;
            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }
            if (TryGetAnalogChannelPrsnt(analyResult, out AnalogPrsnt analogPrsnt))
            {
                //var items = (analogPrsnt.Coupling == AnaChnlCoupling.DC50 ? ComModel.Constants.LZBANDWIDTHNAMES : Constants.HZBANDWIDTHNAMES).Select(o => new KeyValuePair<String, Int32>(o.Name, o.Index)).ToList();
                var items = analogPrsnt.BandWidthNames.Select(o => new KeyValuePair<String, Int32>(o.Name, o.Index)).ToList();

                ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
                if (TryGetPropertyInfo(analogPrsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
                {
                    List<string> param = ParamListToStrList(analyResult.Params);
                    if (param.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            if (item.Key == param[0])
                            {
                                analogPrsnt.Bandwidth = item.Value;
                                returnResult = true;
                                break;
                            }
                        }
                    }
                }
            }
            return returnResult;
        }
        #endregion
    }
}

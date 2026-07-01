using ScopeX.Core;
using ScopeX.SCPIManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Scpi
{
    internal partial class StubFunc
    {
        private static LANItemPrsnt? TryGetLANItemPrsnt()
        {
            if (Presenter.LAN.ConnectedEthernetAdapters.Count > 0)
            {
                if (Presenter.LAN.CurrentSelect >= Presenter.LAN.ConnectedEthernetAdapters.Count)
                {
                    Presenter.LAN.CurrentSelect = 0;
                }
                return Presenter.LAN.ConnectedEthernetAdapters[Presenter.LAN.CurrentSelect];

            }

            if (Presenter.LAN.EthernetAdapters.Count > 0)
            {
                if (Presenter.LAN.CurrentSelect >= Presenter.LAN.EthernetAdapters.Count)
                {
                    Presenter.LAN.CurrentSelect = 0;
                }
                return Presenter.LAN.EthernetAdapters[Presenter.LAN.CurrentSelect];
            }

            return null;
        }

        public static bool scpiQuy_LanAdapterCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            var prsnt = TryGetLANItemPrsnt();

            if (prsnt == null)
                return false;

            var allinfo = prsnt.GetAdapterConfig();
            if (allinfo == null)
                return false;

            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var outputString = string.Empty;
            if (scpiTagObj.Tag != null && scpiTagObj.Tag.ToString() == "Names")
            {
                outputString = $"{Presenter.LAN.ConnectedEthernetAdapters.Count}";
            }
            else if (scpiTagObj.Tag != null && scpiTagObj.Tag.ToString() == "Select")
            {
                outputString = $"{Presenter.LAN.CurrentSelect + 1}";
            }
            else
                outputString = allinfo.FirstOrDefault(info => info.PropertyName == scpiTagObj.PropertyName).Value;

            sendMessage.UseShortScientificNotation = false;
            sendMessage.UsingScientificNotation = false;
            sendMessage.SendData = decodeStr(outputString);

            return true;
        }

        public static bool scpiSet_LanAdapterCommon(SCPICommandProcessFuncParam analyResult)
        {
            var prsnt = TryGetLANItemPrsnt();
            if (prsnt == null)
                return false;

            if (!scpiSet_ParamCheck(analyResult))
            {
                return false;
            }

            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            if (scpiTagObj.PropertyName == nameof(Presenter.LAN.CurrentSelect))
            {
                if (TryGetPropertyInfo(Presenter.LAN, scpiTagObj.PropertyName, out PropertyInfo selectpropertyInfo))
                {
                    List<string> param = ParamListToStrList(analyResult.Params);
                    if (param.Count > 0)
                    {
                        if (Int32.TryParse(param[0], out var setvalue))
                        {
                            setvalue = setvalue <= 0 ? 0 : setvalue - 1;
                            if (setvalue >= Presenter.LAN.ConnectedEthernetAdapters.Count)
                            {
                                setvalue = Presenter.LAN.ConnectedEthernetAdapters.Count - 1;
                            }
                            if (TrySetPropertyValue(Presenter.LAN, selectpropertyInfo, $"{setvalue}", scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                                return true;
                        }
                    }
                }
            }
            else if (TryGetPropertyInfo(prsnt, scpiTagObj.PropertyName, out PropertyInfo propertyInfo))
            {
                List<string> param = ParamListToStrList(analyResult.Params);
                if (param.Count > 0)
                {
                    if (TrySetPropertyValue(prsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        return true;
                }
            }

            return false;
        }

        public static bool scpiSet_LanAdapter(SCPICommandProcessFuncParam analyResult)
        {
            var prsnt = TryGetLANItemPrsnt();
            if (prsnt == null)
                return false;
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            switch (scpiTagObj.Tag.ToString())
            {
                case "Reset":
                    Task.Run(() =>
                    {
                        prsnt.SetIPAddress("0.0.0.0", "255.255.255.0", prsnt.Adapter!.GateWay);
                        prsnt.DHCP = ComModel.GetIPMethod.Auto;
                        Presenter.LAN.Refesh();
                    });
                    break;
                case "AppLy":
                    Task.Run(() =>
                    {
                        prsnt.SetIPAddress(prsnt.IPAddress ?? prsnt.Adapter!.IpAddress, prsnt.SubMask ?? prsnt.Adapter!.Mask, prsnt.GateWay ?? prsnt.Adapter!.GateWay);
                        Presenter.LAN.Refesh();
                    });
                    break;
                default:
                    break;
            }

            return true;
        }

        public static bool scpiQuy_WifiAdapterCommon(SCPICommandProcessFuncParam analyResult, ref SCPISendMessage sendMessage)
        {
            var prsnt = Presenter.LAN.WiFiAdapter;
            if (prsnt == null)
                return false;

            var allinfo = prsnt.GetAdapterConfig();
            if (allinfo == null)
                return false;

            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            var outputString = allinfo.FirstOrDefault(info => info.PropertyName == scpiTagObj.PropertyName).Value;

            sendMessage.UseShortScientificNotation = false;//!IsTimeOrFreq(propertyInfo.Name);
            sendMessage.UsingScientificNotation = false;
            sendMessage.SendData = decodeStr(outputString);
            return true;
        }

        public static bool scpiSet_WifiAdapterCommon(SCPICommandProcessFuncParam analyResult)
        {
            var prsnt = Presenter.LAN.WiFiAdapter;
            if (prsnt == null)
                return false;

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
                    if (TrySetPropertyValue(prsnt, propertyInfo, param[0], scpiTagObj.ParamList, scpiTagObj.IntOrDoubleMultiplier))
                        return true;
                }
            }

            return false;
        }

        public static bool scpiSet_WifiAdapter(SCPICommandProcessFuncParam analyResult)
        {
            var prsnt = Presenter.LAN.WiFiAdapter;
            if (prsnt == null)
                return false;
            ScpiTagObj scpiTagObj = (ScpiTagObj)analyResult.Tag;
            switch (scpiTagObj.Tag.ToString())
            {
                case "Reset":
                    Task.Run(() =>
                    {
                        prsnt.SetIPAddress("0.0.0.0", "255.255.255.0", prsnt.Adapter!.GateWay);
                        prsnt.DHCP = ComModel.GetIPMethod.Auto;
                        Presenter.LAN.Refesh();
                    });
                    break;
                case "AppLy":
                    Task.Run(() =>
                    {
                        prsnt.SetIPAddress(prsnt.IPAddress ?? prsnt.Adapter!.IpAddress, prsnt.SubMask ?? prsnt.Adapter!.Mask, prsnt.GateWay ?? prsnt.Adapter!.GateWay);
                        Presenter.LAN.Refesh();
                    });
                    break;
                default:
                    break;
            }

            return true;
        }
    }
}
//================= 共4个方法 =

using ScopeX.Core.Tools;
using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace ScopeX.Core
{
    /// <summary>
    /// 网络适配器类
    /// </summary>
    public class NetworkAdapter
    {
        #region 属性

        /// <summary>
        /// 网络适配器标识符，如：{274F9DD5-3650-4D59-B61E-710B6AF5AB36}
        /// </summary>
        public String? NetworkInterfaceID { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public String IpAddress
        {
            get
            {
                var address = GetAddress();
                return address == null ? "" : address.Address.ToString();

            }
        }

        /// <summary>
        /// 网关地址
        /// </summary>
        public String GateWay
        {
            get
            {
                String gateWay = GateWayes != null && GateWayes.Count > 0
                    ? GateWayes[0].Address.ToString() == "0.0.0.0" ? "" : GateWayes[0].Address.ToString()
                    : "";
                return gateWay;
            }
        }

        /// <summary>
        /// DHCP服务器地址
        /// </summary>
        public String DhcpServer
        {
            get
            {
                String dhcpserver = DhcpServerAddresses != null && DhcpServerAddresses.Count > 0 ? DhcpServerAddresses[0].ToString() : "";
                return dhcpserver;
            }
        }

        /// <summary>
        /// MAC地址
        /// </summary>
        public String MacAddres
        {
            get
            {
                String macaddress = MacAddress == null
                    ? ""
                    : MacAddress.ToString().Length == 12
                        ? MacAddress.ToString().Insert(2, ":").Insert(5, ":").Insert(8, ":").Insert(11, ":").Insert(14, ":")
                        : MacAddress.ToString();
                return macaddress;
            }
        }

        /// <summary>
        /// 主DNS地址
        /// </summary>
        public String DnsMain
        {
            get
            {
                String dnsMain = "";

                if (DnsAddresses != null && DnsAddresses.Count > 0)
                {
                    if (IsIPAddress(DnsAddresses[0].ToString()))
                    {
                        dnsMain = DnsAddresses[0].ToString();
                    }
                }
                else
                {
                    dnsMain = "";
                }
                return dnsMain;
            }
        }

        /// <summary>
        /// 备用DNS地址
        /// </summary>
        public String DnsBackup
        {
            get
            {
                String dnsBackup = "";
                if (DnsAddresses != null && DnsAddresses.Count > 1)
                {
                    if (IsIPAddress(DnsAddresses[1].ToString()))
                    {
                        dnsBackup = DnsAddresses[1].ToString();
                    }
                }
                else
                {
                    dnsBackup = "";
                }
                return dnsBackup;
            }
        }

        /// <summary>
        /// 子网掩码
        /// </summary>
        public String Mask
        {
            get
            {
                var address = GetAddress();
                return address == null ? "" : address.IPv4Mask != null ? address.IPv4Mask.ToString() : "255.255.255.0";
            }
        }

        /// <summary>
        /// DNS集合
        /// </summary>
        internal IPAddressCollection? DnsAddresses { get; set; }
        /// <summary>
        /// 网关地址集合
        /// </summary>
        internal GatewayIPAddressInformationCollection? GateWayes { get; set; }

        /// <summary>
        /// IP地址集合
        /// </summary>
        internal UnicastIPAddressInformationCollection? IPAddresses { get; set; }

        /// <summary>
        /// DHCP地址集合
        /// </summary>
        internal IPAddressCollection? DhcpServerAddresses { get; set; }

        /// <summary>
        /// 网卡MAC地址
        /// </summary>
        internal PhysicalAddress? MacAddress { get; set; }

        /// <summary>
        /// 是否启用DHCP服务
        /// </summary>
        public Boolean IsDhcpEnabled { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public String? Description { get; set; }
        /// <summary>
        /// 网络接口类型
        /// </summary>
        /// <returns></returns>
        public String? NetworkInterfaceType { get; set; }

        /// <summary>
        /// 速度
        /// </summary>
        public String? Speed { get; set; }

        private Boolean CheckAddressFormat(String? address, Boolean showTips = true)
        {
            if (String.IsNullOrWhiteSpace(address) || address.Equals("...") || address.Equals("0.0.0.0") || !IsIPAddress(address))
            {
                if (showTips)
                    WeakTip.Default.Write("NetWork", MsgTipId.SetIPAddressFail);
                return false;
            }
            return true;
        }

        #endregion

        #region public

        /// <summary>
        /// 是否是正确IP地址
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public Boolean IsIPAddress(String ipAddress)
        {
            String regexStr = @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$";
            Match regex = Regex.Match(ipAddress, regexStr);
            return regex.Success;
        }

        /// <summary>
        /// 检查设置IP地址,如果返回空，表示检查通过，为了方便返回字符串
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="subMask"></param>
        /// <param name="getWay"></param>
        /// <param name="dnsMain"></param>
        /// <param name="dnsBackup"></param>
        /// <returns></returns>
        public String IsIPAddress(String ipAddress, String subMask, String getWay, String dnsMain, String dnsBackup)
        {
            if (!String.IsNullOrEmpty(ipAddress))
            {
                if (!IsIPAddress(ipAddress))
                    return "The Ip Address Format Is Incorrect";
            }
            if (!String.IsNullOrEmpty(subMask))
            {
                if (!IsIPAddress(subMask))
                    return "The Subnet Mask Format Is Incorrect";
            }
            if (!String.IsNullOrEmpty(getWay))
            {
                if (!IsIPAddress(getWay))
                    return "The Gateway Address Format Is Incorrect";
            }
            if (!String.IsNullOrEmpty(dnsMain))
            {
                if (!IsIPAddress(dnsMain))
                    return "The Primary Dns Address Format Is Incorrect";
            }
            if (!String.IsNullOrEmpty(dnsBackup))
            {
                if (!IsIPAddress(dnsBackup))
                    return "The Alternate Dns Address Format Is Incorrect";
            }
            return "";
        }

        /// <summary>
        /// 启用DHCP服务
        /// </summary>
        public Boolean EnableDHCP()
        {
            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (!(Boolean)mo["IPEnabled"])
                    continue;

                if (mo["SettingID"].ToString() == this.NetworkInterfaceID) //网卡接口标识是否相等
                {
                    mo.InvokeMethod("SetDNSServerSearchOrder", null);
                    mo.InvokeMethod("EnableDHCP", null);

                }
            }

            NetworkAdapter networkAdapter = new NetworkAdapterUtil().GetNeworkAdapterByNetworkInterfaceID(this.NetworkInterfaceID); //查询现适配器接口信息
            return networkAdapter != null ? networkAdapter.IsDhcpEnabled : false;
        }

        /// <summary>
        /// 设置IP地址,子网掩码，网关,DNS,
        /// </summary>
        public Boolean SetIPAddressSubMaskDnsGetway(String ipAddress, String subMask, String getWay, String dnsMain, String dnsBackup)
        {
            String[] dnsArray = String.IsNullOrEmpty(dnsBackup) ? (new String[] { dnsMain }) : (new String[] { dnsMain, dnsBackup });
            return SetIPAddress(new String[] { ipAddress }, new String[] { subMask }, new String[] { getWay }, dnsArray);

        }

        /// <summary>
        /// 设置IP地址和子网掩码
        /// </summary>
        public Boolean SetIPAddressAndSubMask(String ipAddress, String subMask)
        {
            return SetIPAddress(new String[] { ipAddress }, new String[] { subMask }, null, null);
        }

        /// <summary>
        /// 设置IP网关
        /// </summary>
        public Boolean SetGetWayAddress(String getWay)
        {
            return SetIPAddress(null, null, new String[] { getWay }, null);
        }

        /// <summary>
        /// 设置主,备份DNS地址
        /// </summary>
        public Boolean SetDNSAddress(String dnsMain, String dnsBackup)
        {
            String[] dnsArray = String.IsNullOrEmpty(dnsBackup) ? (new String[] { dnsMain }) : (new String[] { dnsMain, dnsBackup });
            return SetIPAddress(null, null, null, dnsArray);
        }

        #endregion

        #region private

        /// <summary>
        /// 得到IPV4地址
        /// </summary>
        /// <returns></returns>
        private UnicastIPAddressInformation? GetAddress()
        {
            return IPAddresses?.FirstOrDefault(ip => ip.PrefixOrigin == PrefixOrigin.Manual || ip.PrefixOrigin == PrefixOrigin.Dhcp);
        }

        /// <summary>
        /// 设置IP地址
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="submask"></param>
        /// <param name="getway"></param>
        /// <param name="dns"></param>
        private Boolean SetIPAddress(String?[] ip, String?[] submask, String?[] getway, String?[] dns)
        {
            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            ManagementBaseObject? inPar = null;
            ManagementBaseObject? outPar = null;
            String? str = "";
            foreach (ManagementObject mo in moc)
            {

                if (!(Boolean)mo["IPEnabled"])
                    continue;

                if (this.NetworkInterfaceID == mo["SettingID"].ToString())
                {
                    if (ip != null && submask != null)
                    {
                        String? caption = mo["Caption"].ToString(); //描述
                        inPar = mo.GetMethodParameters("EnableStatic");
                        inPar["IPAddress"] = ip;
                        inPar["SubnetMask"] = submask;
                        outPar = mo.InvokeMethod("EnableStatic", inPar, null);
                        str = outPar["returnvalue"].ToString();
                        return (str == "0" || str == "1") ? true : false;
                        //获取操作设置IP的返回值， 可根据返回值去确认IP是否设置成功。 0或1表示成功
                    }
                    if (getway != null)
                    {
                        inPar = mo.GetMethodParameters("SetGateways");
                        inPar["DefaultIPGateway"] = getway;
                        outPar = mo.InvokeMethod("SetGateways", inPar, null);
                        str = outPar["returnvalue"].ToString();
                        return (str == "0" || str == "1") ? true : false;
                    }
                    if (dns != null)
                    {
                        inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                        inPar["DNSServerSearchOrder"] = dns;
                        outPar = mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);
                        str = outPar["returnvalue"].ToString();
                        return (str == "0" || str == "1") ? true : false;
                    }

                }
            }
            return false;
        }

        /// <summary>
        /// 清空网关信息
        /// </summary>
        internal void ClearGetWay()
        {
            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            foreach (ManagementObject mo in moc)
            {

                if (!(Boolean)mo["IPEnabled"])
                    continue;

                if (this.NetworkInterfaceID == mo["SettingID"].ToString())
                {

                    var inpar = mo.GetMethodParameters("SetGateways");
                    inpar["DefaultIPGateway"] = null;
                    inpar["GatewayCostMetric"] = null;
                    mo.InvokeMethod("SetGateways", inpar, null);
                }
            }
        }

        /// <summary>
        /// 清空DNS信息
        /// </summary>
        internal void ClearDNSInfo()
        {
            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            foreach (ManagementObject mo in moc)
            {

                if (!(Boolean)mo["IPEnabled"])
                    continue;

                if (this.NetworkInterfaceID == mo["SettingID"].ToString())
                {
                    var inpar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                    inpar["DNSServerSearchOrder"] = null;
                    mo.InvokeMethod("SetDNSServerSearchOrder", inpar, null);
                }
            }
        }

        /// <summary>
        /// 设置Ip地址
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="subnetMask">子网掩码</param>
        /// <param name="gateway">默认网关</param>
        /// <returns></returns>
        internal Boolean SetIPAddress(String ipAddress, String subnetMask, String gateway)
        {
            //设置之前先要设置成手动
            var bok = DisableDHCP(Description);
            if (!bok)
            {
                return false;
            }

            // IP地址 和子网掩码是必须设置的，其它的可以根据需要设置。
            if (!CheckAddressFormat(ipAddress, false) || !CheckAddressFormat(subnetMask, false))
            {
                return false;
            }
            var arguments = $"interface ip set address \"{Description}\" static {ipAddress} {subnetMask} {gateway}";
            bok = ExecuteNetshCommand(arguments);

            return bok;
        }

        /// <summary>
        /// 使用 netsh 命令禁用网络适配器的 DHCP
        /// </summary>
        /// <param name="adapterName">网络适配器名称</param>
        /// <returns>true --> 禁用成功，false -- > 禁用失败</returns>
        public Boolean DisableDHCP(String? adapterName)
        {
            if (String.IsNullOrEmpty(adapterName))
            {
                return false;
            }

            var arguments = $"interface ip set address \"{adapterName}\" static";
            var bok = ExecuteNetshCommand(arguments);

            return bok;
        }

        /// <summary>
        /// 使用 netsh 命令启用网络适配器的 DHCP
        /// </summary>
        /// <param name="adapterName">网络适配器名称</param>
        /// <returns>true --> 启用成功，false -- > 启用失败</returns>
        public Boolean EnableDHCP(String? adapterName)
        {
            if (String.IsNullOrEmpty(adapterName))
            {
                return false;
            }

            // 启用 DHCP 以自动获取 IP 地址
            var enabledhcpargs = $"interface ip set address \"{adapterName}\" dhcp";
            // 启用 DHCP 以自动获取 DNS 服务器
            var enablednsdhcpargs = $"interface ip set dns \"{adapterName}\" dhcp";

            var bok = ExecuteNetshCommand(enabledhcpargs);
            bok = ExecuteNetshCommand(enablednsdhcpargs);
            return bok;
        }

        /// <summary>
        /// 执行 netsh 命令
        /// </summary>
        /// <param name="arguments">netsh 的参数</param>
        private Boolean ExecuteNetshCommand(String arguments)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("netsh", arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            try
            {
                using (var process = Process.Start(processInfo))
                {
                    // 获取输出和错误信息
                    var output = process?.StandardOutput.ReadToEnd();
                    var error = process?.StandardError.ReadToEnd();

                    process?.WaitForExit();
                    if (!String.IsNullOrEmpty(error))
                    {
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this,
                               new EventBus.LogEventArgs($"Error ({arguments}): " + error, EventBus.LogLevel.Error));
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this,
                              new EventBus.LogEventArgs($"Error ({arguments}): " + ex.Message, EventBus.LogLevel.Error));
                return false;
            }
        }

        #endregion
    }
}

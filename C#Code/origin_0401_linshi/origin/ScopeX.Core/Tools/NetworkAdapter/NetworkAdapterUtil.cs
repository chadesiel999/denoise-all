using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace ScopeX.Core
{
    /// <summary>
    /// 网络适配器工具类，可获取指定网络适配器，集合
    /// </summary>
    public class NetworkAdapterUtil
    {
        #region 私有字段

        /// <summary>
        /// 用于储存适配器集合
        /// </summary>
        private List<NetworkAdapter>? _AdapterList;

        #endregion

        #region public

        /// <summary>
        /// 获取电脑适配器个数
        /// </summary>
        /// <returns>总个数</returns>
        public Int32 GetCount()
        {
            if (_AdapterList == null)
            {
                // GetAllNetworkAdaptersUPAndDown();
                GetAllNetworkAdapters();
                return 0;
            }
            return _AdapterList.Count;
        }

        /// <summary>
        /// 获取 Ethernet已连接适配器，适配器被禁用则不能获取到
        /// </summary>
        /// <returns></returns>
        public List<NetworkAdapter> GetEthernetAdaptersUP()
        {
            //获得所有UP适配器
            var adapters = NetworkInterface.GetAllNetworkInterfaces().Where(d => d.OperationalStatus == OperationalStatus.Up);
            return GetNetworkAdapters(adapters, NetworkInterfaceType.Ethernet, NetworkInterfaceType.Wireless80211);
        }

        /// <summary>
        /// 获取 Ethernet适配器，适配器被禁用则不能获取到
        /// </summary>
        /// <returns></returns>
        public List<NetworkAdapter> GetEthernetAdapters()
        {
            //获得所有UP适配器
            var adapters = NetworkInterface.GetAllNetworkInterfaces();
            return GetNetworkAdapters(adapters, NetworkInterfaceType.Ethernet);
        }

        /// <summary>
        /// 获取 Wireless80211已连接适配器，适配器被禁用则不能获取到
        /// </summary>
        /// <returns></returns>
        public List<NetworkAdapter> GetWirelessAdaptersByUp()
        {
            //获得所有UP适配器
            var adapters = NetworkInterface.GetAllNetworkInterfaces().Where(d => d.OperationalStatus == OperationalStatus.Up);
            return GetNetworkAdapters(adapters, NetworkInterfaceType.Wireless80211);
        }

        /// <summary>
        /// 获取 Wireless80211适配器，适配器被禁用则不能获取到
        /// </summary>
        /// <returns></returns>
        public List<NetworkAdapter> GetWirelessAdapters()
        {
            //获得所有UP适配器
            var adapters = NetworkInterface.GetAllNetworkInterfaces();
            return GetNetworkAdapters(adapters, NetworkInterfaceType.Wireless80211);
        }

        /// <summary>
        /// 获取所有适配器类型，适配器被禁用则不能获取到
        /// </summary>
        /// <returns></returns>
        public List<NetworkAdapter> GetAllNetworkAdapters()
        {
            var adapters = NetworkInterface.GetAllNetworkInterfaces();
            return GetNetworkAdapters(adapters, NetworkInterfaceType.Ethernet, NetworkInterfaceType.Wireless80211);
        }

        /// <summary>
        /// 获取所有UP适配器类型
        /// </summary>
        /// <returns></returns>
        public List<NetworkAdapter> GetAllNetworkAdaptersByUp()
        {
            var adapters = NetworkInterface.GetAllNetworkInterfaces().Where(d => d.OperationalStatus == OperationalStatus.Up);
            return GetNetworkAdapters(adapters, NetworkInterfaceType.Ethernet, NetworkInterfaceType.Wireless80211);
        }

        /// <summary>
        /// 获取所有DOWN适配器类型
        /// </summary>
        /// <returns></returns>
        public List<NetworkAdapter> GetAllNetworkAdaptersDown()
        {
            var adapters = NetworkInterface.GetAllNetworkInterfaces().Where(d => d.OperationalStatus == OperationalStatus.Down);
            return GetNetworkAdapters(adapters, NetworkInterfaceType.Ethernet, NetworkInterfaceType.Wireless80211);
        }

        /// <summary>
        /// 根据适配器ID得到适配器信息
        /// </summary>
        /// <param name="networkInterfaceID"></param>
        /// <returns></returns>
        public NetworkAdapter? GetNeworkAdapterByNetworkInterfaceID(String? networkInterfaceID)
        {
            if (String.IsNullOrEmpty(networkInterfaceID))
            {
                return null;
            }

            IEnumerable<NetworkInterface> adapters = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var adapter in adapters)
            {
                var network = SetNetworkAdapterValue(adapter);
                if (network.NetworkInterfaceID == networkInterfaceID)
                {
                    return network;
                }
            }
            return null;
        }

        /// <summary>
        /// 启用所有适配器
        /// </summary>
        /// <returns></returns>
        public void EnableAllAdapters()
        {
            // ManagementClass wmi = new ManagementClass("Win32_NetworkAdapter");
            // ManagementObjectCollection moc = wmi.GetInstances();
            System.Management.ManagementObjectSearcher moc = new System.Management.ManagementObjectSearcher("Select * from Win32_NetworkAdapter where NetEnabled!=null ");
            foreach (System.Management.ManagementObject mo in moc.Get())
            {
                //if (!(Boolean)mo["NetEnabled"])
                //    continue;
                var capation = mo["Caption"].ToString();
                var descrption = mo["Description"].ToString();
                mo.InvokeMethod("Enable", null);
            }

        }

        /// <summary>
        /// 禁用所有适配器
        /// </summary>
        public void DisableAllAdapters()
        {
            // ManagementClass wmi = new ManagementClass("Win32_NetworkAdapter");
            // ManagementObjectCollection moc = wmi.GetInstances();
            System.Management.ManagementObjectSearcher moc = new System.Management.ManagementObjectSearcher("Select * from Win32_NetworkAdapter where NetEnabled!=null ");
            foreach (System.Management.ManagementObject mo in moc.Get())
            {
                //if ((Boolean)mo["NetEnabled"])
                //    continue;
                var capation = mo["Caption"].ToString();
                var descrption = mo["Description"].ToString();
                mo.InvokeMethod("Disable", null);
            }

        }

        #endregion

        #region private

        /// <summary>
        /// 根据条件获取IP地址集合，
        /// </summary>
        /// <param name="adapters">网络接口地址集合</param>
        /// <param name="adapterTypes">网络连接状态，如,UP,DOWN等</param>
        /// <returns></returns>
        private List<NetworkAdapter> GetNetworkAdapters(IEnumerable<NetworkInterface> adapters, params NetworkInterfaceType[] networkInterfaceTypes)
        {
            _AdapterList = new List<NetworkAdapter>();

            foreach (var adapter in adapters)
            {
                // 排除无线网络、Loopback和Tunnel类型的适配器
                if (/*adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||*/ adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback || adapter.NetworkInterfaceType == NetworkInterfaceType.Tunnel)
                {
                    continue;
                }

                var description = adapter.Description.ToUpperInvariant();

                // 排除描述中包含特定关键词的适配器               
                if (description.Contains("VIRTUAL") ||   // 虚拟网卡
                    description.Contains("LOOPBACK") ||
                    description.Contains("BLUETOOTH"))   // 蓝牙适配器

                {
                    continue;
                }

                // 如果特定适配器类型被过滤，则排除不在列表中的类型
                if (networkInterfaceTypes.Length > 0 && !networkInterfaceTypes.Contains(adapter.NetworkInterfaceType))
                {
                    continue;
                }

                // 如果适配器通过所有过滤条件，则添加到列表中
                var adp = SetNetworkAdapterValue(adapter);
                _AdapterList.Add(adp);
            }
            return _AdapterList;
        }

        /// <summary>
        /// 设置网络适配器信息
        /// </summary>
        /// <param name="adapter"></param>
        /// <returns></returns>
        private NetworkAdapter SetNetworkAdapterValue(NetworkInterface adapter)
        {
            var networkadapter = new NetworkAdapter();
            var ips = adapter.GetIPProperties();
            networkadapter.Description = adapter.Name;
            networkadapter.NetworkInterfaceType = adapter.NetworkInterfaceType.ToString();
            networkadapter.Speed = adapter.Speed / 1000 / 1000 + "MB"; //速度
            networkadapter.MacAddress = adapter.GetPhysicalAddress(); //物理地址集合
            networkadapter.NetworkInterfaceID = adapter.Id;//网络适配器标识符

            networkadapter.GateWayes = ips.GatewayAddresses; //网关地址集合
            networkadapter.IPAddresses = ips.UnicastAddresses; //IP地址集合
            networkadapter.DhcpServerAddresses = ips.DhcpServerAddresses;//DHCP地址集合
            networkadapter.IsDhcpEnabled = ips.GetIPv4Properties() == null ? false : ips.GetIPv4Properties().IsDhcpEnabled; //是否启用DHCP服务

            var adapterproperties = adapter.GetIPProperties();//获取IPInterfaceProperties实例  
            networkadapter.DnsAddresses = adapterproperties.DnsAddresses; //获取并显示DNS服务器IP地址信息 集合
            return networkadapter;
        }
    }
    #endregion
}

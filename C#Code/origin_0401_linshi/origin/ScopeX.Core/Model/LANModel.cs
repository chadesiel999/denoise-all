using ScopeX.ComModel;
using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ScopeX.Core
{
    internal class LANModel : INotifyPropertyChanged
    {
        private Int32 _CurrentSelect = 0;
        public Int32 CurrentSelect
        {
            get => _CurrentSelect;
            set
            {
                if (_CurrentSelect != value)
                {
                    _CurrentSelect = value;
                    OnPropertyChanged(nameof(CurrentSelect));
                }
            }
        }

        private NetworkAdapterType _Type = NetworkAdapterType.EthernetWirelessUsing;
        internal NetworkAdapterType Type
        {
            get => _Type;
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    OnPropertyChanged();
                }
            }
        }

        #region INotifyPropertyChanged
        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    internal class LANItemModel
    {
        public LANItemModel(PropertyChangedEventHandler? propertyChanged)
        {
            _PropertyChanged = propertyChanged;
        }

        private NetworkAdapter? _Adapter;
        /// <summary>
        /// 网络适配器
        /// </summary>
        internal NetworkAdapter? Adapter
        {
            get => _Adapter;
            set
            {
                if (_Adapter != value)
                {
                    _Adapter = value;
                    //OnPropertyChanged(nameof(Adapter));
                }
            }
        }

        internal String? Description => _Adapter?.Description;

        private String? _IPAddress;
        /// <summary>
        /// IP地址
        /// </summary>
        internal String? IPAddress
        {
            get => _IPAddress;
            set
            {
                if (_IPAddress != value && CheckAddressFormat(value))
                {
                    _IPAddress = value;
                    OnPropertyChanged(nameof(IPAddress));
                }
            }
        }

        private String? _SubMask;
        /// <summary>
        /// 子网掩码
        /// </summary>
        internal String? SubMask
        {
            get => _SubMask;
            set
            {
                if (_SubMask != value && CheckAddressFormat(value))
                {
                    _SubMask = value;
                    OnPropertyChanged(nameof(SubMask));
                }
            }
        }

        private String? _GateWay;
        /// <summary>
        /// 网关
        /// 类似系统网络设置，网络设置是不需要提示的，因为可能根本就没有进行物理连接的，或者是局域网络，不需要配置网关和DNS.
        /// </summary>
        internal String? GateWay
        {
            get => _GateWay;
            set
            {
                if (_GateWay != value)
                {
                    _GateWay = value;
                    OnPropertyChanged(nameof(GateWay));
                }
            }
        }

        private String? _DnsMain;
        /// <summary>
        /// 主DNS
        /// </summary>
        internal String? DnsMain
        {
            get => _DnsMain;
            set
            {
                if (_DnsMain != value)
                {
                    _DnsMain = value;
                    OnPropertyChanged(nameof(DnsMain));
                }
            }
        }

        private String? _DnsBackup;
        /// <summary>
        /// 主DNS
        /// </summary>
        internal String? DnsBackup
        {
            get => _DnsBackup;
            set
            {
                if (_DnsBackup != value)
                {
                    _DnsBackup = value;
                    OnPropertyChanged(nameof(DnsBackup));
                }
            }
        }

        private GetIPMethod _DHCP = GetIPMethod.Auto;
        /// <summary>
        /// 用于切换（自动 IP）和（手动 IP） 配置模式
        /// </summary>
        internal GetIPMethod DHCP
        {
            get => _DHCP;
            set
            {
                //if (_DHCP != value)
                {
                    if (value == GetIPMethod.Auto)
                    {
                        var bok = _Adapter?.EnableDHCP(_Adapter?.Description) ?? false;
                        _DHCP = bok ? GetIPMethod.Auto : GetIPMethod.Manual;
                    }
                    else
                    {
                        var bok = _Adapter?.DisableDHCP(_Adapter?.Description) ?? false;
                        _DHCP = bok ? GetIPMethod.Manual : GetIPMethod.Auto;
                    }
                    OnPropertyChanged(nameof(DHCP));
                }
            }
        }

        /// <summary>
        /// MAC物理地址
        /// </summary>
        internal String? Mac => _Adapter?.MacAddres;

        /// <summary>
        /// 网络传输速度
        /// </summary>
        internal String Speed => _Adapter?.Speed ?? "0MB";

        /// <summary>
        /// 适配器真实IP地址信息 需要获取适配器IP地址信息时，请调用此接口
        /// </summary>
        /// <returns></returns>
        internal List<(String PropertyName, String Value)>? GetAdapterConfig()
        {
            if (_Adapter == null)
            {
                return null;
            }

            var info = new List<(String PropertyName, String Value)>
            {
                (nameof(IPAddress), _Adapter.IpAddress),
                (nameof(SubMask), _Adapter.Mask),
                (nameof(GateWay), _Adapter.GateWay),
                (nameof(DnsMain), _Adapter.DnsMain),
                (nameof(DnsBackup), _Adapter.DnsBackup),
                (nameof(DnsBackup), _Adapter.DnsBackup),
                (nameof(Mac), _Adapter.MacAddres),
                (nameof(Speed), Speed),
                (nameof(DHCP), DHCP== GetIPMethod.Auto? "AUTO":"MANual")
            };
            return info;
        }

        /// <summary>
        /// 设置适配器IP地址
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="subnetMask">子网掩码</param>
        /// <param name="gateway">默认网关</param>
        /// <returns></returns>
        public Boolean SetIPAddress(String ipAddress, String subnetMask, String gateway) => _Adapter?.SetIPAddress(ipAddress, subnetMask, gateway) ?? false;


        /// <summary>
        /// 检查是否真实有效的IP地址格式
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="showTips">是否弱提示</param>
        /// <returns></returns>
        private Boolean CheckAddressFormat(String? address, Boolean showTips = true)
        {
            if (String.IsNullOrWhiteSpace(address) || address.Equals("...") || address.Equals("0.0.0.0") || !(_Adapter?.IsIPAddress(address) ?? false))
            {
                if (showTips)
                    WeakTip.Default.Write("NetWork", MsgTipId.SetIPAddressFail);
                return false;
            }
            return true;
        }

        //internal void SettingIPAddress(String? ipAddress, String? subMask, String? gateWay, String? dnsMain, String? dnsBackup)
        //{
        //    // IP地址 和子网掩码是必须设置的，其它的可以根据需要设置。
        //    if (!CheckAddressFormat(ipAddress) || !CheckAddressFormat(subMask))
        //    {
        //        return;
        //    }
        //    var bOK = _Adapter?.SetIPAddressAndSubMask(ipAddress!, subMask!);

        //    //类似系统网络设置，网络设置是不需要提示的，因为可能根本就没有进行物理连接的，或者是局域网络，不需要配置网关和DNS.
        //    if (CheckAddressFormat(gateWay, false))
        //    {
        //        _Adapter?.SetGetWayAddress(gateWay!);
        //    }
        //    else
        //    {
        //        _Adapter?.ClearGetWay();
        //    }

        //    if (CheckAddressFormat(dnsMain, false) && CheckAddressFormat(_DnsMain, false))
        //    {
        //        _Adapter?.SetDNSAddress(dnsMain!, dnsBackup!);
        //    }
        //    else
        //    {
        //        _Adapter?.ClearDNSInfo();
        //    }
        //}

        //internal void SettingIPAddress()
        //{
        //    // IP地址 和子网掩码是必须设置的，其它的可以根据需要设置。
        //    var bOK = _Adapter?.SetIPAddressAndSubMask(_IPAddress!, _SubMask!);

        //    //类似系统网络设置，网络设置是不需要提示的，因为可能根本就没有进行物理连接的，或者是局域网络，不需要配置网关和DNS.
        //    if (CheckAddressFormat(_GateWay, false))
        //    {
        //        _Adapter?.SetGetWayAddress(_GateWay!);
        //    }
        //    else
        //    {
        //        _Adapter?.ClearGetWay();
        //    }

        //    if (CheckAddressFormat(_DnsBackup, false) && CheckAddressFormat(_DnsMain, false))
        //    {
        //        _Adapter?.SetDNSAddress(_DnsMain!, _DnsBackup!);
        //    }
        //    else
        //    {
        //        _Adapter?.ClearDNSInfo();
        //    }
        //}

        private readonly PropertyChangedEventHandler? _PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

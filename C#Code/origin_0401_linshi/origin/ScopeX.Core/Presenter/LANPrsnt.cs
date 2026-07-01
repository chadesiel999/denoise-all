using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScopeX.Core
{
    public class LANPrsnt : MulticastPrsnt<ILANView>, ILANPrsnt
    {
        private readonly NetworkAdapterUtil _Util = new();

        /// <summary>
        /// 以太网网络适配器集合，包含已连接的以太网网络适配器
        /// </summary>
        public List<LANItemPrsnt> EthernetAdapters = new List<LANItemPrsnt>();

        /// <summary>
        /// 已连接的以太网网络适配器集合
        /// </summary>
        public List<LANItemPrsnt> ConnectedEthernetAdapters = new List<LANItemPrsnt>();

        /// <summary>
        /// WiFi网络适配器 一般只有一个
        /// </summary>
        public LANItemPrsnt? WiFiAdapter
        {
            get;
            private set;
        }

        /// <summary>
        /// 所有网络适配器集合，包括以太网和WiFI
        /// </summary>
        public List<LANItemPrsnt> AllAdapters = new List<LANItemPrsnt>();

        /// <summary>
        /// 所有已连接的网络适配器集合，包括以太网和WiFI
        /// </summary>
        public List<LANItemPrsnt> AllUpAdapters = new List<LANItemPrsnt>();

        public Int32 CurrentSelect
        {
            get => Model.CurrentSelect;
            set => Model.CurrentSelect = value;
        }

        private protected override LANModel Model
        {
            get;
        }

        public NetworkAdapterType Type
        {
            get => Model.Type;
            set => Model.Type = value;
        }

        public LANPrsnt(IDsoPrsnt idp, ILANView? view = null) : base(idp)
        {
            Model = DsoModel.Default.LIN;
            Model.PropertyChanged += OnPropertyChanged;
            if (view is not null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
            LoadAdapters();
        }

        /// <summary>
        /// 刷新网络适配器资源列表，在设置完IP地址或者IP地址获取方式后需要手动调用此方法重新加载适配器列表
        /// </summary>
        public void Refesh()
        {
            LoadAdapters();
        }

        private void LoadAdapters()
        {
            LoadWifiAdapters();
            LoadEthernetAdapters();
            LoadConnectedEthernetAdapters();
            LoadAllUpAdapters();
            AllAdapters.Clear();
            if (EthernetAdapters.Count > 0)
            {
                AllAdapters.AddRange(EthernetAdapters);
            }
            if (WiFiAdapter != null)
            {
                AllAdapters.Add(WiFiAdapter);
            }
        }

        private void LoadEthernetAdapters()
        {
            EthernetAdapters.Clear();
            var adapters = _Util.GetEthernetAdapters();
            if (adapters != null && adapters.Count > 0)
            {
                foreach (var network in adapters)
                {
                    var model = new LANItemModel(OnPropertyChanged);
                    model.Adapter = network;
                    EthernetAdapters.Add(new LANItemPrsnt(model));
                }
            }
        }

        private void LoadConnectedEthernetAdapters()
        {
            ConnectedEthernetAdapters.Clear();
            var adapters = _Util.GetEthernetAdaptersUP();
            if (adapters != null && adapters.Count > 0)
            {
                foreach (var network in adapters)
                {
                    var model = new LANItemModel(OnPropertyChanged);
                    model.Adapter = network;
                    ConnectedEthernetAdapters.Add(new LANItemPrsnt(model));
                }
            }
        }

        private void LoadWifiAdapters()
        {
            var adapter = _Util.GetWirelessAdapters().FirstOrDefault();//Wifi适配器一般只有一个
            if (adapter != null)
            {
                var model = new LANItemModel(OnPropertyChanged);
                model.Adapter = adapter;
                WiFiAdapter = new LANItemPrsnt(model);
            }
            else
                WiFiAdapter = null;
        }

        private void LoadAllUpAdapters()
        {
            AllUpAdapters.Clear();
            AllAdapters.AddRange(ConnectedEthernetAdapters);
            var adapters = _Util.GetWirelessAdaptersByUp();
            if (adapters.Count > 0 && WiFiAdapter != null)//表示Wifi已连接 把现有的WiFiAdapter添加进去
            {
                AllUpAdapters.Add(WiFiAdapter);
            }
        }
    }

    public class LANItemPrsnt
    {
        private protected LANItemModel Model { get; }

        internal LANItemPrsnt(LANItemModel model)
        {
            Model = model;
            Timestamp = DateTime.Now.Ticks;
        }

        public Int64 Timestamp
        {
            get;
            init;
        }

        public NetworkAdapter? Adapter
        {
            get => Model.Adapter;
            set => Model.Adapter = value;
        }

        public String? IPAddress
        {
            get => Model.IPAddress;
            set => Model.IPAddress = value;
        }

        public String? SubMask
        {
            get => Model.SubMask;
            set => Model.SubMask = value;
        }

        public String? GateWay
        {
            get => Model.GateWay;
            set => Model.GateWay = value;
        }

        public String? DnsMain
        {
            get => Model.DnsMain;
            set => Model.DnsMain = value;
        }

        public String? DnsBackup
        {
            get => Model.DnsBackup;
            set => Model.DnsBackup = value;
        }

        public GetIPMethod DHCP
        {
            get => Model.DHCP;
            set => Model.DHCP = value;
        }

        public String Speed => Model.Speed;

        public String? Mac => Model.Mac;

        public List<(String PropertyName, String Value)>? GetAdapterConfig() => Model.GetAdapterConfig();

        public Boolean SetIPAddress(String ipAddress, String subnetMask, String gateway) => Model.SetIPAddress(ipAddress, subnetMask, gateway);
    }
}

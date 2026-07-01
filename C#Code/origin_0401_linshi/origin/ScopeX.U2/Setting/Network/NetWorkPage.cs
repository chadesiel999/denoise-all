using ScopeX.ComModel;
using ScopeX.Controls.Common.Default;
using ScopeX.Core;
using ScopeX.Core.Tools;
using ScopeX.UserControls.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScopeX.U2
{
    public partial class NetWorkPage : UserControl, ILANView, IStylize
    {
        [Browsable(false)]
        public StyleFlag StyleFlags { get; set; } = StyleFlag.None;

        [Description("是否风格化"), Browsable(true), DefaultValue(typeof(Boolean)), Category(Const.Category)]
        public Boolean StylizeFlag { get; set; } = false;

        private WebServerStatus WebStatus { get; set; } = WebServerStatus.Close;

        #region 私有字段

        private Boolean _ArgToCtrl;

        private List<LANItemPrsnt> _AdaptersList = new();

        private WebServerStatus _WaittingState = WebServerStatus.Open;
        #endregion

        public NetWorkPage()
        {
            InitializeComponent();
            Task.Run(() =>
            {
                Program.Oscilloscope.LAN.Refesh();
                BindAdapters();
            });
            UpdateWebStatus();
        }

        protected new Boolean DesignMode
        {
            get
            {
                Boolean rtnflag = false;
#if DEBUG
                rtnflag = DesignTimeHelper.InDesignMode(this);
#endif
                return rtnflag;
            }
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        public LANPrsnt Presenter
        {
            get;
            set;
        }

        ILANPrsnt IView<ILANPrsnt>.Presenter
        {
            get => Presenter;
            set => Presenter = (LANPrsnt)value;
        }

        public void UpdateView(object prsnt, string propertyName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Object, String>(Update), new[] { prsnt, propertyName });
            }
            else
            {
                Update(prsnt, propertyName);
            }
        }

        protected void Update(Object prsnt, string propertyName)
        {
            _ArgToCtrl = true;
            if (propertyName == nameof(Presenter.Type))
            {
                BindAdapters();
                UpdateView();
                return;
            }
            if (_AdaptersList.Count <= 0)
            {
                _ArgToCtrl = false;
                return;
            }
            var item = _AdaptersList[Presenter.CurrentSelect];
            switch (propertyName)
            {
                //case nameof(Presenter.Type):
                //BindAdapters();
                //UpdateView();
                //    break;
                case nameof(Presenter.CurrentSelect):
                    CbxNetworkAdapter.SelectedIndex = Presenter.CurrentSelect;
                    UpdateIpAddressView();
                    break;
                case nameof(item.DHCP):
                    if (item.DHCP == GetIPMethod.Manual)
                    {
                        RdoGetIpMethod.ChoosedButtonIndex = (Int32)item.DHCP;
                        UpdateControlState(item.DHCP == GetIPMethod.Manual);
                    }
                    else
                    {
                        //Ip获取方式改变之后需要重新刷新
                        Presenter.Refesh();
                        BindAdapters();
                        UpdateView();
                    }
                    break;
                case nameof(item.IPAddress):
                    TbxIPAdress.Text = item.IPAddress;
                    break;
                case nameof(item.SubMask):
                    TbxIPMask.Text = item.SubMask;
                    break;
                case nameof(item.GateWay):
                    TbxGateway.Text = item.GateWay;
                    break;
                case nameof(item.DnsBackup):
                    TbxDNSBackup.Text = item.DnsBackup;
                    break;
                case nameof(item.DnsMain):
                    TbxDNSMain.Text = item.DnsMain;
                    break;
                default:
                    break;
            }
            _ArgToCtrl = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _WaittingState = (WebServerStatus)Scpi.ScpiManager.IsWebModelRunning().ToInt();
            UpdateView();
            //TcmNetWork.BackColor = Color.Transparent;
            TpgNetWork.BackColor = Color.Transparent;
            TpgUSB.BackColor = Color.Transparent;
            TpgWebServer.BackColor = Color.Transparent;

            LblSerialNum.Text = Program.Oscilloscope.OptionsManager.SerialNumber; //DsoPrsnt.DefaultDsoPrsnt.OptionsManager.ProductInfo != null ? DsoPrsnt.DefaultDsoPrsnt.OptionsManager.ProductInfo.SerialNumber : "";
            RdoGetIpMethod.IndexChanged += (_, _) =>
            {
                if (!_ArgToCtrl)
                {
                    if (_AdaptersList != null)
                    {
                    _AdaptersList[Presenter.CurrentSelect].DHCP = (GetIPMethod)RdoGetIpMethod.ChoosedButtonIndex;

                    }
                }
            };
            Task.Run(() =>
            {
                Program.Oscilloscope.LAN.Refesh();
                BindAdapters();
                try
                {
                    this.Invoke(UpdateView);    

                }
                catch (Exception)
                {

                }
            });
        }

        private void UpdateControlState(Boolean state = false)
        {
            TbxDNSBackup.Enabled = TbxDNSMain.Enabled = TbxGateway.Enabled = TbxIPMask.Enabled = TbxIPAdress.Enabled = state;
        }

        public override void Refresh()
        {
            base.Refresh();
            UpdateView();
        }

        protected void UpdateIpAddressView()
        {
            if (_AdaptersList!=null) 
            {
                if (Presenter.CurrentSelect >= _AdaptersList.Count)
                {
                    Presenter.CurrentSelect = 0;
                }
                var adapter = _AdaptersList[Presenter.CurrentSelect];
                TbxIPAdress.Text = adapter.Adapter.IpAddress;
                TbxGateway.Text = adapter.Adapter.GateWay;
                TbxIPMask.Text = adapter.Adapter.Mask;
                TbxMAC.Text = adapter.Mac;
                TbxDNSMain.Text = adapter.Adapter.DnsMain;
                TbxDNSBackup.Text = adapter.Adapter.DnsBackup;
                RdoGetIpMethod.ChoosedButtonIndex = (Int32)adapter.DHCP;
                UpdateControlState(adapter.DHCP == GetIPMethod.Manual);
            }
            
        }

        protected void UpdateView()
        {
            _ArgToCtrl = true;
            RdoNetWork.ChoosedButtonIndex = (Int32)Presenter.Type;
            if (_AdaptersList.Count == 0)
            {
                CbxNetworkAdapter.DataSource = null;
                TbxIPAdress.Clear();
                TbxGateway.Clear();
                TbxIPMask.Clear();
                TbxMAC.Clear();
                TbxDNSMain.Clear();
                TbxDNSBackup.Clear();
                RdoGetIpMethod.ChoosedButtonIndex = -1;
                CbxNetworkAdapter.DataSource = _AdaptersList;
                _ArgToCtrl = false;
                return;
            }

            var adapters = _AdaptersList.Select(x => x.Adapter).ToList();
            CbxNetworkAdapter.DisplayMember = "Description";
            CbxNetworkAdapter.DataSource = adapters; //重新绑定数据
            CbxNetworkAdapter.DisplayMember = "Description";
            CbxNetworkAdapter.SelectedIndex = Presenter.CurrentSelect;
            UpdateIpAddressView();

            _ArgToCtrl = false;
        }

        /// <summary>
        /// 绑定适配器,根据AdapterStatus绑定
        /// </summary>
        private void BindAdapters()
        {
            if (_AdaptersList != null)
            {
                _AdaptersList = Presenter.Type switch
                {
                    NetworkAdapterType.EthernetWirelessUsing => Presenter.ConnectedEthernetAdapters,
                    NetworkAdapterType.All => Presenter.AllAdapters,
                    _ => Presenter.EthernetAdapters
                };
            }
            
        }

        #region Event

        private void RdoNetWork_IndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.Type = (NetworkAdapterType)RdoNetWork.ChoosedButtonIndex;
            }
        }

        private void CbxNetworkAdapter_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                Presenter.CurrentSelect = CbxNetworkAdapter.SelectedIndex;
            }
        }

        private void BtnSettingIP_Click(Object sender, EventArgs e)
        {
            try
            {
                var adapter = _AdaptersList[Presenter.CurrentSelect];
                var ipAddress = TbxIPAdress.Text;
                var subMask = TbxIPMask.Text;
                var gateWay = TbxGateway.Text;
                var dnsMain = TbxDNSMain.Text;
                var dnsBackup = TbxDNSBackup.Text;
                var bOk = adapter.SetIPAddress(ipAddress, subMask, gateWay);
                if (bOk)
                {
                    Presenter.Refesh();
                    BindAdapters();//重新绑定适配器
                    UpdateView();
                    WeakTip.Default.Write("NetWork", MsgTipId.SetIPSuccess);
                }
                else
                {
                    WeakTip.Default.Write("NetWork", MsgTipId.SetIPAddressFail);
                }
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this,
                             new EventBus.LogEventArgs("Error (Setting IPAddress): " + ex.Message, EventBus.LogLevel.Error));
            }
        }

        private void ChkWebStatus_CheckedChangedEvent(Object sender, EventArgs e)
        {
            if (!_ArgToCtrl)
            {
                _WaittingState = ChkWebStatus.Checked ? WebServerStatus.Open : WebServerStatus.Close;
                ChkWebStatus.Enabled = false;
                webWaittingLabel.Visible = true;
                if (ChkWebStatus.Checked)
                {
                    WebStatus = (WebServerStatus)Scpi.ScpiManager.StartWebModel().ToInt();
                }
                else
                {
                    Scpi.ScpiManager.StopWebModel();
                }
                Task.Run(WorkWebStatus);
            }
        }

        /// <summary>
        /// 更新Web状态
        /// </summary>
        public void UpdateWebStatus()
        {
            WebStatus = (WebServerStatus)Scpi.ScpiManager.IsWebModelRunning().ToInt();
            _ArgToCtrl = true;
            BtnWebStatus.Icon = WebStatus == WebServerStatus.Open ? Properties.Resources.ON : Properties.Resources.OFF;
            ChkWebStatus.Checked = WebStatus == WebServerStatus.Open;
            _ArgToCtrl = false;
            ChkWebStatus.Enabled = true;
            webWaittingLabel.Visible = false;
        }


        private void WorkWebStatus()
        {
            while (true)
            {
                WebStatus = (WebServerStatus)Scpi.ScpiManager.IsWebModelRunning().ToInt();
                if (_WaittingState == WebStatus)
                {
                    if (IsHandleCreated)
                    {
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            UpdateWebStatus();
                        }));
                    }
                    else
                    {
                        UpdateWebStatus();
                    }
                    break;
                }
                Thread.Sleep(1000);
            }
        }

        #endregion
    }

    internal enum WebServerStatus
    {
        Close,
        Open
    }
}

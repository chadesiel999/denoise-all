using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class PCIeTrigSerialPrsnt : TrigSerialPrsnt
    {
        private protected override PCIeTrigSerialModel Model
        {
            get;
        }


        public override String ConditionName => nameof(Condition);
        public PCIeTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.PCIe, view)
        {
            Model = (PCIeTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.PCIe);
            LoadEvent();
        }
        /// <summary>
        /// 重载参数
        /// </summary>
        public override void LoadEvent()
        {
            if (Model != null)
            {
                Model.PropertyChanged += OnPropertyChanged;
            }
        }

        /// <summary>
        /// 切换类型，注销事件
        /// </summary>
        public override void DisposeEvent()
        {
            if (Model != null)
            {
                Model.PropertyChanged -= OnPropertyChanged;
            }
        }
        public ProtocolPCIe.Condition Condition
        {
            get => Model.Condition;
            set => Model.Condition = value.Clamp();
        }
        public ProtocolPCIe.TLPType TLPType
        {
            get => Model.TLPType;
            set => Model.TLPType = value.Clamp();
        }
        public UInt16 MinSeqID => Model.MinSeqID;
        public UInt16 MaxSeqID => Model.MaxSeqID;
        public UInt16 SeqID
        {
            get => Model.SeqID;
            set => Model.SeqID = Math.Clamp(value, MinSeqID, MaxSeqID);
        }

        public Byte MinTCData => Model.MinTCData;
        public Byte MaxTCData => Model.MaxTCData;

        public Byte TCData
        {
            get => Model.TCData;
            set => Model.TCData = Math.Clamp(value, MinTCData, MaxTCData);
        }

        public Byte MinATData => Model.MinATData;
        public Byte MaxATData => Model.MaxATData;

        public Byte ATData
        {
            get => Model.ATData;
            set => Model.ATData = Math.Clamp(value, MinATData, MaxATData);
        }

        public Byte MinTagData => Model.MinTagData;
        public Byte MaxTagData => Model.MaxTagData;

        public Byte TagData
        {
            get => Model.TagData;
            set => Model.TagData = Math.Clamp(value, MinTagData, MaxTagData);
        }
        public UInt16 MinReqIDData => Model.MinReqIDData;
        public UInt16 MaxReqIDData => Model.MaxReqIDData;

        public UInt16 ReqIDData
        {
            get => Model.ReqIDData;
            set => Model.ReqIDData = Math.Clamp(value, MinReqIDData, MaxReqIDData);
        }

        public Byte MinMsgCodeData => Model.MinMsgCodeData;
        public Byte MaxMsgCodeData => Model.MaxMsgCodeData;

        public Byte MsgCodeData
        {
            get => Model.MsgCodeData;
            set => Model.MsgCodeData = Math.Clamp(value, MinMsgCodeData, MaxMsgCodeData);
        }
        public Byte MinDataLenght => Model.MinDataLenght;
        public Byte MaxDataLenght => Model.MaxDataLenght;

        public Byte DataLenght
        {
            get => Model.DataLenght;
            set
            {
                Model.DataLenght = Math.Clamp(value, MinDataLenght, MaxDataLenght);
            }
        }
        public Int64 MinData => Model.MinData;
        public Int64 MaxData => Model.MaxData;
        public Int64 Data
        {
            get => Model.Data;
            set => Model.Data = Math.Clamp(value, MinData, MaxData);
        }
        public Int64 MinAddressData => Model.MinAddressData;
        public Int64 MaxAddressData => Model.MaxAddressData;

        public Int64 AddressData
        {
            get => Model.AddressData;
            set => Model.AddressData = Math.Clamp(value, MinAddressData, MaxAddressData);
        }
        public Byte MinBusNumber => Model.MinBusNumber;
        public Byte MaxBusNumber => Model.MaxBusNumber;

        public Byte BusNumber
        {
            get => Model.BusNumber;
            set => Model.BusNumber = Math.Clamp(value, MinBusNumber, MaxBusNumber);
        }
        public Byte MinDeviceNumber => Model.MinDeviceNumber;
        public Byte MaxDeviceNumber => Model.MaxDeviceNumber;
        public Byte DeviceNumber
        {
            get => Model.DeviceNumber;
            set => Model.DeviceNumber = Math.Clamp(value, MinDeviceNumber, MaxDeviceNumber);
        }
        public Byte MinFunctionNumber => Model.MinFunctionNumber;
        public Byte MaxFunctionNumber => Model.MaxFunctionNumber;
        public Byte FunctionNumber
        {
            get => Model.FunctionNumber;
            set => Model.FunctionNumber = Math.Clamp(value, MinFunctionNumber, MaxFunctionNumber);
        }

        public ProtocolPCIe.DataRelation DataRelation
        {
            get => Model.DataRelation;
            set => Model.DataRelation = value.Clamp();
        }
    }
}

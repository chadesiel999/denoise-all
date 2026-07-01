using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class EthernetTrigSerialPrsnt : TrigSerialPrsnt
    {
        public EthernetTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.Ethernet, view)
        {
            Model = (EthernetTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.Ethernet);
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

        private protected override EthernetTrigSerialModel Model
        {
            get;
        }

        public override String ConditionName => nameof(Condition);
        public ProtocolEthernet.Condition Condition
        {
            get => Model.Condition;
            set => Model.Condition = value.Clamp();
        }

        public Int64 MinData => Model.MinData;
        public Int64 MaxData => (Int64)Model.MaxData;
        public Byte[] Data
        {
            get => Model.Data;
            set => Model.Data = value;
        }

        public Int64 MaxDataByteLength => Model.MaxDataByteLength;
        public Int64 MinDataByteLength => Model.MinDataByteLength;
        public Int32 DataByteLength
        {
            get => Model.DataByteLength;
            set => Model.DataByteLength = value;
        }

        public Int64 MaxDataOffset => Model.MaxDataOffset;
        public Int64 MinDataOffset => Model.MinDataOffset;
        public Int32 DataOffset
        {
            get => Model.DataOffset;
            set => Model.DataOffset = value;
        }

        public Int64 MaxMACAddress => Model.MaxMACAddress;
        public Int64 MinMACAddress => Model.MinMACAddress;
        public Int32 MACByteCount => Model.MACByteCount;
        public Byte[] SrcMAC
        {
            get => Model.SrcMAC;
            set => Model.SrcMAC = value.Take(MACByteCount).ToArray();
        }
        public Byte[] DestMAC
        {
            get => Model.DestMAC;
            set => Model.DestMAC = value.Take(MACByteCount).ToArray();
        }

        public Int64 MaxMACLengthOrType => Model.MaxMACLengthOrType;
        public Int64 MinMACLengthOrType => Model.MinMACLengthOrType;
        public Int32 MACLengthOrTypeByteCount => Model.MACLengthOrTypeByteCount;
        public Byte[] MACLengthOrType
        {
            get => Model.MACLengthOrType;
            set => Model.MACLengthOrType = value.Take(MACLengthOrTypeByteCount).ToArray();
        }

        public Int64 MaxQTagInfo => Model.MaxQTagInfo;
        public Int64 MinQTagInfo => Model.MinQTagInfo;
        public Int32 QTagInfoByteCount => Model.QTagInfoByteCount;
        public Byte[] QTagInfo
        {
            get => Model.QTagInfo;
            set => Model.QTagInfo = value.Take(QTagInfoByteCount).ToArray();
        }
        public ProtocolEthernet.DataRelation Relation
        {
            get => Model.Relation;
            set => Model.Relation = value.Clamp();
        }
    }
}

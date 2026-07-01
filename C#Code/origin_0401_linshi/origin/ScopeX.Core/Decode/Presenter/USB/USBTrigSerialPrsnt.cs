using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class USBTrigSerialPrsnt : TrigSerialPrsnt
    {
        private protected override UsbTrigSerialModel Model
        {
            get;
        }


        public override String ConditionName => nameof(Condition);
        public USBTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.USB, view)
        {
            Model = (UsbTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.USB);
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

        public ProtocolUSB.Condition Condition { get => Model.Condition; set => Model.Condition = value.Clamp(); }

        public ProtocolUSB.DataPackageType DataPackageType
        {
            get => Model.DataPackageType;
            set => Model.DataPackageType = value.Clamp();
        }

        public ProtocolUSB.DataRelation DataRelation
        {
            get => Model.DataRelation;
            set => Model.DataRelation = value.Clamp();
        }

        public ProtocolUSB.ErrorPackageType ErrorPackageType
        {
            get => Model.ErrorPackageType;
            set => Model.ErrorPackageType = value.Clamp();
        }

        public ProtocolUSB.HandshakePackageType HandshakePackageType
        {
            get => Model.HandshakePackageType;
            set => Model.HandshakePackageType = value.Clamp();
        }

        public ProtocolUSB.TokenPackageType TokenPackageType
        {
            get => Model.TokenPackageType;
            set => Model.TokenPackageType = value.Clamp();
        }
        public ProtocolUSB.SpecialPacketType SpecialPacketType
        {
            get => Model.SpecialPacketType;
            set => Model.SpecialPacketType = value.Clamp();
        }
        public UInt16 MaxData => Model.MaxData;
        public UInt16 MinData => Model.MinData;
        public UInt16 Data
        {
            get => Model.Data;
            set => Model.Data = Math.Clamp(value, MinData, MaxData);
        }
    }
}

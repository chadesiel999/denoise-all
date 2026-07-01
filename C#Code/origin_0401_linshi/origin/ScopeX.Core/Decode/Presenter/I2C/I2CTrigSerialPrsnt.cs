using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class I2CTrigSerialPrsnt : TrigSerialPrsnt
    {

        private protected override I2CTrigSerialModel Model
        {
            get;
        }


        public override String ConditionName => nameof(Condition);
        public I2CTrigSerialPrsnt(IDsoPrsnt idp, ITriggerSerialView? view) : base(idp, SerialProtocolType.I2C, view)
        {
            Model = (I2CTrigSerialModel)TriggerSerialShareParameter.Default.GetTriggerSerial(SerialProtocolType.I2C);
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
        /// <summary>
        /// 地址值
        /// 由于地址可能存在<see cref="ProtocolI2C.AddressBitWidth.AddressBitWidth_10BIT"/>和<see cref="ProtocolI2C.AddressBitWidth.AddressBitWidth_7BIT"/>
        /// 因此一个Byte不能保存所有地址
        /// 当<see cref="BitWidth"/>的值为<see cref="ProtocolI2C.AddressBitWidth.AddressBitWidth_7BIT"/>时，本属性的第一位即为地址值
        /// 当<see cref="Condition"/>的值为<see cref="ProtocolI2C.I2CCondition.Address"/>时有效
        /// </summary>
        public UInt16 AddressData
        {
            get => Model.AddressData;
            set
            {
                Model.AddressData = Math.Clamp(value, MinAddressData, MaxAddressData);
            }
        }
        public UInt16 MaxAddressData => Model.MaxAddressData;
        public UInt16 MinAddressData => Model.MinAddressData;

        /// <summary>
        /// 数据位宽
        /// 当<see cref="Condition"/>的值为<see cref="ProtocolI2C.I2CCondition.Address"/>时有效
        /// 本参数在<see cref="I2CDecodeModel"/>中设置后同步到此属性中
        /// </summary>
        public ProtocolI2C.AddrBitWidth BitWidth
        {
            get => Model.BitWidth;
            internal set
            {
                if (value != Model.BitWidth)
                {
                    Model.BitWidth = value;
                    Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                    AddressData = Model.AddressData;
                }

            }
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        public ProtocolI2C.Condition Condition
        {
            get => Model.Condition;
            set
            {
                Model.Condition = value;
                Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
            }
        }
        public Int64 MinData => Model.MinData;
        public Int64 MaxData => Model.MaxData;
        /// <summary>
        /// 数据
        /// 当<see cref="Condition"/>的值为<see cref="ProtocolI2C.I2CCondition.Data"/>时有效
        /// </summary>
        public Int64 Data
        {
            get => Model.Data;
            set => Model.Data = Math.Clamp(value, MinData, MaxData);
        }

        public UInt32 MinByteCount => Model.MinByteCount;
        public UInt32 MaxByteCount => Model.MaxByteCount;
        /// <summary>
        /// 字节数
        /// 当<see cref="Condition"/>的值为<see cref="ProtocolI2C.I2CCondition.Data"/>时有效
        /// </summary>
        public UInt32 DataBytesCount
        {
            get => Model.DataBytesCount;
            set => Model.DataBytesCount = value;
        }

        /// <summary>
        /// 数据方向
        /// 当<see cref="Condition"/>的值为<see cref="ProtocolI2C.I2CCondition.Address"/>时有效
        /// </summary>
        public ProtocolI2C.DataDirection Direction
        {
            get => Model.Direction;
            set => Model.Direction = value.Clamp();
        }
        /// <summary>
        /// 比较方式
        /// </summary>
        public ProtocolI2C.DataRelation Relation
        {
            get => Model.Relation;
            set => Model.Relation = value.Clamp();
        }
    }
}

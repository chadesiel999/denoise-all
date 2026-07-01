using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    /// <summary>
    /// i2c协议触发的基础model
    /// </summary>
    internal sealed class I2CTrigSerialModel : TriggerSerialModel
    {


        private ProtocolI2C.Condition _Condition = ProtocolI2C.Condition.Start;
        /// <summary>
        /// 事件类型
        /// </summary>
        public ProtocolI2C.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }

        private ProtocolI2C.AddrBitWidth _BitWidth;
        /// <summary>
        /// 数据位宽
        /// 当<see cref="Condition"/>的值为<see cref="ProtocolI2C.I2CCondition.Address"/>时有效
        /// </summary>
        public ProtocolI2C.AddrBitWidth BitWidth
        {
            get { return _BitWidth; }
            set { UpdateProperty(ref _BitWidth, value); }
        }

        private ProtocolI2C.DataDirection _Direction;

        /// <summary>
        /// 数据方向
        /// 当<see cref="Condition"/>的值为<see cref="ProtocolI2C.I2CCondition.Address"/>时有效
        /// </summary>
        public ProtocolI2C.DataDirection Direction
        {
            get { return _Direction; }
            set { UpdateProperty(ref _Direction, value); }
        }


        //public UInt16 MaxAddressData => (UInt16)(Math.Pow(2, (BitWidth switch
        //{
        //    ProtocolI2C.AddrBitWidth.AddrBitWidth_7 => 10,
        //    ProtocolI2C.AddrBitWidth.AddrBitWidth_10 => 10,
        //    _ => 7,
        //})) - 1);

        public UInt16 MaxAddressData => 1024;

        public UInt16 MinAddressData => 0;

        private UInt16 _AddressData;
        /// <summary>
        /// 地址值
        /// 由于地址可能存在<see cref="ProtocolI2C.AddressBitWidth.AddressBitWidth_10BIT"/>和<see cref="ProtocolI2C.AddressBitWidth.AddressBitWidth_7BIT"/>
        /// 因此一个Byte不能保存所有地址
        /// 当<see cref="BitWidth"/>的值为<see cref="ProtocolI2C.AddressBitWidth.AddressBitWidth_7BIT"/>时，本属性的第一位即为地址值
        /// 当<see cref="Condition"/>的值为<see cref="ProtocolI2C.I2CCondition.Address"/>时有效
        /// </summary>
        public UInt16 AddressData
        {
            get { return _AddressData; }
            set { UpdateProperty(ref _AddressData, value); }
        }

        private ProtocolI2C.DataRelation _Relation;

        /// <summary>
        /// 比较方式
        /// </summary>
        public ProtocolI2C.DataRelation Relation
        {
            get { return _Relation; }
            set { UpdateProperty(ref _Relation, value); }
        }

        public UInt32 MaxByteCount { get; } = 5;
        public UInt32 MinByteCount { get; } = 1;
        private UInt32 _DataBytesCount = 1;

        /// <summary>
        /// 字节数
        /// 当<see cref="Condition"/>的值为<see cref="ProtocolI2C.I2CCondition.Data"/>时有效
        /// </summary>
        public UInt32 DataBytesCount
        {
            get { return _DataBytesCount; }

           set { UpdateProperty(ref _DataBytesCount, value); }
            
        }


        public Int64 MinData { get; } = 0;
        public Int64 MaxData => (Int64)Math.Pow(2, 8 * _DataBytesCount) - 1;

        private Int64 _Data;
        /// <summary>
        /// 数据
        /// 当<see cref="Condition"/>的值为<see cref="ProtocolI2C.I2CCondition.Data"/>时有效
        /// </summary>
        public Int64 Data
        {
            get { return _Data; }
            set { UpdateProperty(ref _Data, value); }
        }

        public UInt32 MinDataLength => 1;
        public UInt32 MaxDataLength => 5;
        public UInt32 _DataLength = 1;
        /// <summary>
        /// 数据
        /// </summary>
        public UInt32 DataLength
        {
            get { return _DataLength; }
            set { UpdateProperty(ref _DataLength, value); }
        }

        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigI2CConditionsOptions(Condition)
            {
                AddressData = AddressData,
                Data = Data,
                DataBytesCount = DataBytesCount,
                Direction = Direction,
                Relation = Relation,
            };
        }
    }
}

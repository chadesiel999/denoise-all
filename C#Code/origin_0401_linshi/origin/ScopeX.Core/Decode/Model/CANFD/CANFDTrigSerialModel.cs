using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    internal sealed class CANFDTrigSerialModel : TriggerSerialModel
    {

        private ProtocolCANFD.Condition _Condition = ProtocolCANFD.Condition.FrameEnd;
        /// <summary>
        /// 事件类型
        /// </summary>
        public ProtocolCANFD.Condition Condition
        {
            get { return _Condition; }
            set { UpdateProperty(ref _Condition, value); }
        }

        private ProtocolCANFD.Condition _TrigCondition = ProtocolCANFD.Condition.FrameStart;
        public ProtocolCANFD.Condition TrigCondition
        {
            get { return _TrigCondition; }
            set { UpdateProperty(ref _TrigCondition, value); }
        }

        //帧类型(触发条件选择"帧类型"时使用)
        private ProtocolCANFD.FrameType _FrameType = ProtocolCANFD.FrameType.Data;
        public ProtocolCANFD.FrameType FrameType
        {
            get { return _FrameType; }
            set { UpdateProperty(ref _FrameType, value); }
        }

        //ID标准(触发条件选择"ID"或"ID和数据"时使用)
        private ProtocolCANFD.IDStandard _IDStandard = ProtocolCANFD.IDStandard.Standard;
        public ProtocolCANFD.IDStandard IDStandard
        {
            get { return _IDStandard; }
            set { UpdateProperty(ref _IDStandard, value); }
        }
        //ID帧类型(触发条件选择"ID"或"ID和数据"时使用)
        private ProtocolCANFD.IDFrameDirection _IDFrameDirection = ProtocolCANFD.IDFrameDirection.Read;
        public ProtocolCANFD.IDFrameDirection IDFrameDirection
        {
            get { return _IDFrameDirection; }
            set { UpdateProperty(ref _IDFrameDirection, value); }
        }
        public Int32 MinStandardID => 0;
        public Int32 MaxStandardID { get; } = 0X_7FF;//(Int32)Math.Pow(2, 31) - 1;

        //标准ID号(触发条件选择"ID"或"ID和数据"时使用)
        private Int32 _StandardID = 0x70F;//0x29b;
        public Int32 StandardID
        {
            get { return _StandardID; }
            set { UpdateProperty(ref _StandardID, value); }
        }

        public Int32 MinExtendedID => 0;


        public Int32 MaxExtendedID { get; } = 0X_7F_FF_FF_FF;//(Int32)Math.Pow(2, 29) - 1;
        //扩展ID号(触发条件选择"ID"或"ID和数据"时使用)
        private Int32 _ExtendedID = 0x0;//0x14993;
        public Int32 ExtendedID
        {
            get { return _ExtendedID; }
            set { UpdateProperty(ref _ExtendedID, value); }
        }

        public Int32 MaxByteIndex => MaxByteCount - 1;
        public Int32 MinByteIndex => 0;
        public Int32 MinByteCount => 1;


        public Int32 MaxByteCount => 8;
        //字节数(触发条件选择"数据"或"ID和数据"时使用)
        private Int32 _ByteCount = 1;
        public Int32 ByteCount
        {
            get { return _ByteCount; }
            set { UpdateProperty(ref _ByteCount, value); }
        }

        //字节号
        private Int32 _ByteIndex = 1;
        public Int32 ByteIndex
        {
            get { return _ByteIndex; }
            set { UpdateProperty(ref _ByteIndex, value); }
        }

        private Boolean _DataOffsetEnabled =true;

        public Boolean DataOffsetEnabled
        {
            get { return _DataOffsetEnabled; }
            set { UpdateProperty(ref _DataOffsetEnabled, value); }
        }

        public Int32 MinDataOffset => 0;

        public Int32 MaxDataOffset => 63;

        private Int32 _DataOffset =0;

        public Int32 DataOffset
        {
            get { return _DataOffset; }
            set { UpdateProperty(ref _DataOffset, value); }
        }

        public UInt64 MinData => ByteCount < 8 ? 0: 0XFFFF_FFFF_FFFF_FFFF;
        public UInt64 MaxDataMask => 0X7FFF_FFFF_FFFF_FFFF;//(UInt64)(Math.Pow(2, (MaxByteCount * 8)) - 2);
        public UInt64 MaxData => ByteCount < 8 ? MaxDataMask >> ((MaxByteCount - ByteCount) * 8 - 1) : MaxDataMask;


        //数据(触发条件选择"数据"或"ID和数据"时使用)
        private UInt64 _Data = 0x55;
        public UInt64 Data
        {
            get { return _Data; }
            set { UpdateProperty(ref _Data, value); }
        }

        //数据限定(触发条件选择"数据"或"ID和数据"时使用)
        private ProtocolCANFD.DataRelation _DataRelation = ProtocolCANFD.DataRelation.Eq;
        public ProtocolCANFD.DataRelation DataRelation
        {
            get { return _DataRelation; }
            set { UpdateProperty(ref _DataRelation, value); }
        }
        //private ProtocolCANFD.ErrorPacketType _ErrorPacketType = ProtocolCANFD.ErrorPacketType.AckLose;

        //public ProtocolCANFD.ErrorPacketType ErrorPacketType
        //{
        //    get { return _ErrorPacketType; }
        //    set { UpdateProperty(ref _ErrorPacketType, value); }
        //}

        private ProtocolCANFD.ErrorType _ErrorType = ProtocolCANFD.ErrorType.AnyError;

        public ProtocolCANFD.ErrorType ErrorType
        {
            get { return _ErrorType; }
            set { UpdateProperty(ref _ErrorType, value); }
        }

        public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
        {
            return new HdMessage.TrigCANFDConditionsOptions(Condition)
            {
                ByteCount = ByteCount,
                ByteIndex = ByteIndex,
                Data = Data,
                DataOffsetEnabled = DataOffsetEnabled,
                DataOffset = DataOffset,
                DataRelation = DataRelation,
                ExtendedID = ExtendedID,
                FrameType = FrameType,
                IDFrameDirection = IDFrameDirection,
                IDStandard = IDStandard,
                StandardID = StandardID,
                ErrorType = ErrorType,
            };
        }
    }
}

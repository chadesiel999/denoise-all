using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;


namespace ScopeX.Core.Decode;

sealed internal class CANTrigSerialModel : TriggerSerialModel
{

    ProtocolCAN.Condition condition = ProtocolCAN.Condition.FrameStart;
    public ProtocolCAN.Condition Condition
    {

        get => condition;
        set => UpdateProperty(ref condition, value);
    }


    //帧类型(触发条件选择"帧类型"时使用)
    ProtocolCAN.FrameType _FrameType = ProtocolCAN.FrameType.Data;
    public ProtocolCAN.FrameType FrameType
    {
        get => _FrameType;
        set => UpdateProperty(ref _FrameType, value);
    }

    //ID标准(触发条件选择"ID"或"ID和数据"时使用)
    ProtocolCAN.IDStandard _IDStandard = ProtocolCAN.IDStandard.Standard;
    public ProtocolCAN.IDStandard IDStandard
    {
        get => _IDStandard;
        set => UpdateProperty(ref _IDStandard, value);
    }
    //ID帧类型(触发条件选择"ID"或"ID和数据"时使用)
    ProtocolCAN.IDFrameDirection _IDFrameDirection = ProtocolCAN.IDFrameDirection.Read;
    public ProtocolCAN.IDFrameDirection IDFrameDirection
    {
        get => _IDFrameDirection;
        set => UpdateProperty(ref _IDFrameDirection, value);
    }
    public Int32 MinStandardID => 0;


    public Int32 MaxStandardID { get; } = (Int32)Math.Pow(2, 11) - 1;
    //标准ID号(触发条件选择"ID"或"ID和数据"时使用)
    Int32 _StandardID = 0x70F;//0x29b;
    public Int32 StandardID
    {
        get => _StandardID;
        set => UpdateProperty(ref _StandardID, value);
    }
    public Int32 MinExtendedID => 0;


    public Int32 MaxExtendedID { get; } = (Int32)Math.Pow(2, 29) - 1;
    //扩展ID号(触发条件选择"ID"或"ID和数据"时使用)
    Int32 _ExtendedID = 0x1C3FF3F3;//0x14993;
    public Int32 ExtendedID
    {
        get => _ExtendedID;
        set => UpdateProperty(ref _ExtendedID, value);
    }
    //数据限定(触发条件选择"数据"或"ID和数据"时使用)
    ProtocolCAN.DataRelation _DataRelation = ProtocolCAN.DataRelation.Eq;
    public ProtocolCAN.DataRelation DataRelation
    {
        get => _DataRelation;
        set => UpdateProperty(ref _DataRelation, value);
    }
    public Int32 MaxByteIndex => MaxByteCount - 1;


    public Int32 MinByteIndex => 0;
    //字节号(触发条件选择"数据"或"ID和数据"时使用)
    Int32 _ByteIndex = 1;
    public Int32 ByteIndex
    {
        get => _ByteIndex;
        set => UpdateProperty(ref _ByteIndex, value);
    }
    public Int32 MinByteCount = 1;


    public Int32 MaxByteCount = 8;
    //字节数(触发条件选择"数据"或"ID和数据"时使用)
    Int32 _ByteCount = 1;
    public Int32 ByteCount
    {
        get => _ByteCount;
        set => UpdateProperty(ref _ByteCount, value);
    }

    public UInt64 MinData => ByteCount < 8 ? 0 : 0XFFFF_FFFF_FFFF_FFFF;
    public UInt64 MaxDataMask =>  0X7FFF_FFFF_FFFF_FFFF;
    public UInt64 MaxData => ByteCount < 7 ? MaxDataMask >> ((MaxByteCount - ByteCount) * 8 - 1) : MaxDataMask;

    //数据(触发条件选择"数据"或"ID和数据"时使用)
    UInt64 _Data = 0x55;
    public UInt64 Data
    {
        get => _Data;
        set => UpdateProperty(ref _Data, value);
    }

    ProtocolCAN.ErrorPacketType _ErrorPacketType = ProtocolCAN.ErrorPacketType.AckLose;

    public ProtocolCAN.ErrorPacketType ErrorPacketType
    {
        get => _ErrorPacketType;
        set => UpdateProperty(ref _ErrorPacketType, value);
    }

    public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
    {
        return new HdMessage.TrigCANConditionsOptions(Condition)
        {
            ByteCount = ByteCount,
            ByteIndex = ByteIndex,
            Data =Data,
            DataRelation = DataRelation,
            ExtendedID = ExtendedID,
            FrameType = FrameType,
            IDFrameDirection = IDFrameDirection,
            IDStandard = IDStandard,
            StandardID = StandardID,
            ErrorPacketType = ErrorPacketType
        };
    }
}

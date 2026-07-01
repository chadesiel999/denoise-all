using ScopeX.ComModel;
using System;

namespace ScopeX.Core.Decode;

sealed internal class UsbTrigSerialModel : TriggerSerialModel
{
    ProtocolUSB.Condition _Condition = ProtocolUSB.Condition.DataPackage;
    UInt16 _Data;

    ProtocolUSB.DataPackageType _DataPackageType = ProtocolUSB.DataPackageType.Data0;

    ProtocolUSB.DataRelation _DataRelation = ProtocolUSB.DataRelation.Lt;
    ProtocolUSB.ErrorPackageType _ErrorPackageType = ProtocolUSB.ErrorPackageType.PIDCRC;
    ProtocolUSB.HandshakePackageType _HandshakePackageType = ProtocolUSB.HandshakePackageType.ACK;
    ProtocolUSB.SpecialPacketType _SpecialPacketType = ProtocolUSB.SpecialPacketType.Split;

    ProtocolUSB.TokenPackageType _TokenPackageType = ProtocolUSB.TokenPackageType.IN;
    /// <summary>
    ///     事件类型
    /// </summary>
    public ProtocolUSB.Condition Condition
    {
        get => _Condition;
        set => UpdateProperty(ref _Condition, value);
    }
    public ProtocolUSB.DataPackageType DataPackageType
    {
        get => _DataPackageType;
        set => UpdateProperty(ref _DataPackageType, value);
    }
    public ProtocolUSB.DataRelation DataRelation
    {
        get => _DataRelation;
        set => UpdateProperty(ref _DataRelation, value);
    }
    public ProtocolUSB.ErrorPackageType ErrorPackageType
    {
        get => _ErrorPackageType;
        set => UpdateProperty(ref _ErrorPackageType, value);
    }
    public ProtocolUSB.HandshakePackageType HandshakePackageType
    {
        get => _HandshakePackageType;
        set => UpdateProperty(ref _HandshakePackageType, value);
    }
    public ProtocolUSB.TokenPackageType TokenPackageType
    {
        get => _TokenPackageType;
        set => UpdateProperty(ref _TokenPackageType, value);

    }

    public ProtocolUSB.SpecialPacketType SpecialPacketType
    {
        get => _SpecialPacketType;
        set => UpdateProperty(ref _SpecialPacketType, value);
    }

    public UInt16 MaxData => UInt16.MaxValue;
    public UInt16 MinData => UInt16.MinValue;

    public UInt16 Data
    {
        get => _Data;
        set => UpdateProperty(ref _Data, value);
    }
    public override HdMessage.ITrigDecoderConditionsOptions? GetTrigDecoderRecoder()
    {
        return new HdMessage.TrigUSBConditionsOptions(Condition)
        {
            Data = Data,
            DataPackageType = DataPackageType,
            DataRelation = DataRelation,
            ErrorPackageType = ErrorPackageType,
            HandshakePackageType = HandshakePackageType,
            SpecialPacketType = SpecialPacketType,
            TokenPackageType = TokenPackageType
        };
    }
}

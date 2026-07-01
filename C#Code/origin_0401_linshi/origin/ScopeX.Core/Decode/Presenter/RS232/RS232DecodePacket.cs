using System;
using System.Drawing;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public abstract class Rs232DecodePacket : BaseDecodePacket
    {
        public Rs232DecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public abstract RS232DecodePacketType PacketType { get; }

        public override SerialProtocolType ProtocolType => SerialProtocolType.RS232;

    }
}

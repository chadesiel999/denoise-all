using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDEOFDecodePacket : CANFDDecodePacket
    {
        public CANFDEOFDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override UInt32 BitCount => 7;
        public override Boolean IsInfoPacket => false;
        public override String Title => "EOF";
        public override Color BorderColor => Color.Red;
        public override CANFDPacketType PacketType => CANFDPacketType.EOF;
    }
}

using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDDLCDecodePacket : CANFDDecodePacket
    {
        public CANFDDLCDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Boolean IsInfoPacket => false;
        public override CANFDPacketType PacketType => CANFDPacketType.DLC;
        public override UInt32 BitCount => 4;
        public override String Title => "Data Length Code";
        public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
    }
}

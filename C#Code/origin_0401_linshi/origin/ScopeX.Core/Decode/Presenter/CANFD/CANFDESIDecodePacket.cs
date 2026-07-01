using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDESIDecodePacket : CANFDDecodePacket
    {
        public CANFDESIDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Byte[] Data => new Byte[] { Convert.ToByte(Status) };
        public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
        public override Boolean IsInfoPacket => false;
        public override UInt32 BitCount => 1;
        public override String Title => "ESI";
        public override CANFDPacketType PacketType => CANFDPacketType.ESI;
        public Boolean Status { get; set; }
    }
}

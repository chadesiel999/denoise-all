using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDStuffCountDecodePacket : CANFDDecodePacket
    {
        public CANFDStuffCountDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Byte[] Data => new Byte[] { StuffCount };
        public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
        public override Boolean IsInfoPacket => false;
        public override UInt32 BitCount => 3;
        public override String Title => "Stuff Count";
        public override CANFDPacketType PacketType => CANFDPacketType.StuffCount;
        public Byte StuffCount { get; set; }
    }
}

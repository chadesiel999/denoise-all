using System;
using System.Drawing;
using System.Text;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDSRRDecodePacket : CANFDDecodePacket
    {
        public CANFDSRRDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("SRR");
        public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
        public override Boolean IsInfoPacket => true;
        public override CANFDPacketType PacketType => CANFDPacketType.SRR;
    }
}

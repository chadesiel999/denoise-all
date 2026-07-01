using System;
using System.Drawing;
using System.Text;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDRRSDecodePacket : CANFDDecodePacket
    {
        public CANFDRRSDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("RRS");
        public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
        public override Boolean IsInfoPacket => true;
        public override CANFDPacketType PacketType => CANFDPacketType.RRS;
    }
}

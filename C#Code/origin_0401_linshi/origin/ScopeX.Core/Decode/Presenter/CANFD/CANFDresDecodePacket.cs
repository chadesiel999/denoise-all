using System;
using System.Drawing;
using System.Text;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDresDecodePacket : CANFDDecodePacket
    {
        public CANFDresDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("res");
        public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
        public override Boolean IsInfoPacket => true;
        public override CANFDPacketType PacketType => CANFDPacketType.res;
    }
}

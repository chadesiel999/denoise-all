using System;
using System.Drawing;
using System.Text;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDIDEDecodePacket : CANFDDecodePacket
    {
        public CANFDIDEDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Byte[] Data => new Byte[] { Convert.ToByte(Value) };
        public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
        public override Boolean IsInfoPacket => false;
        public override UInt32 BitCount => 1;
        public override String Title => "IDE";
        public override CANFDPacketType PacketType => CANFDPacketType.IDE;
        public Boolean Value { get; set; }
    }
}

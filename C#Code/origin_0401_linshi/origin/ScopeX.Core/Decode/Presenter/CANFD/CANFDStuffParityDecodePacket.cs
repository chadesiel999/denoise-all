using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDStuffParityDecodePacket : CANFDDecodePacket
    {
        public CANFDStuffParityDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Byte[] Data => new Byte[] { Convert.ToByte(Status) };
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xff400080") : Color.Red;
        public override Boolean IsInfoPacket => false;
        public override UInt32 BitCount => 1;
        public override String Title => "Parity";
        public override CANFDPacketType PacketType => CANFDPacketType.StuffParity;
        public override String ErrorInfo => Success ? "" : "Parity Error";
        public Boolean Status { get; set; }
        public Boolean SuccessStatus { get; set; }
        public Boolean Success => SuccessStatus == Status;
    }
}

using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public class Rs232ParityDecodePacket : Rs232DecodePacket
    {
        public Rs232ParityDecodePacket(Single start, Single lenght) : base(start, lenght)
        {

        }
        public override Boolean IsInfoPacket { get; } = false;
        public override RS232DecodePacketType PacketType => RS232DecodePacketType.Parity;
        public override UInt32 BitCount => 1;
        public override String Title => "Parity";
        public Boolean ParityBit { get; init; }
        public Boolean SuccessParityBit { get; init; }
        public Boolean Success => ParityBit == SuccessParityBit;
        public override Color BorderColor { get => Success ? ColorTranslator.FromHtml("#0xff400080") : Color.Red; }
        public override Byte[] Data => new Byte[1] { Convert.ToByte(ParityBit) };
        public override String ErrorInfo => Success ? "" : "Parity Error";
    }
}

using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public sealed class ARINC429ParityDeocdePacket : ARINC429DecodePacket
    {
        public ARINC429ParityDeocdePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override UInt32 BitCount { get; init; } = 1;
        public override Boolean IsInfoPacket => false;
        public override String ErrorInfo => Error ? "Parity Error" : String.Empty;
        public override Color BorderColor => Error ? Color.Red : ColorTranslator.FromHtml("#FF400080");
        public override Byte[] Data => new Byte[] { Convert.ToByte(Parity) };
        public override ARINC429PacketType PacketType => ARINC429PacketType.Parity;
        public override String Title { get; init; } = "Parity";
        public Boolean Parity { get; init; }
        public Boolean SuccessParity { get; init; }
        public Boolean Error => Parity != SuccessParity;
    }
}

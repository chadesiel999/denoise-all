using System;
using System.Drawing;
using System.Text;

namespace ScopeX.Core.Decode
{
    public sealed class ARINC429GapDecodePacket : ARINC429DecodePacket
    {
        public ARINC429GapDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public override ARINC429PacketType PacketType => ARINC429PacketType.Gap;
        public override Color BorderColor => Color.Red;
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("Gap Error");

        public override Boolean IsInfoPacket => true;
    }
}

using System;
using System.Drawing;
using System.Text;

namespace ScopeX.Core.Decode
{
    public sealed class ARINC429EOFDecodePacket : ARINC429DecodePacket
    {
        public ARINC429EOFDecodePacket(Single start) : base(start, 1)
        {
        }
        public override Boolean IsInfoPacket => true;
        public override Color BorderColor { get; init; } = Color.Red;
        public override ARINC429PacketType PacketType => ARINC429PacketType.EOF;
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("EOF");
    }
}

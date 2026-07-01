using System;
using System.Drawing;
using System.Text;

namespace ScopeX.Core.Decode
{
    public sealed class ARINC429SOFDecodePacket : ARINC429DecodePacket
    {
        public ARINC429SOFDecodePacket(Single start) : base(start, 1)
        {
        }
        public Boolean Success = true;
        public override Color BorderColor => Success ? Color.Green : Color.Red;
        public override Boolean IsInfoPacket => true;
        public override ARINC429PacketType PacketType => ARINC429PacketType.SOF;
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("SOF");
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public class RS232StartDecodePacket : Rs232DecodePacket
    {
        public RS232StartDecodePacket(Single start, Single lenght) : base(start, lenght)
        {

        }
        public override Boolean IsInfoPacket { get; } = true;
        public override RS232DecodePacketType PacketType => RS232DecodePacketType.Start;
        public override Color BorderColor => Color.Green;
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("Start");
    }
}

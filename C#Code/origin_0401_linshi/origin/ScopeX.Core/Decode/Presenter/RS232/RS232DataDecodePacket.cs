using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public class RS232DataDecodePacket : Rs232DecodePacket
    {
        public RS232DataDecodePacket(Single start, Single lenght) : base(start, lenght)
        {

        }
        public override Boolean IsInfoPacket { get; } = false;
        public override RS232DecodePacketType PacketType => RS232DecodePacketType.Data;
        public override String Title { get; init; } = "Data";
        public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xff007f7f");
    }
}

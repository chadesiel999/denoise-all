using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public sealed class ARINC429DataDecodePacket : ARINC429DecodePacket
    {
        public ARINC429DataDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public Boolean Success = true;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#FF008080") : Color.Red;
        public override Boolean IsInfoPacket => false;
        public override ARINC429PacketType PacketType => ARINC429PacketType.Data;
        public override String Title { get; init; } = "Data";
        public override String ErrorInfo => Success ? "" : "Data Error";
    }
}

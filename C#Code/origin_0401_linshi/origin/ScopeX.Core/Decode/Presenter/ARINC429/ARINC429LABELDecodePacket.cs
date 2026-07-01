using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public sealed class ARINC429LABELDecodePacket : ARINC429DecodePacket
    {
        public ARINC429LABELDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public Boolean Success = true;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#FF808000"):Color.Red;
        public override Boolean IsInfoPacket => false;
        public override ARINC429PacketType PacketType => ARINC429PacketType.LABEL;
        public override String Title { get; init; } = "Label";
        public override String ErrorInfo => Success ? "" : "Label Error";
    }
}

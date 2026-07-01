using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public sealed class ARINC429SDIDecodePacket : ARINC429DecodePacket
    {
        public ARINC429SDIDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public Boolean Success = true;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#FF808000") : Color.Red;
        public override ARINC429PacketType PacketType => ARINC429PacketType.SDI;
        public override Boolean IsInfoPacket => false;
        public override UInt32 BitCount { get; init; } = 2;
        public override String Title { get; init; } = "SDI";
        public override String ErrorInfo => Success ? "" : "SDI Error";

    }
}

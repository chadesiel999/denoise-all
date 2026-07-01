using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public sealed class ARINC429SSMDecodePacket : ARINC429DecodePacket
    {
        public ARINC429SSMDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public Boolean Success = true;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#FF400080") : Color.Red;
        public override UInt32 BitCount { get; init; } = 2;
        public override Boolean IsInfoPacket => false;
        public override ARINC429PacketType PacketType => ARINC429PacketType.SSM;
        public override String Title { get; init; } = "SSM";
        public override String ErrorInfo => Success ? "" : "SSM Error";
    }
}

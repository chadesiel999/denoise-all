using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public class NRZDataDecodePacket : NRZDecodePacket
    {
        public NRZDataDecodePacket(Single start, Single lenght) : base(start, lenght)
        {

        }
        public override Boolean IsInfoPacket { get; } = false;
       
        public override String Title { get; init; } = "Data";
        public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xff007f7f");
    }
}

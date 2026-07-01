using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDStandardIDDecodePacket : CANFDDecodePacket
    {
        public CANFDStandardIDDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public override CANFDPacketType PacketType => CANFDPacketType.StandardID;

        public override Boolean IsInfoPacket => false;
        public override UInt32 BitCount => 11;
        public override String Title => "Standard ID";
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xFF808000") : Color.Red;
        public override String ErrorInfo => Success ? "" : "Stuf Error";
        public Boolean Success = true;
    }
}

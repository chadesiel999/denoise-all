using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDDataDecodePacket : CANFDDecodePacket
    {
        public CANFDDataDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override CANFDPacketType PacketType => CANFDPacketType.Data;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xff007f7f") : Color.Red;
        public override UInt32 BitCount => 8;
        public override Boolean IsInfoPacket => false;
        public override String Title => "Data";
        public override String ErrorInfo => Success ? "" : "Stuf Error";
        public Boolean Success = true;
    }
}

using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDExtandIDDecodePacket : CANFDDecodePacket
    {
        public CANFDExtandIDDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override CANFDPacketType PacketType => CANFDPacketType.ExtandID;
        public override Boolean IsInfoPacket => false;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xFF808000") : Color.Red;
        public override UInt32 BitCount => 29;
        public override String Title => "Extand ID";
        public override String ErrorInfo => Success ? "" : "Stuf Error";
        public Boolean Success = true;
    }
}

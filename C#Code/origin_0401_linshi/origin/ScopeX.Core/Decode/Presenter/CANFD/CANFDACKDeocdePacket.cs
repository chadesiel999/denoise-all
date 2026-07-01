using System;
using System.Drawing;
using System.Text;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDACKDeocdePacket : CANFDDecodePacket
    {
        public CANFDACKDeocdePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public Boolean Success { get; set; }

        public override UInt32 BitCount => 1;
        public override Color BorderColor => !Success ? ColorTranslator.FromHtml("#0xff400080") : Color.Red;
        public override CANFDPacketType PacketType => CANFDPacketType.ACK;

        public override Boolean IsInfoPacket => false;
        //public override Byte[] Data;// => Encoding.Default.GetBytes("ACK");
        public override String ErrorInfo => !Success ? "" : "ACK Error";
        public override String Title => "ACK";

    }
}

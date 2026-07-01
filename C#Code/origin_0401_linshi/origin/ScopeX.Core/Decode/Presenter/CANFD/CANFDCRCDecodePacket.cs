using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDCRCDecodePacket : CANFDDecodePacket
    {
        public CANFDCRCDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public Byte[] SuccessCRC { get; init; } = new Byte[0];
        public Boolean Success
        {
            get
            {
                if (Data == null || SuccessCRC == null || Data.Length != SuccessCRC.Length || Data.Length != Math.Ceiling(BitCount / 8.0)) return false;
                for (Int32 i = 0; i < Data.Length; i++)
                {
                    if (Data[i] != SuccessCRC[i]) return false;
                }
                return true;
            }
        }

        public override UInt32 BitCount { get; init; }
        public override Boolean IsInfoPacket => false;
        public override String Title => "CRC";
        public override CANFDPacketType PacketType => CANFDPacketType.CRC;

        public override Color BorderColor { get => Success ? ColorTranslator.FromHtml("#0xff400080") : Color.Red; }
        public override String ErrorInfo => Success ? "" : "CRC Calculated:"+ BitConverter.ToString(SuccessCRC).Replace("-", ""); 
    }
}

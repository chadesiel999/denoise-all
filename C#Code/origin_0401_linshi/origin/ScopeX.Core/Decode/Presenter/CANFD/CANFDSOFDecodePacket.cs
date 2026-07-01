using System;
using System.Drawing;
using System.Text;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDSOFDecodePacket : CANFDDecodePacket
    {
        public CANFDSOFDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Boolean IsInfoPacket => true;
        public override CANFDPacketType PacketType => CANFDPacketType.SOF;
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("SOF");
        public override Color BorderColor => Color.Green;
    }
}

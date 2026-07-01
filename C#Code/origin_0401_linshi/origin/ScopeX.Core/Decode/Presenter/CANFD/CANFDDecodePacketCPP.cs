using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public sealed class CANFDErrorFrameDecodePacketcCPP : CANFDDecodePacket
    {
        public CANFDErrorFrameDecodePacketcCPP(Single start, Single lenght) : base(start, lenght)
        {
        }
        //public override UInt32 BitCount => 12;// (UInt32)Encoding.Default.GetBytes("Error Frame").Length;
        public override Color BorderColor => Color.Red;
        public override CANFDPacketType PacketType => CANFDPacketType.Data;

        public override Boolean IsInfoPacket => true;
        // public override String Title => "Error Frame";

        public override byte[] Data => Encoding.Default.GetBytes("Error Frame");
    }

    public sealed class CANFDOverLoadFrameDeocdePacketCPP : CANFDDecodePacket
    {
        public CANFDOverLoadFrameDeocdePacketCPP(Single start, Single lenght) : base(start, lenght)
        {
        }
        //public override UInt32 BitCount => 14;// (UInt32)Encoding.Default.GetBytes("Over Load Frame").Length;
        public override Color BorderColor => Color.Red;
        public override CANFDPacketType PacketType => CANFDPacketType.Data;

        public override Boolean IsInfoPacket => true;
        //public override String Title => "Over Load Frame";
        public override byte[] Data => Encoding.Default.GetBytes("Over Load Frame");
    }
}

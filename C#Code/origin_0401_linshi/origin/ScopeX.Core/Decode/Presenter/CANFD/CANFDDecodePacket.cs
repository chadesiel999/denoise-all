using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public abstract class CANFDDecodePacket : BaseDecodePacket
    {
        public CANFDDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public abstract CANFDPacketType PacketType { get; }
        public override SerialProtocolType ProtocolType => SerialProtocolType.CAN;
    }
}

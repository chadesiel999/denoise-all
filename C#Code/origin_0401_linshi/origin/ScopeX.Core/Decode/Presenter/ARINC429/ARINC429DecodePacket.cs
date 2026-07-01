using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public abstract class ARINC429DecodePacket : BaseDecodePacket
    {
        public ARINC429DecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SerialProtocolType ProtocolType => SerialProtocolType.ARINC429;
        public abstract ARINC429PacketType PacketType { get; }
    }
}

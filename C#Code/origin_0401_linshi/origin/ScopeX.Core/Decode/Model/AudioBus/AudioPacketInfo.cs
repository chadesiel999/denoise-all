using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    internal struct AudioPacketInfo
    {
        public AudioChannelPacket[] Channels;
    }
    internal struct AudioChannelPacket
    {
        public Boolean HasData;
        public Int32 Index;
        public Int32 BitCount;
        public Int32 SuccessBitCount;
        public Int32 ClkBitCount;
        public Int32 SuccessClkBitCount;
        public Int32 Length;
        public Byte[] Value;
    }
}

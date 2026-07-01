using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public enum CANFDPacketType
    {
        SOF,
        StandardID,
        SRR,
        IDE,
        ExtandID,
        RRS,
        FDF,
        res,
        BRS,
        ESI,
        DLC,
        Data,
        StuffCount,
        StuffParity,
        CRC,
        EOF,
        ACK,
    }
}

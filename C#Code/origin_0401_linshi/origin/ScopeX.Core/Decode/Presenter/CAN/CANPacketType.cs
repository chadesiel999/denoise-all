using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode;

public enum CANPacketType
{
    SOF,
    StandardID,
    SRR,
    IDE,
    ExtandID,
    RTR,
    R0,
    R1,
    DLC,
    Data,
    CRC,
    EOF,
    ACK,
    FrameError,
    PadBitError
}

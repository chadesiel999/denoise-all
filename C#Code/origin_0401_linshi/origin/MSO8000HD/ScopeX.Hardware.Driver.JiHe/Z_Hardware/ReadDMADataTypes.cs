using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal enum DMAReadDataTypes
    {
        AnalogChannelDdr=0b0000_0001,
        ScanFifo=0b0000_0010,
        Dpx=0b0000_0100,
        Decoder=0x05,
        LADdr=0b0001_0000,
        MeasureHist = 0b0010_0000,
    }
}

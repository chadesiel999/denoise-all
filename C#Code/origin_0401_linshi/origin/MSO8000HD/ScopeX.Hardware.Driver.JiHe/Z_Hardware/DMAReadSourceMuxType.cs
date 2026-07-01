using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal enum DMAReadSourceMuxType
    {
        AnalogChanneData=0,
        Dpx=1,
        Decoder=2,
        FreqDomain = 3,
        DDRFast=4,
        LA=5,
        LADDRFast = 7
    }
}

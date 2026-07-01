using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    class AnalogChannelDacPortInfo
    {
        public AnalogChannelDacPortInfo(short _dacIndex, short _portIndex)
        {
            dacIndex = _dacIndex;
            portIndex = _portIndex;
        }
        public short dacIndex;
        public short portIndex;
    }
}

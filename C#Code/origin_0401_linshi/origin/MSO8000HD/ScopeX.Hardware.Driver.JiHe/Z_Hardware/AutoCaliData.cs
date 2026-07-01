using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal class AutoCaliData
    {
        private Dictionary<AcqBdNo, int> _AcqBdProcBdLoopDelay = new Dictionary<AcqBdNo, int>();
        public Dictionary<AcqBdNo, int> AcqBdProcBdLoopDelay => _AcqBdProcBdLoopDelay;
    }
}

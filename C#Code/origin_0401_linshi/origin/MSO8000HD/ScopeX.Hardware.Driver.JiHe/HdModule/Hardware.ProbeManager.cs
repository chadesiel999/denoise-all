using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    public static partial class Hd
    {
        public static ProbeManager? ProbeManager { get => ProbeManager.Default; }
    }
}

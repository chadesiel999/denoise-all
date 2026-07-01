using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    [Obsolete]
    [Serializable]
    public class SoftwareVersionInfo
    {
        public String MaxSoftVersion { set; get; } = "9.9.9";
        public String MinSoftVersion { set; get; } = "0.0.0";
    }
}

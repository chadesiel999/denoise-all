using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class Command_Cymometer : IAppendCommandTable
    {
        public Dictionary<HdCmd, Action[]> AppendCommand()
        {
            return new Dictionary<HdCmd, Action[]>()
            {
#if Cymometer
            [HdCmd.CymometerSrc] = new Action[] { AbstractAcquirer_Cymometer.Config},
#endif
            };
        }
    }
}

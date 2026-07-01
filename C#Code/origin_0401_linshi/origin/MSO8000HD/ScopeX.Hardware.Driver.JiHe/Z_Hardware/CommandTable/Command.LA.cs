using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class Command_LA : IAppendCommandTable
    {
        public Dictionary<HdCmd, Action[]> AppendCommand()
        {
            return new Dictionary<HdCmd, Action[]>()
            {
                #region LA
                [HdCmd.Digital] = new Action[] { AbstractAcquirer_LA.Config , AbstractController_Misc.AnalogChannelActiveChanged },
                #endregion
            };
        }
    }
}

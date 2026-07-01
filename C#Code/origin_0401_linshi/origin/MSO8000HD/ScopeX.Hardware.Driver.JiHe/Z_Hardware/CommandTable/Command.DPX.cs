using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class Command_DPX: IAppendCommandTable
    {
        public Dictionary<HdCmd, Action[]> AppendCommand()
        {
            return new Dictionary<HdCmd, Action[]>()
            {
                #region DPX
                [HdCmd.DpxVectorized] = new Action[] { AbstractController_Misc.DPX_Config },
                [HdCmd.DpxColorStep] = new Action[] { AbstractController_Misc.DPX_Config },
                [HdCmd.DpxEnabled] = new Action[] { AbstractController_Misc.DPX_Config },
                #endregion
            };
        }
    }
}

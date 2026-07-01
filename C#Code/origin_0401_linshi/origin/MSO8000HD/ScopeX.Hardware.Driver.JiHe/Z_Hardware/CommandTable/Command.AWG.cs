using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class Command_AWG : IAppendCommandTable
    {
        public Dictionary<HdCmd, Action[]> AppendCommand()
        {
            return new Dictionary<HdCmd, Action[]>()
            {
                #region AWG
                [HdCmd.AWGConfig] = new Action[] { AWG.Config },
                [HdCmd.AWGData] = new Action[] { AWG.SendWaveSamplingData },
                [HdCmd.AWGTrigger1] = new Action[] { AWG.SendWaveTrigger1 },
                [HdCmd.AWGTrigger2] = new Action[] { AWG.SendWaveTrigger2 },
				[HdCmd.AWGTrigger3] = new Action[] { AWG.SendWaveTrigger3 },
				[HdCmd.AWGTrigger4] = new Action[] { AWG.SendWaveTrigger4 },
				#endregion
			};
        }
    }
}

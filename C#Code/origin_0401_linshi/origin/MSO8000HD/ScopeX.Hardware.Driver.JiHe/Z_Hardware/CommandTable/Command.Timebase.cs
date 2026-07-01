using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class Command_Timebase: IAppendCommandTable
    {
        public Dictionary<HdCmd, Action[]> AppendCommand()
        {
            return new Dictionary<HdCmd, Action[]>()
            {
                #region 时基
                [HdCmd.TmbScaleIndex] = new Action[]
                {
                    AbstractController_Misc.ConfigExtractProcessRoadParameters,
                    AbstractController_Trigger.ConfigFifoStageDepth_WithAcqLength,
                    AbstractController_Trigger.ConfigDgtParameter,
                    AbstractController_Trigger.ConfigTriggerSource,
                    AbstractController_Trigger.ConfigTypeAndParameter//,
                    //,
                    //AbstractController_Misc.ConfigDspCoefficients ///cij
                },
                [HdCmd.TmbStorageLen] = new Action[] { AbstractController_Misc.ConfigExtractProcessRoadParameters, AbstractController_Trigger.ConfigFifoStageDepth_WithAcqLength, AbstractController_Trigger.ConfigDgtParameter, AbstractController_Misc.ConfigLongStorage },
                [HdCmd.TmbLongStorage] = new Action[] { AbstractController_Misc.ConfigLongStorage },
                [HdCmd.TmbInterpolateMode] = new Action[] { AbstractController_Misc.ConfigInterpolateMode},
                #endregion
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class Command_Trigger : IAppendCommandTable
    {
        public Dictionary<HdCmd, Action[]> AppendCommand()
        {
            return new Dictionary<HdCmd, Action[]>()
            {
                #region 触发
                [HdCmd.TrigMode] = new Action[] { AbstractController_Trigger.ConfigTriggerMode,AbstractController_Misc.ConfigExtractProcessRoadParameters },
                [HdCmd.TrigTypeAndParameters] = new Action[] { AbstractController_Trigger.ConfigTypeAndParameter, AbstractController_Trigger.ConfigTriggerSource, AbstractController_Trigger.ConfigDgtParameter, AbstractController_Trigger.ConfigFifoStageDepth_WithAcqLength, AbstractController_AnalogChannel.CtrlExtTrig ,AbstractController_Decoder.DisableDecode},
                [HdCmd.TrigSource] = new Action[] { AbstractController_Trigger.ConfigTriggerSource, AbstractController_Trigger.ConfigDgtParameter, AbstractController_Trigger.ConfigFifoStageDepth_WithAcqLength, AbstractController_AnalogChannel.CtrlExtTrig },
                [HdCmd.TrigCoupling] = new Action[] { AbstractController_Trigger.ConfigTypeAndParameter, AbstractController_AnalogChannel.CtrlExtTrig },
                [HdCmd.TrigHoldoff] = new Action[] { AbstractController_Trigger.ConfigHoldOff },
                [HdCmd.TmbPosition] = new Action[] { AbstractController_Trigger.ConfigFifoStageDepth_WithAcqLength },
                [HdCmd.TrigPosition] = new Action[] { AbstractController_Trigger.ConfigDgtParameter, AbstractController_Trigger.ConfigTypeAndParameter, AbstractController_AnalogChannel.CtrlExtTrig },
                [HdCmd.TrigSensitivity] = new Action[] { AbstractController_AnalogChannel.CtrlExtTrig },
                #endregion
            };
        }
    }
}

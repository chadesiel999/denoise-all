using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class Command_Analog : IAppendCommandTable
    {
        public Dictionary<HdCmd, Action[]> AppendCommand()
        {
            return new Dictionary<HdCmd, Action[]>()
            {
                #region 模拟通道
                [HdCmd.ChnlActive] = new Action[] {
                    AbstractController_Misc.AnalogChannelActiveChanged,
                    AbstractController_Misc.ConfigExtractProcessRoadParameters, 
                    AbstractController_Trigger.ConfigFifoStageDepth_WithAcqLength, 
                    AbstractController_AnalogChannel.Send_IFCCoefficientsToAcqBoardByRegisterMode,
                    AbstractController_AnalogChannel.CtrlAnalogChannelSet,
                    AbstractController_Trigger.ConfigTriggerSource,/*, AbstractController_Misc.ConfigLongStorage*/ },

                //[HdCmd.ChnlPosition] = new Action[] { AbstractController_AnalogChannel.CtrlOffset, AbstractController_Trigger.ConfigDgtParameter, AbstractController_Trigger.ConfigTypeAndParameter },
                //[HdCmd.ChnlGain] = new Action[] { AbstractController_AnalogChannel.CtrlGain },
                //[HdCmd.ChnlInverted] = new Action[] { AbstractController_AnalogChannel.Ctrl4094, AbstractController_AnalogChannel.CtrlOffset, AbstractController_Trigger.ConfigDgtParameter, AbstractController_Trigger.ConfigTypeAndParameter },
                //[HdCmd.ChnlBias] = new Action[] { AbstractController_AnalogChannel.CtrlBias, AbstractController_Trigger.ConfigDgtParameter },
                //[HdCmd.ChnlScaleIndex] = new Action[] { AbstractController_Misc.AnalogChannelActiveChanged, AbstractController_AnalogChannel.Ctrl4094, AbstractController_AnalogChannel.CtrlOffset, AbstractController_AnalogChannel.CtrlGain, AbstractController_Trigger.ConfigFifoStageDepth_WithAcqLength, AbstractController_Misc.ConfigExtractProcessRoadParameters },

                //[HdCmd.ChnlBandwidth] = new Action[] { AbstractController_AnalogChannel.Ctrl4094 },

                [HdCmd.ChnlDelay] = new Action[] { AbstractController_AnalogChannel.CtrlChannelDelay },

                [HdCmd.ChnlPosition] = new Action[] { AbstractController_AnalogChannel.CtrlAnalogChannelSet, AbstractController_Trigger.ConfigDgtParameter, AbstractController_Trigger.ConfigTypeAndParameter },
                [HdCmd.ChnlGain] = new Action[] { AbstractController_AnalogChannel.CtrlAnalogChannelSet},
                [HdCmd.ChnlInverted] = new Action[] { AbstractController_AnalogChannel.CtrlAnalogChannelSet, AbstractController_Trigger.ConfigDgtParameter, AbstractController_Trigger.ConfigTypeAndParameter },
                [HdCmd.ChnlBias] = new Action[] { AbstractController_AnalogChannel.CtrlAnalogChannelSet, AbstractController_Trigger.ConfigDgtParameter },
                [HdCmd.ChnlScaleIndex] = new Action[] { AbstractController_AnalogChannel.CtrlAnalogChannelSet, AbstractController_Trigger.ConfigFifoStageDepth_WithAcqLength, AbstractController_AnalogChannel.Send_IFCCoefficientsToAcqBoardByRegisterMode, AbstractController_AnalogChannel.CtrlADCOffset },

                [HdCmd.ChnlBandwidth] = new Action[] { AbstractController_AnalogChannel.CtrlAnalogChannelSet, AbstractController_AnalogChannel.Send_IFCCoefficientsToAcqBoardByRegisterMode },
                //[HdCmd.OuterPannelLEDCtrl] = new Action[] { /*CtrlAnalogChannel_JiHe2d5G.COMPort_OuterPannelLEDSet*/ }
                #endregion
            };
        }
    }
}

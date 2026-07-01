#if !Product_B21_JinHui_PXI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using static ScopeX.ComModel.HdMessage;

namespace ScopeX.Hardware.Driver
{
    internal partial class CtrlTrigger_Standard
    {
        internal static uint GetTriggerSource_Delay()
        {
            uint TrigSource = (uint)(Hd.UIMessage?.Trigger?.Delay?.SourceTwo ?? 0);
            return TrigSource;
        }

        internal static void Config_Delay()
        {
            uint trigSource = Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
            
            uint condition = (uint)(Hd.UIMessage?.Trigger?.Delay?.Condition ?? 0);
            uint SourceOneSlope = (uint)(Hd.UIMessage?.Trigger?.Delay?.SourceOneSlope ?? 0);//信源1边沿
            uint SourceTwoSlope = (uint)(Hd.UIMessage?.Trigger?.Delay?.SourceTwoSlope ?? 0);//信源2边沿
            uint SourceOne = (uint)(Hd.UIMessage?.Trigger?.Delay?.SourceOne ?? 0);
            uint SourceTwo = (uint)(Hd.UIMessage?.Trigger?.Delay?.SourceTwo ?? 0);
            HdMessage.AnalogOptions analogParametersOne = Hd.UIMessage!.Analog![SourceOne];
            HdMessage.AnalogOptions analogParametersTwo = Hd.UIMessage!.Analog![SourceTwo];
            uint trigType = (uint)Hd.UIMessage?.Trigger?.TrigType;//触发类型
            UInt64 LowerTime = (UInt64)(Hd.UIMessage?.Trigger?.Delay?.LowerByps ?? 4000 * 96) / (400 * 2 * 4);//时间下限
            UInt64 UpperTime = (UInt64)(Hd.UIMessage?.Trigger?.Delay?.UpperByps ?? 4000 * 96) / (400 * 2 * 4);//时间上限
            int trigSourceYPos = AbstractController_Trigger.AdcCenterValue;
            //uint oneposition = (uint)(Hd.UIMessage?.Trigger?.Transition?.UpperPosIndex ?? 0);
            //信源1电平，信源2电平           
            //uint SourceOnePosition = (UInt32)((Hd.UIMessage?.Trigger?.Transition?.UpperPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
            uint SourceOnePosition = (uint)((Constants.SAMPS_PER_YDIV * (Hd.UIMessage.Trigger.Delay.UpperPosition + analogParametersOne.Position) / analogParametersOne.Scale) + AbstractController_Trigger.AdcCenterValue);
            //uint SourceTwoPosition = (UInt32)((Hd.UIMessage?.Trigger?.Transition?.LowerPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
            uint SourceTwoPosition = (uint)((Constants.SAMPS_PER_YDIV * (Hd.UIMessage.Trigger.Delay.LowerPosition + analogParametersTwo.Position) / analogParametersTwo.Scale) + AbstractController_Trigger.AdcCenterValue);
            positionsel(SourceOnePosition,SourceOne);
            positionsel(SourceTwoPosition, SourceTwo);
            uint edgeselect;

            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, SourceTwoSlope);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, trigType);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgTrig_A_Sel, SourceOneSlope << 5 | SourceOne);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgTrig_B_Sel, SourceTwoSlope << 5 | SourceTwo);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinL16, (uint)LowerTime & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinH16, (uint)(LowerTime >> 16) & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMaxL16, (uint)UpperTime & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMaxH16, (uint)(UpperTime >> 16) & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgPwFuncSel, condition);
            //一级跌落极性选择 2bit：00：正极性跌落  01：负极性跌落  10：双沿跌落
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, (uint)Polarity);
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, (uint)trigType);
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinL16, (uint)DurationTime & 0xffff);
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinH16, (uint)(DurationTime >> 16) & 0xffff);
        }

    }
}
#endif
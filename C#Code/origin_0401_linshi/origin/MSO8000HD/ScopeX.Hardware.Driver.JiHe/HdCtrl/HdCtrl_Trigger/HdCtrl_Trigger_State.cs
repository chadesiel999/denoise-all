#if !Product_B21_JinHui_PXI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal partial class HdCtrl_Trigger
    {
        internal static void Config_State()
        {
            //(UInt32 Up, UInt32 Dn) upperCompVolt, LowerCompVolt;
            //int PerYDivAdcSamples = Constants.VIS_ADC_RES / Constants.VIS_YDIVS_NUM;
            //int AdcCenterValue = Constants.MAX_ADC_RES / 2;
            bool conformed = Hd.UIMessage!.Trigger!.State!.Conformed;//[0]
            uint clkPolarity = (uint)Hd.UIMessage!.Trigger!.State!.ClkPolarity;//[1]
            ChannelId clkSource = Hd.UIMessage!.Trigger!.State!.ClkSource;//[1]
            PatLevelCondition currCondition;
            //当前只能选择C1-C4,消息中没有通道分组选择标志
            uint ctrlWord = 0x0000;
            ctrlWord |= conformed ? 0u : 1u;
            ctrlWord |= clkPolarity<<1;
            //int trigSourceYPos = AbstractController_Trigger.AdcCenterValue;
            for (int index = 0; index < 4; index++)
            {
                currCondition = Hd.UIMessage!.Trigger!.State!.Positions![index].Condition;
                if (currCondition != PatLevelCondition.Any& index!= (uint)clkSource)
                    ctrlWord |= 1U << (index + 7);
                if (currCondition == PatLevelCondition.GreaterThan)
                    ctrlWord |= 1U << (index+2);
               
                /*uint trigSource = Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
                if (trigSource < ChannelIdExt.AnaChnlNum)
                    trigSourceYPos = (int)(AbstractController_Trigger.PerYDivAdcSamples * Hd.CurrHdMessage!.Analog![trigSource].PositionIndex / Constants.IDX_PER_YDIV + AbstractController_Trigger.AdcCenterValue);
                uint upper = (UInt32)(Hd.CurrHdMessage!.Analog![index].Position * 4 + AbstractController_Trigger.AdcCenterValue);
                //(UInt32)((Hd.CurrHdMessage?.Trigger?.Pattern? .Positions[index].Value) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
                uint lower = (UInt32)((Hd.CurrHdMessage?.Trigger?.State?.Positions[index].Value) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
                upperCompVolt.Up = upper;
                upperCompVolt.Dn = upper - Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
                LowerCompVolt.Up = lower;
                LowerCompVolt.Dn = lower - Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_1st_CompareVoltage1Down, upperCompVolt.Dn);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_1st_CompareVoltage1Up, upperCompVolt.Up);*/
            }
            currCondition = Hd.UIMessage!.Trigger!.State!.Positions![20].Condition;
            if (currCondition != PatLevelCondition.Any & clkSource.IsExtTrigger())
                ctrlWord |= 1U << 11;
            if (currCondition == PatLevelCondition.GreaterThan)
                ctrlWord |= 1U << 6;
            ctrlWord |= clkSource.IsExtTrigger()?4u:(uint)clkSource << 12;
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_State_CtrlWord, (uint)ctrlWord);    
        }
    }
}
#endif
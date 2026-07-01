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
        internal static void Config_Pattern()
        {
            //throw new NotImplementedException($" this type not Implemented");

            //uint condition =(uint)Hd.CurrHdMessage!.Trigger!.Pattern!.Operator;
            ////当前只能选择C1-C4,消息中没有通道分组选择标志
            //uint ctrlWord = 0x0000;
            ///*(UInt32 Up, UInt32 Dn) upperCompVolt, LowerCompVolt;
            //int  PerYDivAdcSamples = Constants.VIS_ADC_RES / Constants.VIS_YDIVS_NUM;
            //int  AdcCenterValue = Constants.MAX_ADC_RES / 2;*/
            //for (int index = 0; index < 4; index++)
            //{
            //    PatLevelCondition currCondition = Hd.CurrHdMessage!.Trigger!.Pattern!.Positions[index].Condition;
            //    if (currCondition != PatLevelCondition.Any)
            //        ctrlWord |= 1U << (index + 8);//八通道
            //    if (currCondition == PatLevelCondition.GreaterThan)
            //        ctrlWord |= 1U << (index);
            //    /*int trigSourceYPos = AbstractController_Trigger.AdcCenterValue;
            //    uint trigSource = Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
            //    if (trigSource < ChannelIdExt.AnaChnlNum)
            //        trigSourceYPos = (int)(AbstractController_Trigger.PerYDivAdcSamples * Hd.CurrHdMessage!.Analog![trigSource].PositionIndex / Constants.IDX_PER_YDIV + AbstractController_Trigger.AdcCenterValue);               
            //    uint upper = (UInt32)(Hd.CurrHdMessage!.Analog![index].Position * 4 + AbstractController_Trigger.AdcCenterValue);                    
            //    uint lower = (UInt32)((Hd.CurrHdMessage?.Trigger?. Pattern?.Positions[index].Value ) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
            //        upperCompVolt.Up = upper;
            //        upperCompVolt.Dn = upper - Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
            //        LowerCompVolt.Up = lower;
            //        LowerCompVolt.Dn = lower - Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;               
            //    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_1st_CompareVoltage1Down, upperCompVolt.Dn);
            //    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_1st_CompareVoltage1Up, upperCompVolt.Up);*/
            //}
            //uint Width = (uint)Hd.CurrHdMessage!.Trigger!.Pattern!.TimeCondition;
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Code_Condition, (uint)condition);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Code_CtrlWord, (uint)ctrlWord);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Code_WidthAndPolarity, Width);


            //ulong pulseWidth =(ulong) Hd.CurrHdMessage!.Trigger!.Pattern!.DurationByps;
            //ulong pulseUpperWidth = (ulong)Hd.CurrHdMessage!.Trigger!.Pattern!.UpperDurationByps;
            //UInt32 firstWidth = (UInt32)(pulseWidth / ((UInt64)Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 4));
            //UInt32 firstUpperWidth = (UInt32)(pulseUpperWidth / ((UInt64)Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 4));
            //ulong secondWidth = (UInt32)(pulseWidth / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDMA ?? 100000) / 1000));//宽度以逻辑采样间隔ps数计算
            //ulong secondUpperWidth = (UInt32)(pulseUpperWidth / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDMA ?? 100000) / 1000));
            ////设置一级触发脉宽P
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberL, (uint)firstWidth & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberM, (uint)(firstWidth >> 16) & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberH, (uint)(firstWidth >> 32) & 0xffff);
            ////设置一级排除触发脉宽
            ////Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_NumberL, (uint)(pulseUpperWidth) & 0xffff);
            ////Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_NumberM, (uint)(pulseUpperWidth >> 16) & 0xffff);
            ////Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_NumberH, (uint)(pulseUpperWidth >> 32) & 0xffff);
            ////设置二级触发脉宽
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)secondWidth & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(secondWidth >> 16) & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(secondWidth >> 32) & 0xffff);
            ////设置二级排除触发脉宽
            ////HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberL, (uint)secondUpperWidth & 0xffff);
            ////HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberM, (uint)(secondUpperWidth >> 16) & 0xffff);
            ////HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberH, (uint)(secondUpperWidth >> 32) & 0xffff);
        }
    }
}
#endif

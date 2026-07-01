#if !Product_B21_JinHui_PXI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal partial class CtrlTrigger_Standard
    {
        internal static uint GetTriggerSource_Window()
        {
            return (uint)(Hd.UIMessage?.Trigger?.Window?.Source ?? ChannelId.C1);
        }
        //使用internal  是为了共用
        internal static void Config_Window()
        {

            long widthByps = Hd.UIMessage?.Trigger?.Window?.WidthByps ?? 0;
            WindowTimeCondition condition = Hd.UIMessage?.Trigger?.Window?.Condition ?? WindowTimeCondition.GreaterThan;
            WindowRange range = Hd.UIMessage?.Trigger?.Window?.Range ?? WindowRange.Inside;
            if (condition == WindowTimeCondition.GreaterThan)
            {
                //选择一级脉宽触发条件
                //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PulseWidth_Condition, 0x00);
            }
            else if (condition == WindowTimeCondition.LessThan)
            {
                //选择一级脉宽触发条件
                //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PulseWidth_Condition, 0x01);
            }
            else if (condition == WindowTimeCondition.OnEnter)
            {
                //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PulseWidth_Condition, (uint)condition);

            }
            if (range == WindowRange.Inside)
            {

                //选择一级极性
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Window_setting, 0x00);//复用寄存器
            }
            else
            {

                //选择一级极性
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Window_setting, 0x04);//复用寄存器
            }
            UInt64 firstWidth = (UInt64)(widthByps / (Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 20));//所有产品一级触发处理路数固定为20路，宽度计算以ps计数
            (UInt32 Up, UInt32 Dn) upperCompVolt, LowerCompVolt;
            uint trigSource = Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
            int trigSourceYPos = AbstractController_Trigger.AdcCenterValue;
            if (trigSource < ChannelIdExt.AnaChnlNum)
                trigSourceYPos = (int)(AbstractController_Trigger.PerYDivAdcSamples * Hd.UIMessage!.Analog![trigSource].PositionIndex / Constants.IDX_PER_YDIV + AbstractController_Trigger.AdcCenterValue);
            uint upper = (UInt32)((Hd.UIMessage?.Trigger?.Window?.UpperPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
            uint lower = (UInt32)((Hd.UIMessage?.Trigger?.Window?.LowerPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
            if (range == WindowRange.Inside)
            {
                upperCompVolt.Up = upper + Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
                upperCompVolt.Dn = upper;
                LowerCompVolt.Up = lower;
                LowerCompVolt.Dn = lower - Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
            }
            else
            {
                upperCompVolt.Up = upper;
                upperCompVolt.Dn = upper - Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
                LowerCompVolt.Up = lower + Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
                LowerCompVolt.Dn = lower;
            }

            //设置一级触发脉宽
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)firstWidth & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(firstWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(firstWidth >> 32) & 0xffff);
            #if JiHe_MSO7000X
            //一级触发上限电平
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh3Level1, upperCompVolt.Dn);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh4Level1, upperCompVolt.Up);
            //一级触发下限电平
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level1, LowerCompVolt.Dn);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh2Level1, LowerCompVolt.Up);
#endif
            UInt64 secondWidth = (UInt64)(widthByps / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 100000) / 1000)); //宽度以逻辑采样间隔ps数计算
            //设置二级触发脉宽
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)secondWidth & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(secondWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(secondWidth >> 32) & 0xffff);
            //设置触发上限电平
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Down, upperCompVolt.Dn);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Up, upperCompVolt.Up);
            //二级触发下限电平
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, LowerCompVolt.Dn);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, LowerCompVolt.Up);
            //选择二级脉宽触发条件
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PulseWidth_Condition, (uint)condition);
            //选择二级脉宽极性
            if (range == WindowRange.Inside)
            {
                if (condition == WindowTimeCondition.GreaterThan)
                {
                    //选择一级脉宽触发条件
                   // HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Window_setting_2nd, 0x00);
                }
                else if (condition == WindowTimeCondition.LessThan)
                {
                    //选择一级脉宽触发条件
                   // HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Window_setting_2nd, 0x01);
                }
                else if (condition == WindowTimeCondition.OnEnter)
                {
                   // HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Window_setting_2nd, 0x00);

                }
            }
            else
            {
                if (condition == WindowTimeCondition.GreaterThan)
                {
                    //选择一级脉宽触发条件
                   // HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Window_setting_2nd, 0x04);
                }
                else if (condition == WindowTimeCondition.LessThan)
                {
                    //选择一级脉宽触发条件
                   // HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Window_setting_2nd, 0x05);
                }
                else if (condition == WindowTimeCondition.OnEnter)
                {
                   // HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Window_setting_2nd, 0x04);

                }
            }

        }
    }
}
#endif

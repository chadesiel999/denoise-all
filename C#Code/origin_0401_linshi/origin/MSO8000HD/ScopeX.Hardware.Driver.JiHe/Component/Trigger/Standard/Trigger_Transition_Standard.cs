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
        internal static uint GetTriggerSource_Transition()
        {
            return (uint)(Hd.UIMessage?.Trigger?.Transition?.Source ?? ChannelId.C1);
        }
        //使用internal  是为了共用
        internal static void Config_Transition()
        {
            UInt64 runtTime = (UInt64)(Hd.UIMessage?.Trigger?.Transition?.WidthByps ?? 96000) / (400 * 2*4);//持续时间
            //pulseWidth = (UInt64)(Hd.UIMessage?.Trigger?.Pulse?.WidthByps / (400 * 2));//脉宽下限
            UInt64 runtUpperTime = (UInt64)(Hd.UIMessage?.Trigger?.Transition?.UpperWidthByps ?? 96000) / (400 * 2 * 4);//持续时间上限
            UInt64 trigType = (UInt64)Hd.UIMessage?.Trigger?.TrigType;//触发类型
            UInt64 condition = (UInt64)Hd.UIMessage?.Trigger?.Transition?.Condition;//条件
            UInt64 slope = (UInt64)Hd.UIMessage?.Trigger?.Transition?.Slope;//斜率:上升下降
           // UInt64 polarity = (UInt64)Hd.UIMessage?.Trigger?.Transition?.Polarity;//极性

            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinL16, (uint)runtTime & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinH16, (uint)(runtTime >> 16) & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMaxL16, (uint)runtUpperTime & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMaxH16, (uint)(runtUpperTime >> 16) & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, (uint)trigType);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, (uint)slope);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgPwFuncSel, (uint)condition);
            //一级斜率触发条件和极性
            uint polarityAndCondition = 0;
            polarityAndCondition = (uint)(Hd.UIMessage?.Trigger?.Transition?.Condition ?? 0);
            if (Hd.UIMessage?.Trigger?.Transition?.Slope == EdgeSlope.Fall)
                polarityAndCondition |= 1 << 2;
            (UInt32 Up, UInt32 Dn) upperCompVolt, LowerCompVolt;
            uint trigSource = Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
            int trigSourceYPos = AbstractController_Trigger.AdcCenterValue;
            if (trigSource < ChannelIdExt.AnaChnlNum)
                trigSourceYPos = (int)(AbstractController_Trigger.PerYDivAdcSamples * Hd.UIMessage!.Analog![trigSource].PositionIndex / Constants.IDX_PER_YDIV + AbstractController_Trigger.AdcCenterValue);
            //低电平，高电平
            uint upper = (UInt32)((Hd.UIMessage?.Trigger?.Transition?.UpperPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
            uint lower = (UInt32)((Hd.UIMessage?.Trigger?.Transition?.LowerPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);




            if ((Hd.UIMessage?.Trigger?.Transition?.Slope ?? EdgeSlope.Rise) == EdgeSlope.Rise)
            {
                upperCompVolt.Up = upper;
                upperCompVolt.Dn = upper - Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
                LowerCompVolt.Up = lower;
                LowerCompVolt.Dn = lower - Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
            }
            else
            {
                upperCompVolt.Up = upper + Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
                upperCompVolt.Dn = upper;
                LowerCompVolt.Up = lower + Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
                LowerCompVolt.Dn = lower;
            }

            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Slope_PolarityAndCondition, polarityAndCondition);
            //一级斜率触发上限电平
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh3Level1, upperCompVolt.Dn);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh4Level1, upperCompVolt.Up);
            ////一级斜率触发下限电平
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level1, LowerCompVolt.Dn);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh2Level1, LowerCompVolt.Up);
            //设置斜率触发一级触发时间
            UInt64 firstTime = (UInt64)(runtTime / ((UInt64)Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 20));//所有产品一级触发处理路数固定为20路，宽度计算以ps计数
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)firstTime & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(firstTime >> 16) & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(firstTime >> 32) & 0xffff);
            //设置斜率触发一级排除触发时间
            UInt64 firstUpperTime = (UInt64)(runtUpperTime / ((UInt64)Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 20));//所有产品一级触发处理路数固定为20路，宽度计算以ps计数
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_NumberL, (uint)firstUpperTime & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_NumberM, (uint)(firstUpperTime >> 16) & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_NumberH, (uint)(firstUpperTime >> 32) & 0xffff);



            //二级斜率触发条件和极性
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Slope_PolarityAndCondition, polarityAndCondition);
            //设置斜率触发上限电平
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Down, upperCompVolt.Dn);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Up, upperCompVolt.Up);
            //二级斜率触发下限电平
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, LowerCompVolt.Dn);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, LowerCompVolt.Up);
            //设置斜率触发二级触发时间
            UInt64 secondTime = 0;// (UInt64)(runtTime / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDMA ?? 100000) / 1000));//宽度以逻辑采样间隔ps数计算
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)secondTime & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(secondTime >> 16) & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(secondTime >> 32) & 0xffff);
            //设置斜率触发二级排除触发时间
            UInt64 secondUpperTime = 0;// (UInt64)(runtUpperTime / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDMA ?? 100000) / 1000));//宽度以逻辑采样间隔ps数计算
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberL, (uint)secondUpperTime & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberM, (uint)(secondUpperTime >> 16) & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberH, (uint)(secondUpperTime >> 32) & 0xffff);
            //throw new NotImplementedException($" this type not Implemented");
        }
        internal static void Config_Transition_MultiQulified(ITriggerTypeOptions? triggerTypeOptions, int eventIndex)
        {
            if (triggerTypeOptions == null)
                return;
            var tansOption = (TrigTransOptions)triggerTypeOptions;
            UInt64 runtTime = (UInt64)(Hd.UIMessage?.Trigger?.Transition?.WidthByps ?? 96000);
            //一级斜率触发条件和极性
            uint polarityAndCondition = 0;
            polarityAndCondition = (uint)(tansOption?.Condition ?? 0);
            if (tansOption?.Slope == EdgeSlope.Fall)
                polarityAndCondition |= 1 << 2;
            (UInt32 Up, UInt32 Dn) upperCompVolt, LowerCompVolt;
            uint trigSource = Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
            int trigSourceYPos = AbstractController_Trigger.AdcCenterValue;
            if (trigSource < ChannelIdExt.AnaChnlNum)
                trigSourceYPos = (int)(AbstractController_Trigger.PerYDivAdcSamples * Hd.UIMessage!.Analog![trigSource].PositionIndex / Constants.IDX_PER_YDIV + AbstractController_Trigger.AdcCenterValue);

            uint upper = (UInt32)((tansOption?.UpperPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
            uint lower = (UInt32)((tansOption?.LowerPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
            if ((tansOption?.Slope ?? EdgeSlope.Rise) == EdgeSlope.Rise)
            {
                upperCompVolt.Up = upper;
                upperCompVolt.Dn = upper - Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
                LowerCompVolt.Up = lower;
                LowerCompVolt.Dn = lower - Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
            }
            else
            {
                upperCompVolt.Up = upper + Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
                upperCompVolt.Dn = upper;
                LowerCompVolt.Up = lower + Hd.CurrProduct!.Ctrl_Trigger!.DefaultTrigSensitivity;
                LowerCompVolt.Dn = lower;
            }
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Slope_PolarityAndCondition, polarityAndCondition);
            //一级斜率触发上限电平
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh3Level1, upperCompVolt.Dn);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh4Level1, upperCompVolt.Up);
            ////一级斜率触发下限电平
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level1, LowerCompVolt.Dn);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh2Level1, LowerCompVolt.Up);
            //设置斜率触发一级触发时间
            UInt64 firstTime = (UInt64)(runtTime / ((UInt64)Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 20));//所有产品一级触发处理路数固定为20路，宽度计算以ps计数
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)firstTime & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(firstTime >> 16) & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(firstTime >> 32) & 0xffff);



            //二级斜率触发条件和极性
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Slope_PolarityAndCondition, polarityAndCondition);
            //设置斜率触发上限电平
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Down, upperCompVolt.Dn);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Up, upperCompVolt.Up);
            //二级斜率触发下限电平
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, LowerCompVolt.Dn);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, LowerCompVolt.Up);
            //设置斜率触发二级触发时间
            UInt64 secondTime = (UInt64)(runtTime / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 100000) / 1000));//宽度以逻辑采样间隔ps数计算
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)secondTime & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(secondTime >> 16) & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(secondTime >> 32) & 0xffff);

            var count = Hd.UIMessage!.Trigger!.TrigMultiQualified!.EventOptions.Length;//事件个数

            //comment for JiHe_MSO7000X ProcBdReg.W regSource = eventIndex == 0 ? ProcBdReg.W.TrigCtrl_Cascaded_EventASourceSelect : ProcBdReg.W.TrigCtrl_Cascaded_EventBSourceSelect;
            //comment for JiHe_MSO7000X HdIO.WriteReg(regSource, (uint)(tansOption?.Source ?? ChannelId.C1));
            if (tansOption!.Source == ChannelId.C1)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B0, 2 << 3);
            }
            else if (tansOption!.Source == ChannelId.C2)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B2, 2 << 3);
            }
            else if (tansOption!.Source == ChannelId.C3)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B1, 2 << 3);
            }
            else if (tansOption!.Source == ChannelId.C4)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B0, 2 << 3);
            }

        }
    }
}
#endif

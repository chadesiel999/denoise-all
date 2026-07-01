#if !Product_B21_JinHui_PXI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using static ScopeX.ComModel.HdMessage;
using ScopeX.Hardware.Calibration.Data.Base;

namespace ScopeX.Hardware.Driver
{
    internal partial class HdCtrl_Trigger
    {
        internal static void Config_Transition()//????
        {
            UInt32 defaultTrigSensitivity = Hd.CurrProduct?.Acquirer_AnalogChannel?.DefaultTrigSensitivity ?? 50;
            ChannelId trigSource = Hd.UIMessage?.Trigger?.Transition?.Source ?? ChannelId.C1;
            UInt32 CompareLevelL = AbstractAcquirer_AnalogChannel.GetLevelByVoltage(trigSource, Hd.UIMessage?.Trigger?.Transition?.LowerPosition ?? 0);
            UInt32 CompareLevelH = AbstractAcquirer_AnalogChannel.GetLevelByVoltage(trigSource, Hd.UIMessage?.Trigger?.Transition?.UpperPosition ?? 0);
            EdgeSlope slope = Hd.UIMessage?.Trigger?.Transition?.Slope ?? EdgeSlope.Rise;
            PulseCondition slopeCondition = Hd.UIMessage?.Trigger?.Transition?.Condition ?? PulseCondition.GreaterThan;
            long UpperWidthByps = Hd.UIMessage?.Trigger?.Transition?.UpperWidthByps ?? 0;
            long WidthByps = Hd.UIMessage?.Trigger?.Transition?.WidthByps ?? 0;
            if (slope == 0)
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, CompareLevelL - defaultTrigSensitivity);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, CompareLevelL);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Down, CompareLevelH - defaultTrigSensitivity);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Up, CompareLevelH);
                //trig_2nd
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, CompareLevelL - defaultTrigSensitivity);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, CompareLevelL);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Down, CompareLevelH - defaultTrigSensitivity);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Up, CompareLevelH);
            }
            else
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, CompareLevelL);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, CompareLevelL + defaultTrigSensitivity);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Down, CompareLevelH);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Up, CompareLevelH + defaultTrigSensitivity);
                //trig_2nd
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, CompareLevelL);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, CompareLevelL + defaultTrigSensitivity);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Down, CompareLevelH);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Up, CompareLevelH + defaultTrigSensitivity);
            }
            UInt32 TrigSet = ((UInt32)slope << 3) | ((UInt32)slopeCondition);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_SlopeSetAcq, TrigSet);
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_SlopeSetPro, TrigSet);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_AdvSlopeSet, TrigSet);
            UInt64 extramNum = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.ExtractNumFromAdc ?? 1;
            UInt64 interpNum = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.InterplotNumToDMA ?? 1;
            UInt32 InterplotNumPre = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.InterplotNumFromADC ?? 1;
            Int64 firstWidth = WidthByps / 200;//除以200ps，将以ps为单位的建立时间参数变为以200ps为单位的控制字
            double dWidthByps = WidthByps;
            Int64 firstWidth_2nd = (Int64)(dWidthByps / 50 / extramNum * interpNum * InterplotNumPre);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_Width1LAcq, (uint)firstWidth & 0xffff);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_Width1HAcq, (uint)(firstWidth >> 16) & 0xffff);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_Width2LAcq, (uint)UpperWidthByps & 0xffff);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_Width2HAcq, (uint)(UpperWidthByps >> 16) & 0xffff);
            //trig_2nd
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width1SetL, (uint)firstWidth_2nd & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width1SetM, (uint)(firstWidth_2nd >> 16) & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width1SetH, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width2SetL, (uint)UpperWidthByps & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width2SetM, (uint)(UpperWidthByps >> 16) & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width2SetH, 0);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 0);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelectPro, (uint)trigSource);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SourceSelect, (uint)trigSource);
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
            uint trigSource = (UInt32)(Hd.CurrProduct?.Ctrl_Trigger?.CurrentTrigSource() ?? 0);
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
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Down, upperCompVolt.Dn);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Up, upperCompVolt.Up);
            //一级斜率触发下限电平
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, LowerCompVolt.Dn);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, LowerCompVolt.Up);
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
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B1, 2 << 3);
            }
            else if (tansOption!.Source == ChannelId.C2)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B3, 2 << 3);
            }
            else if (tansOption!.Source == ChannelId.C3)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B5, 2 << 3);
            }
            else if (tansOption!.Source == ChannelId.C4)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B7, 2 << 3);
            }

        }
    }
}
#endif

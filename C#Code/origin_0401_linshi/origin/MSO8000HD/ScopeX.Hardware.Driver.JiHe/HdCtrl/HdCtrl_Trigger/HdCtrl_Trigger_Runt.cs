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
        internal static void Config_Runt()//????
        {
            UInt32 defaultTrigSensitivity = Hd.CurrProduct?.Acquirer_AnalogChannel?.DefaultTrigSensitivity ?? 50;
            ChannelId trigSource = Hd.UIMessage?.Trigger?.Runt?.Source ?? ChannelId.C1;
            UInt32 CompareVoltageL = AbstractAcquirer_AnalogChannel.GetLevelByVoltage(trigSource, Hd.UIMessage?.Trigger?.Runt?.LowerPosition ?? 0);
            UInt32 CompareVoltageH = AbstractAcquirer_AnalogChannel.GetLevelByVoltage(trigSource, Hd.UIMessage?.Trigger?.Runt?.UpperPosition ?? 0);
            PulsePolarity runt = Hd.UIMessage?.Trigger?.Runt?.Polarity ?? PulsePolarity.Positive;
            PulseCondition runtCondition = Hd.UIMessage?.Trigger?.Runt?.Condition ?? PulseCondition.GreaterThan;
            long UpperWidthByps = Hd.UIMessage?.Trigger?.Runt?.UpperWidthByps ?? 0;
            long WidthByps = Hd.UIMessage?.Trigger?.Runt?.WidthByps ?? 0;
            if (runt == 0)
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, CompareVoltageL);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, CompareVoltageL + defaultTrigSensitivity);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Down, CompareVoltageH - defaultTrigSensitivity);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Up, CompareVoltageH);
                //trig_2nd
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, CompareVoltageL);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, CompareVoltageL + defaultTrigSensitivity);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Down, CompareVoltageH - defaultTrigSensitivity);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Up, CompareVoltageH);
            }
            else
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, CompareVoltageL);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, CompareVoltageL + defaultTrigSensitivity);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Down, CompareVoltageH - defaultTrigSensitivity);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Up, CompareVoltageH);
                //trig_2nd
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, CompareVoltageL);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, CompareVoltageL + defaultTrigSensitivity);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Down, CompareVoltageH - defaultTrigSensitivity);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Up, CompareVoltageH);
            }
            UInt32 TrigSet = ((UInt32)runt << 3) | ((UInt32)runtCondition);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_RuntSetAcq, TrigSet);
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_RuntSetPro, TrigSet);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_AdvRuntSet, TrigSet);
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
        internal static void Config_Runt_MultiQulified(ITriggerTypeOptions? triggerTypeOptions, int eventIndex)
        {
            if (triggerTypeOptions == null)
                return;
            var runtOption = (TrigRuntOptions)triggerTypeOptions;
            UInt64 runtWidth = (UInt64)(runtOption?.WidthByps ?? 96000);
            uint condition = (uint)(runtOption?.Condition ?? 0) & 0x3;
            condition |= (uint)(runtOption?.Polarity ?? 0) << 2;

            uint trigSource = (UInt32)(Hd.CurrProduct?.Ctrl_Trigger?.CurrentTrigSource()??0);
            int trigSourceYPos = AbstractController_Trigger.AdcCenterValue;
            if (trigSource < ChannelIdExt.AnaChnlNum)
                trigSourceYPos = (int)(AbstractController_Trigger.PerYDivAdcSamples * Hd.UIMessage!.Analog![trigSource].PositionIndex / Constants.IDX_PER_YDIV + AbstractController_Trigger.AdcCenterValue);


            (UInt32 Up, UInt32 Dn) upperCompVolt, LowerCompVolt;
            uint upper = (UInt32)((runtOption?.UpperPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
            uint lower = (uint)((runtOption?.LowerPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
            if (runtOption!.Polarity != PulsePolarity.Positive)//正极性以下降沿判断，反之亦然
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

            //一级欠幅触发条件
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Slope_PolarityAndCondition, condition);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Runt_Condition, condition);
            //一级欠幅触发电平上限
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Down, upperCompVolt.Dn);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Up, upperCompVolt.Up);
            //一级欠幅触发电平下限
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, LowerCompVolt.Dn);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, LowerCompVolt.Up);
            //二级欠幅触发宽度

            UInt64 firstWidth = (UInt64)(runtWidth / ((UInt64)Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 20));//所有产品一级触发处理路数固定为20路，宽度计算以ps计数
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)firstWidth & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(firstWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(firstWidth >> 32) & 0xffff);
            //一级欠幅触发极性
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_1st_EdgeSelect, 0x00);

            //二级欠幅触发条件
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Runt_Condition, condition);
            //二级欠幅触发电平上限
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Down, upperCompVolt.Dn);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Up, upperCompVolt.Up);
            //二级欠幅触发电平下限
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, LowerCompVolt.Dn);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, LowerCompVolt.Up);
            //二级欠幅触发宽度
            UInt64 secondWidth = 0;// (UInt64)(runtWidth / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDMA ?? 100000) / 1000));//宽度以逻辑采样间隔ps数计算
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)secondWidth & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(secondWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(secondWidth >> 32) & 0xffff);
            var count = Hd.UIMessage!.Trigger!.TrigMultiQualified!.EventOptions.Length;//事件个数

            //comment for JiHe_MSO7000X ProcBdReg.W regSource = eventIndex == 0 ? ProcBdReg.W.TrigCtrl_Cascaded_EventASourceSelect : ProcBdReg.W.TrigCtrl_Cascaded_EventBSourceSelect;         
            //comment for JiHe_MSO7000X HdIO.WriteReg(regSource, (uint)(runtOption?.Source ?? ChannelId.C1));
            if (runtOption!.Source == ChannelId.C1)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B1, 3 << 3);
            }
            else if (runtOption!.Source == ChannelId.C2)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B3, 3 << 3);
            }
            else if (runtOption!.Source == ChannelId.C3)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B5, 3 << 3);
            }
            else if (runtOption!.Source == ChannelId.C4)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B7, 3 << 3);
            }

        }
    }
}
#endif

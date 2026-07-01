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
    internal partial class HdCtrl_Trigger
    {
        internal static void Config_PulseWidth()
        {
            UInt64 pulseWidth = (UInt64)Math.Ceiling(((double)Hd.UIMessage?.Trigger?.Pulse?.WidthByps / (400)));//脉宽下限
            UInt64 UpperpulseWidth = (UInt64)Math.Ceiling(((double)Hd.UIMessage?.Trigger?.Pulse?.UpperWidthByps / (400)));//脉宽上限
            //UInt64 pulseWidth = (UInt64)(Hd.UIMessage?.Trigger?.Pulse?.WidthByps ?? 4000 * 96);//脉宽下限
            //UInt64 UpperpulseWidth = (UInt64)(Hd.UIMessage?.Trigger?.Pulse?.UpperWidthByps ?? 4000 * 96);//脉宽上限
            //UInt64 firstWidth = (UInt64)(pulseWidth / ((UInt64)Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 20));//所有产品一级触发处理路数固定为20路，宽度计算以ps计数
            //UInt64 firstUpperWidth = (UInt64)(UpperpulseWidth / ((UInt64)Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 20));
            //设置一级触发脉宽
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)pulseWidth & 0xffff);
            UInt64 polarity = (UInt64)Hd.UIMessage?.Trigger?.Pulse?.Polarity;//极性
            if (polarity == 0)
                polarity = 1;
            else
                polarity = 0;
            UInt64 trigType = (UInt64)Hd.UIMessage?.Trigger?.TrigType;//触发类型
            UInt64 condition = (UInt64)Hd.UIMessage?.Trigger?.Pulse?.Condition;//条件
                                                                               //if (condition==1)//此处FPGA应该比较下限值，为了方便，用driver主动设置上限值
                                                                               //{
                                                                               //    pulseWidth = UpperpulseWidth;
                                                                               //}
                                                                               //pulseWidth = 200;

            //zwj
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrgDelayTimeMinL16_Pro, (uint)pulseWidth & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrgDelayTimeMinH16_Pro, (uint)(pulseWidth >> 16) & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrgDelayTimeMaxL16_Pro, (uint)UpperpulseWidth & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrgDelayTimeMaxH16_Pro, (uint)(UpperpulseWidth >> 16) & 0xffff);


            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrgPwFuncSel_Pro, (uint)condition);

            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinL16, (uint)pulseWidth & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinH16, (uint)(pulseWidth >> 16) & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMaxL16, (uint)UpperpulseWidth & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMaxH16, (uint)(UpperpulseWidth >> 16) & 0xffff);

            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, (uint)trigType);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, (uint)polarity);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgPwFuncSel, (uint)condition);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_NumberL, (uint)firstUpperWidth & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_NumberM, (uint)(firstUpperWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_NumberH, (uint)(firstUpperWidth >> 32) & 0xffff);
            //选择一级脉宽触发条件
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PulseWidth_Condition, (uint)(Hd.CurrHdMessage?.Trigger?.Pulse?.Condition ?? 0));
            //选择一级脉宽极性
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PulseWidth_Polarity, (uint)(Hd.CurrHdMessage?.Trigger?.Pulse?.Polarity ?? 0));


            UInt64 secondWidth = (UInt64)(pulseWidth / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 100000) / 1000));//宽度以逻辑采样间隔ps数计算
            UInt64 secondUpperWidth = (UInt64)(UpperpulseWidth / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 100000) / 1000));
            //设置二级触发脉宽
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)secondWidth & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(secondWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(secondWidth >> 32) & 0xffff);
            //设置二级排除触发脉宽
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberL, (uint)secondUpperWidth & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberM, (uint)(secondUpperWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberH, (uint)(secondUpperWidth >> 32) & 0xffff);
            //选择二级脉宽触发条件
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PulseWidth_Condition, (uint)(Hd.CurrHdMessage?.Trigger?.Pulse?.Condition ?? 0));
            //选择二级脉宽极性
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PulseWidth_Polarity, (uint)(Hd.CurrHdMessage?.Trigger?.Pulse?.Polarity ?? 0));
        }
        internal static void Config_PulseWidth_MultiQulified(ITriggerTypeOptions? triggerTypeOptions, int eventIndex)
        {
            if (triggerTypeOptions == null)
                return;
            var pulseWitdthOption = (TrigPulseOptions)triggerTypeOptions;
            UInt64 pulseWidth = (UInt64)(pulseWitdthOption?.WidthByps ?? 4000 * 96);
            UInt64 firstWidth = (UInt64)(pulseWidth / ((UInt64)Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 20));//所有产品一级触发处理路数固定为20路，宽度计算以ps计数
            //设置一级触发脉宽
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)firstWidth & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(firstWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(firstWidth >> 32) & 0xffff);
            //选择一级脉宽触发条件
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PulseWidth_Condition, (uint)(pulseWitdthOption?.Condition ?? 0));
            //选择一级脉宽极性
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PulseWidth_Polarity, (uint)(pulseWitdthOption?.Polarity ?? 0));


            UInt64 secondWidth = (UInt64)(pulseWidth / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 100000) / 1000));//宽度以逻辑采样间隔ps数计算
            //设置二级触发脉宽
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)secondWidth & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(secondWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(secondWidth >> 32) & 0xffff);
            //选择二级脉宽触发条件
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PulseWidth_Condition, (uint)(pulseWitdthOption?.Condition ?? 0));
            //选择二级脉宽极性
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PulseWidth_Polarity, (uint)(pulseWitdthOption?.Polarity ?? 0));
            var count = Hd.UIMessage!.Trigger!.TrigMultiQualified!.EventOptions.Length;//事件个数

            //comment for JiHe_MSO7000X ProcBdReg.W regSource = eventIndex == 0 ? ProcBdReg.W.TrigCtrl_Cascaded_EventASourceSelect : ProcBdReg.W.TrigCtrl_Cascaded_EventBSourceSelect;
            //comment for JiHe_MSO7000X HdIO.WriteReg(regSource, (uint)(pulseWitdthOption?.Source ?? ChannelId.C1));
            if (pulseWitdthOption!.Source == ChannelId.C1)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B0, 1 << 3);
            }
            else if (pulseWitdthOption!.Source == ChannelId.C2)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B2, 1 << 3);
            }
            else if (pulseWitdthOption!.Source == ChannelId.C3)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B1, 1 << 3);
            }
            else if (pulseWitdthOption!.Source == ChannelId.C4)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B0, 1 << 3);
            }

        }
    }
}
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using static ScopeX.ComModel.HdMessage;

namespace ScopeX.Hardware.Driver
{
    internal static partial class HdCtrl_Trigger
    {
        internal static void Config_Glitch()
        {

            UInt64 pulseWidth = (UInt64)(Hd.UIMessage?.Trigger?.Glitch?.WidthByps ?? 4000 * 96);
            UInt64 firstWidth = (UInt64)(pulseWidth / ((UInt64)Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 20));//所有产品一级触发处理路数固定为20路，宽度计算以ps计数
            //设置一级触发脉宽
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)firstWidth & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(firstWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(firstWidth >> 32) & 0xffff);
            //选择一级脉宽触发条件
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PulseWidth_Condition, (uint)(Hd.CurrHdMessage?.Trigger?.Glitch?.Condition ?? 0));
            //选择一级脉宽极性
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PulseWidth_Polarity, (uint)(Hd.CurrHdMessage?.Trigger?.Glitch?.Polarity ?? 0));


            UInt64 secondWidth = (UInt64)(pulseWidth / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 100000) / 1000));//宽度以逻辑采样间隔ps数计算
            //设置二级触发脉宽
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)secondWidth & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(secondWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(secondWidth >> 32) & 0xffff);
            //选择二级脉宽触发条件
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PulseWidth_Condition, (uint)(Hd.CurrHdMessage?.Trigger?.Glitch?.Condition ?? 0));
            //选择二级脉宽极性
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PulseWidth_Polarity, (uint)(Hd.CurrHdMessage?.Trigger?.Glitch?.Polarity ?? 0));
        }
    }
}

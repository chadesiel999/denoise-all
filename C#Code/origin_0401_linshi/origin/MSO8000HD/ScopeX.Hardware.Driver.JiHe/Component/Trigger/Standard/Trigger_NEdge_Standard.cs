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
        internal static uint GetTriggerSource_NEdge()
        {
            return (uint)(Hd.UIMessage?.Trigger?.NEdge?.Source ?? ChannelId.C1);
        }
        //使用internal  是为了共用
        internal static void Config_NEdge()
        {
            
            uint Polarity = (uint)(Hd.UIMessage?.Trigger?.NEdge?.Polarity ?? 00);//极性
            UInt64 trigType = (UInt64)Hd.UIMessage?.Trigger?.TrigType;//触发类型
            UInt64 FreeTime = (UInt64)(Hd.UIMessage?.Trigger?.NEdge?.DurationByps ?? 4000 * 96) / (400 * 2 * 4); //空闲时间
            uint EdgeNumber = (uint)(Hd.UIMessage?.Trigger?.NEdge?.EdgeNumber??0); 
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Trg_nTrigNum, (uint)EdgeNumber);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, (uint)Polarity);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, (uint)trigType);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinL16, (uint)FreeTime & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinH16, (uint)(FreeTime >> 16) & 0xffff);
            

        }
        internal static void Config_NEdge_MultiQulified(ITriggerTypeOptions? triggerTypeOptions, int eventIndex)
        {
            if (triggerTypeOptions == null)
                return;
            var timeoutOption = (TrigTimeOutOptions)triggerTypeOptions;
            uint firstPolarity = (uint)(timeoutOption?.Polarity ?? 00);//一级超时极性
            if (firstPolarity == 00 || firstPolarity == 01)
            {
                //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Dropout_IsDualEdgeRefresh, 0);
            }
            else
            {
                //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Dropout_IsDualEdgeRefresh, 1);
            }


            //一级跌落极性选择 2bit：00：正极性跌落  01：负极性跌落  10：双沿跌落
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Dropout_Polarity, (uint)(timeoutOption?.Polarity ?? 00));
            UInt64 DurationTime = (UInt64)(timeoutOption?.DurationByps ?? 4000 * 96);

            UInt64 firstWidth = (UInt64)(DurationTime / ((UInt64)Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 20));//所有产品一级触发处理路数固定为20路，宽度计算以ps计数
            //设置一级触发脉宽
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)firstWidth & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(firstWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(firstWidth >> 32) & 0xffff);

            UInt64 secondWidth = 0;// (UInt64)(DurationTime / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDMA ?? 100000) / 1000));//宽度以逻辑采样间隔ps数计算
            //设置二级触发脉宽
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)secondWidth & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(secondWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(secondWidth >> 32) & 0xffff);
            var count = Hd.UIMessage!.Trigger!.TrigMultiQualified!.EventOptions.Length;//事件个数

            //comment for JiHe_MSO7000X    ProcBdReg.W regSource = eventIndex == 0 ? ProcBdReg.W.TrigCtrl_Cascaded_EventASourceSelect : ProcBdReg.W.TrigCtrl_Cascaded_EventBSourceSelect;
            //comment for JiHe_MSO7000X HdIO.WriteReg(regSource, (uint)(timeoutOption?.Source ?? ChannelId.C1));
            if (timeoutOption!.Source == ChannelId.C1)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B0, 0 << 3);
            }
            else if (timeoutOption!.Source == ChannelId.C2)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B2, 0 << 3);
            }
            else if (timeoutOption!.Source == ChannelId.C3)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B1, 0 << 3);
            }
            else if (timeoutOption!.Source == ChannelId.C4)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B0, 0 << 3);
            }

        }
    }
}
#endif

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
        internal static void Config_TimeOut()
        {
            //throw new NotImplementedException($" this type not Implemented");
            uint firstPolarity = (uint)(Hd.UIMessage?.Trigger?.TimeOut?.Polarity ?? 00);//一级超时极性
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PulseWidth_Condition, 0);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PulseWidth_Condition, 0);
            if (firstPolarity == 00 || firstPolarity == 01)
            {
                //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Dropout_IsDualEdgeRefresh, 0);
            }
            else
            {
                //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Dropout_IsDualEdgeRefresh, 1);
            }


            //一级跌落极性选择 2bit：00：正极性跌落  01：负极性跌落  10：双沿跌落
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Dropout_Polarity, (uint)(Hd.CurrHdMessage?.Trigger?.TimeOut.Polarity ?? 00));
            UInt64 DurationTime = (UInt64)(Hd.UIMessage?.Trigger?.TimeOut?.DurationByps ?? 4000 * 96);

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
            //二级超时极性选择  0 负极性超时；1; 正极性超时
            ////uint secondPolarity = (uint)(Hd.CurrHdMessage?.Trigger?.TimeOut?.Polarity ?? 00);//二级超时极性
            ////if (secondPolarity == 0 || secondPolarity == 2)//发送是Polarity第一位？
            ////{
            ////    HdIO.WriteReg((uint)AcqBdReg.W.TrigCtrl_Dropout_Polarity, 0);

            ////}
            ////else
            ////{
            ////    HdIO.WriteReg((uint )AcqBdReg.W.TrigCtrl_Dropout_Polarity, 1);
            ////}
            uint secondPolarity = (uint)(Hd.UIMessage?.Trigger?.TimeOut?.Polarity ?? 00);
            if (secondPolarity == 0 || secondPolarity == 2)//发送是Polarity第一位？
            {
                //comment for JiHe_MSO7000X HdIO.WriteReg((uint)AcqBdReg.W.TrigCtrl_Dropout_Polarity, 0);

            }
            else
            {
                //comment for JiHe_MSO7000X HdIO.WriteReg((uint)AcqBdReg.W.TrigCtrl_Dropout_Polarity, 1);
            }
            if (secondPolarity == 00)
            {
                //comment for JiHe_MSO7000X HdIO.WriteReg((uint)ProcBdReg.W.TrigCtrl_Timeout_IsKeepHighLevel, 1);

            }
            else
            {
                //comment for JiHe_MSO7000X HdIO.WriteReg((uint)ProcBdReg.W.TrigCtrl_Timeout_IsKeepHighLevel, 0);
            }

        }
        internal static void Config_TimeOut_MultiQulified(ITriggerTypeOptions? triggerTypeOptions, int eventIndex)
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

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
        internal static void Config_Interval()
        {
            PulseCondition condition = Hd.UIMessage!.Trigger!.Interval!.Condition;
            PulsePolarity polarity = Hd.UIMessage!.Trigger!.Interval!.Polarity;
            long widthByps = Hd.UIMessage!.Trigger!.Interval!.WidthByps;
            long widthUpperByps = Hd.UIMessage!.Trigger!.Interval!.UpperWidthByps;

            UInt64 firstWidth = (UInt64)(widthByps / (Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 4));//所有产品一级触发处理路数固定为20路，宽度计算以ps计数
            UInt64 firstUpperWidth = (UInt64)(widthUpperByps / (Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 4));
            //设置一级触发间隔
            /*HdIO.WriteReg(ProcBdReg.W.trig_exclude_pro_1st_exclude_width1_l, (uint)firstWidth & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.trig_exclude_pro_1st_exclude_width2_l, (uint)(firstWidth >> 16) & 0xffff);
            //设置一级排除触发间隔
            HdIO.WriteReg(ProcBdReg.W.trig_exclude_pro_1st_exclude_width1_h, (uint)firstUpperWidth & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.trig_exclude_pro_1st_exclude_width2_h, (uint)(firstUpperWidth >> 16) & 0xffff);*/
            //选择一级间隔触发条件
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Interval_Condition, (uint)condition);
            ////选择一级间隔极性
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Interval_Polarity, (uint)polarity);

            UInt64 secondWidth = (UInt64)(widthByps / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 100000) / 1000));//宽度以逻辑采样间隔ps数计算
            UInt64 secondUpperWidth = (UInt64)(widthUpperByps / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 100000) / 1000));
            //设置二级触发间隔
            /*HdIO.WriteReg(ProcBdReg.W.trig_exclude_pro_2nd_exclude_width1_l, (uint)secondWidth & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.trig_exclude_pro_2nd__exclude_width2_l, (uint)(secondWidth >> 16) & 0xffff);
            //设置二级排除触发间隔
            HdIO.WriteReg(ProcBdReg.W.trig_exclude_pro_2nd__exclude_width1_h, (uint)secondUpperWidth & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.trig_exclude_pro_2nd__exclude_width2_h, (uint)(secondUpperWidth >> 16) & 0xffff);*/

            //选择二级间隔触发条件
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Interval_Condition, (uint)condition);
            //选择二级间隔极性 多通道时要用
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_EdgeThenEdge_CapturePolarity, (uint)polarity);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_EdgeThenEdge_LaunchPolarity, (uint)polarity);
            ChannelId source = Hd.UIMessage!.Trigger!.Interval!.Source;

            /*HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ASourceSel_EventASourceSelect, (uint)source);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_BSourceSel_EventBSourceSelect, (uint)source);*/

        }
    }
}

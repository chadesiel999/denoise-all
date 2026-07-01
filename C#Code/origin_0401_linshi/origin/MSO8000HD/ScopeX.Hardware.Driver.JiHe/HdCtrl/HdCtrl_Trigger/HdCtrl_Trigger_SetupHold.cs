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
        internal static void Config_SetupHold()
        {
            if (Hd.UIMessage!.Trigger!.SetupHold == null)
                return;
            uint clkPolarity = (uint)Hd.UIMessage!.Trigger!.SetupHold.ClkPolarity;
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_EdgeThenEdge_CapturePolarity, clkPolarity);
            uint clkPosition = (uint)Hd.UIMessage!.Trigger!.SetupHold.ClkPosition.Value;
            ComModel.ChannelId clkSource = Hd.UIMessage!.Trigger!.SetupHold.ClkSource;
            ComModel.ChannelId dataSource = Hd.UIMessage!.Trigger!.SetupHold.DataSource;
            uint dataLowerPosition = (uint)Hd.UIMessage!.Trigger!.SetupHold.DataLowerPosition.Value;
            uint dataLUpperPosition = (uint)Hd.UIMessage!.Trigger!.SetupHold.DataUpperPosition.Value;
            uint violation = (uint)Hd.UIMessage!.Trigger!.SetupHold.Violation;
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Setuphold_PolarityAndCondition, violation);


           // HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ASourceSel_EventASourceSelect, (UInt32)clkSource);
           // HdIO.WriteReg(ProcBdReg.W.TrigCtrl_BSourceSel_EventBSourceSelect, (UInt32)dataSource);
            Int64 thdByps = Hd.UIMessage!.Trigger!.SetupHold.ThdByps;
            Int64 tsuByps = Hd.UIMessage!.Trigger!.SetupHold.TsuByps;
            Int64 firstWidth = tsuByps / ((Int64)Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps / 4);
            UInt64 secondWidth = 0;// (UInt64)(tsuByps / ((Hd.AnalogChannel?.AcquingParameters.PerDataByfs_AtDMA ?? 100000) / 1000));//宽度以逻辑采样间隔ps数计算HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthL,(uint)firstWidth & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberL, (uint)firstWidth & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberM, (uint)(firstWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_NumberH, (uint)(firstWidth >> 32) & 0xffff);

            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthL, (uint)secondWidth & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthM, (uint)(secondWidth >> 16) & 0xffff);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_WidthSet_WidthH, (uint)(secondWidth >> 32) & 0xffff);
        }
    }
}
#endif
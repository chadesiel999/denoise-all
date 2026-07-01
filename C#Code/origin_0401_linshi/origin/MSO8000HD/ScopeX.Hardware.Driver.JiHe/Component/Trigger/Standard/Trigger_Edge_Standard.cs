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
        internal static uint GetTriggerSource_Edge()
        {
            return (uint)(Hd.UIMessage?.Trigger?.Edge?.Source ?? ChannelId.C1);
        }
        //使用internal  是为了共用
        internal static void Config_Edge()
        {
            int triggerSource = (int)GetTriggerSource_Edge();
            int Slope = (int)Hd.UIMessage!.Trigger!.Edge!.Slope;

            if (triggerSource < ChannelIdExt.AnaChnlNum)
            {
                //if (Hd.UIMessage!.Analog![triggerSource].IsInverted && Slope != (int)EdgeSlope.Both)
                //    Slope = 1 - Slope;
            }
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, (uint)Slope);//compVolt.Edge
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_EdgeSelect_Pro, (uint)Slope);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_EdgeSelect, (uint)(Hd.CurrHdMessage?.Trigger?.Edge?.Slope == 0 ? 1 : 0));

            uint trigChannel = (uint)(Hd.UIMessage?.Trigger?.Edge?.Source ?? ChannelId.C1);
#if LA
            HdIO.WriteReg(ProcBdReg.W.LA_TrigEdgeSel, (UInt32)(((Hd.CurrHdMessage!.Trigger!.TrigType == TriggerType.Edge && ChannelIdExt.IsDigital((ChannelId)trigChannel) ? 1 : 0) << 1) | ((Hd.CurrHdMessage!.Trigger!.Edge!.Slope == EdgeSlope.Rise ? 1 : 0))));
#endif
        }

        internal static void Config_Edge_MultiQulified(ITriggerTypeOptions? triggerTypeOptions, int eventIndex)
        {
            if (triggerTypeOptions == null)
                return;
            var edgeOption = (TrigEdgeOptions)triggerTypeOptions;
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, (uint)(edgeOption.Slope == 0 ? 1 : 0));//compVolt.Edge
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_EdgeSelect_Pro, (uint)(edgeOption.Slope == 0 ? 1 : 0));                                                                                                      //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_EdgeSelect, (uint)(edgeOption.Slope == 0 ? 1 : 0));

            var count = Hd.UIMessage!.Trigger!.TrigMultiQualified!.EventOptions.Length;//事件个数


            if (edgeOption!.Source == ChannelId.C1)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B0, 0 << 3);
            }
            else if (edgeOption!.Source == ChannelId.C2)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B2, 0 << 3);
            }
            else if (edgeOption!.Source == ChannelId.C3)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B1, 0 << 3);
            }
            else if (edgeOption!.Source == ChannelId.C4)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, AcqBdNo.B0, 0 << 3);
            }

            //var compVolt = (UInt32)((edgeOption!.PosIndex) / Constants.IDX_PER_YDIV * Contorller_Trigger_8GModule.PerYDivAdcSamples + Contorller_Trigger_8GModule.AdcCenterValue);
            //var isPositive = edgeOption!.Slope == EdgeSlope.Rise;
            //Contorller_Trigger_8GModule.GetVolt(isPositive,compVolt);
        }
    }
}
#endif

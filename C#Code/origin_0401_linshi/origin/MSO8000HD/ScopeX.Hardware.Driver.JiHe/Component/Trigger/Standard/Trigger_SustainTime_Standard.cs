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
        internal static uint GetTriggerSource_SustainTime()
        {
            return (uint)(/*Hd.UIMessage?.Trigger?.SustainTime?.Source ??*/ ChannelId.C1);
        }

        internal static void Config_SustainTime()
        {
            //throw new NotImplementedException($" this type not Implemented");
            uint trigSource = Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
            var nalog = Hd.UIMessage?.Analog;
            if (nalog != null)
            {
                for (int i = 0; i < nalog.Length; i++)
                {
                    if (nalog[i].Active)
                    {
                        SustainTimeLevelCondition condition = Hd.UIMessage?.Trigger?.SustainTime?.Positions[i].Condition?? SustainTimeLevelCondition.Any ;
                        if (condition != SustainTimeLevelCondition.Any)
                        {
                            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, (uint)i);
                            break;
                        }
                    }
                }
            }
            uint PositionsLength = 20;
            uint chnumber = 4;
            UInt32[] state =new UInt32[PositionsLength];
            UInt64 stateset = 0;
           for(int i = 0; i< PositionsLength; i++)
           {
                state[i] = (uint)Hd.UIMessage?.Trigger?.SustainTime?.Positions[i].Condition;
                stateset |= (uint)Hd.UIMessage?.Trigger?.SustainTime?.Positions[i].Condition << i * 3;
           }
           for(int i = 0;i< chnumber;i++)
            {
                HdMessage.AnalogOptions analogParameters = Hd.UIMessage!.Analog![i];
                uint chposition = (uint)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.SustainTime?.Positions[i].Value + analogParameters.Position) / analogParameters.Scale) + (uint)AbstractController_Trigger.AdcCenterValue;
                positionsel(chposition, (uint)i);
            }
            UInt64 trigType = (UInt64)Hd.UIMessage?.Trigger?.TrigType;//触发类型
            uint codtion = (uint)Hd.UIMessage?.Trigger?.SustainTime?.Condition;
            UInt64 DurationTimeUp = (UInt64)(Hd.UIMessage?.Trigger?.SustainTime?.UpperWidthByps ?? 4000 * 96) / (400 * 2 * 4);
            UInt64 DurationTimeLow = (UInt64)(Hd.UIMessage?.Trigger?.SustainTime?.WidthByps ?? 4000 * 96) / (400 * 2 * 4);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PulseWidth_Condition, 0);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PulseWidth_Condition, 0);


            //一级跌落极性选择 2bit：00：正极性跌落  01：负极性跌落  10：双沿跌落

            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgChStateSel0, (uint)stateset & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgChStateSel1, (uint)stateset >>16& 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgChStateSel2, (uint)(stateset >> 32) & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgChStateSel3, (uint)(stateset >> 48) & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, (uint)0);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgPwFuncSel, codtion);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, (uint)trigType);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMaxL16, (uint)DurationTimeUp & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMaxH16, (uint)(DurationTimeUp >> 16) & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinL16, (uint)DurationTimeLow & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinH16, (uint)(DurationTimeLow >> 16) & 0xffff);
        }

    }
}
#endif
#if !Product_B21_JinHui_PXI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Driver
{
    internal partial class CtrlTrigger_Standard
    {
        internal static uint GetTriggerSource_Pattern()
        {
            return (uint)(uint)(Hd.UIMessage?.Trigger?.Edge?.Source ?? ChannelId.C1);
        }
        //使用internal  是为了共用
        internal static void Config_Pattern()
        {
            uint trigSource = Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
            var nalog = Hd.UIMessage?.Analog;
            if (nalog != null)
            {
                for (int i = 0; i < nalog.Length; i++)
                {
                    if (nalog[i].Active)
                    {
                        PatLevelCondition condition = Hd.UIMessage?.Trigger?.Pattern?.Positions[i].Condition?? PatLevelCondition.Any;
                        if (condition != PatLevelCondition.Any)
                        {
                            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, (uint)i);
                            break;
                        }
                    }
                }
            }
            uint trigType = (uint)Hd.UIMessage?.Trigger?.TrigType;//触发类型
            uint codtion = (uint)Hd.UIMessage?.Trigger?.Pattern?.TimeCondition;
            UInt64 DurationTimeUp = (UInt64)(Hd.UIMessage?.Trigger?.Pattern?.UpperDurationByps ?? 4000 * 96) / (400 * 2 * 4);
            UInt64 DurationTimeLow = (UInt64)(Hd.UIMessage?.Trigger?.Pattern?.DurationByps ?? 4000 * 96) / (400 * 2 * 4);
            uint PositionsLength = 20;
            uint chnumber = 4;
            UInt32[] state = new UInt32[PositionsLength];
            UInt64 stateset = 0;
            for (int i = 0; i < PositionsLength; i++)
            {
                state[i] = (uint)Hd.UIMessage?.Trigger?.Pattern?.Positions[i].Condition;
                stateset |= (uint)Hd.UIMessage?.Trigger?.Pattern?.Positions[i].Condition << i * 3;
            }
            for (int i = 0; i < chnumber; i++)
            {
                HdMessage.AnalogOptions analogParameters = Hd.UIMessage!.Analog![i];
                uint chposition = (uint)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Pattern?.Positions[i].Value + analogParameters.Position) / analogParameters.Scale) + (uint)AbstractController_Trigger.AdcCenterValue;
                positionsel(chposition, (uint)i);
            }
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgChStateSel0, (uint)stateset & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgChStateSel1, (uint)stateset >> 16 & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgChStateSel2, (uint)(stateset >> 32) & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgChStateSel3, (uint)(stateset >> 48) & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgPwFuncSel, codtion);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, trigType);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMaxL16, (uint)DurationTimeUp & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMaxH16, (uint)(DurationTimeUp >> 16) & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinL16, (uint)DurationTimeLow & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinH16, (uint)(DurationTimeLow >> 16) & 0xffff);
            for (int i = 0; i < chnumber; i++)
            {
                if (Hd.UIMessage?.Trigger?.Pattern?.Positions[i].Condition == PatLevelCondition.Rise)
                { 
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, (uint)i);
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, 0);
                }
                if (Hd.UIMessage?.Trigger?.Pattern?.Positions[i].Condition == PatLevelCondition.Fall)
                {
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, (uint)i);
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, 1);
                }
            }
        }
    }
}
#endif

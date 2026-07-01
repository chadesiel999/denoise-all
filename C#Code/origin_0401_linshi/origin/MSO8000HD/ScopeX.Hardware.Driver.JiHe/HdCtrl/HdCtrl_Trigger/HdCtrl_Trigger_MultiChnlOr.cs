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
        internal static void Config_MultiChnlOr()//????
        {
            ChannelId source = ChannelId.C1;
            UInt32 compareLevel = AbstractAcquirer_AnalogChannel.GetLevelByVoltage(source, Hd.UIMessage?.Trigger?.TrigMultiChnlOr?.ThresHold ?? 0);
            UInt32 defaultTrigSensitivity = Hd.CurrProduct?.Acquirer_AnalogChannel?.DefaultTrigSensitivity ?? 50;

            uint Edge_RorF = 0x0000;//沿选择，rise为00，fall为01,any为10
            uint Enable_mask = 0x0000;//使能掩码，开启为1，关闭为0
            var CurrContion_Enable = Hd.UIMessage!.Trigger!.TrigMultiChnlOr!.Condition_Enable;
            if (CurrContion_Enable != null)
            {
                if (CurrContion_Enable[0].Condtion == EdgeSlope.Rise)
                    Edge_RorF = 0;
                else if (CurrContion_Enable[0].Condtion == EdgeSlope.Fall)
                    Edge_RorF = 1;
                else
                    Edge_RorF = 2;
                for (int index = 0; index < 4; index++)//此循环次数应与实际通道数一致。
                {
                    if (CurrContion_Enable[index].Enable == true)
                        Enable_mask |= 1U << (index);
                }
            }
            if (Edge_RorF != 1)//rise or any 
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, (uint)compareLevel - defaultTrigSensitivity);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, (uint)compareLevel);

                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, (uint)compareLevel);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, (uint)compareLevel - defaultTrigSensitivity);
            }
            else //fall
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, (uint)compareLevel);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, (uint)compareLevel + defaultTrigSensitivity);

                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, (uint)compareLevel + defaultTrigSensitivity);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, (uint)compareLevel);
            }


            UInt32 CtrlWordsMultiOr = 0x0000;
            CtrlWordsMultiOr = ((Edge_RorF << 8) | (Enable_mask)) & 0xFFFF;

            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_MultiChOrSet, CtrlWordsMultiOr);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_MultiChOrSet, CtrlWordsMultiOr);
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_MultiChOrSet, 0x0103);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_MultiChOrSet, 0x0103);

        }
    }
           
}
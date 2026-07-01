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
        /// <summary>
        /// 边沿触发的相关配置
        /// </summary>
        internal static void Config_Edge()//????
        {
            Int32 baseline = 1 << (Constants.ADC_BITS - 1);
            ChannelId source = Hd.UIMessage?.Trigger?.Edge?.Source ?? ChannelId.C1;
            if (source == ChannelId.Ext)
                return;
            EdgeSlope slope = Hd.UIMessage?.Trigger?.Edge?.Slope ?? EdgeSlope.Rise;
            UInt32 compareLevel = AbstractAcquirer_AnalogChannel.GetLevelByVoltage(source, Hd.UIMessage?.Trigger?.Edge?.Position ?? 0);
            UInt32 trig_ACDC_read = 0;
            switch (source)
            {
                case (ChannelId.C1):
                    //trig_ACDC_read = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.TrigCtrl_AcDcValueRead0, AcqBdNo.B7);
                    break;
                case (ChannelId.C2):
                    //trig_ACDC_read = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.TrigCtrl_AcDcValueRead0, AcqBdNo.B9);
                    break;
                case (ChannelId.C3):
                    //trig_ACDC_read = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.TrigCtrl_AcDcValueRead0, AcqBdNo.B10);
                    break;
                case (ChannelId.C4):
                    //trig_ACDC_read = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.TrigCtrl_AcDcValueRead0, AcqBdNo.B12);
                    break;
                default:
                    //trig_ACDC_read = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.TrigCtrl_AcDcValueRead0, AcqBdNo.B7);
                    break;
            }

            UInt16 trig_ACDC_value = (ushort)(trig_ACDC_read & 0x7FFF);

            UInt32 defaultTrigSensitivity = Hd.CurrProduct?.Acquirer_AnalogChannel?.DefaultTrigSensitivity ?? 50;


            Int32 trig_AC_temp = 0;
            UInt32 couple_para = 0;
            if (trig_ACDC_value > baseline)
            {
                trig_AC_temp = (Int16)trig_ACDC_value - baseline;
            }
            else
            {
                trig_AC_temp = baseline - (Int16)trig_ACDC_value;
            }


            TriggerCoupling counpling = Hd.UIMessage?.Trigger?.Edge?.Coupling ?? TriggerCoupling.DC;
            switch (counpling)
            {
                case TriggerCoupling.LFR:
                    couple_para = 3;
                    break;
                case TriggerCoupling.HFR:
                    couple_para = 9;
                    break;
            }
            UInt32 trig_coup_sel = (couple_para << 8) | ((UInt32)counpling) & 0xffff;


            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelectPro, (uint)source); // todo:触发源的选择
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelPro, (uint)0); // todo:触发源的选择
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SourceSelect, (uint)source);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 0);


            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_AcDcSet0, (uint)trig_coup_sel);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_FreqRejSet0, (uint)trig_coup_sel);

            if (counpling == TriggerCoupling.LFR | counpling == TriggerCoupling.HFR)
            {
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_FreqRejSetPro, (uint)0x0001);
            }
            else
            {
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_FreqRejSetPro, (uint)0x0000);
            }



            if (counpling == TriggerCoupling.AC & (trig_ACDC_value < baseline))
            {
                if (slope == EdgeSlope.Rise)
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, 0);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_EdgeSelect, 1);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, compareLevel - (UInt16)trig_AC_temp - defaultTrigSensitivity);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, compareLevel - (UInt16)trig_AC_temp);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, compareLevel - (UInt16)trig_AC_temp);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, compareLevel - (UInt16)trig_AC_temp - defaultTrigSensitivity);
                }
                else
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, 1);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_EdgeSelect, 0);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, compareLevel - (UInt16)trig_AC_temp);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, compareLevel - (UInt16)trig_AC_temp + defaultTrigSensitivity);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, compareLevel - (UInt16)trig_AC_temp + defaultTrigSensitivity);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, compareLevel - (UInt16)trig_AC_temp);
                }
            }
            else if ((UInt16)counpling == 1 & (trig_ACDC_value >= baseline))
            {
                if (slope == EdgeSlope.Rise)
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, 0);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_EdgeSelect, 1);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, compareLevel + (UInt16)trig_AC_temp - defaultTrigSensitivity);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, compareLevel + (UInt16)trig_AC_temp);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, compareLevel + (UInt16)trig_AC_temp);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, compareLevel + (UInt16)trig_AC_temp - defaultTrigSensitivity);
                }
                else
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, 1);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_EdgeSelect, 0);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, compareLevel + (UInt16)trig_AC_temp);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, compareLevel + (UInt16)trig_AC_temp + defaultTrigSensitivity);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, compareLevel + (UInt16)trig_AC_temp + defaultTrigSensitivity);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, compareLevel + (UInt16)trig_AC_temp);
                }
            }
            else
            {
                if (slope == EdgeSlope.Rise)
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, 0);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_EdgeSelect, 1);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, compareLevel - defaultTrigSensitivity);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, compareLevel);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, compareLevel);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, compareLevel - defaultTrigSensitivity);
                }
                else
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, 1);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_EdgeSelect, 0);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, compareLevel);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, compareLevel + defaultTrigSensitivity);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, compareLevel + defaultTrigSensitivity);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, compareLevel);
                }
            }
        }

        internal static void Config_Edge_MultiQulified(ITriggerTypeOptions? triggerTypeOptions, int eventIndex)
        {
            if (triggerTypeOptions == null)
                return;
            var edgeOption = (TrigEdgeOptions)triggerTypeOptions;
            ChannelId trigSource = edgeOption.Source;
            EdgeSlope slope = edgeOption.Slope;
            UInt32 CompareVoltage = AbstractAcquirer_AnalogChannel.GetLevelByVoltage(trigSource, edgeOption.Position);
            UInt32 defaultTrigSensitivity = Hd.CurrProduct?.Acquirer_AnalogChannel?.DefaultTrigSensitivity ?? 50;

            if (slope == EdgeSlope.Rise)
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, 0);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_EdgeSelect, 1);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, CompareVoltage - defaultTrigSensitivity);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, CompareVoltage);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, CompareVoltage);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, CompareVoltage - defaultTrigSensitivity);
            }
            else
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, 1);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_EdgeSelect, 0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, CompareVoltage);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, CompareVoltage + defaultTrigSensitivity);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, CompareVoltage + defaultTrigSensitivity);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, CompareVoltage);
            }
        }//????
    }
}

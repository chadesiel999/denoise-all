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
        internal static void Config_Window()//????
        {
            int adcInterleaveMode = (int)Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode;
            ChannelId trigSource = Hd.UIMessage?.Trigger?.Window?.Source ?? ChannelId.C1;
            UInt32 CompareVoltageL = AbstractAcquirer_AnalogChannel.GetLevelByVoltage(trigSource, Hd.UIMessage?.Trigger?.Window?.LowerPosition ?? 0);
            UInt32 CompareVoltageH = AbstractAcquirer_AnalogChannel.GetLevelByVoltage(trigSource, Hd.UIMessage?.Trigger?.Window?.UpperPosition ?? 0);
            WindowRange WRange = Hd.UIMessage?.Trigger?.Window?.Range ?? WindowRange.Inside;
            long WidthByps = Hd.UIMessage?.Trigger?.Window?.WidthByps ?? 0;
            if (WRange == WindowRange.Inside)
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, CompareVoltageL - 150);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, CompareVoltageL);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Down, CompareVoltageH);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Up, CompareVoltageH + 150);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, CompareVoltageL - 150);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, CompareVoltageL);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Down, CompareVoltageH);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Up, CompareVoltageH + 150);
            }
            else
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, CompareVoltageL);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, CompareVoltageL + 150);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Down, CompareVoltageH - 150);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Up, CompareVoltageH);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, CompareVoltageL);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, CompareVoltageL + 150);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Down, CompareVoltageH - 150);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Up, CompareVoltageH);
            }

            UInt32 TrigSet = (UInt32)WRange;
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_WindowsSetAcq, TrigSet);
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_WindowsSetPro, TrigSet);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_AdvWindowSet, TrigSet);
            UInt64 extramNum = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.ExtractNumFromAdc ?? 1;
            UInt64 interpNum = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.InterplotNumToDMA ?? 1;
            UInt32 InterplotNumPre = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.InterplotNumFromADC ?? 1;
            Int64 firstWidth = WidthByps / 200;//除以200ps，将以ps为单位的建立时间参数变为以200ps为单位的控制字
            double dWidthByps = WidthByps;
            Int64 firstWidth_2nd = (Int64)(dWidthByps / 50 / extramNum * interpNum * InterplotNumPre);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_Width1LAcq, (uint)firstWidth & 0xffff);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_Width1HAcq, (uint)(firstWidth >> 16) & 0xffff); ;
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_Width1LPro, (uint)firstWidth & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_Width1HPro, (uint)(firstWidth >> 16) & 0xffff);
            //trig_2nd
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width1SetL, (uint)firstWidth_2nd & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width1SetM, (uint)(firstWidth_2nd >> 16) & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width1SetH, 0);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 0);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelectPro, (uint)trigSource);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SourceSelect, (uint)trigSource);


        }
    }
}
#endif

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
        internal static uint GetTriggerSource_SetupHold()
        {
            uint trigsource;
            uint clkSource = (uint)Hd.UIMessage!.Trigger!.SetupHold?.ClkSource;
            uint dataSource = (uint)Hd.UIMessage!.Trigger!.SetupHold?.DataSource;
            uint violation = (uint)Hd.UIMessage!.Trigger!.SetupHold?.Violation;
            if (violation == 0)
                trigsource = clkSource;
            else
                trigsource = dataSource;
            return trigsource;
            //return (uint)(Hd.UIMessage?.Trigger?.SetupHold?.ClkSource ?? ChannelId.C1);
        }
        //使用internal  是为了共用
        internal static void Config_SetupHold()
        {
            if (Hd.UIMessage!.Trigger!.SetupHold == null)
                return;
            uint trigSource = Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
            uint clkSource = (uint)Hd.UIMessage!.Trigger!.SetupHold?.ClkSource;
            uint dataSource = (uint)Hd.UIMessage!.Trigger!.SetupHold?.DataSource;
            HdMessage.AnalogOptions analogParametersClk = Hd.UIMessage!.Analog![clkSource];
            HdMessage.AnalogOptions analogParametersData = Hd.UIMessage!.Analog![dataSource];
            uint trigType = (uint)Hd.UIMessage?.Trigger?.TrigType;//触发类型
            uint clkPolarity = (uint)Hd.UIMessage!.Trigger!.SetupHold?.ClkPolarity;
            uint datapattern = (uint)Hd.UIMessage!.Trigger!.SetupHold?.DataPosPolarity;
            uint clkPosition = (uint)((Constants.SAMPS_PER_YDIV * (Hd.UIMessage!.Trigger!.SetupHold?.ClkPosition.Value + analogParametersClk.Position) / analogParametersClk.Scale) + AbstractController_Trigger.AdcCenterValue);
            uint dataLUpperPosition = (uint)((Constants.SAMPS_PER_YDIV * (Hd.UIMessage.Trigger!.SetupHold?.DataUpperPosition.Value + analogParametersData.Position) / analogParametersData.Scale) + AbstractController_Trigger.AdcCenterValue);
            
            uint violation = (uint)Hd.UIMessage!.Trigger!.SetupHold?.Violation;
            Int64 thdByps = (uint)Hd.UIMessage!.Trigger!.SetupHold?.ThdByps / (400 * 2 * 4);
            Int64 tsuByps = (uint)Hd.UIMessage!.Trigger!.SetupHold?.TsuByps / (400 * 2 * 4);
            positionsel(clkPosition, clkSource);
            positionsel(dataLUpperPosition, dataSource);
            Int64 time;
            if (violation == 0)
                time = tsuByps; 
            else
                time = thdByps;
            if (datapattern == 0)
                datapattern = 1;
            else
                datapattern = 0;
            uint edgeselect;
            if (violation == 0)
                edgeselect = clkPolarity;
            else
                edgeselect = datapattern;
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, edgeselect);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 9);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinL16, (uint)time & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgDelayTimeMinH16, (uint)(time >> 16) & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgTrig_A_Sel, datapattern << 5 | dataSource);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgTrig_B_Sel, clkPolarity << 5 | clkSource);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrgPwFuncSel, violation);
        }

        internal static void positionsel(uint Position, uint Source)
        {
            switch (Source)//低电平
            {
                case (uint)ChannelId.C1:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level2, (uint)(Position));
                    break;
                case (uint)ChannelId.C2:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh2Level2, (uint)(Position));
                    break;
                case (uint)ChannelId.C3:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh3Level2, (uint)(Position));
                    break;
                case (uint)ChannelId.C4:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh4Level2, (uint)(Position));
                    break;

            }
        }
    }
}
#endif
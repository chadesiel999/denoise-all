using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal class Command_Misc : IAppendCommandTable
    {
        private void SendRunStop()
        {
            if (Hd.UIMessage!.Timebase!.IsScan && Hd.UIMessage!.bAcquireStopped)
            {
                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.ScanCtrl_CountLatchEn, 0x00);
                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.ScanCtrl_CountLatchEn, 0x01);

                //Read position of the DDR (need to include the data length of the FIFO residue)
                UInt32 fifocount = Hd.CurrProduct.AcqBd.ReadReg(AcqBdReg.R.ScanCtrl_WrDataCount, AcqBdNo.B0) - 2;//HL:植树问题，少取2个点
                HdCtrl_AnalogFifo.FreshDdrReadPos(fifocount);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Trig_Scan_Trig_AddrH16, HdCtrl_AnalogFifo.DdrReadPos >> 16); //hight16
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Trig_Scan_Trig_AddrL16, HdCtrl_AnalogFifo.DdrReadPos & 0xffff); //low16

                //重新计算显示数据的长度
                var ratio = 64 / (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.AdcInterleaveMode == AdcInterleaveMode.Mode2To1 ? 1 : 2);
                Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay += fifocount * ratio * Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.Scan2ExtractNum_Total;
                if(Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay > Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.HardwareStorageWaveDotsCnt)
                    Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.HardwareStorageWaveDotsCnt;

                Acquisition.ScanRunningNewDataPerChannelExistsDotCount = 0;
                Acquisition.ScanPerChannelInDdrDotCount_NotDisplay = 0;
            }
            //设置软暂停
            if (Hd.UIMessage?.bAcquireStopped ?? false)
            {
                HdIO.WriteReg(ProcBdReg.W.DataPath_SoftStopPro, 1);
                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_SoftStopAcq, 1);
            }
            else
            {
                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_SoftStopAcq, 0U);
                HdIO.WriteReg(ProcBdReg.W.DataPath_SoftStopPro, 0U);
            }

            //Hd.CurrProduct!.AcqBd!.ExecMiscFunc("SendChMode_SamplingMode");
        }
        public Dictionary<HdCmd, Action[]> AppendCommand()
        {
            return new Dictionary<HdCmd, Action[]>()
            {
                #region Other
                [HdCmd.Run] = new Action[] { SendRunStop },
                [HdCmd.Combo] = new Action[] { Hd.NullAction },
                //[HdCmd.AdjustAnaChnlGain] = new Action[] { AbstractController_AnalogChannel.AdjustGainByUI },

                #endregion
            };
        }
    }
}

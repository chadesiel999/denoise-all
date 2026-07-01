using ScopeX.ComModel;
using ScopeX.Hardware.Driver.Registers.SendManage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 该类主要用来设置Fifo采集流程的初始化，传输，采集
    /// </summary>
    internal class HdCtrl_AnalogFifo
    {
        private static UInt32 _DdrReadPos = 0;
        //DDR的读取地址,当Scan档暂停时使用
        internal static UInt32 DdrReadPos
        {
            get => _DdrReadPos;
            private set
            {
                _DdrReadPos = value;
            }
        }

        /// <summary>
        /// 刷新当前DDR的读取地址
        /// </summary>
        /// <param name="fifoCount">fifo的计数数量</param>
        internal static void FreshDdrReadPos(UInt32 fifoCount)
        {
            UInt32 fifoddrratio = (UInt32)Hd.CurrProduct.Acquirer_AnalogChannel.AcquedParameters.Scan2ExtractNum_Total;
            UInt32 ddrtotallength = 0x8000_0000;//当前使用的ddr深度=2G
            DdrReadPos = (UInt32)((DdrReadPos + fifoCount * fifoddrratio * 4) % (ddrtotallength / 16));
        }

        /// <summary>
        /// 配置Fifo采数
        /// </summary>
        internal static void ConfigWrite()
        {
            //Fifo相关设置
            #region 路径选择
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DataPath_acq_linkmux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkmux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkdemux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            #endregion

            //set 0 before acquiring
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
            //disable digital trig
            ConditionManager.TriggerCtrlEn = false;
            ConditionManager.IsFromDDR = false;
            RegSendManager.Default.Send((UInt32)AcqBdReg.W.TrigCtrl_DigitalTrigEn);
            RegSendManager.Default.Send((UInt32)ProcBdReg.W.TrigCtrl_DigitalTrigEn_Pro);

            //TestMode sim/acq data
            //HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_2, 0x1);
            //HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve10, 0x0);    //reg_debug_single_mode
            //HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve11, 0x30);   //reg_syncfifo_epmty
            //HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve12, 0x2000); //reg_sycnfifo_full
            //HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve13, 0x1);    //reg_sync_fifo_data_mode
            //HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve14, 0x2000); //ti_cross_fifo_full
            //reset the ddr read pos
            DdrReadPos = 0;
        }

        /// <summary>
        /// Fifo是否采到数
        /// </summary>
        /// <returns></returns>
        internal static Boolean CanRead()
        {
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.ScanCtrl_CountLatchEn, 0x00);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.ScanCtrl_CountLatchEn, 0x01);

            //计算数据量：每个通道的采样点数 = AcqBdReg.R.ScanCtrl_WrDataCount * 64 / (是否为20G模式 ? 1 : 2)
            int scancount = (int)Hd.CurrProduct.AcqBd.ReadReg(AcqBdReg.R.ScanCtrl_WrDataCount, AcqBdNo.B0);
            var mergemode = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.AdcInterleaveMode;
            Acquisition.ScanRunningNewDataPerChannelExistsDotCount = scancount  * 64 / (mergemode == AdcInterleaveMode.Mode4To1 ? 1 : 2);
            Trace.WriteLine($"Scan 64 Data Length {Acquisition.ScanRunningNewDataPerChannelExistsDotCount}");
            Trace.WriteLine($"Scan Data Length {scancount}");
            //Acquisition.ScanRunningNewDataPerChannelExistsDotCount = scancount * 64 / (2);
            //if (scancount == 1020)
            //{
            //    int a = 1;
            //}


            Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay += Acquisition.ScanRunningNewDataPerChannelExistsDotCount * Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.Scan2ExtractNum_Total;
            if (Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay > Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.HardwareStorageWaveDotsCnt)
                Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.HardwareStorageWaveDotsCnt;
            return Acquisition.ScanRunningNewDataPerChannelExistsDotCount != 0;
        }

        /// <summary>
        /// 配置从Fifo里面读取
        /// </summary>
        /// <param name="readingParams"></param>
        internal static void ConfigRead(ReadParams readingParams)
        {
            var mergemode = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.AdcInterleaveMode;
            var acqdatareg = (UInt32)(readingParams.PerChannelRecvDotsCount * (mergemode == AdcInterleaveMode.Mode1To1 ? 2 : 1) / 64);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.ScanCtrl_RdDataCount, acqdatareg);
            FreshDdrReadPos(acqdatareg);

            /*后抽系数下发*/
            //把DDR使用的后抽系数置1
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.Decimation_PosGapx, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PosGapValuelL16, 1);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PosGapValueH16, 0);

            //强制打开峰值抽取
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PosDecimationMode, 1U);
        }
    }
}

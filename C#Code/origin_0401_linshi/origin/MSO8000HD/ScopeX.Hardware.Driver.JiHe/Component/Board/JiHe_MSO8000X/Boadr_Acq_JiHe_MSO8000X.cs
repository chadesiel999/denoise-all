using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Ivi.Visa.FormattedIO.Parser;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Driver.Module;
using ScopeX.Hardware.Driver.Registers.SendManage;
using ScopeX.MathExt;
using ScopeX.Measure;
using static ScopeX.ComModel.HdMessage;

namespace ScopeX.Hardware.Driver
{
    internal class Boadr_Acq_JiHe_MSO8000X : AbstractAcqBd
    {
        public Boadr_Acq_JiHe_MSO8000X((String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC)[] FpgaExistsConfig) : base(FpgaExistsConfig) { }

        public Dictionary<AdcConfigDataType, List<AdcAlreadySendData>> Adc5200AlreadySendData = new Dictionary<AdcConfigDataType, List<AdcAlreadySendData>>();
        public override void ClearSendHistory()
        {
            Adc5200AlreadySendData.Clear();
            AdcAlreadySendDataManager.Default.ClearSendHistory();
            Hd.CurrProduct?.Acquirer_AnalogChannel?.ClearSendHistory();
            //CoefficientsTableSender_DBI.ClearSendHistory();
        }

        /// <summary>
        /// 完成基本配置
        /// </summary>
        public override void Init()
        {
            //采集系统传输回路复位成正常模式(非2级传输模式)
            WriteToAllFpga(AcqBdReg.W.FlashOperator_ActionCode, 0);

            //读使能复位
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);

            //临时调试，测试UART误码率
            //var testErrTimes = Hd.CurrProduct?.AcqBd?.UartTest2(AcqBdReg.W.Decimation_PosGapValueH16, 20);

            //临时代码， 触发延迟
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x0A);

            //临时代码， 双板同步触发选择
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x0);


            //临时测试代码，采集板和处理版初始化
            WriteToAllFpga(AcqBdReg.W.TrigCtrl_TestDataMode, 0x01);
            HdIO.WriteReg(ProcBdReg.W.debug_pro_debug_mode, 0x00);
            //临时测试代码，PCIE
            HdIO.WriteReg((uint)PcieBdReg.W.Xdma_XdmaIrqReset, 0);
            HdIO.WriteReg((uint)PcieBdReg.W.Xdma_XdmaIrqReset, 1);

            HdIO.WriteReg((uint)AcqBdReg.W.Adc_FD10BufferPowerEn, 0);
            InitAll5200At20GMode();
            HdIO.WriteReg((uint)AcqBdReg.W.Adc_FD10BufferPowerEn, 1);
            //
            //_0331
            HdIO.Sleep(100);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Adc_AdcCardPowerEnable, 0x05);
  //          Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_3, 0x01);//Open FD //cij

            HdIO.Sleep(5);

            WriteToAllFpga(AcqBdReg.W.ChMode_SamplingMode, Hd.CurrProduct?.HardwareConfig?.Default_AcqBoardCH_MODE_SamplingMode ?? 0x40);
            HdIO.WriteReg(ProcBdReg.W.ScanCtrl_SamplingMode, Hd.CurrProduct?.HardwareConfig?.Default_AcqBoardCH_MODE_SamplingMode ?? 0x40);

            WriteToAllFpga(AcqBdReg.W.DataPath_acq_linkmux_select, 0);
            Hd.Calibration.BoardInteractionDelay_DoAllCali(ExistsDefines);
            ReadFpgaVersion();
/*
            //zsx
            HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_2, 0x1);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_2, 0x1);
            HdIO.Sleep(1);
            HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_2, 0x1);
*/
        }

        #region SampleClockPhase
        private static readonly UInt32[] _HdPrefix =
        {
            0x0107,
            0x00F3,
            0x00DF,
            0x00CB,
            0x0107,
            0x00F3,
            0x00DF,
            0x00CB
        };
        private const int nominal_offset = 0;
        private static readonly Int32[] _SmpClkDelay =
        {
            (150 - nominal_offset) / 25,
            (100 - nominal_offset) / 25,
            (100 - nominal_offset) / 25,
            (75 - nominal_offset) / 25,
            (150 - nominal_offset) / 25,
            (100 - nominal_offset) / 25,
            (100 - nominal_offset) / 25,
            (75 - nominal_offset) / 25,
        };
        private UInt32 GetHdWord(Int32 index, Int32 delta)
        {
            Int32 data = _SmpClkDelay[index];
            data += delta;
            if (data < 0)
                data = 0;
            else if (data > 23)
                data = 23;
            return (_HdPrefix[index] << 8) | (UInt32)data;
        }
        private void AdjustSampleClockPhase()
        {
            uint addr, data;
            for (int i = 0; i < 4; i++)
            {
                addr = GetHdWord(i, 0) & 0xffffff00;
                addr >>= 8;
                data = GetHdWord(i, 0) & 0xff;
                PllWrite(AcqBdNo.B2, addr, data);
                PllWrite(AcqBdNo.B1, addr, data);
                HdIO.Sleep(1);
            }

            for (int i = 4; i < 8; i++)
            {
                addr = GetHdWord(i, 0) & 0xffffff00;
                addr >>= 8;
                data = GetHdWord(i, 0) & 0xff;
                PllWrite(AcqBdNo.B1, addr, data);
                HdIO.Sleep(1);
            }
        }
        #endregion SampleClockPhase

        #region 5200
        private void AD5200_Init(int anaChannelID, AcqBdNo fpgaIndex, int SubbandIndex, int adcIndex, int adcSigalInputPort)
        {
            WriteReg(AcqBdReg.W.Adc_CS, fpgaIndex, (UInt32)(adcIndex + 1));
            //SOFT RESET
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0000, 0xB0);
            HdIO.Sleep(20);
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0000, 0x30);
            //SET CALI EN 校准要在设置链路之前

            //SPI config
            //SendCmdToAD5200(fpgaIndex, adcIndex, 0x0010, 0x01);//3200

            //stop the JESD204B state machine, stop the calibration state machine 
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0200, 0x00); //JESD_EN diable
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0061, 0x00); //CALI_EN diable
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x02B0, 0x00); //sysref cali diable
                                                                        //MISCELLANEOUS ANALOG REGISTERS
                                                                        //使用校准数据
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x002B, 0x15); //
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x02A2, 0x18); //

            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x002A, 0x00); //SYSREF_LVPECL_EN=0,DEVCLK_LVPECL_EN=0, INVERTED//00
            //SendCmdToAD5200(adcIndex, 0x002A, 0x22); //SYSREF_LVPECL_EN=0,DEVCLK_LVPECL_EN=0, INVERTED
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0038, 0x00); //BG_BYPASS
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x003B, 0x00); //not use TMSTP± input,TMSTP_LVPECL_EN=0
                                                                        //SERIALIZER REGISTERS
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0048, 0x04); //Serializer Pre-Emphasis Control

            ////////////////////FS_RANGE_A////////////////////////////////
            //SendCmdToAD5200(adcIndex, 0x0030, 0xFF);//0x2000:500mVPP ,0xA000:800mVPP(default) , 0xffff:1000mVPP
            //SendCmdToAD5200(adcIndex, 0x0031, 0x2A);
            ////////////////////FS_RANGE_B////////////////////////////////
            //cij_new
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0032, 0xff); //0x2000:500mVPP ,0xA000:800mVPP(default) , 0xffff:1000mVPP
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0033, 0xff);//0xa0

            //SET jesd204 
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0201, 0x01); //JMODE =  single channel 10GSPS
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0202, 0x1F); //JMODE,K=32 frame number in one multifrane
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0203, 0x01); //SEL normal sync operation
            //SendCmdToAD5200(fpgaIndex, adcIndex, 0x0204, 0x03); //use SYNCSE pin for sync //2's complement // 8B/10B scrambler disable
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0204, 0x01); //use SYNCSE pin for sync //offset binary // 8B/10B scrambler disable
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0205, 0x00); //adc test mode , ramp test = 0x04,Transport layer test mode:05
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0206, 0x00); // DID: device identifier
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0207, 0x00); //COMMA CHAR = K28.5 //3200写了0x00
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0209, 0x00); //A ADC channel & B ADC channelis powered up
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x020A, 0x00); //Only the link A layer clocks for extra lanes are enabled
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x020B, 0x00); //Only the link B layer clocks for extra lanes are enabled

            //CALIBRATION REGISTERS
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0060, 0x02);//1: INA± is used;2: INB± is used;

            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0062, 0x05);//CAL_CFG0,Disable all calibration
                                                                       //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x0068, 0x61);//CAL_AVG default averaging amount 
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0068, 0x77);//CAL_AVG MAX averaging amount 
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x006B, 0x06);//Use the CAL_SOFT_TRIG register for the calibration trigger,CALSTAT output pin is always low
                                                                       //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x006C, 0x01);//CAL_SOFT_TRIG = 1 
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x006E, 0x88);//Disables low-power background calibration (default)
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0070, 0x00);//CAL_DATA_EN=0
                                                                       //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x0071, 0x00);//CAL_DATA=0
                                                                       //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x007A, 0x00);//GAIN_TRIM_A=0
                                                                       //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x007B, 0x00);//GAIN_TRIM_B=0
                                                                       //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x007C, 0x00);//BG_TRIM=0
                                                                       //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x007E, 0x00);//RTRIM_A=0
                                                                       //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x007F, 0x00);//RTRIM_B=0


            //SYSREF CALIBRATION REGISTERS
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x02B1, 0x05);//SRC_HDUR=01,SRC_AVG=01
                                                                       //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x02B0, 0x01);//SYSREF calibration enables SRC_EN

            //Program CAL_EN = 1 to enable the calibration state machine
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0061, 0x01); //CALI_EN able
                                                                        //LSB CONTROL REGISTERS
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0160, 0x00); //TIMESTAMP_EN=0
                                                                        //ADC BANK REGISTERS

            // Program JESD_EN = 1 to re-start the JESD204B state machine and allow the link to restart
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0200, 0x01); //JESD_EN able

            HdIO.Sleep(100);
            //phase-offset-gain 校准数据
            //AdjustAdc_Phase(false, SubbandIndex, fpgaIndex, adcIndex);
            //此处与其他项目不同。与通道无关。不使用通道号，而是使用FPGAIndex
            //此时的校准数据的组织的含义与其他项目也不一样。由于10个采集卡全部插满，此时的子带号与FPGAIndex的号是一致的。

            //AdjustAdc_Phase(false, (int)fpgaIndex, fpgaIndex, adcIndex);

            //AdjustAdc_Offset(anaChannelID, fpgaIndex, adcIndex);
            //原来在此处同时调整ADCGain，在上电初始化时没有通道信息，故不能再在此进行设置。
            //AdjustAdc_Gain(false, anaChannelID, SubbandIndex, fpgaIndex, adcIndex);

            #region 压稳态窗 SyncSampleClock
            //uint data = TiAdc_SyncSampleClock.Default[anaChannelID][adcIndex].SampleClockDelay & 0x0f;//只有低4位有效
            //此处与其他项目不同，此处不使用通道号而是子带号，此项目的 子带号与FPGAIndex是一致的。
            //此时的校准数据的组织的含义与其他项目也不一样。由于10个采集卡全部插满，此时的子带号与FPGAIndex的号是一致的。
            uint data = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!.InterleaveMode == AdcInterleaveMode.Mode2To1 ? TiAdc_SyncSampleClock.Default[(int)fpgaIndex][adcIndex].Sample20GClockDelay & 0x0f : TiAdc_SyncSampleClock.Default[(int)fpgaIndex][adcIndex].Sample10GClockDelay & 0x0f;//只有低4位有效
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0029, 0x30 | data);//use SYSREF calibration,delay steps are finer,enable the SYSREF receiver circuit
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0029, 0x70 | data);//SYSREF_RECV_EN must be set before setting SYSREF_PROC_EN
            #endregion

            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x7A, 1);//打开增益可调

            WriteReg(AcqBdReg.W.Adc_CS, fpgaIndex, 0x00);
        }
        private Int32 GetMaxLengthZeroMid(uint source, uint Length)
        {
            uint data = 0;
            int j = 0;
            List<List<Int32>> zeroLists = new List<List<Int32>>();
            for (int i = 0; i < Length; i++)
            {
                i = j + 1;
                if (i == Length)
                    break;
                List<Int32> zeros = new List<Int32>();
                for (j = i; j < Length; j++)
                {
                    data = (uint)(source & (0x0001 << j));
                    if (data == 0)
                    {
                        zeros.Add(j + 1);
                    }
                    else
                    {
                        zeroLists.Add(zeros);
                        break;
                    }
                }
            }

            Int32 Maxlength = 0;
            Int32 Maxindex = -1;
            for (int i = 0; i < zeroLists.Count; i++)
            {
                if (zeroLists[i].Count > Maxlength)
                {
                    Maxlength = zeroLists[i].Count;
                    Maxindex = i;
                }
            }
            if (Maxindex != -1)
            {
                return zeroLists[Maxindex][0] + (Int32)Math.Ceiling((Maxlength / 2.0));
            }
            else
            {
                return -1;  //no zero
            }
        }
        internal void SendCmdToAD5200(Int32 adcIndex, UInt32 Address_15bit, UInt32 Commmand_8bit)//Address15bit,Commmand 8bit
        {
            UInt32 tmp = ((0x000 << 23) | (Address_15bit << 8) | Commmand_8bit);//(0x001 << 23)Instruction R(1b'1)/W(1b'0)

            WriteToAllFpga(AcqBdReg.W.Adc_DataCmdL8, tmp & 0xffff);
            WriteToAllFpga(AcqBdReg.W.Adc_DataCmdH16, (tmp >> 8) & 0xffff);
            WriteToAllFpga(AcqBdReg.W.Adc_ConfigEnable, 0xc0);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(1);
            WriteToAllFpga(AcqBdReg.W.Adc_ConfigEnable, 0xe0);
            HdIO.Sleep(1);
        }
        internal static void SendCmdToAD5200_OneFpga(AcqBdNo fpagIndex, Int32 adcIndex, UInt32 Address_15bit, UInt32 Commmand_8bit)//Address15bit,Commmand 8bit
        {
            HdDebugLogger.Log($"[{DateTime.Now}]: ThreadId = {Thread.CurrentThread.ManagedThreadId},AcqBd = {fpagIndex},AdcId = {adcIndex}," +
                $"Addr = 0x{Address_15bit.ToString("x")},Value = 0x{Commmand_8bit.ToString("x")}");
            UInt32 tmp = ((0x000 << 23) | (Address_15bit << 8) | Commmand_8bit);//(0x001 << 23)Instruction R(1b'1)/W(1b'0)

            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Adc_DataCmdL8, fpagIndex, tmp & 0xffff);
            HdIO.Sleep(1);
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Adc_DataCmdH16, fpagIndex, (tmp >> 8) & 0xffff);
            HdIO.Sleep(1);
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Adc_ConfigEnable, fpagIndex, 0xc0);
            HdIO.Sleep(1);
            HdIO.WaitForSpiTransfer(1, 5);
            HdIO.Sleep(1);
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Adc_ConfigEnable, fpagIndex, 0xe0);
            HdIO.Sleep(10);
        }
        internal void Send5200CmdWithCS(Int32 adcIndex, UInt32 Address_15bit, UInt32 Commmand_8bit)
        {
            WriteToAllFpga(AcqBdReg.W.Adc_CS, (UInt32)(adcIndex + 1));
            SendCmdToAD5200(adcIndex, Address_15bit, Commmand_8bit);
            WriteToAllFpga(AcqBdReg.W.Adc_CS, 0x00);
            HdIO.Sleep(1);
        }
        internal static void Send5200CmdWithCS_OneFpga(AcqBdNo fpageIndex, Int32 adcIndex, UInt32 Address_15bit, UInt32 Commmand_8bit)
        {
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Adc_CS, fpageIndex, (UInt32)(adcIndex + 1));
            HdIO.Sleep(1);
            SendCmdToAD5200_OneFpga(fpageIndex, adcIndex, Address_15bit, Commmand_8bit);
            HdIO.Sleep(1);
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Adc_CS, fpageIndex, 0x00);
            HdIO.Sleep(1);
        }
        #endregion 5200

        #region 7044
        private void HMC7044Write(UInt32 ADDR, UInt32 data)//ADDR's width is 13bits; data's width is 8bits
        {
            UInt32 SDATA;//SDATA's width is 24bits
            UInt32 RorW = 0x0000000 & 0x00E00000;//MSB bit23 R/W bit22(W1) bit21(W0) Multibyte field 2'b00  RorW[23:21]=0|00
            SDATA = RorW | ((ADDR << 8) & 0x001fff00) | (data & 0x000000ff);

            WriteToAllFpga(AcqBdReg.W.PllConfig_HMC7044Data_Effect, 0x00);
            HdIO.Sleep(1);
         //   Hd.CurrProduct.AcqBd.WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_L16, AcqBdNo.B0, SDATA & 0xffff);
            WriteToAllFpga(AcqBdReg.W.PllConfig_HMC7044Data_L16, SDATA & 0xffff);
            HdIO.Sleep(1);
        //    Hd.CurrProduct.AcqBd.WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_H8, AcqBdNo.B0, (SDATA >> 16) & 0xffff);
            WriteToAllFpga(AcqBdReg.W.PllConfig_HMC7044Data_H8, (SDATA >> 16) & 0xffff);
            HdIO.Sleep(1);
        //    Hd.CurrProduct.AcqBd.WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_Effect, AcqBdNo.B0, 0x01);
            WriteToAllFpga(AcqBdReg.W.PllConfig_HMC7044Data_Effect, 0x01);
            HdIO.Sleep(1);
            HdIO.WaitForSpiTransfer(1, 6);
            HdIO.Sleep(20);
        }

        private void InitHMC7044_Adc5200()
        {
            //ACQ5200_HMC7044 start
            WriteToAllFpga(AcqBdReg.W.PllConfig_7044Reset, 0);
            HdIO.Sleep(10);
            WriteToAllFpga(AcqBdReg.W.PllConfig_7044Reset, 1);
            HdIO.Sleep(10);
            WriteToAllFpga(AcqBdReg.W.PllConfig_7044Reset, 0);
            HdIO.Sleep(10);

            /**********register_config***********/
            HMC7044Write(0x0000, 0x01);//软复位
            HMC7044Write(0x0000, 0x00);
            HMC7044Write(0x0001, 0x40);//全局请求和模式控制
            HMC7044Write(0x0002, 0x04);//PLL2 autotune triger
            HMC7044Write(0x0003, 0x37);//全局使能控制
            HMC7044Write(0x0004, 0x7F);
            HMC7044Write(0x0005, 0x98); // SYNC MODE (82、58) 98???、/
            HMC7044Write(0x0006, 0x00);
            HMC7044Write(0x0007, 0x00);
            HMC7044Write(0x0009, 0x01);
            //-----------------------------//
            // 		保留寄存器
            //---------------------------//
            HMC7044Write(0x0096, 0x00);
            HMC7044Write(0x0097, 0x00);
            HMC7044Write(0x0098, 0x00);
            HMC7044Write(0x0099, 0x00);
            HMC7044Write(0x009A, 0x00);
            HMC7044Write(0x009B, 0xAA);
            HMC7044Write(0x009C, 0xAA);
            HMC7044Write(0x009D, 0xAA);
            HMC7044Write(0x009E, 0xAA);
            HMC7044Write(0x009F, 0x4D);
            HMC7044Write(0x00A0, 0xDF);
            HMC7044Write(0x00A1, 0x97);
            HMC7044Write(0x00A2, 0x03);
            HMC7044Write(0x00A3, 0x00);
            HMC7044Write(0x00A4, 0x00);
            HMC7044Write(0x00A5, 0x06);
            HMC7044Write(0x00A6, 0x1C);
            HMC7044Write(0x00A7, 0x00);
            HMC7044Write(0x00A8, 0x06);
            HMC7044Write(0x00A9, 0x00);
            HMC7044Write(0x00AB, 0x00);
            HMC7044Write(0x00AC, 0x20);
            HMC7044Write(0x00AD, 0x00);
            HMC7044Write(0x00AE, 0x08);
            HMC7044Write(0x00AF, 0x50);
            HMC7044Write(0x00B0, 0x04);
            HMC7044Write(0x00B1, 0x0D);
            HMC7044Write(0x00B2, 0x00);
            HMC7044Write(0x00B3, 0x00);
            HMC7044Write(0x00B5, 0x00);
            HMC7044Write(0x00B6, 0x00);
            HMC7044Write(0x00B7, 0x00);
            HMC7044Write(0x00B8, 0x00);
            //-----------------------------//
            // 		PLL2配置
            //---------------------------//
            HMC7044Write(0x0031, 0x01);
            HMC7044Write(0x0032, 0x01); // DOUBLE R
            HMC7044Write(0x0033, 0x01); // R2 12位R2除法器控制
            HMC7044Write(0x0034, 0x00);
            HMC7044Write(0x0035, 0x19); // N2 16位反馈分频器控制（除以25）
            HMC7044Write(0x0036, 0x00);
            HMC7044Write(0x0037, 0x0F);
            HMC7044Write(0x0038, 0x18);
            HMC7044Write(0x0039, 0x00);
            HMC7044Write(0x003A, 0x00);
            HMC7044Write(0x003B, 0x00);
            //-----------------------------//
            // 		PLL1配置
            //---------------------------//
            HMC7044Write(0x0046, 0x00);//通用输入输出控制GPIO/SDATA control
            HMC7044Write(0x0047, 0x00);
            HMC7044Write(0x0048, 0x08);
            HMC7044Write(0x0049, 0x10);
            HMC7044Write(0x0050, 0x1F);
            HMC7044Write(0x0051, 0x2B);
            HMC7044Write(0x0052, 0x37);
            //HMC7044read(0x0050, 0x7f);
            //HMC7044Write(0x0050, 0x7f);//1F
            //HMC7044Write(0x0051, 0x7f);
            //HMC7044Write(0x0052, 0x7f);

            HMC7044Write(0x0053, 0x33);
            HMC7044Write(0x0054, 0x03);
            //// significant start
            //HMC7044Write(3, 0x005B, 0x00);
            //HMC7044Write(3, 0x005C, 0x80);
            //HMC7044Write(3, 0x005D, 0x00);
            //// significant end
            HMC7044Write(0x0064, 0x00);
            HMC7044Write(0x0065, 0x00);
            HMC7044Write(0x0070, 0xE0); // alarm
            HMC7044Write(0x0071, 0x19);
            HMC7044Write(0x0078, 0x00);
            HMC7044Write(0x0079, 0x00);
            HMC7044Write(0x007A, 0x00);
            HMC7044Write(0x007B, 0x00);
            HMC7044Write(0x007C, 0x00);
            HMC7044Write(0x007D, 0x00);
            HMC7044Write(0x007E, 0x00);
            HMC7044Write(0x0082, 0x00);
            HMC7044Write(0x0083, 0x00);
            HMC7044Write(0x0084, 0x00);
            HMC7044Write(0x0085, 0x00);
            HMC7044Write(0x0086, 0x00);
            HMC7044Write(0x008C, 0x00);
            HMC7044Write(0x008D, 0x00);
            HMC7044Write(0x008E, 0x00);
            HMC7044Write(0x008F, 0x00);
            HMC7044Write(0x0091, 0x00);
            //-----------------------------//
            // 		Sysref Timer
            //---------------------------//
            HMC7044Write(0x005A, 0x01);//07脉冲发生器控制
            HMC7044Write(0x005B, 0x04); // 04 SYNC控制
            HMC7044Write(0x005C, 0x00); // [7:0]LSB sysref计时器控制
            HMC7044Write(0x005D, 0x0a); // [11：8]MSB 2560
            //-----------------------------//
            // 		Clock Output Channel
            //---------------------------//
            // Output Mode Seclect
            // DCLK Mode : 0xF3
            // SYSREF Pluse Mode : 0x5D
            // Turn Off : 0x00
            //HMC7044Write(0x00C8, 0x5d); // DCLKOUT0 @ 2595B_SYNC //7043B sync
            //HMC7044Write(0x00D2, 0xF3); // SCLKOUT1             //7043a 输入时钟 2.5G
            //HMC7044Write(0x00DC, 0x5d); // DCLKOUT2          //2595A SYNC信号		*
            //HMC7044Write(0x00E6, 0x5d); // SCLKOUT3          //7043a sync    
            //HMC7044Write(0x00F0, 0x5d); // DCLKOUT4        //2595B_SYNC
            //HMC7044Write(0x00FA, 0xF3); // SCLKOUT5  //00304 SYSREF   //2595A SYSREF信号	
            //HMC7044Write(0x0104, 0xF3); // DCLKOUT6                       //2595B_SYSREF
            //HMC7044Write(0x010E, 0xF3); // SCLKOUT7                       //250M-15.625M ,作为0304A的输入钟	*
            //HMC7044Write(0x0118, 0xF3); // DCLKOUT8                       //2595C SYSREF信号
            //HMC7044Write(0x0122, 0xF3); // SCLKOUT9                       //0304B的输入钟
            //HMC7044Write(0x012C, 0x5d); // DCLKOUT10                  //2595C_SYNC
            //HMC7044Write(0x0136, 0xF3); // SCLKOUT11                     //2595D SYSREF信号
            //HMC7044Write(0x0140, 0xF3); // DCLKOUT12                    //7043B 输入时钟 2.5G
            //HMC7044Write(0x014A, 0x5d); // SCLKOUT13                 //2595D_SYNC
            // Output Mode Seclect
            // DCLK Mode : 0xF3
            // SYSREF Pluse Mode : 0x5D
            // Turn Off : 0x00
            HMC7044Write(0x00C8, 0x00); // DCLKOUT0 sysref_adc2 unused
            HMC7044Write(0x00D2, 0xf3); // SCLKOUT1 fpga1_refclk4
            HMC7044Write(0x00DC, 0x5d); // DCLKOUT2 sync_2595A
            HMC7044Write(0x00E6, 0xF3); // SCLKOUT3 acq_f1_gt_refclk
            HMC7044Write(0x00F0, 0x5d); // DCLKOUT4 sync_2595B
            HMC7044Write(0x00FA, 0xf3); // SCLKOUT5 sysref_2595A
            HMC7044Write(0x0104, 0xF3); // DCLKOUT6 sysref_2595B
            HMC7044Write(0x010E, 0xF3); // SCLKOUT7 lmk00304 250M-15.625M,作为00304A的输入钟
            HMC7044Write(0x0118, 0xF3); // DCLKOUT8 fpga1_sysref2
            HMC7044Write(0x0122, 0x5d); // SCLKOUT9 fpga1_sysref1
            HMC7044Write(0x012C, 0xF3); // DCLKOUT10 fpga1_refclk1
            HMC7044Write(0x0136, 0xf3); // SCLKOUT11 fpga1_refclk2
            HMC7044Write(0x0140, 0x00); // DCLKOUT12 sysref_adc1 unused
            HMC7044Write(0x014A, 0xf3); // SCLKOUT13 fpga1_refclk3

            HMC7044Write(0x00C9, 0x00); // [7:0]LSB 1280(1.953125MHz)unused
            HMC7044Write(0x00CA, 0x00); // [3:0]MSB DCLKOUT0  @ADC2_TMSTP/SYSREF                
            HMC7044Write(0x00D3, 0x0A); // [7:0]LSB 10(250MHz)
            HMC7044Write(0x00D4, 0x00); // [3:0]MSB SCLKOUT1  @FPGA1_REFCLK4     

            HMC7044Write(0x00DD, 0x00); // [7:0]LSB 640(3.906250MHz)
            HMC7044Write(0x00DE, 0x0a); // [3:0]MSB DCLKOUT2  @2595A_SYNC

            HMC7044Write(0x00E7, 0x00); // [7:0]LSB 8(312.5MHz)  2024/04/15 zm改
            HMC7044Write(0x00E8, 0x0A); // [3:0]MSB SCLKOUT3  @ACQ_F1_GT_REFCLK

            HMC7044Write(0x00F1, 0x00); // [7:0]LSB 640(3.906250MHz)
            HMC7044Write(0x00F2, 0x0a); // [3:0]MSB DCLKOUT4  @2595B_SYNC                
            HMC7044Write(0x00FB, 0x00); // [7:0]LSB 2560(0.9765625MHz)
            HMC7044Write(0x00FC, 0x0a); // [3:0]MSB SCLKOUT5  @2595A_SYSREF                  	

            HMC7044Write(0x0105, 0x00); // [7:0]LSB 2560(0.9765625MHz)
            HMC7044Write(0x0106, 0x0a); // [3:0]MSB DCLKOUT6  @2595B_SYSREF   
            HMC7044Write(0x010F, 0xA0); // [7:0]LSB 160(15.625MHz)
            HMC7044Write(0x0110, 0x00); // [3:0]MSB SCLKOUT7  @00304A_CLKIN       
            HMC7044Write(0x0119, 0x00); // [7:0]LSB 2560(0.9765625MHz)
            HMC7044Write(0x011A, 0x0a); // [3:0]MSB DCLKOUT8  @FPGA1_SYSREF2          
            HMC7044Write(0x0123, 0x00); // [7:0]LSB 2560(0.9765625MHz)
            HMC7044Write(0x0124, 0x0a); // [3:0]MSB SCLKOUT9  @FPGA1_SYSREF1         

            HMC7044Write(0x012D, 0x0A); // [7:0]LSB 10(250MHz)
            HMC7044Write(0x012E, 0x00); // [3:0]MSB DCLKOUT10  @FPGA1_REFCLK1        
            HMC7044Write(0x0137, 0x0A); // [7:0]LSB 10(250MHz)
            HMC7044Write(0x0138, 0x00); // [3:0]MSB SCLKOUT11  @FPGA1_REFCLK2   
            HMC7044Write(0x0141, 0x00); // [7:0]LSB 1280(1.953125MHz) unused
            HMC7044Write(0x0142, 0x00); // [3:0]MSB DCLKOUT12  @ADC1_TMSTP/SYSREF          
            HMC7044Write(0x014B, 0x0A); // [7:0]LSB 10(250MHz)
            HMC7044Write(0x014C, 0x00); // [3:0]MSB SCLKOUT13  @FPGA1_REFCLK3       

            // Fine analog delay
            // Step size 25ps
            // 0~23 effective
            //模拟精细延迟
            HMC7044Write(0x00CB, 0x00); // DCLKOUT0 
            HMC7044Write(0x00DF, 0x00); // DCLKOUT2 
            HMC7044Write(0x00F3, 0x00); // DCLKOUT4  
            HMC7044Write(0x0107, 0x00); // DCLKOUT6 
            HMC7044Write(0x011B, 0x00); // DCLKOUT8
            HMC7044Write(0x012F, 0x00); // DCLKOUT10
            HMC7044Write(0x0143, 0x00); // DCLKOUT12
            HMC7044Write(0x00D5, 0x00); // SCLKOUT1
            HMC7044Write(0x00E9, 0x00); // SCLKOUT3
            HMC7044Write(0x00FD, 0x00); // SCLKOUT5
            HMC7044Write(0x0111, 0x00); // SCLKOUT7
            HMC7044Write(0x0125, 0x00); // SCLKOUT9
            HMC7044Write(0x0139, 0x00); // SCLKOUT11
            HMC7044Write(0x014D, 0x00); // SCLKOUT13
            // Coarse digital deladelay 粗略数字延迟
            // Step size 1/2 VCO cyclk
            // 0~17 effective
            HMC7044Write(0x00CC, 0x00); // DCLKOUT0
            HMC7044Write(0x00D6, 0x00); // SCLKOUT1
            HMC7044Write(0x00E0, 0x00); // DCLKOUT2
            HMC7044Write(0x00EA, 0x00); // SCLKOUT3
            HMC7044Write(0x00F4, 0x00); // DCLKOUT4
            HMC7044Write(0x00FE, 0x00); // SCLKOUT5
            HMC7044Write(0x0108, 0x00); // DCLKOUT6
            HMC7044Write(0x0112, 0x00); // SCLKOUT7
            HMC7044Write(0x011C, 0x00); // DCLKOUT8
            HMC7044Write(0x0126, 0x00); // SCLKOUT9
            HMC7044Write(0x0130, 0x00); // DCLKOUT10
            HMC7044Write(0x013A, 0x00); // SCLKOUT11
            HMC7044Write(0x0144, 0x00); // DCLKOUT12
            HMC7044Write(0x014E, 0x00); // SCLKOUT13
            // Multislip digital delay 多支路数字延迟
            // Step size : amount * VCO cycles
            HMC7044Write(0x00CD, 0x00); // [7:0]LSB
            HMC7044Write(0x00CE, 0x00); // [3:0]MSB DCLKOUT0
            HMC7044Write(0x00D7, 0x00); // [7:0]LSB
            HMC7044Write(0x00D8, 0x00); // [3:0]MSB SCLKOUT1
            HMC7044Write(0x00E1, 0x00); // [7:0]LSB
            HMC7044Write(0x00E2, 0x00); // [3:0]MSB DCLKOUT2
            HMC7044Write(0x00EB, 0x00); // [7:0]LSB
            HMC7044Write(0x00EC, 0x00); // [3:0]MSB SCLKOUT3
            HMC7044Write(0x00F5, 0x00); // [7:0]LSB
            HMC7044Write(0x00F6, 0x00); // [3:0]MSB DCLKOUT4
            HMC7044Write(0x00FF, 0x00); // [7:0]LSB
            HMC7044Write(0x0100, 0x00); // [3:0]MSB SCLKOUT5
            HMC7044Write(0x0109, 0x00); // [7:0]LSB
            HMC7044Write(0x010A, 0x00); // [3:0]MSB DCLKOUT6
            HMC7044Write(0x0113, 0x00); // [7:0]LSB
            HMC7044Write(0x0114, 0x00); // [3:0]MSB SCLKOUT7
            HMC7044Write(0x011D, 0x00); // [7:0]LSB
            HMC7044Write(0x011E, 0x00); // [3:0]MSB DCLKOUT8
            HMC7044Write(0x0127, 0x00); // [7:0]LSB
            HMC7044Write(0x0128, 0x00); // [3:0]MSB SCLKOUT9
            HMC7044Write(0x0131, 0x00); // [7:0]LSB
            HMC7044Write(0x0132, 0x00); // [3:0]MSB DCLKOUT10
            HMC7044Write(0x013B, 0x00); // [7:0]LSB
            HMC7044Write(0x013C, 0x00); // [3:0]MSB SCLKOUT11
            HMC7044Write(0x0145, 0x00); // [7:0]LSB
            HMC7044Write(0x0146, 0x00); // [3:0]MSB DCLKOUT12
            HMC7044Write(0x014F, 0x00); // [7:0]LSB
            HMC7044Write(0x0150, 0x00); // [3:0]MSB SCLKOUT13
            // Output mux slelction
            HMC7044Write(0x00CF, 0x01); // DCLKOUT0
            HMC7044Write(0x00D9, 0x01); // SCLKOUT1
            HMC7044Write(0x00E3, 0x01); // DCLKOUT2
            HMC7044Write(0x00ED, 0x01); // SCLKOUT3
            HMC7044Write(0x00F7, 0x01); // DCLKOUT4
            HMC7044Write(0x0101, 0x01); // SCLKOUT5
            HMC7044Write(0x010B, 0x01); // DCLKOUT6
            HMC7044Write(0x0115, 0x01); // SCLKOUT7
            HMC7044Write(0x011F, 0x01); // DCLKOUT8
            HMC7044Write(0x0129, 0x01); // SCLKOUT9
            HMC7044Write(0x0133, 0x01); // DCLKOUT10
            HMC7044Write(0x013D, 0x01); // SCLKOUT11
            HMC7044Write(0x0147, 0x01); // DCLKOUT12
            HMC7044Write(0x0151, 0x01); // SCLKOUT13

            // Output driver
            HMC7044Write(0x00D0, 0x90); // DCLKOUT0                                       
            HMC7044Write(0x00DA, 0x10); // SCLKOUT1  0x10                                              
            HMC7044Write(0x00E4, 0x90); // DCLKOUT2                                           
            HMC7044Write(0x00EE, 0x01); // SCLKOUT3                                           

            //sysref:LVPECL输出只有100mv(单端)过完00304有200mv(单端)
            HMC7044Write(0x00F8, 0x90);//DCLKOUT4 LVPECL   //90); // DCLKOUT4 LVDS                              

            //sysref:LVDS可以被00304正常识别,00304输出500mv(单端)

            HMC7044Write(0x0102, 0x90);//SCLKOUT5 CML(100ohm) (PULSE)                                      	


            HMC7044Write(0x010C, 0x90); // DCLKOUT6 CML 100ohm                                               

            HMC7044Write(0x0116, 0x10); // SCLKOUT7 LVDS                                                      
            HMC7044Write(0x0120, 0x90); // DCLKOUT8 LVDS                                                      
            HMC7044Write(0x012A, 0x90); // SCLKOUT9 LVDS                                                       

            HMC7044Write(0x0134, 0x10); // DCLKOUT10                                                    
            HMC7044Write(0x013E, 0x10); // SCLKOUT11                                                           
            HMC7044Write(0x0148, 0x90); // DCLKOUT12                                                          
            HMC7044Write(0x0152, 0x10); // SCLKOUT13                                  
                                        //-----------------------------//
                                        // 		Input buffer
                                        //---------------------------//
            HMC7044Write(0x000A, 0x09); // CLKIN0/RFSYNCIN
            HMC7044Write(0x000B, 0x09); // CLKIN1
            HMC7044Write(0x000C, 0x07); // CLKIN2
            HMC7044Write(0x000D, 0x09); // CLKIN3   //0X03000d
            HMC7044Write(0x000E, 0x07); // OSCIN
                                        //-----------------------------//
                                        // 		Other
                                        //---------------------------//
            HMC7044Write(0x0001, 0x02);
            HMC7044Write(0x0001, 0x00);
            HMC7044Write(0x0014, 0x27); //clkin_priority
            HMC7044Write(0x0015, 0x03);
            HMC7044Write(0x0016, 0x0C);
            HMC7044Write(0x0017, 0x00);
            HMC7044Write(0x0018, 0x04);
            HMC7044Write(0x0019, 0x03);
            HMC7044Write(0x001A, 0x08);
            HMC7044Write(0x001B, 0x18);
            HMC7044Write(0x001C, 0x01);
            HMC7044Write(0x001D, 0x01);
            HMC7044Write(0x001E, 0x01);
            HMC7044Write(0x001F, 0x01);
            HMC7044Write(0x0020, 0x0A);
            HMC7044Write(0x0021, 0x01);
            HMC7044Write(0x0022, 0x00);
            HMC7044Write(0x0026, 0x0A);
            HMC7044Write(0x0027, 0x00);
            HMC7044Write(0x0028, 0x13);
            HMC7044Write(0x0029, 0x07);
            HMC7044Write(0x002A, 0x0F);
        }
        private void InitHMC7044_Adc5200_8G()
        {
            //ACQ5200_HMC7044 start
            WriteToAllFpga(AcqBdReg.W.PllConfig_7044Reset, 0);
            HdIO.Sleep(10);
            WriteToAllFpga(AcqBdReg.W.PllConfig_7044Reset, 1);
            HdIO.Sleep(10);
            WriteToAllFpga(AcqBdReg.W.PllConfig_7044Reset, 0);
            HdIO.Sleep(10);

            /**********register_config***********/
            HMC7044Write(0x0000, 0x01);
            HMC7044Write(0x0000, 0x00);
            HMC7044Write(0x0001, 0x40);
            HMC7044Write(0x0002, 0x04);
            HMC7044Write(0x0003, 0x37);
            HMC7044Write(0x0004, 0x7F);
            HMC7044Write(0x0005, 0x98); // SYNC MODE (82、58)???cij  // hxy 0511 add 218中用的94，94与98的区别在于选择并使能PLL1的参考路径为clkin2和clkin3；SYNC相关的配置没有区别
            HMC7044Write(0x0006, 0x00);
            HMC7044Write(0x0007, 0x00);
            HMC7044Write(0x0009, 0x01);
            //-----------------------------//
            // 		保留寄存器
            //---------------------------//
            HMC7044Write(0x0096, 0x00);
            HMC7044Write(0x0097, 0x00);
            HMC7044Write(0x0098, 0x00);
            HMC7044Write(0x0099, 0x00);
            HMC7044Write(0x009A, 0x00);
            HMC7044Write(0x009B, 0xAA);
            HMC7044Write(0x009C, 0xAA);
            HMC7044Write(0x009D, 0xAA);
            HMC7044Write(0x009E, 0xAA);
            HMC7044Write(0x009F, 0x4D);
            HMC7044Write(0x00A0, 0xDF);
            HMC7044Write(0x00A1, 0x97);
            HMC7044Write(0x00A2, 0x03);
            HMC7044Write(0x00A3, 0x00);
            HMC7044Write(0x00A4, 0x00);
            HMC7044Write(0x00A5, 0x06);
            HMC7044Write(0x00A6, 0x1C);
            HMC7044Write(0x00A7, 0x00);
            HMC7044Write(0x00A8, 0x06);
            HMC7044Write(0x00A9, 0x00);
            HMC7044Write(0x00AB, 0x00);
            HMC7044Write(0x00AC, 0x20);
            HMC7044Write(0x00AD, 0x00);
            HMC7044Write(0x00AE, 0x08);
            HMC7044Write(0x00AF, 0x50);
            HMC7044Write(0x00B0, 0x04);
            HMC7044Write(0x00B1, 0x0D);
            HMC7044Write(0x00B2, 0x00);
            HMC7044Write(0x00B3, 0x00);
            HMC7044Write(0x00B5, 0x00);
            HMC7044Write(0x00B6, 0x00);
            HMC7044Write(0x00B7, 0x00);
            HMC7044Write(0x00B8, 0x00);
            //-----------------------------//
            // 		PLL2配置
            //---------------------------//
            HMC7044Write(0x0031, 0x01);
            HMC7044Write(0x0032, 0x01); // DOUBLE R
            HMC7044Write(0x0033, 0x01); // R2
            HMC7044Write(0x0034, 0x00);
            HMC7044Write(0x0035, 0x19); // N2
            HMC7044Write(0x0036, 0x00);
            HMC7044Write(0x0037, 0x0F);
            HMC7044Write(0x0038, 0x18);
            HMC7044Write(0x0039, 0x00);//   @OSCOUTx/OSCOUTx Path Control  输出分频器和路径使能
            HMC7044Write(0x003A, 0x00);//   @OSCOUTx/OSCOUTx Driver Control输出驱动配置
            HMC7044Write(0x003B, 0x00);//   @OSCOUTx/OSCOUTx Driver Control输出驱动配置
            //-----------------------------//
            // 		PLL1配置
            //---------------------------//
            HMC7044Write(0x0046, 0x00);
            HMC7044Write(0x0047, 0x00);
            HMC7044Write(0x0048, 0x08);
            HMC7044Write(0x0049, 0x10);
            HMC7044Write(0x0050, 0x1F);
            HMC7044Write(0x0051, 0x2B);
            HMC7044Write(0x0052, 0x37);
            //HMC7044read(0x0050, 0x7f);
            //HMC7044Write(0x0050, 0x7f);//1F
            //HMC7044Write(0x0051, 0x7f);
            //HMC7044Write(0x0052, 0x7f);

            HMC7044Write(0x0053, 0x33);
            HMC7044Write(0x0054, 0x03);
            //// significant start
            //HMC7044Write(3, 0x005B, 0x00);
            //HMC7044Write(3, 0x005C, 0x80);
            //HMC7044Write(3, 0x005D, 0x00);
            //// significant end
            HMC7044Write(0x0064, 0x00);
            HMC7044Write(0x0065, 0x00);
            HMC7044Write(0x0070, 0xE0); // alarm
            HMC7044Write(0x0071, 0x19);
            HMC7044Write(0x0078, 0x00);
            HMC7044Write(0x0079, 0x00);
            HMC7044Write(0x007A, 0x00);
            HMC7044Write(0x007B, 0x00);
            HMC7044Write(0x007C, 0x00);
            HMC7044Write(0x007D, 0x00);
            HMC7044Write(0x007E, 0x00);
            HMC7044Write(0x0082, 0x00);
            HMC7044Write(0x0083, 0x00);
            HMC7044Write(0x0084, 0x00);
            HMC7044Write(0x0085, 0x00);
            HMC7044Write(0x0086, 0x00);
            HMC7044Write(0x008C, 0x00);
            HMC7044Write(0x008D, 0x00);
            HMC7044Write(0x008E, 0x00);
            HMC7044Write(0x008F, 0x00);
            HMC7044Write(0x0091, 0x00);
            //-----------------------------//
            // 		Sysref Timer
            //---------------------------//
            HMC7044Write(0x005A, 0x01);//07
            HMC7044Write(0x005B, 0x04); // 04
            HMC7044Write(0x005C, 0x00); // [7:0]LSB
            HMC7044Write(0x005D, 0x0a); // [3:0]MSB   2560
            //-----------------------------//
            // 		Clock Output Channel
            //---------------------------//
            // Output Mode Seclect
            // DCLK Mode : 0xF3
            // SYSREF Pluse Mode : 0x5D
            // Turn Off : 0x00
            HMC7044Write(0x00C8, 0x5D); // DCLKOUT0   @LMX2595B_SYNC                                                        LVDS           
            HMC7044Write(0x00D2, 0x5D); // SCLKOUT1   @LMX2595A_SYNC                                                        LVDS           
            HMC7044Write(0x00DC, 0xF3); // DCLKOUT2   @LMX2595A_SYSREFREQ                           2500/1280=1.953125MHz   LVDS           
            HMC7044Write(0x00E6, 0xF3); // SCLKOUT3   @LMX2595B_SYSREFREQ                           2500/1280=1.953125MHz   LVDS           
            HMC7044Write(0x00F0, 0xF3); // DCLKOUT4   @LMK00304A_CLKIN1  2595的输入时钟             15.625MHz               LVDS       
            HMC7044Write(0x00FA, 0xF3); // SCLKOUT5   @LMK00304B_CLKIN0  给FPGA提供两个204B协议的SYSREF（BANK65上的F1_RX2_SYSREF和F1_RX3_sysref）   2500/1280=1.953125MHz  LVDS     
            HMC7044Write(0x0104, 0xF3); // DCLKOUT6   @FPGA1_REFCLK1     BANK231上的FPGA的GT参考钟  250MHz                  LVDS     
            HMC7044Write(0x010E, 0xF3); // SCLKOUT7   @FPGA1_REFCLK3     BANK227上的FPGA的GT参考钟  250MHz                  LVDS     
            HMC7044Write(0x0118, 0xF3); // DCLKOUT8   @FPGA1_REFCLK2     BANK229上的FPGA的GT参考钟  250MHz                  LVDS     
            HMC7044Write(0x0122, 0xF3); // SCLKOUT9   @FPGA1_REFCLK4     BANK225上的FPGA的GT参考钟  250MHz                           
            HMC7044Write(0x012C, 0x00); // DCLKOUT10  @LMX2595A_CLKIN    NC                         15.625MHz                  LVPECL
            HMC7044Write(0x0136, 0x00); // SCLKOUT11  @FPGA1_REFCLK5     接在SY58608U上             250MHz                     LVPECL
            HMC7044Write(0x0140, 0x00); // DCLKOUT12  @LMX2595B_CLKIN    NC                         15.625MHz                  LVPECL
            HMC7044Write(0x014A, 0x00); // SCLKOUT13  @FPGA1_SYSREF1     BANK65上的F1_RX1_SYSREF    2500/1280=1.953125MHz            

            // Output divider
            // Even divide ratios from 2 to 4094
            // Odd divide ratios are 1、3、5
            HMC7044Write(0x00C9, 0x00); // [7:0]LSB
            HMC7044Write(0x00CA, 0x0a); // [3:0]MSB DCLKOUT0 @LMX2595B_SYNC                                                        LVDS
            HMC7044Write(0x00D3, 0x00); // [7:0]LSB
            HMC7044Write(0x00D4, 0x0a); // [3:0]MSB SCLKOUT1 @LMX2595A_SYNC                                                        LVDS

            HMC7044Write(0x00DD, 0x00); // [7:0]LSB
            HMC7044Write(0x00DE, 0x0a); // [3:0]MSB DCLKOUT2 @LMX2595A_SYSREFREQ   2500/2560=0.9765625MHz   LVDS	
            HMC7044Write(0x00E7, 0x00); // [7:0]LSB
            HMC7044Write(0x00E8, 0x0a); // [3:0]MSB SCLKOUT3 @LMX2595B_SYSREFREQ   2500/2560=0.9765625MHz   LVDS
                                        //CIJ_NEW_CHANGE_78.125MHz
            HMC7044Write(0x00F1, 0xa0); // [7:0]LSB
            HMC7044Write(0x00F2, 0x00); // [3:0]MSB DCLKOUT4 @LMK00304A_CLKIN1  2595的输入时钟            15.625MHz                LVDS
                                        //HMC7044Write(0x00F1, 0x20); // [7:0]LSB
                                        //HMC7044Write(0x00F2, 0x00); // [3:0]MSB DCLKOUT4 @LMK00304A_CLKIN1  2595的输入时钟            78.125MHz                LVDS
                                        //CIJ_NEW_CHANGE_78.125MHz
            HMC7044Write(0x00FB, 0x00); // [7:0]LSB
            HMC7044Write(0x00FC, 0x0a); // [3:0]MSB SCLKOUT5 @LMK00304B_CLKIN0  给FPGA提供两个204B协议的SYSREF（BANK65上的F1_RX2_SYSREF和F1_RX3_sysref）   2500/2560=0.9765625MHz  LVDS

            HMC7044Write(0x0105, 0x0a); // [7:0]LSB
            HMC7044Write(0x0106, 0x00); // [3:0]MSB DCLKOUT6 @FPGA1_REFCLK1     BANK231上的FPGA的GT参考钟  250MHz                  LVDS
            HMC7044Write(0x010F, 0x0a); // [7:0]LSB                                                                                
            HMC7044Write(0x0110, 0x00); // [3:0]MSB SCLKOUT7 @FPGA1_REFCLK3     BANK227上的FPGA的GT参考钟  250MHz                  LVDS
            HMC7044Write(0x0119, 0x0a); // [7:0]LSB                                                                                
            HMC7044Write(0x011A, 0x00); // [3:0]MSB DCLKOUT8 @FPGA1_REFCLK2     BANK229上的FPGA的GT参考钟  250MHz                  LVDS
            HMC7044Write(0x0123, 0x0a); // [7:0]LSB                                                                                
            HMC7044Write(0x0124, 0x00); // [3:0]MSB SCLKOUT9 @FPGA1_REFCLK4     BANK225上的FPGA的GT参考钟  250MHz                  LVDS

            HMC7044Write(0x012D, 0x00); // [7:0]LSB
            HMC7044Write(0x012E, 0x00); // [3:0]MSB DCLKOUT10  @LMX2595A_CLKIN    NC                         15.625MHz             LVPECL
            HMC7044Write(0x0137, 0x00); // [7:0]LSB
            HMC7044Write(0x0138, 0x00); // [3:0]MSB SCLKOUT11  @FPGA1_REFCLK5     接在SY58608U上             250MHz                LVPECL
            HMC7044Write(0x0141, 0x00); // [7:0]LSB
            HMC7044Write(0x0142, 0x00); // [3:0]MSB DCLKOUT12  @LMX2595B_CLKIN    NC                         15.625MHz             LVPECL
            HMC7044Write(0x014B, 0x00); // [7:0]LSB
            HMC7044Write(0x014C, 0x00); // [3:0]MSB SCLKOUT13  @FPGA1_SYSREF1     BANK65上的F1_RX1_SYSREF    2500/1280=1.953125MHz
            // Fine analog delay
            // Step size 25ps
            // 0~23 effective
            HMC7044Write(0x00CB, 0x00); // DCLKOUT0 
            HMC7044Write(0x00DF, 0x00); // DCLKOUT2 
            HMC7044Write(0x00F3, 0x00); // DCLKOUT4 
            HMC7044Write(0x0107, 0x00); // DCLKOUT6 
            HMC7044Write(0x011B, 0x00); // DCLKOUT8
            HMC7044Write(0x012F, 0x00); // DCLKOUT10
            HMC7044Write(0x0143, 0x00); // DCLKOUT12
            HMC7044Write(0x00D5, 0x00); // SCLKOUT1
            HMC7044Write(0x00E9, 0x00); // SCLKOUT3
            HMC7044Write(0x00FD, 0x00); // SCLKOUT5
            HMC7044Write(0x0111, 0x00); // SCLKOUT7
            HMC7044Write(0x0125, 0x00); // SCLKOUT9
            HMC7044Write(0x0139, 0x00); // SCLKOUT11
            HMC7044Write(0x014D, 0x00); // SCLKOUT13
            // Coarse digital deladelay
            // Step size 1/2 VCO cyclk //10G-100ps-50ps
            // 0~17 effective
            HMC7044Write(0x00CC, 0x00); // DCLKOUT0
            HMC7044Write(0x00D6, 0x00); // SCLKOUT1
            HMC7044Write(0x00E0, 0x00); // DCLKOUT2
            HMC7044Write(0x00EA, 0x00); // SCLKOUT3
            HMC7044Write(0x00F4, 0x00); // DCLKOUT4
            HMC7044Write(0x00FE, 0x00); // SCLKOUT5
            HMC7044Write(0x0108, 0x00); // DCLKOUT6
            HMC7044Write(0x0112, 0x00); // SCLKOUT7
            HMC7044Write(0x011C, 0x00); // DCLKOUT8
            HMC7044Write(0x0126, 0x00); // SCLKOUT9
            HMC7044Write(0x0130, 0x00); // DCLKOUT10
            HMC7044Write(0x013A, 0x00); // SCLKOUT11
            HMC7044Write(0x0144, 0x00); // DCLKOUT12
            HMC7044Write(0x014E, 0x00); // SCLKOUT13
            // Multislip digital delay
            // Step size : amount * VCO cycles
            HMC7044Write(0x00CD, 0x00); // [7:0]LSB
            HMC7044Write(0x00CE, 0x00); // [3:0]MSB DCLKOUT0
            HMC7044Write(0x00D7, 0x00); // [7:0]LSB
            HMC7044Write(0x00D8, 0x00); // [3:0]MSB SCLKOUT1
            HMC7044Write(0x00E1, 0x00); // [7:0]LSB
            HMC7044Write(0x00E2, 0x00); // [3:0]MSB DCLKOUT2
            HMC7044Write(0x00EB, 0x00); // [7:0]LSB
            HMC7044Write(0x00EC, 0x00); // [3:0]MSB SCLKOUT3
            HMC7044Write(0x00F5, 0x00); // [7:0]LSB
            HMC7044Write(0x00F6, 0x00); // [3:0]MSB DCLKOUT4
            HMC7044Write(0x00FF, 0x00); // [7:0]LSB
            HMC7044Write(0x0100, 0x00); // [3:0]MSB SCLKOUT5
            HMC7044Write(0x0109, 0x00); // [7:0]LSB
            HMC7044Write(0x010A, 0x00); // [3:0]MSB DCLKOUT6
            HMC7044Write(0x0113, 0x00); // [7:0]LSB
            HMC7044Write(0x0114, 0x00); // [3:0]MSB SCLKOUT7
            HMC7044Write(0x011D, 0x00); // [7:0]LSB
            HMC7044Write(0x011E, 0x00); // [3:0]MSB DCLKOUT8
            HMC7044Write(0x0127, 0x00); // [7:0]LSB
            HMC7044Write(0x0128, 0x00); // [3:0]MSB SCLKOUT9
            HMC7044Write(0x0131, 0x00); // [7:0]LSB
            HMC7044Write(0x0132, 0x00); // [3:0]MSB DCLKOUT10
            HMC7044Write(0x013B, 0x00); // [7:0]LSB
            HMC7044Write(0x013C, 0x00); // [3:0]MSB SCLKOUT11
            HMC7044Write(0x0145, 0x00); // [7:0]LSB
            HMC7044Write(0x0146, 0x00); // [3:0]MSB DCLKOUT12
            HMC7044Write(0x014F, 0x00); // [7:0]LSB
            HMC7044Write(0x0150, 0x00); // [3:0]MSB SCLKOUT13
            // Output mux slelction
            HMC7044Write(0x00CF, 0x00); // DCLKOUT0
            HMC7044Write(0x00D9, 0x00); // SCLKOUT1
            HMC7044Write(0x00E3, 0x00); // DCLKOUT2
            HMC7044Write(0x00ED, 0x00); // SCLKOUT3
            HMC7044Write(0x00F7, 0x00); // DCLKOUT4
            HMC7044Write(0x0101, 0x00); // SCLKOUT5
            HMC7044Write(0x010B, 0x00); // DCLKOUT6
            HMC7044Write(0x0115, 0x00); // SCLKOUT7
            HMC7044Write(0x011F, 0x00); // DCLKOUT8
            HMC7044Write(0x0129, 0x00); // SCLKOUT9
            HMC7044Write(0x0133, 0x00); // DCLKOUT10
            HMC7044Write(0x013D, 0x00); // SCLKOUT11
            HMC7044Write(0x0147, 0x00); // DCLKOUT12
            HMC7044Write(0x0151, 0x00); // SCLKOUT13

            // Output driver
            //90:Force to Logic 0.;                LVDS mode.   ;Internal resistor disable.
            //89;Force to Logic 0.;                LVPECL mode. ;Internal 100 Ω resistor enable per output pin.
            //10:Normal mode (selection for DCLK).;LVDS mode.   ;Internal resistor disable.
            //80:Force to Logic 0.;                CML mode     ;Internal resistor disable.
            //81:Force to Logic 0.;                CML mode     ;Internal 100 Ω resistor enable per output pin.
            //82:Force to Logic 0.;                CML mode     ;Reserved.
            //83:Force to Logic 0.;                CML mode     ;Internal 50 Ω resistor enable per output pin.
            //08:Normal mode (selection for DCLK).;LVPECL mode. ;Internal resistor disable.
            //09;Normal mode (selection for DCLK).;LVPECL mode. ;Internal 100 Ω resistor enable per output pin.

            HMC7044Write(0x00D0, 0x81); // DCLKOUT0  @LMX2595B_SYNC                                                        CML 100       
            HMC7044Write(0x00DA, 0x81); // SCLKOUT1  @LMX2595A_SYNC                                                        CML 100       
            HMC7044Write(0x00E4, 0x01); // DCLKOUT2  @LMX2595A_SYSREFREQ                           2500/1280=1.953125MHz   CML 100       
            HMC7044Write(0x00EE, 0x01); // SCLKOUT3  @LMX2595B_SYSREFREQ                           2500/1280=1.953125MHz   CML 100

            //sysref:LVDS可以被00304正常识别,00304输出500mv(单端)
            HMC7044Write(0x00F8, 0x08); // DCLKOUT4  @LMK00304A_CLKIN1  2595的输入时钟             15.625MHz               LVPECL
            HMC7044Write(0x0102, 0x90); // SCLKOUT5  @LMK00304B_CLKIN0  给ADC1和ADC2提供TMSTP（NC），给FPGA提供两个204B协议的SYSREF（BANK65上的F1_RX2_SYSREF和F1_RX3_sysref）   2500/1280=1.953125MHz   LVDS

            HMC7044Write(0x010C, 0x01); // DCLKOUT6  @FPGA1_REFCLK1     BANK231上的FPGA的GT参考钟  250MHz                  CML 100         
            HMC7044Write(0x0116, 0x01); // SCLKOUT7  @FPGA1_REFCLK3     BANK227上的FPGA的GT参考钟  250MHz                  CML 100         
            HMC7044Write(0x0120, 0x01); // DCLKOUT8  @FPGA1_REFCLK2     BANK229上的FPGA的GT参考钟  250MHz                  CML 100         
            HMC7044Write(0x012A, 0x01); // SCLKOUT9  @FPGA1_REFCLK4     BANK225上的FPGA的GT参考钟  250MHz                  CML 100         

            HMC7044Write(0x0134, 0x08); // DCLKOUT10 @LMX2595A_CLKIN    NC                         15.625MHz               LVPECL   
            HMC7044Write(0x013E, 0x09); // SCLKOUT11 @FPGA1_REFCLK5     接在SY58608U上             250MHz                  LVPECL             
            HMC7044Write(0x0148, 0x09); // DCLKOUT12 @LMX2595B_CLKIN    NC                         15.625MHz               LVPECL             
            HMC7044Write(0x0152, 0x10); // SCLKOUT13 @FPGA1_SYSREF1     BANK65上的F1_RX1_SYSREF    2500/1280=1.953125MHz   
            //-----------------------------//
            // 		Input buffer
            //---------------------------//
            HMC7044Write(0x000A, 0x0B); // CLKIN0/RFSYNCIN//09
            HMC7044Write(0x000B, 0x07); // CLKIN1
            HMC7044Write(0x000C, 0x07); // CLKIN2
            HMC7044Write(0x000D, 0x0B); // CLKIN3 ////09
            HMC7044Write(0x000E, 0x07); // OSCIN
            //-----------------------------//
            // 		Other
            //---------------------------//
            HMC7044Write(0x0001, 0x02);
            HMC7044Write(0x0001, 0x00);
            HMC7044Write(0x0014, 0x27); //clkin_priority
            HMC7044Write(0x0015, 0x03);
            HMC7044Write(0x0016, 0x0C);
            HMC7044Write(0x0017, 0x00);
            HMC7044Write(0x0018, 0x04);
            HMC7044Write(0x0019, 0x03);
            HMC7044Write(0x001A, 0x08);
            HMC7044Write(0x001B, 0x18);
            HMC7044Write(0x001C, 0x01);
            HMC7044Write(0x001D, 0x01);
            HMC7044Write(0x001E, 0x01);
            HMC7044Write(0x001F, 0x01);
            HMC7044Write(0x0020, 0x0A);
            HMC7044Write(0x0021, 0x01);
            HMC7044Write(0x0022, 0x00);
            HMC7044Write(0x0026, 0x0A);
            HMC7044Write(0x0027, 0x00);
            HMC7044Write(0x0028, 0x13);
            HMC7044Write(0x0029, 0x07);
            HMC7044Write(0x002A, 0x0F);
        }
        #endregion

        #region 2595
        private void SendCmdToLMX2595(Int32 adcIndex, UInt32 Address_7bit, UInt32 Commmand_16bit)//Address15bit,Commmand 8bit
        {   //NOTE : ADCsel 如果要分别配置，必须先配置0再配置1
            //=0 LMX2595A
            //=1 LMX2595B
            //=2 both
            UInt32 tmp = ((0x000 << 23) | (Address_7bit << 16) | Commmand_16bit);//(0x001 << 23)Instruction R(1b'1)/W(1b'0)
            WriteToAllFpga(AcqBdReg.W.PllConfig_LMX2595Data_L8, tmp & 0xffff);
            WriteToAllFpga(AcqBdReg.W.PllConfig_LMX2595Data_H16, (tmp >> 8) & 0xffff);
            (UInt32 first, UInt32 second) = adcIndex switch
            {
                0 => (0x00U, 0x40U),//0000 0000,0100 0000
                1 => (0x40U, 0xc0U),//0100 0000,1100 0000
                _ => (0x00U, 0xc0U) //0000 0000,1100 0000 0&1
            };
            WriteToAllFpga(AcqBdReg.W.PllConfig_LMX2595Data_Effect, first);
            //var testReg = HdIO.ReadReg((UInt32)((UInt32)AcqBdReg.R.PllConfig_LMX2595A_SpiReadBackData | AcqFpgaAddrMarkTable[4]));
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(1);
            WriteToAllFpga(AcqBdReg.W.PllConfig_LMX2595Data_Effect, second);
            HdIO.Sleep(1);
        }
        private void LMX2595_Init(Int32 adcIndex)
        {

            SendCmdToLMX2595(adcIndex, 0x00, 0x241e);
            HdIO.Sleep(10);
            SendCmdToLMX2595(adcIndex, 0x00, 0x241c);
            //Write register form highest to lowest
            SendCmdToLMX2595(adcIndex, 0x70, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6F, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6E, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6D, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6C, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6B, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6A, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x69, 0x0021);
            SendCmdToLMX2595(adcIndex, 0x68, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x67, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x66, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x65, 0x0011);
            SendCmdToLMX2595(adcIndex, 0x64, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x63, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x62, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x61, 0x0888);
            SendCmdToLMX2595(adcIndex, 0x60, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5F, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5E, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5D, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5C, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5B, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5A, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x59, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x58, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x57, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x56, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x55, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x54, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x53, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x52, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x51, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x50, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x4F, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x4E, 0x0105);
            SendCmdToLMX2595(adcIndex, 0x4D, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x4C, 0x000C);
            SendCmdToLMX2595(adcIndex, 0x4B, 0x0800);//divider
            SendCmdToLMX2595(adcIndex, 0x4A, 0x1000);//sysref delay control;  5 PULSE  0 DELAY
            SendCmdToLMX2595(adcIndex, 0x49, 0x06E4);//0x06E4
            SendCmdToLMX2595(adcIndex, 0x48, 0x004E);//3c测得sysref  100ns,4c: 125ns
            SendCmdToLMX2595(adcIndex, 0x47, 0x004d);//59:sysref pulse 4d: repeat
            SendCmdToLMX2595(adcIndex, 0x46, 0xC350);
            SendCmdToLMX2595(adcIndex, 0x45, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x44, 0x03E8);
            SendCmdToLMX2595(adcIndex, 0x43, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x42, 0x01F4);
            SendCmdToLMX2595(adcIndex, 0x41, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x40, 0x1388);
            SendCmdToLMX2595(adcIndex, 0x3F, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x3E, 0x0322);
            SendCmdToLMX2595(adcIndex, 0x3D, 0x00A8);
            SendCmdToLMX2595(adcIndex, 0x3C, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x3B, 0x0001);
            SendCmdToLMX2595(adcIndex, 0x3A, 0x5601);//SYNC:LVDS  sysref :LVDS(1001) 
            SendCmdToLMX2595(adcIndex, 0x39, 0x0020);
            SendCmdToLMX2595(adcIndex, 0x38, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x37, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x36, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x35, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x34, 0x0820);
            SendCmdToLMX2595(adcIndex, 0x33, 0x0080);
            SendCmdToLMX2595(adcIndex, 0x32, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x31, 0x4180);
            SendCmdToLMX2595(adcIndex, 0x30, 0x0300);
            SendCmdToLMX2595(adcIndex, 0x2F, 0x0300);
            SendCmdToLMX2595(adcIndex, 0x2E, 0x07FE);
            SendCmdToLMX2595(adcIndex, 0x2D, 0xC0DF);
            SendCmdToLMX2595(adcIndex, 0x2C, 0x1F01);//31
            SendCmdToLMX2595(adcIndex, 0x2B, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x2A, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x29, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x28, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x27, 0x03e8);
            SendCmdToLMX2595(adcIndex, 0x26, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x25, 0x0104);//8104: MASH_SEED_EN=1 0104: MASH_SEED_EN=0
            SendCmdToLMX2595(adcIndex, 0x24, 0x00a0);// 32);
            SendCmdToLMX2595(adcIndex, 0x23, 0x0004);
            SendCmdToLMX2595(adcIndex, 0x22, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x21, 0x1E21);
            SendCmdToLMX2595(adcIndex, 0x20, 0x0393);
            SendCmdToLMX2595(adcIndex, 0x1F, 0x43EC);
            SendCmdToLMX2595(adcIndex, 0x1E, 0x318C);
            SendCmdToLMX2595(adcIndex, 0x1D, 0x318C);
            SendCmdToLMX2595(adcIndex, 0x1C, 0x0488);
            SendCmdToLMX2595(adcIndex, 0x1B, 0x0002);
            SendCmdToLMX2595(adcIndex, 0x1A, 0x0DB0);
            SendCmdToLMX2595(adcIndex, 0x19, 0x0C2B);
            SendCmdToLMX2595(adcIndex, 0x18, 0x071A);
            SendCmdToLMX2595(adcIndex, 0x17, 0x007C);
            SendCmdToLMX2595(adcIndex, 0x16, 0x0001);
            SendCmdToLMX2595(adcIndex, 0x15, 0x0401);
            SendCmdToLMX2595(adcIndex, 0x14, 0xD848);
            SendCmdToLMX2595(adcIndex, 0x13, 0x27B7);
            SendCmdToLMX2595(adcIndex, 0x12, 0x0064);
            SendCmdToLMX2595(adcIndex, 0x11, 0x0130);
            SendCmdToLMX2595(adcIndex, 0x10, 0x0080);
            SendCmdToLMX2595(adcIndex, 0x0F, 0x064F);
            SendCmdToLMX2595(adcIndex, 0x0E, 0x1E70);
            SendCmdToLMX2595(adcIndex, 0x0D, 0x4000);
            SendCmdToLMX2595(adcIndex, 0x0C, 0x5001);
            SendCmdToLMX2595(adcIndex, 0x0B, 0x0018);
            SendCmdToLMX2595(adcIndex, 0x0A, 0x10D8);
            SendCmdToLMX2595(adcIndex, 0x09, 0x0604);
            SendCmdToLMX2595(adcIndex, 0x08, 0x2000);
            SendCmdToLMX2595(adcIndex, 0x07, 0x40B2);
            SendCmdToLMX2595(adcIndex, 0x06, 0xC802);
            SendCmdToLMX2595(adcIndex, 0x05, 0x00C8);
            SendCmdToLMX2595(adcIndex, 0x04, 0x0A43);
            SendCmdToLMX2595(adcIndex, 0x03, 0x0642);
            SendCmdToLMX2595(adcIndex, 0x02, 0x0500);
            SendCmdToLMX2595(adcIndex, 0x01, 0x0808);
            SendCmdToLMX2595(adcIndex, 0x00, 0x641C);
            //FCLA_EN 
            HdIO.Sleep(10);
            SendCmdToLMX2595(adcIndex, 0x00, 0x641C);
        }
        private void LMX2595_Init_8G(Int32 adcIndex)
        {
            SendCmdToLMX2595(adcIndex, 0x00, 0x241e);
            HdIO.Sleep(10);
            SendCmdToLMX2595(adcIndex, 0x00, 0x241c);
            //Write register form highest to lowest
            SendCmdToLMX2595(adcIndex, 0x70, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6F, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6E, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6D, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6C, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6B, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6A, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x69, 0x0021);
            SendCmdToLMX2595(adcIndex, 0x68, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x67, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x66, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x65, 0x0011);
            SendCmdToLMX2595(adcIndex, 0x64, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x63, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x62, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x61, 0x0888);
            SendCmdToLMX2595(adcIndex, 0x60, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5F, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5E, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5D, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5C, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5B, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5A, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x59, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x58, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x57, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x56, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x55, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x54, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x53, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x52, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x51, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x50, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x4F, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x4E, 0x0105);
            SendCmdToLMX2595(adcIndex, 0x4D, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x4C, 0x000C);
            SendCmdToLMX2595(adcIndex, 0x4B, 0x0800);//divider
            SendCmdToLMX2595(adcIndex, 0x4A, 0x1000);//sysref delay control;  5 PULSE  0 DELAY
            SendCmdToLMX2595(adcIndex, 0x49, 0x06E4);
            SendCmdToLMX2595(adcIndex, 0x48, 0x004E);//3c测得sysref  100ns,4c: 125ns
            SendCmdToLMX2595(adcIndex, 0x47, 0x008d);//59:sysref pulse 4d: repeat and 4div  //8d: repeat and 8div
            SendCmdToLMX2595(adcIndex, 0x46, 0xC350);
            SendCmdToLMX2595(adcIndex, 0x45, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x44, 0x03E8);
            SendCmdToLMX2595(adcIndex, 0x43, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x42, 0x01F4);
            SendCmdToLMX2595(adcIndex, 0x41, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x40, 0x1388);
            SendCmdToLMX2595(adcIndex, 0x3F, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x3E, 0x0322);
            SendCmdToLMX2595(adcIndex, 0x3D, 0x00A8);
            SendCmdToLMX2595(adcIndex, 0x3C, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x3B, 0x0001);
            SendCmdToLMX2595(adcIndex, 0x3A, 0x5601);//SYNC:LVDS  sysref :LVDS(1001) 
            SendCmdToLMX2595(adcIndex, 0x39, 0x0020);
            SendCmdToLMX2595(adcIndex, 0x38, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x37, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x36, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x35, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x34, 0x0820);
            SendCmdToLMX2595(adcIndex, 0x33, 0x0080);
            SendCmdToLMX2595(adcIndex, 0x32, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x31, 0x4180);
            SendCmdToLMX2595(adcIndex, 0x30, 0x0300);
            SendCmdToLMX2595(adcIndex, 0x2F, 0x0300);
            SendCmdToLMX2595(adcIndex, 0x2E, 0x07FE);
            SendCmdToLMX2595(adcIndex, 0x2D, 0xC0DF);
            SendCmdToLMX2595(adcIndex, 0x2C, 0x1F01);//31
            SendCmdToLMX2595(adcIndex, 0x2B, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x2A, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x29, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x28, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x27, 0x03e8);
            SendCmdToLMX2595(adcIndex, 0x26, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x25, 0x0104);//8104: MASH_SEED_EN=1 0104: MASH_SEED_EN=0
            SendCmdToLMX2595(adcIndex, 0x24, 0x00a0);// 32);  0x00a0：15.625MHz, 0x0020:78.125
            SendCmdToLMX2595(adcIndex, 0x23, 0x0004);
            SendCmdToLMX2595(adcIndex, 0x22, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x21, 0x1E21);
            SendCmdToLMX2595(adcIndex, 0x20, 0x0393);
            SendCmdToLMX2595(adcIndex, 0x1F, 0x43EC);
            SendCmdToLMX2595(adcIndex, 0x1E, 0x318C);
            SendCmdToLMX2595(adcIndex, 0x1D, 0x318C);
            SendCmdToLMX2595(adcIndex, 0x1C, 0x0488);
            SendCmdToLMX2595(adcIndex, 0x1B, 0x0002);
            SendCmdToLMX2595(adcIndex, 0x1A, 0x0DB0);
            SendCmdToLMX2595(adcIndex, 0x19, 0x0C2B);
            SendCmdToLMX2595(adcIndex, 0x18, 0x071A);
            SendCmdToLMX2595(adcIndex, 0x17, 0x007C);
            SendCmdToLMX2595(adcIndex, 0x16, 0x0001);
            SendCmdToLMX2595(adcIndex, 0x15, 0x0401);
            SendCmdToLMX2595(adcIndex, 0x14, 0xD848);
            SendCmdToLMX2595(adcIndex, 0x13, 0x27B7);
            SendCmdToLMX2595(adcIndex, 0x12, 0x0064);
            SendCmdToLMX2595(adcIndex, 0x11, 0x0130);
            SendCmdToLMX2595(adcIndex, 0x10, 0x0080);
            SendCmdToLMX2595(adcIndex, 0x0F, 0x064F);
            SendCmdToLMX2595(adcIndex, 0x0E, 0x1E70);
            SendCmdToLMX2595(adcIndex, 0x0D, 0x4000);
            SendCmdToLMX2595(adcIndex, 0x0C, 0x5001);
            SendCmdToLMX2595(adcIndex, 0x0B, 0x0018);
            SendCmdToLMX2595(adcIndex, 0x0A, 0x10D8);
            SendCmdToLMX2595(adcIndex, 0x09, 0x0604);
            SendCmdToLMX2595(adcIndex, 0x08, 0x2000);
            SendCmdToLMX2595(adcIndex, 0x07, 0x40B2);
            SendCmdToLMX2595(adcIndex, 0x06, 0xC802);
            SendCmdToLMX2595(adcIndex, 0x05, 0x00C8);
            SendCmdToLMX2595(adcIndex, 0x04, 0x0A43);
            SendCmdToLMX2595(adcIndex, 0x03, 0x0642);
            SendCmdToLMX2595(adcIndex, 0x02, 0x0500);
            SendCmdToLMX2595(adcIndex, 0x01, 0x0808);
            //SendCmdToLMX2595(adcIndex, 0x00, 0x641C);
            //FCLA_EN 
            HdIO.Sleep(10);
            SendCmdToLMX2595(adcIndex, 0x00, 0x641C);
            //HdCommand.PCIX_WriteRegister32_Debug(AD9689_CONFIG_EN1, 0x00000000);
        }
        #endregion

        #region JESD204B链路建立与多片同步
        private void JESD204B_RST(AcqBdNo fpgaIndex)
        {
            //HdCommand.PCIX_WriteRegister32(JESD204B_CORE_RST | CTRL_INDEPENT_REG[index], 0x0002);
            HdIO.Sleep(10);
            WriteReg(AcqBdReg.W.Adc_204bConfig_Reset, fpgaIndex, 0x03);
            HdIO.Sleep(50);
            WriteReg(AcqBdReg.W.Adc_204bConfig_Reset, fpgaIndex, 0x02);
        }
        private void multi_sync_rst(AcqBdNo fpgaIndex)
        {
            WriteReg(AcqBdReg.W.Adc_204bConfig_Reset, fpgaIndex, 0x0000);
            HdIO.Sleep(10);
            WriteReg(AcqBdReg.W.Adc_204bConfig_Reset, fpgaIndex, 0x0002);
            HdIO.Sleep(50);
            WriteReg(AcqBdReg.W.Adc_204bConfig_Reset, fpgaIndex, 0x0000);
        }
        #endregion

        record NeedInitAD5200Config(Int32 ChannelID, int SubbandIndex, AcqBdNo FpgaIndex, int AdcIndex, bool bPhase, bool bGian, bool bReCalc0x29Register);

        private void BuildAdc5200LinkRoad(List<NeedInitAD5200Config> needInitAD5200Configs)
        {
            foreach (NeedInitAD5200Config configParam in needInitAD5200Configs)
            {
                AD5200_Init(configParam.ChannelID, configParam.FpgaIndex, configParam.SubbandIndex, configParam.AdcIndex, 1);//包含发送校准数据
                AD5200_Init(configParam.ChannelID, configParam.FpgaIndex, configParam.SubbandIndex, configParam.AdcIndex, 2);//包含发送校准数据
            }

                HdIO.Sleep(500);

            #region JESD204B_RST
            //JESD204B_RST Build Finish
            WriteToAllFpga(AcqBdReg.W.Adc_204bConfig_Reset, 0x01);
            HdIO.Sleep(200);
            WriteToAllFpga(AcqBdReg.W.Adc_204bConfig_Reset, 0x00);
            HdIO.Sleep(500);
            //       uint data = 0;
            //      data = ReadFromAD5200((AcqBdNo)0, 0, 0x2e);
            //        HdCtrl_Pll.PllSync_A();//*****01
            WriteReg(AcqBdReg.W.Adc_CS, (AcqBdNo)0, (UInt32)(0 + 1));
            uint data = ReadFromAD5200((AcqBdNo)0, 0, 0x208);
            data = ReadFromAD5200((AcqBdNo)0, 0, 0x208);
            WriteReg(AcqBdReg.W.Adc_CS, (AcqBdNo)0, 0);
            WriteReg(AcqBdReg.W.Adc_CS, (AcqBdNo)1, (UInt32)(0 + 1));
            uint data1 = ReadFromAD5200((AcqBdNo)1, 0, 0x208);
            data1 = ReadFromAD5200((AcqBdNo)1, 0, 0x208);
            WriteReg(AcqBdReg.W.Adc_CS, (AcqBdNo)1, 0);
            WriteReg(AcqBdReg.W.Adc_CS, (AcqBdNo)2, (UInt32)(0 + 1));
            uint data2 = ReadFromAD5200((AcqBdNo)2, 0, 0x208);
            data2 = ReadFromAD5200((AcqBdNo)2, 0, 0x208);
            WriteReg(AcqBdReg.W.Adc_CS, (AcqBdNo)2, 0);

            HdIO.Sleep(800);
            #endregion



        }
        private void InitAll5200At20GMode()
        {
    //        HdIO.Sleep(30000);
            WriteToAllFpga(AcqBdReg.W.PllConfig_7044Reset, 0x00);
            HdIO.Sleep(10);
            WriteToAllFpga(AcqBdReg.W.FPGAFlashUpdater_SS, 0x00);
            HdIO.Sleep(10);
            WriteToAllFpga(AcqBdReg.W.Adc_204bConfig_Reset, 0x01);
            //HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_0, 0x03);
            HdIO.Sleep(10);
            //lchy -
            WriteToAllFpga(AcqBdReg.W.Adc_PowerCtrl, 0x03);
            HdIO.Sleep(10);
            WriteToAllFpga(AcqBdReg.W.Adc_ConfigEnable, 0x00);
            HdIO.Sleep(20);
            WriteToAllFpga(AcqBdReg.W.Adc_PowerCtrl, 0x00);
            HdIO.Sleep(20);

            //InitHMC7044_Adc5200();//00
            //          InitHMC7044_Adc5200_8G();
            //           HdIO.Sleep(20);
            //           HdCtrl_Pll.PllSync_A();/****01
            //           HdIO.Sleep(100); //100

            WriteToAllFpga(AcqBdReg.W.PllConfig_LMK00304Config1, 0x05);//0X09
            HdIO.Sleep(500);


            //全部需要初始化,但不需要重新校准0x29寄存器
            List<NeedInitAD5200Config> needInitAD5200Config = new List<NeedInitAD5200Config>();
            //此时还没有通道和幅度档等信息。全部的5200需要初始化
            //需要发送ADC的Phase（与通道组合无关）、Gain（与通道组合有关）、同步压稳态窗（与通道组合无关）
            int subbandIndex = 0;
            for (int fpgaIndex = 0; fpgaIndex < ExistsDefines.Count; fpgaIndex++)
            {
                if (ExistsDefines[fpgaIndex].ISENABLE)
                {
                    needInitAD5200Config.Add(new NeedInitAD5200Config(0, subbandIndex, (AcqBdNo)fpgaIndex, 0, true, false, false));//ADC1
                    needInitAD5200Config.Add(new NeedInitAD5200Config(0, subbandIndex, (AcqBdNo)fpgaIndex, 1, true, false, false));//ADC2
                                                                                                                                   //                   needInitAD5200Config.Add(new NeedInitAD5200Config(0, subbandIndex, (AcqBdNo)fpgaIndex, 2, true, false, false));//ADC1
                                                                                                                                   //                   needInitAD5200Config.Add(new NeedInitAD5200Config(0, subbandIndex, (AcqBdNo)fpgaIndex, 3, true, false, false));//ADC2
                    subbandIndex++;
                }
            }
            //临时注释
            BuildAdc5200LinkRoad(needInitAD5200Config);//在此项目中，ADC相位、同步稳态窗的设置在此函数中进行，而Gain与通道有关，不设置。

            HdIO.Sleep(500);
        }

        private bool CtrlGainByFpga()
        {
            return true;
        }
        public override bool MiscFunc(string FuncName)
        {
            return FuncName switch
            {
                "CtrlGainByFpga" => CtrlGainByFpga(),
                _ => false,
            };
        }

        public override string ReadADC5200SyncWindowRegValue()
        {
            StringBuilder stringBuilder = new StringBuilder();
            int fpgaIndex = 0;
            foreach ((String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC) exists in ExistsDefines)
            {
                if (exists.ISENABLE)
                {
                    for (int perPhyChannelAdcIndex = 0; perPhyChannelAdcIndex < Constants.ADC_NUM; perPhyChannelAdcIndex++)
                    {
                        WriteReg(AcqBdReg.W.Adc_CS, (AcqBdNo)fpgaIndex, (UInt32)(perPhyChannelAdcIndex + 1));
                        uint data24Bit = 0;
                        uint data = 0;
                        data = ReadFromAD5200((AcqBdNo)fpgaIndex, perPhyChannelAdcIndex, 0x2e);
                        data24Bit |= (data & 0xff);
                        data24Bit <<= 8;
                        data = ReadFromAD5200((AcqBdNo)fpgaIndex, perPhyChannelAdcIndex, 0x2d);
                        data24Bit |= (data & 0xff);
                        data24Bit <<= 8;
                        data = ReadFromAD5200((AcqBdNo)fpgaIndex, perPhyChannelAdcIndex, 0x2c);
                        data24Bit |= (data & 0xff);
                        WriteReg(AcqBdReg.W.Adc_CS, (AcqBdNo)fpgaIndex, 0);
                        string bitStr = "";
                        for (int bitIndex = 23; bitIndex >= 0; bitIndex--)
                        {
                            if ((bitIndex + 1) % 4 == 0 && bitIndex != 23)
                                bitStr += "_";
                            bitStr += (data24Bit & (1 << bitIndex)) == 0 ? '0' : '1';
                        }
                        stringBuilder.AppendLine($"B{fpgaIndex + 1}.Adc{perPhyChannelAdcIndex + 1}=>{new String(bitStr)}");
                    }
                }
                fpgaIndex++;
            }
            return stringBuilder.ToString();
        }

        public virtual string ReadADC5200RegStatus()
        {
            string resultStr = "";
            StringBuilder stringBuilder = new StringBuilder();

            uint[] address =
            {
                    0x000,
                    0x002,
                    0x003,
                    0x004,
                    0x00C,
                    0x010,
                    0x029,
                    0x02A,
                    0x02B,
                    0x02C,
                    0x030,
                    0x032,
                    0x038,
                    0x03B,
                    0x048,
                    0x060,
                    0x061,
                    0x062,
                    0x064,
                    0x068,
                    0x06A,
                    0x06B,
                    0x06C,
                    0x06E,
                    0x070,
                    0x071,
                    0x07A,
                    0x07B,
                    0x07C,
                    0x07E,
                    0x07F,
                    0x09D,
                    0x160,
                    0x200,
                    0x201,
                    0x202,
                    0x203,
                    0x204,
                    0x205,
                    0x206,
                    0x207,
                    0x208,
                    0x209,
                    0x20A,
                    0x20B,
                    0x20F,
                    0x210,
                    0x211,
                    0x212,
                    0x213,
                    0x214,
                    0x215,
                    0x216,
                    0x217,
                    0x219,
                    0x220,
                    0x224,
                    0x228,
                    0x22C,
                    0x230,
                    0x234,
                    0x238,
                    0x23C,
                    0x240,
                    0x244,
                    0x248,
                    0x24C,
                    0x250,
                    0x254,
                    0x258,
                    0x25C,
                    0x270,
                    0x297,
                    0x2A2,
                    0x2B0,
                    0x2B1,
                    0x2B2,
                    0x2B5,
                    0x2B8,
                    0x2C0,
                    0x2C1,
                    0x2C2,
                    0x2C4,
                    0x310,
                    0x313,
                    0x314,
                    0x315,
                    0x31A,
                    0x31B,
                    0x344,
                    0x346,
                    0x348,
                    0x34A,
                    0x34C,
                    0x34E,
                    0x350,
                    0x351,
                    0x352,
                    0x353,
                    0x354,
                    0x355,
                    0x356,
                    0x357,
                    0x400,
                    0x418,
                    0x41A,
                    0x41C,
                    0x41E,
                    0x420,
                    0x423,
                    0x425,
                    0x427,
                    0x429,
                    0x448,
                    0x44A,
                    0x44C,
                    0x44E,
                    0x450,
                    0x453,
                    0x455,
                    0x457,
                    0x459
                };


            int fpgaIndex = 0;
            foreach ((String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC) exists in ExistsDefines)
            {
                if (exists.ISENABLE)
                {
                    for (int perPhyChannelAdcIndex = 0; perPhyChannelAdcIndex < Constants.ADC_NUM; perPhyChannelAdcIndex++)
                    {
                        foreach (uint addr in address)
                        {
                            WriteReg(AcqBdReg.W.Adc_CS, (AcqBdNo)fpgaIndex, (UInt32)(perPhyChannelAdcIndex + 1));
                            UInt32 readBackData = ReadFromAD5200((AcqBdNo)fpgaIndex, perPhyChannelAdcIndex, addr);
                            WriteReg(AcqBdReg.W.Adc_CS, (AcqBdNo)fpgaIndex, 0);
                            string? RWaddr = ",address=0x" + addr.ToString("X3");
                            stringBuilder.AppendLine("ACQ " + (((AcqBdNo)fpgaIndex).ToString()).PadRight(1) + (" ADC" + (perPhyChannelAdcIndex + 1).ToString()).PadRight(10) + RWaddr.PadRight(10) + ", value=0x" + readBackData.ToString("X2"));
                            HdIO.Sleep(1);
                        }
                    }
                }
                fpgaIndex++;
            }

            resultStr = stringBuilder.ToString();

            string filetime = DateTime.Now.ToString("yyyy-MM-dd");
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReadbackStatus", filetime);
            string filePath = Path.Combine(folderPath, $"ADC5200RegStatus.txt");

            // 检查文件夹是否存在，不存在则创建 
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (File.Exists(filePath))
            {
                FileInfo fInfo = new FileInfo(filePath);
                if (fInfo.Length > 2000_000) //大于2M删除文件
                    File.Delete(filePath);
            }
            using (StreamWriter outputFile = new StreamWriter(filePath, true))
            {
                outputFile.WriteLine($"[{DateTime.Now}]\r\n" + resultStr);
            }

            return resultStr;
        }

        internal virtual UInt32 ReadFromLMX2595(AcqBdNo fpgaIndex, Int32 adcIndex, UInt32 Address_7bit)
        {

            UInt32 tmp = ((0x001 << 23) | (Address_7bit << 16) | 0x0000);//(0x001 << 23)Instruction R(1b'1)/W(1b'0)

            WriteReg(AcqBdReg.W.PllConfig_LMX2595Data_L8, fpgaIndex, tmp & 0xff); //10'h00:ADC_SPI_data[7:0] <= adsp_databus_wr[7:0];
            WriteReg(AcqBdReg.W.PllConfig_LMX2595Data_H16, fpgaIndex, (tmp >> 8) & 0xffff);//10'h01:ADC_SPI_data[23:8] <= adsp_databus_wr[15:0];
            (UInt32 first, UInt32 second) = adcIndex switch
            {
                0 => (0x00U, 0x40U),//0000 0000,0100 0000
                1 => (0x40U, 0xc0U),//0100 0000,1100 0000
                _ => (0x00U, 0xc0U) //0000 0000,1100 0000 0&1
            };
            WriteReg(AcqBdReg.W.PllConfig_LMX2595Data_Effect, fpgaIndex, first);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(1);
            WriteReg(AcqBdReg.W.PllConfig_LMX2595Data_Effect, fpgaIndex, second);
            HdIO.Sleep(1);

            UInt32 readBackData = adcIndex switch
            {
                0 => ReadReg(AcqBdReg.R.PllConfig_LMX2595A_SpiReadBackData, fpgaIndex),
                1 => ReadReg(AcqBdReg.R.PllConfig_LMX2595B_SpiReadBackData, fpgaIndex),
                _ => ReadReg(AcqBdReg.R.PllConfig_LMX2595A_SpiReadBackData, fpgaIndex)
            };

            return readBackData;
        }

        public virtual string ReadLMX2595RegStatus()
        {
            string resultStr = "";
            StringBuilder stringBuilder = new StringBuilder();

            uint[] address =
            {
                0x00,
                0x01,
                0x02,
                0x03,
                0x04,
                0x05,
                0x06,
                0x07,
                0x08,
                0x09,
                0x0A,
                0x0B,
                0x0C,
                0x0D,
                0x0E,
                0x0F,
                0x10,
                0x11,
                0x12,
                0x13,
                0x14,
                0x15,
                0x16,
                0x17,
                0x18,
                0x19,
                0x1A,
                0x1B,
                0x1C,
                0x1D,
                0x1E,
                0x1F,
                0x20,
                0x21,
                0x22,
                0x23,
                0x24,
                0x25,
                0x26,
                0x27,
                0x28,
                0x29,
                0x2A,
                0x2B,
                0x2C,
                0x2D,
                0x2E,
                0x2F,
                0x30,
                0x31,
                0x32,
                0x33,
                0x34,
                0x35,
                0x36,
                0x37,
                0x38,
                0x39,
                0x3A,
                0x3B,
                0x3C,
                0x3D,
                0x3E,
                0x3F,
                0x40,
                0x41,
                0x42,
                0x43,
                0x44,
                0x45,
                0x46,
                0x47,
                0x48,
                0x49,
                0x4A,
                0x4B,
                0x4C,
                0x4D,
                0x4E,
                0x4F,
                0x50,
                0x51,
                0x52,
                0x53,
                0x54,
                0x55,
                0x56,
                0x57,
                0x58,
                0x59,
                0x5A,
                0x5B,
                0x5C,
                0x5D,
                0x5E,
                0x5F,
                0x60,
                0x61,
                0x62,
                0x63,
                0x64,
                0x65,
                0x66,
                0x67,
                0x68,
                0x69,
                0x6A,
                0x6B,
                0x6C,
                0x6D,
                0x6E,
                0x6F,
                0x70,
            };


            int fpgaIndex = 0;
            foreach ((String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC) exists in ExistsDefines)
            {
                if (exists.ISENABLE)
                {
                    for (int perPhyChannelAdcIndex = 0; perPhyChannelAdcIndex < Constants.ADC_NUM; perPhyChannelAdcIndex++)
                    {
                        SendCmdToLMX2595(perPhyChannelAdcIndex, 0x00, 0x6418);
                        HdIO.Sleep(10);
                        foreach (uint addr in address)
                        {

                            UInt32 readBackData = ReadFromLMX2595((AcqBdNo)fpgaIndex, perPhyChannelAdcIndex, addr);

                            string? RWaddr = ",address=0x" + addr.ToString("X2");
                            stringBuilder.AppendLine("ACQ " + (((AcqBdNo)fpgaIndex).ToString()).PadRight(1) + (" LMX2595PLL" + (perPhyChannelAdcIndex + 1).ToString()).PadRight(10) + RWaddr.PadRight(10) + ", value=0x" + readBackData.ToString("X4"));
                            HdIO.Sleep(1);
                        }
                        HdIO.Sleep(10);
                        SendCmdToLMX2595(perPhyChannelAdcIndex, 0x00, 0x641C);
                        HdIO.Sleep(10);
                    }
                }
                fpgaIndex++;
            }
            resultStr = stringBuilder.ToString();

            string filetime = DateTime.Now.ToString("yyyy-MM-dd");
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReadbackStatus", filetime);
            string filePath = Path.Combine(folderPath, $"LMX2595RegStatus.txt");

            // 检查文件夹是否存在，不存在则创建 
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (File.Exists(filePath))
            {
                FileInfo fInfo = new FileInfo(filePath);
                if (fInfo.Length > 2000_000) //大于2M删除文件
                    File.Delete(filePath);
            }
            using (StreamWriter outputFile = new StreamWriter(filePath, true))
            {
                outputFile.WriteLine($"[{DateTime.Now}]\r\n" + resultStr);
            }

            return resultStr;
        }

        internal virtual UInt32 HMC7044Read(AcqBdNo fpgaIndex, UInt32 ADDR)
        {
            UInt32 SDATA;//SDATA's width is 24bits
            UInt32 RorW = 0x00800000;//MSB bit23 R/W bit22(W1) bit21(W0) Multibyte field 2'b00  RorW[23:21]=1|00
            SDATA = RorW | ((ADDR << 8) & 0x001fff00) | (0x00 & 0x000000ff);

            WriteToAllFpga(AcqBdReg.W.PllConfig_HMC7044Data_Effect, 0x00);
            WriteToAllFpga(AcqBdReg.W.PllConfig_HMC7044Data_L16, SDATA & 0xffff);
            WriteToAllFpga(AcqBdReg.W.PllConfig_HMC7044Data_H8, (SDATA >> 16) & 0xffff);
            WriteToAllFpga(AcqBdReg.W.PllConfig_HMC7044Data_Effect, 0x01);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(1);
            UInt32 readBackData = ReadReg(AcqBdReg.R.PllConfig_HMC7044_SpiReadBackData, fpgaIndex) & 0xff;
            return readBackData;
        }

        public virtual string ReadHMC7044RegStatus()
        {
            string resultStr = "";
            StringBuilder stringBuilder = new StringBuilder();

            uint[] address =
            {
                0x000,
                0x001,
                0x002,
                0x003,
                0x004,
                0x005,
                0x006,
                0x007,
                0x009,
                0x00A,
                0x00B,
                0x00C,
                0x00D,
                0x00E,
                0x014,
                0x015,
                0x016,
                0x017,
                0x018,
                0x019,
                0x01A,
                0x01B,
                0x01C,
                0x01D,
                0x01E,
                0x01F,
                0x020,
                0x021,
                0x022,
                0x026,
                0x027,
                0x028,
                0x029,
                0x02A,
                0x031,
                0x032,
                0x033,
                0x034,
                0x035,
                0x036,
                0x037,
                0x038,
                0x039,
                0x03A,
                0x03B,
                0x046,
                0x047,
                0x048,
                0x049,
                0x050,
                0x051,
                0x052,
                0x053,
                0x054,
                0x05A,
                0x05B,
                0x05C,
                0x05D,
                0x064,
                0x065,
                0x070,
                0x071,
                0x078,
                0x079,
                0x07A,
                0x07B,
                0x07C,
                0x07D,
                0x07E,
                0x082,
                0x083,
                0x084,
                0x085,
                0x086,
                0x08C,
                0x08D,
                0x08E,
                0x08F,
                0x091,
                0x096,
                0x097,
                0x098,
                0x099,
                0x09A,
                0x09B,
                0x09C,
                0x09D,
                0x09E,
                0x09F,
                0x0A0,
                0x0A1,
                0x0A2,
                0x0A3,
                0x0A4,
                0x0A5,
                0x0A6,
                0x0A7,
                0x0A8,
                0x0A9,
                0x0AB,
                0x0AC,
                0x0AD,
                0x0AE,
                0x0AF,
                0x0B0,
                0x0B1,
                0x0B2,
                0x0B3,
                0x0B5,
                0x0B6,
                0x0B7,
                0x0B8,
                0x0C8,
                0x0C9,
                0x0CA,
                0x0CB,
                0x0CC,
                0x0CD,
                0x0CE,
                0x0CF,
                0x0D0,
                0x0D2,
                0x0D3,
                0x0D4,
                0x0D5,
                0x0D6,
                0x0D7,
                0x0D8,
                0x0D9,
                0x0DA,
                0x0DC,
                0x0DD,
                0x0DE,
                0x0DF,
                0x0E0,
                0x0E1,
                0x0E2,
                0x0E3,
                0x0E4,
                0x0E6,
                0x0E7,
                0x0E8,
                0x0E9,
                0x0EA,
                0x0EB,
                0x0EC,
                0x0ED,
                0x0EE,
                0x0F0,
                0x0F1,
                0x0F2,
                0x0F3,
                0x0F4,
                0x0F5,
                0x0F6,
                0x0F7,
                0x0F8,
                0x0FA,
                0x0FB,
                0x0FC,
                0x0FD,
                0x0FE,
                0x0FF,
                0x100,
                0x101,
                0x102,
                0x104,
                0x105,
                0x106,
                0x107,
                0x108,
                0x109,
                0x10A,
                0x10B,
                0x10C,
                0x10E,
                0x10F,
                0x110,
                0x111,
                0x112,
                0x113,
                0x114,
                0x115,
                0x116,
                0x118,
                0x119,
                0x11A,
                0x11B,
                0x11C,
                0x11D,
                0x11E,
                0x11F,
                0x120,
                0x122,
                0x123,
                0x124,
                0x125,
                0x126,
                0x127,
                0x128,
                0x129,
                0x12A,
                0x12C,
                0x12D,
                0x12E,
                0x12F,
                0x130,
                0x131,
                0x132,
                0x133,
                0x134,
                0x136,
                0x137,
                0x138,
                0x139,
                0x13A,
                0x13B,
                0x13C,
                0x13D,
                0x13E,
                0x140,
                0x141,
                0x142,
                0x143,
                0x144,
                0x145,
                0x146,
                0x147,
                0x148,
                0x14A,
                0x14B,
                0x14C,
                0x14D,
                0x14E,
                0x14F,
                0x150,
                0x151,
                0x152,
            };

            int fpgaIndex = 0;
            foreach ((String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC) exists in ExistsDefines)
            {
                if (exists.ISENABLE)
                {

                    foreach (uint addr in address)
                    {

                        UInt32 readBackData = HMC7044Read((AcqBdNo)fpgaIndex, addr);

                        string? RWaddr = ",address=0x" + addr.ToString("X3");
                        stringBuilder.AppendLine("ACQ " + (((AcqBdNo)fpgaIndex).ToString()).PadRight(1) + (" HMC7044").PadRight(10) + RWaddr.PadRight(10) + ", value=0x" + readBackData.ToString("X2"));
                        HdIO.Sleep(1);
                    }

                }
                fpgaIndex++;
            }
            resultStr = stringBuilder.ToString();

            string filetime = DateTime.Now.ToString("yyyy-MM-dd");
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReadbackStatus", filetime);
            string filePath = Path.Combine(folderPath, $"HMC7044RegStatus.txt");
            //try
            //{
            // 检查文件夹是否存在，不存在则创建 
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (File.Exists(filePath))
            {
                FileInfo fInfo = new FileInfo(filePath);
                if (fInfo.Length > 2000_000) //大于2M删除文件
                    File.Delete(filePath);
            }
            using (StreamWriter outputFile = new StreamWriter(filePath, true))
            {
                outputFile.WriteLine($"[{DateTime.Now}]\r\n" + resultStr);
            }
            //}
            //catch (Exception ex)
            //{
            //    throw new ArgumentOutOfRangeException("Error", ex);
            //}
            return resultStr;
        }

        internal virtual UInt32 ProHMC7044Read(UInt32 ADDR)
        {
            UInt32 SDATA;//SDATA's width is 24bits
            UInt32 RorW = 0x00800000;//MSB bit23 R/W bit22(W1) bit21(W0) Multibyte field 2'b00  RorW[23:21]=1|00
            SDATA = RorW | ((ADDR << 8) & 0x001fff00) | (0x00 & 0x000000ff);

            HdIO.WriteReg(S6BdReg.W.PllConfig_7044WriteDataEffect, 0x00);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044WriteData_L16, SDATA & 0xffff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044WriteData_H8, (SDATA >> 16) & 0xffff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7044WriteDataEffect, 0x01);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(1);
            UInt32 readBackData = HdIO.ReadReg(S6BdReg.R.PllConfig_HMC7044_SpiReadBackData) & 0xff;
            return readBackData;
        }

        public virtual string ReadProHMC7044RegStatus()
        {
            string resultStr = "";
            StringBuilder stringBuilder = new StringBuilder();

            uint[] address =
            {
                0x000,
                0x001,
                0x002,
                0x003,
                0x004,
                0x005,
                0x006,
                0x007,
                0x009,
                0x00A,
                0x00B,
                0x00C,
                0x00D,
                0x00E,
                0x014,
                0x015,
                0x016,
                0x017,
                0x018,
                0x019,
                0x01A,
                0x01B,
                0x01C,
                0x01D,
                0x01E,
                0x01F,
                0x020,
                0x021,
                0x022,
                0x026,
                0x027,
                0x028,
                0x029,
                0x02A,
                0x031,
                0x032,
                0x033,
                0x034,
                0x035,
                0x036,
                0x037,
                0x038,
                0x039,
                0x03A,
                0x03B,
                0x046,
                0x047,
                0x048,
                0x049,
                0x050,
                0x051,
                0x052,
                0x053,
                0x054,
                0x05A,
                0x05B,
                0x05C,
                0x05D,
                0x064,
                0x065,
                0x070,
                0x071,
                0x078,
                0x079,
                0x07A,
                0x07B,
                0x07C,
                0x07D,
                0x07E,
                0x082,
                0x083,
                0x084,
                0x085,
                0x086,
                0x08C,
                0x08D,
                0x08E,
                0x08F,
                0x091,
                0x096,
                0x097,
                0x098,
                0x099,
                0x09A,
                0x09B,
                0x09C,
                0x09D,
                0x09E,
                0x09F,
                0x0A0,
                0x0A1,
                0x0A2,
                0x0A3,
                0x0A4,
                0x0A5,
                0x0A6,
                0x0A7,
                0x0A8,
                0x0A9,
                0x0AB,
                0x0AC,
                0x0AD,
                0x0AE,
                0x0AF,
                0x0B0,
                0x0B1,
                0x0B2,
                0x0B3,
                0x0B5,
                0x0B6,
                0x0B7,
                0x0B8,
                0x0C8,
                0x0C9,
                0x0CA,
                0x0CB,
                0x0CC,
                0x0CD,
                0x0CE,
                0x0CF,
                0x0D0,
                0x0D2,
                0x0D3,
                0x0D4,
                0x0D5,
                0x0D6,
                0x0D7,
                0x0D8,
                0x0D9,
                0x0DA,
                0x0DC,
                0x0DD,
                0x0DE,
                0x0DF,
                0x0E0,
                0x0E1,
                0x0E2,
                0x0E3,
                0x0E4,
                0x0E6,
                0x0E7,
                0x0E8,
                0x0E9,
                0x0EA,
                0x0EB,
                0x0EC,
                0x0ED,
                0x0EE,
                0x0F0,
                0x0F1,
                0x0F2,
                0x0F3,
                0x0F4,
                0x0F5,
                0x0F6,
                0x0F7,
                0x0F8,
                0x0FA,
                0x0FB,
                0x0FC,
                0x0FD,
                0x0FE,
                0x0FF,
                0x100,
                0x101,
                0x102,
                0x104,
                0x105,
                0x106,
                0x107,
                0x108,
                0x109,
                0x10A,
                0x10B,
                0x10C,
                0x10E,
                0x10F,
                0x110,
                0x111,
                0x112,
                0x113,
                0x114,
                0x115,
                0x116,
                0x118,
                0x119,
                0x11A,
                0x11B,
                0x11C,
                0x11D,
                0x11E,
                0x11F,
                0x120,
                0x122,
                0x123,
                0x124,
                0x125,
                0x126,
                0x127,
                0x128,
                0x129,
                0x12A,
                0x12C,
                0x12D,
                0x12E,
                0x12F,
                0x130,
                0x131,
                0x132,
                0x133,
                0x134,
                0x136,
                0x137,
                0x138,
                0x139,
                0x13A,
                0x13B,
                0x13C,
                0x13D,
                0x13E,
                0x140,
                0x141,
                0x142,
                0x143,
                0x144,
                0x145,
                0x146,
                0x147,
                0x148,
                0x14A,
                0x14B,
                0x14C,
                0x14D,
                0x14E,
                0x14F,
                0x150,
                0x151,
                0x152
            };

            //if (exists)
            //{

            foreach (uint addr in address)
            {

                UInt32 readBackData = ProHMC7044Read(addr);

                string? RWaddr = ",address=0x" + addr.ToString("X3");
                stringBuilder.AppendLine("PRO " + (" HMC7044").PadRight(10) + RWaddr.PadRight(10) + ", value=0x" + readBackData.ToString("X2"));
                HdIO.Sleep(1);
            }
            //}
            resultStr = stringBuilder.ToString();

            string filetime = DateTime.Now.ToString("yyyy-MM-dd");
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReadbackStatus", filetime);
            string filePath = Path.Combine(folderPath, $"ProHMC7044RegStatus.txt");
            //try
            //{
            // 检查文件夹是否存在，不存在则创建 
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (File.Exists(filePath))
            {
                FileInfo fInfo = new FileInfo(filePath);
                if (fInfo.Length > 2000_000) //大于2M删除文件
                    File.Delete(filePath);
            }
            using (StreamWriter outputFile = new StreamWriter(filePath, true))
            {
                outputFile.WriteLine($"[{DateTime.Now}]\r\n" + resultStr);
            }
            //}
            //catch (Exception ex)
            //{
            //    throw new ArgumentOutOfRangeException("Error", ex);
            //}
            return resultStr;
        }

        internal virtual UInt32 ProHMC7043Read(UInt32 ADDR)
        {
            UInt32 SDATA;//SDATA's width is 24bits
            UInt32 RorW = 0x00800000;//MSB bit23 R/W bit22(W1) bit21(W0) Multibyte field 2'b00  RorW[23:21]=1|00
            SDATA = RorW | ((ADDR << 8) & 0x001fff00) | (0x00 & 0x000000ff);

            HdIO.WriteReg(S6BdReg.W.PllConfig_7043WriteDataEffect, 0x00);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7043WriteData_L16, SDATA & 0xffff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7043WriteData_H8, (SDATA >> 16) & 0xffff);
            HdIO.WriteReg(S6BdReg.W.PllConfig_7043WriteDataEffect, 0x01);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(1);
            UInt32 readBackData = HdIO.ReadReg(S6BdReg.R.PllConfig_HMC7043_SpiReadBackData) & 0xff;
            return readBackData;
        }

        public virtual string ReadProHMC7043RegStatus()
        {
            string resultStr = "";
            StringBuilder stringBuilder = new StringBuilder();

            uint[] address =
            {
                0x000,
                0x001,
                0x002,
                0x003,
                0x004,
                0x006,
                0x007,
                0x00A,
                0x00B,
                0x046,
                0x050,
                0x054,
                0x05A,
                0x05B,
                0x05C,
                0x05D,
                0x064,
                0x065,
                0x071,
                0x078,
                0x079,
                0x07A,
                0x07D,
                0x091,
                0x098,
                0x099,
                0x09A,
                0x09B,
                0x09C,
                0x09D,
                0x09E,
                0x09F,
                0x0A0,
                0x0A1,
                0x0A2,
                0x0A3,
                0x0A4,
                0x0AD,
                0x0AE,
                0x0AF,
                0x0B0,
                0x0B1,
                0x0B2,
                0x0B3,
                0x0B5,
                0x0B6,
                0x0B7,
                0x0B8,
                0x0C8,
                0x0C9,
                0x0CA,
                0x0CB,
                0x0CC,
                0x0CD,
                0x0CE,
                0x0CF,
                0x0D0,
                0x0D2,
                0x0D3,
                0x0D4,
                0x0D5,
                0x0D6,
                0x0D7,
                0x0D8,
                0x0D9,
                0x0DA,
                0x0DC,
                0x0DD,
                0x0DE,
                0x0DF,
                0x0E0,
                0x0E1,
                0x0E2,
                0x0E3,
                0x0E4,
                0x0E6,
                0x0E7,
                0x0E8,
                0x0E9,
                0x0EA,
                0x0EB,
                0x0EC,
                0x0ED,
                0x0EE,
                0x0F0,
                0x0F1,
                0x0F2,
                0x0F3,
                0x0F4,
                0x0F5,
                0x0F6,
                0x0F7,
                0x0F8,
                0x0FA,
                0x0FB,
                0x0FC,
                0x0FD,
                0x0FE,
                0x0FF,
                0x100,
                0x101,
                0x102,
                0x104,
                0x105,
                0x106,
                0x107,
                0x108,
                0x109,
                0x10A,
                0x10B,
                0x10C,
                0x10E,
                0x10F,
                0x110,
                0x111,
                0x112,
                0x113,
                0x114,
                0x115,
                0x116,
                0x118,
                0x119,
                0x11A,
                0x11B,
                0x11C,
                0x11D,
                0x11E,
                0x11F,
                0x120,
                0x122,
                0x123,
                0x124,
                0x125,
                0x126,
                0x127,
                0x128,
                0x129,
                0x12A,
                0x12C,
                0x12D,
                0x12E,
                0x12F,
                0x130,
                0x131,
                0x132,
                0x133,
                0x134,
                0x136,
                0x137,
                0x138,
                0x139,
                0x13A,
                0x13B,
                0x13C,
                0x13D,
                0x13E,
                0x140,
                0x141,
                0x142,
                0x143,
                0x144,
                0x145,
                0x146,
                0x147,
                0x148,
                0x14A,
                0x14B,
                0x14C,
                0x14D,
                0x14E,
                0x14F,
                0x150,
                0x151,
                0x152,
            };

            //if (exists)
            //{

            foreach (uint addr in address)
            {

                UInt32 readBackData = ProHMC7043Read(addr);

                string? RWaddr = ",address=0x" + addr.ToString("X3");
                stringBuilder.AppendLine("PRO " + (" HMC7043").PadRight(10) + RWaddr.PadRight(10) + ", value=0x" + readBackData.ToString("X2"));
                HdIO.Sleep(1);
            }

            //}

            resultStr = stringBuilder.ToString();

            string filetime = DateTime.Now.ToString("yyyy-MM-dd");
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ReadbackStatus", filetime);
            string filePath = Path.Combine(folderPath, $"ProHMC7043RegStatus.txt");
            //try
            //{
            // 检查文件夹是否存在，不存在则创建 
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (File.Exists(filePath))
            {
                FileInfo fInfo = new FileInfo(filePath);
                if (fInfo.Length > 2000_000) //大于2M删除文件
                    File.Delete(filePath);
            }
            using (StreamWriter outputFile = new StreamWriter(filePath, true))
            {
                outputFile.WriteLine($"[{DateTime.Now}]\r\n" + resultStr);
            }
            //}
            //catch (Exception ex)
            //{
            //    throw new ArgumentOutOfRangeException("Error", ex);
            //}
            return resultStr;
        }

        public override void TiAdc_ApplayAdc_SyncSampleClock()
        {
            var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
            AcqModeAndInterleaveDefine interdefine = analogacquiremodel.GetCurrentAcqModeInterleave()!;
            for (int boardid = 0; boardid < Constants.ACQ_BOARD_NUM; boardid++)
            {
                for (int adcIndex = 0; adcIndex < Constants.ADC_NUM; adcIndex++)
                {
                    uint data = interdefine.InterleaveMode == AdcInterleaveMode.Mode2To1 ? TiAdc_SyncSampleClock.Default[boardid][adcIndex].Sample20GClockDelay & 0x0f : TiAdc_SyncSampleClock.Default[boardid][adcIndex].Sample10GClockDelay & 0x0f;//只有低4位有效
                    Send5200CmdWithCS_OneFpga(GetAcqBdNo(boardid), adcIndex, 0x0029, 0x30 | data);//use SYSREF calibration,delay steps are finer,enable the SYSREF receiver circuit
                    Send5200CmdWithCS_OneFpga(GetAcqBdNo(boardid), adcIndex, 0x0029, 0x70 | data);//SYSREF_RECV_EN must be set before setting SYSREF_PROC_EN
                }
            }
        }

        public override void ConfigAdc()
        {
            SendChMode_SamplingMode();
            TiAdc_ApplyAdc_Phase_Offset_Gain();
        }

        public override void TiAdc_ApplyAdc_Phase_Offset_Gain()
        {
            HdDebugLogger.Log($"[{DateTime.Now}]: Adc Phase_Offset_Gain Start!");
            var analogAcquireModel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
            AcqModeAndInterleaveDefine interdefine = analogAcquireModel.GetCurrentAcqModeInterleave()!;
            Stopwatch stopwatch2 = Stopwatch.StartNew();
            stopwatch2.Start();
            foreach (var dtl in interdefine.Details)
            {
                var adcinfo = dtl.Value[0];
                foreach (var item in dtl.Value)
                {
                    adcinfo = item;
                    var usedadcs = analogAcquireModel.GetUsedAdcs(adcinfo);
                    foreach (var adcId in usedadcs)
                    {
                        var tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(new(interdefine.Name, dtl.Key, item.AcqBdNo, adcId % 2))!.Value;
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        stopwatch.Start();
                        AdjustAdc_Gain(true, adcinfo.AcqBdNo, (Int32)adcId, tiadcItem.Gain);//173ms
                        stopwatch.Stop();
                        var ss = stopwatch.ElapsedMilliseconds;
                        //AdjustAdc_Phase(true, adcinfo.AcqBdNo, (Int32)adcId, tiadcItem.Phase);//50ms
                        AdjustAdc_PhaseEx(true, adcinfo.AcqBdNo, (Int32)adcId, tiadcItem.Reserved0, tiadcItem.Reserved1);
                        AdjustAdc_FPGAADCDelay(dtl.Key, adcinfo.AcqBdNo, (Int32)adcId, (UInt32)tiadcItem.AdcDelay_FPGA);//fpga丢点

                        HdMessage.AnalogOptions analogparas = Hd.UIMessage!.Analog![(Int32)dtl.Key];
                        var scale = analogparas.ScaleBymV;
                        //Adjust_FpgaConfigOffset(adcinfo.AcqBdNo, (Int32)adcId, scale<50?(UInt32)tiadcItem.Offset_FPGA_10mv: (UInt32)tiadcItem.Offset_FPGA);//offset细调
                        Adjust_FpgaConfigOffset(adcinfo.AcqBdNo, (Int32)adcId, scale < 50 ? (UInt32)tiadcItem.Offset_FPGA : (UInt32)tiadcItem.Offset_FPGA);//offset细调
                        ////Adjust_FpgaConfigGain(adcInfo.AcqBdNo, (int)adcId, (uint)tiadcItem.Gain_FPGA);//gain细调
                    }
                }

            }
            stopwatch2.Stop();
            var sss = stopwatch2.ElapsedMilliseconds;
            HdDebugLogger.Log($"[{DateTime.Now}]: Adc Phase_Offset_Gain End!");
        }

        public void SendAdc_Phase_Offset_Gain(ChannelId channelId, AcqBdNo AcqBdNo, Int32 adcId, TiadcPhaseOffsetGainItem_Base tiadcItem)
        {
            AdjustAdc_Gain(true, AcqBdNo, (Int32)adcId, tiadcItem.Gain);
            //AdjustAdc_Phase(true, AcqBdNo, (Int32)adcId, tiadcItem.Phase);
            AdjustAdc_PhaseEx(true, AcqBdNo, (Int32)adcId, tiadcItem.Reserved0, tiadcItem.Reserved1);
            //AdjustAdc_Offset(adcInfo.AcqBdNo, (int)adcId, (uint)tiadcItem.Offset);
            AdjustAdc_FPGAADCDelay(channelId, AcqBdNo, (Int32)adcId, (UInt32)tiadcItem.AdcDelay_FPGA);//fpga丢点

            HdMessage.AnalogOptions analogparas = Hd.UIMessage!.Analog![(Int32)channelId];
            var scale = analogparas.ScaleBymV;
            //Adjust_FpgaConfigOffset(AcqBdNo, (Int32)adcId, scale < 50 ? (UInt32)tiadcItem.Offset_FPGA_10mv : (UInt32)tiadcItem.Offset_FPGA);//offset细调
            Adjust_FpgaConfigOffset(AcqBdNo, (Int32)adcId, scale < 50 ? (UInt32)tiadcItem.Offset_FPGA : (UInt32)tiadcItem.Offset_FPGA);//offset细调
            //Adjust_FpgaConfigOffset_10mv(AcqBdNo, (Int32)adcId, (UInt32)tiadcItem.Offset_FPGA);//offset细调
            Thread.Sleep(50);                                                                              //Adjust_FpgaConfigGain(adcInfo.AcqBdNo, (int)adcId, (uint)tiadcItem.Gain_FPGA);//gain细调
        }

        /// <summary>
        /// 下发通道间延时
        /// </summary>
        /// <param name="channelId">通道编号</param>
        /// <param name="fpgaIndex">采集板</param>
        /// <param name="adcIndex">adc索引</param>
        /// <param name="adcDelayErr">FPGA丢点数</param>
        private void AdjustAdc_FPGAADCDelay(ChannelId channelId, AcqBdNo fpgaIndex, Int32 adcIndex, UInt32 adcDelayErr = 0)
        {
            var analogAcquireModel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
            AcqModeAndInterleaveDefine interDefine = analogAcquireModel.GetCurrentAcqModeInterleave()!;
            Double samplingrate = interDefine.InterleaveMode == AdcInterleaveMode.Mode2To1 ? 2e10 : 1e10;
            Int32 temprdelaytime = 0;
            if (Hd.UIMessage?.Analog?[(Int32)channelId] == null)
            {
                temprdelaytime = 0;
            }
            else
            {
                temprdelaytime = Hd.CurrDebugVarints.bEnable_ChannelDelay ? Hd.UIMessage.Analog[(Int32)channelId].FirstStageDelay : 0;
            }
            //    目前FPGA是反着丢点，暂时先反向丢点
            AcqBdReg.W reg = (adcIndex % 2) == 1 ? AcqBdReg.W.Calibration_Adc1Delay : AcqBdReg.W.Calibration_Adc2Delay;
            adcDelayErr = (UInt32)(adcDelayErr + temprdelaytime);
            Hd.CurrProduct.AcqBd!.WriteReg(reg, fpgaIndex, adcDelayErr);
            ////   CIJ 20250517
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Calibration_Adc1Delay, 0x0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Calibration_Adc2Delay, 0x0);
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Calibration_Adc1Delay, AcqBdNo.B0, ((UInt32)0));     //通过控制地址的第7位拉高来生效数据
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Calibration_Adc2Delay, AcqBdNo.B0, ((UInt32)0));
        }

        private static Dictionary<ChannelId, AcqBdNo> acqbddefine = new()
        {
            [ChannelId.C1] = AcqBdNo.B0,
            [ChannelId.C2] = AcqBdNo.B1,
            [ChannelId.C3] = AcqBdNo.B2,
            [ChannelId.C4] = AcqBdNo.B3,
        };

        /// <summary>
        /// FGPA偏置细调
        /// </summary>
        /// <param name="fpgaIndex"></param>
        /// <param name="adcIndex"></param>
        /// <param name="offset"></param>
        private void Adjust_FpgaConfigOffset(AcqBdNo fpgaIndex, Int32 adcIndex, UInt32 offset)
        {
            //HdMessage.AnalogOptions analogParameters = Hd.UIMessage.Analog[(int)bd2Chnldefine[fpgaIndex]];

            //Double limitedPosition_uV = analogParameters.Scale * 1000 * 2;

            //Double posPosition = (Double)(analogParameters.Position);
            //Double bias = Math.Abs(posPosition) > limitedPosition_uV ? (posPosition > 0 ? (posPosition - limitedPosition_uV) : (posPosition + limitedPosition_uV)) : 0;//取大于n格以后的部分
            //posPosition -= bias;                              
            //uint uintPosition = (uint)((posPosition / analogParameters.Scale * Constants.SAMPS_PER_YDIV + offset) * 16);
            uint uintPosition = (uint)(offset * 16);
            if ((adcIndex % 2) == 1)
            {
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.ADCCali_OffsetCoreA, fpgaIndex, uintPosition);
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.ADCCali_OffsetCoreB, fpgaIndex, uintPosition);
            }
            else
            {
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.ADCCali_OffsetCoreC, fpgaIndex, uintPosition);
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.ADCCali_OffsetCoreD, fpgaIndex, uintPosition);
            }
        }

        /// <summary>
        /// FPGA增益细调
        /// </summary>
        /// <param name="fpgaIndex"></param>
        /// <param name="adcIndex"></param>
        /// <param name="gainErr_FPGAByTenThousand"></param>
        private void Adjust_FpgaConfigGain(AcqBdNo fpgaIndex, Int32 adcIndex, UInt32 gainErr_FPGAByTenThousand)
        {
            Double gain = (Double)((1 << 11) * gainErr_FPGAByTenThousand * 1.0 / 1000);
            if (!Hd.CurrDebugVarints.bEnable_CtrlGainByFpga)
            {
                gain = (1 << 11);
            }
            UInt32 gain_uint = (UInt32)gain;
            if (adcIndex == 0)
            {
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch1_L, fpgaIndex, gain_uint);
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch2_L, fpgaIndex, gain_uint);
            }
            else
            {
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch3_L, fpgaIndex, gain_uint);
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch4_L, fpgaIndex, gain_uint);
            }
        }

        private void AdjustAdc_Phase(Boolean withAdcCS, AcqBdNo fpgaIndex, Int32 adcIndex, Int32 phase)
        {
            Action<AcqBdNo, Int32, UInt32, UInt32> sender = withAdcCS ? Send5200CmdWithCS_OneFpga : SendCmdToAD5200_OneFpga;
            (UInt32 lowReg, UInt32 highReg) setregpair = (0x02B5U, 0x02B6U);
            //2=adcinfo.AdcPorts.Count()

            sender.Invoke(fpgaIndex, adcIndex % 2, setregpair.lowReg, (UInt32)(phase & 0xff));          //phase coarse 
            sender.Invoke(fpgaIndex, adcIndex % 2, setregpair.highReg, (UInt32)((phase >> 8) & 0x0ff)); //phase coarse 
        }

        private void AdjustAdc_PhaseEx(Boolean withAdcCS, AcqBdNo fpgaIndex, Int32 adcIndex, Int32 phasecase, Int32 phasefine)
        {
            Action<AcqBdNo, Int32, UInt32, UInt32> sender = withAdcCS ? Send5200CmdWithCS_OneFpga : SendCmdToAD5200_OneFpga;
            (UInt32 lowReg, UInt32 highReg) setregpair = (0x02B5U, 0x02B6U);

            sender.Invoke(fpgaIndex, adcIndex % 2, setregpair.lowReg, (UInt32)(phasefine & 0xffff));  //phase fine
            sender.Invoke(fpgaIndex, adcIndex % 2, setregpair.highReg, (UInt32)(phasecase & 0xffff)); //phase coarse 
        }

        private void AdjustAdc_Offset(AcqBdNo fpgaIndex, Int32 adcIndex, UInt32 offset)
        {
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex % 2, 0x0348, offset & 0xff);                            //offset core A ,low 8 bit
            SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex % 2, 0x0349, (offset >> 8) & 0xff);                     //offset core A ,hight 4 bit
        }

        private void AdjustAdc_Gain(Boolean widthAdcCS, AcqBdNo fpgaIndex, Int32 adcIndex, Int32 gain)
        {
            //TiAdc增益
            gain = Math.Min(0xFFFF, Math.Max(0x2000, gain));
            Int32 port = GetAdcInpiutPort(fpgaIndex, adcIndex % 2);

            //发送
            (UInt32 lowReg, UInt32 highReg) adcgainsetregpair = (port == 0) ? (0x30U, 0x31U) : (0x32U, 0x33U);
            Action<AcqBdNo, Int32, UInt32, UInt32> sender = widthAdcCS ? Send5200CmdWithCS_OneFpga : SendCmdToAD5200_OneFpga;
            //2= adcinfo.AdcPorts.Count()
            sender.Invoke(fpgaIndex, adcIndex % 2, (port == 0) ? 0x7AU : 0x7BU, 1);   //PORT A 打开增益可调,PORT B 打开增益可调
            sender.Invoke(fpgaIndex, adcIndex % 2, adcgainsetregpair.lowReg, (UInt32)(gain & 0xff));    //gain B  ,low 8 bit
            sender.Invoke(fpgaIndex, adcIndex % 2, adcgainsetregpair.highReg, (UInt32)((gain >> 8) & 0x0ff));//gain B  ,high 4 bit
        }

        ////返回端口号。端口号为0，表示PortA；端口号为1，表示PortB;
        private Int32 GetAdcInpiutPort(AcqBdNo fpgaIndex, Int32 adcIndex)
        {
            AcqModeAndInterleaveDefine define = Hd.CurrProduct.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;
            foreach (var dtl in define.Details)
            {
                foreach (var adcinfo in dtl.Value)
                {
                    if (adcinfo.AcqBdNo == fpgaIndex && ((adcinfo.Adc >> adcIndex) & 0x1) == 0x1)
                    {
                        return adcinfo.AdcPorts[adcIndex% adcinfo.AdcPorts.Count()];
                    }
                }
            }
            throw new ArgumentException($"fpgaIndex = {fpgaIndex.ToString()} ; adcIndex = {adcIndex}");
        }

        internal override Boolean SendCoefficientsByRegisterMode_Interpolation(CoefficientsTableType coefficientsTableType, Boolean bForce)
        {
            //预设 各个通道的插值系数是一样的
            Int32[]? coefficientsDataL1 = Misc.ReadCaliCoefDataFronmFile(".\\CaliData\\CoeFiles\\MSO8000_Interpolation_1st_max10.txt");

            Int32[]? coefficientsDataL2 = Misc.ReadCaliCoefDataFronmFile(".\\CaliData\\CoeFiles\\MSO8000_Interpolation_2nd_max10.txt");

            if ((coefficientsDataL1 == null) || (coefficientsDataL2 == null))
                return false;

            WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableWriteEnable, 0x00);
            WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableReset, 0x00);
            WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableReset, 0x01);
            WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableReset, 0x00);
            //WriteToAllFpga(AcqBdReg.W.Interpolate_Ratio, 0x0140A);//4Byte,最低Byte最多能发A(10倍率，只能2，5，A)，2位为0，34位最多20倍率（2，5，10，20）；

            List<Int32> coefficientsDataTable = new List<Int32>();
            coefficientsDataTable.AddRange(coefficientsDataL1);
            coefficientsDataTable.AddRange(coefficientsDataL2);
            Int32[]? coefficientsData = coefficientsDataTable.ToArray();

            for (int j = 0; j < coefficientsData.Length; j++)
            {
                UInt32 Coeaddr = 0;
                if (j < coefficientsDataL1.Length)
                    Coeaddr = (UInt32)j;
                else
                    Coeaddr = (UInt32)(j | 0x8000);

                UInt32 CoeData = (UInt32)coefficientsData[j];

                WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableWriteEnable, 0);
                WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableWriteAddr, (UInt32)Coeaddr);
                WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableWriteData_H, (UInt32)(CoeData >> 16) & 0x1);
                WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableWriteData_L16, (UInt32)CoeData & 0xFFFF);
                WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableWriteEnable, 1);
                WriteToAllFpga(AcqBdReg.W.Interpolate_FactorTableWriteEnable, 0);

                HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableWriteEnable, 0);
                HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableWriteAddr, (UInt32)Coeaddr);
                HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableWriteData_H, (UInt32)(CoeData >> 16) & 0x1);
                HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableWriteData_L16, (UInt32)CoeData & 0xFFFF);
                HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableWriteEnable, 1);
                HdIO.WriteReg(ProcBdReg.W.Interpolate_FactorTableWriteEnable, 0);

            }
            return true;
        }


        private List<Double> _IFCCoefficientsParams = new List<Double>();
        private List<Double> _IFCCoefficientsParams_pro = new List<Double>();

        /// <summary>
        /// 上一次系数下发的交织信息和担当为信息
        /// </summary>
        /// <param name="adcinterleave"></param>
        /// <param name="scale"></param>
        private record LastCofficientsInfo(AdcInterleaveMode adcInterLeave, List<Double> scale);
        private LastCofficientsInfo LastCofficients;

        /// <summary>
        /// 下发TiAdc的系数，按通道分，交织模式改变重新下发；
        /// </summary>
        /// <param name="bForce"></param>
        /// <returns></returns>
        internal override Boolean SendCoefficientsByRegisterMode_ADCTI(Boolean bForce)
        {
            ((ICaliData)CoefficientsParams.Default).LoadFromFile();
            Dictionary<AcqBdNo, List<Double>> coefficientsparamsbyifc = new Dictionary<AcqBdNo, List<Double>>();

            List<double> cofDirect=new List<double>();
            String coefficientsParamsPath = @".\CaliData\tb_coe_allpass.txt";
            if (File.Exists(coefficientsParamsPath))
            {
                using (StreamReader sr = new StreamReader(coefficientsParamsPath))

                {
                    while (!sr.EndOfStream)
                    {
                        cofDirect.Add(Convert.ToInt32("0x" + sr.ReadLine().Trim(), 16));
                    }
                }
            
            }
            //  if (CoefficientsParams.Default.ParamsTable.ContainsKey("TI_All-20G_C1_LOW_12bit_acq1"))
            // //     coefficientsparamsbyifc.TryAdd(AcqBdNo.B0, cofDirect);
            //      coefficientsparamsbyifc.TryAdd(AcqBdNo.B0, CoefficientsParams.Default.ParamsTable["TI_All-20G_C1_LOW_12bit_acq1"].ToList());
            //  if (CoefficientsParams.Default.ParamsTable.ContainsKey("TI_All-20G_C1_LOW_12bit_acq2"))
            ////      coefficientsparamsbyifc.TryAdd(AcqBdNo.B1, cofDirect);
            //      coefficientsparamsbyifc.TryAdd(AcqBdNo.B1, CoefficientsParams.Default.ParamsTable["TI_All-20G_C1_LOW_12bit_acq2"].ToList());


            if (CoefficientsParams.Default.ParamsTable.ContainsKey("TI_All-20G_C1_LOW_12bit_acq1"))
                coefficientsparamsbyifc.TryAdd(AcqBdNo.B0, CoefficientsParams.Default.ParamsTable["TI_All-20G_C1_LOW_12bit_acq1"].ToList());
            if (CoefficientsParams.Default.ParamsTable.ContainsKey("TI_All-20G_C1_LOW_12bit_acq2"))
                coefficientsparamsbyifc.TryAdd(AcqBdNo.B1, CoefficientsParams.Default.ParamsTable["TI_All-20G_C1_LOW_12bit_acq2"].ToList());
            if (CoefficientsParams.Default.ParamsTable.ContainsKey("TI_All-20G_C2_LOW_12bit_acq1"))
                coefficientsparamsbyifc.TryAdd(AcqBdNo.B2, CoefficientsParams.Default.ParamsTable["TI_All-20G_C2_LOW_12bit_acq1"].ToList());
            if (CoefficientsParams.Default.ParamsTable.ContainsKey("TI_All-20G_C2_LOW_12bit_acq2"))
                coefficientsparamsbyifc.TryAdd(AcqBdNo.B3, CoefficientsParams.Default.ParamsTable["TI_All-20G_C2_LOW_12bit_acq2"].ToList());
            if (CoefficientsParams.Default.ParamsTable.ContainsKey("TI_All-20G_C3_LOW_12bit_acq1"))
                coefficientsparamsbyifc.TryAdd(AcqBdNo.B4, CoefficientsParams.Default.ParamsTable["TI_All-20G_C3_LOW_12bit_acq1"].ToList());
            if (CoefficientsParams.Default.ParamsTable.ContainsKey("TI_All-20G_C3_LOW_12bit_acq2"))
                coefficientsparamsbyifc.TryAdd(AcqBdNo.B5, CoefficientsParams.Default.ParamsTable["TI_All-20G_C3_LOW_12bit_acq2"].ToList());
            if (CoefficientsParams.Default.ParamsTable.ContainsKey("TI_All-20G_C4_LOW_12bit_acq1"))
                coefficientsparamsbyifc.TryAdd(AcqBdNo.B6, CoefficientsParams.Default.ParamsTable["TI_All-20G_C4_LOW_12bit_acq1"].ToList());
            if (CoefficientsParams.Default.ParamsTable.ContainsKey("TI_All-20G_C4_LOW_12bit_acq2"))
                coefficientsparamsbyifc.TryAdd(AcqBdNo.B7, CoefficientsParams.Default.ParamsTable["TI_All-20G_C4_LOW_12bit_acq2"].ToList());
            RegSendManager.Default.Send((UInt32)AcqBdReg.W.TIADC_Enable);
            //SendCoefficients_IFC(coefficientsparamsbyifc);

            if (bForce)
            {
                SendCoefficients_IFC(coefficientsparamsbyifc);
            }
            return true;
        }

        internal override Boolean SendCoefficientsByRegisterMode_IFC(Boolean bForce)
        {
            List<Int32> allscales =new() {2,5,10,20,50,100,200,500,1000 };

            for (int currentscaleindex = 0; currentscaleindex < allscales.Count(); currentscaleindex++)
            {
                Dictionary<AdcInterleaveMode, List<ChannelId>> channeldic = new Dictionary<AdcInterleaveMode, List<ChannelId>>();
                var existprobe = Hd.ProbeManager.ProbeStatus();
                var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
                AdcInterleaveMode adcinterleavemode = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode;
                channeldic.Add(AdcInterleaveMode.Mode2To1, new List<ChannelId>()
                {
                     ChannelId.C1,
                     ChannelId.C2,
                     ChannelId.C3,
                     ChannelId.C4,
                });
                //channeldic.Add(AdcInterleaveMode.Mode1To1, new List<ChannelId>()
                //{
                //     ChannelId.C2,
                //     ChannelId.C1,
                //     ChannelId.C4,
                //     ChannelId.C3
                //});
                ((ICaliData)CoefficientsParams.Default).LoadFromFile();
                List<Int32> datas = new List<Int32>();

                Dictionary<ChannelId, List<Double>> coefficientsparamsbyifc_pro = new Dictionary<ChannelId, List<Double>>();
                Dictionary<ChannelId, List<Double>> coefficientsparamsbydelay = new Dictionary<ChannelId, List<Double>>();

                AcqModeAndInterleaveDefine nowdefine = analogacquiremodel.GetCurrentAcqModeInterleave()!;

               
                
                Boolean issendconfficients = true;
                if (_IFCCoefficientsParams == null || _IFCCoefficientsParams.Count == 0)
                {
                    _IFCCoefficientsParams = new List<Double>();
                    //String coefficientsParamsPath = @".\CaliData\tb_coe_allpass.txt";
                    String coefficientsParamsPath = @".\CaliData\tb_coe_allpass.txt";
                    if (File.Exists(coefficientsParamsPath))
                    {
                        using (StreamReader sr = new StreamReader(coefficientsParamsPath))
                        {
                            while (!sr.EndOfStream)
                            {
                                _IFCCoefficientsParams.Add(Convert.ToInt32("0x" + sr.ReadLine().Trim(), 16));
                            }
                        }
                    }
                }
                if (_IFCCoefficientsParams != null || _IFCCoefficientsParams.Count != 0)
                {
                    _IFCCoefficientsParams = new List<Double>();
                    //String coefficientsParamsPath = @".\CaliData\tb_coe_allpass.txt";
                    String coefficientsParamsPath = @".\CaliData\tb_coe_allpass.txt";
                    if (File.Exists(coefficientsParamsPath))
                    {
                        using (StreamReader sr = new StreamReader(coefficientsParamsPath))
                        {
                            while (!sr.EndOfStream)
                            {
                                _IFCCoefficientsParams.Add(Convert.ToInt32("0x" + sr.ReadLine().Trim(), 16));
                            }
                        }
                    }
                }
                if (Hd.CurrDebugVarints.bEnable_IFCDefalutCoefficientsParams)
                {
                    coefficientsparamsbyifc_pro.TryAdd(ChannelId.C1, _IFCCoefficientsParams);
                    coefficientsparamsbyifc_pro.TryAdd(ChannelId.C2, _IFCCoefficientsParams);
                    coefficientsparamsbyifc_pro.TryAdd(ChannelId.C3, _IFCCoefficientsParams);
                    coefficientsparamsbyifc_pro.TryAdd(ChannelId.C4, _IFCCoefficientsParams);
                }
                else
                {

                    foreach (var channel in channeldic)
                    {
                        UInt32 chnlactivestate = 0;
                        foreach (var channelid in channel.Value)
                        {
                            chnlactivestate |= 0x1u << (Int32)channelid;//独热码，1、3通道打开，则cAS = 0b0101;
                        }
                        AcqModeAndInterleaveDefine define = analogacquiremodel.GetAcqModeInterleaveByChnlState(chnlactivestate)!;
                        foreach (var channelid in channel.Value)
                        {
                            if (!define.Details.ContainsKey(channelid))
                            {
                                continue;
                            }
                            //AdcInterleaveMode tempadcinterleavemode = (chnlactivestate == 0b1111) ? AdcInterleaveMode.Mode1To1 : AdcInterleaveMode.Mode2To1;
                            AdcInterleaveMode tempadcinterleavemode = AdcInterleaveMode.Mode2To1;

                            var detail = define.Details[channelid];
                            if (null != detail)
                            {
                                String coupling = Hd.UIMessage!.Analog![(Int32)channelid].Coupling == AnaChnlCoupling.DC50 ? "LOW" : "HIGH";
                                //String scale = Hd.UIMessage!.Analog[(Int32)channelid].Scale.ToString();
                                // 默认使用高阻的幅度衰减表，如果是低阻，显式地进行更换
                                //String ifckey = $"IFC_{define.Name}_{channelid.ToString()}_{coupling}_{GetHighMagnification(Hd.UIMessage!.Analog[(Int32)channelid])}Mv";
                                //String afckey = $"AFC_{define.Name}_{channelid.ToString()}_{coupling}_{GetHighMagnification(Hd.UIMessage!.Analog[(Int32)channelid])}Mv";
                                String ifckey = $"IFC_{define.Name}_{channelid.ToString()}_{coupling}_{allscales[currentscaleindex].ToString()}Mv";
                                String afckey = $"AFC_{define.Name}_{channelid.ToString()}_{coupling}_{allscales[currentscaleindex].ToString()}Mv";
                                var adcInfo = detail[0];
                                List<Double> coefficientsparamsdata = new List<Double>();

                                #region 扫频系数
                                if (CoefficientsParams.Default.AllNames.Contains(ifckey))
                                {
                                    coefficientsparamsdata = CoefficientsParams.Default[ifckey].ToList();
                                }
                                else
                                {
                                    ifckey = $"IFC_{define.Name}_{channelid.ToString()}_{coupling}_{"10"}Mv";
                                    if (CoefficientsParams.Default.AllNames.Contains(ifckey))
                                    {
                                        coefficientsparamsdata = CoefficientsParams.Default[ifckey].ToList();
                                    }
                                    //if (null != _IFCCoefficientsParams && _IFCCoefficientsParams.Count == 12288)
                                    //{
                                    //    if (define.InterleaveMode == AdcInterleaveMode.Mode1To1)
                                    //    {
                                    //        if (channelid == ChannelId.C2 || channelid == ChannelId.C4)
                                    //        {
                                    //            coefficientsparamsdata.AddRange(_IFCCoefficientsParams.GetRange(8192, 2048));
                                    //        }
                                    //        else
                                    //        {
                                    //            coefficientsparamsdata.AddRange(_IFCCoefficientsParams.GetRange(10240, 2048));
                                    //        }

                                    //    }
                                    //    else
                                    //    {
                                    //        coefficientsparamsdata.AddRange(_IFCCoefficientsParams.Take(8192).ToList());
                                    //    }
                                    //}

                                }

                                if (existprobe != null && existprobe[channelid].FreqData.Count != 0 && CoefficientsParams.Default.AllNames.Contains(afckey)
                                    && nowdefine.InterleaveMode == tempadcinterleavemode)
                                {
                                    var tempcoefficientdata = CoefficientsParams.Default[afckey].ToArray();
                                    GetProbeCoefficients(ref tempcoefficientdata, channelid);
                                    if (HasNewIFC)
                                    {
                                        coefficientsparamsdata = tempcoefficientdata.ToList();
                                    }
                                }

                                if (coefficientsparamsbyifc_pro.TryGetValue(channelid, out List<Double> coefficientsifcparams))
                                {
                                    coefficientsifcparams.AddRange(coefficientsparamsdata);
                                }
                                else
                                {
                                    coefficientsparamsbyifc_pro.TryAdd(channelid, coefficientsparamsdata);
                                }
                                #endregion

                            }
                        }
                    }

                }

                

                #region 插值延时

                switch (nowdefine.InterleaveMode)
                {
                    case AdcInterleaveMode.Mode2To1:
                        foreach (var item in channeldic[AdcInterleaveMode.Mode2To1])
                        {
                            String coupling = Hd.UIMessage!.Analog![(Int32)item].Coupling == AnaChnlCoupling.DC50 ? "LOW" : "HIGH";
                            String interleavemode = nowdefine.InterleaveMode == AdcInterleaveMode.Mode2To1 ? "ALL-20G" : "All-10G";
                            String delaypfckey = $"DelayPFC_pro_{interleavemode}_{item.ToString()}_{coupling}";
                            String delayafckey = $"DelayAFC_pro_{interleavemode}_{item.ToString()}_{coupling}";
                            //AcqBdNo acqbdno = item == ChannelId.C1 ? AcqBdNo.B0 : item == ChannelId.C2 ? AcqBdNo.B1 : item == ChannelId.C3 ? AcqBdNo.B2 : AcqBdNo.B3;
                            Double delaypfc = 0;
                            Double delayafc = 0;
                            if (CoefficientsParams.Default.AllNames.Contains(delaypfckey))
                            {
                                delaypfc = CoefficientsParams.Default[delaypfckey].ToList()[0];
                            }
                            if (CoefficientsParams.Default.AllNames.Contains(delayafckey))
                            {
                                delayafc = HasNewIFC ? (NewAFCLength / 2) : (CoefficientsParams.Default[delayafckey].ToList()[0] / 2);
                            }
                            List<Double> coefficientsparamsdata = new List<Double>() { delayafc, delaypfc };
                            if (coefficientsparamsbydelay.TryGetValue(item, out List<Double> coefficientsdelay20gparams))
                            {
                                coefficientsdelay20gparams.Clear();
                                coefficientsdelay20gparams = coefficientsparamsdata;
                                coefficientsparamsbydelay[item] = coefficientsdelay20gparams;
                            }
                            else
                            {
                                coefficientsparamsbydelay.TryAdd(item, coefficientsparamsdata);
                            }
                        }
                        break;

                }

                #endregion


                RegSendManager.Default.Send((UInt32)ProcBdReg.W.TIADC_Enable);
                //CIJ——0523
                //          SendCoefficients_IFC_pro(coefficientsparamsbyifc_pro);

                if (bForce)
                {
                    //CIJ——0523
                    SendCoefficients_IFC_pro(coefficientsparamsbyifc_pro,currentscaleindex*4096);
                }
                SendCoefficients_Delay_pro(coefficientsparamsbydelay);
            }
           
            return true;
        }

        internal override Boolean SendCoefficientsByRegisterMode_IFC_Delay(Boolean bForce)
        {
            
                Dictionary<AdcInterleaveMode, List<ChannelId>> channeldic = new Dictionary<AdcInterleaveMode, List<ChannelId>>();
                var existprobe = Hd.ProbeManager.ProbeStatus();
                var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
                AdcInterleaveMode adcinterleavemode = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode;
                channeldic.Add(AdcInterleaveMode.Mode2To1, new List<ChannelId>()
                {
                     ChannelId.C1,
                     ChannelId.C2,
                     ChannelId.C3,
                     ChannelId.C4,
                });
                ((ICaliData)CoefficientsParams.Default).LoadFromFile();
                List<Int32> datas = new List<Int32>();

                Dictionary<ChannelId, List<Double>> coefficientsparamsbyifc_pro = new Dictionary<ChannelId, List<Double>>();
                Dictionary<ChannelId, List<Double>> coefficientsparamsbydelay = new Dictionary<ChannelId, List<Double>>();

                AcqModeAndInterleaveDefine nowdefine = analogacquiremodel.GetCurrentAcqModeInterleave()!;

         

                #region 插值延时

                switch (nowdefine.InterleaveMode)
                {
                    case AdcInterleaveMode.Mode2To1:
                        foreach (var item in channeldic[AdcInterleaveMode.Mode2To1])
                        {
                            String coupling = Hd.UIMessage!.Analog![(Int32)item].Coupling == AnaChnlCoupling.DC50 ? "LOW" : "HIGH";
                            String interleavemode = nowdefine.InterleaveMode == AdcInterleaveMode.Mode2To1 ? "ALL-20G" : "All-10G";
                            String delaypfckey = $"DelayPFC_pro_{interleavemode}_{item.ToString()}_{coupling}";
                            String delayafckey = $"DelayAFC_pro_{interleavemode}_{item.ToString()}_{coupling}";
                            //AcqBdNo acqbdno = item == ChannelId.C1 ? AcqBdNo.B0 : item == ChannelId.C2 ? AcqBdNo.B1 : item == ChannelId.C3 ? AcqBdNo.B2 : AcqBdNo.B3;
                            Double delaypfc = 0;
                            Double delayafc = 0;
                            if (CoefficientsParams.Default.AllNames.Contains(delaypfckey))
                            {
                                delaypfc = CoefficientsParams.Default[delaypfckey].ToList()[0];
                            }
                            if (CoefficientsParams.Default.AllNames.Contains(delayafckey))
                            {
                                delayafc = HasNewIFC ? (NewAFCLength / 2) : (CoefficientsParams.Default[delayafckey].ToList()[0] / 2);
                            }
                            List<Double> coefficientsparamsdata = new List<Double>() { delayafc, delaypfc };
                            if (coefficientsparamsbydelay.TryGetValue(item, out List<Double> coefficientsdelay20gparams))
                            {
                                coefficientsdelay20gparams.Clear();
                                coefficientsdelay20gparams = coefficientsparamsdata;
                                coefficientsparamsbydelay[item] = coefficientsdelay20gparams;
                            }
                            else
                            {
                                coefficientsparamsbydelay.TryAdd(item, coefficientsparamsdata);
                            }
                        }
                        break;

                }

                #endregion

                SendCoefficients_Delay_pro(coefficientsparamsbydelay);
            
            return true;
        }

        private static Boolean GetGoalResponseCoefficients(ref List<Double> goalFreq, ref List<Double> goalResponse)
        {
            var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave();
            if (analogacquiremodel == null)
            {
                return false;
            }
            Double linestartfreq = 1e8;
            Double linestopfreq = 8.1e9;
            Double step = 1e8;
            Double samplerate = 20e9;
            if (analogacquiremodel.InterleaveMode == AdcInterleaveMode.Mode2To1)
            {
                linestopfreq = Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G ? 8.1e9 : 5.1e9;
                samplerate = Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G ? 20e9 : 10e9;
            }
            else
            {
                linestopfreq = Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G ? 5.1e9 : 4.1e9;
                samplerate = Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G ? 20e9 : 10e9;
            }
            Int32 freqnum = (Int32)((linestopfreq - linestartfreq) / step) + 1;
            for (Int32 i = 0; i < freqnum; i++)
            {
                goalFreq.Add(i * step + linestartfreq);
                goalResponse.Add((goalFreq[i] - linestartfreq) / (linestartfreq - linestopfreq));
            }
            goalFreq.Add(samplerate / 2);
            goalResponse.Add(0);
            return true;
        }

        private void GetProbeCoefficients(ref Double[] coefficientsParamsData, ChannelId id)
        {
            HasNewAFC = false;
            if (coefficientsParamsData == null || coefficientsParamsData.Sum() == 0 || !Constants.AFC_FREQ_RESPONSE_CALI)
            {
                return;
            }
            var existsprobes = Hd.ProbeManager?.ProbeStatus();
            if (existsprobes == null)
            {
                return;
            }
            var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave();
            if (analogacquiremodel == null)
            {
                return;
            }

            Double[] probecoefreq = new Double[existsprobes[id].FreqData.Count()];
            Double[] probecoeresponse = new Double[existsprobes[id].FreqData.Count()];
            for (Int32 i = 0; i < existsprobes[id].FreqData.Count(); i++)
            {
                probecoefreq[i] = existsprobes[id].FreqData[i].Item1;
                probecoeresponse[i] = existsprobes[id].FreqData[i].Item2;
            }

            Boolean probebandwidth = existsprobes[id].SerailNumber.Contains("UT-PA2000");
            String samplerate = String.Empty;
            String bandwidth = String.Empty;
            String goalfilename = String.Empty;
            if (analogacquiremodel.InterleaveMode == AdcInterleaveMode.Mode2To1)
            {
                samplerate = "20e9";
                bandwidth = Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G ? "8e9" : "5e9";
                if (probebandwidth) //此处只有UT-PA2000，后续探头多了根据名称再修改
                {
                    bandwidth = "2.8e9";
                }
            }
            else
            {
                samplerate = "10e9";
                bandwidth = "4e9";
                if (probebandwidth)
                {
                    bandwidth = "2.8e9";
                }
            }

            List<Double> goalresponse = new List<Double>();
            List<Double> goalfreq = new List<Double>();
            if (!GetGoalResponseCoefficients(ref goalfreq, ref goalresponse))
            {
                return;
            };

            StringBuilder stringbuilder = new StringBuilder();
            stringbuilder.Append($"SamplingRate={samplerate},");
            stringbuilder.Append($"ConcernBandwidth={bandwidth},");

            Int32 filtercoelength = 200;
            NewAFCLength = filtercoelength;
            String outputdebugfile = "N";
            Int32 coequantibit = 12;
            Double steplen = 0.0001;
            Int32 traingtimes = 1000;
            Int32 fftlen = 2000;
            stringbuilder.Append($"FilterCoeLength={filtercoelength - 1},");
            stringbuilder.Append($"OutputDebugFile={outputdebugfile},");
            stringbuilder.Append($"CoeQuantiBit={coequantibit},");
            stringbuilder.Append($"StepLen={steplen},");
            stringbuilder.Append($"TraingTimes={traingtimes},");
            stringbuilder.Append($"FFTLen={fftlen},");
            stringbuilder.Append($"DebugFilePath={Path.Combine(Directory.GetCurrentDirectory(), @"CaliData\")!},");
            stringbuilder.Append($"DebugFilePrefix=MatlabGenerateCoefficientsTable_ProbeCompDesign,");

            String othertransparencyparameter = stringbuilder.ToString();
            Double[] afcresult = new Double[filtercoelength]; //由于内存管理的问题，在C#端，必须分配足够的内存。
            String version = new String(' ', 256);            //由于内存管理的问题，在C#端，必须分配足够的内存。

            Double quantizedcoefficients = Math.Pow(2, 12);
            //AFC+探头频响->新的AFC
            MatlabGenerateCoefficientsTableProbeCompDesign(coefficientsParamsData.ToArray(), coefficientsParamsData.Count(), probecoeresponse, probecoefreq, probecoefreq.Count(), goalresponse.ToArray(), goalfreq.ToArray(), goalfreq.Count(), othertransparencyparameter, afcresult, filtercoelength, ref version);
            coefficientsParamsData = afcresult;
            HasNewAFC = true;
            //新的AFC+PFC->新的IFC
            GetNewProbeCoefficients(ref coefficientsParamsData, id);
        }

        private void GetNewProbeCoefficients(ref Double[] result, ChannelId id)
        {
            HasNewIFC = false;
            if (!Constants.PFC_FREQ_RESPONSE_CALI)
            {
                return;
            }
            var analogacquiremodel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave();
            if (analogacquiremodel == null || result == null)
            {
                return;
            }

            String keypfc = "";
            var adcinterleavemode = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode;
            var analog = Hd.UIMessage!.Analog![(Int32)id];
            String coup = analog.Coupling == AnaChnlCoupling.DC50 ? "_LOW" : "_HIGH";
            if (adcinterleavemode == AdcInterleaveMode.Mode2To1)
            {
                keypfc = "PFC_" + "C1C3-20G_" + id.ToString() + coup;
            }
            else
            {
                keypfc = "PFC_" + "All-10G_" + id.ToString() + coup;
            }
            String keyti = keypfc.Replace("PFC", "TI");
            if (!CoefficientsParams.Default.AllNames.Contains(keypfc) && !CoefficientsParams.Default.AllNames.Contains(keyti))
            {
                return;
            }
            String quantibitfpga = "QuantiBitFPGA=12";
            String mergingmode = "MergingMode=TI_AFC_PFC";//8000:TI_AFC, TI_AFC_PFC, TI_PFC
            String filtertypevalue = "F";
            String filtertype = $"FilterType={filtertypevalue}";
            // 8192 = 2048 * tichannelnumbervalue * 2; 2为虚实数
            Int32 filtercoelengthvalue = analogacquiremodel.InterleaveMode == AdcInterleaveMode.Mode2To1 ? (8192 / 4) : (2048 / 4);
            String filtercoelength = $"FilterCoeLength={filtercoelengthvalue}";
            Int32 tichannelnumbervalue = 2;
            String tichannelnumber = $"TIChannelNumber={tichannelnumbervalue}";
            String ticoetype = "TICoeType=F";
            String afccoetype = "AFCCoeType=T";
            String pfccoetype = "PFCCoeType=T";
            String parallelfreqfilter = "ParallelFreqFilter=Y";

            String outputdebugfile = "OutputDebugFile=N";
            String debugfilepath = $"DebugFilePath={Path.Combine(Directory.GetCurrentDirectory(), @"CaliData\")!}";
            String debugfileprefix = "DebugFilePrefix=MatlabGenerateCoefficientsTable_CoeMerging";
            String othertransparencyparameter = $"{quantibitfpga},{mergingmode},{filtertype},{filtercoelength},{tichannelnumber},{ticoetype},{afccoetype},{pfccoetype},{parallelfreqfilter},{outputdebugfile},{debugfilepath},{debugfileprefix}";

            Double tempcoe = Math.Pow(2, 12);
            List<Double> tiadccoe = CoefficientsParams.Default[keyti].Select(o => o / (double)tempcoe).ToList();
            Double[] pfccoefficientsdata = CoefficientsParams.Default[keypfc].ToArray();
            //返回参数所需的最大内存，不一定等于filtercoelength。真实数据长度以filtercoelength为准
            Int32 maxmemeorylength = filtertypevalue == "T" ? filtercoelengthvalue * tichannelnumbervalue : filtercoelengthvalue * tichannelnumbervalue * 2; ;
            Double[] ifcresult = new Double[maxmemeorylength];//由于内存管理的问题，在C#端，必须分配足够的内存。
            String version = new String(' ', 256);            //由于内存管理的问题，在C#端，必须分配足够的内存。

            MatlabGenerateCoefficientsTableCoeMerging(tiadccoe.ToArray(), tiadccoe.Count, result.ToArray(), result.Count(), pfccoefficientsdata, pfccoefficientsdata.Count(), othertransparencyparameter, ifcresult, maxmemeorylength, ref version);

            result = ifcresult.Select(o => Math.Round(o * tempcoe)).ToArray();
            HasNewIFC = true;
        }

        #region Probe
        private Boolean HasNewAFC { get; set; } = false;
        private Boolean HasNewIFC { get; set; } = false;
        private Int32 NewAFCLength { get; set; } = 128;
        [DllImportAttribute("MatlabGenerateCoefficientsTable_ProbeCompDesign_CPlusWrapper.DLL", EntryPoint = "MatlabGenerateCoefficientsTable_ProbeCompDesign_CPlusWrapper", CharSet = CharSet.Ansi, SetLastError = true)]
        static extern Int32 MatlabGenerateCoefficientsTableProbeCompDesign(double[] channelAFCCoe, //通道补偿滤波器系数
               Int32 channelAFCCoeLength,            //AFC滤波器系数长度
               Double[] probeCoeResponse,            //探头幅频频响数据
               Double[] probeCoeFreq,                //探头幅频频响对应的频率
               Int32 probeCoeLength,                 //探头幅频频响长度
               Double[] goalCoeResponse,             //目标幅频频响数据
               Double[] goalCoeFreq,                 //目标幅频频响对应的频率
               Int32 goalCoeLength,                  //目标幅频频响长度
               String otherTransparencyParameter,    //算法相关的参数
               [In, Out] Double[] result,            //输出系数数据
               Int32 resultCount,                    //输出系数长度
               ref String version);                  //输出的dll版本信息

        [DllImportAttribute("MatlabGenerateCoefficientsTable_CoeMerging_CPlusWrapper.DLL", EntryPoint = "MatlabGenerateCoefficientsTable_CoeMerging_CPlusWrapper", CharSet = CharSet.Ansi, SetLastError = true)]
        static extern Int32 MatlabGenerateCoefficientsTableCoeMerging(Double[] tiADCCoeResponse,//tiAdc数据
            Int32 tiADCLength,                  //tiAdc数据长度
            Double[] channelAFCCoe,             //通道AFC数据
            Int32 channelAFCCoeLength,          //通道AFC数据长度
            Double[] channelPFCCoe,             //通道AFC数据
            Int32 channelPFCCoeLength,          //通道AFC数据长度
            String otherTransparencyParameter,  //其他参数集合
            [In, Out] Double[] result,          //新IFC数据
            Int32 maxMemeoryLength,             //分配给新IFC数据内存大小
            ref String version);                //dll版本信息
        #endregion

        /// <summary>
        /// 获取大倍率衰减网络的档位
        /// </summary>
        /// <param name="analogOption">模拟通道参数</param>
        /// <returns></returns>
        private static UInt32 GetHighMagnification(AnalogOptions analogOption)
        {
            Dictionary<AnaChnlScaleIndex, UInt32> atttable = (analogOption.Coupling == AnaChnlCoupling.DC50) ? CtrlAnalogChannel_JiHe2d5G.newAttTableLz : CtrlAnalogChannel_JiHe2d5G.newAttTableHz;
            atttable = Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G ? CtrlAnalogChannel_JiHe2d5G.yScalCtrlLz_4094 : atttable;
            return CtrlAnalogChannel_JiHe2d5G.GetDACScaleMv((AnaChnlScaleIndex)analogOption.ScaleIndex, atttable);
        }

        /// <summary>
        /// 发送IFC系数
        /// </summary>
        /// <param name="coefficientsParamsByIFC"></param>
        private void SendCoefficients_IFC(Dictionary<AcqBdNo, List<Double>> coefficientsParamsByIFC)
        {

            //寄存器改变：
            //TIADC_Enable => reverse_acq_reverse_wr_reg_0;
            //TIADC_WriteEnable => reverse_acq_reverse_wr_reg_4;
            //TIADC_WriteAddress => reverse_acq_reverse_wr_reg_3;
            //TIADC_FactorTableWriteData_r => reverse_acq_reverse_wr_reg_5;
            //TIADC_FactorTableWriteData_i => reverse_acq_reverse_wr_reg_2;
            if (!Hd.CurrDebugVarints.bEnable_Dsp)
            {
                return;
            }
            foreach (var item in coefficientsParamsByIFC)
            {
                //下发系数之前，强行关闭Dsp；
                WriteReg(AcqBdReg.W.TIADC_Enable, item.Key, 0);
                if (item.Value == null || item.Value.Count < 4096)
                {
                    continue;
                }
                for (Int32 i = 0; i < item.Value.Count / 2; i++)
                {
                    WriteReg(AcqBdReg.W.TIADC_WriteEnable, item.Key, 0);
                    //地址
                    WriteReg(AcqBdReg.W.TIADC_WriteAddress, item.Key, (UInt32)i);

                    WriteReg(AcqBdReg.W.TIADC_FactorTableWriteData_r, item.Key, (UInt32)item.Value[i * 2] & 0xffff);
                    WriteReg(AcqBdReg.W.TIADC_FactorTableWriteData_i, item.Key, (UInt32)item.Value[i * 2 + 1] & 0xffff);

                    HdIO.DelayByUs(10);
                    WriteReg(AcqBdReg.W.TIADC_WriteEnable, item.Key, 1);
                }

                WriteReg(AcqBdReg.W.TIADC_WriteEnable, item.Key, 0);
                RegSendManager.Default.Send((UInt32)AcqBdReg.W.TIADC_Enable);
            }
        }

        private void SendCoefficients_IFC_pro(Dictionary<ChannelId, List<Double>> coefficientsParamsByIFC,int ramStartIndex)
        {
            //寄存器改变：
            if (!Hd.CurrDebugVarints.bEnable_Dsp_Pro) //need_add
            {
                return;
            }
            foreach (var item in coefficientsParamsByIFC)
            {
                //下发系数之前，强行关闭Dsp；
                HdIO.WriteReg(ProcBdReg.W.Dsp_CaliEnPro, 0x00); // pro_dsp_pro
                if (item.Value == null || item.Value.Count < 4096)
                {
                    continue; 
                }
                for (Int32 i = 0; i < item.Value.Count / 2; i++)
                {
                    //HdIO.WriteReg(ProcBdReg.W.TIADC_TIADC_cail_select, (UInt32)0x01<<(Int32)item.Key); // channle_select
                    HdIO.WriteReg(ProcBdReg.W.Dsp_WrCoeAddrHPro, (UInt32)(0x01 << (Int32)item.Key)); // TIADC_WriteAddress

                    HdIO.WriteReg(ProcBdReg.W.Dsp_WrCoeEnPro, 0x00); // pro_DSP
                    //地址
                    
                    HdIO.WriteReg(ProcBdReg.W.Dsp_WrCoeAddrLPro, (UInt32)(i+ ramStartIndex)); // TIADC_WriteAddress
                    HdIO.WriteReg(ProcBdReg.W.Dsp_WrCoeDataRPro, (UInt32)item.Value[i * 2] & 0xffff); // pro_DSP
                    HdIO.WriteReg(ProcBdReg.W.Dsp_WrCoeDataIPro, (UInt32)item.Value[i * 2 + 1] & 0xffff); // pro_DSP

                    HdIO.DelayByUs(10);
                    HdIO.WriteReg(ProcBdReg.W.Dsp_WrCoeEnPro, 1); // pro_DSP
                }
                HdIO.WriteReg(ProcBdReg.W.Dsp_WrCoeEnPro, 0); // pro_DSP

                RegSendManager.Default.Send((UInt32)ProcBdReg.W.Dsp_CaliEnPro);
            }
        }

        /// <summary>
        /// 发送丢点数
        /// </summary>
        /// <param name="coefficientsParamsByDelay"></param>
        private void SendCoefficients_Delay(Dictionary<AcqBdNo, List<Double>> coefficientsParamsByDelay)
        {
            #region DSP触发配置下发
            //Int32 offset_ddr = -430;//预留20个点的找点范围，ZM
            //Int32 offset_ddr = (0)*16 ;//cjp
            Int32 offset_ddr = -73;//预留20个点的找点范围，ZM
       
            var define = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;

            List<AcqBdNo> acqbdnos = new List<AcqBdNo>()
            { AcqBdNo.B0,AcqBdNo.B1,AcqBdNo.B2,AcqBdNo.B3};

            UInt32 search_range = 40;
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Trig_Interp_Segment_Offset, (UInt32)offset_ddr);
            if (Hd.CurrDebugVarints.bEnable_Dsp && !ConditionManager.IsExtractEn)
            //if (Hd.CurrDebugVarints.bEnable_Dsp && !ConditionManager.IsExtractEn)
            {
                foreach (var item in coefficientsParamsByDelay)
                {

                    Dictionary<AcqBdNo, Int32> offset_ddr_dsp = new(){
                    {AcqBdNo.B0, -0},
                    {AcqBdNo.B1, -512-22},
                    {AcqBdNo.B2, -512-72},
                    {AcqBdNo.B3, -512-72},
                    };
                    //Int32 offset_ddr_dsp = -512-112;// 2048/4 + 20，补偿1/4个FFT帧长度的点，以及预留20个点的找点范围，ZM

                    //        Int32 offset_ddr_dsp = -512-68;// 2048/4 + 20，补偿1/4个FFT帧长度的点，以及预留20个点的找点范围，ZM

                    if (define.InterleaveMode == AdcInterleaveMode.Mode1To1)
                    {
                        offset_ddr_dsp[item.Key] = -276;
                    }
                    foreach (var delay in item.Value)
                    {
                        offset_ddr_dsp[item.Key] += (Int32)Math.Abs(delay);
                    }
                    WriteReg(AcqBdReg.W.Trig_DDR_Segment_Offset, item.Key, (UInt32)offset_ddr_dsp[item.Key]);
                    WriteReg(AcqBdReg.W.Trig_Extra_DDR_Rd_Offset, item.Key, (UInt32)offset_ddr_dsp[item.Key]);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Trig_DspSearchRangeNum, search_range);//找点范围-zhaomeng
                }
            }
            else
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Trig_DDR_Segment_Offset, 0x00);
                Boolean isinterpolate = define.InterleaveMode == AdcInterleaveMode.Mode1To1 ? Hd.UIMessage!.Timebase!.TmbScale < 10e-3 : Hd.UIMessage!.Timebase!.TmbScale < 5e-3;
                if (isinterpolate)//判断是不是插值档
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Trig_Extra_DDR_Rd_Offset, (UInt32)offset_ddr);
                }
                else
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Trig_Extra_DDR_Rd_Offset, 0);
                }
            }
            #endregion DSP触发
        }
        private UInt32 _RuntimeDelay = 0;
        private Int32 _PreTrigDeepth = 0;
        private UInt32 _Pre_search_range =0;//经过dsp后，在插值之前的触发找点范围
        private UInt32 _Pre_search_range_interp = 0;//经过插值后的找点范围
        private void SendCoefficients_Delay_pro(Dictionary<ChannelId, List<Double>> coefficientsParamsByDelay)
        {
            #region DSP触发配置下发
            //Int32 offset_ddr = -430;//预留20个点的找点范围，ZM
            //Int32 offset_ddr = (0)*16 ;//cjp
            Int32 offset_ddr = 2;//预留20个点的找点范围，ZM

            var define = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;

            //List<AcqBdNo> acqbdnos = new List<AcqBdNo>()
            //{ AcqBdNo.B0,AcqBdNo.B1,AcqBdNo.B2,AcqBdNo.B3};

            UInt32 search_range =70;//经过dsp后，在插值之前的触发找点范围
            UInt32 search_range_interp =1000;//经过插值后的找点范围
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Trig_Interp_Segment_Offset, (UInt32)offset_ddr);//2U 关闭dsp时读取片段给插值找点
            if (Hd.CurrDebugVarints.bEnable_Dsp && !ConditionManager.IsExtractEn)
            //if (Hd.CurrDebugVarints.bEnable_Dsp_pro && !ConditionManager.IsExtractEn)
            {
                Int32 discard_dsp_acq = 512;
                Int32 discard_dsp_pro = 256;//2U 在处理版拼合，故采集板只需要多读一半的丢点值
                Int32 discard_dsp_others = 2;//inerp的滤波器丢点和sync的整数丢点+预留   60
                discard_dsp_others = Hd.UIMessage?.Timebase?.TmbScale < 0.005 ? 39 : 2 ;
                Int32 discard_total = -discard_dsp_acq - discard_dsp_pro - discard_dsp_others;
                foreach (var item in coefficientsParamsByDelay)
                {

                    Dictionary<ChannelId, Int32> offset_ddr_dsp = new(){
                    {ChannelId.C1,discard_total},//dsp-acq   dsp-pro-1/2  3:inerp的滤波器丢点和sync的整数丢点
                    {ChannelId.C2,discard_total},
                    {ChannelId.C3,discard_total},
                    {ChannelId.C4,discard_total},
                    };
                    //Int32 offset_ddr_dsp = -512-112;// 2048/4 + 20，补偿1/4个FFT帧长度的点，以及预留20个点的找点范围，ZM

                    //        Int32 offset_ddr_dsp = -512-68;// 2048/4 + 20，补偿1/4个FFT帧长度的点，以及预留20个点的找点范围，ZM

                    if (define.InterleaveMode == AdcInterleaveMode.Mode1To1)
                    {
                        offset_ddr_dsp[item.Key] = -276;

                    }
                    foreach (var delay in item.Value)
                    {
                        offset_ddr_dsp[item.Key] += (Int32)Math.Abs(delay);
                    }
                    UInt32 SynIntdelayvalue = 0;
                    var parms= Hd.CurrProduct?.Acquirer_AnalogChannel?.SyncParams();
                    if (!Hd.UIMessage.bAcquireStopped)
                    {
                        if (((parms.Count() <= ChannelIdExt.AnaChnlNum)) && ((Int32)Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource() < ChannelIdExt.AnaChnlNum))
                        {
                            Int32 trigSource = (Int32)Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
                            if (parms[trigSource] != null)
                            {
                                SynIntdelayvalue = parms[(Int32)Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource()].DotsCnt;
                                _RuntimeDelay = SynIntdelayvalue;

                                //UInt32 value = (UInt32)(Hd.UIMessage!.Analog![trigSource].Bandwidth == 0 ? 0 : 1);
                                //SynIntdelayvalue += value;
                            }
                        }
                        else
                        {
                            SynIntdelayvalue = _RuntimeDelay;
                            //_RuntimeDelay = SynIntdelayvalue;
                        }
                    }
                    else
                    {
                        SynIntdelayvalue = _RuntimeDelay;
                    }

                    

                    var source = Hd.UIMessage?.Trigger?.Edge?.Source ?? ChannelId.C1;
                    int extvalue = (source == ChannelId.Ext)? ((6700-60)/2 ) : 0;//6700

                    if (!Hd.UIMessage.bAcquireStopped)
                    {
                        extvalue = (source == ChannelId.Ext) ? ((6700 - 60) / 2) : 0;
                        _PreTrigDeepth = extvalue;
                    }
                    else
                    {
                        extvalue = _PreTrigDeepth;
                    }

                    //extvalue =  0;
                 
                    if (!Hd.UIMessage.bAcquireStopped)
                    {
                        if (source == ChannelId.Ext)
                        {
                            search_range = 0;
                            search_range_interp = 0;
                        }
                        else
                        {
                            search_range = 70;
                            search_range_interp = 1000;
                        }
                        _Pre_search_range_interp = search_range_interp;
                        _Pre_search_range = search_range;
                    }
                    else
                    {
                        search_range_interp = _Pre_search_range_interp;
                        search_range = _Pre_search_range;
                    }



                        SynIntdelayvalue = Hd.CurrDebugVarints.bEnable_ChannelSync_Farrow == true ? SynIntdelayvalue : 0;
                    UInt32 SynIntdelaySendValue = (UInt32)Math.Ceiling(SynIntdelayvalue / 2d);
                    //UInt32 SynIntdelaySendValue = SynIntdelayvalue / 2;
                    Int32 SynFarrowDelayValue = Hd.CurrDebugVarints.bEnable_ChannelSync_Farrow == true ? 8 : 0;//分数延迟滤波器额外的丢点值，在找点时没有过该滤波器。所以读数时需要补偿
                    WriteReg(AcqBdReg.W.Trig_DDR_Segment_Offset, GetAcqBdNo(((Int32)item.Key) * 2), (UInt32)(offset_ddr_dsp[item.Key]- SynIntdelaySendValue));//处理版同步，采集板1/2
                    WriteReg(AcqBdReg.W.Trig_Extra_DDR_Rd_Offset, GetAcqBdNo(((Int32)item.Key) * 2), (UInt32)(offset_ddr_dsp[item.Key] + 32 - SynFarrowDelayValue + extvalue - SynIntdelaySendValue));
                    WriteReg(AcqBdReg.W.Trig_DDR_Segment_Offset, GetAcqBdNo(((Int32)item.Key) * 2 + 1), (UInt32)(offset_ddr_dsp[item.Key] - SynIntdelaySendValue));
                    WriteReg(AcqBdReg.W.Trig_Extra_DDR_Rd_Offset, GetAcqBdNo(((Int32)item.Key) * 2 + 1), (UInt32)(offset_ddr_dsp[item.Key] + 32 - SynFarrowDelayValue + extvalue - SynIntdelaySendValue));
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Trig_DspSearchRangeNum, search_range);//2U未用采集板找点范围-zhaomeng
                    HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve15, search_range);//2U处理版dsp找点范围
  
                    HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve24, 0);//addr接收复位，保持0
                    HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve26, search_range_interp);//插值找点范围


                }
            }
            else
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Trig_DDR_Segment_Offset, 0x00);
                Boolean isinterpolate = define.InterleaveMode == AdcInterleaveMode.Mode1To1 ? Hd.UIMessage!.Timebase!.TmbScale < 10e-3 : Hd.UIMessage!.Timebase!.TmbScale < 5e-3;
                if (isinterpolate)//判断是不是插值档
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Trig_Extra_DDR_Rd_Offset, (UInt32)offset_ddr);
                }
                else
                {
                    Int32 offset_decimation = 0;
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Trig_Extra_DDR_Rd_Offset, (UInt32)offset_decimation);
                }
            }
            #endregion DSP触发
        }

        private int[,] _AdcSourcePortLastSend = new int[2, 2];
        private void SendChMode_SamplingMode()
        {
            if (Hd.UIMessage == null)
                return;

            //AdcInterleaveMode adcInterleaveMode = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode;
            //Int32 activedChannels = Hd.CurrProduct.Acquirer_AnalogChannel.AcquingParameters.CurrChBWModeAndActiveState & 0xff;
            AdcInterleaveMode adcInterleaveMode = AdcInterleaveMode.Mode2To1;      //本产品只有20G采样率，这里直接固定写死  (cyw,20250331)
            Int32 activedChannels =0x0F; //   激活 C1 -C4(0b1111),   cyw
            UInt32 SamplingMode = (Hd.UIMessage!.Timebase!.IsScan && !Hd.UIMessage.bAcquireStopped, 1) switch
            {
                (true, _) => 0b10,
                (false, 1) => 0b00,
                (_, _) => 0b01,
            };
            SamplingMode |= adcInterleaveMode switch
            {
                AdcInterleaveMode.Mode1To1 => 0b01u,
                AdcInterleaveMode.Mode2To1 => 0b00u,
                _ => 0b00u,
            } << 2;
            SamplingMode |= (UInt32)(activedChannels << 4);

            //双板同步
            HdIO.WriteReg(ProcBdReg.W.ScanCtrl_SamplingMode, SamplingMode);
            WriteToAllFpga(AcqBdReg.W.ChMode_SamplingMode, SamplingMode);

            //配置Adc的Input
            HdDebugLogger.Log($"[{DateTime.Now}]: Adc Input Mux Start!");
            AcqModeAndInterleaveDefine define = Hd.CurrProduct.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;
            foreach (var dtl in define.Details)
            {
                foreach (var adcInfo in dtl.Value)
                {
                    List<int> adcIds = new List<int>();
                    for (int i = 0; i < sizeof(UInt32); i++)
                    {
                        if (((adcInfo.Adc >> i) & 0x1) == 0x1)
                            adcIds.Add(i);
                    }
                    foreach (var adcId in adcIds)
                    {
                        Int32 adcId2 = adcId % 2;
                        uint portRegValue = (uint)(adcInfo.AdcPorts[adcId2] == 0 ? 2 : 2);
                        Send5200CmdWithCS_OneFpga(adcInfo.AcqBdNo, adcId2, 0x0060, portRegValue);
                        Adc5200AutoCaliManager.Default.TryAddWaitForCali((int)adcInfo.AcqBdNo, adcId2, adcInfo.AdcPorts[adcId2]);
                    }
                }
            }
            HdDebugLogger.Log($"[{DateTime.Now}]: Adc Input Mux End!");


            //Adc是否需要自校准
            if (Adc5200AutoCaliManager.Default.IsNeedExcute)
            {
                Adc5200AutoCaliManager.Default.ExecAutoCali(true, 0x05);
            }
        }

        /// <summary>
        /// 通过板序号获取对应的板编号
        /// </summary>
        /// <param name="boardIndex"></param>
        /// <returns></returns>
        public static AcqBdNo GetAcqBdNo(int boardIndex)
        {
            return boardIndex switch
            {
                0 => AcqBdNo.B0,
                1 => AcqBdNo.B1,
                2 => AcqBdNo.B2,
                3 => AcqBdNo.B3,
                4 => AcqBdNo.B4,
                5 => AcqBdNo.B5,
                6 => AcqBdNo.B6,
                7 => AcqBdNo.B7,
                _ => throw new NotImplementedException(),
            };
        }

        public override void ExecMiscFunc(string param)
        {
            switch (param)
            {
                case nameof(Boadr_Acq_JiHe_MSO8000X.SendChMode_SamplingMode):
                    SendChMode_SamplingMode();
                    break;
            }
        }

        #region 检查锁相环状态
        private CancellationTokenSource _Cts;
        private Boolean _CheckPLLEnable = false;
        public Boolean CheckPLLEnable
        {
            get => _CheckPLLEnable;
            set
            {
                if (value != _CheckPLLEnable)
                {
                    _CheckPLLEnable = value;
                    if (_CheckPLLEnable)
                    {
                        _Cts = new CancellationTokenSource();

                        //开启检查锁相环状态任务
                        Task.Run(() =>
                        {
                            while (!_Cts.Token.IsCancellationRequested)
                            {
                                CheckPLLOnce();
                                Thread.Sleep(1000);
                            }
                        }, _Cts.Token);
                    }
                    else
                    {
                        //取消检查锁相环状态任务
                        _Cts?.Cancel();
                    }
                }
            }
        }

        public void CheckPLLOnce()
        {
   //         ReadADC5200RegStatus();
            //ReadLMX2595RegStatus();
  //          ReadHMC7044RegStatus();
  //          ReadProHMC7044RegStatus();
        }
        #endregion
    }
}

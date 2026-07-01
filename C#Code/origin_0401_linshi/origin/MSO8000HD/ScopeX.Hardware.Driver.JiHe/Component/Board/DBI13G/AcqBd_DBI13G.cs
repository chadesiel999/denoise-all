#if DBI
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Channels;
using System.Xml.Serialization;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using static ScopeX.ComModel.HdMessage;
using static ScopeX.Hardware.Calibration.Data.Base.ProductDataTranslate_MSO8000X;

namespace ScopeX.Hardware.Driver
{
    internal class AcqBd_DBI13G : AbstractAcqBd
    {
        public AcqBd_DBI13G((String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC)[] FpgaExistsConfig) : base(FpgaExistsConfig) { }

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
            //HdIO.WriteReg(ProcBdReg.W.FifoCtrl_AcqWriteEnable, 0);
            //WriteToAllFpga(AcqBdReg.W.Adc_MonitorAdcValid, 0x0);
            //InitAll5200At20GMode();

            ////Must add before any board ScanFlash -- lchy
            ////WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_p2a_sync_wr_en, 0x4);

            ////Acq
            //WriteToAllFpga(AcqBdReg.W.BoardSyncDelayCtrl_VTCAcq, 0x0);
            //WriteToAllFpga(AcqBdReg.W.BoardSyncDelayCtrl_VTCAcq, 0x1);
            //WriteToAllFpga(AcqBdReg.W.BoardSyncDelayCtrl_RstAcq, 0x1);
            //WriteToAllFpga(AcqBdReg.W.BoardSyncDelayCtrl_RstAcq, 0x0);

            //Boolean checkReadIdelayCtrlRdyAcq = HdIO.CheckRegisterValue(AcqBdReg.R.BoardSyncDelayCtrl_RdyReadAcq, 0x1, 1, 200);
            //if (!checkReadIdelayCtrlRdyAcq)
            //{
            //    PublicFunc.WriteLog("[Init]AcqBdReg.R.BoardSyncDelayCtrl_RdyReadAcq is not 1, need check!!!");
            //}

            //WriteToAllFpga(AcqBdReg.W.BoardSyncDelayCtrl_VTCAcq, 0x0);

            //HdIO.WriteReg(ProcBdReg.W.BoardSyncDelayCtrl_VTCPro, 0);
            //HdIO.WriteReg(ProcBdReg.W.BoardSyncDelayCtrl_VTCPro, 1);

            //HdIO.WriteReg(ProcBdReg.W.BoardSyncDelayCtrl_RstPro, 1);
            //HdIO.WriteReg(ProcBdReg.W.BoardSyncDelayCtrl_RstPro, 0);

            //checkReadIdelayCtrlRdyAcq = HdIO.CheckRegisterValue((UInt32)ProcBdReg.R.BoardSyncDelayCtrl_RdyReadPro, 0x1, 1, 200);
            //if (!checkReadIdelayCtrlRdyAcq)
            //{
            //    PublicFunc.WriteLog("[Init]ProcBdReg.R.BoardSyncDelayCtrl_RdyReadPro is not 1, need check!!!");
            //}

            //HdIO.WriteReg(ProcBdReg.W.BoardSyncDelayCtrl_VTCPro, 0);

            ////lchy - add
            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Data_pro_iserdes_idelay_reset, 0x1);//只需要开机初始化一次.必须需要,不能在采集中途发送
            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Data_pro_iserdes_idelay_reset, 0x0);//只需要开机初始化一次.必须需要,不能在采集中途发送
            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Data_pro_iserdes_io_reset, 0x1);
            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Data_pro_iserdes_io_reset, 0x0);
            //HdIO.Sleep(1);
            ////lchy - end

            //WriteToAllFpga(AcqBdReg.W.ChMode_SamplingMode, Hd.CurrProduct?.HardwareConfig?.Default_AcqBoardCH_MODE_SamplingMode ?? 0x0180);

            //TiAdc_ApplyAdc_Phase_Offset_Gain();

            //WriteToAllFpga(AcqBdReg.W.Fir_Enable, 0);

            //WriteToAllFpga(AcqBdReg.W.TIADC_Enable, 0);
            //HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh1, 0);
            //HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh2, 0);
            //HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh3, 0);
            //HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh4, 0);

            ////Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_AcqProgFullThresh, 15000); //数据长度
            //HdIO.WriteReg(ProcBdReg.W.DBI_AcqProgFullThreshProCh1, 15000);
            //HdIO.WriteReg(ProcBdReg.W.DBI_AcqProgFullThreshProCh2, 15000);
            //HdIO.WriteReg(ProcBdReg.W.DBI_AcqProgFullThreshProCh3, 15000);
            //HdIO.WriteReg(ProcBdReg.W.DBI_AcqProgFullThreshProCh4, 15000);

            ////DoScanProcboard2AcqBoardTrigWindow();// 218的扫窗

            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Data_pro_iserdes_io_reset, 0x0);
            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Data_pro_iserdes_io_reset, 0x1);//只需要开机初始化一次.必须需要,不能在采集中途发送
            //HdIO.Sleep(10);
            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Data_pro_iserdes_io_reset, 0x0);

            ////数据扫窗
            //AutoConfigSerdesSync();
            //AutoConfigPCIE_IDDRSync(200u);
            //ReadFpgaVersion();

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.FifoCtrl_BFIFOFullProgDepth, 150);


            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Mode, 0x0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_AutoTimeByms, 50);

            //ScanProc2AcqCtrl();
            //ScanAcq2ProcCtrl();

            ////AutoScanPro2AcqBoardSync();//218
            ////ScanProcboard2AcqBoardTrigWindow_Our(); // 13G的扫窗
            ////ScanAcqBoard2ProcboardTrigWindow_Our();
            //WriteToAllFpga(AcqBdReg.W.BoardSyncDelayCtrl_VTCAcq, 0x1);
            //HdIO.WriteReg(ProcBdReg.W.BoardSyncDelayCtrl_VTCPro, 1);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SignDataDelayAdjust, 28+5);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DataPath_ChannelOffsetNumAcq, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DataPath_RstAfifoFromPC, 0x1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DataPath_RstAfifoFromPC, 0x0);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Adc_MonitorAdcValid, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Adc_MonitorAdcValid, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SysMon_EdgeCheckEnableAcq, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SysMon_EdgeCheckEnableAcq, 1);

            //采集系统传输回路复位成正常模式(非2级传输模式)
            WriteToAllFpga(AcqBdReg.W.FlashOperator_ActionCode, 0);

            //读使能复位
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);

            //临时调试，测试UART误码率
            //var testErrTimes = Hd.CurrProduct?.AcqBd?.UartTest2(AcqBdReg.W.Decimation_PosGapValueH16, 20);

            //临时代码， 触发延迟
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x0A);

            //临时代码， 双板同步触发选择
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x0);


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
        }


        #region 扫窗
        private void ScanProc2AcqCtrl()
        {
            HdCtrl_BoardSync.ResetSyncFlagP2A();

            //switch to test mode
            HdCtrl_BoardSync.SwitchTestFlashPathP2A();
            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Ctrl_a2p_trig_location_scan_switch_pro, 0x0);

            Dictionary<BoardSyncRangeEnum, UInt32> rangeTable = new()
            {
                {BoardSyncRangeEnum.Start, 100 },
                {BoardSyncRangeEnum.Stop,  400},
            };
            HdCtrl_BoardSync.ConfigSyncTapRangeP2A(rangeTable);

          
            HdCtrl_BoardSync.ConfigTapNumP2A(0, 0x2 << 12);

            HdCtrl_BoardSync.StartSearchP2A();

            HdIO.Sleep(100);

            Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> syncflag = HdCtrl_BoardSync.ReadSyncFlagP2A();

            Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> scantap = HdCtrl_BoardSync.ReadTapValueP2A();

            HdCtrl_BoardSync.SaveBoardSyncP2AToFile("./CaliData/SyncBoard/", $"1_{DateTime.Now.ToString("yyMMddhhmmss")}_P2A_Auto_SyncFlag.txt", syncflag);
            HdCtrl_BoardSync.SaveBoardSyncP2AToFile("./CaliData/SyncBoard/", $"1_{DateTime.Now.ToString("yyMMddhhmmss")}_P2A_Auto_ScanTap.txt", scantap);

            Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> tapload = HdCtrl_BoardSync.ReadBoardSyncP2AFromFile("./CaliData/BoardSync_Ctrl_P2A_ScanTapLoad.txt");

            HdCtrl_BoardSync.ResetSyncFlagP2A();

            HdCtrl_BoardSync.ConfigTapNumP2A(tapload, 0x3 << 12);//[13:12] 加载使能打开，打开fix模式

            HdCtrl_BoardSync.StartSearchP2A();

            HdCtrl_BoardSync.ConfigTapNumP2A(tapload, 0x1 << 12);//[13:12] 加载使能关闭，打开fix模式

            HdIO.Sleep(100);

            Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> syncflagcheck = HdCtrl_BoardSync.ReadSyncFlagP2A();

            Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> scantapcheck = HdCtrl_BoardSync.ReadTapValueP2A();

            HdCtrl_BoardSync.SaveBoardSyncP2AToFile("./CaliData/SyncBoard/", $"1_{DateTime.Now.ToString("yyMMddhhmmss")}_P2A_Fix_SyncFlagCheck.txt", syncflagcheck);
            HdCtrl_BoardSync.SaveBoardSyncP2AToFile("./CaliData/SyncBoard/", $"1_{DateTime.Now.ToString("yyMMddhhmmss")}_P2A_Fix_ScanTapCheck.txt", scantapcheck);

            HdCtrl_BoardSync.SwitchNormalPathP2A();
        }

        private void ScanAcq2ProcCtrl()
        {
            HdCtrl_BoardSync.ResetSyncFlagA2P();
            HdCtrl_BoardSync.SwitchTestFlashPathA2P();
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Ctrl_a2p_trig_location_scan_switch_acq, 0);

            Dictionary<BoardSyncRangeEnum, UInt32> rangeTable = new()
            {
                {BoardSyncRangeEnum.Start, 100 },
                {BoardSyncRangeEnum.Stop,  400},
            };
            HdCtrl_BoardSync.ConfigSyncTapRangeA2P(rangeTable);      

            HdCtrl_BoardSync.ConfigTapNumA2P(0, 0x2 << 12);

            HdCtrl_BoardSync.StartSearchA2P();

            HdIO.Sleep(100);

            Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, UInt32>> syncflag = HdCtrl_BoardSync.ReadSyncFlagA2P();

            Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, UInt32>> scantap = HdCtrl_BoardSync.ReadTapValueA2P();

            HdCtrl_BoardSync.SaveBoardSyncA2PToFile("./CaliData/SyncBoard/", $"2_{DateTime.Now.ToString("yyMMddhhmmss")}_A2P_Auto_SyncFlag.txt", syncflag);
            HdCtrl_BoardSync.SaveBoardSyncA2PToFile("./CaliData/SyncBoard/", $"2_{DateTime.Now.ToString("yyMMddhhmmss")}_A2P_Auto_ScanTap.txt", scantap);

            Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, UInt32>> tapload = HdCtrl_BoardSync.ReadBoardSyncA2PFromFile("./CaliData/BoardSync_Ctrl_A2P_ScanTapLoad.txt");

            HdCtrl_BoardSync.ResetSyncFlagA2P();

            HdCtrl_BoardSync.ConfigTapNumA2P(tapload, 0x3 << 12);

            HdCtrl_BoardSync.StartSearchA2P();

            HdCtrl_BoardSync.ConfigTapNumA2P(tapload, 0x1 << 12);

            HdIO.Sleep(100);

            Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, UInt32>> syncflagcheck = HdCtrl_BoardSync.ReadSyncFlagA2P();

            Dictionary<BoardSyncEnumA2P, Dictionary<AcqBdNo, UInt32>> scantapcheck = HdCtrl_BoardSync.ReadTapValueA2P();

            HdCtrl_BoardSync.SaveBoardSyncA2PToFile("./CaliData/SyncBoard/", $"2_{DateTime.Now.ToString("yyMMddhhmmss")}_A2P_Fix_SyncFlagCheck.txt", syncflagcheck);
            HdCtrl_BoardSync.SaveBoardSyncA2PToFile("./CaliData/SyncBoard/", $"2_{DateTime.Now.ToString("yyMMddhhmmss")}_A2P_Fix_ScanTapCheck.txt", scantapcheck);

            HdCtrl_BoardSync.SwitchNormalPathA2P();
        }

        private void AutoScanPro2AcqBoardSync()//????
        {
            HdCtrl_BoardSync.ResetSyncFlagP2A();

            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Ctrl_a2p_trig_location_scan_switch_pro, 0x0);

            HdCtrl_BoardSync.ConfigTapNumP2A(0, 0x3000);

            HdCtrl_BoardSync.ResetSyncFlagA2P();

            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Ctrl_a2p_trig_location_scan_switch_pro, 0x0);//?

            Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, String>> result = new();

            List<UInt32> validCodeList = new List<UInt32>() { 0xf0, 0xe1, 0x0f, 0x78, 0x87, 0xc3, 0x3c, 0x1e };

            List<UInt32> chackTapList = new();
            for (UInt32 tap = 0; tap < 510; tap += 10)
                chackTapList.Add(tap);

            for (Int32 i = 0; i < chackTapList.Count; i++)
            {
                HdCtrl_BoardSync.ResetSyncFlagP2A();

                HdCtrl_BoardSync.ConfigTapNumP2A(chackTapList[i], 0x3 << 12);

                HdCtrl_BoardSync.StartSearchP2A();

                HdIO.Sleep(10);

                Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> readBack = HdCtrl_BoardSync.ReadTestDataP2A();

                foreach (BoardSyncEnumP2A syncType in readBack.Keys)
                {

                    if (!result.ContainsKey(syncType))
                    {
                        result[syncType] = new();
                    }

                    foreach (AcqBdNo acqBd in readBack[syncType].Keys)
                    {
                        if (result[syncType].ContainsKey(acqBd))
                        {
                            result[syncType][acqBd] += validCodeList.Contains(readBack[syncType][acqBd]) ? '1' : '0';
                        }
                        else
                        {
                            result[syncType][acqBd] = validCodeList.Contains(readBack[syncType][acqBd]) ? "1" : "0";
                        }
                    }

                }
            }

            Dictionary<BoardSyncEnumP2A, Dictionary<AcqBdNo, UInt32>> bestTapValue = new();

            foreach (BoardSyncEnumP2A syncType in result.Keys)
            {
                bestTapValue[syncType] = new();

                foreach (AcqBdNo acqBd in result[syncType].Keys)
                {
                    bestTapValue[syncType][acqBd] = chackTapList[FindMaxRange(result[syncType][acqBd], '1')];
                }
            }

            HdCtrl_BoardSync.ConfigTapNumP2A(bestTapValue, 0x3 << 12);

            HdCtrl_BoardSync.SwitchNormalPathP2A();
        }

        private Int32 FindMaxRange(String data, Char key)
        {
            Int32 len = data.Length;
            Int32 findRange = len * 2;

            Int32 curStart = -1;
            Int32 curLen = 0;

            Int32 maxStart = 0;
            Int32 maxLen = 0;

            for (Int32 i = 0; i  < findRange; i++)
            {
                if (data[i % len] == key)
                {
                    if (curLen == 0)
                    {
                        curStart = i % len;
                    }

                    curLen++;
                }
                else
                {
                    if (curLen == 0)
                        continue;

                    if (maxLen < curLen)
                    {
                        maxStart = curStart;
                        maxLen = curLen;
                    }

                    curLen = 0;
                }
                
            }
            return (maxStart + maxLen / 2) % len;
        }
        #endregion

        private void AutoConfigSerdesSync()//????
        {
            ////采集板处理板通信自动校正
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Data_SyncEnable, 1);

            Dictionary<AcqBdNo, BoradSyncRangeValue> tapRange = HdCtrl_BoardSync.ReadBoardSyncDataFromFile("./CaliData/BoardSync_Data_ScanTapRangeLoad.txt");

            HdCtrl_BoardSync.ConfigDataTapRange(tapRange);

            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Data_pro_iserdes_scan_length, 7);

            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Data_pro_iserdes_sync_en, 0);
            //HdIO.Sleep(100);
            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Data_pro_iserdes_sync_en, 1);
            HdIO.Sleep(1000);

            Dictionary<AcqBdNo, UInt32> sycnStatusTable = HdCtrl_BoardSync.ReadSyncFlagData();
            HdCtrl_BoardSync.SaveBoardSyncDataToFile("./CaliData/SyncBoard/", $"0_{DateTime.Now.ToString("yyMMddhhmmss")}_Data_SyncFlagCheck.txt", sycnStatusTable);

            Dictionary<AcqBdNo, UInt32> sycnTapTable = HdCtrl_BoardSync.ReadSyncTapData();
            HdCtrl_BoardSync.SaveBoardSyncDataToFile("./CaliData/SyncBoard/", $"0_{DateTime.Now.ToString("yyMMddhhmmss")}_Data_SyncTapCheck.txt", sycnTapTable);

            //HdIO.Sleep(200);
            //HdIO.WriteReg(ProcBdReg.W.BoardSync_Data_pro_iserdes_sync_en, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.BoardSync_Data_SyncEnable, 0);
        }

        private void AutoConfigPCIE_IDDRSync(UInt32 DelayValue)
        {
            //lchy 20251028
            ////采集板处理板通信自动校正
         /*   HdIO.WriteReg(PcieBdReg.W.DataPath_IddrIdelay3EnVTC, 0x1);
            //HdIO.WriteReg(PcieBdReg.W.DataPath_IddrIdelay3Rst, 0x7);
            HdIO.WriteReg(PcieBdReg.W.DataPath_IddrIdelay3TapValue, DelayValue);
            //HdIO.WriteReg(PcieBdReg.W.DataPath_IddrIdelay3Rst, 0x0);
            //================Wait Ready
            HdIO.WriteReg(PcieBdReg.W.DataPath_IddrIdelay3EnVTC, 0x0);
            HdIO.WriteReg(PcieBdReg.W.DataPath_IddrIdelay3LoadEn, 0x0);
            HdIO.WriteReg(PcieBdReg.W.DataPath_IddrIdelay3LoadEn, 0x1);
            HdIO.WriteReg(PcieBdReg.W.DataPath_IddrIdelay3LoadEn, 0x0);
            HdIO.WriteReg(PcieBdReg.W.DataPath_IddrIdelay3EnVTC, 0x1);
         */
        }//????
        /// <summary>
        /// 板内测试
        /// </summary>
        public override void Test()
        {

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
                PllWrite(AcqBdNo.B3, addr, data);
                PllWrite(AcqBdNo.B5, addr, data);
                HdIO.Sleep(1);
            }

            for (int i = 4; i < 8; i++)
            {
                addr = GetHdWord(i, 0) & 0xffffff00;
                addr >>= 8;
                data = GetHdWord(i, 0) & 0xff;
                PllWrite(AcqBdNo.B5, addr, data);
                HdIO.Sleep(1);
            }
        }
        #endregion

        #region 5200
        //      private void AD5200_Init(int anaChannelID, AcqBdNo fpgaIndex, int SubbandIndex, int adcIndex, bool bReCalc0x29Register)
        //      {
        //          WriteReg(AcqBdReg.W.Adc_CS, fpgaIndex, (UInt32)(adcIndex + 1));
        //          //SOFT RESET
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0000, 0xB0);
        //          HdIO.Sleep(2);
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0000, 0x30);
        //          //SET CALI EN 校准要在设置链路之前

        //          //SPI config
        //          //SendCmdToAD5200(fpgaIndex, adcIndex, 0x0010, 0x01);//3200

        //          //stop the JESD204B state machine, stop the calibration state machine 
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0200, 0x00); //JESD_EN diable
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0061, 0x00); //CALI_EN diable
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x02B0, 0x00); //sysref cali diable
        //                                                                      //MISCELLANEOUS ANALOG REGISTERS
        //                                                                      //使用校准数据
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x002B, 0x15); //
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x02A2, 0x18); //

        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x002A, 0x00); //SYSREF_LVPECL_EN=0,DEVCLK_LVPECL_EN=0, INVERTED//00
        //          //SendCmdToAD5200(adcIndex, 0x002A, 0x22); //SYSREF_LVPECL_EN=0,DEVCLK_LVPECL_EN=0, INVERTED
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0038, 0x00); //BG_BYPASS
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x003B, 0x00); //not use TMSTP± input,TMSTP_LVPECL_EN=0
        //                                                                      //SERIALIZER REGISTERS
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0048, 0x04); //Serializer Pre-Emphasis Control

        //          ////////////////////FS_RANGE_A////////////////////////////////
        //          SendCmdToAD5200(adcIndex, 0x0030, 0x00);//0x2000:500mVPP ,0xA000:800mVPP(default) , 0xffff:1000mVPP
        //          SendCmdToAD5200(adcIndex, 0x0031, 0x20);
        //          ////////////////////FS_RANGE_B////////////////////////////////
        //   //       SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0032, 0x00); //0x2000:500mVPP ,0xA000:800mVPP(default) , 0xffff:1000mVPP
        //   //       SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0033, 0x20);//0xa0

        //          //SET jesd204 
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0201, 0x01); //JMODE =  single channel 10GSPS
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0202, 0x1F); //JMODE,K=32 frame number in one multifrane
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0203, 0x01); //SEL normal sync operation
        //          //SendCmdToAD5200(fpgaIndex, adcIndex, 0x0204, 0x03); //use SYNCSE pin for sync //2's complement // 8B/10B scrambler disable
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0204, 0x01); //use SYNCSE pin for sync //offset binary // 8B/10B scrambler disable
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0205, 0x00); //adc test mode , ramp test = 0x04,Transport layer test mode:05
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0206, 0x00); // DID: device identifier
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0207, 0x00); //COMMA CHAR = K28.5 //3200写了0x00
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0209, 0x00); //A ADC channel & B ADC channelis powered up
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x020A, 0x00); //Only the link A layer clocks for extra lanes are enabled
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x020B, 0x00); //Only the link B layer clocks for extra lanes are enabled

        //          //CALIBRATION REGISTERS
        //          //SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0060, (UInt32)(((int?)Hd.CurrProduct?.HardwareConfig?.ADC5200SignalInputPort) ?? 1));//1 : INA is used (default)   2 : INB is used
        //          int AdcSigalInputPort_AIs1 = GetAdcInpiutPort((int)fpgaIndex, adcIndex);//固定，不改变
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0060, (UInt32)AdcSigalInputPort_AIs1);
        ////          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0060, 1);

        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0062, 0x05);//CAL_CFG0,Disable all calibration
        //                                                                     //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x0068, 0x61);//CAL_AVG default averaging amount 
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0068, 0x77);//CAL_AVG MAX averaging amount 
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x006B, 0x06);//Use the CAL_SOFT_TRIG register for the calibration trigger,CALSTAT output pin is always low
        //                                                                     //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x006C, 0x01);//CAL_SOFT_TRIG = 1 
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x006E, 0x88);//Disables low-power background calibration (default)
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0070, 0x00);//CAL_DATA_EN=0
        //                                                                     //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x0071, 0x00);//CAL_DATA=0
        //                                                                     //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x007A, 0x00);//GAIN_TRIM_A=0
        //                                                                     //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x007B, 0x00);//GAIN_TRIM_B=0
        //                                                                     //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x007C, 0x00);//BG_TRIM=0
        //                                                                     //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x007E, 0x00);//RTRIM_A=0
        //                                                                     //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x007F, 0x00);//RTRIM_B=0


        //          //SYSREF CALIBRATION REGISTERS
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x02B1, 0x05);//SRC_HDUR=01,SRC_AVG=01
        //                                                                     //SendCmdToAD5200_OneFpga(fpgaIndex, fpgaIndex, adcIndex, 0x02B0, 0x01);//SYSREF calibration enables SRC_EN

        //          //Program CAL_EN = 1 to enable the calibration state machine
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0061, 0x01); //CALI_EN able
        //                                                                      //LSB CONTROL REGISTERS
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0160, 0x00); //TIMESTAMP_EN=0
        //                                                                      //ADC BANK REGISTERS

        //          // Program JESD_EN = 1 to re-start the JESD204B state machine and allow the link to restart
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0200, 0x01); //JESD_EN able

        //          HdIO.Sleep(100);
        //          //phase-offset-gain 校准数据
        //          //AdjustAdc_Phase(false, SubbandIndex, fpgaIndex, adcIndex);
        //          //此处与其他项目不同。与通道无关。不使用通道号，而是使用FPGAIndex
        //          //此时的校准数据的组织的含义与其他项目也不一样。由于10个采集卡全部插满，此时的子带号与FPGAIndex的号是一致的。

        //          //AdjustAdc_Phase(false, (int)fpgaIndex, fpgaIndex, adcIndex);

        //          //AdjustAdc_Offset(anaChannelID, fpgaIndex, adcIndex);
        //          //原来在此处同时调整ADCGain，在上电初始化时没有通道信息，故不能再在此进行设置。
        //          //AdjustAdc_Gain(false, anaChannelID, SubbandIndex, fpgaIndex, adcIndex);

        //          #region 压稳态窗 SyncSampleClock
        //          //uint data = TiAdc_SyncSampleClock.Default[anaChannelID][adcIndex].SampleClockDelay & 0x0f;//只有低4位有效
        //          //此处与其他项目不同，此处不使用通道号而是子带号，此项目的 子带号与FPGAIndex是一致的。
        //          //此时的校准数据的组织的含义与其他项目也不一样。由于10个采集卡全部插满，此时的子带号与FPGAIndex的号是一致的。
        //          uint data = TiAdc_SyncSampleClock.Default[(int)fpgaIndex][adcIndex].Sample20GClockDelay & 0x0f;//只有低4位有效
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0029, 0x30 | data);//use SYSREF calibration,delay steps are finer,enable the SYSREF receiver circuit
        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0029, 0x70 | data);//SYSREF_RECV_EN must be set before setting SYSREF_PROC_EN
        //          #endregion

        //          SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x7A, 1);//打开增益可调

        //          WriteReg(AcqBdReg.W.Adc_CS, fpgaIndex, 0x00);
        //      }
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
             UInt32 tmp = ((0x000 << 23) | (Address_15bit << 8) | Commmand_8bit);//(0x001 << 23)Instruction R(1b'1)/W(1b'0)

            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Adc_DataCmdL8, fpagIndex, tmp & 0xffff);
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Adc_DataCmdH16, fpagIndex, (tmp >> 8) & 0xffff);
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Adc_ConfigEnable, fpagIndex, 0xc0);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(1);
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Adc_ConfigEnable, fpagIndex, 0xe0);
            HdIO.Sleep(1);
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
        #endregion

        #region 7044
        private void HMC7044Write(UInt32 ADDR, UInt32 data)//ADDR's width is 13bits; data's width is 8bits
        {
            UInt32 SDATA;//SDATA's width is 24bits
            UInt32 RorW = 0x0000000 & 0x00E00000;//MSB bit23 R/W bit22(W1) bit21(W0) Multibyte field 2'b00  RorW[23:21]=0|00
            SDATA = RorW | ((ADDR << 8) & 0x001fff00) | (data & 0x000000ff);

            WriteToAllFpga(AcqBdReg.W.PllConfig_HMC7044Data_Effect, 0x00);
            WriteToAllFpga(AcqBdReg.W.PllConfig_HMC7044Data_L16, SDATA & 0xffff);
            WriteToAllFpga(AcqBdReg.W.PllConfig_HMC7044Data_H8, (SDATA >> 16) & 0xffff);
            WriteToAllFpga(AcqBdReg.W.PllConfig_HMC7044Data_Effect, 0x01);
            HdIO.WaitForSpiTransfer(1, 5);
            HdIO.Sleep(10);
        }

        private void HMC7044Write(UInt32 ADDR, Dictionary<AcqBdNo, UInt32> dataTable)
        {
            UInt32 SDATA;//SDATA's width is 24bits
            UInt32 RorW = 0x0000000 & 0x00E00000;//MSB bit23 R/W bit22(W1) bit21(W0) Multibyte field 2'b00  RorW[23:21]=0|00

            foreach (AcqBdNo acqBd in dataTable.Keys)
            {
                SDATA = RorW | ((ADDR << 8) & 0x001fff00) | (dataTable[acqBd] & 0x000000ff);
                WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_Effect, acqBd, 0x00);
                WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_L16, acqBd, SDATA & 0xffff);
                WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_H8, acqBd, (SDATA >> 16) & 0xffff);
                WriteReg(AcqBdReg.W.PllConfig_HMC7044Data_Effect, acqBd, 0x01);
            }

            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(10);
        }

        private String SpecailHmcParamFile = "./CaliData/Pll7044Config.txt";
        /// <summary>
        /// mode_register_AcqBdNo_value
        /// </summary>
        /// <returns></returns>
        private Dictionary<UInt32, Dictionary<AcqBdNo, UInt32>> LoadSpecailHmcParam()
        {
            Int32 curtempmode = Hd.UIMessage?.TempMode ?? 1;
            Dictionary<UInt32, Dictionary<AcqBdNo, UInt32>> ans = new();

            if (File.Exists(SpecailHmcParamFile))
            {
                StreamReader sr = new(SpecailHmcParamFile);
                while (!sr.EndOfStream)
                { 
                    String? tmpstr = sr.ReadLine();
                    if (tmpstr == null || !tmpstr.Contains('_'))
                        continue;

                    String[] info = tmpstr.Split('_');
                    if (info.Length < 4)
                        continue;

                    Int32 tempmode = Int32.Parse(info[0].Trim());
                    if (tempmode != curtempmode)
                        continue;

                    UInt32 register = Convert.ToUInt32(info[1].Trim(), 16);
                    if (!ans.ContainsKey(register))
                        ans[register] = new();

                    AcqBdNo acqbd = Enum.Parse<AcqBdNo>(info[2].Trim());
                    UInt32 data = Convert.ToUInt32(info[3].Trim(), 16);
                    ans[register][acqbd] = data;
                }
            }
            return ans;
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
            HMC7044Write(0x0000, 0x01);
            HMC7044Write(0x0000, 0x00);
            HMC7044Write(0x0001, 0x40);
            HMC7044Write(0x0002, 0x04);
            HMC7044Write(0x0003, 0x37);
            HMC7044Write(0x0004, 0x7F);
            HMC7044Write(0x0005, 0x98); // SYNC MODE (82、58) 98
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
            HMC7044Write(0x0039, 0x00);
            HMC7044Write(0x003A, 0x00);
            HMC7044Write(0x003B, 0x00);
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
            HMC7044Write(0x005D, 0x05); // [3:0]MSB\\05
            //-----------------------------//
            // 		Clock Output Channel
            //---------------------------//
            // Output Mode Seclect
            // DCLK Mode : 0xF3
            // SYSREF Pluse Mode : 0x5D
            // Turn Off : 0x00
            HMC7044Write(0x00C8, 0x5d); // DCLKOUT0 @ 7043B sync
            HMC7044Write(0x00D2, 0xF3); // SCLKOUT1 @ 7043a 输入时钟 2.5G
            HMC7044Write(0x00DC, 0x5d); // DCLKOUT2 @ 2595A SYNC信号		*
            HMC7044Write(0x00E6, 0x5D); // SCLKOUT3 @ 7043a sync    
            HMC7044Write(0x00F0, 0x5d); // DCLKOUT4 @ 2595B_SYNC
            HMC7044Write(0x00FA, 0xF3); // SCLKOUT5 @2595A SYSREF信号	
            HMC7044Write(0x0104, 0xF3); // DCLKOUT6 @2595B_SYSREF
            HMC7044Write(0x010E, 0xF3); // SCLKOUT7 @250M-15.625M ,作为0304A的输入钟	*
            HMC7044Write(0x0118, 0xF3); // DCLKOUT8 @2595C SYSREF信号
            HMC7044Write(0x0122, 0xF3); // SCLKOUT9 @0304B的输入钟
            HMC7044Write(0x012C, 0x5d); // DCLKOUT10 @2595C_SYNC
            HMC7044Write(0x0136, 0xF3); // SCLKOUT11 @2595D SYSREF信号
            HMC7044Write(0x0140, 0xF3); // DCLKOUT12 @7043B 输入时钟 2.5G
            HMC7044Write(0x014A, 0x5d); // SCLKOUT13 @ 2595D_SYNC

            // Output divider
            // Even divide ratios from 2 to 4094
            // Odd divide ratios are 1、3、5
            HMC7044Write(0x00C9, 0x00); // [7:0]LSB
            HMC7044Write(0x00CA, 0x05); // [3:0]MSB DCLKOUT0 @ 7043B sync
            HMC7044Write(0x00D3, 0x02); // [7:0]LSB
            HMC7044Write(0x00D4, 0x00); // [3:0]MSB SCLKOUT1 @ 7043a 输入时钟 2.5G

            HMC7044Write(0x00DD, 0x00); // [7:0]LSB
            HMC7044Write(0x00DE, 0x05); // [3:0]MSB DCLKOUT2 @ 2595A SYNC信号	//280	
            HMC7044Write(0x00E7, 0x00); // [7:0]LSB
            HMC7044Write(0x00E8, 0x05); // [3:0]MSB SCLKOUT3 @ 7043a sync

            HMC7044Write(0x00F1, 0x00); // [7:0]LSB
            HMC7044Write(0x00F2, 0x05); // [3:0]MSB DCLKOUT4 @ 15.625MHz        //2595B_SYNC //280
            HMC7044Write(0x00FB, 0x00); // [7:0]LSB
            HMC7044Write(0x00FC, 0x05); // [3:0]MSB SCLKOUT5 @ 00304 SYSREF     //2595A sysref信号//05		

            HMC7044Write(0x0105, 0x00); // [7:0]LSB
            HMC7044Write(0x0106, 0x05); // [3:0]MSB DCLKOUT6 @ 250MHZ           //2595B_SYSREF//05
            HMC7044Write(0x010F, 0xA0); // [7:0]LSB
            HMC7044Write(0x0110, 0x00); // [3:0]MSB SCLKOUT7 @ 250MHZ           //250M-15.625M ,作为0304A的输入钟	
            HMC7044Write(0x0119, 0x00); // [7:0]LSB
            HMC7044Write(0x011A, 0x05); // [3:0]MSB DCLKOUT8 @ 250MHZ           //2595C SYSREF信号//05		
            HMC7044Write(0x0123, 0xA0); // [7:0]LSB
            HMC7044Write(0x0124, 0x00); // [3:0]MSB SCLKOUT9 @ 250MHz           //0304B的输入钟

            HMC7044Write(0x012D, 0x00); // [7:0]LSB
            HMC7044Write(0x012E, 0x05); // [3:0]MSB DCLKOUT10 @ 50MHZ           //2595C_SYNC //140
            HMC7044Write(0x0137, 0x00); // [7:0]LSB
            HMC7044Write(0x0138, 0x05); // [3:0]MSB SCLKOUT11 @ 156.25M         //2595D SYSREF信号//05
            HMC7044Write(0x0141, 0x02); // [7:0]LSB
            HMC7044Write(0x0142, 0x00); // [3:0]MSB DCLKOUT12 @ 156.25M         //7043B 输入时钟 2.5G
            HMC7044Write(0x014B, 0x00); // [7:0]LSB
            HMC7044Write(0x014C, 0x05); // [3:0]MSB SCLKOUT13 @ 156.25M         //2595D_SYNC //140

            // Fine analog delay
            // Step size 25ps
            // 0~23 effective

            //0816 305 CIJ
            HMC7044Write(0x00CB, 0x00); // DCLKOUT0 
            HMC7044Write(0x00DF, 0x00); // DCLKOUT2 
            HMC7044Write(0x00F3, 0x00); // DCLKOUT4  
            HMC7044Write(0x0107, 0x00); // DCLKOUT6 
            HMC7044Write(0x011B, 0x00); // DCLKOUT8
            HMC7044Write(0x012F, 0x00); // DCLKOUT10//02
            //HMC7044Write(0x0143, 0x0f); // DCLKOUT12//0F
            Dictionary<UInt32, Dictionary<AcqBdNo, UInt32>> specialcfg = LoadSpecailHmcParam();

            foreach (UInt32 register in specialcfg.Keys)
            {
                HMC7044Write(register, specialcfg[register]);
            }
            #region DCLKOUT12
            //DCLKOUT12
            //Dictionary<AcqBdNo, UInt32> dataTable_7043B = new()
            //{
            //    [AcqBdNo.B1] = 0x00,
            //    [AcqBdNo.B2] = 0x00,
            //    [AcqBdNo.B3] = 0x00,
            //    [AcqBdNo.B4] = 0x00,
            //    [AcqBdNo.B5] = 0x0,
            //    [AcqBdNo.B6] = 0x0,
            //    [AcqBdNo.B7] = 0x0F,
            //    [AcqBdNo.B8] = 0x0,
            //    [AcqBdNo.B9] = 0x0,
            //    [AcqBdNo.B10] = 0x08,
            //    [AcqBdNo.B11] = 0x08,
            //    [AcqBdNo.B12] = 0x0F,
            //};
            //HMC7044Write(0x0143, dataTable_7043B);

            //Dictionary<AcqBdNo, UInt32> dataTable_7043A = new()
            //{
            //    [AcqBdNo.B1] = 0x00,
            //    [AcqBdNo.B2] = 0x00,
            //    [AcqBdNo.B3] = 0x00,
            //    [AcqBdNo.B4] = 0x00,
            //    [AcqBdNo.B5] = 0x00,
            //    [AcqBdNo.B6] = 0x00,
            //    [AcqBdNo.B7] = 0x00,
            //    [AcqBdNo.B8] = 0x00,
            //    [AcqBdNo.B9] = 0x00,//08
            //    [AcqBdNo.B10] = 0x00,
            //    [AcqBdNo.B11] = 0x00,
            //    [AcqBdNo.B12] = 0x00,
            //};
            //HMC7044Write(0x00D5, dataTable_7043A);
            #endregion

            //          HMC7044Write(0x00D5, 0x00); // SCLKOUT1
            HMC7044Write(0x00E9, 0x00); // SCLKOUT3//0F
            HMC7044Write(0x00FD, 0x00); // SCLKOUT5
            HMC7044Write(0x0111, 0x00); // SCLKOUT7
            HMC7044Write(0x0125, 0x00); // SCLKOUT9
            HMC7044Write(0x0139, 0x00); // SCLKOUT11
            HMC7044Write(0x014D, 0x00); // SCLKOUT13//02//04



            ////0816 bkup ZQB
            //HMC7044Write(0x00CB, 0x00); // DCLKOUT0 
            //HMC7044Write(0x00DF, 0x04); // DCLKOUT2 
            //HMC7044Write(0x00F3, 0x06); // DCLKOUT4  
            //HMC7044Write(0x0107, 0x00); // DCLKOUT6 
            //HMC7044Write(0x011B, 0x00); // DCLKOUT8
            //HMC7044Write(0x012F, 0x04); // DCLKOUT10//02
            //HMC7044Write(0x0143, 0x0F); // DCLKOUT12//0F
            //HMC7044Write(0x00D5, 0x00); // SCLKOUT1
            //HMC7044Write(0x00E9, 0x0F); // SCLKOUT3//0F
            //HMC7044Write(0x00FD, 0x00); // SCLKOUT5
            //HMC7044Write(0x0111, 0x00); // SCLKOUT7
            //HMC7044Write(0x0125, 0x00); // SCLKOUT9
            //HMC7044Write(0x0139, 0x00); // SCLKOUT11
            //HMC7044Write(0x014D, 0x04); // SCLKOUT13//02//04


            // Coarse digital deladelay
            // Step size 1/2 VCO cyclk//10G-100ps-50ps
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

            HMC7044Write(0x00D0, 0x89); // DCLKOUT0 //7043B sync
            HMC7044Write(0x00DA, 0x89); // SCLKOUT1 //7043a 输入时钟 2.5G  88                
            HMC7044Write(0x00E4, 0x80); // DCLKOUT2 //2595A SYNC信号//90		*
            HMC7044Write(0x00EE, 0x89); // SCLKOUT3 @ 2595B_SYSREF                                              //7043a sync

            //sysref:LVPECL输出只有100mv(单端)过完00304有200mv(单端)
            HMC7044Write(0x00F8, 0x80);//DCLKOUT4 LVPECL   //90); // DCLKOUT4 LVDS                              //2595B_SYNC
         
            //sysref:LVDS可以被00304正常识别,00304输出500mv(单端)

            HMC7044Write(0x0102, 0x90);//SCLKOUT5 CML(100ohm) (PULSE)                                           //2595A SYSREF信号	
 

            HMC7044Write(0x010C, 0x90); // DCLKOUT6 CML 100ohm                                                   //2595B_SYSREF

            HMC7044Write(0x0116, 0x10); // SCLKOUT7 LVDS                                                        //250M-15.625M ,作为0304A的输入钟	*//10
            HMC7044Write(0x0120, 0x90); // DCLKOUT8 LVDS                                                        //2595C_SYSREF
            HMC7044Write(0x012A, 0x10); // SCLKOUT9 LVDS                                                        //0304B的输入钟

            HMC7044Write(0x0134, 0x80); // DCLKOUT10 LVPECL                                                     //2595C_SYNC
            HMC7044Write(0x013E, 0x90); // SCLKOUT11                                                            //2595D SYSREF信号
            HMC7044Write(0x0148, 0x89); // DCLKOUT12                                                            //7043B 输入时钟 2.5G    
            HMC7044Write(0x0152, 0x80); // SCLKOUT13                                                            //2595D_SYNC
                                        //-----------------------------//
                                        // 		Input buffer
                                        //---------------------------//
            HMC7044Write(0x000A, 0x09); // CLKIN0/RFSYNCIN
            HMC7044Write(0x000B, 0x07); // CLKIN1
            HMC7044Write(0x000C, 0x07); // CLKIN2
            HMC7044Write(0x000D, 0x09); // CLKIN3   //0X03
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

        #region 7043A
        private void PllWrite7043A(UInt32 ADDR, UInt32 data)//ADDR's width is 13bits; data's width is 8bits
        {
            UInt32 SDATA;//SDATA's width is 24bits
            UInt32 RorW = 0x0000000 & 0x00E00000;//MSB bit23 R/W bit22(W1) bit21(W0) Multibyte field 2'b00  RorW[23:21]=0|00
            SDATA = RorW | ((ADDR << 8) & 0x001fff00) | (data & 0x000000ff);

            //WriteToAllFpga(AcqBdReg.W.PllConfig_7043AWriteDataEffect, 0x00);
            //WriteToAllFpga(AcqBdReg.W.PllConfig_7043AWriteData_L16, SDATA & 0xffff);
            //WriteToAllFpga(AcqBdReg.W.PllConfig_7043AWriteData_H8, (SDATA >> 16) & 0xffff);
            //WriteToAllFpga(AcqBdReg.W.PllConfig_7043AWriteDataEffect, 0x01);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(10);
        }//????


        private void InitACQ_HMC7043A()//????
        //    地址 , 值
        {
            //ACQ5200_HMC7044 start
            //WriteToAllFpga(AcqBdReg.W.PllConfig_7043AReset, 0);
            HdIO.Sleep(10);
            //WriteToAllFpga(AcqBdReg.W.PllConfig_7043AReset, 1);
            HdIO.Sleep(10);
            //WriteToAllFpga(AcqBdReg.W.PllConfig_7043AReset, 0);

            HdIO.Sleep(10);


            PllWrite7043A(0x0000, 0x01);
            PllWrite7043A(0x0000, 0x00);
            PllWrite7043A(0x0001, 0x00);
            PllWrite7043A(0x0002, 0x00);
            PllWrite7043A(0x0003, 0x34);
            PllWrite7043A(0x0004, 0x7F);
            PllWrite7043A(0x0006, 0x00);
            PllWrite7043A(0x0007, 0x00);
            PllWrite7043A(0x000A, 0x0D); //07
            PllWrite7043A(0x000B, 0x0D); //07
            PllWrite7043A(0x0046, 0x00);
            PllWrite7043A(0x0050, 0x28);
            PllWrite7043A(0x0050, 0x2F);
            PllWrite7043A(0x0050, 0x28);
            PllWrite7043A(0x0054, 0x03);
            PllWrite7043A(0x005A, 0x01); // PULSE NUM
            PllWrite7043A(0x005B, 0x04);
            PllWrite7043A(0x005C, 0x00); // PULSE CNT LSB
            PllWrite7043A(0x005D, 0x05); // PULSE CNT HSB
            PllWrite7043A(0x0064, 0x00);
            PllWrite7043A(0x0065, 0x00);
            PllWrite7043A(0x0071, 0x10);
            PllWrite7043A(0x0078, 0x00);
            PllWrite7043A(0x0079, 0x00);
            PllWrite7043A(0x007A, 0x00);
            PllWrite7043A(0x007D, 0x00);
            PllWrite7043A(0x0091, 0x00);
            PllWrite7043A(0x0098, 0x00);
            PllWrite7043A(0x0099, 0x00);
            PllWrite7043A(0x009A, 0x00);
            PllWrite7043A(0x009B, 0xAA);
            PllWrite7043A(0x009C, 0xAA);
            PllWrite7043A(0x009D, 0xAA);
            PllWrite7043A(0x009E, 0xAA);
            PllWrite7043A(0x009F, 0x55);
            PllWrite7043A(0x00A0, 0x56);
            PllWrite7043A(0x00A1, 0x97);
            PllWrite7043A(0x00A2, 0x03);
            PllWrite7043A(0x00A3, 0x00);
            PllWrite7043A(0x00A4, 0x00);
            PllWrite7043A(0x00AD, 0x00);
            PllWrite7043A(0x00AE, 0x08);
            PllWrite7043A(0x00AF, 0x50);
            PllWrite7043A(0x00B0, 0x09);
            PllWrite7043A(0x00B1, 0x0D);
            PllWrite7043A(0x00B2, 0x00);
            PllWrite7043A(0x00B3, 0x00);
            PllWrite7043A(0x00B5, 0x00);
            PllWrite7043A(0x00B6, 0x00);
            PllWrite7043A(0x00B7, 0x00);
            PllWrite7043A(0x00B8, 0x00);
            //------------------------------------------------
            //                 通道设置
            //-------------------------------------------
            //----------------------
            //     Output 模式
            //------------------------
            PllWrite7043A(0x00C8, 0xF3); // DCLKOUT0        //FPGA1 GT sysref  250M  
            PllWrite7043A(0x00D2, 0x00); // SCLKOUT1        //FPGA1 GT_TX_250M //开启：F3，关闭：00 
            PllWrite7043A(0x00DC, 0x00); // DCLKOUT2  
            PllWrite7043A(0x00E6, 0x00); // SCLKOUT3  
            PllWrite7043A(0x00F0, 0x00); // DCLKOUT4  
            PllWrite7043A(0x00FA, 0x00); // SCLKOUT5  
            PllWrite7043A(0x0104, 0x00); // DCLKOUT6  
            PllWrite7043A(0x010E, 0x00); // SCLKOUT7
            PllWrite7043A(0x0118, 0xF3); // DCLKOUT8        //FPGA1 204bsysref  900K
            PllWrite7043A(0x0122, 0x00); // SCLKOUT9
            PllWrite7043A(0x012C, 0x00); // DCLKOUT10       //FPGA1 204bsysref  900K 
            PllWrite7043A(0x0136, 0xF3); // SCLKOUT11       //FPGA1 GT sysref  250M        
            PllWrite7043A(0x0140, 0xF3); // DCLKOUT12       //FPGA1 GT sysref  250M  
            PllWrite7043A(0x014A, 0xF3); // SCLKOUT13       //FPGA1 GT sysref  250M 
            //----------------------
            //     Output 分频
            //------------------------
            //NC
            PllWrite7043A(0x00C9, 0x05); // [7:0]LSB
            PllWrite7043A(0x00CA, 0x00); // [3:0]MSB DCLKOUT0       //FPGA1 GT sysref  250M         
            PllWrite7043A(0x00D3, 0x05); // [7:0]LSB
            PllWrite7043A(0x00D4, 0x00); // [3:0]MSB SCLKOUT1       //FPGA1 GT sysref  250M  
            PllWrite7043A(0x00DD, 0xFA); // [7:0]LSB
            PllWrite7043A(0x00DE, 0x00); // [3:0]MSB DCLKOUT2 
            PllWrite7043A(0x00E7, 0xFA); // [7:0]LSB
            PllWrite7043A(0x00E8, 0x00); // [3:0]MSB SCLKOUT3 
            PllWrite7043A(0x00F1, 0xFA); // [7:0]LSB 
            PllWrite7043A(0x00F2, 0x00); // [3:0]MSB DCLKOUT4 
            PllWrite7043A(0x00FB, 0xFA); // [7:0]LSB
            PllWrite7043A(0x00FC, 0x00); // [3:0]MSB SCLKOUT5 
            PllWrite7043A(0x0105, 0x05); // [7:0]LSB
            PllWrite7043A(0x0106, 0x00); // [3:0]MSB DCLKOUT6 
            PllWrite7043A(0x010F, 0x00); // [7:0]LSB
            PllWrite7043A(0x0110, 0x00); // [3:0]MSB SCLKOUT7 
            PllWrite7043A(0x0119, 0x80); // [7:0]LSB
            PllWrite7043A(0x011A, 0x02); // [3:0]MSB DCLKOUT8       //FPGA1 204bsysref  900K//0280
            PllWrite7043A(0x0123, 0x00); // [7:0]LSB
            PllWrite7043A(0x0124, 0x00); // [3:0]MSB SCLKOUT9 
            PllWrite7043A(0x012D, 0x80); // [7:0]LSB
            PllWrite7043A(0x012E, 0x02); // [3:0]MSB DCLKOUT10      //FPGA1 204bsysref  900K//0280
            PllWrite7043A(0x0137, 0x05); // [7:0]LSB
            PllWrite7043A(0x0138, 0x00); // [3:0]MSB SCLKOUT11      //FPGA1 GT sysref  250M 
            PllWrite7043A(0x0141, 0x05); // [7:0]LSB
            PllWrite7043A(0x0142, 0x00); // [3:0]MSB DCLKOUT12      //FPGA1 GT sysref  250M 
            PllWrite7043A(0x014B, 0x05); // [7:0]LSB
            PllWrite7043A(0x014C, 0x00); // [3:0]MSB SCLKOUT13      //FPGA1 GT sysref  250M 
            //KEEP
            //Pluse
            //----------------------
            //     模拟延迟
            //------------------------
            //NO USE      
            PllWrite7043A(0x00D5, 0x00); // SCLKOUT1  
            PllWrite7043A(0x00E9, 0x00); // SCLKOUT3
            PllWrite7043A(0x00FD, 0x00); // SCLKOUT5
            PllWrite7043A(0x0107, 0x00); // DCLKOUT6
            PllWrite7043A(0x0111, 0x00); // SCLKOUT7
            PllWrite7043A(0x011B, 0x00); // DCLKOUT8
            PllWrite7043A(0x0125, 0x00); // SCLKOUT9
            PllWrite7043A(0x0143, 0x00); // DCLKOUT12
            //KEEP
            PllWrite7043A(0x0139, 0x00); // SCLKOUT11
            PllWrite7043A(0x014D, 0x00); // SCLKOUT13
            //Pluse                    ;
            PllWrite7043A(0x00CB, 0x00); // DCLKOUT0
            PllWrite7043A(0x00DF, 0x00); // DCLKOUT2
            PllWrite7043A(0x00F3, 0x00); // DCLKOUT4
            PllWrite7043A(0x012F, 0x00); // DCLKOUT10
            //----------------------
            //     数字延迟
            //------------------------
            //NO USE
            PllWrite7043A(0x00D6, 0x00); // SCLKOUT1
            PllWrite7043A(0x00EA, 0x00); // SCLKOUT3
            PllWrite7043A(0x00FE, 0x00); // SCLKOUT5
            PllWrite7043A(0x0108, 0x00); // DCLKOUT6
            PllWrite7043A(0x0112, 0x00); // SCLKOUT7 
            PllWrite7043A(0x011C, 0x00); // DCLKOUT8
            PllWrite7043A(0x0126, 0x00); // SCLKOUT9
            PllWrite7043A(0x0144, 0x00); // DCLKOUT12
            //KEEP                     ;
            PllWrite7043A(0x013A, 0x00); // SCLKOUT11
            PllWrite7043A(0x014E, 0x00); // SCLKOUT13
            //Pluse                    ;
            PllWrite7043A(0x00CC, 0x00); // DCLKOUT0
            PllWrite7043A(0x00E0, 0x00); // DCLKOUT2
            PllWrite7043A(0x00F4, 0x00); // DCLKOUT4
            PllWrite7043A(0x0130, 0x00); // DCLKOUT10
                                         //----------------------
                                         //     多跳数字延迟
                                         //------------------------
                                         //NO USE
            PllWrite7043A(0x00D7, 0x00); // [7:0]LSB
            PllWrite7043A(0x00D8, 0x00); // [3:0]MSB SCLKOUT1
            PllWrite7043A(0x00EB, 0x00); // [7:0]LSB
            PllWrite7043A(0x00EC, 0x00); // [3:0]MSB SCLKOUT3
            PllWrite7043A(0x00FF, 0x00); // [7:0]LSB
            PllWrite7043A(0x0100, 0x00); // [3:0]MSB SCLKOUT5
            PllWrite7043A(0x0109, 0x00); // [7:0]LSB
            PllWrite7043A(0x010A, 0x00); // [3:0]MSB DCLKOUT6  
            PllWrite7043A(0x0113, 0x00); // [7:0]LSB
            PllWrite7043A(0x0114, 0x00); // [3:0]MSB SCLKOUT7
            PllWrite7043A(0x011D, 0x00); // [7:0]LSB
            PllWrite7043A(0x011E, 0x00); // [3:0]MSB DCLKOUT8
            PllWrite7043A(0x0127, 0x00); // [7:0]LSB
            PllWrite7043A(0x0128, 0x00); // [3:0]MSB SCLKOUT9                 
            PllWrite7043A(0x0145, 0x00); // [7:0]LSB
            PllWrite7043A(0x0146, 0x00); // [3:0]MSB DCLKOUT12
            //KEEP                     ;
            PllWrite7043A(0x013B, 0x00); // [7:0]LSB
            PllWrite7043A(0x013C, 0x00); // [3:0]MSB SCLKOUT11    
            PllWrite7043A(0x014F, 0x00); // [7:0]LSB
            PllWrite7043A(0x0150, 0x00); // [3:0]MSB SCLKOUT13 
            //Pluse                    ;
            PllWrite7043A(0x00CD, 0x00); // [7:0]LSB
            PllWrite7043A(0x00CE, 0x00); // [3:0]MSB DCLKOUT0
            PllWrite7043A(0x00E1, 0x00); // [7:0]LSB
            PllWrite7043A(0x00E2, 0x00); // [3:0]MSB DCLKOUT2
            PllWrite7043A(0x00F5, 0x00); // [7:0]LSB
            PllWrite7043A(0x00F6, 0x00); // [3:0]MSB DCLKOUT4 
            PllWrite7043A(0x0131, 0x00); // [7:0]LSB
            PllWrite7043A(0x0132, 0x00); // [3:0]MSB DCLKOUT10
            //----------------------
            //     Output mux
            //------------------------
            PllWrite7043A(0x00CF, 0x01); // DCLKOUT0
            PllWrite7043A(0x00D9, 0x01); // SCLKOUT1
            PllWrite7043A(0x00E3, 0x01); // DCLKOUT2
            PllWrite7043A(0x00ED, 0x01); // SCLKOUT3
            PllWrite7043A(0x00F7, 0x01); // DCLKOUT4
            PllWrite7043A(0x0101, 0x01); // SCLKOUT5
            PllWrite7043A(0x010B, 0x01); // DCLKOUT6
            PllWrite7043A(0x0115, 0x01); // SCLKOUT7
            PllWrite7043A(0x011F, 0x01); // DCLKOUT8
            PllWrite7043A(0x0129, 0x01); // SCLKOUT9
            PllWrite7043A(0x0133, 0x01); // DCLKOUT10
            PllWrite7043A(0x013D, 0x01); // SCLKOUT11
            PllWrite7043A(0x0147, 0x01); // DCLKOUT12
            PllWrite7043A(0x0151, 0x01); // SCLKOUT13
            //----------------------
            //     输出驱动
            //------------------------
            PllWrite7043A(0x00D0, 0x10); // DCLKOUT0        //FPGA1 GT sysref  250M  
            PllWrite7043A(0x00DA, 0x10); // SCLKOUT1        //FPGA1 GT sysref  250M  
            PllWrite7043A(0x00E4, 0x89); // DCLKOUT2 
            PllWrite7043A(0x00EE, 0x89); // SCLKOUT3 
            PllWrite7043A(0x00F8, 0x89); // DCLKOUT4 
            PllWrite7043A(0x0102, 0x89); // SCLKOUT5 
            PllWrite7043A(0x010C, 0x81); // DCLKOUT6 
            PllWrite7043A(0x0116, 0x08); // SCLKOUT7
            PllWrite7043A(0x0120, 0x90); // DCLKOUT8        //FPGA1 204bsysref  900K 
            PllWrite7043A(0x012A, 0x08); // SCLKOUT9
            PllWrite7043A(0x0134, 0x90); // DCLKOUT10       //FPGA1 204bsysref  900K 
            PllWrite7043A(0x013E, 0x10); // SCLKOUT11       //FPGA1 GT sysref  250M//90 
            PllWrite7043A(0x0148, 0x10); // DCLKOUT12       //FPGA1 GT sysref  250M  
            PllWrite7043A(0x0152, 0x10); // SCLKOUT13       //FPGA1 GT sysref  250M 
            //----------------------
            //     重启分频器
            //------------------------
            PllWrite7043A(0001, 0x02);
            PllWrite7043A(0001, 0x00); // 重启分频器
        }
        #endregion



        #region 7043B
        private void PllWrite7043B(UInt32 ADDR, UInt32 data)//ADDR's width is 13bits; data's width is 8bits
        {
            UInt32 SDATA;//SDATA's width is 24bits
            UInt32 RorW = 0x0000000 & 0x00E00000;//MSB bit23 R/W bit22(W1) bit21(W0) Multibyte field 2'b00  RorW[23:21]=0|00
            SDATA = RorW | ((ADDR << 8) & 0x001fff00) | (data & 0x000000ff);

            //WriteToAllFpga(AcqBdReg.W.PllConfig_7043BWriteDataEffect, 0x00);
            //WriteToAllFpga(AcqBdReg.W.PllConfig_7043BWriteData_L16, SDATA & 0xffff);
            //WriteToAllFpga(AcqBdReg.W.PllConfig_7043BWriteData_H8, (SDATA >> 16) & 0xffff);
            //WriteToAllFpga(AcqBdReg.W.PllConfig_7043BWriteDataEffect, 0x01);
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(10);
        }//????




        private void InitACQ_HMC7043B()
        //    地址 , 值
        {
            //ACQ5200_HMC7044 start
            //WriteToAllFpga(AcqBdReg.W.PllConfig_7043BReset, 0);
            HdIO.Sleep(10);
            //WriteToAllFpga(AcqBdReg.W.PllConfig_7043BReset, 1);
            HdIO.Sleep(10);
            //WriteToAllFpga(AcqBdReg.W.PllConfig_7043BReset, 0);
            HdIO.Sleep(10);


            PllWrite7043B(0x0000, 0x01);
            PllWrite7043B(0x0000, 0x00);
            PllWrite7043B(0x0001, 0x00);
            PllWrite7043B(0x0002, 0x00);
            PllWrite7043B(0x0003, 0x34);
            PllWrite7043B(0x0004, 0x7F);
            PllWrite7043B(0x0006, 0x00);
            PllWrite7043B(0x0007, 0x00);
            PllWrite7043B(0x000A, 0x0D); //07
            PllWrite7043B(0x000B, 0x0D); //07
            PllWrite7043B(0x0046, 0x00);
            PllWrite7043B(0x0050, 0x28);
            PllWrite7043B(0x0050, 0x2F);
            PllWrite7043B(0x0050, 0x28);
            PllWrite7043B(0x0054, 0x03);
            PllWrite7043B(0x005A, 0x01); // PULSE NUM
            PllWrite7043B(0x005B, 0x04);
            PllWrite7043B(0x005C, 0x00); // PULSE CNT LSB
            PllWrite7043B(0x005D, 0x05); // PULSE CNT HSB //0200
            PllWrite7043B(0x0064, 0x00);//01：＜1GHz;00：＞1GHz
            PllWrite7043B(0x0065, 0x00);
            PllWrite7043B(0x0071, 0x10);
            PllWrite7043B(0x0078, 0x00);
            PllWrite7043B(0x0079, 0x00);
            PllWrite7043B(0x007A, 0x00);
            PllWrite7043B(0x007D, 0x00);
            PllWrite7043B(0x0091, 0x00);
            PllWrite7043B(0x0098, 0x00);
            PllWrite7043B(0x0099, 0x00);
            PllWrite7043B(0x009A, 0x00);
            PllWrite7043B(0x009B, 0xAA);
            PllWrite7043B(0x009C, 0xAA);
            PllWrite7043B(0x009D, 0xAA);
            PllWrite7043B(0x009E, 0xAA);
            PllWrite7043B(0x009F, 0x55);
            PllWrite7043B(0x00A0, 0x56);
            PllWrite7043B(0x00A1, 0x97);
            PllWrite7043B(0x00A2, 0x03);
            PllWrite7043B(0x00A3, 0x00);
            PllWrite7043B(0x00A4, 0x00);
            PllWrite7043B(0x00AD, 0x00);
            PllWrite7043B(0x00AE, 0x08);
            PllWrite7043B(0x00AF, 0x50);
            PllWrite7043B(0x00B0, 0x09);
            PllWrite7043B(0x00B1, 0x0D);
            PllWrite7043B(0x00B2, 0x00);
            PllWrite7043B(0x00B3, 0x00);
            PllWrite7043B(0x00B5, 0x00);
            PllWrite7043B(0x00B6, 0x00);
            PllWrite7043B(0x00B7, 0x00);
            PllWrite7043B(0x00B8, 0x00);
            //-------------------------;----------------------
            //                 通道设置;
            //-------------------------;-----------------
            //----------------------   ;
            //     Output 模式         ;
            //------------------------ ;
            PllWrite7043B(0x00C8, 0x00); // DCLKOUT0  
            PllWrite7043B(0x00D2, 0x00); // SCLKOUT1        //FPGA2 GT_TX_250M   //开启：F3;关闭：00
            PllWrite7043B(0x00DC, 0xF3); // DCLKOUT2        //FPGA2 204bsysref  900K  
            PllWrite7043B(0x00E6, 0x00); // SCLKOUT3        //FPGA2 204bsysref  900K  
            PllWrite7043B(0x00F0, 0xF3); // DCLKOUT4        //FPGA2 GT sysref  250M   
            PllWrite7043B(0x00FA, 0xF3); // SCLKOUT5        //FPGA2 GT sysref  250M   
            PllWrite7043B(0x0104, 0xF3); // DCLKOUT6        //FPGA2 GT sysref  250M   
            PllWrite7043B(0x010E, 0xF3); // SCLKOUT7        //FPGA2 GT sysref  250M 
            PllWrite7043B(0x0118, 0x00); // DCLKOUT8      
            PllWrite7043B(0x0122, 0x00); // SCLKOUT9
            PllWrite7043B(0x012C, 0x00); // DCLKOUT10     
            PllWrite7043B(0x0136, 0x00); // SCLKOUT11
            PllWrite7043B(0x0140, 0x00); // DCLKOUT12 
            PllWrite7043B(0x014A, 0x00); // SCLKOUT13
            //----------------------   ;
            //     Output 分频         ;
            //------------------------ ;
            //NC                       ;
            PllWrite7043B(0x00C9, 0xFA); // [7:0]LSB
            PllWrite7043B(0x00CA, 0x00); // [3:0]MSB DCLKOUT0 
            PllWrite7043B(0x00D3, 0x05); // [7:0]LSB
            PllWrite7043B(0x00D4, 0x00); // [3:0]MSB SCLKOUT1       //FPGA2 GT sysref  250M         
            PllWrite7043B(0x00DD, 0x80); // [7:0]LSB
            PllWrite7043B(0x00DE, 0x02); // [3:0]MSB DCLKOUT2       //FPGA2 204bsysref 900K//0280
            PllWrite7043B(0x00E7, 0x80); // [7:0]LSB
            PllWrite7043B(0x00E8, 0x02); // [3:0]MSB SCLKOUT3       //FPGA2 204bsysref 900K//0280
            PllWrite7043B(0x00F1, 0x05); // [7:0]LSB 
            PllWrite7043B(0x00F2, 0x00); // [3:0]MSB DCLKOUT4       //FPGA2 GT sysref  250M 
            PllWrite7043B(0x00FB, 0x05); // [7:0]LSB
            PllWrite7043B(0x00FC, 0x00); // [3:0]MSB SCLKOUT5       //FPGA2 GT sysref  250M   
            PllWrite7043B(0x0105, 0x05); // [7:0]LSB
            PllWrite7043B(0x0106, 0x00); // [3:0]MSB DCLKOUT6       //FPGA2 GT sysref  250M
            PllWrite7043B(0x010F, 0x05); // [7:0]LSB            
            PllWrite7043B(0x0110, 0x00); // [3:0]MSB SCLKOUT7       //FPGA2 GT sysref  250M 
            PllWrite7043B(0x0119, 0x7D); // [7:0]LSB
            PllWrite7043B(0x011A, 0x00); // [3:0]MSB DCLKOUT8     
            PllWrite7043B(0x0123, 0x00); // [7:0]LSB
            PllWrite7043B(0x0124, 0x00); // [3:0]MSB SCLKOUT9 
            PllWrite7043B(0x012D, 0x7D); // [7:0]LSB
            PllWrite7043B(0x012E, 0x00); // [3:0]MSB DCLKOUT10    
            PllWrite7043B(0x0137, 0xFA); // [7:0]LSB
            PllWrite7043B(0x0138, 0x00); // [3:0]MSB SCLKOUT11
            PllWrite7043B(0x0141, 0xFA); // [7:0]LSB
            PllWrite7043B(0x0142, 0x00); // [3:0]MSB DCLKOUT12
            PllWrite7043B(0x014B, 0x00); // [7:0]LSB
            PllWrite7043B(0x014C, 0x00); // [3:0]MSB SCLKOUT13
            //KEEP                     ;
            //Pluse                    ;
            //----------------------   ;
            //     模拟延迟            ;
            //------------------------ ;
            //NO USE                   ;
            PllWrite7043B(0x00D5, 0x00); // SCLKOUT1  
            PllWrite7043B(0x00E9, 0x00); // SCLKOUT3
            PllWrite7043B(0x00FD, 0x00); // SCLKOUT5
            PllWrite7043B(0x0107, 0x00); // DCLKOUT6
            PllWrite7043B(0x0111, 0x00); // SCLKOUT7
            PllWrite7043B(0x011B, 0x00); // DCLKOUT8
            PllWrite7043B(0x0125, 0x00); // SCLKOUT9
            PllWrite7043B(0x0143, 0x00); // DCLKOUT12
            //KEEP                     ;
            PllWrite7043B(0x0139, 0x00); // SCLKOUT11
            PllWrite7043B(0x014D, 0x00); // SCLKOUT13
            //Pluse                    ;
            PllWrite7043B(0x00CB, 0x00); // DCLKOUT0
            PllWrite7043B(0x00DF, 0x00); // DCLKOUT2
            PllWrite7043B(0x00F3, 0x00); // DCLKOUT4
            PllWrite7043B(0x012F, 0x00); // DCLKOUT10
            //----------------------   ;
            //     数字延迟            ;
            //------------------------ ;
            //NO USE                   ;
            PllWrite7043B(0x00D6, 0x00); // SCLKOUT1
            PllWrite7043B(0x00EA, 0x00); // SCLKOUT3
            PllWrite7043B(0x00FE, 0x00); // SCLKOUT5
            PllWrite7043B(0x0108, 0x00); // DCLKOUT6
            PllWrite7043B(0x0112, 0x00); // SCLKOUT7 
            PllWrite7043B(0x011C, 0x00); // DCLKOUT8
            PllWrite7043B(0x0126, 0x00); // SCLKOUT9
            PllWrite7043B(0x0144, 0x00); // DCLKOUT12
            //KEEP                     ;
            PllWrite7043B(0x013A, 0x00); // SCLKOUT11
            PllWrite7043B(0x014E, 0x00); // SCLKOUT13
            //Pluse                    ;
            PllWrite7043B(0x00CC, 0x00); // DCLKOUT0
            PllWrite7043B(0x00E0, 0x00); // DCLKOUT2
            PllWrite7043B(0x00F4, 0x00); // DCLKOUT4
            PllWrite7043B(0x0130, 0x00); // DCLKOUT10
            //----------------------   ;
            //     多跳数字延迟        ;
            //------------------------ ;
            //NO USE                   ;
            PllWrite7043B(0x00D7, 0x00); // [7:0]LSB
            PllWrite7043B(0x00D8, 0x00); // [3:0]MSB SCLKOUT1
            PllWrite7043B(0x00EB, 0x00); // [7:0]LSB
            PllWrite7043B(0x00EC, 0x00); // [3:0]MSB SCLKOUT3
            PllWrite7043B(0x00FF, 0x00); // [7:0]LSB
            PllWrite7043B(0x0100, 0x00); // [3:0]MSB SCLKOUT5
            PllWrite7043B(0x0109, 0x00); // [7:0]LSB
            PllWrite7043B(0x010A, 0x00); // [3:0]MSB DCLKOUT6  
            PllWrite7043B(0x0113, 0x00); // [7:0]LSB
            PllWrite7043B(0x0114, 0x00); // [3:0]MSB SCLKOUT7
            PllWrite7043B(0x011D, 0x00); // [7:0]LSB
            PllWrite7043B(0x011E, 0x00); // [3:0]MSB DCLKOUT8
            PllWrite7043B(0x0127, 0x00); // [7:0]LSB
            PllWrite7043B(0x0128, 0x00); // [3:0]MSB SCLKOUT9                 
            PllWrite7043B(0x0145, 0x00); // [7:0]LSB
            PllWrite7043B(0x0146, 0x00); // [3:0]MSB DCLKOUT12
            //KEEP                     ;
            PllWrite7043B(0x013B, 0x00); // [7:0]LSB
            PllWrite7043B(0x013C, 0x00); // [3:0]MSB SCLKOUT11    
            PllWrite7043B(0x014F, 0x00); // [7:0]LSB
            PllWrite7043B(0x0150, 0x00); // [3:0]MSB SCLKOUT13 
            //Pluse                    ;
            PllWrite7043B(0x00CD, 0x00); // [7:0]LSB
            PllWrite7043B(0x00CE, 0x00); // [3:0]MSB DCLKOUT0
            PllWrite7043B(0x00E1, 0x00); // [7:0]LSB
            PllWrite7043B(0x00E2, 0x00); // [3:0]MSB DCLKOUT2
            PllWrite7043B(0x00F5, 0x00); // [7:0]LSB
            PllWrite7043B(0x00F6, 0x00); // [3:0]MSB DCLKOUT4 
            PllWrite7043B(0x0131, 0x00); // [7:0]LSB
            PllWrite7043B(0x0132, 0x00); // [3:0]MSB DCLKOUT10
            //----------------------   ;
            //     Output mux          ;
            //------------------------ ;
            PllWrite7043B(0x00CF, 0x01); // DCLKOUT0
            PllWrite7043B(0x00D9, 0x01); // SCLKOUT1
            PllWrite7043B(0x00E3, 0x01); // DCLKOUT2
            PllWrite7043B(0x00ED, 0x01); // SCLKOUT3
            PllWrite7043B(0x00F7, 0x01); // DCLKOUT4
            PllWrite7043B(0x0101, 0x01); // SCLKOUT5
            PllWrite7043B(0x010B, 0x01); // DCLKOUT6
            PllWrite7043B(0x0115, 0x01); // SCLKOUT7
            PllWrite7043B(0x011F, 0x01); // DCLKOUT8
            PllWrite7043B(0x0129, 0x01); // SCLKOUT9
            PllWrite7043B(0x0133, 0x01); // DCLKOUT10
            PllWrite7043B(0x013D, 0x01); // SCLKOUT11
            PllWrite7043B(0x0147, 0x01); // DCLKOUT12
            PllWrite7043B(0x0151, 0x01); // SCLKOUT13
            //----------------------   ;
            //     输出驱动            ;
            //------------------------ ;
            PllWrite7043B(0x00D0, 0x89); // DCLKOUT0 
            PllWrite7043B(0x00DA, 0x10); // SCLKOUT1        //FPGA2 GT sysref  250M 
            PllWrite7043B(0x00E4, 0x90); // DCLKOUT2        //FPGA2 204bsysref  900K 
            PllWrite7043B(0x00EE, 0x90); // SCLKOUT3        //FPGA2 204bsysref  900K 
            PllWrite7043B(0x00F8, 0x10); // DCLKOUT4        //FPGA2 GT sysref  250M 
            PllWrite7043B(0x0102, 0x10); // SCLKOUT5        //FPGA2 GT sysref  250M 
            PllWrite7043B(0x010C, 0x10); // DCLKOUT6        //FPGA2 GT sysref  250M 
            PllWrite7043B(0x0116, 0x10); // SCLKOUT7        //FPGA2 GT sysref  250M//90
            PllWrite7043B(0x0120, 0x10); // DCLKOUT8      
            PllWrite7043B(0x012A, 0x08); // SCLKOUT9
            PllWrite7043B(0x0134, 0x10); // DCLKOUT10     
            PllWrite7043B(0x013E, 0x08); // SCLKOUT11
            PllWrite7043B(0x0148, 0x89); // DCLKOUT12 
            PllWrite7043B(0x0152, 0x89); // SCLKOUT13
            //----------------------
            //     重启分频器
            //------------------------
            PllWrite7043B(0001, 0x02);
            PllWrite7043B(0001, 0x00); // 重启分频器
        }//????
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
            HdIO.WaitForSpiTransfer(1, 4);
            HdIO.Sleep(10);
            WriteToAllFpga(AcqBdReg.W.PllConfig_LMX2595Data_Effect, second);
            HdIO.Sleep(10);
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
            //HdCommand.PCIX_WriteRegister32_Debug(AD9689_CONFIG_EN1, 0x00000000);


            /*    
               // 2572_OK

                 SendCmdToLMX2595(adcIndex, 0x00, 0x201E);
                 HdIO.Sleep(10);
                 SendCmdToLMX2595(adcIndex, 0x00, 0x241c);
                 SendCmdToLMX2595(adcIndex, 0x7D, 0x2288);
                 SendCmdToLMX2595(adcIndex, 0x7C, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x7B, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x7A, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x79, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x78, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x77, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x76, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x75, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x74, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x73, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x72, 0x7802);
                 SendCmdToLMX2595(adcIndex, 0x71, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x70, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x6F, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x6E, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x6D, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x6C, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x6B, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x6A, 0x0007);
                 SendCmdToLMX2595(adcIndex, 0x69, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x68, 0x061B);
                 SendCmdToLMX2595(adcIndex, 0x67, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x66, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x65, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x64, 0x061B);
                 SendCmdToLMX2595(adcIndex, 0x63 ,0x7B7F);
                 SendCmdToLMX2595(adcIndex, 0x62 ,0x0028);
                 SendCmdToLMX2595(adcIndex, 0x61 ,0x0000);
                 SendCmdToLMX2595(adcIndex, 0x60 ,0x0000);
                 SendCmdToLMX2595(adcIndex, 0x5F ,0x0000);
                 SendCmdToLMX2595(adcIndex, 0x5E ,0x0000);
                 SendCmdToLMX2595(adcIndex, 0x5D ,0x0000);
                 SendCmdToLMX2595(adcIndex, 0x5C ,0x0000);
                 SendCmdToLMX2595(adcIndex, 0x5B ,0x0000);
                 SendCmdToLMX2595(adcIndex, 0x5A ,0x0000);
                 SendCmdToLMX2595(adcIndex, 0x59 ,0x0000);
                 SendCmdToLMX2595(adcIndex, 0x58, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x57, 0x0800);
                 SendCmdToLMX2595(adcIndex, 0x56, 0x1000);
                 SendCmdToLMX2595(adcIndex, 0x55, 0x4000);
                 SendCmdToLMX2595(adcIndex, 0x54, 0x0001);
                 SendCmdToLMX2595(adcIndex, 0x53, 0xFFFF);
                 SendCmdToLMX2595(adcIndex, 0x52, 0xFFFF);
                 SendCmdToLMX2595(adcIndex, 0x51, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x50, 0x851E);
                 SendCmdToLMX2595(adcIndex, 0x4F, 0x01EB);
                 SendCmdToLMX2595(adcIndex, 0x4E, 0x0001);
                 SendCmdToLMX2595(adcIndex, 0x4D, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x4C, 0x000C);
                 SendCmdToLMX2595(adcIndex, 0x4B, 0x08C0);
                 SendCmdToLMX2595(adcIndex, 0x4A, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x49, 0x003F);
                 SendCmdToLMX2595(adcIndex, 0x48, 0x013E);//0001  //009E
                 SendCmdToLMX2595(adcIndex, 0x47, 0x0049);//0081 //8D
                 SendCmdToLMX2595(adcIndex, 0x46, 0xC350);
                 SendCmdToLMX2595(adcIndex, 0x45, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x44, 0x03E8);
                 SendCmdToLMX2595(adcIndex, 0x43, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x42, 0x01F4);
                 SendCmdToLMX2595(adcIndex, 0x41, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x40, 0x1388);
                 SendCmdToLMX2595(adcIndex, 0x3F, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x3E, 0x00AF);
                 SendCmdToLMX2595(adcIndex, 0x3D, 0x00A8);
                 SendCmdToLMX2595(adcIndex, 0x3C, 0x03E8);
                 SendCmdToLMX2595(adcIndex, 0x3B, 0x0001);
                 SendCmdToLMX2595(adcIndex, 0x3A, 0x9001);//9001
                 SendCmdToLMX2595(adcIndex, 0x39, 0x0020);
                 SendCmdToLMX2595(adcIndex, 0x38, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x37, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x36, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x35, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x34, 0x0421);
                 SendCmdToLMX2595(adcIndex, 0x33, 0x0080);
                 SendCmdToLMX2595(adcIndex, 0x32, 0x0080);
                 SendCmdToLMX2595(adcIndex, 0x31, 0x4180);
                 SendCmdToLMX2595(adcIndex, 0x30, 0x03E0);
                 SendCmdToLMX2595(adcIndex, 0x2F, 0x0300);
                 SendCmdToLMX2595(adcIndex, 0x2E, 0x07F2);//07F0
                 SendCmdToLMX2595(adcIndex, 0x2D, 0xCE1F);
                 SendCmdToLMX2595(adcIndex, 0x2C, 0x1F23);//1FA3
                 SendCmdToLMX2595(adcIndex, 0x2B, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x2A, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x29, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x28, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x27, 0x0001);
                 SendCmdToLMX2595(adcIndex, 0x26, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x25, 0x0305);
                 SendCmdToLMX2595(adcIndex, 0x24, 0x00A0);//0140
                 SendCmdToLMX2595(adcIndex, 0x23, 0x0004);
                 SendCmdToLMX2595(adcIndex, 0x22, 0x0010);
                 SendCmdToLMX2595(adcIndex, 0x21, 0x1E01);
                 SendCmdToLMX2595(adcIndex, 0x20, 0x05BF);
                 SendCmdToLMX2595(adcIndex, 0x1F, 0xC3E6);
                 SendCmdToLMX2595(adcIndex, 0x1E, 0x18A6);
                 SendCmdToLMX2595(adcIndex, 0x1D, 0x0000);
                 SendCmdToLMX2595(adcIndex, 0x1C, 0x0488);
                 SendCmdToLMX2595(adcIndex, 0x1B, 0x0002);
                 SendCmdToLMX2595(adcIndex, 0x1A, 0x0808);
                 SendCmdToLMX2595(adcIndex, 0x19, 0x0624);
                 SendCmdToLMX2595(adcIndex, 0x18, 0x071A);
                 SendCmdToLMX2595(adcIndex, 0x17, 0x007C);
                 SendCmdToLMX2595(adcIndex, 0x16, 0x0001);
                 SendCmdToLMX2595(adcIndex, 0x15, 0x0409);
                 SendCmdToLMX2595(adcIndex, 0x14, 0x7048);
                 SendCmdToLMX2595(adcIndex, 0x13, 0x27B7);
                 SendCmdToLMX2595(adcIndex, 0x12, 0x0064);
                 SendCmdToLMX2595(adcIndex, 0x11, 0x0096);
                 SendCmdToLMX2595(adcIndex, 0x10, 0x0080);
                 SendCmdToLMX2595(adcIndex, 0x0F, 0x060E);
                 SendCmdToLMX2595(adcIndex, 0x0E, 0x1870);
                 SendCmdToLMX2595(adcIndex, 0x0D, 0x4000);
                 SendCmdToLMX2595(adcIndex, 0x0C, 0x5001);
                 SendCmdToLMX2595(adcIndex, 0x0B, 0xB018);
                 SendCmdToLMX2595(adcIndex, 0x0A, 0x10F8);
                 SendCmdToLMX2595(adcIndex, 0x09, 0x0004);
                 SendCmdToLMX2595(adcIndex, 0x08, 0x2000);
                 SendCmdToLMX2595(adcIndex, 0x07, 0x00B2);
                 SendCmdToLMX2595(adcIndex, 0x06, 0xC802);
                 SendCmdToLMX2595(adcIndex, 0x05, 0x28C8);
                 SendCmdToLMX2595(adcIndex, 0x04, 0x0A43);
                 SendCmdToLMX2595(adcIndex, 0x03, 0x0782);
                 SendCmdToLMX2595(adcIndex, 0x02, 0x0500);
                 SendCmdToLMX2595(adcIndex, 0x01, 0x0808);
                 SendCmdToLMX2595(adcIndex, 0x00, 0x201C);
                 HdIO.Sleep(10);
                 SendCmdToLMX2595(adcIndex, 0x00, 0x601C);
    */
            /*                     
                                 //0704_2572正常
                                            //0630_链路建立正常
                                             SendCmdToLMX2595(adcIndex, 0x00, 0x201E);
                                             HdIO.Sleep(10);
                                             SendCmdToLMX2595(adcIndex, 0x00, 0x201C);
                                             SendCmdToLMX2595(adcIndex, 0x7D, 0x2288);
                                             SendCmdToLMX2595(adcIndex, 0x7C, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x7B, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x7A, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x79, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x78, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x77, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x76, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x75, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x74, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x73, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x72, 0x7802);
                                             SendCmdToLMX2595(adcIndex, 0x71, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x70, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x6F, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x6E, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x6D, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x6C, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x6B, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x6A, 0x0007);
                                             SendCmdToLMX2595(adcIndex, 0x69, 0x0000);//0000
                                             SendCmdToLMX2595(adcIndex, 0x68, 0x061B);
                                             SendCmdToLMX2595(adcIndex, 0x67, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x66, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x65, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x64, 0x061B);
                                             SendCmdToLMX2595(adcIndex, 0x63, 0x7B7F);
                                             SendCmdToLMX2595(adcIndex, 0x62, 0x0028);
                                             SendCmdToLMX2595(adcIndex, 0x61, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x60, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x5F, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x5E, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x5D, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x5C, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x5B, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x5A, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x59, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x58, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x57, 0x0800);
                                             SendCmdToLMX2595(adcIndex, 0x56, 0x1000);
                                             SendCmdToLMX2595(adcIndex, 0x55, 0x4000);
                                             SendCmdToLMX2595(adcIndex, 0x54, 0x0001);
                                             SendCmdToLMX2595(adcIndex, 0x53, 0xFFFF);
                                             SendCmdToLMX2595(adcIndex, 0x52, 0xFFFF);
                                             SendCmdToLMX2595(adcIndex, 0x51, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x50, 0x851E);
                                             SendCmdToLMX2595(adcIndex, 0x4F, 0x01EB);
                                             SendCmdToLMX2595(adcIndex, 0x4E, 0x0001);//0001
                                             SendCmdToLMX2595(adcIndex, 0x4D, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x4C, 0x000C);
                                             SendCmdToLMX2595(adcIndex, 0x4B, 0x08C0);
                                             SendCmdToLMX2595(adcIndex, 0x4A, 0x0000);//0000 
                                             SendCmdToLMX2595(adcIndex, 0x49, 0x003F);
                                             SendCmdToLMX2595(adcIndex, 0x48, 0x013E);
                                             SendCmdToLMX2595(adcIndex, 0x47, 0x004D);//0049//
                                             SendCmdToLMX2595(adcIndex, 0x46, 0xC350);
                                             SendCmdToLMX2595(adcIndex, 0x45, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x44, 0x03E8);
                                             SendCmdToLMX2595(adcIndex, 0x43, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x42, 0x01F4);
                                             SendCmdToLMX2595(adcIndex, 0x41, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x40, 0x1388);
                                             SendCmdToLMX2595(adcIndex, 0x3F, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x3E, 0x00AF);
                                             SendCmdToLMX2595(adcIndex, 0x3D, 0x00A8);
                                             SendCmdToLMX2595(adcIndex, 0x3C, 0x03E8);
                                             SendCmdToLMX2595(adcIndex, 0x3B, 0x0001);
                                             SendCmdToLMX2595(adcIndex, 0x3A, 0x1601);
                                             SendCmdToLMX2595(adcIndex, 0x39, 0x0020);
                                             SendCmdToLMX2595(adcIndex, 0x38, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x37, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x36, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x35, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x34, 0x0421);
                                             SendCmdToLMX2595(adcIndex, 0x33, 0x0080);
                                             SendCmdToLMX2595(adcIndex, 0x32, 0x0080);
                                             SendCmdToLMX2595(adcIndex, 0x31, 0x4180);
                                             SendCmdToLMX2595(adcIndex, 0x30, 0x03E0);
                                             SendCmdToLMX2595(adcIndex, 0x2F, 0x0300);
                                             SendCmdToLMX2595(adcIndex, 0x2E, 0x07F2);
                                             SendCmdToLMX2595(adcIndex, 0x2D, 0xCE30);//CE1E
                                             SendCmdToLMX2595(adcIndex, 0x2C, 0x1e23);//1E23
                                             SendCmdToLMX2595(adcIndex, 0x2B, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x2A, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x29, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x28, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x27, 0x0001);
                                             SendCmdToLMX2595(adcIndex, 0x26, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x25, 0x0305);//0305
                                             SendCmdToLMX2595(adcIndex, 0x24, 0x00A0);
                                             SendCmdToLMX2595(adcIndex, 0x23, 0x0004);
                                             SendCmdToLMX2595(adcIndex, 0x22, 0x0010);
                                             SendCmdToLMX2595(adcIndex, 0x21, 0x1E01);
                                             SendCmdToLMX2595(adcIndex, 0x20, 0x05BF);
                                             SendCmdToLMX2595(adcIndex, 0x1F, 0xC3E6);
                                             SendCmdToLMX2595(adcIndex, 0x1E, 0x18A6);
                                             SendCmdToLMX2595(adcIndex, 0x1D, 0x0000);
                                             SendCmdToLMX2595(adcIndex, 0x1C, 0x0488);
                                             SendCmdToLMX2595(adcIndex, 0x1B, 0x0002);
                                             SendCmdToLMX2595(adcIndex, 0x1A, 0x0808);
                                             SendCmdToLMX2595(adcIndex, 0x19, 0x0624);
                                             SendCmdToLMX2595(adcIndex, 0x18, 0x071A);
                                             SendCmdToLMX2595(adcIndex, 0x17, 0x007C);
                                             SendCmdToLMX2595(adcIndex, 0x16, 0x0001);
                                             SendCmdToLMX2595(adcIndex, 0x15, 0x0409);
                                             SendCmdToLMX2595(adcIndex, 0x14, 0x6048);
                                             SendCmdToLMX2595(adcIndex, 0x13, 0x27B7);
                                             SendCmdToLMX2595(adcIndex, 0x12, 0x0064);
                                             SendCmdToLMX2595(adcIndex, 0x11, 0x0096);//0096
                                             SendCmdToLMX2595(adcIndex, 0x10, 0x0080);
                                             SendCmdToLMX2595(adcIndex, 0x0F, 0x060E);
                                             SendCmdToLMX2595(adcIndex, 0x0E, 0x1810);//1810
                                             SendCmdToLMX2595(adcIndex, 0x0D, 0x4000);
                                             SendCmdToLMX2595(adcIndex, 0x0C, 0x5001);
                                             SendCmdToLMX2595(adcIndex, 0x0B, 0xB018);
                                             SendCmdToLMX2595(adcIndex, 0x0A, 0x10F8);
                                             SendCmdToLMX2595(adcIndex, 0x09, 0x0004);
                                             SendCmdToLMX2595(adcIndex, 0x08, 0x2000);
                                             SendCmdToLMX2595(adcIndex, 0x07, 0x00B2);//00B2
                                             SendCmdToLMX2595(adcIndex, 0x06, 0xC802);
                                             SendCmdToLMX2595(adcIndex, 0x05, 0x28C8);
                                             SendCmdToLMX2595(adcIndex, 0x04, 0x0A43);
                                             SendCmdToLMX2595(adcIndex, 0x03, 0x0782);
                                             SendCmdToLMX2595(adcIndex, 0x02, 0x0500);
                                             SendCmdToLMX2595(adcIndex, 0x01, 0x0808);
                                             SendCmdToLMX2595(adcIndex, 0x00, 0x601C);
                                             HdIO.Sleep(5);
                                             SendCmdToLMX2595(adcIndex, 0x00, 0x601C);
               */
            /*
                        SendCmdToLMX2595(adcIndex, 0x00, 0x201E);
                        HdIO.Sleep(10);
                        SendCmdToLMX2595(adcIndex, 0x00, 0x201C);
                        SendCmdToLMX2595(adcIndex, 0x7D, 0x2288);
                        SendCmdToLMX2595(adcIndex, 0x7C, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x7B, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x7A, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x79, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x78, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x77, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x76, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x75, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x74, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x73, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x72, 0x7802);
                        SendCmdToLMX2595(adcIndex, 0x71, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x70, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x6F, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x6E, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x6D, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x6C, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x6B, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x6A, 0x0007);
                        SendCmdToLMX2595(adcIndex, 0x69, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x68, 0x061B);
                        SendCmdToLMX2595(adcIndex, 0x67, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x66, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x65, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x64, 0x061B);
                        SendCmdToLMX2595(adcIndex, 0x63, 0x7B7F);
                        SendCmdToLMX2595(adcIndex, 0x62, 0x0028);
                        SendCmdToLMX2595(adcIndex, 0x61, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x60, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x5F, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x5E, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x5D, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x5C, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x5B, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x5A, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x59, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x58, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x57, 0x0800);
                        SendCmdToLMX2595(adcIndex, 0x56, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x55, 0x4000);
                        SendCmdToLMX2595(adcIndex, 0x54, 0x0001);
                        SendCmdToLMX2595(adcIndex, 0x53, 0xFFFF);
                        SendCmdToLMX2595(adcIndex, 0x52, 0xFFFF);
                        SendCmdToLMX2595(adcIndex, 0x51, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x50, 0x851E);
                        SendCmdToLMX2595(adcIndex, 0x4F, 0x01EB);
                        SendCmdToLMX2595(adcIndex, 0x4E, 0x0083);
                        SendCmdToLMX2595(adcIndex, 0x4D, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x4C, 0x000C);
                        SendCmdToLMX2595(adcIndex, 0x4B, 0x0B80);
                        SendCmdToLMX2595(adcIndex, 0x4A, 0x1000);
                        SendCmdToLMX2595(adcIndex, 0x49, 0x06E4);
                        SendCmdToLMX2595(adcIndex, 0x48, 0x013E);
                        SendCmdToLMX2595(adcIndex, 0x47, 0x004D);
                        SendCmdToLMX2595(adcIndex, 0x46, 0xEA60);
                        SendCmdToLMX2595(adcIndex, 0x45, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x44, 0x03E8);
                        SendCmdToLMX2595(adcIndex, 0x43, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x42, 0x01F4);
                        SendCmdToLMX2595(adcIndex, 0x41, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x40, 0x1388);
                        SendCmdToLMX2595(adcIndex, 0x3F, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x3E, 0x00AF);
                        SendCmdToLMX2595(adcIndex, 0x3D, 0x00A8);
                        SendCmdToLMX2595(adcIndex, 0x3C, 0x03E8);
                        SendCmdToLMX2595(adcIndex, 0x3B, 0x0001);
                        SendCmdToLMX2595(adcIndex, 0x3A, 0x5601);
                        SendCmdToLMX2595(adcIndex, 0x39, 0x0020);
                        SendCmdToLMX2595(adcIndex, 0x38, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x37, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x36, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x35, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x34, 0x0421);
                        SendCmdToLMX2595(adcIndex, 0x33, 0x0080);
                        SendCmdToLMX2595(adcIndex, 0x32, 0x0080);
                        SendCmdToLMX2595(adcIndex, 0x31, 0x4180);
                        SendCmdToLMX2595(adcIndex, 0x30, 0x03E0);
                        SendCmdToLMX2595(adcIndex, 0x2F, 0x0300);
                        SendCmdToLMX2595(adcIndex, 0x2E, 0x07F2);
                        SendCmdToLMX2595(adcIndex, 0x2D, 0xCE1F);
                        SendCmdToLMX2595(adcIndex, 0x2C, 0x1F02);
                        SendCmdToLMX2595(adcIndex, 0x2B, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x2A, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x29, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x28, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x27, 0x0001);
                        SendCmdToLMX2595(adcIndex, 0x26, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x25, 0x0205);
                        SendCmdToLMX2595(adcIndex, 0x24, 0x00A0);
                        SendCmdToLMX2595(adcIndex, 0x23, 0x0004);
                        SendCmdToLMX2595(adcIndex, 0x22, 0x0010);
                        SendCmdToLMX2595(adcIndex, 0x21, 0x1E01);
                        SendCmdToLMX2595(adcIndex, 0x20, 0x05BF);
                        SendCmdToLMX2595(adcIndex, 0x1F, 0xC3E6);
                        SendCmdToLMX2595(adcIndex, 0x1E, 0x18A6);
                        SendCmdToLMX2595(adcIndex, 0x1D, 0x0000);
                        SendCmdToLMX2595(adcIndex, 0x1C, 0x0488);
                        SendCmdToLMX2595(adcIndex, 0x1B, 0x0002);
                        SendCmdToLMX2595(adcIndex, 0x1A, 0x0808);
                        SendCmdToLMX2595(adcIndex, 0x19, 0x0624);
                        SendCmdToLMX2595(adcIndex, 0x18, 0x071A);
                        SendCmdToLMX2595(adcIndex, 0x17, 0x007C);
                        SendCmdToLMX2595(adcIndex, 0x16, 0x0001);
                        SendCmdToLMX2595(adcIndex, 0x15, 0x0409);
                        SendCmdToLMX2595(adcIndex, 0x14, 0x6048);
                        SendCmdToLMX2595(adcIndex, 0x13, 0x27B7);
                        SendCmdToLMX2595(adcIndex, 0x12, 0x0064);
                        SendCmdToLMX2595(adcIndex, 0x11, 0x00B4);
                        SendCmdToLMX2595(adcIndex, 0x10, 0x0080);
                        SendCmdToLMX2595(adcIndex, 0x0F, 0x060E);
                        SendCmdToLMX2595(adcIndex, 0x0E, 0x1810);
                        SendCmdToLMX2595(adcIndex, 0x0D, 0x4000);
                        SendCmdToLMX2595(adcIndex, 0x0C, 0x5001);
                        SendCmdToLMX2595(adcIndex, 0x0B, 0xB018);
                        SendCmdToLMX2595(adcIndex, 0x0A, 0x10F8);
                        SendCmdToLMX2595(adcIndex, 0x09, 0x0004);
                        SendCmdToLMX2595(adcIndex, 0x08, 0x2000);
                        SendCmdToLMX2595(adcIndex, 0x07, 0x40B2);
                        SendCmdToLMX2595(adcIndex, 0x06, 0xC802);
                        SendCmdToLMX2595(adcIndex, 0x05, 0x20C8);
                        SendCmdToLMX2595(adcIndex, 0x04, 0x0A43);
                        SendCmdToLMX2595(adcIndex, 0x03, 0x0782);
                        SendCmdToLMX2595(adcIndex, 0x02, 0x0500);
                        SendCmdToLMX2595(adcIndex, 0x01, 0x0808);
                        SendCmdToLMX2595(adcIndex, 0x00, 0x601C);
                        HdIO.Sleep(5);
                        SendCmdToLMX2595(adcIndex, 0x00, 0x601C);
            */

            /*
            SendCmdToLMX2595(adcIndex, 0x00, 0x201E);
            HdIO.Sleep(10);
            SendCmdToLMX2595(adcIndex, 0x00, 0x201C);
            SendCmdToLMX2595(adcIndex, 0x7D, 0x2288);
            SendCmdToLMX2595(adcIndex, 0x7C, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x7B, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x7A, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x79, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x78, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x77, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x76, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x75, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x74, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x73, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x72, 0x7802);
            SendCmdToLMX2595(adcIndex, 0x71, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x70, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6F, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6E, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6D, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6C, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6B, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x6A, 0x0007);
            SendCmdToLMX2595(adcIndex, 0x69, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x68, 0x061B);
            SendCmdToLMX2595(adcIndex, 0x67, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x66, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x65, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x64, 0x061B);
            SendCmdToLMX2595(adcIndex, 0x63, 0x7B7F);
            SendCmdToLMX2595(adcIndex, 0x62, 0x0028);
            SendCmdToLMX2595(adcIndex, 0x61, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x60, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5F, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5E, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5D, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5C, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5B, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x5A, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x59, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x58, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x57, 0x0800);
            SendCmdToLMX2595(adcIndex, 0x56, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x55, 0x4000);
            SendCmdToLMX2595(adcIndex, 0x54, 0x0001);
            SendCmdToLMX2595(adcIndex, 0x53, 0xFFFF);
            SendCmdToLMX2595(adcIndex, 0x52, 0xFFFF);
            SendCmdToLMX2595(adcIndex, 0x51, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x50, 0x851E);
            SendCmdToLMX2595(adcIndex, 0x4F, 0x01EB);
            SendCmdToLMX2595(adcIndex, 0x4E, 0x0083);
            SendCmdToLMX2595(adcIndex, 0x4D, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x4C, 0x000C);
            SendCmdToLMX2595(adcIndex, 0x4B, 0x0B80);
            SendCmdToLMX2595(adcIndex, 0x4A, 0x1000);
            SendCmdToLMX2595(adcIndex, 0x49, 0x06E4);
            SendCmdToLMX2595(adcIndex, 0x48, 0x004E);
            SendCmdToLMX2595(adcIndex, 0x47, 0x004D);
            SendCmdToLMX2595(adcIndex, 0x46, 0xEA60);
            SendCmdToLMX2595(adcIndex, 0x45, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x44, 0x03E8);
            SendCmdToLMX2595(adcIndex, 0x43, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x42, 0x01F4);
            SendCmdToLMX2595(adcIndex, 0x41, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x40, 0x1388);
            SendCmdToLMX2595(adcIndex, 0x3F, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x3E, 0x00AF);
            SendCmdToLMX2595(adcIndex, 0x3D, 0x00A8);
            SendCmdToLMX2595(adcIndex, 0x3C, 0x03E8);
            SendCmdToLMX2595(adcIndex, 0x3B, 0x0001);
            SendCmdToLMX2595(adcIndex, 0x3A, 0x5601);
            SendCmdToLMX2595(adcIndex, 0x39, 0x0020);
            SendCmdToLMX2595(adcIndex, 0x38, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x37, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x36, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x35, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x34, 0x0421);
            SendCmdToLMX2595(adcIndex, 0x33, 0x0080);
            SendCmdToLMX2595(adcIndex, 0x32, 0x0080);
            SendCmdToLMX2595(adcIndex, 0x31, 0x4180);
            SendCmdToLMX2595(adcIndex, 0x30, 0x03E0);
            SendCmdToLMX2595(adcIndex, 0x2F, 0x0300);
            SendCmdToLMX2595(adcIndex, 0x2E, 0x07F2);
            SendCmdToLMX2595(adcIndex, 0x2D, 0xCE1E);
            SendCmdToLMX2595(adcIndex, 0x2C, 0x1F02);
            SendCmdToLMX2595(adcIndex, 0x2B, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x2A, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x29, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x28, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x27, 0x0001);
            SendCmdToLMX2595(adcIndex, 0x26, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x25, 0x0205);
            SendCmdToLMX2595(adcIndex, 0x24, 0x00A0);
            SendCmdToLMX2595(adcIndex, 0x23, 0x0004);
            SendCmdToLMX2595(adcIndex, 0x22, 0x0010);
            SendCmdToLMX2595(adcIndex, 0x21, 0x1E01);
            SendCmdToLMX2595(adcIndex, 0x20, 0x05BF);
            SendCmdToLMX2595(adcIndex, 0x1F, 0xC3E6);
            SendCmdToLMX2595(adcIndex, 0x1E, 0x18A6);
            SendCmdToLMX2595(adcIndex, 0x1D, 0x0000);
            SendCmdToLMX2595(adcIndex, 0x1C, 0x0488);
            SendCmdToLMX2595(adcIndex, 0x1B, 0x0002);
            SendCmdToLMX2595(adcIndex, 0x1A, 0x0808);
            SendCmdToLMX2595(adcIndex, 0x19, 0x0624);
            SendCmdToLMX2595(adcIndex, 0x18, 0x071A);
            SendCmdToLMX2595(adcIndex, 0x17, 0x007C);
            SendCmdToLMX2595(adcIndex, 0x16, 0x0001);
            SendCmdToLMX2595(adcIndex, 0x15, 0x0409);
            SendCmdToLMX2595(adcIndex, 0x14, 0x6048);
            SendCmdToLMX2595(adcIndex, 0x13, 0x27B7);
            SendCmdToLMX2595(adcIndex, 0x12, 0x0064);
            SendCmdToLMX2595(adcIndex, 0x11, 0x00B4);
            SendCmdToLMX2595(adcIndex, 0x10, 0x0080);
            SendCmdToLMX2595(adcIndex, 0x0F, 0x060E);
            SendCmdToLMX2595(adcIndex, 0x0E, 0x1810);
            SendCmdToLMX2595(adcIndex, 0x0D, 0x4000);
            SendCmdToLMX2595(adcIndex, 0x0C, 0x5001);
            SendCmdToLMX2595(adcIndex, 0x0B, 0xB018);
            SendCmdToLMX2595(adcIndex, 0x0A, 0x10F8);
            SendCmdToLMX2595(adcIndex, 0x09, 0x0004);
            SendCmdToLMX2595(adcIndex, 0x08, 0x2000);
            SendCmdToLMX2595(adcIndex, 0x07, 0x40B2);
            SendCmdToLMX2595(adcIndex, 0x06, 0xC802);
            SendCmdToLMX2595(adcIndex, 0x05, 0x20C8);
            SendCmdToLMX2595(adcIndex, 0x04, 0x0A43);
            SendCmdToLMX2595(adcIndex, 0x03, 0x0782);
            SendCmdToLMX2595(adcIndex, 0x02, 0x0500);
            SendCmdToLMX2595(adcIndex, 0x01, 0x0808);
            SendCmdToLMX2595(adcIndex, 0x00, 0x601C);
            HdIO.Sleep(5);
            SendCmdToLMX2595(adcIndex, 0x00, 0x601C);
*/
        }
        #endregion

        #region JESD204B链路建立与多片同步
        private void JESD204B_RST(AcqBdNo fpgaIndex)//????
        {
            //HdCommand.PCIX_WriteRegister32(JESD204B_CORE_RST | CTRL_INDEPENT_REG[index], 0x0002);
            HdIO.Sleep(10);
            WriteReg(AcqBdReg.W.Adc_204bConfig_Reset, fpgaIndex, 0x03);
            //HdIO.WriteReg(ProcBdReg.W.ADC_204bReset, 0x1);
            HdIO.Sleep(50);
            WriteReg(AcqBdReg.W.Adc_204bConfig_Reset, fpgaIndex, 0x02);
            //HdIO.WriteReg(ProcBdReg.W.ADC_204bReset, 0x0);
        }
        private void multi_sync_rst(AcqBdNo fpgaIndex)//????
        {
            WriteReg(AcqBdReg.W.Adc_204bConfig_Reset, fpgaIndex, 0x0000);
            //HdIO.WriteReg(ProcBdReg.W.ADC_204bReset, 0x0);
            HdIO.Sleep(10);
            WriteReg(AcqBdReg.W.Adc_204bConfig_Reset, fpgaIndex, 0x0002);
            //HdIO.WriteReg(ProcBdReg.W.ADC_204bReset, 0x1);
            HdIO.Sleep(50);
            WriteReg(AcqBdReg.W.Adc_204bConfig_Reset, fpgaIndex, 0x0000);
            //HdIO.WriteReg(ProcBdReg.W.ADC_204bReset, 0x0);
        }
        #endregion
        record NeedInitAD5200Config(Int32 ChannelID, int SubbandIndex, AcqBdNo FpgaIndex, int AdcIndex, bool bPhase, bool bGian, bool bReCalc0x29Register);
        private void BuildAdc5200LinkRoad(List<NeedInitAD5200Config> needInitAD5200Configs)//????
        {
            //foreach (NeedInitAD5200Config configParam in needInitAD5200Configs)
            //    AD5200_Init(configParam.ChannelID, configParam.FpgaIndex, configParam.SubbandIndex, configParam.AdcIndex, configParam.bReCalc0x29Register);//包含发送校准数据

            //HdIO.Sleep(500);

            //#region JESD204B_RST
            ////JESD204B_RST
            //HdIO.Sleep(10);
            //WriteToAllFpga(AcqBdReg.W.Adc_204bConfig_Reset, 0x03);
            ////HdIO.WriteReg(ProcBdReg.W.ADC_204bReset, 0x1);
            ////HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_0, 0x03);
            //HdIO.Sleep(50);
            //WriteToAllFpga(AcqBdReg.W.Adc_204bConfig_Reset, 0x02);
            ////HdIO.WriteReg(ProcBdReg.W.ADC_204bReset, 0x0);
            ////HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_0, 0x00);
            //HdIO.Sleep(500);

            ////lchy -+-
            //// HdIO.WriteReg(ProcBdReg.W.DCM_CTRL_Reset, 0xFF);
            //HdIO.Sleep(50);
            //// HdIO.WriteReg(ProcBdReg.W.DCM_CTRL_Reset, 0x0);
            //HdIO.Sleep(50);
            ////lchy -+-

            //#endregion
            ////send sysref pulse to FPGA 
            ////Hd.CurrProduct?.S6Bd?.PllSync();
            //Adc5200WaitForAutoCaliFinished.Default.ExecWaitForFinished(true, 0x01);
            foreach (NeedInitAD5200Config configParam in needInitAD5200Configs)
            {
                AD5200_Init(configParam.ChannelID, configParam.FpgaIndex, configParam.SubbandIndex, configParam.AdcIndex, 1);//包含发送校准数据
                AD5200_Init(configParam.ChannelID, configParam.FpgaIndex, configParam.SubbandIndex, configParam.AdcIndex, 2);//包含发送校准数据
            }

            HdIO.Sleep(1000);

            #region JESD204B_RST
            //JESD204B_RST Build Finish
            WriteToAllFpga(AcqBdReg.W.Adc_204bConfig_Reset, 0x01);
            HdIO.Sleep(1000);
            WriteToAllFpga(AcqBdReg.W.Adc_204bConfig_Reset, 0x00);
            HdIO.Sleep(1000);
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
        private void InitAll5200At20GMode()//????
        {
            //WriteToAllFpga(AcqBdReg.W.PllConfig_7044Reset, 0x00);
            //HdIO.Sleep(100);
            ////WriteToAllFpga(AcqBdReg.W.DBI_LO_RST, 0x00);
            //HdIO.WriteReg(ProcBdReg.W.DBI_LO_RSTProCh1, 0x00);
            //HdIO.WriteReg(ProcBdReg.W.DBI_LO_RSTProCh2, 0x00);
            //HdIO.WriteReg(ProcBdReg.W.DBI_LO_RSTProCh3, 0x00);
            //HdIO.WriteReg(ProcBdReg.W.DBI_LO_RSTProCh4, 0x00);
            //HdIO.Sleep(100);
            //WriteToAllFpga(AcqBdReg.W.FPGAFlashUpdater_SS, 0x00);
            //HdIO.Sleep(100);
            //WriteToAllFpga(AcqBdReg.W.Adc_204bConfig_Reset, 0x01);
            ////HdIO.WriteReg(ProcBdReg.W.ADC_204bReset, 0x1);
            ////HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_0, 0x03);
            //HdIO.Sleep(500);
            ////lchy -
            ////HdIO.WriteReg(ProcBdReg.W.DCM_CTRL_Reset, 0xFF);
            //HdIO.Sleep(1);
            ////lchy -
            //WriteToAllFpga(AcqBdReg.W.Adc_PowerCtrl, 0x03);
            //HdIO.Sleep(1);
            //WriteToAllFpga(AcqBdReg.W.Adc_ConfigEnable, 0x00);
            //HdIO.Sleep(100);
            //WriteToAllFpga(AcqBdReg.W.Adc_PowerCtrl, 0x00);

            //HdIO.Sleep(20);

            //InitHMC7044_Adc5200();//00
            //HdIO.Sleep(200);
            ////ACQ7044 sync 
            //HdCtrl_Pll.PllSync();//*****01
            //HdIO.Sleep(200); //100

            //WriteToAllFpga(AcqBdReg.W.PllConfig_LMK00304Config1, 0x09);//0X09
            //                                                           //            WriteToAllFpga(AcqBdReg.W.PllConfig_LMK00304Config2, 0x01);
            //HdIO.Sleep(100);
            //InitACQ_HMC7043A();//***02
            //HdIO.Sleep(20);
            //InitACQ_HMC7043B();//***03
            //HdIO.Sleep(1000); //500
            //                  //          Hd.CurrProduct?.S6Bd?.PllSync();
            //                  //          HdIO.Sleep(20);
            //                  //LMX2595 init
            //for (int adcIndex = 0; adcIndex < Constants.ADC_NUM; adcIndex++)
            //    LMX2595_Init(adcIndex);// //////////// need check
            //HdIO.Sleep(1000); //500

            //Int32 cnt = 5;
            //int adc_valid;
            //do
            //{
            //    cnt--;
            //    adc_valid = 1;
            //    HMC7044Write(0x010E, 0xB3); // SCLKOUT7     250M-15.625M ,作为0304A的输入钟 
            //    HMC7044Write(0x0122, 0xB3); // SCLKOUT9     0304B的输入钟 

            //    HMC7044Write(0x00D2, 0xB3); // SCLKOUT1     7043a 输入时钟 1.25G 
            //    HMC7044Write(0x0140, 0xB3); // DCLKOUT12    7043b 输入时钟 1.25G
            //    HdIO.Sleep(100); //200

            //    HdCtrl_Pll.PllSync();//***04
            //    HdIO.Sleep(500);

            //    //全部需要初始化,但不需要重新校准0x29寄存器
            //    List<NeedInitAD5200Config> needInitAD5200Config = new List<NeedInitAD5200Config>();
            //    //此时还没有通道和幅度档等信息。全部的5200需要初始化
            //    //需要发送ADC的Phase（与通道组合无关）、Gain（与通道组合有关）、同步压稳态窗（与通道组合无关）
            //    int subbandIndex = 0;
            //    foreach (var acqbdno in Enum.GetValues<AcqBdNo>())
            //    {
            //        if (ExistsAcqBdDefine.Contains(acqbdno))
            //        {
            //            needInitAD5200Config.Add(new NeedInitAD5200Config(0, subbandIndex, acqbdno, 0, true, false, false));//ADC1
            //            needInitAD5200Config.Add(new NeedInitAD5200Config(0, subbandIndex, acqbdno, 1, true, false, false));//ADC2
            //            subbandIndex++;
            //        }
            //    }

            //    BuildAdc5200LinkRoad(needInitAD5200Config);//在此项目中，ADC相位、同步稳态窗的设置在此函数中进行，而Gain与通道有关，不设置。

            //    HdIO.Sleep(500);

            //    foreach (var acqBdNo in Enum.GetValues<AcqBdNo>())
            //    {
            //        if (Hd.CurrProduct!.AcqBd!.ExistsAcqBdDefine.Contains(acqBdNo))
            //        {
            //            for (int readTime = 0; readTime < 5; readTime++)
            //            {
            //                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Adc_MonitorAdcValid, acqBdNo, 0);
            //                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Adc_MonitorAdcValid, acqBdNo, 1);
            //                HdIO.Sleep(30);
            //                var AdcLinkStatusFlag = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.Adc_ReadDataValid, acqBdNo);
            //                if (AdcLinkStatusFlag != 0x0300)
            //                {
            //                    adc_valid = 0;
            //                    break;
            //                }
            //            }
            //        }
            //        if (adc_valid == 0) break;
            //    }
            //} while (adc_valid == 0 && cnt >= 0);
            ////WriteToAllFpga(AcqBdReg.W.Data_To_PRO_CTRL_CLK_RESET, 0x01);
            ////WriteToAllFpga(AcqBdReg.W.Data_To_PRO_CTRL_IO_RESET, 0x01);

            //HdIO.Sleep(100);
            ////WriteToAllFpga(AcqBdReg.W.Data_To_PRO_CTRL_CLK_RESET, 0x00);
            ////WriteToAllFpga(AcqBdReg.W.Data_To_PRO_CTRL_IO_RESET, 0x00);
            ///


            //8000 

            //HdIO.Sleep(30000);
            WriteToAllFpga(AcqBdReg.W.PllConfig_7044Reset, 0x00);
            HdIO.Sleep(10);
            HdIO.WriteReg(ProcBdReg.W.DBI_LO_RSTProCh1, 0x00);
            HdIO.WriteReg(ProcBdReg.W.DBI_LO_RSTProCh2, 0x00);
            HdIO.WriteReg(ProcBdReg.W.DBI_LO_RSTProCh3, 0x00);
            HdIO.WriteReg(ProcBdReg.W.DBI_LO_RSTProCh4, 0x00);
            HdIO.Sleep(100);
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
        private bool CtrlGainByFpga()//????
        {
            int currChBWModeAndActiveState = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.CurrChBWModeAndActiveState ?? 0;
            if (!Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode)
            {
                int watchChannelID = (int)Hd.CurrDebugVarints.iDbi_DebugChannelID;
                int FullBandWidth_Is0 = 0;// (Hd.CurrProduct?.AnalogAcquireModel?.GetDbiMergeState(Acquirer_AnalogChanel_DBI13G.DbiActuallActiveState) ?? false) ? 0 : 1;
                int yScaleIndex = Hd.UIMessage?.Analog?[(int)watchChannelID].ScaleIndex ?? 5;
                List<FPGAIndex_AnalogSubbandAbsoluteIndex> list = GetFPGAIndex_SubbandIndex(FullBandWidth_Is0, watchChannelID);
                foreach (FPGAIndex_AnalogSubbandAbsoluteIndex fpgaSubbandIndex in list)
                {
                    float gain_FineByFpgaThousand = (float)(DbiAnalogParams.Default[FullBandWidth_Is0, watchChannelID, yScaleIndex, fpgaSubbandIndex.subbandIndex].Gain_FineByFpgaThousand * 1.0 / 1000);
                    if (Hd.CurrDebugVarints.bEnable_AmplitudeTemperatureCompensate)
                    {
                        Hd.CurrProduct!.Acquirer_AnalogChannel!.GetPhyAnalogChAmplitudeTemperaturesCompensationCoefficient(out List<double> amplitudeTemperaturesCompensationCoefficient);
                        gain_FineByFpgaThousand *= (float)amplitudeTemperaturesCompensationCoefficient[(int)watchChannelID];
                    }
                    var FloatHighLowPair = AbstractAnalogChannel.Convert2HighLowShortPair(gain_FineByFpgaThousand);
                    //Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.DigZoom_Enable, fpgaSubbandIndex.AcqBdNo, 0x3); //两个通道使能都打开

                    //HTF_1120
                    UInt32 gain_FineByFpga = (UInt32)Math.Round(gain_FineByFpgaThousand * 2048);
                    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch1_L, fpgaSubbandIndex.AcqBdNo, gain_FineByFpga);
                    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch2_L, fpgaSubbandIndex.AcqBdNo, gain_FineByFpga);
                    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch3_L, fpgaSubbandIndex.AcqBdNo, gain_FineByFpga);
                    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch4_L, fpgaSubbandIndex.AcqBdNo, gain_FineByFpga);
                    //HTF_end

                    //for (int adcIndex = 0; adcIndex < 2; adcIndex++)
                    //{
                    //    (UInt32 lowReg, UInt32 highReg) DigZoomRegPair = (adcIndex == 0) ?
                    //        ((UInt32)AcqBdReg.W.DigZoom_Gainch1_L, (UInt32)AcqBdReg.W.DigZoom_Gainch1_H) : ((UInt32)AcqBdReg.W.DigZoom_Gainch2_L, (UInt32)AcqBdReg.W.DigZoom_Gainch2_H);
                    //    Hd.CurrProduct!.AcqBd!.WriteReg(DigZoomRegPair.lowReg, fpgaSubbandIndex.AcqBdNo, FloatHighLowPair.Low);
                    //    Hd.CurrProduct!.AcqBd!.WriteReg(DigZoomRegPair.highReg, fpgaSubbandIndex.AcqBdNo, FloatHighLowPair.High);
                    //}
                }
            }
            else
            {
                int FullBandWidth_Is0 = (Hd.CurrProduct?.AnalogAcquireModel?.GetDbiMergeState(Acquirer_AnalogChanel_DBI13G.DbiActuallActiveState) ?? false) ? 0 : 1;
                for (int channelID = 0; channelID < ChannelIdExt.AnaChnlNum; channelID++)
                {
                    //if ((currChBWModeAndActiveState & (1 << channelID)) != 0)
                    {
                        int yScaleIndex = Hd.UIMessage?.Analog?[(int)channelID].ScaleIndex ?? 5;

                        List<FPGAIndex_AnalogSubbandAbsoluteIndex> list = GetFPGAIndex_SubbandIndex(FullBandWidth_Is0, channelID);
                        foreach (FPGAIndex_AnalogSubbandAbsoluteIndex fpgaSubbandIndex in list)
                        {
                            //Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.DigZoom_Enable, fpgaSubbandIndex.AcqBdNo, 0x3); //两个通道使能都打开
                            float gain_FineByFpgaThousand = (float)(DbiAnalogParams.Default[0, channelID, yScaleIndex, fpgaSubbandIndex.subbandIndex].Gain_FineByFpgaThousand * 1.0 / 1000);
                            if (Hd.CurrDebugVarints.bEnable_AmplitudeTemperatureCompensate)
                            {
                                Hd.CurrProduct!.Acquirer_AnalogChannel!.GetPhyAnalogChAmplitudeTemperaturesCompensationCoefficient(out List<double> amplitudeTemperaturesCompensationCoefficient);
                                gain_FineByFpgaThousand *= (float)amplitudeTemperaturesCompensationCoefficient[(int)channelID];
                            }
                            var FloatHighLowPair = AbstractAnalogChannel.Convert2HighLowShortPair(gain_FineByFpgaThousand);
                            //Hd.CurrProduct!.AcqBd!.WriteReg((UInt32)AcqBdReg.W.DigZoom_Enable, fpgaSubbandIndex.AcqBdNo, 0x3); //两个通道使能都打开

                            //HTF_1120
                            UInt32 gain_FineByFpga = (UInt32)Math.Round(gain_FineByFpgaThousand * 2048);
                            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch1_L, fpgaSubbandIndex.AcqBdNo, gain_FineByFpga);
                            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch2_L, fpgaSubbandIndex.AcqBdNo, gain_FineByFpga);
                            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch3_L, fpgaSubbandIndex.AcqBdNo, gain_FineByFpga);
                            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.DigZoom_Gainch4_L, fpgaSubbandIndex.AcqBdNo, gain_FineByFpga);
                            //HTF_end

                            //for (int adcIndex = 0; adcIndex < 2; adcIndex++)
                            //{
                            //    (UInt32 lowReg, UInt32 highReg) DigZoomRegPair = (adcIndex == 0) ?
                            //        ((UInt32)AcqBdReg.W.DigZoom_Gainch1_L, (UInt32)AcqBdReg.W.DigZoom_Gainch1_H) : ((UInt32)AcqBdReg.W.DigZoom_Gainch2_L, (UInt32)AcqBdReg.W.DigZoom_Gainch2_H);
                            //    Hd.CurrProduct!.AcqBd!.WriteReg(DigZoomRegPair.lowReg, fpgaSubbandIndex.AcqBdNo, FloatHighLowPair.Low);
                            //    Hd.CurrProduct!.AcqBd!.WriteReg(DigZoomRegPair.highReg, fpgaSubbandIndex.AcqBdNo, FloatHighLowPair.High);
                            //}
                        }
                    }
                }
            }
            return true;

            //return true;
        }
        public override bool MiscFunc(string FuncName)
        {
            return FuncName switch
            {
                "CtrlGainByFpga" => CtrlGainByFpga(),
                _ => false,
            };
        }
        record FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo AcqBdNo, int subbandIndex/*模拟子带绝对序号，用于配置模拟子带的增益*/);
        private Dictionary<int/*BWMode*/, List<KeyValuePair<int, List<FPGAIndex_AnalogSubbandAbsoluteIndex>>>> BWModeChannel_20GHz4CH = new Dictionary<int, List<KeyValuePair<int, List<FPGAIndex_AnalogSubbandAbsoluteIndex>>>>()
        {
            /*BWMode Full*/
            [0] = new List<KeyValuePair<int, List<FPGAIndex_AnalogSubbandAbsoluteIndex>>>()
            {
                new KeyValuePair<int, List<FPGAIndex_AnalogSubbandAbsoluteIndex>>
                    //CHannel 1
                    (0, new List<FPGAIndex_AnalogSubbandAbsoluteIndex>()
                        {   new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B0, 0),
                            new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B1, 1),
                            new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B2, 2),
                            new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B3, 3)
                        }
                    ),
                //new KeyValuePair<int, List<FPGAIndex_AnalogSubbandAbsoluteIndex>>
                //    //CHannel 2
                //    (1, new List<FPGAIndex_AnalogSubbandAbsoluteIndex>()
                //        {
                //            new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B9, 0),
                //            new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B8, 1),
                //            new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B7, 2),
                //            //new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B1, 3)
                //        }
                //    ),
                //new KeyValuePair<int, List<FPGAIndex_AnalogSubbandAbsoluteIndex>>
                //    (2, new List<FPGAIndex_AnalogSubbandAbsoluteIndex>()
                //        {
                //            new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B10, 0),
                //            new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B11, 1),
                //            new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B12, 2),
                //            //new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B8, 3)
                //        }
                //    ),
                //new KeyValuePair<int, List<FPGAIndex_AnalogSubbandAbsoluteIndex>>
                //    (3, new List<FPGAIndex_AnalogSubbandAbsoluteIndex>()
                //        {
                //            new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B12, 0),
                //            new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B11, 1),
                //            new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B10, 2),
                //            //new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B5, 3)
                //        }
                //    )
            },
            //低带宽模式，此时：
            //CH1使用AcqBdNo.B1、AcqBdNo.B2、AcqBdNo.B3（16G），
            //CH2使用AcqBdNo.B4（5G）
            //CH3使用AcqBdNo.B5、AcqBdNo.B6、AcqBdNo.B7（16G）
            //CH4使用AcqBdNo.B8（5G）
            [1] = new List<KeyValuePair<int, List<FPGAIndex_AnalogSubbandAbsoluteIndex>>>()
            { new KeyValuePair<int, List<FPGAIndex_AnalogSubbandAbsoluteIndex>>
                    (0, new List<FPGAIndex_AnalogSubbandAbsoluteIndex>()
                    {   new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B0, 0), }),
                new KeyValuePair<int, List<FPGAIndex_AnalogSubbandAbsoluteIndex>>
                    (1, new List<FPGAIndex_AnalogSubbandAbsoluteIndex>()
                    {   new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B1, 0), }),
                new KeyValuePair<int, List<FPGAIndex_AnalogSubbandAbsoluteIndex>>
                    (2, new List<FPGAIndex_AnalogSubbandAbsoluteIndex>()
                    {   new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B2, 0), }),
                new KeyValuePair<int, List<FPGAIndex_AnalogSubbandAbsoluteIndex>>
                    (3, new List<FPGAIndex_AnalogSubbandAbsoluteIndex>()
                    {   new FPGAIndex_AnalogSubbandAbsoluteIndex(AcqBdNo.B3, 0)}) }
        };
        private List<FPGAIndex_AnalogSubbandAbsoluteIndex> GetFPGAIndex_SubbandIndex(int FullBandWidth_Is0, int channel)
        {
            Dictionary<int, List<KeyValuePair<int, List<FPGAIndex_AnalogSubbandAbsoluteIndex>>>> define = BWModeChannel_20GHz4CH;
            List<FPGAIndex_AnalogSubbandAbsoluteIndex> listFPGAIndex_SubbandIndex = new List<FPGAIndex_AnalogSubbandAbsoluteIndex>();
            for (int searcIndex = 0; searcIndex < define[FullBandWidth_Is0].Count; searcIndex++)
            {
                if (define[FullBandWidth_Is0][searcIndex].Key == (int)channel)
                {
                    listFPGAIndex_SubbandIndex = define[FullBandWidth_Is0][searcIndex].Value;
                    break;
                }
            }
            return listFPGAIndex_SubbandIndex;
        }
        public override void CtrlFineGain(ChannelId channelIndex)
        {
            //if (Hd.UIMessage?.Analog?[(int)channelIndex] == null)
            //    return;
            //int channelID = (int)channelIndex;
            //int FullBandWidth_Is0 = (Hd.CurrProduct?.AnalogAcquireModel?.GetDbiMergeState(Acquirer_AnalogChanel_DBI13G.DbiActuallActiveState) ?? false) ? 0 : 1;
            //List<FPGAIndex_AnalogSubbandAbsoluteIndex>? listFPGAIndex_SubbandIndex = GetFPGAIndex_SubbandIndex(FullBandWidth_Is0, (int)channelIndex);
            //if (listFPGAIndex_SubbandIndex == null)
            //    return;
            //AnalogOptions analogParameters = Hd.UIMessage.Analog[(int)channelIndex];
            //foreach (var v in listFPGAIndex_SubbandIndex)
            //{
            //    for (int adcIndex = 0; adcIndex < 2; adcIndex++)
            //    {
            //        AdjustAdc_Gain(true, channelID, v.subbandIndex, v.AcqBdNo, adcIndex);
            //    }
            //}
        }

        public override void TiAdc_ApplayAdc_SyncSampleClock()
        {
            //bool old_bForceReFind5200AdcSyncWindow = Hd.bForceReFind5200AdcSyncWindow;
            //Hd.bForceReFind5200AdcSyncWindow = false;
            //DBI 是按子带划分的，而不是通道号
            //全部发送
            int subBandIndex = 0;
            foreach (var acqbdno in Enum.GetValues<AcqBdNo>())
            {
                if (Hd.CurrProduct!.AcqBd!.ExistsAcqBdDefine.Contains(acqbdno))
                {
                    for (int adcIndex = 0; adcIndex < Constants.ADC_NUM; adcIndex++)
                    {
                        uint data = 0;
                        int acqBdIndex = (int)acqbdno;
                        if (acqBdIndex >= 0 && acqBdIndex < CaliConstants.Fixed_AcqBoardMaxCount)
                        {
                            var syncClockByBoard = TiAdc_SyncSampleClock.Default[acqBdIndex];
                            if (syncClockByBoard != null && adcIndex >= 0 && adcIndex < syncClockByBoard.Length)
                                data = syncClockByBoard[adcIndex].Sample20GClockDelay & 0x0f;//只有低4位有效
                        }
                        Send5200CmdWithCS_OneFpga(acqbdno, adcIndex, 0x0029, 0x30 | data);//use SYSREF calibration,delay steps are finer,enable the SYSREF receiver circuit
                        Send5200CmdWithCS_OneFpga(acqbdno, adcIndex, 0x0029, 0x70 | data);//SYSREF_RECV_EN must be set before setting SYSREF_PROC_EN
                    }
                    subBandIndex++;
                }
            }
            //Hd.bForceReFind5200AdcSyncWindow = old_bForceReFind5200AdcSyncWindow;
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



            //增益
            //int currChBWModeAndActiveState = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.CurrChBWModeAndActiveState ?? 0;
            //if (!Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode)
            //{
            //    int watchChannelID = (int)Hd.CurrDebugVarints.iDbi_DebugChannelID;
            //    int FullBandWidth_Is0 = (Hd.CurrProduct?.AnalogAcquireModel?.GetDbiMergeState(Acquirer_AnalogChanel_DBI13G.DbiActuallActiveState) ?? false) ? 0 : 1;
            //    List<FPGAIndex_AnalogSubbandAbsoluteIndex> list = GetFPGAIndex_SubbandIndex(FullBandWidth_Is0, watchChannelID);
            //    int relativeSubbandIndex = 0;
            //    foreach (FPGAIndex_AnalogSubbandAbsoluteIndex fpgaSubbandIndex in list)
            //    {
            //        AdjustAdc_Gain(true, watchChannelID, relativeSubbandIndex, fpgaSubbandIndex.AcqBdNo, 0);//ADC1
            //        AdjustAdc_Gain(true, watchChannelID, relativeSubbandIndex, fpgaSubbandIndex.AcqBdNo, 1);//ADC2

            //        relativeSubbandIndex++;
            //    }
            //}
            //else
            //{
            //    int FullBandWidth_Is0 = (Hd.CurrProduct?.AnalogAcquireModel?.GetDbiMergeState(Acquirer_AnalogChanel_DBI13G.DbiActuallActiveState) ?? false) ? 0 : 1;
            //    //增益
            //    for (int channelID = 0; channelID < ChannelIdExt.AnaChnlNum; channelID++)
            //    {
            //        if ((currChBWModeAndActiveState & (1 << channelID)) != 0)
            //        {
            //            List<FPGAIndex_AnalogSubbandAbsoluteIndex> list = GetFPGAIndex_SubbandIndex(FullBandWidth_Is0, channelID);
            //            int relativeSubbandIndex = 0;
            //            foreach (FPGAIndex_AnalogSubbandAbsoluteIndex fpgaSubbandIndex in list)
            //            {
            //                AdjustAdc_Gain(true, channelID, relativeSubbandIndex, fpgaSubbandIndex.AcqBdNo, 0);//ADC1
            //                AdjustAdc_Gain(true, channelID, relativeSubbandIndex, fpgaSubbandIndex.AcqBdNo, 1);//ADC2

            //                relativeSubbandIndex++;
            //            }
            //        }
            //    }
            //}

            ////Phase，仅仅是板内的两片ADC之间的Phase，故与子带模式无关

            //int subbandIndex = 0;
            //var tiadcparamskeymaps = new List<TiadcParamsKeyMap>()
            //        {
            //            new("All-20G", (ChannelId.C1), 0),
            //            new("All-20G", (ChannelId.C1), 1),
            //            new("All-20G", (ChannelId.C1), 2),
            //            new("All-20G", (ChannelId.C1), 3),
            //            new("All-20G", (ChannelId.C2), 0),
            //            new("All-20G", (ChannelId.C2), 1),
            //            new("All-20G", (ChannelId.C2), 2),
            //            new("All-20G", (ChannelId.C2), 3),
            //            new("All-20G", (ChannelId.C3), 0),
            //            new("All-20G", (ChannelId.C3), 1),
            //            new("All-20G", (ChannelId.C3), 2),
            //            new("All-20G", (ChannelId.C3), 3),
            //            new("All-20G", (ChannelId.C4), 0),
            //            new("All-20G", (ChannelId.C4), 1),
            //            new("All-20G", (ChannelId.C4), 2),
            //            new("All-20G", (ChannelId.C4), 3)
            //        };
            //foreach (var item in tiadcparamskeymaps)
            //{

            //    AdjustAdc_Phase(true, subbandIndex, item.chnlId, (AcqBdNo)(2 * (Int32)item.chnlId + (item.adcId % 2)), 0);//ADC1
            //    AdjustAdc_Phase(true, subbandIndex, item.chnlId, (AcqBdNo)(2 * (Int32)item.chnlId + (item.adcId % 2)), 1);//ADC2
            //    subbandIndex++;

            //}
        }
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

        private void AdjustAdc_PhaseEx(Boolean withAdcCS, AcqBdNo fpgaIndex, Int32 adcIndex, Int32 phasecase, Int32 phasefine)
        {
            Action<AcqBdNo, Int32, UInt32, UInt32> sender = withAdcCS ? Send5200CmdWithCS_OneFpga : SendCmdToAD5200_OneFpga;
            (UInt32 lowReg, UInt32 highReg) setregpair = (0x02B5U, 0x02B6U);

            sender.Invoke(fpgaIndex, adcIndex % 2, setregpair.lowReg, (UInt32)(phasefine & 0xffff));  //phase fine
            sender.Invoke(fpgaIndex, adcIndex % 2, setregpair.highReg, (UInt32)(phasecase & 0xffff)); //phase coarse 
        }
        //public override void TiAdc_ApplyAdc_Phase_Offset_Gain()
        //{
        //    HdDebugLogger.Log($"[{DateTime.Now}]: Adc Phase_Offset_Gain Start!");
        //    var analogAcquireModel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
        //    AcqModeAndInterleaveDefine interdefine = analogAcquireModel.GetCurrentAcqModeInterleave()!;
        //    Stopwatch stopwatch2 = Stopwatch.StartNew();
        //    stopwatch2.Start();
        //    foreach (var dtl in interdefine.Details)
        //    {
        //        var adcinfo = dtl.Value[0];
        //        foreach (var item in dtl.Value)
        //        {
        //            adcinfo = item;
        //            var usedadcs = analogAcquireModel.GetUsedAdcs(adcinfo);
        //            foreach (var adcId in usedadcs)
        //            {
        //                var tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItemWithBoard(new(interdefine.Name, dtl.Key, item.AcqBdNo, adcId % 2))!.Value;
        //                Stopwatch stopwatch = Stopwatch.StartNew();
        //                stopwatch.Start();
        //                AdjustAdc_Gain(true, adcinfo.AcqBdNo, (Int32)adcId, tiadcItem.Gain);//173ms
        //                stopwatch.Stop();
        //                var ss = stopwatch.ElapsedMilliseconds;
        //                //AdjustAdc_Phase(true, adcinfo.AcqBdNo, (Int32)adcId, tiadcItem.Phase);//50ms
        //                AdjustAdc_PhaseEx(true, adcinfo.AcqBdNo, (Int32)adcId, tiadcItem.Reserved0, tiadcItem.Reserved1);
        //                AdjustAdc_FPGAADCDelay(dtl.Key, adcinfo.AcqBdNo, (Int32)adcId, (UInt32)tiadcItem.AdcDelay_FPGA);//fpga丢点

        //                HdMessage.AnalogOptions analogparas = Hd.UIMessage!.Analog![(Int32)dtl.Key];
        //                var scale = analogparas.ScaleBymV;
        //                //Adjust_FpgaConfigOffset(adcinfo.AcqBdNo, (Int32)adcId, scale<50?(UInt32)tiadcItem.Offset_FPGA_10mv: (UInt32)tiadcItem.Offset_FPGA);//offset细调
        //                Adjust_FpgaConfigOffset(adcinfo.AcqBdNo, (Int32)adcId, scale < 50 ? (UInt32)tiadcItem.Offset_FPGA : (UInt32)tiadcItem.Offset_FPGA);//offset细调
        //                ////Adjust_FpgaConfigGain(adcInfo.AcqBdNo, (int)adcId, (uint)tiadcItem.Gain_FPGA);//gain细调
        //            }
        //        }

        //    }
        //    stopwatch2.Stop();
        //    var sss = stopwatch2.ElapsedMilliseconds;
        //    HdDebugLogger.Log($"[{DateTime.Now}]: Adc Phase_Offset_Gain End!");
        //}
        public void AdjustAdc_Phase(bool withAdcCS, int subbandIndex,ChannelId channelId, AcqBdNo fpgaIndex, int adcIndex)
        {
            //phase
            //与通道对应的子带无关，是固定的，采集板的顺序是固定的。
            int acqBoardIndex = ((int)fpgaIndex);/// 2;//0,2,4,6,对应B1,B3,B5,B7
            TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(new("All-20G", (channelId), (uint)adcIndex))!.Value;

            //int phase = TiAdc_PhaseOffsetGain.Default[acqBoardIndex, adcIndex, 0].PhaseErr;
            int phase = tiadcItem.Phase;
            int AdcSigalInputPort_AIs1 = GetAdcInpiutPort(fpgaIndex, adcIndex);// 2;//固定，不改变
            bool bNeedSend = AdcAlreadySendDataManager.Default.CheckNeedSend(AdcConfigDataType.Phase, (int)fpgaIndex, adcIndex, AdcSigalInputPort_AIs1, (int)phase);
            //if (!bNeedSend)
            //    return;
            Action<AcqBdNo, Int32, UInt32, UInt32> sender = withAdcCS ? Send5200CmdWithCS_OneFpga : SendCmdToAD5200_OneFpga;
            (UInt32 lowReg, UInt32 highReg) setRegPair = (AdcSigalInputPort_AIs1 == 1) ? (0x02B5U, 0x02B6U) : (0x02B5U, 0x02B6U);

            sender.Invoke(fpgaIndex, adcIndex, setRegPair.lowReg, (UInt32)(phase & 0xff));//phase coarse 
            sender.Invoke(fpgaIndex, adcIndex, setRegPair.highReg, (UInt32)((phase >> 8) & 0x0ff));//phase coarse 
            Adc5200WaitForAutoCaliFinished.Default.TryAdd((int)fpgaIndex, adcIndex);
        }
        //public void AdjustAdc_Offset(int subbandIndex, AcqBdNo fpgaIndex, int adcIndex)
        //{
        //    //offset                                                                                                                                           //offset
        //    SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0348, (UInt32)(TiAdc_PhaseOffsetGain.Default[subbandIndex, adcIndex, 0].OffsetErr & 0xff));//offset core A ,low 8 bit
        //    SendCmdToAD5200_OneFpga(fpgaIndex, adcIndex, 0x0349, (UInt32)((TiAdc_PhaseOffsetGain.Default[subbandIndex, adcIndex, 0].OffsetErr >> 8) & 0xff));//offset core A ,hight 4 bit
        //}

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

        //private void AdjustAdc_Gain(bool widthAdcCS, int channelID, int subbandIndex, AcqBdNo fpgaIndex, int adcIndex)
        //{
        //    int Impedance_H_is0 = (Hd.UIMessage?.Analog?[(int)channelID].Coupling ?? AnaChnlCoupling.AC1M) == AnaChnlCoupling.DC50 ? 1 : 0;
        //    int yScaleIndex = Hd.UIMessage?.Analog?[(int)channelID].ScaleIndex ?? 5;
        //    int FullBandWidth_Is0 = (Hd.CurrProduct?.AnalogAcquireModel?.GetDbiMergeState(Acquirer_AnalogChanel_DBI13G.DbiActuallActiveState) ?? false) ? 0 : 1;
        //    //TiAdc增益
        //    //与通道对应的子带无关，是固定的，采集板的顺序是固定的。
        //    int acqBoardIndex = (int)fpgaIndex;// ((int)fpgaIndex) / 2;//0,2,4,6,对应B1,B3,B5,B7

        //    TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(new("All-20G", (ChannelId)channelID, (uint)adcIndex))!.Value;

        //    uint gain = (uint)tiadcItem.Gain;
        //    //补偿模拟通道的增益
        //    if (adcIndex == 0)
        //        gain = gain * DbiAnalogParams.Default[FullBandWidth_Is0, channelID, yScaleIndex, subbandIndex].Gain_FineByAdc1ByTenThousand / 10000;//以10000位基数，实际上就是百分比
        //    else
        //        gain = gain * DbiAnalogParams.Default[FullBandWidth_Is0, channelID, yScaleIndex, subbandIndex].Gain_FineByAdc2ByTenThousand / 10000;//以10000位基数，实际上就是百分比

        //    if (gain > 0xffff)
        //        gain = 0xffff;
        //    else if (gain < 0x2000)//8192
        //        gain = 0x2000;
        //    int AdcSigalInputPort_AIs1 = GetAdcInpiutPort((int)fpgaIndex, adcIndex);//固定，不改变
        //    bool bNeedSend = AdcAlreadySendDataManager.Default.CheckNeedSend(AdcConfigDataType.Gain, (int)fpgaIndex, adcIndex, AdcSigalInputPort_AIs1, (int)gain);

        //    //if (!bNeedSend)
        //    //    return;
        //    #region 发送
        //    (UInt32 lowReg, UInt32 highReg) adcGainSetRegPair = (AdcSigalInputPort_AIs1 == 1) ? (0x30U, 0x31U) : (0x32U, 0x33U);
        //    Action<AcqBdNo, Int32, UInt32, UInt32> sender = widthAdcCS ? Send5200CmdWithCS_OneFpga : SendCmdToAD5200_OneFpga;

        //    sender.Invoke(fpgaIndex, adcIndex, (AdcSigalInputPort_AIs1 == 1) ? 0x7AU : 0x7BU, 1);//PORT A 打开增益可调,PORT B 打开增益可调

        //    sender.Invoke(fpgaIndex, adcIndex, adcGainSetRegPair.lowReg, (UInt32)(gain & 0xff));//gain B  ,low 8 bit
        //    sender.Invoke(fpgaIndex, adcIndex, adcGainSetRegPair.highReg, (UInt32)((gain >> 8) & 0x0ff));//gain B  ,high 4 bit

        //    Adc5200WaitForAutoCaliFinished.Default.TryAdd((int)fpgaIndex, adcIndex);

        //    #endregion
        //}
        //private int GetAdcInpiutPort(int fpgaIndex, int adcIndex)
        //{
        //    return Hd.CurrProduct.Acquirer_AnalogChannel!.FirstInitChannelBdAdcInputDefines![fpgaIndex][adcIndex].InputPort_AIs1;
        //}
        private Int32 GetAdcInpiutPort(AcqBdNo fpgaIndex, Int32 adcIndex)
        {
            AcqModeAndInterleaveDefine define = Hd.CurrProduct.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;
            foreach (var dtl in define.Details)
            {
                foreach (var adcinfo in dtl.Value)
                {
                    if (adcinfo.AcqBdNo == fpgaIndex && ((adcinfo.Adc >> adcIndex) & 0x1) == 0x1)
                    {
                        return adcinfo.AdcPorts[adcIndex % adcinfo.AdcPorts.Count()];
                    }
                }
            }
            throw new ArgumentException($"fpgaIndex = {fpgaIndex.ToString()} ; adcIndex = {adcIndex}");
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

    }
}
#endif

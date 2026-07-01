#if DBI
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
namespace ScopeX.Hardware.Driver
{
    internal class Controller_Misc_DBI : AbstractController_Misc
    {
        public Controller_Misc_DBI()
        {
            _AfterFirstInitAction = ourAfterFirstInitAction;
            _AnalogChannelActiveChanged = ourAnalogChannelActiveChanged;
            _CaliDataChanged = ourCaliDataChanged;
            _AcqIsFulled = ourAcqIsFulled;
            _ReadTrigStatus = ourReadTrigStatus;
            _AllPowerDown = ourAllPowerDown;
        }

        private void ourAllPowerDown()//????
        {
            //step1:AnalogChannel
            AbstractController_AnalogChannel.PowerOff();
            //step2:前置接口板,LA
            //HdIO.WriteReg(ProcBdReg.W.LA_PowerCtrl, 0x0);
            //step3:AWG
            AWG.PowerOff();
            //step4:
            if (!Constants.ENABLE_DEBUG)
            {
                //HdIO.WriteReg(PcieBdReg.W.PowerManager_AcqBoard_Power, 0x0);

                HdIO.WriteReg(PcieBdReg.W.PowerManager_ProcBoard_Power, 0x0);
            }

            //HdIO.WriteReg(PcieBdReg.W.PowerManager_reg_acq_bd_power_en, 0);
            Thread.Sleep(5);
            //CIJ_TEST
            //HdIO.WriteReg(PcieBdReg.W.PowerManager_reg_acq_bd_power_en1, 0);
            Thread.Sleep(5);
            //HdIO.WriteReg(PcieBdReg.W.PowerManager_reg_acq_bd_power_en2, 0);

            Thread.Sleep(100);
        }

        private void ourAnalogChannelActiveChanged()
        {
            Hd.CurrProduct?.Acquirer_AnalogChannel?.AnalogChannelActiveChanged();
        }
        internal void ourAfterFirstInitAction()//????
        {

            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_DataAcqL, 24U);
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_DataAcqH, 0);

            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_DDCbadpoints_num, 200);

            HdIO.WriteReg(PcieBdReg.W.ReadFromAcqOrDpo, 0);

            //comment for JiHe_MSO7000X HdIO.WriteReg(PcieBdReg.W.RST_CTRL_SysResetFromPcie, 1);//sys_rst_from_pcie
            //comment for JiHe_MSO7000X HdIO.WriteReg(PcieBdReg.W.RST_CTRL_SysResetFromPcie, 0);//sys_rst_from_pcie
            //Pro_Reset();
            //comment for JiHe_MSO7000X HdIO.WriteReg(PcieBdReg.W.RST_CTRL_SysResetFromPcie, 0x1);
            //comment for JiHe_MSO7000X HdIO.WriteReg(PcieBdReg.W.RST_CTRL_SysResetFromPcie, 0x0);
            //=======================================

            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.Data2Pcie_ResetTxIO, 1);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.Data2Pcie_ResetTxIO, 0);

            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.Data2Pcie_ResetTxClk, 1);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.Data2Pcie_ResetTxClk, 0);

            //Module.Decoder.Init();
            HdIO.WriteReg(PcieBdReg.W.DMADataSource_DdrEnable, 0);
            HdIO.WriteReg(PcieBdReg.W.DMADataSource_DpxEnable, 0);


            //comment for JiHe_MSO7000X HdIO.WriteReg(PcieBdReg.W.SyncDataRxIDelay_RxIOReset, 1);
            //comment for JiHe_MSO7000X HdIO.WriteReg(PcieBdReg.W.SyncDataRxIDelay_RxIOReset, 0);

            //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_ChannelMode, Hd.CurrProduct?.HardwareConfig?.Default_PcieBoardFifoCtrl_ChannelMode ?? 2);
            ConfigFifoLength();

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Location_TestModeEn, 0);  //**trig_1st_test_mode_en

            //AcqBd.WrToAllFpga(0x80c0, 0x02);//channel_mode

            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Location_TrigDiscardColumnEn, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 1U : 0);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Location_TestModeEn, 0);


            bool bEnableInterpolation = Hd.CurrDebugVarints.bEnable_AcqbdInterpolation;
            if (bEnableInterpolation)
            {
                bEnableInterpolation = false;
            }
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Interpolate_Enable, bEnableInterpolation ? 1U : 0U);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueH16, (UInt32)(1));//1抽1
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_M16, (UInt32)(0));
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_H16, (UInt32)(0));
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, 0);//normal

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Interpolate_ProgFullThresh, 10240);

            //HdIO.WriteReg(PcieBdReg.W.DataPath_IddrIdelay3EnVTC, 0x1);

            Acquisition.Init();

            Hd.CurrProduct?.Acquirer_AnalogChannel?.AnalogChannelActiveChanged();

            //InitExtTrig();

            SysAutoCalibration.Default.Trig_AcqProcBdLooptime_Cali();//获取AcqBoard 与 Proc Board 传输回路延迟，用于触发深度修正
                                                                     //SysAutoCalibration.Default.LoadAndSetting_AcqProcBdLooptimeDelay();

            //HdSpecial.FactoryCaliScpiProc_SpecialData_Get("FpgaVersionInfoAtFlash");

            //HdIO.WriteReg(ProcBdReg.W.BoardSync_IO_Reset, 1);
            //Thread.Sleep(1);
            //HdIO.WriteReg(ProcBdReg.W.BoardSync_IO_Reset, 0);
            #region DMA 试读
            if (HdIO.CurrDriver != null && HdIO.CurrDriver.bOpen)
            {
                byte[] tmpBuffer = new byte[4 * 1024 * 1024];
                Hd.CurrProduct!.AcqBd!.SwitchDataPathAndPcieReset_SetDataLength(DMAReadDataTypes.AnalogChannelDdr, (UInt32)tmpBuffer.Length);
                bool bOk = HdIO.DMARead((UInt32)tmpBuffer.Length, ref tmpBuffer);
            }
            #endregion

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_DigitalTrigEnAcq, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 1U : 0);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Acq_NoiseControl, 1U);

            //Hd.Calibration.DoCali_Trigger_AcqBdSignalProcBdDdrCtrlClockDelay();
            #region 除处理板、采集板之外的其他附件的上电
            ExcludeProcAcqBoardPowerOn();
            #endregion
            #region test 用代码
            //HdIO.WriteReg(ProcBdReg.W.Debug_SingleModePro, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x1700);//[15]：test data [11:8] mask data 1 2 3 4  [0]0-trigger [12]-discard location enable
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_acq_ddr_test_en, data: 0);
            #endregion
        }

        private void ExcludeProcAcqBoardPowerOn()
        {
            //STEP1:AWG
            //AWG.PowerOn();
            //不仅仅是LA的上电，而是叫做前置接口板
            //STEP2:LA
            //HdIO.WriteReg(ProcBdReg.W.LA_PowerCtrl, 0x1);
            //Thread.Sleep(50);
            //STEP3:AnalogChannel
            AbstractController_AnalogChannel.PowerOn();
            AbstractController_AnalogChannel.Init();
        }

        private void ConfigFifoLength()
        {
            ////Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.FifoCtrl_FullProgDepth, 1024 + 30);//采集板并行FIFO深度
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.FifoCtrl_FullProgDepth, 500);//采集板并行FIFO深度
            ////Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_AcqSerialFIFODepth, 16000);//采集板串行FIFO深度 16000 -->25600 zy0830 --> 53248
            //HdIO.WriteReg(ProcBdReg.W.FifoCtrl_ParallelFifoDepth, 12288/2);
            //HdIO.WriteReg(ProcBdReg.W.FifoCtrl_FullProgDepth, 12288);//12288 -->22528 --> 53248 
            //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, 12288);
            //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_ReadFromFIFO_Num, 15000);//zy0830  15000-->25000--> 53248
        }
        private void ourCaliDataChanged()
        {
            if (CaliDataManager.DataChangedCaliDataType_Running.Count == 0)
                return;
            foreach (CaliDataType type in CaliDataManager.DataChangedCaliDataType_Running)
            {
                if (type == CaliDataType.None || type == CaliDataType.All)
                    continue;
                switch (type)
                {
                    case CaliDataType.PhyChannelModel2:
                    case CaliDataType.PhyChannel:
                    case CaliDataType.DbiAnalogParams:
                    case CaliDataType.AiAnalogParams:
                        AbstractController_AnalogChannel.Ctrl4094();
                        AbstractController_AnalogChannel.CtrlOffset();
                        AbstractController_AnalogChannel.CtrlGain();
                        AbstractController_AnalogChannel.CtrlBias();
                        AbstractController_AnalogChannel.ActiveChannged();
                        break;
                    case CaliDataType.DbiLocalOscillators:
                        AbstractController_AnalogChannel.SwitchDBI_ASCII();
                        break;
                    case CaliDataType.TiAdc_PhaseOffsetGain_JiHe_MSO7000X:
                    case CaliDataType.TiAdc_PhaseOffsetGain:
                        Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
                        AbstractController_AnalogChannel.CtrlOffset();
                        break;
                    case CaliDataType.TiadcPhaseOffsetGainParams:
                        Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
                        break;
                    case CaliDataType.TiAdc_SyncSampleClock:
                        Hd.CurrProduct?.AcqBd?.TiAdc_ApplayAdc_SyncSampleClock();
                        break;
                    case CaliDataType.CoefficientsTables:
                        foreach (CoefficientsTableType coefficientsTableType in CaliDataManager.DataChangedCoefficientsTableType_Running)
                        {
                            if (Hd.CurrProduct?.HardwareConfig?.LocalCoefficientsTableMeanings.ContainsKey(coefficientsTableType) ?? false)
                                Hd.CurrProduct?.HardwareConfig?.LocalCoefficientsTableMeanings[coefficientsTableType]?.Sender?.Invoke(coefficientsTableType, true);
                        }
                        CaliDataManager.DataChangedCoefficientsTableType_Running.Clear();
                        break;
                    case CaliDataType.Misc:
                        //外触发
                        AbstractController_AnalogChannel.CtrlExtTrig();
                        //==触发
                        AbstractController_Trigger.ConfigTypeAndParameter();
                        AbstractController_Trigger.ConfigFifoStageDepth_WithAcqLength();
                        break;
                    case CaliDataType.DbiCoefficientsTables:
                        lock (CaliDataManager.DbiDataChangedLocker)
                        {
                            CoefficientsTableSender_DBI.SendCoefficientsTables();
                            CaliDataManager.DataChangedDbiCoefficientsTablesType_Running.Clear();
                        }
                        break;
                }
            }
            CaliDataManager.DataChangedCaliDataType_Running.Clear();
        }
    }
}
#endif

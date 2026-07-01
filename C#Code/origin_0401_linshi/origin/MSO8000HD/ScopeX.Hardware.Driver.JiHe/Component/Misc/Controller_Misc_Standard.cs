#if !Product_B21_JinHui_PXI
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
namespace ScopeX.Hardware.Driver
{
    internal class Controller_Misc_Standard : AbstractController_Misc
    {
        private Boolean _IsFirstLoadCaliData = false;
        public Controller_Misc_Standard()
        {
            _AfterFirstInitAction = ourAfterFirstInitAction;
            _AnalogChannelActiveChanged = ourAnalogChannelActiveChanged;
            _CaliDataChanged = delegate { ourCaliDataChanged(); };
            _AcqIsFulled = ourAcqIsFulled;
            _ReadTrigStatus = ourReadTrigStatus;
            _AllPowerDown = ourAllPowerDown;
            _ConfigDspCoefficients = configDspCoefficients;
        }
        private void ourAnalogChannelActiveChanged()
        {
            //Hd.CurrProduct?.Acquirer_AnalogChannel?.AnalogChannelActiveChanged();
            //CoefficientsTableSender_8000X.Send_IFCCoefficientsToAcqBoardByRegisterMode(true);
        }
        private void Pro_Reset()
        {

            //HdIO.WriteReg(ProcBdReg.W.fifoCtrl_FIFO_RST, 0x1);

            //HdIO.WriteReg(ProcBdReg.W.fifoCtrl_FIFO_RST, 0x0);
            //data = HdIO.ReadReg(sys_reset_regsiter);

        }
        private void ExcludeProcAcqBoardPowerOn()
        {
            //STEP1:AWG
            AWG.AwgPoweOnAndDACConfig();
            //不仅仅是LA的上电，而是叫做前置接口板
            //STEP2:LA
            Hd.CurrProduct?.Acquirer_LA?.PowerOn();
            Thread.Sleep(50);
            //STEP3:AnalogChannel
            AbstractController_AnalogChannel.PowerOn();
            AbstractController_AnalogChannel.Init();
        }
        private void ourAllPowerDown()
        {
            //退出软件 风扇转速默认全部2000r
            SystemMonitor.Default.CtrlFanSpeed(2000);
            SystemMonitor.Default.CtrlFanSpeed(2000, false);

            //step1:AnalogChannel
            AbstractController_AnalogChannel.PowerOff();
            //step2:前置接口板,LA
            Hd.CurrProduct?.Acquirer_LA?.PowerOff();
            //step3:AWG
            AWG.PowerOff();
            //step4:
            //cij_new
            if (!Constants.ENABLE_DEBUG)
            {
     //           HdIO.WriteReg(PcieBdReg.W.PowerManager_AcqBoard_Power, 0x0);
     //           HdIO.WriteReg(PcieBdReg.W.PowerManager_ProcBoard_Power, 0x0);
            }
        }
        internal void ourAfterFirstInitAction()
        {
            _IsFirstLoadCaliData = true;
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_DataAcqL, 24U);
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_DataAcqH, 0);

            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.MDO_DDC_DDCbadpoints_num, 200);

            HdIO.WriteReg(PcieBdReg.W.ReadFromAcqOrDpo, 0);

            //comment for JiHe_MSO7000X HdIO.WriteReg(PcieBdReg.W.RST_CTRL_SysResetFromPcie, 1);//sys_rst_from_pcie
            //comment for JiHe_MSO7000X HdIO.WriteReg(PcieBdReg.W.RST_CTRL_SysResetFromPcie, 0);//sys_rst_from_pcie
            Pro_Reset();
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

            HdIO.WriteReg(PcieBdReg.W.FifoCtrl_ChannelMode, Hd.CurrProduct?.HardwareConfig?.Default_PcieBoardFifoCtrl_ChannelMode ?? 2);
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

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_L16, (UInt32)(1));//1抽1
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_M16, (UInt32)(0));
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_H16, (UInt32)(0));
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, 0);//normal

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Interpolate_ProgFullThresh, 10240);
            Acquisition.Init();

            InitExtTrig();

            SysAutoCalibration.Default.Trig_AcqProcBdLooptime_Cali();//获取AcqBoard 与 Proc Board 传输回路延迟，用于触发深度修正
                                                                     //SysAutoCalibration.Default.LoadAndSetting_AcqProcBdLooptimeDelay();

            //HdSpecial.FactoryCaliScpiProc_SpecialData_Get("FpgaVersionInfoAtFlash");

            //HdIO.WriteReg(ProcBdReg.W.BoardSync_IO_Reset, 1);
            //Thread.Sleep(1);
            //HdIO.WriteReg(ProcBdReg.W.BoardSync_IO_Reset, 0);
            #region DMA 试读
            //if (HdIO.CurrDriver != null && HdIO.CurrDriver.bOpen)
            //{
            //    byte[] tmpBuffer = new byte[4 * 1024 * 1024];
            //    Hd.CurrProduct!.AcqBd!.SwitchDataPathAndPcieReset_SetDataLength(DMAReadDataTypes.AnalogChannelDdr, (UInt32)tmpBuffer.Length);
            //    bool bOk = HdIO.DMARead((UInt32)tmpBuffer.Length, ref tmpBuffer);
            //}
            #endregion

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_DigitalTrigEn, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 1U : 0);

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Acq_NoiseControl, 1U);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SchmittGate_Ch1, 10);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SchmittGate_Ch2, 10);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SchmittGate_Ch3, 10);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SchmittGate_Ch4, 10);
           // Hd.Calibration.DoCali_Trigger_AcqBdSignalProcBdDdrCtrlClockDelay();  //2023年初7000X方案，GYT，采集板发出测试波形，对触发偏移进行校准
            #region 除处理板、采集板之外的其他附件的上电
            ExcludeProcAcqBoardPowerOn();
            #endregion

            #region 读硬件版本
            FpgaVersion.ReadHdVersion();
            #endregion

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ADC_INL_ModuleEn, Hd.CurrDebugVarints.bEnable_AdcINL ? 1U : 0);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Acq_NoiseControl, Hd.CurrDebugVarints.bEnable_CtrlGainByFpga ? 1U : 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ConditionFilter_Enable, Hd.CurrDebugVarints.bEnable_AdcConditionFilter ? 1U : 0);

            //开机启动通道温度读取线程
            //Hd.AnalogChannel!.GetCurrentTemperature();
            #region test 用代码
            HdIO.WriteReg(ProcBdReg.W.debug_pro_debug_mode, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x1700);//[15]：test data [11:8] mask data 1 2 3 4  [0]0-trigger [12]-discard location enable
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_acq_ddr_test_en, data: 0);
            #endregion
        }
        private bool ourAcqIsFulled()
        {
            if (Hd.UIMessage!.Timebase!.IsScan && !Hd.UIMessage!.bAcquireStopped)
            {
                return HdCtrl_AnalogFifo.CanRead();
            }
            else
            {
                if ((Hd.UIMessage!.Display?.IsFast ?? false) && (Hd.CurrProduct!.Acquirer_DPX!.bInterpolateNumGT100 == false))
                {
                    return HdIO.ReadReg(ProcBdReg.R.UPO_UpoReady) == 1;
                }
                else if(Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.bIsLongStorageMode ?? false)
                {
                    return HdCtrl_AnalogDDR.WriteFinished();
                }
                else
                    return (HdIO.ReadReg(ProcBdReg.R.LSCtrl_FullFlag) & 0x01) == 1;
            }

        }

        private void InitExtTrig()
        {
            //复位
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Ext_Setting, 0x0004);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Ext_Setting, 0x0001);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Ext_Setting, 0x0004);

            //设置选择分频链路
            //Int32 extTrigSetting = 0x0024;
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Ext_Setting, (uint)extTrigSetting);
        }
        private UInt32 ourReadTrigStatus()
        {//cij_new_0329
            //var trig_status = (uint)(HdIO.ReadReg(ProcBdReg.R.TrigCtrl_Status) & 0x07);
            var trig_status2 = (uint)HdIO.ReadReg(ProcBdReg.R.TrigCtrl_Status) & 0x07;
            //return HdIO.ReadReg(ProcBdReg.R.TrigCtrl_Status) & 0x07;
            return trig_status2;
        }
        private void ConfigFifoLength()
        {
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.FifoCtrl_FullProgDepth, 1024 + 30);//采集板并行FIFO深度
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.FifoCtrl_FullProgDepth, 4000);//采集板并行FIFO深度
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_AcqSerialFIFODepth, 16000);//采集板串行FIFO深度 16000 -->25600 zy0830 --> 53248
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.FifoCtrl_ParallelFifoDepth, 8191);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.FifoCtrl_FullProgDepth, 28*1024);//12288 -->22528 --> 53248 
            //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, 12288 * 8);
            //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_ReadFromFIFO_Num, 28 * 1024 * 4 * 4);//zy0830  15000-->25000--> 53248
        }
        private void ourCaliDataChanged()
        {
            if (CaliDataManager.DataChangedCaliDataType_Running.Count == 0)
                return;
            foreach (CaliDataType type in CaliDataManager.DataChangedCaliDataType_Running)
            {
                if (type == CaliDataType.None || type == CaliDataType.All)
                    continue;
                if (!_IsFirstLoadCaliData)
                {
                    ///放在此处更新的是原因
                    ///不知道是哪一个数据，或者该数据会影响到自校正数据导致自校正参数无效
                    if (type == CaliDataType.PhyChannel || type == CaliDataType.AnalogParams)
                    {
                        AutoCaliParams.Default?.UpDateAutoCaliParams();
                    }
                }
                switch (type)
                {
                    case CaliDataType.AnalogParams:
                    case CaliDataType.PhyChannelModel2:
                    case CaliDataType.PhyChannel:
                        AbstractController_AnalogChannel.Ctrl4094();
                        AbstractController_AnalogChannel.CtrlOffset();
                        AbstractController_AnalogChannel.CtrlGain();
                        AbstractController_AnalogChannel.ActiveChannged();
                        AbstractController_AnalogChannel.CtrlAnalogChannelSet();

                        break;
                    case CaliDataType.TiadcPhaseOffsetGainParams:
                        Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
                        break;
                    case CaliDataType.DbiAnalogParams:
                        //AbstractController_AnalogChannel.Ctrl4094();
                        //AbstractController_AnalogChannel.CtrlOffset();
                        //AbstractController_AnalogChannel.CtrlGain();
                        //AbstractController_AnalogChannel.ActiveChannged();
                        AbstractController_AnalogChannel.CtrlAnalogChannelSet();
                        break;
                    case CaliDataType.DbiLocalOscillators:
                        AbstractController_AnalogChannel.SwitchDBI_ASCII();
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
                        //LA比较电平
                        AbstractAcquirer_LA.Config();
                        break;
                }
            }
            _IsFirstLoadCaliData = false;
            CaliDataManager.DataChangedCaliDataType_Running.Clear();
        }

        private void configDspCoefficients()
        {
            CoefficientsTableSender_8000X.Send_IFCCoefficientsToAcqBoardByRegisterMode(false);
        }
    }
}
#endif

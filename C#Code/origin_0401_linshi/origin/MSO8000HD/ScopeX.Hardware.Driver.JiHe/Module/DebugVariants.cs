using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Unicode;
using System.Text.Json;
using System.Text.Encodings.Web;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System.Threading;
using ScopeX.Hardware.Driver.Registers.SendManage;

namespace ScopeX.Hardware.Driver
{
    internal class DebugVariants
    {
        #region Boolean Variants
        private bool _bEnable_AtStartScanAdcRxWindows = true;
        public bool bEnable_AtStartScanAdcRxWindows
        {
            get => _bEnable_AtStartScanAdcRxWindows;
            set
            {
                _bEnable_AtStartScanAdcRxWindows = value;
            }
        }
        private bool _bEnable_AtStartTrigScanWindow = true;
        public bool bEnable_AtStartTrigScanWindow
        {
            get => _bEnable_AtStartTrigScanWindow;
            set
            {
                _bEnable_AtStartTrigScanWindow = value;
                //if (value == false)
                //    Hd.CurrProduct?.AcqBd?.ScanProcboard2AcqBoardTrigWindow();
            }
        }
        private bool _bEnable_AutoFilter_Fpga = true;
        public bool bEnable_AutoFilter_Fpga
        {
            get => _bEnable_AutoFilter_Fpga;
            set
            {
                _bEnable_AutoFilter_Fpga = value;
            }
        }
        private bool _bEnable_AtStartTrigScanAcqProcBdLoopTime = true;
        public bool bEnable_AtStartTrigScanAcqProcBdLoopTime
        {
            get => _bEnable_AtStartTrigScanAcqProcBdLoopTime;
            set
            {
                _bEnable_AtStartTrigScanAcqProcBdLoopTime = value;
            }
        }
        private bool _bEnbaleFifoMode = false;
        public bool bEnbaleFifoMode
        {
            get => _bEnbaleFifoMode;
            set => _bEnbaleFifoMode = value;
        }
        private bool _bEnable_LA_GT_CDR_Hold = false;
        public bool bEnable_LA_GT_CDR_Hold
        {
            get => _bEnable_LA_GT_CDR_Hold;
            set
            {
                _bEnable_LA_GT_CDR_Hold = value;
#if LA
                if (value == false)
                    AbstractAcquirer_LA.LoadDiscardData();
                HdIO.WriteReg((uint)ProcBdReg.W.LA_GTRXCDRHOLD, _bEnable_LA_GT_CDR_Hold ? 1U : 0U);
#endif
            }
        }
        private bool _bEnableAnalogTemperatureCompensate = false;
        public bool bEnableAnalogTemperatureCompensate
        {
            get => _bEnableAnalogTemperatureCompensate;
            set => _bEnableAnalogTemperatureCompensate = value;
        }

        private bool _bEnableChannelDelay = true;
        public bool bEnable_ChannelDelay
        {
            get => _bEnableChannelDelay;
            set
            {
                _bEnableChannelDelay = value;
                if (Hd.CurrProduct?.AcqBd != null)
                {
                    //CtrlAnalogChannel_JiHe2d5G.CtrlChannelDelay();//????
                }
            }

        }

        /// <summary>
        /// 处理版触发使能状态
        /// </summary>
        private bool? _bEnable_DigitTrigger = null;
        public bool bEnable_DigitTrigger
        {
            get => _bEnable_DigitTrigger ?? false;
            set
            {
                if (value != _bEnable_DigitTrigger)
                {
                    _bEnable_DigitTrigger = value;
                    ConditionManager.ToolProcDigitalTrigEn = value;
                }
            }
        }

        /// <summary>
        /// 采集版触发丢点使能状态
        /// </summary>
        private bool? _bEnable_AcqDigitTrigger = null;
        public bool bEnable_AcqDigitTrigger
        {
            get => _bEnable_AcqDigitTrigger ?? false;
            set
            {
                if (value != _bEnable_AcqDigitTrigger)
                {
                    _bEnable_AcqDigitTrigger = value;
                    ConditionManager.ToolAcqDigitalTrigEn = value;
                }
            }
        }

        private bool _bEnable_AcqbdInterpolation = true;
        public bool bEnable_AcqbdInterpolation
        {
            get => _bEnable_AcqbdInterpolation;
            set
            {
                if (value != _bEnable_AcqbdInterpolation)
                {
                    _bEnable_AcqbdInterpolation = value;
                }
            }
        }
        private bool _bEnable_ProbdInterpolation = true;
        public bool bEnable_ProbdInterpolation
        {
            get => _bEnable_ProbdInterpolation;
            set
            {
                if (value != _bEnable_ProbdInterpolation)
                {
                    _bEnable_ProbdInterpolation = value;
                    ConditionManager.InterpEn = Hd.CurrDebugVarints.bEnable_ProbdInterpolation;
                }
            }
        }
        private bool _bEnable_AcqBd_Afc = true;
        public bool bEnable_AcqBd_Afc
        {
            get => _bEnable_AcqBd_Afc;
            set
            {
                if (value != _bEnable_AcqBd_Afc)
                {
                    _bEnable_AcqBd_Afc = value;
                    //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Fir_Enable, _bEnable_AcqBd_Afc ? 1U : 0);
                }
            }
        }
        private bool _bEnable_AcqBd_Pfc = true;
        public bool bEnable_AcqBd_Pfc
        {
            get => _bEnable_AcqBd_Pfc;
            set
            {
                if (value != _bEnable_AcqBd_Pfc)
                {
                    _bEnable_AcqBd_Pfc = value;
                    //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.IIRfilter_iir_filter_en, _bEnable_AcqBd_Pfc ? 1U : 0);
                }
            }
        }
        private bool _bEnable_CorrectTiAdc = true;
        public bool bEnable_CorrectTiAdc
        {
            get => _bEnable_CorrectTiAdc;
            set
            {
                if (value != _bEnable_CorrectTiAdc)
                {
                    _bEnable_CorrectTiAdc = value;
                }
            }
        }
        private bool _bEnable_Dbi_IntDelay = false;
        public bool bEnable_Dbi_IntDelay
        {
            get => _bEnable_Dbi_IntDelay;
            set
            {
                if (value != _bEnable_Dbi_IntDelay)
                {
                    _bEnable_Dbi_IntDelay = value;
                }
            }
        }
        private bool _bEnable_Dbi_LocalOscillatorCoef = true;
        public bool bEnable_Dbi_LocalOscillatorCoef
        {
            get => _bEnable_Dbi_LocalOscillatorCoef;
            set
            {
                if (value != _bEnable_Dbi_LocalOscillatorCoef)
                {
                    _bEnable_Dbi_LocalOscillatorCoef = value;
                }
            }
        }

        private bool _bEnable_Dbi_AntiImageCoef = true;
        public bool bEnable_Dbi_AntiImageCoef
        {
            get => _bEnable_Dbi_AntiImageCoef;
            set
            {
                if (value != _bEnable_Dbi_AntiImageCoef)
                {
                    _bEnable_Dbi_AntiImageCoef = value;
                }
            }
        }

        private bool _bEnable_Dbi_FractionaryDelayCoef = true;
        public bool bEnable_Dbi_FractionaryDelayCoef
        {
            get => _bEnable_Dbi_FractionaryDelayCoef;
            set
            {
                if (value != _bEnable_Dbi_FractionaryDelayCoef)
                {
                    _bEnable_Dbi_FractionaryDelayCoef = value;
                }
            }
        }

        private bool _bEnable_Dbi_OverlapPhaseFreqDelayCoef = true;
        public bool bEnable_Dbi_OverlapPhaseFreqDelayCoef
        {
            get => _bEnable_Dbi_OverlapPhaseFreqDelayCoef;
            set
            {
                if (value != _bEnable_Dbi_OverlapPhaseFreqDelayCoef)
                {
                    _bEnable_Dbi_OverlapPhaseFreqDelayCoef = value;
                }
            }
        }

        private bool _bEnable_Dbi_AmpFreqCoef = true;
        public bool bEnable_Dbi_AmpFreqCoef
        {
            get => _bEnable_Dbi_AmpFreqCoef;
            set
            {
                if (value != _bEnable_Dbi_AmpFreqCoef)
                {
                    _bEnable_Dbi_AmpFreqCoef = value;
                }
            }
        }
        private bool _bEnable_Dbi_PhaseFreqCoef = true;
        public bool bEnable_Dbi_PhaseFreqCoef
        {
            get => _bEnable_Dbi_PhaseFreqCoef;
            set
            {
                if (value != _bEnable_Dbi_PhaseFreqCoef)
                {
                    _bEnable_Dbi_PhaseFreqCoef = value;
                }
            }
        }
        private bool _bEnable_Dbi_MultiRadioInterpolation = true;
        public bool bEnable_Dbi_MultiRadioInterpolation
        {
            get => _bEnable_Dbi_MultiRadioInterpolation;
            set
            {
                if (value != _bEnable_Dbi_MultiRadioInterpolation)
                {
                    _bEnable_Dbi_MultiRadioInterpolation = value;
                }
            }
        }
        private bool _bEnable_Dbi_IsSubbandMergeMode = true;
        /// <summary>
        /// DBI项目，数据采集是处于子带模式还是通道模式
        /// </summary>
        public bool bEnable_Dbi_IsSubbandMergeMode
        {
            get => _bEnable_Dbi_IsSubbandMergeMode;
            set
            {
                if (value != _bEnable_Dbi_IsSubbandMergeMode)
                {
                    _bEnable_Dbi_IsSubbandMergeMode = value;
                }
            }
        }
        private bool _bEnable_Dbi_DebugMode = true;
        /// <summary>
        /// 只有此参数打开时，iDbi_DebugChannelID 才有意义。此参数与其他任何参数无关
        /// </summary>
        public bool bEnable_AdcDataDebugMode
        {
            get => _bEnable_Dbi_DebugMode;
            set
            {
                if (value != _bEnable_Dbi_DebugMode)
                {
                    _bEnable_Dbi_DebugMode = value;
                }
            }
        }
        private bool _bEnable_ProcBd_Average = false;
        public bool bEnable_ProcBd_Average
        {
            get => _bEnable_ProcBd_Average;
            set
            {
                _bEnable_ProcBd_Average = value || (Hd.UIMessage?.Timebase?.AcqMode ?? AnaChnlAcqMode.Normal) == AnaChnlAcqMode.Average;
            }
        }
        private bool _bEnable_ProcBd_CalcFrequency = false;
        public bool bEnable_ProcBd_CalcFrequency
        {
            get => _bEnable_ProcBd_CalcFrequency;
            set
            {
                if (value != _bEnable_ProcBd_CalcFrequency)
                {
                    _bEnable_ProcBd_CalcFrequency = value;
                }
            }
        }
        private Boolean _bEnable_SaveUpoPictureAtDriver = false;
        public bool bEnable_SaveUpoPictureAtDriver
        {
            get => _bEnable_SaveUpoPictureAtDriver;
            set
            {
                if (value != _bEnable_SaveUpoPictureAtDriver)
                {
                    _bEnable_SaveUpoPictureAtDriver = value;
                }
            }
        }
        private bool _bEnable_AdcINL = true;
        public bool bEnable_AdcINL
        {
            get => _bEnable_AdcINL;
            set
            {
                if (value != _bEnable_AdcINL)
                {
                    _bEnable_AdcINL = value;
                }
            }
        }
        private bool _bEnableAutoFanControl = true;
        public bool bEnableAutoFanControl
        {
            get => _bEnableAutoFanControl;
            set
            {
                if (value != _bEnableAutoFanControl)
                {
                    _bEnableAutoFanControl = value;
                }
            }
        }
        private bool _bEnable_CtrlGainByFpga = true;
        public bool bEnable_CtrlGainByFpga
        {
            get => _bEnable_CtrlGainByFpga;
            set
            {
                if (value != _bEnable_CtrlGainByFpga)
                {
                    _bEnable_CtrlGainByFpga = value;
                }
            }
        }
        private bool _bEnable_AdcConditionFilter = true;
        public bool bEnable_AdcConditionFilter
        {
            get => _bEnable_AdcConditionFilter;
            set
            {
                if (value != _bEnable_AdcConditionFilter)
                {
                    _bEnable_AdcConditionFilter = value;
                    //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ConditionFilter_Enable, value ? 1U : 0);
                }
            }
        }
        private bool _bEnable_AmplitudeTemperatureCompensate = false;
        public bool bEnable_AmplitudeTemperatureCompensate
        {
            get => _bEnable_AmplitudeTemperatureCompensate;
            set => _bEnable_AmplitudeTemperatureCompensate = value;
        }

        /// <summary>
        /// 板间同步状态
        /// </summary>
        private bool _bEnable_InterBoardSynchronizationMode = true;
        public bool bEnable_InterBoardSynchronizationMode
        {
            get => _bEnable_InterBoardSynchronizationMode;
            set
            {
                if (_bEnable_InterBoardSynchronizationMode != value)
                {
                    _bEnable_InterBoardSynchronizationMode = value;
                    //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, _bEnable_InterBoardSynchronizationMode ? 1U : 0);//双板同步触发选择
                }
            }
        }

        /// <summary>
        /// 晶振信号开关
        /// </summary>
        private bool _bEnable_OpenCrystal = false;
        public bool bEnable_OpenCrystal
        {
            get => _bEnable_OpenCrystal;
            set
            {
                if (_bEnable_OpenCrystal != value)
                {
                    _bEnable_OpenCrystal = value;
                    if (Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G)
                    {
                        CtrlAnalogChannel_3U8G.COMPort_AnalogChannelSet();
                    }
                    else
                    {
                        CtrlAnalogChannel_JiHe2d5G.COMPort_AnalogChannelSet();
                    }
                    if (_bEnable_OpenCrystal)
                    {
                        Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(0x00000001, 0xa << 8);
                    }
                    else
                    {
                        Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(0x00000000, 0xa << 8);
                    }
                }
            }
        }

        /// <summary>
        /// 是否发送默认系数
        /// </summary>
        private bool _bEnable_IFCDefalutCoefficientsParams = false;
        public bool bEnable_IFCDefalutCoefficientsParams
        {
            get => _bEnable_IFCDefalutCoefficientsParams;
            set
            {
                if (_bEnable_IFCDefalutCoefficientsParams != value)
                {
                    _bEnable_IFCDefalutCoefficientsParams = value;
                    Hd.CurrProduct?.AcqBd?.SendCoefficientsByRegisterMode_IFC(true);

                }
            }
        }

        /// <summary>
        /// 是否发送默认系数
        /// </summary>
        private bool _bEnable_ADCTI_DefalutCoefficientsParams = false;
        public bool bEnable_ADCTI_DefalutCoefficientsParams
        {
            get => _bEnable_ADCTI_DefalutCoefficientsParams;
            set
            {
                if (_bEnable_ADCTI_DefalutCoefficientsParams != value)
                {
                    _bEnable_ADCTI_DefalutCoefficientsParams = value;
                    //Hd.CurrProduct?.AcqBd?.SendCoefficientsByRegisterMode_IFC(true);

                    Hd.CurrProduct?.AcqBd?.SendCoefficientsByRegisterMode_ADCTI(true);
                }
            }
        }

        /// <summary>
        /// 是否打开DDr
        /// </summary>
        private bool _bEnable_IsOpenDDr = true;
        public bool bEnable_IsOpenDDr
        {
            get => _bEnable_IsOpenDDr;
            set
            {
                if (_bEnable_IsOpenDDr != value)
                {
                    _bEnable_IsOpenDDr = value;
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.LSCtrl_DdrDataSel, _bEnable_IsOpenDDr ? 1U : 0);
                }
            }
        }
        /// <summary>
        /// 是否打开幅度细调
        /// </summary>
        private Boolean _BEnable_DsoGainByFpgar = true;
        public Boolean BEnable_DsoGainByFpga
        {
            get => _BEnable_DsoGainByFpgar;
            set
            {
                if (_BEnable_DsoGainByFpgar != value)
                {
                    _BEnable_DsoGainByFpgar = value;
                }
            }
        }
        /// <summary>
        /// 是否打开幅度细调
        /// </summary>
        private Boolean _BEnable_ScanDsoGainByFpga = true;
        public Boolean BEnable_ScanDsoGainByFpga
        {
            get => _BEnable_ScanDsoGainByFpga;
            set
            {
                if (_BEnable_ScanDsoGainByFpga != value)
                {
                    _BEnable_ScanDsoGainByFpga = value;
                }
            }
        }

        private bool _bEnable_Dsp = true;
        public bool bEnable_Dsp
        {
            get => _bEnable_Dsp;
            set
            {
                if (value != _bEnable_Dsp)
                {
                    _bEnable_Dsp = value;
                    ConditionManager.ToolDspEn = value;
                    //if (value)
                    //{
                    //    HdIO.WriteReg(ProcBdReg.W.Dsp_CaliEnPro, 0x0f);
                    //}
                    //else
                    //{
                    //    HdIO.WriteReg(ProcBdReg.W.Dsp_CaliEnPro, 0x00);

                    //}

                }
            }
        }

        private bool _bEnable_Dsp_Pro = true;
        public bool bEnable_Dsp_Pro
        {
            get => _bEnable_Dsp_Pro;
            set
            {
                if (value != _bEnable_Dsp_Pro)
                {
                    _bEnable_Dsp_Pro = value;
                    ConditionManager.ToolDspProEn = value;
                    if (value)
                    {
                        HdIO.WriteReg(ProcBdReg.W.Dsp_CaliEnPro, 0x0f);
                    }
                    else
                    {
                        HdIO.WriteReg(ProcBdReg.W.Dsp_CaliEnPro, 0x00);

                    }
                }
            }
        }

        private bool _bEnable_ChannelSync = true;
        public bool bEnable_ChannelSync
        {
            get => _bEnable_ChannelSync;
            set
            {
                if (value != _bEnable_ChannelSync)
                {
                    _bEnable_ChannelSync = value;
                    if (_bEnable_ChannelSync==true)
                    {
                        HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayEn, 1);
                    }
                    else
                    {
                        HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayEn, 0);
                    }
                }
            }
        }

        private bool _bEnable_ChannelSync_Farrow = true;
        public bool bEnable_ChannelSync_Farrow
        {
            get => _bEnable_ChannelSync_Farrow;
            set
            {
                if (value != _bEnable_ChannelSync_Farrow)
                {
                    _bEnable_ChannelSync_Farrow = value;
                    if (_bEnable_ChannelSync_Farrow == true)
                    {
                        HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 1);
                    }
                    else
                    {
                        HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 0);
                    }
                }
            }
        }

        private bool _bEnable_bandwidth = true;
        public bool bEnable_bandwidth
        {
            get => _bEnable_bandwidth;
            set
            {
                if (value != _bEnable_bandwidth)
                {
                    _bEnable_bandwidth = value;
                    if (_bEnable_bandwidth == true)
                    {
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Channel_BandLimitEn, 1);
                    }
                    else
                    {
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Channel_BandLimitEn, 0);
                    }
                }
            }
        }

        private bool _bEnable_analog_signal = true;
        public bool bEnable_analog_signal
        {
            get => _bEnable_analog_signal;
            set
            {
                if (value != _bEnable_analog_signal)
                {
                    _bEnable_analog_signal = value;
                    if (_bEnable_analog_signal == true)
                    {
                        Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(0x00000001, 0xa << 8);
                    }
                    else
                    {
                        Hd.CurrProduct?.PcieBd?.SendCmdToCD4094(0x00000000, 0xa << 8);
                    }
                }
            }
        }

      

        private CancellationTokenSource _Cts;
        private UInt32[] _LastValue;
        private Boolean _BEnable_Reverse0 = false;
        public Boolean BEnable_Reverse0
        {
            get => _BEnable_Reverse0;
            set
            {
                if (value != _BEnable_Reverse0)
                {
                    _BEnable_Reverse0 = value;
                    if (_BEnable_Reverse0)
                    {
                        _Cts = new CancellationTokenSource();

                        //开启失锁检查任务
                        Task.Run(() =>
                        {
                            List<(AcqBdReg.R Reg, AcqBdNo Bd, String Alias)> regInfos = new()
                            {
                                (AcqBdReg.R.reverse_acq_reverse_rd_reg_0, AcqBdNo.B0, "caliLoseLock"),
                                (AcqBdReg.R.reverse_acq_reverse_rd_reg_1, AcqBdNo.B0, "pllLoseLock"),
                                (AcqBdReg.R.reverse_acq_reverse_rd_reg_0, AcqBdNo.B1, "caliLoseLock"),
                                (AcqBdReg.R.reverse_acq_reverse_rd_reg_1, AcqBdNo.B1, "pllLoseLock"),
                            };
                            _LastValue = new UInt32[4] { 1, 1, 1, 1 };
                            Hd.SysLogger?.Invoke("Start lose  lock check.", "Info");
                            while (!_Cts.Token.IsCancellationRequested)
                            {
                                for (int regId = 0; regId < regInfos.Count; regId++)
                                {
                                    var reginfo = regInfos[regId];
                                    var regval = Hd.CurrProduct!.AcqBd!.ReadReg(reginfo.Reg, reginfo.Bd);
                                    if (regval != _LastValue[regId])
                                    {
                                        _LastValue[regId] = regval;
                                        var C0 = (regval >> (2 * 5)) & 0b0001_1111;
                                        var C1 = (regval >> (1 * 5)) & 0b0001_1111;
                                        var C2 = (regval >> (0 * 5)) & 0b0001_1111;
                                        Hd.SysLogger?.Invoke($"AcqBdNo_{reginfo.Bd} : {reginfo.Alias} ," +
                                            $" C0 = {C0} , C1 = {C1} , C2 = {C2}", "Info");
                                    }
                                }
                                Thread.Sleep(1000);
                            }
                            _LastValue = null;
                            Hd.SysLogger?.Invoke("Cancel lose  lock check.", "Info");
                        }, _Cts.Token);
                    }
                    else
                    {
                        //取消失锁检查任务
                        _Cts?.Cancel();
                    }
                }
            }
        }

        #endregion
        #region
        private ChannelId _iDbi_DebugChannelID = ChannelId.C1;
        /// <summary>
        /// Debug 模式下观察的通道。0表示观察通道1,1表示观察通道2。
        /// </summary>
        public ChannelId iDbi_DebugChannelID
        {
            get => _iDbi_DebugChannelID;
            set
            {
                if (_iDbi_DebugChannelID != value)
                {
                    _iDbi_DebugChannelID = value;
                }
            }
        }

        #endregion
        #region Backup Adn Restore
        private bool bEnable_LA_GT_CDR_Hold_Backup = false;
        private bool bEnable_AmplitudeTemperatureCompensate_Backup = false;
        private bool bEnable_AdcDataDebugMode_Backup = false;
        private bool bEnable_DigitTrigger_Backup = false;
        private bool bEnable_AcqDigitTrigger_Backup = false;
        private bool bEnable_ChannelDelay_Backup = false;
        private bool bEnable_AcqbdInterpolation_Backup = false;
        private bool bEnable_ProbdInterpolation_Backup = false;
        private bool bEnable_CorrectTiAdc_Backup = false;
        private bool bEnable_AcqBd_Afc_Backup = false;
        private bool bEnable_AcqBd_Pfc_Backup = false;
        private bool bEnable_Dbi_IntDelay_Backup = false;
        private bool bEnable_Dbi_AmpFreqCoef_Backup = false;
        private bool bEnable_Dbi_AntiImageCoef_Backup = false;
        private bool bEnable_Dbi_FractionaryDelayCoef_Backup = false;
        private bool bEnable_Dbi_OverlapPhaseFreqDelayCoef_Backup = false;
        private bool bEnable_Dbi_LocalOscillatorCoef_Backup = false;
        private bool bEnable_Dbi_MultiRadioInterpolation_Backup = false;
        private bool bEnable_Dbi_PhaseFreqCoef_Backup = false;
        private bool bEnable_Dbi_IsSubbandMergeMode_Backup = false;
        private bool bEnable_ProcBd_Average_Backup = false;
        private bool bEnable_ProcBd_CalcFrequency_Backup = false;
        private ChannelId iDbi_DebugChannelID_Backup = 0;
        private Boolean bEnable_SaveUpoPictureAtDriver_Backup = false;
        private Boolean bEnable_CtrlGainByFpga_Backup = false;
        private Boolean _bEnable_AdcConditionFilter_Backup = false;
        private Boolean bEnableAutoFanControl_Backup = false;
        private bool bEnable_Dsp_Backup = false;
        private bool bEnable_Dsp_Pro_Backup = false;
        private bool bEnable_ChannelSync_Backup = false;
        private bool bEnable_ChannelSync_Farrow_Backup = false;
        private bool bEnable_bandwidth_Backup = false;
        private bool bEnable_analog_signal_Backup = false;
        private bool bEnable_AutoFilter_Fpga_Backup = false;
        public void DoBackup()
        {
            bEnable_LA_GT_CDR_Hold_Backup = this.bEnable_LA_GT_CDR_Hold;
            bEnable_AmplitudeTemperatureCompensate_Backup = this.bEnableAnalogTemperatureCompensate;
            bEnable_AdcDataDebugMode_Backup = this.bEnable_AdcDataDebugMode;
            bEnable_DigitTrigger_Backup = this.bEnable_DigitTrigger;
            bEnable_AutoFilter_Fpga_Backup = this.bEnable_AutoFilter_Fpga;
            bEnable_AcqDigitTrigger_Backup = this.bEnable_AcqDigitTrigger;
            bEnable_ChannelDelay_Backup = this.bEnable_ChannelDelay;
            bEnable_AcqbdInterpolation_Backup = this.bEnable_AcqbdInterpolation;
            bEnable_ProbdInterpolation_Backup = this.bEnable_ProbdInterpolation;
            bEnable_CorrectTiAdc_Backup = this.bEnable_CorrectTiAdc;
            bEnable_AcqBd_Afc_Backup = this.bEnable_AcqBd_Afc;
            bEnable_AcqBd_Pfc_Backup = this.bEnable_AcqBd_Pfc;
            bEnable_Dbi_IntDelay_Backup = this.bEnable_Dbi_IntDelay;
            bEnable_Dbi_AmpFreqCoef_Backup = this.bEnable_Dbi_AmpFreqCoef;
            bEnable_Dbi_AntiImageCoef_Backup = this.bEnable_Dbi_AntiImageCoef;
            bEnable_Dbi_FractionaryDelayCoef_Backup = this.bEnable_Dbi_FractionaryDelayCoef;
            bEnable_Dbi_OverlapPhaseFreqDelayCoef_Backup = this.bEnable_Dbi_OverlapPhaseFreqDelayCoef;
            bEnable_Dbi_LocalOscillatorCoef_Backup = this.bEnable_Dbi_LocalOscillatorCoef;
            bEnable_Dbi_MultiRadioInterpolation_Backup = this.bEnable_Dbi_MultiRadioInterpolation;
            bEnable_Dbi_PhaseFreqCoef_Backup = this.bEnable_Dbi_PhaseFreqCoef;
            bEnable_Dbi_IsSubbandMergeMode_Backup = this.bEnable_Dbi_IsSubbandMergeMode;
            bEnable_ProcBd_Average_Backup = this.bEnable_ProcBd_Average;
            bEnable_ProcBd_CalcFrequency_Backup = this.bEnable_ProcBd_CalcFrequency;
            bEnable_SaveUpoPictureAtDriver_Backup = bEnable_SaveUpoPictureAtDriver;
            iDbi_DebugChannelID_Backup = this.iDbi_DebugChannelID;
            bEnable_CtrlGainByFpga_Backup = this.bEnable_CtrlGainByFpga;
            _bEnable_AdcConditionFilter_Backup = this._bEnable_AdcConditionFilter;
            bEnableAutoFanControl_Backup = this.bEnableAutoFanControl;
            bEnable_Dsp_Backup = this.bEnable_Dsp;
            bEnable_Dsp_Pro_Backup = this.bEnable_Dsp_Pro;
            bEnable_ChannelSync_Backup = this.bEnable_ChannelSync;
            bEnable_ChannelSync_Farrow_Backup = this.bEnable_ChannelSync_Farrow;
            bEnable_bandwidth_Backup = this.bEnable_bandwidth;
            bEnable_analog_signal_Backup = this.bEnable_analog_signal;
        }
        public void DoRestore()
        {
            bEnable_LA_GT_CDR_Hold = bEnable_LA_GT_CDR_Hold_Backup;
            bEnableAnalogTemperatureCompensate = bEnable_AmplitudeTemperatureCompensate_Backup;
            bEnable_AdcDataDebugMode = bEnable_AdcDataDebugMode_Backup;
            bEnable_DigitTrigger = bEnable_DigitTrigger_Backup;
            bEnable_AcqDigitTrigger = bEnable_AcqDigitTrigger_Backup;
            bEnable_ChannelDelay = bEnable_ChannelDelay_Backup;
            bEnable_AutoFilter_Fpga = bEnable_AutoFilter_Fpga_Backup;

            bEnable_AcqbdInterpolation = bEnable_AcqbdInterpolation_Backup;
            bEnable_CorrectTiAdc = bEnable_CorrectTiAdc_Backup;
            bEnable_AcqBd_Afc = bEnable_AcqBd_Afc_Backup;
            bEnable_AcqBd_Pfc = bEnable_AcqBd_Pfc_Backup;
            bEnable_Dbi_IntDelay = bEnable_Dbi_IntDelay_Backup;
            bEnable_Dbi_AmpFreqCoef = bEnable_Dbi_AmpFreqCoef_Backup;
            bEnable_Dbi_AntiImageCoef = bEnable_Dbi_AntiImageCoef_Backup;
            bEnable_Dbi_FractionaryDelayCoef = bEnable_Dbi_FractionaryDelayCoef_Backup;
            bEnable_Dbi_OverlapPhaseFreqDelayCoef = bEnable_Dbi_OverlapPhaseFreqDelayCoef_Backup;
            bEnable_Dbi_LocalOscillatorCoef = bEnable_Dbi_LocalOscillatorCoef_Backup;
            bEnable_Dbi_MultiRadioInterpolation = bEnable_Dbi_MultiRadioInterpolation_Backup;
            bEnable_Dbi_PhaseFreqCoef = bEnable_Dbi_PhaseFreqCoef_Backup;
            bEnable_Dbi_IsSubbandMergeMode = bEnable_Dbi_IsSubbandMergeMode_Backup;
            bEnable_ProcBd_Average = bEnable_ProcBd_Average_Backup;
            bEnable_ProcBd_CalcFrequency = bEnable_ProcBd_CalcFrequency_Backup;
            bEnable_SaveUpoPictureAtDriver = bEnable_SaveUpoPictureAtDriver_Backup;
            iDbi_DebugChannelID = iDbi_DebugChannelID_Backup;

            bEnable_CtrlGainByFpga = bEnable_CtrlGainByFpga_Backup;
            bEnable_AdcConditionFilter = _bEnable_AdcConditionFilter_Backup;
            bEnableAutoFanControl = bEnableAutoFanControl_Backup;
            bEnable_Dsp = bEnable_Dsp_Backup;
            bEnable_Dsp_Pro = bEnable_Dsp_Pro_Backup;
            bEnable_ChannelSync = bEnable_ChannelSync_Backup;
            bEnable_ChannelSync_Farrow = bEnable_ChannelSync_Farrow_Backup;
            bEnable_bandwidth = bEnable_bandwidth_Backup;
            bEnable_analog_signal = bEnable_analog_signal_Backup;
        }
        #endregion
        public string StringValue
        {
            get
            {
                string str = $@"
                    bEnable_AtStartTrigScanWindow:{bEnable_AtStartTrigScanWindow},
                    bEnable_AtStartTrigScanAcqProcBdLoopTime:{bEnable_AtStartTrigScanAcqProcBdLoopTime},
                    bEnable_LA_GT_CDR_Hold:{bEnable_LA_GT_CDR_Hold},
                    bEnableAnalogTemperatureCompensate:{bEnableAnalogTemperatureCompensate},
                    bEnable_AdcDataDebugMode:{this.bEnable_AdcDataDebugMode},
                    bEnable_DigitTrigger:{this.bEnable_DigitTrigger},
                    bEnable_AcqDigitTrigger:{this.bEnable_AcqDigitTrigger},
                    bEnable_ChannelDelay:{this.bEnable_ChannelDelay},
                    bEnable_AcqbdInterpolation:{this.bEnable_AcqbdInterpolation},
                    bEnable_ProbdInterpolation:{this.bEnable_ProbdInterpolation},
                    bEnable_CorrectTiAdc:{this.bEnable_CorrectTiAdc},
                    bEnable_AcqBd_Afc: {this.bEnable_AcqBd_Afc},
                    bEnable_AcqBd_Pfc:{this.bEnable_AcqBd_Pfc},
                    bEnable_Dbi_IntDelay:{this.bEnable_Dbi_IntDelay},
                    bEnable_Dbi_AmpFreqCoef:{this.bEnable_Dbi_AmpFreqCoef},
                    bEnable_Dbi_AntiImageCoef:{this.bEnable_Dbi_AntiImageCoef},
                    bEnable_Dbi_FractionaryDelayCoef:{this.bEnable_Dbi_FractionaryDelayCoef},
                    bEnable_Dbi_OverlapPhaseFreqDelayCoef:{this.bEnable_Dbi_OverlapPhaseFreqDelayCoef},
                    bEnable_Dbi_LocalOscillatorCoef:{this.bEnable_Dbi_LocalOscillatorCoef},
                    bEnable_Dbi_MultiRadioInterpolation:{this.bEnable_Dbi_MultiRadioInterpolation},
                    bEnable_Dbi_PhaseFreqCoef:{this.bEnable_Dbi_PhaseFreqCoef},
                    bEnable_Dbi_IsSubbandMergeMode:{this.bEnable_Dbi_IsSubbandMergeMode},
                    bEnable_ProcBd_Average:{this.bEnable_ProcBd_Average},
                    bEnable_ProcBd_CalcFrequency:{this.bEnable_ProcBd_CalcFrequency},
                    iDbi_DebugChannelID:{(int)this.iDbi_DebugChannelID},
                    bEnable_SaveUpoPictureAtDriver:{this.bEnable_SaveUpoPictureAtDriver},
                    bEnable_ENABLE_DEBUG:{Constants.ENABLE_DEBUG},
                    bEnable_AdcINL:{this.bEnable_AdcINL},
                    bEnableAutoFanControl:{this.bEnableAutoFanControl},
                    bEnable_CtrlGainByFpga:{this.bEnable_CtrlGainByFpga},
                    bEnable_AdcConditionFilter:{this.bEnable_AdcConditionFilter},
                    bEnable_InterBoardSynchronizationMode:{this.bEnable_InterBoardSynchronizationMode},
                    bEnable_OpenCrystal:{this.bEnable_OpenCrystal},
                    bEnable_IFCDefalutCoefficientsParams:{this.bEnable_IFCDefalutCoefficientsParams},
                    bEnable_IsOpenDDr:{this.bEnable_IsOpenDDr},
                    BEnable_DsoGainByFpga:{this.BEnable_DsoGainByFpga},
                    BEnable_ScanDsoGainByFpga:{this.BEnable_ScanDsoGainByFpga},
                    bEnable_Dsp:{this.bEnable_Dsp},
                    bEnable_Dsp_Pro:{this.bEnable_Dsp_Pro},                    
                    bEnable_ChannelSync:{this.bEnable_ChannelSync},
                    bEnable_ChannelSync_Farrow:{this.bEnable_ChannelSync_Farrow},
                    bEnable_bandwidth:{this.bEnable_bandwidth},
                    bEnable_analog_signal:{this.bEnable_analog_signal},
                    bEnable_AutoFilter_Fpga:{this.bEnable_AutoFilter_Fpga},
                    BEnable_Reverse0:{this.BEnable_Reverse0},
                    ";
                return str;
            }
            set
            {
                string[] nameValuePairList = value.Split(',');
                string trueStr = "TRUE";
                foreach (string nameValuePair in nameValuePairList)
                {
                    if (nameValuePair.Trim() == "")
                        continue;
                    string[] nameValue = nameValuePair.Split(':');
                    if (nameValue.Length < 2)
                        continue;
                    string valueStr = nameValue[1].Trim().ToUpper();
                    switch (nameValue[0].Trim())
                    {
                        case "bEnable_AtStartTrigScanWindow":
                            bEnable_AtStartTrigScanWindow = valueStr == trueStr;
                            break;
                        case "bEnable_AtStartTrigScanAcqProcBdLoopTime":
                            bEnable_AtStartTrigScanAcqProcBdLoopTime = valueStr == trueStr;
                            break;
                        case "bEnable_LA_GT_CDR_Hold":
                            bEnable_LA_GT_CDR_Hold = valueStr == trueStr;
                            break;
                        case "bEnableAnalogTemperatureCompensate":
                            bEnableAnalogTemperatureCompensate = valueStr == trueStr;
                            break;
                        case "bEnable_DigitTrigger":
                            bEnable_DigitTrigger = valueStr == trueStr;
                            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Location_TrigDiscardColumnEn, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 1U : 0);
                            break;
                        case "bEnable_AcqDigitTrigger":
                            bEnable_AcqDigitTrigger = valueStr == trueStr;
                            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Location_TrigDiscardColumnEn, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 1U : 0);
                            break;
                        case "bEnable_ChannelDelay":
                            bEnable_ChannelDelay = valueStr == trueStr;
                            break;
                        case "bEnable_AcqbdInterpolation":
                            bEnable_AcqbdInterpolation = valueStr == trueStr;
                            break;
                        case "bEnable_ProbdInterpolation":
                            bEnable_ProbdInterpolation = valueStr == trueStr;
                            break;
                        case "bEnable_AcqBd_Afc":
                            bEnable_AcqBd_Afc = valueStr == trueStr;
                            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Fir_Enable, Hd.CurrDebugVarints.bEnable_AcqBd_Afc ? 1U : 0);
                            break;
                        case "bEnable_AcqBd_Pfc":
                            bEnable_AcqBd_Pfc = valueStr == trueStr;
                            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.IIRfilter_iir_filter_en, Hd.CurrDebugVarints.bEnable_AcqBd_Pfc ? 1U : 0);
                            break;
                        case "bEnable_CorrectTiAdc":
                            bEnable_CorrectTiAdc = valueStr == trueStr;
                            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TIADC_Enable, Hd.CurrDebugVarints.bEnable_CorrectTiAdc ? 1U : 0);
                            break;
                        case "bEnable_SaveUpoPictureAtDriver":
                            bEnable_SaveUpoPictureAtDriver = valueStr == trueStr;
                            break;
                        case "bEnable_AdcDataDebugMode"://在Debug模式下，显示的是4个通道，可以分别调试通道1和通道2
                            bEnable_AdcDataDebugMode = valueStr == trueStr;
                            if (!bEnable_AdcDataDebugMode)
                            {
                                //bEnable_DigitTrigger = true;
                                //bEnable_AcqDigitTrigger = true;
                                ////bEnable_ChannelDelay = true;
                                //bEnable_AcqbdInterpolation = true;
                                //bEnable_ProbdInterpolation = true;
                                //bEnable_AcqBd_Afc = true;
                                //bEnable_AcqBd_Pfc = true;
                                //bEnable_CorrectTiAdc = true;
                                //bEnable_Dsp = true;
                                //bEnable_Dsp_Pro = true;
                                //bEnable_ChannelSync = true;
                                //bEnable_ChannelSync_Farrow = true;
                                //bEnable_bandwidth = true;
                                //bEnable_analog_signal = false;

                                //bEnable_Dbi_AmpFreqCoef = true;
                                //bEnable_Dbi_AntiImageCoef = true;
                                //bEnable_Dbi_FractionaryDelayCoef = true;
                                //bEnable_Dbi_MultiRadioInterpolation = true;
                                //bEnable_Dbi_OverlapPhaseFreqDelayCoef = true;
                                //bEnable_Dbi_PhaseFreqCoef = true;
                                //bEnable_ProcBd_Average = true;
                                //bEnable_Dbi_IsSubbandMergeMode = true;
                                //bEnable_Dbi_IntDelay = true;
                            }

                            break;
                        case "bEnable_Dbi_AmpFreqCoef":
                            bEnable_Dbi_AmpFreqCoef = valueStr == trueStr;
                            break;
                        case "bEnable_Dbi_AntiImageCoef":
                            bEnable_Dbi_AntiImageCoef = valueStr == trueStr;
                            break;
                        case "bEnable_Dbi_FractionaryDelayCoef":
                            bEnable_Dbi_FractionaryDelayCoef = valueStr == trueStr;
                            break;
                        case "bEnable_Dbi_LocalOscillatorCoef":
                            bEnable_Dbi_LocalOscillatorCoef = valueStr == trueStr;
                            break;
                        case "bEnable_Dbi_MultiRadioInterpolation":
                            bEnable_Dbi_MultiRadioInterpolation = valueStr == trueStr;
                            break;
                        case "bEnable_Dbi_OverlapPhaseFreqDelayCoef":
                            bEnable_Dbi_OverlapPhaseFreqDelayCoef = valueStr == trueStr;
                            break;
                        case "bEnable_Dbi_PhaseFreqCoef":
                            bEnable_Dbi_PhaseFreqCoef = valueStr == trueStr;
                            break;
                        case "bEnable_Dbi_IsSubbandMergeMode":
                            bEnable_Dbi_IsSubbandMergeMode = valueStr == trueStr;
                            break;
                        case "bEnable_Dbi_IntDelay":
                            bEnable_Dbi_IntDelay = valueStr == trueStr;
                            break;
                        case "bEnable_ProcBd_Average":
                            bEnable_ProcBd_Average = valueStr == trueStr;
                            break;
                        case "bEnable_ProcBd_CalcFrequency":
                            bEnable_ProcBd_CalcFrequency = valueStr == trueStr;
                            break;
                        case "iDbi_DebugChannelID":
                            iDbi_DebugChannelID = (ChannelId)int.Parse(valueStr);
                            break;
                        case "bEnbaleFifoMode":
                            bEnbaleFifoMode = valueStr == trueStr;
                            Hd.LocalCommands |= (long)HdCmd.TmbStorageLen;
                            break;
                        case "bEnable_AtStartScanAdcRxWindows":
                            bEnable_AtStartScanAdcRxWindows = valueStr == trueStr;
                            break;

                        case "bEnableAutoFanControl":
                            bEnableAutoFanControl = valueStr == trueStr;
                            break;
                        case "bEnable_ENABLE_DEBUG":
                            Constants.Set("ENABLE_DEBUG", valueStr);
                            break;

                        case "bEnable_AdcINL":
                            bEnable_AdcINL = valueStr == trueStr;
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ADC_INL_ModuleEn, bEnable_AdcINL ? 1U : 0);
                            break;
                        case "bEnable_CtrlGainByFpga":
                            bEnable_CtrlGainByFpga = valueStr == trueStr;
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Acq_NoiseControl, bEnable_CtrlGainByFpga ? 1U : 0);
                            break;
                        case "bEnable_AdcConditionFilter":
                            bEnable_AdcConditionFilter = valueStr == trueStr;
                            break;
                        case "bEnable_InterBoardSynchronizationMode":
                            bEnable_InterBoardSynchronizationMode = valueStr == trueStr;
                            break;
                        case "bEnable_OpenCrystal":
                            bEnable_OpenCrystal = valueStr == trueStr;
                            break;
                        case "bEnable_IFCDefalutCoefficientsParams":
                            bEnable_IFCDefalutCoefficientsParams = valueStr == trueStr;
                            break;
                        case "bEnable_IsOpenDDr":
                            bEnable_IsOpenDDr = valueStr == trueStr;
                            break;
                        case "BEnable_DsoGainByFpga":
                            BEnable_DsoGainByFpga = valueStr == trueStr;
                            break;

                        case "BEnable_ScanDsoGainByFpga":
                            BEnable_ScanDsoGainByFpga = valueStr == trueStr;
                            break;
                        case "bEnable_Dsp":
                            bEnable_Dsp = valueStr == trueStr;
                            break;
                        case "bEnable_Dsp_Pro":
                            bEnable_Dsp_Pro = valueStr == trueStr;
                            break;
                        case "bEnable_ChannelSync":
                            bEnable_ChannelSync = valueStr == trueStr;
                            break;
                        case "bEnable_ChannelSync_Farrow":
                            bEnable_ChannelSync_Farrow = valueStr == trueStr;
                            break;
                        case "bEnable_bandwidth":
                            bEnable_bandwidth = valueStr == trueStr;
                            break;
                        case "bEnable_analog_signal":
                            bEnable_analog_signal = valueStr == trueStr;
                            break;
                        case "BEnable_Reverse0":
                            BEnable_Reverse0 = valueStr == trueStr;
                            break;
                        case "bEnable_AutoFilter_Fpga":
                            bEnable_AutoFilter_Fpga = valueStr == trueStr;
                            break;
                    }
                }

                Hd.LocalCommands |= (long)HdCmd.CaliDataChanged;//压栈，从而调用CreateAcquireAttribute,进行特殊的配置
            }
        }

        #region 初始化寄存器
        public void InitDebugVariants()
        {
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.LSCtrl_DdrDataSel, bEnable_IsOpenDDr ? 1U : 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, bEnable_InterBoardSynchronizationMode ? 1U : 0);//双板同步触发选择
        }
        #endregion
    }
}

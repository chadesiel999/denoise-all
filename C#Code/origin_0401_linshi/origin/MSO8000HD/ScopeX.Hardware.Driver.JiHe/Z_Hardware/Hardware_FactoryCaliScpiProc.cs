using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
//[assembly: InternalsVisibleToAttribute("ScopeX.Core")]
namespace ScopeX.Hardware.Driver
{
    public static partial class HdSpecial
    {
        public static UInt32 SyncCali = 0;
        internal static UInt32 RecvDbiCoefficientsCnt = 0;
        internal static UInt32 RecvAutoCaliAtInitCnt = 1;
        public static UInt32[] Chnl_SyncDiscarddots2FPGA = new UInt32[4];
        public static UInt32 OnlyCaliTiCnt = 0;
        #region bool 变量
        private static readonly string[] FactoryCaliLogicValueParams =
        { "DigitTrigger,on", "DigitTrigger,off",
          "AdcTestMode,on", "AdcTestMode,off",
          "AdcFlashMode,on","AdcFlashMode,off",
          "AdjustGainByTemperature,on","AdjustGainByTemperature,off",
          "UsingMatlab,on","UsingMatlab,off",
          "bForceReFind5200AdcSyncWindow,on","bForceReFind5200AdcSyncWindow,off",
          "bAcqStatisticsRunning,on","bAcqStatisticsRunning,off",
          "bAFCOpened,on","bAFCOpened,off",
          "bTiAdcOpened,on","bTiAdcOpened,off",
          "bDBIOpened,on","bDBIOpened,off",
        };
        public static bool FactoryCaliScpiProc_LogicValue_Set(string param)
        {
            if (!FactoryCaliLogicValueParams.Contains<string>(param))
                return false;
            string[] pair = param.Split(',');
            if (pair.Length < 2)
                return false;
            bool resultOk = true;
            switch (pair[0])
            {
                case "AdcTestMode":
                    Hd.bAdcAtTestMode = (pair[1] == "on");
                    break;
                case "AdcFlashMode":
                    Hd.bAdcAtFlashMode = (pair[1] == "on");
                    break;
                case "AdjustGainByTemperature":
                    Hd.bAdjustGainByTemperature = (pair[1] == "on");
                    Hd.LocalCommands |= (long)HdCmd.ChnlGain;
                    break;
                case "bAcqStatisticsRunning":
                    Hd.AcqStatisticsRunning = (pair[1] == "on");
                    break;
                default:
                    resultOk = false;
                    break;
            }

            return resultOk;
        }
        private static string BooleanToString(Boolean v)
        {
            return (v ? "on" : "off");
        }
        public static string FactoryCaliScpiProc_LogicValue_Get()
        {
            return $"DigitTrigger:{BooleanToString(Hd.CurrDebugVarints.bEnable_DigitTrigger)}," +
                    //$"ChannelDelay:{BooleanToString(Hd.CurrDebugVarints.bEnable_ChannelDelay)}," +
                    $"AdcTestMode:{BooleanToString(Hd.bAdcAtTestMode)}," +
                    $"AdcFlashMode:{BooleanToString(Hd.bAdcAtFlashMode)}," +
                    $"AdjustGainByTemperature:{BooleanToString(Hd.bAdjustGainByTemperature)}," +
                    $"bAcqStatisticsRunning:{BooleanToString(Hd.AcqStatisticsRunning)}," +
                    $"bAFCOpened:{BooleanToString(Hd.CurrDebugVarints.bEnable_Dsp)}," +
                    $"bDBIOpened:{BooleanToString(Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode)}," +
                    $"bTiAdcOpened:{BooleanToString(Hd.CurrDebugVarints.bEnable_Dsp)}";

        }
        #endregion
        #region 系统全局变量
        private static string GetComModelConstData()
        {
            StringBuilder sb = new StringBuilder();
            int index = 0;
            if (Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings.Count > 0)
            {
                sb.Append("CoefficientsTableTypeDefine_");
                foreach (var kvp in Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings)
                {
                    if (index != 0)
                        sb.Append($",");
                    sb.Append($"{((int)kvp.Key).ToString()}:{kvp.Value.Name}");

                    index++;
                }
            }
            string AcqBdNoChannelCorrespondence = Hd.CurrProduct.ProductType switch
            {
                ProductType.B21_DBI20G => "01230123",//AcqB1、AcqB2、AcqB3、AcqB4、AcqB5、AcqB6、AcqB7、AcqB8对应的Channel的数据,要校准5678板，需要通过通道3输入数据（在DebugMode 下，设置Debug通道为通道3）
                _ => "00112233",//因为只有AcqB1、AcqB3、AcqB5、AcqB7
            };

            var chnlDotsCount = AcqedDataPool.AnalogChData.AllChannelData[0]?.ToArray().Length;
            int adcDotsCountRatio = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquedParameters.AdcInterleaveMode switch
            {
                AdcInterleaveMode.Mode4To1 => 4,
                AdcInterleaveMode.Mode2To1 => 2,
                _ => 1,//AdcInterleaveMode.Mode1To1
            };

            return $"HardwareAttached:1" +
                $"|ProductType:{Hd.CurrProductType.ToString()}" +
                $"|AnaChnlType:{(int)Constants.ANA_CHNL_TYPE}" +
                $"|AdcBits:{Constants.ADC_BITS}" +
                $"|AdcCount:{Constants.ADC_NUM * Constants.ACQ_BOARD_NUM}" +
                $"|AnalogChannelCount:{ChannelIdExt.AnaChnlNum}" +
                $"|PerAnaChannelAdcCount:{Constants.ADC_NUM}" +
                $"|PerAdcCoreCount:{Constants.ADC_CORE_NUM}" +
                $"|PerAnaChannelDataCount:{chnlDotsCount}" +
                $"|PerAdcCoreDataCount:{chnlDotsCount / adcDotsCountRatio}" +
                $"|AnalogChannelType:{(int)AbstractController_AnalogChannel.ChannelModel}" +
                $"|SAMPS_PER_YDIV:{Constants.SAMPS_PER_YDIV}" +
                $"|AcqBdNoChannelCorrespondence:{AcqBdNoChannelCorrespondence}" +
                //$"|AnalogChannelLevelMin:{(int)Hd.CurrHdMessage.Analog[0].ScaleMinIndex}" +
                //$"|AnalogChannelLevelMax:{(int)ChannelIdExt.MaxAChId}" +
                $"|{sb.ToString()}";
        }
        #endregion

        #region SpecialData
        private static bool LoadCaliDataFromFlash()
        {
            return false;
        }
        private static bool SaveCaliData2Flash()
        {
            return false;
        }
        private static void procDbiCoefficientsTables(string param)
        {
            string[] setting = param.Split('_');
            DbiCoefficientsTablesType type = (DbiCoefficientsTablesType)Int32.Parse(setting[0]);
            BandMode _BandMode = (BandMode)Int32.Parse(setting[1]);
            ChannelId _ChannelID = (ChannelId)Int32.Parse(setting[2]);
            int _SubbandIndex = Int32.Parse(setting[3]);
            FilterbandMode _FilterbandMode = (FilterbandMode)Int32.Parse(setting[4]);
            if (type == DbiCoefficientsTablesType.LocalOscillatorCoefficients)
            {
                var localOscillatorItems = Acquirer_AnalogChanel_DBI13G.AcqBoardCoefficientsTablesSendDefine[type].Where(o => o.ChnlScaleIndex == (AnaChnlScaleIndex)Hd.UIMessage!.Analog![Int32.Parse(setting[2])].ScaleIndex).ToArray();
                foreach (var localOscillatorItem in localOscillatorItems) 
                {
                    DBI_CoefTableSendItem item = new DBI_CoefTableSendItem() { BandMode = _BandMode, ChannelID = _ChannelID, FilterbandMode = _FilterbandMode, SubbandIndex = localOscillatorItem.SubbandIndex, DataFileName = localOscillatorItem.DataFileName };
                    item.ChnlScaleIndex = (AnaChnlScaleIndex)Hd.UIMessage!.Analog![Int32.Parse(setting[2])].ScaleIndex;
                    lock (CaliDataManager.DbiDataChangedLocker)
                    {
                        if (!CaliDataManager.DataChangedDbiCoefficientsTablesType.ContainsKey(type))
                        {
                            List<DBI_CoefTableSendItem> list = new List<DBI_CoefTableSendItem>();
                            CaliDataManager.DataChangedDbiCoefficientsTablesType.Add(type, list);
                        }
                        CaliDataManager.DataChangedDbiCoefficientsTablesType[type].Add(item);
                    }
                }
            }
            else 
            {
                var fileName = Acquirer_AnalogChanel_DBI13G.AcqBoardCoefficientsTablesSendDefine[type].Where(o => o.ChnlScaleIndex == (AnaChnlScaleIndex)Hd.UIMessage!.Analog![Int32.Parse(setting[2])].ScaleIndex && o.SubbandIndex == Int32.Parse(setting[1])).First().DataFileName;
                DBI_CoefTableSendItem item = new DBI_CoefTableSendItem() { BandMode = _BandMode, ChannelID = _ChannelID, FilterbandMode = _FilterbandMode, SubbandIndex = _SubbandIndex, DataFileName = fileName };
                item.ChnlScaleIndex = (AnaChnlScaleIndex)Hd.UIMessage!.Analog![Int32.Parse(setting[2])].ScaleIndex;
                lock (CaliDataManager.DbiDataChangedLocker)
                {
                    if (!CaliDataManager.DataChangedDbiCoefficientsTablesType.ContainsKey(type))
                    {
                        List<DBI_CoefTableSendItem> list = new List<DBI_CoefTableSendItem>();
                        CaliDataManager.DataChangedDbiCoefficientsTablesType.Add(type, list);
                    }
                    CaliDataManager.DataChangedDbiCoefficientsTablesType[type].Add(item);
                }
            }


           
        }
        private static HdCmd FactoryCaliScpiProc_SpecialData_Set_OtherProcess(int commasIndex, string key, string value)
        {
            return key.StartsWith("CalibAWG") ? AWG.ExecSetCommand(commasIndex, key, value) : HdCmd.None;
        }
        public static HdCmd FactoryCaliScpiProc_SpecialData_Set(string param)
        {
            int commasIndex = param.IndexOf(',');
            string key = "";
            string value = "";
            if (commasIndex < 0)
            {
                key = param.Trim();
            }
            else
            {
                key = param.Substring(0, commasIndex).Trim();
                value = param.Substring(commasIndex + 1).Trim();
            }
            if (key == "")
                return HdCmd.None;
            try
            {
                switch (key)
                {
                    case "CaliDataChanged":
                        {
                            string[] otherParams = value.Split(",");
                            CaliDataType caliDataType = (CaliDataType)Enum.Parse(typeof(CaliDataType), otherParams[0].Trim());
                            switch (caliDataType)
                            {
                                case CaliDataType.CoefficientsTables:
                                    CoefficientsTableType coefficientsTableType = (CoefficientsTableType)Enum.Parse(typeof(CoefficientsTableType), otherParams[1].Trim());
                                    CaliDataManager.DataChangedCoefficientsTableType.Add(coefficientsTableType);
                                    CaliDataManager.DataChangedCaliDataType.Add(CaliDataType.CoefficientsTables);
                                    Hd.LocalCommands |= (Int64)HdCmd.CaliDataChanged;
                                    return HdCmd.CaliDataChanged;
                            }
                        }
                        break;
                    case "DbiCoefficientsTables":
                        if (value != "")
                        {
                            procDbiCoefficientsTables(value);
                            return HdCmd.CaliDataChanged;
                        }
                        return HdCmd.None;
                    case "DbiCoefficientsTableChanged":
                        if (value != "")
                        {
                            Hd.CurrProduct?.Acquirer_AnalogChannel?.InitAmpCoefficientFile();
                            CaliDataManager.LastChangedDataType = (DbiCoefficientsTablesType)int.Parse(value);
                            return HdCmd.None;
                        }
                        return HdCmd.None;
                    case "DebugVariant":
                        if (value != "")
                        {
                            Hd.CurrDebugVarints.StringValue = value.Trim();
                            return HdCmd.TmbScaleIndex | HdCmd.TmbPosition | HdCmd.TmbStorageLen | HdCmd.ChnlScaleIndex;
                        }
                        return HdCmd.None;
                    case "Message2AcquirerAnalogChannel":
                        if (value != "")
                        {
                            Hd.CurrProduct?.Acquirer_AnalogChannel?.InitAmpCoefficientFile();
                            return Hd.CurrProduct!.Acquirer_AnalogChannel!.ResponseSpecialScpiCmd(value);
                        }
                        else
                            return HdCmd.None;
                    case "FlashCaliData":
                        SaveCaliData2Flash();
                        return HdCmd.None;
                    case "FanSpeed":
                        if (value != "")
                        {
                            string[] whichAndSpeed = value.Split(' ');
                            if (whichAndSpeed.Length >= 2 && int.TryParse(whichAndSpeed[0], out int which) && int.TryParse(whichAndSpeed[0], out int speed))
                                SystemMonitor.Default.CtrlFanSpeed(speed, which);
                        }
                        break;
                    case "ProbeInfo":
                        ProbeManager.Default.SetOneProbeFactoryInfo(value);
                        break;
#if FpgaFlashUpdater
                    case "ProductInfo":
                        return FpgaFlashUpdater_Helper.WriteProductInfo(value);
                    case "ReadbackAllFPGAVersionInfoAtFlash":
                        FpgaFlashUpdater_Helper.ReadbackAllFPGAVersionInfoAtFlash();
                        return HdCmd.None;
#endif
                    case "ReloadAndSetting_PhyAnalogChannelAmpTemperatueCoefficient":
                        Hd.CurrProduct?.Acquirer_AnalogChannel?.InitPhyAnalogChAmplitudeTemperaturesCompensationCoefficient();
                        return HdCmd.ChnlScaleIndex;
                    case "ReloadAndSetting_AcqProcBdLooptimeDelay":
                        SysAutoCalibration.Default.Trig_AcqProcBdLooptime_Cali();
                        return HdCmd.TrigPosition;
                    case "ReloadAndSetting_Procboard2AcqBoardTrigWindow":
                        //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.DoScanProcboard2AcqBoardTrigWindow();
                        return HdCmd.TrigPosition;
                    case "AutoCaliAtInit":
                        Hd.CurrProduct?.Acquirer_AnalogChannel?.AutoCaliAtInit(Hd.UIMessage);
                        return HdCmd.None;
                    case "Cali_AdcReceiveStabilityAtDebugStopMode":
                        Hd.Calibration.AdcReceiveStability();
                        Hd.CurrProduct!.AcqBd!.ConfigAdc();
                        return HdCmd.None;
                    default:
                        return FactoryCaliScpiProc_SpecialData_Set_OtherProcess(commasIndex, key, value);
                }
            }
            catch
            {
            }
            return HdCmd.None;
        }

        private static string FactoryCaliScpiProc_SpecialData_Get_OtherProcess(string param)
        {
            #region AWG校准
            Regex regex = new Regex(@"^(?:CalibAWG)([1-4])");
            if (regex.IsMatch(param))
            {
                return AWG.GetCaliInfos(param);
            }
            #endregion
            else
                return "";
        }
        public static string FactoryCaliScpiProc_SpecialData_Get(string param)
        {
            string result = param switch
            {
                "TrigState" => "trig'd",
                "ADC5200SyncWindowRegValue" => Hd.CurrProduct?.AcqBd?.ReadADC5200SyncWindowRegValue() ?? "",
                "GetComModelConstData" => GetComModelConstData(),
                "GetAcqStatisticsInfo" => Hd.GetAcqStatisticsInfo(),

                "GetSoftLA_FifoFull" => "true",
                "DebugVariant" => Hd.CurrDebugVarints.StringValue,
                //"Lo" => $"{Hd.Lo[0]},{Hd.Lo[1]},{Hd.Lo[2]},{Hd.Lo[3]}",
                "SystemTemperatures" => Hd.ReadSystemTemperatures(),
                "GetSysMonitorInfo" => SystemMonitor.Default.Read(),
                "GetAllPhyChannelTemperatures" => SystemMonitor.Default.GetChannelTemperaturesByCelsius(),
                "GetAllBoardTemperature" => SystemMonitor.Default.GetAllBoardTemperature(),
                "GetPhyChannelTemperatures1" => SystemMonitor.Default.ReadAndGetAnalogChannelTemperatures(0),
                "GetPhyChannelTemperatures2" => SystemMonitor.Default.ReadAndGetAnalogChannelTemperatures(1),
                "FlashCaliData" => LoadCaliDataFromFlash().ToString(),
                "DbiBandWidthMode" => (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.CurrChBWModeAndActiveState & 0x100) == 0 ? "Full" : "Other",
                "AcqModeInterleaveDefine" => Hd.CurrProduct.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetAcqModeInterleaveDefineString(),
                "GetExtTrigCompareResult" => Hd.GetExtTrigCompareResult(),
                "ProbeInfo" => ProbeManager.Default.GetAllProbeFactoryInfo(),
#if FpgaFlashUpdater
                "FpgaVersionInfoAtFlashIsReadback" => FpgaFlashUpdater_Helper.GetAllFPGAVersionInfoAtFlashIsReadback()?"1":"0",
                "FpgaVersionInfoAtFlash" => FpgaFlashUpdater_Helper.GetAllFPGAVersionInfoAtFlash(),
                "ProductInfo" => FpgaFlashUpdater_Helper.GetProductInfoAtFlash(),
#endif
#if RadioFrequency
                "GetCurrentRadioFrequencyWindowCoefficients" => AbstractAcquirer_RadioFrequency.GetWindowCoefficient(),
#endif
                //"AnalogChannelCaliMemo"=> AbstractController_AnalogChannel.GetCaliMemo(),
                _ => FactoryCaliScpiProc_SpecialData_Get_OtherProcess(param),
            };
            return result;
        }
        #endregion
        public static string FactoryCaliScpiProc_ReadbackAdcRegisterValue()
        {
            return "";
            //comment for JiHe_MSO7000X return Hd.CurrProduct?.AcqBd?.TiAdc_ReadbackAdcRegisterValue() ?? "";
        }
        public static string FactoryCaliScpiProc_ReadbackAllDspRegister()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(Hd.CurrProduct?.PcieBd?.GetRegMonitorResult());
            stringBuilder.AppendLine(Hd.CurrProduct?.S6Bd?.GetRegMonitorResult());
            stringBuilder.AppendLine(Hd.CurrProduct?.ProcBd?.GetRegMonitorResult());
            stringBuilder.AppendLine(Hd.CurrProduct?.AcqBd?.GetRegMonitorResult());
            return stringBuilder.ToString();
        }
        #region 获取指定的Bin数据
        public static bool TakeSpecialBinData(string param, out byte[] binData)
        {
            if (param == "TakeFPGASoftLACaptureBinData")
            {
                binData = new byte[10 * 1024];
                for (int i = 0; i < 10 * 1024; i++)
                    binData[i] = (byte)(i % 256);
                return true;
            }
            else if (param.IndexOf("DbiCoefficientsTables") >= 0)
            {
                int commaIndex = param.IndexOf(',');
                DbiCoefficientsTablesType dbiCoefficientsTablesType = (DbiCoefficientsTablesType)int.Parse(param.Substring(commaIndex + 1, 1));
                binData = DbiCoefficientsTables.Default.Serialize(dbiCoefficientsTablesType);
                return true;
            }
            //Other，还没有定义
            binData = new byte[16];
            return false;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bIsAcqBoard"></param>
        /// <param name="regAddr">除采集板外，其他板的地址，使用绝对地址</param>
        /// <param name="data"></param>
        public static void Test_WriteRegister(UInt32 regAddr, UInt32 data, bool bIsAcqBoard)
        {
#if !Product_B21_JinHui_PXI
            if (!bIsAcqBoard)
                HdIO.WriteReg(regAddr, data);
            else
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(regAddr, data);
#else
#endif
        }
        public static string GetAllFPGAVersionInfo()
        {
            return FpgaVersion.GetAllFPGAVersionInfo();
        }
        public static string GetAllFPGAWriteVersionInfo()
        {
            return FpgaVersion.GetAllFPGAVersionInfo();
        }

    }
}

using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Driver.Calibration;
using ScopeX.Hardware.Driver.PlatForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ScopeX.Hardware.Driver
{
    internal partial class ProductFactory
    {
        private static OscilloscopeProduct CreateProduct_B24_AI20G()
        {
            void ConfigDebugBooleanVariantState()
            {
                Hd.CurrDebugVarints.bEnable_AmplitudeTemperatureCompensate = false;
                //Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime = false;
                Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime = true;    //8000config
                Hd.CurrDebugVarints.bEnable_AtStartScanAdcRxWindows = false;
                Hd.CurrDebugVarints.bEnable_AtStartTrigScanWindow = false;

                Hd.CurrDebugVarints.bEnable_AdcDataDebugMode = false;
                Hd.CurrDebugVarints.bEnable_DigitTrigger = true;
                Hd.CurrDebugVarints.bEnable_AcqDigitTrigger = true;
                Hd.CurrDebugVarints.bEnable_AcqbdInterpolation = true;
                Hd.CurrDebugVarints.bEnable_AcqBd_Afc = false;
                Hd.CurrDebugVarints.bEnable_AcqBd_Pfc = false;

                Hd.CurrDebugVarints.bEnable_CorrectTiAdc = true;
                //Hd.CurrDebugVarints.bEnable_Proc_TiAdc = false;//????

                Hd.CurrDebugVarints.bEnable_Dbi_IntDelay = true;
                Hd.CurrDebugVarints.bEnable_Dbi_AmpFreqCoef = true;
                Hd.CurrDebugVarints.bEnable_Dbi_AntiImageCoef = true;
                Hd.CurrDebugVarints.bEnable_Dbi_FractionaryDelayCoef = false;
                Hd.CurrDebugVarints.bEnable_Dbi_LocalOscillatorCoef = true;
                Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation = true;
                Hd.CurrDebugVarints.bEnable_Dbi_OverlapPhaseFreqDelayCoef = false;
                Hd.CurrDebugVarints.bEnable_Dbi_PhaseFreqCoef = false;
                Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode = true;

                Hd.CurrDebugVarints.bEnable_ProcBd_Average = false;

                //8000config

                //Hd.CurrDebugVarints.bEnableAutoFanControl = false;
                //Hd.CurrDebugVarints.bEnableAnalogTemperatureCompensate = false;
                //Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime = true;
                //Hd.CurrDebugVarints.bEnable_AtStartScanAdcRxWindows = true;
                //Hd.CurrDebugVarints.bEnable_AtStartTrigScanWindow = false;

                //Hd.CurrDebugVarints.bEnable_AdcDataDebugMode = false;
                //Hd.CurrDebugVarints.bEnable_DigitTrigger = true;
                //Hd.CurrDebugVarints.bEnable_AcqDigitTrigger = true;
                //Hd.CurrDebugVarints.bEnable_AcqbdInterpolation = true;
                //Hd.CurrDebugVarints.bEnable_ProbdInterpolation = true;
                //Hd.CurrDebugVarints.bEnable_AcqBd_Afc = false;
                //Hd.CurrDebugVarints.bEnable_AcqBd_Pfc = false;

                //Hd.CurrDebugVarints.bEnable_CorrectTiAdc = true;
                //Hd.CurrDebugVarints.bEnable_Dsp = true;
                //Hd.CurrDebugVarints.bEnable_Dsp_Pro = true;
                //Hd.CurrDebugVarints.bEnable_ChannelSync = true;
                //Hd.CurrDebugVarints.bEnable_ChannelSync_Farrow = true;
                //Hd.CurrDebugVarints.bEnable_bandwidth = false;
                //Hd.CurrDebugVarints.bEnable_analog_signal = false;

                //Hd.CurrDebugVarints.bEnable_Dbi_IntDelay = false;
                //Hd.CurrDebugVarints.bEnable_Dbi_AmpFreqCoef = false;
                //Hd.CurrDebugVarints.bEnable_Dbi_AntiImageCoef = false;
                //Hd.CurrDebugVarints.bEnable_Dbi_FractionaryDelayCoef = false;
                //Hd.CurrDebugVarints.bEnable_Dbi_LocalOscillatorCoef = false;
                //Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation = false;
                //Hd.CurrDebugVarints.bEnable_Dbi_OverlapPhaseFreqDelayCoef = false;
                //Hd.CurrDebugVarints.bEnable_Dbi_PhaseFreqCoef = false;
                //Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode = false;


            }
            (String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC)[] AcqFpgaExistsConfig =
       {
                ("B0" ,true  ,0,5,6),//B0，采集板1
                ("B1" ,true  ,0,5,6),//B1，采集板3
                ("B2" ,true ,0,5,6),//B2
                ("B3" ,true ,0,5,6),//B3
                ("B4" ,true ,0,5,6),//B4
                ("B5" ,true ,0,5,6),//B5
                ("B6" ,true ,0,5,6),//B6
                ("B7" ,true ,0,5,6),//B7
                ("B8" ,false ,0,0,0),//B8
                ("B9" ,false ,0,0,0),//B9
                ("B10",false ,0,0,0),//B10
                ("B11",false ,0,0,0),//B11

            };
            //// 硬件板插入配置表，没插就注释掉
            //AcqBdNo[] AcqFpgaExistsConfig = 
            //{
            //    AcqBdNo.B7,
            //    //AcqBdNo.B8,
            //    //AcqBdNo.B9,
            //    AcqBdNo.B10,
            //    AcqBdNo.B11,
            //    AcqBdNo.B12,
            //};

            Dictionary<CoefficientsTableType, CoefficientsTableConfig> localCoefficientsTableMeanings = new()
            {
                //整数是该产品，该类型参数每通道的Int32数据的个数，用于发送时。最大目前定义为8192
                [CoefficientsTableType.Coefficients1] = new() { Name = "Interpolation_Acq", Length =700, LengthOfPartA = 500, LengthOfPartB = 200, Sender = CoefficientsTableSender_Standard.Send_InterpolationCoefficientsToAcqBoardByRegisterMode },
            };
            PlatFormManager.ProductType = ProductType.B23_DBI13G;

            OscilloscopeProduct product = new OscilloscopeProduct();
            product.HardwareConfig = new ProductHardwareConfig()
            {
                AnalogWaveDMAByteCount = 81920,
                Default_AcqBoardCH_MODE_SamplingMode = 0xc180,
                Default_PcieBoardFifoCtrl_ChannelMode = 2,

                bTiAdcOpened = false,
                bAFCOpened = true,
                bDBIOpened = false,
                bExistAWGModule = true,
                LocalCoefficientsTableMeanings = localCoefficientsTableMeanings,

                TrigLoopFixedDelay_20GNoInterpolation = 10,
                TrigLoopFixedDelay_10GNoInterpolation = 10,
                TrigLoopFixedDelay_20GInterpolation = 10,
                TrigLoopFixedDelay_10GInterpolation = 10,

                LA_MaxSamplingRateByPS = 200,

                DownloadBlockDataMode = DownloadBlockDataMode.Register,
                MaxLongStorageDeep = AnaChnlLengthOpt.Of250MDots,

                bExistsAnalogChannelGainFineAdjustMode = true,

                CaliSourceCtrlByChannel = 1,
            };
            ConfigDebugBooleanVariantState();

            product.LogicValue_Set = HdSpecial.FactoryCaliScpiProc_LogicValue_Set;
            product.LogicValue_Get = HdSpecial.FactoryCaliScpiProc_LogicValue_Get;

            product.SpecialData_Set = HdSpecial.FactoryCaliScpiProc_SpecialData_Set;
            product.SpecialData_Get = HdSpecial.FactoryCaliScpiProc_SpecialData_Get;

            product.Ctrl_AnalogChannel = new Controller_AnalogChannel_DBI20G();
            Dictionary<AnaChnlTimebaseIndex, Int32> InterpolationLevelDiscardNumTable = new Dictionary<AnaChnlTimebaseIndex, int>()
            {
                [AnaChnlTimebaseIndex.Lv5p] = 10,
                [AnaChnlTimebaseIndex.Lv10p] = 10,
                [AnaChnlTimebaseIndex.Lv20p] = 10,
                [AnaChnlTimebaseIndex.Lv50p] = 10,
                [AnaChnlTimebaseIndex.Lv100p] = 10,
                [AnaChnlTimebaseIndex.Lv200p] = 10,
                [AnaChnlTimebaseIndex.Lv500p] = 10,
                [AnaChnlTimebaseIndex.Lv1n] = 10,
                [AnaChnlTimebaseIndex.Lv2n] = 10,
                [AnaChnlTimebaseIndex.Lv5n] = 10,
                [AnaChnlTimebaseIndex.Lv10n] = 10,
                [AnaChnlTimebaseIndex.Lv20n] = 10,
                [AnaChnlTimebaseIndex.Lv50n] = 10,
                [AnaChnlTimebaseIndex.Lv100n] = 10,
                [AnaChnlTimebaseIndex.Lv200n] = 10,
                [AnaChnlTimebaseIndex.Lv500n] = 10,
            };
            product.Ctrl_Trigger = new Controller_Trigger_Standard(InterpolationLevelDiscardNumTable);
            product.Ctrl_Decoder = new Controller_Decoder_Standard();
            product.Ctrl_Misc = new Controller_Misc_DBI();

            product.PcieBd = new PcieBd_Standard();
            product.S6Bd = new S6Bd_DBI13G();
            product.ProcBd = new ProcBd_Standard();
            product.AcqBd = new AcqBd_DBI13G(AcqFpgaExistsConfig);

            product.ExtramModule = new Extram_Unidroit();

            product.AnalogAcquireModel = new AnalogAcquireModel_DBI13G_AcqBd6();
            //product.AnalogAcquireModel = new AnalogAcquireModel_DBI13G_AcqBd12();

            product.Acquirer_AnalogChannel = new Acquirer_AnalogChanel_DBI13G();
            product.Acquirer_AnalogChannel.MaxPerDataByps = 100;

            product.Acquirer_Cymometer = new Acquirer_Cymometer_Standard();
            product.Acquirer_Decoder = new Acquirer_Decoder_Standard();
            product.Acquirer_DPX = new Acquirer_DPX_Standard();

            Acquisition.CreateAcquirer(new AbstractAcquirer[]
            {
                    product.Acquirer_AnalogChannel,
                    //product.Acquirer_Cymometer,
                    //product.Acquirer_DPX,
                    //product.Acquirer_Decoder,
             });

            Dictionary<string/*dll name*/, int/*输入参数个数*/> matlabDllsDefine = new Dictionary<string, int>()
            {
                ["smoothfastedge.dll"] = 1,
                ["Dbi_AutoCali_Calc_phase_error.dll"] = 2,
                ["DBI_Overlap_Phase_Diff_3Point.dll"] = 3,
                ["Dbi_AutoCali_SaveLoc.dll"] = 4,
                ["Dbi_AutoCali_CalcIntDelayCount.dll"] = 2,
                ["SoftwareBandwidthProcess_GenCoeff.dll"] = 2,
                ["SoftwareBandwidthProcess_Calc.dll"] = 2,
                ["freqFilter.dll"] = 4,
                ["cal_discard_num_by_multtone.dll"] = 5,
                ["MatlabGenerateOverlapBandSync_LoCoe.dll"] = 5,
            };
            product.MatlabDllsDefine = matlabDllsDefine;

            CoefficientsTableSender_DBI.CheckCrcEnable = false;

            product.EnableAutoCaliAtStart = false;

            product.AutoCaliAtInit = ourAutoCaliAtInit;
            product.AnaChnlBitWidthDefine = new Int32[] { 12, 13, 14, 15, 16 };

            return product;

        }

        private static void ourAutoCaliAtInit()
        {
            Boolean needDiscardAgain = false;
            if (!Hd.bAttachHardware)
                return;
            HdMessage backHdMessage = Hd.UIMessage! with { };//原始参数备份
            Hd.CurrDebugVarints.DoBackup();

            //Cali_XunXin40GAdcDbi.XunXin40GAdcTiAdcByInternalSignalSource();


            //try
            //{
            //    DBIAutoCali.CaliTiadc(ChannelId.C1);

            //}
            //catch (Exception)
            //{

            //}

            //needDiscardAgain = DBIAutoCali.XunXin40GAdc_SubbandDiscardDots(0);
            //if (needDiscardAgain)
            //{
            //    DBIAutoCali.XunXin40GAdc_SubbandDiscardDots(1);
            //}

            //DBIAutoCali.LocalOscillatorCoefficients();

            //Hd.CurrDebugVarints.DoRestore();

            Hd.LocalCommands = (Int64)HdCmd.CaliDataChanged;
            Hd.LocalCommands |= (Int64)HdCmd.ChnlActive;

            Hd.UIMessage = backHdMessage with { };
            Hd.Execute(Hd.UIMessage);
        }
       

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;

namespace ScopeX.Hardware.Driver
{
    internal partial class ProductFactory
    {
        private static OscilloscopeProduct CreateProduct_JiHe_MSO8000X()
        {
            void ConfigDebugBooleanVariantState()
            {
                Hd.CurrDebugVarints.bEnableAutoFanControl = false;
                Hd.CurrDebugVarints.bEnableAnalogTemperatureCompensate = false;
                Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime = true;
                Hd.CurrDebugVarints.bEnable_AtStartScanAdcRxWindows = true;
                Hd.CurrDebugVarints.bEnable_AtStartTrigScanWindow = false;

                Hd.CurrDebugVarints.bEnable_AdcDataDebugMode = false;
                Hd.CurrDebugVarints.bEnable_DigitTrigger = true;
                Hd.CurrDebugVarints.bEnable_AcqDigitTrigger = true;
                Hd.CurrDebugVarints.bEnable_AcqbdInterpolation = true;
                Hd.CurrDebugVarints.bEnable_ProbdInterpolation = true;
                Hd.CurrDebugVarints.bEnable_AcqBd_Afc = false;
                Hd.CurrDebugVarints.bEnable_AcqBd_Pfc = false;

                Hd.CurrDebugVarints.bEnable_CorrectTiAdc = true;
                Hd.CurrDebugVarints.bEnable_Dsp = true;
                Hd.CurrDebugVarints.bEnable_Dsp_Pro = true;
                Hd.CurrDebugVarints.bEnable_ChannelSync = true;
                Hd.CurrDebugVarints.bEnable_ChannelSync_Farrow = true;
                Hd.CurrDebugVarints.bEnable_bandwidth = false;
                Hd.CurrDebugVarints.bEnable_analog_signal = false;

                Hd.CurrDebugVarints.bEnable_Dbi_IntDelay = false;
                Hd.CurrDebugVarints.bEnable_Dbi_AmpFreqCoef = false;
                Hd.CurrDebugVarints.bEnable_Dbi_AntiImageCoef = false;
                Hd.CurrDebugVarints.bEnable_Dbi_FractionaryDelayCoef = false;
                Hd.CurrDebugVarints.bEnable_Dbi_LocalOscillatorCoef = false;
                Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation = false;
                Hd.CurrDebugVarints.bEnable_Dbi_OverlapPhaseFreqDelayCoef = false;
                Hd.CurrDebugVarints.bEnable_Dbi_PhaseFreqCoef = false;
                Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode = false;
            }
            (String Name, Boolean ISENABLE, UInt32 NUM_ACQ_DATA, UInt32 NUM_ACQ_CTRL, UInt32 NUM_PROC)[] FpgaExistsConfig =
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

            #region 有关S6中7044、7043 的配置
            Dictionary<UInt32, UInt32> s6Board7044SpecialConfig = new Dictionary<uint, uint>()
            {
                //寄存器，值
                {0x00DC,0xb3 },
                {0x0122,0xb3 },
                {0x012C,0xb3 },
                {0x0136,0xb3 },
                {0x0104,0xb3 },
                {0x0118,0xb3 },
                {0x014A,0xb3 },
                {0x00E6,0xb3 },
                {0x0140,0xb3 },

                {0x00c8,0xb3 },// DCLKOUT0 to acq1_10M
                {0x00f0,0xb3 } // DCLKOUT4 to acq3_10M
            };
            Dictionary<UInt32, UInt32> s6Board7043SpecialConfig = new Dictionary<uint, uint>()
            {
                //寄存器，值
                {0x118,0xb3 },
                {0x140,0xb3 }
            };
            #endregion

            Dictionary<CoefficientsTableType, CoefficientsTableConfig> localCoefficientsTableMeanings = new Dictionary<CoefficientsTableType, CoefficientsTableConfig>()
            {
                //整数是该产品，该类型参数每通道的Int32数据的个数，用于发送时。最大目前定义为8192
                [CoefficientsTableType.Coefficients1] = new() { Name = "Interpolation_Acq", Length = 500, LengthOfPartA = 400, LengthOfPartB = 100, Sender = CoefficientsTableSender_8000X.Send_InterpolationCoefficientsToAcqBoardByRegisterMode },
            };

            OscilloscopeProduct product = new OscilloscopeProduct();
            //相关硬件配置定义
            product.HardwareConfig = new ProductHardwareConfig()
            {
                PLL7044Mode = PLL7044Mode.Schame_JiHe_MSO8000X,
                PLL7043Mode = PLL7043Mode.Schame_JiHe_MSO8000X,
                AnalogWaveDMAByteCount = Constants.CHNL_DATA_NUM / 1000 * ChannelIdExt.AnaChnlNum * 1024,// Constants.CHNL_DATA_NUM / 1000 * ChannelIdExt.AnaChnlNum/*4个子带*/* 3 / 2 /*相当于1.5*/ * 1024,
                //Default_AcqBoardCH_MODE_SamplingMode = 0x340,
                Default_AcqBoardCH_MODE_SamplingMode = 0x180,
                Default_PcieBoardFifoCtrl_ChannelMode = 2,

                bTiAdcOpened = false,
                bAFCOpened = true,
                bDBIOpened = false,
                bExistAWGModule = true,
                LocalCoefficientsTableMeanings = localCoefficientsTableMeanings,
                S6Board7044SpecialConfig = s6Board7044SpecialConfig,
                S6Board7043SpecialConfig = s6Board7043SpecialConfig,

                TrigLoopFixedDelay_20GNoInterpolation = 10,
                TrigLoopFixedDelay_10GNoInterpolation = 10,
                TrigLoopFixedDelay_20GInterpolation = 10,
                TrigLoopFixedDelay_10GInterpolation = 10,

                LA_MaxSamplingRateByPS = 200,

                DownloadBlockDataMode = DownloadBlockDataMode.Register,
                MaxLongStorageDeep = AnaChnlLengthOpt.Of250MDots,

                bExistsAnalogChannelGainFineAdjustMode = true,
            };

            #region ConfigDebugBooleanVariantState

            ConfigDebugBooleanVariantState();

            #endregion

            product.LogicValue_Set = HdSpecial.FactoryCaliScpiProc_LogicValue_Set;
            product.LogicValue_Get = HdSpecial.FactoryCaliScpiProc_LogicValue_Get;

            product.SpecialData_Set = HdSpecial.FactoryCaliScpiProc_SpecialData_Set;
            product.SpecialData_Get = HdSpecial.FactoryCaliScpiProc_SpecialData_Get;

            //相关控制器配置
            product.Ctrl_AnalogChannel = new Controller_AnalogChannel_3U8G();
            ///<summary>
            ///将8G项目的通道控制部分移植过来，所以注释了原来用的Controller_AnalogChannel_JiHe2d5G()控制器，
            ///改用了Controller_AnalogChannel_BW8G()控制器
            ///</summary>
            //product.Ctrl_AnalogChannel = new Controller_AnalogChannel_BW8G();
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
            product.Ctrl_Trigger = new Controller_Trigger_MSO8000(InterpolationLevelDiscardNumTable);
            product.Ctrl_Decoder = new Controller_Decoder_Standard();
            product.Ctrl_Misc = new Controller_Misc_Standard();

            //相关板卡类配置
            product.PcieBd = new PcieBd_Standard();
            product.S6Bd = new S6Bd_Standard();
            product.ProcBd = new ProcBd_Standard();
            product.AcqBd = new Boadr_Acq_JiHe_MSO8000X(FpgaExistsConfig);

            //相关采集类配置
            product.Acquirer_AnalogChannel = new Acquirer_AnalogChannel_JiHe_MSO8000X();
            product.Acquirer_AnalogChannel.MaxPerDataByps = 50;

            product.Acquirer_Cymometer = new Acquirer_Cymometer_Standard();
            //product.Acquirer_Decoder = new Acquirer_Decoder_Standard();
            product.Acquirer_DPX = new Acquirer_DPX_Standard();
            //product.Acquirer_LA = new Acquirer_LA_Standard(null);

            Acquisition.CreateAcquirer(new AbstractAcquirer[]
            {
                    product.Acquirer_AnalogChannel,
                    product.Acquirer_Cymometer,
                    product.Acquirer_DPX,
                    //product.Acquirer_Decoder,
                    //product.Acquirer_LA
             });
            return product;

        }
    }
}
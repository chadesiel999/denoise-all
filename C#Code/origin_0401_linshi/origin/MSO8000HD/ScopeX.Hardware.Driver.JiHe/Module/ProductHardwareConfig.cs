using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
namespace ScopeX.Hardware.Driver
{
    internal class ProductHardwareConfig
    {
        public PLL7044Mode PLL7044Mode
        {
            get;
            init;
        } = PLL7044Mode.SchemeA;
        public PLL7043Mode PLL7043Mode
        {
            get;
            init;
        } = PLL7043Mode.SchemeA;
        public Int32 AnalogWaveDMAByteCount
        {
            get;
            init;
        } = Constants.CHNL_DATA_NUM / 1000 * (4 * Constants.ADC_BITS + 16) / 8 * 1024;
        public UInt32 Default_PcieBoardFifoCtrl_ChannelMode
        {
            get;
            init;
        } = 2;
        public UInt32 Default_AcqBoardCH_MODE_SamplingMode
        {
            get;
            init;
        } = 2;
        public bool bTiAdcOpened
        {
            get;
            init;
        } = true;
        public bool bAFCOpened
        {
            get;
            init;
        } = true;
        public bool bDBIOpened
        {
            get;
            init;
        } = false;
        public AnaChnlLengthOpt MaxLongStorageDeep
        {
            get;
            init;
        } = AnaChnlLengthOpt.Of25KDots;

        public Dictionary<CoefficientsTableType, CoefficientsTableConfig> LocalCoefficientsTableMeanings
        {
            get;
            init;
        } = new Dictionary<CoefficientsTableType, CoefficientsTableConfig>();
        public Dictionary<UInt32, uint> S6Board7044SpecialConfig
        {
            get;
            init;
        } = new Dictionary<uint, uint>();
        public Dictionary<UInt32, UInt32> S6Board7043SpecialConfig
        {
            get;
            init;
        } = new Dictionary<uint, uint>();
        public UInt32 TrigLoopFixedDelay_20GNoInterpolation
        {
            get;
            init;
        } = 10;
        public UInt32 TrigLoopFixedDelay_10GNoInterpolation
        {
            get;
            init;
        } = 10;
        public UInt32 TrigLoopFixedDelay_20GInterpolation
        {
            get;
            init;
        } = 10;
        public UInt32 TrigLoopFixedDelay_10GInterpolation
        {
            get;
            init;
        } = 10;
        public bool bS6BoardIsAlone
        {
            get;
            init;
        } = false;
        public bool bExistAWGModule
        {
            get;
            init;
        } = false;
        public UInt64 LA_MaxSamplingRateByPS
        {
            get;
            init;
        } = 200;//5G
        public DownloadBlockDataMode DownloadBlockDataMode
        {
            get; set;
        } = DownloadBlockDataMode.Register;

        public Boolean bExistsAnalogChannelGainFineAdjustMode
        {
            get; set;
        }=false;

        /// <summary>
        /// 校准源的控制字是通过哪块通道板转发到校准板
        /// </summary>
        public UInt32 CaliSourceCtrlByChannel
        {
            get;
            init;
        } = 0;

        #region 各种系数文件
        //public Dictionary<string, string> _ChannelPerScaleAmpFreqCoefficientsDefine = new Dictionary<string, string>()
        //{
        //    ["C1_Lv500u"] = $@".\CaliData\DataFiles\Afc_C1_500uV.txt",
        //    ["C1_Lv1m"] = $@".\CaliData\DataFiles\Afc_C1_1mV.txt",
        //    ["C1_Lv2m"] = $@".\CaliData\DataFiles\Afc_C1_2mV.txt",
        //    ["C1_Lv5m"] = $@".\CaliData\DataFiles\Afc_C1_5mV.txt",
        //    ["C1_Lv10m"] = $@".\CaliData\DataFiles\Afc_C1_10mV.txt",
        //    ["C1_Lv20m"] = $@".\CaliData\DataFiles\Afc_C1_20mV.txt",
        //    ["C1_Lv50m"] = $@".\CaliData\DataFiles\Afc_C1_50mV.txt",
        //    ["C1_Lv00m"] = $@".\CaliData\DataFiles\Afc_C1_100mV.txt",
        //    ["C1_Lv200m"] = $@".\CaliData\DataFiles\Afc_C1_200mV.txt",
        //    ["C1_Lv500m"] = $@".\CaliData\DataFiles\Afc_C1_500mV.txt",
        //    ["C1_Lv1"] = $@".\CaliData\DataFiles\Afc_C1_1V.txt",
        //    ["C1_Lv2"] = $@".\CaliData\DataFiles\Afc_C1_2V.txt",
        //    ["C1_Lv5"] = $@".\CaliData\DataFiles\Afc_C1_5V.txt",
        //    ["C1_Lv10"] = $@".\CaliData\DataFiles\Afc_C1_10V.txt",
        //    ["C1_Lv20"] = $@".\CaliData\DataFiles\Afc_C1_20V.txt",
        //    ["C1_Lv50"] = $@".\CaliData\DataFiles\Afc_C1_5V.txt",
        //    ["C1_Lv100"] = $@".\CaliData\DataFiles\Afc_C1_100V.txt",

        //    ["C2_Lv500u"] = $@".\CaliData\DataFiles\Afc_C2_500uV.txt",
        //    ["C2_Lv1m"] = $@".\CaliData\DataFiles\Afc_C2_1mV.txt",
        //    ["C2_Lv2m"] = $@".\CaliData\DataFiles\Afc_C2_2mV.txt",
        //    ["C2_Lv5m"] = $@".\CaliData\DataFiles\Afc_C2_5mV.txt",
        //    ["C2_Lv10m"] = $@".\CaliData\DataFiles\Afc_C2_10mV.txt",
        //    ["C2_Lv20m"] = $@".\CaliData\DataFiles\Afc_C2_20mV.txt",
        //    ["C2_Lv50m"] = $@".\CaliData\DataFiles\Afc_C2_50mV.txt",
        //    ["C2_Lv00m"] = $@".\CaliData\DataFiles\Afc_C2_100mV.txt",
        //    ["C2_Lv200m"] = $@".\CaliData\DataFiles\Afc_C2_200mV.txt",
        //    ["C2_Lv500m"] = $@".\CaliData\DataFiles\Afc_C2_500mV.txt",
        //    ["C2_Lv1"] = $@".\CaliData\DataFiles\Afc_C2_1V.txt",
        //    ["C2_Lv2"] = $@".\CaliData\DataFiles\Afc_C2_2V.txt",
        //    ["C2_Lv5"] = $@".\CaliData\DataFiles\Afc_C2_5V.txt",
        //    ["C2_Lv10"] = $@".\CaliData\DataFiles\Afc_C2_10V.txt",
        //    ["C2_Lv20"] = $@".\CaliData\DataFiles\Afc_C2_20V.txt",
        //    ["C2_Lv50"] = $@".\CaliData\DataFiles\Afc_C2_5V.txt",
        //    ["C2_Lv100"] = $@".\CaliData\DataFiles\Afc_C2_100V.txt",

        //    ["C3_Lv500u"] = $@".\CaliData\DataFiles\Afc_C3_500uV.txt",
        //    ["C3_Lv1m"] = $@".\CaliData\DataFiles\Afc_C3_1mV.txt",
        //    ["C3_Lv2m"] = $@".\CaliData\DataFiles\Afc_C3_2mV.txt",
        //    ["C3_Lv5m"] = $@".\CaliData\DataFiles\Afc_C3_5mV.txt",
        //    ["C3_Lv10m"] = $@".\CaliData\DataFiles\Afc_C3_10mV.txt",
        //    ["C3_Lv20m"] = $@".\CaliData\DataFiles\Afc_C3_20mV.txt",
        //    ["C3_Lv50m"] = $@".\CaliData\DataFiles\Afc_C3_50mV.txt",
        //    ["C3_Lv00m"] = $@".\CaliData\DataFiles\Afc_C3_100mV.txt",
        //    ["C3_Lv200m"] = $@".\CaliData\DataFiles\Afc_C3_200mV.txt",
        //    ["C3_Lv500m"] = $@".\CaliData\DataFiles\Afc_C3_500mV.txt",
        //    ["C3_Lv1"] = $@".\CaliData\DataFiles\Afc_C3_1V.txt",
        //    ["C3_Lv2"] = $@".\CaliData\DataFiles\Afc_C3_2V.txt",
        //    ["C3_Lv5"] = $@".\CaliData\DataFiles\Afc_C3_5V.txt",
        //    ["C3_Lv10"] = $@".\CaliData\DataFiles\Afc_C3_10V.txt",
        //    ["C3_Lv20"] = $@".\CaliData\DataFiles\Afc_C3_20V.txt",
        //    ["C3_Lv50"] = $@".\CaliData\DataFiles\Afc_C3_5V.txt",
        //    ["C3_Lv100"] = $@".\CaliData\DataFiles\Afc_C3_100V.txt",

        //    ["C4_Lv500u"] = $@".\CaliData\DataFiles\Afc_C4_500uV.txt",
        //    ["C4_Lv1m"] = $@".\CaliData\DataFiles\Afc_C4_1mV.txt",
        //    ["C4_Lv2m"] = $@".\CaliData\DataFiles\Afc_C4_2mV.txt",
        //    ["C4_Lv5m"] = $@".\CaliData\DataFiles\Afc_C4_5mV.txt",
        //    ["C4_Lv10m"] = $@".\CaliData\DataFiles\Afc_C4_10mV.txt",
        //    ["C4_Lv20m"] = $@".\CaliData\DataFiles\Afc_C4_20mV.txt",
        //    ["C4_Lv50m"] = $@".\CaliData\DataFiles\Afc_C4_50mV.txt",
        //    ["C4_Lv00m"] = $@".\CaliData\DataFiles\Afc_C4_100mV.txt",
        //    ["C4_Lv200m"] = $@".\CaliData\DataFiles\Afc_C4_200mV.txt",
        //    ["C4_Lv500m"] = $@".\CaliData\DataFiles\Afc_C4_500mV.txt",
        //    ["C4_Lv1"] = $@".\CaliData\DataFiles\Afc_C4_1V.txt",
        //    ["C4_Lv2"] = $@".\CaliData\DataFiles\Afc_C4_2V.txt",
        //    ["C4_Lv5"] = $@".\CaliData\DataFiles\Afc_C4_5V.txt",
        //    ["C4_Lv10"] = $@".\CaliData\DataFiles\Afc_C4_10V.txt",
        //    ["C4_Lv20"] = $@".\CaliData\DataFiles\Afc_C4_20V.txt",
        //    ["C4_Lv50"] = $@".\CaliData\DataFiles\Afc_C4_5V.txt",
        //    ["C4_Lv100"] = $@".\CaliData\DataFiles\Afc_C4_100V.txt",

        //    ["C5_Lv500u"] = $@".\CaliData\DataFiles\Afc_C5_500uV.txt",
        //    ["C5_Lv1m"] = $@".\CaliData\DataFiles\Afc_C5_1mV.txt",
        //    ["C5_Lv2m"] = $@".\CaliData\DataFiles\Afc_C5_2mV.txt",
        //    ["C5_Lv5m"] = $@".\CaliData\DataFiles\Afc_C5_5mV.txt",
        //    ["C5_Lv10m"] = $@".\CaliData\DataFiles\Afc_C5_10mV.txt",
        //    ["C5_Lv20m"] = $@".\CaliData\DataFiles\Afc_C5_20mV.txt",
        //    ["C5_Lv50m"] = $@".\CaliData\DataFiles\Afc_C5_50mV.txt",
        //    ["C5_Lv00m"] = $@".\CaliData\DataFiles\Afc_C5_100mV.txt",
        //    ["C5_Lv200m"] = $@".\CaliData\DataFiles\Afc_C5_200mV.txt",
        //    ["C5_Lv500m"] = $@".\CaliData\DataFiles\Afc_C5_500mV.txt",
        //    ["C5_Lv1"] = $@".\CaliData\DataFiles\Afc_C5_1V.txt",
        //    ["C5_Lv2"] = $@".\CaliData\DataFiles\Afc_C5_2V.txt",
        //    ["C5_Lv5"] = $@".\CaliData\DataFiles\Afc_C5_5V.txt",
        //    ["C5_Lv10"] = $@".\CaliData\DataFiles\Afc_C5_10V.txt",
        //    ["C5_Lv20"] = $@".\CaliData\DataFiles\Afc_C5_20V.txt",
        //    ["C5_Lv50"] = $@".\CaliData\DataFiles\Afc_C5_5V.txt",
        //    ["C5_Lv100"] = $@".\CaliData\DataFiles\Afc_C5_100V.txt",

        //    ["C6_Lv500u"] = $@".\CaliData\DataFiles\Afc_C6_500uV.txt",
        //    ["C6_Lv1m"] = $@".\CaliData\DataFiles\Afc_C6_1mV.txt",
        //    ["C6_Lv2m"] = $@".\CaliData\DataFiles\Afc_C6_2mV.txt",
        //    ["C6_Lv5m"] = $@".\CaliData\DataFiles\Afc_C6_5mV.txt",
        //    ["C6_Lv10m"] = $@".\CaliData\DataFiles\Afc_C6_10mV.txt",
        //    ["C6_Lv20m"] = $@".\CaliData\DataFiles\Afc_C6_20mV.txt",
        //    ["C6_Lv50m"] = $@".\CaliData\DataFiles\Afc_C6_50mV.txt",
        //    ["C6_Lv00m"] = $@".\CaliData\DataFiles\Afc_C6_100mV.txt",
        //    ["C6_Lv200m"] = $@".\CaliData\DataFiles\Afc_C6_200mV.txt",
        //    ["C6_Lv500m"] = $@".\CaliData\DataFiles\Afc_C6_500mV.txt",
        //    ["C6_Lv1"] = $@".\CaliData\DataFiles\Afc_C6_1V.txt",
        //    ["C6_Lv2"] = $@".\CaliData\DataFiles\Afc_C6_2V.txt",
        //    ["C6_Lv5"] = $@".\CaliData\DataFiles\Afc_C6_5V.txt",
        //    ["C6_Lv10"] = $@".\CaliData\DataFiles\Afc_C6_10V.txt",
        //    ["C6_Lv20"] = $@".\CaliData\DataFiles\Afc_C6_20V.txt",
        //    ["C6_Lv50"] = $@".\CaliData\DataFiles\Afc_C6_5V.txt",
        //    ["C6_Lv100"] = $@".\CaliData\DataFiles\Afc_C6_100V.txt",
        //};
        //public Dictionary<string, string> ChannelPerScaleAmpFreqCoefficientsDefine
        //{
        //    get => _ChannelPerScaleAmpFreqCoefficientsDefine;
        //    set => _ChannelPerScaleAmpFreqCoefficientsDefine = value;
        //}

        #endregion
    }

    internal enum PLL7044Mode
    {
        SchemeA = 1,//目前定位为插在Slot3的方案
        SchemeB = 2,//目前定位为插在Slot1的方案
        /// <summary>
        /// 目前定位为DBI20G方案
        /// </summary>
        SchemeC = 3,//目前定位为DBI20G方案

        Schame_JiHe_MSO7000X,
        Schame_JiHe_MSO8000X,
    }
    internal enum PLL7043Mode
    {
        SchemeA = 1,//目前定位为插在Slot3的方案
        SchemeB = 2,//目前定位为插在Slot1的方案
        SchemeC = 3,//目前定位为插在Slot1、Slot2、Slot3、Slot4等连续插入的方案，用在DBI20G上
        Schame_JiHe_MSO7000X,
        Schame_JiHe_MSO8000X,
    }
    internal enum CommCoefficientsTableType
    {
        AcqBd_Interpolation,
        AcqBd_TiAdc,
        AcqBd_AFC,
        AcqBd_PFC,
        None,
    }
    internal record CoefficientsTableConfig
    {
        public string Name { get; init; } = "";
        public int Length { get; init; }
        public int LengthOfPartA { get; init; }
        public int LengthOfPartB { get; init; }
        public CommCoefficientsTableType CoeffType { get; init; }
        public Action<CoefficientsTableType, bool>? Sender { get; init; }
    }
}

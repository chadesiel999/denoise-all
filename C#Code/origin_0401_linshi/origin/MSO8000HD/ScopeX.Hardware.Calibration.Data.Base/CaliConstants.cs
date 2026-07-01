using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class CaliConstants
    {
        public const Int32 KeyStrLen = 32;

        public const Char NameSpiltChar = '_';

        public static Int32 Fixed_MaxPhyCoarseScaleCount = AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV.Count;
        public const Int32 Fixed_MaxPhysicsChannelCount = 8;
        public const Int32 Fixed_PerChannelMergeAdcMaxCount = 4;
        public const Int32 Fixed_PerAdcCoreMaxCount = 4;
        public const Int32 Fixed_PerAdcCoreCount = 4;
        /// <summary>
        /// 目前，最多10张采集卡。该参数，只与ADC有关。
        /// 包括一片之内的压稳态问题，和多片的TiAdc问题。严格意义上与通道没有关系。
        /// 使用该参数的 数据包括TiAdc_SyncSampleClock 和 TiAdc_PhaseOffsetGain
        /// </summary>
        public const Int32 Fixed_AcqBoardMaxCount = 10;
    }
}

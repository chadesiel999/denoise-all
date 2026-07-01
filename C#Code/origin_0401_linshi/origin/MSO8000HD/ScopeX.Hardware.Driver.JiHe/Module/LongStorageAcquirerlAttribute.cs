using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.MathExt;
using System.Diagnostics.CodeAnalysis;
using static ScopeX.Hardware.Driver.AbstractAcquirer_AnalogChannel;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 模拟通道波形采集器的参数
    /// </summary>
    public class LongStorageAcquirerAttribute
    {
        internal UInt64 MaxPerDataByps
        {
            get;
            set;
        } = 50;//20G 采样

        /// <summary>
        /// 一个地址存放16个采样数据；但如果一个采集板对应多个通道（例如浅机箱），则需要除以通道数
        /// </summary>
        internal UInt32 DDR_PerAddrStoreDots = 16;

        /// <summary>
        /// 每8个DMA读回的数据，可以解析出每个通道的一个采样点；与硬件的组装方式有关，每个项目不同
        /// </summary>
        internal Int32 PerChannelDotsBytes = 8;

        internal Int32 ExtractNumMin = 80;

        internal UInt32 LAMaxAddrCnt = 128 * 1024 * 1024;

        internal UInt32 LABitsCntPerAddr = 32;


        internal Action<byte[], Int32, List<List<UInt16>>, Int32>? _ParseChannelDataFromDMA;

        internal void HookParseChannelDataFromDMA(Action<byte[], Int32, List<List<UInt16>>, Int32> action)
        {
            _ParseChannelDataFromDMA = action;
        }

        internal void ParseChannelDataFromDMA(byte[] dmaBuff, List<List<UInt16>> ChannelData, Int32 chnlNum)
        {
            if (_ParseChannelDataFromDMA != null)
            {
                _ParseChannelDataFromDMA(dmaBuff, dmaBuff.Length / PerChannelDotsBytes, ChannelData, chnlNum);
            }
        }

        private Func<AnaChnlTimebaseIndex, ExtractNumInterpolationNumPair>? _GetTimeBaseRatioPara;

        internal void HookGetTimeBaseRatioPara(Func<AnaChnlTimebaseIndex, ExtractNumInterpolationNumPair> func)
        {
            _GetTimeBaseRatioPara = func;
        }

        internal ExtractNumInterpolationNumPair GetTimeBaseRatioPara(AnaChnlTimebaseIndex timebaseIndex)
        {
            if (_GetTimeBaseRatioPara == null)
            {
                return new ExtractNumInterpolationNumPair(1, 1, 1000, 1000);
            }
            return _GetTimeBaseRatioPara(timebaseIndex);
        }

        private Func<AnaChnlTimebaseIndex, UInt32>? _GetLongStorageHardwareExtractNum;
        internal void HookGetLongStorageHardwareExtractNum(Func<AnaChnlTimebaseIndex, UInt32> func)
        {
            _GetLongStorageHardwareExtractNum = func;
        }

        internal UInt32 GetLongStorageHardwareExtractNum(AnaChnlTimebaseIndex timebaseIndex)
        {
            if (_GetLongStorageHardwareExtractNum == null)
            {
                UInt32 extractNum = 1;
                UInt64 perXDivByfs = (UInt64)((Hd.UIMessage?.Timebase?.TmbScale ?? 1) * 1_000_000_000);//TmbScale 以us为单位,*1_000_000_000,us==>fs
                UInt64 expectedPerDataByfs = perXDivByfs / 100;
                UInt64 ddrSampledPeriodByFs = MaxPerDataByps * 1000;
                if (expectedPerDataByfs > ddrSampledPeriodByFs)
                    extractNum = (UInt32)(expectedPerDataByfs / ddrSampledPeriodByFs);
                return extractNum < 10 ? 1 : extractNum / 10;
            }
            return _GetLongStorageHardwareExtractNum(timebaseIndex);
        }
    }
}

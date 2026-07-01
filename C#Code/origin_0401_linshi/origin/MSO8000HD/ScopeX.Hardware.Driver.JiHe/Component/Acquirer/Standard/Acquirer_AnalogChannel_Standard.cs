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
namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 模拟通道波形采集
    /// </summary>
    public class Acquirer_AnalogChannel_Standard : AbstractAcquirer_AnalogChannel
    {
        internal Acquirer_AnalogChannel_Standard() : base()
        {

        }

        #region 普通存储参数
        private static Dictionary<AnaChnlTimebaseIndex, ExtractNumInterpolationNumPair> NormalStorageExtractNumInterpolationNumTable = new()
        {
            //小于80，必须能够整除，大于80，10的倍数就可以
            //                                                             ExtractNum,InterpolationNum,HardwareFifoLen,SoftwareFifoLen
            [AnaChnlTimebaseIndex.Lv50p] = new ExtractNumInterpolationNumPair(1, 100, 1000, 1000),
            [AnaChnlTimebaseIndex.Lv100p] = new ExtractNumInterpolationNumPair(1, 50, 1000, 1000),
            [AnaChnlTimebaseIndex.Lv200p] = new ExtractNumInterpolationNumPair(1, 25, 1000, 1000),
            [AnaChnlTimebaseIndex.Lv500p] = new ExtractNumInterpolationNumPair(1, 10, 1000, 1000),
            [AnaChnlTimebaseIndex.Lv1n] = new ExtractNumInterpolationNumPair(1, 5, 1000, 1000),
            [AnaChnlTimebaseIndex.Lv2n] = new ExtractNumInterpolationNumPair(1, 2, 800, 800), //50/I * L=scale *10
            [AnaChnlTimebaseIndex.Lv5n] = new ExtractNumInterpolationNumPair(1, 1, 1000, 1000),
            [AnaChnlTimebaseIndex.Lv10n] = new ExtractNumInterpolationNumPair(1, 1, 2000, 2000),
            [AnaChnlTimebaseIndex.Lv20n] = new ExtractNumInterpolationNumPair(1, 1, 5000, 5000),
            [AnaChnlTimebaseIndex.Lv50n] = new ExtractNumInterpolationNumPair(1, 1, 10000, 10000),
            [AnaChnlTimebaseIndex.Lv100n] = new ExtractNumInterpolationNumPair(2, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv200n] = new ExtractNumInterpolationNumPair(4, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv500n] = new ExtractNumInterpolationNumPair(10, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv1u] = new ExtractNumInterpolationNumPair(20, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv2u] = new ExtractNumInterpolationNumPair(40, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv5u] = new ExtractNumInterpolationNumPair(100, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv10u] = new ExtractNumInterpolationNumPair(200, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv20u] = new ExtractNumInterpolationNumPair(400, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv50u] = new ExtractNumInterpolationNumPair(1_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv100u] = new ExtractNumInterpolationNumPair(2_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv200u] = new ExtractNumInterpolationNumPair(4_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv500u] = new ExtractNumInterpolationNumPair(10_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv1m] = new ExtractNumInterpolationNumPair(20_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv2m] = new ExtractNumInterpolationNumPair(40_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv5m] = new ExtractNumInterpolationNumPair(100_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv10m] = new ExtractNumInterpolationNumPair(200_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv20m] = new ExtractNumInterpolationNumPair(400_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv50m] = new ExtractNumInterpolationNumPair(1000_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv100m] = new ExtractNumInterpolationNumPair(2_000_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv200m] = new ExtractNumInterpolationNumPair(4_000_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv500m] = new ExtractNumInterpolationNumPair(10_000_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv1] = new ExtractNumInterpolationNumPair(20_000_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv2] = new ExtractNumInterpolationNumPair(40_000_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv5] = new ExtractNumInterpolationNumPair(100_000_000, 1, 10_000, 10_000),
            [AnaChnlTimebaseIndex.Lv10] = new ExtractNumInterpolationNumPair(200_000_000, 1, 10_000, 10_000),
        };
        #endregion
        
        internal override void CreateAcquireAttribute()
        {
            AcquingParameters.AcqStorageMode = Hd.UIMessage?.Timebase?.AcqLength ?? AnaChnlStorageMode.Normal;
            Int64 currPerXDivByps = (Int64)((Hd.UIMessage?.Timebase?.TmbScale ?? 1) * 1_000_000);//TmbScale 以us为单位,*1_000_000,us==>ps
            AnaChnlTimebaseIndex anaChnlTimebaseIndex = (AnaChnlTimebaseIndex)(Hd.UIMessage?.Timebase?.TmbScaleIndex ?? (int)AnaChnlTimebaseIndex.Lv100m);
            AcquingParameters.SettingTrigPositionByfs = currPerXDivByps * 1000 * Constants.VIS_XDIVS_NUM / 2 - (long)((Hd.UIMessage?.Timebase?.TmbPosition ?? 0) * uS2fs);//1000,ps=>fs

            if (!NormalStorageExtractNumInterpolationNumTable.ContainsKey(anaChnlTimebaseIndex))
                return;
            if (!AcquingParameters.bIsLongStorageMode)
            {
                AnaChnlTimebaseIndex currAnaChnlTimebaseIndex = anaChnlTimebaseIndex;
                AcquingParameters.ExtractNumFromAdc = NormalStorageExtractNumInterpolationNumTable[currAnaChnlTimebaseIndex].ExtractNum;
                // AcquingParameters.SoftwareFifoLength = NormalStorageExtractNumInterpolationNumTable[currAnaChnlTimebaseIndex].SoftwareFifoLen;

                AcquingParameters.PerDataByfs_AtDdr = MaxPerDataByps * 1000.0 * AcquingParameters.ExtractNumFromAdc;
                //计算方法与硬件密切先关，请与硬件数据的组合沟通确认。目前的算法：1000 和1024，是为了转换为1024的整数倍，而不是1000的整数倍；8=各个通道+LA 一个点占用的总字节数，12(ADC位数)*4(通道) +16(LA的路数)/8(byte位宽)
            }
            else
            {
            }
        }

        /// <summary>
        /// 将dmaBuff中的数据解析结果缓存至ChannelDataDictionary中
        /// </summary>
        /// <param name="dmaBuff">DMA读取得到的原始buff</param>
        /// <param name="dotCount">此次可以解析出的单通道数据量</param>
        /// <param name="ChannelDataDictionary">用来缓存通道数据解析结果的缓存区</param>
        /// <param name="clearFlag">是否需要清除已缓存的解析结果</param>
        protected override void AnalogChannelDataSplit(UInt32 includeChannelBit,Byte[] dmaBuff, Int32 dotCount, List<List<UInt16>> ChannelDataDictionary, Boolean clearFlag)
        {
            if (clearFlag)
            {
                foreach (var channelData in ChannelDataDictionary)
                    channelData.Clear();
            }
            var dmaBuffInidexMax = dmaBuff.Length / 8;
            dmaBuffInidexMax = dmaBuffInidexMax < dotCount ? dmaBuffInidexMax : dotCount;
            for (int channelId = 0; channelId < ChannelDataDictionary.Count; channelId++)
            {
                for (int dotIndex = 0; dotIndex < dmaBuffInidexMax; dotIndex++)
                {
                    ushort data = channelId switch
                    {
                        0 => (UInt16)((dmaBuff[8 * dotIndex]) | ((dmaBuff[8 * dotIndex + 1] & 0x0F) << 8)),
                        1 => (UInt16)(((dmaBuff[8 * dotIndex + 1] & 0xF0) >> 4 | dmaBuff[8 * dotIndex + 2] << 4)),
                        2 => (UInt16)((dmaBuff[8 * dotIndex + 3] | (dmaBuff[8 * dotIndex + 4] & 0x0F) << 8)),
                        3 => (UInt16)(((dmaBuff[8 * dotIndex + 4] & 0xF0) >> 4 | dmaBuff[8 * dotIndex + 5] << 4)),
                        _ => 0,
                    };
                    ChannelDataDictionary[channelId].Add(data);
                }
            }
            DiscardDotAtTriggerTypeIsSerialMode();
            AbstractController_AnalogChannel.SoftwareBandwidthProcess();
        }
    }
}
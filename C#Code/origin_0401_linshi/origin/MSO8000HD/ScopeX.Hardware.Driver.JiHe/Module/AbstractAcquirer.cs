using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    public abstract class AbstractAcquirer
    {
        internal const long uS2fs = 1_000_000_000;

        internal virtual AcqDataType DataType { get; set; }
        internal virtual void CreateAcquireAttribute() { }
        internal virtual bool bDataVaild { get; set; }
        /// <summary>
        /// 上电缺省初始化，与系统的环境变量无关
        /// </summary>
        internal virtual void Init() { }
        /// <summary>
        /// 开启下一次新的采集
        /// </summary>
        internal virtual void InitAcq() { }
        /// <summary>
        /// 读取采集到的数据
        /// </summary>
        /// <returns></returns>
        internal virtual bool ReadAcqData(List<ReadInfo> readInfoList, out double SamplingRateByus, CancellationToken? softResetToken)
        {
            SamplingRateByus = 1.0;
            return false;
        }
        /// <summary>
        /// 采集数据的后处理
        /// </summary>
        internal virtual void PostProcess(List<ReadInfo> readInfoList, CancellationToken? softResetToken) { }
        internal virtual bool LongStorageIsFull() { return true; }
        internal Boolean AcqFulled
        {
            get;
            set;
        } = false;
        internal virtual void Reset()
        {
            AcqFulled = false;
        }
        internal (long NeedMinDotsCount, long ExtractTotalNum) CalcUI_NeedMinDotsCount(long StorageWaveDotsCnt)
        {
            long ExtractTotalNum = 1;
            long hardwareStorageWaveDotsCnt = StorageWaveDotsCnt;
            int minDotsCount = hardwareStorageWaveDotsCnt.ToString()[0] switch
            {
                '5' => 50000,
                '1' => 100000,
                _ => 25000,
            };
            while ((hardwareStorageWaveDotsCnt / 10) >= minDotsCount)
            {
                ExtractTotalNum *= 10;
                hardwareStorageWaveDotsCnt /= 10;
            }
            return (hardwareStorageWaveDotsCnt, ExtractTotalNum);
        }
        internal virtual void SetWrittedTimeStamp() { }
        internal static (UInt32 Base, UInt32 Multiple) SplitExtractNum(AdcInterleaveMode InterleaveMode, Boolean bIsPeak, ulong totalExtractNum)
        {
            //return (50u,2u);
            UInt32 mergeRoads = InterleaveMode switch
            {
                AdcInterleaveMode.Mode2To1 => 16,
                AdcInterleaveMode.Mode4To1 => 32,
                _ => 8,
            };
            ulong pre_sample = totalExtractNum;
            UInt32 preGap = 1;
            if (pre_sample < 1)
            {
                pre_sample = 1;
            }
            if (InterleaveMode == AdcInterleaveMode.Mode1To1)
            {
                if (pre_sample % 10 == 0)
                    preGap = 10;
                else if (pre_sample == 25)
                    preGap = 25;
                else
                    preGap = (UInt32)pre_sample % 10;
            }
            else
            {
                if (pre_sample % 50 == 0)
                    preGap = 50;
                else
                {
                    if (PreExtractGapModeList.Keys.Select(o => o == (uint)pre_sample).Count() == 0)
                    {
                        Hd.SysLogger?.Invoke("At SplitExtractNum,can't split {InterleaveMode},bIsPeak={bIsPeak},totalExtractNum={totalExtractNum}", "Warning");
                    }
                    preGap = (UInt32)pre_sample;//0 pre_sample;
                }
            }
            //if (Hd.UIMessage.Timebase.AcqMode == AnaChnlAcqMode.Peak && pre_sample > 1)
            //{
            //    pre_sample *= 2;
            //}

            //if (pre_sample <= mergeRoads) // 1 or 2,预抽点数小于抽点倍数
            //{
            //    preGap = (UInt32)pre_sample;
            //}
            //else if (pre_sample % mergeRoads == 0) // 2 or 4,预抽点数等于抽点倍数
            //{
            //    preGap = mergeRoads;
            //}
            //else if (mergeRoads >= 32 && 1000 % 50 == 0) // 25
            //{
            //    preGap = 50;
            //}
            //else if (mergeRoads >= 16 && pre_sample % 50 == 0) // 25
            //{
            //    preGap = 50;//25
            //}
            //else if (mergeRoads >= 16 && pre_sample % 20 == 0) // 25
            //{
            //    preGap = 20;
            //}

            //else if (mergeRoads >= 8 && pre_sample % 10 == 0) // 5
            //{
            //    preGap = 10;
            //}

            //else if (pre_sample < mergeRoads && pre_sample % 5 == 0) // 5
            //{
            //    preGap = (UInt32)pre_sample;
            //}
            //else
            //{
            //    preGap = 1;
            //}

            pre_sample /= preGap;
            if (bIsPeak)
            {
                pre_sample *= 2;
            }
            return (preGap, (UInt32)pre_sample);
        }
        internal static ulong FitExtractNum(AdcInterleaveMode interleaveMode, ulong totalExtractNum)
        {
            ulong fitextractnum = totalExtractNum;
            //return (50u,2u);
            Double pre_sample = totalExtractNum;

            UInt64 scale = 1;
            while (pre_sample >= 10)
            {
                pre_sample = pre_sample / 10;
                scale = scale * 10;
            }
            if (pre_sample <= 1)
            {
                fitextractnum = scale;
            }
            else if (pre_sample <= 2)
            {
                fitextractnum = 2 * scale;
            }
            else if (pre_sample <= 4)
            {
                fitextractnum = 4 * scale;
            }
            else
            {
                fitextractnum = 10 * scale;
            }
            return fitextractnum;
        }
        internal static ulong FitInterpolateNum(AdcInterleaveMode interleaveMode, ulong interpolateNum)
        {
            ulong fitinterpolatenum = interpolateNum;
            if (interleaveMode == AdcInterleaveMode.Mode1To1)
            {
                if (interpolateNum <= 1)
                {
                    fitinterpolatenum = 1;
                }
                else if (interpolateNum <= 2)
                {
                    fitinterpolatenum = 2;
                }
                else if (interpolateNum <= 5)
                {
                    fitinterpolatenum = 5;
                }
                else if (interpolateNum <= 10)
                {
                    fitinterpolatenum = 10;
                }
                else if (interpolateNum <= 20)
                {
                    fitinterpolatenum = 20;
                }
                else if (interpolateNum <= 50)
                {
                    fitinterpolatenum = 50;
                }
                else
                {
                    fitinterpolatenum = 100;
                }
                return fitinterpolatenum;
            }
            else
            {
                if (interpolateNum <= 1)
                {
                    fitinterpolatenum = 1;
                }
                else if (interpolateNum <= 5)
                {
                    fitinterpolatenum = 5;
                }
                else if (interpolateNum <= 10)
                {
                    fitinterpolatenum = 10;
                }
                else if (interpolateNum <= 50)
                {
                    fitinterpolatenum = 50;
                }
                else
                {
                    fitinterpolatenum = 100;
                }
                return fitinterpolatenum;
            }
        }
        internal static Dictionary<uint, uint> PreExtractGapModeList = new Dictionary<uint, uint>()
        {
            [0] = 1,
            [1] = 1,
            [2] = 2,
            [4] = 4,
            [5] = 8,
            [8] = 0x10,
            [10] = 0x20,
            [16] = 0x40,
            [20] = 0x80,
            [25] = 0x100,
            [40] = 0x200,
            [50] = 0x400,
            [100] = 0x800,
        };
    }
}

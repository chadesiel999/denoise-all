using function_ai_trig;
using MathWorks.MATLAB.NET.Arrays;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Driver.PlatForm;
using ScopeX.MathExt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 模拟通道波形采集
    /// </summary>
    public abstract partial class AbstractAcquirer_AnalogChannel : AbstractAcquirer, IAdcInterleave
    {
        /// <summary>
        /// 模拟通道采集模块
        /// </summary>
        internal AbstractAnalogAcquireModel? AnalogAcquireModel;

        internal AcqDataType AcqDataType { get => AcqDataType.AnalogChannel; }

        /// <summary>
        /// 正在采集的采集参数
        /// </summary>
        internal readonly AcquireAttribute AcquingParameters = new AcquireAttribute();
        /// <summary>
        /// 已经采集的数据的采集参数。在读回数据后赋值
        /// </summary>
        internal readonly AcquireAttribute AcquedParameters = new AcquireAttribute();

        #region 实现IAdcInterleave
        public virtual IReadOnlyList<KeyValuePair<String, Int32>> PerAdcStorageLength { get; } = new List<KeyValuePair<String, Int32>>();

        public virtual AdcInterleaveMode GetCurrentMode() => AdcInterleaveMode.Mode1To1;

        #endregion

        #region 深存储相关接口定义，如果不需要深存储功能，移除AbstractAcquirer_AnalogChannel_LongStorage.cs即可
        protected static Action? _LongStorageInit;
        protected static Action? _LongStorageInitAcq;
        protected static Func<List<ReadInfo>, CancellationToken?, Boolean>? _LongStorageReadAcq;
        protected static Func<ChannelId, ReadInfo, List<UInt16>, WfmSampleInfo, CancellationToken?, Boolean>? _LongStorageTakeWave;
        protected static Func<ChannelId, ReadInfo, Int32, Int32, List<UInt16[]>, WfmSampleInfo, CancellationToken?, Boolean, Boolean>? _LongStorageTakeSegmentWave;
        protected static Func<ChannelId, ReadInfo, FileStream?, String, CancellationToken?, Int32>? _LongStorageSaveSourceData;
        protected static Func<Int32>? _LongStorageReadCollectedFrameCnt;
        #endregion
        #region Fifo相关设置
        protected static Action? _FifoInitAcq;
        protected static Func<List<ReadInfo>, CancellationToken?, Boolean>? _FifoReadAcq;
        #endregion
        protected static Action<Int32, Int32, Int32, Int32>? _LongStorageConfig;
        public AbstractAcquirer_AnalogChannel()
        {
            _FifoInitAcq = FifoInitAcq;
            _FifoReadAcq = FifoReadAcq;

            _LongStorageInit = HdCtrl_AnalogDDR.MigReset;
            _LongStorageInitAcq = LongStorageInitAcq;
            _LongStorageReadAcq = LongStorageReadAcq;
            _LongStorageTakeWave = LongStorageTakeWave;
            _LongStorageTakeSegmentWave = LongStorageTakeSegmentWave;
            _LongStorageReadCollectedFrameCnt = ReadCollectedFrameCnt;
            _LongStorageSaveSourceData = LongStorageSaveSourceData;

            for (int channelId = 0; channelId < ChannelIdExt.AnaChnlNum; channelId++)
            {
                _ChnlData.Add(new List<UInt16>());
                AcqedDataPool.AnalogChData.AllChannelData.Add(new List<UInt16>());
            }
        }

        internal DBI_CoefTableSendItem? GetAcqDefineItem(DbiCoefficientsTablesType dbiType, ChannelId chnlId, Int32 subbandId)
        {
            if (AcqBdCoefTablesSendDefine == null || (!AcqBdCoefTablesSendDefine.ContainsKey(dbiType)))
                return null;

            foreach (DBI_CoefTableSendItem defineItem in AcqBdCoefTablesSendDefine[dbiType])
            {
                if (defineItem.ChannelID == chnlId && defineItem.SubbandIndex == subbandId && defineItem.ChnlScaleIndex == AnaChnlScaleIndex.Lv20m)
                {
                    return defineItem;
                }
            }
            return null;
        }

        internal DBI_CoefTableSendItem? GetAcqDefineItemByLevel(DbiCoefficientsTablesType dbiType, ChannelId chnlId, Int32 subbandId, AnaChnlScaleIndex key)
        {
            if (AcqBdCoefTablesSendDefine == null || (!AcqBdCoefTablesSendDefine.ContainsKey(dbiType)))
                return null;

            foreach (DBI_CoefTableSendItem defineItem in AcqBdCoefTablesSendDefine[dbiType])
            {
                if (defineItem.ChannelID == chnlId && defineItem.SubbandIndex == subbandId && defineItem.ChnlScaleIndex == key)
                {
                    return defineItem;
                }
            }
            return null;
        }
        internal virtual Dictionary<int /*mergeMode*/, List<List<ChannelBdAdcInputDefine>>>? ChannelBdAdcInputDefines
        {
            get;
        }

        /// <summary>
        /// 表示每个子带是否有信号能量
        /// </summary>
        internal Dictionary<Int32, Boolean> SubbandEnergyTable = new();

        Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>> EmptyAcqBdCoefTablesSendDefine = new Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>>();
        /// <summary>
        /// 获取采集板各种系数表的发送配置。只有DBI项目存在！！！
        /// </summary>
        internal virtual Dictionary<DbiCoefficientsTablesType, List<DBI_CoefTableSendItem>> AcqBdCoefTablesSendDefine
        {
            get => EmptyAcqBdCoefTablesSendDefine;
        }
        internal virtual Dictionary<string, AmpCoefficientFileInfo>? ChannelPerScaleAmpFreqCoefficientsDefine
        {
            get => ourChannelPerScaleAmpFreqCoefficientsDefine;
        }
        private readonly Dictionary<string, AmpCoefficientFileInfo>? ourChannelPerScaleAmpFreqCoefficientsDefine = new()
        {
            ["C1_1"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C1_1mV.txt" },
            ["C1_2"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C1_2mV.txt" },
            ["C1_5"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C1_5mV.txt" },
            ["C1_10"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C1_10mV.txt" },
            ["C1_20"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C1_20mV.txt" },
            ["C1_50"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C1_50mV.txt" },
            ["C1_100"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C1_100mV.txt" },
            ["C1_200"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C1_200mV.txt" },
            ["C1_500"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C1_500mV.txt" },
            ["C1_1000"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C1_1000mV.txt" },

            ["C2_1"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C2_1mV.txt" },
            ["C2_2"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C2_2mV.txt" },
            ["C2_5"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C2_5mV.txt" },
            ["C2_10"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C2_10mV.txt" },
            ["C2_20"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C2_20mV.txt" },
            ["C2_50"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C2_50mV.txt" },
            ["C2_100"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C2_100mV.txt" },
            ["C2_200"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C2_200mV.txt" },
            ["C2_500"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C2_500mV.txt" },
            ["C2_1000"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C2_1000mV.txt" },

            ["C3_1"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C3_1mV.txt" },
            ["C3_2"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C3_2mV.txt" },
            ["C3_5"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C3_5mV.txt" },
            ["C3_10"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C3_10mV.txt" },
            ["C3_20"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C3_20mV.txt" },
            ["C3_50"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C3_50mV.txt" },
            ["C3_100"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C3_100mV.txt" },
            ["C3_200"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C3_200mV.txt" },
            ["C3_500"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C3_500mV.txt" },
            ["C3_1000"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C3_1000mV.txt" },

            ["C4_1"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C4_1mV.txt" },
            ["C4_2"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C4_2mV.txt" },
            ["C4_5"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C4_5mV.txt" },
            ["C4_10"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C4_10mV.txt" },
            ["C4_20"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C4_20mV.txt" },
            ["C4_50"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C4_50mV.txt" },
            ["C4_100"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C4_100mV.txt" },
            ["C4_200"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C4_200mV.txt" },
            ["C4_500"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C4_500mV.txt" },
            ["C4_1000"] = new AmpCoefficientFileInfo() { FileName = $@".\AfcCaliDataFiles\Afc_C4_1000mV.txt" },
        };

        internal virtual List<List<ChannelBdAdcInputDefine>>? FirstInitChannelBdAdcInputDefines => null;

        internal UInt64 MaxPerDataByps
        {
            get;
            set;
        } = 50;//20G 采样
        internal record ExtractNumInterpolationNumPair(UInt64 ExtractNum, UInt32 InterpolationNum, UInt32 HardwareFifoLen, UInt32 SoftwareFifoLen);

        internal override void Init()
        {
            // InitPhyAnalogChAmplitudeTemperaturesCompensationCoefficient();
            InitAmpCoefficientFile();
            _LongStorageInit?.Invoke();
        }
        internal virtual void InitAmpCoefficientFile()
        {
            if (ChannelPerScaleAmpFreqCoefficientsDefine == null)
            {
                return;
            }

            foreach (var v in ChannelPerScaleAmpFreqCoefficientsDefine)
            {
                var info = v.Value;
                int[]? dataArray = Misc.ReadCaliCoefDataFronmFile(info.FileName);
                if (dataArray == null)
                {
                    info.CRCCode = 0; info.bOk = false;
                    continue;
                }
                info.CRCCode = Misc.GenerateCRCCode(dataArray!);
                info.bOk = true;
            }
        }
        internal virtual HdCmd ResponseSpecialScpiCmd(string message)
        {
            return HdCmd.None;
        }
        protected virtual bool bOpenAverageModule
        {
            get
            {
                if (Hd.CurrDebugVarints.bEnable_ProcBd_Average)
                {
                    if (AcquingParameters.ExtractNumFromAdc > 1)
                    {
                        return false;
                    }

                    if (Hd.UIMessage!.Timebase!.AcqLength == AnaChnlStorageMode.Long)
                    {
                        return false;
                    }

                    if (Hd.UIMessage!.Trigger!.Mode != TriggerMode.Auto)
                    {
                        return false;
                    }

                    if (Hd.UIMessage!.Display!.IsFast)
                    {
                        return false;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// 不抽不插的ADC采样间隔
        /// </summary>
        protected Double _AdcSampleIntervalByps = 100;// 10GSPS

        /// <summary>
        /// 不同交织模式下采样间隔的倍率定义
        /// </summary>
        private Dictionary<AdcInterleaveMode, Double> _RatioDefine = new()
        {
            [AdcInterleaveMode.Mode1To1] = 4.0,
            [AdcInterleaveMode.Mode2To1] = 2.0,
            [AdcInterleaveMode.Mode4To1] = 1.0,
        };

        internal virtual Double GetAdcSampleIntervalByps(AcquireAttribute acquireAttribute)
        {
            if (_RatioDefine.ContainsKey(acquireAttribute.AdcInterleaveMode))
                return _AdcSampleIntervalByps * _RatioDefine[acquireAttribute.AdcInterleaveMode];
            return _AdcSampleIntervalByps;
        }

        internal virtual UInt32 GetAdcMergeRoadCount(AcquireAttribute acquireAttribute)
        {
            return acquireAttribute.AcqAdcMergeRoadCount;
        }

        protected Int32 _MaxExpectedDotsCnt = 10_000;
        protected Int32 _MinExpectedDotsCnt = 2_000;

        internal virtual void CalcExtramInterplotNum(AcquireAttribute acquireAttribute)
        {
            if (acquireAttribute.HdMessage?.Timebase == null)
                return;

            Double needWaveSumTimeByus = acquireAttribute.HdMessage.Timebase.TmbScale * Constants.VIS_XDIVS_NUM;
            Double sampleIntervalInAdcByUs = GetAdcSampleIntervalByps(acquireAttribute) / ConstDefine.Ratio_u2p;
            Double dotsCntOrigin = needWaveSumTimeByus / sampleIntervalInAdcByUs;
            UInt32 mergeRoadCount = acquireAttribute.AcqAdcMergeRoadCount;
            AnaChnlAcqMode acqMode = acquireAttribute.HdMessage.Timebase.AcqMode;

            if (acquireAttribute.bIsLongStorageMode)
            {
                acquireAttribute.ExtractNumFromAdc = 1;
                acquireAttribute.ExtramNumToDMA = Hd.CurrProduct?.ExtramModule?.GetValidExtramNum((UInt64)(dotsCntOrigin / _MaxExpectedDotsCnt), ExtramType.Preceding, acqMode, mergeRoadCount) ?? 1;
                acquireAttribute.InterplotNumFromADC = 1;
                acquireAttribute.InterplotNumToDMA = Hd.CurrProduct?.InterpModule?.GetValidInterpNum(_MinExpectedDotsCnt / dotsCntOrigin) ?? 1;
            }
            else
            {
                acquireAttribute.ExtractNumFromAdc = Hd.CurrProduct?.ExtramModule?.GetValidExtramNum((UInt64)(dotsCntOrigin / _MaxExpectedDotsCnt), ExtramType.Preceding, acqMode, mergeRoadCount) ?? 1;
                if (needWaveSumTimeByus >= 500_000)
                    acquireAttribute.ExtractNumFromAdc /= 10;
                acquireAttribute.ExtramNumToDMA = 1;
                acquireAttribute.InterplotNumFromADC = 1;
                acquireAttribute.InterplotNumToDMA = Hd.CurrProduct?.InterpModule?.GetValidInterpNum(_MinExpectedDotsCnt / dotsCntOrigin) ?? 1;
            }
        }

        internal override void CreateAcquireAttribute()
        {
            if (Hd.UIMessage == null)
                return;
            AcquingParameters.HdMessage = Hd.UIMessage with { };

            UInt32 chnlActiveState = 0;
            for (int channelId = (int)ChannelId.C1; channelId < ChannelIdExt.AnaChnlNum; channelId++)
            {
                if (AcquingParameters.HdMessage.Analog?[channelId].Active ?? false)
                    chnlActiveState |= 0x1u << channelId;//独热码，1、3通道打开，则cAS = 0b0101;
            }
            AcquingParameters.ChnlActiveState = chnlActiveState;

            var acqModeInterleave = AnalogAcquireModel?.GetAcqModeInterleaveByChnlState(chnlActiveState);
            AcquingParameters.AdcInterleaveMode = acqModeInterleave?.InterleaveMode ?? AdcInterleaveMode.Mode1To1;
            AcquingParameters.AcqAdcMergeRoadCount = GetAdcMergeRoadCount(AcquingParameters);
            AcquingParameters.HardwareStorageWaveDotsCnt = Hd.UIMessage?.Timebase?.StorageWaveDotsCnt ?? -1;

            CalcExtramInterplotNum(AcquingParameters);

            AcquingParameters.PerDataByfs_AtDdr = GetAdcSampleIntervalByps(AcquingParameters) * AcquingParameters.ExtractNumFromAdc * ConstDefine.Ratio_p2f / AcquingParameters.InterplotNumFromADC;
            AcquingParameters.PerDataByfs_AtDMA = AcquingParameters.PerDataByfs_AtDdr * AcquingParameters.ExtramNumToDMA / AcquingParameters.InterplotNumToDMA;

            Double needWaveSumTimeByus = Constants.VIS_XDIVS_NUM * AcquingParameters.HdMessage?.Timebase?.TmbScale ?? 0;
            AcquingParameters.DmaReadDotsCnt = (UInt32)(needWaveSumTimeByus * uS2fs / AcquingParameters.PerDataByfs_AtDMA);
        }

        AcquireAttribute[] FrameNo_AcquireAttributeArray = new AcquireAttribute[65000];
        int FrameNo_AcquireAttributeArrayIndex = 0;
        ushort FrameNo = 0;
        internal void FrameNo_AcquireAttribute_Push(AcquireAttribute attribute)
        {
            FrameNo_AcquireAttributeArrayIndex = (FrameNo_AcquireAttributeArrayIndex + 1) % 65000;
            FrameNo = (ushort)(FrameNo + 1);
            attribute.FrameNo = FrameNo;
            attribute.CreateDateTime = DateTime.Now;
            var pushedAttribute = new AcquireAttribute();
            attribute.CloneTo(pushedAttribute);
            FrameNo_AcquireAttributeArray[FrameNo_AcquireAttributeArrayIndex] = pushedAttribute;
        }
        internal AcquireAttribute? FrameNo_AcquireAttribute_Get(ushort frameNo)
        {
            for (int index = 0; index < 65000; index++)
            {
                if (FrameNo_AcquireAttributeArray[index] != null && FrameNo_AcquireAttributeArray[index].FrameNo == FrameNo)
                    return FrameNo_AcquireAttributeArray[index];
            }
            return null;
        }
        internal override void InitAcq()
        {
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_StorageMode, AcquingParameters.bIsLongStorageMode ? 1 : 0U);
            //Acquisition.SwitchDataPathMuxTo(DMAReadSourceMuxType.AnalogChanneData);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.Average_Enable, bOpenAverageModule ? 1U : 0);
            if (!AcquingParameters.bIsLongStorageMode)
                _FifoInitAcq?.Invoke();
            else
                _LongStorageInitAcq?.Invoke();
        }

        protected virtual void AnalogChannelDataSplit(ChannelId chnlId, Byte[] dmaBuff, Int32 perChannelValidDotCount, List<Byte> ChannelDataList)
        {
            List<ChannelId> chnlidingroup = new()
            {
                ChannelId.C1, ChannelId.C2, ChannelId.C3, ChannelId.C4,
            };

            var groupcount = dmaBuff.Length / 2 / chnlidingroup.Count;
            groupcount = groupcount < perChannelValidDotCount ? groupcount : perChannelValidDotCount;

            Int32 chnlIndex = chnlidingroup.FindIndex(o => o == chnlId);

            for (int groupIndex = 0; groupIndex < groupcount; groupIndex++)
            {
                int dataStartIndex = (groupIndex * chnlidingroup.Count + chnlIndex) * 2;
                ChannelDataList.Add(dmaBuff[dataStartIndex]);
                ChannelDataList.Add(dmaBuff[dataStartIndex + 1]);
            }
        }
		internal virtual List<UInt16> ParseDMAData(Int32 channelId, Byte[] dmaBuff)
        {
            return new List<UInt16>();
        }
		
		internal virtual Int32[] GetStorageDotsCnt(ChannelId[] activeChnl)
        {
            var coreCntArray = Hd.CurrProduct?.AnalogAcquireModel?.GetUsedCoreCntOfAdc(activeChnl);
            Int32 coreCnt = 1;
            if (coreCntArray != null && coreCntArray.Count() != 0)
            {
                coreCnt = coreCntArray.Min();
            }

            if (_StorageDotsCntPerCore != null)
                return _StorageDotsCntPerCore.Select(o => o * coreCnt).ToArray();
            return new Int32[0];
        }
        protected virtual Double GetAdcSampleIntervalByUs()
        {
            Double ratio = 1e-3;
            switch (AcquingParameters.AdcInterleaveMode)
            {
                case AdcInterleaveMode.Mode1To1:
                    ratio *= 4.0;
                    break;
                case AdcInterleaveMode.Mode2To1:
                    ratio *= 2.0;
                    break;
                default:
                    break;
            }
            return MaxPerDataByps * ratio;
        }
		
        protected virtual void AnalogChannelDataSplit(Byte[] dmaBuff, Int32 perChannelValidDotCount, Boolean clearFlag = true)
        {
            Int32 chnlCnt = ChannelIdExt.AnaChnlNum;
            while (AcqedDataPool.AnalogChData.AllChannelData.Count < chnlCnt)
                AcqedDataPool.AnalogChData.AllChannelData.Add(new List<ushort>());

            if (clearFlag)
            {
                foreach (var channelData in AcqedDataPool.AnalogChData.AllChannelData)
                    channelData.Clear();
            }

            Int32 dotCnt = dmaBuff.Length / 8;
            if (perChannelValidDotCount < dotCnt)
                dotCnt = perChannelValidDotCount;

            for (int channelId = 0; channelId < chnlCnt; channelId++)
            {
                for (int dotIndex = 0; dotIndex < dotCnt; dotIndex++)
                {
                    ushort data = channelId switch
                    {
                        0 => (UInt16)((dmaBuff[8 * dotIndex]) | ((dmaBuff[8 * dotIndex + 1] & 0x0F) << 8)),
                        1 => (UInt16)(((dmaBuff[8 * dotIndex + 1] & 0xF0) >> 4 | dmaBuff[8 * dotIndex + 2] << 4)),
                        2 => (UInt16)((dmaBuff[8 * dotIndex + 3] | (dmaBuff[8 * dotIndex + 4] & 0x0F) << 8)),
                        3 => (UInt16)(((dmaBuff[8 * dotIndex + 4] & 0xF0) >> 4 | dmaBuff[8 * dotIndex + 5] << 4)),
                        _ => 0,
                    };
                    AcqedDataPool.AnalogChData.AllChannelData[channelId].Add(data);
                }
            }
        }
		
	
		/// <summary>
        /// 将dmaBuff中的数据解析结果缓存至ChannelDataDictionary中
        /// </summary>
        /// <param name="inculdeChannelBit"></param>
        /// <param name="dmaBuff">DMA读取得到的原始buff</param>
        /// <param name="perChannelValidDotCount">此次可以解析出的单通道数据量</param>
        /// <param name="ChannelDataList">用来缓存通道数据解析结果的缓存区</param>
        /// <param name="clearFlag">是否需要清除已缓存的解析结果</param>
        protected virtual void AnalogChannelDataSplit(UInt32 inculdeChannelBit, Byte[] dmaBuff, Int32 perChannelValidDotCount, List<List<UInt16>> ChannelDataList, Boolean clearFlag)
        {
            if (clearFlag)
            {
                foreach (var channelData in ChannelDataList)
                {
                    channelData.Clear();
                }
            }

            //通道在每组数据的格式
            List<ChannelId> chnlidingroup;
            Int32 groupvalidcount;
            if (inculdeChannelBit == 0xFU)
            {
                //4通道全开
                chnlidingroup = new List<ChannelId>() { ChannelId.C1, ChannelId.C2, ChannelId.C3, ChannelId.C4 };
                groupvalidcount = perChannelValidDotCount;
            }
            else
            {
                //只开1，3通道
                chnlidingroup = new List<ChannelId>() { ChannelId.C1, ChannelId.C2, ChannelId.C3, ChannelId.C4 };
                groupvalidcount = perChannelValidDotCount;
            }

            var groupcount = dmaBuff.Length / 2 / chnlidingroup.Count;//2=2字节
            groupcount = groupcount < groupvalidcount ? groupcount : groupvalidcount;
            for (int groupIndex = 0; groupIndex < groupcount; groupIndex++)
            {
                //遍历组
                for (int chnlIndex = 0; chnlIndex < chnlidingroup.Count; chnlIndex++)
                {
                    //遍历组内通道
                    int dataStartIndex = (groupIndex * chnlidingroup.Count + chnlIndex) * 2;//2=2字节
                    ushort data = (UInt16)(dmaBuff[dataStartIndex] | (dmaBuff[dataStartIndex + 1] << 8));
                    int chnlId = (int)chnlidingroup[chnlIndex];
                    ChannelDataList[chnlId].Add(data);
                }
            }
            DiscardDotAtTriggerTypeIsSerialMode();
            AbstractController_AnalogChannel.SoftwareBandwidthProcess();
        }
        protected virtual void SpecialConfig()
        {

        }

        /// <summary>
        /// 在发完读参数后执行，读取PCIe缓存区的数据
        /// </summary>
        /// <param name="dataTypes">想要读取的数据类型</param>
        /// <param name="dataLength">想要读取的数据个数</param>
        /// <param name="dmaBuff">用来缓存数据的buff</param>
        /// <returns></returns>
        internal virtual Boolean ReadDMA(DMAReadDataTypes dataType, UInt32 dataLength, Byte[] dmaBuff)
        {
            Hd.CurrProduct!.AcqBd!.SwitchDataPathAndPcieReset_SetDataLength(dataType, dataLength);

            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);

            var retVal = HdIO.DMARead(dataLength, ref dmaBuff);
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
            return retVal;
        }

        internal virtual Boolean ReadDMAAnorm(DMAReadDataTypes dataType, UInt32 dataLength, Byte[] dmaBuff)
        {
            Hd.CurrProduct!.AcqBd!.SwitchDataPathAndPcieReset_SetDataLength(dataType, dataLength);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.anormdetect_emd_rd_en, 0x0001);
            //HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
            //HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);

            var retVal = HdIO.DMARead(dataLength, ref dmaBuff);
            //HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
            return retVal;
        }

        internal override bool ReadAcqData(List<ReadInfo> readInfoList, out double samplingRateByus, CancellationToken? softResetToken)
        {
            bDataVaild = false;
            bool bOk = false;
            if (Hd.UIMessage == null)
            {
                samplingRateByus = 1.0;
                return bOk;
            }
            if (!Acquisition.bReadOldData)
            {
                AcquingParameters.CloneTo(AcquedParameters);
            }
            samplingRateByus = AcquedParameters.PerDataByfs_AtDdr / ConstDefine.Ratio_u2f;
            if (Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation)
            {
                if (Hd.UIMessage!.Timebase!.TmbScale < 0.02)
                    samplingRateByus /= 4;
            }

            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen || Hd.BPowerOff)
            {
                if (Hd.UIMessage?.Timebase?.IsScan ?? false)
                {
                    //模拟
                    Random random = new Random();
                    int a = random.Next(100, 1000);
                    AcqAnalogChannelSimulateWaveform();
                    for (int iChannelID = 0; iChannelID < ChannelIdExt.AnaChnlNum; iChannelID++)
                    {
                        AcqedDataPool.AnalogChData.AllChannelData[iChannelID].RemoveRange(a, AcqedDataPool.AnalogChData.AllChannelData[iChannelID].Count - a);
                    }
                    return true;
                }
                else
                {
                    return AcqAnalogChannelSimulateWaveform();
                }
            }
            if (!AcquingParameters.bIsLongStorageMode)
            {
                return _FifoReadAcq?.Invoke(readInfoList, softResetToken) ?? false;
            }
            else
            {
                return _LongStorageReadAcq?.Invoke(readInfoList, softResetToken) ?? false;
            }
        }
        AiTrig aiTrig = new();
        internal override void PostProcess(List<ReadInfo> readInfoList, CancellationToken? softResetToken)
        {
            if (!AcquingParameters.bIsLongStorageMode)
            {
                if (IsNeedPostProcessByMatlab)
                {
                    #region 需要MATLAB处理

                    #endregion
                }
            }
            else
            {
                //长存储时的处理
                // LongStorage?.PostProcess();
            }
            try
            {
                var info = readInfoList.Where(o => o.Mark == "View").First();
                MWNumericArray bf = new MWNumericArray(AcqedDataPool.AnalogChData.AllChannelData[0].Select(o => (double)o).ToArray());
                MWArray[] res = aiTrig.function_ai_trig(2, bf, 6000, 3000, 10);
                MWNumericArray tmp = (MWNumericArray)res[1];
                int flag = (int)tmp[1];
                if (flag == 1)
                {
                    MWNumericArray tmp1 = (MWNumericArray)res[0];
                    int triLevel = (int)tmp1[1];
                }

            }
            catch (Exception e)
            {
                ;
            }

        }

        private UInt16[] FixEdgeTrig(UInt16[] source, UInt16 trigVoltage, Int32 trigPos, Int32 fixRange, Boolean isRise)
        {
            Int32 endpos = trigPos + fixRange;
            for (Int32 id = trigPos; id < endpos; id++)
            {
                if (isRise && source[id] <= trigVoltage && source[id + 1] >= trigVoltage)
                    return source.Skip(id - trigPos).ToArray();
                if (!isRise && source[id] >= trigVoltage && source[id + 1] <= trigVoltage)
                    return source.Skip(id - trigPos).ToArray();
            }
            return source;
        }

        #region Core->U2调用函数
        public virtual Boolean TryTakeWave(ChannelId chnlId, ReadInfo readInfo, [NotNullWhen(true)] out List<ushort> waveData, [NotNullWhen(true)] out WfmSampleInfo wfmSampleInfo, CancellationToken? softResetToken)
        {
            waveData = new();
            wfmSampleInfo = new();
            wfmSampleInfo.StartTimeByus = Hd.UIMessage!.Timebase!.TmbScale * Constants.VIS_XDIVS_NUM / 2 - (long)Hd.UIMessage!.Timebase!.TmbPosition;

            if (AcquedParameters.bIsLongStorageMode)
                return _LongStorageTakeWave?.Invoke(chnlId, readInfo, waveData, wfmSampleInfo, softResetToken) ?? false;

            Monitor.Enter(AcqedDataPool.UpdateDataLock);
            if ((Int32)chnlId >= AcqedDataPool.AnalogChData.AllChannelData.Count)
            {
                Monitor.Exit(AcqedDataPool.UpdateDataLock);
                return false;
            }

            //var tmp = AcqedDataPool.AnalogChData.AllChannelData[(Int32)chnlId];

            var tmp = ArtificialIntelligenceProcess.Default.GetProcessedData(chnlId, out Int32 datatype);
            if (tmp == null)
                tmp = AcqedDataPool.AnalogChData.AllChannelData[(Int32)chnlId];
            List<UInt16> trig = new();

            if (datatype == 1 && tmp != null && chnlId == (Hd.UIMessage?.Trigger?.Edge?.Source ?? ChannelId.C1) &&
                (Hd.UIMessage?.Trigger?.TrigType ?? TriggerType.Edge) == TriggerType.Edge)
            {
                UInt32 trifvol = AbstractAcquirer_AnalogChannel.GetLevelByVoltage(chnlId, Hd.UIMessage?.Trigger?.Edge?.Position ?? 0);
                (Int64 depth, Int64 waveDepth) = Hd.CurrProduct?.Acquirer_AnalogChannel?.GetTrigXDepth() ?? (0, 0);
                Boolean isrise = (Hd.UIMessage?.Trigger?.Edge?.Slope ?? EdgeSlope.Rise) == EdgeSlope.Rise;
                for (Int32 i = 0; i < tmp.Count; i++)
                {
                    trig=(FixEdgeTrig(tmp.ToArray(), (UInt16)trifvol, (Int32)depth, 1000, isrise).ToList());
                }
            }
            else if (tmp != null)
            {
                trig=tmp;
            }
            else
            {
                trig=(AcqedDataPool.AnalogChData.AllChannelData[(Int32)chnlId]);
            }

            waveData.AddRange(trig);
            //当未取到数据时，返回false，保证UI绘图正常，采集大循环不会异常退出
            if (AcqedDataPool.AnalogChData.AllChannelData[(Int32)chnlId].Count <= 0)
            {
                Monitor.Exit(AcqedDataPool.UpdateDataLock);
                return false;
            }
            //waveData.AddRange(AcqedDataPool.AnalogChData.AllChannelData[(Int32)chnlId]);
            ////当未取到数据时，返回false，保证UI绘图正常，采集大循环不会异常退出
            //if (AcqedDataPool.AnalogChData.AllChannelData[(Int32)chnlId].Count <= 0)
            //{
            //    Monitor.Exit(AcqedDataPool.UpdateDataLock);
            //    return false;
            //}
            //waveData.AddRange(AcqedDataPool.AnalogChData.AllChannelData[(Int32)chnlId]);
            Monitor.Exit(AcqedDataPool.UpdateDataLock);
            wfmSampleInfo.SampleIntervalByus = AcquedParameters.PerDataByfs_AtDMA / 1E9;
            wfmSampleInfo.StartTimeByus = 0;// AcquedParameters.HdMessage?.Timebase?.TmbPosition ?? 0.0;
            wfmSampleInfo.HdMessage = AcquedParameters.HdMessage;
            return true;
        }


        internal static UInt32 GetLevelByVoltage(ChannelId source, double voltage)
        {
            if (source.IsAnalog()&&Hd.UIMessage?.Analog!=null&&Hd.UIMessage.Analog.Length>(Int32)source)
            {
                var analog = Hd.UIMessage.Analog[(Int32)source];
                return (UInt32)(Constants.SAMPS_PER_YDIV*(voltage+analog.Position)/analog.Scale+Constants.MAX_ADC_RES/2);
            }
            return 0;
        }
        public virtual Boolean TryTakeWave(ChannelId chnlId, List<ReadInfo> readInfoList, [NotNullWhen(true)] out Dictionary<String, (WfmSampleInfo wfmSampleInfo, List<UInt16> wfmData)> wfmPkgs, CancellationToken? softResetToken = null)
        {
            wfmPkgs = new();

            foreach (var readinfo in readInfoList)
            {
                Boolean ret = TryTakeWave(chnlId, readinfo, out List<UInt16> buff, out WfmSampleInfo wfmSampleInfo, softResetToken);
                if (ret)
                {
                    wfmPkgs.TryAdd(readinfo.Mark, (wfmSampleInfo, buff));
                }
            }
            if (wfmPkgs.Count() == 0)
            {
                return false;
            }

            return true;
        }

        public virtual Double TakeSamplingRate()
        {
            return ConstDefine.Ratio_f / AcquingParameters.PerDataByfs_AtDdr;
        }
        public Boolean TryTakeSourceWave(ChannelId channel, ReadInfo readInfo, out List<UInt16> wavedata, CancellationToken? softResetToken = null)
        {
            throw new NotImplementedException();
        }

        public Int32 TrySaveSourceWave(ChannelId channel, ReadInfo readInfo, FileStream? fStream, String format, CancellationToken? softResetToken)
        {
            //使能软暂停
            HdIO.WriteReg(ProcBdReg.W.DataPath_SoftStopPro, 1U);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_SoftStopAcq, 1U);

            var ans = _LongStorageSaveSourceData?.Invoke(channel, readInfo, fStream, format, softResetToken);

            //取消软暂停
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_SoftStopAcq, 0U);
            HdIO.WriteReg(ProcBdReg.W.DataPath_SoftStopPro, 0U);

            return ans ?? 0;
        }

        public virtual bool TryTakeDdrSourceWave(Int32 channel, double startTimeBySecond, double totalTime, [NotNullWhen(true)] out List<ushort> waveData, [NotNullWhen(true)] out WfmSampleInfo wfmSampleInfo)
        {
            //if (LongStorage != null)
            //    return LongStorage!.TryTakeDdrSourceWave(channel, startTimeBySecond, totalTime, out waveData, out wfmSampleInfo);
            //else
            {
                waveData = new List<ushort>();
                wfmSampleInfo = new WfmSampleInfo() { SampleIntervalByus = 0.5 };
                return false;
            }
        }
        public bool TryTakeSegmentWave(ChannelId channel, ReadInfo readInfo, Int32 segmentStartIndex, Int32 segmentCnt, out UInt16[,] waveData, out WfmSampleInfo wfmSampleInfo, out Double SecondByps, CancellationToken? softResetToken, Boolean b4SourceData = false)
        {
            List<ushort[]> readBackData = new List<ushort[]>();
            wfmSampleInfo = new WfmSampleInfo();
            wfmSampleInfo.SampleIntervalByus = AcquedParameters.PerDataByfs_AtDdr * 1.0 / 1E9;
            SecondByps = 0.0;
            var res = _LongStorageTakeSegmentWave?.Invoke(channel, readInfo, segmentStartIndex, segmentCnt, readBackData, wfmSampleInfo, softResetToken, b4SourceData);
            if (res ?? false)
            {
                waveData = new ushort[readBackData.Count, readBackData[0].Length];
                for (int i = 0; i < readBackData.Count; i++)
                {
                    var buffer = readBackData[i].ToArray();
                    Unsafe.CopyBlock(ref Unsafe.As<UInt16, Byte>(ref waveData[i, 0]), ref Unsafe.As<UInt16, Byte>(ref buffer[0]), (UInt32)(Unsafe.SizeOf<UInt16>() * buffer.Length));
                }
                return true;
            }
            else
            {
                waveData = new ushort[readBackData.Count, readInfo.pkgInfo.DotsCount];
                return false;
            }
        }

        public Int32 TryTakeCollectedSegmentCnt()
        {
            return _LongStorageReadCollectedFrameCnt?.Invoke() ?? 0;
            //return 0;// LongStorage?.ReadCollectedFrameCnt() ?? 0;
        }
        #endregion
        public virtual void ConfigLed(ErrorType type)
        {
            //throw new NotImplementedException();
        }

        public virtual void CloseAllLed()
        { }

        protected virtual bool AcqAnalogChannelSimulateWaveform()
        {
            //AcqedDataPool.AnalogChData.AllChannelData.Clear();
            //ushort[] list = { 2048, 1648, 1248, 2448 };
            //for (int channelID = 0; channelID < ChannelIdExt.AnaChnlNum; channelID++)
            //{
            //    AcqedDataPool.AnalogChData.AllChannelData.Add(new List<ushort>());
            //    for (int i = 0; i < 10000; i++)
            //        AcqedDataPool.AnalogChData.AllChannelData[channelID].Add(list[channelID]);
            //}
            Monitor.Enter(AcqedDataPool.UpdateDataLock);
            foreach (var channelData in AcqedDataPool.AnalogChData.AllChannelData)
            {
                channelData.Clear();
            }

            int Length = (int)Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquedParameters.HardwareStorageWaveDotsCnt;
            double SampIntByns = Hd.UIMessage?.Timebase?.TmbScale * Constants.VIS_XDIVS_NUM * 1000 / Constants.CHNL_DATA_NUM ?? 0.5;
            //var cycles = Length * (SampIntByns * 1E-9) * (Constants.AWG_SIN_FRQ_DEF * 1E-6);
            double cycles = 10;
            double NoiseByPercent = 0.05;
            ArbWfmType[] allChannelArbWfmType;
            if (!Hd.BPowerOff)
            {
                allChannelArbWfmType = new ArbWfmType[] { ArbWfmType.Sinc, ArbWfmType.Square, ArbWfmType.Ramp, ArbWfmType.DC, ArbWfmType.Sinc, ArbWfmType.Square, ArbWfmType.Ramp, ArbWfmType.Haversine };
            }
            else
            {
                allChannelArbWfmType = new ArbWfmType[] { ArbWfmType.DC, ArbWfmType.DC, ArbWfmType.DC, ArbWfmType.DC, ArbWfmType.DC, ArbWfmType.DC, ArbWfmType.DC, ArbWfmType.DC };
            }

            int channelCount = ChannelIdExt.AnaChnlNum;
            if (Hd.CurrProduct.ProductType == ProductType.B21_DBI16G || Hd.CurrProduct.ProductType == ProductType.B21_DBI20G)
            {
                channelCount = 4;//按子带数来处理，而不是物理通道数
            }

            for (int channelID = 0; channelID < channelCount; channelID++)
            {
                double anaChannelPosition = 0;// Constants.IDX_PER_YDIV * 5;// Hd.CurrHdMessage?.Analog?[channelID].PositionIndex ?? 0;
                double amplitude = 1.0;// (Hd.CurrHdMessage?.Analog?[channelID].Scale ?? 0) * 6;
                ArbWfmType arbWfmType = allChannelArbWfmType[channelID];
                IEnumerable<Double> y = arbWfmType switch
                {
                    ArbWfmType.Pulse or ArbWfmType.Square => Generator.Rectangular(anaChannelPosition, amplitude, cycles / Length, Length, 0.05, NoiseByPercent, 0.1),
                    ArbWfmType.DC => Generator.DirectCurrent(anaChannelPosition, amplitude, Length, 0.05),
                    ArbWfmType.Haversine => Generator.Haversine(anaChannelPosition, amplitude, cycles / Length, Length, NoiseByPercent, 0.05),
                    _ => Generator.Sine(anaChannelPosition, amplitude, cycles / Length, Length, 0.02, 0.0),
                };
                y = y.Select(o => o * 1E3);
                double pos0 = 0;
                if (channelID < ChannelIdExt.AnaChnlNum)//因为DBI的缘故，子带模式与物理通道模式不一样
                {
                    pos0 = (Hd.UIMessage?.Analog?[channelID].PositionIndex ?? 0) / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
                }
                else
                {
                    pos0 = (Hd.UIMessage?.Analog?[ChannelIdExt.AnaChnlNum - 1].PositionIndex ?? 0) / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
                }

                y = y.Select((o) => o /*(Hd.CurrHdMessage?.Analog?[channelID].Scale ?? 100)*/ / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + pos0);
                Double[] data = y.ToArray();// .ToRowVector();

                for (int i = 0; i < Length; i++)
                {
                    AcqedDataPool.AnalogChData.AllChannelData[channelID].Add((ushort)(data[i]));
                }
            }
            AbstractController_AnalogChannel.SoftwareBandwidthProcess();


            Monitor.Exit(AcqedDataPool.UpdateDataLock);
            return true;
        }
        #region Cali Tool 调用函数
        public virtual bool TakeAllChannelWaveform(out List<List<ushort>> waveData)
        {
            waveData = new List<List<ushort>>();
            Monitor.Enter(AcqedDataPool.UpdateDataLock);
            for (int channelIndex = 0; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                //由于Scpi数据传输的过程中，没有标识每个通道的数据量，当实际数据量出现不是Constants.CHNL_DATA_NUM数时，就要补齐或移除多余的
                //因为Tool端默认是Constants.CHNL_DATA_NUM 的数据长度，否则要报错
                List<ushort> oneChannelData = new List<ushort>();
                oneChannelData.AddRange(AcqedDataPool.AnalogChData.AllChannelData[channelIndex].ToArray());
                waveData.Add(oneChannelData);
            }
            Monitor.Exit(AcqedDataPool.UpdateDataLock);
            return true;
        }
        public virtual bool TakeAdcWaveform(out List<List<ushort>> waveData)
        {
            waveData = new List<List<ushort>>();
            int totalCoreCount = ChannelIdExt.AnaChnlNum * Constants.ADC_NUM;
            for (int coreIndex = 0; coreIndex < totalCoreCount; coreIndex++)
            {
                waveData.Add(new List<ushort>());
            }

            Monitor.Enter(AcqedDataPool.UpdateDataLock);
            int totalCoreIndex = 0;
            for (int channelIndex = 0; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            {
                for (int adcIndex = 0; adcIndex < Constants.ADC_NUM; adcIndex++)
                {
                    //for (int coreIndex = 0; coreIndex < Constants.ADC_CORE_NUM; coreIndex++)
                    {
                        for (int dotIndex = 0; dotIndex < Constants.CORE_DATA_NUM; dotIndex++)
                        {
                            ushort data = (adcIndex % Constants.ADC_NUM, 0) switch
                            {
                                (0, 0) => AcqedDataPool.AnalogChData.AllChannelData[channelIndex][dotIndex * 2],
                                //(0, 1) => AcqedDataPool.AnalogChData.AllChannelData[channelIndex][dotIndex * 4 + 2],
                                (1, 0) => AcqedDataPool.AnalogChData.AllChannelData[channelIndex][dotIndex * 2 + 1],
                                //(1, 1) => AcqedDataPool.AnalogChData.AllChannelData[channelIndex][dotIndex * 4 + 3],
                                _ => 0,
                            };
                            waveData[totalCoreIndex].Add(data);
                        }
                        totalCoreIndex++;
                    }
                }
            }
            Monitor.Exit(AcqedDataPool.UpdateDataLock);
            return true;
        }
        #endregion
        internal virtual bool IsNeedPostProcessByMatlab => false;
        internal virtual void AnalogChannelActiveChanged() { }
        private Dictionary<UInt64, UInt32> _ExtramTable = new Dictionary<UInt64, UInt32>()
        {
            // 抽取倍数   下发参数
            {1,             0 },
            {2,             1 },
            {4,             2 },
            {5,             3 },
            {8,             4 },
            {10,            5 },
            {20,            6 },
            {40,            7 },
        };

        /// <summary>
        /// 根据抽取模式和并行路数下发抽取参数
        /// </summary>
        /// <param name="extramNum">抽取倍数:最大是36位</param>
        /// <param name="parallelRoads">并行路数:40/80</param>
        /// <param name="extramMode"></param>
        internal void ConfigExtramNum(UInt64 extramNum, UInt32 parallelRoads, AnaChnlAcqMode extramMode)
        {
            if (_ExtramTable.ContainsKey(extramNum) && extramNum < parallelRoads)
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, _ExtramTable[extramNum]);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_L16, 0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_M16, 0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_H16, 0);
            }
            else
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_L16, (UInt32)(extramNum & 0xffff));
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_M16, (UInt32)((extramNum >> 16) & 0xffff));
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_H16, (UInt32)((extramNum >> 32) & 0xffff));
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, 8);
            }
        }
        struct ExtramPara
        {
            internal UInt32 Multiple_Pattern;
            internal UInt64 RemainderAdditional;
            internal UInt64 DecimationQuotient;
        }

        private ExtramPara GetExtramPara(UInt64 extramNum, UInt32 parallelRoads, AnaChnlAcqMode extramMode)
        {
            ExtramPara ectram = new ExtramPara();
            if (_ExtramTable.ContainsKey(extramNum) && extramNum < parallelRoads)
            {
                ectram.Multiple_Pattern = _ExtramTable[extramNum];
                ectram.RemainderAdditional = 0;
                ectram.DecimationQuotient = 0;
            }
            else
            {
                ectram.Multiple_Pattern = 8;
                switch (extramMode)
                {
                    case AnaChnlAcqMode.Normal:
                        ectram.DecimationQuotient = extramNum / parallelRoads;
                        ectram.RemainderAdditional = (extramNum / (parallelRoads / 4)) % 4;
                        break;
                    case AnaChnlAcqMode.Peak:
                        ectram.DecimationQuotient = extramNum * 2 / parallelRoads;
                        if (extramNum * 2 % parallelRoads == 0)
                        {
                            ectram.RemainderAdditional = 0x1 << 7;
                        }
                        else
                        {
                            ectram.RemainderAdditional = parallelRoads / (extramNum * 2 % parallelRoads);
                        }

                        break;
                    case AnaChnlAcqMode.HighRes:
                        ectram.DecimationQuotient = extramNum / parallelRoads;
                        if (extramNum % parallelRoads == 0)
                        {
                            ectram.RemainderAdditional = 0x1 << 7;
                        }
                        else
                        {
                            ectram.RemainderAdditional = parallelRoads / (extramNum % parallelRoads);
                        }

                        break;
                    default:
                        break;
                }
            }
            return ectram;
        }

        internal void ConfigExtramNum(UInt64 extramNum, UInt32 parallelRoads, AnaChnlAcqMode extramMode, AcqBdNo acqBd)
        {
            ExtramPara ectramPara = GetExtramPara(extramNum, parallelRoads, extramMode);

            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Decimation_DecimationMode, acqBd, ectramPara.Multiple_Pattern);

            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Decimation_Decimation_L16, acqBd, (UInt32)(ectramPara.DecimationQuotient & 0xffff));
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Decimation_Decimation_M16, acqBd, (UInt32)(ectramPara.DecimationQuotient >> 16 & 0xffff));
            Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.Decimation_Decimation_H16, acqBd, (UInt32)(ectramPara.DecimationQuotient >> 32 & 0xf));
        }

        internal virtual void ConfigLongStorage()
        {
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_MultiSegement_Enable, (UInt16)Hd.UIMessage!.Timebase!.SegmentActive);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_MultiSegement_Mode, Hd.UIMessage!.Timebase!.CallBack ? 1U : 0);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_StorageMode, AcquingParameters.bIsLongStorageMode ? 1 : 0U);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_RcdRstEnable, AcquingParameters.LS_SegmentCount > 1 ? 1u : 0);
        }
        internal virtual void FilterChannelAtSpecailBandwidth(ChannelId channelId)
        {
            //ScopeX.MathExt.Filter.
        }
        internal virtual void ConfigExtractProcessRoadParameters()
        {
            //要使FIFO 满，在确保ADC工作正常的情况下，需要正确配置的参数包括：
            //采集板
            //1、FifoCtrl_OutSpeed  串并转换CH_MODE_SamplingMode
            //2、CH_MODE_SamplingMode
            //3、Interpolate_Enable 、Interpolate_Ratio
            //4、Decimation_Hardware、Decimation_HighResolution、Decimation_Peak等整个采集板处理路径
            //5、FIFO深度FifoCtrl_FullProgDepth(可以在初始化时一次性设置)
            //处理板
            //FifoCtrl_ParallelFifoDepth、FifoCtrl_FullProgDepth(可以在初始化时一次性设置)
            //PCIE 板
            //FifoCtrl_ProgEmpty、FifoCtrl_FullProgDepth、FifoCtrl_ReadFromFIFO_Num(可以在初始化时一次性设置)
            //FifoCtrl_ChannelMode

            //modifyNote20220117   1、原来的不同模式采用不同的寄存器，现在呢？

            UInt32 acqMode = (Hd.UIMessage?.Timebase?.AcqMode ?? AnaChnlAcqMode.Normal) switch
            {
                AnaChnlAcqMode.Peak => 1,
                AnaChnlAcqMode.HighRes => 3,
                _ => 0,
            };

            UInt64 extractNum = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.ExtractNumFromAdc ?? 1U;
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_L16, (UInt32)(extractNum & 0xffff));
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_M16, (UInt32)((extractNum >> 16) & 0xffff));
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_H16, (UInt32)(extractNum >> 32) & 0xffff);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, acqMode & 0xffff);

            ConfigExtramNum(extractNum, Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AcqAdcMergeRoadCount, (Hd.UIMessage?.Timebase?.AcqMode ?? AnaChnlAcqMode.Normal));


            AnaChnlStorageMode anaChnlAcqLength = (Hd.UIMessage?.Timebase?.AcqLength ?? AnaChnlStorageMode.Normal);

            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_StorageMode, anaChnlAcqLength == AnaChnlStorageMode.Long ? 1U : 0U);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Interpolate_Mode, (UInt32)(1 - (int)(Hd.UIMessage?.Timebase?.InterpolateType ?? AnaChnlItplType.Sinx)));
        }

        internal virtual Int32 Dpx_AcqFifoDepth()
        {
            Int64 currPerXDivByps = (Int64)((Hd.UIMessage?.Timebase?.TmbScale ?? 1) * 1_000_000);//TmbScale 以us为单位,*1_000_000,us==>ps
            return (Int32)((currPerXDivByps * 1000 * Constants.VIS_XDIVS_NUM / Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr + 200) / 40);
        }

        #region AFC(幅频特性) 系数
        private Dictionary<ChannelId, AnaChnlScaleIndex> lastChannelYScale = new Dictionary<ChannelId, AnaChnlScaleIndex>()
        {
            [ChannelId.C1] = AnaChnlScaleIndex.Lv100,
            [ChannelId.C2] = AnaChnlScaleIndex.Lv100,
            [ChannelId.C3] = AnaChnlScaleIndex.Lv100,
            [ChannelId.C4] = AnaChnlScaleIndex.Lv100,
            [ChannelId.C5] = AnaChnlScaleIndex.Lv100,
            [ChannelId.C6] = AnaChnlScaleIndex.Lv100,
            [ChannelId.C7] = AnaChnlScaleIndex.Lv100,
            [ChannelId.C8] = AnaChnlScaleIndex.Lv100,
        };
        protected virtual void CheckResendAfc()
        {
            CoefficientsTableType coefficientsTableType = CoefficientsTableType.Coefficients8;
            bool bFound = false;
            foreach (var v in Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings)
            {
                if (v.Value.CoeffType == CommCoefficientsTableType.AcqBd_AFC)
                {
                    coefficientsTableType = v.Key;
                    bFound = true;
                    break;
                }
            }
            if (!bFound)
            {
                return;
            }

            bFound = false;
            for (int channelId = (int)ChannelId.C1; channelId < ChannelIdExt.AnaChnlNum; channelId++)
            {
                if (lastChannelYScale[(ChannelId)channelId] != (AnaChnlScaleIndex)Hd.UIMessage!.Analog![channelId]!.ScaleIndex)
                {
                    lastChannelYScale[(ChannelId)channelId] = (AnaChnlScaleIndex)Hd.UIMessage!.Analog![channelId]!.ScaleIndex;
                    bFound = true;
                }
            }
            if (bFound)
            {
                CaliDataManager.DataChangedCoefficientsTableType.Add(coefficientsTableType);
                Hd.LocalCommands |= (long)HdCmd.CaliDataChanged;
            }
        }
        internal virtual bool Afc_Sender_ByRegisterMode(CoefficientsTableType coefficientsTableType, int[] dataArray, AcqBdNo acqBdNo, ChannelId channelId)
        {
            //comment for JiHe_MSO7000X 
            //int partALength = Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings[coefficientsTableType].LengthOfPartA;

            //int dataCount = dataArray.Length;
            //for (int i = 0; i < dataCount; i++)
            //{
            //    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Fir_FactorTableWriteEnable, acqBdNo, 0);
            //    if (i < partALength) //10G系数  
            //        Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Fir_FactorTableWriteAddr, acqBdNo, (UInt32)i);
            //    else       //20G系数(修改系数长度后需要修改此处)
            //        Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Fir_FactorTableWriteAddr, acqBdNo, (UInt32)(i + 0b00100000000 - partALength));//(适用200阶)下发地址11位，通过高位为1下发的则是20G的系数
            //    Int32 data = dataArray[i];
            //    //低16位
            //    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Fir_FactorTableWriteData_L, acqBdNo, (UInt32)data & 0xffff);
            //    //高位
            //    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Fir_FactorTableWriteData_H, acqBdNo, (UInt32)(data >> 16) & 0xff);
            //    HdIO.DelayByUs(10);
            //    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Fir_FactorTableWriteEnable, acqBdNo, 1);

            //}
            //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Fir_FactorTableWriteEnable, acqBdNo, 0);

            //Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Fir_Enable, acqBdNo, Hd.CurrDebugVarints.bEnable_AcqBd_Afc ? 1U : 0);
            return true;
        }
        internal virtual bool Afc_Sender_ByDMAMode(CoefficientsTableType coefficientsTableType, int[] dataArray, AcqBdNo acqBdNo, ChannelId channelId)
        {
            return false;
        }
        /// <summary>
        /// Key =  $"{acqBdNo}_{((ChannelId)channelId)}",Value=crcCode
        /// </summary>
        protected Dictionary<string, int> AfcSendHistory = new Dictionary<string, int>();

        internal virtual void ClearSendHistory()
        {
            lastChannelYScale[ChannelId.C1] = AnaChnlScaleIndex.Lv100;
            lastChannelYScale[ChannelId.C2] = AnaChnlScaleIndex.Lv100;
            lastChannelYScale[ChannelId.C3] = AnaChnlScaleIndex.Lv100;
            lastChannelYScale[ChannelId.C4] = AnaChnlScaleIndex.Lv100;
            lastChannelYScale[ChannelId.C5] = AnaChnlScaleIndex.Lv100;
            lastChannelYScale[ChannelId.C6] = AnaChnlScaleIndex.Lv100;
            lastChannelYScale[ChannelId.C7] = AnaChnlScaleIndex.Lv100;
            lastChannelYScale[ChannelId.C8] = AnaChnlScaleIndex.Lv100;
            AfcSendHistory.Clear();
        }
        internal virtual void SendCoefficients_Afc(CoefficientsTableType coefficientsTableType, bool bForce)
        {
            return;
        }
        #endregion
        internal virtual bool AutoCaliAtInit(HdMessage? hdMessage) => false;
        internal String SpecialConfigForAutoCaliAtInit = String.Empty;

        internal virtual Boolean IsInterpolation_LS(Int32 channelId)
        {
            return false;
        }
        #region 模拟通道幅度增益温度补偿
        protected DateTime LastPhyAnalogChAmplitudeTemperaturesCompensationCalced_Datetime = DateTime.Now;
        Dictionary<ChannelId, string> PhyAnalogChAmplitudeTemperaturesCompensationCoefficientFile = new Dictionary<ChannelId, string>()
        {
            {ChannelId.C1,$@".\CaliData\CoeFiles\AnalogChannelTemperatureCompensation_C1.txt" },
            {ChannelId.C2,$@".\CaliData\CoeFiles\AnalogChannelTemperatureCompensation_C2.txt" },
            {ChannelId.C3,$@".\CaliData\CoeFiles\AnalogChannelTemperatureCompensation_C3.txt" },
            {ChannelId.C4,$@".\CaliData\CoeFiles\AnalogChannelTemperatureCompensation_C4.txt" },
            {ChannelId.C5,$@".\CaliData\CoeFiles\AnalogChannelTemperatureCompensation_C5.txt" },
            {ChannelId.C6,$@".\CaliData\CoeFiles\AnalogChannelTemperatureCompensation_C6.txt" },
        };
        Dictionary<ChannelId, Dictionary<double, double>> allChannelAmplitudeTemperaturesCompensationCoefficient = new Dictionary<ChannelId, Dictionary<double, double>>();
        protected List<double> LastPhyAnalogChAmplitudeTemperaturesCompensationCoefficient = new List<double>() { 1, 1, 1, 1, 1, 1, 1, 1 };
        protected List<double> PhyAnalogChAmplitudeBaselineTemperature = new List<double>() { 40, 40, 40, 40, 40, 40, 40, 40 };//校准基准温度
        internal virtual void InitPhyAnalogChAmplitudeTemperaturesCompensationCoefficient()
        {
            allChannelAmplitudeTemperaturesCompensationCoefficient.Clear();
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
            for (ChannelId channelId = ChannelId.C1; channelId <= ChannelId.C8; channelId++)
            {
                if (PhyAnalogChAmplitudeTemperaturesCompensationCoefficientFile.ContainsKey(channelId))
                {
                    var result = Misc.ReadAmplitudeTemperaturesCompensationCoefficientFile(PhyAnalogChAmplitudeTemperaturesCompensationCoefficientFile[channelId], out double baselineTemperature);
                    if (result.Count > 0)
                    {
                        if (allChannelAmplitudeTemperaturesCompensationCoefficient.ContainsKey(channelId))
                        {
                            allChannelAmplitudeTemperaturesCompensationCoefficient[channelId] = result;
                        }
                        else
                        {
                            allChannelAmplitudeTemperaturesCompensationCoefficient.Add(channelId, result);
                        }

                        PhyAnalogChAmplitudeBaselineTemperature[(int)channelId] = baselineTemperature;
                    }
                }
            }
        }
        internal virtual bool GetPhyAnalogChAmplitudeTemperaturesCompensationCoefficient(out List<double> Coefficient)
        {
            Coefficient = LastPhyAnalogChAmplitudeTemperaturesCompensationCoefficient;
            //if ((DateTime.Now - LastPhyAnalogChAmplitudeTemperaturesCompensationCalced_Datetime).TotalMilliseconds < Acquisition.PhyAnalogChAmplitudeTemperaturesCompensationIntervalByMs)
            //{
            //    return false;
            //}

            double currTemperature = SystemMonitor.Default.AnalogChannelTemperatures[0];//只有一个通道板
            for (ChannelId channelId = ChannelId.C1; channelId <= ChannelIdExt.MaxAChId; channelId++)
            {
                ChannelId key = channelId;
                if (allChannelAmplitudeTemperaturesCompensationCoefficient.ContainsKey(key))
                {
                    double result = 1;
                    KeyValuePair<double, double> first;
                    try
                    {
                        first = allChannelAmplitudeTemperaturesCompensationCoefficient[key].First((o) => o.Key <= currTemperature);
                    }
                    catch
                    {
                        first = (allChannelAmplitudeTemperaturesCompensationCoefficient[key].First());
                        result = first.Value;
                        if (allChannelAmplitudeTemperaturesCompensationCoefficient[key].ContainsKey(PhyAnalogChAmplitudeBaselineTemperature[(int)key]))
                        {
                            result = result / allChannelAmplitudeTemperaturesCompensationCoefficient[key][PhyAnalogChAmplitudeBaselineTemperature[(int)key]];
                        }

                        LastPhyAnalogChAmplitudeTemperaturesCompensationCoefficient[(int)channelId] = result;
                        return true;
                    }
                    KeyValuePair<double, double> last;
                    try
                    {
                        last = allChannelAmplitudeTemperaturesCompensationCoefficient[key].First((o) => o.Key >= currTemperature);
                    }
                    catch
                    {
                        last = allChannelAmplitudeTemperaturesCompensationCoefficient[key].Last();
                        result = last.Value;
                        if (allChannelAmplitudeTemperaturesCompensationCoefficient[key].ContainsKey(PhyAnalogChAmplitudeBaselineTemperature[(int)key]))
                        {
                            result = result / allChannelAmplitudeTemperaturesCompensationCoefficient[key][PhyAnalogChAmplitudeBaselineTemperature[(int)key]];
                        }

                        LastPhyAnalogChAmplitudeTemperaturesCompensationCoefficient[(int)channelId] = result;
                        return true; ;
                    }

                    if (Math.Abs(first.Key - currTemperature) < 0.01)//==
                    {
                        result = first.Value;
                    }
                    else if (Math.Abs(last.Key - currTemperature) < 0.01)//==
                    {
                        result = last.Value;
                    }
                    else if (first.Key != last.Key)
                    {
                        result = first.Value + (last.Value - first.Value) * (currTemperature - first.Key) / (last.Key - first.Key);//局部线性插值
                    }

                    if (allChannelAmplitudeTemperaturesCompensationCoefficient[channelId].ContainsKey(PhyAnalogChAmplitudeBaselineTemperature[(int)channelId]))
                    {
                        result = result / allChannelAmplitudeTemperaturesCompensationCoefficient[channelId][PhyAnalogChAmplitudeBaselineTemperature[(int)channelId]];
                    }

                    LastPhyAnalogChAmplitudeTemperaturesCompensationCoefficient[(int)channelId] = result;
                }
            }
            LastPhyAnalogChAmplitudeTemperaturesCompensationCalced_Datetime = DateTime.Now;
            return true;
        }
        #endregion

        #region Dpx
        internal virtual DpxAcqParameters CreateDpxAcqParameters()
        {
            return new DpxAcqParameters()
            {
                //bSerialDpx = true
            };
        }
        #endregion

        private Dictionary<string, ReadParams> ExistsReadParams = new Dictionary<string, ReadParams>();
        internal void ExistsReadParamsEnsureCapacity(Boolean bForceClear)
        {
            if (ExistsReadParams.Count > 100 || bForceClear)
            {
                ExistsReadParams.Clear();
            }
        }
        public record UpoConfigParam(uint Level1ExtractNum, uint InterpolateNum, uint Interpolate_ExctractNum, uint UPO_ExtractNum);
        private Dictionary<ulong/*原始数据 数*1000*/, (uint Level1ExtractNum, uint InterpolateNum, double Interpolate_ExctractNum, uint UPO_ExtractNum)> UpoConfigParams = new Dictionary<ulong, (uint Level1ExtractNum, uint InterpolateNum, double Interpolate_ExctractNum, uint UPO_ExtractNum)>()
        {
            //          Level1ExtractNum,InterpolateNum,Interpolate_ExctractNum,UPO_ExtractNum
            [10_000U] = (1U, 100U, 1, 1U),
            [12_500U] = (1U, 100U, 1, 1U),
            [20_000U] = (1U, 50U, 1, 1U),
            [25_000U] = (1U, 50U, 1, 1U),
            [40_000U] = (1U, 50U, 1, 2U),
            [50_000U] = (1U, 20U, 1, 1U),
            [625_00U] = (1U, 20U, 1, 1U),
            [100_000U] = (1U, 10U, 1, 1U),
            [125_000U] = (1U, 10U, 1, 1U),
            [200_000U] = (1U, 5U, 1, 1U),
            [400_000U] = (1U, 5U, 1, 2U),
            [250_000U] = (1U, 4U, 1, 1U),
            [500_000U] = (1U, 2U, 1, 1U),
            [625_000U] = (1U, 2U, 1, 1U),
            [1000_000U] = (1U, 1U, 1, 1U),
            [1250_000U] = (1U, 1U, 1, 1U),
            [2000_000U] = (1U, 1U, 1, 2U),
            [4000_000U] = (1U, 1U, 1, 4U),
            [2500_000U] = (1U, 1U, 1, 2U),
            [4000_000U] = (1U, 1U, 1, 4U),
            [5000_000U] = (1U, 1U, 1, 4U),
            [6250_000U] = (1U, 1U, 1, 5U),
            [10000_000U] = (1U, 1U, 1, 10U),
            [12500_000U] = (1U, 1U, 1, 10U),
            [20000_000U] = (1U, 1U, 1, 20U),
            [25000_000U] = (1U, 1U, 1, 20U),
            [40000_000U] = (1U, 1U, 1, 40U),
            [50000_000U] = (1U, 1U, 1, 40U),
            [62500_000U] = (1U, 1U, 1, 50U),
        };
        private ReadParams CalcDdrReadParams_UPO(DdrData4What data4What, AcquireAttribute acquireAttribute, Double uiStartTimeByUs, Double uiSumTimeByUs, UInt32 uiTotalDots, Int64 writedTimestamp)
        {
            Boolean isscan = acquireAttribute.HdMessage?.Timebase?.IsScan ?? false;
            ReadParams readparams = new ReadParams();
            readparams.WritedTimestamp = writedTimestamp;

            Double ddrwritedsampleintervalbyus = acquireAttribute.PerDataByfs_AtDdr / uS2fs;
            Double ddrexiststotaltimebyus = acquireAttribute.HardwareStorageWaveDotsCnt * acquireAttribute.PerDataByfs_AtDdr / uS2fs;
            Double ddrtheorystarttimebyus = acquireAttribute.SettingTrigPositionByfs / uS2fs - ddrexiststotaltimebyus / 2;
            Double ddracqeddelaytimebyus = ddrtheorystarttimebyus > 0 ? ddrtheorystarttimebyus : 0;
            ddrtheorystarttimebyus -= ddracqeddelaytimebyus;
            Double ddrtheoryendtimebyus = ddrexiststotaltimebyus + ddrtheorystarttimebyus;

            #region Scan暂停态时DDR中数据的时间信息，以数据最后一个点为0时刻

            Double scandisplaytimebyus = 0D;
            Double scannotdisplaytimebyus = 0D;
            Double scanstarttimebyus = 0D;
            Double scanendtimebyus = 0D;
            Double offsettimebyusscantonomal = 0D;//触发时刻到DDR最右侧(0时刻)的偏移

            if (isscan)
            {
                scandisplaytimebyus = Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay * acquireAttribute.PerDataByfs_AtDdr / uS2fs;
                scannotdisplaytimebyus = Acquisition.ScanPerChannelInDdrDotCount_NotDisplay * acquireAttribute.PerDataByfs_AtDdr / uS2fs;
                scanstarttimebyus = 0 - (scandisplaytimebyus + scannotdisplaytimebyus);
                scanendtimebyus = scandisplaytimebyus + scanstarttimebyus;
                offsettimebyusscantonomal = acquireAttribute.SettingTrigPositionByfs / uS2fs + (acquireAttribute.HdMessage?.Timebase?.TmbScale ?? 1.0) * Constants.VIS_XDIVS_NUM / 2 - 0;
                //统一0时刻： 将Scan的时间信息平移至以触发时刻为0时刻，并减去无效数据（未显示的时间）
                ddrtheorystarttimebyus = scanstarttimebyus + offsettimebyusscantonomal + scannotdisplaytimebyus;
                ddrtheoryendtimebyus = scanendtimebyus + offsettimebyusscantonomal + scannotdisplaytimebyus;
            }

            #endregion

            #region UI希望读取的数据时间信息

            Double uiuponeedsampleintervalbyus = uiSumTimeByUs / Constants.UPO_WIDTH;
            Double uidsoneedsampleintervalbyus = uiSumTimeByUs / uiTotalDots;

            Double uineedstarttimebyus = uiStartTimeByUs - uiSumTimeByUs / 2 - ddracqeddelaytimebyus;
            Double uineedendtimebyus = uiSumTimeByUs + uineedstarttimebyus;

            #endregion

            #region 实际读DDR数据的时间信息
            //根据时间得到界面上需要时间的


            //相对于触发时刻的读起始时间
            Double ddrrealstarttimebyus = uineedstarttimebyus < ddrtheorystarttimebyus ? ddrtheorystarttimebyus : uineedstarttimebyus;
            Double ddrrealendtimebyus = uineedendtimebyus > ddrtheoryendtimebyus ? ddrtheoryendtimebyus : uineedendtimebyus;
            ddrrealstarttimebyus = ddrrealstarttimebyus > ddrtheoryendtimebyus ? ddrtheoryendtimebyus : ddrrealstarttimebyus;
            ddrrealendtimebyus = ddrrealendtimebyus < ddrtheorystarttimebyus ? ddrtheorystarttimebyus : ddrrealendtimebyus;

            //相对于DDR最右端为0时刻的读起始时间
            Double scanneedstarttimebyus = (ddrrealstarttimebyus - scannotdisplaytimebyus) - offsettimebyusscantonomal;

            #endregion
            #region 计算各级抽点数
            //计算DSO的后抽取
            Double totalextractnum = uidsoneedsampleintervalbyus / ddrwritedsampleintervalbyus;
            readparams.TotalExtractNum = totalextractnum < 1 ? 1U : (uint)totalextractnum;

            UInt64 theroysourcedots = (UInt64)(uiSumTimeByUs * uS2fs * 1000 / acquireAttribute.PerDataByfs_AtDdr / readparams.TotalExtractNum);//进入UPO模块的点数
            var validInterpolateAndExtract = PlatFormManager.CurrPlatForm.CalcUpoInterpolateAndExtract(theroysourcedots, new List<double>() { 1000D, 1250D });
            readparams.UPO_Level1ExtractNum = 1;
            readparams.UPO_Level2ExtractNum = validInterpolateAndExtract.UPO_ExtractNum;
            readparams.Interpolate_Num_Double = validInterpolateAndExtract.InterpolateNum;
            readparams.UPO_ResultColoumnNum = (UInt32)(theroysourcedots * readparams.Interpolate_Num_Double
                / readparams.UPO_Level1ExtractNum / readparams.UPO_Level2ExtractNum);
            //readparams.Interpolate_DiscardDotNum = (Int32)((tempDdrReadStartDotPosition - readparams.DdrReadStartDotPosition) * readparams.Interpolate_Num_Double);

            readparams.bInterpolateNumGT100 = PlatFormManager.CurrPlatForm.IsInterpolateNumGT100(readparams.Interpolate_Num_Double);
            #endregion

            /***********************************************计算读取的点数*******************************************/
            Int32 returndotcount = (Int32)((ddrrealendtimebyus - ddrrealstarttimebyus) / readparams.TotalExtractNum / ddrwritedsampleintervalbyus);

            /***********************************************计算DDR读起始位置****************************************/
            Double tmddrreaddotstart = (isscan ? scanneedstarttimebyus : ddrrealstarttimebyus) / ddrwritedsampleintervalbyus;

            Int32 tmpreturndotcount = returndotcount;

            if (readparams.Interpolate_Num_Double > 1)
            {
                //去掉插值后的小数部分
                tmddrreaddotstart = ((Int32)(tmddrreaddotstart * readparams.Interpolate_Num_Double)) / readparams.Interpolate_Num_Double;
                readparams.DdrReadStartDotPosition = Math.Floor(tmddrreaddotstart);
                readparams.Interpolate_DiscardDotNum = (Int32)((tmddrreaddotstart - readparams.DdrReadStartDotPosition) * readparams.Interpolate_Num_Double);

            }
            else
            {
                returndotcount = tmpreturndotcount;
                readparams.Interpolate_Num_Double = 1;
                readparams.Interpolate_DiscardDotNum = 0;
                readparams.DdrReadStartDotPosition = tmddrreaddotstart;
            }

            returndotcount = (Int32)((ddrrealendtimebyus - ddrrealstarttimebyus) * readparams.Interpolate_Num_Double / (readparams.TotalExtractNum * acquireAttribute.PerDataByfs_AtDdr / uS2fs));

            Int32 dbi_interp = 1;
            if (Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation)
            {
                if (Hd.UIMessage!.Timebase!.TmbScale < 0.02)
                {
                    returndotcount *= 4;
                    dbi_interp = 4;
                }
            }

                readparams.PerChannelRecvDotsCount = returndotcount;
            //comment by zhaoyong,right?
            //                         (       原坐标系下，触发点左边的时间[屏幕中心之左，为负数]                                )
            //ddr_RealStartTimeByUs:新坐标系下，触发点坐标的时间[屏幕中心之左，为负数],并进行了左右交处理的
            //结果，旧-新坐标系下触发点左边时间之差。
            readparams.StartTimeByus = (acquireAttribute.HdMessage?.Timebase?.TmbPosition ?? 0) - (acquireAttribute.HdMessage?.Timebase?.TmbScale ?? 0) * Constants.VIS_XDIVS_NUM / 2 - ddrrealstarttimebyus - ddracqeddelaytimebyus;
            readparams.SampleIntervalByUs = acquireAttribute.PerDataByfs_AtDdr * readparams.TotalExtractNum / readparams.Interpolate_Num_Double/ dbi_interp / uS2fs;
            readparams.Data4What = data4What;

            //ExistsReadParams.Add(key, readParams);
            return readparams;
        }
        internal ReadParams CalcDdrReadParams(DdrData4What data4What, AcquireAttribute acquireAttribute, Double uiStartTimeByUs, Double uiSumTimeByUs, UInt32 uiTotalDots, Int64 writedTimestamp)
        {
            if (data4What == DdrData4What.Upo)
                return CalcDdrReadParams_UPO(data4What, acquireAttribute, uiStartTimeByUs, uiSumTimeByUs, uiTotalDots, writedTimestamp);
            Boolean isscan = acquireAttribute.HdMessage?.Timebase?.IsScan ?? false;
            #region Scan运行态
            if (isscan && (!(Hd.UIMessage?.bAcquireStopped ?? false)))
            {
                ReadParams readParams_scanRunning = new ReadParams();
                readParams_scanRunning.WritedTimestamp = writedTimestamp;
                readParams_scanRunning.DMADataPath = DMAReadDataTypes.ScanFifo;
                readParams_scanRunning.PerChannelRecvDotsCount = Acquisition.ScanRunningNewDataPerChannelExistsDotCount;
                readParams_scanRunning.SampleIntervalByUs = (acquireAttribute.PerDataByfs_AtDdr * acquireAttribute.Scan2ExtractNum_Total / uS2fs);
                readParams_scanRunning.StartTimeByus = uiStartTimeByUs;
                readParams_scanRunning.Data4What = data4What;
                for (Int32 channelIndex = 0; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
                {
                    readParams_scanRunning.Ddr_PosGainFine[channelIndex] = 1.0;
                    readParams_scanRunning.Ddr_PosOffset[channelIndex] = (Int16)(Hd.UIMessage!.Analog![channelIndex].PositionIndex * Constants.SAMPS_PER_YDIV / Constants.IDX_PER_YDIV + Constants.MAX_ADC_RES / 2);
                    readParams_scanRunning.Ddr_PosOffsetDelta[channelIndex] = 0;
                    readParams_scanRunning.Ddr_PosInvert[channelIndex] = false;
                }
                return readParams_scanRunning;
            }

            #endregion

            #region 非Scan时DDR中数据的时间信息，以触发时刻为0时刻
            String key = $"{acquireAttribute.AdcInterleaveMode}_{acquireAttribute.PerDataByfs_AtDdr}_{acquireAttribute.HardwareStorageWaveDotsCnt}_{acquireAttribute.SettingTrigPositionByfs}_{acquireAttribute.HdMessage?.Timebase?.TmbScale}_{Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay}_{Acquisition.ScanRunningNewDataPerChannelExistsDotCount}_{uiStartTimeByUs}_{uiSumTimeByUs}_{uiTotalDots}";
            if (ExistsReadParams.ContainsKey(key))
            {
                ReadParams readparamsexists = ExistsReadParams[key].Clone();
                readparamsexists.Data4What = data4What;
                readparamsexists.WritedTimestamp = writedTimestamp;
                return readparamsexists;
            }

            ReadParams readparams = new ReadParams();
            readparams.WritedTimestamp = writedTimestamp;
            //cij_NEW_0528
            double ddrwritedsampleintervalbyus = acquireAttribute.PerDataByfs_AtDdr / uS2fs;
            double ddrexiststotaltimebyus = acquireAttribute.HardwareStorageWaveDotsCnt * acquireAttribute.PerDataByfs_AtDdr / uS2fs;
            double ddrtheorystarttimebyus = acquireAttribute.SettingTrigPositionByfs / uS2fs - ddrexiststotaltimebyus / 2;
            double ddr_AcqedDelayTimeByUs = ddrtheorystarttimebyus > 0 ? ddrtheorystarttimebyus : 0;
            ddrtheorystarttimebyus -= ddr_AcqedDelayTimeByUs;
            double ddr_theoryEndTimeByUs = ddrexiststotaltimebyus + ddrtheorystarttimebyus;

            #endregion

            #region Scan暂停态时DDR中数据的时间信息，以数据最后一个点为0时刻

            var scan_DisplayTimeByUs = 0D;
            var scan_NotDisplayTimeByUs = 0D;
            var scan_StartTimeByUs = 0D;
            var scan_EndTimeByUs = 0D;
            var offsetTimeByUsScanToNomal = 0D;//触发时刻到DDR最右侧(0时刻)的偏移

            if (isscan)
            {
                scan_DisplayTimeByUs = Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay * acquireAttribute.PerDataByfs_AtDdr / uS2fs;
                scan_NotDisplayTimeByUs = Acquisition.ScanPerChannelInDdrDotCount_NotDisplay * acquireAttribute.PerDataByfs_AtDdr / uS2fs;
                scan_StartTimeByUs = 0 - (scan_DisplayTimeByUs + scan_NotDisplayTimeByUs);
                scan_EndTimeByUs = scan_DisplayTimeByUs + scan_StartTimeByUs;
                offsetTimeByUsScanToNomal = acquireAttribute.SettingTrigPositionByfs / uS2fs + (acquireAttribute.HdMessage?.Timebase?.TmbScale ?? 1.0) * Constants.VIS_XDIVS_NUM / 2 - 0;
                //统一0时刻： 将Scan的时间信息平移至以触发时刻为0时刻，并减去无效数据（未显示的时间）
                ddrtheorystarttimebyus = scan_StartTimeByUs + offsetTimeByUsScanToNomal + scan_NotDisplayTimeByUs;
                ddr_theoryEndTimeByUs = scan_EndTimeByUs + offsetTimeByUsScanToNomal + scan_NotDisplayTimeByUs;
            }


            #endregion

            #region UI希望读取的数据时间信息

            double ui_NeedSampleIntervalByUs = uiSumTimeByUs / (uiTotalDots);
            double ui_NeedStartTimeByUs = uiStartTimeByUs - uiSumTimeByUs / 2 - ddr_AcqedDelayTimeByUs;
            double ui_NeedEndTimeByUs = uiSumTimeByUs + ui_NeedStartTimeByUs;

            #endregion

            #region 实际读DDR数据的时间信息


            //相对于触发时刻的读起始时间
            double ddr_RealStartTimeByUs = ui_NeedStartTimeByUs < ddrtheorystarttimebyus ? ddrtheorystarttimebyus : ui_NeedStartTimeByUs;
            double ddr_RealEndTimeByUs = ui_NeedEndTimeByUs > ddr_theoryEndTimeByUs ? ddr_theoryEndTimeByUs : ui_NeedEndTimeByUs;
            ddr_RealStartTimeByUs = ddr_RealStartTimeByUs > ddr_theoryEndTimeByUs ? ddr_theoryEndTimeByUs : ddr_RealStartTimeByUs;
            ddr_RealEndTimeByUs = ddr_RealEndTimeByUs < ddrtheorystarttimebyus ? ddrtheorystarttimebyus : ddr_RealEndTimeByUs;

            //相对于DDR最右端为0时刻的读起始时间
            double scan_needStartTimeByUs = (ddr_RealStartTimeByUs - scan_NotDisplayTimeByUs) - offsetTimeByUsScanToNomal;

            #endregion

            /***********************************************计算抽点数***********************************************/
            double _totalExtractNum = ui_NeedSampleIntervalByUs /ddrwritedsampleintervalbyus;
            readparams.TotalExtractNum = _totalExtractNum < 1 ? 1U : (uint)_totalExtractNum;
            if (data4What == DdrData4What.LA)//LA的后抽模块跟DSO的前抽模块是一样的
                readparams.TotalExtractNum = (UInt32)Extract_JiHe_MSO8000X.GetValidPreExtractNum((UInt64)readparams.TotalExtractNum);

            /***********************************************计算读取的点数*******************************************/
            int returnDotCount = (int)(((ddr_RealEndTimeByUs - ddr_RealStartTimeByUs) / readparams.TotalExtractNum / ddrwritedsampleintervalbyus));

            /***********************************************计算DDR读起始位置****************************************/
            double tmp_DdrReadDotStart = ((isscan ? scan_needStartTimeByUs : ddr_RealStartTimeByUs) / ddrwritedsampleintervalbyus);

            readparams.Interpolate_DiscardDotNum = 0;
            readparams.Interpolate_Num_Double = 1;

            int tmp_returnDotCount = returnDotCount;
            if (ui_NeedSampleIntervalByUs < ddrwritedsampleintervalbyus)
            {
                returnDotCount = 1000;

                /***********************************************计算插值倍率****************************************/
                readparams.Interpolate_Num_Double = (int)Math.Ceiling(returnDotCount * 1.0 / (uiSumTimeByUs * uS2fs / acquireAttribute.PerDataByfs_AtDdr));

                readparams.bInterpolateNumGT100 = false;
                if (readparams.Interpolate_Num_Double > 100)
                {
                    readparams.bInterpolateNumGT100 = true;
                    readparams.Interpolate_Num_Double = 100;
                }
                else if (readparams.Interpolate_Num_Double == 0)
                {
                    readparams.Interpolate_Num_Double = 1;
                }
                readparams.Interpolate_Num_Double = GetInterpolateValideNum((int)readparams.Interpolate_Num_Double);
                if (readparams.Interpolate_Num_Double > 1)
                {
                    //去掉插值后的小数部分
                    tmp_DdrReadDotStart = ((int)(tmp_DdrReadDotStart * readparams.Interpolate_Num_Double)) / readparams.Interpolate_Num_Double;
                    readparams.DdrReadStartDotPosition = Math.Floor(tmp_DdrReadDotStart);
                    readparams.Interpolate_DiscardDotNum = (Int32)((tmp_DdrReadDotStart - readparams.DdrReadStartDotPosition) * readparams.Interpolate_Num_Double);
                }
                else
                {
                    returnDotCount = tmp_returnDotCount;
                    readparams.Interpolate_Num_Double = 1;
                }

                returnDotCount = (int)((ddr_RealEndTimeByUs - ddr_RealStartTimeByUs) * readparams.Interpolate_Num_Double / (readparams.TotalExtractNum * acquireAttribute.PerDataByfs_AtDdr / uS2fs));
                readparams.TotalExtractNum = 1;
            }
            if (readparams.Interpolate_Num_Double <= 1)
            {
                //不插值
                readparams.Interpolate_Num_Double = 1;
                readparams.Interpolate_DiscardDotNum = 0;
                readparams.DdrReadStartDotPosition = tmp_DdrReadDotStart;
            }


            Int32 dbi_interp = 1;
            if (Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation)
            {
                if (Hd.UIMessage!.Timebase!.TmbScale < 0.02)
                {
                    returnDotCount *= 4;
                    dbi_interp = 4;
                }
            }

            readparams.PerChannelRecvDotsCount = returnDotCount;
            //comment by zhaoyong,right?
            //                         (       原坐标系下，触发点左边的时间[屏幕中心之左，为负数]                                )
            //ddr_RealStartTimeByUs:新坐标系下，触发点坐标的时间[屏幕中心之左，为负数],并进行了左右交处理的
            //结果，旧-新坐标系下触发点左边时间之差。
            readparams.StartTimeByus = (acquireAttribute.HdMessage?.Timebase?.TmbPosition ?? 0) - (acquireAttribute.HdMessage?.Timebase?.TmbScale ?? 0) * Constants.VIS_XDIVS_NUM / 2 - ddr_RealStartTimeByUs - ddr_AcqedDelayTimeByUs;
            readparams.SampleIntervalByUs = acquireAttribute.PerDataByfs_AtDdr * readparams.TotalExtractNum / readparams.Interpolate_Num_Double / dbi_interp / uS2fs;

            readparams.Data4What = data4What;

            //ExistsReadParams.Add(key, readParams);
            AcquingParameters.PerDataByfs_AtDMA = AcquingParameters.PerDataByfs_AtDdr * readparams.TotalExtractNum / readparams.Interpolate_Num_Double;
            return readparams;
        }
        internal ReadParams CalcDdrReadZoomParams(DdrData4What data4What, AcquireAttribute acquireAttribute, Double uiStartTimeByUs, Double uiSumTimeByUs, UInt32 uiTotalDots, long writedTimestamp)
        {
            if (data4What == DdrData4What.Upo)
                return CalcDdrReadParams_UPO(data4What, acquireAttribute, uiStartTimeByUs, uiSumTimeByUs, uiTotalDots, writedTimestamp);
            Boolean bisscan = acquireAttribute.HdMessage?.Timebase?.IsScan ?? false;
            #region Scan运行态
            if (bisscan && (!(Acquisition.AcqedDataMsg?.bAcquireStopped ?? false)))
            {
                ReadParams readparams_scanrunning = new ReadParams();
                readparams_scanrunning.WritedTimestamp = writedTimestamp;
                readparams_scanrunning.DMADataPath = DMAReadDataTypes.ScanFifo;
                readparams_scanrunning.PerChannelRecvDotsCount = Acquisition.ScanRunningNewDataPerChannelExistsDotCount;
                readparams_scanrunning.SampleIntervalByUs = (acquireAttribute.PerDataByfs_AtDdr * acquireAttribute.Scan2ExtractNum_Total / uS2fs);
                readparams_scanrunning.StartTimeByus = uiStartTimeByUs;
                readparams_scanrunning.Data4What = data4What;
                for (int channelIndex = 0; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
                {
                    readparams_scanrunning.Ddr_PosGainFine[channelIndex] = 1.0;
                    readparams_scanrunning.Ddr_PosOffset[channelIndex] = (short)(Hd.UIMessage!.Analog![channelIndex].PositionIndex * Constants.SAMPS_PER_YDIV / Constants.IDX_PER_YDIV + Constants.MAX_ADC_RES / 2);
                    readparams_scanrunning.Ddr_PosOffsetDelta[channelIndex] = 0;
                    readparams_scanrunning.Ddr_PosInvert[channelIndex] = false;
                }
                return readparams_scanrunning;
            }

            #endregion

            #region 非Scan时DDR中数据的时间信息，以触发时刻为0时刻
            String key = $"{acquireAttribute.AdcInterleaveMode}_{acquireAttribute.PerDataByfs_AtDdr}_{acquireAttribute.HardwareStorageWaveDotsCnt}_{acquireAttribute.SettingTrigPositionByfs}_{acquireAttribute.HdMessage?.Timebase?.TmbScale}_{Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay}_{Acquisition.ScanRunningNewDataPerChannelExistsDotCount}_{uiStartTimeByUs}_{uiSumTimeByUs}_{uiTotalDots}";
            if (ExistsReadParams.ContainsKey(key))
            {
                ReadParams readparams_exists = ExistsReadParams[key].Clone();
                readparams_exists.Data4What = data4What;
                readparams_exists.WritedTimestamp = writedTimestamp;
                return readparams_exists;
            }

            ReadParams readparams = new ReadParams();
            readparams.WritedTimestamp = writedTimestamp;

            Double ddr_WritedSampleIntervalByUs = acquireAttribute.PerDataByfs_AtDdr / uS2fs;
            Double ddr_ExistsTotalTimeByUs = acquireAttribute.HardwareStorageWaveDotsCnt * acquireAttribute.PerDataByfs_AtDdr / uS2fs;
            Double ddrtheorystarttimebyus = acquireAttribute.SettingTrigPositionByfs / uS2fs - ddr_ExistsTotalTimeByUs / 2;
            Double ddracqeddelaytimebyus = ddrtheorystarttimebyus > 0 ? ddrtheorystarttimebyus : 0;
            ddracqeddelaytimebyus = ddracqeddelaytimebyus - (((ddracqeddelaytimebyus * uS2fs) % 3200000) * 1.0) / uS2fs;//3200000fs=3.2ns
            ddrtheorystarttimebyus -= ddracqeddelaytimebyus;
            Double ddrtheoryendtimebyus = ddr_ExistsTotalTimeByUs + ddrtheorystarttimebyus;

            #endregion

            #region Scan暂停态时DDR中数据的时间信息，以数据最后一个点为0时刻

            var scandisplaytimebyus = 0D;
            var scannotdisplaytimebyus = 0D;
            var scanstarttimebyus = 0D;
            var scanendtimebyus = 0D;
            var offsettimebyusscantonomal = 0D;//触发时刻到DDR最右侧(0时刻)的偏移

            if (bisscan)
            {
                scandisplaytimebyus = Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay * acquireAttribute.PerDataByfs_AtDdr / uS2fs;
                scannotdisplaytimebyus = Acquisition.ScanPerChannelInDdrDotCount_NotDisplay * acquireAttribute.PerDataByfs_AtDdr / uS2fs;
                scanstarttimebyus = 0 - (scandisplaytimebyus + scannotdisplaytimebyus);
                scanendtimebyus = scandisplaytimebyus + scanstarttimebyus;
                offsettimebyusscantonomal = acquireAttribute.SettingTrigPositionByfs / uS2fs + (acquireAttribute.HdMessage?.Timebase?.TmbScale ?? 1.0) * Constants.VIS_XDIVS_NUM / 2 - 0;
                //统一0时刻： 将Scan的时间信息平移至以触发时刻为0时刻，并减去无效数据（未显示的时间）
                ddrtheorystarttimebyus = scanstarttimebyus + offsettimebyusscantonomal + scannotdisplaytimebyus;
                ddrtheoryendtimebyus = scanendtimebyus + offsettimebyusscantonomal + scannotdisplaytimebyus;
            }


            #endregion

            #region UI希望读取的数据时间信息

            Double uineedsampleintervalbyus = uiSumTimeByUs / uiTotalDots;
            Double uineedstarttimebyus = uiStartTimeByUs - uiSumTimeByUs / 2 - ddracqeddelaytimebyus;
            Double uineedendtimebyus = uiSumTimeByUs + uineedstarttimebyus;

            #endregion

            #region 实际读DDR数据的时间信息


            //相对于触发时刻的读起始时间
            Double ddrrealstarttimebyus = uineedstarttimebyus < ddrtheorystarttimebyus ? ddrtheorystarttimebyus : uineedstarttimebyus;
            Double ddrrealendtimebyus = uineedendtimebyus > ddrtheoryendtimebyus ? ddrtheoryendtimebyus : uineedendtimebyus;
            ddrrealstarttimebyus = ddrrealstarttimebyus > ddrtheoryendtimebyus ? ddrtheoryendtimebyus : ddrrealstarttimebyus;
            ddrrealendtimebyus = ddrrealendtimebyus < ddrtheorystarttimebyus ? ddrtheorystarttimebyus : ddrrealendtimebyus;

            //相对于DDR最右端为0时刻的读起始时间
            Double scanneedstarttimebyus = (ddrrealstarttimebyus - scannotdisplaytimebyus) - offsettimebyusscantonomal;

            #endregion

            /***********************************************计算抽点数***********************************************/
            Double totalextractnum = uineedsampleintervalbyus / ddr_WritedSampleIntervalByUs;

            totalextractnum = FitExtractNum(acquireAttribute.AdcInterleaveMode, (ulong)Math.Ceiling(totalextractnum));

            readparams.TotalExtractNum = totalextractnum < 1 ? 1U : (UInt32)totalextractnum;

            /***********************************************计算读取的点数*******************************************/
            Int32 returndotcount = (Int32)((ddrrealendtimebyus - ddrrealstarttimebyus) / readparams.TotalExtractNum / ddr_WritedSampleIntervalByUs);

            /***********************************************计算DDR读起始位置****************************************/
            Double tmp_ddrreaddotstart = (bisscan ? scanneedstarttimebyus : ddrrealstarttimebyus) / ddr_WritedSampleIntervalByUs;

            readparams.Interpolate_DiscardDotNum = 0;
            readparams.Interpolate_Num_Double = 1;

            Int32 tmpreturndotcount = returndotcount;
            if (uineedsampleintervalbyus < ddr_WritedSampleIntervalByUs)
            {
                returndotcount = 1000;

                /***********************************************计算插值倍率****************************************/
                readparams.Interpolate_Num_Double = (Int32)Math.Ceiling(returndotcount * 1.0 / (uiSumTimeByUs * uS2fs / acquireAttribute.PerDataByfs_AtDdr));

                readparams.Interpolate_Num_Double = FitInterpolateNum(acquireAttribute.AdcInterleaveMode, (ulong)Math.Ceiling(readparams.Interpolate_Num_Double));
                readparams.bInterpolateNumGT100 = false;
                if (readparams.Interpolate_Num_Double > 100)
                {
                    readparams.bInterpolateNumGT100 = true;
                    readparams.Interpolate_Num_Double = 100;
                }
                else if (readparams.Interpolate_Num_Double == 0)
                {
                    readparams.Interpolate_Num_Double = 1;
                }
                readparams.Interpolate_Num_Double = GetInterpolateValideNum((Int32)readparams.Interpolate_Num_Double);

                if (readparams.Interpolate_Num_Double > 1)
                {
                    //去掉插值后的小数部分
                    tmp_ddrreaddotstart = ((Int32)(tmp_ddrreaddotstart * readparams.Interpolate_Num_Double)) / readparams.Interpolate_Num_Double;
                    readparams.DdrReadStartDotPosition = Math.Floor(tmp_ddrreaddotstart);
                    readparams.Interpolate_DiscardDotNum = (Int32)((tmp_ddrreaddotstart - readparams.DdrReadStartDotPosition) * readparams.Interpolate_Num_Double);
                }
                else
                {
                    returndotcount = tmpreturndotcount;
                    readparams.Interpolate_Num_Double = 1;
                }

                returndotcount = (Int32)((ddrrealendtimebyus - ddrrealstarttimebyus) * readparams.Interpolate_Num_Double / (readparams.TotalExtractNum * acquireAttribute.PerDataByfs_AtDdr / uS2fs));
                readparams.TotalExtractNum = 1;
            }
            if (readparams.Interpolate_Num_Double <= 1)
            {
                //不插值
                readparams.Interpolate_Num_Double = 1;
                readparams.Interpolate_DiscardDotNum = 0;
                readparams.DdrReadStartDotPosition = tmp_ddrreaddotstart;
            }
            readparams.PerChannelRecvDotsCount = returndotcount;
            //comment by zhaoyong,right?
            //                         (       原坐标系下，触发点左边的时间[屏幕中心之左，为负数]                                )
            //ddr_RealStartTimeByUs:新坐标系下，触发点坐标的时间[屏幕中心之左，为负数],并进行了左右交处理的
            //结果，旧-新坐标系下触发点左边时间之差。
            readparams.StartTimeByus = uineedstarttimebyus - ddrrealstarttimebyus /*- ddr_AcqedDelayTimeByUs*/;
            readparams.SampleIntervalByUs = acquireAttribute.PerDataByfs_AtDdr * readparams.TotalExtractNum / readparams.Interpolate_Num_Double / uS2fs;

            readparams.Data4What = data4What;

            //ExistsReadParams.Add(key, readParams);
            return readparams;
        }
        internal ReadParams CalcDdrReadParams_SourceData(AcquireAttribute acquireAttribute, double UI_StartDotIndx, double UI_TotalDots)
        {
            bool bIsScan = acquireAttribute.HdMessage?.Timebase?.IsScan ?? false;
            #region Scan运行态
            if (bIsScan && (!(Acquisition.AcqedDataMsg?.bAcquireStopped ?? false)))
            {
                ;//不可能出现的情况
            }

            #endregion

            #region 非Scan时DDR中数据的时间信息，以触发时刻为0时刻
            ReadParams readParams = new ReadParams();

            double ddr_WritedSampleIntervalByUs = acquireAttribute.PerDataByfs_AtDdr / uS2fs;
            double ddr_ExistsTotalTimeByUs = acquireAttribute.HardwareStorageWaveDotsCnt * acquireAttribute.PerDataByfs_AtDdr / uS2fs;
            double ddr_theoryStartTimeByUs = acquireAttribute.SettingTrigPositionByfs / uS2fs - ddr_ExistsTotalTimeByUs / 2;
            double ddr_AcqedDelayTimeByUs = ddr_theoryStartTimeByUs > 0 ? ddr_theoryStartTimeByUs : 0;
            ddr_theoryStartTimeByUs -= ddr_AcqedDelayTimeByUs;
            double ddr_theoryEndTimeByUs = ddr_ExistsTotalTimeByUs + ddr_theoryStartTimeByUs;

            #endregion

            #region Scan暂停态时DDR中数据的时间信息，以数据最后一个点为0时刻

            var scan_DisplayTimeByUs = 0D;
            var scan_NotDisplayTimeByUs = 0D;
            var scan_StartTimeByUs = 0D;
            var scan_EndTimeByUs = 0D;
            var offsetTimeByUsScanToNomal = 0D;//触发时刻到DDR最右侧(0时刻)的偏移

            if (bIsScan)
            {
                scan_DisplayTimeByUs = Acquisition.ScanPerChannelInDdrTotalDotCount_AlreadyDisplay * acquireAttribute.PerDataByfs_AtDdr / uS2fs;
                scan_NotDisplayTimeByUs = Acquisition.ScanPerChannelInDdrDotCount_NotDisplay * acquireAttribute.PerDataByfs_AtDdr / uS2fs;
                scan_StartTimeByUs = 0 - (scan_DisplayTimeByUs + scan_NotDisplayTimeByUs);
                scan_EndTimeByUs = scan_DisplayTimeByUs + scan_StartTimeByUs;
                offsetTimeByUsScanToNomal = acquireAttribute.SettingTrigPositionByfs / uS2fs + (acquireAttribute.HdMessage?.Timebase?.TmbScale ?? 1.0) * Constants.VIS_XDIVS_NUM / 2 - 0;
                //统一0时刻： 将Scan的时间信息平移至以触发时刻为0时刻，并减去无效数据（未显示的时间）
                ddr_theoryStartTimeByUs = scan_StartTimeByUs + offsetTimeByUsScanToNomal + scan_NotDisplayTimeByUs;
                ddr_theoryEndTimeByUs = scan_EndTimeByUs + offsetTimeByUsScanToNomal + scan_NotDisplayTimeByUs;
            }


            #endregion

            #region UI希望读取的数据时间信息

            double ui_NeedSampleIntervalByUs = acquireAttribute.PerDataByfs_AtDdr / uS2fs;
            double ui_NeedStartTimeByUs = ddr_theoryStartTimeByUs + UI_StartDotIndx * acquireAttribute.PerDataByfs_AtDdr / uS2fs;
            double ui_NeedEndTimeByUs = ui_NeedStartTimeByUs + UI_TotalDots * acquireAttribute.PerDataByfs_AtDdr / uS2fs;

            #endregion

            #region 实际读DDR数据的时间信息


            //相对于触发时刻的读起始时间
            double ddr_RealStartTimeByUs = ui_NeedStartTimeByUs < ddr_theoryStartTimeByUs ? ddr_theoryStartTimeByUs : ui_NeedStartTimeByUs;
            double ddr_RealEndTimeByUs = ui_NeedEndTimeByUs > ddr_theoryEndTimeByUs ? ddr_theoryEndTimeByUs : ui_NeedEndTimeByUs;
            ddr_RealStartTimeByUs = ddr_RealStartTimeByUs > ddr_theoryEndTimeByUs ? ddr_theoryEndTimeByUs : ddr_RealStartTimeByUs;
            ddr_RealEndTimeByUs = ddr_RealEndTimeByUs < ddr_theoryStartTimeByUs ? ddr_theoryStartTimeByUs : ddr_RealEndTimeByUs;

            //相对于DDR最右端为0时刻的读起始时间
            double scan_needStartTimeByUs = (ddr_RealStartTimeByUs - scan_NotDisplayTimeByUs) - offsetTimeByUsScanToNomal;

            #endregion

            /***********************************************计算抽点数***********************************************/
            double _totalExtractNum = ui_NeedSampleIntervalByUs / ddr_WritedSampleIntervalByUs;
            readParams.TotalExtractNum = _totalExtractNum < 1 ? 1U : (uint)_totalExtractNum;

            /***********************************************计算读取的点数*******************************************/
            int returnDotCount = (int)((ddr_RealEndTimeByUs - ddr_RealStartTimeByUs) / readParams.TotalExtractNum / ddr_WritedSampleIntervalByUs + 0.1);

            /***********************************************计算DDR读起始位置****************************************/
            double tmp_DdrReadDotStart = (bIsScan ? (long)(scan_needStartTimeByUs * uS2fs) : (long)(ddr_RealStartTimeByUs * uS2fs)) / (long)(ddr_WritedSampleIntervalByUs * uS2fs);

            readParams.Interpolate_DiscardDotNum = 0;
            readParams.Interpolate_Num_Double = 1;

            int tmp_returnDotCount = returnDotCount;
            if (ui_NeedSampleIntervalByUs < ddr_WritedSampleIntervalByUs)
            {
                ;//1抽1的情况，所以不可能出现
            }
            readParams.PerChannelRecvDotsCount = returnDotCount;
            //comment by zhaoyong,right?
            //                         (       原坐标系下，触发点左边的时间[屏幕中心之左，为负数]                                )
            //ddr_RealStartTimeByUs:新坐标系下，触发点坐标的时间[屏幕中心之左，为负数],并进行了左右交处理的
            //结果，旧-新坐标系下触发点左边时间之差。
            readParams.StartTimeByus = (acquireAttribute.HdMessage?.Timebase?.TmbPosition ?? 0) - (acquireAttribute.HdMessage?.Timebase?.TmbScale ?? 0) * Constants.VIS_XDIVS_NUM / 2 - ddr_RealStartTimeByUs - ddr_AcqedDelayTimeByUs;
            readParams.SampleIntervalByUs = acquireAttribute.PerDataByfs_AtDdr * readParams.TotalExtractNum / readParams.Interpolate_Num_Double / uS2fs;

            readParams.Data4What = DdrData4What.Dso;
            readParams.DdrReadStartDotPosition = tmp_DdrReadDotStart / 2;
            //ExistsReadParams.Add(key, readParams);
            return readParams;
        }

        internal virtual String TryGetData(Object paramInfos, out Object? paramsData)
        {
            if (paramInfos is String)
            {
                String paramstr = (String)paramInfos;
                Int32 spiltid = paramstr.IndexOf("_");
                if (spiltid > 0)
                {
                    Boolean parseflag = Enum.TryParse(paramstr.Substring(0, spiltid), out AnalogParamEnum paramenum);
                    if (parseflag)
                    {
                        switch (paramenum)
                        {
                            case AnalogParamEnum.WaveByteSize:
                                Int32 bytesize = 0;
                                Boolean ret = TryGetWaveByteSize(paramstr.Substring(spiltid + 1, paramstr.Length - spiltid - 1), ref bytesize);
                                if (ret)
                                {
                                    paramsData = bytesize;
                                    return String.Empty;
                                }
                                break;
                            case AnalogParamEnum.AdcWaveData:
                                paramsData = TryGetWaveData_Adc(paramstr.Substring(spiltid + 1, paramstr.Length - spiltid - 1));
                                return String.Empty;

                            case AnalogParamEnum.StorageDotsCnt:
                                paramsData = TryGetStorageDotsCnt(paramstr.Substring(spiltid, paramstr.Length - spiltid), '_');
                                return String.Empty;
                            case AnalogParamEnum.TriggerAddrStart:
                                paramsData = TryGetTriggerAddrStart();
                                return String.Empty;
                            case AnalogParamEnum.SaveDataSegementDotsLength:
                                paramsData = TryGetSaveDataSegementDotsLength();
                                return String.Empty;
                            case AnalogParamEnum.AdcInterleaveMode:
                                paramsData = TryGetAdcInterleaveMode(paramstr.Substring(spiltid, paramstr.Length - spiltid), '_');
                                return String.Empty;
                        }
                    }
                }
            }
            paramsData = null;
            return "Not Supported!";
        }

        private const Int32 _DefaultWaveDataCnt = 100;

        internal virtual Boolean TryGetWaveByteSize(String waveName, ref Int32 byteSize)
        {
            byteSize = _DefaultWaveDataCnt * sizeof(UInt16);
            return true;
        }

        internal virtual List<UInt16>? TryGetWaveData_Adc(String waveName)
        {
            return new UInt16[_DefaultWaveDataCnt].ToList();
        }

        /// <summary>
        /// 单个ADC的单核支持的存储点数
        /// </summary>
        protected Int32[]? _StorageDotsCntPerCore;

        internal virtual List<Int32> TryGetStorageDotsCnt(String ActiveChnlsStr, Char spiltChar)
        {
            List<ChannelId> activechnls = new();
            String[] chnlstrarray = ActiveChnlsStr.Split(spiltChar);
            foreach (String chnlstr in chnlstrarray)
            {
                Boolean parseflag = Enum.TryParse(chnlstr, out ChannelId chnlid);
                if (parseflag) { activechnls.Add(chnlid); }
            }
            if (activechnls.Count == 0)
            {
                return _StorageDotsCntPerCore?.ToList() ?? new List<Int32>();
            }

            //Int32 corecnt = AnalogAcquireModel?.GetUsedCoreCntPerChnl(activechnls.ToArray())?.Values?.Min() ?? 1;
            Int32 array = AnalogAcquireModel?.GetUsedCoreCntPerChnl(activechnls.ToArray()).Count()==0?1:(int) AnalogAcquireModel?.GetUsedCoreCntPerChnl(activechnls.ToArray())?.Values?.Min();
            Int32 corecnt = array;
            return _StorageDotsCntPerCore?.Select(o => o * corecnt)?.ToList() ?? new List<Int32>();
        }

        /// <summary>
        /// 获取触发开始地址
        /// </summary>
        /// <returns></returns>
        internal virtual int TryGetTriggerAddrStart()
        {
            /**********************
            软件里面计算的分段存储的段数最大值
                1、DDR总地址长度，这个FPGA里面是个常数：29'h1FFF_FFFF
                2、存触发地址的起始地址，这个FPGA里面是个常数：29'h1FC0_0000
                3、存储波形长度L个点
                4、通道数目n通道
                    软件计算出来的段数 = (29'h1FC0_0000/8*64)/L/n

            FPGA里面计算的分段存储的段数最大值
                1、DDR总地址长度，这个FPGA里面是个常数：29'h1FFF_FFFF
                2、存触发地址的起始地址，这个FPGA里面是个常数：29'h1FC0_0000
                3、存触发地址的最大数目，地址8突发
                    7000X,8000HD   （29'h1FFF_FFFF - 29'h1FC0_0000）/8 = 7FFFF;

            如果（软件计算出来的段数 <存触发地址的最大数目） ，发送软件计算出来的段数
            如果（软件计算出来的段数 >=存触发地址的最大数目） ，发送存触发地址的最大数目
            *****************************/
            return 0x1FC0_0000;
        }

        /// <summary>
        /// 获取保存深存储数据时，定义的段长度
        /// </summary>
        /// <returns></returns>
        internal virtual UInt64 TryGetSaveDataSegementDotsLength() => 0;

        /// <summary>
        /// 获取当前Adc的交织模式
        /// </summary>
        /// <returns></returns>
        internal virtual AdcInterleaveMode TryGetAdcInterleaveMode(String activeChnlsStr, Char spiltChar)
        {
            int activeChnlState = 0;
            String[] chnlStrArray = activeChnlsStr.Split(spiltChar);
            foreach (String chnlstr in chnlStrArray)
            {
                Boolean parseFlag = Enum.TryParse(chnlstr, out ChannelId chnlid);
                if (parseFlag && ChannelIdExt.GetAnalogs().Contains(chnlid))
                    activeChnlState |= (0x1 << (int)chnlid);
                if (parseFlag && ChannelIdExt.GetDigitals().Contains(chnlid))
                {
                    activeChnlState = 0xf;
                    break;
                }
            }

            return AnalogAcquireModel?.GetAcqModeInterleaveByChnlState((UInt32)activeChnlState).InterleaveMode ?? AdcInterleaveMode.Mode1To1;
        }
            internal virtual void ChnlSyncDiscardDotsEx(/*HdMessage? hdMessage*/) 
        {
        }
        internal virtual SyncParams[] SyncParams()
        {
            return new SyncParams[] { };
        }


    }
    internal class ChannelBdAdcInputDefine
    {
        public AcqBdNo BdNo { get; set; }
        public Int32 AdcIndex { get; set; }
        public int InputPort_AIs1 { get; set; }
        public bool bIs20GMode { get; set; } = false;
    }
    internal class AmpCoefficientFileInfo
    {
        public string FileName = "";
        public int CRCCode = 0;
        public bool bOk = false;
    }
    public class ReadParams
    {
        public long WritedTimestamp = 0;

        /// <summary>
        /// 读取的起始地址相对于波形起始（触发地址-预触发深度的地址个数）的偏移
        /// </summary>
        public double DdrReadStartDotPosition;

        /// 硬件后抽总倍数
        /// </summary>
        public UInt32 TotalExtractNum = 1;
        /// <summary>
        /// 硬件后抽基数
        /// </summary>
        public UInt64 ExtractNum_Base = 1;
        /// <summary>
        /// 硬件后抽倍数
        /// </summary>
        public UInt64 ExtractNum_Multiple = 1;

        /// <summary>
        /// 硬件插值倍数
        /// </summary>
        public double Interpolate_Num_Double = 1;
        public Int32 Interpolate_DiscardDotNum = 0;
        public Boolean bInterpolateNumGT100 = false;
        /// <summary>
        /// 丢点数
        /// </summary>
        public UInt32 DiscardDotCt;

        /// <summary>
        /// 读取点数
        /// </summary>
        public Double PerChannelRecvDotsCount;

        /// <summary>
        /// 读回到软件的采样数据的采样间隔（单位：us）
        /// </summary>
        public Double SampleIntervalByUs;

        /// <summary>
        /// 读回到软件的采样数据起点相对配置参数时屏幕最左边的时间（单位：us）
        /// </summary>
        public Double StartTimeByus;

        public UInt32 UPO_Level1ExtractNum;
        public UInt32 UPO_Level2ExtractNum;
        public UInt32 UPO_ResultColoumnNum;
        public override Boolean Equals(object? obj)
        {
            return obj != null && obj is ReadParams && IsEqual((obj as ReadParams)!);
        }
        public ReadParams Clone()
        {
            ReadParams readParams = new ReadParams();
            readParams.DiscardDotCt = DiscardDotCt;
            readParams.DdrReadStartDotPosition = DdrReadStartDotPosition;
            readParams.DMADataPath = DMADataPath;
            readParams.Interpolate_DiscardDotNum = Interpolate_DiscardDotNum;
            readParams.ExtractNum_Base = ExtractNum_Base;
            readParams.ExtractNum_Multiple = ExtractNum_Multiple;
            readParams.Interpolate_Num_Double = Interpolate_Num_Double;
            readParams.PerChannelRecvDotsCount = PerChannelRecvDotsCount;
            readParams.SampleIntervalByUs = SampleIntervalByUs;
            readParams.StartTimeByus = StartTimeByus;
            readParams.TotalExtractNum = TotalExtractNum;
            readParams.WritedTimestamp = WritedTimestamp;
            readParams.bInterpolateNumGT100 = bInterpolateNumGT100;
            return readParams;
        }
        public Boolean IsEqual(ReadParams source)
        {
            Boolean bEqual = Object.ReferenceEquals(this, source) ||
                   (DdrReadStartDotPosition == source.DdrReadStartDotPosition) &&
                   (Interpolate_DiscardDotNum == source.Interpolate_DiscardDotNum) &&
                   (TotalExtractNum == source.TotalExtractNum) &&
                   (Interpolate_Num_Double == source.Interpolate_Num_Double) &&
                   (PerChannelRecvDotsCount == source.PerChannelRecvDotsCount) &&
                   (DiscardDotCt == source.DiscardDotCt) &&
                   (SampleIntervalByUs == source.SampleIntervalByUs) &&
                   (WritedTimestamp == source.WritedTimestamp) &&
                   (Data4What == source.Data4What) &&
                   (bInterpolateNumGT100 == source.bInterpolateNumGT100) &&
                   (StartTimeByus == source.StartTimeByus);
            if (bEqual)
            {
                for (int channelIndex = 0; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
                {
                    if (Ddr_PosGainFine[channelIndex] != source.Ddr_PosGainFine[channelIndex])
                    {
                        return false;
                    }

                    if (Ddr_PosOffsetDelta[channelIndex] != source.Ddr_PosOffsetDelta[channelIndex])
                    {
                        return false;
                    }

                    if (Ddr_PosOffset[channelIndex] != source.Ddr_PosOffset[channelIndex])
                    {
                        return false;
                    }

                    if (Ddr_PosInvert[channelIndex] != source.Ddr_PosInvert[channelIndex])
                    {
                        return false;
                    }
                }
            }
            return bEqual;
        }
        internal DMAReadDataTypes DMADataPath
        {
            get;
            set;
        } = DMAReadDataTypes.AnalogChannelDdr;
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        internal double[] Ddr_PosGainFine = new double[] { 1.0, 1.0, 1.0, 1.0 };
        internal short[] Ddr_PosOffsetDelta = new short[] { 0, 0, 0, 0 };
        internal short[] Ddr_PosOffset = new short[] { 128, 128, 128, 128 };
        internal Boolean[] Ddr_PosInvert = new Boolean[] { false, false, false, false };

        internal DdrData4What Data4What = DdrData4What.Dso;
    }
    internal enum DdrData4What
    {
        Dso,
        Upo,
        LA
    }
}

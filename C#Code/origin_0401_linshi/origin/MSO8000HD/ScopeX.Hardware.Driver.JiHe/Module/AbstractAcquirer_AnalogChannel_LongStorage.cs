using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;
using ScopeX.ComModel;
using System.Collections;
using System.Xml.Linq;
using System.Text;
using MathWorks.MATLAB.NET.Arrays;
using auto_peak_filter;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 模拟通道采集器的深存储部分，提供深存储功能，可不参与编译
    /// </summary>
    public abstract partial class AbstractAcquirer_AnalogChannel : AbstractAcquirer
    {
        internal override void SetWrittedTimeStamp()
        {
            if (Acquisition.CurrDataAcquireAttribute.WriteParams != null)
            {
                Acquisition.CurrDataAcquireAttribute.WriteParams.WritedTimestamp = Acquisition.AcqFull_TimeStamp;
            }
        }
        internal override void Reset()
        {
            base.Reset();
            AcquedParameters.WriteParams = null;
        }

        #region 深存储内部变量定义
        private Byte[] _DmaBuff = new Byte[0];
        private List<List<UInt16>> _ChnlData = new();

        /// <summary>
        /// 为不同高级功能缓存的采样数据
        /// </summary>
        private readonly Dictionary<ReadInfo, Dictionary<ChannelId, (List<UInt16> buff, ReadParams readParams)>> _AllChnlData = new();


        /// <summary>
        /// 要解析出所有通道的一个点，需要多少Byte的原始DMA数据
        /// </summary>
        protected Double DmaBytesPerDotForAllChannel = 4.0;

        #endregion

        #region 通过接口提供给外部调用的方法
        private void LongStorageInitAcq()
        {
            AcquingParameters.WriteParams = CalcWrittingParams();
            HdCtrl_AnalogDDR.ConfigWrite(AcquingParameters.WriteParams, AcquingParameters);
        }
        internal virtual void DoMarkAfterAcqFinished()
        {
            AcquingParameters.WriteParams!.WritedTimestamp = DateTime.Now.Ticks;
        }
        private Boolean LongStorageReadAcq(List<ReadInfo> readInfoList, CancellationToken? softResetToken)
        {
            Boolean ans = true;
            if (readInfoList.Count == 0)
            {
                return false;
            }

            /*根据读取需求信息去采数*/
            foreach (var readInfo in readInfoList)
            {
                bool equalflag = false;
                //如果是读老数据判断是否重读，读新数据直接读取不用判断
                if (Acquisition.bReadOldData)
                {
                    var readingParam = CalcReadParamsByWriteParams(AcquedParameters, AcquedParameters.WriteParams, readInfo);
                    //此处去找已经读取波形的Param,是根据readInfo的引用来判断的，故新生成的readInfo一定找不到，返回null;
                    var readedParam = GetReadedParam(readInfo.ChannelIds.Count > 0 ? readInfo.ChannelIds[0] : ChannelId.C1, readInfo);
                    equalflag = readedParam?.Equals(readingParam) ?? false;
                    if (!equalflag)
                    {

                    }
                }

                if (!equalflag)
                {
                    ans &= ReadAcq(readInfo, softResetToken);
                    if (ans && Hd.UIMessage?.Decoder?.Where(x => x.Active)?.Count() > 0)
                    {
                        DecodeDataSource.NeedTakeNewData = true;
                    }
                }
            }

            //数据暂存区管理
            if (ans)
            {
                for (Int32 chnlId = 0; chnlId < _ChnlData.Count; chnlId++)
                {
                    if (_AllChnlData.ContainsKey(readInfoList[0]) && _AllChnlData[readInfoList[0]].ContainsKey((ChannelId)chnlId))
                    {
                        AcqedDataPool.AnalogChData.AllChannelData[chnlId].Clear();
                        AcqedDataPool.AnalogChData.AllChannelData[chnlId].AddRange(_AllChnlData[readInfoList[0]][(ChannelId)chnlId].buff);
                    }
                }
                foreach (var readinfo in _AllChnlData.Keys)
                {
                    if (!readInfoList.Contains(readinfo))
                    {
                        _AllChnlData.Remove(readinfo);
                    }
                }
            }
            return ans;
        }

        /// <summary>
        /// 尝试直接获取波形数据，如果没有缓存，重新根据需求进行计算并读取
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="readInfo"></param>
        /// <param name="wfmData"></param>
        /// <param name="wfmSampleInfo"></param>
        /// <param name="softResetToken"></param>
        /// <returns></returns>
        private Boolean LongStorageTakeWave(ChannelId ch, ReadInfo readInfo, List<UInt16> wfmData, WfmSampleInfo wfmSampleInfo, CancellationToken? softResetToken)
        {
            var ret = GetReadedWfm(ch, readInfo, wfmData, wfmSampleInfo);



            if (ret)
            {
                //var tmp =wfmData;
                //List<UInt16> trig = new();

                //if (/*datatype == 1 &&*/ tmp != null && /*ch == (Hd.UIMessage?.Trigger?.Edge?.Source ?? ChannelId.C1) &&*/
                //    (Hd.UIMessage?.Trigger?.TrigType ?? TriggerType.Edge) == TriggerType.Edge)
                //{
                //    UInt32 trifvol = AbstractAcquirer_AnalogChannel.GetLevelByVoltage(ch, Hd.UIMessage?.Trigger?.Edge?.Position ?? 0);
                //    (Int64 depth, Int64 waveDepth) = Hd.CurrProduct?.Acquirer_AnalogChannel?.GetTrigXDepth() ?? (0, 0);
                //    Boolean isrise = (Hd.UIMessage?.Trigger?.Edge?.Slope ?? EdgeSlope.Rise) == EdgeSlope.Rise;
                //    //for (Int32 i = 0; i < tmp.Count; i++)
                //    {
                //        trig = (FixEdgeTrig(tmp.ToArray(), (UInt16)trifvol, (Int32)depth, 1000, isrise).ToList());
                //    }
                //}
                //else if (tmp != null)
                //{
                //    trig = tmp;
                //}
                //else
                //{
                //    trig = (AcqedDataPool.AnalogChData.AllChannelData[(Int32)ch]);
                //}

                //wfmData=(trig);
                ////当未取到数据时，返回false，保证UI绘图正常，采集大循环不会异常退出
                //if (AcqedDataPool.AnalogChData.AllChannelData[(Int32)ch].Count <= 0)
                //{
                //    Monitor.Exit(AcqedDataPool.UpdateDataLock);
                //    return false;
                //}
                return true;
            }
            else
            {
            }

            //if (ReadAcq(readInfo, softResetToken))
            //{
            //    return GetReadedWfm(ch, readInfo, wfmData, wfmSampleInfo);
            //}

            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen || Hd.BPowerOff)
            {
                Monitor.Enter(AcqedDataPool.UpdateDataLock);
                wfmData.AddRange(AcqedDataPool.AnalogChData.AllChannelData[(Int32)ch]);
                Monitor.Exit(AcqedDataPool.UpdateDataLock);
                wfmSampleInfo.SampleIntervalByus = 1.0 / 1E9;
                wfmSampleInfo.StartTimeByus = AcquingParameters.HdMessage!.Timebase!.TmbPosition;
                wfmSampleInfo.HdMessage = AcquedParameters.HdMessage;
                return true;
            }
            return false;
        }


        private List<List<UInt16>> _SplitData = new()
                {
                    new List<ushort>(),
                    new List<ushort>(),
                    new List<ushort>(),
                    new List<ushort>(),
                };

        private List<Byte> _StrogaeData = new();

        private Int32 LongStorageSaveSourceData(ChannelId ch, ReadInfo readInfo, FileStream? fStream, String format, CancellationToken? softResetToken)
        {
            if (fStream == null)
            {
                return 0;
            }
            Int32 activedChannels = AcquedParameters.CurrChBWModeAndActiveState & 0xff;
            var active = (activedChannels & (1 << (int)ch)) != 0;
            ushort zero = (ushort)((AcquedParameters.HdMessage!.Analog![(int)ch].PositionIndex * Constants.SAMPS_PER_YDIV / Constants.IDX_PER_YDIV) + 128);
            List<ushort> _Zeroarray = Enumerable.Repeat(zero, (Int32)readInfo.pkgInfo.DotsCount).ToList();
            if (AcquedParameters.WriteParams == null)
            {
                return 0;
            }
            ReadParams? readingParams = Hd.CurrProduct!.Acquirer_AnalogChannel!.CalcDdrReadParams_SourceData(AcquedParameters, readInfo.pkgInfo.StartTimeByus/*实际是开始点位置*/, readInfo.pkgInfo.DotsCount);
            if (readingParams == null)
            {
                return 0;
            }
            HdCtrl_AnalogDDR.ConfigRead(readingParams, AcquedParameters);

            uint dotBytes = (uint)Math.Ceiling(Constants.ADC_BITS / 8D);
            int dmaLengthBytes = ((int)(readingParams.PerChannelRecvDotsCount * dotBytes * 4 + 1023) / 1024 * 1024);
            if (dmaLengthBytes <= 0)
            {
                return 0;
            }
            if (_DmaBuff.Length != dmaLengthBytes)
            {
                _DmaBuff = new Byte[dmaLengthBytes];
            }

            var bOk = HdCtrl_AnalogDDR.ReadDMA((UInt32)dmaLengthBytes, _DmaBuff);
            if (!bOk)
            {
                return 0;
            }

            _StrogaeData.Clear();

            AnalogChannelDataSplit(ch, _DmaBuff, (Int32)readingParams.PerChannelRecvDotsCount, _StrogaeData);

            if (_Zeroarray.Count != _SplitData[(Int32)ch].Count)
            {
                _Zeroarray = Enumerable.Repeat(zero, _SplitData[(Int32)ch].Count).ToList();
            }
            if (!active)
            {
                _SplitData[(Int32)ch] = _Zeroarray;
            }

            try
            {
                if (format.Equals("bin"))
                {
                    fStream.Write(_StrogaeData.ToArray(), 0, _StrogaeData.Count);
                }
                else //txt
                {
                    StringBuilder sb = new StringBuilder();
                    for (Int32 i = 0; i < _StrogaeData.Count / 2; i++)
                    {
                        Int32 data = _StrogaeData[2 * i] | (_StrogaeData[2 * i + 1] << 8);
                        sb.Append(data).Append(",\r\n");
                    }
                    var byteBuffer = Encoding.Default.GetBytes(sb.ToString());
                    fStream.Write(byteBuffer, 0, byteBuffer.Length);
                }

            }
            catch (Exception ex)
            {
                Hd.SysLogger?.Invoke(ex.Message, "Debug");
                return -1;
            }
            return 1;
        }


        private Boolean LongStorageTakeSegmentWave(ChannelId ch, ReadInfo readInfo, Int32 segmentStartIndex, Int32 segmentCnt, List<ushort[]> wfmData, WfmSampleInfo wfmSampleInfo, CancellationToken? softResetToken, Boolean b4SourceData = false)
        {
            Int32 activedChannels = AcquedParameters.CurrChBWModeAndActiveState & 0xff;
            var active = (activedChannels & (1 << (int)ch)) != 0;
            ushort zero = (ushort)((AcquedParameters.HdMessage!.Analog![(int)ch].PositionIndex * Constants.SAMPS_PER_YDIV / Constants.IDX_PER_YDIV) + 128);
            var zeroarray = Enumerable.Repeat(zero, (int)readInfo.pkgInfo.DotsCount).ToArray();
            for (int index = segmentStartIndex; index < segmentStartIndex + segmentCnt; index++)
            {
                if (AcquedParameters.WriteParams == null)
                {
                    return false;
                }
                ReadParams? readingParams;
                if (!b4SourceData)
                    readingParams = CalcReadParamsByWriteParams(AcquedParameters, AcquedParameters.WriteParams, readInfo);
                else
                    readingParams = Hd.CurrProduct!.Acquirer_AnalogChannel!.CalcDdrReadParams_SourceData(AcquedParameters, readInfo.pkgInfo.StartTimeByus/*实际是开始点位置*/, readInfo.pkgInfo.DotsCount);
                if (readingParams == null)
                {
                    return false;
                }
                wfmSampleInfo.SampleIntervalByus = readingParams.SampleIntervalByUs;
                wfmSampleInfo.StartTimeByus = readingParams.StartTimeByus;
                wfmSampleInfo.HdMessage = AcquedParameters.HdMessage;

                HdCtrl_AnalogDDR.ConfigRead(readingParams, AcquedParameters);
                bool bReadFromDdr = true;
                if ((Acquisition.AcqedDataMsg!.Timebase!.IsScan && !Acquisition.AcqedDataMsg.bAcquireStopped))
                {
                    bReadFromDdr = false;
                }

                if (bReadFromDdr)
                {
                    //var start = (UInt32)index - 1;//起始段号要减1
                    //var end = (UInt32)index - 1;//起始段号要减1
                    //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDR_Rd_Seg_Num_Begin_L16, AcqBdReg.W.LSCtrl_DDR_Rd_Seg_Num_Begin_H16, start);
                    //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDR_Rd_Seg_Num_End_L16, AcqBdReg.W.LSCtrl_DDR_Rd_Seg_Num_End_H16, end);
                    ////Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Dpo_DpxEnable,0);//暂时关闭乒乓
                    //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.Dpo_DpxEnable, (Hd.UIMessage?.bAcquireStopped ?? false) ? 0U : 1U);
                    ////Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DataPath_MuxDataPathACQ, 1);
                    //HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                    //HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);
                }
                int dmaLengthBytes = ((int)(readingParams.PerChannelRecvDotsCount * 2 * 4 + 1023) / 1024 * 1024);
                if (dmaLengthBytes <= 0)
                {
                    if (bReadFromDdr)
                    {
                        HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                    }

                    return false;
                }
                if (_DmaBuff.Length != dmaLengthBytes)
                {
                    _DmaBuff = new Byte[dmaLengthBytes];
                }

                var bOk = HdCtrl_AnalogDDR.ReadDMA((UInt32)dmaLengthBytes, _DmaBuff);
                if (bReadFromDdr)
                {
                    HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, data: 0x0);
                }

                if (!bOk)
                {
                    return false;
                }

                AnalogChannelDataSplit(0xf, _DmaBuff, (Int32)readingParams.PerChannelRecvDotsCount, _SplitData, true);

                if (zeroarray.Length != _SplitData[(Int32)ch].Count)
                {
                    zeroarray = Enumerable.Repeat(zero, _SplitData[(Int32)ch].Count).ToArray();
                }

                wfmData.Add(active ? _SplitData[(Int32)ch].ToArray() : zeroarray);
            }
            wfmSampleInfo.HdMessage = AcquedParameters.HdMessage;

            return true;
        }

        public Int32 ReadCollectedFrameCnt()
        {
            var count = 0;
            if (Hd.UIMessage!.Timebase!.CallBack)
            {
                count = (Int32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.LSCtrl_MultiSegement_Seg_Num_L16, AcqBdNo.B1);
            }
            else
            {
                //暂定使用此寄存器 已写入的段数
                var countl = (Int32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.LSCtrl_DDR_Rd_Every_Seg_Time_L16, AcqBdNo.B1);
                var countm = (Int32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.LSCtrl_DDR_Rd_Every_Seg_Time_M16, AcqBdNo.B1);
                count = countm << 16 | countl;
            }
            return count;
        }

        #endregion

        #region 内部辅助函数
        private ReadParams? GetReadedParam(ChannelId ch, ReadInfo readInfo)
        {
            if (_AllChnlData.ContainsKey(readInfo) && _AllChnlData[readInfo].ContainsKey(ch))
            {
                return _AllChnlData[readInfo][ch].readParams;
            }

            return null;
        }

        private Boolean GetReadedWfm(ChannelId ch, ReadInfo readInfo, List<UInt16> wfmData, WfmSampleInfo wfmSampleInfo)
        {
            Int32 activedChannels = AcquedParameters.CurrChBWModeAndActiveState & 0xff;
            //if ((activedChannels & (1 << (int)ch)) != 0)
            {
                var readingparam = CalcReadParamsByWriteParams(AcquedParameters, AcquedParameters.WriteParams, readInfo);
                var readerparam = GetReadedParam(ch, readInfo);
                var equalflag = readInfo.Mark == "Ai" ? true : readerparam?.Equals(readingparam) ?? false;
                if (Hd.UIMessage!.Timebase!.SegmentActive == 1)
                {
                    equalflag = false;
                    return false;
                }
                if (equalflag)
                {
                    if (_AllChnlData.ContainsKey(readInfo))
                    {
                        if (_AllChnlData[readInfo].ContainsKey(ch))
                        {
                            Monitor.Enter(AcqedDataPool.UpdateDataLock);
                            wfmData.AddRange(_AllChnlData[readInfo][ch].buff);
                            Monitor.Exit(AcqedDataPool.UpdateDataLock);
                            wfmSampleInfo.SampleIntervalByus = _AllChnlData[readInfo][ch].readParams.SampleIntervalByUs;
                            wfmSampleInfo.StartTimeByus = _AllChnlData[readInfo][ch].readParams.StartTimeByus;
                            wfmSampleInfo.HdMessage = AcquedParameters.HdMessage;
                            return true;
                        }
                    }
                }

            }
            //if (_AllChnlData.ContainsKey(readInfo))
            //{
            //    wfmSampleInfo.SampleIntervalByus = _AllChnlData[readInfo][0].readParams.SampleIntervalByUs;
            //    wfmSampleInfo.StartTimeByus = _AllChnlData[readInfo][0].readParams.StartTimeByus;
            //    wfmSampleInfo.HdMessage = AcquedParameters.HdMessage;
            //    ushort zero = (ushort)((AcquedParameters.HdMessage!.Analog![(int)ch].PositionIndex * Constants.SAMPS_PER_YDIV / Constants.IDX_PER_YDIV) + 8192);//临时修改，保证切换时基档位时，无异常波形
            //    for (int i = 0; i < _AllChnlData[readInfo][0].buff.Count; i++)
            //    {
            //        wfmData.Add(zero);
            //    }

            //    return true;
            //}
            return false;
        }

        AutoPeakFilter filter = new();
        private Boolean ReadAcq(ReadInfo readInfo, CancellationToken? softResetToken)
        {
            var readingParams = CalcReadParamsByWriteParams(Acquisition.CurrDataAcquireAttribute, AcquedParameters.WriteParams, readInfo);
            if (AcquedParameters.WriteParams == null || readingParams == null)
                return false;

            if (readInfo.Mark == "Ai")
            {
                return ReadAiData(readInfo, readingParams);
            }
            else if ((Hd.UIMessage?.AiTable?[ChannelId.C1].AIUnion?.AINoiseReductionEnable ?? false) && (Hd.UIMessage?.AiTable?[ChannelId.C1].AIUnion?.CurNoiseRedutionMethod == NoiseRedutionMethod.AdaptiveFilter))
            { 
                return ReadAutoPeakData(readInfo, readingParams);
            }


            HdCtrl_AnalogDDR.ConfigRead(readingParams, AcquedParameters);

            AdcInterleaveMode adcInterleaveMode = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode;
            Int32 chnlnums = (adcInterleaveMode == AdcInterleaveMode.Mode1To1 ? 4 : 4);

            //2:1个点2个字节;
            int dmaLengthBytes = (int)(readingParams.PerChannelRecvDotsCount * 2 * chnlnums);
            if (dmaLengthBytes <= 0)
            {
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                return false;
            }
            if (_DmaBuff.Length != dmaLengthBytes)
                _DmaBuff = new Byte[dmaLengthBytes];

            if (!HdCtrl_AnalogDDR.ReadDMA((UInt32)dmaLengthBytes, _DmaBuff))
                return false;

            UInt32 chnlbits = (adcInterleaveMode == AdcInterleaveMode.Mode1To1 ? 0xfU : 0x5U);//0xf=0b1111=4通道打开;0x5=0b0101=1,3通道打开;
            AnalogChannelDataSplit(chnlbits, _DmaBuff, (Int32)(readingParams.PerChannelRecvDotsCount), _ChnlData, true);
            //2:1个点2个字节;
            Dictionary<ChannelId, (List<UInt16> buff, ReadParams readParams)> curwfmpkg = new();
            for (Int32 chnlId = 0; chnlId < _ChnlData.Count; chnlId++)
            {
                curwfmpkg[(ChannelId)chnlId] = (_ChnlData[chnlId].Skip((Int32)(HdCtrl_AnalogDDR.DiscardDotCnt + readingParams.PerChannelRecvDotsCount - (readingParams.PerChannelRecvDotsCount))).ToList(), readingParams);
            }
            _AllChnlData[readInfo] = curwfmpkg;
            return true;
        }

        private Boolean ReadAutoPeakData(ReadInfo readInfo, ReadParams readingParams)
        {
            HdCtrl_AnalogDDR.ConfigRead(readingParams, AcquedParameters);

            AdcInterleaveMode adcInterleaveMode = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode;
            Int32 chnlnums = (adcInterleaveMode == AdcInterleaveMode.Mode1To1 ? 4 : 4);

            //2:1个点2个字节;
            int dmaLengthBytes = (int)(readingParams.PerChannelRecvDotsCount / 0.7 * 2 * chnlnums);
            if (dmaLengthBytes <= 0)
            {
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                return false;
            }
            if (_DmaBuff.Length != dmaLengthBytes)
                _DmaBuff = new Byte[dmaLengthBytes];

            if (!HdCtrl_AnalogDDR.ReadDMA((UInt32)dmaLengthBytes, _DmaBuff))
                return false;

            UInt32 chnlbits = (adcInterleaveMode == AdcInterleaveMode.Mode1To1 ? 0xfU : 0x5U);//0xf=0b1111=4通道打开;0x5=0b0101=1,3通道打开;
            AnalogChannelDataSplit(chnlbits, _DmaBuff, (Int32)(readingParams.PerChannelRecvDotsCount / 0.7), _ChnlData, true);
            //2:1个点2个字节;
            Dictionary<ChannelId, (List<UInt16> buff, ReadParams readParams)> curwfmpkg = new();
            for (Int32 chnlId = 0; chnlId < _ChnlData.Count; chnlId++)
            {
                try
                {
                    MWNumericArray bf = new MWNumericArray(_ChnlData[chnlId].Select(o => (double)o).ToArray());
                    MWArray[] res = filter.auto_peak_filter(1, bf, ConstDefine.Ratio_u2f * 4e6 / AcquedParameters.PerDataByfs_AtDdr, 40, 15);
                    MWNumericArray tmp1 = (MWNumericArray)res[0];
                    double[] tmp2 = (double[])tmp1.ToVector(MWArrayComponent.Real);
                    int len = Math.Min(tmp2.Length, _ChnlData[chnlId].Count);
                    List<UInt16> tList = _ChnlData[chnlId];
                    tList.Clear();
                    tList.Capacity = tmp2.Length;
                    for (int i = 0; i < tmp2.Length; i++)
                    {
                        tList.Add((UInt16)(tmp2[i]));
                    }
                    curwfmpkg[(ChannelId)chnlId] = (_ChnlData[chnlId].Skip((Int32)HdCtrl_AnalogDDR.DiscardDotCnt).ToList(), readingParams);
                }
                catch (Exception e)
                {
                    ;
                }

            }
            _AllChnlData[readInfo] = curwfmpkg;
            return true;
        }

        private Boolean ReadAiData(ReadInfo readInfo, ReadParams readingParams) 
        {
            readingParams.PerChannelRecvDotsCount = 3E6;
            readingParams.TotalExtractNum = 1;
            readingParams.Interpolate_Num_Double = 1;

            HdCtrl_AnalogDDR.ConfigRead(readingParams, AcquedParameters);

            AdcInterleaveMode adcInterleaveMode = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode;
            Int32 chnlnums = (adcInterleaveMode == AdcInterleaveMode.Mode1To1 ? 4 : 4);

            //2:1个点2个字节;
            int dmaLengthBytes = (int)(readingParams.PerChannelRecvDotsCount * 2 * chnlnums);
            if (dmaLengthBytes <= 0)
            {
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                return false;
            }
            if (_DmaBuff.Length != dmaLengthBytes)
                _DmaBuff = new Byte[dmaLengthBytes];

            if (!HdCtrl_AnalogDDR.ReadDMA((UInt32)dmaLengthBytes, _DmaBuff))
                return false;

            UInt32 chnlbits = (adcInterleaveMode == AdcInterleaveMode.Mode1To1 ? 0xfU : 0x5U);//0xf=0b1111=4通道打开;0x5=0b0101=1,3通道打开;
            AnalogChannelDataSplit(chnlbits, _DmaBuff, (Int32)(readingParams.PerChannelRecvDotsCount), _ChnlData, true);
            //2:1个点2个字节;
            Dictionary<ChannelId, (List<UInt16> buff, ReadParams readParams)> curwfmpkg = new();
            for (Int32 chnlId = 0; chnlId < _ChnlData.Count; chnlId++)
            {
                curwfmpkg[(ChannelId)chnlId] = (_ChnlData[chnlId].Skip((Int32)(HdCtrl_AnalogDDR.DiscardDotCnt + readingParams.PerChannelRecvDotsCount - (readingParams.PerChannelRecvDotsCount / 0.7))).ToList(), readingParams);
            }
            _AllChnlData[readInfo] = curwfmpkg;
            return true;
        }

        internal virtual UInt64 GetValidExtramNum(Double preExtramNum)
        {
            Double[] step = { 2, 2.5, 2 };
            Double ans = 1.0;
            Int32 index = 0;
            while (ans < preExtramNum)
            {
                ans = ans * step[index % step.Length];
                index++;
            }
            return (UInt64)ans;
        }

        //internal virtual Int32 GetValidInterplotNum(Double interplotNum)
        //{
        //    if (interplotNum <= 1.0)
        //    {
        //        return 1;
        //    }

        //    foreach (var tmp in AnalogChannel_LongStorage.InterplotNumTable)
        //    {
        //        if (interplotNum <= tmp.Key)
        //        {
        //            return tmp.Key;
        //        }
        //    }
        //    return AnalogChannel_LongStorage.InterplotNumTable.Keys.Max();
        //}
        #endregion


        #region 写参数的计算

        /// <summary>
        /// 只能写非writed的半区
        /// </summary>
        /// <returns></returns>
        private WriteParams CalcWrittingParams()
        {
            WriteParams writtingParams = new WriteParams();
            writtingParams.WritedTimestamp = DateTime.Now.Ticks;

            writtingParams.WaveAddrSum = (uint)AcquingParameters.HardwareStorageWaveDotsCnt * 2;//12bit量化字，1个点对应2 bytes;

            writtingParams.SegmentNum = 1;// todo:分段数先写死为1
            HdMessage nowMessage = Hd.UIMessage!;
            (UInt32 Base, UInt32 Multiple) baseMuliple =
                SplitExtractNum(nowMessage.Timebase!.InterleaveMode, nowMessage.Timebase!.AcqMode == AnaChnlAcqMode.Peak, AcquingParameters.ExtractNumFromAdc);

            writtingParams.TotalExtractNum = AcquingParameters.ExtractNumFromAdc;
            writtingParams.ExtractNum_Base = baseMuliple.Base;
            writtingParams.ExtractNum_Multiple = baseMuliple.Multiple;
            writtingParams.SegmentNum = (UInt32)AcquingParameters.HdMessage!.Timebase!.FrameCount;
            writtingParams.SegmentCurrentId = (UInt32)AcquingParameters.HdMessage!.Timebase!.CurFrameId;
            writtingParams.SegmentAcitve = AcquingParameters.HdMessage!.Timebase!.SegmentActive;
            writtingParams.SegmentWorkMode = AcquingParameters.HdMessage!.Timebase!.SegmentWorkMode;
            writtingParams.CallBack = AcquingParameters.HdMessage!.Timebase!.CallBack;
            writtingParams.FrameNo = AcquingParameters.FrameNo;
            return writtingParams;
        }
        #endregion
        #region 读参数的计算

        private ReadParams? CalcReadParamsByWriteParams(AcquireAttribute acquireAttribute, WriteParams? writeParams, ReadInfo? readInfo)
        {
            if (writeParams == null || readInfo == null)
            {
                return null;
            }

            uint UI_ReadDataCount = (uint)readInfo.pkgInfo.DotsCount;
            if (Acquisition.bReadOldData && readInfo.Mark == "View"&& (Acquisition.AcqedDataMsg!=null))
            {
                UI_ReadDataCount = Acquisition.AcqedDataMsg!.Timebase!.InterleaveMode switch
                {
                    AdcInterleaveMode.Mode2To1 => 200_000,
                    _ => 50_000,
                };
            }
            UI_ReadDataCount *= 1;
            return readInfo.Mark == "Zoom"
                ? Hd.CurrProduct!.Acquirer_AnalogChannel!.CalcDdrReadZoomParams(DdrData4What.Dso, acquireAttribute, readInfo.pkgInfo.StartTimeByus, readInfo.pkgInfo.SumTimeByus, UI_ReadDataCount, writeParams?.WritedTimestamp ?? DateTime.Now.Ticks)
                : Hd.CurrProduct!.Acquirer_AnalogChannel!.CalcDdrReadParams(DdrData4What.Dso, acquireAttribute, readInfo.pkgInfo.StartTimeByus, readInfo.pkgInfo.SumTimeByus, UI_ReadDataCount, writeParams?.WritedTimestamp ?? DateTime.Now.Ticks);
        }

        #endregion
    }

    #region 硬件同学只需修改这个区间里的内容
    /// <summary>
    /// 静态类，直接与硬件进行交互，不做任何计算，
    /// </summary>
    internal static class AnalogChannel_LongStorage
    {
        /// <summary>
        /// 该函数只负责配置写过程的参数配置和使能
        /// </summary>
        /// <param name="writeParams"></param>
        internal static void ConfigWrite(WriteParams writeParams)
        {
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_AcqWriteEnable, 0);

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DataPath_SoftReadReq, 0);
            uint WaveAddrSum = writeParams.WaveAddrSum;
            WaveAddrSum = (WaveAddrSum + 128 - 1) / 128 * 128;
            //WaveAddrSum += 8 * 128;
            //WaveAddrSum += 8 * 128;
            //WaveAddrSum += 8 * 128;
            //zy add at scan ,deep must > HardwareStorageWaveDotsCnt, for not display data
            if (Hd.UIMessage!.Timebase!.IsScan)
            {
                WaveAddrSum += WaveAddrSum / 2;
            }
            //end zy add

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDR_DATA_Wr_Addr_Len_L16, AcqBdReg.W.LSCtrl_DDR_DATA_Wr_Addr_Len_H16, WaveAddrSum / 1000);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDR_DATA_Wr_Addr_Len_L16, AcqBdReg.W.LSCtrl_DDR_DATA_Wr_Addr_Len_H16, WaveAddrSum);

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDR_Wr_Seg_Num_L16, AcqBdReg.W.LSCtrl_DDR_Wr_Seg_Num_H16, writeParams.SegmentNum);
            //            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_MultiSegement_Enable, 0);
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_SoftStopAcq, 0U);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_MultiSegement_Enable, 0);//重采时需要先复位
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_MultiSegement_Enable, writeParams.SegmentAcitve);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_MultiSegement_Mode, writeParams.CallBack == true ? 1U : 0);

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapX, AbstractAcquirer_AnalogChannel.PreExtractGapModeList[(uint)writeParams.ExtractNum_Base]);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueL16, (uint)(writeParams.ExtractNum_Multiple & 0xffff));
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueH16, (uint)((writeParams.ExtractNum_Multiple >> 16) & 0xffff));

            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapX, AbstractAcquirer_AnalogChannel.PreExtractGapModeList[(uint)writeParams.ExtractNum_Base]);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueH16, (uint)(writeParams.ExtractNum_Multiple & 0xffff));
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueL16, (uint)((writeParams.ExtractNum_Multiple >> 16) & 0xffff));

            //HdIO.WriteReg(ProcBdReg.W.LSCtrl_DDR_Wr_Seg_Num_L16_Pro, (UInt32)writeParams.SegmentNum & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.LSCtrl_DDR_Wr_Seg_Num_H16_Pro, (UInt32)(writeParams.SegmentNum >> 16) & 0xffff);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_CurrFrameNo, writeParams.FrameNo);


            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.FifoCtrl_OutSpeed, 1);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_Reset, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_Reset, 1);

            //=======================TEST CODE OF LCHY START 

        }

        /// <summary>
        /// 判断PCIe板是否有可读数据，然后直接读取
        /// </summary>
        /// <param name="dataLength">想要读取的数据个数</param>
        /// <param name="dmaBuff">用来缓存数据的buff</param>
        /// <returns>读取成功的数据个数，0-读取失败</returns>
        internal static Boolean ReadData(UInt32 dataLength, Byte[] dmaBuff)
        {
            #region 路径选择
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DataPath_acq_linkmux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkmux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkdemux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            #endregion
            var retVal = HdIO.DMARead(dataLength, ref dmaBuff);
            return retVal;
        }

        /// <summary>
        /// Mig复位
        /// </summary>
        internal static void MigReset()
        {
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_MigReset, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_MigReset, 1);
            //var flag = HdIO.CheckRegisterValue(AcqBdReg.R.LSCtrl_MigInitState, 0x1, 1, 1);
            //if (!flag)
            //{
            //    // throw new Exception("DDR MIG Init Failed!!");
            //}
        }

        internal static Dictionary<UInt32, UInt32> InterplotNumTable = new()
        {
            {2,   0x102},
            {5,   0x105},
            {10,  0x10a},
            {20,  0x20a},
            {40,  0x40a},
            {50,  0x50a},
            {80,  0x80a},
            {100, 0xa0a},
            {200, 0x140a},
        };
    }
    #endregion

    #region 参数类
    public class WriteParams
    {
        public long WritedTimestamp = 0;

        /// <summary>
        /// 写入DDR的单段总地址个数
        /// </summary>
        public UInt32 WaveAddrSum;

        /// <summary>
        /// 写入DDR的段数
        /// </summary>
        public UInt32 SegmentNum;

        /// <summary>
        /// 当前段数
        /// </summary>
        public UInt32 SegmentCurrentId;

        /// <summary>
        /// 分段存储使能
        /// </summary>
        public UInt32 SegmentAcitve;

        /// <summary>
        /// 分段存储模式 1：播放 0：停止播放
        /// </summary>
        public SegmentWorkMode SegmentWorkMode;

        /// <summary>
        /// 回放
        /// </summary>
        public Boolean CallBack;

        /// <summary>
        /// 硬件前抽总数
        /// </summary>
        public UInt64 TotalExtractNum;

        /// <summary>
        /// 硬件前抽基数
        /// </summary>
        public UInt64 ExtractNum_Base;
        /// <summary>
        /// 硬件前抽倍数
        /// </summary>
        public UInt64 ExtractNum_Multiple;

        public ushort FrameNo;
    }


    #endregion

    #region 工具类
    public class DeepCopy
    {
        public static T? CopyByXml<T>(T? source)
        {
            object? result;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xs = new XmlSerializer(typeof(T));
                xs.Serialize(ms, source);
                ms.Seek(0, SeekOrigin.Begin);
                result = xs.Deserialize(ms);
                ms.Close();
                ms.Dispose();
            }
            return (T?)result;
        }
    }
    #endregion
}

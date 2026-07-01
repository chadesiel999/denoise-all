#if !Product_B21_JinHui_PXI
using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System.Threading;

namespace ScopeX.Hardware.Driver
{
    internal class Acquirer_LA_Standard : AbstractAcquirer_LA
    {
        public Acquirer_LA_Standard(Abstract_LongStorage? longStorage)
        {
            ConfigFunc = ourConfig;
        }
        internal override void PowerOn()
        {
            HdIO.WriteReg(ProcBdReg.W.LA_PowerCtrl, 0b001);
            HdIO.WriteReg(PcieBdReg.W.PowerManager_LA_Power, 0b001);
        }
        internal override void PowerOff()
        {
            HdIO.WriteReg(ProcBdReg.W.LA_PowerCtrl, 0b000);
            HdIO.WriteReg(PcieBdReg.W.PowerManager_LA_Power, 0b000);

        }
        internal override void CreateAcquireAttribute()
        {
            //AcquingParameters.AcqStorageMode = AnaChnlStorageMode.Long;
            //AcquingParameters.PerDataByfs_AtDdr = 1_000_000;
            //todo 先借用模拟通道的配置
            Hd.CurrProduct.Acquirer_AnalogChannel?.AcquingParameters.CloneTo(AcquingParameters);
        }
        internal override void InitAcq()
        {
            //LA DDR存储深度（现有，LA专用）
            uint ddrlength = (uint)(Math.Round((AcquingParameters.HardwareStorageWaveDotsCnt / 128.0), 0, MidpointRounding.ToPositiveInfinity)) * 128 + 128 * 8;
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_LA_DDR_DATA_Wr_Addr_Len_H16, (ddrlength >> 16) & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_LA_DDR_DATA_Wr_Addr_Len_L16, ddrlength & 0xffff);

            //LA前抽倍率（新增LA专用）
            (UInt32 Base, UInt32 Multiple) extractnum = Extract_JiHe_MSO8000X.GetPreSeperateNum((UInt64)AcquingParameters.ExtractNumFromAdc);
            HdIO.WriteReg(ProcBdReg.W.LA_Decimation_PreGapX, extractnum.Base);
            HdIO.WriteReg(ProcBdReg.W.LA_Decimation_PreGapValueL16, (uint)(extractnum.Multiple & 0xffff));
            HdIO.WriteReg(ProcBdReg.W.LA_Decimation_PreGapValueH16, (uint)((extractnum.Multiple >> 16) & 0xffff));
        }
        internal override void PostProcess(List<ReadInfo> readInfoList, CancellationToken? softResetToken)
        {}

        internal override bool ReadAcqData(List<ReadInfo> readInfoList, out double samplingRateByus, CancellationToken? softResetToken)
        {
            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen || Hd.BPowerOff)
            {
                AcqedDataPool.LAData.Data.Clear();
                samplingRateByus = 1.0;
                foreach(var rInfo in readInfoList)
                {
                    for (int i = 0; i < rInfo.pkgInfo.DotsCount; i++)
                    {
                        AcqedDataPool.LAData.Data.Add((ushort)(i));
                    }
                }
                return true;
            }

            //todo 先借用模拟通道的配置
            Hd.CurrProduct.Acquirer_AnalogChannel?.AcquedParameters.CloneTo(AcquedParameters);

            //不应该直接调用此函数
            samplingRateByus = 1.0;

            foreach (var readInfo in readInfoList)
            {
                //读取LA的数据
                if (!AcquingParameters.bIsLongStorageMode)
                    ReadAcqFifo(readInfo, softResetToken);
                else
                    ReadAcq(readInfo, softResetToken);
            }

            return true;
        }

        ReadParams? _LastReadParams;
        private Boolean ReadAcq(ReadInfo readInfo, CancellationToken? softResetToken)
        {
            ReadParams? readparams = CalcReadParamsByWriteParams(AcquedParameters, readInfo);
            if (readparams == null)
                return false;
            if (Acquisition.bReadOldData && _LastReadParams != null && readparams.IsEqual(_LastReadParams))
            {
                //需要的波形已经在缓存区，直接返回true
                return true;
            }

            /*配置并读取波形数据*/
            ConfigRead(readparams);

            UInt32 ladatabytes = (UInt32)(readparams.PerChannelRecvDotsCount * 2);//数据量匹配10G模拟通道
            Byte[] dmabuff = new Byte[ladatabytes];
            if (!ReadDMA(ladatabytes, dmabuff))
                return false;

            AcqedDataPool.LAData.Data.Clear();
            for (UInt32 pindex = 0; pindex < dmabuff.Length / 2; pindex++)
            {
                Byte highbyte = ReverseByte(dmabuff[pindex * 2]);
                Byte lowbyte = ReverseByte(dmabuff[pindex * 2 + 1]);
                AcqedDataPool.LAData.Data.Add((ushort)((highbyte << 8) | lowbyte));
            }
            _LastReadParams = readparams;

            AcqedDataPool.LAData.WfmSampleInfo.StartTimeByus = readparams.StartTimeByus;
            AcqedDataPool.LAData.WfmSampleInfo.SampleIntervalByus = readparams.SampleIntervalByUs;
            Hd.CurrProduct.Acquirer_LA!.bDataVaild = true;
            return true;
        }

        /// <summary>
        /// Scan档从Fifo读数
        /// </summary>
        /// <param name="readInfo"></param>
        /// <param name="softResetToken"></param>
        /// <returns></returns>
        private Boolean ReadAcqFifo(ReadInfo readInfo, CancellationToken? softResetToken)
        {
            ReadParams? readparams = CalcReadParamsByWriteParams(AcquedParameters, readInfo);
            if (readparams == null)
                return false;

            HdIO.WriteReg(ProcBdReg.W.LA_ScanFifoRdDataCount, (UInt32)(readparams.PerChannelRecvDotsCount / 32));
            UInt32 ladatabytes = (UInt32)(readparams.PerChannelRecvDotsCount * 2);//数据量匹配10G模拟通道
            Byte[] dmabuff = new Byte[ladatabytes];
            if (!ReadDMA(ladatabytes, dmabuff))
                return false;

            AcqedDataPool.LAData.Data.Clear();
            for (UInt32 pindex = 0; pindex < dmabuff.Length / 2; pindex++)
            {
                Byte highbyte = ReverseByte(dmabuff[pindex * 2]);
                Byte lowbyte = ReverseByte(dmabuff[pindex * 2 + 1]);
                AcqedDataPool.LAData.Data.Add((ushort)((highbyte << 8) | lowbyte));
            }
            _LastReadParams = readparams;

            AcqedDataPool.LAData.WfmSampleInfo.StartTimeByus = readparams.StartTimeByus;
            AcqedDataPool.LAData.WfmSampleInfo.SampleIntervalByus = readparams.SampleIntervalByUs;
            Hd.CurrProduct.Acquirer_LA!.bDataVaild = true;
            return true;
        }

        /// <summary>
        /// 配置LA读取之前的设置
        /// </summary>
        /// <param name="readParams"></param>
        private void ConfigRead(ReadParams readParams)
        {
            //DDR初始读地址偏移量（现有，LA专用）
            var define = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;
            uint offset = (uint)(readParams.DdrReadStartDotPosition / (define.InterleaveMode == AdcInterleaveMode.Mode1To1 ? 1 : 2));
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_LA_Offset_L, offset & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_LA_Offset_H, offset >> 16);

            //LA后抽倍率（新增LA专用）
            (UInt32 Base, UInt32 Multiple) baseMuliple = Extract_JiHe_MSO8000X.GetPreSeperateNum((UInt64)readParams.TotalExtractNum);
            HdIO.WriteReg(ProcBdReg.W.LA_Decimation_PosGapx, (uint)baseMuliple.Base);
            HdIO.WriteReg(ProcBdReg.W.LA_Decimation_PosGapValueL16, (UInt32)(baseMuliple.Multiple & 0xffff));
            HdIO.WriteReg(ProcBdReg.W.LA_Decimation_PosGapValueH16, (UInt32)((baseMuliple.Multiple >> 16) & 0xffff));

            //LA插值倍率（新增LA专用）
            HdIO.WriteReg(ProcBdReg.W.LA_InterpolateEn, Hd.CurrDebugVarints.bEnable_InterBoardSynchronizationMode ? 1U : 0);
            HdIO.WriteReg(ProcBdReg.W.LA_InterpolateRatio, (uint)readParams.Interpolate_Num_Double);
            HdIO.WriteReg(ProcBdReg.W.LA_InterpolateRemainderNum, (uint)readParams.Interpolate_DiscardDotNum);
        }

        /// <summary>
        /// 计算读LA数据之前的配置 
        /// </summary>
        /// <param name="acquireAttribute"></param>
        /// <param name="writeParams"></param>
        /// <param name="readInfo"></param>
        /// <returns></returns>
        private ReadParams? CalcReadParamsByWriteParams(AcquireAttribute acquireAttribute,  ReadInfo? readInfo)
        {
            WriteParams? writeparams = CalcWrittingParams(acquireAttribute);
            uint uireaddatacount = (uint)readInfo.pkgInfo.DotsCount;
            return Hd.CurrProduct!.Acquirer_AnalogChannel!.CalcDdrReadParams(DdrData4What.LA, acquireAttribute, readInfo.pkgInfo.StartTimeByus, readInfo.pkgInfo.SumTimeByus, uireaddatacount, writeparams?.WritedTimestamp ?? DateTime.Now.Ticks);
        }

        /// <summary>
        /// 计算读取参数
        /// </summary>
        /// <returns></returns>
        private WriteParams CalcWrittingParams(AcquireAttribute acquireAttribute)
        {
            WriteParams writtingparams = new WriteParams();
            return writtingparams;
        }

        /// <summary>
        /// 通过DMA读取数据
        /// </summary>
        /// <param name="dataLength"></param>
        /// <param name="dmaBuff"></param>
        /// <returns></returns>
        private Boolean ReadDMA(UInt32 dataLength, Byte[] dmaBuff)
        {
            Hd.CurrProduct!.AcqBd!.SwitchDataPathAndPcieReset_SetDataLength(DMAReadDataTypes.LADdr, dataLength);

            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);

            var retval = HdIO.DMARead(dataLength, ref dmaBuff);
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
            return retval;
        }

        /// <summary>
        /// 把字节数据的Bit调整为逆序
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private Byte ReverseByte(Byte data)
        {
            Byte ret = 0;
            List<Byte> bits = new List<Byte>();
            for (int bitid = 0; bitid < sizeof(Byte) * 8; bitid++)
            {
                bits.Add((Byte)((data >> bitid) % 2));
            }
            bits.Reverse();
            for (int bitId = 0; bitId < sizeof(Byte) * 8; bitId++)
            {
                ret |= (Byte)(bits[bitId] << bitId);
            }
            return ret;
        }

        private void ourConfig()
        {
            SetComparisonLevel();
            SetHysteresisLevel();
            var laisopen = Hd.UIMessage?.Digital?.Any(d => d.Active) ?? false;
            HdIO.WriteReg(ProcBdReg.W.LA_ModeEn, laisopen ? 0x1U : 0x0U);
        }
    }
}
#endif
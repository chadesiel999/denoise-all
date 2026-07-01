using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 模拟通道采集器的深存储部分，提供深存储功能，可不参与编译
    /// </summary>
    public abstract partial class AbstractAcquirer_AnalogChannel : AbstractAcquirer
    {
        private Boolean FifoReadAcq(List<ReadInfo> readInfoList, CancellationToken? softResetToken)
        {
            foreach (var readInfo in readInfoList)
            {
                var readingparam = CalcReadParamsByWriteParams(AcquedParameters, AcquedParameters.WriteParams, readInfo);
                FifoReadMethod(readInfo, readingparam);
            }
            Monitor.Enter(AcqedDataPool.UpdateDataLock);
            for (Int32 chnlid = 0; chnlid < _ChnlData.Count; chnlid++)
            {
                if (_AllChnlData.ContainsKey(readInfoList[0]) && _AllChnlData[readInfoList[0]].ContainsKey((ChannelId)chnlid))
                {
                    AcqedDataPool.AnalogChData.AllChannelData[chnlid].Clear();
                    AcqedDataPool.AnalogChData.AllChannelData[chnlid].AddRange(_AllChnlData[readInfoList[0]][(ChannelId)chnlid].buff);
                }
            }
            Monitor.Exit(AcqedDataPool.UpdateDataLock);
            return true;
        }

        private void FifoInitAcq()
        {
            HdCtrl_AnalogFifo.ConfigWrite();
            AcquingParameters.WriteParams = CalcWrittingParams();
            HdCtrl_AnalogDDR.ConfigWrite(AcquingParameters.WriteParams, AcquingParameters);
        }

        private Boolean FifoReadMethod(ReadInfo readInfo, ReadParams? readingParams)
        {
            if (readingParams == null)
                return false;

            int perchnldotscount = (int)((readingParams.PerChannelRecvDotsCount)-1)*4*2;
            int dmalengthbytes = perchnldotscount/* * 4 * 2*/;//4(chnls);2(bytes)=1(pt);
            if (_DmaBuff.Length != dmalengthbytes)
                _DmaBuff = new Byte[dmalengthbytes];

            HdCtrl_AnalogFifo.ConfigRead(readingParams);

            //HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, (UInt32)perchnldotscount * 8);//8=8bit
            //HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset,1);//8=8bit
            //HdIO.WriteReg(PcieBdReg.W.Xdma_XdmaIrqReset,0);//8=8bit
              //reg_debug_single_mode
            var readok = HdCtrl_AnalogDDR.ReadDMA((UInt32)dmalengthbytes,_DmaBuff);
            //Debug.WriteLine($"{DateTime.Now.ToString("ss.fff")} : PerChannelRecvDotsCount:{readingParams.PerChannelRecvDotsCount};" +
            //    $"perChnlDotsCount: {perChnlDotsCount} ; readOk: {readOk}");
            HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve10, 0x0);    //reg_debug_single_mode
            if (!readok)
                return false;

            //分配Dma数据到通道
            AnalogChannelDataSplit(0xf, _DmaBuff, perchnldotscount, _ChnlData, true);
            Dictionary<ChannelId, (List<UInt16> buff, ReadParams readParams)> curWfmPkg = new();
            for (Int32 chnlid = 0; chnlid < _ChnlData.Count; chnlid++)
            {
                curWfmPkg[(ChannelId)chnlid] = (_ChnlData[chnlid].ToList(), readingParams);
            }
            _AllChnlData[readInfo] = curWfmPkg;
            return true;
        }

        protected virtual Int32 GetInterpolateValideNum(Int32 originInterpolate)
        {
            return originInterpolate;
        }

        protected virtual UInt32 GetInterpolateValideValue(Int32 num)
        {
            return 0;
        }
    }
}

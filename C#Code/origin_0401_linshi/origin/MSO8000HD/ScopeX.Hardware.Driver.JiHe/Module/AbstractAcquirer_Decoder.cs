using System;
using System.Collections.Generic;
using System.Text;
using ScopeX.ComModel;
using System.Reflection;
using System.Threading;

namespace ScopeX.Hardware.Driver
{
    public partial class AbstractAcquirer_Decoder : AbstractAcquirer
    {
        protected static Action? ConfigFunc = null;
        internal static void Config() { ConfigFunc?.Invoke(); }

        internal override void Init()
        {

        }
        internal override void InitAcq()
        {

        }
        internal override void PostProcess(List<ReadInfo> readInfoList, CancellationToken? softResetToken)
        {

        }
        //RdReg_Trig 还需要不？如果需要，需要FPGA补全地址并进行维护 2022.07.13 zy
        internal static (ProcBdReg.R RdReg_First, ProcBdReg.R RdReg_Last, ProcBdReg.R RdReg_Trig, ProcBdReg.R RdReg_DataIsReady)[] readAddrTable =
        {
            (RdReg_First:ProcBdReg.R.SysInfo_WorkOKTest,RdReg_Last:ProcBdReg.R.SysInfo_WorkOKTest,RdReg_Trig:ProcBdReg.R.SysInfo_WorkOKTest,RdReg_DataIsReady:ProcBdReg.R.BoardSync_Scan_Finish),
            (RdReg_First:ProcBdReg.R.SysInfo_WorkOKTest,RdReg_Last:ProcBdReg.R.SysInfo_WorkOKTest,RdReg_Trig:ProcBdReg.R.SysInfo_WorkOKTest,RdReg_DataIsReady:ProcBdReg.R.BoardSync_Scan_Finish),
        };
        //internal override bool ReadAcqData(List<ReadInfo> readInfoList, out double samplingRateByus, CancellationToken? softResetToken)
        //{
        //    samplingRateByus = 1.0;
        //    if (Acquisition.AcqedDataMsg!.Timebase!.IsScan)
        //        return false;
        //    //请参考DecodeReadFifoByDma
        //    #region 检查是否数据准备好
        //    for (int channleIndex = 0; channleIndex < ChannelIdExt.BusChnlNum; channleIndex++)
        //    {
        //        if ((HdIO.ReadReg(readAddrTable[channleIndex].RdReg_DataIsReady) & 0x01) == 0)
        //            return false;
        //    }
        //    #endregion
        //    //HdIO.WdReg(ProcBdReg.W.Decode_RAM_WriteEnable, 0);
        //    //HdIO.WdReg(ProcBdReg.W.Decode_RAM_ReadEnable, 1);


        //    #region 读3地址

        //    #endregion 读3地址

        //    #region DMA读数
        //    int totalBytes = ChannelIdExt.BusChnlNum * PerChannelDataSize * sizeof(UInt16);
        //    byte[] recvData = new byte[totalBytes];
        //    HdIO.WriteReg(PcieBdReg.W.ReadFromAcqOrDpo, 1);
        //    #endregion DMA读数

        //    bDataVaild = false;
        //    return false;
        //}
    }
}

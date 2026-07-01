using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal partial class CtrlDecoder
    {
        internal static Int32 Config_Standard_CAN_FD()
        {
            //HdMessage.ProtocolCANFDOptions? decodeOption = null;
            //HdMessage.TrigCANFDConditionsOptions? trigOption = null;
            //UInt32 decodeChID = 0;
            //for (UInt32 chid = 0; chid < ChannelIdExt.BusChnlNum; chid++)
            //{
            //    if (Hd.UIMessage!.Decoder![chid].ProtocolType == SerialProtocolType.CAN_FD && Hd.UIMessage!.Decoder![chid].Active)
            //    {
            //        decodeOption = Hd.UIMessage!.Decoder![chid].ProtocolOptions! as HdMessage.ProtocolCANFDOptions;
            //        decodeChID = chid + 1;
            //    }
            //}
            //if (Hd.UIMessage!.Trigger!.TrigDecoder!.ProtocolType == SerialProtocolType.CAN_FD)
            //{
            //    trigOption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigCANFDConditionsOptions;
            //    decodeOption = Hd.UIMessage!.Trigger!.TrigDecoder!.ProtocolOptions! as HdMessage.ProtocolCANFDOptions;
            //    if (Hd.UIMessage!.Decoder![0].ProtocolType == SerialProtocolType.CAN_FD && Hd.UIMessage!.Decoder![0].Active)
            //    {
            //        decodeOption = Hd.UIMessage!.Decoder![0].ProtocolOptions! as HdMessage.ProtocolCANFDOptions;
            //    }
            //    decodeChID = 1;
            //    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, data: 20);
            //}

            UInt32 decodeChID = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);

            HdMessage.ProtocolCANFDOptions? decodeOption = Hd.UIMessage!.Decoder![decodeChID].ProtocolOptions! as HdMessage.ProtocolCANFDOptions;

            HdMessage.TrigCANFDConditionsOptions? trigOption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigCANFDConditionsOptions;

           // Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 20);

            if (decodeOption == null || trigOption == null)
                return -1;
            //解码RAM预触发深度12bit
            UInt32 predepth = (UInt32)((Double)((ulong)Hd.AnalogChannel!.AcquingParameters.SettingTrigPositionByfs / Hd.AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr) / (Constants.VIS_XDIVS_NUM * 1000) * 4096);
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth, predepth);
            ProtocolCANFD.SignalType signalType = decodeOption!.SignalType;

            #region 通道选择
            ChannelId CAN_FD_H = decodeOption.SignalInput1;
            if (CAN_FD_H.IsAnalog())//模拟通道
            {; }
            else if (CAN_FD_H.IsDigital())//数字通道
                CAN_FD_H -= 31;
            else
                CAN_FD_H = 0;
            ChannelId CAN_FD_L = decodeOption.SignalInput2;
            if (CAN_FD_L.IsAnalog())//模拟通道
            {; }
            else if (CAN_FD_L.IsDigital())//数字通道
                CAN_FD_L -= 31;
            else
                CAN_FD_L = 0;
            UInt64 sourceControlword = (UInt32)CAN_FD_H << 0;// | (UInt32)CAN_FD_L << 6;

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect1Pro, (UInt32)CAN_FD_H);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect2Pro, (UInt32)CAN_FD_L);
           // HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect3Pro, (UInt32)mosi_input);
            #endregion

            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigTypeSelect, (UInt32)SerialProtocolType.CAN_FD);//触发源选择

            #region 触发参数设置
            ////set参数
            UInt32 byteCount = trigOption == null ? 0 : (UInt32)trigOption.ByteCount;//(UInt32)BitHelper.GetByteCount(trigOption.Data);//触发字节数
            UInt32 trigCondition = trigOption == null ? 0 : (UInt32)trigOption.Condition;//触发模式
            UInt32 frameType = trigOption == null ? 0 : (UInt32)trigOption.FrameType;//触发帧类型
            UInt32 canFdType = decodeOption == null ? 0 : (UInt32)(decodeOption.SignalType == ProtocolCANFD.SignalType.CAN_FDL ? 0 : 1);//信号类型,fd类型
            UInt32 idType = trigOption == null ? 0 : (UInt32)trigOption.IDStandard;//ID类型,定义帧类型
            //UInt32 idType = 2;
            //UInt64 bps = decodeOption == null ? 0 : ((UInt32)312500000/(UInt32)decodeOption.SDSignalRate)<<10;//bps信号速率
            UInt32 dataOffset = trigOption == null ? 0u : (UInt32)trigOption.DataOffset;
            UInt32 dataOffsetEnable = trigOption == null ? 0u : (trigOption.DataOffsetEnabled ? 1u : 0u);
            UInt32 errorType = trigOption == null ? 0 : (UInt32)trigOption.ErrorType;

            trigCondition = trigCondition == 6 ? errorType + 6 : trigCondition;

            UInt64 trigControlWord = 0;
            trigControlWord |= idType << 0;//1-3
            trigControlWord |= canFdType << 3;//4-5
            trigControlWord |= frameType << 4;//5-6
            trigControlWord |= trigCondition << 6;//7-10
            trigControlWord |= byteCount << 10;//11-17
            trigControlWord |= dataOffset << 17;//17-23
            trigControlWord |= dataOffsetEnable << 23;//24

            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigControlWord & 0xFFFF));
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigControlWord >> 16) & 0xFFFF));//发送set[31:17]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigControlWord >> 32) & 0xFFFF));//发送set[47:32]

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigControlWord & 0xFFFF));
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigControlWord >> 16) & 0xFFFF));//发送set[31:17]
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigControlWord >> 32) & 0xFFFF));//发送set[45:32]
            #endregion

            #region 触发数据设置
            ////触发条件选择“数据”触发时使用的data
            UInt32 dataBytesLength = 32;
            UInt16[] TrigDataL = new UInt16[dataBytesLength];

            //UInt32 standardId = trigOption == null ? 0 : (UInt32)trigOption.StandardID;//标准id
            //UInt32 standardIdMask = (UInt32)BitHelper.GetBitsMask((Int64)standardId);// trigOption == null ? 0 : (UInt32)trigOption.Data;
            //UInt32 extendId = trigOption == null ? 0 : (UInt32)trigOption.ExtendedID;//扩展id
            //UInt32 extendIdMask = (UInt32)BitHelper.GetBitsMask((Int64)extendId);// trigOption == null ? 0 : (UInt32)trigOption.Data;
            UInt32 bps0 = decodeOption == null ? 0 : (UInt32)(((UInt32)312500000 / (Double)decodeOption.SDSignalRate) * 1024);//can bps信号速率
            UInt32 bps1 = decodeOption == null ? 0 : (UInt32)(((UInt32)312500000 / (Double)decodeOption.FDSignalRate) * 1024);//can fd bps信号速率

            UInt32 brs0 = (UInt32)((312500000 / (Double)decodeOption.SDSignalRate * (decodeOption.SamplePoint / 100f) + 312500000 / (Double)decodeOption.FDSignalRate * (1 - decodeOption.DataSamplePoint / 100f)));
            UInt32 brs1 = (UInt32)((312500000 / (Double)decodeOption.SDSignalRate * (1 - decodeOption.DataSamplePoint / 100f) + 312500000 / (Double)decodeOption.FDSignalRate * (decodeOption.SamplePoint / 100f)));
            //UInt32 brs0 = 484;
            //UInt32 brs1 = 203;
            //Int64 data = trigOption == null ? 0 : (Int64)trigOption.Data;//数据比较值
            //Int64 dateMask = (Int64)BitHelper.GetBitsMask(data);

            UInt32 standardId = trigOption == null ? 0 : (UInt32)trigOption.StandardID;
            UInt32 standardIdMask = (UInt32)BitHelper.GetBitsMaskByHex((Int64)standardId);
            UInt32 extendId = trigOption == null ? 0 : (UInt32)trigOption.ExtendedID;
            UInt32 extendIdMask = (UInt32)BitHelper.GetBitsMaskByHex((Int64)extendId);
            UInt64 data = trigOption == null ? 0 : trigOption.Data;
            UInt64 dateMask = BitHelper.GetBitsMaskByHex((Int64)data);
            if (trigOption != null)
            {
                #region ID0的128位
                // =============   ID0的128位   ==============
                // [0~63]
                UInt64 id0 = 0;
                id0 |= standardId << 0;
                id0 |= standardIdMask << 11;
                id0 |= ((UInt64)brs0) << 40;
                TrigDataL[0] = (UInt16)(id0 & 0xFFFF);
                TrigDataL[1] = (UInt16)((id0 >> 16) & 0xFFFF);
                TrigDataL[2] = (UInt16)((id0 >> 32) & 0xFFFF);
                TrigDataL[3] = (UInt16)((id0 >> 48) & 0xFFFF);

                //[64 ~ 127]
                UInt64 id0_H = bps0;
                TrigDataL[4] = (UInt16)(id0_H & 0xFFFF);
                TrigDataL[5] = (UInt16)((id0_H >> 16) & 0xFFFF);
                TrigDataL[6] = (UInt16)((id0_H >> 32) & 0xFFFF);
                TrigDataL[7] = (UInt16)((id0_H >> 48) & 0xFFFF);

                // =============   ID0的128位   ============== 
                #endregion

                #region ID1的128位
                // =============   ID1的128位   ==============
                UInt64 id1 = 0;
                id1 |= extendId << 0;
                id1 |= extendIdMask << 18;
                id1 |= ((UInt64)brs1) << 40;
                TrigDataL[8] = (UInt16)(id1 & 0xFFFF);
                TrigDataL[9] = (UInt16)((id1 >> 16) & 0xFFFF);
                TrigDataL[10] = (UInt16)((id1 >> 32) & 0xFFFF);
                TrigDataL[11] = (UInt16)((id1 >> 48) & 0xFFFF);

                UInt64 id1_H = bps1;
                TrigDataL[12] = (UInt16)(id1_H & 0xFFFF);
                TrigDataL[13] = (UInt16)((id1_H >> 16) & 0xFFFF);
                TrigDataL[14] = (UInt16)((id1_H >> 32) & 0xFFFF);
                TrigDataL[15] = (UInt16)((id1_H >> 48) & 0xFFFF);

                // =============   ID1的128位   ============== 
                #endregion

                #region 数据的128位

                UInt64 datasd = (UInt64)data;
                TrigDataL[16] = (UInt16)(datasd & 0xFFFF);
                TrigDataL[17] = (UInt16)((datasd >> 16) & 0xFFFF);
                TrigDataL[18] = (UInt16)((datasd >> 32) & 0xFFFF);
                TrigDataL[19] = (UInt16)((datasd >> 48) & 0xFFFF);

                UInt64 datasd_H = 0;
                TrigDataL[20] = (UInt16)(datasd_H & 0xFFFF);
                TrigDataL[21] = (UInt16)((datasd_H >> 16) & 0xFFFF);
                TrigDataL[22] = (UInt16)((datasd_H >> 32) & 0xFFFF);
                TrigDataL[23] = (UInt16)((datasd_H >> 48) & 0xFFFF);

                #endregion

                #region 数据掩码的128
                UInt64 datamasksd = (UInt64)dateMask;
                TrigDataL[24] = (UInt16)(datamasksd & 0xFFFF);
                TrigDataL[25] = (UInt16)((datamasksd >> 16) & 0xFFFF);
                TrigDataL[26] = (UInt16)((datamasksd >> 32) & 0xFFFF);
                TrigDataL[27] = (UInt16)((datamasksd >> 48) & 0xFFFF);

                UInt64 datamasksd_H = 0xFFFFFFFFFFFFFFFF;
                TrigDataL[28] = (UInt16)(datamasksd_H & 0xFFFF);
                TrigDataL[29] = (UInt16)((datamasksd_H >> 16) & 0xFFFF);
                TrigDataL[30] = (UInt16)((datamasksd_H >> 32) & 0xFFFF);
                TrigDataL[31] = (UInt16)((datamasksd_H >> 48) & 0xFFFF);

                #endregion
            }

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 0);

            for (UInt32 dataIndex = 0; dataIndex < dataBytesLength; dataIndex++)
            {
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataL, (uint)TrigDataL[dataIndex] & 0xffff);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLIndex, dataIndex);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x01);//拉高数据触发的数据使能
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x00);//拉高数据触发的数据使能
            }

            #endregion

            return (Int32)decodeChID;
        }
    }
}
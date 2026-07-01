#if !Product_B21_JinHui_PXI
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
        internal static Int32 Config_Standard_CAN()
        {
            //HdMessage.ProtocolCANOptions? decodeOption = null;
            //HdMessage.TrigCANConditionsOptions? trigOption = null;
            //UInt32 decodeChID = 1;//不打开解码时触发参数分配到解码通道一
            //for (UInt32 chid = 0; chid < ChannelIdExt.BusChnlNum; chid++)
            //{
            //    if (Hd.UIMessage!.Decoder![chid].ProtocolType == SerialProtocolType.CAN && Hd.UIMessage!.Decoder![chid].Active)
            //    {
            //        decodeOption = Hd.UIMessage!.Decoder![chid].ProtocolOptions! as HdMessage.ProtocolCANOptions;
            //        decodeChID = chid + 1;
            //    }
            //}
            //if (Hd.UIMessage!.Trigger!.TrigDecoder!.ProtocolType == SerialProtocolType.CAN)
            //{
            //    trigOption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigCANConditionsOptions;
            //    decodeOption = Hd.UIMessage!.Trigger!.TrigDecoder!.ProtocolOptions! as HdMessage.ProtocolCANOptions;
            //    if (Hd.UIMessage!.Decoder![0].ProtocolType == SerialProtocolType.CAN && Hd.UIMessage!.Decoder![0].Active)
            //    {
            //        decodeOption = Hd.UIMessage!.Decoder![0].ProtocolOptions! as HdMessage.ProtocolCANOptions;
            //    }
            //    decodeChID = 1;
            //    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq,data: 15);
            //}

            UInt32 decodeChID = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);

            HdMessage.ProtocolCANOptions? decodeOption = Hd.UIMessage!.Decoder![decodeChID].ProtocolOptions! as HdMessage.ProtocolCANOptions;

            HdMessage.TrigCANConditionsOptions? trigOption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigCANConditionsOptions;

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 15);

            if (decodeOption == null || trigOption == null)
                return -1;
            //解码RAM预触发深度12bit
            UInt32 predepth = (UInt32)((Double)((UInt64)Hd.AnalogChannel!.AcquingParameters.SettingTrigPositionByfs / Hd.AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr) / (Constants.VIS_XDIVS_NUM * 1000) * 2048);
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth, predepth);
            //HdIO.WriteReg(ProcBdReg.W.Decoder_TrigTypeSelect, (UInt32)SerialProtocolType.CAN);
            ProtocolCAN.SignalType signalType = decodeOption!.SignalType;

            #region 通道选择
            ChannelId signalInput1 = decodeOption.SignalInput1;
            if (signalInput1.IsAnalog())//模拟通道
            {; }
            else if (signalInput1.IsDigital())//数字通道
                signalInput1 -= 31;
            else
                signalInput1 = 0;
            /*
            ChannelId signalInput2 = decodeOption.SignalInput2;
            if (signalInput2.IsAnalog())//模拟通道
            {; }
            else if (signalInput2.IsDigital())//数字通道
                signalInput2 -= 31;
            else
                signalInput2 = 0;
            */

            UInt64 sourceControlword = (UInt32)signalInput1 << 0/* | (UInt32)CAN_L << 6*/;
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.CAN);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceL, (UInt32)sourceControlword);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceM, (UInt32)(sourceControlword >> 16));
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceH, 0x00);

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect1Pro, (UInt32)signalInput1);


            #endregion

            #region 触发参数设置
            //set参数
            UInt64 bps = decodeOption == null ? 0 : ((UInt32)(((Decimal)312500000 / (Decimal)decodeOption.SignalRate) * 1024));//bps信号速率
            //UInt64 bps = decodeOption == null ? 0 : (UInt32)1280000;//bps信号速率 需要左移10位
            UInt32 idType = trigOption == null ? 0 : (UInt32)trigOption.IDStandard;//ID类型
            UInt32 idFrameDirection = trigOption == null ? 0 : (UInt32)trigOption.IDFrameDirection;//定义帧方向: 写、读、任意
            UInt32 trigCondition = trigOption == null ? 0 : (UInt32)trigOption.Condition;//触发模式
            UInt32 byteCount = trigOption == null ? 0 : (UInt32)trigOption.ByteCount;//触发字节数
            //UInt32 byteCount = trigOption == null ? 0 : (UInt32)1;//触发字节数
            UInt32 frameType = trigOption == null ? 0 : (UInt32)trigOption.FrameType;//触发帧类型
            UInt32 canType = decodeOption == null ? 0 : (UInt32)decodeOption.SignalType;//信号类型
            UInt32 errorType = trigOption == null ? 0 : (UInt32)trigOption.ErrorPacketType + 6;
            trigCondition = trigCondition == 6 ? errorType : trigCondition;

            UInt64 trigControlWord = 0;
            trigControlWord |= canType << 0;//0-1
            trigControlWord |= frameType << 1;//2-3
            trigControlWord |= byteCount << 3;//4-7
            trigControlWord |= trigCondition << 7;//8-10
            trigControlWord |= idFrameDirection << 11;//10-11
            trigControlWord |= idType << 13;//13
            trigControlWord |= bps << 16;//17-48


            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigControlWord & 0xFFFF));
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigControlWord >> 16) & 0xFFFF));//发送set[31:17]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigControlWord >> 32) & 0xFFFF));//发送set[47:32]
            //HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWord64, (UInt32)((trigControlWord >> 48) & 0xFFFF));//发送set[64:48]  

            #endregion
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigTypeSelect, (UInt32)SerialProtocolType.CAN);//触发源选择

            #region 触发数据设置
            //触发条件选择“数据”触发时使用的data
            UInt32 dataBytesLength = 32;//数据使能拉高->拉低一次为一接收周期，无论使用多少字节数据做触发，FPGA需要触发数据发16个周期
            UInt16[] TrigDataL = new UInt16[dataBytesLength];//数据L


            UInt32 standardIdData = trigOption == null ? 0 : (UInt32)trigOption.StandardID;
            UInt64 standardIdMask = idType == 0 ? BitHelper.GetBitsMaskByHex((Int64)standardIdData) : 0X_FF_FF_FF_FF_FF_FF_FF_FF;
            UInt32 extendIdData = trigOption == null ? 0 : (UInt32)trigOption.ExtendedID;
            UInt64 extendIdMask = idType == 1 ? BitHelper.GetBitsMaskByHex((Int64)extendIdData) : 0X_FF_FF_FF_FF_FF_FF_FF_FF;
            UInt64 data = trigOption == null ? 0 : trigOption.Data;
            UInt64 dateMask = BitHelper.GetBitsMaskByHex((Int64)data);


            if (trigOption != null)
            {
                //低128位用于传数据触发比较值
                TrigDataL[0] = (UInt16)((standardIdData | (extendIdData << 11)) & 0xffff);
                TrigDataL[1] = (UInt16)((extendIdData >> 5) & 0xffff);
                TrigDataL[2] = 0;
                TrigDataL[3] = 0;
                // TrigDataL[4] = (UInt16)((data>> 48) & 0xffff);//Data MSB
                // TrigDataL[5] = (UInt16)((data>> 32) & 0xffff);
                // TrigDataL[6] = (UInt16)((data>> 16) & 0xffff);
                // TrigDataL[7] = (UInt16)((data<<8 ) & 0xffff);//(UInt16)((data) & 0xffff); ?Data Lsb
                TrigDataL[7] = (UInt16)((data >> 48) & 0xffff);//Data MSB
                TrigDataL[6] = (UInt16)((data >> 32) & 0xffff);
                TrigDataL[5] = (UInt16)((data >> 16) & 0xffff);
                TrigDataL[4] = (UInt16)((data) & 0xffff);//(UInt16)((data) & 0xffff); ?Data Lsb

                TrigDataL[8] = (UInt16)((standardIdMask | (extendIdMask << 11)) & 0xffff);
                TrigDataL[9] = (UInt16)((extendIdMask >> 5) & 0xffff);
                TrigDataL[10] = 0;
                TrigDataL[11] = 0;
                TrigDataL[12] = (UInt16)(dateMask & 0xffff);//?
                TrigDataL[13] = (UInt16)((dateMask >> 16) & 0xffff);//?
                TrigDataL[14] = (UInt16)((dateMask >> 32) & 0xffff);//?
                TrigDataL[15] = (UInt16)((dateMask >> 48) & 0xffff);//?

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
#endif

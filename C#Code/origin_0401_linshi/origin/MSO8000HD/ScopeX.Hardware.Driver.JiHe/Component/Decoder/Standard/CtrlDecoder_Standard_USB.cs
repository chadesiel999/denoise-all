using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal partial class CtrlDecoder
    {
        internal static Int32 Config_Standard_USB()
        {
           
            UInt32 decodechid = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);
            /*
           HdMessage.ProtocolUSBOptions? decodeoption = Hd.UIMessage!.Decoder![decodechid].ProtocolOptions! as HdMessage.ProtocolUSBOptions;

           HdMessage.TrigUSBConditionsOptions? trigoption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigUSBConditionsOptions;

           //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 14);//待定，未开放

           if (decodeoption == null || trigoption == null)
               return -1;

           //解码RAM预触发深度12bit
           UInt32 predepth = (UInt32)((Double)((UInt64)Hd.AnalogChannel!.AcquingParameters.SettingTrigPositionByfs / Hd.AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr) / (Constants.VIS_XDIVS_NUM * 1000) * 4096);
           //HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth, predepth);
           Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_RamPreDepth, predepth);
           #region 通道选择
           ChannelId source = decodeoption!.Source1; //源输入通道ch[0]
           if (source.IsAnalog())//模拟通道
               source += 1;
           else if (source.IsDigital())//数字通道
               source -= 31;
           else
               source = 0;
           ChannelId signal_source = decodeoption.Source2;//信号源（信号类型）
           if (signal_source.IsAnalog())//模拟通道
               signal_source += 1;
           else if (signal_source.IsDigital())//数字通道
               signal_source -= 31;
           else
               signal_source = 0;
           UInt64 sourcecontrolword = (UInt32)source << 0 | (UInt32)signal_source << 6;
           //UInt64 sourcecontrolword = 0x62;
           switch (decodechid)
           {
               case 0:
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1Type, (UInt32)SerialProtocolType.USB);
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.USB);
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2Enable, 0x00);
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1Enable, 0x01);
                   //HdIO.Sleep(1);
                   //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1Enable, 0x00);
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceL, (UInt32)sourcecontrolword & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceM, (UInt32)(sourcecontrolword >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceH, (UInt32)(sourcecontrolword >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                   break;
               case 1:
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2Type, (UInt32)SerialProtocolType.USB);
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.USB);
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1Enable, 0x00);
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2Enable, 0x01);
                   //HdIO.Sleep(1);
                   //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2Enable, 0x00);
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2SignalSourceL, (UInt32)sourcecontrolword & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2SignalSourceM, (UInt32)(sourcecontrolword >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                   Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2SignalSourceH, (UInt32)(sourcecontrolword >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                   break;
               default: break;
           }

           //comment At 2023.06.01 switch (decodechid)
           //comment At 2023.06.01 {
           //comment At 2023.06.01     case 1:
           //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_TypeB1, (UInt32)SerialProtocolType.USB);
           //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.USB);
           //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B2Enable, 0x00);
           //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B1Enable, 0x01);
           //comment At 2023.06.01 
           //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_L, (UInt32)0x103 & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
           //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_M, (UInt32)(sourcecontrolword >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
           //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_H, (UInt32)(sourcecontrolword >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
           //comment At 2023.06.01         break;
           //comment At 2023.06.01     case 2:
           //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_TypeB2, (UInt32)SerialProtocolType.USB);
           //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.USB);
           //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B1Enable, 0x00);
           //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B2Enable, 0x01);
           //comment At 2023.06.01 
           //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_L, (UInt32)0x103 & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
           //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_M, (UInt32)(sourcecontrolword >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
           //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_H, (UInt32)(sourcecontrolword >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
           //comment At 2023.06.01         break;
           //comment At 2023.06.01     default: break;
           //comment At 2023.06.01 }

           #endregion
           //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigTypeSelect, (UInt32)SerialProtocolType.PCIe);
           #region 触发参数设置
           //set参数
           UInt32 datalen = (UInt32)decodeoption.ByteCount;//字节长度（“数据触发”时的字节数）
           //UInt32 datalen = 4;//字节长度（“数据触发”时的字节数）
           UInt32 signalrate = (UInt32)decodeoption.SignalRate;//信号速率
           UInt32 trigcondition = trigoption == null ? 0 : (UInt32)trigoption.Condition;//触发条件
           UInt32 relation = trigoption == null ? 0 : (UInt32)trigoption.DataRelation; //限定符
           //UInt32 relation = 4;//限定符

           UInt64 trigcontrolword = 0;
           trigcontrolword |= trigcondition << 0; //触发条件 
           trigcontrolword |= relation << 4; //数据限定
           trigcontrolword |= datalen << 7;//字节数
           trigcontrolword |= signalrate << 17;//信号速率
                                               //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigcontrolword & 0xFFFF));//发送set[15:0]
                                               //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigcontrolword >> 16) & 0xFFFF));//发送set[31:17]
                                               //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigcontrolword >> 32) & 0xFFFF));//发送set[45:32]

           //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigTypeSelect, (UInt32)SerialProtocolType.USB);//触发源选择
           //comment At 2023.06.01 
           //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigcontrolword & 0xFFFF));//发送set[15:0]
           //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigcontrolword >> 16) & 0xFFFF));//发送set[31:17]
           //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigcontrolword >> 32) & 0xFFFF));//发送set[45:32]

           #endregion

           #region 触发数据设置
           //部分set参数借用TrigData通道发送
           //UInt32 signalType = (UInt32)decodeoption.SignalType;//触发源（信号类型）
           UInt32 data = trigoption == null ? 0 : (UInt32)trigoption!.Data;
           UInt32 tokenpackagetype = trigoption == null ? 0 : (UInt32)trigoption!.TokenPackageType;
           UInt32 handshakepackagetype = trigoption == null ? 0 : (UInt32)trigoption!.HandshakePackageType;
           UInt32 datapackagetype = trigoption == null ? 0 : (UInt32)trigoption!.DataPackageType;
           UInt32 errorpackagetype = trigoption == null ? 0 : (UInt32)trigoption!.ErrorPackageType;
           UInt32 specialpackettype = trigoption == null ? 0 : (UInt32)trigoption!.SpecialPacketType;

           UInt32 pid = 0;//触发条件
           switch (trigcondition)
           {
               case (UInt32)ProtocolUSB.Condition.TokenPackage:
                   pid = tokenpackagetype;
                   break;
               case (UInt32)ProtocolUSB.Condition.HandshakePackage:
                   pid = handshakepackagetype;
                   break;
               case (UInt32)ProtocolUSB.Condition.DataPackage:
                   pid = datapackagetype;
                   break;
               case (UInt32)ProtocolUSB.Condition.Special:
                   pid = specialpackettype;
                   break;
               default: break;
           }
           //触发条件选择“数据”触发时使用的data
           //Int64 addressData = trigoption == null ? 0 : trigoption.AddressData;
           // Int64 data = trigoption == null ? 0 : trigoption.Data;
           UInt32 databyteslength = 16;
           //UInt16[] trigdatal = new UInt16[databyteslength];
           UInt16[] trigdatah = new UInt16[databyteslength];
           // trigcontrolword |= relation << 4; //数据限定
           trigdatah[0] |= (UInt16)pid;
           trigdatah[0] |= (UInt16)(errorpackagetype << 4);
           trigdatah[0] |= (UInt16)((data << 6) & 0xffff);
           trigdatah[1] = (UInt16)((data >> 10) & 0xffff);
           trigdatah[2] = (UInt16)((data >> 26) & 0xffff);
           trigdatah[3] = (UInt16)((data >> 42) & 0xffff);
           trigdatah[4] = (UInt16)((data >> 58) & 0xffff);
           trigdatah[5] = (UInt16)((data >> 74) & 0xffff);
           trigdatah[6] = (UInt16)((data >> 90) & 0xffff);
           trigdatah[7] = (UInt16)((data >> 106) & 0xffff);
           trigdatah[8] = 0;
           trigdatah[9] = 0;
           trigdatah[10] = 0;
           trigdatah[11] = 0;
           trigdatah[12] = 0;
           trigdatah[13] = 0;
           trigdatah[14] = 0;
           trigdatah[15] = 0;
           for (UInt32 dataindex = 0; dataindex < databyteslength; dataindex++)
           {
               //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataL, (UInt32)trigdatal[dataindex] & 0xffff);//数据L
               //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLIndex, dataindex);//数据L索引
               //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLValid, 0x01);//数据L使能
               //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLValid, 0x00);
               //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataH, (UInt32)trigdatah[dataindex] & 0xffff);//数据H
               //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataHIndex, dataindex);//数据H索引
               //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataHValid, 0x01);//数据H使能
               //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataHValid, 0x00);

               //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_protocol_user_data_h, (UInt32)trigdatah[dataindex] & 0xffff);//数据触发的数据
               //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_protocol_user_data_addr_h, dataindex);//数据触发的数据索引
               //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_protocol_user_data_valid_h, 0x01);//拉高数据触发的数据使能
               //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_protocol_user_data_valid_h, 0x00);//拉低数据触发的数据使能


           }

           #endregion
           //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 0x00000000);//复位（复位工作模块，清空缓冲区，DECODE_PROTOCOL_RST）
           //HdIO.Sleep(1);
           //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 0x00000001);//复位（使能工作模块）
                  */
            return (Int32)decodechid;
        }
    }
}

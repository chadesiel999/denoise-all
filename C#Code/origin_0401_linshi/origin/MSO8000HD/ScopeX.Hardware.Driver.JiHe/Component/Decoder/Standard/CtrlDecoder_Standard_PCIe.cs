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
        internal static Int32 Config_Standard_PCIe()
        {
            UInt32 decodechid = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);

            HdMessage.ProtocolPCIeOptions? decodeoption = Hd.UIMessage!.Decoder![decodechid].ProtocolOptions! as HdMessage.ProtocolPCIeOptions;

            HdMessage.TrigPCIeConditionsOptions? trigoption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigPCIeConditionsOptions;

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 23);//待定，未开放

            if (decodeoption == null || trigoption == null)
                return -1;

            //解码RAM预触发深度12bit
            UInt32 predepth = (UInt32)((Double)((UInt64)Hd.AnalogChannel!.AcquingParameters.SettingTrigPositionByfs / Hd.AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr) / (Constants.VIS_XDIVS_NUM * 1000) * 4096);
            //HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth, predepth);
           // Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_RamPreDepth, predepth);
            #region 通道选择
            ChannelId source = decodeoption!.SignalInput; //源输入通道ch[0]
            if (source.IsAnalog())//模拟通道
                source += 1;
            else if (source.IsDigital())//数字通道
                source -= 31;
            else
                source = 0;
            ChannelId signal_source = decodeoption.SignalIutput1;//信号源（信号类型）
            if (signal_source.IsAnalog())//模拟通道
                signal_source += 1;
            else if (signal_source.IsDigital())//数字通道
                signal_source -= 31;
            else
                signal_source = 0;
            UInt64 sourcecontrolword = (UInt32)source << 0 | (UInt32)signal_source << 6;
            //sourcecontrolword = 0x02;
            /*
            switch (decodechid)
            {
                case 0:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1Type, (UInt32)SerialProtocolType.PCIe);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.PCIe);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2Enable, 0x00);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1Enable, 0x01);
                    //HdIO.Sleep(1);
                    //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1Enable, 0x00);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceL, (UInt32)sourcecontrolword & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceM, (UInt32)(sourcecontrolword >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceH, (UInt32)(sourcecontrolword >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                    break;
                case 1:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2Type, (UInt32)SerialProtocolType.PCIe);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.PCIe);
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
            #endregion
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigTypeSelect, (UInt32)SerialProtocolType.PCIe);
            #region 触发参数设置
            //set参数
            UInt32 datalen = trigoption == null ? 0 : (UInt32)trigoption.DataLenght-1;//字节长度（“数据触发”时的字节数）
            ProtocolPCIe.TLPType TLPType = trigoption == null ? 0 : trigoption.TLPType;
            //触发条件
            UInt32 trigcondition = trigoption == null ? 0 : (UInt32)trigoption!.Condition;//触发条件
            UInt32 relation = trigoption == null ? 0 : (UInt32)trigoption.ReqIDData; //限定符
            //UInt64 dataL = 0;
            //UInt64 dataH = 0;
            UInt64 trigcontrolword = 0;
            trigcontrolword |= trigcondition << 0;
            trigcontrolword |= relation << 5;
            trigcontrolword |= (UInt32)TLPType << 8;
            trigcontrolword |= datalen << 13;

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigcontrolword & 0xFFFF));//发送set[15:0]
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigcontrolword >> 16) & 0xFFFF));//发送set[31:17]
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigcontrolword >> 32) & 0xFFFF));//发送set[45:32]
            #endregion

            #region 触发数据设置
            //部分set参数借用TrigData通道发送
            UInt32 signaltype = (UInt32)decodeoption.SignalType;//触发源（信号类型）
            UInt32 sequenceid = trigoption == null ? 0 : (UInt32)trigoption!.SeqID;
            UInt32 tcdata = trigoption == null ? 0 : (UInt32)trigoption!.TCData;
            UInt32 atdata = trigoption == null ? 0 : (UInt32)trigoption!.ATData;
            UInt32 reqid = trigoption == null ? 0 : (UInt32)trigoption!.ReqIDData;
            UInt32 tag = trigoption == null ? 0 : (UInt32)trigoption!.TagData;
            UInt32 msgcode = trigoption == null ? 0 : (UInt32)trigoption!.MsgCodeData;

            //触发条件选择“数据”触发时使用的data
            Int64 addressdata = trigoption == null ? 0 : trigoption.AddressData;
            Int64 data = trigoption == null ? 0 : trigoption.Data;
            UInt32 databyteslength = 16;
            UInt16[] trigdatal = new UInt16[databyteslength];
            UInt16[] trigdatah = new UInt16[databyteslength];
            trigdatah[0] = (UInt16)(msgcode | tag << 8);
            trigdatah[1] = (UInt16)reqid;
            trigdatah[2] = (UInt16)(atdata | tcdata << 2 | (sequenceid & 0x07ff) << 5);
            trigdatah[3] = (UInt16)(sequenceid & 0x01);
            trigdatah[4] = 0;
            trigdatah[5] = 0;
            trigdatah[6] = 0;
            trigdatah[7] = 0;
            trigdatah[8] = (UInt16)(data & 0xffff);
            trigdatah[9] = (UInt16)((data >> 16) & 0xffff);
            trigdatah[10] = (UInt16)((data >> 32) & 0xffff);
            trigdatah[11] = (UInt16)((data >> 48) & 0xffff);
            trigdatah[12] = (UInt16)(addressdata & 0xffff);
            trigdatah[13] = (UInt16)((addressdata >> 16) & 0xffff);
            trigdatah[14] = (UInt16)((addressdata >> 32) & 0xffff);
            trigdatah[15] = (UInt16)((addressdata >> 48) & 0xffff);
            for (UInt32 dataindex = 0; dataindex < databyteslength; dataindex++)
            {
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataL, (UInt32)trigdatal[dataindex] & 0xffff);//数据L
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLIndex, dataindex);//数据L索引
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLValid, 0x01);//数据L使能
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLValid, 0x00);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataH, (UInt32)trigdatah[dataindex] & 0xffff);//数据H
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataHIndex, dataindex);//数据H索引
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataHValid, 0x01);//数据H使能
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataHValid, 0x00);
            }
          */
            #endregion
            // Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 0x00000000);//复位（复位工作模块，清空缓冲区，DECODE_PROTOCOL_RST）
            // HdIO.Sleep(1);
            // Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 0x00000001);//复位（使能工作模块）


            return (Int32)decodechid;
        }
    }
}

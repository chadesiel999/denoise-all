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
        internal static Int32 Config_Standard_Ethernet()
        {
            UInt32 decodechid = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);

            HdMessage.ProtocolEthernetOptions? decodeoption = Hd.UIMessage!.Decoder![decodechid].ProtocolOptions! as HdMessage.ProtocolEthernetOptions;

            HdMessage.TrigEthernetConditionsOptions? trigoption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigEthernetConditionsOptions;

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 20);//待定

            if (decodeoption == null || trigoption == null)
                return -1;

            //解码RAM预触发深度12bit
            UInt32 predepth = 2048;
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth, predepth);

            #region 通道选择
            ChannelId source = decodeoption!.SignalInput1; //源输入通道ch[0]
            if (source.IsAnalog())//模拟通道
                source += 1;
            else if (source.IsDigital())//数字通道
                source -= 31;
            else
                source = 0;
            ChannelId sourcel = decodeoption!.SignalInput2;//源L输入通道ch[1]
            if (sourcel.IsAnalog())//模拟通道
                sourcel += 1;
            else if (sourcel.IsDigital())//数字通道
                sourcel -= 31;
            else
                sourcel = 0;
            UInt64 sourcecontrolword = (UInt32)sourcel << 0 | (UInt32)source << 6;
            //comment At 2023.06.01 switch (decodechid)
            //comment At 2023.06.01 {
            //comment At 2023.06.01     case 1:
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_TypeB1, (UInt32)SerialProtocolType.Ethernet);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.Ethernet);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B2Enable, 0x00);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B1Enable, 0x01);
            //comment At 2023.06.01 
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_L, (UInt32)sourcecontrolword & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_M, (UInt32)(sourcecontrolword >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_H, (UInt32)(sourcecontrolword >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         break;
            //comment At 2023.06.01     case 2:
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_TypeB2, (UInt32)SerialProtocolType.Ethernet);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.Ethernet);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B1Enable, 0x00);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B2Enable, 0x01);
            //comment At 2023.06.01 
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_L, (UInt32)sourcecontrolword & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_M, (UInt32)(sourcecontrolword >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_H, (UInt32)(sourcecontrolword >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         break;
            //comment At 2023.06.01     default: break;
            //comment At 2023.06.01 }
            #endregion


            #region 触发参数设置
            UInt32 signaltype = (UInt32)decodeoption.SignalType;//信号源（信号类型单端/差分）
            UInt32 speed = (UInt32)decodeoption.Speed;//10M  100M  1000M
            UInt32 version = (UInt32)decodeoption.Version;//版本：IPV4/IPV6
            //UInt32 qflag = (UInt32)decodeoption.QFlag;//Q标记


            UInt32 trigcondition;
            //UInt32 data = 0;
            Byte[] data = { };
            UInt32 relation = 0;

            if (trigoption != null)
            {
                trigcondition = (UInt32)trigoption!.Condition;//触发条件
               // data = (UInt32)trigoption!.Data;
                relation = (UInt32)trigoption!.Relation;
            }
            else
            {
                trigcondition = 0;
                //data = 0;
            }

            UInt64 trigcontrolword = 0;
            trigcontrolword |= trigcondition << 0;//触发条件
            trigcontrolword |= relation << 4;//数据限定符


            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigTypeSelect, (UInt32)SerialProtocolType.Ethernet);//触发源选择
            //comment At 2023.06.01 
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigcontrolword & 0xFFFF));//发送set[15:0]
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigcontrolword >> 16) & 0xFFFF));//发送set[31:17]
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigcontrolword >> 32) & 0xFFFF));//发送set[45:32]
            #endregion





            #region 触发数据设置
            UInt32 databyteslength = 16;
            UInt16[] trigdata = new UInt16[databyteslength];


            if(trigcondition == 2)
            {
                //TrigData[0] = (UInt16)(data & 0xffff);
                //TrigData[8] = (UInt16)(data & 0xffff);
            }
            else
            {
                trigdata[0] = (UInt16)(trigoption!.SrcMAC![1]<<8 | trigoption!.SrcMAC[0]);
                trigdata[1] = (UInt16)(trigoption!.SrcMAC[3] << 8 | trigoption!.SrcMAC[2]); 
                trigdata[2] = (UInt16)(trigoption!.SrcMAC[5] << 8 | trigoption!.SrcMAC[4]);


                trigdata[8] = (UInt16)(trigoption!.DestMAC![1] << 8 | trigoption!.DestMAC[0]);
                trigdata[9] = (UInt16)(trigoption!.DestMAC[3] << 8 | trigoption!.DestMAC[2]);
                trigdata[10] = (UInt16)(trigoption!.DestMAC[5] << 8 | trigoption!.DestMAC[4]);
            }


            for (UInt32 dataindex = 0; dataindex < databyteslength; dataindex++)
            {
                //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_protocol_user_data_h, (UInt32)TrigData[dataindex] & 0xffff);//数据触发的数据
                //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_protocol_user_data_addr_h, dataindex);//数据触发的数据索引
                //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_protocol_user_data_valid_h, 0x01);//拉高数据触发的数据使能
                //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_protocol_user_data_valid_h, 0x00);//拉低数据触发的数据使能
            }
            #endregion

            return (Int32)decodechid;
        }
    }
}

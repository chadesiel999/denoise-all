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
        internal static Int32 Config_Standard_JTAG()
        {
            UInt32 decodechid = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);

            HdMessage.ProtocolJTAGOptions? decodeoption = Hd.UIMessage!.Decoder![decodechid].ProtocolOptions! as HdMessage.ProtocolJTAGOptions;

            HdMessage.TrigJTAGConditionsOptions? trigoption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigJTAGConditionsOptions;

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 17);//待定，未开放

            if (decodeoption == null || trigoption == null)
                return -1;

            #region 通道选择
            ChannelId tck = decodeoption!.TCK; //时钟线
            if (tck.IsAnalog())//模拟通道
                tck += 1;
            else if (tck.IsDigital())//数字通道
                tck -= 31;
            else
                tck = 0;
            ChannelId TMS = decodeoption!.TMS;//功能选择线
            if (TMS.IsAnalog())//模拟通道
                TMS += 1;
            else if (TMS.IsDigital())//数字通道
                TMS -= 31;
            else
                TMS = 0;
            ChannelId tdi = decodeoption!.TDI;//输入
            if (tdi.IsAnalog())//模拟通道
                tdi += 1;
            else if (tdi.IsDigital())//数字通道
                tdi -= 31;
            else
                tdi = 0;
            ChannelId tdo = decodeoption!.TDO;//输出
            if (tdo.IsAnalog())//模拟通道
                tdo += 1;
            else if (tdo.IsDigital())//数字通道
                tdo -= 31;
            else
                tdo = 0;


            UInt64 sourcecontrolword = (UInt32)tck << 0 | (UInt32)TMS << 6 | (UInt32)tdi << 12 | (UInt32)tdo << 18;
            //comment At 2023.06.01 switch (decodechid)
            //comment At 2023.06.01 {
            //comment At 2023.06.01     case 1:
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_TypeB1, (UInt32)SerialProtocolType.JTAG);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.JTAG);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B2Enable, 0x00);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B1Enable, 0x01);
            //comment At 2023.06.01 
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_L, (UInt32)sourcecontrolword & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_M, (UInt32)(sourcecontrolword >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_H, (UInt32)(sourcecontrolword >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         break;
            //comment At 2023.06.01     case 2:
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_TypeB2, (UInt32)SerialProtocolType.JTAG);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.JTAG);
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
            UInt32 decodemode = (UInt32)decodeoption.DecodeChannel;//0:TDI  1:TDO

            UInt32 trigcondition;
            if (trigoption != null)
                trigcondition = (UInt32)trigoption!.Condition;//触发条件
            else
                trigcondition = 0;

            UInt64 trigcontrolword = 0;
            trigcontrolword |= trigcondition << 0;
            trigcontrolword |= decodemode << 4;

            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigTypeSelect, (UInt32)SerialProtocolType.JTAG);//触发源选择
            //comment At 2023.06.01 
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigcontrolword & 0xFFFF));//发送set[15:0]
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigcontrolword >> 16) & 0xFFFF));//发送set[31:17]
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigcontrolword >> 32) & 0xFFFF));//发送set[45:32]
            #endregion



            #region 触发数据设置
            UInt32 databyteslength = 16;
            UInt16[] trigdata = new UInt16[databyteslength];
            if ((trigoption!.Data?.Length ?? 0) > 0)
            {
                Int32 datalen = trigoption!.Data!.Length;
                for (UInt32 dataindex = 0; dataindex < datalen / 2; dataindex++)
                    trigdata[dataindex] = (UInt16)((trigoption!.Data![dataindex * 2 + 1] << 8) | trigoption!.Data![dataindex * 2 + 0]);
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

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
        internal static Int32 Config_Standard_SPMI()
        {
            UInt32 decodechid = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);

            HdMessage.ProtocolSPMIOptions? decodeoption = Hd.UIMessage!.Decoder![decodechid].ProtocolOptions! as HdMessage.ProtocolSPMIOptions;

            HdMessage.TrigSPMIConditionsOptions? trigoption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigSPMIConditionsOptions;

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 14);//待定，未开放

            if (decodeoption == null || trigoption == null)
                return -1;
            
            #region 通道选择
            ChannelId clk_input = decodeoption!.SCLK;//时钟输入通道ch[0]
            if (clk_input.IsAnalog())//模拟通道
                clk_input += 1;
            else if (clk_input.IsDigital())//数字通道
                clk_input -= 31;
            else
                clk_input = 0;
            ChannelId data_input = decodeoption.SData;//片选输入通道ch[1]
            if (data_input.IsAnalog())//模拟通道
                data_input += 1;
            else if (data_input.IsDigital())//数字通道
                data_input -= 31;
            else
                data_input = 0;

            UInt64 sourcecontrolword = (UInt32)data_input << 0 | (UInt32)clk_input << 6 ;
            //comment At 2023.06.01 switch (decodechid)
            //comment At 2023.06.01 {
            //comment At 2023.06.01     case 1:
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_TypeB1, (UInt32)SerialProtocolType.SPMI);
            //comment At 2023.06.01 		HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.SPMI);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B2Enable, 0x00);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B1Enable, 0x01);
            //comment At 2023.06.01         HdIO.Sleep(1);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B1Enable, 0x00);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_L, (UInt32)sourcecontrolword & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_M, (UInt32)(sourcecontrolword >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_H, (UInt32)(sourcecontrolword >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         break;
            //comment At 2023.06.01     case 2:
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_TypeB2, (UInt32)SerialProtocolType.SPMI);
            //comment At 2023.06.01 		HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.SPMI);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B1Enable, 0x00);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B2Enable, 0x01);
            //comment At 2023.06.01         HdIO.Sleep(1);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_B2Enable, 0x00);
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_L, (UInt32)sourcecontrolword & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_M, (UInt32)(sourcecontrolword >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_H, (UInt32)(sourcecontrolword >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //comment At 2023.06.01         break;
            //comment At 2023.06.01     default: break;
            //comment At 2023.06.01 }
            #endregion

            #region 触发参数设置
            //set参数
            UInt32 checktype = (UInt32)decodeoption.CheckType;//奇偶校验
            UInt32 version = (UInt32)decodeoption.Version;//版本

            UInt32 trigcondition = trigoption == null ? 0 : (UInt32)trigoption.Condition;//触发条件,SPMI全是帧类型触发

            UInt64 trigcontrolword = 0;
            trigcontrolword |= trigcondition << 0;
            trigcontrolword |= version << 5;
            trigcontrolword |= checktype << 6;


            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigcontrolword & 0xFFFF));//发送set[15:0]
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigcontrolword >> 16) & 0xFFFF));//发送set[31:17]
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigcontrolword >> 32) & 0xFFFF));//发送set[45:32]
            #endregion

            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigTypeSelect, (UInt32)SerialProtocolType.SPMI);//触发源选择
            return (Int32)decodechid;
        }
    }
}

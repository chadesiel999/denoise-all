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
        internal static Int32 Config_Standard_DigRF_V4()
        {
            //Boolean isDecode = true;
            //HdIO.WriteReg(ProcBdReg.W.Decoder_IsDecodeFunction,isDecode?1U:0);//选择解码还是触发
            //HdIO.Sleep(1);
            ////if(isDecode)
            ////{
            ////    HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForDecoder, (UInt32)SerialProtocolType.DigRF_V4);
            ////}
            ////else
            ////{
            ////    HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.DigRF_V4);
            ////}
            //HdIO.Sleep(1);
            ////[1:0]:当前解码通道设置
            ////10：选择解码通道2；
            ////01：选择解码通道1
            ////需要修改（新代码使用两个寄存器分别代表解码通道1和2）
            //UInt32 decodeChID = 1;
            //switch (decodeChID)
            //{
            //    case 1:
            //        HdIO.WriteReg(ProcBdReg.W.Decoder_D2Enable, 0x00);
            //        HdIO.WriteReg(ProcBdReg.W.Decoder_D1Enable, 0x01);
            //        HdIO.Sleep(1);
            //        HdIO.WriteReg(ProcBdReg.W.Decoder_D1Enable, 0x00);
            //        break;
            //    case 2:
            //        HdIO.WriteReg(ProcBdReg.W.Decoder_D1Enable, 0x00);;
            //        HdIO.WriteReg(ProcBdReg.W.Decoder_D2Enable, 0x01);
            //        HdIO.Sleep(1);
            //        HdIO.WriteReg(ProcBdReg.W.Decoder_D2Enable, 0x00);
            //        break;
            //    default: break;
            //}

            ////解码RAM预触发深度12bit
            ////UInt32 predepth = (UInt32)(((Double)Acquire.Default.XPhySys.Position / GRID_WIDTH) * DecodePackageQueueBase.DECODE_BUF_SIZE);
            //UInt32 predepth = (UInt32)((Double)((UInt64)Hd.AnalogChannel!.AcquingParameters.SettingTrigPositionByfs / Hd.AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr) / (Constants.VIS_XDIVS_NUM * 1000) * 4096);
            //HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth,predepth);

            ////通道选择
            //UInt32 Signal_InputA = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.SignalInputA*/;//时钟通道 ch[0]
            //if (Signal_InputA < 4)//模拟通道
            //{
            //    Signal_InputA += 1;
            //}
            //else//数字通道
            //{
            //    Signal_InputA += 2;
            //}

            //UInt32 Signal_InputB = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.SignalInputB*/;//时钟通道 ch[0]
            //if (Signal_InputB < 4)//模拟通道
            //{
            //    Signal_InputB += 1;
            //}
            //else//数字通道
            //{
            //    Signal_InputB += 2;
            //}

            ////set参数
            //UInt32 signal_rate = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.SignalRate*/;//信号速率

            //UInt32 input_mode = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.InputMode*/;//输入模式


            //UInt32 relation = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.DataRelation*/;//限定符

            //UInt32 trig_condition = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.TriggerCondition*/;//触发条件

            //UInt32 cri = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.CRI*/;

            //UInt32 id = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.ID*/;

            //UInt32 lci = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.LCI*/;

            //UInt32 byte_count = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.ByteCount - 1*/;

            //UInt32 byte_offset = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.ByteOffset*/;

            ////触发条件选择“数据”触发时使用的data
            ////datal只在IN，OUT限定时才使用
            //UInt32 datal1 = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data3*/ & 0xffff;   //low[15:0]
            //UInt32 datal2 = (UInt32)(0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data3*/ >> 16) & 0xffff; //low[31:16]
            //UInt32 datal3 = (UInt32)(0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data3*/ >> 32) & 0xffff;//[47:32]
            //UInt32 datal4 = (UInt32)(0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data3*/ >> 48) & 0xffff;//[63:48]

            //UInt32 datal5 = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data4*/ & 0xffff; //[79:64]
            //UInt32 datal6 = (UInt32)(0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data4*/ >> 16) & 0xffff;  //[95:80]
            //UInt32 datal7 = (UInt32)(0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data4*/ >> 32) & 0xffff;  //[112:96]
            //UInt32 datal8 = (UInt32)(0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data4*/ >> 48) & 0xffff;   //height[127:113]

            ////Int32[] data1Array = ProtocolSysParameter.Default.DigRF_3GParameter.Data1Array;//daataArray[0]-daataArray[3]
            //UInt32 datah1 = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data1*/ & 0xffff;   //low[15:0]
            //UInt32 datah2 = (UInt32)(0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data1*/ >> 16) & 0xffff;   //[31:16]
            //UInt32 datah3 = (UInt32)(0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data1*/ >> 32) & 0xffff;  //[47:32]
            //UInt32 datah4 = (UInt32)(0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data1*/ >> 48) & 0xffff;  //[63:48]

            //UInt32 datah5 = (UInt32)0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data2*/ & 0xffff;  //[79:64]
            //UInt32 datah6 = (UInt32)(0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data2*/ >> 16) & 0xffff;  //[95:80]
            //UInt32 datah7 = (UInt32)(0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data2*/ >> 32) & 0xffff;  //[112:96]
            //UInt32 datah8 = (UInt32)(0/*ProtocolSysParameter.Default.DigRF_V4Parameter.Data2*/ >> 48) & 0xffff;   //height[127:113]


            //UInt64 send_data = 0;

            /////////////////////////////////////////////////////////////////////////////////////////////////

            //send_data = 0;//清零
            //send_data |= Signal_InputA << 0;
            //send_data |= Signal_InputB << 5;
            //HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSourceL, (UInt32)send_data & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSourceM, (UInt32)(send_data >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSourceH, (UInt32)(send_data >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）

            ////数据
            ////retn = HdCommand.PCIX_WriteRegister32(DECODE_DATA0, (datah1 << 16) | datal1);//low
            ////retn = HdCommand.PCIX_WriteRegister32(DECODE_DATA1, (datah2 << 16) | datal2);
            ////retn = HdCommand.PCIX_WriteRegister32(DECODE_DATA2, (datah3 << 16) | datal3);
            ////retn = HdCommand.PCIX_WriteRegister32(DECODE_DATA3, (datah4 << 16) | datal4);
            ////retn = HdCommand.PCIX_WriteRegister32(DECODE_DATA4, (datah5 << 16) | datal5);
            ////retn = HdCommand.PCIX_WriteRegister32(DECODE_DATA5, (datah6 << 16) | datal6);
            ////retn = HdCommand.PCIX_WriteRegister32(DECODE_DATA6, (datah7 << 16) | datal7);
            ////retn = HdCommand.PCIX_WriteRegister32(DECODE_DATA7, (datah8 << 16) | datal8);//height
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data0L, datal1 & 0xffff);//low
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data0H, datah1 & 0xffff);//high
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data1L, datal2 & 0xffff);//low
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data1H, datah2 & 0xffff);//high
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data2L, datal3 & 0xffff);//low
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data2H, datah3 & 0xffff);//high
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data3L, datal4 & 0xffff);//low
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data3H, datah4 & 0xffff);//high
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data4L, datal5 & 0xffff);//low
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data4H, datah5 & 0xffff);//high
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data5L, datal6 & 0xffff);//low
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data5H, datah6 & 0xffff);//high
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data6L, datal7 & 0xffff);//low
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data6H, datah7 & 0xffff);//high
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data7L, datal8 & 0xffff);//low
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data7H, datah8 & 0xffff);//high

            ////set
            //send_data = 0;//清零
            //send_data |= trig_condition << 0;//3b
            //send_data |= relation << 3;//3b
            //send_data |= byte_offset << 6;//8b
            //send_data |= byte_count << 14;
            //HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)send_data);//发送set[15:0]

            //send_data = 0;  //清零
            //send_data |= (byte_count >> 2) << 0;//4b
            //send_data |= lci << 2;//1b
            //send_data |= id << 3;//3b
            //send_data |= cri << 6;//3b
            //send_data |= input_mode << 9;//2b
            //send_data |= signal_rate << 11;//5b
            //HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)send_data);//发送set[31:17]

            //HdIO.WriteReg(ProcBdReg.W.Decoder_ResetAfterParamChanged, 0x00000000);//复位（复位工作模块，清空缓冲区，DECODE_PROTOCOL_RST）
            //HdIO.Sleep(1);
            //HdIO.WriteReg(ProcBdReg.W.Decoder_ResetAfterParamChanged, 0x00000001);//复位（使能工作模块）

            return -1;
        }
    }
}

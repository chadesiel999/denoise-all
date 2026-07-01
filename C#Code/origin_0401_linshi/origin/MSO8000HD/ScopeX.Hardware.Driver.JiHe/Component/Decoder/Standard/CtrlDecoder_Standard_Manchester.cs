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
        internal static Int32 Config_Standard_Manchester()
        {
            //Boolean isDecode = true;
            //HdIO.WriteReg(ProcBdReg.W.Decoder_IsDecodeFunction, isDecode ? 1U : 0);//选择解码还是触发
            //HdIO.Sleep(1);
            ////if (isDecode)
            ////{
            ////    HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForDecoder, (UInt32)SerialProtocolType.Manchester);
            ////}
            ////else
            ////{
            ////    HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.Manchester);
            ////}
            //HdIO.Sleep(1);
            ////[1:0]:当前解码通道设置
            ////10：选择解码通道2；
            ////01：选择解码通道1
            ////需要修改（新代码使用两个寄存器分别代表解码通道1和2）
            //UInt32 decodechid = 1;
            //switch (decodechid)
            //{
            //    case 1:
            //        HdIO.WriteReg(ProcBdReg.W.Decoder_D2Enable, 0x00);
            //        HdIO.WriteReg(ProcBdReg.W.Decoder_D1Enable, 0x01);
            //        HdIO.Sleep(1);
            //        HdIO.WriteReg(ProcBdReg.W.Decoder_D1Enable, 0x00);
            //        break;
            //    case 2:
            //        HdIO.WriteReg(ProcBdReg.W.Decoder_D1Enable, 0x00); ;
            //        HdIO.WriteReg(ProcBdReg.W.Decoder_D2Enable, 0x01);
            //        HdIO.Sleep(1);
            //        HdIO.WriteReg(ProcBdReg.W.Decoder_D2Enable, 0x00);
            //        break;
            //    default: break;
            //}
            ////解码RAM预触发深度12bit
            ////UInt32 predepth = (UInt32)(((Double)Acquire.Default.XPhySys.Position / GRID_WIDTH) * DecodePackageQueueBase.DECODE_BUF_SIZE);
            //UInt32 predepth = (UInt32)((Double)((UInt64)Hd.AnalogChannel!.AcquingParameters.SettingTrigPositionByfs / Hd.AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr) / (Constants.VIS_XDIVS_NUM * 1000) * 4096);
            //HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth, predepth);

            ////通道选择
            //UInt32 Signal_Input = (UInt32)0/*ProtocolSysParameter.Default.ManchesterParameter.SignalInput*/;//时钟通道 ch[0]
            //if (Signal_Input < 4)//模拟通道
            //{
            //    Signal_Input += 1;
            //}
            //else//数字通道
            //{
            //    Signal_Input += 2;
            //}

            ////set参数
            //Int64 customSignalRate = 250000/*ProtocolSysParameter.Default.ManchesterParameter.CustomSignalRate*/;
            ////DecodeManchesterSignalRate signalRate = ProtocolSysParameter.Default.ManchesterParameter.signalrate;
            ////switch (signalRate)
            ////{
            ////    case DecodeManchesterSignalRate.ManchesterSignalRate_250Kbps:
            ////        customSignalRate = 250000;
            ////        break;
            ////    case DecodeManchesterSignalRate.ManchesterSignalRate_1Mbps:
            ////        customSignalRate = 1000000;
            ////        break;
            ////    case DecodeManchesterSignalRate.ManchesterSignalRate_5Mbps:
            ////        customSignalRate = 5000000;
            ////        break;
            ////    case DecodeManchesterSignalRate.ManchesterSignalRate_10Mbps:
            ////        customSignalRate = 10000000;
            ////        break;
            ////    default: break;
            ////}
            //UInt32 signal_rate = (UInt32)(Constants.PROT_SYS_CLOCK_HZ / customSignalRate) - 1;//信号速率

            //UInt32 idle_state = (UInt32)0/*ProtocolSysParameter.Default.ManchesterParameter.IdleState*/;//空闲状态
            //UInt32 polarity = (UInt32)0/*ProtocolSysParameter.Default.ManchesterParameter.Polarity*/;//极性
            //UInt32 first_bit_polarity = (UInt32)0/*ProtocolSysParameter.Default.ManchesterParameter.FirstBitPolarity*/;//极性
            //UInt32 syn_bit_count = (UInt32)0/*ProtocolSysParameter.Default.ManchesterParameter.SynBitCount*/;//同步位数
            //if (syn_bit_count == 0)
            //{
            //    syn_bit_count = 0x1f;
            //}
            //else
            //{
            //    --syn_bit_count;
            //}
            //UInt32 data_byte_count = (UInt32)0/*ProtocolSysParameter.Default.ManchesterParameter.DataByteCount - 1*/;//数据字节数
            //UInt32 post_bit_count = (UInt32)0/*ProtocolSysParameter.Default.ManchesterParameter.PostBitCount*/;//后置位数
            //if (post_bit_count == 0)
            //{
            //    post_bit_count = 0x1f;
            //}
            //else
            //{
            //    --post_bit_count;
            //}
            //UInt32 pre_bit_count = (UInt32)0/*ProtocolSysParameter.Default.ManchesterParameter.PreBitCount*/;//前置位数
            //if (pre_bit_count == 0)
            //{
            //    pre_bit_count = 0x1f;
            //}
            //else
            //{
            //    --pre_bit_count;
            //}
            //UInt32 bit_seq = (UInt32)0/*ProtocolSysParameter.Default.ManchesterParameter.BitSeq*/;//位序
            //UInt32 type = (UInt32)0/*ProtocolSysParameter.Default.ManchesterParameter.ManchesterType*/;//类型

            //UInt32 relation = (UInt32)0/*ProtocolSysParameter.Default.ManchesterParameter.DataRelation*/;//限定符
            //UInt32 trig_condition = (UInt32)0/*ProtocolSysParameter.Default.ManchesterParameter.TriggerCondition*/;//触发条件

            ////set参数
            //UInt32 idle_time = (UInt32)(0/*ProtocolSysParameter.Default.ManchesterParameter.IdleTimePS*/ / 4000) & 0x0FFFFFFF; //空闲时间（28bit）

            ////触发条件选择“数据”触发时使用的data
            ////datal只在IN，OUT限定时才使用
            //UInt32 data1 = (UInt32)0/*ProtocolSysParameter.Default.ManchesterParameter.Data1*/;
            //UInt32 data2 = (UInt32)0/*ProtocolSysParameter.Default.ManchesterParameter.Data2*/;

            //UInt32 data1l = data2 & 0xffff;
            //UInt32 data2l = (data2 >> 16) & 0xffff;

            //UInt32 data1h = data1 & 0xffff;
            //UInt32 data2h = (data1 >> 16) & 0xffff;

            //UInt32 data3h = idle_time & 0xffff;
            //UInt32 data4h = (idle_time >> 16) & 0xffff;

            //UInt64 send_data = 0;

            ///////////////////////////////////////////////////////////////////////////////////////////////

            //send_data = 0;//清零
            //send_data |= Signal_Input << 0;
            //HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSourceL, (UInt32)(send_data & 0xffff));//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSourceM, (UInt32)(send_data >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSourceH, (UInt32)(send_data >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）

            ////数据
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data0L, data1l);//low
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data0H, data1h);//high
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data1L, data2l);//low
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data1H, data2h);//high//revised by lhy 20201212
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data2L, data3h);//空闲时间低位
            //HdIO.WriteReg(ProcBdReg.W.Decoder_Data3L, data4h);//空闲时间高位

            ////set
            //send_data = 0;//清零
            //send_data |= trig_condition << 0;
            //send_data |= relation << 2;
            //send_data |= type << 5;
            //send_data |= bit_seq << 6;
            //send_data |= post_bit_count << 7;
            //send_data |= data_byte_count << 12;
            //send_data |= pre_bit_count << 14;
            //HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)send_data);//发送set[15:0]

            //send_data = 0;  //清零
            //send_data |= (pre_bit_count >> 2) << 0;
            //send_data |= syn_bit_count << 3;
            //send_data |= first_bit_polarity << 8;
            //send_data |= polarity << 9;
            //send_data |= idle_state << 10;
            //send_data |= signal_rate << 12;
            //HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)send_data);//发送set[31:17]

            //send_data = 0;  //清零
            //send_data |= (signal_rate >> 4) << 0;
            //HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)send_data);//发送set[45:32]

            //HdIO.WriteReg(ProcBdReg.W.Decoder_ResetAfterParamChanged, 0x00000000);//复位（复位工作模块，清空缓冲区，DECODE_PROTOCOL_RST）
            //HdIO.Sleep(1);
            //HdIO.WriteReg(ProcBdReg.W.Decoder_ResetAfterParamChanged, 0x00000001);//复位（使能工作模块）
            return -1;
        }
    }
}

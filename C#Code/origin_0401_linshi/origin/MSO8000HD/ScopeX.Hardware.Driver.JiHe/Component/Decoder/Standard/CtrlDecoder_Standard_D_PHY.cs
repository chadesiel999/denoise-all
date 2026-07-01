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
        private static Int32[] MoveElements2Msb(Int32[] array/*in*/)
        {
            Int32[] newarray = new Int32[array.Length];
            Buffer.BlockCopy(array, 0, newarray, 0, array.Length * sizeof(Int32));
            Byte[] byteArray = new Byte[newarray.Length * sizeof(Int32)];//创建空的Byte数组
            Buffer.BlockCopy(newarray, 0, byteArray, 0, newarray.Length * sizeof(Int32));//数组类型转换（newArray[]->byteArray[]）
            Int32 i = 0, j = 0;
            //从MSB开始向下搜索第一个非零元素byteArray[i]
            for (i = byteArray.Length - 1; i >= 0; i--)
            {
                if (byteArray[i] != 0)
                    break;
            }
            Array.Clear(newarray, 0, newarray.Length);//清零newArray[]
            //循环移位操作
            for (j = newarray.Length - 1; i >= 0 && j >= 0; j--)
            {
                Int32 tmp = 0;
                Int32 k = 0;
                for (k = 0; k < 4; k++)
                {
                    tmp <<= 8;
                    if (i >= 0)
                    {
                        tmp |= byteArray[i];
                        i--;
                    }
                }
                newarray[j] = tmp;
            }
            return newarray;
        }
        internal static Int32 Config_Standard_D_PHY()
        {
            //    Boolean isDecode = true;
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_IsDecodeFunction, isDecode ? 1U : 0);//选择解码还是触发
            //    HdIO.Sleep(1);
            //    //if (isDecode)
            //    //{
            //    //    HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForDecoder, (UInt32)SerialProtocolType.D_PHY);
            //    //}
            //    //else
            //    //{
            //    //    HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.D_PHY);
            //    //}
            //    HdIO.Sleep(1);
            //    //[1:0]:当前解码通道设置
            //    //10：选择解码通道2；
            //    //01：选择解码通道1
            //    //需要修改（新代码使用两个寄存器分别代表解码通道1和2）
            //    UInt32 decodechid = 1;
            //    switch (decodechid)
            //    {
            //        case 1:
            //            HdIO.WriteReg(ProcBdReg.W.Decoder_D2Enable, 0x00);
            //            HdIO.WriteReg(ProcBdReg.W.Decoder_D1Enable, 0x01);
            //            HdIO.Sleep(1);
            //            HdIO.WriteReg(ProcBdReg.W.Decoder_D1Enable, 0x00);
            //            break;
            //        case 2:
            //            HdIO.WriteReg(ProcBdReg.W.Decoder_D1Enable, 0x00); ;
            //            HdIO.WriteReg(ProcBdReg.W.Decoder_D2Enable, 0x01);
            //            HdIO.Sleep(1);
            //            HdIO.WriteReg(ProcBdReg.W.Decoder_D2Enable, 0x00);
            //            break;
            //        default: break;
            //    }

            //    //解码RAM预触发深度12bit
            //    UInt32 predepth = (UInt32)((Double)((UInt64)Hd.AnalogChannel!.AcquingParameters.SettingTrigPositionByfs / Hd.AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr) / (Constants.VIS_XDIVS_NUM * 1000) * 4096);
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth, predepth);

            //    //通道选择
            //    UInt32 HSPlus =0 /*(UInt32)ProtocolSysParameter.Default.D_PHYParameter.HSPlus*/;//时钟通道 ch[0]
            //    if (HSPlus < 4)//模拟通道
            //        HSPlus += 1;
            //    else//数字通道
            //        HSPlus += 2;

            //    UInt32 HSDec = 0/*(UInt32)ProtocolSysParameter.Default.D_PHYParameter.HSDec*/;//时钟通道 ch[0]
            //    if (HSDec < 4)//模拟通道
            //        HSDec += 1;
            //    else//数字通道
            //        HSDec += 2;
            //    UInt32 LPPlus =0 /*(UInt32)ProtocolSysParameter.Default.D_PHYParameter.LPPlus*/;//时钟通道 ch[0]
            //    if (LPPlus < 4)//模拟通道
            //        LPPlus += 1;
            //    else//数字通道
            //        LPPlus += 2;
            //    UInt32 LPDec = 0/*(UInt32)ProtocolSysParameter.Default.D_PHYParameter.LPDec*/;//时钟通道 ch[0]
            //    if (LPDec < 4)//模拟通道
            //        LPDec += 1;
            //    else//数字通道
            //        LPDec += 2;
            //    UInt32 CLKPlus =0 /*(UInt32)ProtocolSysParameter.Default.D_PHYParameter.CLKPlus*/;//时钟通道 ch[0]
            //    if (CLKPlus < 4)//模拟通道
            //        CLKPlus += 1;
            //    else//数字通道
            //        CLKPlus += 2;

            //    UInt32 CLKDec = 0/*(UInt32)ProtocolSysParameter.Default.D_PHYParameter.CLKDec*/;//时钟通道 ch[0]
            //    if (CLKDec < 4)//模拟通道
            //        CLKDec += 1;
            //    else//数字通道
            //        CLKDec += 2;

            //    UInt32 relation = 0/*(UInt32)ProtocolSysParameter.Default.D_PHYParameter.DataRelation*/;//限定符
            //    UInt32 trig_condition = 0/*(UInt32)ProtocolSysParameter.Default.D_PHYParameter.TriggerCondition*/;//触发条件
            //    UInt32 byte_count =0/* (UInt32)ProtocolSysParameter.Default.D_PHYParameter.ByteCount - 1*/;
            //    UInt32 wc =0 /*(UInt32)ProtocolSysParameter.Default.D_PHYParameter.WC*/;
            //    UInt32 dt =0 /*(UInt32)ProtocolSysParameter.Default.D_PHYParameter.DT*/;
            //    UInt32 vc =0 /*(UInt32)ProtocolSysParameter.Default.D_PHYParameter.VC*/;
            //    //触发条件选择“数据”触发时使用的data
            //    //datal只在IN，OUT限定时才使用
            //    Int32[] Data2Array =
            //    {
            //        (Int32)0/*(ProtocolSysParameter.Default.D_PHYParameter.Data3&0xffffffff)*/,
            //        (Int32)0/*(ProtocolSysParameter.Default.D_PHYParameter.Data3>>32&0xffffffff)*/,
            //        (Int32)0/*(ProtocolSysParameter.Default.D_PHYParameter.Data4&0xffffffff)*/,
            //        (Int32)0/*(ProtocolSysParameter.Default.D_PHYParameter.Data4>>32&0xffffffff)*/,
            //    };
            //    Int32[] data2Array = MoveElements2Msb(Data2Array);
            //    UInt32 datal1 = (UInt32)data2Array[0] & 0xffff;   //low[15:0]
            //    UInt32 datal2 = (UInt32)(data2Array[0] >> 16) & 0xffff;   //[31:16]
            //    UInt32 datal3 = (UInt32)data2Array[1] & 0xffff;  //[47:32]
            //    UInt32 datal4 = (UInt32)(data2Array[1] >> 16) & 0xffff;  //[63:48]
            //    UInt32 datal5 = (UInt32)data2Array[2] & 0xffff;  //[79:64]
            //    UInt32 datal6 = (UInt32)(data2Array[2] >> 16) & 0xffff;  //[95:80]
            //    UInt32 datal7 = (UInt32)data2Array[3] & 0xffff;  //[112:96]
            //    UInt32 datal8 = (UInt32)(data2Array[3] >> 16) & 0xffff;   //height[127:113]
            //    Int32[] Data1Array =
            //    {
            //        (Int32)0/*(ProtocolSysParameter.Default.D_PHYParameter.Data1&0xffffffff)*/,
            //        (Int32)0/*(ProtocolSysParameter.Default.D_PHYParameter.Data1>>32&0xffffffff)*/,
            //        (Int32)0/*(ProtocolSysParameter.Default.D_PHYParameter.Data2&0xffffffff)*/,
            //        (Int32)0/*(ProtocolSysParameter.Default.D_PHYParameter.Data2>>32&0xffffffff)*/,
            //    };
            //    Int32[] data1Array = MoveElements2Msb(Data1Array);//daataArray[0]-daataArray[3]
            //    UInt32 datah1 = (UInt32)data1Array[0] & 0xffff;   //low[15:0]
            //    UInt32 datah2 = (UInt32)(data1Array[0] >> 16) & 0xffff;   //[31:16]
            //    UInt32 datah3 = (UInt32)data1Array[1] & 0xffff;  //[47:32]
            //    UInt32 datah4 = (UInt32)(data1Array[1] >> 16) & 0xffff;  //[63:48]
            //    UInt32 datah5 = (UInt32)data1Array[2] & 0xffff;  //[79:64]
            //    UInt32 datah6 = (UInt32)(data1Array[2] >> 16) & 0xffff;  //[95:80]
            //    UInt32 datah7 = (UInt32)data1Array[3] & 0xffff;  //[112:96]
            //    UInt32 datah8 = (UInt32)(data1Array[3] >> 16) & 0xffff;   //height[127:113]


            //    UInt64 send_data = 0;

            //    ///////////////////////////////////////////////////////////////////////////////////////////////

            //    send_data = 0;//清零
            //    send_data |= HSPlus << 0;
            //    send_data |= HSDec << 5;
            //    send_data |= LPPlus << 10;
            //    send_data |= LPDec << 15;
            //    send_data |= CLKPlus << 20;
            //    send_data |= CLKDec << 25;
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSourceL, (UInt32)send_data & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSourceM, (UInt32)(send_data >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSourceH, (UInt32)(send_data >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）

            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data0L, datal1 & 0xffff);//low
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data0H, datah1 & 0xffff);//high
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data1L, datal2 & 0xffff);//low
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data1H, datah2 & 0xffff);//high
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data2L, datal3 & 0xffff);//low
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data2H, datah3 & 0xffff);//high
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data3L, datal4 & 0xffff);//low
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data3H, datah4 & 0xffff);//high
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data4L, datal5 & 0xffff);//low
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data4H, datah5 & 0xffff);//high
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data5L, datal6 & 0xffff);//low
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data5H, datah6 & 0xffff);//high
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data6L, datal7 & 0xffff);//low
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data6H, datah7 & 0xffff);//high
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data7L, datal8 & 0xffff);//low
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_Data7H, datah8 & 0xffff);//high

            //    //set
            //    send_data = 0;//清零
            //    send_data |= trig_condition << 0;
            //    send_data |= relation << 3;
            //    send_data |= byte_count << 5;
            //    send_data |= wc << 10;
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)send_data);//发送set[15:0]

            //    send_data = 0;  //清零
            //    send_data |= (wc >> 6) << 0;
            //    send_data |= dt << 10;
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)send_data);//发送set[31:17]

            //    send_data = 0;  //清零
            //    send_data |= vc << 0;
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)send_data);//发送set[47:32]

            //    HdIO.WriteReg(ProcBdReg.W.Decoder_RamResetEnable, 0x00000000);//复位（复位工作模块，清空缓冲区，DECODE_PROTOCOL_RST）
            //    HdIO.Sleep(1);
            //    HdIO.WriteReg(ProcBdReg.W.Decoder_RamResetEnable, 0x00000001);//复位（使能工作模块）
            return -1;
        }
    }
}

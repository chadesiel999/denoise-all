using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal partial class CtrlDecoder
    {
        internal static Int32 Config_Standard_AudioBus()
        {
            UInt32 decodechid = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);

            HdMessage.ProtocolI2SOptions? decodeoption = Hd.UIMessage!.Decoder![decodechid].ProtocolOptions! as HdMessage.ProtocolI2SOptions;

            HdMessage.TrigI2SConditionsOptions? trigoption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigI2SConditionsOptions;

           // Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 21);

            if (decodeoption == null || trigoption == null)
                return -1;


            #region 通道选择
            ChannelId scl = decodeoption!.SCL; //时钟线SCL
            if (scl.IsAnalog())//模拟通道
            {; }
            else if (scl.IsDigital())//数字通道
                scl -= 31;
            else
                scl = 0;
            ChannelId ws = decodeoption!.WS;//片选线WS
            if (ws.IsAnalog())//模拟通道
            {; }
            else if (ws.IsDigital())//数字通道
                ws -= 31;
            else
                ws = 0;
            ChannelId sda = decodeoption!.SDA;//数据线WS
            if (sda.IsAnalog())//模拟通道
            {; }
            else if (sda.IsDigital())//数字通道
                sda -= 31;
            else
                sda = 0;
            UInt64 sourcecontrolword = (UInt32)scl << 0 | (UInt32)ws << 5 | (UInt32)sda << 10;

            // 需要添加对应通道设置 To do


            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.AudioBus);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceL, (UInt32)sourcecontrolword);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceM, (UInt32)(sourcecontrolword >> 16));
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceH, 0x00);

            #endregion


            #region 触发参数设置
            //UInt32 clockedge = (UInt32)decodeOption.ClockEdge;//有效时钟沿         //1bit
            //UInt32 wsEdge = (UInt32)decodeOption.SyncPolarity; //ws 的信号极性     //1bit
            //UInt32 soundchannel = (UInt32)decodeOption.SoundChannel;//声道类型      //1bit //0; /
            //// 0831晚上只弄到这里  之后的数据需要确认
            //UInt32 datapolarity = (UInt32)(decodeOption.DataPolarity);//数据极性      //1bit
            //UInt32 msb_lsb = (UInt32)(decodeOption.MSB_LSB);                         //1bit    
            //UInt32 databitcount = (UInt32)(decodeOption.DataBitCount);               //6bit
            //UInt32 protocoltype = (UInt32)decodeOption.SubType;                      //2bit
            //uint channelnumberperfream = (uint)decodeOption.ChannelNumberPerFream; // [12:6] 7bit
            //uint clockBitNumberPerchannel = (uint)decodeOption.ClockBitNumberPerChannel; // 每通道时钟位数
            //uint bit_delay = (uint)decodeOption.BitDelayCount; // 触发位延迟[5:0] [37:32] 6bit
            //uint triggeraudioChannel = 0; // 触发通道0-32   [12:6] [44:38]  7bit  


            // UInt32 trigCondition;

            //if (trigOption != null)
            //{
            //    triggeraudioChannel = trigOption!.TDMChannelID;
            //    if (decodeOption.SubType == ProtocolAudioBus.SubType.TDM)
            //    {
            //        trigCondition = (UInt32)trigOption!.ConditionTDM;//触发条件
            //    }
            //    else
            //    {
            //        trigCondition = (UInt32)trigOption!.Condition;//触发条件
            //    }
            //    data = (UInt32)trigOption!.Data;
            //}
            //else
            //{
            //    trigCondition = 0;
            //    data = 0;
            //}
            //UInt64 trigControlWord = 0;
            //trigControlWord |= clockedge << 0;
            //trigControlWord |= wspolarity << 1;
            //trigControlWord |= soundchannel << 2;
            //trigControlWord |= datapolarity << 4;
            //trigControlWord |= msb_lsb << 5;
            //trigControlWord |= databitcount << 6;
            //trigControlWord |= protocoltype << 12; // 触发类型
            //trigControlWord |= trigCondition << 14; // 触发条件
            //trigControlWord |= clockBitNumberPerchannel << (16 + 5); // 每通道时钟位数
            //trigControlWord |= channelnumberperfream << (16 + 12); // 每帧通道数量
            //trigControlWord |= bit_delay << (32 + 5); // 触发位延迟
            //trigControlWord |= triggeraudioChannel << (32 + 12); // 触发通道

            #endregion


            UInt32 clockedge     = (UInt32)decodeoption.ClockEdge;              // 时钟边沿 bit 0
            UInt32 wspolarity    = (UInt32)decodeoption.SyncPolarity;          // 字选择极性 bit 1
            UInt32 datapolarity  = (UInt32)decodeoption.DataPolarity;        // 数据极性 bit 2
            UInt32 trigtype      = (UInt32)trigoption.Condition;             // 触发类型 bit 3-4 0 同步 1数据

            UInt32 bitorder      = (UInt32)decodeoption.MSB_LSB;                 // 位序 bit 5

            // To do
            UInt32 channel      = (UInt32)trigoption.SoundChannel;             // 两通道声道  bit 6-7  0 任意 1左 2 右


            if (decodeoption.SubType == ProtocolAudioBus.SubType.I2S)
            {
                if (trigoption.SoundChannel == ProtocolAudioBus.SoundChannel.Left)
                {
                    channel = 2;
                }
                else if (trigoption.SoundChannel == ProtocolAudioBus.SoundChannel.Right)
                {
                    channel = 1;
                }
                else
                {
                    channel = 0;
                }
            }





            UInt32 trigbit      = (UInt32)decodeoption.DataBitCount;            // 每个通道的数据位数  bit 8-13
            UInt32 protocoltype = (UInt32)decodeoption.SubType;                // 协议类型 14-15

            // TDM专属类型
            UInt32 datanum    = (UInt32)decodeoption.DataBitCount;              // 每个通道的数据位数
            UInt32 clocknum   = (UInt32)decodeoption.ClockBitCount; // 每个通道的时钟位数
            UInt64 channelnum = (UInt32)decodeoption.SoundChannelCount;    // 每个帧的通道数
            UInt64 bitdelay   = (UInt32)decodeoption.BitDelayCount;            // 位延迟
            UInt64 trigchannel= (UInt32)trigoption.TDMChannelID;              // TDM模式的触发通道号
            UInt64 trigset    = 0;                                             // bit 48  0 I2S触发,1 tdm触发

            // 触发数据值
            UInt32 data = 0; 
            if (decodeoption.SubType == ProtocolAudioBus.SubType.TDM)
            {
                 trigtype = (UInt32)trigoption.ConditionTDM;
                 trigset = 1; 
            }
            else
            {
                trigtype = (UInt32)trigoption.Condition;
                trigset = 0;
            }

            //trigOption.ByteCount;

            UInt32 data_mask = 0xFF_FF_FF_FF;

            //data_mask = data_mask << 16; // 32

            if (trigoption != null)
            {
                data = (UInt32)trigoption!.Data;

                // 如果是高位
                //if (decodeOption.MSB_LSB == ProtocolAudioBus.MSB_LSB.MSB)
                
                    data = data << (32 - trigoption.ByteCount * 4);
                    data_mask = data_mask >> (trigoption.ByteCount * 4);
                
                //else
                //{
                //    data_mask = data_mask << (trigOption.ByteCount * 4);
                //}
            }

            UInt64 trigControlWord = 0;
            trigControlWord |=  clockedge;
            trigControlWord |=  wspolarity << 1;
            trigControlWord |=  datapolarity << 2;
            trigControlWord |=  trigtype << 3;
            trigControlWord |=  bitorder << 5;
            trigControlWord |=  channel << 6;
            trigControlWord |=  trigbit << 8;
            trigControlWord |=  protocoltype << 14;

            trigControlWord |=  datanum << 16;
            trigControlWord |=  clocknum << 22;
            trigControlWord |=  channelnum << 28;
            trigControlWord |=  bitdelay << 35;
            trigControlWord |=  trigchannel << 41;
            trigControlWord |=  trigset << 48;

            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigControlWord & 0xFFFF));
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigControlWord >> 16) & 0xFFFF));//发送set[31:17]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigControlWord >> 32) & 0xFFFF));//发送set[47:32]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWord64, (UInt32)((trigControlWord >> 48) & 0xFFFF));//发送set[64:48] 


            //string binaryStr = Convert.ToString((long)trigControlWord, 2);
            //Hd.SysLogger?.Invoke(String.Format("Audio Trig Para：时钟沿: {0}，字选择极性：{1}，数据极性：{2}，触发类型：{3}, 位序：{4}，位序：{5}，" +
            //                     "声道类型：{6}，两通道数据位数：{7}，协议类型：{8}, tdm每个帧通道个数：{9}，tdm每个帧通道个数每通道时钟位数：{10}, tdm每个帧通道个数每字节数据位数:{11}, 位延迟:{12}, 通道序号:{13}, 触发设置：{14},cmd:{15}",
            //                         clockedge, wspolarity, datapolarity, trigtype, bitorder, bitorder, channel, trigbit, 
            //                         protocoltype, channelnum, clocknum, datanum, bitdelay, trigchannel,trigset, binaryStr), "Info");

            #region 触发数据设置
            uint dataBytesLength = 32; 
            UInt16[] TrigData = new UInt16[dataBytesLength];
            //Byte[] temp = BitConverter.GetBytes(data).Reverse().ToArray();

            //Unsafe.CopyBlock(ref Unsafe.As<UInt16, byte>(ref TrigData[0]), ref temp[0], (uint)temp.Length);

            TrigData[0] = (UInt16)(data & 0xffff);
            TrigData[1] = (UInt16)((data >> 16) & 0xffff);

            // 7000 存在的代码
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 0);



            TrigData[8] = (UInt16)(data_mask & 0xffff);
            TrigData[9] = (UInt16)((data_mask >> 16) & 0xffff);

            for (uint dataIndex = 0; dataIndex < dataBytesLength; dataIndex++)
            {

                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataL, (uint)TrigData[dataIndex] & 0xffff);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLIndex, dataIndex);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x01);//拉高数据触发的数据使能
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x00);//拉高数据触发的数据使能
            }
            #endregion

            return (Int32)decodechid;
        }

    }
}

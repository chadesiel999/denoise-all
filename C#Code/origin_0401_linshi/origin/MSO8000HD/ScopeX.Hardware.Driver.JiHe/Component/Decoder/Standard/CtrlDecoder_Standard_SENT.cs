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
        internal static Int32 Config_Standard_SENT()
        {
            UInt32 decode_ch_id = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);

            HdMessage.ProtocolSENTOptions? decodeoption = Hd.UIMessage!.Decoder![decode_ch_id].ProtocolOptions! as HdMessage.ProtocolSENTOptions;

            HdMessage.TrigSENTConditionsOptions? trigoption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigSENTConditionsOptions;

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 22);

            if (decodeoption == null || trigoption == null)
                return -1;

            //解码RAM预触发深度12bit
            UInt32 predepth = (UInt32)((Double)((UInt64)Hd.AnalogChannel!.AcquingParameters.SettingTrigPositionByfs / Hd.AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr) / (Constants.VIS_XDIVS_NUM * 1000) * 4096);
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth, predepth);

            #region 通道选择
            ChannelId source = decodeoption!.Source; //源输入通道ch[0]
            if (source.IsAnalog())//模拟通道
                source += 0;
            else if (source.IsDigital())//数字通道
                source -= 31;
            else
                source = 0;
            UInt64 source_control_word = (UInt32)source;

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect1Pro, (UInt32)source_control_word);
            #endregion

            #region 触发参数设置
            //set参数

            UInt32 pausebit = (UInt32)decodeoption.PauseBit;//停止脉冲 1位 0
            UInt32 channel_mode = (UInt32)trigoption.channelMode;
            UInt32 slow_enhanced_mode = (UInt32)(trigoption.channelMode == ProtocolSENT.ChannelMode.SlowChannel && trigoption.SlowMessageCondition == ProtocolSENT.SlowMessageCondition.EnhancedMessage && trigoption.SlowEnhancedMessageType == ProtocolSENT.SlowEnhancedMessageType.Enhanced4BitID ? 1 : 0);
            UInt32 sync_tolerance = (UInt32)decodeoption.Tolerance;
            UInt32 low_tolerance = 0;
            UInt32 datalength = trigoption.channelMode == ProtocolSENT.ChannelMode.FastChannel ? (UInt32)trigoption!.DataLength + 1 : (UInt32)ProtocolSENT.DataLength.Nibbles_6 + 1;//快速通道消息半字节数 4位 14~11
            UInt32 trigcondition = trigoption.channelMode == ProtocolSENT.ChannelMode.FastChannel ? (UInt32)trigoption!.fastCondition : (UInt32)trigoption.slowCondition;//触发条件 4位 18~15
            if (trigoption.channelMode == ProtocolSENT.ChannelMode.FastChannel)
            {
                datalength = (UInt32)decodeoption!.DataLength + 1;
                if (trigoption!.fastCondition == ProtocolSENT.FastCondition.Error)
                {
                    trigcondition += (UInt32)trigoption.FastError;
                }
            }

            if (trigoption.channelMode == ProtocolSENT.ChannelMode.SlowChannel && trigoption.SlowMessageCondition == ProtocolSENT.SlowMessageCondition.EnhancedMessage)
            {
                trigcondition += 4;
            }
            UInt32 messagetype = (UInt32)trigoption.SlowEnhancedMessageType; //慢速增强型消息类型 1位 22
            UInt64 data = trigoption.channelMode == ProtocolSENT.ChannelMode.FastChannel ? (UInt32)trigoption.FastChannelData : (UInt32)trigoption.SlowChannelData;
            UInt64 signalrate = decodeoption == null ? 0 : (UInt32)((Constants.PROT_SYS_CLOCK_HZ * (Double)decodeoption.ClockTick) * 1024);// 信号速率 公用 32位 63~31   
            UInt32 status = (UInt32)trigoption!.FastChannelStatus;
            UInt32 crc = trigoption.channelMode == ProtocolSENT.ChannelMode.FastChannel ? (UInt32)trigoption!.FastChannelCRC : (UInt32)trigoption!.SlowChannelCRC;


            UInt64 trig_control_word = 0;
            trig_control_word |= pausebit;
            //trig_control_word |= channel_mode << 1;//弃用
            trig_control_word |= sync_tolerance << 4;
            trig_control_word |= datalength << 11;
            trig_control_word |= trigcondition << 15;
            trig_control_word |= slow_enhanced_mode << 22;
            trig_control_word |= channel_mode << 23;
            trig_control_word |= signalrate << 31;

            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trig_control_word & 0xFFFF));
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trig_control_word >> 16) & 0xFFFF));//发送set[31:17]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trig_control_word >> 32) & 0xFFFF));//发送set[47:32]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWord64, (UInt32)((trig_control_word >> 48) & 0xFFFF));//发送set[64:48]  
            #endregion

            #region 触发数据设置
            UInt32 data_bytes_length = 32;
            UInt16[] trig_data_l = new UInt16[data_bytes_length];

            if (trigoption!.channelMode == ProtocolSENT.ChannelMode.FastChannel)
            {
                UInt32 status_cmp = status;
                UInt32 status_mask = 0X000;
                UInt64 data_cmp = data;
                UInt32 data_mask = (UInt32)(Math.Pow(2, ((Int32)ProtocolSENT.DataLength.Nibbles_6 + 1) * 4) - 1);
                data_mask = data_mask & (data_mask << (4 * ((Int32)trigoption!.DataLength + 1)));
                UInt32 crc_cmp = crc;
                UInt32 crc_mask = 0;

                trig_data_l[0] = (UInt16)((status_cmp | (status_mask << 4) | (data_cmp << 8)) & 0xffff);
                trig_data_l[1] = (UInt16)((data_cmp >> 8) & 0xffff);
                trig_data_l[2] = (UInt16)(data_mask & 0xffff);
                trig_data_l[3] = (UInt16)((data_mask >> 16) | (crc_cmp << 8) | (crc_mask << 12) & 0xffff);
            }
            else
            {
                UInt32 id_cmp = (UInt32)trigoption!.SlowChannelID;
                UInt32 id_mask = 0;
                UInt32 data_cmp = (UInt32)trigoption!.SlowChannelData;
                UInt32 data_mask = 0;
                UInt32 crc_cmp = (UInt32)trigoption!.SlowChannelCRC;
                UInt32 crc_mask = 0;
                if (trigoption.SlowMessageCondition == ProtocolSENT.SlowMessageCondition.ShortMessage)
                {
                    trig_data_l[0] = (UInt16)((id_cmp | (id_mask << 4) | (data_cmp << 8)) & 0xffff);
                    trig_data_l[1] = (UInt16)((data_mask | (crc_cmp << 8) | (crc_mask << 12)) & 0xffff);
                }
                else
                {
                    if (trigoption.SlowEnhancedMessageType == ProtocolSENT.SlowEnhancedMessageType.Enhanced4BitID)
                    {
                        id_mask = 0xf0;
                    }
                    else
                    {
                        data_mask = 0xf000;
                    }


                    trig_data_l[2] = (UInt16)((id_cmp | (id_mask << 8)) & 0xffff);
                    trig_data_l[3] = (UInt16)(data_cmp & 0xffff);
                    trig_data_l[4] = (UInt16)(data_mask & 0xffff);
                    trig_data_l[5] = (UInt16)((crc_cmp | (crc_mask << 6)) & 0xffff);
                }
            }
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 0);

            for (UInt32 dataIndex = 0; dataIndex < data_bytes_length; dataIndex++)
            {
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataL, (uint)trig_data_l[dataIndex] & 0xffff);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLIndex, dataIndex);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x01);//拉高数据触发的数据使能
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x00);//拉高数据触发的数据使能
            }
            #endregion
            return (Int32)decode_ch_id;
        }
    }
}

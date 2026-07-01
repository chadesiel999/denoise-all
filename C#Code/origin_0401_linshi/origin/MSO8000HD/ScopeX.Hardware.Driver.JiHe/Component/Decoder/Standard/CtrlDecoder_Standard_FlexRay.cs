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
        internal static Int32 Config_Standard_FlexRay()
        {
            UInt32 decode_ch_id = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);

            HdMessage.ProtocolFlexRayOptions? decodeOption = Hd.UIMessage!.Decoder![decode_ch_id].ProtocolOptions! as HdMessage.ProtocolFlexRayOptions;

            HdMessage.TrigFlexRayConditionOptions? trigOption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigFlexRayConditionOptions;

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 17);

            if (decodeOption == null || trigOption == null)
                return -1;

            //解码RAM预触发深度12bit
            UInt32 predepth = (UInt32)((Double)((UInt64)Hd.AnalogChannel!.AcquingParameters.SettingTrigPositionByfs / Hd.AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr) / (Constants.VIS_XDIVS_NUM * 1000) * 4096);
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth, predepth);

            #region 通道选择
            ChannelId source = decodeOption!.Source; //源输入通道ch[0]
            if (source.IsAnalog())//模拟通道
                source += 0;
            else if (source.IsDigital())//数字通道
                source -= 31;
            else
                source = 0;
            UInt64 source_control_word = (UInt32)source;

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect1Pro, (UInt32)source);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect2Pro, (UInt32)clk_input);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect3Pro, (UInt32)mosi_input);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.FlexRay);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceL, (UInt32)source_control_word);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceM, (UInt32)(source_control_word >> 16));
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceH, 0x00);
            #endregion


            #region 触发参数设置                                                                           
            UInt32 trigcondition = (UInt32)trigOption.Condition;//触发类型 3~0
            UInt32 channeltype = (UInt32)decodeOption.ChannelType;//通道类型 4
            UInt32 sourcetype = (UInt32)decodeOption.SourceType;//源类型 需要修改 5           
            UInt32 datatriglenth = (UInt32)trigOption.ByteCount + 1;//触发数据长度（1~16） 5位 10~6
            UInt32 byteoffset = (UInt32)(trigOption.ByteOffset);//字节偏置 6位 18~11
            UInt32 no_offset = (UInt32)(trigOption.HasDataOffset ? 0 : 1);//有无字节偏置 1位 19
            UInt32 frametrigtype = (UInt32)trigOption.Indicator;//帧类型触发 0为正常（01xx）,1为净载荷（11xx）,2为空（00xx）,3为同步（xx10）,4为启动（xx11） 3位 22~20
            UInt32 trig_cycle_cmp = (UInt32)trigOption.CycleData;//循环数 6位 28~23
            UInt32 frame_kind = (UInt32)trigOption.FrameTail;//帧类型 0静态  1动态  2 all 2位  30~29
            UInt32 frame_error_kind = (UInt32)trigOption.FrameError;//帧错误类型 0为标头CRC错误 ，1为帧尾CRC错误，2为空帧静态， 3为空帧动态，4为同步帧，5为启动帧无同步  3位 35~33
            //UInt32 payloadlenth = (UInt32)trigOption.Payload;//净荷长 界面需要注释
            //UInt32 datarelation = (UInt32)trigOption.Relation;//数据限定符  界面需要注释
            //UInt32 FrameTail1 = (UInt32)trigOption.FrameTail;//数据限定符  界面需要注释

            UInt64 trig_control_word = 0;
            trig_control_word |= trigcondition << 0;  //触发方式
            trig_control_word |= channeltype << 4;
            trig_control_word |= sourcetype << 5;//源类型
            trig_control_word |= datatriglenth << 6; //触发数据长度
            trig_control_word |= byteoffset << 11; //字节偏置
            trig_control_word |= no_offset << 19;
            trig_control_word |= frametrigtype << 20;//帧类型触发
            trig_control_word |= trig_cycle_cmp << 23;
            trig_control_word |= frame_kind << 29;
            trig_control_word |= (UInt64)frame_error_kind << 33;

            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trig_control_word & 0xFFFF));
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trig_control_word >> 16) & 0xFFFF));//发送set[31:17]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trig_control_word >> 32) & 0xFFFF));//发送set[47:32]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWord64, (UInt32)((trig_control_word >> 48) & 0xFFFF));//发送set[64:48]   
            #endregion

            #region 触发数据设置

            UInt32 dataBytesLength = 32;
            UInt16[] TrigDataL = new UInt16[dataBytesLength];

            UInt64 signalRate = decodeOption == null ? 0 : (UInt32)((Constants.PROT_SYS_CLOCK_HZ / (Double)decodeOption.SignalRate) * 1024);// bps信号速率 32位 127~0
            UInt32 id_cmp = (UInt32)trigOption.ID;//11位0~7_FF 138~128
            UInt32 id_mask = 0;//id掩码 11位，默认0 159~139           
            UInt64 field_cmp = (UInt32)trigOption.IndicatorData;        //标头字段（5指示位+11标识符id+7净哉荷长度+11头CRC+10循环数）40位 197~158
            UInt64 field_mask = 0;        //标头字段掩码 默认0（5指示位+11标识符id+7净哉荷长度+11头CRC+6循环数）40位 197~158

            Byte[] data_cmp = new Byte[16];//数据 128位 383~256
            Byte[] data_mask = new Byte[16];//数据掩码 128 默认0 511~384 内部自适应发0就行
            var temp = trigOption!.Data!.Reverse().ToArray();
            if (trigOption?.Data != null && trigOption.Data.Length > 0)
            {
                //Byte[] temp = trigOption.Data.Reverse().ToArray();
                Unsafe.CopyBlock(ref data_cmp[data_cmp.Length - temp.Length], ref temp[0], (UInt32)temp.Length);
                //Unsafe.CopyBlock(ref data_cmp[data_cmp.Length - trigOption.Data.Length], ref trigOption.Data[0], (UInt32)trigOption.Data.Length);
            }

            field_cmp = (field_cmp << 11) + (UInt64)trigOption!.ID;
            field_cmp = (field_cmp << 7) + (UInt64)trigOption.Payload;
            field_cmp = (field_cmp << 11) + (UInt64)trigOption.HeaderCRC;
            field_cmp = (field_cmp << 6) + (UInt64)trigOption.CycleData;

            if (trigOption != null)
            {
                TrigDataL[0] = (UInt16)(signalRate & 0xffff);
                TrigDataL[1] = (UInt16)((signalRate >> 16) & 0xffff);

                TrigDataL[8] = (UInt16)(id_cmp | (id_mask << 11) & 0xffff);
                TrigDataL[9] = (UInt16)((id_mask >> 5) | (field_cmp << 14) & 0xffff);
                TrigDataL[10] = (UInt16)((field_cmp >> 2) & 0xffff);
                TrigDataL[11] = (UInt16)((field_cmp >> 18) & 0xffff);
                TrigDataL[12] = (UInt16)((field_cmp >> 34) | (field_mask << 6) & 0xffff);
                TrigDataL[13] = (UInt16)((field_mask >> 10) & 0xffff);
                TrigDataL[14] = (UInt16)((field_mask >> 26) & 0xffff);

                TrigDataL[23] = (UInt16)(data_cmp[0] << 8 | data_cmp[1]);
                TrigDataL[22] = (UInt16)(data_cmp[2] << 8 | data_cmp[3]);
                TrigDataL[21] = (UInt16)(data_cmp[4] << 8 | data_cmp[5]);
                TrigDataL[20] = (UInt16)(data_cmp[6] << 8 | data_cmp[7]);
                TrigDataL[19] = (UInt16)(data_cmp[8] << 8 | data_cmp[9]);
                TrigDataL[18] = (UInt16)(data_cmp[10] << 8 | data_cmp[11]);
                TrigDataL[17] = (UInt16)(data_cmp[12] << 8 | data_cmp[13]);
                TrigDataL[16] = (UInt16)(data_cmp[14] << 8 | data_cmp[15]);

                //Unsafe.CopyBlock(ref Unsafe.As<UInt16, Byte>(ref TrigDataL[16]), ref data_cmp[0], (UInt32)(Unsafe.SizeOf<Byte>()*data_cmp.Length));
                //Unsafe.CopyBlock(ref Unsafe.As<UInt16, Byte>(ref TrigDataL[24]), ref data_mask[0], (UInt32)(Unsafe.SizeOf<Byte>() * data_mask.Length));
            }

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 0);

            for (UInt32 dataIndex = 0; dataIndex < dataBytesLength; dataIndex++)
            {
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataL, (uint)TrigDataL[dataIndex] & 0xffff);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLIndex, dataIndex);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x01);//拉高数据触发的数据使能
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x00);//拉高数据触发的数据使能
            }

            #endregion

            return (Int32)decode_ch_id;
        }
    }
}

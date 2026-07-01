using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal partial class CtrlDecoder


    {
        internal static Int32 Config_Standard_LIN()
        {
            UInt32 decodechid = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);

            HdMessage.ProtocolLINOptions? decodeoption = Hd.UIMessage!.Decoder![decodechid].ProtocolOptions! as HdMessage.ProtocolLINOptions;

            HdMessage.TrigLINConditionsOptions? trigoption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigLINConditionsOptions;

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 16);

            if (decodeoption == null || trigoption == null)
                return -1;

            //解码RAM预触发深度12bit
            UInt32 predepth = (UInt32)((Double)((UInt64)Hd.AnalogChannel!.AcquingParameters.SettingTrigPositionByfs / Hd.AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr) / (Constants.VIS_XDIVS_NUM * 1000) * 4096);
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth, predepth);
           // ProtocolCANFD.SignalType signalType = decodeoption!.SignalType;

            #region 通道选择
            ChannelId source = decodeoption!.Source; //源输入通道ch[0]
            if (source.IsAnalog())//模拟通道
                source += 0;
            else if (source.IsDigital())//数字通道
                source -= 31;
            else
                source = 0;
            UInt64 sourcecontrolword = (UInt32)source ;

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.LIN);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceL, (UInt32)sourcecontrolword);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceM, (UInt32)(sourcecontrolword >> 16));
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceH, 0x00);

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect1Pro, (UInt32)source);
            #endregion

            /*
                        #region 触发参数设置
                        UInt32 lin_kind = 0;//信号类型   0:L 1:h 
                        UInt32 polarity = (UInt32)decodeOption.Polarity;//极性
                        UInt32 datarelation = (UInt32)trigOption.DataRelation;//数据限定符
                        UInt32 trig_condition = (UInt32)trigOption.Condition; //触发方式                       
                        UInt32 datacount = (UInt32)(trigOption.ByteCount);//compares字节数 
                        UInt32 err_kind = 0;//错误类型 0  1 2
                        UInt32 dlc_set = 0;//id的5 6位是否包括数据长度 0包括   1不包括
                        UInt32 version = (UInt32)decodeOption.Standard;//LIN版本
                       //UInt64 signalRate = (Constants.PROT_SYS_CLOCK_HZ / (UInt32)decodeOption.BPS);  //信号速率（波特率计数值）    
                        UInt64 signalRate = decodeOption == null ? 0 : (UInt32)((Constants.PROT_SYS_CLOCK_HZ/(Double)decodeOption.BPS)*1024);//can bps信号速率
                        UInt64 trigControlWord = 0;

                        trigControlWord |= lin_kind ;  
                        trigControlWord |= polarity << 2; 
                        trigControlWord |= datarelation << 4;
                        trigControlWord |= trig_condition << 8;
                        trigControlWord |= datacount << 11;  
                        trigControlWord |= err_kind << 15;  
                        trigControlWord |= dlc_set << 17;  
                        trigControlWord |= version << 18;  
                        trigControlWord |= signalRate << 32; //波特率

                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordL, (uint)(trigControlWord & 0xFFFF));
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordM, (uint)((trigControlWord >> 16) & 0xFFFF));//发送set[31:17]
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordH, (uint)((trigControlWord >> 32) & 0xFFFF));//发送set[47:32]
                                                                                                                                             //最高的48~64位暂时寄存器缺失，没有连接
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_0, (uint)((trigControlWord >> 48) & 0xFFFF));//发送set[63:48]
                        #endregion
            */

            // 信号类型  
            UInt32 lin_kind = (UInt32)decodeoption.Polarity;        // lin协议类型
            UInt32 polarity = 0;        // 有奇偶校验，无奇偶校验
            UInt32 data_length = 8;     // id数据length  包含id为8位，不包含则为6位
            UInt32 trig_type = (UInt32)trigoption.Condition;       // 触发类型 0：同步 1：id触发 ...
            UInt32 trig_byte = (UInt32)(trigoption.ByteCount);       // 触发字节数
            UInt32 error_type = 0;      // 错误类型                            
            UInt32 include_parity_id = (UInt32)(decodeoption.PIncludeOddEven == ProtocolLIN.PIncludeOddEven.Y ? 1 : 0);   // ID是否包含校验位
            UInt32 lin_version = (UInt32)decodeoption.Standard;         // LIN版本类型

            UInt64 signal_rate = decodeoption == null ? 0 : (UInt32)((Constants.PROT_SYS_CLOCK_HZ / (Double)decodeoption.BPS) * 1024);// bps信号速率

            UInt64 trigControlWord = 0;

            if (trig_type == 6)
                error_type = 0;
            else if (trig_type == 7)
                error_type = 1;
            else if (trig_type == 8)
                error_type = 2;

            if (trig_type >= 6)
                trig_type = 6;



            trigControlWord |= lin_kind;
            trigControlWord |= polarity << 2;
            trigControlWord |= data_length << 4;
            trigControlWord |= trig_type << 8;
            trigControlWord |= trig_byte << 11;
            trigControlWord |= error_type << 15;
            trigControlWord |= include_parity_id << 17;
            trigControlWord |= lin_version << 18;
            trigControlWord |= signal_rate << 32; //波特率


            //string binaryStr = Convert.ToString((long)trigControlWord, 2);

            //Hd.SysLogger?.Invoke(String.Format("Lin Trig Para：lin_kind: {0}，polarity：{1}，data_length：{2}，trig_type：{3}，trig_byte：{4}，error_type：{5}，include_parity_id：{6}，lin_version：{7}, signal_rate:{8}, cmd:{9}",
            //                       lin_kind, polarity, data_length, trig_type, trig_byte, error_type, include_parity_id, lin_version, signal_rate, binaryStr),"Info");

            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigControlWord & 0xFFFF));
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigControlWord >> 16) & 0xFFFF));//发送set[31:17]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigControlWord >> 32) & 0xFFFF));//发送set[47:32]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWord64, (UInt32)((trigControlWord >> 48) & 0xFFFF));//发送set[64:48] 

            #region 触发数据设置

            uint dataBytesLength = 32; // 以前32 目前修改为16
            UInt16[] TrigDataL = new UInt16[dataBytesLength];

            UInt32 id_cmp =  (UInt32)trigoption.ID;
            UInt64 data_cmp = BitConverter.ToUInt64(BitConverter.GetBytes((UInt64)trigoption.Data).ToArray());
            UInt32 id_mask = 0x0;
            UInt64 data_mask = 0xFFFFFFFFFFFFFFFF;

            data_mask = (data_mask << trigoption.ByteCount * 8);

            if (trigoption != null)
            {
          
                TrigDataL[0] = (UInt16)(id_cmp & 0xffff); // 0-16

                //Unsafe.CopyBlock(ref Unsafe.As<UInt16, Byte>(ref TrigDataL[4]), ref Unsafe.As<UInt64, Byte>(ref data_cmp), (uint)Unsafe.SizeOf<UInt64>());
                //TrigDataL[4] = (UInt16)(data_cmp & 0xffff);
                //TrigDataL[5] = (UInt16)((data_cmp >> 16) & 0xffff);
                //TrigDataL[6] = (UInt16)((data_cmp >> 32) & 0xffff);
                //TrigDataL[7] = (UInt16)((data_cmp >> 48) & 0xffff);

                TrigDataL[4] = (UInt16)(data_cmp & 0xffff);             // 64 
                TrigDataL[5] = (UInt16)((data_cmp >> 16) & 0xffff);
                TrigDataL[6] = (UInt16)((data_cmp >> 32) & 0xffff);
                TrigDataL[7] = (UInt16)((data_cmp >> 48) & 0xffff);    // 128 

                TrigDataL[8] = (UInt16)(id_mask & 0xffff);  // 128 - 144
                // Unsafe.CopyBlock(ref Unsafe.As<UInt16, Byte>(ref TrigDataL[12]), ref Unsafe.As<UInt64, Byte>(ref data_mask), (uint)Unsafe.SizeOf<UInt64>());
                //TrigDataL[12] = (UInt16)(data_mask& 0xffff);              
                //TrigDataL[13] = (UInt16)((data_mask>>16)& 0xffff);
                //TrigDataL[14] = (UInt16)((data_mask>>32)& 0xffff);
                //TrigDataL[15] = (UInt16)((data_mask >> 48) & 0xffff);


                TrigDataL[12] = (UInt16)(data_mask & 0xffff);               // 192 
                TrigDataL[13] = (UInt16)((data_mask >> 16) & 0xffff);
                TrigDataL[14] = (UInt16)((data_mask >> 32) & 0xffff);
                TrigDataL[15] = (UInt16)((data_mask >> 48) & 0xffff);

            }
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 0);

            for (UInt32 dataindex = 0; dataindex < dataBytesLength; dataindex++)
            {
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataL, (UInt32)TrigDataL[dataindex] & 0xffff);//数据触发的数据

                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLIndex, dataindex);//数据触发的地址

                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLValid, 0x01);//拉高数据触发的数据使能
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLValid, 0x00);//拉高数据触发的数据使能

                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataL, (uint)TrigDataL[dataindex] & 0xffff);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLIndex, dataindex);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x01);//拉高数据触发的数据使能
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x00);//拉高数据触发的数据使能
            }

            #endregion

            return (Int32)decodechid;
        }
    }
}

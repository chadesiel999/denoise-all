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
        internal static Int32 Config_Standard_SATA()
        {
            
            UInt32 decodechid = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);
            /*
            HdMessage.ProtocolSATAOptions? decodeoption = Hd.UIMessage!.Decoder![decodechid].ProtocolOptions! as HdMessage.ProtocolSATAOptions;

            HdMessage.TrigSATAConditionsOptions? trigoption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigSATAConditionsOptions;

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 12);//未开放，待定

            if (decodeoption == null || trigoption == null)
                return -1;

            //解码RAM预触发深度12bit
            //UInt32 predepth = (UInt32)(((Double)Acquire.Default.XPhySys.Position / GRID_WIDTH) * DecodePackageQueueBase.DECODE_BUF_SIZE);
            UInt32 predepth = (UInt32)((Double)((UInt64)Hd.AnalogChannel!.AcquingParameters.SettingTrigPositionByfs / Hd.AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr) / (Constants.VIS_XDIVS_NUM * 1000) * 4096);
            //HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth, predepth);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_RamPreDepth, predepth);
            #region 通道选择
            ChannelId source = decodeoption!.Source; //源输入通道ch[0]
            if (source.IsAnalog())//模拟通道
                source += 1;
            else if (source.IsDigital())//数字通道
                source -= 31;
            else
                source = 0;
            ChannelId signal_source = decodeoption.Source1;//信号源（信号类型）
            if (signal_source.IsAnalog())//模拟通道
                signal_source += 1;
            else if (signal_source.IsDigital())//数字通道
                signal_source -= 31;
            else
                signal_source = 0;
            UInt64 sourcecontrolword = (UInt32)source << 0 | (UInt32)signal_source << 6;
            //sourcecontrolword = 0x02;
            switch (decodechid)
            {
                case 0:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1Type, (UInt32)SerialProtocolType.SATA);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.SATA);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2Enable, 0x00);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1Enable, 0x01);
                    //HdIO.Sleep(1);
                    //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1Enable, 0x00);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceL, (UInt32)sourcecontrolword & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceM, (UInt32)(sourcecontrolword >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceH, (UInt32)(sourcecontrolword >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                    break;
                case 1:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2Type, (UInt32)SerialProtocolType.SATA);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.SATA);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1Enable, 0x00);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2Enable, 0x01);
                    //HdIO.Sleep(1);
                    //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2Enable, 0x00);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2SignalSourceL, (UInt32)sourcecontrolword & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2SignalSourceM, (UInt32)(sourcecontrolword >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2SignalSourceH, (UInt32)(sourcecontrolword >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                    break;
                default: break;
            }
            #endregion

            #region 触发参数设置
            //set参数
            //UInt32 datalen = trigoption == null ? 0 : (UInt32)trigoption.DataCount;//字节长度（“数据触发”时的字节数）
            UInt32 datalen =0x10 ;//字节长度（“数据触发”时的字节数）
            UInt32 trig_datalen = trigoption == null ? 0 : (UInt32)trigoption.DataCount;//字节长度（“数据触发”时的字节数）
            ProtocolSATA.FISTypeFlag fistype = trigoption == null ? 0 : trigoption.FISType;
            //触发条件
            UInt32 trigcondition = trigoption == null ? 0 : (UInt32)trigoption!.Condition;//触发条件
            UInt32 relation = trigoption == null ? 0 : (UInt32)trigoption.Relation; //限定符
            UInt64 trigcontrolword = 0;
            trigcontrolword |= trigcondition << 0;
            trigcontrolword |= relation << 5;
            trigcontrolword |= (UInt32)fistype << 8;
            trigcontrolword |= datalen << 13;
            trigcontrolword |= trig_datalen << 24;
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigcontrolword & 0xFFFF));//发送set[15:0]
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigcontrolword >> 16) & 0xFFFF));//发送set[31:17]
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigcontrolword >> 32) & 0xFFFF));//发送set[45:32]
            #endregion
            #region 触发数据设置

            //触发条件选择“数据”触发时使用的data
            Int64 addressdata = trigoption == null ? 0 : trigoption.Data;
            Int64 data = trigoption == null ? 0 : trigoption.Data;
            UInt32 databyteslength = 16;
            UInt16[] trigdatal = new UInt16[databyteslength];
            UInt16[] trigdatah = new UInt16[databyteslength];
            trigdatah[0] = (UInt16)(data & 0xffff);
            trigdatah[1] = (UInt16)((data >> 16) & 0xffff);
            trigdatah[2] = (UInt16)((data >> 32) & 0xffff);
            trigdatah[3] = (UInt16)((data >> 48) & 0xffff);
            trigdatah[4] = 0;
            trigdatah[5] = 0;
            trigdatah[6] = 0;
            trigdatah[7] = 0;
            for (UInt32 dataindex = 0; dataindex < databyteslength; dataindex++)
            {
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataL, (UInt32)trigdatal[dataindex] & 0xffff);//数据L
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLIndex, dataindex);//数据L索引
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLValid, 0x01);//数据L使能
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLValid, 0x00);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataH, (UInt32)trigdatah[dataindex] & 0xffff);//数据H
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataHIndex, dataindex);//数据H索引
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataHValid, 0x01);//数据H使能
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataHValid, 0x00);
            }
              
            #endregion
            */
            return (Int32)decodechid;
        }
    }
}

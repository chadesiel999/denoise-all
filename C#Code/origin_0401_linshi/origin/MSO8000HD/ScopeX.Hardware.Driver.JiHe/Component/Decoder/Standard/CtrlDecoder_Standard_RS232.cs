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
        internal static Int32 Config_Standard_RS232()
        {
            UInt32 decodechid = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);

            HdMessage.ProtocolRS232Options? decodeoption = Hd.UIMessage!.Decoder![decodechid].ProtocolOptions! as HdMessage.ProtocolRS232Options;

            HdMessage.TrigRS232ConditionsOptions? trigoption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigRS232ConditionsOptions;

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 12);

            if (decodeoption == null || trigoption == null)
                return -1;


            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth, predepth);

            #region 通道选择
            ChannelId source = decodeoption!.Source; //源输入通道ch[0]
            if (source.IsAnalog())//模拟通道
            {; } 
            else if (source.IsDigital())//数字通道
                source -= 31;
            else
                source = 0;
            ChannelId sourceL = decodeoption!.SourceL;//源L输入通道ch[1]
            if (sourceL.IsAnalog())//模拟通道
            {; }
            else if (sourceL.IsDigital())//数字通道
                sourceL -= 31;
            else
                sourceL = 0;
          //  UInt64 sourcecontrolword = (UInt32)source << 0 | (UInt32)sourceL << 6;
            UInt64 sourcecontrolword = (UInt32)source << 0 ; //(放弃sourceL)
            switch (decodechid)
            {
                case 0:
                case 1:
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect1Pro, (UInt32)sourcecontrolword);
                    break;
                case 2:
                   // HdIO.WriteReg(ProcBdReg.W.Decoder_TypeB2, (UInt32)SerialProtocolType.RS232);
                   // HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger,(UInt32)SerialProtocolType.RS232);
                   // HdIO.WriteReg(ProcBdReg.W.Decoder_B1Enable, 0x00);
                   // HdIO.WriteReg(ProcBdReg.W.Decoder_B2Enable, 0x01);
                   //
                   // HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_L, (UInt32)sourcecontrolword & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                   // HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_M, (UInt32)(sourcecontrolword >> 16) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                   // HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_H, (UInt32)(sourcecontrolword >> 32) & 0xffff);//通道选择参数（DECODE_SET_PROTOCOL_CHANNEL）
                    break;
                default: break;
            }
            #endregion




            //#region 触发参数设置
            //UInt32 signalType = (UInt32)decodeOption.SignalType;//信号源（信号类型单端/差分）
            //UInt32 dataBitWidth = (UInt32)decodeOption.DataBitWidth;//数据位宽
            //UInt32 parityBit = (UInt32)decodeOption.OddEvenCheck;//校验位

            //UInt32 stopBit = 0;//(UInt32)(decodeOption.StopBit);//停止位
            //UInt32 bitSeq = (UInt32)decodeOption.BitSeq; //位序
            //UInt32 polarity = (UInt32)(decodeOption.Polarity == ProtocolCommon.Polarity.Positive ? 1 : 0);//极性
            //UInt64 baudbps = (ulong)(Constants.PROT_SYS_CLOCK_HZ / (double)decodeOption.Baud * 1024);//波特率
            //UInt32 trigCondition;
            //UInt32 data;
            //if (trigOption != null)
            //{
            //    trigCondition = (UInt32)trigOption!.Conditions;//触发条件
            //    data = (UInt32)trigOption!.Data;
            //    if (trigOption!.Conditions == ProtocolRS232.Conditions.PackageEnd)
            //        data = (UInt32)trigOption!.EOPChar;
            //}
            //else
            //{
            //    trigCondition = 0;
            //    data = 0;
            //}
            //UInt64 trigControlWord = 0;
            //trigControlWord |= polarity << 0;   //1bit
            //trigControlWord |= bitSeq << 1;     //1bit
            //trigControlWord |= stopBit << 2;    //1bit
            //trigControlWord |= parityBit << 3;  //2bit
            //trigControlWord |= trigCondition << 5; //2bit
            //trigControlWord |= dataBitWidth << 7;//4bit
            //trigControlWord |= baudbps << 16;  //32bit

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordL, (uint)(trigControlWord & 0xFFFF));
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordM, (uint)((trigControlWord >> 16) & 0xFFFF));//发送set[31:17]
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigControlWordH, (uint)((trigControlWord >> 32) & 0xFFFF));//发送set[45:32]


            //#endregion

            UInt32 datapolarity = (UInt32)decodeoption.Polarity;     // 数据极性 bit 0
            UInt32 bitorder = (UInt32)((decodeoption.BitSeq == ProtocolRS232.MSB_LSB.LSB) ? 0 :1);       // 位序 bit 1
            UInt32 stopbitwidth = (UInt32)decodeoption.StopBit;       // 停止位 位宽 bit 2
            UInt32 paritytype = (UInt32)decodeoption.OddEvenCheck; // 奇偶性  bit 3-4  0 None,     1 Odd,   2 Even
            UInt32 trigtype = (UInt32)trigoption.Conditions;     // 触发类型 bit 5-6
            UInt32 datalength = (UInt32)decodeoption.DataBitWidth;   // 数据长度 bit 7-10
            UInt64 baudbps = (ulong)(Constants.PROT_SYS_CLOCK_HZ / (double)decodeoption.Baud * 1024);//波特率 bit 16-47


            UInt64 trigcontrolword = 0;
            trigcontrolword |= datapolarity << 0;   //1bit
            trigcontrolword |= bitorder << 1;   //1bit
            trigcontrolword |= stopbitwidth << 2;   //1bit
            trigcontrolword |= paritytype << 3;   //1bit
            trigcontrolword |= trigtype << 5;   //1bit
            trigcontrolword |= datalength << 7;   //1bit
            trigcontrolword |= baudbps << 16;   //1bit


            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigcontrolword & 0xFFFF));
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigcontrolword >> 16) & 0xFFFF));//发送set[31:17]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigcontrolword >> 32) & 0xFFFF));//发送set[47:32]


            //string binaryStr = Convert.ToString((long)trigcontrolword, 2);
            //Hd.SysLogger?.Invoke(String.Format("Rs232 Trig Para：数据极性: {0}，位序：{1}，停止位：{2}，奇偶性：{3}, 触发类型：{4}，数据长度：{5}，波特率：{6},  cmd:{7}",
            //                         datapolarity, bitorder, stopbitwidth, paritytype, trigtype, datalength, baudbps, binaryStr), "Info");

            // 数据
            UInt64 data = trigoption!.Data;
            UInt64 data_mask = 0xFFFFFFFFFFFFFFFF;

            data_mask = data_mask << 8;

            #region 触发数据设置
            uint dataBytesLength = 32; // 32改为16
            UInt16[] TrigData = new UInt16[dataBytesLength];

            TrigData[0] = (UInt16)(data & 0xffff);
            //TrigData[1] = (UInt16)((data >> 16) & 0xffff);

            TrigData[8] = (UInt16)(data_mask & 0xffff);
            //TrigData[1] = (UInt16)((data >> 16) & 0xffff);

            // 拷贝数据
            //Unsafe.CopyBlock(ref Unsafe.As<UInt16, byte>(ref TrigDataL[0]), ref Unsafe.As<UInt64, byte>(ref data), (uint)Unsafe.SizeOf<UInt64>());


            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 0);

            for (UInt32 dataindex = 0; dataindex < dataBytesLength; dataindex++)
            {
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataL, (uint)TrigData[dataindex] & 0xffff);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLIndex, dataindex);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x01);//拉高数据触发的数据使能
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x00);//拉高数据触发的数据使能
            }
            #endregion
            return (Int32)decodechid;
        }
    }
}

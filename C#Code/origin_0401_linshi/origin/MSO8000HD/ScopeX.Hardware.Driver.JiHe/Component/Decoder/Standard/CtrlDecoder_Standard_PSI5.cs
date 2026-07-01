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
        internal static Int32 Config_Standard_PSI5()
        {
            UInt32 decodeChID = (UInt32)((int)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (int)ChannelIdExt.MinBChId);

            HdMessage.ProtocolPSI5Options? decodeOption = Hd.UIMessage!.Decoder![decodeChID].ProtocolOptions! as HdMessage.ProtocolPSI5Options;

            HdMessage.TrigPSI5ConditionsOptions? trigOption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigPSI5ConditionsOptions;

            //硬件对应25
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 25);

            if (decodeOption == null || trigOption == null)
                return -1;

            #region 通道选择
            ChannelId source = decodeOption!.Source; //源输入通道ch[0]
            if (source.IsAnalog())//模拟通道
            {; }
            else if (source.IsDigital())//数字通道
                source -= 31;
            else
                source = 0;
            UInt64 sourceControlword = (UInt32)source; //<< 0; 
            //总线里面协议对应12
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.PSI5);

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect1Pro, (UInt32)source);

            #endregion


            #region 触发参数设置
            UInt64 baudbps = (UInt64)(Constants.PROT_SYS_CLOCK_HZ / (Double)decodeOption.Psi5BaudMode);//波特率
            UInt32 serialMessage = (UInt32)(decodeOption.Psi5SerialMessage);//串行通道使能
            UInt32 dataASize = (UInt32)decodeOption.DataASize; //dataA位数 10-28
            UInt32 dataBSize = (UInt32)decodeOption.DataBSize; //dataB位数 0-10
            UInt32 frameControlBits = (UInt32)decodeOption.Psi5FrameControl;//帧控制位数 0-4
            UInt32 status = (UInt32)decodeOption.Psi5Status;//状态位数
            UInt64 idletime = 0x3652; //* (1.0/baudbps); //空闲时间
            UInt32 trig_condition = (UInt32)trigOption.Condition; //触发方式                       
            UInt32 Polarity = 0x1;
           


            UInt64 trigControlWord = 0;
            trigControlWord |= serialMessage << 0;   //1bit
            trigControlWord |= status << 1;
            trigControlWord |= frameControlBits << 3;
            trigControlWord |= dataBSize << 6;
            trigControlWord |= dataASize << 10;
            trigControlWord |= baudbps << 15;  //32bit
            trigControlWord |= Polarity << 27;
            trigControlWord |= trig_condition << 28;//3bit 
            trigControlWord |= idletime << 33;//3bit 

            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigControlWord & 0xFFFF));
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigControlWord >> 16) & 0xFFFF));//发送set[31:17]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigControlWord >> 32) & 0xFFFF));//发送set[47:32]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWord64, (UInt32)((trigControlWord >> 48) & 0xFFFF));//发送set[64:48]  

            #endregion



            #region 触发数据设置
            //UInt32 id_mask = 0;
            //UInt64 data_mask = 0;
            UInt32 trig_data;
            switch (trig_condition)
            {
                case (Int32)ProtocolPSI5.Condition.DataA:
                    trig_data = (UInt32)trigOption!.DataAValue;
                    break;
                case (Int32)ProtocolPSI5.Condition.DataB:
                    trig_data = (UInt32)trigOption!.DataBValue;
                    break;
                case (Int32)ProtocolPSI5.Condition.Block_ID:
                    trig_data = (UInt32)trigOption!.BlockID;
                    break;
                case (Int32)ProtocolPSI5.Condition.Sensor_Status:
                    trig_data = (UInt32)trigOption!.SensorStatus;
                    break;
                default:
                    trig_data = 0;
                    break;
            }


            UInt32 dataBytesLength = 32;
            UInt16[] TrigDataL = new UInt16[dataBytesLength];
            Unsafe.CopyBlock(ref Unsafe.As<UInt16, Byte>(ref TrigDataL[0]), ref Unsafe.As<UInt32, Byte>(ref trig_data), (UInt32)Unsafe.SizeOf<UInt32>());
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


            return (Int32) decodeChID;
        }
    }
}

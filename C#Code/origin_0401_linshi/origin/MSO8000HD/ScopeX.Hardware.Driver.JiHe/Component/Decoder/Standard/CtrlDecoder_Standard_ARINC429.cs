#if !Product_B21_JinHui_PXI
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
        internal static Int32 Config_Standard_ARINC429()
        {
            uint decodeChID = (uint)((int)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (int)ChannelIdExt.MinBChId);

            HdMessage.ProtocolARIN429Options? decodeOption = Hd.UIMessage!.Decoder![decodeChID].ProtocolOptions! as HdMessage.ProtocolARIN429Options;

            HdMessage.TrigARINC429ConditionsOptions? trigOption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigARINC429ConditionsOptions;

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 24);

            if (decodeOption == null || trigOption == null)
                return -1;
            #region 通道选择
            ChannelId sourceH = decodeOption!.SignalInputA; //data_in_h
            if (sourceH.IsAnalog())//模拟通道
            {; }
            else if (sourceH.IsDigital())//数字通道
                sourceH -= 31;
            else
                sourceH = 0;
            ChannelId sourceL = decodeOption!.SignalInputB;//data_in_l 单端输入的时候用这个
            if (sourceL.IsAnalog())//模拟通道
                sourceL += 1;
            else if (sourceL.IsDigital())//数字通道
                sourceL -= 31;
            else
                sourceL = 0;
            UInt64 sourceControlword = (UInt32)sourceH;
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.ARINC429);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceL, (uint)sourceControlword);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceM, (uint)(sourceControlword >> 16));
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceH, 0x00);

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect1Pro, (UInt32)sourceH);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect2Pro, (UInt32)sourceL);
          
            #endregion


            #region 触发参数设置
            UInt32 baudbps = (UInt32)decodeOption.Baud;                 //波特率
            //UInt32 signalRate    = (Constants.PROT_SYS_CLOCK_HZ / baudbps) * 1024 * 2;     //信号速率（波特率计数值）
            //UInt32 signalRate    = (Constants.PROT_SYS_CLOCK_HZ / baudbps)/2;     //信号速率（波特率计数值）
            UInt32 signalRate = (Constants.PROT_SYS_CLOCK_HZ / baudbps) * 1024 / 2;     //信号速率（波特率计数值）
            UInt32 trigCondition = (UInt32)trigOption.Condition;                //触发条件

            UInt32 trigError = (UInt32)trigOption.ErrorType;
            trigCondition = trigCondition == 7 ? trigCondition + trigError : trigCondition;


            UInt64 trigControlWord = 0;
            //trigControlWord |= signalRate<<10;
            trigControlWord |= signalRate;
            trigControlWord |= (UInt64)trigCondition << 32;

            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigControlWord & 0xFFFF));
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigControlWord >> 16) & 0xFFFF));//发送set[31:17]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigControlWord >> 32) & 0xFFFF));//发送set[47:32]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWord64, (UInt32)((trigControlWord >> 48) & 0xFFFF));//发送set[64:48] 

            #endregion



            #region 触发数据设置
            uint dataBytesLength = 32;
            UInt16[] TrigDataL = new UInt16[dataBytesLength];
            //UInt32 standardIdMask = 0;
            if (trigOption.Condition == ProtocolARINC429.Condition.Label)
            {
                TrigDataL[0] = (UInt16)(trigOption.Label & 0x00ff);//1-8 label
            }
            if (trigOption.Condition == ProtocolARINC429.Condition.SDI)
            {
                TrigDataL[0] = (UInt16)((trigOption.SDI & 0x0003) << 8);// 9-10 SDI
            }
            if (trigOption.Condition == ProtocolARINC429.Condition.Data)
            {
                TrigDataL[0] = (UInt16)((trigOption.Data & 0X3f) << 10);//11-16
                TrigDataL[1] = (UInt16)((trigOption.Data >> 6) & 0x1fff);//17-?
            }
            if (trigOption.Condition == ProtocolARINC429.Condition.SSM)
            {
                TrigDataL[1] = (UInt16)((trigOption.SSM & 0x03) << 13);
            }
            if (trigOption.Condition == ProtocolARINC429.Condition.LabelAndData)
            {
                /*
                TrigDataL[0] = (UInt16)(trigOption.Label & 0x00_ff);//1-8 Label
                TrigDataL[0] = (UInt16)((trigOption.SDI & 0x00_03) << 8);// 9 - 10 SDI
                TrigDataL[0] = (UInt16)((trigOption.Data & 0X3f) << 10);//11-16
                TrigDataL[1] = (UInt16)((trigOption.Data >> 6) & 0x1fff);//17-29
                TrigDataL[1] = (UInt16)((trigOption.SSM & 0x03) << 13);//30-31
                */
                TrigDataL[0] = (UInt16)((trigOption.Label & 0x00_ff) | ((trigOption.SDI & 0x00_03) << 8) | ((trigOption.Data & 0X3f) << 10));
                TrigDataL[1] = (UInt16)(((trigOption.Data >> 6) & 0x1fff) | ((trigOption.SSM & 0x03) << 13));
            }

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 0);

            for (uint dataIndex = 0; dataIndex < dataBytesLength; dataIndex++)
            {
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataL, (uint)TrigDataL[dataIndex] & 0xffff);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLIndex, dataIndex);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x01);//拉高数据触发的数据使能
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x00);//拉高数据触发的数据使能
            }

            #endregion

            return (Int32)decodeChID;
        }
    }
}
#endif
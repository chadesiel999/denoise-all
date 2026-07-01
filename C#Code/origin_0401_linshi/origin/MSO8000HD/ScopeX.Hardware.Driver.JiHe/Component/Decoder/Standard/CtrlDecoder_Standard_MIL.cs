using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Driver
{
    internal partial class CtrlDecoder
    {
        internal static Int32 Config_Standard_MIL()
        {
            UInt32 decodechid = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);

            HdMessage.TrigMILConditionsOptions? trigoption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigMILConditionsOptions;

            HdMessage.ProtocolMILOptions? decodeoption = Hd.UIMessage!.Decoder![decodechid].ProtocolOptions! as HdMessage.ProtocolMILOptions;

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 23);

            if (decodeoption == null || trigoption == null)
                return -1;


            #region 通道选择
            ChannelId source = decodeoption!.Source; //时钟线SCL
            if (source.IsAnalog())//模拟通道
            {; }
            else if (source.IsDigital())//数字通道
                source -= 31;
            else
                source = 0;

            UInt64 sourcecontrolword = (UInt32)source << 0;

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect1Pro, (UInt32)source);


            #endregion


            #region 触发参数设置
            UInt32 polarity = (UInt32)(decodeoption.Polarity);//协议类型
            UInt32 trigmode = (UInt32)trigoption.Condition;
            UInt32 errorkind = 0;
            UInt32 frame;
            if(trigmode == (UInt32)(ProtocolMIL.Condition.CmdOrStatus)) 
            {
                frame = 0;
            }
            else
            {
                frame = 1;
            }
            UInt64 trigcontrolword = 0;

            trigcontrolword |= polarity << 0;
            trigcontrolword |= (UInt64)trigmode << 33;
            trigcontrolword |= (UInt64)errorkind << 39;
            trigcontrolword |= (UInt64)frame << 41;

            //mil1553b_set[0]         1   mil_kind 极性
            //mil1553b_set[16:1]      16  response_timecnt[15:0]      最大响应时间（未使用）
            //mil1553b_set[32:17]     16  response_timecnt_min[15:0]  最小响应时间（未使用）
            //mil1553b_set[36:33]     4   trig_mode[3:0]              触发模式 0为同步 1为命令 2为状态 3为数据 4错误
            //mil1553b_set[38:37]  	2   time_kind[1:0]              时间触发参数，暂时未使用
            //mil1553b_set[40:39]  	2   error_kind[1:0]             错误类型：0为奇偶校验    1为同步错误    2为曼彻斯特码错误      3为非连续数据错误
            //mil1553b_set[41]        1   mil1553_frame 协议格式：0为命令字        1为状态字

            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigcontrolword & 0xFFFF));
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigcontrolword >> 16) & 0xFFFF));//发送set[31:17]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigcontrolword >> 32) & 0xFFFF));//发送set[47:32]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWord64, (UInt32)((trigcontrolword >> 48) & 0xFFFF));//发送set[64:48] 

            #endregion



            #region 触发数据设置
            UInt32 databyteslength = 32;
            UInt16[] trigdatal = new UInt16[databyteslength];

            for (UInt32 indexer = 0; indexer < 8; indexer++)
            {
                trigdatal[indexer + 8] = 0xFFFF;
            }


            for (UInt32 dataindex = 0; dataindex < databyteslength; dataindex++)
            {
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataL, (uint)trigdatal[dataindex] & 0xffff);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLIndex, dataindex);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x01);//拉高数据触发的数据使能
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x00);//拉高数据触发的数据使能
            }
            #endregion

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 0);

            return (Int32)decodechid;
        }
    }
}

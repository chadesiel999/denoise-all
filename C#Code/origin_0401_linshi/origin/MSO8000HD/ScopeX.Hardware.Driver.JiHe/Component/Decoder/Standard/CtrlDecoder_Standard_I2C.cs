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
        internal static Int32 Config_Standard_I2C()
        {
            UInt32 decodechid = (UInt32)((Int32)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (Int32)ChannelIdExt.MinBChId);

            HdMessage.ProtocolI2COptions? decodeoption = Hd.UIMessage!.Decoder![decodechid].ProtocolOptions! as HdMessage.ProtocolI2COptions;

            HdMessage.TrigI2CConditionsOptions? trigoption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigI2CConditionsOptions;

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 13);

            if (decodeoption == null || trigoption == null)
                return -1;
            //解码RAM预触发深度12bit
            UInt32 predepth = (UInt32)((Double)((UInt64)Hd.AnalogChannel!.AcquingParameters.SettingTrigPositionByfs / Hd.AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr) / (Constants.VIS_XDIVS_NUM * 1000) * 4096);
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth, predepth);


            #region 通道选择
            ChannelId clkinput = decodeoption!.SCLK;//时钟输入通道ch[0]
            if (clkinput.IsAnalog())//模拟通道
            {; }
            else if (clkinput.IsDigital())//数字通道
                clkinput -= 31;
            else
                clkinput = 0;
            ChannelId datainput = decodeoption.SDA;//片选输入通道ch[1]
            if (datainput.IsAnalog())//模拟通道
            {; }
            else if (datainput.IsDigital())//数字通道
                datainput -= 31;
            else
                datainput = 0;
            UInt64 sourcecontrolword = (UInt32)clkinput << 0 | (UInt32)datainput << 5;

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect1Pro, (UInt32)clkinput);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect2Pro, (UInt32)datainput);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect3Pro, (UInt32)mosi_input);


            #endregion

            // #region 触发参数设置
            // UInt32 addrWidth = trigOption == null ? 0 : (uint)decodeOption.BitWidth; //地址宽度
            // UInt32 addr = trigOption == null ? 0 : (uint)trigOption.AddressData;//地址
            // UInt32 extendAddr = addr >> 7;//扩展地址
            // UInt32 standardAddr = addr & 0x7F;//标准地址
            // UInt32 dataRelation = trigOption == null ? 0 : (uint)trigOption.Relation;//数据关系符
            // UInt32 dataDirection = trigOption == null ? 0 : (uint)trigOption.Direction;//数据方向

            // //UInt32 dataLength = trigOption == null ? 0 : (uint)trigOption.DataBytesCount;//数据长度（字节数）
            // UInt32 dataLength = 0;                         //目前数据触发为1个字节，在FPGA中应当固定发送0(by gyt）
            // UInt32 trigCondition = trigOption == null ? 0 : (uint)trigOption.Condition;//触发条件
            // UInt64 trigControlWord = 0;


            // trigControlWord |= dataLength;
            // trigControlWord |= addrWidth << 3;
            // trigControlWord |= dataDirection << 4;
            // trigControlWord |= trigCondition << 5;
            // if(decodeOption.BitWidth  == ProtocolI2C.AddrBitWidth.AddrBitWidth_7)
            // {
            //     trigControlWord |= standardAddr << 11;       //地址
            // }
            //else
            // { 
            //     trigControlWord |= addr << 8;
            // }
            // #endregion


            UInt32 bytenum      = trigoption.DataBytesCount - 1; // 字节数 bit 0-2
            UInt32 addrtype     = (UInt32)(decodeoption.BitWidth == ProtocolI2C.AddrBitWidth.AddrBitWidth_10 ? 1 : 0); // 地址类型 bit 3
            UInt32 rddriection  = (UInt32)(trigoption.Direction); // 读写方向 bit 4 
            UInt32 trigtype     = (UInt32)(trigoption.Condition); // 触发类型 bit 5-7

            UInt32 addrvalue    = (UInt32)(trigoption.AddressData); // 地址值 bit 8-17

  


            UInt32 addrmask     = 0;

            UInt64 trigcontrolword = 0;

            trigcontrolword |= bytenum;
            trigcontrolword |= addrtype << 3;
            trigcontrolword |= rddriection << 4;
            trigcontrolword |= trigtype << 5;

            if (decodeoption.BitWidth == ProtocolI2C.AddrBitWidth.AddrBitWidth_7)
            {
                trigcontrolword |= addrvalue << 11;
            }
            else
            {
                trigcontrolword |= addrvalue << 8;
            }
 
            trigcontrolword |= addrmask << 18;


            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigcontrolword & 0xFFFF));
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigcontrolword >> 16) & 0xFFFF));//发送set[31:17]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigcontrolword >> 32) & 0xFFFF));//发送set[47:32]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWord64, (UInt32)((trigcontrolword >> 48) & 0xFFFF));//发送set[64:48] 



            //触发条件选择“数据”触发时使用的data
            UInt32 databyteslength = 32;
            UInt16[] trigdata = new UInt16[databyteslength];

            trigdata[0] = (UInt16)(trigoption.Data & 0xffff);
            trigdata[1] = (UInt16)((trigoption.Data >> 16) & 0xffff);
            trigdata[2] = (UInt16)((trigoption.Data >> 32) & 0xff);


            //UInt64 data = (UInt64)(trigOption.Data << (byte)(40 - trigOption.DataBytesCount * 8));

            //TrigData[0] = 00;
            //TrigData[1] = 0x4E00;
            //TrigData[2] = 0x55;


            UInt64 datamask = 0xFF_FF_FF_FF_FF; // 40bit
            datamask = datamask << ((byte)(trigoption.DataBytesCount) * 8);

            //(data_mask << trigOption.ByteCount * 8);

            trigdata[8]  = (UInt16)(datamask & 0xffff);
            trigdata[9]  = (UInt16)((datamask >> 16) & 0xffff);
            trigdata[10] = (UInt16)((datamask >> 32) & 0xff);

            //TrigData[8] = 0;
            //TrigData[9] = 0;
            //TrigData[10] = 0x00;


            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 0);

            for (UInt32 dataindex = 0; dataindex < databyteslength; dataindex++)
            {
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(regAddr: AcqBdReg.W.Decoder_TrigDataL, (UInt32)trigdata[dataindex] & 0xffff);//数据触发的数据

                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLIndex, dataindex);//数据触发的地址

                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLValid, 0x01);//拉高数据触发的数据使能
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_TrigDataLValid, 0x00);//拉高数据触发的数据使能

                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataL, (uint)trigdata[dataindex] & 0xffff);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLIndex, dataindex);//数据触发的地址
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x01);//拉高数据触发的数据使能
                HdIO.WriteReg(ProcBdReg.W.Decoder_TrigDataLValid, 0x00);//拉高数据触发的数据使能
            }


            return (Int32)decodechid;
        }

        private static int UInt32(uint v)
        {
            throw new NotImplementedException();
        }
    }
}

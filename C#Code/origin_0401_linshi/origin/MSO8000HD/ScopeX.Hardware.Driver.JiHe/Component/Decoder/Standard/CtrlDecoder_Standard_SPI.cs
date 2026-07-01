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
        internal const double DECODE_CLK = 3.2e-9;
        static UInt64 CalculateMaskByByte(UInt64 value)
        {
            if (value == 0) return UInt64.MaxValue; // 特殊情况：0的掩码为全1

            UInt64 mask = UInt64.MaxValue; // 初始掩码为全1
            bool foundNonZeroByte = false;

            // 遍历每一个字节，从高到低
            for (Int32 i = 7; i >= 0; i--)
            {
                // 提取当前字节
                byte currentByte = (byte)((value >> (i * 8)) & 0xFF);

                if (!foundNonZeroByte)
                {
                    if (currentByte != 0) // 检查字节是否为非零
                    {
                        foundNonZeroByte = true;
                        // 清除整个字节
                        mask &= ~((UInt64)0xFF << (i * 8));
                    }
                }
                else
                {
                    // 第一个非零字节之后的所有字节设为0
                    mask &= ~((UInt64)0xFF << (i * 8));
                }
            }

            return mask;
        }

        static UInt64 CalculateMask(UInt64 value)
        {
            if (value == 0) return UInt64.MaxValue; // 特殊情况：0的掩码为全1

            UInt64 mask = UInt64.MaxValue; // 初始掩码为全1
            bool foundFirstOne = false;

            // 遍历每一位，从高到低
            for (Int32 i = 63; i >= 0; i--)
            {
                if (!foundFirstOne)
                {
                    if ((value & (1UL << i)) != 0) // 检查第i位是否为1，注意这里要用1UL表示无符号长整型的1
                    {
                        foundFirstOne = true; // 找到第一个非零位
                        mask &= ~(1UL << i);
                    }
                }
                else
                {
                    // 第一个非零位及其后的所有位设为0
                    mask &= ~(1UL << i);
                }
            }

            return mask;
        }
        internal static Int32 Config_Standard_SPI()
        {

            uint decodeChID = (uint)((int)Hd.UIMessage!.Trigger!.TrigDecoder!.id - (int)ChannelIdExt.MinBChId);
            if (Hd.UIMessage!.Decoder == null || decodeChID >= Hd.UIMessage!.Decoder.Count())
            {
                return -1;
            }
            HdMessage.ProtocolSPIOptions? decodeOption = Hd.UIMessage!.Decoder![decodeChID].ProtocolOptions! as HdMessage.ProtocolSPIOptions;

            HdMessage.TrigSPIConditionsOptions? trigOption = Hd.UIMessage!.Trigger!.TrigDecoder!.DecoderConditionsOptions! as HdMessage.TrigSPIConditionsOptions;

            //HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, (UInt32)SerialProtocolType.SPI);

            if (decodeOption == null || trigOption == null)
                return -1;
            #region 通道选择
            ChannelId clk_input = decodeOption!.CLK;//时钟输入通道ch[0]
            if (clk_input.IsAnalog())//模拟通道
            {; }
            else if (clk_input.IsDigital())//数字通道
                clk_input -= 31;
            else
                clk_input = 0;
            ChannelId cs_input = decodeOption.CS;//片选输入通道ch[1]
            if (cs_input.IsAnalog())//模拟通道
            {; }
            else if (cs_input.IsDigital())//数字通道
                cs_input -= 31;
            else
                cs_input = 0;
            ChannelId mosi_input = decodeOption.MOSI;//mosi输入通道ch[2]
            if (mosi_input.IsAnalog())//模拟通道
            {; }
            else if (mosi_input.IsDigital())//数字通道
                mosi_input -= 31;
            else
                mosi_input = 0;
            //ChannelId miso_input = decodeOption.MISO;//miso输入通道ch[3]
            //if (miso_input.IsAnalog())//模拟通道
            //{; }
            //else if (miso_input.IsDigital())//数字通道
            //    miso_input -= 31;
            //else
            //    miso_input = 0;

            ChannelId miso_input = ChannelId.C8;

            //UInt64 sourceControlword = (UInt32)cs_input << 0 | (UInt32)clk_input << 5 | (UInt32)mosi_input << 10 | (UInt32)miso_input << 15;
    
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect1Pro, (UInt32)cs_input);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect2Pro, (UInt32)clk_input);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelect3Pro, (UInt32)mosi_input);
            //HdIO.WriteReg(ProcBdReg.W.Decoder_B1SignalSourceL, (UInt32)sourceControlword);
            //HdIO.WriteReg(ProcBdReg.W.Decoder_B1SignalSourceM, (UInt32)(sourceControlword >> 16));
            //HdIO.WriteReg(ProcBdReg.W.Decoder_B1SignalSourceH, 0x00);

            #endregion

            #region 触发参数设置
            //set参数
            UInt64 idleTime = (UInt32)(decodeOption.IdleTime / DECODE_CLK) & 0xffffffff; //空闲时间（18bit）
            UInt32 csEdge = (UInt32)decodeOption.CSLevelState;//片选电平
            UInt32 clkEdge = (UInt32)(decodeOption.CLKState == ProtocolCommon.Edge.Rise ? 1 : 0);//时钟边沿
            UInt32 idleen = (UInt32)((decodeOption.FramingMode == ProtocolSPI.FramingMode.TIMEOUT) ? 1 : 0);
            UInt32 misopolarity = (UInt32)decodeOption.MISOPolarity;//MISO极性
            UInt32 mosipolarity = (UInt32)decodeOption.MOSIPolarity;//MOSI极性 
            UInt64 frameBitWidth = (UInt64)trigOption.DataBitWidth;//0~128的字节数

            //UInt64 byteCount = frameBitWidth / (UInt64)trigOption.FrameCount;//帧数据位宽
            UInt64 byteCount = 8;

            UInt32 trigCondition;
            switch (trigOption!.Condition)
            {
                case ProtocolSPI.Condition.CS:
                default:
                    trigCondition = 0; // CS片选触发
                    break;
                case ProtocolSPI.Condition.Data:
                    //if (trigOption!.DataSource == ProtocolSPI.DataTriggerSource.MISO)
                    //{
                    //    trigCondition = 0b10;
                    //}
                    //else if (trigOption!.DataSource == ProtocolSPI.DataTriggerSource.MOSI)
                    //{
                    //    trigCondition = 0b01;
                    //}
                    //else
                    //{
                    //    trigCondition = 0b11;
                    //}
                    //数据
                    idleen = 0; //固定，0 片选 1空闲
                    trigCondition = 1;
                    break;
            }

            //片选
            //csEdge = 1; // 1正极性
            //clkEdge = 0; //固定
            UInt64 trigControlWord = (UInt64)mosipolarity;
            trigControlWord |= (misopolarity << 1);
            trigControlWord |= (csEdge << 2);
            trigControlWord |= (clkEdge << 3);
            trigControlWord |= (idleen << 5);
            trigControlWord |= (byteCount << 6);
            trigControlWord |= (frameBitWidth << 14);
            trigControlWord |= (trigCondition << 22);
            trigControlWord |= (idleTime << 32);



            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordL, (UInt32)(trigControlWord & 0xFFFF));
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordM, (UInt32)((trigControlWord >> 16) & 0xFFFF));//发送set[31:17]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWordH, (UInt32)((trigControlWord >> 32) & 0xFFFF));//发送set[47:32]
            HdIO.WriteReg(ProcBdReg.W.Decoder_TrigControlWord64, (UInt32)((trigControlWord >> 48) & 0xFFFF));//发送set[64:48]                                                                                      //8000HD使用PROC板寄存器，但是这个在7000X是使用备用寄存器，8000HD没有对应的寄存器，待处理！！！！！                                                                                                                 //最高的48~64位暂时寄存器缺失，没有连接
                                                                                                        
            #endregion

            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_TrigTypeSelect, (UInt32)SerialProtocolType.SPI);//触发源选择

            #region 触发数据设置
            //触发条件选择“数据”触发时使用的data

            //触发条件选择“数据”触发时使用的data
            uint dataBytesLength = 32;//数据使能拉高->拉低一次为一接收周期，无论使用多少字节数据做触发，FPGA需要触发数据发16个周期
            UInt16[] TrigDataL = new UInt16[dataBytesLength];//数据L


            //TrigData[7] = (UInt16)(trigOption!.FrameData << 8);
            UInt64 trigData = trigOption == null ? 0 : (UInt64)trigOption.FrameData;
            UInt64 trigDataHigh = trigOption == null ? 0 : (UInt64)trigOption.FrameDataHigh;
            HdIO.WriteReg(ProcBdReg.W.Decoder_ResetAfterParamChanged, 1);
            HdIO.WriteReg(ProcBdReg.W.Decoder_ResetAfterParamChanged, 0);
            UInt64 dataMask = CalculateMaskByByte(trigData);

            UInt64 dataMaskHigh = CalculateMaskByByte(trigDataHigh);
            if (trigOption != null)
            {
                //data
                TrigDataL[0] = (UInt16)(trigData & 0xffff);
                TrigDataL[1] = (UInt16)((trigData >> 16) & 0xffff);
                TrigDataL[2] = (UInt16)((trigData >> 32) & 0xffff);
                TrigDataL[3] = (UInt16)((trigData >> 48) & 0xffff);

                TrigDataL[4] = (UInt16)(trigDataHigh & 0xffff);
                TrigDataL[5] = (UInt16)((trigDataHigh >> 16) & 0xffff);
                TrigDataL[6] = (UInt16)((trigDataHigh >> 32) & 0xffff);
                TrigDataL[7] = (UInt16)((trigDataHigh >> 48) & 0xffff);

                TrigDataL[16] = (UInt16)(dataMask & 0xffff);
                TrigDataL[17] = (UInt16)((dataMask >> 16) & 0xffff);
                TrigDataL[18] = (UInt16)((dataMask >> 32) & 0xffff);
                TrigDataL[19] = (UInt16)((dataMask >> 48) & 0xffff);

                TrigDataL[20] = (UInt16)(dataMaskHigh & 0xffff);
                TrigDataL[21] = (UInt16)((dataMaskHigh >> 16) & 0xffff);
                TrigDataL[22] = (UInt16)((dataMaskHigh >> 32) & 0xffff);
                TrigDataL[23] = (UInt16)((dataMaskHigh >> 48) & 0xffff);

            }
            for (uint dataIndex = 0; dataIndex < dataBytesLength; dataIndex++)
            {

                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(regAddr: AcqBdReg.W.Decoder_TrigDataL, (uint)TrigDataL[dataIndex] & 0xffff);//数据触发的数据
                
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

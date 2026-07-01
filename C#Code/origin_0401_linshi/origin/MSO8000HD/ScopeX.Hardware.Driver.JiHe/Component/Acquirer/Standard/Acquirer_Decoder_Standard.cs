#if !Product_B21_JinHui_PXI
using System;
using System.Collections.Generic;
using System.Text;
using ScopeX.ComModel;
using System.Reflection;
using System.Linq;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.ComponentModel.DataAnnotations;
using ScopeX.Hardware.Driver.Module;

namespace ScopeX.Hardware.Driver
{
    public partial class Acquirer_Decoder_Standard : AbstractAcquirer_Decoder
    {
        internal override AcqDataType DataType => AcqDataType.Decode;
        public Acquirer_Decoder_Standard()
        {
            ConfigFunc = ourConfig;
        }
        internal override void Init()
        {
            //高速协议部分
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ProtocolTypeForTrigger, 0);//协议触发选择，控制set参数以及协议解码使能
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1Enable, 0);//解码通道1
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2Enable, 0);//解码通道2
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_DataFromFifoOrRam, 1);//1：传波形数据    0：传解码数据
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_RamPreDepth, 0);//解码RAM预触发深度
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceL, 0);//通道控制字选择
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceM, 0);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B1SignalSourceH, 0);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2SignalSourceL, 0);//通道控制字选择
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2SignalSourceM, 0);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_B2SignalSourceH, 0);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_RamReadEnable, 0);//RAM读使能
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_RamWriteEnable, 0);//RAM写使能之一
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_RamResetEnable, 0);//解码复位，高有效
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 0);//协议复位，低有效

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decoder_ResetAfterParamChanged, 1);//协议复位，低有效

            //comment At 2023.06.01HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, 0);//协议触发选择，控制set参数以及协议解码使能

            //comment At 2023.06.01HdIO.WriteReg(ProcBdReg.W.Decoder_B1Enable, 0);//解码通道1
            //comment At 2023.06.01HdIO.WriteReg(ProcBdReg.W.Decoder_B2Enable, 0);//解码通道2
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.Decoder_DataFromFifoOrRam, 1);//1：传波形数据    0：传解码数据
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamPreDepth, 0);//解码RAM预触发深度
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_L, 0);//通道控制字选择
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_M, 0);
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B1_H, 0);
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_L, 0);//通道控制字选择
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_M, 0);
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_SignalSource_B2_H, 0);


            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamReadEnable, 0);//RAM读使能
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamWriteEnable, 0);//RAM写使能之一
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamResetEnable, 0);

            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_ResetAfterParamChanged, 0);//协议复位，低有效
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_ResetAfterParamChanged, 1);//协议复位，低有效
        }
        internal override void InitAcq()
        {
            HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 1);
            HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0);
            //HdIO.WriteReg(ProcBdReg.W.Decoder_FifoReset, 0x01);
            //comment At 2023.06.01 HdIO.WriteReg(ProcBdReg.W.Decoder_RamWriteEnable, 1);
        }

        internal override bool ReadAcqData(List<ReadInfo> readInfoList, out double SamplingRateByus, CancellationToken? softResetToken)
        {
            Boolean result = false;
            SamplingRateByus = 0;
            foreach (var info in readInfoList)
            {
                if (!String.IsNullOrEmpty(info.ExtInfo) && Enum.TryParse(typeof(ChannelId), info.ExtInfo, out var id) && id != null && id is ChannelId bus && bus.IsDecode())
                {
                    result = result && TryTakeData(bus, ref DecodeDataSource.Instance.AnalogDataSources[bus - ChannelId.B1]);
                    SamplingRateByus = 1E6 / DecodeDataSource.Instance.AnalogDataSources[bus - ChannelId.B1].SampleRate;
                }
            }
            return result;
        }
        internal void ourConfig()
        {
            Type typeOfAcquirer_Decoder = typeof(AbstractAcquirer_Decoder);
            foreach (HdMessage.DecoderOptions decoderOption in Hd.UIMessage!.Decoder!)
            {
                if (decoderOption.Active)
                {
                    SerialProtocolType currProtocol = decoderOption.ProtocolType;
                    string currProtocolTypeName = currProtocol.ToString();
                    MethodInfo? methodInfo = typeOfAcquirer_Decoder.GetMethod($"Config_{currProtocolTypeName}", BindingFlags.Static | BindingFlags.NonPublic);
                    methodInfo?.Invoke(null, null);
                }
            }
        }

        private Boolean TryTakeData(ChannelId busId, ref DeocodeDataSourcePacket packet)
        {
            return false;
            if (!DecodeDataSource.NeedTakeNewData ||
                Hd.UIMessage?.Analog == null ||
                Hd.UIMessage.Analog.Length == 0 ||
                Hd.UIMessage.Timebase == null ||
                HdIO.CurrDriver == null ||
                HdIO.CurrDriver is Driver_Simulator ||
                Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquedParameters == null ||
                Hd.UIMessage.Analog.Count(x => x.Active) == 0)
            {
                packet.HasData = false;
                return true;
            }
            packet.Channels = Enumerable.Range(0, Hd.UIMessage.Analog.Length)
                .Where(x => Hd.UIMessage.Analog[x].Active)
                .Select(x => x + ChannelId.C1).ToArray();
            var analogAcquireModel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
            AcqModeAndInterleaveDefine define = analogAcquireModel.GetCurrentAcqModeInterleave()!;
            UInt32 openchnls = (define.InterleaveMode == AdcInterleaveMode.Mode1To1 ? 0xfU : 0x5U);//0xf=0b1111=4通道打开;0x5=0b0101=1,3通道打开;
            switch (openchnls)
            {
                case 0XFU:
                default:
                    packet.Channels = ChannelIdExt.GetAnalogs().ToArray();
                    break;
                case 0X5U:
                    packet.InterwovenBitCount = 32;
                    packet.Channels = packet.Channels.Take(1).ToArray();
                    break;
            }

            packet.SampleRate = 1E15 / Hd.CurrProduct.Acquirer_AnalogChannel.AcquedParameters.PerDataByfs_AtDdr;
            /*****************************************************
             * 发送比较电平
             ****************************************************/
            var thresholds = Hd.UIMessage.Decoder![busId - ChannelId.B1].ProtocolOptions!.GetThresholdInfos()
               .OrderBy(x => x.ChannelId)
               .DistinctBy(x => x.ChannelId).ToList();
            ConfigReshapedThresholds(busId, thresholds, define.InterleaveMode);
            UInt32 ptscount = (UInt32)Hd.UIMessage.Timebase.StorageWaveDotsCnt;
            UInt32 readlen = ptscount;
            Int32 startaddr = (Int32)((Hd.UIMessage.Timebase.TmbPosition / (Hd.UIMessage.Timebase.TmbScale * Constants.VIS_XDIVS_NUM) - 0.5) * readlen);
            packet.TriggerIndex = -startaddr;
            if (startaddr > 0)
                startaddr = 0;

            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.LSCtrl_Offset_L, AcqBdReg.W.LSCtrl_Offset_H, Unsafe.As<Int32, UInt32>(ref startaddr));
            //(UInt32 Base, UInt32 Multiple) baseMuliple = SplitExtractNum(Hd.UIMessage.Timebase!.InterleaveMode, Hd.UIMessage.Timebase!.AcqMode == AnaChnlAcqMode.Peak, 1);
            //(UInt32 Base, UInt32 Multiple) baseMuliple = SplitExtractNum(Hd.UIMessage.Timebase!.InterleaveMode, true, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PosDecimationMode, Hd.UIMessage!.Timebase!.AcqMode == AnaChnlAcqMode.Peak ? 0U : 0);//此暂时设置0，10G数据有倒刺，pengbo
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.Decimation_PosGapx, (uint)AbstractAcquirer_AnalogChannel.PreExtractGapModeList[(uint)baseMuliple.Base]);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PosGapValuelL16, (UInt32)(baseMuliple.Multiple & 0xffff));
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PosGapValueH16, (UInt32)((baseMuliple.Multiple >> 16) & 0xffff));

            readlen *= 0X4U;
            //**********c1 232 get B1&B2=C1&B2 datalen is double************
            if (openchnls == 0X5U)
            {
                readlen /= 0X2U;
            }

            readlen /= 0X8U;



            #region switch data path
            //Hd.CurrProduct!.AcqBd!.SwitchDataPathAndPcieReset_SetDataLength(DMAReadDataTypes.Decoder, readlen);

            #region PCIE 收数据长度
            HdIO.WriteReg(PcieBdReg.W.FifoCtrl_FullProgDepth, (UInt32)((readlen + 4095) / 4096 * 4096) * 8);//8=8bit
            #endregion

            #region PCIE收数据复位
            HdIO.WriteReg((uint)PcieBdReg.W.Xdma_XdmaIrqReset, 0);
            HdIO.WriteReg((uint)PcieBdReg.W.Xdma_XdmaIrqReset, 1);
            #endregion
            #endregion

            HdIO.WriteReg(ProcBdReg.W.DataPath_ProDataMux, 05);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_MuxDataPathACQ, 8);

            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.DataPath_acq_linkmux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkmux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            HdIO.WriteReg(ProcBdReg.W.DataPath_pro_linkdemux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);
            HdIO.WriteReg(PcieBdReg.W.DataPath_pcie_linkdemux_select, (UInt32)DMAReadSourceMuxType.AnalogChanneData);

            if (openchnls == 0X5U)
            {
                var buffer = new Byte[readlen];
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);//开启从DDR中读数，共生成DPX。电平有效
                var hasdata = HdIO.DMARead(readlen, ref buffer);
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                buffer = C1orC3ChannelReshapedDataSplit(buffer);
                packet.MaxByteCount = (UInt32)buffer.Length;
                packet.TimeStamp = DateTime.Now.Ticks;
                packet.PerChannelDataLength = (UInt32)(buffer.Length * 8);
                packet.ChannelDataSource = buffer;
                packet.HasData = hasdata;
                DecodeDataSource.NeedTakeNewData = false;
                return true;
            }
            else //datarate=4
            {
                packet.MaxByteCount = readlen;
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);//开启从DDR中读数，共生成DPX。电平有效
                packet.ChannelDataSource = new byte[packet.MaxByteCount];
                packet.HasData = HdIO.DMARead(packet.MaxByteCount, ref packet.ChannelDataSource);
                HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                packet.TimeStamp = DateTime.Now.Ticks;
                packet.PerChannelDataLength = ptscount;
                DecodeDataSource.NeedTakeNewData = false;
                return true;
            }
        }


        #region  ************************8000项目整形的比较电平配置************************
        /************规则*************/
        //20G模式（单开CH1或者CH3），level1开头为唯一比较电平
        //10G模式，level1指到Board1的CH2，Board2的CH4
        //10G模式，level2指到Board1的CH1，Board2的CH3

        //对于多阈值协议，每个level1或者level2为16位数据，但是比较电平最多为12位，所以最多支持16个阈值，每个阈值12位


        private readonly static List<AcqBdReg.W> REGS = new List<AcqBdReg.W>()
            {
                AcqBdReg.W.Decoder_ProtocolReshapeLevel112,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel111,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel110,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel19,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel18,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel17,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel16,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel15,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel14,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel13,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel12,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel11
            };
        private readonly static List<AcqBdReg.W> REGS2 = new List<AcqBdReg.W>()
            {
                AcqBdReg.W.Decoder_ProtocolReshapeLevel212,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel211,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel210,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel29,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel28,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel27,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel26,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel25,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel24,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel23,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel22,
                AcqBdReg.W.Decoder_ProtocolReshapeLevel21
            };

        private readonly static Int32 AdcCenterValue = Constants.MAX_ADC_RES / 2;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thresholds">通道阈值键值对</param>
        /// <param name="interleaveMode">1To1为10G模式，1To2为20G模式</param>
        /// <returns></returns>
        private Boolean ConfigReshapedThresholds(ChannelId busId, List<(ChannelId id, Double[] thres)> thresholds, AdcInterleaveMode interleaveMode)
        {
            Boolean result = false;
            UInt32 levelEnable = 0;//双电平阈值使能
            if ((Hd.UIMessage!.Decoder![busId - ChannelId.B1].ProtocolType == SerialProtocolType.MIL) || (Hd.UIMessage.Decoder![busId - ChannelId.B1].ProtocolType == SerialProtocolType.ARINC429))
            {
                levelEnable = 1;
            }
            Int32 thindex = -1;
            Double threshold = 0;
            if (thresholds.Count > 0)
            {
                if (interleaveMode == AdcInterleaveMode.Mode2To1)
                {
                    for (int i = 0; i < Hd.UIMessage!.Analog!.Length; i++)
                    {
                        if (Hd.UIMessage!.Analog![i].Active)
                        {
                            if (thresholds.Count > 0)
                            {
                                thindex = thresholds.FindIndex(x => x.id == ChannelId.C1 + i);
                            }

                            if (thindex != -1 && thresholds[thindex].thres != null && thresholds[thindex].thres.Length > 0)
                            {
                                if (thindex >= 0)
                                    threshold = thresholds[thindex].thres[0] / Hd.UIMessage.Analog[i].ProbeGain;
                                var pos = GetThresholdPostionValue(i, threshold);
                                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(REGS[0], pos);
                            }
                            break;
                        }
                    }
                }
                else if (interleaveMode == AdcInterleaveMode.Mode1To1)
                {
                    for (int i = 0; i < Hd.UIMessage!.Analog!.Length; i++)
                    {
                        if (Hd.UIMessage!.Analog![i].Active)
                        {
                            if (thresholds.Count > 0)
                            {
                                thindex = thresholds.FindIndex(x => x.id == ChannelId.C1 + i);
                            }

                            if (thindex != -1 && thresholds[thindex].thres != null && thresholds[thindex].thres.Length > 0)
                            {
                                if (thindex >= 0)
                                    threshold = thresholds[thindex].thres[0] / Hd.UIMessage.Analog[i].ProbeGain;
                                var pos = GetThresholdPostionValue(i, threshold);
                                switch (i)
                                {
                                    case 0:
                                        Hd.CurrProduct?.AcqBd?.WriteReg(REGS2[0], AcqBdNo.B0, pos);
                                        break;
                                    case 1:
                                        Hd.CurrProduct?.AcqBd?.WriteReg(REGS[0], AcqBdNo.B0, pos);
                                        break;
                                    case 2:
                                        Hd.CurrProduct?.AcqBd?.WriteReg(REGS2[0], AcqBdNo.B1, pos);
                                        break;
                                    case 3:
                                        Hd.CurrProduct?.AcqBd?.WriteReg(REGS[0], AcqBdNo.B1, pos);
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        private UInt32 GetThresholdPostionValue(Int32 chindex, Double threshold)
        {
            UInt32 pos = Math.Clamp((UInt32)(Constants.SAMPS_PER_YDIV * (threshold * 1000 + Hd.UIMessage!.Analog[chindex]!.Position) / Hd.UIMessage!.Analog[chindex]!.Scale + AdcCenterValue), 48, 4047);
            return pos;
        }

        #endregion
        private Byte[] C1orC3ChannelReshapedDataSplit(Byte[] buffer)
        {
            if (buffer == null || buffer.Length <= 0)
            {
                return new Byte[0];
            }

            var position = Hd.UIMessage!.Analog[0].Active ? 0 : 4;//C1 get 0-3byte,C3 get 4-7byte
            var length = 4;
            Byte[] temp = new Byte[buffer.Length / 2];
            var index = 0;
            for (int i = 0, l = buffer.Length / 8; i < l; i++)
            {
                for (int ii = i * 8 + length - 1; ii >= i * 8; ii--)//0-3byte order is 3 2 1 0
                {
                    temp[index] = buffer[ii + position];
                    index++;
                }
            }
            return temp;
        }
    }
}
#endif

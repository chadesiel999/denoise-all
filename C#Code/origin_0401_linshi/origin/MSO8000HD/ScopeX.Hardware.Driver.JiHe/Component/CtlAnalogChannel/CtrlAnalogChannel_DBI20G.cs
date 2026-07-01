using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Data.Base.AnalogChannelEx;

namespace ScopeX.Hardware.Driver
{
    internal  class CtrlAnalogChannel_DBI20G : CtrlAnalogChannel_JiHe2d5G
    {
        private CtrlAnalogChannel_DBI20G()
        {
            ChannelId chnlid = ChannelId.C1;
            {
                foreach (AnaChnlScaleIndex scale in Enum.GetValues<AnaChnlScaleIndex>())
                {
                    String labelname = GetChnlName(chnlid, scale);
                    if (!AiAnalogChannelParams.Default.AllNames.Contains(labelname))
                    {
                        AiAnalogChannelParams.Default[labelname] = new AiAnalogChannelItem();
                    }
                }
            }
        }

        internal static CtrlAnalogChannel_DBI20G Default = new();

        public enum AnalogChannelItems_DBI20G : Int32
        {
            //校准参数
            StartFreqWithMhz = 1,
            EndFreqWithMHz = 2,
            InitPhaseFreqWithMHz = 3,
            LocalFreqByMHz = 4,
            DiscardDots = 5,
            //增益部分
            PreAttenuation = 6,
            GainRoadSelect = 7,
            P_Attenuation = 8,
            N_Attenuation = 9,
            //偏置部分
            InputBias = 10,
            InputBias_FixedDiv = 11,
            PreBias = 12,
            PreBias_FixedDiv = 13,
            PostBias = 14,
            PostBias_FixedDiv = 15,
            Op_FG1_RefVoltage = 16,
            Op_FG2_RefVoltage = 17,
            Op_VOS_RefVoltage = 18,
            //数字校准
            Gain_FineByTenThousandByAdc1 = 19,
            Gain_FineByTenThousandByAdc2 = 20,
            Gain_FineByFpgaThousand = 21,
            //预留寄存器
            Reserved0 = 28,
            Reserved1 = 29,
            Reserved2 = 30,
            Reserved3 = 31,

        }

        #region 内部辅助定义
        private static String GetChnlName(ChannelId chnlId, AnaChnlScaleIndex scale)
        {
            return $"{chnlId}_{scale}";
        }

        private static void ConfigRefVoltage()
        {
            if (Hd.UIMessage?.Analog == null)
                return;

            //for (Int32 chnlid = 0; chnlid < ChannelIdExt.AnaChnlNum; chnlid++)
            for (Int32 chnlid = 0; chnlid < 1; chnlid++)
            {
                if (chnlid >= Hd.UIMessage.Analog.Length)
                    continue;
                Int32 scale = Hd.UIMessage.Analog[chnlid].ScaleIndex;
                String chnlname = GetChnlName((ChannelId)chnlid, (AnaChnlScaleIndex)scale);

                //String cmd = GetSubband1Bias(DbiSunbband1VoltageEnum.RefVoltage, AiAnalogChannelParams.Default[chnlname].Subband1Ref);
                String cmd = GetSubbandBiasOffsetVoltage(DbiSunbbandVoltageEnum.RefVoltage, AiAnalogChannelParams.Default[chnlname].Subband1Ref);
                SendCmd((ChannelId)chnlid, cmd);
            }
        }

        public static void Send_IFCCoefficientsSelect()
        {
            CoefficientsTableSender_DBI.SendCoefficientsTablesByAnalogScale(ChannelId.C1);

        }

        private static readonly Int32[] _DefaultLocalFreq = new Int32[]
        {
            0,
            10_000,
            15_000,
            22_500,
        };

        private static readonly Dictionary<Int32, UInt64> _DefaultLocalFreqByHz = new()
        {
            [0] = 0,
            [1] = 10_000_000_000,
            [2] = 15_000_000_000,
            [3] = 22_500_000_000,
        };

        internal static void ConfigLocalFreq(Dictionary<Int32, UInt64> localFreqByHz, bool rec_local)
        {
            foreach (Int32 subbandid in _DefaultLocalFreqByHz.Keys)
            {
                UInt64 localfreq = localFreqByHz.ContainsKey(subbandid) ? localFreqByHz[subbandid] : _DefaultLocalFreqByHz[subbandid];
                if (subbandid == 0)
                {
                    if (subbandid >= Hd.UIMessage.Analog.Length)
                        continue;
                    double scale = Hd.UIMessage.Analog[subbandid].ScaleBymV;
                    //Subband1SampleGainTypeEnum gaintype = scale < 50 ? Subband1SampleGainTypeEnum.SecondGain : Subband1SampleGainTypeEnum.FirstGain;
                    Subband1SampleGainTypeEnum gaintype = Subband1SampleGainTypeEnum.SecondGain;//only second htf
                    Subband1SampleTypeEnum sampletype = localfreq == 0 ? Subband1SampleTypeEnum.DirectSample : Subband1SampleTypeEnum.MixSample;

                    String nmd = GetSubband1SampleType(sampletype, gaintype);
                    SendCmd((ChannelId)0, nmd);
                    //String nmd = GetSubbandLocalOscillatorFreq(subbandid + 1);
                    //SendCmd(ChannelId.C1, nmd);
                }
                if (rec_local == true)
                //if (true)
                {
                    String cmd = GetSubbandFreq(subbandid + 1, (Int32)(localfreq / 1e7));
                    SendCmd(ChannelId.C1, cmd);
                    Trace.WriteLine($"*********************************subband {subbandid + 1} rec_config local freq {localfreq} Hz");

                }
                else
                {
                    //直接数字合成本振配置
                    //if (subbandid != 1) //不配置2子带
                    {
                        String cmd_cons = GetSubbandLocalOscillatorFreq(subbandid + 1);
                        SendCmd(ChannelId.C1, cmd_cons);
                    }
                }

            }
        }

        internal static void ConfigLocalFreq(Int32[]? localFreqByMHz)
        {
            if (Hd.UIMessage?.Analog == null)
                return;

            Int32 chnlid = 0;
            {
                Int32 scale = Hd.UIMessage.Analog[chnlid].ScaleIndex;
                String chnlname = GetChnlName((ChannelId)chnlid, (AnaChnlScaleIndex)scale);

                Int32[] freqArray = localFreqByMHz ?? _DefaultLocalFreq;

                for (Int32 subid = 0; subid < freqArray.Length; subid++)
                {
                    String cmd = GetSubbandFreq(subid + 1, freqArray[subid] / 100);
                    SendCmd((ChannelId)chnlid, cmd);
                }
            }
        }

        private static Int32 _LastSampleFreqByGHz = 0;
        private static Int32 _LastSubandId = 0;

        /// <summary>
        /// 将指定子带配置为指定的采样率：20G和80G
        /// </summary>
        /// <param name="chnlId"></param>
        /// <param name="subbandId"></param>
        /// <param name="freqByGHz"></param>
        private static void ConfigSubbandSampleFreq(ChannelId chnlId,Int32 subbandId, Int32 freqByGHz)
        {
            if (subbandId == 0)
            {
                String subband1str = GetSubband1SampleFreq(freqByGHz);
                SendCmd(chnlId, subband1str);
            }
            else
            {
                String cmd = GetSubbandSampleFreq(subbandId + 1, freqByGHz);
                SendCmd(chnlId, cmd);
            }
        }

        /// <summary>
        /// 将所有子带配置为20GSPS模式：DBI模式
        /// </summary>
        internal static void ConfigSampleFreq(ChannelId chnlId)
        {
            if (_LastSampleFreqByGHz == 20)
                return;
            for (Int32 subbandid = 0; subbandid < 4; subbandid++)
            {
                ConfigSubbandSampleFreq(chnlId, subbandid, 20);
            }
            _LastSampleFreqByGHz = 20;
        }

        /// <summary>
        /// 将指定子带配置为80GSPS模式：一驱四
        /// </summary>
        /// <param name="chnlId"></param>
        /// <param name="subbandId"></param>
        internal static void ConfigSampleFreq(ChannelId chnlId, Int32 subbandId)
        {
            if (_LastSampleFreqByGHz == 80 && subbandId == _LastSubandId)
                return;
            ConfigSubbandSampleFreq(chnlId, subbandId, 80);
            _LastSampleFreqByGHz = 80;
            _LastSubandId = subbandId;
        }

        private static void ConfigSampleFreq()
        {
            if (Hd.UIMessage?.Analog == null)
                return;

            //for (Int32 chnlid = 0; chnlid < ChannelIdExt.AnaChnlNum; chnlid++)
            for (Int32 chnlid = 0; chnlid < 1; chnlid++)
            {
                if (chnlid >= Hd.UIMessage.Analog.Length)
                    continue;
                Int32 scale = Hd.UIMessage.Analog[chnlid].ScaleIndex;
                String chnlname = GetChnlName((ChannelId)chnlid, (AnaChnlScaleIndex)scale);

                String subband1str = GetSubband1SampleFreq(AiAnalogChannelParams.Default[chnlname].Subband1SampleFreq);
                SendCmd((ChannelId)chnlid, subband1str);

                Int32[] freqArray = new Int32[]
                {
                    AiAnalogChannelParams.Default[chnlname].Subband2SampleFreq,
                    AiAnalogChannelParams.Default[chnlname].Subband3SampleFreq,
                    AiAnalogChannelParams.Default[chnlname].Subband4SampleFreq,
                };

                for (Int32 subid = 0; subid < freqArray.Length; subid++)
                {
                    String cmd = GetSubbandSampleFreq(subid + 2, freqArray[subid]);
                    SendCmd((ChannelId)chnlid, cmd);
                }
            }
        }

        private static void ConfigSamleType()
        {
            Subband1SampleTypeEnum[] sampletype = new Subband1SampleTypeEnum[]
            {
                Subband1SampleTypeEnum.DirectSample,
                Subband1SampleTypeEnum.DirectSample,
                Subband1SampleTypeEnum.DirectSample,
                Subband1SampleTypeEnum.DirectSample,
            };
            //for (Int32 chnlid = 0; chnlid < ChannelIdExt.AnaChnlNum; chnlid++)
            for (Int32 chnlid = 0; chnlid < 1; chnlid++)
            {
                if (chnlid < sampletype.Length)
                {
                    if (chnlid >= Hd.UIMessage.Analog.Length)
                        continue;
                    //double scale = Hd.UIMessage.Analog[chnlid].ScaleBymV;
                    //Subband1SampleGainTypeEnum gaintype = scale < 50 ? Subband1SampleGainTypeEnum.SecondGain : Subband1SampleGainTypeEnum.FirstGain;
                    Subband1SampleGainTypeEnum gaintype = Subband1SampleGainTypeEnum.SecondGain;//only second htf

                    String cmd = GetSubband1SampleType(sampletype[chnlid], gaintype);
                    SendCmd((ChannelId)chnlid, cmd);
                }
            }
        }

        /// <summary>
        /// 这个函数需优化
        /// </summary>
        /// <param name="chnlId"></param>
        /// <param name="subbandId">编号从0开始</param>
        /// <param name="bandLimitByHz"></param>
        private static void ConfigSubbandBandWidth(ChannelId chnlId, Int32 subbandId, UInt64 bandLimitByHz)
        {
            String cmd = String.Empty;

            //if (subbandId == 0)
            //{
            //    if (bandLimitByHz <= 20_000_000)
            //    {
            //        cmd = GetSubband1Bandwidth(DbiSubband1BandwidthEnum.Limit20MHz);

            //    }
            //    else if (bandLimitByHz <= 1_000_000_000)
            //    {
            //        cmd = GetSubband1Bandwidth(DbiSubband1BandwidthEnum.Limin1GHz);
            //    }
            //    else
            //    {
            //        cmd = GetSubband1Bandwidth(DbiSubband1BandwidthEnum.PassThrough);
            //    }
            //}
            //else
            {
                if (bandLimitByHz <= 50_000_000)
                {
                    cmd = GetOthersSubbandBindwidth(subbandId + 1, DbiOthersSubbandBandwidthEnum.Limmit50MHz);
                }
                else if (bandLimitByHz <= 500_000_000)
                {
                    cmd = GetOthersSubbandBindwidth(subbandId + 1, DbiOthersSubbandBandwidthEnum.Limmit500MHz);
                }
                else if (bandLimitByHz <= 1_000_000_000)
                {
                    cmd = GetOthersSubbandBindwidth(subbandId + 1, DbiOthersSubbandBandwidthEnum.Limmit1GHz);
                }
                else
                {
                    cmd = GetOthersSubbandBindwidth(subbandId + 1, DbiOthersSubbandBandwidthEnum.PassThrough);
                }
            }

            SendCmd(chnlId, cmd);
        }

        private static readonly Dictionary<Int32, UInt64> _DefaultSubbandBandwidth = new()
        {
            [0] = 6_000_000_000,
            [1] = 6_000_000_000,
            [2] = 6_000_000_000,
            [3] = 6_000_000_000,
        };

        internal static void ConfigSubbandBandWidth(Dictionary<Int32, UInt64> bandwidthByHz)
        {
            foreach (Int32 subbandid in _DefaultSubbandBandwidth.Keys)
            {
                UInt64 bandwidth = bandwidthByHz.ContainsKey(subbandid) ? bandwidthByHz[subbandid] : _DefaultSubbandBandwidth[subbandid];
                ConfigSubbandBandWidth(ChannelId.C1, subbandid, bandwidth);
                Trace.WriteLine($"*********************************subband {subbandid + 1} config bandwidth {bandwidth} Hz");
            }
        }
        #endregion

        #region FPGA传输定义
        private record DbiRegDefine(PcieBdReg.W Enable, PcieBdReg.W WEnable, PcieBdReg.W Data);

        private static Dictionary<ChannelId, DbiRegDefine> _RigisterTable = new()//?????
        {
            [ChannelId.C1] = new DbiRegDefine(PcieBdReg.W.DBI_LO_DBI_bin_en_ch1, PcieBdReg.W.DBI_LO_DBI_bin_wen_ch1, PcieBdReg.W.DBI_LO_DBI_bin_data_ch1),
            [ChannelId.C2] = new DbiRegDefine(PcieBdReg.W.DBI_LO_DBI_bin_en_ch2, PcieBdReg.W.DBI_LO_DBI_bin_wen_ch2, PcieBdReg.W.DBI_LO_DBI_bin_data_ch2),
            [ChannelId.C3] = new DbiRegDefine(PcieBdReg.W.DBI_LO_DBI_bin_en_ch3, PcieBdReg.W.DBI_LO_DBI_bin_wen_ch3, PcieBdReg.W.DBI_LO_DBI_bin_data_ch3),
            [ChannelId.C4] = new DbiRegDefine(PcieBdReg.W.DBI_LO_DBI_bin_en_ch4, PcieBdReg.W.DBI_LO_DBI_bin_wen_ch4, PcieBdReg.W.DBI_LO_DBI_bin_data_ch4),
        };

        private static void SendCmd(String cmdStr)
        {
            if (String.IsNullOrEmpty(cmdStr))
                return;
            foreach (DbiRegDefine reg in _RigisterTable.Values)
            {
                HdIO.WriteReg(reg.Enable, 0);
                for (Int32 id = 0; id < cmdStr.Length; id++)
                {
                    HdIO.WriteReg(reg.WEnable, 0);
                    HdIO.WriteReg(reg.Data, (UInt32)cmdStr[id]);
                    HdIO.WriteReg(reg.WEnable, 1);

                }
                HdIO.WriteReg(reg.WEnable, 0);
                HdIO.WriteReg(reg.Enable, 1);
            }

            Thread.Sleep(10);

            foreach (DbiRegDefine reg in _RigisterTable.Values)
            {
                HdIO.WriteReg(reg.Enable, 0);
            }
        }

        private static void SendCmd(ChannelId chnlId, String cmdStr)
        {
            if (String.IsNullOrEmpty(cmdStr))
                return;
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            byte channelBits = 0b1111;
            //List<Byte> sendData = new List<byte>
            //{
            //    (byte)(Hd.CurrDebugVarints.bEnable_OpenCrystal ? 0x01 : 0x00),
            //    channelBits
            //};
            List<Byte> sendData = new List<byte> { };
            List<ChannelId> resetResetHighVoltageWarnings = new List<ChannelId>();
           ;
            sendData.AddRange( System.Text.Encoding.UTF8.GetBytes(cmdStr).ToList());

            Hd.ResetHighVoltageWarning(resetResetHighVoltageWarnings);
            //UInt32 crcCode = CRC32.GetCRC32Code(sendData);
            //sendData.Add((byte)((crcCode & 0x00_00_00_ff) >> 0));
            //sendData.Add((byte)((crcCode & 0x00_00_ff_00) >> 8));
            //sendData.Add((byte)((crcCode & 0x00_ff_00_00) >> 16));
            //sendData.Add((byte)((crcCode & 0xff_00_00_00) >> 24));

            //ushort sum = 0;
            //for (int i = 0; i < sendData.Count - 4; i++)
            //    sum += sendData[i];
            //sendData.Add((byte)((sum & 0x00_ff) >> 0));
            //sendData.Add((byte)((sum & 0xff_00) >> 8));

            //CmdSerailNo++;
            //sendData.Add((byte)((CmdSerailNo & 0x00_ff) >> 0));
            //sendData.Add((byte)((CmdSerailNo & 0xff_00) >> 8));
            baseObj1.ClearSpecialReceiveQueue((byte)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet);
            baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet, sendData);
            #region 强制握手
            if (Constants.PRODUCT != ProductType.JiHe_MSO7000A)
            {
                bool execOK = false;
                if (baseObj1.ReadSpecailMessage((byte)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet, 60, true, out var readbackResult_execOK))
                {
                    if (readbackResult_execOK.Value.dataLength >= 2)
                    {
                        ushort readbackSerialNo = readbackResult_execOK.Value.Data[1];
                        readbackSerialNo <<= 8;
                        readbackSerialNo |= readbackResult_execOK.Value.Data[0];
                        if (readbackSerialNo == CmdSerailNo || readbackSerialNo == CmdSerailNo - 1)
                        {
                            execOK = true;
                        }
                    }
                }
                else
                    ;
                if (!execOK)
                {
                    baseObj1.SendData(true, (int)Updater_ReqScopeXommands.CMD0xC1_Request_CommunicateReset, null);
                    baseObj1.SendData(true, (int)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet, sendData);
                }
            }
            #endregion


            if (_RigisterTable.ContainsKey(chnlId))
            {
                DbiRegDefine reg = _RigisterTable[chnlId];

                HdIO.WriteReg(reg.Enable, 0);
                for (Int32 id = 0; id < cmdStr.Length; id++)
                {
                    HdIO.WriteReg(reg.WEnable, 0);
                    HdIO.WriteReg(reg.Data, (UInt32)cmdStr[id]);
                    HdIO.WriteReg(reg.WEnable, 1);

                }
                HdIO.WriteReg(reg.WEnable, 0);
                HdIO.WriteReg(reg.Enable, 1);

                Thread.Sleep(50);
                HdIO.WriteReg(reg.Enable, 0);
            }
        }
        
        //private static void SendCmd(ChannelId chnlId, String cmdStr)
        //{
        //    if (String.IsNullOrEmpty(cmdStr))
        //        return;
        //    if (_RigisterTable.ContainsKey(chnlId))
        //    {
        //        DbiRegDefine reg = _RigisterTable[chnlId];

        //        HdIO.WriteReg(reg.Enable, 0);
        //        for (Int32 id = 0; id < cmdStr.Length; id++)
        //        {
        //            HdIO.WriteReg(reg.WEnable, 0);
        //            HdIO.WriteReg(reg.Data, (UInt32)cmdStr[id]);
        //            HdIO.WriteReg(reg.WEnable, 1);

        //        }
        //        HdIO.WriteReg(reg.WEnable, 0);
        //        HdIO.WriteReg(reg.Enable, 1);

        //        Thread.Sleep(50);
        //        HdIO.WriteReg(reg.Enable, 0);
        //        Trace.WriteLine($"[SendCmd]{chnlId}:{cmdStr}");
        //    }
        //}
        #endregion

        #region 通道控制字定义
        #region 1.预衰减路径配置
        private const Int32 _PreattenuationStep = 5;
        private const Int32 _PreattenuationMin = 0;
        private const Int32 _PreattenuationMax = 315;

        internal enum PreAttenuationEnum
        {
            PassThrough = 0,
            Attenuation = 1,
            CaliSignalInput = 2,
            ChannelMultiplex = 3,
        };

        private static Dictionary<PreAttenuationEnum, String> _PreAttenuationDefine = new()
        {
            [PreAttenuationEnum.PassThrough] = "010",
            [PreAttenuationEnum.Attenuation] = "001",
            [PreAttenuationEnum.CaliSignalInput] = "000",
            [PreAttenuationEnum.ChannelMultiplex] = "011"
        };

        /// <summary>
        /// 获取预衰减路径配置的命令字符串
        /// </summary>
        /// <param name="preAttenuationType">直通/衰减</param>
        /// <param name="attenuationValue">衰减控制字，单位：0.1dB</param>
        /// <returns></returns>
        private static String GetPreattenuation(PreAttenuationEnum preAttenuationType, Int32 attenuationValue)
        {
            if (_PreAttenuationDefine.ContainsKey(preAttenuationType))
            {
                if (preAttenuationType == PreAttenuationEnum.Attenuation)
                {
                    Int32 cfgvalue = attenuationValue / _PreattenuationStep * _PreattenuationStep; // 调整为步进5的数值
                    if (cfgvalue < _PreattenuationMin) cfgvalue = _PreattenuationMin;
                    if (cfgvalue > _PreattenuationMax) cfgvalue = _PreattenuationMax;
                    return $"Com[0]={_PreAttenuationDefine[preAttenuationType]}{cfgvalue.ToString("000")}00@";
                }
                else
                {
                    return $"Com[0]={_PreAttenuationDefine[preAttenuationType]}00000@";
                }
            }
            Trace.WriteLine($"[GetPreattenuation]{preAttenuationType} not supported!");
            return String.Empty;
        }
        private static Dictionary<ChannelId, Int32[]> acqbddefine = new()
        {
            [ChannelId.C1] = new int[2] { 0, 1 },
            [ChannelId.C2] = new int[2] { 2, 3 },
            [ChannelId.C3] = new int[2] { 4, 5 },
            [ChannelId.C4] = new int[2] { 6, 7 },
        };
        #endregion

        #region 2.子带频率配置
        private static Int32 _SubbandFreMin = 10;
        private static Int32 _SubbandFreMax = 2000;

        /// <summary>
        /// 获取子带频率设置字符串
        /// </summary>
        /// <param name="subbandId">子带编号，从1开始</param>
        /// <param name="freqBy100Mhz">频率值，单位：百兆赫兹</param>
        /// <returns></returns>
        private static String GetSubbandFreq(Int32 subbandId, Int32 freqBy10Mhz)
        { 
            if (freqBy10Mhz < _SubbandFreMin) freqBy10Mhz = 0;
            if (freqBy10Mhz > _SubbandFreMax) freqBy10Mhz = _SubbandFreMax;
            return $"Com[{subbandId}]=0{freqBy10Mhz.ToString("0000")}@";
        }
        #endregion

        #region 3.子带衰减配置
        private const Int32 _Attenuation1Min = 0;
        private const Int32 _Attenuation1Max = 315;
        private const Int32 _Attenuation1Step = 5;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subbandId">子带编号，从1开始</param>
        /// <param name="attenuation">衰减控制字，单位：0.1dB</param>
        /// <returns></returns>
        private static String GetSubbandAttenuation(Int32 subbandId, Int32 attenuation)
        {
            Int32 cfg = attenuation / _Attenuation1Step * _Attenuation1Step;
            if (cfg < _Attenuation1Min) cfg = _Attenuation1Min;
            if (cfg > _Attenuation1Max) cfg = _Attenuation1Max;
            return $"Com[{subbandId}]=1{cfg.ToString("000")}@";
        }
        #endregion

        #region 4.第一子带带宽限制
        //internal enum DbiSubband1BandwidthEnum
        //{ 
        //    PassThrough = 0,
        //    Limit20MHz = 1,
        //    Limin1GHz = 2,
        //}

        //private Dictionary<DbiSubband1BandwidthEnum, String> _Subband1WidthDefine = new()
        //{
        //    [DbiSubband1BandwidthEnum.PassThrough] = "00",
        //    [DbiSubband1BandwidthEnum.Limit20MHz]  = "11",
        //    [DbiSubband1BandwidthEnum.Limin1GHz]   = "10",
        //};

        //private String GetSubband1Bandwidth(DbiSubband1BandwidthEnum bandwidth)
        //{
        //    if (_Subband1WidthDefine.ContainsKey(bandwidth))
        //    {
        //        return $"Com[1]=2{_Subband1WidthDefine[bandwidth]}0@";
        //    }
        //    Trace.WriteLine($"[GetSubband1Bandwidth]{bandwidth} not supported!");
        //    return String.Empty;
        //}
        #endregion

        #region 5.非第一子带的带宽限制配置
        internal enum DbiOthersSubbandBandwidthEnum
        {
            PassThrough  = 0,
            Limmit50MHz  = 1,
            Limmit500MHz = 2,
            Limmit1GHz   = 3,
        }

        private static Dictionary<DbiOthersSubbandBandwidthEnum, String> _OthersBindwidthDefine = new()
        {
            [DbiOthersSubbandBandwidthEnum.PassThrough]  = "100",
            [DbiOthersSubbandBandwidthEnum.Limmit50MHz]  = "111",
            [DbiOthersSubbandBandwidthEnum.Limmit500MHz] = "101",
            [DbiOthersSubbandBandwidthEnum.Limmit1GHz]   = "110",
        };

        /// <summary>
        /// 非第一子带的带宽限制配置
        /// </summary>
        /// <param name="subbandId">子带编号，从2开始</param>
        /// <param name="bandwidth">带宽限制枚举</param>
        /// <returns></returns>
        private static String GetOthersSubbandBindwidth(Int32 subbandId, DbiOthersSubbandBandwidthEnum bandwidth)
        {
            if (_OthersBindwidthDefine.ContainsKey(bandwidth))
            {
                return $"Com[6]={_OthersBindwidthDefine[bandwidth]}@";
            }
            return String.Empty;  
        }
        #endregion

        #region 6.第一子带偏置电源、基线调节、基准电压调节
        internal enum DbiSunbband1VoltageEnum
        { 
            BiasVoltage = 0,
            PosOffset = 1,
            RefVoltage = 2,
        }
        private static Dictionary<DbiSunbband1VoltageEnum, String> _Subband1VoltageDefine = new()
        {
            [DbiSunbband1VoltageEnum.BiasVoltage] = "030",
            [DbiSunbband1VoltageEnum.PosOffset] = "032",
            [DbiSunbband1VoltageEnum.RefVoltage] = "035",
        };

        private const Int32 _Subband1VoltageMin = 0;
        private const Int32 _Subband1VoltageMax = 65535;

        private static String GetSubband1Bias(DbiSunbband1VoltageEnum voltageType, Int32 voltageValue)
        {
            if (_Subband1VoltageDefine.ContainsKey(voltageType))
            {
                if (voltageValue < _Subband1VoltageMin) voltageValue = _Subband1VoltageMin;
                if (voltageValue > _Subband1VoltageMax) voltageValue = _Subband1VoltageMax;
                return $"Com[1]={_Subband1VoltageDefine[voltageType]}{voltageValue.ToString("00000")}@";
            }
            Trace.WriteLine($"[GetSubband1Bias]{voltageType} not supported!");
            return String.Empty;
        }



        internal enum DbiSunbbandVoltageEnum
        {
            SecondBias = 0,
            FirstBias = 1,
            SecondOffset = 2,
            FirstOffset = 3,
            RefVoltage=4,
        }

        private static Dictionary<DbiSunbbandVoltageEnum, String> _SubbandVoltageDefine = new()
        {
            [DbiSunbbandVoltageEnum.SecondBias] = "330",
            [DbiSunbbandVoltageEnum.FirstBias] = "331",
            [DbiSunbbandVoltageEnum.SecondOffset] = "333",
            [DbiSunbbandVoltageEnum.FirstOffset] = "332",
            [DbiSunbbandVoltageEnum.RefVoltage] = "334",
        };


        private const Int32 _SubbandVoltageMin = 0;
        private const Int32 _SubbandVoltageMax = 65535;
        private static String GetSubbandBiasOffsetVoltage(DbiSunbbandVoltageEnum voltageType, Int32 voltageValue)
        {
            if (_SubbandVoltageDefine.ContainsKey(voltageType))
            {
                if (voltageValue < _SubbandVoltageMin) voltageValue = _SubbandVoltageMin;
                if (voltageValue > _SubbandVoltageMax) voltageValue = _SubbandVoltageMax;
                return $"Com[1]={_SubbandVoltageDefine[voltageType]}{voltageValue.ToString("00000")}@";
            }
            Trace.WriteLine($"[GetSubband1Bias]{voltageType} not supported!");
            return String.Empty;
        }
        #endregion  

        #region 子带固定本振频率配置
        private static String GetSubbandLocalOscillatorFreq(Int32 subbandId)
        {
            return $"Com[{subbandId}]=5000@";
        }

        #endregion

        #region 7.第一子带采样方式配置
        internal enum Subband1SampleTypeEnum
        { 
            DirectSample = 0,
            MixSample = 1,
        }

        internal enum Subband1SampleGainTypeEnum
        {
            FirstGain = 0,
            SecondGain = 1,
        }

        private static Dictionary<Subband1SampleTypeEnum, String> _Subband1SampleTypeDefine = new()
        {
            [Subband1SampleTypeEnum.DirectSample] = "0",
            [Subband1SampleTypeEnum.MixSample] = "1",
        };
        private static Dictionary<Subband1SampleGainTypeEnum, String> _Subband1SampleGainTypeDefine = new()
        {
            [Subband1SampleGainTypeEnum.FirstGain] = "0",
            [Subband1SampleGainTypeEnum.SecondGain] = "1",
        };

        private static String GetSubband1SampleType(Subband1SampleTypeEnum sampleType, Subband1SampleGainTypeEnum gaintype)
        {
            if (_Subband1SampleTypeDefine.ContainsKey(sampleType)&& _Subband1SampleGainTypeDefine.ContainsKey(gaintype))
            {
                return $"Com[1]=6{_Subband1SampleTypeDefine[sampleType]}{_Subband1SampleGainTypeDefine[gaintype]}0@";
            }
            return String.Empty;
        }
        #endregion

        #region 7.非第一子带采样方式配置
        internal enum SubbandSampleTypeEnum
        {
            DirectSample = 0,
            MixSample = 1,
        }

        internal enum SubbandSampleGainTypeEnum
        {
            FirstGain = 0,
            SecondGain = 1,
        }

        private static Dictionary<SubbandSampleTypeEnum, String> _SubbandSampleTypeDefine = new()
        {
            [SubbandSampleTypeEnum.DirectSample] = "0",
            [SubbandSampleTypeEnum.MixSample] = "1",
        };
        private static Dictionary<SubbandSampleGainTypeEnum, String> _SubbandSampleGainTypeDefine = new()
        {
            [SubbandSampleGainTypeEnum.FirstGain] = "0",
            [SubbandSampleGainTypeEnum.SecondGain] = "1",
        };

        private static String GetSubbandGainType(int subbandidx, SubbandSampleGainTypeEnum gaintype)
        {
            if ( _SubbandSampleGainTypeDefine.ContainsKey(gaintype))
            {
                return $"Com[{subbandidx}]=2{_SubbandSampleGainTypeDefine[gaintype]}00@";
            }
            return String.Empty;
        }
        #endregion

        #region 8.第一子带采样率配置
        private static Dictionary<Int32, String> _SampeFreqDefine = new()
        {
            [20] = "0",
            [80] = "1",
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fsByGhz"></param>
        /// <returns></returns>
        private static String GetSubband1SampleFreq(Int32 fsByGhz)
        {
            if (_SampeFreqDefine.ContainsKey(fsByGhz))
            { 
                return $"Com[1]=7{_SampeFreqDefine[fsByGhz]}00@";
            }
            return String.Empty;
        }
        #endregion

        #region 9.非第一子带的采样率配置
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subbandId">子带编号，从1开始</param>
        /// <param name="fsByGhz"></param>
        /// <returns></returns>
        private static String GetSubbandSampleFreq(Int32 subbandId, Int32 fsByGhz)
        {
            if (_SampeFreqDefine.ContainsKey(fsByGhz))
            {
                return $"Com[{subbandId}]=3{_SampeFreqDefine[fsByGhz]}00@";
            }
            return String.Empty;
        }
        #endregion        

        #region 10.校正源频率配置
        private const Int32 _CaliSrcFreqMin = 1;
        private const Int32 _CaliSrcFreqMax = 2000;
        internal enum SrcTypeEnum
        {
            SingleFreq = 0,
            FastEdge = 1,
        }

        private static Dictionary<SrcTypeEnum, String> _SrcTypeEnumDefine = new()
        {
            [SrcTypeEnum.SingleFreq] = "0",
            [SrcTypeEnum.FastEdge] = "2",
        };
        private static String GetCaliSrcFreq(Boolean enable, UInt32 freqBy10Mhz, SrcTypeEnum srcTypeEnum)
        {
            if (!enable)
            {
                return "Com[5]=1000@";
            }
            if (freqBy10Mhz < _CaliSrcFreqMin) freqBy10Mhz = _CaliSrcFreqMin;
            if (freqBy10Mhz > _CaliSrcFreqMax) freqBy10Mhz = _CaliSrcFreqMax;
            if (_SrcTypeEnumDefine.ContainsKey(srcTypeEnum))
            { 
                return $"Com[5]={_SrcTypeEnumDefine[srcTypeEnum]}{freqBy10Mhz.ToString("0000")}000@";
            }
            return String.Empty;
        }
        #endregion

        #region Buffer配置

        internal enum DBISampleRateEnum
        {
            BD20GSPS = 0,
            BD80GSPS = 1,
        }

        private static Dictionary<DBISampleRateEnum, String> _DBISampleRateEnumDefine = new()
        {
            [DBISampleRateEnum.BD20GSPS] = "0",
            [DBISampleRateEnum.BD80GSPS] = "1",
        };
        private static String GetCaliSrcFreq(int subbandidx, DBISampleRateEnum dbiSampleRateEnum)
        {
            if (_DBISampleRateEnumDefine.ContainsKey(dbiSampleRateEnum))
            {
                return $"Com[6]=0{subbandidx}{_DBISampleRateEnumDefine[dbiSampleRateEnum]}@";
            }
            return String.Empty;
        }
        #endregion
        #endregion

        #region 外部接口
        internal static void Init()
        {
            ConfigRefVoltage();
        }
        internal static Boolean PowerOn(bool isUpdate = false)
        {
            if (baseObj1.Connected)
            {
                Hd.SysLogger?.Invoke("通道板串口已被打开!", "Warning");
                return false;
            }
            var result = baseObj1.Open($"COM{Constants.COMPORTNUM_ANALGCHANNEL1}", 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
            if (!result)
            {
                Hd.SysLogger?.Invoke("通道板串口打开失败!", "Warning");
                return false;
            }
            if (Constants.BOARD_ATTACHED)
            {
                baseObj1.SendData(true, (byte)Updater_ReqScopeXommands.CMD0xC1_Request_CommunicateReset, null);
                Thread.Sleep(500);
                var isBoot = false;
                var isApp = baseObj1.McuUpdate_IsRunAtApp(1500);
                if (!isApp)
                {
                    isBoot = baseObj1.McuUpdate_IsRunAtBoot(150);
                }

                if (isUpdate)
                {
                    result = isBoot || isApp;
                }
                else
                {
                    result = isApp;
                }
                if (!result)
                {
                    //上电失败
                    Hd.SysLogger?.Invoke("通道板上电失败!", "Warning");
                    //return false;
                }
                baseObj1.RegisterAppRunStartTime();
                Hd.SysLogger?.Invoke($"ChannelMcuLastUpdateTime:{baseObj1.ReadUpdatetimeStamp()}", "Info");
                InitRefDac();
            }
            return true;
        }

        internal static new void PowerOff()
        {
            COMPort_PowerCtrl(false);
        }
        public void FPGAReg_AnalogChannelSet()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            stopwatch.Start();
            CtrlOffset();
            CtrlGain();
            CtrlBias();
            //GeneratedAndSend4094CtrlWord();
            //ConfigBandwidth();
            stopwatch.Stop();
            var sss = stopwatch.ElapsedMilliseconds;




        }

        internal new static void COMPort_AnalogChannelSet()
        {
            if (!COMPort_Check() || Hd.UIMessage?.Analog?[0] == null)
                return;
            byte channelBits = 0b1111;
            List<Byte> sendData = new List<byte>
            {
                (byte)(Hd.CurrDebugVarints.bEnable_OpenCrystal ? 0x01 : 0x00),
                channelBits
            };


            CtrlOffset();
            CtrlGain();
            CtrlBias();

            List<ChannelId> resetResetHighVoltageWarnings = new List<ChannelId>();

            #region old 

            //        for (Int32 channelIndex = (int)ChannelId.C1; channelIndex < ChannelIdExt.AnaChnlNum; channelIndex++)
            //        {
            //            HdMessage.AnalogOptions analogParas = Hd.UIMessage.Analog[channelIndex];
            //            int Impedance_H_is0 = analogParas.Coupling == AnaChnlCoupling.DC50 ? 1 : 0;
            //            (int ScaleIndex, int ScaleValueByuV, int Gain_FineByFpgaThousand) bestScaleIndexFpgaFine = GetCurrentScaleIndex(channelIndex);
            //            int yScaleIndex = bestScaleIndexFpgaFine.ScaleIndex;
            //            AnalogChannelItem_Base chnlParams = ProductDataTranslate_MSO8000X.GetChnlParamsItem(
            //                new((ChannelId)channelIndex, Impedance_H_is0 == 0, (uint)(bestScaleIndexFpgaFine.ScaleValueByuV / 1000)))!.Value;



            //            #region old8000

            //            #region 是否过FG10(0-不过，1-过)
            //            //当前垂直档位<=20mv时为高增益(过FG),其他为低增益(不过FG)
            //            if (yScaleIndex <= (int)AnaChnlScaleIndex.Lv20m)
            //                sendData.Add(1);
            //            else
            //                sendData.Add(0);
            //            #endregion

            //            #region 放大倍数
            //            //说明：MSO8000HD有两个DSA，每个DSA的寄存器设置范围为[0,127]
            //            int GainCtr = Math.Min((Math.Max(chnlParams.Gain, 0)), 254);
            //            var attPair = Get5GDsaCtrlWord(Impedance_H_is0 == 0, (AnaChnlScaleIndex)yScaleIndex, GainCtr);

            //            sendData.Add((byte)attPair.Dsa1);
            //            sendData.Add((byte)attPair.Dsa2);
            //            #endregion 放大倍数

            ////            #region 衰减倍数           
            ////            HdMessage.AnalogOptions phychannel = Hd.UIMessage!.Analog![channelIndex];
            ////            bool result;

            ////            // 默认使用高阻的幅度衰减表，如果是低阻，显式地进行更换
            ////            Dictionary<AnaChnlScaleIndex, UInt32> attTable = (phychannel.Coupling == AnaChnlCoupling.DC50) ? newAttTableLz : newAttTableHz;
            ////            result = attTable.TryGetValue((AnaChnlScaleIndex)bestScaleIndexFpgaFine.ScaleIndex, out UInt32 att);
            ////            if (!result)
            ////                return;
            ////            sendData.Add((byte)att);
            ////            #endregion 衰减倍数

            ////            #region 耦合 阻抗
            ////            //DC-0/AC-1
            ////            byte coupling = 0;
            ////            //高阻-0/低阻-1
            ////            byte impedance = 0;
            ////            switch (phychannel.Coupling)
            ////            {
            ////                case AnaChnlCoupling.DC1M:
            ////                    coupling = 0;
            ////                    impedance = 0;
            ////                    break;
            ////                case AnaChnlCoupling.AC1M:
            ////                    coupling = 1;
            ////                    impedance = 0;
            ////                    break;
            ////                case AnaChnlCoupling.DC50:
            ////                    coupling = 0;
            ////                    impedance = 1;
            ////                    resetResetHighVoltageWarnings.Add((ChannelId)channelIndex);
            ////                    break;
            ////                case AnaChnlCoupling.Gnd:
            ////                    coupling = 1;
            ////                    impedance = 0;
            ////                    break;
            ////                default:
            ////                    return;
            ////            }
            ////            sendData.Add(coupling);
            ////            sendData.Add(impedance);
            ////            #endregion 耦合 阻抗

            ////            #region 偏置DAC
            ////            int Pre0Div = chnlParams.Bias;
            ////            uint moveFactorScaleMv = GetDACScaleMv((AnaChnlScaleIndex)yScaleIndex, attTable);
            ////            double MoveFactor = ProductDataTranslate_MSO8000X.GetChnlParamsItem(new((ChannelId)channelIndex, Impedance_H_is0 == 0, moveFactorScaleMv))!
            ////                .Value.Bias_Pos3Div / 100_000_000D;  //Bias_Pos3Div表示：斜率放大100_000_000倍的值。该值的大小可以理解为最大输入电平的值(以uV为单位)
            ////            MoveFactor = analogParas.Bias >= 0 ? MoveFactor : ProductDataTranslate_MSO8000X.GetChnlParamsItem(new((ChannelId)channelIndex, Impedance_H_is0 == 0, moveFactorScaleMv))!
            ////.Value.Bias_Neg3Div / 100_000_000D;
            ////            Int32 Bias = Pre0Div - (Int32)((double)analogParas.Bias * MoveFactor);
            ////            sendData.Add((byte)(Bias & 0xFF));
            ////            sendData.Add((byte)((Bias >> 8) & 0xFF));
            ////            #endregion 偏置DAC

            ////            #region offset DAC
            ////            Int32 pos0Div = (Int32)AutoCaliParams.Default[channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior;
            ////            Double pos3Div_P = AutoCaliParams.Default[channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior_3Div;
            ////            Double pos3div_N = AutoCaliParams.Default[channelIndex, Impedance_H_is0, yScaleIndex].OffsetPosterior_N3Div;

            ////            #region 自校准+温补

            ////            #endregion
            ////            Int32 posPosition = (Int32)(Constants.SAMPS_PER_YDIV * (analogParas.Position) * 1000 / bestScaleIndexFpgaFine.ScaleValueByuV);
            ////            Int32 posPositionCtrl = 0;
            ////            if (posPosition > 0)
            ////            {
            ////                posPositionCtrl = (Int32)(posPosition * pos3Div_P / (Constants.SAMPS_PER_YDIV * 3));
            ////            }
            ////            else
            ////            {
            ////                posPositionCtrl = (Int32)(posPosition * pos3div_N / (Constants.SAMPS_PER_YDIV * 3));
            ////            }

            ////            //反向
            ////            if (analogParas.IsInverted)
            ////                posPositionCtrl = pos0Div - posPositionCtrl;
            ////            else
            ////                posPositionCtrl = pos0Div + posPositionCtrl;

            ////            sendData.Add((byte)(posPositionCtrl & 0xFF));
            ////            sendData.Add((byte)((posPositionCtrl >> 8) & 0xFF));
            ////            //CtrlOffset(false); //test
            ////            #endregion offset DAC

            ////            #region 带宽
            ////            // 带宽
            ////            UInt16 bandwidthByMHz;
            ////            switch (phychannel.Bandwidth)
            ////            {
            ////                case 0: // LZ HZ FULL
            ////                    bandwidthByMHz = 0xFFFF;
            ////                    break;
            ////                case 1: // LZ 500M
            ////                    bandwidthByMHz = 500;
            ////                    break;
            ////                case 2: // LZ 200M
            ////                    bandwidthByMHz = 200;
            ////                    break;
            ////                case 3: // LZ HZ 20M
            ////                    bandwidthByMHz = 20;
            ////                    break;
            ////                default:
            ////                    return;
            ////            }
            ////            sendData.Add((byte)(bandwidthByMHz & 0xFF));
            ////            sendData.Add((byte)((bandwidthByMHz >> 8) & 0xFF));
            ////            #endregion 带宽
            //            #endregion


            //        }
            #endregion

            Hd.ResetHighVoltageWarning(resetResetHighVoltageWarnings);
            UInt32 crcCode = CRC32.GetCRC32Code(sendData);
            sendData.Add((byte)((crcCode & 0x00_00_00_ff) >> 0));
            sendData.Add((byte)((crcCode & 0x00_00_ff_00) >> 8));
            sendData.Add((byte)((crcCode & 0x00_ff_00_00) >> 16));
            sendData.Add((byte)((crcCode & 0xff_00_00_00) >> 24));

            ushort sum = 0;
            for (int i = 0; i < sendData.Count - 4; i++)
                sum += sendData[i];
            sendData.Add((byte)((sum & 0x00_ff) >> 0));
            sendData.Add((byte)((sum & 0xff_00) >> 8));

            CmdSerailNo++;
            sendData.Add((byte)((CmdSerailNo & 0x00_ff) >> 0));
            sendData.Add((byte)((CmdSerailNo & 0xff_00) >> 8));
            baseObj1.ClearSpecialReceiveQueue((byte)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet);
            baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet, sendData);
            #region 强制握手
            if (Constants.PRODUCT != ProductType.JiHe_MSO7000A)
            {
                bool execOK = false;
                if (baseObj1.ReadSpecailMessage((byte)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet, 60, true, out var readbackResult_execOK))
                {
                    if (readbackResult_execOK.Value.dataLength >= 2)
                    {
                        ushort readbackSerialNo = readbackResult_execOK.Value.Data[1];
                        readbackSerialNo <<= 8;
                        readbackSerialNo |= readbackResult_execOK.Value.Data[0];
                        if (readbackSerialNo == CmdSerailNo || readbackSerialNo == CmdSerailNo - 1)
                        {
                            execOK = true;
                        }
                    }
                }
                else
                    ;
                if (!execOK)
                {
                    baseObj1.SendData(true, (int)Updater_ReqScopeXommands.CMD0xC1_Request_CommunicateReset, null);
                    baseObj1.SendData(true, (int)AnalogChannelReqScopeXommands.CMD0x22_Request_AnalogChannelSet, sendData);
                }
            }
            #endregion

            if (CmdSerailNo == ushort.MaxValue)
                CmdSerailNo = 0;

            #region 测试代码
            if (bAnalogChannelComm)
            {

                //通道控制测试
                Thread.Sleep(50);
                List<byte> readBackSendData = new List<byte> { 0 };
                baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0xE3_Request_GetDebugData, readBackSendData);
                string errorStr = "";
                if (baseObj1.ReadSpecailMessage((byte)AnalogChannelReqScopeXommands.CMD0xE3_Request_GetDebugData, 1000, true, out var readbackResult))
                {
                    if (readbackResult.Value.dataLength >= 41)
                    {
                        for (int i = 0; i < 41; i++)
                        {
                            if (readbackResult.Value.Data[i] != sendData[i])
                            {
                                errorStr = errorStr + $"at index={i},sourceData={sendData[i]} != readbackData={readbackResult.Value.Data[i]}";
                            }

                        }
                    }
                    else
                        ;
                }
                else
                    ;
                if (errorStr.Trim() != "")
                    Hd.SysLogger?.Invoke($"AnalogChannel Ctrl Error:{errorStr}", "Warnning");
                else
                    ;

                if (lastSendData.Count > 1)
                {
                    lastSendData.RemoveAt(0);
                }
                lastSendData.Add(sendData);



                List<byte> readBackSendData2 = new List<byte> { 2 };
                baseObj1.SendData(false, (int)AnalogChannelReqScopeXommands.CMD0xE3_Request_GetDebugData, readBackSendData2);
                if (baseObj1.ReadSpecailMessage((byte)AnalogChannelReqScopeXommands.CMD0xE3_Request_GetDebugData, 1000, true, out var readbackResult2))
                {
                    if (readbackResult2.Value.dataLength >= 8)
                    {
                        if (last409[0] == 0xff)
                        {
                            for (int i = 0; i < 8; i++)
                                last409[i] = readbackResult2.Value.Data[i];
                        }
                        for (int i = 0; i < 8; i++)
                        {
                            if (last409[i] != readbackResult2.Value.Data[i])
                                ;
                        }
                    }
                    else
                        ;
                }
            }
            if (bTestComm)
            {
                //单独通信测试
                List<byte> testData = new List<byte>();
                for (byte i = 0; i < 128; i++)
                    testData.Add((byte)(i & 0xFF));
                for (int testTimes = 0; testTimes < 1000; testTimes++)
                {
                    baseObj1.SendData(true, (int)AnalogChannelReqScopeXommands.CMD0xE4_Request_Communication, testData);
                    if (baseObj1.ReadSpecailMessage((byte)AnalogChannelReqScopeXommands.CMD0xE4_Request_Communication, 1000, true, out var readbackResult_test))
                    {
                        if (readbackResult_test.Value.dataLength >= 128)
                        {
                            for (int i = 0; i < 128; i++)
                            {
                                if (readbackResult_test.Value.Data[i] != testData[i])
                                    ;
                            }
                        }
                    }
                }
            }
            #endregion

        }

        private static void SendCmd_ComPort(String cmdStr)
        {
            if (String.IsNullOrEmpty(cmdStr))
                return;
            



            Trace.WriteLine($"[SendCmd]:{cmdStr}");
        }

        internal static void CtrlOffset()
        {
            if (Hd.UIMessage?.Analog == null)
                return;
            //for (Int32 chnlid = 0; chnlid < ChannelIdExt.AnaChnlNum; chnlid++)
            for (Int32 chnlid = 0; chnlid < 1; chnlid++)
            {
                if (chnlid >= Hd.UIMessage.Analog.Length)
                    continue;
                var analogparams = Hd.UIMessage.Analog[chnlid];
                Int32 scale = analogparams.ScaleIndex;
                String chnlname = GetChnlName((ChannelId)chnlid, (AnaChnlScaleIndex)scale);
                Int32 pos0div = AiAnalogChannelParams.Default[chnlname].Subband1Offset;
                Int32 pos3div = AiAnalogChannelParams.Default[chnlname].Subband1Offset3Div;

                //Int32 offset = (Int32)(Constants.SAMPS_PER_YDIV * analogparams.Position / analogparams.Scale);
                //offset = (Int32)(offset * pos3div / (Constants.SAMPS_PER_YDIV * 3));
                //if (analogparams.IsInverted)
                //    offset = pos0div - offset;
                //else
                //    offset = pos0div + offset;
                Int32 offset_FG = pos0div;
                Int32 offset_FD = pos3div;
                String cmd_FG = "";
                String cmd_FD = "";

                cmd_FG = GetSubbandBiasOffsetVoltage(DbiSunbbandVoltageEnum.SecondOffset, offset_FG);
                cmd_FD = GetSubbandBiasOffsetVoltage(DbiSunbbandVoltageEnum.FirstOffset, offset_FD);
                SendCmd((ChannelId)chnlid, cmd_FG);
                SendCmd((ChannelId)chnlid, cmd_FD);
            }
        }



        internal static void CtrlBias()
        {
            ConfigRefVoltage();
            if (Hd.UIMessage?.Analog == null)
                return;
            //for (Int32 chnlid = 0; chnlid < ChannelIdExt.AnaChnlNum; chnlid++)
            for (Int32 chnlid = 0; chnlid < 1; chnlid++)
            {
                if (chnlid >= Hd.UIMessage.Analog.Length)
                    continue;
                var analogparams = Hd.UIMessage.Analog[chnlid];
                Int32 scale = analogparams.ScaleIndex;
                String chnlname = GetChnlName((ChannelId)chnlid, (AnaChnlScaleIndex)scale);
                Int32 pos0div = AiAnalogChannelParams.Default[chnlname].Subband1Bias;
                Int32 pos3div = AiAnalogChannelParams.Default[chnlname].Subband1Bias3Div;

                Int32 bias = (Int32)(Constants.SAMPS_PER_YDIV * analogparams.Position / analogparams.Scale);
                bias = (Int32)(bias * (pos3div-pos0div) / (Constants.SAMPS_PER_YDIV * 3));
                if (analogparams.IsInverted)
                    bias = pos0div - bias;
                else
                    bias = pos0div + bias;

                //Int32 bias = (Int32)Math.Round(analogparams.Bias * pos3div);
                //bias = pos0div - bias;
                //String cmd = GetSubband1Bias(DbiSunbband1VoltageEnum.BiasVoltage, bias);
                String cmd = "";
                //if (analogparams.ScaleBymV >= 50)
                //{
                //    cmd = GetSubbandBiasOffsetVoltage(DbiSunbbandVoltageEnum.FirstBias, bias);
                //}
                //else
                //{
                //    cmd = GetSubbandBiasOffsetVoltage(DbiSunbbandVoltageEnum.SecondBias, bias);
                //}
                cmd = GetSubbandBiasOffsetVoltage(DbiSunbbandVoltageEnum.SecondBias, bias); //only second htf
                SendCmd((ChannelId)chnlid, cmd);
            }
            
            //ConfigLocalFreq(_DefaultLocalFreqByHz);               //HTF_tmp
            ConfigSubbandBandWidth(_DefaultSubbandBandwidth);
            ConfigSampleFreq(ChannelId.C1);
            ConfigSamleType();
            //SendCmd(ChannelId.C1, "Com[0]=00000000@");            // 校准源
            //SendCmd(ChannelId.C1, "Com[5]=2005000@");
        }

        internal static void CtrlGain()
        {
            CtrlAnalogChannel_DBI20G.SendDiscard();

            if (Hd.UIMessage?.Analog == null)
                return;
            //for (Int32 chnlid = 0; chnlid < ChannelIdExt.AnaChnlNum; chnlid++)
            for (Int32 chnlid = 0; chnlid < 1; chnlid++)
            {
                if (chnlid >= Hd.UIMessage.Analog.Length)
                    continue;
                var analogparams = Hd.UIMessage.Analog[chnlid];
                Int32 scale = analogparams.ScaleIndex;
                String chnlname = GetChnlName((ChannelId)chnlid, (AnaChnlScaleIndex)scale);

                Int32 preattenuation = AiAnalogChannelParams.Default[chnlname].PreAttenuation;
                PreAttenuationEnum preattenuationenum = preattenuation == 0 ? PreAttenuationEnum.PassThrough : PreAttenuationEnum.Attenuation;
                //switch (preattenuation)
                //{
                //    case 0:
                //        preattenuationenum = PreAttenuationEnum.PassThrough;
                //        break;
                //    case 1:
                //        preattenuationenum = PreAttenuationEnum.Attenuation;
                //        break;
                //    case 2:
                //        preattenuationenum = PreAttenuationEnum.CaliSignalInput;
                //        break;
                //    default:
                //        break;
                //}
                String prestr = GetPreattenuation(preattenuationenum, preattenuation);
                SendCmd((ChannelId)chnlid, prestr);

                Int32[] subbandattenuationtable = new Int32[]
                {
                    AiAnalogChannelParams.Default[chnlname].Subband1Attenuation,
                    AiAnalogChannelParams.Default[chnlname].Subband2Attenuation,
                    AiAnalogChannelParams.Default[chnlname].Subband3Attenuation,
                    AiAnalogChannelParams.Default[chnlname].Subband4Attenuation,
                };
                for (Int32 subbandid = 0; subbandid < subbandattenuationtable.Length; subbandid++)
                {
                    String subbandstr = GetSubbandAttenuation(subbandid + 1, subbandattenuationtable[subbandid]);
                    SendCmd((ChannelId)chnlid, subbandstr);
                    if (subbandid > 0)
                    {
                        //var gaintype = Hd.UIMessage.Analog[chnlid].ScaleBymV < 50 ? SubbandSampleGainTypeEnum.SecondGain : SubbandSampleGainTypeEnum.FirstGain;
                        var gaintype = SubbandSampleGainTypeEnum.SecondGain;//only second htf
                        String gainstr = GetSubbandGainType(subbandid+1, gaintype);
                        SendCmd((ChannelId)chnlid, gainstr);
                    }
                }
            }
            CtrlGainByFpga();
        }
        internal static void CtrlGainByFpga()
        {
            Hd.CurrProduct.AcqBd?.MiscFunc("CtrlGainByFpga");
          
        }
        internal static void CtrlBandwidth()
        { 
            
        }

        internal static void CtrlAdvancedChannel()
        {

        }


        //internal void CtrlFilterWord()
        //{
        //    ConfigFreq();  //发送子带频率配置、第一子带带宽限制、非第一子带带宽限制、第一子带采样方式配置、第一子带采样率配置、非第一子带采样率配置
        //    ConfigSampleFreq();
        //    ConfigSamleType();
        //    ConfigBandwidth();
        //}
        #endregion

        internal static String CtrlInnerSource(Boolean enableInnerSource, ChannelId channelId, DbiSunbbandCaliSourceEnum? signalTypes, UInt32? freqByMhz, Boolean offsetEnable)
        {
            if (enableInnerSource)
            {
                if (signalTypes == null)
                    return "signalTypes can't null if enableInnerSource=true!";
                if (freqByMhz == null)
                    return "includeChannels can't null if enableInnerSource=true!";
                if (offsetEnable == null)
                    return "gainAdditionalByDB can't null if enableInnerSource=true!";
            }
            String cmd = "";
            if (!enableInnerSource)
            {
                cmd = "Com[5]=1000@";//关闭
                SendCmd(channelId, cmd);
                //CtrlGain(); //重发增益控制，实现预衰减路径的切换
                return "";
            }
            cmd = "Com[0]=00000000@";
            SendCmd(channelId, cmd); //切换预衰减路径到校准信号
            //String keyStr = $"{channelId}_Subband{0 + 1}_UsedForInterSourceFixed";
            //if (offsetEnable)
            //{
            //    CtrlBias_BySpecial((int)ChannelId.C1, 0, keyStr); //开启偏置时，从子带1拿取偏置量
            //}
            //else
            //{
            //    CtrlBias_BySpecial((int)ChannelId.C1, 1, keyStr); //关闭偏置时，从子带2拿取偏置量
            //}

            //for (int subbandIndex = 0; subbandIndex < 4; subbandIndex++)
            //{
            //    keyStr = $"{channelId}_Subband{subbandIndex + 1}_UsedForInterSourceFixed";

            //    CtrlGain_BySpecial((int)channelId, subbandIndex, keyStr, (double)0); //增益系数每个子带轮流发送
            //}
            if (freqByMhz < _Subband1CaliSourceMinBy10MHz * 10) freqByMhz = _Subband1CaliSourceMinBy10MHz;
            if (freqByMhz > _Subband1CaliSourceMaxBy10MHz * 10) freqByMhz = _Subband1CaliSourceMaxBy10MHz;
            if (signalTypes == DbiSunbbandCaliSourceEnum.FastEdge)
            {
                freqByMhz = freqByMhz;
            }
            else if (signalTypes == DbiSunbbandCaliSourceEnum.SingleTone)
            {
                freqByMhz = freqByMhz / 10;
            }
            cmd = $"Com[5]={_Subband1CaliSourceDefine[(DbiSunbbandCaliSourceEnum)signalTypes]}{((uint)freqByMhz).ToString("0000")}{"000"}@";
            SendCmd(channelId, cmd);
            return "";
        }
        internal static void CtrlBias_BySpecial(Int32 channelID, Int32 subbandIndex, String subbandKeyStr)
        {
            if (Hd.UIMessage?.Analog == null)
                return;

            String keyStr = subbandKeyStr;

            Int32 data = 0;
            String cmd = "";
            #region Part1:第一子带第一级放大偏置电源调节
            data = (Int32)DbiAnalogParams_Common.Default[keyStr][(int)AnalogChannelItems_DBI20G.PreBias];
            cmd = $"Com[1]=230{data.ToString("00000")}@";
            SendCmd((ChannelId)subbandIndex, cmd);
            #endregion

            #region Part2:第一子带第二级放大偏置电源调节
            data = (Int32)DbiAnalogParams_Common.Default[keyStr][(int)AnalogChannelItems_DBI20G.PostBias];
            cmd = $"Com[1]=231{data.ToString("00000")}@";
            SendCmd((ChannelId)subbandIndex, cmd);
            #endregion

            #region Part3:第一子带输入偏置调节
            data = (Int32)DbiAnalogParams_Common.Default[keyStr][(int)AnalogChannelItems_DBI20G.InputBias];
            cmd = $"Com[1]=232{data.ToString("00000")}@";
            SendCmd((ChannelId)subbandIndex, cmd);
            #endregion

            #region Part4:第一子带FG1同向输入端电压调节
            data = (Int32)DbiAnalogParams_Common.Default[keyStr][(int)AnalogChannelItems_DBI20G.Op_FG1_RefVoltage];
            cmd = $"Com[1]=233{data.ToString("00000")}@";
            SendCmd((ChannelId)subbandIndex, cmd);
            #endregion

            #region Part5:第一子带FG2同向输入端电压调节
            data = (Int32)DbiAnalogParams_Common.Default[keyStr][(int)AnalogChannelItems_DBI20G.Op_FG2_RefVoltage];
            cmd = $"Com[1]=234{data.ToString("00000")}@";
            SendCmd((ChannelId)subbandIndex, cmd);
            #endregion

            #region Part5:第一子带VOS同向输入端电压调节
            data = (Int32)DbiAnalogParams_Common.Default[keyStr][(int)AnalogChannelItems_DBI20G.Op_VOS_RefVoltage];
            cmd = $"Com[1]=235{data.ToString("00000")}@";
            SendCmd((ChannelId)subbandIndex, cmd);
            #endregion
        }
        internal static void CtrlGain_BySpecial(Int32 channelID, Int32 subbandIndex, String subbandKeyStr, double gainAdditionalByDB)
        {
            #region 衰减路径
            Int32 road = (Int32)DbiAnalogParams_Common.Default[subbandKeyStr][(int)AnalogChannelItems_DBI20G.GainRoadSelect];
            string cmd = (road == 0) ? $"Com[{subbandIndex + 1}]=00000000@" : $"Com[{subbandIndex + 1}]=01100000@";//00代表一级放大路径（最大增益0dB），11代表二级放大路径（最大增益18dB）
            SendCmd((ChannelId)channelID, cmd);
            #endregion
            #region P_N 衰减
            //P 衰减
            int data = (Int32)DbiAnalogParams_Common.Default[subbandKeyStr][(int)AnalogChannelItems_DBI20G.P_Attenuation];
            //gainAdditionalByDB--用于补偿内部源在的幅频特性，非开机自动校准的情况下，都为0
            data += (int)(gainAdditionalByDB / 0.5);
            cmd = $"Com[{subbandIndex + 1}]=100{data.ToString("000")}00@";
            SendCmd((ChannelId)channelID, cmd);
            if (subbandIndex == 0)
            {
                //N 衰减--只有子带1有N衰减
                data = (Int32)DbiAnalogParams_Common.Default[subbandKeyStr][(int)AnalogChannelItems_DBI20G.N_Attenuation];
                data += (int)(gainAdditionalByDB / 0.5);
                cmd = $"Com[{subbandIndex + 1}]=101{data.ToString("000")}00@";
                SendCmd((ChannelId)channelID, cmd);
            }
            #endregion
        }

        internal static String GetAnalogChannelKey(ChannelId channelId, AnaChnlScaleIndex scale, int subbandIndex)
        {
            int scaleValueByuV = AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV[(int)scale];
            String scaleTxt = "";
            if (scaleValueByuV < 1000)
                scaleTxt = $"{scaleValueByuV}uV";
            else if (scaleValueByuV < 1000_000)
                scaleTxt = $"{scaleValueByuV / 1000}mV";
            else
                scaleTxt = $"{scaleValueByuV / 1000_000}V";
            return $"{channelId}_Subband{subbandIndex + 1}_{scaleTxt}";
        }

        internal static void SendDiscard()
        {
            if (Hd.UIMessage?.Analog == null)
            {
                return;
            }

            foreach (ChannelId chnlid in ChannelIdExt.GetAnalogs())
            {
                if ((Int32)chnlid >= Hd.UIMessage.Analog.Length)
                    continue;
                var adcusedinfos = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(Acquirer_AnalogChanel_DBI13G.DbiActuallActiveState, chnlid)?.ToArray();
                if (adcusedinfos == null)
                    continue;
                Int32 yscaleId = (Int32)AnaChnlScaleIndex.Lv1;
                for (Int32 subid = 0; subid < adcusedinfos.Length; subid++)
                {
                    Int32 discardbefore = DbiAnalogParams.Default[0, (Int32)chnlid, yscaleId, subid].DiscardDotsBefore;
                    Int32 discardafter = DbiAnalogParams.Default[0, (Int32)chnlid, yscaleId, subid].DiscardDotsAfter;

                    switch (adcusedinfos[subid].AcqBdNo)//Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DBI_init_delay_en, adcusedinfos[subid].AcqBdNo, discardbefore == 0 ? 0 : 1u);
                    {
                        case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh1, discardbefore == 0 ? 0 : 1u); break;
                        case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh2, discardbefore == 0 ? 0 : 1u); break;
                        case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh3, discardbefore == 0 ? 0 : 1u); break;
                        case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh4, discardbefore == 0 ? 0 : 1u); break;
                    }
                    switch (adcusedinfos[subid].AcqBdNo)//Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DBI_auto_init_delay_num, adcusedinfos[subid].AcqBdNo, (UInt32)discardbefore - 3);
                    {
                        case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh1, (UInt32)discardbefore - 3); break;
                        case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh2, (UInt32)discardbefore - 3); break;
                        case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh3, (UInt32)discardbefore - 3); break;
                        case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_auto_init_delay_numProCh4, (UInt32)discardbefore - 3); break;
                    }

                    //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DBI_IntDelayNum, adcusedinfos[subid].AcqBdNo, (UInt32)discardafter);
                    switch (adcusedinfos[subid].AcqBdNo)//Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DBI_IntDelayNum, adcusedinfos[subid].AcqBdNo, (UInt32)discardafter);
                    {
                        case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh1, (UInt32)discardafter); break;
                        case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh2, (UInt32)discardafter); break;
                        case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh3, (UInt32)discardafter); break;
                        case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh4, (UInt32)discardafter); break;
                    }
                }
            }
        }
      
        /// <summary>
        /// 给所有通道发送相同的内容
        /// </summary>
        /// <param name="cmdStr"></param>
        internal static void DBI_SendASCII(String cmdStr)
        {
            foreach (ChannelId chnlid in _RigisterTable.Keys)
            {
                HdIO.WriteReg(_RigisterTable[chnlid].Enable, 0);
                for (Int32 k = 0; k < cmdStr.Length; k++)
                {
                    HdIO.WriteReg(_RigisterTable[chnlid].WEnable, 0);
                    HdIO.WriteReg(_RigisterTable[chnlid].Data, (UInt32)cmdStr[k]);
                    HdIO.WriteReg(_RigisterTable[chnlid].WEnable, 1);

                }
                HdIO.WriteReg(_RigisterTable[chnlid].WEnable, 0);
                HdIO.WriteReg(_RigisterTable[chnlid].Enable, 1);
            }

            Thread.Sleep(10);

            foreach (ChannelId chnlid in _RigisterTable.Keys)
            {
                HdIO.WriteReg(_RigisterTable[chnlid].Enable, 0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chnlId">通道序号，取值0-3</param>
        /// <param name="cmdStr">完整待发送的控制字符串</param>
        public static void DBI_SendASCII(UInt32 chnlId, String cmdStr)//????
        {
            (PcieBdReg.W Enable, PcieBdReg.W WEnable, PcieBdReg.W Data)[] registerGroup =
            {
                (PcieBdReg.W.DBI_LO_DBI_bin_en_ch1,PcieBdReg.W.DBI_LO_DBI_bin_wen_ch1,PcieBdReg.W.DBI_LO_DBI_bin_data_ch1),
                (PcieBdReg.W.DBI_LO_DBI_bin_en_ch2,PcieBdReg.W.DBI_LO_DBI_bin_wen_ch2,PcieBdReg.W.DBI_LO_DBI_bin_data_ch2),
                (PcieBdReg.W.DBI_LO_DBI_bin_en_ch3,PcieBdReg.W.DBI_LO_DBI_bin_wen_ch3,PcieBdReg.W.DBI_LO_DBI_bin_data_ch3),
                (PcieBdReg.W.DBI_LO_DBI_bin_en_ch4,PcieBdReg.W.DBI_LO_DBI_bin_wen_ch4,PcieBdReg.W.DBI_LO_DBI_bin_data_ch4),
            };

            if (chnlId >= registerGroup.Length)
                return;

            HdIO.WriteReg(registerGroup[chnlId].Enable, 0);
            for (Int32 k = 0; k < cmdStr.Length; k++)
            {
                HdIO.WriteReg(registerGroup[chnlId].WEnable, 0);
                HdIO.WriteReg(registerGroup[chnlId].Data, (UInt32)cmdStr[k]);
                HdIO.WriteReg(registerGroup[chnlId].WEnable, 1);

            }
            HdIO.WriteReg(registerGroup[chnlId].WEnable, 0);
            HdIO.WriteReg(registerGroup[chnlId].Enable, 1);
            Thread.Sleep(10);
            HdIO.WriteReg(registerGroup[chnlId].Enable, 0);
        }
        private static Dictionary<UInt32, Dictionary<ChannelId, UInt32[]>> _BuffDefine = new()
        {
            [0] = new()
            {
                [ChannelId.C1] = new UInt32[] { 0b0100_0000, 0b0010_0000, 0b0001_0000 },
                [ChannelId.C2] = new UInt32[] { 0b0000_0000, 0b0000_0000, 0b0000_0000 },
            },
            [2] = new()
            {
                [ChannelId.C3] = new UInt32[] { 0b0100_0000, 0b0010_0000, 0b0001_0000 },
                [ChannelId.C4] = new UInt32[] { 0b0000_0000, 0b0000_0000, 0b0000_0000 },
            },
        };
        internal static void SendBufferCtrlWords(UInt32 chnlActiveState)
        {
            Dictionary<UInt32, UInt32> ans = new();

            if (Hd.CurrProduct?.AnalogAcquireModel?.GetDbiMergeState(chnlActiveState) ?? false)
            {
                foreach (UInt32 sendboard in _BuffDefine.Keys)
                {
                    UInt32 tmp = 0;
                    foreach (ChannelId chnlid in _BuffDefine[sendboard].Keys)
                    {
                        if (((1u << (Int32)chnlid) & chnlActiveState) == 0)
                            continue;
                        for (Int32 i = 0; i < _BuffDefine[sendboard][chnlid].Length; i++)
                        {
                            tmp |= _BuffDefine[sendboard][chnlid][i];
                        }
                    }
                    ans[sendboard] = tmp;
                }
            }
            else
            {
                foreach (UInt32 sendboard in _BuffDefine.Keys)
                {
                    UInt32 tmp = 0;
                    foreach (ChannelId chnlid in _BuffDefine[sendboard].Keys)
                    {
                        if (_BuffDefine[sendboard][chnlid].Length > 0)
                        {
                            tmp |= _BuffDefine[sendboard][chnlid][0];
                        }
                    }
                    ans[sendboard] = tmp;
                }
            }

            foreach (UInt32 boardid in ans.Keys)
            {
                var tmp = GetCtrlWords(Buffer, (Int32)ans[boardid]);
                DBI_SendASCII(boardid, tmp);
            }
        }
        /// <summary>
        /// 构造通道命令控制字
        /// </summary>
        /// <param name="cmdType">只支持'2'-'D'的命令构造</param> C命令暂未使用 有待补充
        /// <param name="configValue"></param>
        /// <returns></returns>
        private static String GetCtrlWords(Char cmdType, Int32 configValue)
        {
            if (cmdType < BaseFreqSet2 || cmdType > Buffer)
                return String.Empty;

            if (cmdType == DacA)
                return $"Com[{cmdType}]=030{configValue.ToString("00000")}@";

            if (cmdType == DacC)
                return $"Com[{cmdType}]=032{configValue.ToString("00000")}@";

            if (cmdType == DacF)
                return $"Com[{cmdType}]=035{configValue.ToString("00000")}@";

            if (cmdType == Buffer)
                return $"Com[{cmdType}]={configValue.ToString("X2")}000000@";

            return $"Com[{cmdType}]={configValue.ToString("000")}@";
        }
        #region 通道命令常量定义
        private const Char PreChannelSetting = '1';

        private const Char BaseFreqSet2 = '2';
        private const Char BaseFreqSet3 = '3';
        private const Char BaseFreqSet4 = '4';

        private const Char GainSet1 = '5';
        private const Char GainSet2 = '6';
        private const Char GainSet3 = '7';
        private const Char GainSet4 = '8';

        /// <summary>
        /// Bias
        /// </summary>
        private const Char DacA = '9';

        /// <summary>
        /// Offset
        /// </summary>
        private const Char DacC = 'A';

        /// <summary>
        /// Ref
        /// </summary>
        private const Char DacF = 'B';

        private const Char Buffer = 'C';

        /// <summary>
        /// CaliFreq校准源频率配置
        /// </summary>
        private const Char CaliFreq = '5';
        #endregion

        /// <summary>
        /// 配置内部源
        /// </summary>
        /// <param name="enable">内部源使能</param>
        /// <param name="freqByMHz">内部源输出的信号频率</param>
        /// <param name="srcType">输出的信号类型</param>
        /// <param name="channel">输出到的通道</param>
        public static void ConfigInnerSignalSource(Boolean enable,UInt32 freqByMHz, CtrlAnalogChannel_DBI20G.SrcTypeEnum srcType, ChannelId channel)
        {
            //Dictionary<ChannelId, UInt32> mode = new()
            //{
            //    [ChannelId.C1] = 0,
            //    [ChannelId.C2] = 1,
            //    [ChannelId.C3] = 0,
            //    [ChannelId.C4] = 1,
            //};
            //if (mode.ContainsKey(chnlId))
            //{
            //    CtrlCaliModeFreq(mode[chnlId], freqByMHz / 10);
            //}
            string cmd = CtrlAnalogChannel_DBI20G.GetCaliSrcFreq(enable, freqByMHz / 10, srcType);
            SendCmd(channel, cmd);
        }
        ///校准源频率以及输出模式控制字获取函数 <summary>
        ///校准源需要控制频率和输出模式，频率控制字4位，输出模式暂定1位，具体发送给哪个通道都可以，通道的单片机会把控制字转发给校准源的单片机
        /// </summary>
        /// <param name="outputMode">0 or 1</param>
        /// <param name="freqBy10MHz">10Mhz为单位</param>
        /// <returns></returns>
        private static void CtrlCaliModeFreq(UInt32 outputMode, UInt32 freqBy10MHz)
        {
            String cmdStr = $"Com[{CaliFreq}]={freqBy10MHz.ToString("0000")}{outputMode}@";
            for (Int32 i = 0; i < 10; i++)
            {
                DBI_SendASCII(cmdStr);
                Thread.Sleep(10);
            }
        }


        #region 2.校准源配置

        internal enum DbiSunbbandCaliSourceEnum
        {
            SingleTone = 0,
            FastEdge = 1,
        }

        internal static Dictionary<DbiSunbbandCaliSourceEnum, String> _Subband1CaliSourceDefine = new()
        {
            [DbiSunbbandCaliSourceEnum.SingleTone] = "0",
            [DbiSunbbandCaliSourceEnum.FastEdge] = "2",
        };

        private const Int32 _Subband1CaliSourceMinBy10MHz = 1;
        private const Int32 _Subband1CaliSourceMaxBy10MHz = 2000;

        #endregion



    }


}

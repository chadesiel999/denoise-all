using NPOI.POIFS.Crypt.Dsig;
using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using static NPOI.HSSF.Util.HSSFColor;
using static ScopeX.Core.Decode.DecoderTypes;
using static ScopeX.Core.Decode.EdgeInfoCPP;

namespace ScopeX.Core.Decode
{
    internal sealed partial class MILDecodeModelCPP : ProtocolModel
    {
        private List<MILEventCPP> _PacketInfos = new List<MILEventCPP>();
        private DecodeResultData _ResultData = new DecodeResultData();
        internal const UInt32 RTADDRESSSBITCOUNT = 5;
        private static UInt32 _RTAMask = (UInt32)Math.Pow(2, RTADDRESSSBITCOUNT) - 1;
        internal const UInt32 SUBADDRESSBITCOUNT = 5;
        private static UInt32 _SAMask = (UInt32)Math.Pow(2, SUBADDRESSBITCOUNT) - 1;
        internal const UInt32 MODELCODEBITCOUNT = 5;
        private static UInt32 _MCMask = (UInt32)Math.Pow(2, MODELCODEBITCOUNT) - 1;
        internal const UInt32 DATABITCOUNT = 16;
        internal const UInt32 RESERVEBITCOUNT = 3;
        private static UInt32 _RMask = (UInt32)Math.Pow(2, RESERVEBITCOUNT) - 1;
        private const Byte _TRUE = 1;
        private const Int32 _HASDATATREUE = 1;
        private List<PAM3EdgePulse> _EdgePulsesList = new List<PAM3EdgePulse>();
        public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>()
        {
            "Index",
            "Start Time",
            "Type",
            "Remote Terminal Address",
            "Transmit/Receive",
            "Sub Address/Mode",
            "Mode Code",
            "Status",
            "Data",
            "Parity",
            "Error",
        };
        public override Double BitRateByPs => (1f / RealSignalRate) * 1E12;

        private ChannelId _Source;
        public ChannelId Source
        {
            get { return _Source; }
            set { UpdateProperty(ref _Source, value); }
        }

        public String Unit => GetChannelUnit(Source);

        public Double _LowThreshold = -1;
        public Double LowThreshold
        {
            get => _LowThreshold * TryGetChannelGain(Source);
            set => UpdateProperty(ref _LowThreshold, value / TryGetChannelGain(Source));
        }

        private Double _HighThreshold = 1;
        public Double HighThreshold
        {
            get { return _HighThreshold * TryGetChannelGain(Source); }
            set { UpdateProperty(ref _HighThreshold, value / TryGetChannelGain(Source)); }
        }
        public Double MaxThreshold => 30 * TryGetChannelGain(Source);
        public Double MinThreshold => -30 * TryGetChannelGain(Source);

        private ProtocolMIL.SignalRate _SignalRate = ProtocolMIL.SignalRate.Custom;
        public ProtocolMIL.SignalRate SignalRate
        {
            get { return _SignalRate; }
            set
            {
                if (value != _SignalRate)
                {
                    _SignalRate = value;
                    switch (value)
                    {
                        case ProtocolMIL.SignalRate.SignalRate_10M:
                            RealSignalRate = 10_000_000;
                            break;
                        case ProtocolMIL.SignalRate.SignalRate_1M:
                            RealSignalRate = 1_000_000;
                            break;

                    }

                }
            }
        }

        private Int32 _RealSignalRate = 1000_000;
        public Int32 RealSignalRate
        {
            get { return _RealSignalRate; }
            set
            {
                if (value != _RealSignalRate)
                {
                    switch (value)
                    {
                        case 1000_000:
                            SignalRate = ProtocolMIL.SignalRate.SignalRate_1M;
                            break;
                        case 10_000_000:
                            SignalRate = ProtocolMIL.SignalRate.SignalRate_10M;
                            break;
                        default:
                            SignalRate = ProtocolMIL.SignalRate.Custom;
                            break;
                    }
                    UpdateProperty(ref _RealSignalRate, value);
                }
            }
        }

        public Int32 MinSignalRate => 1000;
        public Int32 MaxSignalRate => 20_000_000;
        private ProtocolCommon.Polarity _Polarity = ProtocolCommon.Polarity.Positive;
        public ProtocolCommon.Polarity Polarity
        {
            get { return _Polarity; }
            set { UpdateProperty(ref _Polarity, value); }
        }

        public MILDecodeModelCPP(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.MIL, isTrigDecode)
        {
            _ResultData.Name = "MIL-STD-1553B";
            //if (!isTrigDecode)
            //{
            //    _Decoders.Add(new MILCommandDecoder());
            //    _Decoders.Add(new MILStatusDecoder());
            //    _Decoders.Add(new MILDataDecoder());
            //}
        }

        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolMILOptions()
            {
                Polarity = Polarity,
                SignalRate = RealSignalRate,
                Source = Source,
                HighThreshold = _HighThreshold,
                LowThreshold = _LowThreshold,
            };
        }

        internal override Boolean SourceHasData()
        {
            if (DsoPrsnt.DefaultDsoPrsnt == null)
                return false;

            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Source, out var prsnt);
            if (prsnt == null)
                return false;

            if (Source.IsReference() && prsnt.VuDatabase != null && prsnt.VuDatabase.Current != null)
            {
                return DecodeDataHelper.ReferenceHasData(Source, HighThreshold, LowThreshold);
            }

            if (Source.IsAnalog())
            {
                return DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }

            return false;
        }
        internal override Boolean CheckUpdate(ref Int64 laststamp)
        {
            //if (Source.IsAnalog())
            //{
            //    return laststamp != DecodeDataHelper.Instance.AnalogDataSource.TimeStamp;
            //}
            //if (Source.IsReference())
            //{
            //    return laststamp != DecodeDataHelper.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].TimeStamp;
            //}

            if (Source.IsAnalog() && laststamp != DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp;
                return true;
            }
            if (Source.IsReference() && laststamp != DecodeDataHelper.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].TimeStamp;
                return true;
            }

            return false;
        }

        void GetWaveformDatas(out Double[] datas)
        {
            datas = new Double[1];
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Source, out var prsnt);
            if (prsnt == null)
                return;

            if (Source.IsReference() && prsnt.VuDatabase != null && prsnt.VuDatabase.Current != null)
            {
                if (prsnt == null || prsnt.VuDatabase == null || prsnt.VuDatabase.Current == null || prsnt is not ReferencePrsnt)
                {
                    return;
                }
                ReferencePrsnt refprsnt = prsnt as ReferencePrsnt;
                datas = new Double[(UInt32)refprsnt!.Pack!.Buffer.GetLength(1)];
                Buffer.BlockCopy(refprsnt!.Pack!.Buffer, 0, datas, 0, datas.Length * sizeof(Double));
            }
            else if (Source.IsAnalog() && prsnt.VuDatabase != null && prsnt.VuDatabase.Current != null)
            {
                //datas = new Double[prsnt.VuDatabase.Current.Buffer.Length];
                //Buffer.BlockCopy(prsnt.VuDatabase.Current.Buffer, 0, datas, 0, datas.Length * sizeof(Double));
                AnalogPrsnt refprsnt = prsnt as AnalogPrsnt;
                datas = new Double[(UInt32)refprsnt!.Pack!.Buffer.GetLength(1)];
                Buffer.BlockCopy(refprsnt!.Pack!.Buffer, 0, datas, 0, datas.Length * sizeof(Double));
            }
        }

        Boolean GetEdgePulseNodes(Double samplerate, out IntPtr edgepulseseqptr)
        {
            //边沿脉宽
            // 波形数据
            Double[] waveformdatas;
            GetWaveformDatas(out waveformdatas);
            WaveformInfoCPP waveforminfo = new(samplerate, PAMType.PAM3, (UInt64)waveformdatas.Length, (IntPtr)0);
            GCHandle datashandle;
            waveforminfo.Allocate(ref waveformdatas, out datashandle);

            // 阈值
            ThresholdInfoCPP thresholdinfo = new((Byte)2, (IntPtr)0);
            Double[] thresholds = new Double[2];
            thresholds[0] = HighThreshold * 1000;
            thresholds[1] = LowThreshold * 1000;
            GCHandle thresholshandle;
            thresholdinfo.Allocate(ref thresholds, out thresholshandle);

            // 整形
            DecoderImpl.AnalysisPAMData(waveforminfo, thresholdinfo, out edgepulseseqptr);

            waveforminfo.Free(ref datashandle);
            thresholdinfo.Free(ref thresholshandle);

            return true;
        }

        internal override void ParsingData(ref CancellationToken token)
        {
            Int32 chindex = GetChIndex(Source);
            UInt32 datalen = 0;
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, Source, ref datalen);
            Double samplerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, Source, ref samplerate);
            Boolean needclear = false;
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Source, out var cp);
            MILResult decoderesult = new MILResult();
            if (MoreThanStorage() || chindex == -1 || datalen == 0 || samplerate == 0 || (cp != null && !cp.Active))
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _PacketInfos.Clear();
            }
            try
            {
                if (_NeedDecodeData)
                {
                    Int32 bitdatacount = (Int32)(Math.Round(1d / RealSignalRate * samplerate, 0));
                    _PacketInfos.Clear();
                    _NeedDecodeData = false;
                    _NeedUpdateViewInfo = true;
                    if (bitdatacount >= 2)
                    {
                        //用户参数
                        MILOptions options = new()
                        {
                            SignalRateType = SignalRate,
                            SignalRate = (UInt32)RealSignalRate,
                            Polarity = Polarity,
                            CancelFlag = _CancelFlagPtr
                        };

                        // 软件整形及边沿信息获取
                        //GetEdgePulseNodes(samplerate, out IntPtr edgepulseptr);

                        // 边沿脉宽信息获取
                        ThreeLevelEdgeInfo? node = DecodeDataHelper.Instance.GetThreeLevelEdgeInfo(BusId, 0, Source, ref token, ref needclear) as ThreeLevelEdgeInfo;
                        if (node == null)
                        {
                            return;
                        }
                        IntPtr edgepulseptr = IntPtr.Zero;
                        GCHandle edgepulseshandle;
                        _EdgePulsesList.Clear();
                        DecodeDataHelper.Instance.GetThreeLevelPulses(ref node, ref _EdgePulsesList);
                        PAM3EdgePulseSequence.Allocate(ref _EdgePulsesList, (UInt64)datalen, samplerate, out edgepulseptr, out edgepulseshandle);

                        //开始解码                       
                        decoderesult.EventInfosPtr = IntPtr.Zero;
                        //var starttime = TimeSpanUtility.GetTimestampSpan();
                        if (!DecoderImpl.DecodeMIL1553(options, edgepulseptr, out decoderesult))
                        {
                        }
                        //Double diftime = (TimeSpanUtility.GetTimestampSpan() - starttime).TotalMilliseconds;
                        //if (diftime  > 2000)
                        //{
                        //    
                        //}
                        PAM3EdgePulseSequence.Free(ref edgepulseptr, ref edgepulseshandle);

                        //PAMType pamtype = PAMType.PAM3;
                        //DecoderImpl.FreeEdgePulses(pamtype, edgepulseptr);

                        _PacketInfos.Clear();
                        //解码结果获取转换
                        Int32 structsize = Marshal.SizeOf(typeof(MILEventCPP));
                        for (Int32 i = 0; i < decoderesult.EventCount; i++)
                        {
                            IntPtr presultptr = new IntPtr(decoderesult.EventInfosPtr.ToInt64() + i * structsize);
                            MILEventCPP presult = (MILEventCPP)Marshal.PtrToStructure(presultptr, typeof(MILEventCPP));
                            _PacketInfos.Add(presult);
                        }
                        //c++资源释放
                        DecoderImpl.FreeMIL1553(decoderesult);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            // 通知界面更新
            UpdateView(chindex);
        }

        internal void UpdateView(Int32 chindex)
        {
            try
            {
                if (_NeedUpdateViewInfo)
                {
                    _NeedUpdateViewInfo = false;
                    var decodedatas = GetDecodeBuffer();
                    decodedatas.Clear();
                    _EventInfos.Clear();
                    if (_PacketInfos.Count == 0)
                    {
                        _ResultData.DecodeViewInfos = new IDecodeViewInfo[0];
                        decodedatas.Add(_ResultData);
                        ChangeBuffer();
                        return;
                    }
                    _ResultData.DecodeViewInfos = _PacketInfos.SelectMany(x =>
                    {
                        ProtocolEventInfo @event = new ProtocolEventInfo()
                        {
                            Index = _EventInfos.Count
                        };
                        var endindex = 0;
                        @event.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                        List<MILDecodePacket> packets = new List<MILDecodePacket>();
                        if (x.SOF.HasData == _HASDATATREUE)
                        {
                            var sof = new MILSOFDecodePacket(CalcPosition((UInt32)x.SOF.StartIndex, Source, chindex));
                            packets.Add(sof);
                            @event.StartPosition = sof.Start;
                            endindex = (Int32)x.SOF.StartIndex;
                        }

                        if (x.Sync.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILSyncDecodePacket(CalcPosition((UInt32)x.Sync.StartIndex, Source, chindex), CalcBitLenght((Int32)x.Sync.Length, Source, chindex))
                            {
                                PacketType = x.PacketType,
                                Success = (x.Sync.ErrorType == (Byte)DataErrorType.NoError),
                            });
                            @event.EventInofs[0] = (packets[^1].Data, packets[^1].BitCount);
                            if (x.Sync.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes(("sync time error")), 0);
                            }
                            endindex = (Int32)x.Sync.StartIndex + (Int32)x.Sync.Length;
                        }

                        //command
                        if (x.RTAInfo.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILRTADecodePacket(CalcPosition((UInt32)x.RTAInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.RTAInfo.Length, Source, chindex))
                            {
                                Data = new Byte[] { x.RTA },
                                Success = (x.RTAInfo.ErrorType == (Byte)DataErrorType.NoError),
                            });
                            @event.EventInofs[1] = (packets[^1].Data, packets[^1].BitCount);
                            if (x.RTAInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" the RTA value error")), 0);
                            }
                            endindex = (Int32)x.RTAInfo.StartIndex + (Int32)x.RTAInfo.Length;
                        }
                        if (x.TRInfo.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILTRDecodePacket(CalcPosition((UInt32)x.TRInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.TRInfo.Length, Source, chindex))
                            {
                                TR = x.TR == _HASDATATREUE,
                                Success = (x.TRInfo.ErrorType == (Byte)DataErrorType.NoError),
                            });
                            @event.EventInofs[2] = (Encoding.Default.GetBytes(x.TR == _TRUE ? "T" : "R"), 0);
                            if (x.TRInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" the T/R value error")), 0);
                            }
                            endindex = (Int32)x.TRInfo.StartIndex + (Int32)x.TRInfo.Length;
                        }
                        if (x.SubAddresInfo.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILSADecodePacket(CalcPosition((UInt32)x.SubAddresInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.SubAddresInfo.Length, Source, chindex))
                            {
                                Data = new Byte[] { x.SubAddress },
                                Success = (x.SubAddresInfo.ErrorType == (Byte)DataErrorType.NoError),
                            });
                            @event.EventInofs[3] = (packets[^1].Data, packets[^1].BitCount);
                            if (x.SubAddresInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" the sub address value error")), 0);
                            }
                            endindex = (Int32)x.SubAddresInfo.StartIndex + (Int32)x.SubAddresInfo.Length;
                        }
                        if (x.ModeCodeInfo.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILModeCodeDecodePacket(CalcPosition((UInt32)x.ModeCodeInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.ModeCodeInfo.Length, Source, chindex))
                            {
                                Data = new Byte[] { x.ModelCode },
                                Success = (x.ModeCodeInfo.ErrorType == (Byte)DataErrorType.NoError),
                            });
                            @event.EventInofs[4] = (packets[^1].Data, packets[^1].BitCount);
                            if (x.ModeCodeInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" the mode codes value error")), 0);
                            }
                            endindex = (Int32)x.ModeCodeInfo.StartIndex + (Int32)x.ModeCodeInfo.Length;
                        }

                        //data word
                        if (x.DataInfo.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILDataDecodePacket(CalcPosition((UInt32)x.DataInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.DataInfo.Length, Source, chindex))
                            {
                                Data = BitConverter.GetBytes(x.Data).Reverse().ToArray(),
                                Success = (x.DataInfo.ErrorType == (Byte)DataErrorType.NoError),
                            });
                            @event.EventInofs[6] = (packets[^1].Data, packets[^1].BitCount);
                            if (x.DataInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" the data word value error")), 0);
                            }
                            endindex = (Int32)x.DataInfo.StartIndex + (Int32)x.DataInfo.Length;
                        }

                        //status
                        if (x.MessageErroInfo.HasData == _HASDATATREUE)
                        {
                            MILStatusDecodePacket packet = new MILStatusDecodePacket(CalcPosition((UInt32)x.MessageErroInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.MessageErroInfo.Length, Source, chindex), MILDecodePacketType.MessageError)
                            {
                                Status = x.MessageError == _TRUE,
                                Success = (x.MessageErroInfo.ErrorType == (Byte)DataErrorType.NoError),
                            };
                            packets.Add(packet);
                            // @event.EventInofs[8] = (Encoding.Default.GetBytes("True"), 0);
                            if (x.MessageErroInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" the message formats value error")), 0);
                            }
                            endindex = (Int32)x.MessageErroInfo.StartIndex + (Int32)x.MessageErroInfo.Length;
                        }
                        if (x.InstrumentationInfo.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILStatusDecodePacket(CalcPosition((UInt32)x.InstrumentationInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.InstrumentationInfo.Length, Source, chindex), MILDecodePacketType.Instrumentation)
                            {
                                Status = x.Instrumentation == _TRUE,
                                Success = (x.InstrumentationInfo.ErrorType == (Byte)DataErrorType.NoError),
                            });
                            if (x.InstrumentationInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" the instrument value error")), 0);
                            }
                            endindex = (Int32)x.InstrumentationInfo.StartIndex + (Int32)x.InstrumentationInfo.Length;
                        }
                        if (x.ServiceRequestInfo.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILStatusDecodePacket(CalcPosition((UInt32)x.ServiceRequestInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.ServiceRequestInfo.Length, Source, chindex), MILDecodePacketType.ServiceResquest)
                            {
                                Status = x.ServiceResquest == _TRUE,
                                Success = (x.ServiceRequestInfo.ErrorType == (Byte)DataErrorType.NoError),
                            });
                            if (x.ServiceRequestInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" the service request value error")), 0);
                            }
                            endindex = (Int32)x.ServiceRequestInfo.StartIndex + (Int32)x.ServiceRequestInfo.Length;
                        }
                        if (x.ReservedInfo.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILReservedDecodePacket(CalcPosition((UInt32)x.ReservedInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.ReservedInfo.Length, Source, chindex))
                            {
                                Data = new Byte[] { x.Reserved },
                                Success = (x.ReservedInfo.ErrorType == (Byte)DataErrorType.NoError),
                            });
                            if (x.ReservedInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" the reserved status value error")), 0);
                            }
                            endindex = (Int32)x.ReservedInfo.StartIndex + (Int32)x.ReservedInfo.Length;
                        }
                        if (x.BroadcastCommandReceivedInfo.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILStatusDecodePacket(CalcPosition((UInt32)x.BroadcastCommandReceivedInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.BroadcastCommandReceivedInfo.Length, Source, chindex), MILDecodePacketType.BroadcastCommandReceived)
                            {
                                Status = x.BroadcastCommandReceived == _TRUE,
                                Success = (x.BroadcastCommandReceivedInfo.ErrorType == (Byte)DataErrorType.NoError),
                            });
                            if (x.BroadcastCommandReceivedInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" the broadcast command received value error")), 0);
                            }
                            endindex = (Int32)x.BroadcastCommandReceivedInfo.StartIndex + (Int32)x.BroadcastCommandReceivedInfo.Length;
                        }
                        if (x.BusyInfo.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILStatusDecodePacket(CalcPosition((UInt32)x.BusyInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.BusyInfo.Length, Source, chindex), MILDecodePacketType.Busy)
                            {
                                Status = x.Busy == _TRUE,
                                Success = (x.BusyInfo.ErrorType == (Byte)DataErrorType.NoError),
                            });
                            if (x.BusyInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" the busy value error")), 0);
                            }
                            endindex = (Int32)x.BusyInfo.StartIndex + (Int32)x.BusyInfo.Length;
                        }
                        if (x.SubsystemFlagInfo.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILStatusDecodePacket(CalcPosition((UInt32)x.SubsystemFlagInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.SubsystemFlagInfo.Length, Source, chindex), MILDecodePacketType.SubsystemFlag)
                            {
                                Status = x.SubSystemFlag == _TRUE,
                                Success = (x.SubsystemFlagInfo.ErrorType == (Byte)DataErrorType.NoError),
                            });
                            if (x.SubsystemFlagInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" the subsystem flag value error")), 0);
                            }
                            endindex = (Int32)x.SubsystemFlagInfo.StartIndex + (Int32)x.SubsystemFlagInfo.Length;
                        }
                        if (x.DynamicBusControlAcceptanceInfo.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILStatusDecodePacket(CalcPosition((UInt32)x.DynamicBusControlAcceptanceInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.DynamicBusControlAcceptanceInfo.Length, Source, chindex), MILDecodePacketType.DynamicBusControlAcceptance)
                            {
                                Status = x.DynamicBusControlAcceptance == _TRUE,
                                Success = (x.DynamicBusControlAcceptanceInfo.ErrorType == (Byte)DataErrorType.NoError),
                            });
                            if (x.DynamicBusControlAcceptanceInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" the dynamic bus control value error")), 0);
                            }
                            endindex = (Int32)x.DynamicBusControlAcceptanceInfo.StartIndex + (Int32)x.DynamicBusControlAcceptanceInfo.Length;
                        }
                        if (x.TerminalFlagInfo.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILStatusDecodePacket(CalcPosition((UInt32)x.TerminalFlagInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.TerminalFlagInfo.Length, Source, chindex), MILDecodePacketType.TerminalFlag)
                            {
                                Status = x.TerminalFlag == _TRUE,
                                Success = (x.TerminalFlagInfo.ErrorType == (Byte)DataErrorType.NoError),
                            });
                            if (x.TerminalFlagInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" the terminal flag value error")), 0);
                            }
                            endindex = (Int32)x.TerminalFlagInfo.StartIndex + (Int32)x.TerminalFlagInfo.Length;
                        }

                        if (x.ParityInfo.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILParityDecodePacketCPP(CalcPosition((UInt32)x.ParityInfo.StartIndex, Source, chindex), CalcBitLenght((Int32)x.ParityInfo.Length, Source, chindex))
                            {
                                Parity = x.Parity == _TRUE,
                                Success = x.ParityInfo.ErrorType == (Byte)DataErrorType.NoError,
                            });
                            @event.EventInofs[^2] = (packets[^1].Data, packets[^1].BitCount);
                            if (x.ParityInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                @event.EventInofs[8] = (Encoding.Default.GetBytes((" Parity error")), 0);
                            }
                            endindex = (Int32)x.ParityInfo.StartIndex + (Int32)x.ParityInfo.Length;
                        }
                        if (x.EOF.HasData == _HASDATATREUE)
                        {
                            packets.Add(new MILEOFDecodePacket(CalcPosition((UInt32)x.EOF.StartIndex, Source, chindex)));
                            endindex = (Int32)x.EOF.StartIndex ;
                        }

                        if (x.PacketType == MILPacketType.Status)
                        {
                            List<Int32> vals = new List<Int32>();
                            for (Int32 index = 0; index < 16 - RTADDRESSSBITCOUNT; index++)
                            {
                                vals.Add((x.TempData >> (Int32)(RTADDRESSSBITCOUNT + index)) & 0x01);
                            }
                            @event.EventInofs[5] = (Encoding.Default.GetBytes($"{vals[0]}{vals[1]}{vals[2]}-{vals[3]}{vals[4]}{vals[5]}-{vals[6]}{vals[7]}{vals[8]}{vals[9]}{vals[10]}"), 0);
                        }
                        if (packets.Count > 0)
                        {
                            @event.StartTimeByPs = GetTimeFromPosition(packets.Min(y => y.Start));
                            @event.StartPosition = packets.Min(y => y.Start);
                        }
                        @event.EndPosition = CalcPosition(endindex, Source, chindex);
                        @event.EndTimeByPs = GetTimeFromPosition(@event.EndPosition, chindex);
                        _EventInfos.Add(@event);
                        return packets.OrderBy(x => x.Start);
                    }).ToArray();
                    decodedatas.Add(_ResultData);

                    ChangeBuffer();
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NPOI.HSSF.Record.CF;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    [Obsolete("Please Use Class 'MILDecodeModelCPP'", true)]
    internal sealed class MILDecodeModel :ProtocolModel
    {
        private List<MILBaseDecoder> _Decoders = new List<MILBaseDecoder>();
        private List<MILPacketInfo> _PacketInfos = new List<MILPacketInfo>();
        private const Double _MinimumNoResponseTime = 14E-6;
        private const Double _Tolerance = 0.2;
        private DecodeResultData _ResultData = new DecodeResultData();
        internal const UInt32 RTAddressBitCount = 5;
        private static UInt32 _RTAMask = (UInt32)Math.Pow(2, RTAddressBitCount) - 1;
        internal const UInt32 SubAddressBitCount = 5;
        private static UInt32 _SAMask = (UInt32)Math.Pow(2, SubAddressBitCount) - 1;
        internal const UInt32 ModelCodeBitCount = 5;
        private static UInt32 _MCMask = (UInt32)Math.Pow(2, ModelCodeBitCount) - 1;
        internal const UInt32 DataBitCount = 16;
        internal const UInt32 ReservedBitCount = 3;
        private static UInt32 _RMask = (UInt32)Math.Pow(2, ReservedBitCount) - 1;
        public MILDecodeModel(ChannelId id,Boolean isTrigDecode = false) : base(id,SerialProtocolType.MIL, isTrigDecode)
        {
            _ResultData.Name = "MIL-STD-1553B";
            if (!isTrigDecode)
            {
                _Decoders.Add(new MILCommandDecoder());
                _Decoders.Add(new MILStatusDecoder());
                _Decoders.Add(new MILDataDecoder());
            }
        }

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
            get => _LowThreshold*TryGetChannelGain(Source);
            set => UpdateProperty(ref _LowThreshold, value/ TryGetChannelGain(Source));
        }


        private Double _HighThreshold = 1;

        public Double HighThreshold
        {
            get { return _HighThreshold* TryGetChannelGain(Source); }
            set { UpdateProperty(ref _HighThreshold, value/TryGetChannelGain(Source)); }
        }
        public Double MaxThreshold => 30* TryGetChannelGain(Source);
        public Double MinThreshold => -30* TryGetChannelGain(Source);
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
        public void CalcParity(ref MILPacketInfo packetInfo)
        {
            UInt32 count = 0;
            if (System.Runtime.Intrinsics.X86.Popcnt.IsSupported)
            {
                count = System.Runtime.Intrinsics.X86.Popcnt.PopCount(packetInfo.TempData);
            }
            else
            {
                Int32 bitindex = 0;
                while (bitindex < 16)
                {
                    if (((packetInfo.TempData >> bitindex) & 0x01) == 0x01) count++;
                    bitindex++;
                }
            }
            packetInfo.SuccessParity = count % 2 == 0;
        }
        private class MILEdgeInfo
        {
            [AllowNull]
            public MILEdgeInfo Parent { get; private set; }
            [AllowNull]
            public MILEdgeInfo Child { get; private set; }
            public Edge Edge { get; private set; } = Edge.None;
            public Boolean IsRoot => Parent == null;
            public Boolean IsLast => Child == null;
            public Int32 StartIndex { get; private set; }
            public Int32 EndIndex { get; private set; }
            public Int32 Length => EndIndex - StartIndex + 1;
            public Boolean Status { get; private set; }
            public void AddChild(Int32 startindex, Int32 endindex, Boolean status)
            {
                MILEdgeInfo info = new MILEdgeInfo();
                info.StartIndex = startindex;
                info.EndIndex = endindex;
                info.Status = status;
                info.Edge = status ? Edge.Rise : Edge.Falling;
                this.Child = info;
                info.Parent = this;
            }
        }
        private MILEdgeInfo GetEdgeinfos(ref CancellationToken token, ref Boolean needclear)
        {
            MILEdgeInfo edgeinfo = new MILEdgeInfo();
            var tempnode = edgeinfo;
            ThreeLevelEdgeInfo? node = DecodeDataHelper.Instance.GetThreeLevelEdgeInfo(BusId, 0, Source, ref token, ref needclear) as ThreeLevelEdgeInfo;
            if (node == null) return edgeinfo;
            while (node != null)
            {
                if (node.CurrentLevel == ThreeLevelEdgeInfo.Status.High || node.CurrentLevel == ThreeLevelEdgeInfo.Status.Low)
                {
                    tempnode.AddChild(node.StartIndex, node.EndIndex, node.CurrentLevel == ThreeLevelEdgeInfo.Status.High);
                    tempnode = tempnode.Child;

                }
                node = node.Child as ThreeLevelEdgeInfo;
            }
            return edgeinfo;
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

        internal override void ParsingData(ref CancellationToken token)
        {
            Int32 chindex = GetChIndex(Source);
            UInt32 datalen = 0;
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, Source, ref datalen);
            Double samplerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, Source, ref samplerate);
            Boolean needclear = false;
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Source, out var cp);
            if (chindex == -1 || datalen == 0 || samplerate == 0 || (cp != null && !cp.Active))
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _PacketInfos.Clear();
            }
            if (_NeedDecodeData)
            {
                _NeedDecodeData = false;
                _PacketInfos.Clear();
                Int32 bitdatacount = (Int32)(Math.Round(1d / RealSignalRate * samplerate, 0));
                Int32 minNoResponsecount = (Int32)(Math.Round(4 * 1E-6 * samplerate, 0));
                Int32 maxNoResponsecount = (Int32)(Math.Round(12 * 1E-6 * samplerate, 0));
                var node = GetEdgeinfos(ref token, ref needclear);
                if (node != null && bitdatacount > 0 && datalen > 0 && samplerate > 0)
                {
                    _NeedUpdateViewInfo = true;
                    while (true)
                    {
                        if (node == null) break;
                        node = FindStartNode(node, bitdatacount);
                        if (node == null) break;
                        MILPacketInfo packetInfo = new MILPacketInfo();
                        packetInfo.HasSOF = true;
                        packetInfo.SOFIndex = (Int32)(node.EndIndex - 1.5 * bitdatacount);

                        packetInfo.HasSync = true;
                        packetInfo.SyncIndex = packetInfo.SOFIndex;
                        packetInfo.SyncLength = bitdatacount * 3;
                        if (node.Child.Edge == Edge.Falling)
                        {
                            if (_PacketInfos.Count == 0
                                || _PacketInfos[^1].PacketType == MILPacketType.Status
                                || (packetInfo.SyncIndex - _PacketInfos[^1].ParityIndex - _PacketInfos[^1].ParityLength) >= maxNoResponsecount)
                            {
                                packetInfo.PacketType = MILPacketType.Command;
                            }
                            else
                            {
                                packetInfo.PacketType = MILPacketType.Status;
                            }
                        }
                        else
                        {
                            packetInfo.PacketType = MILPacketType.Data;
                        }
                        var decoder = _Decoders.FirstOrDefault(x => x.PacketType == packetInfo.PacketType);
                        node = node?.Child;
                        if (node == null || decoder == null)
                        {
                            _PacketInfos.Add(packetInfo);
                            break;
                        }
                        if ((node.Length - bitdatacount * 1.5) / bitdatacount <= _Tolerance)
                        {
                            node = node.Child;
                        }
                        if (node == null)
                        {
                            _PacketInfos.Add(packetInfo);
                            break;
                        }
                        decoder?.Decode(ref packetInfo, ref node, bitdatacount, datalen, ref packetInfo.ParityIndex);

                        if (node == null || node.Child == null)
                        {
                            _PacketInfos.Add(packetInfo);
                            break;
                        }

                        packetInfo.HasParity = true;
                        packetInfo.Parity = node.Child.Edge == Edge.Falling;
                        node = node.Child;
                        if (node == null)
                        {
                            packetInfo.ParityLength = (Int32)datalen - packetInfo.ParityIndex;
                            if (packetInfo.ParityLength > bitdatacount)
                            {
                                packetInfo.ParityLength = bitdatacount;
                            }
                            _PacketInfos.Add(packetInfo);
                            break;
                        }
                        if (Checker(node, bitdatacount)) node = node.Child;
                        if (node == null)
                        {
                            packetInfo.ParityLength = (Int32)datalen - packetInfo.ParityIndex;
                            if (packetInfo.ParityLength > bitdatacount)
                            {
                                packetInfo.ParityLength = bitdatacount;
                            }
                            _PacketInfos.Add(packetInfo);
                            break;
                        }
                        else
                        {
                            packetInfo.ParityLength = bitdatacount;
                            packetInfo.HasEOF = true;
                            packetInfo.EOFIndex = packetInfo.ParityIndex + packetInfo.ParityLength;
                        }
                        CalcParity(ref packetInfo);
                        _PacketInfos.Add(packetInfo);
                    }

                }
            }
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

                    @event.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                    List<MILDecodePacket> packets = new List<MILDecodePacket>();
                    if (x.HasSOF)
                    {
                        packets.Add(new MILSOFDecodePacket(CalcPosition(x.SOFIndex, Source, chindex)));
                    }
                    if (x.HasEOF)
                    {
                        packets.Add(new MILEOFDecodePacket(CalcPosition(x.EOFIndex, Source, chindex)));
                    }
                    if (x.HasBusy)
                    {
                        packets.Add(new MILStatusDecodePacket(CalcPosition(x.BusyIndex, Source, chindex), CalcBitLenght(x.BusyLength, Source, chindex), MILDecodePacketType.Busy)
                        {
                            Status = x.Busy,
                        });
                    }

                    if (x.HasBroadcastCommandReceived)
                    {
                        packets.Add(new MILStatusDecodePacket(CalcPosition(x.BroadcastCommandReceivedIndex, Source, chindex), CalcBitLenght(x.BroadcastCommandReceivedLength, Source, chindex), MILDecodePacketType.BroadcastCommandReceived)
                        {
                            Status = x.BroadcastCommandReceived,
                        });
                    }

                    if (x.HasDynamicBusControlAcceptance)
                    {
                        packets.Add(new MILStatusDecodePacket(CalcPosition(x.DynamicBusControlAcceptanceIndex, Source, chindex), CalcBitLenght(x.DynamicBusControlAcceptanceLength, Source, chindex), MILDecodePacketType.DynamicBusControlAcceptance)
                        {
                            Status = x.DynamicBusControlAcceptance,
                        });
                    }

                    if (x.HasInstrumentation)
                    {
                        packets.Add(new MILStatusDecodePacket(CalcPosition(x.InstrumentationIndex, Source, chindex), CalcBitLenght(x.InstrumentationLength, Source, chindex), MILDecodePacketType.Instrumentation)
                        {
                            Status = x.Instrumentation,
                        });
                    }

                    if (x.HasMessageError)
                    {
                        MILStatusDecodePacket packet = new MILStatusDecodePacket(CalcPosition(x.MessageErrorIndex, Source, chindex), CalcBitLenght(x.MessageErrorLength, Source, chindex), MILDecodePacketType.MessageError)
                        {
                            Status = x.MessageError,
                        };
                        packets.Add(packet);
                        @event.EventInofs[8] = (Encoding.Default.GetBytes("True"), 0);
                    }
                    else
                    {
                        @event.EventInofs[8] = (Encoding.Default.GetBytes("False"), 0);
                    }
                    if (x.HasServiceRequest)
                    {
                        packets.Add(new MILStatusDecodePacket(CalcPosition(x.ServiceRequestIndex, Source, chindex), CalcBitLenght(x.ServiceRequestLength, Source, chindex), MILDecodePacketType.ServiceResquest)
                        {
                            Status = x.ServiceResquest,
                        });
                    }
                    if (x.HasSubsystemFlag)
                    {
                        packets.Add(new MILStatusDecodePacket(CalcPosition(x.SubsystemFlagIndex, Source, chindex), CalcBitLenght(x.SubsystemFlagLength, Source, chindex), MILDecodePacketType.SubsystemFlag)
                        {
                            Status = x.SubsystemFlag,
                        });
                    }
                    if (x.HasTerminalFlag)
                    {
                        packets.Add(new MILStatusDecodePacket(CalcPosition(x.TerminalFlagIndex, Source, chindex), CalcBitLenght(x.TerminalFlagLength, Source, chindex), MILDecodePacketType.TerminalFlag)
                        {
                            Status = x.TerminalFlag,
                        });
                    }
                    if (x.HasData)
                    {
                        packets.Add(new MILDataDecodePacket(CalcPosition(x.DataIndex, Source, chindex), CalcBitLenght(x.DataLength, Source, chindex))
                        {
                            Data = x.Data,
                        });
                        @event.EventInofs[6] = (packets[^1].Data, packets[^1].BitCount);
                    }
                    if (x.HasModelCode)
                    {
                        packets.Add(new MILModeCodeDecodePacket(CalcPosition(x.ModelCodeIndex, Source, chindex), CalcBitLenght(x.ModelCodeLength, Source, chindex))
                        {
                            Data = new Byte[] { x.ModelCode },
                        });
                        @event.EventInofs[4] = (packets[^1].Data, packets[^1].BitCount);
                    }

                    if (x.HasParity)
                    {
                        packets.Add(new MILParityDecodePacket(CalcPosition(x.ParityIndex, Source, chindex), CalcBitLenght(x.ParityLength, Source, chindex))
                        {
                            Parity = x.Parity,
                            SuccessParity = x.SuccessParity,
                        });
                        @event.EventInofs[^2] = (packets[^1].Data, packets[^1].BitCount);
                        //if(x.Parity!=x.SuccessParity)
                        //{
                        //    @event.EventInofs[^1] = (Encoding.Default.GetBytes("Parity Error"), 0);
                        //}
                    }
                    if (x.HasReserved)
                    {
                        packets.Add(new MILReservedDecodePacket(CalcPosition(x.ReservedIndex, Source, chindex), CalcBitLenght(x.ReservedLength, Source, chindex))
                        {
                            Data = new Byte[] { x.Reserved },
                        });
                    }
                    if (x.HasRTA)
                    {
                        packets.Add(new MILRTADecodePacket(CalcPosition(x.RTAIndex, Source, chindex), CalcBitLenght(x.RTALength, Source, chindex))
                        {
                            Data = new Byte[] { x.RTA },
                        });
                        @event.EventInofs[1] = (packets[^1].Data, packets[^1].BitCount);
                    }
                    if (x.HasSubAddress)
                    {
                        packets.Add(new MILSADecodePacket(CalcPosition(x.SubAddressIndex, Source, chindex), CalcBitLenght(x.SubAddressLength, Source, chindex))
                        {
                            Data = new Byte[] { x.SubAddress },
                        });
                        @event.EventInofs[3] = (packets[^1].Data, packets[^1].BitCount);
                    }

                    if (x.HasSync)
                    {
                        packets.Add(new MILSyncDecodePacket(CalcPosition(x.SyncIndex, Source, chindex), CalcBitLenght(x.SyncLength, Source, chindex))
                        {
                            PacketType = x.PacketType,
                        });
                        @event.EventInofs[0] = (packets[^1].Data, packets[^1].BitCount);
                    }

                    if (x.HasTR)
                    {
                        packets.Add(new MILTRDecodePacket(CalcPosition(x.TRIndex, Source, chindex), CalcBitLenght(x.TRLength, Source, chindex))
                        {
                            TR = x.TR,
                        });
                        @event.EventInofs[2] = (Encoding.Default.GetBytes(x.TR ? "T" : "R"), 0);
                    }
                    if (x.PacketType == MILPacketType.Status)
                    {
                        List<Int32> vals = new List<Int32>();
                        for (Int32 index = 0; index < 16 - RTAddressBitCount; index++)
                        {
                            vals.Add((x.TempData >> (Int32)(RTAddressBitCount + index)) & 0x01);
                        }
                        @event.EventInofs[5] = (Encoding.Default.GetBytes($"{vals[0]}{vals[1]}{vals[2]}-{vals[3]}{vals[4]}{vals[5]}-{vals[6]}{vals[7]}{vals[8]}{vals[9]}{vals[10]}"), 0);
                    }
                    if (packets.Count > 0)
                    {
                        @event.StartTimeByPs = GetTimeFromPosition(packets.Min(y => y.Start));
                        @event.EndPosition = packets.Min(y => y.Start);
                        @event.EndTimeByPs = GetTimeFromPosition(@event.EndPosition, chindex);
                    }
                    _EventInfos.Add(@event);
                    return packets.OrderBy(x => x.Start);
                }).ToArray();
                decodedatas.Add(_ResultData);
                ChangeBuffer();
            }
        }
        private static Boolean Checker(MILEdgeInfo node, Int32 bitTimeLen)
        {
            return ((node.Length - 0.5 * bitTimeLen) / bitTimeLen) <= _Tolerance/* && (node.Parent.Length - 0.5*bitcount)/bitcount<=Tolerance*/;
        }
        private MILEdgeInfo? FindStartNode(MILEdgeInfo node, Int32 bitTimeLen)
        {
            var tempnode = node;
            var starttime = DateTime.Now;
            var min = 1.5 - _Tolerance;
            var max = 2 + _Tolerance;
            while (tempnode != null)
            {
                if (tempnode.Child == null)
                {
                    tempnode = null;
                    break;
                }
                if (((tempnode.Length) / (Double)(bitTimeLen) < max) && ((tempnode.Length) / (Double)(bitTimeLen) > min)
                    && (tempnode.Child.Length / (Double)bitTimeLen > min) && (tempnode.Child.Length / (Double)bitTimeLen < max))
                {
                    break;
                }
                tempnode = tempnode.Child;
                if ((DateTime.Now - starttime).TotalMilliseconds > 2000)
                {
                    tempnode = null;
                    return tempnode;
                }
            }
            return tempnode;
        }

        private static Int32 GetLevel(Int32 startindex, ref MILEdgeInfo? node, Int32 bitTimeLen)
        {
            while (true)
            {
                if (node == null || node?.Child == null)
                    return -1;
                if ((startindex + bitTimeLen / 2) > node.StartIndex && (startindex + bitTimeLen / 2) < node.Child.StartIndex)
                {
                    return node.Edge == Edge.Rise ? 1 : 0;
                }
                node = node.Child;
            }
        }


        #region Decoder
        private abstract class MILBaseDecoder
        {
            public abstract MILPacketType PacketType { get; }
            public abstract void Decode(ref MILPacketInfo packetInfo, ref MILEdgeInfo? node, Int32 bitTimeLen, UInt32 datalen, ref Int32 lastindex);
        }
        private sealed class MILCommandDecoder : MILBaseDecoder
        {
            public override MILPacketType PacketType => MILPacketType.Command;

            public override void Decode(ref MILPacketInfo packetInfo, ref MILEdgeInfo? node, Int32 bitTimeLen, UInt32 datalen, ref Int32 lastindex)
            {
                if (node == null) return;
                Boolean status = false;
                packetInfo.RTAIndex = packetInfo.SyncIndex + packetInfo.SyncLength;
                Boolean lastStatus = false;
                for (Int32 index = 0; index < RTAddressBitCount; index++)
                {
                    if (node?.Child == null) break;
                    packetInfo.RTABitCount++;
                    status = node.Child.Edge == Edge.Falling;
                    packetInfo.TempData |= (UInt16)(status ? (1 << (Int32)(DataBitCount - (index + 16 - RTAddressBitCount) - 1)) : 0);
                    node = node.Child;
                    if (node == null) break;
                    if (Checker(node, bitTimeLen)) node = node.Child;
                    else if (index == RTAddressBitCount - 1) lastStatus = true;
                }
                packetInfo.HasRTA = packetInfo.RTABitCount > 0;
                packetInfo.RTA = (Byte)(packetInfo.TempData & _RTAMask);
                if (node == null)
                {
                    packetInfo.RTALength = (Int32)datalen - packetInfo.RTAIndex;
                    return;
                }
                else
                {
                    packetInfo.RTALength = (Int32)(node.StartIndex - packetInfo.RTAIndex);
                }
                if (node?.Child == null) return;
                packetInfo.HasTR = true;
                packetInfo.TRIndex = packetInfo.RTAIndex + packetInfo.RTALength;
                //if (Checker(node, bitTimeLen)) node = node.Child;
                //if (node.Child.StartIndex < packetInfo.TRIndex + 0.5 * bitTimeLen)
                if (node.Child.StartIndex < packetInfo.TRIndex)
                {
                    status = node.Edge != Edge.Falling;
                }
                else
                {
                    status = node.Child.Edge == Edge.Falling;
                    node = node.Child;
                }

                packetInfo.TR = status;
                packetInfo.TempData |= (UInt16)(status ? (1 << (Int32)(DataBitCount - (SubAddressBitCount + ModelCodeBitCount) - 1)) : 0);

                if (node == null)
                {
                    packetInfo.TRLength = (Int32)datalen - packetInfo.TRIndex;
                    return;
                }
                lastStatus = false;
                if (Checker(node, bitTimeLen)) node = node.Child;
                else lastStatus = true;
                if (node == null)
                {
                    packetInfo.TRLength = bitTimeLen;
                    return;
                }
                else
                {
                    packetInfo.TRLength = bitTimeLen;
                }

                packetInfo.SubAddressIndex = packetInfo.TRIndex + packetInfo.TRLength;
                lastStatus = false;
                for (Int32 index = 0; index < SubAddressBitCount; index++)
                {
                    if (node?.Child == null) break;
                    packetInfo.SubAddressBitCount++;
                    status = node.Child.Edge == Edge.Falling;
                    packetInfo.TempData |= (UInt16)(status ? (1 << (Int32)(DataBitCount - (index + RTAddressBitCount) - 1)) : 0);
                    node = node.Child;
                    if (node == null) break;
                    if (Checker(node, bitTimeLen)) node = node.Child;
                    else if (index == SubAddressBitCount - 1) lastStatus = true;
                }
                packetInfo.HasSubAddress = packetInfo.SubAddressBitCount > 0;
                packetInfo.SubAddress = (Byte)((packetInfo.TempData >> (Int32)(RTAddressBitCount + 1)) & _SAMask);
                if (node == null)
                {
                    packetInfo.SubAddressLength = (Int32)datalen - packetInfo.SubAddressIndex;
                    return;
                }
                else
                {
                    //packetInfo.SubAddressLength = (Int32)(node.StartIndex - packetInfo.SubAddressIndex + (lastatus ? 0.5 : 0) * bitTimeLen);
                    packetInfo.SubAddressLength = bitTimeLen * 5;
                }

                packetInfo.ModelCodeIndex = packetInfo.SubAddressIndex + packetInfo.SubAddressLength;
                lastStatus = false;
                for (Int32 index = 0; index < ModelCodeBitCount; index++)
                {
                    if (node?.Child == null) break;
                    packetInfo.ModelCodeBitCount++;
                    status = node.Child.Edge == Edge.Falling;
                    packetInfo.TempData |= (UInt16)(status ? (1 << (Int32)(DataBitCount - index - 1)) : 0);
                    node = node.Child;
                    if (node == null) break;
                    if (Checker(node, bitTimeLen)) node = node.Child;
                    else if (index == ModelCodeBitCount - 1) lastStatus = true;
                }
                packetInfo.HasModelCode = packetInfo.ModelCodeBitCount > 0;
                packetInfo.ModelCode = (Byte)((packetInfo.TempData >> (Int32)(DataBitCount - ModelCodeBitCount)) & _MCMask);
                if (node == null)
                {
                    packetInfo.ModelCodeLength = (Int32)datalen - packetInfo.ModelCodeIndex;
                    return;
                }
                else
                {
                    packetInfo.ModelCodeLength = (Int32)(node.StartIndex - packetInfo.ModelCodeIndex + (lastStatus ? 0.5 : 0) * bitTimeLen);
                }
                lastindex = packetInfo.ModelCodeIndex + packetInfo.ModelCodeLength;
            }
        }

        private sealed class MILStatusDecoder : MILBaseDecoder
        {
            public override MILPacketType PacketType => MILPacketType.Status;

            public override void Decode(ref MILPacketInfo packetInfo, ref MILEdgeInfo? node, Int32 bitTimeLen, UInt32 datalen, ref Int32 lastindex)
            {
                if (node == null) return;
                Boolean status = false;
                packetInfo.RTAIndex = packetInfo.SyncIndex + packetInfo.SyncLength;
                Boolean laststatus = false;
                for (Int32 index = 0; index < RTAddressBitCount; index++)
                {
                    if (node?.Child == null) break;
                    packetInfo.RTABitCount++;
                    status = node.Child.Edge == Edge.Falling;
                    packetInfo.TempData |= (UInt16)(status ? (1 << (Int32)(DataBitCount - (index + 16 - RTAddressBitCount) - 1)) : 0);
                    node = node.Child;
                    if (node == null) break;
                    if (Checker(node, bitTimeLen)) node = node.Child;
                    else if (index == RTAddressBitCount - 1) laststatus = true;
                }
                packetInfo.HasRTA = packetInfo.RTABitCount > 0;
                packetInfo.RTA = (Byte)(packetInfo.TempData & _RTAMask);
                if (node == null)
                {
                    packetInfo.RTALength = (Int32)datalen - packetInfo.RTAIndex;
                    return;
                }
                else
                {
                    packetInfo.RTALength = (Int32)(node.StartIndex - packetInfo.RTAIndex);
                }

                if (node.Child == null) return;
                packetInfo.HasMessageError = true;
                packetInfo.MessageErrorIndex = packetInfo.RTAIndex + packetInfo.RTALength;
                status = node.Child.Edge == Edge.Falling;
                packetInfo.MessageError = status;
                packetInfo.TempData |= (UInt16)(status ? (1 << (Int32)(DataBitCount - 11)) : 0);
                node = node.Child;
                if (node == null)
                {
                    packetInfo.MessageErrorLength = (Int32)datalen - packetInfo.MessageErrorIndex;
                    return;
                }
                laststatus = false;
                if (Checker(node, bitTimeLen)) node = node.Child;
                else laststatus = true;
                if (node == null)
                {
                    packetInfo.MessageErrorLength = (Int32)datalen - packetInfo.MessageErrorIndex;
                    return;
                }
                else
                {
                    packetInfo.MessageErrorLength = (Int32)(node.StartIndex - packetInfo.MessageErrorIndex + (laststatus ? 0.5 : 0) * bitTimeLen);
                }

                if (node?.Child == null) return;
                packetInfo.HasInstrumentation = true;
                packetInfo.InstrumentationIndex = packetInfo.MessageErrorIndex + packetInfo.MessageErrorLength;
                status = node.Child.Edge == Edge.Falling;
                packetInfo.Instrumentation = status;
                packetInfo.TempData |= (UInt16)(status ? (1 << (Int32)(DataBitCount - 10)) : 0);
                node = node.Child;
                if (node == null)
                {
                    packetInfo.InstrumentationLength = (Int32)datalen - packetInfo.InstrumentationIndex;
                    return;
                }
                laststatus = false;
                if (Checker(node, bitTimeLen)) node = node.Child;
                else laststatus = true;
                if (node == null)
                {
                    packetInfo.InstrumentationLength = (Int32)datalen - packetInfo.InstrumentationIndex;
                    return;
                }
                else
                {
                    packetInfo.InstrumentationLength = (Int32)(node.StartIndex - packetInfo.InstrumentationIndex + (laststatus ? 0.5 : 0) * bitTimeLen);
                }

                if (node.Child == null) return;
                packetInfo.HasServiceRequest = true;
                packetInfo.ServiceRequestIndex = packetInfo.InstrumentationIndex + packetInfo.InstrumentationLength;
                status = node.Child.Edge == Edge.Falling;
                packetInfo.ServiceResquest = status;
                packetInfo.TempData |= (UInt16)(status ? (1 << (Int32)(DataBitCount - 9)) : 0);
                node = node.Child;
                if (node == null)
                {
                    packetInfo.ServiceRequestLength = (Int32)datalen - packetInfo.ServiceRequestIndex;
                    return;
                }
                laststatus = false;
                if (Checker(node, bitTimeLen)) node = node.Child;
                else laststatus = true;
                if (node == null)
                {
                    packetInfo.ServiceRequestLength = (Int32)datalen - packetInfo.ServiceRequestIndex;
                    return;
                }
                else
                {
                    packetInfo.ServiceRequestLength = (Int32)(node.StartIndex - packetInfo.ServiceRequestIndex + (laststatus ? 0.5 : 0) * bitTimeLen);
                }

                packetInfo.ReservedIndex = (Int32)(packetInfo.ServiceRequestIndex + packetInfo.ServiceRequestLength);
                laststatus = false;
                for (Int32 index = 0; index < ReservedBitCount; index++)
                {
                    if (node?.Child == null) break;
                    packetInfo.ReservedBitCount++;
                    status = node.Child.Edge == Edge.Falling;
                    packetInfo.TempData |= (UInt16)(status ? (1 << (Int32)(DataBitCount - 6 + index)) : 0);
                    node = node.Child;
                    if (node == null) break;
                    if (Checker(node, bitTimeLen)) node = node.Child;
                    else if (index == ReservedBitCount - 1) laststatus = true;
                }
                packetInfo.HasReserved = packetInfo.ReservedBitCount > 0;
                packetInfo.Reserved = (Byte)((packetInfo.TempData >> 11) & _RMask);
                if (node == null)
                {
                    packetInfo.ReservedLength = (Int32)datalen - packetInfo.ReservedIndex;
                    return;
                }
                else
                {
                    packetInfo.ReservedLength = (Int32)(node.StartIndex - packetInfo.ReservedIndex + (laststatus ? 0.5 : 0) * bitTimeLen);
                }

                if (node?.Child == null) return;
                packetInfo.HasBroadcastCommandReceived = true;
                packetInfo.BroadcastCommandReceivedIndex = packetInfo.ReservedIndex + packetInfo.ReservedLength;
                status = node.Child.Edge == Edge.Falling;
                packetInfo.BroadcastCommandReceived = status;
                packetInfo.TempData |= (UInt16)(status ? (1 << (Int32)(DataBitCount - 5)) : 0);
                node = node.Child;
                if (node == null)
                {
                    packetInfo.BroadcastCommandReceivedLength = (Int32)datalen - packetInfo.BroadcastCommandReceivedIndex;
                    return;
                }
                laststatus = false;
                if (Checker(node, bitTimeLen)) node = node.Child;
                else laststatus = true;
                if (node == null)
                {
                    packetInfo.BroadcastCommandReceivedLength = (Int32)datalen - packetInfo.BroadcastCommandReceivedIndex;
                    return;
                }
                else
                {
                    packetInfo.BroadcastCommandReceivedLength = (Int32)(node.StartIndex - packetInfo.BroadcastCommandReceivedIndex + (laststatus ? 0.5 : 0) * bitTimeLen);
                }

                if (node?.Child == null) return;
                packetInfo.HasBusy = true;
                packetInfo.BusyIndex = packetInfo.BroadcastCommandReceivedIndex + packetInfo.BroadcastCommandReceivedLength;
                status = node.Child.Edge == Edge.Falling;
                packetInfo.Busy = status;
                packetInfo.TempData |= (UInt16)(status ? (1 << (Int32)(DataBitCount - 4)) : 0);
                node = node.Child;
                if (node == null)
                {
                    packetInfo.BusyLength = (Int32)datalen - packetInfo.BusyIndex;
                    return;
                }
                laststatus = false;
                if (Checker(node, bitTimeLen)) node = node.Child;
                else laststatus = true;
                if (node == null)
                {
                    packetInfo.BusyLength = (Int32)datalen - packetInfo.BusyIndex;
                    return;
                }
                else
                {
                    packetInfo.BusyLength = (Int32)(node.StartIndex - packetInfo.BusyIndex + (laststatus ? 0.5 : 0) * bitTimeLen);
                }


                if (node?.Child == null) return;
                packetInfo.HasSubsystemFlag = true;
                packetInfo.SubsystemFlagIndex = packetInfo.BusyIndex + packetInfo.BusyLength;
                status = node.Child.Edge == Edge.Falling;
                packetInfo.SubsystemFlag = status;
                packetInfo.TempData |= (UInt16)(status ? (1 << (Int32)(DataBitCount - 3)) : 0);
                node = node.Child;
                if (node == null)
                {
                    packetInfo.SubsystemFlagLength = (Int32)datalen - packetInfo.SubsystemFlagIndex;
                    return;
                }
                laststatus = false;
                if (Checker(node, bitTimeLen)) node = node.Child;
                else laststatus = true;
                if (node == null)
                {
                    packetInfo.SubsystemFlagLength = (Int32)datalen - packetInfo.SubsystemFlagIndex;
                    return;
                }
                else
                {
                    packetInfo.SubsystemFlagLength = (Int32)(node.StartIndex - packetInfo.SubsystemFlagIndex + (laststatus ? 0.5 : 0) * bitTimeLen);
                }


                if (node?.Child == null) return;
                packetInfo.HasDynamicBusControlAcceptance = true;
                packetInfo.DynamicBusControlAcceptanceIndex = packetInfo.SubsystemFlagIndex + packetInfo.SubsystemFlagLength;
                status = node.Child.Edge == Edge.Falling;
                packetInfo.DynamicBusControlAcceptance = status;
                packetInfo.TempData |= (UInt16)(status ? (1 << (Int32)(DataBitCount - 2)) : 0);
                node = node.Child;
                if (node == null)
                {
                    packetInfo.DynamicBusControlAcceptanceLength = (Int32)datalen - packetInfo.DynamicBusControlAcceptanceIndex;
                    return;
                }
                laststatus = false;
                if (Checker(node, bitTimeLen)) node = node.Child;
                else laststatus = true;
                if (node == null)
                {
                    packetInfo.DynamicBusControlAcceptanceLength = (Int32)datalen - packetInfo.DynamicBusControlAcceptanceIndex;
                    return;
                }
                else
                {
                    packetInfo.DynamicBusControlAcceptanceLength = (Int32)(node.StartIndex - packetInfo.DynamicBusControlAcceptanceIndex + (laststatus ? 0.5 : 0) * bitTimeLen);
                }

                if (node?.Child == null) return;
                packetInfo.HasTerminalFlag = true;
                packetInfo.TerminalFlagIndex = packetInfo.DynamicBusControlAcceptanceIndex + packetInfo.DynamicBusControlAcceptanceLength;
                status = node.Child.Edge == Edge.Falling;
                packetInfo.TerminalFlag = status;
                packetInfo.TempData |= (UInt16)(status ? (1 << (Int32)DataBitCount - 1) : 0);
                node = node.Child;
                if (node == null)
                {
                    packetInfo.TerminalFlagLength = (Int32)datalen - packetInfo.TerminalFlagIndex;
                    return;
                }
                laststatus = false;
                if (Checker(node, bitTimeLen)) node = node.Child;
                else laststatus = true;
                if (node == null)
                {
                    packetInfo.TerminalFlagLength = (Int32)datalen - packetInfo.TerminalFlagIndex;
                    return;
                }
                else
                {
                    packetInfo.TerminalFlagLength = (Int32)(node.StartIndex - packetInfo.TerminalFlagIndex + (laststatus ? 0.5 : 0) * bitTimeLen);
                }
                lastindex = packetInfo.TerminalFlagIndex + packetInfo.TerminalFlagLength;
            }
        }
        private sealed class MILDataDecoder : MILBaseDecoder
        {
            public override MILPacketType PacketType => MILPacketType.Data;

            public override void Decode(ref MILPacketInfo packetInfo, ref MILEdgeInfo? node, Int32 bitTimeLen, UInt32 datalen, ref Int32 lastindex)
            {
                if (node == null) return;
                packetInfo.DataIndex = packetInfo.SyncIndex + packetInfo.SyncLength;
                Boolean laststatus = false;
                for (Int32 index = 0; index < DataBitCount; index++)
                {
                    if (node?.Child == null) break;
                    packetInfo.DataBitCount++;
                    Boolean status = node.Child.Edge == Edge.Falling;
                    packetInfo.TempData |= (UInt16)(status ? (1 << ((Int32)DataBitCount - index - 1)) : 0);
                    node = node.Child;
                    if (node == null) break;
                    if (Checker(node, bitTimeLen)) node = node.Child;
                    else if (index == DataBitCount - 1) laststatus = true;
                }

                packetInfo.HasData = packetInfo.DataBitCount > 0;
                if (node == null)
                {
                    packetInfo.DataLength = (Int32)datalen - packetInfo.DataIndex;
                }
                else
                {
                    packetInfo.DataLength = (Int32)(node.StartIndex - packetInfo.DataIndex + (laststatus ? 0.5 : 0) * bitTimeLen);
                }
                packetInfo.Data = BitConverter.GetBytes(packetInfo.TempData).Reverse().ToArray();
                lastindex = packetInfo.DataIndex + packetInfo.DataLength;
            }
        }
        #endregion Decoder
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.Core.Decode.Model.FlexRay;
using ScopeX.Hardware.Driver;
using static ScopeX.Core.Decode.EdgeInfoCPP;

namespace ScopeX.Core.Decode
{
    internal sealed class FlexRayDecodeModelCPP : ProtocolModel
    {
        public FlexRayDecodeModelCPP(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.FlexRay, isTrigDecode)
        {
            uuid=Guid.NewGuid().ToString();
        }
        public String uuid;
        private List<FlexRayPacketInfo> _PacketInfos = new List<FlexRayPacketInfo>();
        private DecodeResultData _ResultData = new DecodeResultData();
        public override Double BitRateByPs => 1f / CustomSignalRate * 1E12;


        public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>()
        {
            "Index",
            "Start Time",
            "Indicator",
            "Frame ID",
            "Payload Length",
            "Header CRC",
            "Cycle Count",
            "Data",
            "Frame CRC",
            "Error"
        };


        //源类型
        private ProtocolFlexRay.SourceType _SourceType = ProtocolFlexRay.SourceType.BP;
        public ProtocolFlexRay.SourceType SourceType
        {
            get { return _SourceType; }
            set { UpdateProperty(ref _SourceType, value); }
        }



        //源1
        private ChannelId _Source1 = ChannelId.C1;
        public ChannelId Source1
        {
            get { return _Source1; }
            set { UpdateProperty(ref _Source1, value); }
        }
        //源2
        private ChannelId _Source2 = ChannelId.C1;
        public ChannelId Source2
        {
            get { return _Source2; }
            set { UpdateProperty(ref _Source2, value); }
        }
        private Double _Threshold = 1;
        public Double Threshold
        {
            get { return _Threshold*TryGetChannelGain(Source1); }
            set { UpdateProperty(ref _Threshold, value/ TryGetChannelGain(Source1)); }
        }
        public String Unit => GetChannelUnit(Source1);


        public Double MaxThreshold1 => (Single)(12 * TryGetChannelGain(_Source1));
        public Double MaxThreshold2 => (Single)(12 * TryGetChannelGain(_Source2));

        public Double MinThreshold1 => -MaxThreshold1;


        public Double MinThreshold2 => -MaxThreshold2;
        //信号速率
        private ProtocolFlexRay.SignalRate _SignalRate = ProtocolFlexRay.SignalRate.SignalRate_Custom;
        public ProtocolFlexRay.SignalRate SignalRate
        {
            get { return _SignalRate; }
            set
            {
                if (_SignalRate != value)
                {
                    _SignalRate = value;
                    switch (value)
                    {
                        case ProtocolFlexRay.SignalRate.SignalRate_1Mbps:
                            CustomSignalRate = 1000_000;
                            break;
                        case ProtocolFlexRay.SignalRate.SignalRate_5Mbps:
                            CustomSignalRate = 5000_000;
                            break;
                        case ProtocolFlexRay.SignalRate.SignalRate_10Mbps:
                            CustomSignalRate = 10_000_000;
                            break;
                    }

                }
            }
        }
        //自定义的信号速率
        private Int64 _CustomSignalRate = Math.Clamp(0, MinSignalRate, MaxSignalRate);
        public Int64 CustomSignalRate
        {
            get { return _CustomSignalRate; }
            set
            {
                if (value != _CustomSignalRate)
                {
                    switch (value)
                    {
                        case 1000_000:
                            SignalRate = ProtocolFlexRay.SignalRate.SignalRate_1Mbps;
                            break;
                        case 5000_000:
                            SignalRate = ProtocolFlexRay.SignalRate.SignalRate_5Mbps;
                            break;
                        case 10_000_000:
                            SignalRate = ProtocolFlexRay.SignalRate.SignalRate_10Mbps;
                            break;
                        default:
                            SignalRate = ProtocolFlexRay.SignalRate.SignalRate_Custom;
                            break;
                    }
                    UpdateProperty(ref _CustomSignalRate, value);
                }
            }
        }

        public static Int32 MaxSignalRate => 10000000;


        public static Int32 MinSignalRate => 1000000;
        //通道类型
        private ProtocolFlexRay.ChannelType _ChannelType = ProtocolFlexRay.ChannelType.A;
        public ProtocolFlexRay.ChannelType ChannelType
        {
            get { return _ChannelType; }
            set { UpdateProperty(ref _ChannelType, value); }
        }
        public static UInt16 MaxByteCount = 16;
        public static UInt16 MinByteCount = 1;

        private UInt16 _ByteCount = 0;

        public UInt16 ByteCount
        {
            get { return _ByteCount; }
            set { UpdateProperty(ref _ByteCount, value); }
        }

        struct FlexRayPacketInfo
        {
            public Int32 StartIndex;

            public Int32 IndicatorIndex;
            public Int32 IndicatorLen;
            public Int32 Indicator;
            public String IndicatorError;

            public Int32 FrameIDIndex;
            public Int32 FrameIDLen;
            public Int32 FrameID;
            public Boolean FrameIDError;

            public Int32 PayloadLengthIndex;
            public Int32 PayloadLengthLen;
            public Int32 PayloadLength;
            public Boolean PayloadLengthError;

            public Int32 HeaderCRCIndex;
            public Int32 HeaderCRCLen;
            public Int32 HeaderCRC;
            public Boolean HeaderCRCError;
            public Int32 HeaderCRCErrorData;

            public Int32 CycleCountIndex;
            public Int32 CycleCountLen;
            public Int32 CycleCount;
            public Boolean CycleCountError;

            public Int32[] DataIndex;
            public Int32[] DataLen;
            public Int32[] Data;
            public Boolean[] DataError;

            public Int32 FrameCRCIndex;
            public Int32 FrameCRCLen;
            public Int32[] FrameCRC;
            public Boolean FrameCRCError;
            public Int32[] FrameCRCErrorData;

            public Int32 FrameEndIndex;
            public Int32 FrameEndLen;
            public Int32 FrameEnd;

            public Int32 DTSIndex;
            public Int32 DTSLen;
            public Int32 DTS;
            public Boolean DTSError;

            public Int32 CIDIndex;
            public Int32 CIDLen;
            public Int32[] CID;
            public Boolean CIDError;
        }

        internal override Boolean SourceHasData()
        {
            if (DsoPrsnt.DefaultDsoPrsnt == null)
                return false;

            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(_Source1, out var prsnt);
            if (prsnt == null)
                return false;

            if (_Source1.IsReference()  && prsnt.VuDatabase != null && prsnt.VuDatabase.Current != null)
            {
                return DecodeDataHelper.ReferenceHasData(_Source1, Threshold);
            }

            if (_Source1.IsAnalog())
            {
                return DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }

            return false;
        }

        internal override Boolean CheckUpdate(ref Int64 laststamp)
        {
            if (_Source1.IsAnalog() && laststamp != DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp;
                return true;
            }
            if (_Source1.IsReference() && laststamp != DecodeDataHelper.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].TimeStamp;
                return true;
            }

            return false;
        }
        internal override void ParsingData(ref CancellationToken token)
        {
            Int32 chindex = GetChIndex(Source1);
            if (chindex == -1)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _PacketInfos.Clear();
            }
            Boolean needclear = false;
            UInt32 datalen = 0;
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, Source1, ref datalen);
            Double samplerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, Source1, ref samplerate);
            if (MoreThanStorage() || samplerate == 0 || datalen == 0)
            {
                needclear = true;
                _NeedDecodeData = false;
            }
            try
            {
                if (_NeedDecodeData)
                {
                    _NeedDecodeData = false;
                    _PacketInfos.Clear();
                    List<FlexRayPacketInfo> packets = new List<FlexRayPacketInfo>();
                    FlexRayResult result = new FlexRayResult();
                    FlexRayOptions options = new FlexRayOptions();

                    options.SourceType = SourceType == ProtocolFlexRay.SourceType.BP ? Model.FlexRay.SourceType.BP : Model.FlexRay.SourceType.BM;
                    options.ChannelType = ChannelType == ProtocolFlexRay.ChannelType.A ? Model.FlexRay.ChannelType.A : Model.FlexRay.ChannelType.B;
                    options.SignalRate = (UInt32)CustomSignalRate;
                    //options.Cancel_Flag = new IntPtr(cancel_flag);

                    UInt32 perChannelDataLength = 0;
                    DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, Source1, ref perChannelDataLength);

                    DecodeDataHelper.Instance.TryGetSampleRate(BusId, Source1, ref samplerate);

                    TwoLevelEdgeInfo? node = DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, Source1, ref token, ref needclear) as TwoLevelEdgeInfo;
                    if (node != null)
                    {
                        // 边沿脉宽信息获取
                        GCHandle edgePulsesHandle;
                        IntPtr edgepulsePtr = IntPtr.Zero;
                        List<PAM2EdgePulse> _EdgePulsesList = new List<PAM2EdgePulse>();
                        DecodeDataHelper.Instance.GetTwoLevelPulses(ref node, ref _EdgePulsesList);
                        PAM2EdgePulseSequence.Allocate(ref _EdgePulsesList, perChannelDataLength, samplerate, out edgepulsePtr, out edgePulsesHandle);

                        result.EventInfosPtr = IntPtr.Zero;
                        result.EventCount = 0;

                        DecoderImpl.DecodeFlexRay(options, edgepulsePtr, out result);

                        packets = ConvertIntPtrToFlexRayPackInfos(result);

                        edgePulsesHandle.Free();

                        Marshal.FreeHGlobal(edgepulsePtr);

                        DecoderImpl.FreeFlexRay(result);
                    }


                    _PacketInfos.AddRange(packets);
                }


            }
            catch (Exception ex)
            {
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
                try
                {
                    _ResultData.DecodeViewInfos = (_PacketInfos.SelectMany(x =>
                    {
                        var eventinfo = new ProtocolEventInfo();
                        var errorsplit = ",";
                        var errorstr = String.Empty;
                        eventinfo.Index = _EventInfos.Count;
                        var endindex = 0;
                        eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                        List<FlexRayDecodePacket> packets = new List<FlexRayDecodePacket>();
                        if (x.StartIndex != 0)
                        {
                            FlexRayStartDecodePacket start = new FlexRayStartDecodePacket(CalcPosition(x.StartIndex, Source1, chindex), CalcBitLenght(10, Source1, chindex));
                            packets.Add(start);
                            eventinfo.StartTimeByPs = GetTimeFromPosition(start.Start, chindex);
                            eventinfo.StartPosition = start.Start;
                            endindex = x.StartIndex;
                        }
                        if (x.IndicatorLen != 0)
                        {
                            FlexRayIndicatorDecodePacket indicator = new FlexRayIndicatorDecodePacket(CalcPosition(x.IndicatorIndex, Source1, chindex), CalcBitLenght(x.IndicatorLen, Source1, chindex))
                            {
                                BitCount = 4,
                                Error= x.IndicatorError,
                                Data = new Byte[] { (Byte)x.Indicator },
                            };
                            if(!String.IsNullOrEmpty(x.IndicatorError))
                            {
                                errorstr = String.Concat(errorstr, x.IndicatorError);
                            }
                            packets.Add(indicator);
                            eventinfo.EventInofs[0] = (indicator.Data, indicator.BitCount);
                            endindex = x.IndicatorIndex + x.IndicatorLen;
                        }
                        if (x.FrameIDLen != 0)
                        {
                            Byte[] framid = BitConverter.GetBytes((UInt32)x.FrameID).Reverse().ToArray();
                            FlexRayFrameIDDecodePacket frameID = new FlexRayFrameIDDecodePacket(CalcPosition(x.FrameIDIndex, Source1, chindex), CalcBitLenght(x.FrameIDLen, Source1, chindex))
                            {
                                BitCount = 11,
                                Data = new Byte[] { framid[2], framid[3] },
                            };
                            packets.Add(frameID);
                            eventinfo.EventInofs[1] = (frameID.Data, frameID.BitCount);
                            endindex = x.FrameIDIndex + x.FrameIDLen;
                        }
                        if (x.PayloadLengthLen != 0)
                        {
                            FlexRayPayloadLengthDecodePacket payloadLength = new FlexRayPayloadLengthDecodePacket(CalcPosition(x.PayloadLengthIndex, Source1, chindex), CalcBitLenght(x.PayloadLengthLen, Source1, chindex))
                            {
                                BitCount = 7,
                                Data = new Byte[] { (Byte)x.PayloadLength },
                            };
                            packets.Add(payloadLength);
                            eventinfo.EventInofs[2] = (payloadLength.Data, payloadLength.BitCount);
                            endindex = x.PayloadLengthIndex + x.PayloadLengthLen;
                        }
                        if (x.HeaderCRCLen != 0)
                        {
                            Byte[] headercrc = BitConverter.GetBytes((UInt32)x.HeaderCRC).Reverse().ToArray();
                            FlexRayHeaderCRCDecodePacket hcrc = new FlexRayHeaderCRCDecodePacket(CalcPosition(x.HeaderCRCIndex, Source1, chindex), CalcBitLenght(x.HeaderCRCLen, Source1, chindex))
                            {
                                BitCount = 11,
                                Data = new Byte[] { headercrc[2], headercrc[3] },
                            };
                            if(x.HeaderCRCError)
                            {
                                hcrc.Error = true;
                                Byte[] error = BitConverter.GetBytes((UInt32)x.HeaderCRCErrorData).Reverse().ToArray();

                                hcrc.CRC = new Byte[] { error[2], error[3] };

                                errorstr = String.IsNullOrEmpty(errorstr) ? nameof(x.HeaderCRC) : String.Concat(errorstr, errorsplit, nameof(x.FrameCRCError));
                            }
                            packets.Add(hcrc);
                            eventinfo.EventInofs[3] = (hcrc.Data, hcrc.BitCount);
                            endindex = x.HeaderCRCIndex + x.HeaderCRCLen;
                        }
                        if (x.CycleCountLen != 0)
                        {
                            FlexRayCycleCountDecodePacket cycleCount = new FlexRayCycleCountDecodePacket(CalcPosition(x.CycleCountIndex, Source1, chindex), CalcBitLenght(x.CycleCountLen, Source1, chindex))
                            {
                                BitCount = 6,
                                Data = new Byte[] { (Byte)x.CycleCount },
                            };
                            packets.Add(cycleCount);
                            eventinfo.EventInofs[4] = (cycleCount.Data, cycleCount.BitCount);
                            endindex = x.CycleCountIndex + x.CycleCountLen;
                        }
                        if (x.Data != null)
                        {
                            Byte[] databytes = new Byte[x.Data.Length];
                            for (Int32 i = 0; i < x.Data.Length; i++)
                            {
                                FlexRayDataDecodePacket data = new FlexRayDataDecodePacket(CalcPosition(x.DataIndex[i], Source1, chindex), CalcBitLenght(x.DataLen[i], Source1, chindex))
                                {
                                    BitCount = 8,
                                    Data = new Byte[] { (Byte)x.Data[i] },
                                };
                                packets.Add(data);
                                databytes[i] = (Byte)x.Data[i];
                                if (x.DataIndex[i] != 0)
                                {
                                    endindex = x.DataIndex[i] + x.DataLen[i];
                                }
                            }
                            eventinfo.EventInofs[5] = (databytes, (UInt32)x.Data.Length * 8);
                        }
                        if (x.FrameCRCLen != 0)
                        {
                            FlexRayFrameCRCDecodePacket fcrc = new FlexRayFrameCRCDecodePacket(CalcPosition(x.FrameCRCIndex, Source1, chindex), CalcBitLenght(x.FrameCRCLen, Source1, chindex))
                            {
                                BitCount = 24,
                                Data = new Byte[] { (Byte)x.FrameCRC[0], (Byte)x.FrameCRC[1], (Byte)x.FrameCRC[2] },
                            };
                            if(x.FrameCRCError)
                            {
                                fcrc.Error = true;
                                fcrc.CRC = new Byte[] { (Byte)x.FrameCRCErrorData[0], (Byte)x.FrameCRCErrorData[1], (Byte)x.FrameCRCErrorData[2] };
                                errorstr = String.IsNullOrEmpty(errorstr)? nameof(x.FrameCRCError): String.Concat(errorstr, errorsplit, nameof(x.FrameCRCError));
                            }
                            packets.Add(fcrc);
                            eventinfo.EventInofs[6] = (fcrc.Data, fcrc.BitCount);
                            endindex = x.FrameCRCIndex + x.FrameCRCLen;
                        }
                        if (x.FrameEndLen != 0)
                        {
                            FlexRayFrameEndDecodePacket end = new FlexRayFrameEndDecodePacket(CalcPosition(x.FrameEndIndex, Source1, chindex), CalcBitLenght(x.FrameEndLen, Source1, chindex));
                            packets.Add(end);
                            endindex = x.FrameEndIndex + x.FrameEndLen;
                        }
                        if (x.DTS != 0)
                        {
                            FlexRayDTSDecodePacket dts = new FlexRayDTSDecodePacket(CalcPosition(x.DTSIndex, Source1, chindex), CalcBitLenght(x.DTSLen, Source1, chindex))
                            {
                                BitCount = 2,
                                Data = new Byte[] { (Byte)x.DTS },
                            };
                            packets.Add(dts);
                            endindex = x.DTSIndex + x.DTSLen;
                        }
                        if (x.CID != null)
                        {
                            FlexRayCIDDecodePacket cid = new FlexRayCIDDecodePacket(CalcPosition(x.CIDIndex, Source1, chindex), CalcBitLenght(x.CIDLen, Source1, chindex))
                            {
                                BitCount = 11,
                                Data = new Byte[] { (Byte)x.CID[0], (Byte)x.CID[1] },
                            };
                            packets.Add(cid);
                            endindex = x.CIDIndex + x.CIDLen;
                        }
                        if(!String.IsNullOrEmpty(errorstr))
                        {
                            Byte[] errorData = Encoding.Default.GetBytes(errorstr);
                            eventinfo.EventInofs[7] = (errorData, 0);
                        }
                        eventinfo.EndPosition = CalcPosition(endindex, Source1, chindex);
                        eventinfo.EndTimeByPs= GetTimeFromPosition(eventinfo.EndPosition, chindex);
                        _EventInfos.Add(eventinfo);
                        return packets;
                    }).ToArray());
                    decodedatas.Add(_ResultData);
                    ChangeBuffer();
                }
                catch (Exception ex)
                {
                }
            }

        }

        private static List<FlexRayPacketInfo> ConvertIntPtrToFlexRayPackInfos(FlexRayResult results)
        {
            List<FlexRayPacketInfo> packs = new List<FlexRayPacketInfo>();

            FlexRayEvent[] events = new FlexRayEvent[results.EventCount];
            Int32 rstsize = Marshal.SizeOf(typeof(FlexRayEvent));
            IntPtr rstptr = results.EventInfosPtr;

            for (UInt64 i = 0; i < results.EventCount; i++)
            {
                IntPtr member = new IntPtr(rstptr.ToInt64() + (Int64)i * rstsize);
                events[i] = (FlexRayEvent)(Marshal.PtrToStructure(member, typeof(FlexRayEvent)) ?? throw new ArgumentException());
            }

            for (UInt32 i = 0; i < results.EventCount; i++)
            {
                Byte[] data = new Byte[events[i].DataCount];
                FlexRayEventField[] fields = new FlexRayEventField[events[i].FieldCount];

                Int32 datasize = Marshal.SizeOf(typeof(Byte));
                IntPtr dataptr = events[i].Data;

                for (UInt32 ii = 0; ii < events[i].DataCount; ii++)
                {
                    IntPtr member = new IntPtr(dataptr.ToInt64() + ii * datasize);
                    data[ii] = (Byte)(Marshal.PtrToStructure(member, typeof(Byte)) ?? throw new ArgumentException());
                }

                Int32 fieldsize = Marshal.SizeOf(typeof(FlexRayEventField));
                IntPtr fieldptr = events[i].Fields;

                for (UInt32 ii = 0; ii < events[i].FieldCount; ii++)
                {
                    IntPtr member = new IntPtr(fieldptr.ToInt64() + ii * fieldsize);
                    fields[ii] = (FlexRayEventField)(Marshal.PtrToStructure(member, typeof(FlexRayEventField)) ?? throw new ArgumentException());
                }

                //装填数据

                FlexRayPacketInfo pack = new FlexRayPacketInfo();

                List<(Int32 index, Int32 length, Int32 data, Boolean error)> datainfo = new List<(Int32 index, Int32 length, Int32 data, Boolean error)>();

                pack.StartIndex = (Int32)events[i].StartIndex;
                datainfo.Clear();

                for (Int32 ii = 0; ii < events[i].FieldCount; ii++)
                {
                    FlexRayDecodeEventType type = (FlexRayDecodeEventType)Enum.Parse(typeof(FlexRayDecodeEventType), fields[ii].Type.ToString());
                    FlexRayDecodeEventErrorType error = (FlexRayDecodeEventErrorType)Enum.Parse(typeof(FlexRayDecodeEventErrorType), fields[ii].Error.ToString());

                    switch (type)
                    {
                        case FlexRayDecodeEventType.INDICATOR:
                            pack.IndicatorIndex = (Int32)fields[ii].StartIndex;

                            pack.IndicatorLen = (Int32)fields[ii].Length;

                            pack.Indicator = 0 << 1;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                pack.Indicator <<= 1;
                                pack.Indicator += data![fields[ii].DataIndex + iii];
                            }

                            break;
                        case FlexRayDecodeEventType.FRAMEID:
                            pack.FrameIDIndex = (Int32)fields[ii].StartIndex;

                            pack.FrameIDLen = (Int32)fields[ii].Length;

                            pack.FrameID = 0<<1;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                pack.FrameID <<= 1;
                                pack.FrameID += data![fields[ii].DataIndex + iii];
                            }

                            break;
                        case FlexRayDecodeEventType.PAYLOADLENGTH:
                            pack.PayloadLengthIndex = (Int32)fields[ii].StartIndex;

                            pack.PayloadLengthLen = (Int32)fields[ii].Length;

                            pack.PayloadLength = 0 << 1;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                pack.PayloadLength <<= 1;
                                pack.PayloadLength += data![fields[ii].DataIndex + iii];
                            }

                            break;
                        case FlexRayDecodeEventType.HEADERCRC:
                            pack.HeaderCRCIndex = (Int32)fields[ii].StartIndex;

                            pack.HeaderCRCLen = (Int32)fields[ii].Length;

                            pack.HeaderCRC = 0 << 1;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                pack.HeaderCRC <<= 1;
                                pack.HeaderCRC += data![fields[ii].DataIndex + iii];
                            }

                            if(fields[ii].ErrorDataLength>0)
                            {
                                pack.HeaderCRCErrorData = 0 << 1;
                                for (Int32 iii = 0; iii < fields[ii].ErrorDataLength; iii++)
                                {
                                    pack.HeaderCRCErrorData <<= 1;
                                    pack.HeaderCRCErrorData += data![fields[ii].ErrorDataIndex + iii];
                                }
                            }

                            break;
                        case FlexRayDecodeEventType.CYCLECOUNT:
                            pack.CycleCountIndex = (Int32)fields[ii].StartIndex;

                            pack.CycleCountLen = (Int32)fields[ii].Length;

                            pack.CycleCount = 0 << 1;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                pack.CycleCount <<= 1;
                                pack.CycleCount += data![fields[ii].DataIndex + iii];
                            }

                            break;
                        case FlexRayDecodeEventType.DATA:
                            Int32 index = (Int32)fields[ii].StartIndex;
                            Int32 length = (Int32)fields[ii].Length;
                            Int32 dt = 0x00;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                dt <<= 1;
                                dt += data![fields[ii].DataIndex + iii];
                            }

                            datainfo.Add((index, length, dt, error != FlexRayDecodeEventErrorType.NONE));
                            break;
                        case FlexRayDecodeEventType.FRAMECRC:
                            Int32 size = (fields[ii].DataLength / 8) + (fields[ii].DataLength % 8 == 0 ? 0 : 1);
                            pack.FrameCRCIndex = (Int32)fields[ii].StartIndex;
                            pack.FrameCRCLen = (Int32)fields[ii].Length;
                            pack.FrameCRC = new Int32[size];

                            for (Int32 iii = 0; iii < size; iii++)
                            {
                                pack.FrameCRC[iii] = 0x00;
                                var start = iii * 8;
                                var end = (iii + 1) * 8 > fields[ii].DataLength ? fields[ii].DataLength : (iii + 1) * 8;
                                for (Int32 iiii = start; iiii < end; iiii++)
                                {
                                    pack.FrameCRC[iii] <<= 1;
                                    pack.FrameCRC[iii] += data![fields[ii].DataIndex + iiii];
                                }
                            }

                            if (fields[ii].ErrorDataLength > 0)
                            {
                                size = (fields[ii].ErrorDataLength / 8) + (fields[ii].ErrorDataLength % 8 == 0 ? 0 : 1);
                                pack.FrameCRCErrorData = new Int32[size];

                                for (Int32 iii = 0; iii < size; iii++)
                                {
                                    pack.FrameCRCErrorData[iii] = 0x00;
                                    var start = iii * 8;
                                    var end = (iii + 1) * 8 > fields[ii].ErrorDataLength ? fields[ii].ErrorDataLength : (iii + 1) * 8;
                                    for (Int32 iiii = start; iiii < end; iiii++)
                                    {
                                        pack.FrameCRCErrorData[iii] <<= 1;
                                        pack.FrameCRCErrorData[iii] += data![fields[ii].ErrorDataIndex + iiii];
                                    }
                                }
                            }

                            break;
                        case FlexRayDecodeEventType.FRAMEEND:
                            pack.FrameEndIndex = (Int32)fields[ii].StartIndex;

                            pack.FrameEndLen = (Int32)fields[ii].Length;

                            pack.FrameEnd = 0x00;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                pack.FrameEnd <<= 1;
                                pack.FrameEnd += data![fields[ii].DataIndex + iii];
                            }

                            break;
                        case FlexRayDecodeEventType.DTS:
                            pack.DTSIndex = (Int32)fields[ii].StartIndex;

                            pack.DTSLen = (Int32)fields[ii].Length;

                            pack.DTS = 0x00;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                pack.DTS <<= 1;
                                pack.DTS += data![fields[ii].DataIndex + iii];
                            }

                            break;
                        case FlexRayDecodeEventType.CID:
                            pack.CIDIndex = (Int32)fields[ii].StartIndex;

                            pack.CIDLen = (Int32)fields[ii].Length;

                            pack.CID = new Int32[2];

                            for (Int32 iii = 0; iii < 2; iii++)
                            {
                                pack.CID[iii] = 0x00;
                                var start = iii * 8;
                                var end = (iii + 1) * 8 > fields[ii].DataLength ? fields[ii].DataLength : (iii + 1) * 8;
                                for (Int32 iiii = start; iiii < end; iiii++)
                                {
                                    pack.CID[iii] <<= 1;
                                    pack.CID[iii] += data![fields[ii].DataIndex + iiii];
                                }
                            }
                            var temp = pack.CID[0];
                            pack.CID[0] = pack.CID[1];
                            pack.CID[1] = temp;

                            break;
                    }

                    switch (error)
                    {
                        case FlexRayDecodeEventErrorType.RESERVEERROR:
                            pack.IndicatorError = "Reserve error";
                            break;
                        case FlexRayDecodeEventErrorType.SYNCFRAMEERROR:
                            pack.IndicatorError = "SyncFrame error";
                            break;
                        case FlexRayDecodeEventErrorType.FRAMEIDERROR:
                            pack.FrameIDError = true;
                            break;
                        case FlexRayDecodeEventErrorType.PAYLOADLENGTHERROR:
                            pack.PayloadLengthError = true;
                            break;
                        case FlexRayDecodeEventErrorType.HEADERCRCERROR:
                            pack.HeaderCRCError = true;
                            break;
                        case FlexRayDecodeEventErrorType.CYCLECOUNTERROR:
                            pack.CycleCountError = true;
                            break;
                        case FlexRayDecodeEventErrorType.MESSAGEIDERROR:
                            break;
                        case FlexRayDecodeEventErrorType.FRAMECRCERROR:
                            pack.FrameCRCError = true;
                            break;
                        case FlexRayDecodeEventErrorType.FRAMEENDERROR:
                            break;
                        case FlexRayDecodeEventErrorType.DTSERROR:
                            pack.DTSError = true;
                            break;
                        case FlexRayDecodeEventErrorType.CIDERROR:
                            pack.CIDError = true;
                            break;
                    }
                }

                pack.Data = new Int32[datainfo.Count];
                pack.DataIndex = new Int32[datainfo.Count];
                pack.DataLen = new Int32[datainfo.Count];
                pack.DataError = new Boolean[datainfo.Count];
                for (Int32 ii = 0; ii < datainfo.Count; ii++)
                {
                    pack.DataIndex[ii] = datainfo[ii].index;
                    pack.DataLen[ii] = datainfo[ii].length;
                    pack.Data[ii] = datainfo[ii].data;
                    pack.DataError[ii] = datainfo[ii].error;
                }

                packs.Add(pack);
            }

            return packs;
        }



        public override void UpdateReferenceDataStatus()
        {
            if (_Source1.IsReference()&& DecodeDataSource.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].Channels != null
                &&DecodeDataSource.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].Channels[0] == _Source1)
            {
                DecodeDataSource.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].HasData = false;
            }
        }

        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolFlexRayOptions()
            {
                Source = Source1,
                SourceL = Source2,
                ChannelType = ChannelType,
                SourceType = SourceType,
                SignalRate = CustomSignalRate,
                Threshold = _Threshold,
            };
        }

    }
}

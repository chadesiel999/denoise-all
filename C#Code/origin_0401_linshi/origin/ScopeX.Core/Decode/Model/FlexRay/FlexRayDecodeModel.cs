using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;

namespace ScopeX.Core.Decode
{
    [Obsolete("Please Use Class 'FlexRayDecodeModelCPP'", true)]
    internal sealed class FlexRayDecodeModel :ProtocolModel
    {
        public FlexRayDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.FlexRay, isTrigDecode)
        {
        }
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
            get { return _Threshold * TryGetChannelGain(Source1); }
            set { UpdateProperty(ref _Threshold, value / TryGetChannelGain(Source1)); }
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
        public Int32 MinPayLoadLen => 0;
        public Int32 MaxPayLoadLen => 127;
        private Int32 _PayLoadLength = 0;
        public Int32 PayLoadLength
        {
            get { return _PayLoadLength; }
            set { UpdateProperty(ref _PayLoadLength, value); }
        }

        public Int32 MinByteOffset => 0;


        public Int32 MaxByteOffset => 128;
        //字节偏置（触发条件：数据）
        private Int32 _ByteOffset = 0;
        public Int32 ByteOffset
        {
            get { return _ByteOffset; }
            set { UpdateProperty(ref _ByteOffset, value); }
        }
        struct FlexRayPacketInfo
        {
            public Int32 StartIndex;

            public Int32 IndicatorIndex;
            public Int32 IndicatorLen;
            public Int32 Indicator;

            public Int32 FrameIDIndex;
            public Int32 FrameIDLen;
            public Int32 FrameID;

            public Int32 PayloadLengthIndex;
            public Int32 PayloadLengthLen;
            public Int32 PayloadLength;

            public Int32 HeaderCRCIndex;
            public Int32 HeaderCRCLen;
            public Int32 HeaderCRC;

            public Int32 CycleCountIndex;
            public Int32 CycleCountLen;
            public Int32 CycleCount;

            public Int32[] DataIndex;
            public Int32[] DataLen;
            public Int32[] Data;

            public Int32[] FrameCRCIndex;
            public Int32[] FrameCRCLen;
            public Int32[] FrameCRC;

            public Int32 FrameEndIndex;
            public Int32 FrameEndLen;
            public Int32 FrameEnd;

            public Int32 DTSIndex;
            public Int32 DTSLen;
            public Int32 DTS;

            public Int32 ErrorIndex;
            public Int32 ErrorLen;
            public Int32 Error;
        }

        internal override Boolean SourceHasData()
        {
            if (DsoPrsnt.DefaultDsoPrsnt == null)
                return false;

            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(_Source1, out var prsnt);
            if (prsnt == null)
                return false;

            if (_Source1.IsReference() && prsnt.VuDatabase != null && prsnt.VuDatabase.Current != null)
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
            if (samplerate == 0 || datalen == 0)
            {
                needclear = true;
                _NeedDecodeData = false;
            }
            try
            {
                if (_NeedDecodeData)
                {
                    _NeedDecodeData = false;
                    Double tolerance = 0.05;//误差值
                    Int32 startindex = 0;
                    Int32 scale = (Int32)(samplerate / CustomSignalRate);
                    //tss为5-15位的低电平
                    Int32 tssminlen = 5 * scale;
                    Int32 tssmaxlen = 15 * scale;
                    _PacketInfos.Clear();
                    while (true)
                    {
                        //寻找传输起始序列, 帧起始序列
                        Int32 tssstartindex = 0;
                        Int32 tssstopindex = 0;
                        //var starttime = DateTime.Now;
                        var starttime = TimeSpanUtility.GetTimestampSpan();
                        while (true)
                        {
                            tssstartindex = FlexRayFindNextFallingEdge(startindex, Source1, ref token, ref needclear);//找下降沿，后面则为低电平0
                            if (tssstartindex == -1)
                                break;
                            tssstopindex = DecodeDataHelper.Instance.FindNextEdge(BusId, tssstartindex + 1, Source1, ref token, ref needclear);//tssstartindex + scale如何在scale里面存在一个异常边沿需要识别出来
                            //tssstopindex = DecodeDataHelper.Instance.FindNextFallingEdge(tssstartindex + 2*scale, Source1, ref token, ref needclear);
                            if (tssstopindex == -1)
                                break;
                            if ((tssstopindex - tssstartindex) >= tssminlen * (1 - tolerance) && (tssstopindex - tssstartindex) <= tssmaxlen * (1 + tolerance))
                            {
                                //fss为帧起始序列，一位高电平
                                Byte fssbit = FlexRayGetBitValue(DecodeDataHelper.Instance.GetLevel(BusId, tssstopindex + scale / 2, Threshold, Source1));//取一位的中间部分电平
                                //字节起始序列，一位高电平并紧随一位低电平
                                Byte bssbit1 = FlexRayGetBitValue(DecodeDataHelper.Instance.GetLevel(BusId, tssstopindex + 3 * scale / 2, Threshold, Source1));//取fss后一位中间部分电平，理论1位高电平1
                                Byte bssbit2 = FlexRayGetBitValue(DecodeDataHelper.Instance.GetLevel(BusId, tssstopindex + 5 * scale / 2, Threshold, Source1));//取fss后两位中间部分电平，理论1位低电平0
                                if (fssbit == 1 && bssbit1 == 1 && bssbit2 == 0)
                                {
                                    startindex = tssstopindex;
                                    break;
                                }
                            }
                            startindex = tssstopindex + scale;
                            if (( /*DateTime.Now*/TimeSpanUtility.GetTimestampSpan() - starttime).TotalMilliseconds > 2000)
                            {
                                return;
                            }
                        }
                        if (tssstartindex == -1 || tssstopindex == -1)
                            break;
                        FlexRayPacketInfo info = new FlexRayPacketInfo();
                        info.StartIndex = tssstartindex;
                        //startindex = DecodeDataHelper.Instance.FindNextEdge(startindex + scale, Source1, ref token, ref needclear);
                        if (startindex >= datalen || startindex == -1)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        startindex += 3 * scale;
                        if (startindex >= datalen || startindex == -1)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        //寻找字节起始序列, 解析帧头段
                        info.IndicatorIndex = startindex;
                        startindex += scale;
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        //解析indicator
                        for (Int32 i = 0; i < 4; i++)
                        {
                            info.Indicator <<= 1;
                            Boolean bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source1);
                            info.Indicator |= FlexRayGetBitValue(bit);
                            startindex += scale;
                            if (startindex >= datalen)
                            {
                                break;
                            }
                        }
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        info.IndicatorLen = startindex - info.IndicatorIndex;
                        //解析frameID
                        info.FrameIDIndex = startindex;
                        for (Int32 i = 0; i < 3; i++)
                        {
                            info.FrameID <<= 1;
                            Boolean bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source1);
                            info.FrameID |= FlexRayGetBitValue(bit);
                            startindex += scale;
                            if (startindex >= datalen)
                            {
                                break;
                            }
                        }
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }

                        startindex += 2 * scale; //+bss字节起始序列
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        for (Int32 i = 0; i < 8; i++)
                        {
                            info.FrameID <<= 1;
                            Boolean bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source1);
                            info.FrameID |= FlexRayGetBitValue(bit);
                            startindex += scale;
                            if (startindex >= datalen)
                            {
                                break;
                            }
                        }
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        info.FrameIDLen = startindex - info.FrameIDIndex;
                        //解析PayloadLength
                        startindex += 2 * scale; //+bss字节起始序列
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        info.PayloadLengthIndex = startindex;
                        for (Int32 i = 0; i < 7; i++)
                        {
                            info.PayloadLength <<= 1;
                            Boolean bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source1);
                            info.PayloadLength |= FlexRayGetBitValue(bit);
                            startindex += scale;
                            if (startindex >= datalen)
                            {
                                break;
                            }
                        }
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        info.PayloadLengthLen = startindex - info.PayloadLengthIndex;
                        //解析HeaderCRC
                        info.HeaderCRCIndex = startindex;
                        info.HeaderCRC <<= 1;
                        Boolean headerCRCbit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source1);
                        info.HeaderCRC |= FlexRayGetBitValue(headerCRCbit);
                        startindex += scale;
                        startindex += 2 * scale; //+bss字节起始序列
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        for (Int32 i = 0; i < 8; i++)
                        {
                            info.HeaderCRC <<= 1;
                            Boolean bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source1);
                            info.HeaderCRC |= FlexRayGetBitValue(bit);
                            startindex += scale;
                            if (startindex >= datalen)
                                break;
                        }
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        startindex += 2 * scale; //+bss字节起始序列
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        for (Int32 i = 0; i < 2; i++)
                        {
                            info.HeaderCRC <<= 1;
                            Boolean bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source1);
                            info.HeaderCRC |= FlexRayGetBitValue(bit);
                            startindex += scale;
                            if (startindex >= datalen)
                                break;
                        }
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        info.HeaderCRCLen = startindex - info.HeaderCRCIndex;
                        //解析cycle count
                        info.CycleCountIndex = startindex;
                        for (Int32 i = 0; i < 6; i++)
                        {
                            info.CycleCount <<= 1;
                            Boolean bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source1);
                            info.CycleCount |= FlexRayGetBitValue(bit);
                            startindex += scale;
                            if (startindex >= datalen)
                                break;
                        }
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        info.CycleCountLen = startindex - info.CycleCountIndex;
                        //解析有效数据段
                        info.DataIndex = new Int32[info.PayloadLength * 2];
                        info.Data = new Int32[info.PayloadLength * 2];
                        info.DataLen = new Int32[info.PayloadLength * 2];
                        for (Int32 i = 0; i < info.Data.Length; i++)
                        {
                            startindex += 2 * scale; //+bss字节起始序列
                            if (startindex >= datalen)
                            {
                                _PacketInfos.Add(info);
                                break;
                            }
                            info.DataIndex[i] = startindex;
                            for (Int32 j = 0; j < 8; j++)
                            {
                                info.Data[i] <<= 1;
                                Boolean bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source1);
                                info.Data[i] |= FlexRayGetBitValue(bit);
                                startindex += scale;
                                if (startindex >= datalen)
                                    break;
                            }
                            if (startindex >= datalen)
                                break;
                            info.DataLen[i] = startindex - info.DataIndex[i];
                        }
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        //解析帧尾段CRC
                        info.FrameCRCIndex = new Int32[3];
                        info.FrameCRC = new Int32[3];
                        info.FrameCRCLen = new Int32[3];
                        for (Int32 i = 0; i < 3; i++)
                        {
                            startindex += 2 * scale; //+bss字节起始序列
                            if (startindex >= datalen)
                            {
                                _PacketInfos.Add(info);
                                break;
                            }
                            info.FrameCRCIndex[i] = startindex;
                            for (Int32 j = 0; j < 8; j++)
                            {
                                info.FrameCRC[i] <<= 1;
                                Boolean bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source1);
                                info.FrameCRC[i] |= FlexRayGetBitValue(bit);
                                startindex += scale;
                                if (startindex >= datalen)
                                    break;
                            }
                            if (startindex >= datalen)
                                break;
                            info.FrameCRCLen[i] = startindex - info.FrameCRCIndex[i];
                        }
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        //寻找帧结束序列
                        info.FrameEndIndex = startindex;
                        startindex += 2 * scale;
                        info.FrameEndLen = startindex - info.FrameEndIndex;
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }
                        //寻找动态段帧尾序列
                        if (FlexRayGetBitValue(DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source1)) == 0)
                        {
                            info.DTSIndex = startindex;
                            startindex = DecodeDataHelper.Instance.FindNextEdge(BusId, startindex + scale, Source1, ref token, ref needclear);
                            if (startindex == -1 || startindex >= datalen)
                            {
                                _PacketInfos.Add(info);
                                break;
                            }
                            startindex += scale;
                            if (startindex >= datalen)
                            {
                                _PacketInfos.Add(info);
                                break;
                            }
                            info.DTS = 1;
                            info.DTSLen = startindex - info.DTSIndex;
                        }
                        //解析帧数据完成
                        _PacketInfos.Add(info);
                        startindex += scale;
                        if (startindex >= datalen)
                        {
                            _PacketInfos.Add(info);
                            break;
                        }

                    }
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
                        var endindex = 0;
                        eventinfo.Index = _EventInfos.Count;
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
                                Data = new Byte[] { (Byte)x.Indicator },
                            };
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
                            FlexRayHeaderCRCDecodePacket headerCRC = new FlexRayHeaderCRCDecodePacket(CalcPosition(x.HeaderCRCIndex, Source1, chindex), CalcBitLenght(x.HeaderCRCLen, Source1, chindex))
                            {
                                BitCount = 11,
                                Data = new Byte[] { headercrc[2], headercrc[3] },
                            };
                            packets.Add(headerCRC);
                            eventinfo.EventInofs[3] = (headerCRC.Data, headerCRC.BitCount);
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
                        if (x.FrameCRCLen != null)
                        {
                            FlexRayFrameCRCDecodePacket frameCRC = new FlexRayFrameCRCDecodePacket(CalcPosition(x.FrameCRCIndex[0], Source1, chindex), CalcBitLenght(x.FrameCRCLen[2] + x.FrameCRCIndex[2] - x.FrameCRCIndex[0], Source1, chindex))
                            {
                                BitCount = 24,
                                Data = new Byte[] { (Byte)x.FrameCRC[0], (Byte)x.FrameCRC[1], (Byte)x.FrameCRC[2] },
                            };
                            packets.Add(frameCRC);
                            eventinfo.EventInofs[6] = (frameCRC.Data, frameCRC.BitCount);
                            endindex = x.FrameCRCIndex[2] + x.FrameCRCLen[2];
                        }
                        if (x.FrameEndLen != 0)
                        {
                            FlexRayFrameEndDecodePacket end = new FlexRayFrameEndDecodePacket(CalcPosition(x.FrameEndIndex, Source1, chindex), CalcBitLenght(x.FrameEndLen, Source1, chindex));
                            packets.Add(end);
                            endindex = x.FrameEndIndex + x.FrameEndLen;
                        }
                        if (x.DTS != 0)
                        {
                            FlexRayDTSDecodePacket dTS = new FlexRayDTSDecodePacket(CalcPosition(x.DTSIndex, Source1, chindex), CalcBitLenght(x.DTSLen, Source1, chindex))
                            {
                                BitCount = 2,
                                Data = new Byte[] { (Byte)x.DTS },
                            };
                            packets.Add(dTS);
                            endindex = x.DTSIndex + x.DTSLen;
                        }
                        eventinfo.EndPosition = CalcPosition(endindex, Source1, chindex);
                        eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, chindex);
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

        private Int32 FlexRayFindNextFallingEdge(Int32 startindex, ComModel.ChannelId ch, ref CancellationToken token, ref Boolean needclear)
        {
            if (SourceType == ProtocolFlexRay.SourceType.BP)
            {
                return DecodeDataHelper.Instance.FindNextFallingEdge(BusId, startindex, ch, ref token, ref needclear);
            }
            else
            {
                return DecodeDataHelper.Instance.FindNextRisingEdge(BusId, startindex, ch, ref token, ref needclear);
            }
        }

        private Int32 FlexRayFindNextRisingEdge(Int32 startindex, ComModel.ChannelId ch, ref CancellationToken token, ref Boolean needclear)
        {
            if (SourceType == ProtocolFlexRay.SourceType.BP)
            {
                return DecodeDataHelper.Instance.FindNextRisingEdge(BusId, startindex, ch, ref token, ref needclear);
            }
            else
            {
                return DecodeDataHelper.Instance.FindNextFallingEdge(BusId, startindex, ch, ref token, ref needclear);
            }
        }

        private Byte FlexRayGetBitValue(Boolean bit)
        {
            if (SourceType == ProtocolFlexRay.SourceType.BP)
            {
                return (Byte)(bit ? 1 : 0);
            }
            else
            {
                return (Byte)(bit ? 0 : 1);
            }
        }


        public override void UpdateReferenceDataStatus()
        {
            if (_Source1.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].Channels != null
                && DecodeDataSource.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].Channels[0] == _Source1)
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

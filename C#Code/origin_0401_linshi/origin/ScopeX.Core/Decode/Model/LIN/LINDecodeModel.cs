using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using NPOI.Util;
using NPOI.Util.ArrayExtensions;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using SixLabors.ImageSharp.ColorSpaces;
using static ScopeX.Core.Decode.Model.LIN.LinDecodeStruct;



namespace ScopeX.Core.Decode
{
    [Obsolete("Please Use Class 'LINDecodeModelCPP'", true)]
    internal sealed class LINDecodeModel :ProtocolModel
    {
        private List<LINPacketInfo> _PacketInfos = new List<LINPacketInfo>();
        private DecodeResultData _ResultData = new DecodeResultData();
        public LINDecodeModel(ChannelId id,Boolean isTrigDecode = false) : base(id,SerialProtocolType.LIN, isTrigDecode)
        {
            _ResultData.Name = "LIN";
            Uuid=Guid.NewGuid().ToString();
        }

        private String Uuid;

        public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>()
        {
            "Index",
            "Start Time",
            "PID",
            "Data",
            "Checksum",
            "Error",
        };


        public override Double BitRateByPs => 1f / CustomBPS * 1E+12;
        private ChannelId _Source = ChannelId.C1;

        public ChannelId Source
        {
            get { return _Source; }
            set { UpdateProperty(ref _Source, value); }
        }

        private ProtocolLIN.Standard _Standard = ProtocolLIN.Standard.V1;

        public ProtocolLIN.Standard Standard
        {
            get { return _Standard; }
            set { UpdateProperty(ref _Standard, value); }
        }

        private Boolean _CheckPIDParity = true;

        public Boolean CheckPIDParity
        {
            get { return _CheckPIDParity; }
            set { UpdateProperty(ref _CheckPIDParity, value); }
        }

        private ProtocolCommon.Polarity _Polarity = ProtocolCommon.Polarity.Positive;

        public ProtocolCommon.Polarity Polarity
        {
            get { return _Polarity; }
            set { UpdateProperty(ref _Polarity, value); }
        }
        private ProtocolLIN.PIncludeOddEven _PIncludeOddEven = ProtocolLIN.PIncludeOddEven.Y;

        public ProtocolLIN.PIncludeOddEven PIncludeOddEven
        {
            get { return _PIncludeOddEven; }
            set { UpdateProperty(ref _PIncludeOddEven, value); }
        }
        private Double _Threshold = 1;

        public Double Threshold
        {
            get { return _Threshold * TryGetChannelGain(Source); }
            set { UpdateProperty(ref _Threshold, value/ TryGetChannelGain(Source)); }
        }
        public String Unit=>GetChannelUnit(Source);

        public Double MaxThreshold => (Single)(12 * TryGetChannelGain(_Source));

        public Double MinThreshold => -MaxThreshold;

        private ProtocolLIN.BPS_ID _BPS = ProtocolLIN.BPS_ID.BPS_Special;

        public ProtocolLIN.BPS_ID BPS
        {
            get { return _BPS; }
            set
            {
                if (value != _BPS)
                {
                    _BPS = value;
                    switch (value)
                    {
                        case ProtocolLIN.BPS_ID.BPS_4800:
                            CustomBPS = 4800;
                            break;
                        case ProtocolLIN.BPS_ID.BPS_19200:
                            CustomBPS = 19200;
                            break;
                        case ProtocolLIN.BPS_ID.BPS_2400:
                            CustomBPS = 2400;
                            break;
                        case ProtocolLIN.BPS_ID.BPS_9600:
                            CustomBPS = 9600;
                            break;
                        case ProtocolLIN.BPS_ID.BPS_Special:
                            break;
                    }

                }
            }
        }
        public Int32 MaxDataCount => 8;
        public Int32 MinDataCount => 1;
        private Int32 _DataCount = 8;

        public Int32 DataCount
        {
            get { return _DataCount; }
            set {UpdateProperty(ref _DataCount,value); }
        }


        private Int32 _CustomBPS = Math.Clamp(0, MinBPS, MaxBPS);

        public Int32 CustomBPS
        {
            get { return _CustomBPS; }
            set
            {
                if (value != _CustomBPS)
                {
                    switch (value)
                    {
                        case 2400:
                            BPS = ProtocolLIN.BPS_ID.BPS_2400;
                            break;
                        case 4800:
                            BPS = ProtocolLIN.BPS_ID.BPS_4800;
                            break;
                        case 9600:
                            BPS = ProtocolLIN.BPS_ID.BPS_9600;
                            break;
                        case 19200:
                            BPS = ProtocolLIN.BPS_ID.BPS_19200;
                            break;
                        default:
                            BPS = ProtocolLIN.BPS_ID.BPS_Special;
                            break;
                    }
                    UpdateProperty(ref _CustomBPS, value);
                }
            }
        }
        public static Int32 MaxBPS => 200_000_000;
        public static Int32 MinBPS => 1000;

        struct LINPacketInfo
        {
            public Int32 StartIndex;

            public Int32 SyncIndex;
            public Int32 SyncLen;
            public Int32 Sync;
            public Boolean SyncError;

            public Int32 PIDIndex;
            public Int32 PIDLen;
            public Int32 PID;
            public Boolean PIDParityError;

            public Int32[] DataIndex;
            public Int32[] DataLen;
            public Int32[] Data;
            public Boolean[] DataError;

            public Int32 ChecksumIndex;
            public Int32 ChecksumLen;
            public Int32 Checksum;
            public Int32 Crc;
            public Boolean ChecksumError;

            public Int32 ErrorIndex;
            public Int32 ErrorLen;
            public Int32 Error;
        }

        internal override Boolean SourceHasData()
        {
            if (DsoPrsnt.DefaultDsoPrsnt == null)
                return false;

            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Source, out var prsnt);
            if (prsnt == null)
                return false;

            if (Source.IsReference()  && prsnt.VuDatabase != null && prsnt.VuDatabase.Current != null)
            {
                return DecodeDataHelper.ReferenceHasData(Source, Threshold);
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
            if (chindex == -1)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;

                _PacketInfos.Clear();
            }
            Boolean needclear = false;
            UInt32 datalen = 0;
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, Source, ref datalen);
            Double samplerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, Source, ref samplerate);
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
                    _NeedUpdateViewInfo = true;
                    _PacketInfos.Clear();
                    List<LINPacketInfo> packet = new List<LINPacketInfo>();
#if !USE_CPP_DECODE_LIN
                    #region C#解码
                    Int32 startindex = 0;
                    Int32 scale = (Int32)(samplerate / CustomBPS);
                    Double tolerance = 0.14;//误差值
                    Double syncintervallen = scale * 13;//理论上同步时间间隔为13位
                    while (true)
                    {
                        Int32 syncintervalstartindex = 0;
                        Int32 syncintervalstopindex = 0;
                        Int32 syncintervalnextindex = 0;
                        //var starttime = DateTime.Now;
                        var starttime = TimeSpanUtility.GetTimestampSpan();
                        while (true)
                        {
                            syncintervalstartindex = LinFindNextFallingEdge(startindex, Source, ref token, ref needclear);
                            if (syncintervalstartindex == -1)
                                break;
                            syncintervalstopindex = DecodeDataHelper.Instance.FindNextEdge(BusId, syncintervalstartindex + 1, Source, ref token, ref needclear);

                            syncintervalnextindex = DecodeDataHelper.Instance.FindNextEdge(BusId, syncintervalstopindex + 1, Source, ref token, ref needclear);
                            if (syncintervalstopindex == -1)
                                break;
                            //同步间隔段长度大于13位的隐性，接至少一位少于13位的同步段间隔符
                            if (((syncintervalstopindex - syncintervalstartindex) >= syncintervallen * (1 - tolerance)) && (syncintervalnextindex - syncintervalstopindex) < syncintervallen * (1 + tolerance))
                            {
                                startindex = syncintervalnextindex;
                                break;
                            }
                            startindex = syncintervalstopindex;

                            if (!Debugger.IsAttached)
                            {
                                if (( /*DateTime.Now*/TimeSpanUtility.GetTimestampSpan() - starttime).TotalMilliseconds > 2000)
                                {
                                    return;
                                }
                            }

                        }
                        if (syncintervalstartindex == -1)
                            break;
                        if (syncintervalstopindex == -1)
                            break;
                        if (syncintervalnextindex == -1)
                            break;
                        LINPacketInfo info = new LINPacketInfo();
                        info.StartIndex = syncintervalstartindex;
                        if ((startindex + 10 * scale) >= datalen)
                        {
                            packet.Add(info);
                            break;
                        }
                        info.SyncIndex = startindex + scale;
                        info.SyncLen = 8 * scale;
                        Int32[] syncdata = new Int32[8];
                        for (Int32 i = 0; i < 8; i++)
                        {
                            startindex += scale;
                            if (startindex >= datalen)
                                break;
                            Boolean bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source);

                            syncdata[i] = LinGetBitValue(bit);
                            info.Sync += (syncdata[i] << i);
                        }
                        info.SyncError = info.Sync != 0x55;
                        //解析PID
                        startindex += 2 * scale;
                        startindex = LinFindNextFallingEdge(startindex, Source, ref token, ref needclear);
                        if (startindex == -1 || (startindex + 10 * scale) >= datalen)
                        {
                            packet.Add(info);
                            break;
                        }
                        info.PIDIndex = startindex + scale;
                        Int32[] piddata = new Int32[6];
                        for (Int32 i = 0; i < 6; i++)
                        {
                            startindex += scale;
                            if (startindex >= datalen)
                                break;
                            Boolean bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source);

                            piddata[i] = LinGetBitValue(bit);
                            info.PID |= (piddata[i] << i);

                        }
                        startindex += scale;
                        if (startindex >= datalen)
                            break;
                        Int32[] parity = new Int32[2];
                        parity[0] = LinGetBitValue(DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source));
                        startindex += scale;
                        if (startindex >= datalen)
                            break;
                        parity[1] = LinGetBitValue(DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source));
                        if (CheckPIDParity)
                        {
                            Int32[] pcheck = new Int32[2];
                            pcheck[0] = piddata[0] ^ piddata[1] ^ piddata[2] ^ piddata[4];
                            pcheck[1] = ~(piddata[1] ^ piddata[3] ^ piddata[4] ^ piddata[5]) & 1;
                            if (parity[0] != pcheck[0] || parity[1] != pcheck[1])
                            {
                                info.PIDParityError = true;
                            }
                            else
                            {
                                info.PIDParityError = false;
                            }
                        }
                        startindex += scale;
                        if (startindex == -1 || startindex >= datalen)
                        {
                            packet.Add(info);
                            break;
                        }
                        info.PIDLen = 8 * scale;
                        //解析Data
                        Int32[,] data = new Int32[_DataCount, 8];
                        info.Data = new Int32[_DataCount];
                        info.DataError = new Boolean[_DataCount];
                        info.DataIndex = new Int32[_DataCount];
                        info.DataLen = new Int32[_DataCount];

                        Boolean addcompleted = false;
                        var datastartindex = startindex;
                        for (Int32 i = 0; i < _DataCount; i++)
                        {
                            startindex = LinFindNextFallingEdge(startindex + scale / 2, Source, ref token, ref needclear);
                            if (startindex == -1 || (startindex + 10 * scale) >= datalen)
                            {
                                addcompleted = true;
                                packet.Add(info);
                                break;
                            }

                            //Boolean bit = DecodeDataHelper.Instance.GetLevel(startindex - scale / 2, Threshold, Source);
                            //if(bit&&(startindex - datastartindex)>20*scale)
                            //{
                            //    packet.Add(info);
                            //    break;
                            //}
                            info.DataIndex[i] = startindex;
                            info.DataLen[i] = 10 * scale;
                            Boolean error = false;
                            Boolean bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source);
                            var first = LinGetBitValue(bit);
                            if (first != 0x00)
                            {
                                error = true;
                            }
                            for (Int32 j = 0; j < 8; j++)
                            {
                                startindex += scale;
                                bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source);
                                data[i, j] = LinGetBitValue(bit);
                                info.Data[i] |= (data[i, j] << j);

                            }
                            startindex += scale;
                            bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source);
                            var end = LinGetBitValue(bit);
                            if (end != 0x01)
                            {
                                error = true;
                            }
                            info.DataError[i] = error;
                        }
                        if (addcompleted)
                            break;
                        ;
                        if (startindex == -1 || startindex >= datalen)
                        {
                            packet.Add(info);
                            break;
                        }
                        //解析checksum
                        startindex = LinFindNextFallingEdge(startindex, Source, ref token, ref needclear);
                        if (startindex == -1 || (startindex + 10 * scale) >= datalen)
                        {
                            packet.Add(info);
                            break;
                        }
                        info.ChecksumIndex = startindex + scale;
                        Int32[] checkdata = new Int32[8];
                        for (Int32 i = 0; i < 8; i++)
                        {
                            startindex += scale;
                            if (startindex >= datalen)
                                break;
                            Boolean bit = DecodeDataHelper.Instance.GetLevel(BusId, startindex + scale / 2, Threshold, Source);
                            checkdata[i] = LinGetBitValue(bit);
                            info.Checksum |= (checkdata[i] << i);

                        }
                        //计算理论crc
                        var crc = Standard == ProtocolLIN.Standard.V1 ? info.Data.Sum() : info.Data.Sum() + info.PID;
                        while (crc > 0Xff)
                        {
                            crc -= 0xff;
                        }
                        info.Crc = 0xff - crc;
                        if (info.Checksum != info.Crc)
                        {
                            info.ChecksumError = true;
                        }
                        else
                        {
                            info.ChecksumError = false;
                        }

                        if (startindex == -1 || startindex >= datalen)
                        {
                            packet.Add(info);
                            break;
                        }
                        info.ChecksumLen = 8 * scale;
                        startindex += 2 * scale;
                        packet.Add(info);
                        if (startindex >= datalen)
                            break;
                    }
                    #endregion
#else
                    packet=ParsingLin(ref token, ref needclear);
#endif
                    _PacketInfos.AddRange(packet);
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
                        eventinfo.Index = _EventInfos.Count;
                        eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                        List<LINDecodePacket> packets = new List<LINDecodePacket>();
                        LINStartDecodePacket start = new LINStartDecodePacket(CalcPosition(x.StartIndex, Source, chindex), CalcBitLenght(10, Source, chindex));
                        packets.Add(start);

                        if (x.StartIndex != -1)
                            eventinfo.StartTimeByPs = GetTimeFromPosition(start.Start, chindex);

                        LINSyncDecodePacket sync = new LINSyncDecodePacket(CalcPosition(x.SyncIndex, Source, chindex), CalcBitLenght(x.SyncLen, Source, chindex))
                        {
                            Data= new Byte[] { (Byte)x.Sync },
                            BitCount = 8,
                            SyncError = x.SyncError
                        };
                        packets.Add(sync);
                        if (x.PIDLen != 0)
                        {
                            LINPIDDecodePacket pid = new LINPIDDecodePacket(CalcPosition(x.PIDIndex, Source, chindex), CalcBitLenght(x.PIDLen, Source, chindex))
                            {
                                Data = new Byte[] { (Byte)x.PID },
                                BitCount = 8,
                                PIDParityError = x.PIDParityError,
                            };
                            eventinfo.EventInofs[0] = (pid.Data, pid.BitCount);
                            packets.Add(pid);
                        }
                        Int32 length = 0;
                        if (x.Data != null)
                        {
                            Byte[] databytes = new Byte[x.Data.Length];
                            for (Int32 i = 0; i < x.Data.Length; i++)
                            {
                                LINDataDecodePacket data = new LINDataDecodePacket(CalcPosition(x.DataIndex[i], Source, chindex), CalcBitLenght(x.DataLen[i], Source, chindex))
                                {
                                    Data = new Byte[] { (Byte)x.Data[i] },
                                    DataError = x.DataError[i],
                                    BitCount = 8,
                                };
                                databytes[i] = (Byte)x.Data[i];
                                packets.Add(data);
                            }
                            eventinfo.EventInofs[1] = (databytes, (UInt32)x.Data.Length * 8);
                        }

                        if (x.ChecksumLen != 0)
                        {
                            LINChecksumDecodePacket checksum = new LINChecksumDecodePacket(CalcPosition(x.ChecksumIndex, Source, chindex), CalcBitLenght(x.ChecksumLen, Source, chindex))
                            {
                                Data = new Byte[] { (Byte)x.Checksum },
                                //CRC=new Byte[] { (Byte)x.Crc},
                                ChecksumError=x.ChecksumError,
                                BitCount = 8,
                            };
                            eventinfo.EventInofs[2] = (checksum.Data, checksum.BitCount);
                            packets.Add(checksum);
                        }

                        if (x.Error != 0)
                        {
                            LINErrorDecodePacket error = new LINErrorDecodePacket(CalcPosition(x.ErrorIndex, Source, chindex), CalcBitLenght(x.ErrorLen, Source, chindex))
                            {
                                Data = new Byte[] { (Byte)x.Error },
                                BitCount = 8,
                            };
                            eventinfo.EventInofs[3] = (error.Data, error.BitCount);
                            packets.Add(error);
                        }
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

        private Int32 LinFindNextFallingEdge(Int32 startindex, ComModel.ChannelId ch, ref CancellationToken token, ref Boolean needclear)
        {
            if (Polarity == ProtocolCommon.Polarity.Positive)
            {
                return DecodeDataHelper.Instance.FindNextFallingEdge(BusId, startindex, ch, ref token, ref needclear);
            }
            else
            {
                return DecodeDataHelper.Instance.FindNextRisingEdge(BusId, startindex, ch, ref token, ref needclear);
            }
        }

        private Int32 LinFindNextRisingEdge(Int32 startindex, ComModel.ChannelId ch, ref CancellationToken token, ref Boolean needclear)
        {
            if (Polarity == ProtocolCommon.Polarity.Positive)
            {
                return DecodeDataHelper.Instance.FindNextRisingEdge(BusId, startindex, ch, ref token, ref needclear);
            }
            else
            {
                return DecodeDataHelper.Instance.FindNextFallingEdge(BusId, startindex, ch, ref token, ref needclear);
            }
        }

        private Byte LinGetBitValue(Boolean bit)
        {
            if (Polarity == ProtocolCommon.Polarity.Positive)
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
            if (Source.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].Channels != null
                && DecodeDataSource.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].Channels[0] == Source)
            {
                DecodeDataSource.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].HasData = false;
            }
        }
        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolLINOptions()
            {
                BPS = CustomBPS,
                PIncludeOddEven = PIncludeOddEven,
                Polarity = Polarity,
                Source = Source,
                Standard = Standard,
                Threshold = _Threshold,
                DataCount = _DataCount - 1,
            };
        }
    }
}

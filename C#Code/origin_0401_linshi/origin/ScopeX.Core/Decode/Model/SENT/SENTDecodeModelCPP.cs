using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using NPOI.POIFS.Crypt.Dsig;
using ScopeX.ComModel;
using ScopeX.Core.Decode.Model.SENT;
using ScopeX.Hardware.Driver;
using static ScopeX.Core.Decode.EdgeInfoCPP;

namespace ScopeX.Core.Decode
{
    internal class SENTDecodeModelCPP : ProtocolModel
    {
        private const Int32 _SENTSYNCLEN = 56;
        private List<SENTPacketInfo> _PacketInfos = new List<SENTPacketInfo>();
        private DecodeResultData _ResultData = new DecodeResultData();
        public SENTDecodeModelCPP(ChannelId id, Boolean isTrigDecode = false) : base(id, ComModel.SerialProtocolType.SENT, isTrigDecode)
        {
            _ResultData.Name = "SENT";
        }

        public override Double BitRateByPs => this.RealClockTick * 1E+6;

        private IReadOnlyList<String> _FastTitles => new List<String>()
        {
            "Index",
            "Start Time",
            "Status&Com",
            "Data",
            "CRC",
            "Pause",
            "Error",
        };

        private IReadOnlyList<String> _SlowTitles => new List<String>()
        {
            "Index",
            "Start Time",
            "ID",
            "Data",
            "CRC",
            "Error"
        };

        public override IReadOnlyList<String> EventInfoTitles
        {
            get
            {
                return ChannelMode == ProtocolSENT.ChannelMode.FastChannel ? _FastTitles : _SlowTitles;
            }
        }


        private ComModel.ChannelId _Source;
        public ComModel.ChannelId Source
        {
            get { return _Source; }
            set { UpdateProperty(ref _Source, value); }
        }

        private ProtocolSENT.PauseBit _PauseBit = ProtocolSENT.PauseBit.Yes;

        public ProtocolSENT.PauseBit PauseBit
        {
            get { return _PauseBit; }
            set { UpdateProperty(ref _PauseBit, value); }
        }

        private ProtocolSENT.ChannelMode _ChannelMode = ProtocolSENT.ChannelMode.FastChannel;

        public ProtocolSENT.ChannelMode ChannelMode
        {
            get { return _ChannelMode; }
            set { UpdateProperty(ref _ChannelMode, value); }
        }

        private ProtocolSENT.FastChannelMode _FastChannelMode = ProtocolSENT.FastChannelMode.Nibbles;

        public ProtocolSENT.FastChannelMode FastChannelMode
        {
            get { return _FastChannelMode; }
            set
            {
                UpdateProperty(ref _FastChannelMode, value);
                OnPropertyChanged(nameof(DataLength));
            }
        }

        private ProtocolSENT.DataLength _DataLength = ProtocolSENT.DataLength.Nibbles_6;

        public ProtocolSENT.DataLength DataLength
        {
            get { return _FastChannelMode == ProtocolSENT.FastChannelMode.Nibbles ? _DataLength : ProtocolSENT.DataLength.Nibbles_6; }
            set { UpdateProperty(ref _DataLength, value); }
        }

        private ProtocolCommon.Polarity _Polarity = ProtocolCommon.Polarity.Positive;
        public ProtocolCommon.Polarity Polarity
        {
            get { return _Polarity; }
            set { UpdateProperty(ref _Polarity, value); }
        }

        private ProtocolSENT.ClockTick _ClockTick = ProtocolSENT.ClockTick.ClockTick_Custom;

        public ProtocolSENT.ClockTick ClockTick
        {
            get { return _ClockTick; }
            set
            {
                if (value != _ClockTick)
                {
                    _ClockTick = value;
                    UpdateProperty(ref _ClockTick, value);
                    switch (value)
                    {
                        case ProtocolSENT.ClockTick.MicroSecond_1:
                            RealClockTick = 1E-6;
                            break;
                        case ProtocolSENT.ClockTick.MicroSecond_3:
                            RealClockTick = 3E-6;
                            break;
                        case ProtocolSENT.ClockTick.MicroSecond_10:
                            RealClockTick = 10E-6;
                            break;
                        case ProtocolSENT.ClockTick.MicroSecond_30:
                            RealClockTick = 30E-6;
                            break;
                        case ProtocolSENT.ClockTick.MicroSecond_100:
                            RealClockTick = 100E-6;
                            break;
                        case ProtocolSENT.ClockTick.MicroSecond_300:
                            RealClockTick = 300E-6;
                            break;
                        case ProtocolSENT.ClockTick.ClockTick_Custom:
                            break;
                    }
                }
               
            }
        }

        private Double _RealClockTick = 3E-6;

        public Double RealClockTick
        {
            get { return _RealClockTick; }
            set
            {
                if (_RealClockTick != value)
                {
                    UpdateProperty(ref _RealClockTick, value);
                    switch (value)
                    {
                        case 1E-6:
                            ClockTick = ProtocolSENT.ClockTick.MicroSecond_1;
                            break;
                        case 3E-6:
                            ClockTick = ProtocolSENT.ClockTick.MicroSecond_3;
                            break;
                        case 10E-6:
                            ClockTick = ProtocolSENT.ClockTick.MicroSecond_10;
                            break;
                        case 30E-6:
                            ClockTick = ProtocolSENT.ClockTick.MicroSecond_30;
                            break;
                        case 100E-6:
                            ClockTick = ProtocolSENT.ClockTick.MicroSecond_100;
                            break;
                        case 300E-6:
                            ClockTick = ProtocolSENT.ClockTick.MicroSecond_300;
                            break;
                        default:
                            ClockTick = ProtocolSENT.ClockTick.ClockTick_Custom;
                            break;
                    }
                   
                }
            }
        }
        public Double MinClockTick => 0.5E-6;
        public Double MaxClockTick => 300E-6;


        private Int32 _Tolerance = 20;

        public Int32 Tolerance
        {
            get { return _Tolerance; }
            set { UpdateProperty(ref _Tolerance, value); }
        }

        public Int32 MinTolerance => 1;
        public Int32 MaxTolerance => 30;

        public Double MaxThreshold => (Single)(12 * TryGetChannelGain(_Source));
        public Double MinThreshold => -MaxThreshold;

        private Double _Threshold = 1;
        public Double Threshold
        {
            get { return _Threshold * TryGetChannelGain(_Source); }
            set { UpdateProperty(ref _Threshold, value / TryGetChannelGain(_Source)); }
        }

        public String Unit => GetChannelUnit(Source);
        private Boolean _IsLowThreshold = false;

        interface SENTPacketInfo
        {
            public Int32 StartIndex
            {
                get;
                set;
            }
        }

        struct SENTFastPaketInfo : SENTPacketInfo
        {
            public int StartIndex { get; set; }
            public Int32 SyncIndex;
            public Int32 StatusIndex;
            public Int32[] FastDataIndex;
            public Int32 FastCRCIndex;
            public Int32 PauseIndex;
            public Int32 FastErrorIndex;
            public Int32 SyncLen;
            public Int32 StatusLen;
            public Int32[] FastDataLen;
            public Int32 FastCRCLen;
            public Int32 PauseLen;
            public Int32 ErrorLen;
            public Byte[] Status;
            public Int32[] FastData;
            public Int32 FastCRC;
            public Int32 FastCorrectCRC;
            public Boolean FastCRCError;
            public Int16 Pause;
            public Boolean PauseError;
            public Int32 FastError;
        }

        struct SENTSlowPaketInfo : SENTPacketInfo
        {
            public int StartIndex { get; set; }
            public Int32 SlowIDIndex;
            public Int32 SlowDataIndex;
            public Int32 SlowCRCIndex;
            public Int32 SlowID;
            public Int32 SlowIDCount;
            public Byte[] SlowData;
            public Int32 SlowDataCount;
            public Int32 SlowCRC;
            public Int32 SlowCRCCount;
            public Int32 SlowCorrectCRC;
            public Int32 SlowCorrectCRCCount;
            public Int32 SlowIDLength;
            public Int32 SlowDataLength;
            public Int32 SlowCRCLength;
            public Boolean SlowCRCError;
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
                return DecodeDataHelper.ReferenceHasData(Source, _Threshold);
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
            if (MoreThanStorage() || chindex == -1)
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
                    _PacketInfos.Clear();
                    _NeedDecodeData = false;
                    _NeedUpdateViewInfo = true;
                    List<SENTPacketInfo> packs = new List<SENTPacketInfo>();
                    packs = ParsingSent(ref token, ref needclear);
                    _PacketInfos.AddRange(packs);
                }
            }
            catch
            {
            }
            if (_NeedUpdateViewInfo)
            {
                _NeedUpdateViewInfo = false;
                var decodebuffer = GetDecodeBuffer();
                decodebuffer.Clear();
                _EventInfos.Clear();
                if (_PacketInfos.Count == 0)
                {
                    _ResultData.DecodeViewInfos = new IDecodeViewInfo[0];
                    decodebuffer.Add(_ResultData);
                    ChangeBuffer();
                    return;
                }
                try
                {
                    _ResultData.DecodeViewInfos = _PacketInfos.SelectMany(x =>
                    {
                        String errorstr = String.Empty;
                        List<(Byte[] errordata, UInt32 errordatabitcount)> errordata = new List<(byte[] errordata, uint errordatabitcount)>();
                        ProtocolEventInfo info = new ProtocolEventInfo();
                        var endindex = 0;
                        info.Index = _EventInfos.Count;
                        info.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                        List<SENTDecodePacket> datapackets = new List<SENTDecodePacket>();
                        if (ChannelMode == ProtocolSENT.ChannelMode.FastChannel && x is SENTFastPaketInfo fast)
                        {
                            if (fast.StartIndex > 0)
                            {
                                SENTStartDecodePaket start = new SENTStartDecodePaket(CalcPosition(x.StartIndex, Source, chindex), CalcBitLenght(x.StartIndex + 1, Source, chindex));
                                info.StartTimeByPs = GetTimeFromPosition(start.Start, chindex);
                                info.StartPosition = start.Start;
                                endindex = (Int32)x.StartIndex + 1;
                            }
                            if (fast.SyncLen > 0)
                            {
                                SENTSyncDecodePacket sync = new SENTSyncDecodePacket(CalcPosition(fast.SyncIndex, Source, chindex), CalcBitLenght(fast.SyncLen, Source, chindex));
                                datapackets.Add(sync);
                                info.StartTimeByPs = GetTimeFromPosition(sync.Start, chindex);
                                info.StartPosition = sync.Start;
                                endindex = (Int32)fast.SyncIndex;
                            }
                            if (fast.StatusLen > 0)
                            {
                                var statusinfo = $"{fast.Status[0]} {fast.Status[1]} {fast.Status[2]} {fast.Status[3]}";
                                SENTStatusDecodePacket status = new SENTStatusDecodePacket(CalcPosition(fast.StatusIndex, Source, chindex), CalcBitLenght(fast.StatusLen, Source, chindex))
                                {
                                    Data = Encoding.Default.GetBytes(statusinfo),
                                };
                                info.EventInofs[0] = (status.Data, status.BitCount);
                                datapackets.Add(status);
                                endindex = (Int32)fast.SyncIndex + (Int32)fast.StatusLen;
                            }

                            if (fast.FastData != null && fast.FastData.Length > 0)
                            {
                                Byte[] databytes = new Byte[fast.FastData.Length];
                                for (Int32 i = 0; i < fast.FastData.Length; i++)
                                {
                                    endindex = (Int32)fast.FastDataIndex[i];
                                    SENTDataDecodePacket data = new SENTDataDecodePacket(CalcPosition(fast.FastDataIndex[i], Source, chindex), CalcBitLenght(fast.FastDataLen[i], Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)fast.FastData[i] },
                                        BitCount = 4,
                                    };
                                    databytes[i] = (Byte)fast.FastData[i];
                                    //info.EventInofs[i + 1] = (data.Data, data.BitCount);
                                    datapackets.Add(data);
                                    if (fast.FastDataLen[i] != 0)
                                    {
                                        endindex = (Int32)fast.FastDataIndex[i] + (Int32)fast.FastDataLen[i];
                                    }
                                }
                                info.EventInofs[1] = (databytes, (UInt32)fast.FastData.Length * 8);
                            }
                            if (fast.FastCRCLen > 0)
                            {
                                SENTCRCDecodePacket crc = new SENTCRCDecodePacket(CalcPosition(fast.FastCRCIndex, Source, chindex), CalcBitLenght(fast.FastCRCLen, Source, chindex))
                                {
                                    HasCRCError = fast.FastCRCError,
                                    Data = new Byte[] { (Byte)fast.FastCRC },
                                    BitCount = 4,
                                    ErrorInfoData= new Byte[] { (Byte)fast.FastCorrectCRC },
                                    ErrorInfoBitCount = 4,
                                };
                                if(crc.HasCRCError)
                                {
                                    errorstr += crc.ErrorInfo;
                                    errordata.Add((crc.ErrorInfoData, crc.ErrorInfoBitCount));
                                }
                                info.EventInofs[2] = (crc.Data, crc.BitCount);
                                if (crc.HasCRCError)
                                {
                                    info.EventInofs[4] = (Encoding.Default.GetBytes(crc.ErrorInfo), 0);
                                    info.ExtraInfos.Add((4, crc.ErrorInfoData, crc.ErrorInfoBitCount));
                                }
                                datapackets.Add(crc);
                                endindex = (Int32)fast.FastCRCIndex + (Int32)fast.FastCRCLen;
                            }
                            if (fast.PauseLen > 0)
                            {
                                var pauseinfo = $"{fast.Pause}";
                                SENTPauseDecodePacket pause = new SENTPauseDecodePacket(CalcPosition(fast.PauseIndex, Source, chindex), CalcBitLenght(fast.PauseLen, Source, chindex))
                                {
                                    Data = Encoding.Default.GetBytes(pauseinfo),
                                    PauseValue = fast.Pause,
                                };

                                info.EventInofs[3] = (pause.Data, pause.BitCount);
                                datapackets.Add(pause);
                                endindex = (Int32)fast.PauseIndex + (Int32)fast.PauseLen;
                            }
                            if (fast.FastError > 0)
                            {
                                SENTErrorDecodePacket error = new SENTErrorDecodePacket(CalcPosition(fast.FastErrorIndex, Source, chindex), CalcBitLenght(fast.ErrorLen, Source, chindex))
                                {
                                    Data = new Byte[] { (Byte)fast.FastError },
                                    BitCount = 8,
                                };
                                info.EventInofs[4] = (error.Data, error.BitCount);
                                datapackets.Add(error);
                                endindex = (Int32)fast.FastErrorIndex + (Int32)fast.ErrorLen;
                            }
                        }
                        else if (ChannelMode == ProtocolSENT.ChannelMode.SlowChannel && x is SENTSlowPaketInfo slow)
                        {
                            if (slow.StartIndex > 0)
                            {
                                SENTStartDecodePaket start = new SENTStartDecodePaket(CalcPosition(x.StartIndex, Source, chindex), CalcBitLenght(x.StartIndex + 1, Source, chindex));
                                info.StartTimeByPs = GetTimeFromPosition(start.Start, chindex);
                                info.StartPosition = start.Start;
                                endindex = (Int32)x.StartIndex + 1;
                            }

                            if (slow.SlowIDLength > 0)
                            {
                                SENTSlowIDDecodePacket id = new SENTSlowIDDecodePacket(CalcPosition(slow.SlowIDIndex, Source, chindex), CalcBitLenght(slow.SlowIDLength, Source, chindex))
                                {
                                    Data = new Byte[] { (Byte)slow.SlowID },
                                    BitCount = (UInt32)slow.SlowIDCount,
                                };
                                info.EventInofs[0] = (id.Data, id.BitCount);
                                datapackets.Add(id);
                                endindex = (Int32)slow.SlowIDIndex + (Int32)slow.SlowIDLength;
                            }

                            if (slow.SlowDataLength > 0)
                            {
                                SENTSlowDataDecodePacket data = new SENTSlowDataDecodePacket(CalcPosition(slow.SlowDataIndex, Source, chindex), CalcBitLenght(slow.SlowDataLength, Source, chindex))
                                {
                                    Data = slow.SlowData,
                                    BitCount = (UInt32)slow.SlowDataCount,
                                };
                                info.EventInofs[1] = (data.Data, data.BitCount);
                                datapackets.Add(data);
                                endindex = (Int32)slow.SlowDataIndex + (Int32)slow.SlowDataLength;
                            }

                            if (slow.SlowCRCLength > 0)
                            {
                                SENTSlowCRCDecodePacket crc = new SENTSlowCRCDecodePacket(CalcPosition(slow.SlowCRCIndex, Source, chindex), CalcBitLenght(slow.SlowCRCLength, Source, chindex))
                                {
                                    Data = new Byte[] { (Byte)slow.SlowCRC },
                                    BitCount = (UInt32)slow.SlowCRCCount,
                                    HasCRCError = slow.SlowCRCError,
                                    ErrorInfoData= new Byte[] { (Byte)slow.SlowCorrectCRC },
                                    ErrorInfoBitCount = (UInt32)slow.SlowCorrectCRCCount,
                                };
                                info.EventInofs[2] = (crc.Data, crc.BitCount);
                                if (crc.HasCRCError)
                                {
                                    info.EventInofs[3] = (Encoding.Default.GetBytes(crc.ErrorInfo),0);
                                    info.ExtraInfos.Add((3, crc.ErrorInfoData, crc.ErrorInfoBitCount));
                                }
                                datapackets.Add(crc);
                                if (endindex < (Int32)slow.SlowCRCIndex + (Int32)slow.SlowCRCLength)
                                {
                                    endindex = (Int32)slow.SlowCRCIndex + (Int32)slow.SlowCRCLength;
                                }
                            }
                        }

                        info.EndPosition = CalcPosition(endindex, Source, chindex);
                        info.EndTimeByPs = GetTimeFromPosition(info.EndPosition, chindex);
                        _EventInfos.Add(info);
                        return datapackets;
                    }).ToArray();
                    decodebuffer.Add(_ResultData);
                }
                catch
                {
                }
                ChangeBuffer();

            }
        }


        public Int32 SENTFindNextFallingEdge(Int32 startindex, ComModel.ChannelId ch, ref CancellationToken token, ref Boolean needclear)
        {
            if (Polarity == ProtocolCommon.Polarity.Positive)
            {
                return DecodeDataHelper.Instance.FindNextFallingEdge(BusId, startindex, Source, ref token, ref needclear);
            }
            else
            {
                return DecodeDataHelper.Instance.FindNextRisingEdge(BusId, startindex, Source, ref token, ref needclear);
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
            return new HdMessage.ProtocolSENTOptions()
            {
                ChannelMode = ChannelMode,
                FastChannelMode = FastChannelMode,
                Source = Source,
                DataLength = DataLength,
                PauseBit = PauseBit,
                Polarity = Polarity,
                ClockTick = RealClockTick,
                Tolerance = Tolerance,
                Threshold = _Threshold,
            };
        }

        private Int32 GetStartIndex(Int32 startindex, UInt32 datalen, ref CancellationToken token, ref Boolean needclear)
        {
            if (startindex >= datalen)
                return -1;
            startindex = DecodeDataHelper.Instance.FindNextEdge(BusId, startindex, Source, ref token, ref needclear);
            if (startindex >= datalen)
                return -1;
            return -1;
        }

        private Int32 FindFallingEdgeIndex(Int32 startindex, UInt32 datalen, ref CancellationToken token, ref Boolean needclear)
        {
            if (startindex >= datalen)
                return -1;
            startindex = DecodeDataHelper.Instance.FindNextEdge(BusId, startindex, Source, ref token, ref needclear);
            if (startindex == -1)
                return -1;
            return DecodeDataHelper.Instance.FindNextEdge(BusId, startindex, Source, ref token, ref needclear);
            //if (startindex == -1) return -1;
            //return startindex;
        }

        private Int32 GetDataIndex(UInt32 datalen, Int32 startindex = 0)
        {
            _IsLowThreshold = false;
            Boolean startstatus = true;
            if (startindex >= datalen - 2)
                return -1;
            startindex += 1;
            while (!_IsLowThreshold)
            {
                if (startindex >= datalen - 1)
                    return -1;
                if (GetBit(startindex) == startstatus && GetBit(startindex + 1) != startstatus)
                {
                    _IsLowThreshold = true;
                    return startindex;
                }
                startindex++;
            }
            return -1;
        }

        private Boolean GetBit(Int32 index)
        {
            return DecodeDataHelper.Instance.GetLevel(BusId, index, Threshold, Source, Polarity == ProtocolCommon.Polarity.Positive);
        }


        #region C++解码相关

        private List<SENTPacketInfo> ParsingSent(ref CancellationToken token, ref Boolean needclear)
        {
            Int32 cancel_flag = 0;

            List<SENTPacketInfo> packs = new List<SENTPacketInfo>();
            SentResult result = new SentResult();
            try
            {
                SentOptions options = new SentOptions();

                options.pause_bit = PauseBit == ProtocolSENT.PauseBit.Yes ? Model.SENT.PauseBit.YES : Model.SENT.PauseBit.NO;
                options.signal_polarity = _Polarity == ProtocolCommon.Polarity.Positive ? Model.SENT.Polarity.POSITIVE : Model.SENT.Polarity.NEGTIVE;
                options.clock_cyle = RealClockTick;
                options.tolerance = Tolerance * 1E-2;
                options.channel_mode = ChannelMode;
                options.signal_nibble_count = _DataLength switch
                {
                    ProtocolSENT.DataLength.Nibbles_1 => SentNibbleCount.ONENIBBLE,
                    ProtocolSENT.DataLength.Nibbles_2 => SentNibbleCount.TWONIBBLE,
                    ProtocolSENT.DataLength.Nibbles_3 => SentNibbleCount.THREENIBBLE,
                    ProtocolSENT.DataLength.Nibbles_4 => SentNibbleCount.FOURNIBBLE,
                    ProtocolSENT.DataLength.Nibbles_5 => SentNibbleCount.FIVENIBBLE,
                    ProtocolSENT.DataLength.Nibbles_6 => SentNibbleCount.SIXNIBBLE,
                    _ => throw new NotImplementedException()
                };
                //options.cancel_flag = new IntPtr(cancel_flag);

                UInt32 perChannelDataLength = 0;
                DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, Source, ref perChannelDataLength);

                Double samplerate = 0;
                DecodeDataHelper.Instance.TryGetSampleRate(BusId, Source, ref samplerate);

                TwoLevelEdgeInfo? node = DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, Source, ref token, ref needclear) as TwoLevelEdgeInfo;
                if (node == null)
                {
                    return packs;
                }

                // 边沿脉宽信息获取
                GCHandle edgePulsesHandle;
                IntPtr edgepulsePtr = IntPtr.Zero;
                List<PAM2EdgePulse> _EdgePulsesList = new List<PAM2EdgePulse>();
                DecodeDataHelper.Instance.GetTwoLevelPulses(ref node, ref _EdgePulsesList);
                PAM2EdgePulseSequence.Allocate(ref _EdgePulsesList, perChannelDataLength, samplerate, out edgepulsePtr, out edgePulsesHandle);

                result.EventInfosPtr = IntPtr.Zero;
                result.EventCount = 0;

                DecoderImpl.DecodeSENT(options, edgepulsePtr, out result);

                packs = ConvertIntPtrToSentPackInfos(this.ChannelMode, result);

                edgePulsesHandle.Free();

                Marshal.FreeHGlobal(edgepulsePtr);

                DecoderImpl.FreeSENT(result);
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "DecodeErrorInfo\\data.txt")))
                {
                    sw.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    foreach (Byte b in DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].ChannelDataSource)
                    {
                        sw.WriteLine(b.ToString());
                    }
                }
            }
            finally
            {
            }
            return packs;
        }

        private static List<SENTPacketInfo> ConvertIntPtrToSentPackInfos(ProtocolSENT.ChannelMode channelMode, SentResult results)
        {
            List<SENTPacketInfo> packs = new List<SENTPacketInfo>();

            SentEvent[] events = new SentEvent[results.EventCount];
            Int32 rstsize = Marshal.SizeOf(typeof(SentEvent));
            IntPtr rstptr = results.EventInfosPtr;

            for (UInt64 i = 0; i < results.EventCount; i++)
            {
                IntPtr member = new IntPtr(rstptr.ToInt64() + (Int64)i * rstsize);
                events[i] = (SentEvent)(Marshal.PtrToStructure(member, typeof(SentEvent)) ?? throw new ArgumentException());
            }

            for (UInt32 i = 0; i < results.EventCount; i++)
            {
                Byte[] data = new Byte[events[i].DataCount];
                SentEventField[] fields = new SentEventField[events[i].FieldCount];

                Int32 datasize = Marshal.SizeOf(typeof(Byte));
                IntPtr dataptr = events[i].Data;

                for (UInt32 ii = 0; ii < events[i].DataCount; ii++)
                {
                    IntPtr member = new IntPtr(dataptr.ToInt64() + ii * datasize);
                    data[ii] = (Byte)(Marshal.PtrToStructure(member, typeof(Byte)) ?? throw new ArgumentException());
                }

                Int32 fieldsize = Marshal.SizeOf(typeof(SentEventField));
                IntPtr fieldptr = events[i].Fields;

                for (UInt32 ii = 0; ii < events[i].FieldCount; ii++)
                {
                    IntPtr member = new IntPtr(fieldptr.ToInt64() + ii * fieldsize);
                    fields[ii] = (SentEventField)(Marshal.PtrToStructure(member, typeof(SentEventField)) ?? throw new ArgumentException());
                }

                //装填数据

                SENTFastPaketInfo fast = new SENTFastPaketInfo();
                SENTSlowPaketInfo slow = new SENTSlowPaketInfo();

                List<(Int32 index, Int32 length, Int32 data, Boolean error)> datainfo = new List<(Int32 index, Int32 length, Int32 data, Boolean error)>();

                fast.StartIndex = (Int32)events[i].StartIndex;
                slow.StartIndex = (Int32)events[i].StartIndex;
                datainfo.Clear();

                for (Int32 ii = 0; ii < events[i].FieldCount; ii++)
                {
                    SentDecodeEventType type = (SentDecodeEventType)Enum.Parse(typeof(SentDecodeEventType), fields[ii].Type.ToString());

                    switch (type)
                    {
                        case SentDecodeEventType.SYNC:
                            fast.SyncIndex = (Int32)fields[ii].StartIndex;

                            fast.SyncLen = (Int32)fields[ii].Length;

                            break;
                        case SentDecodeEventType.STATUS:
                            fast.StatusIndex = (Int32)fields[ii].StartIndex;

                            fast.StatusLen = (Int32)fields[ii].Length;

                            fast.Status = new Byte[4];

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                fast.Status[iii] = data![fields[ii].DataIndex + iii];
                            }

                            break;
                        case SentDecodeEventType.DATA:
                            {
                                Int32 index = (Int32)fields[ii].StartIndex;
                                Int32 length = (Int32)fields[ii].Length;
                                Int32 dt = 0x00;

                                for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                                {
                                    dt = (dt <<= 1) |data![fields[ii].DataIndex + iii];
                                }

                                datainfo.Add((index, length, dt, false));
                            }
                            break;
                        case SentDecodeEventType.CRC:
                            fast.FastCRCIndex = (Int32)fields[ii].StartIndex;

                            fast.FastCRCLen = (Int32)fields[ii].Length;
                            fast.FastCRC = 0x00;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                fast.FastCRC = (fast.FastCRC <<= 1) | data![fields[ii].DataIndex + iii];
                            }

                            fast.FastCRCError = fields[ii].HasError == 1;

                            if (fast.FastCRCError)
                            {
                                fast.FastCorrectCRC = 0x00;
                                for (Int32 iii = 0; iii < fields[ii].ErrorDataLength; iii++)
                                {
                                    fast.FastCRC = (fast.FastCRC <<= 1) | data![fields[ii].ErrorDataIndex + iii];
                                }
                            }

                            break;
                        case SentDecodeEventType.PAUSE:
                            fast.PauseIndex = (Int32)fields[ii].StartIndex;

                            fast.PauseLen = (Int32)fields[ii].Length;

                            fast.Pause = 0x00;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                fast.Pause <<= 1;
                                fast.Pause += data![fields[ii].DataIndex + iii];
                            }
                            break;
                        case SentDecodeEventType.NONE:
                            fast.FastErrorIndex = (Int32)fields[ii].StartIndex;

                            fast.ErrorLen = (Int32)fields[ii].Length;

                            fast.FastError = 0x00;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                fast.FastError += data![fields[ii].DataIndex + iii];
                            }

                            break;
                        case SentDecodeEventType.SLOWID:
                            slow.SlowIDIndex = (Int32)fields[ii].StartIndex;

                            slow.SlowIDLength = (Int32)fields[ii].Length;
                            slow.SlowIDCount = fields[ii].DataLength;
                            slow.SlowID = 0x00;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                slow.SlowID = (slow.SlowID <<= 1) |data![fields[ii].DataIndex + iii];
                            }

                            break;
                        case SentDecodeEventType.SLOWDATA:
                            slow.SlowDataIndex = (Int32)fields[ii].StartIndex;
                            slow.SlowDataLength = (Int32)fields[ii].Length;
                            slow.SlowDataCount = (fields[ii].DataLength / 8) + (fields[ii].DataLength % 8 == 0 ? 0 : 1);
                            slow.SlowData = new Byte[slow.SlowDataCount];

                            //补零
                            var padzero = slow.SlowDataCount * 8 - fields[ii].DataLength;
                            for (int iii = 0; iii < padzero; iii++)
                            {
                                slow.SlowData[0] <<= 1;
                            }
                            //填数
                            for (int iii = padzero; iii < 8; iii++)
                            {
                                slow.SlowData[0] <<= 1;
                                slow.SlowData[0] += data![fields[ii].DataIndex + iii - padzero];
                            }

                            for (Int32 iii = 1; iii < slow.SlowDataCount; iii++)
                            {
                                for (Int32 iiii = 0; iiii < 8; iiii++)
                                {
                                    slow.SlowData[iii] <<= 1;
                                    slow.SlowData[iii] += data![fields[ii].DataIndex + iii * 8 + iiii - padzero];
                                }
                            }
                            slow.SlowDataCount *= 8;
                            break;
                        case SentDecodeEventType.SLOWCRC:
                            slow.SlowCRCIndex = (Int32)fields[ii].StartIndex;

                            slow.SlowCRCLength = (Int32)fields[ii].Length;

                            slow.SlowCRC = 0x00;
                            slow.SlowCRCCount = fields[ii].DataLength;
                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                slow.SlowCRC = (slow.SlowCRC <<= 1) |data![fields[ii].DataIndex + iii];
                            }

                            slow.SlowCRCError = fields[ii].HasError == 1;

                            if(slow.SlowCRCError)
                            {
                                slow.SlowCorrectCRCCount = fields[ii].ErrorDataLength;
                                slow.SlowCorrectCRC = 0x00;
                                for (Int32 iii = 0; iii < fields[ii].ErrorDataLength; iii++)
                                {
                                    slow.SlowCorrectCRC = (slow.SlowCorrectCRC <<= 1) | data![fields[ii].ErrorDataIndex + iii];
                                }

                            }
                            break;
                    }
                }

                if (channelMode == ProtocolSENT.ChannelMode.FastChannel)
                {
                    fast.FastData = new Int32[datainfo.Count];
                    fast.FastDataIndex = new Int32[datainfo.Count];
                    fast.FastDataLen = new Int32[datainfo.Count];
                    for (Int32 ii = 0; ii < datainfo.Count; ii++)
                    {
                        fast.FastDataIndex[ii] = datainfo[ii].index;
                        fast.FastDataLen[ii] = datainfo[ii].length;
                        fast.FastData[ii] = datainfo[ii].data;
                    }
                    packs.Add(fast);
                }
                else
                {
                    packs.Add(slow);
                }

            }

            return packs;
        }

        #endregion
    }
}

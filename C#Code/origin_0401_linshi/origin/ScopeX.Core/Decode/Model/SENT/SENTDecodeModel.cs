using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.Core.Decode.Model.SENT;
using ScopeX.Hardware.Driver;
using static ScopeX.ComModel.ProtocolSENT;
using static ScopeX.Core.Decode.EdgeInfoCPP;
using static ScopeX.Core.Decode.Model.LIN.LinDecodeStruct;

namespace ScopeX.Core.Decode
{
    [Obsolete("Please Use Class 'SENTDecodeModelCPP'", true)]
    internal class SENTDecodeModel : ProtocolModel
    {
        private const Int32 _SENTSYNCLEN = 56;
        private List<SENTPacketInfo> _PacketInfos = new List<SENTPacketInfo>();
        private DecodeResultData _ResultData = new DecodeResultData();
        public SENTDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id, ComModel.SerialProtocolType.SENT, isTrigDecode)
        {
            _ResultData.Name = "SENT";
        }

        public override Double BitRateByPs => 1f / this.RealClockTick * 1E+12;


        public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>()
        {
            "Index",
            "Start Time",
            "Status&Com",
            "Data",
            "CRC",
            "Pause",
            "Error",
        };


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
            set { UpdateProperty(ref _FastChannelMode, value); }
        }

        private ProtocolSENT.DataLength _DataLength = ProtocolSENT.DataLength.Nibbles_6;

        public ProtocolSENT.DataLength DataLength
        {
            get { return _DataLength; }
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
                    UpdateProperty(ref _RealClockTick, value);
                }
            }
        }
        public Double MinClockTick => 0.5E-6;

        private Double _Tolerance = 0.15;

        public Double Tolerance
        {
            get { return _Tolerance; }
            set { UpdateProperty(ref _Tolerance, value); }
        }

        public Double MaxClockTick => 300E-6;
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

        struct SENTPacketInfo
        {
            public Int32 StartIndex;
            public Int32 SyncIndex;
            public Int32 StatusIndex;
            public Int32[] DataIndex;
            public Int32 CRCIndex;
            public Int32 PauseIndex;
            public Int32 ErrorIndex;
            public Int32 SyncLen;
            public Int32 StatusLen;
            public Int32[] DataLen;
            public Int32 CRCLen;
            public Int32 PauseLen;
            public Int32 ErrorLen;
            public Int32 Status;
            public Int32[] Data;
            public Int32 CRC;
            public Int32 Pause;
            public Boolean PauseError;
            public Int32 Error;
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

        internal override Boolean CheckUpdate(ref Int64 lastStamp)
        {
            //if (Source.IsAnalog())
            //{
            //    return laststamp != DecodeDataHelper.Instance.AnalogDataSource.TimeStamp;
            //}
            //if (Source.IsReference())
            //{
            //    return laststamp != DecodeDataHelper.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].TimeStamp;
            //}

            if (Source.IsAnalog() && lastStamp != DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp)
            {
                lastStamp = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp;
                return true;
            }
            if (Source.IsReference() && lastStamp != DecodeDataHelper.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].TimeStamp)
            {
                lastStamp = DecodeDataHelper.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].TimeStamp;
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
                    _PacketInfos.Clear();
                    _NeedDecodeData = false;
                    List<SENTPacketInfo> packs = new List<SENTPacketInfo>();

                    Int32 startindex = 0;
                    Double scale = (Double)samplerate / RealClockTick;
                    Double syncindexlen = scale * _SENTSYNCLEN;
                    Boolean findstartsync = false;
                    while (true)
                    {

                        var syncstart = 0;
                        var syncstop = 0;
                        //var starttime = DateTime.Now;
                        var starttime = TimeSpanUtility.GetTimestampSpan();
                        while (true)
                        {
                            syncstart = startindex;
                            if (!findstartsync)
                            {
                                syncstart = SENTFindNextFallingEdge(startindex, Source, ref token, ref needclear);
                                if (syncstart == -1)
                                {
                                    startindex = -1;
                                    break;
                                }
                            }

                            syncstop = SENTFindNextFallingEdge(syncstart + 1, Source, ref token, ref needclear);
                            if (syncstop == -1)
                            {
                                startindex = -1;
                                break;
                            }
                            var syncfindlen = syncstop - syncstart;
                            startindex = syncstop;
                            if ((syncfindlen > (syncindexlen - scale)) && (syncfindlen < (syncindexlen + scale)))
                            {
                                findstartsync = true;
                                startindex = syncstop;
                                break;

                            }
                            if (( /*DateTime.Now*/TimeSpanUtility.GetTimestampSpan() - starttime).TotalMilliseconds > 2000)
                            {
                                return;
                            }
                        }

                        if (startindex == -1)
                            break;
                        if (startindex >= datalen)
                        {
                            startindex = (Int32)datalen;
                            break;
                        }
                        SENTPacketInfo packet = new SENTPacketInfo();
                        packet.StartIndex = syncstart;
                        packet.SyncIndex = syncstart;

                        packet.SyncLen = startindex - packet.SyncIndex;
                        //if (scale != 0)
                        //    scale = (Double)sentcount * SENTSYNCLEN / packet.SyncLen;
                        packet.StatusIndex = startindex;
                        startindex = SENTFindNextFallingEdge(syncstop + 1, Source, ref token, ref needclear); //FindFallingEdgeIndex(startindex, datalen, ref token, ref needclear);//status结束，data数据段起始
                        if (startindex == -1)
                        {
                            _PacketInfos.Add(packet);
                            break;
                        }
                        packet.StatusLen = startindex - packet.StatusIndex;
                        packet.Status = (Int32)Math.Round((packet.StatusLen / scale), MidpointRounding.AwayFromZero) - 12;
                        packet.Data = new Int32[(Int32)DataLength + 1];
                        packet.DataLen = new Int32[(Int32)DataLength + 1];
                        packet.DataIndex = new Int32[(Int32)DataLength + 1];

                        //解析Data
                        for (Int32 i = 0; i < (Int32)DataLength + 1; i++)
                        {
                            packet.DataIndex[i] = startindex;
                            startindex = SENTFindNextFallingEdge(startindex + 1, Source, ref token, ref needclear); //FindFallingEdgeIndex(startindex, datalen, ref token, ref needclear);
                            if (startindex == -1)
                            {
                                //_PacketInfos.Add(packet);
                                break;
                            }
                            packet.DataLen[i] = startindex - packet.DataIndex[i];
                            packet.Data[i] = (Int32)Math.Round((packet.DataLen[i] / scale), MidpointRounding.AwayFromZero) - 12;
                        }
                        if (startindex == -1)
                        {
                            _PacketInfos.Add(packet);
                            break;
                        }
                        //解析CRC

                        packet.CRCIndex = startindex;
                        startindex = SENTFindNextFallingEdge(startindex + 1, Source, ref token, ref needclear); //FindFallingEdgeIndex(startindex, datalen, ref token, ref needclear);
                        if (startindex == -1)
                        {
                            _PacketInfos.Add(packet);
                            break;
                        }
                        packet.CRCLen = startindex - packet.CRCIndex;
                        packet.CRC = (Int32)Math.Round((packet.CRCLen / scale), MidpointRounding.AwayFromZero) - 12;


                        //解析Pause

                        if (_PauseBit == ProtocolSENT.PauseBit.Yes)
                        {
                            var start = startindex;
                            startindex = SENTFindNextFallingEdge(startindex + 1, Source, ref token, ref needclear); //FindFallingEdgeIndex(startindex, datalen, ref token, ref needclear);
                            if (startindex == -1)
                            {
                                _PacketInfos.Add(packet);
                                break;
                            }
                            var cycnum = (Int32)Math.Round(((startindex - start) / scale), MidpointRounding.AwayFromZero);
                            if (cycnum < 11 || cycnum > 769)//Pause在12-758个时钟周期
                            {
                                _PacketInfos.Add(packet);
                                break;
                            }
                            packet.PauseIndex = start;
                            packet.PauseLen = startindex - packet.PauseIndex;
                            packet.Pause = (Int32)Math.Round((packet.PauseLen / scale), MidpointRounding.AwayFromZero);

                        }
                        else
                        {
                            var errorindex = DecodeDataHelper.Instance.FindNextFallingEdge(BusId, startindex + 1, Source, ref token, ref needclear); //FindFallingEdgeIndex(startindex, datalen, ref token, ref needclear);
                            if (errorindex == -1)
                            {
                                _PacketInfos.Add(packet);
                                break;
                            }
                            Int32 count = (Int32)Math.Round(((errorindex - startindex) / scale), MidpointRounding.AwayFromZero);
                            if (count < 11 || count > 769)//Pause在12-758个时钟周期
                            {
                                _PacketInfos.Add(packet);
                                break;
                            }
                            if (count != _SENTSYNCLEN)
                            {
                                packet.ErrorIndex = startindex;
                                packet.ErrorLen = errorindex - startindex;
                                packet.Error = count;
                                startindex = errorindex;
                            }
                        }
                        packs.Add(packet);
                    }
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
                        ProtocolEventInfo info = new ProtocolEventInfo();
                        info.Index = _EventInfos.Count;
                        info.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                        List<SENTDecodePacket> datapackets = new List<SENTDecodePacket>();
                        if (x.SyncLen > 0)
                        {
                            SENTSyncDecodePacket sync = new SENTSyncDecodePacket(CalcPosition(x.SyncIndex, Source, chindex), CalcBitLenght(x.SyncLen, Source, chindex));
                            datapackets.Add(sync);
                            info.StartTimeByPs = GetTimeFromPosition(sync.Start, chindex);
                        }
                        if (x.StatusLen > 0)
                        {
                            SENTStatusDecodePacket status = new SENTStatusDecodePacket(CalcPosition(x.StatusIndex, Source, chindex), CalcBitLenght(x.StatusLen, Source, chindex))
                            {
                                Data = new Byte[] { (Byte)x.Status },
                                BitCount = 4,
                            };
                            info.EventInofs[0] = (status.Data, status.BitCount);
                            datapackets.Add(status);
                        }

                        if (x.Data != null && x.Data.Length > 0)
                        {
                            Byte[] databytes = new Byte[x.Data.Length];
                            for (Int32 i = 0; i < x.Data.Length; i++)
                            {
                                SENTDataDecodePacket data = new SENTDataDecodePacket(CalcPosition(x.DataIndex[i], Source, chindex), CalcBitLenght(x.DataLen[i], Source, chindex))
                                {
                                    Data = new Byte[] { (Byte)x.Data[i] },
                                    BitCount = 4,
                                };
                                databytes[i] = (Byte)x.Data[i];
                                //info.EventInofs[i + 1] = (data.Data, data.BitCount);
                                datapackets.Add(data);
                            }
                            info.EventInofs[1] = (databytes, (UInt32)x.Data.Length * 8);
                        }
                        if (x.CRCLen > 0)
                        {
                            SENTCRCDecodePacket crc = new SENTCRCDecodePacket(CalcPosition(x.CRCIndex, Source, chindex), CalcBitLenght(x.CRCLen, Source, chindex))
                            {
                                Data = new Byte[] { (Byte)x.CRC },
                                BitCount = 4,
                            };
                            info.EventInofs[2] = (crc.Data, crc.BitCount);
                            datapackets.Add(crc);
                        }
                        if (x.PauseLen > 0)
                        {
                            SENTPauseDecodePacket pause = new SENTPauseDecodePacket(CalcPosition(x.PauseIndex, Source, chindex), CalcBitLenght(x.PauseLen, Source, chindex))
                            {
                                Data = new Byte[] { (Byte)x.Pause },
                                BitCount = 8,
                            };
                            info.EventInofs[3] = (pause.Data, pause.BitCount);
                            datapackets.Add(pause);
                        }
                        if (x.Error > 0)
                        {
                            SENTErrorDecodePacket error = new SENTErrorDecodePacket(CalcPosition(x.ErrorIndex, Source, chindex), CalcBitLenght(x.ErrorLen, Source, chindex))
                            {
                                Data = new Byte[] { (Byte)x.Error },
                                BitCount = 8,
                            };
                            info.EventInofs[4] = (error.Data, error.BitCount);
                            datapackets.Add(error);
                        }

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


        public Int32 SENTFindNextFallingEdge(Int32 startIndex, ComModel.ChannelId ch, ref CancellationToken token, ref Boolean needClear)
        {
            if (Polarity == ProtocolCommon.Polarity.Positive)
            {
                return DecodeDataHelper.Instance.FindNextFallingEdge(BusId, startIndex, Source, ref token, ref needClear);
            }
            else
            {
                return DecodeDataHelper.Instance.FindNextRisingEdge(BusId, startIndex, Source, ref token, ref needClear);
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
                Threshold = _Threshold,
            };
        }

        private Int32 GetStartIndex(Int32 startIndex, UInt32 dataLen, ref CancellationToken token, ref Boolean needClear)
        {
            if (startIndex >= dataLen)
                return -1;
            startIndex = DecodeDataHelper.Instance.FindNextEdge(BusId, startIndex, Source, ref token, ref needClear);
            if (startIndex >= dataLen)
                return -1;
            return -1;
        }

        private Int32 FindFallingEdgeIndex(Int32 startIndex, UInt32 dataLen, ref CancellationToken token, ref Boolean needClear)
        {
            if (startIndex >= dataLen)
                return -1;
            startIndex = DecodeDataHelper.Instance.FindNextEdge(BusId, startIndex, Source, ref token, ref needClear);
            if (startIndex == -1)
                return -1;
            return DecodeDataHelper.Instance.FindNextEdge(BusId, startIndex, Source, ref token, ref needClear);
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

        private List<SENTPacketInfo> ParsingSent(ref CancellationToken token, ref Boolean needClear)
        {
            Int32 cancel_flag = 0;

            List<SENTPacketInfo> packs = new List<SENTPacketInfo>();
            SentResult result = new SentResult();
            try
            {
                SentOptions options = new SentOptions();

                options.pause_bit = PauseBit == ProtocolSENT.PauseBit.Yes ? Model.SENT.PauseBit.YES : Model.SENT.PauseBit.NO;
                options.signal_polarity = _Polarity == ProtocolCommon.Polarity.Positive ? Model.SENT.Polarity.POSITIVE : Model.SENT.Polarity.NEGTIVE;
                options.clock_cyle = (Double)((decimal)1E6 / (decimal)RealClockTick);
                options.tolerance = 0.2;
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

                TwoLevelEdgeInfo? node = DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, Source, ref token, ref needClear) as TwoLevelEdgeInfo;
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

                packs = ConvertIntPtrToSentPackInfos(result);

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

        private static List<SENTPacketInfo> ConvertIntPtrToSentPackInfos(SentResult results)
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

                SENTPacketInfo pack = new SENTPacketInfo();

                List<(Int32 index, Int32 length, Int32 data, Boolean error)> datainfo = new List<(Int32 index, Int32 length, Int32 data, Boolean error)>();

                pack.StartIndex = (Int32)events[i].StartIndex;
                datainfo.Clear();

                for (Int32 ii = 0; ii < events[i].FieldCount; ii++)
                {
                    SentDecodeEventType type = (SentDecodeEventType)Enum.Parse(typeof(SentDecodeEventType), fields[ii].Type.ToString());

                    switch (type)
                    {
                        case SentDecodeEventType.SYNC:
                            pack.SyncIndex = (Int32)fields[ii].StartIndex;

                            pack.SyncLen = (Int32)fields[ii].Length;

                            break;
                        case SentDecodeEventType.STATUS:
                            pack.StatusIndex = (Int32)fields[ii].StartIndex;

                            pack.StatusLen = (Int32)fields[ii].Length;

                            pack.Status = 0x00;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                pack.Status <<= 1;
                                pack.Status += data![fields[ii].DataIndex + iii];
                            }

                            break;
                        case SentDecodeEventType.DATA:
                            Int32 index = (Int32)fields[ii].StartIndex;
                            Int32 length = (Int32)fields[ii].Length;
                            Int32 dt = 0x00;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                dt <<= 1;
                                dt += data![fields[ii].DataIndex + iii];
                            }

                            datainfo.Add((index, length, dt, false));
                            break;
                        case SentDecodeEventType.CRC:
                            pack.CRCIndex = (Int32)fields[ii].StartIndex;

                            pack.CRCLen = (Int32)fields[ii].Length;

                            pack.CRC = 0x00;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                pack.CRC <<= 1;
                                pack.CRC += data![fields[ii].DataIndex + iii];
                            }

                            break;
                        case SentDecodeEventType.PAUSE:
                            pack.PauseIndex = (Int32)fields[ii].StartIndex;

                            pack.PauseLen = (Int32)fields[ii].Length;

                            pack.Pause = 0x00;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                pack.Pause += data![fields[ii].DataIndex + iii];
                            }

                            break;
                        case SentDecodeEventType.NONE:
                            pack.ErrorIndex = (Int32)fields[ii].StartIndex;

                            pack.ErrorLen = (Int32)fields[ii].Length;

                            pack.Error = 0x00;

                            for (Int32 iii = 0; iii < fields[ii].DataLength; iii++)
                            {
                                pack.Error += data![fields[ii].DataIndex + iii];
                            }

                            break;      
                    }
                }

                pack.Data = new Int32[datainfo.Count];
                pack.DataIndex = new Int32[datainfo.Count];
                pack.DataLen = new Int32[datainfo.Count];
                for (Int32 ii = 0; ii < datainfo.Count; ii++)
                {
                    pack.DataIndex[ii] = datainfo[ii].index;
                    pack.DataLen[ii] = datainfo[ii].length;
                    pack.Data[ii] = datainfo[ii].data;
                }

                packs.Add(pack);
            }

            return packs;
        }

        #endregion
    }
}

//#define USE_CPP_DECODE_LIN

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
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
using static ScopeX.Core.Decode.EdgeInfoCPP;
using static ScopeX.Core.Decode.Model.LIN.LinDecodeStruct;



namespace ScopeX.Core.Decode
{
    internal sealed class LINDecodeModelCPP :ProtocolModel
    {
        private List<LINPacketInfo> _PacketInfos = new List<LINPacketInfo>();
        private DecodeResultData _ResultData = new DecodeResultData();

        private List<PAM2EdgePulse> _EdgePulsesList = new List<PAM2EdgePulse>();

        public LINDecodeModelCPP(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.LIN, isTrigDecode)
        {
            _ResultData.Name = "LIN";
            //Uuid=Guid.NewGuid().ToString();
        }

        // private String Uuid;

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

        private Boolean _ParityWithId;

        public Boolean ParityWithId
        {
            get { return _ParityWithId; }
            set { UpdateProperty(ref _ParityWithId, value); }
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
            set { UpdateProperty(ref _Threshold, value / TryGetChannelGain(Source)); }
        }
        public String Unit => GetChannelUnit(Source);

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
            set { UpdateProperty(ref _DataCount, value); }
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

        //采样点(%)
        private Int32 _SamplePoint = 70;
        /// <summary>
        /// 采样点(%)
        /// </summary>
        public Int32 SamplePoint
        {
            get { return _SamplePoint; }
            set { UpdateProperty(ref _SamplePoint, value); }
        }
        public Int32 MinSamplePoint => 50;

        public Int32 MaxSamplePoint => 90;


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

            if (Source.IsReference() && prsnt.VuDatabase != null && prsnt.VuDatabase.Current != null)
            {
                return DecodeDataHelper.ReferenceHasData(Source, Threshold);
            }

            if (Source.IsAnalog())
            {
                return DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }

            return false;
        }

        internal override List<ChannelId> GetDecodeSources()
        {
            return new List<ChannelId>() { Source};
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
            var result = false;
            if (Source.IsAnalog() && laststamp != DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp;
                result = true;
            }
            if (Source.IsReference() && laststamp != DecodeDataHelper.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].TimeStamp;
                result = true;
            }

            //var sources = GetDecodeSources();
            //foreach (var id in sources)
            //{
            //    if (!DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id, out var prsnt) || prsnt != null || !prsnt.Active)
            //    {
            //        result = true;
            //        break;
            //    }
            //}

            return result;
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
                    _NeedUpdateViewInfo = true;
                    _PacketInfos.Clear();
                    List<LINPacketInfo> packet = new List<LINPacketInfo>();

                    TwoLevelEdgeInfo? node = DecodeDataHelper.Instance.GetEdgeInfo(BusId, startindex: 0, Source, ref token, ref needclear) as TwoLevelEdgeInfo;

                    IntPtr edgepulseptr = IntPtr.Zero;

                    GCHandle pulseshandle;

                    _EdgePulsesList.Clear();

                    if (node != null)
                    {
                        DecodeDataHelper.Instance.GetTwoLevelPulses(ref node, ref _EdgePulsesList);
                        PAM2EdgePulseSequence.Allocate(ref _EdgePulsesList, (UInt64)datalen, samplerate, out edgepulseptr, out pulseshandle);

                        LinOption options = new LinOption()
                        {
                            LinSignalVersion = this._Standard,
                            LinSyncPolarity = this._Polarity,
                            SignalRate = this.CustomBPS,
                            SamplePoint = Convert.ToDouble(SamplePoint) / 100.0,
                        };
                        LinResult decoderesult = new LinResult();
                        decoderesult.EventCount = 0;
                        decoderesult.LinEvent = IntPtr.Zero;

                        if (DecoderImpl.DecodeLIN(options, edgepulseptr, out decoderesult))
                        {
                            Int32 eventsize = Marshal.SizeOf(typeof(LinEvent));
                            Int32 bytefieldsize = Marshal.SizeOf(typeof(ByteField));

                            PAM2EdgePulseSequence.Free(ref edgepulseptr, ref pulseshandle);

                            for (Int32 i = 0; i < decoderesult.EventCount; ++i)
                            {
                                LinEvent linevent = (LinEvent)Marshal.PtrToStructure(decoderesult.LinEvent + i * eventsize, typeof(LinEvent));

                                LINPacketInfo info = new LINPacketInfo();

                                List<Int32> data = new List<Int32>();
                                List<Int32> dataindex = new List<Int32>();
                                List<Int32> datalenth = new List<Int32>();
                                List<Boolean> dataerror = new List<Boolean>();

                                info.StartIndex = Convert.ToInt32(linevent.EventStartIndex);

                                info.Crc = linevent.CheckSum;

                                for (Int32 byteindex = 0; byteindex < linevent.ByteFieldCount; ++byteindex)
                                {
                                    ByteField bytefield = (ByteField)Marshal.PtrToStructure(linevent.ByteField + byteindex * bytefieldsize, typeof(ByteField));

                                    // 是同步段
                                    switch (bytefield.ByteFieldType)
                                    {
                                        case ByteFieldType.BYTEFIELD_SYNC:
                                            {
                                                info.SyncIndex = Convert.ToInt32(bytefield.FieldStartIndex);
                                                info.SyncLen = Convert.ToInt32(bytefield.FieldEndIndex - bytefield.FieldStartIndex);
                                                info.Sync = bytefield.Value;
                                                info.SyncError = !Convert.ToBoolean(bytefield.FieldCheckResult);
                                                break;
                                            }
                                        case ByteFieldType.BYTEFIELD_PID:
                                            {
                                                info.PIDIndex = Convert.ToInt32(bytefield.FieldStartIndex);
                                                info.PIDLen = Convert.ToInt32(bytefield.FieldEndIndex - bytefield.FieldStartIndex);
                                                info.PID = bytefield.Value;
                                                info.PIDParityError = !Convert.ToBoolean(bytefield.FieldCheckResult);
                                                break;
                                            }
                                        case ByteFieldType.BYTEFIELD_DATA:
                                            {
                                                data.Add(bytefield.Value);
                                                dataindex.Add(Convert.ToInt32(bytefield.FieldStartIndex));
                                                datalenth.Add(Convert.ToInt32(bytefield.FieldEndIndex - bytefield.FieldStartIndex));
                                                dataerror.Add(false);

                                                break;
                                            }
                                        case ByteFieldType.BYTEFIELD_CHECKSUM:
                                            {
                                                info.ChecksumIndex = Convert.ToInt32(bytefield.FieldStartIndex);
                                                info.ChecksumLen = Convert.ToInt32(bytefield.FieldEndIndex - bytefield.FieldStartIndex);
                                                info.Checksum = bytefield.Value;
                                                info.ChecksumError = !Convert.ToBoolean(bytefield.FieldCheckResult);
                                                break;
                                            }
                                        default:
                                            break;
                                    }
                                }
                                info.DataIndex = dataindex.ToArray();
                                info.DataLen = datalenth.ToArray();
                                info.Data = data.ToArray();
                                info.DataError = dataerror.ToArray();

                                packet.Add(info);

                            }

                            DecoderImpl.FreeLIN(ref decoderesult);
                        }

                        _PacketInfos.AddRange(packet);
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
                        List<LINDecodePacket> packets = new List<LINDecodePacket>();
                        LINStartDecodePacket start = new LINStartDecodePacket(CalcPosition(x.StartIndex, Source, chindex), CalcBitLenght(10, Source, chindex));
                        packets.Add(start);

                        if (x.StartIndex != -1)
                        {
                            eventinfo.StartTimeByPs = GetTimeFromPosition(start.Start, chindex);
                            eventinfo.StartPosition = start.Start;
                            endindex = (Int32)x.StartIndex;
                        }

                        LINSyncDecodePacket sync = new LINSyncDecodePacket(CalcPosition(x.SyncIndex, Source, chindex), CalcBitLenght(x.SyncLen, Source, chindex))
                        {
                            Data = new Byte[] { (Byte)x.Sync },
                            BitCount = 8,
                            SyncError = x.SyncError
                        };
                        packets.Add(sync);
                        endindex = (Int32)x.SyncIndex + (Int32)x.SyncLen;
                        if (x.PIDLen != 0)
                        {
                            string pidstr = "Pid:";

                            pidstr += (x.PID & 0xFC).ToString("x2");

                            if (PIncludeOddEven == ProtocolLIN.PIncludeOddEven.Y)
                            {
                                // pidstr = $"pid:{(x.PID & 0xFC)} parity:{x.PID & 0x03}";

                                pidstr += " Parity:" + (x.PID & 0x03).ToString("x2");
                            }


                            // 有问题
                            LINDecodePacket pid = new LINPIDDecodePacket(CalcPosition(x.PIDIndex, Source, chindex), CalcBitLenght(x.PIDLen, Source, chindex))
                            {
                                
                                Data = new Byte[0],
                                BitCount = 0,
                                PIDParityError = x.PIDParityError,
                                Title = pidstr,
                            };
                            byte[] pid_byte = new byte[1];
                            pid_byte[0] = (byte)(x.PID & 0xFC);
                            eventinfo.EventInofs[0] = (pid_byte, 8);

                            packets.Add(pid);
                            endindex = (Int32)x.PIDIndex + (Int32)x.PIDLen;
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
                                endindex = (Int32)x.DataIndex[i];
                                if (x.DataLen[i]!=0)
                                {
                                    endindex = (Int32)x.DataIndex[i] + (Int32)x.DataLen[i];
                                }
                            }
                            eventinfo.EventInofs[1] = (databytes, (UInt32)x.Data.Length * 8);
                        }

                        if (x.ChecksumLen != 0)
                        {
                            LINChecksumDecodePacket checksum = new LINChecksumDecodePacket(CalcPosition(x.ChecksumIndex, Source, chindex), CalcBitLenght(x.ChecksumLen, Source, chindex))
                            {
                                Data = new Byte[] { (Byte)x.Checksum },
                                //CRC=new Byte[] { (Byte)x.Crc},
                                ChecksumError = x.ChecksumError,
                                BitCount = 8,
                            };
                            eventinfo.EventInofs[2] = (checksum.Data, checksum.BitCount);
                            packets.Add(checksum);
                            endindex = (Int32)x.ChecksumIndex + (Int32)x.ChecksumLen;
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
                        eventinfo.EndPosition = CalcPosition(endindex, Source, chindex);
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


        #region C++解码相关
#if USE_CPP_DECODE_LIN
        [DllImport("Decode\\ProtocolDecoder.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Boolean ParseLin(ref ProtocolOptionsLIN options,ref PAM2EdgePulseSequence edges,
      out DecodeResultPacks results);

        [DllImport("Decode\\ProtocolDecoder.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void ReleaseLinResource(ref DecodeResultPacks results); 

        private List<LINPacketInfo> ParsingLin(ref CancellationToken token, ref Boolean needclear)
        {
            Int32 cancel_flag = 0;

            List<LINPacketInfo> packs = new List<LINPacketInfo>();
            DecodeResultPacks resultpacks = new DecodeResultPacks();
            var handlLocal = new GCHandle();

            try
            {
                ProtocolOptionsLIN options = new ProtocolOptionsLIN();

                options.signal_standard = _Standard == ProtocolLIN.Standard.V1 ? LinSignalStandard.LIN_SGNAL_STANDARD_V1 : LinSignalStandard.LIN_SGNAL_STANDARD_V2;
                options.baud_rate = (UInt32)CustomBPS;
                options.signal_bitcount = _DataCount switch
                {
                    1 => LinSignalBitCount.ONEBIT,
                    2 => LinSignalBitCount.TWOBIT,
                    3 => LinSignalBitCount.THREEBIT,
                    4 => LinSignalBitCount.FOURBIT,
                    5 => LinSignalBitCount.FIVEBIT,
                    6 => LinSignalBitCount.SIXBIT,
                    7 => LinSignalBitCount.SEVENBIT,
                    8 => LinSignalBitCount.EIGHTBIT,
                    _ => throw new NotImplementedException()
                };
                options.signal_polarity = _Polarity== ProtocolCommon.Polarity.Positive? Model.LIN.LinDecodeStruct.Polarity.POSITIVE: Model.LIN.LinDecodeStruct.Polarity.NEGTIVE;
                options.checkpidcrc_flag = _CheckPIDParity;
                options.cancel_flag = new IntPtr(cancel_flag);

                UInt32 perChannelDataLength = 0;
                DecodeDataHelper.Instance.TryGetPerChannelDataLength(Source, ref perChannelDataLength);

                Double samlerate = 0;
                DecodeDataHelper.Instance.TryGetSampleRate(Source, ref samlerate);

                TwoLevelEdgeInfo? node = DecodeDataHelper.Instance.GetEdgeInfo(0, Source,
                       ref token, ref needclear)?.Child as TwoLevelEdgeInfo;
                if (node == null)
                {
                    return packs;
                }

                List<PAM2EdgePulse> edgePulse = new List<PAM2EdgePulse>();
                var rst = DecodeDataHelper.GetPAM2EdgePulseSequence(node, perChannelDataLength, ref edgePulse, samlerate, out PAM2EdgePulseSequence edgedata);
                if (!rst)
                {
                    return packs;
                }

                PAM2EdgePulse[] edgesarr;

                if (edgePulse != null && edgePulse.Count > 0)
                {
                    edgesarr = edgePulse.ToArray();
                    handlLocal = GCHandle.Alloc(edgesarr, GCHandleType.Pinned);
                    edgedata.SetDataPtr(handlLocal.AddrOfPinnedObject());
                }


                resultpacks.decode_result = IntPtr.Zero;
                
                ParseLin(ref options,ref edgedata, out resultpacks);
                packs = ConvertIntPtrToLinPackInfos(resultpacks);
                var flag = false;
                if(flag)
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "DecodeErrorInfo\\data.txt")))
                    {
                        foreach (Byte b in DecodeDataHelper.Instance.AnalogDataSource.ChannelDataSource)
                        {
                            sw.WriteLine(b.ToString());
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "DecodeErrorInfo\\data.txt")))
                {
                    sw.WriteLine(ex.Message+"\n"+ex.StackTrace);
                    foreach (Byte b in DecodeDataHelper.Instance.AnalogDataSource.ChannelDataSource)
                    {
                        sw.WriteLine(b.ToString());
                    }
                }
            }
            finally
            {
                handlLocal.Free();
                ReleaseLinResource(ref resultpacks);
            }
            return packs;
        }

        private static List<LINPacketInfo> ConvertIntPtrToLinPackInfos(DecodeResultPacks results)
        {
            List<LINPacketInfo> packs = new List<LINPacketInfo>();

            DecodeEventPacks[] eventpacks = new DecodeEventPacks[results.decode_result_count];
            Int32 rstsize = Marshal.SizeOf(typeof(DecodeEventPacks));
            IntPtr rstptr = results.decode_result;

            for (UInt64 i = 0;i<results.decode_result_count;i++)
            {
                IntPtr member = new IntPtr(rstptr.ToInt64() + (Int64)i * rstsize);
                eventpacks[i] = (DecodeEventPacks)(Marshal.PtrToStructure(member, typeof(DecodeEventPacks)) ?? throw new ArgumentException());
            }

            for (UInt64 i = 0; i < results.decode_result_count; i++)
            {
                Model.LIN.LinDecodeStruct.DecodeEvent[] events = new Model.LIN.LinDecodeStruct.DecodeEvent[eventpacks[i].decode_event_count]; 
                Model.LIN.LinDecodeStruct.DecodeEventError[] errors = new Model.LIN.LinDecodeStruct.DecodeEventError[eventpacks[i].decode_error_count];

                Int32 eventsize= Marshal.SizeOf(typeof(Model.LIN.LinDecodeStruct.DecodeEvent));
                IntPtr eventptr = eventpacks[i].decode_events_ptr;
                Int32 errorsize= Marshal.SizeOf(typeof(Model.LIN.LinDecodeStruct.DecodeEventError));
                IntPtr errorptr = eventpacks[i].decode_errors_ptr;


                for (UInt64 ii = 0; ii < eventpacks[i].decode_event_count; ii++)
                {
                    IntPtr member = new IntPtr(eventptr.ToInt64() + (Int64)ii * eventsize);
                    events[ii] = (Model.LIN.LinDecodeStruct.DecodeEvent)(Marshal.PtrToStructure(member, typeof(Model.LIN.LinDecodeStruct.DecodeEvent)) ?? throw new ArgumentException());
                }

                for (UInt64 ii = 0; ii < eventpacks[i].decode_error_count; ii++)
                {
                    IntPtr member = new IntPtr(errorptr.ToInt64() + (Int64)ii * errorsize);
                    errors[ii] = (Model.LIN.LinDecodeStruct.DecodeEventError)(Marshal.PtrToStructure(member, typeof(Model.LIN.LinDecodeStruct.DecodeEventError)) ?? throw new ArgumentException());
                }

                LINPacketInfo pack = new LINPacketInfo();

                List<(Int32 index, Int32 length, Int32 data, Boolean error)> datainfo = new List<(Int32 index, Int32 length, Int32 data, Boolean error)>(); 

                pack.StartIndex = (Int32)eventpacks[i].start_index;

                for (UInt64 ii = 0; ii < eventpacks[i].decode_event_count; ii++)
                {
                    LinDecodeEventType type = (LinDecodeEventType)Enum.Parse(typeof(LinDecodeEventType), events[ii].event_type.ToString());

                    Byte[] data = events[ii].data_count != 0 ? ConvertIntPtrToData(events[ii].data, events[ii].data_count):null;

                    switch (type)
                    {
                        case LinDecodeEventType.SYNC:
                            pack.SyncIndex = (Int32)events[ii].start_index;

                            pack.SyncLen = (Int32)events[ii].length;

                            pack.Sync = 0x00;

                            for (Int32 iii = 0; iii < events[ii].data_count; iii++)
                            {
                                pack.Sync += data![iii] << iii;
                            }

                            if(errors!=null&&errors.Length>0)
                            {
                                pack.SyncError = errors.Where(x => x.event_index == events[ii].event_index && x.event_error_type == (Int32)LinDecodeEventErrorType.SYNCERROR).Count() > 0;
                            }

                            break;
                        case LinDecodeEventType.PID:
                            pack.PIDIndex= (Int32)events[ii].start_index;

                            pack.PIDLen = (Int32)events[ii].length;

                            pack.PID = 0x00;

                            for (Int32 iii = 0; iii < events[ii].data_count; iii++)
                            {
                                pack.PID |= data![iii] << iii;
                            }

                            if (errors != null && errors.Length > 0)
                            {
                                pack.PIDParityError = errors.Where(x => x.event_index== events[ii].event_index && x.event_error_type == (Int32)LinDecodeEventErrorType.PIDERROR).Count() > 0;
                            }

                            break;
                        case LinDecodeEventType.DATA:
                            Int32 index= (Int32)events[ii].start_index;
                            Int32 length = (Int32)events[ii].length;
                            Int32 dt = 0x00;
                            Boolean error = false;

                            for (Int32 iii = 0; iii < events[ii].data_count; iii++)
                            {
                                dt |= data![iii] << iii;
                            }

                            if (errors != null && errors.Length > 0)
                            {
                                error = errors.Where(x => x.event_index == events[ii].event_index && x.event_error_type == (Int32)LinDecodeEventErrorType.DATAERROR).Count() > 0;
                            }
                            datainfo.Add((index,length,dt,error));
                            break;
                        case LinDecodeEventType.CHECKSUM:
                            pack.ChecksumIndex = (Int32)events[ii].start_index;

                            pack.ChecksumLen = (Int32)events[ii].length;

                            pack.Checksum = 0x00;

                            for (Int32 iii = 0; iii < events[ii].data_count; iii++)
                            {
                                pack.Checksum |= data![iii] << iii;
                            }

                            if (errors != null && errors.Length > 0)
                            {
                                pack.ChecksumError = errors.Where(x => x.event_index == events[ii].event_index && x.event_error_type == (Int32)LinDecodeEventErrorType.CHECKSUMERROR).Count() > 0;
                            }

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

#endif
        #endregion
    }
}

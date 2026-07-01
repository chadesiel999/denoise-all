using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ScopeX.ComModel;
using static ScopeX.Core.Decode.EdgeInfoCPP;

namespace ScopeX.Core.Decode
{
    internal sealed class CPHYDecodeModelCPP : ProtocolModel
    {
        private List<CPHYDecodeCPP> _Deocders = new List<CPHYDecodeCPP>();
        private DecodeResultData _DecodeResultData = new DecodeResultData();
        //private List<String> _Header = new List<String>() { "Index", "Start Time" };
        private List<CPHYPacketInfo> _PacketInfos = new List<CPHYPacketInfo>();
        private Double[] _ChannelThresholds = new Double[3];

        private List<PAM2EdgePulse> _AbEdgePulsesList = new List<PAM2EdgePulse>();
        private List<PAM2EdgePulse> _BcEdgePulsesList = new List<PAM2EdgePulse>();
        private List<PAM2EdgePulse> _CaEdgePulsesList = new List<PAM2EdgePulse>();


        public CPHYDecodeModelCPP(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.CPHY, isTrigDecode)
        {
            _DecodeResultData.Name = this.ProtocolType.ToString();
            Array.Fill(_ChannelThresholds, 0);
        }

        public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String> {
        "Index",
        "Start Time",
        "Virtual Channel",
        "Data Type",
        "Word Count",
        "Data",
        "CRC LSB MSB",
        "PHCRC",
        "ERROR"
        };

        private ChannelId _A = ChannelId.C1;

        public ChannelId A
        {
            get => _A;
            set
            {
                if (value == _A) return;
                if (GetChIndex(value) == -1) value = ChannelId.C1;
                if (value.IsReference())
                {
                    DecodeDataHelper.UpdaterReferenceData(value, _AThreshold);
                    if (SignalType != ProtocolCPHY.SignalType.Difference) {
                        DecodeDataHelper.UpdaterCPHYDiffData(value, _B, _C, _ChannelThresholds);
                    }
                }
                _NeedDecodeData = true;
                UpdateProperty(ref _A, value);
            }
        }

        private ChannelId _B = ChannelId.C2;

        public ChannelId B
        {
            get => _B;
            set
            {
                if (value == _B) return;
                if (GetChIndex(value) == -1) value = ChannelId.C2;
                if (value.IsReference())
                {
                    DecodeDataHelper.UpdaterReferenceData(value, _BThreshold);
                    if (SignalType != ProtocolCPHY.SignalType.Difference)
                    {
                        DecodeDataHelper.UpdaterCPHYDiffData(_A, value, _C, _ChannelThresholds);
                    }
                }
                _NeedDecodeData = true;
                UpdateProperty(ref _B, value);
            }
        }

        private ChannelId _C = ChannelId.C3;

        public ChannelId C
        {
            get => _C;
            set
            {
                if (value == _C) return;
                if (GetChIndex(value) == -1) value = ChannelId.C3;
                if (value.IsReference())
                {
                    DecodeDataHelper.UpdaterReferenceData(value, _CThreshold);
                    if (SignalType != ProtocolCPHY.SignalType.Difference)
                    {
                        DecodeDataHelper.UpdaterCPHYDiffData(_A, _B, value, _ChannelThresholds);
                    }
                }
                _NeedDecodeData = true;
                UpdateProperty(ref _C, value);
            }
        }

        private ChannelId _LPA = ChannelId.C1;

        public ChannelId LPA
        {
            get { return _LPA; }
            set { UpdateProperty(ref _LPA, value); }
        }
        private ChannelId _LPC = ChannelId.C3;

        public ChannelId LPC
        {
            get { return _LPC; }
            set { UpdateProperty(ref _LPC, value); }
        }

        private Double _AThreshold = 0;

        public Double AThreshold
        {
            get { return _AThreshold * TryGetChannelGain(A); }
            set {
                if (value == _AThreshold) return;
                if (_A.IsReference())
                {
                    DecodeDataHelper.UpdaterReferenceData(_A, value);
                    _ChannelThresholds[0] = value;
                    if (SignalType != ProtocolCPHY.SignalType.Difference)
                    {
                        DecodeDataHelper.UpdaterCPHYDiffData(_A, _B, _C, _ChannelThresholds);
                    }
                }
                _NeedDecodeData = true;
                UpdateProperty(ref _AThreshold, value / TryGetChannelGain(A));
            }
        }

        public String AUnit => GetChannelUnit(A);

        private Double _BThreshold = 0;

        public Double BThreshold
        {
            get { return _BThreshold * TryGetChannelGain(B); }
            set {
                if (value == _BThreshold) return;
                if (_B.IsReference())
                {
                    DecodeDataHelper.UpdaterReferenceData(_B, value);
                    _ChannelThresholds[1] = value;
                    if (SignalType != ProtocolCPHY.SignalType.Difference)
                    {
                        DecodeDataHelper.UpdaterCPHYDiffData(_A, _B, _C, _ChannelThresholds);
                    }
                }
                _NeedDecodeData = true;
                UpdateProperty(ref _BThreshold, value / TryGetChannelGain(B));
            }
        }

        public String BUnit => GetChannelUnit(B);


        private Double _CThreshold = 0;

        public Double CThreshold
        {
            get { return _CThreshold * TryGetChannelGain(C); }
            set {
                if (value == _CThreshold) return;
                if (_C.IsReference())
                {
                    DecodeDataHelper.UpdaterReferenceData(_C, value);
                    _ChannelThresholds[2] = value;
                    if (SignalType != ProtocolCPHY.SignalType.Difference)
                    {
                        DecodeDataHelper.UpdaterCPHYDiffData(_A, _B, _C, _ChannelThresholds);
                    }
                }
                _NeedDecodeData = true;
                UpdateProperty(ref _CThreshold, value / TryGetChannelGain(C)); 
            }
        }

        public String CUnit => GetChannelUnit(C);

        private Double _LPAThreshold = 0.4;

        public Double LPAThreshold
        {
            get { return _LPAThreshold * TryGetChannelGain(A); }
            set { UpdateProperty(ref _LPAThreshold, value / TryGetChannelGain(A)); }
        }
        public String LPAUnit => GetChannelUnit(A);

        private Double _LPCThreshold = 0.4;

        public Double LPCThreshold
        {
            get { return _LPCThreshold * TryGetChannelGain(C); }
            set { UpdateProperty(ref _LPCThreshold, value / TryGetChannelGain(C)); }
        }
        public String LPCUnit => GetChannelUnit(C);

        private ProtocolCPHY.SignalType _SignalType = ProtocolCPHY.SignalType.Single;
        public ProtocolCPHY.SignalType SignalType
        {
            get { return _SignalType; }
            set {
                if (value == _SignalType) return;
                DecodeDataHelper.UpdaterReferenceData(_A, _AThreshold);
                DecodeDataHelper.UpdaterReferenceData(_B, _BThreshold);
                DecodeDataHelper.UpdaterReferenceData(_C, _CThreshold);
                if (value != ProtocolCPHY.SignalType.Difference)
                {
                    DecodeDataHelper.UpdaterCPHYDiffData(_A, _B, _C, _ChannelThresholds);
                }
                _NeedDecodeData = true;
                UpdateProperty(ref _SignalType, value); 
            }
        }

        private ProtocolCPHY.SubType _SubType = ProtocolCPHY.SubType.CSI;
        public ProtocolCPHY.SubType SubType
        {
            get { return _SubType; }
            set { UpdateProperty(ref _SubType, value); }
        }

        private Int64 _BitRate = 50_000_000;
        public Int64 BitRate
        {
            get { return _BitRate; }
            set { UpdateProperty(ref _BitRate, value); }
        }

        private UInt64 _SamplePoints = 2_000_000;
        public UInt64 SamplePoints
        {
            get { return _SamplePoints; }
            set { UpdateProperty(ref _SamplePoints, value); }
        }

        override internal Boolean SourceHasData()
        {
            if (DsoPrsnt.DefaultDsoPrsnt == null)
                return false;

            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(A, out IChnlPrsnt? prsnta);
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(B, out IChnlPrsnt? prsntb);
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(C, out IChnlPrsnt? prsntc);
            if (prsnta == null || prsntb == null || prsntc == null)
                return false;

            Boolean hasadata = false;
            Boolean hasbdata = false;
            Boolean hascdata = false;
            if (A.IsReference() && prsnta.VuDatabase.Current != null)
            {
                hasadata = DecodeDataHelper.ReferenceHasData(A, _AThreshold);
            }
            if (B.IsReference() && prsntb.VuDatabase.Current != null)
            {
                hasbdata = DecodeDataHelper.ReferenceHasData(B, _BThreshold);
            }
            if (C.IsReference() && prsntc.VuDatabase.Current != null)
            {
                hascdata = DecodeDataHelper.ReferenceHasData(C, _CThreshold);
            }

            if (A.IsAnalog())
            {
                hasadata = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }
            if (B.IsAnalog())
            {
                hasbdata = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }
            if (C.IsAnalog())
            {
                hascdata = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }
            return (hasadata && hasbdata && hascdata);

        }

        public override void UpdateReferenceDataStatus()
        {
            //if (_ThresholdChanged)
            //{
            //    _ThresholdChanged = false;
            //    DecodeDataSource.Instance.ReferenceDataSource[_A - ChannelIdExt.MinRChId].HasData = false;
            //    DecodeDataSource.Instance.ReferenceDataSource[_B - ChannelIdExt.MinRChId].HasData = false;
            //    DecodeDataSource.Instance.ReferenceDataSource[_C - ChannelIdExt.MinRChId].HasData = false;
            //}
        }

        internal override void ParsingData(ref CancellationToken token)
        {
            Int32 aindex = GetChIndex(A);
            Int32 bindex = GetChIndex(B);
            Int32 cindex = GetChIndex(C);
            UInt32 datalena = 0;
            UInt32 datalenb = 0;
            UInt32 datalenc = 0;

            Double asamplerate = 0, bsamplerate = 0, csamplerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(BusId,A, ref asamplerate);
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, B, ref bsamplerate);
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, C, ref csamplerate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, A, ref datalena);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, B, ref datalenb);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, C, ref datalenc);

            if (MoreThanStorage() || aindex == -1 || bindex == -1 || cindex == -1 || datalena == 0 || datalenb == 0 || datalenc == 0 || datalena != datalenb || datalena != datalenc)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                if (_DecodeResultData != null) 
                {
                    _DecodeResultData = new DecodeResultData();
                }
                if (_EventInfos != null) {
                    _EventInfos = new List<ProtocolEventInfo>();
                }
                _PacketInfos.Clear();
            }
            Boolean needclear = false;
            CPHYDecodeCPP? decoder = _Deocders.FirstOrDefault(x => x.SubType == SubType);
            try
            {
                if (!_NeedDecodeData)
                {
                    return;
                }
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _PacketInfos.Clear();
                token = new CancellationToken();//测试用
                TwoLevelEdgeInfo? abnode;
                TwoLevelEdgeInfo? bcnode;
                TwoLevelEdgeInfo? canode;
                if (SignalType == ProtocolCPHY.SignalType.Difference)
                {
                    abnode = DecodeDataHelper.Instance.GetEdgeInfo(BusId, startindex: 0, this.A, ref token, ref needclear) as TwoLevelEdgeInfo;
                    bcnode = DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, this.B, ref token, ref needclear) as TwoLevelEdgeInfo;
                    canode = DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, this.C, ref token, ref needclear) as TwoLevelEdgeInfo;
                }
                else
                {
                    abnode = DecodeDataHelper.Instance.GetCPHYDiffEdgeInfo(BusId, startindex: 0, this.A, ref token, ref needclear) as TwoLevelEdgeInfo;
                    bcnode = DecodeDataHelper.Instance.GetCPHYDiffEdgeInfo(BusId, 0, this.B, ref token, ref needclear) as TwoLevelEdgeInfo;
                    canode = DecodeDataHelper.Instance.GetCPHYDiffEdgeInfo(BusId, 0, this.C, ref token, ref needclear) as TwoLevelEdgeInfo;
                }

                if (abnode == null || bcnode == null || canode == null || abnode.Child == null || bcnode.Child == null || canode.Child == null)
                {
                    if (_DecodeResultData != null) 
                    {
                        _DecodeResultData = new DecodeResultData();
                    }
                    if (_EventInfos != null) {
                        _EventInfos = new List<ProtocolEventInfo>();
                    } 
                    return;
                }

                CPHYOptions options = new CPHYOptions()
                {
                    CancelFlag = this._CancelFlagPtr,
                    SignalType = this.SignalType,
                    SubType = this.SubType,
                    BitRate = this.BitRate,
                    SamplePoints = (UInt64)datalena,
                };

                IntPtr abedgepulseptr = IntPtr.Zero;
                IntPtr bcedgepulseptr = IntPtr.Zero;
                IntPtr caedgepulseptr = IntPtr.Zero;

                GCHandle abpulseshandle;
                GCHandle bcpulseshandle;
                GCHandle capulseshandle;

                // 时钟边沿脉宽信息获取                 
                _AbEdgePulsesList.Clear();
                DecodeDataHelper.Instance.GetTwoLevelPulses(ref abnode, ref _AbEdgePulsesList);
                PAM2EdgePulseSequence.Allocate(ref _AbEdgePulsesList, (UInt64)datalena, asamplerate, out abedgepulseptr, out abpulseshandle);

                _BcEdgePulsesList.Clear();
                DecodeDataHelper.Instance.GetTwoLevelPulses(ref bcnode, ref _BcEdgePulsesList);
                PAM2EdgePulseSequence.Allocate(ref _BcEdgePulsesList, (UInt64)datalenb, bsamplerate, out bcedgepulseptr, out bcpulseshandle);

                _CaEdgePulsesList.Clear();
                DecodeDataHelper.Instance.GetTwoLevelPulses(ref canode, ref _CaEdgePulsesList);
                PAM2EdgePulseSequence.Allocate(ref _CaEdgePulsesList, (UInt64)datalenc, csamplerate, out caedgepulseptr, out capulseshandle);

                CPHYResultInfoCPP decoderesult = new CPHYResultInfoCPP();

                decoderesult.ProtocolType = SerialProtocolType.CPHY;
                decoderesult.EventCount = 0;
                decoderesult.CPHYEventPtr = IntPtr.Zero;

                Boolean parseresult = DecoderImpl.DecodeCPHY(options, abedgepulseptr, bcedgepulseptr, caedgepulseptr, out decoderesult);

                PAM2EdgePulseSequence.Free(ref abedgepulseptr, ref abpulseshandle);
                PAM2EdgePulseSequence.Free(ref bcedgepulseptr, ref bcpulseshandle);
                PAM2EdgePulseSequence.Free(ref caedgepulseptr, ref capulseshandle);

                Int32 structsize = Marshal.SizeOf(typeof(CPHYEventCPP));
                CPHYPacketInfo cphypacket = new CPHYPacketInfo();

                for (Int32 i = 0; i < decoderesult.EventCount; i++)
                {
                    CPHYEventCPP presult = (CPHYEventCPP)Marshal.PtrToStructure(decoderesult.CPHYEventPtr + i * structsize, typeof(CPHYEventCPP));

                    CPHYPacketInfo cphyeventpacket = new CPHYPacketInfo();
                    GetCPHYPacket(presult, ref cphyeventpacket);
                    _PacketInfos.Add(cphyeventpacket);
                }

                DecoderImpl.FreeCPHY(ref decoderesult);
                UpdateView(aindex);
            }
            catch
            {
            }
        }

        internal void UpdateView(Int32 chindex)
        {
            if (_NeedUpdateViewInfo)
            {
                _NeedUpdateViewInfo = false;
                var buffer = GetDecodeBuffer();
                _EventInfos.Clear();
                buffer.Clear();
                if (_PacketInfos.Count() == 0)
                {
                    _DecodeResultData.DecodeViewInfos = new IDecodeViewInfo[0];
                    buffer.Add(_DecodeResultData);
                    ChangeBuffer();
                    return;
                }
                else
                {
                    //界面更新--标签
                    UpdateLabel(chindex);
                    //界面更新--事件列表
                    UpdateTabView(chindex);
                    
                    buffer.Add(_DecodeResultData);
                    ChangeBuffer();
                }
            }
        }

        private void UpdateTabView(Int32 chindex)
        {
            _DecodeResultData.DecodeViewInfos = _PacketInfos.SelectMany(x =>
            {
                Int32 endindex = 0;
                _EventInfos.Add(new ProtocolEventInfo
                {
                    Index = _EventInfos.Count
                });
                _EventInfos[^1].EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                List<CPHYDecodePacket> packets = new();
                {
                    CPHYStartDecodePacket packet = new(CalcPosition((UInt32)x.SyncWordInfo.StartIndex, A, chindex), CalcBitLenght((Int32)x.SyncWordInfo.Length, A, chindex));
                    _EventInfos[^1].StartTimeByPs = GetTimeFromPosition(packet.Start, chindex);
                    _EventInfos[^1].EventInofs[0] = (packet.Data, packet.BitCount);
                    _EventInfos[^1].StartPosition = packet.Start;
                    packets.Add(packet);
                    endindex = (Int32)x.SyncWordInfo.StartIndex;
                }
                {
                    CPHYVirtualChannelDecodePacket packet = new(CalcPosition((UInt32)x.VcInfo1.StartIndex, A, chindex), CalcBitLenght((Int32)x.VcInfo1.Length, A, chindex))
                    {
                        Data = new Byte[] { x.Vc1, x.Vc2 },
                    };
                    _EventInfos[^1].EventInofs[0] = (packet.Data, packet.BitCount);
                    packets.Add(packet);
                }
                {
                    CPHYDataTypeDecodePacket packet = new(CalcPosition((UInt32)x.DataTypeInfo1.StartIndex, A, chindex), CalcBitLenght((Int32)x.DataTypeInfo1.Length, A, chindex))
                    {
                        Data = new Byte[] { x.DataType1, x.DataType2 }
                    };
                    _EventInfos[^1].EventInofs[1] = (packet.Data, packet.BitCount);
                    packets.Add(packet);
                }
                if (x.IsLongPacket == 0)
                {
                    {
                        CPHYDataDecodePacket packet = new(CalcPosition((UInt32)x.ShortDataStart1[0], A, chindex), CalcBitLenght((Int32)(x.ShortDataStart1[x.ShortDataStart1.Length - 1] + x.ShortDataLength1[x.ShortDataLength1.Length - 1]), A, chindex))
                        {
                            Data = new Byte[] { x.ShortData1[0], x.ShortData1[1], x.ShortData2[0], x.ShortData2[0] },
                            BitCount = 32
                        };
                        _EventInfos[^1].EventInofs[3] = (packet.Data, packet.BitCount);
                        packets.Add(packet);
                    }
                }
                else
                {
                    var tempdata1 = BitConverter.GetBytes(x.WordCount1);
                    var tempdata2 = BitConverter.GetBytes(x.WordCount2);
                    CPHYWordCountDecodePacket packet = new(CalcPosition((UInt32)x.WordCountInfo1.StartIndex, A, chindex), CalcBitLenght((Int32)(x.WordCountInfo1.Length), A, chindex))
                    {
                        Data = new Byte[] { tempdata1[0], tempdata1[1], tempdata2[0], tempdata2[1] }
                    };
                    _EventInfos[^1].EventInofs[2] = (packet.Data, packet.BitCount);
                    packets.Add(packet);
                    if (x.LongDataStart != null)
                    {
                        CPHYDataDecodePacket packet2 = new(CalcPosition((UInt32)x.LongDataStart[0], A, chindex), CalcBitLenght((Int32)(x.LongDataStart[x.LongDataStart.Length - 1] + x.LongDataLength[x.LongDataLength.Length - 1]), A, chindex))
                        {
                            Data = x.LongData,
                            BitCount = (UInt32)(x.LongData.Length * 8)
                        };
                        _EventInfos[^1].EventInofs[3] = (packet2.Data, packet2.BitCount);
                        packets.Add(packet2);
                    }
                    CPHYPDCRCDecodePacket packet3 = new(CalcPosition((UInt32)x.PdChecksumInfo.StartIndex, A, chindex), CalcBitLenght((Int32)(x.PdChecksumInfo.Length), A, chindex))
                    {
                        Data = BitConverter.GetBytes(x.PdLongChecksum),
                    };
                    _EventInfos[^1].EventInofs[4] = (packet3.Data, packet3.BitCount);
                    packets.Add(packet3);
                }
                {
                    var tempdata1 = BitConverter.GetBytes(x.PhChecksum1);
                    var tempdata2 = BitConverter.GetBytes(x.PhChecksum2);
                    CPHYPHCRCDecodePacket packet = new(CalcPosition((UInt32)x.PhChecksumInfo1.StartIndex, A, chindex), CalcBitLenght((Int32)(x.PhChecksumInfo1.Length), A, chindex))
                    {
                        Data = new Byte[] { tempdata1[0], tempdata1[1], tempdata2[0], tempdata2[1] }
                    };
                    _EventInfos[^1].EventInofs[5] = (packet.Data, packet.BitCount);
                    packets.Add(packet);
                }
                {
                    String str = "";
                    Int32 crcerrornum = 0;
                    if (x.PhChecksum1 != x.PhCalcChecksum1)
                    {
                        crcerrornum++;
                        str = str + "CRC,Calculated " + x.PhCalcChecksum1.ToString("X4") + "h;";
                    }
                    if (x.PhChecksum2 != x.PhCalcChecksum2)
                    {
                        crcerrornum++;
                        str = str + "CRC,Calculated " + x.PhCalcChecksum2.ToString("X4") + "h;";
                    }
                    if (crcerrornum != 0)
                    {
                        str += "PHCRC:CRC Error(" + crcerrornum.ToString() + ")";
                    }
                    else
                    {
                        if (x.PdLongChecksum != x.PdCalcLongChecksum)
                        {
                            str += "CRC,Calculated " + x.PdCalcLongChecksum.ToString("X4") + "h;";
                        }
                        else if (x.IncompletedError == 1)
                        {
                            str += "Incomplete Packet;";
                        }
                    }
                    CPHYErrorDecodePacket packet = new(0, 0)
                    {
                        Data = Encoding.UTF8.GetBytes(str)
                    };
                    _EventInfos[^1].EventInofs[6] = (packet.Data, packet.BitCount);
                    packets.Add(packet);
                }

                return packets;
            }).ToArray();
        }

        private void UpdateLabel(Int32 chindex)
        {
            _DecodeResultData.DecodeViewInfos = _PacketInfos.SelectMany(x =>
            {
                var endindex = 0;
                List<CPHYDecodePacket> packets = new();
                {
                    CPHYStartDecodePacket packet = new(CalcPosition((UInt32)x.SyncWordInfo.StartIndex, A, chindex), CalcBitLenght((Int32)x.SyncWordInfo.Length, A, chindex));
                    packets.Add(packet);
                    endindex = (Int32)x.SyncWordInfo.StartIndex;
                }
                {
                    CPHYVirtualChannelDecodePacket packet = new(CalcPosition((UInt32)x.VcInfo1.StartIndex, A, chindex), CalcBitLenght((Int32)x.VcInfo1.Length, A, chindex))
                    {
                        Data = new Byte[] { x.Vc1 },
                    };
                    packets.Add(packet);
                }
                {
                    CPHYVirtualChannelDecodePacket packet = new(CalcPosition((UInt32)x.VcInfo2.StartIndex, A, chindex), CalcBitLenght((Int32)x.VcInfo2.Length, A, chindex))
                    {
                        Data = new Byte[] { x.Vc2 },
                    };
                    packets.Add(packet);
                }
                {
                    CPHYDataTypeDecodePacket packet = new(CalcPosition((UInt32)x.DataTypeInfo1.StartIndex, A, chindex), CalcBitLenght((Int32)x.DataTypeInfo1.Length, A, chindex))
                    {
                        Data = new Byte[] { x.DataType1 }
                    };
                    packets.Add(packet);
                }
                {
                    CPHYDataTypeDecodePacket packet = new(CalcPosition((UInt32)x.DataTypeInfo2.StartIndex, A, chindex), CalcBitLenght((Int32)x.DataTypeInfo2.Length, A, chindex))
                    {
                        Data = new Byte[] { x.DataType2 }
                    };
                    packets.Add(packet);
                }
                if (x.IsLongPacket == 0)
                {
                    {
                        CPHYDataDecodePacket packet = new(CalcPosition((UInt32)x.ShortDataStart1[0], A, chindex), CalcBitLenght((Int32)(x.ShortDataStart1[x.ShortDataStart1.Length - 1] + x.ShortDataLength1[x.ShortDataLength1.Length - 1]), A, chindex))
                        {
                            Data = new Byte[] { x.ShortData1[0], x.ShortData1[1] },
                            BitCount = 16
                        };
                        packets.Add(packet);
                    }
                    {
                        CPHYDataDecodePacket packet = new(CalcPosition((UInt32)x.ShortDataStart2[0], A, chindex), CalcBitLenght((Int32)(x.ShortDataStart2[x.ShortDataStart1.Length - 1] + x.ShortDataLength1[x.ShortDataLength1.Length - 1]), A, chindex))
                        {
                            Data = new Byte[] { x.ShortData2[0], x.ShortData2[1] },
                            BitCount = 16
                        };
                        packets.Add(packet);
                    }
                }
                else
                {
                    var tempdata1 = BitConverter.GetBytes(x.WordCount1);
                    var tempdata2 = BitConverter.GetBytes(x.WordCount2);
                    CPHYWordCountDecodePacket packet1 = new(CalcPosition((UInt32)x.WordCountInfo1.StartIndex, A, chindex), CalcBitLenght((Int32)(x.WordCountInfo1.Length), A, chindex))
                    {
                        Data = new Byte[] { tempdata1[0], tempdata1[1] }
                    };
                    packets.Add(packet1);
                    packet1 = new(CalcPosition((UInt32)x.WordCountInfo2.StartIndex, A, chindex), CalcBitLenght((Int32)(x.WordCountInfo2.Length), A, chindex))
                    {
                        Data = new Byte[] { tempdata1[0], tempdata1[1] }
                    };
                    packets.Add(packet1);
                    if (x.LongDataStart != null)
                    {
                        CPHYDataDecodePacket packet2 = new(CalcPosition((UInt32)x.LongDataStart[0], A, chindex), CalcBitLenght((Int32)(x.LongDataStart[x.LongDataStart.Length - 1] + x.LongDataLength[x.LongDataLength.Length - 1]), A, chindex))
                        {
                            Data = x.LongData,
                            BitCount = (UInt32)(x.LongData.Length * 8)
                        };
                        packets.Add(packet2);
                    }
                    CPHYPDCRCDecodePacket packet3 = new(CalcPosition((UInt32)x.PdChecksumInfo.StartIndex, A, chindex), CalcBitLenght((Int32)(x.PdChecksumInfo.Length), A, chindex))
                    {
                        Data = BitConverter.GetBytes(x.PdLongChecksum),
                    };
                    packets.Add(packet3);
                }
                {
                    var tempdata1 = BitConverter.GetBytes(x.PhChecksum1);
                    var tempdata2 = BitConverter.GetBytes(x.PhChecksum2);
                    CPHYPHCRCDecodePacket packet = new(CalcPosition((UInt32)x.PhChecksumInfo1.StartIndex, A, chindex), CalcBitLenght((Int32)(x.PhChecksumInfo1.Length), A, chindex))
                    {
                        Data = new Byte[] { tempdata1[0], tempdata1[1] }
                    };
                    packets.Add(packet);
                    packet = new(CalcPosition((UInt32)x.PhChecksumInfo2.StartIndex, A, chindex), CalcBitLenght((Int32)(x.PhChecksumInfo2.Length), A, chindex))
                    {
                        Data = new Byte[] { tempdata2[0], tempdata2[1] }
                    };
                    packets.Add(packet);
                }
                return packets;
            }).ToArray();
        }

        private void GetCPHYPacket(CPHYEventCPP presult, ref CPHYPacketInfo cphyEventPacket)
        {
            cphyEventPacket.SyncWordInfo = presult.SyncWordInfo;

            cphyEventPacket.VcInfo1 = presult.VcInfo1;
            cphyEventPacket.Vc1 = presult.Vc1;
            cphyEventPacket.VcInfo2 = presult.VcInfo2;
            cphyEventPacket.Vc2 = presult.Vc2;

            cphyEventPacket.DataTypeInfo1 = presult.DataTypeInfo1;
            cphyEventPacket.DataType1 = presult.DataType1;
            cphyEventPacket.DataTypeInfo2 = presult.DataTypeInfo2;
            cphyEventPacket.DataType2 = presult.DataType2;

            cphyEventPacket.WordCountInfo1 = presult.WordCountInfo1;
            cphyEventPacket.WordCount1 = presult.WordCount1;
            cphyEventPacket.WordCountInfo2 = presult.WordCountInfo2;
            cphyEventPacket.WordCount2 = presult.WordCount2;

            cphyEventPacket.ShortDataLen = presult.ShortDataLen;

            if (cphyEventPacket.ShortDataLen != 0)
            {
                cphyEventPacket.ShortDataStart1 = new Int64[cphyEventPacket.ShortDataLen];
                cphyEventPacket.ShortDataLength1 = new Int16[cphyEventPacket.ShortDataLen];
                cphyEventPacket.ShortData1 = new Byte[cphyEventPacket.ShortDataLen];
                cphyEventPacket.ShortDataStart2 = new Int64[cphyEventPacket.ShortDataLen];
                cphyEventPacket.ShortDataLength2 = new Int16[cphyEventPacket.ShortDataLen];
                cphyEventPacket.ShortData2 = new Byte[cphyEventPacket.ShortDataLen];
                Marshal.Copy(presult.ShortDataStart1, cphyEventPacket.ShortDataStart1, 0, (Int32)cphyEventPacket.ShortDataLen);
                Marshal.Copy(presult.ShortDataLength1, cphyEventPacket.ShortDataLength1, 0, (Int32)cphyEventPacket.ShortDataLen);
                Marshal.Copy(presult.ShortData1, cphyEventPacket.ShortData1, 0, (Int32)cphyEventPacket.ShortDataLen);
                Marshal.Copy(presult.ShortDataStart2, cphyEventPacket.ShortDataStart2, 0, (Int32)cphyEventPacket.ShortDataLen);
                Marshal.Copy(presult.ShortDataLength2, cphyEventPacket.ShortDataLength2, 0, (Int32)cphyEventPacket.ShortDataLen);
                Marshal.Copy(presult.ShortData2, cphyEventPacket.ShortData2, 0, (Int32)cphyEventPacket.ShortDataLen);
            }

            cphyEventPacket.LongDataLen = presult.LongDataLen;

            if (cphyEventPacket.LongDataLen != 0)
            {
                cphyEventPacket.LongDataStart = new Int64[cphyEventPacket.LongDataLen];
                cphyEventPacket.LongDataLength = new Int16[cphyEventPacket.LongDataLen];
                cphyEventPacket.LongData = new Byte[cphyEventPacket.LongDataLen];
                Marshal.Copy(presult.LongDataStart, cphyEventPacket.LongDataStart, 0, (Int32)cphyEventPacket.LongDataLen);
                Marshal.Copy(presult.LongDataLength, cphyEventPacket.LongDataLength, 0, (Int32)cphyEventPacket.LongDataLen);
                Marshal.Copy(presult.LongData, cphyEventPacket.LongData, 0, (Int32)cphyEventPacket.LongDataLen);
            }

            cphyEventPacket.PhChecksumInfo1 = presult.PhChecksumInfo1;
            cphyEventPacket.PhChecksum1 = presult.PhChecksum1;
            cphyEventPacket.PhChecksumInfo2 = presult.PhChecksumInfo2;
            cphyEventPacket.PhChecksum2 = presult.PhChecksum2;

            cphyEventPacket.PdChecksumInfo = presult.PdChecksumInfo;
            cphyEventPacket.PdLongChecksum = presult.PdLongChecksum;

            cphyEventPacket.PhCalcChecksum1 = presult.PhCalcChecksum1;
            cphyEventPacket.PhCalcChecksum2 = presult.PhCalcChecksum2;
            cphyEventPacket.PdCalcLongChecksum = presult.PdCalcLongChecksum;

            cphyEventPacket.IsLongPacket = presult.IsLongPacket;

            cphyEventPacket.IncompletedError = presult.IncompletedError;
            cphyEventPacket.Ph1CrcError = presult.Ph1CrcError;
            cphyEventPacket.Ph2CrcError = presult.Ph2CrcError;
            cphyEventPacket.PdCrcError = presult.PdCrcError;
        }

        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolCPHYOptions()
            {
                AThreshold = _AThreshold,
                BThreshold = _BThreshold,
                CThreshold = _CThreshold,
                LPAThreshold = _LPAThreshold,
                LPCThreshold = _LPCThreshold,
                SignalInputA = A,
                SignalInputB = B,
                SignalInputC = C,
                SignalType = SignalType,
                SubType = SubType,
                BitRate = BitRate
            };
        }
    }
    abstract class CPHYDecodeCPP
    {
        private protected CPHYDecodeModelCPP _Model;
        public abstract ProtocolCPHY.SubType SubType { get; }
        public CPHYDecodeCPP(CPHYDecodeModelCPP model)
        {
            _Model = model;
        }
        public abstract void ParsingData(ref CancellationToken token, ref Boolean needClear, ref List<CPHYPacketInfo> packetInfos);
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NPOI.HSSF.Record.CF;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.POIFS.Properties;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using NPOI.Util;
using NPOI.XWPF.UserModel;
using ScopeX.ComModel;
using ScopeX.Core.Decode.Model;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using SixLabors.ImageSharp.Memory;
using static ScopeX.Core.Decode.EdgeInfoCPP;
using static ScopeX.Core.Decode.EthernetDecodeModel;

namespace ScopeX.Core.Decode
{
    internal partial class EthernetDecodeModel :ProtocolModel
    {
        private List<EthernetResultPacket> _PacketInfos = new List<EthernetResultPacket>();
        private DecodeResultData _DecodeResultData = new DecodeResultData();
        private List<PAM3EdgePulse> _EdgePulsesList = new List<PAM3EdgePulse>();

        public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>()
        {
            "Index",
            "Start Time",
            "Preamble",
            "SFD",
            "Src MAC",
            "Dest MAC",
            "Lenght/Type",
            "Data",
            "FCS",
            //"UDP Head",
            //"IP Head",
            "Error",
        }.AsReadOnly();

        public IReadOnlyList<String> PhyLayerInfos { get; } = new List<String>()
        {
            "No Error",
            "Decode MLT3:Same As Previous level",
            "Decode MLT3: Too Many Zero Level",
            "Decode 5B4B: Decode Error",
            "Decode Link Layer:FCS Error",
            "Decode Network Layer: Header Check sum Error",
            "Decode Transport Layer: Header Check sum Error",
        }.AsReadOnly();

        public override Double BitRateByPs => 1f / (Speed switch
        {
            ProtocolEthernet.EthernetSpeed.EthernetSpeed_1000M => 1000_000_000,
            ProtocolEthernet.EthernetSpeed.EthernetSpeed_100M => 100_000_000,
            ProtocolEthernet.EthernetSpeed.EthernetSpeed_10M => 10_000_000,
            _ => 10_000_000,
        }) * 1E+12;

        private ProtocolEthernet.EthernetVersion _Version = ProtocolEthernet.EthernetVersion.IPv4;
        public ProtocolEthernet.EthernetVersion Version
        {
            get { return _Version; }
            set { UpdateProperty(ref _Version, value); }
        }

        /// <summary>
        /// 数据源的阈值
        /// </summary>
        //private Single _Source1Threshold = 0;
        //public Single Source1Threshold
        //{
        //    get { return (Single)(_Source1Threshold * TryGetChannelGain(Source1)); }
        //    set { UpdateProperty(ref _Source1Threshold, (Single)(value / TryGetChannelGain(Source1))); }
        //}
        /// <summary>
        /// 数据源的阈值
        /// </summary>
        //private Single _Source2Threshold = 0;
        //public Single Source2Threshold
        //{
        //    get { return (Single)(_Source2Threshold * TryGetChannelGain(Source2)); }
        //    set { UpdateProperty(ref _Source2Threshold, (Single)(value / TryGetChannelGain(Source2))); }
        //}

        public String Source2Unit => GetChannelUnit(Source2);
        public String Source1Unit => GetChannelUnit(Source1);

        public Single MinThreshold1 => -MaxThreshold1;
        public Single MinThreshold2 => -MaxThreshold2;
        public Single MaxThreshold1 => (Single)(8 * TryGetChannelGain(Source1));
        public Single MaxThreshold2 => (Single)(8 * TryGetChannelGain(Source2));

        /// <summary>
        /// 数据源的阈值
        /// </summary>
        private Single _Source1ThresholdH = 0;
        public Single Source1ThresholdH
        {
            get { return (Single)(_Source1ThresholdH * TryGetChannelGain(Source1)); }
            set { UpdateProperty(ref _Source1ThresholdH, (Single)(value / TryGetChannelGain(Source1))); }
        }

        /// <summary>
        /// 数据源的阈值
        /// </summary>
        private Single _Source1ThresholdL = 0;
        public Single Source1ThresholdL
        {
            get { return (Single)(_Source1ThresholdL * TryGetChannelGain(Source1)); }
            set { UpdateProperty(ref _Source1ThresholdL, (Single)(value / TryGetChannelGain(Source1))); }
        }

        private ChannelId _Source1 = ChannelId.C1;
        public ChannelId Source1
        {
            get { return _Source1; }
            set { UpdateProperty(ref _Source1, value); }
        }

        private ChannelId _Source2 = ChannelId.C2;
        public ChannelId Source2
        {
            get { return _Source2; }
            set { UpdateProperty(ref _Source2, value); }
        }

        private ProtocolEthernet.SignalType _SignalType = ProtocolEthernet.SignalType.Difference;
        public ProtocolEthernet.SignalType SignalType
        {
            get { return _SignalType; }
            set { UpdateProperty(ref _SignalType, value); }
        }

        private ProtocolEthernet.EthernetSpeed _Speed = ProtocolEthernet.EthernetSpeed.EthernetSpeed_100M;
        public ProtocolEthernet.EthernetSpeed Speed
        {
            get { return _Speed; }
            set 
            { 
                if (value != ProtocolEthernet.EthernetSpeed.EthernetSpeed_100M)
                {
                    value = ProtocolEthernet.EthernetSpeed.EthernetSpeed_100M;
                }
                UpdateProperty(ref _Speed, value);
            }
        }

        private Boolean _QFlag = false;
        public Boolean QFlag
        {
            get { return _QFlag; }
            set { UpdateProperty(ref _QFlag, value); }
        }

        public EthernetDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.Ethernet, isTrigDecode)
        {
        }

        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolEthernetOptions()
            {
                QFlag = Convert.ToByte(QFlag),
                //Signal1Threshold = _Source1Threshold,
                Signal1ThresholdH = _Source1ThresholdH,
                Signal1ThresholdL = _Source1ThresholdL,
               // Signal2Threshold = _Source2Threshold,
                SignalInput1 = Source1,
                SignalInput2 = Source2,
                SignalType = SignalType,
                Speed = Speed,
                Version = Version,
            };
        }
        internal override Boolean SourceHasData()
        {
            if (DsoPrsnt.DefaultDsoPrsnt == null)
                return false;
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(_Source1, out var prsnt);
            if (prsnt == null)
                return false;

            Double[] thresholds;
            if ((Speed == ProtocolEthernet.EthernetSpeed.EthernetSpeed_100M) || (Speed == ProtocolEthernet.EthernetSpeed.EthernetSpeed_10M))
            {
                thresholds = new Double[2];
                thresholds[0] = _Source1ThresholdH;
                thresholds[1] = _Source1ThresholdL;
            }
            else
            {
                thresholds = new Double[2];
            }

            if (_Source1.IsReference() && prsnt.Active && prsnt.VuDatabase != null && prsnt.VuDatabase.Current != null)
            {
                return DecodeDataHelper.ReferenceHasData(_Source1, thresholds);
            }

            if (_Source1.IsAnalog())
            {
                return DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }

            return false;
        }
        public override void UpdateReferenceDataStatus()
        {
            if (_Source1.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].Channels != null
                && DecodeDataSource.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].Channels[0] == _Source1)
            {
                DecodeDataSource.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].HasData = false;
            }
        }
        internal override Boolean CheckUpdate(ref Int64 laststamp)
        {
            //if (SignalInput1.IsAnalog())
            //{
            //    return laststamp != DecodeDataHelper.Instance.AnalogDataSource.TimeStamp;
            //}
            //if (SignalInput1.IsReference())
            //{
            //    return laststamp != DecodeDataHelper.Instance.ReferenceDataSource[SignalInput1 - ChannelIdExt.MinRChId].TimeStamp;
            //}

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
            Int32 index = 0;
            Int32 chindex = GetChIndex(_Source1);
            UInt32 datalen = 0;
            Double samplerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, _Source1, ref samplerate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, _Source1, ref datalen);
            Boolean needclear = false;

            if (MoreThanStorage() || chindex == -1 || datalen == 0 || samplerate == 0)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _PacketInfos.Clear();
            }
            try
            {
                if (!_NeedDecodeData)
                {
                    return;
                }

                IntPtr edgepulseptr = IntPtr.Zero;
                GCHandle edgepulseshandle;
                EthernetOptions options = new()
                {
                    SignalType = SignalType,
                    Ipv4Flag = (Byte)(Version == ProtocolEthernet.EthernetVersion.IPv4 ? 1 : 0),
                    CancelFlag = _CancelFlagPtr
                };
                if ((ProtocolEthernet.EthernetSpeed.EthernetSpeed_100M == Speed) || (ProtocolEthernet.EthernetSpeed.EthernetSpeed_10M == Speed))
                {
                    options.SignalRatelType = (ProtocolEthernet.EthernetSpeed.EthernetSpeed_100M == Speed) ? ProtocolEthernet.EthernetSpeed.EthernetSpeed_100M : ProtocolEthernet.EthernetSpeed.EthernetSpeed_10M;
                    ThreeLevelEdgeInfo? node = DecodeDataHelper.Instance.GetThreeLevelEdgeInfo(BusId, index, _Source1, ref token, ref needclear)/*?.Child */as ThreeLevelEdgeInfo;
                    if (node == null)
                    {
                        return;
                    }
                    // 边沿脉宽信息获取
                    _EdgePulsesList.Clear();
                    DecodeDataHelper.Instance.GetThreeLevelPulses(ref node, ref _EdgePulsesList);
                    PAM3EdgePulseSequence.Allocate(ref _EdgePulsesList, (UInt64)datalen, samplerate, out edgepulseptr, out edgepulseshandle);
                }
                else
                {
                    return;
                }
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;

                //开始解码
                EthernetResult results;
                results.EventInfosPtr = IntPtr.Zero;
                if (!DecoderImpl.DecodeEthernet(options, edgepulseptr, out results))
                {
                }
                PAM3EdgePulseSequence.Free(ref edgepulseptr, ref edgepulseshandle);

                //解码结果获取转换
                _PacketInfos.Clear();
                Int32 eventsize = Marshal.SizeOf(typeof(EthernetEvent));
                for (Int32 i = 0; i < results.EventCount; i++)
                {
                    IntPtr peventptr = new IntPtr(results.EventInfosPtr.ToInt64() + i * eventsize);
                    EthernetEvent revent = (EthernetEvent)Marshal.PtrToStructure(peventptr, typeof(EthernetEvent));
                    List<EventDataInfo> edatalist = new List<EventDataInfo>();
                    if (EthernetEventType.LinkLayerData == revent.EventType)
                    {
                        revent.GetDataList(out edatalist);
                    }
                    EthernetResultPacket packinfo = new()
                    {
                        PaketInfo = revent,
                        EthDataInfos = edatalist,
                    };
                    _PacketInfos.Add(packinfo);
                }

                //c++资源释放
                DecoderImpl.FreeEthernet(results);

                //数据界面更新
                UpdateView(chindex);
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
                if (_PacketInfos.Count == 0)
                {
                    _DecodeResultData.DecodeViewInfos = new IDecodeViewInfo[0];
                    buffer.Add(_DecodeResultData);
                    ChangeBuffer();
                    return;
                }
                else
                {
                    //界面更新
                    _DecodeResultData.DecodeViewInfos = _PacketInfos.SelectMany(x =>
                    {
                        var eventinfo = new ProtocolEventInfo();
                        var endindex = 0;
                        eventinfo.Index = _EventInfos.Count;
                        eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                        List<EthernetDecodePacket> packets = new List<EthernetDecodePacket>();

                        if (x.PaketInfo.PhylayerInfos.HasData > 0)
                        {
                            EthernetDecodePacket startPacket = new EthernetDecodePacket(CalcPosition((Int32)x.PaketInfo.PhylayerInfos.StartIndex, _Source1, chindex), CalcBitLenght((Int32)x.PaketInfo.PhylayerInfos.Len / 80, _Source1, chindex), EthernetPacketType.Start);
                            packets.Add(startPacket);

                            eventinfo.StartTimeByPs = base.GetTimeFromPosition(x.PaketInfo.PhylayerInfos.StartIndex, chindex);
                            eventinfo.StartPosition = x.PaketInfo.PhylayerInfos.StartIndex;
                            String info = PhyLayerInfos[(Int32)x.PaketInfo.PhylayerInfos.event_error_type];
                            String temp_info = "--";
                            eventinfo.EventInofs[0] = (Encoding.Default.GetBytes(temp_info), 0);
                            eventinfo.EventInofs[1] = (Encoding.Default.GetBytes(temp_info), 0);
                            eventinfo.EventInofs[2] = (Encoding.Default.GetBytes(temp_info), 0);
                            eventinfo.EventInofs[3] = (Encoding.Default.GetBytes(temp_info), 0);
                            eventinfo.EventInofs[4] = (Encoding.Default.GetBytes(temp_info), 0);
                            eventinfo.EventInofs[5] = (Encoding.Default.GetBytes(temp_info), 0);
                            eventinfo.EventInofs[6] = (Encoding.Default.GetBytes(temp_info), 0);
                            eventinfo.EventInofs[7] = (Encoding.Default.GetBytes(info), 0);
                            endindex = (Int32)x.PaketInfo.PhylayerInfos.StartIndex;
                        }
                        if (x.PaketInfo.Preamble.HasData > 0)
                        {
                            EthernetDecodePacket startPacket = new EthernetDecodePacket(CalcPosition((Int32)x.PaketInfo.Preamble.StartIndex - 1, _Source1, chindex), CalcBitLenght((Int32)x.PaketInfo.Preamble.Len / 80, _Source1, chindex), EthernetPacketType.Start);
                            packets.Add(startPacket);

                            EthernetDecodePacket preamblepacket = new EthernetDecodePacket(CalcPosition((Int32)x.PaketInfo.Preamble.StartIndex, _Source1, chindex),
                                CalcBitLenght((Int32)x.PaketInfo.Preamble.Len, _Source1, chindex), EthernetPacketType.Preamble)
                            {
                                _BitCount = 56,
                                _Title = "preamble data",
                                Data = x.PaketInfo.PreambleData,
                            };
                            //if (preamblepacket.Data[0] != 0X55)
                            //{
                            //    preamblepacket._BitCount = 8;
                            //}
                            eventinfo.StartTimeByPs = base.GetTimeFromPosition(preamblepacket.Start, chindex);
                            eventinfo.StartPosition = preamblepacket.Start;
                            eventinfo.EventInofs[0] = (preamblepacket.Data, preamblepacket.BitCount);
                            packets.Add(preamblepacket);
                            endindex = (Int32)x.PaketInfo.Preamble.StartIndex + (Int32)x.PaketInfo.Preamble.Len;
                        }

                        if (x.PaketInfo.SSD.HasData > 0)
                        {
                            EthernetDecodePacket ssdpacket = new EthernetDecodePacket(CalcPosition((Int32)x.PaketInfo.SSD.StartIndex, _Source1, chindex),
                                CalcBitLenght((Int32)x.PaketInfo.SSD.Len, _Source1, chindex), EthernetPacketType.SSD)
                            {
                                _BitCount = 8,
                                _Title = "SSD",
                                Data = new Byte[1],
                            };
                            ssdpacket.Data[0] = x.PaketInfo.SSDData;
                            eventinfo.EventInofs[1] = (ssdpacket.Data, ssdpacket.BitCount);
                            packets.Add(ssdpacket);
                            endindex = (Int32)x.PaketInfo.SSD.StartIndex + (Int32)x.PaketInfo.SSD.Len;
                        }

                        if (x.PaketInfo.DestMacAddress.HasData > 0)
                        {
                            EthernetDecodePacket dstMackframepacket = new EthernetDecodePacket(CalcPosition((Int32)x.PaketInfo.DestMacAddress.StartIndex, _Source1, chindex),
                                CalcBitLenght((Int32)x.PaketInfo.DestMacAddress.Len, _Source1, chindex), EthernetPacketType.INFO)
                            {
                                _BitCount = 48,
                                _Title = "dest mac address",
                                Data = x.PaketInfo.DestMacAddressData,
                            };
                            eventinfo.EventInofs[2] = (dstMackframepacket.Data, dstMackframepacket.BitCount);
                            packets.Add(dstMackframepacket);
                            endindex = (Int32)x.PaketInfo.DestMacAddress.StartIndex + (Int32)x.PaketInfo.DestMacAddress.Len;
                        }

                        if (x.PaketInfo.SrcMacAddress.HasData > 0)
                        {
                            EthernetDecodePacket srcMackframepacket = new EthernetDecodePacket(CalcPosition((Int32)x.PaketInfo.SrcMacAddress.StartIndex, _Source1, chindex),
                                           CalcBitLenght((Int32)x.PaketInfo.SrcMacAddress.Len, _Source1, chindex), EthernetPacketType.INFO)
                            {
                                _BitCount = 48,
                                _Title = "source mac address",
                                Data = x.PaketInfo.SrcMacAddressData,
                            };
                            eventinfo.EventInofs[3] = (srcMackframepacket.Data, srcMackframepacket.BitCount);
                            packets.Add(srcMackframepacket);
                            endindex = (Int32)x.PaketInfo.SrcMacAddress.StartIndex + (Int32)x.PaketInfo.SrcMacAddress.Len;
                        }

                        if (x.PaketInfo.EthernetType.HasData > 0)
                        {
                            EthernetDecodePacket ethernetTypePacket = new EthernetDecodePacket(CalcPosition((Int32)x.PaketInfo.EthernetType.StartIndex, _Source1, chindex),
                                      CalcBitLenght((Int32)x.PaketInfo.EthernetType.Len, _Source1, chindex), EthernetPacketType.INFO)
                            {
                                _BitCount = 16,
                                _Title = "Ethernet type",
                                Data = x.PaketInfo.EthernetTypeData,
                            };
                            Array.Reverse(ethernetTypePacket.Data);
                            eventinfo.EventInofs[4] = (ethernetTypePacket.Data, ethernetTypePacket.BitCount);
                            packets.Add(ethernetTypePacket);
                            endindex = (Int32)x.PaketInfo.EthernetType.StartIndex + (Int32)x.PaketInfo.EthernetType.Len;
                        }
                        if (x.PaketInfo.FrameDatasCnt > 0)
                        {
                            for (Int32 i = 0; i < x.PaketInfo.FrameDatasCnt; i++)
                            {
                                EventDataInfo pData = x.EthDataInfos[i];
                                EthernetDecodePacket datapacket = new EthernetDecodePacket(CalcPosition((Int32)pData.StartIndex, _Source1, chindex),
                                                CalcBitLenght((Int32)pData.Len, _Source1, chindex), EthernetPacketType.Data)
                                {
                                    _BitCount = 8,
                                    _Title = "Data",
                                    Data = new Byte[] { pData.Data },
                                };
                                packets.Add(datapacket);
                                endindex = (Int32)pData.StartIndex + (Int32)pData.Len;
                            }
                            eventinfo.EventInofs[5] = (x.EthDataInfos.Select(x => x.Data).ToArray(), (UInt32)(packets.Where(x => x._PacketType == EthernetPacketType.Data).Select(x => (Int32)x.BitCount).Sum()));
                        }
                        if (x.PaketInfo.FcsCheckSum.HasData > 0)
                        {
                            EthernetDecodePacket crcpacket = new EthernetDecodePacket(CalcPosition((Int32)x.PaketInfo.FcsCheckSum.StartIndex, _Source1, chindex),
                            CalcBitLenght((Int32)x.PaketInfo.FcsCheckSum.Len, _Source1, chindex), EthernetPacketType.INFO)
                            {
                                _BitCount = 32,
                                _Title = "FCS",
                                Data = x.PaketInfo.FcsCheckSumData,
                            };
                            Array.Reverse(crcpacket.Data);
                            eventinfo.EventInofs[6] = (crcpacket.Data, crcpacket.BitCount);
                            packets.Add(crcpacket);
                            endindex = (Int32)x.PaketInfo.FcsCheckSum.StartIndex + (Int32)x.PaketInfo.FcsCheckSum.Len;
                        }

                        if (x.PaketInfo.Esd.HasData > 0)
                        {
                            EthernetDecodePacket endPacket = new EthernetDecodePacket(CalcPosition((Int32)x.PaketInfo.Esd.StartIndex, _Source1, chindex),
                            CalcBitLenght((Int32)x.PaketInfo.Esd.Len / 10, _Source1, chindex), EthernetPacketType.End);
                            packets.Add(endPacket);
                            endindex = (Int32)x.PaketInfo.Esd.StartIndex + (Int32)x.PaketInfo.Esd.Len;
                        }

                        eventinfo.EndPosition = CalcPosition(endindex, Source1, chindex);
                        eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, chindex);
                        _EventInfos.Add(eventinfo);
                        return packets.OrderBy(x => x.Start);
                    }).ToArray();
                    buffer.Add(_DecodeResultData);
                    ChangeBuffer();
                }
            }
        }
    }
}

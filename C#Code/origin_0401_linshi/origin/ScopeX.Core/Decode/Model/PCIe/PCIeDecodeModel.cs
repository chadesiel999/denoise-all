using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using System.Runtime.InteropServices;
using static ScopeX.ComModel.ProtocolPCIe;
using static ScopeX.Core.Decode.EdgeInfoCPP;
using ScopeX.Hardware.Driver;
using NPOI.SS.Formula.PTG;
using NPOI.Util;

namespace ScopeX.Core.Decode
{
    sealed internal class PCIeDecodeModel : ProtocolModel
    {
        private DecodeResultData _DecodeResultData = new DecodeResultData();
        private List<PAM2EdgePulse> _EdgePulsesList = new List<PAM2EdgePulse>();
        private Dictionary<String, Int32> _EventDict= new Dictionary<String,Int32>();


        public PCIeDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.PCIe, isTrigDecode)
        {
            _DecodeResultData.Name = "PCIE";
        }
        public override Double BitRateByPs => 1f / (Version == ProtocolPCIe.PCIeVersion.PCIeV1_0 ? 2.5 * 1E9 : 5 * 1E9) * 1E12;


        public override IReadOnlyList<String> EventInfoTitles { get; } = (new List<String>()
        {
            "Index",
            "Start Time",
            "COMMA or STP or STP",
            "Message Type",
            "Transaction ID",
            "Sequence Number",
            "Address",
            "Payload",
            "Completion Status",
            "ECRC",
            "CRC",  // 指tlp的 lcr和 dllp 的crc
            //"SeqID",
            //"TLP Type",
            //"TC",
            //"AT",
            //"Lenght",
            //"BusNumber",
            //"DeviceNumber",
            //"FunctionNumber",
            //"Tag",
            //"Msg Code",
            //"Address",
            //"Payload",
            //"ECRC",
            //"LCRC",
            //"END",
            //"EDB",
            //"Error",
        }).AsReadOnly();


    private ChannelId _Source = ChannelId.C1;

        public ChannelId Source
        {
            get { return _Source; }
            set { UpdateProperty(ref _Source, value); }
        }
        public Single MinThreshold1 => -MaxThreshold1;
        public Single MaxThreshold1 => (Single)(25 * TryGetChannelGain(_Source));
        public Single MinThreshold2 => -MaxThreshold2;
        public Single MaxThreshold2 => (Single)(25 * TryGetChannelGain(_SignalInput1));
        private Single _Threshold = 1;
        /// <summary>
        /// 数据源的阈值
        /// </summary>
        public Single Threshold
        {
            get { return (Single)(_Threshold* TryGetChannelGain(Source)); }
            set { UpdateProperty(ref _Threshold, (Single)(value/ TryGetChannelGain(Source))); }
        }
        public String Unit => GetChannelUnit(Source);

        private ProtocolPCIe.PCIeVersion _Version = ProtocolPCIe.PCIeVersion.PCIeV1_0;

        public ProtocolPCIe.PCIeVersion Version
        {
            get { return _Version; }
            set { UpdateProperty(ref _Version, value); }
        }

        private ProtocolPCIe.SignalType _SignalType = ProtocolPCIe.SignalType.Single;

        public ProtocolPCIe.SignalType SignalType
        {
            get { return _SignalType; }
            set { UpdateProperty(ref _SignalType, value); }
        }

        private ChannelId _SignalInput1 = ChannelId.C2;

        public ChannelId SignalInput1
        {
            get { return _SignalInput1; }
            set { UpdateProperty(ref _SignalInput1, value); }
        }

        public UInt16 MinBytesCount => 0;
        public UInt16 MaxBytesCount => 1023;
        private UInt16 _BytesCount = 1;
        public UInt16 BytesCount
        {
            get => _BytesCount;
            set => UpdateProperty(ref _BytesCount, value);
        }

        public override void UpdateReferenceDataStatus()
        {
            if (Source.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].Channels != null
                && DecodeDataSource.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].Channels[0] == Source)
            {
                DecodeDataSource.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].HasData = false;
            }
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
            return ;
            Int32 chindex = GetChIndex(Source);
            if (chindex == -1)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;

                //_PacketInfos.Clear();
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

            TwoLevelEdgeInfo? node = DecodeDataHelper.Instance.GetEdgeInfo(BusId, startindex: 0, Source, ref token, ref needclear) as TwoLevelEdgeInfo;

            IntPtr edgepulseptr = IntPtr.Zero;

            GCHandle pulseshandle;

            _EdgePulsesList.Clear();

            if (node == null)
                return;

            DecodeDataHelper.Instance.GetTwoLevelPulses(ref node, ref _EdgePulsesList);
            PAM2EdgePulseSequence.Allocate(ref _EdgePulsesList, (UInt64)datalen, samplerate, out edgepulseptr, out pulseshandle);


            if (_NeedDecodeData)
            {
                //_PacketInfos.Clear();

                PCIEOption option = new PCIEOption()
                {
                    pcie_version = Version,
                    pcie_protocol_type = PcieEncodingType.PCIE_ENCODEING_8B10B,
                };

                PCIEResult decoderesult;// = new PCIEResult();
                decoderesult.EventCount = 0;
                decoderesult.PCIEEvent = IntPtr.Zero;

                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;

                if (!DecoderImpl.DecodePCIE(ref option, edgepulseptr, out decoderesult))
                    return;

                PAM2EdgePulseSequence.Free(ref edgepulseptr, ref pulseshandle);

                List<PCIEDecodePacket> listpacket = new List<PCIEDecodePacket>(); ;

                // 事件size
                Int32 eventsize = Marshal.SizeOf(typeof(PCIEEvent));

                // 解码结果
                List<DecodeResultData> decoderesults = GetDecodeBuffer();

                // 根据界面pcie 信息
                if (_NeedUpdateViewInfo)
                {
                    _NeedUpdateViewInfo = false;
                    _EventInfos.Clear();
                    decoderesults.Clear();
                    ChangeBuffer();
                }

                List<PCIEDecodePacket> decodepackets = new List<PCIEDecodePacket>();

                // pcie packet size
                Int32 pciepacketsize = Marshal.SizeOf<PciePacketInfo>();

                // 事件结果
                for (Int32 i = 0; i < decoderesult.EventCount; ++i)
                {
                    ProtocolEventInfo eventinfo = new ProtocolEventInfo();
            
                    eventinfo.Index = _EventInfos.Count;
                    eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));

                    // pcie event
                    PCIEEvent pcieevent = (PCIEEvent)Marshal.PtrToStructure(decoderesult.PCIEEvent + i * eventsize, typeof(PCIEEvent));

                    eventinfo.StartTimeByPs = base.GetTimeFromPosition(pcieevent.EventStartIndex, chindex);
                    String temp_info = "--";
                    eventinfo.EventInofs[0] = (Encoding.Default.GetBytes(temp_info), 0);
                    eventinfo.EventInofs[1] = (Encoding.Default.GetBytes(temp_info), 0);
                    eventinfo.EventInofs[2] = (Encoding.Default.GetBytes(temp_info), 0);
                    eventinfo.EventInofs[3] = (Encoding.Default.GetBytes(temp_info), 0);
                    eventinfo.EventInofs[4] = (Encoding.Default.GetBytes(temp_info), 0);
                    eventinfo.EventInofs[5] = (Encoding.Default.GetBytes(temp_info), 0);
                    eventinfo.EventInofs[6] = (Encoding.Default.GetBytes(temp_info), 0);
                    eventinfo.EventInofs[7] = (Encoding.Default.GetBytes(temp_info), 0);
                    eventinfo.EventInofs[8] = (Encoding.Default.GetBytes(temp_info), 0);

                    Int32 index = 0;

                    PciePacketInfo packetinfo = (PciePacketInfo)Marshal.PtrToStructure(pcieevent.PciePacketPtr, typeof(PciePacketInfo));

                    // 包类型 skp
                    if (packetinfo.PacketType == PCIEPacketType.PACKET_TYPE_SKP_ORDER)
                    {
                        SkpOrderPacket packet = (SkpOrderPacket)Marshal.PtrToStructure(pcieevent.PciePacketPtr, typeof(SkpOrderPacket));

                        // CalcPosition((Int64)pcieevent.EventStartIndex, Source, chindex);
                        eventinfo.StartTimeByPs = base.GetTimeFromPosition(pcieevent.EventStartIndex, chindex);

                        String strmessage = "SKP Ordered Set";
                        Byte[] byteArray = System.Text.Encoding.Default.GetBytes(strmessage);

                        // skp
                        eventinfo.EventInofs[index++] = (new Byte[] { packet.field_comma }, (UInt32)Marshal.SizeOf(packet.field_comma) * 8);


                        // pcie解码结果
                        PCIEDecodePacket pciepacket = new PCIEDecodePacket(CalcPosition((Int64)pcieevent.EventStartIndex, Source, chindex),
                        CalcBitLenght((Int32)packet.PacketSize*8 , Source, chindex), PCIEPacketType.PACKET_TYPE_SKP_ORDER)
                        {
                            _BitCount = packet.PacketSize * 8,
                            _Title = strmessage,
                        };
                        decodepackets.Add(pciepacket);
                    }
                    else if (packetinfo.PacketType == PCIEPacketType.PACKET_TYPE_TLP)  // 包类型 tlp
                    {
                        TLPPacket packet = (TLPPacket)Marshal.PtrToStructure(ptr: pcieevent.PciePacketPtr, typeof(TLPPacket));

                        // 消息类型名
                        String strmessage = Enum.GetName(typeof(TLPFormat), packet.tlp_format) + " " + Enum.GetName(typeof(TLPMessageType), packet.tlp_message_type);//推荐

                        Byte[] byteArray = System.Text.Encoding.Default.GetBytes(strmessage);

                        // tlp
                        eventinfo.EventInofs[0] = (new Byte[] { packet.field_stp }, (UInt32)Marshal.SizeOf(packet.field_stp) * 8);

                        Byte type = (Byte) ((packet.field_fmt << 5) + packet.field_type);
                        // fmt type
                        eventinfo.EventInofs[1] = (new Byte[] { type }, 8 );

                        // 事务id
                        eventinfo.EventInofs[2] = ( BitConverter.GetBytes(packet.requester_transaction_id).Reverse().ToArray(), (UInt32)Marshal.SizeOf(packet.requester_transaction_id) * 8);
                        
                        // 序列id
                        eventinfo.EventInofs[3] = ( BitConverter.GetBytes(packet.field_sequence_id).Reverse().ToArray(), (UInt32)Marshal.SizeOf(packet.field_sequence_id) * 8);

                        // 内存地址（内存消息和io消息存在）
                        eventinfo.EventInofs[4] = (BitConverter.GetBytes(packet.field_memory.field_address).Reverse().ToArray(), (UInt32)Marshal.SizeOf(packet.field_memory.field_address) *8);

                        // 载荷
                        if (packet.tlp_format == TLPFormat.TLP_3DW_WITHDATA || packet.tlp_format == TLPFormat.TLP_4DW_WITHDATA)
                        {
                            // 载荷数据拷贝
                            Byte[] data = new Byte[packet.field_data_size * 8];
                            Marshal.Copy(packet.field_data, data, 0, (Int32)packet.field_data_size * 8);

                            eventinfo.EventInofs[5] = (data, packet.field_data_size * 8);
                        }

                        // 完成信号
                        if (TLPMessageType.CPL <= packet.tlp_message_type && TLPMessageType.CPIDLK >= packet.tlp_message_type)
                           eventinfo.EventInofs[6] = (BitConverter.GetBytes(packet.field_completion.field_completion_status).Reverse().ToArray(), (UInt32)Marshal.SizeOf(packet.field_completion.field_completion_status));
                       
                        // ecrc
                        if (packet.field_ecrc != 0)
                            eventinfo.EventInofs[7] = (BitConverter.GetBytes(packet.field_ecrc).Reverse().ToArray(), (UInt32)Marshal.SizeOf(packet.field_ecrc) * 8);

                        // lcrc
                        eventinfo.EventInofs[8] = (BitConverter.GetBytes(packet.field_lcrc).Reverse().ToArray(), (UInt32)Marshal.SizeOf(packet.field_lcrc) * 8);

                        // pcie解码结果
                        PCIEDecodePacket pciepacket = new PCIEDecodePacket(CalcPosition((Int64)pcieevent.EventStartIndex, Source, chindex),
                        CalcBitLenght((Int32)packet.PacketSize * 8, Source, chindex), PCIEPacketType.PACKET_TYPE_TLP)
                        {
                            _BitCount = packet.PacketSize * 8,
                            _Title = strmessage,
                        };
                        decodepackets.Add(pciepacket);
                    }
                    else if (packetinfo.PacketType == PCIEPacketType.PACKET_TYPE_DLLP)  // 包类型 dllp
                    {
                        DLLPacket packet = (DLLPacket)Marshal.PtrToStructure(pcieevent.PciePacketPtr, typeof(DLLPacket));

                        String strmessage = Enum.GetName(typeof(DLLPMessageType), packet.dllp_msg_type);
      
                        // dllp
                        eventinfo.EventInofs[index++] = (new Byte[] { packet.field_sdp }, (UInt32)Marshal.SizeOf(packet.field_sdp) * 8);
                        eventinfo.EventInofs[index++] = (BitConverter.GetBytes(packet.field_type).Reverse().ToArray(), (UInt32)Marshal.SizeOf(packet.field_type) * 8);

                        index = 8;
                        eventinfo.EventInofs[index++] = (BitConverter.GetBytes(packet.field_dlp_crc).Reverse().ToArray(), (UInt32)Marshal.SizeOf(packet.field_dlp_crc) * 8);

                        // pcie解码结果
                        PCIEDecodePacket pciepacket = new PCIEDecodePacket(CalcPosition((Int64)pcieevent.EventStartIndex, Source, chindex),
                        CalcBitLenght((Int32)packet.PacketSize * 8, Source, chindex), PCIEPacketType.PACKET_TYPE_DLLP)
                        {
                            _BitCount = packet.PacketSize * 8,
                            _Title = strmessage,
                        };
                        decodepackets.Add(pciepacket);

                    }
                    else
                    {
                        // 错误类型
                    }

                    // 错误事件count
                    if (pcieevent.ErrorEventCount > 0)
                    {
                        PCIEError pcieerror = (PCIEError)Marshal.PtrToStructure(pcieevent.PCIEErrorPtr + i * eventsize, typeof(PCIEError));
                    }

                    // 事件信息
                    _EventInfos.Add(eventinfo);
                }

                if (!DecoderImpl.FreePCIE(ref decoderesult))
                    return;

                _DecodeResultData.DecodeViewInfos = decodepackets.ToArray();
                decoderesults.Add(_DecodeResultData);
            }

        }


        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolPCIeOptions()
            {
                SignalInput = Source,
                SignalIutput1 = SignalInput1,
                SignalType = SignalType,
                Threshold = _Threshold,
                Version = Version,
                ByetsCount = BytesCount,
            };
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using NPOI.HSSF.Record.CF;
using NPOI.POIFS.Crypt.Dsig;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using ScopeX.MathExt;
using static ScopeX.Core.Decode.EdgeInfoCPP;

namespace ScopeX.Core.Decode
{
    internal sealed class CANFDDecodeModelCPP :ProtocolModel
    {
        private List<CANFDPacketInfoPK> _PacketInfos = new List<CANFDPacketInfoPK>();
        private DecodeResultData _DecodeResultData = new DecodeResultData();
        private List<PAM2EdgePulse> _EdgePulsesList = new List<PAM2EdgePulse>();
        private const Int32 _HASDATAFALSE = 0;

        public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>()
        {
            "Index",
            "Start Time",
            //"SOF",
            "StandardID",
            "ExpandID",
            "Control",
            "Data",
            "CRC",
            "ACK",
            "EOF",
            "ERROR"
        };


        public override Double BitRateByPs => (1f / this.SDCustomSignalRate * 1E+12);
        //FD信号速率表
        private Dictionary<ProtocolCANFD.FDSignalRate, Int64> _FDSignalRateMap = new Dictionary<ProtocolCANFD.FDSignalRate, Int64>();
        public Dictionary<ProtocolCANFD.FDSignalRate, Int64> FDSignalRateMap
        {
            get => _FDSignalRateMap;
        }

        public void InitFDSignalRateMap()
        {
            _FDSignalRateMap[ProtocolCANFD.FDSignalRate.SignalRate_custom] = FDCustomSignalRate;
            _FDSignalRateMap[ProtocolCANFD.FDSignalRate.SignalRate_250k] = 250_000L;
            _FDSignalRateMap[ProtocolCANFD.FDSignalRate.SignalRate_500k] = 500_000L;
            _FDSignalRateMap[ProtocolCANFD.FDSignalRate.SignalRate_800k] = 800_000L;
            _FDSignalRateMap[ProtocolCANFD.FDSignalRate.SignalRate_1M] = 1_000_000L;
            _FDSignalRateMap[ProtocolCANFD.FDSignalRate.SignalRate_1_5M] = 1_500_000L;
            _FDSignalRateMap[ProtocolCANFD.FDSignalRate.SignalRate_2M] = 2_000_000L;
            _FDSignalRateMap[ProtocolCANFD.FDSignalRate.SignalRate_3M] = 3_000_000L;
            _FDSignalRateMap[ProtocolCANFD.FDSignalRate.SignalRate_4M] = 4_000_000L;
            _FDSignalRateMap[ProtocolCANFD.FDSignalRate.SignalRate_5M] = 5_000_000L;
            _FDSignalRateMap[ProtocolCANFD.FDSignalRate.SignalRate_6M] = 6_000_000L;
            _FDSignalRateMap[ProtocolCANFD.FDSignalRate.SignalRate_7M] = 7_000_000L;
            _FDSignalRateMap[ProtocolCANFD.FDSignalRate.SignalRate_8M] = 8_000_000L;
        }
        //SD信号速率表
        private Dictionary<ProtocolCANFD.SDSignalRate, Int64> _SDSignalRateMap = new Dictionary<ProtocolCANFD.SDSignalRate, Int64>();
        public Dictionary<ProtocolCANFD.SDSignalRate, Int64> SDSignalRateMap
        {
            get => _SDSignalRateMap;
        }

        public void InitSDSignalRateMap()
        {
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_custom] = SDCustomSignalRate;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_10k] = 10_000L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_19_2k] = 19_200L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_20k] = 20_000L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_33_3k] = 33_300L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_38_4k] = 38_400L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_50k] = 50_000L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_57_6k] = 57_600L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_62_5k] = 62_500L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_83_3k] = 83_300L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_100k] = 100_000L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_115_2k] = 115_200L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_125k] = 125_000L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_230_4k] = 230_400L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_250k] = 250_000L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_490_8k] = 490_800L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_500k] = 500_000L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_800k] = 800_000L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_921_6k] = 921_600L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_1M] = 1_000_000L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_2M] = 2_000_000L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_3M] = 3_000_000L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_4M] = 4_000_000L;
            _SDSignalRateMap[ProtocolCANFD.SDSignalRate.SignalRate_5M] = 5_000_000L;
        }
        //信号速率
        private ProtocolCANFD.SDSignalRate _SDSignalRate = ProtocolCANFD.SDSignalRate.SignalRate_custom;
        public ProtocolCANFD.SDSignalRate SDSignalRate
        {
            get { return _SDSignalRate; }
            set
            {
                if (_SDSignalRate != value)
                {
                    _SDSignalRate = value;
                    SDCustomSignalRate = SDSignalRateMap[value];
                }
            }
        }

        //自定义的信号速率（当SignalRate == TriggerCAN_FDSignalRate.CAN_FDSignalRate_custom时使用）
        private Int64 _SDCustomSignalRate = Math.Clamp(0, MinSDSignalRate, MaxSDSignalRate);
        public Int64 SDCustomSignalRate
        {
            get { return _SDCustomSignalRate; }
            set
            {
                if (_SDCustomSignalRate != value)
                {

                    foreach (var kvp in SDSignalRateMap)
                    {
                        if (value == kvp.Value)
                        {
                            SDSignalRate =  kvp.Key;
                            break;
                        }
                    }
                    
                    UpdateProperty(ref _SDCustomSignalRate, value);
                }
            }
        }
        public static Int64 MaxSDSignalRate => 1000_000;
        public static Int64 MinSDSignalRate => 10_000;

        private ProtocolCANFD.FDSignalRate _FDSignalRate = ProtocolCANFD.FDSignalRate.SignalRate_custom;
        public ProtocolCANFD.FDSignalRate FDSignalRate
        {
            get { return _FDSignalRate; }
            set
            {
                if (_FDSignalRate != value)
                {
                    _FDSignalRate = value;
                    FDCustomSignalRate = FDSignalRateMap[value];
                    
                }
                UpdateProperty(ref _FDSignalRate, value);
            }
        }
        private Int64 _FDCustomSignalRate = Math.Clamp(0, MinFDSignalRate, MaxFDSignalRate);
        public Int64 FDCustomSignalRate
        {
            get { return _FDCustomSignalRate; }
            set
            {
                if (_FDCustomSignalRate != value)
                {
                    foreach (var kvp in FDSignalRateMap)
                    {
                        if (kvp.Value.Equals(value))
                        {
                            FDSignalRate = kvp.Key;
                            break;
                        }
                    }
                    UpdateProperty(ref _FDCustomSignalRate, value);
                }
            }
        }
        public static Int64 MaxFDSignalRate => 10_000_000;
        public static Int64 MinFDSignalRate => 1_000;

        //信号类型
        private ProtocolCANFD.SignalType _SignalType = ProtocolCANFD.SignalType.CAN_FDH;
        public ProtocolCANFD.SignalType SignalType
        {
            get { return _SignalType; }
            set { UpdateProperty(ref _SignalType, value); }
        }

        //输入1
        private ChannelId _Source1 = ChannelId.C1;
        public ChannelId Source1
        {
            get { return _Source1; }
            set { UpdateProperty(ref _Source1, value); }
        }

        public Int32 MinSamplePoint => 30;
        public Int32 MaxSamplePoint => 90;
        //输入2(信号类型选择"差分"时使用)
        private ChannelId _Source2 = ChannelId.C1;
        public ChannelId Source2
        {
            get { return _Source2; }
            set { UpdateProperty(ref _Source2, value); }
        }

        //采样点(%)
        private Int32 _SamplePoint = 70;
        /// <summary>
        /// 仲裁域采样率  采样点(%)
        /// </summary>
        public Int32 SamplePoint
        {
            get { return _SamplePoint; }
            set { UpdateProperty(ref _SamplePoint, value); }
        }

        public Int32 MinDataSamplePoint => 30;
        public Int32 MaxDataSamplePoint => 90;
        private Int32 _DataSamplePoint = 70;
        /// <summary>
        /// 数据域采样率
        /// </summary>
        public Int32 DataSamplePoint
        {
            get { return _DataSamplePoint; }
            set { UpdateProperty(ref _DataSamplePoint, value); }
        }

        public Double MaxThreshold => (Single)(12 * TryGetChannelGain(_Source1));
        public Double MinThreshold => -MaxThreshold;
        private Double _SDAThreshold = 1;
        /// <summary>
        /// 数据源的阈值
        /// </summary>
        public Double SDAThreshold
        {
            get { return _SDAThreshold * TryGetChannelGain(Source1); }
            set { UpdateProperty(ref _SDAThreshold, value / TryGetChannelGain(Source1)); }
        }

        public String SDAUnit => GetChannelUnit(Source1);

        public CANFDDecodeModelCPP(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.CAN_FD, isTrigDecode)
        {
            _DecodeResultData.Name = "CAN_FD";
            InitSDSignalRateMap();
            InitFDSignalRateMap();
        }

        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolCANFDOptions()
            {
                SamplePoint = SamplePoint,
                DataSamplePoint = DataSamplePoint,
                SDAThreshold = _SDAThreshold,
                SignalInput1 = Source1,
                SignalInput2 = Source2,
                SDSignalRate = SDCustomSignalRate,
                FDSignalRate = FDCustomSignalRate,
                SignalType = SignalType,
            };
        }

        internal override Boolean SourceHasData()
        {
            if (DsoPrsnt.DefaultDsoPrsnt == null)
                return false;

            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Source1, out var prsnt);
            if (prsnt == null)
                return false;

            if (Source1.IsReference() && prsnt.Active && prsnt.VuDatabase != null && prsnt.VuDatabase.Current != null)
            {
                return DecodeDataHelper.ReferenceHasData(Source1, SDAThreshold);
            }

            if (Source1.IsAnalog())
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
            if (Source1.IsAnalog() && laststamp != DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp;
                return true;
            }
            if (Source1.IsReference() && laststamp != DecodeDataHelper.Instance.ReferenceDataSource[Source1 - ChannelIdExt.MinRChId].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.ReferenceDataSource[Source1 - ChannelIdExt.MinRChId].TimeStamp;
                return true;
            }

            return false;
        }

        internal override void ParsingData(ref CancellationToken token)
        {
            Int32 index = 0;
            Int32 chindex = GetChIndex(Source1);
            UInt32 datalen = 0;
            Double samplerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, Source1, ref samplerate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, Source1, ref datalen);
            Int32 bitdatacount = 0;
            Boolean needclear = false;
            IntPtr edgepulseptr = IntPtr.Zero;
            CANResult canresults = new CANResult();
            if (MoreThanStorage() || chindex == -1 || datalen == 0 || samplerate == 0)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _PacketInfos.Clear();
            }
            try
            {
                if (_NeedDecodeData)
                {
                    bitdatacount = (Int32)(Math.Round(1d / SDCustomSignalRate * samplerate, 0));

                    _NeedDecodeData = false;
                    _NeedUpdateViewInfo = true;
                    _PacketInfos.Clear();
                    if (bitdatacount >= 2)
                    {
                        //用户参数
                        CANOptions options = new()
                        {
                            CancelFlag = _CancelFlagPtr,
                            SignalType = SignalType,
                            SdSignalRate = (UInt32)SDCustomSignalRate,
                            FdSignalRate = (UInt32)FDCustomSignalRate,
                            SdSamplePointRate = SamplePoint / 100.0,
                            FdSamplePointRate = DataSamplePoint / 100.0,
                        };

                        TwoLevelEdgeInfo? node = DecodeDataHelper.Instance.GetEdgeInfo(BusId, index, Source1, ref token, ref needclear) as TwoLevelEdgeInfo;
                        if (node == null)
                        {
                            return;
                        }

                        // 边沿脉宽信息获取
                        GCHandle edgepulseshandle;
                        _EdgePulsesList.Clear();
                        DecodeDataHelper.Instance.GetTwoLevelPulses(ref node, ref _EdgePulsesList);
                        PAM2EdgePulseSequence.Allocate(ref _EdgePulsesList, (UInt64)datalen, samplerate, out edgepulseptr, out edgepulseshandle);

                        //开始解码                       
                        canresults.EventInfosPtr = IntPtr.Zero;
                        DecoderImpl.DecodeCAN(options, edgepulseptr, out canresults);
                        PAM2EdgePulseSequence.Free(ref edgepulseptr, ref edgepulseshandle);

                        _PacketInfos.Clear();
                        //解码结果获取转换
                        Int32 structsize = Marshal.SizeOf(typeof(CANEventCPP));
                        for (Int32 i = 0; i < canresults.EventCount; i++)
                        {
                            IntPtr presultptr = new IntPtr(canresults.EventInfosPtr.ToInt64() + i * structsize);
                            CANEventCPP canevent = (CANEventCPP)Marshal.PtrToStructure(presultptr, typeof(CANEventCPP));
                            Int32 datastructsize = Marshal.SizeOf(typeof(CANEventDataInfoCPP));
                            List<CANEventDataInfoCPP> edatalist = new List<CANEventDataInfoCPP>();
                            for (UInt32 pindex = 0; pindex < canevent.DataInfosCnt; pindex++)
                            {
                                IntPtr pdataptr = new IntPtr(canevent.DataInfos.ToInt64() + pindex * datastructsize);
                                CANEventDataInfoCPP pdata = (CANEventDataInfoCPP)Marshal.PtrToStructure(pdataptr, typeof(CANEventDataInfoCPP));
                                edatalist.Add(pdata);
                            }
                            CANFDPacketInfoPK packInfo = new()
                            {
                                PacketInfo = canevent,
                                DataInfos = edatalist,
                            };
                            _PacketInfos.Add(packInfo);
                        }
                        //c++资源释放
                        DecoderImpl.FreeCAN(canresults);
                    }
                }

                // 通知界面更新
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
                try
                {
                    if (_PacketInfos.Count == 0)
                    {
                        _DecodeResultData.DecodeViewInfos = new IDecodeViewInfo[0];
                        buffer.Add(_DecodeResultData);
                        ChangeBuffer();
                        return;
                    }

                    _DecodeResultData.DecodeViewInfos = _PacketInfos.SelectMany(x =>
                    {
                        String errorstr = "";
                        var eventinfo = new ProtocolEventInfo();
                        var endindex = 0;
                        eventinfo.Index = _EventInfos.Count;
                        eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                        List<CANFDDecodePacket> packets = new List<CANFDDecodePacket>();

                        if (x.PacketInfo.PacketType == (Byte)FrameType.ErrorFrame)
                        {
                            CANFDSOFDecodePacket packet = new CANFDSOFDecodePacket(CalcPosition((UInt32)x.PacketInfo.SOF.StartIndex, Source1, chindex), CalcBitLenght((int)x.PacketInfo.SOF.Len, Source1, chindex));
                            eventinfo.StartTimeByPs = base.GetTimeFromPosition(packet.Start, chindex);
                            //eventinfo.EventInofs[0] = (packet.Data, packet.BitCount);
                            packets.Add(packet);
                            eventinfo.StartPosition = packet.Start;
                            eventinfo.EventInofs[7] = (packet.Data, 0);
                            packets.Add(packet);
                            endindex = (Int32)x.PacketInfo.SOF.StartIndex + (Int32)x.PacketInfo.SOF.Len;
                        }
                        else if (x.PacketInfo.PacketType == (Byte)FrameType.OverloadFrame)
                        {
                            CANFDOverLoadFrameDeocdePacketCPP packet = new(CalcPosition((UInt32)x.PacketInfo.SOF.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.SOF.Len, Source1, chindex))
                            {
                            };
                            eventinfo.StartTimeByPs = GetTimeFromPosition(packet.Start, chindex);
                            eventinfo.StartPosition = packet.Start;
                            eventinfo.EventInofs[7] = (packet.Data, 0);
                            packets.Add(packet);
                            endindex = (Int32)x.PacketInfo.SOF.StartIndex + (Int32)x.PacketInfo.SOF.Len;
                        }
                        else if (x.PacketInfo.PacketType == (Byte)FrameType.StandardRemoteFrame || x.PacketInfo.PacketType == (Byte)FrameType.StandardDataFrame)
                        {
                            CANFDSOFDecodePacket startpacket = new CANFDSOFDecodePacket(CalcPosition((UInt32)x.PacketInfo.SOF.StartIndex, Source1, chindex), CalcBitLenght((int)x.PacketInfo.SOF.Len, Source1, chindex));
                            eventinfo.StartTimeByPs = base.GetTimeFromPosition(startpacket.Start, chindex);
                            //eventinfo.EventInofs[0] = (packet.Data, packet.BitCount);
                            packets.Add(startpacket);
                            eventinfo.StartPosition = startpacket.Start;
                            endindex = (Int32)x.PacketInfo.SOF.StartIndex;

                            if (x.PacketInfo.StandardIdInfo.HasData != _HASDATAFALSE)
                            {
                                CANFDStandardIDDecodePacket packet = new CANFDStandardIDDecodePacket(CalcPosition((UInt32)x.PacketInfo.StandardIdInfo.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.StandardIdInfo.Len, Source1, chindex))
                                {
                                    Success = x.PacketInfo.StandardIdInfo.ErrorType == (Byte)CANFDDataErrorType.NoError,
                                    Data = x.PacketInfo.StandardId,//ConvertBitArrayToBytes(ref x.PacketInfo.StandardId)
                                };
                                eventinfo.EventInofs[0] = (packet.Data, packet.BitCount);
                                packets.Add(packet);
                                endindex = (Int32)x.PacketInfo.StandardIdInfo.StartIndex + (Int32)x.PacketInfo.StandardIdInfo.Len;
                            }
                        }
                        else
                        {
                            CANFDSOFDecodePacket startpacket = new CANFDSOFDecodePacket(CalcPosition((UInt32)x.PacketInfo.SOF.StartIndex, Source1, chindex), CalcBitLenght((int)x.PacketInfo.SOF.Len, Source1, chindex));
                            eventinfo.StartTimeByPs = base.GetTimeFromPosition(startpacket.Start, chindex);
                            //eventinfo.EventInofs[0] = (packet.Data, packet.BitCount);
                            packets.Add(startpacket);
                            eventinfo.StartPosition = startpacket.Start;
                            endindex = (Int32)x.PacketInfo.SOF.StartIndex;
                            if (x.PacketInfo.ExtIdInfo.HasData != _HASDATAFALSE)
                            {
                                CANFDExtandIDDecodePacket packet = new CANFDExtandIDDecodePacket(CalcPosition((UInt32)x.PacketInfo.ExtIdInfo.StartIndex, Source1, chindex), CalcBitLenght((Int32)(x.PacketInfo.ExtIdInfo.Len), Source1, chindex))
                                {
                                    Success = x.PacketInfo.ExtIdInfo.ErrorType == (Byte)CANFDDataErrorType.NoError,
                                    Data = x.PacketInfo.ExtId,// ConvertBitArrayToBytes(ref x.PacketInfo.ExtId),
                                };
                                eventinfo.EventInofs[1] = (packet.Data, packet.BitCount);
                                packets.Add(packet);
                                endindex = (Int32)x.PacketInfo.ExtIdInfo.StartIndex + (Int32)x.PacketInfo.ExtIdInfo.Len;
                            }
                        }
                        if (x.PacketInfo.DLCInfo.HasData != _HASDATAFALSE)
                        {
                            CANFDDLCDecodePacket packet = new CANFDDLCDecodePacket(CalcPosition((UInt32)x.PacketInfo.DLCInfo.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.DLCInfo.Len, Source1, chindex))
                            {
                                Data = new Byte[] { x.PacketInfo.DLC },
                            };
                            eventinfo.EventInofs[2] = (packet.Data, packet.BitCount);
                            packets.Add(packet);
                            endindex = (Int32)x.PacketInfo.DLCInfo.StartIndex + (Int32)x.PacketInfo.DLCInfo.Len;
                        }
                        //if(x.HasBRS)
                        //{
                        //    packets.Add(new CANFDBRSDecodePacket(CalcPosition(x.BRSIndex, SignalInput1, chindex), CalcBitLenght(x.BRSLength, SignalInput1, chindex))
                        //    {
                        //        Status = x.BRS,
                        //    });
                        //}
                        if (x.PacketInfo.DataInfosCnt != 0)
                        {
                            //List<CANFDDataInfoCPP> ethDataList = new List<CANFDDataInfoCPP>();
                            Int32 dataStructSize = Marshal.SizeOf(typeof(CANEventDataInfoCPP));
                            for (Int32 i = 0; i < x.PacketInfo.DataInfosCnt; i++)
                            {
                                //IntPtr pdataPtr = new IntPtr(x.DataInfos.ToInt64() + i * dataStructSize);
                                CANEventDataInfoCPP pdata = x.DataInfos[i];// (CANFDDataInfoCPP)Marshal.PtrToStructure(pdataPtr, typeof(CANFDDataInfoCPP));
                                CANFDDataDecodePacket packet = new CANFDDataDecodePacket(CalcPosition((UInt32)pdata.StartIndex, Source1, chindex), CalcBitLenght((Int32)pdata.Len, Source1, chindex))
                                {
                                    Success = pdata.ErrorType == (Byte)CANFDDataErrorType.NoError,
                                    Data = new Byte[] { pdata.Data },
                                };
                                packets.Add(packet);
                                endindex = (Int32)pdata.StartIndex + (Int32)pdata.Len;
                                //ethDataList.Add(pdata);
                            }
                            eventinfo.EventInofs[3] = (x.DataInfos.Select(x => x.Data).ToArray(), (UInt32)(packets.Where(x => x is CANFDDataDecodePacket).Select(x => (Int32)x.BitCount).Sum()));
                        }

                        if (x.PacketInfo.CRCInfo.HasData != _HASDATAFALSE)
                        {
                            CANFDCRCDecodePacket packet = new CANFDCRCDecodePacket(CalcPosition((UInt32)x.PacketInfo.CRCInfo.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.CRCInfo.Len, Source1, chindex))
                            {
                                SuccessCRC = x.PacketInfo.SuccessCRC.Skip(1).Take((Int32)Math.Ceiling(x.PacketInfo.CRCBitCount / 8.0)).ToArray(),//BitConverter.GetBytes(x.PacketInfo.SuccessCRC).Take((Int32)Math.Ceiling(x.PacketInfo.CRCBitCount / 8.0)).Reverse().ToArray(),
                                BitCount = x.PacketInfo.CRCBitCount,
                                Data = x.PacketInfo.CRC.Take((Int32)Math.Ceiling(x.PacketInfo.CRCBitCount / 8.0)).ToArray(),// BitConverter.GetBytes(x.PacketInfo.CRC).Take((Int32)Math.Ceiling(x.PacketInfo.CRCBitCount / 8.0)).Reverse().ToArray(),
                            };
                            if (!packet.Success)
                            {
                                errorstr += "CRC " + "Calculated:" + BitConverter.ToString(packet.SuccessCRC).Replace("-", "");
                                ;
                            }
                            eventinfo.EventInofs[4] = (packet.Data, packet.BitCount);
                            packets.Add(packet);
                            endindex = (Int32)x.PacketInfo.CRCInfo.StartIndex + (Int32)x.PacketInfo.CRCInfo.Len;
                        }
                        if (x.PacketInfo.StuffInfo.HasData != _HASDATAFALSE)
                        {
                            packets.Add(new CANFDStuffCountDecodePacket(CalcPosition((UInt32)x.PacketInfo.StuffInfo.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.StuffInfo.Len, Source1, chindex))
                            {
                                StuffCount = x.PacketInfo.Stuff,
                            });
                            endindex = (Int32)x.PacketInfo.StuffInfo.StartIndex + (Int32)x.PacketInfo.StuffInfo.Len;
                        }
                        if (x.PacketInfo.StuffParityInfo.HasData != _HASDATAFALSE)
                        {
                            packets.Add(new CANFDStuffParityDecodePacket(CalcPosition((UInt32)x.PacketInfo.StuffParityInfo.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.StuffParityInfo.Len, Source1, chindex))
                            {
                                Status = x.PacketInfo.StuffParity == 1 ? true : false,
                                SuccessStatus = x.PacketInfo.SuccessStuffParity == 1 ? true : false,
                            });
                            endindex = (Int32)x.PacketInfo.StuffParityInfo.StartIndex + (Int32)x.PacketInfo.StuffParityInfo.Len;
                        }

                        if (x.PacketInfo.ACKInfo.HasData != _HASDATAFALSE)
                        {
                            CANFDACKDeocdePacket packet = new CANFDACKDeocdePacket(CalcPosition((UInt32)x.PacketInfo.ACKInfo.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.ACKInfo.Len, Source1, chindex))
                            {
                                Data = new Byte[] { x.PacketInfo.ACK },
                            };

                            packet.Success = x.PacketInfo.ACK == 1 ? true : false;
                            //packet.Data = new Byte[] { packet.Success?(byte)1: (byte)0 };
                            packets.Add(packet);
                            eventinfo.EventInofs[5] = (packet.Data, packet.BitCount);
                            endindex = (Int32)x.PacketInfo.ACKInfo.StartIndex + (Int32)x.PacketInfo.ACKInfo.Len;
                        }

                        if (x.PacketInfo.EOFInfo.HasData != _HASDATAFALSE)
                        {
                            CANFDEOFDecodePacket packet = new CANFDEOFDecodePacket(CalcPosition((UInt32)x.PacketInfo.EOFInfo.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.EOFInfo.Len, Source1, chindex))
                            {
                                Data = new Byte[] { x.PacketInfo.EOF },
                            };
                            eventinfo.EventInofs[6] = (packet.Data, packet.BitCount);
                            packets.Add(packet);
                            endindex = (Int32)x.PacketInfo.EOFInfo.StartIndex + (Int32)x.PacketInfo.EOFInfo.Len;
                        }

                        if (errorstr.Length > 0)
                        {
                            //errorstr = errorstr.Remove(errorstr.Length - 1, 1);
                            Byte[] errorData = Encoding.Default.GetBytes(errorstr);
                            eventinfo.EventInofs[7] = (errorData, 0);
                        }

                        eventinfo.EndPosition = CalcPosition(endindex, Source1, chindex);
                        eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, chindex);
                        _EventInfos.Add(eventinfo);
                        return packets.OrderBy(x => x.Start);
                    }).ToArray();
                    buffer.Add(_DecodeResultData);
                }
                catch (Exception ex)
                {
                }
                ChangeBuffer();
            }
        }
    }
}

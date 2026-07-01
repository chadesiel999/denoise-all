using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Sockets;
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

namespace ScopeX.Core.Decode
{
    [Obsolete("Please Use Class 'CANFDDecodeModelCPP'", true)]
    internal sealed class CANFDDecodeModel : ProtocolModel
    {
        private readonly Int32 _MinEndFrameBitCount = 7;
        private readonly Int32 _DLCBitcount = 4;
        private readonly Int32 _StandardIDBitCount = 11;
        private readonly Int32 _ExtpandIDBitCount = 18;
        private readonly Int32 _StuffByteCount = 3;
        private List<CANFDPacketInfo> _PacketInfos = new List<CANFDPacketInfo>();
        private DecodeResultData _DecodeResultData = new DecodeResultData();
        private BitManger _BitManger;
        public CANFDDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id, SerialProtocolType.CAN_FD, isTrigDecode)
        {
            _DecodeResultData.Name = "CAN_FD";
            _BitManger = new BitManger(this);
        }


        public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>()
        {
            "Index",
            "Start Time",
            "SOF",
            "StandardID",
            "ExpandID",
            "Control",
            "Data",
            "CRC",
            "ACK",
            "EOF"
        };


        public override Double BitRateByPs => (1f / this.SDCustomSignalRate * 1E+12);

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
                    switch (value)
                    {
                        case ProtocolCANFD.SDSignalRate.SignalRate_10k:
                            SDCustomSignalRate = 10_000;
                            break;
                        case ProtocolCANFD.SDSignalRate.SignalRate_20k:
                            SDCustomSignalRate = 20_000;
                            break;
                        case ProtocolCANFD.SDSignalRate.SignalRate_33_3k:
                            SDCustomSignalRate = 33_300;
                            break;
                        case ProtocolCANFD.SDSignalRate.SignalRate_50k:
                            SDCustomSignalRate = 50_000;
                            break;
                        case ProtocolCANFD.SDSignalRate.SignalRate_62_5k:
                            SDCustomSignalRate = 62_500;
                            break;
                        case ProtocolCANFD.SDSignalRate.SignalRate_83_3k:
                            SDCustomSignalRate = 83_300;
                            break;
                        case ProtocolCANFD.SDSignalRate.SignalRate_100k:
                            SDCustomSignalRate = 100_000;
                            break;
                        case ProtocolCANFD.SDSignalRate.SignalRate_125k:
                            SDCustomSignalRate = 125_000;
                            break;
                        case ProtocolCANFD.SDSignalRate.SignalRate_1M:
                            SDCustomSignalRate = 1_000_000;
                            break;
                        case ProtocolCANFD.SDSignalRate.SignalRate_custom:
                            break;
                    }
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
                    switch (value)
                    {
                        case 10_000:
                            SDSignalRate = ProtocolCANFD.SDSignalRate.SignalRate_10k;
                            break;
                        case 20_000:
                            SDSignalRate = ProtocolCANFD.SDSignalRate.SignalRate_20k;
                            break;
                        case 33_300:
                            SDSignalRate = ProtocolCANFD.SDSignalRate.SignalRate_33_3k;
                            break;
                        case 50_000:
                            SDSignalRate = ProtocolCANFD.SDSignalRate.SignalRate_50k;
                            break;
                        case 62_500:
                            SDSignalRate = ProtocolCANFD.SDSignalRate.SignalRate_62_5k;
                            break;
                        case 83_300:
                            SDSignalRate = ProtocolCANFD.SDSignalRate.SignalRate_83_3k;
                            break;
                        case 100_000:
                            SDSignalRate = ProtocolCANFD.SDSignalRate.SignalRate_100k;
                            break;
                        case 125_000:
                            SDSignalRate = ProtocolCANFD.SDSignalRate.SignalRate_125k;
                            break;
                        case 1_000_000:
                            SDSignalRate = ProtocolCANFD.SDSignalRate.SignalRate_1M;
                            break;
                        default:
                            SDSignalRate = ProtocolCANFD.SDSignalRate.SignalRate_custom;
                            break;
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
                    switch (value)
                    {
                        case ProtocolCANFD.FDSignalRate.SignalRate_1M:
                            FDCustomSignalRate = 1_000_000;
                            break;
                        case ProtocolCANFD.FDSignalRate.SignalRate_2M:
                            FDCustomSignalRate = 2_000_000;
                            break;
                        case ProtocolCANFD.FDSignalRate.SignalRate_3M:
                            FDCustomSignalRate = 3_000_000;
                            break;
                        case ProtocolCANFD.FDSignalRate.SignalRate_4M:
                            FDCustomSignalRate = 4_000_000;
                            break;
                        case ProtocolCANFD.FDSignalRate.SignalRate_5M:
                            FDCustomSignalRate = 5_000_000;
                            break;
                        case ProtocolCANFD.FDSignalRate.SignalRate_6M:
                            FDCustomSignalRate = 6_000_000;
                            break;
                        case ProtocolCANFD.FDSignalRate.SignalRate_7M:
                            FDCustomSignalRate = 7_000_000;
                            break;
                        case ProtocolCANFD.FDSignalRate.SignalRate_8M:
                            FDCustomSignalRate = 8_000_000;
                            break;
                        case ProtocolCANFD.FDSignalRate.SignalRate_custom:
                            break;
                    }

                }
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
                    switch (value)
                    {
                        case 1_000_000:
                            FDSignalRate = ProtocolCANFD.FDSignalRate.SignalRate_1M;
                            break;
                        case 2_000_000:
                            FDSignalRate = ProtocolCANFD.FDSignalRate.SignalRate_2M;
                            break;
                        case 3_000_000:
                            FDSignalRate = ProtocolCANFD.FDSignalRate.SignalRate_3M;
                            break;
                        case 4_000_000:
                            FDSignalRate = ProtocolCANFD.FDSignalRate.SignalRate_4M;
                            break;
                        case 5_000_000:
                            FDSignalRate = ProtocolCANFD.FDSignalRate.SignalRate_5M;
                            break;
                        case 6_000_000:
                            FDSignalRate = ProtocolCANFD.FDSignalRate.SignalRate_6M;
                            break;
                        case 7_000_000:
                            FDSignalRate = ProtocolCANFD.FDSignalRate.SignalRate_7M;
                            break;
                        case 8_000_000:
                            FDSignalRate = ProtocolCANFD.FDSignalRate.SignalRate_8M;
                            break;
                        default:
                            FDSignalRate = ProtocolCANFD.FDSignalRate.SignalRate_custom;
                            break;
                    }
                    UpdateProperty(ref _FDCustomSignalRate, value);
                }
            }
        }
        public static Int64 MaxFDSignalRate => 16_000_000;
        public static Int64 MinFDSignalRate => 500_000;

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
            //if (SignalInput1.IsAnalog())
            //{
            //    return laststamp != DecodeDataHelper.Instance.AnalogDataSource.TimeStamp;
            //}
            //if (SignalInput1.IsReference())
            //{
            //    return laststamp != DecodeDataHelper.Instance.ReferenceDataSource[SignalInput1 - ChannelIdExt.MinRChId].TimeStamp;
            //}

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
            Int32 fdbitdatacount = 0;
            Boolean needclear = false;
            if (chindex == -1 || datalen == 0 || samplerate == 0)
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
                    fdbitdatacount = (Int32)(Math.Round(1d / FDCustomSignalRate * samplerate, 0));

                    _NeedDecodeData = false;
                    _NeedUpdateViewInfo = true;
                    _PacketInfos.Clear();
                    //if (bitdatacount >= 2 && fdbitdatacount >= 2)
                    if (bitdatacount >= 2)
                    {
                        Boolean success = false;
                        Int32 len = 0;
                        BaseEdgeInfo? node = DecodeDataHelper.Instance.GetEdgeInfo(BusId, index, Source1, ref token, ref needclear);
                        while (true)
                        {
                            CANFDPacketInfo packetInfo = new CANFDPacketInfo();
                            _BitManger.Clear(BusId, Source1, bitdatacount);
                            index = FindStartIndex(index, bitdatacount, datalen);
                            //Int32 actualBitDataCount = CalBitDataCount(index, bitdatacount, ref token, ref needclear);
                            //_BitManger.BitDataCount = actualBitDataCount;
                            if (index == -1) break;
                            packetInfo.SOF = _BitManger.GetBit(BusId, index, ref token, ref needclear, out success);
                            if (!success)
                            {
                                break;
                            }
                            index = _BitManger.BitIndex;
                            packetInfo.SOFIndex = _BitManger.BitIndex;
                            packetInfo.SOFLength = _BitManger.BitDataCount;
                            index += _BitManger.BitDataCount;
                            if (index < 0 || index >= datalen)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }
                            packetInfo.TempStandardID = _BitManger.GetBits(BusId, index, _StandardIDBitCount, out len, out success, ref token, ref needclear);
                            packetInfo.StandardID = BitConverter.GetBytes((UInt16)packetInfo.TempStandardID).Reverse().ToArray();
                            if (!success)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }
                            packetInfo.HasStandardID = true;
                            index = _BitManger.BitIndex;
                            packetInfo.StandardIDIndex = index;
                            packetInfo.StandardIDLength = len;
                            index += len;
                            if (index < 0 || index >= datalen)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }
                            packetInfo.RRS = _BitManger.GetBit(BusId, index, ref token, ref needclear, out success);
                            if (!success)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }
                            index = _BitManger.BitIndex;
                            packetInfo.RRSIndex = index;
                            packetInfo.RRSLength = _BitManger.BitDataCount;
                            packetInfo.HasRRS = true;
                            index += _BitManger.BitDataCount;
                            if (index < 0 || index >= datalen)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }
                            packetInfo.IDE = _BitManger.GetBit(BusId, index, ref token, ref needclear, out success);
                            if (!success)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }
                            index = _BitManger.BitIndex;
                            packetInfo.IDEIndex = index;
                            packetInfo.IDELength = _BitManger.BitDataCount;
                            packetInfo.HasIDE = true;
                            index += _BitManger.BitDataCount;
                            if (index < 0 || index >= datalen)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }
                            //扩展/标准帧判断
                            if (packetInfo.IDE)
                            {
                                packetInfo.HasRRS = false;
                                packetInfo.HasSRR = true;
                                packetInfo.SRR = packetInfo.RRS;
                                packetInfo.SRRIndex = packetInfo.RRSIndex;
                                packetInfo.SRRLength = packetInfo.RRSLength;
                                UInt64 tempid = packetInfo.TempStandardID;
                                _BitManger.GetBits(BusId, ref tempid, index, _ExtpandIDBitCount, out len, out success, ref token, ref needclear);
                                packetInfo.ExtID = BitConverter.GetBytes((UInt32)tempid).Reverse().ToArray();
                                if (!success)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                                index = _BitManger.BitIndex;
                                packetInfo.ExtIDIndex = index;
                                packetInfo.ExtIDLength = len;
                                packetInfo.HasExtID = true;
                                index += len;
                                if (index < 0 || index >= datalen)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }

                                packetInfo.RRS = _BitManger.GetBit(BusId, index, ref token, ref needclear, out success);
                                if (!success)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                                index = _BitManger.BitIndex;
                                packetInfo.RRSIndex = index;
                                packetInfo.RRSLength = _BitManger.BitDataCount;
                                packetInfo.HasRRS = true;
                                index += _BitManger.BitDataCount;
                                if (index < 0 || index >= datalen)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                            }
                            else
                            {
                            }
                            packetInfo.FDF = _BitManger.GetBit(BusId, index, ref token, ref needclear, out success);
                            if (!success)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }
                            index = _BitManger.BitIndex;
                            packetInfo.FDFIndex = index;
                            packetInfo.FDFLength = _BitManger.BitDataCount;
                            packetInfo.HasFDF = true;
                            index += _BitManger.BitDataCount;
                            if (index < 0 || index >= datalen)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }
                            //CANFD帧和CAN的扩展帧解析r0位，CAN标准帧r0即FDF字段
                            if ((packetInfo.FDF) || (!packetInfo.FDF && packetInfo.IDE))
                            {
                                packetInfo.Res = _BitManger.GetBit(BusId, index, ref token, ref needclear, out success);
                                if (!success)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                                index = _BitManger.BitIndex;
                                packetInfo.ResIndex = index;
                                packetInfo.ResLength = _BitManger.BitDataCount;
                                packetInfo.HasRes = true;
                                index += _BitManger.BitDataCount;
                                if (index < 0 || index >= datalen)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                            }
                            //若为CANFD帧，则有位速率和错误状态的解析
                            if (packetInfo.FDF)
                            {
                                packetInfo.BRS = _BitManger.GetBit(BusId, index, ref token, ref needclear, out success);
                                if (!success)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                                index = _BitManger.BitIndex;
                                packetInfo.BRSIndex = index;
                                if (packetInfo.BRS && (fdbitdatacount < 2))
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                                packetInfo.BRSLength = packetInfo.BRS ? (Int32)(bitdatacount * (SamplePoint / 100f) + fdbitdatacount * (1 - DataSamplePoint / 100f)) : bitdatacount;
                                packetInfo.HasBRS = true;
                                index += packetInfo.BRSLength;
                                if (index < 0 || index >= datalen)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                                _BitManger.BitDataCount = packetInfo.BRS ? fdbitdatacount : bitdatacount;
                                //Int32 actualFdDataCount = CalBitDataCount(index, fdbitdatacount, ref token, ref needclear);
                                //_BitManger.BitDataCount = packetInfo.BRS?actualFdDataCount: actualBitDataCount;
                                packetInfo.ESI = _BitManger.GetBit(BusId, index, ref token, ref needclear, out success);
                                if (!success)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                                index = _BitManger.BitIndex;
                                packetInfo.ESIIndex = index;
                                packetInfo.ESILength = _BitManger.BitDataCount;
                                packetInfo.HasESI = true;
                                index += _BitManger.BitDataCount;
                                if (index < 0 || index >= datalen)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                            }

                            packetInfo.DLC = (Byte)_BitManger.GetBits(BusId, index, _DLCBitcount, out len, out success, ref token, ref needclear);
                            if (!success)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }
                            index = _BitManger.BitIndex;
                            packetInfo.DLCIndex = index;
                            packetInfo.DLCLength = len;
                            packetInfo.HasDLC = true;
                            packetInfo.DataByteCount = packetInfo.FDF ? GetDLCByteCount(packetInfo.DLC) : packetInfo.DLC;
                            index += len;
                            if (index < 0 || index >= datalen)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }


                            if (!packetInfo.RRS)
                            {
                                if (packetInfo.IDE)
                                {
                                    packetInfo.FrameType = FrameType.ExtendedDataFrame;
                                }
                                else
                                {
                                    packetInfo.FrameType = FrameType.StandardDataFrame;
                                }
                                if (packetInfo.DataByteCount > 0)
                                {
                                    List<DataInfo> datainfos = new List<DataInfo>();
                                    for (Int32 dataindex = 0; dataindex < packetInfo.DataByteCount; dataindex++)
                                    {
                                        if (index < 0 || index >= datalen)
                                        {
                                            success = false;
                                            break;
                                        }
                                        DataInfo datainfo = new DataInfo();
                                        datainfo.Data = (Byte)_BitManger.GetBits(BusId, index, 8, out len, out success, ref token, ref needclear);
                                        if (!success) break;
                                        index = _BitManger.BitIndex;
                                        datainfo.Index = index;
                                        datainfo.Length = len;
                                        index += len;

                                        if ((packetInfo.FDF) && (dataindex == packetInfo.DataByteCount - 1))
                                        {
                                            _BitManger.ProcessDataEndStufBit(BusId, index, out len, ref token, ref needclear, out success);
                                        }
                                        datainfos.Add(datainfo);
                                    }
                                    packetInfo.DataInfos = datainfos.ToArray();
                                    packetInfo.HasData = packetInfo.DataInfos.Length > 0;

                                }
                            }
                            else
                            {
                                if (packetInfo.IDE)
                                {
                                    packetInfo.FrameType = FrameType.ExtendedRemoteFrame;
                                }
                                else
                                {
                                    packetInfo.FrameType = FrameType.StandardRemoteFrame;
                                }
                            }
                            if (!success || index >= datalen)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }

                            if (packetInfo.FDF)
                            {
                                //stuf count前的FSB，该位不做CRC校验
                                _BitManger.GetBit(BusId, index, ref token, ref needclear, out success, false);
                                index += _BitManger.BitDataCount;
                                if (index < 0 || index >= datalen)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                                packetInfo.Stuff = (Byte)_BitManger.GetBits(BusId, index, _StuffByteCount, out len, out success, ref token, ref needclear, true);
                                if (!success)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                                index = _BitManger.BitIndex;
                                packetInfo.StuffIndex = index;
                                packetInfo.StuffLength = len;
                                packetInfo.HasStuff = true;
                                index += len;
                                if (index < 0 || index >= datalen)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                                packetInfo.StuffParity = _BitManger.GetBit(BusId, index, ref token, ref needclear, out success, true);
                                if (!success)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                                packetInfo.StuffParityIndex = index;
                                packetInfo.StuffParityLength = _BitManger.BitDataCount;
                                packetInfo.HasStuffParity = true;
                                packetInfo.SuccessStuffParity = ParityData(packetInfo.Stuff);
                                index += _BitManger.BitDataCount * 2; //包含CRC第一个FSB
                                if (index < 0 || index >= datalen)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }

                                packetInfo.CRCIndex = index;
                                packetInfo.CRCBitCount = (Byte)(packetInfo.DataByteCount > 16 ? 21 : 17);
                                packetInfo.SuccessCRC = BitConverter.GetBytes(_BitManger.CalcCRC(packetInfo.DataByteCount > 16 ?
                                    CRCType.CRC21 : CRCType.CRC17)).Take((Int32)Math.Ceiling(packetInfo.CRCBitCount / 8.0)).Reverse().ToArray();
                                packetInfo.CRC = BitConverter.GetBytes(_BitManger.GetCRC(BusId, index, packetInfo.CRCBitCount,
                                    out len, out success, ref token, ref needclear)).Take((Int32)Math.Ceiling(packetInfo.CRCBitCount / 8.0)).Reverse().ToArray();
                                //packetInfo.SuccessCRC = packetInfo.CRC;
                                if (!success)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                                index = _BitManger.BitIndex;
                                packetInfo.CRCIndex = index;
                                packetInfo.CRCLength = len;
                                packetInfo.HasCRC = true;
                                index += len;
                                //BRS到CRC界定符之间是可变速率，CRC界定符
                                Int32 crcdelimeterlength = (Int32)((bitdatacount * (1 - SamplePoint / 100f)) + fdbitdatacount * (DataSamplePoint / 100f));
                                index += crcdelimeterlength;//_BitManger.BitDataCount;
                                if (index < 0 || index >= datalen)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                                _BitManger.BitDataCount = bitdatacount;
                            }
                            else
                            {
                                packetInfo.CRCIndex = index;
                                packetInfo.CRCBitCount = 15;
                                packetInfo.SuccessCRC = BitConverter.GetBytes(_BitManger.CalcCRC(CRCType.CRC15)).Take((Int32)Math.Ceiling(packetInfo.CRCBitCount / 8.0)).Reverse().ToArray();
                                packetInfo.CRC = BitConverter.GetBytes((UInt16)_BitManger.GetBits(BusId, index, 15, out len, out success,
                                    ref token, ref needclear, true)).Reverse().ToArray();
                                if (!success /*|| token.IsCancellationRequested || needclear*/)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                                index = _BitManger.BitIndex;
                                packetInfo.CRCIndex = index;
                                packetInfo.CRCLength = len;
                                packetInfo.HasCRC = true;
                                index += len;
                                index += bitdatacount;
                                if (index < 0 || index >= datalen)
                                {
                                    _PacketInfos.Add(packetInfo);
                                    break;
                                }
                            }

                            //_BitManger.BitDataCount = actualBitDataCount;
                            packetInfo.ACK = _BitManger.GetBit(BusId, index, ref token, ref needclear, out success, false);
                            if (!success)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }
                            packetInfo.ACKIndex = index;
                            packetInfo.ACKLength = _BitManger.BitDataCount;
                            packetInfo.HasACK = true;
                            index += _BitManger.BitDataCount;
                            if (index < 0 || index >= datalen)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }

                            packetInfo.EOF = (Byte)_BitManger.GetBits(BusId,index, _MinEndFrameBitCount, out len, out success, ref token, ref needclear, false);
                            if (!success)
                            {
                                _PacketInfos.Add(packetInfo);
                                break;
                            }
                            index = _BitManger.BitIndex;
                            packetInfo.EOFIndex = index;
                            packetInfo.EOFLength = len;
                            packetInfo.HasEOF = true;
                            //index += len;
                            _PacketInfos.Add(packetInfo);
                            if (index < 0 || index >= datalen) break;
                        }
                    }
                }
            }
            catch
            {
            }
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
                try
                {
                    _DecodeResultData.DecodeViewInfos = _PacketInfos.SelectMany(x =>
                    {
                        var eventinfo = new ProtocolEventInfo();
                        eventinfo.Index = _EventInfos.Count;
                        eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                        List<CANFDDecodePacket> packets = new List<CANFDDecodePacket>();

                        {
                            CANFDSOFDecodePacket packet = new CANFDSOFDecodePacket(CalcPosition(x.SOFIndex, Source1, chindex), CalcBitLenght(x.SOFLength, Source1, chindex));
                            eventinfo.StartTimeByPs = base.GetTimeFromPosition(packet.Start, chindex);
                            eventinfo.EventInofs[0] = (packet.Data, packet.BitCount);
                            packets.Add(packet);
                        }

                        if (x.FrameType == FrameType.StandardRemoteFrame || x.FrameType == FrameType.StandardDataFrame)
                        {
                            if (x.HasStandardID)
                            {
                                CANFDStandardIDDecodePacket packet = new CANFDStandardIDDecodePacket(CalcPosition(x.StandardIDIndex, Source1, chindex), CalcBitLenght(x.StandardIDLength, Source1, chindex))
                                {
                                    Data = x.StandardID
                                };
                                eventinfo.EventInofs[1] = (packet.Data, packet.BitCount);
                                packets.Add(packet);
                            }
                        }
                        else
                        {
                            if (x.HasExtID)
                            {
                                CANFDExtandIDDecodePacket packet = new CANFDExtandIDDecodePacket(CalcPosition(x.StandardIDIndex, Source1, chindex), CalcBitLenght(x.ExtIDIndex - x.StandardIDIndex + x.ExtIDLength, Source1, chindex))
                                {
                                    Data = x.ExtID,
                                };
                                eventinfo.EventInofs[2] = (packet.Data, packet.BitCount);
                                packets.Add(packet);
                            }
                        }
                        //if(x.HasSRR)
                        //{
                        //    packets.Add(new CANFDSRRDecodePacket(CalcPosition(x.SRRIndex, SignalInput1, chindex), CalcBitLenght(x.SRRLength, SignalInput1, chindex)));
                        //}
                        //if(x.HasIDE)
                        //{
                        //    packets.Add(new CANFDIDEDecodePacket(CalcPosition(x.IDEIndex, SignalInput1, chindex), CalcBitLenght(x.IDELength, SignalInput1, chindex))
                        //    {
                        //        Value = x.IDE,
                        //    });
                        //}
                        //if(x.HasRRS)
                        //{
                        //    packets.Add(new CANFDRRSDecodePacket(CalcPosition(x.RRSIndex, SignalInput1, chindex), CalcBitLenght(x.RRSLength, SignalInput1, chindex)));
                        //}
                        //if(x.HasESI)
                        //{
                        //    packets.Add(new CANFDESIDecodePacket(CalcPosition(x.ESIIndex, SignalInput1, chindex), CalcBitLenght(x.ESILength, SignalInput1, chindex))
                        //    {
                        //        Status = x.ESI,
                        //    });
                        //}
                        //if(x.HasFDF)
                        //{
                        //    packets.Add(new CANFDFDFDecodePacket(CalcPosition(x.FDFIndex, SignalInput1, chindex), CalcBitLenght(x.FDFLength, SignalInput1, chindex)));
                        //}
                        //if(x.HasRes)
                        //{
                        //    packets.Add(new CANFDresDecodePacket(CalcPosition(x.ResIndex, SignalInput1, chindex), CalcBitLenght(x.ResLength, SignalInput1, chindex)));
                        //}

                        if (x.HasDLC)
                        {
                            CANFDDLCDecodePacket packet = new CANFDDLCDecodePacket(CalcPosition(x.DLCIndex, Source1, chindex), CalcBitLenght(x.DLCLength, Source1, chindex))
                            {
                                Data = new Byte[] { x.DLC },
                            };
                            eventinfo.EventInofs[3] = (packet.Data, packet.BitCount);
                            packets.Add(packet);
                        }
                        //if(x.HasBRS)
                        //{
                        //    packets.Add(new CANFDBRSDecodePacket(CalcPosition(x.BRSIndex, SignalInput1, chindex), CalcBitLenght(x.BRSLength, SignalInput1, chindex))
                        //    {
                        //        Status = x.BRS,
                        //    });
                        //}
                        if (x.HasData)
                        {
                            foreach (var data in x.DataInfos)
                            {
                                CANFDDataDecodePacket packet = new CANFDDataDecodePacket(CalcPosition(data.Index, Source1, chindex), CalcBitLenght(data.Length, Source1, chindex))
                                {
                                    Data = new Byte[] { data.Data },
                                };
                                packets.Add(packet);
                            }
                            eventinfo.EventInofs[4] = (x.DataInfos.Select(x => x.Data).ToArray(), (UInt32)(packets.Where(x => x is CANFDDataDecodePacket).Select(x => (Int32)x.BitCount).Sum()));
                        }

                        if (x.HasCRC)
                        {
                            CANFDCRCDecodePacket packet = new CANFDCRCDecodePacket(CalcPosition(x.CRCIndex, Source1, chindex), CalcBitLenght(x.CRCLength, Source1, chindex))
                            {
                                SuccessCRC = x.SuccessCRC,
                                BitCount = x.CRCBitCount,
                                Data = x.CRC,
                            };
                            eventinfo.EventInofs[5] = (packet.Data, packet.BitCount);
                            packets.Add(packet);
                        }
                        if (x.HasStuff)
                        {
                            packets.Add(new CANFDStuffCountDecodePacket(CalcPosition(x.StuffIndex, Source1, chindex), CalcBitLenght(x.StuffLength, Source1, chindex))
                            {
                                StuffCount = x.Stuff,
                            });
                        }
                        if (x.HasStuffParity)
                        {
                            packets.Add(new CANFDStuffParityDecodePacket(CalcPosition(x.StuffParityIndex, Source1, chindex), CalcBitLenght(x.StuffParityLength, Source1, chindex))
                            {
                                Status = x.StuffParity,
                                SuccessStatus = x.SuccessStuffParity,
                            });
                        }

                        if (x.HasACK)
                        {
                            CANFDACKDeocdePacket packet = new CANFDACKDeocdePacket(CalcPosition(x.ACKIndex, Source1, chindex), CalcBitLenght(x.ACKLength, Source1, chindex))
                            {
                                Data = new Byte[] { x.ACK ? (Byte)1 : (Byte)0 },
                            };

                            packet.Success = x.ACK;
                            //packet.Data = new Byte[] { packet.Success?(byte)1: (byte)0 };
                            packets.Add(packet);
                            eventinfo.EventInofs[6] = (packet.Data, packet.BitCount);
                        }

                        if (x.HasEOF)
                        {
                            CANFDEOFDecodePacket packet = new CANFDEOFDecodePacket(CalcPosition(x.EOFIndex, Source1, chindex), CalcBitLenght(x.EOFLength, Source1, chindex))
                            {
                                Data = new Byte[] { x.EOF },
                            };
                            eventinfo.EventInofs[7] = (packet.Data, packet.BitCount);
                            packets.Add(packet);
                        }

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

        public Boolean CheckParityDataExist(Int32 actualStufCount, Byte stufCountdata, Boolean parity)
        {
            Int32 databitcount = 3;
            Byte actualparityremain = (Byte)(actualStufCount % 8);
            //格雷码转换
            Byte actualremaingraydata = BinaryToGray(actualparityremain, databitcount);

            Boolean actualpairtystuf = ParityData(actualremaingraydata);

            if ((actualremaingraydata == stufCountdata) && (actualpairtystuf == parity))
            {
                return true;
            }
            return false;
        }

        private Boolean ParityData(Byte data)
        {
            Int32 databitcount = _StuffByteCount;
            //格雷码转换
            //data = BinaryToGray(data, databitcount);
            Boolean temp = false;
            while (databitcount > 0)
            {
                temp ^= ((data & 0b01) == 1);
                data >>= 1;
                databitcount--;
            }
            return temp;// !temp;
        }

        private Byte BinaryToGray(Byte value, Int32 bitcount)
        {
            if (bitcount > 8 || bitcount < 0) throw new ArgumentOutOfRangeException(nameof(bitcount));
            if (bitcount == 1) return value;
            Byte gray = 0;
            gray = (Byte)(value & (1 << (bitcount - 1)));
            for (Int32 index = bitcount - 1; index > 0; index--)
            {
                gray |= (Byte)((((value >> index) & 0x01) ^ ((value >> (index - 1)) & 0x01)) << (index - 1));
            }
            return gray;
        }
        private Byte GetDLCByteCount(Byte dlc)
        {
            if (dlc <= 8) return dlc;
            switch (dlc)
            {
                case 0b1001:
                    return 12;
                case 0b1010:
                    return 16;
                case 0b1011:
                    return 20;
                case 0b1100:
                    return 24;
                case 0b1101:
                    return 32;
                case 0b1110:
                    return 48;
                case 0b1111:
                    return 64;
                default:
                    return dlc;
            }
        }
        private Int32 FindStartIndex(Int32 dataindex, Int32 bitdatacount, UInt32 datalen)
        {
            if (bitdatacount <= 0) return -1;
            if (dataindex + bitdatacount >= datalen) return -1;
            Int32 idlestartindex = -1;
            Boolean invert = SignalType == ProtocolCANFD.SignalType.CAN_FDH || SignalType == ProtocolCANFD.SignalType.Diff;
            //var starttime = DateTime.Now;
            var starttime = TimeSpanUtility.GetTimestampSpan();
            for (Int32 index = dataindex; index < datalen; index++)
            {
                if (idlestartindex == -1)
                {
                    if (DecodeDataHelper.Instance.GetLevel(BusId, index, SDAThreshold, Source1, invert))
                    {
                        idlestartindex = index;
                    }
                }
                else
                {
                    if (!DecodeDataHelper.Instance.GetLevel(BusId, index, SDAThreshold, Source1, invert))
                    {
                        if (index - idlestartindex < _MinEndFrameBitCount * bitdatacount)
                        {
                            idlestartindex = -1;
                        }
                        else return index;
                    }
                }
                if (( /*DateTime.Now*/TimeSpanUtility.GetTimestampSpan() - starttime).TotalMilliseconds > 2000)
                {
                    return -1;
                }
            }
            return -1;
        }
        //以起始位作为同步位，根据边沿脉宽计算实际位宽
        private Int32 CalBitDataCount(Int32 dataindex, Int32 bitdatacount, ref CancellationToken token, ref Boolean needclear)
        {
            Int32 startindex = DecodeDataHelper.Instance.FindLastEdge(BusId, dataindex, Source1, ref token, ref needclear);
            Int32 endindex = DecodeDataHelper.Instance.FindNextEdge(BusId, dataindex, Source1, ref token, ref needclear);
            Int32 datalength = endindex - startindex;

            Double datacount = datalength / (Double)bitdatacount;

            Int32 actualbitdatacount = datalength / (Int32)datacount;
            return actualbitdatacount;
        }

        private enum CRCType
        {
            CRC15,
            CRC17,
            CRC21,
        }
        private class CANFDCRC
        {
            private readonly UInt32 _CRC15Poly = 0x00_4599;
            private readonly UInt32 _CRC15Mask = 0x00_8000;
            private readonly UInt32 _CRC15MaxValue = (UInt32)Math.Pow(2, 15) - 1;

            private readonly UInt32 _CRC17Poly = 0x01_685B;
            private readonly UInt32 _CRC17Mask = 0x01_0000;
            private readonly UInt32 _CRC17MaxValue = (UInt32)Math.Pow(2, 17) - 1;

            private readonly UInt32 _CRC21Poly = 0x10_2899;
            private readonly UInt32 _CRC21Mask = 0x10_0000;
            private readonly UInt32 _CRC21MaxValue = (UInt32)Math.Pow(2, 21) - 1;
            private List<Boolean> _TempBits = new List<Boolean>();
            private List<Int32> _CRCStufIndexList = new List<Int32>();
            public CANFDCRC()
            {
            }
            public void Clear()
            {
                _TempBits.Clear();
                _CRCStufIndexList.Clear();
            }
            public void AddBit(Boolean bit) => _TempBits.Add(bit);
            public void AddStufIndex() => _CRCStufIndexList.Add(_TempBits.Count - 1);

            public UInt32 CalcCRC(CRCType type)
            {
                //UInt32 crc = 0;
                //if (_TempBits.Count == 0) return crc;
                //UInt32 poly = type == CRCType.CRC17 ? _CRC17Poly : _CRC21Poly;
                //UInt32 mask = type == CRCType.CRC17 ? _CRC17Mask : _CRC21Mask;
                //UInt32 maxvalue = type == CRCType.CRC17 ? _CRC17MaxValue : _CRC21MaxValue;
                //foreach (var val in _TempBits)
                //{
                //    if (((crc ^ (val ? mask : 0)) & mask) != 0)
                //    {
                //        crc ^= poly;
                //    }
                //    crc <<= 1;
                //    //crc &= maxvalue;
                //}
                //crc >>= 1;

                //return crc & maxvalue;

                UInt32 crc = 0;
                if (_TempBits.Count == 0) return crc;

                UInt32 poly = _CRC17Poly;
                UInt32 mask = _CRC17Mask;
                UInt32 resetVal = (UInt32)0x1fffe;
                UInt32 maxvalue = _CRC17MaxValue;
                Int32 shiftCnt = (Int32)16;
                switch (type)
                {
                    case CRCType.CRC15:
                        poly = _CRC15Poly;
                        mask = _CRC15Mask;
                        resetVal = (UInt32)0x7ffe;
                        maxvalue = _CRC15MaxValue;
                        shiftCnt = (Int32)14;
                        crc = 0;
                        break;
                    case CRCType.CRC17:
                        poly = _CRC17Poly;
                        mask = _CRC17Mask;
                        resetVal = (UInt32)0x1fffe;
                        maxvalue = _CRC17MaxValue;
                        shiftCnt = (Int32)16;
                        crc = mask;
                        break;
                    case CRCType.CRC21:
                        poly = _CRC21Poly;
                        mask = _CRC21Mask;
                        resetVal = (UInt32)0x1ffffe;
                        maxvalue = _CRC21MaxValue;
                        shiftCnt = (Int32)20;
                        crc = mask;
                        break;
                }

                Int32 crcindex = 0;
                foreach (var val in _TempBits)
                {
                    if (CRCType.CRC15 == type)
                    {
                        //填充位不做校验
                        if (_CRCStufIndexList.Contains(crcindex))
                        {
                            crcindex++;
                            continue;
                        }
                        if (((crc ^ (val ? mask : 0)) & mask) != 0)
                        {
                            crc ^= poly;
                        }
                        crc <<= 1;
                        crcindex++;
                        continue;
                    }
                    UInt32 crcnexbit = (crc >> shiftCnt) ^ (val ? (UInt32)1 : (UInt32)0);
                    crc <<= 1;
                    crc &= resetVal;
                    if (crcnexbit != 0 ? true : false)
                    {
                        crc ^= poly;
                    }
                }
                if (CRCType.CRC15 == type)
                {
                    crc = crc >> 1;
                }
                else
                {
                    crc &= maxvalue;
                }
                return crc;
            }
        }
        private class BitManger
        {
            private CANFDCRC _CRC = new CANFDCRC();
            private UInt32 _DataLen = 0;
            private CANFDDecodeModel _Parent;
            private readonly Int32 _MaxIdenticalBitCount = 5;
            private Boolean _LastState = false;
            private Int32 OffsetSamplePoint => BitDataCount * _Parent.SamplePoint / 100;
            private Int32 _Count = 0;
            private Int32 _StufCount = 0;
            [AllowNull] public ChannelId CurrentID { get; private set; }
            public Int32 BitDataCount { get; set; }
            public Int32 BitIndex { get; private set; }

            public Int32 StufCount
            {
                get { return _StufCount; }
            }

            internal BitManger(CANFDDecodeModel parent)
            {
                _Parent = parent;
            }
            public void Clear(ChannelId busId, ChannelId id, Int32 bitdatacount)
            {
                _CRC.Clear();
                CurrentID = id;
                BitDataCount = bitdatacount;
                _Count = 0;
                _StufCount = 0;
                DecodeDataHelper.Instance.TryGetPerChannelDataLength(busId, id, ref _DataLen);
            }

            public UInt32 CalcCRC(CRCType type) => _CRC.CalcCRC(type);
            public Boolean GetBit(ChannelId busId, Int32 dataindex, ref CancellationToken token, ref Boolean needclear, out Boolean success, Boolean check = true)
            {
                if (_Count == _MaxIdenticalBitCount)
                {
                    //填充位判断
                    Int32 tempnextstartdataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                    Int32 difdatacnt = tempnextstartdataindex - dataindex;
                    if (difdatacnt < BitDataCount / 4)
                    {
                        //若当前索引再边沿脉宽的边界处，则下一个脉宽为查找的值
                        dataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                    }
                    else
                    {
                        dataindex = DecodeDataHelper.Instance.FindLastEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                    }
                    if (dataindex == -1)
                    {
                        success = false;
                        return false;
                    }

                    _Count = 1;
                    Boolean tempstaus = DecodeDataHelper.Instance.GetLevel(busId, dataindex + OffsetSamplePoint / 2,
                                                                          _Parent.SDAThreshold,
                                                                          CurrentID,
                                                                          _Parent.SignalType == ProtocolCANFD.SignalType.CAN_FDH || _Parent.SignalType == ProtocolCANFD.SignalType.Diff);
                    //if(tempstaus != _LastState)
                    {
                        //填充位正确则填充计数加1
                        _StufCount++;
                    }
                    _LastState = tempstaus;
                    if (check)
                    {
                        _CRC.AddBit(tempstaus);
                        _CRC.AddStufIndex();
                    }
                    dataindex += BitDataCount;
                }
                Int32 tempnextedgestartindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                if (tempnextedgestartindex > 0)
                {
                    //若当前索引在边沿脉宽末尾，则当前索引应在下一个边沿开始
                    Int32 difCntFromNextEdge = tempnextedgestartindex - dataindex;
                    if (difCntFromNextEdge < BitDataCount / 10)
                    {
                        dataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                    }
                }
                BitIndex = dataindex;
                var state = GetPointBit(busId, dataindex, out success);
                if (!success) return false;
                if (state != _LastState)
                {
                    _Count = 1;
                    _LastState = state;
                }
                else _Count++;
                if (check) _CRC.AddBit(state);
                return _LastState;
            }
            public Boolean ProcessDataEndStufBit(ChannelId busId, Int32 dataindex, out Int32 len, ref CancellationToken token, ref Boolean needclear, out Boolean success, Boolean check = true)
            {
                len = 0;
                Int32 tempdataindex = dataindex;
                if (_Count == _MaxIdenticalBitCount)
                {
                    _Count -= 1;
                    ////填充位判断
                    //Int32 tempnextstartdataindex = DecodeDataHelper.Instance.FindNextEdge(dataindex, CurrentID, ref token, ref needclear);
                    //Int32 difdatacnt = tempnextstartdataindex - dataindex;
                    //if (difdatacnt < BitDataCount / 4)
                    //{
                    //    //若当前索引再边沿脉宽的边界处，则下一个脉宽为查找的值
                    //    dataindex = DecodeDataHelper.Instance.FindNextEdge(dataindex, CurrentID, ref token, ref needclear);
                    //}
                    //else
                    //{
                    //    dataindex = DecodeDataHelper.Instance.FindLastEdge(dataindex, CurrentID, ref token, ref needclear);
                    //}
                    //if (dataindex == -1)
                    //{
                    //    success = false;
                    //    return false;
                    //}

                    //_Count = 1;
                    //Boolean tempstaus = DecodeDataHelper.Instance.GetLevel(dataindex + OffsetSamplePoint / 2,
                    //                                                      _Parent.SDAThreshold,
                    //                                                      CurrentID,
                    //                                                      _Parent.SignalType == ProtocolCANFD.SignalType.CAN_FDH || _Parent.SignalType == ProtocolCANFD.SignalType.Diff);
                    ////if(tempstaus != _LastState)
                    //{
                    //    //填充位正确则填充计数加1
                    //    _StufCount++;
                    //}
                    //_LastState = tempstaus;
                    //if (check) _CRC.AddBit(tempstaus);
                    //dataindex += BitDataCount;
                    //len = dataindex - tempdataindex;
                }
                success = true;
                return true;
            }
            public void GetBits(ChannelId busId, ref UInt64 initialvalue, Int32 dataindex, Int32 bitcount, out Int32 len, out Boolean success, ref CancellationToken token, ref Boolean needclear, Boolean check = true)
            {
                if (bitcount > 64)
                    throw new ArgumentOutOfRangeException(nameof(bitcount) + "max value is 64");
                success = false;
                len = 0;
                for (Int32 bitindex = 0; bitindex < bitcount; bitindex++)
                {
                    if (_Count == _MaxIdenticalBitCount && check)
                    {
                        Int32 tempnextstartdataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                        Int32 difdatacnt = tempnextstartdataindex - dataindex;
                        if (difdatacnt < BitDataCount / 4)
                        {
                            dataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                        }
                        else
                        {
                            dataindex = DecodeDataHelper.Instance.FindLastEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                        }
                        if (dataindex == -1)
                        {
                            return;
                        }
                        _Count = 1;
                        Boolean tempstaus = DecodeDataHelper.Instance.GetLevel(busId, dataindex + OffsetSamplePoint / 2,
                                                                              _Parent.SDAThreshold,
                                                                              CurrentID,
                                                                              _Parent.SignalType == ProtocolCANFD.SignalType.CAN_FDH || _Parent.SignalType == ProtocolCANFD.SignalType.Diff);
                        //if (tempstaus != _LastState)
                        {
                            //填充位正确则填充计数加1
                            _StufCount++;
                        }
                        _LastState = tempstaus;
                        if (check)
                        {
                            _CRC.AddBit(tempstaus);
                            _CRC.AddStufIndex();
                        }
                        dataindex += BitDataCount;
                        //_CRCBits.Add(!_LastState);
                    }
                    Int32 tempnextedgestartindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                    if (tempnextedgestartindex > 0)
                    {
                        //若当前索引在边沿脉宽末尾，则当前索引应在下一个边沿开始
                        Int32 difCntFromNextEdge = tempnextedgestartindex - dataindex;
                        if (difCntFromNextEdge < BitDataCount / 10)
                        {
                            dataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                        }
                    }
                    if (bitindex == 0)
                    {
                        BitIndex = dataindex;
                    }
                    var state = GetPointBit(busId, dataindex, out success);
                    if (!success)
                    {
                        return;
                    }
                    if (state != _LastState)
                    {
                        _LastState = state;
                        _Count = 1;
                    }
                    else _Count++;
                    if (check) _CRC.AddBit(state);
                    dataindex += BitDataCount;
                    initialvalue <<= 1;
                    initialvalue |= (UInt32)(state ? 1 : 0);
                }
                success = true;
                len = dataindex - BitIndex;
            }
            public UInt32 GetCRC(ChannelId busId, Int32 dataindex, Int32 bitcount, out Int32 len, out Boolean success, ref CancellationToken token, ref Boolean needclear)
            {
                if (bitcount > 64)
                    throw new ArgumentOutOfRangeException(nameof(bitcount) + "max value is 64");
                success = false;
                len = 0;
                UInt32 val = 0;
                Int32 identcount = 4;
                for (Int32 bitindex = 0; bitindex < bitcount; bitindex++)
                {
                    //if ((bitindex % identcount) == 0 && bitindex > 0)
                    //{
                    //    Boolean tempstaus = DecodeDataHelper.Instance.GetLevel(dataindex, _Parent.SDAThreshold, 
                    //        CurrentID, _Parent.SignalType == ProtocolCANFD.SignalType.CAN_FDH || 
                    //        _Parent.SignalType == ProtocolCANFD.SignalType.Diff);
                    //    if (tempstaus == _LastState)
                    //    {
                    //        dataindex = DecodeDataHelper.Instance.FindNextEdge(dataindex, CurrentID, ref token, ref needclear);
                    //    }
                    //    else
                    //    {
                    //        dataindex = DecodeDataHelper.Instance.FindLastEdge(dataindex, CurrentID, ref token, ref needclear);
                    //    }
                    //    dataindex += BitDataCount;
                    //    if (dataindex >= _DataLen)
                    //    {
                    //        return val;
                    //    }
                    //}
                    if (bitindex == 0)
                    {
                        BitIndex = dataindex;
                    }

                    if ((bitindex % identcount) == 0 && bitindex > 0)
                    {
                        Int32 tempnextstartdataindex1 = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                        Int32 difdatacnt1 = tempnextstartdataindex1 - dataindex;
                        if (difdatacnt1 < BitDataCount / 10)
                        {
                            dataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                        }
                        var stateTemp = GetPointBit(busId, dataindex, out success);
                        dataindex += BitDataCount;
                        if (dataindex >= _DataLen)
                        {
                            return val;
                        }
                    }
                    Int32 tempnextstartdataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                    Int32 difdatacnt = tempnextstartdataindex - dataindex;
                    if (difdatacnt < BitDataCount / 10)
                    {
                        dataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                    }

                    var state = GetPointBit(busId, dataindex, out success);
                    if (!success)
                    {
                        return val;
                    }
                    _LastState = state;
                    dataindex += BitDataCount;
                    val <<= 1;
                    val |= (UInt32)(state ? 1 : 0);
                }

                len = dataindex - BitIndex;
                success = len > 0;
                return val;
            }
            public UInt64 GetBits(ChannelId busId, Int32 dataindex, Int32 bitcount, out Int32 len, out Boolean success, ref CancellationToken token, ref Boolean needclear, Boolean check = true)
            {
                if (bitcount > 64)
                    throw new ArgumentOutOfRangeException(nameof(bitcount) + "max value is 64");
                success = false;
                len = 0;
                UInt64 val = 0;
                for (Int32 bitindex = 0; bitindex < bitcount; bitindex++)
                {
                    if (_Count == _MaxIdenticalBitCount && check)
                    {
                        Int32 tempnextstartdataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                        Int32 difdatacnt = tempnextstartdataindex - dataindex;
                        if (difdatacnt < BitDataCount / 4)
                        {
                            dataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                        }
                        else
                        {
                            dataindex = DecodeDataHelper.Instance.FindLastEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                        }
                        if (dataindex == -1)
                        {
                            return val;
                        }
                        _Count = 1;
                        Boolean tempstaus = DecodeDataHelper.Instance.GetLevel(busId, dataindex + OffsetSamplePoint / 2,
                                                                              _Parent.SDAThreshold,
                                                                              CurrentID,
                                                                              _Parent.SignalType == ProtocolCANFD.SignalType.CAN_FDH || _Parent.SignalType == ProtocolCANFD.SignalType.Diff);
                        //if (tempstaus != _LastState)
                        {
                            //填充位正确则填充计数加1
                            _StufCount++;
                        }
                        _LastState = tempstaus;
                        if (check)
                        {
                            _CRC.AddBit(tempstaus);
                            _CRC.AddStufIndex();
                        }
                        dataindex += BitDataCount;
                    }
                    Int32 tempnextedgestartindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                    if (tempnextedgestartindex > 0)
                    {
                        //若当前索引在边沿脉宽末尾，则当前索引应在下一个边沿开始
                        Int32 difCntFromNextEdge = tempnextedgestartindex - dataindex;
                        if (difCntFromNextEdge < BitDataCount / 10)
                        {
                            dataindex = DecodeDataHelper.Instance.FindNextEdge(busId, dataindex, CurrentID, ref token, ref needclear);
                        }
                    }
                    if (bitindex == 0)
                    {
                        BitIndex = dataindex;
                    }
                    var state = GetPointBit(busId, dataindex, out success);
                    if (!success)
                    {
                        return val;
                    }
                    if (state != _LastState)
                    {
                        _LastState = state;
                        _Count = 1;
                    }
                    else _Count++;
                    if (check) _CRC.AddBit(state);
                    dataindex += BitDataCount;
                    val <<= 1;
                    val |= (UInt32)(state ? 1 : 0);
                }

                len = dataindex - BitIndex;
                success = len > 0;
                return val;
            }
            private Boolean GetPointBit(ChannelId busId, Int32 startindex, out Boolean success)
            {
                success = false;
                Int32 sampleindex = startindex + OffsetSamplePoint;
                if (sampleindex >= _DataLen)
                {
                    success = false;
                    return false;
                }
                Boolean invert = _Parent.SignalType == ProtocolCANFD.SignalType.CAN_FDH || _Parent.SignalType == ProtocolCANFD.SignalType.Diff;

                if (_DataLen <= sampleindex) return false;
                Boolean result = DecodeDataHelper.Instance.GetLevel(busId, sampleindex, _Parent.SDAThreshold, CurrentID, invert);
                success = true;
                return result;
            }


        }

    }
}

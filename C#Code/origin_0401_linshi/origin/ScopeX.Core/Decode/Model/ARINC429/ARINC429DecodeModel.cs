using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NPOI.POIFS.Properties;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;

namespace ScopeX.Core.Decode
{
    [Obsolete("Please Use Class 'ARINC429DecodeModelCPP'", true)]
    internal sealed partial class ARINC429DecodeModel :ProtocolModel
    {
        private const Int32 _LabelBitCount = 8;
        private const Int32 _SSMBitCount = 2;
        private const Int32 _SDIBitCount = 2;
        private const Int32 _DataBitCount = 19;
        private const Int32 _MinGapCount = 4;

        private List<Arinc429PacketInfo> _PacketInfos = new List<Arinc429PacketInfo>();

        private DecodeResultData _ResultData = new DecodeResultData();
        public ARINC429DecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.ARINC429, isTrigDecode)
        {
            _ResultData.Name = ProtocolType.ToString();
        }


        public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>()
        {
            "Index",
            "Start Time",
            "Label",
            "SDI",
            "Data",
            "SSM",
            "Parity",
            "Error",
        };


        public override Double BitRateByPs => 1f / CustomBaud * 1E12;

        private ChannelId _Source1 = ChannelId.C1;

        public ChannelId Source1
        {
            get { return _Source1; }
            set { UpdateProperty(ref _Source1, value); }
        }
        private Double _ThresholdH = 1;

        public Double ThresholdH
        {
            get { return _ThresholdH * TryGetChannelGain(Source1); }
            set { UpdateProperty(ref _ThresholdH, value/ TryGetChannelGain(Source1)); }
        }

        public String UnitH => GetChannelUnit(Source1);

        private Double _ThresholdL;

        public Double ThresholdL
        {
            get { return _ThresholdL* TryGetChannelGain(Source1); }
            set { UpdateProperty(ref _ThresholdL, value/ TryGetChannelGain(Source1)); }
        }

        public String UnitL=> GetChannelUnit(Source2);

        public Double MaxThreshold => (Single)(
            20 * TryGetChannelGain(Source1));


        public Double MinThreshold => -MaxThreshold;
        private ProtocolARINC429.DecodeMode _DecodeMode = ProtocolARINC429.DecodeMode.Mode_8_2_19_1;
        public ProtocolARINC429.DecodeMode DecodeMode
        {
            get { return _DecodeMode; }
            set { UpdateProperty(ref _DecodeMode, value); }
        }


        private ProtocolARINC429.SignalRate _SignalRate = ProtocolARINC429.SignalRate.SignalRate_custom;
        public ProtocolARINC429.SignalRate SignalRate
        {
            get { return _SignalRate; }
            set
            {
                if (value != _SignalRate)
                {
                    if (ProtocolARINC429.SignalRate.SignalRate_custom == value)
                    {
                        UpdateProperty(ref _SignalRate, value);
                    }
                    _SignalRate = value;
                    switch (value)
                    {
                        case ProtocolARINC429.SignalRate.SignalRate_100k:
                            CustomBaud = 100_000;
                            break;
                        case ProtocolARINC429.SignalRate.SignalRate_12_5k:
                            CustomBaud = 12500;
                            break;
						OnPropertyChanged();
                    }
                }
            }
        }

        public Int32 MaxBaud => 10_000_000;
        public Int32 MinBaud => 1000;
        private Int32 _CustomBaud = 100_000;
        public Int32 CustomBaud //自定义速率
        {
            get { return _CustomBaud; }
            set
            {
                if (value != _CustomBaud)
                {
                    switch (value)
                    {
                        case 100_000:
                            SignalRate = ProtocolARINC429.SignalRate.SignalRate_100k;
                            break;
                        case 12500:
                            SignalRate = ProtocolARINC429.SignalRate.SignalRate_12_5k;
                            break;
                        default:
                            SignalRate = ProtocolARINC429.SignalRate.SignalRate_custom;
                            break;
                    }
                    UpdateProperty(ref _CustomBaud, value);
                }
            }
        }
        private ChannelId _Source2 = ChannelId.C2;

        public ChannelId Source2
        {
            get { return _Source2; }
            set { UpdateProperty(ref _Source2, value); }
        }

        private ProtocolCommon.Polarity _Polarity = ProtocolCommon.Polarity.Positive;
        public ProtocolCommon.Polarity Polarity
        {
            get { return _Polarity; }
            set { UpdateProperty(ref _Polarity, value); }
        }

        private ProtocolARINC429.InputMode _InputMode;

        public ProtocolARINC429.InputMode InputMode
        {
            get { return _InputMode; }
            set { UpdateProperty(ref _InputMode, value); }
        }
        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolARIN429Options()
            {
                Baud = CustomBaud,
                DecodeMode = DecodeMode,
                SignalInputA = Source1,
                SignalInputB = Source2,
                ThresholdH = ThresholdH,
                ThresholdL = ThresholdL,
                InputMode = InputMode,
            };
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

        public override void UpdateReferenceDataStatus()
        {
            if (_Source1.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].Channels != null
                && DecodeDataSource.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].Channels[0] == _Source1)
            {
                DecodeDataSource.Instance.ReferenceDataSource[_Source1 - ChannelIdExt.MinRChId].HasData = false;
            }
        }

        internal override Boolean SourceHasData()
        {
            if (DsoPrsnt.DefaultDsoPrsnt == null)
                return false;
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(_Source1, out var prsnt);
            if (prsnt == null)
                return false;

            Double[] thresholds;
            thresholds = new Double[2];
            thresholds[0] = ThresholdH;
            thresholds[1] = ThresholdL;

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
        private Boolean ParityData(Arinc429PacketInfo packetInfo)
        {
            UInt32 count = 0;
            if (System.Runtime.Intrinsics.X86.Popcnt.IsSupported)
            {
                if (packetInfo.HasLabel)
                {
                    count += System.Runtime.Intrinsics.X86.Popcnt.PopCount(packetInfo.Label);
                }
                if (packetInfo.HasSDI)
                {
                    count += System.Runtime.Intrinsics.X86.Popcnt.PopCount(packetInfo.SDI);
                }

                if (packetInfo.HasSSM)
                {
                    count += System.Runtime.Intrinsics.X86.Popcnt.PopCount(packetInfo.SSM);
                }
                if (packetInfo.HasData)
                {
                    count += System.Runtime.Intrinsics.X86.Popcnt.PopCount(packetInfo.TempData);
                }
            }
            else
            {
                if (packetInfo.HasLabel)
                {
                    for (Int32 index = 0; index < packetInfo.LabelBitCount; index++)
                    {
                        if (((packetInfo.Label >> index) & 0x01) == 0x01)
                            count++;
                    }
                }
                if (packetInfo.HasSDI)
                {
                    for (Int32 index = 0; index < packetInfo.SDIBitCount; index++)
                    {
                        if (((packetInfo.SDI >> index) & 0x01) == 0x01)
                            count++;
                    }
                }

                if (packetInfo.HasSSM)
                {
                    for (Int32 index = 0; index < packetInfo.SSMBitCount; index++)
                    {
                        if (((packetInfo.SSM >> index) & 0x01) == 0x01)
                            count++;
                    }
                }
                if (packetInfo.HasData)
                {
                    for (Int32 index = 0; index < packetInfo.DataBitCount; index++)
                    {
                        if (((packetInfo.TempData >> index) & 0x01) == 0x01)
                            count++;
                    }
                }
            }
            return count % 2 == 0;
        }

        private Boolean CheckZeroNodeLevel(ref ThreeLevelEdgeInfo node, Int32 edgecount)
        {
            ThreeLevelEdgeInfo? zeroNode = node.GetEdgeInfoByIndex((Int32)(node.StartIndex + 1.5 * edgecount))as ThreeLevelEdgeInfo;//当前边沿脉宽中心位置索引处获取下一个节点
            if (zeroNode == null)
            {
                return true;
            }
            if (zeroNode.CurrentLevel != ThreeLevelEdgeInfo.Status.Middle)
            {
                return false;
            }
            return true;
        }

        private Boolean GetEndExtraData(ref ThreeLevelEdgeInfo node, ref Int32 bitDataCount, out Int32 extraStartIndex, out Int32 extraDataLen, out Byte extradata, out Int32 extraDataBitCount)
        {
            ThreeLevelEdgeInfo extradatanode = node;
            extraStartIndex = 0;
            extraDataLen = 0;
            extradata = 0;
            extraDataBitCount = 0;
            if (node == null)
            {
                return false;
            }
            Int32 dataindex = 0;
            extraStartIndex = extradatanode.StartIndex;
            while (extradatanode.Length < bitDataCount)
            {
                Boolean state = extradatanode.CurrentLevel == ThreeLevelEdgeInfo.Status.High;
                state = (Polarity == ProtocolCommon.Polarity.Positive) ? state : !state;
                extradata |= (Byte)((state ? 1 : 0) << (7 - extraDataBitCount));
                extraDataBitCount++;
                extradatanode = extradatanode.Child as ThreeLevelEdgeInfo;
                if (null == extradatanode) 
                {
                    break;
                }
                //TODO:一般校验数据位后面存在的多余数据位个数不会很大，因此现在默认最多8个
                if (extraDataBitCount >= 8)
                {
                    break;
                }
                dataindex = dataindex + (Int32)(bitDataCount / 2 * 2.5);
                extradatanode = extradatanode.GetEdgeInfoByIndex(dataindex) as ThreeLevelEdgeInfo;
                if ((null == extradatanode)|| (extradatanode.Length > bitDataCount))
                {
                    break;
                }
                dataindex = extradatanode.StartIndex;
            }
            extraDataLen = dataindex - extraStartIndex;
            return true;
        }

        internal override void ParsingData(ref CancellationToken token)
        {
            Boolean needclear = false;
            Int32 clindex = GetChIndex(Source1);
            UInt32 datalen = 0;
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, Source1, ref datalen);
            Double samplerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, Source1, ref samplerate);
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Source1, out var cp);
            if (clindex == -1 || datalen == 0 || samplerate == 0||(cp!=null&&!cp.Active))
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _PacketInfos.Clear();
            }
            if (_NeedDecodeData)
            {
                ThreeLevelEdgeInfo? node = DecodeDataHelper.Instance.GetThreeLevelEdgeInfo(BusId, 0, Source1, ref token, ref needclear) as ThreeLevelEdgeInfo;
                Int32 bitdatacount = (Int32)(Math.Round(1d / CustomBaud * samplerate, 0));
                _PacketInfos.Clear();
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                Boolean hasssm = true;
                Boolean hassdi = true;
                Int32 databitcount = _DataBitCount;
                Int32 dataindex = 0;

                switch (DecodeMode)
                {
                    case ProtocolARINC429.DecodeMode.Mode_8_2_19_1:
                        hasssm = true;
                        hassdi = true;
                        break;
                    case ProtocolARINC429.DecodeMode.Mode_8_23_1:
                        hasssm = false;
                        hassdi = false;
                        databitcount = _DataBitCount + _SDIBitCount + _SSMBitCount;
                        break;
                    case ProtocolARINC429.DecodeMode.Mode_8_21_2_1:
                        hasssm = true;
                        hassdi = false;
                        databitcount = _DataBitCount + _SDIBitCount;
                        break;
                    default:
                        break;
                }
                if (node != null && samplerate > 0 && bitdatacount > 0)
                {
                    while (true)
                    {
                        if (node == null)
                            break;
                        Arinc429PacketInfo packetinfo = new Arinc429PacketInfo();
                        if (bitdatacount <= 0)
                            break;
                        //帧间间隔判断，可作为起始符判断标志
                        if ((node.CurrentLevel == ThreeLevelEdgeInfo.Status.Middle) &&(node.EndIndex - node.StartIndex - bitdatacount / 2) > bitdatacount * 2)
                        {
                            if ((node.Parent != null) && (node.EndIndex - node.StartIndex - bitdatacount / 2) < (Double)(bitdatacount * 3.5))
                            {
                                //由于不知道数据起始处帧间隙的具体长度，可不判断
                                packetinfo.HasGap = true;
                                packetinfo.GapIndex = node.StartIndex + bitdatacount / 2;
                                packetinfo.GapLength = node.EndIndex - node.StartIndex - bitdatacount / 2;//需排除上一个数据位的归零数据宽度
                                packetinfo.Gap = true;
                            }
                            node = node.Child as ThreeLevelEdgeInfo; //获取帧的起始节点
                            if (node == null)
                                break;
                            packetinfo.SOFIndex = node.StartIndex;
                            packetinfo.HasSOF = true;

                            packetinfo.HasLabel = true;
                            packetinfo.LabelIndex = node.StartIndex;
                            packetinfo.LabelSuccessDataBitCount = _LabelBitCount;
                            dataindex = packetinfo.LabelIndex;
                        }

                        if (packetinfo.HasLabel)
                        {
                            packetinfo.SuccessLabel = true;
                            for (Int32 index = 0; index < _LabelBitCount; index++)
                            {
                                if (node == null)
                                    break;
                                dataindex = node.StartIndex;
                                Boolean state = node.CurrentLevel == ThreeLevelEdgeInfo.Status.High;
                                state = (Polarity == ProtocolCommon.Polarity.Positive) ? state : !state;
                                packetinfo.Label |= (Byte)((state ? 1 : 0) << (_LabelBitCount - index - 1));
                                if(!CheckZeroNodeLevel(ref node, bitdatacount / 2))
                                {
                                    packetinfo.SuccessLabel = false;
                                    //考虑了归零电平错误值,比如高电平、低电平值，因此错误边沿脉宽的宽度*2
                                   // node = node.Child as ThreeLevelEdgeInfo;
                                }
                                dataindex = dataindex + (Int32)(bitdatacount/2 * 2.5);
                                node = node.GetEdgeInfoByIndex(dataindex) as ThreeLevelEdgeInfo;
                                packetinfo.LabelBitCount++;
                            }
                            if (node == null)
                            {
                                packetinfo.LabelLength = (Int32)datalen - packetinfo.LabelIndex;
                                _PacketInfos.Add(packetinfo);
                                break;
                            }
                            packetinfo.LabelLength = node.StartIndex - packetinfo.LabelIndex; //用下一个节点开始的位置  减去Label起点得到Label的总长
                            if (hassdi)
                            {
                                packetinfo.HasSDI = true;
                                packetinfo.SDIIndex = node.StartIndex;
                                packetinfo.SDISuccessDataBitCount = _SDIBitCount;
                                packetinfo.SuccessSDI = true;
                                for (Int32 index = 0; index < _SDIBitCount; index++)
                                {
                                    if (node == null)
                                        break;
                                    dataindex = node.StartIndex;
                                    Boolean state = node.CurrentLevel == ThreeLevelEdgeInfo.Status.High;
                                    state = (Polarity == ProtocolCommon.Polarity.Positive) ? state : !state;
                                    packetinfo.SDI |= (Byte)((state ? 1 : 0) << index);
                                    if (!CheckZeroNodeLevel(ref node, bitdatacount / 2))
                                    {
                                        packetinfo.SuccessSDI = false;
                                    }
                                    dataindex = dataindex + (Int32)(bitdatacount / 2 * 2.5);
                                    node = node.GetEdgeInfoByIndex(dataindex) as ThreeLevelEdgeInfo;
                                    packetinfo.SDIBitCount++;
                                }
                                if (node == null)
                                {
                                    packetinfo.SDILength = (Int32)datalen - packetinfo.SDIIndex;
                                    _PacketInfos.Add(packetinfo);
                                    break;
                                }
                                packetinfo.SDILength = node.StartIndex - packetinfo.SDIIndex;
                            }
                            packetinfo.HasData = true;
                            packetinfo.DataIndex = node.StartIndex;
                            packetinfo.DataSuccessDataBitCount = databitcount;
                            packetinfo.TempData = 0;
                            packetinfo.SuccessData = true;
                            for (Int32 index = 0; index < databitcount; index++)
                            {
                                if (node == null)
                                    break;
                                dataindex = node.StartIndex;
                                Boolean state = node.CurrentLevel == ThreeLevelEdgeInfo.Status.High;
                                state = (Polarity == ProtocolCommon.Polarity.Positive) ? state : !state;
                                packetinfo.TempData |= (UInt32)((state ? 1 : 0) << index);
                                if (!CheckZeroNodeLevel(ref node, bitdatacount / 2))
                                {
                                    packetinfo.SuccessData = false;
                                }
                                dataindex = dataindex + (Int32)(bitdatacount / 2 * 2.5);
                                node = node.GetEdgeInfoByIndex(dataindex) as ThreeLevelEdgeInfo;
                                packetinfo.DataBitCount++;
                            }
                            packetinfo.Data = BitConverter.GetBytes(packetinfo.TempData).Take((Int32)Math.Ceiling(_DataBitCount / 8f)).Reverse().ToArray();
                            if (node == null)
                            {
                                packetinfo.DataLength = (Int32)datalen - packetinfo.DataIndex;
                                _PacketInfos.Add(packetinfo);
                                break;
                            }
                            packetinfo.DataLength = node.StartIndex - packetinfo.DataIndex;

                            if (hasssm)
                            {
                                packetinfo.HasSSM = true;
                                packetinfo.SSMIndex = node.StartIndex;
                                packetinfo.SSMBitCount = _SSMBitCount;
                                packetinfo.SuccessSSM = true;
                                for (Int32 index = 0; index < _SSMBitCount; index++)
                                {
                                    if (node == null)
                                        break;
                                    dataindex = node.StartIndex;
                                    Boolean state = node.CurrentLevel == ThreeLevelEdgeInfo.Status.High;
                                    state = (Polarity == ProtocolCommon.Polarity.Positive) ? state : !state;
                                    packetinfo.SSM |= (Byte)((state ? 1 : 0) << index);
                                    if (!CheckZeroNodeLevel(ref node, bitdatacount / 2))
                                    {
                                        packetinfo.SuccessSSM = false;
                                    }
                                    dataindex = dataindex + (Int32)(bitdatacount / 2 * 2.5);
                                    node = node.GetEdgeInfoByIndex(dataindex) as ThreeLevelEdgeInfo;
                                    packetinfo.SSMBitCount++;
                                }
                                if (node == null)
                                {
                                    packetinfo.SSMLength = _SSMBitCount * bitdatacount;
                                    _PacketInfos.Add(packetinfo);
                                    break;
                                }
                                packetinfo.SSMLength = _SSMBitCount * bitdatacount;
                            }
                            packetinfo.HasParity = true;
                            Boolean statetemp = node.CurrentLevel == ThreeLevelEdgeInfo.Status.High;
                            packetinfo.Parity = (Polarity == ProtocolCommon.Polarity.Positive) ? statetemp : !statetemp;
                            packetinfo.ParityIndex = node.StartIndex;
                            packetinfo.SuccessParity = ParityData(packetinfo);
                            if (node == null)
                            {
                                packetinfo.ParityLength = (Int32)datalen - packetinfo.ParityIndex;
                                _PacketInfos.Add(packetinfo);
                                break;
                            }
                            if (!CheckZeroNodeLevel(ref node, bitdatacount / 2))
                            {
                                packetinfo.Parity = false;
                            }
                             //_PacketInfo.ParityLength = Math.Min((node.EndIndex - _PacketInfo.ParityIndex) * 2, (Int32)datalen - _PacketInfo.ParityIndex);
                            packetinfo.ParityLength = bitdatacount;
                            packetinfo.ParityLength = (packetinfo.ParityIndex + packetinfo.ParityLength > datalen) ? (Int32)(datalen - packetinfo.ParityIndex) : packetinfo.ParityLength;
                            packetinfo.HasEOF = true;
                            packetinfo.EOFIndex = packetinfo.ParityIndex + packetinfo.ParityLength;

                            //结尾错误数据解析
                            dataindex = dataindex + (Int32)(bitdatacount / 2 * 2.5);
                            ThreeLevelEdgeInfo? endzeronode = node.GetEdgeInfoByIndex(dataindex) as ThreeLevelEdgeInfo;
                            //ThreeLevelEdgeInfo? endZeroNode = node.Child as ThreeLevelEdgeInfo;
                            if (endzeronode != null && endzeronode.Length < bitdatacount)
                            {
                                //则说明有额外的数据
                                if (GetEndExtraData(ref endzeronode, ref bitdatacount, out Int32 extraStartIndex, out Int32 extraDataLen, out Byte extrData, out  Int32 extraDataBitcount))
                                {
                                    packetinfo.ParityLength = endzeronode.EndIndex - packetinfo.ParityIndex + 1;
                                    packetinfo.HasExtraBit = true;
                                    packetinfo.ExtraBitIndex = extraStartIndex;
                                    packetinfo.ExtraDataBitCount = extraDataBitcount;
                                    packetinfo.ExtraBitLen = extraDataLen;
                                    packetinfo.ExtraData = extrData;
                                    packetinfo.EOFIndex = packetinfo.ExtraBitIndex + extraDataLen;
                                }
                            }
                          _PacketInfos.Add(packetinfo);
                        }
                        node = node.Child as ThreeLevelEdgeInfo;
                    }

                }
            }
            if (_NeedUpdateViewInfo)
            {
                _NeedUpdateViewInfo = false;
                var buffer = GetDecodeBuffer();
                buffer.Clear();
                _EventInfos.Clear();
                if (_PacketInfos.Count == 0)
                {
                    _ResultData.DecodeViewInfos = new IDecodeViewInfo[0];
                    buffer.Add(_ResultData);
                    ChangeBuffer();
                    return;
                }
                _ResultData.DecodeViewInfos = _PacketInfos.SelectMany(x =>
                {
                    List<ARINC429DecodePacket> packets = new List<ARINC429DecodePacket>();
                    var endindex = 0;
                    if (x.HasGap && x.Gap)
                    {
                        packets.Add(new ARINC429GapDecodePacket(CalcPosition(x.GapIndex, Source1, clindex), CalcBitLenght(x.GapLength, Source1, clindex)));
                        _EventInfos.Add(new ProtocolEventInfo()
                        {
                            Index = _EventInfos.Count,
                        });
                        _EventInfos[^1].EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                        _EventInfos[^1].EventInofs[0] = (Encoding.Default.GetBytes("GAP"), 0);
                        _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes(("Gap: Gap Error")), 0);
                        _EventInfos[^1].StartTimeByPs = GetTimeFromPosition(packets.OrderBy(x => x.Start).First().Start, clindex);
                        _EventInfos[^1].StartPosition = packets.OrderBy(x => x.Start).First().Start;
                        endindex = x.GapIndex;
                    }

                    _EventInfos.Add(new ProtocolEventInfo()
                    {
                        Index = _EventInfos.Count,
                    });
                    _EventInfos[^1].EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));

                    if (x.HasSOF)
                    {
                        packets.Add(new ARINC429SOFDecodePacket(CalcPosition(x.SOFIndex, Source1, clindex)));
                        endindex = x.SOFIndex;
                    }
                    if (x.HasLabel)
                    {
                        packets.Add(new ARINC429LABELDecodePacket(CalcPosition(x.LabelIndex, Source1, clindex), CalcBitLenght(x.LabelLength, Source1, clindex))
                        {
                            Success = x.SuccessLabel,
                            BitCount = (UInt32)x.LabelBitCount,
                            Data = new Byte[] { x.Label },
                        });
                        _EventInfos[^1].EventInofs[0] = (packets[^1].Data, packets[^1].BitCount);
                        if (!x.SuccessLabel)
                        {
                            _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes((packets[^1].ErrorInfo)), 0);
                        }
                        endindex = x.LabelIndex+x.LabelLength;
                    }

                    if (x.HasSDI)
                    {
                        packets.Add(new ARINC429SDIDecodePacket(CalcPosition(x.SDIIndex, Source1, clindex), CalcBitLenght(x.SDILength, Source1, clindex))
                        {
                            Success = x.SuccessSDI,
                            BitCount = (UInt32)x.SDIBitCount,
                            Data = new Byte[] { x.SDI },
                        });
                        _EventInfos[^1].EventInofs[1] = (packets[^1].Data, packets[^1].BitCount);
                        if (!x.SuccessSDI)
                        {
                            _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes((packets[^1].ErrorInfo)), 0);
                        }
                        endindex = x.SDIIndex + x.SDILength;
                    }
                    if (x.HasData)
                    {
                        packets.Add(new ARINC429DataDecodePacket(CalcPosition(x.DataIndex, Source1, clindex), CalcBitLenght(x.DataLength, Source1, clindex))
                        {
                            Success = x.SuccessData,
                            BitCount = (UInt32)x.DataSuccessDataBitCount,
                            Data = x.Data,
                        });
                        _EventInfos[^1].EventInofs[2] = (packets[^1].Data, packets[^1].BitCount);
                        if (!x.SuccessData)
                        {
                            _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes((packets[^1].ErrorInfo)), 0);
                        }
                        endindex = x.DataIndex + x.DataLength;
                    }
                    if (x.HasSSM)
                    {
                        packets.Add(new ARINC429SSMDecodePacket(CalcPosition(x.SSMIndex, Source1, clindex), CalcBitLenght(x.SSMLength, Source1, clindex))
                        {
                            Success = x.SuccessSSM,
                            BitCount = (UInt32)x.SSMBitCount,
                            Data = new Byte[] { x.SSM },
                        });
                        _EventInfos[^1].EventInofs[3] = (packets[^1].Data, packets[^1].BitCount);
                        if (!x.SuccessSSM)
                        {
                            _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes((packets[^1].ErrorInfo)), 0);
                        }
                        endindex = x.SSMIndex + x.SSMLength;
                    }
                    if (x.HasParity)
                    {
                        packets.Add(new ARINC429ParityDeocdePacket(CalcPosition(x.ParityIndex, Source1, clindex), CalcBitLenght(x.ParityLength, Source1, clindex))
                        {
                            Parity = x.Parity,
                            SuccessParity = x.SuccessParity,
                        });
                        _EventInfos[^1].EventInofs[4] = (packets[^1].Data, packets[^1].BitCount);
                        if (x.Parity != x.SuccessParity)
                        {
                            _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes("Parity Error"), 0);
                        }
                        endindex = x.ParityIndex + x.ParityLength;
                    }
                    if (x.HasExtraBit)
                    {
                        ARINC429DataDecodePacket extraPacket = new ARINC429DataDecodePacket(CalcPosition(x.ExtraBitIndex, Source1, clindex), CalcBitLenght(x.ExtraBitLen, Source1, clindex))
                        {
                            Success = false,
                            BitCount = (UInt32)x.ExtraDataBitCount,
                            Data = new Byte[1],
                        };
                        extraPacket.Data[0] = x.ExtraData;
                        packets.Add(extraPacket);
                        _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes("Extra bits:too many bits"), 0);
                        endindex = x.ExtraBitIndex + x.ExtraBitLen;
                    }
                    if (x.HasEOF)
                    {
                        packets.Add(new ARINC429EOFDecodePacket(CalcPosition(x.EOFIndex, Source1, clindex)));
                        endindex = x.EOFIndex;
                    }

                    if (packets.Count > 0)
                    {
                        _EventInfos[^1].StartTimeByPs = GetTimeFromPosition(packets.OrderBy(x => x.Start).First().Start, clindex);
                    }
                    _EventInfos[^1].EndPosition = CalcPosition(endindex, Source1, clindex);
                    _EventInfos[^1].EndTimeByPs = GetTimeFromPosition(_EventInfos[^1].EndPosition, clindex);
                    return packets;
                }).OrderBy(x => x.Start).ToArray();
                buffer.Add(_ResultData);
                ChangeBuffer();
            }
        }
    }
}

using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using static ScopeX.Core.Decode.EdgeInfoCPP;

namespace ScopeX.Core.Decode
{
    internal sealed partial class ARINC429DecodeModelCPP : ProtocolModel
    {
        private const Int32 HASDATATRUE = 1;
        private List<Arinc429EventCPP> _PacketInfos = new List<Arinc429EventCPP>();
        private List<PAM3EdgePulse> _EdgePulsesList = new List<PAM3EdgePulse>();
        private DecodeResultData _ResultData = new DecodeResultData();
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
            set { UpdateProperty(ref _ThresholdH, value / TryGetChannelGain(Source1)); }
        }

        public String UnitH => GetChannelUnit(Source1);

        private Double _ThresholdL;
        public Double ThresholdL
        {
            get { return _ThresholdL * TryGetChannelGain(Source1); }
            set { UpdateProperty(ref _ThresholdL, value / TryGetChannelGain(Source1)); }
        }

        public String UnitL => GetChannelUnit(Source1);

        public Double MaxThreshold => (Single)(20 * TryGetChannelGain(Source1));


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

        public ARINC429DecodeModelCPP(ChannelId id, Boolean isTrigDecode = false) : base(id, SerialProtocolType.ARINC429, isTrigDecode)
        {
            _ResultData.Name = ProtocolType.ToString();
        }


        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolARIN429Options()
            {
                Baud = CustomBaud,
                DecodeMode = DecodeMode,
                SignalInputA = Source1,
                SignalInputB = Source2,
                ThresholdH = _ThresholdH,
                ThresholdL = _ThresholdL,
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

        internal override void ParsingData(ref CancellationToken token)
        {
            Boolean needclear = false;
            Int32 clindex = GetChIndex(Source1);
            UInt32 datalen = 0;
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, Source1, ref datalen);
            Double samplerate = 0;
            ARINC429Result decoderesult = new ARINC429Result();
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, Source1, ref samplerate);
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Source1, out var cp);
            ThreeLevelEdgeInfo? node = null;
            if (MoreThanStorage() || clindex == -1 || datalen == 0 || samplerate == 0 || (cp != null && !cp.Active))
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _PacketInfos.Clear();
            }
            try
            {
                if (_NeedDecodeData)
                {
                    Int32 bitdatacount = (Int32)(Math.Round(1d / CustomBaud * samplerate, 0));
                    _PacketInfos.Clear();
                    _NeedDecodeData = false;
                    _NeedUpdateViewInfo = true;
                    if (bitdatacount >= 2)
                    {
                        //用户参数
                        ARINC429Options options = new()
                        {
                            SignalRateType = SignalRate,
                            DecodeMode = DecodeMode,
                            SignalRate = (UInt32)CustomBaud,
                            Polarity = Polarity,
                            CancelFlag = _CancelFlagPtr
                        };
                        //边沿脉宽
                        node = DecodeDataHelper.Instance.GetThreeLevelEdgeInfo(BusId, 0, Source1, ref token, ref needclear) as ThreeLevelEdgeInfo;
                        if (node == null)
                        {
                            return;
                        }
                        // 边沿脉宽信息获取
                        _EdgePulsesList.Clear();
                        IntPtr edgepulseptr = IntPtr.Zero;
                        GCHandle edgePulsesHandle;
                        DecodeDataHelper.Instance.GetThreeLevelPulses(ref node, ref _EdgePulsesList);
                        PAM3EdgePulseSequence.Allocate(ref _EdgePulsesList, (UInt64)datalen, samplerate, out edgepulseptr, out edgePulsesHandle);

                        //开始解码                       
                        decoderesult.EventInfosPtr = IntPtr.Zero;
                        if (!DecoderImpl.DecodeARINC429(options, edgepulseptr, out decoderesult))
                        {
                        }
                        PAM3EdgePulseSequence.Free(ref edgepulseptr, ref edgePulsesHandle);

                        _PacketInfos.Clear();
                        //解码结果获取转换
                        Int32 structSize = Marshal.SizeOf(typeof(Arinc429EventCPP));
                        for (Int32 i = 0; i < decoderesult.EventCount; i++)
                        {
                            IntPtr presultptr = new IntPtr(decoderesult.EventInfosPtr.ToInt64() + i * structSize);
                            Arinc429EventCPP presult = (Arinc429EventCPP)Marshal.PtrToStructure(presultptr, typeof(Arinc429EventCPP));
                            _PacketInfos.Add(presult);
                        }
                        //c++资源释放
                        DecoderImpl.FreeARINC429(decoderesult);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            // 通知界面更新
            UpdateView(clindex);

        }

        internal void UpdateView(Int32 clindex)
        {
            try
            {
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
                        if (x.GAPInfo.HaData == HASDATATRUE && x.GAP == HASDATATRUE)
                        {
                            var gap = new ARINC429GapDecodePacket(CalcPosition((Int32)x.GAPInfo.StartIndex, Source1, clindex), CalcBitLenght((Int32)x.GAPInfo.Length, Source1, clindex));
                            packets.Add(gap);
                            _EventInfos.Add(new ProtocolEventInfo()
                            {
                                Index = _EventInfos.Count,
                            });
                            _EventInfos[^1].EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                            _EventInfos[^1].EventInofs[0] = (Encoding.Default.GetBytes("GAP"), 0);
                            _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes(("Gap: Gap Error")), 0);
                            _EventInfos[^1].StartTimeByPs = GetTimeFromPosition(packets.OrderBy(x => x.Start).First().Start, clindex);
                            _EventInfos[^1].StartPosition = packets.OrderBy(x => x.Start).First().Start;
                            endindex = (Int32)x.GAPInfo.StartIndex;
                        }

                        _EventInfos.Add(new ProtocolEventInfo()
                        {
                            Index = _EventInfos.Count,
                        });
                        _EventInfos[^1].EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));

                        if (x.SOF.HaData == HASDATATRUE)
                        {
                            packets.Add(new ARINC429SOFDecodePacket(CalcPosition((Int32)x.SOF.StartIndex, Source1, clindex)));
                            endindex = (Int32)x.SOF.StartIndex;
                        }
                        if (x.LabelInfo.HaData == HASDATATRUE)
                        {
                            packets.Add(new ARINC429LABELDecodePacket(CalcPosition((Int32)x.LabelInfo.StartIndex, Source1, clindex), CalcBitLenght((Int32)x.LabelInfo.Length, Source1, clindex))
                            {
                                Success = x.LabelInfo.ErrorType == (Byte)DataErrorType.NoError,
                                BitCount = (UInt32)x.LabelInfo.BitCount,
                                Data = new Byte[] { x.Label },
                            });
                            _EventInfos[^1].EventInofs[0] = (packets[^1].Data, packets[^1].BitCount);
                            if (x.LabelInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes((packets[^1].ErrorInfo)), 0);
                            }
                            endindex = (Int32)x.LabelInfo.StartIndex + (Int32)x.LabelInfo.Length;
                        }

                        if (x.SDIInfo.HaData == HASDATATRUE)
                        {
                            packets.Add(new ARINC429SDIDecodePacket(CalcPosition((Int32)x.SDIInfo.StartIndex, Source1, clindex), CalcBitLenght((Int32)x.SDIInfo.Length, Source1, clindex))
                            {
                                Success = x.SDIInfo.ErrorType == (Byte)DataErrorType.NoError,
                                BitCount = (UInt32)x.SDIInfo.BitCount,
                                Data = new Byte[] { x.SDI },
                            });
                            _EventInfos[^1].EventInofs[1] = (packets[^1].Data, packets[^1].BitCount);
                            if (x.SDIInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes((packets[^1].ErrorInfo)), 0);
                            }
                            endindex = (Int32)x.LabelInfo.StartIndex + (Int32)x.LabelInfo.Length;
                        }
                        if (x.DataInfo.HaData == HASDATATRUE)
                        {
                            packets.Add(new ARINC429DataDecodePacket(CalcPosition((Int32)x.DataInfo.StartIndex, Source1, clindex), CalcBitLenght((Int32)x.DataInfo.Length, Source1, clindex))
                            {
                                Success = x.DataInfo.ErrorType == 0,
                                BitCount = (UInt32)x.DataInfo.BitCount,
                                Data = BitConverter.GetBytes((UInt32)x.Data).Take((Int32)Math.Ceiling(x.DataInfo.BitCount / 8f)).Reverse().ToArray(),
                            });
                            _EventInfos[^1].EventInofs[2] = (packets[^1].Data, packets[^1].BitCount);
                            if (x.DataInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes((packets[^1].ErrorInfo)), 0);
                            }
                            endindex = (Int32)x.DataInfo.StartIndex + (Int32)x.DataInfo.Length;
                        }
                        if (x.SSMInfo.HaData == HASDATATRUE)
                        {
                            packets.Add(new ARINC429SSMDecodePacket(CalcPosition((Int32)x.SSMInfo.StartIndex, Source1, clindex), CalcBitLenght((Int32)x.SSMInfo.Length, Source1, clindex))
                            {
                                Success = x.SSMInfo.ErrorType == (Byte)DataErrorType.NoError,
                                BitCount = (UInt32)x.SSMInfo.BitCount,
                                Data = new Byte[] { x.SSM },
                            });
                            _EventInfos[^1].EventInofs[3] = (packets[^1].Data, packets[^1].BitCount);
                            if (x.SSMInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes((packets[^1].ErrorInfo)), 0);
                            }
                            endindex = (Int32)x.SSMInfo.StartIndex + (Int32)x.SSMInfo.Length;
                        }
                        if (x.ParityInfo.HaData == HASDATATRUE)
                        {
                            packets.Add(new ARINC429ParityDeocdePacket(CalcPosition((Int32)x.ParityInfo.StartIndex, Source1, clindex), CalcBitLenght((Int32)x.ParityInfo.Length, Source1, clindex))
                            {
                                Parity = x.Parity == 1,
                                SuccessParity = x.ParityInfo.ErrorType == (Byte)DataErrorType.NoError,
                            });
                            _EventInfos[^1].EventInofs[4] = (packets[^1].Data, packets[^1].BitCount);
                            if (x.ParityInfo.ErrorType != (Byte)DataErrorType.NoError)
                            {
                                _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes("Parity Error"), 0);
                            }
                            endindex = (Int32)x.ParityInfo.StartIndex + (Int32)x.ParityInfo.Length;
                        }
                        if (x.ExtraDatInfo.HaData == HASDATATRUE)
                        {
                            ARINC429DataDecodePacket extraPacket = new ARINC429DataDecodePacket(CalcPosition((Int32)x.ExtraDatInfo.StartIndex, Source1, clindex), CalcBitLenght((Int32)x.ExtraDatInfo.Length, Source1, clindex))
                            {
                                Success = false,
                                BitCount = (UInt32)x.ExtraDatInfo.BitCount,
                                Data = new Byte[1],
                            };
                            extraPacket.Data[0] = x.ExtraData;
                            packets.Add(extraPacket);
                            _EventInfos[^1].EventInofs[5] = (Encoding.Default.GetBytes("Extra bits:too many bits"), 0);
                            endindex = (Int32)x.ExtraDatInfo.StartIndex + (Int32)x.ExtraDatInfo.Length;
                        }
                        if (x.EOF.HaData == HASDATATRUE)
                        {
                            packets.Add(new ARINC429EOFDecodePacket(CalcPosition((Int32)x.EOF.StartIndex, Source1, clindex)));
                            endindex = (Int32)x.EOF.StartIndex;
                        }

                        if (packets.Count > 0)
                        {
                            _EventInfos[^1].StartTimeByPs = GetTimeFromPosition(packets.OrderBy(x => x.Start).First().Start, clindex);
                            _EventInfos[^1].StartPosition = packets.OrderBy(x => x.Start).First().Start;
                        }
                        _EventInfos[^1].EndPosition = CalcPosition(endindex, Source1, clindex);
                        _EventInfos[^1].EndTimeByPs = GetTimeFromPosition(_EventInfos[^1].EndPosition, clindex);
                        return packets;
                    }).OrderBy(x => x.Start).ToArray();
                    buffer.Add(_ResultData);

                    ChangeBuffer();
                }
            }
            catch (Exception ex)
            {
            }

        }
    }
}

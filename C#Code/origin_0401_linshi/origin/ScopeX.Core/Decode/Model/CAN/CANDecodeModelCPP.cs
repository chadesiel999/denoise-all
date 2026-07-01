using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using static ScopeX.Core.Decode.EdgeInfoCPP;

namespace ScopeX.Core.Decode;


internal partial class CANDecodeModelCPP : ProtocolModel
{
    private readonly Int32 _CRCBitCount = 15;
    private readonly Int32 _DLCBitcount = 4;
    private readonly Int32 _StandardIDBitCount = 11;
    private readonly DecodeResultData _DecodeResultData = new();
    private List<CANFDPacketInfoPK> _PacketInfos = new();
    private List<PAM2EdgePulse> _EdgePulsesList = new List<PAM2EdgePulse>();

    public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>
    {
        "Index",
        "Start Time",
        //"SOF",
        "StandardID",
        "ExpandID",
        "DLC",
        "Data",
        "CRC",
        "ACK",
        "EOF",
        "ERROR"
    };

    #region 公有属性
    public Double MaxThreshold => (Single)(12 * TryGetChannelGain(_Source1));
    public Double MinThreshold => -MaxThreshold;
    public Int64 MaxSignalRate => 10_000_000;
    public Int64 MinSignalRate => 10_000;
    public override Double BitRateByPs => (1f / CustomSignalRate) * 1E+12;

    private Dictionary<ProtocolCAN.SignalRate, Int64> _SignalRateMap = new Dictionary<ProtocolCAN.SignalRate, Int64>();
    public Dictionary<ProtocolCAN.SignalRate, Int64> SignalRateMap
    {
        get => _SignalRateMap;
    }
    
    public void InitSignalRateMap()
    {
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_custom] = CustomSignalRate;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_10k] = 10_000L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_19_2k] = 19_200L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_20k] = 20_000L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_33_3k] = 33_300L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_38_4k] = 38_400L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_50k] = 50_000L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_57_6k] = 57_600L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_62_5k] = 62_500L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_83_3k] = 83_300L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_100k] = 100_000L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_115_2k] = 115_200L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_125k] = 125_000L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_230_4k] = 230_400L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_250k] = 250_000L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_490_8k] = 490_800L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_500k] = 500_000L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_800k] = 800_000L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_921_6k] = 921_600L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_1M] = 1_000_000L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_2M] = 2_000_000L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_3M] = 3_000_000L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_4M] = 4_000_000L;
        _SignalRateMap[ProtocolCAN.SignalRate.SignalRate_5M] = 5_000_000L;

    }


    //信号速率
    ProtocolCAN.SignalRate _SignalRate = ProtocolCAN.SignalRate.SignalRate_100k;
    public ProtocolCAN.SignalRate SignalRate
    {
        get => _SignalRate;
        set
        {
            if (value != _SignalRate)
            {
                if (ProtocolCAN.SignalRate.SignalRate_custom == value)
                {
                    UpdateProperty(ref _SignalRate, value);
                }
                _SignalRate = value;
                CustomSignalRate = SignalRateMap[value];
            }
        }
    }

    //自定义的信号速率（当SignalRate == TriggerCANSignalRate.CANSignalRate_custom时使用）
    Int64 _CustomSignalRate = 100000;//250000;
    public Int64 CustomSignalRate
    {
        get => _CustomSignalRate;
        set
        {
            if (_CustomSignalRate != value)
            {
                foreach (var kvp in SignalRateMap)
                {
                    if (EqualityComparer<Int64>.Default.Equals(kvp.Value, value))
                    {
                        SignalRate = kvp.Key;
                        break;
                    }
                }
                UpdateProperty(ref _CustomSignalRate, value);
            }
        }
    }

    //信号类型
    ProtocolCAN.SignalType _SignalType = ProtocolCAN.SignalType.CAN_H;
    public ProtocolCAN.SignalType SignalType
    {
        get => _SignalType;
        set => UpdateProperty(ref _SignalType, value);
    }

    //输入1
    ChannelId _Source1 = ChannelId.C1;
    public ChannelId Source1
    {
        get => _Source1;
        set => UpdateProperty(ref _Source1, value);
    }

    //输入2(信号类型选择"差分"时使用)
    ChannelId _Source2 = ChannelId.C1;
    public ChannelId Source2
    {
        get => _Source2;
        set => UpdateProperty(ref _Source2, value);
    }

    public Int32 MinSamplePoint => 30;

    public Int32 MaxSamplePoint => 90;
    //采样点
    Int32 _SamplePoint = 30;
    public Int32 SamplePoint
    {
        get => _SamplePoint;
        set => UpdateProperty(ref _SamplePoint, value);
    }
    Double _SDAThreshold = 1;
    /// <summary>
    ///     数据源的阈值
    /// </summary>
    public Double SDAThreshold
    {
        get => _SDAThreshold * TryGetChannelGain(Source1);
        set => UpdateProperty(ref _SDAThreshold, value / TryGetChannelGain(Source1));
    }

    public String SDAUnit => GetChannelUnit(Source1);

    #endregion

    public CANDecodeModelCPP(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.CAN, isTrigDecode)
    {
        _DecodeResultData.Name = "CAN";
        InitSignalRateMap();
    }

    public override HdMessage.IDecoderOptions GetProtocolRecoder()
    {
        return new HdMessage.ProtocolCANOptions
        {
            SamplePoint = SamplePoint,
            SDAThreshold = _SDAThreshold,
            SignalInput1 = Source1,
            SignalInput2 = Source2,
            SignalRate = CustomSignalRate,
            SignalType = SignalType
        };
    }

    override internal Boolean SourceHasData()
    {
        if (DsoPrsnt.DefaultDsoPrsnt == null)
            return false;

        DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Source1, out IChnlPrsnt? prsnt);
        if (prsnt == null)
            return false;

        if (Source1.IsReference() && prsnt.VuDatabase.Current != null)
        {
            return DecodeDataHelper.ReferenceHasData(Source1, _SDAThreshold);
        }

        if (Source1.IsAnalog())
        {
            return DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
        }

        return false;
    }
    override internal Boolean CheckUpdate(ref Int64 laststamp)
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

    override internal void ParsingData(ref CancellationToken token)
    {
        Int32 chindex = GetChIndex(Source1);
        Int32 index = 0;
        Double samplerate = 0;
        DecodeDataHelper.Instance.TryGetSampleRate(BusId, Source1, ref samplerate);
        UInt32 datalen = 0;
        Boolean needclear = false;
        IntPtr edgepulseptr = IntPtr.Zero;
        CANResult canresults = new CANResult();
        DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, Source1, ref datalen);
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
                Int32 bitdatacount = (Int32)Math.Round((1d / CustomSignalRate) * samplerate, 0);

                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _PacketInfos.Clear();
                if (bitdatacount >= 2)
                {
                    CANOptions options = new()
                    {
                        CancelFlag = _CancelFlagPtr,
                        SignalType = SignalType switch
                        {
                            ProtocolCAN.SignalType.CAN_H => ProtocolCANFD.SignalType.CAN_FDH,
                            ProtocolCAN.SignalType.CAN_L => ProtocolCANFD.SignalType.CAN_FDL,
                            ProtocolCAN.SignalType.Diff => ProtocolCANFD.SignalType.Diff,
                            ProtocolCAN.SignalType.RXTX => ProtocolCANFD.SignalType.RXTX,
                        },
                        SdSignalRate = (UInt32)CustomSignalRate,
                        FdSignalRate = (UInt32)CustomSignalRate,
                        SdSamplePointRate = SamplePoint / 100.0,
                        FdSamplePointRate = SamplePoint / 100.0,
                    };

                    //边沿脉宽信息获取
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

                    //解码结果获取转换
                    _PacketInfos.Clear();
                    Int32 structSize = Marshal.SizeOf(typeof(CANEventCPP));
                    for (Int32 i = 0; i < canresults.EventCount; i++)
                    {
                        IntPtr presultptr = new IntPtr(canresults.EventInfosPtr.ToInt64() + i * structSize);
                        CANEventCPP canevent = (CANEventCPP)Marshal.PtrToStructure(presultptr, typeof(CANEventCPP));
                        Int32 datastructsize = Marshal.SizeOf(typeof(CANEventDataInfoCPP));
                        List<CANEventDataInfoCPP> edatalist = new List<CANEventDataInfoCPP>();
                        for (UInt32 pindex = 0; pindex < canevent.DataInfosCnt; pindex++)
                        {
                            IntPtr pdataptr = new IntPtr(canevent.DataInfos.ToInt64() + pindex * datastructsize);
                            CANEventDataInfoCPP pdata = (CANEventDataInfoCPP)Marshal.PtrToStructure(pdataptr, typeof(CANEventDataInfoCPP));
                            edatalist.Add(pdata);
                        }
                        CANFDPacketInfoPK packinfo = new()
                        {
                            PacketInfo = canevent,
                            DataInfos = edatalist,
                        };
                        _PacketInfos.Add(packinfo);
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
            // ignored
        }
    }

    internal void UpdateView(Int32 chindex)
    {
        if (!_NeedUpdateViewInfo) return;
        _NeedUpdateViewInfo = false;
        List<DecodeResultData> buffer = GetDecodeBuffer();
        buffer.Clear();
        _EventInfos.Clear();
        try
        {
            if (_PacketInfos.Count == 0)
            {
                _DecodeResultData.DecodeViewInfos = Array.Empty<IDecodeViewInfo>();
                buffer.Add(_DecodeResultData);
                ChangeBuffer();
                return;
            }
            _DecodeResultData.DecodeViewInfos = _PacketInfos.SelectMany(x =>
            {
                String errorstr = "";
                var endindex = 0;
                _EventInfos.Add(new ProtocolEventInfo
                {
                    Index = _EventInfos.Count
                });
                _EventInfos[^1].EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(_ => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                List<CANDecodePacket> packets = new();

                if (x.PacketInfo.PacketType == (Byte)FrameType.ErrorFrame)
                {
                    CANErrorFrameDeocdePacketCPP packet = new(CalcPosition((UInt32)x.PacketInfo.SOF.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.SOF.Len, Source1, chindex))
                    {
                    };
                    _EventInfos[^1].StartTimeByPs = GetTimeFromPosition(packet.Start, chindex);
                    _EventInfos[^1].StartPosition = packet.Start;
                    _EventInfos[^1].EventInofs[7] = (packet.Data, 0);
                    packets.Add(packet);
                    endindex = (Int32)x.PacketInfo.SOF.StartIndex + (Int32)x.PacketInfo.SOF.Len;
                }
                else if (x.PacketInfo.PacketType == (Byte)FrameType.OverloadFrame)
                {
                    CANOverLoadFrameDeocdePacketCPP packet = new(CalcPosition((UInt32)x.PacketInfo.SOF.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.SOF.Len, Source1, chindex))
                    {
                    };
                    _EventInfos[^1].StartTimeByPs = GetTimeFromPosition(packet.Start, chindex);
                    _EventInfos[^1].StartPosition = packet.Start;
                    _EventInfos[^1].EventInofs[7] = (packet.Data, 0);
                    packets.Add(packet);
                    endindex = (Int32)x.PacketInfo.SOF.StartIndex + (Int32)x.PacketInfo.SOF.Len;
                }
                else if (x.PacketInfo.PacketType == (Byte)FrameType.StandardRemoteFrame || x.PacketInfo.PacketType == (Byte)FrameType.StandardDataFrame)
                {
                    CANSOFDecodePacket startpacket = new(CalcPosition((UInt32)x.PacketInfo.SOF.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.SOF.Len, Source1, chindex));
                    _EventInfos[^1].StartTimeByPs = GetTimeFromPosition(startpacket.Start, chindex);
                    //_EventInfos[^1].EventInofs[0] = (packet.Data, packet.BitCount);
                    packets.Add(startpacket);
                    _EventInfos[^1].StartPosition = startpacket.Start;
                    endindex = (Int32)x.PacketInfo.SOF.StartIndex;

                    if (x.PacketInfo.StandardIdInfo.HasData != 0)
                    {
                        CANStandardIDDecodePacketCPP packet = new(CalcPosition((UInt32)x.PacketInfo.StandardIdInfo.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.StandardIdInfo.Len, Source1, chindex))
                        {
						    Success = (0 == x.PacketInfo.StandardIdInfo.ErrorType),
                            Data = x.PacketInfo.StandardId,//ConvertBitArrayToBytes(ref x.PacketInfo.StandardId)
                        };
                        _EventInfos[^1].EventInofs[0] = (packet.Data, (UInt32)_StandardIDBitCount);
                        packets.Add(packet);
                        endindex = (Int32)x.PacketInfo.StandardIdInfo.StartIndex+ (Int32)x.PacketInfo.StandardIdInfo.Len;
                    }
                }
                else
                {
                    CANSOFDecodePacket startpacket = new(CalcPosition((UInt32)x.PacketInfo.SOF.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.SOF.Len, Source1, chindex));
                    _EventInfos[^1].StartTimeByPs = GetTimeFromPosition(startpacket.Start, chindex);
                    //_EventInfos[^1].EventInofs[0] = (packet.Data, packet.BitCount);
                    packets.Add(startpacket);
                    _EventInfos[^1].StartPosition = startpacket.Start;
                    endindex = (Int32)x.PacketInfo.SOF.StartIndex;

                    if (x.PacketInfo.ExtIdInfo.HasData != 0)
                    {
                        CANExtandIDDecodePacketCPP packet = new(CalcPosition((UInt32)x.PacketInfo.ExtIdInfo.StartIndex, Source1, chindex), CalcBitLenght((Int32)(x.PacketInfo.ExtIdInfo.Len), Source1, chindex))
                        {
                            Success = (0 == x.PacketInfo.ExtIdInfo.ErrorType),
                            Data = x.PacketInfo.ExtId,//ConvertBitArrayToBytes(ref x.PacketInfo.ExtId),
                        };
                        //_EventInfos[^1].EventInofs[1] = (packet.Data, (uint)(_ExtpandIDBitCount + 8));
                        _EventInfos[^1].EventInofs[1] = (packet.Data, packet.BitCount);// (x.ExtID, (uint)(_ExtpandIDBitCount + 8));
                        packets.Add(packet);
                        endindex = (Int32)x.PacketInfo.ExtIdInfo.StartIndex + (Int32)x.PacketInfo.ExtIdInfo.Len;
                    }
                }
                //FIX
                //switch (x.PacketType)
                //{
                //    case CANFrameType.ErrorFrame:
                //        {
                //            CANErrorFrameDecodePacket packet = new(CalcPosition(x.ErrorIndex, Source1, chindex), CalcBitLenght(x.ErrorLen, Source1, chindex))
                //            {
                //                BitCount = (uint)(x.ErrorLen / x.SOFLen)
                //            };
                //            errorStr += "Frame,";
                //            //_EventInfos[^1].EventInofs[2] = (packet.Data, packet.BitCount);
                //            packets.Add(packet);
                //            return packets;
                //        }
                //    case CANFrameType.PadBitError:
                //        {
                //            packets.AddRange(x.FramePaddingErrorIndexs.Select(errorIndex => new CANErrorPadBitDecodePacket(CalcPosition(errorIndex, Source1, chindex), CalcBitLenght(x.SOFLen, Source1, chindex)) { BitCount = 0 }).Cast<CANDecodePacket>());
                //            break;
                //        }
                //}
                if (x.PacketInfo.DLCInfo.HasData != 0)
                {
                    CANDLCDecodePacket packet = new(CalcPosition((UInt32)x.PacketInfo.DLCInfo.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.DLCInfo.Len, Source1, chindex))
                    {
                        Data = new[] { x.PacketInfo.DLC }
                    };
                    _EventInfos[^1].EventInofs[2] = (packet.Data, (UInt32)_DLCBitcount);
                    packets.Add(packet);
                    endindex = (Int32)x.PacketInfo.DLCInfo.StartIndex + (Int32)x.PacketInfo.DLCInfo.Len;
                }

                //if (x.HasData != 0)
                //{
                //    packets.AddRange(x.DataInfos.Select(data => new CANDataDecodePacket(CalcPosition(data.Index, Source1, chindex), CalcBitLenght(data.Len, Source1, chindex)) { Data = new[] { data.Data } }).Cast<CANDecodePacket>());
                //    _EventInfos[^1].EventInofs[3] = (x.DataInfos.Select(info => info.Data).ToArray(), (uint)x.DataInfos.Length * 8);
                //}
                if (x.PacketInfo.DataInfosCnt != 0)
                {
                    for (Int32 i = 0; i < x.PacketInfo.DataInfosCnt; i++)
                    {
                        CANEventDataInfoCPP pData = x.DataInfos[i];// (CANFDDataInfoCPP)Marshal.PtrToStructure(pDataPtr, typeof(CANFDDataInfoCPP));
                        CANDataDecodePacket packet = new CANDataDecodePacket(CalcPosition((UInt32)pData.StartIndex, Source1, chindex), CalcBitLenght((Int32)pData.Len, Source1, chindex))
                        {
                            Data = new Byte[] { pData.Data },
                        };
                        packets.Add(packet);
                        endindex = (Int32)pData.StartIndex + (Int32)pData.Len;
                    }
                    _EventInfos[^1].EventInofs[3] = (x.DataInfos.Select(x => x.Data).ToArray(), (UInt32)(packets.Where(x => x is CANDataDecodePacket).Select(x => (Int32)x.BitCount).Sum()));
                }

                if (x.PacketInfo.CRCInfo.HasData != 0)
                {
                    CANCRCDecodePacket packet = new(CalcPosition((UInt32)x.PacketInfo.CRCInfo.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.CRCInfo.Len, Source1, chindex))
                    {
                        SuccessCRC = x.PacketInfo.SuccessCRC.Skip(2).Take((Int32)Math.Ceiling(x.PacketInfo.CRCBitCount / 8.0)).ToArray(),//BitConverter.GetBytes(x.PacketInfo.SuccessCRC).Take((Int32)Math.Ceiling(x.PacketInfo.CRCBitCount / 8.0)).Reverse().ToArray(),//x.SuccessCRC,
                        BitCount = x.PacketInfo.CRCBitCount,
                        Data = x.PacketInfo.CRC.Take((Int32)Math.Ceiling(x.PacketInfo.CRCBitCount / 8.0)).ToArray(),//BitConverter.GetBytes(x.PacketInfo.CRC).Take((Int32)Math.Ceiling(x.PacketInfo.CRCBitCount / 8.0)).Reverse().ToArray()//x.CRC
                    };
                    if (!packet.Success)
                    {
                        errorstr += "CRC " + "Calculated:" + BitConverter.ToString(packet.SuccessCRC).Replace("-", ""); ;
                    }
                    _EventInfos[^1].EventInofs[4] = (packet.Data, (UInt32)_CRCBitCount);
                    packets.Add(packet);
                    endindex = (Int32)x.PacketInfo.CRCInfo.StartIndex + (Int32)x.PacketInfo.CRCInfo.Len;
                }
                if (x.PacketInfo.ACKInfo.HasData != 0)
                {
                    CANACKDeocdePacketCPP packet = new(CalcPosition((UInt32)x.PacketInfo.ACKInfo.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.ACKInfo.Len, Source1, chindex))
                    {
                        Data = new Byte[] { x.PacketInfo.ACK },
                    };
                    packet.Success = x.PacketInfo.ACK == 1 ? true : false;
                    _EventInfos[^1].EventInofs[5] = (packet.Data, packet.BitCount);
                    //_EventInfos[^1].EventInofs[6] = (Encoding.Default.GetBytes(x.ACK ? "False" : "True"), 5);
                    packets.Add(packet);
                    endindex = (Int32)x.PacketInfo.ACKInfo.StartIndex + (Int32)x.PacketInfo.ACKInfo.Len;
                }
                if (x.PacketInfo.EOFInfo.HasData != 0)
                {
                    CANEOFDecodePacket packet = new(CalcPosition((UInt32)x.PacketInfo.EOFInfo.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PacketInfo.EOFInfo.Len, Source1, chindex))
                    {
                        Data = new[] { x.PacketInfo.EOF }
                    };
                    _EventInfos[^1].EventInofs[6] = (packet.Data, packet.BitCount);
                    packets.Add(packet);
                    endindex = (Int32)x.PacketInfo.EOFInfo.StartIndex + (Int32)x.PacketInfo.EOFInfo.Len;
                }
                if (errorstr.Length > 0)
                {
                    //errorstr = errorstr.Remove(errorstr.Length - 1, 1);
                    Byte[] errorData = Encoding.Default.GetBytes(errorstr);
                    _EventInfos[^1].EventInofs[7] = (errorData, 0);
                }

                _EventInfos[^1].EndPosition = CalcPosition(endindex, Source1, chindex);
                _EventInfos[^1].EndTimeByPs = GetTimeFromPosition(_EventInfos[^1].EndPosition, chindex);
                return packets;
            }).ToArray();
            buffer.Add(_DecodeResultData);

            ChangeBuffer();
        }
        catch
        {
            // ignored
        }
    }

}
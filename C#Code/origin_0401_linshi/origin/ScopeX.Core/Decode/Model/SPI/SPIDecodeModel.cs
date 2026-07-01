#define ENABLE_CPP_DECODE
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using static ScopeX.ComModel.HdMessage;
using static ScopeX.ComModel.ProtocolCommon;
using static ScopeX.Core.Decode.EdgeInfoCPP;
using static ScopeX.Core.Decode.ManchesterDecodeModel;
using static ScopeX.Core.Decode.Mlt3DecodeModel;

namespace ScopeX.Core.Decode
{
    sealed class SPIDecodeModel : ProtocolModel
    {

        //private List<PacketInfo> _OldPacketInfos = new List<PacketInfo>();
        private DecodeResultData _MISOData = new DecodeResultData();
        private DecodeResultData _MOSIData = new DecodeResultData();

        private List<PAM2EdgePulse> _edgePulsesCs = new List<PAM2EdgePulse>();
        private List<PAM2EdgePulse> _edgePulsesClk = new List<PAM2EdgePulse>();
        private List<PAM2EdgePulse> _edgePulsesMosi = new List<PAM2EdgePulse>();
        private List<PAM2EdgePulse> _edgePulsesMiso = new List<PAM2EdgePulse>();

        public SPIDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id, ComModel.SerialProtocolType.SPI, isTrigDecode)
        {
            _MISOData.Name = "MISO";
            _MOSIData.Name = "MOSI";
        }

        public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>()
        {
            "Index",
            "Start Time",
            "MI Data",
            "MO Data",
        };

        /*模式选择*/
        private ComModel.ProtocolSPI.FramingMode _FramingMode;

        public ComModel.ProtocolSPI.FramingMode FramingMode
        {
            get { return _FramingMode; }
            set { UpdateProperty(ref _FramingMode, value); }
        }

        /*通道选择*/
        private ComModel.ProtocolSPI.DecodeChannel _DecodeChannel = ComModel.ProtocolSPI.DecodeChannel.MOSI;

        public ComModel.ProtocolSPI.DecodeChannel DecodeChannel
        {
            get { return _DecodeChannel; }
            set { UpdateProperty(ref _DecodeChannel, value); }
        }

        /*位宽*/
        public Int32 MaxFrameCount => 32;
        public Int32 MinFrameCount => 4;

        private Int32 _FrameCount = 8;

        public Int32 FrameCount
        {
            get { return _FrameCount; }
            set { UpdateProperty(ref _FrameCount, value); }
        }


        private ComModel.ChannelId _CLK = ComModel.ChannelId.C4;

        public ComModel.ChannelId CLK
        {
            get { return _CLK; }
            set { UpdateProperty(ref _CLK, value); }
        }

        private ComModel.ChannelId _MOSI = ComModel.ChannelId.C2;

        public ComModel.ChannelId MOSI
        {
            get { return _MOSI; }
            set { UpdateProperty(ref _MOSI, value); }
        }

        private ComModel.ChannelId _MISO = ComModel.ChannelId.C3;

        public ComModel.ChannelId MISO
        {
            get { return _MISO; }
            set { UpdateProperty(ref _MISO, value); }
        }

        private ComModel.ChannelId _CS = ComModel.ChannelId.C2;

        public ComModel.ChannelId CS
        {
            get { return _CS; }
            set { UpdateProperty(ref _CS, value); }
        }

        /*超时时间*/
        public Double MinOutTime => 50E-9;
        public Double MaxOutTime => 10;
        private Double _OutTime = 50E-9;

        public Double OutTime
        {
            get { return _OutTime; }
            set { UpdateProperty(ref _OutTime, value); }
        }

        private Double _CLKThreshold;

        public Double CLKThreshold
        {
            get { return _CLKThreshold * TryGetChannelGain(CLK); }
            set { UpdateProperty(ref _CLKThreshold, value / TryGetChannelGain(CLK)); }
        }

        public String CLKUnit => GetChannelUnit(CLK);
        private Double _MOSIThreshold;

        public Double MOSIThreshold
        {
            get { return _MOSIThreshold * TryGetChannelGain(MOSI); }
            set { UpdateProperty(ref _MOSIThreshold, value / TryGetChannelGain(MOSI)); }
        }

        public String MOSIUnit => GetChannelUnit(MOSI);
        private Double _MISOThreshold;

        public Double MISOThreshold
        {
            get { return _MISOThreshold * TryGetChannelGain(MISO); }
            set { UpdateProperty(ref _MISOThreshold, value / TryGetChannelGain(MISO)); }
        }
        public String MISOUnit => GetChannelUnit(MISO);
        private ComModel.ProtocolSPI.MSB_LSB _ByteOrder = ProtocolSPI.MSB_LSB.MSB;

        public ComModel.ProtocolSPI.MSB_LSB ByteOrder
        {
            get { return _ByteOrder; }
            set { UpdateProperty(ref _ByteOrder, value); }
        }
        private ComModel.ProtocolCommon.Polarity _MISOPolarity = ProtocolCommon.Polarity.Positive;

        public ComModel.ProtocolCommon.Polarity MISOPolarity
        {
            get { return _MISOPolarity; }
            set { UpdateProperty(ref _MISOPolarity, value); }
        }

        private ComModel.ProtocolCommon.Polarity _MOSIPolarity = ProtocolCommon.Polarity.Positive;

        public ComModel.ProtocolCommon.Polarity MOSIPolarity
        {
            get { return _MOSIPolarity; }
            set { UpdateProperty(ref _MOSIPolarity, value); }
        }

        private ComModel.ProtocolSPI.LevelState _CSLevelState = ProtocolSPI.LevelState.Low;

        public ComModel.ProtocolSPI.LevelState CSLevelState
        {
            get { return _CSLevelState; }
            set { UpdateProperty(ref _CSLevelState, value); }
        }

        private ComModel.ProtocolCommon.Edge _CLKState = ProtocolCommon.Edge.Rise;

        public ComModel.ProtocolCommon.Edge CLKState
        {
            get { return _CLKState; }
            set { UpdateProperty(ref _CLKState, value); }
        }
        public Double MinThresholdMISO => -MaxThresholdMISO;
        public Double MaxThresholdMISO => (float)(8 * TryGetChannelGain(_MISO));
        public Double MinThresholdCLK => -MaxThresholdCLK;
        public Double MaxThresholdCLK => (float)(8 * TryGetChannelGain(_CLK));
        public Double MinThresholdMOSI => -MaxThresholdMOSI;
        public Double MaxThresholdMOSI => (float)(8 * TryGetChannelGain(_MOSI));

        public Double MinThresholdCS => -MaxThresholdCS;
        public Double MaxThresholdCS => (float)(8 * TryGetChannelGain(_CS));
        private Double _CSThreshold;

        public Double CSThreshold
        {
            get { return _CSThreshold * TryGetChannelGain(CS); }
            set { UpdateProperty(ref _CSThreshold, value / TryGetChannelGain(CS)); }
        }
        public String CSUnit => GetChannelUnit(CS);

        private UInt32 GetIdleTimeByFpgaNs(double samplerate)
        {
            return (UInt32)(_OutTime * samplerate);
        }
        internal override void ParsingData(ref CancellationToken token)
        {
            /*数据索引*/
            Int32 csindex = GetChIndex(CS);
            Int32 clkindex = GetChIndex(CLK);
            Int32 mosiindex = GetChIndex(MOSI);
            Int32 misoindex = GetChIndex(MISO);

            /*采样率与数据长度*/
            UInt32 cslen = 0;
            UInt32 clklen = 0;
            UInt32 misolen = 0;
            UInt32 mosilen = 0;
            double sampleRate = 0;

            DecodeDataHelper.Instance.TryGetSampleRate(BusId, CLK, ref sampleRate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, CS, ref cslen);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, CLK, ref clklen);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, MISO, ref misolen);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, MOSI, ref mosilen);
            if (MoreThanStorage() || csindex == -1 || clkindex == -1 || mosiindex == -1 || misoindex == -1 || sampleRate == 0)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _MOSIData.DecodeViewInfos = new IDecodeViewInfo[0];
                _MISOData.DecodeViewInfos = new IDecodeViewInfo[0];
                _EventInfos.Clear();
            }

            if (!_NeedDecodeData && !_NeedUpdateViewInfo)
                return;

            if (_NeedDecodeData || _NeedUpdateViewInfo)
            {
                SpiOptions options = new()
                {
                    CancelFlag = _CancelFlagPtr,
                    DecodeChannel = _DecodeChannel,
                    MsbLsb = ByteOrder,
                    ClkPolarity = _CLKState == ProtocolCommon.Edge.Rise ? Polarity.Positive : Polarity.Negative,
                    CsPolarity = _CSLevelState == ProtocolSPI.LevelState.Hight ? Polarity.Positive : Polarity.Negative,
                    MosiPolarity = _MOSIPolarity,
                    MisoPolarity = _MISOPolarity,
                    CsMode = _FramingMode,                         // 片选模式
                    OutTime = GetIdleTimeByFpgaNs(sampleRate),     // 超时时间
                    BitLength = (uint)_FrameCount,                 // 比特位宽
                };

                /*数据*/
                GCHandle handlCs = new(), handlClk = new(), handlMiso = new(), handlMosi = new();
                IntPtr edgepulsePtrCs = IntPtr.Zero;
                IntPtr edgepulsePtrClk = IntPtr.Zero;
                IntPtr edgepulsePtrMosi = IntPtr.Zero;
                IntPtr edgepulsePtrMiso = IntPtr.Zero;

                /*获取边沿*/
                bool needClear = false;
                TwoLevelEdgeInfo? nodeClk = null;
                TwoLevelEdgeInfo? nodeMosi = null;
                TwoLevelEdgeInfo? nodeCs = null;
                if (options.CsMode == ProtocolSPI.FramingMode.TIMEOUT)
                {
                    nodeClk = DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, CLK, ref token, ref needClear) as TwoLevelEdgeInfo;
                    nodeMosi = DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, MOSI, ref token, ref needClear) as TwoLevelEdgeInfo;

                    if (nodeClk == null || nodeMosi == null)
                        return;
                

                }
                else
                {
                    nodeClk = DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, CLK, ref token, ref needClear) as TwoLevelEdgeInfo;
                    nodeMosi = DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, MOSI, ref token, ref needClear) as TwoLevelEdgeInfo;
                    nodeCs = DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, CS, ref token, ref needClear) as TwoLevelEdgeInfo;

                    if (nodeClk == null || nodeMosi == null || nodeCs == null)
                        return;

                }



                _edgePulsesClk.Clear();
                _edgePulsesCs.Clear();
                _edgePulsesMosi.Clear();

                DecodeDataHelper.Instance.GetTwoLevelPulses(ref nodeClk, ref _edgePulsesClk);
                PAM2EdgePulseSequence.Allocate(ref _edgePulsesClk, (UInt64)clklen, sampleRate, out edgepulsePtrClk, out handlClk);
                DecodeDataHelper.Instance.GetTwoLevelPulses(ref nodeMosi, ref _edgePulsesMosi);
                PAM2EdgePulseSequence.Allocate(ref _edgePulsesMosi, (UInt64)misolen, sampleRate, out edgepulsePtrMosi, out handlMosi);
                DecodeDataHelper.Instance.GetTwoLevelPulses(ref nodeCs, ref _edgePulsesCs);
                PAM2EdgePulseSequence.Allocate(ref _edgePulsesCs, (UInt64)cslen, sampleRate, out edgepulsePtrCs, out handlCs);

                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;

                /*清除界面*/
                List<SPIDecodePacket> decodepackets = new List<SPIDecodePacket>(); 

                /*开始解码*/
                SpiResultStruct results;
                results.DecodeEventCount = 0;
                results.DecodeEvents = IntPtr.Zero;

                if (!DecoderImpl.DecodeSpi(ref options, edgepulsePtrClk, edgepulsePtrMosi, edgepulsePtrCs, out results))
                {
                }

                /*释放边沿*/
                PAM2EdgePulseSequence.Free(ref edgepulsePtrClk, ref handlClk);
                PAM2EdgePulseSequence.Free(ref edgepulsePtrMosi, ref handlMosi);
                PAM2EdgePulseSequence.Free(ref edgepulsePtrCs, ref handlCs);

                //解码结果获取             
                List<DecodeResultData> decoderesults = GetDecodeBuffer();

                if (_NeedUpdateViewInfo)
                {
                    _NeedUpdateViewInfo = false;
                    _EventInfos.Clear();
                    decoderesults.Clear();
                    _MOSIData = new DecodeResultData();
                    _MISOData = new DecodeResultData();
                    ChangeBuffer();
                }
                Int32 eventsize = Marshal.SizeOf(typeof(SpiEvent));
                for (Int32 i = 0; i < results.DecodeEventCount; i++)
                {
                    //创建一个用于界面展示的eventinfo
                    ProtocolEventInfo eventinfo = new ProtocolEventInfo();
                    var endindex = 0;//用以定位帧
                    eventinfo.Index = _EventInfos.Count;
                    eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));

                    /*将事件指针数据转换为SPIEvent结构体，提取事件的起始位置和时间。*/
                    SpiEvent spievent = (SpiEvent)Marshal.PtrToStructure(results.DecodeEvents + i * eventsize, typeof(SpiEvent));

                    eventinfo.StartTimeByPs = base.GetTimeFromPosition(CalcPosition((Int64)spievent.StartIndex, CLK, clkindex), clkindex);
                    eventinfo.StartPosition = CalcPosition((Int64)spievent.StartIndex, CLK, clkindex);

                    /*先将事件表用“--”填充*/
                    string temp_info = "--";
                    eventinfo.EventInofs[0] = (Encoding.Default.GetBytes(temp_info), 0);//
                    eventinfo.EventInofs[1] = (Encoding.Default.GetBytes(temp_info), 0);//

                    /*起始*/
                    SPIStartDecodePacket start = new SPIStartDecodePacket(CalcPosition((long)spievent.StartIndex, CLK, clkindex), 1)
                    {
                    };
                    decodepackets.Add(start);

                    /*数据*/
                    SPIDataDecodePacket data = new SPIDataDecodePacket(CalcPosition((long)spievent.DataStartIndex, CLK, clkindex),
                        CalcBitLenght((int)(spievent.DataEndIndex - spievent.DataStartIndex), CLK, clkindex))
                    {
                        Data = new Byte[] { (Byte)spievent.Data },
                        BitCount = options.BitLength,
                    };
                    decodepackets.Add(data);
                    eventinfo.EventInofs[1] = (data.Data, options.BitLength);

                    /*结束*/
                    SPIEndDecodePacket end = new SPIEndDecodePacket(CalcPosition((long)spievent.EndIndex, CLK, clkindex), 1)
                    {
                    };
                    decodepackets.Add(end);

                    endindex = (Int32)spievent.EndIndex;
                    eventinfo.EndPosition = CalcPosition(endindex, CLK, clkindex);
                    eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, clkindex);
                    _EventInfos.Add(eventinfo);
                }
                if (!DecoderImpl.FreeSpi(ref results))
                    return;

                _MOSIData.DecodeViewInfos = decodepackets.ToArray();
                decoderesults.Add(_MOSIData);
            }
        }
        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolSPIOptions()
            {
                ByteOrder = ByteOrder,
                CLK = CLK,
                CLKState = CLKState,
                CLKThreshold = _CLKThreshold,
                CS = CS,
                CSLevelState = CSLevelState,
                CSThreshold = _CSThreshold,
                FrameCount = FrameCount,
                DecodeChannel = DecodeChannel,
                FramingMode = FramingMode,
                IdleTime = OutTime,
                //MISO = MISO,
                MISOPolarity = MISOPolarity,
                MISOThreshold = _MISOThreshold,
                MOSI = MOSI,
                MOSIPolarity = MOSIPolarity,
                MOSIThreshold = _MOSIThreshold,
            };
        }
    }
}


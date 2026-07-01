using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    [Obsolete("Please Use Class 'AudioBusDecodeModelCPP'", true)]
    internal sealed class AudioBusDecodeModel : ProtocolModel
    {
        private List<AudioDecode> _Deocders = new List<AudioDecode>();
        private List<AudioPacketInfo> _PacketInfos = new List<AudioPacketInfo>();
        private DecodeResultData _ResultData = new DecodeResultData();
        private List<String> _StandardChannelNames = new List<String>() { "Left Channel", "Right Channel" };
        private List<String> _TDMChannelNames = new List<String>();
        private List<String> _Header = new List<String>() { "Index", "Start Time" };
        public AudioBusDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id, SerialProtocolType.AudioBus, isTrigDecode)
        {
            _ResultData.Name = this.ProtocolType.ToString();
            _TDMChannelNames.AddRange(Enumerable.Range(0, _SoundChannelCount).Select(x => $"Channel{x + 1}"));
            if (!IsTrigger)
            {
                _Deocders.Add(new I2SAudioDecode(this));
                _Deocders.Add(new LJAudioDecode(this));
                _Deocders.Add(new RJAudioDecode(this));
                _Deocders.Add(new TDMAudioDecode(this));
            }
        }
        public override IReadOnlyList<String> EventInfoTitles => _Header.Concat(SubType == ProtocolAudioBus.SubType.TDM ? _TDMChannelNames : _StandardChannelNames).ToList().AsReadOnly();

        private ChannelId _SCL = ChannelId.C1;

        public ChannelId SCL
        {
            get { return _SCL; }
            set { UpdateProperty(ref _SCL, value); }
        }

        private ChannelId _WS = ChannelId.C2;

        public ChannelId WS
        {
            get { return _WS; }
            set { UpdateProperty(ref _WS, value); }
        }
        internal new Int32 GetChIndex(ChannelId id) => base.GetChIndex(id);

        private ChannelId _SDA = ChannelId.C3;

        public ChannelId SDA
        {
            get { return _SDA; }
            set { UpdateProperty(ref _SDA, value); }
        }


        public Double MaxThresholdSDA => (Single)(20 * TryGetChannelGain(SDA));

        public Double MaxThresholdSCL => (Single)(20 * TryGetChannelGain(SCL));
        public Double MaxThresholdWS => (Single)(20 * TryGetChannelGain(WS));
        public Double MinThresholdSDA => -MaxThresholdSDA;
        public Double MinThresholdSCL => -MaxThresholdSCL;
        public Double MinThresholdWS => -MaxThresholdWS;
        private Double _SCLThreshold = 1;

        public Double SCLThreshold
        {
            get { return _SCLThreshold * TryGetChannelGain(SCL); }
            set { UpdateProperty(ref _SCLThreshold, value / TryGetChannelGain(SCL)); }
        }

        public String SCLUnit => GetChannelUnit(SCL);

        private Double _WSThreshold = 1;

        public Double WSThreshold
        {
            get { return _WSThreshold * TryGetChannelGain(WS); }
            set { UpdateProperty(ref _WSThreshold, value / TryGetChannelGain(WS)); }
        }
        public String WSUnit => GetChannelUnit(WS);


        private Double _SDAThreshold = 1;

        public Double SDAThreshold
        {
            get { return _SDAThreshold * TryGetChannelGain(SDA); }
            set { UpdateProperty(ref _SDAThreshold, value / TryGetChannelGain(SDA)); }
        }
        public String SDAUnit => GetChannelUnit(SDA);


        private ProtocolAudioBus.SubType _SubType = ProtocolAudioBus.SubType.I2S;
        public ProtocolAudioBus.SubType SubType
        {
            get { return _SubType; }
            set { UpdateProperty(ref _SubType, value); }
        }

        private ProtocolCommon.Polarity _SyncPolarity;
        public ProtocolCommon.Polarity SyncPolarity
        {
            get { return _SyncPolarity; }
            set { UpdateProperty(ref _SyncPolarity, value); }
        }
        private ProtocolCommon.Edge _ClockEdge = ProtocolCommon.Edge.Rise;
        public ProtocolCommon.Edge ClockEdge
        {
            get { return _ClockEdge; }
            set { UpdateProperty(ref _ClockEdge, value); }
        }
        private ProtocolCommon.Polarity _DataPolarity = ProtocolCommon.Polarity.Positive;
        public ProtocolCommon.Polarity DataPolarity
        {
            get { return _DataPolarity; }
            set { UpdateProperty(ref _DataPolarity, value); }
        }
        private ProtocolAudioBus.MSB_LSB _MSB_LSB = ProtocolAudioBus.MSB_LSB.MSB;
        public ProtocolAudioBus.MSB_LSB MSB_LSB
        {
            get { return _MSB_LSB; }
            set { UpdateProperty(ref _MSB_LSB, value); }
        }
        private ProtocolAudioBus.SoundChannel _SoundChannel;
        public ProtocolAudioBus.SoundChannel SoundChannel
        {
            get { return _SoundChannel; }
            set
            {
                UpdateProperty(ref _SoundChannel, value);
            }

        }
        public Int32 MaxBitDelayCount => 64;
        public Int32 MinBitDelayCount => 0;
        private Int32 _BitDelayCount = 0;
        public Int32 BitDelayCount
        {
            get { return _BitDelayCount; }
            set { UpdateProperty(ref _BitDelayCount, value); }
        }

        public Int32 MinClockBitNumberPerChannel => 4;
        public Int32 MaxClockBitNumberPerChannel => 32;

        private Int32 _ClockBitNumberPerChannel = 4;

        /// <summary>
        /// 每通道时钟位
        /// </summary>
        public Int32 ClockBitNumberPerChannel
        {
            get { return _ClockBitNumberPerChannel; }
            set { UpdateProperty(ref _ClockBitNumberPerChannel, value); }
        }

        public Int32 MinChannelNumberPerFream => 2;
        public Int32 MaxChannelNumberPerFream => 64;

        private Int32 _ChannelNumberPerFream = 2;

        /// <summary>
        /// 每帧通道数量
        /// </summary>
        public Int32 ChannelNumberPerFream
        {
            get { return _ChannelNumberPerFream; }
            set { UpdateProperty(ref _ChannelNumberPerFream, value); }
        }

        public Int32 MaxDataBitCount => 64;
        public Int32 MinDataBitCount => 0;


        private Int32 _DataBitCount = 8;
        public Int32 DataBitCount
        {
            get { return _DataBitCount; }
            set { UpdateProperty(ref _DataBitCount, value); }
        }
        public Int32 MaxClockBitCount => 64;
        public Int32 MinClockBitCount => 0;

        private Int32 _ClockBitCount = 8;
        public Int32 ClockBitCount
        {
            get { return _ClockBitCount; }
            set { UpdateProperty(ref _ClockBitCount, value); }
        }
        public Int32 MaxSoundChannelCount => 32;
        public Int32 MinSoundChannelCount => 2;
        private Int32 _SoundChannelCount = 2;
        public Int32 SoundChannelCount
        {
            get { return _SoundChannelCount; }
            set
            {
                if (value != _SoundChannelCount)
                {
                    _TDMChannelNames = Enumerable.Range(0, value).Select((x, index) => $"Channel{index + 1}").ToList();
                }
                UpdateProperty(ref _SoundChannelCount, value);
            }
        }
        internal override void ParsingData(ref CancellationToken token)
        {
            Int32 clkindex = GetChIndex(SCL);
            Int32 wsindex = GetChIndex(WS);
            Int32 sdaindex = GetChIndex(SDA);
            UInt32 datalen = 0;
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, SCL, ref datalen);
            if (clkindex == -1 || wsindex == -1 || sdaindex == -1 || datalen == 0)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _PacketInfos.Clear();
            }
            Boolean needclear = false;
            AudioDecode? decoder = _Deocders.FirstOrDefault(x => x.SubType == SubType);
            if (_NeedDecodeData)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _PacketInfos.Clear();
                decoder?.ParsingData(BusId, ref token, ref needclear, ref _PacketInfos);
                //_PacketInfos.Clear();
                //Stopwatch stopwatch = new Stopwatch();
                //stopwatch.Start();
                //decoder?.ParsingData(ref token, ref needclear, ref _PacketInfos);
                //stopwatch.Stop();
                //Debug.WriteLine(stopwatch.ElapsedMilliseconds);
            }
            if (_NeedUpdateViewInfo)
            {
                _NeedUpdateViewInfo = false;
                var decodebuffer = GetDecodeBuffer();
                decodebuffer.Clear();
                _EventInfos.Clear();
                if (_PacketInfos.Count == 0)
                {
                    _ResultData.DecodeViewInfos = new IDecodeViewInfo[0];
                    decodebuffer.Add(_ResultData);
                    ChangeBuffer();
                    return;
                }
                _ResultData.DecodeViewInfos = _PacketInfos.SelectMany(x =>
                {
                    List<AudioBusDecodePacket> packets = new List<AudioBusDecodePacket>();
                    ProtocolEventInfo info = new ProtocolEventInfo();
                    info.Index = _EventInfos.Count;
                    info.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                    if (SubType == ProtocolAudioBus.SubType.TDM)
                    {
                        for (Int32 index = 0; index < x.Channels.Length; index++)
                        {
                            if (!x.Channels[index].HasData) continue;
                            AudioBusExtDecodePacket packet = new AudioBusExtDecodePacket(CalcPosition(x.Channels[index].Index, SCL, clkindex), CalcBitLenght(x.Channels[index].Length, SCL, clkindex))
                            {
                                BitCount = (UInt32)x.Channels[index].BitCount,
                                SuccessBitCount = (UInt32)x.Channels[index].SuccessBitCount,
                                SyncBitCount = (UInt32)x.Channels[index].ClkBitCount,
                                SuccessSyncBitCount = (UInt32)x.Channels[index].SuccessClkBitCount,
                                ChannelIndex = (UInt32)index,
                                Data = x.Channels[index].Value,
                            };
                            packets.Add(packet);
                            info.EventInofs[index] = (x.Channels[index].Value, (UInt32)x.Channels[index].BitCount);
                        }
                    }
                    else
                    {
                        if (SoundChannel == ProtocolAudioBus.SoundChannel.LeftOrRight || SoundChannel == ProtocolAudioBus.SoundChannel.Left)
                        {
                            if (x.Channels[0].HasData)
                            {
                                packets.Add(new AudioBusLeftDecodePacket(CalcPosition(x.Channels[0].Index, SCL, clkindex), CalcBitLenght(x.Channels[0].Length, SCL, clkindex))
                                {
                                    BitCount = (UInt32)x.Channels[0].BitCount,
                                    SuccessBitCount = (UInt32)x.Channels[0].SuccessBitCount,
                                    Data = x.Channels[0].Value,
                                });
                                info.EventInofs[0] = (packets[^1].Data, packets[^1].BitCount);
                            }
                        }
                        if (SoundChannel == ProtocolAudioBus.SoundChannel.LeftOrRight || SoundChannel == ProtocolAudioBus.SoundChannel.Right)
                        {
                            if (x.Channels[1].HasData)
                            {
                                packets.Add(new AudioBusRightDecodePacket(CalcPosition(x.Channels[1].Index, SCL, clkindex), CalcBitLenght(x.Channels[1].Length, SCL, clkindex))
                                {
                                    BitCount = (UInt32)x.Channels[1].BitCount,
                                    SuccessBitCount = (UInt32)x.Channels[1].SuccessBitCount,
                                    Data = x.Channels[1].Value,
                                });
                                info.EventInofs[1] = (packets[^1].Data, packets[^1].BitCount);
                            }
                        }
                    }
                    if (packets.Count > 0)
                    {
                        info.StartTimeByPs = base.GetTimeFromPosition(packets[0].Start, clkindex);
                        _EventInfos.Add(info);
                    }
                    return packets;
                }).OrderBy(x => x.Start).ToArray();
                decodebuffer.Add(_ResultData);
                ChangeBuffer();
            }
        }
        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolI2SOptions()
            {
                BitDelayCount = BitDelayCount,
                ClockBitNumberPerChannel = ClockBitNumberPerChannel,
                ChannelNumberPerFream = ChannelNumberPerFream,
                ClockBitCount = ClockBitCount,
                ClockEdge = ClockEdge,
                DataBitCount = DataBitCount,
                DataPolarity = DataPolarity,
                MSB_LSB = MSB_LSB,
                SoundChannel = SoundChannel,
                SoundChannelCount = SoundChannelCount,
                SubType = SubType,
                SyncPolarity = SyncPolarity,
                SDA = SDA,
                SDAThreshold = _SDAThreshold,
                SCL = SCL,
                SCLThreshold = _SCLThreshold,
                WS = WS,
                WSThreshold = _WSThreshold,
            };
        }
    }
    [Obsolete("Please Use Class 'AudioDecodeCPP'", true)]
    abstract class AudioDecode
    {
        private protected AudioBusDecodeModel _Model;
        public abstract ProtocolAudioBus.SubType SubType { get; }
        public AudioDecode(AudioBusDecodeModel model)
        {
            _Model = model;
        }
        public abstract void ParsingData(ChannelId busId, ref CancellationToken token, ref Boolean needclear, ref List<AudioPacketInfo> packetInfos);
    }
}

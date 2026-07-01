using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using static ScopeX.ComModel.ProtocolNRZ;
using static ScopeX.Controls.Common.APIs.APIsStructs;
using static ScopeX.Core.Decode.EdgeInfoCPP;

namespace ScopeX.Core.Decode
{
    internal sealed class AudioBusDecodeModelCPP : ProtocolModel
    {
        private List<AudioDecodeCPP> _Deocders = new List<AudioDecodeCPP>();
        private List<AudioPacketInfo> _PacketInfos = new List<AudioPacketInfo>();
        private DecodeResultData _ResultData = new DecodeResultData();
        private List<String> _StandardChannelNames = new List<String>() { "Left Channel", "Right Channel" };
        private List<String> _TDMChannelNames = new List<String>();
        private List<String> _Header = new List<String>() { "Index", "Start Time" };

        private List<PAM2EdgePulse> _ClkEdgePulsesList = new List<PAM2EdgePulse>();
        private List<PAM2EdgePulse> _WsEdgePulsesList = new List<PAM2EdgePulse>();
        private List<PAM2EdgePulse> _SdEdgePulsesList = new List<PAM2EdgePulse>();


        public AudioBusDecodeModelCPP(ChannelId id, Boolean isTrigDecode = false) : base(id, SerialProtocolType.AudioBus, isTrigDecode)
        {
            _ResultData.Name = this.ProtocolType.ToString();
            _TDMChannelNames.AddRange(Enumerable.Range(0, _SoundChannelCount).Select(x => $"Channel{x + 1}"));
            //if (!IsTrigger)
            //{
            //    _Deocders.Add(new I2SAudioDecode(this));
            //    _Deocders.Add(new LJAudioDecode(this));
            //    _Deocders.Add(new RJAudioDecode(this));
            //    _Deocders.Add(new TDMAudioDecode(this));
            //}
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
        /// <summary>
        /// 时钟边沿
        /// </summary>
        public ProtocolCommon.Edge ClockEdge
        {
            get { return _ClockEdge; }
            set { UpdateProperty(ref _ClockEdge, value); }
        }
        private ProtocolCommon.Polarity _DataPolarity = ProtocolCommon.Polarity.Positive;
        /// <summary>
        /// 数据极性
        /// </summary>
        public ProtocolCommon.Polarity DataPolarity
        {
            get { return _DataPolarity; }
            set { UpdateProperty(ref _DataPolarity, value); }
        }
        private ProtocolAudioBus.MSB_LSB _MSB_LSB = ProtocolAudioBus.MSB_LSB.MSB;

        /// <summary>
        /// 位序
        /// </summary>
        public ProtocolAudioBus.MSB_LSB MSB_LSB
        {
            get { return _MSB_LSB; }
            set { UpdateProperty(ref _MSB_LSB, value); }
        }
        private ProtocolAudioBus.SoundChannel _SoundChannel;

        /// <summary>
        /// 声道类型，指在两通道模式下面
        /// </summary>
        public ProtocolAudioBus.SoundChannel SoundChannel
        {
            get { return _SoundChannel; }
            set
            {
                UpdateProperty(ref _SoundChannel, value);
            }

        }
        public Int32 MaxBitDelayCount => 31;
        public Int32 MinBitDelayCount => 0;
        private Int32 _BitDelayCount = 0;
        public Int32 BitDelayCount
        {
            get { return _BitDelayCount; }
            set { UpdateProperty(ref _BitDelayCount, value); }
        }

        public Int32 MinClockBitNumberPerChannel => 4;
        public Int32 MaxClockBitNumberPerChannel => 32;

        private Int32 _ClockBitNumberPerChannel = 8;

        /// <summary>
        /// 每个通道时钟位数
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
        /// 每个帧的通道个数
        /// </summary>
        public Int32 ChannelNumberPerFream
        {
            get { return _ChannelNumberPerFream; }
            set { UpdateProperty(ref _ChannelNumberPerFream, value); }
        }

        public Int32 MaxDataBitCount => 64;
        public Int32 MinDataBitCount => 2;


        private Int32 _DataBitCount = 8;
        /// <summary>
        /// 每个通道的数据位数
        /// </summary>
        public Int32 DataBitCount
        {
            get { return _DataBitCount; }
            set { UpdateProperty(ref _DataBitCount, value); }
        }
        public Int32 MaxClockBitCount => 32;
        public Int32 MinClockBitCount => 4;

        private Int32 _ClockBitCount = 8;

        /// <summary>
        /// 每个通道的时钟位数
        /// </summary>
        public Int32 ClockBitCount
        {
            get { return _ClockBitCount; }
            set { UpdateProperty(ref _ClockBitCount, value); }
        }
        public Int32 MaxSoundChannelCount => 32;
        public Int32 MinSoundChannelCount => 4;
        private Int32 _SoundChannelCount = 4;

        /// <summary>
        /// TDM模式下的通道个数
        /// </summary>
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

        internal override Boolean CheckUpdate(ref Int64 laststamp)
        {
            if (_SCL.IsAnalog() && laststamp != DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp;
                return true;
            }
            if (_SCL.IsReference() && laststamp != DecodeDataHelper.Instance.ReferenceDataSource[_SCL - ChannelIdExt.MinRChId].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.ReferenceDataSource[_SCL - ChannelIdExt.MinRChId].TimeStamp;
                return true;
            }

            if (_WS.IsAnalog() && laststamp != DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp;
                return true;
            }
            if (_WS.IsReference() && laststamp != DecodeDataHelper.Instance.ReferenceDataSource[_WS - ChannelIdExt.MinRChId].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.ReferenceDataSource[_WS - ChannelIdExt.MinRChId].TimeStamp;
                return true;
            }


            if (_SDA.IsAnalog() && laststamp != DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp;
                return true;
            }
            if (_SDA.IsReference() && laststamp != DecodeDataHelper.Instance.ReferenceDataSource[_SDA - ChannelIdExt.MinRChId].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.ReferenceDataSource[_SDA - ChannelIdExt.MinRChId].TimeStamp;
                return true;
            }


            return false;
        }

        public override void UpdateReferenceDataStatus()
        {
            if (_SCL.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[_SCL - ChannelIdExt.MinRChId].Channels != null
                && DecodeDataSource.Instance.ReferenceDataSource[_SCL - ChannelIdExt.MinRChId].Channels[0] == _SCL)
            {
                DecodeDataSource.Instance.ReferenceDataSource[_SCL - ChannelIdExt.MinRChId].HasData = false;
            }
            if (_SDA.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[_SDA - ChannelIdExt.MinRChId].Channels != null
                && DecodeDataSource.Instance.ReferenceDataSource[_SDA - ChannelIdExt.MinRChId].Channels[0] == _SDA)
            {
                DecodeDataSource.Instance.ReferenceDataSource[_SDA - ChannelIdExt.MinRChId].HasData = false;
            }
            if (_SDA.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[_WS - ChannelIdExt.MinRChId].Channels != null
                 && DecodeDataSource.Instance.ReferenceDataSource[_WS - ChannelIdExt.MinRChId].Channels[0] == _WS)
            {
                DecodeDataSource.Instance.ReferenceDataSource[_WS - ChannelIdExt.MinRChId].HasData = false;
            }
        }

        internal override Boolean SourceHasData()
        {
            if (DsoPrsnt.DefaultDsoPrsnt == null)
                return false;

            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(SCL, out IChnlPrsnt? scl_prsnt);
            if (scl_prsnt == null)
                return false;

            Boolean clk = false, ws = false, data = false;

            if (SCL.IsReference() && scl_prsnt.VuDatabase.Current != null)
            {
                clk = DecodeDataHelper.ReferenceHasData(SCL, _SCLThreshold);
            }

            if (SCL.IsAnalog())
            {
                clk = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }


            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(_SDA, out IChnlPrsnt? prsnt);
            if (prsnt == null)
                return false;

            if (_SDA.IsReference() && prsnt.VuDatabase.Current != null)
            {
                ws = DecodeDataHelper.ReferenceHasData(_SDA, _SDAThreshold);
            }

            if (_SDA.IsAnalog())
            {
                ws = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }


            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(_WS, out IChnlPrsnt? ws_prsnt);
            if (ws_prsnt == null)
                return false;

            if (_WS.IsReference() && ws_prsnt.VuDatabase.Current != null)
            {
                data = DecodeDataHelper.ReferenceHasData(_WS, _WSThreshold);
            }

            if (_WS.IsAnalog())
            {
                data = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }


            return (data || ws || clk);
        }

        internal override void ParsingData(ref CancellationToken token)
        {

            Int32 clkindex = GetChIndex(SCL);
            Int32 wsindex = GetChIndex(WS);
            Int32 sdaindex = GetChIndex(SDA);
            UInt32 datalen = 0;

            Double clsamplerate = 0, wssamplerate = 0, sdasamplerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, SCL, ref clsamplerate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, SCL, ref datalen);

            DecodeDataHelper.Instance.TryGetSampleRate(BusId, WS, ref wssamplerate);

            DecodeDataHelper.Instance.TryGetSampleRate(BusId, SDA, ref sdasamplerate);

            if (MoreThanStorage() || clkindex == -1 || wsindex == -1 || sdaindex == -1 || datalen == 0)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _PacketInfos.Clear();
            }
            Boolean needclear = false;
            AudioDecodeCPP? decoder = _Deocders.FirstOrDefault(x => x.SubType == SubType);
            if (_NeedDecodeData)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _PacketInfos.Clear();

                TwoLevelEdgeInfo? wsnode = DecodeDataHelper.Instance.GetEdgeInfo(BusId, startindex: 0, this.WS, ref token, ref needclear) as TwoLevelEdgeInfo;
                TwoLevelEdgeInfo? clknode = DecodeDataHelper.Instance.GetEdgeInfo(BusId,0, this.SCL, ref token, ref needclear) as TwoLevelEdgeInfo;
                TwoLevelEdgeInfo? sdanode = DecodeDataHelper.Instance.GetEdgeInfo(BusId,0, this.SDA, ref token, ref needclear) as TwoLevelEdgeInfo;

                if (wsnode?.Child != null
                    && clknode?.Child != null
                    && sdanode?.Child != null
                    && clknode.Child.EndIndex != clknode.Child.StartIndex
                    && wsnode.Child.EndIndex != wsnode.Child.StartIndex)
                {
                    AudioBusOptions options = new AudioBusOptions()
                    {

                        AudioBusMode = this.SubType,  // audio bus 模式
                        WsSyncPolarity = this.SyncPolarity, // 位选择极性
                        DataPolarity = this.DataPolarity,   // 数据极性
                        ValidClockPolarity = this.ClockEdge == ProtocolCommon.Edge.Rise ? ProtocolCommon.Polarity.Positive :
                                                                                          ProtocolCommon.Polarity.Negative, // 时钟极
                        ByteOrder = this.MSB_LSB, // 位顺序

                        SoundChannel = this.SoundChannel, // 声道类型

                        BitNumberPreChannel = this.DataBitCount,

                        CancelFlag = _CancelFlagPtr, //取消解码标志
                    };

                    // 是否是TDM模式
                    if (this.SubType == ProtocolAudioBus.SubType.TDM)
                    {
                        options.tDMOption.ClockBitNumberPreChannel = _ClockBitCount;
                        options.tDMOption.ChannelNumberPreFrame = _SoundChannelCount;
                        options.tDMOption.BitDelay = _BitDelayCount;
                    }

                    IntPtr clkedgepulseptr = IntPtr.Zero;
                    IntPtr wsedgepulseptr = IntPtr.Zero;
                    IntPtr sdedgepulseptr = IntPtr.Zero;

                    GCHandle clkpulseshandle;
                    GCHandle wspulseshandle;
                    GCHandle sdpulseshandle;

                    // 时钟边沿脉宽信息获取                 
                    _ClkEdgePulsesList.Clear();
                    DecodeDataHelper.Instance.GetTwoLevelPulses(ref clknode, ref _ClkEdgePulsesList);
                    PAM2EdgePulseSequence.Allocate(ref _ClkEdgePulsesList, (UInt64)datalen, clsamplerate, out clkedgepulseptr, out clkpulseshandle);

                    _SdEdgePulsesList.Clear();
                    DecodeDataHelper.Instance.GetTwoLevelPulses(ref sdanode, ref _SdEdgePulsesList);
                    PAM2EdgePulseSequence.Allocate(ref _SdEdgePulsesList, (UInt64)datalen, sdasamplerate, out sdedgepulseptr, out sdpulseshandle);

                    _WsEdgePulsesList.Clear();
                    DecodeDataHelper.Instance.GetTwoLevelPulses(ref wsnode, ref _WsEdgePulsesList);
                    PAM2EdgePulseSequence.Allocate(ref _WsEdgePulsesList, (UInt64)datalen, wssamplerate, out wsedgepulseptr, out wspulseshandle);


                    AudioBusResultInfoCPP decoderesult = new AudioBusResultInfoCPP();
                    decoderesult.EventCount = 0;
                    decoderesult.AudioBusEvent = IntPtr.Zero;

                    Boolean parse_result = DecoderImpl.DecodeAudioBus(options, clkedgepulseptr, wsedgepulseptr, sdedgepulseptr, out decoderesult);


                    PAM2EdgePulseSequence.Free(ref clkedgepulseptr, ref clkpulseshandle);
                    PAM2EdgePulseSequence.Free(ref wsedgepulseptr, ref wspulseshandle);
                    PAM2EdgePulseSequence.Free(ref sdedgepulseptr, ref sdpulseshandle);

                    Int32 structsize = Marshal.SizeOf(typeof(AudioBusEventCPP));
                    Int32 channelsize = Marshal.SizeOf(typeof(AudioChannelCPP));

                    AudioPacketInfo audio_packet = new AudioPacketInfo();


                    for (Int32 i = 0; i < decoderesult.EventCount; i++)
                    {
                        AudioBusEventCPP presult = (AudioBusEventCPP)Marshal.PtrToStructure(decoderesult.AudioBusEvent + i * structsize, typeof(AudioBusEventCPP));

                        audio_packet.Channels = new AudioChannelPacket[presult.ChannelNum];

                        AudioChannelPacket audiochannelpacket = new AudioChannelPacket();
                        audiochannelpacket.HasData = true;

                        audiochannelpacket.BitCount = this.DataBitCount;
                        audiochannelpacket.SuccessBitCount = this.DataBitCount;

                        audiochannelpacket.ClkBitCount = this.ClockBitCount;
                        audiochannelpacket.SuccessClkBitCount = this.ClockBitCount;



                        for (Int32 channelindex = 0; channelindex < presult.ChannelNum; ++channelindex)
                        {
                            AudioChannelCPP channel = (AudioChannelCPP)Marshal.PtrToStructure(presult.AudioChannelPtr + channelindex * channelsize, typeof(AudioChannelCPP));
                            //audio_channel_packet.SuccessBitCount = channel.decode_data_count;
                            audiochannelpacket.Length = Convert.ToInt32(channel.ChannelEndIndex - channel.ChannelStartIndex);
                            audiochannelpacket.Value = new Byte[channel.DecodeDataCount];
                            audiochannelpacket.Index = Convert.ToInt32(channel.ChannelStartIndex);


                            Marshal.Copy(channel.DecodePtr, audiochannelpacket.Value, 0, channel.DecodeDataCount);

                            audio_packet.Channels[channelindex] = audiochannelpacket;
                        }

                        _PacketInfos.Add(audio_packet);

                    }

                    DecoderImpl.FreeAudioBus(ref decoderesult);
                }



                //    // decoder?.ParsingData(ref token, ref needclear, ref _PacketInfos);
                //    //_PacketInfos.Clear();
                //    //Stopwatch stopwatch = new Stopwatch();
                //    //stopwatch.Start();
                //    //decoder?.ParsingData(ref token, ref needclear, ref _PacketInfos);
                //    //stopwatch.Stop();
                //    //Debug.WriteLine(stopwatch.ElapsedMilliseconds);
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
                    var endindex = 0;
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
                            endindex = x.Channels[index].Index + x.Channels[index].Length;
                        }
                    }
                    else
                    {
                        endindex = x.Channels[0].Index;
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
                                endindex = x.Channels[0].Index + x.Channels[0].Length;
                            }
                        }
                        if (SoundChannel == ProtocolAudioBus.SoundChannel.LeftOrRight || SoundChannel == ProtocolAudioBus.SoundChannel.Right)
                        {
                            if (x.Channels.Count() > 1)
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
                                    endindex = x.Channels[1].Index + x.Channels[1].Length;
                                }
                            }
                        }
                    }
                    if (packets.Count > 0)
                    {
                        info.StartTimeByPs = base.GetTimeFromPosition(packets[0].Start, clkindex);
                        info.StartPosition = packets[0].Start;
                        _EventInfos.Add(info);
                    }
                    _EventInfos[^1].EndPosition = CalcPosition(endindex, SCL, clkindex);
                    _EventInfos[^1].EndTimeByPs = GetTimeFromPosition(_EventInfos[^1].EndPosition, clkindex);
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
    abstract class AudioDecodeCPP
    {
        private protected AudioBusDecodeModelCPP _Model;
        public abstract ProtocolAudioBus.SubType SubType { get; }
        public AudioDecodeCPP(AudioBusDecodeModelCPP model)
        {
            _Model = model;
        }
        public abstract void ParsingData(ref CancellationToken token, ref Boolean needclear, ref List<AudioPacketInfo> packetInfos);
    }
}

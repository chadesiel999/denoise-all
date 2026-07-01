using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ScopeX.Core.Decode.EdgeInfoCPP;
using Microsoft.VisualBasic.FileIO;
using System.Reflection;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.PTG;
using ScopeX.Hardware.Driver;
using NPOI.Util;
using static ScopeX.ComModel.ProtocolRS232;
using System.Net.Sockets;
using NPOI.SS.Formula.Functions;

namespace ScopeX.Core.Decode
{
    internal partial class D8B10BDecodeModel : ProtocolModel
    {

        private DecodeResultData _ResultData = new DecodeResultData();
        private List<PAM2EdgePulse> _EdgePulsesList = new List<PAM2EdgePulse>();
        private Dictionary<string, Int32> _EventDict = new Dictionary<string, Int32>();

        //事件显示
        public override IReadOnlyList<String> EventInfoTitles { get; } = (new List<String>()
        {
            "Index",
            "Start Time",
            "Raw Data",
            "Data",
            "Control",
            "Error",
        }).AsReadOnly();

        //通道选择
        private ChannelId _Source = ChannelId.C1;
        public ChannelId Source
        {
            get { return _Source; }
            set { UpdateProperty(ref _Source, value); }
        }

        //门限阈值
        public Double MaxThreshold => 10;
        public Double MinThreshold => -10;
        private Double _Threshold = 0;//默认值
        public Double Threshold
        {
            get { return _Threshold * TryGetChannelGain(Source); }
            set { UpdateProperty(ref _Threshold, value / TryGetChannelGain(Source)); }
        }

        //数据速率
        public Int64 MaxBaudrate => 20000000000000;//最大1Gb/s
        public Int64 MinBaudrate => 1;//最小1b/s
        private Int64 _Baudrate = 2200;//默认
        public Int64 Baudrate
        {
            get { return _Baudrate; }
            set { UpdateProperty(ref _Baudrate, value); }
        }

        //单位？
        public String Unit => GetChannelUnit(Source);

        public D8B10BDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id, SerialProtocolType.Common_8b10b, isTrigDecode)
        {

        }

        //判断是否有数据
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

        public override void UpdateReferenceDataStatus()
        {
            if (Source.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].Channels != null
                && DecodeDataSource.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].Channels[0] == Source)
            {
                DecodeDataSource.Instance.ReferenceDataSource[Source - ChannelIdExt.MinRChId].HasData = false;
            }
        }

        //数据检查
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

            if (_Source.IsAnalog() && laststamp != DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp;
                return true;
            }
            if (_Source.IsReference() && laststamp != DecodeDataHelper.Instance.ReferenceDataSource[_Source - ChannelIdExt.MinRChId].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.ReferenceDataSource[_Source - ChannelIdExt.MinRChId].TimeStamp;
                return true;
            }
            return false;
        }

        //数据处理
        internal override void ParsingData(ref CancellationToken token)
        {
            Int32 chindex = GetChIndex(_Source);
            UInt32 datalen = 0;
            Double samplerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(BusId,_Source, ref samplerate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId,_Source, ref datalen);
            Boolean needclear = false;
            if (MoreThanStorage() || chindex == -1 || datalen == 0 || samplerate == 0)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _ResultData.DecodeViewInfos = new IDecodeViewInfo[0];
                _EventInfos.Clear();

            }
            if (_NeedDecodeData || _NeedUpdateViewInfo)
            {
                /*输入参数*/
                D8B10BOptions options = new()
                {
                    BaudRate = Baudrate,
                };

                IntPtr edgepulseptr = IntPtr.Zero;
                GCHandle edgepulseshandle;

                //获取边沿
                TwoLevelEdgeInfo? node = DecodeDataHelper.Instance.GetEdgeInfo(BusId, startindex: 0, Source, ref token, ref needclear) as TwoLevelEdgeInfo;
                if (node == null)
                {
                    return;
                }
                _EdgePulsesList.Clear();
                DecodeDataHelper.Instance.GetTwoLevelPulses(ref node, ref _EdgePulsesList);
                PAM2EdgePulseSequence.Allocate(ref _EdgePulsesList, (UInt64)datalen, samplerate, out edgepulseptr, out edgepulseshandle);

                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;

                /*清除界面*/
                List<D8B10BDecodePacket> decodepackets = new List<D8B10BDecodePacket>();

                //开始解码
                D8B10BResult results;
                results.EventCount = 0;
                results.D8B10BEvent = IntPtr.Zero;

                if (!DecoderImpl.Decode8B10B(ref options, edgepulseptr, out results))
                {

                }
                /*释放边沿*/
                PAM2EdgePulseSequence.Free(ref edgepulseptr, ref edgepulseshandle);

                //解码结果获取             
                List<DecodeResultData> decoderesults = GetDecodeBuffer();

                if (_NeedUpdateViewInfo)
                {
                    _NeedUpdateViewInfo = false;
                    _EventInfos.Clear();
                    decoderesults.Clear();
                    _ResultData = new DecodeResultData();
                    ChangeBuffer();
                }
                Int32 eventsize = Marshal.SizeOf(typeof(D8B10BEvent));
                
                for (Int32 i = 0; i < results.EventCount; ++i)
                {
                    //创建一个用于界面展示的eventinfo
                    ProtocolEventInfo eventinfo = new ProtocolEventInfo();
                    var endindex = 0;//用以定位帧
                    eventinfo.Index = _EventInfos.Count;
                    eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));

                    //8B10BEvent提取
                    /*将事件指针数据转换为8B10BEvent结构体，提取事件的起始位置和时间。*/
                    D8B10BEvent d8b10bevent = (D8B10BEvent)Marshal.PtrToStructure(results.D8B10BEvent + i * eventsize, typeof(D8B10BEvent));

                    eventinfo.StartTimeByPs = base.GetTimeFromPosition(CalcPosition((Int64)d8b10bevent.StartIndex, Source, chindex), chindex);
                    eventinfo.StartPosition = CalcPosition((Int64)d8b10bevent.StartIndex, Source, chindex);

                    /*先将事件表用“--”填充*/
                    string temp_info = "--";
                    eventinfo.EventInofs[0] = (Encoding.Default.GetBytes(temp_info), 0);//Raw Data
                    eventinfo.EventInofs[1] = (Encoding.Default.GetBytes(temp_info), 0);//Data
                    eventinfo.EventInofs[2] = (Encoding.Default.GetBytes(temp_info), 0);//Control
                    eventinfo.EventInofs[3] = (Encoding.Default.GetBytes(temp_info), 0);//Error

                    //事件表显示原始数据
                    eventinfo.EventInofs[0] = (d8b10bevent.RawData, 0);

                    //错误标志
                    D8B10BPacketError status_flag = D8B10BPacketError.NoError;
                    D8B10BPacketType packetype = D8B10BPacketType.Data;
                    switch (d8b10bevent.Error)
                    {
                        case D8B10BStatusType.D8B10BNoError:
                            status_flag = D8B10BPacketError.NoError;
                            break;

                        case D8B10BStatusType.D8B10B4bitDisparityError:
                            status_flag = D8B10BPacketError.D4bitDisparityError;
                            string errorinfo = "Error:4Bit Disparity Error";
                            packetype = D8B10BPacketType.Error;
                            eventinfo.EventInofs[3] = (Encoding.Default.GetBytes(errorinfo), 0);
                            break;

                        case D8B10BStatusType.D8B10B6bitDisparityError:
                            status_flag = D8B10BPacketError.D6bitDisparityError;
                            string errorinfo1 = "Error:6Bit Disparity Error";
                            packetype = D8B10BPacketType.Error;
                            eventinfo.EventInofs[3] = (Encoding.Default.GetBytes(errorinfo1), 0);
                            break;

                        case D8B10BStatusType.D8B10BDSymbolError:
                            status_flag = D8B10BPacketError.SymbolError;
                            string errorinfo2 = "Error:Symbol Error";
                            packetype = D8B10BPacketType.Error;
                            eventinfo.EventInofs[3] = (Encoding.Default.GetBytes(errorinfo2), 0);
                            break;
                    }

                    if (d8b10bevent.Symbol.Kcode == Kcode.KNone)//Data数据
                    {
                        if (status_flag == D8B10BPacketError.NoError)
                        {
                            packetype = D8B10BPacketType.Data;
                        }
                        //正极性
                        if (d8b10bevent.Symbol.Polarity == SymbolPolarity.SymbolPositive)
                        {
                            /*数据装入图形显示*/
                            D8B10BDecodePacket data = new D8B10BDecodePacket(CalcPosition((long)d8b10bevent.StartIndex, Source, chindex),
                            CalcBitLenght((int)(d8b10bevent.EndIndex - d8b10bevent.StartIndex), Source, chindex), packetype)
                            {
                                Data = new Byte[] { (Byte)d8b10bevent.Symbol.Value },
                                _Title = "Data+",
                                PacketError = status_flag,
                                _BitCount = 8,
                            };
                            decodepackets.Add(data);

                            /*数据装入事件列表*/
                            eventinfo.EventInofs[1] = (data.Data, 8);
                            endindex = (Int32)d8b10bevent.EndIndex;
                        }
                        //负极性
                        else if (d8b10bevent.Symbol.Polarity == SymbolPolarity.SymbolNegative)
                        {
                            /*数据装入图形显示*/
                            D8B10BDecodePacket data = new D8B10BDecodePacket(CalcPosition((long)d8b10bevent.StartIndex, Source, chindex),
                            CalcBitLenght((int)(d8b10bevent.EndIndex - d8b10bevent.StartIndex), Source, chindex), packetype)
                            {
                                Data = new Byte[] { (Byte)d8b10bevent.Symbol.Value },
                                _Title = "Data-",
                                PacketError = status_flag,
                                _BitCount = 8,
                            };
                            decodepackets.Add(data);

                            /*数据装入事件列表*/
                            eventinfo.EventInofs[1] = (data.Data, 8);
                            endindex = (Int32)d8b10bevent.EndIndex;
                        }
                        else//symbol error
                        {
                            /*数据装入图形显示*/
                            D8B10BDecodePacket data = new D8B10BDecodePacket(CalcPosition((long)d8b10bevent.StartIndex, Source, chindex),
                            CalcBitLenght((int)(d8b10bevent.EndIndex - d8b10bevent.StartIndex), Source, chindex), D8B10BPacketType.Error)
                            {
                                Data = new Byte[2],
                                _Title = "Data",
                                PacketError = status_flag,
                                _BitCount = 16,
                            };
                            byte[] raw_data = BitConverter.GetBytes(d8b10bevent.Symbol.Value);
                            Array.Reverse(raw_data);
                            Array.Copy(raw_data, data.Data, 2);
                            decodepackets.Add(data);

                            /*数据装入事件列表*/
                            eventinfo.EventInofs[1] = (raw_data, 16);
                            endindex = (Int32)d8b10bevent.EndIndex;
                        }
                    }
                    else //K码数据
                    {
                        if(status_flag == D8B10BPacketError.NoError)
                        {
                            packetype = D8B10BPacketType.Kcode;
                        }
                        //正极性
                        if (d8b10bevent.Symbol.Polarity == SymbolPolarity.SymbolPositive)
                        {
                            /*数据装入图形显示*/
                            D8B10BDecodePacket kcode = new D8B10BDecodePacket(CalcPosition((long)d8b10bevent.StartIndex, Source, chindex),
                            CalcBitLenght((int)(d8b10bevent.EndIndex - d8b10bevent.StartIndex), Source, chindex), packetype)
                            {
                                Data = new Byte[] { (Byte)d8b10bevent.Symbol.Value },
                                _Title = "Control+",
                                PacketError = status_flag,
                                _BitCount = 8,

                            };
                            decodepackets.Add(kcode);

                            /*数据装入事件列表*/
                            eventinfo.EventInofs[2] = (kcode.Data, 8);
                            endindex = (Int32)d8b10bevent.EndIndex;
                        }
                        //负极性
                        else if (d8b10bevent.Symbol.Polarity == SymbolPolarity.SymbolNegative)
                        {
                            /*数据装入图形显示*/
                            D8B10BDecodePacket kcode = new D8B10BDecodePacket(CalcPosition((long)d8b10bevent.StartIndex, Source, chindex),
                            CalcBitLenght((int)(d8b10bevent.EndIndex - d8b10bevent.StartIndex), Source, chindex), packetype)
                            {
                                Data = new Byte[] { (Byte)d8b10bevent.Symbol.Value },
                                _Title = "Control-",
                                PacketError = status_flag,
                                _BitCount = 8,
                            };
                            decodepackets.Add(kcode);

                            /*数据装入事件列表*/
                            eventinfo.EventInofs[2] = (kcode.Data, 8);
                            endindex = (Int32)d8b10bevent.EndIndex;
                        }
                    }
                    
                    eventinfo.EndPosition = CalcPosition(endindex, Source, chindex);
                    eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, chindex);
                    _EventInfos.Add(eventinfo);
                }

                if (!DecoderImpl.Free8B10B(ref results))
                    return;

                _ResultData.DecodeViewInfos = decodepackets.ToArray();
                decoderesults.Add(_ResultData);

            }
        }

        //输入参数赋值
        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.Protocol8b10bOptions()
            {
                Source = Source,
                Threshold = _Threshold,
                BaudRate = Baudrate,
            };

        }
    }
}


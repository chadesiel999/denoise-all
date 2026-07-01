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
using static ScopeX.Core.Decode.D8B10BDecodeModel;

namespace ScopeX.Core.Decode
{
    internal partial class Mlt3DecodeModel : ProtocolModel
    {

        private DecodeResultData _ResultData = new DecodeResultData();
        private List<PAM3EdgePulse> _EdgePulsesList = new List<PAM3EdgePulse>();
        private Dictionary<string, Int32> _EventDict = new Dictionary<string, Int32>();

        //事件显示
        public override IReadOnlyList<String> EventInfoTitles { get; } = (new List<String>()
        {
            "Index",
            "Start Time",
            "Data",
            "Error",
        }).AsReadOnly();

        //通道选择
        private ChannelId _Source = ChannelId.C1;
        public ChannelId Source
        {
            get { return _Source; }
            set { UpdateProperty(ref _Source, value); }
        }

        //高门限阈值
        public Double MaxHighThreshold => 10;
        public Double MinHighThreshold => -10;
        private Double _HighThreshold = 0;//默认值
        public Double HighThreshold
        {
            get { return _HighThreshold * TryGetChannelGain(Source); }
            set { UpdateProperty(ref _HighThreshold, value / TryGetChannelGain(Source)); }
        }

        //低门限阈值
        public Double MaxLowThreshold => 10;
        public Double MinLowThreshold => -10;
        private Double _LowThreshold = 0;//默认值
        public Double LowThreshold
        {
            get { return _LowThreshold * TryGetChannelGain(Source); }
            set { UpdateProperty(ref _LowThreshold, value / TryGetChannelGain(Source)); }
        }

        //数据速率
        public Int64 MaxBaudrate => 1000000000;//1000M 
        public Int64 MinBaudrate => 1;
        private Int64 _Baudrate = 100000000;//默认100M以太网
        public Int64 Baudrate
        {
            get { return _Baudrate; }
            set { UpdateProperty(ref _Baudrate, value); }
        }

        //零电平保持个数
        public UInt32 MaxZeroCount => 100;
        public UInt32 MinxZeroCount => 1;
        private UInt32 _ZeroCount = 1;//默认
        public UInt32 ZeroCount
        {
            get { return _ZeroCount; }
            set { UpdateProperty(ref _ZeroCount, value); }
        }

        //单位？
        public String Unit => GetChannelUnit(Source);

        public Mlt3DecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id, SerialProtocolType.Mlt3, isTrigDecode)
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

            Double[] thresholds;
            thresholds = new Double[2];
            thresholds[0] = HighThreshold;
            thresholds[1] = LowThreshold;
        
            if (Source.IsReference() && prsnt.VuDatabase != null && prsnt.VuDatabase.Current != null)
            {
                return DecodeDataHelper.ReferenceHasData(Source, thresholds);
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
            var source = _Source;
            Int32 chindex = GetChIndex(source);
            UInt32 datalen = 0;
            Double samplerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, source, ref samplerate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, source, ref datalen);
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(source, out var cp);
            Boolean needclear = false;
            if (MoreThanStorage() || chindex == 0 || datalen == 0 || samplerate == 0)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _ResultData.DecodeViewInfos = new IDecodeViewInfo[0];
                _EventInfos.Clear();

            }
            if (_NeedDecodeData || _NeedUpdateViewInfo)
            {
                /*输入参数*/
                Mlt3Options options = new()
                {
                    BaudRate = Baudrate,
                    KeepZeroCount = ZeroCount,
                };

                IntPtr edgepulseptr = IntPtr.Zero;
                GCHandle edgepulseshandle;

                //获取边沿
                ThreeLevelEdgeInfo? node = DecodeDataHelper.Instance.GetThreeLevelEdgeInfo(BusId, 0, source, ref token, ref needclear) as ThreeLevelEdgeInfo;
                if (node == null)
                {
                    return;
                }
                _EdgePulsesList.Clear();
                DecodeDataHelper.Instance.GetThreeLevelPulses(ref node, ref _EdgePulsesList);
                PAM3EdgePulseSequence.Allocate(ref _EdgePulsesList, (UInt64)datalen, samplerate, out edgepulseptr, out edgepulseshandle);

                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;

                /*清除界面*/
                List<Mlt3DecodePacket> decodepackets = new List<Mlt3DecodePacket>();

                //开始解码
                Mlt3Result results;
                results.EventCount = 0;
                results.Mlt3Event = IntPtr.Zero;
                 if (!DecoderImpl.DecodeMLT3(ref options, edgepulseptr, out results))
                {

                }

                /*释放边沿*/
                PAM3EdgePulseSequence.Free(ref edgepulseptr, ref edgepulseshandle);

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
                Int32 eventsize = Marshal.SizeOf(typeof(Mlt3Event));

                for (Int32 i = 0; i < results.EventCount; ++i)
                {
                    //创建一个用于界面展示的eventinfo
                    ProtocolEventInfo eventinfo = new ProtocolEventInfo();
                    var endindex = 0;//用以定位帧
                    eventinfo.Index = _EventInfos.Count;
                    eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));

                    Mlt3Event mlt3event = (Mlt3Event)Marshal.PtrToStructure(results.Mlt3Event + i * eventsize, typeof(Mlt3Event));

                    eventinfo.StartTimeByPs = base.GetTimeFromPosition(CalcPosition((Int64)mlt3event.start_index, source, chindex), chindex);
                    eventinfo.StartPosition = CalcPosition((Int64)mlt3event.start_index, source, chindex);

                    /*先将事件表用“--”填充*/
                    string temp_info = "--";
                    eventinfo.EventInofs[0] = (Encoding.Default.GetBytes(temp_info), 0);//Data
                    eventinfo.EventInofs[1] = (Encoding.Default.GetBytes(temp_info), 0);//Error

                    //判断错误状态
                    Mlt3PacketError status_flag = Mlt3PacketError.NoError;
                    switch (mlt3event.error_type)
                    {
                        case MLT3Error.MLT3_ERROR_NONE:
                            status_flag = Mlt3PacketError.NoError;
                            break;

                        case MLT3Error.MLT3_ERROR_JUMP_ILLEGAL:
                            status_flag = Mlt3PacketError.MLT3_ERROR_JUMP_ILLEGAL;
                            string errorinfo = "Error:JUMP ILLEGAL";
                            eventinfo.EventInofs[1] = (Encoding.Default.GetBytes(errorinfo), 0);
                            break;

                        case MLT3Error.MLT3_ERROR_JUMP_FAST:
                            status_flag = Mlt3PacketError.MLT3_ERROR_JUMP_FAST;
                            string errorinfo1 = "Error:JUMP FAST;";
                            eventinfo.EventInofs[1] = (Encoding.Default.GetBytes(errorinfo1), 0);
                            break;

                        case MLT3Error.MLT3_ERROR_ZERO_TOOMUCH:
                            status_flag = Mlt3PacketError.MLT3_ERROR_ZERO_TOOMUCH;
                            string errorinfo2 = "Error:Too Many Zero Level";
                            eventinfo.EventInofs[1] = (Encoding.Default.GetBytes(errorinfo2), 0);
                            break;
                    }
                    if (mlt3event.value == 0)//零电平
                    {
                        Mlt3DecodePacket data = new Mlt3DecodePacket(CalcPosition((long)mlt3event.start_index, source, chindex),
                             CalcBitLenght((int)(mlt3event.len), source, chindex), status_flag == Mlt3PacketError.NoError ?Mlt3PacketType.ZeroPacket: Mlt3PacketType.Errror)
                        {
                            Data = new Byte[] { (Byte)mlt3event.value },
                            _Title = "Data",
                            PacketError = status_flag,
                            _BitCount = 1,
                        };
                        decodepackets.Add(data);
                        /*数据装入事件列表*/
                        eventinfo.EventInofs[0] = (data.Data, 2);
                    }
                    else//一电平
                    {
                        Mlt3DecodePacket data = new Mlt3DecodePacket(CalcPosition((long)mlt3event.start_index, source, chindex),
                         CalcBitLenght((int)(mlt3event.len), source, chindex), status_flag == Mlt3PacketError.NoError ? Mlt3PacketType.OnePacket : Mlt3PacketType.Errror)
                        {
                            Data = new Byte[] { (Byte)mlt3event.value },
                            _Title = "Data",
                            PacketError = status_flag,
                            _BitCount = 1,
                        };
                        decodepackets.Add(data);
                        /*数据装入事件列表*/
                        eventinfo.EventInofs[0] = (data.Data, 2);
                    }
                    endindex = (Int32)(mlt3event.start_index + mlt3event.len);
                    eventinfo.EndPosition = CalcPosition(endindex, source, chindex);
                    eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, chindex);
                    _EventInfos.Add(eventinfo);
                }

                if (!DecoderImpl.FreeMLT3(ref results))
                    return;

                _ResultData.DecodeViewInfos = decodepackets.ToArray();
                decoderesults.Add(_ResultData);
            }
        }

        //输入参数赋值
        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolMlt3Options()
            {
                Source = Source,
                HighThreshold = _HighThreshold,
                LowThreshold = _LowThreshold,
                BaudRate = Baudrate,
                ZeroCount = ZeroCount,
            };

        }
    }
}


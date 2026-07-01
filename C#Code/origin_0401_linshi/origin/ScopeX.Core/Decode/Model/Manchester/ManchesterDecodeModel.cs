using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ScopeX.Core.Decode.EdgeInfoCPP;
using static ScopeX.ComModel.ProtocolManchester;
using Microsoft.VisualBasic.FileIO;
using System.Reflection;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.PTG;
using ScopeX.Hardware.Driver;
using System.Xml.Linq;
namespace ScopeX.Core.Decode
{
    internal partial class ManchesterDecodeModel : ProtocolModel
    {

        private DecodeResultData _ResultData = new DecodeResultData();
        private List<PAM2EdgePulse> _EdgePulsesList = new List<PAM2EdgePulse>();
        private Dictionary<string, Int32> _EventDict = new Dictionary<string, Int32>();

        //事件显示
        public override IReadOnlyList<String> EventInfoTitles { get; } = (new List<String>()
        {
            "Index",
            "Start Time",
            "Sync",
            "Header",
            "Data",
            "Trailer",
            "Parity",
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
        public Int64 MaxBaudrate => 1000000000;//最大1Gb/s
        public Int64 MinBaudrate => 1;//最小1b/s
        private Int64 _Baudrate = 125000;
        public Int64 Baudrate
        {
            get { return _Baudrate; }
            set { UpdateProperty(ref _Baudrate, value); }
        }

        //选择边沿
        private ProtocolManchester.Polarity _Polarity = ProtocolManchester.Polarity.Rising;
        public ProtocolManchester.Polarity Polarity
        {
            get { return _Polarity; }
            set { UpdateProperty(ref _Polarity, value); }
        }

        //位顺序
        private ProtocolManchester.MSB_LSB _MSB_LSB;
        public ProtocolManchester.MSB_LSB MSB_LSB
        {
            get { return _MSB_LSB; }
            set { UpdateProperty(ref _MSB_LSB, value); }
        }

        //奇偶性
        private ProtocolManchester.OddEvenCheck _OddEvenCheck;
        public ProtocolManchester.OddEvenCheck OddEvenCheck
        {
            get { return _OddEvenCheck; }
            set { UpdateProperty(ref _OddEvenCheck, value); }
        }

        //SYNC
        public Byte MaxSyncSize => 32;
        public Byte MinSyncSize => 0;
        private Byte _SyncSize = 2;
        public Byte SyncSize
        {
            get { return _SyncSize; }
            set { UpdateProperty(ref _SyncSize, value); }
        }

        //HEADER
        public Byte MaxHeaderSize => 128;
        public Byte MinHeaderSize => 0;
        private Byte _HeaderSize = 3;
        public Byte HeaderSize
        {
            get { return _HeaderSize; }
            set { UpdateProperty(ref _HeaderSize, value); }
        }

        //DATA
        public Byte MaxDataSize => 32;
        public Byte MinDataSize => 1;
        private Byte _DataSize = 6;
        public Byte DataSize
        {
            get { return _DataSize; }
            set { UpdateProperty(ref _DataSize, value); }
        }

        //TRAILER
        public Byte MaxTrailerSize => 128;
        public Byte MinTrailerSize => 0;
        private Byte _TrailerSize = 2;
        public Byte TrailerSize
        {
            get { return _TrailerSize; }
            set { UpdateProperty(ref _TrailerSize, value); }
        }

        //IDLE BIT
        public Double MaxIdleBitSize => 32;
        public Double MinIdleBitSize => 1.2;
        private Double _IdleBitSize = 1.2;
        public Double IdleBitSize
        {
            get { return _IdleBitSize; }
            set { UpdateProperty(ref _IdleBitSize, value); }
        }

        //容限
        public Double MaxTolerance => 50;
        public Double MinTolerance => 1;
        private Double _Tolerance = 5;
        public Double Tolerance
        {
            get { return _Tolerance; }
            set { UpdateProperty(ref _Tolerance, value); }
        }

        //数据包视图flag
        private ProtocolManchester.DataView _DataFlag;
        public ProtocolManchester.DataView DataFlag
        {
            get { return _DataFlag; }
            set { UpdateProperty(ref _DataFlag, value); }
        }

        //字数
        public Byte MaxDataNum => 255;
        public Byte MinDataNum => 1;
        private Byte _DataNum = 1;
        public Byte DataNum
        {
            get { return _DataNum; }
            set { UpdateProperty(ref _DataNum, value); }
        }

        //开始索引
        public Byte MaxStartEdge => 8;
        public Byte MinStartEdge => 1;
        private Byte _StartEdge = 1;
        public Byte StartEdge
        {
            get { return _StartEdge; }
            set { UpdateProperty(ref _StartEdge, value); }
        }

        //单位？
        public String Unit => GetChannelUnit(Source);
   
        public ManchesterDecodeModel(ChannelId id,Boolean isTrigDecode = false) : base(id,SerialProtocolType.Manchester, isTrigDecode)
        {

        }

        private ulong pre_endindex = 0;
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
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, _Source, ref samplerate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, _Source, ref datalen);
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
                ManchesterOptions options = new()
                {
                    BaudRate = Baudrate,
                    SyncSize = SyncSize,
                    TrailerSize = TrailerSize,
                    Tolerance = Tolerance * 0.01,
                    StartEdgeSize = StartEdge,
                    WordSize = DataSize,
                    WordDataNum = DataNum,
                    ByteOrder = MSB_LSB,
                    CancelFlag = _CancelFlagPtr,
                    EdgePolarity = Polarity,
                    HeaderSize = HeaderSize,
                    DataViewFlag = DataFlag,
                    IdleBitsSize = IdleBitSize,
                    OddEvenCheck = OddEvenCheck,

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
                List<ManchesterDecodePacket> decodepackets = new List<ManchesterDecodePacket>();

                //开始解码
                ManchesterResult results;
                results.EventCount = 0;
                results.ManchesterEvent = IntPtr.Zero;

                if (!DecoderImpl.DecodeManchester(ref options, edgepulseptr, out results))
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
                Int32 bytefieldsize = Marshal.SizeOf(typeof(ManchesterByteField));
                Int32 eventsize = Marshal.SizeOf(typeof(ManchesterEvent));

                for (Int32 i = 0; i < results.EventCount; ++i)
                {
                    //创建一个用于界面展示的eventinfo
                    ProtocolEventInfo eventinfo = new ProtocolEventInfo();
                    var endindex = 0;//用以定位帧
                    eventinfo.Index = _EventInfos.Count;
                    eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));

                    //ManchesterEvent提取

                    /*将事件指针数据转换为ManchesterEvent结构体，提取事件的起始位置和时间。*/
                    ManchesterEvent manchesterenevt = (ManchesterEvent)Marshal.PtrToStructure(results.ManchesterEvent + i * eventsize, typeof(ManchesterEvent));

                    eventinfo.StartTimeByPs = base.GetTimeFromPosition(CalcPosition((Int64)manchesterenevt.StartIndex, Source, chindex), chindex);
                    eventinfo.StartPosition = CalcPosition((Int64)manchesterenevt.StartIndex, Source, chindex);

                    /*先将事件表用“--”填充*/
                    string temp_info = "--";
                    eventinfo.EventInofs[0] = (Encoding.Default.GetBytes(temp_info), 0);//Sync
                    eventinfo.EventInofs[1] = (Encoding.Default.GetBytes(temp_info), 0);//Header
                    eventinfo.EventInofs[2] = (Encoding.Default.GetBytes(temp_info), 0);//Data
                    eventinfo.EventInofs[3] = (Encoding.Default.GetBytes(temp_info), 0);//Trailer
                    eventinfo.EventInofs[4] = (Encoding.Default.GetBytes(temp_info), 0);//Parity
                    eventinfo.EventInofs[5] = (Encoding.Default.GetBytes(temp_info), 0);//Error

                    int offset = 0;
                    byte[] data = new byte[1];
                    UInt64 len = 0;
                    if (options.DataViewFlag == ProtocolManchester.DataView.Open)
                    {
                        data = new byte[(uint)options.WordDataNum * (((uint)options.WordSize + 7) / 8)];
                        len = options.WordDataNum;
                    }
                    else if (options.DataViewFlag == ProtocolManchester.DataView.Close)
                    {
                        data = new byte[manchesterenevt.DataNum];
                        len = manchesterenevt.DataNum;
                    }

                    //无法构成帧错误
                    if (manchesterenevt.ManchesterStatusType == ManchesterStatusType.ManchesterUnframedError)
                    {
                        /*事件错误显示*/
                        string errorinfo = "Error:Unframed";
                        eventinfo.EventInofs[5] = (Encoding.Default.GetBytes(errorinfo), 0);
                        eventinfo.EndPosition = CalcPosition((long)manchesterenevt.EndIndex, Source, chindex);//计算事件长度，用以定位帧
                        eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, chindex);
                        ulong len1 = manchesterenevt.EndIndex - manchesterenevt.StartIndex;
                        /*数据装入图形显示*/
                        ManchesterUnframedDecodePacket Unframed = new ManchesterUnframedDecodePacket(CalcPosition((long)manchesterenevt.StartIndex, Source, chindex),
                        CalcBitLenght((int)(len1), Source, chindex))
                        {

                        };
                        decodepackets.Add(Unframed);
                    }
                    else
                    {
                        //根据Event中的ByteFieldNum，把每个byte都取出来判断类型
                        for (Int32 j = 0; j < manchesterenevt.ByteFieldNum; j++)
                        {

                            /*从Event中提取ByteField,判断类型再装入eventinfo用于显示*/
                            ManchesterByteField bytefieldinfo = (ManchesterByteField)Marshal.PtrToStructure(manchesterenevt.ByteField + j * bytefieldsize, typeof(ManchesterByteField));

                            /*判断类型*/
                            switch (bytefieldinfo.ByteFieldType)
                            {
                                case ManchesterByteFieldType.ByteFieldSync:
                                    {
                                        /*数据装入事件列表*/
                                        byte[] syncdata = new byte[bytefieldinfo.ValueSize];
                                        Marshal.Copy(bytefieldinfo.Value, syncdata, 0, (int)bytefieldinfo.ValueSize);
                                        eventinfo.EventInofs[0] = (syncdata, options.SyncSize);
                                        endindex = (Int32)bytefieldinfo.EndIndex;
                                        /*错误显示*/
                                        if (bytefieldinfo.ByteStaus == ManchesterByteStatusType.ByteError)
                                        {
                                            string errorinfo = "Error:Bad manchester enoding";
                                            eventinfo.EventInofs[5] = (Encoding.Default.GetBytes(errorinfo), 0);
                                        }

                                        /*数据装入图形显示*/
                                        ManchesterSyncDecodePacket Sync = new ManchesterSyncDecodePacket(CalcPosition((long)bytefieldinfo.StartIndex, Source, chindex),
                                        CalcBitLenght((int)(bytefieldinfo.EndIndex - bytefieldinfo.StartIndex), Source, chindex))
                                        {
                                            /*显示数据*/
                                            Data = new byte[bytefieldinfo.ValueSize],
                                            /*显示位数*/
                                            _BitCount = options.SyncSize,
                                            /*错误显示*/
                                            SyncError = bytefieldinfo.ByteStaus == ManchesterByteStatusType.ByteNoError ? false : true,
                                        };

                                        Marshal.Copy(bytefieldinfo.Value, Sync.Data, 0, (int)(bytefieldinfo.ValueSize));
                                        decodepackets.Add(Sync);
                                        break;
                                    }
                                case ManchesterByteFieldType.ByteFieldHeader:
                                    {
                                        /*数据*/
                                        byte[] Headerdata = new byte[bytefieldinfo.ValueSize];
                                        Marshal.Copy(bytefieldinfo.Value, Headerdata, 0, (int)bytefieldinfo.ValueSize);
                                        eventinfo.EventInofs[1] = (Headerdata, options.HeaderSize);
                                        endindex = (Int32)bytefieldinfo.EndIndex;
                                        /*错误显示*/
                                        if (bytefieldinfo.ByteStaus == ManchesterByteStatusType.ByteError)
                                        {
                                            string errorinfo = "Error:Bad manchester enoding";
                                            eventinfo.EventInofs[5] = (Encoding.Default.GetBytes(errorinfo), 0);
                                        }

                                        ManchesterHeaderDecodePacket Header = new ManchesterHeaderDecodePacket(CalcPosition((long)bytefieldinfo.StartIndex, Source, chindex),
                                        CalcBitLenght((int)(bytefieldinfo.EndIndex - bytefieldinfo.StartIndex), Source, chindex))
                                        {
                                            Data = new byte[bytefieldinfo.ValueSize],
                                            _BitCount = options.HeaderSize,
                                            HeaderError = bytefieldinfo.ByteStaus == ManchesterByteStatusType.ByteNoError ? false : true,

                                        };
                                        Marshal.Copy(bytefieldinfo.Value, Header.Data, 0, (int)bytefieldinfo.ValueSize);
                                        decodepackets.Add(Header);
                                        break;
                                    }
                                case ManchesterByteFieldType.ByteFieldData:
                                    {
                                        Marshal.Copy(bytefieldinfo.Value, data, offset, (int)bytefieldinfo.ValueSize);
                                        eventinfo.EventInofs[2] = (data, (uint)len * (((uint)options.WordSize + 7) / 8) * 8);
                                        offset += (int)bytefieldinfo.ValueSize;
                                        endindex = (Int32)bytefieldinfo.EndIndex;
                                        /*错误显示*/
                                        if (bytefieldinfo.ByteStaus == ManchesterByteStatusType.ByteError)
                                        {
                                            string errorinfo = "Error:Bad manchester enoding";
                                            eventinfo.EventInofs[5] = (Encoding.Default.GetBytes(errorinfo), 0);
                                        }
                                        ManchesterDataDecodePacket Data = new ManchesterDataDecodePacket(CalcPosition((long)bytefieldinfo.StartIndex, Source, chindex),
                                        CalcBitLenght((int)(bytefieldinfo.EndIndex - bytefieldinfo.StartIndex), Source, chindex))
                                        {
                                            Data = new byte[bytefieldinfo.ValueSize],
                                            _BitCount = options.WordSize,
                                            DataError = bytefieldinfo.ByteStaus == ManchesterByteStatusType.ByteNoError ? false : true,
                                        };
                                        Marshal.Copy(bytefieldinfo.Value, Data.Data, 0, (int)bytefieldinfo.ValueSize);
                                        decodepackets.Add(Data);
                                        break;
                                    }
                                case ManchesterByteFieldType.ByteFieldTrailer:
                                    {
                                        /*数据*/
                                        byte[] Trailerdata = new byte[bytefieldinfo.ValueSize];
                                        Marshal.Copy(bytefieldinfo.Value, Trailerdata, 0, (int)bytefieldinfo.ValueSize);
                                        eventinfo.EventInofs[3] = (Trailerdata, options.TrailerSize);
                                        endindex = (Int32)bytefieldinfo.EndIndex;
                                        /*错误显示*/
                                        if (bytefieldinfo.ByteStaus == ManchesterByteStatusType.ByteError)
                                        {
                                            string errorinfo = "Error:Bad manchester enoding";
                                            eventinfo.EventInofs[5] = (Encoding.Default.GetBytes(errorinfo), 0);
                                        }
                                        ManchesterTrailerDecodePacket Trailer = new ManchesterTrailerDecodePacket(CalcPosition((long)bytefieldinfo.StartIndex, Source, chindex),
                                        CalcBitLenght((int)(bytefieldinfo.EndIndex - bytefieldinfo.StartIndex), Source, chindex))
                                        {
                                            Data = new byte[bytefieldinfo.ValueSize],
                                            _BitCount = options.TrailerSize,
                                            TrailerError = bytefieldinfo.ByteStaus == ManchesterByteStatusType.ByteNoError ? false : true,

                                        };
                                        Marshal.Copy(bytefieldinfo.Value, Trailer.Data, 0, (int)bytefieldinfo.ValueSize);
                                        decodepackets.Add(Trailer);
                                        break;
                                    }
                            }
                        }
                        /*判断是否存在奇偶校验*/
                        if (manchesterenevt.ParityCheckStatusTypes != ParityCheckStatusTypes.ParityCheckNone)
                        {
                            /*将ParityBit存入EventInfo*/
                            byte[] ParityBitData = new byte[1];
                            ParityBitData[0] = manchesterenevt.ParityBit.Value;
                            eventinfo.EventInofs[4] = (ParityBitData, 2);

                            /*错误显示*/
                            if (manchesterenevt.ParityCheckStatusTypes == ParityCheckStatusTypes.ParityCheckError)
                            {
                                string errorinfo = "Error:Parity";
                                eventinfo.EventInofs[5] = (Encoding.Default.GetBytes(errorinfo), 0);
                            }

                            ManchesterParityBitDecodePacket ParityBit = new ManchesterParityBitDecodePacket(CalcPosition((long)manchesterenevt.ParityBit.StartIndex, Source, chindex),
                                       CalcBitLenght((int)(manchesterenevt.ParityBit.EndIndex - manchesterenevt.ParityBit.StartIndex), Source, chindex))
                            {
                                Data = new byte[1] { manchesterenevt.ParityBit.Value },
                                _BitCount = 1,
                                ParityError = manchesterenevt.ParityCheckStatusTypes == ParityCheckStatusTypes.ParityCheckTrue ? false : true,
                            };
                            decodepackets.Add(ParityBit);

                        }
                    }
                    eventinfo.EndPosition = CalcPosition(endindex, Source, chindex);
                    eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, chindex);
                    _EventInfos.Add(eventinfo);
                }

                if (!DecoderImpl.FreeManchester(ref results))
                    return;

                _ResultData.DecodeViewInfos = decodepackets.ToArray();
                decoderesults.Add(_ResultData);

            }
        }

        //输入参数赋值
        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolManchesterOptions()
            {
                Source1 = Source,
                Threshold = _Threshold,
                BaudRate = Baudrate,
                Polarity = Polarity,
                MSB_LSB = MSB_LSB,
                OddEvenCheck = OddEvenCheck,
                SyncSize = SyncSize,
                HeaderSize = HeaderSize,
                DataSize = DataSize,
                TrailerSize = TrailerSize,
                Idle_Size = IdleBitSize,
                Data_Num = DataSize,
                Flag = DataFlag,
                StartEdge = StartEdge,
                Tolerance = Tolerance * 0.01,
            };

        }
    }
}


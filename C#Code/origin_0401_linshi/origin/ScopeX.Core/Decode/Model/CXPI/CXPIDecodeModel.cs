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
using static ScopeX.Core.Decode.ManchesterDecodeModel;
using NPOI.OpenXmlFormats.Spreadsheet;
namespace ScopeX.Core.Decode
{
    internal partial class CXPIDecodeModel : ProtocolModel
    {
        private uint _CrcNum = 8;
        private string _ErrorMessage = string.Empty;
        private DecodeResultData _ResultData = new DecodeResultData();
        private List<PAM2EdgePulse> _EdgePulsesList = new List<PAM2EdgePulse>();
        private Dictionary<string, Int32> _EventDict = new Dictionary<string, Int32>();

        //事件显示
        public override IReadOnlyList<String> EventInfoTitles { get; } = (new List<String>()
        {
            "Index",
            "Start Time",
            "Frame Type",
            "PTYPE FrameID",
            "Frame ID",
            "Counter",
            "Sleep",
            "Wakeup",
            "DLC",
            "Ext DLC",
            "PID Parity",
            "Data",
            "PTYPPE Parity",
            "CRC_LSB_MSB",
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
        public Int64 MaxBaudrate => 20000;//最大1Gb/s
        public Int64 MinBaudrate => 1;//最小1b/s
        private Int64 _Baudrate = 2200;//默认
        public Int64 Baudrate
        {
            get { return _Baudrate; }
            set { UpdateProperty(ref _Baudrate, value); }
        }

        //单位？
        public String Unit => GetChannelUnit(Source);

        public CXPIDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id, SerialProtocolType.CXPI, isTrigDecode)
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
                CXPIOptions options = new()
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
                List<CXPIDecodePacket> decodepackets = new List<CXPIDecodePacket>();

                //开始解码
                CXPIResult results;
                results.EventCount = 0;
                results.CXPIEvent = IntPtr.Zero;

                if (!DecoderImpl.DecodeCXPI(ref options, edgepulseptr, out results))
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
                Int32 bytefieldsize = Marshal.SizeOf(typeof(CXPIByteField));
                Int32 eventsize = Marshal.SizeOf(typeof(CXPIEvent));

                for (Int32 i = 0; i < results.EventCount; ++i)
                {
                    //创建一个用于界面展示的eventinfo
                    ProtocolEventInfo eventinfo = new ProtocolEventInfo();
                    var endindex = 0;//用以定位帧
                    eventinfo.Index = _EventInfos.Count;
                    eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));

                    //CXPIEvent提取
                    /*将事件指针数据转换为CXPIEvent结构体，提取事件的起始位置和时间。*/
                    CXPIEvent cxpierenevt = (CXPIEvent)Marshal.PtrToStructure(results.CXPIEvent + i * eventsize, typeof(CXPIEvent));

                    eventinfo.StartTimeByPs = base.GetTimeFromPosition(CalcPosition((Int64)cxpierenevt.StartIndex, Source, chindex), chindex);

                    eventinfo.StartPosition = CalcPosition((Int64)cxpierenevt.StartIndex, Source, chindex);

                    /*先将事件表用“--”填充*/
                    string temp_info = "--";
                    eventinfo.EventInofs[0] = (Encoding.Default.GetBytes(temp_info), 0);//Frame Type
                    eventinfo.EventInofs[1] = (Encoding.Default.GetBytes(temp_info), 0);//PTYPE frameID
                    eventinfo.EventInofs[2] = (Encoding.Default.GetBytes(temp_info), 0);//Frame ID
                    eventinfo.EventInofs[3] = (Encoding.Default.GetBytes(temp_info), 0);//Counter
                    eventinfo.EventInofs[4] = (Encoding.Default.GetBytes(temp_info), 0);//Sleep
                    eventinfo.EventInofs[5] = (Encoding.Default.GetBytes(temp_info), 0);//Wakeup
                    eventinfo.EventInofs[6] = (Encoding.Default.GetBytes(temp_info), 0);//DLC
                    eventinfo.EventInofs[7] = (Encoding.Default.GetBytes(temp_info), 0);//Ext DLC
                    eventinfo.EventInofs[8] = (Encoding.Default.GetBytes(temp_info), 0);//PID Parity
                    eventinfo.EventInofs[9] = (Encoding.Default.GetBytes(temp_info), 0);//Data
                    eventinfo.EventInofs[10] = (Encoding.Default.GetBytes(temp_info), 0);//PTYPE Parity
                    eventinfo.EventInofs[11] = (Encoding.Default.GetBytes(temp_info), 0);//CRC_LSB_MSB
                    eventinfo.EventInofs[12] = (Encoding.Default.GetBytes(temp_info), 0);//Error


                    //用于装入事件表
                    List<byte> datalist = new List<byte>();


                    //根据Event中的ByteFieldNum，把每个byte都取出来判断类型
                    for (Int32 j = 0; j < cxpierenevt.ByteFieldNum; j++)
                    {
                        /*事件起始标记*/
                        CXPIStartDecodePacket Start = new CXPIStartDecodePacket(CalcPosition((long)cxpierenevt.Start, Source, chindex),1)
                        {
                        };
                        decodepackets.Add(Start);

                        /*从Event中提取ByteField,判断类型再装入eventinfo用于显示*/
                        CXPIByteField bytefieldinfo = (CXPIByteField)Marshal.PtrToStructure(cxpierenevt.ByteField + j * bytefieldsize, typeof(CXPIByteField));
                        
                        /*判断类型*/
                        switch (bytefieldinfo.FieldType)
                        {
                            case CXPIFieldType.CxpiPtype:
                                {
                                    /*数据装入图形显示*/
                                    CXPIStartBitDecodePacket StartBit = new CXPIStartBitDecodePacket(CalcPosition((long)bytefieldinfo.StartBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.StartBit.EndIndex - bytefieldinfo.StartBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.StartBit.Value },
                                        _BitCount = 1,
                                    };
                                    CXPIPtypeDecodePacket Ptype = new CXPIPtypeDecodePacket(CalcPosition((long)bytefieldinfo.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.EndIndex - bytefieldinfo.StartIndex), Source, chindex))
                                    {  
                                        Data = new Byte[] { (Byte)bytefieldinfo.Value },
                                        _BitCount = 7,
                                    };

                                    CXPIParityBitDecodePacket Parity = new CXPIParityBitDecodePacket(CalcPosition((long)bytefieldinfo.ParityBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.ParityBit.EndIndex - bytefieldinfo.ParityBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.ParityBit.Value },
                                        _BitCount = 1,
                                        ParityBitError = bytefieldinfo.StatusType == CXPIStatusType.CxpiParityError,
                                    };

                                    CXPIStopBitDecodePacket StopBit = new CXPIStopBitDecodePacket(CalcPosition((long)bytefieldinfo.StopBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.StopBit.EndIndex - bytefieldinfo.StopBit.StartIndex), Source, chindex))
                                    {                    
                                        Data = new Byte[] { (Byte)bytefieldinfo.StopBit.Value },                             
                                        _BitCount = 1,                             
                                    };
                                    decodepackets.Add(StartBit);
                                    decodepackets.Add(Ptype);
                                    decodepackets.Add(Parity);
                                    decodepackets.Add(StopBit);

                                    /*数据装入事件列表*/
                                    eventinfo.EventInofs[1] = (Ptype.Data, 8);
                                    eventinfo.EventInofs[10] = (Parity.Data, 1);
                                    if (bytefieldinfo.StatusType == CXPIStatusType.CxpiParityError)
                                    {
                                        string errinfo = "Ptype Parity Error,";
                                       _ErrorMessage += errinfo;
                                    }
                                    endindex = (Int32)bytefieldinfo.StopBit.EndIndex;
                                    break;
                                }
                            case CXPIFieldType.CxpiPid:
                                {
                                    /*数据装入图形显示*/
                                    CXPIStartBitDecodePacket StartBit = new CXPIStartBitDecodePacket(CalcPosition((long)bytefieldinfo.StartBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.StartBit.EndIndex - bytefieldinfo.StartBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.StartBit.Value },
                                        _BitCount = 1,
                                    };
                                    CXPIPidDecodePacket Pid = new CXPIPidDecodePacket(CalcPosition((long)bytefieldinfo.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.EndIndex - bytefieldinfo.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.Value },
                                        _BitCount = 7,
                                    };

                                    CXPIParityBitDecodePacket Parity = new CXPIParityBitDecodePacket(CalcPosition((long)bytefieldinfo.ParityBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.ParityBit.EndIndex - bytefieldinfo.ParityBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.ParityBit.Value },
                                        _BitCount = 1,
                                        ParityBitError = bytefieldinfo.StatusType == CXPIStatusType.CxpiParityError,
                                    };

                                    CXPIStopBitDecodePacket StopBit = new CXPIStopBitDecodePacket(CalcPosition((long)bytefieldinfo.StopBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.StopBit.EndIndex - bytefieldinfo.StopBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.StopBit.Value },
                                        _BitCount = 1,
                                    };
                                    decodepackets.Add(StartBit);
                                    decodepackets.Add(Pid);
                                    decodepackets.Add(Parity);
                                    decodepackets.Add(StopBit);

                                    /*数据装入事件列表*/
                                    eventinfo.EventInofs[2] = (Pid.Data, 8);
                                    eventinfo.EventInofs[8] = (Parity.Data, 1);
                                    if (bytefieldinfo.StatusType == CXPIStatusType.CxpiParityError)
                                    {
                                        string errinfo = "Pid Parity Error,";
                                        _ErrorMessage += errinfo;
                                    }
                                    endindex = (Int32)bytefieldinfo.StopBit.EndIndex;
                                    break;
                                }
                            case CXPIFieldType.CxpiFiShort:
                                {
                                    /*数据装入图形显示*/
                                    CXPIStartBitDecodePacket StartBit = new CXPIStartBitDecodePacket(CalcPosition((long)bytefieldinfo.StartBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.StartBit.EndIndex - bytefieldinfo.StartBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.StartBit.Value },
                                        _BitCount = 1,
                                    };
                                    CXPIFICounterDecodePacket Ficounter = new CXPIFICounterDecodePacket(CalcPosition((long)bytefieldinfo.FiByte.CounterBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.FiByte.CounterBit.EndIndex - bytefieldinfo.FiByte.CounterBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.FiByte.CounterBit.Value },
                                        _BitCount = 2,
                                    };
                                    CXPIFIWakeupDecodePacket Fiwakeup = new CXPIFIWakeupDecodePacket(CalcPosition((long)bytefieldinfo.FiByte.WakeUpBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.FiByte.WakeUpBit.EndIndex - bytefieldinfo.FiByte.WakeUpBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.FiByte.WakeUpBit.Value },
                                        _BitCount = 1,
                                    };
                                    CXPIFISleepDecodePacket Fisleep = new CXPIFISleepDecodePacket(CalcPosition((long)bytefieldinfo.FiByte.SleepBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.FiByte.SleepBit.EndIndex - bytefieldinfo.FiByte.SleepBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.FiByte.SleepBit.Value },
                                        _BitCount = 1,
                                    };
                                    CXPIFISleepDecodePacket FiDlcext = new CXPIFISleepDecodePacket(CalcPosition((long)bytefieldinfo.FiByte.DlcBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.FiByte.DlcBit.EndIndex - bytefieldinfo.FiByte.DlcBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.FiByte.DlcBit.Value },
                                        _BitCount = 4,
                                    };
                                    CXPIStopBitDecodePacket StopBit = new CXPIStopBitDecodePacket(CalcPosition((long)bytefieldinfo.StopBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.StopBit.EndIndex - bytefieldinfo.StopBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.StopBit.Value },
                                        _BitCount = 1,
                                    };
                                    
                                    decodepackets.Add(StartBit);
                                    decodepackets.Add(Ficounter);
                                    decodepackets.Add(Fiwakeup);
                                    decodepackets.Add(Fisleep);
                                    decodepackets.Add(FiDlcext);
                                    decodepackets.Add(StopBit);

                                    /*CRC显示位数*/
                                    _CrcNum = 8;

                                    /*数据装入事件列表*/
                                    string eventmessage = "Normal Polling";
                                    eventinfo.EventInofs[0] = (Encoding.Default.GetBytes(eventmessage), 0);

                                    eventinfo.EventInofs[3] = (Ficounter.Data, 2);//counter
                                    eventinfo.EventInofs[4] = (Fisleep.Data, 2);//sleep
                                    eventinfo.EventInofs[5] = (Fiwakeup.Data, 2);//wakeup
                                    eventinfo.EventInofs[6] = (FiDlcext.Data, 4);//dlc
                                    endindex = (Int32)bytefieldinfo.StopBit.EndIndex;
                                    break;
                                }
                            case CXPIFieldType.CxpiFiLong:
                                {
                                    /*数据装入图形显示*/
                                    CXPIStartBitDecodePacket StartBit = new CXPIStartBitDecodePacket(CalcPosition((long)bytefieldinfo.StartBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.StartBit.EndIndex - bytefieldinfo.StartBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.StartBit.Value },
                                        _BitCount = 1,
                                    };
                                    CXPIFICounterDecodePacket Ficounter = new CXPIFICounterDecodePacket(CalcPosition((long)bytefieldinfo.FiByte.CounterBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.FiByte.CounterBit.EndIndex - bytefieldinfo.FiByte.CounterBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.FiByte.CounterBit.Value },
                                        _BitCount = 2,
                                    };
                                    CXPIFIWakeupDecodePacket Fiwakeup = new CXPIFIWakeupDecodePacket(CalcPosition((long)bytefieldinfo.FiByte.WakeUpBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.FiByte.WakeUpBit.EndIndex - bytefieldinfo.FiByte.WakeUpBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.FiByte.WakeUpBit.Value },
                                        _BitCount = 1,
                                    };
                                    CXPIFISleepDecodePacket Fisleep = new CXPIFISleepDecodePacket(CalcPosition((long)bytefieldinfo.FiByte.SleepBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.FiByte.SleepBit.EndIndex - bytefieldinfo.FiByte.SleepBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.FiByte.SleepBit.Value },
                                        _BitCount = 1,
                                    };
                                    CXPIDlcDecodePacket FiDlcext = new CXPIDlcDecodePacket(CalcPosition((long)bytefieldinfo.FiByte.DlcBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.FiByte.DlcBit.EndIndex - bytefieldinfo.FiByte.DlcBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.FiByte.DlcBit.Value },
                                        _BitCount = 4,
                                    };
                                    CXPIStopBitDecodePacket StopBit = new CXPIStopBitDecodePacket(CalcPosition((long)bytefieldinfo.StopBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.StopBit.EndIndex - bytefieldinfo.StopBit.StartIndex), Source, chindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.StopBit.Value },
                                        _BitCount = 1,
                                    };

                                    decodepackets.Add(StartBit);
                                    decodepackets.Add(Ficounter);
                                    decodepackets.Add(Fiwakeup);
                                    decodepackets.Add(Fisleep);
                                    decodepackets.Add(FiDlcext);
                                    decodepackets.Add(StopBit);

                                    /*CRC显示位数*/
                                    _CrcNum = 16;

                                    /*数据装入事件列表*/
                                    string eventmessage = " Long Normal Polling";
                                    eventinfo.EventInofs[0] = (Encoding.Default.GetBytes(eventmessage), 0);

                                    eventinfo.EventInofs[3] = (Ficounter.Data, 2);//counter
                                    eventinfo.EventInofs[4] = (Fisleep.Data, 2);//sleep
                                    eventinfo.EventInofs[5] = (Fiwakeup.Data, 2);//wakeup
                                    eventinfo.EventInofs[6] = (FiDlcext.Data, 4);//dlc
                                    endindex = (Int32)bytefieldinfo.StopBit.EndIndex;
                                    break;
                                }
                            case CXPIFieldType.CxpiFiDlcExt:
                                {
                                    /*数据装入图形显示*/
                                    CXPIStartBitDecodePacket StartBit = new CXPIStartBitDecodePacket(CalcPosition((long)bytefieldinfo.StartBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.StartBit.EndIndex - bytefieldinfo.StartBit.StartIndex), Source, chindex))
                                    {
                                        /*显示数据*/
                                        Data = new Byte[] { (Byte)bytefieldinfo.StartBit.Value },
                                        /*显示位数*/
                                        _BitCount = 1,
                                        /*错误显示*/
                                    };
                                    CXPIDlcDecodePacket Dlc = new CXPIDlcDecodePacket(CalcPosition((long)bytefieldinfo.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.EndIndex - bytefieldinfo.StartIndex), Source, chindex))
                                    {
                                        /*显示数据*/
                                        Data = new Byte[] { (Byte)bytefieldinfo.Value },
                                        /*显示位数*/
                                        _BitCount = 8,
                                        /*错误显示*/
                                    };

                                    CXPIStopBitDecodePacket StopBit = new CXPIStopBitDecodePacket(CalcPosition((long)bytefieldinfo.StopBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.StopBit.EndIndex - bytefieldinfo.StopBit.StartIndex), Source, chindex))
                                    {
                                        /*显示数据*/
                                        Data = new Byte[] { (Byte)bytefieldinfo.StopBit.Value },
                                        /*显示位数*/
                                        _BitCount = 1,
                                        /*错误显示*/
                                    }; 
                                    decodepackets.Add(StartBit);
                                    decodepackets.Add(Dlc);
                                    decodepackets.Add(StopBit);

                                    /*数据装入事件列表*/
                                    eventinfo.EventInofs[7] = (Dlc.Data, 8);
                                    endindex = (Int32)bytefieldinfo.StopBit.EndIndex;
                                    break;
                                }
                            case CXPIFieldType.CxpiCrc:
                                {
                                    /*数据装入图形显示*/
                                    CXPIStartBitDecodePacket StartBit = new CXPIStartBitDecodePacket(CalcPosition((long)bytefieldinfo.StartBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.StartBit.EndIndex - bytefieldinfo.StartBit.StartIndex), Source, chindex))
                                    {
                                        /*显示数据*/
                                        Data = new Byte[] { (Byte)bytefieldinfo.StartBit.Value },
                                        /*显示位数*/
                                        _BitCount = 1,
                                        /*错误显示*/

                                    };
                                    CXPICrcDecodePacket Crc = new CXPICrcDecodePacket(CalcPosition((long)bytefieldinfo.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.EndIndex - bytefieldinfo.StartIndex), Source, chindex))
                                    {
                                        /*显示数据*/
                                        Data = new Byte[] { (Byte)bytefieldinfo.Value },
                                        /*显示位数*/
                                        _BitCount = _CrcNum,
                                        /*错误显示*/
                                        CrcError = bytefieldinfo.StatusType == CXPIStatusType.CxpiCrcError,
                                    };

                                    CXPIStopBitDecodePacket StopBit = new CXPIStopBitDecodePacket(CalcPosition((long)bytefieldinfo.StopBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.StopBit.EndIndex - bytefieldinfo.StopBit.StartIndex), Source, chindex))
                                    {
                                        /*显示数据*/
                                        Data = new Byte[] { (Byte)bytefieldinfo.StopBit.Value },
                                        /*显示位数*/
                                        _BitCount = 1,
                                        /*错误显示*/
                                    };
                                    decodepackets.Add(Crc);
                                    decodepackets.Add(StartBit);
                                    decodepackets.Add(StopBit);

                                    /*数据装入事件列表*/
                                    eventinfo.EventInofs[11] = (Crc.Data, 8);
                                    /*错误显示*/
                                    if (bytefieldinfo.StatusType == CXPIStatusType.CxpiByteError)
                                    {
                                        string errorinfo = "Crc error";
                                        _ErrorMessage += errorinfo;
                                    }
                                    endindex = (Int32)bytefieldinfo.EndIndex;

                                    break;
                                }
                            case CXPIFieldType.CxpiData:
                                {
                                    /*数据装入图形显示 */
                                    CXPIStartBitDecodePacket StartBit = new CXPIStartBitDecodePacket(CalcPosition((long)bytefieldinfo.StartBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.StartBit.EndIndex - bytefieldinfo.StartBit.StartIndex), Source, chindex))
                                    {
                                        /*显示数据*/
                                        Data = new Byte[] { (Byte)bytefieldinfo.StartBit.Value },
                                        /*显示位数*/
                                        _BitCount = 1,
                                        /*错误显示*/
                                    };
                                    CXPIDataDecodePacket Data = new CXPIDataDecodePacket(CalcPosition((long)bytefieldinfo.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.EndIndex - bytefieldinfo.StartIndex), Source, chindex))
                                    {
                                        /*显示数据*/
                                        Data = new Byte[] { (Byte)bytefieldinfo.Value },
                                        /*显示位数*/
                                        _BitCount = 8,
                                        /*错误显示*/

                                    };
                                    CXPIStopBitDecodePacket StopBit = new CXPIStopBitDecodePacket(CalcPosition((long)bytefieldinfo.StopBit.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.StopBit.EndIndex - bytefieldinfo.StopBit.StartIndex), Source, chindex))
                                    {
                                        /*显示数据*/
                                        Data = new Byte[] { (Byte)bytefieldinfo.StopBit.Value },
                                        /*显示位数*/
                                        _BitCount = 1,
                                        /*错误显示*/
                                    };
                                    decodepackets.Add(StartBit);
                                    decodepackets.Add(Data);
                                    decodepackets.Add(StopBit);

                                    /*数据装入事件列表*/
                                    datalist.AddRange(Data.Data);

                                    endindex = (Int32)bytefieldinfo.StopBit.EndIndex;
                                    break;
                                }
                            case CXPIFieldType.CxpiIbs:
                                {
                                    CXPIIbsDecodePacket Ibs = new CXPIIbsDecodePacket(CalcPosition((long)bytefieldinfo.StartIndex, Source, chindex),
                                    CalcBitLenght((int)(bytefieldinfo.EndIndex - bytefieldinfo.StartIndex), Source, chindex))
                                    {
                                        /*显示数据*/
                                        Data = new Byte[] { (Byte)bytefieldinfo.Value },
                                        /*显示位数*/
                                        _BitCount = 1,
                                        /*错误显示*/
                                    };
                                    decodepackets.Add(Ibs);
                                    break;
                                }
                        }
                        if (_ErrorMessage != string.Empty)
                        {
                            eventinfo.EventInofs[12] = (Encoding.Default.GetBytes(_ErrorMessage), 0);
                        }
                    }
                    byte[] finalarray = datalist.ToArray();
                    eventinfo.EventInofs[9] = (finalarray, (UInt32)(8 * finalarray.Length));
                    eventinfo.EndPosition = CalcPosition(endindex, Source, chindex);
                    eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, chindex);
                    _EventInfos.Add(eventinfo);
                }

                if (!DecoderImpl.FreeCXPI(ref results))
                    return;

                _ResultData.DecodeViewInfos = decodepackets.ToArray();
                decoderesults.Add(_ResultData);

            }
        }

        //输入参数赋值
        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolCXPIOptions()
            {
                Source = Source,
                Threshold = _Threshold,
                BaudRate = Baudrate,
            };

        }
    }
}


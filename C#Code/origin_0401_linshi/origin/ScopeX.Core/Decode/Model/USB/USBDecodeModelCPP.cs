using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NPOI.HSSF.Record.CF;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;
using ScopeX.ComModel;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Hardware.Driver;
using static NPOI.HSSF.Util.HSSFColor;
using static ScopeX.Core.Decode.EdgeInfoCPP;
using static ScopeX.Core.Decode.ManchesterDecodeModel;

namespace ScopeX.Core.Decode;

// // ******************************************************************
// //           @File          USBDecodeCPP.cs
// //           @Brief        
// //           @Author        zxl
// //           @Creation      2024-4-1
// //           @Modified      
// // ******************************************************************

internal partial class USBDecodeModelCPP :ProtocolModel
{
    private DecodeResultData _ResultData = new DecodeResultData();
    private List<PAM2EdgePulse> _PAM2EdgePulsesList1 = new List<PAM2EdgePulse>();
    private List<PAM2EdgePulse> _PAM2EdgePulsesList2 = new List<PAM2EdgePulse>();
    private List<PAM3EdgePulse> _PAM3EdgePulsesList = new List<PAM3EdgePulse>();
    public USBDecodeModelCPP(ChannelId id, Boolean isTrigDecode = false) : base(id, SerialProtocolType.USB, isTrigDecode)
    {
        _ResultData.Name = "USB";
        //InitSignalRateMap();
    }

    // 创建一个新的 Dictionary，指定键的类型为 string，值的类型为 int
     private static Dictionary<UsbPacketType, string> MyPidStringMap = new Dictionary<UsbPacketType, string>()
    {
         // 添加pid键值对
         // Token
         { UsbPacketType.SOF, "SOF"},
         { UsbPacketType.SETUP, "SETUP"},
         { UsbPacketType.IN, "IN"},
         { UsbPacketType.OUT, "OUT"},
         { UsbPacketType.PING, "PING"},
         { UsbPacketType.SPLIT, "SPLIT"},
         // Data
         { UsbPacketType.DATA0, "DATA0"},
         { UsbPacketType.DATA1, "DATA1"},
         { UsbPacketType.DATA2, "DATA2"},
         { UsbPacketType.M_DATA, "M_DATA"},
         //HandShack
         { UsbPacketType.ACK, "ACK"},
         { UsbPacketType.NAK, "NAK"},
         { UsbPacketType.STALL, "STALL"},
         { UsbPacketType.NYET, "NYET"},
         { UsbPacketType.ERR, "ERR"},
         //Special
         { UsbPacketType.RESERVED, "RESERVED"},
         //{ UsbPacketType.NO_DEFINE, "NO DEFINE"},
    };

    // 添加键值对

    public override IReadOnlyList<String> EventInfoTitles { get; } = (new List<String>()
        {
            "Index",
            "Start Time",
            "Usb PID",
            "Address",
            "EndPoint",
             "FrameNumber",
            "Data",
            "CRC",
            "ErrorType"
            //"Error",
            //"Stop Time"
        }).AsReadOnly();
    // 事件字段index
    private const Int32 EVENT_FIELD_PID= 0;
    private const Int32 EVENT_FIELD_ADDRESS = 1;
    private const Int32 EVENT_FIELD_ENDPOINT = 2;
    private const Int32 EVENT_FIELD_FRAME = 3;
    private const Int32 EVENT_FIELD_DATA = 4;
    private const Int32 EVENT_FIELD_CRC = 5;
    private const Int32 EVENT_FIELD_ERROR_TYPE = 6;

    /// <summary>
    /// 通道D+
    /// </summary>
    public String Source1Unit => GetChannelUnit(Source1);

    private ChannelId _Source1 = ChannelId.C1;

    public ChannelId Source1
    {
        get { return _Source1; }
        set { UpdateProperty(ref _Source1, value); }
    }

    /// <summary>
    /// 通道D-
    /// </summary>
    public String Source2Unit => GetChannelUnit(Source2);
    private ChannelId _Source2= ChannelId.C2;
    public ChannelId Source2
    {
        get { return _Source2; }
        set { UpdateProperty(ref _Source2, value); }
    }

    /// <summary>
    /// 数据源1的高阈值(HS)
    /// </summary>
    private Single _ThresholdH = 0;
    public Single ThresholdH
    {
        get { return (float)(_ThresholdH * TryGetChannelGain(Source1)); }
        set { UpdateProperty(ref _ThresholdH, (float)(value / TryGetChannelGain(Source1))); }
    }
    public Single MinThresholdH => -MaxThresholdH;
    public Single MaxThresholdH => (float)(8 * TryGetChannelGain(Source1));

    /// <summary>
    /// 数据源1的低阈值(HS)
    /// </summary>
    private Single _ThresholdL = 0;
    public Single ThresholdL
    {
        get { return (float)(_ThresholdL * TryGetChannelGain(Source1)); }
        set { UpdateProperty(ref _ThresholdL, (float)(value / TryGetChannelGain(Source1))); }
    }
    public Single MinThresholdL => -MaxThresholdL;
    public Single MaxThresholdL => (float)(8 * TryGetChannelGain(Source1));

    /// <summary>
    /// 数据源2的阈值 (FS\LS)
    /// </summary>
    private Single _Threshold2 = 0;
    public Single Threshold2
    {
        get { return (float)(_Threshold2 * TryGetChannelGain(Source2)); }
        set { UpdateProperty(ref _Threshold2, (float)(value / TryGetChannelGain(Source2))); }
    }
    public Single MinThreshold2 => -MaxThreshold2;
    public Single MaxThreshold2 => (float)(8 * TryGetChannelGain(Source2));


    /// <summary>
    /// USB速率
    /// </summary>
    private ProtocolUSB.USBSpeed _BaudMode = ProtocolUSB.USBSpeed.HIGH_SPEED;

    public ProtocolUSB.USBSpeed BaudMode
    {
        get { return _BaudMode; }
        set { UpdateProperty(ref _BaudMode, value); }
    }

    /// <summary>
    /// USB输入类型
    /// </summary>
    private ProtocolUSB.USBInputType _InputType= ProtocolUSB.USBInputType.DIFF_INPUT;

    public ProtocolUSB.USBInputType InputType
    {
        get { return _InputType; }
        set { UpdateProperty(ref _InputType, value); }
    }

    /// <summary>
    /// 检查时间戳是否更新
    /// </summary>
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

    /// <summary>
    /// 检查通道是否有数据
    /// </summary>
    internal override Boolean SourceHasData()
    {
        if (DsoPrsnt.DefaultDsoPrsnt == null)
            return false;

        DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Source1, out IChnlPrsnt? prsnt);
        if (prsnt == null)
            return false;

        Double[] thresholds;
        thresholds = new Double[2];
        thresholds[0] = _ThresholdH;
        thresholds[1] = _ThresholdL;

        if (Source1.IsReference() && prsnt.Active && prsnt.VuDatabase != null && prsnt.VuDatabase.Current != null)
        {
            return DecodeDataHelper.ReferenceHasData(Source1, thresholds);
        }

        if (Source1.IsAnalog())
        {
            return DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
        }

        return false;
    }
    private void ClearBuffer()
    {
        List<DecodeResultData> decodebuffer = GetDecodeBuffer();
        decodebuffer.Clear();
        _ResultData.DecodeViewInfos = new IDecodeViewInfo[0];
    }

    //小端数据转大端数据
    public Byte[] ConvertLittleEndianToBigEndian(Byte[] littleEndianData, UInt32 ByteCount)
    {
        // 创建一个与输入数组相同长度的数组，用于存储大端字节顺序的结果
        Byte[] bigEndianData = new Byte[littleEndianData.Length];

        // 反转字节数组
        for (Int32 i = littleEndianData.Length - 1, j = 0; i >= 0; i--)
        {
            if (littleEndianData[i] == 0 && i >= ByteCount)
            { continue; }
            bigEndianData[j] = littleEndianData[i];
            j++;
        }

        return bigEndianData;
    }

    private Boolean GetDecodePacketDisplay(ref ProtocolEventInfo eventinfo, ref List<USBDecodePacket> decodepackets, ref UsbPacket usbevent, Int32 src1index, Int32 src2index)
    {
        //判断是否为数据包(是否携带数据)
        Boolean isdatapacket = false;
        UsbPacketType usbpidtype = usbevent.packet_type;
        if (usbpidtype == UsbPacketType.DATA0 || usbpidtype == UsbPacketType.DATA1 ||
            usbpidtype == UsbPacketType.DATA2 || usbpidtype == UsbPacketType.M_DATA)
        { 
            isdatapacket = true;
        }
        Int32 bytefieldsize = Marshal.SizeOf(typeof(UsbField));
        Int32 databytesize = Marshal.SizeOf(typeof(UsbData));
        //区分USB包内字段域类型
        //DATA
        List<Byte> error_tmp = new List<Byte>();

        for (Int32 j = 0; j < usbevent.field_size; j++)
        {
            /*从Event中提取ByteField,判断类型再装入eventinfo用于显示*/
            UsbField bytefieldinfo = (UsbField)Marshal.PtrToStructure(usbevent.field + j * bytefieldsize, typeof(UsbField));

            switch (bytefieldinfo.field_type)
            {
                case ProtocolUSB.USBFiledType.SYNC:
                    {
                        // SYNC 
                        USBDecodePacket sync_packet = new USBDecodePacket(CalcPosition((Int64)bytefieldinfo.start_index, _Source1, src1index),
                            CalcBitLenght((Int32)(bytefieldinfo.end_index - bytefieldinfo.start_index), _Source1, src1index),
                            USBDecodePacketTypeNew.SYNC)
                        {
                            _BitCount = 32,
                            _Title = "SYNC",
                            ShowStr = "SYNC",
                            Data = new Byte[1],
                        };
                        eventinfo.StartTimeByPs = base.GetTimeFromPosition(sync_packet.Start, src1index);
                        eventinfo.StartPosition = sync_packet.Start;
                        decodepackets.Add(sync_packet);
                        //if (bytefieldinfo.field_error == USBPacketError.SYNC_ERROR)
                        //{
                        //    sync_packet.Data[0] = Encoding.Default.GetBytes("Error")[0];
                        //    error_tmp.Add(Encoding.Default.GetBytes("SYNC_Err")[0]);
                        //}
                    }
                    break;
                case ProtocolUSB.USBFiledType.PID:
                    {
                        //PID
                        USBDecodePacket pid_packet = new USBDecodePacket(CalcPosition((Int64)bytefieldinfo.start_index, _Source1, src1index),
                        CalcBitLenght((Int32)(bytefieldinfo.end_index - bytefieldinfo.start_index), _Source1, src1index),
                                                                                USBDecodePacketTypeNew.PID)
                        {
                            _BitCount = 8,
                            _Title = "PID",
                            ShowStr = "PID",
                            Data = Encoding.Default.GetBytes(MyPidStringMap[usbpidtype]),
                        };
                        
                        decodepackets.Add(pid_packet);
                        eventinfo.EventInofs[EVENT_FIELD_PID] = (Encoding.Default.GetBytes(MyPidStringMap[usbpidtype]), 0);
                        //if (bytefieldinfo.field_error == USBPacketError.PID_ERROR)
                        //{
                        //    pid_packet.Data[0] = Encoding.Default.GetBytes("Error")[0];
                        //    error_tmp.Add(Encoding.Default.GetBytes("PID_Err")[0]);
                        //}
                    }
                    break;
                case ProtocolUSB.USBFiledType.ADDR:
                    {
                        //ADDR
                        USBDecodePacket addr_packet = new USBDecodePacket(CalcPosition((Int64)bytefieldinfo.start_index, _Source1, src1index),
                        CalcBitLenght((Int32)(bytefieldinfo.end_index - bytefieldinfo.start_index), _Source1, src1index),
                                                                                USBDecodePacketTypeNew.ADDR)
                        {
                            _BitCount = 7,
                            _Title = "ADDR",
                            Data = new Byte[1],
                        };
                        UInt32 value = bytefieldinfo.bit_value;
                        addr_packet.Data[0] = BitConverter.GetBytes(value)[0];
                        decodepackets.Add(addr_packet);
                        eventinfo.EventInofs[EVENT_FIELD_ADDRESS] = (BitConverter.GetBytes(value),7);
                    }
                    break;
                case ProtocolUSB.USBFiledType.ENDP:
                    {
                        //ENDP
                        USBDecodePacket endp_packet = new USBDecodePacket(CalcPosition((Int64)bytefieldinfo.start_index, _Source1, src1index),
                        CalcBitLenght((Int32)(bytefieldinfo.end_index - bytefieldinfo.start_index), _Source1, src1index),
                                                                                USBDecodePacketTypeNew.ENDP)
                        {
                            _BitCount = 4,
                            _Title = "ENDP",
                            Data = new Byte[1],
                        };
                        UInt32 value = bytefieldinfo.bit_value;
                        endp_packet.Data[0] = BitConverter.GetBytes(value)[0];
                        decodepackets.Add(endp_packet);
                        eventinfo.EventInofs[EVENT_FIELD_ENDPOINT] = (BitConverter.GetBytes(value), 4);
                    }
                    break;
                case ProtocolUSB.USBFiledType.FRAME:
                    {
                        //Frame Number
                        USBDecodePacket frame_packet = new USBDecodePacket(CalcPosition((Int64)bytefieldinfo.start_index, _Source1, src1index),
                        CalcBitLenght((Int32)(bytefieldinfo.end_index - bytefieldinfo.start_index), _Source1, src1index),
                                                                                USBDecodePacketTypeNew.FRAMENUB)
                        {
                            _BitCount = 11,
                            _Title = "Frame Number",
                            Data = new Byte[2],
                        };
                        UInt32 value = bytefieldinfo.bit_value;
                        //frame_packet.Data[0] = BitConverter.GetBytes(value)[0];
                        for (Int32 cnt = 0; cnt < 2; cnt++)
                        {
                            frame_packet.Data[cnt] = ConvertLittleEndianToBigEndian(BitConverter.GetBytes(value), 2)[cnt];
                        }
                        decodepackets.Add(frame_packet);
                        eventinfo.EventInofs[EVENT_FIELD_FRAME] = (frame_packet.Data, 11);
                    }
                    break;
                case ProtocolUSB.USBFiledType.DATA:
                    {
                        //DATA
                        List<Byte> data_tmp = new List<Byte>();
                        //循环添加数据包标签
                        for (Int32 k = 0; k < usbevent.data_size; k++)
                        {
                            UsbData data_byte_struct = (UsbData)Marshal.PtrToStructure(usbevent.data_byte + k * databytesize, typeof(UsbData));
                            data_tmp.Add(data_byte_struct.data_value);

                            USBDecodePacket data_packet = new USBDecodePacket(CalcPosition((Int64)data_byte_struct.start_index, _Source1, src1index),
                            CalcBitLenght((Int32)(data_byte_struct.end_index - data_byte_struct.start_index), _Source1, src1index),
                                                        USBDecodePacketTypeNew.DATA)
                            {
                                _BitCount = 8,
                                _Title = "Data",
                                Data = new Byte[1],
                            };
                            UInt32 value = data_byte_struct.data_value;
                            data_packet.Data[0] = BitConverter.GetBytes(value)[0];
                            
                            decodepackets.Add(data_packet);
                        }
                        //添加事件表数据
                        eventinfo.EventInofs[EVENT_FIELD_DATA] = (data_tmp.ToArray<Byte>(), (UInt32)data_tmp.Count() * 8);
                    }
                    break;
                case ProtocolUSB.USBFiledType.CRC:
                    {
                        //CRC
                        UInt32 crcbitcount = (UInt32)(isdatapacket ? 16 : 5);
                        UInt32 crcbytecount = (UInt32)Math.Round((Double)crcbitcount / 8, 0);
                        USBDecodePacket crc_packet = new USBDecodePacket(CalcPosition((Int64)bytefieldinfo.start_index, _Source1, src1index),
                        CalcBitLenght((Int32)(bytefieldinfo.end_index - bytefieldinfo.start_index), _Source1, src1index),
                                                                                USBDecodePacketTypeNew.CRC)
                        {
                            _BitCount = crcbitcount,
                            _Title = "CRC",
                            Data = isdatapacket?new Byte[2]:new Byte[1],
                        };

                        UInt32 value = bytefieldinfo.bit_value;
                        
                        for (Int32 cnt = 0; cnt < crcbytecount; cnt++)
                        {
                            crc_packet.Data[cnt] = ConvertLittleEndianToBigEndian(BitConverter.GetBytes(value), crcbytecount)[cnt];
                        }
                        decodepackets.Add(crc_packet);
                        eventinfo.EventInofs[EVENT_FIELD_CRC] = (crc_packet.Data, crcbitcount);
                        //if (bytefieldinfo.field_error == USBPacketError.CRC_ERROR)
                        //{
                        //    crc_packet.Data[0] = Encoding.Default.GetBytes("Error")[0];
                        //    error_tmp.Add(Encoding.Default.GetBytes("CRC_Err")[0]);
                        //}
                    }
                    break;
                case ProtocolUSB.USBFiledType.EOP:
                    {
                        //EOP
                        USBDecodePacket eop_packet = new USBDecodePacket(CalcPosition((Int64)bytefieldinfo.start_index, _Source1, src1index),
                        CalcBitLenght((Int32)(bytefieldinfo.end_index - bytefieldinfo.start_index), _Source1, src1index),
                                                                                USBDecodePacketTypeNew.EOP)
                        {
                            _BitCount = 8,
                            _Title = "EOP",
                            ShowStr = "EOP",
                            Data = new Byte[1],
                        };
                        decodepackets.Add(eop_packet);
                        eventinfo.EndPosition = eop_packet.Start + eop_packet.Lenght;
                        eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, src1index);
                        //if (bytefieldinfo.field_error == USBPacketError.EOP_ERROR)
                        //{
                        //    eop_packet.Data[0] = Encoding.Default.GetBytes("Error")[0];
                        //    error_tmp.Add(Encoding.Default.GetBytes("EOP_Err")[0]);
                        //}
                    }
                    break;
                default:
                    break;
            }
        }
        //添加错误事件表
        eventinfo.EventInofs[EVENT_FIELD_ERROR_TYPE] = (error_tmp.ToArray<Byte>(), (UInt32)error_tmp.Count() * 8);
        return true;
    }


    internal override void ParsingData(ref CancellationToken token)
    {
        //判断输入类型 差分则单线,单端则双线
        ProtocolUSB.USBInputType inputtype = InputType;
        if (inputtype == ProtocolUSB.USBInputType.DIFF_INPUT)
        {
            _Source2 = _Source1;
        }

        Int32 src1index = GetChIndex(_Source1);
        Int32 src2index = GetChIndex(_Source2);

        UInt32 src1len = 0;
        UInt32 src2len = 0;

        Double samplerate = 0.0;

        DecodeDataHelper.Instance.TryGetSampleRate(BusId, _Source1, ref samplerate);
        DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, _Source1, ref src1len);
        DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, _Source2, ref src2len);

        if (src1index == -1 || src1len == 0 || src2index == -1 || src2len == 0)
        {
            _NeedDecodeData = false;    
            _NeedUpdateViewInfo = true;
            _ResultData.DecodeViewInfos = new IDecodeViewInfo[0];
            _EventInfos.Clear();
        }

        if (!_NeedDecodeData && !_NeedUpdateViewInfo)
            return;

        _NeedDecodeData = false;
        _NeedUpdateViewInfo = true;

        Boolean needclear = false;

        IntPtr dataptr1 = IntPtr.Zero;
        GCHandle datahandlediff1;       
        IntPtr dataptr2 = IntPtr.Zero;
        GCHandle datahandlediff2;

        //获取边沿
        if (inputtype == ProtocolUSB.USBInputType.DIFF_INPUT)
        {
            //差分信号输入
            ThreeLevelEdgeInfo? datanodediff = DecodeDataHelper.Instance.GetThreeLevelEdgeInfo(BusId, startindex: 0, _Source1, ref token, ref needclear) as ThreeLevelEdgeInfo;
            if (datanodediff == null)
            {
                return;
            }
            _PAM3EdgePulsesList.Clear();
            DecodeDataHelper.Instance.GetThreeLevelPulses(ref datanodediff, ref _PAM3EdgePulsesList);
            PAM3EdgePulseSequence.Allocate(ref _PAM3EdgePulsesList, (UInt64)src1len, samplerate, out dataptr1, out datahandlediff1);
            dataptr2 = dataptr1;
            datahandlediff2 = datahandlediff1;
        }
        else
        {
            //单端信号输入
            TwoLevelEdgeInfo? datanode1 = DecodeDataHelper.Instance.GetEdgeInfo(BusId, startindex: 0, _Source1, ref token, ref needclear) as TwoLevelEdgeInfo;
            TwoLevelEdgeInfo? datanode2 = DecodeDataHelper.Instance.GetEdgeInfo(BusId, startindex: 0, _Source2, ref token, ref needclear) as TwoLevelEdgeInfo;
            if (datanode1 == null || datanode2 == null)
            {
                return;
            }
            _PAM2EdgePulsesList1.Clear();
            _PAM2EdgePulsesList2.Clear();
            DecodeDataHelper.Instance.GetTwoLevelPulses(ref datanode1, ref _PAM2EdgePulsesList1);
            PAM2EdgePulseSequence.Allocate(ref _PAM2EdgePulsesList1, (UInt64)src1len, samplerate, out dataptr1, out datahandlediff1);
            DecodeDataHelper.Instance.GetTwoLevelPulses(ref datanode2, ref _PAM2EdgePulsesList2);
            PAM2EdgePulseSequence.Allocate(ref _PAM2EdgePulsesList2, (UInt64)src2len, samplerate, out dataptr2, out datahandlediff2);
        }

        UsbOptions options = new UsbOptions()
        {
            input_type = this._InputType,
            usb_speed = this._BaudMode,
        };


        List<USBDecodePacket> decodepackets = new List<USBDecodePacket>();

        UsbResult results = new UsbResult()
        {
            EventCount = 0,
            Event = IntPtr.Zero,
        };

        //开始解码
        Boolean decodeflag = DecoderImpl.DecodeUsb_New(ref options, dataptr1, dataptr2, out results);
        if (!decodeflag)
        {

            Debug.WriteLine(@"解码失败");
            //c++资源释放
            //DecoderImpl.FreeUsb_New(ref results);
        }
        //释放资源
        PAM2EdgePulseSequence.Free(ref dataptr1, ref datahandlediff1);
        //解码结果获取 
        List<DecodeResultData> decoderesults = GetDecodeBuffer();

        if (_NeedUpdateViewInfo)
        {
            _NeedUpdateViewInfo = false;
            _EventInfos.Clear();
            decoderesults.Clear();
            _ResultData = new DecodeResultData();
            ChangeBuffer();
            ClearBuffer();
        }

        Int32 eventsize = Marshal.SizeOf(typeof(UsbPacket));

        // 默认事件显示
        String temp_info = "--";
        //无结果则退出
        if (results.Event == IntPtr.Zero || results.EventCount == 0)
        {
            return;
        }
        // USB包个数
        for (Int32 i = 0; i < results.EventCount; i++)
        {
            ProtocolEventInfo eventinfo = new ProtocolEventInfo();
            eventinfo.Index = _EventInfos.Count;
            eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));

            UsbPacket usbevent = (UsbPacket)Marshal.PtrToStructure(results.Event + i * eventsize, typeof(UsbPacket));

            if (usbevent.field == IntPtr.Zero )
            { 
                continue;
            }

            eventinfo.EventInofs[EVENT_FIELD_PID] = (Encoding.Default.GetBytes(temp_info), 0);   //PID
            eventinfo.EventInofs[EVENT_FIELD_ADDRESS] = (Encoding.Default.GetBytes(temp_info), 0);   // ADDR
            eventinfo.EventInofs[EVENT_FIELD_ENDPOINT] = (Encoding.Default.GetBytes(temp_info), 0);   // ENDP
            eventinfo.EventInofs[EVENT_FIELD_FRAME] = (Encoding.Default.GetBytes(temp_info), 0);   // FN
            eventinfo.EventInofs[EVENT_FIELD_DATA] = (Encoding.Default.GetBytes(temp_info), 0);   // DATA
            eventinfo.EventInofs[EVENT_FIELD_CRC] = (Encoding.Default.GetBytes(temp_info), 0);   //CRC
            eventinfo.EventInofs[EVENT_FIELD_ERROR_TYPE] = (Encoding.Default.GetBytes(temp_info), 0);   //ERRORTYPE

            if (!GetDecodePacketDisplay(ref eventinfo, ref decodepackets, ref usbevent,src1index,src2index))
            {
                Debug.WriteLine(@"获取解码结果失败");
                return;
            };
            //添加事件表
            _EventInfos.Add(eventinfo);
        }

        // 解码结果释放
        DecoderImpl.FreeUsb_New(ref results);

        _ResultData.DecodeViewInfos = decodepackets.ToArray();
        decoderesults.Add(_ResultData);

    }

    public override HdMessage.IDecoderOptions? GetProtocolRecoder()
    {
        return new HdMessage.ProtocolUSBOptions()
        {
            
            Source1 = Source1,
            Source2 = Source2,
            //Source1Threshold = ThresholdH,
            //Source2Threshold = ThresholdL,
            Source1ThresholdH = _ThresholdH,
            Source1ThresholdL = _ThresholdL,
            Source2Threshold = _Threshold2,
            BaudMode = BaudMode,
            InputType = InputType,
        };
    }

}


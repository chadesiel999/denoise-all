using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using System.Runtime.InteropServices;
using static ScopeX.ComModel.ProtocolPSI5;
using static ScopeX.Core.Decode.EdgeInfoCPP;
using ScopeX.Hardware.Driver;
using NPOI.OpenXmlFormats.Dml.Chart;
using NPOI.POIFS.Crypt.Dsig;

namespace ScopeX.Core.Decode
{
    sealed internal class PSI5DecodeModel : ProtocolModel
    {
        private DecodeResultData _ResultData = new DecodeResultData();
        private List<PAM2EdgePulse> _EdgePulsesList = new List<PAM2EdgePulse>();
        private Dictionary<String, Int32> _EventDict = new Dictionary<String, Int32>();
        // internal protected List<ProtocolEventInfo> _EventInfos = new List<ProtocolEventInfo>();
        

        public PSI5DecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id, SerialProtocolType.PSI5, isTrigDecode)
        {
            _ResultData.Name = "PSI5";
        }
        //public override Double BitRateByPs => 1f /(BaudMode == ProtocolPSI5.Psi5BaudMode.BAUD_Standard ? 1.25 * 1E5 : 1.89 * 1E5) * 1E12 ;

        public override IReadOnlyList<String> EventInfoTitles { get; } = (new List<String>()
        {
            "Index",
            "Start Time",
            "DataA",
            "DataB",
            "Initialiazation Data",
            "Status/Error Message",
            "Frame Control",
            "Status",
            "CRC",
            //"Error",
            //"Stop Time"
        }).AsReadOnly();

        // 事件字段index
        private const Int32 EVENT_FIELD_DATA_A = 0;
        private const Int32 EVENT_FIELD_DATA_B = 1;
        private const Int32 EVENT_FIELD_INITIALIAZATION_DATA = 2;
        private const Int32 EVENT_FIELD_STATUS_ERROR = 3;
        private const Int32 EVENT_FIELD_FRAME_CONTROL = 4;
        private const Int32 EVENT_FIELD_STATUS = 5;
        private const Int32 EVENT_FIELD_CRC = 6;
        //private const Int32 EVENT_FIELD_ERROR = 7;
        //private const Int32 EVENT_FIELD_STOP_TIME = 8;

        /// <summary>
        /// 通道
        /// </summary>
        private ChannelId _Source = ChannelId.C1;

        public ChannelId Source1
        {
            get { return _Source; }
            set { UpdateProperty(ref _Source, value); }
        }
        /// <summary>
        /// 阈值最大最小值、默认值
        /// </summary>
        public Single MinThreshold => -MaxThreshold;
        public Single MaxThreshold => (float)(8 * TryGetChannelGain(_Source));

        /// <summary>
        /// 数据源的阈值
        /// </summary>
        private Single _Threshold = 0;
        public Single Threshold1
        {
            get { return (float)(_Threshold * TryGetChannelGain(Source1)); }
            set { UpdateProperty(ref _Threshold, (float)(value / TryGetChannelGain(Source1))); }
        }
        public String Unit => GetChannelUnit(Source1);
        /// <summary>
        /// 波特率模式
        /// </summary>
        private ProtocolPSI5.Psi5BaudMode _BaudMode = ProtocolPSI5.Psi5BaudMode.Standard;

        public ProtocolPSI5.Psi5BaudMode BaudMode
        {
            get { return _BaudMode; }
            set { UpdateProperty(ref _BaudMode, value); }
        }
        /// <summary>
        /// 帧控制大小
        /// </summary>
        private ProtocolPSI5.Psi5FrameControl _FrameControl = ProtocolPSI5.Psi5FrameControl.Frame_0_Bit;

        public ProtocolPSI5.Psi5FrameControl FrameControl
        {
            get { return _FrameControl; }
            set { UpdateProperty(ref _FrameControl, value); }
        }
        /// <summary>
        /// 串行数据
        /// </summary>
        private ProtocolPSI5.Psi5SerialMessage _SerialMessage = ProtocolPSI5.Psi5SerialMessage.OFF;

        public ProtocolPSI5.Psi5SerialMessage SerialMessage
        {
            get { return _SerialMessage; }
            set { UpdateProperty(ref _SerialMessage, value); }
        }
        /// <summary>
        /// 状态位
        /// </summary>
        private ProtocolPSI5.Psi5Status _Status = ProtocolPSI5.Psi5Status.Status_0_Bit;

        public ProtocolPSI5.Psi5Status Status
        {
            get { return _Status; }
            set { UpdateProperty(ref _Status, value); }
        }
        /// <summary>
        /// 校验位
        /// </summary>
        private ProtocolPSI5.Psi5CheckType _CheckType = ProtocolPSI5.Psi5CheckType.Check_Crc;

        public ProtocolPSI5.Psi5CheckType CheckType
        {
            get { return _CheckType; }
            set { UpdateProperty(ref _CheckType, value); }
        }

        /// <summary>
        /// 数据域A范围
        /// </summary>
        /// 
        public String _BitUnit = "bits";
        public String BitUnit
        {
            get => _BitUnit;
            set { _BitUnit = value; }  
        }

        public UInt32 MinDataABitsSize => 10;
        public UInt32 MaxDataABitsSize => 24;

        private UInt32 _DataABitsSize = 13;
        public UInt32 DataABitsSize
        {
            get => _DataABitsSize;
            set => UpdateProperty(ref _DataABitsSize, value);
        }
        /// <summary>
        /// 数据域B范围
        /// </summary>
        public UInt32 MinDataBBitsSize => 0;
        public UInt32 MaxDataBBitsSize => 10;

        private UInt32 _DataBBitsSize = 9;
        public UInt32 DataBBitsSize
        {
            get => _DataBBitsSize;
            set => UpdateProperty(ref _DataBBitsSize, value);
        }
        /// <summary>
        /// Payload范围
        /// </summary>
        public UInt32 MinPayloadSize => 10;
        public UInt32 MaxPayloadSize => 28;

        UInt32 message_size => (UInt32)(SerialMessage == ProtocolPSI5.Psi5SerialMessage.OFF ? 0 : 2);
        private UInt32 _PayloadSize => (UInt32)(message_size + (UInt32)FrameControl + (UInt32)Status + DataBBitsSize + DataABitsSize);
        public UInt32 PayloadSize
        {
            get => _PayloadSize;

        }
        //小端数据转大端数据
        public Byte[] ConvertLittleEndianToBigEndian(Byte[] littleEndianData , UInt32 ByteCount)
        {
            // 创建一个与输入数组相同长度的数组，用于存储大端字节顺序的结果
            Byte[] bigEndianData = new Byte[littleEndianData.Length];

            // 反转字节数组
            for (Int32 i = littleEndianData.Length - 1 ,j=0; i >= 0; i--)
            {
                if (littleEndianData[i] == 0 && i >= ByteCount)
                { continue; }
                bigEndianData[j] = littleEndianData[i];
                j++;
            }

            return bigEndianData;
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
            if (_Source.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[_Source - ChannelIdExt.MinRChId].Channels != null
                && DecodeDataSource.Instance.ReferenceDataSource[_Source - ChannelIdExt.MinRChId].Channels[0] == _Source)
            {
                DecodeDataSource.Instance.ReferenceDataSource[_Source - ChannelIdExt.MinRChId].HasData = false;
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

            if (Source1.IsReference() && prsnt.VuDatabase.Current != null)
            {
                return DecodeDataHelper.ReferenceHasData(Source1, _Threshold);
            }

            if (Source1.IsAnalog())
            {
                return DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }

            return false;
        }


        /// <summary>
        /// 解析数据
        /// </summary>
        internal override void ParsingData(ref CancellationToken token)
        {
            Int32 srcindex = GetChIndex(_Source);

            UInt32 srclen = 0;

            Double srcsamplerate = 0;

            DecodeDataHelper.Instance.TryGetSampleRate(BusId, _Source, ref srcsamplerate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, _Source, ref srclen);

            if (MoreThanStorage() || srcindex == -1 || srclen == 0)
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


            // 输入项
            PSI5Options options = new PSI5Options()
            {
                baud_rate = this._BaudMode,
                serial_Message = this._SerialMessage,
                frame_control_size = this._FrameControl,
                state_size = this._Status,
                data_a_size = this._DataABitsSize,
                data_b_size = this._DataBBitsSize,
                check_type = this._CheckType
            };

            IntPtr dataptr = IntPtr.Zero;
            GCHandle datahandle;
            //获取边沿
            TwoLevelEdgeInfo? datanode = DecodeDataHelper.Instance.GetEdgeInfo(BusId, startindex: 0, _Source, ref token, ref needclear) as TwoLevelEdgeInfo;
            if (datanode == null)
            {
                return;
            }

            _EdgePulsesList.Clear();
            DecodeDataHelper.Instance.GetTwoLevelPulses(ref datanode, ref _EdgePulsesList);
            PAM2EdgePulseSequence.Allocate(ref _EdgePulsesList, (UInt64)srclen, srcsamplerate, out dataptr, out datahandle);

            List<PSI5DecodePacket> decodepackets = new List<PSI5DecodePacket>();

            PSI5Result results;
            results.EventCount = 0;
            results.Event = IntPtr.Zero;
            //unsafe
            //{
            //    Int32 size = sizeof(PSI5Options);
            //    Int32 size2 = sizeof(PSI5Event);
            //    Int32 size3 = sizeof(Psi5FieldInfo);
            //    Int32 size = sizeof(PSI5Result);
            //}
            // 开始解码
            if (!DecoderImpl.DecodePSI5(ref options, dataptr, out results))
            {

            }
            PAM2EdgePulseSequence.Free(ref dataptr, ref datahandle);

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
            Int32 eventsize = Marshal.SizeOf(typeof(PSI5Event));

            // 默认事件显示
            String temp_info = "--";

            // event count
            for (Int32 i = 0; i < results.EventCount; i++)
            {
                ProtocolEventInfo eventinfo = new ProtocolEventInfo();

                eventinfo.Index = _EventInfos.Count;
                eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));

                PSI5Event psi5event = (PSI5Event)Marshal.PtrToStructure(results.Event + i * eventsize, typeof(PSI5Event));

               
                /*
                        private const Int32 EVENT_FIELD_DATA_A = 0;
                        private const Int32 EVENT_FIELD_DATA_B = 1;
                        private const Int32 EVENT_FIELD_INITIALIAZATION_DATA = 2;
                        private const Int32 EVENT_FIELD_STATUS_ERROR = 3;
                        private const Int32 EVENT_FIELD_FRAME_CONTROL = 4;
                        private const Int32 EVENT_FIELD_STATUS = 5;
                        private const Int32 EVENT_FIELD_CRC = 6;
                        private const Int32 EVENT_FIELD_ERROR = 7;
                        private const Int32 EVENT_FIELD_STOP_TIME = 8;
                 */
                //eventinfo.EventInofs[0]  = (Encoding.Default.GetBytes(temp_info), 0);   // Index
                //eventinfo.EventInofs[1]  = (Encoding.Default.GetBytes(temp_info), 0);   // Start Time
                eventinfo.EventInofs[EVENT_FIELD_DATA_A] = (Encoding.Default.GetBytes(temp_info), 0);   // Master ID
                eventinfo.EventInofs[EVENT_FIELD_DATA_B] = (Encoding.Default.GetBytes(temp_info), 0);   // Master ID
                eventinfo.EventInofs[EVENT_FIELD_INITIALIAZATION_DATA] = (Encoding.Default.GetBytes(temp_info), 0);   // Primary Level
                eventinfo.EventInofs[EVENT_FIELD_STATUS_ERROR] = (Encoding.Default.GetBytes(temp_info), 0);   // Secondary Level
                eventinfo.EventInofs[EVENT_FIELD_FRAME_CONTROL] = (Encoding.Default.GetBytes(temp_info), 0);   // Slave Address
                eventinfo.EventInofs[EVENT_FIELD_STATUS] = (Encoding.Default.GetBytes(temp_info), 0);   // Command Type
                eventinfo.EventInofs[EVENT_FIELD_CRC] = (Encoding.Default.GetBytes(temp_info), 0);   // Command Address
                //eventinfo.EventInofs[EVENT_FIELD_ERROR] = (Encoding.Default.GetBytes(temp_info), 0);   // Register Address
                //eventinfo.EventInofs[EVENT_FIELD_STOP_TIME] = (Encoding.Default.GetBytes(temp_info), 0);   // Byte Count

                // start bit 
                PSI5DecodePacket startbit_packet = new PSI5DecodePacket(CalcPosition((Int32)psi5event.event_start_bit.start_index, _Source, srcindex),
                                                                        CalcBitLenght((Int32)(psi5event.event_start_bit.end_index - psi5event.event_start_bit.start_index), _Source, srcindex),
                                                                        PSI5FieldType.FIELD_START_BIT)
                {
                    _BitCount = 2,
                    _Title = "Start Bit",
                    Data = new Byte[1],
                };
                eventinfo.StartTimeByPs = base.GetTimeFromPosition(startbit_packet.Start, srcindex);
                eventinfo.StartPosition = startbit_packet.Start;
                UInt32 value = psi5event.event_start_bit.data_value;
                startbit_packet.Data[0] = BitConverter.GetBytes(value)[0];
                decodepackets.Add(startbit_packet);
                

                // serial message 
                PSI5DecodePacket message_packet = new PSI5DecodePacket(CalcPosition((Int32)psi5event.event_message.start_index, _Source, srcindex), 
                                                                       CalcBitLenght((Int32)(psi5event.event_message.end_index - psi5event.event_message.start_index),_Source,srcindex),
                                                                       PSI5FieldType.FIELD_SERIAL_MESSAGE)
                {
                    _BitCount = psi5event.event_message.data_bin_len,
                    _Title = "M",
                    Data = psi5event.event_message.data_byte_len != 0 ? new Byte[psi5event.event_message.data_byte_len] : new Byte[1],
                };
                value = psi5event.event_message.data_value;
                message_packet.Data[0] = BitConverter.GetBytes(value)[0];
                decodepackets.Add(message_packet);


                // Frame Control 
                PSI5DecodePacket framecontrol_packet = new PSI5DecodePacket(CalcPosition((Int32)psi5event.event_frame_control.start_index, _Source, srcindex),
                                                                       CalcBitLenght((Int32)(psi5event.event_frame_control.end_index - psi5event.event_frame_control.start_index), _Source, srcindex),
                                                                       PSI5FieldType.FIELD_FRAME_CONTROL)
                {
                    _BitCount = psi5event.event_frame_control.data_bin_len,
                    _Title = "FC",
                    Data = psi5event.event_frame_control.data_byte_len != 0 ? new Byte[psi5event.event_frame_control.data_byte_len] : new Byte[1],
                    
                };
                value = psi5event.event_frame_control.data_value;
                framecontrol_packet.Data[0] = BitConverter.GetBytes(value)[0];
                eventinfo.EventInofs[EVENT_FIELD_FRAME_CONTROL] = (BitConverter.GetBytes(value), psi5event.event_frame_control.data_byte_len * 8);
                decodepackets.Add(framecontrol_packet);
                
                // status
                PSI5DecodePacket status_packet = new PSI5DecodePacket(CalcPosition((Int32)psi5event.event_status.start_index, _Source, srcindex),
                                                                       CalcBitLenght((Int32)(psi5event.event_status.end_index - psi5event.event_status.start_index), _Source, srcindex),
                                                                       PSI5FieldType.FIELD_STATUS)
                {
                    _BitCount = psi5event.event_status.data_bin_len,
                    _Title = "S",
                    Data = psi5event.event_status.data_byte_len != 0 ? new Byte[psi5event.event_status.data_byte_len] : new Byte[1],
                };
                value = psi5event.event_status.data_value;
                status_packet.Data[0] = BitConverter.GetBytes(value)[0];
                
                eventinfo.EventInofs[EVENT_FIELD_STATUS] = (BitConverter.GetBytes(value), psi5event.event_status.data_byte_len * 8);
                decodepackets.Add(status_packet);

                // DATA B
                PSI5DecodePacket data_b_packet = new PSI5DecodePacket(CalcPosition((Int32)psi5event.event_data_b.start_index, _Source, srcindex),
                                                                       CalcBitLenght((Int32)(psi5event.event_data_b.end_index - psi5event.event_data_b.start_index), _Source, srcindex),
                                                                       PSI5FieldType.FIELD_DATA_B)
                {
                    _BitCount = psi5event.event_data_b.data_bin_len,
                    _Title = "Data Region B",
                    Data = psi5event.event_data_b.data_byte_len != 0 ? new Byte[psi5event.event_data_b.data_byte_len] : new Byte[1],
                };
                value = psi5event.event_data_b.data_value;

                for (Int32 cnt = 0; cnt < psi5event.event_data_b.data_byte_len; cnt++)
                {
                    data_b_packet.Data[cnt] = ConvertLittleEndianToBigEndian(BitConverter.GetBytes(value), psi5event.event_data_b.data_byte_len)[cnt];
                }
                eventinfo.EventInofs[EVENT_FIELD_DATA_B] = (ConvertLittleEndianToBigEndian(BitConverter.GetBytes(value), psi5event.event_data_b.data_byte_len), psi5event.event_data_b.data_byte_len * 8);
                decodepackets.Add(data_b_packet);

                //检查数据域A是否分包
                
                if ((Int32)psi5event.event_data_a_package_rest.data_bin_len == 0 && (Int32)psi5event.event_data_a_package_rest.data_byte_len == 0)
                {
                    // 数据域A不分包
                    // DATA A
                    PSI5DecodePacket data_a_packet = new PSI5DecodePacket(CalcPosition((Int32)psi5event.event_data_a.start_index, _Source, srcindex),
                                                                       CalcBitLenght((Int32)(psi5event.event_data_a.end_index - psi5event.event_data_a.start_index), _Source, srcindex),
                                                                       PSI5FieldType.FIELD_DATA_A)
                    {
                        _BitCount = psi5event.event_data_a.data_bin_len,
                        _Title = "Data Region A",
                        Data = psi5event.event_data_a.data_byte_len != 0 ? new Byte[psi5event.event_data_a.data_byte_len] : new Byte[1],
                    };
                    value = psi5event.event_data_a.data_value;
                    for (Int32 cnt = 0; cnt < psi5event.event_data_a.data_byte_len; cnt++)
                    {
                        data_a_packet.Data[cnt] = ConvertLittleEndianToBigEndian(BitConverter.GetBytes(value), psi5event.event_data_a.data_byte_len)[cnt];
                    }
                    eventinfo.EventInofs[EVENT_FIELD_DATA_A] = (ConvertLittleEndianToBigEndian(BitConverter.GetBytes(value), psi5event.event_data_a.data_byte_len), psi5event.event_data_a.data_byte_len * 8);
                    decodepackets.Add(data_a_packet);
                }
                else
                {
                    // 数据域A分包
                    // DATA_A_REST
                    PSI5DecodePacket data_a_rest_packet = new PSI5DecodePacket(CalcPosition((Int32)psi5event.event_data_a_package_rest.start_index, _Source, srcindex),
                                                                       CalcBitLenght((Int32)(psi5event.event_data_a_package_rest.end_index - psi5event.event_data_a_package_rest.start_index), _Source, srcindex),
                                                                       PSI5FieldType.FIELD_DATA_A_REST)
                    {
                        _BitCount = psi5event.event_data_a_package_rest.data_bin_len,
                        _Title = "Data Region A",
                        Data = psi5event.event_data_a_package_rest.data_byte_len != 0 ? new Byte[psi5event.event_data_a_package_rest.data_byte_len] : new Byte[1],
                    };
                    value = psi5event.event_data_a_package_rest.data_value;
                    for (Int32 cnt = 0; cnt < psi5event.event_data_a_package_rest.data_byte_len; cnt++)
                    {
                        data_a_rest_packet.Data[cnt] = ConvertLittleEndianToBigEndian(BitConverter.GetBytes(value), psi5event.event_data_a_package_rest.data_byte_len)[cnt];
                    }
                    eventinfo.EventInofs[EVENT_FIELD_DATA_A] = (ConvertLittleEndianToBigEndian(BitConverter.GetBytes(value), psi5event.event_data_a_package_rest.data_byte_len), psi5event.event_data_a_package_rest.data_byte_len * 8);
                    decodepackets.Add(data_a_rest_packet);
                    // 检查包类型
                    String Title_INIT_TMP = "";
                    if ((Int32)psi5event.event_data_status.data_bin_len != 0 && (Int32)psi5event.event_data_status.data_byte_len != 0)
                    {
                        //检查最高位10位是否为Block ID & Data for Initialization
                        UInt32 decimal_value = psi5event.event_data_status.data_value;
                        if (decimal_value < (Int32)DataARange.SENSOR_READY_BUT_UNLOCKED)
                        {
                            //Response
                            Title_INIT_TMP = "Response Code";
                        }
                        else 
                        {
                            // Sensor Status
                            Title_INIT_TMP = "Sensor Status";
                        }
                        // DATA_A_INIT

                        PSI5DecodePacket data_a_init_packet = new PSI5DecodePacket(CalcPosition((Int32)psi5event.event_data_status.start_index, _Source, srcindex),
                                               CalcBitLenght((Int32)(psi5event.event_data_status.end_index - psi5event.event_data_status.start_index), _Source, srcindex),
                                               PSI5FieldType.FIELD_DATA_A_INIT)
                        {
                            _BitCount = psi5event.event_data_status.data_bin_len,
                            _Title = Title_INIT_TMP,
                            Data = psi5event.event_data_status.data_byte_len != 0 ? new Byte[psi5event.event_data_status.data_byte_len] : new Byte[1],
                        };
                        value = psi5event.event_data_status.data_value;
                        for (Int32 cnt = 0; cnt < psi5event.event_data_status.data_byte_len; cnt++)
                        {
                            data_a_init_packet.Data[cnt] = ConvertLittleEndianToBigEndian(BitConverter.GetBytes(value), psi5event.event_data_status.data_byte_len)[cnt];
                        }
                        eventinfo.EventInofs[EVENT_FIELD_STATUS_ERROR] = (ConvertLittleEndianToBigEndian(BitConverter.GetBytes(value), psi5event.event_data_status.data_byte_len), psi5event.event_data_status.data_byte_len * 8);
                        decodepackets.Add(data_a_init_packet);
                    }
                    else 
                    {
                        //检查最高位10位是否为Status & Error Messages
                        UInt32 decimal_value = psi5event.event_init_data.data_value;
                        if (decimal_value > (Int32)DataARange.BLOCK_ID_16)
                        {
                            //Status Data
                            Title_INIT_TMP = "Status Data";
                        }
                        else
                        {
                            // Block ID
                            Title_INIT_TMP = "Block ID";
                        }
                        // DATA_A_INIT
                        PSI5DecodePacket data_a_init_packet = new PSI5DecodePacket(CalcPosition((Int32)psi5event.event_init_data.start_index, _Source, srcindex),
                                                                       CalcBitLenght((Int32)(psi5event.event_init_data.end_index - psi5event.event_init_data.start_index), _Source, srcindex),
                                                                       PSI5FieldType.FIELD_DATA_A_INIT)
                        {
                            _BitCount = psi5event.event_init_data.data_bin_len,
                            _Title = Title_INIT_TMP,
                            Data = psi5event.event_init_data.data_byte_len != 0 ? new Byte[psi5event.event_init_data.data_byte_len] : new Byte[1],
                        };

                        value = psi5event.event_init_data.data_value;
                        for (Int32 cnt = 0; cnt < psi5event.event_init_data.data_byte_len; cnt++)
                        {
                            data_a_init_packet.Data[cnt] = ConvertLittleEndianToBigEndian(BitConverter.GetBytes(value), psi5event.event_init_data.data_byte_len)[cnt];
                        }
                        eventinfo.EventInofs[EVENT_FIELD_INITIALIAZATION_DATA] = (ConvertLittleEndianToBigEndian(BitConverter.GetBytes(value), psi5event.event_init_data.data_byte_len), psi5event.event_init_data.data_byte_len * 8);
                        decodepackets.Add(data_a_init_packet);
                    }
                }
                String Title_VERIFY_TMP = psi5event.check_type == Psi5CheckType.Check_Crc ? "CRC" : "Parity";
                // VERIFY_CRC
                PSI5DecodePacket verify_packet = new PSI5DecodePacket(CalcPosition((Int32)psi5event.event_check_num.start_index, _Source, srcindex),
                                                                       CalcBitLenght((Int32)(psi5event.event_check_num.end_index - psi5event.event_check_num.start_index), _Source, srcindex),
                                                                       PSI5FieldType.FIELD_VERIFY_CRC)
                {
                    _BitCount = psi5event.event_check_num.data_bin_len,
                    _Title = Title_VERIFY_TMP,//"CRC",
                    Data = psi5event.event_check_num.data_byte_len != 0 ? new Byte[psi5event.event_check_num.data_byte_len] : new Byte[1],
                };
                value = psi5event.event_check_num.data_value;
                for (Int32 cnt = 0; cnt < psi5event.event_check_num.data_byte_len; cnt++)
                {
                    verify_packet.Data[cnt] = ConvertLittleEndianToBigEndian(BitConverter.GetBytes(value), psi5event.event_check_num.data_byte_len)[cnt];
                }
                eventinfo.EventInofs[EVENT_FIELD_CRC] = (ConvertLittleEndianToBigEndian(BitConverter.GetBytes(value), psi5event.event_check_num.data_byte_len), psi5event.event_check_num.data_byte_len * 8);
                decodepackets.Add(verify_packet);
                eventinfo.EndPosition = verify_packet.Start + verify_packet.Lenght;
                eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, srcindex);
                // 事件信息
                _EventInfos.Add(eventinfo);

            }

            // 解码结果释放
            DecoderImpl.FreePSI5(ref results);

            _ResultData.DecodeViewInfos = decodepackets.ToArray();
            decoderesults.Add(_ResultData);

        }

        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolPSI5Options()
            {
                Source = Source1,
                Threshold = _Threshold,
                Psi5BaudMode = BaudMode,
                Psi5FrameControl = FrameControl,
                Psi5Status = Status,
                DataASize = _DataABitsSize,
                DataBSize = _DataBBitsSize,
            };
        }

    }
}

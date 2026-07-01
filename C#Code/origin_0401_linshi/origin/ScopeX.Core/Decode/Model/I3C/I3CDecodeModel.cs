using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using static ScopeX.Core.Decode.EdgeInfoCPP;
using ScopeX.Core.Decode;
using System.Collections;
using System.Reflection;
using NPOI.OpenXmlFormats.Dml.Chart;

namespace ScopeX.Core.Decode
{
    internal sealed class I3CDecodeModel : ProtocolModel
    {
        private DecodeResultData _ResultData = new DecodeResultData();
        private List<PAM2EdgePulse> _ClkPulsesList = new List<PAM2EdgePulse>();
        private List<PAM2EdgePulse> _DataPulsesList = new List<PAM2EdgePulse>();

        public I3CDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id, SerialProtocolType.I3C, isTrigDecode)
        {
        }


        public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>()
        {
            "Index",
            "Start Time",
            "Command",
            "Address",
            "Broadcast Address",
            "Dynamic Address",
            "Test Mode",
            "DCR",
            "BCR",
            "ENEC Byte",
            "Command Word",
            "Sub Command Identifier",
            "Response",
            "Data",
            "Preamble",
            "Parity",
            "CRC2",
            "Error",
            "Stop Time",
        };

        // 事件字段index
        private const Int32 _EVENT_FIELD_COMMAND = 0;
        private const Int32 _EVENT_FIELD_ADDRESS = 1;
        private const Int32 _EVENT_FIELD_BROADCAST_ADDRESS = 2;
        private const Int32 _EVENT_FIELD_DYNAMIC_ADDRESS = 3;
        private const Int32 _EVENT_FIELD_TEST_MODE = 4;
        private const Int32 _EVENT_FIELD_DCR = 5;
        private const Int32 _EVENT_FIELD_BCR = 6;
        private const Int32 _EVENT_FIELD_ENEC_BYTE = 7;
        private const Int32 _EVENT_FIELD_COMMAND_WORD = 8;
        private const Int32 _EVENT_FIELD_SUB_COMMAND_IDENTIFIER = 9;
        private const Int32 _EVENT_FIELD_RESPONSE = 10;
        private const Int32 _EVENT_FIELD_DATA = 11;
        private const Int32 _EVENT_FIELD_PREAMBLE = 12;
        private const Int32 _EVENT_FIELD_PARITY = 13;
        private const Int32 _EVENT_FIELD_CRC2 = 14;
        private const Int32 _EVENT_FIELD_ERROR = 15;
        private const Int32 _EVENT_FIELD_STOPTIME = 16;

        // 触发的代码
        private ProtocolSPMI.CheckType _CheckType;
        public ProtocolSPMI.CheckType CheckType
        {
            get { return _CheckType; }
            set { UpdateProperty(ref _CheckType, value); }
        }

        private ChannelId _SData = ChannelId.C1;

        public ChannelId SData
        {
            get { return _SData; }
            set { UpdateProperty(ref _SData, value); }
        }
        private ChannelId _SCLK = ChannelId.C2;

        public ChannelId SCLK
        {
            get { return _SCLK; }
            set { UpdateProperty(ref _SCLK, value); }
        }

        private Single _DataThreshold = 1;

        public Single DataThreshold
        {
            get { return (Single)(_DataThreshold * TryGetChannelGain(SData)); }
            set { UpdateProperty(ref _DataThreshold, (Single)(value / TryGetChannelGain(SData))); }
        }
        public String DataUnit => GetChannelUnit(SData);
        public Single MaxThreshold => 20;
        public Single MinThreshold => -MaxThreshold;

        private Single _CLKThreshold = 1;

        public Single CLKThreshold
        {
            get { return (Single)(_CLKThreshold * TryGetChannelGain(SCLK)); }
            set { UpdateProperty(ref _CLKThreshold, (Single)(value / TryGetChannelGain(SCLK))); }
        }
        public String CLKUnit => GetChannelUnit(SCLK);

        internal override Boolean CheckUpdate(ref Int64 laststamp)
        {
            if (_SCLK.IsAnalog() && laststamp != DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp;
                return true;
            }
            if (_SCLK.IsReference() && laststamp != DecodeDataHelper.Instance.ReferenceDataSource[_SCLK - ChannelIdExt.MinRChId].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.ReferenceDataSource[_SCLK - ChannelIdExt.MinRChId].TimeStamp;
                return true;
            }
            return false;
        }

        public override void UpdateReferenceDataStatus()
        {
            if (_SCLK.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[_SCLK - ChannelIdExt.MinRChId].Channels != null
                && DecodeDataSource.Instance.ReferenceDataSource[_SCLK - ChannelIdExt.MinRChId].Channels[0] == _SCLK)
            {
                DecodeDataSource.Instance.ReferenceDataSource[_SCLK - ChannelIdExt.MinRChId].HasData = false;
            }
            if (_SData.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[_SData - ChannelIdExt.MinRChId].Channels != null
                && DecodeDataSource.Instance.ReferenceDataSource[_SData - ChannelIdExt.MinRChId].Channels[0] == _SData)
            {
                DecodeDataSource.Instance.ReferenceDataSource[_SData - ChannelIdExt.MinRChId].HasData = false;
            }
        }

        internal override Boolean SourceHasData()
        {
            if (DsoPrsnt.DefaultDsoPrsnt == null)
                return false;

            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(_SCLK, out IChnlPrsnt? scl_prsnt);
            if (scl_prsnt == null)
                return false;

            Boolean clk = false, data = false;

            if (_SCLK.IsReference() && scl_prsnt.VuDatabase.Current != null)
            {
                clk = DecodeDataHelper.ReferenceHasData(_SCLK, _CLKThreshold);
            }

            if (_SCLK.IsAnalog())
            {
                clk = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }


            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(_SData, out IChnlPrsnt? prsnt);
            if (prsnt == null)
                return false;

            if (_SData.IsReference() && prsnt.VuDatabase.Current != null)
            {
                data = DecodeDataHelper.ReferenceHasData(_SData, _DataThreshold);
            }

            if (_SData.IsAnalog())
            {
                data = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }
            return (data || clk);
        }

        public I3CFieldType ToI3CFieldType(SequenceCondition sequence_condition, ref string str)
        {
            switch (sequence_condition)
            {
                case SequenceCondition.SEQUENCE_CONDITION_START:
                    {
                        str = "Start";
                        return I3CFieldType.FIELD_TYPE_START;
                    }
                case SequenceCondition.SEQUENCE_CONDITION_RESTART:
                    {
                        str = "ReStart";
                        return I3CFieldType.FIELD_TYPE_RESTART;
                    }
                case SequenceCondition.SEQUENCE_CONDITION_END:
                    {
                        str = "End";
                        return I3CFieldType.FIELD_TYPE_END;
                    }
                default:
                    {
                        str = "";
                        return I3CFieldType.FIELD_TYPE_END;
                    }
            }
        }

        // 
        public void SDRFieldToI3CFieldType(SDRFieldType sdr_type, ref I3CFieldType i3c_type, ref string field_str)
        {
            switch (sdr_type)
            {
                case SDRFieldType.COMMAND:
                    {
                        i3c_type = I3CFieldType.FIELD_TYPE_COMMAND;
                        break;
                    }
                case SDRFieldType.TARGET_ADDRESS:
                case SDRFieldType.RESERVER_ADDRESS:
                    {
                        i3c_type = I3CFieldType.FIELD_TYPE_ADDR;
                        break;
                    }
                case SDRFieldType.DATA:
                    {
                        i3c_type = I3CFieldType.FIELD_TYPE_DATA;
                        break;
                    }
                default:
                    {
                        i3c_type = I3CFieldType.FIELD_TYPE_DATA;
                        break;
                    }

            }

            // field str
            field_str = Enum.GetName(typeof(SDRFieldType), sdr_type); //推荐 
        }

        // 添加错误处理
        public void HandleI3cError(I3CError error,  ref I3CFieldType i3c_type, ref string str)
        {
            switch (error.error_type)
            {
                case I3CErrorType.I3C_ERROR_NONE:
                    {
                        break;
                    }
                case I3CErrorType.I3C_UNFRAME:
                    {
                        str += "Unknown Frame; ";
                        i3c_type = I3CFieldType.FIELD_TYPE_ERROR;
                        break;
                    }
                case I3CErrorType.I3C_ERROR_RW:
                    {
                        str += "Acked Error:" + error.error_value.ToString() + "; ";

                        i3c_type = I3CFieldType.FIELD_TYPE_ERROR;
                        break;
                    }
                case I3CErrorType.I3C_ERROR_PARITY:
                    {
                        str += "Parity Error: Calculate value:" + error.error_value.ToString() + "; ";

                        i3c_type = I3CFieldType.FIELD_TYPE_ERROR;
                        break;
                    }
                case I3CErrorType.I3C_UNKNOWN_FIELD:
                    {
                        str += "Unknown Field; ";

                        i3c_type = I3CFieldType.FIELD_TYPE_ERROR;
                        break;
                    }
            }
        }
        //public void ExpandFieldToI3CFieldType(SDRExpandType sdr_type, ref I3CFieldType i3c_type, ref string field_str)
        //{
        //    switch (sdr_type)
        //    {
        //        case SDRExpandType.SDR_ET_ACK:
        //            {
        //                i3c_type = I3CFieldType.FIELD_TYPE_EXPAND;
        //                break;
        //            }
        //        case SDRExpandType.SDR_ET_RW:
        //            {
        //                break;
        //            }
        //        case SDRExpandType.SDR_ET_PARITY:
        //            {
        //                break;
        //            }
        //        default:
        //            {
        //                i3c_type = I3CFieldType.FIELD_TYPE_DATA;
        //                break;
        //            }
        //    }
        //}

        internal override void ParsingData(ref CancellationToken token)
        {
            Int32 clkindex = GetChIndex(_SCLK);
            Int32 dataindex = GetChIndex(_SData);

            UInt32 clklen = 0, datalen = 0;

            Double clsamplerate = 0, datasamplerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, _SCLK, ref clsamplerate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, _SCLK, ref clklen);

            DecodeDataHelper.Instance.TryGetSampleRate(BusId, _SData, ref datasamplerate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, _SData, ref datalen);

            if (MoreThanStorage() || clkindex == -1 || dataindex == -1 || clklen == 0 || datalen == 0)
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
            I3COption option = new I3COption();

            IntPtr clkptr = IntPtr.Zero, dataptr = IntPtr.Zero;
            GCHandle clkhandle, datahandle;

            //获取边沿
            TwoLevelEdgeInfo? clknode = DecodeDataHelper.Instance.GetEdgeInfo(BusId, startindex: 0, _SCLK, ref token, ref needclear) as TwoLevelEdgeInfo;
            if (clknode == null)
            {
                return;
            }
            _ClkPulsesList.Clear();
            DecodeDataHelper.Instance.GetTwoLevelPulses(ref clknode, ref _ClkPulsesList);
            PAM2EdgePulseSequence.Allocate(ref _ClkPulsesList, (UInt64)clklen, clsamplerate, out clkptr, out clkhandle);

            //获取边沿
            TwoLevelEdgeInfo? datanode = DecodeDataHelper.Instance.GetEdgeInfo(BusId, startindex: 0, _SData, ref token, ref needclear) as TwoLevelEdgeInfo;
            if (datanode == null)
            {


                return;
            }
            _DataPulsesList.Clear();

            DecodeDataHelper.Instance.GetTwoLevelPulses(ref datanode, ref _DataPulsesList);
            PAM2EdgePulseSequence.Allocate(ref _DataPulsesList, (UInt64)datalen, datasamplerate, out dataptr, out datahandle);


            List<I3CDecodePacket> decodepackets = new List<I3CDecodePacket>();

            I3CResult results;
            results.EventCount = 0;
            results.Event = IntPtr.Zero;

            // 开始解码
           DecoderImpl.DecodeI3C(ref option, clkptr, dataptr, out results);
   
            PAM2EdgePulseSequence.Free(ref clkptr, ref clkhandle);
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

            // to do
            Int32 packetsize = Marshal.SizeOf(typeof(I3CPacket));
            Int32 sdrpacketsize = Marshal.SizeOf(typeof(SDRPacket));


            Int32 sdrfieldsize = Marshal.SizeOf(typeof(SDRField));
            Int32 sdrexpandfieldsize = Marshal.SizeOf(typeof(SDRExpandField));

            Int32 errorsize = Marshal.SizeOf(typeof(I3CError));

            // 默认事件显示
            String temp_info = "--";

            // event count
            for (Int32 i = 0; i < results.EventCount; i++)
            {
                // 响应数据
                List<Byte> reponsedata = new List<Byte>();
                List<Byte> parity = new List<Byte>();
                string dynamicaddress = "";
                string dcr = "";
                string bcr = "";
                string error = "";

                ProtocolEventInfo eventinfo = new ProtocolEventInfo();

                eventinfo.Index = _EventInfos.Count;
                eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));

                // 得到I3C 包
                I3CPacket packet = (I3CPacket)Marshal.PtrToStructure(results.Event + i * packetsize, typeof(I3CPacket));

                // 事件字段index
                eventinfo.EventInofs[_EVENT_FIELD_COMMAND] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_ADDRESS] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_BROADCAST_ADDRESS] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_DYNAMIC_ADDRESS] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_TEST_MODE] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_DCR] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_BCR] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_ENEC_BYTE] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_COMMAND_WORD] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_SUB_COMMAND_IDENTIFIER] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_RESPONSE] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_DATA] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_PREAMBLE] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_PARITY] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_CRC2] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_ERROR] = (Encoding.Default.GetBytes(temp_info), 0);
                eventinfo.EventInofs[_EVENT_FIELD_STOPTIME] = (Encoding.Default.GetBytes(temp_info), 0);

                // srd模式解析
                if (packet.packet_type == I3CPacketType.I3C_PT_SDR)
                {
                    SDRPacket sdrpacket = (SDRPacket)Marshal.PtrToStructure(packet.i3c_packet, typeof(SDRPacket));

                    eventinfo.StartTimeByPs = base.GetTimeFromPosition(sdrpacket.start_index, clkindex);
                    eventinfo.StartPosition = CalcPosition((Int32)sdrpacket.start_index, _SCLK, clkindex);
                    eventinfo.EndPosition  = CalcPosition((Int32)sdrpacket.end_index, _SCLK, clkindex);
                    eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, clkindex);

                    string startstr = "",endstr = "";

                    // 开始类型
                    I3CDecodePacket startpacket = new I3CDecodePacket(CalcPosition((Int32)sdrpacket.start_index, _SCLK, clkindex), 1,
                                                      ToI3CFieldType(sdrpacket.start_type, ref startstr))
                    {
                        _BitCount = 0,
                        _Title = startstr,
                    };

                    if (sdrpacket.end_type != SequenceCondition.SEQUENCE_CONDITION_NONE)
                    {
                        // 开始类型
                        I3CDecodePacket endpacket = new I3CDecodePacket(CalcPosition((Int32)sdrpacket.end_index, _SCLK, clkindex), 1,
                                                          ToI3CFieldType(sdrpacket.end_type, ref endstr))
                        {

                            _BitCount = 0,
                            _Title = endstr,
                        };
                    }

                    decodepackets.Add(startpacket);

                    // 遍历字节域
                    for (Int32 field_size = 0; field_size < sdrpacket.sdr_field_size; ++field_size)
                    {
                        SDRField sdrfield = (SDRField)Marshal.PtrToStructure(sdrpacket.sdr_field + field_size * sdrfieldsize, typeof(SDRField));

                        I3CFieldType i3c_type = I3CFieldType.FIELD_TYPE_DATA;

                        string fieldname = "", eventname = "";

                        SDRFieldToI3CFieldType(sdrfield.field_type, ref i3c_type, ref fieldname);

                        if (sdrfield.field_type == SDRFieldType.COMMAND)
                        {
                            string ? val = Enum.GetName(typeof(CCCType), sdrfield.field_value);
                            if (null!= val)
                                fieldname = val;
                        }

                        HandleI3cError(sdrfield.error, ref i3c_type, ref error);
                        
                        // 开始类型
                        I3CDecodePacket field_packet = new I3CDecodePacket(CalcPosition((Int32)sdrfield.start_index, _SCLK, clkindex), CalcBitLenght((Int32)(sdrfield.end_index - sdrfield.start_index), _SCLK, clkindex),
                                                                            i3c_type)
                        {
                            _BitCount = 8,
                            _Title = fieldname,
                            Data = new Byte[1],
                        };
                        field_packet.Data[0] = sdrfield.field_value;
                        decodepackets.Add(field_packet);

                        SDRExpandField expandfield;

                        string expandname = "";

             

                        for (Int32 expand_size = 0; expand_size < sdrfield.expand_size; ++expand_size)
                        {
                            I3CFieldType expand_type = I3CFieldType.FIELD_TYPE_EXPAND;

                            // sdr 扩展字段
                            expandfield = (SDRExpandField)Marshal.PtrToStructure(sdrfield.expand_field + expand_size * sdrexpandfieldsize, typeof(SDRExpandField));

                            HandleI3cError(expandfield.error, ref expand_type, ref error);

                            // ACK为0不绘制
                            if (expandfield.field_type == SDRExpandType.SDR_ET_ACK)
                            {
                                if (expand_type == I3CFieldType.FIELD_TYPE_EXPAND)
                                    continue;
                            }
                            else if (expandfield.field_type == SDRExpandType.SDR_ET_RW)
                            {
                                expandname = expandfield.field_value == 0 ? "Write" : "Read";
                            }
                            else if (expandfield.field_type == SDRExpandType.SDR_ET_PARITY)
                            {
                                expandname = "T";
                                parity.Add(expandfield.field_value);
                            }
                            else
                            {
                                // 默认值，不处理
                            }


                            I3CDecodePacket expand_packet = new I3CDecodePacket(CalcPosition((Int32)expandfield.start_index, _SCLK, clkindex),
                                                                                CalcBitLenght((Int32)(expandfield.end_index - expandfield.start_index), _SCLK, clkindex),
                                                                                expand_type)
                            {
                                _BitCount = 2,
                                _Title = expandname,
                                Data = new byte[1] { expandfield.field_value },
                            };

                            decodepackets.Add(expand_packet);
                        }


                        // 事件信息
                        switch (sdrfield.field_type)
                        {
                            case SDRFieldType.COMMAND:
                                {
                                    
                                    eventname = Enum.GetName(typeof(CCCType), sdrfield.field_value);

                                    if (eventname == null)
                                        eventname = "";

                                    eventinfo.EventInofs[_EVENT_FIELD_COMMAND] = (Encoding.UTF8.GetBytes(eventname), 0); // 0表示string
                                    break;
                                }
                            case SDRFieldType.RESERVER_ADDRESS:
                                {
                                    Byte[] addr = new Byte[1];
                                    addr[0] = sdrfield.field_value;
                                    eventinfo.EventInofs[_EVENT_FIELD_BROADCAST_ADDRESS] = (addr, 8);

                                    break;
                                }
                            case SDRFieldType.TARGET_ADDRESS:
                                {
                                    eventname += sdrfield.field_value.ToString() + ":" + expandname;
                                    eventinfo.EventInofs[_EVENT_FIELD_ADDRESS] = (Encoding.UTF8.GetBytes(eventname), 0); // 0表示string

                                    break;
                                }
                            case SDRFieldType.DATA:
                                {
                                    reponsedata.Add(sdrfield.field_value);
                                    break;
                                }
                            case SDRFieldType.DYNAMIC_ADDR_MASTER:
                                {
                                    dynamicaddress += sdrfield.field_value.ToString() + ":" + "Master ";
                                    break;
                                }
                            case SDRFieldType.DYNAMIC_ADDR_SLAVE:
                                {
                                    dynamicaddress += sdrfield.field_value.ToString() + ":" + "Slave";
                                    break;
                                }
                            case SDRFieldType.DCR_MASTER:
                                {
                                    dcr += sdrfield.field_value.ToString() + ":" + "Slave";
                                    break;
                                }
                            case SDRFieldType.DCR_SLAVE:
                                {
                                    dcr += sdrfield.field_value.ToString() + ":" + "Slave";
                                    break;
                                }
                            case SDRFieldType.BCR_MASTER:
                                {
                                    bcr += sdrfield.field_value.ToString() + ":" + "Slave";
                                    break;
                                }
                            case SDRFieldType.BCR_SLAVE:
                                {
                                    bcr += sdrfield.field_value.ToString() + ":" + "Slave";
                                    break;
                                }
                            default: // 默认当作数据处理
                                {
                                    reponsedata.Add(sdrfield.field_value);
                                    break;
                                }

                        }
                    }
                    
                    // 事件列表处理
                    eventinfo.EventInofs[_EVENT_FIELD_DYNAMIC_ADDRESS] = (Encoding.UTF8.GetBytes(dynamicaddress), 0);
                    eventinfo.EventInofs[_EVENT_FIELD_DCR] = (Encoding.UTF8.GetBytes(dcr), 0);
                    eventinfo.EventInofs[_EVENT_FIELD_BCR] = (Encoding.UTF8.GetBytes(bcr), 0);
                    eventinfo.EventInofs[_EVENT_FIELD_RESPONSE] = (reponsedata.ToArray<Byte>(), (UInt32)reponsedata.Count() * 8);
                    eventinfo.EventInofs[_EVENT_FIELD_PARITY] = (parity.ToArray<Byte>(), (UInt32)parity.Count() * 8);
                    eventinfo.EventInofs[_EVENT_FIELD_ERROR] = (Encoding.UTF8.GetBytes(error), 0);

                }

                // 事件信息
                _EventInfos.Add(eventinfo);

            }
            // 解码结果释放
            DecoderImpl.FreeI3C(ref results);

            _ResultData.DecodeViewInfos = decodepackets.ToArray();
            decoderesults.Add(_ResultData);

        }



        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolSPMIOptions()
            {
                CheckType = CheckType,
                CLKThreshold = _CLKThreshold,
                DataThreshold = _DataThreshold,
                SCLK = SCLK,
                SData = SData,
            };
        }
    }
}

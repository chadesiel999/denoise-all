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
using static ScopeX.ComModel.ProtocolAudioBus;
using static ScopeX.Core.Decode.EdgeInfoCPP;
using static ScopeX.Core.Decode.I2CDecodeModel;
using ScopeX.Core.Decode;
using System.Collections;

namespace ScopeX.Core.Decode
{
    internal sealed class SPMIDecodeModel : ProtocolModel
    {
        private DecodeResultData _ResultData = new DecodeResultData();
        private List<PAM2EdgePulse> _ClkPulsesList = new List<PAM2EdgePulse>();
        private List<PAM2EdgePulse> _DataPulsesList = new List<PAM2EdgePulse>();

        public SPMIDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id, SerialProtocolType.SPMI, isTrigDecode)
        {
            _ResultData.Name = "SPMI";
        }


        public override IReadOnlyList<String> EventInfoTitles { get; } = new List<String>()
        {
            "Index",
            "Start Time",
            "Master ID",
            "Primary Level",
            "Secondary Level",
            "Slave Address",
            "Command Type",
            "Command Address",
            "Register Address",
            "Byte Count",
            "Data",
            "Parity",
            "ACK/Nack",
            "Error",
        };

        // 事件字段index
        private const Int32 _EVENT_FIELD_MASTER_ID = 0;
        private const Int32 _EVENT_FIELD_PRIMARY_LEVEL = 1;
        private const Int32 _EVENT_FIELD_SECONDARY_LEVEL = 2;
        private const Int32 _EVENT_FIELD_SLAVE_ADDRESS = 3;
        private const Int32 _EVENT_FIELD_COMMAND_TYPE = 4;
        private const Int32 _EVENT_FIELD_COMMAND_ADDRESS = 5;
        private const Int32 _EVENT_FIELD_REGISTER_ADDRESS = 6;
        private const Int32 _EVENT_FIELD_BYTE_COUNT = 7;
        private const Int32 _EVENT_FIELD_DATA = 8;
        private const Int32 _EVENT_FIELD_PARITY = 9;
        private const Int32 _EVENT_FIELD_ACK_NACK = 10;
        private const Int32 _EVENT_FIELD_ERROR = 11;

        private ProtocolSPMI.CheckType _CheckType;
        public ProtocolSPMI.CheckType CheckType
        {
            get { return _CheckType; }
            set { UpdateProperty(ref _CheckType, value); }
        }
        private ProtocolSPMI.Version _Version;
        public ProtocolSPMI.Version Version
        {
            get { return _Version; }
            set { UpdateProperty(ref _Version, value); }
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
            SPMIOption option = new SPMIOption()
            {
                spmi_signal_version = (SPMISignalVersion)(Version + 1),
            };

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

            List<SPMIDecodePacket> decodepackets = new List<SPMIDecodePacket>();

            SPMIResult results;
            results.EventCount = 0;
            results.Event = IntPtr.Zero;

            // 开始解码
            if (!DecoderImpl.DecodeSPMI(ref option, clkptr, dataptr, out results))
            {

            }
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
            Int32 eventsize = Marshal.SizeOf(typeof(SPMIEvent));
            Int32 framesize = Marshal.SizeOf(typeof(SPMIFrame));
            Int32 arbitrationsize = Marshal.SizeOf(typeof(ArbitrationInfo));
            Int32 errorsize = Marshal.SizeOf(typeof(SPMIError));

            // 默认事件显示
            String temp_info = "--";

            // event count
            for (Int32 i = 0; i < results.EventCount; i++)
            {
                ProtocolEventInfo eventinfo = new ProtocolEventInfo();

                eventinfo.Index = _EventInfos.Count;
                eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));

                SPMIEvent spmievent = (SPMIEvent)Marshal.PtrToStructure(results.Event + i * eventsize, typeof(SPMIEvent));

                eventinfo.StartTimeByPs = base.GetTimeFromPosition(spmievent.event_start_index, clkindex);
                eventinfo.StartPosition = CalcPosition((Int32)spmievent.event_start_index, _SCLK, clkindex);
                eventinfo.EndPosition  = CalcPosition((Int32)spmievent.event_end_index, _SCLK, clkindex);
                eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, clkindex);

                //eventinfo.EventInofs[0]  = (Encoding.Default.GetBytes(temp_info), 0);   // Index
                //eventinfo.EventInofs[1]  = (Encoding.Default.GetBytes(temp_info), 0);   // Start Time
                eventinfo.EventInofs[_EVENT_FIELD_MASTER_ID] = (Encoding.Default.GetBytes(temp_info), 0);   // Master ID
                eventinfo.EventInofs[_EVENT_FIELD_PRIMARY_LEVEL] = (Encoding.Default.GetBytes(temp_info), 0);   // Primary Level
                eventinfo.EventInofs[_EVENT_FIELD_SECONDARY_LEVEL] = (Encoding.Default.GetBytes(temp_info), 0);   // Secondary Level
                eventinfo.EventInofs[_EVENT_FIELD_SLAVE_ADDRESS] = (Encoding.Default.GetBytes(temp_info), 0);   // Slave Address
                eventinfo.EventInofs[_EVENT_FIELD_COMMAND_TYPE] = (Encoding.Default.GetBytes(temp_info), 0);   // Command Type
                eventinfo.EventInofs[_EVENT_FIELD_COMMAND_ADDRESS] = (Encoding.Default.GetBytes(temp_info), 0);   // Command Address
                eventinfo.EventInofs[_EVENT_FIELD_REGISTER_ADDRESS] = (Encoding.Default.GetBytes(temp_info), 0);   // Register Address
                eventinfo.EventInofs[_EVENT_FIELD_BYTE_COUNT] = (Encoding.Default.GetBytes(temp_info), 0);   // Byte Count
                eventinfo.EventInofs[_EVENT_FIELD_DATA] = (Encoding.Default.GetBytes(temp_info), 0);   // Data
                eventinfo.EventInofs[_EVENT_FIELD_PARITY] = (Encoding.Default.GetBytes(temp_info), 0);   // Parity
                eventinfo.EventInofs[_EVENT_FIELD_ACK_NACK] = (Encoding.Default.GetBytes(temp_info), 0);   // ACK/Nack
                eventinfo.EventInofs[_EVENT_FIELD_ERROR] = (Encoding.Default.GetBytes(temp_info), 0);   // Error

                // 仲裁
                if (spmievent.arbitration_info != IntPtr.Zero)
                {
                    ArbitrationInfo arbitrationinfo = (ArbitrationInfo)Marshal.PtrToStructure(spmievent.arbitration_info, typeof(ArbitrationInfo));

                    // start
                    SPMIDecodePacket startpacket = new SPMIDecodePacket(CalcPosition((Int32)spmievent.event_start_index, _SCLK, clkindex), 1,
                                                                         SPMIFieldType.FIELD_ARBITRATION_START);
                    decodepackets.Add(startpacket);

                    // c
                    SPMIDecodePacket cbitpacket = new SPMIDecodePacket(CalcPosition((Int32)arbitrationinfo.cbit_start_index, _SCLK, clkindex), CalcBitLenght((Int32)(arbitrationinfo.cbit_end_index - arbitrationinfo.cbit_start_index), _SCLK, clkindex),
                                                  SPMIFieldType.FIELD_CONNECT_BIT)
                    {
                        _BitCount = 1,
                        _Title = "C Bit",
                        Data = new Byte[1],
                    };
                    cbitpacket.Data[0] = arbitrationinfo.connect_bit;
                    decodepackets.Add(cbitpacket);

                    // master
                    if (arbitrationinfo.connect_bit == 1)
                    {
                        SPMIDecodePacket masteridpacket = new SPMIDecodePacket(CalcPosition((Int32)arbitrationinfo.masterid_start_index, _SCLK, clkindex), CalcBitLenght((Int32)(arbitrationinfo.masterid_end_index - arbitrationinfo.masterid_start_index), _SCLK, clkindex),
                                                           SPMIFieldType.FIELD_MASTER_ID)
                        {
                            _BitCount = 3,
                            _Title = "Master Id",
                            Data = new Byte[1],
                        };
                        masteridpacket.Data[0] = arbitrationinfo.mastet_id;
                        decodepackets.Add(masteridpacket);

                        // Master Id
                        eventinfo.EventInofs[_EVENT_FIELD_MASTER_ID] = (masteridpacket.Data, 3);
                    }


                    SPMIDecodePacket alertpacket = new SPMIDecodePacket(CalcPosition((Int32)arbitrationinfo.abit_start_index, _SCLK, clkindex), CalcBitLenght((Int32)(arbitrationinfo.abit_end_index - arbitrationinfo.abit_start_index), _SCLK, clkindex),
                                  SPMIFieldType.FIELD_ALERT_BIT)
                    {
                        _BitCount = 1,
                        _Title = "Alert Bit",
                        Data = new Byte[1],
                    };
                    alertpacket.Data[0] = arbitrationinfo.alert_bit;
                    decodepackets.Add(alertpacket);

                    // abit
                    if (arbitrationinfo.alert_bit == 1)
                    {
                        SPMIDecodePacket addrpacket = new SPMIDecodePacket(CalcPosition((Int32)arbitrationinfo.slave_start_index, _SCLK, clkindex), CalcBitLenght((Int32)(arbitrationinfo.slave_end_index - arbitrationinfo.slave_start_index), _SCLK, clkindex),
                                  SPMIFieldType.FIELD_SLAVE_ADDRESS)
                        {
                            _BitCount = 4,
                            _Title = "Primary Slave Addr",
                            Data = new Byte[1],
                        };
                        addrpacket.Data[0] = arbitrationinfo.slave_addr;
                        decodepackets.Add(addrpacket);

                        // slave addr 
                        eventinfo.EventInofs[_EVENT_FIELD_SLAVE_ADDRESS] = ((Encoding.UTF8.GetBytes(addrpacket.Title +":"+ arbitrationinfo.slave_addr.ToString()), 0));
                    }
                    else
                    {
                        SPMIDecodePacket masterlevelpacket = new SPMIDecodePacket(CalcPosition((Int32)arbitrationinfo.masterid_start_index, _SCLK, clkindex), CalcBitLenght((Int32)(arbitrationinfo.masterid_end_index - arbitrationinfo.masterid_start_index), _SCLK, clkindex),
                                  SPMIFieldType.FIELD_PRIMARY_LEVEL)
                        {
                            _BitCount = 4,
                            _Title = "Primary Level",
                            Data = new Byte[1],
                        };
                        masterlevelpacket.Data[0] = arbitrationinfo.master_priority_level;
                        decodepackets.Add(masterlevelpacket);

                        eventinfo.EventInofs[_EVENT_FIELD_PRIMARY_LEVEL] = (masterlevelpacket.Data, 4);

                        if (arbitrationinfo.srbit_start_index != 0)
                        {
                            String masterpriority = "NONE";

                            eventinfo.EventInofs[_EVENT_FIELD_PRIMARY_LEVEL] = (Encoding.UTF8.GetBytes(masterpriority), 0);

                            SPMIDecodePacket srbitpacket = new SPMIDecodePacket(CalcPosition((Int32)arbitrationinfo.srbit_start_index, _SCLK, clkindex), CalcBitLenght((Int32)(arbitrationinfo.srbit_end_index - arbitrationinfo.srbit_start_index), _SCLK, clkindex),
                              SPMIFieldType.FIELD_SR_BIT)
                            {
                                _BitCount = 1,
                                _Title = "SR Bit",
                                Data = new Byte[1],
                            };
                            srbitpacket.Data[0] = arbitrationinfo.sr_bit;

                            decodepackets.Add(srbitpacket);
                        }

                        // SR bit
                        if (arbitrationinfo.sr_bit == 1)
                        {

                            // 二次 sr
                            SPMIDecodePacket secondaddrpacket = new SPMIDecodePacket(CalcPosition((Int32)arbitrationinfo.slave_start_index, _SCLK, clkindex), CalcBitLenght((Int32)(arbitrationinfo.slave_end_index - arbitrationinfo.slave_start_index), _SCLK, clkindex),
                             SPMIFieldType.FIELD_SLAVE_ADDRESS)
                            {
                                _BitCount = 4,
                                _Title = "Second Slave Addr",
                                Data = new Byte[1],
                            };
                            secondaddrpacket.Data[0] = arbitrationinfo.slave_addr;
                            decodepackets.Add(secondaddrpacket);

                            // slave addr 
                            eventinfo.EventInofs[_EVENT_FIELD_SLAVE_ADDRESS] = ((Encoding.UTF8.GetBytes(secondaddrpacket.Title + ":"+ arbitrationinfo.slave_addr.ToString()), 0));
                        }
                        else
                        {
                            if (arbitrationinfo.secondary_mpl_start_index != 0)
                            {
                                SPMIDecodePacket secondlevelpacket = new SPMIDecodePacket(CalcPosition((Int32)arbitrationinfo.secondary_mpl_start_index, _SCLK, clkindex), CalcBitLenght((Int32)(arbitrationinfo.secondary_mpl_end_index - arbitrationinfo.secondary_mpl_start_index), _SCLK, clkindex),
                                 SPMIFieldType.FIELD_SECONDARY_LEVEL)
                                {
                                    _BitCount = 4,
                                    _Title = "Secondary Level",
                                    Data = new Byte[1],
                                };
                                secondlevelpacket.Data[0] = arbitrationinfo.secondary_master_priority_level;
                                decodepackets.Add(secondlevelpacket);

                                eventinfo.EventInofs[_EVENT_FIELD_SECONDARY_LEVEL] = (secondlevelpacket.Data, 4);
                            }
                        }
                    }
                }

                // command start
                SPMIDecodePacket commandstartpacket = new SPMIDecodePacket(CalcPosition((Int32)spmievent.command_frame_start, _SCLK, clkindex), 1,
                                                                     SPMIFieldType.FIELD_ARBITRATION_START);
                decodepackets.Add(commandstartpacket);

                // 命令解析
                SPMIDecodePacket commandaddrpacket = new SPMIDecodePacket(CalcPosition((Int32)spmievent.command_addr_start_index, _SCLK, clkindex), CalcBitLenght((Int32)(spmievent.command_addr_end_index - spmievent.command_addr_start_index), _SCLK, clkindex),
                 SPMIFieldType.FIELD_COMMAND_ADDRESS)
                {
                    _BitCount = 4,
                    _Title = "Command Addr",
                    Data = new Byte[1],
                };
                commandaddrpacket.Data[0] = spmievent.field_command_addr;
                decodepackets.Add(commandaddrpacket);
                eventinfo.EventInofs[_EVENT_FIELD_COMMAND_ADDRESS] = (commandaddrpacket.Data, 4);

                // 命令帧
                String? commandname = Enum.GetName(typeof(CommandFrameType), spmievent.command_frame_type); //推荐 

                SPMIDecodePacket commandpacket = new SPMIDecodePacket(CalcPosition((Int32)spmievent.command_frame.field_start_index, _SCLK, clkindex), CalcBitLenght((Int32)(spmievent.command_frame.field_end_index - spmievent.command_frame.field_start_index), _SCLK, clkindex),
                SPMIFieldType.FIELD_COMMAND_TYPE)
                {
                    _BitCount = 8,
                    _Title = commandname,
                    Data = new Byte[1],
                };
                commandpacket.Data[0] = spmievent.command_frame.field_content;
                decodepackets.Add(commandpacket);

                eventinfo.EventInofs[_EVENT_FIELD_COMMAND_TYPE] = (Encoding.UTF8.GetBytes(commandname), 0);

                List<Byte> data = new List<Byte>();
                List<Byte> address = new List<Byte>();
                List<Byte> parity = new List<Byte>();

                parity.Add(spmievent.command_frame.parity);

                SPMIDecodePacket commandparity = new SPMIDecodePacket(CalcPosition((Int32)spmievent.command_frame.parity_start_index, _SCLK, clkindex), CalcBitLenght((Int32)(spmievent.command_frame.parity_end_index - spmievent.command_frame.parity_start_index), _SCLK, clkindex),
                                                 SPMIFieldType.FIELD_PARITY)
                {
                    _BitCount = 8,
                    _Title = "Parity",
                    Data = new Byte[1],
                };
                commandparity.Data[0] = spmievent.command_frame.parity;
                if (spmievent.command_frame.parity_result == 0) // 报错
                {
                    commandparity.PacketType = SPMIFieldType.FIELD_ERROR;
                }

                decodepackets.Add(commandparity);

                if (spmievent.command_frame_type == CommandFrameType.EXTENDED_REGISTER_WRITE ||
                    spmievent.command_frame_type == CommandFrameType.EXTENDED_REGISTER_READ ||
                    spmievent.command_frame_type == CommandFrameType.EXTENDED_REGISTER_WRITE_LONG ||
                    spmievent.command_frame_type == CommandFrameType.EXTENDED_REGISTER_READ_LONG)
                {
                    Byte[] bytecount = new Byte[1];
                    bytecount[0] = spmievent.field_command_frame_extension;

                    eventinfo.EventInofs[_EVENT_FIELD_BYTE_COUNT] = (bytecount, 8);
                }
                else if (spmievent.command_frame_type == CommandFrameType.REGISTER_WRITE || spmievent.command_frame_type == CommandFrameType.REGISTER_READ)
                {
                    address.Add(spmievent.field_command_frame_extension);
                }
                else if (spmievent.command_frame_type == CommandFrameType.CFT_REGISTER_0_WRITE)
                {
                    data.Add(spmievent.field_command_frame_extension);
                }

                for (Int32 j = 0; j < spmievent.spmi_frame_size; ++j)
                {
                    SPMIFrame spmiframe = (SPMIFrame)Marshal.PtrToStructure(spmievent.spmi_frame_ptr + j * framesize, typeof(SPMIFrame));

                    SPMIDecodePacket framepacket = new SPMIDecodePacket(CalcPosition((Int32)spmiframe.field_start_index, _SCLK, clkindex),
                                                                        CalcBitLenght((Int32)(spmiframe.field_end_index - spmiframe.field_start_index), _SCLK, clkindex),
                                                                        SPMIFieldType.FIELD_DATA)
                    {
                        _BitCount = 8,
                        _Title = "",
                        Data = new Byte[1],
                    };
                    framepacket.Data[0] = spmiframe.field_content;


                    SPMIDecodePacket paritypakcet = new SPMIDecodePacket(CalcPosition((Int32)spmiframe.parity_start_index - 1, _SCLK, clkindex), CalcBitLenght((Int32)(spmiframe.parity_end_index - spmiframe.parity_start_index), _SCLK, clkindex),
                                 SPMIFieldType.FIELD_PARITY)
                    {
                        _BitCount = 8,
                        _Title = "Parity",
                        Data = new Byte[1],
                    };
                    paritypakcet.Data[0] = spmiframe.parity;
                    if (spmiframe.parity_result == 0) // 报错
                    {
                        commandparity.PacketType = SPMIFieldType.FIELD_ERROR;
                    }
                    decodepackets.Add(paritypakcet);

                    if (spmiframe.frame_type == SPMIFrameType.SPMI_FRAME_TYPE_ADRESS)
                    {
                        framepacket._Title = "Address";

                        address.Add(spmiframe.field_content);
                    }
                    else if (spmiframe.frame_type == SPMIFrameType.SPMI_FRAME_TYPE_DATA)
                    {
                        framepacket._Title = "Data";

                        data.Add(spmiframe.field_content);
                    }
                    else if (spmiframe.frame_type == SPMIFrameType.SPMI_FRAME_TYPE_CHALLENGE_DATA)
                    {
                        framepacket._Title = "Challenge Data";
                        data.Add(spmiframe.field_content);
                    }
                    else if (spmiframe.frame_type == SPMIFrameType.SPMI_FRAME_TYPE_RESPONSE_DATA)
                    {
                        framepacket._Title = "Response Data";
                        data.Add(spmiframe.field_content);
                    }
                    decodepackets.Add(framepacket);
                    parity.Add(spmiframe.parity);
                }

                if (spmievent.ack_nack_start_index != 0)
                {
                    SPMIDecodePacket framepacket = new SPMIDecodePacket(CalcPosition((Int32)spmievent.ack_nack_start_index, _SCLK, clkindex),
                                                    CalcBitLenght((Int32)(spmievent.ack_nack_end_index - spmievent.ack_nack_start_index), _SCLK, clkindex),
                                                    SPMIFieldType.FIELD_ACKNACK)
                    {
                        _BitCount = 8,
                        _Title = "",
                        Data = new Byte[1],
                    };
                    framepacket.Data[0] = spmievent.field_ack_nack;
                    eventinfo.EventInofs[_EVENT_FIELD_ACK_NACK] = (framepacket.Data, 8);
                }

                eventinfo.EventInofs[_EVENT_FIELD_REGISTER_ADDRESS] = (address.ToArray<Byte>(), (UInt32)address.Count() * 8);
                eventinfo.EventInofs[_EVENT_FIELD_DATA] = (data.ToArray<Byte>(), (UInt32)data.Count() * 8);
                eventinfo.EventInofs[_EVENT_FIELD_PARITY] = (parity.ToArray<Byte>(), (UInt32)parity.Count() * 8);

                // 事件信息
                _EventInfos.Add(eventinfo);
            }

            // 解码结果释放
            DecoderImpl.FreeSPMI(ref results);

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
                Version = Version,
            };
        }
    }
}

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
using ScopeX.Core.Decode;
using System.Collections;
using System.Numerics;
using System.Diagnostics;

namespace ScopeX.Core.Decode
{
    internal sealed class SMBusDecodeModel : ProtocolModel
    {
        private DecodeResultData _ResultData = new DecodeResultData();
        private List<PAM2EdgePulse> _ClkPulsesList = new List<PAM2EdgePulse>();
        private List<PAM2EdgePulse> _DataPulsesList = new List<PAM2EdgePulse>();

        public SMBusDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id, SerialProtocolType.SMBus, isTrigDecode)
        {
        }
        public override IReadOnlyList<String> EventInfoTitles { get; } = (new List<String>()
        {
            "Index",
            "Start Time",
            "Protocol Type",
            "Address",
            "RD/WR",
            "Command Code",
            "Byte Count",
            "Data_LSB_MSB",
            "Acknowledgement",
            "PEC",
            "Error",
        }).AsReadOnly();

        // 事件字段index
        private const int _EVENT_FIELD_PROTOCOL_TYPE = 0;
        private const int _EVENT_FIELD_ADDRESS = 1;
        private const int _EVENT_FIELD_RD_WR = 2;
        private const int _EVENT_FIELD_COMMAND_CODE = 3;
        private const int _EVENT_FIELD_BYTE_COUNT = 4;
        private const int _EVENT_FIELD_DATA_LSB_MSB = 5;
        private const int _EVENT_FIELD_ACK = 6;
        private const int _EVENT_FIELD_PEC = 7;
        private const int _EVENT_FIELD_ERROR = 8;


        private ProtocolSMBus.PECByte _PECByte;
        public ProtocolSMBus.PECByte PECByte
        {
            get { return _PECByte; }
            set { UpdateProperty(ref _PECByte, value); }
        }
        private ChannelId _SMBData = ChannelId.C1;
        //通道选择 Data
        public ChannelId SMBData
        {
            get { return _SMBData; }
            set { UpdateProperty(ref _SMBData, value); }
        }
        //通道选择 Clk
        private ChannelId _SMBClk = ChannelId.C2;

        public ChannelId SMBClk
        {
            get { return _SMBClk; }
            set { UpdateProperty(ref _SMBClk, value); }
        }
        //门限阈值 Data
        private Double _DataThreshold = 0;

        public Double DataThreshold
        {
            get { return (float)(_DataThreshold * TryGetChannelGain(SMBData)); }
            set { UpdateProperty(ref _DataThreshold, (float)(value / TryGetChannelGain(SMBData))); }
        }
        public String DataUnit => GetChannelUnit(SMBData);
        public Double MaxThreshold => 20;
        public Double MinThreshold => -MaxThreshold;
        //门限阈值 Clk
        private Double _CLKThreshold = 0;

        public Double CLKThreshold
        {
            get { return (float)(_CLKThreshold * TryGetChannelGain(SMBClk)); }
            set { UpdateProperty(ref _CLKThreshold, (float)(value / TryGetChannelGain(SMBClk))); }
        }
        public String CLKUnit => GetChannelUnit(SMBClk);

        internal override Boolean CheckUpdate(ref Int64 laststamp)
        {
            if (_SMBClk.IsAnalog() && laststamp != DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp;
                return true;
            }
            if (_SMBClk.IsReference() && laststamp != DecodeDataHelper.Instance.ReferenceDataSource[_SMBClk - ChannelIdExt.MinRChId].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.ReferenceDataSource[_SMBClk - ChannelIdExt.MinRChId].TimeStamp;
                return true;
            }
            return false;
        }

        public override void UpdateReferenceDataStatus()
        {
            if (_SMBClk.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[_SMBClk - ChannelIdExt.MinRChId].Channels != null
                && DecodeDataSource.Instance.ReferenceDataSource[_SMBClk - ChannelIdExt.MinRChId].Channels[0] == _SMBClk)
            {
                DecodeDataSource.Instance.ReferenceDataSource[_SMBClk - ChannelIdExt.MinRChId].HasData = false;
            }
            if (_SMBData.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[_SMBData - ChannelIdExt.MinRChId].Channels != null
                && DecodeDataSource.Instance.ReferenceDataSource[_SMBData - ChannelIdExt.MinRChId].Channels[0] == _SMBData)
            {
                DecodeDataSource.Instance.ReferenceDataSource[_SMBData - ChannelIdExt.MinRChId].HasData = false;
            }
        }
        //判断是否有数据
        internal override Boolean SourceHasData()
        {
            if (DsoPrsnt.DefaultDsoPrsnt == null)
                return false;

            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(_SMBClk, out IChnlPrsnt? scl_prsnt);
            if (scl_prsnt == null)
                return false;

            Boolean clk = false, data = false;

            if (_SMBClk.IsReference() && scl_prsnt.VuDatabase.Current != null)
            {
                clk = DecodeDataHelper.ReferenceHasData(_SMBClk, _CLKThreshold);
            }

            if (_SMBClk.IsAnalog())
            {
                clk = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }


            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(_SMBData, out IChnlPrsnt? prsnt);
            if (prsnt == null)
                return false;

            if (_SMBData.IsReference() && prsnt.VuDatabase.Current != null)
            {
                data = DecodeDataHelper.ReferenceHasData(_SMBData, _DataThreshold);
            }

            if (_SMBData.IsAnalog())
            {
                data = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }
            return (data || clk);
        }
        //数据处理
        internal override void ParsingData(ref CancellationToken token)
        {
            Int32 clkindex = GetChIndex(_SMBClk);
            Int32 dataindex = GetChIndex(_SMBData);

            UInt32 clklen = 0, datalen = 0;

            Double clsamplerate = 0, datasamplerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, _SMBClk, ref clsamplerate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, _SMBClk, ref clklen);

            DecodeDataHelper.Instance.TryGetSampleRate(BusId, _SMBData, ref datasamplerate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, _SMBData, ref datalen);

            if (MoreThanStorage() || clkindex == -1 || dataindex == -1 || clklen == 0 || datalen == 0)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _ResultData.DecodeViewInfos = new IDecodeViewInfo[0];
                _EventInfos.Clear();
            }

            if (!_NeedDecodeData && !_NeedUpdateViewInfo)
                return;

            //token = new CancellationToken();//TODO:ycf ?
            _NeedDecodeData = false;
            _NeedUpdateViewInfo = true;

            Boolean needclear = false;

            // 输入项
            SMBusOption option = new SMBusOption()
            {
                SmbusPecState = (SMBusPEC)(PECByte),
            };

            IntPtr clkptr = IntPtr.Zero, dataptr = IntPtr.Zero;
            GCHandle clkhandle, datahandle;

            //获取边沿
            TwoLevelEdgeInfo? clknode = DecodeDataHelper.Instance.GetEdgeInfo(BusId, startindex: 0, _SMBClk, ref token, ref needclear) as TwoLevelEdgeInfo;
            if (clknode == null)
            {
                return;
            }
            _ClkPulsesList.Clear();
            DecodeDataHelper.Instance.GetTwoLevelPulses(ref clknode, ref _ClkPulsesList);
            PAM2EdgePulseSequence.Allocate(ref _ClkPulsesList, (UInt64)clklen, clsamplerate, out clkptr, out clkhandle);

            //获取边沿
            TwoLevelEdgeInfo? datanode = DecodeDataHelper.Instance.GetEdgeInfo(BusId, startindex: 0, _SMBData, ref token, ref needclear) as TwoLevelEdgeInfo;
            if (datanode == null)
            {
                return;
            }
            _DataPulsesList.Clear();

            DecodeDataHelper.Instance.GetTwoLevelPulses(ref datanode, ref _DataPulsesList);
            PAM2EdgePulseSequence.Allocate(ref _DataPulsesList, (UInt64)datalen, datasamplerate, out dataptr, out datahandle);

            /*清除界面*/
            List<SMBusDecodePacket> decodepackets = new List<SMBusDecodePacket>();
            //开始解码
            SMBusResult results;
            results.EventCount = 0;
            results.Event = IntPtr.Zero;

            // 开始解码
            if (!DecoderImpl.DecodeSMBus(ref option, clkptr, dataptr, out results))
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
            Int32 eventsize = Marshal.SizeOf(typeof(SMBusEvent));//Marshal 与 C/C++ 交互 在托管代码中操作非托管数据结构（如 struct 或 byte[]）
            Int32 byteinfosize = Marshal.SizeOf(typeof(SMBusDataInfo));
            Int32 arpinfosize = Marshal.SizeOf(typeof(SMBusArpDataInfo));

            // 默认事件显示
            String temp_info = "--";

            // event count
            for (Int32 i = 0; i < results.EventCount; i++)
            {

                ProtocolEventInfo eventinfo = new ProtocolEventInfo();
                var endindex = 0;//用以定位帧

                eventinfo.Index = _EventInfos.Count;
                eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));

                /*将事件指针数据转换为SMBusEvent结构体，提取事件的起始位置和时间。*/
                SMBusEvent smbusevent = (SMBusEvent)Marshal.PtrToStructure(results.Event + i * eventsize, typeof(SMBusEvent));

                eventinfo.StartTimeByPs = base.GetTimeFromPosition(smbusevent.EventStartIndex, clkindex);
                eventinfo.StartPosition = CalcPosition((Int64)smbusevent.EventStartIndex, _SMBClk, clkindex);

                /*先将事件表用“--”填充*/
                //eventinfo.EventInofs[0]  = (Encoding.Default.GetBytes(temp_info), 0);   // Index
                //eventinfo.EventInofs[1]  = (Encoding.Default.GetBytes(temp_info), 0);   // Start Time
                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(temp_info), 0);// Protocol Type
                eventinfo.EventInofs[_EVENT_FIELD_ADDRESS] = (Encoding.Default.GetBytes(temp_info), 0);     //  Address
                eventinfo.EventInofs[_EVENT_FIELD_RD_WR] = (Encoding.Default.GetBytes(temp_info), 0);       //  RD/WR
                eventinfo.EventInofs[_EVENT_FIELD_COMMAND_CODE] = (Encoding.Default.GetBytes(temp_info), 0);//  Command Code
                eventinfo.EventInofs[_EVENT_FIELD_BYTE_COUNT] = (Encoding.Default.GetBytes(temp_info), 0);  //  Byte Count
                eventinfo.EventInofs[_EVENT_FIELD_DATA_LSB_MSB] = (Encoding.Default.GetBytes(temp_info), 0);//  Data_LSB_MSB
                eventinfo.EventInofs[_EVENT_FIELD_ACK] = (Encoding.Default.GetBytes(temp_info), 0);         //  Acknowledgement
                eventinfo.EventInofs[_EVENT_FIELD_PEC] = (Encoding.Default.GetBytes(temp_info), 0);         //  PEC
                eventinfo.EventInofs[_EVENT_FIELD_ERROR] = (Encoding.Default.GetBytes(temp_info), 0);       //  Error

                // step1. start帧 位置
                //CalcPosition计算从事件开始到当前索引的相对位置，可能用来定位时间点或物理位置。
                SMBusStartDecodePacket startpacket = new SMBusStartDecodePacket(CalcPosition((int)smbusevent.EventStartIndex, _SMBData, clkindex), 1)
                {
                    _Title = "Start",
                };
                decodepackets.Add(startpacket);

                // step2. 数据帧
                if (smbusevent.DataInfoPtr != IntPtr.Zero)
                {
                    string protocolinfo = "--";
                    //ACK字段 Data字段 Command命令码段 ARP特殊Data字段
                    List<byte> AckFiledType = new List<byte>();
                    List<byte> DataFiledType = new List<byte>();
                    List<byte> CommandCodeFiledType = new List<byte>();
                    List<SMBusDataInfo> ArpDataFiledType = new List<SMBusDataInfo>();
                    //String ackstr = "";


                    //step2. 命令类型
                    switch (smbusevent.EventProtocol)
                    {
                        case SMBusCommandProtocol.QuickCommand:
                            {
                                protocolinfo = "Quick Command";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.ReceiveByte:
                            {
                                protocolinfo = "Receive Byte";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.SendByte:
                            {
                                protocolinfo = "Send Byte";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.HostNotifyProtocol:
                            {
                                protocolinfo = "Host Notify Protocol";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.NotifyArpMaster:
                            {
                                protocolinfo = "Notify ARP Master";
                                eventinfo.EventInofs[0] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.WriteByte:
                            {
                                protocolinfo = "Write Byte";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.WriteWord:
                            {
                                protocolinfo = "Write Word";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.Write32:
                            {
                                protocolinfo = "Write 32";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.Write64:
                            {
                                protocolinfo = "Write 64";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.BlockWrite:
                            {
                                protocolinfo = "Block Write";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.ReadByteCommand:
                            {
                                protocolinfo = "Read Byte Command";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.ReadByteResponse:
                            {
                                protocolinfo = "Read Byte Response";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.ReadWordCommand:
                            {
                                protocolinfo = "Read Word Command";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.ReadWordResponse:
                            {
                                protocolinfo = "Read Word Response";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.Read32Command:
                            {
                                protocolinfo = "Read 32 Command";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.Read32Response:
                            {
                                protocolinfo = "Read 32 Response";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.Read64Command:
                            {
                                protocolinfo = "Read 64 Command";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.Read64Response:
                            {
                                protocolinfo = "Read 64 Response";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.BlockReadCommand:
                            {
                                protocolinfo = "Block Read Command";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.BlockReadResponse:
                            {
                                protocolinfo = "Block Read Response";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.ProcessCallCommand:
                            {
                                protocolinfo = "Process Call Command";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.ProcessCallResponse:
                            {
                                protocolinfo = "Process Call Response";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.BlockWriteBlockReadProcessCallCommand:
                            {
                                protocolinfo = "BlockWrite BlockRead Process Call Command";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.BlockWriteBlockReadProcessCallResponse:
                            {
                                protocolinfo = "BlockWrite BlockRead Process Call Response";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.PrepareToArp:
                            {
                                protocolinfo = "Prepare To ARP";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.ResetDevice:
                            {
                                protocolinfo = "Reset Device (general)";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.ResetDeviceDirected:
                            {
                                protocolinfo = "Reset Device (directed)";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.AssignAddress:
                            {
                                protocolinfo = "Assign Address";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.GetUdidCommand:
                            {
                                protocolinfo = "Get UDID (general) Command";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.GetUdidResponse:
                            {
                                protocolinfo = "Get UDID (general) Response";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.GetUdidDirectedCommand:
                            {
                                protocolinfo = "Get UDID (directed) Command";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        case SMBusCommandProtocol.GetUdidDirectedResponse:
                            {
                                protocolinfo = "Get UDID (directed) Response";
                                eventinfo.EventInofs[_EVENT_FIELD_PROTOCOL_TYPE] = (Encoding.Default.GetBytes(protocolinfo), 0);
                                break;
                            }
                        default:
                            break;
                    }

                    //step3.根据Event中的DataInfoSize，把每个byte都取出来判断类型
                    for (Int32 j = 0; j < smbusevent.DataInfoSize; j++)
                    {
                        //从内存地址 smbusevent.DataInfoPtr + j * byteinfosize 读取数据，并将其解析为 SMBusDataInfo 结构体
                        SMBusDataInfo bytefieldinfo = (SMBusDataInfo)Marshal.PtrToStructure(smbusevent.DataInfoPtr + j * byteinfosize, typeof(SMBusDataInfo));
                        //ACK字段
                        AckFiledType.Add(bytefieldinfo.Ack);

                        //ACK NACK字段绘图 有错误时绘制
                        if (bytefieldinfo.AckError == 1)
                        {

                            if ((j == smbusevent.DataInfoSize - 1) && (smbusevent.SmbusErrorInfo.HasNackError == 1))//NACK错误
                            {
                                /*数据装入图形显示*/
                                SMBusAckDecodePacket AckErrorFiled = new SMBusAckDecodePacket(CalcPosition((long)bytefieldinfo.AckStartIndex, _SMBClk, clkindex),
                                                                         CalcBitLenght((int)(bytefieldinfo.AckEndIndex - bytefieldinfo.AckStartIndex), _SMBClk, clkindex))
                                {
                                    /*显示数据*/
                                    Data = new Byte[] { (Byte)bytefieldinfo.Ack },
                                    /*显示位数*/
                                    _BitCount = 1,
                                    /*显示标题*/
                                    _Title = "NACK",
                                    /*错误显示*/
                                    AckError = true,
                                    _AckNote = "Error: Unexpected ACK ",
                                };
                                decodepackets.Add(AckErrorFiled);
                            }
                            else
                            {
                                /*数据装入图形显示*/
                                SMBusAckDecodePacket AckErrorFiled = new SMBusAckDecodePacket(CalcPosition((long)bytefieldinfo.AckStartIndex, _SMBClk, clkindex),
                                                                         CalcBitLenght((int)(bytefieldinfo.AckEndIndex - bytefieldinfo.AckStartIndex), _SMBClk, clkindex))
                                {
                                    /*显示数据*/
                                    Data = new Byte[] { (Byte)bytefieldinfo.Ack },
                                    /*显示位数*/
                                    _BitCount = 1,
                                    /*显示标题*/
                                    _Title = "ACK",
                                    /*错误显示*/
                                    AckError = true,
                                    _AckNote = "Error: Unexpected NACK ",
                                };
                                decodepackets.Add(AckErrorFiled);


                            }
                        }

                        /*判断字段类型*/
                        switch (bytefieldinfo.Type)
                        {
                            //地址字段_读写字段
                            case SMBusDataType.SmbusAddrFiled:
                            case SMBusDataType.SmbusHostAddrFiled:
                                {
                                    //String addressstr = "";

                                    if (smbusevent.EventProtocol == SMBusCommandProtocol.HostNotifyProtocol)
                                    {
                                        SMBusAddressDecodePacket Address = new SMBusAddressDecodePacket(CalcPosition((long)bytefieldinfo.DataStartIndex, _SMBClk, clkindex),
                                                                           CalcBitLenght((int)(bytefieldinfo.DataEndIndex - bytefieldinfo.DataStartIndex), _SMBClk, clkindex))
                                        {
                                            Data = new Byte[] { (Byte)bytefieldinfo.Data },
                                            _BitCount = 7,
                                            _Title = "Host Address",
                                        };
                                        decodepackets.Add(Address);
                                    }
                                    else if (smbusevent.EventProtocol == SMBusCommandProtocol.NotifyArpMaster || smbusevent.EventProtocol == SMBusCommandProtocol.ResetDeviceDirected)
                                    {
                                        SMBusAddressDecodePacket Address = new SMBusAddressDecodePacket(CalcPosition((long)bytefieldinfo.DataStartIndex, _SMBClk, clkindex),
                                                                        CalcBitLenght((int)(bytefieldinfo.DataEndIndex - bytefieldinfo.DataStartIndex), _SMBClk, clkindex))
                                        {
                                            Data = new Byte[] { (Byte)bytefieldinfo.Data },
                                            _BitCount = 7,
                                            _Title = "Slave Address",
                                        };
                                        decodepackets.Add(Address);

                                    }
                                    else
                                    {
                                        SMBusAddressDecodePacket Address = new SMBusAddressDecodePacket(CalcPosition((long)bytefieldinfo.DataStartIndex, _SMBClk, clkindex),
                                                                            CalcBitLenght((int)(bytefieldinfo.DataEndIndex - bytefieldinfo.DataStartIndex), _SMBClk, clkindex))
                                        {
                                            Data = new Byte[] { (Byte)bytefieldinfo.Data },
                                            _BitCount = 7,
                                            _Title = "Address",
                                        };
                                        decodepackets.Add(Address);
                                    }
                                    /*数据装入图形显示*/
                                    SMBusWrDecodePacket WR = new SMBusWrDecodePacket(CalcPosition((long)bytefieldinfo.RwStartIndex, _SMBClk, clkindex),
                                                               CalcBitLenght((int)(bytefieldinfo.RwEndIndex - bytefieldinfo.RwStartIndex), _SMBClk, clkindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.Rw },
                                        _BitCount = 1,
                                        _Title = "WR",
                                    };
                                    decodepackets.Add(WR);

                                    /*数据装入事件列表*/
                                    byte[] AddrFiled = new byte[1];
                                    AddrFiled[0] = bytefieldinfo.Data;
                                    eventinfo.EventInofs[_EVENT_FIELD_ADDRESS] = (AddrFiled, 7);

                                    byte[] WrFiled = new byte[1];
                                    WrFiled[0] = bytefieldinfo.Rw;
                                    eventinfo.EventInofs[_EVENT_FIELD_RD_WR] = (WrFiled, 2);//1
                                    //endindex = (Int32)bytefieldinfo.RwEndIndex;
                                    break;
                                }
                            case SMBusDataType.SmbusCommandFiled: //命令码字段
                            case SMBusDataType.SmbusCommandDeviceAddrFiled: //命令码字段
                                {

                                    /*数据装入图形显示*/
                                    //String commandstr = "";
                                    if (smbusevent.EventProtocol == SMBusCommandProtocol.HostNotifyProtocol || smbusevent.EventProtocol == SMBusCommandProtocol.NotifyArpMaster)
                                    {

                                        SMBusCommandDecodePacket CommandCode = new SMBusCommandDecodePacket(CalcPosition((long)bytefieldinfo.DataStartIndex, _SMBClk, clkindex),
                                                                             CalcBitLenght((int)(bytefieldinfo.DataEndIndex - bytefieldinfo.DataStartIndex), _SMBClk, clkindex))
                                        {
                                            Data = new Byte[] { (Byte)bytefieldinfo.Data },
                                            _BitCount = 7,
                                            _Title = "Device Address",
                                        };
                                        SMBusCommandDecodePacket Bit = new SMBusCommandDecodePacket(CalcPosition((long)bytefieldinfo.RwStartIndex, _SMBClk, clkindex),
                                                                        CalcBitLenght((int)(bytefieldinfo.RwEndIndex - bytefieldinfo.RwStartIndex), _SMBClk, clkindex))
                                        {
                                            Data = new Byte[] { (Byte)bytefieldinfo.Rw },
                                            _BitCount = 1,
                                            _Title = "Bit",
                                        };
                                        decodepackets.Add(CommandCode);
                                        decodepackets.Add(Bit);

                                        /*数据装入事件列表*/
                                        CommandCodeFiledType.Add(bytefieldinfo.Data);
                                        CommandCodeFiledType.Add(bytefieldinfo.Rw);
                                        ///*Command命令码字段*/
                                        eventinfo.EventInofs[_EVENT_FIELD_COMMAND_CODE] = (CommandCodeFiledType.ToArray<byte>(), (uint)CommandCodeFiledType.Count() * 8);

                                        //commandstr = commandstr + "Device Address: " + bytefieldinfo.Data.ToString("X2");
                                        //commandstr = commandstr + " Bit: " + bytefieldinfo.Rw.ToString("X1");
                                        //eventinfo.EventInofs[_EVENT_FIELD_COMMAND_CODE] = (Encoding.Default.GetBytes(commandstr), 0);
                                        //endindex = (Int32)bytefieldinfo.RwEndIndex;
                                        break;

                                    }
                                    else if (smbusevent.EventProtocol == SMBusCommandProtocol.GetUdidDirectedCommand || smbusevent.EventProtocol == SMBusCommandProtocol.ResetDeviceDirected)
                                    {
                                        SMBusCommandDecodePacket CommandCode = new SMBusCommandDecodePacket(CalcPosition((long)bytefieldinfo.DataStartIndex, _SMBClk, clkindex),
                                                                        CalcBitLenght((int)(bytefieldinfo.DataEndIndex - bytefieldinfo.DataStartIndex), _SMBClk, clkindex))
                                        {
                                            Data = new Byte[] { (Byte)bytefieldinfo.Data },
                                            _BitCount = 7,
                                            _Title = "Targeted Slave Address",
                                        };
                                        SMBusCommandDecodePacket Bit = new SMBusCommandDecodePacket(CalcPosition((long)bytefieldinfo.RwStartIndex, _SMBClk, clkindex),
                                                                CalcBitLenght((int)(bytefieldinfo.RwEndIndex - bytefieldinfo.RwStartIndex), _SMBClk, clkindex))
                                        {
                                            Data = new Byte[] { (Byte)bytefieldinfo.Rw },
                                            _BitCount = 1,
                                            _Title = "Bit",
                                        };
                                        decodepackets.Add(CommandCode);
                                        decodepackets.Add(Bit);
                                        /*数据装入事件列表*/
                                        CommandCodeFiledType.Add(bytefieldinfo.Data);
                                        CommandCodeFiledType.Add(bytefieldinfo.Rw);
                                        eventinfo.EventInofs[_EVENT_FIELD_COMMAND_CODE] = (CommandCodeFiledType.ToArray<byte>(), (uint)CommandCodeFiledType.Count() * 8);
                                        //commandstr = commandstr + "Targeted Slave Address: " + bytefieldinfo.Data.ToString("X2");
                                        //commandstr = commandstr + " Bit: " + bytefieldinfo.Rw.ToString("X1");
                                        //eventinfo.EventInofs[_EVENT_FIELD_COMMAND_CODE] = (Encoding.Default.GetBytes(commandstr), 0);
                                        //endindex = (Int32)bytefieldinfo.RwEndIndex;
                                        break;
                                    }
                                    else
                                    {
                                        SMBusCommandDecodePacket CommandCode = new SMBusCommandDecodePacket(CalcPosition((long)bytefieldinfo.DataStartIndex, _SMBClk, clkindex),
                                                                                   CalcBitLenght((int)(bytefieldinfo.DataEndIndex - bytefieldinfo.DataStartIndex), _SMBClk, clkindex))
                                        {
                                            Data = new Byte[] { (Byte)bytefieldinfo.Data },
                                            _BitCount = 8,
                                            _Title = "Command Code",
                                        };
                                        decodepackets.Add(CommandCode);
                                        /*数据装入事件列表*/
                                        byte[] CommandCodeFiled = new byte[1];
                                        CommandCodeFiled[0] = bytefieldinfo.Data;
                                        eventinfo.EventInofs[_EVENT_FIELD_COMMAND_CODE] = (CommandCodeFiled, 8);
                                        //endindex = (Int32)bytefieldinfo.DataEndIndex;
                                        break;
                                    }
                                }
                            case SMBusDataType.SmbusBlockCountFiled: //块容量字段
                                {
                                    /*数据装入图形显示*/
                                    SMBusByteCountDecodePacket ByteCount = new SMBusByteCountDecodePacket(CalcPosition((long)bytefieldinfo.DataStartIndex, _SMBClk, clkindex),
                                                                                CalcBitLenght((int)(bytefieldinfo.DataEndIndex - bytefieldinfo.DataStartIndex), _SMBClk, clkindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.Data },
                                        _BitCount = 8,
                                        _Title = "Byte Count",
                                    };
                                    decodepackets.Add(ByteCount);

                                    /*数据装入事件列表*/
                                    byte[] ByteCountFiled = new byte[1];
                                    ByteCountFiled[0] = bytefieldinfo.Data;
                                    eventinfo.EventInofs[4] = (ByteCountFiled, 8);
                                    //endindex = (Int32)bytefieldinfo.DataEndIndex;
                                    break;
                                }
                            case SMBusDataType.SmbusDataFiled: //数据字段
                                {

                                    if (smbusevent.EventProtocol == SMBusCommandProtocol.AssignAddress || smbusevent.EventProtocol == SMBusCommandProtocol.GetUdidResponse || smbusevent.EventProtocol == SMBusCommandProtocol.GetUdidDirectedResponse)
                                    {
                                        /*复制数据包再解析绘图*/
                                        ArpDataFiledType.Add(bytefieldinfo);
                                        /*复制数据再装入事件列表*/
                                        DataFiledType.Add(bytefieldinfo.Data);
                                    }
                                    else
                                    {
                                        /*数据装入图形显示*/
                                        SMBusDataDecodePacket DataByte = new SMBusDataDecodePacket(CalcPosition((long)bytefieldinfo.DataStartIndex, _SMBClk, clkindex),
                                                                    CalcBitLenght((int)(bytefieldinfo.DataEndIndex - bytefieldinfo.DataStartIndex), _SMBClk, clkindex))
                                        {
                                            Data = new Byte[] { (Byte)bytefieldinfo.Data },
                                            _BitCount = 8,
                                            _Title = "Data",
                                        };
                                        decodepackets.Add(DataByte);
                                        //*复制数据再装入事件列表*/
                                        DataFiledType.Add(bytefieldinfo.Data);
                                        //endindex = (Int32)bytefieldinfo.DataEndIndex;
                                    }
                                    break;
                                }
                            case SMBusDataType.SmbusPecFiled: //地址字段_读写字段
                                {
                                    /*数据装入图形显示*/
                                    SMBusPecDecodePacket Pec = new SMBusPecDecodePacket(CalcPosition((long)bytefieldinfo.DataStartIndex, _SMBClk, clkindex),
                                                                                                    CalcBitLenght((int)(bytefieldinfo.DataEndIndex - bytefieldinfo.DataStartIndex), _SMBClk, clkindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.Data },
                                        _BitCount = 8,
                                        _Title = "PEC",
                                        PecError = (smbusevent.SmbusErrorInfo.HasCrcError == 1) ? true : false,
                                        _PecNote = (smbusevent.SmbusErrorInfo.HasCrcError == 1) ? "Error:CRC,calculated " + smbusevent.SmbusErrorInfo.CrcErrorValue.ToString("X2") + "h" : String.Empty,
                                    };
                                    decodepackets.Add(Pec);

                                    /*数据装入事件列表*/
                                    byte[] PecFiled = new byte[1];
                                    PecFiled[0] = bytefieldinfo.Data;
                                    eventinfo.EventInofs[_EVENT_FIELD_PEC] = (PecFiled, 8);
                                    break;

                                }
                        }
                    }

                    /*ACK应答段装入事件列表*/
                    eventinfo.EventInofs[_EVENT_FIELD_ACK] = (AckFiledType.ToArray(), (uint)AckFiledType.Count() * 8);
                    //eventinfo.EventInofs[_EVENT_FIELD_ACK] = (Encoding.Default.GetBytes(ackstr), 0);

                    /*data数据段装入事件列表*/
                    for (Int32 j = 0; j < smbusevent.DataInfoSize; j++)
                    {
                        SMBusDataInfo bytefieldinfo = (SMBusDataInfo)Marshal.PtrToStructure(smbusevent.DataInfoPtr + j * byteinfosize, typeof(SMBusDataInfo));
                        if (bytefieldinfo.Type == SMBusDataType.SmbusDataFiled)
                        {
                            eventinfo.EventInofs[_EVENT_FIELD_DATA_LSB_MSB] = (DataFiledType.ToArray<byte>(), (uint)DataFiledType.Count() * 8);
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //step4.判断类型ARP 3类特殊命令绘图
                    if (smbusevent.EventProtocol == SMBusCommandProtocol.AssignAddress || smbusevent.EventProtocol == SMBusCommandProtocol.GetUdidResponse || smbusevent.EventProtocol == SMBusCommandProtocol.GetUdidDirectedResponse)
                    {
                        if (smbusevent.ArpBytesPtr != IntPtr.Zero)
                        {
                            //endindex = (Int32)ArpDataFiledType[16].DataEndIndex;
                            for (Int32 j = 0; j < smbusevent.ArpBytesSize; j++)
                            {
                                //从内存地址 smbusevent.ArpBytesPtr + j * arpinfosize 读取数据，并将其解析为 SMBusArpDataInfo 结构体
                                SMBusArpDataInfo arpfieldinfo = (SMBusArpDataInfo)Marshal.PtrToStructure(smbusevent.ArpBytesPtr + j * arpinfosize, typeof(SMBusArpDataInfo));
                                switch (arpfieldinfo.ArpType)
                                {
                                    case SMBusArpFiledType.DeviceCapabilities:
                                        {
                                            /*数据装入图形显示*/
                                            SMBusDataDecodePacket DataByte = new SMBusDataDecodePacket(CalcPosition((long)ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex, _SMBClk, clkindex),
                                                                        CalcBitLenght((int)(ArpDataFiledType[arpfieldinfo.byte_end_index].DataEndIndex - ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex), _SMBClk, clkindex))
                                            {
                                                Data = new Byte[] { (Byte)ArpDataFiledType[0].Data }, //第1字节
                                                _BitCount = 8,
                                                _Title = "Device Capabilities",

                                            };
                                            decodepackets.Add(DataByte);
                                            break;
                                        }
                                    case SMBusArpFiledType.VersionRevision:
                                        {
                                            /*数据装入图形显示*/
                                            SMBusDataDecodePacket DataByte = new SMBusDataDecodePacket(CalcPosition((long)ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex, _SMBClk, clkindex),
                                                                        CalcBitLenght((int)(ArpDataFiledType[arpfieldinfo.byte_end_index].DataEndIndex - ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex), _SMBClk, clkindex))
                                            {
                                                Data = new Byte[] { (Byte)ArpDataFiledType[1].Data }, //第2字节
                                                _BitCount = 8,
                                                _Title = "Version/Revision", //第2字节
                                            };
                                            decodepackets.Add(DataByte);
                                            break;
                                        }
                                    case SMBusArpFiledType.VendorId:
                                        {
                                            /*数据装入图形显示*/
                                            SMBusDataDecodePacket DataByte = new SMBusDataDecodePacket(CalcPosition((long)ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex, _SMBClk, clkindex),
                                                                        CalcBitLenght((int)(ArpDataFiledType[arpfieldinfo.byte_end_index].DataEndIndex - ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex), _SMBClk, clkindex))
                                            {
                                                Data = new Byte[] { (Byte)ArpDataFiledType[2].Data, (Byte)ArpDataFiledType[3].Data },//第3-4字节
                                                _BitCount = 16,
                                                _Title = "Vendor ID",
                                            };
                                            decodepackets.Add(DataByte);
                                            break;
                                        }
                                    case SMBusArpFiledType.DeviceId:
                                        {
                                            /*数据装入图形显示*/
                                            SMBusDataDecodePacket DataByte = new SMBusDataDecodePacket(CalcPosition((long)ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex, _SMBClk, clkindex),
                                                                        CalcBitLenght((int)(ArpDataFiledType[arpfieldinfo.byte_end_index].DataEndIndex - ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex), _SMBClk, clkindex))
                                            {
                                                Data = new Byte[] { (Byte)ArpDataFiledType[4].Data, (Byte)ArpDataFiledType[5].Data }, //第5-6字节
                                                _BitCount = 16,
                                                _Title = "Device ID",
                                            };
                                            decodepackets.Add(DataByte);
                                            break;
                                        }
                                    case SMBusArpFiledType.Interface:
                                        {
                                            /*数据装入图形显示*/
                                            SMBusDataDecodePacket DataByte = new SMBusDataDecodePacket(CalcPosition((long)ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex, _SMBClk, clkindex),
                                                                        CalcBitLenght((int)(ArpDataFiledType[arpfieldinfo.byte_end_index].DataEndIndex - ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex), _SMBClk, clkindex))
                                            {
                                                Data = new Byte[] { (Byte)ArpDataFiledType[6].Data, (Byte)ArpDataFiledType[7].Data }, //第7-8字节
                                                _BitCount = 16,
                                                _Title = "Interface",
                                            };
                                            decodepackets.Add(DataByte);
                                            break;
                                        }
                                    case SMBusArpFiledType.SubsystemVendorId:
                                        {
                                            /*数据装入图形显示*/
                                            SMBusDataDecodePacket DataByte = new SMBusDataDecodePacket(CalcPosition((long)ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex, _SMBClk, clkindex),
                                                                        CalcBitLenght((int)(ArpDataFiledType[arpfieldinfo.byte_end_index].DataEndIndex - ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex), _SMBClk, clkindex))
                                            {
                                                Data = new Byte[] { (Byte)ArpDataFiledType[8].Data, (Byte)ArpDataFiledType[9].Data },//第9-10字节
                                                _BitCount = 16,
                                                _Title = "Subsystem Vendor ID",
                                            };
                                            decodepackets.Add(DataByte);
                                            break;
                                        }
                                    case SMBusArpFiledType.SubsystemDeviceId:
                                        {
                                            /*数据装入图形显示*/
                                            SMBusDataDecodePacket DataByte = new SMBusDataDecodePacket(CalcPosition((long)ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex, _SMBClk, clkindex),
                                                                        CalcBitLenght((int)(ArpDataFiledType[arpfieldinfo.byte_end_index].DataEndIndex - ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex), _SMBClk, clkindex))
                                            {
                                                Data = new Byte[] { (Byte)ArpDataFiledType[10].Data, (Byte)ArpDataFiledType[11].Data },//第11-12字节
                                                _BitCount = 16,
                                                _Title = "Subsystem Device ID",
                                            };
                                            decodepackets.Add(DataByte);
                                            break;
                                        }
                                    case SMBusArpFiledType.VendorSpecificId:
                                        {

                                            /*数据装入图形显示*/
                                            SMBusDataDecodePacket DataByte = new SMBusDataDecodePacket(CalcPosition((long)ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex, _SMBClk, clkindex),
                                                                        CalcBitLenght((int)(ArpDataFiledType[arpfieldinfo.byte_end_index].DataEndIndex - ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex), _SMBClk, clkindex))
                                            {
                                                Data = new Byte[] { (Byte)ArpDataFiledType[12].Data, (Byte)ArpDataFiledType[13].Data, (Byte)ArpDataFiledType[14].Data, (Byte)ArpDataFiledType[15].Data }, //第13-16字节
                                                _BitCount = 32,
                                                _Title = "Vendor Specific ID",
                                            };
                                            decodepackets.Add(DataByte);
                                            break;
                                        }
                                    case SMBusArpFiledType.DeviceSlaveAddress:
                                        {
                                            if (smbusevent.EventProtocol == SMBusCommandProtocol.AssignAddress)
                                            {
                                                /*数据装入图形显示*/
                                                SMBusDataDecodePacket DataByte = new SMBusDataDecodePacket(CalcPosition((long)ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex, _SMBClk, clkindex),
                                                                            CalcBitLenght((int)(ArpDataFiledType[arpfieldinfo.byte_end_index].DataEndIndex - ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex), _SMBClk, clkindex))
                                                {
                                                    Data = new Byte[] { (Byte)ArpDataFiledType[16].Data },//第17字节
                                                    _BitCount = 8,
                                                    _Title = "Assigned Address",
                                                };
                                                decodepackets.Add(DataByte);
                                                break;
                                            }
                                            else
                                            {
                                                /*数据装入图形显示*/
                                                SMBusDataDecodePacket DataByte = new SMBusDataDecodePacket(CalcPosition((long)ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex, _SMBClk, clkindex),
                                                                            CalcBitLenght((int)(ArpDataFiledType[arpfieldinfo.byte_end_index].DataEndIndex - ArpDataFiledType[arpfieldinfo.byte_start_index].DataStartIndex), _SMBClk, clkindex))
                                                {
                                                    Data = new Byte[] { (Byte)ArpDataFiledType[16].Data },//第17字节
                                                    _BitCount = 8,
                                                    _Title = "Device Slave Address",
                                                };
                                                decodepackets.Add(DataByte);
                                                break;
                                            }
                                        }
                                }
                            }
                        }
                    }
                }

                // step3. 错误帧 装入事件列表
                String str = "";

                if (smbusevent.DataInfoPtr != IntPtr.Zero)
                {
                    // ACK错误
                    if (smbusevent.SmbusErrorInfo.HasAckError == 1)
                    {
                        if (smbusevent.SmbusErrorInfo.AckErrorCount <= 1)
                        {
                            str = str + "ACK: unexpected NACK; "; // ACK错误
                        }
                        else
                        {
                            str = str + "ACK: unexpected NACK(" + smbusevent.SmbusErrorInfo.AckErrorCount.ToString() + ");";//ACK错误个数超过1个
                        }

                    }
                    // NACK错误
                    if (smbusevent.SmbusErrorInfo.HasNackError == 1)
                    {
                        str = str + "NACK: unexpected ACK ;";

                    }
                    // CRC错误
                    if (smbusevent.SmbusErrorInfo.HasCrcError == 1)
                    {
                        str = str + "PEC:CRC,Calculated " + smbusevent.SmbusErrorInfo.CrcErrorValue.ToString("X2") + "h;";

                    }
                    if(smbusevent.SmbusErrorInfo.HasAckError == 1 || smbusevent.SmbusErrorInfo.HasNackError == 1 || smbusevent.SmbusErrorInfo.HasCrcError == 1)
                    {
                         eventinfo.EventInofs[_EVENT_FIELD_ERROR] = (Encoding.Default.GetBytes(str), 0);
                    }
                }
                else
                {
                    str = str + "Unknown Address "; // 空包
                    eventinfo.EventInofs[_EVENT_FIELD_ERROR] = (Encoding.Default.GetBytes(str), 0);
                }

                // step4. 结束帧
                if (smbusevent.HasEventRestartEnd == 1 && smbusevent.HasEventStopEnd == 1)
                {
                    //stop位置
                    SMBusRestartDecodePacket restartendpacket = new SMBusRestartDecodePacket(CalcPosition((int)smbusevent.EventEndIndex, _SMBClk, clkindex), 1)
                    {
                        _Title = "Restart",
                    };
                    decodepackets.Add(restartendpacket);
                    eventinfo.EndPosition = CalcPosition((long)smbusevent.EventEndIndex, _SMBClk, clkindex);
                    eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, clkindex);
                }
                else if (smbusevent.HasEventRestartEnd == 0 && smbusevent.HasEventStopEnd == 1)
                {
                    //stop位置
                    SMBusStopDecodePacket stopendpacket = new SMBusStopDecodePacket(CalcPosition((int)smbusevent.EventEndIndex, _SMBClk, clkindex), 1)
                    {
                        _Title = "Stop",
                    };
                    decodepackets.Add(stopendpacket);
                    eventinfo.EndPosition = CalcPosition((long)smbusevent.EventEndIndex, _SMBClk, clkindex);
                    eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, clkindex);
                }
                else if (smbusevent.HasEventRestartEnd == 1)//空包
                {
                    //stop位置
                    SMBusRestartDecodePacket restartendpacket = new SMBusRestartDecodePacket(CalcPosition((int)smbusevent.EventEndIndex, _SMBClk, clkindex), 1)
                    {
                        _Title = "Restart",
                    };
                    decodepackets.Add(restartendpacket);
                    eventinfo.EndPosition = CalcPosition((long)smbusevent.EventEndIndex, _SMBClk, clkindex);
                    eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, clkindex);
                }
                else if (smbusevent.HasEventStopEnd == 1)//空包
                {
                    //stop位置
                    SMBusStopDecodePacket stopendpacket = new SMBusStopDecodePacket(CalcPosition((int)smbusevent.EventEndIndex, _SMBClk, clkindex), 1)
                    {
                        _Title = "Stop",
                    };
                    decodepackets.Add(stopendpacket);
                    eventinfo.EndPosition = CalcPosition((long)smbusevent.EventEndIndex, _SMBClk, clkindex);
                    eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, clkindex);
                }

                // 添加事件信息
                _EventInfos.Add(eventinfo);

            }
            // 释放解码结果资源
            DecoderImpl.FreeSMBus(ref results);

            _ResultData.DecodeViewInfos = decodepackets.ToArray();
            decoderesults.Add(_ResultData);
        }

        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolSMBusOptions()
            {
                CLKThreshold = _CLKThreshold,
                DataThreshold = _DataThreshold,
                SMBClK = SMBClk,
                SMBData = SMBData,
                PECByte = PECByte,
            };
        }
    }
}

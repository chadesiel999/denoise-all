using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NPOI.SS.Formula.PTG;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using static ScopeX.Core.Decode.EdgeInfoCPP;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using ScopeX.Hardware.Driver;
using ScopeX.Core.Decode;
using System.Collections;
using System.Numerics;
using System.Xml.Linq;

namespace ScopeX.Core.Decode
{
    ///// <summary>
    /// I2C协议解码的Model类
    /// </summary>
    internal sealed class I2CDecodeModel : ProtocolModel
    {
        private DecodeResultData _ResultData = new DecodeResultData();
        private List<PAM2EdgePulse> _ClkPulsesList = new List<PAM2EdgePulse>();
        private List<PAM2EdgePulse> _DataPulsesList = new List<PAM2EdgePulse>();

        public I2CDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id, SerialProtocolType.I2C, isTrigDecode)
        {
            _ResultData.Name = "I2C";
            RefreashThresholds();
        }


        public override IReadOnlyList<String> EventInfoTitles { get; } = (new List<String>()
        {
            "Index",
            "Start Time",
            "Addr",
            "Data",
            "Ack",
            "Error",
        }).AsReadOnly();

        private ChannelId _SCLK = ChannelId.C2;
        /// <summary>
        /// 时钟源
        /// </summary>
        public ChannelId SCLK
        {
            get { return _SCLK; }
            set { UpdateProperty(ref _SCLK, value); }
        }

        private ChannelId _SDA = ChannelId.C1;
        /// <summary>
        /// 数据源
        /// </summary>
        public ChannelId SDA
        {
            get { return _SDA; }
            set
            {
                if (_SDA != value)
                {
                    UpdateProperty(ref _SDA, value);
                }
            }
        }

        private Double _MinThresholdSDA; 
        private  Double _MinThresholdSCLK ;
        private  Double _MaxThresholdSDA;
        private  Double _MaxThresholdSCLK;
        private Double _SCLKThreshold;
        /// <summary>
        /// 时钟源的阈值
        /// </summary>
        public Double SCLKThreshold
        {
            get { return _SCLKThreshold * TryGetChannelGain(SCLK); }
            set { UpdateProperty(ref _SCLKThreshold, value / TryGetChannelGain(SCLK)); }
        }

        public String SCLKUnit => GetChannelUnit(SCLK);

        private Double _SDAThreshold = 1;
        /// <summary>
        /// 数据源的阈值
        /// </summary>
        public Double SDAThreshold
        {
            get { return _SDAThreshold * TryGetChannelGain(SDA); }
            set { UpdateProperty(ref _SDAThreshold, value / TryGetChannelGain(SDA)); }
        }
        public String SDAUnit => GetChannelUnit(SDA);

        private ProtocolI2C.AddrBitWidth _BitWidth = ProtocolI2C.AddrBitWidth.AddrBitWidth_7;

        public ProtocolI2C.AddrBitWidth BitWidth
        {
            get { return _BitWidth; }
            set { UpdateProperty(ref _BitWidth, value); }
        }

        private void RefreashThresholds()
        {
            Double defaultVal = (float)(8 * TryGetChannelGain(_SCLK));
            _MaxThresholdSCLK = defaultVal;
            _MinThresholdSCLK = -defaultVal;
            defaultVal = (float)(8 * TryGetChannelGain(_SDA));
            _MaxThresholdSDA = defaultVal;
            _MinThresholdSDA = -defaultVal;
            //return;
            if (!GetDynamicThresholdRange(_SCLK, out Double chnlMaxByMv, out Double chnlMinByMv))
            {
                return;
            }
            _MaxThresholdSCLK = chnlMaxByMv / 1000;
            _MinThresholdSCLK = chnlMinByMv / 1000;
            if (!GetDynamicThresholdRange(_SDA, out chnlMaxByMv, out chnlMinByMv))
            {
                return;
            }
            _MaxThresholdSDA = chnlMaxByMv / 1000;
            _MinThresholdSDA = chnlMinByMv / 1000;
        }

        public Double MinThresholdSDA { get => _MinThresholdSDA; }
        public Double MinThresholdSCL { get => _MinThresholdSCLK; }
        public Double MaxThresholdSDA { get => _MaxThresholdSDA; }
        public Double MaxThresholdSCL { get => _MaxThresholdSCLK; }

        // 事件字段index
        private const int _EVENT_FIELD_ADDRESS = 0; // 地址字段
        private const int _EVENT_FIELD_DATA    = 1; // 数据字段
        private const int _EVENT_FIELD_ACK     = 2; // ACK应答字段
        private const int _EVENT_FIELD_ERROR   = 3; // 错误字段
     
  
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
            if (_SDA.IsReference() && DecodeDataSource.Instance.ReferenceDataSource[_SDA - ChannelIdExt.MinRChId].Channels != null
                && DecodeDataSource.Instance.ReferenceDataSource[_SDA - ChannelIdExt.MinRChId].Channels[0] == _SDA)
            {
                DecodeDataSource.Instance.ReferenceDataSource[_SDA - ChannelIdExt.MinRChId].HasData = false;
            }
        }
        //判断是否有数据
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
                clk = DecodeDataHelper.ReferenceHasData(_SCLK, _SCLKThreshold);
            }

            if (_SCLK.IsAnalog())
            {
                clk = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }


            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(_SDA, out IChnlPrsnt? prsnt);
            if (prsnt == null)
                return false;

            if (_SDA.IsReference() && prsnt.VuDatabase.Current != null)
            {
                data = DecodeDataHelper.ReferenceHasData(_SDA, _SDAThreshold);
            }

            if (_SDA.IsAnalog())
            {
                data = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
            }
            return (data || clk);
        }
        //数据处理
        internal override void ParsingData(ref CancellationToken token)
        {
            RefreashThresholds();  // ycf
            Int32 clkindex = GetChIndex(_SCLK);
            Int32 dataindex = GetChIndex(_SDA);

            UInt32 clklen = 0, datalen = 0;

            Double clsamplerate = 0, datasamplerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(BusId, _SCLK, ref clsamplerate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, _SCLK, ref clklen);

            DecodeDataHelper.Instance.TryGetSampleRate(BusId, _SDA, ref datasamplerate);
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, _SDA, ref datalen);

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
            I2cOption option = new I2cOption()
            {
                DataBitWidthLen = (I2cDataBitWidth)(BitWidth),
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
            TwoLevelEdgeInfo? datanode = DecodeDataHelper.Instance.GetEdgeInfo(BusId, startindex: 0, _SDA, ref token, ref needclear) as TwoLevelEdgeInfo;
            if (datanode == null)
            {
                return;
            }
            _DataPulsesList.Clear();

            DecodeDataHelper.Instance.GetTwoLevelPulses(ref datanode, ref _DataPulsesList);
            PAM2EdgePulseSequence.Allocate(ref _DataPulsesList, (UInt64)datalen, datasamplerate, out dataptr, out datahandle);

            /*清除界面*/
            List<I2CDecodePacket> decodepackets = new List<I2CDecodePacket>();
            //开始解码
            I2cResult results;
            results.EventCount = 0;
            results.Event = IntPtr.Zero;

            // 开始解码
            if (!DecoderImpl.DecodeI2c(ref option, clkptr, dataptr, out results))
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
            Int32 eventsize   = Marshal.SizeOf(typeof(I2cEvent));//Marshal 与 C/C++ 交互 在托管代码中操作非托管数据结构（如 struct 或 byte[]）
            Int32 byteinfosize = Marshal.SizeOf(typeof(I2cDataInfo));

            // 默认事件显示
            String temp_info = "--";

            // event count
            for (Int32 i = 0; i < results.EventCount; i++)
            {
                ProtocolEventInfo eventinfo = new ProtocolEventInfo();

                eventinfo.Index = _EventInfos.Count;
                eventinfo.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));

                /*将事件指针数据转换为I2cEvent结构体，提取事件的起始位置和时间。*/
                I2cEvent i2cevent = (I2cEvent)Marshal.PtrToStructure(results.Event + i * eventsize, typeof(I2cEvent));

                eventinfo.StartTimeByPs = base.GetTimeFromPosition(i2cevent.EventStartIndex, clkindex);
                eventinfo.StartPosition = CalcPosition((Int64)i2cevent.EventStartIndex, _SCLK, clkindex);

                /*先将事件表用“--”填充*/
                //eventinfo.EventInofs[0]  = (Encoding.Default.GetBytes(temp_info), 0);   // Index
                //eventinfo.EventInofs[1]  = (Encoding.Default.GetBytes(temp_info), 0);   // Start Time
                eventinfo.EventInofs[_EVENT_FIELD_ADDRESS] = (Encoding.Default.GetBytes(temp_info), 0);     // Address
                eventinfo.EventInofs[_EVENT_FIELD_DATA]    = (Encoding.Default.GetBytes(temp_info), 0);     // Data
                eventinfo.EventInofs[_EVENT_FIELD_ACK]     = (Encoding.Default.GetBytes(temp_info), 0);     // Ack
                eventinfo.EventInofs[_EVENT_FIELD_ERROR]   = (Encoding.Default.GetBytes(temp_info), 0);     // Error

                // step1. start帧 位置
                I2CStartDecodePacket startpacket = new I2CStartDecodePacket(CalcPosition((int)i2cevent.EventStartIndex, _SDA, clkindex), 1)
                {
                    _Title = "Start",
                };
                decodepackets.Add(startpacket);

                // step2. 数据帧
                if (i2cevent.DataInfoPtr != IntPtr.Zero)
                {
                    Int32  datafilednum = 0;
                    Int32  addrfilednum = 0;
                    UInt32 addrbitwidth = 0;
                    String readwritestr = "";
                    //ACK字段 Data字段 
                    List<byte> AckFiledType  = new List<byte>();
                    List<byte> DataFiledType = new List<byte>();
                    List<byte> AddrFiledType = new List<byte>();

                    //step3.根据Event中的DataInfoSize，把每个byte都取出来判断类型
                    for (Int32 j = 0; j < i2cevent.DataInfoSize; j++)
                    {
                        I2cDataInfo bytefieldinfo = (I2cDataInfo)Marshal.PtrToStructure(i2cevent.DataInfoPtr + j * byteinfosize, typeof(I2cDataInfo));
                        //ACK字段
                        AckFiledType.Add(bytefieldinfo.Ack);
                        
                        //ACK NACK字段绘图 有错误时绘制
                        if ( (bytefieldinfo.AckError == 1) && (j == i2cevent.DataInfoSize - 1) && (i2cevent.ErrorInfo.ErrorNackSize == 1))//NACK错误
                        {
                            /*数据装入图形显示*/
                            I2CACKDecodePacket AckErrorFiled = new I2CACKDecodePacket(CalcPosition((long)bytefieldinfo.AckStartIndex, _SCLK, clkindex),
                                                                        CalcBitLenght((int)(bytefieldinfo.AckEndIndex - bytefieldinfo.AckStartIndex), _SCLK, clkindex))
                            {
                                /*显示数据*/
                                Data = new Byte[] { (Byte)bytefieldinfo.Ack },
                                /*显示位数*/
                                _BitCount = 1,
                                /*显示标题*/
                                _Title = "NaK",
                                /*错误显示*/
                                AckError = true,
                                _AckNote = "Error: Unexpected ACK ",
                            };
                            decodepackets.Add(AckErrorFiled);
                        }

                        /*判断字段类型*/
                        switch (bytefieldinfo.Type)
                        {
                            //地址字段_读写字段
                            case I2cDataType.I2cAddr7BitFiled :
                            case I2cDataType.I2cAddr10BitFiled:
                            {
                                    addrfilednum++;
                                    addrbitwidth = ( addrfilednum == 1) ? (UInt32)7 : (UInt32)8;
                                   
                                    //*复制数据再装入事件列表*/
                                    AddrFiledType.Add(bytefieldinfo.Data);
                                    
                                    /*数据装入图形显示*/
                                    I2CAddrDecodePacket Address = new I2CAddrDecodePacket(CalcPosition((long)bytefieldinfo.DataStartIndex, _SCLK, clkindex),
                                                                        CalcBitLenght((int)(bytefieldinfo.DataEndIndex - bytefieldinfo.DataStartIndex), _SCLK, clkindex))
                                    {
                                        Data = new Byte[] { (Byte)bytefieldinfo.Data },
                                        _BitCount = addrbitwidth,
                                        _Title = "Addr",
                                    };
                                    decodepackets.Add(Address);
                                    // RW 
                                    if (addrfilednum == 1)
                                    {
                                        readwritestr = ( bytefieldinfo.Rw == 1 )? "Read" :"Write";
                                        /*数据装入图形显示*/
                                        I2CRWDecodePacket WR = new I2CRWDecodePacket(CalcPosition((long)bytefieldinfo.RwStartIndex, _SCLK, clkindex),
                                                                    CalcBitLenght((int)(bytefieldinfo.RwEndIndex - bytefieldinfo.RwStartIndex), _SCLK, clkindex))
                                        {
                                            RW = bytefieldinfo.Rw,
                                        };
                                        decodepackets.Add(WR);
                                    }
                                    // ACK
                                    if ((bytefieldinfo.AckError == 1) && (j != (i2cevent.DataInfoSize - 1)))
                                    {
                                        /*数据装入图形显示*/
                                        I2CACKDecodePacket AckErrorFiled = new I2CACKDecodePacket(CalcPosition((long)bytefieldinfo.AckStartIndex, _SCLK, clkindex),
                                                                                    CalcBitLenght((int)(bytefieldinfo.AckEndIndex - bytefieldinfo.AckStartIndex), _SCLK, clkindex))
                                        {
                                            /*显示数据*/
                                            Data = new Byte[] { (Byte)bytefieldinfo.Ack },
                                            /*显示位数*/
                                            _BitCount = 1,
                                            /*显示标题*/
                                            _Title = "ACK",
                                            /*错误显示*/
                                            AckError = true,
                                            _AckNote = "Error: Unexpected NaK on Addr ",
                                        };
                                        decodepackets.Add(AckErrorFiled);
                                     }
                                    break;
                            }
                            case I2cDataType.I2cDataFiled: //数据字段
                            {
                                datafilednum++;
                               // Data
                                    /*数据装入图形显示*/
                                I2CDataDecodePacket DataByte = new I2CDataDecodePacket(CalcPosition((long)bytefieldinfo.DataStartIndex, _SCLK, clkindex),
                                                            CalcBitLenght((int)(bytefieldinfo.DataEndIndex - bytefieldinfo.DataStartIndex), _SCLK, clkindex))
                                {
                                    Data = new Byte[] { (Byte)bytefieldinfo.Data },
                                    _BitCount = 8,
                                    _Title = "Data",
                                };
                                decodepackets.Add(DataByte);
                                //*复制数据再装入事件列表*/
                                DataFiledType.Add(bytefieldinfo.Data);
                                // ACK
                                if (bytefieldinfo.AckError == 1 && (j != (i2cevent.DataInfoSize - 1)))
                                {
                                    /*数据装入图形显示*/
                                    I2CACKDecodePacket AckErrorFiled = new I2CACKDecodePacket(CalcPosition((long)bytefieldinfo.AckStartIndex, _SCLK, clkindex),
                                                                                CalcBitLenght((int)(bytefieldinfo.AckEndIndex - bytefieldinfo.AckStartIndex), _SCLK, clkindex))
                                    {
                                        /*显示数据*/
                                        Data = new Byte[] { (Byte)bytefieldinfo.Ack },
                                        /*显示位数*/
                                        _BitCount = 1,
                                        /*显示标题*/
                                        _Title = "ACK",
                                        /*错误显示*/
                                        AckError = true,
                                        _AckNote = "Error: Unexpected NaK on Data ",
                                    };
                                    decodepackets.Add(AckErrorFiled);
                                }
                                break;
                            }
                        }
                    }

                    /*ACK应答段装入事件列表*/
                    eventinfo.EventInofs[_EVENT_FIELD_ACK] = (AckFiledType.ToArray(), (uint)AckFiledType.Count() * 8);

                    /* addr data数据段装入事件列表*/
                    eventinfo.EventInofs[_EVENT_FIELD_ADDRESS] = (AddrFiledType.ToArray<byte>(), (uint)AddrFiledType.Count() * 8);
                   
                    if (datafilednum > 0)
                    {
                        eventinfo.EventInofs[_EVENT_FIELD_DATA] = (DataFiledType.ToArray<byte>(), (uint)DataFiledType.Count() * 8);
                    }

                }

                // step3. 错误帧 装入事件列表
                String str = "";
                if (i2cevent.DataInfoPtr == IntPtr.Zero)
                {
                    str = str + "Unknown Address"; // 空包
                    eventinfo.EventInofs[_EVENT_FIELD_ERROR] = (Encoding.Default.GetBytes(str), 0);

                }
                else
                {   
                    // Addr ACK错误
                    if (i2cevent.ErrorInfo.ErrorAddrAckSize > 0 )
                    {
                        if (i2cevent.ErrorInfo.ErrorAddrAckSize <= 1)
                        {
                            str = str + "ACK: unexpected NaK on Addr; "; // ACK错误
                        }
                        else
                        {
                            str = str + "ACK: unexpected NaK(" + i2cevent.ErrorInfo.ErrorAddrAckSize.ToString() + ")on Addr;";//ACK错误个数超过1个
                        }

                    }
                    // Data ACK错误
                    if (i2cevent.ErrorInfo.ErrorDataAckSize > 0)
                    {
                        if (i2cevent.ErrorInfo.ErrorDataAckSize <= 1)
                        {
                            str = str + "ACK: unexpected NaK on Data; "; // ACK错误
                        }
                        else
                        {
                            str = str + "ACK: unexpected NaK(" + i2cevent.ErrorInfo.ErrorDataAckSize.ToString() + ") on Data;";//ACK错误个数超过1个
                        }

                    }
                    // NACK错误
                    if (i2cevent.ErrorInfo.ErrorNackSize == 1)
                    {
                        str = str + "NaK: unexpected ACK ;"; 

                    }
                    if (i2cevent.ErrorInfo.ErrorAddrAckSize >= 1 || i2cevent.ErrorInfo.ErrorDataAckSize >= 1 || i2cevent.ErrorInfo.ErrorNackSize == 1)
                    {
                        eventinfo.EventInofs[_EVENT_FIELD_ERROR] = (Encoding.Default.GetBytes(str), 0);
                    }
                }

                // step4. 结束帧
                if (i2cevent.HasEventRestartEnd == 1 && i2cevent.HasEventStopEnd == 1)
                {
                    //stop位置
                    I2CStartDecodePacket restartendpacket = new I2CStartDecodePacket(CalcPosition((int)i2cevent.EventEndIndex, _SCLK, clkindex), 1)
                    {
                        _Title = "Start",
                    };
                    decodepackets.Add(restartendpacket);
                    eventinfo.EndPosition = CalcPosition((long)i2cevent.EventEndIndex, _SCLK, clkindex);
                    eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, clkindex);
                }
                else if (i2cevent.HasEventRestartEnd == 0 && i2cevent.HasEventStopEnd == 1)
                {
                    //stop位置
                    I2CStopDecodePacket stopendpacket = new I2CStopDecodePacket(CalcPosition((int)i2cevent.EventEndIndex, _SCLK, clkindex), 1)
                    {
                        _Title = "Stop",
                    };
                    decodepackets.Add(stopendpacket);
                    eventinfo.EndPosition = CalcPosition((long)i2cevent.EventEndIndex, _SCLK, clkindex);
                    eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, clkindex);
                }
                else if (i2cevent.HasEventRestartEnd == 1)//空包
                {
                    //stop位置
                    I2CStartDecodePacket restartendpacket = new I2CStartDecodePacket(CalcPosition((int)i2cevent.EventEndIndex, _SCLK, clkindex), 1)
                    {
                        _Title = "Start",
                    };
                    decodepackets.Add(restartendpacket);
                    eventinfo.EndPosition = CalcPosition((long)i2cevent.EventEndIndex, _SCLK, clkindex);
                    eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, clkindex);
                }
                else if (i2cevent.HasEventStopEnd == 1)//空包
                {
                    //stop位置
                    I2CStopDecodePacket stopendpacket = new I2CStopDecodePacket(CalcPosition((int)i2cevent.EventEndIndex, _SCLK, clkindex), 1)
                    {
                        _Title = "Stop",
                    };
                    decodepackets.Add(stopendpacket);
                    eventinfo.EndPosition = CalcPosition((long)i2cevent.EventEndIndex, _SCLK, clkindex);
                    eventinfo.EndTimeByPs = GetTimeFromPosition(eventinfo.EndPosition, clkindex);
                }
                
                // 添加事件信息
                _EventInfos.Add(eventinfo);

            }
            // 释放解码结果资源
            DecoderImpl.FreeI2c(ref results);

            _ResultData.DecodeViewInfos = decodepackets.ToArray();
            decoderesults.Add(_ResultData);

        }

        public override HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return new HdMessage.ProtocolI2COptions()
            {
                //SCLKThreshold  = _CLKThreshold,
                //SDAThreshold   = _DataThreshold,
                //SDA = SDA,
                //SCLK = SCLK,
                //BitWidth = BitWidth,
                SCLK = SCLK,
                SCLKThreshold = _SCLKThreshold,
                SDA = SDA,
                SDAThreshold = _SDAThreshold,
                BitWidth = BitWidth
            };
        }
    }
}

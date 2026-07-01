using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ScopeX.ComModel;
using static ScopeX.Core.Decode.DecoderTypes;
using static ScopeX.Core.Decode.EdgeInfoCPP;

namespace ScopeX.Core.Decode;
[Obsolete("Please Use Class 'USBDecodeModelCPP'", true)]
sealed internal class USBDecodeModel : ProtocolModel
{
    Boolean _AutoClock;
    const Double TOLERANCE = 0.00001;
    DecodeResultData _ResultData = new();
    ProtocolUSB.SignalRate _SignalRate = ProtocolUSB.SignalRate.FullRate;
    DiffSignalType _SignalType;

    ChannelId _Source1 = ChannelId.C1;
    Single _Source1Threshold = 1.81f;
    ChannelId _Source2 = ChannelId.C2;
    Single _Source2Threshold = 1.81f;

    //public UsbDecodeResult decodeResult = new();
    //private Int32 _tmpEdgeStartIndex = 0;
    public Double SamplingFrequency;
	
	public USBDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.USB, isTrigDecode)
    {
    }


    public override Double BitRateByPs => (1f / SignalRate switch
    {
        ProtocolUSB.SignalRate.LowRate => 1.5E6,
        ProtocolUSB.SignalRate.FullRate => 12E6,
        ProtocolUSB.SignalRate.HighRate => 480E6,
        _ => 1.5E6
    }) * 1E12;


    public override IReadOnlyList<String> EventInfoTitles
    {
        get
        {
            List<String> temp = new()
            {
                "Index",
                "Start Time",
                //"Sync",
                "Packet",
                "Data",
                "Addr",
                "EndPoint",
                "CRC5",
                "CRC16",
                "EOP",
                "Error"
            };
            return temp.AsReadOnly();
        }
    }
    public ProtocolUSB.SignalRate SignalRate
    {
        get => _SignalRate;
        set => UpdateProperty(ref _SignalRate, value);
    }

    public DiffSignalType SignalType
    {
        get => _SignalType;
        set
        {
            if (value != _SignalType)
            {
                UpdateProperty(ref _SignalType, value);
                if (_Source1.IsReference())
                {
                    DecodeDataHelper.UpdaterReferenceDataByRange(_Source1, _Source1Threshold);
                    _NeedDecodeData = true;
                }
            }

        }

    }
    public Boolean AutoClock
    {
        get => _AutoClock;
        set
        {
            if (value != _AutoClock)
            {
                UpdateProperty(ref _AutoClock, value);
                if (_Source1.IsReference())
                {
                    DecodeDataHelper.UpdaterReferenceDataByRange(_Source1, _Source1Threshold);
                    _NeedDecodeData = true;
                }
            }

        }
    }

    public ChannelId Source1
    {
        get => _Source1;
        set
        {
            if (value != _Source1)
            {
                if (value.IsReference())
                {
                    //DecodeDataHelper.UpdaterReferenceData(value, _Source1Threshold);
                    // ljw 24.6
                    DecodeDataHelper.UpdaterReferenceDataByRange(value, _Source1Threshold);
                    _NeedDecodeData = true;
                }
                UpdateProperty(ref _Source1, value);
            }
        }
    }
    public ChannelId Source2
    {
        get => _Source2;
        set
        {
            if (value != _Source2)
            {
                if (value.IsReference())
                {
                    //DecodeDataHelper.UpdaterReferenceData(value, _Source2Threshold);
                    // ljw 24.6
                    DecodeDataHelper.UpdaterReferenceDataByRange(value, _Source2Threshold);
                    _NeedDecodeData = true;
                }
                UpdateProperty(ref _Source2, value);
            }
        }
    }

    /// <summary>
    /// Modify by lihuijun 
    /// </summary>
    public Single Source1Threshold
    {
        get => (Single)(_Source1Threshold*TryGetChannelGain(Source1));
        set
        {
            if (Math.Abs(value - Source1Threshold) > TOLERANCE)
            {
                //DecodeDataHelper.UpdaterReferenceData(_Source1, value);
                // ljw 24.6
                DecodeDataHelper.UpdaterReferenceDataByRange(_Source1, value);
                _NeedDecodeData = true;
            }
            UpdateProperty(ref _Source1Threshold, (Single)(value/ TryGetChannelGain(Source1)));
        }
    }
    public String Source1Unit => GetChannelUnit(Source1);

    public Single Source2Threshold
    {
        get => (Single)(_Source2Threshold * TryGetChannelGain(Source2));
        set
        {
            if (Math.Abs(value - Source2Threshold) > TOLERANCE)
            {
                //DecodeDataHelper.UpdaterReferenceData(_Source2, value);
                // ljw 24.6
                DecodeDataHelper.UpdaterReferenceDataByRange(_Source2, (Single)(value / TryGetChannelGain(Source1)));
                _NeedDecodeData = true;
            }
            UpdateProperty(ref _Source2Threshold, value);
        }
    }
    public String Source2Unit => GetChannelUnit(Source2);
    public Single MaxThreshold1 => (Single)(12 * TryGetChannelGain(Source1));
    public Single MinThreshold1 => -MaxThreshold1;
    public Single MaxThreshold2 => (Single)(12 * TryGetChannelGain(Source2));
    public Single MinThreshold2 => -MaxThreshold2;
    public UInt16 MaxByteCount => 1023;
    public UInt16 MinByteCount => 0;

	override internal Boolean SourceHasData()
    {
        if (DsoPrsnt.DefaultDsoPrsnt == null)
            return false;

        DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Source1, out IChnlPrsnt? prsnt);
        if (prsnt == null)
            return false;

        if (Source1.IsReference() && prsnt.Active && prsnt.VuDatabase != null && prsnt.VuDatabase.Current != null)
        {
            return DecodeDataHelper.ReferenceHasData(Source1, _Source1Threshold);
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
    override internal void ParsingData(ref CancellationToken token)
    {

        ///////////// 准备/检查 ////////////////////////
        return;
  //      _ResultData = new DecodeResultData
  //      {
  //          DecodeViewInfos = Array.Empty<IDecodeViewInfo>()
  //      };
  //      //Double count = 0;
  //      Double sampleFreq = 0;
  //      DecodeDataHelper.Instance.TryGetSampleRate(BusId, _Source1, ref sampleFreq);
  //      UInt32 perChannelDataLength = 0;
  //      DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, _Source1, ref perChannelDataLength);
  //      Boolean hasData = true;
  //      DecodeDataHelper.Instance.TryGetHasData(BusId, _Source1, ref hasData);
  //      Boolean needClear = false;
  //      if (sampleFreq == 0 || perChannelDataLength == 0 || !hasData)
  //      {
  //          needClear = true;
  //          ClearBuffer();
  //          return;
  //      }
  //      SamplingFrequency = sampleFreq;
  //      const Int32 isCancel = 0;
  //      IntPtr isCancelPtr = new IntPtr(isCancel);
  // //     UsbOptions options = new()
  // //     {
		//	//CancelFlag = _CancelFlagPtr,
		//	//SignalType = SignalType,
  // //         SamplingFrequency = SamplingFrequency,
  // //         USBSpeed = SignalRate,
  // //         AutoClock = AutoClock,
        
  // //     };

  //      _EventInfos.Clear();
  //      if (!_NeedDecodeData && _Source1.IsReference())
  //      {
  //          return;
  //      }

		//IntPtr edgepulsePtr1 = IntPtr.Zero;
		//GCHandle edgePulsesHandle1;
		//IntPtr edgepulsePtr2 = IntPtr.Zero;
		//GCHandle edgePulsesHandle2;
		//_NeedDecodeData = false;
  //      _NeedUpdateViewInfo = true;
  //      UInt32 bufferlength = perChannelDataLength;
  //      //decodeResult = new UsbDecodeResult();
  //      ClearBuffer();
  //      ///////////// 数据准备 ////////////////////////
  //      //TwoLevelEdgeInfo? node1 = DecodeDataHelper.Instance.GetEdgeInfo(0, _Source1, ref token, ref needclear)?.Child as TwoLevelEdgeInfo;
  //      //TwoLevelEdgeInfo? node2 = DecodeDataHelper.Instance.GetEdgeInfo(0, _Source2, ref token, ref needclear)?.Child as TwoLevelEdgeInfo;
  //      TwoLevelEdgeInfo? node1 = DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, _Source1, ref token, ref needClear) as TwoLevelEdgeInfo;
  //      TwoLevelEdgeInfo? node2 = DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, _Source2, ref token, ref needClear) as TwoLevelEdgeInfo;
  //      if (node1?.Child == null)
  //      {
  //          //ChangeBuffer();
  //          ChangeBuffer();
  //          return;
  //      }
  //      if (SignalType == DiffSignalType.Difference && (node2?.Child == null))
  //      {
  //          //ClearBuffer();
  //          ChangeBuffer();
  //          return;
  //      }
  //      _NeedDecodeData = false;
  //      //if (_tmpEdgeStartIndex!= node1.Child.StartIndex)
  //      //{
  //      //    _tmpEdgeStartIndex = node1.Child.StartIndex;
  //      //}

  //      List<PAM2EdgePulse> edgePulses1 = new();
  //      List<PAM2EdgePulse> edgePulses2 = new();
         
		/////////////// 解码过程 ////////////////////////
		/////// 边沿脉宽信息获取
		//edgePulses1.Clear();
		//edgePulses2.Clear();
		//DecodeDataHelper.Instance.GetTwoLevelPulses(ref node1, ref edgePulses1);
		//PAM2EdgePulseSequence.Allocate(ref edgePulses1, (UInt64)bufferlength, sampleFreq, out edgepulsePtr1, out edgePulsesHandle1);
		//DecodeDataHelper.Instance.GetTwoLevelPulses(ref node2, ref edgePulses2);
		//PAM2EdgePulseSequence.Allocate(ref edgePulses2, (UInt64)bufferlength, sampleFreq, out edgepulsePtr2, out edgePulsesHandle2);

		////Boolean result = DecoderImpl.DecodeUsb(options, edgepulsePtr1, edgepulsePtr2, out UsbResultStruct decodeResultStruct);
  //      if (!result)
  //      {
  //          Debug.WriteLine(@"解码失败");
        
  //          //释放资源
  //          PAM2EdgePulseSequence.Free(ref edgepulsePtr1, ref edgePulsesHandle1);
  //          PAM2EdgePulseSequence.Free(ref edgepulsePtr2, ref edgePulsesHandle2);
  //          ClearBuffer();
  //          ChangeBuffer();
  //          //c++资源释放
  //          DecoderImpl.FreeUsb(decodeResultStruct);
  //          return;
  //      }
  //      ///////////// 解码结果转换 ////////////////////////
  //     // result = UsbDecodeResult.ConvertData(decodeResultStruct, out decodeResult);
  //      //释放资源
  //      PAM2EdgePulseSequence.Free(ref edgepulsePtr1, ref edgePulsesHandle1);
  //      PAM2EdgePulseSequence.Free(ref edgepulsePtr2, ref edgePulsesHandle2);
  //      if (!result)
  //      {
  //          Debug.WriteLine(@"转换失败");
            
  //          ClearBuffer();
  //          ChangeBuffer();
  //          //c++资源释放
  //          DecoderImpl.FreeUsb(decodeResultStruct);
  //          return;
  //      }
        
  //      if (_NeedUpdateViewInfo)
  //      {
  //          _NeedUpdateViewInfo = false;
  //          if (options.AutoClock)
  //          {
  //              _SignalRate = decodeResult.USBSpeed;
  //          }
  //          List<DecodeResultData> decodebuffer = GetDecodeBuffer();
  //          decodebuffer.Clear();
  //          _EventInfos.Clear();
  //          if (decodeResult.DecodeEventCount == 0)
  //          {
  //              _ResultData.DecodeViewInfos = new IDecodeViewInfo[0];
  //              decodebuffer.Add(_ResultData);
                
  //              ClearBuffer();
  //              ChangeBuffer();
  //              //c++资源释放
  //              DecoderImpl.FreeUsb(decodeResultStruct);
  //              return;
  //          }
  //          try
  //          {
  //              Int32 chindex = GetChIndex(_Source1);
                //_ResultData.DecodeViewInfos = decodeResult.DecodeEvents.SelectMany(eventData =>
               // {
//                    ProtocolEventInfo info = new()
//                    {
//                        Index = _EventInfos.Count
//                    };
//                    var endindex = 0;
//                    info.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(_ => (Encoding.Default.GetBytes("--"), (UInt32)0)));
//                    List<USBDecodePacket> datapackets = new();
//                    Int64 cellStartIndex = eventData.StartIndex;
//                    UInt32 syncBitLen = (UInt32)(decodeResult.USBSpeed == ProtocolUSB.SignalRate.HighRate ? 32 : 8);
//                    USBSYNCDecodePacket sync = new(CalcPosition(cellStartIndex, _Source1, chindex), CalcBitLenght((Int32)(eventData.SingleBitTimingLength * syncBitLen), _Source1, chindex));
//                    datapackets.Add(sync);
//                    info.StartPosition = sync.Start;
//                    endindex = (Int32)cellStartIndex+1;
//                    info.StartTimeByPs = GetTimeFromPosition(sync.Start, chindex);
//                    cellStartIndex += (Int64)(eventData.SingleBitTimingLength * syncBitLen);
//                    String eventTitle = eventData.EventTitle.ToString().Trim();
//                    if (eventData.EventTitle == USBEnums.EventInfoTitles.TOUT)
//                    {
//                        eventTitle = "OUT";
//                    }
//                    //PacketName
//                    info.EventInofs[0] = (Encoding.Default.GetBytes(eventTitle), 0);
//                    USBPIDDecodePacket pid = new(CalcPosition(cellStartIndex, _Source1, chindex), CalcBitLenght((Int32)(eventData.SingleBitTimingLength * 8), _Source1, chindex))
//                    {
//                        Data = Encoding.Default.GetBytes(eventTitle)
//                    };

//                    datapackets.Add(pid);
//                    endindex = (Int32)cellStartIndex + (Int32)(eventData.SingleBitTimingLength * 8);
//                    cellStartIndex += (Int64)(eventData.SingleBitTimingLength * 8);

//                    ////////////////////////// 数据分类展示 ////////////////////////// 
//                    if (eventData.EventTitle == USBEnums.EventInfoTitles.DATA0 || eventData.EventTitle == USBEnums.EventInfoTitles.DATA1)
//                    {
//#if false
//                            // // 连续添加 // // 
//                            //Byte[] datas = new Byte[eventData.DataCount];
//                            String dataStr = "DATA:";
//                            for (UInt64 x = 0; x < eventData.DataCount; x++)
//                            {
//                                dataStr += $" {Marshal.ReadByte(eventData.DecodeDataPtr + (Int32)x):X2}";
//                            }
                            
//                            USBDataDecodePacket dataCell = new(CalcPosition(cellStartIndex, _Source1, chindex), CalcBitLenght((Int32)(eventData.SingleBitTimingLength * 8 * eventData.DataCount), _Source1, chindex))
//                            {
//                                Data = Encoding.Default.GetBytes(dataStr),
//                                BitCount = (UInt32)eventData.DataCount*3+5,
//                            };
//                            datapackets.Add(dataCell);
//                            cellStartIndex += (Int64)(eventData.SingleBitTimingLength * 8 * eventData.DataCount);
//#else
//                        String dataInfoStr = "";
//                        // // 逐个添加 // // 
//                        for (UInt64 x = 0; x < eventData.DataCount; x++)
//                        {
//                            Byte dataVal = Marshal.ReadByte(eventData.DecodeDataPtr + (Int32)x);
//                            dataInfoStr += $" {dataVal:X2}";
//                            USBDataDecodePacket dataCell = new(CalcPosition(cellStartIndex, _Source1, chindex), CalcBitLenght((Int32)(eventData.SingleBitTimingLength * 8), _Source1, chindex))
//                            {
//                                Data = new Byte[1] { dataVal },
//                                //Data = Encoding.Default.GetBytes($"{dataVal:X2}h"),
//                                BitCount = 8
//                            };
//                            datapackets.Add(dataCell);
//                            cellStartIndex += (Int64)(eventData.SingleBitTimingLength * 8);
//                        }
//                        //data
//                        info.EventInofs[1] = (Encoding.Default.GetBytes(dataInfoStr), 0);
//                        endindex = (Int32)cellStartIndex + (Int32)(eventData.SingleBitTimingLength * 8);
//#endif

//                    }
//                    else if (eventData.EventTitle == USBEnums.EventInfoTitles.TOUT
//                    || eventData.EventTitle == USBEnums.EventInfoTitles.SETUP)
//                    {
//                        //////////////   Addr   //////////////
//                        USBPIDDecodePacket addr = new(CalcPosition(cellStartIndex, _Source1, chindex), CalcBitLenght((Int32)(eventData.SingleBitTimingLength * 7), _Source1, chindex))
//                        {
//                            Data = Encoding.Default.GetBytes($"Addr:0x{eventData.Address:X2}")
//                        };
//                        cellStartIndex += (Int64)(eventData.SingleBitTimingLength * 7);
//                        ////////////// EndPoint //////////////
//                        USBPIDDecodePacket endPoint = new(CalcPosition(cellStartIndex, _Source1, chindex), CalcBitLenght((Int32)(eventData.SingleBitTimingLength * 4), _Source1, chindex))
//                        {
//                            Data = Encoding.Default.GetBytes($"EP:0x{eventData.EndPoint:X2}")
//                        };
//                        cellStartIndex += (Int64)(eventData.SingleBitTimingLength * 4);
//                        datapackets.Add(addr);
//                        datapackets.Add(endPoint);
//                        //addr
//                        info.EventInofs[2] = (Encoding.Default.GetBytes($"0x{eventData.Address:X2}"), 0);
//                        //ep
//                        info.EventInofs[3] = (Encoding.Default.GetBytes($"0x{eventData.EndPoint:X2}"), 0);
//                        endindex = (Int32)cellStartIndex + (Int32)(eventData.SingleBitTimingLength * 4);
//                    }
//                    if (eventData.CrcSignNum > 0)
//                    {
//                        if (eventData.CrcSignNum == 5)
//                        {
//                            String crcInfoStr = $"0x{eventData.CrcData:X2}";
//                            USBPIDDecodePacket crc = new(CalcPosition(cellStartIndex, _Source1, chindex), CalcBitLenght((Int32)(eventData.SingleBitTimingLength * 5), _Source1, chindex))
//                            {
//                                Data = Encoding.Default.GetBytes($"CRC5:{crcInfoStr}"),
//                                BorderColor = Color.Purple
//                            };
//                            datapackets.Add(crc);
//                            //crc5
//                            info.EventInofs[4] = (Encoding.Default.GetBytes(crcInfoStr), 0);
//                            endindex = (Int32)cellStartIndex + (Int32)(eventData.SingleBitTimingLength * 5);
//                        }
//                        else
//                        {
//                            String crcInfoStr = $"0x{eventData.CrcData:X4}";
//                            USBPIDDecodePacket crc = new(CalcPosition(cellStartIndex, _Source1, chindex), CalcBitLenght((Int32)(eventData.SingleBitTimingLength * 16), _Source1, chindex))
//                            {
//                                Data = Encoding.Default.GetBytes($"CRC16:{crcInfoStr}"),
//                                BorderColor = Color.Purple
//                            };
//                            datapackets.Add(crc);
//                            //crc16
//                            info.EventInofs[5] = (Encoding.Default.GetBytes(crcInfoStr), 0);
//                            endindex = (Int32)cellStartIndex + (Int32)(eventData.SingleBitTimingLength * 16);
//                        }
//                    }

//                    info.EndPosition = CalcPosition(endindex, _Source1, chindex);
//                    info.EndTimeByPs = GetTimeFromPosition(info.EndPosition, chindex);
//                    _EventInfos.Add(info);

                    //return datapackets;
               // }).ToArray();
 //               decodebuffer.Add(_ResultData);
 //           }
 //           catch
 //           {
 //               // ignored
 //           }
 //           ChangeBuffer();
    }
	
	//	//c++资源释放
	//	DecoderImpl.FreeUsb(decodeResultStruct);
	//}
    //public override HdMessage.IDecoderOptions? GetProtocolRecoder()
    //{
    //    return new HdMessage.ProtocolUSBOptions
    //    {
    //        //ByteCount = ByteCount,
    //        Source1Threshold = _Source1Threshold,
    //        Source1 = Source1,
    //        Source2 = Source2,
    //        Source2Threshold = _Source2Threshold,
    //        SignalRate = SignalRate,
    //        SamplingFrequency = SamplingFrequency,
    //        AutoClock = AutoClock
    //    };
    //}
}

#define ENABLE_CPP_DECODE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NPOI.POIFS.Crypt.Dsig;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using static ScopeX.Core.Decode.EdgeInfoCPP;
using static ScopeX.Core.Decode.I2CDecodeModel;

namespace ScopeX.Core.Decode
{
	/// <summary>
	/// rs232通道解码model
	/// </summary>
	sealed internal class RS232DecodeModel : ProtocolModel
	{

		private List<Rs232PacketInfo> _PacketInfos = new List<Rs232PacketInfo>();
		//private List<Rs232PacketInfo> _OldPacketInfos = new List<Rs232PacketInfo>();
		private DecodeResultData _ResultData = new DecodeResultData();
		public RS232DecodeModel(ChannelId id, Boolean isTrigger = false) : base(id,SerialProtocolType.RS232, isTrigger)
		{
			_ResultData.Name = "RS232";
			_BitRate = _MinSignalRate;
		}
		public override Double BitRateByPs => 1f / this.BitRate * 1E+12;

		private UInt32 _BitRate = 0;
		public override IReadOnlyList<String> EventInfoTitles => new List<String>() { "Index", "Start Time", "Data", "Parity", "Error" };
		
		/// <summary>
		/// 波特率
		/// </summary>
		public UInt32 BitRate
		{
			get { return _BitRate; }
			set
			{
				if (value != _BitRate)
				{
					switch (value)
					{
						case 2400:
							Baud = ProtocolRS232.BPSList.BPSList_2400;
							break;
						case 4800:
							Baud = ProtocolRS232.BPSList.BPSList_4800;
							break;
						case 9600:
							Baud = ProtocolRS232.BPSList.BPSList_9600;
							break;
						case 19200:
							Baud = ProtocolRS232.BPSList.BPSList_19200;
							break;
						case 38400:
							Baud = ProtocolRS232.BPSList.BPSList_38400;
							break;
						case 57600:
							Baud = ProtocolRS232.BPSList.BPSList_57600;
							break;
						case 115200:
							Baud = ProtocolRS232.BPSList.BPSList_115200;
							break;
						default:
							Baud = ProtocolRS232.BPSList.BPSList_Custom;
							break;
					}
					UpdateProperty(ref _BitRate, value);
				}
			}
		}

		private UInt32 _MinSignalRate = 1000;
		private UInt32 _MaxSignalRate = (UInt32)1E9;

		public UInt32 MinSignalRate => _MinSignalRate;
		public UInt32 MaxSignalRate => _MaxSignalRate;

		private ProtocolRS232.DataBitWidth _DataBits = ProtocolRS232.DataBitWidth.DataBitWidth_8Bit;
		/// <summary>
		/// 数据位
		/// </summary>
		public ProtocolRS232.DataBitWidth DataBits
		{
			get { return _DataBits; }
			set { UpdateProperty(ref _DataBits, value); }
		}

		private ProtocolRS232.OddEvenCheck _Parity = ProtocolRS232.OddEvenCheck.None;
		/// <summary>
		/// 校验位
		/// </summary>
		public ProtocolRS232.OddEvenCheck Parity
		{
			get { return _Parity; }
			set { UpdateProperty(ref _Parity, value); }
		}

		private ProtocolRS232.StopBit _StopBits = ProtocolRS232.StopBit.StopBit_1bit;
		/// <summary>
		/// 停止位
		/// </summary>
		public ProtocolRS232.StopBit StopBits
		{
			get { return _StopBits; }
			set { UpdateProperty(ref _StopBits, value); }
		}

		private ChannelId _Source1 = ChannelId.C1;
		/// <summary>
		/// 通道1
		/// </summary>
		public ChannelId Source1
		{
			get { return _Source1; }
			set { UpdateProperty(ref _Source1, value); }
		}


		private ProtocolRS232.MSB_LSB _ByteOrder = ProtocolRS232.MSB_LSB.LSB;
		/// <summary>
		/// 字节顺序
		/// </summary>
		public ProtocolRS232.MSB_LSB ByteOrder
		{
			get { return _ByteOrder; }
			set { UpdateProperty(ref _ByteOrder, value); }
		}


		public Double MaxThreshold => (Single)(8 * TryGetChannelGain(_Source1));
		public Double MinThreshold => -MaxThreshold;

		private Double _Threshold = 1;
		/// <summary>
		/// 阈值
		/// </summary>
		public Double Threshold
		{
			get { return _Threshold * TryGetChannelGain(_Source1); }
			set
			{
				UpdateProperty(ref _Threshold, value / TryGetChannelGain(_Source1));
				DecodeProtocolShareParameter.Default.SetNeedReadData();
			}
		}


		public String Unit => GetChannelUnit(Source1);


		private ProtocolCommon.Polarity _Polarity = ProtocolCommon.Polarity.Positive;
		/// <summary>
		/// 是否反向
		/// </summary>
		public ProtocolCommon.Polarity Polarity
		{
			get { return _Polarity; }
			set { UpdateProperty(ref _Polarity, value); }
		}

		private ChannelId _Source2 = ChannelId.C2;
		/// <summary>
		/// 差分信号时的通道2
		/// </summary>
		public ChannelId Source2
		{
			get => _Source2;
			set => UpdateProperty(ref _Source2, value);
		}

		//private const Int32 isCancel = 0;

		public ProtocolRS232.SignalType _SignalType = ProtocolRS232.SignalType.Single;
		public ProtocolRS232.SignalType SignalType
		{
			get => _SignalType;
			set => UpdateProperty(ref _SignalType, value);
		}

		public ProtocolRS232.BPSList _Baud = ProtocolRS232.BPSList.BPSList_Custom;
		public ProtocolRS232.BPSList Baud
		{
			get => _Baud;
			set
			{
				if (value != _Baud)
				{
					_Baud = value;
					switch (value)
					{
						case ProtocolRS232.BPSList.BPSList_2400:
							BitRate = 2400;
							break;
						case ProtocolRS232.BPSList.BPSList_4800:
							BitRate = 4800;
							break;
						case ProtocolRS232.BPSList.BPSList_9600:
							BitRate = 9600;
							break;
						case ProtocolRS232.BPSList.BPSList_19200:
							BitRate = 19200;
							break;
						case ProtocolRS232.BPSList.BPSList_38400:
							BitRate = 38400;
							break;
						case ProtocolRS232.BPSList.BPSList_57600:
							BitRate = 57600;
							break;
						case ProtocolRS232.BPSList.BPSList_115200:
							BitRate = 115200;
							break;
						case ProtocolRS232.BPSList.BPSList_Custom:
							break;
					}

				}
			}
		}


		public override HdMessage.IDecoderOptions? GetProtocolRecoder()
		{
			return new HdMessage.ProtocolRS232Options()
			{
				Baud = BitRate,
				BitSeq = ByteOrder,
				DataBitWidth = DataBits,
				OddEvenCheck = Parity,
				Polarity = Polarity,
				SignalType = SignalType,
				Source = Source1,
				SourceL = Source2,
				StopBit = StopBits,
				Threshold = _Threshold,
			};
		}
		public struct Rs232PacketInfo
		{
			//public Boolean StartBit;
			public Int32 StartIndex = -1;
			public Byte Data = 0;
			public Int32 DataIndex = -1;
			public Boolean ParityBit = false;
			public Int32 ParityIndex = -1;
			public Boolean ParityFind = false;
			public Boolean ParityResult = false;
			public Double PerBitLenght = 0;

			public Rs232PacketInfo()
			{

			}

			public Rs232PacketInfo(Rs232PacketInfoStruct Rs232PacketStruct)
			{
				StartIndex = Rs232PacketStruct.StartIndex;
				Data = Rs232PacketStruct.Data;
				DataIndex = Rs232PacketStruct.DataIndex;
				ParityBit = Rs232PacketStruct.ParityBit == 1;
				ParityIndex = Rs232PacketStruct.ParityIndex;
				ParityFind = Rs232PacketStruct.ParityFind == 1;
				ParityResult = Rs232PacketStruct.ParityResult == 1;
				PerBitLenght = Rs232PacketStruct.PerBitLenght;
			}
		}

		internal override Boolean SourceHasData()
		{
			if (DsoPrsnt.DefaultDsoPrsnt == null)
				return false;

			DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(Source1, out var prsnt);
			if (prsnt == null)
				return false;

			if (Source1.IsReference() && prsnt.VuDatabase != null && prsnt.VuDatabase.Current != null)
			{
				return DecodeDataHelper.ReferenceHasData(Source1, Threshold);
			}

			if (Source1.IsAnalog())
			{
				return DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
			}

			var shaped = 2;

			if (shaped == 2)
			{
				using (StreamWriter writer = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "ShapedData.txt")))
				{
					if (Source1.IsAnalog())
					{
						foreach (Byte b in DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].ChannelDataSource)
						{
							// 将每个Byte转换为字符串并写入文件的一行
							writer.WriteLine(b.ToString());
						}
					}
					else
					{
						foreach (Byte b in DecodeDataHelper.Instance.ReferenceDataSource[Source1 - ChannelIdExt.MinRChId].ChannelDataSource)
						{
							// 将每个Byte转换为字符串并写入文件的一行
							writer.WriteLine(b.ToString());
						}
					}
				}
			}

			return false;
		}
        internal override Boolean CheckUpdate(ref Int64 laststamp)
		{
			//if (ChannelId.IsAnalog())
			//{
			//    return laststamp != DecodeDataHelper.Instance.AnalogDataSource.TimeStamp;
			//}
			//if (ChannelId.IsReference())
			//{
			//    return laststamp != DecodeDataHelper.Instance.ReferenceDataSource[ChannelId - ChannelIdExt.MinRChId].TimeStamp;
			//}

			if (Source1.IsAnalog() && laststamp != DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp)
			{
				laststamp = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp;
				return true;
			}
			if (Source1.IsReference() && laststamp != DecodeDataHelper.Instance.ReferenceDataSource[Source1 - ChannelIdExt.MinRChId].TimeStamp)
			{
				return true;
			}

			return false;
		}
#if ENABLE_CPP_DECODE
		private void UpdateViewResult(Int32 chindex, Int32 databitcount)
		{
			_NeedUpdateViewInfo = false;
			var decodebuffer = GetDecodeBuffer();
			decodebuffer.Clear();
			_EventInfos.Clear();
			if (_PacketInfos.Count == 0)
			{
				_ResultData.DecodeViewInfos = new IDecodeViewInfo[0];
				decodebuffer.Add(_ResultData);
				ChangeBuffer();
				return;
			}
			try
			{

				_ResultData.DecodeViewInfos = _PacketInfos.SelectMany(x =>
				{
					ProtocolEventInfo info = new ProtocolEventInfo();
                    var endindex = 0;
                    info.Index = _EventInfos.Count;
					info.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
					List<Rs232DecodePacket> datapackets = new List<Rs232DecodePacket>();
					RS232StartDecodePacket start = new RS232StartDecodePacket(CalcPosition(x.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PerBitLenght, Source1, chindex));
					datapackets.Add(start);
					info.StartTimeByPs = GetTimeFromPosition(start.Start, chindex);
                    info.StartPosition = start.Start;
                    RS232DataDecodePacket datapacket = new RS232DataDecodePacket(CalcPosition(x.DataIndex, Source1, chindex), CalcBitLenght((Int32)Math.Round(databitcount * x.PerBitLenght), Source1, chindex))
					{
						Data = new Byte[] { x.Data },
						BitCount = (UInt32)databitcount,
					};
					info.EventInofs[0] = (datapacket.Data, datapacket.BitCount);
					endindex = (Int32)x.DataIndex + (Int32)Math.Round(databitcount * x.PerBitLenght);
                    datapackets.Add(datapacket);
					if (x.ParityFind)
					{
						Rs232ParityDecodePacket paritypacket = new Rs232ParityDecodePacket(CalcPosition(x.ParityIndex, Source1, chindex), CalcBitLenght((Int32)Math.Round(x.PerBitLenght), Source1, chindex))
						{
							ParityBit = x.ParityBit,
							SuccessParityBit = x.ParityResult,
						};
						datapackets.Add(paritypacket);
						info.EventInofs[1] = (new Byte[] { Convert.ToByte(x.ParityBit) }, 4);
						if (!paritypacket.Success)
						{
							info.EventInofs[^1] = (Encoding.Default.GetBytes("Parity Error"), 0);
                        }
                        endindex = (Int32)x.ParityIndex + (Int32)Math.Round(x.PerBitLenght);
                    }
                    info.EndPosition = CalcPosition(endindex, Source1, chindex);
                    info.EndTimeByPs = GetTimeFromPosition(info.EndPosition, chindex);
                    _EventInfos.Add(info);
					return datapackets;
				}).ToArray();
				decodebuffer.Add(_ResultData);
			}
			catch
			{
			}
			ChangeBuffer();
		}
		internal override void ParsingData(ref CancellationToken token)
		{
			Int32 chindex = GetChIndex(Source1);
			Int32 chlindex = GetChIndex(Source2);
			Rs232Result decodeResult = new();
			if (MoreThanStorage() || chindex == -1 || chlindex == -1)
			{
				_NeedDecodeData = false;
				_NeedUpdateViewInfo = true;
				_PacketInfos.Clear();
			}
			Int32 databitcount = DataBits switch
			{
				ProtocolRS232.DataBitWidth.DataBitWidth_5Bit => 5,
				ProtocolRS232.DataBitWidth.DataBitWidth_6Bit => 6,
				ProtocolRS232.DataBitWidth.DataBitWidth_7Bit => 7,
				ProtocolRS232.DataBitWidth.DataBitWidth_8Bit => 8,
				_ => 5,
			};
			Double count = 0;
			Double samlerate = 0;
			DecodeDataHelper.Instance.TryGetSampleRate(BusId, Source1, ref samlerate);
			UInt32 perchanneldatalength = 0;
			DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, Source1, ref perchanneldatalength);
			Boolean hasdata = true;
			DecodeDataHelper.Instance.TryGetHasData(BusId, Source1, ref hasdata);
			Boolean needclear = false;
			if (samlerate == 0 || perchanneldatalength == 0 || !hasdata)
			{
				needclear = true;
				_NeedDecodeData = false;
			}
			if (_NeedDecodeData)
			{
				IntPtr edgepulseptr = IntPtr.Zero;
				GCHandle edgepulseshandle;
				count = (1d / BitRate * samlerate);
				Double realcount = count;
				UInt32 bufferlength = perchanneldatalength;
				_PacketInfos.Clear();
				_NeedDecodeData = false;
				_NeedUpdateViewInfo = true;
				Rs232ResultStruct decoderesultstruct = new();
				 

				_PacketInfos.Clear();
				//TwoLevelEdgeInfo? node = DecodeDataHelper.Instance.GetEdgeInfo(0, Source1,
				//	ref token, ref needclear)?.Child as TwoLevelEdgeInfo;
				TwoLevelEdgeInfo? node = DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, Source1,
					ref token, ref needclear) as TwoLevelEdgeInfo;
				 
				//OldDecode(node, count, realcount, bufferlenght, databitcount, stopbitcount, ref token);
				List<PAM2EdgePulse> edgepulse = new();
				if (node?.Child == null)
				{
					UpdateViewResult(chindex, databitcount);
					return;
				}
				var options = new Rs232Options()
				{

					CancelFlag = _CancelFlagPtr,
					BaudRate = _BitRate,
					DataBitWidth = _DataBits,
					MsbLsb = _ByteOrder,
					OddEvenCheck = Parity,
					Polarity = _Polarity,
					StopBit = _StopBits,

				};
				//if (!DecodeDataHelper.GetPAM2EdgePulseSequence(node, bufferLength, ref edgePulse, samleRate, out PAM2EdgePulseSequence pulseData))
				//{
				//	return;
				//}

				// 边沿脉宽信息获取
				edgepulse.Clear();
				DecodeDataHelper.Instance.GetTwoLevelPulses(ref node, ref edgepulse);
				PAM2EdgePulseSequence.Allocate(ref edgepulse, (UInt64)bufferlength, samlerate, out edgepulseptr, out edgepulseshandle);

				DecoderImpl.DecodeRs232(options, edgepulseptr, out decoderesultstruct);
				PAM2EdgePulseSequence.Free(ref edgepulseptr, ref edgepulseshandle);

				if (decoderesultstruct.EventCount != 0)
				{
					var result = Rs232Result.ConvertData(decoderesultstruct, out _PacketInfos);
					//c++资源释放
					DecoderImpl.FreeRs232(decoderesultstruct);
					if (!result)
					{
						Debug.WriteLine(@"转换失败");
						return;
					}

				}
				_NeedUpdateViewInfo = true;
			}

			if (_NeedUpdateViewInfo)
			{
				UpdateViewResult(chindex, databitcount);
			}
		}

#else
        internal override void ParsingData(ref CancellationToken token)
        {
            Int32 chindex = GetChIndex(Source1);
            Int32 chlindex = GetChIndex(Source2);
            if (chindex == -1 || chlindex == -1)
            {
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                _PacketInfos.Clear();
            }
            Int32 databitcount = DataBits switch
            {
                ProtocolRS232.DataBitWidth.DataBitWidth_5Bit => 5,
                ProtocolRS232.DataBitWidth.DataBitWidth_6Bit => 6,
                ProtocolRS232.DataBitWidth.DataBitWidth_7Bit => 7,
                ProtocolRS232.DataBitWidth.DataBitWidth_8Bit => 8,
                _ => 5,
            };
            Double count = 0;
            Double samlerate = 0;
            DecodeDataHelper.Instance.TryGetSampleRate(Source1, ref samlerate);
            UInt32 perChannelDataLength = 0;
            DecodeDataHelper.Instance.TryGetPerChannelDataLength(Source1, ref perChannelDataLength);
            Boolean hasdata = true;
            DecodeDataHelper.Instance.TryGetHasData(Source1, ref hasdata);
            Boolean needclear = false;
            if (samlerate == 0 || perChannelDataLength == 0 || !hasdata)
            {
                needclear = true;
                _NeedDecodeData = false;
            }
            if (_NeedDecodeData)
            {
                count = (1d / BitRate * samlerate);
                Double realcount = count;
                UInt32 bufferlenght = perChannelDataLength;
                _PacketInfos.Clear();
                _NeedDecodeData = false;
                _NeedUpdateViewInfo = true;
                Int32 stopbitcount = StopBits switch
                {
                    ProtocolRS232.StopBit.StopBit_1bit => 1,
                    ProtocolRS232.StopBit.StopBit_2bit => 2,
                    _ => 1,
                };
                Int32 paritybitcount = Parity switch
                {
                    ProtocolRS232.OddEvenCheck.Odd => 1,
                    ProtocolRS232.OddEvenCheck.Even => 1,
                    ProtocolRS232.OddEvenCheck.None => 0,
                    _ => 0,
                };
                Int32 startindex = 0;
                Int32 packetstartindex = 0;
                _PacketInfos.Clear();
                TwoLevelEdgeInfo? node = DecodeDataHelper.Instance.GetEdgeInfo(startindex, Source1, ref token, ref needclear)?.Child as TwoLevelEdgeInfo;
                try
                {
                    while (true && node != null)
                    {
                        if (count <= 2) break;
                        packetstartindex = GetStartIndex(ref node, count, ref realcount, startindex);
                        if (packetstartindex == -1 /*|| token.IsCancellationRequested*/ || needclear) break;
                        Rs232PacketInfo packet = new Rs232PacketInfo();
                        packetstartindex += (Int32)Math.Round(realcount / 2);
                        if (packetstartindex >= bufferlenght)
                        {
                            break;
                        }
                        if (startindex == bufferlenght) break;
                        //packet.StartBit = GetRS232Bit(packetstartindex, Source1);
                        packet.StartIndex = packetstartindex - (Int32)Math.Round(realcount / 2);
                        packet.DataIndex = packetstartindex + (Int32)Math.Round(realcount / 2);
                        packet.PerBitLenght = realcount;

                        for (Int32 databitindex = 0; databitindex < databitcount; databitindex++)
                        {
                            packetstartindex += (Int32)Math.Round(realcount);
                            if (packetstartindex >= bufferlenght)
                            {
                                startindex = (Int32)bufferlenght;
                                break;
                            }
                            if (ByteOrder == ProtocolRS232.MSB_LSB.MSB)
                            {
                                packet.Data = (Byte)(packet.Data << 1);
                                packet.Data |= (Byte)(GetRS232Bit(packetstartindex, Source1) ? 1 : 0);
                            }
                            else
                            {
                                packet.Data |= (Byte)((GetRS232Bit(packetstartindex, Source1) ? 1 : 0) << databitindex);
                            }
                        }
                        if (startindex == bufferlenght) break;
                        if (Parity != ProtocolRS232.OddEvenCheck.None)
                        {
                            packetstartindex += (Int32)Math.Round(realcount);
                            if (packetstartindex >= bufferlenght)
                            {
                                startindex = (Int32)bufferlenght;
                                _PacketInfos.Add(packet);
                                break;
                            }
                            packet.ParityFind = true;
                            packet.ParityBit = GetRS232Bit(packetstartindex, Source1);
                            packet.ParityResult = ParityData(packet.Data, databitcount);
                            packet.ParityIndex = packet.DataIndex + (Int32)Math.Round(realcount * databitcount);
                        }
                        _PacketInfos.Add(packet);

                        packetstartindex += (Int32)(realcount * stopbitcount);
                        startindex = packetstartindex;
                    }
                }
                catch
                {

                }
            }
            //if(token.IsCancellationRequested || needclear)
            //{
            //    _PacketInfos.Clear();
            //    _NeedUpdateViewInfo = true;
            //}
            if (_NeedUpdateViewInfo)
            {
                _NeedUpdateViewInfo = false;
                var decodebuffer = GetDecodeBuffer();
                decodebuffer.Clear();
                _EventInfos.Clear();
                if (_PacketInfos.Count == 0)
                {
                    _ResultData.DecodeViewInfos = new IDecodeViewInfo[0];
                    decodebuffer.Add(_ResultData);
                    ChangeBuffer();
                    return;
                }
                try
                {

                    _ResultData.DecodeViewInfos = _PacketInfos.SelectMany(x =>
                    {
                        ProtocolEventInfo info = new ProtocolEventInfo();
                        info.Index = _EventInfos.Count;
                        info.EventInofs.AddRange(Enumerable.Range(0, EventInfoTitles.Count - 2).Select(x => (Encoding.Default.GetBytes("--"), (UInt32)0)));
                        List<RS232DecodePacket> datapackets = new List<RS232DecodePacket>();
                        RS232StartDecodePacket start = new RS232StartDecodePacket(CalcPosition(x.StartIndex, Source1, chindex), CalcBitLenght((Int32)x.PerBitLenght, Source1, chindex));
                        datapackets.Add(start);
                        info.TimeByPs = GetTimeFromPosition(start.Start, chindex);
                        RS232DataDecodePacket datapacket = new RS232DataDecodePacket(CalcPosition(x.DataIndex, Source1, chindex), CalcBitLenght((Int32)Math.Round(databitcount * x.PerBitLenght), Source1, chindex))
                        {
                            Data = new Byte[] { x.Data },
                            BitCount = (UInt32)databitcount,
                        };
                        info.EventInofs[0] = (datapacket.Data, datapacket.BitCount);
                        datapackets.Add(datapacket);
                        if (x.ParityFind)
                        {
                            Rs232ParityDecodePacket paritypacket = new Rs232ParityDecodePacket(CalcPosition(x.ParityIndex, Source1, chindex), CalcBitLenght((Int32)Math.Round(x.PerBitLenght), Source1, chindex))
                            {
                                ParityBit = x.ParityBit,
                                SuccessParityBit = x.ParityResult,
                            };
                            datapackets.Add(paritypacket);
                            info.EventInofs[1] = (new Byte[] { Convert.ToByte(x.ParityBit) }, 1);
                            if (!x.ParityResult)
                            {
                                info.EventInofs[^1] = (Encoding.Default.GetBytes("Parity Error"), 0);
                            }
                        }
                        _EventInfos.Add(info);
                        return datapackets;
                    }).ToArray();
                    decodebuffer.Add(_ResultData);
                }
                catch
                {
                }
                ChangeBuffer();
            }
        }
#endif // ENABLE_CPP_DECODE

		private Boolean ParityData(Byte data, Int32 databitCount = 8)
		{
			if (Parity == ProtocolRS232.OddEvenCheck.None) return true;
			Boolean temp = false;
			if (System.Runtime.Intrinsics.X86.Popcnt.IsSupported)
			{
				temp = System.Runtime.Intrinsics.X86.Popcnt.PopCount(data) % 2 == 0;
			}
			else
			{
				while (databitCount > 0)
				{
					temp ^= ((data & 0b01) == 1);
					data >>= 1;
					databitCount--;
				}
			}
			if (Parity == ProtocolRS232.OddEvenCheck.Odd) temp = !temp;
			return temp;
		}
		private Int32 GetStartIndex(ref TwoLevelEdgeInfo? node, Double count, ref Double realcount, Int32 startindex = 0)
		{
			if (node == null) return -1;
			Boolean startstatus = Polarity == ProtocolCommon.Polarity.Positive;
			//var starttime = DateTime.Now;
			var starttime = TimeSpanUtility.GetTimestampSpan();
			Double tolerance = 0.05;//误差值
			while (node != null)
			{
				if (node.CurrentLevel != startstatus && node.StartIndex >= startindex)
				{
					Double bitcount = Math.Round(node.Length / count, 0);
					if (node.Length > count && node.Length < ((Int32)DataBits + 4) * count * (1 - tolerance))
					{
						realcount = node.Length / bitcount;
					}
					if (bitcount > 0) break;
				}
				node = node?.Child as TwoLevelEdgeInfo;
				if (( /*DateTime.Now*/TimeSpanUtility.GetTimestampSpan() - starttime).TotalMilliseconds > 2000)
				{
					return -1;
				}
			}
			return node == null ? -1 : node.StartIndex;
		}


		private Boolean GetRS232Bit(Int32 index, ChannelId ch)
		{
			return DecodeDataHelper.Instance.GetLevel(BusId, index, Threshold, ch, Polarity == ProtocolCommon.Polarity.Negative);
		}
	}
}

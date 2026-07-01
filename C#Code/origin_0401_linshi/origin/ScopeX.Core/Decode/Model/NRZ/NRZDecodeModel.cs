#define ENABLE_CPP_DECODE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using static ScopeX.ComModel.ProtocolCommon;
using static ScopeX.Core.Decode.EdgeInfoCPP;


namespace ScopeX.Core.Decode
{
	internal struct NRZPacketInfo
	{
		public Int32 StartIndex = -1;
		public Byte Data = 0;
		public Double PerBitLength = 0;
		public NRZPacketInfo()
		{

		}

	}
	internal class NRZDecodeModel : ProtocolModel
	{
		//#if ENABLE_CPP_DECODE
		//        [DllImport("Decode\\ProtocolDecoder.dll", CallingConvention = CallingConvention.Cdecl)]
		//        public static extern Boolean ParseNrz(NRZOptions options, PAM2EdgePulseSequence edgePulse,
		//       out NRZResultStruct decodeResult);

		//        [DllImport("Decode\\ProtocolDecoder.dll", CallingConvention = CallingConvention.Cdecl)]
		//        public static extern void ReleaseNrzHeap(IntPtr objPtr);
		//#endif
		public NRZDecodeModel(ChannelId id, Boolean isTrigDecode = false) : base(id,SerialProtocolType.NRZ, isTrigDecode)
		{
		}
		private Byte _CancelFlag = 0;
		private IntPtr _CancelFlagPtr = Marshal.AllocHGlobal(1);
		public Byte CancelFlag
		{
			get { return _CancelFlag; }
			set
			{
				_CancelFlag = value;
				Marshal.WriteByte(_CancelFlagPtr, value);
			}
		}

		public override IReadOnlyList<String> EventInfoTitles => new List<String>()
		{
			"Index",
			"Start Time",
			"Data",
		};

		private DecodeResultData _ResultData = new DecodeResultData();
		private ProtocolNRZ.Mode _Mode = ProtocolNRZ.Mode.NRZ;
		public Polarity Polarity
		{
			get { return _Polarity; }
			set { UpdateProperty(ref _Polarity, value); }
		}
		private Polarity _Polarity;
		public ProtocolNRZ.Mode Mode
		{
			get { return _Mode; }
			set { UpdateProperty(ref _Mode, value); }
		}

		//private ProtocolNRZ.IdleLevel _IdleLevel;

		//public ProtocolNRZ.IdleLevel IdleLevel
		//{
		//    get { return _IdleLevel; }
		//    set { UpdateProperty(ref _IdleLevel, value); }
		//}
		public UInt32 MaxIdleTime => 10000;
		public UInt32 MinIdleTime => 1;
		//private UInt32 _IdleTime = 60;

		//public UInt32 IdleTime
		//{
		//    get { return _IdleTime; }
		//    set { UpdateProperty(ref _IdleTime, value); }
		//}
		public override Double BitRateByPs => 1f / RealSignalRate * 1E12;

		private ChannelId _Source1 = ChannelId.C1;

		public ChannelId Source1
		{
			get { return _Source1; }
			set { UpdateProperty(ref _Source1, value); }
		}
		private List<NRZPacketInfo> _PacketInfos = new List<NRZPacketInfo>();
		//private ChannelId _Source2;

		//public ChannelId Source2
		//{
		//    get { return _Source2; }
		//    set { UpdateProperty(ref _Source2, value); }
		//}

		public Single MaxThreshold1 => (Single)(12 * TryGetChannelGain(_Source1));
		public Single MinThreshold1 => -MaxThreshold1;
		//public Single MaxThreshold2 => (Single)(12 * TryGetChannelGain(_Source2));
		//public Single MinThreshold2 => -MaxThreshold2;
		private Single _Threshold = 1;

		public Single Threshold
		{
			get { return (Single)(_Threshold * TryGetChannelGain(Source1)); }
			set { UpdateProperty(ref _Threshold, (Single)(value / TryGetChannelGain(Source1))); }
		}
		public String Unit => GetChannelUnit(Source1);

		private ProtocolNRZ.SignalRate _SignalRate = ProtocolNRZ.SignalRate.Custom;

		public ProtocolNRZ.SignalRate SignalRate
		{
			get { return _SignalRate; }
			set
			{
				if (value != _SignalRate)
				{
					UpdateProperty(ref _SignalRate, value);
					//switch (value)
					//{
					//    case ProtocolNRZ.SignalRate.Speed_6_5G:
					//        RealSignalRate = 6_500_000_000;
					//        break;
					//    case ProtocolNRZ.SignalRate.Speed_1G:
					//        RealSignalRate = 1_000_000_000;
					//        break;
					//    case ProtocolNRZ.SignalRate.Speed_650M:
					//        RealSignalRate = 650_000_000;
					//        break;
					//}
				}
			}
		}

		private Int64 _RealSignalRate = 1_000_000;

		public Int64 RealSignalRate
		{
			get { return _RealSignalRate; }
			set
			{
				if (value != _RealSignalRate)
				{
					if (value > MaxSignalRate)
					{
						value = MaxSignalRate;
					}
					UpdateProperty(ref _RealSignalRate, value);
					SignalRate = ProtocolNRZ.SignalRate.Custom;
					//switch (value)
					//{
					//    //case 6_500_000_000:
					//    //    SignalRate = ProtocolNRZ.SignalRate.Speed_6_5G;
					//    //    break;
					//    //case 1_000_000_000:
					//    //    SignalRate = ProtocolNRZ.SignalRate.Speed_1G;
					//    //    break;
					//    //case 650_000_000:
					//    //    SignalRate = ProtocolNRZ.SignalRate.Speed_650M;
					//    //    break;
					//    default:
					//        SignalRate = ProtocolNRZ.SignalRate.Custom;
					//        break;
					//}
				}
			}
		}
		public Int64 MinSignalRate => 1000;
		public Int64 MaxSignalRate
		{
			get
			{
				Double samleRate = 0;
				DecodeDataHelper.Instance.TryGetSampleRate(BusId, Source1, ref samleRate);
				samleRate /= 2;
				//UInt32 perChannelDataLength = 0;
				//DecodeDataHelper.Instance.TryGetPerChannelDataLength(Source1, ref perChannelDataLength);
				//if (samleRate > perChannelDataLength / 2)
				//{
				//	samleRate = perChannelDataLength / 2;
				//}
				if (samleRate < MinSignalRate)
				{
					samleRate = MinSignalRate * 10;
				}
				return (Int64)samleRate;
			}
		}
		private ProtocolNRZ.MSB_LSB _MSB_LSB = ProtocolNRZ.MSB_LSB.LSB;

		public ProtocolNRZ.MSB_LSB MSB_LSB
		{
			get { return _MSB_LSB; }
			set { UpdateProperty(ref _MSB_LSB, value); }
		}
		//private ProtocolNRZ.SignalType _SignalType = ProtocolNRZ.SignalType.Single;

		//public ProtocolNRZ.SignalType SignalType
		//{
		//    get { return _SignalType; }
		//    set { UpdateProperty(ref _SignalType, value); }
		//}
#if ENABLE_CPP_DECODE
		private void UpdateViewResult(Int32 chindex)
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
					List<NRZDecodePacket> datapackets = new List<NRZDecodePacket>();
					NRZDataDecodePacket data = new NRZDataDecodePacket(CalcPosition(x.StartIndex, Source1, chindex), CalcBitLenght((Int32)(x.PerBitLength * 8), Source1, chindex))
					{
						Data = new Byte[] { x.Data },
						BitCount = 8
					}
					;

					datapackets.Add(data);
					info.EventInofs[0] = new(data.Data, data.BitCount);
					info.StartTimeByPs = GetTimeFromPosition(x.StartIndex, chindex);
					info.StartPosition = data.Start;
					info.EndPosition = CalcPosition(x.StartIndex+ (Int32)(x.PerBitLength * 8), Source1, chindex);
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
			//_PacketInfos.Clear();
			NRZResult decodeResult = new();
			if (MoreThanStorage() || chindex == -1)
			{
				_NeedDecodeData = false;
				_NeedUpdateViewInfo = true;
				_PacketInfos.Clear();
			}

			Double count = 0;
			Double samleRate = 0;
			DecodeDataHelper.Instance.TryGetSampleRate(BusId, Source1, ref samleRate);
			UInt32 perChannelDataLength = 0;
			DecodeDataHelper.Instance.TryGetPerChannelDataLength(BusId, Source1, ref perChannelDataLength);
			Boolean hasdata = true;
			DecodeDataHelper.Instance.TryGetHasData(BusId, Source1, ref hasdata);
			Boolean needclear = false;
			if (samleRate == 0 || perChannelDataLength == 0 || !hasdata)
			{
				needclear = true;
				_NeedDecodeData = false;
			}
			IntPtr edgepulsePtr = IntPtr.Zero;
			GCHandle edgePulsesHandle;
			if (_NeedDecodeData)
			{
				count = (samleRate / _RealSignalRate);
				_PacketInfos.Clear();
				_NeedUpdateViewInfo = true;
				_NeedDecodeData = false;
				if (count < 1)
				{
					if (_RealSignalRate > MaxSignalRate)
					{
						RealSignalRate = MaxSignalRate;
					}
					UpdateViewResult(chindex);
					return;
				}
				UInt32 bufferLength = perChannelDataLength;
				TwoLevelEdgeInfo? node = DecodeDataHelper.Instance.GetEdgeInfo(BusId, 0, Source1,
					ref token, ref needclear) as TwoLevelEdgeInfo;

				List<PAM2EdgePulse> edgePulse = new();
				if (node?.Child == null)
				{
					UpdateViewResult(chindex);
					return;
				}
				CancelFlag = 0; //用户参数
				var options = new NRZOptions()
				{
					BaudRate = (UInt32)_RealSignalRate,

					MsbLsb = _MSB_LSB,

					Polarity = _Polarity,

					CancelFlag = _CancelFlagPtr,

				};
				//if (!DecodeDataHelper.GetPAM2EdgePulseSequence(node, bufferLength, ref edgePulse, samleRate, out PAM2EdgePulseSequence pulseData))
				//{
				//	return;
				//}

				// 边沿脉宽信息获取
				edgePulse.Clear();
				DecodeDataHelper.Instance.GetTwoLevelPulses(ref node, ref edgePulse);
				PAM2EdgePulseSequence.Allocate(ref edgePulse, (UInt64)bufferLength, samleRate, out edgepulsePtr, out edgePulsesHandle);
				 
				DecoderImpl.DecodeNrz(options, edgepulsePtr, out NRZResultStruct decodeResultStruct);
				if (decodeResultStruct.EventCount != 0)
				{
					var result = NRZResult.ConvertData(decodeResultStruct, out _PacketInfos);
					PAM2EdgePulseSequence.Free(ref edgepulsePtr, ref edgePulsesHandle);
					//c++资源释放
					DecoderImpl.FreeNrz(decodeResultStruct);
					if (!result)
					{
						Debug.WriteLine(@"转换失败");
						return;
					}
				}
				_NeedUpdateViewInfo = true;
			}
			//if(token.IsCancellationRequested || needclear)
			//{
			//    _PacketInfos.Clear();
			//    _NeedUpdateViewInfo = true;
			//}
			if (_NeedUpdateViewInfo)
			{
				UpdateViewResult(chindex);
			}
		}

#else
        internal override void ParsingData(ref CancellationToken token)
        {

        }
#endif

		public override HdMessage.IDecoderOptions? GetProtocolRecoder()
		{
			return new HdMessage.ProtocolNRZOptions()
			{
				MSB_LSB = MSB_LSB,
				SignalRate = RealSignalRate,
				//SignalType = SignalType,
				Source1 = Source1,
				//Source2 = Source2,
				Threshold = _Threshold,
				//IdleLevel = IdleLevel,
				//IdleTime = IdleTime,
				Mode = Mode,
			};
		}

	}
}

using EventBus;
using ScopeX.ComModel;
using ScopeX.Core.Decode;
using ScopeX.Core.Model.Jitter.Common;
using ScopeX.Core.Model.Jitter.Eye;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    internal class MSO7000HD : IPlatform
    {
        public ProductType ProductType { get; } = ProductType.JiHe_MSO7000HD;

        #region BandWidth
        private const String NAME_8GHZ = "8GHz";
        private const String NAME_5GHZ = "5GHz";
        private const String NAME_4GHZ = "4GHz";
        private const String NAME_3GHZ = "3GHz";
        private const String NAME_500MHZ = "500MHz";
        private const String NAME_200MHZ = "200MHz";
        private const String NAME_20MHZ = "20MHz";
        private const String NAME_NULL = "";

        private List<(Int32 Index, String Name)> _HZBandWidthNames = new List<(Int32 Index, String Name)>()
        {
            (0,NAME_NULL),
            (1,NAME_500MHZ),
            (2,NAME_NULL),
            (3,NAME_20MHZ),
            (4,NAME_NULL),
            (5,NAME_NULL),
        };
        private List<(Int32 Index, String Name)> _LZ5GBandWidthNames = new List<(Int32 Index, String Name)>()
        {
            (0,NAME_5GHZ),
            (1,NAME_500MHZ),
            (2,NAME_200MHZ),
            (3,NAME_20MHZ),
            (4,NAME_NULL),
            (5,NAME_NULL),
            };
        private List<(Int32 Index, String Name)> _LZ8GBandWidthNames = new List<(Int32 Index, String Name)>()
        {
            (0,NAME_8GHZ),
            (1,NAME_4GHZ),
            (2,NAME_NULL),
            (3,NAME_20MHZ),
            (4,NAME_NULL),
            (5,NAME_NULL),
        };

        public IReadOnlyList<(Int32 Index, String Name)> GetBandWidthNames(Boolean is2G, AnaChnlCoupling anaChnlCoupling, Boolean active, Boolean isForcedRead)
        {
            if (Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G)
            {
                if (DsoModel.Default.Timebase.InterleaveMode == AdcInterleaveMode.Mode1To1)
                {
                    _LZ8GBandWidthNames[0] = (0, NAME_NULL);
                }
                else
                {
                    _LZ8GBandWidthNames[0] = (0, NAME_8GHZ);
                }
                return _LZ8GBandWidthNames.Where(x => !String.IsNullOrEmpty(x.Name)).ToList().AsReadOnly();
            }
            else
            {
                //AnaChnlType.ANA_5G
                if (anaChnlCoupling == AnaChnlCoupling.DC50)
                {
                    if (DsoModel.Default.Timebase.InterleaveMode == AdcInterleaveMode.Mode1To1)
                        _LZ5GBandWidthNames[0] = (0, NAME_3GHZ);
                    else
                        _LZ5GBandWidthNames[0] = (0, NAME_5GHZ);

                    return _LZ5GBandWidthNames.Where(x => !String.IsNullOrEmpty(x.Name)).ToList().AsReadOnly();
                }
                else
                {
                    return _HZBandWidthNames.Where(x => !String.IsNullOrEmpty(x.Name)).ToList().AsReadOnly();
                }
            }
        }

        #endregion

        #region MinTimeBaseIndex


        private AnaChnlTimebaseIndex _MinIndex1to2 = AnaChnlTimebaseIndex.Lv200p;
        private AnaChnlTimebaseIndex _MinIndex1To1 = AnaChnlTimebaseIndex.Lv100p;
        private AnaChnlTimebaseIndex _MinIndex2To1 = AnaChnlTimebaseIndex.Lv50p;
        public AnaChnlTimebaseIndex GetTimebaseMinIndex(AdcInterleaveMode adcInterleaveMode)
        {
            switch (adcInterleaveMode)
            {
                case AdcInterleaveMode.Mode4To1:
                case AdcInterleaveMode.Mode2To1:
                    return _MinIndex2To1; //20G 7000HD走不到这个分支，代码保留仅作对比
                case AdcInterleaveMode.Mode1To1:
                    return _MinIndex1To1; //10G
                case AdcInterleaveMode.Mode1To2:
                    return _MinIndex1to2; // 5G
                default:
                    return _MinIndex1To1; //10G
            }
        }

        #endregion

        private String _ProductModel = "";
        public IReadOnlyList<KeyValuePair<String, Int32>> GetAnaChnlLengthSource(List<KeyValuePair<String, Int32>> source, Double timeScale, AdcInterleaveMode adcInterleaveMode)
        {
            if (source == null || source.Count == 0)
            {
                return source;
            }
            (Double MaxSamplingByus, Int32 MinDeepDots) maSampling_MinDeepDots = adcInterleaveMode switch
            {
                AdcInterleaveMode.Mode2To1 => (5e-5, 1000), //20G 7000HD走不到这个分支，代码保留仅作对比
                AdcInterleaveMode.Mode1To1 => (1e-4, 1000), //10G
                AdcInterleaveMode.Mode1To2 => (2e-4, 1000), // 5G
                _ => (2e-4, 1000), // 5G
            };
            Int64 currDeepDots = (Int64)(timeScale * 10 / maSampling_MinDeepDots.MaxSamplingByus);
            if (currDeepDots < maSampling_MinDeepDots.MinDeepDots)
                currDeepDots = maSampling_MinDeepDots.MinDeepDots;
            else if (currDeepDots > source.Last().Value)
                currDeepDots = source.Last().Value;
            switch (adcInterleaveMode)
            {
                case AdcInterleaveMode.Mode2To1:
                    currDeepDots = (currDeepDots + 1000 - 1) / 1000 * 1000;
                    break;
                case AdcInterleaveMode.Mode1To1:
                    currDeepDots = (currDeepDots + 1000 - 1) / 1000 * 1000;
                    break;
            }

            source[0] = new(nameof(AnaChnlLengthOpt.Auto), (Int32)currDeepDots);
            return source.AsReadOnly();
        }

        public (Int32 MaxUv, Int32 MinUv) GetBiasRange(AnaChnlCoupling coupling, AnaChnlScaleIndex scale)
        {
            //if(coupling != AnaChnlCoupling.DC50)
            //{
            //    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs("MSO8000HD Only Support DC1M!" , LogLevel.Error));
            //    return (0, 0);
            //}
            if (scale <= AnaChnlScaleIndex.Lv50m)
            {
                return (500_000, -500_000);
            }
            else if (scale <= AnaChnlScaleIndex.Lv200m)
            {
                return (1_000_000, -1_000_000);
            }
            else if (scale <= AnaChnlScaleIndex.Lv1)
            {
                return (4_000_000, -4_000_000);
            }
            else if (scale <= AnaChnlScaleIndex.Lv10)
            {
                return (40_000_000, -40_000_000);
            }
            else
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs("MSO7000HD Only Support <= 1V!", LogLevel.Info));
                return (0, 0);
            }
        }

        public Boolean DoLAMutex() => true;

        public void SetBandWidthByInterleaveMode(AdcInterleaveMode adcInterleaveMode)
        {
            return;
        }

        /// <summary>
        /// added by hjli 
        /// 
        /// 适配不同型号对边沿触发与指示灯的联动
        /// 7000 无联动
        /// </summary>
        /// <param name="slope"></param>
        public void SetEdgeTriggerLed(EdgeSlope slope)
        {
            switch (slope)
            {
                case EdgeSlope.Rise: KeyLed.Default.SetTriggerSlope(0); break;
                case EdgeSlope.Fall: KeyLed.Default.SetTriggerSlope(1); break;
                case EdgeSlope.Both: KeyLed.Default.SetTriggerSlope(2); break;
                default: KeyLed.Default.SetTriggerSlope(255); break;
            }
        }

        public void TriggerTypeChanged(TriggerType type)
        {
            if (type != TriggerType.Edge)
            {
                KeyLed.Default.SetTriggerSlope(255);
            }
            else
            {

            }
        }

        /// <summary>
        /// 获取DDr的分段数
        /// </summary>
        /// <param name="storageWaveDotsCnt">存储深度</param>
        /// <param name="adcInterleaveMode">交织模式</param>
        /// <returns></returns>
        public Int32 GetDDrMaxFrameCount(Int64 storageWaveDotsCnt, AdcInterleaveMode adcInterleaveMode)
        {
            var cnt = (storageWaveDotsCnt + 128 - 1) / 128 * 128 + 5 * 64;//硬件里面数据存储深度加了一段 5*64

            var chnlcnt = adcInterleaveMode switch
            {
                AdcInterleaveMode.Mode2To1 => 1,
                _ => 2,
            };
            Int32 triggerAddrStart = 0;
            Hd.TryGetData(ChannelType.Analog, $"{AnalogParamEnum.TriggerAddrStart}_", out Object? data);
            if (data != null && data is Int32)
            {
                triggerAddrStart = (Int32)data;
            }
            else
            {
                triggerAddrStart = 0x1FC00_000;
            }

            var count = (Int32)Math.Floor((Double)((Int64)triggerAddrStart! * 16 / cnt / chnlcnt));
            count = count >= Constants.SEGMENT_FRAME_COUNT_MAX ? Constants.SEGMENT_FRAME_COUNT_MAX : count;
            return count - 1;//最大存储深度下最后一段存在异常波形，需硬件排查
            //return (Int32)Math.Floor((Double)((Int64)0x1FF00000 * 64 / 8 / cnt / 4)) - 1;//减一：防止分段数据溢出 /*MaxSegmentCnt[AnaChnlLengthOpt.Of25KDots];*/
        }

        public IEnumerable<AnaChnlCoupling> GetSupportedCouplings(String serailNumber, AnaChnlStorageMode storageMode)
        {
            if (Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G)
            {
                return new List<AnaChnlCoupling>() { AnaChnlCoupling.DC50, AnaChnlCoupling.Gnd };
            }
            else
            {
                if (storageMode == AnaChnlStorageMode.Fast)
                {
                    return Enum.GetValues<AnaChnlCoupling>().Where(o => o != AnaChnlCoupling.Gnd);
                }
                else
                {
                    return Enum.GetValues<AnaChnlCoupling>();
                }
            }
        }

        public void LoadOriginSetting()
        {
            if (Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G)
            {
                //8G通道只支持低阻
                foreach (var id in ChannelIdExt.GetAnalogs())
                {
                    var ach = (AnalogModel)DsoModel.Default.GetChannel(id);
                    ach.Conditioning.Coupling = AnaChnlCoupling.DC50;
                    ach.Conditioning.ScaleMaxIndex = AnaChnlScaleIndex.Lv1;
                    ach.Conditioning.ScaleMinIndex = AnaChnlScaleIndex.Lv1m;
                    ach.Conditioning.Scale = 1000;// 1000mv/div
                }
            }
        }

        public Int64 GetViewWaveDotsCnt(AdcInterleaveMode adcInterleaveMode)
        {
            return adcInterleaveMode switch
            {
                AdcInterleaveMode.Mode2To1 => 100_000,
                _ => 50_000,
            };
        }

        public String GetSNPrefix()
        {
            return "HD82";
        }

        public void SetProductModel(String productModel)
        {
            _ProductModel = productModel;
        }

        public String GetProductModel()
        {
            if (String.IsNullOrEmpty(_ProductModel) == false)
            {
                return _ProductModel;
            }

            String serialnumber = OptionsManager.Default.SerialNumber;
            if (String.IsNullOrWhiteSpace(serialnumber) || serialnumber.Length < 5 || serialnumber.Substring(1, 4) == GetSNPrefix())
            {
                return "MSO8804HD";
            }

            return "MSO8504HD";
        }

        /// <summary>
        /// 支持选装的协议（目前几个项目是一样的，将来可能会有不同）
        /// </summary>
        public IDictionary<SerialProtocolType, OptionType> OptionProtocols => _OptionProtocols;

        private Dictionary<SerialProtocolType, OptionType> _OptionProtocols = new Dictionary<SerialProtocolType, OptionType>()
        {
            [SerialProtocolType.CAN_FD] = OptionType.Decode_CanFD,
            [SerialProtocolType.FlexRay] = OptionType.Decode_FlexRay,
            [SerialProtocolType.SENT] = OptionType.Decode_SENT,
            [SerialProtocolType.MIL] = OptionType.Decode_AERO,
            [SerialProtocolType.ARINC429] = OptionType.Decode_AERO,
            [SerialProtocolType.AudioBus] = OptionType.Decode_AudioBus,
        };

        [DllImport("CryptWrapper.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Crypt_8000HD(String key, String optionname);

        public String? GetOptionActiveCode(String sn, String name)
        {
            return Marshal.PtrToStringAnsi(Crypt_8000HD(sn, name));
        }

        public Boolean ShowWeakTip => true;

        public Boolean DefaultHighImpedance => true;

        public Boolean IncludeDigitalChnl => true;

        public Boolean EnableGetOrSetScreenBrightness => true;

        public Int32 JitterMaxDataLength => 10_000_000;

        public Boolean LimitBandwidth => Constants.ENABLE_BANDWIDTH;

        public Boolean NeedCheckBandwidth => false;

        public void ProcessDecodeData(ChannelId id, Int32 bitCount, ref DeocodeDataSourcePacket datasource, ref TwoLevelEdgeInfo root, ref TwoLevelEdgeInfo tempnode, ref Int32 nodeindex, CancellationToken token)
        {
            //8000取数规则
            //如果是单通道且为C1或者C3则顺序取数
            //如果是其他情况则为4通道数据，按照每个通道2个Byte（16bit）顺序取数（C1(2Byte)C2(2Byte)C3(2Byte)C4(2Byte)）
            Byte[]? data = null;
            var maxbytecount = datasource.MaxByteCount;
            var chindex = Array.FindIndex(datasource.Channels, x => x == id);
            var startbyteindex = 0;
            var laststatus = (datasource.ChannelDataSource[startbyteindex] >> (bitCount - 1)) & 0x01;
            var currentstatus = laststatus;
            if (datasource.Channels.Length != 1 || (datasource.Channels.Length == 1 && datasource.Channels[0] != ChannelId.C1 && datasource.Channels[0] != ChannelId.C3))
            {
                data = new Byte[maxbytecount / 4];
                chindex = id - ChannelId.C1;
                var dataindex = 0;
                for (Int32 index = chindex * 2; index < maxbytecount; index += 8)
                {
                    data[dataindex++] = datasource.ChannelDataSource[index + 1];
                    data[dataindex++] = datasource.ChannelDataSource[index];
                }
                maxbytecount = maxbytecount / 4;
                startbyteindex = 0;
            }
            else if (datasource.Channels.Length == 1 && (datasource.Channels[0] != ChannelId.C1 || datasource.Channels[0] != ChannelId.C3))
            {
                data = datasource.ChannelDataSource;
            }

            if (data != null)
            {
                for (Int32 index = startbyteindex; index < maxbytecount; index++)
                {
                    for (Int32 bitindex = 0; bitindex < bitCount; bitindex++)
                    {
                        if (token.IsCancellationRequested)
                        {
                            return;
                        }
                        currentstatus = (data[index] >> (bitCount - 1 - bitindex)) & 0x01;
                        nodeindex = index * bitCount + bitindex;
                        if (currentstatus != laststatus)
                        {
                            tempnode = tempnode.AddChild(nodeindex, currentstatus);
                            laststatus = currentstatus;
                        }
                    }
                }
            }
        }

        public void ProbePrompt(ref Int32 readCount, String sn)
        {
            if (readCount < 20)
                readCount++;
            if (readCount == 20 && sn == String.Empty)
            {
                readCount++;
                WeakTip.Default.Write("Probe", MsgTipId.ProbeUnavailable, duration: 5);
            }
        }

        public Boolean KeyEnumCursor() => false;

        public Boolean JitterFunctionLimit(Boolean forceClose)
        {
            if (DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary.Where(pa => pa.Value.Active).ToList().Count() > 0)
            {
                if (forceClose)
                {
                    foreach (var item in DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary)
                    {
                        item.Value.Active = false;
                    }
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.JitterIsNotSupportedInPowerAnalysis, MessageType.Asking))
                    {
                        foreach (var item in DsoPrsnt.DefaultDsoPrsnt.PwrAnalysisDictionary)
                        {
                            item.Value.Active = false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (DsoPrsnt.DefaultDsoPrsnt.PassFail?.Active ?? false)
            {
                if (forceClose)
                {
                    DsoPrsnt.DefaultDsoPrsnt.PassFail.Active = false;
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.JitterIsNotSupportedInPassFail, MessageType.Asking))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.PassFail.Active = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            var decodelist = DsoPrsnt.DefaultDsoPrsnt.TryGetRange(c => c.Id.IsDecode() && c.Active).Cast<Core.Decode.DecodePrsnt>();
            if (decodelist != null && decodelist.Count() > 0)
            {
                if (forceClose)
                {
                    foreach (var item in decodelist)
                    {
                        item.Active = false;
                    }
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.JitterIsNotSupportedInDecode, MessageType.Asking))
                    {
                        foreach (var item in decodelist)
                        {
                            item.Active = false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (DsoPrsnt.DefaultDsoPrsnt.Timebase.SegmentActive)
            {
                if (forceClose)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Timebase.SegmentActive = false;
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.JitterIsNotSupportedInSegment, MessageType.Asking))
                    {
                        DsoPrsnt.DefaultDsoPrsnt.Timebase.SegmentActive = false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            if ((DsoModel.Default?.DigitalChnls.Any(d => d.Active) ?? false))
            {
                if (forceClose)
                {
                    DsoModel.Default?.DigitalChnls.ToList().ForEach(d => d.Active = false);
                }
                else
                {
                    if (StrongTip.Default.Show(MsgTipId.Asking, MsgTipId.JitterIsNotSupportedInLA, MessageType.Asking))
                    {
                        DsoModel.Default?.DigitalChnls.ToList().ForEach(d => d.Active = false);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public IEnumerable<ChannelId> GetTriggerSource() => ChannelIdExt.GetTriggerSources();

        public Int32 GetChannelDelayPoint(Double uiPoint, Double channelDelay)
        {
            return (Int32)Math.Round(uiPoint, 0, MidpointRounding.AwayFromZero);
        }

        public void GetEyeGraphParams(ref FastEyeParams fastEyeParams, Double averageUILength, Int32 dataHeight, Int32 dataWidth, Double[] interpolated_data, Double[] eye_ref_edges_array, out Double[,] matrix, out Double[] levhist)
        {
            if (eye_ref_edges_array.Length * averageUILength < EyeCommon.ParallelNumber || fastEyeParams?.IsFastEye == true)
            {
                matrix = EyeCommon.GetEyeDiagramMatrixInterpInOriginalData(fastEyeParams, EyeCommon.EyeHight, EyeCommon.EyeWidth, dataHeight, dataWidth, 0, 1, interpolated_data, eye_ref_edges_array, out levhist);
            }
            else
            {
                var task = EyeCommon.GetEyeDiagramMatrixInterpInOriginalDataInParallel(fastEyeParams, EyeCommon.EyeHight, EyeCommon.EyeWidth, dataHeight, dataWidth, 0, 1, interpolated_data, eye_ref_edges_array);
                matrix = task.Matrix;
                levhist = task.Hist;
            }
        }
    }
}

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
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    internal class MSO7000X : IPlatform
    {
        public ProductType ProductType { get; } = ProductType.JiHe_MSO7000X;

        #region BandWidth
        private readonly String HZBANDWIDTH_LV0_NAME = "";
        //private readonly String HZBANDWIDTH_LV1_NAME = "500MHz";
        private readonly String HZBANDWIDTH_LV1_NAME = FULLBANDWIDTH_HZ;
        private readonly String HZBANDWIDTH_LV2_NAME = "";
        private readonly String HZBANDWIDTH_LV3_NAME = "20MHz";
        private readonly String HZBANDWIDTH_LV4_NAME = "";
        private readonly String HZBANDWIDTH_LV5_NAME = "";

        //private readonly String LZBANDWIDTH_LV0_NAME = "FULL";
        private readonly String LZBANDWIDTH_LV0_NAME = FULLBANDWIDTH_LZ;
        private readonly String LZBANDWIDTH_LV1_NAME = "1GHz";
        private readonly String LZBANDWIDTH_LV2_NAME = "500MHz";
        private readonly String LZBANDWIDTH_LV3_NAME = "20MHz";
        private readonly String LZBANDWIDTH_LV4_NAME = "";
        private readonly String LZBANDWIDTH_LV5_NAME = "";

        private const String FULLBANDWIDTH_LZ = "2GHz";
        private const String FULLBANDWIDTH_HZ = "500MHz";

        private System.Collections.Generic.IReadOnlyList<(Int32 Index, String Name)> _HZBandWidthNames = new System.Collections.Generic.List<(Int32 Index, String Name)>();
        private System.Collections.Generic.IReadOnlyList<(Int32 Index, String Name)> _LZ1GBandWidthNames = new System.Collections.Generic.List<(Int32 Index, String Name)>();
        private System.Collections.Generic.IReadOnlyList<(Int32 Index, String Name)> _LZ2GBandWidthNames = new System.Collections.Generic.List<(Int32 Index, String Name)>();
        #endregion
        private AnaChnlTimebaseIndex _MinIndex1To1 = AnaChnlTimebaseIndex.Lv500p;
        private AnaChnlTimebaseIndex _MinIndex2To1 = AnaChnlTimebaseIndex.Lv200p;
        private AnaChnlTimebaseIndex _MinIndex4To1 = AnaChnlTimebaseIndex.Lv20p;
        private String _ProductModel = "";
        public MSO7000X()
        {
            BandWidthInit();
        }
        private void BandWidthInit()
        {
            _LZ1GBandWidthNames = new System.Collections.Generic.List<(Int32 Index, String Name)>()
                                {
                                    (0,""),
                                    (1,LZBANDWIDTH_LV1_NAME),
                                    (2,LZBANDWIDTH_LV2_NAME),
                                    (3,LZBANDWIDTH_LV3_NAME),
                                    (4,LZBANDWIDTH_LV4_NAME),
                                    (5,LZBANDWIDTH_LV5_NAME),
                                }.Where(x => !String.IsNullOrEmpty(x.Name)).ToList().AsReadOnly();

            _LZ2GBandWidthNames = new System.Collections.Generic.List<(Int32 Index, String Name)>()
                                {
                                    (0,LZBANDWIDTH_LV0_NAME),
                                    (1,LZBANDWIDTH_LV1_NAME),
                                    (2,LZBANDWIDTH_LV2_NAME),
                                    (3,LZBANDWIDTH_LV3_NAME),
                                    (4,LZBANDWIDTH_LV4_NAME),
                                    (5,LZBANDWIDTH_LV5_NAME),
                                 }.Where(x => !String.IsNullOrEmpty(x.Name)).ToList().AsReadOnly();

            _HZBandWidthNames = new System.Collections.Generic.List<(Int32 Index, String Name)>()
                                {
                                    (0,HZBANDWIDTH_LV0_NAME),
                                    (1,HZBANDWIDTH_LV1_NAME),
                                    (2,HZBANDWIDTH_LV2_NAME),
                                    (3,HZBANDWIDTH_LV3_NAME),
                                    (4,HZBANDWIDTH_LV4_NAME),
                                    (5,HZBANDWIDTH_LV5_NAME),
                                }.Where(x => !String.IsNullOrEmpty(x.Name)).ToList().AsReadOnly();
        }

        public IReadOnlyList<(Int32 Index, String Name)> GetBandWidthNames(Boolean is2G, AnaChnlCoupling anaChnlCoupling, Boolean active, Boolean isForcedRead)
        {
            if (anaChnlCoupling == AnaChnlCoupling.DC50)
            {
                if (is2G)
                {
                    return _LZ2GBandWidthNames;
                }
                else
                {
                    return _LZ1GBandWidthNames;
                }
            }
            else
            {
                return _HZBandWidthNames;
            }
        }

        public AnaChnlTimebaseIndex GetTimebaseMinIndex(AdcInterleaveMode adcInterleaveMode)
        {
            switch (adcInterleaveMode)
            {
                case AdcInterleaveMode.Mode4To1:
                    return _MinIndex4To1;
                case AdcInterleaveMode.Mode2To1:
                    return _MinIndex2To1;
                case AdcInterleaveMode.Mode1To1:
                    return _MinIndex1To1;
                default:
                    return _MinIndex4To1;
            }
        }

        public IReadOnlyList<KeyValuePair<String, Int32>> GetAnaChnlLengthSource(List<KeyValuePair<String, Int32>> source, Double timeScale, AdcInterleaveMode adcInterleaveMode)
        {
            if (source == null || source.Count == 0)
            {
                return source;
            }
            (Double MaxSamplingByus, Int32 MinDeepDots) maSampling_MinDeepDots = adcInterleaveMode switch
            {
                AdcInterleaveMode.Mode4To1 => (1e-4, 1250),
                AdcInterleaveMode.Mode2To1 => (2e-4, 1250),
                _ => (4e-4, 1250),
            };
            Int64 currDeepDots = (Int64)(timeScale * 10 / maSampling_MinDeepDots.MaxSamplingByus);
            if (currDeepDots < maSampling_MinDeepDots.MinDeepDots)
                currDeepDots = maSampling_MinDeepDots.MinDeepDots;
            else if (currDeepDots > source.Last().Value)
                currDeepDots = source.Last().Value;
            currDeepDots = (currDeepDots + 1250 - 1) / 1250 * 1250;
            source[0] = new(nameof(AnaChnlLengthOpt.Auto), (Int32)currDeepDots);
            return source.AsReadOnly();
        }

        public (Int32 MaxUv, Int32 MinUv) GetBiasRange(AnaChnlCoupling coupling, AnaChnlScaleIndex scale)
        {
            if (coupling == AnaChnlCoupling.DC50)
            {
                if (scale <= AnaChnlScaleIndex.Lv100m)
                {
                    return (2_000_000, -2_000_000);
                }
                else if (scale <= AnaChnlScaleIndex.Lv1)
                {
                    return (5_000_000, -5_000_000);
                }
                else
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs("MSO7000X DC50 Only Support <= 1V!", LogLevel.Info));
                    return (0, 0);
                }
            }
            else
            {
                if (scale <= AnaChnlScaleIndex.Lv50m)
                {
                    return (2_000_000, -2_000_000);
                }
                else if (scale <= AnaChnlScaleIndex.Lv500m)
                {
                    return (20_000_000, -20_000_000);
                }
                else if (scale <= AnaChnlScaleIndex.Lv1)
                {
                    return (40_000_000, -40_000_000);
                }
                else if (scale <= AnaChnlScaleIndex.Lv10)
                {
                    return (100_000_000, -100_000_000);
                }
                else
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs("MSO7000X 1M Only Support <= 10V!", LogLevel.Info));
                    return (0, 0);
                }
            }
        }

        public Boolean DoLAMutex()
        {
            return DsoPrsnt.DefaultDsoPrsnt.DoLAMutex();
        }

        public void SetBandWidthByInterleaveMode(AdcInterleaveMode adcInterleaveMode)
        {
            if (adcInterleaveMode == AdcInterleaveMode.Mode2To1 || adcInterleaveMode == AdcInterleaveMode.Mode1To1)
            {
                if (Constants.ENABLE_BANDWIDTH && DsoPrsnt.DefaultDsoPrsnt != null)
                {
                    foreach (var item in DsoPrsnt.DefaultDsoPrsnt.TryGetRange(c => c.Id.IsAnalog()).Cast<AnalogPrsnt>())
                    {
                        if (item.Coupling == AnaChnlCoupling.DC50 && item.Bandwidth == 0)
                        {
                            item.Bandwidth = item.BandWidthNames.ElementAt(1).Index;
                        }
                    }
                }
            }
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

        }

        public void TriggerTypeChanged(TriggerType type)
        {

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
                AdcInterleaveMode.Mode1To1 => 4,
                AdcInterleaveMode.Mode2To1 => 2,
                _ => 1,
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

            var count = (Int32)Math.Floor((Double)((Int64)triggerAddrStart! * 8 / cnt / chnlcnt));
            count = count >= Constants.SEGMENT_FRAME_COUNT_MAX ? Constants.SEGMENT_FRAME_COUNT_MAX : count;
            return count - 1;//最大存储深度下最后一段存在异常波形，需硬件排查
            //return (Int32)Math.Floor((Double)((Int64)0x1FF00000 * 64 / 8 / cnt / 4)) - 1;//减一：防止分段数据溢出 /*MaxSegmentCnt[AnaChnlLengthOpt.Of25KDots];*/
        }

        public IEnumerable<AnaChnlCoupling> GetSupportedCouplings(String serailNumber, AnaChnlStorageMode storageMode)
        {
            if (!serailNumber.Contains("X"))
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
            else
            {
                if (storageMode == AnaChnlStorageMode.Fast)
                {
                    return Enum.GetValues<AnaChnlCoupling>().Skip(2).Where(o => o != AnaChnlCoupling.Gnd);
                }
                else
                {
                    return Enum.GetValues<AnaChnlCoupling>().Skip(2);
                }
            }
        }

        public void LoadOriginSetting() { }

        public Int64 GetViewWaveDotsCnt(AdcInterleaveMode adcInterleaveMode)
        {
            return adcInterleaveMode switch
            {
                AdcInterleaveMode.Mode4To1 => 100_000,
                AdcInterleaveMode.Mode2To1 => 50_000,
                _ => 25_000,
            };
        }

        public String GetSNPrefix()
        {
            return "MX72";
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
                return "MSO7204X";
            }

            return "MSO7104X";
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
        private static extern IntPtr Crypt_7000X(String key, String optionname);

        public String? GetOptionActiveCode(String sn, String name)
        {
            return Marshal.PtrToStringAnsi(Crypt_7000X(sn, name));
        }
        public Boolean ShowWeakTip => true;

        public Boolean DefaultHighImpedance => true;

        public Boolean IncludeDigitalChnl => true;

        public Boolean EnableGetOrSetScreenBrightness => true;

        public Int32 JitterMaxDataLength => 10_000_000;

        public Boolean LimitBandwidth => Constants.ENABLE_BANDWIDTH;

        public Boolean NeedCheckBandwidth => true;

        public void ProcessDecodeData(ChannelId id, Int32 bitCount, ref DeocodeDataSourcePacket datasource, ref TwoLevelEdgeInfo root, ref TwoLevelEdgeInfo tempnode, ref Int32 nodeindex, CancellationToken token)
        {
            //整型数据源按位存储数据
            //开启单通道时，即实时数据源的所有数据为该通道的数据，4字节类是倒转存放的
            //开启1、2或者3、4通道时，每个通道取连续两个字节数据
            //其他情况，每个通道取一个字节数据，取下一个字节数据需偏移4个字节取数据
            var skipcount = 4;// datasource.InterwovenBitCount;// / 8 * datasource.Channels.Length;
            var startbyteindex = 0;// chindex * (datasource.InterwovenBitCount / 8);
            var bytecount = 0;
            var channels_length = datasource.Channels.Length;
            var bitcount_minus1 = bitCount - 1;  // 预计算 bitcount - 1，避免每次计算
            var channel_data_length = datasource.ChannelDataSource.Length;
            var maxbytecount = datasource.MaxByteCount;
            var laststatus = (datasource.ChannelDataSource[startbyteindex] >> (bitCount - 1)) & 0x01;
            var currentstatus = laststatus;

            for (Int32 index = startbyteindex; index < maxbytecount; index += skipcount)
            {
                // 提前检查是否越界
                if (index + bytecount - 1 >= channel_data_length)
                {
                    return;  // 如果越界，直接返回，避免进入无效的内层循环
                }

                for (Int32 byteindex = 0; byteindex < bytecount; byteindex++)
                {
                    // 提前检查是否越界
                    if (index + bytecount - 1 - byteindex >= channel_data_length)
                    {
                        return;  // 如果越界，退出
                    }

                    Byte byteval = datasource.ChannelDataSource[index + bytecount - 1 - byteindex];

                    // 提前计算节点索引
                    Int32 nodeindex_base = (index + byteindex - startbyteindex) * bitCount / channels_length;

                    // 内层 bit 操作
                    for (Int32 bytebitindex = 0; bytebitindex < bitCount; bytebitindex++)
                    {
                        currentstatus = byteval >> (bitcount_minus1 - bytebitindex) & 0x01;
                        nodeindex = nodeindex_base + bytebitindex;

                        // 只有状态发生变化时，才进行操作，避免不必要的 AddChild 调用
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

        }

        public Boolean KeyEnumCursor() => true;

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

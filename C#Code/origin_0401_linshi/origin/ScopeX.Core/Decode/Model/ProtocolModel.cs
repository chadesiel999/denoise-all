using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using SixLabors.ImageSharp.PixelFormats;

namespace ScopeX.Core.Decode
{
    /// <summary>
    /// 通道解码的基础model
    /// </summary>
    internal abstract class ProtocolModel : INotifyPropertyChanged
    {

        public ChannelId BusId { get; private set; }

        private Object _Locker = new Object();
        private protected Boolean _NeedDecodeData = true;
        private protected Boolean _NeedUpdateViewInfo = true;
        private Boolean _PropertyHasChanged = false;
        private Boolean _BufferFlag = false;
        protected void ChangeBuffer() => _BufferFlag = !_BufferFlag;
        public virtual UInt32 DataClockTimeByPs { get; } = (UInt32)(1f / Constants.PROT_SYS_CLOCK_HZ * 1E12);
        public virtual Double BitRateByPs => 1000;

        protected IntPtr _CancelFlagPtr = Marshal.AllocHGlobal(1);
        private Boolean _CancelFlag = false;
        public Boolean CancelFlag
        {
            get { return _CancelFlag; }
            set
            {
                _CancelFlag = value;
                Byte cancelstatus = _CancelFlag ? (Byte)1 : (Byte)0;
                Marshal.WriteByte(_CancelFlagPtr, cancelstatus);
            }
        }

        // 最大限制为200M
        public const Double MAX_DECODE_DOTS_COUNT = 250_000_000;

        static bool _DecodeTip = false;

        // 超过存储深度限制
        public static Boolean MoreThanStorage()
        {
            bool ss = DsoModel.Default.Timebase.StorageWaveDotsCnt > MAX_DECODE_DOTS_COUNT;
            if (ss && !_DecodeTip)
            {
                WeakTip.Default.Write("Decode", MsgTipId.DecodeNotSupportedMoreThan250MStorageDepth, emergent: false, "", 5);
            }
            _DecodeTip = ss;
            return ss;
        }

        private Boolean _DynamicThresholdRangeValid = false;
        //动态阈值有效
        protected Boolean DynamicThresholdRangeValid
        {
            get
            {
                if (_DynamicThresholdRangeMax == null || _DynamicThresholdRangeMin == null)
                {
                    return false;
                }
                if (_DynamicThresholdRangeMax.Length == 0 || _DynamicThresholdRangeMin.Length != _DynamicThresholdRangeMax.Length)
                {
                    return false;
                }
                for (Int32 i = 0; i < _DynamicThresholdRangeMax.Length; i++)
                {
                    if (_DynamicThresholdRangeMax[i] <= _DynamicThresholdRangeMin[i])
                    {
                        return false;
                    }
                }
                return _DynamicThresholdRangeValid;
            }
            set => _DynamicThresholdRangeValid = value;
        }
        //动态阈值范围 ljw 24.6
        protected Double[]? DynamicThresholdRangeMax { get => _DynamicThresholdRangeMax; set => _DynamicThresholdRangeMax = value; }
        protected Double[]? DynamicThresholdRangeMin { get => _DynamicThresholdRangeMin; set => _DynamicThresholdRangeMin = value; }

        private Double[]? _DynamicThresholdRangeMax;
        private Double[]? _DynamicThresholdRangeMin;

        //ljw 24.6 查询实际动态阈值范围
        protected Boolean GetDynamicThresholdRange(ChannelId id, out Double chnlMaxByMv, out Double chnlMinByMv)
        {

            chnlMaxByMv = 0;
            chnlMinByMv = 0;

            if (GetChannelScale(id, out Double scale) && TryGetAnalogChannelValRange(id, out chnlMaxByMv, out chnlMinByMv))
            {
                Double halfScale = scale / 2; //0.5div
                //峰峰值小于50mv 不做判断 过滤误差
                if (chnlMaxByMv - chnlMinByMv < 50)
                {
                    return false;
                }
                //对应fpga下边界不触发逻辑
                UInt32 negativeScaleCount = (UInt32)Math.Abs(chnlMinByMv / halfScale);

                if (negativeScaleCount < 1)
                {
                    chnlMaxByMv = 8 * halfScale;
                    chnlMinByMv = halfScale;
                }
                else
                {
                    chnlMaxByMv = 8 * halfScale;
                    chnlMinByMv = -(negativeScaleCount - 1) * halfScale;
                }

                if (chnlMaxByMv < chnlMinByMv)
                {
                    return false;
                }
                return true;
            }

            return false;

        }
        //ljw 24.6 判断阈值是否满足 实际动态阈值范围
        protected Boolean CheckDynamicThresholdRange(ChannelId id, Double thresholdByMv, out Double chnlMaxByMv, out Double chnlMinByMv)
        {
            chnlMaxByMv = 0;
            chnlMinByMv = 0;
            if (GetChannelScale(id, out Double scale) && TryGetAnalogChannelValRange(id, out chnlMaxByMv, out chnlMinByMv))
            {
                Double halfScale = scale / 2; //0.5div
                //峰峰值小于50mv 不做判断
                if (chnlMaxByMv - chnlMinByMv < 50)
                {
                    return true;
                }
                if (thresholdByMv - chnlMinByMv < halfScale || chnlMaxByMv - thresholdByMv < halfScale)
                {
                    return false;
                }
                return true;
            }

            return true;

        }
        //ljw 24.6 
        protected Boolean TryGetAnalogChannelValRange(ChannelId id, out Double analogChannelValMax, out Double analogChannelValMin)
        {
            analogChannelValMax = 0;
            analogChannelValMin = 0;
            //UInt16 tmpValMax = 0, tmpValMin = 0;
            if (id < ChannelId.C1 || id > ChannelId.C4 || DsoModel.Default.AnalogChnls == null)
            {
                return false;
            }
            var chnl = DsoModel.Default.AnalogChnls.FirstOrDefault(x => x.Id == id);
            if (chnl == null || chnl.Pack == null)
            {
                return false;
            }
            Double[,] datas = chnl.Pack.Buffer;
            var dataLength = datas.GetLength(1);
            if (dataLength <= 0)
            {
                return false;
            }
            for (Int32 i = 0; i < dataLength; i++)
            {
                var data = datas[0, i];
                if (data < analogChannelValMin)
                {
                    analogChannelValMin = data;
                }
                else if (data > analogChannelValMax)
                {
                    analogChannelValMax = data;
                }
            }

            return true;
        }

        public virtual IReadOnlyList<String> EventInfoTitles { get; } = new List<String>();
        private protected List<DecodeResultData> GetDecodeBuffer() => _BufferFlag ? _DecodePackets1 : _DecodePackets2;

        private List<DecodeResultData> _DecodePackets1 = new List<DecodeResultData>();
        private List<DecodeResultData> _DecodePackets2 = new List<DecodeResultData>();
        public IReadOnlyList<DecodeResultData> DecodePackets => (_BufferFlag ? _DecodePackets2 : _DecodePackets1).AsReadOnly();
        internal protected List<ProtocolEventInfo> _EventInfos = new List<ProtocolEventInfo>();
        public IReadOnlyList<ProtocolEventInfo> ProtocolEvents => _EventInfos.AsReadOnly();
        public ProtocolModel(ChannelId id, SerialProtocolType protocolType = SerialProtocolType.Close, Boolean isTrigger = false)
        {
            BusId = id;
            _ProtocolType = protocolType;
            IsTrigger = isTrigger;
        }
        public virtual ChannelId[] ActivedChannels => DsoModel.Default.AnalogChnls.Select(x => x.Id).Concat(DsoModel.Default.ReferenceChnls.Where(x => x.Active).Select(x => x.Id)).ToArray();
        private Boolean _IsTrigger = false;
        public Boolean IsTrigger
        {
            get => _IsTrigger;
            set
            {
                if (_IsTrigger != value)
                {
                    _IsTrigger = value;
                }
            }
        }
        //ljw 24.6
        protected Boolean GetChannelScale(ChannelId channelId, out Double scale)
        {
            if (!DsoModel.Default.TryGetChannel(channelId, out ChannelModel? channelModel) || channelModel is not AnalogModel analogModel)
            {
                scale = 0;
                return false;
            }
            scale = analogModel.Conditioning.GetScaleValue((Int32)analogModel.Conditioning.ScaleIndex, 0);
            return true;
        }

        private SerialProtocolType _ProtocolType = SerialProtocolType.Close;
        public SerialProtocolType ProtocolType
        {
            get { return _ProtocolType; }
            set
            {
                if (PlatformManager.Default.Platform.OptionProtocols.ContainsKey(value))
                {
                    var option = PlatformManager.Default.Platform.OptionProtocols[value];
                    if (!OptionsManager.Default.GetOptionAvailable(option))
                    {
                        WeakTip.Default.Write("Decode", MsgTipId.PurchaseOptions, duration: 4);
                        return;
                    }
                }

                if (_ProtocolType != value)
                {
                    UpdateProperty(ref _ProtocolType, value);
                }
            }
        }


        internal struct WfmInfo
        {
            public Double ScaleByPs;
            public Double PosIndex;
            public Double Start;
            public static Boolean operator ==(WfmInfo left, WfmInfo right)
            {
                return left.PosIndex.Equals(right.PosIndex) && left.ScaleByPs.Equals(right.ScaleByPs) && left.Start.Equals(right.Start);
            }
            public static Boolean operator !=(WfmInfo left, WfmInfo right)
            {
                return !left.PosIndex.Equals(right.PosIndex) || !left.ScaleByPs.Equals(right.ScaleByPs) || !left.Start.Equals(right.Start);
            }
            public override Boolean Equals([NotNullWhen(true)] Object? obj)
            {
                if (obj is WfmInfo info)
                {
                    return this == info;
                }
                return false;
            }
            public override Int32 GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private Int64[] _LastStamp = new Int64[1];
        private WfmInfo[] _LastWfmInfos = new WfmInfo[0];
        protected Double GetTimeFromPosition(Double position = 5000, Int32 chindex = 0)
        {
            position -= _LastWfmInfos[chindex].PosIndex;
            return position / ComModel.Constants.IDX_PER_XDIV * _LastWfmInfos[chindex].ScaleByPs;
        }
        internal void DecodePacketData(WfmInfo[] wfmInfos, ref CancellationToken token)
        {
            //if (IsTrigger) return;
            var actchs = ActivedChannels;
            Int32 chcount = actchs.Length;
            if (!SourceHasData())
            {
                GetDecodeBuffer().Clear();
                ChangeBuffer();
                return;
            }


            if (_LastWfmInfos.Length < wfmInfos.Length)
            {
                _LastWfmInfos = new WfmInfo[wfmInfos.Length];
            }

            if (Enumerable.Range(0, wfmInfos.Length).Any(x => _LastWfmInfos[x] != wfmInfos[x]))
            {
                _NeedUpdateViewInfo = true;
                Array.Copy(wfmInfos, _LastWfmInfos, wfmInfos.Length);
            }
            lock (_Locker)
            {
                if (CheckUpdate(ref _LastStamp[0]))
                {
                    ClearDecodeBuffer();
                }
                else
                {
                    _NeedDecodeData = _PropertyHasChanged;
                }
                ParsingData(ref token);
                if (_PropertyHasChanged)
                    _PropertyHasChanged = false;
            }
        }

        public void ClearData()
        {
            if (_DecodePackets1 != null && _DecodePackets1.Count > 0)
                _DecodePackets1[^1].DecodeViewInfos = new IDecodeViewInfo[0];
            if (_DecodePackets2 != null && _DecodePackets2.Count > 0)
                _DecodePackets2[^1].DecodeViewInfos = new IDecodeViewInfo[0];
            _EventInfos.Clear();
        }

        internal static Double TryGetChannelGain(ChannelId channelId)
        {
            if (!TryGetChannelGain(channelId, out Double gain))
            {
                return 1;
            }
            return gain;
        }
        internal static Boolean TryGetChannelGain(ChannelId channelId, out Double gain)
        {
            gain = 1;
            if (!DsoModel.Default.TryGetChannel(channelId, out ChannelModel? channelModel) || channelModel is not AnalogModel analogModel)
            {
                return false;
            }
            gain = analogModel.Conditioning.ProbeGain * analogModel.Conditioning.ProbeUnitRatio;
            return true;
        }


        public static String GetChannelUnit(ChannelId id)
        {
            var unittxt = "V";
            IChnlPrsnt chnlPrsnt = null;
            if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(id, out chnlPrsnt))
            {
                unittxt = chnlPrsnt.Unit;
            }

            return unittxt;
        }

        private void ClearDecodeBuffer()
        {
            _NeedDecodeData = true;
            _NeedUpdateViewInfo = true;
        }

        internal virtual Boolean SourceHasData()
        {
            return DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].HasData;
        }

        internal virtual List<ChannelId> GetDecodeSources()
        {
            return new List<ChannelId>();
        }

        internal virtual Boolean CheckUpdate(ref Int64 laststamp)
        {
            if (laststamp != DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp)
            {
                laststamp = DecodeDataHelper.Instance.AnalogDataSources[BusId - ChannelId.B1].TimeStamp;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 解析数据包中的数据
        /// </summary>
        internal virtual void ParsingData(ref CancellationToken token)
        {
            _DynamicThresholdRangeMax = null;
            _DynamicThresholdRangeMin = null;
        }
        /// <summary>
        /// for test
        /// </summary>
        public void ParsingData()
        {
            CancellationTokenSource source = new CancellationTokenSource();

            ParsingData(ref Unsafe.AsRef(source.Token));
        }
        #region INotifyPropertyChanged

        protected PropertyChangedEventHandler? _PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            //DecodeProtocolShareParameter.Default.OnPropertyChanged(this, propertyName);
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void UpdateProperty<T>(ref T properValue, T newValue, [CallerMemberName] String propertyName = "")
        {
            if (Equals(properValue, newValue))
                return;
            _PropertyHasChanged = true;
            DecodeProtocolShareParameter.Default.UpdateProperty(this, ref properValue, newValue, propertyName);
            Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);//实时通道
            UpdateReferenceDataStatus();
            OnPropertyChanged(propertyName);
        }

        public virtual void UpdateReferenceDataStatus()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
                DecodeProtocolShareParameter.Default.PropertyChanged += value;
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
                DecodeProtocolShareParameter.Default.PropertyChanged -= value;
            }
        }

        #endregion

        public virtual HdMessage.IDecoderOptions? GetProtocolRecoder()
        {
            return null;
        }
        private protected Single CalcPosition(Int64 pos, ChannelId ch = ChannelId.C1, Int32 chindex = 0)
        {
            Int64 trigindex = 0;
            Double sampleRate = 0;
            DecodeDataHelper.Instance.TryGetTriggerIndex(BusId, ch, ref trigindex);

            // 临时修改方案，解决在获取采样率失败的情况下，被除数为0，引发异常
            if (!DecodeDataHelper.Instance.TryGetSampleRate(BusId, ch, ref sampleRate))
                return (Single)0.01;
            var len = (decimal)pos - (decimal)trigindex;
            var virtuallen = ((decimal)len / (decimal)sampleRate) / ((decimal)_LastWfmInfos[chindex].ScaleByPs * (decimal)1E-12) * (decimal)Constants.IDX_PER_XDIV;
            return (Single)(virtuallen + (decimal)_LastWfmInfos[chindex].PosIndex);
        }

        private protected Single CalcBitLenght(Int32 lenght, ChannelId ch = ChannelId.C1, Int32 chindex = 0)
        {
            Double sampleRate = 0;
            if(DecodeDataHelper.Instance.TryGetSampleRate(BusId, ch, ref sampleRate))
            {
                var result = (((decimal)lenght / (decimal)sampleRate) / ((decimal)_LastWfmInfos[chindex].ScaleByPs * (decimal)1E-12) * (decimal)Constants.IDX_PER_XDIV);
                return (Single)result;
            }

            return Single.NaN;
        }
        private protected Int32 GetChIndex(ChannelId channelId)
        {
            return Array.FindIndex(ActivedChannels, x => x == channelId);
        }

    }
}

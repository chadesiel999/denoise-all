using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using ScopeX.MathExt;
using System.Numerics;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using System.Drawing;

namespace ScopeX.Core
{
    internal class MultiDomainModel : INotifyPropertyChanged
    {
        internal MultiDomainModel()
        {
            _FigureDefine.Add(MultiDomainFigureEnum.AmpVsTime, new MDGraphModel(MultiDomainFigureEnum.AmpVsTime.ToString()));
            _FigureDefine.Add(MultiDomainFigureEnum.PhaseVsTime, new MDGraphModel(MultiDomainFigureEnum.PhaseVsTime.ToString()));
            _FigureDefine.Add(MultiDomainFigureEnum.FreqVsTime, new MDGraphModel(MultiDomainFigureEnum.FreqVsTime.ToString()));

            _FigureDefine.Add(MultiDomainFigureEnum.AmpleVsFreq, new AVFGraphModel(MultiDomainFigureEnum.AmpleVsFreq.ToString()));
            _FigureDefine.Add(MultiDomainFigureEnum.PhaseVsFreq, new PVFGraphModel(MultiDomainFigureEnum.PhaseVsFreq.ToString()));

            _FigureDefine.Add(MultiDomainFigureEnum.Waterfalls, new WaterFallsGraphModel(MultiDomainFigureEnum.Waterfalls.ToString()));
            _FigureDefine.Add(MultiDomainFigureEnum.Spectrogram, new SpectrogramGraphModel(MultiDomainFigureEnum.Spectrogram.ToString()));
        }

        internal List<List<double>> WaterFallsBuffer = new List<List<Double>>();
        internal double[,] WaterFallsBufferArray;
        internal double[,] WaterFallsBuffer_test = new double[15, 1024];
        internal List<Double[]> ThreeDimensionalBuffer = new List<Double[]>();

        private Double _TranslateSampleRate;
        public Double TranslateSampleRate { get => _TranslateSampleRate; }

        //private Int64 _MaxSampleRate = 0;
        internal Int64 MaxSampleRate
        {
            get => DsoModel.Default.Timebase.Scale > 0.01 ? 6_000000000 : 20_000000000;
            //set
            //{
            //    if (_MaxSampleRate != value)
            //    {
            //        _MaxSampleRate = value;
            //        OnPropertyChanged();
            //    }
            //}
        }

        private Boolean _Active = false;
        internal Boolean Active
        {
            get => _Active;
            set
            {
                if (_Active != value)
                {
                    _Active = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _ThreeDimensionalEnable = false;
        internal Boolean ThreeDimensionalEnable
        {
            get => _ThreeDimensionalEnable;
            set
            {
                if (_ThreeDimensionalEnable != value)
                {
                    _ThreeDimensionalEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int64? _ThreeDimensionalWindowsId = 0;
        internal Int64? ThreeDimensionalWindowsId
        {
            get => _ThreeDimensionalWindowsId;
            set
            {
                if (_ThreeDimensionalWindowsId != value)
                {
                    _ThreeDimensionalWindowsId = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _ThreeDimensionalBuildCnt = 0;

        internal Int32 ThreeDimensionalBuildCnt
        {
            get => _ThreeDimensionalBuildCnt;
            set
            {
                if (_ThreeDimensionalBuildCnt != value)
                {
                    _ThreeDimensionalBuildCnt = value;
                }
            }
        }

        private Boolean _SynchronizationEnable = false;
        internal Boolean SynchronizationEnable
        {
            get => _SynchronizationEnable;
            set
            {
                if (_SynchronizationEnable != value)
                {
                    _SynchronizationEnable = value;
                    if (_SynchronizationEnable)
                    {
                        ParameterTuningEnable = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private Double _ZoomStart = 0;
        internal Double ZoomStart
        {
            get => _ZoomStart;
            set
            {
                if (_ZoomStart != value)
                {
                    _ZoomStart = value;
                }
            }
        }

        private Double _ZoomLength = 0;
        internal Double ZoomLength
        {
            get => _ZoomLength;
            set
            {
                if (_ZoomLength != value)
                {
                    _ZoomLength = value;
                    OnPropertyChanged();
                }
            }
        }

        internal RectangleF ValidArea(RectangleF rect)
        {
            if (!SynchronizationEnable)
            {
                return rect;
            }

            RectangleF res = new RectangleF();
            res.Y = rect.Y;
            res.Height = rect.Height;

            Double delay = ((5000 - DsoModel.Default.Timebase.PosIndex) / 1000) * DsoModel.Default.Timebase.Scale * 1000;
            Double starttmp = (((0 - (5000 - rect.X) / 1000)) * DsoModel.Default.Timebase.Scale) * 1000 + delay;
            Double lengthtmp = (((Double)rect.Width / Constants.IDX_PER_XDIV) * DsoModel.Default.Timebase.Scale) * 1e3;

            (Double start, Double length) = Hd.GetValidRect(starttmp, lengthtmp);
            res.X = (Single)(5000 - (0 - ((start - delay) / 1e3 / DsoModel.Default.Timebase.Scale)) * 1e3);
            res.Width = (Single)(length / 1e3 / DsoModel.Default.Timebase.Scale * 1e3);

            return res;
        }

        private Boolean _ParameterTuningEnable = false;
        internal Boolean ParameterTuningEnable
        {
            get => _ParameterTuningEnable;
            set
            {
                if (_ParameterTuningEnable != value)
                {
                    _ParameterTuningEnable = value;
                    if (_ParameterTuningEnable)
                    {
                        SynchronizationEnable = false;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private ChannelId _Source = ChannelId.C1;
        internal ChannelId Source
        {
            get => _Source;
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    OnPropertyChanged();
                }
            }
        }

        private LogarithmUnit _PUnit = LogarithmUnit.dBm;
        internal LogarithmUnit PUnit
        {
            get => _PUnit;
            set
            {
                if (_PUnit != value)
                {
                    _PUnit = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _NormalLineActive = false;
        internal Boolean NormalLineActive
        {
            get => _NormalLineActive;
            set
            {
                if (_NormalLineActive != value)
                {
                    _NormalLineActive = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _MaxHoldLineActive = false;
        internal Boolean MaxHoldLineActive
        {
            get => _MaxHoldLineActive;
            set
            {
                if (_MaxHoldLineActive != value)
                {
                    _MaxHoldLineActive = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _MinHoldLineActive = false;
        internal Boolean MinHoldLineActive
        {
            get => _MinHoldLineActive;
            set
            {
                if (_MinHoldLineActive != value)
                {
                    _MinHoldLineActive = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _AverageLineActive = false;
        internal Boolean AverageLineActive
        {
            get => _AverageLineActive;
            set
            {
                if (_AverageLineActive != value)
                {
                    _AverageLineActive = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _AverageCount = 10;
        internal Int32 AverageCount
        {
            get => _AverageCount;
            set
            {
                if (_AverageCount != value)
                {
                    _AverageCount = value;
                    OnPropertyChanged();
                }
            }
        }

        private RFWindowType _WindowType = RFWindowType.Rectangle;
        internal RFWindowType WindowType
        {
            get => _WindowType;
            set
            {
                if (value != _WindowType)
                {
                    _WindowType = value;
                    OnPropertyChanged();
                }
            }
        }

        private Double _SampleRateByHz = 1e9;

        private Double _RBWByHz = Constants.RF_RBW_MIN;
        internal Double RBWByHz
        {
            get
            {
                if (SynchronizationEnable)
                {
                    return (wfminfo?.SampleRateHardware ?? _SampleRateByHz) / Hd.GetAcutalFFTLength();
                }
                return (wfminfo?.SampleRateHardware ?? _SampleRateByHz) / _FFTLength;
            }
            set
            {
                if (_RBWByHz != value)
                {
                    //_FFTLength = GetValidFFTLength((Int32)(_SampleRateByHz / value));
                    //_RBWByHz = (wfminfo?.SampleRateHardware ?? _SampleRateByHz) / _FFTLength;
                    OnPropertyChanged();
                }
            }
        }

        private Int64 _FFTLength = 8192;
        internal Int64 FFTLength
        {
            get => _FFTLength;
            set
            {
                if (_FFTLength != value)
                {
                    _FFTLength = GetValidFFTLength(value);
                    _RBWByHz = (wfminfo?.SampleRateHardware ?? _SampleRateByHz) / _FFTLength;
                    OnPropertyChanged();
                }
            }
        }

        private Int64 GetValidFFTLength(Int64 fftLength)
        {

            return fftLength;
        }

        private Int64 _STFTLength = 1024;
        internal Int64 STFTLength
        {
            get => _STFTLength;
            set
            {
                if (_STFTLength != value)
                {
                    _STFTLength = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int64 _STFTStep = 1024;
        internal Int64 STFTStep
        {
            get => _STFTStep;
            set
            {
                if (_STFTStep != value)
                {
                    _STFTStep = value;
                    OnPropertyChanged();
                }
            }
        }

        private Double _TimeStep = 1;
        internal Double TimeStep
        {
            get => _TimeStep;
            set
            {
                if (_TimeStep != value)
                {
                    _TimeStep = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _SpanOptForTimeFreq = 0;
        internal Int32 SpanOptForTimeFreq
        {
            get => _SpanOptForTimeFreq;
            set
            {
                if (_SpanOptForTimeFreq != value)
                {
                    _SpanOptForTimeFreq = value;
                    if (_TimeScaleForTimeFreq < MinTimeScale)
                    {
                        _TimeScaleForTimeFreq = MinTimeScale;
                    }

                    if (_TimeScaleForTimeFreq > MaxTimeScale)
                    {
                        _TimeScaleForTimeFreq = MaxTimeScale;
                    }
                    OnPropertyChanged();
                }
            }
        }

        internal Int64 SpanValueForTimeFreq
        {
            get
            {
                if (_SpanListForTimeFreq.Count > _SpanOptForTimeFreq)
                    return _SpanListForTimeFreq[_SpanOptForTimeFreq];
                return 0;
            }
        }

        private List<Int64> _SpanListForTimeFreq = new();
        internal void UpdateSpanListForTimeFreq()
        {
            _SpanListForTimeFreq.Clear();
            var spanlist = Hd.GetSpanListForTimeFreq(SpanFreqByHz);
            spanlist.Sort();
            spanlist.Reverse();
            _SpanListForTimeFreq.AddRange(spanlist);
        }

        internal List<Int64> SpanListForTimeFreq => _SpanListForTimeFreq.ToList();

        private Double _TimeScaleForTimeFreq = 0;
        internal Double TimeScaleForTimeFreq
        {
            get => ValidateLimitedValue(_TimeScaleForTimeFreq, MinTimeScale, MaxTimeScale);
            set
            {
                if (value != _TimeScaleForTimeFreq && !SynchronizationEnable)
                {
                    _TimeScaleForTimeFreq = value;
                    _TimeScaleForTimeFreq = ValidateLimitedValue(_TimeScaleForTimeFreq, MinTimeScale, MaxTimeScale);
                    OnPropertyChanged();
                }
            }
        }

        internal Double MinTimeScale
        {
            get
            {
                // 最小步进
                return 8.0 / (Double)SpanValueForTimeFreq;
            }
        }

        internal Double MaxTimeScale
        {
            get
            {
                return DsoModel.Default.Timebase.StorageWaveDotsCnt / (Double)Constants.SAMPLING_RATE / 512;
            }
        }

        private UInt32 _RoughSpecCnt = 0;
        internal UInt32 RoughSpecCnt
        {
            get => _RoughSpecCnt;
            set => _RoughSpecCnt = value;
        }

        #region 中心频率和起止频率的范围限制 需要优化

        private Int64 _CenterFreqByHz = Constants.RF_END_FREQUENCY_MAX / 2;
        internal Int64 CenterFreqByHz
        {
            get => _CenterFreqByHz;
            set
            {
                if (_CenterFreqByHz != value)
                {
                    //_CenterFreqByHz = value;
                    _CenterFreqByHz = ValidateLimitedValue(value, Constants.RF_CENTER_FREQUENCY_MIN, Constants.RF_CENTER_FREQUENCY_MAX);
                    ParameterSynchronization("CenterFreqByHz");
                    OnPropertyChanged();
                }
            }
        }

        private Int64 _StartFreqByHz = Constants.RF_START_FREQUENCY_MIN;
        internal Int64 StartFreqByHz
        {
            get => _StartFreqByHz;
            set
            {
                if (_StartFreqByHz != value)
                {
                    //_StartFreqByHz = value;
                    _StartFreqByHz = ValidateLimitedValue(value, Constants.RF_START_FREQUENCY_MIN, Constants.RF_START_FREQUENCY_MAX);
                    ParameterSynchronization("StartFreqByHz");
                    OnPropertyChanged();
                }
            }
        }

        private Int64 _EndFreqByHz = Constants.RF_END_FREQUENCY_MAX;
        internal Int64 EndFreqByHz
        {
            get => _EndFreqByHz;
            set
            {
                if (_EndFreqByHz != value)
                {
                    //_EndFreqByHz = value;
                    _EndFreqByHz = ValidateLimitedValue(value, Constants.RF_END_FREQUENCY_MIN, Constants.RF_END_FREQUENCY_MAX);
                    ParameterSynchronization("EndFreqByHz");
                    OnPropertyChanged();
                }
            }
        }

        private Int64 _SpanFreqByHz = Constants.RF_SPAN_MAX;
        internal Int64 SpanFreqByHz
        {
            get => _SpanFreqByHz;
            set
            {
                if (_SpanFreqByHz != value)
                {
                    //_SpanFreqByHz = value;
                    _SpanFreqByHz = ValidateLimitedValue(value, MinSpanFreq, MaxSpanFreq);
                    ParameterSynchronization("SpanFreqByHz");
                    UpdateSpanListForTimeFreq();
                    OnPropertyChanged();
                }
            }
        }

        internal Int64 MaxSpanFreq
        {
            get
            {
                //return Constants.RF_SPAN_MAX;
                return DsoModel.Default.Timebase.Scale > 0.01 ? 6_000000000 : 20_000000000;
            }
        }

        internal Int64 MinSpanFreq
        {
            get
            {
                if (!SynchronizationEnable)
                    return Constants.RF_SPAN_MIN;
                var scale = DsoModel.Default.Timebase.Scale / 1e6; // us -> s
                var dotcnt = Constants.SAMPLING_RATE * scale * Constants.VIS_XDIVS_NUM;
                var maxextramnum = dotcnt / FFTLength;
                return Hd.GetValidMaxSpanFreq(maxextramnum);
            }
        }

        #endregion

        #region DataValidation
        private Int64 ValidateLimitedValue(Int64 value, Int64 min, Int64 max)
        {
            if (value > max)
            {
                value = max;
                return value;
            }
            if (value < min)
            {
                value = min;
                return value;
            }
            return value;
        }

        private Double ValidateLimitedValue(Double value, Double min, Double max)
        {
            if (value > max)
            {
                value = max;
                return value;
            }
            if (value < min)
            {
                value = min;
                return value;
            }
            return value;
        }

        private void ParameterSynchronization(String param)
        {
            switch (param)
            {
                case "StartFreqByHz":
                    {
                        _SpanFreqByHz = _EndFreqByHz - _StartFreqByHz;
                        if (_EndFreqByHz - _StartFreqByHz < Constants.RF_SPAN_MIN)
                        {
                            _SpanFreqByHz = Constants.RF_SPAN_MIN;
                            _EndFreqByHz = _StartFreqByHz + _SpanFreqByHz;
                        }
                        if (EndFreqByHz - _StartFreqByHz > Constants.RF_SPAN_MAX)
                        {
                            _SpanFreqByHz = Constants.RF_SPAN_MAX;
                            _EndFreqByHz = _StartFreqByHz + _SpanFreqByHz;
                        }
                        _CenterFreqByHz = _StartFreqByHz + _SpanFreqByHz / 2;
                    }
                    break;
                case "EndFreqByHz":
                    {
                        _SpanFreqByHz = _EndFreqByHz - _StartFreqByHz;
                        if (_EndFreqByHz - _StartFreqByHz < Constants.RF_SPAN_MIN)
                        {
                            _SpanFreqByHz = Constants.RF_SPAN_MIN;
                            _StartFreqByHz = _EndFreqByHz - _SpanFreqByHz;
                        }
                        if (EndFreqByHz - _StartFreqByHz > Constants.RF_SPAN_MAX)
                        {
                            _SpanFreqByHz = Constants.RF_SPAN_MAX;
                            _StartFreqByHz = _EndFreqByHz - _SpanFreqByHz;
                        }
                        _CenterFreqByHz = _StartFreqByHz + _SpanFreqByHz / 2;
                    }
                    break;
                case "CenterFreqByHz":
                    {
                        _StartFreqByHz = _CenterFreqByHz - _SpanFreqByHz / 2;
                        _EndFreqByHz = _CenterFreqByHz + _SpanFreqByHz / 2;
                        if (_StartFreqByHz < Constants.RF_START_FREQUENCY_MIN)
                        {
                            _StartFreqByHz = Constants.RF_START_FREQUENCY_MIN;
                            _EndFreqByHz = 2 * _CenterFreqByHz;
                            _SpanFreqByHz = _EndFreqByHz - _StartFreqByHz;
                        }
                        if (_EndFreqByHz > Constants.RF_END_FREQUENCY_MAX)
                        {
                            _EndFreqByHz = Constants.RF_END_FREQUENCY_MAX;
                            _StartFreqByHz = _EndFreqByHz - 2 * (_EndFreqByHz - _CenterFreqByHz);
                            _SpanFreqByHz = _EndFreqByHz - _StartFreqByHz;
                        }
                    }
                    break;
                case "SpanFreqByHz":
                    {
                        _StartFreqByHz = _CenterFreqByHz - _SpanFreqByHz / 2;
                        _EndFreqByHz = _CenterFreqByHz + _SpanFreqByHz / 2;
                        if (_StartFreqByHz < Constants.RF_START_FREQUENCY_MIN)
                        {
                            _StartFreqByHz = Constants.RF_START_FREQUENCY_MIN;
                            _EndFreqByHz = _StartFreqByHz + _SpanFreqByHz;
                            _CenterFreqByHz = _StartFreqByHz + _SpanFreqByHz / 2;
                        }
                        if (_EndFreqByHz > Constants.RF_END_FREQUENCY_MAX)
                        {
                            _EndFreqByHz = Constants.RF_END_FREQUENCY_MAX;
                            _StartFreqByHz = _EndFreqByHz - _SpanFreqByHz;
                            _CenterFreqByHz = _StartFreqByHz + _SpanFreqByHz / 2;
                        }
                    }
                    break;
                default:
                    break;
            }
            _TranslateSampleRate = RFSampInfo.GetTranslateSampleRate((int)_SpanFreqByHz, _Source);

            //_RBW = GetRBW();
            //_FrequencyScale = _Span / Constants.VIS_XDIVS_NUM;
            //_FigureCenterFrequency = _CenterFrequency;
            //_FigureStartFrequency = _FigureCenterFrequency - Constants.VIS_XDIVS_NUM / 2 * _FrequencyScale;
            //_FigureEndFrequency = _FigureCenterFrequency + Constants.VIS_XDIVS_NUM / 2 * _FrequencyScale;
            //Init = true;
        }
        #endregion

        private Dictionary<MultiDomainFigureEnum, MDGraphModel> _FigureDefine = new();

        private MultiDomainFigureEnum _CurFigureType = MultiDomainFigureEnum.AmpleVsFreq;
        internal MultiDomainFigureEnum CurFigureType
        {
            get => _CurFigureType;
            set
            {
                if (_CurFigureType != value)
                {
                    _CurFigureType = value;
                    OnPropertyChanged();
                }
            }
        }

        internal MDGraphModel? GetFigureMathModel(MultiDomainFigureEnum figureType)
        {
            return _FigureDefine.ContainsKey(figureType) ? _FigureDefine[figureType] : null;
        }

        internal Boolean CurFigureEnable
        {
            get => GetFigureMathModel(_CurFigureType)?.Enabled ?? false;
            set
            {
                if (_FigureDefine.ContainsKey(_CurFigureType) && _FigureDefine[_CurFigureType].Enabled != value)
                {
                    SetFigureEnable(_CurFigureType, value);
                    OnPropertyChanged();
                }
            }
        }

        private void SetFigureEnable(MultiDomainFigureEnum figureType, Boolean enableState)
        {
            if (enableState)
            {
                foreach (var chnlid in ChannelIdExt.GetMDMaths())
                {
                    if (DsoModel.Default.TryGetChannel(chnlid, out var mathmodel) && (mathmodel != null) && (mathmodel is MathModel))
                    {
                        if (((MathModel)mathmodel).Args?.Occupier == null && _FigureDefine.ContainsKey(figureType))
                        {
                            _FigureDefine[figureType].MathChannelId = chnlid;
                            ((MathModel)mathmodel).GetOrMakeArg?.Invoke(MathType.Custom);
                            _FigureDefine[figureType].Enabled = enableState;
                            return;
                        }
                    }
                }
            }
            else
            {
                if (_FigureDefine.ContainsKey(figureType))
                {
                    _FigureDefine[figureType].Enabled = enableState;
                }
            }
        }

        internal WfmMdInfo? wfminfo = null;

        private Int64 _SpanParamTuning = 8_000_000_000;
        private Int64 _CenterFreqParamTuning = 4_000_000_000;
        private Int64 _StateParamTuning = 0;

        private Int64 _SpanSync = 8_000_000_000;
        private Int64 _FFTLengthSync = 1024;

        private Double _LastSyncRbw = 0;

        internal int _specdatarows = 512;
        internal int _specdatacolumns = 512;
        internal double[,] SpectrogramBufferArray = new Double[512, 512];
        private bool _specNormInitialized = false;
        private double _specNormMin = 0.0;
        private double _specNormMax = 1.0;
        private const double _specNormLowQuantile = 0.02;
        private const double _specNormHighQuantile = 0.98;
        private const double _specNormSmoothing = 0.18;
        private const double _specGamma = 0.85;

        /// <summary>
        /// 读取IQ数据，根据打开的图像设置，对数据进行转换，provide到数学通道的缓存区
        /// </summary>
        internal void UpdateWfm()
        {
            if (!Active)
            {
                return;
            }
            List<Double> ifft = new List<Double>();
            List<Double> qfft = new List<Double>();
            List<Double> avtbuffer = new List<Double>();
            List<Double> pvtbuffer = new List<Double>();
            List<Double> specbuffer = new List<Double>();

            //_LastSyncRbw = RBWByHz;
            if (_LastSyncRbw != RBWByHz)
            {
                OnPropertyChanged(nameof(RBWByHz));
                _LastSyncRbw = RBWByHz;
            }

            var iflag = Hd.TryGetData(ChannelType.RadioFrequency, MultiDomainDataTypeEnum.IFFT, out Object? idata);
            if (String.IsNullOrEmpty(iflag) && idata != null && idata is List<Double>)
            {
                ifft.AddRange((List<Double>)idata);
            }

            var qflag = Hd.TryGetData(ChannelType.RadioFrequency, MultiDomainDataTypeEnum.QFFT, out Object? qdata);
            if (String.IsNullOrEmpty(iflag) && qdata != null && qdata is List<Double>)
            {
                qfft.AddRange((List<Double>)qdata);
            }

            var avtflag = Hd.TryGetData(ChannelType.RadioFrequency, MultiDomainDataTypeEnum.AmpVSTime, out Object? avtdata);
            if (String.IsNullOrEmpty(iflag) && avtdata != null && avtdata is List<Double>)
            {
                avtbuffer.AddRange((List<Double>)avtdata);
            }

            var pvtflag = Hd.TryGetData(ChannelType.RadioFrequency, MultiDomainDataTypeEnum.PhaseVSTime, out Object? pvtdata);
            if (String.IsNullOrEmpty(iflag) && pvtdata != null && pvtdata is List<Double>)
            {
                pvtbuffer.AddRange((List<Double>)pvtdata);
            }

            Double[,] specbufferarray = new Double[0, 0];
            var specflag = Hd.TryGetData(ChannelType.RadioFrequency, MultiDomainDataTypeEnum.Spectrogram, out Object? specdata);
            if (String.IsNullOrEmpty(specflag) && specdata != null && specdata is List<Double>)
            {
                specbuffer.AddRange((List<Double>)specdata);
                specbufferarray = ConvertTo2DArray(specbuffer);
            }
            else if (String.IsNullOrEmpty(specflag) && specdata != null && specdata is Double[,])
            {
                var raw = (Double[,])specdata;
                specbufferarray = NormalizeAndShiftSpectrogram(raw);
            }

            if (specbufferarray.Length > 0)
            {
                int rows = specbufferarray.GetLength(0);
                int columns = specbufferarray.GetLength(1);
                if (rows != _specdatarows || columns != _specdatacolumns)
                {
                    _specdatarows = rows;
                    _specdatacolumns = columns;
                    SpectrogramBufferArray = new Double[_specdatarows, _specdatacolumns];
                }
                SpectrogramBufferArray = (Double[,])specbufferarray.Clone();
            }

            // 横坐标的范围和刻度
            var paramsflag = Hd.TryGetData(ChannelType.RadioFrequency, MultiDomainDataTypeEnum.WfmParams, out Object? paramsdata);
            if (String.IsNullOrEmpty(iflag) && paramsdata != null && paramsdata is WfmMdInfo)
            {
                wfminfo = (WfmMdInfo)paramsdata;
            }

            // 模拟数据
            List<Double> testdata = new List<Double>(Enumerable.Range(1, 1024).Select(i => (double)i));

            List<Double> avfbuffer = new List<Double>();
            List<Double> pvfbuffer = new List<Double>();
            List<Double> fvtbuffer = new List<Double>();
            //List<List<double>> waterfallsbuffer = new List<List<Double>>();

            IQToAmplitude(ifft, qfft, avfbuffer);
            IQToPhase(ifft, qfft, pvfbuffer);
            PhaseToFrequency(pvtbuffer, 1.0 / (wfminfo?.SampleRateHardware ?? 1.0), fvtbuffer);//lhctest
            RescaleAmp(avfbuffer);

            //if(wfminfo?.SpanHardware ?? 1.0)

            CutData(avfbuffer, SpanFreqByHz, wfminfo?.SpanHardware ?? 8_000_000_000, out List<Double>? dataAfterCut);
            RfreshData(dataAfterCut, true);

            //var transposedBuffer = Enumerable.Range(0, WaterFallsBuffer[0].Count) // 获取列数
            //                                        .Select(colIndex => WaterFallsBuffer.Select(row => row[colIndex]).ToList()) // 获取每一列作为新的行
            //                                        .ToList();

            WaterFallsBufferArray = ConvertTo2DArray(WaterFallsBuffer);
            OccupierBuffer.Default.Provide(nameof(MultiDomainFigureEnum.AmpleVsFreq), new MathExt.Vector(avfbuffer.ToArray(), "Hz", "dBm", wfminfo?.SampleRateHardware ?? 1.0, 1.0));
            OccupierBuffer.Default.Provide(nameof(MultiDomainFigureEnum.PhaseVsFreq), new MathExt.Vector(pvfbuffer.ToArray(), "Hz", "°", wfminfo?.SampleRateHardware ?? 1.0, 1.0));
            OccupierBuffer.Default.Provide(nameof(MultiDomainFigureEnum.AmpVsTime), new MathExt.Vector(avtbuffer.ToArray(), "s", "V", 1.0 / (wfminfo?.SampleRateHardware ?? 1.0), 1.0));
            OccupierBuffer.Default.Provide(nameof(MultiDomainFigureEnum.FreqVsTime), new MathExt.Vector(fvtbuffer.ToArray(), "s", "Hz", 1.0 / (wfminfo?.SampleRateHardware ?? 1.0), 1.0));
            OccupierBuffer.Default.Provide(nameof(MultiDomainFigureEnum.PhaseVsTime), new MathExt.Vector(pvtbuffer.ToArray(), "s", "°", 1.0 / (wfminfo?.SampleRateHardware ?? 1.0), 1.0));
            //OccupierBuffer.Default.Provide(nameof(MultiDomainFigureEnum.Waterfalls), new MathExt.Vector(WaterFallsBuffer.SelectMany(row => row).Select(o => o * 100), "Hz", "V", 1.0, 1.0));
            OccupierBuffer.Default.Provide(nameof(MultiDomainFigureEnum.Waterfalls), new MathExt.Vector(WaterFallsBufferArray, "Hz", "", wfminfo?.SampleRateHardware ?? 1.0, 1.0));
            OccupierBuffer.Default.Provide(nameof(MultiDomainFigureEnum.Spectrogram), new MathExt.Vector(specbufferarray, "Hz", "", wfminfo?.SampleRateHardware ?? 1.0, 1.0));

            if (SynchronizationEnable)
            {
                if (DsoPrsnt.DefaultDsoPrsnt.MultiDomain != null)
                {
                    if (wfminfo?.SpanSync != null && wfminfo?.SpanSync != _SpanSync)
                    {
                        _SpanSync = wfminfo?.SpanSync ?? SpanFreqByHz;
                        DsoPrsnt.DefaultDsoPrsnt.MultiDomain.SpanFreqByHz = _SpanSync;
                    }
                    if (wfminfo?.FFTLengthSync != null && wfminfo?.FFTLengthSync != _FFTLengthSync)
                    {
                        _FFTLengthSync = (wfminfo?.FFTLengthSync ?? FFTLength) == 0 ? 1024 : wfminfo?.FFTLengthSync ?? FFTLength;
                        DsoPrsnt.DefaultDsoPrsnt.MultiDomain.FFTLength = _FFTLengthSync;
                    }
                    //DsoPrsnt.DefaultDsoPrsnt.MultiDomain.SpanFreqByHz = wfminfo?.SpanSync ?? SpanFreqByHz;
                    //DsoPrsnt.DefaultDsoPrsnt.MultiDomain.FFTLength = (wfminfo?.FFTLengthSync ?? FFTLength) == 0 ? 1024 : wfminfo?.FFTLengthSync ?? FFTLength;
                    //DsoPrsnt.DefaultDsoPrsnt.MultiDomain.SpanFreqByHz = 8_000_000_000;
                    //DsoPrsnt.DefaultDsoPrsnt.MultiDomain.FFTLength = 512;
                }
            }
            if (ParameterTuningEnable && DsoPrsnt.DefaultDsoPrsnt.MultiDomain != null && wfminfo?.StateParamTuning != _StateParamTuning)
            {
                _StateParamTuning = wfminfo?.StateParamTuning ?? _StateParamTuning;
                if (wfminfo?.SpanParamTuning != null)
                {
                    _SpanParamTuning = wfminfo?.SpanParamTuning ?? SpanFreqByHz;
                    DsoPrsnt.DefaultDsoPrsnt.MultiDomain.SpanFreqByHz = _SpanParamTuning;
                }
                if (wfminfo?.CenterFreqParamTuning != null)
                {
                    _CenterFreqParamTuning = wfminfo?.CenterFreqParamTuning ?? CenterFreqByHz;
                    DsoPrsnt.DefaultDsoPrsnt.MultiDomain.CenterFreqByHz = _CenterFreqParamTuning;
                }
                //DsoPrsnt.DefaultDsoPrsnt.MultiDomain.SpanFreqByHz = _SpanParamTuning;
                //DsoPrsnt.DefaultDsoPrsnt.MultiDomain.CenterFreqByHz = _CenterFreqParamTuning;

                if (wfminfo?.CenterFreqParamTuning != null)
                {
                    var tuningValue = wfminfo.CenterFreqParamTuning;

                    var spanListForTimeFreq = DsoPrsnt.DefaultDsoPrsnt.MultiDomain.SpanListForTimeFreq;
                    if (spanListForTimeFreq.Count > 0)
                    {
                        Int32 nearestIndex = 0;
                        Double minDiff = Math.Abs(spanListForTimeFreq[0] - tuningValue);
                        for (int i = 1; i < spanListForTimeFreq.Count; i++)
                        {
                            Double diff = Math.Abs(spanListForTimeFreq[i] - tuningValue);
                            if (diff < minDiff)
                            {
                                minDiff = diff;
                                nearestIndex = i;
                            }
                        }

                        if (DsoPrsnt.DefaultDsoPrsnt.MultiDomain.SpanOptForTimeFreq != nearestIndex)
                        {
                            DsoPrsnt.DefaultDsoPrsnt.MultiDomain.SpanOptForTimeFreq = nearestIndex;
                        }
                    }

                }
            }
        }

        private Int64 _CurStart = 0;
        private Int64 _CurLength = 1024;
        private Boolean CutData(List<Double> originalData, Int64 spanSoft, Int64 spanHardware, out List<Double>? dataAfterCut)
        {
            dataAfterCut = new List<double>();
            Int32 fftLength = (int)FFTLength;

            if (originalData.Count != fftLength || originalData.Count == 0 || spanSoft > spanHardware)
            {
                dataAfterCut = null;
                return false;
            }

            var start = 0;
            Int64 length = (fftLength * spanSoft) / spanHardware;
            if (spanHardware == 8_000_000_000)
            {
                length = (Int64)Math.Ceiling(length * 0.4);
            }

            List<double> temp = new List<double>();
            temp.AddRange(originalData);
            //originalData.RemoveRange(0, (int)start);
            //originalData.RemoveRange((int)length, (int)(originalData.Count - length - start));
            if (temp.Count >= (int)(temp.Count - length - start))
            {
                temp.RemoveRange((int)length, (int)(temp.Count - length - start));
            }
            dataAfterCut = temp;

            if (_CurStart != start || _CurLength != length)
            {
                _CurStart = (int)start;
                _CurLength = (int)length;
                WaterFallsBuffer.Clear();
            }

            return true;
        }

        private Boolean RfreshData(List<Double>? databuffer, Boolean replaceUp)
        {
            if (databuffer == null)
            {
                return false;
            }
            // 定义WaterFallsBuffer的行数和列数
            int rows = WaterFallsBuffer.Count;
            int columns = WaterFallsBuffer.Count > 0 ? WaterFallsBuffer[0].Count : (int)_CurLength;

            int rows_test = WaterFallsBuffer.Count;
            int columns_test = 0;

            #region 三维
            if (rows_test > 0)
            {
                //columns_test = ThreeDimensionalBuffer[0].Length;
            }
            if (rows_test < 15)
            {
                // 将 databuffer 中的每个元素转换为 float
                //var floatBuffer = databuffer.Select(x => (float)x).ToArray();
                //ThreeDimensionalBuffer.Add(databuffer.ToArray());
            }
            else
            {
                for (int i = 1; i < rows_test; i++)
                {
                    //ThreeDimensionalBuffer[i - 1] = ThreeDimensionalBuffer[i];
                }
                if (databuffer.Count == columns_test)
                {
                    // 将 databuffer 中的每个元素转换为 float
                    //var floatBuffer = databuffer.Select(x => (float)x).ToArray();
                    //ThreeDimensionalBuffer[0] = floatBuffer;
                    //ThreeDimensionalBuffer[0] = databuffer.ToArray();
                }
            }
            #endregion

            //Double coe = 0.000009;
            // 确保缓冲区行数小于 15 时进行初始化
            if (rows < 50)
            {
                if (databuffer.Count == columns)
                {
                    WaterFallsBuffer.Add(databuffer.Select(value => (value + 230) * 0.004).ToList());
                }
            }
            else
            {
                // 处理 replaceDown 标志：如果为 true 向下替换
                if (replaceUp)
                {
                    // 向下移动每一行，排除最后一行
                    //for (int i = rows - 1; i > 0; i--)
                    //{
                    //    WaterFallsBuffer[i] = new List<double>(WaterFallsBuffer[i - 1]);  // 移动数据
                    //}
                    //WaterFallsBuffer[0] = databuffer.Select(value => value * 0.001).ToList();

                    WaterFallsBuffer.RemoveAt(0);
                    WaterFallsBuffer.Add(databuffer.Select(value => (value + 230) * 0.004).ToList());
                }
                else
                {
                    // 向上移动每一行，排除第一行
                    //for (int i = 0; i < rows - 1; i++)
                    //{
                    //    WaterFallsBuffer[i] = new List<double>(WaterFallsBuffer[i + 1]);  // 移动数据
                    //}
                    //WaterFallsBuffer[rows - 1] = databuffer.Select(value => value * 0.001).ToList();

                    WaterFallsBuffer.RemoveAt(WaterFallsBuffer.Count - 1);
                    WaterFallsBuffer.Insert(0, databuffer.Select(value => (value + 230) * 0.004).ToList());
                }
            }

            #region 待确认
            // 如果replaceDown为true，则向下替换：移除第一行，添加新的行到末尾
            //if (replaceDown)
            //{
            //    // 向下移动每一行
            //    for (int i = 1; i < rows; i++)
            //    {
            //        for (int j = 0; j < columns; j++)
            //        {
            //            WaterFallsBuffer_test[i - 1, j] = WaterFallsBuffer_test[i, j];
            //        }
            //    }

            //    // 将新的databuffer添加到末尾
            //    if (databuffer.Count == columns)
            //    {
            //        for (int j = 0; j < columns; j++)
            //        {
            //            WaterFallsBuffer_test[rows - 1, j] = databuffer[j] * 0.001;
            //        }
            //    }
            //}
            //else
            //{
            //    // 向上替换：移除最后一行，添加新的行到开头
            //    for (int i = rows - 2; i >= 0; i--)
            //    {
            //        for (int j = 0; j < columns; j++)
            //        {
            //            WaterFallsBuffer_test[i + 1, j] = WaterFallsBuffer_test[i, j];
            //        }
            //    }

            //    // 将新的databuffer插入到第一行
            //    if (databuffer.Count == columns)
            //    {
            //        for (int j = 0; j < columns; j++)
            //        {
            //            WaterFallsBuffer_test[0, j] = databuffer[j] * 0.001;
            //        }
            //    }
            //}
            #endregion

            return true;
        }

        public double[,] ConvertTo2DArray(List<List<double>> list)
        {
            if (list == null || list.Count == 0 || list[0].Count == 0)
            {
                return new double[0, 0];
            }

            int rows = list.Count;
            int columns = list[0].Count;

            double[,] array = new double[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    array[i, j] = list[i][j];
                }
            }

            return array;
        }

        public double[,] ConvertTo2DArray(List<Double> specbuffer)
        {
            if (specbuffer.Count != 512 * 512)
            {
                //throw new ArgumentException("specbuffer must contain 512*512 elements.");
                return new double[0, 0];
            }

            double[,] raw = new double[512, 512];
            for (int i = 0; i < 512; i++)
            {
                for (int j = 0; j < 512; j++)
                {
                    int index = i * 512 + j;
                    raw[i, j] = specbuffer[index];
                }
            }

            return NormalizeAndShiftSpectrogram(raw);
        }

        private double[,] NormalizeAndShiftSpectrogram(double[,] raw)
        {
            int rows = raw.GetLength(0);
            int cols = raw.GetLength(1);
            if (rows == 0 || cols == 0)
                return new double[0, 0];

            List<double> finiteValues = new(rows * cols);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double value = raw[i, j];
                    if (Double.IsFinite(value))
                        finiteValues.Add(value);
                }
            }

            if (finiteValues.Count == 0)
                return new double[rows, cols];

            finiteValues.Sort();
            int loIndex = (int)Math.Floor((finiteValues.Count - 1) * _specNormLowQuantile);
            int hiIndex = (int)Math.Floor((finiteValues.Count - 1) * _specNormHighQuantile);
            loIndex = Math.Clamp(loIndex, 0, finiteValues.Count - 1);
            hiIndex = Math.Clamp(hiIndex, loIndex, finiteValues.Count - 1);

            double currentMin = finiteValues[loIndex];
            double currentMax = finiteValues[hiIndex];
            if (currentMax - currentMin < 1e-12)
            {
                currentMin = finiteValues[0];
                currentMax = finiteValues[finiteValues.Count - 1];
            }

            // 帧间平滑动态范围，避免颜色映射在相邻帧间跳变导致“突兀感”。
            if (!_specNormInitialized)
            {
                _specNormMin = currentMin;
                _specNormMax = currentMax;
                _specNormInitialized = true;
            }
            else
            {
                _specNormMin = _specNormMin * (1.0 - _specNormSmoothing) + currentMin * _specNormSmoothing;
                _specNormMax = _specNormMax * (1.0 - _specNormSmoothing) + currentMax * _specNormSmoothing;
            }

            if (_specNormMax - _specNormMin < 1e-12)
            {
                _specNormMin = currentMin;
                _specNormMax = currentMax;
            }

            double range = Math.Max(_specNormMax - _specNormMin, 1e-12);
            int half = cols / 2;
            double[,] result = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    double value = raw[i, j];
                    double normalized = 0.0;
                    if (Double.IsFinite(value))
                    {
                        normalized = Math.Clamp((value - _specNormMin) / range, 0.0, 1.0);
                        normalized = Math.Pow(normalized, _specGamma);
                    }

                    int shiftedCol = (j + half) % cols;
                    result[i, shiftedCol] = normalized;
                }
            }

            return result;
        }

        private Boolean IQToAmplitude(List<double> bufferI, List<double> bufferQ, List<double> result)
        {
            if (bufferI.Count != bufferQ.Count)
                return false;
            //result = new List<double>();
            for (int i = 0; i < bufferI.Count; i++)
            {
                System.Numerics.Complex complex = new System.Numerics.Complex(bufferI[i], bufferQ[i]);
                result.Add(complex.Magnitude);
            }
            return true;
        }

        private Boolean IQToPhase(List<double> bufferI, List<double> bufferQ, List<double> result)
        {
            if (bufferI.Count != bufferQ.Count)
                return false;
            //result = new List<double>();
            for (int i = 0; i < bufferI.Count; i++)
            {
                var phase = GetPhase(bufferI[i], bufferQ[i]);
                result.Add(phase);
            }
            return true;
        }

        private Boolean PhaseToFrequency(List<double> bufferI, Double sampleInterval, List<double> result)
        {
            if (bufferI.Count < 2)
                return false;
            Double pre = 0;
            Double real_pre = 0;
            List<double> bufferI_real = new List<Double>();
            for (int i = 0; i < bufferI.Count; i++)
            {
                bufferI_real.Add(bufferI[i]);
            }
            int k = 0;
            for (int i = 0; i < bufferI.Count; i++)
            {
                if ((bufferI[i] - pre) < -180)
                {
                    k++;
                }
                else if ((bufferI[i] - pre) > 180)
                {
                    k--;
                }
                bufferI_real[i] = bufferI[i] + 360 * k;
                var freq = (bufferI[i] + 360 * k - real_pre) / (sampleInterval * 360);//lhc test------------------------------------------------------------------
                pre = bufferI[i];
                real_pre = bufferI_real[i];


                result.Add(freq);
            }
            return true;
        }

        private Boolean PhaseToFrequency(List<double> result, Double sampleInterval)
        {
            if (result.Count < 2)
                return false;
            var frequency = new List<double>();
            Double pre = 0;
            for (int i = 0; i < result.Count; i++)
            {
                var freq = (result[i] - pre) / sampleInterval;
                pre = result[i];
                frequency.Add(freq);
            }
            result = frequency;
            return true;
        }

        private Double GetPhase(Double i, Double q)
        {
            Double phase = 0;
            if (i > 0 && q > 0)
                phase = Math.Atan(q / i) / Math.PI * 180;
            else if (i > 0 && q < 0)
                phase = Math.Atan(q / i) / Math.PI * 180;
            else if (i < 0 && q > 0)
                phase = Math.Atan(q / i) / Math.PI * 180 + 180;
            else if (i < 0 && q < 0)
                phase = Math.Atan(q / i) / Math.PI * 180 - 180;
            return phase;
        }

        private static Boolean RescaleAmp(List<double> pkg)
        {
            if (pkg == null)
                return false;
            Double unitDiff_test = -106.99;
            unitDiff_test = 30;
            const Double minDb = -200.0;
            for (Int32 i = 0; i < pkg.Count; i++)
            {
                if (!Double.IsNaN(pkg[i]))
                {
                    if (pkg[i] <= 0)
                    {
                        pkg[i] = minDb;
                    }
                    else
                    {
                        Double db = 20 * Math.Log10(pkg[i]) + unitDiff_test;
                        pkg[i] = Double.IsFinite(db) ? db : minDb;
                    }
                }
            }
            return true;
        }

        private static Double ValidateVuSamples(Double y)
        {
            if (y > Constants.MAX_YPOS_IDX)
            {
                y = Constants.MAX_YPOS_IDX;
            }
            else if (y < Constants.MIN_YPOS_IDX)
            {
                y = Constants.MIN_YPOS_IDX;
            }

            return y;
        }

        #region 接口实现
        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

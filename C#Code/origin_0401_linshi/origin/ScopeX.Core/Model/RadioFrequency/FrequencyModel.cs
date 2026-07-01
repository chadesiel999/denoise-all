using System;
using System.Collections.Generic;
using System.Linq;
using ScopeX.ComModel;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    internal sealed class FrequencyModel : SamplingModel
    {
        public FrequencyModel() : base()
        {
            _TranslateSampleRate = RFSampInfo.GetTranslateSampleRate(_Span, _Source);
        }

        private Double _TranslateSampleRate;
        public Double TranslateSampleRate { get => _TranslateSampleRate; }

        private Double _PosIndex;
        public override Double PosIndex
        {
            get
            {
                return -_FigureStartFrequency / ((_FigureEndFrequency -_FigureStartFrequency) / Constants.MAX_XPOS_IDX);
            }
            set
            {
                if (_PosIndex != value)
                {
                    _PosIndex = -_FigureStartFrequency / ((_FigureEndFrequency - _FigureStartFrequency) / Constants.MAX_XPOS_IDX);
                    _FigureCenterFrequency = (Int64)(-(value / Constants.IDX_PER_XDIV) * _FrequencyScale);
                }
            }
        }

        private ChannelId _Source = ChannelId.RF;
        public ChannelId Source
        {
            get
            {
                return _Source;
            }
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                }
            }
        }

        #region 数据（波形）参数

        public Boolean Init = false;

        #region FFTLength

        private Int32 _FFTLength = (Int32)Math.Pow(2, Constants.RF_FFT_POWER_MAX);
        public Int32 FFTLength
        {
            get
            {
                return _FFTLength;
            }
            set
            {
                if (_FFTLength != value)
                {
                    _FFTLength = ValidFFTLength(value);
                    _RBW = GetRBW();
                    //if (_FFTLength>_STFTLength)
                    {
                        _STFTLength = _FFTLength;
                    }
                    Init = true;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _FTPower = Constants.RF_FFT_POWER_MAX;

        public Int32 DataLength
        {
            get
            {
                var prop = Span == Constants.RF_SPAN_MAX ? 1 : 0.8;
                return (Int32)(_FFTLength * prop);
            }
        }

        private Int32 ValidFFTLength(Int32 value)
        {
            Int32 minpower = Constants.RF_FFT_POWER_MIN;
            Int32 maxpower = Constants.RF_FFT_POWER_MAX;

            if (value <= Math.Pow(2, minpower))
            {
                return (Int32)Math.Pow(2, minpower);
            }

            for (Int32 i = minpower; i < maxpower; i++)
            {
                if (value >= Math.Pow(2, i) && value < Math.Pow(2, i + 1))
                {
                    value = (Int32)Math.Pow(2, i);
                    _FTPower = i;
                    return value;
                }
            }

            return (Int32)Math.Pow(2, maxpower);
        }

        private Int32 GetFFTLength(Double rbwvalue)
        {
            return ValidFFTLength((Int32)(RFSampInfo.GetTranslateSampleRate(_Span,_Source) / rbwvalue));
        }

        private Int32 _STFTLength = Constants.RF_STFT_DATA_LENGTH;
        public Int32 STFTLength
        {
            get => _STFTLength;
            set
            {
                if (value != _STFTLength)
                {
                    _STFTLength = value;
                    if (_STFTLength < _FFTLength)
                        _STFTLength = _FFTLength;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _STFTStep = Constants.RF_STFT_STEP;
        public Int32 STFTStep
        {
            get => _STFTStep;
            set
            {
                if (value != _STFTStep)
                {
                    _STFTStep = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region 起始频率

        private Int64 _StartFrequency = Constants.RF_START_FREQUENCY_MIN;
        public Int64 StartFrequency
        {
            get
            {
                return _StartFrequency;
            }
            set
            {
                if (_StartFrequency != value)
                {
                    _StartFrequency = ValidateLimitedValue(value, Constants.RF_START_FREQUENCY_MIN, Constants.RF_START_FREQUENCY_MAX);
                    ParameterSynchronization("StartFrequency");
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        #region 中心频率

        private Int64 _CenterFrequency = Constants.RF_END_FREQUENCY_MAX / 2;
        public Int64 CenterFrequency
        {
            get
            {
                return _CenterFrequency;
            }
            set
            {
                if (_CenterFrequency != value)
                {
                    _CenterFrequency = ValidateLimitedValue(value, Constants.RF_CENTER_FREQUENCY_MIN, Constants.RF_CENTER_FREQUENCY_MAX);
                    ParameterSynchronization("CenterFrequency");
                    OnPropertyChanged();
                }
            }
        }



        #endregion

        #region 终止频率

        private Int64 _EndFrequency = Constants.RF_END_FREQUENCY_MAX;
        public Int64 EndFrequency
        {
            get
            {
                return _EndFrequency;
            }
            set
            {
                if (_EndFrequency != value)
                {
                    _EndFrequency = ValidateLimitedValue(value, Constants.RF_END_FREQUENCY_MIN, Constants.RF_END_FREQUENCY_MAX);
                    ParameterSynchronization("EndFrequency");
                    OnPropertyChanged();
                }
            }
        }


        #endregion

        #region 跨度

        private Int64 _Span = Constants.RF_SPAN_MAX;
        public Int64 Span
        {
            get
            {
                return _Span;
            }
            set
            {
                if (_Span != value)
                {
                    _Span = ValidateLimitedValue(value, Constants.RF_SPAN_MIN, Constants.RF_SPAN_MAX);
                    ParameterSynchronization("Span");
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region RBW

        private Double _RBW = Constants.RF_RBW_MIN;
        public Double RBW
        {
            get
            {
                return GetRBW();
            }
            set
            {
                if (_RBW != value)
                {
                    var ol = _FFTLength;
                    _FFTLength = GetFFTLength(value);
                    if (value<_RBW && _FFTLength==ol)
                    {
                        _FFTLength = (Int32)Math.Pow(2, _FTPower+1);
                    }
                    _RBW = GetRBW();
                    Init = true;
                    OnPropertyChanged();
                }
            }
        }

        private Double GetRBW()
        {
            return (Double)(_TranslateSampleRate / _FFTLength);
        }
        public Double GetRBWMin()
        {
            return (Double)(_TranslateSampleRate / (Double)Math.Pow(2, Constants.RF_FFT_POWER_MAX));
        }

        public Double GetRBWMax()
        {
            return (Double)(_TranslateSampleRate / (Double)Math.Pow(2, Constants.RF_FFT_POWER_MIN));
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

        private void ParameterSynchronization(String param)
        {
            switch (param)
            {
                case "StartFrequency":
                    {
                        _Span = _EndFrequency - _StartFrequency;
                        if (_EndFrequency - _StartFrequency < Constants.RF_SPAN_MIN)
                        {
                            _Span = Constants.RF_SPAN_MIN;
                            _EndFrequency = _StartFrequency + _Span;
                        }
                        if (EndFrequency - _StartFrequency > Constants.RF_SPAN_MAX)
                        {
                            _Span = Constants.RF_SPAN_MAX;
                            _EndFrequency = _StartFrequency + _Span;
                        }
                        _CenterFrequency = _StartFrequency + _Span / 2;
                    }
                    break;
                case "EndFrequency":
                    {
                        _Span = _EndFrequency - _StartFrequency;
                        if (_EndFrequency - _StartFrequency < Constants.RF_SPAN_MIN)
                        {
                            _Span = Constants.RF_SPAN_MIN;
                            _StartFrequency = _EndFrequency - _Span;
                        }
                        if (EndFrequency - _StartFrequency > Constants.RF_SPAN_MAX)
                        {
                            _Span = Constants.RF_SPAN_MAX;
                            _StartFrequency = _EndFrequency - _Span;
                        }
                        _CenterFrequency = _StartFrequency + _Span / 2;
                    }
                    break;
                case "CenterFrequency":
                    {
                        _StartFrequency = _CenterFrequency - _Span / 2;
                        _EndFrequency = _CenterFrequency + _Span / 2;
                        if (_StartFrequency < Constants.RF_START_FREQUENCY_MIN)
                        {
                            _StartFrequency = Constants.RF_START_FREQUENCY_MIN;
                            _EndFrequency = 2 * _CenterFrequency;
                            _Span = _EndFrequency - _StartFrequency;
                        }
                        if (_EndFrequency > Constants.RF_END_FREQUENCY_MAX)
                        {
                            _EndFrequency = Constants.RF_END_FREQUENCY_MAX;
                            _StartFrequency = _EndFrequency - 2 * (_EndFrequency-_CenterFrequency);
                            _Span = _EndFrequency - _StartFrequency;
                        }
                    }
                    break;
                case "Span":
                    {
                        _StartFrequency = _CenterFrequency - _Span / 2;
                        _EndFrequency = _CenterFrequency + _Span / 2;
                        if (_StartFrequency < Constants.RF_START_FREQUENCY_MIN)
                        {
                            _StartFrequency = Constants.RF_START_FREQUENCY_MIN;
                            _EndFrequency = _StartFrequency + _Span;
                            _CenterFrequency = _StartFrequency + _Span / 2;
                        }
                        if (_EndFrequency > Constants.RF_END_FREQUENCY_MAX)
                        {
                            _EndFrequency = Constants.RF_END_FREQUENCY_MAX;
                            _StartFrequency = _EndFrequency - _Span;
                            _CenterFrequency = _StartFrequency + _Span / 2;
                        }
                    }
                    break;
                default:
                    break;
            }
            _TranslateSampleRate = RFSampInfo.GetTranslateSampleRate(_Span,_Source);

            _RBW = GetRBW();
            _FrequencyScale = _Span / Constants.VIS_XDIVS_NUM;
            _FigureCenterFrequency = _CenterFrequency;
            _FigureStartFrequency = _FigureCenterFrequency - Constants.VIS_XDIVS_NUM / 2 * _FrequencyScale;
            _FigureEndFrequency = _FigureCenterFrequency + Constants.VIS_XDIVS_NUM / 2 * _FrequencyScale;
            Init = true;
        }
        #endregion

        #region FigureParameter
        private Int64 _FrequencyScale = Constants.RF_SPAN_MAX/ Constants.VIS_XDIVS_NUM;
        /// <summary>
        /// 频率档位 Hz
        /// </summary>
        public Int64 FrequencyScale
        {
            get
            {
                return _FrequencyScale;
            }
            set
            {
                if (_FrequencyScale != value)
                {
                    _FrequencyScale = value;
                    _FigureStartFrequency = _FigureCenterFrequency - Constants.VIS_XDIVS_NUM / 2 * _FrequencyScale;
                    _FigureEndFrequency = _FigureCenterFrequency + Constants.VIS_XDIVS_NUM / 2 * _FrequencyScale;
                    Init = true;
                    OnPropertyChanged();
                }
            }
        }

        private Int64 _FigureStartFrequency = Constants.RF_START_FREQUENCY_MIN;
        /// <summary>
        /// 图像起始频率
        /// </summary>
        public Int64 FigureStartFrequency
        {
            get
            {
                return _FigureStartFrequency;
            }
        }

        private Int64 _FigureEndFrequency = Constants.RF_END_FREQUENCY_MAX;
        /// <summary>
        /// 图像终止频率
        /// </summary>
        public Int64 FigureEndFrequency
        {
            get
            {
                return _FigureEndFrequency;
            }
        }

        private Int64 _FigureCenterFrequency = (Constants.RF_END_FREQUENCY_MAX+ Constants.RF_START_FREQUENCY_MIN)/2;
        /// <summary>
        /// 图像中心频率
        /// </summary>
        public Int64 FigureCenterFrequency
        {
            get
            {
                return _FigureCenterFrequency;
            }
            set
            {
                if (_FigureCenterFrequency != value)
                {
                    _FigureCenterFrequency = value;
                    //_FreqScale = _Span / Constants.VIS_XDIVS_NUM;
                    _FigureStartFrequency = _FigureCenterFrequency - Constants.VIS_XDIVS_NUM / 2 * _FrequencyScale;
                    _FigureEndFrequency = _FigureCenterFrequency + Constants.VIS_XDIVS_NUM / 2 * _FrequencyScale;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        #endregion
    }

    internal sealed class FrequencyModelV : VertAxisModel
    {
        public FrequencyModelV() : base("Conditioning")
        {
          
        }

        private Double _TranslateSampleRate;
        public Double TranslateSampleRate { get => _TranslateSampleRate; }

        private Double _PosIndex;
        public override Double PosIndex
        {
            get
            {
                return -_FigureStartFrequency / ((_FigureEndFrequency - _FigureStartFrequency) / Constants.MAX_YPOS_IDX);
            }
            set
            {
                if (_PosIndex != value)
                {
                    _PosIndex = -_FigureStartFrequency / ((_FigureEndFrequency - _FigureStartFrequency) / Constants.MAX_YPOS_IDX);
                    _FigureCenterFrequency = (Int64)(-(value / Constants.IDX_PER_YDIV) * _FrequencyScale);
                }
            }
        }

        private ChannelId _Source = ChannelId.RF;
        public ChannelId Source
        {
            get
            {
                return _Source;
            }
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                }
            }
        }

        #region 数据（波形）参数

        #region FigureParameter
        private Int64 _FrequencyScale = Constants.RF_SPAN_MAX / Constants.VIS_YDIVS_NUM;
        /// <summary>
        /// 频率档位 Hz
        /// </summary>
        public Int64 FrequencyScale
        {
            get
            {
                return _FrequencyScale;
            }
            set
            {
                if (_FrequencyScale != value)
                {
                    _FrequencyScale = value;
                    _FigureStartFrequency = _FigureCenterFrequency - Constants.VIS_YDIVS_NUM / 2 * _FrequencyScale;
                    _FigureEndFrequency = _FigureCenterFrequency + Constants.VIS_YDIVS_NUM / 2 * _FrequencyScale;
                  
                    OnPropertyChanged();
                }
            }
        }

        private Int64 _FigureStartFrequency = Constants.RF_START_FREQUENCY_MIN;
        /// <summary>
        /// 图像起始频率
        /// </summary>
        public Int64 FigureStartFrequency
        {
            get
            {
                return _FigureStartFrequency;
            }
        }

        private Int64 _FigureEndFrequency = Constants.RF_END_FREQUENCY_MAX;
        /// <summary>
        /// 图像终止频率
        /// </summary>
        public Int64 FigureEndFrequency
        {
            get
            {
                return _FigureEndFrequency;
            }
        }

        private Int64 _FigureCenterFrequency = (Constants.RF_END_FREQUENCY_MAX + Constants.RF_START_FREQUENCY_MIN) / 2;
        /// <summary>
        /// 图像中心频率
        /// </summary>
        public Int64 FigureCenterFrequency
        {
            get
            {
                return _FigureCenterFrequency;
            }
            set
            {
                if (_FigureCenterFrequency != value)
                {
                    _FigureCenterFrequency = value;
                    //_FreqScale = _Span / Constants.VIS_XDIVS_NUM;
                    _FigureStartFrequency = _FigureCenterFrequency - Constants.VIS_YDIVS_NUM / 2 * _FrequencyScale;
                    _FigureEndFrequency = _FigureCenterFrequency + Constants.VIS_YDIVS_NUM / 2 * _FrequencyScale;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
        #endregion
    }

    static class RFSampInfo
    {
        #region Tables

        private static readonly Dictionary<Int32, (Int64 Span,  Double SampleRate)> _AnalogRFSpanScaleTable = new Dictionary<Int32, (Int64 Span, Double SampleRate)>{
            {0 ,(            0,             1)},
            {1 ,(        8_000,        10_000)},//0418,RBW修改为10Hz,抽取比修改为2000000
            {2 ,(       25_000,        31_250)},
            {3 ,(       62_500,        78_125)},
            {4 ,(      250_000,       312_500)},
            {5 ,(      625_000,       781_250)},
            {6 ,(    2_500_000,     3_125_000)},
            {7 ,(    6_250_000,     7_812_500)},
            {8 ,(   25_000_000,    31_250_000)},
            {9 ,(   50_000_000,    62_500_000)},
            {10,(  100_000_000,   125_000_000)},
            {11,(  250_000_000,   312_500_000)},
            {12,(  500_000_000,   625_000_000)},
            {13,( 1000_000_000, 1_250_000_000)},
            {14,(2_000_000_000, 2_500_000_000)},
            {15,(8_000_000_000,20_000_000_000)},
        };
        private static readonly Dictionary<Int32, (Int64 Span,  Double SampleRate)> _10GRFSpanScaleTable = new Dictionary<Int32, (Int64 Span,  Double SampleRate)>{
            {0 ,(            0 / 2,                 1)},
            {1 ,(        8_000 / 2,        10_000 / 2)},//0418,RBW修改为10Hz,抽取比修改为2000000
            {2 ,(       25_000 / 2,        31_250 / 2)},
            {3 ,(       62_500 / 2,        78_125 / 2)},
            {4 ,(      250_000 / 2,       312_500 / 2)},
            {5 ,(      625_000 / 2,       781_250 / 2)},
            {6 ,(    2_500_000 / 2,     3_125_000 / 2)},
            {7 ,(    6_250_000 / 2,     7_812_500 / 2)},
            {8 ,(   25_000_000 / 2,    31_250_000 / 2)},
            {9 ,(   50_000_000 / 2,    62_500_000 / 2)},
            {10,(  100_000_000 / 2,   125_000_000 / 2)},
            {11,(  250_000_000 / 2,   312_500_000 / 2)},
            {12,(  500_000_000 / 2,   625_000_000 / 2)},
            {13,(1_000_000_000 / 2, 1_250_000_000 / 2)},
            {14,(2_000_000_000 / 2, 2_500_000_000 / 2)},
            {15,(8_000_000_000 / 2,20_000_000_000 / 2)},
        };
       
        public static Double GetTranslateSampleRate(Int64 span, ChannelId source)
        {
            Dictionary<Int32, (Int64 Span, Double SampleRate)> table = source == ChannelId.RF ? _10GRFSpanScaleTable : _AnalogRFSpanScaleTable;
            for (Int32 i = 0; i < table.Count - 1; i++)
            {
                if (span > table[i].Span && span <= table[i + 1].Span)
                {
                    return table[i + 1].SampleRate;
                }
            }
            return table[table.Count - 1].SampleRate;
        }
        
        #endregion
        #region WindowCoefficient
        public static readonly Dictionary<RFWindowType, Double> WindowGainTable = new Dictionary<RFWindowType, Double>{
            {RFWindowType.Rectangle ,0.89},
            {RFWindowType.Hann ,1.44},
            {RFWindowType.Hamming ,1.3},
            {RFWindowType.Blackman ,1.9},
            {RFWindowType.Flattop ,3.77},
            {RFWindowType.Kaiser ,2.23},
            {RFWindowType.Gaussian ,1.4468},
        };
        public static IEnumerable<Double> GetWindowCoefficient(Int32 length, RFWindowType type = RFWindowType.Rectangle)
        {
            var w = new Double[length];
            for (Int32 i = 0; i < length; i++)
                w[i] = 2 * Math.PI * i / (length - 1);

            switch (type)
            {
                default:
                    return Enumerable.Repeat(1.0d, length);
                case RFWindowType.Hamming:
                    return w.Select(o => 0.54 - 0.46 * Math.Cos(o));
                case RFWindowType.Hann:
                    return w.Select(o => 0.5 - 0.5 * Math.Cos(o));
                case RFWindowType.Blackman:
                    return w.Select(o => 0.42 - 0.5 * Math.Cos(o) + 0.08 * Math.Cos(o));
                case RFWindowType.Flattop:
                    Double fta0 = 0.215578995;
                    Double fta1 = 0.41663158;
                    Double fta2 = 0.277263158;
                    Double fta3 = 0.083578947;
                    Double fta4 = 0.006947368;
                    return w.Select(o =>
                        fta0 - fta1 * Math.Cos(o) +
                            fta2 * Math.Cos(2 * o) +
                            fta3 * Math.Cos(3 * o) +
                            fta4 * Math.Cos(4 * o));
                case RFWindowType.Kaiser:
                    return GetKaiserWindow(length);
                case RFWindowType.Gaussian:
                    return GetGaussianWindow(length);

            }
        }
        private static IEnumerable<Double> GetGaussianWindow(Int32 length, Double alpha = 2.5)
        {
            Double sigma = (length - 1) / (2 * alpha);

            var w = new Double[length];

            return w.Select(o => Math.Pow(Math.E, -Math.Pow(o, 2) / (2 * Math.Pow(sigma, 2))));
        }
        private static IEnumerable<Double> GetKaiserWindow(Int32 length)
        {
            for (Int32 i = 0; i < length; i++)
            {
                Double beta = 7;
                Double x = beta * Math.Sqrt(1 - Math.Pow(1 - 2 * i / (length - 1), 2));
                yield return I0function(x) / I0function(beta);
            }
        }
        private static Double I0function(Double x)
        {
            Double i0 = 0;
            for (Int32 i = 0; i < 25; i++)
            {
                Double fact = Fact(i);
                i0 += Math.Pow(x, 2 * i) / Math.Pow(4, x) / Math.Pow(fact, 2);
            }
            return i0;
        }
        private static Double Fact(Int32 n)
        {
            if (n == 0)
                return 1;

            Double y = n;
            for (Double m = n - 1; m > 0; m--)
                y *= m;

            return y;
        }
        #endregion
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.Record.CF;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using ScopeX.MathExt.Filter;

namespace ScopeX.Core
{
    internal class FilterModel :INotifyPropertyChanged
    {
        public FilterModel(Int32 freqDigits = 3, Int32 magDigits = 2)
        {
            FreqFactor = (Int32)Math.Pow(10, freqDigits);
            _LowPassFreq = (Int32)Math.Round(0.4 * FreqFactor, MidpointRounding.AwayFromZero);
            _LowStopFreq = (Int32)Math.Round(0.6 * FreqFactor, MidpointRounding.AwayFromZero);
            _HighPassFreq = (Int32)Math.Round(0.4 * FreqFactor, MidpointRounding.AwayFromZero);
            _HighStopFreq = (Int32)Math.Round(0.6 * FreqFactor, MidpointRounding.AwayFromZero);

            MagFactor = (Int32)Math.Pow(10, magDigits);
            MaxMagnitude = 1_000 * MagFactor;
            MinMagnitude = 0;
            _PassMag = (Int32)Math.Round(0.1 * MagFactor, MidpointRounding.AwayFromZero);
            _StopMag = (Int32)Math.Round(50.0 * MagFactor, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// 1~1000
        /// </summary>
        public readonly Int32 FreqFactor;

        /// <summary>
        /// 0.01~1000
        /// </summary>
        public readonly Int32 MagFactor;

        public readonly Int32 MaxMagnitude;

        public readonly Int32 MinMagnitude;

        private FilterResponseType _RespType = FilterResponseType.LowPass;
        public FilterResponseType RespType
        {
            get => _RespType;
            set
            {
                if (_RespType != value)
                {
                    _RespType = value;
                    OnPropertyChanged();
                }
            }
        }

        private FilterType _FilterType = FilterType.FIRFilter;
        public FilterType FilterType
        {
            get => _FilterType;
            set
            {
                if (_FilterType != value)
                {
                    _FilterType = value;
                    OnPropertyChanged();
                }
            }
        }

        private FIRType _FIRMethod = FIRType.FreqSampling;
        public FIRType FIRMethod
        {
            get => _FIRMethod;
            set
            {
                if (_FIRMethod != value)
                {
                    _FIRMethod = value;
                    OnPropertyChanged();
                }
            }
        }

        private IIRType _IIRMethod = IIRType.Butterworth;
        public IIRType IIRMethod
        {
            get => _IIRMethod;
            set
            {
                if (_IIRMethod != value)
                {
                    _IIRMethod = value;
                    OnPropertyChanged();
                }
            }
        }

        private FilterOrderMode _OrderMode = FilterOrderMode.UserDefined;
        public FilterOrderMode OrderMode
        {
            get => _OrderMode;
            set
            {
                if (_OrderMode != value)
                {
                    _OrderMode = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _Order = MinOrder;
        public Int32 Order
        {
            get => _Order;
            set
            {
                value = ValidateOrder(FilterType,value);
                if (_Order != value)
                {
                    _Order = value;
                    OnPropertyChanged();
                }
            }
        }

        public static readonly Int32 FIRMaxOrder = 1000;
        public static readonly Int32 IIRMaxOrder = 50;

        public static readonly Int32 MinOrder = 2;

        private static Int32 ValidateOrder(FilterType type, Int32 value)
        {
            Int32 max = type == FilterType.FIRFilter ? FIRMaxOrder : IIRMaxOrder;

            if (value > max)
                value = max;
            else if (value < MinOrder)
                value = MinOrder;            
            return value;
        }

        //Hz
        private Double _SamplingFreq = 1;
        public Double SamplingFreq
        {
            get => _SamplingFreq;
            set
            {
                if (_SamplingFreq != value)
                {
                    _SamplingFreq = value;
                    OnPropertyChanged();
                }
            }
        }

        //Default normalize (0, 1)
        private Int32 _LowPassFreq;
        public Int32 LowPassFreq
        {
            get => _LowPassFreq;
            set
            {
                value = ValidateRelFreq(value);
                if (_LowPassFreq != value)
                {
                    _LowPassFreq = value;
                    OnPropertyChanged();
                }
            }
        }

        //Default normalize (0, 1)
        private Int32 _LowStopFreq;
        public Int32 LowStopFreq
        {
            get => _LowStopFreq;
            set
            {
                value = ValidateRelFreq(value);
                if (_LowStopFreq != value)
                {
                    _LowStopFreq = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _HighPassFreq;
        public Int32 HighPassFreq
        {
            get => _HighPassFreq;
            set
            {
                value = ValidateRelFreq(value);
                if (_HighPassFreq != value)
                {
                    _HighPassFreq = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _HighStopFreq;
        public Int32 HighStopFreq
        {
            get => _HighStopFreq;
            set
            {
                value = ValidateRelFreq(value);
                if (_HighStopFreq != value)
                {
                    _HighStopFreq = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 ValidateRelFreq(Int32 value)
        {
            if (value >= FreqFactor - 1)
                value = FreqFactor - 1;
            else if (value <= 0)
                value = 0;
            return value;
        }

        //dB
        private Int32 _PassMag;
        public Int32 PassMag
        {
            get => _PassMag;
            set
            {
                value = ValidateMagnitude(value);
                if (value < _StopMag && _PassMag != value)
                {
                    _PassMag = value;
                    OnPropertyChanged();
                }
            }
        }

        //dB
        private Int32 _StopMag;
        public Int32 StopMag
        {
            get => _StopMag;
            set
            {
                value = ValidateMagnitude(value);
                if (value > _PassMag && _StopMag != value)
                {
                    _StopMag = value;
                    OnPropertyChanged();
                }
            }
        }

        private static Int32 GetStepMag(Int32 value)
        {
            if (value > 0)
            {
                var n = (Int32)Math.Log10(value);
                if (n > 0)
                {
                    n--;
                }
                return (Int32)Math.Pow(10, n);
            }
            return 1;
        }

        private Int32 ValidateMagnitude(Int32 value)
        {
            if (value > MaxMagnitude)
                value = MaxMagnitude;
            else if (value < MinMagnitude)
                value = MinMagnitude;
            //Int32 step = GetStepMag(value);

            //value = (value / step) * step;

            return value;
        }

        public void AdjPassMag(Int32 delta) => PassMag = (Int32)(PassMag +  delta);

        public void AdjStopMag(Int32 delta) => StopMag = (Int32)(StopMag +  delta);

        //For FIR's window function design method
        private WindowType _Window = WindowType.Rectangle;
        public WindowType Window
        {
            get => _Window;
            set
            {
                if (_Window != value)
                {
                    _Window = value;
                    OnPropertyChanged();
                }
            }
        }

        //For FIR's Remez design method
        private UInt32 _DensityFactor = 20;
        public UInt32 DensityFactor
        {
            get => _DensityFactor;
            set
            {
                if (_DensityFactor != value)
                {
                    _DensityFactor = value;
                    OnPropertyChanged();
                }
            }
        }

        public Double[]? Numerator
        {
            get;
            set;
        }

        public Double[]? Denominator
        {
            get;
            set;
        }

        public Double[]? Zeros
        {
            get;
            set;
        }

        public Double[]? Poles
        {
            get;
            set;
        }


        /// <summary>
        /// 将C/C++ dll获取到的数组指针转换为数组
        /// </summary>
        /// <param name="ptr">指针对象</param>
        /// <param name="length">数组长度</param>
        /// <returns></returns>
        private static Double[] ConvertIntPtrToDoubleArray(IntPtr ptr, Int32 length)
        {
            Double[] result = new Double[length];
            Marshal.Copy(ptr, result, 0, length);
            return result;
        }

        public void Design()
        {
            Double ff = FreqFactor;
            Double mf = MagFactor;
            try
            {
                if (FilterType == FilterType.FIRFilter)
                {
                    if (OrderMode == FilterOrderMode.UserDefined)
                    {
                        if (Order % 2 != 0 && (RespType == FilterResponseType.HighPass|| RespType == FilterResponseType.BandStop))
                        {
                            Order += 1;
                            WeakTip.Default.Write(nameof(Order), MsgTipId.FilterTip1, false, "", 1);
                        }

                        if (FIRMethod == FIRType.Remez && Order == 2)
                        {
                            Order += 2;
                            WeakTip.Default.Write(nameof(Order), MsgTipId.FilterTip2, false, "", 1);
                        }

                        Numerator = FIRMethod switch
                        {
                            FIRType.FreqSampling => MathToolAPI.ConvertIntPtrToDoubleArray
                            (
                                MathToolAPI.CreateFirByFS(
                                Order,
                                (Int32)RespType,
                                (Int32)WindowType.Hamming,//default =Hamming
                                LowPassFreq / ff,
                                LowStopFreq / ff
                                ),
                                Order+1),
                            FIRType.Remez => MathToolAPI.ConvertIntPtrToDoubleArray(
                                 MathToolAPI.CreateFirByRemez(
                                Order,
                                 (Int32)RespType,
                                LowPassFreq / ff,
                                LowStopFreq / ff
                                ),
                                Order+1),
                            _ => MathToolAPI.ConvertIntPtrToDoubleArray
                            (
                                MathToolAPI.CreateFirByWindow(
                                Order,
                                 (Int32)RespType,
                                (Int32)Window,
                                LowPassFreq / ff,
                                LowStopFreq / ff
                                ),
                                Order+1),
                        };
                    }
                    else if (OrderMode == FilterOrderMode.Minimum && FIRMethod == FIRType.Remez)
                    {
                        Double[] dev = null;
                        Int32 ndev = 0;
                        Int32 order = 0;
                        Double rp = PassMag / mf;
                        Double rs = StopMag / mf;
                        //(10^(rp/20)-1)/(10^(rp/20)+1)
                        Double pdev = (Math.Pow(10, rp / 20.0) - 1) / (Math.Pow(10, rp / 20.0) + 1);
                        //10^(-rs/20)
                        Double sdev = Math.Pow(10, -rs / 20.0);
                        switch (RespType)
                        {
                            case FilterResponseType.LowPass:
                            case FilterResponseType.HighPass:
                                ndev = 2;
                                dev = new Double[2] { pdev, sdev };
                                break;
                            case FilterResponseType.BandPass:
                                ndev = 3;
                                dev = new Double[3] { sdev, pdev, sdev };
                                break;
                            case FilterResponseType.BandStop:
                                ndev = 3;
                                dev = new Double[3] { pdev, sdev, pdev };
                                break;
                        }

                        IntPtr result = MathToolAPI.CreateFirByRemezord((Int32)RespType, LowPassFreq / ff, LowStopFreq / ff, HighPassFreq / ff, HighStopFreq / ff, dev, ndev, out order);
                        Numerator = MathToolAPI.ConvertIntPtrToDoubleArray(result, order + 1);
                    }
                    Denominator = null;
                }
                else
                {
                    Double[] wn = null;
                    if (OrderMode == FilterOrderMode.Minimum)
                    {
                        Int32 minorder = 0;
                        Double[] minwn;
                        if (RespType < FilterResponseType.BandPass)
                        {
                            minwn = new Double[2] { LowPassFreq / ff, LowStopFreq / ff };
                            wn = new Double[1];
                        }
                        else
                        {
                            minwn = new Double[4] { LowPassFreq / ff, LowStopFreq / ff, HighPassFreq / ff, HighStopFreq / ff };
                            wn = new Double[2];
                        }
                        MathToolAPI.FilterMinimumOrder((Int32)IIRMethod, minwn, minwn.Length, PassMag / mf, StopMag / mf, out minorder, wn);

                        Order = minorder;
                    }
                    else
                    {
                        wn = RespType switch
                        {
                            FilterResponseType.HighPass => new Double[1] { LowPassFreq / ff },
                            FilterResponseType.BandStop => new Double[2] { LowPassFreq / ff, LowStopFreq / ff },
                            FilterResponseType.BandPass => new Double[2] { LowPassFreq / ff, LowStopFreq / ff },
                            _ => new Double[1] { LowStopFreq / ff },
                        };
                    }
                    Int32 length = RespType >= FilterResponseType.BandPass ? 2 * Order + 1 : Order + 1;
                    Double[] ab = new Double[length];
                    Double[] bb = new Double[length];

                    switch (IIRMethod)
                    {
                        case IIRType.Butterworth:
                            MathToolAPI.Butterworth(Order, wn, (Int32)RespType, 0, ab, bb);
                            break;
                        case IIRType.ChebyshevI:
                            MathToolAPI.Chebyshv1(Order, PassMag / mf, wn, (Int32)RespType, 0, ab, bb);
                            break;
                        case IIRType.ChebyshevII:
                            MathToolAPI.Chebyshv2(Order, PassMag / mf, wn, (Int32)RespType, 0, ab, bb);
                            break;
                        case IIRType.Bessel:
                            MathToolAPI.Bessel(Order, wn, (Int32)RespType, ab, bb);
                            break;
                        case IIRType.Elliptic:
                            MathToolAPI.Elliptic(Order, PassMag / mf, StopMag / mf, wn, (Int32)RespType, 0, ab, bb);
                            break;
                    }

                    (Denominator, Numerator) = (ab, bb);
                }
            }
            catch (Exception e)
            {
            }
        }

        /// <summary>
        /// FFT调用示例
        /// </summary>
        private void FFT_test()
        {
            Int32 length = 100;
            Double[] arr ={ 0, 0.0634239196565645, 0.126592453573749, 0.189251244360410, 0.251147987181079, 0.312033445698487, 0.371662455660328, 0.429794912089172, 0.486196736100469, 0.540640817455598, 0.592907929054640, 0.642787609686539, 0.690079011482112, 0.734591708657533, 0.776146464291757, 0.814575952050336, 0.849725429949514, 0.881453363447582, 0.909631995354518, 0.934147860265107, 0.954902241444074, 0.971811568323542, 0.984807753012208, 0.993838464461254, 0.998867339183008, 0.999874127673875, 0.996854775951942, 0.989821441880933, 0.978802446214779, 0.963842158559942, 0.945000818714669, 0.922354294104582, 0.895993774291336, 0.866025403784439, 0.832569854634771, 0.795761840530832, 0.755749574354258, 0.712694171378863, 0.666769000516292, 0.618158986220606, 0.567059863862771, 0.513677391573407, 0.458226521727411, 0.400930535406614, 0.342020143325669, 0.281732556841430, 0.220310532786541, 0.158001395973350, 0.0950560433041829, 0.0317279334980681, -0.0317279334980679, -0.0950560433041826, -0.158001395973350, -0.220310532786541, -0.281732556841429, -0.342020143325669, -0.400930535406613, -0.458226521727410, -0.513677391573406, -0.567059863862771, -0.618158986220605, -0.666769000516292, -0.712694171378863, -0.755749574354258, -0.795761840530832, -0.832569854634771, -0.866025403784439, -0.895993774291336, -0.922354294104581, -0.945000818714668, -0.963842158559942, -0.978802446214779, -0.989821441880933, -0.996854775951942, -0.999874127673875, -0.998867339183008, -0.993838464461254, -0.984807753012208, -0.971811568323542, -0.954902241444074, -0.934147860265107, -0.909631995354519, -0.881453363447582, -0.849725429949514, -0.814575952050336, -0.776146464291757, -0.734591708657534, -0.690079011482113, -0.642787609686540, -0.592907929054640, -0.540640817455597, -0.486196736100469, -0.429794912089172, -0.371662455660328, -0.312033445698487, -0.251147987181079, -0.189251244360411, -0.126592453573750, -0.0634239196565654, -2.44929359829471e-16
        };
            Int32 n = (Int32)Math.Pow(2, Math.Ceiling(Math.Log(arr.Length) / Math.Log(2)));
            Double[] real=new Double[n]
                , imag= new Double[n];
            
            MathToolAPI.FFText(arr, length,n,real,imag);

        }

        #region INotifyPropertyChanged
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

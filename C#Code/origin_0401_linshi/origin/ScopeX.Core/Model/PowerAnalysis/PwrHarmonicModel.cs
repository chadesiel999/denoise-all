namespace ScopeX.Core.PowerAnalysis
{
    using MathNet.Numerics.IntegralTransforms;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;
    using ScopeX.MathExt;
    using ScopeX.Measure;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    internal class PwrHarmonicModel : AdvancedMathModel, INotifyPropertyChanged
    {
        public class DistortionItem
        {
            private Double _Current;

            public Double Current
            {
                get => _Current;

                set
                {
                    _Current = value;
                    if (!Double.IsNaN(value))
                    {
                        StaBuffer.Insert(value);
                    }
                }
            }

            public readonly StatisticBuffer StaBuffer;

            public DistortionItem(Int32 size = 1000)
            {
                StaBuffer = new(size);
            }
        };

        public PwrHarmonicModel(PowerAnalysisModel pam, MeasureModel mm) : base("", DrawMethod.Bar)
        {
            Analysis = pam;
            _Meas = mm;

            _Distortions = new ConcurrentDictionary<String, DistortionItem>()
            {
                ["THDR"] = new DistortionItem(),
                ["THDF"] = new DistortionItem(),
                ["THDrms"] = new DistortionItem(),
            };
            Count = _Distortions.Count;
        }

        public PowerAnalysisModel Analysis
        {
            get;
        }

        private readonly MeasureModel _Meas;

        private readonly ConcurrentDictionary<String, DistortionItem> _Distortions;

        public DistortionItem this[String key] => _Distortions[key];

        public readonly Int32 Count;

        private ChannelId? GetFreqRefSrc()
        {
            if (RefFreqSrc == HarmonicRefFreqSrc.HarmonicSrc)
            {
                return GetHarmonicSrc();
            }
            else if (RefFreqSrc == HarmonicRefFreqSrc.V)
            {
                return Analysis.VoltageSrc1;
            }
            else if (RefFreqSrc == HarmonicRefFreqSrc.I)
            {
                return Analysis.CurrentSrc1;
            }

            return null;
        }

        private ChannelId GetHarmonicSrc()
        {
            return Source == VIType.V ? Analysis.VoltageSrc1 : Analysis.CurrentSrc1;
        }

        private Double _Freq;
        private Double _BaseFreq;

        private void CalcBaseFreq()
        {
            _Freq = Double.NaN;
            ChannelId? freqsrc = GetFreqRefSrc();
            if (freqsrc.HasValue)
            {
                var mi = new MeasureItemModel($"{nameof(MeasParameter.Freq)}@lv", freqsrc.Value);
                mi.RefLevel.RefStandard = MeasureTopBaseRef.ZeroMax;
                var rst = _Meas.Calc.ForceGetResultOrCalc(mi);
                if (rst != null)
                {
                    _Freq = rst.Value;
                }
            }
            else
            {
                _Freq = CustomRefFreq;
            }
            //_Freq = freqsrc.HasValue ? _Meas.Calc.ForceGetResultOrCalc(nameof(MeasParameter.Freq), freqsrc.Value) ?? Double.NaN : CustomRefFreq;
        }

        public Double[,]? Magnitude
        {
            get;
            private set;
        }

        public Double[,]? Phase
        {
            get;
            private set;
        }

        private Double _FreqRes;

        public Int32[] HarmonicIndex;
        private (Prefix Prefix, String Name) _HarmonicUnit;
        public (Prefix Prefix, String Name) HarmonicUnit
        {
            get => _HarmonicUnit;
            set
            {
                if (_HarmonicUnit != value)
                {
                    _HarmonicUnit = value;
                }
            }
        }

        private Boolean _RunCompleted = true;
        public Boolean RunCompleted
        {
            get { return _RunCompleted; }
            set
            {
                if (_RunCompleted != value)
                {
                    _RunCompleted = value;
                }
            }
        }

        private Int64? _WindowId;
        public Int64? WindowId
        {
            get => _WindowId;
            set
            {
                if (_WindowId != value)
                {
                    _WindowId = value;
                    OnPropertyChanged();
                }
            }
        }
        //public void CalcHarmonicEX(WfmPack pkg, Int32 length = 2048)
        //{
        //    System.Numerics.Complex[] input = pkg.Buffer.Cast<Double>().Select(x => new System.Numerics.Complex(x, 0)).ToArray();
        //    Int32 len = input.Length;
        //    MathNet.Numerics.IntegralTransforms.Fourier.Forward(input, MathNet.Numerics.IntegralTransforms.FourierOptions.Matlab);
        //    //Double[] data = pkg.Buffer.Cast<Double>().ToArray();
        //    //Double[] dataR, dataI, dataA;
        //    Double[] dataA;
        //    //Int32 len = data.Length;
        //    //Int32 len = (Int32)Math.Pow(2, Math.Ceiling(Math.Log(data.Length) / Math.Log(2)));
        //    ////Int32 len = (Int32)Math.Pow(2, n);
        //    //dataR = new Double[len];
        //    //dataI = new Double[len];
        //    dataA = new Double[len];

        //    //MathToolAPI.FFText(data, data.Length, len, dataR, dataI);

        //    Magnitude = new Double[1, len];
        //    Phase = new Double[1, len];
        //    for (Int32 i = 0; i < len; i++)
        //    {
        //        //dataA[i] = dataR[i] * dataR[i] + dataI[i] * dataI[i];
        //        dataA[i] = input[i].Magnitude;
        //        Magnitude[0, i] = dataA[i];
        //        //Phase[0,i] = Math.Atan2(dataR[i], dataI[i]) * 180 / Math.PI;
        //        Phase[0, i] = Math.Atan2(input[i].Real, input[i].Imaginary) * 180 / Math.PI;
        //    }

        //    Double maxMagnitude = 0;
        //    HarmonicIndex = new Int32[HarmonicNum];
        //    _HarmonicIndex.Clear();
        //    // 寻找最大幅值的频率索引
        //    for (Int32 i = 1; i < len / 2; i++)
        //    {
        //        if (dataA[i] > maxMagnitude)
        //        {
        //            maxMagnitude = dataA[i];
        //            HarmonicIndex[0] = i;
        //        }
        //    }
        //    //_HarmonicIndex.Add(HarmonicIndex[0]);
        //    // 计算基频率及谐波分量
        //    _BaseFreq = _Freq;// 1.0 / (pkg.Properties.SampInterval * len) * HarmonicIndex[0];//_Freq /*/ len * HarmonicIndex[0]*/;
        //    _FreqRes = 1.0 / (pkg.Properties.SampInterval * length);
        //    for (Int32 i = 0; i < HarmonicNum; i++)
        //    {
        //        var index = HarmonicIndex[0] * (i + 1);
        //        if (index < len / 2)
        //        {
        //            HarmonicIndex[i] = index;
        //            _HarmonicIndex.Add(index);
        //        }

        //    }
        //    _HarmonicUnit = pkg.Properties.ChnlUnit;


        //}

        //public void CalcHarmonic(WfmPack pkg)
        //{
        //    Double[] data = pkg.Buffer.Cast<Double>().ToArray();
        //    Int32 n = (Int32)Math.Log(data.Length, 2);
        //    Int32 len = (Int32)Math.Pow(2, n);
        //    System.Numerics.Complex[] dataComp = new System.Numerics.Complex[len];
        //    System.Numerics.Complex[] fftResult = new System.Numerics.Complex[len];
        //    for (Int32 i = 0; i < len; i++)
        //    {
        //        dataComp[i] = new System.Numerics.Complex(data[i], 0);
        //    }
        //    fftResult = ScopeX.MathExt.Algorithm.FFT(dataComp.ToImmutableList()).ToArray();
        //    Double maxMagnitude = 0;
        //    HarmonicIndex = new Int32[HarmonicNum];
        //    _HarmonicIndex.Clear();
        //    // 寻找最大幅值的频率索引
        //    for (Int32 i = 1; i < fftResult.Length / 2; i++)
        //    {
        //        if (fftResult[i].Magnitude > maxMagnitude)
        //        {
        //            maxMagnitude = fftResult[i].Magnitude;
        //            HarmonicIndex[0] = i;
        //        }
        //    }
        //    //_HarmonicIndex.Add(HarmonicIndex[0]);
        //    // 计算基频率及谐波分量
        //    _BaseFreq = _Freq;// 1.0 / (pkg.Properties.SampInterval * len) * HarmonicIndex[0];//_Freq /*/ len * HarmonicIndex[0]*/;
        //    for (Int32 i = 0; i < HarmonicNum; i++)
        //    {
        //        var index = HarmonicIndex[0] * (i + 1);
        //        if (index < len / 2)
        //        {
        //            HarmonicIndex[i] = index;
        //            _HarmonicIndex.Add(index);
        //        }

        //    }
        //    _HarmonicUnit = pkg.Properties.ChnlUnit;

        //}
        //private void CalcSpectrumEx(WfmPack pkg, Int32 length = 2048)
        //{
        //    //var vec = new Vector(pkg.Buffer.Multiply(1E-3), pkg.Properties.TmbUnit.Name, pkg.Properties.ChnlUnit.Name, pkg.Properties.SampInterval, pkg.Properties.ChnlScale.Value * 1E-3);

        //    //_MagVector = Execute.FFT(vec, WindowType.Hann, FFTResultOpt.Ampltd, length, FFTCoordUnit.Vrms);
        //    Double[] data = pkg.Buffer.Cast<Double>().ToArray();
        //    Int32 n = (Int32)Math.Log(data.Length, 2);
        //    length = (Int32)Math.Pow(2, n);
        //    _FreqRes = _Freq;

        //    var res = pkg.Buffer.ToRowEnumerable().Select(row => row.Select(o => o * 1E-3)).Select(row => row.FFT(WindowType.Hann, length));
        //    Magnitude = res.Select(o => o.Spec.Take(length / 2).MagSpectrum(o.Factor)/*.Select(o => 20 * Math.Log10(o))*/).ToRowMatrix();

        //    Phase = res.Select(o => o.Spec.PhaseSpectrum()).ToRowMatrix();
        //}

        //static Double wincorrect = 4.6391f;

        ////private void CalcSpectrum(WfmPack pack)
        ////{
        ////    var data = pack.Buffer.Cast<Double>().ToList();
        ////    var deltaf = 1 / (pack.Properties.SampInterval * _BaseFreq * 3);
        ////    Int32 len = (Int32)Math.Pow(2, Math.Floor(Math.Log(data.Count) / Math.Log(2)));
        ////    data = PadZero(data, len);
        ////    var win = Algorithm.GetWindowCoefficient(len, WindowType.Flattop);//平顶窗
        ////    var arr = data.Select(x => new System.Numerics.Complex(x * 1E-3, 0.0));
        ////    var rst = arr.FFT().ToList();
        ////    var temp = rst.Take(len / 2).Select(x => x.Magnitude).Select(x => x / (len / 2)* wincorrect).ToList();
        ////    if (temp != null)
        ////    {
        ////        Magnitude = new Double[1, temp.Count];
        ////        Phase = new Double[1, temp.Count];
        ////        for (Int32 i = 0; i < temp.Count; i++)
        ////        {
        ////            Magnitude[0, i] = temp[i];
        ////            Phase[0, i] = rst[i].Phase * 180.0 / Math.PI;
        ////        }
        ////    }

        ////    var maxi = 0;
        ////    // 寻找最大幅值的频率索引
        ////    for (Int32 i = 1; i < temp.Count; i++)
        ////    {
        ////        if (temp[i] > temp[maxi])
        ////        {
        ////            maxi = i;
        ////        }
        ////    }

        ////    HarmonicIndex = new Int32[HarmonicNum];
        ////    HarmonicIndex[0] = maxi;
        ////    //_HarmonicIndex.Add(HarmonicIndex[0]);
        ////    // 计算基频率及谐波分量
        ////    _BaseFreq = _Freq;// 1.0 / (pkg.Properties.SampInterval * len) * HarmonicIndex[0];//_Freq /*/ len * HarmonicIndex[0]*/;
        ////    for (Int32 i = 0; i < HarmonicNum; i++)
        ////    {
        ////        var index = HarmonicIndex[0] * (i + 1);
        ////        if (index < len / 2)
        ////        {
        ////            HarmonicIndex[i] = index;
        ////            _HarmonicIndex.Add(index);
        ////        }

        ////    }
        ////    _HarmonicUnit = pack.Properties.ChnlUnit;

        ////    //CalcResult();
        ////}

        private (Double[,]?, Double[,]?, List<Int32>) CalcSpectrum(WfmPack pack)
        {
            _HarmonicUnit = pack.Properties.ChnlUnit;
            var data = pack.Buffer.Cast<Double>().ToList();
            if (_Freq == 0 || !Double.IsFinite(_Freq))
            {
                return (null, null, new List<Int32>());
            }
            Double[,] mag = null, phase = null;
            List<Int32> harm = new List<Int32>();
            _BaseFreq = Math.Round(_Freq);
            var fs = 1 / pack.Properties.SampInterval;
            var k_base = (Int32)Math.Ceiling(_BaseFreq / fs * data.Count);
            Int32 length = (Int32)(k_base * fs / _BaseFreq);
            var cof = pack.Buffer.GetLength(1) / 2.0 * Math.Sqrt(2);
            if (pack.Buffer.GetLength(1) != length)
            {
                for (Int32 i = pack.Buffer.GetLength(1); i < length; i++)
                {
                    data.Add(0);
                }
            }

            var fftdata = data.Select(x => new System.Numerics.Complex(x * 1E-3, 0.0)).ToArray();

            Fourier.Forward(fftdata, FourierOptions.Matlab);
            var arr = fftdata.Select(x => x / cof).ToArray();
            if (arr != null)
            {
                mag = new Double[1, length];
                phase = new Double[1, length];
                for (Int32 i = 0; i < length; i++)
                {
                    mag[0, i] = arr[i].Magnitude;
                    phase[0, i] = arr[i].Phase * 180.0 / Math.PI;
                }
            }

            HarmonicIndex = new Int32[HarmonicNum];
            harm.Clear();
            HarmonicIndex[0] = k_base;

            // 计算基频率及谐波分量
            for (Int32 i = 0; i < HarmonicNum; i++)
            {
                var index = HarmonicIndex[0] * (i + 1);
                if (index < length / 2)
                {
                    HarmonicIndex[i] = index;
                    harm.Add(index);
                }
            }

            return (mag, phase, harm);
        }

        private List<Double> PadZero(List<Double> span, Int32 n)
        {
            Int32 num = 1;
            if (!((n & (n - 1)) == 0))
            {
                while (num < n)
                {
                    num *= 2;
                }

                n = num;
            }

            Int32 num2 = span.Count();
            return num2 >= n ? span.Take(2 * n).ToList() : span.Concat(Enumerable.Repeat(0.0, n - num2)).ToList();
        }

        private List<Int32> _HarmonicIndex = new();

        /// <summary>
        /// THD-R=sqrt(H2+H3+H4+...)/sqrt(H1+H2+H3+H4+...)
        /// THD-F=sqrt(H2+H3+H4+...)/sqrt(H1）
        /// H1表示基波的平方，H2 表示二次谐波的平方，H3 表示三次谐波的平方，其余依此类推。 
        /// </summary>
        private void CalcTHD()
        {
            if (Magnitude is null || _HarmonicIndex == null || _HarmonicIndex.Count == 0)
            {
                _Distortions["THDR"].Current = Double.NaN;
                _Distortions["THDF"].Current = Double.NaN;
                return;
            }

            Double numerator = 0;
            for (Int32 i = 1; i < _HarmonicIndex.Count; i++)
            {
                numerator += Magnitude[0, _HarmonicIndex[i]] * Magnitude[0, _HarmonicIndex[i]];
            }

            Double basemag = Magnitude[0, _HarmonicIndex[0]];
            Double denominator = basemag * basemag + numerator;

            denominator = Math.Sqrt(denominator);
            numerator = Math.Sqrt(numerator);

            if (Double.IsFinite(denominator) && denominator != 0 && Double.IsFinite(basemag) && basemag != 0)
            {
                _Distortions["THDR"].Current = numerator / denominator * 100;
                _Distortions["THDF"].Current = numerator / basemag * 100;
            }
            else
            {
                _Distortions["THDR"].Current = Double.NaN;
                _Distortions["THDF"].Current = Double.NaN;
            }
        }

        private void CalcTHDrms()
        {
            Double data = _Meas.Calc.ForceGetResultOrCalc(nameof(MeasParameter.RMS), GetHarmonicSrc()) ?? Double.NaN;
            if (Double.IsFinite(data))
            {
                _Distortions["THDrms"].Current = Quantity.ConvertByPrefix(data, MeasureProc.GetPfxUnitString("RMS", GetHarmonicSrc()).Prefix);
            }
            else
            {
                _Distortions["THDrms"].Current = Double.NaN;
            }
        }

        public Double GetHarmonicFreq(Int32 i)
        {
            return i < _HarmonicIndex.Count ? !Double.IsNaN(_BaseFreq) ? _BaseFreq * (i + 1) : _HarmonicIndex[i] * _FreqRes : Double.NaN;
        }

        public Double GetMagRMS(Int32 i)
        {
            return i < _HarmonicIndex.Count ? Magnitude?[0, _HarmonicIndex[i]] ?? Double.NaN : Double.NaN;
        }

        public Double GetMagRatio(Int32 i)
        {
            return i < _HarmonicIndex.Count
                ? Magnitude?[0, _HarmonicIndex[i]] / Magnitude?[0, _HarmonicIndex[0]] * 100 ?? Double.NaN
                : Double.NaN;
        }

        public Double GetPhase(Int32 i)
        {
            return i < _HarmonicIndex.Count ? Phase?[0, _HarmonicIndex[i]] ?? Double.NaN : Double.NaN;
        }


        private VIType _Source = VIType.V;
        public VIType Source
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
        private SweepType _Unit = SweepType.Linear;

        public SweepType Unit
        {
            get => _Unit;
            set
            {
                if (_Unit != value)
                {
                    _Unit = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _HarmonicNum = 40;
        public Int32 HarmonicNum
        {
            get => _HarmonicNum;
            set
            {
                if (_HarmonicNum != value)
                {
                    _HarmonicNum = value;
                    UpdateHarmonicIndexes();
                    OnPropertyChanged();
                }
            }
        }
        public readonly String Titles = "Frequency (hz),Measurement Value (%),Root Mean Square of Measurement,Phase";
        public Int32 MinHarmonicNum = 40;

        public Int32 MaxHarmonicNum = 100;

        private Int32 _HarmonicNumIdx = 0;
        public Int32 HarmonicNumIdx
        {
            get => _HarmonicNumIdx;
            set
            {
                if (_HarmonicNumIdx != value)
                {
                    _HarmonicNumIdx = value;
                    OnPropertyChanged();
                }
            }
        }

        private HarmonicDisplayOpt _HarmonicOpt = HarmonicDisplayOpt.All;
        public HarmonicDisplayOpt HarmonicOpt
        {
            get => _HarmonicOpt;
            set
            {
                if (_HarmonicOpt != value)
                {
                    _HarmonicOpt = value;
                    UpdateHarmonicIndexes();
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateHarmonicIndexes()
        {
            var temp = Enumerable.Range(1, HarmonicNum);
            switch (_HarmonicOpt)
            {
                case HarmonicDisplayOpt.Odd:
                    temp = temp.Where(x => x % 2 != 0);
                    break;
                case HarmonicDisplayOpt.Even:
                    temp = temp.Where(x => x == 1 || x % 2 == 0);
                    break;
            }
            HarmonicIndexes = temp.ToList();
        }

        public List<Int32> HarmonicIndexes
        {
            get;
            set;
        } = Enumerable.Range(1, 40).ToList();

        private HarmonicStandard _Standard = HarmonicStandard.None;
        public HarmonicStandard Standard
        {
            get => _Standard;
            set
            {
                if (_Standard != value)
                {
                    _Standard = value;
                    OnPropertyChanged();
                }
            }
        }

        private HarmonicRefFreqSrc _RefFreqSrc = HarmonicRefFreqSrc.HarmonicSrc;
        public HarmonicRefFreqSrc RefFreqSrc
        {
            get => _RefFreqSrc;
            set
            {
                if (_RefFreqSrc != value)
                {
                    _RefFreqSrc = value;
                    OnPropertyChanged();
                }
            }
        }

        public const Int32 MAX_CUSTOM_FREQ = 1_000_00;//Hz

        public const Int32 MIN_CUSTOM_FREQ = 10;//Hz

        private Int64 _CustomRefFreq = 60;
        public Int64 CustomRefFreq
        {
            get => _CustomRefFreq;
            set
            {
                value = ValidateRefFreq(value);
                if (_CustomRefFreq != value)
                {
                    _CustomRefFreq = value;
                    OnPropertyChanged();
                }
            }
        }

        private static Int64 GetStepFreq(Int64 value)
        {
            var n = (Int64)Math.Log10(value);
            if (n > 0)
            {
                n--;
            }

            return (Int64)Math.Pow(10, n);
        }

        private static Int64 ValidateRefFreq(Int64 value)
        {
            if (value > MAX_CUSTOM_FREQ)
            {
                value = MAX_CUSTOM_FREQ;
            }
            else if (value < MIN_CUSTOM_FREQ)
            {
                value = MIN_CUSTOM_FREQ;
            }

            return value;
        }

        public void AdjRefFreq(Int32 delta)
        {
            CustomRefFreq += GetStepFreq(CustomRefFreq) * delta;
        }

        public void Run()
        {
            if (_RunCompleted)
            {
                _ = SingleRun();
            }
        }

        public async Task SingleRun()
        {
            try
            {
                await Task.Run(() => HarmonicAnalysis());
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }

        private void HarmonicAnalysis()
        {
            _RunCompleted = false;
            var sch = GetHarmonicSrc();
            DsoModel.Default.TryGetChannel(sch, out var chnl);
            if (chnl != null && chnl.Pack is not null)
            {
                CalcBaseFreq();
                var result = CalcSpectrum(chnl.Pack);
                Magnitude = result.Item1;
                Phase = result.Item2;
                _HarmonicIndex = result.Item3;
                CalcTHD();
                CalcTHDrms();
            }
            _RunCompleted = true;
            Thread.Sleep(10);
        }

        public void Reset()
        {
            foreach (var p in _Distortions)
            {
                p.Value.Current = Double.NaN;
                p.Value.StaBuffer.Clear();
            }
        }
        public override Vector? Take()
        {
            Int32 harmonicidx = 0;
            List<Double> mag = new List<Double>(HarmonicNum);

            while (harmonicidx < HarmonicNum)
            {
                mag.Add(GetMagRMS(harmonicidx));

                if (HarmonicOpt == HarmonicDisplayOpt.All)
                {
                    harmonicidx++;
                }
                else if (HarmonicOpt == HarmonicDisplayOpt.Odd)
                {
                    harmonicidx += 2;
                }
                else if (HarmonicOpt == HarmonicDisplayOpt.Even)
                {
                    if (harmonicidx == 0)
                    {
                        harmonicidx = 1;
                    }
                    else
                    {
                        harmonicidx += 2;
                    }
                }
            }

            Double min = mag.Min();
            Double max = mag.Max();
            Double mid = (max + min) / 2;
            min = ((min - mid) * 1.1) + mid;
            max = ((max - mid) * 1.1) + mid;
            if (Double.IsNaN(min) || Double.IsNaN(max))
            {
                return null;
            }

            Int32 imax = 100;

            return new Vector(
                  mag.Select(y => y / max * imax).ToMatrix(1, mag.Count).ToEnumerable(),
                   "#",
                  Source == VIType.V ? QuantityUnit.Voltage.ToUnitString() : QuantityUnit.Ampere.ToUnitString(),
                  HarmonicNum / 10,
                    max / 8
            );
        }
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
    }
}

namespace ScopeX.Core.PowerAnalysis
{
    using MathNet.Numerics.IntegralTransforms;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;
    using ScopeX.MathExt;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.ComponentModel;
    using System.Linq;
    using System.Numerics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Vector = MathExt.Vector;

    internal class PwrLoopAnalysisModel : AdvancedMathModel, INotifyPropertyChanged
    {
        public PwrLoopAnalysisModel(PowerAnalysisModel pam, MeasureModel mm) : base("", DrawMethod.Bar)
        {
            Analysis = pam;
            _Meas = mm;
            for (Int32 i = 0; i < AmpValue.Length; i++)
            {
                AmpValue[i] = _Amplitude;
            }
            Data = new DataOpt[_ScanNum];
        }

        public PowerAnalysisModel Analysis
        {
            get;
        }

        private readonly MeasureModel _Meas;

        private Int32 _CalcIndex = 0;

        private Object _Locker = new Object();

        private AWGId _AWGSource = AWGId.G1;
        public AWGId AWGSource
        {
            get => _AWGSource;
            set
            {
                if (_AWGSource != value)
                {
                    _AWGSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private ScanMode _Scan = ScanMode.Single;
        public ScanMode Scan
        {
            get => _Scan;
            set
            {
                if (_Scan != value)
                {
                    _Scan = value;
                    OnPropertyChanged();
                }
            }
        }

        private ImpedanceType _Impedance = ImpedanceType.Low50;
        public ImpedanceType Impedance
        {
            get => _Impedance;
            set
            {
                if (_Impedance != value)
                {
                    _Impedance = value;
                    if (_Impedance == ImpedanceType.Low50)
                    {
                        Amplitude /= 2.0;
                        AmpValue = AmpValue.Select(x => x /= 2.0).ToArray();
                    }
                    else
                    {
                        Amplitude *= 2;
                        AmpValue = AmpValue.Select(x => x *= 2.0).ToArray();
                    }
                    OnPropertyChanged();
                }
            }
        }

        public readonly Int64 MAX_FREQ = 50_000_000;

        public readonly Int64 MIN_FREQ = 10;

        public readonly String Titles = "Frequency,Amplitude,Gain,Phase";
        private Int64 _StartFreq = 1_000;
        public Int64 StartFreq
        {
            get => _StartFreq;
            set
            {
                value = ValidateFreq(value);
                if (_StartFreq != value)
                {
                    _StartFreq = value;
                    OnPropertyChanged();
                }
            }
        }
        private Int64 _EndFreq = 1_000_000;
        public Int64 EndFreq
        {
            get => _EndFreq;
            set
            {
                value = ValidateFreq(value);
                if (_EndFreq != value)
                {
                    _EndFreq = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Int32 MAX_ScanNum = 1_000;

        public readonly Int32 MIN_ScanNum = 1;

        private Int32 _ScanNum = 10;
        public Int32 ScanNum
        {
            get => _ScanNum;
            set
            {
                value = ValidateScanNum(value);
                if (value != _ScanNum)
                {
                    _ScanNum = value;
                    Data = new DataOpt[_ScanNum];
                    OnPropertyChanged();
                }
            }
        }

        private AmplitudeMode _AmplitudeMode = AmplitudeMode.Constant;
        public AmplitudeMode AmplitudeMode
        {
            get => _AmplitudeMode;
            set
            {
                if (value != _AmplitudeMode)
                {
                    _AmplitudeMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly Double MaxAmplitude = 3_000;
        public readonly Double MinAmplitude = 10;

        private Double _Amplitude = 1_000;
        public Double Amplitude
        {
            get => _Amplitude;
            set
            {
                value = ValidateAmp(value);
                if (_Amplitude != value)
                {
                    _Amplitude = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 ValidateScanNum(Int32 value)
        {
            if (value > MAX_ScanNum)
            {
                value = MAX_ScanNum;
            }
            else if (value < MIN_ScanNum)
            {
                value = MIN_ScanNum;
            }

            return value;
        }

        private Int64 ValidateFreq(Int64 value)
        {
            if (value > MAX_FREQ)
            {
                value = MAX_FREQ;
            }
            else if (value < MIN_FREQ)
            {
                value = MIN_FREQ;
            }

            Int64 step = GetStepFreq(value);

            value = (value / step) * step;

            return value;
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

        private Double ValidateAmp(Double value)
        {
            Int32 scale = 1;
            if (Impedance == ImpedanceType.High1M)
            {
                scale = 2;
            }
            if (value > MaxAmplitude * scale)
            {
                value = MaxAmplitude * scale;
            }
            else if (value < MinAmplitude * scale)
            {
                value = MinAmplitude * scale;
            }

            return value;
        }
        public Double[] AmpValue = new Double[8];

        public void SetAmplitudeValue(Int32 index, Double value)
        {
            if ((index < AmpValue.Length))
            {
                AmpValue[index] = value;
                OnPropertyChanged(nameof(AmpValue));
            }
        }

        public Double GetAmplitudeValue(Int32 index)
        {
            return index < AmpValue.Length ? AmpValue[index] : 0;
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
                    _UpdateData = true;
                    OnPropertyChanged();
                }
            }
        }

        public DataOpt[] Data = new DataOpt[10];
        private Boolean _CalcCompleted = true;
        public Boolean CalcCompleted
        {
            get { return _CalcCompleted; }
            set
            {
                if (_CalcCompleted != value)
                {
                    _CalcCompleted = value;
                }
            }
        }
        private Double _PmFre = Double.MaxValue;
        public Double PmFre
        {
            get => _PmFre;
            set
            {
                if (_PmFre != value)
                {
                    _PmFre = value;
                }
            }
        }
        private Double _PmPha = Double.MaxValue;
        public Double PmPha
        {
            get => _PmPha;
            set
            {
                if (_PmPha != value)
                {
                    _PmPha = value;
                }
            }
        }

        private Double _GmFre = Double.MaxValue;
        public Double GmFre
        {
            get => _GmFre;
            set
            {
                if (_GmFre != value)
                {
                    _GmFre = value;
                }
            }
        }
        private Double _GmAmp = Double.MaxValue;
        public Double GmAmp
        {
            get => _GmAmp;
            set
            {
                if (_GmAmp != value)
                {
                    _GmAmp = value;
                }
            }
        }

        private Int32 _DataCount = 0;
        public Int32 DataCount
        {
            get => _DataCount;
            set
            {
                if (_DataCount != value)
                {
                    _DataCount = value;
                    _UpdateData = true;
                }
            }
        }

        private Boolean _UpdateData = true;
        public Boolean UpdateData
        {
            get
            {
                var temp = _UpdateData;
                _UpdateData = false;
                return temp;
            }
            private set => _UpdateData = value;
        }

        private Boolean _RunFlag = false;
        public Boolean RunFlag
        {
            get => _RunFlag;
            set
            {
                if (_RunFlag != value)
                {
                    if (!_RunFlag && value)
                    {
                        DataCount = 0;
                        Data = new DataOpt[_ScanNum];
                        _GmFre = Double.MaxValue;
                        _GmAmp = Double.MaxValue;
                        _PmFre = Double.MaxValue;
                        _PmPha = Double.MaxValue;
                        WeakTip.Default.Write(nameof(PowerAnalysisOpt.LoopAnalysis), MsgTipId.RunningLoopAnalysisTip, false, "", Int32.MaxValue);
                        WeakTip.Default.Enabled = false;
                        PowerAnalysisTools.OpenPwrLoopFlag(true);
                    }
                    if (_RunFlag && !value)
                    {
                        WeakTip.Default.Enabled = true;
                        WeakTip.Default.Write(nameof(PowerAnalysisOpt.LoopAnalysis), MsgTipId.CompletedLoopAnalysisTip, false, "", 1);
                        PowerAnalysisTools.OpenPwrLoopFlag(false);
                    }
                    _RunFlag = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _CheckTriggerStatus = true;
        private Int32 _CheckTriggerStatusNum = 10;
        private const Int32 _MaxCheckTriggerStatusNum = 10;
        public Boolean CheckTriggerStatus
        {
            get => _CheckTriggerStatus;
            set
            {
                if (_CheckTriggerStatus != value)
                {
                    _CheckTriggerStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Run()
        {
            if (RunFlag && CalcCompleted)
            {
                _ = SingleRun();
            }
        }

        public async Task SingleRun()
        {
            try
            {
                await Task.Run(() => LoopAnalysis());
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }

        private void LoopAnalysis()
        {
            CalcCompleted = false;
            RunAnalysis();
            RunFlag = false;
            CalcCompleted = true;
        }

        private void RunAnalysis()
        {
            if (!CheckSystemState())
            {
                return;
            }

            if (!InitAWG(out var awg))
            {
                return;
            }

            if (!InitAnalog(out var vol, out var cur))
            {
                return;
            }

            Thread.Sleep(1000);
            InitScanPoint();//计算相应扫描点的频率和幅度
            _PmPha = Double.MaxValue;
            _GmAmp = Double.MaxValue;
            Int32 gmflag = 0;
            Int32 pmflag = 0;
            for (Int32 i = 0; i < ScanNum; i++)
            {
                if (!CheckSystemState())
                {
                    return;
                }
                SetTimebase(Data[i].Freq, 10);
                WaveGenByAWG(awg, Data[i].Freq, Data[i].Amp);
                Thread.Sleep(2000);
                SetAnalogPara(vol, cur, Data[i].Amp);
                Thread.Sleep(500);
                CalcBodeValue(awg, i);
                CalcGmPm(ref gmflag, ref pmflag);
                DataCount = i + 1;
            }
            awg.Active = false;
        }


        private Boolean CheckSystemState()
        {
            while (TriggerPrsnt.State == SysState.Stop)
            {
                Thread.Sleep(100);
            }
            if (Analysis == null || Analysis.Active == false)
            {
                return false;
            }
            if (DsoPrsnt.DefaultDsoPrsnt.Timebase.Mode != AnaChnlAcqMode.HighRes)
            {
                DsoPrsnt.DefaultDsoPrsnt.Timebase.Mode = AnaChnlAcqMode.HighRes;
            }
            return true;
        }


        private Boolean InitAWG(out ArbWfmGenModel awg)
        {
            awg = DsoModel.Default.GetWfmGenerator(ChannelId.AWG1 + (Int32)_AWGSource);
            if (awg == null)
            {
                return false;
            }
            awg.Active = true;
            awg.WfmType = ArbWfmType.DC;

            if (!CheckSystemState())
            {
                return false;
            }

            return awg.Active;
        }

        private Boolean InitAnalog(out AnalogPrsnt vol, out AnalogPrsnt cur)
        {
            vol = DsoPrsnt.DefaultDsoPrsnt.GetAllChnls().Where(x => x.Id == Analysis.VoltageSrc1).FirstOrDefault() as AnalogPrsnt;
            cur = DsoPrsnt.DefaultDsoPrsnt.GetAllChnls().Where(x => x.Id == Analysis.CurrentSrc1).FirstOrDefault() as AnalogPrsnt;
            if (vol == null || cur == null)
            {
                return false;
            }
            if (vol.Active == false)
            {
                vol.Active = true;

            }
            vol.ResetPosIndex();
            vol.Bias = 0;
            if (cur.Active == false)
            {
                cur.Active = true;
            }
            cur.ResetPosIndex();
            cur.Bias = 0;
            var analogs = ChannelIdExt.GetAnalogs();
            foreach (var chnl in analogs)
            {
                if (chnl != vol.Id && chnl != cur.Id && DsoModel.Default.TryGetChannel(chnl, out var model) && model != null && model.Active)
                {
                    model.Active = false;
                }
            }

            if (!CheckSystemState())
            {
                return false;
            }

            return cur.Active && vol.Active;
        }

        private void WaveGenByAWG(ArbWfmGenModel awg, Double freq, Double amp)
        {
            if (!awg.Active)
                return;
            awg.Mode = WfmGenMode.Continuous;
            awg.WfmType = ArbWfmType.Sinusoid;
            //需要先设置阻抗
            awg.Impedance = Impedance == ImpedanceType.High1M ? WfmGenImpedance.HighZ : WfmGenImpedance.Low50;

            awg.Frequency = (Int64)(freq * 1E6);
            awg.Amplitude = Impedance == ImpedanceType.Low50 ? (Int32)amp * 2 : (Int32)amp;
            awg.Offset = 0;
            awg.Phase = 0;

            Hardware.HdCmdFactory.Push(HdCmd.AWGConfig);
            Thread.Sleep(300);
        }

        public void CalcBodeValue(ArbWfmGenModel awg, Int32 index)
        {
            if (!awg.Active)
            {
                awg.Active = true;
                Thread.Sleep(1500);
            }

            _CheckTriggerStatusNum = _MaxCheckTriggerStatusNum;
            while (_CheckTriggerStatusNum > 0 && _CheckTriggerStatus)
            {
                _CheckTriggerStatusNum--;
                CheckTrigger();//检查触发类型和触发源
                if (TriggerModel.State == SysState.Triged)
                {
                    break;
                }
            }

            if (_CheckTriggerStatus && TriggerModel.State != SysState.Triged)
            {
                return;
            }


            //获取数据
            var inputpkg = DsoModel.Default.GetWfmPack(Analysis.VoltageSrc1);
            var outputpkg = DsoModel.Default.GetWfmPack(Analysis.CurrentSrc1);
            Double inputamp = 0;
            Double outputamp = 0;
            Double inputphase;
            Double outputphase;
            //计算增益裕度和相位裕度
            if (inputpkg == null || outputpkg == null)
            {
                return;
            }
            _CalcIndex = 0;
            Double[] inputdata = inputpkg.Buffer.Cast<Double>().ToArray();

            Double inputfs = 1 / inputpkg.Properties.SampInterval;

            inputphase = CalcPhase_Default_Ex(Data[index].Freq, inputfs, inputdata, ref inputamp);

            Double[] outputdata = outputpkg.Buffer.Cast<Double>().ToArray();

            Double outputfs = 1 / outputpkg.Properties.SampInterval;

            outputphase = CalcPhase_Default_Ex(Data[index].Freq, outputfs, outputdata, ref outputamp);

            Double phaseDelt = inputphase - outputphase;

            phaseDelt = phaseDelt > 180 ? phaseDelt - 360 : phaseDelt;

            phaseDelt = phaseDelt < -180 ? phaseDelt + 360 : phaseDelt;

            Data[index].Phase = phaseDelt;

            Data[index].Gain = CalcGain(inputamp, outputamp);
        }

        private void CheckTrigger()
        {
            var trigprsnt = TriggerPrsnt.GetOrMakeTrigger(DsoPrsnt.DefaultDsoPrsnt, TriggerType.Edge);


            if (trigprsnt != null && trigprsnt is TrigEdgePrsnt edgeprsnt)
            {
                if (edgeprsnt!.Source != Analysis.VoltageSrc1)
                {
                    edgeprsnt!.Source = Analysis.VoltageSrc1;
                }

                edgeprsnt.Slope = EdgeSlope.Rise;//固定上升沿
                edgeprsnt.ResetPosIndex();//固定居中触发
                Thread.Sleep(200);
            }
        }

        /// <summary>
        /// _GmAmp _GmFre
        /// _PmPha _PmFre 
        /// 线性插值 y0 + (x - x0) * (y1 - y0) / (x1 - x0);
        /// </summary>
        public Boolean CalcGmPm(ref Int32 gmFlag, ref Int32 pmFlag)
        {
            if (pmFlag == 1 && gmFlag == 1)
            {
                return true;
            }

            Int32 length = Data.Length;
            for (Int32 i = 0; i < length - 2; i++)
            {
                if (gmFlag != 1)
                {
                    if ((Data[i].Gain > 0 && Data[i + 1].Gain < 0))
                    {
                        _PmFre = Data[i].Freq - (Data[i].Gain * (Data[i + 1].Freq - Data[i].Freq)) / (Data[i + 1].Gain - Data[i].Gain);
                        _PmPha = Data[i].Phase + (_PmFre - Data[i].Freq) * (Data[i + 1].Phase - Data[i].Phase) / (Data[i + 1].Freq - Data[i].Freq);
                        gmFlag = 1;
                    }
                }

                if (pmFlag != 1)
                {
                    if ((Data[i].Phase > 0 && Data[i + 1].Phase < 0))
                    {
                        _GmFre = Data[i].Freq - (Data[i].Phase * (Data[i + 1].Freq - Data[i].Freq)) / (Data[i + 1].Phase - Data[i].Phase);
                        _GmAmp = Data[i].Gain + (_GmFre - Data[i].Freq) * (Data[i + 1].Gain - Data[i].Gain) / (Data[i + 1].Freq - Data[i].Freq);
                        _GmAmp = Math.Abs(_GmAmp);
                        pmFlag = 1;
                    }
                }

                if (pmFlag == 1 && gmFlag == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            _PmFre = Double.NaN;
            _PmPha = Double.NaN;
            _GmFre = Double.NaN;
            _GmAmp = Double.NaN;

            for (var i = 0; i < Data.Length; i++)
            {
                Data[i].Freq = Double.NaN;
                Data[i].Amp = Double.NaN;
                Data[i].PSRR = Double.NaN;
            }
        }

        //计算扫描点相应的的频率和幅度
        public void InitScanPoint()
        {
            Double startRateLog;
            Double endRateLog;
            if (_ScanNum < 2)
                _ScanNum = 2;
            //Data = new DataOpt[_ScanNum];
            startRateLog = Math.Log10(_StartFreq);
            endRateLog = Math.Log10(_EndFreq);
            for (Int32 i = 0; i < _ScanNum; i++)
            {
                Data[i].Freq = Math.Round(Math.Pow(10, (Double)((decimal)startRateLog + (decimal)i * ((decimal)endRateLog - (decimal)startRateLog) / ((decimal)_ScanNum - (decimal)1))), 6);
                if (AmplitudeMode == AmplitudeMode.Constant)
                {
                    Data[i].Amp = Amplitude;
                }
                else
                {
                    for (Int32 j = 7; j >= 0; j--)
                    {
                        Double freqLimit = 7 == j ? 2 * Math.Pow(10, j) : Math.Pow(10, j + 1);
                        if (Data[i].Freq >= freqLimit)
                        {
                            Data[i].Amp = AmpValue[j];
                            break;
                        }
                    }
                }

            }
        }

        public Double CalcGain(Double inputamp, Double outputamp)
        {
            Double gain = 0;
            if (0 == inputamp)
            {
                //Bode_ErroPrintf("division by zero error\n");
                return 0;
            }
            gain = 20 * Math.Log10(outputamp / inputamp); // 增益
            return gain;
        }
        public Double CalcPhase_Default_Ex(Double freq, Double fs, Double[] data, ref Double amp)
        {
            var cof = data.Length / 2.0;

            Int32 k = (Int32)Math.Ceiling(freq / fs * data.Length);

            Int32 len = (Int32)(k * fs / freq);

            Complex[] fftdata = PadZero(data, len).Select(x => new System.Numerics.Complex(x * 1E-3, 0.0)).ToArray();

            //MathToolAPI.FFText(data, data.Length, len, dataR, dataI);
            Fourier.Forward(fftdata, FourierOptions.Matlab);

            var arr = fftdata.Select(x => x / cof).ToArray();

            _CalcIndex = k;

            amp = arr[_CalcIndex].Magnitude;

            return Math.Atan2(arr[_CalcIndex].Real, arr[_CalcIndex].Imaginary) * 180 / Math.PI;	// 相位
        }
        public Double CalcPhase_Default(Double[] data, ref Double amp)
        {
            Double[] dataR, dataI, dataA;
            Int32 n = (Int32)Math.Log(data.Length, 2);
            Int32 len = (Int32)Math.Pow(2, n);
            dataR = new Double[len];
            dataI = new Double[len];
            dataA = new Double[len];
            //Int32 M = 11;
            Int32 istart = data.Length / 2 - len / 2;
            for (Int32 i = 0; i < len; i++)
            {
                dataR[i] = data[i + istart];
                //		fprintf(fp, "%d,", pData1[i+istart]);
                dataI[i] = 0;
            }
            FFT_Default(ref dataR, ref dataI, ref dataA, len);
            if (_CalcIndex == 0)
            {
                _CalcIndex = 1;
                Double temp = dataA[1];
                for (Int32 i = 1; i < dataR.Length / 2 - 1; i++)
                {
                    if (temp < dataA[i])
                    {
                        _CalcIndex = i;
                        temp = dataA[i];
                    }
                }
            }
            amp = Math.Sqrt(dataA[_CalcIndex]);
            //phase = Math.Atan2(dataR[indexMax], dataI[indexMax]) * 180 / Math.PI;	// 相位
            return Math.Atan2(dataR[_CalcIndex], dataI[_CalcIndex]) * 180 / Math.PI;	// 相位
        }

        private Double[] PadZero(Double[] data, Int32 n)
        {
            List<Double> temp = data.ToList();
            if (data.Length != n)
            {
                for (Int32 i = data.Length; i < n; i++)
                {
                    temp.Add(0);
                }
            }

            return temp.ToArray();
        }

        public void FFT_Default(ref Double[] dataR, ref Double[] dataI, ref Double[] dataA, Int32 len)
        {
            Int32 invr = -1;
            Double ur, ui, tr, ti;
            Int32 i, j, k, l, m, le, lei;
            Double pi;

            pi = Math.PI;
            // code position reverse 
            j = 0;
            for (i = 0; i < len - 1; i++)
            {
                if (i < j)
                {
                    tr = dataR[i];
                    ti = dataI[i];
                    dataR[i] = dataR[j];
                    dataI[i] = dataI[j];
                    dataR[j] = tr;
                    dataI[j] = ti;
                }
                k = len / 2;
                while (j >= k)
                {
                    j -= k;
                    k /= 2;
                }
                j += k;
            }

            //FFT(invr=1) or IFFT(invr=-1) begin 
            i = len;
            for (l = 0; i > 1; l++)
                i /= 2;
            le = 1;
            for (i = 0; i < l; i++)
            {
                le = 2 * le;
                lei = le / 2;
                for (j = 0; j < lei; j++)
                {
                    ur = Math.Cos(invr * j * pi / lei);
                    ui = -Math.Sin(invr * j * pi / lei);
                    for (k = j; k < len; k += le)
                    {
                        m = k + lei;
                        tr = dataR[m] * ur - dataI[m] * ui;
                        ti = dataR[m] * ui + dataI[m] * ur;
                        dataR[m] = dataR[k] - tr;
                        dataI[m] = dataI[k] - ti;
                        dataR[k] += tr;
                        dataI[k] += ti;
                    }
                }
            }
            invr = -1;
            if (invr == -1)
            {
                for (i = 0; i < len; i++)
                {
                    dataR[i] /= len;
                    dataI[i] /= len;
                }
            }
            //计算幅值 
            for (i = 0; i < len; i++)
            {
                dataA[i] = dataR[i] * dataR[i] + dataI[i] * dataI[i];
            }
        }

        public Double CalcPhase(Double[] data, ref Double amp)
        {
            Int32 n = (Int32)Math.Log(data.Length, 2);
            Int32 len = (Int32)Math.Pow(2, n);
            System.Numerics.Complex[] dataComp = new System.Numerics.Complex[len];
            System.Numerics.Complex[] fftResult = new System.Numerics.Complex[len];
            for (Int32 i = 0; i < len; i++)
            {
                dataComp[i] = new System.Numerics.Complex(data[i], 0);
            }
            fftResult = ScopeX.MathExt.Algorithm.FFT(dataComp.ToImmutableList()).ToArray();
            Double temp = 0;
            Double phase = 0;
            for (Int32 i = 1; i < fftResult.Length - 1; i++)
            {
                if (temp < fftResult[i].Magnitude)
                {
                    temp = fftResult[i].Magnitude;
                    phase = fftResult[i].Phase;
                }
            }
            amp = temp /*/ (fftResult.Length - 2)*/;
            return Math.Abs(phase)/* / (fftResult.Length - 2)*/ * 180 / Math.PI;
        }

        private void SetTimebase(Double freq, Int32 PeriodNum = 2)
        {
            //var freq = _Oscilloscope.Cymometer.GetFrequency();
            var periodByus = 1.0 / freq * 1e6;
            var scaleperdiv = PeriodNum * periodByus / Constants.VIS_XDIVS_NUM;//一个屏幕内PeriodNum个周期 10div

            scaleperdiv *= 8;

            var scaleindexlist = Enum.GetValues<AnaChnlTimebaseIndex>()
                .Where(index => (Int32)index >= (Int32)DsoModel.Default.Timebase.ScaleMinIndex && (Int32)index <= (Int32)DsoModel.Default.Timebase.ScaleMaxIndex).ToList();
            AnaChnlTimebaseIndex bestscale = scaleindexlist
                .OrderBy(scale => Math.Abs(DsoModel.Default.Timebase.GetScaleValue((Int32)scale, 0) - scaleperdiv))
                .First();

            if (bestscale >= DsoModel.Default.Timebase.ScanMinIndex)
            {
                bestscale = DsoModel.Default.Timebase.ScanMinIndex - 1;
            }

            DsoModel.Default.Timebase.Scale = DsoModel.Default.Timebase.GetScaleValue((Int32)bestscale, 0);
        }

        private void SetAnalogPara(AnalogPrsnt vol, AnalogPrsnt cur, Double amp)
        {
            if (vol == null || cur == null)
            {
                return;
            }


            vol.Coupling = AnaChnlCoupling.AC1M;
            cur.Coupling = AnaChnlCoupling.AC1M;

            Double volcoff = 1.0;
            Double curcoff = 1.0;

            #region 电压源垂直档位自适应
            //存在削波需要增加档位
            while (vol.Pack.Properties.Clipping != Clipping.None)
            {
                vol.ScaleBymV = vol.ScaleBymV * 2;
                Thread.Sleep(1000);
            }

            var volamp = _Meas.Calc.ForceGetResultOrCalc("Pk2Pk", Analysis.VoltageSrc1);
            if (volamp == null && volamp == 0)
            {
                volamp = 1;
            }
            if (volamp != null)
            {
                volcoff = Impedance == ImpedanceType.Low50 ? 2 : 4;
                if (vol.Coupling == AnaChnlCoupling.DC50)
                {
                    volcoff *= 2;
                }
            }

            vol.ScaleBymV = volamp!.Value / volcoff;
            #endregion

            #region 电流源垂直档位自适应
            //存在削波需要增加档位
            while (cur.Pack.Properties.Clipping != Clipping.None)
            {
                cur.ScaleBymV = cur.ScaleBymV * 2;
                Thread.Sleep(1000);
            }

            var curamp = _Meas.Calc.ForceGetResultOrCalc("Pk2Pk", Analysis.CurrentSrc1);
            if (curamp == null || curamp == 0)
            {
                curamp = 1;
            }
            if (curamp != null)
            {
                curcoff = Impedance == ImpedanceType.Low50 ? 2 : 4;
                if (cur.Coupling == AnaChnlCoupling.DC50)
                {
                    curcoff *= 2;
                }
            }

            cur.ScaleBymV = curamp!.Value / curcoff;
            #endregion

            if (!CheckSystemState())
            {
                return;
            }
        }

        public override Vector? Take()
        {
            return null;

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

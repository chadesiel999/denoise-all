// Copyright (c) ScopeX. All Rights Reserved
// <author></author>
// <date>2022/3/25</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using NPOI.SS.Formula.Functions;
    using ScopeX.ComModel;
    using ScopeX.Core.Hardware;
    using ScopeX.Core.Tools;

    public class ArbWfmGenPrsnt : MulticastPrsnt<IWfmGenView>, IWfmGenPrsnt
    {
        public ArbWfmGenPrsnt(ChannelId id, IDsoPrsnt idp, IWfmGenView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.GetWfmGenerator(id),
                ModelCreateOptions.Standalone => new(id),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };

            Model.PropertyChanged += OnPropertyChanged;
            DrawColor = ColorLookup.Default[Id.ToString()];

            if (view != null)
            {
                view.Presenter = this;
                TryAddView(view);
            }
        }

        private Color _DrawColor;

        public Color DrawColor
        {
            get => _DrawColor;
            set
            {
                if (_DrawColor != value)
                {
                    _DrawColor = value;
                    OnPropertyChanged(this, new PropertyChangedEventArgs(nameof(DrawColor)));
                }
            }
        }

        public ChannelType Type => Model.Type;

        public ChannelId Id => Model.Id;

        public String Name => Model.Name;

        public void WfmGenDoTriger()
        {
            if (Id == ChannelId.AWG1)
            {
                Hardware.HdCmdFactory.Push(HdCmd.AWGTrigger1);
            }
            else
            {
                Hardware.HdCmdFactory.Push(HdCmd.AWGTrigger2);
            }
        }
        public Boolean TirgerOutEnabel
        {
            get => Model.TirgerOutEnabel;
            set
            {
                Model.TirgerOutEnabel = value;
            }
        }

        private Boolean _EnablePointByPoint;
        public Boolean EnablePointByPoint
        {
            get => Model.EnablePointByPoint;
            set
            {
                Model.EnablePointByPoint = false;//目前无逐点输出功能，屏蔽此功能
            }
        }

        private TriggerSource _WfmGenTriger;

        public TriggerSource WfmGenTriger
        {
            get => Model.WfmGenTriger;
            set
            {
                Model.WfmGenTriger = value;
            }
        }
        public Boolean IsShow
        {
            get => Model.IsShow;
            set
            {
                Model.IsShow = value;
            }
        }
        public Boolean Active
        {
            get => Model.Active;
            set
            {
                if (!Constants.ENABLE_AWG && value)
                {
                    WeakTip.Default.Write("AWG", MsgTipId.FunctionDisabled);
                    Model.Active = false;
                    return;
                }
                Model.Active = value;

            }
        }

        public WfmGenMode Mode
        {
            get => Model.Mode;
            set
            {
                Model.Mode = value;
            }
        }

        public ArbWfmType WfmType
        {
            get => Model.WfmType;
            set
            {
                Model.WfmType = value;
                switch (QuantityUnitByAmp)
                {
                    case QuantityUnit.Vrms:
                        Int32 ampmagnification = Impedance == WfmGenImpedance.Low50 ? 2 : 1; //低阻幅度的关系是2倍
                        Double tempmaxvrm = UnitConversionVppToVrms(MaxAmplitude);
                        Double tempminvrm = UnitConversionVppToVrms(MinAmplitude);
                        if (AmplitudeVrms > tempmaxvrm)
                        {
                            AmplitudeVrms = tempmaxvrm;
                        }
                        else if (AmplitudeVrms < tempminvrm)
                        {
                            AmplitudeVrms = tempminvrm;
                        }
                        else
                        {
                            Amplitude = (Int32)Math.Round(UnitConversionVrmsToVpp(AmplitudeVrms), MidpointRounding.AwayFromZero);
                        }
                        break;
                    case QuantityUnit.dBm:
                        Double tempmaxdbm = UnitConversionVppTodBm(MaxAmplitude * 1.0 / 1000);
                        Double tempmindbm = UnitConversionVppTodBm(MinAmplitude * 1.0 / 1000);
                        if (AmplitudedBm > tempmaxdbm)
                        {
                            AmplitudedBm = tempmaxdbm;
                        }
                        else if (AmplitudedBm < tempmindbm)
                        {
                            AmplitudedBm = tempmindbm;
                        }
                        else
                        {
                            Amplitude = (Int32)UnitConversiondBmToVpp(AmplitudedBm) * 1000;
                        }
                        break;
                }
            }
        }

        public QuantityUnit QuantityUnitByAmp
        {
            get
            {
                return Model.QuantityUnitByAmp;
            }
            set
            {
                Model.QuantityUnitByAmp = value;
            }
        }

        /// <summary>
        /// 峰峰值Vpp转换为有效值Vrms
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double UnitConversionVppToVrms(Double data)
        {
            Double result = 0d;
            switch (WfmType)
            {
                case ArbWfmType.Pulse:
                case ArbWfmType.Square:
                    result = data / 2;
                    break;
                case ArbWfmType.Ramp:
                    result = data / (2 * Math.Sqrt(3));
                    break;
                case ArbWfmType.Sinusoid:
                case ArbWfmType.Noise:
                case ArbWfmType.DC:
                case ArbWfmType.Sinc:
                case ArbWfmType.ExpRise:
                case ArbWfmType.ExpFall:
                case ArbWfmType.Lorentz:
                case ArbWfmType.Haversine:
                case ArbWfmType.Gaussian:
                case ArbWfmType.ECG:
                case ArbWfmType.Arbitrary:
                default:
                    result = data / (2 * Math.Sqrt(2));
                    break;
            }
            return Math.Round(result, 3);
        }

        /// <summary>
        /// 有效值Vrms转换为峰峰值Vpp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double UnitConversionVrmsToVpp(Double data)
        {
            Double result = 0d;
            switch (WfmType)
            {
                case ArbWfmType.Pulse:
                case ArbWfmType.Square:
                    result = 2 * data;
                    break;
                case ArbWfmType.Ramp:
                    result = (2 * Math.Sqrt(3)) * data;
                    break;
                case ArbWfmType.Sinusoid:
                case ArbWfmType.Noise:
                case ArbWfmType.DC:
                case ArbWfmType.Sinc:
                case ArbWfmType.ExpRise:
                case ArbWfmType.ExpFall:
                case ArbWfmType.Lorentz:
                case ArbWfmType.Haversine:
                case ArbWfmType.Gaussian:
                case ArbWfmType.ECG:
                case ArbWfmType.Arbitrary:
                default:
                    result = (2 * Math.Sqrt(2)) * data;
                    break;
            }
            return Math.Round(result, 3);
        }

        /// <summary>
        /// 峰峰值Vpp转换为功率dBm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double UnitConversionVrmsTodBm(Double data)
        {
            //data /= (1_000 * 1.0);
            //double R = Impedance == WfmGenImpedance.Low50 ? 50 : 1_000_000;
            double R = 50;
            return Math.Round(10 * Math.Log10((data * data) / (R * 0.001)), 3);
        }

        /// <summary>
        /// 功率dBm 转换为有效值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double UnitConversiondBmToVrms(Double data)
        {
            double powerW = 0.001 * Math.Pow(10, data / 10);
            double R = Impedance == WfmGenImpedance.Low50 ? 50 : 1_000_000;
            return Math.Round(Math.Sqrt(powerW * R), 3);
        }

        /// <summary>
        /// 峰峰值Vpp转换为功率dBm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double UnitConversionVppTodBm(Double data)
        {
            return Math.Round(UnitConversionVrmsTodBm(UnitConversionVppToVrms(data)), 3);
        }

        /// <summary>
        /// 功率dBm 转换为峰峰值Vpp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public double UnitConversiondBmToVpp(Double data)
        {
            double powerW = 0.001 * Math.Pow(10, data / 10);
            double R = Impedance == WfmGenImpedance.Low50 ? 50 : 1_000_000;
            double Vrms = Math.Sqrt(powerW * R);
            return Math.Round(UnitConversionVrmsToVpp(Vrms), 3);
        }

        public Double AmpliteudeValue
        {
            get
            {
                switch (QuantityUnitByAmp)
                {
                    case QuantityUnit.Vrms:
                        return AmplitudeVrms;
                    case QuantityUnit.dBm:
                        return AmplitudedBm;
                    case QuantityUnit.VoltagePeakPeak:
                    default:
                        return Amplitude;
                }
            }
        }

        public Double AmplitudeVrms
        {
            get { return GetAmpBasedImp(Model.AmplitudeVrms); }
            set
            {
                Double amp = SetAmpImp(value);
                Model.AmplitudeVrms = amp;
                Int32 ampmagnification = Impedance == WfmGenImpedance.Low50 ? 2 : 1; //低阻幅度的关系是2倍
                Model.Amplitude = (Int32)Math.Round(UnitConversionVrmsToVpp(value), MidpointRounding.AwayFromZero) * ampmagnification;
                Model.AmplitudedBm = UnitConversionVrmsTodBm(value * 1.0 / 1_000);
            }
        }
        private Int32 SetAmpImp(Double v)
        {
            if (Impedance != WfmGenImpedance.HighZ)
            {
                return (Int32)Math.Round((Constants.AWG_RES_DEF + CustomImpedance) * v / CustomImpedance);
            }
            return (Int32)v;
        }
        private Double GetAmpBasedImp(Double amp)
        {
            if (Impedance != WfmGenImpedance.HighZ)
            {
                return (Double)Math.Round(CustomImpedance * amp / (Constants.AWG_RES_DEF + CustomImpedance));
            }
            return (Double)amp;
        }


        public Double AmplitudedBm
        {
            get { return Model.AmplitudedBm; }
            set
            {
                Impedance = WfmGenImpedance.Low50;
                Model.AmplitudedBm = value;
                Model.Amplitude = (Int32)UnitConversiondBmToVpp(value) * 2 * 1000;
                Model.AmplitudeVrms = (Int32)UnitConversionVppToVrms(UnitConversiondBmToVpp(value) * 1000);
            }
        }

        /// <summary>
        /// Gets or sets the Amplitude
        /// 幅度，单位mV，取值范围20mVpp~6Vpp(@High-Z)，10mVpp~3Vpp(@50Ohm)；分辨率1mV.
        /// </summary>
        public Int32 Amplitude
        {
            get => GetVoltageBasedImp(Model.Amplitude);
            set
            {
                Int32 setamp = value;

                if (setamp > MaxAmplitude)
                {
                    setamp = MaxAmplitude;
                }
                if (setamp < MinAmplitude)
                {
                    setamp = MinAmplitude;
                }

                Int32 maxamp = MaxAmplitude;
                Int32 offsetimp = Math.Abs(Offset);

                if (offsetimp + setamp / 2 > maxamp / 2)
                {
                    if (Offset > 0)
                    {
                        Model.Offset = SetVoltageBasedImp(maxamp - setamp) / 2;
                    }
                    else
                    {
                        Model.Offset = -SetVoltageBasedImp(maxamp - setamp) / 2;
                    }
                }

                Model.Amplitude = SetVoltageBasedImp(setamp);
            }
        }
        public Int64 PulseFallTime
        {
            get => Model.PulseFallTime;
            set
            {
                Model.PulseFallTime = value;
            }
        }
        public Int64 PulseRiseTime//Byps
        {
            get => Model.PulseRiseTime;
            set
            {
                Model.PulseRiseTime = value;
            }
        }

        public Double PulseRiseTimeByus
        {
            get => PulseRiseTime / 1_000_000D;
            set => PulseRiseTime = (Int64)(value * 1_000_000D);
        }

        public Double PulseFallTimeByus
        {
            get => PulseFallTime / 1_000_000D;
            set => PulseFallTime = (Int64)(value * 1_000_000D);
        }

        public Int64 MaxPulseEdgeTime
        {
            get { return Model.MaxPulseEdgeTime; }
        }

        public Int64 MinPulseEdgeTime
        {
            get { return Model.MinPulseEdgeTime; }
        }

        /// <summary>
        /// 脉冲上升下降最大时间
        /// </summary>
        private const Double MIN_PULSE_WIDTH = 8e-9;//2.4e-9;

        public Int32 MaxAmplitude => GetVoltageBasedImp(Model.MaxAmplitude);

        public Int32 MinAmplitude => GetVoltageBasedImp(Model.MinAmplitude);

        /// <summary>
        /// Gets or sets the Offset
        /// 偏移电压，单位mV，取值范围-3V~3V(@High-Z)，-1.5V~1.5V(@50Ohm)；分辨率1mV.
        /// </summary>
        public Int32 Offset
        {
            get => GetVoltageBasedImp(Model.Offset);
            set
            {
                Int32 setoffset = value;
                if (setoffset < MinOffset)
                {
                    setoffset = MinOffset;
                }
                if (setoffset > MaxOffset)
                {
                    setoffset = MaxOffset;
                }
                Int32 amp = Amplitude;
                Int32 maxamp = MaxAmplitude;
                //int offsetImp = SetVoltageBasedImp(Math.Abs(Offset));
                if (amp / 2 + Math.Abs(setoffset) > maxamp / 2)
                {
                    Model.Amplitude = SetVoltageBasedImp(maxamp / 2 - Math.Abs(setoffset)) * 2;
                }
                switch (QuantityUnitByAmp)
                {
                    case QuantityUnit.Vrms:
                        AmplitudeVrms = UnitConversionVppToVrms(Amplitude);
                        break;
                    case QuantityUnit.dBm:
                        AmplitudedBm = UnitConversionVppTodBm(Amplitude * 1.0 / 1000);
                        break;
                }
                Model.Offset = SetVoltageBasedImp(setoffset);
                HdCmdFactory.Push(HdCmd.AWGConfig);
            }
        }
        public Int32 MaxOffset => GetVoltageBasedImp(Model.MaxOffset);
        //public Int32 MaxOffset => GetVoltageBasedImp(Model.MaxAmplitude);
        public Int32 MinOffset => GetVoltageBasedImp(Model.MinOffset);
        //public Int32 MinOffset => -GetVoltageBasedImp(Model.MinAmplitude);
        public Int32 HighLevel
        {
            get => GetVoltageBasedImp(Model.HighLevel);
            set
            {
                Model.HighLevel = value;
            }
        }

        public Int32 LowLevel
        {
            get => GetVoltageBasedImp(Model.LowLevel);
            set
            {
                Model.LowLevel = value;
            }
        }

        /// <summary>
        /// Gets or sets the Frequency
        /// 频率，单位uHz；分辨率1uHz.
        /// </summary>
        public Int64 Frequency
        {
            get => Model.Frequency;
            set
            {
                Model.Frequency = value;
            }
        }

        public Double FrequencyBymHz
        {
            get => Frequency / 1000D;
            set => Frequency = (Int64)(value * 1000D);
        }

        public Int64 MaxFrequency => Model.MaxFrequency;

        public Int64 MinFrequency => Model.MinFrequency;

        /// <summary>
        /// Gets or sets the Phase
        /// 相位，单位m°，取值范围0~360 ， 分辨率0.01 .
        /// </summary>
        public Int32 Phase
        {
            get => Model.Phase;
            set
            {
                Model.Phase = value;
            }
        }

        public Double PhaseByDegree
        {
            get => Phase / 100D;
            set => Phase = (Int32)(value * 100D);
        }

        public Int32 HalfPhase => Model.HalfPhase;

        public Int32 MaxPhase => Model.MaxPhase;

        public Int32 MinPhase => Model.MinPhase;

        /// <summary>
        /// Gets or sets the Duty
        /// 脉冲占空比，单位%，取值范围1~99%，占空比分辨率0.01% .
        /// </summary>
        public Int32 Duty
        {
            get
            {
                Model.MaxPulseEdgeTime = Model.GetMaxPulseEdge();
                return (10000 - Model.Duty);
            }//修复正负占空比
            set
            {
                Model.Duty = (10000 - value); //修复正负占空比
            }
        }

        /// <summary>
        /// 实际下发到硬件的占空比
        /// </summary>
        public Int32 RealDuty
        {
            get { return Model.RealDuty; }
        }

        public Int32 MaxDuty => Model.MaxDuty;

        public Int32 MinDuty => Model.MinDuty;

        public WfmGenImpedance Impedance
        {
            get => Model.Impedance;
            set
            {
                Model.Impedance = value;
            }
        }

        public Int32 CustomImpedance
        {
            get => Model.CustomImpedance;
            set => Model.CustomImpedance = value;
        }

        public Int32 MaxCustomImpedance => Model.MaxCustomImpedance;

        public Int32 MinCustomImpedance => Model.MinCustomImpedance;

        public Boolean Opposition
        {
            get => Model.Opposition;
            set
            {
                Model.Opposition = value;
            }
        }

        /// <summary>
        /// Gets or sets the Noise
        /// 噪声，单位%，取值范围0~100%，分辨率1%.
        /// </summary>
        public Int32 Noise
        {
            get => Model.Noise;
            set
            {
                Model.Noise = value;
            }
        }

        public Int32 MaxNoise => Model.MaxNoise;

        public Int32 MinNoise => Model.MinNoise;

        public WfmModMethod ModMethod
        {
            get => Model.ModMethod;
            set
            {
                Model.ModMethod = value;
            }
        }

        public ArbWfmType ModulatedWfm
        {
            get => Model.ModulatedWfm;
            set
            {
                Model.ModulatedWfm = value;
            }
        }

        public ImmutableList<ArbWfmType> CarrierSignalList => Model.CarrierSignalList;

        public ImmutableList<ArbWfmType> ModulatedSignalList => Model.ModulatedSignalList;

        public ImmutableList<ArbWfmType> SweepSignalList => Model.SweepSignalList;

        public WfmRampType RampType
        {
            get => Model.RampType;
            set
            {
                Model.RampType = value;
            }
        }

        public Int32 AmpDepth
        {
            get => Model.AmpDepth;
            set
            {
                Model.AmpDepth = value;
            }
        }

        public Int32 MaxAmpDepth => Model.MaxAmpDepth;

        public Int32 MinAmpDepth => Model.MinAmpDepth;

        [DisplayName("频偏")]
        public Int64 FreqBias//By uHz
        {
            get => Model.FreqBias;
            set
            {
                Model.FreqBias = value;
            }
        }

        public Double FreqBiasBymHz
        {
            get => FreqBias / 1000D;
            set => FreqBias = (Int64)(value * 1000D);
        }

        public Int64 MaxFreqBias => Model.MaxFreqBias;

        public Int64 MinFreqBias => Model.MinFreqBias;

        public Int64 ModFreq
        {
            get => Model.ModFreq;
            set
            {
                Model.ModFreq = value;
            }
        }

        public Double ModFreqBymHz
        {
            get => ModFreq / 1000D;
            set
            {
                ModFreq = (Int64)(value * 1000D);
            }
        }

        public Int64 MaxModFreq => Model.MaxModFreq;

        public Int64 MinModFreq => Model.MinModFreq;


        /// <summary>
        /// Gets or sets the PhaseBias
        /// PM相偏，单位°，取值范围0~360 ， 分辨率0.01 .
        /// </summary>
        [DisplayName("相偏")]
        public Int32 PhaseBias
        {
            get => Model.PhaseBias;
            set
            {
                Model.PhaseBias = value;
            }
        }

        public Double PhaseBiasBymC
        {
            get => PhaseBias * 1.0D;
            set => PhaseBias = (Int32)(value / 1.0D);
        }

        public SweepType SweepType
        {
            get => Model.SweepType;
            set
            {
                Model.SweepType = value;
            }
        }

        public Int64 SweepDuration
        {
            get => Model.SweepDuration;
            set
            {
                Model.SweepDuration = value;
            }
        }

        public Double SweepDurationByus
        {
            get => SweepDuration;
            set => SweepDuration = (Int64)value;
        }

        public Int64 MaxSweepDuration => Model.MaxSweepDuration;

        public Int64 MinSweepDuration => Model.MinSweepDuration;

        public Int64 SweepStartFreq
        {
            get => Model.SweepStartFreq;
            set
            {
                Model.SweepStartFreq = value;
            }
        }

        public Double SweepStartFreqBymHz
        {
            get => SweepStartFreq / 1000D;
            set => SweepStartFreq = (Int64)(value * 1000D);
        }

        public Int64 SweepEndFreq
        {
            get => Model.SweepEndFreq;
            set
            {
                Model.SweepEndFreq = value;
            }
        }
        public Double SweepEndFreqBymHz
        {
            get => SweepEndFreq / 1000D;
            set => SweepEndFreq = (Int64)(value * 1000D);
        }

        public Int64 MaxSweepFreq => Model.MaxSweepFreq;

        public Int64 MinSweepFreq => Model.MinSweepFreq;

        private protected override ArbWfmGenModel Model { get; }

        private Int32 GetVoltageBasedImp(Double v)
        {
            if (Impedance != WfmGenImpedance.HighZ)
            {
                return (Int32)Math.Round(CustomImpedance * v / (Constants.AWG_RES_DEF + CustomImpedance));
            }
            return (Int32)v;
        }

        private Int32 SetVoltageBasedImp(Double v)
        {
            if (Impedance != WfmGenImpedance.HighZ)
            {
                return (Int32)Math.Round((Constants.AWG_RES_DEF + CustomImpedance) * v / CustomImpedance);
            }
            return (Int32)v;
        }
        public List<Int32>? ArbWfmData
        {
            get => Model.ArbWfmData;
        }
        public String FilePath
        {
            get => Model.FilePath;
            set
            {
                Model.FilePath = value;
            }
        }

        public String ModFilePath
        {
            get => Model.ModFilePath;
            set
            {
                Model.ModFilePath = value;
            }
        }

        /// <summary>
        /// 连续波模式波类型
        /// </summary>
        public ArbWfmType ContinuousArbWfmType
        {
            get
            {
                return Model.ContinuousArbWfmType;
            }
            set
            {
                Model.ContinuousArbWfmType = value;
            }
        }

        /// <summary>
        /// 调制波模式波类型
        /// </summary>
        public ArbWfmType ModulationWfmType
        {
            get
            {
                return Model.ModulationWfmType;
            }
            set
            {
                Model.ModulationWfmType = value;
            }
        }

        /// <summary>
        /// 扫频模式波类型
        /// </summary>
        public ArbWfmType SweepWfmType
        {
            get
            {
                return Model.SweepWfmType;
            }
            set
            {
                Model.SweepWfmType = value;
            }
        }


        public static Int64 PeriodFromFreq(Int64 freqByuHz) => ArbWfmGenModel.PeriodFromFreq(freqByuHz);

        public static Int64 PeriodToFreq(Int64 periodByns) => ArbWfmGenModel.PeriodToFreq(periodByns);

        public static Int64 PulseWidthFromDuty(Int32 duty, Int64 periodByns) => ArbWfmGenModel.PulseWidthFromDuty(duty, periodByns);

        public static Int32 PulseWidthToDuty(Int64 pwByns, Int64 periodByns) => ArbWfmGenModel.PulseWidthToDuty(pwByns, periodByns);

        public void AdjAmpDepth(Int32 step) => Model.AdjAmpDepth(step);

        public void AdjAmplitude(Int32 step)
        {
            switch (QuantityUnitByAmp)
            {
                case QuantityUnit.Vrms:
                    AmplitudeVrms += step * Constants.AWG_AMP_STP;
                    break;
                case QuantityUnit.dBm:
                    AmplitudedBm += step * Constants.AWG_AMP_STP;
                    break;
                default:
                    Amplitude += step * Constants.AWG_AMP_STP;
                    break;
            }
        }

        public void AdjPulseRiseTime(Int32 step) => Model.AdjPulseRiseTime(step);

        public void AdjPulseFallTime(Int32 step) => Model.AdjPulseFallTime(step);

        public void AdjDuty(Int32 step) => Model.AdjDuty(step);

        public void AdjFreqBias(Int32 step) => Model.FreqBias += Model.GetFreqStep(step);

        public void AdjPhaseBias(Int32 step) => Model.AdjPhaseBias(step);

        public void AdjFrequency(Int32 step) => Model.Frequency += Model.GetFreqStep(step);

        public void AdjHighLevel(Int32 step)
        {
            if (HighLevel > MaxOffset || HighLevel < MinOffset || (HighLevel == MaxOffset && step > 0) || (HighLevel < MinOffset && step < 0))
            {
                return;
            }

            var v1 = Offset + Amplitude / 2;
            var v0 = Offset - Amplitude / 2;
            v1 += step * Constants.AWG_AMP_STP;
            MidpointRounding midpoint = step > 0 ? MidpointRounding.ToPositiveInfinity : MidpointRounding.ToNegativeInfinity;
            Offset = (Int32)Math.Round((v1 + v0) * 1.0 / 2, midpoint);
            Amplitude = v1 - v0;
            switch (QuantityUnitByAmp)
            {
                case QuantityUnit.Vrms:
                    AmplitudeVrms = UnitConversionVppToVrms(Amplitude);
                    break;
                case QuantityUnit.dBm:
                    AmplitudedBm = UnitConversionVppTodBm(Amplitude);
                    break;
            }
        }


        public void AdjLowLevel(Int32 step)
        {
            if (LowLevel > MaxOffset || LowLevel < MinOffset || (LowLevel == MaxOffset && step > 0) || (LowLevel < MinOffset && step < 0))
            {
                return;
            }
            var v1 = Offset + Amplitude / 2;
            var v0 = Offset - Amplitude / 2;
            MidpointRounding midpoint = step > 0 ? MidpointRounding.ToPositiveInfinity : MidpointRounding.ToNegativeInfinity;
            v0 += step * Constants.AWG_AMP_STP;
            Offset = (Int32)Math.Round((v1 + v0) * 1.0 / 2, midpoint); ;
            Amplitude = v1 - v0;
            switch (QuantityUnitByAmp)
            {
                case QuantityUnit.Vrms:
                    AmplitudeVrms = UnitConversionVppToVrms(Amplitude);
                    break;
                case QuantityUnit.dBm:
                    AmplitudedBm = UnitConversionVppTodBm(Amplitude);
                    break;
            }
        }

        public void AdjModFreq(Int32 step) => Model.ModFreq += Model.GetFreqStep(step);

        public void AdjNoise(Int32 step) => Model.AdjNoise(step);

        public void AdjOffset(Int32 step) => Model.AdjOffset(step);

        public void AdjPeriod(Int32 step) => Model.AdjPeriod(step);

        public void AdjPhase(Int32 step) => Model.AdjPhase(step);

        public void AdjSweepStartFreq(Int32 step) => Model.SweepStartFreq += Model.GetFreqStep(step);

        //public void AdjSweepEndFreq(Int32 step) => Model.SweepStartFreq += Model.GetFreqStep(step);
        public void AdjSweepEndFreq(Int32 step) => Model.SweepEndFreq += Model.GetFreqStep(step);
        public void AdjSweepDuration(Int32 step) => Model.AdjSweepDuration(step);

        private void GetArbWfmData(Boolean isModData = false) => Model.GetArbWfmData(isModData);

        #region 2.0添加临时代码SCPI专用  3.0是需要移除

        public Double AmplitudeBymV
        {
            get => Amplitude * 1.0;
            set => Amplitude = (Int32)value;
        }

        public Double OffsetBymV
        {
            get => Offset * 1.0;
            set => Offset = (Int32)value;
        }

        public Double HighLevelBymV
        {
            get => HighLevel * 1.0;
            set => HighLevel = (Int32)value;
        }

        public Double LowLevelBymV
        {
            get => LowLevel * 1.0;
            set => LowLevel = (Int32)value;
        }

        public Double DutyByDouble
        {
            get => Duty * 1.0;
            set => Duty = (Int32)value;
        }

        public Double AmpDepthByDouble
        {
            get => AmpDepth * 1.0;
            set => AmpDepth = (Int32)value;
        }

        #endregion
    }
}

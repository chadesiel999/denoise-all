// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/3/25</date>

namespace ScopeX.Core
{
    using NPOI.HPSF;
    using NPOI.SS.Formula.Functions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using ScopeX.ComModel;
    using ScopeX.Core.Hardware;
    using ScopeX.Controls.Common.Helper;
    using NPOI.XSSF.Streaming.Values;
    using static NPOI.HSSF.Util.HSSFColor;
    using ScopeX.Core.Tools;

    internal class ArbWfmGenModel : INotifyPropertyChanged
    {
        public ChannelType Type
        {
            get;
        } = ChannelType.SigGen;

        public ChannelId Id
        {
            get;
        }

        public String Name
        {
            get;
        }
        private Boolean _IsShow = false;
        public Boolean IsShow
        {
            get => _IsShow;
            set
            {
                if (_IsShow != value)
                {
                    _IsShow = value;
                    OnPropertyChanged(nameof(IsShow));
                }
            }
        }
        private Boolean _Active = false;
        public Boolean Active
        {
            get => _Active;
            set
            {
                if (value && !OptionsManager.Default.GetOptionAvailable(OptionType.AWG))
                {
                    WeakTip.Default.Write("AWG", MsgTipId.PurchaseOptions, duration: 4);
                    value = false;
                }

                if (_Active != value)
                {
                    _Active = value;
                    if(_Active)
                    {
                        IsShow = true;
                    }
                    // <Remark>作者：彭博 创建日期：2023/12/1 11:28:00 创建原因：添加只选择波形时才发生波形的功能 </Remark>
                    _IsSendWaveType = true;
                    OnPropertyChanged(nameof(Active));
                    DsoModel.Default.RefreshAWGStaus();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }
        private Boolean _EnablePointByPoint;
        public Boolean EnablePointByPoint
        {
            get => _EnablePointByPoint;
            set
            {
                //<Remark>更改人：彭博 创建日期：2024/2/20 13:23:00  原因：逐点输出，应按波类型的样点数，进行输出 </Remark>
                if (_EnablePointByPoint != value)
                {
                    _EnablePointByPoint = value;
                    if (ArbWfmData?.Count > 0)
                    {
                        if (_EnablePointByPoint)
                        {
                            Frequency = Frequency * ArbWfmData.Count;
                        }
                        else
                        {
                            Frequency = (Int64)(Frequency * 1.0 / ArbWfmData.Count);
                        }
                    }
                    if (!value && Frequency > MaxFrequency)
                    {
                        Frequency = MaxFrequency;
                    }
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }
        private TriggerSource _WfmGenTriger;

        public TriggerSource WfmGenTriger
        {
            get => _WfmGenTriger;
            set
            {
                if (_WfmGenTriger != value)
                {
                    _WfmGenTriger = value;
                    //<Remark>更改人：彭博 创建日期：2024/1/16 10:00:00  原因：外触发才进行唯一性判断 </Remark>
                    if (WfmGenTriger == TriggerSource.Outside)
                    {
                        PassiveTriggerSource(Id);
                        // < Remark > 更改人：彭博 创建日期：2024 / 1 / 22 15:50:00  原因：触发输出选择建议关联辅助输出 </ Remark >
                        DsoPrsnt.DefaultDsoPrsnt.Setting.AuxInputSignal = AuxInputType.Sync_AWG;
                        WeakTip.Default.Write("AWG", MsgTipId.AUXInIsRequired);
                    }
                    else
                    {
                        // < Remark > 更改人：彭博 创建日期：2024 / 1 / 22 15:50:00  原因：触发输出选择建议关联辅助输出 </ Remark >
                        DsoPrsnt.DefaultDsoPrsnt.Setting.AuxInputSignal = DsoPrsnt.DefaultDsoPrsnt.Setting.AuxInputSignal == AuxInputType.Sync_AWG ? AuxInputType.Close : DsoPrsnt.DefaultDsoPrsnt.Setting.AuxInputSignal;
                    }
                    OnPropertyChanged(nameof(WfmGenTriger));
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        /// <summary>
        /// 互斥被动设置触发源
        /// </summary>
        /// <param name="soueceId"></param>
        public void PassiveTriggerSource(ChannelId soueceId)
        {
            var awgprsnts = DsoPrsnt.DefaultDsoPrsnt.ArbWfmGens;
            foreach (var awg in awgprsnts)
            {
                if (awg.Id != soueceId && awg.Id < ChannelId.AWG3)
                {
                    var awgmodel = DsoModel.Default.GetWfmGenerator(awg.Id);
                    awgmodel.WfmGenTriger = awgmodel.WfmGenTriger == TriggerSource.Outside ? TriggerSource.Inside : awgmodel.WfmGenTriger;
                }
            }
        }

        private WfmGenMode _Mode = WfmGenMode.Continuous;
        public WfmGenMode Mode
        {
            get => _Mode;
            set
            {
                if (_Mode != value)
                {
                    _Mode = value;
                    // <Remark>作者：彭博 创建日期：2023/12/1 11:28:00 创建原因：添加只选择波形时才发生波形的功能 </Remark>
                    Frequency = ValidateFreq(Frequency);
                    if (_Mode == WfmGenMode.Sweep)
                    {
                        SweepEndFreq = SweepEndFreq > MaxSweepFreq ? MaxSweepFreq : SweepEndFreq;
                        SweepStartFreq = SweepStartFreq > MaxSweepFreq ? MaxSweepFreq : SweepStartFreq;
                    }
                    _Amplitude = ValidateAmp((Int32)_Amplitude);
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }


        private ArbWfmType _WfmType = ArbWfmType.Sinusoid;

        public ArbWfmType WfmType
        {
            get => _WfmType;
            set
            {
                //<Remark>更改人：彭博 创建日期：2023/12/4 13:48:00  原因：这类波形不支持调制或扫频时，不能选择调制和扫频嘛 </Remark>
                if (_WfmType != value && IsWfmTypeValid(Mode, value))
                {
                    _WfmType = value;
                    //<Remark>作者：彭博 创建日期：2024/03/21 17:22:00 原因：修改波形时，缓存的波形一起修改 </Remark>
                    switch (Mode)
                    {
                        case WfmGenMode.Continuous:
                            ContinuousArbWfmType = _WfmType;
                            break;
                        case WfmGenMode.Modulation:
                            ModulationWfmType = _WfmType;
                            break;
                        case WfmGenMode.Sweep:
                            SweepWfmType = _WfmType;
                            break;
                        default:
                            break;
                    }
                    _IsSendWaveType = true;
                    // <Remark>作者：彭博 创建日期：2023/12/1 11:28:00 创建原因：添加只选择波形时才发生波形的功能 </Remark>
                    Frequency = ValidateFreq(Frequency);
                    if (_Mode == WfmGenMode.Sweep)
                    {
                        SweepEndFreq = SweepEndFreq > MaxSweepFreq ? MaxSweepFreq : SweepEndFreq;
                        SweepStartFreq = SweepStartFreq > MaxSweepFreq ? MaxSweepFreq : SweepStartFreq;
                    }
                    _Amplitude = ValidateAmp((Int32)_Amplitude);
                    // <Remark>作者：彭博 创建日期：2023/11/30 11:26:00 创建原因：添加占空比最大值随频率变化的功能 </Remark>
                    SetMaxAndMinDuty();
                    String filename = "";
                    switch (value)
                    {
                        case ArbWfmType.Noise:
                            filename = "Noisewhite";
                            break;
                        case ArbWfmType.Sinc:
                            filename = "Sinc";
                            break;
                        case ArbWfmType.ExpRise:
                            filename = "ExpRise";
                            break;
                        case ArbWfmType.ExpFall:
                            filename = "ExpFall";
                            break;
                        case ArbWfmType.Lorentz:
                            filename = "Lorentz";
                            break;
                        case ArbWfmType.Haversine:
                            filename = "Haversine";
                            break;
                        case ArbWfmType.Gaussian:
                            filename = "Gaussian";
                            break;
                        case ArbWfmType.ECG:
                            filename = "ECG";
                            break;
                        default:
                            break;
                    }
                    if (!String.IsNullOrWhiteSpace(filename))
                    {
                        var filepath = Environment.CurrentDirectory + @$"\Resources\BaseWaves\{filename}.bsv";
                        if (File.Exists(filepath))
                        {
                            _FilePath = filepath;
                            ReadWaveFileData_Bsv();
                        }
                    }
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        #region 用于缓存各类模式下的发送波类型

        private ArbWfmType _ContinuousArbWfmType = ArbWfmType.Sinusoid;
        /// <summary>
        /// 连续波模式波类型
        /// </summary>
        public ArbWfmType ContinuousArbWfmType
        {
            get
            {
                return _ContinuousArbWfmType;
            }
            set
            {
                _ContinuousArbWfmType = value;
            }
        }

        private ArbWfmType _ModulationWfmType = ArbWfmType.Sinusoid;
        /// <summary>
        /// 调制波模式波类型
        /// </summary>
        public ArbWfmType ModulationWfmType
        {
            get
            {
                return _ModulationWfmType;
            }
            set
            {
                _ModulationWfmType = value;
            }
        }

        private ArbWfmType _SweepWfmType = ArbWfmType.Sinusoid;
        /// <summary>
        /// 扫频模式波类型
        /// </summary>
        public ArbWfmType SweepWfmType
        {
            get
            {
                return _SweepWfmType;
            }
            set
            {
                _SweepWfmType = value;
            }
        }

        #endregion

        private List<Int32>? _ModArbWfmData;
        public List<Int32>? ModArbWfmData
        {
            get => _ModArbWfmData;
            set
            {
                _ModArbWfmData = value;
            }
        }
        private List<Int32>? _ArbWfmData;
        public List<Int32>? ArbWfmData
        {
            get => _ArbWfmData;
            set
            {
                _ArbWfmData = value;
            }
        }
        private String _FilePath = "";
        public String FilePath
        {
            get => _FilePath;
            set
            {
                if (_FilePath != value && System.IO.File.Exists(value))
                {
                    // <Remark>作者：彭博 创建日期：2023/12/1 11:28:00 创建原因：添加只选择波形时才发生波形的功能 </Remark>
                    _IsSendWaveType = true;
                    _FilePath = value;
                    GetArbWfmData();
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }

            }
        }

        private String _ModFilePath = "";
        public String ModFilePath
        {
            get => _ModFilePath;
            set
            {
                if (_ModFilePath != value && System.IO.File.Exists(value))
                {
                    GetArbWfmData(true);
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        private WfmFormat? _WfmFileFormat
        {
            get
            {
                if (String.IsNullOrWhiteSpace(FilePath) || !File.Exists(FilePath))
                {
                    return null;
                }
                var extensionstr = FilePath.Substring(FilePath.LastIndexOf('.') + 1);
                if (String.IsNullOrWhiteSpace(extensionstr)
                    || !Enum.TryParse(typeof(WfmFormat), extensionstr, true, out Object? result))
                {
                    return null;
                }
                return (WfmFormat?)result;
            }
        }
        private WfmFormat? _ModWfmFileFormat
        {
            get
            {
                if (String.IsNullOrWhiteSpace(ModFilePath) || !File.Exists(ModFilePath))
                {
                    return null;
                }
                var extensionStr = ModFilePath.Substring(ModFilePath.LastIndexOf('.') + 1);
                if (String.IsNullOrWhiteSpace(extensionStr)
                    || !Enum.TryParse(typeof(WfmFormat), extensionStr, true, out Object? result))
                {
                    return null;
                }
                return (WfmFormat?)result;
            }
        }

        //<Remark>更改人：彭博 创建日期：2023/12/4 13:48:00  原因：这类波形不支持调制或扫频时，不能选择调制和扫频嘛 </Remark>
        private Boolean IsWfmTypeValid(WfmGenMode wfmGenMode, ArbWfmType value)
        {
            //Carrier and sweep signal are limited.
            return wfmGenMode switch
            {
                WfmGenMode.Modulation => CarrierSignalList.Exists(o => o == value),
                WfmGenMode.Sweep => SweepSignalList.Exists(o => o == value),
                _ => true,
            };
        }



        public readonly ImmutableList<ArbWfmType> CarrierSignalList = new List<ArbWfmType>()
        {
            ArbWfmType.Sinusoid,
            ArbWfmType.Square,
            ArbWfmType.Ramp,
            ArbWfmType.Arbitrary,
        }.ToImmutableList();

        public readonly ImmutableList<ArbWfmType> SweepSignalList = new List<ArbWfmType>()
        {
            ArbWfmType.Sinusoid,
            ArbWfmType.Square,
            ArbWfmType.Ramp,
            ArbWfmType.Arbitrary,
        }.ToImmutableList();

        private WfmGenImpedance _Impedance = WfmGenImpedance.HighZ;

        public WfmGenImpedance Impedance
        {
            get => _Impedance;
            set
            {
                if (_Impedance != value)
                {
                    _Impedance = value;

                    if (value == WfmGenImpedance.HighZ)
                    {
                        CustomImpedance = Constants.AWG_RES_MAX;
                        QuantityUnitByAmp = QuantityUnitByAmp == QuantityUnit.dBm ? QuantityUnit.VoltagePeakPeak : QuantityUnitByAmp;
                    }
                    else if (value == WfmGenImpedance.Low50)
                    {
                        CustomImpedance = Constants.AWG_RES_DEF;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Amplitude));
                    OnPropertyChanged(nameof(Offset));
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        private Int32 _CustomImpedance = Constants.AWG_RES_DEF;

        /// <summary>
        /// 自定义阻抗 0 - +∞ 
        /// </summary>
        public Int32 CustomImpedance
        {
            get => _CustomImpedance;
            set
            {
                value = ValidateCustomImpedance(value);
                if (_CustomImpedance != value)
                {
                    _CustomImpedance = value;
                    OnPropertyChanged();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 MaxCustomImpedance => Constants.AWG_RES_MAX;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 MinCustomImpedance => Constants.AWG_RES_MIN;

        private Int32 ValidateCustomImpedance(Int32 value)
        {
            if (value > MaxCustomImpedance)
            {
                value = MaxCustomImpedance;
            }
            else if (value < MinCustomImpedance)
            {
                value = MinCustomImpedance;
            }
            return value;
        }

        private Int64 _Frequency = Constants.AWG_SIN_FRQ_DEF;

        public Int64 Frequency
        {
            get => _Frequency;
            set
            {
                value = ValidateFreq(value);
                if (_Frequency != value)
                {
                    _Frequency = value;
                    if (ModMethod == WfmModMethod.FM)
                    {
                        Int64 freqdiffer = Math.Abs(MaxFrequency - _Frequency);
                        Int64 maxfreqbias = freqdiffer < _Frequency ? freqdiffer : _Frequency;
                        FreqBias = FreqBias > maxfreqbias ? maxfreqbias : FreqBias;
                        FreqBias = FreqBias < MinFreqBias ? MinFreqBias : FreqBias;
                    }
                    if (value > Constants.AWG_AMP_FREQ_HIGH && Amplitude / 2 + Math.Abs(Offset) > MaxAmplitude / 2)
                    {
                        Amplitude = MaxAmplitude;
                    }
                    MaxPulseEdgeTime = GetMaxPulseEdge();
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
                SetMaxAndMinDuty();
            }
        }

        public Int64 MaxFrequency => WfmType switch
        {
            ArbWfmType.Sinusoid => Constants.AWG_SIN_FRQ_MAX,
            ArbWfmType.Square => Constants.AWG_SQUARE_FRQ_MAX,
            ArbWfmType.Pulse => Constants.AWG_PULSE_FRQ_MAX,
            ArbWfmType.Ramp => Constants.AWG_RAMP_FRQ_MAX,
            _ => _EnablePointByPoint ? Constants.AWG_ARB_SA_MAX : Constants.AWG_ARB_FRQ_MAX,
        };

        //<Remark>更改人：彭博 创建日期：2023/11/23 10:04:00 原值：Constants.AWG_SIN_FRQ_MIN  原因：与技术手册不一致，技术手册要求每种波形的值不一定相同 </Remark>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int64 MinFrequency => WfmType switch
        {
            ArbWfmType.Arbitrary => Constants.AWG_ARB_FRQ_MIN,
            _ => Constants.AWG_SIN_FRQ_MIN,
        };

        private Int64 ValidateFreq(Int64 value)
        {
            if (value < Constants.AWG_FRQ_CORNER1)
            {
                value = (value / Constants.AWG_FRQ_STP0) * Constants.AWG_FRQ_STP0;
            }
            else if (value < Constants.AWG_FRQ_CORNER2)
            {
                value = (value * 100 / Constants.AWG_FRQ_STP1) * (Constants.AWG_FRQ_STP1 / 100);
            }
            else
            {
                value = (value * 100 / Constants.AWG_FRQ_STP2) * (Constants.AWG_FRQ_STP2 / 100);
            }

            if (value > MaxFrequency)
            {
                value = MaxFrequency;
            }
            else if (value < MinFrequency)
            {
                value = MinFrequency;
            }
            if (value > Constants.AWG_AMP_FREQ_HIGH)
            {
                MaxAmplitude = Constants.AWG_AMP_1M_MAX_HIGH_FREQ;
            }
            else
            {
                MaxAmplitude = Constants.AWG_AMP_1M_MAX;
            }
            return value;
        }

        public Int64 GetFreqStep(Int32 stride)
        {
            Int64 delta;
            if (Frequency < Constants.AWG_FRQ_CORNER1)
            {
                delta = Constants.AWG_FRQ_STP0;
            }
            if (Frequency < Constants.AWG_FRQ_CORNER2)
            {
                delta = Constants.AWG_FRQ_STP1;
            }
            else
            {
                delta = Constants.AWG_FRQ_STP2;
            }

            return stride * delta;
        }

        public void AdjPeriod(Int32 stride)
        {
            Double delta;
            if (Frequency < Constants.AWG_FRQ_CORNER2)
            {
                delta = Constants.AWG_PERIOD_STP2;
            }
            else
            {
                delta = Constants.AWG_PERIOD_STP1;
            }

            Frequency = Convert.ToInt64(Frequency / (1 + stride * delta * 1E-9 * Frequency * 1E-6));
        }
        private Int32 SetVoltageBasedImp(Double v)
        {
            if (Impedance != WfmGenImpedance.HighZ)
            {
                return (Int32)Math.Round((Constants.AWG_RES_DEF + CustomImpedance) * v / CustomImpedance);
            }
            return (Int32)v;
        }

        /// <summary>
        /// 幅度单位类型
        /// </summary>
        private QuantityUnit _QuantityUnitByAmp = QuantityUnit.VoltagePeakPeak;
        public QuantityUnit QuantityUnitByAmp
        {
            get
            {
                return _QuantityUnitByAmp;
            }
            set
            {
                _QuantityUnitByAmp = value;
                OnPropertyChanged(nameof(Amplitude));
            }
        }


        private Double _Amplitude = Constants.AWG_AMP_1M_DEF;
        public Int32 Amplitude
        {
            get => (Int32)_Amplitude;
            set
            {
                value = ValidateAmp(value);
                if (_Amplitude != value)
                {
                    _Amplitude = value;
                    OnPropertyChanged();
                    Hardware.HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        /// <summary>
        /// 有效值Vrms
        /// </summary>
        private Double _AmplitudeVrms;
        public Double AmplitudeVrms
        {
            get { return _AmplitudeVrms; }
            set { _AmplitudeVrms = value; OnPropertyChanged(nameof(Amplitude)); }
        }

        /// <summary>
        /// 功率dBm
        /// </summary>
        private Double _AmplitudedBm;
        public Double AmplitudedBm
        {
            get { return _AmplitudedBm; }
            set { _AmplitudedBm = value; OnPropertyChanged(nameof(Amplitude)); }
        }

        private Int32 _MaxAmplitude = Constants.AWG_AMP_1M_MAX;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 MaxAmplitude
        {
            get
            {
                return _MaxAmplitude;
            }
            set
            {
                if (_MaxAmplitude != value)
                {
                    _MaxAmplitude = value;
                    if (Amplitude > _MaxAmplitude)
                    {
                        Amplitude = _MaxAmplitude;
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 MinAmplitude => Constants.AWG_AMP_1M_MIN;

        private Int32 ValidateAmp(Int32 value)
        {
            value = (value / Constants.AWG_AMP_STP) * Constants.AWG_AMP_STP;

            if (value > MaxAmplitude)
            {
                value = MaxAmplitude;
            }
            else if (value < MinAmplitude)
            {
                value = MinAmplitude;
            }

            return value;
        }

        public void AdjAmplitude(Int32 stride)
        {
            Amplitude += stride * Constants.AWG_AMP_STP;
        }

        public Int64 MaxPulseEdgeTime = Constants.AWG_Pulse_Edge_TIME_NS_MAX;
        public Int64 MinPulseEdgeTime = Constants.AWG_Pulse_Edge_TIME_NS_MIN;

        /// <summary>
        /// 获取上升时间或下降时间的最大值
        /// </summary>
        /// <returns>返回上升时间或下降时间最大值</returns>
        /// <Remark>作者：彭博 创建日期：2023/11/22 11:55:00 </Remark>
        /// <Remark>创建原因：添加上升时间和下降时间最大值随频率变化的功能 </Remark>
        internal Int64 GetMaxPulseEdge()
        {
            //获取最小上升下降时间
            Double minedge = Constants.AWG_Pulse_Edge_TIME_NS_MIN / 1e+9;
            //获取最大上升下降时间
            Double maxedge = Constants.AWG_Pulse_Edge_TIME_NS_MAX / 1e+9;
            Double narrowlevelwidth;
            //获取最小频率
            Double minfrequency = MinFrequency / 1e+9;
            //获取输入频率
            Double frequency = Frequency / 1e+9;
            //获取占空比
            Double duty = Duty / 100;
            Double minpulsewidth = (MinPulseWidth * 1.0) / 2 / 1e+7;
            //判断输入频率和最小频率的大小
            if (frequency < minfrequency)
            {
                frequency = minfrequency;
            }

            //获取系数
            Double period = 1 / frequency;

            //判断占空比大小
            if (duty < 50)
            {
                //获取上升时间或下降时间
                narrowlevelwidth = 0.8 * (duty * 1 * period / 100 - minpulsewidth);
            }
            else
            {
                //获取上升时间或下降时间
                narrowlevelwidth = 0.8 * ((100 - duty) * 1 * period / 100 - minpulsewidth);
            }

            //上升时间或下降时间
            Double tempmax = narrowlevelwidth;

            //判断是否比最小时间小
            if (tempmax < minedge)
            {
                tempmax = minedge;
            }
            //判断是否比最大时间大
            if (tempmax > maxedge)
            {
                tempmax = maxedge;
            }

            Int64 pulsemax = (Int64)(tempmax * 1e+9);

            //判断当前下降时间是否大于设定的最大时间
            if (PulseFallTime > pulsemax)
            {
                PulseRiseTime = pulsemax;
                //     Hd.AddAWGCmd(HdMsgFactory.MakeArbWfmGenOpt());
            }
            //判断当前上升时间是否大于设定的最大时间
            if (PulseRiseTime > pulsemax)
            {
                PulseRiseTime = pulsemax;
                //     Hd.AddAWGCmd(HdMsgFactory.MakeArbWfmGenOpt());
            }
            return pulsemax;
        }

        private Int64 _PulseRiseTime = Constants.AWG_Pulse_Edge_TIME_NS_MIN;
        public Int64 PulseRiseTime
        {
            get => _PulseRiseTime;
            set
            {
                if (_PulseRiseTime != value)
                {
                    _PulseRiseTime = value;
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        private Int64 _PulseFallTime = Constants.AWG_Pulse_Edge_TIME_NS_MIN;
        public Int64 PulseFallTime
        {
            get => _PulseFallTime;
            set
            {
                if (_PulseFallTime != value)
                {
                    _PulseFallTime = value;
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        private Double _Offset = Constants.AWG_OFS_1M_DEF;

        public Int32 Offset
        {
            get => (Int32)_Offset;
            set
            {
                value = ValidateOfs(value);
                if (_Offset != value)
                {
                    _Offset = value;
                    OnPropertyChanged();
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 MaxOffset => Constants.AWG_OFS_1M_MAX;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 MinOffset => Constants.AWG_OFS_1M_MIN;

        private Int32 ValidateOfs(Int32 value)
        {
            value = (value / Constants.AWG_OFS_STP) * Constants.AWG_OFS_STP;

            if (value > MaxOffset)
            {
                value = MaxOffset;
            }
            else if (value < MinOffset)
            {
                value = MinOffset;
            }

            return value;
        }

        public void AdjOffset(Int32 stride)
        {
            Offset += stride * Constants.AWG_OFS_STP;
        }

        public Int32 HighLevel
        {
            get
            {
                Int32 level = (Int32)(_Offset + _Amplitude / 2);
                if (level < MinOffset)
                {
                    level = MinOffset;
                }
                if (level > MaxOffset)
                {
                    level = MaxOffset;
                }
                return level;
            }
            set
            {
                Int32 setlevel = value;
                if (setlevel < MinOffset)
                {
                    setlevel = MinOffset;
                }
                if (setlevel > MaxOffset)
                {
                    setlevel = MaxOffset;
                }
                setlevel = SetVoltageBasedImp(setlevel);
                var v1 = _Offset + _Amplitude / 2;
                if (v1 != setlevel)
                {
                    var v0 = _Offset - _Amplitude / 2;
                    _Amplitude = setlevel - v0;
                    if (_Amplitude < 20)
                    {
                        _Amplitude = 20;
                        _Offset = (setlevel - 20 + v0) / 2;
                    }
                    else
                    {
                        _Offset = (setlevel + v0) / 2;
                    }
                    OnPropertyChanged(nameof(Offset));
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        public void AdjHighLevel(Int32 stride)
        {
            var v1 = Offset + Amplitude / 2;
            var v0 = Offset - Amplitude / 2;
            v1 += stride * Constants.AWG_AMP_STP;

            Offset = (v1 + v0) / 2;
            Amplitude = v1 - v0;
        }

        public Int32 LowLevel
        {
            get
            {
                Int32 level = (Int32)(_Offset - _Amplitude / 2);
                if (level < MinOffset)
                {
                    level = MinOffset;
                }
                if (level > MaxOffset)
                {
                    level = MaxOffset;
                }
                return level;
            }
            set
            {
                Int32 setlevel = value;
                if (setlevel < MinOffset)
                {
                    setlevel = MinOffset;
                }
                if (setlevel > MaxOffset)
                {
                    setlevel = MaxOffset;
                }
                setlevel = SetVoltageBasedImp(setlevel);
                var v0 = _Offset - _Amplitude / 2;
                if (v0 != setlevel)
                {
                    var v1 = _Offset + _Amplitude / 2;
                    _Amplitude = v1 - setlevel;
                    if (_Amplitude < 20)
                    {
                        _Amplitude = 20;
                        _Offset = (v1 + setlevel - 20) / 2;
                    }
                    else
                    {
                        _Offset = (v1 + setlevel) / 2;
                    }
                    OnPropertyChanged(nameof(Offset));
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        public void AdjLowLevel(Int32 stride)
        {
            var v1 = Offset + Amplitude / 2;
            var v0 = Offset - Amplitude / 2;
            v0 += stride * Constants.AWG_AMP_STP;

            Offset = (v1 + v0) / 2;
            Amplitude = v1 - v0;
        }

        private Int32 _Duty = Constants.AWG_DUTY_DEF;

        /// <summary>
        ///  实际下发到硬件的占空比
        /// </summary>
        public Int32 RealDuty { get; set; }

        public Int32 Duty
        {
            get => _Duty;
            set
            {
                value = ValidateDuty(value);
                if (_Duty != value)
                {
                    _Duty = value;
                    RealDuty = ValidateRealDuty(value);
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 MaxDuty { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 MinDuty { get; set; }

        #region 占空比最大值最小值获取

        /// <summary>
        /// 设置占空比的最大值和最小值
        /// </summary>
        public void SetMaxAndMinDuty()
        {
            if (WfmType == ArbWfmType.Pulse || WfmType == ArbWfmType.Square)
            {
                MaxDuty = GetMaxDuty();
                MinDuty = GetMinDuty();
                Duty = ValidateDuty(Duty);
                RealDuty = ValidateRealDuty(Duty);
            }
            else
            {
                MaxDuty = Constants.AWG_DUTY_MAX;
                MinDuty = Constants.AWG_DUTY_MIN;
                Duty = ValidateDuty(Duty);
                RealDuty = ValidateRealDuty(Duty);
            }
        }

        /// <summary>
        /// 获取占空比的最大值
        /// </summary>
        /// <returns>返回占空比最大值</returns>
        /// <Remark>作者：彭博 创建日期：2023/11/27 15:26:00 </Remark>
        /// <Remark>创建原因：添加占空比最大值随频率变化的功能 </Remark>
        private Int32 GetMaxDuty()
        {
            //获取最大占空比
            Double pulsetime = (Math.Abs(_PulseFallTime) + Math.Abs(_PulseRiseTime)) * 1e-3 * 0.8;
            Double cycle = 1.0 * 1e15 / Frequency;
            Int32 duty = Math.Abs((Int32)((cycle - pulsetime) * 10000 / cycle));
            if (duty <= 5000)
            {
                duty = 10000 - duty;
            }
            if (duty >= 10000)
            {
                duty = Constants.AWG_DUTY_MAX;
            }

            #region 信号源获取最大占空比

            //Int64 frequency = Frequency;
            //if (frequency > 1_000_000_000 && frequency < 25_000_000_000_000)
            //{
            //    duty = (Int32)((10000 - (frequency / 1_000_000_000 - 1) * 0.4));
            //    if (duty <= 5000)
            //    {
            //        duty = 10000 - duty;
            //    }
            //}

            #endregion

            return duty;
        }

        /// <summary>
        /// 获取占空比的最小值
        /// </summary>
        /// <returns>返回占空比最小值</returns>
        /// <Remark>作者：彭博 创建日期：2023/11/27 15:26:00 </Remark>
        /// <Remark>创建原因：添加占空比最小值随频率变化的功能 </Remark>
        private Int32 GetMinDuty()
        {
            //获取最小占空比
            Double pulsetime = (Math.Abs(_PulseFallTime) + Math.Abs(_PulseRiseTime)) * 1e-3 * 0.8;
            Double cycle = 1.0 * 1e15 / Frequency;
            Int32 duty = Math.Abs((Int32)((pulsetime) * 10000 / cycle));
            if (duty >= 5000)
            {
                duty = 10000 - duty;
            }
            if (duty <= 0)
            {
                duty = Constants.AWG_DUTY_MIN;
            }

            #region 信号源获取最小占空比

            //Int64 frequency = Frequency;
            //if (frequency >= 1_000_000_000 && frequency < 25_000_000_000_000)
            //{
            //    duty = (Int32)((frequency / 1_000_000_000) * 0.4);
            //    if (duty >= 5000)
            //    {
            //        duty = 10000 - duty;
            //    }
            //}

            #endregion

            return duty;
        }

        #endregion

        /// <summary>
        /// 获取有效下发的占空比
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ///  <Remark>作者：彭博 创建日期：2024/3/21 16:26:00 </Remark>
        private Int32 ValidateRealDuty(Int32 value)
        {
            var dutystep = PulseWidthToDuty(Constants.AWG_PULSE_WIDTH_STP, PeriodFromFreq(Frequency));
            if (dutystep < Constants.AWG_DUTY_STP)
            {
                dutystep = Constants.AWG_DUTY_STP;
            }

            value = (value / dutystep) * dutystep;

            if (value > MaxDuty)
            {
                value = MaxDuty;
            }
            else if (value < MinDuty)
            {
                value = MinDuty;
            }

            return value;
        }

        private Int32 ValidateDuty(Int32 value)
        {
            if (value > MaxDuty)
            {
                value = MaxDuty;
            }
            else if (value < MinDuty)
            {
                value = MinDuty;
            }

            return value;
        }

        public void AdjPulseRiseTime(Int32 stride)
        {
            PulseRiseTime += stride * Constants.AWG_Pulse_Edge_STP;
        }

        public void AdjPulseFallTime(Int32 stride)
        {
            PulseFallTime += stride * Constants.AWG_Pulse_Edge_STP;
        }

        public void AdjDuty(Int32 stride)
        {
            var dutystep = PulseWidthToDuty(Constants.AWG_PULSE_WIDTH_STP, PeriodFromFreq(Frequency));
            if (dutystep < Constants.AWG_DUTY_STP)
            {
                dutystep = Constants.AWG_DUTY_STP;
            }

            Duty += stride * dutystep;
        }

        private Boolean _Opposition;
        public Boolean Opposition
        {
            get => _Opposition;
            set
            {
                _Opposition = value;
                OnPropertyChanged();
            }
        }

        private Int32 _Phase = Constants.AWG_PHASE_DEF;
        public Int32 Phase
        {
            get => Opposition ? (_Phase + HalfPhase) % (Constants.AWG_PHASE_HALF * 2) : _Phase;
            set
            {
                value = ValidatePhase(value);
                if (_Phase != value)
                {
                    _Phase = value;
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 HalfPhase => Constants.AWG_PHASE_HALF;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 MaxPhase => Constants.AWG_PHASE_MAX;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 MinPhase => Constants.AWG_PHASE_MIN;

        private Int32 ValidatePhase(Int32 value)
        {
            value = (value / Constants.AWG_PHASE_STP) * Constants.AWG_PHASE_STP;

            if (value > MaxPhase)
            {
                value = value % MaxPhase;
            }
            else if (value < MinPhase)
            {
                value = MinPhase;
            }

            return value;
        }

        public void AdjPhase(Int32 stride)
        {
            Phase += stride * Constants.AWG_PHASE_STP;
        }

        private Int32 _Noise = Constants.AWG_NOISE_DEF;
        public Int32 Noise
        {
            get => _Noise;
            set
            {
                value = ValidateNoise(value);
                if (_Noise != value)
                {
                    _Noise = value;
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 MaxNoise => Constants.AWG_NOISE_MAX;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 MinNoise => Constants.AWG_NOISE_MIN;

        private Int32 ValidateNoise(Int32 value)
        {
            value = (value / Constants.AWG_NOISE_STP) * Constants.AWG_NOISE_STP;

            if (value > MaxNoise)
            {
                value = MaxNoise;
            }
            else if (value < MinNoise)
            {
                value = MinNoise;
            }

            return value;
        }

        public void AdjNoise(Int32 stride)
        {
            Noise += stride * Constants.AWG_NOISE_STP;
        }


        private WfmModMethod _ModMethod = WfmModMethod.AM;

        public WfmModMethod ModMethod
        {
            get => _ModMethod;
            set
            {
                if (_ModMethod != value)
                {
                    _ModMethod = value;
                    if (ModMethod == WfmModMethod.FM)
                    {
                        Int64 freqdiffer = Math.Abs(MaxFrequency - _Frequency);
                        Int64 maxfreqbias = freqdiffer < _Frequency ? freqdiffer : _Frequency;
                        FreqBias = FreqBias > maxfreqbias ? maxfreqbias : FreqBias;
                        FreqBias = FreqBias < MinFreqBias ? MinFreqBias : FreqBias;
                    }
                    OnPropertyChanged(nameof(ModMethod));
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        private ArbWfmType _ModulatedWfm = ArbWfmType.Sinusoid;

        public ArbWfmType ModulatedWfm
        {
            get => _ModulatedWfm;
            set
            {
                if (_ModulatedWfm != value && ModulatedSignalList.Exists(o => o == value))
                {
                    _ModulatedWfm = value;
                    _ModFreq = ValidateFreq(_ModFreq);
                    String filename = "";
                    switch (value)
                    {
                        case ArbWfmType.Noise:
                            filename = "Noisewhite";
                            break;

                        default:
                            break;
                    }
                    if (!String.IsNullOrWhiteSpace(filename))
                    {
                        var filepath = Environment.CurrentDirectory + @$"\Resources\BaseWaves\{filename}.bsv";
                        if (File.Exists(filepath))
                        {
                            _ModFilePath = filepath;
                            ReadWaveFileData_Bsv(true);
                        }
                    }
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        private WfmRampType _RampType = WfmRampType.Rise;
        public WfmRampType RampType
        {
            get => _RampType;
            set
            {
                if (_RampType != value)
                {
                    _RampType = value;
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        public readonly ImmutableList<ArbWfmType> ModulatedSignalList = new List<ArbWfmType>()
        {
            ArbWfmType.Sinusoid,
            ArbWfmType.Square,
            ArbWfmType.Ramp,
            ArbWfmType.Noise,
            ArbWfmType.Arbitrary,
        }.ToImmutableList();

        private Int64 _SweepDuration = Constants.AWG_SWEEP_TIME_MIN;

        public Int64 SweepDuration
        {
            get => _SweepDuration;
            set
            {
                value = ValidateSweepDuration(value);
                if (_SweepDuration != value)
                {
                    _SweepDuration = value;
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int64 MaxSweepDuration => Constants.AWG_SWEEP_TIME_MAX;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int64 MinSweepDuration => Constants.AWG_SWEEP_TIME_MIN;

        private Int64 ValidateSweepDuration(Int64 value)
        {
            if (value > MaxSweepDuration)
            {
                value = MaxSweepDuration;
            }
            else if (value < MinSweepDuration)
            {
                value = MinSweepDuration;
            }
            return value;
        }

        public void AdjSweepDuration(Int32 stride)
        {
            SweepDuration += stride * Constants.AWG_SWEEP_TIME_STP;
        }


        private SweepType _SweepType = SweepType.Linear;

        public SweepType SweepType
        {
            get => _SweepType;
            set
            {
                if (_SweepType != value)
                {
                    _SweepType = value;
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        private Int64 _SweepStartFreq = Constants.AWG_SWEEP_FRQ_MIN;

        public Int64 SweepStartFreq
        {
            get => _SweepStartFreq;
            set
            {
                value = ValidateSweepFreq(value);
                if (_SweepStartFreq != value)
                {
                    _SweepStartFreq = value;
                    if (value > Constants.AWG_AMP_FREQ_HIGH && Amplitude / 2 + Math.Abs(Offset) > MaxAmplitude / 2)
                    {
                        Amplitude = MaxAmplitude;
                    }
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        private Int64 _SweepEndFreq = Constants.AWG_SWEEP_FRQ_MAX;
        public Int64 SweepEndFreq
        {
            get => _SweepEndFreq;
            set
            {
                value = ValidateSweepFreq(value);
                if (_SweepEndFreq != value)
                {
                    _SweepEndFreq = value;
                    if (value > Constants.AWG_AMP_FREQ_HIGH && Amplitude / 2 + Math.Abs(Offset) > MaxAmplitude / 2)
                    {
                        Amplitude = MaxAmplitude;
                    }
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int64 MaxSweepFreq => WfmType switch
        {
            ArbWfmType.Sinusoid => Constants.AWG_SIN_FRQ_MAX,
            ArbWfmType.Square => Constants.AWG_SQUARE_FRQ_MAX,
            ArbWfmType.Pulse => Constants.AWG_PULSE_FRQ_MAX,
            ArbWfmType.Ramp => Constants.AWG_RAMP_FRQ_MAX,
            _ => _EnablePointByPoint ? Constants.AWG_ARB_SA_MAX : Constants.AWG_ARB_FRQ_MAX,
        };



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int64 MinSweepFreq => Constants.AWG_SWEEP_FRQ_MIN;

        private Int64 ValidateSweepFreq(Int64 value)
        {
            if (value > MaxSweepFreq)
            {
                value = MaxSweepFreq;
            }
            else if (value < MinSweepFreq)
            {
                value = MinSweepFreq;
            }
            if (value > Constants.AWG_AMP_FREQ_HIGH)
            {
                MaxAmplitude = Constants.AWG_AMP_1M_MAX_HIGH_FREQ;
            }
            else
            {
                MaxAmplitude = Constants.AWG_AMP_1M_MAX;
            }
            return value;
        }


        private Int64 _ModFreq = Constants.AWG_MODEM_FRQ_MIN;
        public Int64 ModFreq
        {
            get => _ModFreq;
            set
            {
                value = ValidateModFreq(value);
                if (_ModFreq != value)
                {
                    _ModFreq = value;
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int64 MaxModFreq => Constants.AWG_MODEM_FRQ_MAX;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int64 MinModFreq => Constants.AWG_MODEM_FRQ_MIN;

        private Int64 ValidateModFreq(Int64 value)
        {
            if (value < Constants.AWG_FRQ_CORNER1)
            {
                value = (value / Constants.AWG_FRQ_STP0) * Constants.AWG_FRQ_STP0;
            }
            else if (value < Constants.AWG_FRQ_CORNER2)
            {
                value = (value / Constants.AWG_FRQ_STP1) * Constants.AWG_FRQ_STP1;
            }
            else
            {
                value = (value / Constants.AWG_FRQ_STP2) * Constants.AWG_FRQ_STP2;
            }

            if (value > MaxModFreq)
            {
                value = MaxModFreq;
            }
            else if (value < MinModFreq)
            {
                value = MinModFreq;
            }

            return value;
        }

        private Int32 _AmpDepth = Constants.AWG_AM_DEPTH_DEF;
        public Int32 AmpDepth
        {
            get => _AmpDepth;
            set
            {
                value = ValidateAmpDepth(value);
                if (_AmpDepth != value)
                {
                    _AmpDepth = value;
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 MaxAmpDepth => Constants.AWG_AM_DEPTH_MAX;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int32 MinAmpDepth => Constants.AWG_AM_DEPTH_MIN;

        private Int32 ValidateAmpDepth(Int32 value)
        {
            value = (value / Constants.AWG_AMP_STP) * Constants.AWG_AMP_STP;

            if (value > MaxAmpDepth)
            {
                value = MaxAmpDepth;
            }
            else if (value < MinAmpDepth)
            {
                value = MinAmpDepth;
            }

            return value;
        }

        public void AdjAmpDepth(Int32 stride)
        {
            AmpDepth += stride * Constants.AWG_AMP_STP;
        }

        private Int64 _FreqBias = Constants.AWG_FM_BIAS_DEF;
        public Int64 FreqBias
        {
            get => _FreqBias;
            set
            {
                value = ValidateFreqBias(value);
                if (_FreqBias != value)
                {
                    _FreqBias = value;
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int64 MaxFreqBias => Constants.AWG_FM_BIAS_MAX;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int64 MinFreqBias => Constants.AWG_FM_BIAS_MIN;

        private Int64 ValidateFreqBias(Int64 value)
        {
            if (value < Constants.AWG_FRQ_CORNER1)
            {
                value = (value / Constants.AWG_FRQ_STP0) * Constants.AWG_FRQ_STP0;
            }
            else if (value < Constants.AWG_FRQ_CORNER2)
            {
                value = (value * 100 / Constants.AWG_FRQ_STP1) * Constants.AWG_FRQ_STP1 / 100;
            }
            else
            {
                value = (value * 100 / Constants.AWG_FRQ_STP2) * Constants.AWG_FRQ_STP2 / 100;
            }

            // <Remark>作者：彭博 创建日期：2024/1/29 10:26:00 创建原因：频偏不能大于当前设置频率 </Remark>
            if (value > _Frequency)
            {
                value = _Frequency;
            }

            if (value > MaxFreqBias)
            {
                value = MaxFreqBias;
            }
            else if (value < MinFreqBias)
            {
                value = MinFreqBias;
            }



            return value;
        }
        public Boolean TirgerOutEnabel
        {
            get => _TirgerOutEnabel;
            set
            {

                if (_TirgerOutEnabel != value)
                {
                    _TirgerOutEnabel = value;
                    if (_TirgerOutEnabel)
                    {
                        PassiveTriggerOut(Id);
                    }
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        //互斥被动设置触发输出
        public void PassiveTriggerOut(ChannelId sourceId)
        {
            var awgprsnts = AdcInterleaveProcessor.Default.Oscilloscope.ArbWfmGens.ToList();
            for (Int32 i = 0; i < awgprsnts.LongCount(); i++)
            {
                if (awgprsnts[i].Id != sourceId)
                {
                    awgprsnts[i].TirgerOutEnabel = false;
                }
            }
        }

        private Boolean _TirgerOutEnabel;
        private Int32 _PhaseBias = Constants.AWG_PHASE_DEF;
        public Int32 PhaseBias
        {
            get => Opposition ? _PhaseBias % (Constants.AWG_PHASE_HALF * 2) : _PhaseBias;
            set
            {
                value = ValidatePhase(value);

                if (_PhaseBias != value)
                {
                    _PhaseBias = value;
                    OnPropertyChanged();
                    HdCmdFactory.Push(HdCmd.AWGConfig);
                }
            }
        }

        public void AdjPhaseBias(Int32 stride)
        {
            PhaseBias += stride * Constants.AWG_PHASE_STP;
        }

        public static Int64 PulseWidthFromDuty(Int32 duty, Int64 periodByns)
        {
            return periodByns * duty / 100;
        }

        public static Int32 PulseWidthToDuty(Int64 pwByns, Int64 periodByns)
        {
            return Convert.ToInt32(pwByns * 100.0 / periodByns);
        }

        public static Int64 PeriodFromFreq(Int64 freqByuHz)
        {
            return Convert.ToInt64(1E9 / (freqByuHz * 1E-6));
        }

        public static Int64 PeriodToFreq(Int64 periodByns)
        {
            return Convert.ToInt64(1E6 / (periodByns * 1E-9));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public Int64 MinPulseWidth => Constants.AWG_PULSE_WIDTH_MIN;

        public IList<Double>? ArbBuffer
        {
            get;
            set;
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

        public ArbWfmGenModel(ChannelId id)
        {
            Id = id;
            Name = id.ToString();
        }
        #region 波形文件解析
        public static Boolean Normalization(ref short[] data)
        {
            if (data.Length == 0)
            {
                return false;
            }
            var datalst = data.ToList();
            var max = datalst.Max();
            var min = datalst.Min();
            for (Int32 i = 0; i < data.Length; i++)
            {
                data[i] = (short)(((data[i] - min) / (max - min) - 0.5) * 2);
            }
            return true;
        }
        public static Boolean Normalization(ref Double[] data)
        {
            if (data.Length == 0)
            {
                return false;
            }
            var datalst = data.ToList();
            var max = datalst.Max();
            var min = datalst.Min();
            for (Int32 i = 0; i < data.Length; i++)
            {
                data[i] = ((data[i] - min) / (max - min) - 0.5) * 2;
            }
            return true;
        }
        private void ReadWaveFileData_Bsv(Boolean isModData = false)
        {
            List<Int32>? tmpdata = new();
            String _filepath = FilePath;
            if (isModData)
            {
                _filepath = ModFilePath;
            }
            try
            {
                const Int32 dataBytes = 2;
                using MemoryStream memorystream = new(File.ReadAllBytes(_filepath));
                Int32 index = 0;

                List<Byte> contentdatas = new();
                Int32 baseheadlength = 150;
                if (baseheadlength > memorystream.Length)
                {
                    baseheadlength = (Int32)memorystream.Length;
                }
                Int32 datalength, datavpp, dataoffset, datapoints;
                Byte[] tmpByte = new Byte[baseheadlength];

                #region 获取头
                memorystream.Read(tmpByte, 0, baseheadlength);
                String headstr = Encoding.ASCII.GetString(tmpByte.ToArray());
                Regex headregex = new(@"(?<=\[HEAD\]:)\d+");
                Regex dataregex = new(@"(?<=\[DATA\]:)\d+");
                var matchcollection1 = headregex.Match(headstr);
                var matchcollection2 = dataregex.Match(headstr);
                if (matchcollection1.Success && matchcollection2.Success)
                {
                    datalength = Int32.Parse(matchcollection1.Value);
                    datapoints = Int32.Parse(matchcollection2.Value);
                    //headStr = Regex.Match(headStr, @"\[HEAD\]:\d+\r\n\[DATA\]:\d+\r\n").Value;
                    var tmpheadstr = $"[DATA]:{datapoints}";
                    index = headstr.IndexOf(tmpheadstr) + tmpheadstr.Length + 2;
                }
                //if (Regex.IsMatch(headStr, @"\[HEAD\]:\d+(?:(.+?))\[DATA\]:\d+"))
                //{
                //    var matchCollection = Regex.Matches(headStr, @"\d+");
                //    dataLength = Int32.Parse(matchCollection[0].Value);
                //    dataVpp = Int32.Parse(matchCollection[1].Value);
                //    dataOffset = Int32.Parse(matchCollection[2].Value);
                //    dataPoints = Int32.Parse(matchCollection[3].Value);
                //    headStr = Regex.Match(headStr, @"\[HEAD\]:\d+\r\nVPP:\d+\r\nOFFSET:\d+\r\n\[DATA\]:\d+\n").Value;
                //    index = headStr.Length;
                //}
                //else if (Regex.IsMatch(headStr, @"\[HEAD\]:\d+\r\n\[DATA\]:\d+\r\n"))
                //{
                //    var matchCollection = Regex.Matches(headStr, @"\d+");
                //    dataLength = Int32.Parse(matchCollection[0].Value);
                //    dataPoints = Int32.Parse(matchCollection[1].Value);
                //    headStr = Regex.Match(headStr, @"\[HEAD\]:\d+\r\n\[DATA\]:\d+\r\n").Value;
                //    index = headStr.Length;
                //}
                else
                {
                    return;
                }
                #endregion 获取头
                #region 获取数据
                memorystream.Position = index;
                for (; index < memorystream.Length; index++)
                {
                    var data = memorystream.ReadByte();
                    if (data == -1)
                    {
                        break;
                    }

                    contentdatas.Add((Byte)(data & 0xff));
                }
                var datacount = contentdatas.Count;
                if (datacount > 1 && datacount % 2 != 0)
                {
                    datacount -= 1;

                }
                datapoints = datacount / dataBytes;
                //if (dataCount != dataBytes * dataPoints)
                //{
                //    if (dataCount < dataBytes * dataPoints)
                //        return;
                //}
                Int16[] dataValues = new Int16[datapoints];
                for (Int32 i = 0; i < datapoints; i++)
                {
                    dataValues[i] = (Int16)(((Int16)contentdatas[2 * i]) + ((short)contentdatas[2 * i + 1] * 256));
                }
                #endregion 获取数据
                //Normalization(ref dataValues);
                tmpdata = new List<Int32>();
                if (dataValues.Length < 8192)//判断数据点数，是否小于8192，若小于8912个点，周期性的填写满8192个点
                {
                    Int32 count = 8192 / dataValues.Length;
                    for (Int32 i = 0; i < count; i++)
                    {
                        tmpdata.AddRange(dataValues.Select(p => (Int32)p));
                    }
                }
                else
                {
                    tmpdata.AddRange(dataValues.Select(p => (Int32)p));
                }

            }
            catch (Exception e)
            {
                tmpdata = null;
            }
            if (isModData)
            {
                ModArbWfmData = tmpdata;
            }
            else
            {
                ArbWfmData = tmpdata;
            }
        }
        private void ReadWaveFileData_Bin(Boolean isModData = false)
        {
            List<Int32>? tmpdata = new();
            String _filepath = FilePath;
            if (isModData)
            {
                _filepath = ModFilePath;
            }
            try
            {
                using MemoryStream memorystream = new(File.ReadAllBytes(_filepath));
                var pkg = BinaryConvert.Deserialize<WfmPack>(memorystream);
                if (pkg == null || pkg.Length == 0)
                {
                    return;
                }
                tmpdata = new();
                Double[] datas = pkg.Buffer.Cast<Double>().ToArray();
                Normalization(ref datas);
                foreach (var data in datas)
                {
                    tmpdata.Add((Int32)(data * 32767.0));
                }
            }
            catch (Exception e)
            {
                tmpdata = null;
            }
            if (isModData)
            {
                ModArbWfmData = tmpdata;
            }
            else
            {
                ArbWfmData = tmpdata;
            }
        }
        #endregion  波形文件解析
        public void GetArbWfmData(Boolean isModData = false)
        {
            var fileformat = isModData ? _ModWfmFileFormat : _WfmFileFormat;
            if (fileformat == null)
            {
                fileformat = WfmFormat.Binary;
            }
            switch (fileformat)
            {
                case WfmFormat.Binary:
                    ReadWaveFileData_Bin(isModData);
                    break;
                case WfmFormat.BSV:
                    ReadWaveFileData_Bsv(isModData);
                    break;
                default:
                    break;
            }
        }

        // <Remark>作者：彭博 创建日期：2023/12/1 15:26:00 创建原因：是否发送波形数据 </Remark>
        private Boolean _IsSendWaveType = false;
        public Boolean IsSendWaveType
        {
            get => _IsSendWaveType;
            set
            {
                _IsSendWaveType = value;
            }
        }
    }
}

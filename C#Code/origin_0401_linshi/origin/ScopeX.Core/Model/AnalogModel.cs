// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/15</date>

namespace ScopeX.Core
{
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;
    using ScopeX.Hardware.Driver;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Threading.Channels;


    public enum AnaChnlScaleIndex
    {
        Lv500u,
        Lv1m,
        Lv2m,
        Lv5m,
        Lv10m,
        Lv20m,
        Lv50m,
        Lv100m,
        Lv200m,
        Lv500m,
        Lv1,
        Lv2,
        Lv5,
        Lv10,
        Lv20,
    }

    public enum ProbeType
    {
        Null,
        Singled,
        Diff
    }

    internal class AnalogModel : ChannelModel
    {
        public AnalogModel(ChannelId id, Color color, Boolean active, TimebaseModel tmb)
            : base(ChannelType.Analog, id, color)
        {
            Active = active;

            Conditioning = new ConditioningModel(this);
            Sampling = tmb;

            Persistent = new PersistentModel<Double>(Constants.MAX_PERSISTENT_CNT);

            _Deskew = new(3, () => OnPropertyChanged(nameof(Deskew)))
            {
                Max = Constants.MAX_DESKEW_FS,
                Min = Constants.MIN_DESKEW_FS,
                Stp = Constants.STP_DESKEW_FS
            };

            if (Constants.INPUT_SOURCE_SELECTABILITY)
            {
                Conditioning.FlagInfo = new AnaChnlIpnutSource();
            }
            Conditioning.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case nameof(Conditioning) + nameof(ConditioningModel.Position):
                        var trigger = DsoModel.Default?.GetTrigger();
                        if (trigger is TriggerSingleSrcModel edge && edge.Source == Id)
                        {
                            //edge.SetPosition(Id, edge.GetPosition(Id));
                            Double CompPosIndex = edge.GetPosIndex(Id);
                            var chmodel = DsoModel.Default.AnalogChnls.First(x => x.Id == edge.Source);
                            if (CompPosIndex > edge.MaxPosIndex)
                            {
                                // CompPosIndex = edge.MaxPosIndex - chmodel.aaaaaaaConditioning.PosIndex;
                                CompPosIndex = edge.MaxPosIndex;
                            }
                            else if (CompPosIndex < edge.MinPosIndex)
                            {
                                // CompPosIndex = edge.MinPosIndex - chmodel.Conditioning.PosIndex;
                                CompPosIndex = edge.MinPosIndex;
                            }
                            //else
                            //{
                            //    //CompPosIndex = edge.ScreenPosIndex;
                            //    edge.SetPosition(Id, edge.GetPosition(Id));
                            //    break;
                            //}
                            edge.SetPosIndex(Id, CompPosIndex);
                        }
                        else if (trigger is TriggerMultiLevelModel mlm)
                        {
                            // 双电平

                            if (mlm is TriggerDelayModel tdm)
                            {
                                // 延迟触发
                                if (tdm.SourceOne == Id)
                                {
                                    SetMultiLeavelUpperPos(mlm);
                                }
                                else if (tdm.SourceTwo == id)
                                {
                                    if (tdm.DataCompPosIndex > tdm.MaxDataCompPositionIndex)
                                    {
                                        tdm.DataCompPosIndex = tdm.MaxDataCompPositionIndex;
                                    }
                                    else if (tdm.DataCompPosIndex < tdm.MinDataCompPositionIndex)
                                    {
                                        tdm.DataCompPosIndex = tdm.MinDataCompPositionIndex;
                                    }
                                }
                            }
                            else if (mlm is TriggerSustainTimeModel tstm)
                            {
                                // 持续时间
                                tstm.Bits.Source = Id;
                                var pos = tstm.Bits.GetPosIndex(Id);
                                if (pos > tstm.Bits.MaxPosIndex)
                                {
                                    tstm.Bits.SetPosIndex(Id, tstm.Bits.MaxPosIndex);
                                }
                                else if (pos < tstm.Bits.MinPosIndex)
                                {
                                    tstm.Bits.SetPosIndex(Id, tstm.Bits.MinPosIndex);
                                }
                            }
                            else if (mlm.Source == Id)
                            {
                                SetMultiLevalPos(mlm);
                            }
                        }
                        else if (trigger is TriggerSetupHoldModel tshm)
                        {
                            // 建立保持
                            if (tshm.ClkSource == Id)
                            {
                                if (tshm.ClkRelPosIndex > tshm.MaxPosIndex)
                                {
                                    tshm.ClkRelPosIndex = tshm.MaxPosIndex;
                                }
                                else if (tshm.ClkRelPosIndex < tshm.MinPosIndex)
                                {
                                    tshm.ClkRelPosIndex = tshm.MinPosIndex;
                                }
                            }
                            else if (tshm.DataSource == Id)
                            {
                                if (tshm.UpperDataPosIndex > tshm.MaxPosIndex)
                                {
                                    tshm.UpperDataPosIndex = tshm.MaxPosIndex;
                                }
                                else if (tshm.UpperDataPosIndex < tshm.MinPosIndex)
                                {
                                    tshm.UpperDataPosIndex = tshm.MinPosIndex;
                                }
                            }
                        }
                        else if (trigger is TriggerPatternModel tpm)
                        {
                            tpm.Bits.Source = Id;
                            tpm.Bits.GetPosIndex(Id);
                            var pos = tpm.Bits.GetPosIndex(Id);
                            if (pos > tpm.Bits.MaxPosIndex)
                            {
                                tpm.Bits.SetPosIndex(Id, tpm.Bits.MaxPosIndex);
                            }
                            else if (pos < tpm.Bits.MinPosIndex)
                            {
                                tpm.Bits.SetPosIndex(Id, tpm.Bits.MinPosIndex);
                            }
                        }
                        break;
                }
            };
        }

        /// <summary>
        /// 仅设置下限电平
        /// </summary>
        /// <param name="mlm"></param>
        private void SetMultiLeavelLowerPos(TriggerMultiLevelModel mlm)
        {
            if (mlm.PosLowerIndex > mlm.MaxPosIndex)
            {
                mlm.PosLowerIndex = mlm.MaxPosIndex;
            }
            else if (mlm.PosLowerIndex < mlm.MinPosIndex)
            {
                mlm.PosLowerIndex = mlm.MinPosIndex;
            }
        }

        /// <summary>
        /// 仅设置上限电平
        /// </summary>
        /// <param name="mlm"></param>
        private void SetMultiLeavelUpperPos(TriggerMultiLevelModel mlm)
        {
            if (mlm.PosUpperIndex > mlm.MaxPosIndex)
            {
                mlm.PosUpperIndex = mlm.MaxPosIndex;
            }
            else if (mlm.PosUpperIndex < mlm.MinPosIndex)
            {
                mlm.PosUpperIndex = mlm.MinPosIndex;
            }
        }

        /// <summary>
        /// 同时设置多电平的上限和下限
        /// </summary>
        /// <param name="mlm"></param>
        private void SetMultiLevalPos(TriggerMultiLevelModel mlm)
        {
            SetMultiLeavelLowerPos(mlm);
            SetMultiLeavelUpperPos(mlm);
        }

        public override ConditioningModel Conditioning { get; }

        public override TimebaseModel Sampling { get; }

        public override Boolean Active
        {
            get => base.Active;
            set
            {
                if (base.Active != value)
                {
                    base.Active = value;
                    Hardware.HdCmdFactory.Push(HdCmd.ChnlActive);
                    //Hardware.HdCmdFactory.Push(HdCmd.OuterPannelLEDCtrl);
                }
            }
        }

        public PlotRenderType RenderType
        {
            get;
            set;
        } = PlotRenderType.None;

        public Byte[]? WfmDpx { get; set; }
        public UInt32 MainWinMaxHitTimes { get; set; }
        public UInt32 MainWinMinHitTimes { get; set; }
        public Int32 DpxChOnCount { get; set; }
        public Int32 DpxChIndex1 { get; set; }
        public Int32 DpxChIndex2 { get; set; }
        public Double DpxCorrection { get; set; }
        public IEnumerable<Double>? LastBuffer = Enumerable.Empty<Double>();

        public override void ClearBuffer()
        {
            base.ClearBuffer();
            LastBuffer = Enumerable.Empty<Double>();
            //VuDatabase.Reset();
        }

        public Int32 ZIndex { get; set; }
        public void SetPackFromSegment(WfmPack wfmPack)
        {
            Pack = wfmPack;
        }

        public PersistentModel<Double>? Persistent
        {
            get;
            set;
        }

        internal sealed class ConditioningModel : VertAxisModel
        {
            public ConditioningModel(AnalogModel model) : base("Conditioning")
            {
                _Model = model;
                InitialScale = (0, 0.5);
                GetScaleValue = GetAnalogScaleFromIndexTick;
                GetScaleIndex = GetIndexTickFromAnalogScale;

                ScaleHighZMaxIndex = AnaChnlScaleIndex.Lv5;
                Scale50OhmMaxIndex = AnaChnlScaleIndex.Lv1;

                Coupling = AnaChnlCoupling.DC1M;
                ScaleMaxIndex = ScaleHighZMaxIndex;
                ScaleMinIndex = AnaChnlScaleIndex.Lv2m;
                Prefix = Prefix.Milli;
                Unit = "V";

                ScaleIndex = AnaChnlScaleIndex.Lv5;
                //PosIndex = Constants.DEF_YPOS_IDX;
                //PosPrefix = Prefix.Milli;
                //PosUnit = "div";

                _LimitedBias = new("Bias")
                {
                    GetValue = GetBiasValueFromIndex,
                    GetIndex = GetBiasIndexFromValue,
                    Max = Constants.MAX_BIAS_UV,
                    Min = Constants.MIN_BIAS_UV,
                    Current = 0,
                    Prefix = Prefix.Micro,
                    Unit = "V"
                };

                _LimitedDelay = new(nameof(Delay))
                {
                    Min = Constants.MIN_CHANNEL_DELAY, // 800 ps
                    Max = Constants.MAX_CHANNEL_DELAY, // 800 ps
                    Prefix = Prefix.Pico,
                    Unit = QuantityUnit.Second.ToUnitString(),
                    GetValue = GetChannelDelayValue,
                    Current = 0,
                };
            }

            private AnalogModel _Model;

            private Object _FlagInfo = 0;

            public Object FlagInfo
            {
                get => _FlagInfo;
                set
                {
                    if (_FlagInfo != value)
                    {
                        _FlagInfo = value;
                        OnPropertyChanged();
                        Hardware.HdCmdFactory.Push(HdCmd.ChnlActive | HdCmd.ChnlBandwidth | HdCmd.ChnlPosition | HdCmd.ChnlGain | HdCmd.ChnlInverted | HdCmd.ChnlBias | HdCmd.ChnlScaleIndex | HdCmd.ChnlDelay);
                    }
                }
            }

            private Int32 _Bandwidth = 0;
            public Int32 Bandwidth
            {
                get => _Bandwidth;
                set
                {
                    if (value != _Bandwidth)
                    {
                        _Bandwidth = value;
                        OnPropertyChanged();
                    }

                    if (value == BandWidthNames.Last().Index)
                    {
                        Hd.SetConditionFilter(_Model.Id - ChannelId.C1, true);
                    }
                    else
                    {
                        Hd.SetConditionFilter(_Model.Id - ChannelId.C1, false);
                    }
                }
            }

            private IReadOnlyList<(Int32 Index, String Name)> _BandWidthNames;
            /// <summary>
            /// 当前支持的带宽集合
            /// </summary>
            public IReadOnlyList<(Int32 Index, String Name)> BandWidthNames
            {
                get
                {
                    //var bandwidthnamestemp = PlatformManager.Default.Platform.GetBandWidthNames(OptionsManager.Default.Is2GHz || OptionsManager.Default.GetOptionAvailable(OptionType.BW10T20), Coupling, _Model.Active, _BandWidthNames == null);
                    var bandwidthnamestemp = PlatformManager.Default.Platform.GetBandWidthNames(ScaleBymV < 10 ? true : false, Coupling, _Model.Active, _BandWidthNames == null);

                    if (bandwidthnamestemp != null)
                    {
                        _BandWidthNames = bandwidthnamestemp;
                        if (!_BandWidthNames.Select(bd => bd.Index).Contains(Bandwidth))
                        {
                            Bandwidth = _BandWidthNames[0].Index;
                        }
                    }
                    return _BandWidthNames;
                }
            }

            /// <summary>
            /// 当前通道支持的耦合方式
            /// </summary>
            public IEnumerable<AnaChnlCoupling> SupportedCouplings
            {
                get
                {
                    return PlatformManager.Default.Platform.GetSupportedCouplings(_Model.SerailNumber, DsoModel.Default?.Timebase?.StorageMode ?? AnaChnlStorageMode.Normal);
                }
            }

            /// <summary>
            /// 通道延迟范围限制。
            /// </summary>
            private readonly LimitedValue<Double, Double, Double> _LimitedDelay;

            private Double _Delay = 0;
            /// <summary>
            /// 通道延迟时间。单位：s
            /// </summary>
            public Double Delay
            {
                get => _LimitedDelay.Current;
                set
                {
                    _LimitedDelay.Current = _LimitedDelay.GetValue(value, 0);
                    OnPropertyChanged();
                }
            }


            /// <summary>
            /// 通道延迟范围校验。
            /// </summary>
            /// <param name="arg1">传入延迟值</param>
            /// <param name="arg2">未使用</param>
            /// <returns>更正后的延迟值</returns>
            private Double GetChannelDelayValue(Double arg1, Double arg2)
            {
                if (arg1 == 0 || !IsDelayable)
                    return 0d;

                // 一级延时(单位：ps)：左移/右移 一个点耗时，波形左右移动时，必须是整数个点，也就是必须是：移动时间 = N * firstOrderDelay(整数倍)
                //var firstOrderDelay = 1 / Model.Sampling.AnalogSamplingRate;

                // 必须是DelayStep的整数倍。
                var argps = Math.Round(Quantity.ConvertByPrefix(arg1, Prefix.Empty, Prefix.Pico), 0);
                if (argps == 0)
                    return 0;

                var delaystep = Math.Round(Quantity.ConvertByPrefix(DelayStep, Prefix.Empty, Prefix.Pico));
                if (argps % delaystep != 0)
                    return Quantity.ConvertByPrefix((Int32)(argps / delaystep) * delaystep, Prefix.Pico);

                return arg1;
            }

            /// <summary>
            /// 通道延迟功能是否启用
            /// </summary>
            public Boolean IsDelayable
            {
                get
                {
                    // 采样率≤0或者时基≤0时，都视为不正确值
                    if (_Model.Sampling == null || _Model.Sampling.Scale <= 0 || _Model.Sampling.AnalogSamplingRate <= 0)
                    {
                        Delay = 0;
                        return false;
                    }

                    // 当时基挡位大于50ns时，目前是不会启用插值模式，同样也不启用通道延迟，后续FPGA会改。
                    if (_Model.Sampling.Scale > 0.05)
                    {
                        Delay = 0;
                        return false;
                    }

                    return true;
                }
            }

            private Double _DelayStep = 0d;

            /// <summary>
            /// 通道延迟的最小步进值，单位为：s
            /// </summary>
            public Double DelayStep
            {
                get
                {
                    if (!IsDelayable)
                    {
                        _DelayStep = 0d;
                        return _DelayStep;
                    }

                    // 目前以一级延时为准。
                    _DelayStep = 1d / _Model.Sampling.AnalogSamplingRate;
                    return _DelayStep;

                    /*// 当前时基挡位，单位为：ns
                    var currentScaleByns = Model.Sampling.Scale * 1000;
                    Int32 rate = 1; // 指定时基挡位下，插值倍率。用于计算插值后的采样率。插值后一个点的时间即为最小步进值。
                    switch (currentScaleByns)
                    {
                        case >= 100:
                            // ≥100ns，倍率都是1.
                            rate = 1;
                            break;
                        case < 100 and >= 50:
                            // 50ns,倍率为2
                            rate = 2;
                            break;
                        case < 50 and >= 20:
                            // 20ns,倍率为5
                            rate = 5;
                            break;
                        case < 20 and >= 10:
                            // 10ns,倍率为10
                            rate = 10;
                            break;
                        case < 10 and >= 5:
                            // 5ns,倍率为20
                            rate = 20;
                            break;
                        case < 5 and >= 2:
                            // 2ns,倍率为50
                            rate = 50;
                            break;
                        case < 2:
                            // <1ns,倍率为100
                            rate = 100;
                            break;
                    }

                    // 一级延时(单位：ps)：左移/右移 一个点耗时，波形左右移动时，必须是整数个点，也就是必须是：移动时间 = N * firstOrderDelay(整数倍)
                    //var firstOrderDelay = 1 / Model.Sampling.AnalogSamplingRate * 1000 * 1000 * 1000 * 1000;

                    // 插值后的采样率
                    var newSamplingRate = Model.Sampling.AnalogSamplingRate * rate;

                    // 二级延时：插值后一个点耗时（单位：s） ， 二级延时一个点后的值就是最小移动单位。
                    _delayStep = 1 / newSamplingRate;
                    return _delayStep;*/
                }
            }

            private readonly LimitedPosition<Int64> _LimitedBias;
            public Double BiasByuV
            {
                get => _LimitedBias.GetValue(BiasIndex, 0);
                set => BiasIndex = _LimitedBias.GetIndex(value, 0);
            }

            public Double MaxBias => _LimitedBias.GetValue(_LimitedBias.Max, 0);

            public Double MinBias => _LimitedBias.GetValue(_LimitedBias.Min, 0);

            public Int64 BiasIndex
            {
                get => _LimitedBias.Current;
                set
                {
                    _LimitedBias.Current = value;
                    //!!!Notice: LimitedBias.PropertyChanged is not attach. REF#TriggerModel.cs
                    OnPropertyChanged("Bias");
                }
            }

            private Int64 GetBiasIndexFromValue(Double biasUv, Double _)
            {
                biasUv /= (_ProbeGain * _ProbeUnitRatio);
                var rangeuv = PlatformManager.Default.Platform.GetBiasRange(Coupling, ScaleIndex);
                Double biasindex = (Double)biasUv / rangeuv.MaxUv * _LimitedBias.Max;
                return (Int64)Math.Round(biasindex / Constants.STP_BIAS_UV, MidpointRounding.AwayFromZero) * Constants.STP_BIAS_UV;
            }

            private Double GetBiasValueFromIndex(Int64 biasIndex, Double _)
            {
                var rangeuv = PlatformManager.Default.Platform.GetBiasRange(Coupling, ScaleIndex);
                //biasIndex为±16_000_000，映射到偏置范围
                Double biasuv = (Double)biasIndex / _LimitedBias.Max * rangeuv.MaxUv;
                return biasuv * _ProbeGain * _ProbeUnitRatio;
            }

            private AnaChnlCoupling _Coupling = AnaChnlCoupling.DC50;
            public AnaChnlCoupling Coupling
            {
                get => _Coupling;
                set
                {
                    if (value != _Coupling)
                    {
                        var validcouplings = PlatformManager.Default.Platform.GetSupportedCouplings(_Model?.SerailNumber ?? String.Empty, DsoModel.Default?.Timebase?.StorageMode ?? AnaChnlStorageMode.Normal);
                        if (!validcouplings.Contains(value))
                            return;

                        _Coupling = value;
                        //Hardware.HdCmdFactory.Push(HdCmd.OuterPannelLEDCtrl);
                        OnPropertyChanged();

                        if (_Coupling == AnaChnlCoupling.DC50)
                        {
                            ///2024.06.14  pengbo 当高阻的垂直挡位超过1v时，应将细调值归零
                            if (ScaleIndex > Scale50OhmMaxIndex)
                            {
                                ScaleBymVAdd = 0;
                            }
                            ScaleMaxIndex = Scale50OhmMaxIndex;
                        }
                        else
                        {
                            ScaleMaxIndex = ScaleHighZMaxIndex;
                        }
                    }
                }
            }

            private Boolean _IsInverted = false;
            public Boolean IsInverted
            {
                get => _IsInverted;
                set
                {
                    if (_IsInverted != value)
                    {
                        _IsInverted = value;
                        OnPropertyChanged();
                    }
                }
            }

            private Double _ProbeGain = 1;
            public Double ProbeGain
            {
                get => _ProbeGain;
                set
                {
                    //电流探头增益有转化，当未转化的新值可能与已转化的旧值相同，导致新值设置失败 LHJ
                    //所以探头单位未A时，新旧值相同也需要进行设置
                    if (value != _ProbeGain /*|| _ProbeUnit == ProbeUnitType.A*/)
                    {
                        ScaleBymVAdd /= (_ProbeGain * _ProbeUnitRatio);
                        //if (_ProbeUnit == ProbeUnitType.A)
                        //{
                        //    value = 1D / value;
                        //}

                        if (value < MinProbeGain)
                        {
                            _ProbeGain = MinProbeGain;
                        }
                        else if (value > MaxProbeGain)
                        {
                            _ProbeGain = MaxProbeGain;
                        }
                        else
                        {
                            _ProbeGain = value;
                        }
                        //ScaleBymVAdd *= (_ProbeGain * _ProbeUnitRatio);
                        var trigger = DsoModel.Default.GetTrigger();
                        if (trigger is TriggerSingleSrcModel trig)
                        {
                            Double position = trig.CompPosition;
                            ScaleBymVAdd *= (_ProbeGain * _ProbeUnitRatio);
                            if (trig.Source == _Model.Id)
                                trig.CompPosition = position;
                        }
                        else
                            ScaleBymVAdd *= (_ProbeGain * _ProbeUnitRatio);
                        OnPropertyChanged();
                        Hardware.HdCmdFactory.Push(HdCmd.ChnlGain);
                    }
                }
            }
            public Double MaxProbeGain = 2000; //BUG5642 测试反馈期望倍率能到2000
            public Double MinProbeGain = 0.001;
            private AnaChnlProbe _ProbeIndex = AnaChnlProbe.x1;
            public AnaChnlProbe ProbeIndex
            {
                get => _ProbeIndex;
                set
                {
                    if (_ProbeIndex != value && !_ProbeLocked)
                    {
                        _ProbeIndex = value;
                        ScaleBymVAdd /= (_ProbeGain * _ProbeUnitRatio);
                        switch (_ProbeIndex)
                        {
                            case AnaChnlProbe.x1:
                                //if (_ProbeUnit == ProbeUnitType.A)
                                //    _ProbeGain = 10;
                                //else
                                _ProbeGain = 1;
                                break;
                            case AnaChnlProbe.x5:
                                //if (_ProbeUnit == ProbeUnitType.A)
                                //    _ProbeGain = 10;
                                //else
                                _ProbeGain = 5;
                                break;
                            case AnaChnlProbe.x10:
                                //if (_ProbeUnit == ProbeUnitType.A)
                                //    _ProbeGain = 5;
                                //else
                                _ProbeGain = 10;
                                break;
                            case AnaChnlProbe.x100:
                                //if (_ProbeUnit == ProbeUnitType.A)
                                //    _ProbeGain = 2;
                                //else
                                _ProbeGain = 100;
                                break;
                            default: break;
                        }
                        //ScaleBymVAdd *= (_ProbeGain * _ProbeUnitRatio);
                        var trigger = DsoModel.Default.GetTrigger();
                        if (trigger is TriggerSingleSrcModel trig)
                        {
                            Double position = trig.CompPosition;
                            ScaleBymVAdd *= (_ProbeGain * _ProbeUnitRatio);
                            if (trig.Source == _Model.Id)
                                trig.CompPosition = position;
                        }
                        else
                            ScaleBymVAdd *= (_ProbeGain * _ProbeUnitRatio);
                        OnPropertyChanged();
                        Hardware.HdCmdFactory.Push(HdCmd.ChnlGain);
                    }
                }
            }


            private Double _ProbeUnitRatio = 1;
            /// <summary>
            /// 探头单位比率 x**/V
            /// </summary>
            public Double ProbeUnitRatio
            {
                get => _ProbeUnitRatio;
                set
                {
                    value = Validate(value, MaxProbeUnitRatio, MinProbeUnitRatio);
                    if (_ProbeUnitRatio != value)
                    {
                        ScaleBymVAdd /= (_ProbeGain * _ProbeUnitRatio);
                        {
                            _ProbeUnitRatio = value;
                        }
                        ScaleBymVAdd *= (_ProbeGain * _ProbeUnitRatio);
                        OnPropertyChanged();
                        //简洁修改了增益
                        Hardware.HdCmdFactory.Push(HdCmd.ChnlGain);
                    }
                }
            }
            public Double MaxProbeUnitRatio = 1000;
            public Double MinProbeUnitRatio = 0.001;

            private Boolean _ProbeUnitIsCustomized = false;
            /// <summary>
            /// 探头单位是否是定制的
            /// </summary>
            public Boolean ProbeUnitIsCustomized
            {
                get => _ProbeUnitIsCustomized;
                set
                {
                    if (_ProbeUnitIsCustomized != value)
                    {
                        _ProbeUnitIsCustomized = value;
                        OnPropertyChanged();
                        //间接修改了增益
                        Hardware.HdCmdFactory.Push(HdCmd.ChnlGain);
                    }
                }
            }

            //探头单位删除 统一用 Unit属性
            //private ProbeUnitType _ProbeUnit = ProbeUnitType.V;
            ///// <summary>
            ///// 冗余定义-弹头单位 renark by lihuijun
            /////
            ///// </summary>
            //public ProbeUnitType ProbeUnit
            //{
            //    get => _ProbeUnit;
            //    set
            //    {
            //        if (_ProbeUnit != value)
            //        {
            //            _ProbeUnit = value;
            //            switch (_ProbeIndex)
            //            {
            //                case AnaChnlProbe.x1:
            //                    if (_ProbeUnit == ProbeUnitType.A)
            //                        _ProbeGain = 10;
            //                    else
            //                        _ProbeGain = 1;
            //                    break;
            //                case AnaChnlProbe.x10:
            //                    if (_ProbeUnit == ProbeUnitType.A)
            //                        _ProbeGain = 5;
            //                    else
            //                        _ProbeGain = 10;
            //                    break;
            //                case AnaChnlProbe.x100:
            //                    if (_ProbeUnit == ProbeUnitType.A)
            //                        _ProbeGain = 2;
            //                    else
            //                        _ProbeGain = 100;
            //                    break;
            //                default: break;
            //            }
            //            Unit = _ProbeUnit.ToString();
            //            OnPropertyChanged();
            //        }
            //    }
            //}


            private Boolean _ProbeLocked = false;
            public Boolean ProbeLocked
            {
                get => _ProbeLocked;
                set
                {
                    if (_ProbeLocked != value)
                    {
                        _ProbeLocked = value;
                        OnPropertyChanged();
                    }
                }
            }

            private DateTime _ProbeCaliDate = DateTime.MinValue;
            public DateTime ProbeCaliDate
            {
                get => _ProbeCaliDate;
                set
                {
                    if (_ProbeCaliDate != value)
                    {
                        _ProbeCaliDate = value;
                        OnPropertyChanged();
                    }
                }
            }

            private Double _ProbeGainCaliRatioDefVal = 1;
            /// <summary>
            /// 探头增益校准默认值 出厂校准时存于探头
            /// </summary>
            public Double ProbeGainCaliRatioDefVal
            {
                get => _ProbeGainCaliRatioDefVal;
                set
                {
                    _ProbeGainCaliRatioDefVal = value;
                }
            }

            private Double _ProbeGainCaliRatio = 1;
            /// <summary>
            /// add by lihuijun
            /// 
            /// 探头增益校正系数，默认为1
            /// 检测到有源探头后，根据探头信息从本地校准数据读取
            /// </summary>
            public Double ProbeGainCaliRatio
            {
                get => _ProbeGainCaliRatio;
                set
                {
                    if (_ProbeGainCaliRatio != value)
                    {
                        _ProbeGainCaliRatio = value;
                        Hardware.HdCmdFactory.Push(HdCmd.ChnlGain);
                        OnPropertyChanged();
                    }
                }
            }
            private Double _ProbeOffsetCaliBiasDefVal = 0;
            /// <summary>
            /// 探头偏置校准默认值 出厂校准时存于探头
            /// </summary>
            public Double ProbeOffsetCaliBiasDefVal
            {
                get => _ProbeOffsetCaliBiasDefVal;
                set
                {
                    _ProbeOffsetCaliBiasDefVal = value;
                }
            }

            private Double _ProbeOffsetCaliBias = 0;
            /// <summary>
            /// add by lihuijun
            /// 
            /// 探头偏置校正，默认为0
            /// 检测到有源探头后，根据探头信息从本地校准数据读取
            /// </summary>
            public Double ProbeOffsetCaliBias
            {
                get => _ProbeOffsetCaliBias;
                set
                {
                    if (_ProbeOffsetCaliBias != value)
                    {
                        _ProbeOffsetCaliBias = value;
                        Hardware.HdCmdFactory.Push(HdCmd.ChnlBias);
                        OnPropertyChanged();
                    }
                }
            }

            public AnaChnlScaleIndex ScaleHighZMaxIndex { get; internal set; }

            public AnaChnlScaleIndex Scale50OhmMaxIndex { get; internal set; }

            public new AnaChnlScaleIndex ScaleIndex
            {
                get => (AnaChnlScaleIndex)base.ScaleIndex;
                set => base.ScaleIndex = (Int32)value;
            }

            public new AnaChnlScaleIndex ScaleMaxIndex
            {
                get => (AnaChnlScaleIndex)base.ScaleMaxIndex;
                internal set => base.ScaleMaxIndex = (Int32)value;
            }

            public new AnaChnlScaleIndex ScaleMinIndex
            {
                get => (AnaChnlScaleIndex)base.ScaleMinIndex;
                internal set => base.ScaleMinIndex = (Int32)value;
            }

            private Double GetAnalogScaleFromIndexTick(Int32 index, Int32 tick)
            {
                var (initindex, _) = ScaleFactory.Default.TryGetScaleIndex(InitialScale.Value, InitialScale.Value);
                // modify by lhj 根据档位索引获取Scale，需要乘上 探头倍率增益和单位比率
                return ScaleFactory.Default.GetScale(index + initindex, InitialScale.Value) * _ProbeGain * _ProbeUnitRatio;
            }

            private (Int32 Index, Int32 Tick) GetIndexTickFromAnalogScale(Double scaleValue)
            {
                //var (index, tick) = ScaleFactory.Default.TrySetScale(scaleValue / (InitialScale.Value * _ProbeGain));
                //return (index + InitialScale.Index, tick);
                var (initindex, inittick) = ScaleFactory.Default.TryGetScaleIndex(InitialScale.Value, InitialScale.Value);
                var (index, tick) = ScaleFactory.Default.TryGetScaleIndex(scaleValue, InitialScale.Value);

                index -= initindex;

                return (index, tick);
            }

        }

        private readonly AdaptNum _Deskew;
        public Int64 Deskew
        {
            get => _Deskew.Value;
            set
            {
                if (value != _Deskew.Value)
                {
                    _Deskew.Value = value;
                    Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
                }

            }
        }
        //public Boolean bProbeConnected = false;
        private Boolean _ProbeConnected = false;
        public Boolean ProbeConnected
        {
            get => _ProbeConnected;
            set
            {
                ProbePrompt();
                if (_ProbeConnected != value)
                {
                    AnaChnlCoupling oldcoupling = Conditioning.Coupling;
                    AnaChnlProbe oldprobeindex = Conditioning.ProbeIndex;

                    Conditioning.ProbeLocked = false;
                    _ProbeConnected = value;
                    if (_ProbeConnected)
                    {
                        //删除ProbeUnit，统一使用Unit，并且探头连接后单位默认为V
                        //Conditioning.ProbeUnit = ProbeUnitType.V;
                        Conditioning.Unit = "V";
                    }
                    if (_ProbeConnected && _SerailNumber == "")
                    {
                        Conditioning.Coupling = AnaChnlCoupling.DC1M;
                        Conditioning.ProbeIndex = AnaChnlProbe.x10;
                        Conditioning.ProbeLocked = true;
                        OnPropertyChanged();
                    }
                    if (!_ProbeConnected)
                    {
                        //探头拔出后这校准数据需要复位
                        ProbeSN = String.Empty;
                        Conditioning.ProbeGainCaliRatio = 1;
                        Conditioning.ProbeOffsetCaliBias = 0;

                        Conditioning.Coupling = AnaChnlCoupling.DC1M;
                        Conditioning.ProbeIndex = AnaChnlProbe.x1;
                        Conditioning.ProbeLocked = false;
                        _ReadCount = 0;
                        OnPropertyChanged();
                    }
                    _ProbeDCBiasBymV = 0;
                    if (oldcoupling != Conditioning.Coupling
                        || oldprobeindex != Conditioning.ProbeIndex)
                    {
                        Hardware.HdCmdFactory.Push(HdCmd.ChnlScaleIndex | HdCmd.ChnlGain);
                    }
                }
            }
        }
        //public ProbeMultipurposeButton ProbeButtonFuncSelect = ProbeMultipurposeButton.Led;
        public Boolean ProbeLedLight = false;

        public readonly Int64 MaxDeskew = Constants.MAX_DESKEW_FS;

        public readonly Int64 MinDeskew = Constants.MIN_DESKEW_FS;

        public readonly Int64 StpDeskew = Constants.STP_DESKEW_FS;

        public void AdjDeskew(Int64 delta) => _Deskew.Increase(delta);

        private ProbeKeyType _ProbeBtnType = ProbeKeyType.Headlight;

        public ProbeKeyType ProbeBtnType
        {
            get => _ProbeBtnType;
            set
            {
                if (value != _ProbeBtnType)
                {
                    _ProbeBtnType = value;
                    OnPropertyChanged();
                }
            }
        }

        private AttenuationType _AttenuationType = AttenuationType.Decibel;
        public AttenuationType AttenuationType
        {
            get => _AttenuationType;
            set
            {
                if (value != _AttenuationType)
                {
                    _AttenuationType = value;
                    OnPropertyChanged();
                }
            }
        }



        private String _ProbeHwVersion = String.Empty;
        /// <summary>
        /// 探头固件版本
        /// </summary>
        public String ProbeHwVerion
        {
            get => _ProbeHwVersion;
            set
            {
                if (_ProbeHwVersion != value)
                {
                    _ProbeHwVersion = value;
                    OnPropertyChanged();
                    Hardware.HdCmdFactory.Push(HdCmd.ChnlScaleIndex);
                }
            }
        }

        /// <summary>
        /// 探头SN码(注意与SerailNumber的区别)
        /// </summary>
        public String _ProbeSN = String.Empty;
        public String ProbeSN
        {
            get => _ProbeSN;
            set
            {
                if (_ProbeSN != value)
                {//如果序列号有变化

                    _ProbeSN = value;
                    if (!String.IsNullOrEmpty(_ProbeSN))
                    {//尝试加载用户本地校准数据

                        if (Hd.ProbeManager!.LoadProbeUserCalibDataToLocal(Id, _ProbeSN, out var data))
                        {
                            Conditioning.ProbeGainCaliRatio = data.GainCaliRatio;
                            Conditioning.ProbeOffsetCaliBias = data.OffsetCaliBias;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 探头序列号 实际上存储的为写入的文本串
        /// </summary>
        private String _SerailNumber = String.Empty;
        public String SerailNumber
        {
            get => _SerailNumber;
            set
            {
                if (_SerailNumber != value)
                {
                    _ReadCount++;
                    if (value.StartsWith('\0'))
                    {
                        _SerailNumber = value.Remove(0, 1);
                    }
                    else
                    {
                        _SerailNumber = value;
                    }
                    AnalogInfoUpdate();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// add by xulintao
        /// 
        /// 探头直流偏，默认为0
        /// 检测到有源探头后，根据探头信息从本地校准数据读取
        /// </summary>
        private Double _ProbeDCBiasBymV = 0;
        public Double ProbeDCBiasBymV
        {
            get => _ProbeDCBiasBymV;
            set
            {
                if (_ProbeDCBiasBymV != value)
                {
                    _ProbeDCBiasBymV = value;
                    SetProbeDCBias();
                    OnPropertyChanged();
                }
            }
        }

        public Double MaxProbeDCBiasBymV = 4000;

        public Double MinProbeDCBiasBymV = -4000;

        private static Double Validate(Double value, Double maxvalue, Double minvalue)
        {
            if (value > maxvalue)
            {
                value = maxvalue;
            }
            else if (value < minvalue)
            {
                value = minvalue;
            }
            return value;
        }

        private Int32 _ReadCount = 0;
        private void ProbePrompt()
        {
            if (!_ProbeConnected)
                return;
            PlatformManager.Default.Platform.ProbePrompt(ref _ReadCount, _SerailNumber);
        }

        public void AnalogInfoUpdate()
        {
            AnaChnlCoupling oldcoupling = Conditioning.Coupling;
            AnaChnlProbe oldprobeindex = Conditioning.ProbeIndex;
            Conditioning.ProbeLocked = false;
            if (_SerailNumber.Contains(AnaChnlProbe.x1.GetAlias().First()))
            {
                Conditioning.Coupling = AnaChnlCoupling.DC50;
                try
                {
                    String str = _SerailNumber.Replace("\r", "");
                    String[] probeinfo = str.Split(',');
                    if (probeinfo[3] == AnaChnlProbe.x1.GetAlias())
                    {
                        Conditioning.ProbeIndex = AnaChnlProbe.x1;
                    }
                    else if (probeinfo[3] == AnaChnlProbe.x5.GetAlias())
                    {
                        Conditioning.ProbeIndex = AnaChnlProbe.x5;
                    }
                    else if (probeinfo[3] == AnaChnlProbe.x10.GetAlias())
                    {
                        Conditioning.ProbeIndex = AnaChnlProbe.x10;
                    }
                    else if (probeinfo[3] == AnaChnlProbe.x100.GetAlias())
                    {
                        Conditioning.ProbeIndex = AnaChnlProbe.x100;
                    }
                    ProbeSN = probeinfo[2];
                    Conditioning.ProbeLocked = true;
                }
                catch
                {
                    Conditioning.ProbeIndex = AnaChnlProbe.x1;
                }
            }
            else
            {
                if (_ProbeConnected && _SerailNumber != "")
                {
                    Conditioning.Coupling = AnaChnlCoupling.DC1M;
                    Conditioning.ProbeIndex = AnaChnlProbe.x10;
                    Conditioning.ProbeLocked = true;
                }
                //else
                //{
                //    Conditioning.ProbeIndex = AnaChnlProbe.x1;
                //}
            }

            //if (oldcoupling != Conditioning.Coupling
            //    || oldprobeindex != Conditioning.ProbeIndex)
            //{
            //    //Modify by LHJ 探头插拔回导致属性频繁切换，下发硬件指令导致过快，导致失效；暂时延迟处理
            //    Thread.Sleep(200);
            //    Hardware.HdCmdFactory.Push(HdCmd.ChnlScaleIndex);
            //}
        }

        /// <summary>
        /// 保存用户校准信息(增益和偏置数据)
        /// </summary>
        /// <returns></returns>
        public Boolean SaveProbeUserCalibDataToLocal()
        {
            if (null == Hd.ProbeManager)
            {
                return false;
            }

            var analogModel = (AnalogModel)DsoModel.Default.GetChannel(Id);
            if (null == analogModel)
            {
                return false;
            }

            var data = new ProbeCalibDsoData()
            {
                Id = Id,
                GainCaliRatio = Conditioning.ProbeGainCaliRatio,
                OffsetCaliBias = Conditioning.ProbeOffsetCaliBias,
                ProbeSN = ProbeSN
            };


            return Hd.ProbeManager!.SaveProbeUserCalibDataToLocal(data);
        }

        public Boolean ProbeUserCalibDefault()
        {
            Conditioning.ProbeGainCaliRatio = Conditioning.ProbeGainCaliRatioDefVal;
            Conditioning.ProbeOffsetCaliBias = Conditioning.ProbeOffsetCaliBiasDefVal;
            return SaveProbeUserCalibDataToLocal();
        }

        private void SetProbeDCBias()
        {
            if (!Hd.ProbeManager!.SetProbeCodeValue(Id, _ProbeDCBiasBymV))
            {
                //探头直流偏设置失败！
            }
        }
    }
}

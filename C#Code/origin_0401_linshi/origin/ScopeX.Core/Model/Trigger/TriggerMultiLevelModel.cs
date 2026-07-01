// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    /// <summary>
    /// Defines the <see cref="TriggerMultiLevelModel" />.
    /// </summary>
    internal abstract class TriggerMultiLevelModel : TriggerModel
    {
        public TriggerMultiLevelModel()
        {
            _Positions = new (Double, Double, Double, Double)[ChannelIdExt.AnaChnlNum + 1];
            for (Int32 i = 0; i < _Positions.Length; i++)
            {
                _Positions[i] = new(0, Constants.MIN_TRIGGER_GAP, 0, 0);
            };
        }

        // 电平部分不需要磁吸效果，先去掉
        /*private readonly MagnetManager _magnetManager_lower = new MagnetManager()
        {
            Threshold = 50,
        };

        private readonly MagnetManager _magnetManager_upper = new MagnetManager()
        {
            Threshold = 50,
        };*/

        private readonly Int32 _ANA_COMP_SITE = ChannelIdExt.AnaChnlNum;

        private readonly (Double Lower, Double Upper, Double LowerValue, Double UpperValue)[] _Positions;
        //private readonly (Double Lower, Double Upper, Double LowerValue, Double UpperValue)[] _Positions = new (Double, Double, Double, Double)[]
        //{
        //    new(0, Constants.MIN_TRIGGER_GAP, 0, 0),
        //    new(0, Constants.MIN_TRIGGER_GAP, 0, 0),
        //    new(0, Constants.MIN_TRIGGER_GAP, 0, 0),
        //    new(0, Constants.MIN_TRIGGER_GAP, 0, 0),
        //    new(0, Constants.MIN_TRIGGER_GAP, 0, 0),
        //};

        private PulseCondition _Condition = PulseCondition.GreaterThan;

        private ChannelId _Source = ChannelId.C1;

        private Int64 _WidthByps = Constants.MIN_PULSEWIDTH_PS;

        private Int64 _UpperWidthByps = Constants.MIN_PULSEWIDTH_PS + 2 * Constants.STP_PULSEWIDTH_PS;

        /// <summary>
        /// 电平值
        /// </summary>
        public (Double Lower, Double Upper) CompPosition
        {
            get => GetPosition(_ANA_COMP_SITE);
            set => SetPosition(_ANA_COMP_SITE, value);
        }

        /// <summary>
        /// 无范围的电平值
        /// </summary>
        public Double NoRangeCompPosition
        {
            get => GetNoRangePosition(_ANA_COMP_SITE);
            set => SetPosition(_ANA_COMP_SITE, value);
        }

        /// <summary>
        /// 高电平值
        /// </summary>
        public Double UpperCompPosition
        {
            get => GetPosition(_ANA_COMP_SITE).Upper;
            set => SetPosition(_ANA_COMP_SITE, (null, value));
        }

        /// <summary>
        /// 低电平值
        /// </summary>
        public Double LowerCompPosition
        {
            get => GetPosition(_ANA_COMP_SITE).Lower;
            set => SetPosition(_ANA_COMP_SITE, (value, null));
        }

        /// <summary>
        /// 最大电平值
        /// </summary>
        public Double MaxCompPosition => GetMaxPosition(_ANA_COMP_SITE);

        /// <summary>
        /// 最小电平值
        /// </summary>
        public Double MinCompPosition => GetMinPosition(_ANA_COMP_SITE);

        /// <summary>
        /// 电平值虚拟坐标 -- (下限,上限) [相对于通道0电平的虚拟坐标，不是相对于屏幕垂直0点的坐标]
        /// </summary>
        public (Double Lower, Double Upper) PosIndex => GetPosIndex(_ANA_COMP_SITE);

        public Double NoRangePosIndex
        {
            get
            {
                var pos = GetAnalogAxisInfo(Source).Pos0;
                return GetNoRangePosIndex(_ANA_COMP_SITE) + pos;
            }
            set
            {
                var pos = GetAnalogAxisInfo(Source).Pos0;
                SetPosIndex(_ANA_COMP_SITE, (value - pos, null), 0);
            }
        }

        /// <summary>
        /// 电平值虚拟坐标 -- 上限 [相对于通道0电平的虚拟坐标，不是相对于屏幕垂直0点的坐标]
        /// </summary>
        public Double PosUpperIndex
        {
            get => GetPosIndex(_ANA_COMP_SITE).Upper;
            set => SetPosIndex(_ANA_COMP_SITE, (null, value));
        }

        /// <summary>
        /// 电平值虚拟坐标 -- 下限 [相对于通道0电平的虚拟坐标，不是相对于屏幕垂直0点的坐标]
        /// </summary>
        public Double PosLowerIndex
        {
            get => GetPosIndex(_ANA_COMP_SITE).Lower;
            set => SetPosIndex(_ANA_COMP_SITE, (value, null));
        }

        public Prefix PosPrefix => GetPosPrefix(Source);

        public String PosUnit => GetPosUnit(Source);

        public Double RelPosUpperIndex
        {
            get => GetRelPosIndex(_ANA_COMP_SITE).Upper;
            set => SetRelUpperPosIndex(_ANA_COMP_SITE, value);
        }

        public Double RelPosLowerIndex
        {
            get => GetRelPosIndex(_ANA_COMP_SITE).Lower;
            set => SetRelLowerPosIndex(_ANA_COMP_SITE, value);
        }

        public PulseCondition Condition
        {
            get => _Condition;
            set
            {
                if (value != _Condition)
                {
                    _Condition = value;

                    if (value == PulseCondition.NotEqual || value == PulseCondition.Equal)
                    {
                        _WidthByps = ValidateWidth(_WidthByps, MinWidth, MaxWidth - 2 * StpWidth);
                        _UpperWidthByps = ValidateWidth(_UpperWidthByps, _WidthByps + 2 * StpWidth, MaxWidth);
                    }


                    OnPropertyChanged();
                }
            }
        }

        public ChannelId Source
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

        public Int64 MaxWidth
        {
            get; init;
        } = Constants.MAX_PULSEWIDTH_PS;

        public Int64 MinWidth
        {
            get; init;
        } = Constants.MIN_PULSEWIDTH_PS;


        public Int64 StpWidth
        {
            get; init;
        } = Constants.STP_PULSEWIDTH_PS;

        public Int64 WidthByps
        {
            get => _WidthByps;
            set
            {
                var (min, max) = GetWidthRange(0);
                value = ValidateWidth(value, min, max);

                if (value != _WidthByps)
                {
                    _WidthByps = value;

                    var minoffset = 2 * StpWidth;
                    if (Condition == PulseCondition.Equal || Condition == PulseCondition.NotEqual)
                    {
                        // 相等或者不相等时，最大值和最小值必须有 2 * StpWidth的差异
                        if (_WidthByps > max)
                            _WidthByps = max;

                        if (_UpperWidthByps < (_WidthByps + minoffset))
                        {
                            _UpperWidthByps = _WidthByps + minoffset;
                            OnPropertyChanged(nameof(UpperWidthByps));
                        }
                    }
                    else
                    {
                        // 大于或者小于时，最大值可以和最小值相等。
                        if (_UpperWidthByps < _WidthByps)
                        {
                            _UpperWidthByps = _WidthByps;
                            OnPropertyChanged(nameof(UpperWidthByps));
                        }
                    }

                    OnPropertyChanged();
                }
            }
        }

        public Int64 UpperWidthByps
        {
            get => _UpperWidthByps;
            set
            {
                var (min, max) = GetWidthRange(1);
                value = ValidateWidth(value, min, max);

                if (value != _UpperWidthByps)
                {
                    _UpperWidthByps = value;

                    var minoffset = 2 * StpWidth;
                    if (Condition == PulseCondition.Equal || Condition == PulseCondition.NotEqual)
                    {
                        // 相等或者不相等时，最大值和最小值必须有 2 * StpWidth的差异
                        if (_UpperWidthByps < min)
                            _UpperWidthByps = min;

                        if (_WidthByps > (_UpperWidthByps - minoffset))
                        {
                            _WidthByps = _UpperWidthByps - minoffset;
                            OnPropertyChanged(nameof(WidthByps));
                        }
                    }
                    else
                    {
                        // 大于或者小于时，最大值可以和最小值相等。
                        if (_WidthByps > _UpperWidthByps)
                        {
                            _WidthByps = _UpperWidthByps;
                            OnPropertyChanged(nameof(WidthByps));
                        }
                    }
                    OnPropertyChanged();
                }
            }
        }

        protected internal (Int64 min, Int64 max) GetWidthRange(Int32 index)
        {
            /*return Condition switch
            {
                PulseCondition.Equal or PulseCondition.NotEqual => index == 0 ? (MinWidth, UpperWidthByps - 2 * StpWidth) : (WidthByps + 2 * StpWidth, MaxWidth),
                _ => (MinWidth, MaxWidth),
            };*/

            return Condition switch
            {
                PulseCondition.Equal or PulseCondition.NotEqual => index == 0 ? (MinWidth, MaxWidth - 2 * StpWidth) : (MinWidth + 2 * StpWidth, MaxWidth),
                _ => (MinWidth, MaxWidth),
            };
        }

        protected virtual Int64 ValidateWidth(Int64 value, Int64 min, Int64 max)
        {
            //规范时间值的大小在最小精度内
            value = (Int64)Math.Round((Double)value / StpWidth, MidpointRounding.AwayFromZero) * StpWidth;
            if (value > max)
            {
                value = max;
                WeakTip.Default.Write("Trigger", MsgTipId.GreatethanMax);
            }
            else if (value < min)
            {
                value = min;
                WeakTip.Default.Write("Trigger", MsgTipId.LessthanMin);
            }

            return value;
        }

        public override Double MaxPosIndex
        {
            get
            {
                if (Source.IsAnalog())
                {
                    var chmodel = DsoModel.Default.AnalogChnls.First(x => x.Id == Source);
                    return Constants.MAX_TRIGGER_IDX - chmodel.Conditioning.PosIndex;
                }
                else return Constants.MAX_TRIGGER_IDX;
            }
        }
        public override Double MinPosIndex
        {
            get
            {
                if (Source.IsAnalog())
                {
                    var chmodel = DsoModel.Default.AnalogChnls.First(x => x.Id == Source);
                    return Constants.MIN_TRIGGER_IDX - chmodel.Conditioning.PosIndex;
                }
                else return Constants.MIN_TRIGGER_IDX;
            }
        }
        public Double GetMaxPosition(Int32 _)
        {
            return PosIndexToValue(Source, MaxPosIndex);
        }

        public Double GetMinPosition(Int32 _)
        {
            return PosIndexToValue(Source, MinPosIndex);
        }

        public override void LeapPosIndex()
        {
            LeapPosIndex(_ANA_COMP_SITE);
        }

        protected (Double Lower, Double Upper) GetRelPosIndex(Int32 site)
        {
            var pos = GetAnalogAxisInfo(Source).Pos0;
            var (loidx, upidx) = GetPosIndex(site);
            return (pos + loidx, pos + upidx);
        }

        protected void SetRelUpperPosIndex(Int32 site, Double value)
        {
            var pos = GetAnalogAxisInfo(Source).Pos0;
            SetPosIndex(site, (null, value - pos));
        }

        protected void SetRelLowerPosIndex(Int32 site, Double value)
        {
            var pos = GetAnalogAxisInfo(Source).Pos0;
            SetPosIndex(site, (value - pos, null));
        }

        private protected (Double Lower, Double Upper) GetPosIndex(Int32 site)
        {
            return (_Positions[site].Lower, _Positions[site].Upper);
        }

        private protected Double GetNoRangePosIndex(Int32 site)
        {
            return _Positions[site].Lower;
        }

        private protected (Double Lower, Double Upper) GetPosition(Int32 site)
        {
            var (loidx, upidx) = GetPosIndex(site);
            return (PosIndexToValue(Source, loidx), PosIndexToValue(Source, upidx));
        }

        private protected Double GetNoRangePosition(Int32 site)
        {
            var (loidx, _) = GetPosIndex(site);
            return PosIndexToValue(Source, loidx);
        }

        private protected void LeapPosIndex(Int32 site)
        {
            var (lower, upper) = (_Positions[site].Lower, _Positions[site].Upper);

            _Positions[site].Lower = ValidatePosIndex(ValueToPosIndex(Source, _Positions[site].LowerValue),false);
            if (_Positions[site].Lower != lower)
            {
                OnPropertyChanged(nameof(PosIndex));
            }

            _Positions[site].Upper = ValidatePosIndex(ValueToPosIndex(Source, _Positions[site].UpperValue),false);
            if (_Positions[site].Upper != upper)
            {
                OnPropertyChanged(nameof(PosIndex));
            }
        }

        private protected void SetPosIndex(Int32 site, (Double? lower, Double? upper) posIndex, Double mingap = Constants.MIN_TRIGGER_GAP)
        {
            // 电平部分不需要磁吸效果
            /*if (posIndex.lower.HasValue)
            {
                var newval = _magnetManager_lower.Determine(posIndex.lower.Value, MinPosIndex, MaxPosIndex);
                if (newval != null)
                    posIndex.lower = newval;
            }

            if (posIndex.upper.HasValue)
            {
                var newval = _magnetManager_upper.Determine(posIndex.upper.Value, MinPosIndex, MaxPosIndex);
                if (newval != null)
                    posIndex.upper = newval;
            }*/

            Double upidx, loidx;
            switch (posIndex.lower.HasValue, posIndex.upper.HasValue)
            {
                case (true, true):
                    upidx = ValidatePosIndex(posIndex.upper!.Value);
                    loidx = ValidatePosIndex(posIndex.lower!.Value);
                    break;
                case (false, true):
                    upidx = posIndex.upper!.Value;
                    if (upidx > MaxPosIndex)
                    {
                        WeakTip.Default.Write("PosIndex", MsgTipId.GreatethanMax, false, "", 1);
                        upidx = MaxPosIndex;
                    }
                    loidx = _Positions[site].Lower;
                    if (upidx < loidx + mingap)
                    {
                        loidx = ValidatePosIndex(upidx - mingap);
                        if (upidx < loidx + mingap)
                        {
                            upidx = loidx + mingap;
                        }
                    }
                    break;
                case (true, false):
                    upidx = _Positions[site].Upper;
                    loidx = posIndex.lower!.Value;
                    if (loidx < MinPosIndex)
                    {
                        WeakTip.Default.Write("PosIndex", MsgTipId.LessthanMin, false, "", 1);
                        loidx = MinPosIndex;
                    }
                    if (loidx > upidx - mingap)
                    {
                        upidx = ValidatePosIndex(loidx + mingap);
                        if (loidx > upidx - mingap)
                        {
                            loidx = upidx - mingap;
                        }
                    }
                    break;
                default:
                    return;
            }

            upidx = ValidatePosIndex(upidx);
            if (upidx != _Positions[site].Upper)
            {
                _Positions[site].Upper = upidx;
                _Positions[site].UpperValue = PosIndexToValue(Source, upidx);
                OnPropertyChanged(nameof(PosIndex));
            }

            loidx = ValidatePosIndex(loidx);
            if (loidx != _Positions[site].Lower)
            {
                _Positions[site].Lower = loidx;
                _Positions[site].LowerValue = PosIndexToValue(Source, loidx);
                OnPropertyChanged(nameof(PosIndex));
            }
        }

        private protected void SetPosition(Int32 site, (Double? lov, Double? upv) posValue)
        {
            Double? upidx, loidx;
            if (posValue.lov.HasValue)
            {
                loidx = ValueToPosIndex(Source, posValue.lov.Value);
            }
            else
            {
                loidx = null;
            }

            if (posValue.upv.HasValue)
            {
                upidx = ValueToPosIndex(Source, posValue.upv.Value);
            }
            else
            {
                upidx = null;
            }

            SetPosIndex(site, (loidx, upidx));
        }

        private protected void SetPosition(Int32 site, Double posValue)
        {
            var loidx = ValueToPosIndex(Source, posValue);
            SetPosIndex(site, (loidx, null), 0);
        }


        public Double SetCompPosIndexCenter(ChannelId ts)
        {
            try
            {
                var pkg = DsoModel.Default.GetWfmPack(ts);
                Double[] buffer = pkg.Buffer.Cast<Double>().ToArray();
                Double centerpos = (buffer.Max() + buffer.Min()) / 2;
                return centerpos;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}

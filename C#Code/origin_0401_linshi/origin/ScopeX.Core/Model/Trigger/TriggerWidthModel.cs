// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using NPOI.SS.Formula.Functions;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    internal class TriggerGlitchModel : TriggerWidthModel
    {
        public TriggerGlitchModel()
        {
            Condition = PulseCondition.LessThan;
        }

        public override String Name => TriggerType.Glitch.ToString();
    }

    internal class TriggerIntervalModel : TriggerWidthModel
    {
        public override String Name => TriggerType.Interval.ToString();
    }

    internal class TriggerWidthModel : TriggerSingleSrcModel
    {
        public TriggerWidthModel(UInt32 effiDigits = 3, Int64 maxWidth = Constants.MAX_PULSEWIDTH_PS, Int64 minWidth = Constants.MIN_PULSEWIDTH_PS, Int64 stpWidth = Constants.STP_PULSEWIDTH_PS)
        {
            EffiDigits = effiDigits;

            MaxWidth = maxWidth;
            MinWidth = minWidth;
            StpWidth = stpWidth;

            /*_WidthByps = new(effiDigits, () => OnPropertyChanged(nameof(WidthByps)))
            {
                Max = MaxWidth,
                Min = MinWidth,
                Stp = StpWidth,
                Value = MinWidth
            };

            _UpperWidthByps = new(effiDigits, () => OnPropertyChanged(nameof(UpperWidthByps)))
            {
                Max = MaxWidth,
                Min = MinWidth + 2 * StpWidth,
                Stp = StpWidth,
                Value = MinWidth + 2 * StpWidth,
            };*/
        }

        public override String Name => TriggerType.PulseWidth.ToString();

        private PulsePolarity _Polarity = PulsePolarity.Positive;
        public PulsePolarity Polarity
        {
            get => _Polarity;
            set
            {
                if (value != _Polarity)
                {
                    _Polarity = value;
                    OnPropertyChanged();
                }
            }
        }

        private PulseCondition _Condition = PulseCondition.GreaterThan;
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
                        //_WidthByps = ValidateWidth(_WidthByps, MinWidth, MaxWidth - 2 * StpWidth);
                        //_UpperWidthByps = ValidateWidth(_UpperWidthByps, _WidthByps + 2 * StpWidth, MaxWidth);
                        var maxlower = AdaptNum.GetNext(MaxWidth, -2, EffiDigits, StpWidth);
                        var minupper = AdaptNum.GetNext(WidthByps, 2, EffiDigits, StpWidth);
                        //_WidthByps.Value = ValidateWidth(WidthByps, MinWidth, maxlower);
                        _WidthByps = ValidateWidth(WidthByps, MinWidth, maxlower);
                        _UpperWidthByps = ValidateWidth(UpperWidthByps, minupper, MaxWidth);
                    }

                    OnPropertyChanged();
                }
            }
        }

        //private Int64 _WidthByps = Constants.MIN_PULSEWIDTH_PS;
        //public Int64 WidthByps
        //{
        //    get => _WidthByps;
        //    set
        //    {
        //        var (min, max) = GetWidthRange(0);
        //        value = ValidateWidth(value, min, max);

        //        if (value != _WidthByps)
        //        {
        //            _WidthByps = value;
        //            if (_UpperWidthByps < value + 2 * StpWidth)
        //            {
        //                _UpperWidthByps = value + 2 * StpWidth;
        //            }
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        protected UInt32 EffiDigits
        {
            get;
        }

        // private readonly AdaptNum _WidthByps;
        private Int64 _WidthByps = Constants.MIN_PULSEWIDTH_PS;
        public Int64 WidthByps
        {
            get => _WidthByps;
            set
            {
                var (min, max) = GetWidthRange(0);
                value = ValidateWidth(value, min, max);

                /*if (value != _WidthByps.Value)
                {
                    switch (Condition)
                    {
                        case PulseCondition.Equal:
                        case PulseCondition.NotEqual:
                            _WidthByps.Min = MinWidth + 2 * StpWidth;
                            break;
                        case PulseCondition.GreaterThan:
                        case PulseCondition.LessThan:
                        default:
                            _WidthByps.Min = min;
                            break;
                    }

                    _WidthByps.Value = value;
                    var minupper = value + 2 * StpWidth; //AdaptNum.GetNext(value, 2, EffiDigits, StpWidth);
                    if (_UpperWidthByps.Value < minupper)
                    {
                        _UpperWidthByps.Value = minupper;
                    }
                }*/

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

        //private Int64 _UpperWidthByps = Constants.MIN_PULSEWIDTH_PS;
        //public Int64 UpperWidthByps
        //{
        //    get => _UpperWidthByps;
        //    set
        //    {
        //        var (min, max) = GetWidthRange(1);
        //        value = ValidateWidth(value, min, max);

        //        if (value != _UpperWidthByps)
        //        {
        //            _UpperWidthByps = value;
        //            if (_WidthByps > value - 2 * StpWidth)
        //            {
        //                _WidthByps = value - 2 * StpWidth;
        //            }
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        // private readonly AdaptNum _UpperWidthByps;
        private Int64 _UpperWidthByps = Constants.MIN_PULSEWIDTH_PS + 2 * Constants.STP_PULSEWIDTH_PS;
        public Int64 UpperWidthByps
        {
            get => _UpperWidthByps;
            set
            {
                var (min, max) = GetWidthRange(1);
                value = ValidateWidth(value, min, max);

                /*if (value != _UpperWidthByps.Value)
                {
                    switch (Condition)
                    {
                        case PulseCondition.Equal:
                        case PulseCondition.NotEqual:
                            _UpperWidthByps.Min = MinWidth + 2 * StpWidth;
                            break;
                        case PulseCondition.GreaterThan:
                        case PulseCondition.LessThan:
                        default:
                            _UpperWidthByps.Min = min;
                            break;
                    }

                    _UpperWidthByps.Value = value;
                    var maxlower = value - 2 * StpWidth; // AdaptNum.GetNext(value, -2, EffiDigits, StpWidth);
                    if (_WidthByps.Value > maxlower)
                    {
                        _WidthByps.Value = maxlower;
                    }
                    OnPropertyChanged();
                }*/

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

        public Int64 MaxWidth
        {
            get;
        }

        public Int64 MinWidth
        {
            get;
        }

        public Int64 StpWidth
        {
            get;
        }

        protected internal (Int64 min, Int64 max) GetWidthRange(Int32 index)
        {
            return Condition switch
            {
                PulseCondition.Equal or PulseCondition.NotEqual => index == 0 ? (MinWidth, UpperWidthByps - 2 * StpWidth) : (WidthByps + 2 * StpWidth, MaxWidth),
                _ => (MinWidth, MaxWidth),
            };
            /*var maxlower = AdaptNum.GetNext(MinWidth, 2, EffiDigits, StpWidth);
            var minupper = AdaptNum.GetNext(MaxWidth, -2, EffiDigits, StpWidth);
            return Condition switch
            {
                PulseCondition.Equal or PulseCondition.NotEqual => index == 0 ? (MinWidth, minupper) : (maxlower, MaxWidth),
                _ => (MinWidth, MaxWidth),
            };*/
        }

        public void AdjWidth(Int64 delta)
        {
            // WidthByps = AdaptNum.GetNext(WidthByps, delta, EffiDigits, StpWidth);
            if ((WidthByps >= 1E6) && (WidthByps < 1E9))
            {
                WidthByps += delta * (Int32)1E3;
            }
            else if ((WidthByps >= 1E9) && (WidthByps < 1E12))
            {
                WidthByps += delta * (Int32)1E6;
            }
            else if (WidthByps >= 1E12)
            {
                WidthByps += delta * (Int32)1E9;
            }
            else
            {
                WidthByps += delta * StpWidth;
            }
        }

        public void AdjUpperWidth(Int64 delta)
        {
            // UpperWidthByps = AdaptNum.GetNext(UpperWidthByps, delta, EffiDigits, StpWidth);
            if ((UpperWidthByps >= 1E6) && (UpperWidthByps < 1E9))
            {
                UpperWidthByps += delta * (Int32)1E3;
            }
            else if ((UpperWidthByps >= 1E9) && (UpperWidthByps < 1E12))
            {
                UpperWidthByps += delta * (Int32)1E6;
            }
            else if (UpperWidthByps >= 1E12)
            {
                UpperWidthByps += delta * (Int32)1E9;
            }
            else
            {
                UpperWidthByps += delta * StpWidth;
            }
        }

        protected virtual Int64 ValidateWidth(Int64 value, Int64 min, Int64 max)
        {
            return Math.Clamp(value, min, max);
            /*//规范时间值的大小在最小精度内
            //value = (Int64)Math.Round((Double)value / StpWidth, MidpointRounding.AwayFromZero) * StpWidth;
            if (value == 0)
                value = min;
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

            return value;*/
        }
    }
}

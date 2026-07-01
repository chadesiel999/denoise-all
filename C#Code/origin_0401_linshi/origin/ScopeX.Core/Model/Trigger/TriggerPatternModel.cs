// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using ScopeX.ComModel;



    //判断所有的触发源的状态
    internal class TriggerPatternModel : TriggerModel
    {
        public override String Name => TriggerType.Pattern.ToString();

        internal class TriggerPatternBit : TriggerSingleSrcModel
        {
            public override String Name => "PatternBits";

            private readonly Dictionary<ChannelId, PatLevelCondition> _Conditions;

            public PatLevelCondition GetCondition(ChannelId ts)
            {
                if (!_Conditions.ContainsKey(ts))
                    return new PatLevelCondition();
                return _Conditions[ts];
            }

            public void SetCondition(ChannelId ts, PatLevelCondition plc)
            {
                if (plc != _Conditions[ts])
                {
                    _Conditions[ts] = plc;
                    OnPropertyChanged("CompCondition");
                }
            }

            public Int32 Length => _Conditions.Count;

            public TriggerPatternBit()
            {
                IsAnalogFirst = false;

                _Conditions = new();
                foreach (var ts in ChannelIdExt.GetAnalogs())
                {
                    _Conditions.Add(ts, PatLevelCondition.GreaterThan);
                }

                foreach (var ts in ChannelIdExt.GetDigitals())
                {
                    _Conditions.Add(ts, PatLevelCondition.GreaterThan);
                }

                _Conditions.Add(ChannelId.Ext, PatLevelCondition.GreaterThan);
                _Conditions.Add(ChannelId.AC, PatLevelCondition.GreaterThan);
                _Conditions.Add(ChannelId.AuxIn, PatLevelCondition.GreaterThan);
            }
        }

        private PatOperator _Operator = PatOperator.And;
        public PatOperator Operator
        {
            get => _Operator;
            set
            {
                if (value != _Operator)
                {
                    _Operator = value;
                    OnPropertyChanged();
                }
            }
        }

        private PatTimeCondition _TimeCondition = PatTimeCondition.GreaterThan;
        public PatTimeCondition TimeCondition
        {
            get => _TimeCondition;
            set
            {
                if (value != _TimeCondition)
                {
                    _TimeCondition = value;

                    if (value == PatTimeCondition.Inside || value == PatTimeCondition.Outside)
                    {
                        _DurationByps = ValidateDuration(_DurationByps, MinDuration, MaxDuration - 2 * StpDuration);
                        _UpperDurationByps = ValidateDuration(_UpperDurationByps, _DurationByps + 2 * StpDuration, MaxDuration);
                    }

                    OnPropertyChanged();
                }
            }
        }

        private Int64 _DurationByps = Constants.MIN_PULSEWIDTH_PS;

        public Int64 DurationByps
        {
            get => _DurationByps;
            set
            {
                var (min, max) = GetDurationRange(0);
                value = ValidateDuration(value, min, max);

                if (value != _DurationByps)
                {
                    _DurationByps = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int64 _UpperDurationByps = Constants.MIN_PULSEWIDTH_PS;
        public Int64 UpperDurationByps
        {
            get => _UpperDurationByps;
            set
            {
                var (min, max) = GetDurationRange(1);
                value = ValidateDuration(value, min, max);

                if (value != _UpperDurationByps)
                {
                    _UpperDurationByps = value;
                    OnPropertyChanged();
                }
            }
        }

        protected internal (Int64 min, Int64 max) GetDurationRange(Int32 index)
        {
            return TimeCondition switch
            {
                PatTimeCondition.Inside or PatTimeCondition.Outside => index == 0 ? (MinDuration, UpperDurationByps - 2 * StpDuration) : (DurationByps + 2 * StpDuration, MaxDuration),
                _ => (MinDuration, MaxDuration),
            };
        }

        protected virtual Int64 ValidateDuration(Int64 value, Int64 min, Int64 max)
        {
            //规范时间值的大小在最小精度内
            value = (Int64)Math.Round((Double)value / StpDuration, MidpointRounding.AwayFromZero) * StpDuration;
            if (value > max)
            {
                value = max;
            }
            else if (value < min)
            {
                value = min;
            }

            return value;
        }

        public Int64 MaxDuration
        {
            get;
            init;
        } = Constants.MAX_PULSEWIDTH_PS;

        public Int64 MinDuration
        {
            get;
            init;
        } = Constants.MIN_PULSEWIDTH_PS;

        public Int64 StpDuration
        {
            get;
            init;
        } = Constants.STP_PULSEWIDTH_PS;

        public TriggerPatternBit Bits
        {
            get;
        } = new();

        public Int32 Length => Bits.Length;

        public override void LeapPosIndex()
        { }

        //!!!Notice: TriggerModel.PropertyChanged is virtual. REF#AnalogModel.cs
        public override event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                base.PropertyChanged += value;
                Bits.PropertyChanged += value;

            }
            remove
            {
                base.PropertyChanged -= value;
                Bits.PropertyChanged -= value;
            }
        }
    }
}

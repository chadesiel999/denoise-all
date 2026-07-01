// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using NPOI.Util;
    using ScopeX.ComModel;
    using static ScopeX.Core.TriggerPatternModel;

    //高、低电平持续时间过长，或电平长时间不改变
    /// <summary>
    /// Defines the <see cref="TriggerSustainTimeModel" />.
    /// </summary>
    internal class TriggerSustainTimeModel : TriggerMultiLevelModel
    {
        /// <summary>
        /// Gets the Name.
        /// </summary>
        public override String Name => TriggerType.SustainTime.ToString();

        internal class TriggerSustainTimeBit : TriggerSingleSrcModel
        {
            public override String Name => "TriggerSustainTimeBits";

            private readonly Dictionary<ChannelId, SustainTimeLevelCondition> _Conditions;

            public SustainTimeLevelCondition GetCondition(ChannelId ts)
            {
                return _Conditions[ts];
            }

            internal Dictionary<ChannelId, SustainTimeLevelCondition> GetConditions() => _Conditions.Copy();

            public void SetCondition(ChannelId ts, SustainTimeLevelCondition plc)
            {
                if (plc != _Conditions[ts])
                {
                    _Conditions[ts] = plc;
                    OnPropertyChanged("CompCondition");
                }
            }

            public Int32 Length => _Conditions.Count;

            public TriggerSustainTimeBit()
            {
                IsAnalogFirst = false;

                _Conditions = new();
                foreach (var ts in ChannelIdExt.GetAnalogs())
                {
                    _Conditions.Add(ts, SustainTimeLevelCondition.GreaterThan);
                }

                foreach (var ts in ChannelIdExt.GetDigitals())
                {
                    _Conditions.Add(ts, SustainTimeLevelCondition.GreaterThan);
                }

                _Conditions.Add(ChannelId.Ext, SustainTimeLevelCondition.GreaterThan);
                _Conditions.Add(ChannelId.AC, SustainTimeLevelCondition.GreaterThan);
                _Conditions.Add(ChannelId.AuxIn, SustainTimeLevelCondition.GreaterThan);
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
                    base.Condition = _Condition;
                    OnPropertyChanged();
                }
            }
        }

        public TriggerSustainTimeBit Bits
        {
            get;
        } = new();

        public Int32 Length => Bits.Length;

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

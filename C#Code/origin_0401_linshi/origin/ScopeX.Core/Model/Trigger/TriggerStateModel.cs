// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using System.ComponentModel;
    using ScopeX.ComModel;

    //除了一个作为时钟源以外，其他所有触发源的状态

    internal class TriggerStateModel : TriggerPatternModel
    {
        public override String Name => TriggerType.State.ToString();

        private Boolean _Conformed = true;
        public Boolean Conformed
        {
            get => _Conformed;
            set
            {
                if (value != _Conformed)
                {
                    _Conformed = value;
                    OnPropertyChanged();
                }
            }
        }

        private ChannelId _ClkSource = ChannelId.C1;

        public ChannelId ClkSource
        {
            get => _ClkSource;
            set
            {
                if (value != _ClkSource)
                {
                    _ClkSource = value;
                    OnPropertyChanged();
                }
            }
        }

        private PulsePolarity _ClkPolarity = PulsePolarity.Positive;
        public PulsePolarity ClkPolarity
        {
            get => _ClkPolarity;
            set
            {
                if (value != _ClkPolarity)
                {
                    _ClkPolarity = value;
                    OnPropertyChanged();
                }
            }
        }

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

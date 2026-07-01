// Copyright (c) UESTC. All Rights Reserved
// <author>QC</author>
// <date>2022/4/6</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    internal class MixerNode : DspNode
    {
        public readonly Double MaxSuggestedFreq = Constants.MAX_MIXER_FREQ;

        public readonly Double MinSuggestedFreq = Constants.MIN_MIXER_FREQ;

        private Double _SuggestedFreq = Constants.MIN_MIXER_FREQ;

        public MixerNode(Action<String>? onPropertyChanged = null) : base(VsaNodeTypeOpt.Mixer)
        {
            OnPropertyChanged = onPropertyChanged;
        }

        public Double SuggestedFreq
        {
            get => _SuggestedFreq;
            set
            {
                value = ValidateFreq(value);
                if (_SuggestedFreq != value)
                {
                    _SuggestedFreq = value;
                    OnPropertyChanged?.Invoke(nameof(SuggestedFreq));
                }
            }
        }

        public override Double[] Process(Double[] buffer) => buffer;

        private Double ValidateFreq(Double value)
        {
            value = Math.Round(value / Constants.STP_MIXER_FREQ) * Constants.STP_MIXER_FREQ;
            if (value > MaxSuggestedFreq)
            {
                value = MaxSuggestedFreq;
            }
            else if (value < MinSuggestedFreq)
            {
                value = MinSuggestedFreq;
            }

            return value;
        }
    }
}

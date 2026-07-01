// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    /// <summary>
    /// Defines the <see cref="TriggerRuntModel" />.
    /// 两个比较电平
    /// </summary>
    internal class TriggerRuntModel : TriggerMultiLevelModel
    {
        /// <summary>
        /// Defines the _Polarity.
        /// </summary>
        private PulsePolarity _Polarity = PulsePolarity.Positive;

        /// <summary>
        /// Gets the Name.
        /// </summary>
        public override String Name => TriggerType.Runt.ToString();

        /// <summary>
        /// Gets or sets the Polarity.
        /// </summary>
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
    }
}

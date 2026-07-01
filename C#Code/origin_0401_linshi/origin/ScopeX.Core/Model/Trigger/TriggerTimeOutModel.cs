// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    //高、低电平持续时间过长，或电平长时间不改变
    /// <summary>
    /// Defines the <see cref="TriggerTimeOutModel" />.
    /// </summary>
    internal class TriggerTimeOutModel : TriggerSingleSrcModel
    {
        /// <summary>
        /// Defines the _DurationByps.
        /// </summary>
        private Int64 _DurationByps = Constants.MIN_TIMEOUT_PS;

        /// <summary>
        /// Defines the _Polarity.
        /// </summary>
        private LevelPolarity _Polarity = LevelPolarity.Positive;

        /// <summary>
        /// Gets or sets the DurationByps.
        /// </summary>
        public Int64 DurationByps
        {
            get => _DurationByps;
            set
            {
                value = ValidateDuration(value);

                if (value != _DurationByps)
                {
                    _DurationByps = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the MaxDuration.
        /// </summary>
        public Int64 MaxDuration
        {
            get; init;
        } = Constants.MAX_TIMEOUT_PS;

        /// <summary>
        /// Gets the MinDuration.
        /// </summary>
        public Int64 MinDuration
        {
            get; init;
        } = Constants.MIN_TIMEOUT_PS;

        /// <summary>
        /// Gets the Name.
        /// </summary>
        public override String Name => TriggerType.TimeOut.ToString();

        /// <summary>
        /// Gets or sets the Polarity.
        /// </summary>
        public LevelPolarity Polarity
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

        /// <summary>
        /// Gets the StpDuration.
        /// </summary>
        public Int64 StpDuration
        {
            get; init;
        } = Constants.STP_TIMEOUT_PS;

        /// <summary>
        /// The ValidateDuration.
        /// </summary>
        /// <param name="value">The value<see cref="Int64"/>.</param>
        /// <returns>The <see cref="Int64"/>.</returns>
        protected virtual Int64 ValidateDuration(Int64 value)
        {
            //规范时间值的大小在最小精度内
            value = (Int64)Math.Round((Double)value / StpDuration, MidpointRounding.AwayFromZero) * StpDuration;
            if (value > MaxDuration)
            {
                value = MaxDuration;
            }
            else if (value < MinDuration)
            {
                value = MinDuration;
            }

            return value;
        }
    }
}

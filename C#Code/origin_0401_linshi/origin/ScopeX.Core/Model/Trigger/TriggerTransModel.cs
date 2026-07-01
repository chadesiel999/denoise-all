// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    //两个比较电平
    /// <summary>
    /// Defines the <see cref="TriggerTransModel" />.
    /// </summary>
    internal class TriggerTransModel : TriggerMultiLevelModel
    {
        /// <summary>
        /// Defines the _Slope.
        /// </summary>
        private EdgeSlope _Slope = EdgeSlope.Rise;

        /// <summary>
        /// Gets the Name.
        /// </summary>
        public override String Name => TriggerType.Transition.ToString();

        /// <summary>
        /// Gets or sets the Slope.
        /// </summary>
        public EdgeSlope Slope
        {
            get => _Slope;
            set
            {
                if (value != _Slope)
                {
                    _Slope = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}

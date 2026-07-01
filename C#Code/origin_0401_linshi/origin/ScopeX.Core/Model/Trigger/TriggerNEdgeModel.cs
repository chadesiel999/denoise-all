// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using NPOI.SS.Formula.Functions;
    using System;
    using System.Reflection.PortableExecutable;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    //高、低电平持续时间过长，或电平长时间不改变
    /// <summary>
    /// Defines the <see cref="TriggerNEdgeModel" />.
    /// </summary>
    internal class TriggerNEdgeModel : TriggerSingleSrcModel
    {
        /// <summary>
        /// Defines the _DurationByps.
        /// </summary>
        private Int64 _DurationByps = Constants.MIN_TIMEOUT_PS;

        /// <summary>
        /// Defines the _Polarity.
        /// </summary>
        private EdgeSlope _Polarity = EdgeSlope.Rise;

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
        public override String Name => TriggerType.NEdge.ToString();

        /// <summary>
        /// Gets or sets the Polarity.
        /// </summary>
        public EdgeSlope Polarity
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

        public Int32 MinEdgeNumber { get; private set; } = 1;

        public Int32 MaxEdgeNumber { get; private set; } = 65536;

        private Int32 _EdgeNumber = 1;

        public Int32 EdgeNumber
        {
            get => _EdgeNumber;
            set
            {
                value = ValidateEdgeNumber(value);
                if (value != _EdgeNumber)
                {
                    _EdgeNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 ValidateEdgeNumber(Int32 edgeNumber)
        {
            if (edgeNumber < MinEdgeNumber)
            {
                WeakTip.Default.Write("EdgeNumber", MsgTipId.LessthanMin, false, "", 1);
                edgeNumber = MinEdgeNumber;
            }
            else if (edgeNumber > MaxEdgeNumber)
            {
                WeakTip.Default.Write("EdgeNumber", MsgTipId.GreatethanMax, false, "", 1);
                edgeNumber = MaxEdgeNumber;
            }

            return edgeNumber;
        }

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

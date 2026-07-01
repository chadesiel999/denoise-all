// Copyright (c) UESTC. All Rights Reserved
// <author>QC</author>
// <date>2022/4/6</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    internal class FilterNode : DspNode
    {
        public readonly Double MaxBandwidth = 100E6;

        public readonly Double MaxCenterFreq = 100E6;

        public readonly Int32 MaxOrder = 1000;

        public readonly Double MaxRolloffFactor = 10;

        public readonly Double MinBandwidth = 1E3;

        public readonly Double MinCenterFreq = 1E6;

        public readonly Int32 MinOrder = 1;

        public readonly Double MinRolloffFactor = 0.1;

        public readonly Double StpBandwidth = Constants.STP_MIXER_FREQ;

        public readonly Double StpCenterFreq = Constants.STP_MIXER_FREQ;

        public readonly Double StpRolloffFactor = 1;

        private Double _Bandwidth = 1E3;

        private Double _CenterFreq = 1E6;

        private Int32 _Order = 1;

        private Double _RolloffFactor = 0.3;

        private VsaMeasureFilterTypeOpt _MeasureFilterType = VsaMeasureFilterTypeOpt.Gaussian;

        public FilterNode(Action<String>? onPropertyChanged = null) : base(VsaNodeTypeOpt.Filter)
        {
            OnPropertyChanged = onPropertyChanged;

            _Bandwidth = MinBandwidth;
            _CenterFreq = MinCenterFreq;
        }

        public Double Bandwidth
        {
            get => _Bandwidth;
            set
            {
                value = ValidateBandwidth(value);
                if (_Bandwidth != value)
                {
                    _Bandwidth = value;
                    OnPropertyChanged?.Invoke(nameof(Bandwidth));
                }
            }
        }

        public Double CenterFreq
        {
            get => _CenterFreq;
            set
            {
                value = ValidateCenterFreq(value);
                if (_CenterFreq != value)
                {
                    _CenterFreq = value;
                    OnPropertyChanged?.Invoke(nameof(CenterFreq));
                }
            }
        }

        public Int32 Order
        {
            get => _Order;
            set
            {
                value = ValidateOrder(value);
                if (_Order != value)
                {
                    _Order = value;
                    OnPropertyChanged?.Invoke(nameof(Order));
                }
            }
        }

        public Double RolloffFactor
        {
            get => _RolloffFactor;
            set
            {
                value = ValidateRolloff(value);
                if (_RolloffFactor != value)
                {
                    _RolloffFactor = value;
                    OnPropertyChanged?.Invoke(nameof(RolloffFactor));
                }
            }
        }

        public VsaMeasureFilterTypeOpt Type
        {
            get => _MeasureFilterType;
            set
            {
                if (_MeasureFilterType != value)
                {
                    _MeasureFilterType = value;
                    OnPropertyChanged?.Invoke(nameof(Type));
                }
            }
        }

        public override Double[] Process(Double[] buffer)
        {

            return buffer;
        }

        public override String ToString()
        {
            return "Filter";
        }

        private Double ValidateBandwidth(Double value)
        {
            value = Math.Round(value / StpBandwidth) * StpBandwidth;
            if (value > MaxBandwidth)
            {
                value = MaxBandwidth;
            }
            else if (value < MinBandwidth)
            {
                value = MinBandwidth;
            }

            return value;
        }

        private Double ValidateCenterFreq(Double value)
        {
            value = Math.Round(value / StpCenterFreq) * StpCenterFreq;
            if (value > MaxCenterFreq)
            {
                value = MaxCenterFreq;
            }
            else if (value < MinCenterFreq)
            {
                value = MinCenterFreq;
            }

            return value;
        }

        private Int32 ValidateOrder(Int32 value)
        {
            if (value > MaxOrder)
            {
                value = MaxOrder;
            }
            else if (value < MinOrder)
            {
                value = MinOrder;
            }

            return value;
        }

        private Double ValidateRolloff(Double value)
        {
            value = Math.Round(value / StpRolloffFactor) * StpRolloffFactor;
            if (value > MaxRolloffFactor)
            {
                value = MaxRolloffFactor;
            }
            else if (value < MinRolloffFactor)
            {
                value = MinRolloffFactor;
            }

            return value;
        }
    }
}

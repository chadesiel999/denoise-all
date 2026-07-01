using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    internal class EqualizerNode : DspNode
    {
        private Double _Gradient = 1E-3;

        public Double Gradient
        {
            get => _Gradient;
            set
            {
                value = ValidateGradient(value);
                if (_Gradient  != value)
                {
                    _Gradient = value;
                    OnPropertyChanged?.Invoke(nameof(Gradient));
                }
            }
        }

        public Double MaxGradient
        {
            get;
            init;
        } = 1;

        public Double MinGradient
        {
            get;
            init;
        } = 1E-6;

        private Double ValidateGradient(Double value)
        {
            value = Math.Round(value, 6, MidpointRounding.AwayFromZero);

            if (value > MaxGradient)
            {
                value = MaxGradient;
            }
            else if (value < MinGradient)
            {
                value = MinGradient;
            }

            return value;
        }

        private Int32 _SymLength = 10;

        public Int32 SymLength
        {
            get => _SymLength;
            set
            {
                value = ValidateSymLength(value);
                if (_SymLength != value)
                {
                    _SymLength = value;
                    OnPropertyChanged?.Invoke(nameof(SymLength));
                }
            }
        }

        public Int32 MaxSymLength
        {
            get;
            init;
        } = 1_000_000;

        public Int32 MinSymLength
        {
            get;
            init;
        } = 10;

        private Int32 ValidateSymLength(Int32 value)
        {
            if (value > MaxSymLength)
            {
                value = MaxSymLength;
            }
            else if (value < MinSymLength)
            {
                value = MinSymLength;
            }

            return value;
        }

        private Int32 _TapLength = 2;

        public Int32 TapLength
        {
            get => _TapLength;
            set
            {
                value = ValidateTap(value);
                if (_TapLength != value)
                {
                    _TapLength = value;
                    OnPropertyChanged?.Invoke(nameof(TapLength));
                }
            }
        }

        public Int32 MaxTapLength
        {
            get;
            init;
        } = 1_000;

        public Int32 MinTapLength
        {
            get;
            init;
        } = 2;

        private Int32 ValidateTap(Int32 value)
        {
            if (value > MaxTapLength)
            {
                value = MaxTapLength;
            }
            else if (value < MinTapLength)
            {
                value = MinTapLength;
            }

            return value;
        }


        public EqualizerNode(Action<String>? onPropertyChanged = null) : base(VsaNodeTypeOpt.Equalizer)
        {
            OnPropertyChanged = onPropertyChanged;
        }

        public override Double[] Process(Double[] buffer)
        {
            return buffer;
        }

        public override String ToString()
        {
            return "Equalizer";
        }
    }
}

// Copyright (c) UESTC. All Rights Reserved
// <author>QC</author>
// <date>2022/4/6</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    internal class PhaseEstNode : DspNode
    {
        private VsaPhaseEstOpt _PhaseEst = VsaPhaseEstOpt.Viterbi;

        public PhaseEstNode(Action<String>? onPropertyChanged = null) : base(VsaNodeTypeOpt.PhaseEst)
        {
            OnPropertyChanged = onPropertyChanged;
        }

        public VsaPhaseEstOpt PhaseEst
        {
            get => _PhaseEst;
            set
            {
                if (_PhaseEst != value)
                {
                    _PhaseEst = value;
                    OnPropertyChanged?.Invoke(nameof(PhaseEst));
                }
            }
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

        public readonly Int32 MaxSymLength = 1000;

        public readonly Int32 MinSymLength = 2;

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

        public override Double[] Process(Double[] buffer)
        {

            return buffer;
        }

        public override String ToString()
        {
            return "Phase Estimator";
        }
    }
}

// Copyright (c) UESTC. All Rights Reserved
// <author>QC</author>
// <date>2022/4/6</date>

namespace ScopeX.Core
{
    using System;

    internal class DCBlockNode : DspNode
    {
        private Boolean _IsAnalogDCBlock = true;

        public DCBlockNode(Action<String>? onPropertyChanged = null) : base(VsaNodeTypeOpt.DCBlock)
        {
            OnPropertyChanged = onPropertyChanged;
        }

        public Boolean IsAnalogDCBlock
        {
            get => _IsAnalogDCBlock;
            set
            {
                if (_IsAnalogDCBlock != value)
                {
                    _IsAnalogDCBlock = value;
                    OnPropertyChanged?.Invoke(nameof(IsAnalogDCBlock));
                }
            }
        }

        public override Double[] Process(Double[] buffer)
        {

            return buffer;
        }

        public override String ToString()
        {
            return "DC Block";
        }
    }
}

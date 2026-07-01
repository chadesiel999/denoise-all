// Copyright (c) UESTC. All Rights Reserved
// <author>QC</author>
// <date>2022/4/6</date>

namespace ScopeX.Core
{
    using System;

    internal class NoOpNode : DspNode
    {
        public override Double[] Process(Double[] buffer)
        {
            return buffer;
        }

        public override String ToString()
        {
            return "None";
        }

        public NoOpNode(Action<String>? onPropertyChanged = null) : base(VsaNodeTypeOpt.NoOp)
        {
            OnPropertyChanged = onPropertyChanged;
        }
    }
}

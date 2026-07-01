using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    internal class CustomNode : DspNode
    {
        private String _Code = "";
        public String Code
        {
            get => _Code;
            set
            {
                if (_Code == value)
                {
                    _Code = value;
                    OnPropertyChanged?.Invoke(nameof(Code));
                }
            }
        }

        public CustomNode(Action<String>? onPropertyChanged = null) : base(VsaNodeTypeOpt.Custom)
        {
            OnPropertyChanged = onPropertyChanged;
        }

        public override Double[] Process(Double[] buffer)
        {
            return buffer;
        }

        public override String ToString()
        {
            return "Matlab";
        }
    }
}

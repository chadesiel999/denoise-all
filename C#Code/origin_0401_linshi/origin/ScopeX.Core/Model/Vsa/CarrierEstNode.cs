using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    internal class CarrierEstNode : DspNode
    {
        public CarrierEstNode(Action<String>? onPropertyChanged = null) : base(VsaNodeTypeOpt.CarrierEst)
        {
            OnPropertyChanged = onPropertyChanged;
        }

        public override Double[] Process(Double[] buffer)
        {
            return buffer;
        }

        public override String ToString()
        {
            return "Carrier Estimator";
        }
    }
}

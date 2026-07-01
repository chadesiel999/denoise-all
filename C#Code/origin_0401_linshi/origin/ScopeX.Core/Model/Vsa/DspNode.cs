using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    internal abstract class DspNode
    {
        public abstract Double[] Process(Double[] buffer);

        public Action<String>? OnPropertyChanged { private protected get; set; }

        public VsaNodeTypeOpt NodeType { get; }

        public DspNode(VsaNodeTypeOpt type)
        {
            NodeType = type;
        }
    }

    public enum VsaNodeTypeOpt
    {
        NoOp,
        Mixer,
        Filter,
        Equalizer,
        DCBlock,
        CarrierEst,
        PhaseEst,
        Custom,
    }
}

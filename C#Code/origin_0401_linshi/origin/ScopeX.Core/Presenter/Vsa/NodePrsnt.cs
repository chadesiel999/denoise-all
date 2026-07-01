using System;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public abstract class DspNodePrsnt
    {
        private protected readonly DspNode _Model;

        internal virtual DspNode Model => _Model;

        public VectorAnalysisPrsnt Presenter
        {
            get;
        }

        internal DspNodePrsnt(VectorAnalysisPrsnt vap, DspNode node)
        {
            Presenter = vap;
            _Model = node;
        }

        public VsaNodeTypeOpt NodeType => Model.NodeType;

    }

    public class NoOpNodePrsnt : DspNodePrsnt
    {
        internal override NoOpNode Model => (NoOpNode)_Model;

        internal NoOpNodePrsnt(VectorAnalysisPrsnt vap, DspNode node) : base(vap, node)
        { }
    }

    public class MixerNodePrsnt : DspNodePrsnt
    {
        internal override MixerNode Model => (MixerNode)_Model;

        internal MixerNodePrsnt(VectorAnalysisPrsnt vap, MixerNode node) : base(vap, node)
        { }

        public Double SuggestedFreq
        {
            get => Model.SuggestedFreq;
            set => Model.SuggestedFreq = value;
        }

        public Double MaxSuggestedFreq => Model.MaxSuggestedFreq;

        public Double MinSuggestedFreq => Model.MinSuggestedFreq;
    }

    public class CarrierEstNodePrsnt : DspNodePrsnt
    {
        internal override CarrierEstNode Model => (CarrierEstNode)_Model;

        internal CarrierEstNodePrsnt(VectorAnalysisPrsnt vap, CarrierEstNode node) : base(vap, node)
        { }
    }

    public class PhaseEstNodePrsnt : DspNodePrsnt
    {
        internal override PhaseEstNode Model => (PhaseEstNode)_Model;

        internal PhaseEstNodePrsnt(VectorAnalysisPrsnt vap, PhaseEstNode node) : base(vap, node)
        { }

        public VsaPhaseEstOpt PhaseEst
        {
            get => Model.PhaseEst;
            set => Model.PhaseEst = value;
        }

        public Int32 SymLength
        {
            get => Model.SymLength;
            set => Model.SymLength = value;
        }

        public Int32 MaxSymLength => Model.MaxSymLength;

        public Int32 MinSymLength => Model.MinSymLength;
    }

    public class CustomNodePrsnt : DspNodePrsnt
    {
        internal override CustomNode Model => (CustomNode)_Model;

        internal CustomNodePrsnt(VectorAnalysisPrsnt vap, CustomNode node) : base(vap, node)
        { }

        public String Code
        {
            get => Model.Code;
            set => Model.Code = value;
        }
    }

    public class DCBlockNodePrsnt : DspNodePrsnt
    {
        internal override DCBlockNode Model => (DCBlockNode)_Model;

        internal DCBlockNodePrsnt(VectorAnalysisPrsnt vap, DCBlockNode node) : base(vap, node)
        { }

        public Boolean IsAnalogDCBlock
        {
            get => Model.IsAnalogDCBlock;
            set => Model.IsAnalogDCBlock = value;
        }
    }

    public class EqualizerNodePrsnt : DspNodePrsnt
    {
        internal override EqualizerNode Model => (EqualizerNode)_Model;

        internal EqualizerNodePrsnt(VectorAnalysisPrsnt vap, EqualizerNode node) : base(vap, node)
        { }

        public Double Gradient
        {
            get => Model.Gradient;
            set => Model.Gradient = value;
        }

        public Double MaxGradient => Model.MaxGradient;

        public Double MinGradient => Model.MinGradient;

        public Int32 SymLength
        {
            get => Model.SymLength;
            set => Model.SymLength = value;
        }

        public Int32 MaxSymLength => Model.MaxSymLength;

        public Int32 MinSymLength => Model.MinSymLength;

        public Int32 TapLength
        {
            get => Model.TapLength;
            set => Model.TapLength = value;
        }

        public Int32 MaxTapLength => Model.MaxTapLength;

        public Int32 MinTapLength => Model.MinTapLength;

    }

    public class FilterNodePrsnt : DspNodePrsnt
    {
        internal override FilterNode Model => (FilterNode)_Model;

        internal FilterNodePrsnt(VectorAnalysisPrsnt vap, FilterNode node) : base(vap, node)
        { }

        public Double Bandwidth
        {
            get => Model.Bandwidth;
            set => Model.Bandwidth = value;
        }

        public Double MaxBandwidth => Model.MaxBandwidth;

        public Double MinBandwidth => Model.MinBandwidth;

        public Double CenterFreq
        {
            get => Model.CenterFreq;
            set => Model.CenterFreq = value;
        }

        public Double MaxCenterFreq => Model.MaxCenterFreq;

        public Double MinCenterFreq => Model.MinCenterFreq;

        public Int32 Order
        {
            get => Model.Order;
            set => Model.Order = value;
        }

        public Int32 MaxOrder => Model.MaxOrder;

        public Int32 MinOrder => Model.MinOrder;

        public Double RolloffFactor
        {
            get => Model.RolloffFactor;
            set => Model.RolloffFactor = value;
        }

        public Double MaxRolloffFactor => Model.MaxRolloffFactor;

        public Double MinRolloffFactor => Model.MinRolloffFactor;

        public VsaMeasureFilterTypeOpt MeasureFilterType
        {
            get => Model.Type;
            set => Model.Type = value;
        }

    }
}

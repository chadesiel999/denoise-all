// Copyright (c) UESTC. All Rights Reserved
// <author>QC</author>
// <date>2022/4/11</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using ScopeX.ComModel;

    public class VectorAnalysisPrsnt : MulticastPrsnt<IVsaView>, IVsaPrsnt
    {
        public VectorAnalysisPrsnt(IDsoPrsnt idp, IVsaView? view, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.VectorAnalysisModel,
                ModelCreateOptions.Standalone => new(),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };
            GenerateDigtalPrsnt = new(idp);
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public VsaSignalType SignalType { get => Model.SignalType; set => Model.SignalType = value; }

        public Int32 BitsPerSym { get => Model.BitsPerSym; set => Model.BitsPerSym = value; }

        public Boolean Enabled { get => Model.Enabled; set => Model.Enabled = value; }

        public VsaItplOpt Interpolation { get => Model.Interpolation; set => Model.Interpolation = value; }

        public Int32 MaxBitsPerSym => Model.MaxBitsPerSym;

        public Int32 MaxSampPerBaud => Model.MaxSampPerBaud;

        public Double MaxSymbolRate => Model.MaxSymbolRate;

        public Int32 MinBitsPerSym => Model.MinBitsPerSym;

        public Int32 MinSampPerBaud => Model.MinSampPerBaud;

        public Double MinSymbolRate => Model.MinSymbolRate;

        public VsaFormatOpt Format { get => Model.Format; set => Model.Format = value; }

        public Int32 SampPerBaud { get => Model.SampPerBaud; set => Model.SampPerBaud = value; }

        public ChannelId Source { get => Model.Source; set => Model.Source = value; }

        public ChannelId Source2nd { get => Model.Source2nd; set => Model.Source2nd = value; }

        public Double SymbolRate { get => Model.SymbolRate; set => Model.SymbolRate = value; }

        public VsaTemplateOpt Template { get => Model.Template; set => Model.Template = value; }

        public VsaTimingEstOpt TimingEst { get => Model.TimingEst; set => Model.TimingEst = value; }

        private protected override VectorAnalysisModel Model { get; }

        public GenerateDigtalPrsnt GenerateDigtalPrsnt
        {
            get; set;
        }

        private DspNodePrsnt MakeNode(VsaNodeTypeOpt type)
        {
            return type switch
            {
                VsaNodeTypeOpt.Mixer => new MixerNodePrsnt(this, Model.MakeMixerNode()),
                VsaNodeTypeOpt.Filter => new FilterNodePrsnt(this, Model.MakeFilterNode()),
                VsaNodeTypeOpt.Equalizer => new EqualizerNodePrsnt(this, Model.MakeEqualizerNode()),
                VsaNodeTypeOpt.DCBlock => new DCBlockNodePrsnt(this, Model.MakeDCBlockNode()),
                VsaNodeTypeOpt.CarrierEst => new CarrierEstNodePrsnt(this, Model.MakeCarrierEstNode()),
                VsaNodeTypeOpt.PhaseEst => new PhaseEstNodePrsnt(this, Model.MakePhaseEstNode()),
                VsaNodeTypeOpt.Custom => new CustomNodePrsnt(this, Model.MakeCustomNode()),
                _ => new NoOpNodePrsnt(this, Model.MakeNoOpNode()),
            };
        }

        public DspNodePrsnt? GetNode(Int32 index)
        {
            var node = Model.GetNode(index);
            return node?.NodeType switch
            {
                VsaNodeTypeOpt.NoOp => new NoOpNodePrsnt(this, (NoOpNode)node),
                VsaNodeTypeOpt.Mixer => new MixerNodePrsnt(this, (MixerNode)node),
                VsaNodeTypeOpt.Filter => new FilterNodePrsnt(this, (FilterNode)node),
                VsaNodeTypeOpt.Equalizer => new EqualizerNodePrsnt(this, (EqualizerNode)node),
                VsaNodeTypeOpt.DCBlock => new DCBlockNodePrsnt(this, (DCBlockNode)node),
                VsaNodeTypeOpt.CarrierEst => new CarrierEstNodePrsnt(this, (CarrierEstNode)node),
                VsaNodeTypeOpt.PhaseEst => new PhaseEstNodePrsnt(this, (PhaseEstNode)node),
                VsaNodeTypeOpt.Custom => new CustomNodePrsnt(this, (CustomNode)node),
                _ => null,
            };
        }

        public DspNodePrsnt? SetCustomNode(Int32 index, VsaNodeTypeOpt type)
        {
            var node = Model.GetNode(index);
            if (node?.NodeType != type)
            {
                var dnp = MakeNode(type);
                Model.SetNode(index, dnp.Model);
                return dnp;
            }
            return null;
        }

        public DspNodePrsnt? AddCustomNode(VsaNodeTypeOpt type = VsaNodeTypeOpt.NoOp)
        {
            if (Model.Count < 8)
            {
                var dnp = MakeNode(type);
                Model.AddNode(dnp.Model);
                return dnp;
            }

            return null;
        }

        public void RemoveCustomNodeAt(Int32 index) => Model.RemoveNodeAt(index);
    }
}

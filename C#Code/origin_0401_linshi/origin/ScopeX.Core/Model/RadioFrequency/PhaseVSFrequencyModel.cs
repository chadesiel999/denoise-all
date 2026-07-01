using System;
using System.Drawing;
using System.Threading;
using ScopeX.ComModel;

namespace ScopeX.Core
{ 
    internal class PhaseVSFrequencyModel : ChannelModel
    {
        public PhaseVSFrequencyModel(ChannelId id, Color color, Boolean active, FrequencyModel frequencyModel)
               : base(ChannelType.PhaseVSFrequency, id, color)
        {
            Active = active;

            Conditioning = new PhaseModel();
            Sampling = frequencyModel;
        }

        public override PhaseModel Conditioning
        {
            get;
        }

        public override FrequencyModel Sampling
        {
            get;
        }
        private ChannelId _Source = ChannelId.C1;
        public ChannelId Source
        {
            get => _Source;
            set
            {
                if (value != _Source)
                {
                    _Source = value;
                    Sampling.Source = value;
                    OnPropertyChanged();
                }
            }
        }
        public override Boolean Active
        {
            get => base.Active;
            set
            {
                base.Active = value;
                OnPropertyChanged("PhaseVSFrequencyActive");
            }
        }
        public Func<Object,MDVirticalType,RFWaveType, WfmVuBlock?>? MakeVuSamplesIQFFT
        {
            get;
            set;
        }
        public override Boolean Take(Boolean init, CancellationToken ct, CancellationToken? softResetToken = null)
        {
            var args = PrepareSamples?.Invoke(init, Id, ct);
            var buffer = ReadSamples?.Invoke(args);//采样值
            if (buffer != null)
            {
                Pack = ProcessSamples?.Invoke(((Double[,], Object))buffer, args);

                //VuBuffer.Add(MakeVuSamples?.Invoke(this));
                return true;
            }
            return false;
        }
    }
}

using System;
using System.Drawing;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
namespace ScopeX.Core
{
    internal class AmpVSTimeModel : ChannelModel
    {
        public AmpVSTimeModel(ChannelId id, Color color, Boolean active, TimebaseModel timeModelH)
               : base(ChannelType.AmpVSTime, id, color)
        {
            Active = active;

            Conditioning = new AmplitudeModel()
            {
                Unit = "V",
                UnitType = AmplitudeUnitType.Linear,
                FigureCenterAmplitude = 0,
                AmpScale = 0.250,
            };
            Sampling = timeModelH;
        }

        public override AmplitudeModel Conditioning
        {
            get;
        }

        public override TimebaseModel Sampling
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
                    //Sampling.Source = value;
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
                OnPropertyChanged("AmpVSTimeActive");
            }
        }
       
        public Func<Object, MDVirticalType, WfmVuBlock?>? MakeVuSamplesIQ
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

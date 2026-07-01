using System;
using System.Drawing;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
namespace ScopeX.Core
{
    internal class FrequencyVSTimeModel : ChannelModel
    {
        public FrequencyVSTimeModel(ChannelId id, Color color, Boolean active, TimebaseModel timeModelH)
               : base(ChannelType.FrequencyVSTime, id, color)
        {
            Active = active;

            Conditioning = new FrequencyModelV()
            {
                Unit = "H",
            };
            Sampling = timeModelH;
        }

        public override FrequencyModelV Conditioning
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
                OnPropertyChanged("FrequencyVSTimeActive");
            }
        }

        public Func<Object,MDVirticalType, WfmVuBlock?>? MakeVuSamplesIQ
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

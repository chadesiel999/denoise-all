using System;
using System.Drawing;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    internal class PhaseVSTimeModel : ChannelModel
    {
        public PhaseVSTimeModel(ChannelId id, Color color, Boolean active, TimebaseModel timeModelH)
               : base(ChannelType.PhaseVSTime, id, color)
        {
            Active = active;

            Conditioning = new PhaseModel();
            Sampling = timeModelH;
        }

        public override PhaseModel Conditioning
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
                OnPropertyChanged("PhaseVSTimeActive");
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
        private Int32 _FFTLength = (Int32)Math.Pow(2, Constants.RF_FFT_POWER_MAX);
        public Int32 FFTLength
        {
            get
            {
                return _FFTLength;
            }
            set
            {
                if (_FFTLength != value)
                {
                    _FFTLength = ValidFFTLength(value);
                    OnPropertyChanged();
                }
            }
        }
        private Int32 ValidFFTLength(Int32 value)
        {
            Int32 minpower = Constants.RF_FFT_POWER_MIN;
            Int32 maxpower = Constants.RF_FFT_POWER_MAX;

            for (Int32 i = minpower; i < maxpower + 1; i++)
            {
                if (value >= Math.Pow(2, i) && value < Math.Pow(2, i + 1))
                {
                    return (Int32)Math.Pow(2, i);
                }
            }

            if (value < Math.Pow(2, minpower))
                return (Int32)Math.Pow(2, minpower);
            else
                return (Int32)Math.Pow(2, maxpower);
        }

    }
}

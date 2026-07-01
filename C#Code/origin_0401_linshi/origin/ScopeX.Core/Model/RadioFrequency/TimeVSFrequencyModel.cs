using System;
using System.Drawing;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    internal class TimeVSFrequencyModel : ChannelModel
    {
        public TimeVSFrequencyModel(ChannelId id, Color color, Boolean active, FrequencyModel frequencyModel)
               : base(ChannelType.TimeVSFrequency, id, color)
        {
            Active = active;

            Sampling = frequencyModel;
            Conditioning = new TimeModelV()
            {
                Prefix = Prefix.Micro,
            };
        }

        public override FrequencyModel Sampling
        {
            get;
        }

        public override TimeModelV Conditioning
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
                OnPropertyChanged("TimeVSFrequencyActive");
            }
        }

        /// <summary>
        /// 所有频谱数据，用于时频图、光谱、三维显示
        /// </summary>
        public HistoryData TFHistoryData = new HistoryData();

        private Boolean _RecordHistoricalData = false;
        public Boolean RecordHistoricalData
        {
            get => Active;
            set
            {
                if (value != _RecordHistoricalData)
                {
                    TFHistoryData.Clear();
                    _RecordHistoricalData = value;
                }
            }
        }
        private Boolean _ThreeD = false;
        public Boolean ThreeD
        {
            get => _ThreeD;
            set
            {
                if (value != _ThreeD)
                {
                    _ThreeD = value;
                    OnPropertyChanged();
                }
            }
        }

        public Func<Object, MDVirticalType, RFWaveType,WfmVuBlock?>? MakeVuSamplesIQFFT
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

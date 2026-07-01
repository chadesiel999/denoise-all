using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    //Ratio the power in the fundamental frequency to the power contained in the rest of the harmonics and noise
    //THD=Math.Squrt(math.Pow((G5/G1),2)+math.Pow((G4/G1),2)+math.Pow((G3/G1),2)+math.Pow((G2/G1),2));//Gn第n次谐波的功率
    public class TotalHarmonicDistortion : IFrequencyMeasure, INotifyPropertyChanged
    {
        internal TotalHarmonicDistortion(ChannelId id)
        {
            _ChannelId = id;
        }

        //public static readonly TotalHarmonicDistortion Default = new TotalHarmonicDistortion();

        private Boolean _Enable = false;
        public Boolean Enable
        {
            get { return _Enable; }
            set
            {
                if (value != _Enable)
                {
                    _Enable = value;
                }
            }
        }

        private ChannelId _ChannelId = ChannelId.M1;
        public ChannelId ChannelId
        {
            get { return _ChannelId; }
            set
            {
                if (value != _ChannelId && value.IsMath())
                {
                    _ChannelId = value;
                    GetMeasGates();
                    OnPropertyChanged();
                }
            }
        }

        private Double _ChannelSpan = Constants.RF_MEASURE_MIN_THD_CHANNEL_SPAN;
        public Double ChannelSpan 
        {
            get { return _ChannelSpan; }
            set
            {
                if (value != _ChannelSpan)
                {
                    value = value > Constants.RF_MEASURE_MAX_THD_CHANNEL_SPAN ? Constants.RF_MEASURE_MAX_THD_CHANNEL_SPAN : value;
                    value = value < Constants.RF_MEASURE_MIN_THD_CHANNEL_SPAN ? Constants.RF_MEASURE_MIN_THD_CHANNEL_SPAN : value;
                    _ChannelSpan = value;
                    GetMeasGates();
                    OnPropertyChanged();
                }
            }
        }

        private Double _ChannelSpacing = Constants.RF_MEASURE_MIN_THD_CHANNEL_SPACING;
        public Double ChannelSpacing 
        {
            get { return _ChannelSpacing; }
            set
            {
                if (value != _ChannelSpacing)
                {
                    value = value > Constants.RF_MEASURE_MAX_THD_CHANNEL_SPACING ? Constants.RF_MEASURE_MAX_THD_CHANNEL_SPACING : value;
                    value = value < Constants.RF_MEASURE_MIN_THD_CHANNEL_SPACING ? Constants.RF_MEASURE_MIN_THD_CHANNEL_SPACING : value;
                    _ChannelSpacing = value;
                    GetMeasGates();
                    OnPropertyChanged();
                }
            }
        }
        //public Double ChannelSpan { get; set; } = 4_000_000;//不可更改
        //public Double ChannelSpacing { get; set; } = 5_000_000;//不可更改

        private Int32 _ChannelCount = Constants.RF_MEASURE_MIN_THD_CHANNEL_COUNT;
        /// <summary>
        /// ChannelCount   min=3  max=5  ，中1  右2      中1  右2
        /// </summary>
        public Int32 ChannelCount
        {
            get { return _ChannelCount; }
            set
            {
                if (value != _ChannelCount)
                {
                    value = value > Constants.RF_MEASURE_MAX_THD_CHANNEL_COUNT ? Constants.RF_MEASURE_MAX_THD_CHANNEL_COUNT : value;
                    value = value < Constants.RF_MEASURE_MIN_THD_CHANNEL_COUNT ? Constants.RF_MEASURE_MIN_THD_CHANNEL_COUNT : value;
                    _ChannelCount = value;
                    GetMeasGates();
                    OnPropertyChanged();
                }
            }
        }

        private List<(Single xMin, Single xMax)> _MeasGates = new();
        public List<(Single xMin, Single xMax)> MeasGates
        {
            get
            {
                if (_Enable)
                {
                    return _MeasGates;
                }
                return new();
            }
        }

        private Boolean GetMeasGates()
        {
            if (FrequencyMeasurement.GetMeasGates(ChannelId, ChannelSpan, ChannelSpacing, ChannelCount, out var measuregates, true))
            {
                _MeasGates = measuregates;
                return true;
            }
            else { return false; }
        }

        private List<(Single Position, Single Result, String ResultUnitString)> _MeasResults = new();
        public List<(Single Position, Single Result, String ResultUnitString)> MeasResults
        {
            get
            {
                if (_Enable)
                {
                    return _MeasResults;
                }
                return new();
            }
        }

        private Boolean GetMeasResults(List<Double> result, Prefix prefix,String mainUnit, String sideUnit)
        {
            if (FrequencyMeasurement.GetMeasResultsPositions(result, prefix, mainUnit, sideUnit, ChannelId, ChannelSpan, ChannelSpacing, ChannelCount, out var measureresults, true, true))
            {
                _MeasResults = measureresults;
                return true;
            }
            else { return false; }
        }
        internal Double GetTotalHarmonicDistortion(WfmPack pack, Double span, Double spacing, Int32 channelCount, MathModel mathModel, MathFftArg fftArg)
        {
            if (!(pack.Buffer.Length > 0))
            {
                return Double.NaN;
            }

            var bufferarray = pack.Buffer.GetRow(0);
            var maxfreq = pack.Properties.SampInterval * bufferarray.Length;
            Double minchannelfreq, maxchannelfreq;
            var freqres = pack.Properties.SampInterval;

            Double[] channelpowers = new Double[1+channelCount];
            for (Int32 i = 0; i < 1+channelCount; i++)//只算从左到右
            {
                minchannelfreq = mathModel.FrequencyAdapter.ValueCenter - span / 2 + spacing * i;
                minchannelfreq /= 1_000_000;
                minchannelfreq = minchannelfreq < 0 ? 0 : minchannelfreq;
                minchannelfreq = minchannelfreq > maxfreq ? maxfreq : minchannelfreq;

                maxchannelfreq = mathModel.FrequencyAdapter.ValueCenter + span / 2 + spacing * i;
                maxchannelfreq /= 1_000_000;
                maxchannelfreq = maxchannelfreq > maxfreq ? maxfreq : maxchannelfreq;
                maxchannelfreq = maxchannelfreq < 0 ? 0 : maxchannelfreq;

                channelpowers[i] = FrequencyMeasurement.GetMaxPower(bufferarray, minchannelfreq, maxchannelfreq, freqres, fftArg.ResultUnit);
            }

            //THD=Math.Squrt(math.Pow((G5/G1),2)+math.Pow((G4/G1),2)+math.Pow((G3/G1),2)+math.Pow((G2/G1),2));//Gn第n次谐波的功率
            Double total = 0;
            for (Int32 i = 1; i < channelCount; i++)
            {
                total += Math.Pow(10,channelpowers[i]/10);
            }
            total /= Math.Pow(10,channelpowers[0]/10);
            Double thd = Math.Sqrt(total);
            return thd;
        }
        public Double? Run()
        {
            if (!Enable)
            {
                return null;
            }
            ChannelId channelid = _ChannelId;
            if (!FrequencyMeasurement.IsCorrectChannelState(channelid, out var mathmodel, out var fftarg))
            {
                return null;
            }
            var pack = DsoModel.Default.GetWfmPack(channelid);
            if (pack != null)
            {
                GetMeasGates();
                var result = GetTotalHarmonicDistortion(pack, ChannelSpan, ChannelSpacing, ChannelCount, mathmodel, fftarg);
                GetMeasResults(new List<Double>() { result }, Prefix.Empty, "%","");

                return result;
            }

            return null;
        }

        #region INotifyPropertyChanged
        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

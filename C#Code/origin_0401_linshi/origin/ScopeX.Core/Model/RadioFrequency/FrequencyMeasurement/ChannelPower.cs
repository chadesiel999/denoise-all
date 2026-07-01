using NPOI.Util;
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
    //Power across one frequency range
    //channelPower=AveragePower+log(pointsInChannel)
    public class ChannelPower : IFrequencyMeasure,INotifyPropertyChanged
    {
        internal ChannelPower(ChannelId id)
        {
            _ChannelId = id;
        }

        //public static readonly ChannelPower Default = new ChannelPower();
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
        //public Double ChannelSpan { get; set; } = 5_000_000;

        private Double _ChannelSpan = Constants.RF_MEASURE_MIN_CP_CHANNEL_SPAN;
        public Double ChannelSpan
        {
            get { return _ChannelSpan; }
            set
            {
                if (value != _ChannelSpan)
                {
                    value = value > Constants.RF_MEASURE_MAX_CP_CHANNEL_SPAN ? Constants.RF_MEASURE_MAX_CP_CHANNEL_SPAN : value;
                    value = value < Constants.RF_MEASURE_MIN_CP_CHANNEL_SPAN ? Constants.RF_MEASURE_MIN_CP_CHANNEL_SPAN : value;
                    _ChannelSpan = value;
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
            if (FrequencyMeasurement.GetMeasGates(ChannelId, ChannelSpan, 0, 0, out var measuregates))
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

        private Boolean GetMeasResults(List<Double> result, Prefix prefix, String mainUnit, String sideUnit)
        {
            if (FrequencyMeasurement.GetMeasResultsPositions(result, prefix, mainUnit, sideUnit, ChannelId, ChannelSpan, 0, 0, out var measureresults, false, false))
            {
                _MeasResults = measureresults;
                return true;
            }
            else { return false; }
        }

        internal Double GetChannelPower(WfmPack pack,Double span,MathModel mathModel,MathFftArg fftArg)
        {
            var bufferarray = pack.Buffer.GetRow(0);
            var maxfreq = pack.Properties.SampInterval * bufferarray.Length;
            var minchannelfreq = mathModel.FrequencyAdapter.ValueCenter - span / 2;
            minchannelfreq /= 1_000_000;
            minchannelfreq = minchannelfreq < 0 ? 0 : minchannelfreq;
            minchannelfreq = minchannelfreq > maxfreq ? maxfreq : minchannelfreq;


            var maxchannelfreq = mathModel.FrequencyAdapter.ValueCenter + span / 2;
            maxchannelfreq /= 1_000_000;
            maxchannelfreq = maxchannelfreq > maxfreq ? maxfreq : maxchannelfreq;
            maxchannelfreq = maxchannelfreq < 0 ? 0 : maxchannelfreq;

            var freqres = pack.Properties.SampInterval ;// 1 / (pack.Properties.SampInterval * pack.Buffer.ToRowEnumerable().Count());
            Double channelpower = Double.NaN;

            channelpower = FrequencyMeasurement.GetChannelPower(bufferarray, minchannelfreq, maxchannelfreq, freqres, fftArg.ResultUnit);
            return channelpower;
        }
       

        public Double? Run()
        {
            if (!Enable)
            {
                return null;
            }
            ChannelId channelId = _ChannelId;
            if (!FrequencyMeasurement.IsCorrectChannelState(channelId,out var mathmodel, out var fftarg))
            {
                return null;
            }
            var pack = DsoModel.Default.GetWfmPack(channelId);
            if (pack != null)
            {
                GetMeasGates();
                var result = GetChannelPower(pack, ChannelSpan, mathmodel, fftarg);
                GetMeasResults(new List<Double>() { result }, Prefix.Empty, fftarg.ResultUnit.ToString(), fftarg.ResultUnit.ToString());

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

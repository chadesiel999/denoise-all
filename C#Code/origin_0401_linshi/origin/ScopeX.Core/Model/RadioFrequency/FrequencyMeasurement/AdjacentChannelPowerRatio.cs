using NPOI.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    //Ratio the power in the main frequency range to the power contained in one or more sidebands
    //ACPR = P adjacent / P main ,邻信道的功率、主信道的功率  结果为数组
    public class AdjacentChannelPowerRatio: IFrequencyMeasure, INotifyPropertyChanged
    {
        internal AdjacentChannelPowerRatio(ChannelId id) 
        {
            _ChannelId = id;
        }

        //public static readonly AdjacentChannelPowerRatio Default = new AdjacentChannelPowerRatio();

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

        private ChannelId _ChannelId;
        public ChannelId ChannelId 
        {
            get { return _ChannelId; }
            set {
                if (value!= _ChannelId && value.IsMath())
                {
                    _ChannelId = value; 
                    GetMeasGates();
                    OnPropertyChanged();
                }
            } 
        } 


        private Double _ChannelSpan = Constants.RF_MEASURE_MIN_ACPR_CHANNEL_SPAN;
        public Double ChannelSpan
        {
            get { return _ChannelSpan; }
            set
            {
                if (value != _ChannelSpan)
                {
                    value = value > Constants.RF_MEASURE_MAX_ACPR_CHANNEL_SPAN ? Constants.RF_MEASURE_MAX_ACPR_CHANNEL_SPAN : value;
                    value = value < Constants.RF_MEASURE_MIN_ACPR_CHANNEL_SPAN ? Constants.RF_MEASURE_MIN_ACPR_CHANNEL_SPAN : value;
                    _ChannelSpan = value;
                    GetMeasGates();
                    OnPropertyChanged();
                }
            }
        }

        private Double _ChannelSpacing = Constants.RF_MEASURE_MIN_ACPR_CHANNEL_SPACING;
        public Double ChannelSpacing
        {
            get { return _ChannelSpacing; }
            set
            {
                if (value != _ChannelSpacing)
                {
                    value = value > Constants.RF_MEASURE_MAX_ACPR_CHANNEL_SPACING ? Constants.RF_MEASURE_MAX_ACPR_CHANNEL_SPACING : value;
                    value = value < Constants.RF_MEASURE_MIN_ACPR_CHANNEL_SPACING ? Constants.RF_MEASURE_MIN_ACPR_CHANNEL_SPACING : value;
                    _ChannelSpacing = value;
                    GetMeasGates();
                    OnPropertyChanged();
                }
            }
        }
        //public Double ChannelSpan { get; set; } = 4_000_000;//不可更改
        //public Double ChannelSpacing { get; set; } = 5_000_000;//不可更改

        private Int32 _ChannelCount = Constants.RF_MEASURE_MIN_ACPR_CHANNEL_COUNT;
        /// <summary>
        /// ChannelCount   min=1  max=5  ，中1 左1 右1      中1 左5 右5
        /// </summary>
        public Int32 ChannelCount
        {
            get { return _ChannelCount; }
            set {
                if (value !=_ChannelCount)
                {
                    value = value > Constants.RF_MEASURE_MAX_ACPR_CHANNEL_COUNT ? Constants.RF_MEASURE_MAX_ACPR_CHANNEL_COUNT : value;
                    value = value < Constants.RF_MEASURE_MIN_ACPR_CHANNEL_COUNT ? Constants.RF_MEASURE_MIN_ACPR_CHANNEL_COUNT : value;
                    _ChannelCount = value; 
                    GetMeasGates();
                    OnPropertyChanged();
                }
            }
        }

        private List<(Single xMin, Single xMax)> _MeasGates = new();
        public List<(Single xMin, Single xMax)> MeasGates 
        {
            get {
                if (_Enable)
                {
                    return _MeasGates;
                }
                return new();
            }
        }

        private Boolean GetMeasGates()
        {
            if (FrequencyMeasurement.GetMeasGates(ChannelId, ChannelSpan, ChannelSpacing, ChannelCount, out var measureGates))
            {
                _MeasGates = measureGates;
                return true;
            }
            else { return false; }
        }
        private List<(Single Position, Single Result, String ResultUnitString)> _MeasResults = new();
        public List<(Single Position, Single Result, String ResultUnitString)> MeasResults
        {
            get {
                if (_Enable)
                {
                    return _MeasResults;
                }
                return new();
            }
        }


        private Boolean GetMeasResults(List<Double> result, Prefix prefix, String mainUnit, String sideUnit)
        {
            if (FrequencyMeasurement.GetMeasResultsPositions(result, prefix, mainUnit, sideUnit, ChannelId, ChannelSpan, ChannelSpacing, ChannelCount, out var measureResults,false,false))
            {
                _MeasResults = measureResults;
                return true;
            }
            else { return false; }
        }
        internal Double[] GetAdjacentChannelPowerRatio(WfmPack pack, Double span, Double spacing,Int32 channelCount, MathModel mathModel, MathFftArg fftArg)
        {
            Int32 totalnums = channelCount * 2 + 1;

            //span /= 1_000_000;
            //spacing /= 1_000_000;
            if (!(pack.Buffer.Length>0))
            {
                Double[] nans = new Double[totalnums];
                for (Int32 i = 0; i < nans.Length; i++)
                {
                    nans[i] = Double.NaN;
                }
                return nans;
            }

            var bufferarray = pack.Buffer.GetRow(0);
            var maxFreq = pack.Properties.SampInterval * bufferarray.Length;
            Double minChannelFreq, maxChannelFreq;
            var freqres = pack.Properties.SampInterval;// 1 / (pack.Properties.SampInterval * pack.Buffer.ToRowEnumerable().Count());

            Double[] channelpowers = new Double[totalnums];
            for (Int32 i = -channelCount; i < channelCount + 1; i++)
            {
                minChannelFreq = mathModel.FrequencyAdapter.ValueCenter - span / 2 + spacing * i;
                minChannelFreq /= 1_000_000;
                minChannelFreq = minChannelFreq < 0 ? 0 : minChannelFreq;
                minChannelFreq = minChannelFreq > maxFreq ? maxFreq : minChannelFreq;

                maxChannelFreq = mathModel.FrequencyAdapter.ValueCenter + span / 2 + spacing * i;
                maxChannelFreq /= 1_000_000;
                maxChannelFreq = maxChannelFreq > maxFreq ? maxFreq : maxChannelFreq;
                maxChannelFreq = maxChannelFreq < 0 ? 0 : maxChannelFreq;

                channelpowers[i + channelCount] = FrequencyMeasurement.GetChannelPower(bufferarray, minChannelFreq, maxChannelFreq, freqres, fftArg.ResultUnit);
            }
            Double[] ratios = new Double[totalnums];
            Double mainPower = channelpowers[channelCount];
            for (Int32 i = 0; i < channelpowers.Length; i++)
            {
                if (i == channelCount)
                {
                    ratios[i] = mainPower;
                }
                else
                {
                    ratios[i] = channelpowers[i] - mainPower;
                }
            }

            return ratios;
        }

        public Double[]? Run()
        {
            if (!Enable)
            {
                return null;
            }
            ChannelId channelId=_ChannelId;
            if (!FrequencyMeasurement.IsCorrectChannelState(channelId, out var mathModel, out var fftArg))
            {
                return null;
            }
            var pack = DsoModel.Default.GetWfmPack(channelId);
            if (pack != null)
            {
                GetMeasGates();
                Double[] result = GetAdjacentChannelPowerRatio(pack, ChannelSpan, ChannelSpacing, ChannelCount,mathModel, fftArg);
                GetMeasResults(result.ToList(), Prefix.Empty, QuantityUnit.Constant.ToUnitString(),QuantityUnit.Decibel.ToUnitString());

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

using NPOI.SS.Formula.Functions;
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
using static ScopeX.Core.MathFftArg;

namespace ScopeX.Core
{
    //Percentage of the total power, centered on an assigned channel frequency as specified by user
    public class OccupiedBandwidth : IFrequencyMeasure, INotifyPropertyChanged
    {
        internal OccupiedBandwidth(ChannelId id)
        {
            _ChannelId = id;
        }

        //public static readonly OccupiedBandwidth Default = new OccupiedBandwidth();

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
        private Double _ChannelSpan = Constants.RF_MEASURE_MIN_OB_CHANNEL_SPAN;
        public Double ChannelSpan
        {
            get { return _ChannelSpan; }
            set
            {
                if (value != _ChannelSpan)
                {
                    value = value > Constants.RF_MEASURE_MAX_OB_CHANNEL_SPAN ? Constants.RF_MEASURE_MAX_OB_CHANNEL_SPAN : value;
                    value = value < Constants.RF_MEASURE_MIN_OB_CHANNEL_SPAN ? Constants.RF_MEASURE_MIN_OB_CHANNEL_SPAN : value;
                    _ChannelSpan = value;
                    GetMeasGates();
                    OnPropertyChanged();
                }
            }
        }

        private Double _Percentage = Constants.RF_MEASURE_DEFAULT_OB_PERCENTAGE;
        /// <summary>
        /// 从总功率totalPower的50% 处往左Percentage/2*totalPower 往右Percentage/2*totalPower的 最小频率 minFreq和最大频率 maxFreq的差值 OBW = maxFreq - minFreq,
        /// </summary>
        public Double Percentage
        {
            get { return _Percentage; }
            set
            {
                if (value != _Percentage)
                {
                    value = value > Constants.RF_MEASURE_MAX_OB_PERCENTAGE ? Constants.RF_MEASURE_MAX_OB_PERCENTAGE : value;
                    value = value < Constants.RF_MEASURE_MIN_OB_PERCENTAGE ? Constants.RF_MEASURE_MIN_OB_PERCENTAGE : value;
                    _Percentage = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _dBDown = Constants.RF_MEASURE_MAX_OB_DB_DOWN;
        /// <summary>
        /// 最大-1，以1为步进，从功率最高下降到指定dBDown功率的，在maxdB+dBDowm处  最小频率 minFreq和最大频率 maxFreq的差值 OBW = maxFreq - minFreq,不为dB单位时OBW的测量结果总为0
        /// </summary>
        public Int32 dBDown
        {
            get { return _dBDown; }
            set
            {
                if (value != _dBDown)
                {
                    value = value > Constants.RF_MEASURE_MAX_OB_DB_DOWN ? Constants.RF_MEASURE_MAX_OB_DB_DOWN : value;
                    value = value < Constants.RF_MEASURE_MIN_OB_DB_DOWN ? Constants.RF_MEASURE_MIN_OB_DB_DOWN : value;
                    _dBDown = value;
                    OnPropertyChanged();
                }
            }
        }

        private OBWAnalysisType _AnalysisType = OBWAnalysisType.Percentage;
        public OBWAnalysisType AnalysisType
        {
            get { return _AnalysisType; }
            set {
                if (value!= _AnalysisType)
                {
                    _AnalysisType = value;
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
            if (FrequencyMeasurement.GetMeasGates(ChannelId, ChannelSpan, 0, 0, out var measureGates))
            {
                _MeasGates = measureGates;
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

        private Boolean GetMeasResults(List<Double> result,Prefix prefix, String mainUnit, String sideUnit)
        {
            if (FrequencyMeasurement.GetMeasResultsPositions(result, prefix, mainUnit, sideUnit, ChannelId, ChannelSpan, 0, 0, out var measureResults,false, true))
            {
                _MeasResults = measureResults;
                return true;
            }
            else { return false; }
        }


        internal Double GetOccupiedBandwidth(WfmPack pack, Double span,Double percentage,Int32 dbdown,OBWAnalysisType analysisType, MathModel mathModel, MathFftArg fftArg)
        {
            if (fftArg.ResultUnit == FFTCoordUnit.Vrms) 
            {
                return 0;
            }

            var bufferarray = pack.Buffer.GetRow(0);
            var maxfreq = pack.Properties.SampInterval * bufferarray.Length ;
            var minchannelfreq = mathModel.FrequencyAdapter.ValueCenter - span / 2;
            minchannelfreq /= 1_000_000;
            minchannelfreq = minchannelfreq < 0 ? 0 : minchannelfreq;
            var maxChannelFreq = mathModel.FrequencyAdapter.ValueCenter + span / 2;
            maxChannelFreq /= 1_000_000;
            maxChannelFreq = maxChannelFreq > maxfreq ? maxfreq : maxChannelFreq;
            var freqRes = pack.Properties.SampInterval ;// 1 / (pack.Properties.SampInterval * pack.Buffer.ToRowEnumerable().Count());

            
            Double bandwidth = 0;
            switch (analysisType)
            {
                case OBWAnalysisType.Percentage:
                    bandwidth = GetBandwidthByPercentage(percentage, bufferarray, minchannelfreq, maxChannelFreq, freqRes, fftArg.ResultUnit);
                    break;
                case OBWAnalysisType.dBDown:
                    bandwidth = GetBandwidthBydBDown(dbdown, bufferarray, minchannelfreq, maxChannelFreq, freqRes, fftArg.ResultUnit);
                    break;
                default:
                    break;
            }
            return bandwidth;
        }

        private Double GetBandwidthByPercentage(Double percentage, Double[] bufferArray, Double minChannelFreq, Double maxChannelFreq, Double freqRes, FFTCoordUnit fFTCoordUnit)
        {
            Double totalpower = Double.NaN;
            totalpower = FrequencyMeasurement.GetChannelPower(bufferArray, minChannelFreq, maxChannelFreq, freqRes, FFTCoordUnit.Vrms);

            Int32 minindex = (Int32)Math.Ceiling(minChannelFreq / freqRes);
            Int32 maxindex = (Int32)Math.Floor(maxChannelFreq / freqRes);
            if (maxindex - minindex < 0 || minindex < 0 || maxindex < 0)
            {
                return Double.NaN;
            }
            Double[] channeldata = new Double[maxindex - minindex];
            Array.Copy(bufferArray, minindex, channeldata, 0, channeldata.Length);

            Double frontPercentage = (1 - percentage / 100) / 2;
            Double endOfPercentage = (1 - percentage / 100) / 2;
            Double accumulatepower = 0;
            Double averagepower = 0;
            Double temptotalpower = 0;
            Double amp = fFTCoordUnit == FFTCoordUnit.Vrms ? 1 : 1000;

            Boolean findstartfreq = false;
            Boolean findendfreq = false;
            Double startfreq = 0;
            Double endfreq = 0;
            for (Int32 i = 0; i < channeldata.Length - 1; i++)
            {
                var lowerpower = FrequencyMeasurement.GetChannelPower(bufferArray, minChannelFreq, minChannelFreq+(i+1)* freqRes, freqRes, FFTCoordUnit.Vrms);
                //accumulatePower += (channelData[i] / amp);
                //averagePower = 10 * Math.Log10(accumulatePower / (i + 1));
                //tempTotalPower = 10 * Math.Log10(i + 1) + averagePower;
                if (lowerpower*(i+1) < totalpower * channeldata.Length * frontPercentage)
                {

                    startfreq = ( minindex + (i+1) ) * freqRes;
                    findstartfreq = true;
                    break;
                }
                
            }
            accumulatepower = 0;
            averagepower = 0;
            temptotalpower = 0;
            for (Int32 i = 0; i < channeldata.Length - 1; i++)
            {
                var upperpower = FrequencyMeasurement.GetChannelPower(bufferArray, maxChannelFreq-(i+1)*freqRes,maxChannelFreq, freqRes, FFTCoordUnit.Vrms);
                //accumulatePower += Math.Pow(10, (channelData[channelData.Length - 1 - i] / amp)/10);
                //averagePower = 10 * Math.Log10(accumulatePower / (i + 1));
                //tempTotalPower = 10 * Math.Log10(i + 1) + averagePower;
                if (upperpower * (i + 1) < totalpower * channeldata.Length * endOfPercentage)
                {
                    endfreq = (maxindex - (i +1)) * freqRes;
                    findendfreq = true;
                    break;
                }
            }
            if (findendfreq == true && findstartfreq == true)
            {
                var obw = endfreq - startfreq;
                return obw > 0 ? obw : 0;
            }
            else
            {
                return 0;
            }

        }
        //private Double GetBandwidthByPercentage(Double percentage, Double[] bufferArray, Double minChannelFreq, Double maxChannelFreq, Double freqRes, FFTCoordUnit fFTCoordUnit)
        //{
        //    Double totalPower = Double.NaN;
        //    totalPower = FrequencyMeasurement.GetChannelPower(bufferArray, minChannelFreq, maxChannelFreq, freqRes, fFTCoordUnit);

        //    Int32 minIndex = (Int32)Math.Ceiling(minChannelFreq / freqRes);
        //    Int32 maxIndex = (Int32)Math.Floor(maxChannelFreq / freqRes);
        //    if (maxIndex - minIndex < 0 || minIndex < 0 || maxIndex < 0)
        //    {
        //        return Double.NaN;
        //    }
        //    Double[] channelData = new Double[maxIndex - minIndex];
        //    Array.Copy(bufferArray, minIndex, channelData, 0, channelData.Length);

        //    Double frontPercentage = (1 - percentage / 100) / 2;
        //    Double endOfPercentage = (1 - percentage / 100) / 2;
        //    Double accumulatePower = 0;
        //    Double averagePower = 0;
        //    Double tempTotalPower = 0;
        //    Double amp = fFTCoordUnit == FFTCoordUnit.Vrms ? 1 : 1000;

        //    Boolean findStartFreq = false;
        //    Boolean findEndFreq = false;
        //    Double startFreq = 0;
        //    Double endFreq = 0;
        //    for (Int32 i = 0; i < channelData.Length - 1; i++)
        //    {
        //        var lowerPower = FrequencyMeasurement.GetChannelPower(bufferArray, minChannelFreq, minChannelFreq + (i + 1) * freqRes, freqRes, fFTCoordUnit);
        //        //accumulatePower += (channelData[i] / amp);
        //        //averagePower = 10 * Math.Log10(accumulatePower / (i + 1));
        //        //tempTotalPower = 10 * Math.Log10(i + 1) + averagePower;
        //        if (lowerPower * (i + 1) < totalPower * channelData.Length * frontPercentage)
        //        {

        //            startFreq = (minIndex + (i + 1)) * freqRes;
        //            findStartFreq = true;
        //            break;
        //        }

        //    }
        //    accumulatePower = 0;
        //    averagePower = 0;
        //    tempTotalPower = 0;
        //    for (Int32 i = 0; i < channelData.Length - 1; i++)
        //    {
        //        var upperPower = FrequencyMeasurement.GetChannelPower(bufferArray, maxChannelFreq - (i + 1) * freqRes, maxChannelFreq, freqRes, fFTCoordUnit);
        //        //accumulatePower += Math.Pow(10, (channelData[channelData.Length - 1 - i] / amp)/10);
        //        //averagePower = 10 * Math.Log10(accumulatePower / (i + 1));
        //        //tempTotalPower = 10 * Math.Log10(i + 1) + averagePower;
        //        if (upperPower * (i + 1) < totalPower * channelData.Length * endOfPercentage)
        //        {
        //            endFreq = (maxIndex - (i + 1)) * freqRes;
        //            findEndFreq = true;
        //            break;
        //        }
        //    }
        //    if (findEndFreq == true && findStartFreq == true)
        //    {
        //        var obw = endFreq - startFreq;
        //        return obw > 0 ? obw : 0;
        //    }
        //    else
        //    {
        //        return 0;
        //    }

        //}

        private Double GetBandwidthBydBDown(Int32 dbdown, Double[] bufferArray, Double minChannelFreq, Double maxChannelFreq, Double freqRes, FFTCoordUnit fFTCoordUnit)
        {
            //Double totalPower = Double.NaN;
            //totalPower = ChannelPower.GetChannelPower(bufferArray, minChannelFreq, maxChannelFreq, freqRes, fFTCoordUnit);

            Int32 minIndex = (Int32)Math.Ceiling(minChannelFreq / freqRes);
            Int32 maxIndex = (Int32)Math.Floor(maxChannelFreq / freqRes);
            if (maxIndex - minIndex < 0 || minIndex < 0 || maxIndex < 0)
            {
                return Double.NaN;
            }

            Double[] channelData = new Double[maxIndex - minIndex];
            Array.Copy(bufferArray, minIndex, channelData, 0, channelData.Length);

            Double amp = fFTCoordUnit == FFTCoordUnit.Vrms ? 1 : 1000;
            Double maxPower = channelData.Max() / amp;
            Double minPower = channelData.Min() / amp;
            Double threshold = maxPower + dbdown;


            Boolean findStartFreq = false;
            Boolean findEndFreq = false;
            Double startFreq = 0;
            Double endFreq = 0;
            for (Int32 i = 0; i < channelData.Length; i++)
            {
                if (channelData[i] / amp > threshold)
                {
                    startFreq = (minIndex + i) * freqRes;
                    findStartFreq = true;
                    break;
                }
            }
            for (Int32 i = 0; i < channelData.Length; i++)
            {
                if (channelData[channelData.Length - 1 - i] / amp > threshold)
                {
                    endFreq = (maxIndex - i) * freqRes;
                    findEndFreq = true;
                    break;
                }
            }
            if (findEndFreq == true && findStartFreq == true)
            {
                var obw = endFreq - startFreq;
                return obw > 0 ? obw : 0;
            }
            else
            {
                return 0;
            }
        }

        public Double? Run()
        {
            ChannelId channelId = _ChannelId;
            if (!FrequencyMeasurement.IsCorrectChannelState(channelId, out var mathModel, out var fftArg))
            {
                return null;
            }
            var pack = DsoModel.Default.GetWfmPack(channelId);
            if (pack != null)
            {
                GetMeasGates();
                var result = GetOccupiedBandwidth(pack, ChannelSpan, Percentage, dBDown, AnalysisType,mathModel, fftArg);
                GetMeasResults(new List<Double>() { result }, Prefix.Empty, QuantityUnit.Hertz.ToUnitString(), QuantityUnit.Hertz.ToUnitString());

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

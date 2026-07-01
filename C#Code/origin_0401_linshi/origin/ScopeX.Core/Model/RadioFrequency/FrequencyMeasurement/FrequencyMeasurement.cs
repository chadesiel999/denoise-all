using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    public static class FrequencyMeasurement
    {
        public static Boolean GetMeasGates(ChannelId id, Double channelSpan, Double spacing, Int32 channelCount, [NotNullWhen(true)] out List<(Single xMin, Single xMax)>? measureGates, Boolean unidirectional = false)
        {
            if (!FrequencyMeasurement.IsCorrectChannelState(id, out var mathmodel, out var fftarg))
            {
                measureGates = null;
                return false;
            }
            var pack = DsoModel.Default.GetWfmPack(id);
            if (pack != null)
            {
                var span = mathmodel.FrequencyAdapter.ValueSpan;
                var halfspan = span / 2;
                var xspannum = Constants.VIS_XDIVS_NUM * Constants.IDX_PER_XDIV;
                Single xleft;
                Single xright;
                measureGates = new();
                Int32 start = unidirectional ? 0 : -channelCount;
                List<Single> allpoints = new() { 0 };
                for (Int32 i = start; i < channelCount + 1; i++)
                {
                    xleft = (Single)((halfspan + i * spacing - channelSpan / 2) / span * xspannum);
                    xleft = xleft < 0 ? 0 : xleft;
                    xleft = xleft > xspannum ? xspannum : xleft;
                    allpoints.Add(xleft);

                    xright = (Single)((halfspan + i * spacing + channelSpan / 2) / span * xspannum);
                    xright = xright < 0 ? 0 : xright;
                    xright = xright > xspannum ? xspannum : xright;
                    allpoints.Add(xright);
                }
                allpoints.Add(xspannum);

                for (Int32 i = 0; i < allpoints.Count / 2; i++)
                {
                    if (2*i + 1 < allpoints.Count)
                    {
                        measureGates.Add(new(allpoints[2*i], allpoints[2*i + 1]));
                    }
                }

                return true;
            }
            measureGates = null;
            return false;
        }

        public static Boolean GetMeasResultsPositions(List<Double> reaults, Prefix prefix, String mainUnitString, String sideUnitString, ChannelId id, Double channelSpan, Double spacing, Int32 channelCount, [NotNullWhen(true)] out List<(Single Position, Single Result, String ResultUnitString)>? measureResults, Boolean unidirectional = false,Boolean changeMainUnit = false)
        {
            Int32 resultlength = 1 + channelCount * (unidirectional ? 1 : 2);
            if (!unidirectional && reaults.Count != resultlength)
            {
                measureResults = null;
                return false;
            }
            if (!FrequencyMeasurement.IsCorrectChannelState(id, out var mathModel, out var fftArg))
            {
                measureResults = null;
                return false;
            }
            var pack = DsoModel.Default.GetWfmPack(id);
            if (pack != null)
            {
                var span = mathModel.FrequencyAdapter.ValueSpan;
                var halfspan = span / 2;
                var xspannum = Constants.VIS_XDIVS_NUM * Constants.IDX_PER_XDIV;
                Single xposition;
                String unitstring = "";
                measureResults = new();
                Int32 start = unidirectional ? 0 : -channelCount;
                List<(Single Position,String UnitString)> allPoints = new();
                for (Int32 i = start; i < channelCount + 1; i++)
                {
                    xposition = (Single)((halfspan + i * spacing) / span * xspannum);
                    xposition = xposition < 0 ? 0 : xposition;
                    xposition = xposition > xspannum ? xspannum : xposition;

                    unitstring = i == 0 ? (changeMainUnit ? mainUnitString : fftArg.ResultUnit.ToString()): sideUnitString;
                    allPoints.Add((xposition, unitstring));
                }

                for (Int32 i = 0; i < allPoints.Count; i++)
                {
                    if ( i < reaults.Count)
                    {
                        measureResults.Add(new(allPoints[i].Position, (Single)reaults[i], allPoints[i].UnitString));
                    }
                }

                return true;
            }
            measureResults = null;
            return false;
        }

        internal static Boolean IsCorrectChannelState(ChannelId channelId, [NotNullWhen(true)] out MathModel? mathModel, [NotNullWhen(true)] out MathFftArg? fftArg)
        {
            if (channelId.IsMath())
            {
                if (DsoModel.Default.TryGetChannel(channelId, out var channelModel))
                {
                    mathModel = (MathModel)channelModel;
                    if (mathModel.Args != null && mathModel.Args.Type == MathType.FFT)
                    {
                        if (((MathFftArg)mathModel.Args).ResultType == FFTResultOpt.Ampltd)
                        {
                            fftArg = (MathFftArg)mathModel.Args;
                            return true;
                        }
                    }
                }
            }
            mathModel = null;
            fftArg = null;
            return false;
        }
        
        public static Double GetChannelPower(Double[] bufferArray, Double minChannelFreq, Double maxChannelFreq, Double freqRes, FFTCoordUnit fFTCoordUnit)
        {
            Int32 minIndex = (Int32)Math.Ceiling(minChannelFreq / freqRes);
            Int32 maxIndex = (Int32)Math.Floor(maxChannelFreq / freqRes);
            if (maxIndex - minIndex < 0 || minIndex < 0 || maxIndex < 0)
            {
                return Double.NaN;
            }
            Double[] channelData = new Double[maxIndex - minIndex];

            Array.Copy(bufferArray, minIndex, channelData, 0, channelData.Length);

            Double amp = fFTCoordUnit == FFTCoordUnit.Vrms ? 1 : 1000;
            Double averagePower = 0;
            for (Int32 i = 0; i < channelData.Length; i++)
            {
                averagePower += (channelData[i] / amp);
            }

            averagePower = averagePower / (channelData.Length);
            Double channelPower = Double.NaN;
            if (fFTCoordUnit == FFTCoordUnit.Vrms)
            {
                channelPower = averagePower;
            }
            else
            {
                channelPower = 10 * Math.Log10(((maxChannelFreq - minChannelFreq) / freqRes)) + averagePower;
            }
            return channelPower;
        }

        public static Double GetMaxPower(Double[] bufferArray, Double minChannelFreq, Double maxChannelFreq, Double freqRes, FFTCoordUnit fFTCoordUnit)
        {
            Int32 minIndex = (Int32)Math.Ceiling(minChannelFreq / freqRes);
            Int32 maxIndex = (Int32)Math.Floor(maxChannelFreq / freqRes);
            if (maxIndex - minIndex < 0 || minIndex < 0 || maxIndex < 0)
            {
                return Double.NaN;
            }
            Double amp = fFTCoordUnit == FFTCoordUnit.Vrms ? 1 : 1000;
            if (maxIndex - minIndex == 0)
            {
                return Double.NaN; 
            }
            Double[] channelData = new Double[maxIndex - minIndex];

            Array.Copy(bufferArray, minIndex, channelData, 0, channelData.Length);


            Double maxPower = channelData.Max() / amp;

            return maxPower;
        }
    }
}

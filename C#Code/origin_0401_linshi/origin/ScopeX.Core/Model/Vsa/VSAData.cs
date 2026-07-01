using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public class VSAData
    {
        public VSAData()
        {
            JitterParameter = new JitterParameter();
        }
        public VsaFormatOpt _FormatOpt = VsaFormatOpt.QPSK;





        public ChannelId Source;
        public PllTypeOpt PLLType;
        public SignalType SignalType;
        public ClockTypeOpt ClockType;

        public double[] Data;
        public double[] TIEData;
        public double[] ClockEdges;
        public double[] RecoveredEdges;
        public double[] Edges;
        public double[] ActualEdges;
        public double[] ReferenceEdges;
        public Int32[] NonzeroIndex;


        public double TIE;
        public double DDJ;
        public double ISI;
        public double DCD;

        public Int32 UICount;
        public double UnitInterval;
        public double HighLevel;
        public double LowLevel;
        public double Fs = 50_000_000_000;
        public double NaturalFreq;
        public double CutoffDivisor;
        public double DamplingFactor;
        public double ThresholdFreq;
        public double PatternLength;
        public double Hysteresis;
        public double Threshold;
        public double TopThreshold;
        public double BaseThreshold;
        public double AverageUILength;
        public double BinWidth = 0.01;

        public double EyeSampleInterval;

        public Int32 InterpolationMultiple;

        public JitterParameter JitterParameter;

    }
}

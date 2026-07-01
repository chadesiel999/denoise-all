using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public class JitterParameter
    {
        public JitterParameter()
        {
        }
        public ChannelId Source;

        public PllTypeOpt PLLType;
        public SignalType SignalType;
        public ClockTypeOpt ClockType;

        public (Double SampleInterval, Double VerPosIndex, Double VerBias, Double VerScale, Double HorScale) DataParams;
        public Double Fs;
        public Double NaturalFreq;
        public Double CutoffFreq;
        public Double CutoffDivisor;
        public Double DamplingFactor;
        public Double ThresholdFreq;
        public Int32 PatternLength;
        public Double Hysteresis;
        public Double Threshold;
        public Double TopThreshold;
        public Double BaseThreshold;
        public Int32 InterpolationMultiple;
        public Double BinWidth = 0.01;
        public Double OldFs = 50_000_000_000;
        public Double OldAverageUILength;
        public MaxBinNum UIBinNum = MaxBinNum.Num250;



        public Double[] TIEData;
        public Double[] TIEDataAccumulate;
        public Double[] TIEDataAfterInterp;
        public Double[] TIEDataWithoutDDJAfterInterp;


        public Double EyeSampleInterval;
        public Double VirticalScaleRatio = Double.NaN;
        public Double VirticalLevelPosition = Double.NaN;

        public void Dispose()
        {
            // 清理托管资源
            
            if (TIEData != null)
            {
                Array.Clear(TIEData, 0, TIEData.Length);
                TIEData = null;
            }

            if (TIEDataAccumulate != null)
            {
                Array.Clear(TIEDataAccumulate, 0, TIEDataAccumulate.Length);
                TIEDataAccumulate = null;
            }

            if (TIEDataAfterInterp != null)
            {
                Array.Clear(TIEDataAfterInterp, 0, TIEDataAfterInterp.Length);
                TIEDataAfterInterp = null;
            }

            if (TIEDataWithoutDDJAfterInterp != null)
            {
                Array.Clear(TIEDataWithoutDDJAfterInterp, 0, TIEDataWithoutDDJAfterInterp.Length);
                TIEDataWithoutDDJAfterInterp = null;
            }

            GC.SuppressFinalize(this);
        }
    }


    public class JitterPrepare
    {
        public Double Fs;

        public Double HighLevel;
        public Double LowLevel;

        public Int32 UICount;
        public Double AverageUILength = Double.NaN;

        public Double[] ClockEdges;
        public Double[] RecoveredEdges;
        public Int32[] NonzeroIndex;
        public (Double[] TIEs, Int32 Length, Double Max, Double Min, Double Average) TIEData;
    }

    public class NrzEyePrepare
    {
        public Double[] SampleData;
        public (Double SampleInterval, Double VerPosIndex, Double VerBias, Double VerScale, Double HorScale) DataParams;
        public Double Fs;

        public (Double[] TIEs, Int32 Length, Double Max, Double Min, Double Average) TIEData;

        public Double HighLevel;
        public Double LowLevel;
        public Double Threshold;

        public Int32 UICount;
        public Double AverageUILength = Double.NaN;

        public Double[] ClockEdges;
        public Double[] RecoveredEdges;
    }

    public class Pam4EyePrepare
    {
        public Double[] SampleData;
        public (Double SampleInterval, Double VerPosIndex, Double VerBias, Double VerScale, Double HorScale) DataParams;
        public Double Fs;
        public Double UILength;
        public List<Double> FinalEdges;
        public List<Double> InsertEdges;
    }

    public class JitterResult
    {
        public Double TIE { get; set; }

        public Double DJ { get; set; }
        public Double PJ { get; set; }
        public Double RJ { get; set; }
        public Double TJ { get; set; }
        public Double TJ_BER12 { get; set; }

        public Double ISI { get; set; }
        public Double DDJ { get; set; }
        public Double DCD { get; set; }
        public Double CC { get; set; }
    }

    public class EyeResult
    {

    }
}

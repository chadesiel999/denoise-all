using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    public class MJSQAnalyzer : IJitterAnalyzer
    {
        public Double TJ;

        public Double DJ;
        public Double RJ;
        public Double TJ_BER12;
        public Double[,] QWaveMatrix;
        public Double[,] BathWaveMatrix;
        public Boolean Run(JitterParameter jitterData, JitterPrepare prepareData, JitterResult result)
        {
            //StatisticalAnalyzer.StatisticalProcess(jitterData.TIEData.ToList(), jitterData.AverageUILength,jitterData.BinWidth, out Double tjStats, out Double djStats, out Double rjStats, out Double tj_e12, out Double[,] bathMatrix, out Double[,] qMatrix);

            var rst = StatisticalAnalyzer.StatisticalProcessNew(TIE.JitterHist, jitterData.Fs, prepareData.AverageUILength, out Double tjStats, out Double djStats, out Double rjStats, out Double tj_e12, out Double[,] bathMatrix, out Double[,] qMatrix);

            if (rst)
            {
                result.TJ = tjStats;
                result.TJ_BER12 = tj_e12;
                result.DJ = djStats;
                result.RJ = rjStats;

                QWaveMatrix = qMatrix;
                BathWaveMatrix = bathMatrix;

                DJ = djStats;
                RJ = rjStats;
                TJ = tjStats;
                TJ_BER12 = tj_e12;
                return true;
            }

            return false;
        }

        public Vector GetButhWave(JitterParameter jitterData)
        {
            return new Vector(BathWaveMatrix,
                //QuantityUnitExt.ToUnitString(QuantityUnit.Second),
                "UI",
                QuantityUnitExt.ToUnitString(QuantityUnit.BER),
                //1 / jitterData.Fs / BathWaveMatrix.GetLength(1) * (jitterData.SignalType == SignalType.PRBSCode ? 2 * jitterData.AverageUILength : jitterData.AverageUILength),
                1.0 / BathWaveMatrix.GetLength(1),
                Constants.DEF_XPOS_IDX);
        }

        public Vector GetQWave(JitterParameter jitterData, JitterPrepare prepareData)
        {
            return new Vector(QWaveMatrix,
                QuantityUnitExt.ToUnitString(QuantityUnit.Second),
                QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
                1 / jitterData.Fs / BathWaveMatrix.GetLength(1) * (jitterData.SignalType == SignalType.PRBSCode ? 2 * prepareData.AverageUILength : prepareData.AverageUILength),
                Constants.DEF_XPOS_IDX);
        }

        public DecompositionParameter GetResult()
        {
            return new DecompositionParameter()
            {
                PJ = Double.NaN,
                RJ = RJ,
                DJ = DJ,
                TJ = TJ,
                TJ_BER12 = TJ_BER12,
            };
        }

        public void Dispose()
        {
            QWaveMatrix = null;
            BathWaveMatrix = null;
        }
    }
}

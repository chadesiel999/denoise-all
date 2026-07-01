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
    public class NQScaleAnalyzer : IJitterAnalyzer
    {
        public Double DJ;
        public Double RJ;
        public Double TJ;
        public Double TJ_BER12;
        public Double[,] QWaveMatrix;
        public Double[,] BathWaveMatrix;
        public Boolean Run(JitterParameter jitterData,JitterPrepare prepare,JitterResult result)
        {
            StatisticalAnalyzer.StatisticalProcess(prepare, prepare.AverageUILength, jitterData.BinWidth, out Double tjStats, out Double djStats, out Double rjStats, out Double tj_e12, out Double[,] bathMatrix, out Double[,] qMatrix);
            result.TJ = tjStats;
            result.TJ_BER12 = tj_e12;
            result.DJ = djStats;
            result.RJ = rjStats;

            QWaveMatrix = qMatrix;
            BathWaveMatrix = bathMatrix;
            GC.Collect();
            DJ = tjStats;
            RJ = tjStats;
            TJ = tjStats;
            TJ_BER12 = tj_e12;
            return true;
        }

        public Vector GetBathWave(JitterParameter jitterData,JitterPrepare prepareData)
        {
            return new Vector(BathWaveMatrix,
                QuantityUnitExt.ToUnitString(QuantityUnit.Second),
                QuantityUnitExt.ToUnitString(QuantityUnit.Constant),
                1 / jitterData.Fs / BathWaveMatrix.GetLength(1) * (jitterData.SignalType == SignalType.PRBSCode ? 2 * prepareData.AverageUILength : prepareData.AverageUILength),
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
            };
        }

        public void Dispose()
        {
            QWaveMatrix = null;
            BathWaveMatrix = null;
        }
    }
}

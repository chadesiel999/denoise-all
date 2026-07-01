using MathWorks.MATLAB.NET.Arrays;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ScopeX.Hardware.Driver
{
    internal class MatlabDll_CutPts
    {
        private const String _CutptsDllName = "MatlabGenerateOverlapBandSync_Cutpts.dll";
        private const String _CutptsTypeName = "MatlabGenerateOverlapBandSync_Cutpts.Class1";
        private const String _CutptsMethodName = "MatlabGenerateOverlapBandSync_Cutpts";
        private MatlabDllEngine? _CutPts;

        public MatlabDll_CutPts()
        {
            Type[] signature = { typeof(MWCharArray), typeof(MWCharArray), typeof(MWCharArray), typeof(MWCharArray), typeof(MWCharArray) };
            _CutPts = new(_CutptsDllName, _CutptsTypeName, new() { [_CutptsMethodName] = signature });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subbandId"></param>
        /// <param name="phaseDiffTable"></param>
        /// <param name="sampleFreqByGhz"></param>
        /// <param name="parallelRoad"></param>
        /// <returns></returns>
        public Double[] CalcCutPts(Int32 subbandId, Int32[] originDiscardDots, Dictionary<Int32, Double> phaseDiffTable, Int32 sampleFreqByGhz, Int32 parallelRoad)
        {
            String freqStr = String.Join(",", phaseDiffTable.Keys);
            String phase = String.Join(",", phaseDiffTable.Values.Select(o => o.ToString("0.000")));
            String cfgparam = $"{freqStr} {phase} {sampleFreqByGhz} {parallelRoad} {originDiscardDots.Length / 2} {subbandId}";

            String paramstr = $"{cfgparam} {String.Join(",", originDiscardDots)}";
            Trace.WriteLine($"[CalcCutPts]paramstr:{paramstr}");

            String filepath = "0";
            String filename = "0";
            String DebugSaveFilePath = "0";
            String DebugSaveFilePrefixName = "0";
            MWArray[] paramArray = new MWArray[]
            {
                new MWCharArray(filepath.ToCharArray()),
                new MWCharArray(filename.ToCharArray()),
                new MWCharArray(DebugSaveFilePath.ToCharArray()),
                new MWCharArray(DebugSaveFilePrefixName.ToCharArray()),
                new MWCharArray(paramstr.ToCharArray()),
            };

            var ans = _CutPts?.Excute(_CutptsMethodName, paramArray);

            if (ans == null || ans is not MWNumericArray)
            {
                return new Double[0];
            }

            var cutpts = (Double[]?)((ans as MWNumericArray)?.ToVector(MWArrayComponent.Real));
            if (cutpts == null)
                return new Double[0];

            return cutpts;
        }
    }
}

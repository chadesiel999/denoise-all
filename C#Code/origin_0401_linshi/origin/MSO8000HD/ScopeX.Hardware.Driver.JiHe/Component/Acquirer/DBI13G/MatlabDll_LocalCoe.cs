using MathWorks.MATLAB.NET.Arrays;
using System;
using System.Diagnostics;

namespace ScopeX.Hardware.Driver
{
    public class MatlabDll_LocalCoe
    {
        private const String _LocalCoeDllName = "MatlabGenerateOverlapBandSync_LoCoe.dll";
        private const String _LocalCoeTypeName = "MatlabGenerateOverlapBandSync_LoCoe.Class1";
        private const String _LocalCoeMethodName = "MatlabGenerateOverlapBandSync_LoCoe";
        private Type[] _LocalCoeSignature = { typeof(MWCharArray), typeof(MWCharArray), typeof(MWCharArray), typeof(MWCharArray), typeof(MWCharArray) };

        private MatlabDllEngine? _LocalCoe;

        public MatlabDll_LocalCoe()
        {
            _LocalCoe = new(_LocalCoeDllName, _LocalCoeTypeName, new() { [_LocalCoeMethodName] = _LocalCoeSignature });
        }

        public Double[] CalcLocalCoe(Double phaseDiff, Double sampleFreqByGHz, Double localFreqByGHz, Int32 coeLength)
        {
            string cfgparam = $"{sampleFreqByGHz} {phaseDiff.ToString("0.000")} {localFreqByGHz} 16 {coeLength}";
            string filepath = "111";
            string filename = "111";
            string DebugSaveFilePath = "111";
            string DebugSaveFilePrefixName = "111";
            MWArray[] paramArray = new MWArray[]
            {
                filepath,
                filename,
                DebugSaveFilePath,
                DebugSaveFilePrefixName,
                cfgparam,
            };
            Trace.WriteLine($"[CalcLocalCoe]cfgparam:{cfgparam}");

            var tmp = _LocalCoe?.Excute(_LocalCoeMethodName, paramArray);
            if (tmp == null || tmp is not MWNumericArray)
            {
                return new Double[0];
            }

            var ans = (Double[]?)((tmp as MWNumericArray)?.ToVector(MWArrayComponent.Real));
            if (ans == null)
            {
                return new Double[0];
            }

            return ans;
        }

    }
}

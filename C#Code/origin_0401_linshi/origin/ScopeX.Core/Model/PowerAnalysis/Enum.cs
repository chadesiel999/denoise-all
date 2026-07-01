using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.PowerAnalysis
{
    public enum PowerAnalysisOpt
    {
        [Description("Enum_PowerAnalysisOpt_PowerQuality")]
        PowerQuality,
        [Description("Enum_PowerAnalysisOpt_Harmonic")]
        Harmonic,
        [Description("Enum_PowerAnalysisOpt_Ripple")]
        Ripple,
        [Description("Enum_PowerAnalysisOpt_SwitchingLoss")]
        SwitchingLoss,
        [Description("Enum_PowerAnalysisOpt_SafeOperationArea")]
        SafeOperationArea,
        [Description("Enum_PowerAnalysisOpt_LoopAnalysis")]
        LoopAnalysis,
        [Description("Enum_PowerAnalysisOpt_Modulation")]
        Modulation,
        [Description("Enum_PowerAnalysisOpt_InrushCurrent")]
        InrushCurrent,
        [Description("Enum_PowerAnalysisOpt_PowerEfficency")]
        PowerEfficency,
        [Description("Enum_PowerAnalysisOpt_Differ")]
        Differ,
        [Description("Enum_PowerAnalysisOpt_Transient")]
        Transient,
        [Description("Enum_PowerAnalysisOpt_RDSon")]
        RDSon,
        [Description("Enum_PowerAnalysisOpt_TurnOnOff")]
        TurnOnOff,
        [Description("Enum_PowerAnalysisOpt_PSRR")]
        PSRR,
        [Description("Enum_PowerAnalysisOpt_SlewRate")]
        SlewRate,
    }

    public enum VIType
    {
        V = 0,
        I = 1
    }

    public enum HarmonicDisplayMode
    {
        Excel = 0,
        Figure = 1
    }

    public enum HarmonicDisplayOpt
    {
        Odd = 0,
        Even = 1,
        All = 2
    }

    public enum HarmonicRefFreqSrc
    {
        HarmonicSrc = 0,
        V = 1,
        I = 2,
        Constant = 3
    }
    public enum HarmonicStandard
    {
        None = 0,
        IEC6100032A = 1,
        IEC6100032B = 2,
        IEC6100032C = 3,
        IEC6100032D = 4
    }
    public enum ModulationType
    {
        [Description("ModulationPeriod")]
        Period,
        [Description("ModulationFrequency")]
        Frequency,
        [Description("ModulationPDuty")]
        PDuty,
        [Description("ModulationNDuty")]
        NDuty,
        [Description("ModulationPWidth")]
        PWidth,
        [Description("ModulationNWidth")]
        NWidth,
        [Description("ModulationRiseTime")]
        RiseTime,
        [Description("ModulationFallTime")]
        FallTime,
    }

    public enum ModulationWfmType
    {
        Trend,
        Histgram,
    }

    public enum TurnOnOffType
    {
        [Description("DC-DC")]
        DC2DC,
        [Description("AC-DC")]
        AC2DC,
    }

    public enum TurnOnOffTestType
    {
        [Description("TurnOn")]
        TurnOn,
        [Description("TurnOff")]
        TurnOff,
    }

    public enum SWLossType
    {
        All = 0,
        Power = 1,
        Energy = 2
    }

    public enum ScanMode
    {
        Continuous,
        Single
    }
    public enum ImpedanceType
    {
        Low50,
        High1M
    }
    public enum AmplitudeMode
    {
        Constant,
        Variable
    }
    public enum AWGId
    {
        G1,
        G2
    }
    public enum CurrentType
    {
        AC,
        DC
    }

    public enum TestMode
    {
        Single,//单次测试
        RealTime,//实时分析
    }

    public struct DataOpt
    {
        public Double Freq;     // Hz
        public Double Amp;      // V
        public Double Gain;     // dB
        public Double Phase;	// °
        public Double PSRR;     //dB
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    [Flags]
    public enum CaliDataType
    {
        None                                        = 0,
        TiAdc_PhaseOffsetGain                       = 0x0001,
        TiAdc_PhaseOffsetGain_JiHe_MSO7000X         = 0x0002, 
        TiAdc_SyncSampleClock                       = 0x0003,
        PhyChannel                                  = 0x0004,
        PhyChannelModel2                            = 0x0005,
        Misc                                        = 0x0006,
        CoefficientsTables                          = 0x0007,
        DbiAnalogParams                             = 0x0008,
        DbiCoefficientsTables                       = 0x0009,
        DbiLocalOscillators                         = 0x000A,
        AWG                                         = 0x000B,
        TiAdc_PhaseOffsetGain_Factory               = 0x000C,
        AnalogParams                                = 0x0010,
        TiadcPhaseOffsetGainParams                  = 0x0011,
        CoefficientsParams                          = 0x0012,
        AutoCalibration                             = 0x0013,
 		AiAnalogParams                              = 0x0014,
        ProcTiCoefficientsTables                    = 0x0015,
        DbiAnalogParams_Common                      = 0x0016,
        All = 0xffff
    }
}

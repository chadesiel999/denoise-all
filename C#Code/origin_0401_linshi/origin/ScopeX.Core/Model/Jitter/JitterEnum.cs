using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public enum JitterDecompositionType
    {
        Spectrum,
        MJSQ,
        NQScale,
    }

    public enum SignalType
    {
        Clock,
        PRBSCode,
    }

    public enum StatisticalConstructionMode
    {
        Single,
        Accumulation,
    }

    public enum EyeSignalType
    {
        NRZ,
        PAM3,
        PAM4,
        PAM5,
        PAM6,
        PAM7,
        PAM8,
    }
}

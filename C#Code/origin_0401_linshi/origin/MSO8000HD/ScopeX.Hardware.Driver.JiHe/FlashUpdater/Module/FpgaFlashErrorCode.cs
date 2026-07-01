using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    public enum FpgaFlashErrorCode
    {
        Succeed = 0,
        FLashIDMismatching = 1,
        EraseOvertime = 2,
        EraseDefeated = 3,
        WriteOvertime = 4,
        WriteVerifyDefeated = 5,
        InfoError=6,
        NotFound = 7,
        NullInfoZone,
        ErrorInfoZone,
        ErrorEraseLength,
        ErrorAddr,
        GeneralError = 99,
        ErrorOvertime=100,
    }
}

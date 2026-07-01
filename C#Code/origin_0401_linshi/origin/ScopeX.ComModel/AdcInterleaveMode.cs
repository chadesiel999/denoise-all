using System;

namespace ScopeX.ComModel
{
    public enum AdcInterleaveMode : Byte
    {
        Mode4To1 = 0x00,
        Mode2To1 = 0x02,
        Mode1To1 = 0x03,
        Mode1To2 = 0x04,//7000HD中独用
    }
}

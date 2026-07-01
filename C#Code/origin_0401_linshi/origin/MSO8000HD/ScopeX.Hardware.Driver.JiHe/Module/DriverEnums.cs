using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    public enum DriverTypes
    {
        DCCardPcie,
        CyUsb3_0,
    }
    internal enum FPGAType
    {
        Pcie,
        S6,
        Proc,
        Acq
    }
    internal enum ComboDebugKeys
    {
        Ctrl_X = 0,
    }
    internal enum DownloadBlockDataMode
    {
        Register=0,
        DMA=1,
    }
}

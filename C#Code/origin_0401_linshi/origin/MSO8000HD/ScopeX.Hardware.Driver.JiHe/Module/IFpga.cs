using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Data.Base;
namespace ScopeX.Hardware.Driver
{
    interface IFpga
    {
        void Init();
        void Test();
        bool IsAllPowerOk();
        FpgaVersion FpgaVersion { get; set; }
        CaliDataType ChangedCaliDataType { get; set; }
        string GetRegMonitorResult();
    }
}

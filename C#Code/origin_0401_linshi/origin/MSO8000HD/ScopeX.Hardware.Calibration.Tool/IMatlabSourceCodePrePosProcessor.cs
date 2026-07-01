using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    interface IMatlabSourceCodePrePosProcessor
    {
        void PutInputData(MLApp.MLApp? matLabApp,IInstrumentSession? currInstrumentSession);
        void GetOutputData(MLApp.MLApp? matLabApp, IInstrumentSession? currInstrumentSession);
    }
}

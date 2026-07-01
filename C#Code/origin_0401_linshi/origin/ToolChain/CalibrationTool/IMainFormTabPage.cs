using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Calibration.Tool
{
    interface IMainFormTabPage
    {
        void SetInstrumentInteract(IInstrumentSession? instrumentInteract);
        void RefreshData();
        CaliDataType CaliDataType { get=> CaliDataType.None; }
        List<ProductType> Used4ProductTypes { get; }
    }
}

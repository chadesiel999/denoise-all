using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MathWorks.MATLAB.NET.Arrays;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 只定义不同型号可能不一样的地方
    /// </summary>
    internal partial class OscilloscopeProduct
    {
        #region Acquirer
        public AbstractAcquirer_Cymometer? Acquirer_Cymometer;
        #endregion
    }
}

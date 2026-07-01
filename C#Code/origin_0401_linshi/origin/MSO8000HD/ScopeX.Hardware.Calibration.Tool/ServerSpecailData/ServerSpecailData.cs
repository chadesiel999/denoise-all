using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    internal partial class ServerSpecailData
    {
        public static void Load(IInstrumentSession currInstrument)
        {
            MethodInfo[] methodInfos = typeof(ServerSpecailData).GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            foreach (MethodInfo m in methodInfos)
            {
                if (m.Name.StartsWith("LoadServerSpecailData_"))
                {
                    m.Invoke(null, new object[] { currInstrument });
                }
            }
        }
    }
}

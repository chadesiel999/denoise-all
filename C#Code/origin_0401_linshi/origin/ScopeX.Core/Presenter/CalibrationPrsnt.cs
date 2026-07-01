using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Hardware;

namespace ScopeX.Core
{
    public class CalibrationPrsnt
    {
        public static void Push(HdCmd cmd) => HdCmdFactory.Push(cmd);
    }
}

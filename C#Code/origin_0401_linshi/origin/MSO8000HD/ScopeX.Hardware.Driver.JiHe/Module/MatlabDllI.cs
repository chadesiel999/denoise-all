using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal record MatlabDll
    {
        public object? Instance = null;
        public MethodInfo? Method = null;
    }
}

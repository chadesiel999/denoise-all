using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Driver
{
    internal interface IAppendCommandTable
    {
        Dictionary<HdCmd, Action[]> AppendCommand();
    }
}

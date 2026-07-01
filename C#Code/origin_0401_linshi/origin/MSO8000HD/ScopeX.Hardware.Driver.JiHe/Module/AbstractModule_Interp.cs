using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver.Module
{
    internal abstract class AbstractModule_Interp
    {
        internal virtual UInt32 GetValidInterpNum(Double expectedNum)
        {
            return (UInt32)expectedNum;
        }

        internal virtual void ConfigNum(UInt32 interpNum)
        { 
            
        }
    }
}

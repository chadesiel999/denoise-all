using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal interface IBoard
    {
        void Init();
        void Test();
        FpgaVersion? FpgaVersion
        {
            get;
        }
    }
}

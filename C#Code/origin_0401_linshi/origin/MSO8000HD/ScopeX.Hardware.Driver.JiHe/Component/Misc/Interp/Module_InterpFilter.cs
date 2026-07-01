using ScopeX.Hardware.Driver.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 采用二级滤波器，减少滤波器的总阶数
    /// </summary>
    internal class Module_InterpFilter : AbstractModule_Interp
    {
        private Dictionary<UInt32, UInt32> _ValidInterpNumTable = new()
        {
            {1,   0x0101 },
            {2,   0x0102 },
            {5,   0x0105 },
            {10,  0x010a },
            {20,  0x020a },
            {50,  0x050a },
            {100, 0x0a0a },
            {200, 0x140a },
        };

        internal override void ConfigNum(uint interpNum)
        {
            // todo
        }

        internal override UInt32 GetValidInterpNum(Double expectedNum)
        {
            if (expectedNum < 1.0)
                return 1;

            UInt32[] validNum = _ValidInterpNumTable.Keys.ToArray();
            for (Int32 i = 0; i < validNum.Length - 1; i++)
            {
                if (validNum[i] <= expectedNum && validNum[i + 1] > expectedNum)
                    return validNum[i];
            }
            return validNum.Max();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    internal class ConstDefine
    {
        /// <summary>
        /// 微(u)到皮(p)的乘积倍率
        /// </summary>
        internal const Int64 Ratio_u2p = 1000;

        /// <summary>
        /// 皮(p)到飞(f)的乘积倍率
        /// </summary>
        internal const Int64 Ratio_p2f = 1000_000;

        /// <summary>
        /// 微(u)到飞(f)的乘积倍率
        /// </summary>
        internal const Int64 Ratio_u2f = 1_000_000_000;

        internal const Double Ratio_f = 1e15;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public sealed class HistParamter
    {
        public Double BinZoomRatio { get; internal set; } = 1;

        public Double MaxValue { get;internal set; }

        public Double MinValue { get;internal set; }

        public Int32 NBins { get; internal set; } = 100;

        public Int32 FixedBins { get; internal set; }
        public Int32 Total { get; internal set; }
    }
}

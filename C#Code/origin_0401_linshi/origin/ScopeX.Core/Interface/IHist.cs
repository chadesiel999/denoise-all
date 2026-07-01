using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public interface IHist
    {
        //HistData HistData { get;  }

        public Double BinZoomRatio { get;  set; }

        public Double MaxValue { get;  set; }

        public Double MinValue { get;  set; }

        public Int32 NBins { get;  set; }

        public Int32 FixedBins { get;  set; }
    }
}

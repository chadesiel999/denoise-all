using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public static class AnalogChanneScaleDefine
    {
        public static List<Int32> PhyChCoarseLevelTableByuV = new List<int>()
        {
            /*[(int)AnaChnlScaleIndex.Lv500u] = */500,
            /*[(int)AnaChnlScaleIndex.Lv1m] = */1_000,
            /*[(int)AnaChnlScaleIndex.Lv2m] = */2_000,
            /*[(int)AnaChnlScaleIndex.Lv5m] = */5_000,
            /*[(int)AnaChnlScaleIndex.Lv10m] = */10_000,
            /*[(int)AnaChnlScaleIndex.Lv20m] = */20_000,
            /*[(int)AnaChnlScaleIndex.Lv50m] = */50_000,
            /*[(int)AnaChnlScaleIndex.Lv100m] = */100_000,
            /*[(int)AnaChnlScaleIndex.Lv200m] = */200_000,
            /*[(int)AnaChnlScaleIndex.Lv500m] = */500_000,
            /*[(int)AnaChnlScaleIndex.Lv1] = */1_000_000,
            /*[(int)AnaChnlScaleIndex.Lv2] = */2_000_000,
            /*[(int)AnaChnlScaleIndex.Lv5] = */5_000_000,
            /*[(int)AnaChnlScaleIndex.Lv10] = */10_000_000,
            /*[(int)AnaChnlScaleIndex.Lv20] = */20_000_000,
            /*[(int)AnaChnlScaleIndex.Lv20] = */50_000_000,
            /*[(int)AnaChnlScaleIndex.Lv20] = */100_000_000,
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AutoCaliParamsPerChannel
    {
        public Int32 OffsetPreceding { get; set; }

        /// <summary>
        /// 前级偏正3格
        /// </summary>
        public Int32 OffsetPreceding_3Div { get; set; }

        /// <summary>
        /// 前级偏负3格
        /// </summary>
        public Int32 OffsetPreceding_N3Div { get; set; }

        /// <summary>
        /// 后级偏正3格
        /// </summary>
        public Int32 OffsetPosterior
        {
            get; set;
        }

        /// <summary>
        /// 后级偏负3格
        /// </summary>
        public Int32 OffsetPosterior_3Div
        {
            get; set;
        }

        public Int32 OffsetPosterior_N3Div
        {
            get; set;
        }

        public Int32 Gain_CoarseCtrlWord
        {
            get; set;
        }
        /// <summary>
        /// 以10000为基数，表示100%
        /// </summary>
        public Int32 Gain_FineByAdc
        {
            get; set;
        }
        /// <summary>
        /// 以1000为基数，表示100%。精度为千分之一
        /// </summary>
        public Int32 Gain_FineByFpgaThousand
        {
            get; set;
        }
        public Int32 DCTrigZero
        {
            get; set;
        }
        public Int32 DCTrigZero_3Div
        {
            get; set;
        }
        public Int32 Reserved1
        {
            get; set;
        }
        public Int32 Reserved2
        {
            get; set;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public struct ChannelPerScaleItem
    {
        public UInt32 OffsetPreceding
        {
            get;set;
        }
        public UInt32 OffsetPreceding_3Div
        {
            get; set;
        }
        public UInt32 OffsetPosterior
        {
            get; set;
        }
        public UInt32 OffsetPosterior_3Div
        {
            get; set;
        }

        public UInt32 Gain_CoarseCtrlWord
        {
            get; set;
        }
        /// <summary>
        /// 以10000为基数，表示100%
        /// </summary>
        public UInt32 Gain_FineByAdc
        {
            get; set;
        }
        /// <summary>
        /// 以1000为基数，表示100%。精度为千分之一
        /// </summary>
        public UInt32 Gain_FineByFpgaThousand
        {
            get; set;
        }
        public UInt32 DCTrigZero
        {
            get;
            set;
        }
        public UInt32 DCTrigZero_3Div
        {
            get;
            set;
        }
        public UInt32 Reserved1
        {
            get;
            set;
        }
        public UInt32 Reserved2
        {
            get;
            set;
        }
    }
}

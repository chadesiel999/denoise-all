using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public struct DbiAnalogChannelSubbandItem
    {
        /// <summary>
        /// 整数延迟
        /// </summary>
        public UInt32 IntDiscardDots
        {
            get;
            set;
        }

        /// <summary>
        /// 最前级的衰减控制字，也就是金老师的那个通道
        /// </summary>
        public UInt32 AnalogChannelGain
        {
            get;
            set;
        }
        /// <summary>
        /// 子带摸摸你通道，也就是邱老师的模拟通道
        /// </summary>
        public UInt32 SubbandGain
        {
            get;
            set;
        }
        /// <summary>
        /// 前级偏，也就是Bias，控制的是邱老师的通道
        /// </summary>
        public UInt32 BiasPreceding
        {
            get; set;
        }
        public UInt32 BiasPreceding_3Div
        {
            get; set;
        }
        /// <summary>
        /// 后级偏，也就是Offset，控制的是邱老师的
        /// </summary>
        public UInt32 OffsetPosterior
        {
            get; set;
        }
        public UInt32 OffsetPosterior_3Div
        {
            get; set;
        }
        /// <summary>
        /// 每个子带，也就是每块FPGA 中的ADC1,万分之几，10000位100%，不调整
        /// </summary>
        public UInt32 Gain_FineByAdc1ByTenThousand
        {
            get; set;
        }
        /// <summary>
        /// 每个子带，也就是每块FPGA 中的ADC2,万分之几，10000位100%，不调整
        /// </summary>
        public UInt32 Gain_FineByAdc2ByTenThousand
        {
            get; set;
        }
        /// <summary>
        /// 通过数字调整。千分之几，1000表示100%，不调整
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

        public Int32 DiscardDotsBefore
        {
            get;
            set;
        }

        public Int32 DiscardDotsAfter
        {
            get;
            set;
        }
    }
}

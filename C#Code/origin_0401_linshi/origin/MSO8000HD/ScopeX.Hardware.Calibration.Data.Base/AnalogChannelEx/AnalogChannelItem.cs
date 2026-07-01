using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public struct AnalogChannelItem
    {
        public uint Bias { get; set; }
        public uint Bias_3Div { get; set; }
        public uint Offset { get; set; }
        public uint Offset_3Div { get; set; }
        public uint Gain { get; set; }
        public uint Gain_FineByFpgaThousand { get; set; }
        public uint Gain_FineByTenThousandByAdc1 { get; set; }
        public uint Gain_FineByTenThousandByAdc2 { get; set; }
        public uint DCTrigZero { get; set; }
        public uint DCTrigZero_3Div { get; set; }
        public uint DiscardDotsBefore { get; set; }
        public uint DiscardDotsAfter { get; set; }
        public uint Reserved0 { get; set; }
        public uint Reserved1 { get; set; }
        public uint Reserved2 { get; set; }
        public uint Reserved3 { get; set; }
    }
}

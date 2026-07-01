using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base.AnalogChannelEx
{
    public struct AiAnalogChannelItem
    {
        public Int32 PreAttenuation { get; set; }

        public Int32 Subband1Freq { get; set; }
        public Int32 Subband1Attenuation { get; set; }
        public Int32 Subband1Bias { get; set; }
        public Int32 Subband1Bias3Div { get; set; }
        public Int32 Subband1Offset { get; set; }
        public Int32 Subband1Offset3Div { get; set; }
        public Int32 Subband1Ref { get; set; }
        public Int32 Subband1SampleFreq { get; set; }

        public Int32 Subband2Freq { get; set; }
        public Int32 Subband2Attenuation { get; set; }
        public Int32 Subband2SampleFreq { get; set; }

        public Int32 Subband3Freq { get; set; }
        public Int32 Subband3Attenuation { get; set; }
        public Int32 Subband3SampleFreq { get; set; }

        public Int32 Subband4Freq { get; set; }
        public Int32 Subband4Attenuation { get; set; }
        public Int32 Subband4SampleFreq { get; set; }

        public Int32 Reserved0 { get; set; }
        public Int32 Reserved1 { get; set; }
        public Int32 Reserved2 { get; set; }
        public Int32 Reserved3 { get; set; }
    }
}

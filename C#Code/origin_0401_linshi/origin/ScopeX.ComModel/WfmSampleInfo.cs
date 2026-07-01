using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    public class WfmSampleInfo
    {
        public Double SampleIntervalByus { get; set; }

        public Double StartTimeByus { get; set; }
        /// <summary>
        /// 以秒为单位，在导出数据时，时间应加入该值。
        /// </summary>
        public Double TrigErrorTime { get; set; }
        public HdMessage? HdMessage { get; set; }

        public  UInt16 PreFrameNo { get; set; }
        public  UInt16 FrameNo { get; set; }
    }
    public enum AcqDataType
    {
        AnalogChannel,
        LA,
        DPX,
        Search,
        Cymometer,
        Decode,
    }
    public record ReadInfo(AcqDataType DataType, List<ChannelId> ChannelIds, WfmPkgInfo pkgInfo, String Mark)
    {
       public String ExtInfo;
    }

    /// <summary>
    /// 采样数据包的水平信息
    /// </summary>
    /// <param name="DotsCount">采样数据点数</param>
    /// <param name="SumTimeByus">采样数据对应的采样总时间（单位：us）</param>
    /// <param name="StartTimeByus">与TimebaseOptions中的StartTimeByus含义相同，触发点相对屏幕正中央（时基档*总格数/2）的偏移时间(单位：us)，向左为正，向右为负</param>
    public record WfmPkgInfo(Int64 DotsCount, Double SumTimeByus, Double StartTimeByus);
    public record ExceptionData(ChannelId chnlId, List<UInt32> frameIds);

    public record WfmMdInfo()
    {
        public Double RBWByHz { get; init; }

        public Int64 FFTLength { get; init; }

        public Double SampleRateHardware { get; init; }

        public Double RBWHardware { get; init; }

        public Int64 SpanHardware { get; init; }

        public Int64 SpanSync { get; init; }

        public Int64 FFTLengthSync { get; init; }

        public Int64 SpanParamTuning { get; set; }

        public Int64 CenterFreqParamTuning { get; set; }

        public Int64 StateParamTuning { get; set; }
    }
}

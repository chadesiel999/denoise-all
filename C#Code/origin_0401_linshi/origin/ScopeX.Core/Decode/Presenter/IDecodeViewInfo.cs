using System;
using System.Drawing;

namespace ScopeX.Core.Decode
{
    /// <summary>
    /// 协议解析结果标签绘制相关信息
    /// </summary>
    public interface IDecodeViewInfo : IDecodePackets
    {
        public Color ForeColor { get; }
        public Color BorderColor { get; }
        public Byte[] Data { get; }
        public UInt32 BitCount { get; }
        public String Title { get; }
        public String ErrorInfo { get; }
        public String ShowStr { get; }
        public Byte[] ErrorInfoData { get; }
        public UInt32 ErrorInfoBitCount { get; }
    }
    public sealed class DecodeResultData
    {
        public String Name { get; internal set; } = String.Empty;
        public IDecodeViewInfo[] DecodeViewInfos { get; internal set; } = new IDecodeViewInfo[0];
    }
}

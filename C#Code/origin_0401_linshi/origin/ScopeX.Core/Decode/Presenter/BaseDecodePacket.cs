using System;
using System.Drawing;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public abstract class BaseDecodePacket : IDecodeViewInfo
    {
        public BaseDecodePacket(Single start, Single lenght)
        {
            Start = start;
            Lenght = lenght;
        }
        public Single Start { get; }
        public Single Lenght { get; }
        public abstract Boolean IsInfoPacket { get; }
        public virtual Color ForeColor { get; init; } = Color.White;
        public virtual Color BorderColor { get; init; }
        public virtual Byte[] Data { get; init; } = new Byte[0];
        public virtual UInt32 BitCount { get; init; }
        public virtual Byte[] ErrorInfoData { get; init; } = new Byte[0];
        public virtual UInt32 ErrorInfoBitCount { get; init; }

        public virtual String Title { get; init; } = String.Empty;
        public virtual String ErrorInfo { get; } = String.Empty;
        public virtual String ShowStr { get; init; } = String.Empty;
        public abstract SerialProtocolType ProtocolType { get; }
    }
}

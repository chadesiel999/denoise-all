using System;
using System.Drawing;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public abstract class NRZDecodePacket : BaseDecodePacket
    {
        public NRZDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SerialProtocolType ProtocolType => SerialProtocolType.NRZ;

    }
}

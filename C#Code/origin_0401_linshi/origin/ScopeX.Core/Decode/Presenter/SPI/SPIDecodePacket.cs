using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public abstract class SPIDecodePacket : BaseDecodePacket
    {
        public SPIDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public override SerialProtocolType ProtocolType => SerialProtocolType.SPI;
        public virtual SPIPacketType PacketType { get; }

        public override Boolean IsInfoPacket => PacketType != SPIPacketType.Data;
    }

    public sealed class SPIStartDecodePacket : SPIDecodePacket
    {
        public SPIStartDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SPIPacketType PacketType => SPIPacketType.Start;
        public override Byte[] Data => Encoding.Default.GetBytes("Start");
        public override Color BorderColor => Color.Green;
    }
    public sealed class SPIDataDecodePacket : SPIDecodePacket
    {
        public SPIDataDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SPIPacketType PacketType => SPIPacketType.Data;
        public override String Title => "Data";
        public override Color BorderColor => BitLost ? Color.Red : ColorTranslator.FromHtml("#0xFF008080");
        public Int32 MaxBitCount { get; set; }
        public Boolean BitLost => MaxBitCount > BitCount;
        public override String ErrorInfo => BitLost ? (MaxBitCount - BitCount) + "Bit Lost" : String.Empty;
    }
    public sealed class SPIEndDecodePacket : SPIDecodePacket
    {
        public SPIEndDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SPIPacketType PacketType => SPIPacketType.End;
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("End");
        public override Color BorderColor => Color.Red;
    }
    public enum SPIPacketType
    {
        Start,
        Data,
        End,
    }
}

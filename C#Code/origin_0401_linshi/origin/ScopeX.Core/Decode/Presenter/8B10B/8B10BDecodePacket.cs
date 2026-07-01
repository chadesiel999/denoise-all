using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public class D8B10BDecodePacket : BaseDecodePacket
    {
        public D8B10BDecodePacket(Single start, Single lenght, D8B10BPacketType packetType) : base(start, lenght)
        {
            _PacketType = packetType;
        }

        public String _Title = "";
        public UInt32 _BitCount = 1;
        public D8B10BPacketType _PacketType = D8B10BPacketType.Data;//包类型
        public D8B10BPacketType PacketType => _PacketType;
        public D8B10BPacketError PacketError { get; init; }
        public override SerialProtocolType ProtocolType => SerialProtocolType.Common_8b10b;
       
        public override String ErrorInfo => PacketError switch
        {
            D8B10BPacketError.NoError => string.Empty,
            D8B10BPacketError.D6bitDisparityError => "6Bit Disparity Error",
            D8B10BPacketError.D4bitDisparityError => "4Bit Disparity Error",
            D8B10BPacketError.SymbolError => "Symbol Error",
            _ => string.Empty,
        };
        public override String Title => _Title;
        public override UInt32 BitCount => _BitCount;
        public override Color BorderColor => PacketType switch
        {
            D8B10BPacketType.Data => Color.Blue,
            D8B10BPacketType.Kcode => Color.Green,
            D8B10BPacketType.Error => Color.Red,
            _ => ColorTranslator.FromHtml("#0xff007f7f"),
        };
        public override Boolean IsInfoPacket => false;
    }

    public enum D8B10BPacketType
    {
        Data,
        Kcode,
        Error,
    }

    public enum D8B10BPacketError
    { 
        NoError,
        D6bitDisparityError,
        D4bitDisparityError,
        SymbolError,
    }

}

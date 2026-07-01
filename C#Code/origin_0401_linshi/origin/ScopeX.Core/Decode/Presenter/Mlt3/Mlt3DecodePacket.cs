using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public class Mlt3DecodePacket : BaseDecodePacket
    {
        public Mlt3DecodePacket(Single start, Single lenght, Mlt3PacketType packetType) : base(start, lenght)
        {
            _PacketType = packetType;
        }

        public String _Title = "";
        public UInt32 _BitCount = 1;
        public Mlt3PacketError PacketError { get; init; }
        public override SerialProtocolType ProtocolType => SerialProtocolType.Mlt3;

        public Mlt3PacketType _PacketType = Mlt3PacketType.ZeroPacket;//包类型
        public Mlt3PacketType PacketType => _PacketType;
        public override String ErrorInfo => PacketError switch
        {
            Mlt3PacketError.NoError => string.Empty,
            Mlt3PacketError.MLT3_ERROR_JUMP_ILLEGAL => "Jump Illegal",
            Mlt3PacketError.MLT3_ERROR_JUMP_FAST => "Jump Fast",
            Mlt3PacketError.MLT3_ERROR_ZERO_TOOMUCH => "Too Many Zero Level",
            _ => string.Empty,
        };
        public override UInt32 BitCount => _BitCount;
        public override String Title => _Title;
        public override Color BorderColor => PacketType switch
        {
            Mlt3PacketType.ZeroPacket => Color.Green,
            Mlt3PacketType.OnePacket => ColorTranslator.FromHtml("#0xff007f7f"),
            Mlt3PacketType.Errror => Color.Red,
            _ => ColorTranslator.FromHtml("#0xff007f7f"),
        };
        public override Boolean IsInfoPacket => false;

    }

    public enum Mlt3PacketError
    { 
        NoError,
        MLT3_ERROR_JUMP_ILLEGAL, // 跳变错误 0电平前后电平一致
        MLT3_ERROR_JUMP_FAST,    // 跳变过快 电平从 -1 到 1
        MLT3_ERROR_ZERO_TOOMUCH, // 0电平过多
    }

    public enum Mlt3PacketType
    { 
        ZeroPacket,
        OnePacket,
        Errror,
    }


}

using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public class EthernetDecodePacket : BaseDecodePacket
    {
        public EthernetDecodePacket(Single start, Single lenght, EthernetPacketType packetType) : base(start, lenght)
        {
            _PacketType = packetType;
        }

        public String _Title = "";
        public UInt32 _BitCount = 1;
        public EthernetPacketType _PacketType = EthernetPacketType.Start;

        public EthernetPacketType PacketType => _PacketType;
        public override SerialProtocolType ProtocolType => SerialProtocolType.Ethernet;
        public override String Title => _Title;
        public override UInt32 BitCount => _BitCount;
        public override Color BorderColor => PacketType switch
        {
            EthernetPacketType.Start => Color.Green,
            EthernetPacketType.Data => ColorTranslator.FromHtml("#0xff007f7f"),
            EthernetPacketType.INFO => Color.Yellow,
            EthernetPacketType.Preamble => Color.Purple,
            EthernetPacketType.SSD => Color.Purple,
            _=> ColorTranslator.FromHtml("#0xff007f7f"),
        };
        public override Boolean IsInfoPacket => PacketType switch
        {
            EthernetPacketType.Start => true,
            EthernetPacketType.End => true,
            _ => false,
        };
    }
}

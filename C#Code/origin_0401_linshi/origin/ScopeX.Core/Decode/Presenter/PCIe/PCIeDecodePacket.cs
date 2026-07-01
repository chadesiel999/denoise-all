using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public enum PCIETextType
    {
        FIELD_PACKET_TYPE,  // 包类型 
        FIELD_START,        // 包开始
        FIELD_TRANSACTION_ID, // 包传输id
        FIELD_END,            // 包结束
    }

    public class PCIEDecodePacket : BaseDecodePacket
    {
        public PCIEDecodePacket(Single start, Single lenght, PCIEPacketType packetType) : base(start, lenght)
        {
            _PacketType = packetType;
        }


        public String _Title = "";
        public UInt32 _BitCount = 1;
        public PCIEPacketType _PacketType;

        public PCIEPacketType PacketType => _PacketType;
        public override SerialProtocolType ProtocolType => SerialProtocolType.PCIe;
        public override String Title => _Title;
        public override UInt32 BitCount => _BitCount;

        public override Boolean IsInfoPacket { get; }

        //public override Color BorderColor => PacketType switch
        //{
        //    EthernetPacketType.Start => Color.Green,
        //    EthernetPacketType.Data => ColorTranslator.FromHtml("#0xff007f7f"),
        //    EthernetPacketType.INFO => Color.Yellow,
        //    EthernetPacketType.Preamble => Color.Purple,
        //    EthernetPacketType.SSD => Color.Purple,
        //    _ => ColorTranslator.FromHtml("#0xff007f7f"),
        //};
        //public override Boolean IsInfoPacket => PacketType switch
        //{
        //    //EthernetPacketType.Start => true,
        //    //EthernetPacketType.End => true,
        //    //_ => false,
        //};
    }
}

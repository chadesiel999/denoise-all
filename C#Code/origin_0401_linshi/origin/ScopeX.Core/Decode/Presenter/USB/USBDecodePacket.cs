using System;
using System.Drawing;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public class USBDecodePacket : BaseDecodePacket
    {
        public USBDecodePacket(Single start, Single lenght, USBDecodePacketTypeNew packetType, USBPacketError packetError = USBPacketError.NO_ERROR) : base(start, lenght)
        {
            PacketType = packetType;
            _PacketError = packetError;
        }
        //public abstract USBDecodePacketType PacketType { get; }
        public override SerialProtocolType ProtocolType => SerialProtocolType.USB;
        
        public USBPacketError _PacketError = USBPacketError.NO_ERROR;
        public USBDecodePacketTypeNew _PacketType;
        public USBDecodePacketTypeNew PacketType
        {
            get
            {
                return _PacketType;
            }
            set
            {
                if (_PacketType != value)
                {
                    _PacketType = value;
                }
            }
        }

        public String _Title = "";
        public UInt32 _BitCount = 1;

        public override String Title => _Title;
        public override UInt32 BitCount => _BitCount;

        public override Color BorderColor => PacketType switch
        {
            USBDecodePacketTypeNew.SYNC => _PacketError == USBPacketError.SYNC_ERROR ? Color.Red : Color.Green,
            USBDecodePacketTypeNew.PID => _PacketError == USBPacketError.PID_ERROR ? Color.Red : Color.Yellow,
            USBDecodePacketTypeNew.ADDR => Color.Yellow,
            USBDecodePacketTypeNew.ENDP => Color.Yellow,
            USBDecodePacketTypeNew.FRAMENUB => Color.Yellow,
            USBDecodePacketTypeNew.DATA => Color.Cyan,
            USBDecodePacketTypeNew.CRC => _PacketError == USBPacketError.CRC_ERROR ? Color.Red : Color.Purple,
            USBDecodePacketTypeNew.EOP => Color.Red,
            _ => ColorTranslator.FromHtml("#0xff007f7f"),
        };

        public override Boolean IsInfoPacket => PacketType switch
        {
            //不带数据
            USBDecodePacketTypeNew.SYNC => true,
            USBDecodePacketTypeNew.PID => true,
            USBDecodePacketTypeNew.EOP => true,
            _ => false,
        };
    }
}

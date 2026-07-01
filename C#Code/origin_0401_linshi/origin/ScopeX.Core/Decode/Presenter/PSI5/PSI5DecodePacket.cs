using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public class PSI5DecodePacket : BaseDecodePacket
    {
        public PSI5DecodePacket(Single start, Single lenght, PSI5FieldType packetType) : base(start, lenght)
        {
            _PacketType = packetType;
        }

        public String _Title = "";
        public UInt32 _BitCount = 1;
        public PSI5FieldType _PacketType = PSI5FieldType.FIELD_START_BIT;
        public PSI5FieldType PacketType
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

        public override SerialProtocolType ProtocolType => SerialProtocolType.PSI5;
        public override String Title => _Title;
        public override UInt32 BitCount => _BitCount;

        public override Color BorderColor => PacketType switch
        {
            PSI5FieldType.FIELD_START_BIT => Color.Green,
            PSI5FieldType.FIELD_SERIAL_MESSAGE => Color.Yellow,
            PSI5FieldType.FIELD_FRAME_CONTROL => Color.Yellow,
            PSI5FieldType.FIELD_STATUS => Color.Yellow,
            PSI5FieldType.FIELD_DATA_B => Color.Cyan,
            PSI5FieldType.FIELD_DATA_A => Color.Cyan,
            PSI5FieldType.FIELD_DATA_A_REST => Color.Cyan,
            PSI5FieldType.FIELD_DATA_A_INIT => Color.Cyan,
            PSI5FieldType.FIELD_VERIFY_CRC => ColorTranslator.FromHtml("#0xFF800000"),
            _ => ColorTranslator.FromHtml("#0xff007f7f"),
        };

        public override Boolean IsInfoPacket => PacketType switch
        {
            //PSI5FieldType.FIELD_START_BIT => true,
            _ => false,
        };

        //public IReadOnlyList<String> ErrorInfos { get; } = new List<String>()
        //{
        //    "No Error",
        //    "Parity Error",
        //    "Unsupported Command",
        //    "Unsupported Address",
        //}.AsReadOnly();


    };
}


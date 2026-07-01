using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public class I3CDecodePacket : BaseDecodePacket
    {
        public I3CDecodePacket(Single start, Single lenght, I3CFieldType packetType) : base(start, lenght)
        {
            _PacketType = packetType;
        }

        public String _Title = "";
        public UInt32 _BitCount = 1;
        public I3CFieldType _PacketType = I3CFieldType.FIELD_TYPE_RESTART;
        public I3CFieldType PacketType
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

        public override SerialProtocolType ProtocolType => SerialProtocolType.I3C;
        public override String Title => _Title;
        public override UInt32 BitCount => _BitCount;

        public override Color BorderColor => PacketType switch
        {
            I3CFieldType.FIELD_TYPE_START   => Color.Green,
            I3CFieldType.FIELD_TYPE_RESTART => Color.Green,
            I3CFieldType.FIELD_TYPE_END     => Color.Red,
            I3CFieldType.FIELD_TYPE_ADDR    => Color.Yellow,
            I3CFieldType.FIELD_TYPE_COMMAND => Color.Yellow,
            I3CFieldType.FIELD_TYPE_DATA    => Color.Cyan,
            I3CFieldType.FIELD_TYPE_EXPAND  => Color.Purple,
            I3CFieldType.FIELD_TYPE_ERROR   => Color.Red,
            _ => ColorTranslator.FromHtml("#0xff007f7f"),
        };

        public override Boolean IsInfoPacket => PacketType switch
        {
            I3CFieldType.FIELD_TYPE_START => true,
            I3CFieldType.FIELD_TYPE_RESTART => true,
            I3CFieldType.FIELD_TYPE_END => true,
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


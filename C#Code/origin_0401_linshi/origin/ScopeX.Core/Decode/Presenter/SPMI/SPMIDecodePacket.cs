using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public class SPMIDecodePacket : BaseDecodePacket
    {
        public SPMIDecodePacket(Single start, Single lenght, SPMIFieldType packetType) : base(start, lenght)
        {
            _PacketType = packetType;
        }

        public String _Title = "";
        public UInt32 _BitCount = 1;
        public SPMIFieldType _PacketType = SPMIFieldType.FIELD_ARBITRATION_START;
        public SPMIFieldType PacketType
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

        public override SerialProtocolType ProtocolType => SerialProtocolType.SPMI;
        public override String Title => _Title;
        public override UInt32 BitCount => _BitCount;

        public override Color BorderColor => PacketType switch
        {
            SPMIFieldType.FIELD_ARBITRATION_START => Color.Yellow,
            SPMIFieldType.FIELD_CONNECT_BIT       => Color.Purple,
            SPMIFieldType.FIELD_ALERT_BIT         => Color.Yellow,
            SPMIFieldType.FIELD_SR_BIT            => Color.Yellow,
            SPMIFieldType.FIELD_MASTER_ID         => Color.Purple,
            SPMIFieldType.FIELD_PRIMARY_LEVEL     => Color.Gray,
            SPMIFieldType.FIELD_SECONDARY_LEVEL   => Color.Gray,
            SPMIFieldType.FIELD_SLAVE_ADDRESS     => Color.Yellow,
            SPMIFieldType.FIELD_COMMAND_START     => Color.Green,
            SPMIFieldType.FIELD_COMMAND_TYPE      => Color.Yellow,
            SPMIFieldType.FIELD_COMMAND_ADDRESS   => Color.Yellow,
            SPMIFieldType.FIELD_DATA              => Color.Cyan,
            SPMIFieldType.FIELD_PARITY            => Color.Purple,
            SPMIFieldType.FIELD_ACKNACK           => Color.Purple,
            SPMIFieldType.FIELD_ERROR             => ColorTranslator.FromHtml("#0xFF800000"),
            _ => ColorTranslator.FromHtml("#0xff007f7f"),
        };

        public override Boolean IsInfoPacket => PacketType switch
        {
            SPMIFieldType.FIELD_ARBITRATION_START =>true,
            SPMIFieldType.FIELD_COMMAND_START => true,
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


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public abstract class FlexRayDecodePacket : BaseDecodePacket
    {
        public FlexRayDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SerialProtocolType ProtocolType => SerialProtocolType.FlexRay;
        public abstract FlexRayPacketType PacketType { get; }

    }
    public class FlexRayStartDecodePacket : FlexRayDecodePacket
    {
        public FlexRayStartDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override FlexRayPacketType PacketType => FlexRayPacketType.Start;
        public override Boolean IsInfoPacket => true;
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("Start");
        public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF00FF00");
    }

    public class FlexRayIndicatorDecodePacket : FlexRayDecodePacket
    {
        public FlexRayIndicatorDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override FlexRayPacketType PacketType => FlexRayPacketType.Indicator;
        public override Boolean IsInfoPacket => false;

        public String Error { get; set; } = String.Empty;

        public override String ErrorInfo => Error ;

        public override Color BorderColor  => !String.IsNullOrEmpty(Error) ? Color.Red : ColorTranslator.FromHtml("#0xff400080");
        public override String Title => "Indicator";
    }
    public class FlexRayFrameIDDecodePacket : FlexRayDecodePacket
    {
        public FlexRayFrameIDDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Color BorderColor => ColorTranslator.FromHtml("#0xFF808000");
        public override FlexRayPacketType PacketType => FlexRayPacketType.FrameID;
        public override Boolean IsInfoPacket => false;
        public override String Title => "FrameID";
    }
    public class FlexRayPayloadLengthDecodePacket : FlexRayDecodePacket
    {
        public FlexRayPayloadLengthDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override FlexRayPacketType PacketType => FlexRayPacketType.PayloadLength;
        public override Boolean IsInfoPacket => false;
        public override String Title => "PayloadLength";
        public override Color BorderColor => ColorTranslator.FromHtml("#0xFF8A2BE2");
    }
    public class FlexRayHeaderCRCDecodePacket : FlexRayDecodePacket
    {
        public FlexRayHeaderCRCDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public Byte[] CRC { get; set; } = new Byte[0];

        public Boolean Error { get; set; } = false;

        public override String ErrorInfo => Error ? "HeaderCRC Error,calculated :" + BitConverter.ToString(CRC).Replace("-", String.Empty) : String.Empty;

        public override Color BorderColor => Error ? Color.Red : ColorTranslator.FromHtml("#0xFF808000");

        public override Boolean IsInfoPacket => false;
        public override String Title => "HeaderCRC";
        public override FlexRayPacketType PacketType => FlexRayPacketType.HeaderCRC;
    }

    public class FlexRayCycleCountDecodePacket : FlexRayDecodePacket
    {
        public FlexRayCycleCountDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Color BorderColor => ColorTranslator.FromHtml("#0xFF8A2BE2");
        public override Boolean IsInfoPacket => false;
        public override String Title => "CycleCount";
        public override FlexRayPacketType PacketType => FlexRayPacketType.CycleCount;
    }
    public class FlexRayDataDecodePacket : FlexRayDecodePacket
    {
        public FlexRayDataDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override FlexRayPacketType PacketType => FlexRayPacketType.Data;

        public Boolean Error { get; set; } = false;

        public override String ErrorInfo => Error ? "Data Error" : String.Empty;

        public override Color BorderColor => Error ? Color.Red : ColorTranslator.FromHtml("#00FFFF");
        public override Boolean IsInfoPacket => false;
        public override String Title => "Data";
    }

    public class FlexRayFrameCRCDecodePacket : FlexRayDecodePacket
    {
        public FlexRayFrameCRCDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public Byte[] CRC { get; set; } = new Byte[0];

        public Boolean Error { get; set; } = false;

        public override String ErrorInfo => Error ? "FrameCRC Error,calculated :" + BitConverter.ToString(CRC).Replace("-", String.Empty) : String.Empty;

        public override Color BorderColor => Error ? Color.Red : ColorTranslator.FromHtml("#0xFF808000");
        public override Boolean IsInfoPacket => false;
        public override String Title => "FrameCRC";
        public override FlexRayPacketType PacketType => FlexRayPacketType.FrameCRC;
    }
    public class FlexRayFrameEndDecodePacket : FlexRayDecodePacket
    {
        public FlexRayFrameEndDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override FlexRayPacketType PacketType => FlexRayPacketType.FrameEnd;
        public override Boolean IsInfoPacket => true;
        public override Color BorderColor => Color.Red;
        public override Byte[] Data => Encoding.Default.GetBytes("End");
    }

    public class FlexRayDTSDecodePacket : FlexRayDecodePacket
    {
        public FlexRayDTSDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Color BorderColor => Color.Blue;
        public override Boolean IsInfoPacket => false;
        public override String Title => "DTS";
        public override FlexRayPacketType PacketType => FlexRayPacketType.DTS;
    }

    public class FlexRayCIDDecodePacket :FlexRayDecodePacket
    {
        public FlexRayCIDDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public Boolean Error { get; set; } = false;

        public override String ErrorInfo => Error ? "CID Error" : String.Empty;

        public override Color BorderColor => Error ? Color.Red : Color.Blue;
        public override Boolean IsInfoPacket => false;
        public override String Title => "CID";
        public override FlexRayPacketType PacketType => FlexRayPacketType.DTS;
    }

    public enum FlexRayPacketType
    {
        Start,
        Indicator,
        FrameID,
        PayloadLength,
        HeaderCRC,
        CycleCount,
        Data,
        FrameCRC,
        FrameEnd,
        DTS,
    }
}

using System;
using System.Drawing;
using System.Text;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public abstract class SENTDecodePacket : BaseDecodePacket
    {
        public SENTDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SerialProtocolType ProtocolType => SerialProtocolType.SENT;
        public abstract SENTPacketType PacketType { get; }
    }

    public class SENTSyncDecodePacket : SENTDecodePacket
    {
        public SENTSyncDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SENTPacketType PacketType => SENTPacketType.Sync;
        public override Boolean IsInfoPacket => true;
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("Sync");
        public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF008000");
    }

    public class SENTStatusDecodePacket : SENTDecodePacket
    {
        public SENTStatusDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SENTPacketType PacketType => SENTPacketType.Status;
        public override Boolean IsInfoPacket => true;

        public override string ShowStr => "Status";

        public override String Title => "Status";
        public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF8A2BE2");
    }

    public class SENTDataDecodePacket : SENTDecodePacket
    {
        public SENTDataDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SENTPacketType PacketType => SENTPacketType.Data;
        public override Boolean IsInfoPacket => false;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override String Title => "Data";
        public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF008080");
    }

    public class SENTCRCDecodePacket : SENTDecodePacket
    {
        public SENTCRCDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SENTPacketType PacketType => SENTPacketType.CRC;
        public override Boolean IsInfoPacket => false;

        public Boolean HasCRCError = false;

        public override string ErrorInfo => HasCRCError ? "CRC Error:Calculated {0}" : String.Empty;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override String Title => "CRC";
        public override Color BorderColor => HasCRCError ? Color.Red : ColorTranslator.FromHtml("#0xFF00FF00");
    }

    public class SENTPauseDecodePacket : SENTDecodePacket
    {
        public SENTPauseDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SENTPacketType PacketType => SENTPacketType.Pause;
        public override Boolean IsInfoPacket => true;

        public Int16 PauseValue;
        public override string ShowStr => $"Pause";
        public override String Title => $"Pause";
        public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF8A2BE2");
    }

    public class SENTErrorDecodePacket : SENTDecodePacket
    {
        public SENTErrorDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SENTPacketType PacketType => SENTPacketType.Error;
        public override Boolean IsInfoPacket => false;


        public override Byte[] Data { get; init; } = new Byte[0];


        //public override String Title => "Error";
        public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF800000");
    }


    public class SENTStartDecodePaket : SENTDecodePacket
    {
        public SENTStartDecodePaket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SENTPacketType PacketType => SENTPacketType.SlowStart;
        public override Boolean IsInfoPacket => true;
        public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF008000");
    }

    public class SENTSlowIDDecodePacket : SENTDecodePacket
    {
        public SENTSlowIDDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SENTPacketType PacketType => SENTPacketType.SlowID;
        public override Boolean IsInfoPacket => false;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override String Title => "ID";
        public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF8A2BE2");
    }

    public class SENTSlowDataDecodePacket : SENTDecodePacket
    {
        public SENTSlowDataDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SENTPacketType PacketType => SENTPacketType.SlowData;
        public override Boolean IsInfoPacket => false;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override String Title => "Data";
        public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF008080");
    }

    public class SENTSlowCRCDecodePacket : SENTDecodePacket
    {
        public SENTSlowCRCDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SENTPacketType PacketType => SENTPacketType.SlowData;
        public override Boolean IsInfoPacket => false;

        public Boolean HasCRCError = false;

        public override string ErrorInfo => HasCRCError ? "CRC Error:Calculated {0}" : String.Empty;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override String Title => "CRC";
        public override Color BorderColor => HasCRCError ? Color.Red : ColorTranslator.FromHtml("#0xFF00FF00");
    }



    public enum SENTPacketType
    {
        Sync,
        Status,
        Data,
        CRC,
        Pause,
        Error,
        SlowStart,
        SlowID,
        SlowData,
        SlowCRC,
    }
}

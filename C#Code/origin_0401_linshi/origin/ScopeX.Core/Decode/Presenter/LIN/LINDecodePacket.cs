using System;
using System.Drawing;
using System.Text;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public abstract class LINDecodePacket : BaseDecodePacket
    {
        public LINDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SerialProtocolType ProtocolType => SerialProtocolType.LIN;
        public abstract LINPacketType PacketType { get; }
    }

    public class LINStartDecodePacket : LINDecodePacket
    {
        public LINStartDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override LINPacketType PacketType => LINPacketType.Start;
        public override Boolean IsInfoPacket => true;
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("Start");
        public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF00FF00");
    }

    public class LINSyncDecodePacket : LINDecodePacket
    {
        public LINSyncDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "Sync";
        public override LINPacketType PacketType => LINPacketType.Sync;
        public override Boolean IsInfoPacket => false;
        public Boolean SyncError;
        public override String ErrorInfo => SyncError ? "Sync Error" : String.Empty;
        public override Byte[] Data { get; init; }
        public override Color BorderColor => SyncError ? Color.Red : ColorTranslator.FromHtml("#0xFF008000");
    }

    public class LINPIDDecodePacket : LINDecodePacket
    {
        public LINPIDDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override LINPacketType PacketType => LINPacketType.PID;
        public override Boolean IsInfoPacket => false;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override String Title { get; init; } = "PID";

        public Boolean PIDParityError;
        public override String ErrorInfo => PIDParityError ? "Parity Error" : String.Empty;
        public override Color BorderColor => PIDParityError ? Color.Red : ColorTranslator.FromHtml("#0xFF8A2BE2");
    }

    public class LINDataDecodePacket : LINDecodePacket
    {
        public LINDataDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override LINPacketType PacketType => LINPacketType.Data;
        public override Boolean IsInfoPacket => false;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override String Title => "Data";
        public Boolean DataError;
        public override String ErrorInfo => DataError ? "Data Error" : String.Empty;
        public override Color BorderColor => DataError ? Color.Red : ColorTranslator.FromHtml("#0xFF008080");
    }

    public class LINChecksumDecodePacket : LINDecodePacket
    {
        public LINChecksumDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override LINPacketType PacketType => LINPacketType.Checksum;
        public override Boolean IsInfoPacket => false;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override String Title => "Checksum";
        public Boolean ChecksumError;
        public override String ErrorInfo => ChecksumError ? "Checksum Error" : String.Empty;
        public override Color BorderColor => ChecksumError ? Color.Red : ColorTranslator.FromHtml("#0xFF8A2BE2");
    }

    public class LINErrorDecodePacket : LINDecodePacket
    {
        public LINErrorDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override LINPacketType PacketType => LINPacketType.Error;
        public override Boolean IsInfoPacket => false;


        public override Byte[] Data { get; init; } = new Byte[0];


        //public override String Title => "Error";
        public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF800000");
    }

    public enum LINPacketType
    {
        Start,
        Sync,
        PID,
        Data,
        Checksum,
        Error,
    }
}

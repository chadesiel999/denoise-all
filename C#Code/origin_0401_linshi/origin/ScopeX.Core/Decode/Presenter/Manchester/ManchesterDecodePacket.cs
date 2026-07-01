using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public abstract class ManchesterDecodePacket :BaseDecodePacket
    {
        public ManchesterDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public UInt32 _BitCount = 1;
        public override SerialProtocolType ProtocolType => SerialProtocolType.Manchester;
            
        public abstract ManchesterPacketType PacketType { get; }
        public override uint BitCount => _BitCount;

        public IReadOnlyList<String> ErrorInfos { get; } = new List<String>()
        {
            "No Error",
            "Unfamed Error",
            "Sync Error",
            "Header Error",
            "Data Error",
            "Trailer Error",
            "Parity Bit Error",
        }.AsReadOnly();
    }

    public class ManchesterSyncDecodePacket : ManchesterDecodePacket
    {
        public ManchesterSyncDecodePacket(Single start, Single lenght) : base(start, lenght)
        { 
        }
        public override String Title => "Sync";
        public override ManchesterPacketType PacketType => ManchesterPacketType.Sync;
        public override Boolean IsInfoPacket => false;

  
        public Boolean SyncError;
        public override String ErrorInfo => SyncError ? "Error:Bad manchester enoding" : String.Empty;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override Color BorderColor => SyncError ? Color.Red : ColorTranslator.FromHtml("#0xFF008000");
    }

    public class ManchesterHeaderDecodePacket : ManchesterDecodePacket
    {
        public ManchesterHeaderDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "Header";
        public override ManchesterPacketType PacketType => ManchesterPacketType.Header;
        public override Boolean IsInfoPacket => false;

        public Boolean HeaderError;
        public override String ErrorInfo => HeaderError ? "Error:Bad manchester enoding" : String.Empty;
        public override Byte[] Data { get; init; } = new byte[0];
        public override Color BorderColor => HeaderError ? Color.Red : ColorTranslator.FromHtml("#FFCC9900");
    }

    public class ManchesterDataDecodePacket : ManchesterDecodePacket
    {
        public ManchesterDataDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "Data";
        public override ManchesterPacketType PacketType => ManchesterPacketType.Data;
        public override Boolean IsInfoPacket => false;

        public Boolean DataError;
        public override String ErrorInfo => DataError ? "Error:Bad manchester enoding" : String.Empty;
        public override Byte[] Data { get; init; } = new byte[0];

        
        public override Color BorderColor => DataError ? Color.Red : ColorTranslator.FromHtml("#FF00008B");
    }

    public class ManchesterTrailerDecodePacket : ManchesterDecodePacket
    {
        public ManchesterTrailerDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "Trailer";
        public override ManchesterPacketType PacketType => ManchesterPacketType.Trailer;
        public override Boolean IsInfoPacket => false;

        public Boolean TrailerError;
        public override String ErrorInfo => TrailerError ? "Error:Bad manchester enoding" : String.Empty;
        public override Byte[] Data { get; init; } = new byte[0];
        public override Color BorderColor => TrailerError ? Color.Red : ColorTranslator.FromHtml("#FFCC9900");
    }
    public class ManchesterParityBitDecodePacket : ManchesterDecodePacket
    {
        public ManchesterParityBitDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "Parity";
        public override ManchesterPacketType PacketType => ManchesterPacketType.Parity;
        public override Boolean IsInfoPacket => false;

        public Boolean ParityError;
        public override String ErrorInfo => ParityError ? "Error:Parity" : String.Empty;
        public override Byte[] Data { get; init; } = new byte[0];
        public override Color BorderColor => ParityError ? Color.Red : ColorTranslator.FromHtml("#FFCC9900");
    }
    public class ManchesterUnframedDecodePacket : ManchesterDecodePacket
    {
        public ManchesterUnframedDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "Unframed Error";

        public override byte[] Data { get; init; } = Encoding.Default.GetBytes("Unframed error");

        public override ManchesterPacketType PacketType => ManchesterPacketType.Unframed;
        public override Boolean IsInfoPacket => true;
        
        public override Color BorderColor => Color.Red;
    }

 
    public enum ManchesterPacketType
    {
        Sync,
        Header,
        Data,
        Trailer,
        Parity,
        Unframed,
    }


}

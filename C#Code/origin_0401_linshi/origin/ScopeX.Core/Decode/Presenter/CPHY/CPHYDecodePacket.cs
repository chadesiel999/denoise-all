using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.POIFS.Crypt.Dsig;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode;

public abstract class CPHYDecodePacket : BaseDecodePacket
{
    public CPHYDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }

    public abstract CPHYPacketType PacketType { get; }
    public override SerialProtocolType ProtocolType => SerialProtocolType.CPHY;
}

public sealed class CPHYStartDecodePacket : CPHYDecodePacket
{
    public CPHYStartDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Boolean IsInfoPacket => false;
    public override CPHYPacketType PacketType => CPHYPacketType.Sync;
    public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("Sync");
    public override Color BorderColor => Color.Green;

}

public sealed class CPHYVirtualChannelDecodePacket : CPHYDecodePacket
{
    public CPHYVirtualChannelDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Boolean IsInfoPacket => false;
    public override CPHYPacketType PacketType => CPHYPacketType.Sync;
    public override Byte[] Data { get; init; } /*= Encoding.Default.GetBytes("VirtualChannel");*/
    public override Color BorderColor => Color.Green;
    public override String Title => "Virtual Channel";
    public override UInt32 BitCount => 16;

}
public sealed class CPHYDataTypeDecodePacket : CPHYDecodePacket
{
    public CPHYDataTypeDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Byte[] Data { get; init; } /*= Encoding.Default.GetBytes("DataType");*/
    public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
    public override Boolean IsInfoPacket => false;
    public override CPHYPacketType PacketType => CPHYPacketType.DataType;
    public override String Title => "Data Type";
    public override UInt32 BitCount => 16;
}
public sealed class CPHYWordCountDecodePacket : CPHYDecodePacket
{
    public CPHYWordCountDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Byte[] Data { get; init; } /*= Encoding.Default.GetBytes("WordCount");*/
    public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
    public override Boolean IsInfoPacket => false;
    public override CPHYPacketType PacketType => CPHYPacketType.WordCount;
    public override String Title => "Word Count";
    public override UInt32 BitCount => 32;
}
public sealed class CPHYDataDecodePacket : CPHYDecodePacket
{
    public CPHYDataDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Byte[] Data { get; init; } /*= Encoding.Default.GetBytes("Data");*/
    public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
    public override Boolean IsInfoPacket => false;
    public override CPHYPacketType PacketType => CPHYPacketType.Data;
    public override String Title => "Data";
    public override UInt32 BitCount { get; init; }
}
public sealed class CPHYPDCRCDecodePacket : CPHYDecodePacket
{
    public CPHYPDCRCDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Byte[] Data { get; init; } /*= Encoding.Default.GetBytes("CRC");*/
    public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
    public override Boolean IsInfoPacket => false;
    public override CPHYPacketType PacketType => CPHYPacketType.PDCRC;
    public override String Title => "CRC LSB MSB";
    public override UInt32 BitCount => 16;
}
public sealed class CPHYPHCRCDecodePacket : CPHYDecodePacket
{
    public CPHYPHCRCDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Byte[] Data { get; init; } /*= Encoding.Default.GetBytes("PHCRC");*/
    public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
    public override Boolean IsInfoPacket => false;
    public override CPHYPacketType PacketType => CPHYPacketType.PHCRC;
    public override String Title => "PHCRC";
    public override UInt32 BitCount => 16;
}
public sealed class CPHYErrorDecodePacket : CPHYDecodePacket
{
    public CPHYErrorDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("ERROR");
    public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
    public override Boolean IsInfoPacket => true;
    public override CPHYPacketType PacketType => CPHYPacketType.ERROR;
    public override String Title => "ERROR";
    public override UInt32 BitCount => 0;
}
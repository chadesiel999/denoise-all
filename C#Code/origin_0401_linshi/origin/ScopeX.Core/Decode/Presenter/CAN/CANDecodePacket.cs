using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode;

public abstract class CANDecodePacket : BaseDecodePacket
{
    public CANDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }

    public abstract CANPacketType PacketType { get; }
    public override SerialProtocolType ProtocolType => SerialProtocolType.CAN;
}
public sealed class CANSOFDecodePacket : CANDecodePacket
{
    public CANSOFDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Boolean IsInfoPacket => true;
    public override CANPacketType PacketType => CANPacketType.SOF;
    public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("SOF");
    public override Color BorderColor => Color.Green;
}
public sealed class CANR0DecodePacket : CANDecodePacket
{
    public CANR0DecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("R0");
    public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
    public override Boolean IsInfoPacket => true;
    public override CANPacketType PacketType => CANPacketType.R0;
}
public sealed class CANR1DecodePacket : CANDecodePacket
{
    public CANR1DecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("R1");
    public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
    public override Boolean IsInfoPacket => true;
    public override CANPacketType PacketType => CANPacketType.R1;
}
public sealed class CANRTRDecodePacket : CANDecodePacket
{
    public CANRTRDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("RTR");
    public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
    public override Boolean IsInfoPacket => true;
    public override CANPacketType PacketType => CANPacketType.RTR;
}
public sealed class CANIDEDecodePacket : CANDecodePacket
{
    public CANIDEDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("IDE");
    public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
    public override Boolean IsInfoPacket => true;
    public override CANPacketType PacketType => CANPacketType.IDE;
}
public sealed class CANSRRDecodePacket : CANDecodePacket
{
    public CANSRRDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("R0");
    public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
    public override Boolean IsInfoPacket => true;
    public override CANPacketType PacketType => CANPacketType.SRR;
}
public sealed class CANStandardIDDecodePacket : CANDecodePacket
{
    public CANStandardIDDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }

    public override CANPacketType PacketType => CANPacketType.StandardID;

    public override Boolean IsInfoPacket => false;
    public override UInt32 BitCount => 11;
    public override String Title => "Standard ID";
    public override Color BorderColor => ColorTranslator.FromHtml("#0xFF808000");
}
public sealed class CANDLCDecodePacket : CANDecodePacket
{
    public CANDLCDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override Boolean IsInfoPacket => false;
    public override CANPacketType PacketType => CANPacketType.DLC;
    public override UInt32 BitCount => 4;
    public override String Title => "Data Length Code";
    public override Color BorderColor => ColorTranslator.FromHtml("#0xff400080");
}
public sealed class CANExtandIDDecodePacket : CANDecodePacket
{
    public CANExtandIDDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override CANPacketType PacketType => CANPacketType.ExtandID;
    public override Boolean IsInfoPacket => false;
    public override Color BorderColor => ColorTranslator.FromHtml("#0xFF808000");
    public override UInt32 BitCount => 29;
    public override String Title => "Extand ID";
}
public sealed class CANDataDecodePacket : CANDecodePacket
{
    public CANDataDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override CANPacketType PacketType => CANPacketType.Data;
    public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xff007f7f");
    public override UInt32 BitCount => 8;
    public override Boolean IsInfoPacket => false;
    public override String Title => "Data";
}
public sealed class CANCRCDecodePacket : CANDecodePacket
{
    public CANCRCDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public Byte[] SuccessCRC { get; init; } = new Byte[0];
    public Boolean Success
    {
        get
        {
            if (Data == null || SuccessCRC == null || Data.Length != SuccessCRC.Length || Data.Length != 2) return false;
            for (Int32 i = 0; i < Data.Length; i++)
            {
                if (Data[i] != SuccessCRC[i]) return false;
            }
            return true;
        }
    }
    public override UInt32 BitCount => 15;
    public override Boolean IsInfoPacket => false;
    public override String Title => "CRC";
    public override CANPacketType PacketType => CANPacketType.CRC;

    public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xff400080") : Color.Red;
    public override String ErrorInfo => Success ? "" : "CRC Calculated:" + BitConverter.ToString(SuccessCRC).Replace("-", "");
}
public sealed class CANErrorFrameDecodePacket : CANDecodePacket
{
    public CANErrorFrameDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }

    public override UInt32 BitCount => 8;
    public override Boolean IsInfoPacket => false;
    public override String Title => "Error";
    public override CANPacketType PacketType => CANPacketType.FrameError;

    public override Color BorderColor => Color.Red;
    public override String ErrorInfo => "FrameError";
}
public sealed class CANErrorPadBitDecodePacket : CANDecodePacket
{
    public CANErrorPadBitDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }

    public override UInt32 BitCount => 1;
    public override Boolean IsInfoPacket => false;
    public override String Title => "Error";
    public override CANPacketType PacketType => CANPacketType.PadBitError;

    public override Color BorderColor => Color.OrangeRed;
    public override String ErrorInfo => "PadBitError";
}
public sealed class CANEOFDecodePacket : CANDecodePacket
{
    public CANEOFDecodePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override UInt32 BitCount => 7;
    public override Boolean IsInfoPacket => false;
    public override String Title => "EOF";
    public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xff400080");
    public override CANPacketType PacketType => CANPacketType.EOF;
}
public sealed class CANACKDeocdePacket : CANDecodePacket
{
    public CANACKDeocdePacket(Single start, Single lenght) : base(start, lenght)
    {
    }
    public override UInt32 BitCount => 1;
    public Boolean Success
    {
        get
        {
            if (Data == null || Data.Length != 1) return false;
            return Encoding.Default.GetString(Data).Trim() == "1";
        }
    }
    public override CANPacketType PacketType => CANPacketType.ACK;
    public override Color BorderColor => Color.IndianRed;
    public override Boolean IsInfoPacket => true;

    public override String ErrorInfo => Success ? "" : "ACK Error";
}

public sealed class CANACKDeocdePacketCPP : CANDecodePacket
{
    public CANACKDeocdePacketCPP(Single start, Single lenght) : base(start, lenght)
    {
    }
    public Boolean Success { get; set; }

    public override UInt32 BitCount => 1;
    public override Color BorderColor => !Success ? ColorTranslator.FromHtml("#0xff400080") : Color.Red;
    public override CANPacketType PacketType => CANPacketType.ACK;

    public override Boolean IsInfoPacket => false;
    //public override Byte[] Data;// => Encoding.Default.GetBytes("ACK");
    public override String ErrorInfo => !Success ? "" : "ACK Error";
    public override String Title => "ACK";

}

public sealed class CANErrorFrameDeocdePacketCPP : CANDecodePacket
{
    public CANErrorFrameDeocdePacketCPP(Single start, Single lenght) : base(start, lenght)
    {
    }
    //public override UInt32 BitCount => 12;// (UInt32)Encoding.Default.GetBytes("Error Frame").Length;
    public override Color BorderColor =>  Color.Red;
    public override CANPacketType PacketType => CANPacketType.Data;

    public override Boolean IsInfoPacket => true;
   // public override String Title => "Error Frame";

    public override byte[] Data => Encoding.Default.GetBytes("Error Frame");
}

public sealed class CANOverLoadFrameDeocdePacketCPP : CANDecodePacket
{
    public CANOverLoadFrameDeocdePacketCPP(Single start, Single lenght) : base(start, lenght)
    {
    }
    //public override UInt32 BitCount => 14;// (UInt32)Encoding.Default.GetBytes("Over Load Frame").Length;
    public override Color BorderColor => Color.Red;
    public override CANPacketType PacketType => CANPacketType.Data;

    public override Boolean IsInfoPacket => true;
    //public override String Title => "Over Load Frame";
    public override byte[] Data => Encoding.Default.GetBytes("Over Load Frame");
}

public sealed class CANStandardIDDecodePacketCPP : CANDecodePacket
{
    public CANStandardIDDecodePacketCPP(float start, float lenght) : base(start, lenght)
    {
    }

    public Boolean Success { get; set; }
    public override CANPacketType PacketType => CANPacketType.StandardID;

    public override bool IsInfoPacket => false;
    public override uint BitCount => 11;
    public override string Title => "Standard ID";
    public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xFF808000") : Color.Red;
    public override String ErrorInfo => Success ? "" : "Stuf Error";
}


public sealed class CANExtandIDDecodePacketCPP : CANDecodePacket
{
    public CANExtandIDDecodePacketCPP(float start, float lenght) : base(start, lenght)
    {
    }
    public Boolean Success { get; set; }
    public override CANPacketType PacketType => CANPacketType.ExtandID;
    public override bool IsInfoPacket => false;
    public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xFF808000") : Color.Red;
    public override uint BitCount => 29;
    public override string Title => "Extand ID";

    public override String ErrorInfo => Success ? "" : "Stuf Error";
}
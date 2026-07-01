using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public abstract class CXPIDecodePacket : BaseDecodePacket
    {
        public CXPIDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public UInt32 _BitCount = 1;
        public override SerialProtocolType ProtocolType => SerialProtocolType.CXPI;

        public abstract CxpiPacketType PacketType { get; }
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

    public sealed class CXPIStartDecodePacket : CXPIDecodePacket
    {
        public CXPIStartDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override CxpiPacketType PacketType => CxpiPacketType.Start;
        public override Byte[] Data => Encoding.Default.GetBytes("Start");
        public override Color BorderColor => Color.Green;
        public override Boolean IsInfoPacket => true;
    }

    public class CXPIPtypeDecodePacket : CXPIDecodePacket//PTYPE字段
    {
        public CXPIPtypeDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "Ptype";
        public override CxpiPacketType PacketType => CxpiPacketType.Ptype;
        public override Boolean IsInfoPacket => false;

        public Boolean PtypeError;
        public override String ErrorInfo => PtypeError ? "PTYPE Error" : String.Empty;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override Color BorderColor => PtypeError ? Color.Red : ColorTranslator.FromHtml("#0xFF008000");
    }

    public class CXPIPidDecodePacket : CXPIDecodePacket//PID字段
    {
        public CXPIPidDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "PID";
        public override CxpiPacketType PacketType => CxpiPacketType.Pid;
        public override Boolean IsInfoPacket => false;

        public Boolean PidError;
        public override String ErrorInfo => PidError ? "PID Error" : String.Empty;
        public override Byte[] Data { get; init; } = new byte[0];
        public override Color BorderColor => PidError ? Color.Red : ColorTranslator.FromHtml("#FFCC9900");
    }

    public class CXPIFICounterDecodePacket : CXPIDecodePacket//FI_Counter字段
    {
        public CXPIFICounterDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "Counter";
        public override CxpiPacketType PacketType => CxpiPacketType.Counter;
        public override Boolean IsInfoPacket => false;

        public Boolean CounterError;
        public override String ErrorInfo => CounterError ? "Counter Error" : String.Empty;
        public override Byte[] Data { get; init; } = new byte[0];
        public override Color BorderColor => CounterError ? Color.Red : Color.Green;
    }

    public class CXPIFIWakeupDecodePacket : CXPIDecodePacket//FI_wakeup字段
    {
        public CXPIFIWakeupDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "Wakeup";
        public override CxpiPacketType PacketType => CxpiPacketType.Wakeup;
        public override Boolean IsInfoPacket => false;

        public Boolean WakeupError;
        public override String ErrorInfo => WakeupError ? "Wakeup Error" : String.Empty;
        public override Byte[] Data { get; init; } = new byte[0];
        public override Color BorderColor => WakeupError ? Color.Red : Color.Green;
    }
    public class CXPIFISleepDecodePacket : CXPIDecodePacket//FI_Sleep字段
    {
        public CXPIFISleepDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "Sleep";
        public override CxpiPacketType PacketType => CxpiPacketType.Sleep;
        public override Boolean IsInfoPacket => false;

        public Boolean SleepError;
        public override String ErrorInfo => SleepError ? "Error:Parity" : String.Empty;
        public override Byte[] Data { get; init; } = new byte[0];
        public override Color BorderColor => SleepError ? Color.Red : Color.Green;
    }
    public class CXPIDataDecodePacket : CXPIDecodePacket//DATA字段
    {
        public CXPIDataDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "Data";
        public override CxpiPacketType PacketType => CxpiPacketType.Data;
        public override Byte[] Data { get; init; } = new byte[0];

        public override Boolean IsInfoPacket => false;

        public Boolean DataError;
        public override String ErrorInfo => DataError ? "Data Error" : String.Empty;
        public override Color BorderColor => DataError ? Color.Red : Color.Yellow;
    }
    public class CXPICrcDecodePacket : CXPIDecodePacket//CRC字段
    {
        public CXPICrcDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "CRC_LSB_MSB";
        public override CxpiPacketType PacketType => CxpiPacketType.Crc;
        public override Boolean IsInfoPacket => false;

        public Boolean CrcError;
        public override String ErrorInfo => CrcError ? "Error:CRC" : String.Empty;
        public override Byte[] Data { get; init; } = new byte[0];
        public override Color BorderColor => CrcError ? Color.Red : Color.Purple;//紫色
    }
    public class CXPIStartBitDecodePacket : CXPIDecodePacket//StartBit
    {
        public CXPIStartBitDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "Start Bit";
        public override CxpiPacketType PacketType => CxpiPacketType.StartBit;
        public override Boolean IsInfoPacket => false;

        public Boolean StartBitError;
        public override String ErrorInfo => StartBitError ? "StartBit Error" : String.Empty;
        public override Byte[] Data { get; init; } = new byte[0];
        public override Color BorderColor => StartBitError ? Color.Red : ColorTranslator.FromHtml("#FFCC99");//淡绿色
    }

    public class CXPIStopBitDecodePacket : CXPIDecodePacket//StopBit
    {
        public CXPIStopBitDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "Stop Bit";
        public override CxpiPacketType PacketType => CxpiPacketType.StopBit;
        public override Boolean IsInfoPacket => false;

        public Boolean StopBitError;
        public override String ErrorInfo => StopBitError ? "StopBit Error" : String.Empty;
        public override Byte[] Data { get; init; } = new byte[0];
        public override Color BorderColor => StopBitError ? Color.Red : ColorTranslator.FromHtml("#FFCC99");//淡绿色
    }

    public class CXPIParityBitDecodePacket : CXPIDecodePacket//ParityBit
    {
        public CXPIParityBitDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "Parity Bit";
        public override CxpiPacketType PacketType => CxpiPacketType.ParityBit;
        public override Boolean IsInfoPacket => false;

        public Boolean ParityBitError;
        public override String ErrorInfo => ParityBitError ? "Error:Parity" : String.Empty;
        public override Byte[] Data { get; init; } = new byte[0];
        public override Color BorderColor => ParityBitError ? Color.Red : ColorTranslator.FromHtml("#800080");//紫色
    }

    public class CXPIDlcDecodePacket : CXPIDecodePacket//Dlcext
    {
        public CXPIDlcDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "DLCExt";
        public override CxpiPacketType PacketType => CxpiPacketType.Dlc;
        public override Boolean IsInfoPacket => false;

        public Boolean DlcError;
        public override String ErrorInfo => DlcError ? "Error:Parity" : String.Empty;
        public override Byte[] Data { get; init; } = new byte[0];
        public override Color BorderColor => DlcError ? Color.Red : ColorTranslator.FromHtml("#FFCC9900");
    }

    public class CXPIIbsDecodePacket : CXPIDecodePacket//ParityBit
    {
        public CXPIIbsDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override String Title => "IBS";
        public override CxpiPacketType PacketType => CxpiPacketType.Ibs;
        public override Boolean IsInfoPacket => false;

        public override Byte[] Data { get; init; } = new byte[0];
        public override Color BorderColor => Color.Blue;
    }

    public enum CxpiPacketType
    {
        Start,
        Ptype,
        Pid,
        Counter,
        Wakeup,
        Sleep,
        Dlc,
        Data,
        Crc,
        StartBit,
        StopBit,
        ParityBit,
        Ibs,
    }
}

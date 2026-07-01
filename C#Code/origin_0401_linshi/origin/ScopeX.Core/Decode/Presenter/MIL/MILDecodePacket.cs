using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public abstract class MILDecodePacket : BaseDecodePacket
    {
        protected MILDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SerialProtocolType ProtocolType => SerialProtocolType.MIL;
        public abstract MILDecodePacketType DecodePacketType { get; }
    }
    public sealed class MILSOFDecodePacket : MILDecodePacket
    {
        public MILSOFDecodePacket(Single start) : base(start, 1)
        {
        }
        public override MILDecodePacketType DecodePacketType => MILDecodePacketType.SOF;
        public override Boolean IsInfoPacket => true;
        public override Color BorderColor { get; init; } = Color.Green;
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("SOF");
    }
    public sealed class MILEOFDecodePacket : MILDecodePacket
    {
        public MILEOFDecodePacket(Single start) : base(start, 1)
        {
        }
        public override MILDecodePacketType DecodePacketType => MILDecodePacketType.EOF;
        public override Boolean IsInfoPacket => true;
        public override Color BorderColor { get; init; } = Color.Red;
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("EOF");
    }
    public sealed class MILSyncDecodePacket : MILDecodePacket
    {
        public MILSyncDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Byte[] Data => Encoding.Default.GetBytes(PacketType.ToString());
        public override MILDecodePacketType DecodePacketType => MILDecodePacketType.Sync;
        public override Boolean IsInfoPacket => true;
        public Boolean Success = true;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xFF400080") : Color.Red;
        //public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF400080");
        public override String ErrorInfo => Success ? String.Empty : "Sync Error";
        public MILPacketType PacketType { get; init; }
    }
    public sealed class MILRTADecodePacket : MILDecodePacket
    {
        public MILRTADecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override MILDecodePacketType DecodePacketType => MILDecodePacketType.RTA;
        public override UInt32 BitCount => MILDecodeModelCPP.RTADDRESSSBITCOUNT;
        public override Boolean IsInfoPacket => false;
        public override String Title => "Remote Terminal Address";
        public Boolean Success = true;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xFF808000") : Color.Red;
        //public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF808000");
        public override String ErrorInfo => Success ? String.Empty :  "RTA Error";
    }
    public sealed class MILTRDecodePacket : MILDecodePacket
    {
        public MILTRDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public override MILDecodePacketType DecodePacketType => MILDecodePacketType.TR;

        public override Boolean IsInfoPacket => false;
        public override String Title { get; init; } = "Transmit/Receive";
        public Boolean TR { get; init; }
        public override UInt32 BitCount { get; init; } = 1;
        public override Byte[] Data => new Byte[] { Convert.ToByte(TR) };
        public Boolean Success = true;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xFF808000") : Color.Red;
        public override String ErrorInfo => Success ? String.Empty : "TR Error";
        // public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF808000");
    }
    public sealed class MILSADecodePacket : MILDecodePacket
    {
        public MILSADecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override UInt32 BitCount { get; init; } = MILDecodeModelCPP.SUBADDRESSBITCOUNT;
        public override Boolean IsInfoPacket => false;
        public Boolean Success = true;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xFF808000") : Color.Red;
        // public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF808000");
        public override String ErrorInfo => Success ? String.Empty : "Sub Address Error";
        public override MILDecodePacketType DecodePacketType => MILDecodePacketType.SubAddress;
        public override String Title { get; init; } = "Sub Address";

    }
    public sealed class MILModeCodeDecodePacket : MILDecodePacket
    {
        public MILModeCodeDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Boolean IsInfoPacket => false;
        public Boolean Success = true;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xFF808000") : Color.Red;
        // public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF808000");
        public override String ErrorInfo => Success ? String.Empty : "Mode Code Error";
        public override UInt32 BitCount { get; init; } = MILDecodeModelCPP.MODELCODEBITCOUNT;
        public override String Title { get; init; } = "Mode Code";
        public override MILDecodePacketType DecodePacketType => MILDecodePacketType.ModeCode;
    }
    public sealed class MILParityDecodePacket : MILDecodePacket
    {
        public MILParityDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Byte[] Data => new Byte[] { Convert.ToByte(Parity) };
        public override UInt32 BitCount => 1;
        public override String Title { get; init; } = "Parity";
        public Boolean Parity { get; init; }
        public Boolean SuccessParity { get; init; }
        public Boolean Success => Parity == SuccessParity;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xFF400080") : Color.Red;
        public override MILDecodePacketType DecodePacketType => MILDecodePacketType.Parity;
        public override Boolean IsInfoPacket => false;
        public override String ErrorInfo => Success ? String.Empty : "Parity Error";
    }
    public sealed class MILParityDecodePacketCPP : MILDecodePacket
    {
        public MILParityDecodePacketCPP(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Byte[] Data => new Byte[] { Convert.ToByte(Parity) };
        public override UInt32 BitCount => 1;
        public override String Title { get; init; } = "Parity";
        public Boolean Parity { get; init; }
        public Boolean Success = true;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xFF400080") : Color.Red;
        public override MILDecodePacketType DecodePacketType => MILDecodePacketType.Parity;
        public override Boolean IsInfoPacket => false;
        public override String ErrorInfo => Success ? String.Empty : "Parity Error";
    }
    public sealed class MILDataDecodePacket : MILDecodePacket
    {
        public MILDataDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override UInt32 BitCount { get; init; } = MILDecodeModelCPP.DATABITCOUNT;
        public override String Title { get; init; } = "Data";
        public Boolean Success = true;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xFF008080") : Color.Red;
        //public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF008080");
        public override String ErrorInfo => Success ? String.Empty : "Data Error";
        public override MILDecodePacketType DecodePacketType => MILDecodePacketType.Data;
        public override Boolean IsInfoPacket => false;
    }
    public sealed class MILStatusDecodePacket : MILDecodePacket
    {
        public MILStatusDecodePacket(Single start, Single lenght, MILDecodePacketType decodePacketType) : base(start, lenght)
        {
            DecodePacketType = decodePacketType;
            switch (decodePacketType)
            {
                case MILDecodePacketType.BroadcastCommandReceived:
                    Title = "Broadcast Command Received";
                    break;
                case MILDecodePacketType.Busy:
                    Title = "Busy";
                    break;
                case MILDecodePacketType.SubsystemFlag:
                    Title = "Subsystem Flag";
                    break;
                case MILDecodePacketType.ServiceResquest:
                    Title = "Service Resquest";
                    break;
                case MILDecodePacketType.TerminalFlag:
                    Title = "Terminal Flag";
                    break;
                case MILDecodePacketType.MessageError:
                    Title = "Message Error";
                    break;
                case MILDecodePacketType.Instrumentation:
                    Title = "Instrumentation";
                    break;
                case MILDecodePacketType.DynamicBusControlAcceptance:
                    Title = "Dynamic Bus Control Acceptance";
                    break;
            }
        }
        public override Boolean IsInfoPacket => false;
        public Boolean Success = true;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xFF400080") : Color.Red;
        //public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF400080");
        public override String ErrorInfo => Success ? String.Empty : Title + "Error";
        public override MILDecodePacketType DecodePacketType { get; }
        public Boolean Status { get; init; }
        public override Byte[] Data => new Byte[] { Convert.ToByte(Status) };
        public override UInt32 BitCount { get; init; } = 1;
    }
    public sealed class MILReservedDecodePacket : MILDecodePacket
    {
        public MILReservedDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override Boolean IsInfoPacket => false;
        public override MILDecodePacketType DecodePacketType => MILDecodePacketType.Reserved;
        public override UInt32 BitCount { get; init; } = MILDecodeModelCPP.RESERVEBITCOUNT;
        public Boolean Success = true;
        public override Color BorderColor => Success ? ColorTranslator.FromHtml("#0xFF400080") : Color.Red;
        // public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF400080");
        public override String ErrorInfo => Success ? String.Empty : "Reserved Error";
        public override String Title { get; init; } = "Reserved";
    }
    public enum MILDecodePacketType
    {
        SOF,
        Sync,
        RTA,
        TR,
        SubAddress,
        ModeCode,
        Data,
        MessageError,
        Instrumentation,
        ServiceResquest,
        Reserved,
        BroadcastCommandReceived,
        Busy,
        SubsystemFlag,
        DynamicBusControlAcceptance,
        TerminalFlag,
        Parity,
        EOF,
    }
}

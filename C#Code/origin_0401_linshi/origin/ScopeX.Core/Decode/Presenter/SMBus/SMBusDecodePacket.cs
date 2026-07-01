using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    public abstract class SMBusDecodePacket : BaseDecodePacket
    {
        public SMBusDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public UInt32 _BitCount = 1;
        public override SerialProtocolType ProtocolType => SerialProtocolType.SMBus;

        public abstract SMBusFieldType PacketType { get; }
        public override uint BitCount => _BitCount;

    }

    public class SMBusStartDecodePacket : SMBusDecodePacket//Start字段
    {
        public SMBusStartDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override SMBusFieldType PacketType => SMBusFieldType.FIELD_START;
        public override byte[] Data { get; init; } = Encoding.Default.GetBytes("Start");

        public override Boolean IsInfoPacket => true;

        public override Color BorderColor => Color.Green;//绿色
    }

    public class SMBusRestartDecodePacket : SMBusDecodePacket//Restart字段
    {
        public SMBusRestartDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override SMBusFieldType PacketType => SMBusFieldType.FIELD_RESTART;
        public override byte[] Data { get; init; } = Encoding.Default.GetBytes("Restart");
        public override Boolean IsInfoPacket => true;//是否显示数值 true：不显示
        public override Color BorderColor => Color.Green;//绿色
    }

    public class SMBusStopDecodePacket : SMBusDecodePacket//Stop字段
    {
        public SMBusStopDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override SMBusFieldType PacketType => SMBusFieldType.FIELD_STOP;
        //public override Byte[] Data { get; init; } = new Byte[0];
        public override byte[] Data { get; init; } = Encoding.Default.GetBytes("Stop");

        public override Boolean IsInfoPacket => true;
        public override Color BorderColor => Color.Red;//红色
    }

    public class SMBusAddressDecodePacket : SMBusDecodePacket//Address字段
    {
        public SMBusAddressDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override SMBusFieldType PacketType => SMBusFieldType.FIELD_ADDRESS;
        public override Boolean IsInfoPacket => false;

        public override Byte[] Data { get; init; } = new Byte[0];
        public override Color BorderColor => Color.Yellow;//黄色
    }

    public class SMBusWrDecodePacket : SMBusDecodePacket//Rw字段
    {
        public SMBusWrDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override SMBusFieldType PacketType => SMBusFieldType.FIELD_ADDRESS_WR;
        public override Boolean IsInfoPacket => false;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override Color BorderColor => Color.Yellow;//黄色
    }

    public class SMBusCommandDecodePacket : SMBusDecodePacket//Command字段
    {
        public SMBusCommandDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override SMBusFieldType PacketType => SMBusFieldType.FIELD_COMMAND_CODE;
        public override Boolean IsInfoPacket => false;

        public override Byte[] Data { get; init; } = new Byte[0];
        public override Color BorderColor => Color.Yellow;//黄色
    }

    public class SMBusByteCountDecodePacket : SMBusDecodePacket //ByteCount字段
    {
        public SMBusByteCountDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override SMBusFieldType PacketType => SMBusFieldType.FIELD_BYTE_COUNT;
        public override Boolean IsInfoPacket => false;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override Color BorderColor => Color.Yellow;//黄色
    }

    public class SMBusDataDecodePacket : SMBusDecodePacket //Data字段
    {
        public SMBusDataDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override SMBusFieldType PacketType => SMBusFieldType.FIELD_DATA;

        public Boolean _InfoPacketValue = false;
        public override Boolean IsInfoPacket => _InfoPacketValue;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override Color BorderColor => Color.Cyan;//青色
    }

    public class SMBusPecDecodePacket : SMBusDecodePacket//PEC字段
    {
        public SMBusPecDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override SMBusFieldType PacketType => SMBusFieldType.FIELD_PEC;
        public override Boolean IsInfoPacket => false;

        public String _PecNote = String.Empty;
        public Boolean PecError;
        //public override String ErrorInfo => _PecNote ;
        public override String ErrorInfo => PecError ? _PecNote : String.Empty;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override Color BorderColor => PecError ? Color.Red : Color.Purple;//＃FF0000红色 紫色 ＃9400D3
    }


    public class SMBusAckDecodePacket : SMBusDecodePacket//ACK字段
    {
        public SMBusAckDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override SMBusFieldType PacketType => SMBusFieldType.FIELD_ACKNACK;
        public override Boolean IsInfoPacket => false;

        public String _AckNote = String.Empty;

        public Boolean AckError;
        public override String ErrorInfo => AckError ? _AckNote : String.Empty;

        //public override String ErrorInfo => AckError ? "Error: Unexpected NACK" : String.Empty;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override Color BorderColor => AckError ? Color.Red : Color.Purple;//紫色
    }





}







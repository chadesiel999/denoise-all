using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public abstract class I2CDecodePacket : BaseDecodePacket
    {
        public I2CDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public UInt32 _BitCount = 1;
        public override SerialProtocolType ProtocolType => SerialProtocolType.I2C;

        public abstract I2CPacketType PacketType { get; }
        public override uint BitCount => _BitCount;

    }

    public class I2CStartDecodePacket : I2CDecodePacket//Start字段
    {
        public I2CStartDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override I2CPacketType PacketType => I2CPacketType.Start;
        public override byte[] Data { get; init; } = Encoding.Default.GetBytes("Start");

        public override Boolean IsInfoPacket => true;

        public override Color BorderColor => Color.Green;//绿色
    }

    public class I2CReStartDecodePacket : I2CDecodePacket
    {
        public I2CReStartDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override I2CPacketType PacketType => I2CPacketType.ReStart;
        public override Boolean IsInfoPacket => true;
        public override Byte[] Data { get; init; } = Encoding.Default.GetBytes("ReStart");
        public override Color BorderColor { get; init; } = ColorTranslator.FromHtml("#0xFF00FF00");
    }
    public class I2CAddrDecodePacket : I2CDecodePacket//Address字段
    {
        public I2CAddrDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override I2CPacketType PacketType => I2CPacketType.Addr;
        public override Boolean IsInfoPacket => false;

        public override Byte[] Data { get; init; } = new Byte[0];
        public override Color BorderColor => Color.Yellow ;//黄色
    }
    public class I2CACKDecodePacket : I2CDecodePacket
    {
        public I2CACKDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override I2CPacketType PacketType => I2CPacketType.AddrACK;
        public override Boolean IsInfoPacket => false;

        public String _AckNote = String.Empty;

        public Boolean AckError;
        public override String ErrorInfo => AckError ? _AckNote : String.Empty;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override Color BorderColor => AckError ? Color.Red : Color.Purple;//紫色
    }
    public class I2CRWDecodePacket : I2CDecodePacket
    {
        public I2CRWDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        //public String _Title = "";
        //public override String Title => _Title;
        public override I2CPacketType PacketType => I2CPacketType.RW;
        public override Boolean IsInfoPacket => true;
        public Byte RW { get; init; }
        public override Byte[] Data => Encoding.Default.GetBytes( (RW==1) ? "Read" : "Write");

        //public override Byte[] Data { get; init; } = new Byte[0];
        public override Color BorderColor =>  Color.Yellow;//黄色
    }
    public class I2CDataDecodePacket : I2CDecodePacket
    {
        public I2CDataDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override I2CPacketType PacketType => I2CPacketType.Data;

        public Boolean _InfoPacketValue = false;
        public override Boolean IsInfoPacket => _InfoPacketValue;
        public override Byte[] Data { get; init; } = new Byte[0];
        public override Color BorderColor =>  Color.Cyan;//青色
    }
    public class I2CStopDecodePacket : I2CDecodePacket
    {
        public I2CStopDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public String _Title = "";
        public override String Title => _Title;
        public override I2CPacketType PacketType => I2CPacketType.Stop;
        public override byte[] Data { get; init; } = Encoding.Default.GetBytes("Stop");
        public override Boolean IsInfoPacket => true;
        public override Color BorderColor => Color.Red;//红色
    }
    public enum I2CPacketType
    {
        Start,
        ReStart,
        Addr,
        RW,
        AddrACK,
        Data,
        DataACK,
        Stop,
    }
}

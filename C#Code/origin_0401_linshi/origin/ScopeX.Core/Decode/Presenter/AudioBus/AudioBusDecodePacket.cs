using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode
{
    public abstract class AudioBusDecodePacket : BaseDecodePacket
    {
        protected AudioBusDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }
        public override SerialProtocolType ProtocolType => SerialProtocolType.AudioBus;
        public abstract AudioChannelType ChannelType { get; }
    }
    public sealed class AudioBusLeftDecodePacket : AudioBusDecodePacket
    {
        public AudioBusLeftDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public override AudioChannelType ChannelType => AudioChannelType.Left;

        public override Boolean IsInfoPacket => false;
        public override String Title => "Left Channel";
        public override String ErrorInfo => BitCount > SuccessBitCount ? "Bit Lost" : "";
        public override Color BorderColor => BitCount > SuccessBitCount ? Color.Red : ColorTranslator.FromHtml("#0xFFC0C000");
        public UInt32 SuccessBitCount { get; init; }

    }
    public sealed class AudioBusRightDecodePacket : AudioBusDecodePacket
    {
        public AudioBusRightDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public override AudioChannelType ChannelType => AudioChannelType.Right;

        public override Boolean IsInfoPacket => false;
        public override String Title => "Right Channel";
        public override String ErrorInfo => BitCount > SuccessBitCount ? "Bit Lost" : "";
        public override Color BorderColor => BitCount > SuccessBitCount ? Color.Red : ColorTranslator.FromHtml("#0xFF008080");
        public UInt32 SuccessBitCount { get; init; }
    }
    public sealed class AudioBusExtDecodePacket : AudioBusDecodePacket
    {
        public AudioBusExtDecodePacket(Single start, Single lenght) : base(start, lenght)
        {
        }

        public override AudioChannelType ChannelType => AudioChannelType.Ext;
        public UInt32 ChannelIndex { get; init; }
        public override String Title => "Channel";
        public override String ErrorInfo => Success ? "" : (DataBitSuccess ? "" : "Data Bit Lost;" + "\r\n" + (SyncBitSuccess ? "" : "Sync Bit Lost;")).Trim();
        public override Color BorderColor => Success ? (ChannelIndex % 2 == 0 ? ColorTranslator.FromHtml("#0xFFC0C000") : ColorTranslator.FromHtml("#0xFF008080")) : Color.Red;
        public override Boolean IsInfoPacket => false;
        public UInt32 SyncBitCount { get; init; }
        public UInt32 SuccessSyncBitCount { get; init; }

        public UInt32 SuccessBitCount { get; init; }
        public Boolean DataBitSuccess => BitCount == SuccessBitCount;
        public Boolean SyncBitSuccess => SuccessSyncBitCount == SyncBitCount;
        public Boolean Success => DataBitSuccess || SyncBitSuccess;
    }
    public enum AudioChannelType
    {
        Left,
        Right,
        Ext,
    }
}

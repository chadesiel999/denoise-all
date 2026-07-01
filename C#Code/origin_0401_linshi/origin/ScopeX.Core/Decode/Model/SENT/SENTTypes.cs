using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode.Model.SENT
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SentEventField
    {
        public Byte Type;
        public Int64 StartIndex;
        public Int64 Length;
        public Int32 DataIndex;
        public Int32 DataLength;
        public Byte HasError;
        public Int32 ErrorDataIndex;
        public Int32 ErrorDataLength;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SentEvent
    {
        public Int64 StartIndex;
        public IntPtr Fields;
        public Int32 FieldCount;
        public IntPtr Data;
        public Int32 DataCount;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SentResult
    {
        public IntPtr EventInfosPtr;
        public UInt32 EventCount;
        public SerialProtocolType ProtocolType; //协议类型
    }

    #region SENT
    public enum SentNibbleCount
    {
        ONENIBBLE = 1,
        TWONIBBLE,
        THREENIBBLE,
        FOURNIBBLE,
        FIVENIBBLE,
        SIXNIBBLE,
    }

    public enum SentCrcStandard
    {
        SENT_CRC_STANDARD_V2008,
        SENT_CRC_STANDARD_V2010,
    }

    public enum PauseBit
    {
        NO,
        YES,
    }

    public enum Polarity :Int32
    {
        POSITIVE,
        NEGTIVE
    }

    public enum SentDecodeEventType
    {
        NONE,
        SYNC,
        STATUS,
        CRC,
        DATA,
        PAUSE,
        SLOWID,
        SLOWCRC,
        SLOWDATA
    };

    public enum SentDecodeEventErrorType
    {
        NONE,
        SYNCERROR,
        STATUSERROR,
        CRCERROR,
        PAUSEERROR,
        DATAERROR,
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SentOptions
    {
        public IntPtr cancel_flag;                            //取消解码标志

        public PauseBit pause_bit;                            //是否有暂停位

        public Double clock_cyle;                             //信号时钟周期

        public Double tolerance;                              //时钟容差

        public SentNibbleCount signal_nibble_count;           //包半字节数

        public Polarity signal_polarity;                      //信号极性

        public ProtocolSENT.ChannelMode channel_mode;        //快速通道还是慢速通道
    };

    #endregion
}

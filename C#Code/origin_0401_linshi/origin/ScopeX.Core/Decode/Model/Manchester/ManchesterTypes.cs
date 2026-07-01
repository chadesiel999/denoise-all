using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ScopeX.ComModel.ProtocolManchester;
namespace ScopeX.Core.Decode
{
    internal partial class ManchesterDecodeModel
    {
        public enum ManchesterByteStatusType
        {
            ByteNoError,
            ByteError,
        };
        [StructLayout(LayoutKind.Sequential)]
        public struct ManchesterOptions
        {
            public IntPtr CancelFlag;//取消解码标志

            public Double BaudRate;//波特率
            public Byte HeaderSize;//头
            public Byte TrailerSize;//尾
            public Double Tolerance;//容限
            public Byte SyncSize;//同步位
            public Byte StartEdgeSize;//起始边沿
            public Byte WordSize;//字节
            public Byte WordDataNum;//字数
            public Double IdleBitsSize;//空闲位

            public ProtocolManchester.DataView DataViewFlag;//数据包视图
            public ProtocolManchester.MSB_LSB ByteOrder;//位顺序
            public ProtocolManchester.Polarity EdgePolarity;//上升沿为1 or 下降沿为1
            public ProtocolManchester.OddEvenCheck OddEvenCheck;//奇偶性
           
        };
        
        public unsafe struct ManchesterEvent
        {
            public IntPtr ByteField;//值指针
            public UInt32 ByteFieldNum;//数据长度

            public ManchesterStatusType ManchesterStatusType;//错误状态

            public ParityCheckStatusTypes ParityCheckStatusTypes;//奇偶校验状态

            public ManchesterBitField ParityBit;

            public UInt64 DataNum;//非数据包视图下计算的data数据包个数

            
            public UInt64 StartIndex;
            public UInt64 EndIndex;

            public ManchesterEvent()
            {
                ByteField = IntPtr.Zero;
                ByteFieldNum = 0;

                ManchesterStatusType = ManchesterStatusType.ManchesterNoError;

                ParityCheckStatusTypes = ParityCheckStatusTypes.ParityCheckNone;

                ParityBit =  new ManchesterBitField();

                DataNum = 0;

                StartIndex = 0;
                EndIndex = 0;
            }
        };

        public struct ManchesterByteField
        {
            public UInt64 StartIndex;
            public UInt64 EndIndex;

            public IntPtr Value;
            public Byte ValueSize;

            public ManchesterByteStatusType ByteStaus;

            public ManchesterByteFieldType ByteFieldType;

            public ManchesterByteField()
            {
                StartIndex = 0;
                EndIndex = 0;

                Value = IntPtr.Zero;
                ValueSize = 0;
             

                ByteStaus = ManchesterByteStatusType.ByteNoError;
                ByteFieldType = ManchesterByteFieldType.ByteFieldNone;
            }
        };

        public struct ManchesterBitField
        {
            public UInt64 StartIndex;
            public UInt64 EndIndex;
            public Byte Value;

            public ManchesterBitField()
            {
                StartIndex = 0;
                EndIndex = 0;
                Value = 0;
            }
        }

        public enum ManchesterStatusType
        { 
           ManchesterNoError = 0,
           ManchesterBitError,
           ManchesterUnframedError,
        };

        public enum ParityCheckStatusTypes
        {
            ParityCheckNone = 0,
            ParityCheckError,
            ParityCheckTrue,
        };

        
        public enum ManchesterByteFieldType
        {
            ByteFieldNone = 0,
            ByteFieldSync,
            ByteFieldHeader,
            ByteFieldData,
            ByteFieldTrailer,
        };

        public struct ManchesterResult
        {
            public IntPtr ManchesterEvent;
            public UInt32 EventCount;
            public SerialProtocolType ProtocolType; // 协议类型
        };

    }
}

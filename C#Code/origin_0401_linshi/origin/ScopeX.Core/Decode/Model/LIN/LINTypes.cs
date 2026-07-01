using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ScopeX.ComModel.ProtocolAudioBus;

namespace ScopeX.Core.Decode
{
    // lin信号版本
    public enum  LinSignalVersion
    {
        LIN_VERSION_NONE = -1,  // 版本none
        LIN_VERSION1,   // 1.0
        LIN_VERSION2,   // 2.0
    };

    public enum ByteFieldType
    {
        BYTEFIELD_NONE = 0,
        BYTEFIELD_SYNC, // 同步段类型
        BYTEFIELD_PID,  // PID类型
        BYTEFIELD_DATA, // 数据类型 
        BYTEFIELD_CHECKSUM, // 校验和类型
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct LinOption
    {
        public ProtocolLIN.Standard LinSignalVersion;  // lin协议类型
        public ProtocolCommon.Polarity LinSyncPolarity;           // lin同步位极性 正向反向    }
        public Int32 SignalRate ;          // lin信号速率
        public Double SamplePoint ;            // 默认采样点数为 50% ，后期可提供设置
    }

    public struct ByteField
    {
        public ByteFieldType ByteFieldType; // 字节域类型
        public Byte FieldCheckResult; // 字节域校验结果
        public Byte Value;  // 字节值
        public UInt64 FieldStartIndex ; // 字节域开始
        public UInt64 FieldEndIndex;   // 字节域结束
    }

    public struct LinEvent
    {
        public UInt64 EventStartIndex; // 字节域开始
        public UInt64 EventEndIndex;   // 字节域结束

        public IntPtr ByteField; // 字节域指针
        public Int32 ByteFieldCount; // 字节域个数

        public Int32 CheckSum;
    }

    public struct LinResult
    {
        public IntPtr LinEvent;
        public Int32  EventCount;
        public Int32  protocol_type; // 协议类型
    };
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ScopeX.ComModel.ProtocolCommon;

namespace ScopeX.Core.Decode.Model.LIN
{
    internal class LinDecodeStruct
    {
        #region C++解码入参结构体和枚举
        public enum Polarity :Int32
        {
            POSITIVE,
            NEGTIVE
        }


        #region LIN
        public enum LinSignalBitCount :Byte
        {
            ONEBIT = 1,
            TWOBIT,
            THREEBIT,
            FOURBIT,
            FIVEBIT,
            SIXBIT,
            SEVENBIT,
            EIGHTBIT
        }

        public enum LinDecodeEventType :Byte
        {
            NONE,
            SYNC,
            PID,
            DATA,
            CHECKSUM
        }

        public enum LinDecodeEventErrorType :Byte
        {
            NONE,
            SYNCERROR,
            PIDERROR,
            DATAERROR,
            CHECKSUMERROR
        }

        public enum LinSignalStandard :Byte
        {
            LIN_SGNAL_STANDARD_V1 = 0, // 2003
            LIN_SGNAL_STANDARD_V2 = 1  // 2010
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ProtocolOptionsLIN
        {
            public LinSignalStandard signal_standard;    // 信号标准版本
            public UInt32 baud_rate;                       // 信号波特率
            public LinSignalBitCount signal_bitcount;    // 包字节数
            public Polarity signal_polarity;             // 信号极性
            [MarshalAs(UnmanagedType.I1)]
            public Boolean checkpidcrc_flag;                // 检查PID校验
            public IntPtr cancel_flag;                   // 取消解码标志
        }
        #endregion
        #endregion


        #region C++解码结果映射结构体
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DecodeEvent
        {
            public Int64 event_index;            // 事件索引
            public Int64 start_index;            // 事件起始索引
            public Int64 length;                 // 事件长度
            public IntPtr data;                  // 事件数据数组索引
            public Int64 data_count;             // 事件数据数组长度
            public Byte event_type;             // 当前解码类型下的事件种类
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DecodeEventError
        {
            public Int64 event_error_index;      // 事件错误信息索引
            public Int64 event_index;            // 对应的事件索引
            public IntPtr data;                  // 事件错误对应的正确数据数组指针
            public Int64 data_count;             // 事件错误对应的正确数据数组长度
            public Byte event_error_type;       // 当前解码类型下的事件错误种类
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DecodeEventPacks
        {
            public Int64 start_index;                      // 帧起始位置索引
            public IntPtr decode_events_ptr;               // 解码结果事件数组指针
            public IntPtr decode_errors_ptr;               // 解码结果事件数组指针
            public UInt64 decode_event_count;              // 解码结果事件数组长度
            public UInt64 decode_error_count;              // 解码结果事件数组长度
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DecodeResultPacks
        {
            public IntPtr decode_result;                   // 解码帧事件数
            public UInt64 decode_result_count;             // 解码帧数量
        }
        #endregion




        internal static Byte[] ConvertIntPtrToData(IntPtr ptr, Int64 length)
        {
            // 验证长度是否有效
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length cannot be negative.");
            }

            // 创建一个字节数组来接收数据
            Byte[] data = new Byte[length];

            // 确保指针不为空
            if (ptr == IntPtr.Zero)
            {
                throw new ArgumentNullException(nameof(ptr), "Pointer cannot be null.");
            }

            // 使用 Marshal.Copy 从非托管内存复制到托管内存
            Marshal.Copy(ptr, data, 0, (Int32)length);

            return data;
        }
    }
}

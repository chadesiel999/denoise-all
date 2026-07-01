using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace ScopeX.Core.Decode
{
    internal partial class Mlt3DecodeModel
    {
        // MLT3错误
        public enum MLT3Error
        {
            MLT3_ERROR_NONE, // 默认值
            MLT3_ERROR_JUMP_ILLEGAL, // 跳变错误 0电平前后电平一致
            MLT3_ERROR_JUMP_FAST,    // 跳变过快 电平从 -1 到 1
            MLT3_ERROR_ZERO_TOOMUCH, // 0电平过多
        };

        public struct Mlt3Event//事件包
        {
            public byte value = 0;
            public MLT3Error error_type;

            public UInt64 start_index;
            public UInt64 len ;

            public Mlt3Event()
            {
                value = 0;
                error_type = MLT3Error.MLT3_ERROR_NONE;
                start_index = 0;
                len = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Mlt3Options
        {
            public IntPtr CancelFlag;//取消解码标志

            public Double BaudRate;//波特率

            public UInt32 KeepZeroCount;//零电平保持个数
        };
        

        public struct Mlt3Result
        {
            public IntPtr Mlt3Event;
            public UInt32 EventCount;
            public SerialProtocolType ProtocolType; // 协议类型
        };
        

    }
}

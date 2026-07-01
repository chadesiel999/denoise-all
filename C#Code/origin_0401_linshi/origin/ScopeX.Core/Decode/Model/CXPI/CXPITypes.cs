using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace ScopeX.Core.Decode
{
    internal partial class CXPIDecodeModel
    {
        public enum CXPIFieldType
        {
            CxpiNone = 0,
            CxpiPtype,//PTYPE字段
            CxpiPid,//PID字段
            CxpiFiLong,//长帧下的FI字段
            CxpiFiShort,//短帧下的FI字段
            CxpiFiDlcExt,//长帧DLCExt
            CxpiData,//数据
            CxpiCrc,//CRC校验数据
            CxpiIbs,//IBS空闲
        };
        public enum CXPIStatusType
        {
            CxpiNoError,
            CxpiByteError,
            CxpiCrcError,
            CxpiDlcError,
            CxpiDlcextError,
            CxpiParityError,
            CxpiFramingError,
        };
        [StructLayout(LayoutKind.Sequential)]
        public struct CXPIOptions
        {
            public IntPtr CancelFlag;//取消解码标志

            public Double BaudRate;//波特率
        };
        
        public unsafe struct CXPIEvent
        {
            public UInt64 Start;//事件起始：事件的起始标记

            public UInt64 StartIndex;//起始索引
            public UInt64 EndIndex;//结束索引

            public IntPtr ByteField;//值指针
            public UInt32 ByteFieldNum;//数据长度

            
            public CXPIEvent()
            {
                Start = 0;

                ByteField = IntPtr.Zero;
                ByteFieldNum = 0;

                StartIndex = 0;
                EndIndex = 0;
            }
        };

        /*字段*/
        public struct CXPIByteField
        {
            public UInt64 StartIndex;
            public UInt64 EndIndex;

            public ushort Value;

            public CXPIBitField StartBit;//起始位
            public CXPIBitField StopBit;//停止位
            public CXPIBitField ParityBit;//校验位

            public FIByteField FiByte;//FI字段

            public CXPIFieldType FieldType;//字段类型
            public CXPIStatusType StatusType;//解码状态

          
            public CXPIByteField()
            {
                StartIndex = 0;
                EndIndex = 0;

                Value = 0;

                StartBit = new CXPIBitField();
                StopBit = new CXPIBitField();
                ParityBit = new CXPIBitField();
                FiByte = new FIByteField();

                FieldType = CXPIFieldType.CxpiNone;
                StatusType = CXPIStatusType.CxpiNoError;

            }
        };

        /*FI字段*/
        public struct FIByteField
        {
            public CXPIBitField CounterBit;//FI_Counter
            public CXPIBitField WakeUpBit;//FI_Wakeup
            public CXPIBitField SleepBit;//FI_Sleep
            public CXPIBitField DlcBit;//FI_DLC

            public FIByteField()
            {
                CounterBit = new CXPIBitField();
                WakeUpBit = new CXPIBitField();
                SleepBit = new CXPIBitField();
                DlcBit = new CXPIBitField();
            }
        }

        /*比特域*/
        public struct CXPIBitField
        {
            public UInt64 StartIndex;
            public UInt64 EndIndex;
            public Byte Value;


            public CXPIBitField()
            {
                StartIndex = 0;
                EndIndex = 0;
                Value = 0;
            }
        };

        public struct CXPIResult
        {
            public IntPtr CXPIEvent;
            public UInt32 EventCount;
            public SerialProtocolType ProtocolType; // 协议类型
        };
        
        

    }
}

using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ScopeX.ComModel.ProtocolI2C;

namespace ScopeX.Core.Decode
{
    // I2C 地址位宽
    public enum I2cDataBitWidth
    {
        DataBitWidth7Bit = 7,
        DataBitWidth10Bit = 10,
    };

    // I2C帧类型
    public enum I2cDataType
    {
        I2cAddr7BitFiled ,  // 7bit地址字段
        I2cAddr10BitFiled,  // 10bit地址字段
        I2cDataFiled,       // 数据帧
    };

    // I2C输入定义
    [StructLayout(LayoutKind.Sequential)]
    public struct I2cOption
    {
        public I2cDataBitWidth DataBitWidthLen; //默认7位地址长度
    };

    // I2C错误
    [StructLayout(LayoutKind.Sequential)]
    public struct I2cError
    {
        public UInt64 ErrorAddrAckSize    ;  // 地址字段ACK错误个数
        public UInt64 ErrorDataAckSize    ;  // 数据字段ACK错误个数
        public UInt64 ErrorNackSize       ;
    };

    [StructLayout(LayoutKind.Sequential)]
    // I2C帧
    public struct I2cDataInfo
    {
        public I2cDataType Type;   // 数据类型
        public byte   Data;        // 数据位 
        public byte   Rw;          // 读写位
        public byte   Ack;         // 应答位
        public byte   AckError;    // 应答位状态 

        public UInt64 DataStartIndex;
        public UInt64 DataEndIndex;
        public UInt64 RwStartIndex;
        public UInt64 RwEndIndex;
        public UInt64 AckStartIndex;
        public UInt64 AckEndIndex;

    };
    // I2C事件
    struct I2cEvent
    {
        public UInt64     EventStartIndex;    // 事件开始index
        public UInt64     EventEndIndex;      // 事件结束index
        public byte       HasEventRestartEnd; // 事件结束状态 以restart结束
        public byte       HasEventStopEnd;    // 事件结束状态 以stop结束

        public IntPtr     DataInfoPtr;        // 数据帧指针
        public UInt32     DataInfoSize;       // 数据帧个数
       
        public I2cError   ErrorInfo;          // 错误信息

    };

    public struct I2cResult
    {
        public IntPtr Event;
        public Int32  EventCount;
        public Int32  protocol_type; //协议类型
    };
}

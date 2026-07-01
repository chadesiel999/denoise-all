using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ScopeX.ComModel.ProtocolI3C;

namespace ScopeX.Core.Decode
{
    // I3C包类型
    public enum I3CPacketType
    {
        I3C_PT_NONE,
        I3C_PT_SDR,  // SDR模式
        I3C_PT_DDR,  // HDR-DDR模式
        I3C_PT_TSR,  // HDR-TSR模式  
    };

    // I3C流程类型
    public enum SDRProcessType
    {
        SDR_PT_NONE = 0,
        SDR_PT_START,
        SDR_PT_RESTART,
        SDR_PT_END,
    };

    // 序列条件
    public enum SequenceCondition
    {
        SEQUENCE_CONDITION_NONE,
        SEQUENCE_CONDITION_START,
        SEQUENCE_CONDITION_RESTART,
        SEQUENCE_CONDITION_END,
        SEQUENCE_CONDITION_PULSE_END,
    };

    // CCC类型
    public enum CCCType
    {
        BROADCAST_ENEC = 0X00, // 广播 使能事件命令
        BROADCAST_DISEC = 0X01,
        BROADCAST_ENTAS0 = 0X02,
        BROADCAST_ENTAS1 = 0X03,
        BROADCAST_ENTAS2 = 0X04,
        BROADCAST_ENTAS3 = 0X05,
        BROADCAST_RSTDAA = 0X06,
        BROADCAST_ENTDAA = 0X07,
        BROADCAST_DEFTGTS = 0X08, // 之前协议叫 DEFSLVS
        BROADCAST_SETMWL = 0X09,
        BROADCAST_SETMRL = 0X0A,
        BROADCAST_ENTTM = 0X0B,
        BROADCAST_SETBUSCON = 0X0C,
        BROADCAST_ENDXFER = 0X12,
        BROADCAST_RESERVED = 0X1F,

        BROADCAST_ENTHDR0 = 0X20,
        BROADCAST_ENTHDR1 = 0X21,
        BROADCAST_ENTHDR2 = 0X22,
        BROADCAST_ENTHDR3 = 0X23,
        BROADCAST_ENTHDR4 = 0X24,
        BROADCAST_ENTHDR5 = 0X25,
        BROADCAST_ENTHDR6 = 0X26,
        BROADCAST_ENTHDR7 = 0X27,

        BROADCAST_SETXTIME = 0X28,
        BROADCAST_SETAASA = 0X29,

        BROADCAST_RSTACT = 0X2A,

        BROADCAST_DEFGRPA = 0X2B,
        BROADCAST_RSTGRPA = 0X2C,
        BROADCAST_MLANE = 0X2D,

        BROADCAST_VENDOR_EXTENSION = 0X61,
        BROADCAST_VENDOR_END = 0X7F,

        DIRECT_ENEC = 0X80,
        DIRECT_DISEC = 0X81,
        DIRECT_ENTAS0 = 0X82,
        DIRECT_ENTAS1 = 0X83,
        DIRECT_ENTAS2 = 0X84,
        DIRECT_ENTAS3 = 0X85,
        DIRECT_RSTDAA = 0X86,
        DIRECT_SETDASA = 0X87,
        DIRECT_SETNEWDA = 0X88,
        DIRECT_SETMWL = 0X89,
        DIRECT_SETMRL = 0X8A,
        DIRECT_GETMWL = 0X8B,
        DIRECT_GETMRL = 0X8C,
        DIRECT_GETPID = 0X8D,
        DIRECT_GETBCR = 0X8E,

        DIRECT_GETDCR = 0X8F,
        DIRECT_GETSTATUS = 0X90,
        DIRECT_GETACCMST = 0X91,
        DIRECT_GETMXDS = 0X94,
        DIRECT_GETHDRCAP = 0X95,
        DIRECT_SETXTIME = 0X98,
        DIRECT_GETXTIME = 0X99,
        DIRECT_VENDOR_EXTENSION = 0XE0,

        DIRECT_END = 0xFF,
    };

    // i3c字段类型
    public enum SDRFieldType
    {
       NONE = 0,

       COMMAND,               /// ccc命令

       RESERVER_ADDRESS,      // 保留地址 7E
       TARGET_ADDRESS,         // 目标地址 

       DATA,                   // 数据

       DYNAMIC_ADDR_MASTER = 1000, // 主机动态地址
       DYNAMIC_ADDR_SLAVE,         // 从机动态地址
       STATIC_I2C_ADDR,            // 静态I2C地址
       STATIC_I2C_SLVAE_ADDR,      // 静态I2C从机地址

       COUNT,                      // 数量
       DCR_MASTER,                 // 主机DCR 
       BCR_MASTER,                 // 主机BCR 

       DCR_SLAVE,                  // 从机DCR
       BCR_SLAVE,                  // 从机BCR 

       MSB,                        // 状态MSB             
       LSB,                        // 状态LSB

       MAXIMUM_WRITE_DATA_SPEED,   // 最大写数据速度
       MAXIMUM_READ_DATA_SPEED,    // 最大读数据速度

       MAXIMUM_READ_TURNAROUND_TIME, // 最大读 周期时间

    };

    // I3C拓展字段类型
    public enum SDRExpandType
    {
        SDR_ET_NONE = 0,
        SDR_ET_ACK,        // 回复
        SDR_ET_RW,         // 读/写
        SDR_ET_PARITY,     // 校验
    };

    // I3C错误类型
    public enum I3CErrorType
    {
        I3C_ERROR_NONE,

        I3C_UNFRAME,      // 不成帧

        I3C_ERROR_RW,    // 读写位错误 7E

        I3C_ERROR_PARITY, // 字段校验错误

        I3C_UNKNOWN_FIELD,// 字段未知


    };

    // I3C输入定义
    public struct I3COption
    {

    };

    //public struct SPMIError
    //{
    //    public SPMIErrorType spmi_error_type;
    //    public UInt64 error_start_index;
    //    public UInt64 error_end_index;
    //};

    [StructLayout(LayoutKind.Sequential)]
    public struct I3CError
    {
        public I3CErrorType error_type;          // 错误类型
        public UInt32 error_value;               // 错误值 ，计算值
    };

    [StructLayout(LayoutKind.Sequential)]
    // I3CPacket 包结构定义
    public struct I3CPacket
    {
        public I3CPacketType packet_type;    // i3c包类型

        public UInt64 start_index;           // 开始index
        public UInt64 end_index;             // 结束index

        public I3CError error;               // 错误

        public IntPtr i3c_packet;            // I3C 包
    };

    public struct SDRExpandField
    {
        public SDRExpandType field_type;     // I3C拓展字段类型
        public Byte field_value;              // 字段扩展位

        public UInt64 start_index;
        public UInt64 end_index;

        public I3CError error;               // 错误
    };

    public struct SDRField
    {
        public SDRFieldType field_type; // 字段类型
        public Byte field_value;           // 字段值

        public IntPtr expand_field; // 拓展字段指针
        public int expand_size;

        public UInt64 start_index;
        public UInt64 end_index;

        public I3CError error;               // 错误
    };

    public struct SDRPacket
    {
        public I3CPacketType packet_type;    // i3c包类型

        public UInt64 start_index;           // 开始index
        public UInt64 end_index;             // 结束index

        I3CError error;                      // 错误
        public IntPtr i3c_packet;            // I3C 包

        public SequenceCondition start_type; // i3c 开始类型

        public IntPtr sdr_field ;

        public UInt32 sdr_field_size;

        public SequenceCondition end_type; // i3c   结束类型
    }


    public struct I3CResult
    {
        public IntPtr Event;
        public Int32 EventCount;
        public Int32 protocol_type; // 协议类型
    };
}
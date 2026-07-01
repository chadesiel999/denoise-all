using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ScopeX.ComModel.ProtocolSPMI;

namespace ScopeX.Core.Decode
{
    // SPMI信号版本
    public enum SPMISignalVersion
    {
        SPMI_VERSION_NONE = 0, // 版本none
        SPMI_VERSION1,         // 1.0
        SPMI_VERSION2,         // 2.0
    };

    // 命令帧类型
    public enum CommandFrameType
    {
        NONE = -1,
        EXTENDED_REGISTER_WRITE = 0X00, // 寄存器类型
        RESET = 0X10,
        SLEEP = 0X11,
        SHUTDOWN = 0X12,
        WAKEUP = 0X13,
        AUTHENTICATE = 0X14,
        MASTER_READ = 0X15,
        MASTER_WRITE = 0X16,

        RESERVED = 0X19, // 17 18 19 1D 1E 1F RESERVED

        TRANSFER_BUS_OWNERSHIP = 0X1A,
        DEVICE_DESCRIPTOR_BLOCK_MASTER_READ = 0X1B,
        DEVICE_DESCRIPTOR_BLOCK_SLAVE_READ = 0X1C,

        EXTENDED_REGISTER_READ = 0X20,
        EXTENDED_REGISTER_WRITE_LONG = 0X30,
        EXTENDED_REGISTER_READ_LONG = 0X38,

        REGISTER_WRITE = 0X40,
        REGISTER_WRITE_1 = 0x50,

        REGISTER_READ = 0X60,
        REGISTER_READ_1 = 0x70,

        CFT_REGISTER_0_WRITE = 0X80,

    };

    // 仲裁等级
    public enum ArbitrateLevel
    {
        ARBITRATELEVEL_NONE = 0,
        ARBITRATELEVEL_ALERT, // slave 
        ARBITRATELEVEL_MASTER_PRIORITY, // 主优先级校验
        ARBITRATELEVEL_SR,  // slave二次校验
        ARBITRATELEVEL_MASTER_SECONDARY_ARBITRATION, // 主二次校验
    };


    // SPMI帧类型
    public enum SPMIFrameType
    {
        SPMI_FRAME_TYPE_NONE,

        SPMI_FRAME_TYPE_COMMAND,          // 命令帧

        SPMI_FRAME_TYPE_ADRESS,           // 地址帧
        SPMI_FRAME_TYPE_DATA,             // 数据帧

        SPMI_FRAME_TYPE_CHALLENGE_DATA,  // challenge data
        SPMI_FRAME_TYPE_RESPONSE_DATA,   // 响应数据
    };

    // 命令帧拓展字段
    public  enum CommandFrameExtension
    {
        CFE_NONE,
        CFE_BYTE_COUNT_4, // 4位 
        CFE_BYTE_COUNT_3, // 3位
        CFE_ADDRESS,    // 地址
        CFE_DATA,      // 数据

    };

    // 错误类型
    public enum SPMIErrorType
    {
        SPMIERRORTYPE_NONE,
        SPMIERRORTYPE_PARITY, //校验错

        SPMIERRORTYPE_UNSUPPORTED_COMMAND, // 不支持命令
        SPMIERRORTYPE_UNSUPPORTED_ADDRESS,  // 不支持地址
    };

    // SPMI输入定义

    [StructLayout(LayoutKind.Sequential)]
    public struct SPMIOption
    {
        public SPMISignalVersion spmi_signal_version; // 消息请求类型
    };

    // SPMI错误
    [StructLayout(LayoutKind.Sequential)]
    public struct SPMIError
    {
        public SPMIErrorType spmi_error_type;
        public UInt64 error_start_index;
        public UInt64 error_end_index;
    };

    [StructLayout(LayoutKind.Sequential)]
    // 仲裁信息
    public struct ArbitrationInfo
    {
        public ArbitrateLevel arbitrate_level;

        public Byte connect_bit ; // 连接位 判断是否有序列
        public UInt64 cbit_start_index ;
        public UInt64 cbit_end_index ;
         
        public Byte mastet_id;               // c bti 为1 则存在此信息
        public UInt64 masterid_start_index ;
        public UInt64 masterid_end_index ;
         
        public Byte alert_bit;   // 警报位 说明 slave 一级仲裁
        public UInt64 abit_start_index;
        public UInt64 abit_end_index;
         
        public Byte slave_addr;         //  从地址
        public UInt64 slave_start_index ; //
        public UInt64 slave_end_index ;   //
         
        public Byte master_priority_level; // 主优先级 一级信息
        public UInt64 mpl_start_index;
        public UInt64 mpl_end_index;
         
        public Byte sr_bit; // slave 次级仲裁
        public UInt64 srbit_start_index;
        public UInt64 srbit_end_index;
         
        public Byte secondary_master_priority_level; // 主优先级 二次仲裁
        public UInt64 secondary_mpl_start_index;
        public UInt64 secondary_mpl_end_index;
         
        public UInt64 arbitration_start_index ;       // 仲裁开始结束
        public UInt64 arbitration_end_index;
    };

    // SPMI 帧
    [StructLayout(LayoutKind.Sequential)]
    public struct SPMIFrame
    {
        public SPMIFrameType frame_type; // 帧类型

        public Byte field_content;                    // 帧内容
        public UInt64 field_start_index; // 帧内容开始index
        public UInt64 field_end_index;      // 帧内容结束index
         
        public Byte parity;   // 校验位
        public UInt64 parity_start_index; // 校验位内容开始index
        public UInt64 parity_end_index;   // 校验位内容开始index
         
        public Byte parity_result; // 校验结果
    };

    // SPMI事件
    struct SPMIEvent
    {
        public IntPtr arbitration_info; // 总线仲裁指针

        public Byte field_command_addr; // slave 从地址 低4bit  master 主地址 低2bit
        public UInt64 command_addr_start_index ;
        public UInt64 command_addr_end_index;

        public CommandFrameType command_frame_type;
        public SPMIFrame command_frame;                  // spmi命令帧
        public Byte field_command_frame_extension; // 命令帧拓展字段
        public UInt64 command_frame_start;


        public IntPtr spmi_frame_ptr;  // spmi 帧地址
        public Int32 spmi_frame_size;  // spmi 帧个数

        public Byte field_ack_nack;   // ack 和 nack
        public UInt64 ack_nack_start_index; // 校验位内容开始index
        public UInt64 ack_nack_end_index;   // 校验位内容开始index
         
        public UInt64 event_start_index; // 事件开始index
        public UInt64 event_end_index;   // 事件结束index
         
        public IntPtr spmi_error;
        public Int32 spmi_error_size;
    };

    public struct SPMIResult
    {
        public IntPtr Event;
        public Int32  EventCount;
        public Int32  protocol_type; // 协议类型
    };
}

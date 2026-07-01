using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ScopeX.ComModel.ProtocolPCIe;

namespace ScopeX.Core.Decode
{
    //public enum PcieVersion
    //{
    //    PCIE_VERSION_1,
    //    PCIE_VERSION_2,

    //    PCIE_VERSION_3,
    //    PCIE_VERSION_4,
    //    PCIE_VERSION_5,

    //    PCIE_VERSION_6,
    //};

    //编码类型
    public enum PcieEncodingType
    {
        PCIE_ENCODEING_8B10B,    // 8b10b编码
        PCIE_ENCODEING_128B130B, // 128b130b编码
        PCIE_ENCODEING_1B1B,     // 1b1b编码
    };

    //  TLP 包类型
    public enum TlpType
    {
        TLP_NONE,          // 未知类型
        TLP_MEMORY_IO,     // 内存和IO读写
        TLP_CONFIGURATION, // 配置读写
        TLP_COMPLETION,    // 完成报文
        TLP_MESSAGE,       // 消息报文
    };

    // tlp 格式
    public enum TLPFormat
    {
        TLP_NONE = -1,
        TLP_3DW_NODATA = 0,
        TLP_4DW_NODATA,
        TLP_3DW_WITHDATA,
        TLP_4DW_WITHDATA,
        TLP_PREFIX,
    };

    // pice错误类型
    public enum PcieErrorType
    {
        ERROR_NONE,           // NONE
        ERROR_NO_FRAMEHEADER, // 无帧头

        ERROR_DECODE_FAILED,     // 解码失败 8b10b,128b130b，1b1b
        ERROR_DESCRAMBLE_FAILED, // 解扰失败

        ERROR_NO_FRAME_TAIL, // 无帧尾

        ERROR_SKP_NO_COMPLETE, // SKP ORDER不完整

        ERROR_TLP_END_BAD,    // TLP END BAD
        ERROR_TLP_DATA_ERROR, // 数据错误
        ERROR_TLP_ECRC_ERROR, // ECRC校验错误
        ERROR_TLP_LCRC_ERROR, // LCRC校验错误

        ERROR_DLLP_DATA_ERROR, // DLLP数据错误
        ERROR_DLLP_CRC_ERROR,  // DLLP CRC错误
    };

    // pcie 包类型
    public enum PCIEPacketType
    {
        PACKET_TYPE_NONE,
        PACKET_TYPE_SKP_ORDER,
        PACKET_TYPE_TLP,
        PACKET_TYPE_DLLP,
    };

    public enum MessageRequestType
    {
        MRT_NONE,

        // 中断消息
        MRT_ASSERT_INTA,
        MRT_ASSERT_INTB,
        MRT_ASSERT_INTC,
        MRT_ASSERT_INTD,
        MRT_DEASSERT_INTA,
        MRT_DEASSERT_INTB,
        MRT_DEASSERT_INTC,
        MRT_DEASSERT_INTD,

        // 电源管理
        MRT_PM_ACTIVE_STATE_NAK,
        MRT_PM_PME,
        MRT_PME_TURN_OFF,
        MRT_PME_TO_ACK,

        // 错误消息
        MRT_ERR_COR,
        MRT_ERR_NONFATAL,
        MRT_ERR_FATAL,

        // 锁事务消息
        MRT_UNLOCK,

        // 插槽功率限制支持
        MRT_SET_SLOT_POWER_LIMIT,

        // 供应商定义消息
        MRT_VENDOR_DEFINED_TYPE0,
        MRT_VENDOR_DEFINED_TYPE1,

        // 忽略消息
        MRT_IGNORED_MESSAGE,

        // 延迟容忍度报告
        MRT_LTR,

        // 优化的缓冲区刷新/填充消息
        MRT_OBFF,
    };

    // DLLP消息类型
    public enum DLLPMessageType
    {
        DLLP_ACK = 0b00000000,
        DLLP_NAK = 0b00010000,
        DLLP_PM_ENTER_L1 = 0b00100000,
        DLLP_PM_ENTER_L23 = 0b00100001,
        DLLP_PM_ACTIVE_STATE_REQUEST_L1 = 0b00100011,
        DLLP_PM_REQUEST_ACK = 0b00100100,
        DLLP_VENDOR_SPECIFIC = 0b00110000,
        DLLP_INITFC1_P = 0b01000000,
        DLLP_INITFC1_NP = 0b01010000,
        DLLP_INITFC1_CPL = 0b01100000,
        DLLP_INITFC2_P = 0b11000000,
        DLLP_INITFC2_NP = 0b11010000,
        DLLP_INITFC2_CPL = 0b11100000,
        DLLP_UPDATEFC_P = 0b10000000,
        DLLP_UPDATEFC_NP = 0b10010000,
        DLLP_UPDATEFC_CPL = 0b10100000,
        DLLP_RESERVED,
    };

    // TLP message type
    public enum TLPMessageType
    {
        NONE = -1, // 空

        MRD,   // MRD
        MRDLK, // MRDLK
        MWR,

        IORD,
        IOWR,

        CFGRD0,
        CFGWR0,
        CFGRD1,
        CFGWR1,

        TCFGRD,
        TCFGWR,

        MSG_ROUTED_TO_ROOT_COMPLEX,
        MSG_ROUTED_BY_ADDRESS,
        MSG_ROUTED_BY_ID,
        MSG_BROADCAST_FROM_ROOT_COMPLEX,
        MSG_LOCAL_TERMINATE_AT_RECEIVER,
        MSG_GATHERED_AND_ROUTED_TO_ROOT_COMPLEX,
        MSG_RESERVED_TERMINATE_AT_RECEIVER,

        MSGD_ROUTED_TO_ROOT_COMPLEX,
        MSGD_ROUTED_BY_ADDRESS,
        MSGD_ROUTED_BY_ID,
        MSGD_BROADCAST_FROM_ROOT_COMPLEX,
        MSGD_LOCAL_TERMINATE_AT_RECEIVER,
        MSGD_GATHERED_AND_ROUTED_TO_ROOT_COMPLEX,
        MSGD_RESERVED_TERMINATE_AT_RECEIVER,

        CPL,
        CPLD,
        CPILK,
        CPIDLK,

        FETCHADD,
        SWAP,
        CAS,

        LPRFX_MR_IOV,
        LPRFX_VENDPREFIXL0,
        LPRFX_VENDPREFIXL1,

        EPRFX_EXTTPH,
        EPRFX_VENDPREFIXE0,
        EPRFX_VENDPREFIXE1,

        TLP_END = 100, // tlp结束

    };
    [StructLayout(LayoutKind.Sequential)]
    public struct PCIEError
    {
        public PcieErrorType ErrorType; // PCIE错误类型

        public UInt64 StartIndex; // 起始序号 在整数数据中
        public UInt64 EndIndex;   // 结束index
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct PciePacketInfo
    {
        public PCIEPacketType PacketType;
        public UInt32 PacketSize;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct MemoryIOTLP
    {
        public Byte field_last_dwbyte;  // 字段 last dw
        public Byte field_first_dwbyte; // 字段 first dw

        public UInt64 field_address; // 字段采用64位的，因为不确定具体是64bit还是32bit的地址
        public Byte field_reserve4; // 最后两位保留字段 ph
    };
    [StructLayout(LayoutKind.Sequential)]
    // 配置读写TLP 3DW
    public struct ConfigurationTLP
    {
        public Byte field_last_dwbyte;  // 字段 last dw
        public Byte field_first_dwbyte; // 字段 first dw

        public Byte field_bus_number;               // 字段 目标设备 bus number
        public Byte field_device_number;            // 字段 目标设备 设备number
        public Byte field_function_number;          // 字段 目标设备 function number
        public Byte field_reserver4;                // 字段 rsvd
        public Byte field_extended_register_number; // 字段ext number
        public Byte field_register_number;          // 字段注册number
        public Byte field_reserver5;                // 保留字段
    };
    [StructLayout(LayoutKind.Sequential)]
    // 继承TLP 完成TLP
    public struct CompletionTLP
    {
        public UInt16 field_completer_id;       // 字段 完成者id
        public Byte field_completion_status;   // 完成状态  存储在 低3bit
        public Byte field_byte_count_modifyed; // BCM字段
        public UInt16 field_byte_count;         // Byte count

        public Byte filed_completion_reserve; // 完成tlp的保留字段
        public Byte filed_lower_address;      // 低地址

        // 先屏蔽
        // uint64_t filed_payload;     // 是德解码 有此 参数，但规范里面不存在
    };
    [StructLayout(LayoutKind.Sequential)]
    // 继承TLP 内存IO读写请求TLP
    public struct MessageTLP
    {
        public MessageRequestType message_request_type; // 消息请求类型
        public Byte filed_message_code;              // code
    };
    [StructLayout(LayoutKind.Sequential)]
    // 基本消息
    // 保留消息结构  INTx,Power Manager
    public struct ReserveMessage
    {
        public MessageTLP tlp_message;

        public UInt16 field_reserved1;
        public UInt16 field_reserved2;
        public UInt16 field_reserved3;
        public UInt16 field_reserved4;
    };
    [StructLayout(LayoutKind.Sequential)]
    // 错误处理message
    public struct ErrorSignalMessage
    {
        public Byte field_ecs;
        public UInt16 field_reserved1;
        public UInt16 field_reserved2;
        public UInt16 field_reserved3;
        public UInt16 field_reserved4;
    };
    [StructLayout(LayoutKind.Sequential)]
    // vendor defined消息
    public struct VendorDefinedMessage
    {
        public Byte field_bus_number;      // 字段 目标设备 bus number
        public Byte field_device_number;   // 字段 目标设备 设备number
        public Byte field_function_number; // 字段 目标设备 function number

        public UInt16 field_vendor_id; // vendor id
        public UInt32 field_vendor_defined; // field defined
    };
    [StructLayout(LayoutKind.Sequential)]
    // ltr message
    public struct LTRMessage
    {
        public UInt16 field_reserved1;
        public UInt16 field_reserved2;
        public UInt16 field_no_snoop_latency;
        public UInt16 field_snoop_latency;
    };
    [StructLayout(LayoutKind.Sequential)]
    // obff message
    public struct OBFFMessage
    {
        public UInt16 field_reserved1;
        public UInt16 field_reserved2;
        public UInt16 field_reserved3;
        public UInt16 field_reserved4;
        public Byte field_OBFFCode;
    };
    [StructLayout(LayoutKind.Sequential)]
    // tlp前缀
    public struct TLPPrefix
    {
        public Byte field_fmt;  // 字段fmt  低3位
        public Byte field_type; // 字段type
         
        public Byte field_prefix_reserve1;
        public Byte field_prefix_reserve2;
        public Byte field_prefix_reserve3;
    };
    [StructLayout(LayoutKind.Sequential)]
    // Ack/Nak
    public struct AckNakDLLP
    {
        public UInt16 field_reserve1;
        public UInt16 field_acknak_seq_num;
    };
    [StructLayout(LayoutKind.Sequential)]
    // 功耗管理
    public struct PowerManagementDLLP
    {
        public Byte field_reserve1;
        public Byte field_reserve2;
        public Byte field_reserve3;
    };
    [StructLayout(LayoutKind.Sequential)]
    // 流量控制
    public struct FlowControlDLLP
    {
        public Byte field_virtual_channel; // VC ID
         
        public Byte field_reserve1;
         
        public Byte field_header_fc;
        public Byte field_reserve2;

        UInt16 field_data_fc;
    };
    [StructLayout(LayoutKind.Sequential)]
    // VendorDefined
    public struct VendorDefined
    {
        public Byte field_vendor_defined1;
        public Byte field_vendor_defined2;
        public Byte field_vendor_defined3;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct DLLPacket
    {
        public PCIEPacketType PacketType;
        public UInt32 PacketSize;
        public DLLPMessageType dllp_msg_type; // dllp 消息类型
         
        public Byte field_sdp;  //  字段sdp
        public Byte field_type; //  字段dlp类型
        public Byte field_zero; // 就是 0 字节 第5bit
         
        public AckNakDLLP packet_acknak;
        public PowerManagementDLLP packet_power_management;
        public FlowControlDLLP packet_flow_control;
        public VendorDefined packet_vendon_defined;
         
        public UInt16 field_dlp_crc;
        public Byte dllp_crc_result;
        
        public Byte field_end; // 结束字段
    }
    [StructLayout(LayoutKind.Sequential)]
    struct SkpOrderPacket
    {
        public PCIEPacketType PacketType;
        public UInt32 PacketSize;
        public Byte field_comma;
        public Byte field_skp1;
        public Byte field_skp2;
        public Byte field_skp3;
    }
    [StructLayout(LayoutKind.Sequential)]
    // TLP 内存IO包信息
    public struct TLPPacket
    {
        public  PCIEPacketType PacketType;
        public UInt32 PacketSize;

        public  Byte field_stp; // 物理层 字段stp
         
        public  IntPtr field_tlp_prefix;
        public  Byte field_prefix_size;             // tlp size
         
        public  Byte field_reserve;      // 链路层 保留字段 低4位
        public  UInt16 field_sequence_id; // 链路层 序列id
         
        public  TLPFormat tlp_format;
        public  TLPMessageType tlp_message_type;
         
        public  Byte field_fmt;  // 字段fmt  高3位
        public  Byte field_type; // 字段type 低4位
         
        public  Byte field_reserve1; // 字段 reserve1
        public  Byte field_tc;       // 字段 tc
        public  Byte field_reserve2; // 字段 reserve2
         
        public  Byte field_attr2; // 字段 属性 全部放入此选项
         
        public  Byte field_reserve3; // 字段 reserve3
         
        public  Byte field_th; // 字段 th
        public  Byte field_td; // 字段 td
        public  Byte field_ep; // 字段 ep
        public  Byte field_at; // 字段 at
         
        public UInt16 field_payload_length; // 字段 payload length
         
        public CompletionTLP field_completion; // 完成消息特有字段
         
        public UInt32 requester_transaction_id;       // 组合字段 传输id
        public UInt16 field_requester_id;             // 注册者id
        public Byte field_requester_bus_number;      // 字段   发送方bus number
        public Byte field_requester_device_number;   // 字段   发送方设备number
        public Byte field_requester_function_number; // 字段 发送方function number
        public Byte field_tag;                       // 字段tag
         
        public ConfigurationTLP field_config; // 配置消息特有字段
        public IntPtr field_message;        // message消息特有字段
         
        public MemoryIOTLP field_memory;      // 内存消息特有字段
         
        public IntPtr field_data;          // 数据指针
        public UInt32 field_data_size;     // 数据大小
         
        public UInt32 field_ecrc; // ecrc 可能没有，取决于TD字段
        public Byte ecrc_result;    // ecrc校验结果
         
        public UInt32 field_lcrc; // lcrc
        public Byte lcrc_result;    // lcrc校验结果
        
        public Byte field_end; // 结束字段

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PCIEOption
    {
        public PCIeVersion pcie_version;
        public PcieEncodingType pcie_protocol_type; // 默认8b10b
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PCIEEvent
    {
        public IntPtr PCIEErrorPtr;
        public Int32  ErrorEventCount;

        public IntPtr PciePacketPtr;
        //public PCIEPacketType PciePacketType;

        public UInt64 EventStartIndex; // 字节域开始
        public UInt64 EventEndIndex;   // 字节域结束
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct PCIEResult
    {
        public IntPtr PCIEEvent;
        public Int32  EventCount;
        public Int32  protocol_type; // 协议类型
    };
}

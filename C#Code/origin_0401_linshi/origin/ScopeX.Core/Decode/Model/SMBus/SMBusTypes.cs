using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ScopeX.ComModel.ProtocolSMBus;

namespace ScopeX.Core.Decode
{
    // SMBus命令类型枚举
    public enum SMBusCommandProtocol
    {
        Unknown = -1,

        QuickCommand = 0x00, //快速命令 无命令码
        ReceiveByte = 0x01,  //接收字节 无命令码
        SendByte = 0x02,     //发送字节 无命令码

        HostNotifyProtocol = 0x03, //主机通知命令 无命令码
        NotifyArpMaster = 0x04,  //ARP通知 无命令码
        WriteByte = 0x05,  //写字节命令
        WriteWord = 0x06,  //写字命令
        Write32 = 0x07,    //写32命令
        Write64 = 0x08,    //写64命令
        BlockWrite = 0x09, //写块命令

        ReadByteCommand = 0x0A,  //读字节命令
        ReadByteResponse = 0x0B, //读字节响应 
        ReadWordCommand = 0x0C,  //读字命令
        ReadWordResponse = 0x0D, //读字响应
        Read32Command = 0x0E,    //读32命令
        Read32Response = 0x0F,   //读32响应
        Read64Command = 0x10,    //读64命令
        Read64Response = 0x11,   //读64响应
        BlockReadCommand = 0x12,  //读块命令
        BlockReadResponse = 0x13, //读块响应

        ProcessCallCommand = 0x14,  //进程调用命令
        ProcessCallResponse = 0x15, //进程调用响应
        BlockWriteBlockReadProcessCallCommand = 0x16,  //块读写进程调用命令
        BlockWriteBlockReadProcessCallResponse = 0x17, //块读写进程调用响应
        //ARP命令(With PEC)  
        PrepareToArp = 0x18,             //准备ARP命令             W 
        ResetDevice = 0x19,               //设备复位命令           W
        ResetDeviceDirected = 0x1A,      //设备复位命令(定向)      W
        AssignAddress = 0x1B,            //分配地址命令            W

        GetUdidCommand = 0x1C,           //获取UDID命令            R
        GetUdidResponse = 0x1D,          //获取UDID命令响应        R 
        GetUdidDirectedCommand = 0x1E,  //获取UDID命令(定向)       R 
        GetUdidDirectedResponse = 0x1F, //获取UDID命令响应(定向)   R
    };
    // SMBusPEC
    public enum SMBusPEC
    {
        SmbusPecValid = 0x0,   // PEC无效
        SmbusPecInvalid = 0x1,   // PEC有效
    };
    // SMBus帧类型
    public enum SMBusDataType
    {
        SmbusAddrFiled,        // 地址字段
        SmbusHostAddrFiled,   // 主机地址字段
        SmbusCommandFiled,     //命令字段
        SmbusCommandDeviceAddrFiled,  // 命令字段_设备地址
        SmbusBlockCountFiled, //读/写块字段
        SmbusDataFiled,       // 数据帧
        SmbusPecFiled,        // PEC帧 可选
    };
    // SMBusARP数据类型
    public enum SMBusArpFiledType
    {
        DeviceCapabilities, // 1
        VersionRevision,    // 1
        VendorId,           // 2
        DeviceId,           // 2
        Interface,          // 2
        SubsystemVendorId,  // 2
        SubsystemDeviceId,  // 2
        VendorSpecificId,   // 4
        DeviceSlaveAddress, // 1 Assigned Address

        //DynamicAndVolatileAddressDevice, // 1
        //UdidVersion1,                    // 1
        //VendorId,                        // 2
        //DeviceId,                        // 2
        //Reserved,                        // 2
        //SubsystemVendorId,               // 2
        //SubsystemDeviceId,               // 2
        //VendorSpecificId,                // 4
        //DeviceSlaveAddress,              // 1
    };
    // SMBus输入定义
    [StructLayout(LayoutKind.Sequential)]
    public struct SMBusOption
    {
        public SMBusPEC SmbusPecState; //默认PEC无效
    };

    // SMBus错误信息
    [StructLayout(LayoutKind.Sequential)]
    public struct SMBusErrorInfo
    {
        public byte HasCrcError;
        public byte HasAckError;
        public byte HasNackError;

        public UInt16 CrcErrorValue;  // CRC计算值
        public UInt16 AckErrorCount;  // ACK错误
        public UInt16 NackErrorCount;  // NACK错误
    };


    [StructLayout(LayoutKind.Sequential)]
    // SMBus 帧
    public struct SMBusDataInfo
    {
        public SMBusDataType Type; // 数据类型
        public byte Data;        // 数据位 
        public byte Rw;          // 读写位
        public byte Ack;         // 应答位
        public byte AckError;    // 应答位状态 

        public UInt64 DataStartIndex;
        public UInt64 DataEndIndex;
        public UInt64 RwStartIndex;
        public UInt64 RwEndIndex;
        public UInt64 AckStartIndex;
        public UInt64 AckEndIndex;

    };
    // SMBusARP帧传输格式
    [StructLayout(LayoutKind.Sequential)]
    public struct SMBusArpDataInfo
    {
        public SMBusArpFiledType ArpType;
        public byte byte_start_index; // 数据起始索引
        public byte byte_end_index;   // 数据结束索引
        public byte bit_count;        // 位宽
    };

    // SMBus事件
    struct SMBusEvent
    {
        public UInt64 EventStartIndex;                  // 事件开始index
        public UInt64 EventEndIndex;                    // 事件结束index
        public byte HasEventRestartEnd;                 // 事件结束状态 以restart结束
        public byte HasEventStopEnd;                    // 事件结束状态 以stop结束

        public SMBusCommandProtocol EventProtocol;      // 事件协议命令类型
        public IntPtr DataInfoPtr;                      // 数据帧指针
        public UInt32 DataInfoSize;                     // 数据帧个数
        public IntPtr ArpBytesPtr;                      // SMBus ARP命令指针
        public byte ArpBytesSize;                       // SMBus ARP命令字节数
        public SMBusErrorInfo SmbusErrorInfo;           // 错误信息

    };

    public struct SMBusResult
    {
        public IntPtr Event;
        public Int32  EventCount;
        public Int32  protocol_type; //协议类型
    };
}

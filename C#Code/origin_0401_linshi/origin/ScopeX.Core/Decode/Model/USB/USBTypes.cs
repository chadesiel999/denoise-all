// // ******************************************************************
// //       /\ /|       @File         Protocol_USB_Options.cs
// //       \ V/        @Brief
// //       | "")       @Author        lijinwen, ghz005@uni-trend.com.cn
// //       /  |        @Creation      2024-1-18
// //      /  \\        @Modified      2024-10-24
// //    *(__\_\		@Modified	   2025-4-2  by ZXL
// // ******************************************************************
using System;
using System.Runtime.InteropServices;
using ScopeX.ComModel;
using static ScopeX.Core.Decode.DecoderTypes;
using static ScopeX.ComModel.ProtocolUSB;
namespace ScopeX.Core.Decode 
{
    //[StructLayout(LayoutKind.Sequential)]
    //public struct UsbOptions
    //{
    //    public IntPtr CancelFlag;//取消解码标志
    //    public DiffSignalType SignalType;
    //    public Double SamplingFrequency;
    //    public ProtocolUSB.SignalRate USBSpeed;
    //    public Boolean AutoClock;
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //internal struct UsbResultCellStruct
    //{
    //    public Int64 StartIndex;
    //    public Int64 Length;
    //    public IntPtr Datas;
    //    public Int64 DatasCount;
    //    public Boolean IsEventInfo;
    //    public UsbResultCellStruct()
    //    {
    //        StartIndex = -1;
    //        Length = -1;
    //        DatasCount = 0;
    //        Datas = IntPtr.Zero;
    //        IsEventInfo = false;
    //    }

    //}
    //[StructLayout(LayoutKind.Sequential)]
    //public struct UsbResultStruct
    //{
    //    public IntPtr DecodeEvents;
    //    public UInt32 DecodeEventCount;
    //    public SerialProtocolType ProtocolType;

    //    public Boolean DecodeResultNeedUpdate;
    //    public Boolean DecodeEventNeedUpdate;
    //    public IntPtr DecodeResultCells;

    //    public UInt64 DecodeResultCount;

    //    public ProtocolUSB.SignalRate USBSpeed;
    //    public IntPtr DecoderPtr;
    //}

    /// <Author>
    /// ZXL
    /// </Author>

    [StructLayout(LayoutKind.Sequential)]
    public struct UsbOptions
    {

        public USBInputType input_type;
        public USBSpeed usb_speed;
    }


    [StructLayout(LayoutKind.Sequential)]
    //USB字段内容
    public struct UsbField
    {
        public UInt64 start_index;
        public UInt64 end_index;

        public UInt32 bit_value;              //将16个bit的值按位存储在一个变量中
        public Byte bit_size;               //字段bit长度

        public USBFiledType field_type;            //字段类型
        public USBPacketError field_error;         //字段错误类型
    };

    [StructLayout(LayoutKind.Sequential)]
    //USB数据字节
    public struct UsbData
    {
        public UInt64 start_index;
        public UInt64 end_index;

        public Byte data_value; //将数据一个字节一个字节存入
    };
    [StructLayout(LayoutKind.Sequential)]
    //USB传输包
    public struct UsbPacket
    {
        public UInt64 start_index;                            //包起始索引(绘制竖线)
        public UInt64 end_index;

        public IntPtr field;                  //包字段信息
        public UInt32 field_size; //包字段大小(不包括数据)

        public IntPtr data_byte;              //数据(仅数据包有)
        public UInt32 data_size;                     //数据字节大小

        public UsbPacketType packet_type;

        public UInt16 recv_crc_value;
        public UInt16 calculate_crc_value;
        public Boolean is_verified;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct UsbResult
    {
        public IntPtr Event;
        public Int32 EventCount;
        public Int32 protocol_type; // 协议类型
    };

    public enum  UsbPacketType
    {
        //NO_DEFINE = 0,
        // Token
        SOF = 0b0101, //pid:0xA5 fn crc5
        SETUP = 0b1101, //pid:0xB4 addr endp crc5
        IN = 0b1001,    //pid:0x96 addr endp crc5
        OUT = 0b0001,   //pid:0x87 addr endp crc5

        //PRE = 0b1100, //pid:1100 主机设备发送的前导码，启用下游低速设备
        SPLIT = 0b1000, //pid:1000 addr endp crc5
        PING = 0b0100,  //pid:0100 addr endp crc5

        // Data
        DATA0 = 0b0011, //pid:0xFC data crc16
        DATA1 = 0b1011, //pid:0xF4 data crc16
        DATA2 = 0b0111,
        M_DATA = 0b1111,

        //HandShack          //pid:handshack
        ACK = 0b0010, //pid:0010
        NAK = 0b1010, //pid:1010
        STALL = 0b1110, //pid:1110
        NYET = 0b0110,  //pid:0110

        ERR = 0b1100, //pid:1100

        //Special
        RESERVED = 0b0000 //pid:0000
    };

    public enum USBPacketError
    {
        NO_ERROR = 0,
        SYNC_ERROR,
        PID_ERROR,
        CRC_ERROR,
        EOP_ERROR
    };
}





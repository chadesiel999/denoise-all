using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ScopeX.ComModel.ProtocolPSI5;

namespace ScopeX.Core.Decode
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PSI5Options
    {
        public Psi5BaudMode baud_rate;              //传输模式
        public Psi5SerialMessage serial_Message;     //串行通道开关 0/2bit
        public Psi5FrameControl frame_control_size; //帧控制 0-3bit
        public Psi5Status state_size;               //状态位 0-2bit
        public UInt32 data_a_size;                 //数据域A 10-24bit
        public UInt32 data_b_size;                 //数据域B 0-10bit
        public Psi5CheckType check_type;
    }
    //PSI5 字段信息(标签)
    [StructLayout(LayoutKind.Sequential)]
    public struct Psi5FieldInfo
    {
        public UInt64 start_index; // 字段开始index
        public UInt64 end_index;   // 字段结束index

        //public IntPtr data_byte;  //8位十进制数据用于画图
        public UInt32 data_byte_len;   //字节个数

        public UInt32 data_value; //字段数据值
        public UInt32 data_bin_len; // 字段长度bit

        Psi5FieldErrorStatus error_status; //字段是否有误
        public Psi5FieldInfo() 
        { 
            this.start_index = 0;
            this.end_index = 0;

            //data_byte = IntPtr.Zero;
            data_byte_len = 0;

            data_value = 0;
            data_bin_len = 0;

            error_status = Psi5FieldErrorStatus.FIELD_NO_ERROR;
        }
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PSI5Event
    {
        //数据帧
        public Psi5FieldInfo event_start_bit;        // 起始位
        public Psi5FieldInfo event_message;          // 串行通道消息
        public Psi5FieldInfo event_frame_control;    // 事件表帧控制
        public Psi5FieldInfo event_status;           // 事件表状态位
        public Psi5FieldInfo event_data_b;           // 事件表数据域B
        public Psi5FieldInfo event_data_a;           // 事件表数据域A
        public Psi5FieldInfo event_check_num;        // 校验位

        //数据A分包
        public Psi5FieldInfo event_data_a_package_rest; // 事件表数据域A分包以后剩余数据
        public Psi5FieldInfo event_init_data;           // 事件表初始化数据
        public Psi5FieldInfo event_data_status;         // 事件表数据域A中最高10位表示的状态位

        //public IntPtr Psi5FieldPtr;//?


        public UInt64 EventStartIndex; // 字节域开始
        public UInt64 EventEndIndex;   // 字节域结束

        //实际校验值
        public Psi5CheckType check_type;
        public UInt32 crc_check_value;
        public Boolean is_verified;
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct PSI5Result
    {
        public IntPtr Event;
        public Int32 EventCount;
        public Int32 protocol_type; // 协议类型
    };

    public enum DataARange
    {
    //Status & Error Messages Priority:2  event_data_status 481-500
    BIDIRECTIONAL_COMMUNICATION_RC_OK = 0x1E1, 
    BIDIRECTIONAL_COMMUNICATION_RC_ERROR = 0x1E2,

    SENSOR_READY_BUT_UNLOCKED = 0x1E6,
	SENSOR_READY,
    SENSOR_BUSY,
    SENSOR_IN_SERVICE_MODE,

    SENSOR_DEFECT = 0x1F4,

    //Block ID  Priority:3 event_init_data  512-527
    BLOCK_ID_1 = 0x200,
    BLOCK_ID_2,
    BLOCK_ID_3,
    BLOCK_ID_4,
    BLOCK_ID_5,
    BLOCK_ID_6,
    BLOCK_ID_7,
    BLOCK_ID_8,
    BLOCK_ID_9,
    BLOCK_ID_10,
    BLOCK_ID_11,
    BLOCK_ID_12,
    BLOCK_ID_13,
    BLOCK_ID_14,
    BLOCK_ID_15,
    BLOCK_ID_16 = 0x20F,

    // Data for Initialization Priority:3 event_init_data 528-543
    STATUS_DATA_0000 = 0x210,
    STATUS_DATA_0001,
    STATUS_DATA_0010,
    STATUS_DATA_0011,
    STATUS_DATA_0100,
    STATUS_DATA_0101,
    STATUS_DATA_0110,
    STATUS_DATA_0111,
    STATUS_DATA_1000,
    STATUS_DATA_1001,
    STATUS_DATA_1010,
    STATUS_DATA_1011,
    STATUS_DATA_1100,
    STATUS_DATA_1101,
    STATUS_DATA_1110,
    STATUS_DATA_1111 = 0x21F,

    };

}

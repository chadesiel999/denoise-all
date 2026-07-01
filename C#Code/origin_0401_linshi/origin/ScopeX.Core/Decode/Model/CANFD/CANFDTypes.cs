using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    /// <summary>
    /// /C++定义使用
    /// </summary>
    public enum CANFDDataErrorType
    {
        NoError,
        BitPaddingError // 位填充错误
    };
    [StructLayout(LayoutKind.Sequential)]
    public struct CANOptions
    {
        public IntPtr CancelFlag; //取消解码标志
        public ProtocolCANFD.SignalType SignalType;
        public UInt32 SdSignalRate;
        public UInt32 FdSignalRate;
        public Double SdSamplePointRate;
        public Double FdSamplePointRate;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CANEventFieldInfoCPP
    {
        public UInt64 StartIndex; // 事件字段起始索引
        public UInt64 Len;         // 事件字段长度
        public Byte HasData;        // 该字段是否有效
        public Byte ErrorType;   // 字段错误类型，0-无错误
    };
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CANEventDataInfoCPP
    {
        public UInt64 StartIndex; // 事件字段起始索引
        public UInt64 Len;         // 事件字段长度
        public Byte HasData;        // 该字段是否有效
        public Byte ErrorType;   // 字段错误类型，0-无错误
        public Byte Data; // 事件字段数据
    };
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    //MAC帧封装信息
    public struct CANEventCPP
    {
        public CANEventFieldInfoCPP SOF;

        public CANEventFieldInfoCPP StandardIdInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public Byte[] StandardId;

        public CANEventFieldInfoCPP SRRInfo;
        public Byte SRR;

        public CANEventFieldInfoCPP ExtIdInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Byte[] ExtId;

        public CANEventFieldInfoCPP RRSInfo;
        public Byte RRS;

        public CANEventFieldInfoCPP IdeInfo;
        public Byte Ide;

        public CANEventFieldInfoCPP FDFInfo;
        public Byte FDF;

        public CANEventFieldInfoCPP ResInfo;
        public Byte Res;

        public CANEventFieldInfoCPP BRSInfo;
        public Byte BRS;

        public CANEventFieldInfoCPP ESIInfo;
        public Byte ESI;

        public CANEventFieldInfoCPP DLCInfo;
        public Byte DLC;

        public Byte PacketType;

        public IntPtr DataInfos;
        public UInt32 DataInfosCnt;

        public CANEventFieldInfoCPP StuffInfo;
        public Byte Stuff;

        public CANEventFieldInfoCPP StuffParityInfo;
        public Byte StuffParity;
        public Byte SuccessStuffParity;

        public CANEventFieldInfoCPP CRCInfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Byte[] CRC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Byte[] SuccessCRC;
        public Byte CRCBitCount;

        public CANEventFieldInfoCPP ACKInfo;
        public Byte ACK;

        public CANEventFieldInfoCPP EOFInfo;
        public Byte EOF;
    };
    //完整的数据解码信息
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CANResult
    {
        public IntPtr EventInfosPtr; // 结果事件列表
        public UInt32 EventCount; // 结果事件个数
        public SerialProtocolType ProtocolType; //协议类型
    }
    internal class CANFDPacketInfoPK
    {
        public CANEventCPP PacketInfo;
        public List<CANEventDataInfoCPP> DataInfos;
    }
}

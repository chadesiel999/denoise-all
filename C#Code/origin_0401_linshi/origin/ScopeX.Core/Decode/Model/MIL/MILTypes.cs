using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    internal partial class MILDecodeModelCPP
    {
        public enum DataErrorType
        {
            NoError,
            SyncError,      // 同步段错误
            BitLevelError, //电平值错误
            PairtyError     // 校验错误
        }

        ////C++定义
        [StructLayout(LayoutKind.Sequential)]
        public struct MILOptions
        {
            public IntPtr CancelFlag; //取消解码标志
            public ProtocolMIL.SignalRate SignalRateType;
            public UInt32 SignalRate;
            public ProtocolCommon.Polarity Polarity;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EventFiledInfo
        {
            public Byte HasData;
            public UInt64 StartIndex;
            public UInt64 Length;
            public Byte ErrorType;
            public Byte BitCount;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MILEventCPP
        {
            public EventFiledInfo SOF; // 起始位
            public MILPacketType PacketType;
            public EventFiledInfo Sync; // 同步段

            //Command
            public EventFiledInfo RTAInfo;        // 远程终端地址信息
            public Byte RTA;                     // 远程终端地址
            public EventFiledInfo TRInfo;         // TR信息
            public Byte TR;                      // TR值
            public EventFiledInfo SubAddresInfo; // 子地址信息
            public Byte SubAddress;             // 子地址值
            public EventFiledInfo ModeCodeInfo; // 数据字计数、方式代码信息
            public Byte ModelCode;              // 数据字计数、方式代码值

            //Data
            public EventFiledInfo DataInfo; // 数据信息
            public UInt16 Data;            // 数据值
            public UInt16 TempData;       // 数据

            // Status
            public EventFiledInfo MessageErroInfo;                  // 消息差错信息
            public Byte MessageError;                               // 消息差错信息值
            public EventFiledInfo InstrumentationInfo;                // 测试手段信息
            public Byte Instrumentation;                             // 测试手段值
            public EventFiledInfo ServiceRequestInfo;                // 服务请求信息
            public Byte ServiceResquest;                            // 服务请求信息值
            public EventFiledInfo ReservedInfo;                       // 保留信息
            public Byte Reserved;                                    // 保留
            public EventFiledInfo BroadcastCommandReceivedInfo;     // 广播指令接收信息
            public Byte BroadcastCommandReceived;                  // 广播指令接收
            public EventFiledInfo BusyInfo;                           // 忙信息
            public Byte Busy;                                        // 忙
            public EventFiledInfo SubsystemFlagInfo;                 // 子系统标志信息
            public Byte SubSystemFlag;                              // 子系统标志
            public EventFiledInfo DynamicBusControlAcceptanceInfo; // 动态总线控制接收信息
            public Byte DynamicBusControlAcceptance;              // 动态总线控制接收
            public EventFiledInfo TerminalFlagInfo;                  // 终端标志信息
            public Byte TerminalFlag;                               // 终端标志

            public EventFiledInfo ParityInfo; // 校验信息
            public Byte Parity;

            public EventFiledInfo EOF; // 结尾 
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MILResult
        {
            public IntPtr EventInfosPtr;
            public UInt32 EventCount;
            public SerialProtocolType ProtocolType; //协议类型
        }
    }
}

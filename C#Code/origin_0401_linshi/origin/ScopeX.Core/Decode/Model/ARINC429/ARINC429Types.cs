using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    internal partial class ARINC429DecodeModelCPP
    {
        /// <summary>
        /// /C++定义使用
        /// </summary>
        public enum DataErrorType
        {
            NoError,
            BitLevelError, // 位数据信号电平值错误
            ZeroBitLevelError, // 归零电平值错误
            PairtyError // 校验错误
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct ARINC429Options
        {
            public IntPtr CancelFlag; //取消解码标志
            public ProtocolARINC429.SignalRate SignalRateType;
            public ProtocolARINC429.DecodeMode DecodeMode;
            public UInt32 SignalRate;
            public ProtocolCommon.Polarity Polarity;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EventFiledInfo
        {
            public Byte HaData;
            public UInt64 StartIndex;
            public UInt64 Length;
            public Byte ErrorType;
            public Byte BitCount;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Arinc429EventCPP
        {
            public EventFiledInfo GAPInfo;
            public Byte GAP; // 帧间隙
            public EventFiledInfo SOF; // 起始信息
            public EventFiledInfo EOF; // 结束信息

            public EventFiledInfo ExtraDatInfo; // 多余信息
            public Byte ExtraData; // 多余数据

            public EventFiledInfo ParityInfo; // 校验位信息
            public Byte Parity; // 校验位

            public EventFiledInfo SSMInfo; // 信号状态矩阵信息
            public Byte SSM; // 信号状态矩阵

            public EventFiledInfo DataInfo; // 数据信息
            public Int32 Data; // 数据

            public EventFiledInfo SDIInfo; // 源目的标记位信息
            public Byte SDI; // 源目的标记位

            public EventFiledInfo LabelInfo; // 标签信息
            public Byte Label; // 标签
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct ARINC429Result
        {
            public IntPtr EventInfosPtr;
            public UInt32 EventCount;
            public SerialProtocolType ProtocolType; //协议类型
        }
    }
}

using MathNet.Numerics.LinearAlgebra;
using ScopeX.ComModel;
using ScopeX.Core.Decode.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using static ScopeX.Core.Decode.EthernetDecodeModel;

namespace ScopeX.Core.Decode
{

    [StructLayout(LayoutKind.Sequential)]
    public struct CPHYEventCPP
    {
        public EventFiledInfo SyncWordInfo;  // 16bit同步信息1

        public Int64 PacketDataIndex;         // Packet Data开始位置

        public EventFiledInfo ResInfo1;      // 5bit保留域同步信息1
        public Byte Res1;                    // 5bit保留域同步数据1
        public EventFiledInfo ResInfo2;      // 5bit保留域同步信息2
        public Byte Res2;                    // 5bit保留域同步数据2
        public EventFiledInfo VcxInfo1;      // 3bit虚拟通道扩展域（高3位）信息1
        public Byte Vcx1;                    // 3bit虚拟通道扩展域（高3位）数据1
        public EventFiledInfo VcxInfo2;      // 3bit虚拟通道扩展域（高3位）信息2
        public Byte Vcx2;                    // 3bit虚拟通道扩展域（高3位）数据2

        public EventFiledInfo VcInfo1;       // 2bit虚拟通道（低2位）信息1
        public Byte Vc1;                     // 2bit虚拟通道（低2位）数据1
        public EventFiledInfo VcInfo2;       // 2bit虚拟通道（低2位）信息2
        public Byte Vc2;                     // 2bit虚拟通道（低2位）数据2

        public EventFiledInfo DataTypeInfo1; // 6bit数据类型信息1
        public Byte DataType1;               // 6bit数据类型数据1
        public EventFiledInfo DataTypeInfo2; // 6bit数据类型信息2
        public Byte DataType2;               // 6bit数据类型数据2

        public EventFiledInfo WordCountInfo1;// 16bit Byte数目信息1
        public UInt16 WordCount1;            // 16bit Byte数目数据1
        public EventFiledInfo WordCountInfo2;// 16bit Byte数目信息2
        public UInt16 WordCount2;            // 16bit Byte数目数据2

        public UInt32 ShortDataLen;            // 短包1长度 - 数据长度

        public IntPtr ShortDataStart1;   // 短包1开始位置 - 标签
        public IntPtr ShortDataLength1;  // 短包1长度 - 标签
        public IntPtr ShortData1;        // 短包数据1
        public IntPtr ShortDataStart2;   // 短包2开始位置 - 标签
        public IntPtr ShortDataLength2;  // 短包2长度 - 标签
        public IntPtr ShortData2;        // 短包数据2

        public UInt32 LongDataLen;         // 长包长度 - 数据长度            
        public IntPtr LongDataStart;     // 长包开始位置 - 标签  
        public IntPtr LongDataLength;    // 长包数据长度 - 标签
        public IntPtr LongData;          // 长包数据

        public EventFiledInfo PhChecksumInfo1;// 16bit CRC包头校验信息1
        public UInt16 PhChecksum1;            // 16bit CRC包头校验数据1
        public EventFiledInfo PhChecksumInfo2;// 16bit CRC包头校验信息2
        public UInt16 PhChecksum2;            // 16bit CRC包头校验数据2

        public EventFiledInfo PdChecksumInfo; // 16bit CRC数据校验信息 - 长包
        public UInt16 PdLongChecksum;         // 16bit CRC数据校验数据 - 长包

        public UInt16 PhCalcChecksum1;        // 16bit CRC包头部分计算出的校验数据1       
        public UInt16 PhCalcChecksum2;        // 16bit CRC包头部分计算出的校验数据2
        public UInt16 PdCalcLongChecksum;     // 16bit CRC数据校验数据 - 长包

        public Byte IsLongPacket;             // 判断是否为长包

        public Byte IncompletedError;         // 包不全信息
        public Byte Ph1CrcError;              // 包头1crc信息错误
        public Byte Ph2CrcError;              // 包头2crc信息错误
        public Byte PdCrcError;               // 数据crc信息错误

        public CPHYEventCPP() {
            PacketDataIndex = 0;
            SyncWordInfo = new EventFiledInfo();
            ResInfo1 = new EventFiledInfo();
            Res1 = 0;
            ResInfo2 = new EventFiledInfo();
            Res2 = 0;
            VcxInfo1 = new EventFiledInfo();
            Vcx1 = 0;
            VcxInfo2 = new EventFiledInfo();
            Vcx2 = 0;
            VcInfo1 = new EventFiledInfo();
            Vc1 = 0;
            VcInfo2 = new EventFiledInfo();
            Vc2 = 0;
            DataTypeInfo1 = new EventFiledInfo();
            DataType1 = 0;
            DataTypeInfo2 = new EventFiledInfo();
            DataType2 = 0;
            WordCountInfo1 = new EventFiledInfo();
            WordCount1 = 0;
            WordCountInfo2 = new EventFiledInfo();
            WordCount2 = 0;
            ShortDataLen = 0;
            ShortDataStart1 = IntPtr.Zero;
            ShortDataLength1 = IntPtr.Zero;
            ShortData1 = IntPtr.Zero;
            ShortDataStart2 = IntPtr.Zero;
            ShortDataLength2 = IntPtr.Zero;
            ShortData2 = IntPtr.Zero;
            LongDataLen = 0;           
            LongDataStart = IntPtr.Zero;
            LongDataLength = IntPtr.Zero;
            LongData = IntPtr.Zero;
            PhChecksumInfo1 = new EventFiledInfo();
            PhChecksum1 = 0;
            PhChecksumInfo2 = new EventFiledInfo();
            PhChecksum2 = 0;
            PdChecksumInfo = new EventFiledInfo();
            PdLongChecksum = 0;
            PhCalcChecksum1 = 0;
            PhCalcChecksum2 = 0;
            PdCalcLongChecksum = 0;
            IsLongPacket = 0;
            IncompletedError = 0;
            Ph1CrcError = 0;
            Ph2CrcError = 0;
            PdCrcError = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct EventFiledInfo
    {
        public Byte HasData;
        public UInt64 StartIndex;
        public UInt64 Length;

        public EventFiledInfo()
        {
            HasData = 0;
            StartIndex = 0;
            Length = 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CPHYOptions
    {
        public IntPtr CancelFlag;//取消解码标志
        public ProtocolCPHY.SignalType SignalType;
        public ProtocolCPHY.SubType SubType;
        public Int64 BitRate;
        public UInt64 SamplePoints; //总采样点数，不作为界面的输入参数
    };

    public struct CPHYResultInfoCPP
    {
        public IntPtr CPHYEventPtr;
        public Int32 EventCount;
        public SerialProtocolType ProtocolType; // 协议类型
    };
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    internal struct CPHYPacketInfo
    {
        public EventFiledInfo SyncWordInfo;  // 16bit同步信息1

        public EventFiledInfo VcInfo1;        // 2bit虚拟通道（低2位）信息1
        public Byte Vc1;                      // 2bit虚拟通道（低2位）数据1
        public EventFiledInfo VcInfo2;        // 2bit虚拟通道（低2位）信息2
        public Byte Vc2;                      // 2bit虚拟通道（低2位）数据2

        public EventFiledInfo DataTypeInfo1;  // 6bit数据类型信息1
        public Byte DataType1;                // 6bit数据类型数据1
        public EventFiledInfo DataTypeInfo2;  // 6bit数据类型信息2
        public Byte DataType2;                // 6bit数据类型数据2

        public EventFiledInfo WordCountInfo1; // 16bit Byte数目信息1
        public UInt16 WordCount1;             // 16bit Byte数目数据1
        public EventFiledInfo WordCountInfo2; // 16bit Byte数目信息2
        public UInt16 WordCount2;             // 16bit Byte数目数据2

        public UInt32 ShortDataLen;             // 短包1长度 - 数据长度

        public Int64[] ShortDataStart1;      // 短包1开始位置 - 标签
        public Int16[] ShortDataLength1;     // 短包1长度 - 标签
        public Byte[] ShortData1;             // 短包数据1
        public Int64[] ShortDataStart2;      // 短包2开始位置 - 标签
        public Int16[] ShortDataLength2;     // 短包2长度 - 标签
        public Byte[] ShortData2;             // 短包数据2

        public UInt32 LongDataLen;              // 长包长度 - 数据长度            
        public Int64[] LongDataStart;        // 长包开始位置 - 标签  
        public Int16[] LongDataLength;       // 长包数据长度 - 标签
        public Byte[] LongData;               // 长包数据

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

    }
}

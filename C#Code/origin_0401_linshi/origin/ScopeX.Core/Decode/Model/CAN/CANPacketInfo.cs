using System;
using System.Collections.Generic;

namespace ScopeX.Core.Decode;

internal partial class CANDecodeModel
{
    class CANPacketInfo
    {
        public Int32 SOFIndex;
        public Int32 SOFLen;
        public Boolean SOF;

        public Boolean HasStandardID;
        public Int32 StandardIDIndex;
        public Int32 StandardIDLen;
        public Byte[] StandardID;
        internal UInt64 TempStandardID;

        public FrameType FrameType;

        public Boolean HasSRR;
        public Int32 SRRIndex;
        public Int32 SRRLen;
        public Boolean SRR;

        public Boolean HasIDE;
        public Int32 IDEIndex;
        public Int32 IDELen;
        public Boolean IDE;

        public Boolean HasRTR;
        public Int32 RTRIndex;
        public Int32 RTRLen;
        public Boolean RTR;

        public Boolean HasR0;
        public Int32 R0Index;
        public Int32 R0Len;
        public Boolean R0;

        public Boolean HasR1;
        public Int32 R1Index;
        public Int32 R1Len;
        public Boolean R1;

        public Boolean HasExtID;
        public Int32 ExtIDIndex;
        public Int32 ExtIDLen;
        public Byte[] ExtID = Array.Empty<Byte>();

        public Boolean HasDLC;
        public Int32 DLCIndex;
        public Int32 DLCLen;
        public Byte DLC;

        public Boolean HasData;
        public DataInfo[] DataInfos = Array.Empty<DataInfo>();

        public Boolean HasCRC;
        public Int32 CRCIndex;
        public Int32 CRCLen;
        public Byte[] CRC = Array.Empty<Byte>();
        public Byte[] SuccessCRC = Array.Empty<Byte>();

        public Boolean HasACK;
        public Int32 ACKIndex;
        public Int32 ACKLen;
        public Boolean ACK;

        public Boolean HasEOF;
        public Int32 EOFIndex;
        public Int32 EOFLen;
        public Byte EOF;

        public Int32 ErrorIndex;
        public Int32 ErrorLen;
        public List<Int32> FramePaddingErrorIndexs = new();
    }
    struct DataInfo
    {
        public Int32 Index;
        public Int32 Len;
        public Byte Data;
    }

    enum FrameType
    {
        StandardDataFrame,
        ExtendedDataFrame,
        StandardRemoteFrame,
        ExtendedRemoteFrame,
        ErrorFrame,
        OverloadFrame,
        FrameInterval,
        PadBitError
    }
}

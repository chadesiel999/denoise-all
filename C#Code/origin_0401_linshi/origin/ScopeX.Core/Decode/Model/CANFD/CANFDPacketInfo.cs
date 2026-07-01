using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    internal struct CANFDPacketInfo
    {
        public Int32 SOFIndex;
        public Int32 SOFLength;
        public Boolean SOF;

        public Boolean HasStandardID;
        public Int32 StandardIDIndex;
        public Int32 StandardIDLength;
        public Byte[] StandardID;
        internal UInt64 TempStandardID;

        public FrameType FrameType;

        public Boolean HasSRR;
        public Int32 SRRIndex;
        public Int32 SRRLength;
        public Boolean SRR;

        public Boolean HasExtID;
        public Int32 ExtIDIndex;
        public Int32 ExtIDLength;
        public Byte[] ExtID;

        public Boolean HasRRS;
        public Int32 RRSIndex;
        public Int32 RRSLength;
        public Boolean RRS;

        public Boolean HasIDE;
        public Int32 IDEIndex;
        public Int32 IDELength;
        public Boolean IDE;

        public Boolean HasFDF;
        public Int32 FDFIndex;
        public Int32 FDFLength;
        public Boolean FDF;

        public Boolean HasRes;
        public Int32 ResIndex;
        public Int32 ResLength;
        public Boolean Res;

        public Boolean HasBRS;
        public Int32 BRSIndex;
        public Int32 BRSLength;
        public Boolean BRS;

        public Boolean HasESI;
        public Int32 ESIIndex;
        public Int32 ESILength;
        public Boolean ESI;

        public Boolean HasDLC;
        public Int32 DLCIndex;
        public Int32 DLCLength;
        public Byte DataByteCount;
        public Byte DLC;

        public Boolean HasData;
        public DataInfo[] DataInfos;

        public Boolean HasStuff;
        public Int32 StuffIndex;
        public Int32 StuffLength;
        public Byte Stuff;

        public Boolean HasStuffParity;
        public Int32 StuffParityIndex;
        public Int32 StuffParityLength;
        public Boolean StuffParity;
        public Boolean SuccessStuffParity;

        public Boolean HasCRC;
        public Int32 CRCIndex;
        public Int32 CRCLength;
        public Byte[] CRC;
        public Byte[] SuccessCRC;
        public Byte CRCBitCount;

        public Boolean HasACK;
        public Int32 ACKIndex;
        public Int32 ACKLength;
        public Boolean ACK;

        public Boolean HasEOF;
        public Int32 EOFIndex;
        public Int32 EOFLength;
        public Byte EOF;
    }
    internal struct DataInfo
    {
        public Int32 Index;
        public Int32 Length;
        public Byte Data;
    }
    public enum FrameType
    {
        StandardDataFrame,
        ExtendedDataFrame,
        StandardRemoteFrame,
        ExtendedRemoteFrame,
        ErrorFrame,
        OverloadFrame,
        FrameInterval,
    }
}

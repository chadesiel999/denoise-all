using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode.Model.FlexRay
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FlexRayEventField
    {
        public Byte Type;
        public Int64 StartIndex;
        public Int64 Length;
        public Int32 DataIndex;
        public Int32 DataLength;
        public Int32 ErrorDataIndex;
        public Int32 ErrorDataLength;
        public Byte Error;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FlexRayEvent
    {
        public Int64 StartIndex;
        public IntPtr Fields;
        public Int32 FieldCount;
        public IntPtr Data;
        public Int32 DataCount;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FlexRayResult
    {
        public IntPtr EventInfosPtr;
        public UInt32 EventCount;
        public SerialProtocolType ProtocolType; //协议类型
    }

    #region FlexRay
    public enum SourceType
    {
        BP,
        BM
    }

    public enum ChannelType
    {
        A,
        B,
    }

    public enum FlexRayDecodeEventType
    {
        NONE,
        INDICATOR,
        FRAMEID,
        PAYLOADLENGTH,
        HEADERCRC,
        CYCLECOUNT,
        DATA,
        FRAMECRC,
        FRAMEEND,
        DTS,
        CID,
    };

    public enum FlexRayDecodeEventErrorType
    {
        NONE,
        RESERVEERROR,
        SYNCFRAMEERROR,
        FRAMEIDERROR,
        PAYLOADLENGTHERROR,
        HEADERCRCERROR,
        CYCLECOUNTERROR,
        MESSAGEIDERROR,
        DATAERROR,
        FRAMECRCERROR,
        FRAMEENDERROR,
        DTSERROR,
        CIDERROR,
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FlexRayOptions
    {
        public IntPtr Cancel_Flag;                             //取消解码标志

        public SourceType SourceType;                          //源类型（极性）

        public ChannelType ChannelType;                        //通道类型

        public UInt32 SignalRate;                              //信号速率
    };

    #endregion
}

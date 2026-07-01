using System;
using System.Runtime.InteropServices;
using ScopeX.ComModel;
using static ScopeX.ComModel.ProtocolCommon;

namespace ScopeX.Core.Decode;

[StructLayout(LayoutKind.Sequential)]
public struct SpiOptions
{
    public IntPtr CancelFlag;//取消解码标志

    public ProtocolSPI.DecodeChannel DecodeChannel;  //解码通道
    public ProtocolSPI.MSB_LSB MsbLsb;               //位顺序
    public Polarity ClkPolarity;                     //时钟极性
    public Polarity CsPolarity;                      // 片选信号极性  
    public Polarity MosiPolarity;                    // 主输出从输入信号极性
    public Polarity MisoPolarity;                    // 主输入从输出信号极性
    public ProtocolSPI.FramingMode CsMode;           // 片选模式
    public UInt32 OutTime;                           // 超时时间
    public UInt32 BitLength;                         // 比特位宽
}

public struct SpiEvent
{
    public UInt64 StartIndex;
    public UInt64 EndIndex;

    public UInt64 DataStartIndex;
    public UInt64 DataEndIndex;

    public UInt32 Data;

}
[StructLayout(LayoutKind.Sequential)]
public struct SpiResultStruct
{
    //public bool DecodeEventNeedUpdate;
    public IntPtr DecodeEvents;
    public UInt32 DecodeEventCount;
    public SerialProtocolType ProtocolType; //协议类型
}
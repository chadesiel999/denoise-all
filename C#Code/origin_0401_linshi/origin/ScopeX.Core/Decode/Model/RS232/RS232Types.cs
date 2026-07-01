// // ******************************************************************
// //       /\ /|       @File         Protocol_Rs232_Options.cs
// //       \ V/        @Brief
// //       | "")       @Author        lijinwen, ghz005@uni-trend.com.cn
// //       /  |        @Creation      2024-1-18
// //      /  \\        @Modified      2024-6-4
// //    *(__\_\
// // ******************************************************************
using System;
using System.Runtime.InteropServices;
using ScopeX.ComModel;
using static ScopeX.ComModel.ProtocolCommon;
using static ScopeX.ComModel.ProtocolRS232;

namespace ScopeX.Core.Decode;

[StructLayout(LayoutKind.Sequential)]
public struct Rs232Options
{
	public IntPtr CancelFlag;//取消解码标志
	public UInt32 BaudRate;
	public OddEvenCheck OddEvenCheck;
	public Polarity Polarity;

	public MSB_LSB MsbLsb;
	public DataBitWidth DataBitWidth;
	public StopBit StopBit;
 
}

[StructLayout(LayoutKind.Explicit, Size = 32)]
public struct Rs232PacketInfoStruct
{
	[FieldOffset(0)] public Byte StartBit;
	[FieldOffset(1)] public Byte ParityBit;
	[FieldOffset(2)] public Byte ParityFind;
	[FieldOffset(3)] public Byte ParityResult;
	[FieldOffset(4)] public Byte Data;
	[FieldOffset(8)] public Int32 DataIndex;
	[FieldOffset(12)] public Int32 ParityIndex;
	[FieldOffset(16)] public Int32 StartIndex;
	[FieldOffset(24)] public Double PerBitLenght;

}

[StructLayout(LayoutKind.Explicit)]
public struct Rs232DataInfoStruct
{
	[FieldOffset(0)]
	public Int32 StartIndex;
	[FieldOffset(4)]
	public Int32 Length;
	[FieldOffset(8)]
	public Byte data;
}
[StructLayout(LayoutKind.Explicit)]
public struct Rs232ResultPacketStruct
{
	[FieldOffset(0)]
	public Int32 ParityResult;
	[FieldOffset(4)]
	public IntPtr DataInfo;
	[FieldOffset(8)]
	public Int32 DataInfoCount;
}

[StructLayout(LayoutKind.Sequential)]
public struct Rs232ResultStruct
{
	public IntPtr EventInfosPtr;
	public UInt32 EventCount;
	public SerialProtocolType ProtocolType; //协议类型
}


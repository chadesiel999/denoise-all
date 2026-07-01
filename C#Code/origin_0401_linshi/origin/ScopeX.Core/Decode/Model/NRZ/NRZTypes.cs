// // ******************************************************************
// //       /\ /|       @File         Protocol_NRZ_Options.cs
// //       \ V/        @Brief
// //       | "")       @Author        lijinwen, ghz005@uni-trend.com.cn
// //       /  |        @Creation      2024-8-1
// //      /  \\        @Modified      2024-8-1
// //    *(__\_\
// // ******************************************************************
using System;
using System.Runtime.InteropServices;
using ScopeX.ComModel;
using static ScopeX.ComModel.ProtocolCommon;
 
namespace ScopeX.Core.Decode;

[StructLayout(LayoutKind.Sequential)]
public struct NRZOptions
{
	public IntPtr CancelFlag;//取消解码标志

	public UInt32 BaudRate;

    public Polarity Polarity;

    public ProtocolNRZ.MSB_LSB MsbLsb;

}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct NRZResultStruct
{
	public IntPtr EventInfosPtr;
	public UInt32 EventCount;
	public SerialProtocolType ProtocolType; //协议类型
	public Double PerBitLength;
}

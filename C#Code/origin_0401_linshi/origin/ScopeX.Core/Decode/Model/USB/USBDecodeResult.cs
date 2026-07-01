// // ******************************************************************
// //       /\ /|       @File         struct UsbDecodeResult.cs
// //       \ V/        @Brief
// //       | "")       @Author        lijinwen, ghz005@uni-trend.com.cn
// //       /  |        @Creation      2024-1-22
// //      /  \\        @Modified      2024-5-6
// //    *(__\_\
// // ******************************************************************

//using System.Drawing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ScopeX.ComModel;

namespace ScopeX.Core.Decode;

public class UsbDecodeResultCell
{
//    public readonly List<Byte> Datas;

//    public Boolean IsEventInfo;
//    public Int64 Length;
//    public Int64 StartIndex;
//    public UsbDecodeResultCell()
//    {
//        StartIndex = -1;
//        Length = 0;
//        Datas = new List<Byte>();
//    }
//    internal UsbDecodeResultCell(UsbResultCellStruct decodeResult)
//    {
//        StartIndex = decodeResult.StartIndex;
//        Length = decodeResult.Length;
//        Datas = new List<Byte>();
//        IsEventInfo = decodeResult.IsEventInfo;
//        //232 只有一个字节数据
//        if (Length > 0)
//        {
//            IntPtr ptr = decodeResult.Datas;
//            Datas.Add((Byte)(Marshal.PtrToStructure(ptr, typeof(Byte)) ?? throw new InvalidOperationException()));

//        }
//    }
//}
//[StructLayout(LayoutKind.Explicit)]
//public struct UsbDecodeEventStruct
//{
//    [FieldOffset(0)] public Int64 EventIndex; //事件序号
//    [FieldOffset(8)] public Int64 StartIndex; //事件时域起始帧序号
//    [FieldOffset(16)] public Int64 EndIndex; //事件时域结束帧序号
//    //public String Name; //String 事件名称
//    //////////////////////////////////////////
//    [FieldOffset(24)] public USBEnums.EventInfoTitles EventTitle;
//    [FieldOffset(28)] public Int32 Address;
//    [FieldOffset(32)] public Int32 FrameId;
//    [FieldOffset(36)] public Int16 Port;
//    [FieldOffset(38)] public Int16 EndPoint;
//    [FieldOffset(40)] public Int16 CrcSignNum;
//    [FieldOffset(42)] public UInt16 CrcData;
//    [FieldOffset(48)] public UInt64 DataCount;
//    [FieldOffset(56)] public IntPtr DecodeDataPtr;
//    [FieldOffset(64)] public UInt64 SingleBitTimingLength;
//}
//public class UsbDecodeResult
//{
//    #region 属性

//    public Boolean ResultValid;
//    public Boolean DecodeResultNeedUpdate;
//    public Boolean DecodeEventNeedUpdate;
//    public List<UsbDecodeResultCell> DecodeResultUnits;
//    public List<UsbDecodeEventStruct> DecodeEvents;
//    public ProtocolUSB.SignalRate USBSpeed;
//    public UsbDecodeResult()
//    {
//        DecodeResultUnits = new List<UsbDecodeResultCell>();
//        DecodeEvents = new List<UsbDecodeEventStruct>();
//    }
//    public Int32 DecodeResultCount => DecodeResultUnits.Count;
//    public Int32 DecodeEventCount => DecodeEvents.Count;

//    #endregion 属性

//    #region 方法

//    public SerialProtocolType GetProtocolType()
//    {
//        return SerialProtocolType.USB;
//    }
//    public static Boolean ConvertData(UsbResultStruct dataStruct, out UsbDecodeResult dataResult)
//    {

//        dataResult = new UsbDecodeResult();

//        if (dataStruct.DecodeEventCount > 0)
//        {
//            IntPtr ptr = dataStruct.DecodeEvents;
//            //events = new UsbDecodeEventStruct[dataStruct.DecodeEventCount];
//            // 计算结构体的字节数
//            Int32 structSize = Marshal.SizeOf(typeof(UsbDecodeEventStruct));
//            for (UInt64 i = 0; i < dataStruct.DecodeEventCount; i++)
//            {
//                IntPtr structPtr = new(ptr.ToInt64() + ((Int64)i * structSize));
//                UsbDecodeEventStruct decodeEventStruct = (UsbDecodeEventStruct)(Marshal.PtrToStructure(structPtr, typeof(UsbDecodeEventStruct)) ?? throw new InvalidOperationException());
//                //UsbDecodeEvent decodeEvent = new(decodeEventStruct);
//                dataResult.DecodeEvents.Add(decodeEventStruct);
//            }

//        }

//        if (dataStruct.DecodeResultCount > 0)
//        {
//            IntPtr ptr = dataStruct.DecodeResultCells;
//            Console.WriteLine($@"DecodeResultUnits Ptr:0x{ptr:X}");
//            //results = new UsbDecodeResultCellStruct[dataStruct.DecodeResultCount];
//            // 计算结构体的字节数
//            Int32 structSize = Marshal.SizeOf(typeof(UsbResultCellStruct));
//            for (UInt64 i = 0; i < dataStruct.DecodeResultCount; i++)
//            {
//                IntPtr structPtr = new(ptr.ToInt64() + ((Int64)i * structSize));
//                UsbResultCellStruct UsbDecodeResultCellStruct = (UsbResultCellStruct)(Marshal.PtrToStructure(structPtr, typeof(UsbResultCellStruct)) ?? throw new InvalidOperationException());
//                UsbDecodeResultCell UsbDecodeResultCell = new(UsbDecodeResultCellStruct);
//                dataResult.DecodeResultUnits.Add(UsbDecodeResultCell);
//            }
//            //test out 
//            Console.WriteLine(@"    First Data: ");
//            Console.WriteLine($@"        StartIndex:{dataResult.DecodeResultUnits[0].StartIndex} ");
//        }
//        dataResult.USBSpeed = dataStruct.USBSpeed;
//        return true;
//    }

//    #endregion 方法
}

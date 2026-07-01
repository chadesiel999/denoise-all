// // ******************************************************************
// //       /\ /|       @File         struct RS232DecodeResult.cs
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
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using ScopeX.ComModel;
using static ScopeX.Core.Decode.RS232DecodeModel;

namespace ScopeX.Core.Decode;
//public struct PacketInfo
//{
//    public Int32 StartIndex;
//    public Int32 StartLen;
//    public Boolean HasStart;
//    public DataPacketInfo[] Datas;
//    public Boolean HasEnd;
//    public Int32 EndIndex;
//    public Int32 EndLen;
//}
//public struct DataPacketInfo
//{
//    public Int32 Index;
//    public Int32 Len;
//    public Boolean IsLast;
//    public Byte[] MISOData;
//    public Byte[] MOSIData;
//    public Int32 BitCount;
//    public Int32 RealBitCount;
//}

public class Rs232Result
{
    #region 属性

    public Boolean ResultValid;
    public Boolean DecodeResultNeedUpdate;
    public Boolean DecodeEventNeedUpdate;

    internal List<Rs232PacketInfo> DecodeEvents;

    public Rs232Result()
    {

        DecodeEvents = new List<Rs232PacketInfo>();
    }

    public Int32 DecodeEventCount => DecodeEvents.Count;

    #endregion 属性

    #region 方法

    public SerialProtocolType GetProtocolType()
    {
        return SerialProtocolType.RS232;
    }
    internal static Boolean ConvertData(Rs232ResultStruct dataStruct
        , out List<Rs232PacketInfo> rs232Infos)
    {
        rs232Infos = new();
        UInt64 dataInfoCount = dataStruct.EventCount;
        if (dataInfoCount <= 0)
        {
            return false;
        }
        IntPtr eventPtr = dataStruct.EventInfosPtr;
        for (UInt64 i = 0; i < dataInfoCount; i++)
        {
            Int32 structSize = Marshal.SizeOf(typeof(Rs232PacketInfoStruct));
            IntPtr structPtr = new(eventPtr.ToInt64() + ((Int64)i * structSize));
            Rs232PacketInfoStruct decodeEventStruct = (Rs232PacketInfoStruct)(Marshal.PtrToStructure(structPtr,
                typeof(Rs232PacketInfoStruct)) ?? throw new InvalidOperationException());

            rs232Infos.Add(new(decodeEventStruct));
        }

        return true;
    }
    //public static Boolean ConvertData(Rs232ResultStruct dataStruct, out RS232DecodeResult dataResult)
    //{
    //    dataResult = new();
    //    UInt64 dataInfoCount = dataStruct.DecodeEventCount;
    //    if (dataInfoCount <= 0)
    //    {
    //        return false;
    //    }
    //    IntPtr eventPtr = dataStruct.DecodeEvents;
    //    for (UInt64 i = 0; i < dataInfoCount; i++)
    //    {
    //        Int32 structSize = Marshal.SizeOf(typeof(RS232DecodeResultPacketStruct));
    //        IntPtr structPtr = new(eventPtr.ToInt64() + ((Int64)i * structSize));
    //        RS232DecodeResultPacketStruct decodeEventStruct = (RS232DecodeResultPacketStruct)(Marshal.PtrToStructure(structPtr,
    //            typeof(RS232DecodeResultPacketStruct)) ?? throw new InvalidOperationException());

    //        RS232PacketInfo rS232PacketInfo = new()
    //        {
    //            ParityResult = decodeEventStruct.ParityResult == 1,
    //        };
    //        IntPtr structEventCellPtr = decodeEventStruct.DataInfo;
    //        if (decodeEventStruct.DataInfoCount <= 0
    //            || structEventCellPtr == IntPtr.Zero
    //             )
    //        {
    //            break;
    //        }
    //        RS232DataInfoStruct decodeDataInfoStruct = (RS232DataInfoStruct)(Marshal.PtrToStructure(structEventCellPtr,
    //           typeof(RS232DataInfoStruct)) ?? throw new InvalidOperationException());

    //        rS232PacketInfo.StartIndex = decodeDataInfoStruct.StartIndex;
    //        rS232PacketInfo.PerBitLenght = decodeDataInfoStruct.Length / 8;
    //        rS232PacketInfo.Data = decodeDataInfoStruct.data;
    //    }


    //    return dataStruct.DecodeEventNeedUpdate;
    //}
    //public static Boolean ConvertData(Rs232ResultStruct dataStruct, out RS232DecodeResult dataResult)
    //{

    //    dataResult = new RS232DecodeResult();

    //    if (dataStruct.DecodeEventCount > 0)
    //    {
    //        IntPtr ptr = dataStruct.DecodeEvents;
    //        //events = new RS232DecodeEventStruct[dataStruct.DecodeEventCount];
    //        // 计算结构体的字节数

    //        for (UInt64 i = 0; i < dataStruct.DecodeEventCount; i++)
    //        {
    //            Int32 structSize = Marshal.SizeOf(typeof(RS232DecodeResultPacketStruct));
    //            IntPtr structPtr = new(ptr.ToInt64() + ((Int64)i * structSize));
    //            RS232DecodeResultPacketStruct decodeEventStruct = (RS232DecodeResultPacketStruct)(Marshal.PtrToStructure(structPtr,
    //                typeof(RS232DecodeResultPacketStruct)) ?? throw new InvalidOperationException());
    //            //RS232DecodeEvent decodeEvent = new(decodeEventStruct);

    //            PacketInfo packetInfo = new PacketInfo()
    //            {
    //                StartIndex = decodeEventStruct.StartIndex,
    //                StartLen = decodeEventStruct.StartLen,
    //                HasStart = decodeEventStruct.HasStart == 1,
    //                HasEnd = decodeEventStruct.HasEnd == 1,
    //                EndIndex = decodeEventStruct.EndIndex,
    //                EndLen = decodeEventStruct.EndLen
    //            };
    //            List<DataPacketInfo> packets = new List<DataPacketInfo>();
    //            try
    //            {
    //                if (decodeEventStruct.DataInfoCount > 0)
    //                {
    //                    //if (decodeEventStruct.DataInfoPtr == IntPtr.Zero)
    //                    //{
    //                    //    break;
    //                    //}
    //                    //var RS232DataInfoStruct = Marshal.PtrToStructure<RS232DataInfoStruct>(decodeEventStruct.DataInfoPtr + x);
    //                    // 计算结构体的字节数
    //                    //structSize = Marshal.SizeOf(typeof(RS232DataInfoStruct));
    //                    //structPtr = new((Int64)decodeEventStruct.DataInfoPtr + ((Int64)x * structSize));
    //                    // RS232DataInfoStruct RS232DataInfoStruct = (RS232DataInfoStruct)(Marshal.PtrToStructure(structPtr,
    //                    //typeof(RS232DataInfoStruct)) ?? throw new Exception());
    //                    RS232DataInfoStruct RS232DataInfoStruct = decodeEventStruct.DataInfoStruct;
    //                    DataPacketInfo dataPacketInfo = new DataPacketInfo()
    //                    {
    //                        Index = RS232DataInfoStruct.Index,
    //                        Len = RS232DataInfoStruct.Length,
    //                        IsLast = RS232DataInfoStruct.IsLast == 1,
    //                        BitCount = RS232DataInfoStruct.BitCount,
    //                        RealBitCount = RS232DataInfoStruct.RealBitCount,
    //                    };
    //                    //data
    //                    List<Byte> mosiDatas = new();
    //                    //for (Int32 mosiId = 0; mosiId < RS232DataInfoStruct.MosiDataCount; mosiId++)
    //                    if (RS232DataInfoStruct.MosiDataCount > 0)

    //                    {
    //                        //mosiDatas.Add(Marshal.ReadByte(RS232DataInfoStruct.MosiDataPtr + mosiId));
    //                        mosiDatas.Add(RS232DataInfoStruct.MosiData);
    //                    }
    //                    List<Byte> misoDatas = new();
    //                    //for (Int32 misoId = 0; misoId < RS232DataInfoStruct.MisoDataCount; misoId++)
    //                    if (RS232DataInfoStruct.MisoDataCount > 0)
    //                    {
    //                        // misoDatas.Add(Marshal.ReadByte(RS232DataInfoStruct.MisoDataPtr + misoId));
    //                        misoDatas.Add(RS232DataInfoStruct.MisoData);
    //                    }
    //                    dataPacketInfo.MISOData = misoDatas.ToArray();
    //                    dataPacketInfo.MOSIData = mosiDatas.ToArray();
    //                    packets.Add(dataPacketInfo);
    //                }
    //            }
    //            catch (Exception ex)
    //            {
    //                return false;
    //            }

    //            packetInfo.Datas = packets.ToArray();

    //            dataResult.DecodeEvents.Add(packetInfo);
    //        }

    //    }

    //    return true;
    //}

    #endregion 方法
}

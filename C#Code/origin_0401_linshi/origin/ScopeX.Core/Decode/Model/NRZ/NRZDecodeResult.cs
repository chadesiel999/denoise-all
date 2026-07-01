// // ******************************************************************
// //       /\ /|       @File         struct NRZResult.cs
// //       \ V/        @Brief
// //       | "")       @Author        lijinwen, ghz005@uni-trend.com.cn
// //       /  |        @Creation      2024-8-1
// //      /  \\        @Modified      2024-9-10
// //    *(__\_\
// // ******************************************************************

//using System.Drawing;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using ScopeX.ComModel;
using static ScopeX.Core.Decode.NRZDecodeModel;

namespace ScopeX.Core.Decode;

public class NRZResult
{
    #region 属性

    public Boolean DecodeResultNeedUpdate;

    internal List<NRZPacketInfo> DecodeEvents;

    public NRZResult()
    {

        DecodeEvents = new List<NRZPacketInfo>();
    }

    public Int32 DecodeEventCount => DecodeEvents.Count;

    #endregion 属性

    #region 方法

    public SerialProtocolType GetProtocolType()
    {
        return SerialProtocolType.NRZ;
    }
    internal static Boolean ConvertData(NRZResultStruct dataStruct
        , out List<NRZPacketInfo> NRZInfos)
    {
        NRZInfos = new();
        UInt64 dataCount = dataStruct.EventCount;
        if (dataCount <= 0)
        {
            return false;
        }
        IntPtr eventPtr = dataStruct.EventInfosPtr;
        for (UInt64 i = 0; i < dataCount; i++)
        {
            Int32 structSize = Marshal.SizeOf(typeof(Byte));
            IntPtr structPtr = new(eventPtr.ToInt64() + ((Int64)i * structSize));
            Byte data = (Byte)(Marshal.PtrToStructure(structPtr,
                typeof(Byte)) ?? throw new InvalidOperationException());

            NRZInfos.Add(new()
            {
                Data = data,
                PerBitLength = dataStruct.PerBitLength,
                StartIndex = (Int32)(i * 8 * dataStruct.PerBitLength),
            }
            );
        }

        return true;
    }
   
    #endregion 方法
}

using NPOI.POIFS.Crypt.Dsig;
using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ScopeX.Core.Decode.DecoderTypes;
using static ScopeX.Core.Decode.EdgeInfoCPP;

namespace ScopeX.Core.Decode
{
    public class DecoderTypes
    {
        public enum DiffSignalType
        {
            Single,
            Difference
        }
        
        public enum PAM2StatusType
        {
            Low,
            High,
            None
        };
        public enum PAM3StatusType
        {
            High = 0b11,
            Middle = 0b01,
            Low = 0b00,
            None = 0b10
        };

        public enum PAMType
        {
            PAM2 = 0,
            PAM3,
            PAM4,
            NONE
        };


        // 原始采样数据信息
        [StructLayout(LayoutKind.Sequential)]
        public struct WaveformInfoCPP
        {
            public Double SampleRate; // 采样速率
            public PAMType PamType; // 电平类型
            public UInt64 DataCount; // 信号原始采样数据个数
            public IntPtr DataPtr; // 信号原始采样数据

            public WaveformInfoCPP(Double sampleRate, PAMType pamType, UInt64 dataCount, IntPtr datas)
            {
                SampleRate = sampleRate;
                PamType = pamType;
                DataCount = dataCount;
                DataPtr = datas;
            }

            public void Allocate(ref Double[]datas, out GCHandle datasHandle)
            {
                datasHandle = GCHandle.Alloc(datas, GCHandleType.Pinned);
                DataPtr = Marshal.UnsafeAddrOfPinnedArrayElement(datas, 0);
            }

            public void Free(ref GCHandle datasHandle)
            {
                datasHandle.Free();
            }
        };

        // 阈值信息
        [StructLayout(LayoutKind.Sequential)]
        public struct ThresholdInfoCPP
        {
            public Byte ThresholdCount; // 阈值个数
            public IntPtr ThresholdPtr; // 阈值

            public ThresholdInfoCPP(Byte thresholdCount, IntPtr threshold)
            {
                ThresholdCount = thresholdCount;
                ThresholdPtr = threshold;
            }

            public void Allocate(ref Double[] thresholds, out GCHandle thresholsHandle)
            {
                thresholsHandle = GCHandle.Alloc(thresholds, GCHandleType.Pinned);
                ThresholdPtr = Marshal.UnsafeAddrOfPinnedArrayElement(thresholds, 0);
            }

            public void Free(ref GCHandle thresholsHandle)
            {
                thresholsHandle.Free();
            }
        };

        // 事件字段定义
        //public class EventDataSizeType
        //{
        //    public const Int32 EventDataSizeOne = 1;
        //    public const Int32 EventDataSizeTwo = 1;
        //    public const Int32 EventDataSizeThree = 1;
        //    public const Int32 EventDataSizeFour = 1;
        //    public const Int32 EventDataSizeFive = 1;

        //    public
        //};

        //public interface ISize
        //{
        //    public abstract Int32 DataSize { get; }
        //}
        //public interface Size1 : ISize
        //{
        //    public static Int32 DataSize => 1;
        //}

        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public unsafe struct EventFieldInfo<TSize> where TSize : struct, ISize
        //{
        //    public Int64 StartIndex; // 事件字段起始索引
        //    public Int64 Len;         // 事件字段长度
        //    public Byte HasData;            // 该字段是否有效
        //    public Byte event_error_type;  // 字段错误类型，0-无错误
        //    private fixed Byte data[TSize.DataSize];       // 事件字段数据
        //}

    }

}

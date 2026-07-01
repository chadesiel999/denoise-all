using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

/// <summary>
/// 目前该参数用于一片ADC一个两个核之前的同步校准。也就是解决压稳态的问题。
/// 从目前来看，其只与ADC的相位调整和时钟有关，与模拟通道的信号无关。故不应该与模拟通道号相联系
/// 特别是DBI项目，物理通道的概念与子带的概念是两个不同的概念。
/// 在DBI项目中，按子带来区分。
/// =============================
/// 在实际使用中，非DBI项目按通道的顺序来存放。因为都是20Gsps的（10Gsps的也是正确的）
/// DBI项目按子带编号的顺序来存放的
/// </summary>
namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class TiAdc_SyncSampleClock : ICaliData
    {
        public static TiAdc_SyncSampleClock Default = new TiAdc_SyncSampleClock();
        public TiAdc_SyncSampleClock()
        {
            for (int acqBoardIndex = 0; acqBoardIndex < CaliConstants.Fixed_AcqBoardMaxCount; acqBoardIndex++)
            {
                data[acqBoardIndex] = new AcqSyncItem[CaliConstants.Fixed_PerChannelMergeAdcMaxCount];
                for (int adcIndx = 0; adcIndx < CaliConstants.Fixed_PerChannelMergeAdcMaxCount; adcIndx++)
                    data[acqBoardIndex][adcIndx] = new AcqSyncItem();
            }
        }
        private AcqSyncItem[][] data = new AcqSyncItem[CaliConstants.Fixed_AcqBoardMaxCount][];
        public Int32 TotalBytes
        {
            get
            {
                int perItemDataBytes = Marshal.SizeOf(data[0][0]);
                int totalBytes = perItemDataBytes * CaliConstants.Fixed_AcqBoardMaxCount * CaliConstants.Fixed_PerChannelMergeAdcMaxCount;
                return totalBytes;
            }
        }
        public Int32 OriginTotleBytes
        {
            get => TotalBytes;
            set { }
        }
        public CaliDataType DataType { get=> CaliDataType.TiAdc_SyncSampleClock; }
        /// <summary>
        /// 以ps为单位的正整数,必须是25ps的整数倍
        /// </summary>
        /// <param name="acqBoardIndex"></param>
        /// <returns></returns>
        public AcqSyncItem[] this[int acqBoardIndex]
        {
            get => data[acqBoardIndex];
            set
            {
                data[acqBoardIndex] = value;
            }
        }

        public AcqSyncItem this[int channelIndex,int adcIndex]
        {
            get => data[channelIndex][adcIndex];
            set
            {
                data[channelIndex][adcIndex] = value;
            }
        }
        public byte[] Serialize()
        {
            System.IO.MemoryStream memoryStream = new MemoryStream();

            for (int acqBoardIndex = 0; acqBoardIndex < CaliConstants.Fixed_AcqBoardMaxCount; acqBoardIndex++)
            {
                for (int adcIndex = 0; adcIndex < CaliConstants.Fixed_PerChannelMergeAdcMaxCount; adcIndex++)
                {
                    memoryStream.Write(Helper.StructToBytes(data[acqBoardIndex][adcIndex]));
                }
            }
            byte[] result = memoryStream.ToArray();
            memoryStream.Close();

            return result;
        }
        public void Deserialize(byte[] content)
        {
            if (content.Length < CaliConstants.Fixed_PerChannelMergeAdcMaxCount * sizeof(Int32))
                return;
            int perItemBytes = Marshal.SizeOf(data[0][0]);
            for (int acqBoardIndex = 0; acqBoardIndex < CaliConstants.Fixed_AcqBoardMaxCount; acqBoardIndex++)
            {
                for (int adcIndex = 0; adcIndex < CaliConstants.Fixed_PerChannelMergeAdcMaxCount; adcIndex++)
                {
                    int startIndex = (acqBoardIndex * CaliConstants.Fixed_PerChannelMergeAdcMaxCount + adcIndex) * perItemBytes;
                    if ((startIndex + perItemBytes) < content.Length)
                        data[acqBoardIndex][adcIndex] = Helper.BytesToStruct<AcqSyncItem>(content, startIndex, typeof(AcqSyncItem));
                    else
                        data[acqBoardIndex][adcIndex] = new AcqSyncItem();
                }
            }
        }

        public void LoadDefaultValue()
        {
        }
    }
    

}

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class DbiAnalogParams : ICaliData
    {
        public static DbiAnalogParams Default = new DbiAnalogParams();
        private DbiAnalogParams()
        {
            data = new DbiAnalogChannelSubbandItem[2, 4, CaliConstants.Fixed_MaxPhyCoarseScaleCount, 4];
            for (int mode = 0; mode < 2; mode++)
            {
                for (int channelID = 0; channelID < 4; channelID++)//最多4个通道
                {
                    for (int scaleIndex = 0; scaleIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleIndex++)
                    {
                        for (int subbandIndex = 0; subbandIndex < 4; subbandIndex++)//最多4个子带
                        {
                            data[mode, channelID, scaleIndex, subbandIndex] = new DbiAnalogChannelSubbandItem();
                            data[mode, channelID, scaleIndex, subbandIndex].Gain_FineByFpgaThousand = 1000;//千分之
                            data[mode, channelID, scaleIndex, subbandIndex].Gain_FineByAdc1ByTenThousand = 10000;//万分之
                            data[mode, channelID, scaleIndex, subbandIndex].Gain_FineByAdc2ByTenThousand = 10000;//万分之
                        }
                    }
                }
            }
        }
        private DbiAnalogChannelSubbandItem[/*独占模式=0,1=共享模式*/,/*channel*/,/*yScale*/,/*data*/] data;
        public CaliDataType DataType => CaliDataType.DbiAnalogParams;

        public int TotalBytes => 2 * 4 * CaliConstants.Fixed_MaxPhyCoarseScaleCount * 4 * Marshal.SizeOf(data[0, 0, 0, 0]);
        public Int32 OriginTotleBytes
        {
            get => TotalBytes;
            set { }
        }
        public DbiAnalogChannelSubbandItem this[int bandMode, int channelIndex, int scaleIndex, int subbandIndex]
        {
            get => data[bandMode, channelIndex, scaleIndex, subbandIndex];
            set => data[bandMode, channelIndex, scaleIndex, subbandIndex] = value;
        }
        private void CheckValid()
        {
            for (int mode = 0; mode < 2; mode++)
            {
                for (int channelID = 0; channelID < 4; channelID++)//最多4个通道
                {
                    for (int scaleIndex = 0; scaleIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleIndex++)
                    {
                        for (int subbandIndex = 0; subbandIndex < 4; subbandIndex++)//最多4个子带
                        {
                            if (data[mode, channelID, scaleIndex, subbandIndex].Gain_FineByFpgaThousand == 0)
                                data[mode, channelID, scaleIndex, subbandIndex].Gain_FineByFpgaThousand = 1000;//千分之
                            if (data[mode, channelID, scaleIndex, subbandIndex].Gain_FineByAdc1ByTenThousand == 0)
                                data[mode, channelID, scaleIndex, subbandIndex].Gain_FineByAdc1ByTenThousand = 10000;//万分之
                            if (data[mode, channelID, scaleIndex, subbandIndex].Gain_FineByAdc2ByTenThousand == 0)
                                data[mode, channelID, scaleIndex, subbandIndex].Gain_FineByAdc2ByTenThousand = 10000;//万分之

                            if (data[mode, channelID, scaleIndex, subbandIndex].OffsetPosterior == 0)
                                data[mode, channelID, scaleIndex, subbandIndex].OffsetPosterior = 32000;

                            if (data[mode, channelID, scaleIndex, subbandIndex].BiasPreceding == 0)
                                data[mode, channelID, scaleIndex, subbandIndex].BiasPreceding = 32000;
                        }
                    }
                }
            }
        }
        public void Deserialize(byte[] content)
        {
            if (content.Length < TotalBytes)
                return;
            int perModeBytes = 4/*4个通道*/ * CaliConstants.Fixed_MaxPhyCoarseScaleCount * 4/*每通道最多4个子带*/ * Marshal.SizeOf(data[0, 0, 0, 0]);
            int perChannelBytes = CaliConstants.Fixed_MaxPhyCoarseScaleCount * 4 /*每通道最多4个子带*/ * Marshal.SizeOf(data[0, 0, 0, 0]);
            int perScaleBytes = 4 /*每通道最多4个子带*/ * Marshal.SizeOf(data[0, 0, 0, 0]);
            int perSubbandBytes = Marshal.SizeOf(data[0, 0, 0, 0]);
            for (int mode = 0; mode < 2; mode++)
            {
                for (int channelID = 0; channelID < 4; channelID++)
                {
                    for (int scaleIndex = 0; scaleIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleIndex++)
                    {
                        for (int subbandIndex = 0; subbandIndex < 4; subbandIndex++)
                        {
                            data[mode, channelID, scaleIndex, subbandIndex] = Helper.BytesToStruct<DbiAnalogChannelSubbandItem>(content, mode * perModeBytes + channelID * perChannelBytes + perScaleBytes * scaleIndex + subbandIndex * perSubbandBytes, typeof(DbiAnalogChannelSubbandItem));
                        }
                    }
                }
            }
            #region 1mV_2mV Special:eq 5mV
            for (int mode = 0; mode < 2; mode++)
            {
                for (int channelIndex = 0; channelIndex < 4; channelIndex++)
                {
                    for (int subbandIndex = 0; subbandIndex < 4; subbandIndex++)
                    {
                        data[mode, channelIndex, (int)AnaChnlScaleIndex.Lv1m, subbandIndex] = data[mode, channelIndex, (int)AnaChnlScaleIndex.Lv5m, subbandIndex];
                        data[mode, channelIndex, (int)AnaChnlScaleIndex.Lv2m, subbandIndex] = data[mode, channelIndex, (int)AnaChnlScaleIndex.Lv5m, subbandIndex];
                    }
                }
            }
            #endregion
            CheckValid();
        }

        public byte[] Serialize()
        {
            #region 1mV_2mV Special:eq 5mV
            for (int mode = 0; mode < 2; mode++)
            {
                for (int channelIndex = 0; channelIndex < 4; channelIndex++)
                {
                    for (int subbandIndex = 0; subbandIndex < 4; subbandIndex++)
                    {
                        data[mode, channelIndex, (int)AnaChnlScaleIndex.Lv1m, subbandIndex] = data[mode, channelIndex, (int)AnaChnlScaleIndex.Lv5m, subbandIndex];
                        data[mode, channelIndex, (int)AnaChnlScaleIndex.Lv2m, subbandIndex] = data[mode, channelIndex, (int)AnaChnlScaleIndex.Lv5m, subbandIndex];
                    }
                }
            }
            #endregion

            System.IO.MemoryStream memoryStream = new MemoryStream();
            for (int mode = 0; mode < 2; mode++)
            {
                for (int channelIndex = 0; channelIndex < 4; channelIndex++)
                {
                    for (int scaleIndex = 0; scaleIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleIndex++)
                    {
                        for (int subbandIndex = 0; subbandIndex < 4; subbandIndex++)
                        {
                            memoryStream.Write(Helper.StructToBytes(data[mode, channelIndex, scaleIndex, subbandIndex]));
                        }
                    }
                }
            }
            byte[] result = memoryStream.ToArray();
            memoryStream.Close();
            return result;
        }
    }
}

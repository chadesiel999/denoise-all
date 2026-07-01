using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class ChannelParams : ICaliData
    {
        enum ReservedInt32Index
        {
            TemperatureAtCaliBaseline_mCelsius=0,
            TemperatureAtCaliGain_mCelsius = 1,
        }
        public static ChannelParams Default = new ChannelParams();
        private ChannelPerScaleItem[/*channelIndex*/ , /*impedanceIndex,50_Om=1 */ , /*yScaleIndex*/] perScaleData;
        private Int32 newAppendUInt32Count = 12;
        public Int32 TotalBytes
        {
            get
            {
                int perScaleDataBytes = Marshal.SizeOf(perScaleData[0, 0, 0]);
                int allPerScaleDataBytes = 2 * CaliConstants.Fixed_MaxPhysicsChannelCount * CaliConstants.Fixed_MaxPhyCoarseScaleCount * perScaleDataBytes;//2=高阻、低阻
                int totalBytes = allPerScaleDataBytes + sizeof(UInt32) + sizeof(UInt32); //+ Trig_ACZero + Trig_ACZero3Div

                totalBytes+=reservedInt32Count * sizeof(UInt32);

                return totalBytes;
            }
        }
        public Int32 OriginTotleBytes
        {
            get => TotalBytes;
            set { }
        }
        public CaliDataType DataType { get => CaliDataType.PhyChannel; }

        public UInt32 Trig_ACZero
        {
            get;
            set;
        }
        public UInt32 Trig_ACZero3Div
        {
            get;
            set;
        }
        /// <summary>
        /// 校准时的温度，以毫摄氏度为单位，也就是摄氏度*1000
        /// </summary>
        public Int32 TemperatureAtCaliBaseline_mCelsius
        {
            get => (Int32)reserved[(int)ReservedInt32Index.TemperatureAtCaliBaseline_mCelsius];
            set=> reserved[(int)ReservedInt32Index.TemperatureAtCaliBaseline_mCelsius] =(UInt32)value;
        }
        public Int32 TemperatureAtCaliGain_mCelsius
        {
            get => (Int32)reserved[(int)ReservedInt32Index.TemperatureAtCaliGain_mCelsius];
            set => reserved[(int)ReservedInt32Index.TemperatureAtCaliGain_mCelsius] = (UInt32)value;
        }
        private const Int32 reservedInt32Count = 64;
        private UInt32[] reserved=new UInt32[reservedInt32Count];

        public ChannelPerScaleItem this[int channelIndex, int impedanceIndex, int yScaleIndex]
        {
            get => perScaleData[channelIndex, impedanceIndex, yScaleIndex];
            set => perScaleData[channelIndex, impedanceIndex, yScaleIndex] = value;
        }
        private ChannelParams()
        {
            perScaleData = new ChannelPerScaleItem[CaliConstants.Fixed_MaxPhysicsChannelCount, 2, CaliConstants.Fixed_MaxPhyCoarseScaleCount];
            for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                for (int impedanceIndex = 0; impedanceIndex < 2; impedanceIndex++)
                {
                    for (int scaleIndex = 0; scaleIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleIndex++)
                    {
                        perScaleData[channelIndex, impedanceIndex, scaleIndex] = new ChannelPerScaleItem();
                    }
                }
            }
            CheckValid();
        }

        public byte[] Serialize()
        {
            System.IO.MemoryStream memoryStream = new MemoryStream();
            for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                for (int impedanceIndex = 0; impedanceIndex < 2; impedanceIndex++)
                {
                    for (int scaleIndex = 0; scaleIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleIndex++)
                    {
                        memoryStream.Write(Helper.StructToBytes(perScaleData[channelIndex, impedanceIndex, scaleIndex]));
                    }
                }
            }
            memoryStream.Write(BitConverter.GetBytes(Trig_ACZero));
            memoryStream.Write(BitConverter.GetBytes(Trig_ACZero3Div));

            for (int i = 0; i < reservedInt32Count; i++)
                memoryStream.Write(BitConverter.GetBytes(reserved[i]));

            byte[] result = memoryStream.ToArray();
            memoryStream.Close();

            return result;
        }
        private void CheckValid()
        {
            for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                for (int impedanceIndex = 0; impedanceIndex < 2; impedanceIndex++)
                {
                    for (int scaleIndex = 0; scaleIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleIndex++)
                    {
                        //Gain_FineByAdc 在 PXI 项目中有其他约定，故不能限制
                        //if (perScaleData[channelIndex, impedanceIndex, scaleIndex].Gain_FineByAdc==0)
                        //    perScaleData[channelIndex, impedanceIndex, scaleIndex].Gain_FineByAdc = 10000;
                        if (perScaleData[channelIndex, impedanceIndex, scaleIndex].Gain_FineByFpgaThousand == 0)
                            perScaleData[channelIndex, impedanceIndex, scaleIndex].Gain_FineByFpgaThousand = 1000;
                    }
                }
            }
            if (TemperatureAtCaliBaseline_mCelsius == 0)
                TemperatureAtCaliBaseline_mCelsius = 40 * 1000;//默认为40摄氏度
            if (TemperatureAtCaliGain_mCelsius == 0)
                TemperatureAtCaliGain_mCelsius = 40 * 1000;//默认为40摄氏度
        }
        public void Deserialize(byte[] content)
        {
            int perScaleDataBytes = Marshal.SizeOf(perScaleData[0, 0, 0]);

            if (content.Length < (TotalBytes- reservedInt32Count* sizeof(UInt32)))
                return;
            int bufferIndex = 0;
            for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                for (int impedanceIndex = 0; impedanceIndex < 2; impedanceIndex++)
                {
                    for (int scaleIndex = 0; scaleIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleIndex++)
                    {
                        perScaleData[channelIndex, impedanceIndex, scaleIndex] = Helper.BytesToStruct<ChannelPerScaleItem>(content, bufferIndex, typeof(ChannelPerScaleItem));
                        bufferIndex += perScaleDataBytes;
                    }
                }
            }
            int allPerScaleDataBytes = CaliConstants.Fixed_MaxPhysicsChannelCount * 2 * CaliConstants.Fixed_MaxPhyCoarseScaleCount * perScaleDataBytes;
            this.Trig_ACZero = BitConverter.ToUInt32(content, allPerScaleDataBytes + 0 * sizeof(UInt32));
            this.Trig_ACZero3Div = BitConverter.ToUInt32(content, allPerScaleDataBytes + 1 * sizeof(UInt32));

            if (content.Length>= TotalBytes)
            {
                int newAppendStartByteIndex = allPerScaleDataBytes + 2 * sizeof(UInt32);
                for(int i = 0; i < reservedInt32Count; i++)
                    reserved[i]= BitConverter.ToUInt32(content, newAppendStartByteIndex+sizeof(Int32)*i);
            }
            CheckValid();
        }
        private void LoadValue(ChannelPerScaleItem[/*impedanceIndex,50_Om=1 */ , /*yScaleIndex*/]? defaultValue)
        {
            if (defaultValue == null)
                return;
            for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                for (int impedanceIndex = 0; impedanceIndex < 2; impedanceIndex++)
                {
                    for (int scaleIndex = 0; scaleIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleIndex++)
                        perScaleData[channelIndex, impedanceIndex, scaleIndex] = defaultValue[impedanceIndex, scaleIndex];
                }
            }
            CheckValid();
        }
        public void LoadDefaultValue()
        {
            LoadValue(default_BW1G2G4G);
        }
        private ChannelPerScaleItem[/*impedanceIndex,50_Om=1 */ , /*yScaleIndex*/] default_BW1G2G4G =
        {
            {
                //高阻
                /*[(int)AnaChnlScaleIndex.Lv500u] = */  new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0 ,OffsetPreceding=0    ,OffsetPreceding_3Div=0     ,OffsetPosterior=0    ,OffsetPosterior_3Div=0    ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv1m] = */    new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0 ,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=11000,OffsetPosterior_3Div=80   ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv2m] = */    new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =5 ,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=11000,OffsetPosterior_3Div=160  ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv5m] = */    new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =13,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=11000,OffsetPosterior_3Div=400  ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv10m] = */   new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =19,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=11000,OffsetPosterior_3Div=800  ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv20m] = */   new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =25,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=11000,OffsetPosterior_3Div=1600 ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv50m] = */   new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =31,OffsetPreceding=32000,OffsetPreceding_3Div=145000,OffsetPosterior=11000,OffsetPosterior_3Div=3200 ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv100m] = */  new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =19,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=11000,OffsetPosterior_3Div=800  ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv200m] = */  new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =25,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=11000,OffsetPosterior_3Div=1600 ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv500m] = */  new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =31,OffsetPreceding=32000,OffsetPreceding_3Div=15000 ,OffsetPosterior=11000,OffsetPosterior_3Div=3200 ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv1] = */     new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =29,OffsetPreceding=32000,OffsetPreceding_3Div=5000  ,OffsetPosterior=11000,OffsetPosterior_3Div=2800 ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv2] = */     new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =16,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=11000,OffsetPosterior_3Div=560  ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv5] = */     new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =24,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=11000,OffsetPosterior_3Div=1400 ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv10] = */    new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =30,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=11000,OffsetPosterior_3Div=2800 ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv20] = */    new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0 ,OffsetPreceding=0    ,OffsetPreceding_3Div=0     ,OffsetPosterior=0    ,OffsetPosterior_3Div=0    ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
            },
            {
                //低阻
                /*[(int)AnaChnlScaleIndex.Lv500u] = */  new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0 ,OffsetPreceding=0    ,OffsetPreceding_3Div=0     ,OffsetPosterior=0    ,OffsetPosterior_3Div=0    ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv1m] = */    new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0 ,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=9500 ,OffsetPosterior_3Div=100  ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv2m] = */    new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =4 ,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=9500 ,OffsetPosterior_3Div=200  ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv5m] = */    new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =12,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=9500 ,OffsetPosterior_3Div=500  ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv10m] = */   new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =18,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=9500 ,OffsetPosterior_3Div=1000 ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv20m] = */   new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =24,OffsetPreceding=32000,OffsetPreceding_3Div=250000,OffsetPosterior=9500 ,OffsetPosterior_3Div=2000 ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv50m] = */   new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =18,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=9500 ,OffsetPosterior_3Div=900  ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv100m] = */  new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =24,OffsetPreceding=32000,OffsetPreceding_3Div=50000 ,OffsetPosterior=9500 ,OffsetPosterior_3Div=1800 ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv200m] = */  new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =15,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=9500 ,OffsetPosterior_3Div=600  ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv500m] = */  new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =23,OffsetPreceding=32000,OffsetPreceding_3Div=0     ,OffsetPosterior=9500 ,OffsetPosterior_3Div=1500 ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv1] = */     new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =29,OffsetPreceding=32000,OffsetPreceding_3Div=8500  ,OffsetPosterior=9500 ,OffsetPosterior_3Div=3000 ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv2] = */     new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0 ,OffsetPreceding=0    ,OffsetPreceding_3Div=0     ,OffsetPosterior=0    ,OffsetPosterior_3Div=0    ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv5] = */     new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0 ,OffsetPreceding=0    ,OffsetPreceding_3Div=0     ,OffsetPosterior=0    ,OffsetPosterior_3Div=0    ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv10] = */    new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0 ,OffsetPreceding=0    ,OffsetPreceding_3Div=0     ,OffsetPosterior=0    ,OffsetPosterior_3Div=0    ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
                /*[(int)AnaChnlScaleIndex.Lv20] = */    new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0 ,OffsetPreceding=0    ,OffsetPreceding_3Div=0     ,OffsetPosterior=0    ,OffsetPosterior_3Div=0    ,Gain_FineByAdc=128 ,Gain_FineByFpgaThousand=1000 },
            }

        };

        public void LoadDefaultValue(AnalogChannelType analogChannelType)
        {
            ChannelPerScaleItem[/*impedanceIndex,50_Om=1 */ , /*yScaleIndex*/]? defaultValue = analogChannelType switch
            {
                AnalogChannelType.BW1G2G4G or AnalogChannelType.BW2G6CH => default_BW1G2G4G,
                AnalogChannelType.JiHe2d5G => default_BW1G2G4G,

                _ => null,
            };
            if (defaultValue == null)
                return;
            LoadValue(defaultValue);
        }
    }
}

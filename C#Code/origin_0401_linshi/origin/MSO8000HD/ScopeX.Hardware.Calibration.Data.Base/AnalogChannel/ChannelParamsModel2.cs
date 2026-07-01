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
    public class ChannelParamsModel2 : ICaliData
    {
        public static ChannelParamsModel2 Default = new ChannelParamsModel2();
        private ChannelPerScaleItem[/*channelIndex*/ , /*impedanceIndex,50_Om=1 */ , /*yScaleIndex*/] perScaleData;
        public Int32 TotalBytes
        {
            get
            {
                int perScaleDataBytes = Marshal.SizeOf(perScaleData[0, 0, 0]);
                int allPerScaleDataBytes = 2 * CaliConstants.Fixed_MaxPhysicsChannelCount * CaliConstants.Fixed_MaxPhyCoarseScaleCount * perScaleDataBytes;//2=高阻、低阻
                int totalBytes = allPerScaleDataBytes +sizeof(UInt32) + sizeof(UInt32); //+ Trig_ACZero + Trig_ACZero3Div

                return totalBytes;
            }
        }
        public Int32 OriginTotleBytes
        {
            get => TotalBytes;
            set { }
        }
        public CaliDataType DataType { get => CaliDataType.PhyChannelModel2; }

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
        public ChannelPerScaleItem this[int channelIndex,int impedanceIndex,int yScaleIndex]
        {
            get => perScaleData[channelIndex, impedanceIndex, yScaleIndex];
            set => perScaleData[channelIndex, impedanceIndex, yScaleIndex]=value;
        }
        private ChannelParamsModel2()
        {
            perScaleData= new ChannelPerScaleItem[CaliConstants.Fixed_MaxPhysicsChannelCount, 2, CaliConstants.Fixed_MaxPhyCoarseScaleCount];
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
                        if (perScaleData[channelIndex, impedanceIndex, scaleIndex].Gain_FineByAdc == 0)
                            perScaleData[channelIndex, impedanceIndex, scaleIndex].Gain_FineByAdc = 10000;
                        if (perScaleData[channelIndex, impedanceIndex, scaleIndex].Gain_FineByFpgaThousand == 0)
                            perScaleData[channelIndex, impedanceIndex, scaleIndex].Gain_FineByFpgaThousand = 1000;
                    }
                }
            }
        }
        public void Deserialize(byte[] content)
        {
            int perScaleDataBytes = Marshal.SizeOf(perScaleData[0, 0, 0]);

            if (content.Length < TotalBytes)
                return;
            int bufferIndex = 0;
            for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
            {
                for (int impedanceIndex = 0; impedanceIndex < 2; impedanceIndex++)
                {
                    for (int scaleIndex = 0; scaleIndex < CaliConstants.Fixed_MaxPhyCoarseScaleCount; scaleIndex++)
                    {
                        perScaleData[channelIndex, impedanceIndex, scaleIndex] =Helper.BytesToStruct<ChannelPerScaleItem>(content, bufferIndex, typeof(ChannelPerScaleItem));
                        bufferIndex += perScaleDataBytes;
                    }
                }
            }
            int allPerScaleDataBytes = CaliConstants.Fixed_MaxPhysicsChannelCount * 2 * CaliConstants.Fixed_MaxPhyCoarseScaleCount * perScaleDataBytes;
            this.Trig_ACZero = BitConverter.ToUInt32(content, allPerScaleDataBytes + 0 * sizeof(UInt32));
            this.Trig_ACZero3Div = BitConverter.ToUInt32(content, allPerScaleDataBytes + 1 * sizeof(UInt32));
            CheckValid();
        }

        public void LoadDefaultValue()
        {
        }
        private ChannelPerScaleItem[/*impedanceIndex,50_Om=1 */ , /*yScaleIndex*/] default_BW8G =
        {
            {
                //高阻
                /*[(int)AnaChnlScaleIndex.Lv500u] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =2,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=0,OffsetPosterior_3Div=0 },
                /*[(int)AnaChnlScaleIndex.Lv1m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0,OffsetPreceding=32480,OffsetPreceding_3Div=0,OffsetPosterior=30800,OffsetPosterior_3Div=0 },
                /*[(int)AnaChnlScaleIndex.Lv2m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0,OffsetPreceding=32480,OffsetPreceding_3Div=0,OffsetPosterior=30800,OffsetPosterior_3Div=0 },
                /*[(int)AnaChnlScaleIndex.Lv5m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =5,OffsetPreceding=32480,OffsetPreceding_3Div=0,OffsetPosterior=30800,OffsetPosterior_3Div=80 },
                /*[(int)AnaChnlScaleIndex.Lv10m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =11,OffsetPreceding=32480,OffsetPreceding_3Div=0,OffsetPosterior=30800,OffsetPosterior_3Div=170 },
                /*[(int)AnaChnlScaleIndex.Lv20m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =16,OffsetPreceding=32480,OffsetPreceding_3Div=0,OffsetPosterior=30800,OffsetPosterior_3Div=325 },
                /*[(int)AnaChnlScaleIndex.Lv50m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =24,OffsetPreceding=32480,OffsetPreceding_3Div=0,OffsetPosterior=30800,OffsetPosterior_3Div=750 },
                /*[(int)AnaChnlScaleIndex.Lv100m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =12,OffsetPreceding=32480,OffsetPreceding_3Div=0,OffsetPosterior=30800,OffsetPosterior_3Div=185 },
                /*[(int)AnaChnlScaleIndex.Lv200m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =18,OffsetPreceding=32480,OffsetPreceding_3Div=0,OffsetPosterior=30800,OffsetPosterior_3Div=350 },
                /*[(int)AnaChnlScaleIndex.Lv500m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =24,OffsetPreceding=32480,OffsetPreceding_3Div=0,OffsetPosterior=30800,OffsetPosterior_3Div=800 },
                /*[(int)AnaChnlScaleIndex.Lv1] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =6,OffsetPreceding=32480,OffsetPreceding_3Div=0,OffsetPosterior=30800,OffsetPosterior_3Div=95 },
                /*[(int)AnaChnlScaleIndex.Lv2] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =10,OffsetPreceding=32480,OffsetPreceding_3Div=0,OffsetPosterior=30800,OffsetPosterior_3Div=167 },
                /*[(int)AnaChnlScaleIndex.Lv5] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =18,OffsetPreceding=32480,OffsetPreceding_3Div=0,OffsetPosterior=30800,OffsetPosterior_3Div=360 },
                /*[(int)AnaChnlScaleIndex.Lv10] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =2,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=0,OffsetPosterior_3Div=0 },
                /*[(int)AnaChnlScaleIndex.Lv20] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =2,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=0,OffsetPosterior_3Div=0 },
            },
            {
                //低阻
                /*[(int)AnaChnlScaleIndex.Lv500u] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =2,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=0,OffsetPosterior_3Div=0 },
                /*[(int)AnaChnlScaleIndex.Lv1m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=31300,OffsetPosterior_3Div=0 },
                /*[(int)AnaChnlScaleIndex.Lv2m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=31300,OffsetPosterior_3Div=0 },
                /*[(int)AnaChnlScaleIndex.Lv5m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=31300,OffsetPosterior_3Div=0 },
                /*[(int)AnaChnlScaleIndex.Lv10m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =0,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=31300,OffsetPosterior_3Div=2150 },
                /*[(int)AnaChnlScaleIndex.Lv20m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =16350,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=31568,OffsetPosterior_3Div=0 },
                /*[(int)AnaChnlScaleIndex.Lv50m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =65500,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=30068,OffsetPosterior_3Div=17000 },
                /*[(int)AnaChnlScaleIndex.Lv100m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =29200,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=30068,OffsetPosterior_3Div=8090 },
                /*[(int)AnaChnlScaleIndex.Lv200m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =34800,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=29668,OffsetPosterior_3Div=15600 },
                /*[(int)AnaChnlScaleIndex.Lv500m] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =23000,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=31068,OffsetPosterior_3Div=4700 },
                /*[(int)AnaChnlScaleIndex.Lv1] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =31200,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=29868,OffsetPosterior_3Div=10300 },
                /*[(int)AnaChnlScaleIndex.Lv2] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =2,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=0,OffsetPosterior_3Div=0 },
                /*[(int)AnaChnlScaleIndex.Lv5] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =2,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=0,OffsetPosterior_3Div=0 },
                /*[(int)AnaChnlScaleIndex.Lv10] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =2,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=0,OffsetPosterior_3Div=0 },
                /*[(int)AnaChnlScaleIndex.Lv20] = */new ChannelPerScaleItem(){ Gain_CoarseCtrlWord =2,OffsetPreceding=0,OffsetPreceding_3Div=0,OffsetPosterior=0,OffsetPosterior_3Div=0 },
            }

        };

        public void LoadDefaultValue(AnalogChannelType analogChannelType)
        {
            ChannelPerScaleItem[/*impedanceIndex,50_Om=1 */ , /*yScaleIndex*/]? defaultValue = analogChannelType switch
            {
                AnalogChannelType.BW8G => default_BW8G,
                _ =>null,
            };
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
    }
}

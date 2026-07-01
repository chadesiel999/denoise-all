using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class CoefficientsTables : ICaliData/*, ICoefficientsTable*/
    {
        public static CoefficientsTables Default = new CoefficientsTables();
        public const Int32 Fixed_TypeCount = 8;
        public const Int32 Fixed_PerChannelDataCount = 8*1024;
        private CoefficientsTables()
        {
            for(int typeIndex=0;typeIndex< Fixed_TypeCount; typeIndex++)
            {
                data[typeIndex] = new Int32[CaliConstants.Fixed_MaxPhysicsChannelCount][];
                for (int channelID = 0; channelID < CaliConstants.Fixed_MaxPhysicsChannelCount; channelID++)
                    data[typeIndex][channelID] = new Int32[Fixed_PerChannelDataCount];
            }
        }
        public CaliDataType DataType { get => CaliDataType.CoefficientsTables; }

        private Int32[/*CoefficientsType*/][/*channel*/][/*data*/] data = new Int32[Fixed_TypeCount][][];
        public Int32 TotalBytes
        {
            get
            {
                int totalBytes = Fixed_TypeCount * CaliConstants.Fixed_MaxPhysicsChannelCount * Fixed_PerChannelDataCount * sizeof(Int32);
                return totalBytes;
            }
        }
        public Int32 OriginTotleBytes
        {
            get => TotalBytes;
            set { }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="coefficientsTableType"></param>
        /// <param name="channelIndex"></param>
        /// <returns></returns>
        public Int32[] this[CoefficientsTableType coefficientsTableType, int channelIndex]
        {
            get => data[(int)coefficientsTableType][channelIndex];
        }
        /// <summary>
        /// [CoefficientsTableType coefficientsTableType,int channelIndex,int index]
        /// </summary>
        /// <param name="coefficientsTableType"></param>
        /// <param name="channelIndex"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Int32 this[CoefficientsTableType coefficientsTableType, int channelIndex,int index]
        {
            get => data[(int)coefficientsTableType][channelIndex][index];
            set => data[(int)coefficientsTableType][channelIndex][index] = value;
        }
        public byte[] Serialize()
        {
            System.IO.MemoryStream memoryStream = new MemoryStream();
            for (int typeIndex = 0; typeIndex < Fixed_TypeCount; typeIndex++)
            {
                for (int channelIndex = 0; channelIndex < CaliConstants.Fixed_MaxPhysicsChannelCount; channelIndex++)
                {
                    for (int i = 0; i < Fixed_PerChannelDataCount; i++)
                    {
                        //if (typeIndex == 1 && channelIndex == 0 && i >= 3903)
                        //    ;
                        memoryStream.Write(Helper.StructToBytes(data[typeIndex][channelIndex][i]));
                    }
                }
            }
            byte[] result = memoryStream.ToArray();
            memoryStream.Close();
            return result;
        }
        public byte[] SerializeByFpgaFormat(CoefficientsTableType coefficientsTableType, int channelID)
        {
            byte[] bytes = new byte[Fixed_PerChannelDataCount * (24/8)];//24=24bits,8=byte's bits
            for (int i = 0; i < Fixed_PerChannelDataCount; i++)
            {
                bytes[i * 3 + 0] =(byte)(data[(int)coefficientsTableType][channelID][i] & 0xff);
                bytes[i * 3 + 1] = (byte)(data[(int)coefficientsTableType][channelID][i]>>8 & 0xff);
                bytes[i * 3 + 2] = (byte)(data[(int)coefficientsTableType][channelID][i] >> 16 & 0xff);
            }
            return bytes;
        }
        public void Deserialize(byte[] content)
        {
            if (content.Length < TotalBytes)
                return;
            int perTypeBytes = CaliConstants.Fixed_MaxPhysicsChannelCount * Fixed_PerChannelDataCount * sizeof(Int32);
            int perChannelBytes = Fixed_PerChannelDataCount * sizeof(Int32);
            for (int typeIndex = 0; typeIndex < Fixed_TypeCount; typeIndex++)
            {
                for (int channelID = 0; channelID < CaliConstants.Fixed_MaxPhysicsChannelCount; channelID++)
                {
                    for (int index = 0; index < Fixed_PerChannelDataCount; index++)
                    {
                        data[typeIndex][channelID][index] = Helper.BytesToStruct<Int32>(content, typeIndex * perTypeBytes + channelID * perChannelBytes + index * sizeof(Int32), typeof(Int32));
                    }
                }
            }
        }
    }
}

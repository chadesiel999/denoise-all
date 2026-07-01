using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using ScopeX.ComModel;
namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class MiscData : ICaliData
    {
        public static MiscData Default = new MiscData();
        public CaliDataType DataType { get => CaliDataType.Misc; }
        private MiscData()
        {
            ConstructDefaultValue();
        }
        private void ConstructDefaultValue()
        {
            #region 外触发
            #endregion

            #region 不同类型的触发
            #endregion
            #region LA的缺省值
            AllData[(int)MiscDefine.LA_CaliDataBlock1_VotageBymV] = 32000;
            AllData[(int)MiscDefine.LA_CaliDataBlock1_RadioMulti1000] = 7000;
            AllData[(int)MiscDefine.LA_CaliDataBlock2_VotageBymV] = 32000;
            AllData[(int)MiscDefine.LA_CaliDataBlock2_RadioMulti1000] = 7000;
            AllData[(int)MiscDefine.LA_CaliDataBlock3_VotageBymV] = 32000;
            AllData[(int)MiscDefine.LA_CaliDataBlock3_RadioMulti1000] = 7000;
            AllData[(int)MiscDefine.LA_CaliDataBlock4_VotageBymV] = 32000;
            AllData[(int)MiscDefine.LA_CaliDataBlock4_RadioMulti1000] = 7000;
            #endregion
        }
        const Int32 TotalCount = 512;
        int[] AllData = new Int32[TotalCount];
        public Int32 TotalBytes { get => TotalCount * sizeof(Int32); }
        public Int32 OriginTotleBytes
        {
            get => TotalBytes;
            set { }
        }
        public Int32 this[int index]
        {
            get => AllData[index];
            set => AllData[index] = value;
        }
        public byte[] Serialize()
        {
            System.IO.MemoryStream memoryStream = new MemoryStream();
            for (int i = 0; i < TotalCount; i++)
            {
                memoryStream.Write(Helper.StructToBytes(AllData[i]));
            }
            byte[] result = memoryStream.ToArray();
            memoryStream.Close();

            return result;
        }
        public void Deserialize(byte[] content)
        {
            if (content.Length < TotalBytes)
                return;
            Int32 perItemBytes = sizeof(Int32);
            Int32 byteIndex = 0;
            for (int i = 0; i < TotalCount; i++)
            {
                AllData[i] = Helper.BytesToStruct<Int32>(content, byteIndex, typeof(Int32));
                byteIndex += perItemBytes;
            }
        }
        public void LoadDefaultValue()
        {
            ConstructDefaultValue();
        }
    }
}

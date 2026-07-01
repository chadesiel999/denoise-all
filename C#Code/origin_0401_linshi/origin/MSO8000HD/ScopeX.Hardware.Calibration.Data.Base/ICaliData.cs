using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public interface ICaliData
    {
        CaliDataType DataType { get; }
        Int32 TotalBytes { get; }
        Int32 OriginTotleBytes { get; set; }
        byte[] Serialize();
        private string CaliDataPath
        {
            get => AppDomain.CurrentDomain.BaseDirectory + @"CaliData\";
        }
        void Deserialize(byte[] content);
        /// <summary>
        /// 该函数只能在与Hardware同进程的函数中使用。
        /// </summary>
        void SaveToFile()
        {
            string fileName = CaliDataPath + DataType.ToString()+".bin";
            if (!Directory.Exists(CaliDataPath))
                Directory.CreateDirectory(CaliDataPath);
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllBytes(fileName, Serialize());
        }
        /// <summary>
        /// 该函数只能在与Hardware同进程的函数中使用。
        /// </summary>
        /// <returns></returns>
        Boolean LoadFromFile()
        {
            string fileName = CaliDataPath + DataType.ToString() + ".bin";
            if (!File.Exists(fileName))
                return false;
            Deserialize(File.ReadAllBytes(fileName));
            return true;
        }

        /// <summary>
        /// 该函数只能在Tools中使用
        /// </summary>
        public void SaveToFile(string path)
        {
            string fileName = path + DataType.ToString() + ".bin";
            if (File.Exists(fileName))
                File.Delete(fileName);
            File.WriteAllBytes(fileName, Serialize());
        }
        /// <summary>
        /// 该函数只能在Tools中使用
        /// </summary>
        Boolean LoadFromFile(string path)
        {
            string fileName = path + DataType.ToString() + ".bin";
            if (!File.Exists(fileName))
                return false;
            Deserialize(File.ReadAllBytes(fileName));
            return true;
        }

        void LoadDefaultValue()
        {
        }
    }
}

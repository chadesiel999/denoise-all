using ScopeX.ComModel;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Converters;

namespace ScopeX.Hardware.Calibration.Data.Base
{
    public class CalibrationData
    {
        /// <summary>
        /// 校准人员
        /// </summary>
        public String? Calibrator { get; set; } = "GHz";

        /// <summary>
        /// 校准类型
        /// </summary>
        public CaliDataType Type { get; set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public String? ProductModel { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public String? Verion { get; set; } = "V1";

        /// <summary>
        /// 时间戳
        /// </summary>
        public String? Timestamp { get; set; }

        /// <summary>
        /// 校准数据
        /// </summary>
        public Byte[]? Data { get; set; }

        public UInt32 CheckCode { get; set; } = 0;
    }

    public class CalibrationStore
    {
        public static CalibrationStore Default = new CalibrationStore();

        private String FilePath => $"{AppDomain.CurrentDomain.BaseDirectory}CaliData";

        public Byte[] GetCaliData(CaliDataType type, Boolean loadLocalCaliData = false)
        {
            if (loadLocalCaliData)
            {
                Helper.GetICaliData(type)?.LoadFromFile();
            }

            //CalibrationData
            var caliData = new CalibrationData();
            caliData.ProductModel = Constants.PRODUCT.ToString();
            caliData.Timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.ffff");
            caliData.Type = type;

            caliData.Data = Helper.GetICaliData(type)?.Serialize();
            caliData.CheckCode = GetCRC32(caliData.Data);

            // 配置 JsonSerializerSettings
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = { new StringEnumConverter() } // 全局应用 StringEnumConverter
            };
            var jsonstring = JsonConvert.SerializeObject(caliData, settings);
            var jsonbytes = Encoding.Default.GetBytes(jsonstring);

            var filepath = Path.Combine(FilePath, $"{caliData.Type}.json");
            File.WriteAllText(filepath, jsonstring);
            return jsonbytes;
        }

        public Boolean SetCaliData(CaliDataType type, Byte[] buffer)
        {
            if (buffer == null || buffer.Length <= 0)
            {
                return false;
            }
            // 1
            var jsonstring = Encoding.Default.GetString(buffer);
            // 配置 JsonSerializerSettings
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = { new StringEnumConverter() } // 全局应用 StringEnumConverter
            };
            var calidata = JsonConvert.DeserializeObject<CalibrationData>(jsonstring, settings);
            if (calidata == null || calidata.Type != type || calidata.Data == null || calidata.Data.Length <= 0)
            {
                return false;
            }

            var checkcode = GetCRC32(calidata.Data);
            if (checkcode != calidata.CheckCode)
            {
                return false;
            }

            switch (type)
            {

                case CaliDataType.Misc:
                    MiscData.Default?.Deserialize(calidata.Data);
                    break;
                case CaliDataType.AWG:
                    AWGCaliData.Default?.Deserialize(calidata.Data);
                    break;
                case CaliDataType.AnalogParams:
                    AnalogChannelParams.Default?.Deserialize(calidata.Data);
                    break;
                case CaliDataType.TiadcPhaseOffsetGainParams:
                    TiadcPhaseOffsetGainParams.Default?.Deserialize(calidata.Data);
                    break;
                case CaliDataType.CoefficientsParams:
                    CoefficientsParams.Default?.Deserialize(calidata.Data);
                    break;
                default:
                    break;
            }
            return true;
        }

        private UInt32 GetCRC32(Byte[]? buffer)
        {
            if (buffer == null || buffer.Length <= 0)
            {
                return UInt32.MaxValue;
            }

            uint crc = 0xFFFFFFFF;
            const uint polynomial = 0xEDB88320;
            for (int i = 0; i < buffer.Length; i++)
            {
                crc ^= buffer[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) != 0)
                        crc = (crc >> 1) ^ polynomial;
                    else
                        crc >>= 1;
                }
            }
            return ~crc;
        }
    }
}

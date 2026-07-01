using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public class CalibrationOscilloscopeInfo
    {
        public string ProductType { get; set; }
        public string SerialNumber { get; set; }
        public string SoftVersion { get; set; }

        private static CalibrationOscilloscopeInfo _calibrationOscilloscopeInfo;

        private string _fileDir;
        public string FileDir
        {
            get
            {
                string _fileDir = Path.Combine(Environment.CurrentDirectory, $"{ProductType}\\{SerialNumber}\\");
                if (!Directory.Exists(_fileDir))
                {
                    Directory.CreateDirectory(_fileDir);
                }
                return _fileDir;
            }
        }

        public static CalibrationOscilloscopeInfo? Defalut { get { return _calibrationOscilloscopeInfo; } }

        public static void SetCalibrationOscilloscopeInfo(string info)
        {
            _calibrationOscilloscopeInfo = new CalibrationOscilloscopeInfo();
            if (!string.IsNullOrEmpty(info) && info.Split(',').Length > 3)
            {
                string[] infos = info.Split(',');
                if (infos[1] != "NULL")
                {
                    _calibrationOscilloscopeInfo.ProductType = infos[1];
                    _calibrationOscilloscopeInfo.SerialNumber = infos[2];
                    _calibrationOscilloscopeInfo.SoftVersion = infos[3];
                }
                else
                {
                    _calibrationOscilloscopeInfo.ProductType = "unknown";
                    _calibrationOscilloscopeInfo.SerialNumber = "unknown";
                    _calibrationOscilloscopeInfo.SoftVersion = "unknown";
                }
            }
            else
            {
                _calibrationOscilloscopeInfo.ProductType = "test";
                _calibrationOscilloscopeInfo.SerialNumber = "test";
                _calibrationOscilloscopeInfo.SoftVersion = "test";
            }
        }

    }
}

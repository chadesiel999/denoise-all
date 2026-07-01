using ScopeX.Hardware.Calibration.Tool.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ScopeX.Hardware.Calibration.Tool.Utilities
{
    internal class Logger : IDisposable
    {
        private FileStream _FileStream;
        string filePath;
        private object _Locker = new object();

        private static Logger _Instance;
        private static Logger Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Logger();
                return _Instance;
            }
        }

        public static void RefreshLogger()
        {
            if (_Instance != null)
            {
                Logger.Instance._FileStream.Close();
                Logger.Instance._FileStream.Dispose();
            }
            _Instance = new Logger();
        }


        private Logger()
        {
            string fileDir = $"{CalibrationOscilloscopeInfo.Defalut?.FileDir}Log_Cali";
            //string fileDir = Path.Combine(Environment.CurrentDirectory, "Log_Cali");
            filePath = Path.Combine(fileDir, $"{DateTime.Now.ToString("MMdd_HH_mm_ss")}.txt");
            if (!Directory.Exists(fileDir))
                Directory.CreateDirectory(fileDir);

            _FileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        void IDisposable.Dispose()
        {
            _FileStream.Close();
            _FileStream.Dispose();
        }

        private void WriteInternal(string text)
        {
            lock (_Locker)
            {
                byte[] info = new UTF8Encoding(true).GetBytes(text);
                _FileStream.Write(info, 0, info.Length);
                _FileStream.Flush();
            }
        }

        private void WriteLineInternal(string text)
        {
            Write($"[{DateTime.Now.ToString("HH:mm:ss")}] : ");
            Write(text);
            Write("\r\n");
        }

        public static void Write(string text)
        {
            Logger.Instance.WriteInternal(text);
        }
        public static void WriteLine(string text)
        {
            Logger.Instance.WriteLineInternal(text);
        }
        public static void LogCaliInfo(long uniqueId, string infoType, string funcName, string info)
        {
            WriteLine(info);
            new CaliLogInfo()
            {
                ExecuteId = uniqueId,
                DateTime = DateTime.Now,
                InfoType = infoType,
                FuncName = funcName,
                Content = info
            }.Insert();
        }
    }
}

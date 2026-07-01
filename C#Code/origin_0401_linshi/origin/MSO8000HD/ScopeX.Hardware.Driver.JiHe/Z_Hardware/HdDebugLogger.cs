using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Driver
{
    public class HdDebugLogger
    {
        private static FileStream? _LogStream;
        private const string FileDir = @"ReadbackStatus\AdcConfig\";

        public static Boolean Enable { get; set; } = false;

        public static void Log(String msg)
        {
            if (!Enable)
                return;

            if (_LogStream == null)
            {
                if (!Directory.Exists(FileDir))
                    Directory.CreateDirectory(FileDir);
                String filename = $"AutoSet_{DateTime.Now.ToString("MMddHHmm")}.txt";
                String filepath = Path.Combine(FileDir, filename);
                _LogStream = new FileStream(filepath, FileMode.Create);
            }
            var msgbytes = Encoding.UTF8.GetBytes(msg + Environment.NewLine);
            _LogStream.Write(msgbytes);
            _LogStream.Flush();
        }
    }
}

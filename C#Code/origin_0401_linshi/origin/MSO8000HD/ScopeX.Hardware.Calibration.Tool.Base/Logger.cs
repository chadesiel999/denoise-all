using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Hardware.Calibration.Tool.Base
{
    public class Logger
    {
        private StringBuilder sb = new StringBuilder();
        public static Logger Defualt = new Logger();
        public void WriteLine(string message)
        {
            sb.AppendLine(message);
        }
        public void Clear()
        {
            sb.Clear();
        }
        public string GetContent()
        {
            return sb.ToString();
        }
    }
}

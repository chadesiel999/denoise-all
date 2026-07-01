using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ScopeX.Updater
{
    /// <summary>
    /// 日志帮助类，从MainForm中提取出来的。
    /// </summary>
    internal static class LogHelper
    {
        private static readonly ReaderWriterLockSlim writerLogLockSlim = new();

        /// <summary>
        /// 日志文件名称
        /// </summary>
        private static string _logFileName = $"Update_Log{DateTime.Now:yyMMdd_HHmmss}.txt";
        public static string LogFileName
        {
            get => _logFileName;
        }

        /// <summary>
        /// 日志文件路径
        /// </summary>
        private static string _logFilePath = string.Empty ;
        public static string LogFilePath
        {
            get
            {
                if (_logFilePath == null || _logFilePath.Length == 0)
                {
                    var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    var dirPath = Path.Combine(path, "UNIT", "Logs");
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }

                    _logFilePath = Path.Combine(dirPath, LogFileName);
                }

                return _logFilePath;
            }
        }

        private static string ConvertNowTimeToString()
        {
            DateTime now = DateTime.Now;
            return string.Format(now.Year.ToString() + "." + now.Month.ToString().PadLeft(2, '0') + "." + now.Day.ToString().PadLeft(2, '0') + " " + now.Hour.ToString().PadLeft(2, '0') + ":" + now.Minute.ToString().PadLeft(2, '0') + ":" +
                                 now.Second.ToString().PadLeft(2, '0'));
        }

        internal static void WriteLog(string message)
        {
            try
            {
                writerLogLockSlim.EnterWriteLock();
                File.AppendAllText(LogFilePath, ConvertNowTimeToString() + "=>" + message + System.Environment.NewLine);
            }
            catch
            {

            }
            finally
            {
                writerLogLockSlim.ExitWriteLock();
            }
        }

        /// <summary>
        /// 增加InnoSetup安装包固件升级后的错误提示内容
        /// </summary>
        /// <param name="message"></param>
        internal static void WriteInnoSetupMessageInfos(string message)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(message))
                    File.AppendAllText(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "innosetupmsg.txt"), message + System.Environment.NewLine);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
        }
    }
}

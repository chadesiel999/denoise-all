using NPOI.SS.Formula.Functions;
using ScopeX.Core.Decode;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.Core.Decode
{
    internal static class DecoderLogger
    {
        public static Boolean State
        {
            get=>_State;
        } 
        private static Boolean _State = DecoderLogger.Config();
        //private static IntPtr _LogInsPtr;

        // 日志委托 解码的初始化接口 日志信息区分协议
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Int32 LogDelegate(Int32 logLevel, [MarshalAs(UnmanagedType.LPStr)] String logMsg);
        private static LogDelegate _LogDelegate;// = new LogDelegate(Log);

        /// <summary>
        /// 注入日志接口，供解码使用
        /// </summary>
        /// <param name="logPtr"></param>
        [DllImport("Decode\\Decoder.dll")]
        private static extern void SetLogger(LogDelegate logPtr, Int32 loggerType);
        public static Int32 Log(Int32 logLevel, String logMsg)
        {
            EventBus.LogLevel logleveltmp;
            switch(logLevel)
            {
                case 0:
                case 1:
                    logleveltmp = EventBus.LogLevel.Debug;
                    break;
                case 2:
                    logleveltmp = EventBus.LogLevel.Info;
                    break;
                case 3:
                    logleveltmp = EventBus.LogLevel.Warn;
                    break;
                case 4:
                case 5:
                    logleveltmp = EventBus.LogLevel.Error;
                    break;
                default:
                    return 0;
            }
            EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"=====Decoder CPP CANCELED (id = {Environment.CurrentManagedThreadId})! =====" + logMsg, (EventBus.LogLevel)(logleveltmp)));
            return 1;
        }

        public static Boolean Config()
        {
            try
            {
               _LogDelegate = new LogDelegate(Log);
                //_LogInsPtr = Marshal.GetFunctionPointerForDelegate(_LogDelegate);
                //String logname = "decode";
                //String filename = "logs/decode.log";
                //_LogInsPtr = GetLogger(logname, filename, 0);
                //SetLogger(_LogDelegate, 0);// 为解码注入日志接口
            }
            catch (Exception e)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), 
                    new EventBus.LogEventArgs($"=====LoggerCPP.Config ERROR (id = {Environment.CurrentManagedThreadId})! =====\n{e.Message}\n{e.StackTrace}", EventBus.LogLevel.Debug));
#if DEBUG
                Trace.WriteLine($"=====Config.Config ERROR (id = {Environment.CurrentManagedThreadId})! =====");
#endif
            }
            return true;
        }

        // TODO :保留注释的内容，日志配置
        //[StructLayout(LayoutKind.Sequential)]
        //private struct LoggerConfig
        //{
        //    // 日志文件特性配置
        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        //    Byte[] LoggeName; // 日志记录器名称，默认decod
        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        //    Byte[] FileName; // 日志文件名，默认logs/decode.log
        //    UInt32 FileMaxSize; // 日志最大size,最大5M，以Byte为单位，即5*1024*1024
        //    UInt32 FileMaxNum;   // 日志最大个数，最大为8个
        //    // 日志文件内容配置
        //    Int32 LogLevel; //    TRACE = 0,DEBUG,INFO,WARN,ERROR, CRITICAL,OFF,
        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]// 日志等级
        //    Byte LogPattern; // 日志格式，默认格式"[%Y-%m-%d %H:%M:%S.%e][%l][%t][%s %!:%#] %v"
        //    Int32 LogFlushTime; // 日志缓冲区刷新时间，单位秒，默认5秒
        //    Byte IsAsyn; // 是否异步，默认同步
        //}
    }
}

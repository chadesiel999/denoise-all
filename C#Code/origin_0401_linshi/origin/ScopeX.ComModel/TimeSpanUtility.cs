using System;
using System.Diagnostics;

namespace ScopeX.ComModel
{
    /// <summary>
    /// 时间戳帮助类，用于避免DateTime.Now由于修改了系统时间导致的程序错误
    /// </summary>
    public static class TimeSpanUtility
    {
        /// <summary>
        /// 获取时间戳，避免DateTime.Now由于修改了系统时间导致的程序错误。
        /// </summary>
        /// <returns></returns>
        public static TimeSpan GetTimestampSpan()
        {
            double timestampSeconds = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency * 1000d;
            TimeSpan timestampTimeSpan = TimeSpan.FromMilliseconds(timestampSeconds);
            return timestampTimeSpan;
        }

        /// <summary>
        /// 获取开机以来以毫秒为单位的值
        /// </summary>
        /// <returns></returns>
        public static long GetTimesLongByms() => (long)(Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency * 1000d);

        /// <summary>
        /// 获取时间戳，避免DateTime.Now由于修改了系统时间导致的程序错误。
        /// </summary>
        /// <param name="basetime">基准时间</param>
        /// <returns></returns>
        public static DateTime GetTimestampDateTime(DateTime basetime)
        {
            double timestampSeconds = Stopwatch.GetTimestamp() / (double)Stopwatch.Frequency * 1000d;
            TimeSpan timestampTimeSpan = TimeSpan.FromMilliseconds(timestampSeconds);
            return basetime + timestampTimeSpan;
        }
    }
}

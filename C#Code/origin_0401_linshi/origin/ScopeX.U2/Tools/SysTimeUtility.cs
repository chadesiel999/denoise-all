using System;
using System.Text.RegularExpressions;

namespace ScopeX.U2
{
    internal class SysTimeUtility
    {
        internal static SysTimeUtility Default = new SysTimeUtility();

        private SysTimeUtility()
        {

        }

        /// <summary>
        /// 获取本地时间
        /// 返回格式为年，月，日，时，分，秒
        /// 如2017年7月7日20点8分8秒返回为："2017,7,7,20,8,8"
        /// </summary>
        /// <returns></returns>
        internal String GetLocalTime()
        {
            var currenttime = new NativeMethods.SystemTime();
            NativeMethods.GetLocalTime(ref currenttime);

            var time = $"{currenttime.Year},{currenttime.Month},{currenttime.Day},{currenttime.Hour},{currenttime.Minute},{currenttime.Second}";
            return time;
        }

        /// <summary>
        /// 设置本地时间 
        /// 时间格式为"年，月，日，时，分，秒"
        /// 如："2017,7,7,20,8,8"则设置本地时间为2017年7月7日20点8分8秒
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        internal Boolean SetLocalTime(String time)
        {
            // 正则表达式匹配 "YYYY,M,D,H,m,s" 格式
            var pattern = @"^\d{4},\d{1,2},\d{1,2},\d{1,2},\d{1,2},\d{1,2}$";
            if (!Regex.IsMatch(time, pattern))
            {
                return false;
            }

            // 分割并解析时间
            var parts = time.Split(',');
            if (parts.Length != 6)
            {
                return false;
            }

            try
            {
                var year = UInt16.Parse(parts[0]);
                var month = UInt16.Parse(parts[1]);
                var day = UInt16.Parse(parts[2]);
                var hour = UInt16.Parse(parts[3]); //这个函数使用的是0时区的时间,例如，要设12点，则为12 - 8
                var minute = UInt16.Parse(parts[4]);
                var second = UInt16.Parse(parts[5]);

                // 检查年份
                if (year < DateTime.MinValue.Year || year > DateTime.MaxValue.Year)
                {
                    return false;
                }

                //检查月份
                if (month < 1 || month > 12)
                {
                    return false;
                }

                // 检查月份的天数（考虑闰年）
                var maxday = DateTime.DaysInMonth(year, month);
                if (day < 1 || day > maxday)
                {
                    return false;
                }

                //检查时分秒
                if (hour < 0 || hour > 23 || minute < 0 || minute > 59 || second < 0 || second > 59)
                {
                    return false;
                }

                var bOK = false;
                var temptime = new NativeMethods.SystemTime();
                temptime.Year = year;
                temptime.Month = month;
                temptime.Day = day;
                temptime.Hour = hour; //这个函数使用的是0时区的时间,例如，要设12点，则为12 - 8
                temptime.Minute = minute;
                temptime.Second = second;

                bOK = NativeMethods.SetLocalTime(ref temptime);
                return bOK;
            }
            catch (Exception ex) 
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
                return false;
            }
        }
    }
}

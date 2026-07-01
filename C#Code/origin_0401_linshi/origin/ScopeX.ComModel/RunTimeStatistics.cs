using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ScopeX.ComModel
{
    /// <summary>
    /// 运行时间统计工具，用于统计代码运行时间，非线程安全
    /// </summary>
    public class RunTimeStatistics : IDisposable
    {
        public static RunTimeStatistics Default { get; private set; }
        static RunTimeStatistics()
        {
            Default = new RunTimeStatistics();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="log"></param>
        public void InitLog(Action<string> log, uint defaultcachelength = 2000)
        {
            _log = log;
            MaxCacheCount = defaultcachelength;
        }

        private uint _maxCacheCount = 2000;

        /// <summary>
        /// 最大缓存数量默认值，小于此值时，不立即写入到日志
        /// </summary>
        public uint MaxCacheCount
        {
            get { return _maxCacheCount; }
            set
            {
                _maxCacheCount = value < 10 ? 10 : value;
            }
        }

        private Dictionary<string, uint> _tagCacheLength = new Dictionary<string, uint>();

        private Action<string> _log;

        private RunTimeStatistics()
        {
        }

        public RunTimeStatistics(Action<string> log, uint defaultcachelength = 2000)
        {
            _log = log;
            MaxCacheCount = defaultcachelength;
        }

        ~RunTimeStatistics()
        {
            Dispose();
        }

        private Stopwatch sw = new Stopwatch();

        /// <summary>
        ///  计时启动时间
        /// </summary>
        private long _startms = 0;

        /// <summary>
        /// 记录结果
        /// </summary>
        private Dictionary<string, List<long>> _singleMarkRecorders = new Dictionary<string, List<long>>();

        /// <summary>
        /// 范围标志记录
        /// </summary>
        private Dictionary<string, List<(long?, long?)>> _rangeMarkRecorders = new Dictionary<string, List<(long?, long?)>>();

        /// <summary>
        /// 配置指定Key的最大缓存数量
        /// </summary>
        /// <param name="tagName">标志名称</param>
        /// <param name="maxlength">最大缓存数量</param>
        public void ConfigCacheByKey(string tagName, uint maxlength)
        {
            if (string.IsNullOrEmpty(tagName))
                return;

            _tagCacheLength[tagName] = maxlength;
        }

        /// <summary>
        /// 单次标记，用于记录统计整个循环用时
        /// </summary>
        /// <param name="tagname">标志名称</param>
        public void SingleMark(string tagname)
        {
            if (string.IsNullOrEmpty(tagname))
                return;

            if (!sw.IsRunning)
            {
                sw.Start();
                _startms = TimeSpanUtility.GetTimesLongByms();
            }

            if (_singleMarkRecorders.ContainsKey(tagname))
            {
                var temp = _singleMarkRecorders[tagname];
                temp!.Add(TimeSpanUtility.GetTimesLongByms());
                uint ct = MaxCacheCount;
                if (_tagCacheLength.ContainsKey(tagname))
                    ct = _tagCacheLength[tagname];

                if (temp.Count > ct)
                {
                    PrintSingleMarkAverageTime(tagname);
                    temp.Clear();
                }
            }
            else
            {
                _singleMarkRecorders.Add(tagname, new List<long>()
                {
                    TimeSpanUtility.GetTimesLongByms()
                });
            }
        }

        /// <summary>
        /// 范围统计
        /// </summary>
        /// <param name="tagname"></param>
        public void RangeMark(string tagname)
        {
            /*
             用法示例：
             RunTimeStatistics.Default.RangeMark(pciereaddata);
            var retVal = HdIO.DMARead(dataLength, ref dmaBuff);
            RunTimeStatistics.Default.RangeMark(pciereaddata);
             */
            if (string.IsNullOrEmpty(tagname))
                return;

            if (!sw.IsRunning)
            {
                sw.Start();
                _startms = TimeSpanUtility.GetTimesLongByms();
            }

            if (_rangeMarkRecorders.ContainsKey(tagname))
            {
                var temp = _rangeMarkRecorders[tagname];
                var lastrecord = temp.Last();
                if (lastrecord.Item1 != null && lastrecord.Item2 == null)
                {
                    temp.RemoveAt(temp.Count - 1);
                    temp.Add((lastrecord.Item1, TimeSpanUtility.GetTimesLongByms()));
                }
                else if (lastrecord.Item1 != null && lastrecord.Item2 != null)
                {
                    uint cachelength = MaxCacheCount;
                    if (_tagCacheLength.ContainsKey(tagname))
                        cachelength = _tagCacheLength[tagname];

                    if (temp.Count >= cachelength)
                    {
                        // 记录平均值
                        PrintRangeAveragTimeByName(tagname);
                    }

                    temp.Add((TimeSpanUtility.GetTimesLongByms(), null));
                }
                else
                {

                }
            }
            else
            {
                _rangeMarkRecorders.Add(tagname, new List<(long?, long?)>()
                {
                    (TimeSpanUtility.GetTimesLongByms(), null),
                });
            }
        }

        /// <summary>
        /// 记录范围统计的平均值
        /// </summary>
        /// <param name="tagname"></param>
        private void PrintRangeAveragTimeByName(string tagname)
        {
            var recorders = _rangeMarkRecorders[tagname];
            var tp = recorders.Where(c => c.Item1 != null && c.Item2 != null).Select(c => c.Item2 - c.Item1);

            var averageval = tp.Average();
            _log?.Invoke($"{tagname} {tp.Count()} 次记录的平均耗时为：{averageval} ms");

            recorders.Clear();
        }

        /// <summary>
        /// 记录一次平均时间
        /// </summary>
        /// <param name="key"></param>
        public void PrintSingleMarkAverageTime(string key)
        {
            if (string.IsNullOrEmpty(key) || !_singleMarkRecorders.ContainsKey(key))
                return;

            var recorders = _singleMarkRecorders[key];
            var offsets = recorders.Zip(recorders.Skip(1), (a, b) => b - a).ToList();
            var averg = offsets.Average();

            _log?.Invoke($"{key} {recorders.Count - 1} 次记录的平均耗时为：{averg} ms");
        }

        /// <summary>
        /// 打印某个标识的最后两次记录的时间差值
        /// </summary>
        /// <param name="key">标识</param>
        public void PrintLastTime(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (!_singleMarkRecorders.ContainsKey(key))
                return;

            var recorders = _singleMarkRecorders[key];
            if (recorders.Count > 1)
            {
                var lasttwo = recorders.Skip(recorders.Count).Take(2);
                var timesp = lasttwo.Last() - lasttwo.First();
                _log?.Invoke($"{key} 耗时：{timesp}ms");
            }
        }

        public void Dispose()
        {
            if (sw.IsRunning)
                sw.Stop();
        }
    }
}

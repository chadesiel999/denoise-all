using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public class SysRunTimeMangager
    {
        private const Double UPDATE_INTERVAL_MINUTES = 5.0;

        public static SysRunTimeMangager Default { get; } = new SysRunTimeMangager();

        private DateTime _StartTime = DateTime.MinValue;
        private CancellationTokenSource _CancelTokenSource = new CancellationTokenSource();

        private readonly System.Threading.Timer? _RunTimer;
        private Stopwatch _Stopwatch;

        public String SystemRunTime { get; private set; } = String.Empty;

        public event EventHandler? RunTimeChanged;//= delegate { };

        private SysRunTimeMangager()
        {
            _Stopwatch = new Stopwatch();
            _RunTimer = new Timer(new TimerCallback(UpdateRunTime), null, Timeout.Infinite, 1000);
            _Stopwatch.Start();
            _RunTimer?.Change(0, 1000);
        }

        public void Run()
        {
            _StartTime = DateTime.Now;
            Task.Run(() =>
            {
                while (!_CancelTokenSource.Token.IsCancellationRequested)
                {
                    var runtime = DateTime.Now.Subtract(_StartTime).TotalMinutes;
                    runtime = runtime < 0 ? 0 : runtime;
                    if (runtime >= UPDATE_INTERVAL_MINUTES)
                    {
                        OptionsManager.Default.UpdateRemainingTime();
                        _StartTime = DateTime.Now;
                        if (OptionsManager.Default.GetRemainingTimeByHour() <= 0)
                        {
                            return;
                        }
                    }

                    Thread.Sleep(2000);
                }
            });
        }

        private void UpdateRunTime(Object? sender)
        {
            var timespan = _Stopwatch.Elapsed;
            SystemRunTime = $"{timespan.Hours.ToString("00")} : {timespan.Minutes.ToString("00")} : {timespan.Seconds.ToString("00")}";
            RunTimeChanged?.Invoke(this, new EventArgs());
        }

        public void Stop()
        {
            _CancelTokenSource?.Cancel();
            _RunTimer?.Change(Timeout.Infinite, 1000);
        }
    }
}

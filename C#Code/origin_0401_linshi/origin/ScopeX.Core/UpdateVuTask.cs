using NPOI.POIFS.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    public static class UpdateVuTask
    {
        private static CancellationTokenSource? _Cts;
        private static Task? _WorkTask;
        internal static Object PackLocker = new();
        public static readonly Object VuLocker = new();

        internal static volatile Boolean ClearFlag = false;
        public static Int64 HardCopyTimestamp = 0;
        internal static void Cancel()
        {
            _Cts?.Cancel();
            try
            {
                _WorkTask?.Wait();
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex.Message, EventBus.LogLevel.Error));
            }
            finally
            {
                _Cts = null;
                _WorkTask = null;
            }
        }

        internal static void Run()
        {
            Int64 slice = 0;
            var sw = Stopwatch.StartNew();
            List<Int64> elapse = new();
            _Cts = new CancellationTokenSource();
            var token = _Cts.Token;
            _WorkTask = new Task(() =>
            {
                try
                {
                    while (true)
                    {
                        if (sw.ElapsedMilliseconds > 5000)
                        {
                            sw.Restart();
                        }
                        elapse.Clear();

                        if(DsoModel.Default.PassFail.Active && DsoModel.Default.PassFail.Running 
                            &&DsoModel.Default.PassFail.HardCopy && HardCopyTimestamp < DsoModel.Default.PassFail.Results.FailedTimestamp)
                        {
                            Thread.Sleep(1);
                            continue;
                        }

                        slice = sw.ElapsedMilliseconds;
                        
                        if (Dispatcher.IsModelNewerThanWfm() || Dispatcher.IsWfmNewerThanDraw())
                        {
                            Acquisition.Default.UpdateVuSample(DsoModel.Default.AnalogChnls.Cast<ChannelModel>()
                                .Concat(DsoModel.Default.RadioFrequencyChnls)
                                .Concat(DsoModel.Default.DigitalChnls));
                        }
                        elapse.Add(sw.ElapsedMilliseconds - slice);
                        slice = sw.ElapsedMilliseconds;

                        Acquisition.Default.UpdateVuSample(DsoModel.Default.ReferenceChnls);

                        if(!DsoModel.Default.Timebase.IsScan)
                        {
                            if (DateTime.Now.Subtract(DsoModel.Default.Timebase.ScaleOrPosUpdateTime).TotalMilliseconds >= 250)
                            {
                                try
                                {
                                    DsoModel.Default.PassFail.Run();
                                }
                                catch (Exception ex)
                                {
                                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex.Message, EventBus.LogLevel.Debug));
                                }
                            }
                        }

                        if (ClearFlag)
                        {
                            lock (VuLocker)
                            {
                                var chnls = DsoModel.Default.AnalogChnls.Cast<ChannelModel>().Concat(DsoModel.Default.DigitalChnls);
                                foreach (var ch in chnls)
                                {
                                    ch.VuDatabase.Reset();
                                    ch.ZoomVuDatabase.Reset();
                                }
                            }
                            if (Dispatcher.ClearOver) ClearFlag = false;
                        }

                        elapse.Add(sw.ElapsedMilliseconds - slice);
                        if (elapse.Sum() < 10)
                        {
                            Thread.Sleep(10);
                        }
                        token.ThrowIfCancellationRequested();
                    }
                }
                catch (Exception e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"=====UpdateVuTask.Run task CANCELED (id = {Environment.CurrentManagedThreadId})! =====\n{e.Message}\n{e.StackTrace}", EventBus.LogLevel.Debug));
#if DEBUG
                    Trace.WriteLine($"=====UpdateVuTask.Run task CANCELED (id = {Environment.CurrentManagedThreadId})! =====");
#endif
                }
            }, token, TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);
            _WorkTask.Start();
        }
    }
}

using ScopeX.ComModel;
using ScopeX.Core.Decode;
using ScopeX.Core.Hardware;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using ScopeX.Updater.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    internal static class Dispatcher
    {
        private static CancellationTokenSource? _Cts;

        private static Task? _WorkTask;

        public static void Cancel()
        {
            MiscMonitor.Default.Cancel();

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
                _PwrAnalysisStarted = 0;
            }
        }
        private static Boolean bNeedDoStopAction = false;
        private static Boolean isExecuteStop = false;
        public static void Stop()
        {
            lock (_Locker)
            {
                bNeedDoStopAction = true;
            }
            //TriggerModel.State = SysState.Stop;
            //SoftReset();
        }
        private static Boolean _IsCali = false;
        public static void Resume()
        {
            if (TriggerPrsnt.Mode == TriggerMode.OneShot)
            {
                TriggerModel.Mode = TriggerMode.Auto;
            }
            TriggerModel.ResetState();
            SoftReset();
            if (!DsoPrsnt.DefaultDsoPrsnt.Measure.StopMeasure)
            {
                DsoModel.Default.Meas.Calc.ClearAllStat();
            }
            if (TriggerPrsnt.Type == TriggerType.Serial)
            {
                if (TriggerSerialShareParameter.Default.ProtocolType != SerialProtocolType.Close)
                {
                    Hardware.HdCmdFactory.Push(HdCmd.DecodeProtocal);
                }
            }
            else
            {
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
            //Hardware.HdCmdFactory.Push(HdCmd.OuterPannelLEDCtrl);
        }

        //public void Oneshot()
        //{
        //    TriggerModel.Mode = TriggerMode.OneShot;
        //    TriggerModel.State = SysState.Armed;
        //    SoftReset();
        //}

        //private static readonly Acquisition _Acquisition = Acquisition;

        private static Int64 _CmdUpdatedStamp;

        private static Int64 _WfmTakenStamp;

        private static Int64 _WfmDrawnStamp;

        private static void UpdateModelStamp()
        {
            Volatile.Write(ref _CmdUpdatedStamp, /*DateTime.Now.Ticks*/Stopwatch.GetTimestamp());
        }

        private static void UpdateWfmStamp()
        {
            Volatile.Write(ref _WfmTakenStamp, /*DateTime.Now.Ticks*/Stopwatch.GetTimestamp());
        }

        public static void UpdateDrawStamp()
        {
            Volatile.Write(ref _WfmDrawnStamp, /*DateTime.Now.Ticks*/Stopwatch.GetTimestamp());
        }

        internal static Boolean IsModelNewerThanWfm()
        {
            return _CmdUpdatedStamp >= Volatile.Read(ref _WfmTakenStamp);
        }

        public static Boolean IsModelNewerThanDraw()
        {
            return IsModelNewerThanWfm();
        }

        public static Boolean IsWfmNewerThanDraw()
        {
            return _WfmTakenStamp >= Volatile.Read(ref _WfmDrawnStamp);
        }

        public static volatile Int32 SoftResetFlag = 0;

        private static Boolean CheckAndClearSoftReset => Interlocked.CompareExchange(ref SoftResetFlag, 0, 1) > 0;

        private static Boolean _BNeedSoftReset = true;
        private static TriggerMode _NewTriggerMode = TriggerMode.Auto;
        private static Boolean _BNeedChangeTriggerMode = true;
        public static void SetNewTriggerMode(TriggerMode _newTriggerMode)
        {
            lock (_Locker)
            {
                _NewTriggerMode = _newTriggerMode;
                _BNeedChangeTriggerMode = true;
            }
        }
        private static void DoSoftReset()
        {
            if (IsScan && (TriggerModel.State != SysState.Stop))
            {
                bNeedDoClearAction = true;
            }
            SoftResetFlag = 1;
            UpdateModelStamp();
        }
        public static void SoftReset()
        {
            lock (_Locker)
            {
                _BNeedSoftReset = true;
            }
        }

        private volatile static Int32 _SegememtFlag = 0;
        internal static Boolean SegememtFlag => Interlocked.CompareExchange(ref _SegememtFlag, 0, 1) > 0;

        public static void NeedUpdateSegmentFlag()
        {
            _SegememtFlag = 1;
        }

        private volatile static Int32 _ClearFlag = 0;
        internal static Boolean ClearFlag => Interlocked.CompareExchange(ref _ClearFlag, 0, 1) > 0;
        /// <summary>
        /// 确保work线程已经将需要清除的数据清除后，再通知UpdateVu线程结束清除数据
        /// </summary>
        internal static volatile Boolean ClearOver = true;
        private static Boolean bNeedDoClearAction = true;
        private static void ClearAction()
        {
            UpdateVuTask.ClearFlag = true;
            SoftResetFlag = 1;
            ClearOver = false;
            _ClearFlag = 1;
            UpdateModelStamp();
        }
        public static void DoClear()
        {
            lock (_Locker)
            {
                bNeedDoClearAction = true;
            }
        }

        private static Int32 _DataSourceChanged = 0;

        private static Boolean DataSourceChanged => Interlocked.CompareExchange(ref _DataSourceChanged, 0, 1) > 0;

        public static void OnDataSourceChanged()
        {
            _DataSourceChanged = 1;
        }

        internal static Boolean IsRunning = false;

        public static Boolean IsScan = false;
        internal static Boolean GetScanState()
        {
            return DsoModel.Default.Timebase.ScaleIndex >= DsoModel.Default.Timebase.ScanMinIndex
            //DsoModel.Default.Timebase.IsScan
            && TriggerModel.Mode == TriggerMode.Auto
            //&& DsoModel.Default.Timebase.Mode < AnaChnlAcqMode.Average
            //&& DsoModel.Default.Timebase.StorageMode != AnaChnlStorageMode.Fast
            //&& !DsoModel.Default.GetChannel(ChannelId.D0).Active
#if ScopeX_DIST
            && !DsoModel.Default.MathChnls.Any(o => o.Active)
#endif
            && !DsoModel.Default.DecodeChnls.Any(o => o.Active);
        }


        public static Boolean Open(String deviceInfo, ProductType pt, DataSourceOpt ds, Action<String, String>? sysLogger, Action<Int32>? errorMsgbox)
        {
            Boolean result = false;
            DsoModel.DataSrcOpt = ds;

            result = Hd.Open(deviceInfo, Constants.BOARD_ATTACHED, false);
            if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
            {
                if (!Hd.Initialize(msg, sysLogger, errorMsgbox))
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs("Cannot open PCIE", EventBus.LogLevel.Error));
                    result = false;
                    //Thread.Sleep(1000);
                }
                //根据实际硬件信息再执行一次
                AdcInterleaveProcessor.Default.Process();

                var tempinfo = Hd.GetParamters("System", "TempInfo", null);
                if (tempinfo != null && tempinfo is Dictionary<String, Double>)
                {
                    DsoModel.Default.TempCtrl.UpdateTemp((Dictionary<String, Double>)tempinfo);
                }
                DsoModel.Default.TempCtrl.UpdateCaliTemp();
                DsoModel.Default.ArtificialIntelligence.InitAnaChnlBitWidthDefine();
            }
            DsoModel.Default.ArtificialIntelligence.UpdateSubbandTable();
            DsoModel.Default.MultiDomain.UpdateSpanListForTimeFreq();

            var fansname = Hd.GetParamters("System", "FansName", null);
            if (fansname != null && fansname is String[])
            {
                DsoModel.Default.TempCtrl.InitFansName((String[])fansname);
            }

            if (Constants.BOARD_ATTACHED)
            {
                //读取产品信息
                ReadDsoInfo();
            }
            if (OptionsManager.Default.GetRemainingTimeByHour() > 0)
            {
                SysRunTimeMangager.Default.Run();//选件试用时间大于0时，才运行
            }
            return result;
        }

        private static void ReadDsoInfo()
        {
            DsoModel.Default.DsoInfoBackup = new();

            //从Flash加载信息
            DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncOperation = FalshOpreation.Read;
            DsoModel.Default.DsoInfoBackup.ProductInfos.SyncOperation = FalshOpreation.Read;
            Hd.UpdateDsoInfo(DsoModel.Default.DsoInfoBackup);
            if (DsoModel.Default.DsoInfoBackup.ProductInfos.SyncResult == FalshOpreationResult.Success)
            {
                DsoModel.Default.DsoInfoBackup.ProductInfos.CloneTo(DsoModel.Default.DsoInfo.ProductInfos);
                DsoModel.Default.DsoInfoBackup.ProductInfos.SyncResult = FalshOpreationResult.None;
                if (Constants.ENABLE_USB)
                {
                    Hd.WriteUSBTMCSN(OptionsManager.Default.SerialNumber);
                }
                if (Constants.ENABLE_AWG)
                {
                    AWGCailbration.InitCalibSN(OptionsManager.Default.SerialNumber);
                }
            }
            else
            {
                DsoModel.Default.DsoInfo.ProductInfos.CloneTo(DsoModel.Default.DsoInfoBackup.ProductInfos);
                DsoModel.Default.DsoInfoBackup.ProductInfos.SyncResult = FalshOpreationResult.None;
            }

            if (DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncResult == FalshOpreationResult.Success)
            {
                DsoModel.Default.DsoInfoBackup.OptionsInfos.CloneTo(DsoModel.Default.DsoInfo.OptionsInfos);
                DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncResult = FalshOpreationResult.None;
            }
            else
            {
                DsoModel.Default.DsoInfo.OptionsInfos.CloneTo(DsoModel.Default.DsoInfoBackup.OptionsInfos);
                DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncResult = FalshOpreationResult.None;
            }

            //初始化选件激活码
            DsoPrsnt.DefaultDsoPrsnt.OptionsManager.InitAllOption();

            //更新剩余时间
            DsoModel.Default.DsoInfoBackup.OptionsInfos.TrialRemainingTimeByHour -= DsoPrsnt.DefaultDsoPrsnt.OptionsManager.CumulativeUseTime;
            DsoModel.Default.DsoInfoBackup.ProductInfos.SyncOperation = FalshOpreation.None;
            DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncOperation = FalshOpreation.Write;

            //再次从Flash加载信息
            Hd.UpdateDsoInfo(DsoModel.Default.DsoInfoBackup);
            if (DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncResult == FalshOpreationResult.Success)
            {
                DsoModel.Default.DsoInfoBackup.OptionsInfos.CloneTo(DsoModel.Default.DsoInfo.OptionsInfos);
                DsoPrsnt.DefaultDsoPrsnt.OptionsManager.CumulativeUseTime = 0;
                DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncResult = FalshOpreationResult.None;
            }
        }

        public static void Close()
        {
            SysRunTimeMangager.Default?.Stop();
            MiscMonitor.Default.Close();
            if (!Constants.ENABLE_DEBUG)
            {
                HardwarePowerOff();
            }
            Hd.Close();
        }
        private static Object _Locker = new Object();
        public static void HardwarePowerOff()
        {
            Hd.PowerDown();
        }
        public static TimeSpan _ScanInfinityClearDateTime = TimeSpanUtility.GetTimestampSpan();
        public static void Run()
        {
            _Cts = new CancellationTokenSource();
            TriggerModel.ResetState();
            bNeedDoClearAction = true;
            _BNeedSoftReset = true;

            MiscMonitor.Default.EnableAWGPrtected = Constants.ENABLE_AWG_PROTECT;

            MiscMonitor.Default.Run();
            Boolean bHdCmdChannged = true;
            var token = _Cts.Token;
            _CalcStarted = 0;
            _PwrAnalysisStarted = 0;
            DateTime datetime = DateTime.Now;
            _WorkTask = new Task(() =>
            {
                try
                {
                    while (true)
                    {
                        datetime = DateTime.Now;
                        Boolean bneedchangetriggermodetemp = false;
                        lock (_Locker)
                        {
                            bneedchangetriggermodetemp = _BNeedChangeTriggerMode;
                            if (_BNeedChangeTriggerMode)
                            {
                                TriggerModel.Mode = _NewTriggerMode;

                                _BNeedSoftReset = true;
                                Hardware.HdCmdFactory.Push(HdCmd.TrigMode);

                                _BNeedChangeTriggerMode = false;
                            }
                            if (bNeedDoStopAction)
                            {
                                TriggerModel.State = SysState.Stop;
                                _BNeedSoftReset = true;
                                bNeedDoStopAction = false;
                            }
                            IsRunning = TriggerModel.State != SysState.Stop;
                            if (IsRunning)
                            {
                                DsoModel.Default.Timebase.CallBack = false;
                                IsScan = GetScanState();
                                DsoModel.Default.Timebase.IsScan = IsScan;
                                DsoPrsnt.DefaultDsoPrsnt.RemoveDemo();
                                isExecuteStop = false;
                            }
                            else
                            {
                                isExecuteStop = true;
                            }

                            if (_BNeedSoftReset)
                            {
                                DoSoftReset();
                                _BNeedSoftReset = false;
                            }
                            if (bNeedDoClearAction)
                            {
                                ClearAction();
                                bNeedDoClearAction = false;
                            }
                            if (DataSourceChanged)
                            {
                                Acquisition.Default.BindDataSource(DsoModel.DataSrcOpt);
                            }
                        }

                        ////Send hardware commmand
                        //if (_IsCali)
                        //{
                        //    WeakTip.Default.Write("AutoCaliAtInit", $"自校准完成", emergent: false, "", 5);
                        //    _IsCali = false;
                        //}

                        UInt64 command = HdCmdFactory.Command;
                        if (HdMsgFactory.TryMake(command, out var msg))
                        {
                            Hd.Execute(msg!);
                            Acquisition.Default.UpdateReadInfoList();
                            bHdCmdChannged = msg!.Command != (Int64)HdCmd.Run;

                            //if (((command & (long)HdCmd.TmbScaleIndex) != 0 || (command & (long)HdCmd.TmbPosition) != 0) &&
                            //   DsoModel.Default.TempCtrl.GetNeedCali() && IsRunning && DsoModel.Default.Timebase.StorageDepthOpt == 0)
                            //{
                            //    if (DsoModel.Default.TempCtrl.AutoCaliSystem)
                            //    {
                            //        WeakTip.Default.Write("AutoCaliAtInit", $"温度变化较大，开启自校准", emergent: false, "", 60);
                            //        Hd.MiscFunc("UserAutoCali", $"DBI_AtInit_Tiadc_{DsoPrsnt.FocusId}");
                            //        DsoModel.Default.TempCtrl.UpdateCaliTemp();
                            //        _IsCali = true;
                            //    }
                            //    else
                            //    {
                            //        WeakTip.Default.Write("AutoCaliAtInit", $"温度变化较大，建议使用自校准功能", emergent: false, "", 15);
                            //    }
                            //}
                        }
                        else
                        {
                            bHdCmdChannged = false;
                            if (DsoModel.Default.Timebase.ZoomChanged)
                            {
                                Acquisition.Default.UpdateReadInfoList();
                                bHdCmdChannged = true;
                            }
                        }

                        var modelupated = CheckAndClearSoftReset;
                        if (modelupated)
                        {
                            DsoModel.Default.SoftResetCount++;
                            if (IsScan && DsoModel.Default.Display.Persist == WfmPersist.Infinity)
                            {
                                //_ScanInfinityClearDateTime = DateTime.Now;
                                _ScanInfinityClearDateTime = TimeSpanUtility.GetTimestampSpan();
                            }
                            //DsoModel.Default.JitterModel.IsNeedResetOHistoryValue = true;
                        }
                        if (DsoModel.Default.Display.Persist == WfmPersist.Infinity &&
                            (DateTime.Now.Subtract(DsoModel.Default.Timebase.ScaleOrPosUpdateTime).TotalMilliseconds < 250
                            || (TimeSpanUtility.GetTimestampSpan() - _ScanInfinityClearDateTime).TotalMilliseconds < 250))
                        {
                            DsoModel.Default.SoftResetCount++;
                        }
                        // var modelupated = IsModelNewerThanWfm();
                        //Acquire hardware samples
                        Boolean wfmupdated = false;
                        Boolean clearflag = ClearFlag;
                        //Boolean laswitchflag = (command & (UInt64)HdCmd.LASwitch) != 0;//????

                        Boolean chnlchanged = ((command & ((UInt64)HdCmd.ChnlScaleIndex)) != 0 ||
                                               (command & ((UInt64)HdCmd.ChnlInverted)) != 0 ||
                                               (command & ((UInt64)HdCmd.ChnlPosition)) != 0) && TriggerModel.State != SysState.Stop;

                        // Bug：3563 修改时间：2024年6月24日，原因：Auto切换到Normal时，没有及时停止下来，触发模式改变时，需要将读取重置一下，以前没有将重置下发给读取波形，故而读取的波形错误了。
                        //Boolean reset = clearflag || chnlchanged; 
                        Boolean reset = clearflag || chnlchanged || bneedchangetriggermodetemp /*|| laswitchflag*/;//????

                        if (chnlchanged)
                        {
                            Thread.Sleep(120);
                        }

                        if (reset)
                        {
                            DsoModel.Default.Meas.Calc.ClearAllStat();
                        }

                        wfmupdated = Acquisition.Default.ReadWfm(bHdCmdChannged, reset, token, null);

                        if (wfmupdated)
                            ExportHdFuncs.ProcessScpiSpecial();
                        //if (SoftResetFlag != 0)
                        //    continue;

                        if (DsoModel.Default.Timebase.SegmentActive && !SegememtFlag)
                        {
                            DsoModel.Default.Timebase.SegmentUpdate();
                        }
                        if (wfmupdated || modelupated || bHdCmdChannged)
                        {
                            lock (Acquisition.Locker)
                            {
                                Acquisition.Default.AssignAnalogWfm(modelupated || bHdCmdChannged || reset, token, null);
                                Acquisition.Default.AssignRadioFrequencyWfm(modelupated, token);
                                Acquisition.Default.AssignDigitalWfm(modelupated, token);

                                Acquisition.Default.AssignDecodeWfm(modelupated, token);
                                Acquisition.Default.AssignSearchResult(modelupated, token);

                                DsoModel.Default.MultiDomain.UpdateWfm();
                                //if (!IsRunning) Acquisition.UpdateDecodePacket();
                            }
                            if (wfmupdated)
                            {
                                Acquisition.Default.UpdateAnalogWfmTime();
                            }
                            //if (SoftResetFlag != 0)
                            //    continue;
                        }
                        else
                        {
                            Acquisition.Default.AssignSearchResult(modelupated, token);
                        }

                        if (DecodeProtocolShareParameter.Default.NeedReadDecodeData)
                        {
                            DecodeProtocolShareParameter.Default.ClearNeedReadData();
                        }

                        if (clearflag)
                        {
                            Acquisition.Default.ClearAnalogBuffer();
                            Acquisition.Default.ClearDecodeData();
                            DsoModel.Default.Voltmeter.StaBuffer.Clear();
                            DsoModel.Default.Cymometer.StaBuffer.Clear();
                        }

                        Acquisition.Default.CopyToPackLock(DsoModel.Default.AnalogChnls.Cast<ChannelModel>()
                            .Concat(DsoModel.Default.MathChnls)
                            .Concat(DsoModel.Default.ReferenceChnls)
                            .Concat(DsoModel.Default.DigitalChnls));

                        UpdateWfmStamp();

                        if (clearflag) ClearOver = true;

                        _ = CalcAsync(wfmupdated, modelupated, token);
                        //_ = CalcPwrAnalysisAsync(wfmupdated, token);
                        CalcPwrAnalysis(wfmupdated, token);
                        //TrigAssisted(wfmupdated);
                        if (!IsScan)
                        {
                            _ = JitterAnalysisModel();
                            _ = ExceptionCaptureModel(); // 异常捕获
                            _ = VsaAnalysisModel();
                        	_ = AiModel();
                            AnalyzeWfm(wfmupdated);
                        }

                        UpdateDsoInfos();
                        UpdateCaliData();
                        Hd.ExecuteCmdDelayedDmaTask();
                        token.ThrowIfCancellationRequested();
                        if (DateTime.Now.Subtract(datetime).TotalMilliseconds < 10)
                        {
                            Thread.Sleep(1);
                        }
                        if (TriggerModel.State == SysState.Stop && msg == null)
                            Thread.Sleep(50);
                    }
                }
                catch (OperationCanceledException oce)
                {
                    String msg = $"=====Dispatcher.Default.Run task CANCELED (id = {Environment.CurrentManagedThreadId})! =====\r\n" +
                        $"OperationCanceledException Message: {oce.Message} \r\n" +
                        $"OperationCanceledException StackTrace: {oce.StackTrace}";
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(msg, EventBus.LogLevel.Debug));
                    _CalcStarted = 0;
                    _PwrAnalysisStarted = 0;
#if DEBUG
                    Trace.WriteLine(msg);
#endif
                    //Hd.Close();
                }
                catch (AggregateException ae)
                {
                    String msg = $"=====Dispatcher.Default.Run task CANCELED (id = {Environment.CurrentManagedThreadId})! =====\r\n" +
                        $"AggregateException Message: {ae.Message} \r\n" +
                        $"AggregateException StackTrace: {ae.StackTrace}";
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(msg, EventBus.LogLevel.Debug));
#if DEBUG
                    Trace.WriteLine(msg);
#endif
                }
                catch (Exception ex)
                {
                    String msg = $"=====Dispatcher.Default.Run task CANCELED (id = {Environment.CurrentManagedThreadId})! =====\r\n" +
                        $"Exception Message: {ex.Message} \r\n" +
                        $"Exception StackTrace: {ex.StackTrace}";
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(msg, EventBus.LogLevel.Error));
                }
            }, _Cts.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);

            _WorkTask.Start();

        }

        private static volatile Int32 _CalcStarted = 0;

        private static volatile Int32 _PwrAnalysisStarted = 0;

        private static async Task CalcAsync(Boolean wfmTaken, Boolean modelUpdated, CancellationToken token)
        {
            if (_CalcStarted > 0)
            {
                return;
            }

            _CalcStarted++;
            var res = await Calc(wfmTaken, modelUpdated, token);
            //if (_CalcStarted > 0)
            //    _CalcStarted--;

            if (Constants.ENABLE_Math && res)
            {
                //Acquisition.UpdateVuSample(DsoModel.Default.MathChnls);
                Acquisition.UpdateMathVuSample(DsoModel.Default.MathChnls);
            }

            //if (_PwrAnalysisStarted > 0)
            //{
            //    return;
            //}
            //_PwrAnalysisStarted++;
            //if (Constants.ENABLE_PowerAs)
            //    await AnalyzePwrAsync(wfmTaken, token);
            //if (_PwrAnalysisStarted > 0)
            //    _PwrAnalysisStarted--;
        }

        private static void CalcPwrAnalysis(Boolean wfmTaken, CancellationToken token)
        {
            if (!Constants.ENABLE_PowerAs)
            {
                return;
            }
            if (_PwrAnalysisStarted > 0)
            {
                return;
            }
            _PwrAnalysisStarted++;
            AnalyzePwr(wfmTaken, token);
        }

        private static async Task CalcPwrAnalysisAsync(Boolean wfmTaken, CancellationToken token)
        {
            if (!Constants.ENABLE_PowerAs)
            {
                return;
            }
            if (_PwrAnalysisStarted > 0)
            {
                return;
            }
            _PwrAnalysisStarted++;
            await AnalyzePwrAsync(wfmTaken, token);
            _PwrAnalysisStarted--;
        }
        private static Task<Boolean> Calc(Boolean wfmTaken, Boolean modelUpdated, CancellationToken token)
        {
            return Task.Run(() =>
            {
                try
                {
#if DEBUG
                    //Logger.Debug($"=====+++++=====Async Calc task STARTING (id = {Thread.CurrentThread.ManagedThreadId}) {_CalcStarted}!"); 
                    Stopwatch sw = Stopwatch.StartNew();
                    Int64 t = sw.ElapsedMilliseconds;
#endif

                    Boolean finish;
                    var stamp = DateTime.Now.Ticks;

                    if (Constants.ENABLE_Math)
                        Acquisition.Default.PrepareMathSrcWfm();

                    if (Constants.ENABLE_Measure)
                        DsoModel.Default.Meas.Calc.Run(wfmTaken);

                    if (Constants.ENABLE_Math)
                        DsoModel.Default.Markers.Run();


                    #region 电压表/频率计
                    if (Constants.ENABLE_VOLTMETER)
                        DsoPrsnt.DefaultDsoPrsnt.Voltmeter.Run();

                    if (Constants.ENABLE_CYMOMETER)
                        DsoPrsnt.DefaultDsoPrsnt.Cymometer.Run();


                    #endregion

                    if (Constants.ENABLE_Math)
                        finish = Acquisition.Default.CalcMathWfm(wfmTaken, modelUpdated, stamp, token);//math

                    token.ThrowIfCancellationRequested();

                }
                catch (Exception e)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"=====+++++=====Async Calc task CANCELED (id = {Thread.CurrentThread.ManagedThreadId})!\n{e.Message}\n{e.StackTrace}", EventBus.LogLevel.Debug));
#if DEBUG
                    Trace.WriteLine($"=====+++++=====Async Calc task CANCELED (id = {Thread.CurrentThread.ManagedThreadId})!");
#endif

                }
                finally
                {
                    if (_CalcStarted > 0)
                        _CalcStarted--;
                }

                return true;
            }, token);

        }

        private static void AnalyzePwr(Boolean wfmTaken, CancellationToken token)
        {
            try
            {

                var powermodel = DsoModel.Default.PowerAnalysisModels.Where(x => x.Active);
                foreach (var item in powermodel)
                {
                    if (item.Mode == PowerAnalysis.PowerAnalysisOpt.SafeOperationArea)//安全工作区临时修改
                    {
                        if (bNeedDoStopAction || TriggerModel.State == SysState.Stop)
                        {
                            continue;
                        }
                    }

                    item.Run(wfmTaken);
                }
            }
            catch (OperationCanceledException)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"=====+++++=====Async AnalyzePwr task CANCELED (id = {Thread.CurrentThread.ManagedThreadId})!", EventBus.LogLevel.Debug));
            }
            finally
            {
                if (_PwrAnalysisStarted > 0)
                    _PwrAnalysisStarted--;
            }
        }

        private static Task AnalyzePwrAsync(Boolean wfmTaken, CancellationToken token)
        {
            //方案需要更改，多个电源分析目前是串行
            return Task.Run(() =>
            {
                try
                {
                    var powermodel = DsoModel.Default.PowerAnalysisModels;
                    foreach (var item in powermodel)
                    {
                        if (item.Mode == PowerAnalysis.PowerAnalysisOpt.SafeOperationArea)//安全工作区临时修改
                        {
                            if (isExecuteStop)
                            {
                                continue;
                            }
                        }

                        item.Run(wfmTaken);
                    }
                    token.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs($"=====+++++=====Async AnalyzePwr task CANCELED (id = {Thread.CurrentThread.ManagedThreadId})!", EventBus.LogLevel.Debug));
                }
                finally
                {
                    if (_PwrAnalysisStarted > 0)
                        _PwrAnalysisStarted--;
                }

            }, token);
        }

        private static volatile Int32 _JitterAnalysiStarted = 0;
        private static async Task JitterAnalysisModel()
        {
            if (!Constants.ENABLE_SDA)
                return;
            if (_JitterAnalysiStarted > 0)
            {
                return;
            }

            if (!DsoModel.Default.JitterModel.Active)
            {
                DsoModel.Default.JitterModel.Dispose();
                return;
            }

            _JitterAnalysiStarted++;
            //await Task.Run(() => DsoModel.Default.SerialAnalysisModel.Run());
            //await Task.Run(() => Jitter.Default.Run());
            try
            {

                await Task.Run(() => DsoModel.Default.JitterModel.Run());
            }
            catch (Exception)
            {

            }
            _JitterAnalysiStarted--;
        }

        private static volatile Int32 _ExceptionCaptureStarted = 0;
        private static async Task ExceptionCaptureModel()
        {
            if (_ExceptionCaptureStarted > 0)
                return;

            _ExceptionCaptureStarted++;
            await Task.Run(() => DsoModel.Default.ExceptionCapture.Run());
            //try
            //{
            //    await Task.Run(() => DsoModel.Default.IntelligentChartManager.Run());
            //}
            //catch (Exception)
            //{
            //}

            _ExceptionCaptureStarted--;
        }

        private static volatile Int32 _VsaAnalysiStarted = 0;
        private static async Task VsaAnalysisModel()
        {
            if (_VsaAnalysiStarted > 0)
            {
                return;
            }

            if (!DsoModel.Default.VectorAnalysisModel.Enabled)
            {
                return;
            }

            _VsaAnalysiStarted++;
            await Task.Run(() => DsoModel.Default.VectorAnalysisModel.Run());
            _VsaAnalysiStarted--;
        }

        private static volatile Int32 _AiStarted = 0;
        private static async Task AiModel()
        {
            if (_AiStarted > 0)
                return;

            _AiStarted++;
            await Task.Run(() => DsoModel.Default.ArtificialIntelligence.Run());
            //try
            //{
            //    await Task.Run(() => DsoModel.Default.IntelligentChartManager.Run());
            //}
            //catch (Exception)
            //{
            //}



            _AiStarted--;
        }

        //功率分析 //PF测试 //波形搜索
        private static void AnalyzeWfm(Boolean wfmTaken)
        {
            //DsoModel.Default.PassFail.Run(wfmTaken);
            //DsoModel.Default.WfmInspector.Run(wfmTaken);
        }

        private static void TrigAssisted(Boolean wfmTaken)
        {
            VisualTrig(wfmTaken);
            MeasureTrig();
        }
        private static void VisualTrig(Boolean wfmTaken)
        {
            if (DsoModel.Default.VisualTrigger.Enabled && DsoModel.Default.TriggerAssisted.Enabled)
                DsoModel.Default.VisualTrigger.Run(wfmTaken);
        }

        private static void MeasureTrig()
        {
            if (DsoModel.Default.LocAssisted.Enabled && DsoModel.Default.TriggerAssisted.Enabled)
            {
                var index = DsoModel.Default.LocAssisted.Locate();
                if (index >= 0)
                {
                    foreach (var ach in DsoModel.Default.AnalogChnls)
                    {
                        if (ach.Pack is not null)
                            ach.Pack.Properties.VuStartIndex = -index + ach.Pack.Properties.TmbPosition.Index;
                    }
                }
            }
        }

        private volatile static Int32 _DsoInfoUpdateFlag = 0;
        internal static Boolean DsoInfoUpdateFlag => Interlocked.CompareExchange(ref _DsoInfoUpdateFlag, 0, 1) > 0;

        public static void NeedUpdateDsoInfo()
        {
            _DsoInfoUpdateFlag = 1;
        }
        private static void UpdateDsoInfos()
        {
            if (!DsoInfoUpdateFlag)
                return;
            var storagemode = DsoModel.Default.Timebase.StorageMode;
            if (storagemode == AnaChnlStorageMode.Fast)
            {
                DsoModel.Default.Timebase.StorageMode = AnaChnlStorageMode.Long;
                UInt64 command = HdCmdFactory.Command;
                if (HdMsgFactory.TryMake(command, out var msg))
                {
                    Hd.Execute(msg!);
                }
            }

            OptionsManager.Default.UpdateCompleteFlag = false;
            Hd.UpdateDsoInfo(DsoModel.Default.DsoInfoBackup);
            if (DsoModel.Default.DsoInfoBackup.ProductInfos.SyncResult == FalshOpreationResult.Success)
            {
                DsoModel.Default.DsoInfoBackup.ProductInfos.CloneTo(DsoModel.Default.DsoInfo.ProductInfos);
                if (Constants.ENABLE_USB)
                {
                    Hd.WriteUSBTMCSN(OptionsManager.Default.SerialNumber);
                }
                if (Constants.ENABLE_AWG)
                {
                    AWGCailbration.InitCalibSN(OptionsManager.Default.SerialNumber);
                }
            }

            if (DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncResult == FalshOpreationResult.Success)
            {
                DsoModel.Default.DsoInfoBackup.OptionsInfos.CloneTo(DsoModel.Default.DsoInfo.OptionsInfos);
                DsoModel.Default.DsoInfoBackup.OptionsInfos.SyncResult = FalshOpreationResult.None;

                if (DsoPrsnt.DefaultDsoPrsnt.OptionsManager.IsActiveOption == 2)
                    WeakTip.Default.Write(nameof(OptionsManager), MsgTipId.OptionActiveSuccess, duration: 5);
                else if (DsoPrsnt.DefaultDsoPrsnt.OptionsManager.IsActiveOption == 1)
                    WeakTip.Default.Write(nameof(OptionsManager), MsgTipId.LicenseRemoveSuccess, duration: 5);
                DsoPrsnt.DefaultDsoPrsnt.OptionsManager.IsActiveOption = 0;
            }

            if (DsoModel.Default.DsoInfoBackup.ProductInfos.SyncResult == FalshOpreationResult.Success)
            {
                DsoModel.Default.DsoInfoBackup.ProductInfos.SyncResult = FalshOpreationResult.None;
                OptionsManager.Default.InitDsoInfo();
            }

            OptionsManager.Default.UpdateCompleteFlag = true;

            if (OptionsManager.Default.IsAllActive())
            {
                if (!OptionsManager.Default.AllActiveUpdateStatus)
                {
                    OptionsManager.Default.AllActiveUpdateStatus = true;
                    OptionsManager.Default.ResetRemainingTime(true);
                    NeedUpdateDsoInfo();
                }
            }
            else
            {
                OptionsManager.Default.AllActiveUpdateStatus = false;
            }
        }

        private static void UpdateCaliData()
        {
            Hd.UpdateFlashCaliData(ExportHdFuncs.FlashCaliType);
        }
    }
}

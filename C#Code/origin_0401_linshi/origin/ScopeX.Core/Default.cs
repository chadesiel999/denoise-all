using EventBus;
using ScopeX.ComModel;
using ScopeX.Controls.Common.Structs;
using ScopeX.Core.Tools;
using System;
using System.Linq;
using System.Threading;

namespace ScopeX.Core
{
    internal class Default
    {
        private DsoPrsnt _DsoPrsnt = null;
        public Default(DsoPrsnt dso)
        {
            _DsoPrsnt = dso;
        }
        public void SetDefault()
        {
            if (_DsoPrsnt == null)
                return;
            TimebaseDefault();
            AcqDefault();
            TriggerDefault();
            CursorDefault();
            PrintDefault();
            FileDefault();
            SettingDefault();
            //PassFailDefault();
            //JitterDefault();
            //VoltmeterDefault();
            //CymometerDefault();
            MeasureDefault();
            AnalogDefault();
            //ReferenceDefault();
            //DigitalDefault();
            //DecodeDefault();
            //AWGDefault();
            MathDefault();
            DisplayDefault();
            //PwrAnalysisDefault();
            //SearchDefault();
            OtherDefault();
        }
        private void OtherDefault()
        {
            try
            {

                foreach (var dp in _DsoPrsnt.TryGetRange(c => !c.Id.IsAnalog()))
                {
                    dp.Active = false;
                }
                _DsoPrsnt.ManufacturerAdatper = ManufacturerAdatper.Default;
                _DsoPrsnt.MoveFocusId();
                _DsoPrsnt.SoftReset();
                _DsoPrsnt.DoClear();
                _DsoPrsnt.Resume();
                _DsoPrsnt.SystemCheckDefault();
                _DsoPrsnt.View?.UpdateView(this, "Default");

                foreach (var item in _DsoPrsnt.GetAllChnls())
                {
                    item.DrawColor = ColorLookup.Default[item.Id.ToString()];
                }

            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Warn));
            }
        }

        private void PwrAnalysisDefault()
        {
            foreach (var item in _DsoPrsnt.PwrAnalysisDictionary)
            {
                item.Value.Active = false;
            }
        }

        private void DisplayDefault()
        {
            _DsoPrsnt.Display.AxisTickVisible = true;
            _DsoPrsnt.Display.Persist = WfmPersist.Close;
            _DsoPrsnt.Display.YAxisTickRight = true;
            _DsoPrsnt.Display.XAxisTickBottom = true;
            _DsoPrsnt.Display.DrawMode = WfmDrawMode.Vector;
            _DsoPrsnt.Display.GridStyle = GridType.Full;
            _DsoPrsnt.Display.GridIntensity = Constants.DEF_GRID_INTENSITY;
            _DsoPrsnt.Display.WfmIntensity = Constants.DEF_WAVE_INTENSITY;
            if (PlatformManager.Default.Platform.EnableGetOrSetScreenBrightness)
            {
                _DsoPrsnt.Display.ScreenBrightness = 90;
            }
            _DsoPrsnt.Display.ScreenContrast = 90;
        }

        private void MathDefault()
        {
            foreach (var dp in _DsoPrsnt.TryGetRange(c => c.Id.IsMath()))
            {
                if (dp.Active)
                {
                    dp.Active = false;
                    Thread.Sleep(50);
                }
                try
                {
                    if (((MathPrsnt)dp).GetOrMakeArg(MathType.Binary) is MathBinaryArg math)
                    {
                        math.Source1st = ChannelId.C1;
                        math.Source2nd = ChannelId.C2;
                        math.BinaryOp = MathBinaryType.Add;
                    }
                    var ch = DsoModel.Default.GetChannel(dp.Id);
                    ((MathPrsnt)dp).Scale = ch.Conditioning.MinScale;
                    ((MathPrsnt)dp).PosIndexBymDiv = 0;
                    ((MathPrsnt)dp).Label = "";
                    ((MathPrsnt)dp).Unit = "V";
                }
                catch (Exception ex)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex.Message, LogLevel.Warn));
                }
            }
        }

        private void AWGDefault()
        {
            foreach (var item in _DsoPrsnt.ArbWfmGens)
            {
                item.Active = false;
                item.Frequency = 1_000_000_000;
                item.Amplitude = 100;
                item.Mode = WfmGenMode.Continuous;
                item.WfmType = ArbWfmType.Sinusoid;
                item.Impedance = WfmGenImpedance.HighZ;
            }
        }

        private void DecodeDefault()
        {
            foreach (var dp in _DsoPrsnt.TryGetRange(c => c.Id.IsDecode()))
            {
                if (dp.Active)
                {
                    dp.Active = false;
                    Thread.Sleep(50);
                }
            }
        }

        private void DigitalDefault()
        {
            foreach (var dp in _DsoPrsnt.TryGetRange(c => c.Id.IsDigital()))
            {
                if (dp.Active)
                {
                    dp.Active = false;
                    Thread.Sleep(50);
                }
                for (Int32 i = 0; i < 4; i++)
                {
                    ((DigitalPrsnt)dp).SetActiveAt(i, false);
                }

                ((DigitalPrsnt)dp).FocusBitId = 0;
                Int32 num = ((DigitalPrsnt)dp).BitLength;
                for (Int32 i = 0; i < num; i++)
                {
                    ((DigitalPrsnt)dp).SetLabelAt(i, "");
                }
                ((DigitalPrsnt)dp).BitHeightOpt = DigiHeightOpt.Small;
            }
        }

        private void ReferenceDefault()
        {
            foreach (var dp in _DsoPrsnt.TryGetRange(c => c.Id.IsReference()))
            {
                dp.Active = false;
            }
        }

        private void AnalogDefault()
        {
            foreach (var dp in _DsoPrsnt.TryGetRange(c => c.Id.IsAnalog()))
            {
                dp.Active = dp.Id == ChannelId.C1;

                ((AnalogPrsnt)dp).IsInverted = false;

                ((AnalogPrsnt)dp).PosIndexBymDiv = 0;
                ((AnalogPrsnt)dp).Bias = 0;
                if (((AnalogPrsnt)dp).ProbeConnected == false)
                {
                    ((AnalogPrsnt)dp).ProbeIndex = AnaChnlProbe.x1;
                    ((AnalogPrsnt)dp).ProbeGain = 1;
                    ((AnalogPrsnt)dp).ProbeUnitRatio = 1;
                    ((AnalogPrsnt)dp).ProbeGainCaliRatio = 1;

                    ((AnalogPrsnt)dp).Coupling = AnaChnlCoupling.DC1M;
                    ((AnalogPrsnt)dp).Bandwidth = ((AnalogPrsnt)dp).BandWidthNames.First().Index;
                    ((AnalogPrsnt)dp).ScaleBymV = 200;
                    ((AnalogPrsnt)dp).Ylevel_SelectStatus = false;
                }
                ((AnalogPrsnt)dp).Unit = "V";
                ((AnalogPrsnt)dp).Label = "";
                ((AnalogPrsnt)dp).AttenuationType = AttenuationType.Decibel;
                ((AnalogPrsnt)dp).ProbeBtnType = ProbeKeyType.Headlight;
            }
        }

        private void MeasureDefault()
        {
            _DsoPrsnt.Measure.SnapshotActive = false;
            _DsoPrsnt.Measure.SnapshotSource = ChannelId.C1;
            _DsoPrsnt.Measure.Active = false;
            _DsoPrsnt.Measure.Strobe = MeasureGate.Screen;
            _DsoPrsnt.Measure.IsStatActive = false;
        }

        private void CymometerDefault()
        {
            _DsoPrsnt.Cymometer.Active = false;
            _DsoPrsnt.Cymometer.ShowPeriod = false;
        }

        private void VoltmeterDefault()
        {
            _DsoPrsnt.Voltmeter.Active = false;
            _DsoPrsnt.Voltmeter.Source = ChannelId.C1;
            _DsoPrsnt.Voltmeter.AutoRange = false;
            _DsoPrsnt.Voltmeter.Mode = VoltmeterMode.DC;
        }

        private void JitterDefault()
        {
            _DsoPrsnt.Jitter.Active = false;
            _DsoPrsnt.Jitter.Source = ChannelId.C1;
            _DsoPrsnt.Jitter.SignalType = JitterSignalType.Custom;
            _DsoPrsnt.Jitter.Threshold = 50;
            _DsoPrsnt.Jitter.Hysteresis = 30;
            _DsoPrsnt.Jitter.PatternLength = 127;
            _DsoPrsnt.Jitter.BitRate = Double.NaN;
            _DsoPrsnt.Jitter.ClockType = ClockTypeOpt.Constant;
            _DsoPrsnt.Jitter.PllType = PllTypeOpt.Golden;
            _DsoPrsnt.Jitter.CutoffFreq1 = Double.NaN;
            _DsoPrsnt.Jitter.CutoffDivisor = 1667;
            _DsoPrsnt.Jitter.CurGraphType = JitterGraphType.Trend;
            _DsoPrsnt.Jitter.CurrentBinNum = MaxBinNum.Num250;
            _DsoPrsnt.Jitter.ThresholdFreq = 50;
            _DsoPrsnt.Jitter.NaturalFreq = Double.NaN;
            _DsoPrsnt.Jitter.DamplingFactor = 0.707;
        }

        private void PassFailDefault()
        {
            _DsoPrsnt.PassFail.Active = false;
            _DsoPrsnt.PassFail.Source = ChannelId.C1;
            _DsoPrsnt.PassFail.Mode = PFTestMode.LimitMode;
            _DsoPrsnt.PassFail.VertTolerance = 200;
            _DsoPrsnt.PassFail.HorzTolerance = 100;
            _DsoPrsnt.PassFail.MaskSource = ChannelId.C1;
            _DsoPrsnt.PassFail.Violations = 1;
            _DsoPrsnt.PassFail.TestWfms = 100;
            _DsoPrsnt.PassFail.TestDurationByms = 1000;
            _DsoPrsnt.PassFail.Store = false;
            _DsoPrsnt.PassFail.Beep = true;
            _DsoPrsnt.PassFail.Pulse = true;
            _DsoPrsnt.PassFail.HardCopy = false;
            _DsoPrsnt.PassFail.StdMaskType = PFStdMaskType.ANSI_T1_102;
            _DsoPrsnt.PassFail.MaskLocked = false;
        }

        private void SettingDefault()
        {
            _DsoPrsnt.AutoSet.VerticalSetting = true;
            _DsoPrsnt.AutoSet.HorizontalSetting = true;
            _DsoPrsnt.AutoSet.AcquisitionSetting = false;
            _DsoPrsnt.AutoSet.TriggerSetting = true;
            _DsoPrsnt.AutoSet.ChannelSetting = true;
            _DsoPrsnt.AutoSet.CouplingHold = false;
            _DsoPrsnt.AutoSet.OverlapView = false;
            _DsoPrsnt.Setting.AuxInputSignal = AuxInputType.Close;
            _DsoPrsnt.Setting.AuxInPolarity = EdgeSlope.Rise;
            _DsoPrsnt.Setting.AuxOutputSignal = AuxOutputType.Close;
            _DsoPrsnt.Setting.AuxOutPolarity = EdgeSlope.Rise;
        }

        private void FileDefault()
        {
            _DsoPrsnt.File.WfmFormat = WfmFormat.Binary;
            _DsoPrsnt.File.LongWfmFormat = WfmFormat.Binary;
            _DsoPrsnt.File.WfmSource = ChannelId.C1;
            _DsoPrsnt.File.WfmPath = Constants.WFM_DEF_PATH;
            _DsoPrsnt.File.FileName = "Uni-t000";
            _DsoPrsnt.File.IfAppendDatetime = false;
            _DsoPrsnt.File.PicRegion = PicArea.Application;
            _DsoPrsnt.File.PicColor = PicColor.Standard;
            _DsoPrsnt.File.PicFormat = PicFormat.Png;
            _DsoPrsnt.File.PicPath = Constants.PIC_DEF_PATH;
            _DsoPrsnt.File.SettingSavePath = Constants.SET_DEF_PATH;
            _DsoPrsnt.File.SettingLoadFullPath = Constants.SET_DEF_PATH;
        }

        private void PrintDefault()
        {
            _DsoPrsnt.Print.Orient = PrintOrient.Hor;
            _DsoPrsnt.Print.PrintArea = PrintSaveArea.Screen;
            _DsoPrsnt.Print.PrintColor = PrintColor.Standard;
        }

        private void CursorDefault()
        {
            _DsoPrsnt.Cursor.Active = false;
            _DsoPrsnt.Cursor.SyncSource = ChannelId.C1;
            _DsoPrsnt.Cursor.IsSyncMove = false;
            _DsoPrsnt.Cursor.Type = CursorType.Horizontal;
            _DsoPrsnt.Cursor.HCursor[0] = -3200;
            _DsoPrsnt.Cursor.HCursor[1] = 3200;
            _DsoPrsnt.Cursor.VCursor[0] = 1000;
            _DsoPrsnt.Cursor.VCursor[1] = 9000;
            _DsoPrsnt.Cursor.HCursor.PosFormat = CursorPosFormat.Axis;
            _DsoPrsnt.Cursor.VCursor.PosFormat = CursorPosFormat.Axis;
        }

        private void TriggerDefault()
        {
            TriggerPrsnt.GetOrMakeTrigger(_DsoPrsnt, TriggerType.Edge);
            _DsoPrsnt.CurrentTrigger.PosIndex = 0;
            if (_DsoPrsnt.CurrentTrigger is TrigEdgePrsnt edge)
            {
                edge.Source = ChannelId.C1;
                edge.Coupling = TriggerCoupling.DC;
                edge.Slope = EdgeSlope.Rise;
                edge.Impedance = TriggerImpedance.High1M;
            }
            TriggerPrsnt.Mode = TriggerMode.Auto;
            TriggerPrsnt.HoldoffByps = 6400;

            #region 脉宽设置
            try
            {
                if (DsoModel.Default.GetTriggerModel(TriggerType.PulseWidth) is TriggerWidthModel pulsewidthmode)
                {
                    pulsewidthmode.Source = ChannelId.C1;
                    pulsewidthmode.Polarity = PulsePolarity.Positive;
                    pulsewidthmode.Condition = PulseCondition.GreaterThan;
                    pulsewidthmode.WidthByps = 3200;
                    pulsewidthmode.UpperWidthByps = 3600;
                    pulsewidthmode.CompPosition = 0;
                }
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex.Message, LogLevel.Warn));
            }
            #endregion
            #region 视频设置
            try
            {
                if (DsoModel.Default.GetTriggerModel(TriggerType.Video) is TriggerVideoModel videomode)
                {
                    videomode.Source = ChannelId.C1;
                    videomode.Standard = VideoStandard.PAL;
                    videomode.Polarity = VideoPolarity.Positive;
                    videomode.Sync = VideoSync.Odd;
                    videomode.Line = 1;
                    videomode.CompPosition = 0;
                }
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex.Message, LogLevel.Warn));
            }
            #endregion
            #region 斜率设置
            try
            {
                if (DsoModel.Default.GetTriggerModel(TriggerType.Transition) is TriggerTransModel transmode)
                {
                    transmode.Source = ChannelId.C1;
                    transmode.Slope = EdgeSlope.Rise;
                    transmode.Condition = PulseCondition.GreaterThan;
                    transmode.LowerCompPosition = 0;
                    transmode.UpperCompPosition = 0.1;
                    transmode.WidthByps = 3200;
                    transmode.UpperWidthByps = 3600;
                }
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex.Message, LogLevel.Warn));
            }
            #endregion
            #region 欠幅设置
            try
            {
                if (DsoModel.Default.GetTriggerModel(TriggerType.Runt) is TriggerRuntModel runtmode)
                {
                    runtmode.Source = ChannelId.C1;
                    runtmode.Polarity = PulsePolarity.Positive;
                    runtmode.Condition = PulseCondition.GreaterThan;
                    runtmode.LowerCompPosition = 0;
                    runtmode.UpperCompPosition = 0.1;
                    runtmode.WidthByps = 3200;
                }
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex.Message, LogLevel.Warn));
            }
            #endregion
            #region 延迟设置
            try
            {
                if (DsoModel.Default.GetTriggerModel(TriggerType.Delay) is TriggerDelayModel delaymode)
                {
                    delaymode.SourceOne = ChannelId.C1;
                    delaymode.SourceTwo = ChannelId.C2;
                    delaymode.SourceOneSlope = EdgeSlope.Rise;
                    delaymode.SourceTwoSlope = EdgeSlope.Rise;
                    delaymode.Condition = PulseCondition.GreaterThan;
                    delaymode.WidthByps = 3200;
                    delaymode.UpperWidthByps = 3600;
                    delaymode.UpperCompPosition = 0;
                    delaymode.DataCompPosition = 0;
                }
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex.Message, LogLevel.Warn));
            }
            #endregion
            #region 超时设置
            try
            {
                if (DsoModel.Default.GetTriggerModel(TriggerType.TimeOut) is TriggerTimeOutModel timeoutmode)
                {
                    timeoutmode.Source = ChannelId.C1;
                    timeoutmode.Polarity = LevelPolarity.Positive;
                    timeoutmode.CompPosition = 0;
                    timeoutmode.DurationByps = 3200;
                }
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex.Message, LogLevel.Warn));
            }
            #endregion
            #region 持续时间设置
            try
            {
                if (DsoModel.Default.GetTriggerModel(TriggerType.SustainTime) is TriggerSustainTimeModel sustainmode)
                {
                    sustainmode.Condition = PulseCondition.GreaterThan;
                    sustainmode.WidthByps = 3200;
                    sustainmode.UpperWidthByps = 3600;
                }
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex.Message, LogLevel.Warn));
            }
            #endregion
            #region 建立保持设置
            try
            {
                if (DsoModel.Default.GetTriggerModel(TriggerType.SetupHold) is TriggerSetupHoldModel setupholdmode)
                {
                    setupholdmode.ClkSource = ChannelId.C1;
                    setupholdmode.DataSource = ChannelId.C2;
                    setupholdmode.ClkPolarity = EdgeSlope.Rise;
                    setupholdmode.ClkCompPosition = 0;
                    setupholdmode.DataPosPolarity = EdgeSlope.Rise;
                    setupholdmode.UpperDataPosition = 0;
                    setupholdmode.Violation = SetupHoldViolation.Setup;
                    setupholdmode.TsuByps = 3200;
                    setupholdmode.ThdByps = 3200;
                }
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex.Message, LogLevel.Warn));
            }
            #endregion
            #region N边沿设置
            try
            {
                if (DsoModel.Default.GetTriggerModel(TriggerType.NEdge) is TriggerNEdgeModel nedgemode)
                {
                    nedgemode.Source = ChannelId.C1;
                    nedgemode.Polarity = EdgeSlope.Rise;
                    nedgemode.DurationByps = 3200;
                    nedgemode.EdgeNumber = 1;
                    nedgemode.CompPosition = 0;
                }
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex.Message, LogLevel.Warn));
            }
            #endregion
        }

        private void AcqDefault()
        {
            _DsoPrsnt.Timebase.Mode = AnaChnlAcqMode.Normal;
            _DsoPrsnt.Timebase.AverageCnt = 8;
            _DsoPrsnt.Timebase.EnvelopeCnt = 8;
            _DsoPrsnt.Timebase.EnvelopOpt = MathExt.EvlpOpt.Roof;
            _DsoPrsnt.Timebase.ClockSrc = AnaChnlClkSrc.Inner;
            _DsoPrsnt.Timebase.StorageDepthOpt = 0;
            _DsoPrsnt.Timebase.InterplType = AnaChnlItplType.Sinx;
            _DsoPrsnt.Timebase.StorageMode = AnaChnlStorageMode.Long;
            _DsoPrsnt.Timebase.WorkMode = SegmentWorkMode.Single;
            _DsoPrsnt.Timebase.CurFrameId = 1;
            _DsoPrsnt.Timebase.ReferFrameIds = 1;
            _DsoPrsnt.Timebase.SequentStartFrame = 1;
            _DsoPrsnt.Timebase.RenderType = PlotRenderType.None;
        }

        private void TimebaseDefault()
        {
            _DsoPrsnt.Timebase.EnhancedBitsActive = false;
            _DsoPrsnt.Timebase.EnhancedBits = 0.5;
            _DsoPrsnt.Timebase.ScaleIndex = AnaChnlTimebaseIndex.Lv1u;
            _DsoPrsnt.Timebase.PositionByus = 0;
            _DsoPrsnt.Timebase.IsZoom = false;
        }

        private void SearchDefault()
        {
            _DsoPrsnt.Search.RemoveAll();
            //foreach (var item in DsoModel.Default.Search.Items)
            //{
            //    if (item.Value.EventEnable)
            //    {
            //        item.Value.EventEnable = false;
            //    }
            //    item.Value.Visible = false;
            //}
        }
    }

    public record DefaultMessageArgs
    {
    }
}

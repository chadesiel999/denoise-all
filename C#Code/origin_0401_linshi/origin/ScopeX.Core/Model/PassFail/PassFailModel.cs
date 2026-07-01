// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/3/23</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;
    using ScopeX.Hardware.Driver;
    using ScopeX.MathExt;
    using System.Linq;
    internal class PassFailModel : INotifyPropertyChanged
    {
        //测试使能
        private Boolean _Active = false;
        public Boolean Active
        {
            get => _Active;
            set
            {
                if (_Active != value)
                {
                    _Active = value;
                    if (_Active == false && Running)
                    {
                        Running = false;
                    }
                    Results.Reset();
                    if (value == true && _Pulse == true)
                    {
                        if (DsoModel.Default.Setting.AuxOutputSignal != AuxOutputType.Other)
                        {
                            DsoModel.Default.Setting.AuxOutputSignal = AuxOutputType.Other;
                            WeakTip.Default.Write("PassFail", MsgTipId.AuxOutputSetPassFail);
                        }
                    }
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _VisibleMask = true;
        /// <summary>
        /// Gets or sets the VisibleMask.
        /// </summary>
        public Boolean VisibleMask
        {
            get => _VisibleMask;
            set
            {
                if (_VisibleMask != value)
                {
                    _VisibleMask = value;
                    OnPropertyChanged();
                }
            }
        }

        //测试数据源
        private ChannelId _Source = ChannelId.C1;
        public ChannelId Source
        {
            get => _Source;
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    OnPropertyChanged();
                }
            }
        }

        //测试模式：标准或极限
        private PFTestMode _Mode = PFTestMode.LimitMode;
        public PFTestMode Mode
        {
            get => _Mode;
            set
            {
                if (_Mode != value)
                {
                    _Mode = value;
                    OnPropertyChanged();
                    //Reset();
                }
            }
        }

        //是否在测试中
        private Boolean _Running = false;
        public Boolean Running
        {
            get => _Running;
            set
            {
                if (_Running != value)
                {
                    if (value)
                    {
                        var chnl = DsoModel.Default.GetChannel(Source);
                        if (chnl != null && !chnl.Active)
                        {
                            WeakTip.Default.Write("PassFail", MsgTipId.PassFailChannelClosed);
                            return;
                        }
                        Results.Reset();
                        UpdateVuTask.HardCopyTimestamp = 0;
                    }
                    _Running = value;
                    OnPropertyChanged();
                }
            }
        }

        //private Boolean _VisibleInfo = true;
        //public Boolean VisibleInfo
        //{
        //    get => _VisibleInfo;
        //    set
        //    {
        //        if (_VisibleInfo != value)
        //        {
        //            _VisibleInfo = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        //private Boolean _VisibleMask = true;
        //public Boolean VisibleMask
        //{
        //    get => _VisibleMask;
        //    set
        //    {
        //        if (_VisibleMask != value)
        //        {
        //            _VisibleMask = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        private Boolean _MaskLocked = false;
        public Boolean MaskLocked
        {
            get => _MaskLocked;
            set
            {
                if (_MaskLocked != value)
                {
                    _MaskLocked = value;
                    if (_MaskLocked)
                    {
                        StdMaskTest.LockMask(Source);
                    }
                    OnPropertyChanged();
                }
            }
        }

        //测试违例阈值，无单位，缺省1次
        private Int32 _Violations = 1;
        public Int32 Violations
        {
            get => _Violations;
            set
            {
                value = ValidateViolations(value);
                if (_Violations != value)
                {
                    _Violations = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 ValidateViolations(Int32 value)
        {
            if (TestStopType == PFTestStopOpt.TestTime)
            {
                if (value > MaxViolations)
                {
                    value = MaxViolations;
                }
                else if (value < MinViolations)
                {
                    value = MinViolations;
                }
            }
            else
            {
                if (value > TestWfms)
                {
                    value = TestWfms;
                }
                else if (value < MinViolations)
                {
                    value = MinViolations;
                }
            }
            return value;
        }

        public readonly Int32 MaxViolations = Constants.MAX_VIOLATION_NUM;

        public readonly Int32 MinViolations = Constants.MIN_VIOLATION_NUM;

        //测试完成条件：测试时间或测试波形数
        private PFTestStopOpt _TestStopType = PFTestStopOpt.TestWfms;
        public PFTestStopOpt TestStopType
        {
            get => _TestStopType;
            set
            {
                if (_TestStopType != value)
                {
                    _TestStopType = value;
                    Violations = ValidateViolations(_Violations);
                    OnPropertyChanged();
                }
            }
        }


        //测试波形数，无单位，缺省100幅波形
        private Int32 _TestWfms = 100;
        public Int32 TestWfms
        {
            get => _TestWfms;
            set
            {
                value = ValidateTestWfms(value);
                if (_TestWfms != value)
                {
                    _TestWfms = value;
                    if (Violations > value)
                    {
                        Violations = value;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private Int32 ValidateTestWfms(Int32 value)
        {
            if (value > MaxTestWfms)
            {
                value = MaxTestWfms;
            }
            else if (value < MinTestWfms)
            {
                value = MinTestWfms;
            }

            return value;
        }

        public readonly Int32 MaxTestWfms = Constants.MAX_TEST_WFMS;

        public readonly Int32 MinTestWfms = Constants.MIN_TEST_WFMS;

        //测试时间，单位ms，缺省1s
        private Int32 _TestDurationByms = 1_000;
        public Int32 TestDurationByms
        {
            get => _TestDurationByms;
            set
            {
                value = ValidateTestDuration(value);
                if (_TestDurationByms != value)
                {
                    _TestDurationByms = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 ValidateTestDuration(Int32 value)
        {
            Int32 duration = (Int32)Math.Round((Double)value / StpTestDuration, MidpointRounding.AwayFromZero) * StpTestDuration;

            if (duration < MinTestDuration)
            {
                duration = MinTestDuration;
            }
            else if (duration > MaxTestDuration)
            {
                duration = MaxTestDuration;
            }

            return duration;
        }

        public readonly Int32 MaxTestDuration = Constants.MAX_TEST_DURATION_MS;

        public readonly Int32 MinTestDuration = Constants.MIN_TEST_DURATION_MS;

        public readonly Int32 StpTestDuration = Constants.STP_TEST_DURATION_MS;

        public Boolean TestState
        {
            get;
            private set;
        }

        private Boolean _Store = false;
        public Boolean Store
        {
            get => _Store;
            set
            {
                if (_Store != value)
                {
                    _Store = value;
                    OnPropertyChanged();
                }
            }
        }

        //private Boolean _Stop = false;
        //public Boolean Stop
        //{
        //    get => _Stop;
        //    set
        //    {
        //        if (_Stop != value)
        //        {
        //            _Stop = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        private Boolean _Beep = false;
        public Boolean Beep
        {
            get => _Beep;
            set
            {
                if (_Beep != value)
                {
                    _Beep = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _Pulse = false;
        public Boolean Pulse
        {
            get => _Pulse;
            set
            {
                if (_Pulse != value)
                {
                    _Pulse = value;
                    if (_Pulse == true && Active)
                    {
                        if (DsoModel.Default.Setting.AuxOutputSignal != AuxOutputType.Other)
                        {
                            DsoModel.Default.Setting.AuxOutputSignal = AuxOutputType.Other;
                            WeakTip.Default.Write("PassFail", MsgTipId.AuxOutputSetPassFail);
                        }
                    }
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _HardCopy = false;
        public Boolean HardCopy
        {
            get => _HardCopy;
            set
            {
                if (_HardCopy != value)
                {
                    _HardCopy = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _LabNoteBook = false;
        public Boolean LabNoteBook
        {
            get => _LabNoteBook;
            set
            {
                if (_LabNoteBook != value)
                {
                    _LabNoteBook = value;
                    OnPropertyChanged();
                }
            }
        }
        private WfmFormat _WfmFormat = WfmFormat.Binary;

        public WfmFormat WfmFormat
        {
            get => _WfmFormat;
            set
            {
                if (_WfmFormat != value)
                {
                    _WfmFormat = value;
                    OnPropertyChanged();
                }
            }
        }
        private PicFormat _PicFormat = PicFormat.Png;
        public PicFormat PicFormat
        {
            get => _PicFormat;
            set
            {
                if (_PicFormat != value)
                {
                    _PicFormat = value;
                    OnPropertyChanged();
                }
            }
        }
        private Boolean _IfAppendDatetime = false;
        public Boolean IfAppendDatetime
        {
            get => _IfAppendDatetime;
            set
            {
                if (_IfAppendDatetime != value)
                {
                    _IfAppendDatetime = value;
                    OnPropertyChanged();
                }
            }
        }

        private String _FileName = "Uni-t000";
        public String FileName
        {
            get => _FileName;
            set
            {
                if (_FileName != value)
                {
                    _FileName = value;
                    OnPropertyChanged();
                }
            }
        }
        private String _SavePath = Constants.PASSFAIL_DEF_PATH;
        public String SavePath
        {
            get => _SavePath;
            set
            {
                if (_SavePath != value)
                {
                    _SavePath = value;
                    OnPropertyChanged();
                }

                if (!AccessToFolder(_SavePath))
                {
                    WeakTip.Default.Write("SavePath", MsgTipId.AccessToFolderFailed, false, "", 2);
                    _SavePath = Constants.PASSFAIL_DEF_PATH;
                    OnPropertyChanged();
                }
            }
        }
        private TxtFormat _WfmTxtFormat = TxtFormat.UTF8;
        public TxtFormat WfmTxtFormat
        {
            get => _WfmTxtFormat;
            set
            {
                if (_WfmTxtFormat != value)
                {
                    _WfmTxtFormat = value;
                    OnPropertyChanged();
                }
            }
        }
        public String DefaultPrefixName
        {
            get;
            init;
        } = "Uni-t";
        private Boolean AccessToFolder(String Path)
        {
            try
            {
                var result = new DirectoryInfo(Path)
                .GetFiles($"*{_WfmTxtFormat.GetAlias()}", SearchOption.TopDirectoryOnly)
                .Where(x => Regex.IsMatch(x.Name, $"^{DefaultPrefixName}{""}[0-9]{"{3}"}{_WfmTxtFormat.GetAlias()}$", RegexOptions.IgnoreCase));
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //模板是否创建或读入
        public Boolean MaskCreated
        {
            get
            {
                if (Mode == PFTestMode.LimitMode)
                {
                    return LimitTest.MaskCreated;
                }
                else
                {
                    return StdMaskTest.MaskCreated;
                }
            }
        }

        //private void Reset()
        //{
        //    if (MaskCreated)
        //        MaskCreated = false;
        //}

        public readonly LimitTest LimitTest = new();

        private ChannelId _MaskSource = ChannelId.C1;
        public ChannelId MaskSource
        {
            get => _MaskSource;
            set
            {
                if (_MaskSource != value)
                {
                    _MaskSource = value;
                    OnPropertyChanged();
                }
            }
        }

        public readonly StdMaskTest StdMaskTest = new();

        public readonly PassFailInfo Results = new();
        private Int64 _WfmUpdateTime = 0;
        public void Run()
        {
            if (Running)
            {
                if (!Results.Timer.IsRunning)
                {
                    Results.Timer.Restart();
                }
                var chnl = DsoModel.Default.GetChannel(Source);
                if (chnl != null && !chnl.Active)
                {
                    WeakTip.Default.Write("PassFail", MsgTipId.PassFailChannelClosed);
                    Running = false;
                    return;
                }
                if (chnl?.PackForVu is null)
                {
                    return;
                }
                if (chnl?.PackForVu.Properties.WfmUpdateTime == _WfmUpdateTime)
                {
                    return;
                }
                _WfmUpdateTime = chnl.PackForVu.Properties.WfmUpdateTime;

                Boolean bsucceed = Test(Source);

                if (!bsucceed)
                {
                    Results.FailWfms[0]++;
                }

                Results.TotalWfms[0]++;

                if (Results.FailWfms[0] >= Violations)
                {
                    Running = false;
                }
                if (TestStopType == PFTestStopOpt.TestWfms)
                {
                    if (Results.TotalWfms[0] >= TestWfms)
                    {
                        Running = false;
                    }
                }
                else
                {
                    if (Results.Timer.ElapsedMilliseconds >= TestDurationByms)
                    {
                        Running = false;
                    }
                }

                Results.RunningTime[0] = Results.Timer.ElapsedMilliseconds;
                if (!Running)
                {
                    if (LabNoteBook)
                    {
                        FilePrsnt.SaveLabNoteBook(Constants.PASSFAIL_DEF_PATH, "PassFailLabNote", Source);
                    }
                    Results.Timer.Stop();
                }

                if (!bsucceed)
                {
                    DoAction();
                }

                TestState = bsucceed;
            }
            else
            {
                Results.Timer.Stop();
            }
        }
        private Boolean Test(ChannelId id)
        {
            if (Mode == PFTestMode.LimitMode)
            {
                return LimitTest.Test(id, Results, MaskLocked);
            }
            else
            {
                return StdMaskTest.Test(id, Results);
            }
        }

        private void DoAction()
        {
            if(HardCopy || Store)
            {
                if (!AccessToFolder(SavePath))
                {
                    WeakTip.Default.Write("SavePath", MsgTipId.AccessToFolderFailed, false, "", 2);
                    SavePath = Constants.PASSFAIL_DEF_PATH;
                }
            }
            if (HardCopy)
            {
                Results.FailedTimestamp = Stopwatch.GetTimestamp();
            }
            if (Store)
            {
                FileName = DsoPrsnt.DefaultDsoPrsnt.File.MakeDefaultFileName(SavePath, FilePrsnt.GetWfmFileExtName(WfmFormat));
                try
                {

                    if (WfmFormat != WfmFormat.CSV)
                    {
                        FilePrsnt.SaveWaveform(SavePath, FileName, WfmFormat, Source, IfAppendDatetime);
                    }
                    else
                    {
                        FilePrsnt.SavePassFailWaveform(SavePath, FileName, Source, IfAppendDatetime);
                    }
                }
                catch(Exception ex)
                {
                    EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex.ToString(), EventBus.LogLevel.Warn));
                }
            }

            if (Beep)
            {
                Hd.SetPassFailWarn();
            }

            if (Pulse)
            {
                Hd.SetAuxOutputSignal(false);
                System.Threading.Thread.Sleep(1);
                Hd.SetAuxOutputSignal(true);
            }

        }


        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
                LimitTest.PropertyChanged += value;
                StdMaskTest.PropertyChanged += value;
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
                LimitTest.PropertyChanged -= value;
                StdMaskTest.PropertyChanged -= value;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "") => _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}

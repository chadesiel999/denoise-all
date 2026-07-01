// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/3/24</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    /// <summary>
    /// Defines the <see cref="PassFailPrsnt" />.
    /// </summary>
    public class PassFailPrsnt : MulticastPrsnt<IPassFailView>, IPassFailPrsnt
    {
        public readonly List<String>[] StdMaskNames = new List<String>[]
        {
            new List<String>
            {
                "DS1 1.544Mb.msk",
                "DS1A 2.048Mb.msk",
                "DS1C 3.152Mb.msk",
                "DS2 6.312Mb.msk",
                "DS3 44.736Mb.msk",
                "DS4 Max Output 138.26Mb.msk",
                "DS4NA 139.26Mb.msk",
                "STS_1 Eye 51.84Mb.msk",
                "STS_1 Pulse 51.84Mb.msk",
                "STS_3 155.52Mb.msk",
                "STS_3 Max Output 155.52Mb.msk"
            },

            new List<String>
            {
                //" 32Mb 32.064Mb.msk",
                " 97Mb 97.728Mb.msk",
                "Clock Interface Coaxial Pair 2.048Mb.msk",
                "Clock Interface Symmetric Pair 2.048Mb.msk",
                "DS0 Data Contradirectional 64Mb.msk",
                "DS0 Double 64Mb.msk",
                "DS0 Single 64Mb.msk",
                "DS0 Timing 64Mb.msk",
                "DS1 G.703 1.544M.msk",
                "DS1 Old Rate 1.544Mb.msk",
                "DS2 Rate Coax Pair 6.312Mb.msk",
                "DS2 Rate Symmetric Pair 6.312Mb.msk",
                "DS3 G.703 44.736Mb.msk",
                "DS3 Old Rate 44.736Mb.msk",
                "E1 Coaxial Pair 2.048Mb.msk",
                "E1 Symmetric Pair 2.048Mb.msk",
                "E2 8.448Mb.msk",
                "E3 34.368Mb.msk",
                "E4 Binary 0 139.26Mb.msk",
                "E4 Binary 1 139.26Mb.msk",
                "STM_0 CMI 0 51.84Mb.msk",
                "STM_0 CMI 1 51.84Mb.msk",
                "STM_0 HDBx 51.84Mb.msk",
                "STM1E Binary 0 155.52Mb.msk",
                "STM1E Binary 1 155.52Mb.msk"
            },

            new List<String>
            {
                "HST1 480Mb.msk",
                "HST2 480Mb.msk",
                "HST3 480Mb.msk",
                "HST4 480Mb.msk",
                "HST5 480Mb.msk",
                "HST6 480Mb.msk"
            }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="PassFailPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="IPassFailView?"/>.</param>
        /// <param name="mco">The mco<see cref="ModelCreateOptions"/>.</param>
        public PassFailPrsnt(IDsoPrsnt idp, IPassFailView? view, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.PassFail,
                ModelCreateOptions.Standalone => new(),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        /// <summary>
        /// Gets or sets the Beep.
        /// </summary>
        public Boolean Beep { get => Model.Beep; set => Model.Beep = value; }

        /// <summary>
        /// Gets the BtmSegment.
        /// </summary>
        public List<(Double x, Double y)> BtmSegment => Model.LimitTest.BtmSegment;

        /// <summary>
        /// Gets or sets the Enabled.
        /// </summary>
        public Boolean Active
        {
            get => Model.Active;
            set
            {
                if (value && !Constants.ENABLE_PassFail)
                {
                    WeakTip.Default.Write("PassFail", MsgTipId.FunctionDisabled);
                    Model.Active = false;
                    return;
                }
                if (value)
                {
                    if (FunctionLimit.PassFailFunctionLimit(((DsoPrsnt)Dso).MutexFunctionFlag) == false)
                    {
                        return;
                    }
                }
                if (value && Model.Active != value)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Timebase.LimitScan(MsgTipId.PassFailIsNotSupportedInScan);
                }
                Model.Active = value;
                if (MaskCreated == false && value)
                {
                    if (Mode == PFTestMode.LimitMode)
                        Model.LimitTest.MakeMask(MaskSource);
                }
            }
        }

        public Boolean VisibleMask
        {
            get => Model.VisibleMask;
            set => Model.VisibleMask = value;
        }

        /// <summary>
        /// Gets or sets the HardCopy.
        /// </summary>
        public Boolean HardCopy { get => Model.HardCopy; set => Model.HardCopy = value; }

        /// <summary>
        /// Gets or sets the HorzTolerance by mDiv.
        /// </summary>
        public Int32 HorzTolerance
        {
            get => Model.LimitTest.HorzTolerance;
            set => Model.LimitTest.HorzTolerance = value;
        }

        /// <summary>
        /// Gets the IsStdMaskValid.
        /// </summary>
        public Boolean IsStdMaskValid => Model.StdMaskTest.StdMask.IsVaild;
        public WfmFormat WfmFormat
        {
            get => Model.WfmFormat;
            set => Model.WfmFormat = value;
        }

        public PicFormat PicFormat
        {
            get => Model.PicFormat;
            set => Model.PicFormat = value;
        }
        public Boolean IfAppendDatetime
        {
            get => Model.IfAppendDatetime;
            set => Model.IfAppendDatetime = value;
        }

        public String FileName
        {
            get => Model.FileName;
            set => Model.FileName = value;
        }

        public String SavePath
        {
            get => Model.SavePath;
            set => Model.SavePath = value;
        }
        /// <summary>
        /// Gets or sets the LabNoteBook.
        /// </summary>
        public Boolean LabNoteBook { get => Model.LabNoteBook; set => Model.LabNoteBook = value; }

        /// <summary>
        /// Gets the Length.
        /// </summary>
        public Int32 Length => Model.StdMaskTest.StdMask.SegPaths.Length;

        /// <summary>
        /// Gets the MaskCreated.
        /// </summary>
        public Boolean MaskCreated => Model.MaskCreated;

        /// <summary>
        /// Gets or sets the MaskLocked.
        /// </summary>
        public Boolean MaskLocked { get => Model.MaskLocked; set => Model.MaskLocked = value; }

        /// <summary>
        /// Gets or sets the MaskSource.
        /// </summary>
        public ChannelId MaskSource { get => Model.MaskSource; set => Model.MaskSource = value; }

        /// <summary>
        /// Gets the MaskStart.
        /// </summary>
        public Double MaskStart => Model.LimitTest.MaskStart;

        /// <summary>
        /// Gets the MaskZoomRatio.
        /// </summary>
        public Double MaskZoomRatio => Model.LimitTest.MaskZoomRatio;

        /// <summary>
        /// Gets the MaxHorzTolerance.
        /// </summary>
        public Int32 MaxHorzTolerance => Model.LimitTest.MaxHorzTolerance;

        /// <summary>
        /// Gets the MaxTestDuration.
        /// </summary>
        public Int32 MaxTestDuration => Model.MaxTestDuration;

        /// <summary>
        /// Gets the MaxTestWfms.
        /// </summary>
        public Int32 MaxTestWfms => Model.MaxTestWfms;

        /// <summary>
        /// Gets the MaxVertTolerance.
        /// </summary>
        public Int32 MaxVertTolerance => Model.LimitTest.MaxVertTolerance;

        /// <summary>
        /// Gets the MaxViolations.
        /// </summary>
        public Int32 MaxViolations => Model.MaxViolations;

        /// <summary>
        /// Gets the MinHorzTolerance.
        /// </summary>
        public Int32 MinHorzTolerance => Model.LimitTest.MinHorzTolerance;

        /// <summary>
        /// Gets the MinTestDuration.
        /// </summary>
        public Int32 MinTestDuration => Model.MinTestDuration;

        /// <summary>
        /// Gets the MinTestWfms.
        /// </summary>
        public Int32 MinTestWfms => Model.MinTestWfms;

        /// <summary>
        /// Gets the MinVertTolerance.
        /// </summary>
        public Int32 MinVertTolerance => Model.LimitTest.MinVertTolerance;

        /// <summary>
        /// Gets the MinViolations.
        /// </summary>
        public Int32 MinViolations => Model.MinViolations;

        /// <summary>
        /// Gets or sets the Mode.
        /// </summary>
        public PFTestMode Mode { get => Model.Mode; set => Model.Mode = value; }

        /// <summary>
        /// Gets or sets the Pulse.
        /// </summary>
        public Boolean Pulse { get => Model.Pulse; set => Model.Pulse = value; }

        /// <summary>
        /// Gets the Results.
        /// </summary>
        public PassFailInfo Results => Model.Results;

        /// <summary>
        /// Gets or sets the Running.
        /// </summary>
        public Boolean Running { get => Model.Running; set => Model.Running = value; }

        /// <summary>
        /// Gets or sets the Source.
        /// </summary>
        public ChannelId Source { get => Model.Source; set => Model.Source = value; }

        /// <summary>
        /// Gets or sets the StdMaskName.
        /// </summary>
        public String CurrentStdMaskName
        {
            get
            {
                var name = StdMaskNames[(Int32)StdMaskType][StdMaskIndex];
                return name.Substring(0, name.LastIndexOf('.'));
            }
        }

        public Int32 StdMaskIndex
        {
            get => Model.StdMaskTest.MaskIndex;
            set
            {
                value = Math.Clamp(value, 0, StdMaskNames[(Int32)StdMaskType].Count - 1);
                Model.StdMaskTest.MaskIndex = value;
                ReadStdMask();
            }
        }

        /// <summary>
        /// Gets or sets the StdMaskType.
        /// </summary>
        public PFStdMaskType StdMaskType
        {
            get => Model.StdMaskTest.StdMaskType;
            set
            {
                Model.StdMaskTest.StdMaskType = value;
                ReadStdMask();
            }
        }

        /// <summary>
        /// Gets or sets the Store.
        /// </summary>
        public Boolean Store { get => Model.Store; set => Model.Store = value; }

        /// <summary>
        /// Gets or sets the TestDurationByms.
        /// </summary>
        public Int32 TestDurationByms { get => Model.TestDurationByms; set => Model.TestDurationByms = value; }

        /// <summary>
        /// Gets or sets the TestDurationByms.
        /// </summary>
        public Double TestDurationByus
        {
            get => TestDurationByms * 1_000D;
            set => TestDurationByms = (Int32)(value / 1_000D);
        }

        /// <summary>
        /// Gets or sets the TestState.
        /// </summary>
        public Boolean TestState { get => Model.TestState; }

        /// <summary>
        /// Gets or sets the TestStopType.
        /// </summary>
        public PFTestStopOpt TestStopType { get => Model.TestStopType; set => Model.TestStopType = value; }

        /// <summary>
        /// Gets or sets the TestWfms.
        /// </summary>
        public Int32 TestWfms { get => Model.TestWfms; set => Model.TestWfms = value; }

        /// <summary>
        /// Gets the TopSegment.
        /// </summary>
        public List<(Double x, Double y)> TopSegment => Model.LimitTest.TopSegment;

        /// <summary>
        /// Gets or sets the VertTolerance by mDiv.
        /// </summary>
        public Int32 VertTolerance
        {
            get => Model.LimitTest.VertTolerance;
            set => Model.LimitTest.VertTolerance = value;
        }

        /// <summary>
        /// Gets or sets the Violations.
        /// </summary>
        public Int32 Violations { get => Model.Violations; set => Model.Violations = value; }

        /// <summary>
        /// Gets the Model.
        /// </summary>
        private protected override PassFailModel Model { get; }

        /// <summary>
        /// The AdjTestDuration.
        /// </summary>
        /// <param name="step">The step<see cref="Int32"/>.</param>
        public void AdjTestDuration(Int32 step)
        {
            Model.TestDurationByms += step * Model.StpTestDuration;
        }

        /// <summary>
        /// The GetStdMaskPolygon.
        /// </summary>
        /// <param name="index">The index<see cref="Int32"/>.</param>
        /// <returns>The <see cref="IReadOnlyList{(Double x, Double y)}"/>.</returns>
        public IReadOnlyList<(Double x, Double y)> GetStdMaskPolygon(Int32 index)
        {
            return Model.StdMaskTest.StdMask.SegPaths[index].AsReadOnly();
        }

        /// <summary>
        /// The MakeMask.
        /// </summary>
        public void MakeMask()
        {
            if (Mode == PFTestMode.LimitMode)
                Model.LimitTest.MakeMask(MaskSource);
            else
                Model.StdMaskTest.MakeMask(Source, MaskLocked);
        }
        public void UpdateLimitMask()
        {
            if (MaskLocked || Active == false)
                return;
            Model.LimitTest.UpdateMask(MaskSource);
        }
        /// <summary>
        /// The ReadStdMask.
        /// </summary>
        /// <returns>The <see cref="Boolean"/>.</returns>
        public Boolean ReadStdMask() => ReadStdMask(GetResourceString(CurrentStdMaskName));

        /// <summary>
        /// The ReadStdMask.
        /// </summary>
        /// <param name="name">The name<see cref="String"/>.</param>
        /// <returns>The <see cref="Boolean"/>.</returns>
        public Boolean ReadStdMask(String name)
        {
            if (Mode == PFTestMode.StdMaskMode && Model.StdMaskTest.ReadMask(name, Source))
            {
                return Model.StdMaskTest.MakeMask(Source, MaskLocked);
            }

            return false;
        }

        /// <summary>
        /// The GetResourceString.
        /// </summary>
        /// <param name="name">The name<see cref="String"/>.</param>
        /// <returns>The <see cref="String"/>.</returns>
        private static String GetResourceString(String name)
        {
            return name.Replace(" ", "_").Replace(".", "_");
        }

        /// <summary>
        /// The GetTableReslutString.
        /// </summary>
        /// <returns>The <see cref="String"/>.</returns>
        public String GetTableResultString()
        {
            String outputstring = Environment.NewLine;
            outputstring += Results.Titles[0] + "," + (Running ? @"Running" : @"OFF") + Environment.NewLine;
            outputstring += Results.Titles[1] + "," + Results.TotalWfms[0].ToString() + Environment.NewLine;
            outputstring += Results.Titles[2] + "," + Results.FailWfms[0].ToString() + Environment.NewLine;
            outputstring += Results.Titles[3] + "," + new Quantity(Results.RunningTime[0], Prefix.Milli, QuantityUnit.Second).ToString() + Environment.NewLine;
            outputstring += Results.Titles[4] + "," + Results.TotalHits[0].ToString() + Environment.NewLine;
            outputstring += Results.Titles[5] + Environment.NewLine;
            for (Int32 i = 1; i <= Results.SegHits.GetLength(0); i++)
            {
                outputstring += i + "," + Results.SegHits[i - 1, 0] + Environment.NewLine;
            }

            return outputstring;
        }
    }
}

using ScopeX.ComModel;
using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;
using static ScopeX.Core.MathEResArg;
using static ScopeX.Core.MathFftArg;

namespace ScopeX.Core
{
    public class MathPrsnt : ChannelPrsnt
    {
        private protected override MathModel Model
        {
            get;
        }

        public event Action<ChannelPrsnt> FirstAsyncPackComed;

        public MathPrsnt(ChannelId id, IDsoPrsnt idp) : base(idp, null)
        {
            Model = (MathModel)DsoModel.Default.GetChannel(id);
            Model.PropertyChanged += OnPropertyChanged;


            Model.Sampling.PropertyChanged += OnPropertyChanged;

            Model.GetOrMakeArg = GetOrMakeArg;
            Sampling = new SamplingPrsnt(Model.Sampling);

            Model.Conditioning.Prompter = WeakTip.Default;
            Model.Sampling.Prompter = WeakTip.Default;

            var formula = Model.Formula.Split(":");
            var type = Enum.Parse<MathType>(formula[0]);

            BinaryPrsnt = new Lazy<MathBinaryArg>(() => new(this,id, Model.Formula));
            FftPrsnt = new Lazy<MathFftArg>(() => new(this,id, Model.Formula));
            ZoomPrsnt = new Lazy<MathZoomArg>(() => new(this,id, Model.Formula));
            CustomPrsnt = new Lazy<MathCustomArg>(() => new(this, id, Model.Formula));
            FilterPrsnt = new Lazy<MathFilterArg>(() => new(this, id, Model.Formula));
            EResPrsnt = new Lazy<MathEResArg>(() => new(this, id, Model.Formula));
            HistPrsnt = new Lazy<MathHistArg>(() => new(this, id, Model.Formula));
            TrackPrsnt = new Lazy<MathTrackArg>(() => new(this, id, Model.Formula));
            TrendPrsnt = new Lazy<MathTrendArg>(() => new(this, id, Model.Formula));
            UserProgramPrsnt = new Lazy<MathUserProgramArg>(() => new(this, id, Model.Formula));

            //!!!Notice: disable the recall of custom function
            if (type == MathType.Custom)
            {
                type = MathType.Binary;
            }

            Args = GetOrMakeArg(type);


            FirstAsyncPackComed += (_) =>
            {
                //FrequencyAdapter.FreshHorizonValue();
                //FrequencyAdapter.FreshVerticalValue();
            };

            Model.FirstAsyncPackComed += (_) => FirstAsyncPackComed?.Invoke(this);

        }


        public FrequencyAdapter FrequencyAdapter
        {
            get => Model.FrequencyAdapter;
        }
        /// <summary>
        /// Format of formula is "TYPE:EXP", for example "Binary:C1+C2".
        /// </summary>
        public String Formula
        {
            get => Model.Formula;
            set => Model.Formula = value;
        }

        public List<ChannelId> PreMathChannels
        {
            get => Model.PreMathChannels;
            set => Model.PreMathChannels = value;
        }
        /// <summary>
        /// Math arguments which are explanation about 'Formula'.
        /// </summary>
        public MathArgPrsnt Args
        {
            get => Model.Args!;
            set => Model.Args = value;
        }

        public MathArgPrsnt GetOrMakeArg(MathType mt)
        {
            if (Args?.Type == mt)
            {
                if (Args.Type == MathType.Track && DsoModel.Default.Timebase != null)//???　　     todo
                {
                    //!!!Notice: Solve unkown Exception
                    Model.Conditioning.ScaleIndex = 0;
                    Model.Conditioning.PosIndex = Model.Conditioning.PosDefIndex;
                    Model.Sampling.ScaleMaxIndex = (Int32)DsoModel.Default.Timebase.ScaleMaxIndex;
                    Model.Sampling.ScaleMinIndex = (Int32)DsoModel.Default.Timebase.ScaleMinIndex;
                    Model.Sampling.InitialScale = ((Int32)DsoModel.Default.Timebase.ScaleIndex, DsoModel.Default.Timebase.Scale);
                    Model.Sampling.ScaleIndex = (Int32)DsoModel.Default.Timebase.ScaleIndex;
                    Model.Sampling.PosMaxIndex = (Int32)DsoModel.Default.Timebase.PosMaxIndex;
                    Model.Sampling.PosMinIndex = (Int32)DsoModel.Default.Timebase.PosMinIndex;
                    Model.Sampling.PosIndex = DsoModel.Default.Timebase.PosIndex;
                }
                return Args;
            }
            else
            {
                //Model.Conditioning.ScaleIndex = 0;
                //Model.Conditioning.PosIndex = Model.Conditioning.PosDefIndex;
                //Model.Sampling.ScaleIndex = 0;
                //Model.Sampling.PosIndex = Model.Sampling.PosDefIndex;
            }
            if (Args != null && Args.Type == MathType.UserProgram && Args is MathUserProgramArg userp && userp != null)
            {
                if (userp.RunState != RunStateType.Stop)
                {
                    userp.RunState = RunStateType.Stop;
                }
            }
            Args = mt switch
            {
                MathType.Binary => BinaryPrsnt.Value,
                MathType.FFT => FftPrsnt.Value,
                MathType.Zoom => ZoomPrsnt.Value,
                MathType.Filter => FilterPrsnt.Value,
                MathType.ERes => EResPrsnt.Value,
                MathType.Histgram => HistPrsnt.Value,
                MathType.Track => TrackPrsnt.Value,
                MathType.Trend => TrendPrsnt.Value,
                MathType.Custom => CustomPrsnt.Value,
                MathType.UserProgram => UserProgramPrsnt.Value,

                _ => throw new NotImplementedException($"{mt} is not supported by {nameof(MathType)}."),
            };

            Formula = Args.MakeFormula();
            Model.InitFlag = true;
            Model.ResetFFT = true;

            if (Args.Type == MathType.Track && DsoModel.Default.Timebase != null)
            {
                //!!!Notice: Solve unkown Exception
                Model.Conditioning.ScaleIndex = 0;
                Model.Conditioning.PosIndex = Model.Conditioning.PosDefIndex;
                Model.Sampling.ScaleMaxIndex = (Int32)DsoModel.Default.Timebase.ScaleMaxIndex;
                Model.Sampling.ScaleMinIndex = (Int32)DsoModel.Default.Timebase.ScaleMinIndex;
                Model.Sampling.InitialScale = ((Int32)DsoModel.Default.Timebase.ScaleIndex, DsoModel.Default.Timebase.Scale);
                Model.Sampling.ScaleIndex = (Int32)DsoModel.Default.Timebase.ScaleIndex;
                Model.Sampling.PosMaxIndex = (Int32)DsoModel.Default.Timebase.PosMaxIndex;
                Model.Sampling.PosMinIndex = (Int32)DsoModel.Default.Timebase.PosMinIndex;
                Model.Sampling.PosIndex = DsoModel.Default.Timebase.PosIndex;
            }
            if (Args.Type == MathType.Histgram || Args.Type == MathType.Trend|| Args.Type == MathType.Track)
            {
                Model.AutoScale = true;
            }
            else
            {
                Model.AutoScale = false;
            }
            Dispatcher.SoftReset();
            return Args;
        }


        /// <summary>
        /// Vertical zero position, its unit is usually 'mV'.
        /// </summary>
        public Double Position => Model.Conditioning.Position;


        public Double MaxScale => Model.Conditioning.MaxScale;

        public Double MinScale => Model.Conditioning.MinScale;

        /// <summary>
        /// Vertical max scale value, its unit is usually 'mV'.
        /// </summary>
        public Int32 ScaleMaxIndex => Model.Conditioning.ScaleMaxIndex;

        /// <summary>
        /// Vertical min scale value, its unit is usually 'mV'.
        /// </summary>
        public Int32 ScaleMinIndex => Model.Conditioning.ScaleMinIndex;

        public void InitFlag()
        {
            Model.InitFlag = true;
        }

        public void WindowSwtich(Int64? windowId, Boolean isswtich)
        {
            Model.WindowId = windowId;
            Model.IsSwitchWindow = isswtich;
        }

        public void TmbInitFlag()
        {
            Model.TmbInitFlag = true;
        }
        public void ResetFFT()
        {
            Model.ResetFFT = true;
        }
        /// <summary>
        /// Whether or not math channel's vertical unit is generated automatically.
        /// </summary>
        public Boolean IsAutoUnit
        {
            get => Model.IsAutoUnit;
            set
            {
                Model.IsAutoUnit = value;
                Args.IsAutoUnit = value;
                Dispatcher.SoftReset();
            }
        }

        /// <summary>
        /// used by scpi
        /// </summary>
        public Boolean IsCustomUnit
        {
            get => !IsAutoUnit;
            set => IsAutoUnit = !value;
        }

        public String CustomUnit
        {
            get => Model.CustomUnit;
            set => Model.CustomUnit = value;
        }

        public override String Unit
        {
            get => base.Unit;
            set
            {
                IsAutoUnit = false;
                base.Unit = value;
            }
        }

        public override Boolean Active
        {
            get => base.Active;
            set
            {
                if (!Constants.ENABLE_Math)
                {
                    WeakTip.Default.Write("Math", MsgTipId.FunctionDisabled);
                    base.Active = false;
                    return;
                }
                if (base.Active != value)
                {
                    if (value)
                    {
                        if (FunctionLimit.MathFunctionLimit(((DsoPrsnt)Dso).MutexFunctionFlag) == false)
                        {
                            return;
                        }
                    }
                    if (!Id.IsAdvancedMath())
                    {
                        if (!value)
                        {
                            DsoPrsnt.DefaultDsoPrsnt.Markers.TryRemoveItem(Id);
                            if (FftPrsnt.Value != null)
                            {
                                FftPrsnt.Value.Marker = null;
                            }
                            if (Args is MathHistArg mha)
                            {
                                if (mha.Source.IsMeasure())
                                {
                                    ((DsoPrsnt)Dso).Measure[mha.Source - ChannelId.P1].HistgramEnable = value;
                                }
                                if (mha.Source == ChannelId.DVM)
                                {
                                    DsoPrsnt.DefaultDsoPrsnt.Voltmeter.HistgramEnable = value;
                                }
                                if (mha.Source == ChannelId.CYM)
                                {
                                    DsoPrsnt.DefaultDsoPrsnt.Cymometer.HistgramEnable = value;
                                }

                            }
                            if (Args is MathTrendArg mtd)
                            {
                                if (mtd.Source.IsMeasure())
                                {
                                    ((DsoPrsnt)Dso).Measure[mtd.Source - ChannelId.P1].TrendEnable = value;
                                }
                                if (mtd.Source == ChannelId.DVM)
                                {
                                    DsoPrsnt.DefaultDsoPrsnt.Voltmeter.TrendEnable = value;
                                }
                                if (mtd.Source == ChannelId.CYM)
                                {
                                    DsoPrsnt.DefaultDsoPrsnt.Cymometer.TrendEnable = value;
                                }
                            }
                            if (Args is MathTrackArg mtk)
                            {
                                if (mtk.Source.IsMeasure())
                                {
                                    ((DsoPrsnt)Dso).Measure[mtk.Source - ChannelId.P1].TrackEnable = value;
                                }
                                if (mtk.Source == ChannelId.DVM)
                                {
                                    DsoPrsnt.DefaultDsoPrsnt.Voltmeter.TrackEnable = value;
                                }
                                if (mtk.Source == ChannelId.CYM)
                                {
                                    DsoPrsnt.DefaultDsoPrsnt.Cymometer.TrackEnable = value;
                                }
                            }
                        }
                        else
                        {
                            DsoPrsnt.DefaultDsoPrsnt.Markers.TryGetorAddItem(Id, out var prsnt);
                            prsnt.AtuoMarkerActive = false;
                            FftPrsnt.Value.Marker = prsnt;
                        }
                    }
                    //if (DsoPrsnt.DefaultDsoPrsnt?.Jitter != null && Id.IsJitterMath() && Id == DsoPrsnt.DefaultDsoPrsnt.Jitter.GetGraphChannelId(JitterGraphType.Eye))
                    //{
                    //    DsoPrsnt.DefaultDsoPrsnt.Jitter.EyeEnable = value;
                    //}
                    if (Id >= ChannelIdExt.MinCEMChId && Id <= ChannelIdExt.MaxCEMChId && value == false)
                    {
                        DsoModel.Default.ExceptionCapture.Active = false;
                    }
                    base.Active = value;
                    Model.InitFlag = true;
                    Model.ResetFFT = true;
                }
            }
        }

        public Boolean AutoScale
        {
            get => Model.AutoScale;
            set
            {
                Model.AutoScale = value;
            }
        }
        public override SamplingPrsnt Sampling
        {
            get;
        }

        private static String MakeFileName()
        {
            return "CustomFormula" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public Boolean IsJitterMath()
        {
            if (this.Args.Occupier is JitterGraphModel jitter && (jitter.Formula == Constants.JITTER_HISTOGRAM_FORMULA || jitter.Formula == Constants.JITTER_EYE_FORMULA))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean IsOnlyIndependenForm()
        {
            if (this.Args.Type is MathType.FFT or
                MathType.Zoom or
                MathType.Histgram or MathType.Trend
                || (this.Args.Occupier is JitterGraphModel jitter && (jitter.Formula == Constants.JITTER_HISTOGRAM_FORMULA
                                                                      || jitter.Formula == Constants.JITTER_SPECTRUM_FORMULA
                                                                      || jitter.Formula == Constants.JITTER_BATHTUB_FORMULA
                                                                      || jitter.Formula == Constants.JITTER_EYE_FORMULA)))
            {
                return true;
            } 
            else if (this.Id >= ChannelIdExt.MinMDChId & this.Id <= ChannelIdExt.MaxMDChId) 
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public Boolean Save(String path, String name = "")
        //{
        //    if (Formula is not null)
        //    {
        //        if (String.IsNullOrWhiteSpace(name))
        //        {
        //            name = MakeFileName();
        //        }

        //        return FilePrsnt.SaveToText(path, name, sw => sw.WriteLine(Formula));

        //    }
        //    return false;
        //}

        //public Boolean Load(String fullName)
        //{
        //    String? formula = null;
        //    var res = FilePrsnt.LoadFromText(fullName, (sr) => formula = sr.ReadLine());

        //    if (formula is not null)
        //    {
        //        Formula = formula;

        //        return true;
        //    }
        //    return false;
        //}

        public Double QueryScaleValue(Int32 ScaleIndex)
        {
            return Model.Conditioning.GetScaleValue(ScaleIndex, 0);
        }

        protected readonly Lazy<MathBinaryArg> BinaryPrsnt;
        protected readonly Lazy<MathFftArg> FftPrsnt;
        protected readonly Lazy<MathZoomArg> ZoomPrsnt;
        protected readonly Lazy<MathCustomArg> CustomPrsnt;
        protected readonly Lazy<MathFilterArg> FilterPrsnt;
        protected readonly Lazy<MathEResArg> EResPrsnt;
        protected readonly Lazy<MathHistArg> HistPrsnt;
        protected readonly Lazy<MathTrackArg> TrackPrsnt;
        protected readonly Lazy<MathTrendArg> TrendPrsnt;
        protected readonly Lazy<MathUserProgramArg> UserProgramPrsnt;
    }
}
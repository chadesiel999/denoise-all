using EventBus;
using ScopeX.ComModel;
using ScopeX.Core;
using ScopeX.Core.Presenter.RadioFrequency;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ScopeX.Core
{
    public class MathFftArg : MathArgPrsnt
    {
        public MathFftArg(MathPrsnt mp, ChannelId id, String formula) : base(mp,id, MathType.FFT)
        {
            _Args = ParseFormula(formula);
            _OldUnit = FFTCoordUnit.Vrms;

            ACPR = new AdjacentChannelPowerRatio(id);
            OB = new OccupiedBandwidth(id);
            CP = new ChannelPower(id);
            THD = new TotalHarmonicDistortion(id);
            if (DsoPrsnt.DefaultDsoPrsnt != null && DsoPrsnt.DefaultDsoPrsnt.Markers != null && Marker == null)
            {
                Marker = DsoPrsnt.DefaultDsoPrsnt.Markers[id];
            }
        }

        private FftArgs _Args;

        public MarkerItemPrsnt Marker { get; set; }
        public Boolean EnableFreqMeasure
        {
            get;
            set;
        }

        public FrequencyMeasureType MeasureType
        {
            get;
            set;
        } = FrequencyMeasureType.OB;



        public List<(Single xMin, Single xMax)> MeasureGates
        {
            get
            {
                switch (MeasureType)
                {
                    case FrequencyMeasureType.ACPR:
                        return ACPR.MeasGates;
                    case FrequencyMeasureType.OB:
                        return OB.MeasGates;
                    case FrequencyMeasureType.CP:
                        return CP.MeasGates;
                    case FrequencyMeasureType.THD:
                        return THD.MeasGates;
                    default:
                        return new();
                }
            }

        }

        public List<(Single Position, Single Result, String ResultUnitString)> MeasureResults
        {
            get
            {
                switch (MeasureType)
                {
                    case FrequencyMeasureType.ACPR:
                        return ACPR.MeasResults;
                    case FrequencyMeasureType.OB:
                        return OB.MeasResults;
                    case FrequencyMeasureType.CP:
                        return CP.MeasResults;
                    case FrequencyMeasureType.THD:
                        return THD.MeasResults;
                    default:
                        return new();
                }
            }

        }
        #region FreqMeas
        #region ACPR
        public AdjacentChannelPowerRatio ACPR
        {
            get;
            set;
        }

        public Boolean EnableACPR
        {
            get { return ACPR.Enable; }
            set
            {
                if (value != ACPR.Enable)
                {
                    ACPR.Enable = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }
        public Double ChannelSpanACPR
        {
            get { return ACPR.ChannelSpan; }
            set
            {
                if (value != ACPR.ChannelSpan)
                {
                    ACPR.ChannelSpan = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }

        public Double ChannelSpacingACPR
        {
            get { return ACPR.ChannelSpacing; }
            set
            {
                if (value != ACPR.ChannelSpacing)
                {
                    ACPR.ChannelSpacing = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }

        public Int32 ChannelCountACPR
        {
            get { return ACPR.ChannelCount; }
            set
            {
                if (value != ACPR.ChannelCount)
                {
                    ACPR.ChannelCount = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }

        #endregion
        #region OB
        public OccupiedBandwidth OB
        {
            get;
            set;
        }

        public Boolean EnableOB
        {
            get { return OB.Enable; }
            set
            {
                if (value != OB.Enable)
                {
                    OB.Enable = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }

        public Double ChannelSpanOB
        {
            get { return OB.ChannelSpan; }
            set
            {
                if (value != OB.ChannelSpan)
                {
                    OB.ChannelSpan = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }

        public OBWAnalysisType AnalysisTypeOB
        {
            get { return OB.AnalysisType; }
            set
            {
                if (value != OB.AnalysisType)
                {
                    OB.AnalysisType = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }

        public Double PercentageOB
        {
            get { return OB.Percentage; }
            set
            {
                if (value != OB.Percentage)
                {
                    OB.Percentage = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }

        public Int32 dBDownOB
        {
            get { return OB.dBDown; }
            set
            {
                if (value != OB.dBDown)
                {
                    OB.dBDown = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }


        #endregion
        #region CP
        public ChannelPower CP
        {
            get;
            set;
        }

        public Boolean EnableCP
        {
            get { return CP.Enable; }
            set
            {
                if (value != CP.Enable)
                {
                    CP.Enable = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }
        public Double ChannelSpanCP
        {
            get { return CP.ChannelSpan; }
            set
            {
                if (value != CP.ChannelSpan)
                {
                    CP.ChannelSpan = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }


        #endregion
        #region THD
        public TotalHarmonicDistortion THD
        {
            get;
            set;
        }

        public Boolean EnableTHD
        {
            get { return THD.Enable; }
            set
            {
                if (value != THD.Enable)
                {
                    THD.Enable = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }
        public Double ChannelSpanTHD
        {
            get { return THD.ChannelSpan; }
            set
            {
                if (value != THD.ChannelSpan)
                {
                    THD.ChannelSpan = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }

        public Double ChannelSpacingTHD
        {
            get { return THD.ChannelSpacing; }
            set
            {
                if (value != THD.ChannelSpacing)
                {
                    THD.ChannelSpacing = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }

        public Int32 ChannelCountTHD
        {
            get { return THD.ChannelCount; }
            set
            {
                if (value != THD.ChannelCount)
                {
                    THD.ChannelCount = value;
                    Model.FreshMeasureSetParameter();
                }
            }
        }

        #endregion


        #endregion

        public ChannelId Source
        {
            get => _Args.Source;
            set
            {
                if (_Args.Source != value)
                {
                    _Args = _Args with { Source = value };

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    OnPropertyChanged();
                }
            }
        }

        public override Boolean IsAutoUnit
        {
            get => Model.IsAutoUnit;
            set
            {
                if (value != Model.IsAutoUnit)
                {
                    if (_Args.ResultType == FFTResultOpt.Ampltd && value == true)
                    {
                        _OldUnit = _Args.ResultUnit;
                        _Args = _Args with { ResultUnit = FFTCoordUnit.Vrms };
                    }
                    else
                    {
                        _Args = _Args with { ResultUnit = _OldUnit };
                    }
                    Model.IsAutoUnit = value;
                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    Model.ResetLine = true;
                }
            }
        }
        
        public WindowType Window
        {
            get => _Args.Window;
            set
            {
                if (_Args.Window != value)
                {
                    _Args = _Args with { Window = value };

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    Model.ResetLine = true;
                    OnPropertyChanged();
                }
            }
        }

        public FFTNumber Number
        {
            get => _Args.N;
            set
            {
                if (_Args.N != value)
                {
                    _Args = _Args with { N = value };

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    Model.ResetLine = true;
                    OnPropertyChanged();
                }
            }
        }
        private FFTCoordUnit _OldUnit = FFTCoordUnit.Vrms;
        public FFTResultOpt ResultType
        {
            get => _Args.ResultType;
            set
            {
                if (_Args.ResultType != value)
                {
                    if (_Args.ResultType == FFTResultOpt.Ampltd)
                    {
                        _OldUnit = _Args.ResultUnit;
                        _Args = _Args with { ResultUnit = FFTCoordUnit.Vrms };
                    }
                    if (value == FFTResultOpt.Ampltd)
                    {
                        _Args = _Args with { ResultUnit = _OldUnit };
                    }

                    _Args = _Args with { ResultType = value };

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    Model.InitFlag = true;
                    Model.ResetLine = true;
                    OnPropertyChanged();
                }
            }
        }

        public FFTCoordUnit ResultUnit
        {
            get => _Args.ResultUnit;
            set
            {
                if (_Args.ResultUnit != value)
                {
                    _Args = _Args with { ResultUnit = value };

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    Model.InitFlag = true;
                    Model.ResetLine = true;
                    OnPropertyChanged();
                }
            }
        }

        public FFTPhaseUnit PhaseUnit
        {
            get => _Args.PhaseUnit;
            set
            {
                if (_Args.PhaseUnit != value)
                {
                    _Args = _Args with { PhaseUnit = value };

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    Model.InitFlag = true;
                    Model.ResetLine = true;
                    OnPropertyChanged();
                }
            }
        }




        #region 谱线

        #region 正常
        public Boolean NormalLine
        {
            get { return Model.NormalLine; }
            set
            {
                if (Model.NormalLine != value)
                {
                    Model.NormalLine = value;
                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                }
            }
        }

        public PickMode NormalLinePickMode
        {
            get { return Model.NormalLinePickMode; }
            set
            {
                Model.NormalLinePickMode = value;
            }
        }

        public Color NormalLineColor => Model.NormalLineColor;

        //public WfmPack? PackNormal
        //{
        //    get;
        //    protected set;
        //}

        //public WfmVuDatabase VuDatabaseNormal
        //{
        //    get;
        //} = new();

        #endregion

        #region 最大值保持

        public Boolean MaxHoldLine
        {
            get { return Model.MaxHoldLine; }
            set
            {
                if (Model.MaxHoldLine != value)
                {
                    Model.MaxHoldLine = value;
                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                }
            }
        }

        public PickMode MaxHoldLinePickMode
        {
            get { return Model.MaxHoldLinePickMode; }
            set
            {
                Model.MaxHoldLinePickMode = value;
            }
        }

        public Color MaxHoldLineColor => Model.MaxHoldLineColor;

        //public WfmPack? PackMaxHold
        //{
        //    get;
        //    protected set;
        //}

        //public WfmVuDatabase VuDatabaseMaxHold
        //{
        //    get;
        //} = new();

        #endregion

        #region 最小值保持

        public Boolean MinHoldLine
        {
            get { return Model.MinHoldLine; }
            set
            {
                if (Model.MinHoldLine != value)
                {
                    Model.MinHoldLine = value;
                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                }
            }
        }

        public PickMode MinHoldLinePickMode
        {
            get { return Model.MinHoldLinePickMode; }
            set
            {
                Model.MinHoldLinePickMode = value;
            }
        }

        public Color MinHoldLineColor => Model.MinHoldLineColor;

        //public WfmPack? PackMinHold
        //{
        //    get;
        //    protected set;
        //}

        //public WfmVuDatabase VuDatabaseMinHold
        //{
        //    get;
        //} = new();

        #endregion

        #region 平均

        public Int32 AverageTimes
        {
            get { return Model.AverageTimes; }
            set
            {
                Model.AverageTimes = value;
            }
        }

        public Boolean AverageLine
        {
            get { return Model.AverageLine; }
            set
            {
                if (Model.AverageLine != value)
                {
                    Model.AverageLine = value;
                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                }
            }
        }

        public PickMode AverageLinePickMode
        {
            get { return Model.AverageLinePickMode; }
            set
            {
                Model.AverageLinePickMode = value;
            }
        }

        public Color AverageLineColor => Model.AverageLineColor;

        //public WfmPack? PackAverage
        //{
        //    get;
        //    protected set;
        //}

        //public WfmVuDatabase VuDatabaseAverage
        //{
        //    get;
        //} = new();

        #endregion
        #endregion
        #region 数据
        public WfmPack? PackNormal => Model.PackNormal;

        public WfmVuDatabase VuDatabaseNormal
        {
            get => Model.VuDatabaseNormal;
        }

        public WfmPack? PackMaxHold => Model.PackMaxHold;

        public WfmVuDatabase VuDatabaseMaxHold
        {
            get => Model.VuDatabaseMaxHold;
        }

        public WfmPack? PackMinHold => Model.PackMinHold;

        public WfmVuDatabase VuDatabaseMinHold
        {
            get => Model.VuDatabaseMinHold;
        }

        public WfmPack? PackAverage => Model.PackAverage;

        public WfmVuDatabase VuDatabaseAverage
        {
            get => Model.VuDatabaseAverage;
        }

        #endregion
        public override String Description => $"FFT({Source})";

        public override String MakeFormula()
        {
            return $"{MathType.FFT}:{MakeFormula(_Args)}";
        }

        #region Validity And Configuration
        internal sealed record FftArgs(ChannelId Source, WindowType Window, FFTResultOpt ResultType, FFTNumber N, FFTCoordUnit ResultUnit, FFTPhaseUnit PhaseUnit);

        private static readonly String _PreFormula = "Execute.FFT(";
        internal static FftArgs ParseFormula(String formula)
        {
            var exp = formula;
            if (MathArgPrsnt.TryParse(exp, out var arg))
            {
                if (MathType.FFT != arg.Value.ExpType)
                    return new(ChannelId.C1, WindowType.Hamming, FFTResultOpt.Ampltd, FFTNumber.Num1K, FFTCoordUnit.dBm, FFTPhaseUnit.Degree);
                exp = arg.Value.Exp;
            }

            if (exp[.._PreFormula.Length] == _PreFormula)
            {
                var substr = exp.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                return new(
                    Enum.Parse<ChannelId>(substr[1]),
                    Enum.Parse<WindowType>(substr[2][(substr[2].IndexOf('.') + 1)..]),
                    Enum.Parse<FFTResultOpt>(substr[3][(substr[3].IndexOf('.') + 1)..]),
                    Enum.Parse<FFTNumber>(substr[4]),
                    Enum.Parse<FFTCoordUnit>(substr[5][(substr[5].IndexOf('.') + 1)..]),
                    Enum.Parse<FFTPhaseUnit>(substr[6][(substr[6].IndexOf('.') + 1)..]));
            }
            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"The formula '{formula}' is not a correct FFT(...) expression.", LogLevel.Warn));
            throw new ArgumentException($"The formula '{formula}' is not a correct FFT(...) expression.");
        }

        internal static String MakeFormula(FftArgs fa)
        {
            return $"{_PreFormula}{fa.Source}, {nameof(WindowType)}.{fa.Window}, {nameof(FFTResultOpt)}.{fa.ResultType}, {(Int32)(fa.N)}, {nameof(FFTCoordUnit)}.{fa.ResultUnit},{nameof(FFTPhaseUnit)}.{fa.PhaseUnit})";
        }

        internal static WfmProperties Config(MathModel mch, String exp, Vector? res)
        {
            FftArgs fa = ParseFormula(exp);

            Double cscale = 1;
            Double tscale = 1;

            if (DsoModel.Default.TryGetChannel(fa.Source, out var sch) && sch != null)
            {
                if (sch.Pack is not null)
                {
                    cscale = sch.Pack.Properties.ChnlScale.Value;
                    tscale = sch.Sampling.PosIdxPerDiv / (sch.Pack.Properties.TmbScale.Value * 1E-6 * Constants.VIS_XDIVS_NUM) * 1E6;
                }
                else
                {
                    cscale = sch.Conditioning.Scale;
                    tscale = sch.Sampling.PosIdxPerDiv / (sch.Sampling.Scale * 1E-6 * Constants.VIS_XDIVS_NUM) * 1E6;
                }
            }

            var ru = fa.ResultType switch
            {
                FFTResultOpt.Power or FFTResultOpt.Psd => FFTCoordUnit.dBmV,
                FFTResultOpt.Phase => FFTCoordUnit.Vrms,
                _ => fa.ResultUnit,
            };

            if (fa.ResultType != FFTResultOpt.Imaginary && fa.ResultType != FFTResultOpt.Real)
            {
                mch.Conditioning.InitialScale = (0, 1E4);
                mch.Conditioning.ScaleMaxIndex = 3;
                mch.Conditioning.ScaleMinIndex = -3;
                mch.Conditioning.Prefix = Prefix.Milli;
            }
            mch.Sampling.ScaleMaxIndex = -4;
            mch.Sampling.ScaleMinIndex = -26;
            mch.Sampling.Prefix = Prefix.Micro;

            if (mch.InitFlag)
            {
                (Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, res);
                var args = mch.Args;
                var conditionIndex = scale.VScaleIndex;
                if (args is MathFftArg)
                {
                    var fftArgs = (MathFftArg)args;
                    if (mch.ResetFFT == true)
                    {
                        fftArgs.ResultType = FFTResultOpt.Ampltd;
                        fftArgs.ResultUnit = FFTCoordUnit.dBm;
                        fftArgs.Number = FFTNumber.Num8K;
                        fftArgs.Window = WindowType.Rectangle;
                        mch.Sampling.InitialScale = (0, 4E16);

                        mch.Conditioning.ScaleIndex = 1;
                        mch.Sampling.ScaleIndex = -4;
                        mch.Conditioning.PosIndex = 3500;
                        mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                        if (DsoModel.Default.TryGetChannel(fa.Source, out var ssch) && ssch != null)
                        {
                            if (ssch.Pack is not null)
                            {
                                var samplerate = 1 / ssch.Pack.Properties.SampInterval;
                                mch.FrequencyAdapter.ValueSpan = samplerate / 2 * 1E6;
                                mch.FrequencyAdapter.ValueCenter = mch.FrequencyAdapter.ValueSpan / 2;
                            }
                        }
                        else
                        {
                            mch.FrequencyAdapter.ValueCenter = 4E15;
                            mch.FrequencyAdapter.ValueSpan = 4E15;
                        }
                        fftArgs.MakeFormula();
                        mch.ResetFFT = false;
                        mch.IsAutoUnit = true;
                    }
                    else
                    {

                        if ((fftArgs.ResultType == FFTResultOpt.Ampltd) &&
                            fftArgs.ResultUnit != FFTCoordUnit.Vrms &&
                            fftArgs.ResultUnit != FFTCoordUnit.dBmV)
                        {
                            conditionIndex = conditionIndex + 6;

                        }
                        if (true)
                        {
                            if (fa.ResultType == FFTResultOpt.Imaginary || fa.ResultType == FFTResultOpt.Real)
                            {
                                if (DsoModel.Default.TryGetChannel(fftArgs.Source, out var chnl))
                                {
                                    if (chnl is AnalogModel am && am != null)
                                    {
                                        mch.Conditioning.Prefix = am.Conditioning.Prefix;
                                        mch.Conditioning.InitialScale = am.Conditioning.InitialScale;
                                        mch.Conditioning.ScaleMaxIndex = (Int32)am.Conditioning.ScaleMaxIndex;
                                        mch.Conditioning.ScaleMinIndex = (Int32)am.Conditioning.ScaleMinIndex;
                                    }
                                }
                                mch.Conditioning.PosIndex = 0;
                            }
                            else
                            {
                                mch.Conditioning.PosIndex = 3500;
                                mch.Conditioning.ScaleIndex = 0;
                            }
                            mch.Sampling.InitialScale = (0, 2E16);
                            if (fftArgs.ResultType == FFTResultOpt.Ampltd && fftArgs.ResultUnit == FFTCoordUnit.Vrms)
                            {
                                mch.Conditioning.ScaleIndex = -6;
                                mch.Conditioning.PosIndex = -3500;

                            }
                            if (fftArgs.ResultType == FFTResultOpt.Phase)
                            {
                                mch.Conditioning.ScaleIndex = 1;
                                mch.Conditioning.PosIndex = 0;
                            }
                            mch.Sampling.ScaleIndex = -4;

                            mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                            if (DsoModel.Default.TryGetChannel(fa.Source, out var ssch))
                            {
                                if (ssch.Pack is not null)
                                {
                                    var samplerate = 1 / ssch.Pack.Properties.SampInterval;
                                    mch.FrequencyAdapter.ValueSpan = samplerate / 2*1E6;
                                    mch.FrequencyAdapter.ValueCenter = mch.FrequencyAdapter.ValueSpan / 2;
                                }
                            }
                            else
                            {
                                mch.FrequencyAdapter.ValueCenter = 4E15;
                                mch.FrequencyAdapter.ValueSpan = 4E15;
                            }

                            fftArgs.Number = FFTNumber.Num8K;
                        }
                        else
                        {
                            mch.Conditioning.ScaleIndex = conditionIndex;
                            mch.Sampling.ScaleIndex = scale.HScaleIndex;
                            mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;
                            mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                        }
                    }
                }
                else
                {
                    mch.Conditioning.ScaleIndex = conditionIndex;
                    mch.Sampling.ScaleIndex = scale.HScaleIndex;

                    mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;
                    mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                }




                mch.InitFlag = false;
            }
            mch.Conditioning.Unit = mch.IsAutoUnit ? (res?.YUnit ?? "?") : mch.CustomUnit;

            mch.Sampling.Unit = res?.XUnit ?? "?";

            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = ((Int32)mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                TmbPosition = (0, mch.Sampling.GetPosition(0)),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),
            };

            prop.SampInterval = res?.SampInterval ?? 1;
            return prop;
        }


        #endregion

    }
}

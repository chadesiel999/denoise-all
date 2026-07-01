using EventBus;
using System;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System.ComponentModel;
using System.Linq;
using NPOI.SS.Formula.Functions;

namespace ScopeX.Core
{
    [Description(nameof(MathType.Filter))]
    public class MathFilterArg : MathArgPrsnt
    {
        public MathFilterArg(MathPrsnt mp, ChannelId id, String formula) : base(mp, id, MathType.Filter)
        {
            _Args = ParseFormula(formula);
            _CoeffKey = GetCoeffKey(id.ToString());
        }

        private FilterArgs _Args;

        private readonly String _CoeffKey;

        public ChannelId Source
        {
            get => _Args.Source;
            set
            {
                if (_Args.Source != value)
                {
                    _Args = _Args with { Source = value };
                    if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(value, out var p))
                    {
                        if (p is AnalogPrsnt ap)
                        {
                            SampleInterval = ap.Pack?.Properties?.SampInterval;
                        }
                        else if (p is ReferencePrsnt rp)
                        {
                            SampleInterval = rp.Pack?.Properties?.SampInterval;
                        }
                    }

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsQuickDesign
        {
            get => _Args.CoeffKey != "\"" + _CoeffKey + "\"";
            set
            {
                _Args = _Args with { CoeffKey = value ? "\"\"" : ("\"" + _CoeffKey + "\"") };

                Model.Formula = MakeFormula();
                Dispatcher.SoftReset();
                OnPropertyChanged();
            }
        }

        public Boolean CustomFilterEnable
        {
            get => !IsQuickDesign;
            set => IsQuickDesign = !value;
        }

        public FilterResponseType RespType
        {
            get => _Args.RespType;
            set
            {
                if (_Args.RespType != value)
                {
                    _Args = _Args with { RespType = value };

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// FIR还是IIR
        /// </summary>
        private FilterType _FilterType;
        public FilterType FilterType
        {
            get => _FilterType;
            set
            {
                if(_FilterType!=value)
                {
                    _FilterType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 分子
        /// </summary>
        private Double[]? _Numerator;
        public Double[]? Numerator
        {
            get => _Numerator;
            set
            {
                if (_Numerator != value)
                {
                    _Numerator = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 分母
        /// </summary>
        private Double[]? _Denominator;
        public Double[]? Denominator
        {
            get => _Denominator;
            set
            {
                if (_Denominator != value)
                {
                    _Denominator = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int64 Freq1
        {
            get => _Args.Freq1;
            set
            {
                value = ValidateFreq(value);

                if (_Args.Freq1 != value)
                {
                    _Args = _Args with { Freq1 = value };

                    Model.Formula = MakeFormula();
                    //Dispatcher.SoftReset();
                    OnPropertyChanged();
                }
            }
        }

        public Double Freq1BymHz
        {
            get => Freq1 * 1000D;
            set => Freq1 = (Int64)(value / 1000D);
        }


        public static readonly Int64 MaxFreq = 5_000_000;
        public static readonly Int64 MinFreq = 1;
        public Int64 MaxFreq2 { get; set; }
        public Int64 MinFreq2 { get; set; }

        public Int64 Freq2
        {
            get => _Args.Freq2;
            set
            {
                value = ValidateFreq(value);

                if (_Args.Freq2 != value)
                {
                    _Args = _Args with { Freq2 = value };

                    Model.Formula = MakeFormula();
                    //Dispatcher.SoftReset();
                    OnPropertyChanged();
                }
            }
        }

        private Double? _SampleInterval;
        public Double? SampleInterval
        {
            get => _SampleInterval;
            set
            {
                if (_SampleInterval != value)
                {
                    _SampleInterval = value;
                    OnPropertyChanged();
                }
            }
        }

        public Double Freq2BymHz
        {
            get => Freq2 * 1000D;
            set => Freq2 = (Int64)(value / 1000D);
        }

        private static Int64 GetStepFreq(Int64 value)
        {
            var n = (Int64)Math.Log10(value);
            if (n > 0)
                n--;
            return (Int64)Math.Pow(10, n);
        }

        private static Int64 ValidateFreq(Int64 value)
        {
            if (value > MaxFreq)
            {
                value = MaxFreq;
                WeakTip.Default.Write("Freq", MsgTipId.GreatethanMax, false, "", 1);
            }
            else if (value < MinFreq)
            {
                value = MinFreq;
                WeakTip.Default.Write("Freq", MsgTipId.LessthanMin, false, "", 1);
            }

            return value;
        }

        public void AdjFreq1(Int32 delta) => Freq1 += delta;

        public void AdjFreq2(Int32 delta) => Freq2 += delta;


        public override String Description => $"Filter({Source})";

        public override String MakeFormula()
        {
            return $"{MathType.Filter}:{MakeFormula(_Args)}";
        }

        #region Validity And Configuration
        public static String GetCoeffKey(String name) => $"{name}Filter";

        internal sealed record FilterArgs(ChannelId Source, String CoeffKey, Int64 Freq1, Int64 Freq2, FilterResponseType RespType);

        internal static FilterArgs ParseFormula(String formula)
        {
            var exp = formula;
            if (MathArgPrsnt.TryParse(exp, out var arg))
            {
                if (MathType.Filter != arg.Value.ExpType)
                {
                    return new(ChannelId.C1, "", 1000, 2000, FilterResponseType.LowPass);
                }
                exp = arg.Value.Exp;
            }

            String name = "Execute.Filter(";
            if (exp.Substring(0, name.Length) == name)
            {
                var substr = exp.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                try
                {
                    return new(
                        Enum.Parse<ChannelId>(substr[1]),
                        substr[2],
                        Int64.Parse(substr[3]),
                        Int64.Parse(substr[4]),
                        Enum.Parse<FilterResponseType>(substr[5][(substr[5].IndexOf('.') + 1)..]));
                }
                catch
                {
                    return new(ChannelId.C1, "", 1000, 2000, FilterResponseType.LowPass);
                }
            }
            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"The formula '{formula}' is not a correct Filter(...) expression.", LogLevel.Warn));
            throw new ArgumentException($"The formula '{formula}' is not a correct Filter(...) expression.");
        }

        internal static String MakeFormula(FilterArgs fa)
        {
            return $"Execute.Filter({fa.Source}, {fa.CoeffKey}, {fa.Freq1}, {fa.Freq2}, {nameof(FilterResponseType)}.{fa.RespType})";
        }

        internal static WfmProperties Config(MathModel mch, String exp, Vector? vec)
        {
            var fa = ParseFormula(exp);

            Double cscale = 1;
            Double tscale = 1;
            Double start = 0;

            if (DsoModel.Default.TryGetChannel(fa.Source, out var sch))
            {
                if (sch.Pack is not null)
                {
                    cscale = sch.Pack.Properties.ChnlScale.Value;
                    tscale = sch.Pack.Properties.TmbScale.Value;
                    start = sch.Pack.Properties.VuStartIndex;
                }
                else
                {
                    cscale = sch.Conditioning.Scale;
                    tscale = sch.Sampling.Scale;
                }
            }

            (Double Index, Double Value) tmbposition = (Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX));

            if (mch.InitFlag)
            {
                mch.Conditioning.InitialScale = (0, cscale);
                mch.Conditioning.ScaleMinIndex = -10;
                mch.Conditioning.ScaleMaxIndex = 10;
                mch.Conditioning.Prefix = Prefix.Milli;

                mch.Sampling.InitialScale = (0, tscale);
                mch.Sampling.ScaleMinIndex = -20;
                mch.Sampling.ScaleMaxIndex = 20;
                mch.Sampling.Prefix = Prefix.Micro;
            }
            else
            {
                mch.Conditioning.SetInitScaleValue(0, cscale, -20, 20, false);
                mch.Sampling.SetInitScaleValue(0, tscale, -30, 30, mch.WindowId == DsoModel.Default.AnalogChnls.First().WindowId);
                if (sch != null && sch.Pack != null && sch.Pack.Properties != null)
                {
                    if (mch.WindowId == sch.WindowId)
                    {
                        var scale = DsoPrsnt.DefaultDsoPrsnt.Timebase.GetScale(DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleIndex);
                        if (mch.Sampling.Scale != scale)
                        {
                            mch.Sampling.Scale = scale;
                        }
                        mch.Sampling.PosIndex = DsoPrsnt.DefaultDsoPrsnt.Timebase.PosIndexBymDiv;
                        tmbposition = sch.Pack.Properties.TmbPosition;
                    }
                    else
                    {
                        var index = (TriggerPrsnt.State == SysState.Auto || TriggerPrsnt.State == SysState.Triged) ? sch.Pack.Properties.TmbPosition.Index : Constants.DEF_XPOS_IDX;
                        tmbposition = (index, mch.Sampling.GetPosition(index));
                        var tmb = mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX);
                        start = (-tmb) / mch.Sampling.Scale * Constants.IDX_PER_XDIV;
                    }
                }
            }


            if (mch.InitFlag)
            {
                (Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                mch.Conditioning.ScaleIndex = mch.IsSwitchWindow ? mch.Conditioning.ScaleIndex : scale.VScaleIndex;
                mch.Sampling.ScaleIndex = scale.HScaleIndex;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
            }

            mch.Conditioning.Unit = mch.IsAutoUnit ? (vec?.YUnit ?? "?") : mch.CustomUnit;
            mch.Sampling.Unit = vec?.XUnit ?? "?";

            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = ((Int32)mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                TmbPosition = tmbposition,
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),

                VuStartIndex = start,
            };

            prop.SampInterval = vec?.SampInterval ?? 1;

            return prop;

            //Boolean init = false;
            //if (mch.Pack is not null)
            //{
            //    if (mch.Pack.Properties.Stamp < wfmpkg?.Properties.Stamp)
            //        init = true;
            //}
            //else
            //    init = true;

            //if (!init)
            //    return;
        }

        #endregion
    }
}

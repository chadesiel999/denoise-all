using EventBus;
using System;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System.ComponentModel;
using System.Linq;

namespace ScopeX.Core
{
    [Description(nameof(MathType.ERes))]
    public class MathEResArg :MathArgPrsnt
    {
        public MathEResArg(MathPrsnt mp, ChannelId id, String formula) : base(mp, id,MathType.ERes)
        {
            _Args = ParseFormula(formula);
        }

        private EResArgs _Args;

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

        public Double EnhancedBits
        {
            get => _Args.EnhancedBits;
            set
            {
                value = ValidateEnhancedBits(value);
                if (_Args.EnhancedBits != value)
                {
                    _Args = _Args with { EnhancedBits = value };

                    Model.Formula = MakeFormula();
                    //Dispatcher.SoftReset();
                    OnPropertyChanged();
                }
            }
        }

        private static Double ValidateEnhancedBits(Double value)
        {
            value = Math.Round(value / Constants.StepEnhancedBit, MidpointRounding.AwayFromZero) * Constants.StepEnhancedBit;
            if (value > Constants.MaxEnhancedBits)
                value = Constants.MaxEnhancedBits;
            else if (value < Constants.MinEnhancedBits)
                value = Constants.MinEnhancedBits;
            return value;
        }

        public void AdjEnhancedBits(Int32 step)
        {
            EnhancedBits += step * Constants.StepEnhancedBit;
        }

        public override String Description => $"ERes({Source})";

        public override String MakeFormula()
        {
            return $"{MathType.ERes}:{MakeFormula(_Args)}";
        }

        #region Validity And Configuration
        internal sealed record EResArgs(ChannelId Source, Double EnhancedBits);

        internal static EResArgs ParseFormula(String formula)
        {
            var exp = formula;
            if (MathArgPrsnt.TryParse(formula, out var arg))
            {
                if (MathType.ERes != arg.Value.ExpType)
                    return new(ChannelId.C1, Constants.MinEnhancedBits);
                exp = arg.Value.Exp;
            }

            String name = "Execute.ERes(";
            if (exp.Substring(0, name.Length) == name)
            {
                var substr = exp.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                return new(Enum.Parse<ChannelId>(substr[1]), Double.Parse(substr[2]));
            }

            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"The formula '{formula}' is not a correct ERes(...) expression.", LogLevel.Warn));
            throw new ArgumentException($"The formula '{formula}' is not a correct ERes(...) expression.");
        }

        internal static String MakeFormula(EResArgs era)
        {
            return $"Execute.ERes({era.Source}, {era.EnhancedBits})";
        }

        internal static WfmProperties Config(MathModel mch, String exp, Vector? vec)
        {
            var era = ParseFormula(exp);

            Double cscale = 1;
            Double tscale = 1;
            Double start = 0;
            if (DsoModel.Default.TryGetChannel(era.Source, out var sch))
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

            if (mch.InitFlag)
            {
                mch.Conditioning.InitialScale = (0, cscale);
                mch.Conditioning.ScaleMinIndex = -10;
                mch.Conditioning.ScaleMaxIndex = 10;
                mch.Conditioning.Prefix = Prefix.Milli;

                mch.Sampling.InitialScale = (0, tscale);
                mch.Sampling.ScaleMinIndex = -3;
                mch.Sampling.ScaleMaxIndex = 1;
                //mch.Sampling.PosDefIndex = Constants.DEF_XPOS_IDX;
                mch.Sampling.Prefix = Prefix.Micro;
            }
            else
            {
                mch.Conditioning.SetInitScaleValue(0, cscale, -10, 10, false);
                mch.Sampling.SetInitScaleValue(0, tscale, -5, 5, mch.WindowId == DsoModel.Default.AnalogChnls.First().WindowId);
                if(mch.WindowId == DsoModel.Default.AnalogChnls.First().WindowId)
                {
                    mch.Sampling.Scale = DsoPrsnt.DefaultDsoPrsnt.Timebase.GetScale(DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleIndex);
                }
            }

            if (mch.InitFlag)
            {
                (Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                mch.Conditioning.ScaleIndex = mch.IsSwitchWindow ? mch.Conditioning.ScaleIndex : scale.VScaleIndex;
                mch.Sampling.ScaleIndex = scale.HScaleIndex;
                //mch.Conditioning.ScaleIndex = 0;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;

                //mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
                mch.IsSwitchWindow = false;
                //ScaleFit(mch, vec);
            }

            mch.Conditioning.Unit = mch.IsAutoUnit ? (vec?.YUnit ?? "?") : mch.CustomUnit;

            mch.Sampling.Unit = vec?.XUnit ?? "?";

            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = ((Int32)mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                TmbPosition = (Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX)),
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

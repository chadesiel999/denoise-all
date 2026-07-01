using System;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System.ComponentModel;

namespace ScopeX.Core
{
    [Description(nameof(MathType.Binary))]
    public class MathBinaryArg : MathArgPrsnt
    {
        public MathBinaryArg(MathPrsnt mp,ChannelId id, String formula) : base(mp,id, MathType.Binary)
        {
            _Args = ParseFormula(formula);
        }

        private BinaryArgs _Args;

        public ChannelId Source1st
        {
            get => _Args.Source1st;
            set
            {
                if (_Args.Source1st != value)
                {
                    _Args = _Args with { Source1st = value };

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    OnPropertyChanged();
                }
            }
        }

        public ChannelId Source2nd
        {
            get => _Args.Source2nd;
            set
            {
                if (_Args.Source2nd != value)
                {
                    _Args = _Args with { Source2nd = value };

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    OnPropertyChanged();
                }
            }
        }

        public MathBinaryType BinaryOp
        {
            get => _Args.BinaryOp;
            set
            {
                if (_Args.BinaryOp != value)
                {
                    _Args = _Args with { BinaryOp = value };

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    OnPropertyChanged();
                }
            }
        }

        public override String Description => BinaryOp switch
        {
            MathBinaryType.Add => $"{Source1st}+{Source2nd}",
            MathBinaryType.Subtract => $"{Source1st}-{Source2nd}",
            MathBinaryType.Multiply => $"{Source1st}×{Source2nd}",
            MathBinaryType.Divide => $"{Source1st}÷{Source2nd}",
            _ => "?",
        };

        public override String MakeFormula()
        {
            return $"{Type}:{MakeFormula(_Args)}";
        }

        #region Validity And Configuration
        internal sealed record BinaryArgs(ChannelId Source1st, MathBinaryType BinaryOp, ChannelId Source2nd);

        internal static BinaryArgs ParseFormula(String formula)
        {
            var exp = formula;
            if (TryParse(formula, out var arg))
            {
                if (MathType.Binary != arg.Value.ExpType)
                {
                    return new(ChannelId.C1, MathBinaryType.Add, ChannelId.C2);
                }
                exp = arg.Value.Exp;
            }

            var opidx = exp.IndexOfAny(new[] { '+', '-', '*', '/' });
            if (opidx > 0)
            {
                return new(
                    Enum.Parse<ChannelId>(exp[0..opidx]),
                    exp[opidx] switch
                    {
                        '+' => MathBinaryType.Add,
                        '-' => MathBinaryType.Subtract,
                        '*' => MathBinaryType.Multiply,
                        '/' => MathBinaryType.Divide,
                        _ =>
                        //EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"The formula '{formula}' is not correct binary expression.", LogLevel.Warn));
                        throw new NotImplementedException($"The formula '{formula}' is not correct binary expression."),
                    },
                    Enum.Parse<ChannelId>(exp[(opidx + 1)..]));
            }

            throw new ArgumentException($"The formula '{formula}' is not correct binary expression.");
        }

        internal static String MakeFormula(BinaryArgs ba)
        {
            return ba.Source1st + ba.BinaryOp switch
            {
                MathBinaryType.Add => "+",
                MathBinaryType.Subtract => "-",
                MathBinaryType.Multiply => "*",
                MathBinaryType.Divide => "/",
                _ => "?",
            } + ba.Source2nd;
        }

        internal static WfmProperties Config(MathModel mch, String exp, Vector? vec)
        {
            var ba = ParseFormula(exp);

            Double lcscale = 1, rcscale = 1;
            Double ltscale = 1, rtscale = 1;
            Double lstart = 0, rstart = 0;

            if (DsoModel.Default.TryGetChannel(ba.Source1st, out var lsch))
            {
                if (lsch.Pack is not null)
                {
                    lcscale = lsch.Pack.Properties.ChnlScale.Value;
                    ltscale = lsch.Pack.Properties.TmbScale.Value;
                    lstart = lsch.Pack.Properties.VuStartIndex;
                }
                else
                {
                    lcscale = lsch.Conditioning.Scale;
                    ltscale = lsch.Sampling.Scale;
                }
            }
            if (DsoModel.Default.TryGetChannel(ba.Source2nd, out var rsch))
            {
                if (rsch.Pack is not null)
                {
                    rcscale = rsch.Pack.Properties.ChnlScale.Value;
                    rtscale = rsch.Pack.Properties.TmbScale.Value;
                    rstart = rsch.Pack.Properties.VuStartIndex;
                }
                else
                {
                    rcscale = rsch.Conditioning.Scale;
                    rtscale = rsch.Sampling.Scale;
                }
            }

            (Double Index, Double Value) tmbposition = (Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX));
            Double vustartindex = 0;

            if (mch.InitFlag)
            {
                mch.Conditioning.InitialScale = (0, Math.Max(lcscale, rcscale));
                mch.Conditioning.ScaleMinIndex = -10;
                mch.Conditioning.ScaleMaxIndex = 10;
                mch.Conditioning.Prefix = Prefix.Milli;
                mch.Sampling.InitialScale = (0, Math.Max(ltscale, rtscale));
                mch.Sampling.ScaleMinIndex = -10;
                mch.Sampling.ScaleMaxIndex = 10;
            }
            else if (rsch != null && rsch is AnalogModel am && am != null)
            {
                mch.Conditioning.SetInitScaleValue(0, Math.Max(lcscale, rcscale), -20, 20, false);
                mch.Sampling.SetInitScaleValue(0, Math.Max(ltscale, rtscale), -30, 30, mch.WindowId == am.WindowId);
                if (mch.WindowId == am.WindowId)
                {
                    mch.Sampling.IgnorePositionMaxMin = IgnoreScaleLimit.Both;
                    mch.Sampling.IgnoreScaleMaxMin = IgnoreScaleLimit.Both;
                    var scale = DsoPrsnt.DefaultDsoPrsnt.Timebase.GetScale(DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleIndex);
                    if (mch.Sampling.Scale != scale)
                    {
                        mch.Sampling.Scale = scale;
                    }
                    if (lsch?.Pack?.Properties != null)
                    {
                        mch.Sampling.PosIndex = DsoPrsnt.DefaultDsoPrsnt.Timebase.PosIndexBymDiv;
                        tmbposition = lsch.Pack.Properties.TmbPosition;
                        vustartindex = lsch.Pack.Properties.VuStartIndex;
                    }
                }
                else
                {
                    if (mch.Sampling.IgnorePositionMaxMin == IgnoreScaleLimit.Both)
                    {
                        mch.Sampling.IgnorePositionMaxMin = IgnoreScaleLimit.None;
                        mch.Sampling.PosIndex = mch.Sampling.PosIndex;
                    }
                    if (mch.Sampling.IgnoreScaleMaxMin == IgnoreScaleLimit.Both)
                    {
                        mch.Sampling.IgnoreScaleMaxMin = IgnoreScaleLimit.None;
                        mch.Sampling.Scale = mch.Sampling.Scale;
                    }
                    var index = (TriggerPrsnt.State == SysState.Auto || TriggerPrsnt.State == SysState.Triged) && lsch?.Pack?.Properties != null ? lsch.Pack.Properties.TmbPosition.Index : DsoPrsnt.DefaultDsoPrsnt.Timebase.PosIndexBymDiv;
                    tmbposition = (index, mch.Sampling.GetPosition(index));
                    var tmb = mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX);
                    vustartindex = (-tmb) / mch.Sampling.Scale * Constants.IDX_PER_XDIV;
                }
            }

            if (mch.InitFlag)
            {
                (Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                mch.Conditioning.ScaleIndex = mch.IsSwitchWindow ? mch.Conditioning.ScaleIndex : scale.VScaleIndex;
                mch.Sampling.ScaleIndex = scale.HScaleIndex;
                mch.Sampling.Prefix = Prefix.Micro;


                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;

                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;

                mch.InitFlag = false;
                mch.IsSwitchWindow = false;
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
                //TmbScale = mch.Sampling.ScaleIndex,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),

                VuStartIndex = vustartindex,
            };

            prop.SampInterval = vec?.SampInterval ?? 1;
            return prop;
        }
        #endregion


    }
}

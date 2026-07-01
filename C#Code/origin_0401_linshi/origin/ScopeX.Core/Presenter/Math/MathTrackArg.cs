using EventBus;
using System;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;

namespace ScopeX.Core
{
    [Description(nameof(MathType.Track))]
    public class MathTrackArg : MathArgPrsnt
    {
        public MathTrackArg(MathPrsnt mp, ChannelId id, String formula) : base(mp, id, MathType.Track)
        {
            _Args = ParseFormula(formula);
        }

        private TrackArgs _Args;

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

        public override String Description => $"({Source})";

        public override String MakeFormula()
        {
            return $"{MathType.Track}:{MakeFormula(_Args)}";
        }

        #region Validity And Configuration
        internal sealed record TrackArgs(ChannelId Source);

        internal static TrackArgs ParseFormula(String formula)
        {
            var exp = formula;
            var sep = exp.IndexOf(":");
            if (sep >= 0)
            {
                if (MathType.Track != Enum.Parse<MathType>(exp[0..sep]))
                    return new(ChannelId.P1);
                exp = exp[(sep + 1)..];
            }

            String name = "Execute.Track(";
            if (exp.Substring(0, name.Length) == name)
            {
                var substr = exp.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                return new(Enum.Parse<ChannelId>(substr[1]));
            }

            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"The formula '{formula}' is not a correct Track(...) expression.", LogLevel.Warn));
            throw new ArgumentException($"The formula '{formula}' is not a correct Track(...) expression.");
        }

        internal static String MakeFormula(TrackArgs ta)
        {
            return $"Execute.Track({ta.Source}, 1000000)";//????
        }

        internal static WfmProperties Config(MathModel mch, String exp, Vector? vec)
        {
            var ta = ParseFormula(exp);

            Double cscale = 1;
            Double tscale = 1;
            Double start = 0;
            Prefix prefix = Prefix.Milli;
            MeasureItemModel selected = null;

            if (DsoModel.Default.TryGetChannel(ta.Source, out var sch))
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
            else if (ChannelIdExt.IsMeasure(ta.Source) && DsoModel.Default.Meas.SelectedItems[ta.Source - ChannelIdExt.MinPChId].MeasureType == MeasureType.Single)
            {
                selected = DsoModel.Default.Meas.SelectedItems[ta.Source - ChannelIdExt.MinPChId];
                var pfxunit = DsoModel.Default.Meas.Calc.GetPfxUnitString(ta.Source - ChannelIdExt.MinPChId);
                var src = DsoModel.Default.GetChannel(selected.Source);
                if (src.Pack is not null)
                {
                    cscale = src.Pack.Properties.ChnlScale.Value;
                    tscale = src.Pack.Properties.TmbScale.Value;
                    start = src.Pack.Properties.VuStartIndex;
                    if (pfxunit.Name == src.Pack.Properties.ChnlUnit.Name)
                    {
                        prefix = Prefix.Micro;
                    }

                    if (pfxunit.Name == $"{src.Pack.Properties.ChnlUnit.Name}{src.Pack.Properties.TmbUnit.Name}")
                    {
                        prefix = Prefix.Micro;
                    }
                }
                else
                {
                    cscale = src.Conditioning.Scale;
                    tscale = DsoModel.Default.Timebase.Scale;
                }

                if (selected.MeasureType == MeasureType.Composite)
                {
                    prefix = Prefix.Milli;
                }
            }

            if (mch.InitFlag)
            {
                mch.Conditioning.InitialScale = (0, cscale);               
                mch.Conditioning.Prefix = prefix;
            }

            mch.Sampling.InitialScale = ((Int32)DsoModel.Default.Timebase.ScaleIndex, tscale);
            mch.Sampling.ScaleMinIndex = (Int32)DsoModel.Default.Timebase.ScaleMinIndex;
            mch.Sampling.ScaleMaxIndex = (Int32)DsoModel.Default.Timebase.ScaleMaxIndex;
            mch.Sampling.Prefix = Prefix.Micro;

            if (mch.InitFlag)
            {
                (Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                mch.Conditioning.IgnoreScaleMaxMin = IgnoreScaleLimit.Both;
                mch.Conditioning.IgnorePositionMaxMin = IgnoreScaleLimit.Both;
                mch.Conditioning.ScaleIndex = mch.IsSwitchWindow ? mch.Conditioning.ScaleIndex : scale.VScaleIndex;
                mch.Conditioning.ScaleMaxIndex = 100;
                mch.Conditioning.ScaleMinIndex = -100;
                mch.Conditioning.PosMaxIndex = 10000000000;
                mch.Conditioning.PosMinIndex = -10000000000;
                mch.Sampling.ScaleIndex = scale.HScaleIndex;
                mch.Conditioning.PosIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
                mch.IsSwitchWindow = false;
            }

            AutoScale(mch, ta, vec, selected, prefix);

            mch.Conditioning.Unit = mch.IsAutoUnit ? (vec?.YUnit ?? "?") : mch.CustomUnit;

            if (vec != null)
            {
                mch.Sampling.Unit = vec?.XUnit ?? "?";
            }


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
        }

        private static void AutoScale(MathModel mch, TrackArgs ta, Vector? vec, MeasureItemModel? selected, Prefix prefix)
        {
            try
            {
                if (mch.AutoScale)
                {
                    IEnumerable<Double> data;
                    data = mch?.Pack?.Buffer != null ? mch?.Pack?.Buffer?.Cast<Double>()?.Skip<Double>((Int32)vec?.Elements?.GetLength(1))?.Select(x => x / 1E3)?.Concat(vec?.Elements?.Cast<Double>()) : vec?.Elements?.Cast<Double>();
                    data = data?.Where(x => Double.IsFinite(x));
                    if (data != null && data.Count() > 0)
                    {
                        var max = data.Max();
                        var min = data.Min();
                        if (min != null && max != null && max == min)
                        {
                            max = min * 1.1;
                            min = min * 0.9;
                        }
                        var delta = min != null && max != null ? Math.Abs((max - min)) : 0;
                        var ave = min != null && max != null ? (max + min) / 2 : 0;
                        var length = data.Count();
                        if (max != null && min != null)
                        {
                            Prefix valuepfx = Prefix.Empty;
                            if (vec != null)
                            {
                                if (selected != null && DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(selected.Source, out var chnl) && chnl?.Pack?.Properties != null && vec.YUnit == chnl?.Pack?.Properties.ChnlUnit.Name)
                                {
                                    valuepfx = Prefix.Milli;
                                }
                                if (selected != null && selected.MeasureType == MeasureType.Composite)
                                {
                                    valuepfx = Prefix.Micro;
                                }
                            }
                            if (selected != null && selected.MeasureType == MeasureType.Single && (selected.Name == "Area" || selected.Name == "CycArea"))
                            {
                                valuepfx = Prefix.Milli;
                                mch.Conditioning.Prefix = Prefix.Micro;
                            }
                            Double value = 10;
                            value = Quantity.ConvertByPrefix(value, mch.Conditioning.Prefix, Prefix.Empty);
                            var deltavalue = Quantity.ConvertByPrefix(delta, valuepfx, mch.Conditioning.Prefix);
                            var avevalue = Quantity.ConvertByPrefix(ave, valuepfx, mch.Conditioning.Prefix);
                            if (ta.Source == ChannelId.CYM)
                            {
                                avevalue = Quantity.ConvertByPrefix(ave, Prefix.Kilo, Prefix.Empty);
                            }
                            if (Math.Abs(deltavalue) > 1E-10)
                            {
                                var condiindex = mch.Conditioning.ScaleIndex;
                                var condiscale = mch.Conditioning.Scale;
                                var target = Math.Abs(deltavalue);
                                while (Math.Abs(deltavalue) > condiscale)
                                {
                                    condiindex++;
                                    condiscale = mch.Conditioning.GetScale(condiindex);
                                }
                                while (Math.Abs(deltavalue) < condiscale * 1.5)
                                {
                                    condiindex--;
                                    condiscale = mch.Conditioning.GetScale(condiindex);
                                }

                                if (Math.Abs(deltavalue) > condiscale)
                                {
                                    condiindex++;
                                }

                                mch.Conditioning.ScaleIndex = condiindex;
                            }
                            else
                            {
                                mch.Conditioning.Scale = Quantity.ConvertByPrefix(1, Prefix.Empty, prefix);
                            }
                            var position = -Math.Ceiling(avevalue / mch.Conditioning.Scale * mch.Conditioning.PosIdxPerDiv);
                            if (Math.Abs(position - mch.Conditioning.PosIndex) > mch.Conditioning.PosIdxPerDiv)
                            {
                                mch.Conditioning.PosIndex = position;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(nameof(MathTrendArg), new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
        }
        #endregion
    }
}

using EventBus;
using System;
using System.Collections.Generic;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.MathExt;
using System.ComponentModel;
using System.Linq;
using NPOI.SS.Formula.Functions;
using System.Diagnostics;
using ScopeX.Measure;

namespace ScopeX.Core
{
    [Description(nameof(MathType.Trend))]
    public class MathTrendArg : MathArgPrsnt
    {
        public MathTrendArg(MathPrsnt mp, ChannelId id, String formula) : base(mp, id, MathType.Trend)
        {
            _Args = ParseFormula(formula);
            _TrendValues = _Args.Values;
        }

        private TrendArgs _Args;

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


        public Int32 DataLength = 0;

        private List<Double> _TrendValues;
        private static Object _LockObject = new Object();

        public override String Description => $"({Source})";

        public override String MakeFormula()
        {
            return $"{MathType.Trend}:{MakeFormula(_Args)}";
        }

        public void AddValues(IEnumerable<Double> values)
        {
            lock (_LockObject)
            {
                _TrendValues.AddRange(values);
                if (_TrendValues.Count > Constants.MAX_TREND_LENGTH)
                {
                    var length = _TrendValues.Count - Constants.MAX_TREND_LENGTH;
                    for (Int32 i = 0; i < length; i++)
                    {
                        _TrendValues.RemoveAt(0);
                    }
                }
            }
        }

        public Double[] GetValues()
        {
            lock (_LockObject)
            {
                return _TrendValues.ToArray();
            }
        }

        #region Validity And Configuration
        internal sealed record TrendArgs(ChannelId Source, List<Double> Values);

        internal static TrendArgs ParseFormula(String formula)
        {
            var exp = formula;
            var sep = exp.IndexOf(":");
            if (sep >= 0)
            {
                if (MathType.Trend != Enum.Parse<MathType>(exp[0..sep]))
                    return new(ChannelId.P1, new List<Double>());
                exp = exp[(sep + 1)..];
            }

            String name = "Execute.Trend(";
            if (exp.Substring(0, name.Length) == name)
            {
                var substr = exp.Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                return new(Enum.Parse<ChannelId>(substr[1]), new List<Double>());
            }

            EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"The formula '{formula}' is not a correct Trend(...) expression.", LogLevel.Warn));
            throw new ArgumentException($"The formula '{formula}' is not a correct Trend(...) expression.");
        }

        internal static String MakeFormula(TrendArgs ta)
        {
            return $"Execute.Trend({ta.Source}, 10000)";
        }

        internal static WfmProperties Config(MathModel mch, String exp, Vector? vec, Int32 dataLength)
        {
            var ta = ParseFormula(exp);

            Double cscale = 1;
            Prefix prefix = Prefix.Milli;
            String unit = String.Empty;
            MeasureItemModel selected = null;

            if (ChannelIdExt.IsMeasure(ta.Source))
            {
                selected = DsoModel.Default.Meas.SelectedItems[ta.Source - ChannelIdExt.MinPChId];
                if (selected.Key != mch.OldKey || ta.Source != mch.OldID)
                {
                    mch.OldKey = selected.Key;
                    mch.OldID = ta.Source;
                    mch.InitFlag = true;
                }
                var pfxunit = DsoModel.Default.Meas.Calc.GetPfxUnitString(ta.Source - ChannelIdExt.MinPChId);
                unit = pfxunit.Name;
                if (DsoModel.Default.TryGetChannel(selected.Source, out var src))
                {
                    if (src.Pack is not null && vec != null && vec.Elements.Length > 0)
                    {
                        cscale = src.Pack.Properties.ChnlScale.Value;

                        if (pfxunit.Name == src.Pack.Properties.ChnlUnit.Name)
                        {
                            prefix = Prefix.Micro;
                        }
                    }
                    else
                    {
                        cscale = src.Conditioning.Scale;
                    }
                }
                if (selected.MeasureType == MeasureType.Composite)
                {
                    prefix = Prefix.Micro;
                }
            }
            else
            {
                if (ta.Source != mch.OldID)
                {
                    mch.OldID = ta.Source;
                    mch.InitFlag = true;
                }
            }

            if (mch.InitFlag)
            {
                mch.Conditioning.IgnoreScaleMaxMin = IgnoreScaleLimit.Both;
                mch.Conditioning.IgnorePositionMaxMin = IgnoreScaleLimit.Both;
                mch.Sampling.IgnoreScaleMaxMin = IgnoreScaleLimit.Both;
                mch.Sampling.IgnorePositionMaxMin = IgnoreScaleLimit.Both;
                mch.Conditioning.InitialScale = (0, cscale);
                mch.Conditioning.Prefix = prefix;
                mch.Conditioning.Unit = vec?.YUnit ?? String.Empty;
                mch.Sampling.Scale = 10;
            }
            if (mch.InitFlag)
            {
                (Int32 VScaleIndex, Int32 HScaleIndex) scale = mch.ReadMathScale(mch.MathType, vec);
                mch.Conditioning.ScaleIndex = mch.IsSwitchWindow ? mch.Conditioning.ScaleIndex : scale.VScaleIndex;
                mch.Sampling.ScaleIndex = scale.HScaleIndex;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;
                mch.Sampling.PosIndex = Constants.VIS_XDIVS_NUM * Constants.IDX_PER_XDIV;
                mch.InitFlag = false;
                mch.IsSwitchWindow = false;
            }

            AutoScale(mch, ta, vec, selected, prefix);

            mch.Conditioning.Unit = mch.IsAutoUnit ? (vec?.YUnit ?? "?") : mch.CustomUnit;

            mch.Sampling.Unit = "";

            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = ((Int32)mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                TmbPosition = (mch.Sampling.PosDefIndex * 2, mch.Sampling.GetPosition(mch.Sampling.PosDefIndex)),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),

                VuStartIndex = Constants.MAX_XPOS_IDX - Math.Clamp(dataLength + vec?.Elements.GetLength(1) ?? 0, 0, Constants.MAX_XPOS_IDX),
            };

            prop.SampInterval = 1E-6;

            return prop;
        }

        private static void AutoScale(MathModel mch, TrendArgs ta, Vector? vec, MeasureItemModel? selected, Prefix prefix)
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
                            Double value = 1;
                            value = Quantity.ConvertByPrefix(value, mch.Conditioning.Prefix, Prefix.Empty);
                            var deltavalue = Quantity.ConvertByPrefix(delta, valuepfx, mch.Conditioning.Prefix);
                            var avevalue = Quantity.ConvertByPrefix(ave, valuepfx, mch.Conditioning.Prefix);
                            //if (ta.Source == ChannelId.CYM)
                            //{
                            //    avevalue = Quantity.ConvertByPrefix(ave, Prefix.Kilo, Prefix.Empty);
                            //}
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

                        if (mch.Sampling.ScaleMaxIndex != 35)
                        {
                            mch.Sampling.ScaleMaxIndex = 100;
                            mch.Sampling.ScaleMinIndex = -100;
                            mch.Sampling.Prefix = Prefix.Empty;

                        }

                        mch.Sampling.AutoScale = mch.Sampling.Scale;
                        var l = mch?.Pack?.Length ?? 10;
                        if (l == 0)
                        {
                            l = 10;
                        }

                        var scale = mch.Sampling.Scale;
                        var index = mch.Sampling.ScaleIndex;

                        while (l > scale * 10)
                        {
                            index++;
                            scale = mch.Sampling.GetScaleValue(index, 0);
                        }

                        while (l < scale * 10)
                        {
                            index--;
                            scale = mch.Sampling.GetScaleValue(index, 0);
                        }

                        if (scale * 10 < l)
                        {
                            index++;
                        }

                        mch.Sampling.ScaleIndex = index;

                        if (mch.Sampling.Position != 5000)
                        {
                            mch.Sampling.Position = 5000;
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

// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/14</date>

namespace ScopeX.Core
{
    using System;
    using System.Collections.Generic;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;
    using ScopeX.MathExt;
    using System.ComponentModel;
    using System.Linq;

    [Description(nameof(MathType.Custom))]
    /// <summary>
    /// Defines the <see cref="MathCustomArg" />.
    /// </summary>
    public class MathCustomArg : MathArgPrsnt
    {
        /// <summary>
        /// Defines the _Formula.
        /// </summary>
        private String _Formula;

        /// <summary>
        /// Initializes a new instance of the <see cref="MathCustomArg"/> class.
        /// </summary>
        /// <param name="id">The id<see cref="ChannelId"/>.</param>
        /// <param name="formula">The formula<see cref="String"/>.</param>
        /// <param name="occupier">The occupier<see cref="Object?"/>.</param>
        public MathCustomArg(MathPrsnt mp, ChannelId id, String formula, Object? occupier = null) : base(mp,id, MathType.Custom, occupier)
        {
            if (TryParse(formula, out var arg))
            {
                if (Type != arg.Value.ExpType)
                {
                    formula = "";
                }
                else
                {
                    formula = arg.Value.Exp;
                }
            }

            _Formula = formula;
            Expression = formula;
        }
        public HistParamter HistParamter { get; } = new HistParamter();

        public ChannelId? Source
        {
            get
            {
                if (Occupier is JitterGraphModel)
                {
                    return DsoModel.Default.JitterModel.Source;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the Description.
        /// </summary>
        public override String Description => "y=f(x)";

        /// <summary>
        /// Gets or sets the Expression.
        /// </summary>
        public String Expression { get; set; }

        /// <summary>
        /// Gets or sets the Formula.
        /// </summary>
        public String Formula
        {
            get => _Formula;
            set
            {
                if (_Formula != value)
                {
                    _Formula = value;

                    Model.Formula = MakeFormula();
                    Dispatcher.SoftReset();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the Token.
        /// </summary>
        public List<String> Token { get; } = new();

        /// <summary>
        /// The MakeFormula.
        /// </summary>
        /// <returns>The <see cref="String"/>.</returns>
        public override String MakeFormula()
        {
            return $"{MathType.Custom}:{Formula}";
        }

        public List<ChannelId> GetSourcesByToken()
        {
            List<ChannelId> sources = new List<ChannelId>();

            if (Token != null && Token.Count > 0)
            {
                var sourcetags = DsoPrsnt.DefaultDsoPrsnt.MathFormulaCollections.Where(x => x.Value.Type == MathDefineFormulaType.Source)?.Where(x=> Token.Contains(x.Key))?.Select(x=> x.Value.Name)?.ToList();
                if(sourcetags!=null&&sourcetags.Count>0)
                {
                    foreach (var tag in sourcetags)
                    {
                        if(Enum.TryParse(typeof(ChannelId),tag,out var result)&&result!=null&&result is ChannelId id)
                        {
                            if(!sources.Contains(id))
                            {
                                sources.Add(id);
                            }
                        }
                    }
                }
            }

            return sources;
        }

        public override Boolean SaveFormula(String path, String name, String formula)
        {
            if (!String.IsNullOrEmpty(formula))
            {
                if (Evaluate(ref formula))
                {
                    return FilePrsnt.SaveToText(path, name, sw =>
                    {
                        //write the formula and token
                        sw.WriteLine($"{MathType.Custom}:{formula}");
                        sw.Write(nameof(Token) + " : ");
                        for (Int32 i = 0; i < Token.Count; i++)
                        {
                            if (i != Token.Count - 1)
                            {
                                sw.Write(Token[i] + ",");
                            }
                            else
                            {
                                sw.Write(Token[i]);
                            }
                        }
                        return true;
                    });
                }
            }
            return false;
        }

        public override Boolean LoadFormula(String fullName, out String formula)
        {
            String? originfml = null;

            try
            {
                //Load file content
                _ = FilePrsnt.LoadFromText(fullName, (sr) =>
                {
                    originfml = sr.ReadLine();

                    var origintoken = sr.ReadLine();
                    if (origintoken is not null)
                    {
                        var tokenlist = origintoken.Split(new char[] { ':', ',' });

                        Token.Clear();
                        foreach (var token in tokenlist)
                        {
                            Token.Add(token.Trim());
                        }
                        return true;
                    }
                    return false;
                });

                //evaluate and translate formula
                if (originfml is not null)
                {
                    if (Evaluate(ref originfml))
                    {
                        formula = originfml;
                        return true;
                    }
                }
            }
            catch
            { }
            formula = "";
            return false;
        }

        public static Boolean Evaluate(ref String formula)
        {
            if (formula.StartsWith($"{MathType.Custom}:"))
            {
                if (MathArgPrsnt.TryParse(formula, out var arg))
                {
                    if (arg.Value.ExpType == MathType.Custom)
                    {
                        formula = arg.Value.Exp;
                        return true;

                    }
                }
            }
            return false;
        }

        internal static WfmProperties Config(MathModel mch, String exp, Vector? vec)
        {
            if (mch.Occupier != null)
            {
                if (mch.Occupier is AdvancedMathModel)
                {
                    AdvancedMathModel advMathModel = (AdvancedMathModel)mch.Occupier;
                    return advMathModel.Config(mch, exp, vec);
                }
                if (mch.InitFlag)
                    mch.Sampling.PosIndex = 5000;
            }

            var si = vec?.SampInterval ?? 1;
            var cscale = 1000;
            var length = (vec?.Elements.GetLength(1) ?? 1000) == Constants.MAX_ADC_RES ? 4096 : vec?.Elements.GetLength(1) ?? 1000;
            var tscale = Math.Round(si * 1E6 * (length) / Constants.VIS_XDIVS_NUM, 7);

            mch.Conditioning.InitialScale = (0, cscale);
            mch.Conditioning.ScaleMaxIndex = 30;
            mch.Conditioning.ScaleMinIndex = -20;
            if (mch.Args?.Occupier is not null)
            {
                mch.Conditioning.Prefix = Prefix.Milli;
            }

            mch.Sampling.InitialScale = (0, tscale);
            mch.Sampling.ScaleMaxIndex = 5;
            mch.Sampling.ScaleMinIndex = -5;
            mch.Sampling.Prefix = Prefix.Micro;


            (Double Index, Double Value) tmbposition = (Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX));
            Double vustartindex = 0;

            if (mch.InitFlag)
            {
                mch.Conditioning.ScaleIndex = mch.IsSwitchWindow ? mch.Conditioning.ScaleIndex : 0;
                mch.Conditioning.PosIndex = mch.Conditioning.PosDefIndex;

                mch.Sampling.ScaleIndex = 0;
                mch.Sampling.PosIndex = mch.Sampling.PosDefIndex;
                mch.InitFlag = false;
                mch.IsSwitchWindow = false;
            }
            else
            {
                if (mch.Occupier == null)
                {
                    mch.Conditioning.SetInitScaleValue(0, cscale, -20, 20, false);
                    mch.Sampling.SetInitScaleValue(0, tscale, -30, 30, mch.WindowId == DsoModel.Default.AnalogChnls.First().WindowId);
                    var ach = DsoModel.Default.AnalogChnls.FirstOrDefault(x => x.Active);
                    if (ach != null && ach.Pack != null)
                    {
                        if (mch.WindowId == ach.WindowId)
                        {
                            var scale = DsoPrsnt.DefaultDsoPrsnt.Timebase.GetScale(DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleIndex);
                            if (mch.Sampling.Scale != scale)
                            {
                                mch.Sampling.Scale = scale;
                            }
                            mch.Sampling.PosIndex = DsoPrsnt.DefaultDsoPrsnt.Timebase.PosIndexBymDiv;
                            tmbposition = ach!.Pack!.Properties.TmbPosition;
                            vustartindex = ach!.Pack!.Properties.VuStartIndex;
                        }
                        else
                        {
                            var index = (TriggerPrsnt.State == SysState.Auto || TriggerPrsnt.State == SysState.Triged) ? ach.Pack.Properties.TmbPosition.Index : Constants.DEF_XPOS_IDX;
                            tmbposition = (index, mch.Sampling.GetPosition(index));
                            var tmb = mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX);
                            vustartindex = (-tmb) / mch.Sampling.Scale * Constants.IDX_PER_XDIV;
                        }
                    }
                }
            }

            if (mch.Occupier != null && !mch.AutoScale)
            {
                mch.AutoScale = true;
            }
            VerticalAutoScale(mch, vec);

            mch.Conditioning.Unit = mch.IsAutoUnit ? (vec?.YUnit ?? "?") : mch.CustomUnit;
            mch.Sampling.Unit = vec?.XUnit ?? "?";

            var prop = new WfmProperties(mch.Name)
            {
                ChnlPosition = (mch.Conditioning.PosIndex, mch.Conditioning.Position),
                ChnlScale = (mch.Conditioning.ScaleIndex, mch.Conditioning.Scale),
                ChnlUnit = (mch.Conditioning.Prefix, mch.Conditioning.Unit),

                //TmbPosition = (Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX)),
                //TmbPosition = (vec?.RefSampPos ?? mch.Sampling.PosIndex, mch.Sampling.GetPosition(vec?.RefSampPos ?? mch.Sampling.PosIndex)),
                TmbPosition = tmbposition,
                //TmbPosition = (Constants.DEF_XPOS_IDX, mch.Sampling.GetPosition(Constants.DEF_XPOS_IDX)),
                TmbScale = mch.Sampling.InitialScale,
                TmbUnit = (mch.Sampling.Prefix, mch.Sampling.Unit),
            };
            if (mch.Occupier != null && (mch.Id > ChannelIdExt.MaxMChId && mch.Id < ChannelId.M17))
            {
                prop.VuStartIndex = vustartindex;
                if (mch.Conditioning.Unit == "AV" || mch.Conditioning.Unit == "V²" || mch.Conditioning.Unit == "VV" || vec?.YUnit == "V²" || vec?.YUnit == "VV")
                    mch.Conditioning.Unit = "W";
            }

            if (mch.Args != null && mch.Args!.Occupier != null && mch.Args!.Occupier is AdvancedMathModel)
            {
                prop.DrawMethod = (mch.Args!.Occupier as AdvancedMathModel)!.DrawMethod;
            }

            prop.SampInterval = si;

            return prop;
        }


        private static void VerticalAutoScale(MathModel mch, Vector? vec)
        {
            if (mch.AutoScale)
            {
                if (mch.Conditioning.IgnoreScaleMaxMin != IgnoreScaleLimit.Both)
                {
                    mch.Conditioning.IgnoreScaleMaxMin = IgnoreScaleLimit.Both;
                }
                if (mch.Conditioning.IgnorePositionMaxMin != IgnoreScaleLimit.Both)
                {
                    mch.Conditioning.IgnorePositionMaxMin = IgnoreScaleLimit.Both;
                }
                if (mch.Sampling.IgnoreScaleMaxMin != IgnoreScaleLimit.Both)
                {
                    mch.Sampling.IgnoreScaleMaxMin = IgnoreScaleLimit.Both;
                }
                if (mch.Sampling.IgnorePositionMaxMin != IgnoreScaleLimit.Both)
                {
                    mch.Sampling.IgnorePositionMaxMin = IgnoreScaleLimit.Both;
                }

                IEnumerable<Double> data;
                data = vec?.Elements?.Cast<Double>()?.Where(x => Double.IsFinite(x));
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
                    Prefix valuepfx = Prefix.Empty;

                    Double value = 10;
                    value = Quantity.ConvertByPrefix(value, mch.Conditioning.Prefix, Prefix.Empty);
                    var deltavalue = Quantity.ConvertByPrefix(delta, valuepfx, mch.Conditioning.Prefix);
                    var avevalue = Quantity.ConvertByPrefix(ave, valuepfx, mch.Conditioning.Prefix);
                    if (Math.Abs(deltavalue) > 1E-10)
                    {
                        var condiindex = mch.Conditioning.ScaleIndex;
                        var condiscale = mch.Conditioning.Scale;
                        var target = Math.Abs(deltavalue);
                        while (Math.Abs(deltavalue) > condiscale * 8)
                        {
                            condiindex++;
                            condiscale = mch.Conditioning.GetScale(condiindex);
                        }
                        while (Math.Abs(deltavalue) < condiscale * 4)
                        {
                            condiindex--;
                            condiscale = mch.Conditioning.GetScale(condiindex);
                        }

                        mch.Conditioning.ScaleIndex = condiindex;
                    }
                    else
                    {
                        mch.Conditioning.Scale = Quantity.ConvertByPrefix(1, Prefix.Empty, Prefix.Milli);
                    }
                    var position = -Math.Ceiling(avevalue / mch.Conditioning.Scale * mch.Conditioning.PosIdxPerDiv);
                    if (Math.Abs(position - mch.Conditioning.PosIndex) > mch.Conditioning.PosIdxPerDiv)
                    {
                        mch.Conditioning.PosIndex = position;
                    }
                }
            }
            else
            {
                if (mch.Conditioning.IgnoreScaleMaxMin == IgnoreScaleLimit.Both)
                {
                    mch.Conditioning.IgnoreScaleMaxMin = IgnoreScaleLimit.None;
                }
                if (mch.Conditioning.IgnorePositionMaxMin == IgnoreScaleLimit.Both)
                {
                    mch.Conditioning.IgnorePositionMaxMin = IgnoreScaleLimit.None;
                }
                if (mch.Sampling.IgnoreScaleMaxMin == IgnoreScaleLimit.Both)
                {
                    mch.Sampling.IgnoreScaleMaxMin = IgnoreScaleLimit.None;
                }
                if (mch.Sampling.IgnorePositionMaxMin == IgnoreScaleLimit.Both)
                {
                    mch.Sampling.IgnorePositionMaxMin = IgnoreScaleLimit.None;
                }
            }
        }
    }
}

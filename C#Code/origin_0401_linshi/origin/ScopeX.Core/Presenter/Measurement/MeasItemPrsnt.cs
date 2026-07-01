// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/15</date>

namespace ScopeX.Core
{
    using ScopeX.ComModel;
    using ScopeX.Core.Decode;
    using ScopeX.Core.Tools;
    using System;
    using System.Drawing;
    using System.Linq;

    public enum MeasItemFigureType
    {
        Close,
        Histgram,
        Trend,
        Track
    }

    public class MeasItemPrsnt
    {
        internal MeasItemPrsnt(MeasureItemModel m, MeasPrsnt mp)
        {
            Model = m;
            Presenter = mp;
        }

        public Func<Boolean, ChannelId, MeasItemFigureType, Boolean>? OpenOrCloseFigure
        {
            set => Model.OpenOrCloseFigure = value;
        }


        public ChannelType Type => Model.Type;

        public MeasureType MeasureType
        {
            get => Model.MeasureType;
            set => Model.MeasureType = value;
        }

        public ChannelId Id => Model.Id;

        public String Name
        {
            get => Model.Name;
            set
            {
                if (Model.Name != value)
                {
                    Model.Name = value;
                    Presenter.ResetStat(Id - ChannelId.P1);
                    if (Active && MeasureType != MeasureType.Composite)
                    {
                        Presenter.LastChangedItem = this;
                    }
                }
            }
        }

        public Boolean TrackEnable
        {
            get => Model.TrackEnable;
            set => Model.TrackEnable = value;
        }

        public Boolean TrendEnable
        {
            get => Model.TrendEnable;
            set => Model.TrendEnable = value;
        }

        public Boolean HistgramEnable
        {
            get => Model.HistgramEnable;
            set => Model.HistgramEnable = value;
        }

        public Color DrawColor
        {
            get => Model.DrawColor;
            set => Model.DrawColor = value;
        }

        public Boolean Active
        {
            get => Model.Active;
            set
            {
                if (!Constants.ENABLE_Measure && value)
                {
                    WeakTip.Default.Write("Measure", MsgTipId.FunctionDisabled);
                    Model.Active = false;
                    return;
                }
                if (value)
                {
                    Presenter.LastChangedItem = this;
                }
                if (!value && Model.Active != value)
                {
                    var activemchs = DsoPrsnt.DefaultDsoPrsnt.TryGetRange(c => c.Active && c.Id.IsMath());
                    if (activemchs.Any())
                    {
                        var meas = activemchs.Where(x =>
                        {
                            if (x is MathPrsnt math && math.Args.Occupier == null)
                            {
                                if (math.Args is MathHistArg hist)
                                {
                                    return hist.Source == Id;
                                }
                                if (math.Args is MathTrackArg track)
                                {
                                    return track.Source == Id;
                                }
                                if (math.Args is MathTrendArg trend)
                                {
                                    return trend.Source == Id;
                                }
                            }

                            return false;
                        }).ToList();
                        meas.ForEach(x => x.Active = false);
                    }
                }


                Model.Active = value;
                Presenter.Active = Presenter.IsAnyActive();
                //if (!Presenter.IsAnyActive())
                //{
                //    Presenter.Active = false;
                //}
                //else
                //{
                //    Presenter.Active = true;
                //}
            }
        }

        /// <summary>
        /// Gets or sets the Visiable.
        /// </summary>
        //public Boolean Visiable { get => Model.Visible; set => Model.Visible = value; }

        public ChannelId Source
        {
            get => Model.Source;
            set
            {
                if (Model.Source != value)
                {
                    Model.Source = value;
                    Presenter.ResetStat(Id - ChannelId.P1);
                }
            }
        }

        //public MeasItemExProp? ExProp => Model.ExProp;

        public ChannelId Source2nd
        {
            get => Model.Source2nd;
            set
            {
                if (Model.Source2nd != value)
                {
                    Model.Source2nd = value;
                    Presenter.ResetStat(Id - ChannelId.P1);
                }
            }
        }

        public MeasureOperator Operation
        {
            get => Model.Operation;
            set
            {
                if (Model.Operation != value)
                {
                    Model.Operation = value;
                    Presenter.ResetStat(Id - ChannelId.P1);
                }
            }
        }


        private readonly String[] _DualSrcMeasItems =
        {
            "Delay@lv",
            "Phase@lv",
            "Setup",
            "Hold",
            "Crossing",
            "Skew"
        };

        public Boolean Dualsrc
        {
            get
            {
                Boolean temp = false;
                foreach (var item in _DualSrcMeasItems)
                {
                    if (Name.Contains(item))
                    {
                        temp = true;
                        break;
                    }
                }

                return temp;
            }
        }

        //public Color SourceColor => DsoModel.Default.GetChannel(Source).DrawColor;

        public Color Source2ndColor => DsoModel.Default.TryGetChannel(Source2nd, out var ch) ? ch.DrawColor : Color.Gray;

        public Boolean IsSourceActive => !DsoModel.Default.TryGetChannel(Source, out var ch) || ch.Active;

        public Boolean IsSource2ndActive => !DsoModel.Default.TryGetChannel(Source2nd, out var ch) || ch.Active;

        public Boolean IsStatActive
        {
            get => Model.IsStatActive;
            set => Model.IsStatActive = value;
        }

        public Int32 GapThrold => Model.RefLevel.GapThrold;

        public Int32 HighThrold { get => Model.RefLevel.HighThrold; set => Model.RefLevel.HighThrold = value; }

        public Int32 LowThrold { get => Model.RefLevel.LowThrold; set => Model.RefLevel.LowThrold = value; }

        public Int32 MaxThrold => Model.RefLevel.MaxThrold;

        public Int32 MidThrold { get => Model.RefLevel.MidThrold; set => Model.RefLevel.MidThrold = value; }

        public Int32 MinThrold => Model.RefLevel.MinThrold;


        public Double MaxAbsoluteThrold => Model.RefLevel.MaxAbsoluteThrold * TryGetSourcesGain();
        public Double MinAbsoluteThrold => Model.RefLevel.MinAbsoluteThrold * TryGetSourcesGain();


        private Double TryGetSourcesGain()
        {
            var gain = ProtocolModel.TryGetChannelGain(Source);
            if (Dualsrc)
            {
                var gain2 = ProtocolModel.TryGetChannelGain(Source2nd);
                if (gain2 > gain)
                    gain = gain2;
            }

            return gain;
        }


        public Double HighAbsoluteThrold
        {
            get
            {
                var value = Math.Clamp(Model.RefLevel.HighAbsoluteThrold, MinAbsoluteThrold, MaxAbsoluteThrold);
                if (value != Model.RefLevel.HighAbsoluteThrold)
                {
                    Model.RefLevel.HighAbsoluteThrold = value;
                }
                return Model.RefLevel.HighAbsoluteThrold;
            }
            set
            {
                Model.RefLevel.HighAbsoluteThrold = Math.Clamp(value, MinAbsoluteThrold, MaxAbsoluteThrold);
            }
        }

        public Double LowAbsoluteThrold
        {
            get
            {
                var value = Math.Clamp(Model.RefLevel.LowAbsoluteThrold, MinAbsoluteThrold, MaxAbsoluteThrold);
                if (value != Model.RefLevel.LowAbsoluteThrold)
                {
                    Model.RefLevel.LowAbsoluteThrold = value;
                }
                return Model.RefLevel.LowAbsoluteThrold;
            }
            set
            {
                Model.RefLevel.LowAbsoluteThrold = Math.Clamp(value, MinAbsoluteThrold, MaxAbsoluteThrold);
            }
        }

        public Double MidAbsoluteThrold
        {
            get
            {
                var value = Math.Clamp(Model.RefLevel.MidAbsoluteThrold, MinAbsoluteThrold, MaxAbsoluteThrold);
                if (value != Model.RefLevel.MidAbsoluteThrold)
                {
                    Model.RefLevel.MidAbsoluteThrold = value;
                }
                return Model.RefLevel.MidAbsoluteThrold;
            }
            set
            {
                Model.RefLevel.MidAbsoluteThrold = Math.Clamp(value, MinAbsoluteThrold, MaxAbsoluteThrold);
            }
        }
        public MeasureTopBaseRef RefStandard { get => Model.RefLevel.RefStandard; set => Model.RefLevel.RefStandard = value; }

        public MeasureTopBaseRefUnit RefUnit { get => Model.RefLevel.RefUnit; set => Model.RefLevel.RefUnit = value; }


        public (Prefix Prefix, String Unit) ThroldUnit => RefUnit switch
        {
            MeasureTopBaseRefUnit.Absolute => (Prefix.Milli, DsoModel.Default.GetChannel(Source).Pack?.Properties.ChnlUnit.Name ?? DsoModel.Default.GetChannel(Source).Conditioning.Unit),
            _ => (Prefix.Empty, QuantityUnit.Percent.ToUnitString()),
        };

        private protected MeasureItemModel Model { get; }

        protected MeasPrsnt Presenter { get; }

        public void AdjHighThrold(Int32 step)
        {
            if (RefUnit == MeasureTopBaseRefUnit.Percent)
            {
                HighThrold += step * Model.RefLevel.StpThrold;
            }
            else if (RefUnit == MeasureTopBaseRefUnit.Absolute)
            {
                HighAbsoluteThrold += step * Model.RefLevel.StpThrold;
            }
        }

        public void AdjLowThrold(Int32 step)
        {
            if (RefUnit == MeasureTopBaseRefUnit.Percent)
            {
                LowThrold += step * Model.RefLevel.StpThrold;
            }
            else if (RefUnit == MeasureTopBaseRefUnit.Absolute)
            {
                LowAbsoluteThrold += step * Model.RefLevel.StpThrold;
            }
        }

        public void AdjMidThrold(Int32 step)
        {
            if (RefUnit == MeasureTopBaseRefUnit.Percent)
            {
                MidThrold += step * Model.RefLevel.StpThrold;
            }
            else if (RefUnit == MeasureTopBaseRefUnit.Absolute)
            {
                MidAbsoluteThrold += step * Model.RefLevel.StpThrold;
            }
        }
    }
}

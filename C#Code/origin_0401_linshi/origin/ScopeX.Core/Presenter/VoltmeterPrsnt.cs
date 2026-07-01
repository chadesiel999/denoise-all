// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/14</date>

using System;
using System.Drawing;
using System.Linq;
using NPOI.OpenXmlFormats.Spreadsheet;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.Measure;

namespace ScopeX.Core
{
    public class VoltmeterPrsnt : MulticastPrsnt<IView>, IVoltmeterPrsnt
    {
        private protected override VoltmeterModel Model
        {
            get;
        }

        //public VoltmeterPrsnt(ChannelId id, IDsoPrsnt idp, IVoltmeterView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        //{
        //    Model = mco switch
        //    {
        //        ModelCreateOptions.Dependant => DsoModel.Default.Voltmeter,
        //        ModelCreateOptions.Standalone => new(id),
        //        _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
        //    };

        //    Model.PropertyChanged += OnPropertyChanged;

        //    //_DrawColor = ColorConfig.Default[Id.ToString()];

        //    if (view is not null)
        //    {
        //        view.Presenter = this;
        //        TryAddView(view);
        //    }
        //}

        internal VoltmeterPrsnt(VoltmeterModel model, IDsoPrsnt idp, IView? view) : base(idp)
        {
            Model = model;

            Model.PropertyChanged += OnPropertyChanged;

            if (view is not null)
            {
                if (view is IVoltmeterView vv)
                {
                    vv.Presenter = this;
                }
                TryAddView(view);
            }
        }

        public VoltmeterPrsnt(IDsoPrsnt idp, IView? view = null) : this(DsoModel.Default.Voltmeter, idp, view)
        { }

        public VoltmeterPrsnt(ChannelId id, IDsoPrsnt idp, IView? view = null) : this(new VoltmeterModel(id), idp, view)
        { }

        public ChannelType Type => Model.Type;

        public ChannelId Id => Model.Id;

        public String Name => Model.Name;

        public Boolean Active
        {
            get
            {
                return Model.Active;
            }
            set
            {
                if (Model.Active != value)
                {
                    Model.Active = value;
                    if (!value)
                    {
                        HistgramEnable = false;
                        TrackEnable = false;
                        TrendEnable = false;
                    }
                }
            }
        }
        public Boolean IsStatActive { get => Model.IsStatActive; set => Model.IsStatActive = value; }

        public Boolean IsTriggerSource() => Model.IsTriggerSource();

        public ChannelId Source { get => Model.Source; set => Model.Source = value; }

        //Autorange is not available when the oscilloscope is triggering on the same channel that is being measured.
        public Boolean AutoRange { get => Model.AutoRange; set => Model.AutoRange = value; }

        public Color DrawColor
        {
            get => DsoModel.Default.GetChannel(Source).DrawColor;
            set => WeakTip.Default.Write(nameof(VoltmeterPrsnt), MsgTipId.FeatureUnsupported);
        }

        public Boolean EnableMode => VoltmeterModel.IsACCoupling(Source);

        public String VoltageToString(Double value) => Model.VoltageToString(value);

        public void ResetStatistics()
        {
            var maths = DsoModel.Default.MathChnls.Where(x => x.Args is MathTrendArg arg && arg.Source == ChannelId.DVM).ToList();
            maths?.ForEach(x => x.ClearFlag = true);
            StaBuffer?.Clear();
        }

        public VoltmeterMode Mode
        {
            get => Model.Mode;
            set
            {
                if (VoltmeterModel.IsACCoupling(Source))
                {
                    Model.Mode = VoltmeterMode.ACrms;
                }
                else
                {
                    Model.Mode = value;
                }
            }
        }

        public String Unit => Model.Unit;

        public StatisticBuffer StaBuffer => Model.StaBuffer;

        public Double Current => Model.Current;

        public Boolean HistgramEnable
        {
            get => Model.HistgramEnable;
            set => Model.HistgramEnable = value;
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

        public Func<Boolean, ChannelId, MeasItemFigureType, Boolean>? OpenOrCloseFigure
        {
            set => Model.OpenOrCloseFigure = value;
        }


        private Boolean _CalcFlag = true;
        public void Run()
        {
            if (Model != null && Active && _CalcFlag)
            {
                _CalcFlag = false;
                Model.CalcVoltBymV();
                _CalcFlag = true;
            }
        }

    }
}

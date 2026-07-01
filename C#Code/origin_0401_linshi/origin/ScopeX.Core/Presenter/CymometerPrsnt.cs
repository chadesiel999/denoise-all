using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using ScopeX.Measure;

namespace ScopeX.Core
{
    public class CymometerPrsnt : MulticastPrsnt<IView>, ICymometerPrsnt
    {
        private protected override CymometerModel Model
        {
            get;
        }

        //public CymometerPrsnt(ChannelId id, IDsoPrsnt idp, ICymometerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        //{
        //    Model = mco switch
        //    {
        //        ModelCreateOptions.Dependant => DsoModel.Default.Cymometer,
        //        ModelCreateOptions.Standalone => new(id),
        //        _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
        //    };

        //    Model.PropertyChanged += OnPropertyChanged;

        //    if (view is not null)
        //    {
        //        view.Presenter = this;
        //        TryAddView(view);
        //    }
        //}

        internal CymometerPrsnt(CymometerModel model, IDsoPrsnt idp, IView? view) : base(idp)
        {
            Model = model;
            Model.PropertyChanged += OnPropertyChanged;

            if (view is not null)
            {
                if (view is ICymometerView cv)
                {
                    cv.Presenter = this;
                }
                TryAddView(view);
            }

            this.Source = TriggerPrsnt.GetTriggerSource();
        }

        public CymometerPrsnt(IDsoPrsnt idp, IView? view = null) : this(DsoModel.Default.Cymometer, idp, view)
        { }

        public CymometerPrsnt(ChannelId id, IDsoPrsnt idp, IView? view = null) : this(new CymometerModel(id), idp, view)
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

        public Boolean ShowPeriod
        {
            get
            {
                return Model.ShowPeriod;
            }
            set
            {
                if (Model.ShowPeriod != value)
                {
                    Model.ShowPeriod = value;
                }
            }
        }
        public ChannelId? Source
        {
            get => Model.Source;
            set
            {
                Model.Source = value;
                Hardware.HdCmdFactory.Push(HdCmd.CymometerSrc);
            }
        }

        public Color DrawColor
        {
            get => Source == null ? DsoModel.Default.GetChannel(ChannelId.C1).DrawColor : DsoModel.Default.GetChannel(Source.Value).DrawColor;
            set => WeakTip.Default.Write(nameof(CymometerPrsnt), MsgTipId.FeatureUnsupported);
        }

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

        public Boolean IsStatActive { get => Model.IsStatActive; set => Model.IsStatActive = value; }
        public StatisticBuffer StaBuffer => Model.StaBuffer;

        public Double FrequencyByHz => Model.CurrentCym;
        public String Unit => Model.Unit;
        public String FrequencyToString(Double value) => Model.FrequencyToString(value);

        private Boolean _CalcFlag = true;
        public void Run()
        {
            if (Model != null && Active && Source != null && _CalcFlag)
            {
                _CalcFlag = false;
                Model.GetFrequency();
                _CalcFlag = true;
            }
        }

        public void ResetStatistics()
        {
            var maths = DsoModel.Default.MathChnls.Where(x => x.Args is MathTrendArg arg && arg.Source == ChannelId.CYM).ToList();
            maths?.ForEach(x => x.ClearFlag = true);
            StaBuffer?.Clear();
        }
    }
}

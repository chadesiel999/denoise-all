using System;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    public class SearchEdgePrsnt : TrigEdgePrsnt, ISearchTypePrsnt//, ISearchItemView
    {
        internal SearchEdgePrsnt(IDsoPrsnt idp, SearchEdgeModel m, ITriggerView? view = null) : base(idp)
        {
            ItemModel = m;
            ItemModel.PropertyChanged += OnPropertyChanged;
            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
            LoadTriggerPrsnt();
        }

        private TriggerPrsnt _Trigger;

        public TriggerPrsnt Trigger { get => (TrigEdgePrsnt)_Trigger; }

        public new ChannelId Source
        {
            get => Model.Source;
            set
            {
                Model.Source = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }

        private protected SearchEdgeModel ItemModel { get; }

        private protected override SearchEdgeModel Model => ItemModel;

        public Int32 ResultCount { get => Model.ResultCount; set => Model.ResultCount = value; }
        public (Double[,] Result, Int32 ResultCount)? ResultPack
        {
            get => Model.ResultPack;
            set => Model.ResultPack = value;
        }

        public void LoadTriggerPrsnt()
        {
            _Trigger = DsoPrsnt.DefaultDsoPrsnt.CurrentTrigger;
        }

        public Boolean ReadFromTrigger()
        {
            if (_Trigger == null || _Trigger is not TrigEdgePrsnt tp || tp.Source == null)
                return false;

            this.Source = ((TrigEdgePrsnt)_Trigger).Source!.Value;
            this.Slope = ((TrigEdgePrsnt)_Trigger).Slope;
            this.PosIndex = ((TrigEdgePrsnt)_Trigger).PosIndex;
            //this.CompPosition = ((TriggerEdgeModel)_Trigger).CompPosition;
            //this.Coupling = ((TriggerEdgeModel)_Trigger).Coupling;
            //this.Impedance = ((TriggerEdgeModel)_Trigger).Impedance;

            return true;
        }

        public Boolean SetToTrigger()
        {
            if (Trigger == null)
            {
                return false;
            }

            ((TrigEdgePrsnt)_Trigger).Source = this.Source;
            ((TrigEdgePrsnt)_Trigger).Slope = this.Slope;
            ((TrigEdgePrsnt)_Trigger).PosIndex = this.PosIndex;
            //((TriggerEdgeModel)_Trigger).CompPosition = this.CompPosition;
            //((TriggerEdgeModel)_Trigger).Coupling = this.Coupling;
            //((TriggerEdgeModel)_Trigger).Impedance = this.Impedance;
            return true;
        }

        public override void ResetPosIndex()
        {
            if (DsoModel.Default.Timebase.ScaleIndex >= AnaChnlTimebaseIndex.Lv50m)
            {
                // 滚动模式时，触发电平复位按钮失效
                return;
            }

            Model.ResetCompPosIndex();
        }

        #region Trigger

        public new TriggerCoupling Coupling
        {
            get => Model.Coupling;
            set
            {
                Model.Coupling = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }

        public new TriggerImpedance Impedance
        {
            get => Model.Impedance;
            set
            {
                Model.Impedance = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }

        public new EdgeSlope Slope
        {
            get => Model.Slope;
            set
            {
                Model.Slope = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        #endregion
    }

    public class SearchPulsePrsnt : TrigWidthPrsnt, ISearchTypePrsnt
    {
        internal SearchPulsePrsnt(IDsoPrsnt idp, SearchPulseModel m, ITriggerView? view = null) : base(idp)
        {
            ItemModel = m;
            ItemModel.PropertyChanged += OnPropertyChanged;
            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
            LoadTriggerPrsnt();
        }

        private TriggerPrsnt _Trigger;

        public TriggerPrsnt Trigger { get => (TrigWidthPrsnt)_Trigger; }

        private protected SearchPulseModel ItemModel { get; }

        public new ChannelId Source
        {
            get => Model.Source;
            set
            {
                Model.Source = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }

        private protected override SearchPulseModel Model => ItemModel;

        public Int32 ResultCount { get => Model.ResultCount; set => Model.ResultCount = value; }

        public (Double[,] Result, Int32 ResultCount)? ResultPack { get => Model.ResultPack; set => Model.ResultPack = value; }

        public void LoadTriggerPrsnt()
        {
            _Trigger = DsoPrsnt.DefaultDsoPrsnt.CurrentTrigger;
        }
        public Boolean ReadFromTrigger()
        {
            if (_Trigger == null || _Trigger is not TrigWidthPrsnt tp || tp.Source == null)
                return false;

            this.Source = ((TrigWidthPrsnt)_Trigger).Source!.Value;
            this.Polarity = ((TrigWidthPrsnt)_Trigger).Polarity;
            this.PosIndex = ((TrigWidthPrsnt)_Trigger).PosIndex;
            this.Condition = ((TrigWidthPrsnt)_Trigger).Condition;
            this.WidthByps = ((TrigWidthPrsnt)_Trigger).WidthByps;
            this.UpperWidthByps = ((TrigWidthPrsnt)_Trigger).UpperWidthByps;
            this.CompPosition = ((TrigWidthPrsnt)_Trigger).CompPosition;

            return true;
        }

        public Boolean SetToTrigger()
        {
            if (_Trigger == null)
                return false;

            ((TrigWidthPrsnt)_Trigger).Source = this.Source;
            ((TrigWidthPrsnt)_Trigger).Polarity = this.Polarity;
            ((TrigWidthPrsnt)_Trigger).PosIndex = this.PosIndex;
            ((TrigWidthPrsnt)_Trigger).CompPosition = this.CompPosition;
            ((TrigWidthPrsnt)_Trigger).WidthByps = this.WidthByps;
            ((TrigWidthPrsnt)_Trigger).UpperWidthByps = this.UpperWidthByps;
            ((TrigWidthPrsnt)_Trigger).Condition = this.Condition;

            return true;
        }

        public override void ResetPosIndex()
        {
            if (DsoModel.Default.Timebase.ScaleIndex >= AnaChnlTimebaseIndex.Lv50m)
            {
                // 滚动模式时，触发电平复位按钮失效
                return;
            }

            Model.ResetCompPosIndex();
        }

        #region Trigger

        public new PulseCondition Condition
        {
            get => Model.Condition;
            set
            {
                Model.Condition = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }

        public Int64 MaxWidth => Model.MaxWidth;

        public Int64 MinWidth => Model.MinWidth;

        public new PulsePolarity Polarity
        {
            get => Model.Polarity;
            set
            {
                Model.Polarity = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }

        public new Int64 WidthByps
        {
            get => Model.WidthByps;
            set
            {
                Model.WidthByps = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }

        public new Double WidthByus
        {
            get => WidthByps / 1000_000D;
            set => WidthByps = (Int64)(value * 1000_000D);
        }

        #endregion
    }

    public class SearchTransitionPrsnt : TrigTransPrsnt, ISearchTypePrsnt
    {
        internal SearchTransitionPrsnt(IDsoPrsnt idp, SearchTransitionModel m, ITriggerView? view = null) : base(idp)
        {
            ItemModel = m;
            ItemModel.PropertyChanged += OnPropertyChanged;
            if (view != null)
            {
                view.Presenter = this;
                TryAddView(view);
            }
            LoadTriggerPrsnt();
        }


        private TriggerPrsnt _Trigger;

        public TriggerPrsnt Trigger { get => (TrigTransPrsnt)_Trigger; }


        public new ChannelId Source
        {
            get => Model.Source;
            set
            {
                Model.Source = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        private protected SearchTransitionModel ItemModel { get; }
        private protected override SearchTransitionModel Model => ItemModel;
        public Int32 ResultCount { get => Model.ResultCount; set => Model.ResultCount = value; }
        public (Double[,] Result, Int32 ResultCount)? ResultPack { get => Model.ResultPack; set => Model.ResultPack = value; }

        public void LoadTriggerPrsnt()
        {
            _Trigger = DsoPrsnt.DefaultDsoPrsnt.CurrentTrigger;
        }
        public Boolean ReadFromTrigger()
        {
            if (_Trigger == null)
                return false;
            this.Source = ((TrigTransPrsnt)_Trigger).Source;
            this.Slope = ((TrigTransPrsnt)_Trigger).Slope;
            this.PosLowerIndex = ((TrigTransPrsnt)_Trigger).PosLowerIndex;
            this.PosUpperIndex = ((TrigTransPrsnt)_Trigger).PosUpperIndex;
            this.WidthCompCondition = ((TrigTransPrsnt)_Trigger).WidthCompCondition;
            this.WidthByps = ((TrigTransPrsnt)_Trigger).WidthByps;  //持续时间
            this.LowerCompPosition = ((TrigTransPrsnt)_Trigger).LowerCompPosition;
            this.UpperCompPosition = ((TrigTransPrsnt)_Trigger).UpperCompPosition;
            return true;
        }
        public Boolean SetToTrigger()
        {
            if (_Trigger == null)
                return false;
            ((TrigTransPrsnt)_Trigger).Source = this.Source;
            ((TrigTransPrsnt)_Trigger).Slope = this.Slope;
            ((TrigTransPrsnt)_Trigger).PosLowerIndex = this.PosLowerIndex;
            ((TrigTransPrsnt)_Trigger).PosUpperIndex = this.PosUpperIndex;
            ((TrigTransPrsnt)_Trigger).LowerCompPosition = this.LowerCompPosition;
            ((TrigTransPrsnt)_Trigger).UpperCompPosition = this.UpperCompPosition;
            ((TrigTransPrsnt)_Trigger).WidthByps = this.WidthByps;
            ((TrigTransPrsnt)_Trigger).WidthCompCondition = this.WidthCompCondition;
            return true;
        }

        #region Trigger
        public new EdgeSlope TransSlope
        {
            get => Model.Slope;
            set
            {
                Model.Slope = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        #endregion
    }
    public class SearchRuntPrsnt : TrigRuntPrsnt, ISearchTypePrsnt
    {
        internal SearchRuntPrsnt(IDsoPrsnt idp, SearchRuntModel m, ITriggerView? view = null) : base(idp)
        {
            ItemModel = m;
            ItemModel.PropertyChanged += OnPropertyChanged;
            if (view != null)
            {
                view.Presenter = this;
                TryAddView(view);
            }
            LoadTriggerPrsnt();
        }

        private TriggerPrsnt _Trigger;

        public TriggerPrsnt Trigger { get => (SearchRuntPrsnt)_Trigger; }

        private protected SearchRuntModel ItemModel { get; }
        public new ChannelId Source
        {
            get => Model.Source;
            set
            {
                Model.Source = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        private protected override SearchRuntModel Model => ItemModel;
        public Int32 ResultCount { get => Model.ResultCount; set => Model.ResultCount = value; }
        public (Double[,] Result, Int32 ResultCount)? ResultPack { get => Model.ResultPack; set => Model.ResultPack = value; }
        public void LoadTriggerPrsnt()
        {
            _Trigger = DsoPrsnt.DefaultDsoPrsnt.CurrentTrigger;
        }
        public Boolean ReadFromTrigger()
        {
            if (_Trigger == null)
                return false;
            this.Source = ((SearchRuntPrsnt)_Trigger).Source;
            this.Polarity = ((SearchRuntPrsnt)_Trigger).Polarity;
            this.PosLowerIndex = ((SearchRuntPrsnt)_Trigger).PosLowerIndex;
            this.PosUpperIndex = ((SearchRuntPrsnt)_Trigger).PosUpperIndex;
            this.WidthCompCondition = ((SearchRuntPrsnt)_Trigger).WidthCompCondition;
            this.WidthByps = ((SearchRuntPrsnt)_Trigger).WidthByps;  //持续时间
            this.LowerCompPosition = ((SearchRuntPrsnt)_Trigger).LowerCompPosition;
            this.UpperCompPosition = ((SearchRuntPrsnt)_Trigger).UpperCompPosition;
            return true;
        }
        public Boolean SetToTrigger()
        {
            if (_Trigger == null)
                return false;
            ((SearchRuntPrsnt)_Trigger).Source = this.Source;
            ((SearchRuntPrsnt)_Trigger).Polarity = this.Polarity;

            ((SearchRuntPrsnt)_Trigger).PosLowerIndex = this.PosLowerIndex;
            ((SearchRuntPrsnt)_Trigger).PosUpperIndex = this.PosUpperIndex;
            ((SearchRuntPrsnt)_Trigger).LowerCompPosition = this.LowerCompPosition;
            ((SearchRuntPrsnt)_Trigger).UpperCompPosition = this.UpperCompPosition;
            ((SearchRuntPrsnt)_Trigger).WidthByps = this.WidthByps;
            ((SearchRuntPrsnt)_Trigger).WidthCompCondition = this.WidthCompCondition;
            return true;
        }

        #region Trigger
        public new PulsePolarity Polarity
        {
            get => Model.Polarity;
            set
            {
                Model.Polarity = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        #endregion
    }
    public class SearchTimeoutPrsnt : TrigTimeOutPrsnt, ISearchTypePrsnt
    {
        internal SearchTimeoutPrsnt(IDsoPrsnt idp, SearchTimeoutModel m, ITriggerView? view = null) : base(idp)
        {
            ItemModel = m;
            ItemModel.PropertyChanged += OnPropertyChanged;
            if (view != null)
            {
                view.Presenter = this;
                TryAddView(view);
            }
            LoadTriggerPrsnt();
        }


        private TriggerPrsnt _Trigger;

        public TriggerPrsnt Trigger { get => (SearchRuntPrsnt)_Trigger; }


        private protected SearchTimeoutModel ItemModel { get; }

        public new ChannelId Source
        {
            get => Model.Source;
            set
            {
                Model.Source = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        private protected override SearchTimeoutModel Model => ItemModel;
        public Int32 ResultCount { get => Model.ResultCount; set => Model.ResultCount = value; }
        public (Double[,] Result, Int32 ResultCount)? ResultPack { get => Model.ResultPack; set => Model.ResultPack = value; }
        public void LoadTriggerPrsnt()
        {
            _Trigger = DsoPrsnt.DefaultDsoPrsnt.CurrentTrigger;
        }
        public Boolean ReadFromTrigger()
        {
            if (_Trigger == null)
                return false;
            this.Source = ((SearchTimeoutPrsnt)_Trigger).Source;
            this.Polarity = ((SearchTimeoutPrsnt)_Trigger).Polarity;
            this.DurationByps = ((SearchTimeoutPrsnt)_Trigger).DurationByps;
            this.CompPosition = ((SearchTimeoutPrsnt)_Trigger).CompPosition;
            this.PosIndex = ((SearchTimeoutPrsnt)_Trigger).PosIndex;
            return true;
        }
        public Boolean SetToTrigger()
        {
            if (_Trigger == null)
                return false;
            ((SearchTimeoutPrsnt)_Trigger).Source = this.Source;
            ((SearchTimeoutPrsnt)_Trigger).Polarity = this.Polarity;
            ((SearchTimeoutPrsnt)_Trigger).PosIndex = this.PosIndex;
            ((SearchTimeoutPrsnt)_Trigger).CompPosition = this.CompPosition;
            ((SearchTimeoutPrsnt)_Trigger).DurationByps = this.DurationByps;

            return true;
        }

        #region Trigger
        public new Int64 DurationByps
        {
            get => Model.DurationByps;
            set
            {
                Model.DurationByps = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        //public Int64 MaxDuration => Model.MaxDuration;
        //public Int64 MinDuration => Model.MinDuration;
        public new LevelPolarity Polarity
        {
            get => Model.Polarity;
            set
            {
                Model.Polarity = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new void AdjDuration(Int64 step)
        {
            Model.DurationByps += step * Model.StpDuration;
           //Hardware.HdCmdFactory.Push(HdCmd.Search);
        }
        #endregion
    }
    public class SearchWindowPrsnt : TrigWindowPrsnt, ISearchTypePrsnt
    {
        internal SearchWindowPrsnt(IDsoPrsnt idp, SearchWindowModel m, ITriggerView? view = null) : base(idp)
        {
            ItemModel = m;
            ItemModel.PropertyChanged += OnPropertyChanged;
            if (view != null)
            {
                view.Presenter = this;
                TryAddView(view);
            }
            LoadTriggerPrsnt();
        }

        private TriggerPrsnt _Trigger;

        public TriggerPrsnt Trigger { get => (TrigWindowPrsnt)_Trigger; }

        private protected SearchWindowModel ItemModel { get; }
        public new ChannelId Source
        {
            get => Model.Source;
            set
            {
                Model.Source = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        private protected override SearchWindowModel Model => ItemModel;
        public Int32 ResultCount { get => Model.ResultCount; set => Model.ResultCount = value; }
        public (Double[,] Result, Int32 ResultCount)? ResultPack { get => Model.ResultPack; set => Model.ResultPack = value; }
        public void LoadTriggerPrsnt()
        {
            _Trigger = DsoPrsnt.DefaultDsoPrsnt.CurrentTrigger;
        }
        public Boolean ReadFromTrigger()
        {
            if (_Trigger == null)
                return false;
            this.Source = ((TrigWindowPrsnt)_Trigger).Source;
            this.PosCondition = ((TrigWindowPrsnt)_Trigger).PosCondition; //窗口范围
            this.WidthByps = ((TrigWindowPrsnt)_Trigger).WidthByps;       //持续时间
            this.TimeCondition = ((TrigWindowPrsnt)_Trigger).TimeCondition; //时间条件
            //检查这部分属性是否一一对应
            this.PosLowerIndex = ((TrigWindowPrsnt)_Trigger).PosLowerIndex;
            this.PosUpperIndex = ((TrigWindowPrsnt)_Trigger).PosUpperIndex;
            this.LowerCompPosition = ((TrigWindowPrsnt)_Trigger).LowerCompPosition;
            this.UpperCompPosition = ((TrigWindowPrsnt)_Trigger).UpperCompPosition;
            return true;
        }
        public Boolean SetToTrigger()
        {
            if (_Trigger == null)
                return false;
            ((TrigWindowPrsnt)_Trigger).Source = this.Source;
            ((TrigWindowPrsnt)_Trigger).PosCondition = this.PosCondition;
            ((TrigWindowPrsnt)_Trigger).WidthByps = this.WidthByps;
            ((TrigWindowPrsnt)_Trigger).TimeCondition = this.TimeCondition;
            ((TrigWindowPrsnt)_Trigger).PosLowerIndex = this.PosLowerIndex;
            ((TrigWindowPrsnt)_Trigger).PosUpperIndex = this.PosUpperIndex;
            ((TrigWindowPrsnt)_Trigger).LowerCompPosition = this.LowerCompPosition;
            ((TrigWindowPrsnt)_Trigger).UpperCompPosition = this.UpperCompPosition;
            return true;
        }

        #region Trigger
        public new WindowRange PosCondition
        {
            get => Model.PosCondition;
            set
            {
                Model.PosCondition = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new WindowTimeCondition TimeCondition
        {
            get => Model.TimeCondition;
            set
            {
                Model.TimeCondition = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }

        #endregion
    }
    public class SearchSetupHoldPrsnt : TrigSetupHoldPrsnt, ISearchTypePrsnt
    {
        internal SearchSetupHoldPrsnt(IDsoPrsnt idp, SearchSetupHoldModel m, ITriggerView? view = null) : base(idp)
        {
            ItemModel = m;
            ItemModel.PropertyChanged += OnPropertyChanged;
            if (view != null)
            {
                view.Presenter = this;
                TryAddView(view);
            }
            LoadTriggerPrsnt();
        }


        private TriggerPrsnt _Trigger;

        public TriggerPrsnt Trigger { get => (TrigSetupHoldPrsnt)_Trigger; }


        private protected SearchSetupHoldModel ItemModel { get; }
        public new ChannelId ClkSource
        {
            get
            {
                return Model.ClkSource;
            }
            set
            {
                Model.ClkSource = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public ChannelId Source
        {
            get
            {
                return Model.DataSource;
            }
            set
            {
                Model.DataSource = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        private protected override SearchSetupHoldModel Model => ItemModel;
        public Int32 ResultCount { get => Model.ResultCount; set => Model.ResultCount = value; }
        public (Double[,] Result, Int32 ResultCount)? ResultPack { get => Model.ResultPack; set => Model.ResultPack = value; }
        public void LoadTriggerPrsnt()
        {
            _Trigger = DsoPrsnt.DefaultDsoPrsnt.CurrentTrigger;
        }
        public Boolean ReadFromTrigger()
        {
            if (_Trigger == null)
                return false;
            this.Source = ((TrigSetupHoldPrsnt)_Trigger).DataSource;
            this.ClkSource = ((TrigSetupHoldPrsnt)_Trigger).ClkSource;
            this.ClkPolarity = ((TrigSetupHoldPrsnt)_Trigger).ClkPolarity;
            this.ThdByps = ((TrigSetupHoldPrsnt)_Trigger).ThdByps;
            this.TsuByps = ((TrigSetupHoldPrsnt)_Trigger).TsuByps;
            this.Violation = ((TrigSetupHoldPrsnt)_Trigger).Violation;
            this.UpperDataPosIndex = ((TrigSetupHoldPrsnt)_Trigger).UpperDataPosIndex;
            this.LowerDataPosIndex = ((TrigSetupHoldPrsnt)_Trigger).LowerDataPosIndex;
            this.UpperDataPosition = ((TrigSetupHoldPrsnt)_Trigger).UpperDataPosition;
            this.LowerDataPosition = ((TrigSetupHoldPrsnt)_Trigger).LowerDataPosition;
            this.ClkCompPosIndex = ((TrigSetupHoldPrsnt)_Trigger).ClkCompPosIndex;
            this.ClkCompPosition = ((TrigSetupHoldPrsnt)_Trigger).ClkCompPosition;
            return true;
        }
        public Boolean SetToTrigger()
        {
            if (_Trigger == null)
                return false;
            ((TrigSetupHoldPrsnt)_Trigger).DataSource = this.Source;
            ((TrigSetupHoldPrsnt)_Trigger).ClkSource = this.ClkSource;
            ((TrigSetupHoldPrsnt)_Trigger).ClkPolarity = this.ClkPolarity;
            ((TrigSetupHoldPrsnt)_Trigger).ThdByps = this.ThdByps;
            ((TrigSetupHoldPrsnt)_Trigger).TsuByps = this.TsuByps;
            ((TrigSetupHoldPrsnt)_Trigger).Violation = this.Violation;
            ((TrigSetupHoldPrsnt)_Trigger).UpperDataPosIndex = this.UpperDataPosIndex;
            ((TrigSetupHoldPrsnt)_Trigger).LowerDataPosIndex = this.LowerDataPosIndex;
            ((TrigSetupHoldPrsnt)_Trigger).UpperDataPosition = this.UpperDataPosition;
            ((TrigSetupHoldPrsnt)_Trigger).LowerDataPosition = this.LowerDataPosition;
            ((TrigSetupHoldPrsnt)_Trigger).ClkCompPosIndex = this.ClkCompPosIndex;
            ((TrigSetupHoldPrsnt)_Trigger).ClkCompPosition = this.ClkCompPosition;
            return true;
        }

        #region Trigger
        public new Double ClkCompPosIndex
        {
            get
            {
                return Model.ClkCompPosIndex;
            }
            set
            {
                Model.ClkCompPosIndex = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new Double ClkCompPosition
        {
            get
            {
                return Model.ClkCompPosition;
            }
            set
            {
                Model.ClkCompPosition = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new EdgeSlope ClkPolarity
        {
            get
            {
                return Model.ClkPolarity;
            }
            set
            {
                Model.ClkPolarity = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new Prefix ClkPrefix => Model.ClkPrefix;
        public new Double ClkRelPosIndex => Model.ClkRelPosIndex;
        public new String ClkUnit => Model.ClkUnit;
        public new Prefix DataPrefix => Model.DataPrefix;
        public new Double DataRelPosLowerIndex
        {
            get
            {
                return Model.DataRelPosLowerIndex;
            }
            set
            {
                Model.DataRelPosLowerIndex = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new Double DataRelPosUpperIndex
        {
            get
            {
                return Model.DataRelPosUpperIndex;
            }
            set
            {
                Model.DataRelPosUpperIndex = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new String DataUnit => Model.DataUnit;
        public new Double LowerDataPosIndex
        {
            get
            {
                return Model.LowerDataPosIndex;
            }
            set
            {
                Model.LowerDataPosIndex = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new Double LowerDataPosition
        {
            get
            {
                return Model.LowerDataPosition;
            }
            set
            {
                Model.LowerDataPosition = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new Double MaxClkCompPosition => Model.MaxClkCompPosition;

        public new Double MaxDataCompPosition => Model.MaxDataCompPosition;
        public new Int64 MaxThd => Model.MaxThd;
        public new Int64 MaxTsu => Model.MaxTsu;
        public new Double MinClkCompPosition => Model.MinClkCompPosition;
        public new Double MinDataCompPosition => Model.MinDataCompPosition;
        public new Int64 MinThd => Model.MinThd;
        public new Int64 MinTsu => Model.MinTsu;
        public override Double PosIndex
        {
            get
            {
                return Model.ClkCompPosIndex;
            }
            set
            {
                Model.ClkCompPosIndex = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new Int64 ThdByps
        {
            get
            {
                return Model.ThdByps;
            }
            set
            {
                Model.ThdByps = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new Int64 TsuByps
        {
            get
            {
                return Model.TsuByps;
            }
            set
            {
                Model.TsuByps = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new Double UpperDataPosIndex
        {
            get
            {
                return Model.UpperDataPosIndex;
            }
            set
            {
                Model.UpperDataPosIndex = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new Double UpperDataPosition
        {
            get
            {
                return Model.UpperDataPosition;
            }
            set
            {
                Model.UpperDataPosition = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new SetupHoldViolation Violation
        {
            get
            {
                return Model.Violation;
            }
            set
            {
                Model.Violation = value;
               //Hardware.HdCmdFactory.Push(HdCmd.Search);
            }
        }
        public new void AdjThd(Int32 step)
        {
            Model.ThdByps += step * Model.StpThd;
           //Hardware.HdCmdFactory.Push(HdCmd.Search);
        }
        public new void AdjTsu(Int32 step)
        {
            Model.TsuByps += step * Model.StpTsu;
           //Hardware.HdCmdFactory.Push(HdCmd.Search);
        }
        public override void ResetPosIndex()
        {
            Model.ClkCompPosIndex = 0.0;
        }
        #endregion
    }

}

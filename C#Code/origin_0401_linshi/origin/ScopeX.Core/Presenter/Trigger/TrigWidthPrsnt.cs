// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    public class TrigWidthPrsnt : TrigSingleSrcPrsnt
    {
        public TrigWidthPrsnt(IDsoPrsnt idp, ITriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp, view)
        {
            _Model = mco switch
            {
                ModelCreateOptions.Dependant => (TriggerWidthModel)DsoModel.Default.GetTriggerModel(TriggerType.PulseWidth),
                ModelCreateOptions.Standalone => new(),
                _ => null,
            };
            LoadEvent();
        }

        /// <summary>
        /// 重载参数
        /// </summary>
        public override void LoadEvent()
        {
            if (_Model != null && Source != null)
            {
                KeyLed.Default.SetTriggerSrc(Source!.Value);
                _Model.PropertyChanged += OnPropertyChanged;
            }
        }

        /// <summary>
        /// 切换类型，注销事件
        /// </summary>
        public override void DisposeEvent()
        {
            if (_Model != null && Source != null)
            {
                _Model.PropertyChanged -= OnPropertyChanged;
            }
        }

        internal TrigWidthPrsnt(IDsoPrsnt idp, ITriggerView view, TriggerWidthModel model) : base(idp, view)
        {
            _Model = model;
            LoadEvent();
        }

        public PulseCondition Condition
        {
            get => Model.Condition;
            set
            {
                Model.Condition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public (Int64 min, Int64 max) GetWidthRange() => Model.GetWidthRange(0);

        public (Int64 min, Int64 max) GetUpperWidthRange() => Model.GetWidthRange(1);

        //public String Name => Model.Name;

        public PulsePolarity Polarity
        {
            get => Model.Polarity;
            set
            {
                Model.Polarity = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Int64 WidthByps
        {
            get => Model.WidthByps;
            set
            {
                Model.WidthByps = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Double WidthByus
        {
            get => WidthByps / 1000_000D;
            set => WidthByps = (Int64)(value * 1000_000D);
        }

        public Int64 UpperWidthByps
        {
            get => Model.UpperWidthByps;
            set
            {
                Model.UpperWidthByps = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Double UpperWidthByus
        {
            get => UpperWidthByps / 1000_000D;
            set => UpperWidthByps = (Int64)(value * 1000_000);
        }

        private protected override TriggerWidthModel Model => (TriggerWidthModel)_Model!;

        public void AdjWidth(Int64 delta)
        {
            //Model.WidthByps += step * Model.StpWidth;
            Model.AdjWidth(delta);
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }

        public void AdjUpperWidth(Int64 delta)
        {
            //Model.UpperWidthByps += step * Model.StpWidth;
            Model.AdjUpperWidth(delta);
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }
    }
}

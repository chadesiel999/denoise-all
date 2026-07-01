// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    /// <summary>
    /// Defines the <see cref="TrigNEdgePrsnt" />.
    /// </summary>
    public class TrigNEdgePrsnt : TrigSingleSrcPrsnt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrigNEdgePrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView?"/>.</param>
        /// <param name="mco">The mco<see cref="ModelCreateOptions"/>.</param>
        public TrigNEdgePrsnt(IDsoPrsnt idp, ITriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp, view)
        {
            _Model = mco switch
            {
                ModelCreateOptions.Dependant => (TriggerNEdgeModel)DsoModel.Default.GetTriggerModel(TriggerType.NEdge),
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
            if (_Model != null)
            {
                _Model.PropertyChanged -= OnPropertyChanged;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrigNEdgePrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView"/>.</param>
        /// <param name="model">The model<see cref="TriggerNEdgeModel"/>.</param>
        internal TrigNEdgePrsnt(IDsoPrsnt idp, ITriggerView view, TriggerNEdgeModel model) : base(idp, view)
        {
            _Model = model;
            LoadEvent();
        }


        /// <summary>
        /// Gets or sets the DurationByps.
        /// </summary>
        public Int64 DurationByps
        {
            get => Model.DurationByps;
            set
            {
                Model.DurationByps = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Double DurationByus
        {
            get => DurationByps / 1000_000D;
            set => DurationByps = (Int64)(value * 1000_000D);
        }

        /// <summary>
        /// Gets the MaxDuration.
        /// </summary>
        public Int64 MaxDuration => Model.MaxDuration;

        /// <summary>
        /// Gets the MinDuration.
        /// </summary>
        public Int64 MinDuration => Model.MinDuration;

        /// <summary>
        /// Gets or sets the Polarity.
        /// </summary>
        public EdgeSlope Polarity
        {
            get => Model.Polarity;
            set
            {
                Model.Polarity = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        public Int32 MaxEdgeNumber => Model.MaxEdgeNumber;

        public Int32 MinEdgeNumber => Model.MinEdgeNumber;

        public Int32 EdgeNumber
        {
            get => Model.EdgeNumber;
            set
            {
                Model.EdgeNumber = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets the Model.
        /// </summary>
        private protected override TriggerNEdgeModel Model => (TriggerNEdgeModel)_Model!;

        /// <summary>
        /// The AdjDuration.
        /// </summary>
        /// <param name="step">The step<see cref="Int64"/>.</param>
        public void AdjDuration(Int64 step)
        {
            if ((Model.DurationByps >= 1E9) && (Model.DurationByps < 1E12))
            {
                Model.DurationByps += step * (Int32)1E6;
            }
            else if (Model.DurationByps >= 1E12)
            {
                Model.DurationByps += step * (Int32)1E9;
            }
            else
            {
                Model.DurationByps += step * Model.StpDuration;
            }
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }
    }
}

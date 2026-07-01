// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    /// <summary>
    /// Defines the <see cref="TrigTimeOutPrsnt" />.
    /// </summary>
    public class TrigTimeOutPrsnt : TrigSingleSrcPrsnt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrigTimeOutPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView?"/>.</param>
        /// <param name="mco">The mco<see cref="ModelCreateOptions"/>.</param>
        public TrigTimeOutPrsnt(IDsoPrsnt idp, ITriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp, view)
        {
            _Model = mco switch
            {
                ModelCreateOptions.Dependant => (TriggerTimeOutModel)DsoModel.Default.GetTriggerModel(TriggerType.TimeOut),
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
        /// Initializes a new instance of the <see cref="TrigTimeOutPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView"/>.</param>
        /// <param name="model">The model<see cref="TriggerTimeOutModel"/>.</param>
        internal TrigTimeOutPrsnt(IDsoPrsnt idp, ITriggerView view, TriggerTimeOutModel model) : base(idp, view)
        {
            _Model = model;
            _Model.PropertyChanged += OnPropertyChanged;
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
        public LevelPolarity Polarity
        {
            get => Model.Polarity;
            set
            {
                Model.Polarity = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets the Model.
        /// </summary>
        private protected override TriggerTimeOutModel Model => (TriggerTimeOutModel)_Model!;

        /// <summary>
        /// The AdjDuration.
        /// </summary>
        /// <param name="step">The step<see cref="Int64"/>.</param>
        public void AdjDuration(Int64 step)
        {
            //Model.DurationByps += step * Model.StpDuration;
            if ((Model.DurationByps >= 1E6) && (Model.DurationByps < 1E9))
            {
                Model.DurationByps += step * (Int32)1E3;
            }
            else if ((Model.DurationByps >= 1E9) && (Model.DurationByps < 1E12))
            {
                Model.DurationByps += step * (Int32)1E6;
            }
            else if (Model.DurationByps >= 1E12)
            {
                Model.DurationByps += step * (Int32)1E9;
            }
            else
            {
                Model.DurationByps += step * Constants.STP_PULSEWIDTH_PS;//Model.DurationByps;
            }
            Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
        }
    }
}

// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    /// <summary>
    /// Defines the <see cref="TrigTransPrsnt" />.
    /// </summary>
    public class TrigTransPrsnt : TrigMultiLevelPrsnt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrigTransPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView?"/>.</param>
        /// <param name="mco">The mco<see cref="ModelCreateOptions"/>.</param>
        public TrigTransPrsnt(IDsoPrsnt idp, ITriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp, view)
        {
            _Model = mco switch
            {
                ModelCreateOptions.Dependant => (TriggerTransModel)DsoModel.Default.GetTriggerModel(TriggerType.Transition),
                ModelCreateOptions.Standalone => new(),
                _ => null,
            };
            LoadEvent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrigTransPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView"/>.</param>
        /// <param name="model">The model<see cref="TriggerTransModel"/>.</param>
        internal TrigTransPrsnt(IDsoPrsnt idp, ITriggerView view, TriggerTransModel model) : base(idp, view)
        {
            _Model = model;
            LoadEvent();
        }

        /// <summary>
        /// 重载参数
        /// </summary>
        public override void LoadEvent()
        {
            if (_Model != null)
            {
                KeyLed.Default.SetLedColor(LedEnum.LedTriggerLevel, System.Drawing.Color.Black);
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
        /// Gets or sets the TransSlope.
        /// </summary>
        public EdgeSlope Slope
        {
            get => Model.Slope;
            set
            {
                Model.Slope = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets the Model.
        /// </summary>
        private protected override TriggerTransModel Model => (TriggerTransModel)_Model!;
    }
}

// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using ScopeX.ComModel;

    /// <summary>
    /// Defines the <see cref="TrigWindowPrsnt" />.
    /// </summary>
    public class TrigWindowPrsnt : TrigMultiLevelPrsnt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrigWindowPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView?"/>.</param>
        /// <param name="mco">The mco<see cref="ModelCreateOptions"/>.</param>
        public TrigWindowPrsnt(IDsoPrsnt idp, ITriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp, view)
        {
            _Model = mco switch
            {
                ModelCreateOptions.Dependant => (TriggerWindowModel)DsoModel.Default.GetTriggerModel(TriggerType.Window),
                ModelCreateOptions.Standalone => new(),
                _ => null,
            };
            LoadEvent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrigWindowPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView"/>.</param>
        /// <param name="model">The model<see cref="TriggerWindowModel"/>.</param>
        internal TrigWindowPrsnt(IDsoPrsnt idp, ITriggerView view, TriggerWindowModel model) : base(idp, view)
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
        /// Gets or sets the PosCompCondition.
        /// </summary>
        public WindowRange PosCondition
        {
            get => Model.PosCondition;
            set
            {
                Model.PosCondition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets or sets the TimeCondition.
        /// </summary>
        public WindowTimeCondition TimeCondition
        {
            get => Model.TimeCondition;
            set
            {
                Model.TimeCondition = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets the Model.
        /// </summary>
        private protected override TriggerWindowModel Model => (TriggerWindowModel)_Model!;
    }
}

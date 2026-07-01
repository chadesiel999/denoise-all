// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using ScopeX.ComModel;

    /// <summary>
    /// Defines the <see cref="TrigRuntPrsnt" />.
    /// </summary>
    public class TrigRuntPrsnt : TrigMultiLevelPrsnt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrigRuntPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView?"/>.</param>
        /// <param name="mco">The mco<see cref="ModelCreateOptions"/>.</param>
        public TrigRuntPrsnt(IDsoPrsnt idp, ITriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp, view)
        {
            _Model = mco switch
            {
                ModelCreateOptions.Dependant => (TriggerRuntModel)DsoModel.Default.GetTriggerModel(TriggerType.Runt),
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
        /// Initializes a new instance of the <see cref="TrigRuntPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView"/>.</param>
        /// <param name="model">The model<see cref="TriggerRuntModel"/>.</param>
        internal TrigRuntPrsnt(IDsoPrsnt idp, ITriggerView view, TriggerRuntModel model) : base(idp, view)
        {
            _Model = model;
            _Model.PropertyChanged += OnPropertyChanged;
        }

        //public String Name => Model.Name;
        /// <summary>
        /// Gets or sets the Polarity.
        /// </summary>
        public PulsePolarity Polarity
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
        private protected override TriggerRuntModel Model => (TriggerRuntModel)_Model!;
    }
}

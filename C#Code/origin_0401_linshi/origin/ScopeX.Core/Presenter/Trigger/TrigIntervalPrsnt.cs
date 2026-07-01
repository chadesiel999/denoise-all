// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using ScopeX.ComModel;

    /// <summary>
    /// Defines the <see cref="TrigIntervalPrsnt" />.
    /// </summary>
    public class TrigIntervalPrsnt : TrigWidthPrsnt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrigIntervalPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView?"/>.</param>
        /// <param name="mco">The mco<see cref="ModelCreateOptions"/>.</param>
        public TrigIntervalPrsnt(IDsoPrsnt idp, ITriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp, view, ModelCreateOptions.InitializedByChild)
        {
            _Model = mco switch
            {
                ModelCreateOptions.Dependant => (TriggerIntervalModel)DsoModel.Default.GetTriggerModel(TriggerType.Interval),
                ModelCreateOptions.Standalone => new(),
                _ => null,
            };
            if (_Model != null)
            {
                _Model.PropertyChanged += OnPropertyChanged;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrigIntervalPrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView"/>.</param>
        /// <param name="model">The model<see cref="TriggerIntervalModel"/>.</param>
        internal TrigIntervalPrsnt(IDsoPrsnt idp, ITriggerView view, TriggerIntervalModel model) : base(idp, view)
        {
            _Model = model;
            _Model.PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Gets the Model.
        /// </summary>
        private protected override TriggerIntervalModel Model => (TriggerIntervalModel)_Model!;
    }
}

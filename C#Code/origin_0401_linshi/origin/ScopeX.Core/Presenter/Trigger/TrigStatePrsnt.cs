// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;
    

    /// <summary>
    /// Defines the <see cref="TrigStatePrsnt" />.
    /// </summary>
    public class TrigStatePrsnt : TrigPatPrsnt
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrigStatePrsnt"/> class.
        /// </summary>
        /// <param name="view">The view<see cref="ITriggerView?"/>.</param>
        /// <param name="mco">The mco<see cref="ModelCreateOptions"/>.</param>
        public TrigStatePrsnt(IDsoPrsnt idp, ITriggerView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp, view, ModelCreateOptions.InitializedByChild)
        {
            _Model = mco switch
            {
                ModelCreateOptions.Dependant => (TriggerStateModel)DsoModel.Default.GetTriggerModel(TriggerType.State),
                ModelCreateOptions.Standalone => new(),
                _ => null,
            };
            if (_Model != null)
            {
                _Model.PropertyChanged += OnPropertyChanged;
            }
        }

        /// <summary>
        /// Gets or sets the ClkPolarity.
        /// </summary>
        public PulsePolarity ClkPolarity
        {
            get => Model.ClkPolarity;
            set
            {
                Model.ClkPolarity = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets or sets the ClkSource.
        /// </summary>
        public ChannelId ClkSource
        {
            get => Model.ClkSource;
            set
            {
                Model.ClkSource = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets or sets the Conformed.
        /// </summary>
        public Boolean Conformed
        {
            get => Model.Conformed;
            set
            {
                Model.Conformed = value;
                Hardware.HdCmdFactory.Push(HdCmd.TrigTypeAndParameters);
            }
        }

        /// <summary>
        /// Gets the Model.
        /// </summary>
        private protected override TriggerStateModel Model => (TriggerStateModel)_Model!;
    }
}

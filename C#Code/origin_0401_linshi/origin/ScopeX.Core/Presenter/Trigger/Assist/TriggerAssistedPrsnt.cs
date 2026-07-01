using System;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public class TriggerAssistedPrsnt : MulticastPrsnt<ITriggerAssistedView>, ITriggerAssistedPrsnt
    {
        public TriggerAssistedPrsnt(IDsoPrsnt idp, ITriggerAssistedView? view = null) : base(idp)
        {
            Model = DsoModel.Default.TriggerAssisted;
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public Boolean Enabled
        {
            get => Model.Enabled;
            set
            {
                Model.Enabled = value;
                if (!value)
                {
                    LocationAssistedEnabled = false;
                    VisualTriggerEnabled = false;
                    Hardware.HdCmdFactory.Push(HdCmd.TrigAreas);
                }
            }
        }
        public Boolean LocationAssistedEnabled { get => Model.LocationAssistedEnabled; set => Model.LocationAssistedEnabled = value; }
        public Boolean VisualTriggerEnabled { get => Model.VisualTriggerEnabled; set => Model.VisualTriggerEnabled = value; }

        private protected override TriggerAssistedModel Model { get; }
    }
}

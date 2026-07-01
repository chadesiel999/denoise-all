namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    public class LocationAssistedPrsnt : MulticastPrsnt<ILocationAssistedView>, ILocationAssistedPrsnt
    {
      
        public LocationAssistedPrsnt(IDsoPrsnt idp, ILocationAssistedView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.LocAssisted,
                ModelCreateOptions.Standalone => new(DsoModel.Default.Meas),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public CompareCondition Condition { get => Model.Condition; set => Model.Condition = value; }

        public Boolean Enabled { get => Model.Enabled; set => Model.Enabled = value; }

        public Double MaxThreshold => Model.MaxThreshold;

        public String MeasName { get => Model.MeasName; set => Model.MeasName = value; }

        public String MeasUnit => MeasPrsnt.GetPfxUnitString(Model.MeasName, Model.Source).Unit;

        public Double MinThreshold => Model.MinThreshold;

        public ChannelId Source { get => Model.Source; set => Model.Source = value; }

        public Double LowerThreshold
        { 
            get => Model.LowerThreshold;
            set => Model.LowerThreshold = value; 
        }

        public Double UpperThreshold 
        { 
            get => Model.UpperThreshold; 
            set => Model.UpperThreshold = value;
        }

        private protected override LocationAssistedModel Model { get; }
    }
}

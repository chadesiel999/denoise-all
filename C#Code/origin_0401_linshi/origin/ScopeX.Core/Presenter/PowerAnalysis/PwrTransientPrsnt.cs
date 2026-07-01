using System;
using System.Collections.Generic;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.PowerAnalysis
{
    public class PwrTransientPrsnt: MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {
        public PwrTransientPrsnt(IDsoPrsnt idp, IPwrOptionView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.PwrAnalysis.Transient,
                ModelCreateOptions.Standalone => new(DsoModel.Default.PwrAnalysis, DsoModel.Default.Meas),
                _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            };
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public Int32 Count => Model.Count;

        public TransientItems Transient => new(nameof(Transient), Model[nameof(Transient)].Current, QuantityUnit.Ampere);

        public IEnumerable<TransientItems> Items
        {
            get
            {
                //  yield return Starttime;
                //  yield return Endtime;
                yield return Transient;
            }
        }

        public VIType Source
        {
            get => Model.Source;
            set => Model.Source = value;
        }

        public Double StableValue //稳定输出电压
        {
            get => Model.StableValue;
            set => Model.StableValue = value;
        }

        public Double Overshoot //过冲百分比
        {
            get => Model.Overshoot;
            set => Model.Overshoot = value;
        }

        private protected override PwrTransientModel Model { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public void TryShowTransientWfm(MathPrsnt? mp)
        {
            if (mp is not null)
            {
                mp.Args.Occupier = null;
                mp.Label = "";
                mp.Active = false;
                mp.IsAutoUnit = false;
            }
        }

        public record TransientItems(String Name, Double? Value, QuantityUnit Unit);
    }
}

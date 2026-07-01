// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/12</date>

namespace ScopeX.Core.PowerAnalysis
{
    using System;

    public class PwrDifferPrsnt : MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {
        public PwrDifferPrsnt(IDsoPrsnt idp, IPwrOptionView? view = null, ModelCreateOptions mco = ModelCreateOptions.Dependant) : base(idp)
        {
            Model = mco switch
            {
                ModelCreateOptions.Dependant => DsoModel.Default.PwrAnalysis.Differ,
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

        public DifferItems SlewRate => new(nameof(SlewRate), Model["Max"].Current, Model["Min"].Current, Model.Unit);

        public VIType Source { get => Model.Source; set => Model.Source = value; }

        public Boolean ValidExp => Model.ValidExp;

        private protected override PwrDifferModel Model { get; }

        public void TryShowDifferWfm(MathPrsnt? mp)
        {
            if (mp is not null)
            {
                if (mp.Formula != Model.Formula)
                {
                    mp.GetOrMakeArg(MathType.Custom);
                    mp.Formula = Model.Formula;
                }

                mp.Args.Occupier = Model.Analysis;
                mp.Label = Source == VIType.V ? "dV/dt" : "dI/dt";
                mp.Active = true;
                mp.IsAutoUnit = false;

                Model.BoundMathId = mp.Id;
            }
        }

        public record DifferItems(String Name, Double? Max, Double? Min, String Unit);
    }
}

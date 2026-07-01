namespace ScopeX.Core.PowerAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    public class PwrEfficiencyPrsnt : MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {
        public PwrEfficiencyPrsnt(IDsoPrsnt idp, ChannelId id = ChannelId.POWER1, IPwrOptionView? view = null) : base(idp)
        {
            Model = DsoModel.Default.GetPowerChannel(id)?.Efficiency;

            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public Int32 Count => Model.Count;
        public Action? VuTryAddPwr1;
        public Action? VuTryAddPwr2;
        public EfficiencyItems InTruePower =>
                new(nameof(InTruePower),
                Model[nameof(InTruePower)].Current,
                Model[nameof(InTruePower)].StaBuffer.Average,
                Model[nameof(InTruePower)].StaBuffer.Max,
                Model[nameof(InTruePower)].StaBuffer.Min,
                QuantityUnit.Watt);

        public EfficiencyItems OutTruePower =>
                new(nameof(OutTruePower),
                Model[nameof(OutTruePower)].Current,
                Model[nameof(OutTruePower)].StaBuffer.Average,
                Model[nameof(OutTruePower)].StaBuffer.Max,
                Model[nameof(OutTruePower)].StaBuffer.Min,
                QuantityUnit.Watt);

        public EfficiencyItems Efficiency =>
                new(nameof(Efficiency),
                Model[nameof(Efficiency)].Current,
                Model[nameof(Efficiency)].StaBuffer.Average,
                Model[nameof(Efficiency)].StaBuffer.Max,
                Model[nameof(Efficiency)].StaBuffer.Min,
                QuantityUnit.Percent);


        public IEnumerable<EfficiencyItems> Efficiencies
        {
            get
            {
                yield return InTruePower;
                yield return OutTruePower;
                yield return Efficiency;
            }
        }

        public ChannelId InCurrentSrc { get => Model.InCurrentSrc; set => Model.InCurrentSrc = value; }

        public ChannelId InVoltageSrc { get => Model.InVoltageSrc; set => Model.InVoltageSrc = value; }

        public ChannelId OutCurrentSrc { get => Model.OutCurrentSrc; set => Model.OutCurrentSrc = value; }

        public ChannelId OutVoltageSrc { get => Model.OutVoltageSrc; set => Model.OutVoltageSrc = value; }

        public CurrentType InputType { get => Model.InputType; set => Model.InputType = value; }

        public CurrentType OutputType { get => Model.OutputType; set => Model.OutputType = value; }

        private protected override PwrEfficiencyModel Model { get; }

        public void TryShowEfficiencyWfmPower1(MathPrsnt? mp)
        {
            if (mp is not null)
            {
                if (mp.Formula != Model.Formula1)
                {
                    mp.GetOrMakeArg(MathType.Custom);
                    mp.Formula = Model.Formula1;
                }

                mp.Args.Occupier = Model.Analysis;
                mp.Label = "InputPower";
                mp.Active = true;
                mp.IsAutoUnit = true;

                Model.BoundMathId1 = mp.Id;
            }
        }

        public void TryShowEfficiencyWfmPower2(MathPrsnt? mp)
        {
            if (mp is not null)
            {
                if (mp.Formula != Model.Formula2)
                {
                    mp.GetOrMakeArg(MathType.Custom);
                    mp.Formula = Model.Formula2;
                }

                mp.Args.Occupier = Model.Analysis;
                mp.Label = "OutputPower";
                mp.Active = true;
                mp.IsAutoUnit = true;

                Model.BoundMathId2 = mp.Id;
            }
        }

        public String EfficiencyTableData()
        {
            var outputstring = "Value,Average,Max,Min";
            outputstring += Environment.NewLine;
            foreach (var item in Efficiencies)
            {
                outputstring += item.Name + ",";
                if (item.Value == null || item.Value.ToString() == Double.NaN.ToString())
                {
                    outputstring += Double.MaxValue.ToString("E5") + ",";
                }
                else
                {
                    outputstring += ((Double)item.Value).ToString("E5") + ",";
                }
                if (item.Mean == null || item.Value.ToString() == Double.NaN.ToString())
                {
                    outputstring += Double.MaxValue.ToString("E5") + ",";
                }
                else
                {
                    outputstring += ((Double)item.Mean).ToString("E5") + ",";
                }
                if (item.Max == null || item.Value.ToString() == Double.NaN.ToString())
                {
                    outputstring += Double.MaxValue.ToString("E5") + ",";
                }
                else
                {
                    outputstring += ((Double)item.Max).ToString("E5") + ",";
                }
                if (item.Min == null || item.Value.ToString() == Double.NaN.ToString())
                {
                    outputstring += Double.MaxValue.ToString("E5") + ",";
                }
                else
                {
                    outputstring += ((Double)item.Min).ToString("E5") + ",";
                }
                outputstring += Environment.NewLine;
            }
            return outputstring;
        }
        public record EfficiencyItems(String Name, Double? Value, Double? Mean, Double? Max, Double? Min, QuantityUnit Unit);
    }
}

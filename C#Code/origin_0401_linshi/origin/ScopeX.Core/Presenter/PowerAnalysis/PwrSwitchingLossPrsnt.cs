using System;
using System.Collections.Generic;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.PowerAnalysis
{
    public class PwrSwitchingLossPrsnt : MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {
        public PwrSwitchingLossPrsnt(IDsoPrsnt idp, ChannelId id = ChannelId.POWER1, IPwrOptionView? view = null) : base(idp)
        {
            Model = DsoModel.Default.GetPowerChannel(id)?.SwitchingLoss;

            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public Int32 Count => Model.Count;
        public String Titles => Model.Titles;
        public Double MaxRdsOn => PwrSwitchingLossModel.MaxRdsOn;
        public Double MinRdsOn => PwrSwitchingLossModel.MinRdsOn;

        public Double RdsOn
        {
            get
            {
               return Model.RdsOn;
            }
            set
            {

                if (value > MaxRdsOn)
                {
                    WeakTip.Default.Write(nameof(RdsOn), MsgTipId.GreatethanMax, false, "", 1);
                    value = MaxRdsOn;
                }
                if(value < MinRdsOn)
                {
                    WeakTip.Default.Write(nameof(RdsOn), MsgTipId.LessthanMin, false, "", 1);
                    value = MinRdsOn;
                }

                if (value!=Model.RdsOn)
                {
                    Model.RdsOn = value;
                }
            }

        }

        public SwitchingLossItems PowerTOn => new(
            nameof(PowerTOn),
            Model[nameof(PowerTOn)].Current,
            Model[nameof(PowerTOn)].StaBuffer.Average,
            Model[nameof(PowerTOn)].StaBuffer.Max,
            Model[nameof(PowerTOn)].StaBuffer.Min,
            QuantityUnit.Watt);

        public SwitchingLossItems PowerTConduct => new(
            nameof(PowerTConduct),
            Model[nameof(PowerTConduct)].Current,
            Model[nameof(PowerTConduct)].StaBuffer.Average,
            Model[nameof(PowerTConduct)].StaBuffer.Max,
            Model[nameof(PowerTConduct)].StaBuffer.Min,
            QuantityUnit.Watt);

        public SwitchingLossItems PowerTOff => new(
            nameof(PowerTOff),
            Model[nameof(PowerTOff)].Current,
            Model[nameof(PowerTOff)].StaBuffer.Average,
            Model[nameof(PowerTOff)].StaBuffer.Max,
            Model[nameof(PowerTOff)].StaBuffer.Min,
            QuantityUnit.Watt);

        public SwitchingLossItems PowerTNonConduct => new(
            nameof(PowerTNonConduct),
            Model[nameof(PowerTNonConduct)].Current,
            Model[nameof(PowerTNonConduct)].StaBuffer.Average,
            Model[nameof(PowerTNonConduct)].StaBuffer.Max,
            Model[nameof(PowerTNonConduct)].StaBuffer.Min,
            QuantityUnit.Watt);

        public SwitchingLossItems PowerTotal => new(
            nameof(PowerTotal),
            Model[nameof(PowerTotal)].Current,
            Model[nameof(PowerTotal)].StaBuffer.Average,
            Model[nameof(PowerTotal)].StaBuffer.Max,
            Model[nameof(PowerTotal)].StaBuffer.Min,
            QuantityUnit.Watt);

        public SwitchingLossItems EnergyTOn => new(
            nameof(EnergyTOn),
            Model[nameof(EnergyTOn)].Current,
            Model[nameof(EnergyTOn)].StaBuffer.Average,
            Model[nameof(EnergyTOn)].StaBuffer.Max,
            Model[nameof(EnergyTOn)].StaBuffer.Min,
            QuantityUnit.Joule);

        public SwitchingLossItems EnergyTConduct => new(
            nameof(EnergyTConduct),
            Model[nameof(EnergyTConduct)].Current,
            Model[nameof(EnergyTConduct)].StaBuffer.Average,
            Model[nameof(EnergyTConduct)].StaBuffer.Max,
            Model[nameof(EnergyTConduct)].StaBuffer.Min,
            QuantityUnit.Joule);

        public SwitchingLossItems EnergyTOff => new(
            nameof(EnergyTOff),
            Model[nameof(EnergyTOff)].Current,
            Model[nameof(EnergyTOff)].StaBuffer.Average,
            Model[nameof(EnergyTOff)].StaBuffer.Max,
            Model[nameof(EnergyTOff)].StaBuffer.Min,
            QuantityUnit.Joule);

        public SwitchingLossItems EnergyTNonConduct => new(
            nameof(EnergyTNonConduct),
            Model[nameof(EnergyTNonConduct)].Current,
            Model[nameof(EnergyTNonConduct)].StaBuffer.Average,
            Model[nameof(EnergyTNonConduct)].StaBuffer.Max,
            Model[nameof(EnergyTNonConduct)].StaBuffer.Min,
            QuantityUnit.Joule);

        public SwitchingLossItems EnergyTotal => new(
            nameof(EnergyTotal),
            Model[nameof(EnergyTotal)].Current,
            Model[nameof(EnergyTotal)].StaBuffer.Average,
            Model[nameof(EnergyTotal)].StaBuffer.Max,
            Model[nameof(EnergyTotal)].StaBuffer.Min,
            QuantityUnit.Joule);     
        
        public SwitchingLossItems CycleCount => new(
            nameof(CycleCount),
            Model[nameof(CycleCount)].Current,
            Model[nameof(CycleCount)].StaBuffer.Average,
            Model[nameof(CycleCount)].StaBuffer.Max,
            Model[nameof(CycleCount)].StaBuffer.Min,
            QuantityUnit.Count);

        public IEnumerable<SwitchingLossItems> SwitchingLoss
        {
            get
            {
                yield return PowerTOn;
                yield return PowerTConduct;
                yield return PowerTOff;
                //yield return PowerTNonConduct;
                yield return PowerTotal;
                yield return EnergyTOn;
                yield return EnergyTConduct;
                yield return EnergyTOff;
                yield return EnergyTotal;
            }
        }

        public VIType Source
        {
            get => Model.Source;
            set => Model.Source = value;
        }

        public Boolean Statistics
        {
            get => Model.Statistics;
            set => Model.Statistics = value;
        }

        public Boolean CalcCompleted
        {
            get => Model.RunCompleted;
            set => Model.RunCompleted = value;
        }

        private protected override PwrSwitchingLossModel Model
        {
            get;
        }

        public void Reset() => Model.Reset();

        public void TryShowPowerWfm(MathPrsnt? mp)
        {
            if (mp is not null)
            {
                if (mp.Formula != Model.Formula)
                {
                    mp.GetOrMakeArg(MathType.Custom);
                    mp.Formula = Model.Formula;
                }

                mp.Args.Occupier = Model.Analysis;
                mp.Label = "VI";
                mp.Active = true;
                mp.IsAutoUnit = false;

                Model.BoundMathId = mp.Id;
            }
        }

        public record SwitchingLossItems(String Name, Double? Value, Double? Mean, Double? Max, Double? Min, QuantityUnit Unit);

        public String GetSwLossTableData()
        {
            var outputstring= String.Empty;
            foreach (var i in SwitchingLoss)
            {
                //outputstring += i.Name.ToString()+",";
                if (i.Value == null)
                    outputstring += Quantity.ConvertByPrefix(Double.MaxValue, Prefix.Empty).ToString("E5");
                else
                    outputstring += Quantity.ConvertByPrefix((Double)i.Value, Prefix.Empty).ToString("E5");

                if (i.Mean == null)
                    outputstring += Quantity.ConvertByPrefix(Double.MaxValue, Prefix.Empty).ToString("E5");
                else
                    outputstring += Quantity.ConvertByPrefix((Double)i.Mean, Prefix.Empty).ToString("E5");

                if (i.Max == null)
                    outputstring += Quantity.ConvertByPrefix(Double.MaxValue, Prefix.Empty).ToString("E5");
                else
                    outputstring += Quantity.ConvertByPrefix((Double)i.Max, Prefix.Empty).ToString("E5");

                if (i.Min == null)
                    outputstring += Quantity.ConvertByPrefix(Double.MaxValue, Prefix.Empty).ToString("E5");
                else
                    outputstring += Quantity.ConvertByPrefix((Double)i.Min, Prefix.Empty).ToString("E5");
                outputstring += Environment.NewLine;
            }
            return outputstring;
        }
    }
}

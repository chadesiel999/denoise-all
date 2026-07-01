using ScopeX.ComModel;
using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;

namespace ScopeX.Core.PowerAnalysis
{
     public class PwrSlewRatePrsnt : MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {
        public PwrSlewRatePrsnt(IDsoPrsnt idp, ChannelId id = ChannelId.POWER1, IPwrOptionView? view = null) : base(idp)
        {
            Model = DsoModel.Default.GetPowerChannel(id)?.SlewRate;

            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }
        public Action? VuTryAddDvdtUI;
        public Action? VuTryAddDidtUI;
        public Int32 Count => Model.Count;

        public SlewRateItems VoltageRaiseRateMax =>
                new(nameof(VoltageRaiseRateMax),
                Model[nameof(VoltageRaiseRateMax)].Current,
                Model[nameof(VoltageRaiseRateMax)].StaBuffer.Average,
                Model[nameof(VoltageRaiseRateMax)].StaBuffer.Max,
                Model[nameof(VoltageRaiseRateMax)].StaBuffer.Min,
                Model.VoltageRateUnit);

        public SlewRateItems VoltageRaiseRateMin =>
               new(nameof(VoltageRaiseRateMin),
               Model[nameof(VoltageRaiseRateMin)].Current,
               Model[nameof(VoltageRaiseRateMin)].StaBuffer.Average,
               Model[nameof(VoltageRaiseRateMin)].StaBuffer.Max,
               Model[nameof(VoltageRaiseRateMin)].StaBuffer.Min,
               Model.VoltageRateUnit);

        public SlewRateItems VoltageRaiseRatePk2Pk =>
                new(nameof(VoltageRaiseRatePk2Pk),
                Model[nameof(VoltageRaiseRatePk2Pk)].Current,
                Model[nameof(VoltageRaiseRatePk2Pk)].StaBuffer.Average,
                Model[nameof(VoltageRaiseRatePk2Pk)].StaBuffer.Max,
                Model[nameof(VoltageRaiseRatePk2Pk)].StaBuffer.Min,
                Model.VoltageRateUnit);

        public SlewRateItems VoltageFallRateMax =>
                new(nameof(VoltageFallRateMax),
                Model[nameof(VoltageFallRateMax)].Current,
                Model[nameof(VoltageFallRateMax)].StaBuffer.Average,
                Model[nameof(VoltageFallRateMax)].StaBuffer.Max,
                Model[nameof(VoltageFallRateMax)].StaBuffer.Min,
                Model.VoltageRateUnit);

        public SlewRateItems VoltageFallRateMin =>
               new(nameof(VoltageFallRateMin),
               Model[nameof(VoltageFallRateMin)].Current,
               Model[nameof(VoltageFallRateMin)].StaBuffer.Average,
               Model[nameof(VoltageFallRateMin)].StaBuffer.Max,
               Model[nameof(VoltageFallRateMin)].StaBuffer.Min,
               Model.VoltageRateUnit);

        public SlewRateItems VoltageFallRatePk2Pk =>
                new(nameof(VoltageFallRatePk2Pk),
                Model[nameof(VoltageFallRatePk2Pk)].Current,
                Model[nameof(VoltageFallRatePk2Pk)].StaBuffer.Average,
                Model[nameof(VoltageFallRatePk2Pk)].StaBuffer.Max,
                Model[nameof(VoltageFallRatePk2Pk)].StaBuffer.Min,
                Model.VoltageRateUnit);

        public SlewRateItems CurrentRaiseRateMax =>
                new(nameof(CurrentRaiseRateMax),
                Model[nameof(CurrentRaiseRateMax)].Current,
                Model[nameof(CurrentRaiseRateMax)].StaBuffer.Average,
                Model[nameof(CurrentRaiseRateMax)].StaBuffer.Max,
                Model[nameof(CurrentRaiseRateMax)].StaBuffer.Min,
                Model.CurrentRateUnit);

        public SlewRateItems CurrentRaiseRateMin =>
               new(nameof(CurrentRaiseRateMin),
               Model[nameof(CurrentRaiseRateMin)].Current,
               Model[nameof(CurrentRaiseRateMin)].StaBuffer.Average,
               Model[nameof(CurrentRaiseRateMin)].StaBuffer.Max,
               Model[nameof(CurrentRaiseRateMin)].StaBuffer.Min,
               Model.CurrentRateUnit);

        public SlewRateItems CurrentRaiseRatePk2Pk =>
                new(nameof(CurrentRaiseRatePk2Pk),
                Model[nameof(CurrentRaiseRatePk2Pk)].Current,
                Model[nameof(CurrentRaiseRatePk2Pk)].StaBuffer.Average,
                Model[nameof(CurrentRaiseRatePk2Pk)].StaBuffer.Max,
                Model[nameof(CurrentRaiseRatePk2Pk)].StaBuffer.Min,
                Model.CurrentRateUnit);

        public SlewRateItems CurrentFallRateMax =>
                new(nameof(CurrentFallRateMax),
                Model[nameof(CurrentFallRateMax)].Current,
                Model[nameof(CurrentFallRateMax)].StaBuffer.Average,
                Model[nameof(CurrentFallRateMax)].StaBuffer.Max,
                Model[nameof(CurrentFallRateMax)].StaBuffer.Min,
                Model.CurrentRateUnit);

        public SlewRateItems CurrentFallRateMin =>
               new(nameof(CurrentFallRateMin),
               Model[nameof(CurrentFallRateMin)].Current,
               Model[nameof(CurrentFallRateMin)].StaBuffer.Average,
               Model[nameof(CurrentFallRateMin)].StaBuffer.Max,
               Model[nameof(CurrentFallRateMin)].StaBuffer.Min,
               Model.CurrentRateUnit);

        public SlewRateItems CurrentFallRatePk2Pk =>
                new(nameof(CurrentFallRatePk2Pk),
                Model[nameof(CurrentFallRatePk2Pk)].Current,
                Model[nameof(CurrentFallRatePk2Pk)].StaBuffer.Average,
                Model[nameof(CurrentFallRatePk2Pk)].StaBuffer.Max,
                Model[nameof(CurrentFallRatePk2Pk)].StaBuffer.Min,
                Model.CurrentRateUnit);


        public IEnumerable<SlewRateItems> SlewRates
        {
            get
            {
                yield return VoltageRaiseRateMax;
                yield return VoltageRaiseRateMin;
                yield return VoltageRaiseRatePk2Pk;
                yield return VoltageFallRateMax;
                yield return VoltageFallRateMin;
                yield return VoltageFallRatePk2Pk;
                yield return CurrentRaiseRateMax;
                yield return CurrentRaiseRateMin;
                yield return CurrentRaiseRatePk2Pk;
                yield return CurrentFallRateMax;
                yield return CurrentFallRateMin;
                yield return CurrentFallRatePk2Pk;

            }
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
        private protected override PwrSlewRateModel Model { get; }

        public void TryShowSlewRateWfmVoltageRate(MathPrsnt? mp)
        {
            if (mp is not null)
            {
                if (mp.Formula != Model.Formula1)
                {
                    mp.GetOrMakeArg(MathType.Custom);
                    mp.Formula = Model.Formula1;
                }

                mp.Args.Occupier = Model.Analysis;
                mp.Label = "dv/dt";
                mp.Active = true;
                mp.IsAutoUnit = true;
                mp.CustomUnit = Model.VoltageRateUnit.ToString();

                Model.BoundMathId1 = mp.Id;
            }
        }
        public String SlewRateTableData()
        {
            var outputstring = "Value,Average,Max,Min";
            outputstring += Environment.NewLine;
            foreach (var item in SlewRates)
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
        public void TryShowSlewRateWfmCurrentRate(MathPrsnt? mp)
        {
            if (mp is not null)
            {
                if (mp.Formula != Model.Formula2)
                {
                    mp.GetOrMakeArg(MathType.Custom);
                    mp.Formula = Model.Formula2;
                }

                mp.Args.Occupier = Model.Analysis;
                mp.Label = "di/dt";
                mp.Active = true;
                mp.IsAutoUnit = true;
                mp.CustomUnit = Model.CurrentRateUnit.ToString();

                Model.BoundMathId2 = mp.Id;
            }
        }

        public record SlewRateItems(String Name, Double? Value, Double? Mean, Double? Max, Double? Min, String Unit);
    }
}

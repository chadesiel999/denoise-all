namespace ScopeX.Core.PowerAnalysis
{
    using System;
    using System.Collections.Generic;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    public class PwrQualityPrsnt : MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {
        public PwrQualityPrsnt(IDsoPrsnt idp, ChannelId id = ChannelId.POWER1, IPwrOptionView? view = null) : base(idp)
        {
            Model = DsoModel.Default.GetPowerChannel(id)?.Quality;

            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }
 
        public Int32 Count => Model.Count;
        public String Titles=>Model.Titles;
        
        public QualityItems ApparantPower => new(
            nameof(ApparantPower),
            Model[nameof(ApparantPower)].Current,
            Model[nameof(ApparantPower)].StaBuffer.Average,
            Model[nameof(ApparantPower)].StaBuffer.Max,
            Model[nameof(ApparantPower)].StaBuffer.Min,
            QuantityUnit.VA);

        public QualityItems Frequency => new(
            nameof(Frequency),
            Model[nameof(Frequency)].Current,
            Model[nameof(Frequency)].StaBuffer.Average,
            Model[nameof(Frequency)].StaBuffer.Max,
            Model[nameof(Frequency)].StaBuffer.Min,
            QuantityUnit.Hertz);

        public QualityItems ICrestFactor => new(
            nameof(ICrestFactor),
            Model[nameof(ICrestFactor)].Current,
            Model[nameof(ICrestFactor)].StaBuffer.Average,
            Model[nameof(ICrestFactor)].StaBuffer.Max,
            Model[nameof(ICrestFactor)].StaBuffer.Min,
            QuantityUnit.Constant);

        public QualityItems Irms => new(
            nameof(Irms),
            Model[nameof(Irms)].Current,
            Model[nameof(Irms)].StaBuffer.Average,
            Model[nameof(Irms)].StaBuffer.Max,
            Model[nameof(Irms)].StaBuffer.Min,
            QuantityUnit.Ampere);

        public QualityItems Phase => new(
            nameof(Phase),
            Model[nameof(Phase)].Current,
            Model[nameof(Phase)].StaBuffer.Average,
            Model[nameof(Phase)].StaBuffer.Max,
            Model[nameof(Phase)].StaBuffer.Min,
            QuantityUnit.Angle);

        public QualityItems PwrFactor => new(
            nameof(PwrFactor),
            Model[nameof(PwrFactor)].Current,
            Model[nameof(PwrFactor)].StaBuffer.Average,
            Model[nameof(PwrFactor)].StaBuffer.Max,
            Model[nameof(PwrFactor)].StaBuffer.Min,
            QuantityUnit.Constant);

        public QualityItems ReactivePower => new(
            nameof(ReactivePower),
            Model[nameof(ReactivePower)].Current,
            Model[nameof(ReactivePower)].StaBuffer.Average,
            Model[nameof(ReactivePower)].StaBuffer.Max,
            Model[nameof(ReactivePower)].StaBuffer.Min,
            QuantityUnit.Var);

        public QualityItems TruePower => new(
            nameof(TruePower),
            Model[nameof(TruePower)].Current,
            Model[nameof(TruePower)].StaBuffer.Average,
            Model[nameof(TruePower)].StaBuffer.Max,
            Model[nameof(TruePower)].StaBuffer.Min,
            QuantityUnit.Watt);

        public QualityItems VCrestFactor => new(
            nameof(VCrestFactor),
            Model[nameof(VCrestFactor)].Current,
            Model[nameof(VCrestFactor)].StaBuffer.Average,
            Model[nameof(VCrestFactor)].StaBuffer.Max,
            Model[nameof(VCrestFactor)].StaBuffer.Min,
            QuantityUnit.Constant);

        public QualityItems Vrms => new(
            nameof(Vrms),
            Model[nameof(Vrms)].Current,
            Model[nameof(Vrms)].StaBuffer.Average,
            Model[nameof(Vrms)].StaBuffer.Max,
            Model[nameof(Vrms)].StaBuffer.Min,
            QuantityUnit.Voltage);

        public IEnumerable<QualityItems> Qualities
        {
            get
            {
                yield return Vrms;
                yield return VCrestFactor;
                yield return Frequency;
                yield return Irms;
                yield return ICrestFactor;
                yield return TruePower;
                yield return ApparantPower;
                yield return ReactivePower;
                yield return PwrFactor;
                yield return Phase;
            }
        }

        public VIType RefFreq { get => Model.RefFreq; set => Model.RefFreq = value; }

        public Boolean Statistics { get => Model.Statistics; set => Model.Statistics = value; }

        public Boolean CompletionFlag { get => Model.CompletionFlag; }

        private protected override PwrQualityModel Model { get; }

        public void Reset()
        {
            Model.Reset();
        }
        
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
                mp.IsAutoUnit = true;
                mp.CustomUnit = QuantityUnit.Watt.ToUnitString();

                Model.BoundMathId = mp.Id;
            }
        }

        public record QualityItems(String Name, Double? Value, Double? Mean, Double? Max, Double? Min, QuantityUnit Unit);

        public String QualityTableData()
        {
            var outputstring = String.Empty;
            foreach (var item in Qualities)
            {
                if (item.Value == null)
                {
                    outputstring += Double.MaxValue.ToString("E5")+",";
                    
                }
                else
                {
                    outputstring += ((Double)item.Value).ToString("E5") + ",";
                }
                if (item.Mean == null)
                {
                    outputstring += Double.MaxValue.ToString("E5") + ",";
                    
                }
                else
                {
                    outputstring += ((Double)item.Mean).ToString("E5") + ",";
                }
                if (item.Max == null)
                {
                    outputstring += Double.MaxValue.ToString("E5") + ",";
                    
                }
                else
                {
                    outputstring += ((Double)item.Max).ToString("E5") + ",";
                }
                if (item.Min == null)
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
    }
}

namespace ScopeX.Core.PowerAnalysis
{
    using System;
    using System.Collections.Generic;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    public class PwrHarmonicPrsnt :MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {
        private MathPrsnt _MathPrsnt = null;
        public PwrHarmonicPrsnt(IDsoPrsnt idp, ChannelId id = ChannelId.POWER1, IPwrOptionView? view = null) : base(idp)
        {
            Model = DsoModel.Default.GetPowerChannel(id)?.Harmonic;
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public Int32 Count => Model.Count;

        public DistortionItem THDF => new(nameof(THDF), Model[nameof(THDF)].Current, QuantityUnit.Percent);

        public DistortionItem THDR => new(nameof(THDR), Model[nameof(THDR)].Current, QuantityUnit.Percent);

        public DistortionItem THDrms => new(nameof(THDrms), Model[nameof(THDrms)].Current, Source == VIType.V ? QuantityUnit.Voltage : QuantityUnit.Ampere);

        public IEnumerable<DistortionItem> Distortions
        {
            get
            {
                yield return THDR;
                yield return THDF;
                yield return THDrms;
            }
        }

        public Int64 CustomRefFreq { get => Model.CustomRefFreq; set => Model.CustomRefFreq = value; }

        public Double CustomRefFreqBymHz
        {
            get => CustomRefFreq * 1_000D;
            set => CustomRefFreq = (Int64)(value / 1000D);
        }

        public readonly Int32 MaxCustomFreq = PwrHarmonicModel.MAX_CUSTOM_FREQ;

        public readonly Int32 MinCustomFreq = PwrHarmonicModel.MIN_CUSTOM_FREQ;

        public Int32 MinHarmonicNum => Model.MinHarmonicNum;
        public Int32 MaxHarmonicNum => Model.MaxHarmonicNum;
        public String Titles => Model.Titles;
        public Int32 HarmonicNum
        {
            get => Model.HarmonicNum;
            set
            {
                Model.HarmonicNum = value;
                if (_MathPrsnt.Args is MathHistArg histArg)
                {
                    histArg.NBins = Model.HarmonicNum;
                }
            }
        }
        public Int32 HarmonicNumIdx
        {
            get => Model.HarmonicNumIdx;
            set
            {
                Model.HarmonicNumIdx = value;
                if (_MathPrsnt.Args is MathHistArg histArg)
                {
                    histArg.NBins = Model.HarmonicNum;
                }
            }
        }

        public (Prefix Prefix, String Name) HarmonicUnit
        {
            get => Model.HarmonicUnit;
        }
        public Boolean RunCompleted
        {
            get => Model.RunCompleted;
        }
        public Int64? WindowId
        {
            get => Model.WindowId;
            set => Model.WindowId = value;
        }

        public HarmonicDisplayOpt HarmonicOpt { get => Model.HarmonicOpt; set => Model.HarmonicOpt = value; }
        public List<Int32> HarmonicIndexes=> Model.HarmonicIndexes;

        public HarmonicRefFreqSrc RefFreqSrc { get => Model.RefFreqSrc; set => Model.RefFreqSrc = value; }

        public VIType Source { get => Model.Source; set => Model.Source = value; }
        public SweepType Unit { get => Model.Unit; set => Model.Unit = value; }

        private protected override PwrHarmonicModel Model { get; }

        public Double[,]? Magnitude => Model.Magnitude;

        public Double[,]? Phase => Model.Phase;

        public void AdjRefFreq(Int32 step) => Model.AdjRefFreq(step);

        public Double GetHarmonicFreq(Int32 i) => Model.GetHarmonicFreq(i);

        public Double GetMagRatio(Int32 i) => Model.GetMagRatio(i);

        public Double GetMagRMS(Int32 i)
        {
        {
            Double value = Model.GetMagRMS(i);
            if (Double.IsFinite(value))
            {
               value= Unit == SweepType.Linear? value : (20 * Math.Log10(value * 1E6));//固定转换为mV再取对数
            }

            return value;
        }
        }

        public Double GetPhase(Int32 i) => Model.GetPhase(i);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]

        public void TryShowHarmonicWfm(MathPrsnt? mp)
        {
            if (mp is not null)
            {
                _MathPrsnt = mp;
                mp.Args.Occupier = null;
                mp.Label = "";
                mp.IsAutoUnit = false;

            }
        }
       
        public record DistortionItem(String Name, Double? Value, QuantityUnit Unit);

        public String GetHarmonicTableData()
        {
            var outputstring = String.Empty;
            if(THDR.Value==null)
                outputstring += THDR.Name + "," + (Double.MaxValue).ToString("E5")+Environment.NewLine;
            else
                outputstring += THDR.Name + "," + ((Double)THDR.Value).ToString("E5") + Environment.NewLine;
            if (THDF.Value == null)
                outputstring += THDF.Name + "," + (Double.MaxValue).ToString("E5") + Environment.NewLine;
            else
                outputstring += THDF.Name + "," + ((Double)THDF.Value).ToString("E5") + Environment.NewLine;
            if (THDrms.Value == null)
                outputstring += THDrms.Name + "," + (Double.MaxValue).ToString("E5") + Environment.NewLine;
            else
                outputstring += THDrms.Name + "," + ((Double)THDrms.Value).ToString("E5") + Environment.NewLine;				
				
            outputstring += Titles+","+Environment.NewLine;
            var temp = HarmonicIndexes;
            QuantityUnit magrmsunit = QuantityUnit.Voltage;
            QuantityUnit typeunit = Unit == SweepType.Linear ? QuantityUnit.Variant : QuantityUnit.Decibel;
            for (Int32 i = 0; i < temp.Count; i++)
            {
                outputstring += temp[i].ToString()+",";
                
                outputstring+= new Quantity(GetHarmonicFreq(temp[i]-1), Prefix.Empty, QuantityUnit.Hertz).ToString("E5",false)+",";
                outputstring+= new Quantity(GetMagRatio(temp[i]-1), Prefix.Empty, QuantityUnit.Percent).ToString("E5", false)+",";
                if (Unit == SweepType.Linear)
                {
                    outputstring += Quantity.ConvertByPrefix(GetMagRMS(temp[i] - 1)/1000, Prefix.Empty).ToString("E5") + ",";
                }
                else
                {
                    outputstring += Quantity.ConvertByPrefix(GetMagRMS(temp[i] - 1)/1000, Prefix.Micro).ToString("E5") + ",";
                }
                outputstring+= new Quantity(GetPhase(temp[i]-1), Prefix.Empty, QuantityUnit.Angle).ToString("E5", false) +",";
                outputstring += Environment.NewLine;
            }


            return outputstring;
        }
    }
}

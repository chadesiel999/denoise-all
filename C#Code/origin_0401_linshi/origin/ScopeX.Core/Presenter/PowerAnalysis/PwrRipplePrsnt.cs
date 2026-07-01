using System;
using System.Collections.Generic;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core.PowerAnalysis
{
    public class PwrRipplePrsnt :MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {
        public PwrRipplePrsnt(IDsoPrsnt idp, ChannelId id = ChannelId.POWER1, IPwrOptionView? view = null) : base(idp)
        {
            Model = DsoModel.Default.GetPowerChannel(id)?.Ripple;
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public Int32 Count => Model.Count;
        public String Titles => Model.Ttitles;

        public List<String> RippleHearders => PwrRippleModel.RippleHearders;
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

        private protected override PwrRippleModel Model
        {
            get;
        }

        public void Reset() => Model.Reset();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:将成员标记为 static", Justification = "<挂起>")]
        public void TryShowRippleWfm(MathPrsnt? mp)
        {
            if (mp is not null)
            {
                mp.Args.Occupier = null;
                mp.Label = "";
                mp.Active = false;
                mp.IsAutoUnit = false;
            }
        }

        public record RippleItems(String Name, Double? Value, Double? Mean, Double? Max, Double? Min, QuantityUnit Unit);

        public RippleItems this[String item]
        {
            get
            {
                return new(item,
                    Model[item].Current,
                    Model[item].StaBuffer.Average,
                    Model[item].StaBuffer.Max,
                    Model[item].StaBuffer.Min,
                    Source == VIType.V ? QuantityUnit.Voltage : QuantityUnit.Ampere);
            }
        }

        public String GetRippleTableData()
        {
            var outputstring = String.Empty;
            for (Int32 i = 0; i < RippleHearders.Count; i++)
            {
                var title = String.Empty;
                if (i == 0)
                {
                    title = "Pk-Pk";
                }
                else
                {
                    title = "ACRMS";
                }
                outputstring += title+":";
                var tl = Titles.Split(',');
                if (this[RippleHearders[i]].Value == null)
                {
                    outputstring += tl[0] + "," + Quantity.ConvertByPrefix(Double.MaxValue, Prefix.Empty).ToString("E5") + ",";
                }
                else
                {
                    outputstring += tl[0] + "," + Quantity.ConvertByPrefix(((Double)this[RippleHearders[i]].Value), Prefix.Empty).ToString("E5") + ",";
                }
                if (this[RippleHearders[i]].Mean == null)
                {
                    outputstring += tl[1] + "," + Quantity.ConvertByPrefix(Double.MaxValue, Prefix.Empty).ToString("E5") + ",";
                }
                else
                {
                    outputstring += tl[1] + "," + Quantity.ConvertByPrefix(((Double)this[RippleHearders[i]].Mean), Prefix.Empty).ToString("E5") + ",";
                }
                if (this[RippleHearders[i]].Max == null)
                {
                    outputstring += tl[2] + "," + Quantity.ConvertByPrefix(Double.MaxValue, Prefix.Empty).ToString("E5") + ",";
                }
                else
                {
                    outputstring += tl[2] + "," + Quantity.ConvertByPrefix(((Double)this[RippleHearders[i]].Max), Prefix.Empty).ToString("E5") + ",";
                }
                if (this[RippleHearders[i]].Min == null)
                {
                    outputstring += tl[3] + "," + Quantity.ConvertByPrefix(Double.MaxValue, Prefix.Empty).ToString("E5") + ",";
                }
                else
                {
                    outputstring += tl[3] + "," + Quantity.ConvertByPrefix(((Double)this[RippleHearders[i]].Min), Prefix.Empty).ToString("E5") + ",";
                }
                
                outputstring += Environment.NewLine;
            }
            return outputstring;
        }
    }

}

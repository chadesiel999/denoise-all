using ScopeX.ComModel;
using ScopeX.Core.PowerAnalysis;
using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ScopeX.Core
{
    [Description("Rds(on)")]
    public class PwrRDSonPrsnt : MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {
        private MathPrsnt _MathPrsnt = null;
        public PwrRDSonPrsnt(IDsoPrsnt idp, ChannelId id = ChannelId.POWER1, IPwrOptionView? view = null) : base(idp)
        {
            Model = DsoModel.Default.GetPowerChannel(id)?.RDSon;
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public Action? VuTryAddRDSWave;
        public RDSonItem Value => new(
            nameof(Value),
            Model[nameof(Value)].Current,
            Model[nameof(Value)].StaBuffer.Average,
            Model[nameof(Value)].StaBuffer.Max,
            Model[nameof(Value)].StaBuffer.Min,
            QuantityUnit.Ohm);


        public IEnumerable<RDSonItem> RDSons
        {
            get
            {
                yield return Value;
            }
        }

        public (Prefix Prefix, String Name) Unit
        {
            get => Model.Unit;
        }
        public Boolean RunCompleted
        {
            get => Model.RunCompleted;
        }


        private protected override PwrRDSonModel Model { get; }



        public void TryShowRDSonWfm(MathPrsnt? mp)
        {
            if (mp is not null)
            {
                if (mp.Formula != Model.Formula)
                {
                    mp.GetOrMakeArg(MathType.Custom);
                    mp.Formula = Model.Formula;
                }

                mp.Args.Occupier = Model.Analysis;
                mp.Label = typeof(PwrRDSonPrsnt).GetClassDescription();
                mp.Active = true;
                mp.IsAutoUnit = false;
                mp.CustomUnit = QuantityUnit.Ohm.ToUnitString();
                Model.BoundMathId = mp.Id;
            }
        }

        public String RdsonTableData()
        {
            var outputstring = "Value,Average,Max,Min";
            outputstring += Environment.NewLine;
            foreach (var item in RDSons)
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
        public record RDSonItem(String Name, Double? Value, Double? Mean, Double? Max, Double? Min, QuantityUnit Unit);
    }
}

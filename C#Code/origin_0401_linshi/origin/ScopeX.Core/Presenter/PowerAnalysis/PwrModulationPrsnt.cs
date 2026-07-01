using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using static ScopeX.Core.PowerAnalysis.PwrModulationModel;

namespace ScopeX.Core.PowerAnalysis
{
    public class PwrModulationPrsnt : MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {
        public PwrModulationPrsnt(IDsoPrsnt idp, ChannelId id = ChannelId.POWER1, IPwrOptionView? view = null) : base(idp)
        {
            //Model = mco switch
            //{
            //    ModelCreateOptions.Dependant => DsoModel.Default.PwrAnalysis.Modulation,
            //    ModelCreateOptions.Standalone => new(DsoModel.Default.PwrAnalysis, DsoModel.Default.Meas),
            //    _ => throw new ArgumentException($"Argument '{nameof(mco)}' can not assign to '{nameof(ModelCreateOptions.InitializedByChild)}'."),
            //};
            Model = DsoModel.Default.GetPowerChannel(id)?.Modulation;
            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public ModulationItems Period => new(
          nameof(Period),
          Model[nameof(Period)].Current,
          Model[nameof(Period)].StaBuffer.Average,
          Model[nameof(Period)].StaBuffer.Max,
          Model[nameof(Period)].StaBuffer.Min,
          QuantityUnit.Second);
        public ModulationItems Frequency => new(
           nameof(Frequency),
           Model[nameof(Frequency)].Current,
           Model[nameof(Frequency)].StaBuffer.Average,
           Model[nameof(Frequency)].StaBuffer.Max,
           Model[nameof(Frequency)].StaBuffer.Min,
           QuantityUnit.Hertz);
        public ModulationItems PDuty => new(
           nameof(PDuty),
           Model[nameof(PDuty)].Current,
           Model[nameof(PDuty)].StaBuffer.Average,
           Model[nameof(PDuty)].StaBuffer.Max,
           Model[nameof(PDuty)].StaBuffer.Min,
           QuantityUnit.Percent);
        public ModulationItems NDuty => new(
          nameof(NDuty),
          Model[nameof(NDuty)].Current,
          Model[nameof(NDuty)].StaBuffer.Average,
          Model[nameof(NDuty)].StaBuffer.Max,
          Model[nameof(NDuty)].StaBuffer.Min,
          QuantityUnit.Percent);
        public ModulationItems PWidth => new(
         nameof(PWidth),
         Model[nameof(PWidth)].Current,
         Model[nameof(PWidth)].StaBuffer.Average,
         Model[nameof(PWidth)].StaBuffer.Max,
         Model[nameof(PWidth)].StaBuffer.Min,
         QuantityUnit.Second);
        public ModulationItems NWidth => new(
         nameof(NWidth),
         Model[nameof(NWidth)].Current,
         Model[nameof(NWidth)].StaBuffer.Average,
         Model[nameof(NWidth)].StaBuffer.Max,
         Model[nameof(NWidth)].StaBuffer.Min,
         QuantityUnit.Second);
        public ModulationItems RiseTime => new(
         nameof(RiseTime),
         Model[nameof(RiseTime)].Current,
         Model[nameof(RiseTime)].StaBuffer.Average,
         Model[nameof(RiseTime)].StaBuffer.Max,
         Model[nameof(RiseTime)].StaBuffer.Min,
         QuantityUnit.Second);
        public ModulationItems FallTime => new(
         nameof(FallTime),
         Model[nameof(FallTime)].Current,
         Model[nameof(FallTime)].StaBuffer.Average,
         Model[nameof(FallTime)].StaBuffer.Max,
         Model[nameof(FallTime)].StaBuffer.Min,
         QuantityUnit.Second);

        public IEnumerable<ModulationItems> Modulation
        {
            get
            {
                yield return Period;
                yield return Frequency;
                yield return PDuty;
                yield return NDuty;
                yield return PWidth;
                yield return NWidth;
                yield return RiseTime;
                yield return FallTime;
            }
        }
        public Action? VuAddHistogram;
        public Action? VuAddTrend;
        public VIType SourceType { get => Model.SourceType; set => Model.SourceType = value; }
        public ModulationType HistogramSource { get => Model.HistogramSource; set => Model.HistogramSource = value; }
        public ModulationType TrendSource { get => Model.TrendSource; set => Model.TrendSource = value; }
        public String Titles => Model.Titles;
        public Boolean CompletionFlag { get => Model.CompletionFlag; }
        //public List<String> Items => Model.Items;

        public ChannelId MathId { get => Model.MathId; set => Model.MathId = value; }

        //public Boolean IsPositive { get => Model.IsPositive; }

        public ConcurrentDictionary<ModulationType, ModulationItem> Items { get => Model.ModulationItems; set => Model.ModulationItems = value; }

        public ConcurrentDictionary<ChannelId, ModulationType> ItemType { get => Model.ItemType; set => Model.ItemType = value; }

        public Int64? WindowId
        {
            get => Model.WindowId;
            set => Model.WindowId = value;
        }

        private protected override PwrModulationModel Model { get; }

        public Double? GetLocationByIndex(Double value) => Model?.GetLocationByIndex(value);

        public void TryShowPowerWfm(MathPrsnt? mp, MathType mathtype, ModulationType modulationtype)
        {
            //if (mp is not null)
            //{
            //    if (mathtype == MathType.Histgram)
            //    {
            //        //mp.Formula = $"{MathType.Histgram}:Execute.Hist(P1, 100)";
            //        if (mp.Args is MathHistArg mha)
            //        {
            //            mha.Source = ChannelId.P1;
            //        }
            //    }
            //    else
            //    {
            //        mp.Formula = $"{MathType.Trend}:Execute.Trend(M40, 10000)";
            //        //if (mp.Args is MathTrendArg mtd)
            //        //{
            //        //    mtd.Source = ChannelId.P1;
            //        //}
            //        //mp.CustomUnit = QuantityUnit.Second.ToUnitString();
            //    }

            //    mp.Args.Occupier = Model.Analysis;
            //    mp.Label = modulationtype.ToString();
            //    mp.Active = true;
            //    mp.IsAutoUnit = true;
            //    //mp.CustomUnit = QuantityUnit.Watt.ToUnitString();

            //    //Model.BoundMathId = mp.Id;
            //}
        }

        //public (Boolean Active, Int64? WindowId)? this[String key, ModulationWfmType wfmType]
        //{
        //    get
        //    {
        //        var item = Model[key];
        //        if (wfmType == ModulationWfmType.Trend)
        //        {
        //            return (item.TrendActive, item.TrendWindowId);
        //        }
        //        else
        //        {
        //            return (item.HistgramActive, item.HistgramWindowId);
        //        }
        //    }
        //}
        public String ModulationTableData()
        {
            var outputstring = "Value,Average,Max,Min";
            outputstring += Environment.NewLine;
            foreach (var item in Modulation)
            {
                outputstring += item.Name + ",";
                if (item.Value == null || item.Value.ToString()==Double.NaN.ToString())
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




        public record ModulationItems(String Name, Double? Value, Double? Mean, Double? Max, Double? Min, QuantityUnit Unit/*String Unit*/);
    }
}

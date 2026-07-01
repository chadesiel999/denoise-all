namespace ScopeX.Core.PowerAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;


    public class PwrInrushCurrentPrsnt : MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {      
        public PwrInrushCurrentPrsnt(IDsoPrsnt idp, ChannelId id = ChannelId.POWER1, IPwrOptionView? view = null) : base(idp)
        {
            Model = DsoModel.Default.GetPowerChannel(id)?.InrushCurrent;

            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }
        public Action? VuSingleRun;
        
        public TestMode TestMode
        {
            get => Model.TestMode;
            set => Model.TestMode = value;
        }

        public Boolean RunFlag
        {
            get => Model.RunFlag;
            set => Model.RunFlag = value;
        }
        
        public Double MaxPeakCurrent => Model.MaxCurrent;
        public Double MinPeakCurrent => Model.MinCurrent;
        public Double PeakCurrent
        {
            get => Model.PeakCurrent;
            set => Model.PeakCurrent = Math.Clamp(value, MinPeakCurrent, MaxPeakCurrent);
        }

        public String Unit => Model.Unit;

        public InrushCurrentItems Max => new(
           nameof(Max),
           Model[nameof(Max)].Current,
           Model[nameof(Max)].StaBuffer.Average,
           Model[nameof(Max)].StaBuffer.Max,
           Model[nameof(Max)].StaBuffer.Min,
           Model.Unit);

        public InrushCurrentItems Min => new(
           nameof(Min),
           Model[nameof(Min)].Current,
           Model[nameof(Min)].StaBuffer.Average,
           Model[nameof(Min)].StaBuffer.Max,
           Model[nameof(Min)].StaBuffer.Min,
           Model.Unit);

        public InrushCurrentItems Pk2Pk => new(
            nameof(Pk2Pk),
            Model[nameof(Pk2Pk)].Current,
            Model[nameof(Pk2Pk)].StaBuffer.Average,
            Model[nameof(Pk2Pk)].StaBuffer.Max,
            Model[nameof(Pk2Pk)].StaBuffer.Min,
            Model.Unit);
        public IEnumerable<InrushCurrentItems> InrushCurrents
        {
            get
            {
                yield return Max;
                yield return Min;
                yield return Pk2Pk;
            }
        }

        public String InrushTableData()
        {
            var outputstring = "Value,Average,Max,Min";
            outputstring += Environment.NewLine;
            foreach (var item in InrushCurrents)
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
        private protected override PwrInrushCurrentModel Model { get; }

        public record InrushCurrentItems(String Name, Double? Value, Double? Mean, Double? Max, Double? Min, String Unit);
    }
}

using ScopeX.ComModel;
using ScopeX.Core.Tools;
using System;
using System.Collections.Generic;

namespace ScopeX.Core.PowerAnalysis
{
    public class PwrOnOffTimePrsnt : MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {
        public PwrOnOffTimePrsnt(IDsoPrsnt idp, ChannelId id = ChannelId.POWER1, IPwrOptionView? view = null) : base(idp)
        {
            Model = DsoModel.Default.GetPowerChannel(id)?.OnOffTime;

            Model.PropertyChanged += OnPropertyChanged;

            if (view != null)
            {
                view.Presenter = this;

                TryAddView(view);
            }
        }

        public ChannelId InVoltageSrc
        {
            get => Model.InVoltageSrc;
            set => Model.InVoltageSrc = value;
        }

        public ChannelId OutVoltageSrc
        {
            get => Model.OutVoltageSrc;
            set => Model.OutVoltageSrc = value;
        }

        public TurnOnOffType Type
        {
            get => Model.Type;
            set => Model.Type = value;
        }

        public TurnOnOffTestType TestType
        {
            get => Model.TestType;
            set => Model.TestType = value;
        }

        public Double MaxAcquisitionTime => Model.MaxAcquisitionTime;
        public Double MinAcquisitionTime => Model.MinAcquisitionTime;
        public Double AcquisitionTime
        {
            get => Model.AcquisitionTime;
            set => Model.AcquisitionTime = Math.Clamp(value, MinAcquisitionTime, MaxAcquisitionTime);
        }

        public Double MaxInPeakVoltage => Model.MaxInPeakVoltage;
        public Double MinInPeakVoltage => Model.MinInPeakVoltage;
        public Double InPeakVoltage
        {
            get => Model.InPeakVoltage;
            set => Model.InPeakVoltage = Math.Clamp(value, MinInPeakVoltage, MaxInPeakVoltage);
        }

        public Double MaxOutPeakVoltage => Model.MaxOutPeakVoltage;
        public Double MinOutPeakVoltage => Model.MinOutPeakVoltage;
        public Double OutPeakVoltage
        {
            get => Model.OutPeakVoltage;
            set => Model.OutPeakVoltage = Math.Clamp(value, MinOutPeakVoltage, MaxOutPeakVoltage);
        }


        public Int32 Count => Model.Count;
        public String Titles => Model.Titles;

        public Boolean RunFlag
        {
            get => Model.RunFlag;
            set
            {
                if (Model.RunFlag != value)
                {
                    Model.RunFlag = value;
                }
            }
        }

        public OnOffTimeItems TurnOnTime => new(
            nameof(TurnOnTime),
            Model[nameof(TurnOnTime)].Current,
            Model[nameof(TurnOnTime)].StaBuffer.Average,
            Model[nameof(TurnOnTime)].StaBuffer.Max,
            Model[nameof(TurnOnTime)].StaBuffer.Min,
            QuantityUnit.Second);

        public OnOffTimeItems TurnOffTime => new(
            nameof(TurnOffTime),
            Model[nameof(TurnOffTime)].Current,
            Model[nameof(TurnOffTime)].StaBuffer.Average,
            Model[nameof(TurnOffTime)].StaBuffer.Max,
            Model[nameof(TurnOffTime)].StaBuffer.Min,
            QuantityUnit.Second);

        public IEnumerable<OnOffTimeItems> TurnOnTimes
        {
            get
            {
                yield return TurnOnTime;
            }
        }

        public IEnumerable<OnOffTimeItems> TurnOffTimes
        {
            get
            {
                yield return TurnOffTime;
            }
        }

        public String Unit => Model.Unit;

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

        private protected override PwrOnOffTimeModel Model
        {
            get;
        }

        public void Reset() => Model.Reset();

        public String OnOffTimeTableData()
        {
            var outputstring = "Value,Average,Max,Min";
            outputstring += Environment.NewLine;
            foreach (var item in TurnOffTimes)
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

        public record OnOffTimeItems(String Name, Double? Value, Double? Mean, Double? Max, Double? Min, QuantityUnit Unit);
    }
}

using ScopeX.ComModel;
using ScopeX.Core.Tools;
using System;

namespace ScopeX.Core.PowerAnalysis
{
     public class PwrPSRRPrsnt : MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {
        private MathPrsnt _MathPrsnt = null;
        public PwrPSRRPrsnt(IDsoPrsnt idp, ChannelId id = ChannelId.POWER1, IPwrOptionView? view = null) : base(idp)
        {
            Model = DsoModel.Default.GetPowerChannel(id)?.PSRR;
            Model.PropertyChanged += OnPropertyChanged;
            if (view != null)
            {
                view.Presenter = this;
                TryAddView(view);
            }
        }

        public AWGId AWGSource
        {
            get => Model.AWGSource;
            set => Model.AWGSource = value;
        }

        public ScanMode Scan
        {
            get => Model.Scan;
            set => Model.Scan = value;
        }

        public ImpedanceType Impedance
        {
            get => Model.Impedance;
            set => Model.Impedance = value;
        }

        public Int64 StartFreq
        {
            get => Model.StartFreq;
            set => Model.StartFreq = value;
        }

        public Double StartFreqBymHz
        {
            get => StartFreq * 1000D;
            set => StartFreq = (Int64)(value / 1000D);
        }

        public Int64 EndFreq
        {
            get => Model.EndFreq;
            set => Model.EndFreq = value;
        }

        public Double EndFreqBymHz
        {
            get => EndFreq * 1000D;
            set => EndFreq = (Int64)(value / 1000D);
        }

        public Int64 MaxFreq => Model.MAX_FREQ;
        public Int64 MinFreq => Model.MIN_FREQ;

        public Int32 ScanNum
        {
            get => Model.ScanNum;
            set => Model.ScanNum = value;
        }

        public AmplitudeMode AmplitudeMode
        {
            get => Model.AmplitudeMode;
            set => Model.AmplitudeMode = value;
        }

        public Int32 MaxScanNum => Model.MAX_ScanNum;
        public Int32 MinScanNum => Model.MIN_ScanNum;
        public String Titles => Model.Titles;
        public Double Amplitude
        {
            get => Model.Amplitude;
            set => Model.Amplitude = value;
        }

        public Double MaxAmplitude => Model.MaxAmplitude * (Impedance == ImpedanceType.High1M ? 2 : 1);
        public Double MinAmplitude => Model.MinAmplitude * (Impedance == ImpedanceType.High1M ? 2 : 1);

        public Double[] AmpValue => Model.AmpValue;

        public void SetAmplitudeValue(Int32 index, Double value)
        {
            Model.SetAmplitudeValue(index, value);
        }

        public Double GetAmplitudeValue(Int32 index)
        {
            return Model.GetAmplitudeValue(index);
        }

        public Int64? WindowId
        {
            get => Model.WindowId;
            set => Model.WindowId = value;
        }

        public DataOpt[] Data
        {
            get { return Model.Data; }
        }
        public Boolean CalcCompleted
        {
            get => Model.CalcCompleted;
            set => Model.CalcCompleted = value;
        }
        public Boolean RunFlag
        {
            get => Model.RunFlag;
            set => Model.RunFlag = value;
        }

        public Boolean CheckTriggerStatus
        {
            get => Model.CheckTriggerStatus;
            set => Model.CheckTriggerStatus = value;
        }

        public Double MaxFre
        {
            get => Model.MaxFre;
            set => Model.MaxFre = value;
        }
        public Double Max
        {
            get => Model.Max;
            set => Model.Max = value;
        }
        public Double MinFre
        {
            get => Model.MinFre;
            set => Model.MinFre = value;
        }
        public Double Min
        {
            get => Model.Min;
            set => Model.Min = value;
        }
        public Int32 DataCount
        {
            get => Model.DataCount;
        }
        public Boolean UpdateData
        {
            get => Model.UpdateData;
        }

        private protected override PwrPSRRModel Model { get; }

        public Action? VuTryAddPsrrBode;
        public Action? VuPsrrRun;

        
        public String GetPSRRTableData()
        {
            var outputstring = "Frequency,Amplitude,PSRR";
            outputstring += Environment.NewLine;
            for (Int32 i = 0; i < ScanNum; i++)
            {
                outputstring += (i + 1).ToString() + ",";
                outputstring += Quantity.ConvertByPrefix(Data[i].Freq, Prefix.Empty).ToString("E5") + ",";
                outputstring += Quantity.ConvertByPrefix(Data[i].Amp, Prefix.Milli).ToString("E5") + ",";
                outputstring += Data[i].Gain.ToString("E5") + ",";
                outputstring += Data[i].Phase.ToString("E5") + ",";
                outputstring += Environment.NewLine;
            }
            return outputstring;
        }

    }
}

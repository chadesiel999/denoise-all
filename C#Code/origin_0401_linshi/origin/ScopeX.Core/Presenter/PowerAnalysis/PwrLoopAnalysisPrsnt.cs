namespace ScopeX.Core.PowerAnalysis
{
    using System;
    using System.Threading;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    public class PwrLoopAnalysisPrsnt : MulticastPrsnt<IPwrOptionView>, IPwrOptionPrsnt
    {
        private MathPrsnt _MathPrsnt = null;
        private ArbWfmGenPrsnt awgprsnt1;
        private ArbWfmGenPrsnt awgprsnt2;
        public PwrLoopAnalysisPrsnt(IDsoPrsnt idp, ChannelId id = ChannelId.POWER1, IPwrOptionView? view = null) : base(idp)
        {
            Model = DsoModel.Default.GetPowerChannel(id)?.LoopAnalysis;
            Model.PropertyChanged += OnPropertyChanged;
            awgprsnt1 = new ArbWfmGenPrsnt(ChannelId.AWG1, idp);
            awgprsnt2 = new ArbWfmGenPrsnt(ChannelId.AWG2, idp);
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

        public Double PmFre
        {
            get => Model.PmFre;
            set => Model.PmFre = value;
        }
        public Double PmPha
        {
            get => Model.PmPha;
            set => Model.PmPha = value;
        }
        public Double GmFre
        {
            get => Model.GmFre;
            set => Model.GmFre = value;
        }
        public Double GmAmp
        {
            get => Model.GmAmp;
            set => Model.GmAmp = value;
        }
        public Int32 DataCount
        {
            get => Model.DataCount;
        }

        public Boolean UpdateData
        {
            get => Model.UpdateData;
        }

        public void Run()
        {
            Model.SingleRun();
        }
        public void WaveGenByAWG(Double freq, Double amp)
        {
            awgprsnt1.Active = true;
            awgprsnt1.Mode = WfmGenMode.Continuous;
            awgprsnt1.WfmType = ArbWfmType.Sinusoid;
            awgprsnt1.Frequency = (Int64)(freq * 1E6);
            awgprsnt1.Amplitude = (Int32)amp;
            awgprsnt1.Offset = 0;
            awgprsnt1.Phase = 0;
            awgprsnt2.Active = true;
            awgprsnt2.Mode = WfmGenMode.Continuous;
            awgprsnt2.WfmType = ArbWfmType.Sinusoid;
            awgprsnt2.Frequency = (Int64)(freq * 1E6);
            awgprsnt2.Amplitude = (Int32)amp;
            awgprsnt2.Offset = 0;
            awgprsnt2.Phase = 0;
            if (Impedance == ImpedanceType.High1M)
            {
                awgprsnt1.Impedance = WfmGenImpedance.HighZ;
                awgprsnt2.Impedance = WfmGenImpedance.HighZ;
            }
            else
            {
                awgprsnt1.Impedance = WfmGenImpedance.Low50;
                awgprsnt2.Impedance = WfmGenImpedance.Low50;
            }

            Thread.Sleep(100);
        }
        public void ScanPoint_CalcFreqAmp()
        {
            Model.InitScanPoint();
        }
       
        private protected override PwrLoopAnalysisModel Model { get; }

        public void TryShowLoopAnalysisWfm(MathPrsnt? mp)
        {
            if (mp is not null)
            {
                _MathPrsnt = mp;
                mp.Args.Occupier = null;
                mp.Label = "";
                mp.IsAutoUnit = false;
            }
        }

        public String GetLoopTableData()
        {
            var outputstring = String.Empty;
            for (Int32 i = 0; i < ScanNum; i++)
            {
                outputstring += (i + 1).ToString()+",";
                outputstring += Quantity.ConvertByPrefix(Data[i].Freq, Prefix.Empty).ToString("E5")+",";
                outputstring += Quantity.ConvertByPrefix(Data[i].Amp, Prefix.Milli).ToString("E5") + ",";
                outputstring += Data[i].Gain.ToString("E5")+",";
                outputstring += Data[i].Phase.ToString("E5")+",";
                outputstring += Environment.NewLine;
            }
            return outputstring;
        }

    }
}

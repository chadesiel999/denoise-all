using System;
using System.Drawing;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    public class RadioFrequencyPrsnt : ChannelPrsnt/*, IRadioFrequencyPrsnt*/
    {
        private protected override RadioFrequencyModel Model
        {
            get;
        }

        public AmpVSTimePrsnt AmpVSTime
        { 
            get;
        }

        public PhaseVSTimePrsnt PhaseVSTime
        {
            get;
        }

        public PhaseVSFrequencyPrsnt PhaseVSFrequency
        {
            get;
        }

        public TimeVSFrequencyPrsnt TimeVSFrequency
        {
            get;
        }

        public FrequencyVSTimePrsnt FrequencyVSTime
        {
            get;
        }

        public RadioFrequencyPrsnt(ChannelId id, IDsoPrsnt idp, AmpVSTimePrsnt ampVSTime, PhaseVSTimePrsnt phaseVSTime, PhaseVSFrequencyPrsnt phaseVSFrequency, TimeVSFrequencyPrsnt timeVSFrequency, FrequencyVSTimePrsnt frequencyVSTimePrsnt) : base(idp, null)
        {
            Model = (RadioFrequencyModel)DsoModel.Default.GetChannel(id);
            Model.PropertyChanged += OnPropertyChanged;
            Model.Sampling.PropertyChanged += OnPropertyChanged;
            Model.Conditioning.Prompter = WeakTip.Default;


            AmpVSTime = ampVSTime;
            PhaseVSTime = phaseVSTime;
            PhaseVSFrequency = phaseVSFrequency;
            TimeVSFrequency = timeVSFrequency;
            FrequencyVSTime = frequencyVSTimePrsnt;

            //TranslateADCSamplerate = Model.Sampling._RFTranslateSampleRate;
            FFTLength=Model.Sampling.FFTLength;
            CenterFrequency=Model.Sampling.CenterFrequency;
        }

        public override Boolean Active
        {
            get => Model.Active;
            set {
                Model.Active = value;
                Hardware.HdCmdFactory.Push(HdCmd.ChnlActive);
                Hardware.HdCmdFactory.Push(HdCmd.RadioFrequency);
            }
        }

        public ChannelId Source
        {
            get => Model.Source;
            set => Model.Source = value;
        }

        public Double ScaleV
        {
            get => Model.Conditioning.AmpScale;
            set => Model.Conditioning.AmpScale = value;
        }

        public Double PositionV
        {
            get => Model.Conditioning.PosIndex;
            set => Model.Conditioning.PosIndex = value;
        }

        public Int64 ScaleH
        {
            get => Model.Sampling.FrequencyScale;
            set { 
                Model.Sampling.FrequencyScale = value;
            }
        }

        public Double PositionH
        {
            get => Model.Sampling.PosIndex;
            set => Model.Sampling.PosIndex = value;
        }


        public String UnitH
        {
            get => Model.Sampling.Unit;
            set => Model.Sampling.Unit = value;
        }

        public String UnitV
        {
            get => Model.Conditioning.Unit;
            set => Model.Conditioning.Unit = value;
        }

        #region 参考电平

        public Double RefLevelValue
        {
            get => Model.Conditioning.RefLevelValue;
            set 
            { 
                Model.Conditioning.RefLevelValue = value;
                Hardware.HdCmdFactory.Push(HdCmd.RadioFrequency);
            }
        }

        #endregion

        #region 单位

        public AmplitudeUnitType UnitType
        {
            get => Model.Conditioning.UnitType;
            set => Model.Conditioning.UnitType = value;
        }

        public LogarithmUnit PUnit
        {
            get => Model.Conditioning.PUnit;
            set => Model.Conditioning.PUnit = value;
        }

        #endregion

        public Double AmpScale
        {
            get => Model.Conditioning.AmpScale;
            set { 
                Model.Conditioning.AmpScale = value; 
                AmpVSTime.AmpScale = value;
            }
        }

        public Int64 FrequencyScale
        {
            get => Model.Sampling.FrequencyScale;
            set{
                Model.Sampling.FrequencyScale = value;
                TimeVSFrequency.FrequescyScale = value;
                PhaseVSFrequency.FrequencyScale = value;
            }
        }

        #region 数据（波形）参数

        public Int32 FFTLength
        {
            get => Model.FFTLength;
            set
            {
                Model.FFTLength = value;
                Hardware.HdCmdFactory.Push(HdCmd.RadioFrequency);
            }
        }
        public Int32 STFTLength
        {
            get => Model.STFTLength;
            set
            {
                Model.STFTLength = value;
                Hardware.HdCmdFactory.Push(HdCmd.RadioFrequency);
            }
        }
        public Int32 STFTStep
        {
            get => Model.STFTStep;
            set
            {
                Model.STFTStep = value;
                Hardware.HdCmdFactory.Push(HdCmd.RadioFrequency);
            }
        }
        #region 起始频率

        public Int64 StartFrequency
        {
            get => Model.StartFrequency;
            set
            {
                Model.StartFrequency = value;
                Hardware.HdCmdFactory.Push(HdCmd.RadioFrequency);
            }
        }

        #endregion

        #region 中心频率

        public Int64 CenterFrequency
        {
            get => Model.CenterFrequency;
            set {
                Model.CenterFrequency = value;
                Hardware.HdCmdFactory.Push(HdCmd.RadioFrequency);
            }
        }

        #endregion

        #region 终止频率

        public Int64 EndFrequency
        {
            get => Model.EndFrequency;
            set
            {
                Model.EndFrequency = value;
                Hardware.HdCmdFactory.Push(HdCmd.RadioFrequency);
            }
        }

        #endregion

        #region 跨度

        public Int64 Span
        {
            get => Model.Span;
            set
            {
                Model.Span = value;
                Hardware.HdCmdFactory.Push(HdCmd.RadioFrequency);
            }
        }

        #endregion

        #region RBW

        public Double RBW
        {
            get => Model.RBW;
            set
            {
                Model.RBW = value;
                Hardware.HdCmdFactory.Push(HdCmd.RadioFrequency);
            }
        }

        public Double GetRBWMin()
        {
            return Model.Sampling.GetRBWMin();
        }

        public Double GetRBWMax()
        {
            return Model.Sampling.GetRBWMax();
        }

        #endregion

        #endregion

        #region FigureParameter
   
        public Double FigureStartAmplitude
        {
            get => Model.Conditioning.FigureStartAmplitude;
        }

        public Double FigureEndAmplitude
        {
            get => Model.Conditioning.FigureEndAmplitude;
        }

        public Double FigureCenterAmplitude
        {
            get => Model.Conditioning.FigureCenterAmplitude;
            set { 
                Model.Conditioning.FigureCenterAmplitude = value;
            }
        }
    
        public Int64 FigureStartFrequency
        {
            get => Model.Sampling.FigureStartFrequency;
        }

        public Int64 FigureEndFrequency
        {
            get => Model.Sampling.FigureEndFrequency;
        }

        public Int64 FigureCenterFrequency
        {
            get => Model.Sampling.FigureCenterFrequency;
            set
            { 
                Model.Sampling.FigureCenterFrequency = value;
                TimeVSFrequency.FigureCenterFrequency = value;
                PhaseVSFrequency.FigureCenterFrequency = value;
            }
        }
        #endregion

        #region 谱线

        #region 正常
        public Boolean NormalLine
        {
            get => Model.NormalLine;
            set => Model.NormalLine = value;
        }

        public PickMode NormalLinePickMode
        {
            get => Model.NormalLinePickMode;
            set => Model.NormalLinePickMode = value;
        }

        public Color NormalLineColor => Model.NormalLineColor;
        #endregion

        #region 最大值保持
        public Boolean MaxHoldLine
        {
            get => Model.MaxHoldLine;
            set => Model.MaxHoldLine = value;
        }

        public PickMode MaxHoldLinePickMode
        {
            get => Model.MaxHoldLinePickMode;
            set => Model.MaxHoldLinePickMode = value;
        }

        public Color MaxHoldLineColor => Model.MaxHoldLineColor;
        #endregion

        #region 最小值保持
        public Boolean MinHoldLine
        {
            get => Model.MinHoldLine;
            set => Model.MinHoldLine = value;
        }

        public PickMode MinHoldLinePickMode
        {
            get => Model.MinHoldLinePickMode;
            set => Model.MinHoldLinePickMode = value;
        }

        public Color MinHoldLineColor => Model.MinHoldLineColor;
        #endregion

        #region 平均

        public Int32 AverageTimes
        {
            get => Model.AverageTimes;
            set => Model.AverageTimes = value;
        }

        public Boolean AverageLine
        {
            get => Model.AverageLine;
            set => Model.AverageLine = value;
        }

        public PickMode AverageLinePickMode
        {
            get => Model.AverageLinePickMode;
            set => Model.AverageLinePickMode = value;
        }

        public Color AverageLineColor => Model.AverageLineColor;
        #endregion

        #endregion

        #region 窗

        public RFWindowType Window                   
        {
            get => Model.Window;
            set
            {
                Model.Window = value;
                Hardware.HdCmdFactory.Push(HdCmd.RadioFrequency);
            }
        }

        #endregion

        #region 数据
        public WfmPack? PackNormal => Model.PackNormal;

        public WfmVuDatabase VuDatabaseNormal
        {
            get => Model.VuDatabaseNormal;
        }

        public WfmPack? PackMaxHold => Model.PackMaxHold;

        public WfmVuDatabase VuDatabaseMaxHold
        {
            get => Model.VuDatabaseMaxHold;
        }

        public WfmPack? PackMinHold => Model.PackMinHold;

        public WfmVuDatabase VuDatabaseMinHold
        {
            get => Model.VuDatabaseMinHold;
        }

        public WfmPack? PackAverage => Model.PackAverage;

        public WfmVuDatabase VuDatabaseAverage
        {
            get => Model.VuDatabaseAverage;
        }

        #endregion

        #region ChannelPrsnt

        public override ISampling Sampling => null;
        #endregion

        #region 光谱数据采集
        public Boolean RecordHistoricalData
        { 
            get => TimeVSFrequency.RecordHistoricalData; 
            set => TimeVSFrequency.RecordHistoricalData = value;
        }
        public Boolean ThreeD
        {
            get => Model.ThreeD;
            set => Model.ThreeD = value;
        }
        #endregion

    }
}

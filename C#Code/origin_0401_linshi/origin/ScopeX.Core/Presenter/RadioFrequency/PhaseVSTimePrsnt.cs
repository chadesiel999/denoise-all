using System;
using System.Drawing;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    public class PhaseVSTimePrsnt: ChannelPrsnt
    {
        private protected override PhaseVSTimeModel Model
        {
            get;
        }
        public override ISampling Sampling => throw new NotImplementedException();
        public PhaseVSTimePrsnt(ChannelId id, IDsoPrsnt idp) : base(idp, null)
        {
            Model = (PhaseVSTimeModel)DsoModel.Default.GetChannel(id);
            Model.PropertyChanged += OnPropertyChanged;
            Model.Sampling.PropertyChanged += OnPropertyChanged;
            Model.Conditioning.Prompter = WeakTip.Default;

        }
        public ChannelId Source
        {
            get => Model.Source;
            set => Model.Source = value;
        }
        public Double PhaseScale
        {
            get => Model.Conditioning.PhaseScale;
            set => Model.Conditioning.PhaseScale = value;
        }
        #region PhaseVSTime
        public Prefix PrefixH
        {
            get { return Model.Sampling.Prefix; }
            set { Model.Sampling.Prefix = value; }
        }
        public Prefix PrefixV
        {
            get { return Model.Conditioning.Prefix; }
            set { Model.Conditioning.Prefix = value; }
        }
        public Boolean PhaseVSTimeActive
        {
            get => Model.Active;
            set { 
                Model.Active = value;
                Hardware.HdCmdFactory.Push(HdCmd.RadioFrequency);
            }
        }

        public Color PhaseVSTimeColor
        {
            get => Model.DrawColor;
        }

        public Int64? PhaseVSTimeWindowId
        {
            get => Model.WindowId;
            set => Model.WindowId = value;
        }

        public String PhaseVSTimeLabel
        {
            get => Model.Label;
        }

        public Double PhaseVSTimeScaleV
        {
            get => Model.Conditioning.PhaseScale;
            set => Model.Conditioning.PhaseScale = value;
        }

        public Double PhaseVSTimePositionV
        {
            get => Model.Conditioning.PosIndex;
            set => Model.Conditioning.PosIndex = value;
        }

        public AnaChnlTimebaseIndex PhaseVSTimeScaleH
        {
            get => Model.Sampling.ScaleIndex;
            set => Model.Sampling.ScaleIndex = value;
        }

        public Double PhaseVSTimePositionH
        {
            get => Model.Sampling.PosIndex;
            set => Model.Sampling.PosIndex = value;
        }

        public Double PhaseVSTimeCenterPhase
        {
            get => Model.Conditioning.FigureCenterPhase;
            set => Model.Conditioning.FigureCenterPhase = value;
        }

        //public Double PhaseVSTimeCenterTime
        //{
        //    get => Model.Sampling.FigureCenterTime;
        //    set => Model.Sampling.FigureCenterTime = value;
        //}

        public String PhaseVSTimeUnitH
        {
            get => Model.Sampling.Unit;
            set => Model.Sampling.Unit = value;
        }

        public String PhaseVSTimeUnitV
        {
            get => Model.Conditioning.Unit;
            set => Model.Conditioning.Unit = value;
        }

        public WfmPack? PackPhaseVSTime => Model.Pack;

        public WfmVuDatabase VuDatabasePhaseVSTime
        {
            get => Model.VuDatabase;
        }
        #endregion
    }

}

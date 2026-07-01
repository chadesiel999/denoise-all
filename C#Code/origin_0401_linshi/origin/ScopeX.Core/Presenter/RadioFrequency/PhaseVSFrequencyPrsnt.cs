using System;
using System.Drawing;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    public class PhaseVSFrequencyPrsnt : ChannelPrsnt
    {
        private protected override PhaseVSFrequencyModel Model
        {
            get;
        }
        public override ISampling Sampling => throw new NotImplementedException();
        public PhaseVSFrequencyPrsnt(ChannelId id, IDsoPrsnt idp) : base(idp, null)
        {
            Model = (PhaseVSFrequencyModel)DsoModel.Default.GetChannel(id);
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
        public PhaseUnitType PhaseUnit
        {
            get => Model.Conditioning.UnitType;
            set => Model.Conditioning.UnitType = value;
        }

        public Int64 FrequencyScale
        {
            get => Model.Sampling.FrequencyScale;
            set => Model.Sampling.FrequencyScale = value;
        }

        #region PhaseVSFrequency

        public Boolean PhaseVSFrequencyActive
        {
            get => Model.Active;
            set => Model.Active = value;
        }

        public Color PhaseVSFrequencyColor
        {
            get => Model.DrawColor;
        }

        public Int64? PhaseVSFrequencyWindowId
        {
            get => Model.WindowId;
            set => Model.WindowId = value;
        }

        public String PhaseVSFrequencyLabel
        {
            get => Model.Label;
        }

        public Double PhaseVSFrequencyScaleV
        {
            get => Model.Conditioning.PhaseScale;
            set => Model.Conditioning.PhaseScale = value;
        }

        public Double PhaseVSFrequencyPositionV
        {
            get => Model.Conditioning.PosIndex;
            set => Model.Conditioning.PosIndex = value;
        }

        public Int64 PhaseVSFrequencyScaleH
        {
            get => Model.Sampling.FrequencyScale;
            set => Model.Sampling.FrequencyScale = value;
        }

        public Double PhaseVSFrequencyPositionH
        {
            get => Model.Sampling.PosIndex;
            set => Model.Sampling.PosIndex = value;
        }

        public Double PhaseVSFrequencyCenterPhase
        {
            get => Model.Conditioning.FigureCenterPhase;
            set => Model.Conditioning.FigureCenterPhase = value;
        }

        public Int64 PhaseVSFrequencyCenterFrequency
        {
            get => Model.Sampling.FigureCenterFrequency;
            set => Model.Sampling.FigureCenterFrequency = value;
        }

        public String PhaseVSFrequencyUnitH
        {
            get => Model.Sampling.Unit;
            set => Model.Sampling.Unit = value;
        }

        public String PhaseVSFrequencyUnitV
        {
            get => Model.Conditioning.Unit;
            set => Model.Conditioning.Unit = value;
        }

        public WfmPack? PackPhaseVSFrequency => Model.Pack;

        public WfmVuDatabase VuDatabasePhaseVSFrequency
        {
            get => Model.VuDatabase;
        }
        #endregion

        #region FigureParameter
        /// <summary>
        /// 图像起始频率
        /// </summary>
        public Int64 FigureStartFrequency
        {
            get => Model.Sampling.FigureStartFrequency;
        }

        /// <summary>
        /// 图像终止频率
        /// </summary>
        public Int64 FigureEndFrequency
        {
            get => Model.Sampling.FigureEndFrequency;
        }

        /// <summary>
        /// 图像中心频率
        /// </summary>
        public Int64 FigureCenterFrequency
        {
            get => Model.Sampling.FigureCenterFrequency;
            set => Model.Sampling.FigureCenterFrequency = value;
        }
        #endregion
    }


}
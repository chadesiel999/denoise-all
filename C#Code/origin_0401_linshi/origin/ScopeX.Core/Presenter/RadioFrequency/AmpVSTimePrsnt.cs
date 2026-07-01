using System;
using System.Drawing;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    public class AmpVSTimePrsnt : ChannelPrsnt
    {
        private protected override AmpVSTimeModel Model
        {
            get;
        }
        public override ISampling Sampling => throw new NotImplementedException();
        public AmpVSTimePrsnt(ChannelId id, IDsoPrsnt idp) : base(idp, null)
        {
            Model = (AmpVSTimeModel)DsoModel.Default.GetChannel(id);
            Model.PropertyChanged += OnPropertyChanged;
            Model.Sampling.PropertyChanged += OnPropertyChanged;
            Model.Conditioning.Prompter = WeakTip.Default;

        }
        public ChannelId Source
        {
            get => Model.Source;
            set => Model.Source = value;
        }
        public Double AmpScale
        {
            get => Model.Conditioning.AmpScale;
            set => Model.Conditioning.AmpScale = value;
        }
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

        #region AmpVSTime

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
        public Boolean AmpVSTimeActive
        {
            get => Model.Active;
            set => Model.Active = value;
                
        }

        public Color AmpVSTimeColor
        {
            get => Model.DrawColor;
        }

        public Int64? AmpVSTimeWindowId
        {
            get => Model.WindowId;
            set => Model.WindowId = value;
        }

        public String AmpVSTimeLabel
        {
            get => Model.Label;
        }

        public Double AmpVSTimeScaleV
        {
            get => Model.Conditioning.AmpScale;
            set => Model.Conditioning.AmpScale = value;
        }

        public Double AmpVSTimePositionV
        {
            get => Model.Conditioning.PosIndex;
            set => Model.Conditioning.PosIndex = value;
        }

        public AnaChnlTimebaseIndex AmpVSTimeScaleH
        {
            get => Model.Sampling.ScaleIndex;
            set => Model.Sampling.ScaleIndex = value;
        }

        public Double AmpVSTimePositionH
        {
            get => Model.Sampling.PosIndex;
            set => Model.Sampling.PosIndex = value;
        }

        public Double AmpVSTimeCenterAmp
        {
            get => Model.Conditioning.FigureCenterAmplitude;
            set => Model.Conditioning.FigureCenterAmplitude = value;
        }

        //public Double AmpVSTimeCenterTime
        //{
        //    get => Model.Sampling.FigureCenterTime;
        //    set => Model.Sampling.FigureCenterTime = value;
        //}

        public String AmpVSTimeUnitH
        {
            get => Model.Sampling.Unit;
            set => Model.Sampling.Unit = value;
        }

        public String AmpVSTimeUnitV
        {
            get => Model.Conditioning.Unit;
            set => Model.Conditioning.Unit = value;
        }


        public WfmPack? PackAmpVSTime => Model.Pack;

        public WfmVuDatabase VuDatabaseAmpVSTime
        {
            get => Model.VuDatabase;
        }


        #endregion

        #region FigureParameter
        /// <summary>
        /// 图像起始功率
        /// </summary>
        public Double FigureStartAmplitude
        {
            get => Model.Conditioning.FigureStartAmplitude;
        }

        /// <summary>
        /// 图像终止功率
        /// </summary>
        public Double FigureEndAmplitude
        {
            get => Model.Conditioning.FigureEndAmplitude;
        }

        /// <summary>
        /// 图像中心功率
        /// </summary>
        public Double FigureCenterAmplitude
        {
            get => Model.Conditioning.FigureCenterAmplitude;
            set
            {
                Model.Conditioning.FigureCenterAmplitude = value;
            }
        }
        #endregion

    }
}

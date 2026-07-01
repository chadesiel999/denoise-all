using System;
using System.Drawing;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    public class FrequencyVSTimePrsnt : ChannelPrsnt
    {
        private protected override FrequencyVSTimeModel Model
        {
            get;
        }
        public override ISampling Sampling => throw new NotImplementedException();
        public FrequencyVSTimePrsnt(ChannelId id, IDsoPrsnt idp) : base(idp, null)
        {
            Model = (FrequencyVSTimeModel)DsoModel.Default.GetChannel(id);
            Model.PropertyChanged += OnPropertyChanged;
            Model.Sampling.PropertyChanged += OnPropertyChanged;
            Model.Conditioning.Prompter = WeakTip.Default;

        }
        public ChannelId Source
        {
            get => Model.Source;
            set => Model.Source = value;
        }
        public Int64 FrequencyScale
        {
            get => Model.Conditioning.FrequencyScale;
            set => Model.Conditioning.FrequencyScale = value;
        }


        #region FrequencyVSTime

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
        public Boolean FrequencyVSTimeActive
        {
            get => Model.Active;
            set => Model.Active = value;

        }

        public Color FrequencyVSTimeColor
        {
            get => Model.DrawColor;
        }

        public Int64? FrequencyVSTimeWindowId
        {
            get => Model.WindowId;
            set => Model.WindowId = value;
        }

        public String FrequencyVSTimeLabel
        {
            get => Model.Label;
        }

        public Int64 FrequencyVSTimeScaleV
        {
            get => Model.Conditioning.FrequencyScale;
            set => Model.Conditioning.FrequencyScale = value;
        }

        public Double FrequencyVSTimePositionV
        {
            get => Model.Conditioning.PosIndex;
            set => Model.Conditioning.PosIndex = value;
        }

        public AnaChnlTimebaseIndex FrequencyVSTimeScaleH
        {
            get => Model.Sampling.ScaleIndex;
            set => Model.Sampling.ScaleIndex = value;
        }

        public Double FrequencyVSTimePositionH
        {
            get => Model.Sampling.PosIndex;
            set => Model.Sampling.PosIndex = value;
        }

        public Int64 FrequencyVSTimeCenterFrequency
        {
            get => Model.Conditioning.FigureCenterFrequency;
            set => Model.Conditioning.FigureCenterFrequency = value;
        }

        //public Double FrequencyVSTimeCenterTime
        //{
        //    get => Model.Sampling.FigureCenterTime;
        //    set => Model.Sampling.FigureCenterTime = value;
        //}

        public String FrequencyVSTimeUnitH
        {
            get => Model.Sampling.Unit;
            set => Model.Sampling.Unit = value;
        }

        public String FrequencyVSTimeUnitV
        {
            get => Model.Conditioning.Unit;
            set => Model.Conditioning.Unit = value;
        }


        public WfmPack? PackFrequencyVSTime => Model.Pack;

        public WfmVuDatabase VuDatabaseFrequencyVSTime
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
            get => Model.Conditioning.FigureStartFrequency;
        }

        /// <summary>
        /// 图像终止频率
        /// </summary>
        public Int64 FigureEndFrequency
        {
            get => Model.Conditioning.FigureEndFrequency;
        }

        /// <summary>
        /// 图像中心频率
        /// </summary>
        public Int64 FigureCenterFrequency
        {
            get => Model.Conditioning.FigureCenterFrequency;
            set
            {
                Model.Conditioning.FigureCenterFrequency = value;
            }
        }
        #endregion

    }

}

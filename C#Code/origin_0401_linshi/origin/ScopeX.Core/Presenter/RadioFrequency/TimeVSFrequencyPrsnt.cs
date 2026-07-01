using System;
using System.Drawing;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    public class TimeVSFrequencyPrsnt : ChannelPrsnt
    {
        private protected override TimeVSFrequencyModel Model
        {
            get;
        }
        public override ISampling Sampling => throw new NotImplementedException();
        public TimeVSFrequencyPrsnt(ChannelId id, IDsoPrsnt idp) : base(idp, null)
        {
            Model = (TimeVSFrequencyModel)DsoModel.Default.GetChannel(id);
            Model.PropertyChanged += OnPropertyChanged;
            Model.Sampling.PropertyChanged += OnPropertyChanged;
            Model.Conditioning.Prompter = WeakTip.Default;
        }
        public ChannelId Source
        {
            get => Model.Source;
            set => Model.Source = value;
        }
        public HistoryData HistoryData => Model.TFHistoryData;

        public Boolean RecordHistoricalData
        {
            get => Model.RecordHistoricalData;
            set => Model.RecordHistoricalData = value;
        }

        public Boolean ThreeD
        {
            get => Model.ThreeD;
            set => Model.ThreeD = value;
        }

        public Int64 FrequescyScale
        {
            get => Model.Sampling.FrequencyScale;
            set => Model.Sampling.FrequencyScale = value;
        }

        #region TimeVSFrequency
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
        public Boolean TimeVSFrequencyActive
        {
            get => Model.Active;
            set => Model.Active = value;
        }

        public Color TimeVSFrequencyColor
        {
            get => Model.DrawColor;
        }

        public Int64? TimeVSFrequencyWindowId
        {
            get => Model.WindowId;
            set => Model.WindowId = value;
        }

        public String TimeVSFrequencyLabel
        {
            get => Model.Label;
        }

        public Double TimeVSFrequencyScaleV
        {
            get => Model.Conditioning.TimeScale;
            set => Model.Conditioning.TimeScale = value;
        }

        public Double TimeVSFrequencyPositionV
        {
            get => Model.Conditioning.PosIndex;
            set => Model.Conditioning.PosIndex = value;
        }

        public Int64 TimeVSFrequencyScaleH
        {
            get => Model.Sampling.FrequencyScale;
            set => Model.Sampling.FrequencyScale = value;
        }

        public Double TimeVSFrequencyPositionH
        {
            get => Model.Sampling.PosIndex;
            set => Model.Sampling.PosIndex = value;
        }

        public Double TimeVSFrequencyCenterPhase
        {
            get => Model.Conditioning.FigureCenterTime;
            set => Model.Conditioning.FigureCenterTime = value;
        }

        public Int64 TimeVSFrequencyCenterTime
        {
            get => Model.Sampling.FigureCenterFrequency;
            set => Model.Sampling.FigureCenterFrequency = value;
        }

        public String TimeVSFrequencyUnitH
        {
            get => Model.Sampling.Unit;
            set => Model.Sampling.Unit = value;
        }

        public String TimeVSFrequencyUnitV
        {
            get => Model.Conditioning.Unit;
            set => Model.Conditioning.Unit = value;
        }

        public WfmPack? PackTimeVSFrequency => Model.Pack;

        public WfmVuDatabase VuDatabaseTimeVSFrequency
        {
            get => Model.VuDatabase;
        }
        #endregion

        #region 数据（波形）参数

        public Int32 FFTLength
        {
            get => Model.Sampling.FFTLength;
            set
            {
                Model.Sampling.FFTLength = value;
            }
        }

        #region 起始频率

        public Int64 StartFrequency
        {
            get => Model.Sampling.StartFrequency;
            set
            {
                Model.Sampling.StartFrequency = value;
            }
        }

        #endregion

        #region 中心频率

        public Int64 CenterFrequency
        {
            get => Model.Sampling.CenterFrequency;
            set
            {
                Model.Sampling.CenterFrequency = value;
            }
        }

        #endregion

        #region 终止频率

        public Int64 EndFrequency
        {
            get => Model.Sampling.EndFrequency;
            set
            {
                Model.Sampling.EndFrequency = value;
            }
        }

        #endregion

        #region 跨度

        public Int64 Span
        {
            get => Model.Sampling.Span;
            set
            {
                Model.Sampling.Span = value;
            }
        }

        #endregion

        #region RBW

        public Double RBW
        {
            get => Model.Sampling.RBW;
            set
            {
                Model.Sampling.RBW = value;
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

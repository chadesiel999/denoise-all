using System;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    internal class TimeModelV : VertAxisModel
    {
        public TimeModelV(): base("Conditioning")
        {
            Unit = "s";
            ScalePrefix = Prefix.Micro;
        }

        public new Prefix Prefix
        { 
            get { return base.Prefix; }
            set { base.Prefix = value; }
        }

        private Double _PosIndex;
        public override Double PosIndex
        {
            get { return -(_FigureCenterTime / _TimeScale * Constants.IDX_PER_YDIV); }
            set
            {
                if (_PosIndex != value)
                {
                    _FigureCenterTime = -(value / Constants.IDX_PER_YDIV) * _TimeScale;
                }
            }
        }

        private Double _TimeScale = Constants.RF_TIME_SCALE_US;
        public Double TimeScale
        {
            get
            {
                return _TimeScale;
            }
            set
            {
                if (_TimeScale != value)
                {
                    //_TimeScale = ValidateTimeScale(value);
                    _TimeScale = value;
                    _FigureStartTime = _FigureCenterTime + Constants.VIS_YDIVS_NUM / 2 * _TimeScale;
                    _FigureEndTime = _FigureCenterTime - Constants.VIS_YDIVS_NUM / 2 * _TimeScale;
                    _PosIndex = -(_FigureCenterTime / _TimeScale * Constants.IDX_PER_YDIV);
                    OnPropertyChanged();
                }
            }
        }

        private Double ValidateTimeScale(Double value)
        {
            value = value < Constants.RF_TIME_MIN_SCALE_PS ? Constants.RF_TIME_MIN_SCALE_PS : value;
            value = value > Constants.RF_TIME_MAX_SCALE_PS ? Constants.RF_TIME_MAX_SCALE_PS : value;

            return value;
        }

        #region FigureParameter
        private Double _FigureStartTime = Constants.RF_FIGURE_START_TIME_V;
        /// <summary>
        /// 图像起始时间
        /// </summary>
        public Double FigureStartTime
        {
            get
            {
                return _FigureStartTime;
            }

        }

        private Double _FigureEndTime = Constants.RF_FIGURE_END_TIME_V;
        /// <summary>
        /// 图像终止时间
        /// </summary>
        public Double FigureEndTime
        {
            get
            {
                return _FigureEndTime;
            }
        }

        private Double _FigureCenterTime = Constants.RF_FIGURE_CENTER_TIME_V;
        /// <summary>
        /// 图像中心时间
        /// </summary>
        public Double FigureCenterTime
        {
            get
            {
                return _FigureCenterTime;
            }
            set
            {
                if (_FigureCenterTime != value)
                {
                    _FigureCenterTime = value;
                    _FigureStartTime = _FigureCenterTime + Constants.VIS_YDIVS_NUM / 2 * _TimeScale;
                    _FigureEndTime = _FigureCenterTime - Constants.VIS_YDIVS_NUM / 2 * _TimeScale;
                    _PosIndex = -(_FigureCenterTime / _TimeScale * Constants.IDX_PER_YDIV);
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }

    internal class TimeModelH  : SamplingModel
    {
        public TimeModelH() : base()
        {
            Unit = "s";
            ScalePrefix = Prefix.Micro;
            PosIndex = 0;
        }
        public new Prefix Prefix
        {
            get { return base.Prefix; }
            set { base.Prefix = value; }
        }

        private Double _PosIndex;
        public new Double PosIndex
        {
            get { return -(_FigureStartTime / _TimeScale * Constants.IDX_PER_XDIV); }
            set
            {
                if (_PosIndex != value)
                {
                    _FigureStartTime = -(value / Constants.IDX_PER_XDIV) * _TimeScale;
                }
            }
        }

        private Double _TimeScale = Constants.RF_TIME_SCALE_US;
        public Double TimeScale
        {
            get
            {
                return _TimeScale;
            }
            set
            {
                if (_TimeScale != value)
                {
                    _TimeScale = ValidateTimeScale(value);
                    _FigureCenterTime = _TimeScale * Constants.VIS_XDIVS_NUM / 2;
                    _FigureStartTime = _FigureCenterTime - Constants.VIS_XDIVS_NUM / 2 * _TimeScale;
                    _FigureEndTime = _FigureCenterTime + Constants.VIS_XDIVS_NUM / 2 * _TimeScale;
                    _PosIndex = -(_FigureCenterTime / _TimeScale * Constants.IDX_PER_XDIV);
                    OnPropertyChanged();
                }
            }
        }

        private Double ValidateTimeScale(Double value)
        {
            value = value < Constants.RF_TIME_MIN_SCALE_PS ? Constants.RF_TIME_MIN_SCALE_PS : value;
            value = value > Constants.RF_TIME_MAX_SCALE_PS ? Constants.RF_TIME_MAX_SCALE_PS : value;

            return value;
        }

        #region FigureParameter
        private Double _FigureStartTime = Constants.RF_FIGURE_START_TIME_H;
        /// <summary>
        /// 图像起始时间
        /// </summary>
        public Double FigureStartTime
        {
            get
            {
                return _FigureStartTime;
            }

        }

        private Double _FigureEndTime = Constants.RF_FIGURE_END_TIME_H;
        /// <summary>
        /// 图像终止时间
        /// </summary>
        public Double FigureEndTime
        {
            get
            {
                return _FigureEndTime;
            }
        }

        private Double _FigureCenterTime = Constants.RF_FIGURE_CENTER_TIME_H;
        /// <summary>
        /// 图像中心时间
        /// </summary>
        public Double FigureCenterTime
        {
            get
            {
                return _FigureCenterTime;
            }
            set
            {
                if (_FigureCenterTime != value)
                {
                    _FigureCenterTime = value;
                    _FigureStartTime = _FigureCenterTime + Constants.VIS_XDIVS_NUM / 2 * _TimeScale;
                    _FigureEndTime = _FigureCenterTime - Constants.VIS_XDIVS_NUM / 2 * _TimeScale;
                    _PosIndex = -(_FigureCenterTime / _TimeScale * Constants.IDX_PER_XDIV);
                    OnPropertyChanged();
                }
            }
        }
        #endregion


        private Double _TranslateSamplerate = Constants.RF_SAMPLING_FREQUENCY;

        public Double TranslateSamplerate
        {
            get
            {
                return _TranslateSamplerate;
            }
            set
            {
                if (_TranslateSamplerate != value)
                {
                    _TranslateSamplerate = value;
                    OnPropertyChanged();
                }
            }
        }

    }
}

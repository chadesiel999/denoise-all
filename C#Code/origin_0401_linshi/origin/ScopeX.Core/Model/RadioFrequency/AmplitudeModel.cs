using System;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    internal sealed class AmplitudeModel : VertAxisModel
    {
        public AmplitudeModel() : base("Conditioning")
        {
            Prefix = Prefix.Empty;
            Unit = GetAmplitudeUnit();
            _PosIndex = -(_FigureCenterAmplitude / _AmpScale * Constants.IDX_PER_YDIV);
        }

        public Double TransformDatawithUnit(Double ampdata)//将数据根据设置的单位进行转换
        {
            var diff = GetDiffBaseUV(_PUnit);
            ampdata += diff;
            return ampdata;
        }

        private static Double GetDiffBaseUV(LogarithmUnit unit)
        {
            switch (unit)
            {
                case LogarithmUnit.dBm:
                    return -106.99;
                case LogarithmUnit.dBμW:
                    return -76.99;
                case LogarithmUnit.dBmV:
                    return -60;
                case LogarithmUnit.dBμV:
                    return 0;
                case LogarithmUnit.dBmA:
                    return -93.98;
                case LogarithmUnit.dBμA:
                    return -33.98;
                default:
                    break;
            }
            return 0;
        }

        public String GetAmplitudeUnit()
        {
            return _UnitType == AmplitudeUnitType.Linear ? Unit.ToString() : _PUnit.ToString();
        }

        private Double _PosIndex;
        public override Double PosIndex
        {
            get { return -(_FigureCenterAmplitude / _AmpScale * Constants.IDX_PER_YDIV); }
            set
            {
                if (_PosIndex != value)
                {
                    _FigureCenterAmplitude = -(value / Constants.IDX_PER_YDIV) * _AmpScale;
                }
            }
        }

        private Double _AmpScale = Constants.RF_AMP_SCALE;
     
        public Double AmpScale
        {
            get
            {
                return _AmpScale;
            }
            set
            {
                if (_AmpScale != value)
                {
                    _AmpScale = ValidateAmpScale(value);
                    _FigureStartAmplitude = _FigureCenterAmplitude + Constants.VIS_YDIVS_NUM / 2 * _AmpScale;
                    _FigureEndAmplitude = _FigureCenterAmplitude - Constants.VIS_YDIVS_NUM / 2 * _AmpScale;
                    _PosIndex = -(_FigureCenterAmplitude / _AmpScale * Constants.IDX_PER_YDIV);
                    OnPropertyChanged();
                }
            }
        }

        private Double ValidateAmpScale(Double value)
        {
            value = value < Constants.RF_AMP_MIN_SCALE ? Constants.RF_AMP_MIN_SCALE : value;
            value = value > Constants.RF_AMP_MAX_SCALE ? Constants.RF_AMP_MAX_SCALE : value;

            return value;
        }

        #region 参考电平

        private Double _RefLevelValue = 0;
        public Double RefLevelValue
        {
            get
            {
                return _RefLevelValue;
            }
            set
            {
                if (_RefLevelValue != value)
                {
                    _RefLevelValue = ValidRefLevelValue(value);
                    OnPropertyChanged();
                }
            }
        }

        private Double ValidRefLevelValue(Double value)
        {
            var diff = value - _RefLevelValue;
            if (diff > 0)
                return _RefLevelValue += Math.Ceiling(Math.Abs(diff / Constants.RF_REF_LEVEL_STEP)) * Constants.RF_REF_LEVEL_STEP;
            else
                return _RefLevelValue -= Math.Ceiling(Math.Abs(diff / Constants.RF_REF_LEVEL_STEP)) * Constants.RF_REF_LEVEL_STEP;
            
        }
        #endregion

        #region 线性、对数单位

        private AmplitudeUnitType _UnitType = AmplitudeUnitType.Logarithm;
        public AmplitudeUnitType UnitType
        {
            get
            {
                return _UnitType;
            }
            set
            {
                if (_UnitType != value)
                {
                    _UnitType = value;
                    Unit = GetAmplitudeUnit();
                    OnPropertyChanged();
                }
            }
        }

        private LogarithmUnit _PUnit = LogarithmUnit.dBm;
        public LogarithmUnit PUnit
        {
            get
            {
                return _PUnit;
            }
            set
            {
                if (_PUnit != value)
                {
                    _PUnit = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region FigureParameter
        private Double _FigureStartAmplitude = Constants.RF_FIGURE_START_AMPLITUDE;
        /// <summary>
        /// 图像起始功率
        /// </summary>
        public Double FigureStartAmplitude
        {
            get
            {
                return _FigureStartAmplitude;
            }

        }

        private Double _FigureEndAmplitude = Constants.RF_FIGURE_END_AMPLITUDE;
        /// <summary>
        /// 图像终止功率
        /// </summary>
        public Double FigureEndAmplitude
        {
            get
            {
                return _FigureEndAmplitude;
            }
        }

        private Double _FigureCenterAmplitude = Constants.RF_FIGURE_CENTER_AMPLITUDE;
        /// <summary>
        /// 图像中心功率
        /// </summary>
        public Double FigureCenterAmplitude
        {
            get
            {
                return _FigureCenterAmplitude;
            }
            set
            {
                if (_FigureCenterAmplitude != value)
                {
                    _FigureCenterAmplitude = value;
                    _FigureStartAmplitude = _FigureCenterAmplitude + Constants.VIS_YDIVS_NUM / 2 * _AmpScale;
                    _FigureEndAmplitude = _FigureCenterAmplitude - Constants.VIS_YDIVS_NUM / 2 * _AmpScale;
                    _PosIndex = -(_FigureCenterAmplitude / _AmpScale * Constants.IDX_PER_YDIV);
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

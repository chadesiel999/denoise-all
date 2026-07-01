using System;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    internal sealed class PhaseModel : VertAxisModel
    {
        public PhaseModel() : base("Conditioning")
        {
            Prefix = Prefix.Empty;
            Unit = GetPhaseUnit();
            _PosIndex = -(_FigureCenterPhase / _PhaseScale * Constants.IDX_PER_YDIV);
        }

        public String GetPhaseUnit()
        {
            switch (_UnitType)
            {
                case PhaseUnitType.Degree:
                    Prefix = Prefix.Empty;
                    PhaseScale = Constants.RF_PHASE_SCALE;
                    return "°" ;
                case PhaseUnitType.Radian:
                    Prefix = Prefix.Empty;
                    PhaseScale = Math.PI/3;
                    return "rad";
                case PhaseUnitType.GroupDelay:
                    Prefix = Prefix.Pico;
                    TimeScale = _TimeScale;
                    return "s";
            }
            return "°";
        }
        private Double _PosIndex;
        public override Double PosIndex
        {
            get { return -(_FigureCenterPhase / _PhaseScale * Constants.IDX_PER_YDIV); }
            set
            {
                if (_PosIndex != value)
                {
                    _FigureCenterPhase = -(value / Constants.IDX_PER_YDIV) * _PhaseScale;
                }
            }
        }

        private PhaseUnitType _UnitType = PhaseUnitType.Degree;
        public PhaseUnitType UnitType
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
                    Unit = GetPhaseUnit();
                    OnPropertyChanged();
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
                //if (_TimeScale != value)
                {
                    _TimeScale = value;
                    _PhaseScale = value;
                    _FigureStartPhase = _FigureCenterPhase - Constants.VIS_YDIVS_NUM / 2 * _PhaseScale;
                    _FigureEndPhase = _FigureCenterPhase + Constants.VIS_YDIVS_NUM / 2 * _PhaseScale;
                    _PosIndex = -(_FigureCenterPhase / _PhaseScale * Constants.IDX_PER_YDIV);
                }
            }
        }

        private Double _PhaseScale = Constants.RF_PHASE_SCALE;
        public Double PhaseScale
        {
            get
            {
                return _PhaseScale;
            }
            set
            {
                if (_PhaseScale != value)
                {
                    _PhaseScale = ValidatePhaseScale(value);
                    _FigureStartPhase = _FigureCenterPhase + Constants.VIS_YDIVS_NUM / 2 * _PhaseScale;
                    _FigureEndPhase = _FigureCenterPhase - Constants.VIS_YDIVS_NUM / 2 * _PhaseScale;
                    _PosIndex = -(_FigureCenterPhase / _PhaseScale * Constants.IDX_PER_YDIV);
                    OnPropertyChanged();
                }
            }
        }

        private Double ValidatePhaseScale(Double value)
        {
            value = value < Constants.RF_PHASE_SCALE_MIN ? Constants.RF_PHASE_SCALE_MIN : value;
            value = value > Constants.RF_PHASE_SCALE_MAX ? Constants.RF_PHASE_SCALE_MAX : value;

            return value;
        }

        #region FigureParameter
        private Double _FigureStartPhase = Constants.RF_FIGURE_START_PHASE;
        /// <summary>
        /// 图像起始相位 
        /// </summary>
        public Double FigureStartPhase
        {
            get
            {
                return _FigureStartPhase;
            }

        }

        private Double _FigureEndPhase = Constants.RF_FIGURE_END_PHASE;
        /// <summary>
        /// 图像终止相位
        /// </summary>
        public Double FigureEndPhase
        {
            get
            {
                return _FigureEndPhase;
            }
        }

        private Double _FigureCenterPhase = Constants.RF_FIGURE_CENTER_PHASE;
        /// <summary>
        /// 图像中心相位
        /// </summary>
        public Double FigureCenterPhase
        {
            get
            {
                return _FigureCenterPhase;
            }
            set
            {
                if (_FigureCenterPhase != value)
                {
                    _FigureCenterPhase = value;
                    _FigureStartPhase = _FigureCenterPhase + 5 * _PhaseScale;
                    _FigureEndPhase = _FigureCenterPhase - 5 * _PhaseScale;
                    _PosIndex = -(_FigureCenterPhase / _PhaseScale * Constants.IDX_PER_YDIV);
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}

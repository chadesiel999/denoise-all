// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using ScopeX.ComModel;

    internal class TriggerEdgeModel : TriggerSingleSrcModel
    {
        public override String Name => TriggerType.Edge.ToString();

        private EdgeSlope _Slope = EdgeSlope.Rise;
        public EdgeSlope Slope
        {
            get => _Slope;
            set
            {
                if (_Slope != value)
                {
                    _Slope = value;
                    OnPropertyChanged();
                }

                PlatformManager.Default.Platform.SetEdgeTriggerLed(_Slope);
            }
        }

        private TriggerCoupling _Coupling = TriggerCoupling.DC;
        public TriggerCoupling Coupling
        {
            get => _Coupling;
            set
            {
                if (_Coupling != value)
                {
                    _Coupling = value;
                    OnPropertyChanged();
                }
            }
        }

        private TriggerImpedance _Impedance = TriggerImpedance.High1M;
        public TriggerImpedance Impedance
        {
            get => _Impedance;
            set
            {
                if (_Impedance != value)
                {
                    _Impedance = value;
                    OnPropertyChanged();
                }
            }
        }

        private Int32 _SensitivityBymdiv = Constants.TRIGGER_DEFAULT_SENSITIVITY_MDIV;

        public Int32 SensitivityBymdivMax
        {
            get => Constants.MAX_TRIGGER_SENSITIVITY_MDIV;
        }
        public Int32 SensitivityBymdivMin
        {
            get => Constants.MIN_TRIGGER_SENSITIVITY_MDIV;
        }
        public Int32 SensitivityBymdiv
        {
            get => _SensitivityBymdiv;
            set
            {
                if (value < Constants.MIN_TRIGGER_SENSITIVITY_MDIV)
                {
                    value = Constants.MIN_TRIGGER_SENSITIVITY_MDIV;
                }
                else if (value > Constants.MAX_TRIGGER_SENSITIVITY_MDIV)
                {
                    value = Constants.MAX_TRIGGER_SENSITIVITY_MDIV;
                }

                if (_SensitivityBymdiv != value)
                {
                    _SensitivityBymdiv = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}

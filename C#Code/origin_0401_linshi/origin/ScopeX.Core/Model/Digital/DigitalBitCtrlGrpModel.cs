using System;
using ScopeX.ComModel;
using ScopeX.Core.Tools;

namespace ScopeX.Core
{
    //public enum DigiCtrlGroup
    //{
    //    D01_D04,
    //    D05_D08,
    //    D09_D12,
    //    D13_D16,
    //    D17_D20,
    //    D21_D24,
    //    D25_D28,
    //    D29_D32,
    //}

    //public enum DigiDispGroup
    //{
    //    D01_D08,
    //    D09_D16,
    //    D17_D24,
    //    D25_D32,
    //}

    internal class DigitalBitCtrlGrpModel
    {
        private DigiTholdFamily _Family = DigiTholdFamily.TTL;
        public DigiTholdFamily Family
        {
            get => _Family;
            set
            {
                if (_Family != value)
                {
                    _Family = value;
                    switch (value)
                    {
                        case DigiTholdFamily.TTL:
                            _LimitedThrold.Current = _LimitedThrold.GetIndex(Constants.DIGI_TTL_THROLD, 0);
                            _LimitedHyst.Current = _LimitedHyst.GetIndex(Constants.DIGI_TTL_HYSTE, 0);
                            break;
                        case DigiTholdFamily.CMOS5000:
                            _LimitedThrold.Current = _LimitedThrold.GetIndex(Constants.DIGI_CMOS50_THROLD, 0);
                            _LimitedHyst.Current = _LimitedHyst.GetIndex(Constants.DIGI_CMOS50_HYSTE, 0);
                            break;
                        case DigiTholdFamily.CMOS3300:
                            _LimitedThrold.Current = _LimitedThrold.GetIndex(Constants.DIGI_CMOS33_THROLD, 0);
                            _LimitedHyst.Current = _LimitedHyst.GetIndex(Constants.DIGI_CMOS33_HYSTE, 0);
                            break;
                        case DigiTholdFamily.CMOS2500:
                            _LimitedThrold.Current = _LimitedThrold.GetIndex(Constants.DIGI_CMOS25_THROLD, 0);
                            _LimitedHyst.Current = _LimitedHyst.GetIndex(Constants.DIGI_CMOS25_HYSTE, 0);
                            break;
                        case DigiTholdFamily.CMOS1800:
                            _LimitedThrold.Current = _LimitedThrold.GetIndex(Constants.DIGI_CMOS18_THROLD, 0);
                            _LimitedHyst.Current = _LimitedHyst.GetIndex(Constants.DIGI_CMOS18_HYSTE, 0);
                            break;
                        case DigiTholdFamily.ECL:
                            _LimitedThrold.Current = _LimitedThrold.GetIndex(Constants.DIGI_ECL_THROLD, 0);
                            _LimitedHyst.Current = _LimitedHyst.GetIndex(Constants.DIGI_ECL_HYSTE, 0);
                            break;
                        case DigiTholdFamily.PECL:
                            _LimitedThrold.Current = _LimitedThrold.GetIndex(Constants.DIGI_PECL_THROLD, 0);
                            _LimitedHyst.Current = _LimitedHyst.GetIndex(Constants.DIGI_PECL_HYSTE, 0);
                            break;
                        case DigiTholdFamily.LVDS:
                            _LimitedThrold.Current = _LimitedThrold.GetIndex(Constants.DIGI_LVDS_THROLD, 0);
                            _LimitedHyst.Current = _LimitedHyst.GetIndex(Constants.DIGI_LVDS_HYSTE, 0);
                            break;
                    };

                    OnPropertyChanged?.Invoke(nameof(Family));
                }
            }
        }

        //private Int64 _UserThroldBymV = 1400;
        //public Int64 UserThroldBymV
        //{
        //    get => Family switch
        //    {
        //        DigiTholdFamily.TTL => Constants.DIGI_TTL_THROLD,
        //        DigiTholdFamily.CMOS5000 => Constants.DIGI_CMOS50_THROLD,
        //        DigiTholdFamily.CMOS3300 => Constants.DIGI_CMOS33_THROLD,
        //        DigiTholdFamily.CMOS2500 => Constants.DIGI_CMOS25_THROLD,
        //        DigiTholdFamily.ECL => Constants.DIGI_ECL_THROLD,
        //        DigiTholdFamily.PECL => Constants.DIGI_PECL_THROLD,
        //        DigiTholdFamily.LVDS => Constants.DIGI_LVDS_THROLD,
        //        DigiTholdFamily.USER or _ => _UserThroldBymV,
        //    };
        //    set
        //    {
        //        value = ValidateThrold(value);
        //        if (value < Constants.DIGI_THROLD_MIN)
        //            value = Constants.DIGI_THROLD_MIN;
        //        else if (value > Constants.DIGI_THROLD_MAX)
        //            value = Constants.DIGI_THROLD_MAX;

        //        if (_UserThroldBymV != value)
        //        {
        //            _UserThroldBymV = value;
        //            OnPropertyChanged?.Invoke(nameof(UserThroldBymV));
        //        }
        //        Family = DigiTholdFamily.USER;
        //    }
        //}

        //private static Int64 ValidateThrold(Int64 value)
        //{
        //    return Convert.ToInt64((Double)value / StpUserThrold) * StpUserThrold;
        //}

        //public void AdjUserThrold(Int64 step) => UserThroldBymV += step * StpUserThrold;

        public static readonly Double MaxUserThrold = Constants.DIGI_THROLD_MAX;

        public static readonly Double MinUserThrold = Constants.DIGI_THROLD_MIN;

        public static readonly Double StpUserThrold = Constants.DIGI_THROLD_STP;

        private readonly LimitedPosition<Int32> _LimitedThrold;

        private Double ThroldIndexToValue(Int32 throldIndex, Double _)
        {
            return throldIndex * StpUserThrold;
        }

        private Int32 ThroldIndexFromValue(Double throldValue, Double _)
        {
            if (throldValue > MaxUserThrold)
            {
                throldValue = MaxUserThrold;
            }
            else if (throldValue < MinUserThrold)
            {
                throldValue = MinUserThrold;
            }

            return Convert.ToInt32(throldValue / StpUserThrold);
        }

        public Int32 UserThroldIndex
        {
            get => _LimitedThrold.Current;
            set
            {
                _LimitedThrold.Current = value;
                Family = DigiTholdFamily.USER;
                OnPropertyChanged?.Invoke(nameof(UserThroldBymV));
            }
        }

        public Double UserThroldBymV
        {
            get => _LimitedThrold.GetValue(UserThroldIndex, 0);
            set => UserThroldIndex = _LimitedThrold.GetIndex(value, 0);
        }

        #region Hysteresis
        public static readonly Double MaxUserHyst = Constants.DIGI_HYSTE_MAX;

        public static readonly Double MinUserHyst = Constants.DIGI_HYSTE_MIN;

        public static readonly Double StpUserHyst = Constants.DIGI_HYSTE_STP;

        private readonly LimitedPosition<Int32> _LimitedHyst;

        private Double HystIndexToValue(Int32 hystIndex, Double _)
        {
            return hystIndex * StpUserHyst;
        }

        private Int32 HystIndexFromValue(Double HystValue, Double _)
        {
            if (HystValue > MaxUserHyst)
            {
                HystValue = MaxUserHyst;
            }
            else if (HystValue < MinUserHyst)
            {
                HystValue = MinUserHyst;
            }

            return Convert.ToInt32(HystValue / StpUserHyst);
        }

        public Int32 UserHystIndex
        {
            get => _LimitedHyst.Current;
            set
            {
                _LimitedHyst.Current = value;
                Family = DigiTholdFamily.USER;
                OnPropertyChanged?.Invoke(nameof(UserHystBymV));
            }
        }

        public Double UserHystBymV
        {
            get => _LimitedHyst.GetValue(UserHystIndex, 0);
            set => UserHystIndex = _LimitedHyst.GetIndex(value, 0);
        }
        #endregion Hysteresis

        public DigitalBitCtrlGrpModel(LimitedPosition<Int32> hyst, Action<String>? onPropertyChanged = null)
        {
            OnPropertyChanged = onPropertyChanged;

            _LimitedThrold = new("UserThrold");
            _LimitedThrold.GetValue = ThroldIndexToValue;
            _LimitedThrold.GetIndex = ThroldIndexFromValue;
            _LimitedThrold.Max = ThroldIndexFromValue(MaxUserThrold, 0);
            _LimitedThrold.Min = ThroldIndexFromValue(MinUserThrold, 0);
            _LimitedThrold.Current = ThroldIndexFromValue(Constants.DIGI_TTL_THROLD, 0);
            _LimitedThrold.Prefix = Prefix.Milli;
            _LimitedThrold.Unit = "V";

            //_LimitedHyst = new("UserHyst");
            _LimitedHyst = hyst;
            _LimitedHyst.GetValue = HystIndexToValue;
            _LimitedHyst.GetIndex = HystIndexFromValue;
            _LimitedHyst.Max = HystIndexFromValue(MaxUserHyst, 0);
            _LimitedHyst.Min = HystIndexFromValue(MinUserHyst, 0);
            _LimitedHyst.Current = HystIndexFromValue(Constants.DIGI_TTL_HYSTE, 0);
            _LimitedHyst.Prefix = Prefix.Milli;
            _LimitedHyst.Unit = "V";
        }

        protected Action<String>? OnPropertyChanged
        {
            get;
        }
    }
}

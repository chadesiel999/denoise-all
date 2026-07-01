// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/16</date>

namespace ScopeX.Core
{
    using System;
    using System.ComponentModel;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    //Clk和Data两个源，Clk做时间参考，Data是触发源

    internal class TriggerSetupHoldModel : TriggerModel
    {
        internal class TriggerDataModel : TriggerMultiLevelModel
        {
            public override String Name => "Data";

            public Int64 _SetupByps;
            public Int64 SetupByps
            {
                get => _SetupByps;
                set
                {
                    //var (min, max) = GetWidthRange(0);
                    value = ValidateWidth(value, MinWidth, MaxWidth);

                    if (value != _SetupByps)
                    {
                        _SetupByps = value;
                        OnPropertyChanged();
                    }
                }
            }

            public TriggerDataModel()
            {
                _SetupByps = MinWidth;
            }
        }

        private TriggerEdgeModel Clock
        {
            get;
        } = new() { Source = ChannelId.C1 };

        //private TriggerWindowModel Data
        //{
        //    get;
        //} = new() { LevelCompCondition = WindowRange.Inside };

        private TriggerDataModel Data
        {
            get;
        } = new() { Source = ChannelId.C2 };

        public override String Name => TriggerType.SetupHold.ToString();

        private SetupHoldViolation _Violation = SetupHoldViolation.Setup;
        public SetupHoldViolation Violation
        {
            get => _Violation;
            set
            {
                if (value != _Violation)
                {
                    _Violation = value;
                    OnPropertyChanged();
                }

            }
        }

        public ChannelId ClkSource
        {
            get => Clock.Source;
            set
            {
                Clock.Source = value;
                OnPropertyChanged();
            }
        }

        public EdgeSlope ClkPolarity
        {
            get => Clock.Slope;
            set
            {
                Clock.Slope = value;
                OnPropertyChanged();
            }
        }

        public Double ClkCompPosIndex
        {
            get => Clock.CompPosIndex;
            set => Clock.CompPosIndex = value;
        }

        public Double ClkCompPosition
        {
            get => Clock.CompPosition;
            set => Clock.CompPosition = value;
        }

        public Double MaxClkCompPositionIndex => Clock.MaxPosIndex;

        public Double MinClkCompPositionIndex => Clock.MinPosIndex;

        public Double MaxClkCompPosition => Clock.MaxCompPosition;

        public Double MinClkCompPosition => Clock.MinCompPosition;


        public Prefix ClkPrefix => Clock.PosPrefix;

        public String ClkUnit => Clock.PosUnit;

        public Double ClkRelPosIndex
        {
            get => Clock.RelPosIndex;
            set => Clock.RelPosIndex = ValidatePosIndex(value);
        }

        public ChannelId DataSource
        {
            get => Data.Source;
            set
            {
                Data.Source = value;
                OnPropertyChanged();
            }
        }

        private EdgeSlope _DataPosPolarity = EdgeSlope.Rise;
        public EdgeSlope DataPosPolarity
        {
            get => _DataPosPolarity;
            set
            {
                if (_DataPosPolarity != value)
                {
                    _DataPosPolarity = value;
                    OnPropertyChanged();
                }
            }
        }

        public Double LowerDataPosIndex
        {
            get => Data.PosLowerIndex;
            set => Data.PosLowerIndex = value;
        }

        public Double UpperDataPosIndex
        {
            get => Data.NoRangePosIndex;
            set => Data.NoRangePosIndex = value;
        }

        public Double LowerDataPosition
        {
            get => Data.LowerCompPosition;
            set => Data.LowerCompPosition = value;
        }

        public Double UpperDataPosition
        {
            get => Data.NoRangeCompPosition;
            set => Data.NoRangeCompPosition = value;
        }

        public Double MaxPosIndex => Constants.MAX_TRIGGER_IDX;
        public Double MinPosIndex => Constants.MIN_TRIGGER_IDX;

        public Double MaxDataCompPositionIndex => Data.MaxPosIndex;

        public Double MinDataCompPositionIndex => Data.MinPosIndex;

        public Double MaxDataCompPosition => Data.MaxCompPosition;

        public Double MinDataCompPosition => Data.MinCompPosition;

        public Prefix DataPrefix => Data.PosPrefix;

        public String DataUnit => Data.PosUnit;

        //public (Double Lower, Double Upper) DataRelPosIndex => Data.RelPosIndex;

        public Double DataRelPosUpperIndex
        {
            get => Data.RelPosUpperIndex;
            set => Data.RelPosUpperIndex = value;
        }

        public Double DataRelPosLowerIndex
        {
            get => Data.RelPosLowerIndex;
            set => Data.RelPosLowerIndex = value;
        }

        public Int64 TsuByps
        {
            get => Data.SetupByps;
            set => Data.SetupByps = value;
        }

        public Int64 MaxTsu => Data.MaxWidth;

        public Int64 MinTsu => Data.MinWidth;

        public Int64 StpTsu => Data.StpWidth;

        public Int64 ThdByps
        {
            get => Data.WidthByps;
            set => Data.WidthByps = value;
        }

        public Int64 MaxThd => Data.MaxWidth;

        public Int64 MinThd => Data.MinWidth;

        public Int64 StpThd => Data.StpWidth;

        public override void LeapPosIndex()
        {
            Clock.LeapPosIndex();
            Data.LeapPosIndex();
        }

        public override event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                base.PropertyChanged += value;
                Clock.PropertyChanged += value;
                Data.PropertyChanged += value;
            }
            remove
            {
                base.PropertyChanged -= value;
                Clock.PropertyChanged -= value;
                Data.PropertyChanged -= value;
            }
        }
    }
}

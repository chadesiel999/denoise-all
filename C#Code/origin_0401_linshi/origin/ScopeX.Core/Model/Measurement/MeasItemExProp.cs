// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/15</date>

namespace ScopeX.Core
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using ScopeX.ComModel;
    using ScopeX.Core.Tools;

    public abstract class MeasItemExProp
    {
        protected readonly Object Container;

        internal MeasItemExProp(Object? obj, PropertyChangedEventHandler? propertyChanged)
        {
            Container = obj ?? this;
            _PropertyChanged = propertyChanged;
        }

        private readonly PropertyChangedEventHandler? _PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(Container, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MeasThresholdExProp : MeasItemExProp
    {
        internal MeasThresholdExProp(Object? obj, PropertyChangedEventHandler? propertyChanged = null) : base(obj, propertyChanged)
        { }

        #region 绝对阈值
        public Double MaxAbsoluteThrold { get; protected set; } = 40000;//40V
        public Double MinAbsoluteThrold { get; protected set; } = -40000;//40V

        public Double _HighAbsoluteThrold = 1000;//1V
        public Double HighAbsoluteThrold
        {
            get => _HighAbsoluteThrold;
            set
            {
                if (value != _HighAbsoluteThrold)
                {
                    _HighAbsoluteThrold = value;
                    OnPropertyChanged("RefLevel");
                }
            }
        }

        public Double _MidAbsoluteThrold = 0;//1V
        public Double MidAbsoluteThrold
        {
            get => _MidAbsoluteThrold;
            set
            {
                if (value != _MidAbsoluteThrold)
                {
                    _MidAbsoluteThrold = value;
                    OnPropertyChanged("RefLevel");
                }
            }
        }

        public Double _LowAbsoluteThrold = -1000;//1V

        public Double LowAbsoluteThrold
        {
            get => _LowAbsoluteThrold;
            set
            {
                if (value != _LowAbsoluteThrold)
                {
                    _LowAbsoluteThrold = value;
                    OnPropertyChanged("RefLevel");
                }
            }
        }
        #endregion

        #region 相对阈值
        public Int32 MaxThrold { get; protected set; } = 95;

        public Int32 MinThrold { get; protected set; } = 5;

        public Int32 GapThrold { get; protected set; } = 10;

        public Int32 StpThrold { get; protected set; } = 1;

        private Int32 _HighThrold = 90;
        public Int32 HighThrold
        {
            get => _HighThrold;

            set
            {
                value = ValidateThrold(value);

                if (value > MaxThrold)
                {
                    value = MaxThrold;
                }
                else if (value < MinThrold + GapThrold * 2)
                {
                    value = MinThrold + GapThrold * 2;
                }

                if (value != _HighThrold)
                {
                    _HighThrold = value;
                    if (_MidThrold > value - GapThrold)
                    {
                        _MidThrold = value - GapThrold;
                    }

                    if (_LowThrold > value - GapThrold * 2)
                    {
                        _LowThrold = value - GapThrold * 2;
                    }

                    OnPropertyChanged("RefLevel");
                }
            }
        }

        private Int32 _LowThrold = 10;
        public Int32 LowThrold
        {
            get => _LowThrold;

            set
            {
                value = ValidateThrold(value);

                if (value > MaxThrold - GapThrold * 2)
                {
                    value = MaxThrold - GapThrold * 2;
                }
                else if (value < MinThrold)
                {
                    value = MinThrold;
                }

                if (value != _LowThrold)
                {
                    _LowThrold = value;
                    if (_MidThrold < value + GapThrold)
                    {
                        _MidThrold = value + GapThrold;
                    }

                    if (_HighThrold < value + GapThrold * 2)
                    {
                        _HighThrold = value + GapThrold * 2;
                    }

                    OnPropertyChanged("RefLevel");
                }
            }
        }

        private Int32 _MidThrold = 50;
        public Int32 MidThrold
        {
            get => _MidThrold;

            set
            {
                value = ValidateThrold(value);

                if (value > MaxThrold - GapThrold)
                {
                    value = MaxThrold - GapThrold;
                }
                else if (value < MinThrold + GapThrold)
                {
                    value = MinThrold + GapThrold;
                }

                if (value != _MidThrold)
                {
                    _MidThrold = value;
                    if (_HighThrold < value + GapThrold)
                    {
                        _HighThrold = value + GapThrold;
                    }
                    else if (_LowThrold > value - GapThrold)
                    {
                        _LowThrold = value - GapThrold;
                    }

                    OnPropertyChanged("RefLevel");
                }

            }
        }

        protected virtual Int32 ValidateThrold(Int32 value)
        {
            return Convert.ToInt32((Double)value / StpThrold) * StpThrold;
        } 
        #endregion

        private MeasureTopBaseRef _TopBaseRef = MeasureTopBaseRef.BaseTop;
        public MeasureTopBaseRef RefStandard
        {
            get => _TopBaseRef;
            set
            {
                if (_TopBaseRef != value)
                {
                    _TopBaseRef = value;
                    OnPropertyChanged();
                }
            }
        }

        private MeasureTopBaseRefUnit _RefUnit = MeasureTopBaseRefUnit.Percent;
        public MeasureTopBaseRefUnit RefUnit
        {
            get => _RefUnit;
            set
            {
                if (_RefUnit != value)
                {
                    _RefUnit = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public class MeasDualSrcExProp : MeasItemExProp
    {
        public MeasDualSrcExProp(Object? obj, PropertyChangedEventHandler? propertyChanged = null) : base(obj, propertyChanged)
        { }

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
            }
        }

        private ChannelId _Source2nd = ChannelId.C1;
        public ChannelId Source2nd
        {
            get => _Source2nd;
            set
            {
                if (_Source2nd != value)
                {
                    _Source2nd = value;
                    OnPropertyChanged();
                }
            }
        }

        private EdgeSlope _Slope2nd = EdgeSlope.Rise;
        public EdgeSlope Slope2nd
        {
            get => _Slope2nd;
            set
            {
                if (_Slope2nd != value)
                {
                    _Slope2nd = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public class MeasPosIndexExProp : MeasItemExProp
    {
        internal MeasPosIndexExProp(Object? container, PropertyChangedEventHandler? propertyChanged = null) : base(container, propertyChanged)
        {
            _PosIndex = new(nameof(PosIndex));
            //_PosIndex.PropertyChanged += (_, _) => propertyChanged?.Invoke(Container, new PropertyChangedEventArgs(nameof(PosIndex)));
            _PosIndex.PropertyChanged += propertyChanged;
        }

        private readonly LimitedNum<Int32> _PosIndex;

        public Int32 PosIndex
        {
            get => _PosIndex.Current;
            set => _PosIndex.Current = value;
        }

        public Int32 PosMaxIndex
        {
            get => _PosIndex.Max;
            init => _PosIndex.Max = value;
        }

        public Int32 PosMinIndex
        {
            get => _PosIndex.Min;
            init => _PosIndex.Min = value;
        }
    }

    //Zhangqc
    //internal class MeasureSimpleCalc
    //{
    //    public MeasureSimpleCalc(Action<String>? onPropertyChanged = null)
    //    {
    //        OnPropertyChanged = onPropertyChanged;
    //    }

    //    private Int32 _LeftIndex = 0;
    //    public Int32 LeftIndex
    //    {
    //        get => _LeftIndex;
    //        set
    //        {
    //            if (_LeftIndex != value)
    //            {
    //                _LeftIndex = value;
    //                OnPropertyChanged?.Invoke(nameof(LeftIndex));
    //            }
    //        }
    //    }

    //    private Int32 _RightIndex = 1;
    //    public Int32 RightIndex
    //    {
    //        get => _RightIndex;
    //        set
    //        {
    //            if (_RightIndex != value)
    //            {
    //                _RightIndex = value;
    //                OnPropertyChanged?.Invoke(nameof(RightIndex));
    //            }
    //        }
    //    }

    //    private MathBinaryType _Operator = MathBinaryType.Add;
    //    public MathBinaryType Operator
    //    {
    //        get => _Operator;
    //        set
    //        {
    //            if (_Operator != value)
    //            {
    //                _Operator = value;
    //                OnPropertyChanged?.Invoke(nameof(Operator));
    //            }
    //        }
    //    }

    //    public override String ToString()
    //    {
    //        return $@"P{LeftIndex}{Operator.GetAlias()}P{RightIndex}";
    //    }

    //    protected Action<String>? OnPropertyChanged { get; }
    //}
}

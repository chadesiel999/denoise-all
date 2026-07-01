using System;
using System.ComponentModel;

namespace ScopeX.Core.Tools
{
    internal class LimitedScale<V> : LimitedValue<Int32, Int32, V>
        where V : struct
    {
        public LimitedScale(String name) : base(name)
        { }

        private Int32 _Tick;
        public Int32 Tick
        {
            get => _Tick;
            set
            {
                value = ValidateTick(value);

                if (value != _Tick)
                {
                    _Tick = value;
                    OnPropertyChanged(Name);
                }
            }
        }

        private Boolean _IsCoarse = false;
        public Boolean IsCoarse
        {
            get { return _IsCoarse; }
            set
            {
                if (_IsCoarse != value)
                {
                    _IsCoarse = value;
                    if (_IsCoarse)
                        _Tick = 0;
                    OnPropertyChanged(Name);
                }
            }
        }

        private Int32 ValidateTick(Int32 value)
        {
            if (value > 0 && Current >= Max)
                value = 0;
            else if (value < 0 && Current <= Min)
                value = 0;
            //else if (value >= ScaleFactory.Default.GetPosTicks(Current))
            else if (IsTickOverflow(Current, value))
            {
                Current++;
                value = 0;
            }
            //else if (value <= ScaleFactory.Default.GetNegTicks(Current))
            else if (IsTickUnderflow(Current, value))
            {
                Current--;
                value = 0;
            }
            return value;
        }

        public Func<Int32, Int32, Boolean> IsTickOverflow
        {
            get;
            set;
        } = (tick, index) => tick > 0;

        public Func<Int32, Int32, Boolean> IsTickUnderflow
        {
            get;
            set;
        } = (tick, index) => tick < 0;

        public Func<V, (Int32, Int32)> GetIndex
        {
            get;
            internal set;
        } = (_) => default;
    }

    internal class LimitedPosition<T> : LimitedValue<T, Double, Double>
        where T : struct, IComparable<T>
    {
        public LimitedPosition(String name) : base(name)
        { }
        
        public Func<Double, Double, T> GetIndex
        {
            get;
            internal set;
        } = (_, _) => default;

        //private Double _PosStpIndex = 1E-6;
        //public Double PosStpIndex
        //{
        //    get => _PosStpIndex;
        //    set => _PosStpIndex = value;
        //}
    }
}

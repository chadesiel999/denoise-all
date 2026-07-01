using ScopeX.Core.Tools;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    internal class PositionScaleTuple : INotifyPropertyChanged
    {
        private readonly String _Name;

        public PositionScaleTuple(String name)
        {
            _Name = name;
            _LimitedPosition = new(_Name + "Position");
            _LimitedScale = new(_Name + "Scale");
        }

        #region Position
        private readonly LimitedPosition<Double> _LimitedPosition;

        protected Func<Double, Double, Double> GetPosIndex
        {
            get => _LimitedPosition.GetIndex;
            set => _LimitedPosition.GetIndex = value;
        }

        protected Func<Double, Double, Double> GetPosValue
        {
            get => _LimitedPosition.GetValue;
            set => _LimitedPosition.GetValue = value;
        }

        //Default 'PosIndexPerDiv' is 1000/div, But you can set 'PosIndexPerDiv' to arbitrary value, so 'PosIndex' does not append postfix "BymDiv"
        public virtual Double PosIndex
        {
            get => _LimitedPosition.Current;
            set => _LimitedPosition.Current = ValidatePosIndex(value);
        }

        public Double PosDefIndex
        {
            get;
            set;
        } = 0;

        public Double PosStpIndex
        {
            get;
            set;
        } = 1E-6;

        public Double PosIdxPerDiv
        {
            get;
            set;
        } = 1000;

        public Double PosMaxIndex
        {
            get => _LimitedPosition.Max;
            set => _LimitedPosition.Max = value;
        }

        public Double PosMinIndex
        {
            get => _LimitedPosition.Min;
            set => _LimitedPosition.Min = value;
        }

        protected Prefix PosPrefix
        {
            get => _LimitedPosition.Prefix;
            set => _LimitedPosition.Prefix = value;
        }

        protected String PosUnit
        {
            get => _LimitedPosition.Unit;
            set => _LimitedPosition.Unit = value;
        }

        protected Double ValidatePosIndex(Double posIndex)
        {
            //return Math.Round(posIndex / PosStpIndex, MidpointRounding.AwayFromZero) * PosStpIndex; //为了修复Bug：312 Trigger：调节时基，时基延迟值会变化 。使用Round会导致精度丢失，从而计算结果变化了。
            return (posIndex / PosStpIndex) * PosStpIndex;
        }
        #endregion

        #region Scale For Division
        private readonly LimitedScale<Double> _LimitedScale;

        private (Int32 Index, Double Value) _InitialScale = (0, 5);
        public (Int32 Index, Double Value) InitialScale
        {
            get => _InitialScale;
            set
            {
                if (_InitialScale != value)
                {
                    _InitialScale = value;
                    OnPropertyChanged(_Name + "Scale");
                }
            }
        }

        protected internal Func<Double, (Int32, Int32)> GetScaleIndex
        {
            get => _LimitedScale.GetIndex;
            set => _LimitedScale.GetIndex = value;
        }

        protected internal Func<Int32, Int32, Double> GetScaleValue
        {
            get => _LimitedScale.GetValue;
            set => _LimitedScale.GetValue = value;
        }

        protected Func<Int32, Int32, Boolean> IsScaleTickOverflow
        {
            set => _LimitedScale.IsTickOverflow = value;
        }

        protected Func<Int32, Int32, Boolean> IsScaleTickUnderflow
        {
            set => _LimitedScale.IsTickUnderflow = value;
        }

        public Int32 ScaleTick
        {
            get => _LimitedScale.Tick;
            set => _LimitedScale.Tick = value;
        }

        protected internal Boolean IsCoarse
        {
            get => _LimitedScale.IsCoarse;
            set => _LimitedScale.IsCoarse = value;
        }

        public virtual Int32 ScaleIndex
        {
            get => _LimitedScale.Current;
            set
            {
                if (_LimitedScale.Tick < 0)
                {
                    if (value > _LimitedScale.Current)
                        value--;
                    _LimitedScale.Tick = 0;
                }
                else if (_LimitedScale.Tick > 0)
                {
                    if (value < _LimitedScale.Current)
                        value++;
                    _LimitedScale.Tick = 0;
                }
                if (_LimitedScale.Current != value)
                {
                    _LimitedScale.Current = value;
                    OnPropertyChanged(_Name + "Scale");
                }
            }
        }

        public IgnoreScaleLimit IgnoreScaleMaxMin
        {
            get
            {
                return _LimitedScale.IgnoreMaxMin;
            }
            set
            {
                _LimitedScale.IgnoreMaxMin = value;
            }
        }

        public IgnoreScaleLimit IgnorePositionMaxMin
        {
            get
            {
                return _LimitedPosition.IgnoreMaxMin;
            }
            set
            {
                _LimitedPosition.IgnoreMaxMin = value;
            }
        }

        public Int32 ScaleMinIndex
        {
            get => _LimitedScale.Min;
            set => _LimitedScale.Min = value;
        }

        public Int32 ScaleMaxIndex
        {
            get => _LimitedScale.Max;
            set => _LimitedScale.Max = value;
        }

        protected Prefix ScalePrefix
        {
            get => _LimitedScale.Prefix;
            set => _LimitedScale.Prefix = value;
        }

        protected String ScaleUnit
        {
            get => _LimitedScale.Unit;
            set => _LimitedScale.Unit = value;
        }

        //!!!Test Code
        private protected void ReCalcMaxMinPosIdx(Double scale)
        {
            var ratio = scale / GetScaleValue(ScaleIndex, ScaleTick);
            PosMinIndex = PosDefIndex - ratio * Constants.VIS_XDIVS_NUM * PosIdxPerDiv / 2;
            PosMaxIndex = PosDefIndex + ratio * Constants.VIS_XDIVS_NUM * PosIdxPerDiv / 2;
        }

        public virtual void SetInitScaleValue(Int32 index, Double value, Int32 minindex, Int32 maxindex, Boolean synchronize)
        {
        }

        #endregion

        public IWeakTip Prompter
        {
            set
            {
                _LimitedScale.Prompter = value;
                _LimitedPosition.Prompter = value;
            }
        }

        #region INotifyPropertyChanged
        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
                _LimitedPosition.PropertyChanged += value;
                _LimitedScale.PropertyChanged += value;
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
                _LimitedPosition.PropertyChanged -= value;
                _LimitedScale.PropertyChanged -= value;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

}

using EventBus;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ScopeX.ComModel;

namespace ScopeX.Core.Tools
{
    internal class LimitedNum<T> : INotifyPropertyChanged
        where T : struct, IComparable<T>
    {
        protected String Name
        {
            get;
        }

        public IWeakTip? Prompter
        {
            get;
            set;
        }

        public LimitedNum(String name = "")
        {
            Name = name;
        }

        private T _Current;
        private IgnoreScaleLimit _IgnoreMaxMin = IgnoreScaleLimit.None;
        /// <summary>
        /// 忽略最大值最小值达到提示
        /// </summary>
        public IgnoreScaleLimit IgnoreMaxMin
        {
            get => _IgnoreMaxMin;
            set=> _IgnoreMaxMin = value;
        }

        public T Current
        {
            get
            {
                return _Current;
            }
            set
            {
                if (value.CompareTo(Min) < 0)
                {
                    value = Min;
                    if (IgnoreMaxMin!= IgnoreScaleLimit.Both&& IgnoreMaxMin!= IgnoreScaleLimit.Min)
                    {
                        Prompter?.Write(Name + nameof(Current), MsgTipId.LessthanMin, false, "", 1);
                    }
                }
                else if (value.CompareTo(Max) > 0)
                {
                    value = Max;
                    if (IgnoreMaxMin != IgnoreScaleLimit.Both && IgnoreMaxMin != IgnoreScaleLimit.Max)
                    {
                        Prompter?.Write(Name + nameof(Current), MsgTipId.GreatethanMax, false, "", 1);
                    }
                }

                if (!value.Equals(_Current))
                {
                    _Current = value;
                    OnPropertyChanged(Name);
                }
            }
        }

        private T _Max;
        public T Max
        {
            get
            {
                return _Max;
            }
            set
            {
                if (!value.Equals(_Max))
                {
                    _Max = value;

                    if (_Current.CompareTo(_Max) > 0)
                    {
                        _Current = _Max;
                        OnPropertyChanged(Name);
                    }
                }
            }
        }

        private T _Min;

        public T Min
        {
            get
            {
                return _Min;
            }
            set
            {
                if (!value.Equals(_Min))
                {
                    _Min = value;

                    if (_Current.CompareTo(_Min) < 0)
                    {
                        _Current = _Min;
                        OnPropertyChanged(Name);
                    }
                }
            }
        }

        protected PropertyChangedEventHandler? _PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
            }
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal class LimitedValue<T, U, V> : LimitedNum<T>
        where T : struct, IComparable<T>
        where V : struct
    {
        public LimitedValue(String name = "", Prefix pfx = Prefix.Empty, String unit = "") : base(name)
        {
            Prefix = pfx;
            _Unit = unit;
        }

        public Func<T, U, V> GetValue
        {
            get;
            internal set;
        } = (_, _) => default;

        public Prefix Prefix
        {
            get;
            set;
        }

        private String _Unit = "";
        public String Unit
        {
            get => _Unit;
            set
            {
                if (_Unit != value)
                {
                    _Unit = value;
                    OnPropertyChanged(Name + nameof(Unit));
                }
            }
        }
    }


}

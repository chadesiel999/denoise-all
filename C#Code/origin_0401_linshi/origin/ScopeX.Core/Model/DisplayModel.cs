// Copyright (c) ScopeX. All Rights Reserved
// <author>QC</author>
// <date>2022/4/20</date>

namespace ScopeX.Core
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using ScopeX.ComModel;

    public enum GridType
    {
        Full = 0,
        Brief = 1,
        None = 2
    }

    //public enum WfmDrawMode
    //{
    //    Vector = 0,
    //    Dot = 1
    //}

    //public enum WfmPersist
    //{
    //    Close,
    //    Auto,
    //    Infinity
    //}

    internal class DisplayModel : INotifyPropertyChanged
    {
        private WfmDrawMode _DrawMode = WfmDrawMode.Vector;
        public WfmDrawMode DrawMode
        {
            get => _DrawMode;
            set
            {
                if (_DrawMode != value)
                {
                    _DrawMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public PlotRenderType RenderType { get; set; } = PlotRenderType.None;

        private MultiWfmsLayout _WfmLayout = MultiWfmsLayout.Overlay;
        public MultiWfmsLayout WfmLayout
        {
            get => _WfmLayout;
            set
            {
                if (_WfmLayout != value)
                {
                    _WfmLayout = value;
                    OnPropertyChanged();
                }
            }
        }

        private WfmPersist _Persist = WfmPersist.Close;
        public WfmPersist Persist
        {
            get => _Persist;
            set
            {
                if (_Persist != value)
                {
                    _Persist = value;
                    OnPropertyChanged();
                }
            }
        }

        private static Int32 ValidateIntensity(Int32 value)
        {
            if (value > MaxIntensity)
            {
                value = MaxIntensity;
            }
            else if (value < MinIntensity)
            {
                value = MinIntensity;
            }

            return value;
        }

        //wcj:临时解决"波形亮度=100"时，LA波形不显示的Bug;
        public static readonly Int32 MaxIntensity = 99;

        public static readonly Int32 MinIntensity = 10;

        public static readonly Int32 DefIntensity = Constants.DEF_WAVE_INTENSITY;

        private Int32 _WfmIntensity = DefIntensity;
        public Int32 WfmIntensity
        {
            get => _WfmIntensity;
            set
            {
                value = ValidateIntensity(value);
                if (_WfmIntensity != value)
                {
                    _WfmIntensity = value;
                    OnPropertyChanged();
                }
            }
        }

        private GridType _GridStyle = GridType.Brief;
        public GridType GridStyle
        {
            get => _GridStyle;
            set
            {
                if (_GridStyle != value)
                {
                    _GridStyle = value;
                    OnPropertyChanged();
                }
            }
        }


        private Int32 _GridIntensity = Constants.DEF_GRID_INTENSITY;
        public Int32 GridIntensity
        {
            get => _GridIntensity;
            set
            {
                value = ValidateIntensity(value);
                if (_GridIntensity != value)
                {
                    _GridIntensity = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _AxisTickVisible = true;
        public Boolean AxisTickVisible
        {
            get => _AxisTickVisible;
            set
            {
                if (_AxisTickVisible != value)
                {
                    _AxisTickVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _XAxisTickBottom = true;
        public Boolean XAxisTickBottom
        {
            get => _XAxisTickBottom;
            set
            {
                if (_XAxisTickBottom != value)
                {
                    _XAxisTickBottom = value;
                    OnPropertyChanged();
                }
            }
        }

        private Boolean _YAxisTickRight = true;
        public Boolean YAxisTickRight
        {
            get => _YAxisTickRight;
            set
            {
                if (_YAxisTickRight != value)
                {
                    _YAxisTickRight = value;
                    OnPropertyChanged();
                }
            }
        }
        private Int32 _AnalogZIndex = 0;

        public Int32 AnalogZIndex
        {
            get => _AnalogZIndex;
            set
            {
                if (_AnalogZIndex != value)
                {
                    _AnalogZIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        internal Action<Int32>? SetBrightness { get; set; } = null;
        internal Func<Int32>? GetBrightness { get; set; } = null;

        private Int32 _ScreenBrightness = 90;
        public Int32 ScreenBrightness
        {
            get
            {
                if (!PlatformManager.Default.Platform.EnableGetOrSetScreenBrightness)
                {
                    return 0;
                }
                else
                    return GetBrightness?.Invoke() ?? _ScreenBrightness;
            }
            set
            {
                if (!PlatformManager.Default.Platform.EnableGetOrSetScreenBrightness)
                {
                    return;
                }
                else//if (_ScreenBrightness != value)
                {
                    _ScreenBrightness = Math.Clamp(value, 5, 100);
                    SetBrightness?.Invoke(_ScreenBrightness);
                    OnPropertyChanged();
                }
            }
        }


        internal Func<Int32, Boolean>? SetContrast { get; set; } = null;
        internal Func<(Boolean, Int32)>? GetContrast { get; set; } = null;

        private Int32 _ScreenContrast = 70;

        public Int32 ScreenContrast
        {
            get
            {
                var res = GetContrast?.Invoke();
                if (res != null)
                {
                    return res.Value.Item1 ? res.Value.Item2 : _ScreenContrast;
                }
                else
                    return _ScreenContrast;
            }
            set
            {
                if (_ScreenContrast != value)
                {
                    value = Math.Clamp(value, 50, 100);
                    var res = SetContrast?.Invoke(value);
                    if (res ?? false)
                    {
                        _ScreenBrightness = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        internal Func<Boolean>? GetTouchable { get; set; } = null;
        internal Action<Boolean>? SetTouchable { get; set; } = null;

        private Boolean _TouchLoack = false;
        /// <summary>
        /// 触摸使能 True --> 使能打开表示锁定（不支持触摸），false --> 使能关闭表示未锁定（支持触摸）
        /// </summary>
        public Boolean TouchLock
        {
            get => !(GetTouchable?.Invoke() ?? false);
            set
            {
                _TouchLoack = !(GetTouchable?.Invoke() ?? false);
                if (_TouchLoack != value)
                {
                    SetTouchable?.Invoke(!value);
                    OnPropertyChanged();
                    KeyLed.Default.SetLed(LedEnum.LedTouchLock, !(GetTouchable?.Invoke() ?? false));
                }
            }
        }

        private Boolean _SystemMessageLock = false;


        public Boolean SystemMessageLock
        {
            get => _SystemMessageLock;
            set => _SystemMessageLock = value;
        }

        internal Func<String>? GetLocalTime { get; set; } = null;
        internal Func<String, Boolean>? SetLocalTime { get; set; } = null;

        public String _SystemTime = DateTime.Now.ToString("yyyy,M,d,HH,m,s");

        public String SystemTime
        {
            get => GetLocalTime?.Invoke() ?? _SystemTime;
            set
            {
                if (_SystemTime != value)
                {
                    var res = SetLocalTime?.Invoke(value) ?? false;
                    if (res)
                    {
                        _SystemTime = value;
                        OnPropertyChanged();
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
}

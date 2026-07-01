using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace ScopeX.Core
{
    public class AreaHistogramModel : INotifyPropertyChanged
    {
        private Boolean _Enabled;

        public Boolean Enabled
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled != value)
                {
                    _Enabled = value;
                    OnPropertyChanged();
                }
            }
        }


        private (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) _RectanglePoints;
        /// <summary>
        /// 直方图虚拟坐标值
        /// </summary>
        public (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) RectanglePoints
        {
            get => _RectanglePoints;
            set
            {
                if (value != _RectanglePoints)
                {
                    _RectanglePoints = value;
                    OnPropertyChanged();
                }
            }
        }

        private (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) _PositionOfHV;
        /// <summary>
        /// 直方图期望坐标值 (x us,y mv)
        /// </summary>
        public (PointF LeftUp, PointF RightUp, PointF RightDown, PointF LeftDown) PositionOfHV
        {
            get => _PositionOfHV;
            set
            {
                if (value != _PositionOfHV)
                {
                    _PositionOfHV = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// 当前形状所属的垂直缩放值，用于记录和计算形状缩放使用
        /// </summary>
        internal Double VerticalScale
        {
            get
            {
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(DsoPrsnt.FocusId, out var channel))
                {
                    if (channel is AnalogPrsnt aprsnt)
                    {
                        return aprsnt.ScaleBymV;
                    }
                }
                return Double.NaN;
            }
        }

        /// <summary>
        /// 垂直档位偏移div数量
        /// </summary>
        internal Double VerticalPosIndexBymDiv
        {
            get
            {
                if (DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(DsoPrsnt.FocusId, out var channel))
                {
                    if (channel is AnalogPrsnt aprsnt)
                    {
                        return aprsnt.PosIndexBymDiv;
                    }
                }
                return Double.NaN;
            }
        }

        protected PropertyChangedEventHandler? _PropertyChanged;

        public virtual event PropertyChangedEventHandler? PropertyChanged
        {
            add
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Combine(Delegate.Remove(_PropertyChanged, value), value);
                TriggerShareParameter.Default.PropertyChanged += value;
            }
            remove
            {
                _PropertyChanged = (PropertyChangedEventHandler?)Delegate.Remove(_PropertyChanged, value);
                TriggerShareParameter.Default.PropertyChanged -= value;
            }
        }


        protected void ItemPropertyChanged(Object? sender, PropertyChangedEventArgs e)
        {
            _PropertyChanged?.Invoke(sender, e);
        }

        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            _PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

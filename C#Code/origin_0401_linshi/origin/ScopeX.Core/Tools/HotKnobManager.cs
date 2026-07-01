using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ScopeX.Core
{
    public class HotKnobManager : IHotKnob
    {
        public static HotKnobManager Default { get; } = new();

        private KeySpeed<Int32> _KeySpeed;
        private readonly IReadOnlyList<Int32> _SupportSpeedUpKey = new List<Int32>()
        {
            KeyCode.TRIGGER,
            KeyCode.KNOB_MULTI_LEFT,
            KeyCode.KNOB_MULTI_RIGHT,
            KeyCode.KNOB_UPMULTI_LEFT,
            KeyCode.KNOB_UPMULTI_RIGHT,
            KeyCode.KNOB_DNMULTI_LEFT,
            KeyCode.KNOB_DNMULTI_RIGHT,
        };

        /// <summary>
        /// 多功能旋钮对应的对象
        /// </summary>
        public Object? Presenter
        {
            get;
            set;
        }

        /// <summary>
        /// 旋钮最小步进
        /// </summary>
        public Double MinStep { get; set; } = 1;

        /// <summary>
        /// 多功能旋钮对应的对象需要设置的属性的属性名
        /// </summary>
        public String? PropertyName
        {
            get;
            set;
        }

        /// <summary>
        /// 是否细调
        /// </summary>
        public Boolean Fineable { get; set; }

        /// <summary>
        /// 是否可加减调节
        /// </summary>
        public Boolean Adjustable { get; set; } = false;

        /// <summary>
        /// 对象是否丢失焦点
        /// </summary>
        public Boolean LostFocus { get; set; } = true;

        public Action<Object, Double>? TurnRight;
        public Action<Object, Double>? TurnLeft;

        public HotKnobManager()
        {
            _KeySpeed = new KeySpeed<Int32>(_SupportSpeedUpKey, 500, 1, 0.15);
        }

        //旋钮控件中主要是iconbutton,touchneb和textbox
        public void Turn(Int32 keyCode, Int32 keyStep)
        {
            if (LostFocus)
                return;

            var step = _KeySpeed.GetStep(keyCode, Math.Abs(MinStep));//这是粗调时的步进，随着旋转旋钮的速度变化(如果出现调节旋钮值未改变的问题，有可能是MinStep太小)
            PropertyInfo? propertyInfo = null;

            switch (keyCode)
            {
                case KeyCode.KNOB_MULTI_RIGHT:
                    step = Fineable ? MinStep : (Int32)step;
                    if (Adjustable)
                    {
                        TurnRight?.Invoke(this, step);
                    }
                    else
                    {
                        if (TryGetPropertyInfo(Presenter, PropertyName, out propertyInfo))
                        {
                            TrySetPropertyValue(Presenter, propertyInfo, step.ToString());
                        }
                    }

                    break;
                case KeyCode.KNOB_MULTI_LEFT:
                    step = Fineable ? -MinStep : -(Int32)step;
                    if (Adjustable)
                    {
                        TurnLeft?.Invoke(this, step);
                    }
                    else
                    {
                        if (TryGetPropertyInfo(Presenter, PropertyName, out propertyInfo))
                        {
                            TrySetPropertyValue(Presenter, propertyInfo, (step).ToString());
                        }
                    }
                    break;
                default:
                    break;
            }

            if (PlatformManager.Default.Platform.KeyEnumCursor() && keyCode == KeyCode.KNOB_MULTI_SELECT)
            {
                Fineable = !Fineable;
            }
        }

        public void LedControl(Boolean status)
        {
            KeyLed.Default.SetLed(LedEnum.LedMultipupose, status || DsoModel.Default.Cursors.Active);
        }

        //初始化控件属性
        #region 通过反射更改属性值的函数

        private Boolean TrySetPropertyValue(Object? prsnt, PropertyInfo? propertyInfo, String valueString)
        {
            if (propertyInfo == null)
            {
                return false;
            }

            var setValue = ConvertObject(valueString, propertyInfo.PropertyType);
            if (setValue != null)
            {
                var propertyvalue = propertyInfo.GetValue(prsnt);
                if (propertyvalue == null)
                    return false;

                Object? value = null;
                if (propertyvalue.GetType() == typeof(Int32))
                {
                    value = Convert.ToInt32(setValue) + (Int32)propertyvalue;
                }
                else if (propertyvalue.GetType() == typeof(Double))
                {
                    value = (Double)setValue + (Double)propertyvalue;
                }
                else if (propertyvalue.GetType() == typeof(Int64))
                {
                    value = (Int64)setValue + (Int64)propertyvalue;
                }
                else if (propertyvalue.GetType() == typeof(Single))
                {
                    value = (Single)setValue + (Single)propertyvalue;
                }
                propertyInfo.SetValue(prsnt, value);
            }
            else
                return false;
            return propertyInfo != null;
        }

        private Boolean TryGetPropertyInfo(Object? obj, String? propertyName, out PropertyInfo? propertyInfo)
        {
            propertyInfo = null;
            if (obj != null && !String.IsNullOrEmpty(propertyName))
                propertyInfo = obj.GetType().GetProperty(propertyName);

            return propertyInfo != null;
        }

        private Object? ConvertObject(String obj, Type type)
        {
            if (type == typeof(Int32))
            {
                if (Decimal.TryParse(obj, out var val))
                {
                    var value = (Int32)val;
                    return value;
                }
            }
            else if (type == typeof(Int64))
            {
                if (Decimal.TryParse(obj, out var val))
                {
                    var value = (Int64)val;
                    return value;
                }
            }
            else if (type == typeof(Double))
            {
                if (obj.Contains('e') || obj.Contains('E'))
                {
                    if (Double.TryParse(obj, NumberStyles.AllowExponent, null, out Double v))
                        return v;
                }
                else if (Double.TryParse(obj, out var v))
                    return v;
            }
            else if (type == typeof(Single))
            {
                if (Decimal.TryParse(obj, out var val))
                {
                    var value = (Single)val;
                    return value;
                }
            }
            return null;
        }

        #endregion
    }

    internal class KeySpeed<T> where T : unmanaged
    {
        private List<T> _SupportKey = new List<T>();
        private T _LastKey = default;
        private DateTime _LastTime = DateTime.Now;
        private Double _LastTicks = 0; // 记录上一次的ticks值
        private readonly Double _StopStep;
        private readonly Double _StartStep;
        private readonly Double _R;
        /*
         * 较低的平滑因子（接近0）：新数据对平均值的影响较小，更依赖于历史数据。这种情况下，EMA的变化会更加平缓。
         * 较高的平滑因子（接近1）：新数据对平均值的影响较大，更依赖于最新数据。这种情况下，EMA的变化会更快速地反映最新数据的变化。
         * 假设 _SmoothingFactor 为0.1，0.5和0.9的效果：
         * 0.1：新数据的影响较小，变化平滑，适合需要缓慢响应的场景。
         * 0.5：新数据和历史数据的影响相对均衡，适合一般场景。
         * 0.9：新数据的影响较大，变化快速，适合需要快速响应的场景。
         */
        private readonly Double _SmoothingFactor = 0.1; // EMA平滑因子

        public KeySpeed(IEnumerable<T> supportKey, Double stopStep = 500, Double startStep = 1, Double r = 0.15, Double SoomthingFactor = 0.25)
        {
            if (supportKey == null || !supportKey.Any()) throw new ArgumentNullException(nameof(supportKey));
            _SupportKey.AddRange(supportKey);
            _StopStep = stopStep;
            _StartStep = startStep;
            _R = r;
            _SmoothingFactor = SoomthingFactor;
        }

        public Int32 GetStep(T keycode, Int32 step)
        {
            var sign = Math.Sign(step);
            step = Math.Abs(step);
            Int32 backstep = step;
            if (step == 0 || !_SupportKey.Contains(keycode))
            {
                // Do nothing if step is 0 or key is not supported
            }
            else
            {
                DateTime currentTime = DateTime.Now;
                Double currentTicks = (currentTime - _LastTime).TotalMilliseconds;

                // 使用EMA平滑处理ticks
                if (_LastTicks == 0)
                {
                    _LastTicks = currentTicks; // 初始化
                }
                else
                {
                    _LastTicks = _SmoothingFactor * currentTicks + (1 - _SmoothingFactor) * _LastTicks;
                }

                Double smoothedTicks = _LastTicks;
                smoothedTicks = Math.Max(_StartStep, Math.Min(smoothedTicks, _StopStep));
                if (!_LastKey.Equals(keycode))
                {
                    backstep *= (Int32)_StartStep;
                }
                else
                {
                    backstep *= Logistic(_StopStep / smoothedTicks, r: _R);
                }
            }
            _LastKey = keycode;
            _LastTime = DateTime.Now;
            return backstep * sign;
        }

        /// <summary>
        /// 获取步进
        /// </summary>
        /// <param name="keycode">按键码</param>
        /// <param name="step">步进</param>
        /// <returns></returns>
        public Double GetStep(T keycode, Double step)
        {
            if (!_LastKey.Equals(keycode) && !keycode.Equals(KeyCode.KNOB_MULTI_SELECT))
            {
                _LastTime = DateTime.Now;
            }

            var sign = Math.Sign(step);
            step = Math.Abs(step);
            var backstep = step;
            if (step == 0 || !_SupportKey.Contains(keycode))
            {
                // Do nothing if step is 0 or key is not supported
            }
            else
            {
                DateTime currentTime = DateTime.Now;
                Double currentTicks = (currentTime - _LastTime).TotalMilliseconds;

                // 使用EMA平滑处理ticks
                if (_LastTicks == 0)
                {
                    _LastTicks = currentTicks; // 初始化
                }
                else
                {
                    _LastTicks = _SmoothingFactor * currentTicks + (1 - _SmoothingFactor) * _LastTicks;
                }

                Double smoothedTicks = _LastTicks;
                smoothedTicks = Math.Max(_StartStep, Math.Min(smoothedTicks, _StopStep));
                if (!_LastKey.Equals(keycode))
                {
                    backstep *= (Int32)_StartStep;
                }
                else
                {
                    backstep *= Logistic(_StopStep / smoothedTicks, r: _R);
                }
            }
            _LastKey = keycode;
            _LastTime = DateTime.Now;
            return backstep * sign;
        }

        private Int16 Logistic(Double Int32erval, Double stopStep = 300, Double startStep = 1, Double r = 0.08)
        {
            return (Int16)Math.Ceiling(stopStep * startStep * Math.Pow(Math.E, r * Int32erval) / (stopStep + startStep * (Math.Pow(Math.E, r * Int32erval) - 1)));
        }
    }
}

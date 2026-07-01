using System;
using System.Collections.Generic;
using System.Linq;

namespace ScopeX.U2
{
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
            if (!_LastKey.Equals(keycode) && !keycode.Equals(KeyCode.KNOB_MULTI_SELECT))
            {
                _LastTime = DateTime.Now;
            }

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
                    //if (smoothedTicks > _StopStep / 2)
                    //{
                    //    smoothedTicks = _StopStep;
                    //}
                    //else if (smoothedTicks < _StartStep)
                    //{
                    //    smoothedTicks = _StartStep;
                    //}
                    backstep *= Logistic(_StopStep / smoothedTicks, r: _R);
                }
            }
            _LastKey = keycode;
            _LastTime = DateTime.Now;
            return backstep * sign;
        }

        private Int16 Logistic(Double interval, Double stopStep = 300, Double startStep = 1, Double r = 0.08)
        {
            return (Int16)Math.Ceiling(stopStep * startStep * Math.Pow(Math.E, r * interval) / (stopStep + startStep * (Math.Pow(Math.E, r * interval) - 1)));
        }
    }
}


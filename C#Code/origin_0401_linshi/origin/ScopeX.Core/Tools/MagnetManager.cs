using System;
using System.Collections.Generic;

namespace ScopeX.Core.Tools
{
    /// <summary>
    /// 磁吸功能管理器
    /// </summary>
    public class MagnetManager
    {
        /// <summary>
        /// 最后一次记录
        /// </summary>
        private Double _lastValue;
        /// <summary>
        /// 识别阈值
        /// </summary>
        public Double Threshold { get; set; }

        public Double Step { get; private set; }

        public Double Min { get; private set; }

        public Double Max { get; private set; }

        /// <summary>
        /// 可以磁吸的点位集合
        /// </summary>
        public IEnumerable<Double>? KeyPoints { get; private set; }

        /// <summary>
        /// 使用范围和步进生成磁吸点。
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="step">步进值</param>
        /// <returns></returns>
        private List<Double> GenerateValues(Double min, Double max, Double step)
        {
            List<Double> values = new List<Double>();
            // 计算生成的数组长度
            Int32 length = (Int32)Math.Ceiling((max - min) / step) + 1;

            // 生成数组元素
            Double value = min;
            for (Int32 i = 0; i < length; i++)
            {
                values.Add(value);
                value += step;
            }

            return values;
        }

        /// <summary>
        /// 裁定是否可以磁吸，返回磁吸值（从范围外进入磁吸时生效，离开磁吸范围时不生效）
        /// </summary>
        /// <param name="value">待判定的值</param>
        public Double? Determine(Double value, Double min, Double max, Double step = 1000)
        {
            if (min != Min || max != Max && step != Step || KeyPoints == null)
            {
                KeyPoints = GenerateValues(min, max, step);
                Min = min;
                Max = max;
                Step = step;
            }

            Boolean? isleavling = null;
            foreach (var point in KeyPoints)
            {
                if (Math.Abs(point - value) <= Threshold)
                {
                    isleavling = IsLeaving(_lastValue, value, point - Threshold, point + Threshold);
                    if (isleavling == true)
                    {
                        // 如果正在离开
                        _lastValue = value;
                        return null;
                    }
                    //_lastValue = point;
                    _lastValue = value;
                    return point;
                }
            }

            _lastValue = value;
            return null;
        }

        /// <summary>
        /// 判定是否正在离开磁吸范围
        /// </summary>
        /// <param name="last">上一次记录的值</param>
        /// <param name="current">当前值</param>
        /// <param name="rangeStart">范围开始值</param>
        /// <param name="rangeEnd">范围结束值</param>
        /// <returns></returns>
        private Boolean? IsLeaving(Double last, Double current, Double rangeStart, Double rangeEnd)
        {
            // 计算范围中心点
            Double center = (rangeStart + rangeEnd) / 2;

            // 计算当前值与中心点的距离
            Double currentDistance = Math.Abs(current - center);

            // 计算上一个值与中心点的距离
            Double lastDistance = Math.Abs(last - center);

            if (currentDistance < lastDistance)
            {
                return false;
                // Approaching the center of the range
            }
            else if (currentDistance > lastDistance)
            {
                return true;
                // Moving away from the center of the range
            }
            else
            {
                return null;
                // Maintaining the same distance from the center of the range 
            }
        }

        private Boolean IsInRange(Double value, Double min, Double max)
        {
            return value >= min && value <= max;
        }
    }
}

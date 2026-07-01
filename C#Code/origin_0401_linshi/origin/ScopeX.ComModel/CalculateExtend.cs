using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ScopeX.ComModel
{
    /// <summary>
    /// 计算扩展类
    /// </summary>
    public static class CalculateExtend
    {
        #region 求最大值最小值和均值
        /// <summary>
        /// Int类型
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static (Int32 Max, Int32 Min, Double Average) GetMaxMinAvg(this Int32[] data)
        {
            // 确保集合不为空
            if (data == null || !data.Any())
            {
                throw new ArgumentException("Collection cannot be null or empty.");
            }
            Int32 max = data[0];
            Int32 min = data[0];
            Double sum = 0;
            Double count = 0;

            // 遍历集合，计算最大值、最小值和总和
            foreach (var item in data)
            {
                // 更新最大值
                if (item > max)
                {
                    max = item;
                }
                // 更新最小值
                if (item < min)
                {
                    min = item;
                }
                // 累加求和
                sum += item;
                count++;
            }
            // 计算均值
            Double average = sum / count;

            return (max, min, average);
        }

        /// <summary>
        /// Double类型
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static (Double Max, Double Min, Double Average) GetMaxMinAvg(this Double[] data)
        {
            // 确保集合不为空
            if (data == null || !data.Any())
            {
                throw new ArgumentException("Collection cannot be null or empty.");
            }
            Double max = data[0];
            Double min = data[0];
            Double sum = 0;
            Double count = 0;

            // 遍历集合，计算最大值、最小值和总和
            foreach (var item in data)
            {
                // 更新最大值
                if (item > max)
                {
                    max = item;
                }
                // 更新最小值
                if (item < min)
                {
                    min = item;
                }
                // 累加求和
                sum += item;
                count++;
            }
            // 计算均值
            Double average = sum / count;

            return (max, min, average);
        }
        #endregion


        public static Int32 MaxIndex(this Double[] array)
        {
            Double max = Double.MinValue;
            Int32 index = -1;
            for (Int32 i = 0, l = array.Length; i < l; i++)
            {
                if (array[i] > max)
                {
                    max = array[i];
                    index = i;
                }
            }

            return index;
        }


        public static Double[] RangeSelectorArray(this Double[] array, Func<Double, Double> selector)
        {
            if (selector == null)
                return array;

            Int32 l = array.Length;
            Double[] temp = new Double[l];

            for (Int32 i = 0; i < l; i++)
            {
                temp[i] = selector.Invoke(i); ;
            }

            return temp;
        }

        public static Double[] SelectorArray(this Double[] array, Func<Double, Double> selector)
        {
            if (selector == null)
                return array;

            Int32 l = array.Length;
            Double[] temp = new Double[l];

            for (Int32 i = 0; i < l; i++)
            {
                temp[i] = selector.Invoke(array[i]);
                ;
            }

            return temp;
        }

        public static Double[] SelectorArray(this List<Double> array, Func<Double, Double> selector)
        {
            if (selector == null)
                return array.ToArray();

            Int32 l = array.Count;
            Double[] temp = new Double[l];

            for (Int32 i = 0; i < l; i++)
            {
                temp[i] = selector.Invoke(array[i]);
            }

            return temp;
        }

        public static Double[] SkipArray(this Double[] array, Int32 skipcount)
        {
            Int32 l = array.Length;
            if (skipcount >= l)
            {
                return new Double[0];
            }
            Double[] temp = new Double[l - skipcount];

            for (Int32 i = skipcount; i < l; i++)
            {
                temp[i - skipcount] = array[i];
            }
            return temp;
        }

        public static Double SelectorSum(this IEnumerable<Double> data, Func<Double, Double> selector)
        {
            Double sum = 0;
            foreach (var value in data)
            {
                sum += selector.Invoke(value);
            }
            return sum;
        }

        public static Double RangeSum(this IEnumerable<Double> data, Int32 startIndex, Int32 count)
        {
            Double sum = 0;
            Int32 currentIndex = 0;

            foreach (var item in data)
            {
                if (currentIndex >= startIndex && currentIndex < startIndex + count)
                {
                    sum += item;
                }
                currentIndex++;

                if (currentIndex >= startIndex + count)
                {
                    break;
                }
            }

            return sum;
        }

        public static Double[] ReverseArray(this Double[] array)
        {
            Int32 l = array.Length;
            Double[] reversed = new Double[l];
            for (Int32 i = 0; i < l; i++)
            {
                reversed[i] = array[l - 1 - i];
            }

            return reversed;
        }

        public static Double[] TakeArray(this Double[] array, Int32 takeCount)
        {
            Double[] take = new Double[takeCount];

            for (Int32 i = 0; i < takeCount; i++)
            {
                take[i] = array[i];
            }

            return take;
        }


        public static Double[] TakeAndReverseArray(this Double[] array, Int32 takeCount)
        {
            Double[] result = new Double[takeCount];
            Int32 length = array.Length;

            for (Int32 i = 0; i < takeCount; i++)
            {
                result[i] = array[length - i - 1];
            }

            return result;
        }


        #region 复数 Complex

        public static Double[] RealArray(this Complex[] array)
        {
            if (array == null || array.Length == 0)
                return new Double[0];

            Int32 length = array.Length;
            Double[] result = new Double[length];

            for (Int32 i = 0; i < length; i++)
            {
                result[i] = array[i].Real;
            }

            return result;
        }

        public static (Double[] Real, Double[] Imag, Double[] Amp) ToDoubleArray(this Complex[] array)
        {
            if (array == null || array.Length == 0)
                return (new Double[0], new Double[0], new Double[0]);

            Int32 length = array.Length;
            Double[] real = new Double[length];
            Double[] imag = new Double[length];
            Double[] amp = new Double[length];

            for (Int32 i = 0; i < length; i++)
            {
                real[i] = array[i].Real;
                imag[i] = array[i].Imaginary;
                amp[i] = Math.Sqrt(real[i] * real[i] + imag[i] * imag[i]) / length;
            }

            return (real, imag, amp);
        }

        #endregion
    }
}

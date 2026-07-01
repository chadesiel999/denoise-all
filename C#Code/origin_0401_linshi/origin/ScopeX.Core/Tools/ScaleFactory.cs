using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.POIFS.Crypt.Agile;
using ScopeX.Measure;

namespace ScopeX.Core.Tools
{
    internal class ScaleFactory
    {
        ConcurrentDictionary<KeyValuePair<Double[], Int32>, Double> _ScaleDictionary = new ConcurrentDictionary<KeyValuePair<Double[], Int32>, Double>(new ScaleKeyComparer());
        public ScaleFactory(Double gain, Double[] scale, Int32[] ticks)
        {
            _Scales = scale;
            _Ticks = ticks;
            _Gain = gain;
        }

        private readonly Double[] _Scales;
        private readonly Double _Gain;
        private readonly Int32[] _Ticks;

        public Double GetScale(Int32 index, Double initvalue)
        {
            Double scale;
            Double[] scales = _Scales;
            var tmp = initvalue;
            var gain = 0;
            KeyValuePair<Double[], Int32> scalesIndexPair = new KeyValuePair<Double[], Int32>(scales, index);
            //return 10000d;
            if (_ScaleDictionary.TryGetValue(scalesIndexPair, out scale))
            {
                return scale;
            }
            else
            {
                if (index < 0)
                {
                    gain = -(1 + index) / scales.Length + 1;
                    scale = scales[index + scales.Length * gain];
                    scale /= Math.Pow(_Gain, gain);
                }
                else
                {
                    scale = scales[index % scales.Length];

                    gain = index / scales.Length;
                    scale *= Math.Pow(_Gain, gain);
                }
                scalesIndexPair = new KeyValuePair<Double[], Int32>(scalesIndexPair.Key, index);
                _ScaleDictionary[scalesIndexPair] = scale;
                return scale;
            }
        }

        public Double GetScale(Int32 index, Int32 tick, Double initvalue)
        {
            Double step;
            if (tick >= 0)
                step = GetScale(index, initvalue);
            else
                step = GetScale(index - 1, initvalue);

            return GetScale(index, initvalue) + step / 100.0 * tick;
        }

        public Int32 GetPosTicks(Int32 index)
        {
            if (index < 0)
                return 0;

            return _Ticks[index % _Ticks.Length];
        }

        public Int32 GetNegTicks(Int32 index)
        {
            if (index <= 0)
                return 0;

            return -_Ticks[(index - 1) % _Ticks.Length];
        }

        public (Int32 Index, Int32 Tick) TryGetScaleIndex(Double scale, Double initvalue)
        {
            var (index, tick) = TrySetScale(scale);

            if (tick != 0)
            {
                var nextscale = GetScale(index + 1, initvalue);
                var curscale = GetScale(index, initvalue);
                var lastscale = GetScale(index - 1, initvalue);

                var delta = new Double[] { Math.Abs(scale - nextscale), Math.Abs(scale - curscale), Math.Abs(scale - lastscale) };

                switch (delta.FirstIndex(x => x == delta.Min()))
                {
                    case 0:
                        index++;
                        tick = (Int32)(scale - nextscale);
                        break;
                    case 2:
                        index--;
                        tick = (Int32)(scale - lastscale);
                        break;
                    default:
                    case 1:
                        tick = (Int32)(scale - curscale);
                        break;
                }
            }

            return (index, tick);
        }

        public (Int32 Index, Int32 Tick) TrySetScale(Double scale)
        {
            //Int32 exp = (Int32)Math.Truncate(Math.Log10(scale) / Math.Log10(_Gain));

            Int32 exponent = 0;
            decimal quotient = (decimal)scale;
            if (quotient != 0)
            {
                if (scale > _Scales[0])
                {
                    while (quotient >= (decimal)_Scales[0] * (decimal)_Gain)
                    {
                        quotient /= (decimal)_Gain;
                        exponent++;
                    }
                }
                else 
                {
                    while (quotient < (decimal)_Scales[0])
                    {
                        quotient *= (decimal)_Gain;
                        exponent--;
                    }
                } 
            }


            Int32 index = Array.BinarySearch(_Scales, (Double)quotient);
            if (index < 0)
            {
                index = -index - 1;
                decimal start, end;
                Int32 tick;
                if (index == _Scales.Length)
                {
                    start = (decimal)_Scales[^1];
                    end = (decimal)_Scales[0] * (decimal)_Gain;
                    tick = (Int32)Math.Round((quotient - end) / (end - start) * _Ticks[^1], MidpointRounding.AwayFromZero);
                }
                else if (index == 0)
                {
                    start = (decimal)_Scales[^1] / (decimal)_Gain;
                    end = (decimal)_Scales[0];
                    tick = (Int32)Math.Round((quotient - end) / (end - start) * _Ticks[^1], MidpointRounding.AwayFromZero);
                }
                else
                {
                    start = (decimal)_Scales[index - 1];
                    end = (decimal)_Scales[index];
                    tick = (Int32)Math.Round((quotient - end) / (end - start) * _Ticks[index - 1], MidpointRounding.AwayFromZero);
                }

                return (index + exponent * _Scales.Length, tick);
            }
            else
                return (index + exponent * _Scales.Length, 0);
        }

        public class ScaleKeyComparer : IEqualityComparer<KeyValuePair<Double[], Int32>>
        {
            public Boolean Equals(KeyValuePair<Double[], Int32> x, KeyValuePair<Double[], Int32> y)
            {
                return x.Value == y.Value && Enumerable.SequenceEqual(x.Key, y.Key);
            }

            public Int32 GetHashCode(KeyValuePair<Double[], Int32> obj)
            {
                unchecked
                {
                    Int32 hash = 17;
                    hash = hash * 23 + obj.Value.GetHashCode();
                    foreach (var element in obj.Key)
                    {
                        hash = hash * 23 + element.GetHashCode();
                    }
                    return hash;
                }
            }
        }

        public static readonly ScaleFactory Default = new(10, new[] { 1, 2, 5.0 }, new[] { 100, 100, 150 });
    }
}

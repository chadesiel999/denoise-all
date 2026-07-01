using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Core.Tools;
using ScopeX.Measure;

namespace ScopeX.Core
{
    public class JitterItems
    {
        private Double _Current;

        public Double Current
        {
            get => _Current;

            set
            {
                _Current = value;
                if (!Double.IsNaN(value))
                {
                    StaBuffer.Insert(value);
                }
            }
        }

        public Double? Max => StaBuffer.Max;

        public Double? Min => StaBuffer.Min;

        public Double? Count => StaBuffer.Count;

        public readonly StatisticBuffer StaBuffer;

        public JitterItems(Int32 size = 1000)
        {
            StaBuffer = new(size);
        }
    };

    public record JitterItem(Double? Value, QuantityUnit Unit);
}

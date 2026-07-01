using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core.Tools
{
    public static class IndexValuePair
    {
        public static List<Double> ValueToIndex(Double scale, Double idx0, List<Double> values)
        {
            return values.ConvertAll((o) => o / scale * Constants.IDX_PER_YDIV + idx0);
        }

        public static List<Double> IndexToValue(Double scale, Double idx0, List<Double> index)
        {
            return index.ConvertAll((o) => (o - idx0) / Constants.IDX_PER_YDIV * scale);
        }

        public static List<Double> IndexToValue(Double scale, Double idx0, List<Int32> index)
        {
            return index.ConvertAll((o) => (o - idx0) / Constants.IDX_PER_YDIV * scale);
        }

        private static List<Double> TimeToIndex(Double tmb, Double idx0, List<Double> values)
        {
            return values.ConvertAll((o) => o / tmb * Constants.IDX_PER_XDIV + idx0);
        }

        private static List<Double> IndexToTime(Double tmb, List<Double> index)
        {
            return index.ConvertAll((o) => o / Constants.IDX_PER_XDIV * tmb);
        }

        private static List<Double> IndexToTime(Double tmb, Double idx0, List<Double> index)
        {
            return index.ConvertAll((o) => (o - idx0) / Constants.IDX_PER_XDIV * tmb);
        }
    }
}

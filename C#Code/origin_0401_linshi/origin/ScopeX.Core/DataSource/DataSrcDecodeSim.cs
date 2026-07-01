using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.Core.Decode;
using ScopeX.ComModel;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    internal class DataSrcDecodeSim : IDataSource
    {
        public SerialProtocolType ProtocolType
        {
            get;
            set;
        }

        private sealed record Context(WfmProperties Properties, CancellationToken CancelToken);

        public Object? Prepare(Boolean init, ChannelId aid, CancellationToken ct, DataRole dataRole, CancellationToken? softResetToken)
        {
            var dch = (DecodeModel)DsoModel.Default.GetChannel(ChannelId.B1);

            var prop = new WfmProperties(dch.Name)
            {
                ChnlPosition = (0, 0),
                ChnlScale = (0, 0),
                ChnlUnit = (dch.Conditioning.Prefix, dch.Conditioning.Unit),

                TmbPosition = (dch.Sampling.PosIndex, dch.Sampling.Position),
                TmbScale = ((Int32)dch.Sampling.ScaleIndex, dch.Sampling.Scale),
                TmbUnit = (dch.Sampling.Prefix, dch.Sampling.Unit),

                //VuFactor = 0.01,
            };
            return new Context(prop, ct);
        }

        public (Double[,], Object)? Read(Object? param)
        {
            IEnumerable<Double> y;
            if (param is not Context context)
                return null;

            switch (ProtocolType)
            {
                case SerialProtocolType.RS232:
                    y = SerialProtocolGenerator.CreateRS232Sequence(1024 * 32 * 4).Select(o => (Double)o);
                    break;
                case SerialProtocolType.I2C:
                    y = SerialProtocolGenerator.CreateI2CSequence(1024 * 32 * 4).Select(o => (Double)o);
                    break;
                default:
                    return (new Double[0, 0], context.Properties);
            }

            return (y.ToMatrix(1, y.Count()), context.Properties);
        }

        public WfmPack Process((Double[,] Buffer, Object Prop) pkg, Object? arg)
        {
            return new WfmPack(pkg.Buffer, 0, pkg.Buffer.GetLength(1), (WfmProperties)pkg.Prop);
        }

        public DataSrcDecodeSim(SerialProtocolType protocolType)
        {
            ProtocolType = protocolType;
        }
    }
}

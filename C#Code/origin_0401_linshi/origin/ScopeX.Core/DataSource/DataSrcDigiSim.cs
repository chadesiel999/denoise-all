using System;
using System.Linq;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    internal class DataSrcDigiSim : IDataSource
    {
        private sealed record Context(WfmProperties Properties, CancellationToken CancelToken);

        private const Int32 _LENGTH = 1000;

        private const Double _MIN_SAMPINTVAL = 1E-9;

        public Object? Prepare(Boolean init, ChannelId aid, CancellationToken ct, DataRole dataRole, CancellationToken? softResetToken)
        {
            var dch = (DigitalModel)DsoModel.Default.GetChannel(ChannelId.D0);

            var si = dch.Sampling.Scale * 1E-6 * Constants.VIS_XDIVS_NUM / _LENGTH;
            if (si < _MIN_SAMPINTVAL)
            {
                si = _MIN_SAMPINTVAL;
            }

            var prop = new WfmProperties(dch.Name)
            {
                ChnlPosition = (0, 0),
                ChnlScale = (0, 0),
                ChnlUnit = (dch.Conditioning.Prefix, dch.Conditioning.Unit),

                TmbPosition = (dch.Sampling.PosIndex, dch.Sampling.Position),
                TmbScale = ((Int32)dch.Sampling.ScaleIndex, dch.Sampling.Scale),
                TmbUnit = (dch.Sampling.Prefix, dch.Sampling.Unit),

                SampInterval = si,
            };

            return new Context(prop, ct);
        }

        public (Double[,], Object)? Read(Object? arg)
        {
            var y = Generator.RndBit32Seq(_LENGTH * ChannelIdExt.DigiChnlNum / 16).Select(o => (Double)o).ToList();
            if (arg is Context ctx && y!=null)
            {
                Double[,] tempbuffer = new Double[ChannelIdExt.DigiChnlNum / 16,y.Count / (ChannelIdExt.DigiChnlNum / 16)];
                for (Int32 index = 0; index < y.Count; index++)
                {
                    tempbuffer[index % (ChannelIdExt.DigiChnlNum / 16), index / (ChannelIdExt.DigiChnlNum / 16)] =y[index];
                }
                return (tempbuffer, ctx.Properties);
            }
            //!!!For compatibility, cast Int32 to Double 
            //var y = Generator.RndBit32Seq(_LENGTH * ChannelIdExt.DigiChnlNum / 16).Select(o => (Double)o);
            //if (arg is Context ctx)
            //{
                //return (y.ToMatrix(_LENGTH, y.Count() / _LENGTH), ctx.Properties);
            //}

            return null;
        }

        public WfmPack Process((Double[,] Buffer, Object Prop) pkg, Object? arg)
        {
            return new WfmPack(pkg.Buffer, 0, pkg.Buffer.GetLength(1), (WfmProperties)pkg.Prop);
            //return new WfmPack(pkg.Buffer, 0, pkg.Buffer.GetLength(0), (WfmProperties)pkg.Prop);
        }
    }
}

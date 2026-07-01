using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    internal class DataSrcDigi : IDataSource
    {
        private sealed record Context(IEnumerable<Double> LastBuffer, WfmProperties Properties, List<UInt16> Buffer, Int32 BitLength, CancellationToken CancelToken);

        public Object? Prepare(Boolean init, ChannelId aid, CancellationToken ct, DataRole dataRole, CancellationToken? softResetToken)
        {
            if (TriggerModel.State == SysState.Stop) 
            {
                //!!! 2024/03/20 HC:暂停态下LA不读数据
                //return null;            
            }
            if (Hd.LA is not null &&　Hd.LA.TryTakeWave(out var buffer, out var si))
            {
                var dch = (DigitalModel)DsoModel.Default.GetChannel(aid);
                dch.LastBuffer = dch.Pack?.Buffer?.ToEnumerable() ?? Enumerable.Empty<Double>();

                var wfmtimeposition = si.HdMessage?.Timebase?.TmbPosition ?? dch.Sampling.Position /*- si.StartTimeByus*/;
                var wfmtmbposindex = dch.Sampling.CalcPosIndex(wfmtimeposition, si.HdMessage?.Timebase?.TmbScale ?? dch.Sampling.Scale);
                var prop = new WfmProperties(dch.Name)
                {
                    ChnlPosition = (0, 0),
                    ChnlScale = (0, 0),
                    ChnlUnit = (dch.Conditioning.Prefix, dch.Conditioning.Unit),

                    TmbPosition = (wfmtmbposindex, wfmtimeposition),
                    TmbScale = (si.HdMessage?.Timebase?.TmbScaleIndex ?? (Int32)dch.Sampling.ScaleIndex, si.HdMessage?.Timebase?.TmbScale ?? dch.Sampling.Scale),
                    TmbUnit = (dch.Sampling.Prefix, dch.Sampling.Unit),

                    SampInterval = si.SampleIntervalByus * 1E-6,
                    VuStartIndex = (-si.StartTimeByus) / (si.HdMessage?.Timebase?.TmbScale ?? dch.Sampling.Scale) * Constants.IDX_PER_XDIV,
                    //VuFactor = si.SampleIntervalByus * Constants.IDX_PER_XDIV / dch.Sampling.ScaleByus,
                };

                return new Context(dch.LastBuffer, prop, buffer, dch.Conditioning.Bits.Count, ct);
            }

            return null;
        }

        public (Double[,], Object)? Read(Object? args)
        {
            if (args is not Context ctx)
            {
                return null;
            }
            Double[,] tempbuffer = new Double[ctx.BitLength / 16, ctx.Buffer.Count / (ctx.BitLength / 16)];
            for(Int32 index =0;index<ctx.Buffer.Count;index++)
            {
                tempbuffer[index%(ctx.BitLength/16),index/(ctx.BitLength/16)] = ctx.Buffer[index];
            }

            return (tempbuffer, ctx.Properties);

            //!!!For compatibility, cast Int32 to Double 
            //!!!Matrix row: b0~b15 b16~b31 b32~b47
            //return (ctx.Buffer.Select((b16) => (Double)b16).ToMatrix(ctx.Buffer.Count / (ctx.BitLength / 16), ctx.BitLength / 16), ctx.Properties);
        }

        public WfmPack Process((Double[,] Buffer, Object Prop) pkg, Object? args)
        {
            var ctx = (Context)args!;
            if (Dispatcher.IsScan && Dispatcher.IsRunning)
            {
                var length = (Int32)Math.Round(ctx.Properties.TmbScale.Value * 1E-6 * Constants.VIS_XDIVS_NUM / ctx.Properties.SampInterval, MidpointRounding.AwayFromZero);
                var buffer = ctx.LastBuffer.Concat(pkg.Buffer.ToEnumerable()).TakeLast(length);
                pkg.Buffer = buffer.ToMatrix(1, buffer.Count());


                var start = (length - pkg.Buffer.GetLength(1)) * Constants.MAX_XPOS_IDX / length;
                ctx.Properties.VuStartIndex = start;
            }
            return new WfmPack(pkg.Buffer, 0, pkg.Buffer.GetLength(1), (WfmProperties)pkg.Prop);
        }
    }
}

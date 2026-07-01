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
    internal class AuxDataSrcDPX
    {
        //private sealed record Context(List<List<UInt16>> Buffer, ChannelId Id, CancellationToken CancelToken);

        //public Object? Prepare(Boolean init, ChannelId aid, CancellationToken ct)
        //{
            

        //    if (init)
        //    {
        //        ;
        //    }

        //    return new Context(buffer, aid, ct);
        //}
        public Byte[]? Read(out WfmSampleInfo wfmSampleInfo,out List<ChannelId> includeChannels,  out UInt32 MainWinMaxHitTimes, out UInt32 MainWinMinHitTimes, out Double MainWinRadioOfSoftWaveSampleDivDpxWaveSample)
        {
            MainWinRadioOfSoftWaveSampleDivDpxWaveSample = 1.0;
            MainWinMaxHitTimes = 0;
            MainWinMinHitTimes= 0;
            wfmSampleInfo = new WfmSampleInfo();
            includeChannels = new List<ChannelId>();
            if (Hd.Dpx is not null && Hd.Dpx.TryTakeWave(out var buffer,out wfmSampleInfo,out includeChannels, out  MainWinMaxHitTimes, out  MainWinMinHitTimes, out UInt32 SubWinMaxHitTimes, out UInt32 SubWinMinHitTimes, out  MainWinRadioOfSoftWaveSampleDivDpxWaveSample, out Double SubWinRadioOfSoftWaveSampleDivDpxWaveSample))
                return buffer;

            return null;
        }

        //public WfmPack Process(Double[,] Buffer, Object? args)
        //{
        //    var ctx = (Context)args!;

        //    //for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
        //    //{
        //    //    for (Int32 j = 0; j < pkg.Buffer.GetLength(1); j++)
        //    //    {
        //    //        if (pkg.Buffer[i, j] > Constants.MAX_ADC_RES)
        //    //            pkg.Buffer[i, j] = Constants.MAX_ADC_RES;
        //    //        else if (pkg.Buffer[i, j] < 0)
        //    //            pkg.Buffer[i, j] = 0;

        //    //        pkg.Buffer[i, j] = (pkg.Buffer[i, j] - ctx.Pos0ByAdc) / Constants.SAMPS_PER_YDIV * ctx.Properties.ChnlScale.Value;
        //    //    }
        //    //}

        //    return new WfmPack(pkg.Buffer, 0, pkg.Buffer.GetLength(1), (WfmProperties)pkg.Prop);
        //}
    }
}

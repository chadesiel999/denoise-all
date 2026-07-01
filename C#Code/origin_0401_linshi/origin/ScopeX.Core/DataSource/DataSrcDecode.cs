using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.Hardware.Driver;
using ScopeX.MathExt;

namespace ScopeX.Core
{
    class DataSrcDecode:IDataSource
    {
        private sealed record Parameter(WfmProperties Properties, DecoderPackage package,  CancellationToken CancelToken);

        public Object? Prepare(Boolean init, ChannelId aid, CancellationToken ct, DataRole dataRole, CancellationToken? softResetToken)
        {
            //if (Hd.Decoder is not null && Hd.Decoder.TryTakeData((Int32)aid, out var packages, out var si))
            //{

            //    var ach = (Decode.DecodeModel)DsoModel.Default.GetChannel(aid);
            //    var prop = new WfmProperties(ach.Name)
            //    {
            //        ChnlPosition = (ach.Conditioning.PosIndex, ach.Conditioning.Position),
            //        ChnlScale = ((Int32)ach.Conditioning.ScaleIndex, ach.Conditioning.Scale),
            //        ChnlUnit = (ach.Conditioning.Prefix, ach.Conditioning.Unit),

            //        TmbPosition = (ach.Sampling.PosIndex, ach.Sampling.Position),
            //        TmbScale = ((Int32)ach.Sampling.ScaleIndex, ach.Sampling.Scale),
            //        TmbUnit = (ach.Sampling.Prefix, ach.Sampling.Unit),

            //        //VuFactor = ach.Sampling.ScaleByus * Constants.VIS_XDIVS_NUM /*/ (si.SampleIntervalByps / 1000_000.0 * buffer.Count)*/,
            //    };

            //    var pos0 = ach.Conditioning.PosIndex / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
            //    return new Parameter(prop, packages!, ct);
            //}
            return null;
        }

        public (Double[,], Object)? Read(Object? param)
        {
            if (param is not Parameter pm)
            {
                return null;
            }
            else
            {

            }

            if (pm.package == null || pm.package.SourceData.Length <=1)
            {
                return null;
            }
            else
            {

            }

            List<Double> tempdata = new List<Double>();
            if((pm.package.SourceData[0] & 0xFF) ==0xA5)//现在FPGA代码有bug，暂时在软件中处理
            {
                for(Int32 index =0;index<pm.package.SourceData.Length-1;index++)
                {
                    tempdata.Add(pm.package.SourceData[index] << 8 | pm.package.SourceData[index + 1] >> 8);
                }
            }
            else
            {
                tempdata.AddRange(pm.package.SourceData.Select(x => (Double)x));
            }

            Double[,] dst = new Double[1, tempdata.Count];
            Buffer.BlockCopy(tempdata.ToArray(), 0, dst, 0, dst.Length * sizeof(Double));
            return (dst,pm.Properties);
        }

        public WfmPack Process((Double[,] Buffer, Object Prop) pkg, Object? arg)
        {
            return new WfmPack(pkg.Buffer, 0, pkg.Buffer.GetLength(1), (WfmProperties)pkg.Prop);
        }

        public DataSrcDecode()
        {
          
        }
    }
}

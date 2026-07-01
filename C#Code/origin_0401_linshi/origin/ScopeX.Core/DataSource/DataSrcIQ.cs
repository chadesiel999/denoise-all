using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.Hardware;
using ScopeX.MathExt;
using ScopeX.Hardware.Driver;
using System.Numerics;

namespace ScopeX.Core
{
    internal class DataSrcIQ : IRFDataSource
    {
        private sealed record Parameter(WfmProperties Properties, List<Double> BufferI, List<Double> BufferQ,MDVirticalType VirticalType, CancellationToken CancelToken)
        {
        };

        public Object? Prepare(Boolean init, ChannelId rfid, CancellationToken ct, DataRole dataRole, CancellationToken? softResetToken)
        {
            
            var rfch = DsoModel.Default.GetChannel(rfid);
            _ = Hd.RF!.TryTakeIQWave(out var bufferFFTI, out var bufferFFTQ, out var sampleInterval);

            var prop = new RFWfmProperties(rfch.Name)
            {
                ChnlUnit = (rfch.Conditioning.Prefix, rfch.Conditioning.Unit),
                TmbUnit = (rfch.Sampling.Prefix, rfch.Sampling.Unit),

                SampInterval = sampleInterval,
            };

            return new Parameter(prop, bufferFFTI, bufferFFTQ, GetVirticalType(rfid), ct) { };
            //return new Object();
        }

        public (Double[,],Object)? Read(Object? param)
        {
            var pm = (Parameter)param!;
            List<Double> buffer = new List<Double>();
            switch (pm.VirticalType)
            {
                case MDVirticalType.Phase:
                    IQToPhase(pm.BufferI,pm.BufferQ,buffer);
                    break;
                case MDVirticalType.Amplitude:
                    IQToAmplitude(pm.BufferI, pm.BufferQ, buffer);
                    break;
                case MDVirticalType.Frequency:
                    PhaseToFrequency(buffer, pm.Properties.SampInterval);
                    break;
                default:
                    break;
            }

            return (buffer.ToMatrix(1, pm.BufferI.Count),
                pm.Properties);
        }

        public WfmPack Process((Double[,] Buffer, Object Prop) pkg, Object? param)
        {
            var pm = (Parameter)param!;
            
            return new WfmPack(pkg.Buffer, 0, pkg.Buffer.GetLength(1), (WfmProperties)pkg.Prop);
        }

        public DataSrcIQ()
        {
        }

        public WfmPack ProcessNormal((Double[,] Buffer, Object Prop) pkg, Object? context)
        {
            Double[,] buffer = (Double[,])pkg.Buffer.Clone();
            var ctx = (Parameter)context!;
            var pos0 = ctx.Properties.ChnlPosition.Index / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
            //for (Int32 i = 0; i < buffer.GetLength(0); i++)
            //    for (Int32 j = 0; j < buffer.GetLength(1); j++)
            //    {
            //        buffer[i, j] = (buffer[i, j] - pos0) / Constants.SAMPS_PER_YDIV * ctx.Properties.ChnlScale.Value;
            //    }
            return new WfmPack(buffer, 0, buffer.GetLength(1), ctx.Properties);
        }

        WfmPack IRFDataSource.ProcessMaxHold((Double[,] Buffer, Object Prop) pkg, Object? context)
        {
            throw new NotImplementedException();
        }

        WfmPack IRFDataSource.ProcessMinHold((Double[,] Buffer, Object Prop) pkg, Object? context)
        {
            throw new NotImplementedException();
        }

        WfmPack IRFDataSource.ProcessAverage((Double[,] Buffer, Object Prop) pkg, Object? context)
        {
            throw new NotImplementedException();
        }

        void IRFDataSource.Init()
        {
            throw new NotImplementedException();
        }

        private MDVirticalType GetVirticalType(ChannelId id)
        {
            if (id.IsPhaseVSTime() || id.IsPhaseVSFrequency() || id.IsPhaseGroupDelay())
            {
                return MDVirticalType.Phase;
            }
            else if (id.IsAmpVSTime())
            {
                return MDVirticalType.Amplitude;
            }
            else if (id.IsFrequencyVSTime())
            {
                return MDVirticalType.Frequency;
            }
            else if (id.IsTimeVSFrequency())
            {
                return MDVirticalType.Time;
            }
            else
            {
                return MDVirticalType.Amplitude;
            }
        }
        private Boolean IQToAmplitude(List<Double> bufferI, List<Double> bufferQ, List<Double> result)
        {
            if (bufferI.Count != bufferQ.Count)
                return false;
            result = new List<Double>();
            for (Int32 i = 0; i < bufferI.Count; i++)
            {
                Complex complex = new Complex(bufferI[i], bufferQ[i]);
                result.Add(complex.Magnitude);
            }
            return true;
        }
        
        private Boolean IQToPhase(List<Double> bufferI, List<Double> bufferQ,List<Double> result)
        {
            if (bufferI.Count != bufferQ.Count)
                return false;
            result = new List<Double>();
            for (Int32 i = 0; i < bufferI.Count; i++)
            {
                var phase = GetPhase(bufferI[i], bufferQ[i]);
                result.Add(phase);
            }
            return true;
        }
        private Boolean PhaseToFrequency(List<Double> result,Double sampleInterval)
        {
            if (result.Count < 2)
                return false;
            var frequency = new List<Double>();
            Double pre = 0;
            for (Int32 i = 0; i < result.Count; i++)
            {
                var freq =(result[i] - pre) /sampleInterval ;
                pre = result[i];
                frequency.Add(freq);
            }
            result = frequency;
            return true;
        }

        private Double GetPhase(Double i, Double q)
        {
            Double phase = 0;
            if (i > 0 && q > 0)
                phase = Math.Atan(q / i) / Math.PI * 180;
            else if (i > 0 && q < 0)
                phase = Math.Atan(q / i) / Math.PI * 180;
            else if (i < 0 && q > 0)
                phase = Math.Atan(q / i) / Math.PI * 180 + 180;
            else if (i < 0 && q < 0)
                phase = Math.Atan(q / i) / Math.PI * 180 - 180;
            return phase;
        }
    }
}

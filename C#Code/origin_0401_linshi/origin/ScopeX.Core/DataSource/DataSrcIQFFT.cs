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
    internal class DataSrcIQFFT : IRFDataSource
    {
        private sealed record Parameter(WfmProperties Properties, List<Double> BufferI, List<Double> BufferQ, MDVirticalType VirticalType, CancellationToken CancelToken)
        {
            public Int32 AverageTimes = 1000;
            public Int32 STFTTimes = 1;
        };

        public Object? Prepare(Boolean init, ChannelId rfid, CancellationToken ct, DataRole dataRole, CancellationToken? softResetToken)
        {
            return null;
            var rfch = /*(RadioFrequencyModel)*/DsoModel.Default.GetChannel(rfid);
            _ = Hd.RF!.TryTakeIQFFTWave(out var bufferFFTI, out var bufferFFTQ, out var centerFreq, out var RBW);

            var prop = new RFWfmProperties(rfch.Name)
            {
                ChnlUnit = (rfch.Conditioning.Prefix, rfch.Conditioning.Unit),
                TmbUnit = (rfch.Sampling.Prefix, rfch.Sampling.Unit),

                //ReferenceLevel = rfch.Conditioning.RefLevelValue,

                CenterFrequency = (Int64)centerFreq,
                RBW = RBW,
            };



            return new Parameter(prop, bufferFFTI, bufferFFTQ, GetVirticalType(rfid), ct)
            {
                STFTTimes =1//= (Int32)((rfch.STFTLength < rfch.Sampling.FFTLength) ? 1 : ((rfch.STFTLength - rfch.Sampling.FFTLength) / rfch.STFTStep) + 1)
            };
            //return new Object();
        }

        public (Double[,], Object)? Read(Object? param)
        {
            var pm = (Parameter)param!;
            List<Double> buffer = new List<Double>();
            switch (pm.VirticalType)
            {
                case MDVirticalType.Phase:
                    IQToPhase(pm.BufferI, pm.BufferQ, buffer);
                    break;
                case MDVirticalType.Amplitude:
                    IQToAmplitude(pm.BufferI, pm.BufferQ, buffer);
                    break;
                //case MDVirticalType.Time:
                //    PhaseToFrequency(buffer, pm.Properties.SampInterval);
                //    break;
                default:
                    break;
            }

            return (buffer.ToMatrix(pm.STFTTimes, pm.BufferI.Count / pm.STFTTimes),
                pm.Properties);
        }

        public WfmPack Process((Double[,] Buffer, Object Prop) pkg, Object? param)
        {
            var pm = (Parameter)param!;

            return new WfmPack(pkg.Buffer, 0, pkg.Buffer.GetLength(1), (WfmProperties)pkg.Prop);
        }

        public DataSrcIQFFT()
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

        public WfmPack ProcessAverage((Double[,] Buffer, Object Prop) pkg, Object? context)
        {
            Double[,] buffer = (Double[,])pkg.Buffer.Clone();
            var ctx = (Parameter)context!;
            var pos0 = ctx.Properties.ChnlPosition.Index / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
            Int32 num = _WfmNums;
            if (_AverageBuffer.Length != buffer.GetLength(1))
            {
                Init(buffer);
                _Restart = true;
            }
            if (_Restart)
            {
                Init(buffer);
                for (Int32 i = 0; i < buffer.GetLength(0); i++)
                    for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        _AverageBuffer[j] = buffer[i, j];
                num = 1;
                _RestartAverage = false;
            }
            else
            {
                lock (_AverageBuffer.SyncRoot)
                {
                    Int32 maxaveragenum = ctx.AverageTimes;
                    if (num == 0)
                        num++;
                    else if (num < maxaveragenum)
                    {
                        for (Int32 i = 0; i < buffer.GetLength(0); i++)
                            for (Int32 j = 0; j < buffer.GetLength(1); j++)
                                _AverageBuffer[j] += buffer[i, j];
                        num++;
                    }
                    else
                    {
                        for (Int32 i = 0; i < buffer.GetLength(0); i++)
                            for (Int32 j = 0; j < buffer.GetLength(1); j++)
                            {
                                _AverageBuffer[j] *= num - 1;
                                _AverageBuffer[j] /= num;
                                _AverageBuffer[j] += buffer[i, j];
                            }
                    }
                }
            }
            for (Int32 i = 0; i < buffer.GetLength(0); i++)
                for (Int32 j = 0; j < buffer.GetLength(1); j++)
                {
                    buffer[i, j] = _AverageBuffer[j] / num;
                    //buffer[i, j] = (buffer[i, j] - pos0) / Constants.SAMPS_PER_YDIV * ctx.Properties.ChnlScale.Value;
                }

            if (_WfmNums < ctx.AverageTimes)
                _WfmNums++;

            return new WfmPack(buffer, 0, buffer.GetLength(1), ctx.Properties);
        }
        public WfmPack ProcessMaxHold((Double[,] Buffer, Object Prop) pkg, Object? context)
        {
            Double[,] buffer = (Double[,])pkg.Buffer.Clone();
            var ctx = (Parameter)context!;
            var pos0 = ctx.Properties.ChnlPosition.Index / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
            if (_MaxHoldBuffer.Length != buffer.GetLength(1))
            {
                Init(buffer);
                _Restart = true;
            }
            if (_Restart)
            {
                Init(buffer);
                for (Int32 i = 0; i < buffer.GetLength(0); i++)
                    for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        _MaxHoldBuffer[j] = buffer[i, j];
                _RestartMax = false;
            }
            if (!_Restart && _RestartMax)
            {
                lock (_MaxHoldBuffer.SyncRoot)
                    for (Int32 i = 0; i < buffer.GetLength(0); i++)
                        for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        {
                            if (_MaxHoldBuffer[j] < buffer[i, j])
                                _MaxHoldBuffer[j] = buffer[i, j];
                            //buffer[i, j] = (_MaxHoldBuffer[j] - pos0) / Constants.SAMPS_PER_YDIV * ctx.Properties.ChnlScale.Value;
                        }
            }
            if (_RestartMax)
            {
                lock (_MaxHoldBuffer.SyncRoot)
                    for (Int32 i = 0; i < buffer.GetLength(0); i++)
                        for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        {
                            //if (_MaxHoldBuffer[j] < buffer[i, j])
                            //    _MaxHoldBuffer[j] = buffer[i, j];
                            //buffer[i, j] = (_MaxHoldBuffer[j] - pos0) / Constants.SAMPS_PER_YDIV * ctx.Properties.ChnlScale.Value;
                            buffer[i, j] = _MaxHoldBuffer[j];
                        }
            }
            else
            {
                lock (_MaxHoldBuffer.SyncRoot)
                    for (Int32 i = 0; i < buffer.GetLength(0); i++)
                        for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        {
                            //if (_MaxHoldBuffer[j] < buffer[i, j])
                            //    _MaxHoldBuffer[j] = buffer[i, j];
                            //buffer[i, j] = (buffer[i, j] - pos0) / Constants.SAMPS_PER_YDIV * ctx.Properties.ChnlScale.Value;
                            buffer[i, j] = _MaxHoldBuffer[j];
                        }
            }


            return new WfmPack(buffer, 0, buffer.GetLength(1), ctx.Properties);
        }
        public WfmPack ProcessMinHold((Double[,] Buffer, Object Prop) pkg, Object? context)
        {
            Double[,] buffer = (Double[,])pkg.Buffer.Clone();
            var ctx = (Parameter)context!;
            var pos0 = ctx.Properties.ChnlPosition.Index / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
            if (_MinHoldBuffer.Length != buffer.GetLength(1))
            {
                Init(buffer);
                _Restart = true;
            }
            if (_Restart)
            {
                Init(buffer);
                for (Int32 i = 0; i < buffer.GetLength(0); i++)
                    for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        _MinHoldBuffer[j] = buffer[i, j];
                _RestartMin = false;
            }
            if (_RestartMin)
            {
                lock (_MinHoldBuffer.SyncRoot)
                    for (Int32 i = 0; i < buffer.GetLength(0); i++)
                        for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        {
                            if (_MinHoldBuffer[j] > buffer[i, j])
                                _MinHoldBuffer[j] = buffer[i, j];

                            //buffer[i, j] = (_MinHoldBuffer[j] - pos0) / Constants.SAMPS_PER_YDIV * ctx.Properties.ChnlScale.Value;
                            buffer[i, j] = _MinHoldBuffer[j];
                        }
            }
            else
            {
                lock (_MinHoldBuffer.SyncRoot)
                    for (Int32 i = 0; i < buffer.GetLength(0); i++)
                        for (Int32 j = 0; j < buffer.GetLength(1); j++)
                        {
                            if (_MinHoldBuffer[j] > buffer[i, j])
                                _MinHoldBuffer[j] = buffer[i, j];

                            //buffer[i, j] = (buffer[i, j] - pos0) / Constants.SAMPS_PER_YDIV * ctx.Properties.ChnlScale.Value;
                            buffer[i, j] = _MinHoldBuffer[j];
                        }
            }

            return new WfmPack(buffer, 0, buffer.GetLength(1), ctx.Properties);
        }

        #region WaveformProcess

        private Double[] _AverageBuffer = new Double[10000];
        private Double[] _MaxHoldBuffer = new Double[10000];
        private Double[] _MinHoldBuffer = new Double[10000];
        private Boolean _RestartAverage = true;
        private Boolean _RestartMax = true;
        private Boolean _RestartMin = true;
        private Boolean _Restart = true;
        private Int32 _WfmNums;
        private readonly List<Double[]> _HistoryBuffer = new List<Double[]>();

        public void Init()
        {
            _Restart = true;
        }

        public void Init(Double[,] originalBuffer)
        {
            Double[] buffer = new Double[originalBuffer.GetLength(1)];
            for (Int32 i = 0; i < originalBuffer.GetLength(1); i++)
            {
                buffer[i] = originalBuffer[0, i];
            }
            _WfmNums = 0;

            _HistoryBuffer.Clear();
            lock (_AverageBuffer.SyncRoot)
            {
                lock (_MaxHoldBuffer.SyncRoot)
                {
                    lock (_MinHoldBuffer.SyncRoot)
                    {
                        _MaxHoldBuffer = (Double[])buffer.Clone();
                        _MinHoldBuffer = (Double[])buffer.Clone();
                        _AverageBuffer = (Double[])buffer.Clone();
                    }
                }
            }

            for (Int32 i = 0; i < _HistoryBuffer.Count; i++)
                Array.Clear(_HistoryBuffer[i], 0, _HistoryBuffer[i].Length);

            if (!_RestartMax && !_RestartMax && !_RestartMin)
            {
                _RestartAverage = true;
                _RestartMax = true;
                _RestartMin = true;
                _Restart = false;
            }
        }



        #endregion

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

        private Boolean IQToPhase(List<Double> bufferI, List<Double> bufferQ, List<Double> result)
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
        //private Boolean PhaseToFrequency(List<Double> result, Double sampleInterval)
        //{
        //    if (result.Count < 2)
        //        return false;
        //    var frequency = new List<Double>();
        //    Double pre = 0;
        //    for (Int32 i = 0; i < result.Count; i++)
        //    {
        //        var freq = (result[i] - pre) / sampleInterval;
        //        pre = result[i];
        //        frequency.Add(freq);
        //    }
        //    result = frequency;
        //    return true;
        //}

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

using ScopeX.ComModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.MathExt;
using System.Diagnostics;

namespace ScopeX.Core
{
    internal class DataSrcSim : IDataSource
    {
        protected ArbWfmGenModel Model
        {
            get;
        }

        public DataSrcSim(ArbWfmGenModel model, Double si, Int32 length)
        {
            Model = model;

            SampInterval = si;

            Length = length;

            _Averager = new(1, length);
            _Evlp = new(1, length);
        }

        private readonly Boolean _VariableSampleRate = true;

        private const Double _MIN_SAMPINTVAL = 100E-9;

        private Stopwatch _ScanTimer = new();

        private Double _LastPhase = 0;

        public Double SampInterval
        {
            get;
            set;
        }

        public Int32 Length
        {
            get;
            set;
        }

        private CohAverager _Averager
        {
            get;
            init;
        }

        private Envelope _Evlp
        {
            get;
            init;
        }

        private sealed record Context(IEnumerable<Double> LastBuffer, WfmProperties Properties, AnaChnlAcqMode AcqMode, Boolean Inverted, Boolean Active, WfmGenMode GenMode, ArbWfmType Type, Double Pos0, Double Amplitude, Double Cycles, Double Duty, Double Phase, Double Noise, CancellationToken CancelToken)
        {
            public WfmModMethod ModMethod
            {
                get;
                init;
            }

            public ArbWfmType ModulatedWfm
            {
                get;
                init;
            }

            public WfmRampType RampType
            {
                get;
                init;
            }

            public Double ModFreq
            {
                get;
                init;
            }

            public Int32 AmpDepth
            {
                get;
                init;
            }

            public Double FreqBias
            {
                get;
                init;
            }

            public EvlpOpt EnvelopeOpt
            {
                get;
                init;
            }

            public WfmPersist PersistType
            {
                get;
                init;
            }

            public PersistentModel<Double>? Persistent
            {
                get;
                init;
            }
        }

        private Double GetSamplInterval(Double time)
        {
            //var si = (ach.Sampling.ScaleByus * 1E-6 / ach.Sampling.PosIdxPerDiv);
            var si = time / Length;
            if (si < _MIN_SAMPINTVAL)
                si = _MIN_SAMPINTVAL;

            return si;
        }

        public Object? Prepare(Boolean init, ChannelId id, CancellationToken ct, DataRole dataRole, CancellationToken? softResetToken)
        {
            DataSrcFifo.TryTakeAdvancedFuncSource();
            var ach = (AnalogModel)DsoModel.Default.GetChannel(id);
            var persisittype = DsoModel.Default.Display.Persist;

            var si = SampInterval;
            if (_VariableSampleRate)
            {
                si = GetSamplInterval(ach.Sampling.Scale * 1E-6 * Constants.VIS_XDIVS_NUM);
            }

            var lastbuffer = ach.Pack?.Buffer?.ToEnumerable() ?? Enumerable.Empty<Double>();

            if (init)
            {
                _Averager.Reset();
                _Evlp.Reset();
                _ScanTimer = Stopwatch.StartNew();
                _LastPhase = 0;
                lastbuffer = Enumerable.Empty<Double>();
            }
            _Averager.MaxCnts = ach.Sampling.AverageCnt;
            _Evlp.MaxCnts = ach.Sampling.EnvelopeCnt;

            var prop = new WfmProperties(ach.Name)
            {
                ChnlPosition = (ach.Conditioning.PosIndex, ach.Conditioning.Position),
                ChnlScale = ((Int32)ach.Conditioning.ScaleIndex, ach.Conditioning.ScaleBymV),
                ChnlUnit = (ach.Conditioning.Prefix, ach.Conditioning.Unit),
                ChnlBias = ach.Conditioning.BiasByuV / 1000.0,

                TmbPosition = (ach.Sampling.PosIndex, ach.Sampling.Position),
                TmbScale = ((Int32)ach.Sampling.ScaleIndex, ach.Sampling.Scale),
                TmbUnit = (ach.Sampling.Prefix, ach.Sampling.Unit),

                SampInterval = si,

                //VuFactor = (ach.Sampling.ScaleByus / ach.Sampling.PosIdxPerDiv) / (SampInterval * 1E6),
            };

            var cycles = Length * si * (Model.Frequency * 1E-6);
            return new Context(lastbuffer, prop, ach.Sampling.Mode, ach.Conditioning.IsInverted,
                Model.Active, Model.Mode, Model.WfmType, Model.Offset * 1E-3, Model.Amplitude / 2.0 * 1E-3,
                cycles, Model.Duty / 100.0,
                //((Model.Phase / 100.0 + (Model.Opposition ? 180 : 0)) % 360) / 3.6, 
                Model.Phase / 360.0,
                Model.Noise / 100.0, ct)
            {
                ModMethod = Model.ModMethod,
                ModulatedWfm = Model.ModulatedWfm,
                RampType = Model.RampType,
                ModFreq = Model.ModFreq * 1E-6 * si,
                AmpDepth = Model.AmpDepth,
                FreqBias = Model.FreqBias * 1E-6 * si,
                EnvelopeOpt = ach.Sampling.EnvelopOpt,
                PersistType = persisittype,
                Persistent = ach.Persistent,
            };
        }

        public (Double[,], Object)? Read(Object? arg)
        {
            if (arg is not Context ctx)
            {
                return null;
            }

            IEnumerable<Double> y;

            if (!_ScanTimer.IsRunning)
            {
                _ScanTimer.Restart();
                return null;
            }

            Int32 segmentlen = Length;
            if (Dispatcher.IsScan)
            {
                segmentlen = (Int32)Math.Floor((Double)_ScanTimer.ElapsedTicks / Stopwatch.Frequency / ctx.Properties.SampInterval);

                if (segmentlen > Length||segmentlen<=0)
                    segmentlen = Length;
            }

            //var phofs = Math.Round(_LastPhase * (ctx.Properties.SampleIntByus * 1E-6) * (Model.Frequency * 1E-6) / 100.0, 2);
            if (ctx.Active)
            {
                switch (ctx.GenMode)
                {
                    case WfmGenMode.Continuous:
                        switch (ctx.Type)
                        {
                            case ArbWfmType.Sinusoid:
                                y = Generator.Sine(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, segmentlen, ctx.Noise / 100f, ctx.Phase / 100f + _LastPhase);
                                break;
                            case ArbWfmType.Square:
                                y = Generator.Rectangular(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, segmentlen, ctx.Noise / 100f, 0.5, ctx.Phase / 100f + _LastPhase);
                                break;
                            case ArbWfmType.Pulse:
                                y = Generator.Rectangular(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, segmentlen, ctx.Noise / 100f, ctx.Duty / 100f, ctx.Phase / 100f + _LastPhase);
                                break;
                            //<Remark>更改人：彭博 创建日期：2023/11/24 16:44:00  原因：技术手册改动，将锯齿波和三角波合为斜波 </Remark>
                            //case ArbWfmType.Triangular:
                            //    y = Generator.Triangular(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, segmentlen, ctx.Noise / 100f, 0.5, ctx.Phase / 100f + _LastPhase);
                            //    break;
                            case ArbWfmType.Ramp:
                                y = Generator.Triangular(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, segmentlen, ctx.Noise / 100f, ctx.Duty / 100f, ctx.Phase / 100f + _LastPhase);
                                break;
                            case ArbWfmType.Noise:
                                y = Generator.GaussianNarrowNoise(ctx.Pos0, ctx.Amplitude, segmentlen);
                                break;
                            case ArbWfmType.DC:
                                y = Generator.DirectCurrent(ctx.Pos0, ctx.Properties.ChnlScale.Value * 1E-3, segmentlen, ctx.Noise / 100f);
                                break;
                            case ArbWfmType.Sinc:
                                y = Generator.Sinc(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, segmentlen, ctx.Noise / 100f, 0.1/*ctx.Duty / 100f*/, ctx.Phase / 100f + _LastPhase);
                                break;
                            case ArbWfmType.ExpRise:
                                y = Generator.RiseExp(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, segmentlen, ctx.Noise / 100f, 0.2/*ctx.Duty / 100f*/, ctx.Phase / 100f + _LastPhase);
                                break;
                            case ArbWfmType.ExpFall:
                                y = Generator.DecayExp(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, segmentlen, ctx.Noise / 100f, 0.2/*ctx.Duty / 100f*/, ctx.Phase / 100f + _LastPhase);
                                break;
                            case ArbWfmType.Lorentz:
                                y = Generator.Lorentz(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, segmentlen, ctx.Noise / 100f, 0.2/*ctx.Duty / 100f*/, ctx.Phase / 100f + _LastPhase);
                                break;
                            case ArbWfmType.Haversine:
                                y = Generator.Haversine(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, segmentlen, ctx.Noise / 100f, ctx.Phase / 100f + _LastPhase);
                                break;
                            case ArbWfmType.Gaussian:
                                y = Generator.Gaussian(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, segmentlen, ctx.Noise / 100f, 0.2/*ctx.Duty / 100f*/, ctx.Phase / 100f + _LastPhase);
                                break;
                            case ArbWfmType.ECG:
                                y = Generator.ECG(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, segmentlen, ctx.Noise / 100f, 25, ctx.Phase / 100f + _LastPhase);
                                break;
                            case ArbWfmType.Arbitrary:
                                y = Generator.RingRect(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, ctx.Phase / 100f + _LastPhase, segmentlen, ctx.Noise / 100f, ctx.Duty / 100f, 0.08, 0.01, 0.1);
                                break;
                            default:
                                return null;
                        }
                        break;
                    case WfmGenMode.Modulation:
                        var rd = ctx.RampType switch
                        {
                            WfmRampType.Rise => 0.99,
                            WfmRampType.Fall => 0.01,
                            _ => 0.5,
                        };

                        switch (ctx.ModMethod)
                        {
                            case WfmModMethod.AM:
                                {
                                    //var xc = Length / ctx.Cycles;
                                    //var xm = 1 / (ctx.Properties.SampInterval * ctx.ModFreq * 1E-6);
                                    //y = Generator.AmpMod(ctx.Pos0, ctx.Amplitude, ctx.Cycles, ctx.Phase, Length, ctx.Noise,
                                    //    x => Math.Sin(2 * Math.PI * x / xc),
                                    //    x => Math.Sin(2 * Math.PI * x / xm),
                                    //    ctx.AmpDepth / 100.0);


                                    y = Generator.AmpMod(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, segmentlen, ctx.Noise / 100f, ctx.Duty / 100f, ctx.Phase / 100f + _LastPhase, ctx.Type,
                                        ctx.ModulatedWfm, rd, ctx.AmpDepth / 100.0, ctx.ModFreq);
                                }
                                break;

                            case WfmModMethod.FM:
                                {
                                    //var xc = 1 / (ctx.Properties.SampInterval * ctx.ModFreq * 1E-6);
                                    //var xm = Length / ctx.Cycles;
                                    //var xdev = 1 / (ctx.Properties.SampInterval * ctx.FreqBias * 1E-6);
                                    //y = Generator.FreqMod(ctx.Pos0, ctx.Amplitude, ctx.Cycles, ctx.Phase, Length, ctx.Noise,
                                    //    (x, m) => Math.Sin(2 * Math.PI * x / xc + m),
                                    //    (x) => Math.Cos(2 * Math.PI * x / xm),
                                    //    xm / xdev);

                                    y = Generator.FreqMod(ctx.Pos0, ctx.Amplitude, ctx.Cycles / Length, segmentlen, ctx.Noise / 100f, ctx.Duty / 100f, ctx.Phase / 100f + _LastPhase, ctx.Type,
                                        ctx.ModulatedWfm, rd, ctx.FreqBias, ctx.ModFreq);
                                }
                                break;

                            default:
                                //todo FSK
                                return null;
                        }
                        break;
                    case WfmGenMode.Sweep:
                        y = Generator.DirectCurrent(ctx.Pos0, ctx.Properties.ChnlScale.Value * 1E-3, segmentlen, 1 / 100f);
                        break;
                    default:
                        y = Generator.DirectCurrent(ctx.Pos0, ctx.Properties.ChnlScale.Value * 1E-3, segmentlen, 1 / 100f);
                        break;
                }
            }
            else
            {
                y = Generator.DirectCurrent(0, ctx.Properties.ChnlScale.Value * 1E-3, segmentlen, 1 / 100f);
            }

            y = y.Select(o => o * 1E3);
            if (Dispatcher.IsScan)
            {
                if (segmentlen < Length)
                    y = ctx.LastBuffer.Concat(y).TakeLast(Length);

                var phase = _LastPhase + segmentlen * ctx.Cycles / Length;
                _LastPhase = phase - Math.Truncate(phase);

                var start = (Length - y.Count()) * Constants.MAX_XPOS_IDX / (Double)Length;
                ctx.Properties.VuStartIndex = start;
            }
            _ScanTimer.Restart();

            //Convert physical signal to sampling points, like ADC
            var pos0 = ctx.Properties.ChnlPosition.Index / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
            var scale = Constants.SAMPS_PER_YDIV / ctx.Properties.ChnlScale.Value;
            Double[] temp = y.Select(o => (o - ctx.Properties.ChnlBias) * scale/*/ ctx.Properties.ChnlScale.Value * Constants.SAMPS_PER_YDIV */+ pos0).ToArray();
            Double[,] buffer = TakeWave(temp);
            return (buffer, ctx.Properties);
        }

        private static Double[,] TakeWave(Double[] temp)
        {
            if (DsoModel.Default.Timebase.SegmentActive)
            {
                Double[,] segmentwave = new Double[DsoModel.Default.Timebase.ChoseFrameIds.Count, temp.Length];
                for (Int32 i = 0; i < segmentwave.GetLength(0); i++)
                {
                    Buffer.BlockCopy(temp, 0, segmentwave, i * temp.Length * sizeof(Double), temp.Length * sizeof(Double));
                }
                return segmentwave;
            }
            Double[,] buffer = new Double[1, temp.Length];

            Buffer.BlockCopy(temp, 0, buffer, 0, buffer.Length * sizeof(Double));
            return buffer;
        }

        public WfmPack Process((Double[,] Buffer, Object Prop) pkg, Object? arg)
        {
            if (arg is Context ctx)
            {
                var pos0 = ctx.Properties.ChnlPosition.Index / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
                for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
                {
                    if (ctx.Inverted)
                    {
                        for (Int32 j = 0; j < pkg.Buffer.GetLength(1); j++)
                            pkg.Buffer[i, j] = pos0 * 2 - pkg.Buffer[i, j];
                    }

                    for (Int32 j = 0; j < pkg.Buffer.GetLength(1); j++)
                    {
                        if (pkg.Buffer[i, j] > Constants.MAX_ADC_RES - 1)
                            pkg.Buffer[i, j] = Constants.MAX_ADC_RES - 1;
                        else if (pkg.Buffer[i, j] < 0)
                            pkg.Buffer[i, j] = 0;

                        pkg.Buffer[i, j] = (pkg.Buffer[i, j] - pos0) / Constants.SAMPS_PER_YDIV * ctx.Properties.ChnlScale.Value + ctx.Properties.ChnlBias;
                    }

                    switch (ctx.AcqMode)
                    {
                        case AnaChnlAcqMode.Average:
                            pkg.Buffer = _Averager.Run(pkg.Buffer, 0);
                            break;
                        case AnaChnlAcqMode.Envelope:
                            pkg.Buffer = _Evlp.Run(pkg.Buffer, ctx.EnvelopeOpt);
                            break;
                    }

                    if (DsoModel.Default.Timebase.EnableRIS)
                    {

                    }
                }

                //if (DsoModel.Default.LocAssisted.Enabled)
                //{
                //    var index = DsoModel.Default.LocAssisted.Locate();
                //    if (index >= 0)
                //    {
                //        ctx.Properties.VuStartIndex = -index + ctx.Properties.TmbPosition.Index;
                //    }

                //}

                if (ctx.Persistent != null)
                {
                    //if (ctx.PersistType == WfmPersist.Auto)
                    //{
                    //    if (ctx.Persistent.AddFrames(pkg.Buffer, out Double[,]? alldata))
                    //        pkg.Buffer = alldata;
                    //}
                    //else
                        ctx.Persistent.Reset();
                }

            }

            return new WfmPack(pkg.Buffer, 0, pkg.Buffer.GetLength(1), (WfmProperties)pkg.Prop);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ScopeX.ComModel;

namespace ScopeX.Core
{
    public record WfmVuBlock(Double[,] Buffer, Double Start, Double ZoomRatio, Double DpxCorrection = 1.0);

    internal record WfmVuBaseParam(Double Scale, Double PosIndex, Double PosIdxPerDiv);

    public class WfmVuDatabase
    {

        private readonly List<WfmVuBlock> _Block;

        //private Int32 _WrIndex;

        //private Int32 _RdIndex;

        public WfmVuDatabase(Int32 size = 32)
        {
            _Block = new(size);
            //_WrIndex = 0;
            //_RdIndex = -1;
            Reset();
        }

        public void Reset()
        {
            NewIndex = 0;
            OldIndex = 0;
        }

        public Boolean IsEmpty => NewIndex == OldIndex;

        public Int32 OldIndex
        {
            get;
            private set;
        }

        public Int32 NewIndex
        {
            get;
            private set;
        }

        public Int32 Count => _Block.Count;

        public WfmVuBlock? Current =>
                //var ri = _RdIndex;
                //return ri < 0 ? null : _Block[ri];
                IsEmpty ? null : _Block[(NewIndex - 1 + _Block.Capacity) % _Block.Capacity];

        public ImmutableList<WfmVuBlock> All => _Block.ToImmutableList();
        public Boolean GetBlocks(ref List<Double> datas)
        {
            if (_Block == null)
            {
                return false;
            }
            datas = new List<Double>();

            foreach (var block in _Block)
            {
                Int32 index = 0;
                for (; index < block.Buffer.GetLength(1); index++)
                {
                    datas.Add(block.Buffer[0, index]);
                }
            }
            return true;
        }
        public Boolean GetBlock(Int32 index, ref Double[,] data)
        {
            if (_Block == null || index < 0 || index >= _Block.Count)
            {
                return false;
            }

            data = _Block[index].Buffer;
            return true;
        }
        public void Add(WfmVuBlock? block)
        {
            if (block is not null)
            {
                //if (_WrIndex >= _Block.Count)
                //    _Block.Add(block);
                //else
                //    _Block[_WrIndex] = block;
                //_RdIndex = _WrIndex;
                //_WrIndex = (_WrIndex + 1) % _Block.Capacity;

                if (NewIndex >= _Block.Count)
                {
                    _Block.Add(block);
                }
                else
                {
                    _Block[NewIndex] = block;
                }

                var ni = (NewIndex + 1) % _Block.Capacity;
                if (ni == OldIndex)
                {
                    OldIndex = (OldIndex + 1) % _Block.Capacity;
                }
                NewIndex = ni;
            }
        }

        private static Double ValidateVuSamples(Double y)
        {
            if (y > Constants.MAX_YPOS_IDX)
            {
                y = Constants.MAX_YPOS_IDX;
            }
            else if (y < Constants.MIN_YPOS_IDX)
            {
                y = Constants.MIN_YPOS_IDX;
            }

            return y;
        }

        internal static Double scale;

        internal static (WfmVuBlock?, WfmVuBlock?)? Rescale(ChannelModel cm, Int32 length, WfmVuBaseParam? wfmVuBaseParam)
        {
            var wfmvublock = WareRescale(cm, ref length, wfmVuBaseParam);

            if (DsoModel.Default.Timebase.IsZoom)
            {
                Int32 zoomlength = 0;
                var zoomwfmvublock = ZoomWareRescale(cm, ref zoomlength, wfmVuBaseParam, wfmvublock);
                return (wfmvublock, zoomwfmvublock);
            }
            return (wfmvublock, null);
        }
        private static WfmVuBlock? ZoomWareRescale(ChannelModel cm, ref Int32 length, WfmVuBaseParam? wfmVuBaseParam, WfmVuBlock? wfmVuBlock)
        {
            WfmPack? pkg = cm.ZoomPackForVu;
            if (pkg is not null)
            {
                if (length <= 0)
                {
                    length = pkg.Buffer.GetLength(1);
                }
                else if (length > pkg.Length)
                {
                    length = pkg.Buffer.GetLength(1);
                }

                var vubuf = new Double[pkg.Buffer.GetLength(0), length];

                if (length <= 0)
                {
                    return new WfmVuBlock(vubuf, Constants.MAX_XPOS_IDX, pkg.Properties.SampInterval);
                }

                var step = pkg.Buffer.GetLength(1) / length;

                if (pkg.Properties.DrawMethod == DrawMethod.XYDots || pkg.Properties.DrawMethod == DrawMethod.XYLines)
                    RescaleXY(cm, length, pkg, vubuf, step);
                else if (pkg.Properties.DrawMethod == DrawMethod.DPX)
                    RescaleDPX(cm, length, pkg, vubuf, step);
                else
                    RescaleYT(cm, length, pkg, vubuf, step);

                var wfmtmbposindex = (Single)((DsoModel.Default.Timebase.PosIndex - (DsoModel.Default.Timebase.ZoomCenterX - Constants.MAX_XPOS_IDX * DsoModel.Default.Timebase.ZoomScaleX / 2)) / DsoModel.Default.Timebase.ZoomScaleX);

                var vuratio = (((wfmVuBaseParam?.Scale ?? cm.Sampling.Scale) * 1E-6 / (wfmVuBaseParam?.PosIdxPerDiv ?? cm.Sampling.PosIdxPerDiv)) / (pkg.Properties.SampInterval * step)) * DsoModel.Default.Timebase.ZoomScaleX;
                var vustart = (wfmtmbposindex) - (pkg.Properties.TmbPosition.Index - pkg.Properties.VuStartIndex) /** pkg.Properties.TmbScale.Value / (wfmVuBaseParam?.Scale ?? cm.Sampling.Scale)*/;
                return new WfmVuBlock(vubuf, vustart, vuratio, pkg.Properties.DpxCorrection);
            }
            else
            {
                if (wfmVuBlock == null)
                {
                    return null;
                }

                Int32 maxpoints = wfmVuBlock.Buffer.GetLength(1);
                Int32 startx = (Int32)((DsoModel.Default.Timebase.ZoomCenterX - (Constants.MAX_XPOS_IDX * DsoModel.Default.Timebase.ZoomScaleX / 2)) * wfmVuBlock.ZoomRatio);
                Int32 endx = (Int32)((DsoModel.Default.Timebase.ZoomCenterX + (Constants.MAX_XPOS_IDX * DsoModel.Default.Timebase.ZoomScaleX / 2)) * wfmVuBlock.ZoomRatio);
                startx = (Int32)(startx - wfmVuBlock.Start * wfmVuBlock.ZoomRatio) - 1;
                endx = (Int32)(endx - wfmVuBlock.Start * wfmVuBlock.ZoomRatio) + 1;
                Double offsetx = 0;

                if (Math.Abs(endx - startx) > maxpoints)
                {
                    startx = 0;
                    endx = maxpoints;
                    offsetx = wfmVuBlock.Start - (DsoModel.Default.Timebase.ZoomCenterX - (Constants.MAX_XPOS_IDX * DsoModel.Default.Timebase.ZoomScaleX / 2));
                }
                else
                {
                    if (startx < 0)
                    {
                        offsetx = (0 - startx) / wfmVuBlock.ZoomRatio;
                        startx = 0;
                        endx = (Int32)(endx + offsetx * wfmVuBlock.ZoomRatio);
                    }
                    if (endx > maxpoints)
                    {
                        offsetx = offsetx + (maxpoints - endx) / wfmVuBlock.ZoomRatio;
                        endx = maxpoints;
                        startx = (Int32)(startx + offsetx * wfmVuBlock.ZoomRatio);
                    }
                }

                startx = startx < 0 ? 0 : startx;
                endx = endx > maxpoints ? maxpoints : endx;

                Int32 bufferstart = (endx - startx) > 0 ? startx : 0;
                Int32 bufferend = (endx - startx) > 0 ? endx : wfmVuBlock.Buffer.Length;
                var vubuf = new Double[wfmVuBlock.Buffer.GetLength(0), bufferend - bufferstart];
                for (Int32 i = 0; i < wfmVuBlock.Buffer.GetLength(0); i++)
                {
                    for (Int32 j = bufferstart; j < bufferend; j++)
                    {
                        vubuf[i, j - bufferstart] = wfmVuBlock.Buffer[i, j];
                    }
                }

                return new WfmVuBlock(vubuf, offsetx / DsoModel.Default.Timebase.ZoomScaleX, wfmVuBlock.ZoomRatio * DsoModel.Default.Timebase.ZoomScaleX);
            }
        }
        private static WfmVuBlock? WareRescale(ChannelModel cm, ref Int32 length, WfmVuBaseParam? wfmVuBaseParam)
        {
            WfmPack? pkg = cm.PackForVu;
            if (pkg is not null)
            {
                if (length <= 0)
                {
                    length = pkg.Buffer.GetLength(1);
                }
                else if (length > pkg.Length)
                {
                    length = pkg.Buffer.GetLength(1);
                }

                var vubuf = new Double[pkg.Buffer.GetLength(0), length];

                if (length <= 0)
                {
                    return new WfmVuBlock(vubuf, Constants.MAX_XPOS_IDX, pkg.Properties.SampInterval);
                }

                var step = pkg.Buffer.GetLength(1) / length;

                if (pkg.Properties.DrawMethod == DrawMethod.XYDots || pkg.Properties.DrawMethod == DrawMethod.XYLines)
                    RescaleXY(cm, length, pkg, vubuf, step);
                else if (pkg.Properties.DrawMethod == DrawMethod.DPX)
                    RescaleDPX(cm, length, pkg, vubuf, step);
                else
                    RescaleYT(cm, length, pkg, vubuf, step);

                var vuratio = ((Decimal)(wfmVuBaseParam?.Scale ?? cm.Sampling.Scale) * (Decimal)1E-6 / (Decimal)(wfmVuBaseParam?.PosIdxPerDiv ?? cm.Sampling.PosIdxPerDiv)) / ((Decimal)pkg.Properties.SampInterval * (Decimal)step);
                var vustart = (Decimal)(wfmVuBaseParam?.PosIndex ?? cm.Sampling.PosIndex) - ((Decimal)pkg.Properties.TmbPosition.Index - (Decimal)pkg.Properties.VuStartIndex) * (Decimal)pkg.Properties.TmbScale.Value / (Decimal)(wfmVuBaseParam?.Scale ?? cm.Sampling.Scale);
                if (cm.Id.IsMath() && DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(cm.Id, out var cp))
                {
                    if (cp is MathPrsnt mp && mp.Args is MathTrendArg)
                    {
                        vustart = 0;
                    }
                }
                return new WfmVuBlock(vubuf, (Double)vustart, (Double)vuratio, pkg.Properties.DpxCorrection);
            }
            return null;
        }
        internal static WfmVuBlock? RescaleMathFFT(MathModel cm, Int32 length, RFWaveType waveType, WfmVuBaseParam? wfmVuBaseParam)
        {
            WfmPack? pkg = cm.PackForVu;
            switch (waveType)
            {
                case RFWaveType.Normal:
                    pkg = cm.PackNormal;
                    break;
                case RFWaveType.Average:
                    pkg = cm.PackAverage;
                    break;
                case RFWaveType.MaxHold:
                    pkg = cm.PackMaxHold;
                    break;
                case RFWaveType.MinHold:
                    pkg = cm.PackMinHold;
                    break;
                default:
                    break;
            }
            if (pkg is not null)
            {
                if (length <= 0)
                {
                    length = pkg.Buffer.GetLength(1);
                }
                else if (length > pkg.Length)
                {
                    length = pkg.Buffer.GetLength(1);
                }

                var vubuf = new Double[pkg.Buffer.GetLength(0), length];

                if (length <= 0)
                {
                    return new WfmVuBlock(vubuf, Constants.MAX_XPOS_IDX, pkg.Properties.SampInterval);
                }

                var step = pkg.Buffer.GetLength(1) / length;

                if (pkg.Properties.DrawMethod == DrawMethod.XYDots || pkg.Properties.DrawMethod == DrawMethod.XYLines)
                    RescaleXY(cm, length, pkg, vubuf, step);
                else if (pkg.Properties.DrawMethod == DrawMethod.DPX)
                    RescaleDPX(cm, length, pkg, vubuf, step);
                else
                    RescaleYT(cm, length, pkg, vubuf, step);

                var vuratio = ((wfmVuBaseParam?.Scale ?? cm.Sampling.Scale) * 1E-6 / (wfmVuBaseParam?.PosIdxPerDiv ?? cm.Sampling.PosIdxPerDiv)) / (pkg.Properties.SampInterval * step);
                var vustart = (wfmVuBaseParam?.PosIndex ?? cm.Sampling.PosIndex) - (pkg.Properties.TmbPosition.Index - pkg.Properties.VuStartIndex) * pkg.Properties.TmbScale.Value / (wfmVuBaseParam?.Scale ?? cm.Sampling.Scale);
                return new WfmVuBlock(vubuf, vustart, vuratio, pkg.Properties.DpxCorrection);
            }
            return null;
        }
        private static void RescaleYT(ChannelModel cm, Int32 length, WfmPack pkg, Double[,] vubuf, Int32 step)
        {
            var chnlbias = pkg.Properties.ChnlBias;
            var scale = cm.Conditioning.ScaleBymV;
            var posidxperdiv = cm.Conditioning.PosIdxPerDiv;
            var posdndex = cm.Conditioning.PosIndex;
            var posscale = posidxperdiv / scale;

            //Parallel.For(0, length, (index) =>
            //{
            //    for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
            //    {
            //        Double y = pkg.Buffer[i, index * step];
            //        if (!Double.IsNaN(y))
            //        {
            //            y = (y - chnlbias) * posscale + posdndex;
            //            y = ValidateVuSamples(y);
            //        }
            //        vubuf[i, index] = y;
            //    }
            //});

            for (Int32 index = 0; index < length; index++)
            {
                for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
                {
                    Double y = pkg.Buffer[i, index * step];
                    if (!Double.IsNaN(y))
                    {
                        y = (y - chnlbias) * posscale + posdndex;
                        y = ValidateVuSamples(y);
                    }
                    vubuf[i, index] = y;
                }
            };
        }

        private static void RescaleDPX(ChannelModel cm, Int32 length, WfmPack pkg, Double[,] vubuf, Int32 step)
        {
            Parallel.For(0, length, (index) =>
            {
                for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
                {
                    Double y = pkg.Buffer[i, index * step];
                    if (!Double.IsNaN(y))
                    {
                    }
                    vubuf[i, index] = y;
                }
            });
        }

        private static void RescaleXY(ChannelModel cm, Int32 length, WfmPack pkg, Double[,] vubuf, Int32 step)
        {
            if (pkg.Buffer.GetLength(0) < 2)
                return;
            Parallel.For(0, length, (index) =>
            {
                Double x = pkg.Buffer[0, index * step];
                if (!Double.IsNaN(x))
                {
                    x = x / cm.Sampling.Scale * cm.Sampling.PosIdxPerDiv * 1e3 + cm.Sampling.PosIndex;
                }
                vubuf[0, index] = x;

                Double y = pkg.Buffer[1, index * step];
                if (!Double.IsNaN(y))
                {
                    y = y / cm.Conditioning.ScaleBymV * cm.Conditioning.PosIdxPerDiv + cm.Conditioning.PosIndex;
                }
                vubuf[1, index] = y;

            });
        }

        internal static (WfmVuBlock?, WfmVuBlock?)? RescaleBitSeq(ChannelModel cm, Int32 _, WfmVuBaseParam? wfmVuBaseParam)
        {
            var dm = (DigitalModel)cm;
            var pkg = cm.PackForVu;
            if (pkg is not null)
            {
                //!!!Notice: make a bit matrix for digital channel view
                var vubuf = new Double[pkg.Buffer.GetLength(0) * 16, pkg.Length];
                //var vubuf = new Double[pkg.Buffer.GetLength(1) * 16, pkg.Length];
                //避免过渡态
                var height = dm.BitHeight;
                var bitscount = dm.Conditioning.Bits.Count;
                var bitspos = dm.Conditioning.Bits.Select(b => b.PosIndex).ToList();

                for (Int32 waveindex = 0; waveindex < pkg.Length; waveindex++)
                {
                    for (Int32 chindex = 0; chindex < bitscount; chindex++)
                    {
                        if (!Double.IsNaN(pkg.Buffer[chindex / 16, waveindex]))
                        //if (!Double.IsNaN(pkg.Buffer[waveindex, chindex / 16]))
                        {
                            Boolean bitstate = Convert.ToBoolean((((UInt16)pkg.Buffer[chindex / 16, waveindex]) >> (15 - chindex % 16)) & 0x01);
                            //Boolean bitstate = Convert.ToBoolean((((UInt16)pkg.Buffer[waveindex, chindex / 16]) >> (chindex % 16)) & 0x01);
                            vubuf[chindex, waveindex] = bitspos[chindex] + (bitstate ? height : 0);
                        }
                        else
                        {
                            chindex += 15;
                        }

                    }
                }



                //for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
                //{
                //    //Parallel.For(0, pkg.Length, (index) =>
                //    //{                        
                //    //    if (!Double.IsNaN(pkg.Buffer[i, index]))
                //    //    {
                //    //        var y = Convert.ToInt32(pkg.Buffer[i, index]);
                //    //        for (Int32 j = 0; j < dm.Conditioning.Bits.Count; j++) 
                //    //        {
                //    //            vubuf[i * dm.Conditioning.Bits.Count + j, index] = dm.Conditioning.Bits[j].PosIndex + ((y & 0x0001) == 1 ? dm.Conditioning.BitHeight : 0);
                //    //            y >>= 1;
                //    //        }
                //    //    }


                //    //});

                //    for (Int32 k = 0; k < pkg.Length; k++)
                //    {
                //        if (!Double.IsNaN(pkg.Buffer[i, k]))
                //        {
                //            var y = Convert.ToInt32(pkg.Buffer[i, k]) & 0xFFFF;
                //            for (Int32 j = 0; j < 16; j++)
                //            {
                //                vubuf[i * 16 + j, k] = dm.Conditioning.Bits[i * 16 + j].PosIndex + ((y & 0x0001) == 1 ? height : 0);
                //                y >>= 1;
                //            }
                //        }
                //    }
                //}
                var vuratio = (dm.Sampling.Scale * 1E-6 / dm.Sampling.PosIdxPerDiv) / pkg.Properties.SampInterval;
                var vustart = dm.Sampling.PosIndex - (pkg.Properties.TmbPosition.Index - pkg.Properties.VuStartIndex) * pkg.Properties.TmbScale.Value / dm.Sampling.Scale;
                return (new(vubuf, vustart, vuratio), null);
            }
            return null;
        }

        #region RadioFrequency

        internal static (WfmVuBlock?, WfmVuBlock?)? RescaleRadioFrequency(ChannelModel cm, Int32 length, WfmVuBaseParam? wfmVuBaseParam)
        {
            var pkg = cm.Pack;
            var rf = ((RadioFrequencyModel)cm);
            if (pkg != null /*&& pkg.Properties.Version == "U2.0"*/)
            {
                var vubuf = new Double[pkg.Buffer.GetLength(0), pkg.Buffer.GetLength(1)];
                Parallel.For(0, pkg.Length, (index) =>
                {
                    for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
                    {
                        vubuf[i, index] = pkg.Buffer[i, index];
                    }
                });
                var lengthint = (Int32)Math.Ceiling(((RFWfmProperties)pkg.Properties).FFTLength);
                var lengthdouble = ((RFWfmProperties)pkg.Properties).FFTLength;
                var diff = lengthint / 2 - lengthdouble / 2;
                if (diff <= 0 | rf.Sampling.Span == Constants.RF_SPAN_MAX)
                    diff = 0;
                var ratio = ((Double)rf.Sampling.FrequencyScale / Constants.IDX_PER_XDIV) / ((RFWfmProperties)pkg.Properties).RBW;
                var start = ((Double)(rf.Sampling.StartFrequency - diff * ((RFWfmProperties)pkg.Properties).RBW - rf.Sampling.FigureStartFrequency) / rf.Sampling.FrequencyScale) * Constants.IDX_PER_XDIV;
                return (new(vubuf, start, ratio), null);
            }
            return null;
        }

        #region MDFunction
        internal static WfmVuBlock? RescaleMDIQ(Object rf, MDVirticalType virticalType)
        {
            WfmPack? pkg;
            Double[,] vubuf = new Double[0, 0];
            switch (virticalType)
            {
                case MDVirticalType.Phase:
                    pkg = ((PhaseVSTimeModel)rf).Pack;
                    RescalePhase(((PhaseVSTimeModel)rf).Conditioning, pkg, ref vubuf);
                    break;
                case MDVirticalType.Amplitude:
                    pkg = ((AmpVSTimeModel)rf).Pack;
                    RescaleAmp(((AmpVSTimeModel)rf).Conditioning, pkg, ref vubuf);
                    break;
                case MDVirticalType.Frequency:
                    pkg = ((FrequencyVSTimeModel)rf).Pack;
                    RescaleFreqV(((FrequencyVSTimeModel)rf).Conditioning, pkg, ref vubuf);
                    break;
                default:
                    return null;
            }
            if (pkg != null)
            {
                var sampInterval = ((RFWfmProperties)pkg.Properties).SampInterval;
                var ratio = pkg.Buffer.GetLength(1) * sampInterval / Constants.VIS_XDIVS_NUM / Constants.IDX_PER_XDIV / sampInterval;
                var start = 0;// cm.Sampling.PosIndex - pkg.Properties.TmbPosition.Index / ratio;
                return new(vubuf, start, ratio);
            }
            return null;
        }

        internal static WfmVuBlock? RescaleMDIQFFT(Object rf, MDVirticalType virticalType, RFWaveType waveType)
        {
            WfmPack? pkg;
            Double[,] vubuf = new Double[0, 0];
            FrequencyModel sampling;
            switch (virticalType)
            {
                case MDVirticalType.Phase:
                    pkg = ((PhaseVSFrequencyModel)rf).Pack;
                    sampling = ((PhaseVSFrequencyModel)rf).Sampling;
                    RescalePhase(((PhaseVSFrequencyModel)rf).Conditioning, pkg, ref vubuf);
                    break;
                case MDVirticalType.Amplitude:
                    RadioFrequencyModel rfm = ((RadioFrequencyModel)rf);
                    sampling = ((RadioFrequencyModel)rf).Sampling;
                    switch (waveType)
                    {
                        case RFWaveType.Normal:
                            pkg = rfm.PackNormal;
                            break;
                        case RFWaveType.Average:
                            pkg = rfm.PackAverage;
                            break;
                        case RFWaveType.MaxHold:
                            pkg = rfm.PackMaxHold;
                            break;
                        case RFWaveType.MinHold:
                            pkg = rfm.PackMinHold;
                            break;
                        default:
                            pkg = rfm.Pack;
                            break;
                    }
                    RescaleAmp(((RadioFrequencyModel)rf).Conditioning, pkg, ref vubuf);
                    break;
                case MDVirticalType.Time:
                    pkg = ((TimeVSFrequencyModel)rf).Pack;
                    sampling = ((TimeVSFrequencyModel)rf).Sampling;
                    RescaleTimeV(((TimeVSFrequencyModel)rf).Conditioning, pkg, ref vubuf);
                    break;
                default:
                    return null;
            }
            if (pkg != null)
            {
                var lengthDouble = ((RFWfmProperties)pkg.Properties).FFTLength;
                var lengthInt = (Int32)lengthDouble;
                var rbw = ((RFWfmProperties)pkg.Properties).RBW;

                var diff = (lengthInt - lengthDouble) / 2;
                if (diff <= 0)
                    diff = 0;
                var ratio = (Double)sampling.FrequencyScale / Constants.IDX_PER_XDIV / rbw;
                var start = (Double)(sampling.StartFrequency - diff * rbw - sampling.FigureStartFrequency) / sampling.FrequencyScale * Constants.IDX_PER_XDIV;

                return new(vubuf, start, ratio);
            }
            return null;
        }

        private static Boolean RescaleAmp(AmplitudeModel condition, WfmPack? pkg, ref Double[,] buffer)
        {
            if (pkg == null)
                return false;
            Double[,] vubuf = new Double[pkg.Buffer.GetLength(0), pkg.Buffer.GetLength(1)];
            switch (condition.UnitType)
            {
                case AmplitudeUnitType.Linear:
                    Parallel.For(0, pkg.Length, (index) =>
                    {
                        for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
                        {
                            Double y = pkg.Buffer[i, index];
                            if (!Double.IsNaN(y))
                            {
                                y = y / condition.AmpScale * Constants.IDX_PER_YDIV + condition.PosIndex;
                                y = ValidateVuSamples(y);
                            }
                            vubuf[i, index] = y;
                        }
                    });
                    break;
                case AmplitudeUnitType.Logarithm:
                    Double unitDiff = 0;
                    switch (condition.PUnit)
                    {
                        case LogarithmUnit.dBm:
                            unitDiff = -106.99;
                            break;
                        case LogarithmUnit.dBμW:
                            unitDiff = -76.99;
                            break;
                        case LogarithmUnit.dBmV:
                            unitDiff = -60;
                            break;
                        case LogarithmUnit.dBμV:
                            unitDiff = 0;
                            break;
                        case LogarithmUnit.dBmA:
                            unitDiff = -93.98;
                            break;
                        case LogarithmUnit.dBμA:
                            unitDiff = -33.98;
                            break;
                    }
                    Parallel.For(0, pkg.Length, (index) =>
                    {
                        for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
                        {
                            Double y = pkg.Buffer[i, index];
                            if (!Double.IsNaN(y))
                            {
                                y = 20 * Math.Log10(y);
                                y = y + unitDiff;
                                y = y / condition.AmpScale * Constants.IDX_PER_YDIV + condition.PosIndex;
                                y = ValidateVuSamples(y);
                            }
                            vubuf[i, index] = y;
                        }
                    });
                    break;
            }
            buffer = vubuf;
            return true;
        }

        private static Boolean RescalePhase(PhaseModel condition, WfmPack? pkg, ref Double[,] buffer)
        {
            if (pkg == null)
                return false;
            Double[,] vubuf = new Double[pkg.Buffer.GetLength(0), pkg.Buffer.GetLength(1)];
            switch (condition.UnitType)
            {
                case PhaseUnitType.Degree:
                    Parallel.For(0, pkg.Length, (index) =>
                    {
                        for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
                        {
                            Double y = pkg.Buffer[i, index];
                            if (!Double.IsNaN(y))
                            {
                                y = y / condition.PhaseScale * Constants.IDX_PER_YDIV + condition.PosIndex;
                                y = ValidateVuSamples(y);
                            }
                            vubuf[i, index] = y;
                        }
                    });
                    break;
                case PhaseUnitType.Radian:
                    Parallel.For(0, pkg.Length, (index) =>
                    {
                        for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
                        {
                            Double y = pkg.Buffer[i, index];
                            if (!Double.IsNaN(y))
                            {
                                y = y / 180 * Math.PI;
                                y = y / condition.PhaseScale * Constants.IDX_PER_YDIV + condition.PosIndex;
                                y = ValidateVuSamples(y);
                            }
                            vubuf[i, index] = y;
                        }
                    });
                    break;
                case PhaseUnitType.GroupDelay:
                    Double prey = 0;
                    Parallel.For(0, pkg.Length, (index) =>
                    {
                        for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
                        {
                            Double y = pkg.Buffer[i, index];

                            if (!Double.IsNaN(y))
                            {
                                var lasty = prey;
                                prey = y;
                                y = -(y - lasty) / (2 * 180 * ((RFWfmProperties)pkg.Properties).RBW) * 1000000000000;//转为ps
                                y = y / condition.PhaseScale * Constants.IDX_PER_YDIV + condition.PosIndex;//
                                y = ValidateVuSamples(y);
                            }
                            vubuf[i, index] = y;
                        }
                    });
                    break;
                default:
                    break;
            }
            buffer = vubuf;
            return true;
        }

        private static Boolean RescaleFreqV(FrequencyModelV condition, WfmPack? pkg, ref Double[,] buffer)
        {
            if (pkg == null)
                return false;
            Double[,] vubuf = new Double[pkg.Buffer.GetLength(0), pkg.Buffer.GetLength(1)];
            Parallel.For(0, pkg.Length, (index) =>
            {
                for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
                {
                    Double y = pkg.Buffer[i, index];
                    if (!Double.IsNaN(y))
                    {
                        y = (y - pkg.Properties.ChnlBias) / condition.Scale * condition.PosIdxPerDiv + condition.PosIndex;
                        y = ValidateVuSamples(y);
                    }
                    vubuf[i, index] = y;
                }
            });
            buffer = vubuf;
            return true;
        }

        private static Boolean RescaleTimeV(TimeModelV condition, WfmPack? pkg, ref Double[,] buffer)
        {
            if (pkg == null)
                return false;
            Double[,] vubuf = new Double[pkg.Buffer.GetLength(0), pkg.Buffer.GetLength(1)];
            Parallel.For(0, pkg.Length, (index) =>
            {
                for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
                {
                    Double y = pkg.Buffer[i, index];
                    if (!Double.IsNaN(y))
                    {
                        y = (y - pkg.Properties.ChnlBias) / condition.Scale * condition.PosIdxPerDiv + condition.PosIndex;
                        y = ValidateVuSamples(y);
                    }
                    vubuf[i, index] = y;
                }
            });
            return true;
        }

        #endregion
        #endregion

    }
}

using ScopeX.ComModel;
using ScopeX.Hardware.Driver;
using ScopeX.MathExt;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ScopeX.Core
{
    internal class DataSrcFifo : IDataSource
    {
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

        private Int32 _aiSourceCallCounter = 0;
        private const Int32 AI_SOURCE_CALL_INTERVAL = 10; // 每10次调用才执行一次TryTakeAiSource

        private sealed record Context(IEnumerable<Double> LastBuffer, WfmProperties Properties, UInt16[,] Buffer, Double Pos0ByAdc, AnaChnlCoupling Coupling, Boolean Inverted, AnaChnlAcqMode Mode, CancellationToken CancelToken)
        {
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
        };

        private static Boolean TryTakeDDRFastTransWave(Int32 channelId, DataRole dataRole, out UInt16[] buffer, out Double sampleInterval)
        {
            buffer = new UInt16[0];
            sampleInterval = 0.1;

            var viewmask = dataRole.ToString();
            Boolean ret = Hd.AnalogChannel!.TryTakeWave((ChannelId)channelId, Acquisition.Default.AllChnlReadInfo.Where(o => o.Mark.Equals(viewmask)).ToList(), out var wfm, null);

            //Boolean ret = Hd.AnalogChannel!.TryTakeDdrSourceWave(channelId, 0.0, waveLength / Constants.SAMPLING_RATE, out var data, out var si);
            if (ret)
            {
                buffer = wfm[viewmask].wfmData.ToArray();
                sampleInterval = wfm[viewmask].wfmSampleInfo.SampleIntervalByus / 1e6;
                DsoModel.Default.VectorAnalysisModel.SetSampleData(buffer, sampleInterval);
            }
            return ret;
        }

        public static void TryTakeAdvancedFuncSource()
        {
            if (DsoModel.Default.JitterModel.Active)
            {
                Int32 channelid = (Int32)DsoModel.Default.JitterModel.Source;

                if (!(DsoModel.Default.TryGetChannel(DsoModel.Default.JitterModel.Source, out var chnl) && chnl.Active == true))
                {
                    return;
                }
                if (DsoModel.Default.JitterModel.Source.IsAnalog())
                {
                    Boolean ret = TryTakeDDRFastTransWave(channelid, DataRole.Sda, out UInt16[] buffer, out Double interval);
                    if (ret)
                    {
                        var ch = chnl as AnalogModel;
                        DsoModel.Default.JitterModel.SetData(buffer, (interval, ch.Conditioning.PosIndex, ch.Conditioning.BiasByuV, ch.Conditioning.Scale, ch.Sampling.Scale));
                    }
                }
                else if (DsoModel.Default.JitterModel.Source.IsReference())
                {
                    if (chnl.Pack != null)
                    {
                        var data = chnl.Pack.Buffer.Cast<Double>().ToArray();
                        //还原为码值（逆量化）
                        //量化公式pkg.Buffer[i, j] = (pkg.Buffer[i, j] - ctx.Pos0ByAdc) / Constants.SAMPS_PER_YDIV * ctx.Properties.ChnlScale.Value + ctx.Properties.ChnlBias;
                        var pos0ByAdc = (chnl.Conditioning.PosIndex / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2);
                        var ydivscale = Constants.SAMPS_PER_YDIV / chnl.Conditioning.Scale;
                        for (Int32 i = 0, l = data.Length; i < l; i++)
                        {
                            data[i] = data[i] * ydivscale + pos0ByAdc;
                        }
                        var sampleInterval = chnl.Pack.Properties.SampInterval;
                        var refbufferu16 = data.Select(x => (UInt16)x).ToArray();
                        var ch = chnl as ReferenceModel;
                        DsoModel.Default.JitterModel.SetData(refbufferu16, (sampleInterval, ch.Conditioning.PosIndex, 0, ch.Conditioning.Scale, ch.Sampling.Scale));
                    }
                }
            }

            //if (DsoModel.Default.VectorAnalysisModel.Enabled)
            //{
            //    Int32 channelid = (Int32)DsoModel.Default.VectorAnalysisModel.Source;
            //    UInt32 wavelength = DsoModel.Default.VectorAnalysisModel.DataLength;
            //    Boolean ret = TryTakeDDRFastTransWave(channelid, DataRole.Vsa, out UInt16[] buffer, out Double interval);
            //    if (buffer.Length > wavelength)
            //        buffer = buffer.Take((Int32)wavelength).ToArray();
            //    if (ret)
            //        DsoModel.Default.VectorAnalysisModel.SetSampleData(buffer, interval);
            //}
        }
        public Boolean TryTakeSegmentWave(ChannelId channel, out UInt16[,] waveData, out WfmSampleInfo wfmSampleInfo, CancellationToken? softResetToken = null)
        {
            Boolean ans = true;
            wfmSampleInfo = new();
            if (DsoModel.Default.Timebase.CollectedFrameCount < DsoModel.Default.Timebase.FrameCount)
            {
                waveData = new UInt16[0, 0];
                return false;
            }

            var viewmask = nameof(DataRole.View);
            var readinfo = Acquisition.Default.AllChnlReadInfo.FirstOrDefault(o => o.Mark.Equals(viewmask));

            var startframeid = DsoModel.Default.Timebase.SequentStartFrame;
            var endframeid = DsoModel.Default.Timebase.SequentEndFrame;
            endframeid = startframeid > endframeid ? startframeid : endframeid;

            if (DsoModel.Default.Timebase.WorkMode == SegmentWorkMode.Single)
            {
                endframeid = startframeid = DsoModel.Default.Timebase.CurFrameId;
            }
            var framecount = endframeid - startframeid + 1;
            framecount = framecount > Constants.SEGMENT_FRAME_SPAN_COUNT_DEFAULT ? Constants.SEGMENT_FRAME_SPAN_COUNT_DEFAULT : framecount;
            //第一幅为模拟波形
            ans &= Hd.AnalogChannel!.TryTakeSegmentWave(channel, readinfo!, startframeid, framecount, out waveData, out wfmSampleInfo, out var secondByps, softResetToken);

            if (DsoModel.Default.Timebase.RefActive)
            {
                endframeid = startframeid = DsoModel.Default.Timebase.ReferFrameIds;
                framecount = endframeid - startframeid + 1;
                //第二幅为参考波形
                ans &= Hd.AnalogChannel!.TryTakeSegmentWave(channel, readinfo!, startframeid, framecount, out var refframebuffer, out wfmSampleInfo, out var timeByps, softResetToken);
                if (ans)
                {
                    var framelength = refframebuffer.GetLength(1);
                    var buffer = new UInt16[2, framelength];
                    //拷贝第一幅
                    Unsafe.CopyBlock(ref Unsafe.As<UInt16, Byte>(ref buffer[0, 0]), ref Unsafe.As<UInt16, Byte>(ref waveData[0, 0]), (UInt32)(Unsafe.SizeOf<UInt16>() * framelength));
                    //拷贝第二幅
                    Unsafe.CopyBlock(ref Unsafe.As<UInt16, Byte>(ref buffer[1, 0]), ref Unsafe.As<UInt16, Byte>(ref refframebuffer[0, 0]), (UInt32)(Unsafe.SizeOf<UInt16>() * framelength));

                    waveData = buffer;
                }
            }

            //var viewmask = nameof(DataRole.View);
            //var readinfo = Acquisition.AllChnlReadInfo.FirstOrDefault(o => o.Mark.Equals(viewmask));
            //var datacountperframe = readinfo!.pkgInfo.DotsCount;

            //waveData = new UInt16[framecount, datacountperframe];
            //for (Int32 frameid = startframeid; frameid <= endframeid; frameid++)
            //{

            //    ans &= Hd.AnalogChannel!.TryTakeSegmentWave(channel, readinfo, startframeid, framecount, out var wfm, out wfmSampleInfo, out var secondByps, softResetToken);
            //    if (ans && wfm.ContainsKey(viewmask))
            //    {
            //        var tempwavedata = wfm[viewmask].wfmData.ToArray();
            //        Unsafe.CopyBlock(ref Unsafe.As<UInt16, Byte>(ref waveData[frameid - startframeid, 0]), ref Unsafe.As<UInt16, Byte>(ref tempwavedata[0]), (UInt32)(Unsafe.SizeOf<UInt16>() * tempwavedata.Length));
            //        wfmSampleInfo = wfm[viewmask].wfmSampleInfo;
            //    }
            //}

            //if (DsoModel.Default.Timebase.WorkMode == SegmentWorkMode.Single)
            //{
            //    ans &= Hd.AnalogChannel!.TryTakeSegmentWave(channel, DsoModel.Default.Timebase.CurFrameId, out var buffer, out var si, out var time);
            //    waveData = buffer.ToMatrix(1, buffer.Count);
            //    wfmSampleInfo.SampleIntervalByus = si.SampleIntervalByus;
            //    DsoModel.Default.Timebase.CurFrameSecond = time;
            //    return ans;
            //}

            //foreach (var segment in DsoModel.Default.Timebase.ChoseFrameIds)
            //{
            //    ans &= Hd.AnalogChannel!.TryTakeSegmentWave(channel, segment, out var buffer, out var si, out var time);
            //    if (ans)
            //    {
            //        tmp.Add(buffer);
            //        wfmSampleInfo.SampleIntervalByus = si.SampleIntervalByus;
            //        wavelen = buffer.Count < wavelen ? buffer.Count : wavelen;
            //    }
            //}
            //waveData = new UInt16[tmp.Count, wavelen];
            //for (Int32 i = 0; i < tmp.Count; i++)
            //{

            //    for (Int32 j = 0; j < wavelen; j++)
            //    {
            //        waveData[i, j] = tmp[i][j];
            //    }
            //}
            return ans;
        }

        //private Boolean TryTakeWave(Int32 channel, out UInt16[,] buffer, out WfmSampleInfo si, CancellationToken? softResetToken)
        //{
        //    if (DsoModel.Default.Timebase.SegmentActive)
        //        return TryTakeSegmentWave(channel, out buffer, out si);

        //    Boolean ans = Hd.AnalogChannel!.TryTakeWave(channel, out var wave, out si, softResetToken);
        //    buffer = wave.ToMatrix(1, wave.Count);
        //    return ans;
        //}

        private Boolean TryTakeWave(ChannelId chnlId, DataRole dataRole, out UInt16[,] buffer, out WfmSampleInfo si, CancellationToken? softResetToken)
        {
            var viewmask = nameof(DataRole.View);
            switch (dataRole)
            {
                case DataRole.View:
                    viewmask = nameof(DataRole.View);
                    break;
                case DataRole.Zoom:
                    viewmask = nameof(DataRole.Zoom);
                    break;
                case DataRole.Sda:
                    viewmask = nameof(DataRole.Sda);
                    break;
                case DataRole.Vsa:
                    viewmask = nameof(DataRole.Vsa);
                    break;
                default:
                    break;
            }
            Boolean ans = Hd.AnalogChannel!.TryTakeWave(chnlId, Acquisition.Default.AllChnlReadInfo.Where(o => o.Mark.Equals(viewmask)).ToList(), out var wfm, softResetToken);
            if (wfm.ContainsKey(viewmask))
            {
                buffer = wfm[viewmask].wfmData.ToMatrix(1, wfm[viewmask].wfmData.Count);
                si = wfm[viewmask].wfmSampleInfo;
                if (ans)
                {
                    DsoModel.Default.Timebase.AcqedStorageWaveDotsCnt = wfm[viewmask].wfmSampleInfo?.HdMessage?.Timebase?.StorageWaveDotsCnt ?? 0;
                }
            }
            else
            {
                buffer = new UInt16[0, 0];
                si = new WfmSampleInfo();
            }

            return ans;
        }

        public static void TryTakeAiSource(ChannelId aid)
        {
            var buffer = new UInt16[0];
            var sampleInterval = 0.1;

            var viewmask = DataRole.Ai.ToString() ;
            Boolean ret = Hd.AnalogChannel!.TryTakeWave((ChannelId)aid, Acquisition.Default.AllChnlReadInfo.Where(o => o.Mark.Equals(viewmask)).ToList(), out var wfm, null);
             
            //Boolean ret = Hd.AnalogChannel!.TryTakeDdrSourceWave(channelId, 0.0, waveLength / Constants.SAMPLING_RATE, out var data, out var si);
            if (ret)
            {
                buffer = wfm[viewmask].wfmData.ToArray();
                sampleInterval = wfm[viewmask].wfmSampleInfo.SampleIntervalByus / 1e6;
                DsoModel.Default.ArtificialIntelligence.UpdateData(aid, buffer, sampleInterval);
                DsoModel.Default.VectorAnalysisModel.SetSampleData(buffer, sampleInterval);
            }
        }

        public Object? Prepare(Boolean init, ChannelId aid, CancellationToken ct, DataRole dataRole, CancellationToken? softResetToken = null)
        {
            TryTakeAdvancedFuncSource();
            
            // 降低AI数据源调用频率，避免资源浪费
            _aiSourceCallCounter++;
            if (_aiSourceCallCounter >= AI_SOURCE_CALL_INTERVAL)
            {
                TryTakeAiSource(aid);
                _aiSourceCallCounter = 0;
            }
            
            if (dataRole == DataRole.Zoom)
            {
                if (Dispatcher.IsScan && Dispatcher.IsRunning)
                {
                    return null;
                }
                return ZoomPrepare(init, aid, dataRole, ct, softResetToken);
            }
            else
            {
                return WavePrepare(init, aid, dataRole, ct, softResetToken);
            }
        }
        private Object WavePrepare(Boolean init, ChannelId aid, DataRole dataRole, CancellationToken ct, CancellationToken? softResetToken)
        {
            var ans = false;
            UInt16[,] buffer = new UInt16[0, 0];
            WfmSampleInfo si = new WfmSampleInfo();
            if (DsoModel.Default.Timebase.SegmentActive)
            {
                ans = TryTakeSegmentWave(aid, out buffer, out si, softResetToken);
            }
            else
                ans = TryTakeWave(aid, dataRole, out buffer, out si, softResetToken);
            if (init)
            {
                _Averager.Reset();
                _Evlp.Reset();
            }
            if (!ans)
            {
                return null;
            }
            var ach = (AnalogModel)DsoModel.Default.GetChannel(aid);
            DsoPrsnt.DefaultDsoPrsnt.TryGetChannel(aid, out IChnlPrsnt prsnt);
            var persisitType = DsoModel.Default.Display.Persist;
            var wfmtimeposition = si.HdMessage?.Timebase?.TmbPosition ?? ach.Sampling.Position /*- si.StartTimeByus*/;
            var wfmtmbposindex = ach.Sampling.CalcPosIndex(wfmtimeposition, si.HdMessage?.Timebase?.TmbScale ?? ach.Sampling.Scale);
            //var probegain = (si.HdMessage?.Analog?[(Int32)aid]?.ProbeIndex ?? ach.Conditioning.ProbeIndex) switch
            //{
            //    AnaChnlProbe.x1 => 1,
            //    AnaChnlProbe.x10 => 10,
            //    AnaChnlProbe.x100 => 100,
            //    _ => 1,
            //};
            var probegain = si.HdMessage?.Analog?[(Int32)aid]?.ProbeGain ?? ach.Conditioning.ProbeGain;
            probegain *= ach.Conditioning.ProbeUnitRatio;
            var prop = new WfmProperties(ach.Name)
            {
                //ChnlPosition = (si.HdMessage?.Analog?[(Int32)aid]?.PositionIndex ?? ach.Conditioning.PosIndex, (si.HdMessage?.Analog?[(Int32)aid]?.Position ?? (ach.Conditioning.Position / probegain)) * probegain),
                //ChnlScale = (si.HdMessage?.Analog?[(Int32)aid]?.ScaleIndex ?? (Int32)ach.Conditioning.ScaleIndex, (si.HdMessage?.Analog?[(Int32)aid]?.ScaleBymV ?? (ach.Conditioning.ScaleBymV / probegain)) * probegain),
                ChnlPosition = (TriggerModel.State == SysState.Stop ? ach.Conditioning.PosIndex : si.HdMessage?.Analog?[(Int32)aid]?.PositionIndex ?? ach.Conditioning.PosIndex, (TriggerModel.State == SysState.Stop ? (ach.Conditioning.Position / probegain) * probegain : si.HdMessage?.Analog?[(Int32)aid]?.Position ?? (ach.Conditioning.Position / probegain)) * probegain),
                ChnlScale = (si.HdMessage?.Analog?[(Int32)aid]?.ScaleIndex ?? (Int32)ach.Conditioning.ScaleIndex, TriggerModel.State == SysState.Stop ? (ach.Conditioning.ScaleBymV / probegain) * probegain : (si.HdMessage?.Analog?[(Int32)aid]?.ScaleBymV ?? (ach.Conditioning.ScaleBymV / probegain)) * probegain),
                ChnlUnit = (ach.Conditioning.Prefix, ach.Conditioning.Unit),
                ChnlBias = (si.HdMessage?.Analog?[(Int32)aid]?.Bias ?? (ach.Conditioning.BiasByuV / probegain)) / 1000.0 * probegain,
                ProbeInfo = (si.HdMessage?.Analog?[(Int32)aid]?.ProbeGain ?? ach.Conditioning.ProbeGain, ach.Conditioning.ProbeUnitRatio),

                TmbPosition = (wfmtmbposindex, wfmtimeposition),
                TmbScale = (si.HdMessage?.Timebase?.TmbScaleIndex ?? (Int32)ach.Sampling.ScaleIndex, si.HdMessage?.Timebase?.TmbScale ?? ach.Sampling.Scale),
                TmbUnit = (ach.Sampling.Prefix, ach.Sampling.Unit),

                SampInterval = si.SampleIntervalByus * 1E-6,
                TrigErrorTime = si.TrigErrorTime,
                VuStartIndex = (-si.StartTimeByus) / (si.HdMessage?.Timebase?.TmbScale ?? ach.Sampling.Scale) * Constants.IDX_PER_XDIV,
                FrameNo = si.FrameNo,
            };
            _Averager.MaxCnts = ach.Sampling.AverageCnt;
            _Evlp.MaxCnts = ach.Sampling.EnvelopeCnt;
            ach.LastBuffer = ach.Pack?.Buffer?.ToEnumerable() ?? Enumerable.Empty<Double>();

            if (init)
            {
                _Averager.Reset();
                _Evlp.Reset();
            }

            if (buffer is null)
            {
                buffer = new UInt16[0, 0];
            }
            DsoModel.Default.ArtificialIntelligence.UpdateScreenData(aid, buffer.Cast<UInt16>().ToArray(), si.SampleIntervalByus);

            var pos0 = prop.ChnlPosition.Index / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
            var coupling = si.HdMessage?.Analog?[(Int32)aid]?.Coupling ?? ach.Conditioning.Coupling;
            //var isinverted = si.HdMessage?.Analog?[(Int32)aid]?.IsInverted ?? ach.Conditioning.IsInverted;
            var isinverted = ach.Conditioning.IsInverted;//Dirver未做反相操作
            var mode = si.HdMessage?.Timebase?.AcqMode ?? ach.Sampling.Mode;
            //if((si.HdMessage?.Analog?[(Int32)aid]?.Active ?? ach.Active) == false)
            //{
            //    si.HdMessage.Timebase.NeedWaveDotsCnt
            //}

            if (DsoModel.Default.ChannelAdcDatas.ContainsKey(aid))
            {
                if (DsoModel.Default.ChannelAdcDatas[aid].GetLength(1) != buffer.GetLength(1))
                {
                    DsoModel.Default.ChannelAdcDatas[aid] = new UInt16[1, buffer.GetLength(1)];
                }
                if (buffer.Length > 0)
                {
                    Unsafe.CopyBlock(ref Unsafe.As<UInt16, Byte>(ref DsoModel.Default.ChannelAdcDatas[aid][0, 0]), ref Unsafe.As<UInt16, Byte>(ref buffer[0, 0]), (UInt32)(Unsafe.SizeOf<UInt16>() * buffer.GetLength(1)));
                }
            }

            return new Context(ach.LastBuffer, prop, buffer, pos0, coupling, isinverted, mode, ct)
            {
                EnvelopeOpt = ach.Sampling.EnvelopOpt,
                PersistType = persisitType,
                Persistent = ach.Persistent,
            };
        }
        private Object ZoomPrepare(Boolean init, ChannelId aid, DataRole dataRole, CancellationToken ct, CancellationToken? softResetToken)
        {
            var ans = false;
            UInt16[,] buffer = new UInt16[0, 0];
            WfmSampleInfo si = new WfmSampleInfo();
            if (DsoModel.Default.Timebase.SegmentActive)
            {
                ans = TryTakeSegmentWave(aid, out buffer, out si, softResetToken);
            }
            else
                ans = TryTakeWave(aid, dataRole, out buffer, out si, softResetToken);
            if (!ans)
            {
                return null;
            }
            DsoModel.Default.ArtificialIntelligence.UpdateScreenData(aid, buffer.Cast<UInt16>().ToArray(), si.SampleIntervalByus);
            var ach = (AnalogModel)DsoModel.Default.GetChannel(aid);
            var persisitType = DsoModel.Default.Display.Persist;

            var xscale = (si.HdMessage?.Timebase?.TmbScale ?? ach.Sampling.Scale) * DsoModel.Default.Timebase.ZoomScaleX;
            var wfmtmbposindex = (Single)((DsoModel.Default.Timebase.PosIndex - (DsoModel.Default.Timebase.ZoomCenterX - Constants.MAX_XPOS_IDX * DsoModel.Default.Timebase.ZoomScaleX / 2)) / DsoModel.Default.Timebase.ZoomScaleX);
            var wfmtimeposition = (wfmtmbposindex - Constants.MAX_XPOS_IDX / 2) / Constants.IDX_PER_XDIV * xscale;

            var probegain = si.HdMessage?.Analog?[(Int32)aid]?.ProbeGain ?? ach.Conditioning.ProbeGain;
            probegain *= ach.Conditioning.ProbeUnitRatio;
            var prop = new WfmProperties(ach.Name)
            {
                ChnlPosition = (TriggerModel.State == SysState.Stop ? ach.Conditioning.PosIndex : si.HdMessage?.Analog?[(Int32)aid]?.PositionIndex ?? ach.Conditioning.PosIndex, (TriggerModel.State == SysState.Stop ? (ach.Conditioning.Position / probegain) * probegain : si.HdMessage?.Analog?[(Int32)aid]?.Position ?? (ach.Conditioning.Position / probegain)) * probegain),
                //ChnlScale = (si.HdMessage?.Analog?[(Int32)aid]?.ScaleIndex ?? (Int32)ach.Conditioning.ScaleIndex, (si.HdMessage?.Analog?[(Int32)aid]?.Scale ?? (ach.Conditioning.Scale / probegain)) * probegain),
                ChnlScale = (si.HdMessage?.Analog?[(Int32)aid]?.ScaleIndex ?? (Int32)ach.Conditioning.ScaleIndex, TriggerModel.State == SysState.Stop ? (ach.Conditioning.ScaleBymV / probegain) * probegain : (si.HdMessage?.Analog?[(Int32)aid]?.ScaleBymV ?? (ach.Conditioning.ScaleBymV / probegain)) * probegain),

                ChnlUnit = (ach.Conditioning.Prefix, ach.Conditioning.Unit),
                ChnlBias = (si.HdMessage?.Analog?[(Int32)aid]?.Bias ?? (ach.Conditioning.BiasByuV / probegain)) / 1000.0 * probegain,

                TmbPosition = (wfmtmbposindex, wfmtimeposition),
                TmbScale = (si.HdMessage?.Timebase?.TmbScaleIndex ?? (Int32)ach.Sampling.ScaleIndex, si.HdMessage?.Timebase?.TmbScale ?? ach.Sampling.Scale),
                TmbUnit = (ach.Sampling.Prefix, ach.Sampling.Unit),

                SampInterval = si.SampleIntervalByus * 1E-6,
                TrigErrorTime = si.TrigErrorTime,
                VuStartIndex = (-si.StartTimeByus) / ((/*si.HdMessage?.Timebase?.TmbScale ?? */ach.Sampling.Scale) * DsoModel.Default.Timebase.ZoomScaleX) * Constants.IDX_PER_XDIV,// (-si.StartTimeByus - xscale * 10) / ((si.HdMessage?.Timebase?.TmbScale ?? ach.Sampling.Scale) * DsoModel.Default.Timebase.ZoomScaleX) * Constants.IDX_PER_XDIV + wfmtmbposindex,
            };

            _Averager.MaxCnts = ach.Sampling.AverageCnt;
            _Evlp.MaxCnts = ach.Sampling.EnvelopeCnt;
            ach.LastBuffer = ach.Pack?.Buffer?.ToEnumerable() ?? Enumerable.Empty<Double>();

            if (init)
            {
                _Averager.Reset();
                _Evlp.Reset();
            }

            if (buffer is null)
            {
                buffer = new UInt16[0, 0];
            }

            var pos0 = prop.ChnlPosition.Index / Constants.IDX_PER_YDIV * Constants.SAMPS_PER_YDIV + Constants.MAX_ADC_RES / 2;
            var coupling = si.HdMessage?.Analog?[(Int32)aid]?.Coupling ?? ach.Conditioning.Coupling;
            var isinverted = ach.Conditioning.IsInverted;//Dirver未做反相操作
            var mode = si.HdMessage?.Timebase?.AcqMode ?? ach.Sampling.Mode;


            if (DsoModel.Default.ChannelAdcDatas.TryGetValue(aid, out var channelData))
            {
                if (DsoModel.Default.ChannelAdcDatas[aid].GetLength(1) != buffer.GetLength(1))
                {
                    DsoModel.Default.ChannelAdcDatas[aid] = new UInt16[1, buffer.GetLength(1)];
                }
                if (buffer.Length > 0)
                {
                    Unsafe.CopyBlock(ref Unsafe.As<UInt16, Byte>(ref DsoModel.Default.ChannelAdcDatas[aid][0, 0]), ref Unsafe.As<UInt16, Byte>(ref buffer[0, 0]), (UInt32)(Unsafe.SizeOf<UInt16>() * buffer.GetLength(1)));
                }
            }

            return new Context(ach.LastBuffer, prop, buffer, pos0, coupling, isinverted, mode, ct)
            {
                EnvelopeOpt = ach.Sampling.EnvelopOpt,
                PersistType = persisitType,
                Persistent = ach.Persistent,
            };
        }

        public (Double[,], Object)? Read(Object? arg)
        {
            if (arg == null)
                return null;
            var ctx = (Context)arg!;

            return (ctx.Buffer.Select(o => (Double)o), ctx.Properties);
        }
        Boolean reset = false;

        /// <summary>
        /// 软件完全执行反相操作
        /// </summary>
        /// <param name="i"></param>
        /// <param name="ctx"></param>
        /// <param name="buffer"></param>
        private void InvertProcess_OnlySoft(Int32 i, Context ctx, Double[,] buffer)
        {
            Double temp = ctx.Pos0ByAdc * 2;
            for (Int32 j = 0; j < buffer.GetLength(1); j++)
            {
                buffer[i, j] = temp - buffer[i, j];
            }
        }

        public WfmPack Process((Double[,] Buffer, Object Prop) pkg, Object? arg)
        {
            //if (reset == true)
            //{
            //    reset = false;
            //    return null;
            //}
            var ctx = (Context)arg!;
            ChannelId channelId = ctx.Properties.Name switch
            {
                "C2" => ChannelId.C2,
                "C3" => ChannelId.C3,
                "C4" => ChannelId.C4,
                _ => ChannelId.C1,
            };
            var ach = (AnalogModel)DsoModel.Default.GetChannel(channelId);
            Boolean isSimulator = (DsoModel.DataSrcOpt == DataSourceOpt.Simulator);
            (Boolean bNeedInvertProcess, Action<Int32, Context, Double[,]>? Processor) invertProcess = (isSimulator, ach.Conditioning.IsInverted, ctx.Inverted) switch
            {
                (true, true, true) => (true, InvertProcess_OnlySoft),
                (true, false, true) => (true, InvertProcess_OnlySoft),
                (true, _, _) => (false, null),
                _ => (false, null),
            };
            for (Int32 i = 0; i < pkg.Buffer.GetLength(0); i++)
            {
                if (ctx.Coupling == AnaChnlCoupling.Gnd)
                {
                    for (Int32 j = 0; j < pkg.Buffer.GetLength(1); j++)
                    {
                        pkg.Buffer[i, j] = ctx.Pos0ByAdc;
                    }
                }
                else if (invertProcess.bNeedInvertProcess)
                {
                    invertProcess.Processor?.Invoke(i, ctx, pkg.Buffer);
                }

                Int32 clipposcount = 0;
                Int32 clipnegcount = 0;
                Clipping clipping = Clipping.None;

                for (Int32 j = 0; j < pkg.Buffer.GetLength(1); j++)
                {
                    if (pkg.Buffer[i, j] > Constants.MAX_ADC_RES)
                    {
                        pkg.Buffer[i, j] = Constants.MAX_ADC_RES;
                    }
                    else if (pkg.Buffer[i, j] < 0)
                    {
                        pkg.Buffer[i, j] = 0;
                    }

                    if (clipping == Clipping.None)//判定波形是否超出屏幕
                    {
                        if (clipposcount < ctx.Properties.ClippingThreshold)
                        {
                            if (pkg.Buffer[i, j] >= Constants.VIS_MAX_ADC)
                            {
                                clipposcount++;
                            }
                            else
                            {
                                clipposcount = 0;
                            }
                        }

                        if (clipnegcount < ctx.Properties.ClippingThreshold)
                        {
                            if (pkg.Buffer[i, j] <= Constants.VIS_MIN_ADC)
                            {
                                clipnegcount++;
                            }
                            else
                            {
                                clipnegcount = 0;
                            }
                        }
                    }

                    pkg.Buffer[i, j] = (pkg.Buffer[i, j] - ctx.Pos0ByAdc) / Constants.SAMPS_PER_YDIV * ctx.Properties.ChnlScale.Value + ctx.Properties.ChnlBias;
                }

                if (clipposcount >= ctx.Properties.ClippingThreshold || clipnegcount >= ctx.Properties.ClippingThreshold)
                {
                    if (clipposcount >= ctx.Properties.ClippingThreshold && clipnegcount >= ctx.Properties.ClippingThreshold)
                    {
                        clipping = Clipping.Both;
                    }
                    else if (clipposcount >= ctx.Properties.ClippingThreshold)
                    {
                        clipping = Clipping.Pos;
                    }
                    else if (clipnegcount >= ctx.Properties.ClippingThreshold)
                    {
                        clipping = Clipping.Neg;
                    }
                }

                ctx.Properties.Clipping = clipping;

                if (DsoModel.Default.Timebase.IsScan == false)
                {
                    switch (ctx.Mode)
                    {
                        case AnaChnlAcqMode.Average:
                            pkg.Buffer = _Averager.Run(pkg.Buffer, 0);
                            break;
                        case AnaChnlAcqMode.Envelope:
                            if (pkg.Buffer.Length > 0)
                            {
                                pkg.Buffer = _Evlp.Run(pkg.Buffer, ctx.EnvelopeOpt);
                            }
                            break;
                    }
                }
            }

            if (Dispatcher.IsScan && Dispatcher.IsRunning)
            {
                var length = (Int32)Math.Round(ctx.Properties.TmbScale.Value * 1E-6 * Constants.VIS_XDIVS_NUM / ctx.Properties.SampInterval, MidpointRounding.AwayFromZero);
                var buffer = ctx.LastBuffer.Concat(pkg.Buffer.ToEnumerable()).TakeLast(length).ToArray();
                //pkg.Buffer = buffer.ToMatrix(1, buffer.Count());

                var tempbuffer = new Double[1, buffer.Count()];
                if (buffer.Length > 0)
                {
                    Unsafe.CopyBlock(ref Unsafe.As<Double, Byte>(ref tempbuffer[0, 0]), ref Unsafe.As<Double, Byte>(ref buffer[0]), (UInt32)(Unsafe.SizeOf<Double>() * buffer.Count()));
                }
                pkg.Buffer = tempbuffer;

                var start = (length - pkg.Buffer.GetLength(1)) * Constants.MAX_XPOS_IDX / length;
                ctx.Properties.VuStartIndex = start;
            }
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

            return new WfmPack(pkg.Buffer, 0, pkg.Buffer.GetLength(1), (WfmProperties)pkg.Prop);
        }

        public DataSrcFifo(Int32 length)
        {
            _Averager = new(1, length);
            _Evlp = new(1, length);
        }

    }
}

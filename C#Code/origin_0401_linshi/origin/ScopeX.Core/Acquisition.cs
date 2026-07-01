using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using ScopeX.MathExt;
using System.ComponentModel.DataAnnotations;
using ScopeX.Core.Decode;
using NPOI.POIFS.Properties;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ScopeX.Core
{
    internal class Acquisition
    {

        internal Dictionary<AcqDataType, Double> _HardwareSampeInterval = new Dictionary<AcqDataType, Double>();

        private SysState _TrigState = SysState.Triged;

        internal Boolean ReadWfm(Boolean bStoped, Boolean bNeedReset, CancellationToken token, CancellationToken? softResetToken)
        {
            //SysState ss = SysState.Armed;

            if (DsoModel.DataSrcOpt == DataSourceOpt.Simulator)
            {
                if (Dispatcher.IsScan)
                {
                    TriggerModel.State = SysState.Scan;
                }
                else if (TriggerModel.Mode != TriggerMode.OneShot)
                {
                    TriggerModel.State = SysState.Auto;
                }
                else
                {
                    TriggerModel.State = SysState.Stop;
                }

                //TriggerModel.State = ss;
                return true;
            }

            var res = Hd.AcqWave(bStoped, bNeedReset, AllChnlReadInfo, ref _HardwareSampeInterval, /*softResetToken*/null);

            if (res && _HardwareSampeInterval.ContainsKey(AcqDataType.AnalogChannel))
            {
                DsoModel.Default.Timebase.AnalogSamplingRate = 1 / (_HardwareSampeInterval[AcqDataType.AnalogChannel] * 1E-6);
            }

            if (Dispatcher.IsScan)
            {
                TriggerModel.State = SysState.Scan;
            }
            else
            {
                if ((Hd.TrigState & 0x02) != 0)
                {
                    if (TriggerModel.Mode == TriggerMode.OneShot && res)
                    {
                        TriggerModel.State = SysState.Stop;
                    }
                    else
                    {
                        TriggerModel.State = SysState.Triged;
                        _TrigState = SysState.Triged;
                    }
                }
                if ((Hd.TrigState & 0x01) != 0)
                {
                    TriggerModel.State = SysState.Auto;
                    _TrigState = SysState.Auto;
                }
                if ((Hd.TrigState & 0x4) != 0)
                {
                    if (_TrigState != SysState.Auto || TriggerModel.Mode != TriggerMode.Auto)
                        TriggerModel.State = SysState.Ready;
                }
                if (Hd.TrigState==0)
                {
                    if (_TrigState != SysState.Auto || TriggerModel.Mode != TriggerMode.Auto)
                        TriggerModel.State = SysState.Armed;
                }

             
            }
            var tempinfo = Hd.GetParamters("System", "TempInfo", null);
            if (tempinfo != null && tempinfo is Dictionary<String, Double>)
            {
                DsoModel.Default.TempCtrl.UpdateTemp((Dictionary<String, Double>)tempinfo);
            }
            //if (DsoPrsnt.DefaultDsoPrsnt.VisualTrigger.Enabled && DsoPrsnt.DefaultDsoPrsnt.Timebase.StorageMode == AnaChnlStorageMode.Fast)
            //{
            //    var ct = DsoPrsnt.DefaultDsoPrsnt.VisualTrigger.SelectedItems.Count(c => c.Enabled);
            //    if (ct > 0 && TriggerModel.State == SysState.Triged)
            //    {
            //        TriggerModel.State = Hd.RegionTrigStatus == 1 ? SysState.Triged : SysState.Ready;
            //    }
            //}

            token.ThrowIfCancellationRequested();

            return res;
        }

        public Boolean CalcMathWfm(Boolean wfmTaken, Boolean modelUpdated, Int64 stamp, CancellationToken token)
        {
            if (!wfmTaken && !modelUpdated)
            {
                return true;
            }

            var activemaths = DsoModel.Default.MathChnls.Where(x => x.Active);
            if (activemaths.Count() <= 0)
            {
                return true;
            }

            Int32 count = 0;
            foreach (var mch in activemaths)
            {
                if (mch.Active)
                {
                    try
                    {
                        if (stamp < mch.Pack?.Properties.Stamp)
                        {
                            count++;
                        }
                        else if (mch.Take(modelUpdated, token))
                        {
                            count++;
                        }
                    }
                    catch (Exception ex)
                    {
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(new Object(), new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
                    }
                }
                else
                {
                    count++;
                }
            }
            return count >= ChannelId.M8 - ChannelId.M1 + 1;
        }

        public void PrepareMathSrcWfm()
        {
            //lock (Locker)
            {
                foreach (var ach in DsoModel.Default.AnalogChnls)
                {
                    if (!DsoModel.Default.Timebase.IsScan && ach.Active)
                    {
                        var pack = ach.DeepClonePack();
                        if (pack is not null)
                        {
                            MathVecBuffer.Default.Provide(pack.Properties.Name, new Vector(pack.Buffer.Select(o => o * 1E-3), pack.Properties.TmbUnit.Name, pack.Properties.ChnlUnit.Name, pack.Properties.SampInterval, pack.Properties.ChnlScale.Value * 1E-3));
                        }
                        else
                        {
                            MathVecBuffer.Default.Provide(ach.Id.ToString(), new Vector());
                        }
                    }
                    else
                    {
                        MathVecBuffer.Default.Provide(ach.Id.ToString(), new Vector());
                    }
                }

                foreach (var rch in DsoModel.Default.ReferenceChnls)
                {
                    if (!DsoModel.Default.Timebase.IsScan && rch.Active)
                    {
                        var pack = rch.DeepClonePack();
                        if (pack is not null)
                        {
                            MathVecBuffer.Default.Provide(pack.Properties.Name, new Vector(pack.Buffer.Select(o => o * 1E-3), pack.Properties.TmbUnit.Name, pack.Properties.ChnlUnit.Name, pack.Properties.SampInterval, pack.Properties.ChnlScale.Value * 1E-3));
                        }
                        else
                        {
                            MathVecBuffer.Default.Provide(rch.Id.ToString(), new Vector());
                        }
                    }
                    else
                    {
                        MathVecBuffer.Default.Provide(rch.Id.ToString(), new Vector());
                    }
                }

                for (Int32 i = 0; i < DsoModel.Default.Meas.Calc.StatBuffer.Length; i++)
                {
                    if (!DsoModel.Default.Timebase.IsScan && DsoModel.Default.Meas.SelectedItems[i].Active)
                    {
                        var selected = DsoModel.Default.Meas.SelectedItems[i];
                        WfmPack? pack = null;
                        if (selected.MeasureType == MeasureType.Single && DsoModel.Default.TryGetChannel(selected.Source, out var src))
                        {
                            if (selected.Active)
                            {
                                pack = src.DeepClonePack();
                                if (pack is not null)
                                {
                                    var data = DsoModel.Default.Meas.Calc.StatBuffer[i].ToArray();
                                    var (pfx, name) = DsoModel.Default.Meas.Calc.GetPfxUnitString(i);
                                    var vec = new Vector(pfx == Prefix.Milli ? data?.Multiply_(1E-3) : data?.Multiply_(1E-6), src.Sampling.Unit, name, pack == null ? 1 : pack.Properties.SampInterval)
                                    {
                                        Calc = () => DsoModel.Default.Meas.Calc.GetTrack(selected),
                                    };
                                    MathVecBuffer.Default.Provide($"P{i + 1}", vec);
                                }
                                else
                                {
                                    MathVecBuffer.Default.Provide($"P{i + 1}", new Vector());
                                }
                            }
                            else
                            {
                                MathVecBuffer.Default.Provide($"P{i + 1}", new Vector());
                            }
                        }
                        else if (selected.MeasureType == MeasureType.Composite)
                        {
                            var data = DsoModel.Default.Meas.Calc.StatBuffer[i].ToArray();
                            var (pfx, name) = DsoModel.Default.Meas.Calc.GetPfxUnitString(i);
                            var vec = new Vector(data?.Multiply_(1E-3), String.Empty, name, 1)
                            {
                                Calc = () => DsoModel.Default.Meas.Calc.GetTrack(selected),
                            };
                            MathVecBuffer.Default.Provide($"P{i + 1}", vec);
                        }
                    }
                    else
                    {
                        MathVecBuffer.Default.Provide($"P{i + 1}", new Vector());
                    }
                }


                if (DsoModel.Default.Voltmeter?.Active == true)
                {
                    if (!DsoModel.Default.Timebase.IsScan && DsoModel.Default.TryGetChannel(DsoModel.Default.Voltmeter.Source, out var src))
                    {
                        if (DsoModel.Default.Voltmeter!.Active)
                        {
                            var pack = src.DeepClonePack();
                            if (pack is not null)
                            {
                                var data = DsoModel.Default.Voltmeter.StaBuffer.ToArray();
                                var (pfx, name) = (Prefix.Milli, DsoModel.Default.Voltmeter.Unit);
                                var vec = new Vector(data?.Multiply_(1E-3), src.Sampling.Unit, name, pack == null ? 1 : pack.Properties.SampInterval)
                                {
                                    Calc = () =>
                                    {
                                        return (new List<Double>() { 0, 0 }, new List<Double>() { DsoModel.Default.Voltmeter.StaBuffer.Current });
                                    },
                                };
                                MathVecBuffer.Default.Provide(nameof(ChannelId.DVM), vec);
                            }
                            else
                            {
                                MathVecBuffer.Default.Provide(nameof(ChannelId.DVM), new Vector());
                            }
                        }
                        else
                        {
                            MathVecBuffer.Default.Provide(nameof(ChannelId.DVM), new Vector());
                        }
                    }
                }
                else
                {
                    MathVecBuffer.Default.Provide(nameof(ChannelId.DVM), new Vector());
                }


                if (DsoModel.Default.Cymometer?.Active == true && DsoModel.Default.Cymometer?.Source != null)
                {
                    if (!DsoModel.Default.Timebase.IsScan && DsoModel.Default.TryGetChannel(DsoModel.Default.Cymometer.Source.Value, out var src))
                    {
                        if (DsoModel.Default.Cymometer!.Active)
                        {
                            var pack = src.DeepClonePack();
                            if (pack is not null)
                            {
                                var data = DsoModel.Default.Cymometer!.StaBuffer.ToArray();
                                var (pfx, name) = (Prefix.Empty, DsoModel.Default.Cymometer!.Unit);
                                var vec = new Vector(data?.Multiply_(1E-3), src.Sampling.Unit, name, pack == null ? 1 : pack.Properties.SampInterval)
                                {
                                    Calc = () =>
                                    {
                                        return (new List<Double>() { 0, 0 }, new List<Double>() { DsoModel.Default.Cymometer!.CurrentCym });
                                    },
                                };
                                MathVecBuffer.Default.Provide(nameof(ChannelId.CYM), vec);
                            }
                            else
                            {
                                MathVecBuffer.Default.Provide(nameof(ChannelId.CYM), new Vector());
                            }
                        }
                        else
                        {
                            MathVecBuffer.Default.Provide(nameof(ChannelId.CYM), new Vector());
                        }
                    }
                }


                foreach (var dch in DsoModel.Default.DigitalChnls)
                {
                    if (!DsoModel.Default.Timebase.IsScan && dch.Active)
                    {
                        var pack = dch.DeepClonePack();
                        if (pack is not null)
                        {
                            MathVecBuffer.Default.Provide("D", new Vector(pack.Buffer, pack.Properties.TmbUnit.Name, pack.Properties.ChnlUnit.Name, pack.Properties.SampInterval, pack.Properties.ChnlScale.Value * 1E-3));
                        }
                        else
                        {
                            MathVecBuffer.Default.Provide(dch.Id.ToString(), new Vector());
                        }
                    }
                    else
                    {
                        MathVecBuffer.Default.Provide(dch.Id.ToString(), new Vector());
                    }
                }
            }
        }

        public List<ReadInfo> AllChnlReadInfo = new();

        public void UpdateReadInfoList()
        {
            AllChnlReadInfo = TakeAnalogReadInfoList();
        }
        public void ClearAnalogBuffer()
        {
            lock (Locker)
            {
                foreach (var ach in DsoModel.Default.AnalogChnls)
                {
                    ach.WfmDpx = new Byte[0];
                    ach.ClearBuffer();
                }
                foreach (var dch in DsoModel.Default.DigitalChnls)
                {
                    dch.ClearBuffer();
                }
                for (Int32 i = 0; i < ChannelIdExt.BusChnlNum; i++)
                {
                    DecodeDataSource.Instance.AnalogDataSources[i].HasData = false;

                }
                if (DsoModel.Default.DecodeChnls.Count(x => x.Active) > 0)
                {
                    foreach (var val in DsoModel.Default.DecodeChnls.Where(x => x.Active))
                    {
                        val.ClearBuffer();
                    }
                }
            }
        }
        private Boolean _LastRunStatus = true;

        private List<ReadInfo> TakeAnalogReadInfoList()
        {
            Boolean isrun = TriggerModel.State != SysState.Stop;
            List<ReadInfo> chnlinfo = new();

            #region 显示
            //WfmPkgInfo viewpkg = new(DsoModel.Default.Timebase.NeedWaveDotsCnt, DsoModel.Default.Timebase.Scale * Constants.VIS_XDIVS_NUM, DsoModel.Default.Timebase.Scale * Constants.VIS_XDIVS_NUM / 2 - DsoModel.Default.Timebase.Position);
            //StartTimeByus 应该是触发位置离屏幕中心（也就是长存储DDR的存储深度的一半的位置）的时间。向左为正
            WfmPkgInfo viewpkg = new(DsoModel.Default.Timebase.TakeViewWaveDotsCnt, DsoModel.Default.Timebase.Scale * Constants.VIS_XDIVS_NUM, DsoModel.Default.Timebase.Position);
            ReadInfo viewinfo = new(AcqDataType.AnalogChannel, ChannelIdExt.GetAnalogs().ToList(), viewpkg, nameof(DataRole.View));
            chnlinfo.Add(viewinfo);
            WfmPkgInfo aipkg = new(10000, DsoModel.Default.Timebase.Scale * Constants.VIS_XDIVS_NUM, DsoModel.Default.Timebase.Position);
            ReadInfo aiinfo = new(AcqDataType.AnalogChannel, ChannelIdExt.GetAnalogs().ToList(), aipkg, nameof(DataRole.Ai));
            chnlinfo.Add(aiinfo);
            if (Constants.ENABLE_SDA && DsoModel.Default.JitterModel.Active)
            {
                var datalength = DsoModel.Default.Timebase.AnaChnlLengthSource[DsoModel.Default.Timebase.StorageDepthOpt].Value;
                if (datalength > DsoModel.Default.JitterModel.MaxDataLength)
                {
                    datalength = DsoModel.Default.JitterModel.MaxDataLength;
                }
                WfmPkgInfo sdapkg = new(datalength, DsoModel.Default.Timebase.Scale * Constants.VIS_XDIVS_NUM, DsoModel.Default.Timebase.Position);
                viewinfo = new(AcqDataType.AnalogChannel, new List<ChannelId>() { DsoModel.Default.JitterModel.Source }, sdapkg, nameof(DataRole.Sda));
                chnlinfo.Add(viewinfo);
            }

            if (Constants.ENABLE_VSA && DsoModel.Default.VectorAnalysisModel.Enabled)
            {
                var datalength = DsoModel.Default.Timebase.AnaChnlLengthSource[DsoModel.Default.Timebase.StorageDepthOpt].Value;
                if (datalength > DsoModel.Default.VectorAnalysisModel.DataLength)
                {
                    datalength = (int)DsoModel.Default.VectorAnalysisModel.DataLength;
                }
                WfmPkgInfo sdapkg = new(datalength, DsoModel.Default.Timebase.Scale * Constants.VIS_XDIVS_NUM, DsoModel.Default.Timebase.Position);
                ReadInfo svsViewinfo = new(AcqDataType.AnalogChannel, new List<ChannelId>() { DsoModel.Default.JitterModel.Source }, sdapkg, nameof(DataRole.Vsa));
                chnlinfo.Add(svsViewinfo);
            }

            //if (DsoModel.Default.Timebase.StorageMode == AnaChnlStorageMode.Fast && (!DsoModel.Default.Timebase.IsScan || TriggerModel.State == SysState.Stop))
            //{
            //    WfmPkgInfo viewpkgfast = new(DsoModel.Default.Timebase.TakeViewWaveDotsCnt, DsoModel.Default.Timebase.Scale * Constants.VIS_XDIVS_NUM, DsoModel.Default.Timebase.Position);
            //    ReadInfo viewinfofast = new(AcqDataType.DPX, new List<ChannelId>() { ChannelId.C1 }, viewpkgfast, nameof(DataRole.View));
            //    chnlinfo.Add(viewinfofast);
            //}
            if ((_LastRunStatus && !isrun) || Decode.DecodeProtocolShareParameter.Default.NeedReadDecodeData || isrun)
            {
                var buses = DsoModel.Default.DecodeChnls.Cast<Decode.DecodeModel>().Where(x => x.Active && x.ProtocolType != SerialProtocolType.Close);
                foreach (var bus in buses)
                {
                    WfmPkgInfo viewpkgfast = new(DsoModel.Default.Timebase.TakeViewWaveDotsCnt, DsoModel.Default.Timebase.Scale * Constants.VIS_XDIVS_NUM, DsoModel.Default.Timebase.Position);
                    chnlinfo.Add(new ReadInfo(AcqDataType.Decode, new List<ChannelId>() { ChannelId.C1 }, viewpkgfast, nameof(DataRole.View)) { ExtInfo = bus.Id.ToString() });
                }


            }

            var channel = DsoModel.Default.GetChannel(ChannelId.C1);
            if (DsoModel.Default.Timebase.IsZoom)
            {
                if (!(Dispatcher.IsScan && Dispatcher.IsRunning))
                {
                    var time = DsoModel.Default.Timebase.Scale * Constants.VIS_XDIVS_NUM * DsoModel.Default.Timebase.ZoomScaleX;
                    //var num = DsoModel.Default.Timebase.AnalogSamplingRate* time*10e-6;

                    WfmPkgInfo viewpkgzoom = new(DsoModel.Default.Timebase.TakeViewWaveDotsCnt, time, DsoModel.Default.Timebase.Position + (DsoModel.Default.Timebase.ZoomCenterX - 5000) / 1000.0 * DsoModel.Default.Timebase.Scale);
                    ReadInfo viewinfozoom = new(AcqDataType.AnalogChannel, ChannelIdExt.GetAnalogs().ToList(), viewpkgzoom, nameof(DataRole.Zoom));
                    chnlinfo.Add(viewinfozoom);
                }
            }
            if (PlatformManager.Default.Platform.IncludeDigitalChnl)
            {
                if (DsoModel.Default.DigitalChnls.Where(o => o.Active).Count() > 0)
                {
                    WfmPkgInfo laviewpkg = viewpkg;
                    ReadInfo laviewinfo = new(AcqDataType.LA, ChannelIdExt.GetDigitals().ToList(), laviewpkg, nameof(DataRole.View));
                    chnlinfo.Add(laviewinfo);
                }
            }
            #endregion
            _LastRunStatus = isrun;
            return chnlinfo;
        }

        public static void CopyFormPackLock(IEnumerable<ChannelModel> chnls)
        {
            lock (UpdateVuTask.PackLocker)
            {
                foreach (var ch in chnls)
                {
                    ch.CopyFormPackLock();
                }
            }
        }

        public void CopyToPackLock(IEnumerable<ChannelModel> chnls)
        {
            lock (UpdateVuTask.PackLocker)
            {
                foreach (var ch in chnls)
                {
                    ch.CopyToPackLock();
                }
            }
        }
        public void UpdateAnalogWfmTime()
        {
            Int64 timestamp = Stopwatch.GetTimestamp();
            lock (UpdateVuTask.PackLocker)
            {
                foreach (var ch in DsoModel.Default.AnalogChnls)
                {
                    if (ch.Pack is null || !ch.Active)
                    {
                        continue;
                    }

                    ch.Pack.Properties.WfmUpdateTime = timestamp;
                }
            }
        }


        public void AssignAnalogWfm(Boolean init, CancellationToken token, CancellationToken? softResetToken = null)
        {
            //DsoModel.Default.Timebase.SegmentUpdate();

            //lock (Locker)
            {
                if (TimeSpanUtility.GetTimestampDateTime(DateTime.MinValue).Subtract(DsoModel.Default.AnalogChPositionUpdateTime).TotalMilliseconds > 250)
                {
                    //目前顺序模式与非顺序模式采集流程还有不一致的地方 暂时分开采用两种方法
                    if (!DsoModel.Default.Timebase.SegmentActive)
                    {
                        Parallel.For(0, DsoModel.Default.AnalogChnls.Count(), chid =>
                        {
                            //if (DsoModel.Default.AnalogChnls.ToList()[chid].Active)
                            {
                                DsoModel.Default.AnalogChnls.ToList()[chid].Take(init, token, softResetToken);
                            }
                        });
                    }
                    else
                    {
                        foreach (var ach in DsoModel.Default.AnalogChnls)
                        {
                            ach.Take(init, token, softResetToken);
                        }
                    }
                }
            }

            if (DsoModel.Default.Timebase.StorageMode == AnaChnlStorageMode.Fast)
            {
                if (Dispatcher.IsScan && TriggerModel.State != SysState.Stop)
                {
                    foreach (var item in DsoModel.Default.AnalogChnls)
                    {
                        item.WfmDpx = new Byte[0];
                    }
                }
                //else
                //{
                //    var buffer = _AuxDpxSource.Read(out var si, out var includeChannels, out UInt32 MainWinMaxHitTimes, out UInt32 MainWinMinHitTimes, out Double RadioOfSoftWaveSampleDivDpxWaveSample);
                //    Int32 num = includeChannels?.Count ?? 0;
                //    foreach (var item in DsoModel.Default.AnalogChnls)
                //    {
                //        item.WfmDpx = buffer;
                //        item.MainWinMaxHitTimes = MainWinMaxHitTimes;
                //        item.MainWinMinHitTimes = MainWinMinHitTimes;

                //        item.DpxCorrection = RadioOfSoftWaveSampleDivDpxWaveSample;
                //        item.DpxChOnCount = num;
                //        if (num == 1)
                //        {
                //            item.DpxChIndex1 = includeChannels[0] - ChannelId.C1;
                //        }
                //        else if (num == 2)
                //        {
                //            item.DpxChIndex1 = includeChannels[0] - ChannelId.C1;
                //            item.DpxChIndex2 = includeChannels[1] - ChannelId.C1;
                //        }
                //        else
                //        {

                //        }
                //    }
                //}
            }
        }

        public void AssignDigitalWfm(Boolean init, CancellationToken token)
        {
            if (!Constants.ENABLE_LA)
                return;

            if (DsoModel.Default.DigitalChnls.Where(o => o.Active).Count() > 0)
            {
                if (TimeSpanUtility.GetTimestampDateTime(DateTime.MinValue).Subtract(DsoModel.Default.DigitalChPositionUpdateTime).TotalMilliseconds > 500)
                {
                    foreach (var dch in DsoModel.Default.DigitalChnls)
                    {
                        dch.Take(init, token);
                    }
                }
            }
        }

        public void AssignSearchResult(Boolean init, CancellationToken token)
        {
            if (!Constants.ENABLE_Search)
                return;
            if (DsoModel.Default.Search.Items.Values.Where(x => x.Active).Count() <= 0)
                return;

            DsoModel.Default.Search.Take(token);
        }

        public void AssignDecodeWfm(Boolean init, CancellationToken token)
        {
            if (!Constants.ENABLE_BUS)
                return;
            if (DsoModel.Default.DecodeChnls.Count(x => x.Active) == 0)
            {
                for (Int32 i = 0; i < ChannelIdExt.BusChnlNum; i++)
                {
                    DecodeDataSource.Instance.AnalogDataSources[i].HasData = false;
                }
                return;
            }
            else
            {
                if (Hd.Decoder == null)
                {
                    for (Int32 i = 0; i < ChannelIdExt.BusChnlNum; i++)
                    {
                        DecodeDataSource.Instance.AnalogDataSources[i].HasData = false;
                    }
                }
                else
                {
                    for (Int32 i = 0; i < ChannelIdExt.BusChnlNum; i++)
                    {
                        DecodeDataSource.Instance.AnalogDataSources[i].HasData = true;
                    }
                    //result = Hd.Decoder.TryTakeData(ChannelId.C1, ref ScopeX.Hardware.Driver.DecodeDataSource.Instance.AnalogDataSource);
                }
            }
            foreach (var bch in DsoModel.Default.DecodeChnls.Cast<Core.Decode.DecodeModel>())
            {
                bch.DecodePacketData();
            }
        }

        public void ClearDecodeData()
        {
            foreach (var bch in DsoModel.Default.DecodeChnls.Cast<Core.Decode.DecodeModel>())
            {
                bch.ClearPacketData();
            }
        }



        public static Object Locker = new();


        public void UpdateVuSample(IEnumerable<ChannelModel> chnls, Int32 length = 0)
        {
            WfmVuBaseParam? tmpwfmparam = null;
            Dictionary<String, (WfmVuBlock?, WfmVuBlock?)?> wvbtable = new();
            CopyFormPackLock(chnls);

            foreach (ChannelModel ch in chnls)
            {
                //if (ch.Active)
                {
                    if (tmpwfmparam == null)
                    {
                        tmpwfmparam = new WfmVuBaseParam(ch.Sampling.Scale, ch.Sampling.PosIndex, ch.Sampling.PosIdxPerDiv);
                    }
                    if (ch is MathModel math)
                    {
                        wvbtable.Add(math.Name, math.MakeVuSamples?.Invoke(math, length, null));
                    }
                    else
                    {
                        wvbtable.Add(ch.Name, ch.MakeVuSamples?.Invoke(ch, length, tmpwfmparam));
                    }
                    if (ch.Id.IsReference())
                    {
                        tmpwfmparam = null;
                    }
                }
            }


            lock (UpdateVuTask.VuLocker)
            {
                foreach (ChannelModel ch in chnls)
                {
                    if (wvbtable.ContainsKey(ch.Name))
                    {
                        ch.VuDatabase.Add(wvbtable[ch.Name]?.Item1);
                        if (ch.Id.IsAnalog() || ch.Id.IsReference())
                        {
                            ch.ZoomVuDatabase.Add(wvbtable[ch.Name]?.Item2);
                        }

                    }
                }
            }
        }

        public static void UpdateMathVuSample(IEnumerable<MathModel> chnls, Int32 length = 0)
        {
            Dictionary<String, WfmVuBlock?> wvbtable = new();
            CopyFormPackLock(chnls);

            foreach (MathModel ch in chnls)
            {
                if (ch.Active)
                {
                    wvbtable.Add(ch.Name, ch.MakeVuSamples?.Invoke(ch, length, null)?.Item1);
                }
            }


            lock (UpdateVuTask.VuLocker)
            {
                foreach (MathModel ch in chnls)
                {
                    if (ch.Active && wvbtable.ContainsKey(ch.Name))
                    {
                        ch.VuDatabase.Add(wvbtable[ch.Name]);
                        ch.VuDatabaseNormal.Add(ch.MakeVuSamplesMathFFT?.Invoke(ch, length, RFWaveType.Normal, null));
                        ch.VuDatabaseAverage.Add(ch.MakeVuSamplesMathFFT?.Invoke(ch, length, RFWaveType.Average, null));
                        ch.VuDatabaseMaxHold.Add(ch.MakeVuSamplesMathFFT?.Invoke(ch, length, RFWaveType.MaxHold, null));
                        ch.VuDatabaseMinHold.Add(ch.MakeVuSamplesMathFFT?.Invoke(ch, length, RFWaveType.MinHold, null));
                    }
                }
            }
        }
        public void UpdateDigitalVuSample(IEnumerable<ChannelModel> chnls, Int32 length = 0)
        {
            foreach (var ch in chnls)
            {
                if (ch.Active)
                {
                    ch.VuDatabase.Add(ch.MakeVuSamples?.Invoke(ch, length, null)?.Item1);
                }
            }
        }

        Acquisition()
        {
            BindDataSource(DsoModel.DataSrcOpt);
            BindRFDataSource(DsoModel.DataSrcOpt);
        }
        static Acquisition()
        {

        }
        private Dictionary<ChannelId, IDataSource> GetAnalogDataSource(DataSourceOpt dataSource)
        {
            Dictionary<ChannelId, IDataSource> source = new Dictionary<ChannelId, IDataSource>();
            if (dataSource == DataSourceOpt.Simulator)
            {
                var config = AppConfig.GetIntance();
                DataSrcSim[] dss = new DataSrcSim[ChannelIdExt.AwgNum];
                Int32 i = 0;
                foreach (var id in ChannelIdExt.GetAWGs())
                {
                    Double si = 4E-6;
                    Int32 len = 10000;
                    if (id == ChannelId.AWG1)
                    {
                        si = config.AWG1SI;
                        len = config.AWG1Len;
                    }
                    else if (id == ChannelId.AWG2)
                    {
                        si = config.AWG2SI;
                        len = config.AWG2Len;
                    }
                    else if (id == ChannelId.AWG3)
                    {
                        si = config.AWG3SI;
                        len = config.AWG3Len;
                    }
                    else if (id == ChannelId.AWG4)
                    {
                        si = config.AWG4SI;
                        len = config.AWG4Len;
                    }
                    else
                    {

                    }

                    dss[i++] = new DataSrcSim(DsoModel.Default.GetWfmGenerator(id), si, len);
                }

                i = 0;
                foreach (var id in ChannelIdExt.GetAnalogs())
                {
                    source[id] = dss[i];
                    i = (i + 1) % dss.Length;
                }
            }
            else
            {
                foreach (var id in ChannelIdExt.GetAnalogs())
                {
                    source[id] = new DataSrcFifo(Constants.CHNL_DATA_NUM);
                }
            }
            return source;
        }
        public static Acquisition Default { get; } = new Acquisition();
        public void BindDataSource(DataSourceOpt source)
        {
            BindAnalogDataSource(GetAnalogDataSource(source));
            BindDigitalDataSource(source == DataSourceOpt.Simulator ? new DataSrcDigiSim() : new DataSrcDigi());
            //var dds = source == DataSourceOpt.Simulator ? _SimDigiSource : _DigiSource;
            BindDecodeDataSource(source == DataSourceOpt.Simulator ? new DataSrcDecodeSim(SerialProtocolType.RS232) : new DataSrcDecode());

            //var dcs = source == DataSourceOpt.Simulator ? _SimDecodeSource : _DecodeSource;


        }
        private void BindDecodeDataSource(IDataSource dcs)
        {
            foreach (var dch in DsoModel.Default.DecodeChnls)
            {
                dch.PrepareSamples = dcs.Prepare;
                dch.ReadSamples = dcs.Read;
                dch.ProcessSamples = dcs.Process;

                dch.MakeVuSamples = WfmVuDatabase.RescaleBitSeq;
            }
        }
        private void BindDigitalDataSource(IDataSource dds)
        {
            foreach (var dch in DsoModel.Default.DigitalChnls)
            {
                dch.PrepareSamples = dds.Prepare;
                dch.ReadSamples = dds.Read;
                dch.ProcessSamples = dds.Process;

                dch.MakeVuSamples = WfmVuDatabase.RescaleBitSeq;
            }

        }
        private void BindAnalogDataSource(Dictionary<ChannelId, IDataSource> ads)
        {
            foreach (var ach in DsoModel.Default.AnalogChnls)
            {
                ach.PrepareSamples = ads[ach.Id].Prepare;
                ach.ReadSamples = ads[ach.Id].Read;
                ach.ProcessSamples = ads[ach.Id].Process;

                ach.MakeVuSamples = WfmVuDatabase.Rescale;
            }

        }
        //private  readonly Dictionary<ChannelId, IDataSource> _SimSource = new();
        //private  readonly Dictionary<ChannelId, IDataSource> _FifoSource = new();
        private AuxDataSrcDPX _AuxDpxSource = new Lazy<AuxDataSrcDPX>().Value;

        //private IDataSource _SimDigiSource = new Lazy<DataSrcDigiSim>().Value;
        //private IDataSource _DigiSource = new Lazy<DataSrcDigi>().Value;
        //private IDataSource _SimDecodeSource = new Lazy<DataSrcDecodeSim>(()=>new DataSrcDecodeSim(SerialProtocolType.RS232)).Value;
        //private IDataSource _DecodeSource = new Lazy<DataSrcDecode>().Value;

        public void AssignRadioFrequencyWfm(Boolean init, CancellationToken token)
        {
            foreach (var rfch in DsoModel.Default.RadioFrequencyChnls)
            {
                if (rfch.Active)
                {
                    rfch.Take(init, token);
                }
            }
            foreach (var rfch in DsoModel.Default.RFAmpVSTimeChnls)
            {
                if (rfch.Active)
                {
                    rfch.Take(init, token);
                }
            }
            foreach (var rfch in DsoModel.Default.RFPhaseVSTimeChnls)
            {
                if (rfch.Active)
                {
                    rfch.Take(init, token);
                }
            }
            foreach (var rfch in DsoModel.Default.RFPhaseVSFrequencyChnls)
            {
                if (rfch.Active)
                {
                    rfch.Take(init, token);
                }
            }
            foreach (var rfch in DsoModel.Default.RFFrequencyVSTimeChnls)
            {
                if (rfch.Active)
                {
                    rfch.Take(init, token);
                }
            }
            foreach (var rfch in DsoModel.Default.RFTimeVSFrequencyChnls)
            {
                if (rfch.Active)
                {
                    rfch.Take(init, token);
                }
            }
        }

        public void UpdateRFVuSample(IEnumerable<RadioFrequencyModel> chnls, Int32 length = 0)
        {
            foreach (var ch in chnls)
            {
                if (ch.Active)
                {
                    ch.VuDatabase.Add(ch.MakeVuSamples?.Invoke(ch, length, null)?.Item1);

                    ch.VuDatabaseNormal.Add(ch.MakeVuSamplesIQFFT?.Invoke(ch, MDVirticalType.Amplitude, RFWaveType.Normal));
                    ch.VuDatabaseAverage.Add(ch.MakeVuSamplesIQFFT?.Invoke(ch, MDVirticalType.Amplitude, RFWaveType.Average));
                    ch.VuDatabaseMaxHold.Add(ch.MakeVuSamplesIQFFT?.Invoke(ch, MDVirticalType.Amplitude, RFWaveType.MaxHold));
                    ch.VuDatabaseMinHold.Add(ch.MakeVuSamplesIQFFT?.Invoke(ch, MDVirticalType.Amplitude, RFWaveType.MinHold));
                }
            }
            foreach (var ch in DsoModel.Default.RFAmpVSTimeChnls)
            {
                ch.VuDatabase.Add(ch.MakeVuSamplesIQ?.Invoke(ch, MDVirticalType.Amplitude));
            }
            foreach (var ch in DsoModel.Default.RFPhaseVSTimeChnls)
            {
                ch.VuDatabase.Add(ch.MakeVuSamplesIQ?.Invoke(ch, MDVirticalType.Phase));
            }
            foreach (var ch in DsoModel.Default.RFPhaseVSFrequencyChnls)
            {
                ch.VuDatabase.Add(ch.MakeVuSamplesIQFFT?.Invoke(ch, MDVirticalType.Phase, RFWaveType.Normal));
            }
            foreach (var ch in DsoModel.Default.RFFrequencyVSTimeChnls)
            {
                ch.VuDatabase.Add(ch.MakeVuSamplesIQ?.Invoke(ch, MDVirticalType.Frequency));
            }
            foreach (var ch in DsoModel.Default.RFTimeVSFrequencyChnls)
            {
                ch.VuDatabase.Add(ch.MakeVuSamplesIQFFT?.Invoke(ch, MDVirticalType.Time, RFWaveType.Normal));
            }
        }


        private Dictionary<ChannelId, IRFDataSource> GetTimeVSFrequenciesDataSource(DataSourceOpt source)
        {
            Dictionary<ChannelId, IRFDataSource> dds = new Dictionary<ChannelId, IRFDataSource>();
            Boolean status = source != DataSourceOpt.Simulator;
            foreach (var id in ChannelIdExt.GetTimeVSFrequencies())
            {
                dds[id] = status ? new DataSrcIQFFT() : new DataSrcIQFFTSim(DsoModel.Default.GetWfmGenerator(ChannelId.AWG1),
                    Double.Parse(AppConfigureHelper.AppSettings[id.ToString() + "SI"]?.ToString() ?? "4E-6"),
                    Int32.Parse(AppConfigureHelper.AppSettings[id.ToString() + "Len"]?.ToString() ?? "10000"));
            }
            return dds;
        }
        private Dictionary<ChannelId, IRFDataSource> GetFrequencyVSTimesDataSource(DataSourceOpt source)
        {
            Dictionary<ChannelId, IRFDataSource> dds = new Dictionary<ChannelId, IRFDataSource>();
            Boolean status = source != DataSourceOpt.Simulator;
            foreach (var id in ChannelIdExt.GetFrequencyVSTimes())
            {
                dds[id] = status ? new DataSrcIQ() : new DataSrcIQSim(DsoModel.Default.GetWfmGenerator(ChannelId.AWG1),
                    Double.Parse(AppConfigureHelper.AppSettings[id.ToString() + "SI"]?.ToString() ?? "4E-6"),
                    Int32.Parse(AppConfigureHelper.AppSettings[id.ToString() + "Len"]?.ToString() ?? "10000"));
            }
            return dds;
        }
        private Dictionary<ChannelId, IRFDataSource> GetPhaseVSFrequenciesChnlDataSource(DataSourceOpt source)
        {
            Dictionary<ChannelId, IRFDataSource> dds = new Dictionary<ChannelId, IRFDataSource>();
            Boolean status = source != DataSourceOpt.Simulator;
            foreach (var id in ChannelIdExt.GetPhaseVSFrequencies())
            {
                dds[id] = status ? new DataSrcIQFFT() : new DataSrcIQFFTSim(DsoModel.Default.GetWfmGenerator(ChannelId.AWG1),
                    Double.Parse(AppConfigureHelper.AppSettings[id.ToString() + "SI"]?.ToString() ?? "4E-6"),
                    Int32.Parse(AppConfigureHelper.AppSettings[id.ToString() + "Len"]?.ToString() ?? "10000"));
            }
            return dds;
        }

        private Dictionary<ChannelId, IRFDataSource> GetPhaseVSTimeChnlDataSource(DataSourceOpt source)
        {
            Dictionary<ChannelId, IRFDataSource> dds = new Dictionary<ChannelId, IRFDataSource>();
            Boolean status = source != DataSourceOpt.Simulator;
            foreach (var id in ChannelIdExt.GetPhaseVSTimes())
            {
                dds[id] = status ? new DataSrcIQ() : new DataSrcIQSim(DsoModel.Default.GetWfmGenerator(ChannelId.AWG1),
                    Double.Parse(AppConfigureHelper.AppSettings[id.ToString() + "SI"]?.ToString() ?? "4E-6"),
                    Int32.Parse(AppConfigureHelper.AppSettings[id.ToString() + "Len"]?.ToString() ?? "10000"));
            }
            return dds;
        }
        private Dictionary<ChannelId, IRFDataSource> GetRadioFrequencyChnlDataSource(DataSourceOpt source)
        {
            Dictionary<ChannelId, IRFDataSource> dds = new Dictionary<ChannelId, IRFDataSource>();
            Boolean status = source != DataSourceOpt.Simulator;
            foreach (var id in ChannelIdExt.GetRadioFrequencies())
            {
                dds[id] = status ? new DataSrcIQFFT() : new DataSrcIQFFTSim(DsoModel.Default.GetWfmGenerator(ChannelId.AWG1),
                    Double.Parse(AppConfigureHelper.AppSettings[id.ToString() + "SI"]?.ToString() ?? "4E-6"),
                    Int32.Parse(AppConfigureHelper.AppSettings[id.ToString() + "Len"]?.ToString() ?? "10000"));
            }
            return dds;
        }

        private Dictionary<ChannelId, IRFDataSource> GetRFAmpVSTimeChnlDataSource(DataSourceOpt source)
        {
            Dictionary<ChannelId, IRFDataSource> dds = new Dictionary<ChannelId, IRFDataSource>();
            Boolean status = source != DataSourceOpt.Simulator;
            foreach (var id in ChannelIdExt.GetAmpVSTimes())
            {
                dds[id] = status ? new DataSrcIQ() : new DataSrcIQSim(DsoModel.Default.GetWfmGenerator(ChannelId.AWG1),
                    Double.Parse(AppConfigureHelper.AppSettings[id.ToString() + "SI"]?.ToString() ?? "4E-6"),
                    Int32.Parse(AppConfigureHelper.AppSettings[id.ToString() + "Len"]?.ToString() ?? "10000"));
            }
            return dds;
        }
        private void BindRadioFrequencyChnlDataSource(Dictionary<ChannelId, IRFDataSource> rfcs)
        {
            foreach (var rfch in DsoModel.Default.RadioFrequencyChnls)
            {
                rfch.PrepareSamples = rfcs[rfch.Id].Prepare;
                rfch.ReadSamples = rfcs[rfch.Id].Read;
                rfch.ProcessSamples = rfcs[rfch.Id].Process;
                rfch.ProcessNormalSamples = rfcs[rfch.Id].ProcessNormal;
                rfch.ProcessMaxHoldSamples = rfcs[rfch.Id].ProcessMaxHold;
                rfch.ProcessMinHoldSamples = rfcs[rfch.Id].ProcessMinHold;
                rfch.ProcessAverageSamples = rfcs[rfch.Id].ProcessAverage;
                rfch.ProcessInit = rfcs[rfch.Id].Init;
                rfch.MakeVuSamples = WfmVuDatabase.RescaleRadioFrequency;
                rfch.MakeVuSamplesIQFFT = WfmVuDatabase.RescaleMDIQFFT;
            }
        }
        private void BindRFAmpVSTimeChnlDataSource(Dictionary<ChannelId, IRFDataSource> avstcs)
        {
            foreach (var rfch in DsoModel.Default.RFAmpVSTimeChnls)
            {
                rfch.PrepareSamples = avstcs[rfch.Id].Prepare;
                rfch.ReadSamples = avstcs[rfch.Id].Read;
                rfch.ProcessSamples = avstcs[rfch.Id].Process;
                rfch.MakeVuSamplesIQ = WfmVuDatabase.RescaleMDIQ;
            }
        }
        private void BindRFPhaseVSTimeChnlDataSource(Dictionary<ChannelId, IRFDataSource> pvstcs)
        {
            foreach (var rfch in DsoModel.Default.RFPhaseVSTimeChnls)
            {
                rfch.PrepareSamples = pvstcs[rfch.Id].Prepare;
                rfch.ReadSamples = pvstcs[rfch.Id].Read;
                rfch.ProcessSamples = pvstcs[rfch.Id].Process;
                rfch.MakeVuSamplesIQ = WfmVuDatabase.RescaleMDIQ;
            }
        }
        private void BindRFPhaseVSFrequencyChnlsDataSource(Dictionary<ChannelId, IRFDataSource> pvsfcs)
        {
            foreach (var rfch in DsoModel.Default.RFPhaseVSFrequencyChnls)
            {
                rfch.PrepareSamples = pvsfcs[rfch.Id].Prepare;
                rfch.ReadSamples = pvsfcs[rfch.Id].Read;
                rfch.ProcessSamples = pvsfcs[rfch.Id].Process;
                rfch.MakeVuSamplesIQFFT = WfmVuDatabase.RescaleMDIQFFT;
            }
        }
        private void BindRFFrequencyVSTimeChnlDataSource(Dictionary<ChannelId, IRFDataSource> fvstcs)
        {
            foreach (var rfch in DsoModel.Default.RFFrequencyVSTimeChnls)
            {
                rfch.PrepareSamples = fvstcs[rfch.Id].Prepare;
                rfch.ReadSamples = fvstcs[rfch.Id].Read;
                rfch.ProcessSamples = fvstcs[rfch.Id].Process;
                rfch.MakeVuSamplesIQ = WfmVuDatabase.RescaleMDIQ;
            }
        }
        private void BindRFTimeVSFrequencyChnlDataSource(Dictionary<ChannelId, IRFDataSource> tvsfcs)
        {
            foreach (var rfch in DsoModel.Default.RFTimeVSFrequencyChnls)
            {
                rfch.PrepareSamples = tvsfcs[rfch.Id].Prepare;
                rfch.ReadSamples = tvsfcs[rfch.Id].Read;
                rfch.ProcessSamples = tvsfcs[rfch.Id].Process;
                rfch.MakeVuSamplesIQFFT = WfmVuDatabase.RescaleMDIQFFT;
            }
        }
        public void BindRFDataSource(DataSourceOpt source)
        {
            BindRadioFrequencyChnlDataSource(GetRadioFrequencyChnlDataSource(source));

            BindRFAmpVSTimeChnlDataSource(GetRFAmpVSTimeChnlDataSource(source));
            BindRFPhaseVSTimeChnlDataSource(GetPhaseVSTimeChnlDataSource(source));
            BindRFPhaseVSFrequencyChnlsDataSource(GetPhaseVSFrequenciesChnlDataSource(source));
            BindRFFrequencyVSTimeChnlDataSource(GetFrequencyVSTimesDataSource(source));
            BindRFTimeVSFrequencyChnlDataSource(GetTimeVSFrequenciesDataSource(source));

        }

        //private  readonly Dictionary<ChannelId, IRFDataSource> _SimRFSource = new();
        //private  readonly Dictionary<ChannelId, IRFDataSource> _FifoRFSource = new();
        //private  readonly Dictionary<ChannelId, IRFDataSource> _SimAmpVSTimeSource = new();
        //private  readonly Dictionary<ChannelId, IRFDataSource> _FifoAmpVSTimeSource = new();
        //private  readonly Dictionary<ChannelId, IRFDataSource> _SimPhaseVSTimeSource = new();
        //private  readonly Dictionary<ChannelId, IRFDataSource> _FifoPhaseVSTimeSource = new();
        //private  readonly Dictionary<ChannelId, IRFDataSource> _SimPhaseVSFrequencySource = new();
        //private  readonly Dictionary<ChannelId, IRFDataSource> _FifoPhaseVSFrequencySource = new();
        //private  readonly Dictionary<ChannelId, IRFDataSource> _SimFrequencyVSTimeSource = new();
        //private  readonly Dictionary<ChannelId, IRFDataSource> _FifoFrequencyVSTimeSource = new();
        //private  readonly Dictionary<ChannelId, IRFDataSource> _SimTimeVSFrequencySource = new();
        //private  readonly Dictionary<ChannelId, IRFDataSource> _FifoTimeVSFrequencySource = new();

    }
    public enum DataRole
    {
        View,
        Zoom,
        Sda,
        Vsa,
        SourceData,
        Ai,
    }
}

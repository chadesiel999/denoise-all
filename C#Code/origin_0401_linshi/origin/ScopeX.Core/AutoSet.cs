//#define DEBUG_PROCEDURE
//#define DEBUG_TIME

using EventBus;
using ScopeX.ComModel;
using ScopeX.Core.Hardware;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using ScopeX.MathExt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace ScopeX.Core
{
    public class AutoSet
    {
        private Boolean _IsStarting = false;                              //开始标志
        private const Int32 MaxIterationCount = 10;                       //幅度当最大迭代次数
        private const Int32 CymometerGateByms = 50;                       //频率计闸门时间
        private Int32 _ScreenAdcValueRange = 4000;                         //ADC显示范围
        private const Int32 ChannelMinValidSignalBy_mV = 10;              //最小设置幅度
        private Double _Pos0ByAdc = 2048;                                  //基线位置ADC量化字
        private const Int32 DelayTimeByms = 100;                          //发送命令后等待时间
        private ChannelId _FocusId = ChannelId.C1;                        //焦点通道

        private Double _Frequency = 0;
        private record ChnlSetting(Boolean Active, AnaChnlScaleIndex AnaScaleIndex, Double ProbeGain, Double ProbeUnitRatio, AnaChnlCoupling Coupling, Boolean FineTurnStatus);

        private record BackupSetting(AnaChnlAcqMode AcqMode/*采集模式*/, Boolean Eres/*增强位数*/, AnaChnlStorageMode StorageMode/*存储模式*/, Int32 StorageOption/*存储深度*/, AnaChnlTimebaseIndex TmScale/*时基*/, TriggerType TriggerType/*触发类型*/, List<ChnlSetting> ChnlSetting/*通道模式*/, UInt32? CymometerGate/*闸门时间*/, Boolean CursorActive);
        private record ChnlInfo(Boolean Active, Double Max, Double Min, Double Ave, Double Vpp, Double Mid);

        private readonly DsoPrsnt _Oscilloscope;
        private ChannelId _FirstSignal = ChannelId.C1;
        private Stopwatch _StopWatcher;
        private Dictionary<AcqDataType, Double> _SamplingRate = new Dictionary<AcqDataType, Double>();
        private AutoSetResult _AtSetResult = AutoSetResult.Finish;
        public AutoSetResult AtSetResult
        {
            get => _AtSetResult;
            set => _AtSetResult = value;
        }

        private Int32 _OverTimeByMs;
        public Boolean AutosetBOK { get; private set; } = false;

        public Boolean IsAutoseting => _IsStarting;

        /// <summary>
        /// 堆叠显示
        /// </summary>
        public Boolean OverlapView { get; set; } = false;

        /// <summary>
        /// 耦合保持
        /// </summary>
        public Boolean CouplingHold { get; set; } = false;

        /// <summary>
        /// 触发
        /// </summary>
        public Boolean TriggerSetting { get; set; } = true;

        /// <summary>
        /// 垂直设置
        /// </summary>
        public Boolean VerticalSetting { get; set; } = true;

        /// <summary>
        /// 水平设置
        /// </summary>
        public Boolean HorizontalSetting { get; set; } = true;

        /// <summary>
        /// 采集设置
        /// </summary>
        public Boolean AcquisitionSetting { get; set; } = false;

        /// <summary>
        /// 通道设置：通道开关 有信号通道自动打开、关闭
        /// </summary>
        public Boolean ChannelSetting { get; set; } = true;

        public AutoSet(DsoPrsnt dso)
        {
            _Oscilloscope = dso;
            _StopWatcher = new Stopwatch();
            ParamsInit();
        }

        private void ParamsInit()
        {
            _Pos0ByAdc = Math.Pow(2, Constants.ADC_BITS) / 2;
            _ScreenAdcValueRange = (Int32)(Constants.SAMPS_PER_YDIV * Constants.VIS_YDIVS_NUM);
            _Frequency = 0;
        }

        private BackupSetting SaveBackup()
        {
            var chnlsetting = new List<ChnlSetting>();
            foreach (var id in ChannelIdExt.GetAnalogs())
            {
                if (_Oscilloscope.TryGetChannel(id, out var prsnt))
                {
                    var anaprsnt = (AnalogPrsnt)prsnt;
                    chnlsetting.Add(new(anaprsnt.Active, anaprsnt.AnaScaleIndex, anaprsnt.ProbeGain, anaprsnt.ProbeUnitRatio, anaprsnt.Coupling, anaprsnt.Ylevel_SelectStatus));
                }
            }
            _FocusId = DsoPrsnt.FocusId;
            return new(_Oscilloscope.Timebase.Mode, _Oscilloscope.Timebase.EnhancedBitsActive, _Oscilloscope.Timebase.StorageMode, _Oscilloscope.Timebase.StorageDepthOpt, _Oscilloscope.Timebase.ScaleIndex, TriggerPrsnt.Type, chnlsetting
                , Hd.Cymometer?.GateTime, _Oscilloscope.Cursor.Active);
        }

        private void RestoreBackup(BackupSetting bs)
        {
            if (!AcquisitionSetting)//采集设置
            {
                _Oscilloscope.Timebase.Mode = bs.AcqMode;
            }
            _Oscilloscope.Timebase.StorageMode = bs.StorageMode;
            _Oscilloscope.Timebase.EnhancedBitsActive = bs.Eres;
            _Oscilloscope.Timebase.StorageDepthOpt = bs.StorageOption;

            for (Int32 i = 0; i < bs.ChnlSetting.Count; i++)
            {
                if (_Oscilloscope.TryGetChannel((ChannelId)i, out var prsnt))
                {
                    var anaprsnt = (AnalogPrsnt)prsnt;
                    if (!ChannelSetting)//通道设置
                    {
                        anaprsnt.Active = bs.ChnlSetting[i].Active;
                    }

                    if (!VerticalSetting)//垂直设置
                    {
                        anaprsnt.AnaScaleIndex = bs.ChnlSetting[i].AnaScaleIndex;
                    }

                    if (CouplingHold || anaprsnt.HasActiveProbe)//耦合保持
                    {
                        anaprsnt.Coupling = bs.ChnlSetting[i].Coupling;
                    }
                    anaprsnt.Ylevel_SelectStatus = bs.ChnlSetting[i].FineTurnStatus;//还原幅度细调

                    anaprsnt.ProbeGain = bs.ChnlSetting[i].ProbeGain;//还原探头倍率
                    anaprsnt.ProbeUnitRatio = bs.ChnlSetting[i].ProbeUnitRatio;
                }
            }
            if (!HorizontalSetting)//水平设置
            {
                _Oscilloscope.Timebase.ScaleIndex = bs.TmScale;
                _Oscilloscope.Timebase.ScaleByus = _Oscilloscope.Timebase.GetScale(_Oscilloscope.Timebase.ScaleIndex);
            }

            Hd.Cymometer!.GateTime = bs.CymometerGate!.Value;

            if (TriggerSetting)//触发设置
            {
                TriggerPrsnt.GetOrMakeTrigger(_Oscilloscope, TriggerType.Edge);
                var edge = _Oscilloscope.CurrentTrigger as TrigEdgePrsnt;
                if (edge != null)
                {
                    edge.Source = _FirstSignal;
                    if (_Oscilloscope.TryGetChannel((ChannelId)edge.Source, out var prsnt))
                    {
                        var ap = (AnalogPrsnt)prsnt;
                        edge.CompPosition *= (ap.ProbeGain * ap.ProbeUnitRatio);
                    }
                }
            }
            else
            {
                _Oscilloscope.SetTrigger(bs.TriggerType);
            }

            //焦点通道ID设置
            if (_Oscilloscope.TryGetChannel(_FocusId, out var p))
            {
                if (p.Active)
                {
                    DsoPrsnt.FocusId = _FocusId;
                }
                else
                {
                    _Oscilloscope.MoveFocusId();
                }
            }

            //光标设置
            CursorSetting(bs.CursorActive);
            Dispatcher.IsScan = Dispatcher.GetScanState();
            //执行硬件命令并生效
            if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
            {
                Hd.Execute(msg!);
                Thread.Sleep(DelayTimeByms);
                Acquisition.Default.UpdateReadInfoList();
            }
        }

        private List<ChannelId> Init()
        {
            _Oscilloscope.Cursor.Active = false;
            _Oscilloscope.Measure.Indicator = 0;

            //设置时基
            _Oscilloscope.Timebase.ScaleIndex = AnaChnlTimebaseIndex.Lv5m;

            //设置存储深度
            _Oscilloscope.Timebase.StorageDepthOpt = 1;//25K

            //设置触发
            TriggerPrsnt.HoldoffType = DelayOpt.Time;
            TriggerPrsnt.HoldoffByps = TriggerPrsnt.MinHoldoffTime;
            var edge = _Oscilloscope.SetTrigger(TriggerType.Edge) as TrigEdgePrsnt;
            if (edge != null)
            {
                edge.Coupling = TriggerCoupling.DC;
                edge.Slope = EdgeSlope.Rise;
                _FirstSignal = edge!.Source!.Value;
            }
            //设置时基及采集
            _Oscilloscope.Timebase.Mode = AnaChnlAcqMode.Peak;
            //_Oscilloscope.Timebase.ScaleIndex = AnaChnlTimebaseIndex.Lv10m; //todo:当前最小可调频率为10Hz
            _Oscilloscope.Timebase.StorageMode = AnaChnlStorageMode.Long;
            _Oscilloscope.Timebase.ResetPosIndex();

            //设置并记录通道
            List<ChannelId> activeChnls = new List<ChannelId>();
            foreach (var id in ChannelIdExt.GetAnalogs())
            {
                if (_Oscilloscope.TryGetChannel(id, out var p))
                {
                    var aprsnt = (AnalogPrsnt)p;
                    if (ChannelSetting)
                    {
                        aprsnt.Active = true;
                    }
                    if (aprsnt.Active)
                    {
                        if (PlatformManager.Default.Platform.DefaultHighImpedance)
                        {
                            if (aprsnt.Coupling != AnaChnlCoupling.DC1M)
                                aprsnt.Coupling = AnaChnlCoupling.DC1M;
                        }
                        else
                        {
                            if (aprsnt.Coupling != AnaChnlCoupling.DC50)
                                aprsnt.Coupling = AnaChnlCoupling.DC50;
                        }
                        aprsnt.Bandwidth = 0;
                        aprsnt.ResetPosIndex();
                        aprsnt.Bias = 0;
                        //aprsnt.AnaScaleIndex = AnaChnlScaleIndex.Lv100m;
                        aprsnt.IsCoarse = true;
                        aprsnt.Ylevel_SelectStatus = false;//关闭幅度细调
                        aprsnt.ProbeGain = 1;
                        aprsnt.ProbeUnitRatio = 1;

                        activeChnls.Add(id);
                    }
                }
            }
            Dispatcher.IsScan = Dispatcher.GetScanState();
            //执行硬件命令并生效
            if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
            {
                Hd.Execute(msg!);
                Thread.Sleep(DelayTimeByms);
                //Clear掉Dirver端缓存数据
                Hd.AcqWave(false, true, Acquisition.Default.AllChnlReadInfo, ref _SamplingRate);
            }

            return activeChnls;
        }

        private void Prepare()
        {
            //关闭eres
            _Oscilloscope.Timebase.EnhancedBitsActive = false;

            //关闭除了模拟通道外的其他所有通道比如LA、BUS、Math
            foreach (IChnlPrsnt ptsnt in _Oscilloscope.TryGetRange((IChnlPrsnt c) => !ChannelIdExt.IsAnalog(c.Id) && c.Active))
            {
                ptsnt.Active = false;
            }

            //关闭顺序模式
            _Oscilloscope.Timebase.SegmentActive = false;

            //关闭通过失败、电源分析、抖动分析、搜索
            if (_Oscilloscope.PassFail?.Active ?? false)
            {
                _Oscilloscope.PassFail.Active = false;
            }
            foreach (var prsnt in _Oscilloscope.PwrAnalysisDictionary)
            {
                prsnt.Value.Active = false;
            }
            if (_Oscilloscope.Jitter?.Active ?? false)
            {
                _Oscilloscope.Jitter.Active = false;
            }
            if (_Oscilloscope.Search?.Enabled ?? false)
            {
                _Oscilloscope.Search.Enabled = false;
            }

            TriggerPrsnt.Mode = TriggerMode.Auto;
            TriggerPrsnt.ResetState();
            //执行硬件命令并生效
            if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
            {
                Hd.Execute(msg!);
                Thread.Sleep(DelayTimeByms);
            }
        }

        private Dictionary<ChannelId, ChnlInfo> FindMaxMin(List<ChannelId> chnls)
        {
            var chnlInfos = new Dictionary<ChannelId, ChnlInfo>();
            var viewmask = nameof(DataRole.View);
            chnls.ForEach(chnl =>
            {
                if (_Oscilloscope.TryGetChannel(chnl, out var anaprsnt))
                {
                    var bok = Hd.AnalogChannel!.TryTakeWave(chnl, Acquisition.Default.AllChnlReadInfo.Where(o => o.Mark.Equals(viewmask)).ToList(), out var wfm, null);
                    if (bok)
                    {
                        if (wfm[viewmask].wfmData == null || wfm[viewmask].wfmData.Count <= 0)
                        {
                            chnlInfos.Add(chnl, new ChnlInfo(anaprsnt.Active, Double.NaN, Double.NaN, Double.NaN, Double.NaN, Double.NaN));
                        }
                        else
                        {
                            var buffer = wfm[viewmask].wfmData.ToList();
                            var pbuffer = buffer.Select(x => (Double)x);
                            var max = pbuffer.Max();
                            var min = pbuffer.Min();
                            var avg = pbuffer.Average();
                            var vpp = Math.Abs(max - min);
                            var mid = (max + min) / 2;
                            chnlInfos.Add(chnl, new ChnlInfo(anaprsnt.Active, max, min, avg, vpp, mid));
                        }
                    }
                    else
                    {
                        chnlInfos.Add(chnl, new ChnlInfo(anaprsnt.Active, Double.NaN, Double.NaN, Double.NaN, Double.NaN, Double.NaN));
                    }
                }
            });

            return chnlInfos;
        }

        private Boolean AcquireWave(CancellationToken token)
        {
            _StopWatcher.Reset();
            _StopWatcher.Start();
            var bok = Hd.AcqWave(false, true, Acquisition.Default.AllChnlReadInfo, ref _SamplingRate);
            while (!bok && _StopWatcher.ElapsedMilliseconds < _OverTimeByMs && (token.IsCancellationRequested == false))
            {
                var t1 = DateTime.Now;
                bok = Hd.AcqWave(false, false, Acquisition.Default.AllChnlReadInfo, ref _SamplingRate);
                var t2 = DateTime.Now;
#if DEBUG_PROCEDURE
                Debug.WriteLine($"[AutoSet][{DateTime.Now.ToString("hh:mm:ssss")}]:AcquireWave time：{(t2 - t1).TotalMilliseconds} ms");
#endif
            }

#if DEBUG_PROCEDURE
            Debug.WriteLine($"[AutoSet][{DateTime.Now.ToString("hh:mm:ssss")}]:AcquireWave total time = {_StopWatcher.ElapsedMilliseconds}ms");
#endif
            //丢掉前两次数据
            Hd.AcqWave(false, false, Acquisition.Default.AllChnlReadInfo, ref _SamplingRate);
            Hd.AcqWave(false, false, Acquisition.Default.AllChnlReadInfo, ref _SamplingRate);
            _StopWatcher.Stop();
            return bok;
        }

        private Dictionary<ChannelId, (ChnlInfo Info, Boolean IsSignal)> LocateVoltage(List<ChannelId> adjustChnls, CancellationToken token)
        {
            Dictionary<ChannelId, (ChnlInfo Info, Boolean IsSignal)> locatedchnls = new();
            Dictionary<ChannelId, List<AnaChnlScaleIndex>> chnlscaleindex = new Dictionary<ChannelId, List<AnaChnlScaleIndex>>();
            var iteration = 0;//迭代次数
            while (adjustChnls.Count() > 0 && iteration <= MaxIterationCount/*超过最大迭代次数*/)
            {
                iteration++;
                //刷新读参数
                Acquisition.Default.UpdateReadInfoList();
                var bok = AcquireWave(token);

                if (!bok)
                {
                    _AtSetResult = AutoSetResult.ReadDataTimeout;
                    return locatedchnls;
                }
                else
                {
                    _AtSetResult = AutoSetResult.Finish;
                }
                var chnlInfos = FindMaxMin(adjustChnls);
                var list = adjustChnls.ToList();

                foreach (var item in list)
                {
#if DEBUG_PROCEDURE
                    Debug.WriteLine($"[AutoSet][{DateTime.Now.ToString("hh:mm:ss")}]:{item}幅度档第{iteration}迭代");
#endif
                    if (!chnlscaleindex.ContainsKey(item))
                    {
                        chnlscaleindex[item] = new List<AnaChnlScaleIndex>();
                    }
                    _Oscilloscope.TryGetChannel(item, out var prsnt);
                    var anaPrsnt = (AnalogPrsnt)prsnt!;
                    Double max = Math.Max(Math.Abs(chnlInfos[item].Max - _Pos0ByAdc), Math.Abs(chnlInfos[item].Min - _Pos0ByAdc));
#if DEBUG_PROCEDURE
                    Debug.WriteLine($"[AutoSet][{DateTime.Now.ToString("hh:mm:ss")}]:{item}：Max = {chnlInfos[item].Max},Min = {chnlInfos[item].Min},Diff = {max},Vpp={chnlInfos[item].Vpp}");
#endif
                    Int32 yStep = 0;
                    if (max >= _Pos0ByAdc - 1)
                    {
                        yStep = 4;
                    }
                    else if (max > (_ScreenAdcValueRange / 2) - 15)
                    {
                        yStep = 1;
                    }
                    else if (YScaleDecrease(1, 6, max, anaPrsnt))
                    {
                        yStep = -6;
                    }
                    else if (YScaleDecrease(1, 5, max, anaPrsnt))
                    {
                        yStep = -5;
                    }
                    else if (YScaleDecrease(1, 4, max, anaPrsnt))
                    {
                        yStep = -4;
                    }
                    else if (YScaleDecrease(1, 3, max, anaPrsnt))
                    {
                        yStep = -3;
                    }
                    else if (YScaleDecrease(1, 2, max, anaPrsnt))
                    {
                        yStep = -2;
                    }
                    else if (YScaleDecrease(0, 1, max, anaPrsnt))
                    {
                        yStep = -1;
                    }
                    else
                    {
                        yStep = 0;
                        adjustChnls.Remove(item);
#if DEBUG_PROCEDURE
                        Debug.WriteLine($"[AutoSet][{DateTime.Now.ToString("hh:mm:ss")}]:Info {item}幅度档第 {iteration} 迭代成功");
#endif
                        if (!IsSignal(chnlInfos[item].Vpp, anaPrsnt.AnaScaleIndex, anaPrsnt))//Have Signal ?
                        {
                            locatedchnls.Add(item, (chnlInfos[item], true));//Yes
                        }
                        else
                        {
                            anaPrsnt.AnaScaleIndex = (AnaChnlScaleIndex)(((Int16)anaPrsnt.ScaleMinIndex + (Int16)anaPrsnt.ScaleMaxIndex) / 2);
                            locatedchnls.Add(item, (chnlInfos[item], false));//No
                        }
                    }

                    if (yStep > 0)
                    {
                        if (anaPrsnt.AnaScaleIndex == anaPrsnt.ScaleMaxIndex)
                        {
                            adjustChnls.Remove(item);
#if DEBUG_PROCEDURE
                            Debug.WriteLine($"[AutoSet][{DateTime.Now.ToString("hh:mm:ss")}]:Info {item}幅度档第 {iteration} 迭代成功");
#endif
                            if (!IsSignal(chnlInfos[item].Vpp, anaPrsnt.AnaScaleIndex, anaPrsnt))//Have Signal ?
                            {
                                locatedchnls.Add(item, (chnlInfos[item], true));//Yes
                            }
                            else
                            {
                                anaPrsnt.AnaScaleIndex = (AnaChnlScaleIndex)(((Int16)anaPrsnt.ScaleMinIndex + (Int16)anaPrsnt.ScaleMaxIndex) / 2);
                                locatedchnls.Add(item, (chnlInfos[item], false));//No
                            }
                        }
                        else
                        {
                            anaPrsnt.AnaScaleIndex = anaPrsnt.AnaScaleIndex + yStep < anaPrsnt.ScaleMaxIndex ? anaPrsnt.AnaScaleIndex + yStep : anaPrsnt.ScaleMaxIndex;
                        }
                    }
                    else if (yStep < 0)
                    {
                        if (anaPrsnt.AnaScaleIndex == anaPrsnt.ScaleMinIndex)
                        {
                            adjustChnls.Remove(item);
#if DEBUG_PROCEDURE
                            Debug.WriteLine($"[AutoSet][{DateTime.Now.ToString("hh:mm:ss")}]:Info {item} 幅度档第 {iteration} 迭代成功");
#endif
                            if (!IsSignal(chnlInfos[item].Vpp, anaPrsnt.AnaScaleIndex, anaPrsnt))//Have Signal ?
                            {
                                locatedchnls.Add(item, (chnlInfos[item], true));//Yes
                            }
                            else
                            {
                                anaPrsnt.AnaScaleIndex = (AnaChnlScaleIndex)(((Int16)anaPrsnt.ScaleMinIndex + (Int16)anaPrsnt.ScaleMaxIndex) / 2);
                                locatedchnls.Add(item, (chnlInfos[item], false));//No
                            }
                        }
                        else
                        {
                            anaPrsnt.AnaScaleIndex = anaPrsnt.AnaScaleIndex + yStep > anaPrsnt.ScaleMinIndex ? anaPrsnt.AnaScaleIndex + yStep : anaPrsnt.ScaleMinIndex;
                        }
                    }
                    else
                    {

                    }
                    if (chnlscaleindex[item].Count == 3)
                    {
                        if (chnlscaleindex[item].Contains(anaPrsnt.AnaScaleIndex)) //(anaPrsnt.AnaScaleIndex == chnlscaleindex[item][0] || anaPrsnt.AnaScaleIndex == chnlscaleindex[item][1])
                        {
                            if (adjustChnls.Contains(item))
                            {
                                adjustChnls.Remove(item);
#if DEBUG_PROCEDURE
                                Debug.WriteLine($"[AutoSet][{DateTime.Now.ToString("hh:mm:ss")}]:Warning {item}进入垂直挡位循环，提前退出当前流程");
#endif
                                if (!IsSignal(chnlInfos[item].Vpp, anaPrsnt.AnaScaleIndex, anaPrsnt))//Have Signal ?
                                {
                                    locatedchnls.Add(item, (chnlInfos[item], true));//Yes
                                }
                                else
                                {
                                    anaPrsnt.AnaScaleIndex = (AnaChnlScaleIndex)(((Int16)anaPrsnt.ScaleMinIndex + (Int16)anaPrsnt.ScaleMaxIndex) / 2);
                                    locatedchnls.Add(item, (chnlInfos[item], false));//No
                                }
                            }
                        }
                    }
                    chnlscaleindex[item].Add(anaPrsnt.AnaScaleIndex);
                    if (chnlscaleindex[item].Count > 3)//缓存最近两次的幅度索引
                    {
                        chnlscaleindex[item].RemoveAt(0);
                    }
                }

                if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
                {
                    Hd.Execute(msg!);
                    Thread.Sleep(DelayTimeByms);
                    //Clear掉Dirver端缓存数据
                    Hd.AcqWave(false, true, Acquisition.Default.AllChnlReadInfo, ref _SamplingRate);
                }
            }
            //CalcBias(locatedChnls);
            return iteration >= MaxIterationCount ? locatedchnls : locatedchnls;
        }

        private Boolean YScaleDecrease(Int32 v1, Int32 yscale, Double max, AnalogPrsnt prsnt)
        {
            if (!((Int32)prsnt.AnaScaleIndex >= yscale))
            {
                return false;
            }
            var res = (max - v1) * prsnt.GetScale(prsnt.AnaScaleIndex) / prsnt.GetScale(prsnt.AnaScaleIndex - yscale);
            return res < _ScreenAdcValueRange / 2;
        }

        private Boolean IsSignal(Double vpp, AnaChnlScaleIndex yScale, AnalogPrsnt prsnt)
        {
            var res = vpp * prsnt.GetScale(yScale) / prsnt.GetScale(AnaChnlScaleIndex.Lv1m) / Constants.SAMPS_PER_YDIV;
            return res < ChannelMinValidSignalBy_mV;
        }

        public void SetTriggerEdge(EdgeSlope edge)
        {
            SetTimebase(1, _Frequency);
            var edgePrsnt = _Oscilloscope.SetTrigger(TriggerType.Edge) as TrigEdgePrsnt;
            edgePrsnt.Slope = edge;
        }

        private Boolean SetTimebase(ChannelId chnlId, Int32 PeriodNum, CancellationToken token)
        {
            Int32 lev = (Int32)_Oscilloscope.Timebase.ScaleMaxIndex;
            Int32 maxiterationcount = AnaChnlTimebaseIndex.Lv10m - _Oscilloscope.Timebase.ScaleMinIndex;
            var iteration = 0;//迭代次数
            while (iteration <= maxiterationcount/*超过最大迭代次数*/)
            {
                iteration++;
                DateTime WriteTime = DateTime.Now;
                _Oscilloscope.Timebase.ScaleByus = _Oscilloscope.Timebase.GetScale((AnaChnlTimebaseIndex)lev);
                if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
                {
                    Hd.Execute(msg!);
                    //刷新读参数
                    Thread.Sleep(DelayTimeByms);
                }

                Acquisition.Default.UpdateReadInfoList();

                var bok = AcquireWave(token);

                if (!bok)
                {
                    _AtSetResult = AutoSetResult.ReadDataTimeout;
                    return false;
                }
                else
                {
                    _AtSetResult = AutoSetResult.Finish;
                }

                var curperiodnum = GetPeriodNum(chnlId);
                if (curperiodnum == 0)
                {
                    return false;
                }

                TimeSpan cost = DateTime.Now - WriteTime;

                Trace.WriteLine($"**************** SetTimebase {(AnaChnlTimebaseIndex)lev} cost:    {cost.TotalMilliseconds}        ms **********");

                if (curperiodnum <= PeriodNum)
                {
                    break;
                }
                if (curperiodnum > PeriodNum * 100)
                {
                    lev -= 7;
                }
                else if (curperiodnum > PeriodNum * 50)
                {
                    lev -= 6;
                }
                else if (curperiodnum > PeriodNum * 20)
                {
                    lev -= 5;
                }
                else if (curperiodnum > PeriodNum * 10)
                {
                    lev -= 4;
                }
                else if (curperiodnum > PeriodNum * 5)
                {
                    lev -= 3;
                }
                else if (curperiodnum > PeriodNum * 2)
                {
                    lev -= 2;
                }
                else
                {
                    lev -= 1;
                }



                if (lev < 0)
                    return false;
                if ((AnaChnlTimebaseIndex)lev < _Oscilloscope.Timebase.ScaleMinIndex)
                {
                    lev = (Int32)_Oscilloscope.Timebase.ScaleMinIndex;
                }
            }

            if (iteration >= maxiterationcount)//超过最大迭代次数
            {
                return false;
            }

            return true;
        }

        private Int32 GetPeriodNum(ChannelId chnlId)
        {
            var chnlInfos = new Dictionary<ChannelId, ChnlInfo>();
            var viewmask = nameof(DataRole.View);
            var bok = Hd.AnalogChannel!.TryTakeWave(chnlId, Acquisition.Default.AllChnlReadInfo.Where(o => o.Mark.Equals(viewmask)).ToList(), out var wfm, null);
            if (bok && wfm.Count > 0)
            {
                var source = wfm.FirstOrDefault().Value.wfmData;
                var hist = MakeHist(source);
                var avg = source.Select(o => (Double)o).Average();
                Int32 avgid = (Int32)Math.Round(avg);

                UInt32 lowvalue = 0;
                Int32 lowpos = 0;
                for (Int32 i = 0; i < avgid; i++)
                {
                    if (hist[i] > lowvalue)
                    {
                        lowvalue = hist[i];
                        lowpos = i;
                    }
                }

                UInt32 highvalue = 0;
                Int32 highpos = avgid;
                for (Int32 i = avgid; i < hist.Length; i++)
                {
                    if (hist[i] > highvalue)
                    {
                        highvalue = hist[i];
                        highpos = i;
                    }
                }

                Int32 amp = highpos - lowpos;

                if (amp < Constants.SAMPS_PER_YDIV)
                {
                    return 0;
                }

                Int32 lowcompare = (Int32)(lowpos + 0.3 * amp);
                Int32 highcompare = (Int32)(lowpos + 0.7 * amp);

                Int32 periodnum = 0;
                Boolean ispasslowvalue = false;
                for (Int32 i = 0; i < source.Count; i++)
                {
                    if (!ispasslowvalue)
                    {
                        if (source[i] < lowcompare)
                        {
                            ispasslowvalue = true;
                        }
                    }
                    else
                    {
                        if (source[i] > highcompare)
                        {
                            periodnum++;
                            ispasslowvalue = false;
                        }
                    }
                }
                return periodnum;
            }
            return 0;
        }

        private UInt32[] MakeHist(List<UInt16> source)
        {
            Int32 len = (Int32)(Math.Pow(2, Constants.ADC_BITS));
            UInt32[] hist = new UInt32[len];

            for (Int32 i = 0; i < source.Count; i++)
            {
                hist[source[i]]++;
            }

            return hist;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="periodNum">一个屏幕内周期数</param>
        public void SetTimebase(Int32 periodNum = 2, Double frequency = 0)
        {
            frequency = frequency == 0 ? _Frequency : frequency;
            if (frequency == 0)
            {
                frequency = Hd.Cymometer?.GetFrequencyByHz((Int32)ChannelId.C1) ?? Double.NaN;
                if (frequency == 0 || frequency == Double.NaN)
                    return;
            }
            var periodByus = 1.0 / frequency * 1e6;
            var scaleperdiv = periodNum * periodByus / Constants.VIS_XDIVS_NUM;//一个屏幕内PeriodNum个周期 10div

            var scaleindexlist = Enum.GetValues<AnaChnlTimebaseIndex>()
                .Where(index => (Int32)index >= (Int32)_Oscilloscope.Timebase.ScaleMinIndex && (Int32)index <= (Int32)_Oscilloscope.Timebase.ScaleMaxIndex).ToList();
            AnaChnlTimebaseIndex bestscale = scaleindexlist
                .OrderBy(scale => Math.Abs(_Oscilloscope.Timebase.GetScale(scale) - scaleperdiv))
                .First();

            _Oscilloscope.Timebase.ScaleByus = _Oscilloscope.Timebase.GetScale(bestscale);
        }

        private Boolean SetTimebaseByCymometer()
        {
            //Thread.Sleep(_CymometerGate);
            Hd.Cymometer!.GateTime = CymometerGateByms;
            Thread.Sleep(CymometerGateByms);
            _Frequency = Hd.Cymometer?.GetFrequencyByHz((Int32)ChannelId.C1) ?? Double.NaN;
            _StopWatcher.Restart();
            while (_Frequency == 0 && _StopWatcher.ElapsedMilliseconds < CymometerGateByms * 5)
            {
                Thread.Sleep(1);
                _Frequency = Hd.Cymometer?.GetFrequencyByHz((Int32)ChannelId.C1) ?? Double.NaN;
            }
#if DEBUG_PROCEDURE
            Debug.WriteLine($"[AutoSet][{DateTime.Now.ToString("hh:mm:ssss")}]:Source = {((TrigEdgePrsnt)_Oscilloscope.CurrentTrigger).Source};Frequency = {freq}Hz,Reading time = {_StopWatcher.ElapsedMilliseconds}");
#endif
            if (Double.IsNaN(_Frequency) || _Frequency <= 0)
            {
                _AtSetResult = AutoSetResult.ReadCymometeTimeout;
                return false;
            }
            else
                _AtSetResult = AutoSetResult.Finish;

            SetTimebase(6, _Frequency);
            if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
            {
                Hd.Execute(msg!);
                Thread.Sleep(DelayTimeByms);
                Acquisition.Default.UpdateReadInfoList();
            }
            return true;

            //            Int32 lev = 0;

            //            while (freq >= 200)
            //            {
            //                freq /= 10;
            //                lev -= 3;
            //            }

            //            if (freq <= 20)
            //                lev += (Int32)AnaChnlTimebaseIndex.Lv50m;// XLEV20MS;
            //            else if (freq <= 50)
            //                lev += (Int32)AnaChnlTimebaseIndex.Lv20m;
            //            else if (freq <= 100)
            //                lev += (Int32)AnaChnlTimebaseIndex.Lv10m;
            //            else
            //                lev += (Int32)AnaChnlTimebaseIndex.Lv5m;

            //            if (lev < (Int32)_Oscilloscope.Timebase.ScaleMinIndex)
            //                lev = (Int32)_Oscilloscope.Timebase.ScaleMinIndex;

            //            //默认一屏包含4个周期的波形
            //            _Oscilloscope.Timebase.ScaleByus = _Oscilloscope.Timebase.GetScale((AnaChnlTimebaseIndex)lev);
            //#if DEBUG_PROCEDURE
            //            Debug.WriteLine($"[AutoSet][{DateTime.Now.ToString("hh:mm:ssss")}]:Timebase = {_Oscilloscope.Timebase.ScaleByus}us");
            //#endif
            //            if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
            //            {
            //                Hd.Execute(msg!);
            //                Thread.Sleep(DelayTimeByms);
            //                Acquisition.Default.UpdateReadInfoList();
            //            }
            //            return true;
        }

        private Boolean SetTimebaseByMeasure(ChannelId source)
        {
            var period = _Oscilloscope.Measure.GetOrCalcResult("Period", source);
            if (period is null)
            {
                return false;
            }
            _Oscilloscope.Timebase.ScaleByus = Math.Round(2.5 * period.Value);

            return true;
        }

        private void LocateTimeBase(Dictionary<ChannelId, (ChnlInfo Info, Boolean IsSignal)> singleChnls, CancellationToken token)
        {
            var havesinglechnl = singleChnls.Where(chnl => chnl.Value.IsSignal == true).ToList();
            if (havesinglechnl.Count == 0)
            {
                _Oscilloscope.Timebase.ScaleByus = _Oscilloscope.Timebase.GetScale(AnaChnlTimebaseIndex.Lv1u);
                if (_AtSetResult != AutoSetResult.ReadDataTimeout)
                {
                    _AtSetResult = AutoSetResult.NoSignal;
                }
                return;
            }
            else
                _AtSetResult = AutoSetResult.Finish;

            var edgeprsnt = _Oscilloscope.CurrentTrigger as TrigEdgePrsnt;

            //sort by channel id
            havesinglechnl.Sort((T1, T2) => T1.Key.CompareTo(T2.Key));
            foreach (var item in havesinglechnl)
            {
                edgeprsnt!.Source = item.Key;

                _ = _Oscilloscope.TryGetChannel(edgeprsnt.Source.Value, out var ap);
                var anaprsnt = (AnalogPrsnt)ap!;

                //设置触发电平
                edgeprsnt.CompPosition = (item.Value.Info.Mid - _Pos0ByAdc) * anaprsnt.GetScale(anaprsnt.AnaScaleIndex) / Constants.SAMPS_PER_YDIV;
                //edgePrsnt.SetPosIndexCenterZero();

                if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
                {
                    Hd.Execute(msg!);
                    Thread.Sleep(DelayTimeByms);
                    token.ThrowIfCancellationRequested();
                }

                Boolean cymflag = SetTimebaseByCymometer();
                //Boolean cymflag = SetTimebase(item.Key, 10, token);
                if (cymflag)
                {
                    _FirstSignal = item.Key;
                    _Oscilloscope.Cymometer.Source = item.Key;
                    return;
                }
            }
        }

        private void ChannelActiveSetting(Dictionary<ChannelId, (ChnlInfo Info, Boolean IsSignal)> Channels)
        {
            Boolean isanyactive = false;
            foreach (var chnl in Channels)
            {
                if (_Oscilloscope.TryGetChannel(chnl.Key, out var prsnt) && prsnt is AnalogPrsnt ap)
                {
                    ap.Active = chnl.Value.IsSignal;//根据信号有无 进行开关通道
                    isanyactive |= ap.Active;
                };
            }

            if (!isanyactive)//如果通道全部无信号（全部关闭） 则默认打开CH1
            {
                if (_Oscilloscope.TryGetChannel(ChannelId.C1, out var prsnt) && prsnt is AnalogPrsnt ap)
                {
                    ap.Active = true;
                };
            }
        }

        private void CursorSetting(Boolean cursorActive)
        {
            _Oscilloscope.Cursor.Active = cursorActive;
            if (cursorActive)
            {
                if (_Oscilloscope.TryGetChannel(_Oscilloscope.Cursor.SyncSource, out var cursorprsnt))
                {
                    if (!cursorprsnt.Active)//若当前光标源对应通道未打开  则切换到FocusId对应通道
                    {
                        if (DsoPrsnt.FocusId.IsAnalog() || DsoPrsnt.FocusId.IsBaseMath() || DsoPrsnt.FocusId.IsReference())//模拟通道、数学、参考
                        {
                            _Oscilloscope.Cursor.SyncSource = DsoPrsnt.FocusId;
                        }
                        else
                        {
                            var anaprsnts = _Oscilloscope.TryGetRange(ChannelIdExt.GetAnalogs());
                            if (anaprsnts != null)
                            {
                                _Oscilloscope.Cursor.SyncSource = anaprsnts.FirstOrDefault(prsnt => prsnt.Active)!.Id;//默认打开的第一个模拟通道
                            }
                        }
                    }
                    else
                    {
                        //to do
                    }
                }
            }
        }

        private void SplitChannel(Dictionary<ChannelId, (ChnlInfo Info, Boolean IsSignal)> singleChnls)
        {
            var needsplitchnl = singleChnls.Where(chnl => chnl.Value.IsSignal).ToList();

            //sort by channel id
            needsplitchnl.Sort((T1, T2) => T1.Key.CompareTo(T2.Key));
            Int32 count = needsplitchnl.Count();
            if (count < 2)
            {
                return;
            }
            Int32 scale = 0;
            switch (count)
            {
                case 4:
                case 3:
                    scale = 2;
                    break;
                case 2:
                    scale = 1;
                    break;
                default:
                    scale = 0;
                    break;
            }
            Int32 i = 0;
            foreach (var item in needsplitchnl)
            {
                _ = _Oscilloscope.TryGetChannel(item.Key, out var ap);
                var anaprsnt = (AnalogPrsnt)ap!;

                anaprsnt.AnaScaleIndex = anaprsnt.AnaScaleIndex + scale;
                anaprsnt.PosIndexBymDiv = 4000 - (i * 8000 / count) - (4000 / count);
                i++;
            }

            if (needsplitchnl.Count >= 3)
            {
                if (_Oscilloscope.CurrentTrigger is TrigEdgePrsnt edgeprsnt)
                {
                    var trigger = needsplitchnl.Where(item => item.Key == edgeprsnt.Source).FirstOrDefault();
                    if (_Oscilloscope.TryGetChannel((ChannelId)edgeprsnt.Source!, out var ap))
                    {
                        var anaprsnt = (AnalogPrsnt)ap!;
                        var min = (trigger.Value.Info.Min - _Pos0ByAdc) * anaprsnt.GetScale(anaprsnt.AnaScaleIndex - 2) / Constants.SAMPS_PER_YDIV;//平铺的压缩了两个挡位 此处再减回来
                        edgeprsnt.CompPosition = min + 0.5 * anaprsnt.GetScale(anaprsnt.AnaScaleIndex);

                        if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
                        {
                            Hd.Execute(msg!);
                        }
                    }

                }
            }
        }

        private Stopwatch _SWatch = new Stopwatch();
        private void TimeLogger(String info)
        {
#if DEBUG_TIME
            Debug.WriteLine("[AutoSet]" + info + $"：time elapsed {sWatch.Elapsed.TotalMilliseconds}ms");
            sWatch.Restart();
#endif
        }

        public void Run(CancellationToken token)
        {
            if (_IsStarting)
            {
                return;
            }
            _Oscilloscope.VuClear();
            _IsStarting = true;//开始标志
            DsoPrsnt.KeyBoardLockEnable = true;//键盘锁
            _OverTimeByMs = 500;
            AutosetBOK = true;
            //Prepare();
            //Dispatcher.Cancel();
            ////清除数据
            //Dispatcher.SoftReset();
            //Dispatcher.DoClear();
            //UpdateVuTask.Cancel();

            //清除数据
            Dispatcher.SoftReset();
            Dispatcher.DoClear();
            Thread.Sleep(100);
            Dispatcher.Cancel();
            UpdateVuTask.Cancel();

            //保证Dispatcher.Run()完全结束
            Prepare();

            _SWatch.Start();
            WeakTip.Default.Enabled = false;
            try
            {
                var bkp = SaveBackup();
                //TimeLogger("Prepare");
                ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set("DebugVariant,bEnable_Dsp:false");

                List<ChannelId> activeChnls = Init();
                //TimeLogger("Init");
                ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set("DebugVariant,bEnable_DsoGainByFpga:false");
                ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set("DebugVariant,bEnable_DigitTrigger:false");
                var singleChnls = LocateVoltage(activeChnls, token);
                //TimeLogger("LocateVoltage");
                ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set("DebugVariant,bEnable_DigitTrigger:true");
                ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"DebugVariant,bEnable_DsoGainByFpga:true");

                LocateTimeBase(singleChnls, token);
                //TimeLogger("LocateTimeBase");

                if (ChannelSetting)
                {
                    ChannelActiveSetting(singleChnls);
                }

                if (!OverlapView)//通道拆分
                {
                    SplitChannel(singleChnls);
                }

                //TimeLogger("SplitChannel");

                RestoreBackup(bkp);
                ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set("DebugVariant,bEnable_Dsp:true");

            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Error));
                //WeakTip.Default.Write("AutoSet", MsgTipId.AutoSetFail, emergent: false, "", 5);
                AutosetBOK = false;
                _IsStarting = false;
                _AtSetResult = AutoSetResult.Finish;
                DsoPrsnt.KeyBoardLockEnable = false;//键盘锁
                ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set("DebugVariant,bEnable_Dsp:true");
            }
            WeakTip.Default.Enabled = PlatformManager.Default.Platform.ShowWeakTip;
            try
            {
                Dispatcher.Run();
                UpdateVuTask.Run();
                TimeLogger("Remain");
                _SWatch.Stop();
                switch (_AtSetResult)
                {
                    case AutoSetResult.NoSignal:
                        if (PlatformManager.Default.Platform.ShowWeakTip)
                        {
                            WeakTip.Default.Write("AutoSet", MsgTipId.NoSignal, emergent: false, "", 5);
                        }
                        AutosetBOK = false;
                        break;
                    case AutoSetResult.ReadDataTimeout:
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs("Autosetting fail,reading channel data timeout!", EventBus.LogLevel.Debug));
                        //WeakTip.Default.Write("AutoSet", MsgTipId.AutoSetFail, emergent: false, "", 5);
                        AutosetBOK = false;
                        break;
                    case AutoSetResult.ReadCymometeTimeout:
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs("Autosetting fail,reading cymomete timeout!", EventBus.LogLevel.Debug));
                        //WeakTip.Default.Write("AutoSet", MsgTipId.AutoSetFail, emergent: false, "", 5);
                        AutosetBOK = false;
                        break;
                    case AutoSetResult.Finish:
                        break;
                    default:
                        break;
                }
                if (PlatformManager.Default.Platform.ShowWeakTip)
                {
                    _AtSetResult = AutoSetResult.Finish;
                }
                _IsStarting = false;
                if (_Oscilloscope.Search != null)
                {
                    _Oscilloscope.Search.Enabled = true;
                }
                DsoPrsnt.KeyBoardLockEnable = false;//键盘锁
            }
            catch (Exception ex)
            {
                EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(this, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
            }
            finally
            {
                if (PlatformManager.Default.Platform.ShowWeakTip)
                {
                    _AtSetResult = AutoSetResult.Finish;
                }
                _IsStarting = false;
                if (_Oscilloscope.Search != null)
                {
                    _Oscilloscope.Search.Enabled = true;
                }
                DsoPrsnt.KeyBoardLockEnable = false;//键盘锁
            }
        }
    }
    public enum AutoSetResult
    {
        NoSignal,
        ReadDataTimeout,
        ReadCymometeTimeout,
        Finish
    }
}

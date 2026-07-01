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
using System.Threading;

namespace ScopeX.Core
{
    public class ProbeCalibration
    {
        private readonly DsoPrsnt _Oscilloscope;

        private ChannelId _BackFocusId = ChannelId.C1; //焦点通道

        private record ChnlSetting(Boolean Active, AnaChnlScaleIndex AnaScaleIndex, AnaChnlCoupling Coupling);

        private record TimebaseSetting(AnaChnlAcqMode AcqMode/*采集模式*/, AnaChnlStorageMode StorageMode/*存储模式*/, Int32 StorageOption/*存储深度*/, AnaChnlTimebaseIndex TmScale/*时基*/);

        private record BackupSetting(Boolean CursorActive, Int32 MeasureIndicator, TriggerType TriggerType/*触发类型*/, TimebaseSetting TimebaseSetting, List<ChnlSetting> ChnlSetting/*通道模式*/, Boolean CymometerActive/*频率计*/);

        public String CalibMessage { get; private set; }

        public ProbeCalibration(DsoPrsnt dso)
        {
            _Oscilloscope = dso;
        }


        private BackupSetting SaveBackup()
        {
            var timebasesetting = new TimebaseSetting(_Oscilloscope.Timebase.Mode, _Oscilloscope.Timebase.StorageMode, _Oscilloscope.Timebase.StorageDepthOpt, _Oscilloscope.Timebase.ScaleIndex) { };

            var chnlsetting = new List<ChnlSetting>();
            foreach (var id in ChannelIdExt.GetAnalogs())
            {
                if (_Oscilloscope.TryGetChannel(id, out var prsnt))
                {
                    var anaprsnt = (AnalogPrsnt)prsnt;
                    chnlsetting.Add(new(anaprsnt.Active, anaprsnt.AnaScaleIndex, anaprsnt.Coupling));
                }
            }
            _BackFocusId = DsoPrsnt.FocusId;
            return new(_Oscilloscope.Cursor.Active, _Oscilloscope.Measure.Indicator, TriggerPrsnt.Type, timebasesetting, chnlsetting, _Oscilloscope.Cymometer.Active);
        }
        private void RestoreBackup(BackupSetting bs)
        {
            //还原光标、指示器
            _Oscilloscope.Cursor.Active = bs.CursorActive;
            _Oscilloscope.Measure.Indicator = bs.MeasureIndicator;

            //还原时基
            _Oscilloscope.Timebase.Mode = bs.TimebaseSetting.AcqMode;
            _Oscilloscope.Timebase.StorageMode = bs.TimebaseSetting.StorageMode;
            _Oscilloscope.Timebase.StorageDepthOpt = bs.TimebaseSetting.StorageOption;
            _Oscilloscope.Timebase.ScaleIndex = bs.TimebaseSetting.TmScale;
            _Oscilloscope.Timebase.ScaleByus = _Oscilloscope.Timebase.GetScale(_Oscilloscope.Timebase.ScaleIndex);

            //还原通道
            for (Int32 i = 0; i < bs.ChnlSetting.Count; i++)
            {
                if (_Oscilloscope.TryGetChannel((ChannelId)i, out var prsnt))
                {
                    var anaprsnt = (AnalogPrsnt)prsnt;

                    anaprsnt.Active = bs.ChnlSetting[i].Active;

                    anaprsnt.AnaScaleIndex = bs.ChnlSetting[i].AnaScaleIndex;

                    anaprsnt.Coupling = bs.ChnlSetting[i].Coupling;
                }
            }

            //还原触发
            _Oscilloscope.SetTrigger(bs.TriggerType);

            //焦点通道ID设置
            if (_Oscilloscope.TryGetChannel(_BackFocusId, out var p))
            {
                if (p.Active)
                {
                    DsoPrsnt.FocusId = _BackFocusId;
                }
                else
                {
                    _Oscilloscope.MoveFocusId();
                }
            }

            //还原频率计
            _Oscilloscope.Cymometer.Active = bs.CymometerActive;
            Dispatcher.IsScan = Dispatcher.GetScanState();

            //执行硬件命令并生效
            if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
            {
                Hd.Execute(msg!);
                Acquisition.Default.UpdateReadInfoList();
            }
        }

        /// <summary>
        /// 初始化所有挡位
        /// </summary>
        private void Init(ChannelId ProbeChlId)
        {
            _Oscilloscope.Cursor.Active = false;
            _Oscilloscope.Measure.Indicator = 0;

            //设置时基
            _Oscilloscope.Timebase.ScaleByus = _Oscilloscope.Timebase.GetScale(AnaChnlTimebaseIndex.Lv200u); //设置时基 200us
            _Oscilloscope.Timebase.Mode = AnaChnlAcqMode.HighRes;

            //设置存储深度
            _Oscilloscope.Timebase.StorageMode = AnaChnlStorageMode.Long;
            _Oscilloscope.Timebase.StorageDepthOpt = 1;//25K
            _Oscilloscope.Timebase.ResetPosIndex();

            //设置触发
            TriggerPrsnt.HoldoffType = DelayOpt.Time;
            TriggerPrsnt.HoldoffByps = TriggerPrsnt.MinHoldoffTime;
            var edge = _Oscilloscope.SetTrigger(TriggerType.Edge) as TrigEdgePrsnt;
            edge.Coupling = TriggerCoupling.DC;
            edge.Slope = EdgeSlope.Rise;
            edge.ResetPosIndex();

            //设置频率计
            _Oscilloscope.Cymometer.Active = false;

            //设置通道
            ChannelIdExt.GetAnalogs().ToList().ForEach(chnlid =>
            {
                if (chnlid == ProbeChlId)
                {
                    if (_Oscilloscope.TryGetChannel(chnlid, out var prsnt))
                    {
                        var anaprsnt = (AnalogPrsnt)prsnt;
                        anaprsnt.Active = true;
                        anaprsnt.ResetPosIndex();
                        anaprsnt.IsCoarse = true;
                    }
                }
            });

            Acquisition.Default.UpdateReadInfoList();
            Dispatcher.IsScan = Dispatcher.GetScanState();
            //执行硬件命令
            if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
            {
                Hd.Execute(msg!);
                //Clear掉Driver端缓存数据
                var samplingrate = new Dictionary<AcqDataType, Double>();
                Hd.AcqWave(false, true, Acquisition.Default.AllChnlReadInfo, ref samplingrate);
            }
        }

        private void Prepare()
        {
            //关闭除了模拟通道外的其他所有通道比如LA、BUS、Math
            foreach (IChnlPrsnt ptsnt in _Oscilloscope.TryGetRange((IChnlPrsnt c) => !ChannelIdExt.IsAnalog(c.Id) && c.Active))
            {
                ptsnt.Active = false;
            }

            //关闭通过失败、电源分析、抖动分析、搜索
            _Oscilloscope.PassFail.Active = false;
            foreach (var prsnt in _Oscilloscope.PwrAnalysisDictionary)
            {
                prsnt.Value.Active = false;
            }
            _Oscilloscope.Jitter.Active = false;
            _Oscilloscope.Search.Enabled = false;

            //关闭参数快照
            _Oscilloscope.Measure.SnapshotActive = false;

            TriggerPrsnt.Mode = TriggerMode.Auto;
            TriggerPrsnt.ResetState();
            //执行硬件命令
            if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
            {
                Hd.Execute(msg!);
                Thread.Sleep(100);
            }
        }
        public Int32 GetProbeCaliFinishedCount() => Hd.Calibration.GetProbeCaliFinishedCount();

        public Int32 GetProbeCaliTotalItemCount() => Hd.Calibration.GetProbeCaliTotalItemCount();

        public void ClearProbeCaliFinishedCount() => Hd.Calibration.ClearProbeCaliFinishedCount();
        public void ForceProbeCaliFinished() => Hd.Calibration.ForceProbeCaliFinished();
        public void Run(ChannelId probeChlId)
        {
            DsoPrsnt.KeyBoardLockEnable = true;
            try
            {
                var enable = Constants.ANALOG_TEMPERATURE_COMPENSATE;
                Prepare();
                Dispatcher.Cancel();
                UpdateVuTask.Cancel();
                var bs = SaveBackup();
                Init(probeChlId);
                Dispatcher.SoftReset();
                Dispatcher.DoClear();

                //关闭温漂补偿和风扇控制：因为它们要与通道底板串口通信读取温度信息，会打乱校准通信
                ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set("DebugVariant,bEnableAnalogTemperatureCompensate:false");
                ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set("DebugVariant,bEnableAutoFanControl:false");

                var bok = Hd.Calibration.AutoCaliProbeBaseline_Exec(probeChlId, null, out String msg) == 0;
                CalibMessage = msg;
                Hd.Calibration.ForceProbeCaliFinished();
                //恢复温漂补偿和风扇控制
                var enablestr = enable ? "true" : "false";
                ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set($"DebugVariant,bEnableAnalogTemperatureCompensate:{enablestr}");
                ExportHdFuncs.FactoryCaliScpiProc_SpecialData_Set("DebugVariant,bEnableAutoFanControl:true");

                RestoreBackup(bs);
                Dispatcher.Run();
                UpdateVuTask.Run();
            }
            catch (Exception ex)
            {
                Hd.Calibration.ForceProbeCaliFinished();
                _Oscilloscope.Search.Enabled = true;
                DsoPrsnt.KeyBoardLockEnable = false;
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Warn));
            }
            _Oscilloscope.Search.Enabled = true;
            DsoPrsnt.KeyBoardLockEnable = false;
        }
    }
}

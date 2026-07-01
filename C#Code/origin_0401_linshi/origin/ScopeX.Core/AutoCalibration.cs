using EventBus;
using ScopeX.ComModel;
using ScopeX.Core.Hardware;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace ScopeX.Core
{
    public class AutoCalibration
    {
        public Action? Use2SCPI { get; set; }

        private readonly DsoPrsnt _Oscilloscope;

        private ChannelId _FocusId = ChannelId.C1;                        //焦点通道

        private record ChnlSetting(Boolean Active, AnaChnlScaleIndex AnaScaleIndex, AnaChnlCoupling Coupling, Boolean FineTurnStatus);

        private record TimebaseSetting(AnaChnlAcqMode AcqMode/*采集模式*/, AnaChnlStorageMode StorageMode/*存储模式*/, Int32 StorageOption/*存储深度*/, AnaChnlTimebaseIndex TmScale/*时基*/);

        private record BackupSetting(Boolean CursorActive, Int32 MeasureIndicator, TriggerType TriggerType/*触发类型*/, TimebaseSetting TimebaseSetting, List<ChnlSetting> ChnlSetting/*通道模式*/, Boolean CymometerActive/*频率计*/);

        public AutoCalibration(DsoPrsnt dso)
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
                    chnlsetting.Add(new(anaprsnt.Active, anaprsnt.AnaScaleIndex, anaprsnt.Coupling, anaprsnt.Ylevel_SelectStatus));
                }
            }
            _FocusId = DsoPrsnt.FocusId;
            return new(_Oscilloscope.Cursor.Active, _Oscilloscope.Measure.Indicator, TriggerPrsnt.Type, timebasesetting, chnlsetting, _Oscilloscope.Cymometer.Active);
        }
        private void RestoreBackup(BackupSetting? bs)
        {
            if (bs is null)
            {
                return;
            }
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

                    anaprsnt.Ylevel_SelectStatus = bs.ChnlSetting[i].FineTurnStatus;
                }
            }

            //还原触发
            _Oscilloscope.SetTrigger(bs.TriggerType);

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
        private List<ChannelId> Init()
        {
            _Oscilloscope.Cursor.Active = false;
            _Oscilloscope.Measure.Indicator = 0;

            //设置时基
            _Oscilloscope.Timebase.ScaleByus = _Oscilloscope.Timebase.GetScale(AnaChnlTimebaseIndex.Lv5m);
            _Oscilloscope.Timebase.Mode = AnaChnlAcqMode.Normal;

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
            var _Chnls = new List<ChannelId>();
            ChannelIdExt.GetAnalogs().ToList().ForEach(chnlid =>
            {
                if (_Oscilloscope.TryGetChannel(chnlid, out var prsnt))
                {
                    var anaprsnt = (AnalogPrsnt)prsnt;
                    anaprsnt.Active = true;
                    anaprsnt.Bias = 0;
                    anaprsnt.ResetPosIndex();
                    anaprsnt.IsCoarse = true;
                    anaprsnt.Ylevel_SelectStatus = false;
                    _Chnls.Add(chnlid);
                }
            });

            Acquisition.Default.UpdateReadInfoList();
            Dispatcher.IsScan = Dispatcher.GetScanState();
            //执行硬件命令
            if (HdMsgFactory.TryMake(HdCmdFactory.Command, out var msg))
            {
                Hd.Execute(msg!);
                //Clear掉Driver端缓存数据
                var _SamplingRate = new Dictionary<AcqDataType, Double>();
                Hd.AcqWave(false, true, Acquisition.Default.AllChnlReadInfo, ref _SamplingRate);
            }

            return _Chnls;
        }

        private void Prepare()
        {
            //关闭除了模拟通道外的其他所有通道比如LA、BUS、Math
            foreach (IChnlPrsnt ptsnt in _Oscilloscope.TryGetRange((IChnlPrsnt c) => !ChannelIdExt.IsAnalog(c.Id) && c.Active))
            {
                ptsnt.Active = false;
            }

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


        public Int32 GetFinishedCount() => Hd.Calibration.GetFinishedCount();

        public Int32 GetTotalItemCount() => Hd.Calibration.GetTotalItemCount();

        public void ClearFinishedCount() => Hd.Calibration.ClearFinishedCount();

        #region Fan Test U2 调用参数

        public List<(String Description, Int32 Speed)> GetAllFanSpeed() => Hd.SystemMonitor.GetAllFanSpeed();

        public List<(String Description, Double Temperature)> GetAllTemperature() => Hd.SystemMonitor.GetAllTemperature();

        public Color GetColor(Int32 index)
        {
            Color color = Color.Red;
            if (index > (Int32)ChannelId.C8 && index < (Int32)ChannelId.M1)
            {
                index += (Int32)ChannelId.M1 - (Int32)ChannelId.C8 - 1;
            }
            if (index >= (Int32)ChannelId.M9 && index < (Int32)ChannelId.R1)
            {
                index += (Int32)ChannelId.R1 - (Int32)ChannelId.M9;
            }
            if (index >= (Int32)ChannelId.R9 && index < (Int32)ChannelId.D0)
            {
                index += (Int32)ChannelId.D0 - (Int32)ChannelId.R9;
            }
            color = ColorLookup.Default[$"{(ChannelId)index}"];
            return color;
        }

        public Boolean UsingUIParam
        {
            set => SystemMonitor.UsingUIParam = value;
        }

        public void SettingFanSpeed(Int32 Speed, Int32 Index, Boolean isRight = true) => Hd.SystemMonitor.SettingFanSpeed(Speed, Index, isRight);

        public Int32 ChannelFanSpeed
        {
            get => Hd.SystemMonitor.ChannelFanSpeed;
            set => Hd.SystemMonitor.ChannelFanSpeed = value;
        }

        public Int32 PcieFanSpeed
        {
            get => Hd.SystemMonitor.PcieSpeed;
            set => Hd.SystemMonitor.PcieSpeed = value;
        }

        public Int32 CpuFanSpeed
        {
            get => Hd.SystemMonitor.CpuSpeed;
            set => Hd.SystemMonitor.CpuSpeed = value;
        }

        public Double CtrollerPcieFanSpeedTempertuare => Hd.SystemMonitor.CtrollerPcieFanSpeedTempertuare;

        #endregion Fan Test U2 调用参数

        public void Run()
        {
            DsoPrsnt.KeyBoardLockEnable = true;
            BackupSetting? bs = null;
            try
            {
                //Prepare();
                //Dispatcher.Cancel();
                //UpdateVuTask.Cancel();

                //清除数据
                Dispatcher.SoftReset();
                Dispatcher.DoClear();
                Thread.Sleep(100);
                Dispatcher.Cancel();
                UpdateVuTask.Cancel();

                //保证Dispatcher.Run()完全结束
                Prepare();

                bs = SaveBackup();
                var ativechnls = Init();
                //Dispatcher.SoftReset();
                //Dispatcher.DoClear();

                var bok = Hd.Calibration.AutoCalibration(ativechnls, null, out var msg) == 0;

            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Warn));
            }
            finally
            {
                if (_Oscilloscope.Search != null)
                {
                    _Oscilloscope.Search.Enabled = true;
                }
                DsoPrsnt.KeyBoardLockEnable = false;
                RestoreBackup(bs);
            }

            try
            {
                Dispatcher.Run();
                UpdateVuTask.Run();
            }
            catch (Exception ex)
            {
                EventBroker.Instance.GetEvent<LogEventArgs>().Publish(this, new LogEventArgs(ex, LogLevel.Warn));
            }
        }
    }
}

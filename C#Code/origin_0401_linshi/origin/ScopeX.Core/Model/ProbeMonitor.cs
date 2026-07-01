using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using EventBus;
using ScopeX.ComModel;
using ScopeX.Core.Model;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;

namespace ScopeX.Core
{
    public class MiscMonitor
    {
        private static CancellationTokenSource? _Cts;
        public static MiscMonitor Default = new MiscMonitor();
        private static Task? _WorkTask;
        public Boolean EnableAWGPrtected;
        private Boolean _IsRunning = true;
        private Int32 _WorkTaskNum = 0;
        private Dictionary<ChannelId, Boolean> _ChanneProbeStatelMap = new Dictionary<ChannelId, Boolean>()
        {
            [ChannelId.C1] = false,
            [ChannelId.C2] = false,
            [ChannelId.C3] = false,
            [ChannelId.C4] = false,
        };

        public void Close()
        {
            Cancel();
        }
        public void Run()
        {
            #region 解决启动多个线程的问题，试产临时修改
            Int32 count = 0;
            while (_WorkTaskNum > 0)
            {
                _IsRunning = false;
                Thread.Sleep(10);
                count++;
                if (count > 1000)
                {
                    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"MiscMonitor Run Error !  _WorkTaskNum = {_WorkTaskNum}", LogLevel.Warn));
                    return;
                }
            }

            _IsRunning = true;
            #endregion
            _Cts = new();
            var token = _Cts.Token;
            var sw = Stopwatch.StartNew();

            _WorkTask = new Task(() =>
            {
                _WorkTaskNum++;

                while (_IsRunning)
                {
                    if (_Cts.Token.IsCancellationRequested)
                    {
                        break;
                    }
                    try
                    {
                        ProbeHandle();
                        AWGProtectCheck();

                        DsoModel.Default.Timebase.Ext10MHzLocked = Hd.MiscFunc("Ext10MHzLocked", "") == "true";
                    }
                    catch (Exception ex)
                    {
                        //break;
                    }
                    Thread.Sleep(100);
                }
                _WorkTaskNum--;

            }, _Cts.Token, TaskCreationOptions.LongRunning | TaskCreationOptions.PreferFairness);
            _WorkTask.Start();
        }

        private Boolean ProbeHandle()
        {
            var probestatus = Hd.ProbeManager?.Read();
            if (probestatus == null)
            {
                return false;
            }
            foreach (var channel in probestatus)
            {
                var ach = (AnalogModel)DsoModel.Default.GetChannel(channel.Key);
                ach.Conditioning.ProbeGainCaliRatioDefVal = (double)channel.Value.CaliGain;
                ach.Conditioning.ProbeOffsetCaliBiasDefVal = (double)channel.Value.CaliBias;

                ach.SerailNumber = channel.Value.SerailNumber.Replace("\0", "");
                ach.ProbeConnected = channel.Value.IsConnected;
                ach.ProbeHwVerion = channel.Value.ProbeVersion;

                if (channel.Value.IsConnected && !_ChanneProbeStatelMap[channel.Key])//接入探头
                {
                    //ach.Conditioning.Unit = ach.Conditioning.ProbeUnit.ToString();
                    ach.ProbeLedLight = false;
                    WeakTip.Default.Write("Probe", MsgTipId.ProbeConnected, mark: $"{channel.Key}");
                    //EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"探头连接通道: {channel.Key},探头信息: {channel.Value.SerailNumber}", LogLevel.Info));
                }
                else if (!channel.Value.IsConnected && _ChanneProbeStatelMap[channel.Key])//移除探头
                {
                    ach.ProbeLedLight = false;
                    WeakTip.Default.Write("Probe", MsgTipId.ProbeRemoved, mark: $"{channel.Key}");
                }
                _ChanneProbeStatelMap[channel.Key] = channel.Value.IsConnected;

                //if (channel.Value.Connected)
                //{
                //    EventBroker.Instance.GetEvent<LogEventArgs>().Publish(null, new LogEventArgs($"探头连接通道: {channel.Key},探头信息: {channel.Value.SerailNumber}", LogLevel.Info));

                //}
                if (channel.Value.IsPushed == false || channel.Value.IsConnected == false)
                {
                    continue;
                }

                switch (ach.ProbeBtnType)
                {
                    case ProbeKeyType.Headlight:
                        {
                            ach.ProbeLedLight = !ach.ProbeLedLight;
                            Hd.ProbeManager.CtrlHeadLight(channel.Key, ach.ProbeLedLight);
                        }
                        break;
                    case ProbeKeyType.RunOrStop:
                        {
                            if (TriggerPrsnt.State != SysState.Stop)
                            {
                                Dispatcher.Stop();
                            }
                            else
                            {
                                Dispatcher.Resume();
                            }
                        }
                        break;
                    case ProbeKeyType.Clear:
                        {
                            EventBus.EventBroker.Instance.GetEvent<ProbeKeyType>().Publish(this, ProbeKeyType.Clear);
                        }
                        break;
                    case ProbeKeyType.ForceTrig:
                        {
                            TriggerPrsnt.Force();
                        }
                        break;
                    case ProbeKeyType.NoOps:
                        break;
                    default:
                        break;
                }
                channel.Value.IsPushed = false;
            }
            return true;
        }
        #region AWG
        private DateTime lastTime_AwgCheck = DateTime.MinValue;
        private void AWGProtectCheck()
        {
            if (!EnableAWGPrtected)
            {
                return;
            }

            if (DateTime.Now - lastTime_AwgCheck < TimeSpan.FromMilliseconds(0.4))
            {
                return;
            }

            String resultStr = Hd.MiscFunc("AWGProtectState", "");
            Regex regex = new Regex(@"\d,\d");

            if (!String.IsNullOrEmpty(resultStr) && regex.IsMatch(resultStr))
            {
                var protectedchs = resultStr.Split(',');
                for (Int32 i = 0; i < protectedchs.Length; i++)
                {
                    if (Int32.Parse(protectedchs[i]) == 1)
                    {
                        ArbWfmGenModel awgChnl = DsoModel.Default.GetWfmGenerator(ChannelId.AWG1 + i);
                        if (!awgChnl.Active)
                        {
                            continue;
                        }
                        awgChnl.Active = false;
                        switch (i)
                        {
                            case 0:
                                WeakTip.Default.Write("AWG", MsgTipId.AWGProtected1);
                                break;
                            case 1:
                                WeakTip.Default.Write("AWG", MsgTipId.AWGProtected2);
                                break;
                            case 2:
                                WeakTip.Default.Write("AWG", MsgTipId.AWGProtected3);
                                break;
                            case 3:
                                WeakTip.Default.Write("AWG", MsgTipId.AWGProtected4);
                                break;
                            default:
                                break;
                        }

                    }
                }

            }

            lastTime_AwgCheck = DateTime.Now;
        }
        #endregion AWG
        private void Mso7000ALedCtrl()
        {
            //Hd.OuterLedCtrl();
        }
        public void Cancel()
        {
            _Cts?.Cancel();
            try
            {
                if (_Cts != null)
                {
                    _WorkTask?.Wait(_Cts.Token);
                }
                else
                {
                    _WorkTask?.Wait();
                }
            }
            catch (AggregateException e)
            {
                e.Handle(x => x is OperationCanceledException);
            }
            catch (OperationCanceledException)
            { }
        }
        public void Resume()
        {

        }
    }
}

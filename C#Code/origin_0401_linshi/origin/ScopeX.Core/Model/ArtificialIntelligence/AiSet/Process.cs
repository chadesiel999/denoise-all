using ScopeX.ComModel;
using ScopeX.Core.Model.ArtificialIntelligence.AiSet;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ScopeX.Core
{
    internal enum AiSetActionType
    {
        Full,
        IdentifyOnly,
        AutoScaleOnly,
        TimebaseOnly,
        AcqResourceAdaptive,
        RestoreLast
    }

    internal class  AiSetProcess
    {
        private enum AiSetTipOutput
        {
            AiTipInfo,
            WeakTip
        }

        private static AiSetProcess _Default;
        private readonly Dictionary<ChannelId, AiSetSnapshot> _lastSnapshots = new();

        private AiSetProcess() { }

        private sealed class AiSetSnapshot
        {
            public Int32 ScaleIndex { get; init; }
            public Double TimebaseScaleByus { get; init; }
            public Boolean AverageEnable { get; init; }
            public DateTime CapturedAt { get; init; }

            public Boolean CurAiSetEnable { get; init; }
            public NoiseRedutionMethod CurNoiseRedutionMethod { get; init; }
            public Boolean CurAINoiseReductionEnable { get; init; }
            public SubbandCtrlMethod CurSubbandCtrlMethod { get; init; }

            public Boolean MultiDomainActive { get; init; }
            public Boolean MultiDomainParameterTuningEnable { get; init; }
            public MultiDomainFigureEnum MultiDomainCurFigureType { get; init; }
            public Boolean MultiDomainCurFigureEnable { get; init; }
            public Dictionary<MultiDomainFigureEnum, Boolean> MultiDomainFigureEnableStates { get; init; } = new();
            public Double MultiDomainTimeScaleForTimeFreq { get; init; }
            public Boolean MultiDomainThreeDimensionalEnable { get; init; }

            public ChannelId VsaSource { get; init; }
            public VsaGraphType VsaCurGraphType { get; init; }
            public Boolean VsaEqualizerEnabled { get; init; }
            public VsaFormatOpt VsaFormatOpt { get; init; }
            public Boolean VsaCurGraphEnabled { get; init; }
            public Boolean VsaEnabled { get; init; }
            public Int32 StorageDepthOpt { get; init; }
        }


        public static AiSetProcess Default 
        { 
            get 
            {
                if (_Default == null)
                { 
                    _Default = new AiSetProcess();
                }
                return _Default; 
            } 
        }

        public void Execute(ChannelId channelId, AiSetActionType actionType, String? signalTypeOverride = null)
        {
            DsoModel.Default.ArtificialIntelligence.ClearAiTipInfo();

            switch (actionType)
            {
                case AiSetActionType.IdentifyOnly:
                    SaveSnapshot(channelId);
                    ExecuteIdentifyOnly(channelId);
                    WeakTip.Default.Write("AiSet", "信号识别已开启", emergent: false, "", 2);
                    break;
                case AiSetActionType.AutoScaleOnly:
                    SaveSnapshot(channelId);
                    ExecuteAutoScaleOnly(channelId);
                    WeakTip.Default.Write("AiSet", "自动幅度档调整已完成", emergent: false, "", 2);
                    break;
                case AiSetActionType.TimebaseOnly:
                    SaveSnapshot(channelId);
                    ExecuteTimebaseOnly(channelId);
                    WeakTip.Default.Write("AiSet", "时基自动设置已完成", emergent: false, "", 2);
                    break;
                case AiSetActionType.AcqResourceAdaptive:
                    SaveSnapshot(channelId);
                    ExecuteAcqResourceAdaptive(channelId);
                    break;
                case AiSetActionType.RestoreLast:
                    RestoreLast(channelId);
                    break;
                case AiSetActionType.Full:
                default:
                    SaveSnapshot(channelId);
                    AiSet(channelId, signalTypeOverride);
                    break;
            }
            if (actionType != AiSetActionType.RestoreLast)
                WeakTip.Default.Write("AiSet", "AiSet已执行完成", emergent: false, "", 2);
        }

        public Boolean CanRestore(ChannelId channelId) => _lastSnapshots.ContainsKey(channelId);

        public void AiSet(ChannelId channelId, String? signalTypeOverride = null)
        {
            try 
            {

                //double acrms = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Stddev", channelId) ?? Double.NaN;
                //if (acrms < 3 || Double.IsNaN(acrms))
                //{

                //    DsoModel.Default.ArtificialIntelligence.AverageEnable = true;
                //    Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
                //    DsoModel.Default.ArtificialIntelligence.AppenAiTipInfo("信号较弱，已自动打开平均降噪");
                //    return;
                //}

                //信号分类
                BaseSignal? signal;
                String sigtype = String.IsNullOrWhiteSpace(signalTypeOverride)
                    ? DsoModel.Default.IntelligentChartManager.GetMatchType(new double[0, 0])
                    : signalTypeOverride;
                signal = buildSignal(sigtype);
                if (signal == null)
                {
                    WeakTip.Default.Write("AiSet", "未知信号", emergent: false, "", 2);
                    return;
                }

                if (signal is RadarSignal) 
                {
                    AutoCfgVol(channelId);
                    SetBitWidth();
                    signal.AiSet();
                    return;
                }
                if (signal is CommunicationSignal)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus = 0.05;
                    AutoCfgVol(channelId);
                    signal.AiSet();
                    return;
                }

                Hd.TryGetData(ChannelType.ReconfigDBI, "FrequencyMeter", out Object? frequencyObj);
                var freq = (double)frequencyObj;

                //幅度档设置
                if (freq > 10e6) 
                {
                    AutoCfgVol(channelId);
                }


                ////分辨率设置，通过打开TS自适应功能实现

                if (freq < 10e6)
                {
                    //TODO 调整存储深度和时基
                    DsoModel.Default.Timebase.StorageDepthOpt = 0;
                    if (freq < 300)
                    {
                        DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus = 20000;
                    }
                    else if (freq < 500)
                    {
                        DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus = 1000;
                    }
                    else 
                    {
                        DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus = 1;
                        AutoCfgVol(channelId); //大时基挡刷新太慢，无法立刻调整幅度档位
                    }

                }
                else 
                {
                    //TODO 调整存储深度和时基（单音信号调整，多音不调整，时基调整到10ns）
                    SetBitWidth();


                    //采样率、时基设置
                    if (freq > 19e9)
                    {
                        DsoModel.Default.Timebase.StorageDepthOpt = 1;
                        DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus = 0.00005;
                    }
                    else 
                    {
                        DsoModel.Default.ArtificialIntelligence.SetTimeBaseScale(channelId, sigtype);
                    } 
                    
                }

                //BaseInfoOutput(channelId, signal);
                DsoPrsnt.DefaultDsoPrsnt.ArtificialIntelligence.CurAiSetEnable = true;




                ////开启信号类型对应的功能
                signal.AiSet();

            }
            catch (Exception ex)
            {

            }
        }

        private void ExecuteIdentifyOnly(ChannelId channelId)
        {
            DsoModel.Default.ArtificialIntelligence.ClearAiTipInfo();
            DsoModel.Default.ArtificialIntelligence.CurAiSetEnable = true;

            //var sigtype = DsoModel.Default.ArtificialIntelligence.GetSignalType(channelId);;
            //var signal = buildSignal(sigtype);
            //BaseInfoOutput(channelId, signal);
        }

        private void ExecuteAutoScaleOnly(ChannelId channelId)
        {
            DsoModel.Default.ArtificialIntelligence.ClearAiTipInfo();
            AutoCfgVol(channelId);
            DsoModel.Default.ArtificialIntelligence.AppenAiTipInfo("已完成自动幅度档调整");
        }

        private void ExecuteTimebaseOnly(ChannelId channelId)
        {
            DsoModel.Default.ArtificialIntelligence.ClearAiTipInfo();
            _ = DsoModel.Default.ArtificialIntelligence.GetSignalType(channelId);
            DsoModel.Default.ArtificialIntelligence.SetTimeBaseScale(channelId, "正弦信号");
            DsoModel.Default.ArtificialIntelligence.AppenAiTipInfo("已按识别结果更新时基");
        }

        private void ExecuteAcqResourceAdaptive(ChannelId channelId)
        {
            Hd.TryGetData(ChannelType.ReconfigDBI, "FrequencyMeter", out Object? frequencyObj);
            var freq = (double)frequencyObj;
            double freqMin = 0;
            if (freq >= 90e6) 
            {
                Hd.TryGetData(ChannelType.ReconfigDBI, "FrequencyMin", out Object? frequencyMinObj);
                freqMin = (double)frequencyMinObj;
            }

            //幅度档设置
            if (freq > 10e6)
            {
                AutoCfgVol(channelId);
            }

            if (freq < 10e6)
            {
                //TODO 调整存储深度和时基
                DsoModel.Default.Timebase.StorageDepthOpt = 0;
                if (freq < 300)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus = 20000;
                }
                else if (freq < 500)
                {
                    DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus = 1000;
                }
                else
                {
                    DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus = 1;
                    AutoCfgVol(channelId); //大时基挡刷新太慢，无法立刻调整幅度档位
                }

            }
            else
            {
                //TODO 调整存储深度和时基（单音信号调整，多音不调整，时基调整到10ns）
                SetBitWidth();

                if (freqMin == 0 || freq - freqMin > 200e6)
                {

                }
                else 
                {
                    //采样率、时基设置
                    if (freq > 19e9)
                    {
                        DsoModel.Default.Timebase.StorageDepthOpt = 1;
                        DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus = 0.00005;
                    }
                    else
                    {
                        DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus = 1e6 / freq;
                    }
                }
            }

            // TODO: 采集资源自适应执行逻辑待补充。
            //SetBitWidth(AiSetTipOutput.WeakTip);

            //Hd.TryGetData(ChannelType.ReconfigDBI, "FrequencyMeter", out Object? frequencyObj);
            //if (frequencyObj is double) 
            //{
            //    var freq = (double)frequencyObj;

            //}
            //WeakTip.Default.Write("AiSet", "采集资源自适应已执行", emergent: false, "", 2);
        }

        private void SaveSnapshot(ChannelId channelId)
        {
            var channel = DsoModel.Default.GetChannel(channelId);
            if (channel == null)
                return;

            var mdModel = DsoModel.Default.MultiDomain;
            var mdPrsnt = DsoPrsnt.DefaultDsoPrsnt.MultiDomain;
            var vap = DsoPrsnt.DefaultDsoPrsnt.VectorAnalysis;
            var gd = vap.GenerateDigtalPrsnt;
            var curVsaGraph = gd.GetCurVsaGraphPrsnt;
            _lastSnapshots[channelId] = new AiSetSnapshot()
            {
                ScaleIndex = channel.Conditioning.ScaleIndex,
                TimebaseScaleByus = DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus,
                AverageEnable = DsoModel.Default.ArtificialIntelligence.AverageEnable,
                CapturedAt = DateTime.Now,
                CurAiSetEnable = DsoModel.Default.ArtificialIntelligence.CurAiSetEnable,
                CurNoiseRedutionMethod = DsoModel.Default.ArtificialIntelligence.CurNoiseRedutionMethod,
                CurAINoiseReductionEnable = DsoModel.Default.ArtificialIntelligence.CurAINoiseReductionEnable,
                CurSubbandCtrlMethod = DsoModel.Default.ArtificialIntelligence.CurSubbandCtrlMethod,
                MultiDomainActive = mdModel.Active,
                MultiDomainParameterTuningEnable = mdModel.ParameterTuningEnable,
                MultiDomainCurFigureType = mdModel.CurFigureType,
                MultiDomainCurFigureEnable = mdModel.CurFigureEnable,
                MultiDomainFigureEnableStates = CaptureMultiDomainFigureEnableStates(mdModel),
                MultiDomainTimeScaleForTimeFreq = mdModel.TimeScaleForTimeFreq,
                MultiDomainThreeDimensionalEnable = mdPrsnt.ThreeDimensionalEnable,
                VsaSource = vap.Source,
                VsaCurGraphType = gd.CurGraphType,
                VsaEqualizerEnabled = gd.EqualizerEnabled,
                VsaFormatOpt = gd.FormatOpt,
                VsaCurGraphEnabled = curVsaGraph?.Enabled ?? false,
                VsaEnabled = vap.Enabled,
                StorageDepthOpt = DsoModel.Default.Timebase.StorageDepthOpt,
            };
        }

        private Boolean RestoreLast(ChannelId channelId)
        {
            DsoModel.Default.ArtificialIntelligence.ClearAiTipInfo();
            if (!_lastSnapshots.TryGetValue(channelId, out AiSetSnapshot snapshot))
            {
                return false;
            }

            var channel = DsoModel.Default.GetChannel(channelId);
            if (channel == null)
                return false;

            if (channel.Conditioning.ScaleIndex != snapshot.ScaleIndex)
            {
                channel.Conditioning.ScaleIndex = snapshot.ScaleIndex;
                Hardware.HdCmdFactory.Push(HdCmd.ChnlScaleIndex);
            }

            DsoPrsnt.DefaultDsoPrsnt.Timebase.ScaleByus = snapshot.TimebaseScaleByus;

            if (DsoModel.Default.ArtificialIntelligence.AverageEnable != snapshot.AverageEnable)
            {
                DsoModel.Default.ArtificialIntelligence.AverageEnable = snapshot.AverageEnable;
                Hardware.HdCmdFactory.Push(HdCmd.ArtificialIntelligence);
            }

            var aiPrsnt = DsoPrsnt.DefaultDsoPrsnt.ArtificialIntelligence;
            if (aiPrsnt.CurNoiseRedutionMethod != snapshot.CurNoiseRedutionMethod)
                aiPrsnt.CurNoiseRedutionMethod = snapshot.CurNoiseRedutionMethod;
            if (aiPrsnt.CurAINoiseReductionEnable != snapshot.CurAINoiseReductionEnable)
                aiPrsnt.CurAINoiseReductionEnable = snapshot.CurAINoiseReductionEnable;
            if (aiPrsnt.CurSubbandCtrlMethod != snapshot.CurSubbandCtrlMethod)
                aiPrsnt.CurSubbandCtrlMethod = snapshot.CurSubbandCtrlMethod;

            DsoModel.Default.ArtificialIntelligence.CurAiSetEnable = snapshot.CurAiSetEnable;
            DsoModel.Default.Timebase.StorageDepthOpt = snapshot.StorageDepthOpt;

            RestoreMultiDomainFromSnapshot(snapshot);
            RestoreVectorAnalysisFromSnapshot(snapshot);

            return true;
        }

        private static void RestoreMultiDomainFromSnapshot(AiSetSnapshot snapshot)
        {
            var mdModel = DsoModel.Default.MultiDomain;
            var mdPrsnt = DsoPrsnt.DefaultDsoPrsnt.MultiDomain;

            // 先按“先关后开”逐图恢复，避免只恢复当前图窗导致漏关。
            RestoreAllFigureEnableStates(mdModel, snapshot.MultiDomainFigureEnableStates);

            if (mdModel.CurFigureType != snapshot.MultiDomainCurFigureType)
                mdModel.CurFigureType = snapshot.MultiDomainCurFigureType;
            if (mdModel.CurFigureEnable != snapshot.MultiDomainCurFigureEnable)
                mdModel.CurFigureEnable = snapshot.MultiDomainCurFigureEnable;
            if (mdModel.TimeScaleForTimeFreq != snapshot.MultiDomainTimeScaleForTimeFreq)
                mdModel.TimeScaleForTimeFreq = snapshot.MultiDomainTimeScaleForTimeFreq;
            if (mdPrsnt.ThreeDimensionalEnable != snapshot.MultiDomainThreeDimensionalEnable)
                mdPrsnt.ThreeDimensionalEnable = snapshot.MultiDomainThreeDimensionalEnable;
            if (mdModel.ParameterTuningEnable != snapshot.MultiDomainParameterTuningEnable)
                mdModel.ParameterTuningEnable = snapshot.MultiDomainParameterTuningEnable;
            if (mdModel.Active != snapshot.MultiDomainActive)
                mdModel.Active = snapshot.MultiDomainActive;
        }

        private static Dictionary<MultiDomainFigureEnum, Boolean> CaptureMultiDomainFigureEnableStates(MultiDomainModel mdModel)
        {
            var states = new Dictionary<MultiDomainFigureEnum, Boolean>();
            foreach (MultiDomainFigureEnum figureType in Enum.GetValues(typeof(MultiDomainFigureEnum)))
            {
                states[figureType] = mdModel.GetFigureMathModel(figureType)?.Enabled ?? false;
            }
            return states;
        }

        private static void RestoreAllFigureEnableStates(MultiDomainModel mdModel, Dictionary<MultiDomainFigureEnum, Boolean> targetStates)
        {
            var originalFigureType = mdModel.CurFigureType;

            foreach (MultiDomainFigureEnum figureType in Enum.GetValues(typeof(MultiDomainFigureEnum)))
            {
                var currentEnabled = mdModel.GetFigureMathModel(figureType)?.Enabled ?? false;
                var targetEnabled = targetStates.TryGetValue(figureType, out var enabled) && enabled;
                if (currentEnabled && !targetEnabled)
                {
                    mdModel.CurFigureType = figureType;
                    mdModel.CurFigureEnable = false;
                }
            }

            foreach (MultiDomainFigureEnum figureType in Enum.GetValues(typeof(MultiDomainFigureEnum)))
            {
                var currentEnabled = mdModel.GetFigureMathModel(figureType)?.Enabled ?? false;
                var targetEnabled = targetStates.TryGetValue(figureType, out var enabled) && enabled;
                if (!currentEnabled && targetEnabled)
                {
                    mdModel.CurFigureType = figureType;
                    mdModel.CurFigureEnable = true;
                }
            }

            mdModel.CurFigureType = originalFigureType;
        }

        private static void RestoreVectorAnalysisFromSnapshot(AiSetSnapshot snapshot)
        {
            var vap = DsoPrsnt.DefaultDsoPrsnt.VectorAnalysis;
            var gd = vap.GenerateDigtalPrsnt;
            if (vap.Source != snapshot.VsaSource)
                vap.Source = snapshot.VsaSource;
            if (gd.CurGraphType != snapshot.VsaCurGraphType)
                gd.CurGraphType = snapshot.VsaCurGraphType;
            if (gd.EqualizerEnabled != snapshot.VsaEqualizerEnabled)
                gd.EqualizerEnabled = snapshot.VsaEqualizerEnabled;
            if (gd.FormatOpt != snapshot.VsaFormatOpt)
                gd.FormatOpt = snapshot.VsaFormatOpt;
            var curGraph = gd.GetCurVsaGraphPrsnt;
            if (curGraph != null && curGraph.Enabled != snapshot.VsaCurGraphEnabled)
                curGraph.Enabled = snapshot.VsaCurGraphEnabled;
            if (vap.Enabled != snapshot.VsaEnabled)
                vap.Enabled = snapshot.VsaEnabled;
        }

        private void BaseInfoOutput(ChannelId channelId, BaseSignal signal)
        {
            if (signal is SimpleSignal)
            {
                OutputSimpleSignalInfo(channelId, signal.type);
            }
            else if (signal is RadarSignal)
            {
                OutputRadarSignalInfo();
            }
        }

        private void OutputSimpleSignalInfo(ChannelId channelId, String? signalType)
        {
            Double amplitude = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Pk2Pk", channelId) ?? Double.NaN;
            Double frequency = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Freq", channelId) ?? Double.NaN;

            AppendTip("信号幅度", amplitude, Prefix.Milli, QuantityUnit.VoltagePeakPeak);
            AppendTip("信号频率", frequency, Prefix.Empty, QuantityUnit.Hertz);

            if (String.Equals(signalType, "Square", StringComparison.OrdinalIgnoreCase))
            {
                Double duty = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Duty", channelId) ?? Double.NaN;
                Double pulseWidth = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("PWidth", channelId) ?? Double.NaN;
                Double dutyPercent = duty;
                if (Double.IsFinite(dutyPercent) && Math.Abs(dutyPercent) <= 1.0)
                {
                    dutyPercent *= 100.0;
                }

                AppendTip("占空比", dutyPercent, Prefix.Empty, QuantityUnit.Percent);
                AppendTip("脉冲宽度", pulseWidth, Prefix.Empty, QuantityUnit.Second);
            }

            if (String.Equals(signalType, "Tri", StringComparison.OrdinalIgnoreCase))
            {
                Double riseTime = DsoModel.Default.Meas.Calc.ForceGetResultOrCalc("Rise", channelId) ?? Double.NaN;
                AppendTip("上升时间", riseTime, Prefix.Empty, QuantityUnit.Second);
            }
        }

        private void OutputRadarSignalInfo()
        {
            var vectorAnalysisModel = DsoModel.Default.VectorAnalysisModel;
            var (carrierFreq, symbolRate, evm, snr) = vectorAnalysisModel.EstimateCarrierAndSymbolRate();
            AppendTip("符号速率", symbolRate, Prefix.Empty, QuantityUnit.Hertz);
            AppendTip("载波频率", carrierFreq, Prefix.Empty, QuantityUnit.Hertz);
            AppendTip("EVM", evm, Prefix.Empty, QuantityUnit.Percent);
            AppendTip("SNR", snr, Prefix.Empty, QuantityUnit.Decibel);
        }

        private static void AppendTip(String name, Double value, Prefix prefix, QuantityUnit unit)
        {
            DsoModel.Default.ArtificialIntelligence.AppenAiTipInfo(
                $"{name}为{new Quantity(value, prefix, unit).ToString("##0.000", true)}");
        }

        private void SetBitWidth(AiSetTipOutput tipOutput = AiSetTipOutput.AiTipInfo)
        {
            Hd.TryGetData(ChannelType.ReconfigDBI, "EnableIds", out Object? enableIds);
            Int32[] ids = enableIds switch
            {
                Int32[] ary => ary,
                Int32 one => new Int32[] { one },
                _ => Array.Empty<Int32>()
            };

            if (ids.Length > 1)
            {
                EnableAdaptiveNoiseReduction();
                AppendBitWidthTip(12, tipOutput);
                return;
            }

            //if (ids.Length == 1 && (ids[0] == 2 || ids[0] == 3))
            //{
            //    EnableAdaptiveNoiseReduction();
            //    if (TryGetBitWidth(out Int32 bitWidthByFixed))
            //        AppendBitWidthTip(bitWidthByFixed, tipOutput);
            //    return;
            //}

            DsoPrsnt.DefaultDsoPrsnt.ArtificialIntelligence.CurSubbandCtrlMethod = SubbandCtrlMethod.BitWidthAdaptive;
            if (!WaitReconfigSetFinish(10, 500))
            {
                AppendAiSetMessage("分辨率设置超时，未能完成自适应配置", tipOutput);
                return;
            }

            if (TryGetBitWidth(out Int32 bitWidth))
                AppendBitWidthTip(bitWidth, tipOutput);
        }

        private void EnableAdaptiveNoiseReduction()
        {
            DsoPrsnt.DefaultDsoPrsnt.ArtificialIntelligence.CurNoiseRedutionMethod = NoiseRedutionMethod.AdaptiveFilter;
            DsoPrsnt.DefaultDsoPrsnt.ArtificialIntelligence.CurAINoiseReductionEnable = true;
        }

        private Boolean TryGetBitWidth(out Int32 bitWidth)
        {
            Hd.TryGetData(ChannelType.ReconfigDBI, "BitWidth", out Object? bitWidthObj);
            if (bitWidthObj is Int32 value)
            {
                bitWidth = value;
                return true;
            }
            bitWidth = 12;
            return false;
        }

        private static Int32 GetSamplingRateByBitWidth(Int32 bitWidth)
        {
            return bitWidth switch
            {
                14 => 20,
                15 => 10,
                16 => 5,
                _ => 80
            };
        }

        private void AppendBitWidthTip(Int32 bitWidth, AiSetTipOutput tipOutput)
        {
            Int32 samplingRate = GetSamplingRateByBitWidth(bitWidth);
            if (bitWidth >= 16)
            {
                AppendAiSetMessage($"设置为≥15Bit模式", tipOutput);
                AppendAiSetMessage($"采样率为{samplingRate}GSPS，带宽为1GHz", tipOutput);
            }
            else
            {
                AppendAiSetMessage($"设置为{bitWidth}Bit模式", tipOutput);
                if (bitWidth == 15)
                {
                    AppendAiSetMessage($"采样率为{samplingRate}GSPS，带宽为3GHz", tipOutput);
                }
                else if (bitWidth == 14)
                {
                    AppendAiSetMessage($"采样率为{samplingRate}GSPS，带宽为5GHz", tipOutput);
                }
                else if (bitWidth == 12)
                {
                    AppendAiSetMessage($"采样率为{samplingRate}GSPS，带宽为20GHz", tipOutput);
                }
            }
        }

        private static void AppendAiSetMessage(String message, AiSetTipOutput tipOutput)
        {
            if (tipOutput == AiSetTipOutput.WeakTip)
            {
                WeakTip.Default.Write("AiSet", message, emergent: false, "", 2);
                return;
            }

            DsoModel.Default.ArtificialIntelligence.AppenAiTipInfo(message);
        }

        private Boolean WaitReconfigSetFinish(Int32 maxRetries, Int32 retryDelayMs)
        {
            for (Int32 retry = 0; retry < maxRetries; retry++)
            {
                Hd.TryGetData(ChannelType.ReconfigDBI, "SetFinish", out Object? flag);
                if (flag is Boolean finished && finished)
                    return true;
                Thread.Sleep(retryDelayMs);
            }
            return false;
        }

        private void AutoCfgVol(ChannelId chnlId)
        {
            var chnlmodel = DsoModel.Default.GetChannel(chnlId);
            if (chnlmodel == null)
                return;

            // 每次仅调整一档时，适当提高重试次数，给收敛留出空间
            Int32 maxAdjustTimes = 12;
            Double targetLower = Constants.SAMPS_PER_YDIV * 6;
            Double targetUpper = Constants.SAMPS_PER_YDIV * 8;
            int flag = 0;

            for (Int32 id = 0; id < maxAdjustTimes; id++)
            {
                var pkg = DsoModel.Default.ArtificialIntelligence.GetScreenData(chnlId);
                if (pkg == null || pkg.data == null || pkg.data.Length == 0)
                    break;

                Int32 vpp = pkg.data.Max() - pkg.data.Min();

                // 已在目标范围，直接结束
                if (vpp > targetLower && vpp <= targetUpper)
                    break;

                // 一步一步调档：超下限只升一档，超上限只降一档
                Int32 step = 0;
                if (vpp < targetLower)
                {
                    if (flag == 1)
                        break;

                    step = -1;
                    flag = -1;
                } else if (vpp > targetUpper)
                {
                    if (flag == -1)
                    {
                        chnlmodel.Conditioning.ScaleIndex += 1;
                        Hardware.HdCmdFactory.Push(HdCmd.ChnlScaleIndex);
                        break;
                    }
                    
                    step = 1;
                    flag = 1;
                }
                else
                    break;

                if (!ApplyScaleStepAndWait(chnlId, pkg.data, vpp, chnlmodel, step))
                    break;
            }
        }

        private Boolean ApplyScaleStepAndWait(ChannelId chnlId, UInt16[] oldData, Int32 oldVpp, ChannelModel chnlmodel, Int32 step)
        {
            Int32 min = (Int32)chnlmodel.Conditioning.ScaleMinIndex;
            Int32 max = (Int32)chnlmodel.Conditioning.ScaleMaxIndex;
            Int32 curr = (Int32)chnlmodel.Conditioning.ScaleIndex;
            Int32 next = Math.Clamp(curr + step, min, max);
            if (next == curr)
                return false;

            chnlmodel.Conditioning.ScaleIndex = next;
            Hardware.HdCmdFactory.Push(HdCmd.ChnlScaleIndex);

            // 等待“档位生效”而不是仅等待数据刷新
            Boolean effective = WaitScaleEffectApplied(chnlId, oldData, oldVpp, step, 8000, 100);
            if (!effective)
            {
                DsoModel.Default.ArtificialIntelligence.AppenAiTipInfo("等待幅度档生效超时，已停止继续自动调挡");
            }
            return effective;
        }

        private Boolean WaitScaleEffectApplied(ChannelId chnlId, UInt16[] oldData, Int32 oldVpp, Int32 step, Int32 timeoutMs, Int32 pollMs)
        {
            Stopwatch sw = Stopwatch.StartNew();
            Int32 stableHit = 0;
            Int32 deltaThreshold = Math.Max(16, Math.Abs(oldVpp) / 20); // 至少变化约5%才认为档位生效
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                var pkg = DsoModel.Default.ArtificialIntelligence.GetScreenData(chnlId);
                if (pkg?.data == null || pkg.data.Length == 0 || ReferenceEquals(pkg.data, oldData))
                {
                    Thread.Sleep(pollMs);
                    continue;
                }

                Int32 newVpp = pkg.data.Max() - pkg.data.Min();
                Boolean directionMatched = step < 0
                    ? newVpp >= oldVpp + deltaThreshold
                    : newVpp <= oldVpp - deltaThreshold;

                if (directionMatched)
                {
                    stableHit++;
                    if (stableHit >= 2)
                        return true;
                }
                else
                {
                    stableHit = 0;
                }

                Thread.Sleep(pollMs);
            }
            return false;
        }


        //"64QAM", "SFM", "LFM", "Pulse", "QPSK", "AM", "32QAM", "BPSK", "8PSK", "16QAM", "Tri", "128QAM", "Sine", "Square"
        public BaseSignal? buildSignal(String signalType)
        {
            switch (signalType)
            {
                case "Square":
                case "Sine":
                case "Tri":
                    return new SimpleSignal() { type = signalType };
                case "BPSK":
                case "QPSK":
                case "8PSK":
                case "16QAM":
                case "32QAM":
                case "64QAM":
                case "128QAM":
                    return new CommunicationSignal() { type = signalType };
                case "SFM":
                case "AM":
                case "LFM":
                case "Pulse":
                    return new RadarSignal() { type = signalType }; ;
            }
            return null;
        }

    }
}

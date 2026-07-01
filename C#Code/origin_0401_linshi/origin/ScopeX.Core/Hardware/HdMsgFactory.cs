using System;
using System.Collections.Generic;
using System.Linq;
using NPOI.SS.Formula.Functions;
using ScopeX.ComModel;
using ScopeX.Core.Decode;
using ScopeX.Core.Tools;
using ScopeX.Hardware.Driver;
using static ScopeX.ComModel.HdMessage;

namespace ScopeX.Core.Hardware
{
    internal static class HdMsgFactory
    {
        internal static Boolean TryMake(UInt64 cmd, out HdMessage? msg)
        {
            //Int32 channelactivebits = 0;
            //foreach (AnalogModel ach in DsoModel.Default.AnalogChnls)
            //{
            //    if (ach.Active)
            //        channelactivebits |= (1 << (Int32)ach.Id);
            //}
            //if (Hd.ProductConfig.TryTakeAdcInterleaveMode(channelactivebits, out var newadcinterleavemode))
            //{
            //    DsoModel.Default.InterleaveMode = newadcinterleavemode;

            //    if (Hd.ProductConfig.TryGetStorageDeepList(newadcinterleavemode, out var result))
            //    {
            //        DsoModel.Default.Timebase.UpdateAnaChnLenghtSource(result.ToList());
            //    }
            //    if (Hd.ProductConfig.TryTakeMaxSegmentCount(newadcinterleavemode, out var newdict))
            //    {
            //        DsoModel.Default.Timebase.UpdateMaxSegmentCnt(newdict);
            //    }
            //}
            msg = null;
            if (cmd == 0)
            {
                return false;
            }

            msg = new HdMessage()
            {
                Command = cmd,

                ComboBits = WidgetPrsnt.Combo,

                FocusId = DsoPrsnt.FocusId,

                bAcquireStopped = TriggerModel.State == SysState.Stop,

                bSquareWaveSwitch = DsoPrsnt.DefaultDsoPrsnt.SquareWaveSwitch,

                Display = MakeDisplayOpt(),

                Analog = MakeAnalogOpt(),

                Timebase = MakeTimebaseOpt(),

                Trigger = MakeTriggerOpt(),

                Digital = MakeDigitalOpt(),

                Decoder = MakeDecoderOpt(),

                ArbWfmGen = MakeArbWfmGenOpt(),

                Cymometer = MakeCymometerOpt(),

                Search = MakeSearchOpt(),

                RadioFrequency = MakeRadioFrequencyOpt(),

                System = MakeSystemOptions(),

                AiTable = MakeAiTableOpt(),

                MultiDomain = MakeMultiDomainOpt(),

                Precision = MakePrecisionOpt(),
            };

            return true;
        }
        private static DisplayOptions MakeDisplayOpt()
        {
            return new()
            {
                DrawMode = DsoModel.Default.Display.DrawMode,

                Persist = DsoModel.Default.Display.Persist,

                IsFast = DsoModel.Default.Timebase.StorageMode == AnaChnlStorageMode.Fast ? (Dispatcher.IsScan ? (false | TriggerModel.State == SysState.Stop) : true) : false,

                AnalogZIndex = DsoModel.Default.Display.AnalogZIndex,
            };
        }

        private static PrecisionOptions MakePrecisionOpt()
        {
            ArtificialIntelligenceModel aimodel = DsoModel.Default.ArtificialIntelligence;
            PrecisionOptions ans = new PrecisionOptions()
            {
                AutoCfgBitWidthEnable = aimodel.AutoCfgAnaChnlBitWidthEnable,
                AnaChnlBitWidth = aimodel.AnaChnlBitWidth,
            };
            return ans;
        }

        private static AppConfig _AppConfig = AppConfig.GetIntance();
        private static AnalogOptions[] MakeAnalogOpt()
        {
            var aopt = new AnalogOptions[ChannelIdExt.AnaChnlNum];

            foreach (AnalogModel ach in DsoModel.Default.AnalogChnls)
            {
                //Double probegain = ach.Conditioning.ProbeIndex switch
                //{
                //	AnaChnlProbe.x1 => 1,
                //	AnaChnlProbe.x10 => 10,
                //	AnaChnlProbe.x100 => 100,
                //	_ => 1,
                //};

                // 最终需要移动的点数
                var resultpoint = ach.Id switch
                {
                    ChannelId.C1 => _AppConfig?.ChannelDelay_C1 ?? 0,
                    ChannelId.C2 => _AppConfig?.ChannelDelay_C2 ?? 0,
                    ChannelId.C3 => _AppConfig?.ChannelDelay_C3 ?? 0,
                    ChannelId.C4 => _AppConfig?.ChannelDelay_C4 ?? 0,
                    _ => 0
                };

                #region 通道延迟使用界面时的代码，目前使用配置文件直接指定每个通道的延迟点数了

                // 一级延时，默认延时100ns,在此基础上左右移动。
                var siglepointtime = Tools.Quantity.ConvertByPrefix(1d / ach.Sampling.AnalogSamplingRate, Tools.Prefix.Empty, Tools.Prefix.Nano); // 单点时间，单位已转为ns
                var basepoints = 100d / siglepointtime; // 整体默认向左移动100ns
                var usermovepoints = Tools.Quantity.ConvertByPrefix(ach.Conditioning.Delay, Tools.Prefix.Empty, Tools.Prefix.Nano); // 用户希望移动的时间,已转为ns

                // 最终需要移动的点数
                var uipoint = basepoints + (-usermovepoints / siglepointtime);

                /*// 由硬件影响，当为2.5G模式时，1、2通道需要多移动一个点。具体可以询问硬件同事
                if (ach.Sampling.AnalogSamplingRate == 2.5 * 1E9 && (ach.Id == ChannelId.C1 || ach.Id == ChannelId.C2))
                {
                    uipoint += 1d;
                }*/

                resultpoint = PlatformManager.Default.Platform.GetChannelDelayPoint(uipoint, ach.Conditioning.Delay);
                #endregion

                var chlgain = (ach.Conditioning.ProbeGain == 0 ? 1 : ach.Conditioning.ProbeGain);
                chlgain *= ach.Conditioning.ProbeUnitRatio;//由于设置下去的是探头增益，没有设置转换倍率，所以返回的ProbeGain是不包含单位倍率的，这里需要乘上
                bool active = ach.Id == ChannelId.C1 ? ach.Active : false;
                //aopt[(Int32)ach.Id] = new(ach.Active, (Int32)ach.Conditioning.ScaleIndex, ach.Conditioning.PosIndex)
                aopt[(Int32)ach.Id] = new(active, (Int32)ach.Conditioning.ScaleIndex, ach.Conditioning.PosIndex)
                {
                    Bandwidth = ach.Conditioning.Bandwidth,
                    IsInverted = ach.Conditioning.IsInverted,
                    Coupling = ach.Conditioning.Coupling,
                    ProbeIndex = ach.Conditioning.ProbeIndex,
                    ProbeGain = ach.Conditioning.ProbeGain,
                    ProbeUnitRatio = ach.Conditioning.ProbeUnitRatio,
                    ProbeGainCaliRatio = ach.Conditioning.ProbeGainCaliRatio,

                    Scale = ach.Conditioning.Scale / chlgain,
                    ScaleBymV = ach.Conditioning.ScaleBymV / chlgain,
                    Position = ach.Conditioning.Position / chlgain,
                    Bias = ach.Conditioning.BiasByuV / chlgain,
                    ProbeOffsetCaliBias = ach.Conditioning.ProbeOffsetCaliBias / chlgain,

                    FirstStageDelay = (Int32)resultpoint,
                    InputSource = (AnaChnlIpnutSource)(ach.Conditioning.FlagInfo ?? 0),
                    InterChannelOffset = ach.Deskew,
                };
            }
            return aopt;
        }

        private static TimebaseOptions MakeTimebaseOpt()
        {
            return new(DsoModel.Default.Timebase.PosIndex, DsoModel.Default.Timebase.Position, (Int32)DsoModel.Default.Timebase.ScaleIndex, DsoModel.Default.Timebase.Scale)
            {
                AcqMode = DsoModel.Default.Timebase.Mode,
                AcqLength = DsoModel.Default.Timebase.StorageMode,
                IsScan = Dispatcher.IsScan,

                StorageWaveDotsCnt = DsoModel.Default.Timebase.StorageWaveDotsCnt,
                InterleaveMode = DsoModel.Default.Timebase.InterleaveMode,

                FrameCount = DsoModel.Default.Timebase.SegmentActive ? DsoModel.Default.Timebase.FrameCount : 1,
                CurFrameId = DsoModel.Default.Timebase.SegmentActive ? DsoModel.Default.Timebase.CurFrameId : 1,
                ReferFrameId = DsoModel.Default.Timebase.SegmentActive ? (UInt32)DsoModel.Default.Timebase.ReferFrameIds : 1U,
                SegmentActive = DsoModel.Default.Timebase.SegmentActive ? 1U : 0,
                SegmentWorkMode = DsoModel.Default.Timebase.WorkMode,
                CallBack = DsoModel.Default.Timebase.SegmentActive ? DsoModel.Default.Timebase.CallBack : false,

                ClockSrc = DsoModel.Default.Timebase.ClockSrc,
                BlankTime = DsoModel.Default.Timebase.BlankTime,
                NeedWaveDotsCnt = DsoModel.Default.Timebase.TakeViewWaveDotsCnt,
                InterpolateType = DsoModel.Default.Timebase.InterplType,

                ZoomCenterX = DsoModel.Default.Timebase.ZoomCenterX,
                ZoomCenterY = DsoModel.Default.Timebase.ZoomCenterY,
                ZoomScaleX = DsoModel.Default.Timebase.ZoomScaleX,
                ZoomScaleY = DsoModel.Default.Timebase.ZoomScaleY,

                EnhancedBitsActive = DsoModel.Default.Timebase.EnhancedBitsActive,
                EnhancedBits = DsoModel.Default.Timebase.EnhancedBits
            };
        }

        private static TriggerOptions MakeTriggerOpt()
        {
            var tem = (TriggerEdgeModel)DsoModel.Default.GetTriggerModel(TriggerType.Edge);
            var edge = new TrigEdgeOptions(tem.Source, tem.Slope, tem.CompPosIndex)
            {
                Position = tem.CompPosition,
                Coupling = tem.Coupling,
                Impedance = tem.Impedance,
                SensitivityBymdiv = tem.SensitivityBymdiv,
            };

            var tpm = (TriggerWidthModel)DsoModel.Default.GetTriggerModel(TriggerType.PulseWidth);
            var pulse = new TrigPulseOptions(tpm.Source, tpm.Polarity, tpm.Condition, tpm.WidthByps, tpm.UpperWidthByps, tpm.CompPosIndex)
            {
                Position = tpm.CompPosition
            };

            var tgm = (TriggerGlitchModel)DsoModel.Default.GetTriggerModel(TriggerType.Glitch);
            var glitch = new TrigPulseOptions(tgm.Source, tgm.Polarity, tgm.Condition, tgm.WidthByps, tgm.UpperWidthByps, tgm.CompPosIndex)
            {
                Position = tgm.CompPosition
            };

            var tim = (TriggerIntervalModel)DsoModel.Default.GetTriggerModel(TriggerType.Interval);
            var interval = new TrigPulseOptions(tim.Source, tim.Polarity, tim.Condition, tim.WidthByps, tim.UpperWidthByps, tim.CompPosIndex)
            {
                Position = tim.CompPosition
            };

            var trm = (TriggerRuntModel)DsoModel.Default.GetTriggerModel(TriggerType.Runt);
            var runt = new TrigRuntOptions(trm.Source, trm.Polarity, trm.Condition, trm.WidthByps, trm.UpperWidthByps, trm.PosUpperIndex, trm.PosLowerIndex)
            {
                UpperPosition = trm.UpperCompPosition,
                LowerPosition = trm.LowerCompPosition,
            };

            var ttm = (TriggerTransModel)DsoModel.Default.GetTriggerModel(TriggerType.Transition);
            var trans = new TrigTransOptions(ttm.Source, ttm.Slope, ttm.Condition, ttm.WidthByps, ttm.UpperWidthByps, ttm.PosUpperIndex, ttm.PosLowerIndex)
            {
                UpperPosition = ttm.UpperCompPosition,
                LowerPosition = ttm.LowerCompPosition,
            };

            var twm = (TriggerWindowModel)DsoModel.Default.GetTriggerModel(TriggerType.Window);
            var window = new TrigWindowOptions(twm.Source, twm.PosCondition, twm.TimeCondition, twm.WidthByps, twm.PosUpperIndex, twm.PosLowerIndex)
            {
                UpperPosition = twm.UpperCompPosition,
                LowerPosition = twm.LowerCompPosition,
            };

            var dm = (TriggerDelayModel)DsoModel.Default.GetTriggerModel(TriggerType.Delay);
            var delay = new TrigDelayOptions(dm.SourceOne, dm.SourceOneSlope, dm.SourceTwo, dm.SourceTwoSlope, dm.Condition, dm.WidthByps, dm.UpperWidthByps, dm.PosUpperIndex, dm.DataCompPosIndex)
            {
                UpperPosition = dm.UpperCompPosition,
                LowerPosition = dm.DataCompPosition,
            };

            var ttom = (TriggerTimeOutModel)DsoModel.Default.GetTriggerModel(TriggerType.TimeOut);
            var timeout = new TrigTimeOutOptions(ttom.Source, ttom.Polarity, ttom.DurationByps, ttom.CompPosIndex)
            {
                Position = ttom.CompPosition,
            };

            var tstm = (TriggerSustainTimeModel)DsoModel.Default.GetTriggerModel(TriggerType.SustainTime);
            var sustaintimepos = new (SustainTimeLevelCondition Condition, Double Index, Double Value)[tstm.Length];

            var sourecs = PlatformManager.Default.Platform.GetTriggerSource();

            Int32 st = 0;
            foreach (var id in sourecs)
            {
                sustaintimepos[st++] = (tstm.Bits.GetCondition(id), tstm.Bits.GetPosIndex(id), tstm.Bits.GetPosition(id));
            }
            var sustaintime = new TrigSustainTimeOptions(tstm.Condition, tstm.WidthByps, tstm.UpperWidthByps, tstm.Length)
            {
                Positions = sustaintimepos,
            };
            var nem = (TriggerNEdgeModel)DsoModel.Default.GetTriggerModel(TriggerType.NEdge);
            var nedge = new TrigNEdgeOptions(nem.Source, nem.Polarity, nem.DurationByps, nem.EdgeNumber, nem.CompPosIndex)
            {
                Position = nem.CompPosition,
            };
            var tvm = (TriggerVideoModel)DsoModel.Default.GetTriggerModel(TriggerType.Video);
            var video = new TrigVideoOptions(tvm.Source, tvm.Standard, tvm.Polarity, tvm.Sync, tvm.Field, tvm.Line, tvm.CompPosIndex)
            {
                Position = tvm.CompPosition,
            };

            var tpatm = (TriggerPatternModel)DsoModel.Default.GetTriggerModel(TriggerType.Pattern);
            var patpos = new (PatLevelCondition Condition, Double Index, Double Value)[tpatm.Length];
            Int32 p = 0;
            foreach (var id in sourecs)
            {
                patpos[p++] = (tpatm.Bits.GetCondition(id), tpatm.Bits.GetPosIndex(id), tpatm.Bits.GetPosition(id));
            }
            var pattern = new TrigPatOptions(tpatm.Operator, tpatm.TimeCondition, tpatm.DurationByps, tpatm.UpperDurationByps, tpatm.Length)
            {
                Positions = patpos,
            };

            var tsm = (TriggerStateModel)DsoModel.Default.GetTriggerModel(TriggerType.State);
            var spos = new (PatLevelCondition Condition, Double Index, Double Value)[tsm.Length];
            Int32 s = 0;
            foreach (var id in sourecs)
            {
                spos[s++] = (tsm.Bits.GetCondition(id), tsm.Bits.GetPosIndex(id), tsm.Bits.GetPosition(id));
            }
            var state = new TrigStateOptions(tsm.Operator, tsm.TimeCondition, tsm.DurationByps, tsm.Length)
            {
                Positions = spos,
                ClkSource = tsm.ClkSource,
                ClkPolarity = tsm.ClkPolarity,
                Conformed = tsm.Conformed,
            };

            var tshm = (TriggerSetupHoldModel)DsoModel.Default.GetTriggerModel(TriggerType.SetupHold);
            var setuphold = new TrigSetupHoldOptions(tshm.ClkSource, tshm.ClkPolarity, tshm.DataSource, tshm.DataPosPolarity, tshm.Violation, tshm.TsuByps, tshm.ThdByps)
            {
                ClkPosition = (tshm.ClkCompPosIndex, tshm.ClkCompPosition),
                DataUpperPosition = (tshm.UpperDataPosIndex, tshm.UpperDataPosition),
                DataLowerPosition = (tshm.LowerDataPosIndex, tshm.LowerDataPosition),
            };

            var tmqm = (TriggerMultiQualifiedModel)DsoModel.Default.GetTriggerModel(TriggerType.MultiQulified);
            var multiqualified = new TrigMultiQualifiedOptions()
            {
                EventOptions = new (String Name, ITriggerTypeOptions? TriggerOption, DelayOpt DelayType, Int32 Counts, Int64 DurationByps)?[4] {
                   tmqm.Count>0? (tmqm[0].Node.Name,GetOptionsByName(tmqm[0].Node.Name,tmqm[0].Node),tmqm[0].Pathway.DelayType,tmqm[0].Pathway.EventCounts,tmqm[0].Pathway.DurationByps):null,
                   tmqm.Count>1? (tmqm[1].Node.Name,GetOptionsByName(tmqm[1].Node.Name,tmqm[1].Node),tmqm[1].Pathway.DelayType,tmqm[1].Pathway.EventCounts,tmqm[1].Pathway.DurationByps):null,
                   tmqm.Count>2? (tmqm[2].Node.Name,GetOptionsByName(tmqm[2].Node.Name,tmqm[2].Node),tmqm[2].Pathway.DelayType,tmqm[2].Pathway.EventCounts,tmqm[2].Pathway.DurationByps):null,
                   tmqm.Count>3? (tmqm[3].Node.Name,GetOptionsByName(tmqm[3].Node.Name,tmqm[3].Node),tmqm[3].Pathway.DelayType,tmqm[3].Pathway.EventCounts,tmqm[3].Pathway.DurationByps):null
                }
            };

            TriggerSerialModel trigserialmodel = (TriggerSerialModel)DsoModel.Default.GetTriggerModel(TriggerType.Serial);
            ProtocolModel protocolmodel = DecodeTools.GetChannelDecodeModel(TriggerSerialShareParameter.Default.Source, TriggerSerialShareParameter.Default.ProtocolType);

            TrigDecoderOptions trigserial = new(TriggerSerialShareParameter.Default.Source, TriggerSerialShareParameter.Default.ProtocolType)
            {
                ProtocolOptions = protocolmodel?.GetProtocolRecoder(),
                DecoderConditionsOptions = trigserialmodel?.GetTrigDecoderRecoder(),
            };

            ITriggerTypeOptions? GetOptionsByName(String name, TriggerModel triggerModel)
            {
                switch (name)
                {
                    case "Edge":
                        var tem = (TriggerEdgeModel)triggerModel;
                        return new TrigEdgeOptions(tem.Source, tem.Slope, tem.CompPosIndex)
                        {
                            Position = tem.CompPosition,
                            Coupling = tem.Coupling,
                            Impedance = tem.Impedance,
                            SensitivityBymdiv = tem.SensitivityBymdiv,
                        };
                    case "PulseWidth":
                        var tpm = (TriggerWidthModel)triggerModel;
                        return new TrigPulseOptions(tpm.Source, tpm.Polarity, tpm.Condition, tpm.WidthByps, tpm.UpperWidthByps, tpm.CompPosIndex)
                        {
                            Position = tpm.CompPosition
                        };
                    case "Glitch":
                        var tgm = (TriggerGlitchModel)triggerModel;
                        return new TrigPulseOptions(tgm.Source, tgm.Polarity, tgm.Condition, tgm.WidthByps, tgm.UpperWidthByps, tgm.CompPosIndex)
                        {
                            Position = tgm.CompPosition
                        };
                    case "Transition":
                        var ttm = (TriggerTransModel)triggerModel;
                        return new TrigTransOptions(ttm.Source, ttm.Slope, ttm.Condition, ttm.WidthByps, ttm.UpperWidthByps, ttm.PosUpperIndex, ttm.PosLowerIndex)
                        {
                            UpperPosition = ttm.UpperCompPosition,
                            LowerPosition = ttm.LowerCompPosition,
                        };
                    case "Runt":
                        var trm = (TriggerRuntModel)triggerModel;
                        return new TrigRuntOptions(trm.Source, trm.Polarity, trm.Condition, trm.WidthByps, trm.UpperWidthByps, trm.PosUpperIndex, trm.PosLowerIndex)
                        {
                            UpperPosition = trm.UpperCompPosition,
                            LowerPosition = trm.LowerCompPosition,
                        };
                    case "Window":
                        var twm = (TriggerWindowModel)triggerModel;
                        return new TrigWindowOptions(twm.Source, twm.PosCondition, twm.TimeCondition, twm.WidthByps, twm.PosUpperIndex, twm.PosLowerIndex)
                        {
                            UpperPosition = twm.UpperCompPosition,
                            LowerPosition = twm.LowerCompPosition,
                        };
                    case "TimeOut":
                        var ttom = (TriggerTimeOutModel)triggerModel;
                        return new TrigTimeOutOptions(ttom.Source, ttom.Polarity, ttom.DurationByps, ttom.CompPosIndex)
                        {
                            Position = ttom.CompPosition,
                        };
                    default:
                        break;
                }
                return null;
            }

            List<TriggerAreasOptions>? areasTrigger = null;
            if (DsoPrsnt.DefaultDsoPrsnt != null && DsoPrsnt.DefaultDsoPrsnt.VisualTrigger != null && DsoPrsnt.DefaultDsoPrsnt.VisualTrigger.Length > 0 && DsoPrsnt.DefaultDsoPrsnt.Timebase != null)
            {
                areasTrigger = new List<TriggerAreasOptions>();
                TriggerAreasOptions? tempitem = null;

                // 将虚拟坐标映射为量化值
                Single GetRealVal(Single posindex) => (posindex - (-4000)) / (Single)(4000 - (-4000)) * (248 - 8) + 8;
                foreach (var item in DsoPrsnt.DefaultDsoPrsnt.VisualTrigger.SelectedItems)
                {
                    try
                    {
                        if (item.RectanglePoints.LeftUp.IsEmpty || item.RectanglePoints.RightUp.IsEmpty || item.RectanglePoints.RightDown.IsEmpty || item.RectanglePoints.LeftDown.IsEmpty)
                            continue;
                        /*if (item.Polygons == null || item.Polygons.Count <= 0 || !item.Enabled)
                            continue;*/
                        var xarrary = new List<Single>()
                        {
                            item.RectanglePoints.LeftUp.X,
                            item.RectanglePoints.RightUp.X,
                            item.RectanglePoints.RightDown.X,
                            item.RectanglePoints.LeftDown.X,
                        };
                        var yarrary = new List<Single>()
                        {
                            item.RectanglePoints.LeftUp.Y,
                            item.RectanglePoints.RightUp.Y,
                            item.RectanglePoints.RightDown.Y,
                            item.RectanglePoints.LeftDown.Y,
                        };

                        var minx = xarrary.Min(c => c);
                        var maxy = yarrary.Max(c => c);
                        var maxx = xarrary.Max(c => c);
                        var miny = yarrary.Min(c => c);

                        tempitem = new TriggerAreasOptions(item.Source)
                        {
                            MinY = GetRealVal(miny),
                            MaxY = GetRealVal(maxy),
                            MinX = minx,
                            MaxX = maxx,
                            Enabled = item.Enabled && DsoPrsnt.DefaultDsoPrsnt.VisualTrigger.Enabled,
                            Reset = item.ReSet,
                            TriggerShape = item.TriggerShape,
                            TriggerState = item.TriggerState,
                        };

                        areasTrigger.Add(tempitem);
                    }
                    catch (Exception ex)
                    {
                        // 需要吃掉异常，以免Dispatcher崩溃退出。
                        EventBus.EventBroker.Instance.GetEvent<EventBus.LogEventArgs>().Publish(null, new EventBus.LogEventArgs(ex, EventBus.LogLevel.Error));
                        continue;
                    }
                }
            }

            return new TriggerOptions(TriggerModel.Type, TriggerModel.Mode)
            {
                HoldoffType = TriggerModel.HoldoffType,
                HoldoffByps = TriggerModel.HoldoffByps,
                HoldoffByCnt = TriggerModel.HoldoffByCnt,
                EnableExtAtten = TriggerModel.EnableExtAtten,
                AreasTrigger = areasTrigger,
                Edge = edge,
                Pulse = pulse,
                Glitch = glitch,
                Interval = interval,
                Window = window,
                Runt = runt,
                Transition = trans,
                Delay = delay,
                TimeOut = timeout,
                Video = video,
                SustainTime = sustaintime,
                Pattern = pattern,
                State = state,
                NEdge = nedge,
                SetupHold = setuphold,
                TrigMultiQualified = multiqualified,
                TrigDecoder = trigserial,
                TriggerStatus = TriggerPrsnt.State
            };
        }

        private static DigitalOptions[] MakeDigitalOpt()
        {
            var dch = (DigitalModel)DsoModel.Default.GetChannel(ChannelId.D0);

            var dopt = new DigitalOptions[dch.Conditioning.Groups.Count];

            for (Int32 i = 0; i < dch.Conditioning.Groups.Count; i++)
            {
                dopt[i] = new DigitalOptions(
                    dch.Active,
                    dch.Conditioning.Groups[i].UserThroldIndex,
                    dch.Conditioning.Groups[i].UserThroldBymV,
                    dch.Conditioning.Groups[i].UserHystIndex,
                    dch.Conditioning.Groups[i].UserHystBymV);
            }

            return dopt;
        }

        private static ArbWfmGenOptions[] MakeArbWfmGenOpt()
        {
            var aopt = new ArbWfmGenOptions[ChannelIdExt.AwgNum];
            var settings = DsoModel.Default.Setting;

            for (Int32 i = 0; i < ChannelIdExt.AwgNum; i++)
            {
                var awg = DsoModel.Default.GetWfmGenerator(ChannelId.AWG1 + i);
                aopt[i] = new(awg.Active, awg.WfmType, awg.Impedance, awg.Frequency, awg.Amplitude, awg.Offset, awg.RealDuty, awg.Phase, awg.WfmGenTriger, awg.RampType)
                {
                    GenChannelId = awg.Id,
                    EnablePointByPoint = awg.EnablePointByPoint,

                    Mode = awg.Mode,
                    Noise = awg.Noise,
                    FilePath = awg.FilePath,

                    ModFreq = awg.ModFreq,
                    AmpDepth = awg.AmpDepth,
                    FreqBias = awg.FreqBias,
                    PhaseBias = awg.PhaseBias,
                    ModMethod = awg.ModMethod,
                    ModWfmType = awg.ModulatedWfm,
                    CustomImpedance = awg.CustomImpedance,

                    SweepFreqType = awg.SweepType,
                    //SweepFreqActive = awg.SweepFreqActive,
                    SweepFreqTime = awg.SweepDuration,
                    SweepFreqEndFreq = awg.SweepEndFreq,
                    SweepFreqStartFreq = awg.SweepStartFreq,
                    ArbWfmData = awg.ArbWfmData,
                    ModArbWfmData = awg.ModArbWfmData,
                    AuxIn = settings.AuxInputSignal == AuxInputType.Sync_AWG,
                    AuxOut = awg.TirgerOutEnabel,
                    AuxInPolarity = settings.AuxInPolarity == EdgeSlope.Rise ? AwgTrigPolarity.Rise : AwgTrigPolarity.Fall,
                    AuxOutPolarity = settings.AuxOutPolarity == EdgeSlope.Rise ? AwgTrigPolarity.Rise : AwgTrigPolarity.Fall,
                    PulseFallTime = awg.PulseFallTime,
                    PulseRiseTime = awg.PulseRiseTime,
                    RampType = awg.RampType,
                    // <Remark>作者：彭博 创建日期：2023/12/1 11:28:00 创建原因：添加只选择波形时才发生波形的功能 </Remark>
                    IsSendWaveType = awg.IsSendWaveType
                };
                // <Remark>作者：彭博 创建日期：2023/12/1 11:28:00 创建原因：添加只选择波形时才发生波形的功能 </Remark>
                awg.IsSendWaveType = false;
            }

            return aopt;
        }

        private static CymometerOptions MakeCymometerOpt()
        {
            return new(DsoModel.Default.Cymometer.Source);
        }

        private static DecoderOptions[] MakeDecoderOpt()
        {
            var bopt = DsoModel.Default.DecodeChnls.Cast<DecodeModel>()
                .OrderBy(bch => bch.Id)
                .Select(bch => new DecoderOptions(bch.Active, bch.ProtocolType)
                {
                    ProtocolOptions = bch.GetChDecodeModel()?.GetProtocolRecoder(),
                }).ToArray();
            return bopt;
        }



        private static RadioFrequencyOptions[] MakeRadioFrequencyOpt()
        {
            var rfopt = new RadioFrequencyOptions[ChannelIdExt.RFChnlNum];
            foreach (RadioFrequencyModel rfch in DsoModel.Default.RadioFrequencyChnls)
            {
                rfopt[rfch.Id - ChannelIdExt.MinRFChId] = new(rfch.Active)
                {
                    Source = rfch.Source,

                    CenterFrequency = rfch.Sampling.CenterFrequency,
                    Span = rfch.Sampling.Span,
                    FFTLength = rfch.Sampling.FFTLength,
                    RBW = rfch.Sampling.RBW,
                    Window = rfch.Window,
                    STFTLength = rfch.STFTLength,
                    STFTStep = rfch.STFTStep,

                    TVFON = rfch.TimeVSFrequency.Active,
                    AVTON = rfch.AmpVSTime.Active,
                    PVTON = rfch.PhaseVSTime.Active,
                    PVFON = rfch.PhaseVSFrequency.Active,
                    ReferenceLevel = rfch.Conditioning.RefLevelValue,
                };
            }
            return rfopt;
        }

        private static SearchOptions MakeSearchOpt()
        {
            var search = DsoModel.Default.Search;
            Dictionary<Int64, (SearchType Type, ISearchTypeOptions Option)> searchs = new Dictionary<Int64, (SearchType Type, ISearchTypeOptions Option)>();
            lock (SearchModel.Locker)
            {
                foreach (var item in search.Items)
                {
                    searchs.Add(item.Key, (item.Value.Type, item.Value.SearchTypeModel.GetOption()));
                }
            }

            return new SearchOptions(search.Enabled) { Searchs = searchs };
        }

        private static SystemOptions MakeSystemOptions()
        {
            var tempctrl = DsoModel.Default.TempCtrl;
            Dictionary<String, Int32> fansspeed = new();
            foreach (String fanname in tempctrl.FansName)
            {
                fansspeed[fanname] = tempctrl.GetFanSpeed(fanname);
            }
            return new SystemOptions(tempctrl.AutoCtrlFans, fansspeed);
        }
        private static MultiDomainRecord MakeMultiDomainOpt()
        {
            var mdmodel = DsoModel.Default.MultiDomain;

            MultiDomainRecord md = new MultiDomainRecord()
            {
                Active = mdmodel.Active,
                Source = mdmodel.Source,
                WindowType = mdmodel.WindowType,
                FFTLength = mdmodel.FFTLength,
                STFTLength = mdmodel.STFTLength,
                STFTStep = mdmodel.STFTStep,
                CenterFreqByHz = mdmodel.CenterFreqByHz,
                SpanByHz = mdmodel.SpanFreqByHz,
                SpanForTimeFreq = mdmodel.SpanValueForTimeFreq,
                TimeScaleForTimeFreq = mdmodel.TimeScaleForTimeFreq,
                RoughSpecCnt = mdmodel.RoughSpecCnt,
                AVTON = mdmodel.GetFigureMathModel(MultiDomainFigureEnum.AmpVsTime)?.Enabled ?? false,
                FVTON = mdmodel.GetFigureMathModel(MultiDomainFigureEnum.FreqVsTime)?.Enabled ?? false,
                PVTON = mdmodel.GetFigureMathModel(MultiDomainFigureEnum.PhaseVsTime)?.Enabled ?? false,
                SpecON = (mdmodel.GetFigureMathModel(MultiDomainFigureEnum.Spectrogram)?.Enabled ?? false) || (mdmodel.ThreeDimensionalEnable),
                SynchronizationEnable = mdmodel.SynchronizationEnable,
                ParameterTuningEnable = mdmodel.ParameterTuningEnable,
                ZoomStart = mdmodel.ZoomStart,
                ZoomLength = mdmodel.ZoomLength,
                TimeStep = mdmodel.TimeStep,
            };

            return md;
        }

        private static Dictionary<ChannelId, AiOptions> MakeAiTableOpt()
        {
            ArtificialIntelligenceModel aimodel = DsoModel.Default.ArtificialIntelligence;
            ExceptionCaptureModel exceptionmodel = DsoModel.Default.ExceptionCapture;
            Dictionary<ChannelId, AiOptions> ans = new();
            foreach (ChannelId chnlid in ChannelIdExt.GetAnalogs())
            {
                var exceptionenable = aimodel.GetCaptureExceptionEnable(chnlid);

                var triggerenable = aimodel.GetTemplateTriggerEnable(chnlid);
                var triggersourcetype = aimodel.GetTemplateSource(chnlid);
                var templateoffset = aimodel.GetTemplateOffset(chnlid);
                var startpos = aimodel.GetUserDefinePosStart(chnlid);

                ans[chnlid] = new AiOptions()
                {
                    RecfgDbi = new RecfgDbiRecord(aimodel.GetRecfgDbiEnable(chnlid))
                    {
                        SubbandCtrlMethod = aimodel.GetSubbandCtrlMethod(chnlid),
                        CriticalFreq = aimodel.GetCriticalFreq(chnlid),
                        AutoFilterMode = aimodel.GetAutoFilterMode(chnlid),
                        SubbandEnable = aimodel.SubbandsEnable,
                        IterFilterEnable = aimodel.GetIterFilterEnable(chnlid),
                        BaseNoise = aimodel.GetSubbandBaseNoise(chnlid),
                        LocalFreqByHz = aimodel.GetSubbandLocalFreq(chnlid),
                        BandFreqLimitByHz = aimodel.GetBandFreqLimit(chnlid),
                        AntImageFreqByHz = aimodel.GetAntImageFreq(chnlid),
                    },

                    CaptureException = new CaptureExceptionRecord(exceptionmodel.Active)
                    {
                        SourceType = exceptionmodel.GetTemplateTriggerSourceEnum(chnlid),
                        FrameLength = exceptionmodel.GetCaptureExceptionFrameLength(chnlid),
                        SendTemplateCnt = exceptionmodel.GetTemplateBuildCnt(chnlid),
                        Export2FileCnt = exceptionmodel.GetExport2FileCnt(chnlid),
                        ImfData = null,
                        ResData = null,
                    },

                    TemplateTrigger = new TemplateTriggerRecord(triggerenable, triggersourcetype, templateoffset, startpos)
                    {
                        SendTemplateCnt = aimodel.GetTemplateTriggerSendCnt(chnlid),
                        FrameIdForTrig = aimodel.GetFrameIdForTrig(chnlid),
                        FrameTrigDataLen = aimodel.GetFrameTrigDataLen(chnlid),
                    },

                    AIUnion = new AIUnionRecord()
                    {
                        RecfgDbiUnion = aimodel.GetReconfigDbiUnionEnable(chnlid),
                        CaptureExceptionUnion = aimodel.GetCaptureExceptionUnionEnable(chnlid),
                        AINoiseReductionEnable = aimodel.GetAINoiseReductionEnable(chnlid),
                        AverageEnable = aimodel.AverageEnable,
                        CurNoiseRedutionMethod = aimodel.CurNoiseRedutionMethod,
                    }
                };
            }
            return ans;
        }
    }

}

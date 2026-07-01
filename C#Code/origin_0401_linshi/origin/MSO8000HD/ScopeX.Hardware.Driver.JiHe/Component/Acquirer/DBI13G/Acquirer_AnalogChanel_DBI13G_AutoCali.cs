using MathWorks.MATLAB.NET.Arrays;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.MathExt;
using static ScopeX.Hardware.Driver.ProductDataTranslate_MSO8000X;
using static ScopeX.Hardware.Calibration.Data.Base.ProductDataTranslate_MSO8000X;
using TiadcParamsKeyMap = ScopeX.Hardware.Driver.ProductDataTranslate_MSO8000X.TiadcParamsKeyMap;

namespace ScopeX.Hardware.Driver
{
    public partial class Acquirer_AnalogChanel_DBI13G
    {
        internal override bool AutoCaliAtInit(HdMessage? hdMessage)
        {
            if (Hd.UIMessage == null)
                return false;

            HdMessage backHdMessage = Hd.UIMessage with { };
            List<ChannelId> tiadccalichnllist = new();
            List<ChannelId> calicutptchnllist = new();
            List<ChannelId> localcoechnllist = new();
            if (SpecialConfigForAutoCaliAtInit == String.Empty)
            {
                tiadccalichnllist.Add(ChannelId.C1);
                tiadccalichnllist.Add(ChannelId.C3);
                calicutptchnllist.AddRange(ChannelIdExt.GetAnalogs());
                localcoechnllist.AddRange(ChannelIdExt.GetAnalogs());
            }
            else
            {
                if (SpecialConfigForAutoCaliAtInit.StartsWith("Tiadc_"))
                {
                    tiadccalichnllist.Add(Enum.Parse<ChannelId>(SpecialConfigForAutoCaliAtInit.Substring("Tiadc_".Length)));
                }
                if (SpecialConfigForAutoCaliAtInit.StartsWith("Cutpts_"))
                {
                    calicutptchnllist.Add(Enum.Parse<ChannelId>(SpecialConfigForAutoCaliAtInit.Substring("Cutpts_".Length)));
                }
                if (SpecialConfigForAutoCaliAtInit.StartsWith("Local_"))
                {
                    localcoechnllist.Add(Enum.Parse<ChannelId>(SpecialConfigForAutoCaliAtInit.Substring("Local_".Length)));
                }
            }
            var before = DateTime.Now;
            //WeakTip.Default.Write("AutoCaliAtInit", $"启用内部信号源，开启自校准", emergent: false, "", 10);
            SwitchSource(true);
            foreach (ChannelId chnlId in ChannelIdExt.GetAnalogs())
            {
                if (tiadccalichnllist.Contains(chnlId))
                {
                    var beforeTiadc = DateTime.Now;
                    //WeakTip.Default.Write("AutoCaliAtInit", $"自动校准{chnlId}的TIADC误差", emergent: false, "", 30);
                    CaliTiadc(chnlId);
                    var afterTiadc = DateTime.Now;
                    Trace.WriteLine($"[AutoCaliAtInit]Tiadc calibration {chnlId} cost {(afterTiadc - beforeTiadc).TotalMilliseconds}ms");
                }
                if (calicutptchnllist.Contains(chnlId))
                {
                    //WeakTip.Default.Write("AutoCaliAtInit", $"自动校准{chnlId}的丢点", emergent: false, "", 5);
                    CaliCutpts(false, chnlId);
                    CaliLocalCoe(false, chnlId);
                    CaliCutpts(true, chnlId);
                }
                if (localcoechnllist.Contains(chnlId))
                {
                    //WeakTip.Default.Write("AutoCaliAtInit", $"自动校准{chnlId}的本振初相", emergent: false, "", 5);
                    CaliLocalCoe(true, chnlId);
                }
            }

            //WeakTip.Default.Write("AutoCaliAtInit", $"关闭内部信号源，正在恢复正常采集", emergent: false, "", 5);
            CtrlAnalogChannel_DBI20G.ConfigInnerSignalSource(false, 0,CtrlAnalogChannel_DBI20G.SrcTypeEnum.SingleFreq, ChannelId.C1);
            
            SwitchSource(false);
            if (!SpecialConfigForAutoCaliAtInit.StartsWith("Cutpts_"))
                InitDebugVarints();
            Helper.GetICaliData(CaliDataType.TiAdc_PhaseOffsetGain)?.SaveToFile();
            Helper.GetICaliData(CaliDataType.DbiAnalogParams)?.SaveToFile();
            Hd.LocalCommands |= (long)HdCmd.TmbScaleIndex;
            Hd.LocalCommands |= (long)HdCmd.ChnlGain;
            Hd.Execute(backHdMessage);
            var after = DateTime.Now;
            Trace.WriteLine($"[AutoCaliAtInit]sum cost {(after - before).TotalMilliseconds}ms");
            SpecialConfigForAutoCaliAtInit = String.Empty;
            return true;
        }

        private void SwitchSource(Boolean isInnerSource)
        {
            Int32 FullBandWidth_Is0 = 0;
            Int32 logicSubbandIndex = 0;
            for (Int32 phyChannelID = 0; phyChannelID < ChannelIdExt.AnaChnlNum; phyChannelID++)
            {
                for (AnaChnlScaleIndex scaleIndex = AnaChnlScaleIndex.Lv10m; scaleIndex <= AnaChnlScaleIndex.Lv500m; scaleIndex++)
                {
                    
                    DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)scaleIndex, 0] = new DbiAnalogChannelSubbandItem()
                    {
                        IntDiscardDots = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)scaleIndex, logicSubbandIndex].IntDiscardDots,
                        AnalogChannelGain = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)scaleIndex, logicSubbandIndex].AnalogChannelGain,
                        SubbandGain = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)scaleIndex, logicSubbandIndex].SubbandGain,
                        BiasPreceding = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)scaleIndex, logicSubbandIndex].BiasPreceding,
                        BiasPreceding_3Div = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)scaleIndex, logicSubbandIndex].BiasPreceding_3Div,
                        OffsetPosterior = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)scaleIndex, logicSubbandIndex].OffsetPosterior,
                        OffsetPosterior_3Div = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)scaleIndex, logicSubbandIndex].OffsetPosterior_3Div,
                        Gain_FineByAdc1ByTenThousand = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)scaleIndex, logicSubbandIndex].Gain_FineByAdc1ByTenThousand,
                        Gain_FineByAdc2ByTenThousand = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)scaleIndex, logicSubbandIndex].Gain_FineByAdc2ByTenThousand,
                        Gain_FineByFpgaThousand = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)scaleIndex, logicSubbandIndex].Gain_FineByFpgaThousand,
                        DCTrigZero = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)scaleIndex, logicSubbandIndex].DCTrigZero,
                        DCTrigZero_3Div = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)scaleIndex, logicSubbandIndex].DCTrigZero_3Div,
                        Reserved1 = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)scaleIndex, logicSubbandIndex].Reserved1,
                        Reserved2 = isInnerSource ? 1u : 0,
                        DiscardDotsAfter = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (Int32)AnaChnlScaleIndex.Lv1, logicSubbandIndex].DiscardDotsAfter,
                        DiscardDotsBefore = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (Int32)AnaChnlScaleIndex.Lv1, logicSubbandIndex].DiscardDotsBefore,
                    };
                }

                for (Int32 subid = 0; subid < 3; subid++)
                {
                    DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)AnaChnlScaleIndex.Lv1, subid] = new DbiAnalogChannelSubbandItem()
                    {
                        IntDiscardDots = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)AnaChnlScaleIndex.Lv500m, subid].IntDiscardDots,
                        AnalogChannelGain = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)AnaChnlScaleIndex.Lv500m, subid].AnalogChannelGain,
                        SubbandGain = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)AnaChnlScaleIndex.Lv500m, subid].SubbandGain,
                        BiasPreceding = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)AnaChnlScaleIndex.Lv500m, subid].BiasPreceding,
                        BiasPreceding_3Div = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)AnaChnlScaleIndex.Lv500m, subid].BiasPreceding_3Div,
                        OffsetPosterior = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)AnaChnlScaleIndex.Lv500m, subid].OffsetPosterior,
                        OffsetPosterior_3Div = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)AnaChnlScaleIndex.Lv500m, subid].OffsetPosterior_3Div,
                        Gain_FineByAdc1ByTenThousand = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)AnaChnlScaleIndex.Lv500m, subid].Gain_FineByAdc1ByTenThousand,
                        Gain_FineByAdc2ByTenThousand = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)AnaChnlScaleIndex.Lv500m, subid].Gain_FineByAdc2ByTenThousand,
                        Gain_FineByFpgaThousand = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)AnaChnlScaleIndex.Lv500m, subid].Gain_FineByFpgaThousand,
                        DCTrigZero = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)AnaChnlScaleIndex.Lv500m, subid].DCTrigZero,
                        DCTrigZero_3Div = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)AnaChnlScaleIndex.Lv500m, subid].DCTrigZero_3Div,
                        Reserved1 = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (int)AnaChnlScaleIndex.Lv500m, subid].Reserved1,
                        Reserved2 = isInnerSource ? 1u : 0,
                        DiscardDotsAfter = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (Int32)AnaChnlScaleIndex.Lv1, subid].DiscardDotsAfter,
                        DiscardDotsBefore = DbiAnalogParams.Default[FullBandWidth_Is0, phyChannelID, (Int32)AnaChnlScaleIndex.Lv1, subid].DiscardDotsBefore,
                    };
                }                
            }
            AbstractController_AnalogChannel.CtrlGain();
            if (Hd.UIMessage == null || Hd.UIMessage.Timebase == null || Hd.UIMessage.Analog == null)
                return;
            HdMessage message = Hd.UIMessage with
            {
                Timebase = Hd.UIMessage.Timebase with
                {
                    TmbScale = 0.002,
                    TmbScaleIndex = (Int32)AnaChnlTimebaseIndex.Lv2n,
                },
                Analog = new HdMessage.AnalogOptions[]
                {
                    Hd.UIMessage.Analog[0] with{ ScaleIndex = (Int32)AnaChnlScaleIndex.Lv1 },
                    Hd.UIMessage.Analog[1] with{ ScaleIndex = (Int32)AnaChnlScaleIndex.Lv1 },
                    Hd.UIMessage.Analog[2] with{ ScaleIndex = (Int32)AnaChnlScaleIndex.Lv1 },
                    Hd.UIMessage.Analog[3] with{ ScaleIndex = (Int32)AnaChnlScaleIndex.Lv1 },
                }

            };
            Hd.LocalCommands |= (long)HdCmd.TmbScaleIndex;
            Hd.LocalCommands |= (long)HdCmd.ChnlGain;
            Hd.Execute(message);
            Thread.Sleep(5000);
        }

        private void InitDebugVarints()
        {
            Hd.CurrDebugVarints.bEnable_AmplitudeTemperatureCompensate = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime = false;
            Hd.CurrDebugVarints.bEnable_AtStartScanAdcRxWindows = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanWindow = false;

            Hd.CurrDebugVarints.bEnable_AdcDataDebugMode = false;
            Hd.CurrDebugVarints.bEnable_DigitTrigger = true;
            Hd.CurrDebugVarints.bEnable_AcqbdInterpolation = true;
            Hd.CurrDebugVarints.bEnable_AcqBd_Afc = false;
            Hd.CurrDebugVarints.bEnable_AcqBd_Pfc = false;

            Hd.CurrDebugVarints.bEnable_CorrectTiAdc = true;

            Hd.CurrDebugVarints.bEnable_Dbi_IntDelay = true;
            Hd.CurrDebugVarints.bEnable_Dbi_AmpFreqCoef = true;
            Hd.CurrDebugVarints.bEnable_Dbi_AntiImageCoef = true;
            Hd.CurrDebugVarints.bEnable_Dbi_FractionaryDelayCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_LocalOscillatorCoef = true;
            Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation = true;
            Hd.CurrDebugVarints.bEnable_Dbi_OverlapPhaseFreqDelayCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_PhaseFreqCoef = true;
            Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode = true;

            Hd.CurrDebugVarints.bEnable_ProcBd_Average = true;
        }

        #region TIADC
        /// <summary>
        /// 1、控制内部校准源，输出指定频率的信号
        /// 2、获取对应通道和对应子带的采样数据
        /// 3、计算增益和相位偏差
        /// 4、根据偏差，得出ADC需要调整的相位和增益控制字
        /// 
        /// </summary>
        /// <param name="chnlId">通道ID</param>
        /// <param name="subbandId">子带编号</param>
        /// <param name="signalFreqByMhz">校准用的信号频率</param>
        /// <param name="subbandFreqByMhz">子带实际采集到的信号频率</param>
        private void CaliDbiTiadc(ChannelId chnlId, Int32 subbandId, UInt32 signalFreqByMhz, Double subbandFreqByMhz)
        {
            Int32 maxCaliTimeByms = 60_000;
            Int32 paramSendTimesByms = 2000;

            Double sampFreqByMHz = 20000;
            Int32 minPhaseCtrlWord = 0;
            Int32 maxPhaseCtrlWord = 0xffff;
            Int32 minGainCtrlWord = 0x2000;
            Int32 maxGainCtrlWord = 0xffff;

            Double theoryGainStep = 400.0 / 0.02;
            Double theoryPhaseStep = 1000.0 / 200;

            Double phaseLimitByfs = 800;
            Double gainLimit = 0.05;

            var adcusedinfos = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(Acquirer_AnalogChanel_DBI13G.DbiActuallActiveState, chnlId, subbandId);
            if (adcusedinfos == null)
                return;
            Int32 curAcqBdNo = (Int32)adcusedinfos.AcqBdNo;

            CtrlAnalogChannel_DBI20G.ConfigInnerSignalSource(true, signalFreqByMhz, CtrlAnalogChannel_DBI20G.SrcTypeEnum.SingleFreq, chnlId);
            Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
            AbstractController_AnalogChannel.CtrlOffset();

            Thread.Sleep(paramSendTimesByms);

            Int32 baseAdcId = 0;
            List<Int32> adcIdList = new() { baseAdcId, 1};

            Dictionary<Int32, Boolean> gainOK = new();
            Dictionary<Int32, Boolean> phaseOK = new();

            Dictionary<Int32, Int32> phaseBaseCtrlWord = new();
            Dictionary<Int32, Int32> phaseDeltaCtrlWord = new();

            Dictionary<Int32, Int32> maxPhaseDelta = new();
            Dictionary<Int32, Int32> minPhaseDelta = new();
       
            foreach (Int32 adcId in adcIdList)
            {
                TiadcParamsKeyMap itemKey = new("All-20G", chnlId, (uint)adcId);
                TiadcPhaseOffsetGainItem_Base tmpItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(itemKey)!.Value;


                //TiadcPhaseOffsetGainItem_Base tmpItem = TiAdc_PhaseOffsetGain.Default[curAcqBdNo, adcId, 0];// 小心数组越界
                if (tmpItem.Gain < minGainCtrlWord)
                    tmpItem.Gain = minGainCtrlWord;
                //TiAdc_PhaseOffsetGain.Default[curAcqBdNo, adcId, 0] = tmpItem;
                ProductDataTranslate_MSO8000X.SetTiadcParamsItem(itemKey, tmpItem);
                phaseBaseCtrlWord[adcId] = tmpItem.Phase;
                phaseDeltaCtrlWord[adcId] = 0;
            }

            foreach (Int32 adcId in adcIdList)
            {
                maxPhaseDelta[adcId] = (maxPhaseCtrlWord - phaseBaseCtrlWord[adcId]) + (phaseBaseCtrlWord[baseAdcId] - minPhaseCtrlWord);
                minPhaseDelta[adcId] = (minPhaseCtrlWord - phaseBaseCtrlWord[adcId]) + (phaseBaseCtrlWord[baseAdcId] - maxPhaseCtrlWord);
            }

            CtrlWordAndPhase? positivephase = null;
            CtrlWordAndPhase? negativephase = null;

            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();

            while (sw.ElapsedMilliseconds < maxCaliTimeByms)
            {
                Dictionary<Int32, PhaseGainError> curTiadcError = GetAdcAverageError(subbandId, adcIdList, sampFreqByMHz, subbandFreqByMhz);

                foreach (Int32 adcId in curTiadcError.Keys)
                {
                    TiadcParamsKeyMap itemKey = new("All-20G", chnlId, (uint)adcId);
                    TiadcPhaseOffsetGainItem_Base tmpItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(itemKey)!.Value;
                    TiadcParamsKeyMap baseitemKey = new("All-20G", chnlId, (uint)baseAdcId);
                    TiadcPhaseOffsetGainItem_Base baseItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(baseitemKey)!.Value;

                    if ((!phaseOK.ContainsKey(adcId)) || (!phaseOK[adcId]))
                    {
                        if (Math.Abs(curTiadcError[adcId].phaseByfs) < phaseLimitByfs)
                        {
                            phaseOK[adcId] = true;
                        }
                        else
                        {
                            if (curTiadcError[adcId].phaseByfs < 0)
                            {
                                if (negativephase == null || negativephase.PhaseError < curTiadcError[adcId].phaseByfs)
                                    negativephase = new CtrlWordAndPhase(phaseDeltaCtrlWord[adcId], curTiadcError[adcId].phaseByfs);
                            }
                            else
                            {
                                if (positivephase == null || positivephase.PhaseError > curTiadcError[adcId].phaseByfs)
                                    positivephase = new CtrlWordAndPhase(phaseDeltaCtrlWord[adcId], curTiadcError[adcId].phaseByfs);
                            }
                            CtrlWordAndPhase curphase = new CtrlWordAndPhase(phaseDeltaCtrlWord[adcId], curTiadcError[adcId].phaseByfs);
                            phaseDeltaCtrlWord[adcId] = GetPhaseDeltaCtrlWords(negativephase, positivephase, theoryPhaseStep, curphase);
                            Trace.WriteLine($"[CaliDbiTiadc]curPhaseError:{curTiadcError[adcId].phaseByfs.ToString("0.000")}fs\tnext delta phaseCtrlWord:{phaseDeltaCtrlWord[adcId]}");
                            if (phaseDeltaCtrlWord[adcId] < minPhaseDelta[adcId] || phaseDeltaCtrlWord[adcId] > maxPhaseDelta[adcId])
                            {
                                Trace.WriteLine($"[CaliDbiTiadc]phaseDeltaCtrlWord[{adcId}]:{phaseDeltaCtrlWord[adcId]} over range!");
                                continue;
                            }
                                

                            Int32 ctrlwordTmp = phaseBaseCtrlWord[adcId] + phaseDeltaCtrlWord[adcId];
                            
                            if (ctrlwordTmp <= maxPhaseCtrlWord && ctrlwordTmp >= 0)
                            {
                                tmpItem.Phase = ctrlwordTmp;
                                baseItem.Phase = phaseBaseCtrlWord[baseAdcId];
                            }
                            else if (ctrlwordTmp < 0)
                            {
                                tmpItem.Phase = minPhaseCtrlWord;
                                baseItem.Phase = phaseBaseCtrlWord[baseAdcId] - (phaseDeltaCtrlWord[adcId] + phaseBaseCtrlWord[adcId] - minPhaseCtrlWord);
                            }
                            else
                            {
                                tmpItem.Phase = maxPhaseCtrlWord;
                                baseItem.Phase = phaseBaseCtrlWord[baseAdcId] - (maxPhaseCtrlWord - phaseDeltaCtrlWord[adcId]);
                            }
                        }
                    }

                    if ((!gainOK.ContainsKey(adcId)) || (!gainOK[adcId]))
                    {
                        if (Math.Abs(curTiadcError[adcId].gain) < gainLimit)
                        {
                            gainOK[adcId] = true;
                        }
                        else
                        {
                            if (curTiadcError[adcId].gain < 0)
                            {
                                baseItem.Gain -= (Int32)(curTiadcError[adcId].gain * theoryGainStep);
                                if (baseItem.Gain < minGainCtrlWord || baseItem.Gain > maxGainCtrlWord)
                                    continue;
                            }
                            else
                            {
                                tmpItem.Gain += (Int32)(curTiadcError[adcId].gain * theoryGainStep);
                                if (tmpItem.Gain < minGainCtrlWord || tmpItem.Gain > maxGainCtrlWord)
                                    continue;
                            }
                        }
                    }

                    //TiAdc_PhaseOffsetGain.Default[curAcqBdNo, adcId, 0] = tmpItem;
                    //TiAdc_PhaseOffsetGain.Default[curAcqBdNo, baseAdcId, 0] = baseItem;

                    ProductDataTranslate_MSO8000X.SetTiadcParamsItem(itemKey, tmpItem);
                    ProductDataTranslate_MSO8000X.SetTiadcParamsItem(baseitemKey, tmpItem);
                }

                Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
                AbstractController_AnalogChannel.CtrlOffset();

                Thread.Sleep(paramSendTimesByms);

                Boolean allOk = true;
                foreach (Int32 adcId in curTiadcError.Keys)
                {
                    if ((!phaseOK.ContainsKey(adcId)) || (!phaseOK[adcId]))
                    {
                        allOk = false;
                    }

                    if ((!gainOK.ContainsKey(adcId)) || (!gainOK[adcId]))
                    {
                        allOk = false;
                    }
                }

                if (allOk)
                {
                    Trace.WriteLine($"[CaliDbiTiadc]****************************************Tiadc OK({chnlId} s{subbandId} freq:{signalFreqByMhz}MHz)************************");
                    //WeakTip.Default.Write("AutoCaliAtInit", $"{chnlId}子带{subbandId + 1}在内部源{signalFreqByMhz}MHz下TIADC误差校准成功", emergent: false, "", 5);
                    return;
                }
                    
            }

            Trace.WriteLine($"!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!Tiadc over time ({chnlId} s{subbandId} freq:{signalFreqByMhz}MHz)!!!!!!!!!!!!!!!!!!!!");
            //WeakTip.Default.Write("AutoCaliAtInit", $"{chnlId}子带{subbandId + 1}在内部源{signalFreqByMhz}MHz下TIADC误差校准失败", emergent: false, "", 5);
        }

        record CtrlWordAndPhase(Int32 CtrlWord, Double PhaseError);

        private Int32 GetPhaseDeltaCtrlWords(CtrlWordAndPhase? negativePhase, CtrlWordAndPhase? positivePhase, Double theotryStep, CtrlWordAndPhase curPhase)
        {
            if (negativePhase != null && positivePhase != null)
            {
                Double phaseerror = positivePhase.PhaseError - negativePhase.PhaseError;
                Double ratio = (positivePhase.CtrlWord - negativePhase.CtrlWord) / phaseerror;
                return negativePhase.CtrlWord + (Int32)(-negativePhase.PhaseError * ratio);
            }

            if (Math.Abs(curPhase.PhaseError) < 150000)
                return (Int32)(curPhase.CtrlWord - curPhase.PhaseError * 2 / theotryStep);

            return (Int32)(curPhase.CtrlWord - curPhase.PhaseError / 2 / theotryStep);
        }

        record PhaseGainError(Double phaseByfs, Double gain);

        /// <summary>
        /// 获取多个TIADC的增益和相位误差，获取采样数据前，系统应当工作在子带模式
        /// </summary>
        /// <param name="subbandId">需要计算的子带编号</param>
        /// <param name="adcIdList">需要计算的ADC编号列表</param>
        /// <param name="sampFreqByMHz">TIADC系统的采样率，MHz为单位</param>
        /// <param name="subbandFreqByMhz">经过混频后，子带实际采样的信号频率，MHz为单位</param>
        /// <param name="baseAdcId">误差以哪个ADC为基准进行计算</param>
        /// <param name="adcCnt">TIADC中的ADC个数</param>
        /// <param name="staticTimes">统计次数</param>
        /// <returns>每个ADC的增益和相位误差</returns>
        private Dictionary<Int32, PhaseGainError> GetAdcAverageError(Int32 subbandId, List<Int32> adcIdList, Double sampFreqByMHz, Double subbandFreqByMhz, Int32 baseAdcId = 0, Int32 adcCnt = 2, Int32 staticTimes = 5)
        {
            Dictionary<Int32, PhaseGainError> ans = new();

            Dictionary<Int32, List<Double>> gainError = new();
            Dictionary<Int32, List<Double>> phaseError = new();

            if (!adcIdList.Contains(baseAdcId))
                adcIdList.Add(baseAdcId);
            foreach (Int32 adcId in adcIdList)
            {
                gainError[adcId] = new();
                phaseError[adcId] = new();
            }

            for (Int32 i = 0; i < staticTimes; i++)
            {
                for (Int32 j = 0; j < 20; j++)
                {
                    if (AbstractController_Misc.AcqIsFulled())
                        break;
                    Thread.Sleep(1);
                }
                Dictionary<Int32, Double[]> adcData = GetSubbandAdcData(subbandId, adcIdList);
                if (adcData.Count == 0)
                    continue;

                Boolean ampisok = true;
                foreach (Int32 adcId in adcData.Keys)
                {
                    if (adcData[adcId].Length == 0)
                    {
                        ampisok = false;
                        Trace.WriteLine($"[GetAdcAverageError]adcId({adcId}).Length = 0");
                        continue;
                    }
                    var maxvalue = adcData[adcId].Max();
                    var minvalue = adcData[adcId].Min();
                    if (maxvalue - minvalue < 160 || maxvalue - minvalue > 4000 || maxvalue == 4095 || minvalue == 0)
                    {
                        Trace.WriteLine($"[GetAdcAverageError]adcId({adcId}).amp is not ok(minvalue:{minvalue},maxvalue{maxvalue}).");
                        ampisok = false;
                    }
                }
                if (!ampisok)
                {
                    continue;
                }
                Dictionary<Int32, SinFitResult> waveOffsetGainPhaseAdc = new();
                foreach (Int32 adcId in adcData.Keys)
                {
                    waveOffsetGainPhaseAdc[adcId] = SinFitClass.SinFit(adcData[adcId], sampFreqByMHz / adcCnt, subbandFreqByMhz) ?? new SinFitResult(0, 0, 0);
                }

                if (!waveOffsetGainPhaseAdc.Keys.Contains(baseAdcId))
                    break;

                if (i == 0)
                    continue;

                foreach (Int32 adcId in waveOffsetGainPhaseAdc.Keys)
                {
                    if (adcId == baseAdcId)
                        continue;
                    Double gainTmp = (waveOffsetGainPhaseAdc[adcId].Gain - waveOffsetGainPhaseAdc[baseAdcId].Gain) / waveOffsetGainPhaseAdc[baseAdcId].Gain;
                    Double phaseTmp = (waveOffsetGainPhaseAdc[adcId].Phase + Math.PI * 2 - waveOffsetGainPhaseAdc[baseAdcId].Phase) % (Math.PI * 2);

                    gainError[adcId].Add(gainTmp);
                    phaseError[adcId].Add(phaseTmp);
                }
            }

            foreach (Int32 adcId in gainError.Keys)
            {
                if (gainError[adcId].Count > 0)
                    Trace.WriteLine($"[GetAdcAverageError]subbandId({subbandId}) adcId({adcId}) gainError({String.Join(",", gainError[adcId].Select(o => o.ToString("0.000")))})");
            }
            foreach (Int32 adcId in phaseError.Keys)
            {
                if (phaseError[adcId].Count > 0)
                    Trace.WriteLine($"[GetAdcAverageError]subbandId({subbandId}) adcId({adcId}) phaseError({String.Join(",", phaseError[adcId].Select(o => o.ToString("0.000")))})");
            }

            Double theoryDeltaByfs = 1_000_000_000d / sampFreqByMHz;

            foreach (Int32 adcId in adcIdList)
            {
                if (gainError.ContainsKey(adcId) && phaseError.ContainsKey(adcId))
                {
                    if (gainError[adcId].Count == 0 || phaseError[adcId].Count == 0)
                        continue;
                    Double gainAvg = gainError[adcId].Average();

                    Double sinAverage = phaseError[adcId].Select(o => Math.Sin(o)).Average();
                    Double cosAverage = phaseError[adcId].Select(o => Math.Cos(o)).Average();
                    Double phaseByfs = Math.Atan2(sinAverage, cosAverage) * 1000_000_000 / subbandFreqByMhz / (2 * Math.PI) - theoryDeltaByfs;

                    if (phaseByfs > 1000_000_000 / subbandFreqByMhz / 2)
                        phaseByfs -= 1000_000_000 / subbandFreqByMhz;
                    else if (phaseByfs < -1000_000_000 / subbandFreqByMhz / 2)
                        phaseByfs += 1000_000_000 / subbandFreqByMhz;

                    ans[adcId] = new(phaseByfs, gainAvg);
                    Trace.WriteLine($"[GetAdcAverageError]subbandId({subbandId}) adcId({adcId}) phaseByfs({phaseByfs.ToString("0.000")}fs) gainAvg({gainAvg.ToString("0.000")})");
                }
            }
            return ans;
        }

        private Dictionary<Int32, Double[]> GetSubbandAdcData(Int32 subbandId, List<Int32> adcIdList, Int32 adcCnt = 2)
        {
            Int32 dataLen = 1024;
            Dictionary<Int32, Double[]> ans = new();
            var readinfo = new List<ReadInfo>
            {
                new ReadInfo(AcqDataType.AnalogChannel,
                             new List<ChannelId>() { ChannelId.C1, ChannelId.C2, ChannelId.C3, ChannelId.C4 },
                             new WfmPkgInfo(25000, 0.2, 0.1),
                             ""),
            };
            Dictionary<AcqDataType, double> samplingRate = new Dictionary<AcqDataType, double>();
            Boolean acqOk = Hd.AcqWave(false, false, readinfo, ref samplingRate);
            if ((!acqOk) || AcqedDataPool.AnalogChData.AllChannelData.Count <= subbandId)
                return ans;

            foreach (var adcId in adcIdList)
            {
                ans[adcId] = new Double[dataLen];
                for (Int32 i = 0; i < dataLen; i++)
                {
                    Int32 dotsId = i * adcCnt + adcId;
                    if (dotsId < AcqedDataPool.AnalogChData.AllChannelData[subbandId].Count)
                    {
                        ans[adcId][i] = AcqedDataPool.AnalogChData.AllChannelData[subbandId][dotsId];
                    }
                }
            }
            
            return ans;
        }

        /// <summary>
        /// 1、关闭所有协处理，包括采集板插值，处理板插值，数字出发，PFC，AFC，DBI拼合；
        /// 2、实时档，当前使用2ns/div
        /// 3、子带模式，依次上传各个通道的多个子带的数据
        /// </summary>
        private void CaliTiadc(ChannelId chnlId)
        {
            Hd.CurrDebugVarints.bEnable_AmplitudeTemperatureCompensate = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime = true;
            Hd.CurrDebugVarints.bEnable_AtStartScanAdcRxWindows = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanWindow = false;

            Hd.CurrDebugVarints.bEnable_DigitTrigger = false;
            Hd.CurrDebugVarints.bEnable_AcqbdInterpolation = false;
            Hd.CurrDebugVarints.bEnable_AcqBd_Afc = false;
            Hd.CurrDebugVarints.bEnable_AcqBd_Pfc = false;

            Hd.CurrDebugVarints.bEnable_CorrectTiAdc = false;

            Hd.CurrDebugVarints.bEnable_Dbi_IntDelay = false;
            Hd.CurrDebugVarints.bEnable_Dbi_AmpFreqCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_AntiImageCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_FractionaryDelayCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_LocalOscillatorCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation = false;
            Hd.CurrDebugVarints.bEnable_Dbi_OverlapPhaseFreqDelayCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_PhaseFreqCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode = false;

            Hd.CurrDebugVarints.bEnable_AdcDataDebugMode = true;

            // key:子带编号 value:校准源输出的信号频率 - 子带经过混频后实际采集的信号频率
            Dictionary<Int32, (UInt32 signalFreqByMHz, Double actualFreqByMHz)> caliItems = new()
            {
                [0] = (6000,  6000.0),
                [1] = (9500, 500.0),
                [2] = (14500, 500.0),
                [3] = (19500, 500.0),
            };

            Hd.CurrDebugVarints.iDbi_DebugChannelID = chnlId;
            Hd.LocalCommands |= (Int64)HdCmd.CaliDataChanged;
            if (Hd.UIMessage != null)
                Hd.Execute(Hd.UIMessage);

            foreach (Int32 subbandId in caliItems.Keys)
            {
                CaliDbiTiadc((ChannelId)Hd.CurrDebugVarints.iDbi_DebugChannelID, subbandId, caliItems[subbandId].signalFreqByMHz, caliItems[subbandId].actualFreqByMHz);
            }
        }
        #endregion

        #region DiscardDotsForSync        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="chnlId"></param>
        /// <param name="subbandId"></param>
        /// <param name="phaseDiffTable"></param>
        /// <param name="sampleFreqByGhz"></param>
        /// <param name="parallelRoad"></param>
        /// <param name="subbandCnt"></param>
        /// <returns></returns>
        private Double[] CalcCutPtsMatlabDll(Int32 chnlId, Int32 subbandId, Dictionary<Int32, Double> phaseDiffTable, Int32 sampleFreqByGhz, Int32 parallelRoad, Int32 subbandCnt)
        {
            Int32 curBandMode = 0;
            Int32 curYscale = (Int32)AnaChnlScaleIndex.Lv1;

            Int32[] originDiscardDots = new Int32[subbandCnt * 2];

            for (Int32 i = 0; i < subbandCnt; i++)
            {
                originDiscardDots[i] = DbiAnalogParams.Default[curBandMode, chnlId, curYscale, i].DiscardDotsBefore;
            }

            for (Int32 i = 0; i < subbandCnt; i++)
            {
                originDiscardDots[i + subbandCnt] = DbiAnalogParams.Default[curBandMode, chnlId, curYscale, i].DiscardDotsAfter;
            }

            MatlabDll_CutPts cutpts = new();
            return cutpts.CalcCutPts(subbandId, originDiscardDots, phaseDiffTable, sampleFreqByGhz, parallelRoad);
        }

        private void SendCutPts(Double[] cutpts, Int32 chnlId)
        {
            Int32 curBandMode = 0;
            Int32 subbandCnt = 3;
            if (cutpts.Length < subbandCnt * 2)
                return;

            for (Int32 yscaleId = 0; yscaleId < CaliConstants.Fixed_MaxPhyCoarseScaleCount; yscaleId++)
            {
                for (Int32 subbandId = 0; subbandId < subbandCnt; subbandId++)
                {
                    DbiAnalogChannelSubbandItem originItem = DbiAnalogParams.Default[curBandMode, chnlId, yscaleId, subbandId];
                    DbiAnalogChannelSubbandItem newItem = new DbiAnalogChannelSubbandItem()
                    {
                        AnalogChannelGain = originItem.AnalogChannelGain,
                        IntDiscardDots = originItem.IntDiscardDots,
                        SubbandGain = originItem.SubbandGain,
                        BiasPreceding = originItem.BiasPreceding,
                        BiasPreceding_3Div = originItem.BiasPreceding_3Div,
                        OffsetPosterior = originItem.OffsetPosterior,
                        OffsetPosterior_3Div = originItem.OffsetPosterior_3Div,
                        Gain_FineByAdc1ByTenThousand = originItem.Gain_FineByAdc1ByTenThousand,
                        Gain_FineByAdc2ByTenThousand = originItem.Gain_FineByAdc2ByTenThousand,
                        Gain_FineByFpgaThousand = originItem.Gain_FineByFpgaThousand,
                        Reserved1 = originItem.Reserved1,
                        Reserved2 = originItem.Reserved2,
                        DiscardDotsAfter = Convert.ToInt32(cutpts[subbandId + subbandCnt]),
                        DiscardDotsBefore = Convert.ToInt32(cutpts[subbandId]),
                    };
                    DbiAnalogParams.Default[curBandMode, chnlId, yscaleId, subbandId] = newItem;
                }
            }
            CtrlAnalogChannel_DBI20G.SendDiscard();
            Trace.WriteLine($"[SendCutPts]chnlId({chnlId}) ({String.Join(",", cutpts)})");
            //WeakTip.Default.Write("AutoCaliAtInit", $"C{chnlId + 1}发送的丢点数为：{String.Join(",", cutpts)}", emergent: false, "", 5);
            Thread.Sleep(1000);
        }
        private const String _PhaseDiffFilePath = "./CaliData/";
        private const String _PhaseDiffFileName = "PhaseDiff.txt";
        private Dictionary<Int32, Dictionary<Int32, Double>> LoadPhaseDiffDictionary(ChannelId chnlId)
        {
            Dictionary<Int32, Dictionary<Int32, Double>> ans = new();
            String filename = $"{_PhaseDiffFilePath}{chnlId}_{_PhaseDiffFileName}";
            if (File.Exists(filename))
            {
                StreamReader sr = new StreamReader(filename);
                while (!sr.EndOfStream)
                {
                    var tmp = sr.ReadLine();
                    if (tmp == null || !tmp.Contains(','))
                        continue;
                    var phasediffinfo = tmp.Split(',');
                    if (phasediffinfo == null || phasediffinfo.Length < 3)
                        continue;
                    Int32 subbandid = Int32.Parse(phasediffinfo[0]);
                    Int32 freq = Int32.Parse(phasediffinfo[1]);
                    Double phasediff = Double.Parse(phasediffinfo[2]);
                    if (!ans.ContainsKey(subbandid))
                        ans[subbandid] = new();
                    ans[subbandid][freq] = phasediff;
                }
                sr.Close();
            }

            return ans;
        }

        /// <summary>
        /// 1、开启本振系数，采集板的插值，关闭DBI拼合
        /// 2、子带模式
        /// </summary>
        private void CaliCutpts(Boolean isUseActualPhaseDiff, ChannelId chnlId)
        {
            Hd.CurrDebugVarints.bEnable_AmplitudeTemperatureCompensate = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime = false;
            Hd.CurrDebugVarints.bEnable_AtStartScanAdcRxWindows = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanWindow = false;

            Hd.CurrDebugVarints.bEnable_AdcDataDebugMode = true;
            Hd.CurrDebugVarints.bEnable_DigitTrigger = false;
            Hd.CurrDebugVarints.bEnable_AcqbdInterpolation = true;
            Hd.CurrDebugVarints.bEnable_AcqBd_Afc = false;
            Hd.CurrDebugVarints.bEnable_AcqBd_Pfc = false;

            Hd.CurrDebugVarints.bEnable_CorrectTiAdc = true;

            Hd.CurrDebugVarints.bEnable_Dbi_IntDelay = true;
            Hd.CurrDebugVarints.bEnable_Dbi_AmpFreqCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_AntiImageCoef = true;
            Hd.CurrDebugVarints.bEnable_Dbi_FractionaryDelayCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_LocalOscillatorCoef = true;
            Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation = false;
            Hd.CurrDebugVarints.bEnable_Dbi_OverlapPhaseFreqDelayCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_PhaseFreqCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode = false;

            Hd.CurrDebugVarints.iDbi_DebugChannelID = chnlId;
            Hd.LocalCommands |= (Int64)HdCmd.CaliDataChanged;

            if (Hd.UIMessage == null || Hd.UIMessage.Timebase == null) return;
            HdMessage backHdMessage = Hd.UIMessage with
            {
                Timebase = Hd.UIMessage.Timebase with
                {
                    TmbScale = 0.002,
                    TmbScaleIndex = (Int32)AnaChnlTimebaseIndex.Lv2n,
                }
            };
            Hd.Execute(backHdMessage);

            Int32 subbandCnt = 3;
            Int32 parallelRoad = 80;
            Int32 sampleFreqByMHz = 60000;

            if (!isUseActualPhaseDiff)
            {
                Double[] cutpts = new Double[subbandCnt * 2];
                Int32 yscaleId = (Int32)AnaChnlScaleIndex.Lv1;
                for (Int32 subid = 0; subid < subbandCnt; subid++)
                {
                    cutpts[subid] = 0;// DbiAnalogParams.Default[0, (Int32)chnlId, yscaleId, subid].DiscardDotsBefore;
                    cutpts[subid + subbandCnt] = 0;// DbiAnalogParams.Default[0, (Int32)chnlId, yscaleId, subid].DiscardDotsAfter;
                }
                SendCutPts(cutpts, (Int32)chnlId);
                return;
            }
            
            //        子带编号        信号频率  相位差
            Dictionary<Int32, Dictionary<Int32, Double>> lastphasediff = LoadPhaseDiffDictionary(chnlId);

            foreach (Int32 subbandId in lastphasediff.Keys)
            {
                //WeakTip.Default.Write("AutoCaliAtInit", $"正在扫描{chnlId}交叠带{subbandId}的相位偏差", emergent: false, "", 10);
                Dictionary<Int32, Double> phaseDiffTable = new();
                foreach (Int32 signalFreqByMhz in lastphasediff[subbandId].Keys)
                {
                    CtrlAnalogChannel_DBI20G.ConfigInnerSignalSource(true, (UInt32)signalFreqByMhz, CtrlAnalogChannel_DBI20G.SrcTypeEnum.FastEdge, chnlId);
                    Thread.Sleep(2000);
                    Hd.LocalCommands |= (Int64)HdCmd.CaliDataChanged;
                    Hd.Execute(backHdMessage);
                    (Boolean phasevalid, Double phase) = GetSubbandAveragePhaseDiff(subbandId, sampleFreqByMHz, signalFreqByMhz);
                    if (phasevalid)
                        phaseDiffTable[signalFreqByMhz] = phase - lastphasediff[subbandId][signalFreqByMhz];
                }

                if (phaseDiffTable.Count != 0)
                {
                    Double[] cutpts = CalcCutPtsMatlabDll((Int32)chnlId, subbandId, phaseDiffTable, sampleFreqByMHz / 1000, parallelRoad, subbandCnt);
                    SendCutPts(cutpts, (Int32)chnlId);
                }
            }

        }
        #endregion

        #region LocalCoe
        /// <summary>
        /// 1、开启本振系数，采集板的插值，关闭DBI拼合
        /// 2、子带模式
        /// </summary>
        /// <param name="isUseActualPhaseDiff">是否使用实际采样的数据计算相位偏差来计算本振系数</param>
        /// <param name="chnlId"></param>
        private void CaliLocalCoe(Boolean isUseActualPhaseDiff, ChannelId chnlId)
        {
            Hd.CurrDebugVarints.bEnable_AmplitudeTemperatureCompensate = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime = false;
            Hd.CurrDebugVarints.bEnable_AtStartScanAdcRxWindows = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanWindow = false;

            Hd.CurrDebugVarints.bEnable_AdcDataDebugMode = true;
            Hd.CurrDebugVarints.bEnable_DigitTrigger = false;
            Hd.CurrDebugVarints.bEnable_AcqbdInterpolation = true;
            Hd.CurrDebugVarints.bEnable_AcqBd_Afc = false;
            Hd.CurrDebugVarints.bEnable_AcqBd_Pfc = false;

            Hd.CurrDebugVarints.bEnable_CorrectTiAdc = true;

            Hd.CurrDebugVarints.bEnable_Dbi_IntDelay = true;
            Hd.CurrDebugVarints.bEnable_Dbi_AmpFreqCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_AntiImageCoef = true;
            Hd.CurrDebugVarints.bEnable_Dbi_FractionaryDelayCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_LocalOscillatorCoef = true;
            Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation = false;
            Hd.CurrDebugVarints.bEnable_Dbi_OverlapPhaseFreqDelayCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_PhaseFreqCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode = false;

            if (Hd.UIMessage == null || Hd.UIMessage.Timebase == null) return;
            HdMessage backHdMessage = Hd.UIMessage with
            {
                Timebase = Hd.UIMessage.Timebase with
                {
                    TmbScale = 0.002,
                    TmbScaleIndex = (Int32)AnaChnlTimebaseIndex.Lv2n,
                }
            };
            Hd.Execute(backHdMessage);

            Int32 coeLen = 120;
            Int32 sampleFreqByMHz = 60000;
            // key:子带编号 value:校准源输出的信号频率(MHz) - 子带经过混频后实际采集的信号频率(MHz) - 本振频率(MHz)
            Dictionary<Int32, (Int32 signalFreqByMHz, Int32 actualFreqByMHz, Int32 localFreqByMHz)> caliItems = new()
            {
                [1] = (6000, 6000, 11500),
                [2] = (11000, 11000, 10500),
            };

            foreach (Int32 subbandId in caliItems.Keys)
            {
                Boolean phasevalid = true;
                Double phaseDiff = 0;
                if (isUseActualPhaseDiff)
                {
                    CtrlAnalogChannel_DBI20G.ConfigInnerSignalSource(true, (UInt32)caliItems[subbandId].signalFreqByMHz, CtrlAnalogChannel_DBI20G.SrcTypeEnum.SingleFreq, chnlId);
                    Thread.Sleep(2000);
                    Hd.LocalCommands |= (Int64)HdCmd.CaliDataChanged;
                    Hd.Execute(backHdMessage);
                    (phasevalid, phaseDiff) = GetSubbandAveragePhaseDiff(subbandId, sampleFreqByMHz, caliItems[subbandId].signalFreqByMHz);
                }
                if (phasevalid)
                    CalcAndSendLocalCoe(chnlId, subbandId, phaseDiff, sampleFreqByMHz / 1000.0, caliItems[subbandId].localFreqByMHz / 1000.0, coeLen);
            }
        }

        private (Boolean, Double) GetSubbandAveragePhaseDiff(Int32 subbandId, Int32 sampleFreqByMHz, Int32 signalFreqByMHz, Int32 staticTimes = 5)
        {
            List<Double> phaseDiffList = new();

            if (subbandId < 1)
                return (false, 0.0);
            for (Int32 i = 0; i < staticTimes; i++)
            {
                for (Int32 j = 0; j < 30; j++)
                {
                    if (AbstractController_Misc.AcqIsFulled())
                        break;
                    Thread.Sleep(1);
                }
                Dictionary<Int32, Int16[]> ans = new();
                var readinfo = new List<ReadInfo>
                {
                    new ReadInfo(AcqDataType.AnalogChannel,
                                 new List<ChannelId>() { ChannelId.C1, ChannelId.C2, ChannelId.C3, ChannelId.C4 },
                                 new WfmPkgInfo(25000, 0.2, 0.1),
                                 ""),
                };
                Dictionary<AcqDataType, double> samplingRate = new Dictionary<AcqDataType, double>();
                Boolean acqOk = Hd.AcqWave(false, false, readinfo, ref samplingRate);

                if (AcqedDataPool.AnalogChData.AllChannelData.Count <= subbandId || acqOk == false || i == 0)
                    continue;

                var data0 = AcqedDataPool.AnalogChData.AllChannelData[subbandId - 1].ToArray();
                var data1 = AcqedDataPool.AnalogChData.AllChannelData[subbandId].ToArray();

                var data0pk = data0.Max() - data0.Min();
                var data1pk = data1.Max() - data1.Min();
                if (data0pk < 160 || data0pk > 4000 || data1pk < 160 || data1pk > 4000)
                {
                    Trace.WriteLine($"[GetSubbandAveragePhaseDiff]signalFreqByMHz({signalFreqByMHz}) subbandId({subbandId - 1}) peak({data0pk}) subbandId({subbandId}) peak({data1pk})");
                    continue;
                }

                var tmp0 = SinFitClass.SinFit(AcqedDataPool.AnalogChData.AllChannelData[subbandId - 1].Select(o => (Double)o).ToArray(), sampleFreqByMHz, signalFreqByMHz) ?? new SinFitResult(0, 0, 0);
                var tmp1 = SinFitClass.SinFit(AcqedDataPool.AnalogChData.AllChannelData[subbandId].Select(o => (Double)o).ToArray(), sampleFreqByMHz, signalFreqByMHz) ?? new SinFitResult(0, 0, 0);
                if (tmp0 != null && tmp1 != null)
                    phaseDiffList.Add(tmp0.Phase - tmp1.Phase);
            }

            if (phaseDiffList.Count == 0)
            {
                Trace.WriteLine($"[GetSubbandAveragePhaseDiff]signalFreqByMHz:{signalFreqByMHz} phaseDiffList error, return 0");
                return (false, 0.0);
            }

            Trace.WriteLine($"[GetSubbandAveragePhaseDiff]signalFreqByMHz:{signalFreqByMHz} phaseDiffList:{String.Join(",", phaseDiffList.Select(o => o.ToString("0.000")))}");
            Double phasesin = phaseDiffList.Select(o => Math.Sin(o)).Average();
            Double phasecos = phaseDiffList.Select(o => Math.Cos(o)).Average();
            return (true, Math.Atan2(phasesin, phasecos));
        }

        private void CalcAndSendLocalCoe(ChannelId chnlId, Int32 subbandId, Double phaseDiff, Double sampleFreqByGHz, Double localFreqByGHz, Int32 coeLength)
        {
            MatlabDll_LocalCoe matlabDll_LocalCoe = new();
            var ans = matlabDll_LocalCoe.CalcLocalCoe(phaseDiff, sampleFreqByGHz, localFreqByGHz, coeLength);

            if (ans.Length != 0)
                SaveAndSendCoe(ans, DbiCoefficientsTablesType.LocalOscillatorCoefficients, chnlId, subbandId);
        }
         
        private void SaveAndSendCoe(Double[] coeData, DbiCoefficientsTablesType coeType, ChannelId chnlId, Int32 subbandId)
        {
            var defineItem = GetAcqDefineItem(DbiCoefficientsTablesType.LocalOscillatorCoefficients, chnlId, subbandId);
            if (defineItem == null)
                return;

            String fileName = defineItem.DataFileName;

            StreamWriter sw = new StreamWriter(fileName);
            for (Int32 i = 0; i < coeData.Length; i++)
            {
                sw.WriteLine(coeData[i]);
            }
            sw.Flush();
            sw.Close();

            CoefficientsTableSender_DBI.Send2AcqBoardByDefineItem(coeType, new List<DBI_CoefTableSendItem>() { defineItem });
            Thread.Sleep(1000);
        }
        #endregion
    }
}

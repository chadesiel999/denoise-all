using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.MathExt;
using System.Diagnostics.CodeAnalysis;
namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 模拟通道波形采集
    /// </summary>
    public abstract partial class AbstractAcquirer_AnalogChannel : AbstractAcquirer
    {
        #region 触发深度等
        internal virtual void DiscardDotAtTriggerTypeIsSerialMode()
        {
            if ((!Hd.CurrDebugVarints.bEnable_DigitTrigger) || Hd.CurrDebugVarints.bEnable_AdcDataDebugMode)
                return;
            if (Hd.UIMessage!.Trigger!.TrigType != TriggerType.Serial)
                return;
            UInt32 trigSource = Hd.CurrProduct.Ctrl_Trigger!.CurrentTrigSource();
            if (trigSource >= ChannelIdExt.AnaChnlNum)
                return;
            ChannelBdAdcInputDefine channelBdAdcInputDefine = GetChannelAcqBdAdcInputCorresponding((int)trigSource) ?? new ChannelBdAdcInputDefine() { BdNo = AcqBdNo.B2 };
            UInt32 skipcount = (UInt32)(Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Decoder_TimeDelaycntlock, channelBdAdcInputDefine.BdNo));

            UInt32 skipcount_bound = (UInt32)(Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters!.ExtractNumFromAdc) - 1;

            if (skipcount < skipcount_bound)
            {
                skipcount = skipcount + (UInt32)(Hd.CurrProduct.Acquirer_AnalogChannel.AcquingParameters.ExtractNumFromAdc);
            }
            SerialProtocolType protocolType = Hd.UIMessage!.Trigger!.TrigDecoder!.ProtocolType;
            if ((Hd.UIMessage!.Trigger!.TrigDecoder.ProtocolOptions is HdMessage.ProtocolUSBOptions usb && usb.SignalRate == ProtocolUSB.SignalRate.HighRate) || protocolType == SerialProtocolType.PCIe || protocolType == SerialProtocolType.SATA || Hd.UIMessage!.Timebase!.TmbScaleIndex <= (int)AnaChnlTimebaseIndex.Lv100n)
                skipcount = 0;
            long mergeRoadCount = 40;
            if (Hd.CurrProduct.ProductType == ProductType.B21_DBI16G || Hd.CurrProduct.ProductType == ProductType.B21_DBI20G || Hd.CurrProduct.ProductType == ProductType.B21_MD8G || Hd.CurrProduct.ProductType == ProductType.B21_HB8G)
                mergeRoadCount = 80;
            skipcount = (UInt32)(mergeRoadCount * skipcount / ((long)(Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.ExtractNumFromAdc ?? 1U)));
            if (skipcount > 0 && (skipcount < 500)) //500????????
            {
                for (int displayChIndex = 0; displayChIndex < ChannelIdExt.AnaChnlNum; displayChIndex++)
                    AcqedDataPool.AnalogChData.AllChannelData[displayChIndex].RemoveRange(0, (int)skipcount);
            }
        }
        internal virtual UInt32 DefaultTrigSensitivity
        {
            get => (UInt32)((Hd.UIMessage?.Trigger?.Edge?.SensitivityBymdiv??500)*Constants.VIS_ADC_RES/Constants.VIS_YDIVS_NUM  / Constants.IDX_PER_YDIV);
        }
        internal virtual Int32 Trig_LS_TrigOffset
        {
            get
            {
                return 8 * 15;
            }
        }
        internal virtual Int32 Trig_LS_DiscardNumMax
        {
            get
            {
                return 4096;
            }
        }
        internal virtual ChannelBdAdcInputDefine? GetChannelAcqBdAdcInputCorresponding(int channelIndex) => null;
        internal const int Trig_XDepthFixedAddDots = 400;
        internal virtual TrigSourceParams GetTrigSourceParams(UInt32 trigChannelFromSoft)
        {
            return null;
        }
        private Boolean IsInterpolation()
        {
            return false;
        }
        internal virtual (Int64 depth, Int64 waveDepth) GetTrigXDepth()
        {
            UInt32 interpMul = 1U;
            double tmbScale = Hd.UIMessage?.Timebase?.TmbScale ?? 1.0;
            double tmbPosition = Hd.UIMessage?.Timebase?.TmbPosition ?? 0;
            double PerDataByfs_AtDdr = (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 50_000_000); //20G sample,50_000_000
            Int64 depth = (Int64)((tmbScale * Constants.VIS_XDIVS_NUM / 2 - tmbPosition) * 1_000_000_000 / PerDataByfs_AtDdr);
            Int64 waveDepth = (Int64)((tmbScale * Constants.VIS_XDIVS_NUM / 2 - tmbPosition) * 1_000_000_000 / (PerDataByfs_AtDdr / interpMul));
            
            if (!Hd.AnalogChannel?.IsNeedPostProcessByMatlab ?? false)
            {
                return (depth, waveDepth);
            }
            else
            {
                return (depth + Trig_XDepthFixedAddDots, waveDepth);
            }
        }
        //point 1
        /// <summary>
        /// 应该这样计算
        /// </summary>
        /// <param name="xdepth"></param>
        /// <param name="trigSource"></param>
        /// <returns></returns>
        internal virtual UInt32 GetAcqBdTrigDiscardColumnNums(Int64 xdepth, UInt32 trigSource)
        {
            if (xdepth < 0)
                return 0;
            double mergeRoadCount = 40d;
            UInt64 discard_tmp = (UInt64)(xdepth % mergeRoadCount); ;
            if (IsInterpolation())
            {
                AnaChnlTimebaseIndex TmbScaleIndex = (AnaChnlTimebaseIndex)(Hd.UIMessage?.Timebase?.TmbScaleIndex ?? (int)AnaChnlTimebaseIndex.Lv10n);//20220218

                if (discard_tmp == 0)
                    return (uint)(discard_tmp + mergeRoadCount * (ulong)Hd.CurrProduct.Ctrl_Trigger!.InterpolationLevelDiscardNumTable![TmbScaleIndex] - 2 + 20);
                else
                    return (uint)((mergeRoadCount - discard_tmp) * (ulong)Hd.CurrProduct.Ctrl_Trigger!.InterpolationLevelDiscardNumTable![TmbScaleIndex] + mergeRoadCount - 2 + 20);
            }
            else
            {
                if (discard_tmp == 0)
                    return (uint)(discard_tmp - 2 + 20);
                else
                    return (uint)(mergeRoadCount - discard_tmp - 2 + 20);
            }
        }
        //point 2
        internal virtual UInt32 ourGetSigProDiscardNum()  //计算总的数字信号处理丢点数
        {
            int LengthOfInterpFilter_1st = Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings[CoefficientsTableType.Coefficients1].LengthOfPartA; //一级插值滤波器阶数
            int LengthOfInterpFilter_2nd = Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings[CoefficientsTableType.Coefficients1].LengthOfPartB; //二级插值滤波器阶数
            //int LengthOfTiadc = Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings[Calibration.Data.Base.CoefficientsTableType.Coefficients2].Length;                   //TIADC滤波器阶数
            UInt32 TiADCDiscard = 256;  //进入TIADC模块会丢掉256个点
            int LengthOfAfc = 0;
            if (Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings.ContainsKey(CoefficientsTableType.Coefficients3))
                LengthOfAfc = Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings[CoefficientsTableType.Coefficients3].Length;                     //AFC滤波器阶数
            int LengthOfPfc = 0;
            if (Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings.ContainsKey(CoefficientsTableType.Coefficients4))
                LengthOfPfc = Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings[CoefficientsTableType.Coefficients4].Length;                     //PFC滤波器阶数
            UInt32 InterpDiscard = ourGetInterpDiscardNum(LengthOfInterpFilter_1st, LengthOfInterpFilter_2nd);
            //UInt32 Discard = (uint)(InterpDiscard + LengthOfTiadc / 2 + LengthOfAfc / 2 + LengthOfPfc / 2);
            UInt32 Discard = (uint)(InterpDiscard + TiADCDiscard + LengthOfAfc / 2 + 2 * LengthOfPfc / 5);

            return Discard;
        }
        internal virtual UInt32 ourGetInterpDiscardNum(int LengthOfInterpFilter_1st, int LengthOfInterpFilter_2nd)  //计算总的插值滤波器丢点数
        {
            AnaChnlTimebaseIndex currAnaChnlTimebaseIndex = (AnaChnlTimebaseIndex)(Hd.UIMessage?.Timebase?.TmbScaleIndex ?? (int)AnaChnlTimebaseIndex.Lv100u);
            UInt32 ExtractNum = (UInt32)(Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.ExtractNumFromAdc ?? 1U);
            UInt32 InterpNum = 1;// (UInt32)(Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.InterpolationNum_Step1 ?? 1U);
            UInt32 Max_1stInterpRatio = 10;  //一级最大插值倍率
            UInt32 DiscardBforeInterp = 2;   //进入插值模块之前丢掉两个点
            UInt32 InterpDiscard = 0;
            if (InterpNum > Max_1stInterpRatio)
                InterpDiscard = (UInt32)(DiscardBforeInterp * InterpNum + LengthOfInterpFilter_1st / ExtractNum / 2);
            else
                InterpDiscard = (UInt32)((DiscardBforeInterp * InterpNum + LengthOfInterpFilter_1st / ExtractNum / 2) * InterpNum + LengthOfInterpFilter_2nd / ExtractNum / 2);

            return InterpDiscard;
        }
        internal virtual UInt64 GetProcBd_LA_TrigCtrl_1st_PreDepth(Int64 xdepth, uint trigSource)
        {
            return GetProcBdTrigCtrl_PreDepth(xdepth, trigSource);
        }
        internal virtual UInt64 GetProcBdTrigCtrl_PreDepth(Int64 xdepth, uint trigSource)
        {
            //计算触发信号板间通信偏移量
            uint trigChannel = (uint)trigSource;
            UInt32 SigProDiscardNum = ourGetSigProDiscardNum();
            AnaChnlTimebaseIndex curTimeBase = (AnaChnlTimebaseIndex)(Hd.UIMessage?.Timebase?.TmbScaleIndex ?? (Int32)AnaChnlTimebaseIndex.Lv5n);
            //if (!adjustnumTableNew.ContainsKey(curTimeBase))
            //    throw new Exception("adjustnumTableNew not contain curTimeBase:" + curTimeBase.ToString());
            UInt32 adjust_num = 0;// adjustnumTableNew[curTimeBase];
            UInt32 xdpeth_fix;
            double mergeRoadCount;//并行路数
                                  //    ulong hardwareExtractNum = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.HardwareExtractNum ?? 1;  硬件抽取数
            if (Hd.UIMessage!.Analog![trigSource].InputSource == AnaChnlIpnutSource.BNC)
            {
                mergeRoadCount = 40d;
            }
            else
            {
                mergeRoadCount = 80d;
            }
            bool condition = false;
            if (condition == true)
            {
                xdpeth_fix = (UInt32)xdepth + SigProDiscardNum + adjust_num;
            }
            else
            {
                xdpeth_fix = (UInt32)xdepth + adjust_num;
            }

            UInt32 PreDepth = (UInt32)Math.Floor(xdpeth_fix / mergeRoadCount) + (UInt32)SysAutoCalibration.Default.Trig_AcqProcBdLooptimBySysClockCount((int)trigChannel);
            return PreDepth;
        }
        internal virtual UInt32 GetAcqBdTrigCtrl_PreDepth(Int64 xdpeth_fix)
        {
            if (xdpeth_fix < 0)
            {
                return 0;
            }
            double mergeRoadCount = (Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode) switch
            {
                AdcInterleaveMode.Mode1To1 => 16,
                AdcInterleaveMode.Mode2To1 => 32,
                _ => 64,
            };
            UInt32 PreDepth = (UInt32)(Math.Floor(xdpeth_fix / mergeRoadCount) * 8)+8; //突发路数8
            return PreDepth;
        }
        //point 2
        /// <summary>
        /// 预触发深度精确定位
        /// </summary>
        internal virtual UInt32 GetAcqBdTrigCtrl_Fine_PreDepth(Int64 xdepth)
        {
            double mergeRoadCount = (Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode) switch
            {
                AdcInterleaveMode.Mode1To1 => 16,
                AdcInterleaveMode.Mode2To1 => 32,
                _ => 64,
            };  //并行路数
            UInt32 InterpNum = 1;
            UInt32 Fine_PreDepth = (UInt32)(Math.Ceiling(xdepth / InterpNum / mergeRoadCount) * mergeRoadCount - xdepth);
            Fine_PreDepth = (UInt32)mergeRoadCount-(UInt32)xdepth %(UInt32)mergeRoadCount;
            if (xdepth< 0)
            {
                Fine_PreDepth = (UInt32)(Math.Ceiling(xdepth / InterpNum / mergeRoadCount) * mergeRoadCount - xdepth);
            }
            if (InterpNum > 1)
            {
                return 0;
            }
            else
            {
                return Fine_PreDepth;
            }
        }
        //point 3
        internal virtual UInt64 GetProcBdTrigCtrl_PosDepth(Int64 xdepth, UInt32 trigSource)
        {
            if (xdepth > 0)
                return 0;
            ulong hardwareExtractNum = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.ExtractNumFromAdc ?? 1;
            double mergeRoadCount = (Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode) switch
            {
                AdcInterleaveMode.Mode1To1 => 8,
                AdcInterleaveMode.Mode2To1 => 16,
                _ => 32,
            };
            UInt64 result = (UInt64)Math.Floor(-xdepth /(mergeRoadCount*2)) * hardwareExtractNum*2;
            return result;
        }
        //point 4
        /// <summary>
        /// 不同采样模式和时基档位下抽值模块引入丢点数
        /// </summary>
        internal virtual UInt32 GetTrigDec_Delay()
        {
            
            Dictionary<AnaChnlTimebaseIndex, (uint Model1To1, uint Model2To1, uint Model4To1)> TrigDec_DelayTable = new ()
            {
                [AnaChnlTimebaseIndex.Lv2p] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv5p] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv10p] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv20p] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv50p] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv100p] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv200p] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv500p] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv1n] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv2n] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv5n] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv10n] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv20n] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv50n]  = (0, 0, 21),
                [AnaChnlTimebaseIndex.Lv100n] = (0, 0, 21),
                [AnaChnlTimebaseIndex.Lv200n] = (0, 0, 21),
                [AnaChnlTimebaseIndex.Lv500n] = (0, 0, 21),
                [AnaChnlTimebaseIndex.Lv1u]   = (0, 0, 21),
                [AnaChnlTimebaseIndex.Lv2u] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv5u] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv10u] =(0, 0, 0),
                [AnaChnlTimebaseIndex.Lv20u] =(0, 0, 0),
                [AnaChnlTimebaseIndex.Lv50u] =(0, 0, 0),
                [AnaChnlTimebaseIndex.Lv100u] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv200u] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv500u] = (0, 0, 0),
                [AnaChnlTimebaseIndex.Lv1m] = (0, 0, 0),

            };

            AnaChnlTimebaseIndex curTimeBase = (AnaChnlTimebaseIndex)(Hd.UIMessage?.Timebase?.TmbScaleIndex ?? (Int32)AnaChnlTimebaseIndex.Lv5n);
            if (!TrigDec_DelayTable.ContainsKey(curTimeBase))
                throw new Exception("adjustnumTableNew not contain curTimeBase:" + curTimeBase.ToString());
            UInt32 adjust_num = (Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode) switch
            {
                AdcInterleaveMode.Mode1To1 => TrigDec_DelayTable[curTimeBase].Model1To1,
                AdcInterleaveMode.Mode2To1 => TrigDec_DelayTable[curTimeBase].Model2To1,
                _ => TrigDec_DelayTable[curTimeBase].Model4To1,
            };
            return adjust_num;            
        }
        internal virtual UInt32 GetProcBdTrigCtrl_2nd_PreDepth(Int64 xdepth, UInt32 trigSource)
        {
            UInt32 result = 0;
            if (xdepth > 0)
                result = (uint)(xdepth);
            return result;
        }
        //point 5
        internal virtual UInt32 GetProcBdTrigCtrl_2nd_SearchRange(Int64 xdepth, UInt32 trigSource)
        {
            UInt32 result = 0;
            if (xdepth < 0)
                result = 0;
            else if (IsInterpolation())
                result = (uint)(16 * 1024 - (1000 - xdepth) - 1000);
            else
                result = 5000;
            return result;
        }
        internal virtual (UInt32 Up, UInt32 Dn, UInt32 Edge) GetDigitTrigCompVolt()
        {
            UInt32 compVolt;
            int trigSource = (int)Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
            int trigSourceYPos = AbstractController_Trigger.AdcCenterValue;
            bool isPositive = true;
            if ((trigSource < ChannelIdExt.AnaChnlNum) || (trigSource == (int)ChannelId.Ext))
            {
                if ((Hd.UIMessage?.Trigger?.TrigType == TriggerType.Edge) && (trigSource == (int)ChannelId.Ext))
                {
                    trigSourceYPos = 0;
                }
                else
                {
                    HdMessage.AnalogOptions analogParameters = Hd.UIMessage!.Analog![trigSource];
                    trigSourceYPos = (int)(Constants.SAMPS_PER_YDIV * analogParameters.Position / analogParameters.Scale + AbstractController_Trigger.AdcCenterValue);
                }

            }
            else
                return (0, 0, (UInt32)(((Hd.UIMessage!.Trigger!.TrigType == TriggerType.Edge && ChannelIdExt.IsDigital((ChannelId)trigSource) ? 1 : 0) << 1) | ((Hd.UIMessage!.Trigger!.Edge!.Slope == EdgeSlope.Rise ? 1 : 0))));//LA

            switch (Hd.UIMessage?.Trigger?.TrigType)
            {
                case TriggerType.Edge:
                    compVolt = (UInt32)((Hd.UIMessage!.Trigger!.Edge!.PosIndex) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
                    isPositive = Hd.UIMessage!.Trigger!.Edge!.Slope == EdgeSlope.Rise;
                    break;
                case TriggerType.PulseWidth:
                    compVolt = (UInt32)((Hd.UIMessage!.Trigger!.Pulse!.PosIndex) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);//正脉宽以下降沿为判断基准，反之亦然
                    isPositive = Hd.UIMessage!.Trigger!.Pulse!.Polarity == PulsePolarity.Positive;
                    break;
                case TriggerType.Runt:
                    compVolt = (UInt32)((Hd.UIMessage!.Trigger!.Runt!.LowerPosIndex) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);//同上
                    isPositive = Hd.UIMessage!.Trigger!.Runt!.Polarity == PulsePolarity.Positive;
                    break;
                case TriggerType.Transition:
                    compVolt = (UInt32)((Hd.UIMessage!.Trigger!.Transition!.LowerPosIndex) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
                    isPositive = Hd.UIMessage!.Trigger!.Transition!.Slope == EdgeSlope.Rise;
                    break;
                case TriggerType.TimeOut:
                    compVolt = (UInt32)((Hd.UIMessage!.Trigger!.TimeOut!.PosIndex) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
                    isPositive = Hd.UIMessage!.Trigger!.TimeOut!.Polarity == LevelPolarity.Positive;
                    break;
                case TriggerType.Glitch:
                    compVolt = (UInt32)((Hd.UIMessage!.Trigger!.Glitch!.PosIndex) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
                    isPositive = Hd.UIMessage!.Trigger!.Glitch!.Polarity == PulsePolarity.Positive;
                    break;
                case TriggerType.SetupHold:
                    compVolt = (UInt32)((Hd.UIMessage!.Trigger!.SetupHold!.ClkPosition.Value) * 10 / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
                    break;
                case TriggerType.Pattern:
                    //10是换算
                    compVolt = (UInt32)((Hd.UIMessage!.Trigger!.Pattern!.Positions![trigSource].Value) * 10 / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
                    isPositive = Hd.UIMessage!.Trigger!.Pattern!.Positions[trigSource].Condition == PatLevelCondition.GreaterThan;
                    break;
                case TriggerType.State:
                    compVolt = (UInt32)((Hd.UIMessage!.Trigger!.State!.Positions![trigSource].Value) * 10 / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
                    break;
                case TriggerType.Interval:
                    compVolt = (UInt32)((Hd.UIMessage!.Trigger!.Interval!.PosIndex) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + AbstractController_Trigger.AdcCenterValue);
                    isPositive = Hd.UIMessage!.Trigger!.Interval!.Polarity == PulsePolarity.Positive;
                    break;
                case TriggerType.Window:
                    compVolt = (UInt32)((Hd.UIMessage!.Trigger!.Window!.LowerPosIndex) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + AbstractController_Trigger.AdcCenterValue);
                    isPositive = (Hd.UIMessage?.Trigger?.Window?.Range ?? WindowRange.Inside) == WindowRange.Outside;
                    break;
                default:
                    compVolt = (UInt32)((Hd.UIMessage!.Trigger!.Edge!.PosIndex) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
                    isPositive = Hd.UIMessage!.Trigger!.Edge!.Slope == EdgeSlope.Rise;
                    break;
            }
            uint currDefaultTrigSensitivity = DefaultTrigSensitivity;
            if ((TriggerCoupling)(Hd.UIMessage?.Trigger?.Edge?.Coupling ?? TriggerCoupling.DC) == TriggerCoupling.NR)
                currDefaultTrigSensitivity /= 2;
            switch (Hd.UIMessage?.Trigger?.TrigType)
            {
                case TriggerType.PulseWidth:
                case TriggerType.Runt:
                case TriggerType.TimeOut:
                case TriggerType.Glitch:
                case TriggerType.Pattern:
                case TriggerType.Window:
                    if (isPositive)
                    {
                        if ((compVolt - DefaultTrigSensitivity) < 0)
                            compVolt = DefaultTrigSensitivity;
                        return (Up: compVolt + DefaultTrigSensitivity, Dn: compVolt, 0);
                    }
                    else
                    {
                        if ((compVolt + DefaultTrigSensitivity) > 4095)
                            compVolt = 4095 - DefaultTrigSensitivity;
                        return (Up: compVolt, Dn: compVolt - DefaultTrigSensitivity, 1);
                    }
                default:
                    if (isPositive)
                    {
                        if ((compVolt - DefaultTrigSensitivity) < 0)
                            compVolt = DefaultTrigSensitivity;
                        return (Up: compVolt, Dn: compVolt - DefaultTrigSensitivity, 0);
                    }
                    else
                    {
                        if ((compVolt + DefaultTrigSensitivity) > 4095)
                            compVolt = 4095 - DefaultTrigSensitivity;
                        return (Up: compVolt + DefaultTrigSensitivity, Dn: compVolt, 1);
                    }
            }
        }
        internal virtual void Trig_SpecailConfig(Int64 xdepth, UInt32 trigSource)
        {
            if (Hd.UIMessage?.Timebase?.TmbScaleIndex! > (int)AnaChnlTimebaseIndex.Lv5n)
            {
                //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SerialTrigEnable, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 9U : 0);
                //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Location_TrigDiscardColumnEn, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 1U : 0U);
            }
            else
            {
                //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SerialTrigEnable, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 9U : 0);
                //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Location_TrigDiscardColumnEn, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 1U : 0U);
            }
        }
        #endregion
    }
}

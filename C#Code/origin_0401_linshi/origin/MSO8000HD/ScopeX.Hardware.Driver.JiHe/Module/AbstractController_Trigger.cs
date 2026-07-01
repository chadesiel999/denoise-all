using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;

namespace ScopeX.Hardware.Driver
{
    internal record TrigSourceParams
    {
        public UInt32 AcqBd_TrigCtrl_1st_SourceWhichAdcStartWith0;
        public UInt32 ProcBd_TrigCtrl_1st_SourceWhichAcqBdStartWith0;
        public UInt32 ProcBd_TrigCtrl_2nd_SourceWhichChannelStartWith0;
    }
    internal abstract class AbstractController_Trigger
    {
        internal virtual int PrimaryClockPeriodByps => 3200;//当前系统时钟周期4000ps,频率250MHz

        public Dictionary<AnaChnlTimebaseIndex, Int32> InterpolationLevelDiscardNumTable = new Dictionary<AnaChnlTimebaseIndex, int>();
        protected virtual void ourConfigTriggerSource()
        {
            int trigType = (int)Hd.UIMessage?.Trigger?.TrigType;//触发类型
            if (trigType == (int)TriggerType.Pattern || trigType == (int)TriggerType.SustainTime)
            {
                return;
            }
            ChannelId trigChannel = (ChannelId)CurrentTrigSource();
            uint tmp_ChannelID = (uint)trigChannel;
            switch (trigChannel)
            {
                case ChannelId chn when chn >= ChannelId.C1 && chn <= ChannelId.C4:
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, (uint)trigChannel);
                    break;
                case ChannelId.AuxIn:
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 0x20);
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_Exit50HzSourceSelect, 0b10);
                    break;
                case ChannelId.Ext:
                case ChannelId.Ext5:
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 0x20);
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_Exit50HzSourceSelect, 0b01);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrigSourceSel1Pro, 0x0300);
           
                    break;
                case ChannelId.AC:
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 0x20);
                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_Exit50HzSourceSelect, 0b00);
                    break;
                case ChannelId chn when chn >= ChannelId.D0 && chn <= ChannelId.D15:
                    tmp_ChannelID = 4 + (uint)(trigChannel - ChannelId.D0);
                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, tmp_ChannelID);
                    break;
            }
            //if (trigChannel!= ChannelId.Ext5|| trigChannel != ChannelId.Ext)
            //    HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrigSourceSel1Pro, 0x0000);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(0x92c4, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(0x92b0, 0);
            //LA
            if (ChannelIdExt.IsDigital((ChannelId)trigChannel))
            {
                HdIO.WriteReg(ProcBdReg.W.LA_TrigSourceSel, (UInt32)((ChannelId)trigChannel - ChannelId.D0 + 1));
#if LA
                HdIO.WriteReg(ProcBdReg.W.LA_TrigEdgeSel, (UInt32)(((Hd.CurrHdMessage!.Trigger!.TrigType == TriggerType.Edge && ChannelIdExt.IsDigital((ChannelId)trigChannel) ? 1 : 0) << 1) | ((Hd.CurrHdMessage!.Trigger!.Edge!.Slope == EdgeSlope.Rise ? 1 : 0))));
#endif
            }
            
        }
        protected virtual void ourConfigTriggerMode()
        {
            //触发模式，只能在FPGA端处理，故归入数字触发模块
            UInt32 trigMode = 0;
            if (Hd.UIMessage?.Trigger?.Mode == TriggerMode.Auto)
                trigMode = 0;
            else if (Hd.UIMessage?.Trigger?.Mode == TriggerMode.Normal)
                trigMode = 1;
            else
            {
                if (Hd.UIMessage?.Trigger?.Mode == TriggerMode.OneShot)
                    trigMode = 2;
                else
                    trigMode = 0;// Hd.CurrHdMessage?.Trigger?.Mode == TriggerMode.Auto ? 2 : (uint)4;
            }

            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Mode, trigMode);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_AutoTimeByms, 250);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_AutoModeEnable, trigMode);//一级触发部分移至处理板
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_AutoModeEnable, trigMode);//2级触发模式

            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_AutoModeEnable, (200 << 1) | trigMode);  //1级触发模式
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_AutoTrigSet, (trigMode << 15) | 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_FindRange, 500);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SearchRange, 5000);
        }
        protected virtual void ourConfigTriggerClock()
        {
            //AdjSource()
        }
        protected virtual void ourConfigHoldOff()
        {
            uint holdOfftime = (uint)(Hd.UIMessage?.Trigger?.HoldoffByps ?? Constants.MIN_HOLDOFF_PS) / (UInt32)Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps;
            uint Eventcnt = (uint)(Hd.UIMessage?.Trigger?.HoldoffByCnt ?? Constants.MIN_HOLDOFF_EVENT);
            uint Data = 0;
            if (Hd.UIMessage?.Trigger?.HoldoffType == DelayOpt.Time)
            {
                Data = holdOfftime;
                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_TypeAcq, 0);
            }
            else
            {
                Data = Eventcnt;
                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_TypeAcq, 1);
            }
            /**********Temp Code Begin***************/
            if (Data == 0)
            {
                Data = 1;
            }
            /**********Temp Code End ****************/
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_DataAcqL, Data & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_DataAcqH, (Data >> 16) & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_HoldOff_DataProL, Data & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_HoldOff_DataProH, (Data >> 16) & 0xffff);
        }
        /// <summary>
        /// 采集Fifo阶段的深度
        /// </summary>
        protected virtual void ourConfigFifoStageDepth_WithAcqLength()
        {
            if (Hd.UIMessage?.Timebase?.AcqLength != AnaChnlStorageMode.Long)
            {
                (Int64 xdepth, Int64 waveDepth) = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetTrigXDepth();
                Int64 hardwareExtractNum = (Int64)(Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.ExtractNumFromAdc ?? 1);
                //xdepth = 1000;
                Int64 xdepth_tmp = hardwareExtractNum switch
                {
                    1 => xdepth / 80 + 19,
                    2 => xdepth * 2 / 80 + 19,
                    _ => (Int64)(xdepth * (Int64)hardwareExtractNum / 80 + hardwareExtractNum * 3),
                };

                //xdepth_tmp = 500 * 2 / 80;
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetL, (UInt32)xdepth_tmp & 0xffff);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetM, (UInt32)(xdepth_tmp >> 16) & 0xffff);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetH, (UInt32)(xdepth_tmp >> 32) & 0xfff);


                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetL, 0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetM, 0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetH, 0);

                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PreDepth, (UInt32)xdepth);
            }
            else
            {
                // DDR中的数据分辨率已经确定，在此只需要发送抽点数就可以了。
                // DDR中触发点的时间已经确定，计算就可得到需要发送的触发位置了。
            }
        }
        internal static Int32 PerYDivAdcSamples = Constants.VIS_ADC_RES / Constants.VIS_YDIVS_NUM;
        internal static Int32 AdcCenterValue = Constants.MAX_ADC_RES / 2;

        protected virtual Dictionary<TriggerType, MethodInfo> TriggerSourceFuncTable
        {
            get => triggerSourceFuncTable;
        }

        private readonly Dictionary<TriggerType, MethodInfo> triggerSourceFuncTable = new Dictionary<TriggerType, MethodInfo>();

        internal virtual uint DefaultTrigSensitivity
        {
            get => Hd.CurrProduct!.Acquirer_AnalogChannel!.DefaultTrigSensitivity;
        }
        internal virtual uint CurrentTrigSource()
        {
            uint trigSource = (uint)(Hd.UIMessage?.Trigger?.Edge?.Source ?? ChannelId.C1);
            if (TriggerSourceFuncTable.ContainsKey(Hd.UIMessage!.Trigger!.TrigType))
            {
                trigSource = (uint)TriggerSourceFuncTable[Hd.UIMessage!.Trigger!.TrigType].Invoke(null, null)!;
            }
            //#region!!! 临时规避：单通道统一发0；双通道，左边发0，右边发1；2023/4/12 李承阳-黄川
            //if (Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode == AdcInterleaveMode.Mode4To1)
            //{
            //    return 0;
            //}

            //if (Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode == AdcInterleaveMode.Mode2To1)
            //{
            //    // 临时规避：只开2，3通道，触发源选2时，要发1，触发源选3时，要发0；2023 / 4 / 14 李承阳 - 黄川
            //    if ((Hd.CurrHdMessage!.Analog![0].Active == false) && (Hd.CurrHdMessage!.Analog![1].Active == true) && (Hd.CurrHdMessage!.Analog![2].Active == true) && (Hd.CurrHdMessage!.Analog![3].Active == false))
            //    {
            //        if (trigSource <= 1)
            //            return 1;
            //        if (trigSource >= 2)
            //            return 0;
            //    }
            //    for (int ch = 0; ch < trigSource; ch++)
            //    {
            //        if (Hd.CurrHdMessage!.Analog![ch].Active)
            //        {
            //            return 1;
            //        }
            //    }
            //    return 0;
            //}
            //#endregion
            return trigSource;
        }

        /// <summary>
        /// 配置数字触发相关参数。如丢点数，搜索宽度等。
        /// </summary>
        internal virtual void ourConfigDgtParameter()
        {
            //临时代码
            UInt32 compVolt;
            int trigSource = (int)Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
            int trigSourceYPos = AbstractController_Trigger.AdcCenterValue;
            bool isPositive = true;

            int toFpgaTrigPos = AbstractController_Trigger.AdcCenterValue;
            int tofpgatrigsec = AbstractController_Trigger.AdcCenterValue;

            if ((trigSource < ChannelIdExt.AnaChnlNum) || (trigSource == (int)ChannelId.Ext))
            {
                if ((Hd.UIMessage?.Trigger?.TrigType == TriggerType.Edge) && (trigSource == (int)ChannelId.Ext))
                {
                    trigSourceYPos = 0;
                }
                else
                {
                    HdMessage.AnalogOptions analogParameters = Hd.UIMessage!.Analog![trigSource];
                    int trigType = (int)Hd.UIMessage?.Trigger?.TrigType;//触发类型
                    if (trigType == (int)TriggerType.Delay 
                        || trigType == (int)TriggerType.SetupHold
                        ||trigType==(int)TriggerType.SustainTime
                        || trigType == (int)TriggerType.Pattern)
                        return;
                    var probeGain = analogParameters.ProbeGain;
                    int invert = 1;
                    if (analogParameters.IsInverted)
                    {
                        invert = -1;
                    }
                    int triggerSource = (int)Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
                    int oldSlope = (int)Hd.UIMessage!.Trigger!.Edge!.Slope;
                    int newSlope = oldSlope;
                    if (triggerSource < ChannelIdExt.AnaChnlNum)
                    {
                        if (Hd.UIMessage!.Analog![triggerSource].IsInverted && oldSlope != (int)EdgeSlope.Both)
                            newSlope = 1 - oldSlope;
                    }
                    int trigSourceZero = AbstractController_Trigger.AdcCenterValue;
                    switch (trigType)
                    {
                        case (int)TriggerType.Edge:
                            if (oldSlope != newSlope)
                                trigSourceZero -= (int)(2.0 * analogParameters.PositionIndex * Constants.VIS_ADC_RES / Constants.IDX_PER_YDIV / Constants.VIS_YDIVS_NUM);
                            toFpgaTrigPos = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Edge?.Position * invert / probeGain + analogParameters.Position) / analogParameters.Scale) + trigSourceZero; 
                            break;
                        case (int)TriggerType.PulseWidth:
                            toFpgaTrigPos = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Pulse?.Position * invert / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;
                            break;
                        case (int)TriggerType.Transition:
                            //tofpgatrigsec = (int)((Hd.UIMessage?.Trigger?.Transition?.UpperPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
                            //toFpgaTrigPos = (int)((Hd.UIMessage?.Trigger?.Transition?.LowerPosIndex ?? 0) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
                            if (Hd.UIMessage?.Trigger?.Transition?.Slope == EdgeSlope.Rise)
                            {
                                toFpgaTrigPos = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Transition?.UpperPosition * invert / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;
                                tofpgatrigsec = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Transition?.LowerPosition * invert / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;

                            }
                            else
                            {
                                tofpgatrigsec = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Transition?.UpperPosition * invert / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;
                                toFpgaTrigPos = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Transition?.LowerPosition * invert / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;
                            }
                            break;
                        case (int)TriggerType.Video:
                            toFpgaTrigPos = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Video?.Position * invert / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;
                            break;
                        case (int)TriggerType.Serial:

                            toFpgaTrigPos = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Edge?.Position * invert / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;
                            break;
                        case (int)TriggerType.Runt:
                            if (Hd.UIMessage?.Trigger?.Runt?.Polarity == PulsePolarity.Positive)
                            {
                                tofpgatrigsec = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Runt?.UpperPosition * invert / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;
                                toFpgaTrigPos = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Runt?.LowerPosition * invert / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;
                            }
                            else
                            {
                                toFpgaTrigPos = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Runt?.UpperPosition * invert / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;
                                tofpgatrigsec = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Runt?.LowerPosition * invert / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;

                            }
                            break;
                        case (int)TriggerType.TimeOut:
                            toFpgaTrigPos = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.TimeOut?.Position * invert / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;
                            break;
                        case (int)TriggerType.NEdge:
                            toFpgaTrigPos = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage.Trigger.NEdge.Position * invert / probeGain + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;
                            break;

                            //Hd.UIMessage?.Trigger?.Transition.
                    }
                    //toFpgaTrigPos = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage.Trigger.Edge.Position + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue; 
                    //if (Hd.UIMessage!.Trigger!.Edge!.Coupling == TriggerCoupling.DC)
                    //    toFpgaTrigPos += (int)(Constants.SAMPS_PER_YDIV * analogParameters.Position / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;

                }
                int diff = 0;
                switch (Hd.UIMessage?.Trigger?.TrigType)
                {
                    case TriggerType.Edge:
                        diff = 0;
                        break;
                    case TriggerType.PulseWidth:
                        diff = 0;
                        break;
                    case TriggerType.Transition:
                        diff = 0;
                        break;
                    case TriggerType.Video:
                        diff = 0;
                        break;
                    default:
                        break;
                }
                if (Hd.UIMessage?.Trigger?.Edge?.Slope == EdgeSlope.Fall)
                {
                    toFpgaTrigPos -= diff;
                }
                else if (Hd.UIMessage?.Trigger?.Edge?.Slope == EdgeSlope.Rise)
                {
                    toFpgaTrigPos += diff;
                }

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, 0);

                toFpgaTrigPos = toFpgaTrigPos > 65535 ? 65535 : toFpgaTrigPos;
                toFpgaTrigPos = toFpgaTrigPos < 25 ? 25 : toFpgaTrigPos;
                tofpgatrigsec = tofpgatrigsec > 255 ? 255 : tofpgatrigsec;
                tofpgatrigsec = tofpgatrigsec < 25 ? 25 : tofpgatrigsec;
                HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve17, (uint)(toFpgaTrigPos));//找点触发电平thj
                if (Hd.UIMessage?.Trigger?.TrigType!=TriggerType.Serial)
		        {
                    switch (trigSource)//低电平
                    {
                        case (int)ChannelId.C1:
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level2, (uint)(toFpgaTrigPos));
                            break;
                        case (int)ChannelId.C2:
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh2Level2, (uint)(toFpgaTrigPos));
                            break;
                        case (int)ChannelId.C3:
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh3Level2, (uint)(toFpgaTrigPos));
                            break;
                        case (int)ChannelId.C4:
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh4Level2, (uint)(toFpgaTrigPos));
                            break;

                    }
                    #if JiHe_MSO7000X
                    switch (trigSource)//高电平
                    {
                        case (int)ChannelId.C1:
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1Level1, (uint)(tofpgatrigsec));
                            break;
                        case (int)ChannelId.C2:
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh2Level1, (uint)(tofpgatrigsec));
                            break;
                        case (int)ChannelId.C3:
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh3Level1, (uint)(tofpgatrigsec));
                            break;
                        case (int)ChannelId.C4:
                            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh4Level1, (uint)(tofpgatrigsec));
                            break;

                    }
#endif
                }

            }
            //else
                // return (0, 0, (UInt32)(((Hd.CurrHdMessage!.Trigger!.TrigType == TriggerType.Edge && ChannelIdExt.IsDigital((ChannelId)trigSource) ? 1 : 0) << 1) | ((Hd.CurrHdMessage!.Trigger!.Edge!.Slope == EdgeSlope.Rise ? 1 : 0))));//LA


        }
        /// <summary>
        /// 开关数字触发
        /// </summary>
        /// <param name="bOpen"></param>
        protected virtual void ourSwitchDgtStatus(bool bOpen)
        {

        }

        #region TriggerTypeAndParameter
        protected virtual void ourConfigTypeAndParameter()
        {
            if (Hd.UIMessage == null)
                return;
            TriggerType currType = Hd.UIMessage?.Trigger?.TrigType ?? TriggerType.Edge;
            #region Step1 设置类型
            #endregion

            #region Step2 设置类型需要的参数
            if (triggerTypeDefineTable.ContainsKey(currType))
            {
                if (TriggerTypeDefineTable[currType].Value != null)
                    TriggerTypeDefineTable[currType].Value.Invoke(null, null);
            }
            #endregion
        }
        #endregion
        //不同产品对改变进行不同的配置
        protected virtual Dictionary<TriggerType, KeyValuePair<UInt32/*FPGA Define Code ,see ....寄存器的说明*/, MethodInfo/*ConfigFunction*/>> TriggerTypeDefineTable
        {
            get => triggerTypeDefineTable;
        }
        private readonly Dictionary<TriggerType, KeyValuePair<UInt32/*FPGA Define Code ,see ....寄存器的说明*/, MethodInfo/*ConfigFunction*/>> triggerTypeDefineTable = new Dictionary<TriggerType, KeyValuePair<uint, MethodInfo>>();


        protected virtual void Init()
        {
        }

        protected Action? _ConfigTriggerSource;
        protected Action? _ConfigTriggerMode;
        protected Action? _ConfigTriggerClock;
        protected Action? _ConfigHoldOff;
        protected Action? _ConfigDgtParameter;
        protected Action<bool>? _SwitchDgtStatus;
        protected Action? _ConfigTypeAndParameter;
        protected Action? _ConfigFifoStageDepth_WithAcqLength;

        public static void ConfigTriggerSource() => Hd.CurrProduct?.Ctrl_Trigger?._ConfigTriggerSource?.Invoke();
        public static void ConfigTriggerMode() => Hd.CurrProduct?.Ctrl_Trigger?._ConfigTriggerMode?.Invoke();
        public static void ConfigTriggerClock() => Hd.CurrProduct?.Ctrl_Trigger?._ConfigTriggerClock?.Invoke();
        public static void ConfigHoldOff() => Hd.CurrProduct?.Ctrl_Trigger?._ConfigHoldOff?.Invoke();
        public static void ConfigDgtParameter() => Hd.CurrProduct?.Ctrl_Trigger?._ConfigDgtParameter?.Invoke();
        public static void SwitchDgtStatus(bool bOpen) => Hd.CurrProduct?.Ctrl_Trigger?._SwitchDgtStatus?.Invoke(bOpen);
        public static void ConfigTypeAndParameter() => Hd.CurrProduct?.Ctrl_Trigger?._ConfigTypeAndParameter?.Invoke();
        public static void ConfigFifoStageDepth_WithAcqLength() => Hd.CurrProduct?.Ctrl_Trigger?._ConfigFifoStageDepth_WithAcqLength?.Invoke();
        public static Dictionary<AcqBdNo, int/**/> AcqBdProcBdLoopDelayOf4nsCount = new Dictionary<AcqBdNo, int>();
        public static void GetAcqBdProcBdLoopDelayOf4nsCount()
        {
            //
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using ScopeX.ComModel;
//using ScopeX.Hardware.Calibration.Data.Base;

//namespace ScopeX.Hardware.Driver
//{
//    internal record TrigSourceParams
//    {
//        public UInt32 AcqBd_TrigCtrl_1st_SourceWhichAdcStartWith0;
//        public UInt32 ProcBd_TrigCtrl_1st_SourceWhichAcqBdStartWith0;
//        public UInt32 ProcBd_TrigCtrl_2nd_SourceWhichChannelStartWith0;
//    }
//    internal abstract class AbstractController_Trigger
//    {
//        internal virtual int PrimaryClockPeriodByps => 3200;//当前系统时钟周期4000ps,频率250MHz

//        public Dictionary<AnaChnlTimebaseIndex, Int32> InterpolationLevelDiscardNumTable = new Dictionary<AnaChnlTimebaseIndex, int>();
//        protected virtual void ourConfigTriggerSource()
//        {
//            ChannelId trigChannel = (ChannelId)CurrentTrigSource();
//            uint tmp_ChannelID = (uint)trigChannel;
//            switch (trigChannel)
//            {
//                case ChannelId chn when chn >= ChannelId.C1 && chn <= ChannelId.C4: 
//                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, (uint)trigChannel);
//                    break;
//                case ChannelId.Ext:
//                case ChannelId.Ext5:
//                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 0x20);
//                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_Exit50HzSourceSelect, 1);
//                    break;
//                case ChannelId.AC:
//                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 0x20);
//                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_Exit50HzSourceSelect, 0);
//                    break;
//                case ChannelId chn when chn>= ChannelId.D1 && chn<=ChannelId.D16:
//                    tmp_ChannelID = 4 + (uint)(trigChannel - ChannelId.D1);
//                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, tmp_ChannelID);
//                    break;
//                case ChannelId.AuxIn:
//                    Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 0x20);
//                    HdIO.WriteReg(ProcBdReg.W.IO_Ctrl_Exit50HzSourceSelect, 10);
//                    break;
//            }
//            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(0x92c4, 0);
//            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(0x92b0, 0);
//            //LA
//            if (ChannelIdExt.IsDigital((ChannelId)trigChannel))
//            {
//                HdIO.WriteReg(ProcBdReg.W.LA_TrigSourceSel, (UInt32)((ChannelId)trigChannel - ChannelId.D1 + 1));
//#if LA
//                HdIO.WriteReg(ProcBdReg.W.LA_TrigEdgeSel, (UInt32)(((Hd.CurrHdMessage!.Trigger!.TrigType == TriggerType.Edge && ChannelIdExt.IsDigital((ChannelId)trigChannel) ? 1 : 0) << 1) | ((Hd.CurrHdMessage!.Trigger!.Edge!.Slope == EdgeSlope.Rise ? 1 : 0))));
//#endif
//            }

//        }
//        protected virtual void ourConfigTriggerMode()
//        {
//            //触发模式，只能在FPGA端处理，故归入数字触发模块
//            UInt32 trigMode = 0;
//            if (Hd.UIMessage?.Trigger?.Mode == TriggerMode.Auto)
//                trigMode = 0;
//            else if (Hd.UIMessage?.Trigger?.Mode == TriggerMode.Normal)
//                trigMode = 1;
//            else
//            {
//                if (Hd.UIMessage?.Trigger?.Mode == TriggerMode.OneShot)
//                    trigMode = 2;
//                else
//                    trigMode = 0;// Hd.CurrHdMessage?.Trigger?.Mode == TriggerMode.Auto ? 2 : (uint)4;
//            }

//            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Mode, trigMode);
//            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_AutoTimeByms, 250);
//            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_AutoModeEnable, trigMode);//一级触发部分移至处理板
//            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_AutoModeEnable, trigMode);//2级触发模式

//            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_AutoModeEnable, (200 << 1) | trigMode);  //1级触发模式
//            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_AutoTrigSet, (trigMode << 15) | 1);
//            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_FindRange, 500);
//            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SearchRange, 5000);
//        }
//        protected virtual void ourConfigTriggerClock()
//        {
//            //AdjSource()
//        }
//        protected virtual void ourConfigHoldOff()
//        {
//            uint holdOfftime = (uint)(Hd.UIMessage?.Trigger?.HoldoffByps ?? Constants.MIN_HOLDOFF_PS) / (UInt32)Hd.CurrProduct!.Ctrl_Trigger!.PrimaryClockPeriodByps;
//            uint Eventcnt = (uint)(Hd.UIMessage?.Trigger?.HoldoffByCnt ?? Constants.MIN_HOLDOFF_EVENT);
//            uint Data = 0;
//            if (Hd.UIMessage?.Trigger?.HoldoffType == DelayOpt.Time)
//            {
//                Data = holdOfftime;
//                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_TypeAcq, 0);
//            }
//            else
//            {
//                Data = Eventcnt;
//                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_TypeAcq, 1);
//            }
//            /**********Temp Code Begin***************/
//            if (Data == 0)
//            {
//                Data = 1;
//            }
//            /**********Temp Code End ****************/
//            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_DataAcqL, Data & 0xffff);
//            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_DataAcqH, (Data >> 16) & 0xffff);
//            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_HoldOff_DataProL, Data & 0xffff);
//            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_HoldOff_DataProH, (Data >> 16) & 0xffff);
//        }
//        /// <summary>
//        /// 采集Fifo阶段的深度
//        /// </summary>
//        protected virtual void ourConfigFifoStageDepth_WithAcqLength()
//        {
//            if (Hd.UIMessage?.Timebase?.AcqLength != AnaChnlStorageMode.Long)
//            {
//                (Int64 xdepth, Int64 waveDepth) = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetTrigXDepth();
//                Int64 hardwareExtractNum = (Int64)(Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.ExtractNumFromAdc ?? 1);
//                //xdepth = 1000;
//                Int64 xdepth_tmp = hardwareExtractNum switch
//                {
//                    1 => xdepth / 80 + 19,
//                    2 => xdepth * 2 / 80 + 19,
//                    _ => (Int64)(xdepth * (Int64)hardwareExtractNum / 80 + hardwareExtractNum * 3),
//                };

//                //xdepth_tmp = 500 * 2 / 80;
//                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetL, (UInt32)xdepth_tmp & 0xffff);
//                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetM, (UInt32)(xdepth_tmp >> 16) & 0xffff);
//                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetH, (UInt32)(xdepth_tmp >> 32) & 0xfff);


//                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetL, 0);
//                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetM, 0);
//                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetH, 0);

//                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PreDepth, (UInt32)xdepth);
//            }
//            else
//            {
//                // DDR中的数据分辨率已经确定，在此只需要发送抽点数就可以了。
//                // DDR中触发点的时间已经确定，计算就可得到需要发送的触发位置了。
//            }
//        }
//        internal static Int32 PerYDivAdcSamples = Constants.VIS_ADC_RES / Constants.VIS_YDIVS_NUM;
//        internal static Int32 AdcCenterValue = Constants.MAX_ADC_RES / 2;

//        protected virtual Dictionary<TriggerType, MethodInfo> TriggerSourceFuncTable
//        {
//            get => triggerSourceFuncTable;
//        }

//        private readonly Dictionary<TriggerType, MethodInfo> triggerSourceFuncTable = new Dictionary<TriggerType, MethodInfo>();

//        internal virtual uint DefaultTrigSensitivity
//        {
//            get => Hd.CurrProduct!.Acquirer_AnalogChannel!.DefaultTrigSensitivity;
//        }
//        internal virtual uint CurrentTrigSource()
//        {
//            uint trigSource = (uint)(Hd.UIMessage?.Trigger?.Edge?.Source ?? ChannelId.C1);
//            if (TriggerSourceFuncTable.ContainsKey(Hd.UIMessage!.Trigger!.TrigType))
//            {
//                trigSource = (uint)TriggerSourceFuncTable[Hd.UIMessage!.Trigger!.TrigType].Invoke(null, null)!;
//            }
//            //#region!!! 临时规避：单通道统一发0；双通道，左边发0，右边发1；2023/4/12 李承阳-黄川
//            //if (Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode == AdcInterleaveMode.Mode4To1)
//            //{
//            //    return 0;
//            //}

//            //if (Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode == AdcInterleaveMode.Mode2To1)
//            //{
//            //    // 临时规避：只开2，3通道，触发源选2时，要发1，触发源选3时，要发0；2023 / 4 / 14 李承阳 - 黄川
//            //    if ((Hd.CurrHdMessage!.Analog![0].Active == false) && (Hd.CurrHdMessage!.Analog![1].Active == true) && (Hd.CurrHdMessage!.Analog![2].Active == true) && (Hd.CurrHdMessage!.Analog![3].Active == false))
//            //    {
//            //        if (trigSource <= 1)
//            //            return 1;
//            //        if (trigSource >= 2)
//            //            return 0;
//            //    }
//            //    for (int ch = 0; ch < trigSource; ch++)
//            //    {
//            //        if (Hd.CurrHdMessage!.Analog![ch].Active)
//            //        {
//            //            return 1;
//            //        }
//            //    }
//            //    return 0;
//            //}
//            //#endregion
//            return trigSource;
//        }

//        /// <summary>
//        /// 配置数字触发相关参数。如丢点数，搜索宽度等。
//        /// </summary>
//        internal virtual void ourConfigDgtParameter()
//        {
//            //临时代码
//            UInt32 compVolt;
//            int trigSource = (int)Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
//            int trigSourceYPos = AbstractController_Trigger.AdcCenterValue;
//            bool isPositive = true;

//            int toFpgaTrigPos = AbstractController_Trigger.AdcCenterValue;
//            int tofpgatrigsec = AbstractController_Trigger.AdcCenterValue;

//            if ((trigSource < ChannelIdExt.AnaChnlNum) || (trigSource == (int)ChannelId.Ext))
//            {
//                if ((Hd.UIMessage?.Trigger?.TrigType == TriggerType.Edge) && (trigSource == (int)ChannelId.Ext))
//                {
//                    trigSourceYPos = 0;
//                }
//                else
//                {
//                    HdMessage.AnalogOptions analogParameters = Hd.UIMessage!.Analog![trigSource];
//                    int trigType = (int)Hd.UIMessage?.Trigger?.TrigType;//触发类型
//                    switch(trigType)
//                    {
//                        case (int)TriggerType.Edge:
//                            toFpgaTrigPos += (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage.Trigger.Edge.Position + analogParameters.Position) / analogParameters.Scale);
//                            break;
//                        case (int)TriggerType.PulseWidth:
//                            toFpgaTrigPos += (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage.Trigger.Pulse.Position + analogParameters.Position) / analogParameters.Scale);
//                            break;
//                        case (int)TriggerType.Transition:
//                            tofpgatrigsec += (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Transition?.UpperPosition + analogParameters.Position) / analogParameters.Scale);
//                            toFpgaTrigPos += (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage?.Trigger?.Transition?.LowerPosition + analogParameters.Position) / analogParameters.Scale);
//                            break;
//                            //Hd.UIMessage?.Trigger?.Transition.
//                    }
//                    //toFpgaTrigPos = (int)(Constants.SAMPS_PER_YDIV * (Hd.UIMessage.Trigger.Edge.Position + analogParameters.Position) / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue; 
//                    //if (Hd.UIMessage!.Trigger!.Edge!.Coupling == TriggerCoupling.DC)
//                    //    toFpgaTrigPos += (int)(Constants.SAMPS_PER_YDIV * analogParameters.Position / analogParameters.Scale) + AbstractController_Trigger.AdcCenterValue;

//                }
//                switch(trigSource)
//                {
//                    case (int)ChannelId.C1:
//                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1, (uint)(toFpgaTrigPos));
//                        break;
//                    case (int)ChannelId.C2:
//                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh2, (uint)(toFpgaTrigPos));
//                        break;
//                    case (int)ChannelId.C3:
//                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh3, (uint)(toFpgaTrigPos));
//                        break;
//                    case (int)ChannelId.C4:
//                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh4, (uint)(toFpgaTrigPos));
//                        break;

//                }
//                //switch (trigSource)
//                //{
//                //    case (int)ChannelId.C1:
//                //        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh1, (uint)(tofpgatrigsec));
//                //        break;
//                //    case (int)ChannelId.C2:
//                //        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh2, (uint)(tofpgatrigsec));
//                //        break;
//                //    case (int)ChannelId.C3:
//                //        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh3, (uint)(tofpgatrigsec));
//                //        break;
//                //    case (int)ChannelId.C4:
//                //        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SoftTrigCmpCh4, (uint)(tofpgatrigsec));
//                //        break;

//                //}
//            }
//            else
//                ;// return (0, 0, (UInt32)(((Hd.CurrHdMessage!.Trigger!.TrigType == TriggerType.Edge && ChannelIdExt.IsDigital((ChannelId)trigSource) ? 1 : 0) << 1) | ((Hd.CurrHdMessage!.Trigger!.Edge!.Slope == EdgeSlope.Rise ? 1 : 0))));//LA



//            ////switch (Hd.CurrHdMessage?.Trigger?.TrigType)
//            ////{
//            ////    case TriggerType.Edge:
//            ////        compVolt = (UInt32)((Hd.CurrHdMessage!.Trigger!.Edge!.PosIndex) / Constants.IDX_PER_YDIV * AbstractController_Trigger.PerYDivAdcSamples + trigSourceYPos);
//            ////        isPositive = Hd.CurrHdMessage!.Trigger!.Edge!.Slope == EdgeSlope.Rise;
//            ////        break;
//            ////}


//            ////comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Location_TrigDiscardColumnEn, Hd.CurrDebugVarints!.bEnable_DigitTrigger ? 1U : 0);
//            ////HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SerialTrigEnable, Hd.CurrDebugVarints!.bEnable_DigitTrigger ? 1U : 0);
//            ////comment for JiHe_MSO7000X if (Hd.CurrHdMessage!.Trigger!.TrigType == TriggerType.Serial)
//            ////comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SerialTrigEnable, 0);

//            //var compVolt = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetDigitTrigCompVolt();
//            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, (uint)(compVolt.Up));//0x87e
//            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, (uint)(compVolt.Dn));//0x74a
//            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, compVolt.Edge);//compVolt.Edge

//            ////comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_EdgeSelect, compVolt.Edge);            //处理板二级触发边沿选择
//            //int trigSource = (int)CurrentTrigSource();
//            //if (trigSource < ChannelIdExt.AnaChnlNum)
//            //{
//            //    if (Hd.CurrHdMessage!.Analog![trigSource].InputSource != AnaChnlIpnutSource.SMA)
//            //    {
//            //        //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, (uint)(compVolt.Up));               //处理板二级触发迟滞高电平选择
//            //        //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, (uint)(compVolt.Dn));               //处理板二级触发迟滞低电平选择
//            //    }
//            //}
//            ////HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_CaliTrigDelayEnable, 0);??
//            ////HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_CalibrationNum, 0);

//        }
//        /// <summary>
//        /// 开关数字触发
//        /// </summary>
//        /// <param name="bOpen"></param>
//        protected virtual void ourSwitchDgtStatus(bool bOpen)
//        {

//        }

//        #region TriggerTypeAndParameter
//        protected virtual void ourConfigTypeAndParameter()
//        {
//            if (Hd.UIMessage == null)
//                return;
//            TriggerType currType = Hd.UIMessage?.Trigger?.TrigType ?? TriggerType.Edge;
//            #region Step1 设置类型
//            #endregion

//            #region Step2 设置类型需要的参数
//            if (triggerTypeDefineTable.ContainsKey(currType))
//            {
//                if (TriggerTypeDefineTable[currType].Value != null)
//                    TriggerTypeDefineTable[currType].Value.Invoke(null, null);
//            }
//            #endregion
//        }
//        #endregion
//        //不同产品对改变进行不同的配置
//        protected virtual Dictionary<TriggerType, KeyValuePair<UInt32/*FPGA Define Code ,see ....寄存器的说明*/, MethodInfo/*ConfigFunction*/>> TriggerTypeDefineTable
//        {
//            get => triggerTypeDefineTable;
//        }
//        private readonly Dictionary<TriggerType, KeyValuePair<UInt32/*FPGA Define Code ,see ....寄存器的说明*/, MethodInfo/*ConfigFunction*/>> triggerTypeDefineTable = new Dictionary<TriggerType, KeyValuePair<uint, MethodInfo>>();


//        protected virtual void Init()
//        {
//        }

//        protected Action? _ConfigTriggerSource;
//        protected Action? _ConfigTriggerMode;
//        protected Action? _ConfigTriggerClock;
//        protected Action? _ConfigHoldOff;
//        protected Action? _ConfigDgtParameter;
//        protected Action<bool>? _SwitchDgtStatus;
//        protected Action? _ConfigTypeAndParameter;
//        protected Action? _ConfigFifoStageDepth_WithAcqLength;

//        public static void ConfigTriggerSource() => Hd.CurrProduct?.Ctrl_Trigger?._ConfigTriggerSource?.Invoke();
//        public static void ConfigTriggerMode() => Hd.CurrProduct?.Ctrl_Trigger?._ConfigTriggerMode?.Invoke();
//        public static void ConfigTriggerClock() => Hd.CurrProduct?.Ctrl_Trigger?._ConfigTriggerClock?.Invoke();
//        public static void ConfigHoldOff() => Hd.CurrProduct?.Ctrl_Trigger?._ConfigHoldOff?.Invoke();
//        public static void ConfigDgtParameter() => Hd.CurrProduct?.Ctrl_Trigger?._ConfigDgtParameter?.Invoke();
//        public static void SwitchDgtStatus(bool bOpen) => Hd.CurrProduct?.Ctrl_Trigger?._SwitchDgtStatus?.Invoke(bOpen);
//        public static void ConfigTypeAndParameter() => Hd.CurrProduct?.Ctrl_Trigger?._ConfigTypeAndParameter?.Invoke();
//        public static void ConfigFifoStageDepth_WithAcqLength() => Hd.CurrProduct?.Ctrl_Trigger?._ConfigFifoStageDepth_WithAcqLength?.Invoke();
//        public static Dictionary<AcqBdNo, int/**/> AcqBdProcBdLoopDelayOf4nsCount = new Dictionary<AcqBdNo, int>();
//        public static void GetAcqBdProcBdLoopDelayOf4nsCount()
//        {
//            //
//        }
//    }
//}

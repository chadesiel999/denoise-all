#if !Product_B21_JinHui_PXI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
//   _____
//___|
//   |____
namespace ScopeX.Hardware.Driver
{
    internal class Controller_Trigger_Standard : AbstractController_Trigger
    {
        public Controller_Trigger_Standard(Dictionary<AnaChnlTimebaseIndex, Int32> interpolationLevelDiscardNumTable)
        {
            Init();
            this.InterpolationLevelDiscardNumTable = interpolationLevelDiscardNumTable;
            _ConfigTriggerSource = ourConfigTriggerSource;
            _ConfigTriggerMode = ourConfigTriggerMode;
            _ConfigTriggerClock = ourConfigTriggerClock;
            _ConfigHoldOff = ourConfigHoldOff;
            _ConfigDgtParameter = ourConfigDgtParameter;
            _SwitchDgtStatus = ourSwitchDgtStatus;
            _ConfigTypeAndParameter = ourConfigTypeAndParameter;
            _ConfigFifoStageDepth_WithAcqLength = ourConfigFifoStageDepth_WithAcqLength;
        }
        protected override void ourConfigTriggerClock()
        {
            //AdjSource()
        }

        //==========================
        internal int DDRPositionDelay = 0;
        internal int FIFOPositionDeley = 0;

        //默认的数据延迟fifo深度，在初始化校正过程使用，校正后设置为正确值
        public uint TrigDecDelay = 0;
        //默认的片段长度（仅在初始化校正过程使用，校正后将设置为正确值）
        public uint defaultFragLength = 60;
        //默认的DDR数据预触发偏移修正量（在初始校正过程使用，校正后将设置为正确值）
        public UInt32 predepth_offset = 30 * 8;
        //实时档预触发深度修正量
        public UInt32 RealTimeOffset = 0;

        public int RealTimeFineDiscard = 0;

        //插值模块设计满载所需要的片段长度  
        public uint FragLengthDefinedByInterp
        {
            get { return 7; }
        }

        //片段数据中触发点之后的数据量
        public uint DataAfterTrigInFrag
        {
            get { return 3; }
        }


        private Int64 _PreTrigdotatddrtime = 0;

        /// <summary>
        /// 采集Fifo阶段的深度
        /// </summary>
        protected override void ourConfigFifoStageDepth_WithAcqLength()
        {
            #region 采集板触发深度
            Double trigtimebyfsatui = Hd.UIMessage!.Timebase!.TmbPosition * AbstractAcquirer_AnalogChannel.uS2fs;
            var protectlen = Hd.CurrProduct?.Acquirer_AnalogChannel!.AcquingParameters.AdcInterleaveMode == AdcInterleaveMode.Mode1To1 ? 1024 : 2048;
            protectlen = 0;
            Double ddrtotaltimebyfs = (Hd.UIMessage.Timebase.StorageWaveDotsCnt + protectlen) * Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr;//0530
            Double trigtimebyfsatddr = (ddrtotaltimebyfs / 2 - trigtimebyfsatui);
            if (trigtimebyfsatddr >= 0)
            {
                Int64 trigdotatddrtime = (Int64)Math.Ceiling(trigtimebyfsatddr / 3200_000);//预触发位置计数，3.2ns的周期数
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PreDepthSetL, (UInt32)trigdotatddrtime & 0xffff);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PreDepthSetM, (UInt32)(trigdotatddrtime >> 16) & 0xffff);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PreDepthSetH, (UInt32)(trigdotatddrtime >> 32) & 0xffff);

                //trigtimebyfsatddr = trigtimebyfsatddr * 2;
                ChannelId trigsource = Hd.UIMessage?.Trigger?.Edge?.Source ?? ChannelId.C1;
                //if (trigsource == ChannelId.Ext || trigsource == ChannelId.Ext5)
                //    trigdotatddrtime += 6700/64;//6700


                if (!Hd.UIMessage.bAcquireStopped)
                {
                    if (trigsource == ChannelId.Ext || trigsource == ChannelId.Ext5)
                        trigdotatddrtime += 6200 / 64;//6700
                    _PreTrigdotatddrtime = trigdotatddrtime;
                }
                else
                {
                    trigdotatddrtime = _PreTrigdotatddrtime;
                }
                //trigdotatddrtime += 0;
                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetL_Pro, (UInt32)trigdotatddrtime & 0xffff);
                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetM_Pro, (UInt32)(trigdotatddrtime >> 16) & 0xffff);
                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetH_Pro, (UInt32)(trigdotatddrtime >> 32) & 0xffff);
                //LA
                HdIO.WriteReg(ProcBdReg.W.LA_TrigCtrl_PreDepthSetL, (UInt32)trigdotatddrtime & 0xffff);
                HdIO.WriteReg(ProcBdReg.W.LA_TrigCtrl_PreDepthSetM, (UInt32)(trigdotatddrtime >> 16) & 0xffff);
                HdIO.WriteReg(ProcBdReg.W.LA_TrigCtrl_PreDepthSetH, (UInt32)(trigdotatddrtime >> 32) & 0xffff);

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetL, (UInt32)0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetM, (UInt32)0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetH, (UInt32)0);

                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PosDepthSetH_Pro, (UInt32)0);
                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PosDepthSetM_Pro, (UInt32)0);
                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PosDepthSetL_Pro, (UInt32)0);

                //LA
                HdIO.WriteReg(ProcBdReg.W.LA_TrigCtrl_PosDepthSetL, (UInt32)0);
                HdIO.WriteReg(ProcBdReg.W.LA_TrigCtrl_PosDepthSetM, (UInt32)0);
                HdIO.WriteReg(ProcBdReg.W.LA_TrigCtrl_PosDepthSetH, (UInt32)0);

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetL, (UInt32)0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetM, (UInt32)0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetH, (UInt32)0);

            }
            else
            {
                //Double timebaseprecisionratio = 0.999;
                //ddrtotaltimebyfs = (Hd.UIMessage.Timebase.StorageWaveDotsCnt * timebaseprecisionratio) * Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr;//0530
                //trigtimebyfsatddr = (ddrtotaltimebyfs / 2 - trigtimebyfsatui);
                Int64 trigdotatddrcount = (Int64)(trigtimebyfsatddr / Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr);//后触发位置计数，DDR的点数
                //trigdotatddrcount = (Int64)(trigtimebyfsatui / Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.PerDataByfs_AtDdr);//后触发位置计数，DDR的点数

                //trigdotatddrcount = trigdotatddrcount / 2;

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PreDepthSetL, (UInt32)0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PreDepthSetM, (UInt32)0);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PreDepthSetH, (UInt32)0);

                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetH_Pro, (UInt32)0);
                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetM_Pro, (UInt32)0);
                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetL_Pro, (UInt32)0);

                //LA
                HdIO.WriteReg(ProcBdReg.W.LA_TrigCtrl_PreDepthSetL, (UInt32)0);
                HdIO.WriteReg(ProcBdReg.W.LA_TrigCtrl_PreDepthSetM, (UInt32)0);
                HdIO.WriteReg(ProcBdReg.W.LA_TrigCtrl_PreDepthSetH, (UInt32)0);

                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetL, (UInt32)(UInt32)trigdotatddrcount & 0xffff);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetM, (UInt32)(trigdotatddrcount >> 16) & 0xffff);
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetH, (UInt32)(trigdotatddrcount >> 32) & 0xffff);

                //trigdotatddrcount = trigdotatddrcount * 2;
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetL, 0);
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetM, 0);
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PosDepthSetH, 0);

                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PosDepthSetL_Pro, (UInt32)trigdotatddrcount & 0xffff);
                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PosDepthSetM_Pro, (UInt32)(trigdotatddrcount >> 16) & 0xffff);
                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PosDepthSetH_Pro, (UInt32)(trigdotatddrcount >> 32) & 0xffff);

                //LA
                HdIO.WriteReg(ProcBdReg.W.LA_TrigCtrl_PosDepthSetL, (UInt32)trigdotatddrcount & 0xffff);
                HdIO.WriteReg(ProcBdReg.W.LA_TrigCtrl_PosDepthSetM, (UInt32)(trigdotatddrcount >> 16) & 0xffff);
                HdIO.WriteReg(ProcBdReg.W.LA_TrigCtrl_PosDepthSetH, (UInt32)(trigdotatddrcount >> 32) & 0xffff);
            }
            #endregion
        }
        private Boolean IsInterpolation()
        {
            return false;
        }

        #region TriggerTypeAndParameter
        protected override void ourConfigTypeAndParameter()
        {
            if (Hd.UIMessage == null)
                return;

            // 自动触发的超时计时器配置
            HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve22, 2500);//单位不同
            TriggerType currType = Hd.UIMessage?.Trigger?.TrigType ?? TriggerType.Edge;
            uint trigType = 0;
            #region Step1 设置类型
            if (currType != TriggerType.Serial)
            {    //关闭协议触发
                HdIO.WriteReg(ProcBdReg.W.Decoder_ProtocolTypeForTrigger, 0);
            }
            switch (currType)
            {
                case TriggerType.Edge:
                    ChannelId id = Hd.UIMessage?.Trigger?.Edge!.Source ?? ChannelId.C1;
                    break;
                case TriggerType.PulseWidth:// .PulseWidth:
                    trigType = 0x01 << 8;
                    break;
                case TriggerType.Glitch:
                    trigType = 0x01 << 8;
                    break;
                case TriggerType.Transition:
                    trigType = 0x02 << 3;
                    break;

                case TriggerType.Window:
                    trigType = 0x06 << 3;
                    break;
                //case TriggerType.DropOut:
                //    trigType = 0x03 << 3;
                //    break;
                case TriggerType.Runt:
                    trigType = 0x04 << 3;
                    break;
                case TriggerType.TimeOut:
                    trigType = 0x05 << 3;
                    break;
                case TriggerType.Video:
                    trigType = 0x05;
                    break;
                case TriggerType.Pattern:
                    trigType = 0x01 | 0x00 << 7;
                    break;
                case TriggerType.State:
                    trigType = 0x01 | 0x03 << 7;
                    break;
                case TriggerType.SetupHold:
                    trigType = 0x01 | 0x04 << 7;
                    break;
                case TriggerType.MultiQulified:
                    trigType = 0x01 | 0x05 << 7;
                    break;
                case TriggerType.Serial:
                    switch (Hd.UIMessage?.Trigger?.TrigDecoder?.ProtocolType ?? SerialProtocolType.Close)
                    {
                        case SerialProtocolType.Close:
                        default:
                            trigType = 0x03;
                            break;
                        case SerialProtocolType.RS232:
                            trigType = 12;
                            break;
                        case SerialProtocolType.I2C:
                            trigType = 13;
                            break;
                        case SerialProtocolType.SPI:
                            trigType = 14;
                            break;
                        case SerialProtocolType.CAN:
                            trigType = 15;
                            break;
                        case SerialProtocolType.CAN_FD:
                            trigType = 20;
                            break;
                        case SerialProtocolType.MIL:
                            trigType = 23;
                            break;
                        case SerialProtocolType.FlexRay:
                            trigType = 17;
                            break;

                    }
                    break;
                case TriggerType.Interval:
                    trigType = 0x01 | 0x02 << 7;
                    break;
                default:
                    trigType = 0x00;
                    break;
            };
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, trigType);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrigTypeSelectPro, trigType);
            //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_Glitch_Condition, 0);
            //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_TrigTypeSelect, trigType);
            #endregion

            #region Step2 设置类型需要的参数
            if (currType == TriggerType.MultiQulified)
            {
                //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Cascaded_CascadedEnable, (uint)1);
            }
            else
            {
                //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Cascaded_CascadedEnable, (uint)0);
            }
            if (triggerTypeDefineTable_Standard.ContainsKey(currType))
            {
                if (triggerTypeDefineTable_Standard[currType].Value != null)
                    triggerTypeDefineTable_Standard[currType].Value.Invoke(null, null);
            }
            #endregion
            TriggerCoupling coupling;

            coupling = (TriggerCoupling)(Hd.UIMessage?.Trigger?.Edge?.Coupling ?? TriggerCoupling.DC);

            switch (coupling)
            {
                case TriggerCoupling.DC:
                    //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_ac_dc_setting, 0);
                    //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_AC_DC_setting, 0);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_FreqReject_Enable, 0x00);
                    break;
                case TriggerCoupling.AC:
                    //comment for JiHe_MSO7000X Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_AC_DC_setting, 1 << 15);
                    //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_ac_dc_setting, 1);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_FreqReject_Enable, 0x00);
                    break;
                case TriggerCoupling.HFR:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_FreqReject_Enable, 0x04);
                    break;
                case TriggerCoupling.LFR:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_FreqReject_Enable, 0x02);
                    break;
            }
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_FreqReject_FreqValue, 5000);//50K
            #region 外触发
            bool bIs8GProduct = (Hd.CurrProduct!.ProductType == ProductType.B21_MD8G || Hd.CurrProduct.ProductType == ProductType.B21_HB8G);

            if (!bIs8GProduct && (currType == TriggerType.Edge && (Hd.UIMessage!.Trigger!.Edge!.Source == ChannelId.Ext || Hd.UIMessage!.Trigger!.Edge.Source == ChannelId.Ext5)))
            {
                //comment for JiHe_MSO7000X HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_TrigTypeSelect, 4);
                //根据文档
                Int32 extTrigSetting = ((Hd.UIMessage!.Trigger!.Edge.Slope == EdgeSlope.Fall) ? 1 : 0) << (6 - 0) | 0X1004;
                //extTrigSetting |= 1 << (15 - 0);
                HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Ext_Setting, (uint)extTrigSetting);
            }

            #endregion
        }
        #endregion
        private Dictionary<TriggerType, UInt32> triggerTypeFPGACode = new Dictionary<TriggerType, uint>()
        {
            [TriggerType.Edge] = 0x01,
            [TriggerType.Glitch] = 0x02,
            [TriggerType.MultiQulified] = 0x03,
            [TriggerType.Pattern] = 0x04,
            [TriggerType.PulseWidth] = 0x05,
            [TriggerType.Runt] = 0x06,
            [TriggerType.Serial] = 0x07,
            [TriggerType.SetupHold] = 0x08,
            [TriggerType.State] = 0x09,
            [TriggerType.TimeOut] = 0x0a,
            [TriggerType.Transition] = 0x0b,
            [TriggerType.Video] = 0x0c,
            [TriggerType.Window] = 0x0d,
            [TriggerType.Interval] = 0x0e,
            [TriggerType.Delay] = 0x0f,
            [TriggerType.SustainTime] = 0x10,
            [TriggerType.NEdge] = 0x11,
        };
        protected override Dictionary<TriggerType, KeyValuePair<UInt32/*FPGA Define Code ,see ....寄存器的说明*/, MethodInfo/*ConfigFunction*/>> TriggerTypeDefineTable
        {
            get => triggerTypeDefineTable_Standard;
        }
        //不同产品对改变进行不同的配置
        private readonly Dictionary<TriggerType, KeyValuePair<UInt32/*FPGA Define Code ,see ....寄存器的说明*/, MethodInfo/*ConfigFunction*/>> triggerTypeDefineTable_Standard = new Dictionary<TriggerType, KeyValuePair<uint, MethodInfo>>();


        protected override Dictionary<TriggerType, MethodInfo> TriggerSourceFuncTable
        {
            get => triggerSourceFuncTable_Standard;
        }

        private Dictionary<TriggerType, MethodInfo> triggerSourceFuncTable_Standard = new Dictionary<TriggerType, MethodInfo>();
        protected override void Init()
        {
            triggerTypeDefineTable_Standard.Clear();
            MethodInfo[] methodInfos = typeof(CtrlTrigger_Standard).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).Where(o => o.Name.StartsWith($"Config_")).ToArray<MethodInfo>();
            foreach (TriggerType triggerType in Enum.GetValues(typeof(TriggerType)))
            {
                foreach (MethodInfo methodInfo in methodInfos)
                {
                    if (methodInfo.Name == $"Config_{triggerType.ToString()}")
                    {
                        if (!triggerTypeDefineTable_Standard.ContainsKey(triggerType) && triggerTypeFPGACode.ContainsKey(triggerType))
                        {
                            triggerTypeDefineTable_Standard.Add(triggerType, new(triggerTypeFPGACode[triggerType], methodInfo));
                            break;
                        }
                    }
                }
            }

            triggerSourceFuncTable_Standard.Clear();
            MethodInfo[] methodInfos_getTriggerSource = typeof(CtrlTrigger_Standard).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static).Where(o => o.Name.StartsWith($"GetTriggerSource_")).ToArray<MethodInfo>();
            foreach (TriggerType triggerType in Enum.GetValues(typeof(TriggerType)))
            {
                foreach (MethodInfo methodInfo in methodInfos_getTriggerSource)
                {
                    if (methodInfo.Name == $"GetTriggerSource_{triggerType.ToString()}")
                    {
                        if (!triggerSourceFuncTable_Standard.ContainsKey(triggerType))
                        {
                            triggerSourceFuncTable_Standard.Add(triggerType, methodInfo);
                            break;
                        }
                    }
                }
            }
        }
    }
}
#endif
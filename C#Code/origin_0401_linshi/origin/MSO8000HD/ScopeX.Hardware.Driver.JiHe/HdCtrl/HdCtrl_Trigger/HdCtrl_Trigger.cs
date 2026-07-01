using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;

namespace ScopeX.Hardware.Driver
{
    internal static partial class HdCtrl_Trigger
    {
        private static Dictionary<TriggerType, UInt32> _TriggerTypeCode = new()
        {
            { TriggerType.Edge          , 0    },
            { TriggerType.SetupHold     , 513  },
            { TriggerType.Pattern       , 1    },
            { TriggerType.State         , 385  },
            { TriggerType.MultiQulified , 641  },
            { TriggerType.PulseWidth    , 0x08 },
            { TriggerType.Transition    , 0x10 },
            { TriggerType.Runt          , 0x20 },
            { TriggerType.Window        , 0x30 },
            { TriggerType.TimeOut       , 0x28 },
            { TriggerType.Glitch        , 0x08 },
            { TriggerType.MultiChnlOr   , 769  },
        };
        internal static void ConfigType(TriggerType triggerType)
        {
            ChannelId source = Hd.UIMessage?.Trigger?.Edge?.Source ?? ChannelId.C1;
            if (_TriggerTypeCode.ContainsKey(triggerType))
            {
                if (source == ChannelId.Ext)
                {
                    HdCtrl_Trigger.Config_MultiChnlOr();
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 4);
                    HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrigTypeSelectPro, 4);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_TrigTypeSelect, 4);
                }
                else
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, _TriggerTypeCode[triggerType]);
                    HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrigTypeSelectPro, _TriggerTypeCode[triggerType]);
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_TrigTypeSelect, _TriggerTypeCode[triggerType]);
                }
            }
        }//????

        private static Dictionary<TriggerMode, UInt32> _TriggerModeCode = new()
        {
            { TriggerMode.Auto, 0 },
            { TriggerMode.Normal, 1},
            { TriggerMode.OneShot, 2},
        };
        internal static void ConfigTriggerMode(TriggerMode triggerMode)//????
        {
            if (_TriggerModeCode.ContainsKey(triggerMode))
            {
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Mode, _TriggerModeCode[triggerMode]);
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_Write15, _TriggerModeCode[triggerMode]);
            }
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_AutoTimeByms, 250);//250
        }

        internal static void ConfigHoldOff(TriggerHoldOffParams triggerHoldOff)//????
        {
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_HoldOffMode, triggerHoldOff.Type == DelayOpt.Time ? 0 : 1u);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOffAcqL, AcqBdReg.W.TrigCtrl_HoldOffAcqH, (triggerHoldOff.CtrlWords + 1));

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_HoldOffProL, ProcBdReg.W.TrigCtrl_HoldOffProH, (triggerHoldOff.CtrlWords + 1));
        }

        internal static void ConfigDigtalParams(TriggerDigtalParams triggerDigtal)//????
        {
            if (triggerDigtal.Source == ChannelId.Ext)
            {
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_trig_ext_setting, 0X80FE);
            }
            else
            {
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_trig_ext_setting, 0);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_CaliEnable, 0);
            }

            if (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.bIsLongStorageMode ?? false)
            {
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_DigitalTrigEnAcq, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 3U : 0);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SerialTrigEnable, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 3U : 0);
            }
            else
            {
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_DigitalTrigEnAcq, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 1U : 0);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SerialTrigEnable, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 1U : 0);
            }
            UInt32 InterplotNum = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.InterplotNumToDMA ?? 1;
            UInt32 InterplotNumPre = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.InterplotNumFromADC ?? 1;
            Int32 AutoNum = 0;
            if (InterplotNumPre > 1)
            {
                switch (InterplotNum)
                {
                    case 100:
                        AutoNum = 600;
                        break;
                    case 200:
                        AutoNum = 500;
                        break;
                    default:
                        AutoNum = 400;
                        break;
                }

                UInt32 trig_2nd_serach_num = (UInt32)(2000 + AutoNum * InterplotNumPre * InterplotNum);
                /*                trig_2nd_serach_num += 10000;*/
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SearchRangeL16, trig_2nd_serach_num);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SearchRangeH16, trig_2nd_serach_num >> 16);
            }
            else
            {
                switch (InterplotNum)
                {
                    case 10:
                        AutoNum = 7000;
                        break;
                    case 20:
                        AutoNum = 20000;
                        break;
                    case 50:
                        AutoNum = 25000;
                        break;
                    case 100:
                        AutoNum = 50000;
                        break;
                    case 200:
                        AutoNum = 10000;
                        break;
                    default:
                        // AutoNum = 10000;
                        AutoNum = 500;
                        break;
                }
                bool bFast = Hd.UIMessage!.Display!.IsFast;
                Int32 ActiveChnlCnt = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.ActiveChnlCnt;
                if (bFast && Hd.UIMessage?.Timebase?.TmbScaleIndex == (int)AnaChnlTimebaseIndex.Lv5n && (UInt32)ActiveChnlCnt == 1)
                    AutoNum = 700;
                //UInt32 trig_2nd_serach_num = 10000;
                //  UInt32 trig_2nd_serach_num = (UInt32)1000 + AutoNum * InterplotNumPre * InterplotNum;
                UInt32 trig_2nd_serach_num = (UInt32)(1000 + AutoNum);//cij_new
                //trig_2nd_serach_num += 5000;
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SearchRangeL16, trig_2nd_serach_num);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SearchRangeH16, trig_2nd_serach_num >> 16);
            }
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_search_en, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_AutoModeEnable, 1);
        }

        internal static void ConfigSourceAndType(TriggerSourceAndType sourceAndType)//????
        {
            // todo：触发源的下发

            if (sourceAndType.Type == TriggerType.Edge)
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 0);
            else
            {

            }

            #region trig_init
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ChMode_SamplingMode, 180);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TestDataMode, 2);
            if (sourceAndType.Source == ChannelId.Ext)
            {
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_trig_ext_setting, 0X80FE);
            }
            else
            {
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_trig_ext_setting, 0);
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_CaliEnable, 0);
            }

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PreDepthFine, 10);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOffAcqH, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOffAcqL, 40);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TypeAcq, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_DigitalTrigEnAcq, 7);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SignDataDelayAdjust, 27);

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Mode, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ForceTrigEnable, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_AutoTimeByms, 250);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_CaliNum, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_CaliEnable, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_HoldOffMode, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_HoldOffProH, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_HoldOffProL, 40);

            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelectPro, 0); 
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrigTypeSelectPro, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_trig_ext_setting, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_AutoModeEnable, 0);

            #endregion
        }

        internal static void ConfigDepth(TriggerDepthParams depthParams)//????
        {
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PosDepthSetH, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PosDepthSetM, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PosDepthSetL, 0);
            Int64 preDepth = (Int64)(depthParams.PreDepth) / 80;
            if (Hd.UIMessage!.Timebase!.TmbScale == 0.01)   ///10ns
            {
                preDepth = (Int64)(depthParams.PreDepth) / 80 / 2 + 10;
            }
            else if (Hd.UIMessage!.Timebase!.TmbScale == 0.02)   ///20ns
            {
                preDepth = (Int64)(depthParams.PreDepth) / 80 / 2 + 17;
            }
            else if (Hd.UIMessage!.Timebase!.TmbScale == 0.05)   ///50ns
            {
                preDepth = (Int64)(depthParams.PreDepth) / 80 / 2 + 40;
            }
            else if (Hd.UIMessage!.Timebase!.TmbScale == 0.1)   ///100ns
            {
                preDepth = (Int64)(depthParams.PreDepth) / 80 / 2 + 70;
            }
            else if (Hd.UIMessage!.Timebase!.TmbScale == 0.2)   ///200ns
            {
                preDepth = (Int64)(depthParams.PreDepth) / 80 / 2 + 135;
            }
            else if (Hd.UIMessage!.Timebase!.TmbScale == 0.5)   ///500ns
            {
                preDepth = (Int64)(depthParams.PreDepth) / 80 / 2 + 325;
            }
            // preDepth = (Int64)(depthParams.PreDepth) / 80;
            //UInt64 preDepth = (depthParams.PreDepth+1840*2) / 80/2-20; //ZH0701
            //Int64 preDepth = (Int64)(depthParams.PreDepth) / 80 / 2 +45;
            //Int64 preDepth = (Int64)(depthParams.PreDepth + 1840 * 2) / 80 / 2;

            //            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetH, (UInt32)((preDepth >> 32) & 0xffff));
            //            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetM, (UInt32)((preDepth >> 16) & 0xffff));
            //            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetL, (UInt32)(preDepth & 0xffff));

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PreDepthFine, 40);
            //cij_new
            //  UInt64 preDepth2nd = depthParams.PreDepth2nd;
            //  HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_PreDepth, (UInt32)preDepth2nd);

            Double timescale_AVE = (Hd.UIMessage?.Timebase?.TmbScale ?? 0.005);//us单位
            AnaChnlStorageMode acqMode = Hd.UIMessage?.Timebase.StorageMode ?? AnaChnlStorageMode.Normal;
            if (acqMode == AnaChnlStorageMode.Fast)//DPX模式的触发深度补偿
            {
                preDepth += depthParams.DPXPreDepth;
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetH, (UInt32)((preDepth >> 32) & 0xffff));
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetM, (UInt32)((preDepth >> 16) & 0xffff));
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetL, (UInt32)(preDepth & 0xffff));
                if (timescale_AVE >= 0.005)
                {
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_PreDepth, (UInt32)depthParams.PreDepth2nd + 3);
                }
                else
                {
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_PreDepth, (UInt32)((Int64)depthParams.PreDepth2nd + depthParams.DPXPreDepth2nd));
                }
            }
            else//一般情况
            {
                //preDepth += 100;
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetH, (UInt32)((preDepth >> 32) & 0xffff));
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetM, (UInt32)((preDepth >> 16) & 0xffff));
                //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetL, (UInt32)(preDepth & 0xffff));

                //HdIO.WriteReg(ProcBdReg.W.TrigAdv_ProWrReverse0, 3);//二级触发来的读使能延时
                //HdIO.WriteReg(ProcBdReg.W.TrigAdv_ProWrReverse1, 2);//softfifo数据延时，对齐二级触发



                if (timescale_AVE >= 0.005)
                {
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_PreDepth, (UInt32)depthParams.PreDepth2nd + 1);
                }
                else
                {
                    //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_PreDepth, (UInt32)depthParams.PreDepth2nd + 3);
                }
            }

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_SerialTrigEnable, Hd.CurrDebugVarints.bEnable_DigitTrigger ? 1U : 0);
        }

        internal static void ConfigSpecail(UInt32 enAdjust)//????
        {
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_adjust_en, 0x0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_PreDepthSetL, (UInt32)procBdTrigCtrl_1st_PreDepth & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_PreDepthSetM, (UInt32)(procBdTrigCtrl_1st_PreDepth >> 16) & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_PreDepthSetH, (UInt32)(procBdTrigCtrl_1st_PreDepth >> 32) & 0xfff);

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_trig_predepth_set1_l16, (UInt32)procBdTrigCtrl_1st_PreDepth1 & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_trig_predepth_set1_m16, (UInt32)(procBdTrigCtrl_1st_PreDepth1 >> 16) & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_trig_predepth_set1_h16, (UInt32)(procBdTrigCtrl_1st_PreDepth1 >> 32) & 0xfff);

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_trig_predepth_set2_l16, (UInt32)procBdTrigCtrl_1st_PreDepth2 & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_trig_predepth_set2_m16, (UInt32)(procBdTrigCtrl_1st_PreDepth2 >> 16) & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_trig_predepth_set2_h16, (UInt32)(procBdTrigCtrl_1st_PreDepth2 >> 32) & 0xfff);

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_trig_predepth_set3_l16, (UInt32)procBdTrigCtrl_1st_PreDepth3 & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_trig_predepth_set3_m16, (UInt32)(procBdTrigCtrl_1st_PreDepth3 >> 16) & 0xffff);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_1st_trig_predepth_set3_h16, (UInt32)(procBdTrigCtrl_1st_PreDepth3 >> 32) & 0xfff);

            ////Point 6
            //HdIO.WriteReg((UInt32)ProcBdReg.W.dbi_DBI_TRIG2NDPRETRIGDEPTHINTERP, (uint)procBdReginterp_2st_PosDepth); //dbi发送二级触发预触发深度
            ////point 7
            //HdIO.WriteReg((UInt32)ProcBdReg.W.dbi_DBI_DBIPROAUTOTRIGNUM, 0x400); //dbi发送二级触发找点范围 default:0x20 
            //TriggerType currType = Hd.CurrHdMessage?.Trigger?.TrigType ?? TriggerType.Edge;
            //if (currType != TriggerType.Edge)
            //{
            //    HdIO.WriteReg((UInt32)ProcBdReg.W.dbi_DBI_DBIPROAUTOTRIGNUM, 0x1); //dbi发送二级触发找点范围 default:0x20 
            //}
        }

        internal static void ConfigAdvanced()//????
        {
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ChMode_SamplingMode, 180);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Down, 2000);//2000
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, 2148);//2048
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Down, 2200);//2000
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage2Up, 2348);//2048
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TestDataMode, 2);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_EdgeSelect, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_PWSetAcq, 0X01);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_SlopeSetAcq, 0X01);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TrigTypeSelectAcq, 0X10);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PreDepthFine, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOffAcqH, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOffAcqL, 40);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_TypeAcq, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_DigitalTrigEnAcq, 7);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SourceSelectAcq, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SignDataDelayAdjust, 51);//edge=28 slope=51  
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_Width1LAcq, 400);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_Width1HAcq, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_Width2LAcq, 500);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigAdv_Width2HAcq, 0);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.FifoCtrl_BFIFOFullProgDepth, 250);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_DigitalTrigEnAcq, 1);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_AutoTestOffsetEnable, 1);

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_Mode, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ForceTrigEnable, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_AutoTimeByms, 250);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_CaliNum, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_CaliEnable, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_HoldOffMode, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_HoldOffProH, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_HoldOffProL, 40);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PosDepthSetH, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PosDepthSetM, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PosDepthSetL, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetH, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetM, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_PreDepthSetL, 10);//82
            //HdIO.WriteReg(ProcBdReg.W.DataPath_SoftfifoFullThreshold, 8000);
            HdIO.WriteReg(ProcBdReg.W.DataPath_SyncFifoFullThreshold, 8000);
            //           HdIO.WriteReg(ProcBdReg.W.Scan_ScanEnablePro, 0);
            // HdIO.WriteReg(ProcBdReg.W.Debug_SingleModePro, 1);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_SourceSelPro, 0);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_TrigTypeSelectPro, 0X10);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_TrigTypeSelect, 0X10);
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_PWSetPro, 0X01);
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_SlopeSetPro, 0X01);
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_Width2tLPro, 400);
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_Width2HPro, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigAdv_Width1LPro, 500);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width2SetL, 500);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width2SetM, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width2SetH, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width1SetL, 400);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width1SetM, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_Width1SetH, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_PreDepth, 505);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Down, 2000);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage1Up, 2048);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Down, 2100);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_CompareVoltage2Up, 2148);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_trig_ext_setting, 0);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_2nd_AutoModeEnable, 1);
        }
    }

    internal record TriggerSourceAndType()
    { 
        internal ChannelId Source
        {
            get;
            init;
        }

        internal TriggerType Type
        {
            get;
            init;
        }
    }

    internal record TriggerHoldOffParams()
    {
        internal DelayOpt Type 
        {
            get;
            init;
        }

        internal UInt32 HoldOffEvent
        {
            get;
            init;
        }

        internal UInt32 HoldOffTime
        {
            get;
            init;
        }

        internal UInt32 CtrlWords => Type == DelayOpt.Time ? HoldOffTime : HoldOffEvent;
    }

    internal record TriggerDigtalParams()
    {
        internal ChannelId Source
        {
            get;
            init;
        }

        internal UInt32 SerachRange
        {
            get;
            init;
        }
    }

    internal record TriggerDepthParams()
    { 
        /// <summary>
        /// 预触发深度
        /// </summary>
        internal UInt64 PreDepth
        {
            get;
            init;
        }

        /// <summary>
        /// 后触发深度
        /// </summary>
        internal UInt64 PostDepth
        {
            get;
            init;
        }

        /// <summary>
        /// 2级预触发深度
        /// </summary>
        internal UInt64 PreDepth2nd
        {
            get;
            init;
        }
        /// <summary>
        /// dpx触发深度补偿值
        /// </summary>
        internal Int64 DPXPreDepth
        {
            get;
            init;
        }
        /// <summary>
        /// dpx二级触发深度补偿值
        /// </summary>
        internal Int64 DPXPreDepth2nd
        {
            get;
            init;
        }
    }
}

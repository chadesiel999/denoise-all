using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;

namespace ScopeX.Hardware.Driver
{
    public partial class Acquirer_AnalogChanel_DBI13G : AbstractAcquirer_AnalogChannel
    {
        internal override (Int64 depth, Int64 waveDepth) GetTrigXDepth()
        {
            double tmbScale = Hd.UIMessage?.Timebase?.TmbScale ?? 1.0;
            double tmbPosition = Hd.UIMessage?.Timebase?.TmbPosition ?? 0;
            UInt32 InterplotNum = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.InterplotNumToDMA ?? 1;
            UInt32 InterplotNumPre = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.InterplotNumFromADC ?? 1;
            double perDataByfs_AtStorage = (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 50_000_000)/ InterplotNum; //20G sample,50_000_000
            Int64 depth = (Int64)((tmbScale * Constants.VIS_XDIVS_NUM / 2 - tmbPosition) * uS2fs / perDataByfs_AtStorage);
            return (depth, 0);
            ////ProcBdInterpolationNum
            //double tmbScale = Hd.UIMessage?.Timebase?.TmbScale ?? 1.0;
            //double tmbPosition = Hd.UIMessage?.Timebase?.TmbPosition ?? 0;
            //double perDataByfs_AtStorage = (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.PerDataByfs_AtStorage ?? 50_000_000); //20G sample,50_000_000
            //Int64 depth = (Int64)((tmbScale * Constants.VIS_XDIVS_NUM / 2 - tmbPosition) * 1_000_000_000 / perDataByfs_AtStorage);
            //Int64 xdepth;
            //ChannelId id = Hd.UIMessage?.Trigger?.Edge!.Source ?? ChannelId.C1;

            //Int32 trigChannel = (Int32)Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
            //if (trigChannel >= ChannelIdExt.AnaChnlNum)
            //    trigChannel = 0;


            //Int64 dis_num_olp_init = 0;
            //Int64 dis_num_acq = 0;
            //Int64 dis_num_pro = 0;
            //Int64 dis_num_afc = 0;
            //Int64 dis_num_pfc = 462/*pfc*/ + 76 /*fix*/+ 0;
            //Int64 dis_num_pro_fix_1st_depth = 0;//修正插值档位下触发点前移的情况

            //Int64 dis_num_ti = 256;

            //if (IsAcqBdInterpolation())
            //{ dis_num_pro_fix_1st_depth = 10/*插值档硬件固定多丢10个*/+ 4; }//与中间级预触发深度修正的偏移量保持一致 默default：4 ,max<dbi_DBI_DBIPROAUTOTRIGNUM
            //else
            //{
            //    dis_num_pro_fix_1st_depth = 10 + 4;
            //}
            //// if (Hd.CurrDebugVarints.bEnable_Dbi_AmpFreqCoef == false)
            ////     dis_num_afc = 0;
            //if (Hd.CurrDebugVarints.bEnable_Dbi_PhaseFreqCoef == true)
            //    dis_num_pfc = dis_num_pfc + 399;

            ////根据触发源选择第一自带丢点数
            //if (trigChannel == 0)
            //{ dis_num_olp_init = 59 + 101; }
            //else if (trigChannel == 1)
            //{ dis_num_olp_init = 132; }
            //else if (trigChannel == 2)
            //{ dis_num_olp_init = 59 + 101/*通道间的延迟*/; }
            //else
            //{ dis_num_olp_init = 59 + 101/*通道间的延迟*/; }

            //dis_num_acq = 668/*4*inter*/ /*+272DUC+ant*/ + 0/*60/*OLP*/  + dis_num_olp_init/*OLP_INIT*//*?*/; //第一子代没有上变频+抗镜像

            //dis_num_pro = dis_num_afc + dis_num_pfc + dis_num_pro_fix_1st_depth;//591;//1715 /* 799(afc) + 916(pfc)*/ + 0/*multi_inter*/;
            //if (IsAcqBdInterpolation())
            //{
            //    //if (id.IsDigital() )
            //    //    return 100;
            //    if (ProcBdInterpolationNum == 1)
            //    {
            //        //xdepth = (Int64)Math.Floor((((depth + 535 + 400 + 341) / 4)) + 256.0);
            //        xdepth = (Int64)Math.Floor((depth + dis_num_acq + dis_num_pro) / 4.0) + dis_num_ti + 100;
            //        return xdepth;
            //    }
            //    else if (ProcBdInterpolationNum == 5)
            //    {
            //        //xdepth = (Int64)Math.Floor(((depth +40+ dis_num_pro) / ProcBdInterpolationNum + dis_num_acq) / 4.0) + dis_num_ti;//161+200 2000
            //        xdepth = (Int64)Math.Floor(((depth + 160) / ProcBdInterpolationNum + dis_num_pro + dis_num_acq) / 4.0) + dis_num_ti;                                                                                          //xdepth = (Int64)Math.Floor((((((depth + 10 * ProcBdInterpolationNum + 1) / ProcBdInterpolationNum) + 535 + 400 + 341) / 4)) + 256.0);
            //        return xdepth;
            //    }
            //    else if (ProcBdInterpolationNum == 10)
            //    {
            //        xdepth = (Int64)Math.Floor(((depth + 160) / ProcBdInterpolationNum + dis_num_pro + dis_num_acq) / 4.0) + dis_num_ti;//1276
            //                                                                                                                            // xdepth = (Int64)Math.Floor((((((depth + 10 * ProcBdInterpolationNum + 1) / ProcBdInterpolationNum) + 535 + 400 +341) / 4)) + 256.0);
            //        return xdepth;
            //    }
            //    else if (ProcBdInterpolationNum == 20)
            //    {
            //        xdepth = (Int64)Math.Floor(((depth + 160) / ProcBdInterpolationNum + dis_num_pro + dis_num_acq) / 4.0) + dis_num_ti;//1276                                                                                                            // xdepth = (Int64)Math.Floor((((((depth + 10 * ProcBdInterpolationNum + 1) / ProcBdInterpolationNum) + 535 + 400 +341) / 4)) + 256.0);
            //        return xdepth;
            //    }
            //    else if (ProcBdInterpolationNum > 20)
            //    {
            //        xdepth = (Int64)Math.Floor(((depth + 225) / ProcBdInterpolationNum + 8 + dis_num_pro + dis_num_acq) / 4.0) + dis_num_ti;//1276
            //        return xdepth;
            //    }
            //    //else if (ProcBdInterpolationNum > 40)
            //    // {
            //    // xdepth = (Int64)Math.Floor(((depth + 823 + dis_num_pro) / ProcBdInterpolationNum + dis_num_acq) / 4.0) + dis_num_ti;
            //    // return xdepth;
            //    // }
            //    else
            //        return 0;
            //}
            //else
            //{
            //    Int64 hardwareExtractNum = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.Scan2ExtractNum_Total ?? 1;
            //    //if (id.IsDigital() )
            //    //{
            //    //    return 100;
            //    //}
            //    //else
            //    {
            //        if (hardwareExtractNum == 1)
            //        {
            //            return depth + 200/* 开二级调整的偏移量 */ + dis_num_ti + dis_num_pro_fix_1st_depth/* 开二级调整的偏移量 */; /*Trig_XDepthFixedAddDots + 2950*/;
            //        }
            //        else if (hardwareExtractNum == 2)
            //        {
            //            return depth + 500/* 开二级调整的偏移量 */+ dis_num_ti + dis_num_pro_fix_1st_depth;
            //        }
            //        else if (hardwareExtractNum == 4)
            //        {
            //            return depth + 400/* 开二级调整的偏移量 */+ dis_num_ti;
            //        }
            //        else if (hardwareExtractNum == 10)
            //        {
            //            return depth + 400/* 开二级调整的偏移量 */+ dis_num_ti;
            //        }
            //        else
            //        {
            //            return depth + 400/* 开二级调整的偏移量 */+ dis_num_ti;
            //        }
            //    }
            //}
        }

        private Boolean IsAcqBdInterpolation()
        {
            AnaChnlTimebaseIndex anaChnlTimebaseIndex = (AnaChnlTimebaseIndex)(Hd.UIMessage?.Timebase?.TmbScaleIndex ?? (int)AnaChnlTimebaseIndex.Lv100m);
            if (anaChnlTimebaseIndex < AnaChnlTimebaseIndex.Lv20n)
            {
                if (Hd.CurrDebugVarints.bEnable_AcqbdInterpolation)
                {
                    //if (AcquingParameters.InterpolationNum > 1) // todo
                        return true;
                }
            }
            return false;
        }

        internal override TrigSourceParams GetTrigSourceParams(UInt32 trigChannelFromSoft)
        {
            UInt32 AcqBd_TrigCtrl_1st_SourceWhichAdcStartWith0 = 0;
            UInt32 ProcBd_TrigCtrl_1st_SourceWhichAcqBdStartWith0 = 0;
            UInt32 ProcBd_TrigCtrl_2nd_SourceWhichChannelStartWith0 = 0;

            //1.判断触发信号来自于采集板哪个ADC，所有采集板配置相同，所有被选中通道的触发信号都送到处理板做二次选择
            //相当于对应一个采集板上的哪个ADC。以触发的物理通道接入的那个ADC为准。ADC1：发0，ADC2：发1，两片：发0
            if (trigChannelFromSoft < ChannelIdExt.AnaChnlNum)
            {
                if (Hd.CurrProduct!.Acquirer_AnalogChannel!.ChannelBdAdcInputDefines != null)
                    AcqBd_TrigCtrl_1st_SourceWhichAdcStartWith0 = (UInt32)Hd.CurrProduct!.Acquirer_AnalogChannel!.ChannelBdAdcInputDefines![0][(int)trigChannelFromSoft][0].AdcIndex;
            }

            //2.一级触发源：表示来自于那个采集板的信号。用0,1,2，表示。
            if (trigChannelFromSoft < ChannelIdExt.AnaChnlNum)
            {

                uint acqBdIndex = 0;
                int currChBWModeAndActiveState = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.CurrChBWModeAndActiveState ?? 0;
                int FullBWeq0 = (currChBWModeAndActiveState & 0x100) == 0 ? 0 : 1;

                foreach (var v in AcqBoardCoefficientsTablesSendDefine[DbiCoefficientsTablesType.TiAdc])
                {
                    if ((int)v.BandMode == FullBWeq0 && (int)v.ChannelID == trigChannelFromSoft && v.SubbandIndex == 0)
                    {
                        acqBdIndex = (uint)v.FPGAIndex;
                        break;
                    }
                }
                ProcBd_TrigCtrl_1st_SourceWhichAcqBdStartWith0 = acqBdIndex;
            }
            //二级触发，表示对应的物理通道，与DMA中的数据路数一致
            if (trigChannelFromSoft < ChannelIdExt.AnaChnlNum)
                ProcBd_TrigCtrl_2nd_SourceWhichChannelStartWith0 = trigChannelFromSoft;
            else
                ProcBd_TrigCtrl_2nd_SourceWhichChannelStartWith0 = (uint)(trigChannelFromSoft % ChannelIdExt.AnaChnlNum);//乱整


            return new TrigSourceParams()
            {
                AcqBd_TrigCtrl_1st_SourceWhichAdcStartWith0 = AcqBd_TrigCtrl_1st_SourceWhichAdcStartWith0,
                ProcBd_TrigCtrl_1st_SourceWhichAcqBdStartWith0 = ProcBd_TrigCtrl_1st_SourceWhichAcqBdStartWith0,
                ProcBd_TrigCtrl_2nd_SourceWhichChannelStartWith0 = ProcBd_TrigCtrl_2nd_SourceWhichChannelStartWith0
            };
        }
        //point 1
        internal override UInt32 GetAcqBdTrigDiscardColumnNums(Int64 xdepth, UInt32 trigSource)
        {
            if (xdepth < 0)
                return 0;
            double mergeRoadCount = 80d;
            //if (Hd.UIMessage!.Analog![trigSource].InputSource == AnaChnlIpnutSource.BNC)
            //     mergeRoadCount = 40d;
            UInt64 discard_tmp = (UInt64)(xdepth % mergeRoadCount); //改动
            if (IsAcqBdInterpolation())
            {
                AnaChnlTimebaseIndex TmbScaleIndex = (AnaChnlTimebaseIndex)(Hd.UIMessage?.Timebase?.TmbScaleIndex ?? (int)AnaChnlTimebaseIndex.Lv10n);//20220218

                if (discard_tmp == 0)
                    return (uint)(10);//丢点值不能下发0,因此，加一个固定偏移
                else
                    return (uint)((mergeRoadCount - discard_tmp) * 4 + 10);


                // return (uint)(mergeRoadCount - discard_tmp);
            }
            else
            {
                if (discard_tmp == 0)
                    return (uint)(discard_tmp + 10);//丢点值不能下发0,因此，加一个固定偏移
                else
                    return (uint)(mergeRoadCount - discard_tmp + 10);
            }
        }
        //point 2
        internal override UInt64 GetProcBd_LA_TrigCtrl_1st_PreDepth(Int64 xdepth, uint trigSource)
        {
            return GetProcBdTrigCtrl_1st_PreDepth(xdepth, trigSource);
        }
        internal UInt64 GetProcBdTrigCtrl_1st_PreDepth(Int64 xdepth, uint trigSource)
        {
            if (xdepth <= 0)
                return 0;
            if (trigSource >= ChannelIdExt.AnaChnlNum)
                trigSource = 0;
            double mergeRoadCount = 80d;
            //Int32 trigChannel = (Int32)Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource();
            UInt32[] offset = new UInt32[4];
            //for (int subIndex = 0; subIndex < 4; subIndex++)
            //{
            //    //int acqBd = CtrlAnalogChannel_DBI20G.LogicSubbandIndex_AcqBdNo_Correspond[0, currChannelID, subIndex];
            //    string key = $"C{currChannelID}_Sub{subIndex}";
            //    int data = 0;
            //    if (DiscardNumAtRecvAdcData.ContainsKey(key))
            //    {
            //        data = DiscardNumAtRecvAdcData[key];
            //        //data = (int)discanrnum1[subIndex];//测试用丢点值
            //    }

            ulong hardwareExtractNum = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.ExtractNumFromAdc ?? 1;
            UInt64 TrigOffset = (UInt64)SysAutoCalibration.Default.Trig_AcqProcBdLooptimBySysClockCount((int)trigSource);
            if (TrigOffset == 0 || TrigOffset > 80 || TrigOffset < 60)
            {
                offset[0] = 67;
                offset[1] = 68;
                offset[2] = 68;
                offset[3] = 69;

            }

            UInt64 result = (UInt64)Math.Ceiling(xdepth / mergeRoadCount  /*AcqAdcMergeRoadCount*/) * hardwareExtractNum + TrigOffset + offset[trigSource];//57 此处为板间偏
            UInt64 result_inter = (UInt64)Math.Ceiling(xdepth / 80.0) + TrigOffset + offset[trigSource];

            switch (Hd.UIMessage?.Trigger?.TrigType)
            {
                case TriggerType.TimeOut:
                    result += 20;
                    break;
                case TriggerType.PulseWidth:
                    result += 5;
                    break;
                case TriggerType.Runt:
                    result += 22;
                    break;
                default:
                    result += 0;
                    break;
            }

            HdMessage.ProtocolUSBOptions? decodeOption = Hd.UIMessage!.Trigger!.TrigDecoder!.ProtocolOptions! as HdMessage.ProtocolUSBOptions;
            switch (Hd.UIMessage!.Trigger!.TrigDecoder!.ProtocolType)
            {
                case SerialProtocolType.RS232:
                    result -= 803;//133
                    break;
                case SerialProtocolType.I2C:
                    result -= 264;
                    break;
                case SerialProtocolType.SPI:
                    result -= 877;
                    break;
                case SerialProtocolType.USB://
                    if (decodeOption.SignalRate == ProtocolUSB.SignalRate.HighRate)
                    {
                        result += 4;
                    }
                    else if (decodeOption.SignalRate == ProtocolUSB.SignalRate.FullRate)
                    {
                        result -= 7;
                    }
                    else
                    {
                        result -= 150;
                    }
                    break;
                case SerialProtocolType.CAN:
                    result -= 946;
                    break;
                case SerialProtocolType.LIN:
                    result -= 47888;
                    break;
                case SerialProtocolType.FlexRay:
                    result -= 85;
                    break;
                case SerialProtocolType.MIL:
                    result -= 886;
                    break;
                case SerialProtocolType.AudioBus:
                    result += 528;
                    break;
                case SerialProtocolType.ARINC429:
                    result += 22;
                    break;
                case SerialProtocolType.SENT:
                    result += 22;
                    break;
                case SerialProtocolType.SPMI:
                    result += 22;
                    break;
                case SerialProtocolType.Ethernet:
                    result += 22;
                    break;
                case SerialProtocolType.CAN_FD:
                    result += 150;
                    break;
                case SerialProtocolType.NRZ:
                    result -= 25;
                    break;
                case SerialProtocolType.JTAG:
                    result += 61;
                    break;
                case SerialProtocolType.SATA:
                    result += 44;
                    break;

                case SerialProtocolType.PCIe:
                    result -= 30;
                    break;
                default:
                    result += 0;
                    break;
            }

            if (IsAcqBdInterpolation())
                return result_inter;
            else
                return result;
        }
        //point 3
        internal UInt64 GetProcBdTrigCtrl_1st_PosDepth(Int64 xdepth, UInt32 trigSource)
        {
            if (xdepth > 0)
                return 0;
            ulong hardwareExtractNum = Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.ExtractNumFromAdc ?? 1;
            double mergeRoadCount = 80d;

            UInt64 result = (UInt64)Math.Floor(-xdepth / mergeRoadCount) * hardwareExtractNum;
            return result;
        }

        //point 4
        internal override UInt32 GetProcBdTrigCtrl_2nd_PreDepth(Int64 xdepth, UInt32 trigSource)
        {
            Int64 depth = 0;
            Int32 isDigital = 0;
            Int32 trigChannel = (Int32)(Hd.CurrProduct?.Ctrl_Trigger?.CurrentTrigSource() ?? 0);
            if (trigChannel >= (Int32)ChannelId.D1 && trigChannel <= (Int32)ChannelId.D16)
                isDigital = 1;//1206

            double tmbScale = Hd.UIMessage?.Timebase?.TmbScale ?? 1.0;
            double tmbPosition = Hd.UIMessage?.Timebase?.TmbPosition ?? 0;
            double perDataByfs_AtStorage = (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters.PerDataByfs_AtDdr ?? 50_000_000); //20G sample,50_000_000
            if (isDigital == 1)
            {
                depth = (Int64)((tmbScale * Constants.VIS_XDIVS_NUM / 2 - tmbPosition) * 1_000_000_000 / perDataByfs_AtStorage);
                //depth = (Int64)((tmbScale * Constants.VIS_XDIVS_NUM / 2 - tmbPosition) * 1_000_000_000 / perDataByfs_AtStorage / 4);
                if (tmbScale <= 0.01 && tmbScale >= 0.001)
                {
                    depth = (Int64)((tmbScale * Constants.VIS_XDIVS_NUM / 2 - tmbPosition) * 1_000_000_000 / perDataByfs_AtStorage / 4);
                    return (UInt32)depth;
                }
                else if (tmbScale < 0.001)
                {
                    depth = (Int64)((tmbScale * Constants.VIS_XDIVS_NUM / 2 - tmbPosition) * 1_000_000_000 / perDataByfs_AtStorage / 4 / ProcBdInterpolationNum);
                    return (UInt32)depth;
                }

            }
            else
                depth = (Int64)((tmbScale * Constants.VIS_XDIVS_NUM / 2 - tmbPosition) * 1_000_000_000 / perDataByfs_AtStorage);
            if (depth < 0)
                depth = 0;
            return (UInt32)depth + 3;
        }
        //point 5
        internal override UInt32 GetProcBdTrigCtrl_2nd_SearchRange(Int64 xdepth, UInt32 trigSource)
        {
            UInt32 result = 0;
            if (xdepth < 0)
                result = 0;
            else if (IsAcqBdInterpolation())
            {
                //result = (uint)(16 * 1024 - (1000 - xdepth) - 1000);
                result = (uint)2 * 1024;
            }
            else
                //result = (uint)(16 * 1024 - (1000 - xdepth) - 1000);
                result = (uint)2 * 1024;
            // result = 5000;
            return result;
        }

        private UInt64 GetProcBdInterp_2st_PreDepth(Int64 xdepth, uint trigSource)
        {
            UInt32 procBdTrigCtrl_2nd_PreDepth = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetProcBdTrigCtrl_2nd_PreDepth(xdepth, trigSource);
            Int64 dis_num_pro = 591 + 26;
            Int32 fixed_dbi_2nd_predpeth = 4;//default:4;
            if (procBdTrigCtrl_2nd_PreDepth <= 0)
                return 0;
            // UInt64 interp_1st = (UInt64)Math.Floor((procBdTrigCtrl_2nd_PreDepth + 161.0) / ProcBdInterpolationNum + 8);
            // UInt64 interp_2st = (UInt64)Math.Floor((procBdTrigCtrl_2nd_PreDepth + 225.0) / ProcBdInterpolationNum + 16);
            UInt64 interp_1st = (UInt64)Math.Floor((procBdTrigCtrl_2nd_PreDepth + 160.0) / ProcBdInterpolationNum + fixed_dbi_2nd_predpeth);
            UInt64 interp_2st = (UInt64)Math.Floor((procBdTrigCtrl_2nd_PreDepth + 225.0) / ProcBdInterpolationNum + fixed_dbi_2nd_predpeth/**2*/+ 6/*floor+1*/);
            UInt64 interp_no = (UInt64)procBdTrigCtrl_2nd_PreDepth + (UInt64)fixed_dbi_2nd_predpeth;
            if (ProcBdInterpolationNum > 1)
            {
                if (ProcBdInterpolationNum == 5)
                    return interp_1st;
                else if (ProcBdInterpolationNum == 10)
                    return interp_1st;
                else if (ProcBdInterpolationNum == 20)
                    return interp_1st;
                else
                    return interp_2st;
            }
            else
                return interp_no;
        }

        internal override void Trig_SpecailConfig(Int64 xdepth, UInt32 trigSource)
        {
            #region channel_offset_adjust
            //通道间偏移功能
            var offset1 = Hd.UIMessage!.Analog![0]!.InterChannelOffset;
            var offset2 = Hd.UIMessage!.Analog[1]!.InterChannelOffset;
            var offset3 = Hd.UIMessage!.Analog[2]!.InterChannelOffset;
            var offset4 = Hd.UIMessage!.Analog[3]!.InterChannelOffset;
            
            UInt64 procBdTrigCtrl_1st_PreDepth = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetProcBdTrigCtrl_PreDepth(xdepth, trigSource);
            UInt64 procBdTrigCtrl_1st_PreDepth1 = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetProcBdTrigCtrl_PreDepth(xdepth, trigSource);
            UInt64 procBdTrigCtrl_1st_PreDepth2 = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetProcBdTrigCtrl_PreDepth(xdepth, trigSource);
            UInt64 procBdTrigCtrl_1st_PreDepth3 = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetProcBdTrigCtrl_PreDepth(xdepth, trigSource);
            //            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ch_adjust_adjust_en, 0x0);
            if ((offset1 != 0) || (offset2 != 0) || (offset3 != 0) || (offset4 != 0))
            {
                //                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ch_adjust_adjust_en, 0x0);
                if ((offset1 != 0))
                {
                    procBdTrigCtrl_1st_PreDepth = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetProcBdTrigCtrl_PreDepth(xdepth, trigSource);
                    procBdTrigCtrl_1st_PreDepth = (UInt64)(procBdTrigCtrl_1st_PreDepth + (float)(offset1 / 4000000));
                }
                else if ((offset2 != 0))
                {
                    procBdTrigCtrl_1st_PreDepth1 = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetProcBdTrigCtrl_PreDepth(xdepth, trigSource);
                    procBdTrigCtrl_1st_PreDepth1 = (UInt64)(procBdTrigCtrl_1st_PreDepth1 + (float)(offset2 / 4000000));
                }
                else if ((offset3 != 0))
                {
                    procBdTrigCtrl_1st_PreDepth2 = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetProcBdTrigCtrl_PreDepth(xdepth, trigSource);
                    procBdTrigCtrl_1st_PreDepth2 = (UInt64)(procBdTrigCtrl_1st_PreDepth2 + (float)(offset3 / 4000000));
                }
                else if ((offset4 != 0))
                {
                    procBdTrigCtrl_1st_PreDepth3 = Hd.CurrProduct!.Acquirer_AnalogChannel!.GetProcBdTrigCtrl_PreDepth(xdepth, trigSource);
                    procBdTrigCtrl_1st_PreDepth3 = (UInt64)(procBdTrigCtrl_1st_PreDepth3 + (float)(offset4 / 4000000));
                }
            }
            #endregion


            UInt64 procBdReginterp_2st_PosDepth = GetProcBdInterp_2st_PreDepth(xdepth, trigSource);
        }
    }
}

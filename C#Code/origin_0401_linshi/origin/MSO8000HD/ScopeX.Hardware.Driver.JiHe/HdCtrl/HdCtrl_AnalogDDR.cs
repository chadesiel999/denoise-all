using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Driver.Module;
using ScopeX.Hardware.Driver.Registers.SendManage;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 模拟通道的DDR控制类，由硬件同学编写和调试
    /// </summary>
    internal static class HdCtrl_AnalogDDR
    {
        /// <summary>
        /// 配置DDR的写参数和使能
        /// </summary>
        /// <param name="writeParams"></param>
        /// <param name="AcquingParameters"></param>
        internal static void ConfigWrite(WriteParams writeParams, AcquireAttribute AcquingParameters)
        {
            //set 0 before acquiring
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
            //cij_0811
            ////临时调试PFC
            ///
            SubbandCtrlMethod ctrlmethod = Hd.UIMessage?.AiTable?[ChannelId.C1].RecfgDbi?.SubbandCtrlMethod ?? SubbandCtrlMethod.UserManual;
            if(ctrlmethod == SubbandCtrlMethod.BitWidthAdaptive && ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.BitWidth!=14)
            {
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TIADC_Enable, 0x0);
            }
            else
            {
                RegSendManager.Default.Send((UInt32)AcqBdReg.W.TIADC_Enable);

            }

             RegSendManager.Default.Send((UInt32)ProcBdReg.W.Dsp_DspEnPro);

            //     HdIO.WriteReg(ProcBdReg.W.Dsp_DspEnPro, 0x0);
            //触发设置
            ConditionManager.IsFromDDR = true;
            RegSendManager.Default.Send((UInt32)AcqBdReg.W.TrigCtrl_DigitalTrigEn);
            RegSendManager.Default.Send((UInt32)ProcBdReg.W.TrigCtrl_DigitalTrigEn_Pro);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, Hd.CurrDebugVarints.bEnable_InterBoardSynchronizationMode ? 1U : 0);//双板同步触发选择

            UInt32 ddrlength = (uint)(Math.Round((AcquingParameters.HardwareStorageWaveDotsCnt / 128.0), 0, MidpointRounding.ToPositiveInfinity)) * 128;// +256;
            //ddrlength = 4095*8;
            //ddrlength = 513920;
            //ddrlength += 6000;
            ChannelId trigsource = Hd.UIMessage?.Trigger?.Edge?.Source ?? ChannelId.C1;
            //if (trigsource == ChannelId.Ext || trigsource == ChannelId.Ext5)
            //    ddrlength = ddrlength+6700+0;//6700

            if (!Hd.UIMessage.bAcquireStopped)
            {
                ddrlength = (trigsource == ChannelId.Ext) ? (ddrlength + 6200) : ddrlength;//6700
                _PreTrigDeepth = ddrlength;
            }
            else
            {
                ddrlength = _PreTrigDeepth;
            }
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDR_DATA_Wr_Addr_Len_H16, (ddrlength >> 16) & 0xffff);//DDR存储深度
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDR_DATA_Wr_Addr_Len_L16, ddrlength & 0xffff);//DDR存储深度
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ChMode_SamplingMode, 0x0);//DDR存储深度
            //ddrlength = 50000;
            ddrlength = ddrlength * 2;

            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProDDR_DATA_Wr_Addr_Len_H16, (ddrlength >> 16) & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProDDR_DATA_Wr_Addr_Len_L16, ddrlength & 0xffff);

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Acq_NoiseControl, 1);

            # region 临时调试，触发；

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_SignDataDelayAdjust, 25);//触发延迟FIFO

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_TypeAcq, 0x00);//触发释译
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_DataAcqL, 0x01);//触发释译时间长度
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_HoldOff_DataAcqH, 0x00);//触发释译时间长度

            uint pretriglength = (uint)(AcquingParameters.HardwareStorageWaveDotsCnt / (AcquingParameters.AdcInterleaveMode == AdcInterleaveMode.Mode1To1 ? 1 : 2) + 2048);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PreDepthAddr_L, pretriglength & 0xffff);//预触发深度
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_PreDepthAddr_H, (pretriglength >> 16) & 0xffff);//预触发深度

            #endregion

            HdIO.WriteReg(ProcBdReg.W.LSCtrl_CurrFrameNoPro, writeParams.FrameNo);
            return;
        }
        private static UInt32 _PreTrigDeepth = 0;
        internal static Boolean WriteFinished()
        {
            uint flagvalue = 0;
            Stopwatch sw = Stopwatch.StartNew();
            flagvalue = HdIO.ReadReg(ProcBdReg.R.LSCtrl_FullFlag);
            while (flagvalue != 1 && sw.ElapsedMilliseconds < 1000)
            {
                flagvalue = HdIO.ReadReg(ProcBdReg.R.LSCtrl_FullFlag);
            }
            flagvalue = HdIO.ReadReg(ProcBdReg.R.LSCtrl_FullFlag);
            if (flagvalue == 1)
            {
                _NeedSoftwareFix = false;
            }
            //    var     flagvalue = HdIO.ReadReg(ProcBdReg.R.LSCtrl_FullFlag);
            //         return true;
            return flagvalue == 1;
         //            return true;
        }

        private static Boolean _NeedSoftwareFix = false;
        private static UInt32 _RuntimeDelay = 0;

        /// <summary>
        /// 配置DDR的读参数和使能
        /// </summary>
        /// <param name="readParams"></param>
        /// <param name="acquedParameters"></param>
        internal static void ConfigRead(ReadParams readParams, AcquireAttribute acquedParameters)
        {
            UInt32 SynIntdelayvalue = 0;
            var parms = Hd.CurrProduct?.Acquirer_AnalogChannel?.SyncParams();
            if (!Hd.UIMessage.bAcquireStopped)
            {
                if ((parms.Count() <= ChannelIdExt.AnaChnlNum) && ((Int32)Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource() < ChannelIdExt.AnaChnlNum))
                {
                    if (parms[(Int32)Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource()] != null)
                    {
                        SynIntdelayvalue = parms[(Int32)Hd.CurrProduct!.Ctrl_Trigger!.CurrentTrigSource()].DotsCnt;//整数延迟丢点
                        _RuntimeDelay = SynIntdelayvalue;
                    }
                }
                else
                {
                    SynIntdelayvalue = _RuntimeDelay;
                    //_RuntimeDelay = SynIntdelayvalue;
                }
            }
            else
            {
                SynIntdelayvalue = _RuntimeDelay;
            }
          
            #region 插值配置下发
            UInt32 Interpolate_Num_send = (UInt32)Math.Round(100.0 / (Int32)readParams.Interpolate_Num_Double);
            if (readParams.Interpolate_Num_Double > 1)
            //  if (readParams.TotalExtractNum == 1)
            {

                HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, Hd.CurrDebugVarints.bEnable_ChannelSync_Farrow==true? 1u : 0);//打开farrow使能 
                HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayEn, Hd.CurrDebugVarints.bEnable_ChannelSync == true ? 1u : 0);//打开farrow使能 



                ConditionManager.InterpEn = Hd.CurrDebugVarints.bEnable_ProbdInterpolation;

                RegSendManager.Default.Send((UInt32)ProcBdReg.W.Interpolate_InterpEnPro);

                //cij——0811                                                                                                       
                HdIO.WriteReg(ProcBdReg.W.Dsp_DspEnPro, Hd.CurrDebugVarints.bEnable_Dsp == true ? 1u : 0);//关闭处理板DSP使能 
                HdIO.WriteReg(ProcBdReg.W.Dsp_CaliEnPro, Hd.CurrDebugVarints.bEnable_Dsp_Pro == true ? 0xfu : 0);//关闭处理板DSP使能 
                SubbandCtrlMethod ctrlmethod = Hd.UIMessage?.AiTable?[ChannelId.C1].RecfgDbi?.SubbandCtrlMethod ?? SubbandCtrlMethod.UserManual;
                if (ctrlmethod == SubbandCtrlMethod.BitWidthAdaptive && ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.BitWidth != 14)
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TIADC_Enable,  0);
                }
                else 
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TIADC_Enable, Hd.CurrDebugVarints.bEnable_CorrectTiAdc == true ? 1u : 0);
                }
                   
                //cij——0811
                UInt32 Inter_discard_num = 10;// 20;
                UInt32 Inter_reverse_num = 3;//除丢点外，预留进插值前触发点位置
                //var delay = Hd.CurrDebugVarints.bEnable_ChannelSync_Farrow == true ? Inter_discard_num  + (UInt32)Math.Ceiling(SynIntdelayvalue / 2d)*2 + Inter_reverse_num : Inter_discard_num + Inter_reverse_num;
                var delay = Hd.CurrDebugVarints.bEnable_ChannelSync_Farrow == true ? Inter_discard_num + SynIntdelayvalue + Inter_reverse_num : Inter_discard_num + Inter_reverse_num;

                HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve27, delay);//插值前触发点位置
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Interpolate_Ratio, Interp_JiHe_MSO8000X.GetValideValue((Int32)readParams.Interpolate_Num_Double));//4Byte,最低Byte最多能发A(10倍率，只能2，5，A)，2位为0，34位最多20倍率（2，5，10，20）；
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Interpolate_RemaimderNun, (UInt32)readParams.Interpolate_DiscardDotNum);
                //HdIO.WriteReg(ProcBdReg.W.Interpolate_Ratio, Interp_JiHe_MSO8000X.GetValideValue((Int32)readParams.Interpolate_Num_Double));//4Byte,最低Byte最多能发A(10倍率，只能2，5，A)，2位为0，34位最多20倍率（2，5，10，20）；
                HdIO.WriteReg(ProcBdReg.W.Interpolate_Ratio, Interpolate_Num_send);//4Byte,最低Byte最多能发A(10倍率，只能2，5，A)，2位为0，34位最多20倍率（2，5，10，20）；
                //HdIO.WriteReg(ProcBdReg.W.Interpolate_Ratio, 50);//4Byte,最低Byte最多能发A(10倍率，只能2，5，A)，2位为0，34位最多20倍率（2，5，10，20）；
                HdIO.WriteReg(ProcBdReg.W.Interpolate_RemaimderNunPro, (UInt32)readParams.Interpolate_DiscardDotNum);

            }
            else  if (acquedParameters.ExtractNumFromAdc == 1)
            {

                HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, Hd.CurrDebugVarints.bEnable_ChannelSync_Farrow == true ? 1u : 0);//打开farrow使能 
                HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayEn, Hd.CurrDebugVarints.bEnable_ChannelSync == true ? 1u : 0);//打开farrow使能 
                ConditionManager.InterpEn = false;

                //if (Hd.Calibration.CaliStatus || Hd.Calibration.CaliStatus_offset || (Hd.CurrDebugVarints.bEnable_AdcDataDebugMode))
                //{
                //    ConditionManager.InterpEn = false;
                //}
                //else
                //{
                //    ConditionManager.InterpEn = true;
                //}
                RegSendManager.Default.Send((UInt32)AcqBdReg.W.Interpolate_EnableAcq);
                RegSendManager.Default.Send((UInt32)ProcBdReg.W.Interpolate_InterpEnPro);
                //HdIO.WriteReg(ProcBdReg.W.Interpolate_InterpEnPro, 0);

                //cij——0811                                                                                                       
                HdIO.WriteReg(ProcBdReg.W.Dsp_DspEnPro, Hd.CurrDebugVarints.bEnable_Dsp == true ? 1u : 0);//关闭处理板DSP使能 
                HdIO.WriteReg(ProcBdReg.W.Dsp_CaliEnPro, Hd.CurrDebugVarints.bEnable_Dsp_Pro == true ? 0xfu : 0);//关闭处理板DSP使能 
                SubbandCtrlMethod ctrlmethod = Hd.UIMessage?.AiTable?[ChannelId.C1].RecfgDbi?.SubbandCtrlMethod ?? SubbandCtrlMethod.UserManual;
                if (ctrlmethod == SubbandCtrlMethod.BitWidthAdaptive && ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.BitWidth != 14)
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TIADC_Enable, 0);
                }
                else
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TIADC_Enable, Hd.CurrDebugVarints.bEnable_CorrectTiAdc == true ? 1u : 0);
                }
                //cij——0811

                var delay = Hd.CurrDebugVarints.bEnable_ChannelSync_Farrow == true ? SynIntdelayvalue : 0;
                //发到dsp找点模块给进同步和插值前多留的点数，该模块在dsp和sync之间   其中  0：插值滤波器丢点    16：同步滤波器丢点   delayvalue：整数延时丢点
                //200: 进同步前多留的点数，200和{ChannelId.C1, -512-256-25-35-100},的100相关   2:将最后进插值触发点时的触发点放在第40+2的位置，不要放在最前面
                //ConditionManager.InterpEn = Hd.CurrDebugVarints.bEnable_ProbdInterpolation;

                HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve27, delay);//经过dsp后，触发点在第几个点
 //               Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Interpolate_Ratio, Interp_JiHe_MSO8000X.GetValideValue((Int32)readParams.Interpolate_Num_Double));//4Byte,最低Byte最多能发A(10倍率，只能2，5，A)，2位为0，34位最多20倍率（2，5，10，20）；
 //               Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Interpolate_RemaimderNun, (UInt32)readParams.Interpolate_DiscardDotNum);
                HdIO.WriteReg(ProcBdReg.W.Interpolate_Ratio, Interpolate_Num_send);//4Byte,最低Byte最多能发A(10倍率，只能2，5，A)，2位为0，34位最多20倍率（2，5，10，20）；
                //HdIO.WriteReg(ProcBdReg.W.Interpolate_Ratio,100);//4Byte,最低Byte最多能发A(10倍率，只能2，5，A)，2位为0，34位最多20倍率（2，5，10，20）；
                HdIO.WriteReg(ProcBdReg.W.Interpolate_RemaimderNunPro, (UInt32)readParams.Interpolate_DiscardDotNum);
            }
            else
            {
               HdIO.WriteReg(ProcBdReg.W.ChannelSync_FarrowFilterEn, 0);//关闭farrow使能 
               HdIO.WriteReg(ProcBdReg.W.ChannelSync_IntDelayEn, 0);//关闭farrow使能 
                                                                    //cij——0811
                HdIO.WriteReg(ProcBdReg.W.Dsp_DspEnPro, 0);//关闭处理板DSP使能 
                HdIO.WriteReg(ProcBdReg.W.Dsp_CaliEnPro, 0);//关闭处理板DSP使能 
                Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TIADC_Enable, 0);
                //cij——0811

                ConditionManager.InterpEn = false;
                //               RegSendManager.Default.Send((UInt32)AcqBdReg.W.Interpolate_EnableAcq);
                RegSendManager.Default.Send((UInt32)ProcBdReg.W.Interpolate_InterpEnPro);

                HdIO.WriteReg(ProcBdReg.W.reverse_reg_reserve27, 0);
            }
            #endregion 插值

            #region 后抽系数下发
            //此抽取数为DSO波形的抽取数，UPO是例外的专用寄存器
            HdMessage nowmessage = Acquisition.CurrDataAcquireAttribute.HdMessage!;// Acquisition.bReadOldData ? Acquisition.AcqedDataMsg! : Hd.UIMessage!;
            UInt32 totalextractnum = readParams.TotalExtractNum;
            (UInt32 Base, UInt32 Multiple) basemuliple = Extract_JiHe_MSO8000X.GetPostSeperateNum(totalextractnum);
            //Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.Decimation_PosGapx, basemuliple.Base);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PosGapValuelL16, basemuliple.Multiple & 0xffff);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PosGapValueH16, (basemuliple.Multiple >> 16) & 0xffff);
            ////强制打开峰值抽取
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PosDecimationMode, 1U);

            //UInt32 precoevalue = UInt32.MaxValue / basemuliple.Multiple;
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PosHrCoeH16, basemuliple.Multiple & 0xffff);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PosHrCoeH16, (basemuliple.Multiple >> 16) & 0xffff);

            //new to pro
            HdIO.WriteReg(ProcBdReg.W.Decimation_PosGapx, (basemuliple.Base)); //处理板后抽 zwj
            HdIO.WriteReg(ProcBdReg.W.Decimation_PosGapValuelL16, basemuliple.Multiple & 0xffff); //处理板后抽 zwj
            HdIO.WriteReg(ProcBdReg.W.Decimation_PosGapValueH16, (basemuliple.Multiple >> 16) & 0xffff); //处理板后抽 zwj

            HdIO.WriteReg(ProcBdReg.W.Decoder_B2SignalSourceL, 0x0001);//en 1:decimation  2:pk decimation  must 1 or 2
            HdIO.WriteReg(ProcBdReg.W.Decoder_B2SignalSourceM, 1);//L16 decimation gap 
            HdIO.WriteReg(ProcBdReg.W.Decoder_B2SignalSourceH,0);//H16 decimation gap

            //强制打开峰值抽取
            HdIO.WriteReg(ProcBdReg.W.Decimation_PosDecimationMode, 1U);
            //HdIO.WriteReg(ProcBdReg.W.Decimation_PosDecimationMode, 1U);
            HdIO.WriteReg(ProcBdReg.W.Decimation_PosHrCoeH16, basemuliple.Multiple & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.Decimation_PosHrCoeH16, (basemuliple.Multiple >> 16) & 0xffff);
            #endregion

            //读数据起始点配置
            UInt32 offset = (uint)(readParams.DdrReadStartDotPosition);
            //offset = 4000;
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_Offset_L, AcqBdReg.W.LSCtrl_Offset_H, offset);
            // HTF
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ProDdrOffsetLow, offset & 0xffff);
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ProDdrOffsetHigh, (offset>>16) & 0xffff);
            //END

            HdIO.Sleep(10);

            UInt32 lo_phase_ro = Hd.CurrProduct.AcqBd.ReadReg(AcqBdReg.R.TrigCtrl_RdTrigStatusAcqInterp, AcqBdNo.B1);

            HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiCntCap64Ch3, lo_phase_ro);

            return;
        }

        public static UInt32 DiscardDotCnt = 0;

        /// <summary>
        /// 在发完读参数后执行，读取PCIe缓存区的数据
        /// </summary>
        /// <param name="dataLength">想要读取的数据个数</param>
        /// <param name="dmaBuff">用来缓存数据的buff</param>
        /// <returns></returns>
        internal static Boolean ReadDMA(UInt32 dataLength, Byte[] dmaBuff)
        {
            
            Hd.CurrProduct!.AcqBd!.SwitchDataPathAndPcieReset_SetDataLength(DMAReadDataTypes.AnalogChannelDdr, dataLength);
            
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);

            //202500712 DDR DATA SAVE
            if (Hd.CurrDebugVarints.bEnable_DigitTrigger && Hd.CurrDebugVarints.bEnable_AcqDigitTrigger && _NeedSoftwareFix)
            {
                UInt32 read_discard_from_acq = Hd.CurrProduct.AcqBd.ReadReg(AcqBdReg.R.reverse_acq_reverse_rd_reg_1, AcqBdNo.B0) & 0xff;
                UInt32 read_discard_from_acq1 = Hd.CurrProduct.AcqBd.ReadReg(AcqBdReg.R.reverse_acq_reverse_rd_reg_1, AcqBdNo.B0) & 0xff;
                //Thread.Sleep(1);
                UInt32 read_discard_from_acq2 = Hd.CurrProduct.AcqBd.ReadReg(AcqBdReg.R.reverse_acq_reverse_rd_reg_1, AcqBdNo.B0) & 0xff;
                DiscardDotCnt = 0;// read_discard_from_acq2;
                HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_6, 0x8000 | read_discard_from_acq2);
                UInt32 state = HdIO.ReadReg(ProcBdReg.R.Acq_Pro_Main_State);
                //HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_6, 0);
            }
            else 
            {
                DiscardDotCnt = 0;
                HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_6, 0);
            }
            _NeedSoftwareFix = true;

            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x1);




            var retVal = HdIO.DMARead(dataLength, ref dmaBuff);
            HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
            //HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_6, 0x0);
            return retVal;
        }

        /// <summary>
        /// Mig复位
        /// </summary>
        internal static void MigReset()
        {  //cij_0604???
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_MigReset, 0);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_MigReset, 1);
            var flag = HdIO.CheckRegisterValue(AcqBdReg.R.LSCtrl_MigInitState, 0x1, 1, 100);
            if (!flag)
            {
                //throw new Exception("DDR MIG Init Failed!!");
            }
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.LSCtrl_DDRZoomProgFull, 0x01C);
        }
    }
}

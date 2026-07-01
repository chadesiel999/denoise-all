using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.MathExt;

namespace ScopeX.Hardware.Driver
{
    internal class PrecisionProcess
    {
        internal PrecisionProcess()
        { 
            
        }

        internal void Init()
        { 
            
        }

        internal void PropertyChanged()
        {
            Int32 bitwidth = Hd.UIMessage?.Precision?.AnaChnlBitWidth ?? 12;
            ConfigBitWidth(bitwidth, 3);
            AbstractController_Misc.ConfigExtractProcessRoadParameters();
        }

        private void ConfigBitWidth(Int32 bitWidth, Int32 subbandId = 0)
        {
            String chnlname = "C1_Lv500u";
            Int32 PassReserved = AiAnalogChannelParams.Default[chnlname].Reserved1;
            //Trace.WriteLine($"[PassReserved]chnlname({chnlname})");
            //Trace.WriteLine($"[PassReserved]Reserved1({PassReserved})");
            switch (bitWidth)
            {
                case 12:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ChMode_SamplingMode, 0x8180);
                    HdIO.WriteReg(ProcBdReg.W.DBI_DbiSampleMode, 0);
                    //HdIO.WriteReg(ProcBdReg.W.reverse_Write0, 1);
                    //CtrlAnalogChannel_DBI20G.ConfigSampleFreq(ChannelId.C1);
                    HdIO.WriteReg(ProcBdReg.W.DBI_TsPfcEn, 0x0);    //1248:muxsub1234    0:mux1234
                    //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_Interp2or4Select, 0);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh1, 0);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh2, 0);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh3, 0);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh4, 0);
                    Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x8000);  //拼1次
                    HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_0, 0x8000);//8 4 C
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueL16, 1);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueH16, 1);
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, 0);//0:normal 1:peak 3:hd

                    HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapX, 1);
                    HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueH16, 1);
                    break;
                case 13:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ChMode_SamplingMode, 0x4180);
                    HdIO.WriteReg(ProcBdReg.W.DBI_DbiSampleMode, 0);
                    CtrlAnalogChannel_DBI20G.ConfigSampleFreq(ChannelId.C1);
                    //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_Interp2or4Select, 1);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh1, 1);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh2, 1);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh3, 1);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh4, 1);
                    break;
                case 14:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ChMode_SamplingMode, 0x8180);
                    //HdIO.WriteReg(ProcBdReg.W.DBI_channel_en, 1);//1
                    //CtrlAnalogChannel_DBI20G.ConfigSampleFreq(ChannelId.C1, subbandId);
                    HdIO.WriteReg(ProcBdReg.W.DBI_TsPfcEn, (UInt32)0x1 << subbandId);    //1248:muxsub1234    0:mux1234
                    //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_Interp2or4Select, 0);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh1, 0);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh2, 0);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh3, 0);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh4, 0);
                    if(ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.LowFreqMode == true)
                    {
                        HdIO.WriteReg(ProcBdReg.W.DBI_DbiSampleMode, 3);
                        Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x8000);  //拼1次
                        HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_0, 0xC000);//8 4 C
                        //8抽
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueL16, 1);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueH16, 8);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, 3);//0:normal 1:peak 3:hd
                        HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapX, 8);
                        HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueH16, 1);
                        //HdIO.WriteReg(ProcBdReg.W.reverse_Write6, 5);
                    }
                    else
                    {
                        HdIO.WriteReg(ProcBdReg.W.DBI_DbiSampleMode, 1);
                        Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x8000);  //拼1次
                        HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_0, 0x8000);//8 4 C
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueL16, 1);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueH16, 1);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, 0);//0:normal 1:peak 3:hd
                        HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapX, 1);
                        HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueH16, 1);
                    }

                    break;
                case 15:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ChMode_SamplingMode, 0x4180);
                    //HdIO.WriteReg(ProcBdReg.W.DBI_channel_en, 1);
                    //CtrlAnalogChannel_DBI20G.ConfigSampleFreq(ChannelId.C1, subbandId);
                    HdIO.WriteReg(ProcBdReg.W.DBI_TsPfcEn, (UInt32)0x1 << subbandId);    //1248:muxsub1234    0:mux1234
                    //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_Interp2or4Select, 1);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh1, 1);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh2, 1);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh3, 1);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh4, 1);
                    if (ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.LowFreqMode == true)
                    {
                        HdIO.WriteReg(ProcBdReg.W.DBI_DbiSampleMode, 3);
                        Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x8000);  //拼1次
                        HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_0, 0xC000);//8 4 C
                        //16抽
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueL16, 1);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueH16, 16);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, 3);//0:normal 1:peak 3:hd
                        HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapX, 16);
                        HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueH16, 1);
                        //HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh1, 0);
                        //HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh2, 0);
                        //HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh3, 0);
                        //HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh4, 0);
                        //HdIO.WriteReg(ProcBdReg.W.reverse_Write6, 10);
                    }
                    else
                    {
                        HdIO.WriteReg(ProcBdReg.W.DBI_DbiSampleMode, 1);
                        Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x4000);  //拼1次
                        HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_0, 0x4000);//8 4 C
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueL16, 1);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueH16, 2);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, 0);//0:normal 1:peak 3:hd
                        HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapX, 2);
                        HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueH16, 1);
                    }
                    break;
                case 16:
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ChMode_SamplingMode, 0xc180);
                    //CtrlAnalogChannel_DBI20G.ConfigSampleFreq(ChannelId.C1, subbandId);
                    HdIO.WriteReg(ProcBdReg.W.DBI_TsPfcEn, (UInt32)0x1<< subbandId);    //1248:muxsub1234    0:mux1234
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh1, 2);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh2, 2);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh3, 2);
                    HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh4, 2);
					 if (ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.LowFreqMode == true)
                    {
                        HdIO.WriteReg(ProcBdReg.W.DBI_DbiSampleMode, 3);
                        Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0x8000);  //拼1次
                        HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_0, 0xC000);//8 4 C
                        //32抽
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueL16, 1);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueH16, 32);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, 3);//0:normal 1:peak 3:hd
                        HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapX, 32);
                        HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueH16, 1);
                        //HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh1, 0);
                        //HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh2, 0);
                        //HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh3, 0);
                        //HdIO.WriteReg(ProcBdReg.W.DBI_Interp2or4SelectProCh4, 0);
                        //HdIO.WriteReg(ProcBdReg.W.reverse_Write6, 20);
                    }
                    else
                    {
                        HdIO.WriteReg(ProcBdReg.W.DBI_DbiSampleMode, 1);
                        Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.reverse_acq_reverse_wr_reg_1, 0xC000);  //拼2次
                        HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_0, 0xC000);//8 4 C
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueL16, 1);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueH16, 4);
                        Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, 0);//0:normal 1:peak 3:hd
                        HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapX, 4);
                        HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueH16, 1);
                    }
                    break;
                default:
                    break;
            }
        }

        record PhaseGainError(Double phaseByfs, Double gain);
        record CtrlWordAndPhase(Int32 CtrlWord, Double PhaseError);

        private SubbandCtrlMethod _LastSubbandCtrlMethod = SubbandCtrlMethod.UserManual;

        private Int32 _ExcuteCnt = 0;

        private Int32 _Bitwidth = 0;
        private Int32 _Subbandid = 0;

        internal void AutoCfg(ChannelId chnlId, SubbandCtrlMethod ctrlmethod)
        {
            ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.ReadSubbandInfo();
            if (_ExcuteCnt < 20)
            {
                _ExcuteCnt++;
                return;
            }
            _ExcuteCnt = 0;
            //下面这段注释掉后不会重新算频率
            ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.UpdateSubbandInfo();
            if (ctrlmethod == SubbandCtrlMethod.BitWidthAdaptive)
            {
                _IsDefaultState = false;
                if (ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.CheckSigChanged() || _LastSubbandCtrlMethod != SubbandCtrlMethod.BitWidthAdaptive)
                {
                    ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.InitParams();
                    ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.ConfigLast_CurSigFreqParams();
                    ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.ConfigDefaultDBI();
                    _LastSubbandCtrlMethod = SubbandCtrlMethod.BitWidthAdaptive;
                    ConfigBitWidth(ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.BitWidth, ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.SubbandId);
                    Acquisition.CreateAcquireAttribute();
                    AbstractController_Misc.ConfigExtractProcessRoadParameters();
                    return;
                }
                ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.ConfigLast_CurSigFreqParams();
                if (ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.ConfigSubbandEnabel(chnlId))
                {
                    return;
                }

                ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.ConfigCurDBI();
                Int32 bitwidth = ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.BitWidth;
                Int32 subbandid = ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.SubbandId;
                ConfigBitWidth(bitwidth, subbandid);
                if (bitwidth != _Bitwidth || subbandid != _Subbandid)
                {
                    Acquisition.CreateAcquireAttribute();
                    AbstractController_Misc.ConfigExtractProcessRoadParameters();
                    _Bitwidth = bitwidth;
                    _Subbandid = subbandid;
                }
            }
        }

        Boolean _IsDefaultState = true;

        internal void Run()
        {
            Boolean autots = Hd.UIMessage?.Precision?.AutoCfgBitWidthEnable ?? false;

            SubbandCtrlMethod ctrlmethod = Hd.UIMessage?.AiTable?[ChannelId.C1].RecfgDbi?.SubbandCtrlMethod ?? SubbandCtrlMethod.UserManual;

            AutoCfg(ChannelId.C1, ctrlmethod);
                
            if (ctrlmethod != SubbandCtrlMethod.BitWidthAdaptive)
            {
                if (!_IsDefaultState)
                {
                    ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.ConfigDefaultDBI();
                    AbstractController_Misc.ConfigExtractProcessRoadParameters();
                    _IsDefaultState = true;
                }


                _LastSubbandCtrlMethod = ctrlmethod;

                List<List<UInt16>> adcdata = new();
                Int32 chnlid = 0;
                Int32 adccnt = 2;
                Int32 dotcnt = 5000;

                Double inputSignalFreqByMhz = 100;
                Double sampFreqByMHz = 20e3;

                SinFitResult[] ans = new SinFitResult[adccnt];

                for (Int32 adcid = 0; adcid < adccnt; adcid++)
                {
                    List<UInt16> tmpdata = new();
                    for (Int32 dotid = 0; dotid < dotcnt; dotid++)
                    {
                        Int32 dotsid = dotid * adccnt + adcid;
                        if (dotsid < AcqedDataPool.AnalogChData.AllChannelData[chnlid].Count)
                        {
                            //       AcqedDataPool.AnalogChData.AllChannelData[chnlid][dotsid] >>= 4;
                            tmpdata.Add(AcqedDataPool.AnalogChData.AllChannelData[chnlid][dotsid]);

                        }
                    }

                    if (tmpdata.Count == 0)
                        continue;

                    ans[adcid] = SinFitClass.SinFit(tmpdata.Select(o => (Double)(o)).ToArray(), inputSignalFreqByMhz, sampFreqByMHz / adccnt) ?? new SinFitResult(0, 0, 0);

                    if (adcid + 1 == adccnt)
                    {
                        Double phaseTmp = (ans[adcid].Phase + Math.PI * 2 - ans[0].Phase) % (Math.PI * 2);
                        Double gainTmp = ans[adcid].Gain / ans[0].Gain;
                        Double offsetTmp = ans[adcid].Offset - ans[0].Offset;

                        Double phaseByps = phaseTmp * 1000_000 / inputSignalFreqByMhz / (2 * Math.PI);

                        if (phaseByps > 1000_000 / inputSignalFreqByMhz / 2)
                            phaseByps -= 1000_000 / inputSignalFreqByMhz;
                        else if (phaseByps < -1000_000 / inputSignalFreqByMhz / 2)
                            phaseByps += 1000_000 / inputSignalFreqByMhz;

                        // Trace.WriteLine($"*************** phaseByfs = {phaseByps.ToString("0.000")}ps, gainTmp = {gainTmp.ToString("0.000")}, offsetTmp = {offsetTmp.ToString("0.000")}***********");
                    }

                }
            }
        }
    }
}

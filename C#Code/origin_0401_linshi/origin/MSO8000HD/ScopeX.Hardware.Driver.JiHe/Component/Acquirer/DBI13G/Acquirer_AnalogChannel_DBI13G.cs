using MathWorks.MATLAB.NET.Arrays;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Driver.Registers.SendManage;
using ScopeX.Hardware.Driver.PlatForm;

namespace ScopeX.Hardware.Driver
{
    public partial class Acquirer_AnalogChanel_DBI13G : AbstractAcquirer_AnalogChannel
    {
        internal Acquirer_AnalogChanel_DBI13G() : base() 
        {
            this.AnalogAcquireModel=new AnalogAcquireModel_DBI13G_AcqBd6();
            _LongStorageConfig?.Invoke(8, 512 * 1024 * 1024, 1024 * 1024, 1024 * 1024);
            _StorageDotsCntPerCore = new int[7]
            {
                2_500,
                3_125,
                31_250,
                312_500,
                3_125_000,
                31_250_000,
                62_500_000,
            };
            DmaBytesPerDotForAllChannel = 8.0;

            InitTiadcInfo();
        }
        private SyncParams[] _SyncParams = new SyncParams[ChannelIdExt.AnaChnlNum];
        private const Int32 _BaseNoiseSampleWindow = 5;
        private const Int32 _BaseNoiseTrueThreshold = 4;
        private Int32 _BaseNoiseSampleCount = 0;
        private readonly Dictionary<Int32, Int32> _BaseNoiseTrueCntTable = new();
        private readonly HashSet<Int32> _BaseNoiseSubbandIds = new();

        internal override SyncParams[] SyncParams()
        {
            return _SyncParams;
        }
        internal override void Init()
        {
            bFirstTimes = true;
            _FirstActive = true;
            AcquingParameters.OldChBWModeAndActiveState = 0;
            _BaseNoiseSampleCount = 0;
            _BaseNoiseTrueCntTable.Clear();
            _BaseNoiseSubbandIds.Clear();
            for (Int32 channelId = 0; channelId < ChannelIdExt.AnaChnlNum; channelId++)
            {
                lastChannelYScaleIndex.Add(-1);
            }

            InitPhyAnalogChAmplitudeTemperaturesCompensationCoefficient();
            _LongStorageInit?.Invoke();
            InitAmpCoefficientFile();
        }

        private void InitTiadcInfo()
        {
            #region Test Code
            TiadcPhaseOffsetGainParams.Default["111222_333"] = new TiadcPhaseOffsetGainItem_Base() { Offset = 105, Offset_FPGA = 200 };
            TiadcPhaseOffsetGainParams.Default["aabb"] = new TiadcPhaseOffsetGainItem_Base() { Phase = 105, Phase_FPGA = 200 };
            TiadcPhaseOffsetGainParams.Default["c1_adc1_core2"] = new TiadcPhaseOffsetGainItem_Base() { Gain = 105, Gain_FPGA = 200 };
            TiadcPhaseOffsetGainParams.Default["merge_c2_adc3_core4"] = new TiadcPhaseOffsetGainItem_Base() { Reserved0 = 105, Reserved1 = 200 };
            #endregion
        }
        internal override void ConfigExtractProcessRoadParameters()
        {
            UInt32 acqmode = (Hd.UIMessage?.Timebase?.AcqMode ?? AnaChnlAcqMode.Normal) switch
            {
                AnaChnlAcqMode.Peak => 1,
                AnaChnlAcqMode.HighRes => 3,
                _ => 0,
            };

            Hd.CurrProduct?.S6Bd?.SetPll10MSyncClk(Hd.UIMessage!.Timebase!.ClockSrc);

            /*前抽*/
            UInt64 extracttotalnum = (UInt64)AcquingParameters.ExtractNumFromAdc;
            ConditionManager.IsExtractEn = extracttotalnum > 1;
            Int32 scale = Hd.UIMessage?.Timebase?.TmbScaleIndex ?? 17;

            //(UInt32 H16, UInt32 L16) extractsplitnum = GetPreSeperateNum((AnaChnlTimebaseIndex)scale);
            /*2N抽取适配*/
            Dictionary<String, object> addition = new Dictionary<String, object>();
            addition.Add(nameof(DdrData4What), DdrData4What.Dso);
            addition.Add(nameof(AdcInterleaveMode), AcquingParameters.AdcInterleaveMode);
            (UInt32 H16, UInt32 L16) extractsplitnum = PlatFormManager.CurrPlatForm.GetPreSeperateNum(extracttotalnum, addition);

            ////Gap值下发需要，除法器转换乘法器
            var precoevalue = UInt32.MaxValue / extractsplitnum.L16;
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_HrCoeL16, (UInt32)(precoevalue & 0xffff));
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_HrCoeH16, (UInt32)((precoevalue >> 16) & 0xffff));

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapX, extractsplitnum.Base);/*extractsplitnum.Base/2*/

            var aimsg = Hd.UIMessage?.AiTable;
            if (aimsg != null)
            {
                var chnlid = ChannelId.C1;
                if (aimsg[chnlid].RecfgDbi?.SubbandCtrlMethod != SubbandCtrlMethod.BitWidthAdaptive)
                {
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueL16, (UInt32)(extractsplitnum.L16));
                    Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueH16, (UInt32)(extractsplitnum.H16));
                    HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapX, (UInt32)(extractsplitnum.H16));
                    HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueH16, (UInt32)(extractsplitnum.L16));
                }
            }
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapX, (UInt32)(extractsplitnum.H16));
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueH16, (UInt32)(extractsplitnum.L16));

            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapX, extractsplitnum.Base);
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueL16, (UInt32)(extractsplitnum.Multiple & 0xffff));
            //HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueH16, (UInt32)((extractsplitnum.Multiple >> 16) & 0xffff));

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_L16, (UInt32)(extracttotalnum & 0xffff));
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_M16, (UInt32)((extracttotalnum >> 16) & 0xffff));
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_Decimation_H16, (UInt32)(extracttotalnum >> 32) & 0xffff);
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_DecimationMode, acqmode & 0xffff);

            /*扫描档前抽L3配置,生效前提是L1*L2=100*/
            //Gap值下发需要，除法器转换乘法器
            Int64 l3extract = AcquingParameters.Scan2ExtractNum_Total;
            if (((UInt64)AcquingParameters.Scan2ExtractNum_Total * AcquingParameters.ExtractNumFromAdc) >= 100 &&
                AcquingParameters.ExtractNumFromAdc < 100)
            {
                l3extract = (Int64)(AcquingParameters.Scan2ExtractNum_Total / (100D / AcquingParameters.ExtractNumFromAdc));
            }
            var scanCoeValue = UInt32.MaxValue / l3extract;
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ScanCtrl_HrCoe4ScanL16, (UInt32)(scanCoeValue & 0xffff));
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.ScanCtrl_HrCoe4ScanH16, (UInt32)((scanCoeValue >> 16) & 0xffff));

            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.ScanCtrl_ScanPreGapValueL16, (UInt32)l3extract & 0xffff);
            Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.ScanCtrl_ScanPreGapValueH16, (UInt32)(l3extract >> 16) & 0xffff);

            ////以下是长存储的有关配置
            //Hd.CurrProduct!.AcqBd!.ExecMiscFunc("SendChMode_SamplingMode");//??

            //只有抽取档，关闭Afc; 10G模式也关闭Afc;
            var interdefine = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;

            if (interdefine.InterleaveMode == AdcInterleaveMode.Mode1To1 || AcquingParameters.ExtractNumFromAdc > 1)
                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.Afc_AfcFilterEn, 0);
            else
                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.Afc_AfcFilterEn, 1);
        }
        public static (UInt32, UInt32) GetPreSeperateNum(AnaChnlTimebaseIndex scale)
        {
            if (_PreExtractTable.ContainsKey(scale))
            {
                return _PreExtractTable[scale];
            }
            return (1,1);
        }
        private static Dictionary<AnaChnlTimebaseIndex,( UInt32, UInt32)> _PreExtractTable = new()
        { 
            {AnaChnlTimebaseIndex.Lv1u,(2,             1)},
            {AnaChnlTimebaseIndex.Lv2u,(4,             1)},
            {AnaChnlTimebaseIndex.Lv5u,(8,             2)},
            {AnaChnlTimebaseIndex.Lv10u,(16,            2)},
            {AnaChnlTimebaseIndex.Lv20u,(32,            2)},
            {AnaChnlTimebaseIndex.Lv50u,(64,            2)},
            {AnaChnlTimebaseIndex.Lv100u,(64,            5)},
            {AnaChnlTimebaseIndex.Lv200u,(64,            10)},
            {AnaChnlTimebaseIndex.Lv500u,(64,            20)},
            {AnaChnlTimebaseIndex.Lv1m,(64,            50)},
            {AnaChnlTimebaseIndex.Lv2m,(64,            100)},
            {AnaChnlTimebaseIndex.Lv5m,(64,            200)},
            {AnaChnlTimebaseIndex.Lv10,(64,            500)},
        };

        /// <summary>
        /// 没看懂
        /// </summary>
        /// <param name="channelIndex"></param>
        /// <returns></returns>
        internal override ChannelBdAdcInputDefine? GetChannelAcqBdAdcInputCorresponding(int channelIndex)
        {
            int[] channelAcqBdCorresponding = new int[] { 0, 3, 4, 7 };
            return new ChannelBdAdcInputDefine() { BdNo = (AcqBdNo)channelAcqBdCorresponding[channelIndex], AdcIndex = 0, bIs20GMode = true, InputPort_AIs1 = 2 };
        }

        /// <summary>
        /// DBI模式下的原始采样率：每个子带20GSPS（两片10GSPS的ADC），3个子带拼合
        /// </summary>
        private const Double _DbiModeSampleInterval_us = 1.0 / 80.0 / 1000.0;

        /// <summary>
        /// 非DBI模式下的原始采样率：单子带20GSPS
        /// </summary>
        private const Double _SampleInterval_us = 1.0 / 20.0 / 1000.0;
        internal void CreateAcquireAttribute2()
        {
            AcquingParameters.AcqStorageMode = Hd.UIMessage?.Timebase?.AcqLength ?? AnaChnlStorageMode.Normal;

            AcquingParameters.CurrChBWModeAndActiveState = 0;//没有高采样模式
            for (Int32 channelid = (Int32)ChannelId.C1; channelid < ChannelIdExt.AnaChnlNum; channelid++)
            {
                if (Hd.UIMessage!.Analog![channelid].Active)
                    AcquingParameters.CurrChBWModeAndActiveState |= (1 << channelid);
            }

            if (Hd.UIMessage != null)
                AcquingParameters.HdMessage = Hd.UIMessage with { };

            AcquingParameters.AdcInterleaveMode = AnalogAcquireModel?.GetCurrentAcqModeInterleave()?.InterleaveMode ?? AdcInterleaveMode.Mode1To1;
            AcquingParameters.CurrChBWModeAndActiveState |= (Int32)AcquingParameters.AdcInterleaveMode << 16;

            AcquingParameters.HardwareStorageWaveDotsCnt = Hd.UIMessage?.Timebase?.StorageWaveDotsCnt ?? -1;
            //适配处理界面状态与driver状态不一致的情况；
            if (AcquingParameters.AdcInterleaveMode != Hd.UIMessage?.Timebase?.InterleaveMode)
            {
                if (AcquingParameters.AdcInterleaveMode == AdcInterleaveMode.Mode1To1)
                    AcquingParameters.HardwareStorageWaveDotsCnt /= 2;
                else
                    AcquingParameters.HardwareStorageWaveDotsCnt *= 2;
            }

            AcquingParameters.SettingTrigPositionByfs = (Hd.UIMessage?.Timebase?.TmbPosition ?? 0) * uS2fs;
            CalcExtramInterplotNum(AcquingParameters);

            //抽取计算
            (Double AdcSampleIntervalUs, Int32 UIPoints) sampleinfo = AcquingParameters.AdcInterleaveMode switch
            {
                AdcInterleaveMode.Mode2To1 => (25E-6, 100_000),
                _ => (50E-6, 50_000),//Mode1To1
            };
            AcquingParameters.ExtractNumFromAdc = CalcPreExtractNum(sampleinfo.AdcSampleIntervalUs);

            /*scan档抽取配置*/
            AcquingParameters.Scan2ExtractNum_Total = Math.Max(AcquingParameters.HardwareStorageWaveDotsCnt / sampleinfo.UIPoints, 1);
            AcquingParameters.Scan2ExtractNum_Base = 1;
            AcquingParameters.Scan2ExtractNum_Multiple = 1;

            //       acquireAttribute.PerDataByfs_AtDdr_pre =
            AcquingParameters.PerDataByfs_AtDdr = sampleinfo.AdcSampleIntervalUs * AcquingParameters.ExtractNumFromAdc * uS2fs;

            //抽取数适配：DDR使用后抽，Fifo使用L3的前抽
            UInt64 extractnum = AcquingParameters.ExtramNumToDMA;
            if (Hd.UIMessage!.Timebase!.IsScan && (!Hd.UIMessage.bAcquireStopped))
                extractnum = (UInt64)AcquingParameters.Scan2ExtractNum_Total;
            AcquingParameters.PerDataByfs_AtDMA = AcquingParameters.PerDataByfs_AtDdr * extractnum / AcquingParameters.InterplotNumToDMA;
             
            FrameNo_AcquireAttribute_Push(AcquingParameters);
            if (!Hd.UIMessage.bAcquireStopped)
            {
                Acquisition.CurrDataAcquireAttribute = FrameNo_AcquireAttribute_Get(AcquingParameters.FrameNo);
                //Debug.WriteLine($"at CreateAcquireAttribute CurrDataAcquireAttribute={Acquisition.CurrDataAcquireAttribute.FrameNo}");

                ///*ext_trig Reset*///
                //HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_1, 0x0);
                //HdIO.Sleep(1);
                //HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_1, 0xf);
                //HdIO.Sleep(1);
                //HdIO.WriteReg(ProcBdReg.W.reverse_pro_reverse_wr_reg_1, 0x0);

            }
        }
        /// <summary>
        /// 计算前抽数
        /// </summary>
        /// <param name="sampleIntervalInAdcByUs"></param>
        /// <returns></returns>
        private UInt64 CalcPreExtractNum(Double sampleIntervalInAdcByUs)
        {
            Double needwavesumtimebyus = Hd.UIMessage!.Timebase!.TmbScale * Constants.VIS_XDIVS_NUM;
            var minstoragetimebyus = sampleIntervalInAdcByUs * AcquingParameters.HardwareStorageWaveDotsCnt;
            var storagetimebyus = Math.Max(needwavesumtimebyus, minstoragetimebyus);

            var preextramnum = Math.Ceiling(storagetimebyus / (sampleIntervalInAdcByUs * AcquingParameters.HardwareStorageWaveDotsCnt));

            //Extract_JiHe_MSO8000X.GetValidPreExtractNum((UInt64)preextramnum);

            //Int32 scale = Hd.UIMessage?.Timebase?.TmbScaleIndex ?? 17;
            //(UInt32 H16, UInt32 L16) extractsplitnum = GetPreSeperateNum((AnaChnlTimebaseIndex)scale);
            //UInt64 value =(UInt64)( extractsplitnum.L16 * extractsplitnum.H16);
            //return value;

            /*2N抽取适配*/
            Dictionary<String, object> addition = new Dictionary<String, object>();
            var interleave = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;
            addition.Add(nameof(AdcInterleaveMode), interleave.InterleaveMode);
            return PlatFormManager.CurrPlatForm.GetValidPreExtractNum((UInt64)preextramnum, addition);
        }
        protected override Double GetAdcSampleIntervalByUs()
        {
		    Int64 storageDotsCnt = 10240;
		    bool bFast = Hd.UIMessage!.Display!.IsFast;
            Int32 ActiveChnlCnt = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.ActiveChnlCnt;
            if (bFast && Hd.UIMessage?.Timebase?.TmbScaleIndex == (int)AnaChnlTimebaseIndex.Lv5n && (UInt32)ActiveChnlCnt == 1)
                storageDotsCnt = 1000; //LXT 250723 5ns/div下80GSPS转为20GSPS，wby
            Double sumTimeByUs = (AcquingParameters.HdMessage?.Timebase?.TmbScale ?? 1e-3) * Constants.VIS_XDIVS_NUM;

            if (storageDotsCnt * _DbiModeSampleInterval_us >= sumTimeByUs)
                return _DbiModeSampleInterval_us;

            return _SampleInterval_us;
        }

        /// <summary>
        /// 支持的DMA读取数据量
        /// </summary>
        private UInt32[] _DmaBytesCnt = 
        {
            120 * 1024 
        };

        private UInt32 GetValidDmaBytesCnt(UInt32 expectedDmaBytesCnt)
        {
            if (expectedDmaBytesCnt <= _DmaBytesCnt.Min())
                return _DmaBytesCnt.Min();

            for (Int32 index = 0; index < _DmaBytesCnt.Length - 1; index++)
            {
                if (expectedDmaBytesCnt > _DmaBytesCnt[index] && expectedDmaBytesCnt <= _DmaBytesCnt[index + 1])
                {
                    return _DmaBytesCnt[index + 1];
                }
            }

            return _DmaBytesCnt.Max();
        }

        private Boolean _ChnlActiveChanged = false;
        private Boolean _AverageEnable = false;

        internal override void CreateAcquireAttribute()
        {
            base.CreateAcquireAttribute();
            CreateAcquireAttribute2();
            FrameNo_AcquireAttribute_Push(AcquingParameters);
            UInt32 newchnlactive = Hd.CurrProduct?.AnalogAcquireModel?.GetActuallActiveState(AcquingParameters.ChnlActiveState) ?? 0x5;
            if (Hd.CurrDebugVarints.bEnable_AdcDataDebugMode)
            {
                newchnlactive = Hd.CurrProduct?.AnalogAcquireModel?.GetActuallActiveState(1u << (int)Hd.CurrDebugVarints.iDbi_DebugChannelID) ?? 0x5;
            }
            if (DbiActuallActiveState != newchnlactive)
            {
                DbiActuallActiveState = newchnlactive;
                _ChnlActiveChanged = true;
            }
            else
            {
                _ChnlActiveChanged = false;
            }

            AcquingParameters.DmaBytsCnt = GetValidDmaBytesCnt(AcquingParameters.DmaReadDotsCnt * 8);
            if (GetAdcSampleIntervalByUs() == _DbiModeSampleInterval_us)
            {
                AcquingParameters.InterplotNumFromADC = 3;
            }
            else
            { 
                AcquingParameters.InterplotNumFromADC = 1;
                DbiActuallActiveState = 0xf;
            }

            if (AcquingParameters.PerDataByfs_AtDMA != AcquedParameters.PerDataByfs_AtDMA)
            {
                _ChnlActiveChanged = true;
            }

            CheckCoefficientSend();

            CheckAmpleCoefficientSend();
        }

        internal static UInt32 DbiActuallActiveState = 0;

        protected override void SpecialConfig()//????
        {
            //      CtrlAnalogChannel_DBI20G.Default.ConfigSampleFreq(ChannelId.C1, 1);

            // Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TIADC_Enable, Hd.CurrDebugVarints[DebugBooleanEnum.bEnable_CorrectTiAdc] ? 1U : 0);
            //HdIO.WriteReg(ProcBdReg.W.DBI_SubDataDebugNum, 0);//暂时写死，只要通道1
            //HdIO.WriteReg(ProcBdReg.W.DBI_PfcCaliEn, (Hd.CurrDebugVarints?.bEnable_Dbi_PhaseFreqCoef ?? true) ? 1u : 0);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_ctrl, 0b00011);
            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.SenceFFT_sence_fft_ctrl, 0b01100);
            //    HTF_DEBUG_TMP
            Double curSampleInterval = GetAdcSampleIntervalByUs();
            if (curSampleInterval == _DbiModeSampleInterval_us)
            {
                if (Hd.CurrDebugVarints.bEnable_AdcDataDebugMode)
                {
                    ConfigSubBandMode((ChannelId)Hd.CurrDebugVarints.iDbi_DebugChannelID);
                    //HdIO.WriteReg(ProcBdReg.W.DataPath_ChannelModePro, (DbiActuallActiveState << 8) | 0b1000_0000);
                    //HdIO.WriteReg(ProcBdReg.W.Average_Enable, 0);
                }
                else
                {
                    //if (Hd.CurrProduct?.AnalogAcquireModel?.GetDbiMergeState(DbiActuallActiveState) ?? false)
                    if(Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode)
                    {
                        ConfigDbiMode();  //拼合
                        //HdIO.WriteReg(ProcBdReg.W.DataPath_ChannelModePro, (DbiActuallActiveState << 8) | 0b1000_0000);
                        //HdCtrl_Extram.ConfigAverageCnt(2);
                    }
                    else
                    {
                        ConfigFakeDbiMode();
                       // HdIO.WriteReg(ProcBdReg.W.DataPath_ChannelModePro, (DbiActuallActiveState << 8) | 0b0100_0000);
                        //HdCtrl_Extram.ConfigAverageCnt(4);
                         //HdIO.WriteReg(ProcBdReg.W.Average_Enable, 0);
                    }

                    //HdIO.WriteReg(ProcBdReg.W.DataPath_ChannelModePro, 0x40);
                }
            }
            else
            {
                ConfigSingBandMode();
                //HdIO.WriteReg(ProcBdReg.W.DataPath_ChannelModePro, 0x0F40);
                //HdIO.WriteReg(ProcBdReg.W.Average_Enable, 0);
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_init_delay_en, 0);
                HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh1, 0);
                HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh2, 0);
                HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh3, 0);
                HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh4, 0);

                Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.TIADC_Enable, 0);
                //HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh1, 0);
                //HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh2, 0);
                //HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh3, 0);
                //HdIO.WriteReg(ProcBdReg.W.TIADC_EnableProCh4, 0);

                HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiModuleEn, 0b1000);
            }
            
           ConfigDbiMode();

            Dictionary<AcqBdNo, UInt32> TrigCtrl_TestDataModeValue = new()
            {
                {AcqBdNo.B1,  0},
                {AcqBdNo.B2,  0},
                {AcqBdNo.B3,  0},
                {AcqBdNo.B4,  0},
                {AcqBdNo.B5,  0},
                {AcqBdNo.B6,  0},
                {AcqBdNo.B7,  0},
                {AcqBdNo.B8,  0},
                {AcqBdNo.B9,  0},
                {AcqBdNo.B10, 0},
                {AcqBdNo.B11, 0},
                {AcqBdNo.B12, 0},
            };

            foreach (var testmode in TrigCtrl_TestDataModeValue)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_TestDataMode, testmode.Key, testmode.Value);//testmode选择
            }

            var adcusedinfo = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(DbiActuallActiveState, (ChannelId)(Hd.CurrProduct?.Ctrl_Trigger?.CurrentTrigSource()), 0);
            if (adcusedinfo != null)
            {
                //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CurrentBoardSel, 0x0);
                //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.TrigCtrl_CurrentBoardSel, adcusedinfo.AcqBdNo, 0);
            }

            UInt32 debugSingleMode = 0;
            foreach (AcqBdNo acqBdNo in Enum.GetValues<AcqBdNo>())
            {
                if (!(Hd.CurrProduct?.AcqBd?.PowerOkBoardList?.Contains(acqBdNo) ?? false) && acqBdNo >= AcqBdNo.B7)
                {
                    debugSingleMode |= 0x1u << (Int32)(acqBdNo - AcqBdNo.B7);// B7在FPGA中为B1
                }
            }
            //if (debugSingleMode != 0)
            //{
            //    debugSingleMode |= 0x1u << 15;
            //}
            ConfigAverage();
            if ((Hd.UIMessage?.AiTable?[ChannelId.C1].AIUnion?.AverageEnable ?? false) != _AverageEnable)
            {
                ConfigAverage();
                HdIO.WriteReg(ProcBdReg.W.Average_Enable, (uint)((Hd.UIMessage?.AiTable?[ChannelId.C1].AIUnion?.AverageEnable ?? false) ? 1 : 0));
                _AverageEnable = Hd.UIMessage?.AiTable?[ChannelId.C1].AIUnion?.AverageEnable ?? false;
            }


            //HdIO.WriteReg(ProcBdReg.W.Debug_SingleModePro, debugSingleMode);
            //HdIO.WriteReg(ProcBdReg.W.Debug_SingleModePro, 0b0000_1111_1100_0110);   1027hcj
            //HdIO.WriteReg(ProcBdReg.W.Debug_SingleModePro, 0b0000_1111_1100_1110);

            //HdIO.WriteReg(ProcBdReg.W.Debug_SingleModePro, 0b_000_000);
        }

        internal static void ConfigAverage()
        {
            HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude0, (UInt32)1000);
            HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude0Cnt, (UInt32)3000);
            HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude1, (UInt32)1000);
            HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude1Cnt, (UInt32)3000);
            HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude2, (UInt32)(4 << 12) + 2048);
            HdIO.WriteReg(ProcBdReg.W.Average_SingalAmplitude2Cnt, (UInt32)1000);
            HdIO.WriteReg(ProcBdReg.W.Average_Number, (UInt32)4);
            HdIO.WriteReg(ProcBdReg.W.Average_AddrInit, 0);
            HdIO.WriteReg(ProcBdReg.W.Average_AddrRegion, 16000);
            HdIO.WriteReg(ProcBdReg.W.Average_average_addr_over_dly_num, 30);
        }

    /// <summary>
    /// 只有进入到实时档（DBI拼合模式）后，子带模式和拼合模式才有可能生效，抽取档固定工作在单子带模式
    /// </summary>
    internal override void InitAcq()
        {
            //CtrlAnalogChannel_DBI20G.ConfigInnerSignalSource(0000, ChannelId.C3);
            HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x1);
            HdIO.WriteReg(PcieBdReg.W.RST_CTRL_PcieReset, 0x0);
      //      Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_Interp2or4Select, 0x1);
            SpecialConfig();
            // HdIO.WriteReg(ProcBdReg.W.Debug_SingleModePro, 0b_1000_1111_1100_0000);
            TiadcPhaseOffsetGainParams.Default["merge_c4_adc3_core4"] = new TiadcPhaseOffsetGainItem_Base() { Reserved0 = 800, Reserved1 = 200 };
            TiadcPhaseOffsetGainParams.Default["single_c4_adc3_core4"] = new TiadcPhaseOffsetGainItem_Base() { Reserved0 = 100, Reserved1 = 200 };
            TiadcPhaseOffsetGainParams.Default["single_c1_adc3_core4"] = new TiadcPhaseOffsetGainItem_Base() { Reserved0 = 100, Reserved1 = 200 };
            base.InitAcq();
        }

        private UInt32 GetDbiCtrlWords(Int32 subbandId, Boolean IsLocalOscillator)
        {
            UInt32 ctrlwords = 0;

            //if (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.bIsLongStorageMode ?? false)
            //{

            //}
            //else
            //{
                if (Hd.CurrDebugVarints.bEnable_Dbi_IntDelay)
                {
                    ctrlwords |= (1 << 1);
                }
                if ((Hd.CurrDebugVarints.bEnable_Dbi_AntiImageCoef || Hd.CurrDebugVarints.bEnable_Dbi_LocalOscillatorCoef) && IsLocalOscillator)
                {
                    ctrlwords |= (1 << 2);
                    ctrlwords |= (0 << 4);
                }

                if (Hd.CurrDebugVarints.bEnable_Dbi_FractionaryDelayCoef || Hd.CurrDebugVarints.bEnable_Dbi_OverlapPhaseFreqDelayCoef)
                {
                    ctrlwords |= (1 << 0);
                }

            //}

            //if (Hd.CurrDebugVarints.bEnable_AcqbdInterpolation)
            if (Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation)
            {
                ctrlwords |= (1 << 3);
            }

            return ctrlwords;
        }

        /// <summary>
        /// 获取各个子带在DBI模式下的控制字
        /// </summary>
        /// <returns></returns>
        private UInt32[] GetDbiCtrlWords()
        {
            UInt32 CH_Band1 = 0;
            UInt32 CH_Band2_3 = 0;
            
            if (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.bIsLongStorageMode ?? false)
            {

            }
            else
            {
                if (Hd.CurrDebugVarints.bEnable_Dbi_IntDelay)
                {
                    CH_Band1 |= (1 << 1);
                    CH_Band2_3 |= (1 << 1);
                }
                if (Hd.CurrDebugVarints.bEnable_Dbi_AntiImageCoef || Hd.CurrDebugVarints.bEnable_Dbi_LocalOscillatorCoef)
                {
                    CH_Band2_3 |= (1 << 2);
                    CH_Band2_3 |= (0 << 4);
                }

                if (Hd.CurrDebugVarints.bEnable_Dbi_FractionaryDelayCoef || Hd.CurrDebugVarints.bEnable_Dbi_OverlapPhaseFreqDelayCoef)
                {
                    CH_Band2_3 |= (1 << 0);
                }
                    
            }
            
            if (Hd.CurrDebugVarints.bEnable_AcqbdInterpolation)
            {
                CH_Band1 |= (1 << 3);
                CH_Band2_3 |= (1 << 3);
            }

            Int32 bitwidth = Hd.UIMessage?.Precision?.AnaChnlBitWidth ?? 12;//dyh

            //switch (bitwidth)
            //{
            //    case 14:

            //        return new UInt32[] { 0x0008, 0x0018, 0x0018, 0x0018 };

            //    case 15:

            //        return new UInt32[] { 0x0008, 0x0018, 0x0018, 0x0018 };

            //    default:
            //        return new UInt32[] { 0x0008, 0x0018, 0x0018, 0x0018 };
            //        break;
            //}

            if (ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.BitWidth < 14)
            {
                return new UInt32[] { CH_Band1, CH_Band2_3 , CH_Band2_3 , CH_Band2_3  };
            }
            else
            {
                if (ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.SubbandId == 0)
                {
                    //return new UInt32[] { CH_Band1, CH_Band1 + 0x10, CH_Band1 + 0x10, CH_Band1 + 0x10 };
                    return new UInt32[] { CH_Band1, CH_Band1, CH_Band1, CH_Band1 };
                }
                else
                {
                    //return new UInt32[] { CH_Band2_3 + 0x10, CH_Band2_3, CH_Band2_3, CH_Band2_3 };
                    return new UInt32[] { CH_Band2_3, CH_Band2_3, CH_Band2_3, CH_Band2_3 };
                }
            }

            return new UInt32[] { CH_Band1, CH_Band2_3, CH_Band2_3, CH_Band2_3 };//dyh_1_8
        }

        private void ConfigProcDbiWords()//????
        {
            UInt32 procBdFuncEnable = 0;
            UInt32 PFCFuncEnable = 0;
            if (Hd.CurrDebugVarints.bEnable_Dbi_AmpFreqCoef)
                procBdFuncEnable |= (1 << 1);
            if (Hd.CurrDebugVarints.bEnable_Dbi_PhaseFreqCoef)
            {
                PFCFuncEnable = 1;
                procBdFuncEnable |= (1 << 0);
            }
            if (Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode)
                procBdFuncEnable |= (1 << 2);


            //HdIO.WriteReg(ProcBdReg.W.DBI_PfcCaliEn, PFCFuncEnable);
            HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiModuleEn, procBdFuncEnable);

            //if (Hd.CurrProduct?.Acquirer_AnalogChannel?.AcquingParameters?.bIsLongStorageMode ?? false)
            //{
            //    HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiModuleEn, 0b1011);
            //} //HTF_1117
            bool bFast = Hd.UIMessage!.Display!.IsFast;
            Int32 ActiveChnlCnt = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.ActiveChnlCnt;
            if (bFast && Hd.UIMessage?.Timebase?.TmbScaleIndex == (int)AnaChnlTimebaseIndex.Lv5n && (UInt32)ActiveChnlCnt == 1)
                HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiModuleEn, 8);//25.7.21 WBY
        }

        /// <summary>
        /// 假Dbi模式：使用0数据填入每个通道的子带2和子带3，然后与各自的子带1进行拼合，60GSPS
        /// </summary>
        private void ConfigFakeDbiMode()
        {
            ConfigProcDbiWords();

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_init_delay_en, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh1, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh2, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh3, 0);
            HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh4, 0);

            var subbandctrlwords = GetDbiCtrlWords();

            Dictionary<AcqBdNo, UInt32> DBI_ACQ_DataModeValueTable = new();
            foreach (ChannelId chnlid in ChannelIdExt.GetAnalogs())
            {
                if (subbandctrlwords.Length > 0)
                {
                    var adcuseinfo = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(DbiActuallActiveState, chnlid, 0);
                    if (adcuseinfo != null)
                    {
                        DBI_ACQ_DataModeValueTable[adcuseinfo.AcqBdNo] = subbandctrlwords[0];
                    }
                }
            }

            foreach (AcqBdNo acqBd in DBI_ACQ_DataModeValueTable.Keys)
            {
                switch (acqBd)//Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DBI_DigitProcessEn, acqBd, DBI_ACQ_DataModeValueTable[acqBd]);//testmode选择
                {
                    case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh1, DBI_ACQ_DataModeValueTable[acqBd]); break;
                    case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh2, DBI_ACQ_DataModeValueTable[acqBd]); break;
                    case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh3, DBI_ACQ_DataModeValueTable[acqBd]); break;
                    case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh4, DBI_ACQ_DataModeValueTable[acqBd]); break;
                }
            }
        }

        /// <summary>
        /// 配置为DBI拼合模式：三个子带拼合为一个通道
        /// </summary>
        private void ConfigDbiMode()//????
        {
            #region 存疑：DBI模式下为啥还要受Tool的通道控制？
            ChannelId watchChannelID = (ChannelId)Hd.CurrDebugVarints.iDbi_DebugChannelID;
            Dictionary<ChannelId, UInt32> datamodetable = new()
                {
                    {ChannelId.C1, 0x4 },
                    {ChannelId.C2, 0x5 },
                    {ChannelId.C3, 0x6 },
                    {ChannelId.C4, 0x7 },
                };
            //HdIO.WriteReg(ProcBdReg.W.DBI_DataMode, datamodetable.ContainsKey(watchChannelID) ? datamodetable[watchChannelID] : 0);
            #endregion

            ConfigProcDbiWords();

            Int32 bitwidth = Hd.UIMessage?.Precision?.AnaChnlBitWidth ?? 12;
            SubbandCtrlMethod ctrlmethod = Hd.UIMessage?.AiTable?[ChannelId.C1].RecfgDbi?.SubbandCtrlMethod ?? SubbandCtrlMethod.UserManual;
            if (ctrlmethod == SubbandCtrlMethod.BitWidthAdaptive)
            {
                bitwidth = ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.BitWidth;
            }

            Dictionary<AcqBdNo, UInt32> DBI_ACQ_DataModeValueTable = new();

            for (Int32 subbandid = 0; subbandid < ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.CurLoFreq.Length; subbandid++)
            {
                UInt32 ctrlword = GetDbiCtrlWords(subbandid, ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.CurLoFreq[subbandid] != 0); //HTF_1117_DEBUG
                //UInt32 ctrlword = GetDbiCtrlWords(subbandid,subbandid != 0); //HTF_1117_DEBUG
                var adcuseinfo = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(DbiActuallActiveState, ChannelId.C1, subbandid);
                //if (bitwidth >= 14 && ((ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.SubbandId == 0 && subbandid != 0) || (ArtificialIntelligenceProcess.Default.ReconfigDBIProcess.SubbandId != 0 && subbandid == 0)))
                //{
                //    ctrlword += 0x10;
                //}

                if (adcuseinfo != null)
                {
                    DBI_ACQ_DataModeValueTable[adcuseinfo.AcqBdNo] = ctrlword;
                }
            }

            foreach (AcqBdNo acqBd in DBI_ACQ_DataModeValueTable.Keys)
            {
                switch (acqBd)//Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DBI_DigitProcessEn, acqBd, DBI_ACQ_DataModeValueTable[acqBd]);//testmode选择
                {
                    case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh1, DBI_ACQ_DataModeValueTable[acqBd]); break;
                    case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh2, DBI_ACQ_DataModeValueTable[acqBd]); break;
                    case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh3, DBI_ACQ_DataModeValueTable[acqBd]); break;
                    case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh4, DBI_ACQ_DataModeValueTable[acqBd]); break;
                }
            }
        }

        /// <summary>
        /// 单子带模式：上传数据为各个通道的第一子带的数据
        /// </summary>
        private void ConfigSingBandMode()
        {/*
            //           HdIO.WriteReg(ProcBdReg.W.DBI_PfcCaliEn, 0);
            //           HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiModuleEn, 0);
            //DBI_ACQ_ctrl_cij
            UInt32 CH_Band1 = 0;
            UInt32 CH_Band2_3 = 0;
            if (Hd.CurrDebugVarints.bEnable_Dbi_IntDelay)
            {
                CH_Band1 |= (1 << 1);
                CH_Band2_3 |= (1 << 1);
            }
            if (Hd.CurrDebugVarints.bEnable_Dbi_AntiImageCoef || Hd.CurrDebugVarints.bEnable_Dbi_LocalOscillatorCoef)
                CH_Band2_3 |= (1 << 2);
            if (Hd.CurrDebugVarints.bEnable_Dbi_FractionaryDelayCoef || Hd.CurrDebugVarints.bEnable_Dbi_OverlapPhaseFreqDelayCoef)
                CH_Band2_3 |= (1 << 0);
            if (Hd.CurrDebugVarints.bEnable_AcqbdInterpolation)
            {
                CH_Band1 |= (1 << 3);
                CH_Band2_3 |= (1 << 3);

            }
            Dictionary<AcqBdNo, UInt32> DBI_ACQ_DataModeValue = new()
            {
                {AcqBdNo.B1,  CH_Band1},
                {AcqBdNo.B2,  CH_Band2_3},
                {AcqBdNo.B3,  CH_Band2_3},
                {AcqBdNo.B4,  CH_Band1},
                {AcqBdNo.B5,  CH_Band2_3},
                {AcqBdNo.B6,  CH_Band2_3},
                {AcqBdNo.B7,  CH_Band1},
                {AcqBdNo.B8,  CH_Band2_3},
                {AcqBdNo.B9,  CH_Band2_3},
                {AcqBdNo.B10, CH_Band1},
                {AcqBdNo.B11, CH_Band2_3},
                {AcqBdNo.B12, CH_Band2_3},
            };
            foreach (var testmode in DBI_ACQ_DataModeValue)
            {
                Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DBI_DigitProcessEn, testmode.Key, testmode.Value);//testmode选择
            }

            //DBI_pro_ctrl_cij
            UInt32 procBdFuncEnable = 0;
            UInt32 PFCFuncEnable = 0;       //wy pfc_en 0928
            if (Hd.CurrDebugVarints.bEnable_Dbi_AmpFreqCoef)
                procBdFuncEnable |= (1 << 1);
            if (Hd.CurrDebugVarints.bEnable_Dbi_PhaseFreqCoef)
            {
                PFCFuncEnable = 1;
                procBdFuncEnable |= (1 << 0);
            }

            //DDR_DBI传拼合数据
            //           if (Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode)
            //             procBdFuncEnable |= (1<< 2);
            //DDR_DBI传拼合数据


            //DDR_DBI只传第一子带的数据
            procBdFuncEnable |= (1 << 3);
            procBdFuncEnable |= (0 << 2);
            //DDR_DBI只传第一子带的数据

            HdIO.WriteReg(ProcBdReg.W.DBI_PfcCaliEn, PFCFuncEnable);
            HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiModuleEn, procBdFuncEnable);
            */
            
            HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiModuleEn, 0x8);   //0919 wy

            foreach (AcqBdNo acqbd in Hd.CurrProduct?.AcqBd?.ExistsAcqBdDefine ?? new List<AcqBdNo>())
            {
                switch (acqbd)//Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DBI_DigitProcessEn, acqbd, 0);
                {
                    case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh1, 0); break;
                    case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh2, 0); break;
                    case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh3, 0); break;
                    case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh4, 0); break;
                }
            }
        }

        /// <summary>
        /// 子带模式：上传数据为指定通道的各个子带
        /// </summary>
        private void ConfigSubBandMode(ChannelId watchChannelID)//????
        {
            Dictionary<ChannelId, UInt32> datamodetable = new()
                {
                    {ChannelId.C1, 0x0 },
                    {ChannelId.C2, 0x1 },
                    {ChannelId.C3, 0x2 },
                    {ChannelId.C4, 0x3 },
                };
            //HdIO.WriteReg(ProcBdReg.W.DBI_DataMode, datamodetable.ContainsKey(watchChannelID) ? datamodetable[watchChannelID] : 0);

            //HdIO.WriteReg(ProcBdReg.W.DataPath_ProDataSelectMode, 0x8000u | (0x1u << (Int32)watchChannelID));
            bool bFast = Hd.UIMessage!.Display!.IsFast;
            Int32 ActiveChnlCnt = Hd.CurrProduct!.Acquirer_AnalogChannel!.AcquingParameters.ActiveChnlCnt;
            //if (bFast && Hd.UIMessage?.Timebase?.TmbScaleIndex == (int)AnaChnlTimebaseIndex.Lv5n && (UInt32)ActiveChnlCnt == 1)
            //    HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiModuleEn, 8);//25.7.21 WBY
            //else
            //    HdIO.WriteReg(ProcBdReg.W.DBI_ProDbiModuleEn, 0);//dpx 350M 260115wby

            var subbandctrlwords = GetDbiCtrlWords();

            UInt32 fpgaactivestate = Hd.CurrProduct?.AnalogAcquireModel?.GetActuallActiveState(0x1u << (Int32)watchChannelID) ?? 0xf;

            Dictionary<AcqBdNo, UInt32> DBI_ACQ_DataModeValueTable = new();
            foreach (ChannelId chnlid in ChannelIdExt.GetAnalogs())
            {
                for (Int32 subbandid = 0; subbandid < subbandctrlwords.Length; subbandid++)
                {
                    var adcuseinfo = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(fpgaactivestate, (ChannelId)chnlid, subbandid);
                    if (adcuseinfo != null)
                    {
                        DBI_ACQ_DataModeValueTable[adcuseinfo.AcqBdNo] = subbandctrlwords[subbandid];
                    }
                }
            }

            foreach (AcqBdNo acqBd in DBI_ACQ_DataModeValueTable.Keys)
            {
                switch (acqBd)//Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DBI_DigitProcessEn, acqBd, DBI_ACQ_DataModeValueTable[acqBd]);//testmode选择
                {
                    case AcqBdNo.B0: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh1, DBI_ACQ_DataModeValueTable[acqBd]); break;
                    case AcqBdNo.B1: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh2, DBI_ACQ_DataModeValueTable[acqBd]); break;
                    case AcqBdNo.B2: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh3, DBI_ACQ_DataModeValueTable[acqBd]); break;
                    case AcqBdNo.B3: HdIO.WriteReg(ProcBdReg.W.DBI_DigitProcessEnProCh4, DBI_ACQ_DataModeValueTable[acqBd]); break;
                }
            }
        }
        private Boolean _FirstActive = true;
        private const Char Buffer ='C';//暂时调试用
        internal override void AnalogChannelActiveChanged()
        {
            //Int32 bufferCtrlWords = 0b0111_1000;
            Int32 bufferCtrlWords = 0b0100_1000;
            //CtrlAnalogChannel_DBI20G.DBI_SendASCII(0, $"Com[{Buffer}]={bufferCtrlWords.ToString("X2")}000000@");
            //CtrlAnalogChannel_DBI20G.DBI_SendASCII(2, $"Com[{Buffer}]={bufferCtrlWords.ToString("X2")}000000@");
            //CtrlAnalogChannel_DBI20G.SendBufferCtrlWords(DbiActuallActiveState);
            //if (_ChnlActiveChanged || _FirstActive)
            if (_FirstActive)
            {
                CtrlAnalogChannel_DBI20G.DBI_SendASCII(0, $"Com[{Buffer}]={bufferCtrlWords.ToString("X2")}000000@");
                CtrlAnalogChannel_DBI20G.DBI_SendASCII(2, $"Com[{Buffer}]={bufferCtrlWords.ToString("X2")}000000@");
                CtrlAnalogChannel_DBI20G.SendBufferCtrlWords(DbiActuallActiveState);
                if (GetAdcSampleIntervalByUs() == _DbiModeSampleInterval_us)
                    CtrlAnalogChannel_DBI20G.SendDiscard();
                else
                {
                    //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.DBI_init_delay_en, 0);
                    HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh1, 0);
                    HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh2, 0);
                    HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh3, 0);
                    HdIO.WriteReg(ProcBdReg.W.DBI_init_delay_enProCh4, 0);
                }

                _FirstActive = false;
            }
            
        }

        internal override List<UInt16> ParseDMAData(Int32 channelId, Byte[] dmaBuff)
        {
            Int32 dotCnt = dmaBuff.Length / 8;
            List<UInt16> dataarray = new();
            for (int dotIndex = 0; dotIndex < dotCnt; dotIndex++)
            {
                ushort data = channelId switch
                {
                    0 => (UInt16)(dmaBuff[8 * dotIndex + 1] << 8 | (dmaBuff[8 * dotIndex + 0])),
                    1 => (UInt16)(dmaBuff[8 * dotIndex + 3] << 8 | (dmaBuff[8 * dotIndex + 2])),
                    2 => (UInt16)(dmaBuff[8 * dotIndex + 5] << 8 | (dmaBuff[8 * dotIndex + 4])),
                    3 => (UInt16)(dmaBuff[8 * dotIndex + 7] << 8 | (dmaBuff[8 * dotIndex + 6])),
                    _ => 0,
                };
                dataarray.Add(data);
            }
            return dataarray;
        }

        private Double _noise = double.NaN;

        protected override void AnalogChannelDataSplit(UInt32 inculdeChannelBit, Byte[] dmaBuff, Int32 perChannelValidDotCount, List<List<UInt16>> ChannelDataList, Boolean clearFlag)
        {
            if (clearFlag)
            {
                foreach (var channelData in ChannelDataList)
                    channelData.Clear();
            }
            var dmaBuffInidexMax = dmaBuff.Length / 8;
            dmaBuffInidexMax = dmaBuffInidexMax < perChannelValidDotCount ? dmaBuffInidexMax : perChannelValidDotCount;
            for (int channelId = 0; channelId < ChannelDataList.Count; channelId++)
            {
                for (int dotIndex = 0; dotIndex < dmaBuffInidexMax; dotIndex++)
                {
                    ushort data = channelId switch
                    {
                        0 => (UInt16)(dmaBuff[8 * dotIndex + 1] << 8 | (dmaBuff[8 * dotIndex + 0])),
                        1 => (UInt16)(dmaBuff[8 * dotIndex + 3] << 8 | (dmaBuff[8 * dotIndex + 2])),
                        2 => (UInt16)(dmaBuff[8 * dotIndex + 5] << 8 | (dmaBuff[8 * dotIndex + 4])),
                        3 => (UInt16)(dmaBuff[8 * dotIndex + 7] << 8 | (dmaBuff[8 * dotIndex + 6])),
                        _ => 0,
                    };
                    ChannelDataList[channelId].Add(data);
                }
            }
            DiscardDotAtTriggerTypeIsSerialMode();
            AbstractController_AnalogChannel.SoftwareBandwidthProcess();
        }

        protected override void AnalogChannelDataSplit(byte[] dmaBuff, int perChannelValidDotCount, bool clearFlag = true)
        {
            Int32 chnlCnt = ChannelIdExt.AnaChnlNum;
            while (AcqedDataPool.AnalogChData.AllChannelData.Count < chnlCnt)
                AcqedDataPool.AnalogChData.AllChannelData.Add(new List<ushort>());

            if (clearFlag)
            {
                foreach (var channelData in AcqedDataPool.AnalogChData.AllChannelData)
                    channelData.Clear();
            }

            Int32 dotCnt = dmaBuff.Length / 8;
            if (perChannelValidDotCount < dotCnt)
                dotCnt = perChannelValidDotCount;

            for (int channelId = 0; channelId < chnlCnt; channelId++)
            {
                for (int dotIndex = 0; dotIndex < dotCnt; dotIndex++)
                {
                    ushort data = channelId switch
                    {                      
                        0 => (UInt16)(dmaBuff[8 * dotIndex + 1] << 8 | (dmaBuff[8 * dotIndex + 0])),
                        1 => (UInt16)(dmaBuff[8 * dotIndex + 3] << 8 | (dmaBuff[8 * dotIndex + 2])),
                        2 => (UInt16)(dmaBuff[8 * dotIndex + 5] << 8 | (dmaBuff[8 * dotIndex + 4])),
                        3 => (UInt16)(dmaBuff[8 * dotIndex + 7] << 8 | (dmaBuff[8 * dotIndex + 6])),
                        _ => 0,
                    };
                    AcqedDataPool.AnalogChData.AllChannelData[channelId].Add((UInt16)(data >> 0));
                }
            }
            //PublicFunc.SaveDataToFile("ch1.txt", AcqedDataPool.AnalogChData.AllChannelData[0]);

            Int32[] DiscardNum = new Int32[] { 0, 0, 0, 0 };
            for (int channelId = 0; channelId < chnlCnt; channelId++)
            {
                if (AcqedDataPool.AnalogChData.AllChannelData[channelId].Count >= DiscardNum[channelId])
                    AcqedDataPool.AnalogChData.AllChannelData[channelId].RemoveRange(0, DiscardNum[channelId]);
            }

            Boolean flag = ArtificialIntelligenceProcess.Default.AutoFilterProcess.TryExcuteBySoftware(AcqedDataPool.AnalogChData.AllChannelData[0], AcquedParameters.PerDataByfs_AtDMA, out List<UInt16> filterresult);
            if (flag && filterresult.Count > 0)
            {
                AcqedDataPool.AnalogChData.AllChannelData[0].Clear();
                AcqedDataPool.AnalogChData.AllChannelData[0].AddRange(filterresult);
            }

            DiscardDotAtTriggerTypeIsSerialMode();
            AbstractController_AnalogChannel.SoftwareBandwidthProcess();

            Boolean enableMatlab = false;//是否将matlab运算的数据灌入通道4
            if (enableMatlab)
            {
                Boolean success = TryExcuteDbi(out Double[]? result);
                if (success && result != null)
                {
                    AcqedDataPool.AnalogChData.AllChannelData[3].Clear();
                    AcqedDataPool.AnalogChData.AllChannelData[3].AddRange(result.Select(o => (UInt16)o));
                }
            }

            if (Hd.CurrDebugVarints.bEnable_CorrectTiAdc)
            {
                //var coeff = TakeFreqFilterCoeMatlabEngine(AcqedDataPool.AnalogChData.AllChannelData[0], Double.IsNaN(_noise) ? 0 : _noise);

                //if (Hd.CurrDebugVarints.bEnable_CalcNoise)
                //{
                //    Double noise = CalcNoiseMatlabEngine(AcqedDataPool.AnalogChData.AllChannelData[0]);
                //    if (Double.IsNormal(noise))
                //    {
                //        if (Double.IsNaN(_noise))
                //        {
                //            _noise = noise;
                //        }
                //        else
                //        {
                //            _noise = 0.618 * _noise + (1 - 0.618) * noise;
                //        }
                //    }
                //}
                //if (Hd.CurrDebugVarints.bEnable_SendTiadcCoeff)
                //{
                //    if (Hd.CurrDebugVarints.bEnable_ResetTiadcCoeff)
                //    {
                //        Int32[] tmp = new Int32[4096];
                //        for (Int32 i = 0; i < tmp.Length / 2; i++)
                //        {
                //            tmp[i * 2] = 4096;
                //        }
                //        HdCtrl_Coefficient.SendTiadcByRegister(0, AcqBdNo.B11, tmp, tmp.Length);
                //    }
                //    else if (coeff.Length > 0)
                //    {
                //        HdCtrl_Coefficient.SendTiadcByRegister(0, AcqBdNo.B11, coeff.Select(o => (Int32)o).ToArray(), coeff.Length);
                //    }
                //}
            }
        }

        private Boolean TryExcuteDbi(out Double[] result)
        {
            var ans = Matlab.Default.SetWorkFolder("C:\\Matlab\\");

            ans = Matlab.Default.ExcuteCode("clear all");

            var subBand1 = AcqedDataPool.AnalogChData.AllChannelData[0].Select(o => (Double)o).ToArray();
            var subBand2 = AcqedDataPool.AnalogChData.AllChannelData[1].Select(o => (Double)o).ToArray();
            var subBand3 = AcqedDataPool.AnalogChData.AllChannelData[2].Select(o => (Double)o).ToArray();

            ans = Matlab.Default.PutData(subBand1, "subBand1");
            ans = Matlab.Default.PutData(subBand2, "subBand2");
            ans = Matlab.Default.PutData(subBand3, "subBand3");

            List<String> codelist = new List<String>()
            {
                "s1 = cell2mat(subBand1);",
                "s2 = cell2mat(subBand2);",
                "s3 = cell2mat(subBand3);",
                "resultData = fun_sinfit_diffphase(s1, s2, s3)",
            };

            ans = Matlab.Default.ExcuteCode(String.Join("\n", codelist));

            //Trace.WriteLine(ans?.ToString());

            //ans = Matlab.Default.TryGetData("resultData", out Object? data);
            //if (data is not null)
            //{
            //    if (data is Double[])
            //    {
            //        result = (Double[])data;
            //        return true;
            //    }

            //    if (data is Double[,])
            //    {
            //        Double[,] tmp = (Double[,])data;
            //        result = new Double[tmp.GetLength(0) * tmp.GetLength(1)];
            //        Array.Copy(tmp, result, result.Length * sizeof(Double));
            //        return true;
            //    }
            //}

            result = new Double[0];
            return false;
        }

        private Double CalcNoiseMatlabEngine(List<UInt16> timeDomainData)
        {
            var ans = Matlab.Default.SetWorkFolder("C://Matlab//");

            ans = Matlab.Default.ExcuteCode("clear all");

            ans = Matlab.Default.PutData(timeDomainData.Select(o => (Double)o).ToArray(), "data");

            List<String> codelist = new List<String>()
            {
                "source = cell2mat(data);",
                $"resultData = calcNoise(source, 1024);",
            };

            ans = Matlab.Default.ExcuteCode(String.Join("\n", codelist));

            ans = Matlab.Default.TryGetData("resultData", out Object? data);
            if (data is Double)
                return (Double)data;

            return double.NaN;
        }

        private UInt16[] TakeFreqFilterCoeMatlabEngine(List<UInt16> timeDomainData, Double th_noise)
        {
            var ans = Matlab.Default.SetWorkFolder("C://Matlab//");

            ans = Matlab.Default.ExcuteCode("clear all");

            ans = Matlab.Default.PutData(timeDomainData.Select(o => (Double)o).ToArray(), "data");

            List<String> codelist = new List<String>()
            {
                "source = cell2mat(data);",
                $"resultData = freqFilter(source, 1024, {th_noise.ToString("0.000")}, 4096);",
            };

            ans = Matlab.Default.ExcuteCode(String.Join("\n", codelist));

            ans = Matlab.Default.TryGetData("resultData", out Object? data);

            if (data is Double[,])
            {
                Double[,] resultdata = (Double[,])data;
                UInt16[] output = new UInt16[4096];

                for (Int32 i = 0; i < output.Length / 8; i++)
                {
                    output[i * 2] = (UInt16)Math.Round(resultdata[0, i]);
                    output[i * 2 + 2048] = (UInt16)Math.Round(resultdata[0, i]);
                    output[2046 - 2 * i] = (UInt16)Math.Round(resultdata[0, i]);
                    output[4094 - 2 * i] = (UInt16)Math.Round(resultdata[0, i]);
                }
                return output;
            }

            return new UInt16[0];
        }

        private UInt16[] TakeFreqFilterCoe(List<UInt16> timeDomainData)
        {
            MatlabDll? freqFilterinstance = Hd.CurrProduct?.MatlabDlls["freqFilter.dll"];
            var data = new MWNumericArray(timeDomainData.Select(o => (Double)o).ToArray());
            var coelength = new MWNumericArray(1024);
            var sigth = new MWNumericArray(-20.0);
            var coemax = new MWNumericArray(4096);
            var coe = (MWNumericArray?)freqFilterinstance?.Method?.Invoke(freqFilterinstance?.Instance, new MWArray[] { data, coelength, sigth, coemax });
            Double[] tmp_data = (double[])((MWNumericArray)coe!).ToVector(MWArrayComponent.Real);
            UInt16[] result = new UInt16[2048];

            for (Int32 i = 0; i < tmp_data.Length - 1; i++)
            {
                result[2 * i] = (UInt16)Math.Round(tmp_data[i]);
                result[2046 - 2 * i] = (UInt16)Math.Round(tmp_data[i]);
            }

            return result;
        }

        private void ReadTrigSensitivityFromFile()
        {
            String fileName = "./CaliData/TrigSensitivity.txt";
            if (File.Exists(fileName))
            { 
                StreamReader sr = new StreamReader(fileName);
                while (!sr.EndOfStream)
                { 
                    var tmp = sr.ReadLine();
                    if (tmp == null)
                        continue;
                    var infostr = tmp.Split(" ");
                    if (infostr.Length != 2)
                        continue;
                    AnaChnlScaleIndex scale = (AnaChnlScaleIndex)Enum.Parse(typeof(AnaChnlScaleIndex), infostr[0]);
                    _TrigSensitivityTable[scale] = UInt32.Parse(infostr[1]);
                }
            }
        }

        private Dictionary<AnaChnlScaleIndex, UInt32> _TrigSensitivityTable = new();

        internal override uint DefaultTrigSensitivity
        {
            get
            {
                uint Sensitivity = 400;
                return (UInt32)((Hd.UIMessage?.Trigger?.Edge?.SensitivityBymdiv ?? 500) * Constants.VIS_ADC_RES / Constants.VIS_YDIVS_NUM / Constants.IDX_PER_YDIV);
                //ChannelId trigchnlid = Hd.CurrProduct?.Ctrl_Trigger?.CurrentTrigSource ?? ChannelId.C1;
                //if (Hd.UIMessage?.Analog != null && (Int32)trigchnlid < Hd.UIMessage.Analog.Length)
                //{
                //    AnaChnlScaleIndex scaleindex = (AnaChnlScaleIndex)Hd.UIMessage.Analog[(Int32)trigchnlid].ScaleIndex;
                //    ReadTrigSensitivityFromFile();
                //    if (_TrigSensitivityTable.ContainsKey(scaleindex))
                //        Sensitivity = _TrigSensitivityTable[scaleindex];
                //}

                //return Sensitivity;
                //int trigSource = (int)(Hd.CurrProduct?.Ctrl_Trigger?.CurrentTrigSource ?? ChannelId.C1);
                //if (Hd.UIMessage!.Trigger!.TrigType == TriggerType.Edge && Hd.UIMessage!.Trigger!.Edge!.Coupling == TriggerCoupling.NR)
                //{
                //    Sensitivity = (uint)(2 * AbstractController_Trigger.PerYDivAdcSamples);    //噪声抑制开启2格灵敏度
                //    return Sensitivity;
                //}
                //else
                //{
                //    Sensitivity = (uint)(0.3 * AbstractController_Trigger.PerYDivAdcSamples); //其他情况默认0.3格灵敏度  
                //    if (trigSource > (int)ChannelId.C4)
                //    {
                //        if (trigSource == (int)ChannelId.Ext)
                //            return 800;
                //        else
                //            return Sensitivity;
                //    }
                //}

                //if (Hd.UIMessage!.Trigger!.TrigType != TriggerType.Edge)
                //    return 500;

                //AnaChnlScaleIndex trigSourceScaleIndex = (AnaChnlScaleIndex)Hd.UIMessage!.Analog![trigSource].ScaleIndex;

                //switch (trigSourceScaleIndex)
                //{
                //    case AnaChnlScaleIndex.Lv20m:
                //        if (AcquingParameters.bIsLongStorageMode)
                //            return 800;
                //        else
                //            return 480;
                //    case AnaChnlScaleIndex.Lv1:
                //        return 800;
                //    case AnaChnlScaleIndex.Lv500m:
                //        return 1400;
                //    case AnaChnlScaleIndex.Lv200m:
                //        return 480;
                //    case AnaChnlScaleIndex.Lv100m:
                //        return 1400;
                //    case AnaChnlScaleIndex.Lv50m:
                //        return 1400;
                //    case AnaChnlScaleIndex.Lv10m:
                //        return 1600;
                //    case AnaChnlScaleIndex.Lv5m:
                //        return 2800;
                //    case AnaChnlScaleIndex.Lv2m:
                //        return 1200;
                //    default: return 120;

                //}
            }
        }

        internal override List<List<ChannelBdAdcInputDefine>>? FirstInitChannelBdAdcInputDefines => ourFirstInitChannelBdAdcInputDefines;
        private List<List<ChannelBdAdcInputDefine>> ourFirstInitChannelBdAdcInputDefines = new List<List<ChannelBdAdcInputDefine>>()
        {
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B1, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B1, AdcIndex=1, InputPort_AIs1=1  },
            },
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B2, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B2, AdcIndex=1, InputPort_AIs1=1  },
            },
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B3, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B3, AdcIndex=1, InputPort_AIs1=1  },
            },
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B4, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B4, AdcIndex=1, InputPort_AIs1=1  },
            },
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B5, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B5, AdcIndex=1, InputPort_AIs1=1  },
            },
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B6, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B6, AdcIndex=1, InputPort_AIs1=1  },
            },
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B7, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B7, AdcIndex=1, InputPort_AIs1=1  },
            },
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B8, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B8, AdcIndex=1, InputPort_AIs1=1  },
            },
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B9, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B9, AdcIndex=1, InputPort_AIs1=1  },
            },
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B10, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B10, AdcIndex=1, InputPort_AIs1=1  },
            },
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B11, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B11, AdcIndex=1, InputPort_AIs1=1  },
            },
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B12, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B12, AdcIndex=1, InputPort_AIs1=1  },
            },
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B13, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B13, AdcIndex=1, InputPort_AIs1=1  },
            },
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B14, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B14, AdcIndex=1, InputPort_AIs1=1  },
            },
            new List<ChannelBdAdcInputDefine>()
            {
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B15, AdcIndex=0, InputPort_AIs1=1  },
                new ChannelBdAdcInputDefine(){ BdNo= AcqBdNo.B15, AdcIndex=1, InputPort_AIs1=1  },
            },
        };

        /// <summary>
        /// 只用了一个采集板的一个ADC的4个核
        /// </summary>
        /// <param name="activeChnl"></param>
        /// <returns></returns>
        internal override int[] GetStorageDotsCnt(ChannelId[] activeChnl)
        {
            Int32 coreCnt = 4;

            if (_StorageDotsCntPerCore != null)
                return _StorageDotsCntPerCore.Select(o => o * coreCnt).ToArray();
            return new Int32[0];
        }

        internal override void PostProcess(List<ReadInfo> readInfoList, CancellationToken? softResetToken)
        {
            base.PostProcess(readInfoList, softResetToken);
            ConfigBaseNoise();
            ArtificialIntelligenceProcess.Default.AutoFilterProcess.IterAvgFilter();
            ArtificialIntelligenceProcess.Default.MultiDomainProcess.Run();
        }

        private void ConfigBaseNoise()
        {
            var aimsg = Hd.UIMessage?.AiTable;
            if (aimsg == null)
            {
                _BaseNoiseSampleCount = 0;
                _BaseNoiseTrueCntTable.Clear();
                _BaseNoiseSubbandIds.Clear();
                return;
            }

            foreach (ChannelId chnlid in aimsg.Keys)
            {
                var basenoise = aimsg[chnlid]?.RecfgDbi?.BaseNoise;
                UInt32 fpgaactivestate = Hd.CurrProduct?.AnalogAcquireModel?.GetActuallActiveState(0x1u << (Int32)chnlid) ?? 0xf;
                if (basenoise == null)
                    continue;

                foreach (Int32 subbandid in basenoise.Keys)
                {
                    var adcuseinfo = Hd.CurrProduct?.AnalogAcquireModel?.GetAdcUsedInfo(fpgaactivestate, chnlid, subbandid);
                    if (adcuseinfo != null)
                    {
                        Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.SenceFFT_sence_fft_threshold_l16, adcuseinfo.AcqBdNo, 0);                        //
                        //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.SenceFFT_sence_fft_threshold_h16, adcuseinfo.AcqBdNo, basenoise[subbandid]);     //
                        Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.SenceFFT_sence_fft_threshold_h16, adcuseinfo.AcqBdNo, 20);     //55
                        //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.SenceFFT_sence_fft_threshold, adcuseinfo.AcqBdNo, basenoise[subbandid]);

                        //UInt32 tmpdatah = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.reverse_Read0, adcuseinfo.AcqBdNo) ?? 0;
                        //UInt32 tmpdatal = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.reverse_Read1, adcuseinfo.AcqBdNo) ?? 0;
                        UInt32 tmpdatah = 0;
                        UInt32 tmpdatal = 0;
                        UInt32 tmpdataall = (tmpdatah << 16) | tmpdatal;
                        UInt32? tmpdata = Hd.CurrProduct?.AcqBd?.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_out_valid, adcuseinfo.AcqBdNo);

                        UInt32 max = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_freg_max, adcuseinfo.AcqBdNo);
                        UInt32 min = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.SenceFFT_sence_fft_freg_min, adcuseinfo.AcqBdNo);

                        if (subbandid == 2)
                        {

                        }
                        Boolean tmp = tmpdata == 1;
                        _BaseNoiseSubbandIds.Add(subbandid);
                        if (tmp)
                        {
                            _BaseNoiseTrueCntTable[subbandid] = (_BaseNoiseTrueCntTable.TryGetValue(subbandid, out Int32 cnt) ? cnt : 0) + 1;
                        }
                    }
                }
            }

            _BaseNoiseSampleCount++;
            if (_BaseNoiseSampleCount < _BaseNoiseSampleWindow)
                return;

            foreach (Int32 subbandid in _BaseNoiseSubbandIds)
            {
                Boolean tmp = (_BaseNoiseTrueCntTable.TryGetValue(subbandid, out Int32 cnt) ? cnt : 0) >= _BaseNoiseTrueThreshold;
                if (SubbandEnergyTable.ContainsKey(subbandid) && SubbandEnergyTable[subbandid] != tmp)
                {
                    Trace.WriteLine($"AcqBdReg.R.SenceFFT_sence_fft_out_valid changed({SubbandEnergyTable[subbandid]}->{tmp})");
                }
                SubbandEnergyTable[subbandid] = tmp;
            }

            _BaseNoiseSampleCount = 0;
            _BaseNoiseTrueCntTable.Clear();
            _BaseNoiseSubbandIds.Clear();
        }
    }
}

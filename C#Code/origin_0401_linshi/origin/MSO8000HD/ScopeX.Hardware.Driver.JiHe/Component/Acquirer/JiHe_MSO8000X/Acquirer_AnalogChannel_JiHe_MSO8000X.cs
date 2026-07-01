//#if JiHe_MSO7000X
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.MathExt;
using System.Drawing.Imaging;
using Ivi.Visa;
using System.Threading.Channels;
using ScopeX.Hardware.Driver.Module;
using ScopeX.Hardware.Driver.Registers.SendManage;

namespace ScopeX.Hardware.Driver
{
    /// <summary>
    /// 模拟通道波形采集
    /// </summary>
    public partial class Acquirer_AnalogChannel_JiHe_MSO8000X : AbstractAcquirer_AnalogChannel
    {
        private readonly IReadOnlyList<KeyValuePair<String, Int32>> _PerAdcStorageLength = new List<KeyValuePair<String, Int32>>()
        {
            new KeyValuePair<String, Int32>("Auto", 1250),
            //new KeyValuePair<String, Int32>("50kPts", 50_000),
            new KeyValuePair<String, Int32>("500kPts", 500_000),
            new KeyValuePair<String, Int32>("5MPts", 5_000_000),
            new KeyValuePair<String, Int32>("50MPts", 50_000_000),
            new KeyValuePair<String, Int32>("500MPts", 500_000_000),
            //new KeyValuePair<String, Int32>("1GPts", 1_000_000_000),
        };
        public override IReadOnlyList<KeyValuePair<String, Int32>> PerAdcStorageLength { get => _PerAdcStorageLength; }
        public override AdcInterleaveMode GetCurrentMode()
        {
            var activechnls = ChannelIdExt.GetAnalogs().Where(id => Hd.UIMessage!.Analog![(Int32)id].Active).ToList();
            if (activechnls.Count == 0)
                return Hd.UIMessage!.Timebase!.InterleaveMode;

            Int32 c1andc3count = activechnls.Where(c => c == ChannelId.C1 || c == ChannelId.C3).Count();

            if (activechnls.Count == 1 && c1andc3count == 1)//20G
            {
                return AdcInterleaveMode.Mode2To1;
            }
            else if (activechnls.Count == 2 && c1andc3count == 2)//20G
            {
                return AdcInterleaveMode.Mode2To1;
            }
            else
            {
                return AdcInterleaveMode.Mode1To1;
            }
        }

        internal Acquirer_AnalogChannel_JiHe_MSO8000X() : base()
        {
            this.AnalogAcquireModel = new AnalogAcquireModule_Jihe_MSO8000X();
            InitCaliInfo();
            _StorageDotsCntPerCore = _PerAdcStorageLength.Select(o => o.Value*2).ToArray();
        }

        private void InitCaliInfo()
        {
            //DataStruct refactoring: Mso8000 Init
            TiadcPhaseOffsetGainParams.Default.GodVersion = TiadcGodVersionEnum.Test;
            TiadcPhaseOffsetGainParams.Default.ItemVersion = TiadcItemVersionEnum.Base;
            ProductDataTranslate_MSO8000X.GetAllTiadcParamsKeys((AnalogAcquireModule_Jihe_MSO8000X)this.AnalogAcquireModel!)
                .ForEach(tiadcKey =>   TiadcPhaseOffsetGainParams.Default[tiadcKey] = new TiadcPhaseOffsetGainItem_Base() { Gain = 0xA000, Phase = 32000 });

            AnalogChannelParams.Default.GodVersion = AnalogGodVersionEnum.Base;
            AnalogChannelParams.Default.ItemVersion = AnalogItemVersionEnum.Base;
            ProductDataTranslate_MSO8000X.GetAllChnlParamsKeys().ForEach(chnlKey =>
                AnalogChannelParams.Default[chnlKey] = new AnalogChannelItem_Base() { Bias = 32000, Offset = 31500, Gain = 6, Gain_FineByFpgaThousand = 1000 });

            CoefficientsParams.Default["AmpFreq_C1_100mV"] = new Double[] { 1, 2, 3 };
        }

        internal override ChannelBdAdcInputDefine? GetChannelAcqBdAdcInputCorresponding(Int32 channelIndex)
        {
            var currdefine = AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;
            if(currdefine.Details.Keys.Contains((ChannelId)channelIndex))
            {
                var adcinfos = currdefine.Details[(ChannelId)channelIndex];
                if (adcinfos != null)
                {
                    Int32 boardid = (Int32)(adcinfos![0]!.AcqBdNo);
                    return new ChannelBdAdcInputDefine() { BdNo = Boadr_Acq_JiHe_MSO8000X.GetAcqBdNo(boardid) };
                }
            }
            return null;
        }

        internal override UInt64 TryGetSaveDataSegementDotsLength() => 25_000_000L;


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
            (UInt32 Base, UInt32 Multiple) extractsplitnum = Extract_JiHe_MSO8000X.GetPreSeperateNum(extracttotalnum);

            //Gap值下发需要，除法器转换乘法器
            var precoevalue = UInt32.MaxValue / extractsplitnum.Multiple;
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_HrCoeL16, (UInt32)(precoevalue & 0xffff));
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_HrCoeH16, (UInt32)((precoevalue >> 16) & 0xffff));

            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapX, extractsplitnum.Base);/*extractsplitnum.Base/2*/
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueL16, (UInt32)(extractsplitnum.Multiple & 0xffff));
            Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.Decimation_PreGapValueH16, (UInt32)((extractsplitnum.Multiple >> 16) & 0xffff));
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapX, extractsplitnum.Base);
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueL16, (UInt32)(extractsplitnum.Multiple & 0xffff));
            HdIO.WriteReg(ProcBdReg.W.TrigCtrl_ProPreGapValueH16, (UInt32)((extractsplitnum.Multiple >> 16) & 0xffff));

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
            return Extract_JiHe_MSO8000X.GetValidPreExtractNum((UInt64)preextramnum);
        }

        //扫描档时抽取档的抽取比
        private const UInt64 ScanDecimationExtractNum = 20_000;

        /// <summary>
        /// 创建采集板采集数据的属性，主要配置AcquingParameters
        /// </summary>
        internal override void CreateAcquireAttribute()
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
                _ => (100E-6, 50_000),//Mode1To1
            };
            AcquingParameters.ExtractNumFromAdc = CalcPreExtractNum(sampleinfo.AdcSampleIntervalUs);

            /*scan档抽取配置*/
            AcquingParameters.Scan2ExtractNum_Total = Math.Max(AcquingParameters.HardwareStorageWaveDotsCnt / sampleinfo.UIPoints, 1);
            AcquingParameters.Scan2ExtractNum_Base = 1;
            AcquingParameters.Scan2ExtractNum_Multiple = 1;

     //       acquireAttribute.PerDataByfs_AtDdr_pre =
            AcquingParameters.PerDataByfs_AtDdr = sampleinfo.AdcSampleIntervalUs  * AcquingParameters.ExtractNumFromAdc * uS2fs;

            //抽取数适配：DDR使用后抽，Fifo使用L3的前抽
            UInt64 extractnum = AcquingParameters.ExtramNumToDMA;
            if(Hd.UIMessage!.Timebase!.IsScan && (!Hd.UIMessage.bAcquireStopped))
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
        internal override Boolean IsNeedPostProcessByMatlab => false;

        /// <summary>
        /// 通道开关变更需要做的操作
        /// </summary>
        internal override void AnalogChannelActiveChanged()
        {
            Hd.CurrProduct!.AcqBd!.ConfigAdc();
        }

        /// <summary>
        /// 获取adc采集到的波形
        /// </summary>
        /// <param name="adcsData"></param>
        /// <returns></returns>
        public override bool TakeAdcWaveform(out List<List<ushort>> adcsData)
         {
            var adcsDataArray = new List<ushort>[16]
            {
                new List<ushort>(),
                new List<ushort>(),
                new List<ushort>(),
                new List<ushort>(),
                new List<ushort>(),
                new List<ushort>(),
                new List<ushort>(),
                new List<ushort>(),

                new List<ushort>(),
                new List<ushort>(),
                new List<ushort>(),
                new List<ushort>(),
                new List<ushort>(),
                new List<ushort>(),
                new List<ushort>(),
                new List<ushort>()
            };

            Monitor.Enter(AcqedDataPool.UpdateDataLock);
            #region chnlData to adc
            //获取当前的交织模式
            var analogAcquireModel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
            AcqModeAndInterleaveDefine interDefine = analogAcquireModel.GetCurrentAcqModeInterleave()!;

            //4个通道循环
            for (int channelId = (int)ChannelId.C1; channelId < ChannelIdExt.AnaChnlNum; channelId++)
            {
                //获取当前通道是否打开
                if (interDefine.Details.Keys.Contains((ChannelId)channelId))
                {
                   
                    foreach (var item in interDefine.Details[(ChannelId)channelId])
                    {
                        //MSO8000X一个通道有且只有一个采集板的Adc
                        //var adcInfo = interDefine.Details[(ChannelId)channelId][0];
                        var adcInfo = item;
                        int acqNumofchannel = analogAcquireModel.GetUsedAdcs(adcInfo).Count* interDefine.Details[(ChannelId)channelId].Length;
                        int acqNumofBoard = analogAcquireModel.GetUsedAdcs(adcInfo).Count;
                        var chnlDataBuffer = AcqedDataPool.AnalogChData.AllChannelData[channelId].ToArray();
                        //把通道数据拆分到Adc里面去
                        for (int adcDataIndex = 0; adcDataIndex < chnlDataBuffer.Length / acqNumofchannel; adcDataIndex++)
                        {
                            for (int acqId = 0; acqId < acqNumofBoard; acqId++)
                            {
                                ushort data = chnlDataBuffer[adcDataIndex * acqNumofchannel + (((Int32)adcInfo.AcqBdNo)% acqNumofBoard)* acqNumofBoard+ acqId];
                                //int adcIndex = (int)analogAcquireModel.GetAcqUintIndex(adcInfo, acqId)!;
                                int adcIndex = ((int)adcInfo.AcqBdNo * Constants.ADC_NUM + acqId);
                                switch (adcIndex)
                                {
                                    //case 0: adcIndex = 1; break;
                                    //case 1: adcIndex = 3; break;
                                    //case 2: adcIndex = 0; break;
                                    //case 3: adcIndex = 2; break;
                                    //case 4: adcIndex = 5; break;
                                    //case 5: adcIndex = 7; break;
                                    //case 6: adcIndex = 4; break;
                                    //case 7: adcIndex = 6; break;
                                    //case 8: adcIndex = 9; break;
                                    //case 9: adcIndex = 11; break;
                                    //case 10: adcIndex = 8; break;
                                    //case 11: adcIndex = 10; break;
                                    //case 12: adcIndex = 13; break;
                                    //case 13: adcIndex = 15; break;
                                    //case 14: adcIndex = 12; break;
                                    //case 15: adcIndex = 14; break;
                                    case 0: adcIndex = 2; break;
                                    case 1: adcIndex = 0; break;
                                    case 2: adcIndex = 3; break;
                                    case 3: adcIndex = 1; break;
                                    case 4: adcIndex = 6; break;
                                    case 5: adcIndex = 4; break;
                                    case 6: adcIndex = 7; break;
                                    case 7: adcIndex = 5; break;
                                    case 8: adcIndex = 10; break;
                                    case 9: adcIndex = 8; break;
                                    case 10: adcIndex = 11; break;
                                    case 11: adcIndex = 9; break;
                                    case 12: adcIndex = 14; break;
                                    case 13: adcIndex = 12; break;
                                    case 14: adcIndex = 15; break;
                                    case 15: adcIndex = 13; break;
                                    default:
                                        break;
                                }
                                adcsDataArray[adcIndex].Add(data);
                            }
                        }
                    }
                 
                }
            }
            #endregion chnlData to Core
            Monitor.Exit(AcqedDataPool.UpdateDataLock);
            adcsData = adcsDataArray.ToList();
            return true;
        }

        internal override Boolean TryGetWaveByteSize(String waveName, ref Int32 byteSize)
        {
            var waves = TryGetWaveData_Adc(waveName);
            byteSize = waves != null ? waves.Count * 2 : 0;
            return true;
        }

        internal override List<UInt16>? TryGetWaveData_Adc(String waveName)
        {
            string[] param = waveName.Split("_");
            var analogAcquireModel = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
            AcqModeAndInterleaveDefine interDefine = analogAcquireModel.GetCurrentAcqModeInterleave()!;
            if (param[0] != interDefine.Name)
            {
                return null;
            }
            int adcId = int.Parse(param[2].Replace("Adc", ""));
            TakeAdcWaveform(out List<List<ushort>> adcsData);
            List<ushort> adcData = new List<ushort>();
            foreach (var item in interDefine.Details)
            {
                int chaneIndex = 0;
                switch (item.Key)
                {
                    case ChannelId.C3:
                    case ChannelId.C4:
                        chaneIndex = 2;
                        break;

                }
                foreach (var Adcindex in item.Value.First().AdcPorts)
                {
                    int index = chaneIndex + Adcindex.Key;
                    adcData.AddRange(adcsData[index]);
                }
            }
            return adcData;
        }

        internal override bool ReadAcqData(List<ReadInfo> readInfoList, out double samplingRateByus, CancellationToken? softResetToken)
        {
            bDataVaild = false;
            bool bOk = false;
            if (Hd.UIMessage == null)
            {
                samplingRateByus = 1.0;
                return bOk;
            }
            
            samplingRateByus = AcquedParameters.PerDataByfs_AtDdr * 1e-9;

            /*模拟数据*/
            if (HdIO.CurrDriver == null || !HdIO.CurrDriver.bOpen || Hd.BPowerOff)
            {
                if (!Acquisition.bReadOldData)
                    return true;
                if (Hd.UIMessage?.Timebase?.IsScan ?? false)
                {
                    Random random = new Random();
                    int a = random.Next(100, 1000);
                    AcqAnalogChannelSimulateWaveform();
                    for (int iChannelID = 0; iChannelID < ChannelIdExt.AnaChnlNum; iChannelID++)
                    {
                        AcqedDataPool.AnalogChData.AllChannelData[iChannelID].RemoveRange(a, AcqedDataPool.AnalogChData.AllChannelData[iChannelID].Count - a);
                    }
                    return true;
                }
                else
                {
                    return AcqAnalogChannelSimulateWaveform();
                }
            }
            /*真实数据*/
            if (!AcquingParameters.bIsLongStorageMode)
            {
                return _FifoReadAcq?.Invoke(readInfoList, softResetToken) ?? false;
            }
            else
            {
                return _LongStorageReadAcq?.Invoke(readInfoList, softResetToken) ?? false;
            }
        }

        internal override void PostProcess(List<ReadInfo> readInfoList, CancellationToken? softResetToken)
        {
            base.PostProcess(readInfoList, softResetToken);
            ArtificialIntelligenceProcess.Default.MultiDomainProcess.Run();
        }

        protected override int GetInterpolateValideNum(int originInterpolate)
        {
            return Interp_JiHe_MSO8000X.GetValideNum(originInterpolate);
        }

        protected override UInt32 GetInterpolateValideValue(int num)
        {
            return Interp_JiHe_MSO8000X.GetValideValue(num);
        }

        private record AfcCoedDataInfo(AcqModeAndInterleaveDefine define, ChannelId chnlId, int scaleIndex, Int32[]? caliData);
        private List<AfcCoedDataInfo> AfcCoedDataInfos = new List<AfcCoedDataInfo>();
        internal override void SendCoefficients_Afc(CoefficientsTableType coefficientsTableType, bool bForce)
        {
            var anaAcquire = (AnalogAcquireModule_Jihe_MSO8000X)Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!;
            //初始化校准数据
            if (AfcCoedDataInfos.Count == 0)
            {
                //遍历模式，通道定义，垂直挡位
                foreach (var modeDefine in anaAcquire.AcqModeAndInterleaveDefineTable)
                {
                    int frequencyG = (modeDefine.Value.InterleaveMode == AdcInterleaveMode.Mode1To1) ? 10 : 20;
                    foreach (var dtl in modeDefine.Value.Details)
                    {
                        foreach (var ylevel in AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV)
                        {
                            int yScaleMv = ylevel / 1000;
                            Int32[]? coefficients = Misc.ReadCaliCoefDataFronmFile($".\\CaliData\\CoeFiles\\Afc_generation_result_" +
                                $"{frequencyG}G_ch{(int)(dtl.Key + 1)}_{yScaleMv}mv.txt");
                            AfcCoedDataInfos.Add(new AfcCoedDataInfo(modeDefine.Value, dtl.Key, yScaleMv, coefficients));
                        }
                    }
                    //foreach (var item in modeDefine.Value.Details)
                    //{
                    //    AfcCoedDataInfos.Add(new AfcCoedDataInfo(modeDefine.Value, item.Key, 200, CoefficientsTables.Default[coefficientsTableType, (int)item.Key]));
                    //}
                }
            }

            var define = anaAcquire.GetCurrentAcqModeInterleave()!;

            //下发校准数据
            foreach (var chDtl in define.Details!)
            {
                //MSO8000X一个通道有且只有一个采集板的Adc
                var adcInfo = chDtl.Value[0];
                var scale = Hd.UIMessage!.Analog![(int)chDtl.Key].Scale;
                var dataArray = AfcCoedDataInfos.FirstOrDefault(info =>
                {
                    //if (info.define == define && info.chnlId == chDtl.Key && info.scaleIndex == scale)
                    if (info.define == define && info.chnlId == chDtl.Key)
                        return true;
                    return false;
                })?.caliData;

                if (dataArray != null)
                {
                    if (Hd.CurrProduct!.HardwareConfig!.DownloadBlockDataMode == DownloadBlockDataMode.DMA)
                        Afc_Sender_ByDMAMode(coefficientsTableType, dataArray!, adcInfo.AcqBdNo, chDtl.Key);
                    else
                        Afc_Sender_ByRegisterMode(coefficientsTableType, dataArray!, adcInfo.AcqBdNo, chDtl.Key);

                    //抽取档，关闭Afc
                    if (AcquingParameters.ExtractNumFromAdc > 1)
                        Hd.CurrProduct!.AcqBd!.WriteToAllFpga(AcqBdReg.W.Afc_AfcFilterEn, 0);
                }
                else
                {
                    //没有Afc系数，关闭Afc
                    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Afc_AfcFilterEn, adcInfo.AcqBdNo, 0);
                }
            }

            return;
        }

        internal override bool Afc_Sender_ByRegisterMode(CoefficientsTableType coefficientsTableType, int[] dataArray, AcqBdNo acqBdNo, ChannelId channelId)
        {
            int partALength = Hd.CurrProduct!.HardwareConfig!.LocalCoefficientsTableMeanings[coefficientsTableType].LengthOfPartA;

            int dataCount = dataArray.Length;
            for (int i = 0; i < dataCount; i++)
            {
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Afc_AfcCoeffWRriteEn, acqBdNo, 0);
                if (i < partALength) //10G系数  
                    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Afc_AfcCoeffWRriteAddr, acqBdNo, (UInt32)i);
                else       //20G系数(修改系数长度后需要修改此处)
                    Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Afc_AfcCoeffWRriteAddr, acqBdNo, (UInt32)(i + 0b00100000000 - partALength));//(适用200阶)下发地址11位，通过高位为1下发的则是20G的系数
                Int32 data = dataArray[i];
                //低16位
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Afc_AfcCoeffWRriteDataLow16, acqBdNo, (UInt32)data & 0xffff);
                //高位
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Afc_AfcCoeffWRriteDataH1, acqBdNo, (UInt32)(data >> 16) & 0xff);
                HdIO.DelayByUs(10);
                Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Afc_AfcCoeffWRriteEn, acqBdNo, 1);

            }
            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Afc_AfcCoeffWRriteEn, acqBdNo, 0);

            Hd.CurrProduct!.AcqBd!.WriteReg(AcqBdReg.W.Afc_AfcFilterEn, acqBdNo, 1);
            return true;
        }
    }

    /// <summary>
    /// MSO8000的抽取模块
    /// </summary>
    internal class Extract_JiHe_MSO8000X
    {
        private const UInt32 PREBASENUM = 100;//前抽第一级最大倍率
        private const UInt32 POSTBASENUM = 10;//后抽第一级最大倍率

        /// <summary>
        /// 获取有效的前抽倍率
        /// </summary>
        /// <param name="expectedExtramNum">期望的前抽倍率</param>
        /// <returns></returns>
        internal static UInt64 GetValidPreExtractNum(UInt64 expectedExtramNum)
        {
            UInt32[] baseextractnum = _PreExtractTable.Keys.ToArray();
            return GetValidExtractNum(expectedExtramNum, baseextractnum, PREBASENUM);
        }

        /// <summary>
        /// 获取有效的后抽倍率
        /// </summary>
        /// <param name="expectedExtramNum">期望的后抽倍率</param>
        /// <returns></returns>
        internal static UInt64 GetValidPostExtractNum(UInt64 expectedExtramNum)
        {
            UInt32[] baseextractnum = _PostExtractTable.Keys.ToArray();
            return GetValidExtractNum(expectedExtramNum, baseextractnum, POSTBASENUM);
        }

        /// <summary>
        /// 获取有效的抽取倍率
        /// </summary>
        /// <param name="expectedExtramNum">期望的后抽倍率</param>
        /// <param name="baseExtractNum">第一级抽取定义表</param>
        /// <param name="baseMum">第一级抽取最大倍率</param>
        /// <returns></returns>
        private static UInt64 GetValidExtractNum(UInt64 expectedExtramNum, UInt32[] baseExtractNum, UInt32 baseMum)
        {
            if (expectedExtramNum <= baseExtractNum.Min())
                return baseExtractNum.Min();

            if (expectedExtramNum <= PREBASENUM && expectedExtramNum <= baseExtractNum.Max())
            {
                if (baseExtractNum.Contains((UInt32)expectedExtramNum))
                    return expectedExtramNum;

                for (Int32 i = 0; i < baseExtractNum.Length - 1; i++)
                {
                    if (baseExtractNum[i] < expectedExtramNum && baseExtractNum[i + 1] > expectedExtramNum)
                    {
                        return baseExtractNum[i + 1];
                    }
                }
            }

            UInt64 validnum = expectedExtramNum / baseMum * baseMum;
            if (validnum == expectedExtramNum)
                return expectedExtramNum;
            return (validnum + baseMum);
        }

        /// <summary>
        ///  FPGA前抽模块，抽取倍数及下发参数定义表
        /// </summary>
        private static Dictionary<UInt32, UInt32> _PreExtractTable = new()
        { 
            // 抽取倍数   下发参数
            {0,             1 },
            {1,             1 },
            {2,             2 },
            {4,             4 },
            {10,            0x20 },
            {20,            0x80 },
            {40,            0x200 },
            {100,           0x800 },
        };

        /// <summary>
        /// 获取前抽的两级下发参数
        /// </summary>
        /// <param name="extramNum">总的前抽倍率</param>
        /// <returns>(base=下发参数, multiple=抽取倍数)</returns>
        internal static (UInt32, UInt32) GetPreSeperateNum(UInt64 extramNum)
        {
            if (extramNum < PREBASENUM)
            {
                if (_PreExtractTable.ContainsKey((UInt32)extramNum))
                    return (_PreExtractTable[(UInt32)extramNum], 1);
                return (1, 1);
            }
            return (_PreExtractTable[PREBASENUM], (UInt32)(extramNum / PREBASENUM));
        }

        /// <summary>
        /// FPGA后抽模块，抽取倍数及下发参数定义表
        /// </summary>
        internal static Dictionary<UInt32, UInt32> _PostExtractTable = new()
        { 
                // 抽取倍数   下发参数
                {0,             1 },
                {1,             1 },
                {2,             2 },
                {4,             4 },
                {5,             8 },
                {8,             0x10 },
                {10,            0x20 },
                {25,            0x100 },
        };

        /// <summary>
        /// 获取后抽的两级下发参数
        /// </summary>
        /// <param name="extramNum">总的后抽倍率</param>
        /// <returns>(base=下发参数, multiple=抽取倍数)</returns>
        internal static (UInt32, UInt32) GetPostSeperateNum(UInt64 extramNum)
        {
            if (_PostExtractTable.ContainsKey((uint)extramNum))
            {
                return (_PostExtractTable[(uint)extramNum], 1);
            }
            return (_PostExtractTable[POSTBASENUM], (UInt32)(extramNum / POSTBASENUM));
        }

    }

    internal class Interp_JiHe_MSO8000X
    {
        private static Dictionary<Int32, UInt32> _InterpNumTable = new()
        {
            {1, 0x0101 },
            {2, 0x0102 },
            {5, 0x0105 },
            {10, 0x010a },
            {20, 0x020a },
            {50, 0x050a },
            {100, 0x0a0a },
            {200, 0x140a },
        };

        public static Int32 GetValideNum(Int32 originInterpolate)
        {
            foreach (var item in _InterpNumTable)
            {
                if (item.Key >= originInterpolate)
                    return item.Key;
            }
            return _InterpNumTable.Last().Key;
        }

        public static UInt32 GetValideValue(Int32 num)
        {
            if (_InterpNumTable.ContainsKey(num))
            {
                return _InterpNumTable[num];
            }
            return _InterpNumTable.First().Value;
        }
    }
}
//#endif
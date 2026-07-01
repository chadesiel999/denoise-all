using MathWorks.MATLAB.NET.Arrays;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using static ScopeX.Hardware.Driver.CaliStateManager;
using static ScopeX.Hardware.Driver.CtrlAnalogChannel_DBI20G;
using static System.Formats.Asn1.AsnWriter;
using TiadcParamsKeyMap = ScopeX.Hardware.Driver.ProductDataTranslate_MSO8000X.TiadcParamsKeyMap;

namespace ScopeX.Hardware.Driver.Calibration
{
    internal class DBIAutoCali
    {
        private static void InitDebugVarints_LocalOscillatorCoefficients()
        {
            Hd.CurrDebugVarints.bEnable_AmplitudeTemperatureCompensate = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime = false;
            Hd.CurrDebugVarints.bEnable_AtStartScanAdcRxWindows = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanWindow = false;

            Hd.CurrDebugVarints.bEnable_AdcDataDebugMode = false;
            Hd.CurrDebugVarints.bEnable_DigitTrigger = false;
            Hd.CurrDebugVarints.bEnable_AcqbdInterpolation = false;
            Hd.CurrDebugVarints.bEnable_AcqBd_Afc = false;
            Hd.CurrDebugVarints.bEnable_AcqBd_Pfc = false;

            Hd.CurrDebugVarints.bEnable_CorrectTiAdc = true;

            Hd.CurrDebugVarints.bEnable_Dbi_IntDelay = true;
            Hd.CurrDebugVarints.bEnable_Dbi_AmpFreqCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_AntiImageCoef = true;
            Hd.CurrDebugVarints.bEnable_Dbi_FractionaryDelayCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_LocalOscillatorCoef = true;
            Hd.CurrDebugVarints.bEnable_Dbi_MultiRadioInterpolation = true;
            Hd.CurrDebugVarints.bEnable_Dbi_OverlapPhaseFreqDelayCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_PhaseFreqCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode = false;

            Hd.CurrDebugVarints.bEnable_ProcBd_Average = false;
        }
        public static void LocalOscillatorCoefficients()
        {
            ChangeAnalogConfig(100000, 1000);
            //MatlabDll_LocalCoe matlabDll_LocalCoe = new();

            List<ChannelId> includeChannels = new List<ChannelId>() { ChannelId.C1};//通过此控制现在的硬件已经插上几个物理通道
            List<ChannelId> includeChannelsForCali = new List<ChannelId>() { ChannelId.C1 };
            //#region Step0:参数备份
            //Hd.CurrDebugVarints.DoBackup();
            //HdMessage backHdMessage = Hd.UIMessage! with { };//原始参数备份
            //#endregion Step0:参数备份

            #region Step1:硬件参数调整控制
            InitDebugVarints_LocalOscillatorCoefficients();

            //List<HdMessage.AnalogOptions> newAnalogOptions = new List<HdMessage.AnalogOptions>();
            //foreach (var channel in includeChannels)
            //{
            //    double scaleValueBymV = 50;
            //    int PositionIndex = 0;
            //    HdMessage.AnalogOptions analogOption = new HdMessage.AnalogOptions(true, (int)AnaChnlScaleIndex.Lv50m, PositionIndex) { Bandwidth = 2, Bias = 0, Coupling = AnaChnlCoupling.DC50, IsInverted = false, ProbeIndex = AnaChnlProbe.x1, InputSource = AnaChnlIpnutSource.BNC, InterChannelOffset = 0, Scale = scaleValueBymV, ScaleIndex = (int)AnaChnlScaleIndex.Lv50m, Position = PositionIndex * scaleValueBymV, PositionIndex = PositionIndex };
            //    newAnalogOptions.Add(analogOption);
            //}

            //HdMessage currHdMessage = Hd.UIMessage! with { Timebase = Hd.UIMessage.Timebase! with { IsScan = false, StorageWaveDotsCnt = 10_000, TmbPositionIndex = 0, TmbScaleIndex = (int)AnaChnlTimebaseIndex.Lv5n, TmbScale = TimebaseTableByus.Table[AnaChnlTimebaseIndex.Lv5n].Scale, TmbPosition = 0, } }; // AnaChnlTimebaseIndex.Lv10n， AnaChnlTimebaseIndex.Lv10n
            //currHdMessage = currHdMessage with { Analog = newAnalogOptions.ToArray() };
            ////HdMessage currHdMessage = Hd.UIMessage! with { Timebase = Hd.UIMessage.Timebase! with { IsScan = false, StorageWaveDotsCnt = 10_000, TmbPositionIndex = 0, TmbScaleIndex = (int)AnaChnlTimebaseIndex.Lv5n, TmbScale = TimebaseTableByus.Table[AnaChnlTimebaseIndex.Lv5n].Scale, TmbPosition = 0, } };
            //Hd.LocalCommands |= (long)HdCmd.TmbScaleIndex;

            //Hd.Execute(currHdMessage);
            //Thread.Sleep(50);

            #endregion

            #region Setp2:各通道、各子带的频率点
            Dictionary<ChannelId, Dictionary<Int32/*子带编号*/, Int32 /*频率点，用MHz为单位*/>> perChannelPerSubbandFreqByMz = new Dictionary<ChannelId, Dictionary<int, int>>
            {
                {
                    ChannelId.C1,//其定义必须包含includeChannels中通道
                    new Dictionary<int, int>
                    {
                        {0,0 },//必须包含第一子带
                        {1,6_000 },//后续的子带编号必须连续
                        {2,9_500 },//后续的子带编号必须连续
                        {3,14_500 },//后续的子带编号必须连续
                    }
                 }
            };

            Dictionary<int, int> PerSubbandLocalFreqByMhz = new Dictionary<int, int>
            {
                [0] = 0,
                [1] = 10_000,
                [2] = 15_000,
                [3] = 22_500,
            };

            Dictionary<AnaChnlScaleIndex, Dictionary<int, double>> DeltaOverlapPhase = new Dictionary<AnaChnlScaleIndex, Dictionary<int, double>>
            {
                {
                    AnaChnlScaleIndex.Lv50m,
                    new Dictionary<int, double>
                    {
                        [0] = 0.0,
                        [1] = -0.64,
                        [2] = -0.58,
                        [3] = -0.65,
                    }
                },
                {
                    AnaChnlScaleIndex.Lv100m,
                    new Dictionary<int, double>
                    {
                        [0] = 0.0,
                        [1] = -0.64,
                        [2] = -0.58,
                        [3] = -0.65,
                    }
                },
                {
                    AnaChnlScaleIndex.Lv200m,
                    new Dictionary<int, double>
                    {
                        [0] = 0.0,
                        [1] = -0.64,
                        [2] = -0.58,
                        [3] = -0.65,
                    }
                },                {
                    AnaChnlScaleIndex.Lv500m,
                    new Dictionary<int, double>
                    {
                        [0] = 0.0,
                        [1] = -0.64,
                        [2] = -0.58,
                        [3] = -0.65,
                    }
                },
                {
                    AnaChnlScaleIndex.Lv10m,
                    new Dictionary<int, double>
                    {
                        [0] = 0.0,
                        [1] = -0.84,
                        [2] = -0.77,
                        [3] = -1.01,
                    }
                },
                {
                    AnaChnlScaleIndex.Lv20m,
                    new Dictionary<int, double>
                    {
                        [0] = 0.0,
                        [1] = -0.46,
                        [2] = -0.26,
                        [3] = -0.36,
                    }
                }
            };

            //前端路径不一致导致的相位残留
            //Dictionary<int, double> DeltaOverlapPhase_20mv = new Dictionary<int, double>
            //{
            //    [0] = 0.0,
            //    [1] = -0.46,
            //    [2] = -0.28,
            //    [3] = -0.36,
            //};

            int coeLength = 800;

            //初始化本振初相为0
            foreach (var channelID in includeChannelsForCali)
            {
                int totalSubbandCount = perChannelPerSubbandFreqByMz[channelID].Values.Count;
                for (int subbandID = 1; subbandID < totalSubbandCount; subbandID++)
                {
                    int localFreqByMHz = PerSubbandLocalFreqByMhz[subbandID];
                    double phaseDiff = 0;

                    string cfgparam = $"{(XunXin40G_SampleFreqPerSubbandByMsps * 1.0) / 1000} {phaseDiff.ToString("0.000")} {(localFreqByMHz * 1.0) / 1000} 16 {coeLength}";
                    string filepath = "111";
                    string filename = "111";
                    string DebugSaveFilePath = "111";
                    string DebugSaveFilePrefixName = "111";
                    MatlabDll? calc_local_coe = Hd.CurrProduct.MatlabDlls["MatlabGenerateOverlapBandSync_LoCoe.dll"];
                    MWArray[] paramArray = new MWArray[]
                    {
                        filepath,
                        filename,
                        DebugSaveFilePath,
                        DebugSaveFilePrefixName,
                        cfgparam,
                    };
                    Trace.WriteLine($"[CalcLocalCoe]cfgparam:{cfgparam}");
                    MWNumericArray? dataout_dbi = (MWNumericArray?)calc_local_coe.Method.Invoke(calc_local_coe.Instance, paramArray);
                    double[] lo_coe = (double[])((MWNumericArray)dataout_dbi!).ToVector(MWArrayComponent.Real);

                    if (lo_coe.Length != 0)
                        SaveAndSendCoe(lo_coe, DbiCoefficientsTablesType.LocalOscillatorCoefficients, channelID, subbandID);
                }
            }
            #endregion Setp2:各通道、各子带的频率点

            #region Step3:计算
            foreach (var channelID in includeChannelsForCali)
            {
                int totalSubbandCount = perChannelPerSubbandFreqByMz[channelID].Values.Count;
                for (int subbandID = 1; subbandID < totalSubbandCount; subbandID++)
                {
                    int freqByMHz = perChannelPerSubbandFreqByMz[channelID][subbandID];
                    int localFreqByMHz = PerSubbandLocalFreqByMhz[subbandID];
                    double phaseDiff = CalcChannelSubbandPhaseDiff(channelID, subbandID, freqByMHz);
                    
                    foreach (var key in DeltaOverlapPhase.Keys) 
                    {
                        var DeltaOverlapPhaseS = DeltaOverlapPhase[key];
                        double phaseDiff_new = phaseDiff + DeltaOverlapPhaseS[subbandID];
                        string cfgparam = $"{(XunXin40G_SampleFreqPerSubbandByMsps * 1.0) / 1000} {phaseDiff_new.ToString("0.000")} {(localFreqByMHz * 1.0) / 1000} 16 {coeLength}";
                        string filepath = "111";
                        string filename = "111";
                        string DebugSaveFilePath = "111";
                        string DebugSaveFilePrefixName = "111";
                        MatlabDll? calc_local_coe = Hd.CurrProduct.MatlabDlls["MatlabGenerateOverlapBandSync_LoCoe.dll"];
                        MWArray[] paramArray = new MWArray[]
                        {
                            filepath,
                            filename,
                            DebugSaveFilePath,
                            DebugSaveFilePrefixName,
                            cfgparam,
                        };
                        Trace.WriteLine($"[CalcLocalCoe]cfgparam:{cfgparam}");
                        MWNumericArray? dataout_dbi = (MWNumericArray?)calc_local_coe.Method.Invoke(calc_local_coe.Instance, paramArray);
                        double[] lo_coe = (double[])((MWNumericArray)dataout_dbi!).ToVector(MWArrayComponent.Real);

                        //var ans = matlabDll_LocalCoe.CalcLocalCoe(phaseDiff, (XunXin40G_SampleFreqPerSubbandByMsps * 1.0)/1000, (localFreqByMHz * 1.0)/1000, coeLength);

                        if (lo_coe.Length != 0)
                            SaveAndSendCoeAllLevel(lo_coe, DbiCoefficientsTablesType.LocalOscillatorCoefficients, channelID, subbandID, key);
                    }
                    
                }
            }
            #endregion Step3:计算

            #region Step:Last 参数恢复
            //Hd.UIMessage = backHdMessage with { };
            //Hd.CurrDebugVarints.DoRestore();
            foreach (var channelID in includeChannels)
                Cali_ControlInterSource(false, channelID, DbiSunbbandCaliSourceEnum.SingleTone, 0, false);
            #endregion Step:Last 参数恢复
        }
        private static double CalcChannelSubbandPhaseDiff(ChannelId channelID, Int32 subbandID, Int32 freqByMHz)
        {
            Cali_ControlInterSource(true, channelID, DbiSunbbandCaliSourceEnum.SingleTone, freqByMHz, false);
            HdIO.Sleep(1000);
            Cali_ControlInterSource(true, channelID, DbiSunbbandCaliSourceEnum.SingleTone, freqByMHz, false);
            HdIO.Sleep(1000);

            #region  Step2: 多次采集，分别求出 求出与前一个相邻子带的相位，为统计计算做准备
            List<UInt16[]>? allWaveData = new List<ushort[]>();
            List<UInt16[]>? allAdcWaveData = new List<ushort[]>();

            int staticTimes = 60;
            List<double> phaseDiff = new List<double>();
            for (Int32 i = 0; i < staticTimes; i++)
            {
                if (i < 10) //fake read
                {
                    allWaveData?.Clear();
                    Hd.Calibration.AcqWaveData(out allWaveData);
                    allAdcWaveData = allWaveData;
                }
                else
                {
                    allWaveData?.Clear();
                    Hd.Calibration.AcqWaveData(out allWaveData);
                    //todo: 此处需要修改，需要根据DMA的数据格式和数量进行处理
                    allAdcWaveData = allWaveData;

                    var waveData = allAdcWaveData[((int)channelID * 4) + subbandID - 1].Take(6000).Select(o => (Int16)o).ToArray();
                    double lastPhase = SineFitFunc.SineFit(waveData, XunXin40G_SampleFreqPerSubbandByMsps, freqByMHz).Phase;
                    waveData = allAdcWaveData[((int)channelID * 4) + subbandID + 0].Take(6000).Select(o => (Int16)o).ToArray();
                    double thisPhase = SineFitFunc.SineFit(waveData, XunXin40G_SampleFreqPerSubbandByMsps, freqByMHz).Phase;

                    if (lastPhase - thisPhase >= 3.14159)
                    {
                        phaseDiff.Add(lastPhase - thisPhase - 6.28319);
                    }
                    else if (lastPhase - thisPhase <= -3.14159)
                    {
                        phaseDiff.Add(lastPhase - thisPhase + 6.28319);
                    }
                    else
                    {
                        phaseDiff.Add(lastPhase - thisPhase);
                    }
                }
            }
            #endregion
            #region Step3:平均后计算
            Double phasesin = phaseDiff.Select(o => Math.Sin(o)).Average();
            Double phasecos = phaseDiff.Select(o => Math.Cos(o)).Average();

            Double phase = Math.Atan2(phasesin, phasecos);
            #endregion

            return phase;
        }
        private static void SaveAndSendCoe(Double[] coeData, DbiCoefficientsTablesType coeType, ChannelId chnlId, Int32 subbandId)
        {
            var defineItem = Hd.CurrProduct.Acquirer_AnalogChannel?.GetAcqDefineItem(DbiCoefficientsTablesType.LocalOscillatorCoefficients, chnlId, subbandId);
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
            Thread.Sleep(10);
        }

        private static void SaveAndSendCoeAllLevel(Double[] coeData, DbiCoefficientsTablesType coeType, ChannelId chnlId, Int32 subbandId, AnaChnlScaleIndex key)
        {
            var defineItem = Hd.CurrProduct.Acquirer_AnalogChannel?.GetAcqDefineItemByLevel(DbiCoefficientsTablesType.LocalOscillatorCoefficients, chnlId, subbandId, key);
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
            Thread.Sleep(10);
        }

        public static Boolean XunXin40GAdc_SubbandDiscardDots(UInt16 discardAttempCount)
        {
            Boolean needDiscardAgain;

            ChangeAnalogConfig(50000, 1000);

            List<ChannelId> includeChannels = new List<ChannelId>() { ChannelId.C1};//通过此控制现在的硬件已经插上几个物理通道
            AnaChnlScaleIndex analogChnlScaleIndex = XunXin40G_LevelID_DiscardDotsStoreAt;

            #region Step0:参数备份与丢点数初始化
            string[] ref_discard_num_line = new string[0];

            // reference and temp discard number filename
            String ref_signal_100m_ref_discard_num_FileName;
            if (discardAttempCount == 0)
            {
                ref_signal_100m_ref_discard_num_FileName = AppDomain.CurrentDomain.BaseDirectory + @"recovery\ref_discard_num.txt";
            }
            else
            {
                ref_signal_100m_ref_discard_num_FileName = AppDomain.CurrentDomain.BaseDirectory + @"recovery\temp_discard_num.txt";
            }

            try
            {
                ref_discard_num_line = File.ReadAllLines(ref_signal_100m_ref_discard_num_FileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"参考丢点值获取发生错误：{ex.Message}");
            }

            int[] ref_discard_num_raw = ref_discard_num_line[0].Split(',').Select(int.Parse).ToArray();
            int[] ref_discard_num = ref_discard_num_raw;

            for (int channelID = 0; channelID < 1; channelID++)
            {
                for (int subbandID = 0; subbandID < 4; subbandID++)
                {
                    ChangeDBIAnalogParamsItem(0, channelID, (Int32)AnaChnlScaleIndex.Lv1, subbandID, Array.ConvertAll(ref_discard_num,x=>(double)x));

                    String keyStr = GetAnalogChannelKey((ChannelId)channelID, analogChnlScaleIndex, subbandID);
                    AcqBdNo acqBdNo = (AcqBdNo)(channelID * 4 + subbandID);
                    DbiAnalogChannelItem_Common currData = DbiAnalogParams_Common.Default[keyStr];
                    currData[(int)AnalogChannelItems_DBI20G.DiscardDots] = (int)ref_discard_num[subbandID];
                    DbiAnalogParams_Common.Default[keyStr] = currData;
                    Int32 discardDots = (Int32)DbiAnalogParams_Common.Default[keyStr][(int)AnalogChannelItems_DBI20G.DiscardDots];

                    //HdIO.WriteReg(ProcBdReg.W.DBI_IntDelayNumProCh1, (UInt32)discardDots);
                    //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DBI_IntDelayNum, acqBdNo, (UInt32)discardDots);
                }
            }
            //CtrlAnalogChannel_DBI20G.SendDiscard();
            #endregion


            #region Step1:硬件参数调整控制
            InitDebugVarints_XunXin40G_CaliDiscardDots();
            //List<HdMessage.AnalogOptions> newAnalogOptions = new List<HdMessage.AnalogOptions>();
            //foreach (var channel in includeChannels)
            //{
            //    double scaleValueBymV = 50;
            //    int PositionIndex = 0;
            //    HdMessage.AnalogOptions analogOption = new HdMessage.AnalogOptions(true, (int)AnaChnlScaleIndex.Lv50m, PositionIndex) { Bandwidth = 2, Bias = 0, Coupling = AnaChnlCoupling.DC50, IsInverted = false, ProbeIndex = AnaChnlProbe.x1, InputSource = AnaChnlIpnutSource.BNC, InterChannelOffset = 0, Scale = scaleValueBymV, ScaleIndex = (int)AnaChnlScaleIndex.Lv50m, Position = PositionIndex * scaleValueBymV, PositionIndex = PositionIndex };
            //    newAnalogOptions.Add(analogOption);
            //}

            //HdMessage currHdMessage = Hd.UIMessage! with { Timebase = Hd.UIMessage.Timebase! with { IsScan = false, StorageWaveDotsCnt = 10_000, TmbPositionIndex = 0, TmbScaleIndex = (int)AnaChnlTimebaseIndex.Lv5n, TmbScale = TimebaseTableByus.Table[AnaChnlTimebaseIndex.Lv5n].Scale, TmbPosition = 0, } }; // AnaChnlTimebaseIndex.Lv10n， AnaChnlTimebaseIndex.Lv10n
            //currHdMessage = currHdMessage with { Analog = newAnalogOptions.ToArray() };

            //Hd.LocalCommands |= (long)HdCmd.TmbScaleIndex;
            //Hd.LocalCommands |= (long)HdCmd.ChnlActive;

            //Hd.Execute(currHdMessage);
            //Thread.Sleep(50);
            if (Hd.UIMessage != null)
                Hd.Execute(Hd.UIMessage);
            Thread.Sleep(50);

            //Hd.CurrProduct?.AcqBd?.WriteToAllFpga(AcqBdReg.W.TrigCtrl_CompareVoltage1Up, 128);//2048
            #endregion Step1:硬件参数调整控制

            #region Step2:多次采集，求平均，并保存到文件，文件名称在EXE目录下的tempAcqData.txt
            Thread.Sleep(1000);
            Cali_ControlInterSource(true, ChannelId.C1, DbiSunbbandCaliSourceEnum.FastEdge, 50, true);//打开内部源输出
            Thread.Sleep(1000);
            Cali_ControlInterSource(true, ChannelId.C1, DbiSunbbandCaliSourceEnum.FastEdge, 50, true);//打开内部源输出
            Thread.Sleep(1000);

            // 定义两个List，用于当次采样数据存储和多幅波形的存储
            List<UInt16[]>? allWaveData = new List<ushort[]>();
            List<UInt16[]> validWaveData = new List<UInt16[]>();

            int staticTimes = 110;
            for (Int32 times = 0; times < staticTimes; times++)
            {
                allWaveData?.Clear();
                Hd.Calibration.AcqWaveData(out allWaveData);
                // 忽略前10次不稳定的采集
                if (times > 9)
                {
                    validWaveData.Add((ushort[])allWaveData[0].Clone());
                }
            }

            String fileName = AppDomain.CurrentDomain.BaseDirectory + @"recovery\tempAcqData.bin";
            if (File.Exists(fileName))
                File.Delete(fileName);
            try
            {
                using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                {
                    foreach (var singleWave in validWaveData)
                    {
                        foreach (var point in singleWave)
                        {
                            writer.Write(point);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("写入二进制文件时发生错误: " + ex.Message);
            }
            #endregion


            #region Step3:调MatlabDll 计算丢点数
            string debugsavefilepath = AppDomain.CurrentDomain.BaseDirectory + @"recovery\";
            string debugsavefileprefixname = "AICalcDiscardNum_";

            String ref_signal_100m_fileName = AppDomain.CurrentDomain.BaseDirectory + @"recovery\ref_signal_50m.txt";
            //String ref_signal_100m_ref_discard_num_FileName = AppDomain.CurrentDomain.BaseDirectory + @"recovery\ref_discard_num.txt";
            String acqData_File = AppDomain.CurrentDomain.BaseDirectory + @"recovery\tempAcqData.bin";
            string inputstr = $"{ref_signal_100m_fileName} {ref_signal_100m_ref_discard_num_FileName} {acqData_File} 80 0,6,9.5,14.5,20 0.05,0.1,19.95 4,4,4,4,4,4,4,8 8000";
            MatlabDll? calc_discard_dll = Hd.CurrProduct.MatlabDlls["cal_discard_num_by_multtone.dll"];
            MWArray[] paramArray = new MWArray[]
            {
                new MWCharArray("0"),
                new MWCharArray("0"),
                new MWCharArray(debugsavefilepath.ToCharArray()),
                new MWCharArray(debugsavefileprefixname.ToCharArray()),
                new MWCharArray(inputstr.ToCharArray()),
            };

            MWNumericArray? dataout_dbi = (MWNumericArray?)calc_discard_dll.Method.Invoke(calc_discard_dll.Instance, paramArray);
            if (((MWNumericArray)dataout_dbi!).ToArray().Length==0)
            {
                return false;
            }
            Double[] tmp_coeff = (double[])((MWNumericArray)dataout_dbi!).ToVector(MWArrayComponent.Real);

            if (Math.Abs(ref_discard_num_raw[0] - tmp_coeff[0]) >= 200 || Math.Abs(ref_discard_num_raw[1] - tmp_coeff[1]) >= 200 || Math.Abs(ref_discard_num_raw[2] - tmp_coeff[2]) >= 200 || Math.Abs(ref_discard_num_raw[3] - tmp_coeff[3]) >= 200)
            {
                needDiscardAgain = true;
            }
            else
            {
                needDiscardAgain = false;
            }

            if (needDiscardAgain)
            {
                try
                {
                    string temp_discard_filename = debugsavefilepath + "temp_discard_num.txt";
                    string temp_discard_num = string.Join(",", tmp_coeff);
                    File.WriteAllText(temp_discard_filename, temp_discard_num);
                    Console.WriteLine("Write temp discard num finished!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Write temp discard num error!");
                }
            }
            else
            {
                Console.WriteLine("Discard numbers done, no extra operation needed!");

            }

            //AnaChnlScaleIndex analogChnlScaleIndex = Cali_XunXin40GAdcDbi.XunXin40G_LevelID_DiscardDotsStoreAt;
            for (int channelID = 0; channelID < 1; channelID++)
            {
                for (int subbandID = 0; subbandID < 4; subbandID++)
                {
                    ChangeDBIAnalogParamsItem(0, channelID, (Int32)AnaChnlScaleIndex.Lv1, subbandID, tmp_coeff);

                    //String keyStr = GetAnalogChannelKey((ChannelId)channelID, analogChnlScaleIndex, subbandID);
                    //AcqBdNo acqBdNo = (AcqBdNo)(channelID * 4 + subbandID);
                    //DbiAnalogChannelItem_Common currData = DbiAnalogParams_Common.Default[keyStr];
                    //currData[(int)AnalogChannelItems_DBI20G.DiscardDots] = (int)tmp_coeff[subbandID];
                    //DbiAnalogParams_Common.Default[keyStr] = currData;
                    //Int32 discardDots = (Int32)DbiAnalogParams_Common.Default[keyStr][(int)AnalogChannelItems_DBI20G.DiscardDots];

                    //Hd.CurrProduct?.AcqBd?.WriteReg(AcqBdReg.W.DBI_IntDelayNum, acqBdNo, (UInt32)discardDots);
                }
            }
            CtrlAnalogChannel_DBI20G.SendDiscard();
            #endregion

            #region Step:Last 参数恢复
            Cali_ControlInterSource(false, ChannelId.C1, DbiSunbbandCaliSourceEnum.FastEdge, 50, false);//关闭内部源输出
            #endregion Step:Last 参数恢复
            return needDiscardAgain;
        }



        public static void ChangeDBIAnalogParamsItem(int bandMode, int channelIndex, int scaleIndex, int subbandIndex,double[] value)
        {
            DbiAnalogChannelSubbandItem originItem = DbiAnalogParams.Default[bandMode, channelIndex, scaleIndex, subbandIndex];
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
                DiscardDotsAfter = Convert.ToInt32(value[subbandIndex]),
                DiscardDotsBefore = Convert.ToInt32(0),
            };
            DbiAnalogParams.Default[bandMode, channelIndex, scaleIndex, subbandIndex] = newItem;
        }

        #region 项目的现在或定版是的一些参数
        internal static AnaChnlScaleIndex XunXin40G_LevelID_DiscardDotsStoreAt = AnaChnlScaleIndex.Lv20m;
        internal static int XunXin40G_TotalSubbandCount = 4;
        internal static int XunXin40G_ParallelRoadCount = 128;
        private static Double XunXin40G_SampleFreqPerSubbandByMsps = 80_000;/*每子带插值后的采样率，160G*/
        #endregion

        private static void InitDebugVarints_XunXin40G_CaliDiscardDots()
        {
            Hd.CurrDebugVarints.bEnable_AmplitudeTemperatureCompensate = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanAcqProcBdLoopTime = false;
            Hd.CurrDebugVarints.bEnable_AtStartScanAdcRxWindows = false;
            Hd.CurrDebugVarints.bEnable_AtStartTrigScanWindow = false;

            Hd.CurrDebugVarints.bEnable_AdcDataDebugMode = false;
            Hd.CurrDebugVarints.bEnable_DigitTrigger = false;
            Hd.CurrDebugVarints.bEnable_AcqbdInterpolation = false;
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
            Hd.CurrDebugVarints.bEnable_Dbi_PhaseFreqCoef = false;
            Hd.CurrDebugVarints.bEnable_Dbi_IsSubbandMergeMode = true;

            Hd.CurrDebugVarints.bEnable_ProcBd_Average = false;
        }

        #region 内部源的处理
        internal static void Cali_ControlInterSource(bool bEnable, ChannelId channelId, DbiSunbbandCaliSourceEnum signalType, Int32 freqByMHz, Boolean offsetEnable)
        {
            Int32 freqByMHz_TrimBy100MHz = freqByMHz;
            freqByMHz_TrimBy100MHz = (freqByMHz_TrimBy100MHz / 10) * 10;
            #region Step1:控制内部源
            CtrlAnalogChannel_DBI20G.CtrlInnerSource(bEnable, channelId, signalType, (UInt32)freqByMHz_TrimBy100MHz, offsetEnable);
            Thread.Sleep(500);
            #endregion Step1:控制内部源
        }
        #endregion;
        private static void InitDebugVarints_XunXin40G_CaliTiAdcByInternalSignalSource()
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

            Hd.CurrDebugVarints.bEnable_CorrectTiAdc = false;

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
        public static void XunXin40GAdcTiAdcByInternalSignalSource()//????   20251120
        {
            //#region 原有代码
            //UInt16 mult_boards_sync_mode = 2; // 0 - NO SYNC; 2 - SYNCED

            //UInt32 b1_sync_state = 0;
            //UInt32 b2_sync_state = 0;
            //UInt32 b3_sync_state = 0;
            //UInt32 b4_sync_state = 0;

            //UInt32 b1_prbs_shifter;
            //UInt32 b1_shifter_extra;
            //UInt32 b2_shifter_extra;
            //UInt32 b3_shifter_extra;
            //UInt32 b4_shifter_extra;

            //UInt16 edge_measure_b1 = 0;
            //UInt16 edge_measure_b2 = 0;
            //UInt16 edge_measure_b3 = 0;
            //UInt16 edge_measure_b4 = 0;

            //UInt32 busy = 0;
            //UInt32 busy1 = 0;
            //UInt32 busy2 = 0;
            //UInt32 busy3 = 0;
            //UInt32 busy4 = 0;
            //uint busy1_reg = 0;
            //uint busy2_reg = 0;
            //uint busy3_reg = 0;
            //uint busy4_reg = 0;

            ////*************** Reset block design and set register init value *****************//
            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Xunxin_sdk_cali_RegBdRstCtrl, 1); // block design gobal reset
            //HdIO.Sleep(200);
            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Xunxin_sdk_cali_RegBdRstCtrl, 0);
            //HdIO.Sleep(200);
            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Xunxin_sdk_cali_RegBdRstCtrl, 1);
            //HdIO.Sleep(200);

            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.reverse_Write0, AcqBdNo.B1, 0x0000); // master board, fixed to be b1
            //if (mult_boards_sync_mode == 2)
            //{
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.reverse_Write0, AcqBdNo.B2, 0x0002); // slave board, other boards enabled
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.reverse_Write0, AcqBdNo.B3, 0x0002); // slave board, other boards enabled
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.reverse_Write0, AcqBdNo.B4, 0x0002); // slave board, other boards enabled
            //}
            //else if (mult_boards_sync_mode == 0)
            //{
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.reverse_Write0, AcqBdNo.B2, 0x0000); // slave board, other boards enabled
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.reverse_Write0, AcqBdNo.B3, 0x0000); // slave board, other boards enabled
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.reverse_Write0, AcqBdNo.B4, 0x0000); // slave board, other boards enabled
            //}

            //HdIO.WriteReg(ProcBdReg.W.ADC_204bReset, 0x0);

            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Xunxin_sdk_cali_RegSdkCtrl, 0x0); //  sdk cali ctrl
            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Xunxin_sync_prbs_RegSdkCtrl32lane, 0x0); // sdk prbs ctrl

            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Xunxin_sync_prbs_RegShifterExtra, 0); // shifter extra
            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Xunxin_sync_prbs_RegSlaveShifterPrbs, 0); // slave shifter prbs

            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Xunxin_sdk_cali_RegSdkGainCaliNum1, 200); // sdk_gain_cali_num1
            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Xunxin_sdk_cali_RegSdkSkewCaliNum, 2000); // sdk_skew_cali_num
            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Xunxin_sdk_cali_RegSdkGainCaliNum2, 200); // sdk_gain_cali_num1

            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Equalizer_ctrl_RegEqualizerCtrl, 0x00); // eq ctrl
            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Equalizer_ctrl_RegEqualizerCtrlData, 0x01); // eq data
            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Equalizer_ctrl_RegEqualizerCtrl, 0x01); // eq ctrl
            //HdIO.WaitForSpiTransfer(1, 5);
            //HdIO.Sleep(10);
            ////*************** Begin prbs and mult boards sync *****************//
            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Xunxin_sdk_cali_RegSdkCtrl, 0x0); // sdk ctrl
            //HdIO.Sleep(2000);
            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Xunxin_sdk_cali_RegSdkCtrl, 0x1);
            //HdIO.Sleep(2000);
            //Hd.CurrProduct.AcqBd!.WriteToAllFpga(AcqBdReg.W.Xunxin_sdk_cali_RegSdkCtrl, 0x0);
            //HdIO.Sleep(2000);


            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.reverse_Write1, AcqBdNo.B1, 0);
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.reverse_Write1, AcqBdNo.B2, 0);
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.reverse_Write1, AcqBdNo.B3, 0);
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.reverse_Write1, AcqBdNo.B4, 0);
            //HdIO.Sleep(200);


            //HdCtrl_Pll.PllSync_A();
            //HdIO.Sleep(200);

            //HdIO.WriteReg(ProcBdReg.W.ADC_204bReset, 0x0);
            //HdIO.Sleep(30);
            //HdIO.WriteReg(ProcBdReg.W.ADC_204bReset, 0x1);
            //HdIO.Sleep(30);
            //HdIO.WriteReg(ProcBdReg.W.ADC_204bReset, 0x0);
            //HdIO.Sleep(1000);

            //edge_measure_b1 = (ushort)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.reverse_Read0, AcqBdNo.B1);
            //edge_measure_b2 = (ushort)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.reverse_Read0, AcqBdNo.B2);
            //edge_measure_b3 = (ushort)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.reverse_Read0, AcqBdNo.B3);
            //edge_measure_b4 = (ushort)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.reverse_Read0, AcqBdNo.B4);
            //Trace.WriteLine($"adc sync pulse measure results after are b1:{edge_measure_b1}, b2:{edge_measure_b2}, b3:{edge_measure_b3}, b4:{edge_measure_b4}");

            //HdCtrl_Pll.PllSync_A();
            //HdIO.Sleep(200);

            ////*************** Master board sync stage *****************//
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSdkCtrl32lane, AcqBdNo.B1, 0); // start master board sync
            //HdIO.Sleep(500);
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSdkCtrl32lane, AcqBdNo.B1, 1);
            //HdIO.Sleep(500);
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSdkCtrl32lane, AcqBdNo.B1, 0);
            //while (b1_sync_state != 0XFFFF)
            //{
            //    b1_sync_state = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_RoSynced, AcqBdNo.B1);
            //}
            //b1_shifter_extra = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_ROShifterExtraCalc, AcqBdNo.B1);
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegShifterExtra, AcqBdNo.B1, b1_shifter_extra);

            //b1_prbs_shifter = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_RoShifterPrbs, AcqBdNo.B1);
            //if (mult_boards_sync_mode == 2)
            //{
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSlaveShifterPrbs, AcqBdNo.B2, b1_prbs_shifter); // send prbs shifter to slave boards
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSlaveShifterPrbs, AcqBdNo.B3, b1_prbs_shifter); // send prbs shifter to slave boards
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSlaveShifterPrbs, AcqBdNo.B4, b1_prbs_shifter); // send prbs shifter to slave boards
            //}
            ////*************** Slave board sync stage *****************//
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSdkCtrl32lane, AcqBdNo.B2, 0); // start slave boards sync
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSdkCtrl32lane, AcqBdNo.B3, 0); // start slave boards sync
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSdkCtrl32lane, AcqBdNo.B4, 0); // start slave boards sync
            //HdIO.Sleep(200);
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSdkCtrl32lane, AcqBdNo.B2, 1);
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSdkCtrl32lane, AcqBdNo.B3, 1);
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSdkCtrl32lane, AcqBdNo.B4, 1);
            //HdIO.Sleep(200);
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSdkCtrl32lane, AcqBdNo.B2, 0);
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSdkCtrl32lane, AcqBdNo.B3, 0);
            //Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegSdkCtrl32lane, AcqBdNo.B4, 0);
            //if (mult_boards_sync_mode == 0)
            //{
            //    while (b2_sync_state != 0XFFFF)
            //    {
            //        b2_sync_state = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_RoSynced, AcqBdNo.B2);
            //    }
            //    b2_shifter_extra = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_ROShifterExtraCalc, AcqBdNo.B2);
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegShifterExtra, AcqBdNo.B2, b2_shifter_extra);
            //}
            //else if (mult_boards_sync_mode == 2)
            //{
            //    while (b2_sync_state != 0xffff)
            //    {
            //        b2_sync_state = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_RoSynced, AcqBdNo.B2);
            //    }
            //    b2_shifter_extra = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_ROShifterExtraCalc, AcqBdNo.B1);
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegShifterExtra, AcqBdNo.B2, b1_shifter_extra);
            //    HdIO.Sleep(200);
            //}

            //if (mult_boards_sync_mode == 0)
            //{
            //    while (b3_sync_state != 0XFFFF)
            //    {
            //        b3_sync_state = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_RoSynced, AcqBdNo.B3);
            //    }
            //    b3_shifter_extra = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_ROShifterExtraCalc, AcqBdNo.B3);
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegShifterExtra, AcqBdNo.B3, b3_shifter_extra);
            //}
            //else if (mult_boards_sync_mode == 2)
            //{
            //    while (b3_sync_state != 0xffff)
            //    {
            //        b3_sync_state = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_RoSynced, AcqBdNo.B3);
            //    }
            //    b3_shifter_extra = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_ROShifterExtraCalc, AcqBdNo.B3);
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegShifterExtra, AcqBdNo.B3, b1_shifter_extra);
            //    HdIO.Sleep(200);
            //}

            //if (mult_boards_sync_mode == 0)
            //{
            //    while (b4_sync_state != 0XFFFF)
            //    {
            //        b4_sync_state = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_RoSynced, AcqBdNo.B4);
            //    }
            //    b4_shifter_extra = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_ROShifterExtraCalc, AcqBdNo.B4);
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegShifterExtra, AcqBdNo.B4, b4_shifter_extra);
            //}
            //else if (mult_boards_sync_mode == 2)
            //{
            //    while (b4_sync_state != 0xffff)
            //    {
            //        b4_sync_state = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_RoSynced, AcqBdNo.B4);
            //    }
            //    b4_shifter_extra = (UInt32)Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sync_prbs_ROShifterExtraCalc, AcqBdNo.B4);
            //    Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sync_prbs_RegShifterExtra, AcqBdNo.B4, b1_shifter_extra);
            //    HdIO.Sleep(200);
            //}

            //#endregion 原有代码

            //#region Adc Tiadc calibration
            //List<ChannelId> includeChannels = new List<ChannelId>() { ChannelId.C1 };//通过此控制现在的硬件已经插上几个物理通道

            //InitDebugVarints_XunXin40G_CaliTiAdcByInternalSignalSource();
            //Hd.CurrDebugVarints.bEnable_AdcDataDebugMode = false;
            //Hd.CurrDebugVarints.iDbi_DebugChannelID = includeChannels[0];
            //Hd.LocalCommands |= (Int64)HdCmd.CaliDataChanged;
            //if (Hd.UIMessage != null)
            //    Hd.Execute(Hd.UIMessage);

            //List<Int32> subbandInnerSourceFreqByMHz = new List<Int32>() { 6130, 20000 - 6130, 30000 - 7130, 40000 - 8130 };
            //List<int> includeSubbands = new List<int>() { 0, 1, 2, 3 };//通过此控制现在的硬件已经插上几个物理子带

            //foreach (int subBandIndex in includeSubbands)
            //{
            //    foreach (var channelId in includeChannels)
            //        Cali_ControlInterSource(true, channelId, DbiSunbbandCaliSourceEnum.SingleTone, subbandInnerSourceFreqByMHz[subBandIndex], false);
            //    #region 开启校准
            //    foreach (ChannelId channelId in includeChannels)
            //    {
            //        AcqBdNo acqBdNo = (AcqBdNo)(((int)channelId) * 4 + subBandIndex);

            //        Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sdk_cali_RegSdkCtrl, acqBdNo, 0x0);
            //        HdIO.Sleep(2000);
            //        Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sdk_cali_RegSdkCtrl, acqBdNo, 0x1);
            //        HdIO.Sleep(2000);
            //        Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sdk_cali_RegSdkCtrl, acqBdNo, 0x0);
            //    }
            //    #endregion

            //    #region 检查校准是否结束
            //    List<ChannelId> notfinished_channels = new List<ChannelId>();
            //    notfinished_channels.AddRange(includeChannels);
            //    Stopwatch sw = Stopwatch.StartNew();
            //    while (notfinished_channels.Count != 0)
            //    {
            //        if (sw.ElapsedMilliseconds > 60 * 1000)
            //        {
            //            String errorMessage = "";
            //            foreach (ChannelId channelId in notfinished_channels)
            //            {
            //                if (errorMessage == "")
            //                    errorMessage += channelId;
            //                else
            //                    errorMessage += "," + channelId;
            //            }
            //            errorMessage += $@" 之子带 {subBandIndex} 没有等到校准结束信号！";

            //            Trace.WriteLine(errorMessage);
            //            break;
            //        }
            //        List<ChannelId> okChannel = new List<ChannelId>();
            //        foreach (ChannelId channelId in notfinished_channels)
            //        {
            //            AcqBdNo acqBdNo = (AcqBdNo)(((int)channelId) * 4 + subBandIndex);
            //            UInt32 readback = Hd.CurrProduct!.AcqBd!.ReadReg(AcqBdReg.R.Xunxin_sdk_cali_RoSdkCalistateRdOReg, acqBdNo);
            //            if (readback == 0x00FF)
            //            {
            //                okChannel.Add(channelId);
            //            }
            //        }
            //        foreach (ChannelId channelId in okChannel)
            //            notfinished_channels.Remove(channelId);
            //    }
            //    #endregion

            //    foreach (ChannelId channelId in includeChannels)
            //    {
            //        AcqBdNo acqBdNo = (AcqBdNo)(((int)channelId) * 4 + subBandIndex);
            //        Hd.CurrProduct.AcqBd!.WriteReg(AcqBdReg.W.Xunxin_sdk_cali_RegBdRstCtrl, acqBdNo, 0);
            //        HdIO.Sleep(5);
            //    }
            //}
            ////关闭内部源输出
            //foreach (var channelId in includeChannels)
            //    Cali_ControlInterSource(false, channelId, DbiSunbbandCaliSourceEnum.SingleTone, 0, false);
            //#endregion
        }

        public static void CaliTiadc(ChannelId chnlId)
        {
            ChangeAnalogConfig(100000, 1000);

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
                [0] = (3500, 3500),
                [1] = (9500, 500.0),
                [2] = (14500, 500.0),
                [3] = (19500, 3000.0),
            };



            Hd.CurrDebugVarints.iDbi_DebugChannelID = chnlId;
            Hd.LocalCommands |= (Int64)HdCmd.CaliDataChanged;

            //ChangeAnalogConfig(100000,1000);

            if (Hd.UIMessage != null)
                Hd.Execute(Hd.UIMessage);



            AcqModeAndInterleaveDefine define = Hd.CurrProduct!.Acquirer_AnalogChannel!.AnalogAcquireModel!.GetCurrentAcqModeInterleave()!;
            Hd.Calibration.ReSetAdcConfig(define);//初始化Adc参数 phase:32000 delay:128 //
            var deltas = Hd.Calibration.InitAdcDelta();

            foreach (Int32 subbandId in caliItems.Keys)
            {
                Cali_ControlInterSource(true, chnlId, DbiSunbbandCaliSourceEnum.SingleTone, (Int32)caliItems[subbandId].signalFreqByMHz, false);//打开内部源输出
                HdIO.Sleep(1000);
                Cali_ControlInterSource(true, chnlId, DbiSunbbandCaliSourceEnum.SingleTone, (Int32)caliItems[subbandId].signalFreqByMHz, false);//打开内部源输出
                HdIO.Sleep(1000);

                #region 判断采集器是否采满，用于取数据判断凭据
                if (!AbstractController_Misc.AcqIsFulled())
                {
                    //读使能复位
                    HdIO.WriteReg(ProcBdReg.W.LSCtrl_ReadEnable, 0x0);
                    Acquisition.InitAcq(true);
                    Thread.Sleep(10);
                }
                #endregion

                Int32 acqcount = 0;
                Thread.Sleep(1000);
                Hd.Calibration.CalcTIofEachChannelDBI(define, deltas, 1, caliItems[subbandId].actualFreqByMHz,subbandId, acqcount == 10);
                Thread.Sleep(2000);
                Hd.Calibration.SetSyncSampleClock(define, true);
                Thread.Sleep(2000);


                while (/*Hd.Calibration.CaliStatus && */!Hd.Calibration.CalcAdcPhaseDataDBI(define, deltas, 0.85, caliItems[subbandId].actualFreqByMHz,subbandId, acqcount == 50) && acqcount < 50)
                {
                    acqcount++;
                    Hd.Calibration.PrintTiAdcCaliLog($"校准20G,第{acqcount}次");
                    Hd.Calibration.SetSyncSampleClock(define, true);
                    Thread.Sleep(3000);
                }

                Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
                Hd.Calibration.SetSyncSampleClock(define);//设置亚稳态区间
                Thread.Sleep(1000);
            }
            Cali_ControlInterSource(false, chnlId, DbiSunbbandCaliSourceEnum.SingleTone, 0, false);
        }
        public static void ChangeAnalogConfig(Int32 scele = 20, Int16 DelayMsAfterHardwareChannged = 1000)
        {
            var analog0ptions = new List<HdMessage.AnalogOptions>();
            Int32 storagewavedotscnt = 50 * 1000;
            //var coupling = Constants.ANA_CHNL_TYPE == AnaChnlType.ANA_8G ? AnaChnlCoupling.DC50 : AnaChnlCoupling.DC1M;
            var coupling = AnaChnlCoupling.DC50;
            AnaChnlScaleIndex anaChnlScaleIndex = AnaChnlScaleIndex.Lv100m;
            switch (scele)
            {
                case 1: anaChnlScaleIndex = AnaChnlScaleIndex.Lv1m; break;
                case 2: anaChnlScaleIndex = AnaChnlScaleIndex.Lv2m; break;
                case 5: anaChnlScaleIndex = AnaChnlScaleIndex.Lv5m; break;
                case 10: anaChnlScaleIndex = AnaChnlScaleIndex.Lv10m; break;
                case 20: anaChnlScaleIndex = AnaChnlScaleIndex.Lv20m; break;
                case 50: anaChnlScaleIndex = AnaChnlScaleIndex.Lv50m; break;
                case 100: anaChnlScaleIndex = AnaChnlScaleIndex.Lv100m; break;
                case 200: anaChnlScaleIndex = AnaChnlScaleIndex.Lv200m; break;
                case 500: anaChnlScaleIndex = AnaChnlScaleIndex.Lv500m; break;
                case 1000: anaChnlScaleIndex = AnaChnlScaleIndex.Lv1; break;
                case 50000: anaChnlScaleIndex = AnaChnlScaleIndex.Lv50; break;
                case 100000: anaChnlScaleIndex = AnaChnlScaleIndex.Lv100; break;
                default:
                    break;
            }
            List<ChannelId> needcali20gchannels = new()
                    {
                        ChannelId.C1, ChannelId.C2, ChannelId.C3,ChannelId.C4
                    };
            foreach (var item in needcali20gchannels)
            {
                HdMessage.AnalogOptions ch = Hd.UIMessage!.Analog![(Int32)item] with
                {
                    Active = true,
                    Bandwidth = 0,
                    IsInverted = false,
                    ScaleIndex = (Int32)anaChnlScaleIndex,
                    Scale = scele,
                    ScaleBymV = scele,
                    Coupling = coupling,
                    Bias = 0,
                    Position = 0,
                };
                analog0ptions.Add(ch);
            }
            storagewavedotscnt = 1000 * 1000;
            var mode = Hd.UIMessage!.Timebase! with
            {
                TmbScale = TimebaseTableByus.Table[AnaChnlTimebaseIndex.Lv10n].Scale,
                TmbScaleIndex = (Int32)AnaChnlTimebaseIndex.Lv10n,
                StorageWaveDotsCnt = storagewavedotscnt,
                NeedWaveDotsCnt = storagewavedotscnt,
                //AcqMode= AnaChnlAcqMode.Normal,
            };
            var display = Hd.UIMessage!.Display! with { IsFast = false };
            Hd.UIMessage = Hd.UIMessage! with { Analog = analog0ptions.ToArray(), Timebase = mode, Display = display };
            Hd.Execute(Hd.UIMessage);
            Thread.Sleep(DelayMsAfterHardwareChannged);
        }


        record CtrlWordAndPhase(Int32 CtrlWord, Double PhaseError);

        record PhaseGainError(Double phaseByfs, Double gain);

        private static void CaliDbiTiadc(ChannelId chnlId, Int32 subbandId, UInt32 signalFreqByMhz, Double subbandFreqByMhz)
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

            //CtrlAnalogChannel_DBI20G.ConfigInnerSignalSource(true, signalFreqByMhz, CtrlAnalogChannel_DBI20G.SrcTypeEnum.SingleFreq, chnlId);
            Cali_ControlInterSource(true, chnlId, DbiSunbbandCaliSourceEnum.SingleTone, (Int32)signalFreqByMhz, false);//打开内部源输出
            HdIO.Sleep(1000);
            Cali_ControlInterSource(true, chnlId, DbiSunbbandCaliSourceEnum.SingleTone, (Int32)signalFreqByMhz, false);//打开内部源输出
            HdIO.Sleep(1000);

            Hd.CurrProduct?.AcqBd?.TiAdc_ApplyAdc_Phase_Offset_Gain();
            AbstractController_AnalogChannel.CtrlOffset();

            Thread.Sleep(paramSendTimesByms);

            Int32 baseAdcId = 0;
            List<Int32> adcIdList = new() { baseAdcId, 1 };

            Dictionary<Int32, Boolean> gainOK = new();
            Dictionary<Int32, Boolean> phaseOK = new();

            Dictionary<Int32, Int32> phaseBaseCtrlWord = new();
            Dictionary<Int32, Int32> phaseDeltaCtrlWord = new();

            Dictionary<Int32, Int32> maxPhaseDelta = new();
            Dictionary<Int32, Int32> minPhaseDelta = new();

            foreach (Int32 adcId in adcIdList)
            {
                TiadcParamsKeyMap itemKey = new("All-20G", (ChannelId)subbandId, (uint)adcId);
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
                    TiadcParamsKeyMap itemKey = new("All-20G", (ChannelId)subbandId, (uint)adcId);
                    TiadcPhaseOffsetGainItem_Base tmpItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(itemKey)!.Value;
                    TiadcParamsKeyMap baseitemKey = new("All-20G", (ChannelId)subbandId, (uint)baseAdcId);
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
                    ProductDataTranslate_MSO8000X.SetTiadcParamsItem(baseitemKey, baseItem);
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

        private static Dictionary<Int32, PhaseGainError> GetAdcAverageError(Int32 subbandId, List<Int32> adcIdList, Double sampFreqByMHz, Double subbandFreqByMhz, Int32 baseAdcId = 0, Int32 adcCnt = 2, Int32 staticTimes = 5)
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
                    if (maxvalue - minvalue < 2560 || maxvalue - minvalue > 64000|| maxvalue == 65536 || minvalue == 0)
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

        private static Dictionary<Int32, Double[]> GetSubbandAdcData(Int32 subbandId, List<Int32> adcIdList, Int32 adcCnt = 2)
        {
            Int32 dataLen = 10000;
            Dictionary<Int32, Double[]> ans = new();
            var readinfo = new List<ReadInfo>
            {
                new ReadInfo(AcqDataType.AnalogChannel,
                             new List<ChannelId>() { ChannelId.C1, ChannelId.C2, ChannelId.C3, ChannelId.C4 },
                             new WfmPkgInfo(25000, 1, 0.1),
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

        private static Int32 GetPhaseDeltaCtrlWords(CtrlWordAndPhase? negativePhase, CtrlWordAndPhase? positivePhase, Double theotryStep, CtrlWordAndPhase curPhase)
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
    }
}

using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using ScopeX.Hardware.Calibration.Tool.Utilities;
using System.Xml;
using static ScopeX.Hardware.Calibration.Data.Base.ProductDataTranslate_MSO8000X;

namespace ScopeX.Hardware.Calibration.Tool
{
    internal class BatchTaskPart_TiAdc_MSO8000X : BatchTaskPartBase
    {
        public override string FuncionDescription
        {
            get => $"校准MSO8000X的TiAdc";
        }
        public override string ParametersDescription
        {
            get
            {
                int argIndex = 1;
                return $"第{argIndex++}个参数：通道组合构造成的交织方式，请使用Driver端代码中的AnalogAcquireModel.cs 之 AcqModeInterleaveDefines 之InterleaveName{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，需要校准的通道，Ch1、Ch2、Ch3、Ch4，可以用|连接两个同时校准的通道，如Ch1|Ch2{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，阻抗，low=50欧姆，低阻，higf表示高阻{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，以mV为单位的档位值，浮点数{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，输入信号频率，用MHz为单位，浮点数{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，单路信号的采样率，Msps为单位，浮点数{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，Gain误差门限，以百分之为单位，浮点数{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，Phase误差门限，以飞秒(fs)单位，整数{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，调整步进(1~50)，整数{System.Environment.NewLine}";
            }
        }
        public override string Example
        {
            get => "BatchTaskPart_TiAdc_MSO8000X C1C3_20G,Ch1|Ch2,HIGH,50,100,10000,0.5,1";
        }
        private string InterleaveName = "";
        private List<int> NeedCaliChannelList = new List<int>();
        private ChannelId CaliChannelId;
        private int Impedance = 0;  //0为高，1为低；
        private double InputLevelBymV = 50;
        private double inputSignalFreqByMHz = 100;
        private double sampleByM_Sps = 10000;
        private double GainErrorThresholdByPercent = 0.5;
        private double OffsetErrorThreshold = 1;
        private double PhaseErrorThreshold_fs = 1;
        private int AdjustStep = 1;
        private static long _UniqueId = 0;//日志记录唯一码
        private string ProductType = "ANA_8G";
        private bool _ParamsValid = false;
        private bool AnalyParameter(string parameter)
        {
            parameterStr = parameter;
            NeedCaliChannelList.Clear();
            string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramList.Length < 8)
            {
                return _ParamsValid = false;
            }
            //[0]InterleaveName
            InterleaveName = paramList[0].Trim();

            //[1]: NeedCaliChannelList
            string channelID = paramList[1];
            switch (channelID)
            {
                case "1": CaliChannelId = ChannelId.C1; break;
                case "2": CaliChannelId = ChannelId.C2; break;
                case "3": CaliChannelId = ChannelId.C3; break;
                case "4": CaliChannelId = ChannelId.C4; break;
            }
            //[2]:Impedance
            Impedance = paramList[2].ToUpper() switch
            {
                "HIGH" => 0,
                _ => 1
            };
            //[3]:InputLevelBymV
            InputLevelBymV = BaseHelper.TryConvertToDouble(paramList[3]);
            //[4]:signalFreqByMHz
            inputSignalFreqByMHz = BaseHelper.TryConvertToDouble(paramList[4]);
            //[5]:sampleByM_Sps
            sampleByM_Sps = BaseHelper.TryConvertToDouble(paramList[5]);
            //[6]:GainErrorThresholdByPercent
            GainErrorThresholdByPercent = BaseHelper.TryConvertToDouble(paramList[6]);
            //[7]:OffsetErrorThreshold
            OffsetErrorThreshold = BaseHelper.TryConvertToDouble(paramList[7]);
            //[8]:AdjustStep
            AdjustStep = BaseHelper.TryConvertToInt(paramList[8]);
            //[9]:ProductType
            ProductType = paramList[9].Trim();
            return _ParamsValid = true;
        }

        public override bool SetParameter(XmlScpiCmd? xmlScpiCmd, string parameter)
        {
            if (xmlScpiCmd == null)
                return false;
            base.SetParameter(xmlScpiCmd, parameter);
            string[]? myName_ParameterPair = BaseHelper.SplitClassNameAndParameter(xmlScpiCmd.ProgramFuncName.Trim());
            if (myName_ParameterPair == null)
                return false;
            return AnalyParameter(myName_ParameterPair[1]);
        }

        private static void LogInfo(string info)
        {
            Utilities.Logger.LogCaliInfo(_UniqueId, "Log", "BatchTaskPart_TiAdc_JiHe_MSO8000X_Phase", DateTime.Now.ToString("【yyyy-MM-dd HH:mm:ss.fff】") + info);
        }


        public override BatchTaskPartResult Exec(double overtimeBySec, out string outMsg, CancellationTokenSource? cancelTokenSrc = null)
        {
            outMsg = String.Empty;
            _UniqueId = DateTime.Now.Ticks;

            if (!_ParamsValid)
            {
                LogInfo($"(ExecuteId={_UniqueId};)参数错误！");
                return BatchTaskPartResult.ErrorParameter;
            }
            ServerSpecailData.Load(currInstrumentSession);
            CommonMethod.RefreshConstDataFromServer(currInstrumentSession);
            if (!ServerSpecailData.JiHe_AcqModeInterleaveDefines?.Any(o => o.Value.Name == InterleaveName) ?? false)
            {
                LogInfo($"(ExecuteId={_UniqueId};)参数错误！");
                return BatchTaskPartResult.ErrorParameter;
            }
            var currModeDef = GetAcqModeInterleaveDefine(InterleaveName);
            int adcportcount = currModeDef.Details!.First()!.Value.First().AdcPorts.Count;
            CaliStateManager caliStateManager = new CaliStateManager(1, ServerDomainConstants.AdcCount / 2);
            int needRecvedByteCount = sizeof(ushort) * ServerDomainConstants.PerAdcCoreDataCount * ServerDomainConstants.AdcCount;
            //初始化
            foreach (var adc in currModeDef.Details.First().Value.First().AdcPorts)
            {
                TiadcParamsKeyMap itemKey = new(InterleaveName, CaliChannelId, (uint)adc.Key);
                TiadcPhaseOffsetGainItem_Base currItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(itemKey)!.Value;
                currItem.Gain = 40960;
                currItem.Offset_FPGA = 0;
                ProductDataTranslate_MSO8000X.SetTiadcParamsItem(itemKey, currItem);
            }
            Stopwatch stopwatch = new Stopwatch();
            long maxWaitMilliseconds = overtimeBySec > 0 ? (long)(1000 * overtimeBySec) : 60 * 1000;
            StringBuilder caliMsg = new StringBuilder();
            stopwatch.Start();

            while (!caliStateManager.IsAllCompleted())
            {
                try
                {
                    cancelTokenSrc?.Token.ThrowIfCancellationRequested();
                }
                catch
                {
                    LogInfo($"(ExecuteId={_UniqueId};)任务被取消！");
                    return BatchTaskPartResult.Cancel;
                }
                if (stopwatch.ElapsedMilliseconds > maxWaitMilliseconds)
                {
                    LogInfo($"(ExecuteId={_UniqueId};)任务超时！");
                    return BatchTaskPartResult.ErrorOvertime;
                }

                InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.TiadcPhaseOffsetGainParams);
                Thread.Sleep(500);

                //获取多次数据做分析
                List<List<ushort[]>> datas = new List<List<ushort[]>>();
                for (int i = 0; i < 7; i++)
                {
                    List<ushort[]>? adcData = InstrumentInteract.Factory_WaveData_Adc(currInstrumentSession, 6_000, null, needRecvedByteCount, currModeDef.Details.Count);
                    if (adcData == null)
                    {
                        LogInfo($"(ExecuteId={_UniqueId};)采集数据错误!");
                        break;
                    }
                    datas.Add(adcData);
                    Thread.Sleep(50);
                }
                DataManager dataManager = new DataManager(datas, sampleByM_Sps, inputSignalFreqByMHz);

                //下发值校准
                for (var channelIndex = 0; channelIndex < 1; channelIndex++)
                {
                    var currentDefineDetail = currModeDef.Details[CaliChannelId];
                    if (currentDefineDetail == null)
                        continue;

                    //校准gain,phase
                    if (caliStateManager.IsChnlCompleted(channelIndex))
                        continue;
                    foreach (var adc in currModeDef.Details.First().Value.First().AdcPorts)
                    {
                        if (caliStateManager.IsAdcCompleted(channelIndex, adc.Key))
                            continue;

                        TiadcParamsKeyMap itemKey = new(InterleaveName, CaliChannelId, (uint)adc.Key);
                        TiadcPhaseOffsetGainItem_Base currItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(itemKey)!.Value;

                        var caliStateGain = caliStateManager.GetCaliState(channelIndex, adc.Key, CaliStateManager.CaliItem.Gain);
                        if (!caliStateGain.IsItemCompleted())
                            CaliGain(caliStateGain, dataManager, ref currItem, caliMsg, currentDefineDetail.First(), adc.Key);

                        var caliStateOffset = caliStateManager.GetCaliState(channelIndex, adc.Key, CaliStateManager.CaliItem.Offset);
                        if (!caliStateOffset.IsItemCompleted())
                            CaliOffset(caliStateOffset, dataManager, ref currItem, caliMsg, currentDefineDetail.First(), adc.Key);

                        //var caliStatePhase = caliStateManager.GetCaliState(channelIndex, acqUnitIndex, CaliStateManager.CaliItem.Phase);
                        //if (!caliStatePhase.IsItemCompleted())
                        //    CaliPhase(caliStatePhase, dataManager, ref currItem, caliMsg, currentDefineDetail.First(), acqUnitIndex);

                        //更新寄存器值
                        ProductDataTranslate_MSO8000X.SetTiadcParamsItem(itemKey, currItem);
                        //TiAdcPhaseOffsetGain_MSO8000X.Default.SetItem(currModeDefIndex, currChnlDefIndex, acqUnitIndex, currItem);
                    }
                }
            }
            CopyC1C3_20GData();
            StringBuilder sb = new StringBuilder();
            sb.Append($"(ExecuteId={_UniqueId};)");
            sb.Append($";总共用时{stopwatch.ElapsedMilliseconds}ms.");
            outMsg += sb.ToString();
            LogInfo($"校准结果：" + outMsg);
            InstrumentInteract.CaliData_SaveData(currInstrumentSession, CaliDataType.TiadcPhaseOffsetGainParams);
            return BatchTaskPartResult.Succeed;
        }

        /// <summary>
        /// 将C1C3_20G拷贝到C1-20G和C3-20G
        /// </summary>
        public void CopyC1C3_20GData()
        {
            List<TiadcParamsKeyMap> tiadcParamsKeyMaps = new List<TiadcParamsKeyMap>()
            {
              new("C1-20G", (ChannelId.C1), 0),
              new("C1-20G", (ChannelId.C1), 1),
              new("C3-20G", (ChannelId.C3), 0),
              new("C3-20G", (ChannelId.C3), 1)
            };

            foreach (var item in tiadcParamsKeyMaps)
            {
                TiadcParamsKeyMap itemKey = new("C1C3-20G", item.chnlId, item.adcId);
                TiadcPhaseOffsetGainItem_Base tiadcFineItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(itemKey)!.Value;//20G参数
                TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(item)!.Value;//20G参数
                tiadcItem = tiadcFineItem;
                ProductDataTranslate_MSO8000X.SetTiadcParamsItem(item, tiadcItem);
            }
        }


        /// <summary>
        /// 获取对应交织模式的参数
        /// </summary>
        /// <param name="interleaveMode"></param>
        /// <returns></returns>
        public AcqModeAndInterleaveDefine GetAcqModeInterleaveDefine(string interleaveModeName)
        {
            AcqModeAndInterleaveDefine currDetail = null;
            foreach (var item in ServerSpecailData.JiHe_AcqModeInterleaveDefines)
            {
                if (item.Value.Name == interleaveModeName)
                {
                    currDetail = item.Value;
                    if (currDetail != null)
                        break;
                }
            }
            return currDetail;
        }

        #region 具体的校准方法
        private void CaliGain(CaliStateManager.CaliState caliState, DataManager dataManager, ref TiadcPhaseOffsetGainItem_Base currItem
           , StringBuilder caliMsg, AdcUsedInfo currentDefineDetail, int acqUnitIndex)
        {
            var fixedAcqUnitInfo = 1;
            fixedAcqUnitInfo += currentDefineDetail.AcqBdNo == AcqBdNo.B2 ? 2 : 0;
            var currentAcqUnitInfo = currentDefineDetail.AcqBdNo == AcqBdNo.B2 ? 2 : 0;
            currentAcqUnitInfo += acqUnitIndex;

            double avgErrGain = dataManager.GetAvgErrGain(currentAcqUnitInfo, fixedAcqUnitInfo);
            string msg = GenerateMsg($"Gain Calibration(caliTimes:{caliState.GetCaliCount()}): ",
               /*(int)currentDefineDetail.ChannelId*/0, currentAcqUnitInfo, fixedAcqUnitInfo, currItem.Gain, avgErrGain);

            if (Math.Abs(avgErrGain) > (GainErrorThresholdByPercent / 100))//0.0005=>0.5
            {
                caliState.AddRegErr(currItem.Gain, avgErrGain);
                int step = 1000;
                if (Math.Abs(avgErrGain) < 0.01)
                {
                    step = 100;
                }
                caliState.AdjustStep = (int)(step * 0.9 * (avgErrGain > 0 ? 1 : -1));
                currItem.Gain = caliState.CalculateReg();
                if (currItem.Gain <= 0 || Math.Abs(currItem.Gain) > 65535)
                {
                    caliState.CurrFlag = CaliStateManager.CaliState.Flag.Failed;
                }
            }
            else
                caliState.CurrFlag = CaliStateManager.CaliState.Flag.Succeed;

            msg += $" - workFlag({caliState.CurrFlag})";
            LogInfo(msg);
        }

        private void CaliOffset(CaliStateManager.CaliState caliState, DataManager dataManager, ref TiadcPhaseOffsetGainItem_Base currItem
           , StringBuilder caliMsg, AdcUsedInfo currentDefineDetail, int acqUnitIndex)
        {
            var fixedAcqUnitInfo = 1;
            fixedAcqUnitInfo += currentDefineDetail.AcqBdNo == AcqBdNo.B2 ? 2 : 0;
            var currentAcqUnitInfo = currentDefineDetail.AcqBdNo == AcqBdNo.B2 ? 2 : 0;
            currentAcqUnitInfo += acqUnitIndex;

            double avgErrOffset = dataManager.GetAvgErrOffset(currentAcqUnitInfo, fixedAcqUnitInfo);
            string msg = GenerateMsg($"Offset Calibration(caliTimes:{caliState.GetCaliCount()}): ",
               /*(int)currentDefineDetail.ChannelId*/0, currentAcqUnitInfo, fixedAcqUnitInfo, currItem.Offset_FPGA, avgErrOffset);

            if (Math.Abs(avgErrOffset) > OffsetErrorThreshold)
            {
                caliState.AddRegErr(currItem.Offset_FPGA, avgErrOffset);
                caliState.AdjustStep = (int)(16 * avgErrOffset) * (ProductType == "ANA_8G" ? -1 : 1);
                currItem.Offset_FPGA = caliState.CalculateReg();
                if (Math.Abs(currItem.Offset_FPGA) > 65535)
                {
                    caliState.CurrFlag = CaliStateManager.CaliState.Flag.Failed;
                }
            }
            else
            {
                caliState.CurrFlag = CaliStateManager.CaliState.Flag.Succeed;
            }
            msg += $" - workFlag({caliState.CurrFlag})";
            LogInfo(msg);
        }

        private void CaliPhase(CaliStateManager.CaliState caliState, DataManager dataManager, ref TiadcPhaseOffsetGainItem_Base currItem,
            StringBuilder caliMsg, AdcUsedInfo currentDefineDetail, int acqUnitIndex)
        {
            var fixedAcqUnitInfo = currentDefineDetail.AdcPorts!.First().Key;
            var currentAcqUnitInfo = currentDefineDetail.AdcPorts![acqUnitIndex];

            double coreSampleIntervalPs = Math.Pow(10, 12) / (sampleByM_Sps * Math.Pow(10, 6));
            double theoryIntervalPs = coreSampleIntervalPs / currentDefineDetail.AdcPorts!.Count * acqUnitIndex;
            double avgErrPhase = dataManager.GetAvgErrPhase(currentAcqUnitInfo, fixedAcqUnitInfo);

            double avgErrPhase_pS = ((avgErrPhase + Math.PI * 2) % (Math.PI * 2)) * 1000_000 / inputSignalFreqByMHz / (2 * Math.PI) - theoryIntervalPs;
            if (avgErrPhase_pS > 1000_000 / inputSignalFreqByMHz / 2)
                avgErrPhase_pS -= 1000_000 / inputSignalFreqByMHz;
            else if (avgErrPhase_pS < -1000_000 / inputSignalFreqByMHz / 2)
                avgErrPhase_pS += 1000_000 / inputSignalFreqByMHz;

            string msg = GenerateMsg($"Phase Calibration(caliTimes:{caliState.GetCaliCount()}): ",
                /*(int)currentDefineDetail.ChannelId*/0, currentAcqUnitInfo, currentAcqUnitInfo, currItem.Phase, avgErrPhase_pS);

            if (Math.Abs(avgErrPhase_pS) * 1000 > PhaseErrorThreshold_fs)
            {
                caliState.AddRegErr(currItem.Phase, avgErrPhase_pS);
                caliState.AdjustStep = AdjustStep * (avgErrPhase_pS > 0 ? -1 : 1) * 4;
                currItem.Phase = caliState.CalculateReg();
            }
            else
                caliState.CurrFlag = CaliStateManager.CaliState.Flag.Succeed;

            msg += $" - workFlag({caliState.CurrFlag})";
        }

        private string GenerateMsg(string prefix, int chnlID, int coreID, int compCoreID, int regValue, double errValue)
        {
            return prefix + $"Chnl({chnlID + 1}) - " +
                $"Core({coreID + 1}-{compCoreID + 1}) - RegValue({regValue}) - ErrValue({errValue})";
        }

        #endregion 具体的校准方法

        private class CaliStateManager
        {
            public enum CaliItem
            {
                Gain,
                Offset,
            }

            public class CaliState
            {
                public enum Flag
                {
                    Continue,
                    Failed,
                    Succeed,
                }

                private readonly (int min, int max) AdcRange = (-65535, 65535);        //校准值取值范围

                //使用过的寄存器值,误差对的集合
                private List<KeyValuePair<int, double>> _RegErrPairs = new List<KeyValuePair<int, double>>();

                /// <summary>
                /// 当前状态标识
                /// </summary>
                public Flag CurrFlag { set; get; } = Flag.Continue;

                public Boolean IsFirstAdjust { get; set; } = true;

                public int AdjustStep { set; get; } = 1;

                public Boolean IsItemCompleted() => !(CurrFlag == CaliState.Flag.Continue);

                public int GetCaliCount() => _RegErrPairs.Count;

                public void AddRegErr(int regValue, double errValue)
                {
                    _RegErrPairs.Add(new KeyValuePair<int, double>(regValue, errValue));
                }

                public int CalculateReg() => CalulateBase();

                private int CalulateBase()
                {
                    int tempReg = _RegErrPairs.Last().Key + AdjustStep;

                    return Math.Min(AdcRange.max, Math.Max(AdcRange.min, tempReg));
                }
            }

            //校准工作状态的三维数组：1）通道;2）acqUnit;3) 校准项(Gain,Phase);
            private CaliState[,,] CaliStates;
            public CaliStateManager(Int32 chnlCount, Int32 acqUnitCount)
            {
                int caliItemCount = Enum.GetValues(typeof(CaliItem)).Length;
                CaliStates = new CaliState[chnlCount, acqUnitCount, caliItemCount];
                for (int chnlId = 0; chnlId < CaliStates.GetLength(0); chnlId++)
                {
                    for (int adcId = 0; adcId < CaliStates.GetLength(1); adcId++)
                    {
                        for (int caliItemId = 0; caliItemId < CaliStates.GetLength(2); caliItemId++)
                        {
                            CaliStates[chnlId, adcId, caliItemId] = new CaliState();
                        }
                    }
                }
            }

            public CaliState GetCaliState(Int32 chnlIndex, Int32 acqUnitIndex, CaliItem caliItem)
            {
                return CaliStates[chnlIndex, acqUnitIndex, (int)caliItem];
            }

            #region flag相关

            public Boolean IsAllSucceed()
            {
                for (int chnlId = 0; chnlId < CaliStates.GetLength(0); chnlId++)
                {
                    for (int acqUnitId = 0; acqUnitId < CaliStates.GetLength(1); acqUnitId++)
                    {
                        foreach (CaliItem item in Enum.GetValues(typeof(CaliItem)))
                        {
                            if (GetCaliState(chnlId, acqUnitId, item).CurrFlag != CaliState.Flag.Succeed)
                                return false;
                        }
                    }
                }
                return true;
            }

            public Boolean IsAllCompleted()
            {
                for (int chnlId = 0; chnlId < CaliStates.GetLength(0); chnlId++)
                {
                    if (!IsChnlCompleted(chnlId))
                        return false;
                }
                return true;
            }

            public Boolean IsChnlCompleted(Int32 chnlIndex)
            {
                for (int acqUnitId = 0; acqUnitId < CaliStates.GetLength(1); acqUnitId++)
                {
                    if (!IsAdcCompleted(chnlIndex, acqUnitId))
                        return false;
                }
                return true;
            }

            public Boolean IsChnlGainCompleted(Int32 chnlIndex)
            {
                for (int acqUnitId = 0; acqUnitId < CaliStates.GetLength(1); acqUnitId++)
                {
                    if (!GetCaliState(chnlIndex, acqUnitId, (int)CaliItem.Gain).IsItemCompleted())
                        return false;
                }
                return true;
            }

            public Boolean IsAdcCompleted(Int32 chnlIndex, Int32 acqUnitIndex)
            {
                foreach (CaliItem item in Enum.GetValues(typeof(CaliItem)))
                {
                    if (!GetCaliState(chnlIndex, acqUnitIndex, item).IsItemCompleted())
                        return false;
                }
                return true;
            }

            #endregion flag相关
        }

        private class DataManager
        {
            //List<ushort[]>为一次获取的数据,包含了Adc的采集数据
            private List<List<ushort[]>> Datas;
            //List<WaveOffsetGainPhase>为一次分析的结果数据,包含了Adc的结果数据
            private List<List<WaveOffsetGainPhase>> waveOffsetGainPhases = new List<List<WaveOffsetGainPhase>>();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="datas">多次的adc核数据</param>
            /// <param name="sampleByMHz"></param>
            /// <param name="signalByMHz"></param>
            public DataManager(List<List<ushort[]>> datas, double sampleByMHz, double signalByMHz)
            {
                Datas = datas;
                foreach (var timeData in Datas)
                {
                    List<WaveOffsetGainPhase> offsetGainPhases = new List<WaveOffsetGainPhase>();
                    foreach (var coreData in timeData)
                        offsetGainPhases.Add(SineFitFunc.SineFit(coreData, sampleByMHz, signalByMHz));
                    waveOffsetGainPhases.Add(offsetGainPhases);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="currCoreID"></param>
            /// <param name="relativeCoreID"></param>
            /// <returns>Gain差异，比例</returns>
            public double GetAvgErrGain(int currCoreID, int relativeCoreID)
            {
                List<double> errs = new List<double>();
                foreach (var timeOGP in waveOffsetGainPhases)
                    errs.Add((timeOGP[currCoreID].Gain - timeOGP[relativeCoreID].Gain) / timeOGP[relativeCoreID].Gain);
                return CommonMethod.MiddleDataFilter(errs, errs.Count - 2 * 2).Average();
            }

            /// <summary>
            /// 相对core0的相位差异
            /// </summary>
            /// <param name="currCoreID"></param>
            /// <param name="RelativeID"></param>
            /// <returns>Phase差异，弧度</returns>
            public double GetAvgErrPhase(int currCoreID, int RelativeID)
            {
                List<double> errs = new List<double>();
                foreach (var timeOGP in waveOffsetGainPhases)
                {
                    double err = timeOGP[currCoreID].Phase - timeOGP[RelativeID].Phase;
                    if (err < 0)
                        err += (Math.PI * 2 * 1000_000);
                    errs.Add(err);
                }
                return CommonMethod.MiddleDataFilter(errs, errs.Count - 2 * 2).Average();
            }

            /// <summary>
            /// 相对core0的相位差异
            /// </summary>
            /// <param name="currCoreID"></param>
            /// <param name="RelativeID"></param>
            /// <returns>Offset偏置</returns>
            public double GetAvgErrOffset(int currCoreID, int RelativeID)
            {
                List<double> errs = new List<double>();
                foreach (var timeOGP in waveOffsetGainPhases)
                {
                    double err = timeOGP[currCoreID].Offset - timeOGP[RelativeID].Offset;
                    errs.Add(err);
                }
                return CommonMethod.MiddleDataFilter(errs, errs.Count - 2 * 2).Average();
            }
        }

    }
}

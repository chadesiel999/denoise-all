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
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml;
using ScopeX.Hardware.Calibration.Tool.BatchTask.XmlHelper;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata.Ecma335;
using System.IO;
using System.Reflection.Metadata;
using static ScopeX.Hardware.Calibration.Data.Base.ProductDataTranslate_MSO8000X;
using System.Xml.Linq;

namespace ScopeX.Hardware.Calibration.Tool
{
    internal class BatchTaskPart_TiAdc_JiHe_MSO8000X_Phase : BatchTaskPartBase
    {
        bool caliAdcStatus = true;//判断是否继续校准

        public override string FuncionDescription
        {
            get => $"校准MSO8000X的TiAdc同步";
        }
        public override string ParametersDescription
        {
            get
            {
                int argIndex = 1;
                return $"第{argIndex++}个参数，需要类型，0:校准扫窗 1:校准10G 2:校准10G{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，输入信号频率，用MHz为单位，浮点数{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，单路信号的采样率，Msps为单位，浮点数{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，Phase误差门限，以飞秒(fs)单位，整数{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，是否初始化AdcPhase和FPGADelay，整数{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，校准数据生效时间，用ms表示{System.Environment.NewLine}" +
                       $"第{argIndex++}个参数，最大超时时间，用s表示{System.Environment.NewLine}";
            }
        }
        public override string Example
        {
            get => "BatchTaskPart_TiAdc_JiHe_MSO8000X_Phase 0,100,10000,100,0,100,60";
        }

        private double sampleByM_Sps = 10000;//采样率
        private double inputSignalFreqByMHz = 100;//输入信号频率
        private double PhaseErrorThreshold_fs = 1;//相位差范围
        private int CaliType = 0;//校准类型 0:校准扫窗 1:校准10G 2:校准10G
        private static long _UniqueId = 0;//日志记录唯一码
        private long maxWaitMilliseconds = 100;//超时时间
        private int DelayMsAfterHardwareChannged = 300;
        private bool IsReSetPhaseAndDelay = true;// 是否初始化AdcPhase和FPGADelay 0：不初始化 1：表示初始化
        private AdcInterleaveMode InterleaveMode;
        private string adcInterleaveName = string.Empty;
        private string m_ModelType = string.Empty;
        private AcqModeAndInterleaveDefine acqModeAndInterleaveDefine;

        private static List<AdcDelta> AdcDeltas = new List<AdcDelta>();

        public TiadcParamsKeyMap tiadcParamsKeyMap;
        public string paramName;

        private List<int> NeedCaliChannelList = new List<int>();
        private int Impedance = 0;  //0为高，1为低；
        private double InputLevelBymV = 50;
        private double GainErrorThresholdByPercent = 0.5;
        private double OffsetErrorThreshold = 1;
        private int AdjustStep = 1;

        private bool _ParamsValid = false;

        private bool AnalyParameter(string parameter)
        {
            parameterStr = parameter;
            string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramList.Length < 5)
            {
                return _ParamsValid = false;
            }
            //[0]:CaliType
            adcInterleaveName = paramList[0].Trim();
            switch (adcInterleaveName)
            {
                case "C1-20G":
                case "C3-10G":
                case "C1C3-20G": InterleaveMode = AdcInterleaveMode.Mode2To1; break;
                case "All-10G": InterleaveMode = AdcInterleaveMode.Mode1To1; break;
				case "All-20G": InterleaveMode = AdcInterleaveMode.Mode2To1; break;
            }
            //[1]:signalFreqByMHz
            inputSignalFreqByMHz = BaseHelper.TryConvertToDouble(paramList[1]);
            //[2]:sampleByM_Sps
            sampleByM_Sps = BaseHelper.TryConvertToDouble(paramList[2]);
            //[3]:PhaseErrorThreshold_fs
            PhaseErrorThreshold_fs = BaseHelper.TryConvertToDouble(paramList[3]);
            //[4]:IsReSetPhaseAndDelay
            IsReSetPhaseAndDelay = int.Parse(paramList[4]) == 1 ? true : false;
            //[5]:校准数据生效时间，用ms表示
            DelayMsAfterHardwareChannged = int.Parse(paramList[5]);
            //[6]：最大超时时间
            maxWaitMilliseconds = long.Parse(paramList[6]) * 1000;
            //[7]：校准扫窗
            CaliType = int.Parse(paramList[7]);
            m_ModelType = InterleaveMode == AdcInterleaveMode.Mode1To1 ? "10G" : "20G";

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

        private bool calistatus = true;

        public override BatchTaskPartResult Exec(double overtimeBySec, out string message, CancellationTokenSource? cancelTokenSrc = null)
        {
            message = String.Empty;
            _UniqueId = DateTime.Now.Ticks;
            CommonMethod.RefreshConstDataFromServer(currInstrumentSession);
            CommonMethod.SetDigitTrigger(currInstrumentSession, false);//关闭触发
            //CommonMethod.SetChannelDelay(currInstrumentSession, false);//关闭通道延时
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (cancelTokenSrc == null)
            {
                return BatchTaskPartResult.Cancel;
            }
            int count = 0;
            acqModeAndInterleaveDefine = GetAcqModeInterleaveDefine();

            //设置扫窗
            if (CaliType == 1 || CaliType == 2)
            {
                LogInfo($"BatchTaskPart_TiAdc_JiHe_MSO8000X_Phase:SetSyncSampleClock");
                SetSyncSampleClock();
                //CheckAdcPhaseData(AdcInterleaveMode.Mode1To1, PhaseErrorThreshold_fs);
                if (CaliType == 2)
                {
                    return BatchTaskPartResult.Succeed;
                }
            }
            LogInfo($"BatchTaskPart_TiAdc_JiHe_MSO8000X_Phase:Calc{m_ModelType}AdcPhaseData");
            //SwitchSamplingMode(InterleaveMode);
            if (IsReSetPhaseAndDelay)
            {
                InitAdcDelay(acqModeAndInterleaveDefine);
            }
            InitAdcDelta(acqModeAndInterleaveDefine);
            calistatus = true;
            while (CalcAdcPhaseData(true) && calistatus)
            {
                //退出处理
                try
                {
                    cancelTokenSrc?.Token.ThrowIfCancellationRequested();
                }
                catch
                {
                    message = $"(ExecuteId={_UniqueId};)任务被取消！";
                    LogInfo(message);
                    CommonMethod.SetTemperatureCompensate(currInstrumentSession, true);
                    return BatchTaskPartResult.Cancel;
                }
                if (stopwatch.ElapsedMilliseconds > maxWaitMilliseconds)
                {
                    message = $"(ExecuteId={_UniqueId};)超时退出";
                    LogInfo(message);
                    CommonMethod.SetTemperatureCompensate(currInstrumentSession, true);
                    return BatchTaskPartResult.ErrorOvertime;
                }
                count++;
                LogInfo($"{m_ModelType}模式第{count}次校准");
                Thread.Sleep(DelayMsAfterHardwareChannged);
                InstrumentInteract.CaliData_SaveData(currInstrumentSession, CaliDataType.TiadcPhaseOffsetGainParams);
                SetAcqModeInterleaveFPGADelay(acqModeAndInterleaveDefine);
            }
            //if (!caliAdcStatus)
            //{
            //    message = $"(ExecuteId={_UniqueId};)校准{m_ModelType}失败";
            //    LogInfo(message);
            //}
            //else
            {
                CopyC1C3_20GData();
                InstrumentInteract.CaliData_SaveData(currInstrumentSession, CaliDataType.TiadcPhaseOffsetGainParams);
            }
            StringBuilder sb = new StringBuilder();
            sb.Append($"(ExecuteId={_UniqueId};)");
            sb.Append($";总共用时{stopwatch.ElapsedMilliseconds}ms.");
            message += sb.ToString();
            Utilities.Logger.WriteLine($"校准结果：" + message);
            CommonMethod.SetDigitTrigger(currInstrumentSession, true);//打开触发
            //CommonMethod.SetChannelDelay(currInstrumentSession, true);//打开通道延时
            return caliAdcStatus ? BatchTaskPartResult.Succeed : BatchTaskPartResult.ErrorFatal;
        }

        /// <summary>
        /// 设置同步扫窗
        /// </summary>
        public void SetSyncSampleClock()
        {
            //获取扫窗数据
            string scpiCmd = InstrumentInteract.GetCmdStr(ScpiCmd.Factory_SpecailData);
            scpiCmd += " ? " + "ADC5200SyncWindowRegValue";
            currInstrumentSession!.WriteString(scpiCmd);
            Thread.Sleep(1000);
            string readbackStr = currInstrumentSession.ReadString();
            LogInfo($"扫窗数据: {readbackStr}");

            int fpgaIndex = 0;
            int adcIndex = 0;
            foreach (var item in readbackStr.Split(Environment.NewLine))
            {
                if (string.IsNullOrEmpty(item))
                {
                    continue;
                }
                var adcScandata = item.Split('>');
                //采集板
                string board = adcScandata[0];
                //扫窗数据
                string sacndata = adcScandata[1].Replace("_", "");

                Dictionary<int, int> steadystate = new Dictionary<int, int>();
                int offset = 0;
                int soffset = 0;
                for (int i = 0; i < sacndata.Length && i < 17; i++)
                {
                    if (sacndata[i] == '0')
                    {
                        offset++;
                        if (!steadystate.TryAdd(soffset, offset))
                        {
                            steadystate[soffset] = offset;
                        }
                    }
                    else
                    {
                        offset = 0;
                        soffset = i;
                    }
                }
                if (steadystate.Count == 0)
                {
                    continue;
                }
                var maxValue = steadystate.Values.Max();
                var maxKeyValuePair = steadystate.FirstOrDefault(kvp => kvp.Value == maxValue);

                //设置采集板
                switch (board.Replace("=", ""))
                {
                    case "B5.Adc1": fpgaIndex = 1; adcIndex = 0; break;
                    case "B5.Adc2": fpgaIndex = 1; adcIndex = 1; break;
                    case "B7.Adc1": fpgaIndex = 0; adcIndex = 0; break;
                    case "B7.Adc2": fpgaIndex = 0; adcIndex = 1; break;
                    default:
                        break;
                }
                string boardtxt = fpgaIndex == 1 ? "B5" : "B7";
                var postion = maxValue / 2;
                postion += maxValue % 2 == 1 ? 1 : 0;
                TiAdc_SyncSampleClock.Default[fpgaIndex][adcIndex].Sample20GClockDelay = (uint)(maxKeyValuePair.Key + postion);
                LogInfo($"扫窗亚稳态区间校准，board:{boardtxt} adcindex:{adcIndex} value: {(uint)(maxKeyValuePair.Key + postion)}");
            }

            #region 亚稳态窗 SyncSampleClock
            InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.TiAdc_SyncSampleClock);
            #endregion

            //保存校准数据
            InstrumentInteract.CaliData_SaveData(currInstrumentSession, CaliDataType.TiAdc_SyncSampleClock);
        }

        public void CopyC1C3_20GData()
        {
            if (acqModeAndInterleaveDefine.InterleaveMode != AdcInterleaveMode.Mode2To1)
            {
                return;
            }
            List<TiadcParamsKeyMap> tiadcParamsKeyMaps = new List<TiadcParamsKeyMap>()
            {
              new("C1-20G", (ChannelId.C1), 0),
              new("C1-20G", (ChannelId.C1), 1),
              new("C3-20G", (ChannelId.C3), 0),
              new("C3-20G", (ChannelId.C3), 1)
            };
            foreach (var item in acqModeAndInterleaveDefine.Details)
            {
                foreach (var adc in item.Value.First().AdcPorts)
                {
                    TiadcParamsKeyMap itemKey = new(acqModeAndInterleaveDefine!.Name, (ChannelId)item.Key, (uint)adc.Key);
                    TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(itemKey)!.Value;
                    var key = tiadcParamsKeyMaps.Where(p => p.chnlId == itemKey.chnlId && p.adcId == itemKey.adcId).First();
                    ProductDataTranslate_MSO8000X.SetTiadcParamsItem(key, tiadcItem);
                }
            }
        }


        /// <summary>
        /// 校准同步
        /// </summary>
        public bool CalcAdcPhaseData(bool isSetAdcPhase = true)
        {
            int sgFrequencyByHz = 100;//单位MHz
            double samplingRate = 10_000;//采样间隔，单位Sps
            double inputsignalFreqByMHz = 100d;
            double theoryDelta_pS = InterleaveMode == AdcInterleaveMode.Mode1To1 ? 1000d / 10 : 1000d / 20;

            #region 获取数据

            Dictionary<string, WaveOffsetGainPhase> keyValueWaveOffsetGainPhases = new Dictionary<string, WaveOffsetGainPhase>();
            Dictionary<string, WaveOffsetGainPhase> WaveOffsetGainPhaseerr = new Dictionary<string, WaveOffsetGainPhase>();
            string fintKey = string.Empty;
            int boardnum = 4;   //cyws
            int needrecvedByteCount = 0;
            switch (acqModeAndInterleaveDefine.Name)
            {
                case "C1-20G":
                case "C3-20G": needrecvedByteCount = sizeof(ushort) * ServerDomainConstants.PerAdcCoreDataCount * 2; boardnum = 1; break;
                case "C1C3-20G":
                case "All-10G": needrecvedByteCount = sizeof(ushort) * ServerDomainConstants.PerAdcCoreDataCount * ServerDomainConstants.AdcCount; break;
				case "All-20G": needrecvedByteCount = sizeof(ushort) * ServerDomainConstants.PerAdcCoreDataCount * ServerDomainConstants.AdcCount; break;
            }

            var adcData = InstrumentInteract.Factory_WaveData_Adc(currInstrumentSession, 6_000, null, needrecvedByteCount, boardnum);

            foreach (var item in acqModeAndInterleaveDefine.Details)
            {
                //通道一当前故障，只校准通道三和通道四
                if (acqModeAndInterleaveDefine.InterleaveMode == AdcInterleaveMode.Mode2To1 && item.Key == ChannelId.C1)
                {
                    continue;
                }
                //通道一当前故障，只校准通道三和通道四
                if (acqModeAndInterleaveDefine.InterleaveMode == AdcInterleaveMode.Mode1To1 && (item.Key == ChannelId.C1 || item.Key == ChannelId.C2))
                {
                    continue;
                }
                foreach (var adc in item.Value.First().AdcPorts)
                {
                    fintKey = acqModeAndInterleaveDefine.Name + "_" + item.Key + "_Adc" + adc.Key;
                    //当前两个采集板，固定通道1、2是采集板1，3、4是采集板2
                    int boardId = (item.Key) switch
                    {
                        ChannelId.C1 => 0,
                        ChannelId.C2 => 0,
                        _ => 1,
                    };
                    int index = boardId * Constants.ADC_NUM + adc.Key;
                    if (acqModeAndInterleaveDefine.InterleaveMode == AdcInterleaveMode.Mode2To1)
                    {
                        boardId = (item.Key) switch
                        {
                            ChannelId.C1 => 0,
                            ChannelId.C2 => 1,
                            ChannelId.C3 => 2,
                            ChannelId.C4 => 3
                        };
                        index = boardId;
                    }
                    var data = adcData[index];
                    ushort[] adcdata = data.ToArray();
                    if (data != null && data.Length > 0)
                    {
                        keyValueWaveOffsetGainPhases.Add(fintKey, SineFitFunc.SineFit(data.ToArray(), samplingRate, sgFrequencyByHz));
                    }
                }
            }

            //foreach (var item in acqModeAndInterleaveDefine.Details)
            //{
            //    foreach (var adc in item.Value.First().AdcPorts)
            //    {
            //        fintKey = acqModeAndInterleaveDefine.Name + "_" + item.Key + "_Adc" + adc.Key;
            //        var data = InstrumentInteract.Factory_WaveData_Adc(currInstrumentSession, 6_000, fintKey ?? "");
            //        if (data != null && data.Count > 0)
            //        {
            //            keyValueWaveOffsetGainPhases.Add(fintKey, SineFitFunc.SineFit(data.ToArray(), samplingRate, sgFrequencyByHz));
            //        }
            //    }
            //}
            //获取比较核
            fintKey = keyValueWaveOffsetGainPhases.First().Key;
            //计算三参数差
            foreach (var item in keyValueWaveOffsetGainPhases)
            {
                int adcIndex = fintKey == item.Key ? 0 : 1;
                double PhaseError_pS = ((item.Value.Phase - keyValueWaveOffsetGainPhases[fintKey].Phase + Math.PI * 2) % (Math.PI * 2)) * 1000_000 / inputsignalFreqByMHz / (2 * Math.PI) - adcIndex * theoryDelta_pS;
                double GainError = 100 * (item.Value.Gain - keyValueWaveOffsetGainPhases[fintKey].Gain) / keyValueWaveOffsetGainPhases[fintKey].Gain;
                double OffsetError = (item.Value.Offset - keyValueWaveOffsetGainPhases[fintKey].Offset);

                if (PhaseError_pS > 1000_000 / inputsignalFreqByMHz / 2)
                    PhaseError_pS -= 1000_000 / inputsignalFreqByMHz;
                else if (PhaseError_pS < -1000_000 / inputsignalFreqByMHz / 2)
                    PhaseError_pS += 1000_000 / inputsignalFreqByMHz;
                WaveOffsetGainPhaseerr.Add(item.Key, new WaveOffsetGainPhase()
                {
                    Gain = GainError,
                    Phase = PhaseError_pS,
                    Offset = OffsetError,
                });
            }

            caliAdcStatus = true;
            foreach (var item in AdcDeltas)
            {
                string key = acqModeAndInterleaveDefine.Name + "_" + (ChannelId)item.ChanelIndex + "_Adc" + item.AdcIndx;
                item.Delta = WaveOffsetGainPhaseerr[key].Phase;
                //判断相位差是否小于设定的误差范围
                if (Math.Abs(item.Delta) > PhaseErrorThreshold_fs)
                {
                    caliAdcStatus = false;
                }
            }

            #endregion

            #region 校准相位差

            if (!caliAdcStatus)
            {
                foreach (var item in AdcDeltas)
                {
                    int TA0 = (int)item.Delta % 100;
                    int fpgaDelay = (int)Math.Abs(item.Delta) / 100;
                    if (Math.Abs(TA0) >= 50 && Math.Abs(TA0) <= 100)
                    {
                        TA0 = TA0 > 0 ? 100 - TA0 : TA0 + 100;
                        fpgaDelay += 1;
                    }
                    string key = acqModeAndInterleaveDefine.Name + "_" + (ChannelId)item.ChanelIndex + "_Adc" + item.AdcIndx;
                    TiadcParamsKeyMap itemKey = new(acqModeAndInterleaveDefine!.Name, (ChannelId)item.ChanelIndex, item.AdcIndx);
                    TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(itemKey)!.Value;
                    if (item.Delta != 0)
                    {
                        item.AddVaule(new KeyValuePair<int, double>(tiadcItem.Phase, item.Delta));
                    }
                    //更新校准基数
                    item.Delta = TA0;
                    item.fpgaDelayer = fpgaDelay;
                    item.calc();
                    SetAcqData(itemKey, new int[] { (int)item.CaliDelta, item.fpgaDelayer }, item.AdcIndx, fintKey != key);
                }
                //发送TiAdc参数
                InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.TiadcPhaseOffsetGainParams);
                return !caliAdcStatus;
            }

            #endregion 校准相位差


            return !caliAdcStatus;
        }

        /// <summary>
        /// 获取对应交织模式的参数
        /// </summary>
        /// <param name="interleaveMode"></param>
        /// <returns></returns>
        public AcqModeAndInterleaveDefine GetAcqModeInterleaveDefine()
        {
            AcqModeAndInterleaveDefine currDetail = null;
            ServerSpecailData.Load(currInstrumentSession);
            foreach (var item in ServerSpecailData.JiHe_AcqModeInterleaveDefines)
            {
                if (item.Value.Name == adcInterleaveName)
                {
                    currDetail = item.Value;
                    if (currDetail != null)
                        break;
                }
            }
            return currDetail;
        }

        /// <summary>
        /// 初始化通道间延时
        /// </summary>
        public void InitAdcDelay(AcqModeAndInterleaveDefine currDetail)
        {
            foreach (var Detail in currDetail.Details)
            {
                foreach (var adc in Detail.Value.First().AdcPorts)
                {
                    TiadcParamsKeyMap tiadcParamsKeyMap = new TiadcParamsKeyMap(currDetail.Name, Detail.Key, (uint)adc.Key);
                    string keyMap = ProductDataTranslate_MSO8000X.GenerateTiadcParamsKey(tiadcParamsKeyMap);
                    ProductDataTranslate_MSO8000X.SetTiadcPhaseOffsetGainParamValue("AdcDelay_FPGA", 0, keyMap);
                    ProductDataTranslate_MSO8000X.SetTiadcPhaseOffsetGainParamValue("Phase", 32000, keyMap);
                }
            }
            InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.TiadcPhaseOffsetGainParams);
        }

        /// <summary>
        /// 初始化Adc校准参数实例
        /// </summary>
        /// <param name="currDetail"></param>
        public void InitAdcDelta(AcqModeAndInterleaveDefine currDetail)
        {
            int acqCount = 0;
            int model = currDetail.InterleaveMode == AdcInterleaveMode.Mode1To1 ? 1 : 0;
            int param = currDetail.InterleaveMode == AdcInterleaveMode.Mode1To1 ? 2 : 1;
            param = 0;
            AdcDeltas = new List<AdcDelta>();
            for (int chnlIndex = 0; chnlIndex < currDetail.Details.Values.Count - param; chnlIndex++)
            {
                foreach (var adc in currDetail.Details[(ChannelId)chnlIndex].First().AdcPorts)
                {
                    TiadcParamsKeyMap itemKey = new(currDetail.Name, (ChannelId)chnlIndex, (uint)adc.Key);
                    TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(itemKey)!.Value;
                    tiadcItem.AdcDelay_FPGA = 0;
                    AdcDelta adcDelta = new AdcDelta();
                    adcDelta.tiadcItem = tiadcItem;
                    adcDelta.itemKey = itemKey;
                    adcDelta.ModelIndex = model;
                    adcDelta.ChanelIndex = chnlIndex;
                    adcDelta.AdcIndx = (uint)adc.Key;
                    adcDelta.Index = acqCount;
                    adcDelta.adcInterleaveMode = currDetail.InterleaveMode;
                    acqCount++;
                    AdcDeltas.Add(adcDelta);
                }
            }

        }

        /// <summary>
        /// 设置通道参数
        /// </summary>
        /// <param name="adcInterleaveMode"></param>
        /// <returns></returns>
        private bool SwitchSamplingMode()
        {
            bool bOK = true;
            switch (adcInterleaveName)
            {
                case "C1_20G":
                    string cmdStrModeC1_20G = $":FACT:SOUR1:APPL 1,200,0,2,0,0,0,9,0,0";
                    bOK = currInstrumentSession.WriteString(cmdStrModeC1_20G);
                    break;
                case "C3_10G":
                    string cmdStrModeC3_10G = $":FACT:SOUR3:APPL 3,200,0,2,0,0,0,9,0,0";
                    bOK = currInstrumentSession.WriteString(cmdStrModeC3_10G);
                    break;
                case "C1C3_20G":
                    for (int i = 1; i < 5; i++)
                    {
                        int value = i == 2 || i == 4 ? 0 : 1;
                        string cmdStrMode2To1 = $":FACT:SOUR{i}:APPL {value},200,0,2,0,0,0,9,0,0";
                        bOK = currInstrumentSession.WriteString(cmdStrMode2To1);
                    }
                    break;
                case "All_10G":
                    string cmdStrMode1To1 = ":FACTory:ALLSource:APPLy 1,200,0,2,3,0,0,9,0,0";
                    bOK = currInstrumentSession.WriteString(cmdStrMode1To1);
                    break;
				case "All_20G":
                    string cmdStrModeAll_20G = ":FACTory:ALLSource:APPLy 1,200,0,2,3,0,0,9,0,0";
                    bOK = currInstrumentSession.WriteString(cmdStrModeAll_20G);
                    break;
                default:
                    break;
            }
            return bOK;
        }

        /// <summary>
        /// FPGA丢点规整
        /// </summary>
        /// <param name="currDetail"></param>
        public void SetAcqModeInterleaveFPGADelay(AcqModeAndInterleaveDefine currDetail)
        {
            List<int> delay = new List<int>();
            int param = currDetail.InterleaveMode == AdcInterleaveMode.Mode1To1 ? 2 : 1;
            param = 0;
            for (int chnlIndex = 0; chnlIndex < currDetail.Details!.Count - param; chnlIndex++)
            {
                foreach (var adc in currDetail.Details[(ChannelId)chnlIndex].First()!.AdcPorts)
                {

                    TiadcParamsKeyMap itemKey = new(currDetail.Name, (ChannelId)chnlIndex, (uint)adc.Key);
                    TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(itemKey)!.Value;
                    delay.Add(tiadcItem.AdcDelay_FPGA);
                }
            }
            int minDelay = delay.Min();
            for (int chnlIndex = 0; chnlIndex < currDetail.Details!.Count - param; chnlIndex++)
            {
                foreach (var adc in currDetail.Details[(ChannelId)chnlIndex].First()!.AdcPorts)
                {
                    TiadcParamsKeyMap itemKey = new(currDetail.Name, (ChannelId)chnlIndex, (uint)adc.Key);
                    TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(itemKey)!.Value;
                    tiadcItem.AdcDelay_FPGA -= minDelay;
                    ProductDataTranslate_MSO8000X.SetTiadcParamsItem(itemKey, tiadcItem);
                }
            }
        }

        /// <summary>
        /// 获取相位差相位差
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="sgFrequencyByHz"></param>
        /// <returns></returns>
        private static double GetDelta(double delta, int sgFrequencyByHz)
        {
            if (delta > 1000_000 / sgFrequencyByHz / 2)
                delta -= 1000_000 / sgFrequencyByHz;
            else if (delta < -1000_000 / sgFrequencyByHz / 2)
                delta += 1000_000 / sgFrequencyByHz;
            return delta;
        }

        /// <summary>
        /// 设置Adc的相位和丢点
        /// </summary>
        /// <param name="acqData"></param>
        /// <param name="data"></param>
        /// <param name="status"></param>
        /// <param name="AdcIndex"></param>
        /// <param name="IsUpdatePhase"></param>
        /// <returns></returns>
        private void SetAcqData(TiadcParamsKeyMap tiadcParamsKeyMap, int[] data, uint AdcIndex, bool IsUpdatePhase = true)
        {
            TiadcPhaseOffsetGainItem_Base tiadcItem = ProductDataTranslate_MSO8000X.GetTiadcParamsItem(tiadcParamsKeyMap)!.Value;
            tiadcItem.AdcDelay_FPGA = 0;
            if (IsUpdatePhase)
            {
                if (data[0] != 0)
                {
                    LogInfo($"{m_ModelType}校准前，PhaseErrAD{AdcIndex}：{tiadcItem.Phase}");
                    if (tiadcItem.Phase - data[0] < 0)
                    {
                        LogInfo($"超出Adc校准范围：当前：{tiadcItem.Phase} 校准值：{data[0]}");
                        calistatus = false;
                    }
                    else if (tiadcItem.Phase - data[0] > 65535)
                    {
                        LogInfo($"超出Adc校准范围：当前：{tiadcItem.Phase} 校准值：{data[0]}");
                        calistatus = false;
                    }
                    else
                    {
                        tiadcItem.Phase -= data[0];
                    }
                    LogInfo($"{m_ModelType}校准后，PhaseErrAD{AdcIndex}：{tiadcItem.Phase}");
                }
            }
            if (data[1] != 0)
            {
                if (tiadcItem.AdcDelay_FPGA + data[1] > 255)
                {
                    LogInfo($"超出FPGA丢点范围：当前AdcDelayErr_FPGAAD{AdcIndex}：{tiadcItem.Phase} 校准值：{data[0]}");
                    calistatus = false;
                }
                else
                {
                    LogInfo($"{m_ModelType}校准前，AdcDelayErr_FPGAAD{AdcIndex}：{tiadcItem.AdcDelay_FPGA}");
                    tiadcItem.AdcDelay_FPGA += data[1];
                    LogInfo($"{m_ModelType}校准后，AdcDelayErr_FPGAAD{AdcIndex}：{tiadcItem.AdcDelay_FPGA}");
                }
            }
            ProductDataTranslate_MSO8000X.SetTiadcParamsItem(tiadcParamsKeyMap, tiadcItem);
        }

        private class AdcDelta
        {
            //20G 0:A0 1:B1 2:A2 3:B3
            //10G 0:A0 1:A1 2:A3 3:A4
            public TiadcPhaseOffsetGainItem_Base tiadcItem;
            public TiadcParamsKeyMap itemKey;
            public int ChanelIndex;
            public int ModelIndex;
            public uint AdcIndx;
            public int Index;//adc索引
            public double Delta;//相位差
            public int fpgaDelayer;//丢点
            public int CaliDelta;
            public AdcInterleaveMode adcInterleaveMode;
            // 数据、偏差
            private List<KeyValuePair<int, double>> _RegErrPairs = new List<KeyValuePair<int, double>>();

            public void AddVaule(KeyValuePair<int, double> dd)
            {
                _RegErrPairs.Add(dd);
            }
            /// <summary>
            /// 斜率
            /// </summary>
            public double rate = 300;

            public void calc()
            {
                double ddd = 1;
                if (_RegErrPairs != null && _RegErrPairs.Count > 1)
                {
                    var param = _RegErrPairs[_RegErrPairs.Count - 1].Value - _RegErrPairs[_RegErrPairs.Count - 2].Value;
                    if (param == 0)
                    {
                        rate = 1;
                    }
                    else
                    {
                        if ((_RegErrPairs[_RegErrPairs.Count - 1].Value < 0 && _RegErrPairs[_RegErrPairs.Count - 2].Value > 0) || (_RegErrPairs[_RegErrPairs.Count - 2].Value < 0 && _RegErrPairs[_RegErrPairs.Count - 1].Value > 0))
                        {
                            ddd = Math.Abs(_RegErrPairs[_RegErrPairs.Count - 1].Value) / (Math.Abs(_RegErrPairs[_RegErrPairs.Count - 1].Value) + Math.Abs(_RegErrPairs[_RegErrPairs.Count - 2].Value));
                        }
                        rate = (_RegErrPairs[_RegErrPairs.Count - 1].Key - _RegErrPairs[_RegErrPairs.Count - 2].Key) / (_RegErrPairs[_RegErrPairs.Count - 1].Value - _RegErrPairs[_RegErrPairs.Count - 2].Value);
                    }
                }
                else
                {
                    rate = adcInterleaveMode == AdcInterleaveMode.Mode1To1 ? 266 : 323;
                    if (Math.Abs(Delta) < 5)
                    {
                        rate = adcInterleaveMode == AdcInterleaveMode.Mode1To1 ? 10 : 323;
                    }
                    if (Math.Abs(Delta) < 2.5)
                    {
                        rate = adcInterleaveMode == AdcInterleaveMode.Mode1To1 ? 5 : 50;
                    }
                }
                rate = Math.Abs(rate);
                if (rate <= 3) { rate = 3; }

                CaliDelta = (int)((rate * Delta) * ddd);
                if (Delta != 0)
                {
                    LogInfo($"斜率：{rate} 基数：{Delta}");
                }
            }
        }
    }
}

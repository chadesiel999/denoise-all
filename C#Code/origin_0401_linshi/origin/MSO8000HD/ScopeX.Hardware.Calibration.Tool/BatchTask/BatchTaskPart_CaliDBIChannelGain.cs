using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    public class BatchTaskPart_CaliDbiChannelGain : BatchTaskPartBase
    {
        enum AutoCaliAmpStatus
        {
            Working = 1,
            Failure = 2,
            Ok = 3
        }
        enum AutoCaliAmp_Stage
        {
            DSA = 0,
            ADC = 1,
            FPGA = 2,
            End = 3
        }
        public override string FuncionDescription
        {
            get => $"Dbi子带增益校准。";
        }
        public override string ParametersDescription
        {
            get => $"第1个参数：表示通道，编号从1开始,只能是一个通道；{System.Environment.NewLine}" +
                    $"第2个参数：子带编号，编号从1开始,只能是一个子带；{System.Environment.NewLine}" +
                    $"第3个参数，表示高阻还是低阻，low=50欧姆，低阻，high表示高阻。{System.Environment.NewLine}" +
                    $"第4个参数，是否是满带宽模式，true或flase。{System.Environment.NewLine}" +
                    $"第5个参数，以mV为单位的档位值，浮点数{System.Environment.NewLine}" +
                    $"第6个参数，为输入信号的幅度值，以mV为单位，浮点数{System.Environment.NewLine}" +
                    $"第7个参数，表示需要经历的阶段，包括 ：Dsa_Adc_Fpga , Dsa , Dsa_Adc , Dsa_Fpga,Adc_Fpga,Fpga{System.Environment.NewLine}" +
                    $"第8个参数，DSA阶段每次采集前延时时间，用ms表示{System.Environment.NewLine}" +
                    $"第9个参数，ADC阶段每次采集前延时时间，用ms表示{System.Environment.NewLine}" +
                    $"第10个参数，Fpga阶段每次采集前延时时间，用ms表示{System.Environment.NewLine}" +
                    $"第11个参数，DSA每个控制字幅度改变百分比，也就是DSA的可调精度{System.Environment.NewLine}" +
                    $"第12个参数: ADC每个控制字幅度改变百分比，也就是ADC的可调精度{System.Environment.NewLine}" +

                    $"第13个参数: 总体误差判据，百分比表示{System.Environment.NewLine}" +
                    $"第14个参数: Adc阶段跳出误差，百分比表示{System.Environment.NewLine}" +
                    $"第15个参数: 是否复制到半带宽{System.Environment.NewLine}" +
                    $"第16个参数: ADC控制字复位，true或flase{System.Environment.NewLine}" +
                    $"第17个参数: Fpga控制字复位，true或flase{System.Environment.NewLine}";
        }
        public override string Example
        {
            get => "";
        }
        private void GotoNextStage(ref AutoCaliAmp_Stage stage, double currErrorByPercent)
        {
            if (stage == AutoCaliAmp_Stage.DSA)
            {
                if (bIncludeAdcStage)
                    stage = AutoCaliAmp_Stage.ADC;
                else if (bIncludeFpgaStage)
                    stage = AutoCaliAmp_Stage.FPGA;
                else
                {
                    stage = AutoCaliAmp_Stage.End;
                }

            }
            else if (stage == AutoCaliAmp_Stage.ADC)
            {
                if (bIncludeFpgaStage)
                    stage = AutoCaliAmp_Stage.FPGA;
                else
                {
                    stage = AutoCaliAmp_Stage.End;
                }

            }
            else if (stage == AutoCaliAmp_Stage.FPGA)
                stage = AutoCaliAmp_Stage.End;
            if (Math.Abs(currErrorByPercent) < TotalErrorByPercent)
            {
                stage = AutoCaliAmp_Stage.End;
            }
        }

        private bool oldValid = false;
        int channelID = 0;
        int subbandIndex = 0;
        long SourceAmpVoltByuV = 0;
        int inputLevelBymV = 10000;
        int inputLevelID = 0;
        bool bIncludeDsaStage = false;
        bool bIncludeAdcStage = false;
        bool bIncludeFpgaStage = false;
        bool bCopy2HalfBandwidth = false;
        bool bIsFullbandwidth = true;
        double DSAStage_PerStepChangedPercent = 1;
        double ADCStage_PerStepChangedPercent = -0.005;
        int DelayMsAfterHardwareChannged_DSAStage = 500;
        int DelayMsAfterHardwareChannged_AdcStage = 2500;
        int DelayMsAfterHardwareChannged_FpgaStage = 1000;
        double TotalErrorByPercent = 1.0;
        double AdcErrorByPercent = 1.0;
        bool bResetCaliCtrlWord_Adc = false;
        bool bResetCaliCtrlWord_Fpga = false;
        private bool AnalyParameter(string parameter)
        {
            if (parameterStr == parameter)
                return oldValid;
            parameterStr = parameter;
            string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramList.Length < 4)
            {
                oldValid = false;
                return false;
            }
            //[0]: channelID
            channelID = int.Parse(paramList[0]) - 1;
            //[1]: subBandIndex
            subbandIndex = int.Parse(paramList[1]) - 1;
            //[2]:高阻、低阻，低阻用Low表示，High表示高阻
            int impedance = paramList[2].ToUpper() switch
            {
                "HIGH" => 0,
                _ => 1
            };
            //[3]是否是满带宽模式，true或flase。
            bIsFullbandwidth = (paramList[2].Trim() == "true" || paramList[3].Trim() == "1");
            //[4]:幅度档，用mV为单位
            inputLevelBymV = BaseHelper.TryConvertToInt(paramList[4]);
            if (inputLevelBymV == 0)
            {
                oldValid = false;
                return false;
            }
            inputLevelBymV *= 1000;//mv=>uV
            inputLevelID = Array.IndexOf<Int32>(AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV.ToArray(), inputLevelBymV);
            if (inputLevelID < 0)
            {
                oldValid = false;
                return false;
            }
            //[5]：输入信号的幅度，用mV为单位
            SourceAmpVoltByuV = BaseHelper.TryConvertToLong(paramList[5]);
            if (SourceAmpVoltByuV < 0)
            {
                oldValid = false;
                return false;
            }
            SourceAmpVoltByuV *= 1000;//mv-->uV
            //[6]:阶段法，包括Dsa_Adc_Fpga,Dsa,Dsa_Adc,Dsa_Fpga4种类型
            string tmpStr = paramList[6].ToUpper();
            if (tmpStr.IndexOf("DSA") >= 0)
                bIncludeDsaStage = true;
            if (tmpStr.IndexOf("ADC") >= 0)
                bIncludeAdcStage = true;
            if (tmpStr.IndexOf("FPGA") >= 0)
                bIncludeFpgaStage = true;
            //[7]:每次DSA阶段采集前延时时间，用ms表示
            DelayMsAfterHardwareChannged_DSAStage = int.Parse(paramList[7]);
            //[8]:每次ADC阶段采集前延时时间，用ms表示
            DelayMsAfterHardwareChannged_AdcStage = int.Parse(paramList[8]);
            //[9]:每次Fpga阶段采集前延时时间，用ms表示
            DelayMsAfterHardwareChannged_FpgaStage = int.Parse(paramList[9]);

            //[10]:DSA每个控制字幅度改变百分比，也就是DSA的可调精度
            DSAStage_PerStepChangedPercent = double.Parse(paramList[10]);
            //[11]:ADC每个控制字幅度改变百分比，也就是ADC的可调精度
            ADCStage_PerStepChangedPercent = double.Parse(paramList[11]);
            //[12]:总体误差判据，百分比表示
            TotalErrorByPercent = double.Parse(paramList[12]);
            if (paramList.Length > 13)
                AdcErrorByPercent = double.Parse(paramList[13]);
            if (paramList.Length > 14)
                bCopy2HalfBandwidth = (paramList[14].Trim() == "true" || paramList[14].Trim() == "1");
            if (paramList.Length > 15)
                bResetCaliCtrlWord_Adc = (paramList[15].Trim() == "true" || paramList[15].Trim() == "1");
            if (paramList.Length > 16)
                bResetCaliCtrlWord_Fpga = (paramList[16].Trim() == "true" || paramList[16].Trim() == "1");
            oldValid = true;
            return true;
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
        private const int NeedStaticTimes = 2;
        private const int MinCourseCtrlWord = 0;
        private const int MaxCourseCtrlWord = 65535;
        record StageParams()
        {
            public int DelayMs;
            public double PerStepChangedPercent;
            public int CtrlWord_Min;
            public int CtrlWord_Max;
            public double LastErrorByPrecent;
        }
        private int GetCaliValue(DbiAnalogChannelSubbandItem currItem, AutoCaliAmp_Stage currStage)
        {
            if (currStage == AutoCaliAmp_Stage.DSA)
                return (int)currItem.SubbandGain;
            else if (currStage == AutoCaliAmp_Stage.ADC)
                return (int)currItem.Gain_FineByAdc1ByTenThousand;
            else if (currStage == AutoCaliAmp_Stage.FPGA)
                return (int)currItem.Gain_FineByFpgaThousand;
            else
                return 0;
        }
        private void SetCaliValue(ref DbiAnalogChannelSubbandItem currItem, AutoCaliAmp_Stage currStage, int ctrlCowrd)
        {
            if (currStage == AutoCaliAmp_Stage.DSA)
                currItem.SubbandGain = (uint)ctrlCowrd;
            else if (currStage == AutoCaliAmp_Stage.ADC)
            {
                currItem.Gain_FineByAdc1ByTenThousand = (uint)ctrlCowrd;
                currItem.Gain_FineByAdc2ByTenThousand = (uint)ctrlCowrd;
            }
            else if (currStage == AutoCaliAmp_Stage.FPGA)
                currItem.Gain_FineByFpgaThousand = (uint)ctrlCowrd;
        }
        public override BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null)
        {
            errorCount = 0;
            if (!oldValid)
            {
                message = "参数错误！";
                return BatchTaskPartResult.ErrorParameter;
            }

            BatchTaskPartResult batchTaskPartResult = BatchTaskPartResult.ErrorGeneral;

            long maxWaitMilliseconds = 5 * 1000;
            if (overtimeOfSecond > 0)
                maxWaitMilliseconds = (long)(1000 * overtimeOfSecond);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            int AdcMaxValue = (int)Math.Pow(2, ServerDomainConstants.AdcBits);
            AutoCaliAmp_Stage currStage = AutoCaliAmp_Stage.End;
            if (bIncludeDsaStage)
                currStage = AutoCaliAmp_Stage.DSA;
            else if (bIncludeAdcStage)
                currStage = AutoCaliAmp_Stage.ADC;
            else if (bIncludeFpgaStage)
                currStage = AutoCaliAmp_Stage.FPGA;
            currInstrumentSession?.WriteString($":MEASure:ITEM1:SOURce C{subbandIndex + 1}");
            currInstrumentSession?.WriteString($":MEASure:ITEM1:TYPe AMPL");

            int BWMode_FullIs0 = 0;
            Dictionary<AutoCaliAmp_Stage, StageParams> AllStageParams = new Dictionary<AutoCaliAmp_Stage, StageParams>()
            {
                [AutoCaliAmp_Stage.DSA] = new() { CtrlWord_Min = 0, CtrlWord_Max = 127, LastErrorByPrecent = 1000, DelayMs = DelayMsAfterHardwareChannged_DSAStage, PerStepChangedPercent = DSAStage_PerStepChangedPercent },
                [AutoCaliAmp_Stage.ADC] = new() { CtrlWord_Min = 2500, CtrlWord_Max = 40000, LastErrorByPrecent = 1000, DelayMs = DelayMsAfterHardwareChannged_AdcStage, PerStepChangedPercent = ADCStage_PerStepChangedPercent },
                [AutoCaliAmp_Stage.FPGA] = new() { CtrlWord_Min = 250, CtrlWord_Max = 10000, LastErrorByPrecent = 1000, DelayMs = DelayMsAfterHardwareChannged_FpgaStage, PerStepChangedPercent = -0.1 }
            };
            DbiAnalogChannelSubbandItem currItem = DbiAnalogParams.Default[BWMode_FullIs0, channelID, inputLevelID, subbandIndex];
            //ADC增益，FPGA增益 初始化为缺省状态
            int delayMs = 1;
            if (bResetCaliCtrlWord_Adc)
            {
                currItem.Gain_FineByAdc1ByTenThousand = 10000;
                currItem.Gain_FineByAdc2ByTenThousand = 10000;
            }
            if (bResetCaliCtrlWord_Fpga)
                currItem.Gain_FineByFpgaThousand = 1000;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            double errorByPercent = 10;
            while (currStage != AutoCaliAmp_Stage.End && stopwatch.ElapsedMilliseconds < maxWaitMilliseconds)//1000,s=>ms
            {
                # region 校准数据生效
                DbiAnalogParams.Default[BWMode_FullIs0, channelID, inputLevelID, subbandIndex] = currItem;
                InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.DbiAnalogParams);
                Thread.Sleep(AllStageParams[currStage].DelayMs);
                #endregion

                #region 统计
                double currTotalStaticMeasureValue_uV = 0;
                bool bOverAdc = false;
                for (int staticTimes = 0; staticTimes < NeedStaticTimes; staticTimes++)
                {
                    //获取波形数据
                    List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(currInstrumentSession, 6_000, 4);//4个子带

                    if (allChannelData == null)
                    {
                        message = "数据读取错误！";
                        return BatchTaskPartResult.ErrorFatal;
                    }
                    #region 判断采集是否越界
                    long max = allChannelData[subbandIndex].Max<ushort>();
                    long min = allChannelData[subbandIndex].Min<ushort>();
                    if ((max >= AdcMaxValue - 1) || (min <= 0))//上下都越界，调整幅度
                    {
                        bOverAdc = true;
                        break;
                    }
                    #endregion
                    //继续-读取参数测量值
                    string measureResultStr = "";
                    Stopwatch stopwatchReadMeasure = new Stopwatch();
                    stopwatchReadMeasure.Start();
                    while (measureResultStr.Trim() == "")
                    {
                        currInstrumentSession!.WriteString($":MEASure:ITEM1:VAL?");
                        measureResultStr = currInstrumentSession!.ReadShortString();
                        if (stopwatchReadMeasure.ElapsedMilliseconds > 10_000)
                        {
                            message = "数据读取错误！";
                            return BatchTaskPartResult.ErrorFatal;
                        }
                        if (cancelTokenSrc != null)
                        {
                            try
                            {
                                cancelTokenSrc.Token.ThrowIfCancellationRequested();
                            }
                            catch
                            {
                                message = "任务被取消";
                                return BatchTaskPartResult.Cancel;
                            }
                        }
                    }
                    currTotalStaticMeasureValue_uV += double.Parse(measureResultStr) * 1e6;//10e6 v=>uV
                }
                #endregion

                #region 调整
                if (bOverAdc)
                    errorByPercent = 50.0;//6DIV ,每DIV 400个ADC量化值，共2400个量化字，4096/2400=1.70 ,1.70*100,取小一点，避免立即超过控制字范围
                else
                    errorByPercent = ((currTotalStaticMeasureValue_uV / NeedStaticTimes) - SourceAmpVoltByuV) * 100 / SourceAmpVoltByuV;

                if (Math.Abs(errorByPercent) < TotalErrorByPercent)
                {
                    currStage = AutoCaliAmp_Stage.End;
                    break;
                }
                int ctrlWordDelta = (int)((errorByPercent) / AllStageParams[currStage].PerStepChangedPercent);

                if (ctrlWordDelta == 0)
                    ctrlWordDelta = (int)(errorByPercent / Math.Abs(errorByPercent));
                if (Math.Abs(AllStageParams[currStage].LastErrorByPrecent) < errorByPercent && Math.Abs(ctrlWordDelta) == 1)
                {
                    GotoNextStage(ref currStage, errorByPercent);
                }
                else
                {
                    int newCtrlWord = GetCaliValue(currItem, currStage) + ctrlWordDelta;
                    if (newCtrlWord < AllStageParams[currStage].CtrlWord_Min)
                    {
                        stringBuilder.AppendLine($"[×CH{channelID + 1}]_Subband{subbandIndex + 1}，[{currStage}]阶段 已到最小值。");
                        SetCaliValue(ref currItem, currStage, AllStageParams[currStage].CtrlWord_Min);
                        GotoNextStage(ref currStage, errorByPercent);
                    }
                    else if (newCtrlWord > AllStageParams[currStage].CtrlWord_Max)
                    {
                        stringBuilder.AppendLine($"[×CH{channelID + 1}]_Subband{subbandIndex + 1}，[{currStage}]阶段 已到最大值。");
                        SetCaliValue(ref currItem, currStage, AllStageParams[currStage].CtrlWord_Max);
                        GotoNextStage(ref currStage, errorByPercent);
                    }
                    else
                    {
                        SetCaliValue(ref currItem, currStage, newCtrlWord);
                    }
                }
                if (cancelTokenSrc != null)
                {
                    try
                    {
                        cancelTokenSrc.Token.ThrowIfCancellationRequested();
                    }
                    catch
                    {
                        message = "任务被取消";
                        return BatchTaskPartResult.Cancel;
                    }
                }
                #endregion
            }
            if (currStage != AutoCaliAmp_Stage.End)
            {
                stringBuilder.AppendLine($"[×CH{channelID + 1}]_Subband{subbandIndex + 1}，超时没有校准好。");
                batchTaskPartResult = BatchTaskPartResult.ErrorOvertime;
            }
            else if (Math.Abs(errorByPercent) > TotalErrorByPercent)
            {
                stringBuilder.AppendLine($"[×CH{channelID + 1}]_Subband{subbandIndex + 1}，控制字超出可调范围，没有校准好。");
                batchTaskPartResult = BatchTaskPartResult.ErrorGeneral;
            }
            else
            {
                stringBuilder.AppendLine($"[CH{channelID + 1}]_Subband{subbandIndex + 1}，已经校准好。");
                batchTaskPartResult = BatchTaskPartResult.Succeed;
            }
            if (bCopy2HalfBandwidth)
            {
                #region 复制全带宽的参数到半带宽
                for (int i = 0; i < 4; i++)
                {
                    //半带宽的数据 等于全带宽的数据
                    DbiAnalogParams.Default[1, channelID, inputLevelID, i] = DbiAnalogParams.Default[BWMode_FullIs0, channelID, inputLevelID, i];
                }
                #endregion

                InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.DbiAnalogParams);
            }
            message = stringBuilder.ToString();
            return batchTaskPartResult;
        }
    }
}

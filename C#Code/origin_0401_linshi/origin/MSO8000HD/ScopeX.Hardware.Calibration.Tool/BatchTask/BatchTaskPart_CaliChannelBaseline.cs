using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;
using System.Threading.Channels;
using ScopeX.Hardware.Calibration.Tool.Utilities;
using static ScopeX.Hardware.Calibration.Data.Base.ProductDataTranslate_MSO8000X;
using System.Reflection;
using Microsoft.Office.Interop.Excel;

namespace ScopeX.Hardware.Calibration.Tool
{
    public class BatchTaskPart_CaliChannelBaseLine : BatchTaskPartBase
    {
        enum AutoCaliOffsetStatus
        {
            Working = 1,
            Failure = 2,
            Ok = 3
        }

        /// <summary>
        /// 波形的状态
        /// </summary>
        enum WaveStatus
        {
            Normal, //正常，在±4Div内
            OverTop,    //超过上边界，都在+4Div以上
            OverBottom  //超过下边界，都在-4Div以下

        }

        class PerChannel
        {
            public AnaChnlIpnutSource IpnutSource = AnaChnlIpnutSource.BNC;
            public int channelID;
            public int yLevelID;
            public int Impedance;
            public int StaticTimes = 0;
            public List<double> MeasureValueUvs = new List<double>();
            public long TotalStaticBaseLine = 0;
            public long LastTotalStaticBaseLine = 0;
            public long LastMeasureValue_uV = 0;
            public bool bBaselineCtrlWordChanged = false;
            public bool bBaseline0Div = false;
            public uint LastOffsetCtrlWord = 0;
            public uint LastUpperOffsetCtrlWord = 0;
            public uint LastLowerOffsetCtrlWord = 0;
            public AnalogChannelItem_Base analogChannelItem_Base;
            public ChnlParamsKeyMap chnlParams;
            public string paramName;
            public (uint reg, double avg)? FirstStepInPoint = null;
            public (uint reg, double avg)? SecondStepInPoint = null;
            public AutoCaliOffsetStatus Status = AutoCaliOffsetStatus.Working;



            public uint Cali_FPGAFinegainWord
            {
                get
                {
                    return (uint)ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlParams).Value.Gain_FineByFpgaThousand;
                    //if (IpnutSource == AnaChnlIpnutSource.BNC)
                    //    return ChannelParams.Default[channelID, Impedance, yLevelID].Gain_FineByFpgaThousand;
                    //else
                    //    return ChannelParams.Default[channelID, Impedance, yLevelID].Gain_FineByFpgaThousand;
                }
            }

            public uint Cali_ChannelOffset
            {
                get
                {
                    return (uint)ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlParams).Value.Offset;
                    //if (IpnutSource == AnaChnlIpnutSource.BNC)
                    //    return ChannelParams.Default[channelID, Impedance, yLevelID].OffsetPosterior;
                    //else
                    //    return ChannelParamsModel2.Default[channelID, Impedance, yLevelID].OffsetPosterior;
                }
                set
                {
                    ProductDataTranslate_MSO8000X.SetAnalogChannelParamValue("Offset", (uint)value, paramName);
                    //if (IpnutSource == AnaChnlIpnutSource.BNC)
                    //{
                    //    ChannelPerScaleItem perScaleItem = ChannelParams.Default[channelID, Impedance, yLevelID];
                    //    perScaleItem.OffsetPosterior = value;
                    //    //DataStruct refactoring: send during calibration
                    //    ChannelParams.Default[channelID, Impedance, yLevelID] = perScaleItem;
                    //}
                    //else
                    //{
                    //    ChannelPerScaleItem perScaleItem = ChannelParamsModel2.Default[channelID, Impedance, yLevelID];
                    //    perScaleItem.OffsetPosterior = value;
                    //    ChannelParamsModel2.Default[channelID, Impedance, yLevelID] = perScaleItem;
                    //}
                }
            }
            public uint Cali_Channel3Div
            {
                get
                {
                    return (uint)ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlParams).Value.Offset_Pos3Div;

                    //if (IpnutSource == AnaChnlIpnutSource.BNC)
                    //    return ChannelParams.Default[channelID, Impedance, yLevelID].OffsetPosterior_3Div;
                    //else
                    //    return ChannelParamsModel2.Default[channelID, Impedance, yLevelID].OffsetPosterior_3Div;
                }
                set
                {
                    ProductDataTranslate_MSO8000X.SetAnalogChannelParamValue("Offset_Pos3Div", (uint)value, paramName);
                    //if (IpnutSource == AnaChnlIpnutSource.BNC)
                    //{
                    //    ChannelPerScaleItem perScaleItem = ChannelParams.Default[channelID, Impedance, yLevelID];
                    //    perScaleItem.OffsetPosterior_3Div = value;
                    //    ChannelParams.Default[channelID, Impedance, yLevelID] = perScaleItem;
                    //}
                    //else
                    //{
                    //    ChannelPerScaleItem perScaleItem = ChannelParamsModel2.Default[channelID, Impedance, yLevelID];
                    //    perScaleItem.OffsetPosterior_3Div = value;
                    //    ChannelParamsModel2.Default[channelID, Impedance, yLevelID] = perScaleItem;
                    //}
                }
            }
            /// <summary>
            /// 后级偏负三格系数
            /// </summary>
            public uint Cali_ChannelN3Div
            {
                get
                {
                    return (uint)ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlParams).Value.Offset_Neg3Div;
                }
                set
                {
                    ProductDataTranslate_MSO8000X.SetAnalogChannelParamValue("Offset_Neg3Div", (uint)value, paramName);
                }
            }

            public void UpdateLastUpperOffsetCtrlWord(uint newWord, bool isDesDiv)
            {
                if (Math.Abs((int)newWord - (int)LastLowerOffsetCtrlWord) < Math.Abs((int)LastUpperOffsetCtrlWord - (int)LastLowerOffsetCtrlWord))
                {
                    if (isDesDiv)
                        LastLowerOffsetCtrlWord = newWord;
                    else
                        LastUpperOffsetCtrlWord = newWord;
                }
            }

            public void UpdateLastLowerOffsetCtrlWord(uint newWord, bool isDesDiv)
            {
                if (Math.Abs((int)newWord - (int)LastUpperOffsetCtrlWord) < Math.Abs((int)LastLowerOffsetCtrlWord - (int)LastUpperOffsetCtrlWord))
                {
                    if (isDesDiv)
                        LastUpperOffsetCtrlWord = newWord;
                    else
                        LastLowerOffsetCtrlWord = newWord;
                }
            }
        }

        static List<double> slope = new List<double>() { 0, 0, 0, 0, 0, 0 };
        private string _ChnlsText = "";
        List<PerChannel> caliChannels = new List<PerChannel>();
        private bool oldValid = false;
        double inputLevelBymV = 10000;
        int DesDiv = 0;
        int DelayMsAfterHardwareChannged = 300;
        double baseErrorLimitByPercentDiv = 1.0;
        uint MinOffsetCtrlWord = 0;
        uint MaxOffsetCtrlWord = 50000;
        int NeedStaticTimes = 3;
        int Step = 100;
        long maxWaitMilliseconds = 100;
        bool bfirstStatic = false;
        uint Delta = 0;
        long _UniqueId = 0;

        public override string FuncionDescription
        {
            get => $"基线校准。";
        }
        public override string ParametersDescription
        {
            get => $"[0]其中1|2 表示通道，编号从1开始；{System.Environment.NewLine}" +
                    $"[1]表示高阻还是低阻，low=50欧姆，低阻，higf表示高阻{System.Environment.NewLine}" +
                    $"[2]以mV为单位的档位值，浮点数{System.Environment.NewLine}" +
                    $"[3]0格或3格{System.Environment.NewLine}" +
                    $"[4]通道最小粗调值{System.Environment.NewLine}" +
                    $"[5]通道最大粗调值{System.Environment.NewLine}" +
                    $"[6]调节步进值{System.Environment.NewLine}" +
                    $"[7]总体误差判据，LSB表示 {System.Environment.NewLine}" +
                    $"[8]统计次数 {System.Environment.NewLine}" +
                    $"[10]校准数据生效时间，用ms表示{System.Environment.NewLine}" +
                    $"[11]最大超时时间，用s表示{System.Environment.NewLine}" +
                    $"[12]offset3Div阈值,8G通道专用{System.Environment.NewLine}";
        }
        public override string Example
        {
            get => "";
        }

        private bool AnalyParameter(string parameter)
        {
            if (parameterStr == parameter)
                return oldValid;
            caliChannels.Clear();

            parameterStr = parameter;
            string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramList.Length < 4)
            {
                oldValid = false;
                return false;
            }
            //channel list
            //[0]: channel list
            _ChnlsText = paramList[0];
            string[] channelIDList = _ChnlsText.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (channelIDList.Length == 0)
            {
                oldValid = false;
                return false;
            }
            //[1]:高阻、低阻，低阻用Low表示，High表示高阻
            int impedance = paramList[1].ToUpper() switch
            {
                "HIGH" => 0,
                _ => 1
            };
            //[2]:幅度档，用mV为单位
            inputLevelBymV = BaseHelper.TryConvertToDouble(paramList[2]);
            if (inputLevelBymV == 0)
            {
                oldValid = false;
                return false;
            }
            inputLevelBymV *= 1000;//mv=>uV
            int inputLevelID = Array.IndexOf<Int32>(AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV.ToArray(), (int)inputLevelBymV);
            if (inputLevelID < 0)
            {
                oldValid = false;
                return false;
            }
            //[3]:0格或3格
            DesDiv = int.Parse(paramList[3]);
            //[4]:通道最小粗调值
            MinOffsetCtrlWord = uint.Parse(paramList[4]);
            //[5]:通道最大粗调值
            MaxOffsetCtrlWord = uint.Parse(paramList[5]);
            //[6]:调节步进值
            Step = int.Parse(paramList[6]);
            //[7]:总体误差判据，百分比表示
            baseErrorLimitByPercentDiv = double.Parse(paramList[7]);
            //[8]:统计次数
            NeedStaticTimes = int.Parse(paramList[8]) + 2;
            //[9]:校准数据生效时间，用ms表示
            DelayMsAfterHardwareChannged = int.Parse(paramList[9]);
            //[10]：最大超时时间
            maxWaitMilliseconds = long.Parse(paramList[10]);
            //[11]:是否计算步进
            bfirstStatic = bool.Parse(paramList[11]);
            //[12]:offset3Div阈值,8G通道专用
            if (paramList.Length > 12)
            {
                Delta = uint.Parse(paramList[12]);
            }

            foreach (string channelId in channelIDList)
            {
                int id = BaseHelper.TryConvertToInt(channelId);
                if (id <= 0 || id > ServerDomainConstants.AnalogChannelCount)
                {
                    oldValid = false;
                    return false;
                }
                PerChannel perChannel = new PerChannel();
                perChannel.LastUpperOffsetCtrlWord = MaxOffsetCtrlWord;
                perChannel.LastLowerOffsetCtrlWord = MinOffsetCtrlWord;
                perChannel.channelID = id - 1;
                perChannel.yLevelID = inputLevelID;
                perChannel.Impedance = impedance;
                perChannel.chnlParams = new ChnlParamsKeyMap((ChannelId)perChannel.channelID, impedance == 0 ? true : false, (uint)inputLevelBymV / 1000);
                perChannel.paramName = ProductDataTranslate_MSO8000X.GenerateChnlParamsKey(perChannel.chnlParams);
                caliChannels.Add(perChannel);
            }
            if (caliChannels.Count == 0)
            {
                oldValid = false;
                return false;
            }
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
        private void LogInfo(string info)
        {
            Utilities.Logger.LogCaliInfo(_UniqueId, "Log", "BatchTaskPart_CaliChannelBaseLine", info);
        }
        public override void SetDebugVariantStatus(bool status)
        {
            CommonMethod.Set_CorrectTiAdc(currInstrumentSession, status);
            CommonMethod.SetChannelDelay(currInstrumentSession, status);
        }

        public override BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null)
        {
            AnaChnlIpnutSource anaChnlIpnutSource = AnaChnlIpnutSource.BNC;
            CommonMethod.RefreshConstDataFromServer(currInstrumentSession);
            string scpiCMD = $":FACT:CHAN{caliChannels[0].channelID + 1}:INP ?";
            currInstrumentSession!.WriteString(scpiCMD);
            string result = currInstrumentSession.ReadShortString();
            if (result.Trim().ToUpper() == "SMA")
                anaChnlIpnutSource = AnaChnlIpnutSource.SMA;
            foreach (var v in caliChannels)
                v.IpnutSource = anaChnlIpnutSource;
            Int32 PerYDivAdcSamples = (int)ServerDomainConstants.SAMPS_PER_YDIV;
            int errorCount = 0;
            maxWaitMilliseconds = maxWaitMilliseconds * 1000;
            BatchTaskPartResult batchTaskPartResult = BatchTaskPartResult.ErrorGeneral;
            StringBuilder stringBuilder = new StringBuilder();
            message = "";
            int WorkingFlag = 0;
            foreach (PerChannel perChannel in caliChannels)
                WorkingFlag |= 1 << perChannel.channelID;
            int AdcMaxValue = (int)Math.Pow(2, ServerDomainConstants.AdcBits);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            int[] step = new int[] { Step, Step, Step, Step, Step, Step };
            List<long> TotalStaticBaseLine = new List<long>() { 0, 0, 0, 0, 0, 0 };
            List<uint> delta1 = new List<uint>() { 0, 0, 0, 0, 0, 0 };
            List<uint> delta2 = new List<uint>() { 0, 0, 0, 0, 0, 0 };
            List<long> average1 = new List<long>() { 0, 0, 0, 0, 0, 0 };
            List<long> average2 = new List<long>() { 0, 0, 0, 0, 0, 0 };
            List<uint> oldCali_ChannelOffset = new List<uint>() { 0, 0, 0, 0, 0, 0 };
            List<bool> bfirstStep = new List<bool>() { true, true, true, true, true, true };
            foreach (PerChannel channel in caliChannels)
            {
                currInstrumentSession?.WriteString($":CHAN{(int)channel.channelID + 1}:OFFSet {DesDiv}");
                channel.Status = AutoCaliOffsetStatus.Working;
            }
            #region WCJ处理
            _UniqueId = DateTime.Now.Ticks;
            LogInfo($"(ExecuteId={_UniqueId};)BatchTaskPart_CaliChannelBaseLine_Start:Impedance_{caliChannels[0].Impedance};" +
                $"inputLevelBymV_{inputLevelBymV / 1000};Chnls_{_ChnlsText};DesDiv_{DesDiv}");
            CommonMethod.SetTemperatureCompensate(currInstrumentSession, false);

            //校准
            while (caliChannels.Any(ch => ch.Status == AutoCaliOffsetStatus.Working))
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
                //if (stopwatch.ElapsedMilliseconds > maxWaitMilliseconds)
                if (stopwatch.ElapsedMilliseconds > maxWaitMilliseconds*4)
                {
                    message = $"(ExecuteId={_UniqueId};)超时退出";
                    LogInfo("校准结果：" + message);
                    CommonMethod.SetTemperatureCompensate(currInstrumentSession, true);
                    return BatchTaskPartResult.ErrorOvertime;
                }
                //校准数据生效
                InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.AnalogParams);
                Thread.Sleep(DelayMsAfterHardwareChannged*2);
                


                ///获取波形数据
                List<ushort[]>? allChannelData = new List<ushort[]>();
                for (int staticIndex = 0; staticIndex < NeedStaticTimes; staticIndex++)
                {
                    Thread.Sleep(50);

                    if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO7000X)
                        allChannelData = CommonMethod.Factory_WaveData_Channel(currInstrumentSession);
                    else if (ServerDomainConstants.ProductType == ProductType.JiHe_MSO8000X)
                        allChannelData = CommonMethod.Factory_WaveData_Channel(currInstrumentSession, ServerDomainConstants.PerAdcCoreDataCount);

                    Int32 rereadtimes = 0;

                    while ((allChannelData == null || allChannelData.Count == 0)&& rereadtimes<10)
                    {
                        //InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.AnalogParams);
                        Thread.Sleep(1000);
                        allChannelData = CommonMethod.Factory_WaveData_Channel(currInstrumentSession, ServerDomainConstants.PerAdcCoreDataCount);
                        rereadtimes++;
                    }

                    if (allChannelData == null || allChannelData.Count == 0)
                    {
                        message = $"(ExecuteId={_UniqueId};)数据读取错误！";
                        LogInfo(message);
                        CommonMethod.SetTemperatureCompensate(currInstrumentSession, true);
                        return BatchTaskPartResult.ErrorFatal;
                    }
                    foreach (PerChannel channel in caliChannels)
                    {
                        if (staticIndex == 0)
                            channel.MeasureValueUvs.Clear();
                        channel.MeasureValueUvs.Add(allChannelData[channel.channelID].Average(d => d));
                    }
                }

                ///调整参数校准
                foreach (PerChannel channel in caliChannels)
                {
                    if (channel.Status != AutoCaliOffsetStatus.Working)
                        continue;
                    var waveStatus = GetWaveStatus(allChannelData[channel.channelID], Math.Abs(channel.Cali_FPGAFinegainWord / 1000D));

                    //越界处理
                    if (waveStatus != WaveStatus.Normal)
                    {
                        if (AdjustOverStep(waveStatus, channel) == BatchTaskPartResult.ErrorFatal)
                        {
                            channel.Status = AutoCaliOffsetStatus.Failure;
                        }
                    }
                    else //校准位置
                    {
                        double measureValue = CommonMethod.MiddleDataFilter(channel.MeasureValueUvs, channel.MeasureValueUvs.Count() - 2).Average();
                        var tempRet = AdjustInStep(measureValue, channel);
                        if (tempRet == BatchTaskPartResult.Succeed)
                            channel.Status = AutoCaliOffsetStatus.Ok;
                        else if (tempRet == BatchTaskPartResult.ErrorFatal)
                            channel.Status = AutoCaliOffsetStatus.Failure;
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            sb.Append($"(ExecuteId={_UniqueId};)");
            caliChannels.ForEach(ch =>
            {
                sb.Append($"Ch{ch.channelID + 1}[{ch.Status}],");
            });
            sb.Append($";总共用时{stopwatch.ElapsedMilliseconds}ms.");
            CommonMethod.SetTemperatureCompensate(currInstrumentSession, true);

            message += sb.ToString();
            LogInfo($"校准结果：" + message);
            Boolean isAllOk = caliChannels.All(ch => ch.Status == AutoCaliOffsetStatus.Ok);
            return isAllOk ? BatchTaskPartResult.Succeed : BatchTaskPartResult.ErrorGeneral;
            #endregion
        }

        #region WCJ_校准处理
        private (int min, int max) _AdcRange { get => (0, (int)Math.Pow(2, ServerDomainConstants.AdcBits) - 1); }

        /// <summary>
        /// 获取当前波形的状态
        /// </summary>
        /// <param name="chnlData"></param>
        /// <param name="ratio">数字放大系数，为正数</param>
        /// <returns></returns>
        private WaveStatus GetWaveStatus(ushort[] chnlData, double ratio)
        {
            int upValue = _AdcRange.max;
            int downValue = _AdcRange.min;

            if (ratio < 1D)
            {
                upValue -= (int)((_AdcRange.max - _AdcRange.min) * (1 - ratio) / 2 + 1);
                downValue += (int)((_AdcRange.max - _AdcRange.min) * (1 - ratio) / 2 + 1);
            }

            //超过上边界判断
            if (chnlData.Count(d => d >= upValue) >= chnlData.Length / 2)
                return WaveStatus.OverTop;

            //超过下边界判断
            if (chnlData.Count(d => d <= downValue) >= chnlData.Length / 2)
                return WaveStatus.OverBottom;
            return WaveStatus.Normal;
        }

        private uint GetCurrOffsetCtrlWord(PerChannel channel)
        {
            return DesDiv switch
            {
                0 => channel.Cali_ChannelOffset,
                3 => channel.Cali_Channel3Div,
                -3 => channel.Cali_ChannelN3Div,
                _ => throw new ArgumentException("DesDiv"),
            };
        }

        private void SetCurrOffsetCtrlWord(PerChannel channel, uint ctrlWord)
        {
            switch (DesDiv)
            {
                case 0:
                    channel.Cali_ChannelOffset = ctrlWord;
                    break;
                case 3:
                    channel.Cali_Channel3Div = ctrlWord;
                    break;
                case -3:
                    channel.Cali_ChannelN3Div = ctrlWord;
                    break;
                default:
                    throw new ArgumentException("DesDiv");
            };
        }

        private BatchTaskPartResult AdjustOverStep(WaveStatus waveStatus, PerChannel channel)
        {
            uint offsetValue = GetCurrOffsetCtrlWord(channel);
            double ratio = Math.Abs(channel.Cali_FPGAFinegainWord / 1000D);

            //把波形调到范围内
            if (waveStatus == WaveStatus.OverTop)
            {
                channel.UpdateLastUpperOffsetCtrlWord(offsetValue, DesDiv == -3);
            }
            else if (waveStatus == WaveStatus.OverBottom)
            {
                channel.UpdateLastLowerOffsetCtrlWord(offsetValue, DesDiv == -3);
            }

            var logInfo = $"({_UniqueId}) AdjustOverStep: ChnlId_{channel.channelID + 1},WaveStatus_{waveStatus}, OffsetCtrlWord_{offsetValue}";
            LogInfo(logInfo);
            uint tempOffset = (channel.LastUpperOffsetCtrlWord + channel.LastLowerOffsetCtrlWord) / 2;
            if (tempOffset == channel.LastUpperOffsetCtrlWord || tempOffset == channel.LastLowerOffsetCtrlWord)
                return BatchTaskPartResult.ErrorFatal;

            SetCurrOffsetCtrlWord(channel, tempOffset);
            return BatchTaskPartResult.Succeed;
        }

        private BatchTaskPartResult AdjustInStep(double measureValue, PerChannel channel)
        {
            int adcMaxValue = (int)Math.Pow(2, ServerDomainConstants.AdcBits) - 1;
            double destAvg = adcMaxValue / 2 + DesDiv * (int)ServerDomainConstants.SAMPS_PER_YDIV + 1;
            (uint reg, double avg) currPoint = (GetCurrOffsetCtrlWord(channel), measureValue);

            if (Math.Abs(destAvg - currPoint.avg) < baseErrorLimitByPercentDiv)
            {
                var logInfo = $"({_UniqueId}) AdjustInStep, OK: ChnlId_{channel.channelID + 1},OffsetCtrlWord_{currPoint.reg};";
                Utilities.Logger.LogCaliInfo(_UniqueId, "Log", "BatchTaskPart_CaliChannelBaseLine", logInfo);
                return BatchTaskPartResult.Succeed;
            }

            if (channel.FirstStepInPoint == null)
            {
                var logInfo = $"({_UniqueId}) AdjustInStep,FirstStepInPoint: ChnlId_{channel.channelID + 1},Value_{currPoint.avg}, OffsetCtrlWord_{currPoint.reg}";
                LogInfo(logInfo);

                channel.FirstStepInPoint = (currPoint.reg, currPoint.avg);
                //去寻找第二个点
                if (DesDiv == -3)
                {
                    if (currPoint.avg < destAvg)
                        channel.LastUpperOffsetCtrlWord = channel.FirstStepInPoint.Value.reg;
                    else
                        channel.LastLowerOffsetCtrlWord = channel.FirstStepInPoint.Value.reg;
                }
                else
                {
                    if (currPoint.avg < destAvg)
                        channel.LastLowerOffsetCtrlWord = channel.FirstStepInPoint.Value.reg;
                    else
                        channel.LastUpperOffsetCtrlWord = channel.FirstStepInPoint.Value.reg;
                }

                SetCurrOffsetCtrlWord(channel, (channel.LastUpperOffsetCtrlWord + channel.LastLowerOffsetCtrlWord) / 2);
            }
            else
            {
                var logInfo = $"({_UniqueId}) AdjustInStep,SecondStepInPoint: ChnlId_{channel.channelID + 1},Value_{currPoint.avg}, OffsetCtrlWord_{currPoint.reg}";
                LogInfo(logInfo);

                //从多点中筛选两点
                if (channel.SecondStepInPoint == null)
                {
                    channel.SecondStepInPoint = (currPoint.reg, currPoint.avg);
                }
                else
                {
                    Boolean oneSideFlag = destAvg > channel.FirstStepInPoint.Value.avg &&
                                          destAvg > channel.SecondStepInPoint.Value.avg &&
                                          destAvg > currPoint.avg;
                    if (!oneSideFlag)
                    {
                        oneSideFlag = destAvg < channel.FirstStepInPoint.Value.avg &&
                                      destAvg < channel.SecondStepInPoint.Value.avg &&
                                      destAvg < currPoint.avg;
                    }
                    double lostAvg = currPoint.avg;
                    if (oneSideFlag)
                    {
                        //同侧: 取当前点，第一点，第二点中avg值距离destAvg最近的两个点做计算。
                        if (Math.Abs(channel.FirstStepInPoint.Value.avg - destAvg) > Math.Abs(lostAvg - destAvg))
                            lostAvg = channel.FirstStepInPoint.Value.avg;
                        if (Math.Abs(channel.SecondStepInPoint.Value.avg - destAvg) > Math.Abs(lostAvg - destAvg))
                            lostAvg = channel.SecondStepInPoint.Value.avg;
                    }
                    else
                    {
                        //异侧: 有且只有两个点在一侧，取其中一个距离destAvg最近的点及另一侧的点做计算
                        if ((currPoint.avg - destAvg) * (channel.FirstStepInPoint.Value.avg - destAvg) > 0)
                        {
                            if (Math.Abs(channel.FirstStepInPoint.Value.avg - destAvg) > Math.Abs(currPoint.avg - destAvg))
                                lostAvg = channel.FirstStepInPoint.Value.avg;
                        }
                        else if ((currPoint.avg - destAvg) * (channel.SecondStepInPoint.Value.avg - destAvg) > 0)
                        {
                            if (Math.Abs(channel.SecondStepInPoint.Value.avg - destAvg) > Math.Abs(currPoint.avg - destAvg))
                                lostAvg = channel.SecondStepInPoint.Value.avg;
                        }
                        else
                        {
                            if (Math.Abs(channel.FirstStepInPoint.Value.avg - destAvg) > Math.Abs(channel.SecondStepInPoint.Value.avg - destAvg))
                                lostAvg = channel.FirstStepInPoint.Value.avg;
                            else
                                lostAvg = channel.SecondStepInPoint.Value.avg;
                        }

                    }

                    if (lostAvg == channel.FirstStepInPoint.Value.avg)
                        channel.FirstStepInPoint = (currPoint.reg, currPoint.avg);
                    else if (lostAvg == channel.SecondStepInPoint.Value.avg)
                        channel.SecondStepInPoint = (currPoint.reg, currPoint.avg);
                    else
                    {
                        logInfo = $"({_UniqueId}) AdjustInStep: Current point is invalid!";
                        LogInfo(logInfo);
                        return BatchTaskPartResult.ErrorFatal;
                    }
                }

                //用两点计算斜率
                if (channel.FirstStepInPoint.Value.reg == channel.SecondStepInPoint.Value.reg)
                {
                    logInfo = $"({_UniqueId}) AdjustInStep: FirstStepInPoint.reg = SecondStepInPoint.reg!";
                    LogInfo(logInfo);

                    return BatchTaskPartResult.ErrorFatal;
                }
                double ratio = (channel.FirstStepInPoint.Value.avg - channel.SecondStepInPoint.Value.avg)
                    / (int)(channel.FirstStepInPoint.Value.reg - channel.SecondStepInPoint.Value.reg);
                uint tempOffset = (uint)Math.Round(channel.FirstStepInPoint.Value.reg + (destAvg - channel.FirstStepInPoint.Value.avg) / ratio);
                if (tempOffset > MaxOffsetCtrlWord || tempOffset < MinOffsetCtrlWord)
                {
                    logInfo = $"({_UniqueId}) AdjustInStep: tempOffset overFlow:{tempOffset}";
                    LogInfo(logInfo);
                    return BatchTaskPartResult.ErrorFatal;
                }
                SetCurrOffsetCtrlWord(channel, tempOffset);

                //更新LastLowerOffsetCtrlWord与LastUpperOffsetCtrlWord,保证即使计算的tempOffset超出边界值也能算的回来；
                if ((channel.FirstStepInPoint.Value.avg - destAvg) * (channel.SecondStepInPoint.Value.avg - destAvg) > 0)
                {
                    //同侧
                    if (channel.FirstStepInPoint.Value.avg > destAvg)
                    {
                        uint offsetValue = (channel.FirstStepInPoint.Value.avg < channel.SecondStepInPoint.Value.avg)
                            ? channel.FirstStepInPoint.Value.reg : channel.SecondStepInPoint.Value.reg;
                        channel.UpdateLastUpperOffsetCtrlWord(offsetValue, DesDiv == -3);
                    }
                    else
                    {
                        uint offsetValue = (channel.FirstStepInPoint.Value.avg > channel.SecondStepInPoint.Value.avg)
                            ? channel.FirstStepInPoint.Value.reg : channel.SecondStepInPoint.Value.reg;
                        channel.UpdateLastLowerOffsetCtrlWord(offsetValue, DesDiv == -3);
                    }
                }
                else
                {
                    //异侧
                    uint offsetValueU = (channel.FirstStepInPoint.Value.avg > channel.SecondStepInPoint.Value.avg)
                        ? channel.FirstStepInPoint.Value.reg : channel.SecondStepInPoint.Value.reg;
                    uint offsetValueL = (channel.FirstStepInPoint.Value.avg < channel.SecondStepInPoint.Value.avg)
                        ? channel.FirstStepInPoint.Value.reg : channel.SecondStepInPoint.Value.reg;
                    channel.UpdateLastUpperOffsetCtrlWord(offsetValueU, DesDiv == -3);
                    channel.UpdateLastLowerOffsetCtrlWord(offsetValueL, DesDiv == -3);
                }
                logInfo = $"({_UniqueId}) AdjustInStep: LastUpperOffsetCtrlWord_{channel.LastUpperOffsetCtrlWord}, LastLowerOffsetCtrlWord_{channel.LastLowerOffsetCtrlWord}";
                LogInfo(logInfo);
            }
            return BatchTaskPartResult.ErrorGeneral;
        }

        #endregion WCJ_校准处理
    }
}

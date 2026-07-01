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
    public class BatchTaskPart_Cali8GHzChannelOffset : BatchTaskPartBase
    {
        enum AutoCaliState
        {
            Working = 1,
            Failure = 2,
            Ok = 3
        }
        enum CaliStages
        {
            FindDirection,
            FindUp2DnInvert,
            FindDn2UpInvert,
            Finished,
        }
        class PerChannel
        {
            public AnaChnlIpnutSource IpnutSource = AnaChnlIpnutSource.BNC;
            public int channelID;
            public int yLevelID;
            public int Impedance;
            public int StaticTimes = 0;
            public Delta_Average result = new();
            public AutoCaliState channelCaliStateRecord = AutoCaliState.Working;
            public CaliRecord channelLastRecord = new();
        }

        class CaliRecord
        {
            public CaliStages Stage;
            public uint LastCtrlWord;
            public uint LargeAtUpInvertedCtrlWord;
            public uint LargeAtDnInvertedCtrlWord;
            public int AverageDecAmpHalf;
            public uint LastStep;
        }
        private bool oldValid = false;
        //private List<CaliRecord> channelLastRecord = new List<CaliRecord>();
        List<PerChannel> caliChannels = new List<PerChannel>();
        private int yScaleID = 0;
        private uint MaxPosOffset = 0;
        public override string FuncionDescription
        {
            get => $"校准8GHz通道的偏置";
        }
        public override string ParametersDescription
        {
            get => $"第1个参数：其中1|2 表示通道，编号从1开始；" +
                   "第2个参数，以mV为单位的档位值，浮点数" +
                   "第3个参数，后级偏最大调整范围";
        }
        public override string Example
        {
            get => "BatchTaskPart_Cali8GHzChannelOffset 1|2,100";
        }
        private bool AnalyParameter(string parameter)
        {
            parameterStr = parameter;
            caliChannels.Clear();
            string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramList.Length < 2)
            {
                oldValid = false;
                return false;
            }
            //channel list
            //[0]: channel list
            string[] channelIDList = paramList[0].Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (channelIDList.Length == 0)
            {
                oldValid = false;
                return false;
            }

            //[1]:幅度档，用mV为单位
            int inputLevelBymV = BaseHelper.TryConvertToInt(paramList[1]);
            if (inputLevelBymV == 0)
            {
                oldValid = false;
                return false;
            }
            inputLevelBymV *= 1000;//mv=>uV
            yScaleID = Array.IndexOf<Int32>(AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV.ToArray(), inputLevelBymV);
            if (yScaleID < 0)
            {
                oldValid = false;
                return false;
            }

            MaxPosOffset = uint.Parse(paramList[2]);
            if (MaxPosOffset < 0)
            {
                oldValid = false;
                return false;
            }
            if (inputLevelBymV == 0)
            {
                oldValid = false;
                return false;
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
                perChannel.channelID = id - 1;
                perChannel.yLevelID = yScaleID;
                perChannel.Impedance = 1;
                caliChannels.Add(perChannel);
            }
            if (caliChannels.Count == 0)
            {
                oldValid = false;
                return false;
            }
            //for (int i = 0; i < 8; i++)
            //{
            //    channelCaliStateRecord.Add(AutoCaliState.Ok);

            //}
            //foreach (string s in channelIDList)
            //{
            //    channelCaliStateRecord[(int.Parse(s) - 1)] = AutoCaliState.Working;
            //    channelLastRecord.Add(new());

            //}

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
        class Delta_Average
        {
            public int Delta;
            public int Average;
        }
        private void Do5TimesStatisticsAverage()
        {
            foreach (PerChannel channel in caliChannels)
                channel.result.Average = 0;
            for (int statisticsTimes = 0; statisticsTimes < 5; statisticsTimes++)
            {
                List<ushort[]>? allChannelData = InstrumentInteract.Factory_WaveData_Channel(currInstrumentSession!, 6_000);
                if (allChannelData == null)
                    continue;

                foreach (PerChannel channel in caliChannels)
                {
                    if (channel.channelCaliStateRecord == AutoCaliState.Working)
                    {
                        long max_value = long.MinValue;
                        long min_value = long.MaxValue;
                        long sum_10000 = 0;

                        max_value = allChannelData[channel.channelID].Max<ushort>();
                        min_value = allChannelData[channel.channelID].Min<ushort>();
                        for (int i = 0; i < 10000; i++)
                        {
                            sum_10000 += allChannelData[channel.channelID][i];
                        }
                        int average = (int)(sum_10000 / 10000);
                        //平均值 - 编写幅度中心值
                        channel.result.Delta += (int)(average - ((max_value - min_value) / 2 + min_value));
                        channel.result.Average += average;
                    }
                }
            }
            foreach (PerChannel channel in caliChannels)
            {
                channel.result.Delta /= 5;
                channel.result.Average /= 5;
            }
        }
        public override BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null)
        {
            if (!oldValid)
            {
                message = "参数错误！";
                return BatchTaskPartResult.ErrorParameter;
            }
            errorCount = 0;
            BatchTaskPartResult batchTaskPartResult = BatchTaskPartResult.ErrorGeneral;
            Stopwatch stopwatch = new Stopwatch();
            long maxWaitMilliseconds = 60 * 1000;
            if (overtimeOfSecond > 0)
                maxWaitMilliseconds = (long)(1000 * overtimeOfSecond);
            StringBuilder stringBuilder = new StringBuilder();
            stopwatch.Start();
            int WorkingFlag = 0;
            foreach (PerChannel perChannel in caliChannels)
                WorkingFlag |= 1 << perChannel.channelID;

            #region 首次统计，确定方向
            Do5TimesStatisticsAverage();

            foreach (PerChannel channel in caliChannels)
            {
                if (channel.channelCaliStateRecord == AutoCaliState.Working)
                {
                    int delta = channel.result.Delta;
                    channel.channelLastRecord.AverageDecAmpHalf = delta;
                    if (delta < 0)
                    {
                        channel.channelLastRecord.Stage = CaliStages.FindUp2DnInvert;
                        channel.channelLastRecord.LastCtrlWord = ChannelParamsModel2.Default[channel.channelID, 1, yScaleID].OffsetPosterior;
                    }
                    else if (delta > 0)
                    {
                        channel.channelLastRecord.Stage = CaliStages.FindDn2UpInvert;
                        channel.channelLastRecord.LastCtrlWord = ChannelParamsModel2.Default[channel.channelID, 1, yScaleID].OffsetPosterior;
                    }
                    else //==0
                    {
                        channel.channelLastRecord.Stage = CaliStages.FindDirection;
                    }
                    channel.channelLastRecord.LastCtrlWord = ChannelParamsModel2.Default[channel.channelID, 1, yScaleID].OffsetPosterior;
                    ChannelPerScaleItem item = ChannelParamsModel2.Default[channel.channelID, 1, yScaleID];
                    item.OffsetPosterior_3Div = 2048;//在值作为 Delta Zero ,初始化为0.从而让FPGA不参与移位处理。
                    item.OffsetPreceding_3Div = item.OffsetPosterior;//保存原有的基线校准值，该值可能是手动校准出来的，宝贵！
                    ChannelParamsModel2.Default[channel.channelID, 1/*低阻，只有低阻*/, yScaleID] = item;
                }
            }
            #endregion

            uint step = 0;
            //while (WorkingFlag != 0 && stopwatch.ElapsedMilliseconds < maxWaitMilliseconds)
            while (WorkingFlag != 0)
            {
                InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.PhyChannelModel2);
                Thread.Sleep(500);
                Do5TimesStatisticsAverage();
                foreach (PerChannel channel in caliChannels)
                {
                    if (channel.channelCaliStateRecord != AutoCaliState.Working)
                        continue;
                    int delta = channel.result.Delta;
                    ChannelPerScaleItem item = ChannelParamsModel2.Default[channel.channelID, 1, yScaleID];
                    if (channel.channelCaliStateRecord == AutoCaliState.Working)
                    {
                        if (channel.channelLastRecord.Stage == CaliStages.FindDirection)
                        {
                            if (delta < 0)
                            {
                                channel.channelLastRecord.Stage = CaliStages.FindUp2DnInvert;
                                channel.channelLastRecord.LastCtrlWord = ChannelParamsModel2.Default[channel.channelID, 1, yScaleID].OffsetPosterior;
                                item.OffsetPosterior = channel.channelLastRecord.LastCtrlWord - 10;
                                channel.channelLastRecord.LastStep = 1000;
                            }
                            else if (delta > 0)
                            {
                                channel.channelLastRecord.Stage = CaliStages.FindDn2UpInvert;
                                channel.channelLastRecord.LastCtrlWord = ChannelParamsModel2.Default[channel.channelID, 1, yScaleID].OffsetPosterior;
                                item.OffsetPosterior = channel.channelLastRecord.LastCtrlWord - 10;
                                channel.channelLastRecord.LastStep = 1000;
                            }
                            else
                            {
                                item.OffsetPosterior = channel.channelLastRecord.LastCtrlWord - 1000;
                                channel.channelLastRecord.LastStep = 1000;
                            }
                        }
                        else if (channel.channelLastRecord.Stage == CaliStages.FindDn2UpInvert)
                        {
                            step = channel.channelLastRecord.LastStep;
                            if (step == 0)
                                step = 300;
                            //else
                            //   step /= 2;
                            if (step == 0)
                                step = 1;
                            if (delta > 0)
                            {
                                item.OffsetPosterior = (uint)(channel.channelLastRecord.LastCtrlWord - step);
                                channel.channelLastRecord.LastStep = step;
                            }
                            else if (delta <= 0)
                            {
                                if (delta == 0)
                                {
                                    int average = channel.result.Average;
                                    item.OffsetPosterior_3Div = (UInt32)average;
                                    WorkingFlag &= (~(1 << channel.channelID));
                                }
                                else if (channel.channelLastRecord.LastStep == 1)
                                {
                                    channel.channelLastRecord.LargeAtUpInvertedCtrlWord = channel.channelLastRecord.LastCtrlWord;
                                    if (channel.channelLastRecord.LargeAtUpInvertedCtrlWord != 0)
                                    {
                                        channel.channelLastRecord.Stage = CaliStages.Finished;
                                        item.OffsetPosterior = (channel.channelLastRecord.LargeAtUpInvertedCtrlWord + channel.channelLastRecord.LargeAtDnInvertedCtrlWord) / 2;
                                    }
                                    else
                                    {
                                        channel.channelLastRecord.Stage = CaliStages.FindUp2DnInvert;
                                        channel.channelLastRecord.LastCtrlWord = channel.channelLastRecord.LastCtrlWord + 1000;
                                        item.OffsetPosterior = channel.channelLastRecord.LastCtrlWord;
                                        channel.channelLastRecord.LastStep = 1000;

                                    }
                                }
                                else
                                {
                                    channel.channelLastRecord.LastCtrlWord += channel.channelLastRecord.LastStep;
                                    if (channel.channelLastRecord.LastStep / 2 == 0)
                                        channel.channelLastRecord.LastStep = 2;
                                    channel.channelLastRecord.LastCtrlWord -= channel.channelLastRecord.LastStep / 2;
                                    item.OffsetPosterior = channel.channelLastRecord.LastCtrlWord;
                                    channel.channelLastRecord.LastStep = channel.channelLastRecord.LastStep / 2;
                                }
                            }
                        }
                        else if (channel.channelLastRecord.Stage == CaliStages.FindUp2DnInvert)
                        {
                            step = channel.channelLastRecord.LastStep;
                            if (step == 0)
                                step = 300;
                            //else
                            //   step /= 2;
                            //if (step == 0)
                            //   step = 1;

                            if (delta < 0)
                            {
                                item.OffsetPosterior = (uint)(channel.channelLastRecord.LastCtrlWord + step);
                                channel.channelLastRecord.LastStep = step;
                            }
                            else if (delta >= 0)
                            {
                                if (delta == 0)
                                {
                                    int average = channel.result.Average;
                                    item.OffsetPosterior_3Div = (UInt32)average;
                                    WorkingFlag &= (~(1 << channel.channelID));
                                }
                                else if (channel.channelLastRecord.LastStep == 1)
                                {
                                    channel.channelLastRecord.LargeAtDnInvertedCtrlWord = channel.channelLastRecord.LastCtrlWord;
                                    if (channel.channelLastRecord.LargeAtDnInvertedCtrlWord != 0)
                                    {
                                        channel.channelLastRecord.LargeAtUpInvertedCtrlWord = channel.channelLastRecord.LastCtrlWord;
                                        channel.channelLastRecord.Stage = CaliStages.Finished;
                                        item.OffsetPosterior = (channel.channelLastRecord.LargeAtUpInvertedCtrlWord + channel.channelLastRecord.LargeAtDnInvertedCtrlWord) / 2;
                                    }
                                    else
                                    {
                                        channel.channelLastRecord.Stage = CaliStages.FindDn2UpInvert;
                                        channel.channelLastRecord.LastCtrlWord = channel.channelLastRecord.LastCtrlWord - 1000;
                                        item.OffsetPosterior = channel.channelLastRecord.LastCtrlWord;
                                        channel.channelLastRecord.LastStep = 1000;

                                    }
                                }
                                else
                                {
                                    channel.channelLastRecord.LastCtrlWord -= (uint)channel.channelLastRecord.LastStep;
                                    if (channel.channelLastRecord.LastStep / 2 == 0)
                                        channel.channelLastRecord.LastStep = 2;
                                    channel.channelLastRecord.LastCtrlWord += (uint)channel.channelLastRecord.LastStep / 2;
                                    item.OffsetPosterior = channel.channelLastRecord.LastCtrlWord;
                                    channel.channelLastRecord.LastStep = channel.channelLastRecord.LastStep / 2;
                                }
                            }
                        }
                        else//Finished
                        {
                            int average = channel.result.Average;
                            item.OffsetPosterior_3Div = (UInt32)average;
                            WorkingFlag &= (~(1 << channel.channelID));
                        }
                        if (item.OffsetPosterior > MaxPosOffset + item.OffsetPreceding_3Div)
                        {
                            stringBuilder.Append($"[×CH{channel.channelID + 1}]，后级偏已到最大值。");
                            item.OffsetPosterior = MaxPosOffset + item.OffsetPreceding_3Div;
                            WorkingFlag &= (~(1 << channel.channelID));
                        }
                        //else if(item.OffsetPosterior < MinPosOffset[channelID])
                        else if (item.OffsetPosterior < (item.OffsetPreceding_3Div - MaxPosOffset))
                        {
                            stringBuilder.Append($"[×CH{channel.channelID + 1}]，后级偏已到最小值。");
                            item.OffsetPosterior = item.OffsetPreceding_3Div - MaxPosOffset;
                            WorkingFlag &= (~(1 << channel.channelID));
                        }
                        ChannelParamsModel2.Default[channel.channelID, 1, yScaleID] = item;
                    }
                    channel.channelLastRecord.LastCtrlWord = item.OffsetPosterior;
                }
            }
            message = stringBuilder.ToString();
            if (WorkingFlag != 0)
                return BatchTaskPartResult.ErrorOvertime;
            else if (errorCount != 0)
                return BatchTaskPartResult.ErrorGeneral;
            else
                return batchTaskPartResult;
        }
    }
}

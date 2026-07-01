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
using ScopeX.Hardware.Calibration.Tool.Utilities;
using System.Threading.Channels;
using Microsoft.Office.Interop.Excel;
using System.Xml;

namespace ScopeX.Hardware.Calibration.Tool.BatchTask
{
    /// <summary>
    /// 通过DC直流校准LA
    /// </summary>
    public class BatchTaskPart_CaliLACompareLevelDC : BatchTaskPartBase
    {
        enum CaliStatus
        {
            Working,
            Failure,
            Ok,
        }

        enum CaliStage
        {
            HighLevel,
            LowLevel,
        }

        class CaliChnlInfo
        {
            public int ChannelID;
            public CaliStage CaliStage = CaliStage.HighLevel;
            public CaliStatus Status = CaliStatus.Working;

            public (int Up, int Down) LastHigh = (0, 0);   //寻找高电平边界的值范围
            public (int Up, int Down) LastLow = (0, 0);    //寻找低电平边界的值范围
            public int AdjustCount; //调整计数
            /// <summary>
            /// 当前DAC寄存器实际发送的值，前提比较电平设为0
            /// </summary>
            public int RegValue
            {
                get
                {
                    int comparetorIndex = ChannelID / 4;
                    return MiscData.Default[(int)MiscDefine.LA_CaliDataBegin + comparetorIndex * 2];
                }
                set
                {
                    int comparetorIndex = ChannelID / 4;
                    MiscData.Default[(int)MiscDefine.LA_CaliDataBegin + comparetorIndex * 2] = value;//base
                    AdjustCount++;
                }
            }
            public Int32 Ratio
            {
                get
                {
                    Int32 comparetorindex = ChannelID / 4;
                    return MiscData.Default[(Int32)MiscDefine.LA_CaliDataBegin + comparetorindex * 2 + 1];
                }
                set
                {
                    Int32 comparetorindex = ChannelID / 4;
                    MiscData.Default[(Int32)MiscDefine.LA_CaliDataBegin + comparetorindex * 2 + 1] = value;//base
                }
            }

            public void FreshLastHigh(byte[] data)
            {
                //up值，全是0
                if (!data.Any(d => d > 0))
                {
                    LastHigh.Up = RegValue;
                }
                else //down值，不全为0
                {
                    LastHigh.Down = RegValue;
                }
            }

            public void FreshLastLow(byte[] data)
            {
                //up值，不全是1
                if (data.Any(d => d == 0))
                {
                    LastLow.Up = RegValue;
                }
                else //down值，全为1
                {
                    LastLow.Down = RegValue;
                }
            }
        }

        private const Int32 BaseErrorMax = 4000;//0V校准值最大偏移范围
        private const Int32 RatioErrorMax = 2000;//1V偏移值最大偏移范围
        private const Double ProbeAtt = 10;      //LA探头10倍衰减
        private readonly Int32 DACBits;
        //输入参数
        private List<CaliChnlInfo> _CaliChannels = new List<CaliChnlInfo>();
        private Int64 _SourceVoltMv = 0;
        private Int64 _SuggestBase = 0;
        private Int64 _SuggestRatio = 0;
        private Int64 _UniqueId = 0;

        public BatchTaskPart_CaliLACompareLevelDC()
        {
            DACBits = ServerDomainConstants.ProductType switch
            {
                ProductType.JiHe_MSO8000HD => 16,
                ProductType.JiHe_MSO8000X => 16,
                _ => 12,
            };
        }

        public override string FuncionDescription
        {
            get => $"LA比较电平校准";
        }
        public override string ParametersDescription
        {
            get
            {
                int argIndex = 1;
                return $"第{argIndex++}个参数：要校准的通道，从0开始(表示D0通道),可以多通道校准(1|2);{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数：校准信号的幅度值，用mv为单位;{System.Environment.NewLine}";
            }
        }

        public override string Example
        {
            get => $"BatchTaskPart_CaliLACompareLevel 0|4 ,4000";
        }

        private bool AnalyParameter(string parameter)
        {
            string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramList.Length < 2)
            {
                return false;
            }

            //[0]: channel list
            string[] channelIDList = paramList[0].Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (channelIDList.Length == 0)
            {
                return false;
            }

            //[1]:幅度值，用mV为单位
            _SourceVoltMv = BaseHelper.TryConvertToLong(paramList[1]);
            //if (_SourceVoltMv == 0)
            //{
            //    return false;
            //}
            //[2]:建议的0V校准值
            _SuggestBase = BaseHelper.TryConvertToLong(paramList[2]);
            if (_SuggestBase < 0)
            {
                return false;
            }
            //[3]:建议的1V偏移值
            _SuggestRatio = BaseHelper.TryConvertToLong(paramList[3]);
            if (_SuggestRatio < 0)
            {
                return false;
            }
            //校验参数有效性
            int defaulthighup = (Int32)(_SuggestBase + BaseErrorMax + _SourceVoltMv / ProbeAtt / 1000D / 2 * (_SuggestRatio + RatioErrorMax));
            int defaulthighdown = (Int32)(_SuggestBase - BaseErrorMax + _SourceVoltMv / ProbeAtt / 1000D / 2 * (_SuggestRatio - RatioErrorMax));
            int defaultlowup = (Int32)(_SuggestBase + BaseErrorMax - _SourceVoltMv / ProbeAtt / 1000D / 2 * (_SuggestRatio - RatioErrorMax));
            int defaultlowdown = (Int32)(_SuggestBase - BaseErrorMax - _SourceVoltMv / ProbeAtt / 1000D / 2 * (_SuggestRatio + RatioErrorMax));
            if (defaulthighup < 0 || defaulthighup >= Math.Pow(2, DACBits) ||
               defaulthighdown < 0 || defaulthighdown >= Math.Pow(2, DACBits) ||
               defaultlowup < 0 || defaultlowup >= Math.Pow(2, DACBits) ||
               defaultlowdown < 0 || defaultlowdown >= Math.Pow(2, DACBits))
            {
                return false;
            }
            else
            {
                if (defaulthighup - defaulthighdown < 10 ||
                   defaultlowup - defaultlowdown < 10)
                {
                    return false;
                }
            }


            //初始化要校准的通道对象集合
            foreach (string channelId in channelIDList)
            {
                int id = BaseHelper.TryConvertToInt(channelId);
                if (id < 0 || id >= ServerDomainConstants.LAChannelCount)
                {
                    return false;
                }
                CaliChnlInfo perchannel = new CaliChnlInfo();
                perchannel.ChannelID = id;
                perchannel.LastHigh.Up = (int)(Math.Pow(2, DACBits) - 1000);
                perchannel.LastHigh.Down = 1000;
                perchannel.LastLow.Up = (int)(Math.Pow(2, DACBits) - 1000);
                perchannel.LastLow.Down = 1000;
                _CaliChannels.Add(perchannel);
            }

            return _CaliChannels.Count != 0;
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
            Utilities.Logger.LogCaliInfo(_UniqueId, "Log", "BatchTaskPart_CaliLACompareLevel", info);
        }

        public override BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null)
        {
            message = String.Empty;
            StringBuilder messageSB = new StringBuilder();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _UniqueId = DateTime.Now.Ticks;

            List<int> LaData = new List<int>();
            if (_SourceVoltMv > 0)
            {
                for (int i = 0; i < _CaliChannels.Count; i++)
                {
                    //*配置校准值
                    int comparetorIndex = _CaliChannels[i].ChannelID / 4;
                    LaData.Add(MiscData.Default[(int)MiscDefine.LA_CaliDataBegin + comparetorIndex * 2]);
                }
            }

            #region 

            //记录执行信息
            StringBuilder chnlsText = new StringBuilder();
            _CaliChannels.ForEach(chnl => chnlsText.Append("Ch" + chnl.ChannelID + ","));
            LogInfo($"(ExecuteId={_UniqueId};)BatchTaskPart_CaliLACompareLevel:chnls_{chnlsText}; SourceVoltMv_{_SourceVoltMv};");

            //DAC寄存器发默认值
            _CaliChannels.ForEach(chnl => chnl.RegValue = (chnl.LastHigh.Up));
            _CaliChannels.ForEach(chnl => chnl.Ratio = (int)_SuggestRatio);
            //Thread.Sleep(1000); //等待信号源输出稳定

            while (_CaliChannels.Any(ch => ch.Status == CaliStatus.Working))
            {
                //*取消处理
                try
                {
                    cancelTokenSrc?.Token.ThrowIfCancellationRequested();
                }
                catch
                {
                    message = $"(ExecuteId={_UniqueId};)任务被取消！";
                    LogInfo(message);
                    return BatchTaskPartResult.Cancel;
                }
                if (stopwatch.ElapsedMilliseconds > overtimeOfSecond * 1000)
                {
                    message = $"(ExecuteId={_UniqueId};)超时退出";
                    LogInfo(message);
                    return BatchTaskPartResult.ErrorOvertime;
                }

                //*设置数据发送
                InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.Misc);
                Thread.Sleep(1100);


                //获取波形数据
                List<byte[]>? allChannelData = InstrumentInteract.DoFactory_WaveData_LA(currInstrumentSession, 6000);
                if (allChannelData == null)
                {
                    message = $"(ExecuteId={_UniqueId};)数据读取错误！";
                    LogInfo(message);
                    return BatchTaskPartResult.ErrorFatal;
                }

                //*校准
                foreach (var chnl in _CaliChannels)
                {
                    if (chnl.Status != CaliStatus.Working)
                        continue;
                    // 斯密特门从1->0
                    //先校准高电平(二分法)
                    if (chnl.CaliStage == CaliStage.LowLevel)
                    {
                        //chnl.FreshToOne(allChannelData[chnl.ChannelID]);
                        if (allChannelData[chnl.ChannelID].Any(d => d == (_SourceVoltMv < 0 ? 1 : 1)))
                        {
                            chnl.LastLow.Down = chnl.RegValue;
                        }
                        else //down值，全为1
                        {
                            chnl.LastLow.Up = chnl.RegValue;
                        }
                        //当up与down之间的差<=1时，认为中间值对应电平
                        if (chnl.LastLow.Up - chnl.LastLow.Down <= 1)
                        {
                            chnl.Status = CaliStatus.Ok;
                        }
                        else
                        {
                            if (chnl.LastLow.Down != chnl.RegValue)
                            {
                                chnl.RegValue = ((chnl.LastLow.Up + chnl.LastLow.Down) / 2 + chnl.LastLow.Down) / 2;
                                if (chnl.RegValue == chnl.LastLow.Up || chnl.RegValue == chnl.LastLow.Down)
                                {
                                    chnl.RegValue = (chnl.LastLow.Up + chnl.LastLow.Down) / 2;
                                }
                            }
                            else
                            {
                                chnl.RegValue = (chnl.LastLow.Up + chnl.LastLow.Down) / 2;
                            }
                        }
                    }

                    //斯密特门从0->1
                    if (chnl.CaliStage == CaliStage.HighLevel)
                    {
                        if (allChannelData[chnl.ChannelID].Any(d => d == (_SourceVoltMv < 0 ? 0 : 0)))
                        {
                            chnl.LastHigh.Up = chnl.RegValue;
                        }
                        else //down值，Down
                        {
                            chnl.LastHigh.Down = chnl.RegValue;
                        }

                        if (chnl.LastHigh.Up - chnl.LastHigh.Down <= 1)
                        {
                            chnl.CaliStage = CaliStage.LowLevel;
                            chnl.RegValue = chnl.LastLow.Down;
                        }
                        else
                        {
                            if (chnl.LastHigh.Up != chnl.RegValue)
                            {
                                chnl.RegValue = ((chnl.LastHigh.Up + chnl.LastHigh.Down) / 2 + chnl.LastHigh.Up) / 2;
                                if (chnl.RegValue == chnl.LastHigh.Up || chnl.RegValue == chnl.LastHigh.Down)
                                {
                                    chnl.RegValue = (chnl.LastHigh.Up + chnl.LastHigh.Down) / 2;
                                }
                            }
                            else
                            {
                                chnl.RegValue = (chnl.LastHigh.Up + chnl.LastHigh.Down) / 2;
                            }
                        }
                    }
                    LogInfo($"ChannelID_{chnl.ChannelID},Status_{chnl.Status},CaliStage_{chnl.CaliStage},RegValue_{chnl.RegValue}," +
                        $"HighUp_{chnl.LastHigh.Up},HighDown_{chnl.LastHigh.Down},LowUp_{chnl.LastLow.Up},LowDown_{chnl.LastLow.Down}");
                }
            }

            #endregion

            for (int i = 0; i < _CaliChannels.Count; i++)
            {
                //*配置校准值
                int comparetorIndex = _CaliChannels[i].ChannelID / 4;
                double lastlowdata = (_CaliChannels[i].LastLow.Down + _CaliChannels[i].LastLow.Up) / 2D;
                double lasthighdata = (_CaliChannels[i].LastHigh.Down + _CaliChannels[i].LastHigh.Up) / 2D;
                double baseValue = (lasthighdata + lastlowdata) / 2D;
                MiscData.Default[(int)MiscDefine.LA_CaliDataBegin + comparetorIndex * 2] = (int)Math.Round(baseValue);
                LogInfo($"SourceVoltMv:{_SourceVoltMv} lastlowdata:{lastlowdata} lasthighdata:{lasthighdata} baseValue:{baseValue}");
                if (_SourceVoltMv > 0)
                {
                    double ratio = Math.Abs((baseValue - LaData[i]) / (_SourceVoltMv / 10D));  //LA探头10倍衰减
                    MiscData.Default[(int)MiscDefine.LA_CaliDataBegin + comparetorIndex * 2] = (int)(baseValue + LaData[i]) / 2;
                    LogInfo($"SourceVoltMv:{_SourceVoltMv} ratio:{ratio} lasthighdata:{lasthighdata} LA_CaliDataBegin:{(int)(baseValue + LaData[i]) / 2}");
                    MiscData.Default[(int)MiscDefine.LA_CaliDataBegin + comparetorIndex * 2 + 1] = (int)Math.Round(ratio);
                }

                //*校准信息输出
                string caliRet = (_CaliChannels[i].Status == CaliStatus.Ok && _CaliChannels[i].Status == CaliStatus.Ok) ? "[OK]" : "[Failure]";
                messageSB.Append($"(ExecuteId={_UniqueId};)D{_CaliChannels[i].ChannelID}{caliRet}:校准次数_{_CaliChannels[i].AdjustCount + _CaliChannels[i].AdjustCount};");
            }

            InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.Misc);

            message = messageSB.ToString();
            LogInfo(message);
            Boolean isAllOk = _CaliChannels.All(ch => ch.Status == CaliStatus.Ok);
            return isAllOk ? BatchTaskPartResult.Succeed : BatchTaskPartResult.ErrorGeneral;
        }

    }
}


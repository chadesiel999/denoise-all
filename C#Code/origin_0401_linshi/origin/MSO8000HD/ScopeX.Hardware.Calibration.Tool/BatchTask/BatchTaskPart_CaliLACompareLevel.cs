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
    public class BatchTaskPart_CaliLACompareLevel : BatchTaskPartBase
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
            public Int32 ChannelID;
            public CaliStage CaliStage = CaliStage.HighLevel;
            public CaliStatus Status = CaliStatus.Working;

            public (Int32 Up,Int32 Down) LastHigh = (0,0);    //寻找高电平边界的值范围
            public (Int32 Up,Int32 Down) LastLow = (0,0);     //寻找低电平边界的值范围
            public Int32 AdjustCount; //调整计数

            /// <summary>
            /// 当前DAC寄存器实际发送的值，前提比较电平设为0
            /// </summary>
            public Int32 RegValue
            {
                get
                {
                    Int32 comparetorindex = ChannelID / 4;
                    return MiscData.Default[(Int32)MiscDefine.LA_CaliDataBegin + comparetorindex * 2];
                }
                set
                {
                    Int32 comparetorindex = ChannelID / 4;
                    MiscData.Default[(Int32)MiscDefine.LA_CaliDataBegin + comparetorindex * 2] = value;//base
                    AdjustCount++;
                }
            }

            public void FreshLastHigh(Byte[] data)
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

            public void FreshLastLow(Byte[] data)
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
        //调整的Dac的位数
        private readonly Int32 DACBits;
        //输入参数
        private List<CaliChnlInfo> _CaliChannels = new List<CaliChnlInfo>();
        private Int64 _SourceVoltMv = 0;
        private Int64 _SuggestBase = 0;
        private Int64 _SuggestRatio = 0;
        private Int64 _UniqueId = 0;


        public BatchTaskPart_CaliLACompareLevel()
        {
            DACBits = ServerDomainConstants.ProductType switch
            {
                ProductType.JiHe_MSO8000HD => 16,
                ProductType.JiHe_MSO8000X => 16,
                _ => 12,
            };
        }

        public override String FuncionDescription
        {
            get => $"LA比较电平校准";
        }
        public override String ParametersDescription
        {
            get
            {
                Int32 argIndex = 1;
                return $"第{argIndex++}个参数：要校准的通道，从0开始(表示D0通道),可以多通道校准(1|2);{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数：校准信号的幅度值，用mv为单位;{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数：建议的0V校准值;{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数：建议的1V偏移值;{System.Environment.NewLine}";
            }
        }

        public override String Example
        {
            get => $"BatchTaskPart_CaliLACompareLevel 0|4 ,4000";
        }

        private Boolean AnalyParameter(String parameter)
        {
            String[] paramlist = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramlist.Length < 4)
            {
                return false;
            }

            //[0]: channel list
            String[] channelidlist = paramlist[0].Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (channelidlist.Length == 0)
            {
                return false;
            }

            //[1]:幅度值，用mV为单位
            _SourceVoltMv = BaseHelper.TryConvertToLong(paramlist[1]);
            if (_SourceVoltMv == 0)
            {
                return false;
            }
            //[2]:建议的0V校准值
            _SuggestBase = BaseHelper.TryConvertToLong(paramlist[2]);
            if (_SuggestBase < 0)
            {
                return false;
            }
            //[3]:建议的1V偏移值
            _SuggestRatio = BaseHelper.TryConvertToLong(paramlist[3]);
            if (_SuggestRatio < 0)
            {
                return false;
            }
            //校验参数有效性
            int defaulthighup = (Int32)(_SuggestBase + BaseErrorMax + _SourceVoltMv / ProbeAtt/ 1000D / 2 * (_SuggestRatio + RatioErrorMax));
            int defaulthighdown = (Int32)(_SuggestBase - BaseErrorMax + _SourceVoltMv / ProbeAtt/ 1000D / 2 * (_SuggestRatio - RatioErrorMax));
            int defaultlowup = (Int32)(_SuggestBase + BaseErrorMax - _SourceVoltMv / ProbeAtt/ 1000D / 2 * (_SuggestRatio - RatioErrorMax));
            int defaultlowdown = (Int32)(_SuggestBase - BaseErrorMax - _SourceVoltMv / ProbeAtt/ 1000D / 2 * (_SuggestRatio + RatioErrorMax));
            if(defaulthighup < 0 || defaulthighup >= Math.Pow(2, DACBits) ||
               defaulthighdown < 0 || defaulthighdown >= Math.Pow(2, DACBits) ||
               defaultlowup < 0 || defaultlowup >= Math.Pow(2, DACBits) ||
               defaultlowdown < 0 || defaultlowdown >= Math.Pow(2, DACBits))
            {
                return false;
            }
            else
            {
                if(defaulthighup - defaulthighdown <10 || 
                   defaultlowup - defaultlowdown <10 )
                {
                    return false;
                }
            }

            //初始化要校准的通道对象集合
            foreach (String channelId in channelidlist)
            {
                Int32 id = BaseHelper.TryConvertToInt(channelId);
                if (id < 0 || id >= ServerDomainConstants.LAChannelCount)
                {
                    return false;
                }
                CaliChnlInfo perchannel = new CaliChnlInfo();
                perchannel.ChannelID = id;
                //优化上下限值
                
                perchannel.LastHigh.Up = defaulthighup;
                perchannel.LastHigh.Down = defaulthighdown;
                perchannel.LastLow.Up = defaultlowup;
                perchannel.LastLow.Down = defaultlowdown;
                _CaliChannels.Add(perchannel);
            }

            return _CaliChannels.Count != 0;
        }

        public override Boolean SetParameter(XmlScpiCmd? xmlScpiCmd, String parameter)
        {
            if (xmlScpiCmd == null)
                return false;
            base.SetParameter(xmlScpiCmd, parameter);
            String[]? myName_ParameterPair = BaseHelper.SplitClassNameAndParameter(xmlScpiCmd.ProgramFuncName.Trim());
            if (myName_ParameterPair == null)
                return false;
            return AnalyParameter(myName_ParameterPair[1]);
        }

        private void LogInfo(String info)
        {
            Utilities.Logger.LogCaliInfo(_UniqueId, "Log", "BatchTaskPart_CaliLACompareLevel", info);
        }

        public override BatchTaskPartResult Exec(Double overtimeOfSecond, out String message, CancellationTokenSource? cancelTokenSrc = null)
        {
            message = String.Empty;
            StringBuilder messagesb = new StringBuilder();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            _UniqueId = DateTime.Now.Ticks;
            messagesb.Append($"(ExecuteId={_UniqueId};)");

            //记录执行信息
            StringBuilder chnlstext = new StringBuilder();
            _CaliChannels.ForEach(chnl => chnlstext.Append("Ch" + chnl.ChannelID + ","));
            LogInfo($"BatchTaskPart_CaliLACompareLevel:chnls_{chnlstext}; SourceVoltMv_{_SourceVoltMv};");

            //DAC寄存器发默认值
            _CaliChannels.ForEach(chnl => chnl.RegValue = (chnl.LastHigh.Up + chnl.LastHigh.Down) / 2);
            Thread.Sleep(1000); //等待信号源输出稳定

            while (_CaliChannels.Any(ch => ch.Status == CaliStatus.Working))
            {
                //*取消处理
                try
                {
                    cancelTokenSrc?.Token.ThrowIfCancellationRequested();
                }
                catch
                {
                    message = messagesb.ToString() + "任务被取消！";
                    return BatchTaskPartResult.Cancel;
                }
                if (stopwatch.ElapsedMilliseconds > overtimeOfSecond * 1000)
                {
                    message = messagesb.ToString() + "超时退出";
                    return BatchTaskPartResult.ErrorOvertime;
                }

                //*设置数据发送
                InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.Misc);
                Thread.Sleep(800);


                //获取波形数据
                List<Byte[]>? allChannelData = InstrumentInteract.DoFactory_WaveData_LA(currInstrumentSession, 6000);
                if (allChannelData == null)
                {
                    message = messagesb.ToString() + "数据读取错误！";
                    return BatchTaskPartResult.ErrorFatal;
                }

                //*校准
                foreach (var chnl in _CaliChannels)
                {
                    if(chnl.Status != CaliStatus.Working)
                        continue;

                    LogInfo($"ChannelID_{chnl.ChannelID},Status_{chnl.Status},CaliStage_{chnl.CaliStage},RegValue_{chnl.RegValue}," +
                        $"HighUp_{chnl.LastHigh.Up},HighDown_{chnl.LastHigh.Down},LowUp_{chnl.LastLow.Up},LowDown_{chnl.LastLow.Down}");

                    //先校准高电平(二分法)
                    if (chnl.CaliStage == CaliStage.HighLevel)
                    {
                        chnl.FreshLastHigh(allChannelData[chnl.ChannelID]);
                        //当up与down之间的差<=1时，认为中间值对应电平
                        if(chnl.LastHigh.Up - chnl.LastHigh.Down <= 1)
                        {
                            chnl.CaliStage = CaliStage.LowLevel;
                        }
                        else
                        {
                            chnl.RegValue = (chnl.LastHigh.Up + chnl.LastHigh.Down) / 2;
                        }
                    }
                    else //再校准低电平
                    {
                        chnl.FreshLastLow(allChannelData[chnl.ChannelID]);
                        if (chnl.LastLow.Up - chnl.LastLow.Down <= 1)
                        {
                            chnl.Status = CaliStatus.Ok;
                            LogInfo($"ChannelID_{chnl.ChannelID},Status_{chnl.Status},CaliStage_{chnl.CaliStage},RegValue_{chnl.RegValue}," +
                                $"HighUp_{chnl.LastHigh.Up},HighDown_{chnl.LastHigh.Down},LowUp_{chnl.LastLow.Up},LowDown_{chnl.LastLow.Down}");
                        }
                        else
                        {
                            chnl.RegValue = (chnl.LastLow.Up + chnl.LastLow.Down) / 2;
                        }
                    }
                }
            }

            foreach (var chnl in _CaliChannels)
            {
                //*配置校准值
                Int32 comparetorIndex = chnl.ChannelID / 4;
                Double baseValue = (chnl.LastHigh.Up + chnl.LastLow.Down) / 2D;
                Double ratio = (chnl.LastHigh.Up - chnl.LastLow.Down) / (_SourceVoltMv / ProbeAtt/ 1000D );
                MiscData.Default[(Int32)MiscDefine.LA_CaliDataBegin + comparetorIndex * 2] = (Int32)Math.Round(baseValue);
                MiscData.Default[(Int32)MiscDefine.LA_CaliDataBegin + comparetorIndex * 2 + 1] = (Int32)Math.Round(ratio);

                if (Math.Abs(_SuggestBase - baseValue) > BaseErrorMax || Math.Abs(_SuggestRatio - ratio) > RatioErrorMax)
                    chnl.Status = CaliStatus.Failure;
                //*校准信息输出
                String caliRet = (chnl.Status == CaliStatus.Ok) ? "[OK]" : "[Failure]";
                messagesb.Append($"D{chnl.ChannelID}{caliRet}:校准次数_{chnl.AdjustCount};");
            }
            InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.Misc);

            message = messagesb.ToString();
            LogInfo(message);
            Boolean isallok = _CaliChannels.All(ch => ch.Status == CaliStatus.Ok);
            return isallok ? BatchTaskPartResult.Succeed : BatchTaskPartResult.ErrorGeneral;
        }

    }
}   
   

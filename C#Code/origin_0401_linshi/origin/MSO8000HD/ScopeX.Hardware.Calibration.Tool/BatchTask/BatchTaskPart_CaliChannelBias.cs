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
using System.Xml;
using static ScopeX.Hardware.Calibration.Data.Base.ProductDataTranslate_MSO8000X;

namespace ScopeX.Hardware.Calibration.Tool.BatchTask
{
    public class BatchTaskPart_CaliOffset : BatchTaskPartBase
    {
        enum CaliStatus
        {
            Working,
            Failure,
            Ok,
        }

        class CaliChnlInfo
        {
            public int channelID;
            public int yLevelID;
            public int Impedance;
            public double CurrentAvg;   //当前通道平均值
            public int AdjustCount; //调整计数
            public AnalogChannelItem_Base analogChannelItem_Base;
            public ChnlParamsKeyMap chnlParams;
            public string paramName;
            public (double OffsetFrom0Div, uint CtrlWord, double StepRatio)? NearestPoint;

            public CaliStatus Status = CaliStatus.Working;
            public uint Cali_Channel3divOffset_P
            {
                get => (uint)ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlParams).Value.Bias_Pos3Div;
                set
                {
                    ProductDataTranslate_MSO8000X.SetAnalogChannelParamValue("Bias_Pos3Div", (uint)value, paramName);
                    //ChannelPerScaleItem perScaleItem = ChannelParams.Default[channelID, Impedance, yLevelID];
                    //perScaleItem.OffsetPreceding_3Div = value;
                    //ChannelParams.Default[channelID, Impedance, yLevelID] = perScaleItem;
                }
            }

            public uint Cali_Channel3divOffset_N
            {
                get => (uint)ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnlParams).Value.Bias_Neg3Div;
                set
                {
                    ProductDataTranslate_MSO8000X.SetAnalogChannelParamValue("Bias_Neg3Div", (uint)value, paramName);
                    //ChannelPerScaleItem perScaleItem = ChannelParams.Default[channelID, Impedance, yLevelID];
                    //perScaleItem.OffsetPreceding_3Div = value;
                    //ChannelParams.Default[channelID, Impedance, yLevelID] = perScaleItem;
                }
            }
        }

        //前级偏3格放大系数
        private uint AmpFactor = 100_000_000;
        //各阻抗对应的偏执(off)与偏移(move)的系数比例;
        private (double low, double high) OffMoveRatioDefine
        {
            get
            {
                return ServerDomainConstants.AnaChnlType switch
                {
                    AnaChnlType.ANA_2D5G => (6.83 / 0.55, 1.958 / 0.573),//MSO7000X系数
                    AnaChnlType.ANA_5G => (15 / 5.6, 8.2 / 2.4),//MSO8000HD系数
                    AnaChnlType.ANA_8G => (1, 1),//MSO8000HD系数
                    _ => (1, 1),//ANA_8G
                };
            }
        } 
        //private readonly (double low, double high) OffMoveRatioDefine = 

        private List<CaliChnlInfo> _CaliChannels = new List<CaliChnlInfo>();
        private int _Impedance;
        private double _CurrentOffMoveRatio;
        private double _InputLevelBymV = 0;
        private long _SourceVoltMv = 0;
        private long _MaxWaitMilliseconds = 0;
        private long _UniqueId = 0;
        private int _CaliType = 0;

        public override string FuncionDescription
        {
            get => $"偏执校准";
        }
        public override string ParametersDescription
        {
            get
            {
                int argIndex = 1;
                return $"第{argIndex++}个参数：表示通道，其中1|2 编号从1开始;{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数，表示高阻还是低阻;LOW表示低阻(50欧姆);HIGH表示高阻;{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数：以mV为单位的档位值，浮点数;{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数，偏移输入的理论电压值，用V为单位;{System.Environment.NewLine}" +
                        //$"第{argIndex++}个参数，表示前级偏校准还是后级偏校准，0表示前级偏，1表示后级偏;{System.Environment.NewLine}" +
                        $"第{argIndex++}个参数，最大等待时间,单位mS;{System.Environment.NewLine}";
            }
        }

        public override string Example
        {
            get => $"BatchTaskPart_CaliOffset 1|2 ,LOW,1000,5000,0,40000";
        }

        private bool AnalyParameter(String parameter)
        {
            String[] paramlist = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (paramlist.Length < 4)
            {
                return false;
            }
            //channel list
            //[0]: channel list
            String[] channelIDList = paramlist[0].Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (channelIDList.Length == 0)
            {
                return false;
            }
            //[1]:高阻、低阻，低阻用Low表示，High表示高阻
            if (paramlist[1].ToUpper() == "HIGH")
            {
                _Impedance = 0;
                _CurrentOffMoveRatio = OffMoveRatioDefine.high;
            }
            else
            {
                _Impedance = 1;
                _CurrentOffMoveRatio = OffMoveRatioDefine.low;
            }

            //[2]:幅度档，用mV为单位
            _InputLevelBymV = BaseHelper.TryConvertToDouble(paramlist[2]);
            if (_InputLevelBymV == 0)
            {
                return false;
            }
            Double inputLevelByuV = _InputLevelBymV * 1000;//mv=>uV
            Int32 inputLevelID = Array.IndexOf<Int32>(AnalogChanneScaleDefine.PhyChCoarseLevelTableByuV.ToArray(), (Int32)inputLevelByuV);
            if (inputLevelID < 0)
            {
                return false;
            }
            //[3]：偏移输入的理论电压值，V为单位
            _SourceVoltMv = (Int32)(BaseHelper.TryConvertToDouble(paramlist[3]) * 1000);
            //if (_SourceVoltMv < 0)
            //{
            //    return false;
            //}
            //[4]校准类型，
            //_CaliType = BaseHelper.TryConvertToInt(paramList[4]);

            //[4]: 最大等待时间
            _MaxWaitMilliseconds = BaseHelper.TryConvertToLong(paramlist[4]) * 1000;
            if (_MaxWaitMilliseconds == 0)
            {
                return false;
            }

            //初始化要校准的通道对象集合
            foreach (String channelid in channelIDList)
            {
                Int32 id = BaseHelper.TryConvertToInt(channelid);
                if (id <= 0 || id > ServerDomainConstants.AnalogChannelCount)
                {
                    return false;
                }
                CaliChnlInfo perchannel = new CaliChnlInfo();
                perchannel.channelID = id - 1;
                perchannel.yLevelID = inputLevelID;
                perchannel.Impedance = _Impedance;
                perchannel.chnlParams = new ChnlParamsKeyMap((ChannelId)perchannel.channelID, _Impedance == 0 ? true : false, (UInt32)_InputLevelBymV);
                perchannel.paramName = ProductDataTranslate_MSO8000X.GenerateChnlParamsKey(perchannel.chnlParams);
                _CaliChannels.Add(perchannel);
            }
            if (_CaliChannels.Count == 0)
            {
                return false;
            }
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
            Utilities.Logger.LogCaliInfo(_UniqueId, "Log", "BatchTaskPart_CaliOffset", info);
        }

        public override void SetDebugVariantStatus(bool status)
        {
            CommonMethod.Set_CorrectTiAdc(currInstrumentSession, status);
            CommonMethod.SetChannelDelay(currInstrumentSession, status);
        }

        public override BatchTaskPartResult Exec(double overtimeOfSecond, out string message, CancellationTokenSource? cancelTokenSrc = null)
        {
            CommonMethod.RefreshConstDataFromServer(currInstrumentSession);
            message = String.Empty;
            StringBuilder messagesb = new StringBuilder();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            StringBuilder chnlsText = new StringBuilder();
            _CaliChannels.ForEach(chnl => chnlsText.Append("Ch" + chnl.channelID + ","));

            _UniqueId = DateTime.Now.Ticks;
            LogInfo($"(ExecuteId={_UniqueId};)BatchTaskPart_CaliOffset_Start:chnls_{chnlsText};" +
                $"inputLevelBymV_{_InputLevelBymV};Impedance_{_CaliChannels[0].Impedance};");
            Thread.Sleep(1000); //等待信号源输出稳定

            #region 设置初始值
            foreach (var chnl in _CaliChannels)
            {
                var perscaleitem = ProductDataTranslate_MSO8000X.GetChnlParamsItem(chnl.chnlParams);
                double deltacw = perscaleitem.Value.Offset_Pos3Div / _CurrentOffMoveRatio;
                if(ServerDomainConstants.AnaChnlType == AnaChnlType.ANA_8G)
                {
                    if (_SourceVoltMv >= 0)
                    {
                        chnl.Cali_Channel3divOffset_P = (UInt32)(perscaleitem.Value.Bias_Pos3Div);
                    }
                    else
                    {
                        chnl.Cali_Channel3divOffset_N = (UInt32)(perscaleitem.Value.Bias_Neg3Div);
                    }
                }
                else
                {
                    if (_SourceVoltMv >= 0)
                    {
                        chnl.Cali_Channel3divOffset_P = (UInt32)(deltacw / (_InputLevelBymV * 3 * 1000) * 100_000_000);
                    }
                    else
                    {
                        chnl.Cali_Channel3divOffset_N = (UInt32)(deltacw / (_InputLevelBymV * 3 * 1000) * 100_000_000);
                    }
                }
                chnl.AdjustCount = 0;
            }
            #endregion 设置初始值

            #region 校准前级偏

            while (_CaliChannels.Any(ch => ch.Status == CaliStatus.Working))
            {
                //取消处理
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
                if (stopwatch.ElapsedMilliseconds > _MaxWaitMilliseconds)
                {
                    message = $"(ExecuteId={_UniqueId};)超时退出";
                    LogInfo(message);
                    return BatchTaskPartResult.ErrorOvertime;
                }

                //设置数据发送
                InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.AnalogParams);
                Thread.Sleep(1000);
                InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.AnalogParams);
                Thread.Sleep(1000);

                //获取波形数据
                List<UInt16[]>? allChannelData = CommonMethod.Factory_WaveData_Channel(currInstrumentSession, ServerDomainConstants.PerAdcCoreDataCount);
                if (allChannelData == null)
                {
                    message = $"(ExecuteId={_UniqueId};)数据读取错误！";
                    LogInfo(message);
                    return BatchTaskPartResult.ErrorFatal;
                }

                //并行计算，计算平均值
                Parallel.For(0, allChannelData.Count, dataId =>
                {
                    var chnl = _CaliChannels.FirstOrDefault(chnl => chnl.channelID == dataId);
                    if (chnl != null && chnl.Status == CaliStatus.Working)
                        chnl.CurrentAvg = allChannelData[dataId].Select(d => (int)d).Average();
                });

                //调整前级偏
                foreach (var chnl in _CaliChannels)
                {
                    if (chnl.Status != CaliStatus.Working)
                        continue;

                    //校准策略说明：
                    //当偏移>=3.5Div的时候，直接用当前的Delta电压，与Delta控制字算斜率;
                    //当有偏移<3.5Div的时候，看是否新的斜率的结果偏移更大
                    //如果更大，则斜率减半
                    //如果更小，则继续
                    uint calichannel3divoffset = (_SourceVoltMv >= 0) ? chnl.Cali_Channel3divOffset_P : chnl.Cali_Channel3divOffset_N;

                    double curroffsetfromOdiv = chnl.CurrentAvg - Math.Pow(2, ServerDomainConstants.AdcBits) / 2;
                    LogInfo($"chnlId_{chnl.channelID};" +
                        $"Channel3divOffset_{calichannel3divoffset};currOffsetFromODiv_{curroffsetfromOdiv}");

                    //记录最近的点，只有当偏移小于3.5Div才记录
                    if (Math.Abs(curroffsetfromOdiv) < ServerDomainConstants.SAMPS_PER_YDIV * 3.5)
                    {
                        if (chnl.NearestPoint == null || curroffsetfromOdiv < Math.Abs(chnl.NearestPoint.Value.OffsetFrom0Div))
                        {
                            chnl.NearestPoint = (curroffsetfromOdiv, calichannel3divoffset, 1);
                        }
                        else //新斜率偏移更大,增量减半
                        {
                            double newratio = chnl.NearestPoint.Value.StepRatio / 2;
                            chnl.NearestPoint = (chnl.NearestPoint.Value.OffsetFrom0Div, chnl.NearestPoint.Value.CtrlWord, newratio);
                        }
                    }

                    //使用最靠近的点来计算理想控制字
                    double calcoffsetfromOdiv = chnl.NearestPoint?.OffsetFrom0Div ?? curroffsetfromOdiv;
                    double calcctrlword = chnl.NearestPoint?.CtrlWord ?? calichannel3divoffset;
                    double calcratio = chnl.NearestPoint?.StepRatio ?? 1;
                     
                    double counteractvoltmv = _SourceVoltMv - (calcoffsetfromOdiv / ServerDomainConstants.SAMPS_PER_YDIV) * _InputLevelBymV;
                    double deltadacreg = _SourceVoltMv * 1000 * (calcctrlword / 100_000_000D);
                    double newctrlword = deltadacreg / (counteractvoltmv * 1000) * AmpFactor;
                    if (_SourceVoltMv >= 0)
                        chnl.Cali_Channel3divOffset_P = (uint)Math.Round(calcctrlword + (newctrlword - calcctrlword) * calcratio);
                    else
                        chnl.Cali_Channel3divOffset_N = (uint)Math.Round(calcctrlword + (newctrlword - calcctrlword) * calcratio);

                    chnl.AdjustCount++;

                    //校准终止条件，avg值在[-1div,1div]内
                    if (Math.Abs(curroffsetfromOdiv) < ServerDomainConstants.SAMPS_PER_YDIV / 10)
                    {
                        chnl.Status = CaliStatus.Ok;
                    }
                }

            }

            #endregion


            foreach (var chnl in _CaliChannels)
            {
                String ret = (chnl.Status == CaliStatus.Ok) ? "[OK]" : "[Failure]";
                messagesb.Append($"CH{chnl.channelID + 1}{ret}:校准次数_{chnl.AdjustCount};");
            }
            message = $"(ExecuteId={_UniqueId};)" + messagesb.ToString();
            LogInfo(message);
            Boolean isAllOk = _CaliChannels.All(ch => ch.Status == CaliStatus.Ok);
            return isAllOk ? BatchTaskPartResult.Succeed : BatchTaskPartResult.ErrorGeneral;
        }


    }
}


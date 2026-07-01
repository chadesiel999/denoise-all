using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScopeX.ComModel;
using ScopeX.Hardware.Calibration.Data.Base;
using ScopeX.Hardware.Calibration.Tool.Base;

namespace ScopeX.Hardware.Calibration.Tool
{
    public class BatchTaskPart_ExtTrigger: BatchTaskPartBase
    {
        (int zero, int hundred) _DefineIndex;
        IInstrumentSession? _SrcInstrument;

        //参数相关
        private ChannelId _ParamChannelId = ChannelId.Ext;
        private TriggerImpedance _ParamTriggerImpedance = TriggerImpedance.High1M;
        private TriggerCoupling _ParamTriggerCoupling = TriggerCoupling.DC;
        private int _ParamAmpByMv = 1000;
        private int _ParamErrThreshByUv = 1000;
        private string _ParamSrcAddr = "source scpi addr";

        private readonly List<ChannelId> _ChannelIdOptions = new List<ChannelId>()
        {
            ChannelId.Ext, 
            ChannelId.Ext5
        };
        private readonly List<TriggerImpedance> _TriggerImpedanceOptions = new List<TriggerImpedance>()
        {
            TriggerImpedance.High1M, 
            TriggerImpedance.Low50 
        };
        private readonly List<TriggerCoupling> _TriggerCouplingOptions = new List<TriggerCoupling>()
        {
            TriggerCoupling.DC, 
            TriggerCoupling.AC, 
            TriggerCoupling.LFR, 
            TriggerCoupling.HFR
        };

        private Boolean _ParamValid = false;

        public override string FuncionDescription
        {
            get => "校准外触发通道";
        }

        public override string ParametersDescription
        {
            get 
            {
                StringBuilder channelIdOptions = new StringBuilder();
                StringBuilder impedanceOptions = new StringBuilder();
                StringBuilder couplingOptions = new StringBuilder();

                _ChannelIdOptions.ForEach(op => channelIdOptions.Append(op.ToString() + ","));
                _TriggerImpedanceOptions.ForEach(op => impedanceOptions.Append(op.ToString() + ","));
                _TriggerCouplingOptions.ForEach(op => couplingOptions.Append(op.ToString() + ","));


                return $"第1个参数: 外触发通道({channelIdOptions.ToString()});{System.Environment.NewLine}" +
                       $"第2个参数: 高低阻({impedanceOptions.ToString()}){System.Environment.NewLine}" +
                       $"第3个参数: 耦合方式({couplingOptions.ToString()}){System.Environment.NewLine}" +
                       $"第4个参数: 信号幅度,单位Mv{System.Environment.NewLine}" +
                       $"第5个参数: 误差阈值,单位Uv{System.Environment.NewLine}" +
                       $"第6个参数: 信号源地址{System.Environment.NewLine}";
            } 
        }

        public override string Example
        {
            get =>  $"BatchTaskPart_MiscData " +
                    $"{_ChannelIdOptions[0].ToString()}, " +
                    $"{_TriggerImpedanceOptions[0].ToString()}, " +
                    $"{_TriggerCouplingOptions[0].ToString()}, " +
                    $"{_ParamAmpByMv.ToString()}, " +
                    $"{_ParamErrThreshByUv.ToString()}"+
                    $"{_ParamSrcAddr.ToString()}";
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

        public override BatchTaskPartResult Exec(double overtimeBySec, out string outMsg, CancellationTokenSource? cancelTokenSrc = null)
        {
            if(!_ParamValid)
            {
                outMsg = "参数错误!";
                return BatchTaskPartResult.ErrorParameter;
            }

            BatchTaskPartResult ret = BatchTaskPartResult.Succeed;
            outMsg = string.Empty;
            Stopwatch stopwatch = new Stopwatch();
            long maxWaitMs = overtimeBySec > 0 ? (long)(1000 * overtimeBySec) : 60 * 1000;
            StringBuilder caliMsg = new StringBuilder();

            //校准2个项目，0V校准及100V校准
            CaliStateManager stateManager = new CaliStateManager(2);
            stateManager.GetCaliState(0).VolType = CaliStateManager.VolType.VolZero;
            stateManager.GetCaliState(1).VolType = CaliStateManager.VolType.VolHundred;
            _DefineIndex = GetZeroAndHundredIndexs();

            stopwatch.Start();
            //校准0V及100V
            ret = CaliEntry(stateManager.GetCaliState(0), _DefineIndex.zero, maxWaitMs, stopwatch, caliMsg, cancelTokenSrc);
            if (ret != BatchTaskPartResult.Succeed)
            {
                outMsg = caliMsg.ToString();
                return ret;
            }

            ret = CaliEntry(stateManager.GetCaliState(1), _DefineIndex.hundred, maxWaitMs, stopwatch, caliMsg, cancelTokenSrc);
            if (ret != BatchTaskPartResult.Succeed)
            {
                outMsg = caliMsg.ToString();
                return ret;
            }
               

            if (!stateManager.IsAllSucceed())
            {
                if (stateManager.IsAllCompleted())
                    ret = BatchTaskPartResult.ErrorGeneral;
                else
                    ret = BatchTaskPartResult.ErrorOvertime;
            }
            outMsg = caliMsg.ToString();
            return ret;
        }

        private bool AnalyParameter(string parameter)
        {
            try
            {
                string[] paramList = parameter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                _ParamChannelId = _ChannelIdOptions.First(op => paramList[0].Trim() == op.ToString());
                _ParamTriggerImpedance = _TriggerImpedanceOptions.First(op => paramList[1].Trim() == op.ToString());
                _ParamTriggerCoupling = _TriggerCouplingOptions.First(op => paramList[2].Trim() == op.ToString());

                _ParamAmpByMv = Int32.Parse(paramList[3].Trim());
                _ParamErrThreshByUv = Int32.Parse(paramList[4].Trim());
                _ParamSrcAddr = paramList[5].Trim();
                _SrcInstrument = InstrumentSessionEngine.TryGetSession(_ParamSrcAddr, "500", null, out string msg);
                if(_SrcInstrument == null)
                {
                    throw new ArgumentException();
                }
            }
            catch
            {
                return _ParamValid = false;
            }
            return _ParamValid = true;
        }

        private (int, int) GetZeroAndHundredIndexs()
        {
            return (_ParamChannelId, _ParamTriggerImpedance, _ParamTriggerCoupling) switch
            {
                //Ext
                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.DC) => ((int)MiscDefine.ExtTrigger_HighImp_DC_ZeroDelta_uV, (int)MiscDefine.ExtTrigger_HighImp_DC_100VDelta_uV),
                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.AC) => ((int)MiscDefine.ExtTrigger_HighImp_AC_ZeroDelta_uV, (int)MiscDefine.ExtTrigger_HighImp_AC_100VDelta_uV),
                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.LFR) => ((int)MiscDefine.ExtTrigger_HighImp_LFR_ZeroDelta_uV, (int)MiscDefine.ExtTrigger_HighImp_LFR_100VDelta_uV),
                (ChannelId.Ext, TriggerImpedance.High1M, TriggerCoupling.HFR) => ((int)MiscDefine.ExtTrigger_HighImp_HFR_ZeroDelta_uV, (int)MiscDefine.ExtTrigger_HighImp_HFR_100VDelta_uV),

                (ChannelId.Ext, TriggerImpedance.Low50, TriggerCoupling.DC) => ((int)MiscDefine.ExtTrigger_LowImp_DC_ZeroDelta_uV, (int)MiscDefine.ExtTrigger_LowImp_DC_100VDelta_uV),
                //Ext5
                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.DC) =>  ((int)MiscDefine.Ext5Trigger_HighImp_DC_ZeroDelta_uV,  (int)MiscDefine.Ext5Trigger_HighImp_DC_100VDelta_uV),
                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.AC) =>  ((int)MiscDefine.Ext5Trigger_HighImp_AC_ZeroDelta_uV,  (int)MiscDefine.Ext5Trigger_HighImp_AC_100VDelta_uV),
                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.LFR) => ((int)MiscDefine.Ext5Trigger_HighImp_LFR_ZeroDelta_uV, (int)MiscDefine.Ext5Trigger_HighImp_LFR_100VDelta_uV),
                (ChannelId.Ext5, TriggerImpedance.High1M, TriggerCoupling.HFR) => ((int)MiscDefine.Ext5Trigger_HighImp_HFR_ZeroDelta_uV, (int)MiscDefine.Ext5Trigger_HighImp_HFR_100VDelta_uV),

                (ChannelId.Ext5, TriggerImpedance.Low50, TriggerCoupling.DC) =>  ((int)MiscDefine.Ext5Trigger_LowImp_DC_ZeroDelta_uV , (int)MiscDefine.Ext5Trigger_LowImp_DC_100VDelta_uV ),
                (_, _, _) => throw new ArgumentException(),
            };
        }


        private void SetExtTriggerPos(Int32 posByMv)
        {
            string scpiCMD = ":TRIG:EDG:LEV " + posByMv + "mV";
            currInstrumentSession!.WriteString(scpiCMD);
        }

        private List<Boolean> SrcGreaterThanTriggerVoltageRets()
        {
            string scpiCMD = ":FACT:CALI:SPEC:DATA? GetExtTrigCompareResults";
            currInstrumentSession!.WriteString(scpiCMD);
            string rets = currInstrumentSession!.ReadString();
            //0-输入信号 > 触发电平;1-输入信号 < 触发电平;
            return rets.Select(s => s == '0').ToList();
        }

        /// <summary>
        /// 设置信号源的偏执
        /// </summary>
        /// <param name="offsetUv"></param>
        private void SetSrcOffset(int offsetUv)
        {
            string scpiCMD = ":SOURce1:VOLTage:OFFSet " + (offsetUv / 1000_000D).ToString();
            _SrcInstrument!.WriteString(scpiCMD);
        }

        #region 具体校准方法

        private BatchTaskPartResult CaliEntry(CaliStateManager.CaliState caliState, int defineIndex, double maxWaitMs, Stopwatch stopwatch,
            StringBuilder caliMsg, CancellationTokenSource? cancelTokenSrc = null)
        {
            //说明100V的误差值使用1V的误差换算出来的;
            BatchTaskPartResult ret = BatchTaskPartResult.Succeed;
            int originOffsetUvZero = -1 * _ParamAmpByMv * 1000 / 2;
            int originOffsetUvHundred = originOffsetUvZero + 1000_000;
            int currOffsetUv = (caliState.VolType == CaliStateManager.VolType.VolZero) ? originOffsetUvZero: originOffsetUvHundred;

            caliState.SetLastOffsetUv(currOffsetUv - 500_000, currOffsetUv + 500_000);
            int oldData = MiscData.Default[defineIndex];
            MiscData.Default[defineIndex] = 0;
            SetExtTriggerPos((caliState.VolType == CaliStateManager.VolType.VolZero) ? 0 : 1000);
            InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.Misc);
            caliMsg.AppendLine($"开始{caliState.VolType}的校准:");
            while (caliState.CurrFlag == CaliStateManager.Flag.Continue)
            {
                SetSrcOffset(currOffsetUv);
                Thread.Sleep(100);
                try
                {
                    cancelTokenSrc?.Token.ThrowIfCancellationRequested();
                }
                catch
                {
                    ret = BatchTaskPartResult.Cancel;
                    break;
                }

                if(caliState.VolType == CaliStateManager.VolType.VolZero)
                    currOffsetUv = CaliZero(caliState, currOffsetUv);
                else //caliState.VolType == CaliStateManager.VolType.VolHundred
                    currOffsetUv = CaliHundred(caliState, currOffsetUv);

                if (stopwatch.ElapsedMilliseconds > maxWaitMs)
                {
                    caliMsg.AppendLine("任务超时!");
                    ret = BatchTaskPartResult.ErrorOvertime;
                    break;
                }
            }

            if (ret == BatchTaskPartResult.Succeed)
            {
                MiscData.Default[defineIndex] = currOffsetUv - originOffsetUvZero;
                if (caliState.VolType != CaliStateManager.VolType.VolZero)
                {
                    int offsetUvZero = MiscData.Default[_DefineIndex.zero];
                    MiscData.Default[defineIndex] = (currOffsetUv - originOffsetUvHundred - offsetUvZero) * 100;
                }
                InstrumentInteract.CaliData_Send(currInstrumentSession, CaliDataType.Misc);
                caliMsg.AppendLine($"校准成功!当前校准值为:{MiscData.Default[defineIndex]}");
            }
            else
            {
                MiscData.Default[defineIndex] = oldData;
                caliMsg.AppendLine($"校准失败!当前校准值为:{MiscData.Default[defineIndex]}");
            }
            return ret;
        }

        private int CaliZero(CaliStateManager.CaliState caliState, int currOffset)
        {
            int ret = currOffset;
            //获取比较状态
            List<Boolean> cmpRets = SrcGreaterThanTriggerVoltageRets();
            //有效性判断
            if (currOffset <= caliState.LastOffsetUv.Down &&
                currOffset >= caliState.LastOffsetUv.Up)
            {
                caliState.CurrFlag = CaliStateManager.Flag.Failed;
            }

            if (cmpRets.All(r => !r))
            {
                caliState.LastOffsetUv.Down = currOffset;
            }
            else
            {
                caliState.LastOffsetUv.Up = currOffset;
            }

            //是否成功判断
            if (Math.Abs(caliState.LastOffsetUv.Up - caliState.LastOffsetUv.Down) <= _ParamErrThreshByUv + 1)
                caliState.CurrFlag = CaliStateManager.Flag.Succeed;

            if (caliState.CurrFlag == CaliStateManager.Flag.Continue)
                ret = (int)((caliState.LastOffsetUv.Down + caliState.LastOffsetUv.Up) / 2);

            return ret;
        }

        private int CaliHundred(CaliStateManager.CaliState caliState, int currOffset)
        {
            return CaliZero(caliState, currOffset);
        }

        #endregion 具体校准方法

        private class CaliStateManager
        {
            public enum Flag
            {
                Continue,
                Failed,
                Succeed,
            }

            public enum VolType
            {
                VolZero,
                VolHundred
            }

            public class CaliState
            {
                //使用过的<校准值,结果>的集合
                private List<KeyValuePair<int, bool>> _CaliRetPairs = new();

                /// <summary>
                /// 当前状态标识
                /// </summary>
                public Flag CurrFlag { set; get; } = Flag.Continue;

                /// <summary>
                /// 当前测试的电压
                /// </summary>
                public VolType VolType { set; get; }

                public (int Down, int Up) LastOffsetUv;

                public void SetLastOffsetUv(int down, int up)
                {
                    LastOffsetUv.Down = down;
                    LastOffsetUv.Up = up;
                }

                public int GetCaliCount() => _CaliRetPairs.Count;
            }

            //校准工作状态数组
            private CaliState[] _CaliStates;

            public CaliStateManager(int caliItemCount)
            {
                _CaliStates = new CaliState[caliItemCount];
                for (int caliId = 0; caliId < caliItemCount; caliId++)
                {
                    _CaliStates[caliId] = new CaliState();
                }
            }

            public CaliState GetCaliState(Int32 stateIndex)
            {
                return _CaliStates[stateIndex];
            }

             public Boolean IsAllSucceed()
            {
                foreach(var state in _CaliStates)
                {
                    if(state.CurrFlag != Flag.Succeed)
                        return false;
                }
                return true;
            }

            public Boolean IsAllCompleted()
            {
                foreach (var state in _CaliStates)
                {
                    if (state.CurrFlag == Flag.Continue)
                        return false;
                }
                return true;
            }
        }

    }
}
